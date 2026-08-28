using System.Text.Json;

namespace BusinessFinance.Api.Tests.Security;

/// <summary>
/// Commit edilen yapılandırma dosyalarının secret taşımadığının kapısı.
///
/// Aşama 06.1 Grup 1: kapı eskiden yalnız <c>appsettings.json</c> içindeki iki
/// anahtara bakıyordu. Sağlayıcı anahtarları (Gemini, Brevo) listede yoktu ve
/// <c>appsettings.Development.json</c> hiç taranmıyordu — geliştirme dosyası da
/// commit edilen bir dosyadır ve yerel çalışırken elle doldurulması en olası
/// yerdir.
/// </summary>
public sealed class ConfigurationSecurityTests
{
    /// <summary>
    /// Değeri yalnız user-secrets veya environment içinde yaşayan anahtarlar.
    /// Liste <c>documentation/variables.md</c> envanterinin koddaki karşılığıdır;
    /// yeni bir sağlayıcı eklendiğinde anahtarı buraya da yazılır.
    /// </summary>
    private static readonly string[] SecretKeyPaths =
    {
        "ConnectionStrings",
        "Jwt:SigningKey",
        "Gemini:ApiKey",
        "Brevo:ApiKey"
    };

    [Fact]
    public void Appsettings_CarryNoSecretKey()
    {
        var offenders = new List<string>();

        foreach (var file in ConfigurationFiles())
        {
            var keyPaths = KeyPaths(file);

            foreach (var secret in SecretKeyPaths)
            {
                if (keyPaths.Contains(secret))
                {
                    // Değerin kendisi hiçbir yere yazılmaz; bulgunun kaydı
                    // dosya adı ve anahtar yoludur.
                    offenders.Add($"{Path.GetFileName(file)} → {secret}");
                }
            }
        }

        Assert.Empty(offenders);
    }

    /// <summary>
    /// Taranacak dosya kalmadığında test sessizce geçerdi; bir yeniden
    /// adlandırma kapıyı boşaltmasın diye dosya kümesi de doğrulanır.
    /// </summary>
    [Fact]
    public void ConfigurationFiles_AreActuallyFound()
    {
        var files = ConfigurationFiles()
            .Select(Path.GetFileName)
            .ToList();

        Assert.Contains("appsettings.json", files);
        Assert.Contains("appsettings.Development.json", files);
    }

    private static IReadOnlyList<string> ConfigurationFiles()
    {
        var apiProject = Path.Combine(
            RepositoryRoot.Find(),
            "src",
            "BusinessFinance.Api");

        return Directory
            .GetFiles(apiProject, "appsettings*.json", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();
    }

    private static HashSet<string> KeyPaths(string file)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(file));
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Collect(document.RootElement, prefix: string.Empty, paths);
        return paths;
    }

    private static void Collect(JsonElement element, string prefix, HashSet<string> paths)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        foreach (var property in element.EnumerateObject())
        {
            var path = prefix.Length == 0
                ? property.Name
                : $"{prefix}:{property.Name}";
            paths.Add(path);
            Collect(property.Value, path, paths);
        }
    }
}
