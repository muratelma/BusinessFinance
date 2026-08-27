using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.UserAccount;

/// <summary>
/// Kullanıcının kendi hesabı hakkında gördüğü bilgi. Hassas hiçbir şey taşımaz:
/// parola hash'i, token ve token hash'i buraya girmez.
/// </summary>
public sealed record UserAccountDto(
    Guid UserId,
    string Email,
    bool EmailConfirmed,
    DateTimeOffset CreatedAtUtc,
    int ActiveSessionCount);

/// <summary>
/// Açık bir oturum satırı. Token veya hash göstermez; oturumun ne zaman
/// açıldığını ve ne zaman düşeceğini söyler.
/// </summary>
public sealed record UserSessionDto(
    Guid SessionId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc);

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword);

/// <summary>
/// Parola değişince kullanıcının bütün oturumları kapanır; isteği yapan cihaz
/// uygulamadan atılmasın diye cevapla birlikte taze bir token çifti alır.
/// </summary>
public sealed record ChangePasswordResponse(
    Guid SessionId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);

public sealed record RevokeUserSessionCommand(Guid SessionId);

public sealed record RevokeUserSessionResponse();

/// <summary>
/// Hesap silme isteği. İki kapı birden ister: kullanıcı parolasını yeniden
/// yazar (yeniden kimlik doğrulama) ve isteği açıkça onaylar.
/// </summary>
public sealed record DeleteUserAccountCommand(string Password, bool Confirmed);

public sealed record DeleteUserAccountResponse();

public enum PasswordChangeStatus
{
    Succeeded = 1,
    InvalidCurrentPassword = 2,
    PasswordPolicy = 3,
    UserNotFound = 4
}

/// <summary>
/// Hesabın tamamının silinmesi (ADR 0017). Kullanıcıya ait bütün kayıtları tek
/// bir transaction'da siler; yarım silinmiş hesap bırakmaz.
/// </summary>
public interface IUserAccountEraser
{
    Task EraseAsync(Guid userId, CancellationToken cancellationToken);
}

public static class UserAccountErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError InvalidPassword = new(
        "account.invalid_password",
        "The supplied password is invalid.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError SessionNotFound = new(
        "account.session_not_found",
        "The session was not found.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError DeleteNotConfirmed = new(
        "account.delete_not_confirmed",
        "Account deletion must be confirmed explicitly.",
        ApplicationErrorType.Validation);
}
