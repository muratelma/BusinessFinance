using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Authentication.LoginUser;

public sealed class LoginUserUseCase
{
    private readonly IIdentityAccountService _identityAccountService;
    private readonly ISecurityTokenService _securityTokenService;
    private readonly IRefreshSessionRepository _refreshSessionRepository;

    public LoginUserUseCase(
        IIdentityAccountService identityAccountService,
        ISecurityTokenService securityTokenService,
        IRefreshSessionRepository refreshSessionRepository)
    {
        _identityAccountService = identityAccountService;
        _securityTokenService = securityTokenService;
        _refreshSessionRepository = refreshSessionRepository;
    }

    public async Task<ApplicationResult<LoginUserResponse>> ExecuteAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var identity = await _identityAccountService.AuthenticateAsync(
            command.Email,
            command.Password,
            cancellationToken);

        if (identity is null)
        {
            return ApplicationResult<LoginUserResponse>.Failure(
                AuthenticationErrors.InvalidCredentials);
        }

        var accessToken = _securityTokenService.CreateAccessToken(identity);
        var refreshToken = _securityTokenService.CreateRefreshToken();
        var refreshSession = new RefreshSession(
            Guid.NewGuid(),
            identity.UserId,
            refreshToken.Hash,
            refreshToken.CreatedAtUtc,
            refreshToken.ExpiresAtUtc);

        await _refreshSessionRepository.AddAsync(refreshSession, cancellationToken);

        return ApplicationResult<LoginUserResponse>.Success(
            new LoginUserResponse(
                identity.UserId,
                identity.Email,
                accessToken.Value,
                accessToken.ExpiresAtUtc,
                refreshToken.Value,
                refreshToken.ExpiresAtUtc));
    }
}
