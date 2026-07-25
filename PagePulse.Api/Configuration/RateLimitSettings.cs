namespace PagePulse.Api.Configuration;

public class RateLimitSettings
{
    public int PermitLimit { get; set; }

    public int WindowSeconds { get; set; }
}