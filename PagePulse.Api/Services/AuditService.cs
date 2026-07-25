using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PagePulse.Api.Interfaces;
using PagePulse.Api.Models;
using System.Diagnostics;

namespace PagePulse.Api.Services;

public class AuditService : IAuditService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuditService> _logger;

    private static readonly SemaphoreSlim _semaphore = new(5);

    public AuditService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<AuditService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuditResponse> AuditAsync(string url)
    {
        await _semaphore.WaitAsync();

        _logger.LogInformation("Starting audit for {Url}", url);

        try
        {
            // Check cache first
            if (_cache.TryGetValue(url, out AuditResponse? cachedResponse))
            {
                cachedResponse!.Cached = true;

                _logger.LogInformation("Cache hit for {Url}", url);

                return cachedResponse;
            }

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var stopwatch = Stopwatch.StartNew();

            var response = await client.GetAsync(url);

            stopwatch.Stop();

            var html = await response.Content.ReadAsStringAsync();

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var title = document.DocumentNode
                .SelectSingleNode("//title")
                ?.InnerText ?? "No Title";

            var auditResponse = new AuditResponse
            {
                Url = url,
                StatusCode = (int)response.StatusCode,
                Title = title,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                Cached = false,
                RequestId = Guid.NewGuid().ToString()
            };

            var cacheMinutes =
                _configuration.GetValue<int>("CacheSettings:DurationMinutes");

            _cache.Set(
                url,
                auditResponse,
                TimeSpan.FromMinutes(cacheMinutes));

            _logger.LogInformation(
                "Audit completed for {Url} in {Time} ms",
                url,
                stopwatch.ElapsedMilliseconds);

            return auditResponse;
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Request timed out for {Url}", url);

            throw new Exception("The request timed out.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Audit failed for {Url}", url);

            throw new Exception($"Audit failed: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}