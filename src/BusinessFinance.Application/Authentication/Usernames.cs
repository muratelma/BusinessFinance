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
    /// E-postanın <c>@</c> öncesinden kurala uyan bir ad tabanı (Google ile ilk
    /// girişteki öneri). Kural <c>BackfillUsernamesFromEmail</c> migration'ıyla
    /// aynıdır: küçük harf; yalnız a–z, rakam, nokta, alt çizgi ve tire kalır;
    /// 3 karakterden kısa kalırsa başına "kullanici" eklenir; en çok 30 karakter.
    /// Ad alınmışsa sona eklenecek sayı çağıranındır (<see cref="WithNumber"/>).
    /// </summary>
    public static string SuggestFromEmail(string email)
    {
        ArgumentNullException.ThrowIfNull(email);
        var at = email.IndexOf('@', StringComparison.Ordinal);
        var local = (at < 0 ? email : email[..at]).ToLowerInvariant();

        var builder = new System.Text.StringBuilder(local.Length);
        foreach (var character in local)
        {
            if (character is >= 'a' and <= 'z' or >= '0' and <= '9' or '.' or '_' or '-')
            {
                builder.Append(character);
            }
        }

        var suggestion = builder.ToString();
        if (suggestion.Length < MinLength)
        {
            suggestion = "kullanici" + suggestion;
        }

        return suggestion.Length > MaxLength ? suggestion[..MaxLength] : suggestion;
    }

    /// <summary>
    /// Tabanın sonuna sayı ekler; ad 30 karakteri geçmesin diye taban kısalır
    /// (<c>ali</c>, 2 → <c>ali2</c>).
    /// </summary>
    public static string WithNumber(string suggestion, int number)
    {
        ArgumentNullException.ThrowIfNull(suggestion);
        var suffix = number.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var room = MaxLength - suffix.Length;
        return (suggestion.Length > room ? suggestion[..room] : suggestion) + suffix;
    }

    /// <summary>
    /// Girişteki tek alanın değeri kullanıcı adı mı (e-posta değil mi).
    /// </summary>
    public static bool LooksLikeUsername(string identifier) =>
        !identifier.Contains('@', StringComparison.Ordinal);
}
