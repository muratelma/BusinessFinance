using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication.Tokens;

namespace BusinessFinance.Application.UserAccount;

public sealed class ListUserSessionsUseCase(
    ICurrentUser currentUser,
    IRefreshSessionRepository refreshSessionRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<IReadOnlyList<UserSessionDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<UserSessionDto>>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        var sessions = await refreshSessionRepository.ListActiveForUserAsync(
            userId,
            timeProvider.GetUtcNow(),
            cancellationToken);

        IReadOnlyList<UserSessionDto> items = sessions
            .OrderByDescending(session => session.CreatedAtUtc)
            .Select(session => new UserSessionDto(
                session.Id,
                session.CreatedAtUtc,
                session.ExpiresAtUtc))
            .ToArray();

        return ApplicationResult<IReadOnlyList<UserSessionDto>>.Success(items);
    }
}
