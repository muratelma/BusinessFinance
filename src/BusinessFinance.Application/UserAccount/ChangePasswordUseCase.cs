using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.UserAccount;

/// <summary>
/// Parola değiştirme. Başarılı olduğunda kullanıcının <b>bütün</b> refresh
/// oturumları kapanır — çalınmış bir parolayla açılmış oturum, parola
/// değiştikten sonra yaşamaya devam etmemelidir. İsteği yapan cihaz uygulamadan
/// atılmasın diye cevapla birlikte yeni bir oturum ve taze token çifti alır;
/// elindeki eski refresh token artık reddedilir.
/// </summary>
public sealed class ChangePasswordUseCase(
    ICurrentUser currentUser,
    IIdentityAccountService identityAccountService,
    ISecurityTokenService securityTokenService,
    IRefreshSessionRepository refreshSessionRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<ChangePasswordResponse>> ExecuteAsync(
        ChangePasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<ChangePasswordResponse>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        var status = await identityAccountService.ChangePasswordAsync(
            userId,
            command.CurrentPassword ?? string.Empty,
            command.NewPassword ?? string.Empty,
            cancellationToken);

        switch (status)
        {
            case PasswordChangeStatus.InvalidCurrentPassword:
                return ApplicationResult<ChangePasswordResponse>.Failure(
                    UserAccountErrors.InvalidPassword);
            case PasswordChangeStatus.PasswordPolicy:
                return ApplicationResult<ChangePasswordResponse>.Failure(
                    AuthenticationErrors.PasswordPolicy);
            case PasswordChangeStatus.UserNotFound:
                return ApplicationResult<ChangePasswordResponse>.Failure(
                    UserAccountErrors.AuthenticationRequired);
        }

        var identity = await identityAccountService.FindActiveByIdAsync(userId, cancellationToken);
        if (identity is null)
        {
            return ApplicationResult<ChangePasswordResponse>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        await refreshSessionRepository.RevokeAllActiveForUserAsync(
            userId,
            timeProvider.GetUtcNow(),
            cancellationToken);

        var accessToken = securityTokenService.CreateAccessToken(identity);
        var refreshToken = securityTokenService.CreateRefreshToken();
        var session = new RefreshSession(
            Guid.NewGuid(),
            userId,
            refreshToken.Hash,
            refreshToken.CreatedAtUtc,
            refreshToken.ExpiresAtUtc);

        await refreshSessionRepository.AddAsync(session, cancellationToken);

        return ApplicationResult<ChangePasswordResponse>.Success(new ChangePasswordResponse(
            session.Id,
            accessToken.Value,
            accessToken.ExpiresAtUtc,
            refreshToken.Value,
            refreshToken.ExpiresAtUtc));
    }
}
