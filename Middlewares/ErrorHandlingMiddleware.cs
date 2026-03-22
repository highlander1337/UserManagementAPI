using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UserManagementAPI.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        const string CorrelationHeader = "X-Correlation-ID";

        // Use incoming correlation id if provided, otherwise generate a new one
        var correlationId = context.Request.Headers.ContainsKey(CorrelationHeader)
            ? context.Request.Headers[CorrelationHeader].ToString()
            : Guid.NewGuid().ToString();

        // Make correlation id available to other middlewares/controllers
        context.Items[CorrelationHeader] = correlationId;
        // Ensure response contains the correlation id
        if (!context.Response.HasStarted)
        {
            context.Response.Headers[CorrelationHeader] = correlationId;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception for diagnostics
            Console.Error.WriteLine($"CorrelationId={correlationId} - Exception: {ex}");

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            // Ensure the correlation header is present on the error response
            context.Response.Headers[CorrelationHeader] = correlationId;

            var payload = new { Message = "An unexpected error occurred.", CorrelationId = correlationId };
            await context.Response.WriteAsJsonAsync(payload);
        }
    }
}
