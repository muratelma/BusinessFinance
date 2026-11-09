namespace BusinessFinance.Application.Authentication.Google;

/// <summary>
/// Google'ın imzaladığı ve bu uygulamanın istemcilerinden birine verilmiş bir
/// kimlik belirtecinden okunan kişi.
/// </summary>
/// <param name="Subject">Google hesabının değişmeyen kimliği (<c>sub</c>).</param>
/// <param name="EmailVerified">
/// Google bu e-postanın hesabın sahibine ait olduğunu doğruladı mı. Gmail
/// adreslerinde her zaman doğrudur; Google hesabı başka bir adresle
/// açılmışsa yanlış olabilir.
/// </param>
public sealed record GoogleIdentity(string Subject, string Email, bool EmailVerified);

public interface IGoogleIdTokenValidator
{
    /// <summary>
    /// Belirtecin imzasını, verenini, süresini ve hangi istemciye verildiğini
    /// denetler. Geçersiz belirteç ya da yapılandırılmamış istemci null döner.
    /// </summary>
    Task<GoogleIdentity?> ValidateAsync(string idToken, CancellationToken cancellationToken);
}

/// <summary>Dış sağlayıcıyla (Google) giriş için kimlik deposu işlemleri.</summary>
public interface IExternalAccountService
{
    /// <summary>Sağlayıcı kimliğine bağlanmış hesap; pasif hesap da döner.</summary>
    Task<ExternalAccount?> FindByLoginAsync(
        string provider,
        string providerKey,
        CancellationToken cancellationToken);

    /// <summary>E-postası bu olan hesap; pasif hesap da döner.</summary>
    Task<ExternalAccount?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    /// <summary>
    /// Var olan hesaba sağlayıcı kimliğini bağlar ve e-postayı doğrulanmış
    /// sayar. <paramref name="removePassword"/> doğruysa hesabın parolası
    /// kaldırılır.
    /// </summary>
    Task LinkAsync(
        Guid userId,
        string provider,
        string providerKey,
        bool removePassword,
        CancellationToken cancellationToken);

    /// <summary>
    /// Parolasız hesap açar ve sağlayıcı kimliğini bağlar.
    /// <paramref name="username"/> normalize edilmiş ve kurala uyduğu
    /// denetlenmiş olarak gelir.
    /// </summary>
    Task<IdentityRegistrationResult> CreateAsync(
        string email,
        bool emailConfirmed,
        string username,
        string provider,
        string providerKey,
        CancellationToken cancellationToken);
}

public sealed record ExternalAccount(
    Guid UserId,
    string Email,
    bool IsActive,
    bool EmailConfirmed);
