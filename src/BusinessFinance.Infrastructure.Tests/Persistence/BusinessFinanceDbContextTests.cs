using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Infrastructure.Identity;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

public sealed class BusinessFinanceDbContextTests
{
    [Fact]
    public void AddInfrastructure_RegistersSqlServerDbContextAndEfIdentityStore()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        services.AddLogging();
        services.AddInfrastructure(configuration);

        var accountRepository = services.Single(
            descriptor => descriptor.ServiceType == typeof(IAccountRepository));
        var refreshRepository = services.Single(
            descriptor => descriptor.ServiceType == typeof(IRefreshSessionRepository));

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
        var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();

        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
        Assert.Contains(
            "Microsoft.AspNetCore.Identity.EntityFrameworkCore",
            userStore.GetType().FullName,
            StringComparison.Ordinal);
        Assert.Equal(ServiceLifetime.Scoped, accountRepository.Lifetime);
        Assert.Equal("EfAccountRepository", accountRepository.ImplementationType?.Name);
        Assert.Equal(ServiceLifetime.Scoped, refreshRepository.Lifetime);
        Assert.Equal("EfRefreshSessionRepository", refreshRepository.ImplementationType?.Name);
    }

    [Fact]
    public void Model_IncludesIdentityTablesAndUniqueNormalizedEmailIndex()
    {
        var options = new DbContextOptionsBuilder<BusinessFinanceDbContext>()
            .UseSqlServer("Server=(local);Database=ModelOnly;Integrated Security=true")
            .Options;
        using var context = new BusinessFinanceDbContext(options);

        var userEntity = context.Model.FindEntityType(typeof(ApplicationUser));
        var roleEntity = context.Model.FindEntityType(typeof(IdentityRole<Guid>));
        var emailIndex = userEntity?.GetIndexes().Single(
            index => index.Properties.Single().Name == nameof(ApplicationUser.NormalizedEmail));

        Assert.NotNull(userEntity);
        Assert.Equal("AspNetUsers", userEntity.GetTableName());
        Assert.NotNull(roleEntity);
        Assert.Equal("AspNetRoles", roleEntity.GetTableName());
        Assert.NotNull(emailIndex);
        Assert.True(emailIndex.IsUnique);
    }

    [Fact]
    public void AddInfrastructure_WithoutConnectionString_StopsAtCompositionRoot()
    {
        var configuration = CreateConfiguration(includeConnectionString: false);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(configuration);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>());

        Assert.Contains(BusinessFinanceDbContext.ConnectionStringName, exception.Message);
    }

    private static IConfiguration CreateConfiguration(bool includeConnectionString = true)
    {
        var values = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "BusinessFinance.Tests",
            ["Jwt:Audience"] = "BusinessFinance.Tests",
            ["Jwt:SigningKey"] =
                    "synthetic-infrastructure-tests-only-signing-key-123456789",
            ["Jwt:AccessTokenLifetime"] = "00:15:00",
            ["Jwt:RefreshTokenLifetime"] = "30.00:00:00"
        };

        if (includeConnectionString)
        {
            values["ConnectionStrings:BusinessFinance"] =
                "Server=(local);Database=BusinessFinanceTests;Integrated Security=true";
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
