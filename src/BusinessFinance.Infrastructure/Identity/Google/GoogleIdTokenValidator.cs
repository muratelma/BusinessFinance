using System.Security.Claims;
using BusinessFinance.Application.Authentication.Google;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace BusinessFinance.Infrastructure.Identity.Google;

public sealed class GoogleSignInOptions
{
    public const string SectionName = "GoogleSignIn";

    /// <summary>
    /// Belirtecin verilebileceği istemciler: Web istemcisi (Android belirteci
    /// onun adına ister) ve iOS istemcisi. Client ID gizli bilgi değildir.
    /// Boşsa Google ile giriş kapalıdır.
    /// </summary>
    public string[] ClientIds { get; init; } = [];
}

/// <summary>
/// Google kimlik belirtecini Google'ın yayımladığı anahtarlarla doğrular.
/// </summary>
/// <remarks>
/// Ayrı bir Google kütüphanesi eklenmedi: JWT bearer'ın zaten getirdiği
/// OpenID Connect kütüphanesi anahtarları Google'ın keşif belgesinden çeker,
/// önbellekte tutar ve döndükçe yeniler. Giden tek çağrı Google'ın sertifika
/// adresinedir (gelen bağlantı değil; AGENTS "Veri ve güvenlik").
/// </remarks>
public sealed class GoogleIdTokenValidator : IGoogleIdTokenValidator
{
    private const string DiscoveryAddress =
        "https://accounts.google.com/.well-known/openid-configuration";

    private static readonly string[] Issuers =
        ["https://accounts.google.com", "accounts.google.com"];

    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configuration;
    private readonly IOptions<GoogleSignInOptions> _options;
    private readonly ILogger<GoogleIdTokenValidator> _logger;
    private readonly JsonWebTokenHandler _handler = new() { MapInboundClaims = false };

    public GoogleIdTokenValidator(
        IOptions<GoogleSignInOptions> options,
        ILogger<GoogleIdTokenValidator> logger,
        IConfigurationManager<OpenIdConnectConfiguration>? configuration = null)
    {
        _options = options;
        _logger = logger;
        _configuration = configuration ?? new ConfigurationManager<OpenIdConnectConfiguration>(
            DiscoveryAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever { RequireHttps = true });
    }

    public async Task<GoogleIdentity?> ValidateAsync(
        string idToken,
        CancellationToken cancellationToken)
    {
        var clientIds = _options.Value.ClientIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToArray();
        if (clientIds.Length == 0)
        {
            _logger.LogWarning("Google sign-in was requested but no client id is configured.");
            return null;
        }

        var configuration = await _configuration.GetConfigurationAsync(cancellationToken);
        var result = await _handler.ValidateTokenAsync(idToken, new TokenValidationParameters
        {
            ValidIssuers = Issuers,
            ValidAudiences = clientIds,
            IssuerSigningKeys = configuration.SigningKeys,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        });

        if (!result.IsValid)
        {
            // Belirtecin kendisi loglanmaz.
            _logger.LogInformation(
                "Google id token rejected: {Reason}",
                result.Exception?.GetType().Name);
            if (result.Exception is SecurityTokenSignatureKeyNotFoundException)
            {
                // Google anahtarını döndürmüş olabilir; bir sonraki istek
                // yeni anahtarlarla doğrulasın.
                _configuration.RequestRefresh();
            }

            return null;
        }

        var claims = result.ClaimsIdentity;
        var subject = claims.FindFirst("sub")?.Value;
        var email = claims.FindFirst("email")?.Value;
        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return new GoogleIdentity(subject, email, IsTrue(claims.FindFirst("email_verified")));
    }

    private static bool IsTrue(Claim? claim) =>
        claim is not null && bool.TryParse(claim.Value, out var value) && value;
}
