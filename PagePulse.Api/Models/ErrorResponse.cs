namespace PagePulse.Api.Models;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;

    public string RequestId { get; set; } = string.Empty;
}