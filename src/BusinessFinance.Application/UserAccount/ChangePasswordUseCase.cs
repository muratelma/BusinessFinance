using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.UserAccount;

/// <summary>
/// Parola değiştirme. Başarılı olduğunda kullanıcının <b>bütün</b> refresh
/// oturumları kapanır — çalınmış bir parolayla açılmış oturum, parola
/// değiştikten sonra yaşamaya devam etmemelidir. İsteği yapan cihaz uygulamadan
/// atılmasın diye cevapla birlikte yeni bir oturum ve taze token çifti alır;
/// elindeki eski refresh token artık reddedilir.
/// </summary>
/// <remarks>
/// <b>Parola belirleme</b> (9 Kasım 2026): Google ile açılmış hesabın parolası
/// yoktur. Mevcut parolanın yerine e-postasına giden sıfırlama kodunu
/// gönderir. Oturum açık diye kod sormadan parola koydurmak, telefonu ele
/// geçiren birinin hesabı da ele geçirmesine izin verirdi. Sıfırlamadan farkı:
/// isteği yapan cihaz açık kalır.
/// </remarks>
public sealed class ChangePasswordUseCase(
    ICurrentUser currentUser,
    IIdentityAccountService identityAccountService,
    ISecurityTokenService securityTokenService,
    IRefreshSessionRepository refreshSessionRepository,
    TimeProvider timeProvider,
    IVerificationCodeService? codeService = null,
    IVerificationCodeRepository? codeRepository = null)
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

        var account = await identityAccountService.FindAccountAsync(userId, cancellationToken);
        if (account is null)
        {
            return ApplicationResult<ChangePasswordResponse>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        PasswordChangeStatus status;
        if (account.HasPassword)
        {
            status = await identityAccountService.ChangePasswordAsync(
                userId,
                command.CurrentPassword ?? string.Empty,
                command.NewPassword ?? string.Empty,
                cancellationToken);
        }
        else
        {
            var matched = codeService is not null &&
                codeRepository is not null &&
                await VerificationCodeIssuer.TryConsumeAsync(
                    userId,
                    VerificationPurpose.PasswordReset,
                    command.ResetCode,
                    codeService,
                    codeRepository,
                    timeProvider.GetUtcNow(),
                    cancellationToken);
            if (!matched)
            {
                return ApplicationResult<ChangePasswordResponse>.Failure(
                    VerificationErrors.InvalidResetCode);
            }

            status = await identityAccountService.SetPasswordAsync(
                userId,
                command.NewPassword ?? string.Empty,
                cancellationToken);
        }

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
