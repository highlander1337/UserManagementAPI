
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private const string CorrelationHeader = "X-Correlation-ID";

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Items.ContainsKey(CorrelationHeader) ? context.Items[CorrelationHeader]?.ToString() : null;
        var sw = Stopwatch.StartNew();

        _logger.LogInformation("Incoming request {Method} {Path} CorrelationId={CorrelationId}",
            context.Request.Method, context.Request.Path, correlationId);

        await _next(context);

        sw.Stop();
        var status = context.Response?.StatusCode;
        _logger.LogInformation("Outgoing response {Method} {Path} {StatusCode} in {Elapsed}ms CorrelationId={CorrelationId}",
            context.Request.Method, context.Request.Path, status, sw.ElapsedMilliseconds, correlationId);
    }
}