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
/// </remarks>
public sealed record RegisterUserCommand(
    string Email,
    string Password,
    bool HasBusiness = false);
