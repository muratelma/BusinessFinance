namespace BusinessFinance.Application.Authentication.RegisterUser;

/// <summary>
/// Kayıt isteği. <paramref name="HasBusiness"/> onboarding'in tek sorusudur.
/// </summary>
/// <remarks>
/// Cevap verilmediğinde "işletmesi yok" kabul edilir. Bu bir tahmin değil,
/// muhafazakâr olan taraf: kişisel set kurulur, kapsam boyutu arayüzde
/// görünmez ve kullanıcı sonradan cevabını değiştirebilir. Tersi — herkesi
/// işletme saymak — esnaf olmayan kullanıcıyı hiç ihtiyacı olmayan bir
/// ayrımla karşılardı.
/// <para>
/// <paramref name="Username"/> isteğe bağlıdır ve sözleşmeyi kırmamak için
/// sondadır: gönderilmezse hesabın kullanıcı adı e-postası olur (bugüne kadar
/// açılan hesaplar gibi). Uygulama her zaman gönderir.
/// </para>
/// </remarks>
public sealed record RegisterUserCommand(
    string Email,
    string Password,
    bool HasBusiness = false,
    string? Username = null);
