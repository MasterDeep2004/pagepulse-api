using PagePulse.Api.Models;

namespace PagePulse.Api.Interfaces;

public interface IAuditService
{
    Task<AuditResponse> AuditAsync(string url);
}