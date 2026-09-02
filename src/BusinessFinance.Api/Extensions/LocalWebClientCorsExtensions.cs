namespace BusinessFinance.Api.Extensions;

public static class LocalWebClientCorsExtensions
{
    public const string PolicyName = "LocalWebClient";

    public static IServiceCollection AddLocalWebClientCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("WebClient:AllowedOrigins")
            .Get<string[]>()?
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];

        if (allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException(
                "WebClient:AllowedOrigins must contain at least one origin in Development.");
        }

        services.AddCors(options => options.AddPolicy(
            PolicyName,
            policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()));

        return services;
    }
}
