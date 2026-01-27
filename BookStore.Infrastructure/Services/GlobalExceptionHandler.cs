using BookStore.Application.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookStore.Infrastructure.Services;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Log the Error (Critical for SigNoz/OpenTelemetry)
        logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        // 2. Determine Status Code & Message based on Exception Type
        (int statusCode, string errorCode, string message) = exception switch
        {
            // Custom Domain Exceptions
            UnauthorizedAccessException => (401, "UNAUTHORIZED", "You do not have permission."),
            KeyNotFoundException => (404, "NOT_FOUND", "The requested resource was not found."),
            ArgumentException => (400, "BAD_REQUEST", exception.Message),
            _ => (500, "INTERNAL_ERROR", "An unexpected error occurred. Please contact support.")
        };
            
        httpContext.Response.StatusCode = statusCode;
            
        // 4. Create the Standard Body
        // In Development, show the real error. In Production, show the Generic Message.
        var response = new ApiResponse<object>(
            errors: new List<string>(){"Something went wrong"},
            message: env.IsDevelopment() ? exception.Message : message
        );
            
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; // don't propagate further.
    }
}