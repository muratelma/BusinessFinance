using System.Xml.Linq;

namespace BusinessFinance.Application.Tests;

public sealed class ArchitectureTests
{
    [Fact]
    public void ApplicationProject_ReferencesOnlyDomainProject()
    {
        var repositoryRoot = FindRepositoryRoot();
        var projectPath = Path.Combine(
            repositoryRoot,
            "src",
            "BusinessFinance.Application",
            "BusinessFinance.Application.csproj");
        var project = XDocument.Load(projectPath);

        var projectReferences = project
            .Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .OfType<string>()
            .Select(reference => reference.Replace('\\', '/'))
            .ToArray();

        var packageReferences = project
            .Descendants("PackageReference")
            .ToArray();

        const string domainReference =
            "../BusinessFinance.Domain/BusinessFinance.Domain.csproj";

        Assert.Equal([domainReference], projectReferences);
        Assert.Empty(packageReferences);
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
