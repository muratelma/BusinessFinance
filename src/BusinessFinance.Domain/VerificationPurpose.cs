namespace BusinessFinance.Domain;

/// <summary>
/// Bir doğrulama kodunun ne için üretildiği.
/// </summary>
/// <remarks>
/// İki amaç tek tabloda yaşar ama <b>asla birbirinin yerine geçmez</b>: e-posta
/// doğrulama için üretilmiş bir kodla parola sıfırlanamaz. Amaç, kodun arandığı
/// her sorgunun parçasıdır.
/// </remarks>
public enum VerificationPurpose
{
    EmailConfirmation = 1,
    PasswordReset = 2
}
