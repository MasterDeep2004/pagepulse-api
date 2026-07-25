using FluentValidation;
using PagePulse.Api.Models;

namespace PagePulse.Api.Validators;

public class AuditRequestValidator : AbstractValidator<AuditRequest>
{
    public AuditRequestValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("URL is required.");

        RuleFor(x => x.Url)
            .Must(url =>
                Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp ||
                 uri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("Please enter a valid HTTP or HTTPS URL.");
    }
}