using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication.Tokens;

namespace BusinessFinance.Application.Authentication.Logout;

public sealed class LogoutUseCase
{
    private readonly ISecurityTokenService _securityTokenService;
    private readonly IRefreshSessionRepository _refreshSessionRepository;
    private readonly TimeProvider _timeProvider;

    public LogoutUseCase(
        ISecurityTokenService securityTokenService,
        IRefreshSessionRepository refreshSessionRepository,
        TimeProvider timeProvider)
    {
        _securityTokenService = securityTokenService;
        _refreshSessionRepository = refreshSessionRepository;
        _timeProvider = timeProvider;
    }

    public async Task<ApplicationResult<LogoutResponse>> ExecuteAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (!string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            var hash = _securityTokenService.HashRefreshToken(command.RefreshToken);
            var session = await _refreshSessionRepository.FindByTokenHashAsync(
                hash,
                cancellationToken);

            if (session is not null && !session.IsRevoked)
            {
                session.Revoke(_timeProvider.GetUtcNow());
                await _refreshSessionRepository.UpdateAsync(session, cancellationToken);
            }
        }

        return ApplicationResult<LogoutResponse>.Success(new LogoutResponse());
    }
}
