namespace BusinessFinance.Application.Authentication;

/// <summary>
/// Kullanıcı adının tek kuralı (kullanıcı kararı, 8 Kasım 2026): 3–30
/// karakter; İngilizce harf, rakam, nokta, alt çizgi ve tire. Büyük-küçük harf
/// fark etmez; ad küçük harfle saklanır.
/// </summary>
/// <remarks>
/// Türkçe harf yoktur: "yılmaz" ile "yilmaz" iki ayrı kişi olup girişte
/// telefonun hangi harfi yazdığına göre hesap bulunamayabilirdi. <c>@</c>
/// yoktur: girişteki tek alan e-postayı kullanıcı adından bu işaretle ayırır,
/// ve eski hesapların e-postayla dolu <c>UserName</c>'i yeni bir adla hiçbir
/// zaman çakışmaz. Boşluk yoktur: sonda kalan görünmez bir boşluk kişiyi
/// hesabından ederdi.
/// </remarks>
public static class Usernames
{
    public const int MinLength = 3;
    public const int MaxLength = 30;

    /// <summary>Kıyas ve saklama biçimi: kırpılmış, küçük harf.</summary>
    public static string Normalize(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.Trim().ToLowerInvariant();
    }

    /// <summary>Normalize edilmiş adın kurala uyup uymadığı.</summary>
    public static bool IsValid(string normalized)
    {
        ArgumentNullException.ThrowIfNull(normalized);
        if (normalized.Length is < MinLength or > MaxLength)
        {
            return false;
        }

        foreach (var character in normalized)
        {
            var allowed = character is >= 'a' and <= 'z'
                or >= '0' and <= '9'
                or '.' or '_' or '-';
            if (!allowed)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Girişteki tek alanın değeri kullanıcı adı mı (e-posta değil mi).
    /// </summary>
    public static bool LooksLikeUsername(string identifier) =>
        !identifier.Contains('@', StringComparison.Ordinal);
}
