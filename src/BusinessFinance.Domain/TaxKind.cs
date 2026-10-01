namespace BusinessFinance.Domain;

/// <summary>
/// Bir tekrarlayan planın vergi olduğunu ve hangi vergi olduğunu söyler
/// (ADR 0018 T2).
/// </summary>
/// <remarks>
/// <para>
/// Vergi ayrı bir kayıt türü değildir (İ6): tanımlı vergi bir tekrarlayan
/// plandır ve bu alan planın bir biçimidir. Alanı dolu plan Vergiler
/// ekranında yaşar, Tekrarlayanlar listesinde görünmez, Yaklaşanlar'da
/// görünür.
/// </para>
/// <para>
/// Kimlik bir ada bağlanmaz (İ8): kullanıcı planın adını değiştirebilir, planın
/// vergi olduğu bu alandan bilinir. Hazır türler yalnız ritim ve gün önerir;
/// kurulduğu an kullanıcının verisidir. <see cref="Custom"/> kullanıcının kendi
/// türüdür ve adı planın açıklamasıdır.
/// </para>
/// </remarks>
public enum TaxKind
{
    SocialSecurityPremium = 1,
    VatReturn = 2,
    WithholdingReturn = 3,
    AdvanceTax = 4,
    AnnualIncomeTax = 5,
    PropertyTax = 6,
    MotorVehicleTax = 7,
    AdvertisingTax = 8,
    Custom = 9
}
