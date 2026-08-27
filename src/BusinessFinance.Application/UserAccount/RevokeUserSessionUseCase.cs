using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication.Tokens;

namespace BusinessFinance.Application.UserAccount;

/// <summary>
/// Tek bir oturumu kapatır. Başka kullanıcıya ait oturum ile var olmayan oturum
/// aynı <c>NotFound</c> sonucuna gider: sahiplik kapsamı repository çağrısının
/// içindedir, cevabın içinde değil.
/// </summary>
public sealed class RevokeUserSessionUseCase(
    ICurrentUser currentUser,
    IRefreshSessionRepository refreshSessionRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<RevokeUserSessionResponse>> ExecuteAsync(
        RevokeUserSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RevokeUserSessionResponse>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        var session = await refreshSessionRepository.FindOwnedByIdAsync(
            command.SessionId,
            userId,
            cancellationToken);

        if (session is null || session.IsRevoked)
        {
            return ApplicationResult<RevokeUserSessionResponse>.Failure(
                UserAccountErrors.SessionNotFound);
        }

        session.Revoke(timeProvider.GetUtcNow());
        await refreshSessionRepository.UpdateAsync(session, cancellationToken);

        return ApplicationResult<RevokeUserSessionResponse>.Success(
            new RevokeUserSessionResponse());
    }
}
