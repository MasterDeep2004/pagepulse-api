using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PagePulse.Api.Interfaces;
using PagePulse.Api.Models;

namespace PagePulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpPost]
    [EnableRateLimiting("AuditLimiter")]
    public async Task<IActionResult> Audit(
        [FromBody] AuditRequest request,
        [FromServices] IValidator<AuditRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _auditService.AuditAsync(request.Url);

        return Ok(result);
    }
}
