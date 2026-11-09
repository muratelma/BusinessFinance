using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Google;

namespace BusinessFinance.Application.UserAccount;

/// <summary>
/// Hesabın kapatılması (ADR 0017). Bu bir finansal düzeltme değildir: "silme
/// yerine iptal" kuralı bir defterin içindeki hareket içindir, defterin
/// kendisini kapatan kullanıcı için değil. Geri dönüşü olmadığı için iki kapı
/// birden ister — yeniden kimlik doğrulama ve açık onay. Yeniden doğrulama
/// parolası olan hesapta parolanın yazılması, Google ile açılmış (parolasız)
/// hesapta Google'a yeniden giriştir (büyük uygulamaların "yakın zamanda giriş"
/// deseni, 9 Kasım 2026).
/// </summary>
public sealed class DeleteUserAccountUseCase(
    ICurrentUser currentUser,
    IIdentityAccountService identityAccountService,
    IUserAccountEraser eraser,
    IGoogleIdTokenValidator? googleTokens = null,
    IExternalAccountService? externalAccounts = null)
{
    public async Task<ApplicationResult<DeleteUserAccountResponse>> ExecuteAsync(
        DeleteUserAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<DeleteUserAccountResponse>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        if (!command.Confirmed)
        {
            return ApplicationResult<DeleteUserAccountResponse>.Failure(
                UserAccountErrors.DeleteNotConfirmed);
        }

        var account = await identityAccountService.FindAccountAsync(userId, cancellationToken);
        if (account is null)
        {
            return ApplicationResult<DeleteUserAccountResponse>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        if (account.HasPassword)
        {
            var passwordIsValid = await identityAccountService.VerifyPasswordAsync(
                userId,
                command.Password ?? string.Empty,
                cancellationToken);

            if (!passwordIsValid)
            {
                return ApplicationResult<DeleteUserAccountResponse>.Failure(
                    UserAccountErrors.InvalidPassword);
            }
        }
        else if (!await GoogleConfirmsAsync(userId, command.GoogleIdToken, cancellationToken))
        {
            return ApplicationResult<DeleteUserAccountResponse>.Failure(
                UserAccountErrors.ReauthenticationFailed);
        }

        // Önce finansal veri, sonra kimlik: ters sırada bir hata, sahibi
        // silinmiş ama verisi duran bir hesap bırakırdı.
        await eraser.EraseAsync(userId, cancellationToken);
        await identityAccountService.DeleteAsync(userId, cancellationToken);

        return ApplicationResult<DeleteUserAccountResponse>.Success(
            new DeleteUserAccountResponse());
    }

    /// <summary>
    /// Belirteç geçerli ve bu hesaba bağlı Google hesabının mı. Başka bir
    /// Google hesabıyla onay verilemez.
    /// </summary>
    private async Task<bool> GoogleConfirmsAsync(
        Guid userId,
        string? idToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idToken) || googleTokens is null || externalAccounts is null)
        {
            return false;
        }

        var google = await googleTokens.ValidateAsync(idToken, cancellationToken);
        if (google is null)
        {
            return false;
        }

        var linked = await externalAccounts.FindByLoginAsync(
            GoogleSignInUseCase.Provider,
            google.Subject,
            cancellationToken);
        return linked?.UserId == userId;
    }
}
