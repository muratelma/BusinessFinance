using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using BusinessFinance.Application.Verification;

namespace BusinessFinance.Infrastructure.Verification;

/// <summary>
/// Altı haneli kodu üretir ve hash'ler.
/// </summary>
/// <remarks>
/// <see cref="RandomNumberGenerator"/> kullanılır, <c>Random</c> değil: tahmin
/// edilebilir bir kod, parola sıfırlamayı tahmin edilebilir bir kapıya çevirir.
/// Aralık <c>[0, 1_000_000)</c> üzerinden çekilir ve modulo sapması olmayan
/// <see cref="RandomNumberGenerator.GetInt32(int, int)"/> ile alınır.
///
/// Hash SHA-256'dır ve tuzsuzdur — refresh token'da olduğu gibi. Tuz burada bir
/// şey eklemez: kod on beş dakika yaşayan, beş denemede ölen ve tek kullanımlık
/// bir sırdır; saklanan hash'in offline kırılması için geçen sürede kodun
/// kendisi çoktan ölmüş olur.
/// </remarks>
internal sealed class VerificationCodeService : IVerificationCodeService
{
    public string CreateCode()
    {
        return RandomNumberGenerator
            .GetInt32(0, 1_000_000)
            .ToString("D6", CultureInfo.InvariantCulture);
    }

    public string HashCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Verification code is required.", nameof(code));
        }

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));
    }
}
