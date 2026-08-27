using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Verification;

/// <summary>
/// Kodun üretimi ve hash'lenmesi. Rastgelelik kriptografik olmak zorunda
/// olduğu için gerçeklemesi Infrastructure'dadır.
/// </summary>
public interface IVerificationCodeService
{
    /// <summary>Altı haneli, kriptografik rastgele kod.</summary>
    string CreateCode();

    string HashCode(string code);
}

public interface IVerificationCodeRepository
{
    Task AddAsync(VerificationCode code, CancellationToken cancellationToken);

    /// <summary>
    /// Kullanıcının o amaç için ürettiği <b>en son</b> kod. Tüketilmiş veya
    /// süresi geçmiş olabilir; kullanılabilirliğine çağıran karar verir.
    /// </summary>
    Task<VerificationCode?> FindLatestAsync(
        Guid userId,
        VerificationPurpose purpose,
        CancellationToken cancellationToken);

    Task UpdateAsync(VerificationCode code, CancellationToken cancellationToken);

    /// <summary>
    /// O amaç için bekleyen bütün kodları tüketilmiş sayar. Yeni kod
    /// üretilirken çağrılır: aynı anda iki geçerli kodun yaşaması, birini
    /// gören saldırganın diğerini de denemesine izin verirdi.
    /// </summary>
    Task ConsumeAllAsync(
        Guid userId,
        VerificationPurpose purpose,
        DateTimeOffset consumedAtUtc,
        CancellationToken cancellationToken);
}

/// <summary>
/// Kodu kullanıcıya ulaştıran giden kanal.
/// </summary>
/// <remarks>
/// Application yalnız <b>ne</b> gönderileceğini bilir; metnin kendisi, sağlayıcı
/// ve HTTP çağrısı Infrastructure'dadır. Bu ayrım sayesinde birim ve integration
/// testlerinin hiçbiri ağa çıkmaz.
/// </remarks>
public interface IVerificationEmailSender
{
    Task<EmailDeliveryStatus> SendCodeAsync(
        string email,
        VerificationPurpose purpose,
        string code,
        DateTimeOffset expiresAtUtc,
        CancellationToken cancellationToken);
}

public enum EmailDeliveryStatus
{
    Sent = 1,

    /// <summary>
    /// Gönderici yapılandırılmadı (yerelde anahtar yok). Bir hata değil, kapalı
    /// bir kapıdır: uygulama başlar, kimlik akışı çalışır, yalnız posta gitmez.
    /// </summary>
    NotConfigured = 2,

    Failed = 3
}

public sealed record SendEmailVerificationResponse(bool AlreadyConfirmed, bool CodeSent);

public sealed record ConfirmEmailCommand(string Code);

public sealed record ConfirmEmailResponse();

public sealed record RequestPasswordResetCommand(string Email);

public sealed record RequestPasswordResetResponse();

public sealed record ResetPasswordCommand(string Email, string Code, string NewPassword);

public sealed record ResetPasswordResponse();

public static class VerificationErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    /// <summary>
    /// Yanlış kod, süresi geçmiş kod, tüketilmiş kod ve hiç kod istememiş
    /// kullanıcı <b>aynı</b> cevaba döner: hangisinin doğru olduğunu söylemek,
    /// saldırgana nerede durduğunu öğretirdi.
    /// </summary>
    public static readonly ApplicationError InvalidVerificationCode = new(
        "account.invalid_verification_code",
        "The verification code is invalid or has expired.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidResetCode = new(
        "authentication.invalid_reset_code",
        "The reset code is invalid or has expired.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Aynı adrese kod yağdırılamaz. Yalnız oturum açmış kullanıcının kendi
    /// isteğinde görünür; anonim sıfırlama yolunda bu cevap hiç dönmez, çünkü
    /// "biraz bekleyin" demek adresin kayıtlı olduğunu söylerdi.
    /// </summary>
    public static readonly ApplicationError CodeRequestedTooSoon = new(
        "account.verification_code_too_soon",
        "A verification code was requested recently.",
        ApplicationErrorType.Conflict);
}
