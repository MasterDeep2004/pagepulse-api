using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using PagePulse.Api.Interfaces;
using PagePulse.Api.Middleware;
using PagePulse.Api.Services;
using PagePulse.Api.Validators;


var options = new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Production
};

var builder = WebApplication.CreateBuilder(options);

builder.Configuration.Sources.Clear();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: false);
builder.Configuration.Sources
    .Where(s => s.GetType().Name.Contains("JsonConfigurationSource"))
    .ToList()
    .ForEach(s =>
    {
        if (s is Microsoft.Extensions.Configuration.Json.JsonConfigurationSource jsonSource)
        {
            jsonSource.ReloadOnChange = false;
        }
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.AddValidatorsFromAssemblyContaining<AuditRequestValidator>();

builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AuditLimiter", limiter =>
    {
        limiter.PermitLimit =
            builder.Configuration.GetValue<int>("RateLimitSettings:PermitLimit");

        limiter.Window =
            TimeSpan.FromSeconds(
                builder.Configuration.GetValue<int>("RateLimitSettings:WindowSeconds"));

        limiter.QueueLimit = 0;
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseRateLimiter();

app.MapControllers();

app.Run();
