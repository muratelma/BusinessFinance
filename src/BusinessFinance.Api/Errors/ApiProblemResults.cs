using System.Diagnostics;

namespace BusinessFinance.Api.Errors;

public static class ApiProblemResults
{
    public static IResult Create(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        string code,
        IReadOnlyDictionary<string, string>? details = null)
    {
        var extensions = new Dictionary<string, object?>
        {
            ["code"] = code,
            ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
        };

        // Hatanın taşıdığı makine değerleri; `code` ve `traceId` ezilmez.
        foreach (var (key, value) in details ?? new Dictionary<string, string>())
        {
            extensions.TryAdd(key, value);
        }

        return Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            extensions: extensions);
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
