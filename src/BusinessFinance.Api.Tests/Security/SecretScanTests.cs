using System.Text.RegularExpressions;

namespace BusinessFinance.Api.Tests.Security;

/// <summary>
/// Çalışma ağacının secret taraması (Aşama 06.1 Grup 1).
///
/// Tarama bilerek bir <b>test</b>: her <c>dotnet test</c> koşusunda ve CI'ın her
/// push'unda çalışır, ayrı bir araç kurulumu istemez. Repo geçmişinin ve
/// üretilen APK'nın taraması bir teste sığmaz; onlar
/// <c>scripts/Invoke-SecretScan.ps1</c> içindedir.
///
/// Aranan şey <b>değerin biçimidir</b>, anahtarın adı değil: bir sağlayıcı
/// anahtarı hangi değişkene atanırsa atansın aynı şekle sahiptir. Bulgu
/// raporlanırken değerin kendisi asla yazılmaz — yalnız dosya ve satır.
/// </summary>
public sealed class SecretScanTests
{
    private static readonly (string Name, Regex Pattern)[] SecretShapes =
    {
        ("Google API anahtarı", new Regex(@"AIza[0-9A-Za-z_\-]{30,}", RegexOptions.Compiled)),
        ("Brevo API anahtarı", new Regex(@"xkeysib-[0-9a-f]{16,}", RegexOptions.Compiled)),
        ("OpenAI benzeri anahtar", new Regex(@"\bsk-[A-Za-z0-9]{32,}", RegexOptions.Compiled)),
        ("GitHub token", new Regex(@"\bgh[pousr]_[A-Za-z0-9]{30,}", RegexOptions.Compiled)),
        ("AWS erişim anahtarı", new Regex(@"\bAKIA[0-9A-Z]{16}\b", RegexOptions.Compiled)),
        ("Özel anahtar bloğu", new Regex(@"-----BEGIN [A-Z ]*PRIVATE KEY-----", RegexOptions.Compiled)),
        ("Kodlanmış JWT", new Regex(@"\beyJhbGciOi[A-Za-z0-9._\-]{40,}", RegexOptions.Compiled)),

        // Parolalı connection string. Placeholder'lar elenir: `$degisken`,
        // `{...}` ve `Replace-With...` gerçek bir sır değildir.
        ("Parolalı connection string", new Regex(
            @"(?i)(Server|Data Source)=[^;\r\n]{1,80};[^\r\n]{0,120}?(Password|Pwd)=(?![$%{]|Replace-With)[^;\r\n]{4,}",
            RegexOptions.Compiled))
    };

    private static readonly string[] SkippedDirectories =
    {
        ".git", ".vs", ".idea", ".dart_tool", ".gradle", ".serena", ".claude",
        ".agents", "bin", "obj", "build", "node_modules", "Pods"
    };

    private static readonly string[] SkippedExtensions =
    {
        ".png", ".jpg", ".jpeg", ".gif", ".ico", ".pdf", ".ttf", ".otf",
        ".woff", ".woff2", ".zip", ".jar", ".apk", ".aab", ".so", ".dll",
        ".exe", ".pdb", ".bin", ".snk", ".keystore", ".jks"
    };

    /// <summary>2 MB üstü metin dosyası kaynak değil, üretilmiş artefakttır.</summary>
    private const long MaximumFileSizeBytes = 2 * 1024 * 1024;

    [Fact]
    public void WorkingTree_CarriesNoSecret()
    {
        var findings = Scan(RepositoryRoot.Find()).ToList();

        Assert.Empty(findings);
    }

    /// <summary>
    /// Hiçbir dosyayı okumayan bir tarama da yeşil görünür. Bu test taramanın
    /// gerçekten çalıştığını kanıtlar: bilinen bir şekli sentetik bir dosyaya
    /// yazar ve taramanın onu bulmasını bekler.
    /// </summary>
    [Fact]
    public void Scan_ActuallyDetectsAKnownShape()
    {
        var directory = Directory.CreateTempSubdirectory("business-finance-secret-scan");

        try
        {
            // Sentetik ve geçersiz bir değer; hiçbir sağlayıcıda karşılığı yok.
            File.WriteAllText(
                Path.Combine(directory.FullName, "leaked.txt"),
                "apiKey: AIza" + new string('0', 35));

            var findings = Scan(directory.FullName).ToList();

            Assert.Single(findings);
            Assert.Contains("Google API", findings[0]);
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Scan_ReadsTheRepositorySource()
    {
        // Kapının kaynağı gerçekten gezdiğinin kanıtı: bu dosyanın kendisi
        // taranan kümede olmalı.
        var scanned = TextFiles(RepositoryRoot.Find())
            .Select(Path.GetFileName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("SecretScanTests.cs", scanned);
        Assert.Contains("appsettings.json", scanned);
        Assert.Contains("compose.yaml", scanned);
    }

    private static IEnumerable<string> Scan(string root)
    {
        foreach (var file in TextFiles(root))
        {
            string[] lines;

            try
            {
                lines = File.ReadAllLines(file);
            }
            catch (IOException)
            {
                // Kilitli bir dosya bulgu değildir; tarama onu atlar.
                continue;
            }

            for (var index = 0; index < lines.Length; index++)
            {
                foreach (var (name, pattern) in SecretShapes)
                {
                    if (pattern.IsMatch(lines[index]))
                    {
                        // Eşleşen değer bilerek raporlanmaz.
                        yield return
                            $"{Path.GetRelativePath(root, file)}:{index + 1} → {name}";
                    }
                }
            }
        }
    }

    private static IEnumerable<string> TextFiles(string root)
    {
        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(root, file);

            if (relative
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(segment => SkippedDirectories.Contains(segment, StringComparer.OrdinalIgnoreCase)))
            {
                continue;
            }

            if (SkippedExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            if (new FileInfo(file).Length > MaximumFileSizeBytes)
            {
                continue;
            }

            yield return file;
        }
    }
}
