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
    /// E-postasıyla aktif kullanıcıyı bulur. <b>Enumeration sorumluluğu
    /// çağıranındır</b>: bu metot "yok" der, cevabın kullanıcıya nasıl
    /// döneceğine use case karar verir.
    /// </summary>
    Task<Guid?> FindActiveUserIdByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task MarkEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Mevcut parolayı sormadan yenisini yazar. Yalnız kodu doğrulanmış
    /// sıfırlama akışı çağırır; parola politikası burada da uygulanır.
    /// </summary>
    Task<PasswordChangeStatus> SetPasswordAsync(
        Guid userId,
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
