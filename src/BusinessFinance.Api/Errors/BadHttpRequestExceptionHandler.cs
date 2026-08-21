using Microsoft.AspNetCore.Diagnostics;

namespace BusinessFinance.Api.Errors;

public sealed class BadHttpRequestExceptionHandler(
    ILogger<BadHttpRequestExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException)
        {
            return false;
        }

        logger.LogWarning(
            "Invalid HTTP request was rejected. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        await ApiProblemResults.Validation(
                httpContext,
                "The HTTP request body or parameter format is invalid.",
                "request.invalid_format")
            .ExecuteAsync(httpContext);

        return true;
    }
}
