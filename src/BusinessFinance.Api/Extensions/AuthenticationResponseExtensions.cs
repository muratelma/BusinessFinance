using Microsoft.AspNetCore.Authentication.JwtBearer;
using BusinessFinance.Api.Errors;

namespace BusinessFinance.Api.Extensions;

public static class AuthenticationResponseExtensions
{
    public static IServiceCollection AddApiAuthenticationResponses(
        this IServiceCollection services)
    {
        services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.Events ??= new JwtBearerEvents();
                options.Events.OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.Headers.WWWAuthenticate = "Bearer";
                    await ApiProblemResults.Create(
                            context.HttpContext,
                            StatusCodes.Status401Unauthorized,
                            "Authentication is required.",
                            "A valid bearer access token is required.",
                            "authentication.required")
                        .ExecuteAsync(context.HttpContext);
                };
                options.Events.OnForbidden = async context =>
                {
                    await ApiProblemResults.Create(
                            context.HttpContext,
                            StatusCodes.Status403Forbidden,
                            "Access is forbidden.",
                            "The authenticated user cannot perform this operation.",
                            "authorization.forbidden")
                        .ExecuteAsync(context.HttpContext);
                };
            });

        return services;
    }
}
