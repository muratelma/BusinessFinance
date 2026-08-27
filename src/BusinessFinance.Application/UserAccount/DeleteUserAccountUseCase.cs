using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;

namespace BusinessFinance.Application.UserAccount;

/// <summary>
/// Hesabın kapatılması (ADR 0017). Bu bir finansal düzeltme değildir: "silme
/// yerine iptal" kuralı bir defterin içindeki hareket içindir, defterin
/// kendisini kapatan kullanıcı için değil. Geri dönüşü olmadığı için iki kapı
/// birden ister — parolanın yeniden yazılması ve açık onay.
/// </summary>
public sealed class DeleteUserAccountUseCase(
    ICurrentUser currentUser,
    IIdentityAccountService identityAccountService,
    IUserAccountEraser eraser)
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

        var passwordIsValid = await identityAccountService.VerifyPasswordAsync(
            userId,
            command.Password ?? string.Empty,
            cancellationToken);

        if (!passwordIsValid)
        {
            return ApplicationResult<DeleteUserAccountResponse>.Failure(
                UserAccountErrors.InvalidPassword);
        }

        // Önce finansal veri, sonra kimlik: ters sırada bir hata, sahibi
        // silinmiş ama verisi duran bir hesap bırakırdı.
        await eraser.EraseAsync(userId, cancellationToken);
        await identityAccountService.DeleteAsync(userId, cancellationToken);

        return ApplicationResult<DeleteUserAccountResponse>.Success(
            new DeleteUserAccountResponse());
    }
}
