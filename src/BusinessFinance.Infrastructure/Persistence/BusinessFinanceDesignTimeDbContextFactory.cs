using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BusinessFinance.Infrastructure.Persistence;

public sealed class BusinessFinanceDesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<BusinessFinanceDbContext>
{
    private const string ConnectionStringEnvironmentVariable =
        "ConnectionStrings__BusinessFinance";

    public BusinessFinanceDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            ConnectionStringEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Environment variable '{ConnectionStringEnvironmentVariable}' is required for EF tools.");
        }

        var options = new DbContextOptionsBuilder<BusinessFinanceDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new BusinessFinanceDbContext(options);
    }
}
