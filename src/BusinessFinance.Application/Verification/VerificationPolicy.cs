namespace BusinessFinance.Application.Verification;

/// <summary>
/// Kodun ömrü ve yeniden gönderim aralığı. Tek yerde durur: iki farklı akış
/// (doğrulama ve sıfırlama) aynı kuralı taşımalı, yoksa kullanıcı hangisinin ne
/// kadar yaşadığını akılda tutmak zorunda kalır.
/// </summary>
public static class VerificationPolicy
{
    /// <summary>
    /// On beş dakika: e-postanın gelmesi ve kodun elle yazılması için fazlasıyla
    /// yeter, çalınan bir posta kutusunun günler sonra işe yaramasına yetmez.
    /// </summary>
    public static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Aynı adrese arka arkaya kod gönderilmez. Sınır dakikada 10 istekle
    /// çalışan IP kotasının tamamlayıcısıdır: o, tek makineden gelen seli
    /// keser; bu, hedef adresin posta kutusunu korur.
    /// </summary>
    public static readonly TimeSpan ResendInterval = TimeSpan.FromSeconds(60);
}
