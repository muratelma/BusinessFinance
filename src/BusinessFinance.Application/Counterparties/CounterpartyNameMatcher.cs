using System.Text;

namespace BusinessFinance.Application.Counterparties;

/// <summary>
/// Belgeden okunan bir adı kullanıcının kişileriyle <b>hoşgörülü</b> eşler.
/// </summary>
/// <remarks>
/// <para>
/// Kişileri kullanıcı elle yazar; belgede aynı ad büyük harfle, şirket
/// unvanıyla ya da küçük bir yazım farkıyla geçer ("ÖRNEK ELEKTRİK DAĞITIM
/// A.Ş." ile "Örnek Elektrik Dağıtım"). Birebir karşılaştırma bu yüzden
/// neredeyse hiç eşleşmiyordu; Türkçe büyük-küçük harf (İ/i, I/ı) tek başına
/// eşleşmeyi bozuyordu.
/// </para>
/// <para>
/// Sonuç bir <b>öneridir</b>: formda seçili gelir, kullanıcı değiştirir. Yine
/// de yanlış kişiyi seçili getirmek, hiç getirmemekten kötüdür; kurallar bu
/// yüzden dardır:
/// </para>
/// <list type="number">
/// <item>Aynı ad (harf büyüklüğü, Türkçe harf, noktalama, sondaki şirket
/// unvanı ve boşluk farkı sayılmaz).</item>
/// <item>Biri öbürünün içinde, <b>kelimeler bitişik ve aynı sırada</b>. Tek
/// kelimelik ad yalnız uzun adın <b>ilk</b> kelimesiyse sayılır: "Enerjisa",
/// "Enerjisa Başkent Elektrik" ile eşleşir; "Market", "Şok Market" ile
/// eşleşmez — sektör kelimesi kimseyi ayırt etmez.</item>
/// <item>Tek harflik yazım farkı, <b>yalnız bir kelimede</b> ve o kelime en az
/// beş harfliyse ("Ahmet" / "Ahmed"). "Kaya" ile "Kara", "Yılmaz" ile "Yıldız"
/// ayrı kişilerdir ve eşleşmez.</item>
/// </list>
/// <para>
/// En iyi düzeyde birden çok aday varsa hiçbiri önerilmez. Teklik kuralı ("bu
/// ad zaten var") bu eşleştirmeyi <b>kullanmaz</b>; o yalnız harf büyüklüğüne
/// bakar (<c>Counterparty.NameKeyOf</c>).
/// </para>
/// </remarks>
public static class CounterpartyNameMatcher
{
    private const int MinimumSingleWordLength = 3;
    private const int MinimumTypoWordLength = 5;

    // Adın sonundaki şirket türü kimseyi ayırt etmez. "A.Ş." noktalama gidince
    // "a" ve "s" olarak kalır. Yalnız sondan atılır: "A Plus" ya da "Ayşe ve
    // Fatma" gibi adların içindeki aynı kelimeler adın parçasıdır.
    private static readonly HashSet<string> TrailingTitles = new(StringComparer.Ordinal)
    {
        "a", "s", "as", "ltd", "sti", "tic", "san", "ve", "anonim", "limited",
        "sirketi", "sirket",
    };

    /// <summary>
    /// <paramref name="name"/> ile en iyi eşleşen adayın kimliği; aday yoksa
    /// ya da en iyi düzeyde birden çok aday varsa <c>null</c>.
    /// </summary>
    public static Guid? FindBest(
        string? name,
        IEnumerable<(Guid Id, string Name)> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        var target = Tokens(name);
        if (target.Length == 0)
        {
            return null;
        }

        var scored = candidates
            .Select(candidate => (candidate.Id, Score: Score(target, Tokens(candidate.Name))))
            .Where(item => item.Score > 0)
            .ToArray();
        if (scored.Length == 0)
        {
            return null;
        }

        var best = scored.Max(item => item.Score);
        var winners = scored.Where(item => item.Score == best).ToArray();
        return winners.Length == 1 ? winners[0].Id : null;
    }

    /// <summary>
    /// 0: uymuyor. Aynı ad en yüksek; içinde geçen ad, eşleşen kelime sayısı
    /// kadar yükselir ("Enerjisa Başkent", "Enerjisa"nın önüne geçer); yazım
    /// farkı en düşük.
    /// </summary>
    private static int Score(string[] target, string[] candidate)
    {
        const int Exact = 3000;
        const int Contained = 2000;
        const int Typo = 1000;

        if (candidate.Length == 0)
        {
            return 0;
        }

        // "A 101" ile "A101": boşluk farkı ad farkı değildir.
        if (string.Equals(string.Concat(target), string.Concat(candidate), StringComparison.Ordinal))
        {
            return Exact;
        }

        var shorter = target.Length <= candidate.Length ? target : candidate;
        var longer = ReferenceEquals(shorter, target) ? candidate : target;
        if (shorter.Length < longer.Length && IsContained(shorter, longer))
        {
            return Contained + shorter.Length;
        }

        return IsSingleTypo(target, candidate) ? Typo : 0;
    }

    private static bool IsContained(string[] shorter, string[] longer)
    {
        if (shorter.Length == 1)
        {
            return shorter[0].Length >= MinimumSingleWordLength &&
                   string.Equals(shorter[0], longer[0], StringComparison.Ordinal);
        }

        for (var start = 0; start + shorter.Length <= longer.Length; start++)
        {
            if (longer.AsSpan(start, shorter.Length).SequenceEqual(shorter))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSingleTypo(string[] left, string[] right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }

        var differing = -1;
        for (var index = 0; index < left.Length; index++)
        {
            if (string.Equals(left[index], right[index], StringComparison.Ordinal))
            {
                continue;
            }

            if (differing >= 0)
            {
                return false;
            }

            differing = index;
        }

        return differing >= 0 &&
               Math.Min(left[differing].Length, right[differing].Length) >= MinimumTypoWordLength &&
               IsOneEditApart(left[differing], right[differing]);
    }

    /// <summary>
    /// Tek harf eksik, fazla, yanlış ya da yan yana iki harf yer değiştirmiş.
    /// </summary>
    private static bool IsOneEditApart(string left, string right)
    {
        if (left.Length == right.Length)
        {
            var first = -1;
            var second = -1;
            for (var index = 0; index < left.Length; index++)
            {
                if (left[index] == right[index])
                {
                    continue;
                }

                if (first < 0)
                {
                    first = index;
                }
                else if (second < 0)
                {
                    second = index;
                }
                else
                {
                    return false;
                }
            }

            return second < 0 ||
                   (second == first + 1 && left[first] == right[second] && left[second] == right[first]);
        }

        var shorter = left.Length < right.Length ? left : right;
        var longer = ReferenceEquals(shorter, left) ? right : left;
        if (longer.Length - shorter.Length != 1)
        {
            return false;
        }

        var skipped = 0;
        for (var index = 0; index < shorter.Length; index++)
        {
            if (shorter[index] == longer[index + skipped])
            {
                continue;
            }

            if (skipped == 1)
            {
                return false;
            }

            skipped = 1;
            index--;
        }

        return true;
    }

    /// <summary>
    /// Adı karşılaştırılabilir kelimelere indirir: Türkçe harfler sadeleşir,
    /// noktalama gider, sondaki şirket türü kelimeleri atılır.
    /// </summary>
    private static string[] Tokens(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return [];
        }

        var builder = new StringBuilder(name.Length);
        foreach (var character in name)
        {
            builder.Append(character switch
            {
                'İ' or 'I' or 'ı' or 'i' => 'i',
                'Ş' or 'ş' => 's',
                'Ğ' or 'ğ' => 'g',
                'Ü' or 'ü' => 'u',
                'Ö' or 'ö' => 'o',
                'Ç' or 'ç' => 'c',
                // "i̇": bazı klavyeler küçük i'yi nokta işaretiyle birlikte yazar.
                '̇' => '\0',
                _ when char.IsLetterOrDigit(character) => char.ToLowerInvariant(character),
                _ => ' ',
            });
        }

        var tokens = builder.Replace("\0", string.Empty).ToString()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var length = tokens.Length;
        while (length > 1 && TrailingTitles.Contains(tokens[length - 1]))
        {
            length--;
        }

        return tokens[..length];
    }
}
