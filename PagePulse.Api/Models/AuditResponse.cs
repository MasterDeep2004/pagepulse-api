namespace PagePulse.Api.Models;

public class AuditResponse
{
    public string Url { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public string Title { get; set; } = string.Empty;

    public long ResponseTimeMs { get; set; }

    public bool Cached { get; set; }

    public string RequestId { get; set; } = string.Empty;
}