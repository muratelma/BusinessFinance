using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using BusinessFinance.Infrastructure.Persistence;
using BusinessFinance.Domain;
using BusinessFinance.Application.Attachments;

namespace BusinessFinance.Api.Tests;

public sealed class BusinessFinanceApiFactory : WebApplicationFactory<Program>
{
    private readonly string _environment;
    private readonly bool _useConfiguredSqlServer;
    private readonly Action<IServiceCollection>? _configureServices;

    public BusinessFinanceApiFactory(
        string environment = "Testing",
        bool useConfiguredSqlServer = false,
        Action<IServiceCollection>? configureServices = null)
    {
        _environment = environment;
        _useConfiguredSqlServer = useConfiguredSqlServer;
        _configureServices = configureServices;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(_environment);
        // Windows' default EventLog provider requires machine-level write
        // access. Integration tests run in a restricted process and assert HTTP
        // responses directly, so external log sinks would only turn a harmless
        // framework warning into a test failure.
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            var values = new Dictionary<string, string?>
            {
                ["Jwt:SigningKey"] =
                    "synthetic-integration-tests-only-signing-key-123456789"
            };

            values["ConnectionStrings:BusinessFinance"] = _useConfiguredSqlServer
                ? Environment.GetEnvironmentVariable(
                    "BUSINESS_FINANCE_SQL_TEST_CONNECTION") ??
                    throw new InvalidOperationException(
                        "BUSINESS_FINANCE_SQL_TEST_CONNECTION is required.")
                : "Server=(local);Database=BusinessFinanceTests;Integrated Security=true";

            configuration.AddInMemoryCollection(values);
        });

        builder.ConfigureServices(services =>
        {
            if (_useConfiguredSqlServer)
            {
                _configureServices?.Invoke(services);
                return;
            }

            var databaseName = $"business-finance-api-{Guid.NewGuid():N}";
            services.RemoveAll<DbContextOptions<BusinessFinanceDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<BusinessFinanceDbContext>>();
            services.RemoveAll<BusinessFinanceDbContext>();
            services.AddDbContext<BusinessFinanceDbContext>(options =>
                options.UseInMemoryDatabase(databaseName)
                    .AddInterceptors(new InMemoryRowVersionInterceptor()));
            services.RemoveAll<IAttachmentObjectStore>();
            services.AddSingleton<IAttachmentObjectStore, InMemoryAttachmentObjectStore>();
            _configureServices?.Invoke(services);
        });
    }

    private sealed class InMemoryAttachmentObjectStore : IAttachmentObjectStore
    {
        private readonly Dictionary<string, byte[]> _files = new(StringComparer.Ordinal);
        private readonly Lock _lock = new();

        public Task WriteAsync(
            string objectKey,
            ReadOnlyMemory<byte> content,
            CancellationToken cancellationToken)
        {
            lock (_lock) _files.Add(objectKey, content.ToArray());
            return Task.CompletedTask;
        }

        public Task<Stream?> OpenReadAsync(string objectKey, CancellationToken cancellationToken)
        {
            lock (_lock)
                return Task.FromResult<Stream?>(_files.TryGetValue(objectKey, out var content)
                    ? new MemoryStream(content, writable: false)
                    : null);
        }

        public Task DeleteIfExistsAsync(string objectKey, CancellationToken cancellationToken)
        {
            lock (_lock) _files.Remove(objectKey);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryRowVersionInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            SetVersions(eventData.Context);
            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            SetVersions(eventData.Context);
            return ValueTask.FromResult(result);
        }

        private static void SetVersions(DbContext? context)
        {
            if (context is null) return;
            foreach (var entry in context.ChangeTracker.Entries<RecurringTransactionOccurrence>()
                         .Where(item => item.State is EntityState.Added or EntityState.Modified))
            {
                entry.Property<byte[]>("Version").CurrentValue = Guid.NewGuid().ToByteArray();
            }

            foreach (var entry in context.ChangeTracker.Entries<ImportRow>()
                         .Where(entry => entry.State is EntityState.Added or EntityState.Modified))
            {
                entry.Property<byte[]>("Version").CurrentValue = Guid.NewGuid().ToByteArray();
            }

            foreach (var entry in context.ChangeTracker.Entries<DebtInstallment>()
                         .Where(entry => entry.State is EntityState.Added or EntityState.Modified))
            {
                entry.Property<byte[]>("Version").CurrentValue = Guid.NewGuid().ToByteArray();
            }
        }
    }
}
