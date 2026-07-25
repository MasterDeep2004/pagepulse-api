using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using PagePulse.Api.Interfaces;
using PagePulse.Api.Middleware;
using PagePulse.Api.Services;
using PagePulse.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseRateLimiter();

app.MapControllers();

app.Run();