using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Authentication.RefreshTokens;

public sealed class RefreshTokensUseCase
{
    private readonly IIdentityAccountService _identityAccountService;
    private readonly ISecurityTokenService _securityTokenService;
    private readonly IRefreshSessionRepository _refreshSessionRepository;
    private readonly TimeProvider _timeProvider;

    public RefreshTokensUseCase(
        IIdentityAccountService identityAccountService,
        ISecurityTokenService securityTokenService,
        IRefreshSessionRepository refreshSessionRepository,
        TimeProvider timeProvider)
    {
        _identityAccountService = identityAccountService;
        _securityTokenService = securityTokenService;
        _refreshSessionRepository = refreshSessionRepository;
        _timeProvider = timeProvider;
    }

    public async Task<ApplicationResult<RefreshTokensResponse>> ExecuteAsync(
        RefreshTokensCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return InvalidRefreshToken();
        }

        var utcNow = _timeProvider.GetUtcNow();
        var tokenHash = _securityTokenService.HashRefreshToken(command.RefreshToken);
        var currentSession = await _refreshSessionRepository.FindByTokenHashAsync(
            tokenHash,
            cancellationToken);

        if (currentSession is null)
        {
            return InvalidRefreshToken();
        }

        if (currentSession.IsRevoked)
        {
            currentSession.MarkReuseDetected(utcNow);
            await _refreshSessionRepository.UpdateAsync(currentSession, cancellationToken);
            await _refreshSessionRepository.RevokeAllActiveForUserAsync(
                currentSession.UserId,
                utcNow,
                cancellationToken);
            return InvalidRefreshToken();
        }

        if (currentSession.IsExpired(utcNow))
        {
            currentSession.Revoke(utcNow);
            await _refreshSessionRepository.UpdateAsync(currentSession, cancellationToken);
            return InvalidRefreshToken();
        }

        var identity = await _identityAccountService.FindActiveByIdAsync(
            currentSession.UserId,
            cancellationToken);

        if (identity is null)
        {
            currentSession.Revoke(utcNow);
            await _refreshSessionRepository.UpdateAsync(currentSession, cancellationToken);
            return InvalidRefreshToken();
        }

        var accessToken = _securityTokenService.CreateAccessToken(identity);
        var refreshToken = _securityTokenService.CreateRefreshToken();
        var replacementSession = new RefreshSession(
            Guid.NewGuid(),
            identity.UserId,
            refreshToken.Hash,
            refreshToken.CreatedAtUtc,
            refreshToken.ExpiresAtUtc);

        currentSession.Rotate(replacementSession.Id, utcNow);
        await _refreshSessionRepository.RotateAsync(
            currentSession,
            replacementSession,
            cancellationToken);

        return ApplicationResult<RefreshTokensResponse>.Success(
            new RefreshTokensResponse(
                replacementSession.Id,
                accessToken.Value,
                accessToken.ExpiresAtUtc,
                refreshToken.Value,
                refreshToken.ExpiresAtUtc));
    }

    private static ApplicationResult<RefreshTokensResponse> InvalidRefreshToken()
    {
        return ApplicationResult<RefreshTokensResponse>.Failure(
            AuthenticationErrors.InvalidRefreshToken);
    }
}
