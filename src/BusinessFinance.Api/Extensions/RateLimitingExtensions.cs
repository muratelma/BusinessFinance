using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace BusinessFinance.Api.Extensions;

public static class RateLimitingExtensions
{
    public const string AuthenticationPolicy = "authentication";
    public const string ReceiptAnalysisPolicy = "receipt-analysis";
    public const int ReceiptAnalysisPermitLimit = 5;

    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                await Results.Problem(
                    statusCode: StatusCodes.Status429TooManyRequests,
                    title: "Too many requests.",
                    detail: "Please wait before trying again.",
                    extensions: new Dictionary<string, object?>
                    {
                        ["code"] = "rate_limit.exceeded",
                        ["traceId"] = Activity.Current?.Id ??
                                      context.HttpContext.TraceIdentifier
                    })
                    .ExecuteAsync(context.HttpContext);
            };
            options.AddPolicy(
                AuthenticationPolicy,
                httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ??
                                  "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
            options.AddPolicy(
                ReceiptAnalysisPolicy,
                httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                                  $"anonymous:{httpContext.Connection.RemoteIpAddress}",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = ReceiptAnalysisPermitLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });

        return services;
    }
}
