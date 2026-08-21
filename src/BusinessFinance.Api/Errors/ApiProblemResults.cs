using System.Diagnostics;

namespace BusinessFinance.Api.Errors;

public static class ApiProblemResults
{
    public static IResult Create(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        string code)
    {
        return Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            extensions: new Dictionary<string, object?>
            {
                ["code"] = code,
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            });
    }

    public static IResult Validation(
        HttpContext httpContext,
        string detail,
        string code)
    {
        return Create(
            httpContext,
            StatusCodes.Status400BadRequest,
            "Request validation failed.",
            detail,
            code);
    }
}
