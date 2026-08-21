using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Api.Errors;

public static class ApplicationErrorHttpExtensions
{
    public static IResult ToProblemResult(
        this ApplicationError error,
        HttpContext httpContext)
    {
        var statusCode = error.Type switch
        {
            ApplicationErrorType.Validation => StatusCodes.Status400BadRequest,
            ApplicationErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ApplicationErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ApplicationErrorType.NotFound => StatusCodes.Status404NotFound,
            ApplicationErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status401Unauthorized)
        {
            httpContext.Response.Headers.WWWAuthenticate = "Bearer";
        }

        return ApiProblemResults.Create(
            httpContext,
            statusCode,
            GetTitle(error.Type),
            error.Message,
            error.Code);
    }

    private static string GetTitle(ApplicationErrorType errorType)
    {
        return errorType switch
        {
            ApplicationErrorType.Validation => "Request validation failed.",
            ApplicationErrorType.Unauthorized => "Authentication is required.",
            ApplicationErrorType.Forbidden => "Access is forbidden.",
            ApplicationErrorType.NotFound => "The requested resource was not found.",
            ApplicationErrorType.Conflict => "The request conflicts with current state.",
            _ => "An unexpected server error occurred."
        };
    }
}
