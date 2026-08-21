using System.Text.Json;

namespace BusinessFinance.Api.Tests.Security;

public sealed class ConfigurationSecurityTests
{
    [Fact]
    public void Appsettings_DoesNotContainJwtSigningKey()
    {
        var repositoryRoot = FindRepositoryRoot();
        var appsettingsPath = Path.Combine(
            repositoryRoot,
            "src",
            "BusinessFinance.Api",
            "appsettings.json");
        using var document = JsonDocument.Parse(File.ReadAllText(appsettingsPath));
        var jwt = document.RootElement.GetProperty("Jwt");

        Assert.False(jwt.TryGetProperty("SigningKey", out _));
    }

    [Fact]
    public void Appsettings_DoesNotContainConnectionStrings()
    {
        var repositoryRoot = FindRepositoryRoot();
        var appsettingsPath = Path.Combine(
            repositoryRoot,
            "src",
            "BusinessFinance.Api",
            "appsettings.json");
        using var document = JsonDocument.Parse(File.ReadAllText(appsettingsPath));

        Assert.False(document.RootElement.TryGetProperty("ConnectionStrings", out _));
    }

    private static string FindRepositoryRoot()
    {
        var configuredRoot = Environment.GetEnvironmentVariable(
            "BUSINESS_FINANCE_REPOSITORY_ROOT");
        if (!string.IsNullOrWhiteSpace(configuredRoot) &&
            File.Exists(Path.Combine(configuredRoot, "BusinessFinance.slnx")))
        {
            return Path.GetFullPath(configuredRoot);
        }

        string[] startingPaths =
        {
            Directory.GetCurrentDirectory(),
            AppContext.BaseDirectory
        };

        foreach (var startingPath in startingPaths)
        {
            var directory = new DirectoryInfo(startingPath);

            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "BusinessFinance.slnx")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }
        }

        throw new DirectoryNotFoundException("Repository root could not be found.");
    }
}
