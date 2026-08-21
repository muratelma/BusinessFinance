using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace BusinessFinance.Api.Errors;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(
            "Unhandled exception type {ExceptionType}. TraceId: {TraceId}",
            exception.GetType().FullName,
            traceId);

        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An unexpected server error occurred.",
            detail: "The server could not complete the request.",
            extensions: new Dictionary<string, object?>
            {
                ["code"] = "server.unexpected_error",
                ["traceId"] = traceId
            })
            .ExecuteAsync(httpContext);

        return true;
    }
}
