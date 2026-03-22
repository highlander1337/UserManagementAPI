
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Middlewares;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenAuthenticationMiddleware> _logger;
    private const string CorrelationHeader = "X-Correlation-ID";

    // For demo: token read from env var VALID_API_TOKEN or fallback to a default.
    private readonly string _validToken = Environment.GetEnvironmentVariable("VALID_API_TOKEN") ?? "12345";

    public TokenAuthenticationMiddleware(RequestDelegate next, ILogger<TokenAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow health or swagger endpoints if you want (optional), e.g.:
        // if (context.Request.Path.StartsWithSegments("/health")) { await _next(context); return; }

        var correlationId = context.Items.ContainsKey(CorrelationHeader) ? context.Items[CorrelationHeader]?.ToString() : null;

        if (!context.Request.Headers.TryGetValue("Authorization", out var authValues))
        {
            _logger.LogWarning("Missing Authorization header. CorrelationId={CorrelationId}", correlationId);
            await RespondUnauthorized(context, correlationId);
            return;
        }

        var auth = authValues.ToString();
        if (!auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Invalid Authorization scheme. CorrelationId={CorrelationId}", correlationId);
            await RespondUnauthorized(context, correlationId);
            return;
        }

        var token = auth.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token) || !string.Equals(token, _validToken, StringComparison.Ordinal))
        {
            _logger.LogWarning("Invalid token. CorrelationId={CorrelationId}", correlationId);
            await RespondUnauthorized(context, correlationId);
            return;
        }

        // Token valid: set a simple principal so downstream can use User.Identity if needed
        var claims = new[] { new Claim(ClaimTypes.Name, "ApiClient") };
        var identity = new ClaimsIdentity(claims, "Token");
        context.User = new ClaimsPrincipal(identity);

        await _next(context);
    }

    private static async Task RespondUnauthorized(HttpContext context, string? correlationId)
    {
        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        context.Response.Headers["WWW-Authenticate"] = "Bearer";

        if (!string.IsNullOrEmpty(correlationId))
            context.Response.Headers[CorrelationHeader] = correlationId;

        var payload = new { error = "Unauthorized", CorrelationId = correlationId };
        await context.Response.WriteAsJsonAsync(payload);
    }
}