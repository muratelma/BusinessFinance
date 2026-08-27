using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Tokens;

namespace BusinessFinance.Application.UserAccount;

public sealed class GetUserAccountUseCase(
    ICurrentUser currentUser,
    IIdentityAccountService identityAccountService,
    IRefreshSessionRepository refreshSessionRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<UserAccountDto>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<UserAccountDto>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        var account = await identityAccountService.FindAccountAsync(userId, cancellationToken);
        if (account is null)
        {
            return ApplicationResult<UserAccountDto>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        var sessions = await refreshSessionRepository.ListActiveForUserAsync(
            userId,
            timeProvider.GetUtcNow(),
            cancellationToken);

        return ApplicationResult<UserAccountDto>.Success(new UserAccountDto(
            account.UserId,
            account.Email,
            account.EmailConfirmed,
            account.CreatedAtUtc,
            sessions.Count));
    }
}
