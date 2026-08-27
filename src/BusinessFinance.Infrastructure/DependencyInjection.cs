using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Accounts.DeleteAccount;
using BusinessFinance.Infrastructure.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Application.Pos;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.UserAccount;
using BusinessFinance.Infrastructure.Pos;
using BusinessFinance.Infrastructure.Taxes;
using BusinessFinance.Infrastructure.Profiles;
using BusinessFinance.Infrastructure.UserAccounts;
using BusinessFinance.Infrastructure.Categories;
using BusinessFinance.Infrastructure.Transactions;
using BusinessFinance.Application.Budgets;
using BusinessFinance.Infrastructure.Budgets;
using BusinessFinance.Application.Reports;
using BusinessFinance.Infrastructure.Reports;
using BusinessFinance.Application.Transfers;
using BusinessFinance.Infrastructure.Transfers;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Infrastructure.CreditCards;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Infrastructure.RecurringTransactions;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Application.UpcomingPayments;
using BusinessFinance.Infrastructure.FinancialActivities;
using BusinessFinance.Infrastructure.UpcomingPayments;
using BusinessFinance.Application.Imports;
using BusinessFinance.Infrastructure.Imports;
using BusinessFinance.Infrastructure.Identity;
using BusinessFinance.Infrastructure.Identity.Tokens;
using BusinessFinance.Infrastructure.Persistence;
using BusinessFinance.Application.DataPortability;
using BusinessFinance.Infrastructure.DataPortability;
using BusinessFinance.Application.Debts;
using BusinessFinance.Infrastructure.Debts;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Infrastructure.Counterparties;
using BusinessFinance.Application.SavingsGoals;
using BusinessFinance.Infrastructure.SavingsGoals;
using BusinessFinance.Application.Attachments;
using BusinessFinance.Infrastructure.Attachments;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Infrastructure.Receipts;
using BusinessFinance.Application.Cash;
using BusinessFinance.Application.Obligations;
using BusinessFinance.Infrastructure.Cash;
using BusinessFinance.Infrastructure.Obligations;

namespace BusinessFinance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.TryAddSingleton(configuration);

        services.AddDbContext<BusinessFinanceDbContext>((provider, options) =>
        {
            var currentConfiguration = provider.GetRequiredService<IConfiguration>();
            var connectionString = currentConfiguration.GetConnectionString(
                BusinessFinanceDbContext.ConnectionStringName) ??
                throw new InvalidOperationException(
                    $"Connection string '{BusinessFinanceDbContext.ConnectionStringName}' is required.");

            options.UseSqlServer(connectionString);
        });

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<BusinessFinanceDbContext>();

        services.AddScoped<IIdentityAccountService, IdentityAccountService>();

        services.AddScoped<IRefreshSessionRepository, EfRefreshSessionRepository>();
        services.AddSingleton<ISecurityTokenService, JwtSecurityTokenService>();
        services.AddScoped<IAccountRepository, EfAccountRepository>();
        services.AddScoped<IUnusedAccountDeletion, EfUnusedAccountDeletion>();
        services.AddScoped<ICategoryRepository, EfCategoryRepository>();
        services.AddScoped<IUserProfileRepository, EfUserProfileRepository>();
        services.AddScoped<IUserAccountEraser, EfUserAccountEraser>();
        services.AddScoped<IReceiptDuplicateLookup, EfReceiptDuplicateLookup>();
        services.AddScoped<IReceiptRefundLookup, EfReceiptRefundLookup>();
        services.AddScoped<ITransactionRepository, EfTransactionRepository>();
        services.AddScoped<IBudgetRepository, EfBudgetRepository>();
        services.AddScoped<IFinancialReportRepository, EfFinancialReportRepository>();
        services.AddScoped<ITransferRepository, EfTransferRepository>();
        services.AddScoped<ICreditCardRepository, EfCreditCardRepository>();
        services.AddScoped<ICardChargeRepository, EfCardChargeRepository>();
        services.AddScoped<ICardPaymentRepository, EfCardPaymentRepository>();
        services.AddScoped<ICreditCardStatementRepository, EfCreditCardStatementRepository>();
        services.AddScoped<IInstallmentPlanRepository, EfInstallmentPlanRepository>();
        services.AddScoped<IRecurringTransactionRepository, EfRecurringTransactionRepository>();
        // One instance serves both ports so a request that lists activities and then
        // checks an origin shares the same DbContext scope.
        services.AddScoped<EfFinancialActivityRepository>();
        services.AddScoped<IFinancialActivityRepository>(provider =>
            provider.GetRequiredService<EfFinancialActivityRepository>());
        services.AddScoped<IActivityOriginReader>(provider =>
            provider.GetRequiredService<EfFinancialActivityRepository>());
        services.AddScoped<IPlannedActivityRepository, EfPlannedActivityRepository>();
        services.AddScoped<IUpcomingPaymentRepository, EfUpcomingPaymentRepository>();
        services.AddScoped<IDataPortabilityRepository, EfDataPortabilityRepository>();
        services.AddScoped<IDebtRepository, EfDebtRepository>();
        services.AddScoped<ICounterpartyRepository, EfCounterpartyRepository>();
        services.AddScoped<IObligationRepository, EfObligationRepository>();
        services.AddScoped<ICashCountRepository, EfCashCountRepository>();
        services.AddScoped<IPosSettlementRepository, EfPosSettlementRepository>();
        services.AddScoped<IAccountantPackageRepository, EfAccountantPackageRepository>();
        services.AddScoped<ISavingsGoalRepository, EfSavingsGoalRepository>();
        services.AddScoped<IAttachmentRepository, EfAttachmentRepository>();
        services.AddSingleton<IAttachmentFileInspector, AttachmentFileInspector>();
        services.AddSingleton<IAttachmentObjectStore, LocalAttachmentObjectStore>();
        services.AddScoped<IImportBatchRepository, EfImportBatchRepository>();
        services.AddSingleton<ICsvImportParser, CsvImportParser>();
        services.AddSingleton<IReceiptImagePreprocessor, ReceiptImagePreprocessor>();
        services
            .AddHttpClient<IReceiptAnalyzer, GeminiReceiptAnalyzer>((provider, client) =>
            {
                var receiptOptions = provider.GetRequiredService<IOptions<GeminiOptions>>().Value;
                client.BaseAddress = new Uri(receiptOptions.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(receiptOptions.TimeoutSeconds);
            });

        services
            .AddOptions<AttachmentStorageOptions>()
            .Bind(configuration.GetSection(AttachmentStorageOptions.SectionName));

        services
            .AddOptions<ReceiptImageOptions>()
            .Bind(configuration.GetSection(ReceiptImageOptions.SectionName));

        // Deliberately not validated at startup: a missing API key switches the
        // receipt endpoint off, it does not stop the application from serving
        // every other feature.
        services
            .AddOptions<GeminiOptions>()
            .Bind(configuration.GetSection(GeminiOptions.SectionName));

        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetRequiredSection(JwtOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Issuer),
                "JWT issuer is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Audience),
                "JWT audience is required.")
            .Validate(
                options => Encoding.UTF8.GetByteCount(options.SigningKey) >= 32,
                "JWT signing key must contain at least 32 bytes.")
            .Validate(
                options => options.AccessTokenLifetime > TimeSpan.Zero &&
                           options.RefreshTokenLifetime > TimeSpan.Zero,
                "JWT token lifetimes must be positive.")
            .ValidateOnStart();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearerOptions, jwtOptions) =>
            {
                var options = jwtOptions.Value;
                bearerOptions.MapInboundClaims = false;
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(options.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = JwtRegisteredClaimNames.Sub
                };
            });

        services.AddAuthorization();

        return services;
    }
}
