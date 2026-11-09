using BusinessFinance.Application.Authentication.LoginUser;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Authentication.Tokens;

/// <summary>
/// Kimliği doğrulanmış kullanıcıya oturum açar: erişim belirteci, yenileme
/// belirteci ve kalıcı yenileme oturumu. Parolayla ve Google'la giriş aynı
/// yoldan geçer.
/// </summary>
public static class SessionIssuance
{
    public static async Task<LoginUserResponse> IssueAsync(
        AuthenticatedIdentity identity,
        ISecurityTokenService securityTokenService,
        IRefreshSessionRepository refreshSessionRepository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(identity);

        var accessToken = securityTokenService.CreateAccessToken(identity);
        var refreshToken = securityTokenService.CreateRefreshToken();
        var refreshSession = new RefreshSession(
            Guid.NewGuid(),
            identity.UserId,
            refreshToken.Hash,
            refreshToken.CreatedAtUtc,
            refreshToken.ExpiresAtUtc);

        await refreshSessionRepository.AddAsync(refreshSession, cancellationToken);

        return new LoginUserResponse(
            identity.UserId,
            refreshSession.Id,
            identity.Email,
            accessToken.Value,
            accessToken.ExpiresAtUtc,
            refreshToken.Value,
            refreshToken.ExpiresAtUtc);
    }
}
