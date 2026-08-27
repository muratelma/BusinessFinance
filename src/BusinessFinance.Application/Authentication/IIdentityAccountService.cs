using BusinessFinance.Application.UserAccount;

namespace BusinessFinance.Application.Authentication;

public interface IIdentityAccountService
{
    Task<IdentityRegistrationResult> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<AuthenticatedIdentity?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<AuthenticatedIdentity?> FindActiveByIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Kullanıcının kendi hesap bilgisi. Bulunamayan veya pasif kullanıcı için
    /// null döner.
    /// </summary>
    Task<UserAccountProfile?> FindAccountAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<bool> VerifyPasswordAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken);

    Task<PasswordChangeStatus> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken);

    /// <summary>
    /// Kimlik kaydını kalıcı olarak siler (ADR 0017). Finansal veriyi silmez;
    /// onu <see cref="IUserAccountEraser"/> yapar ve bu çağrıdan önce çalışır.
    /// </summary>
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed record UserAccountProfile(
    Guid UserId,
    string Email,
    bool EmailConfirmed,
    DateTimeOffset CreatedAtUtc);
