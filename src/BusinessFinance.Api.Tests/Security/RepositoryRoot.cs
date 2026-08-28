namespace BusinessFinance.Api.Tests.Security;

/// <summary>
/// Kaynak ağacını tarayan testlerin ortak çıkış noktası.
///
/// Test ikilisi <c>bin/</c> altından koştuğu için çalışma dizini kaynak
/// ağacının kökü değildir; kök, <c>BusinessFinance.slnx</c> aranarak bulunur.
/// </summary>
internal static class RepositoryRoot
{
    private const string SolutionFileName = "BusinessFinance.slnx";

    public static string Find()
    {
        var configuredRoot = Environment.GetEnvironmentVariable(
            "BUSINESS_FINANCE_REPOSITORY_ROOT");
        if (!string.IsNullOrWhiteSpace(configuredRoot) &&
            File.Exists(Path.Combine(configuredRoot, SolutionFileName)))
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
                if (File.Exists(Path.Combine(directory.FullName, SolutionFileName)))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }
        }

        throw new DirectoryNotFoundException("Repository root could not be found.");
    }
}
