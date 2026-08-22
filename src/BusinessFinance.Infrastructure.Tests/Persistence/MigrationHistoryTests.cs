using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

/// <summary>
/// Şema tek bir <c>InitialCreate</c> ile kuruldu (ADR 0012) ve o serbestlik bir
/// daha kullanılmıyor: <c>AddTransactionScope</c> zincirin ilk gerçek yükseltme
/// adımıdır. Bu test artık tekliği değil <b>zinciri</b> koruyor — sırasını,
/// yükseltme güvenliği kurallarına uyduğunu ve şemanın modelle birebir
/// örtüştüğünü.
/// </summary>
public sealed class MigrationHistoryTests
{
    /// <summary>
    /// Zincirin beklenen sırası. Yeni migration eklendiğinde bu liste büyür;
    /// araya bir ad sıkıştırmak veya bir adımı yeniden üretmek burayı kırar.
    /// </summary>
    private static readonly string[] ExpectedChain =
    [
        "InitialCreate",
        "AddTransactionScope",
        "AddUserProfile"
    ];

    [Fact]
    public void Migrations_FormTheExpectedChainAndMatchTheModel()
    {
        var migrations = LoadMigrations(out var context);
        using (context)
        {
            Assert.Equal(
                ExpectedChain,
                migrations.Select(entry => entry.Id.Split('_', 2)[1]).ToArray());

            // Sıra zaman damgasından okunur; EF migration'ları id sırasına göre
            // uygular ve bu id'ler artan olmak zorundadır.
            Assert.Equal(
                migrations.Select(entry => entry.Id).OrderBy(id => id, StringComparer.Ordinal).ToArray(),
                migrations.Select(entry => entry.Id).ToArray());

            // Asıl kapı: migration ile model arasında fark kalmamalı. Bir entity
            // değişip migration üretilmediğinde burası kırılır.
            Assert.False(context.Database.HasPendingModelChanges());
        }
    }

    [Fact]
    public void InitialCreate_BuildsTheWholeSchemaWithItsFinancialGuards()
    {
        var only = LoadMigrations(out var context).First();
        using (context)
        {
            var up = only.Migration.UpOperations;
            var tables = up.OfType<CreateTableOperation>().ToArray();

            Assert.Equal(27, tables.Length);
            Assert.Equal(65, up.OfType<CreateIndexOperation>().Count());
            Assert.Equal(27, only.Migration.DownOperations.OfType<DropTableOperation>().Count());
            Assert.Equal(81, tables.Sum(table => table.CheckConstraints.Count));
            Assert.Equal(58, tables.Sum(table => table.ForeignKeys.Count));
            Assert.Equal(11, tables.Sum(table => table.UniqueConstraints.Count));

            // Bakiye kalıcı kolon değildir; açılış bakiyesi kısıtla korunur.
            AssertCheck("Accounts", "CK_Accounts_OpeningBalance");

            // Transfer gelir/gider değildir: iki farklı hesap ve tek para birimi şart.
            AssertCheck("Transfers", "CK_Transfers_Amount");
            AssertCheck("Transfers", "CK_Transfers_Currency");
            AssertCheck("Transfers", "CK_Transfers_DifferentAccounts");

            AssertCheck("CreditCards", "CK_CreditCards_Limit");
            AssertCheck("CreditCards", "CK_CreditCards_StatementClosingDay");
            AssertCheck("SavingsGoals", "CK_SavingsGoals_ProgressSource");
            AssertCheck("ImportRows", "CK_ImportRows_Validation");
            AssertCheck("ImportRows", "CK_ImportRows_SignedAmountRange");
            AssertCheck("FinancialAttachments", "CK_FinancialAttachments_ContentType");

            // Tekrarlayan planın kaynağı hesap ya da karttır — tam olarak biri.
            AssertCheck("RecurringTransactions", "CK_RecurringTransactions_Source");

            // Borcun açılış kaynağı ve yönü SQL seviyesinde de bağlıdır.
            AssertCheck("DebtAgreements", "CK_DebtAgreements_Source");
            AssertCheck("DebtAgreements", "CK_DebtAgreements_DirectionalSource");

            // Ayrım kolonları nullable olmak zorunda: bu ayrımdan önce oluşmuş
            // taksitlerin anapara/faiz payı bilinmiyor ve uydurulamaz.
            var installments = tables.Single(table => table.Name == "DebtInstallments");
            Assert.All(
                installments.Columns.Where(column =>
                    column.Name is "PrincipalPortion" or "InterestPortion"),
                column => Assert.True(column.IsNullable, $"{column.Name} nullable olmalı."));
            AssertCheck("DebtInstallments", "CK_DebtInstallments_Split");

            // Eşzamanlı yazımı ayıran rowversion kolonları.
            Assert.Equal(
                3,
                tables.SelectMany(table => table.Columns)
                    .Count(column => column.ColumnType == "rowversion"));

            // Idempotency ve sahiplik izolasyonunu taşıyan tekil indeksler.
            AssertUniqueIndex("UX_InstallmentPlans_UserId_ClientRequestId");
            AssertUniqueIndex("UX_RecurringOccurrences_UserId_OccurrenceKey");
            AssertUniqueIndex("UX_ImportRows_UserId_BatchId_RowNumber");
            AssertUniqueIndex("UX_ImportBatches_UserId_FileFingerprint");

            // Bir occurrence en fazla bir kart harcaması üretir; filtre NULL'ları
            // kapsam dışında bırakır, yoksa gerçekleşmemiş satırlar çakışırdı.
            var chargeIndex = SingleIndex("UX_RecurringOccurrences_UserId_ChargeId");
            Assert.True(chargeIndex.IsUnique);
            Assert.NotNull(chargeIndex.Filter);

            // Occurrence'ın hareketi sahiplik kapsamıyla bağlanabilsin diye.
            Assert.Contains(
                tables.Single(table => table.Name == "BudgetTransactions").UniqueConstraints,
                constraint => constraint.Name == "AK_BudgetTransactions_UserId_Id");

            // Rapor sorgularının dayandığı indeksler.
            Assert.Equal(
                ["UserId", "Year", "Month"],
                SingleIndex("IX_MonthlyBudgets_UserId_Year_Month").Columns);
            AssertIndexExists("IX_RecurringTransactions_UserId_IsActive_NextDate");

            void AssertCheck(string table, string constraint) => Assert.Contains(
                tables.Single(item => item.Name == table).CheckConstraints,
                item => item.Name == constraint);

            CreateIndexOperation SingleIndex(string name) => Assert.Single(
                up.OfType<CreateIndexOperation>(),
                index => index.Name == name);

            void AssertIndexExists(string name) => Assert.Contains(
                up.OfType<CreateIndexOperation>(),
                index => index.Name == name);

            void AssertUniqueIndex(string name)
            {
                var index = SingleIndex(name);
                Assert.True(index.IsUnique, $"{name} tekil olmalı.");
            }
        }
    }

    /// <summary>
    /// Kapsam kolonlarını ekleyen adım yükseltme güvenliği kurallarına uyuyor mu:
    /// kolonlar kısıtlardan önce geliyor, zorunlu kolonlar kalıcı bir veritabanı
    /// varsayılanı bırakmıyor ve isteğe bağlı olanlar nullable.
    /// </summary>
    [Fact]
    public void AddTransactionScope_AddsColumnsBeforeConstraintsAndLeavesNoDefault()
    {
        var scopeMigration = LoadMigrations(out var context)
            .Single(entry => entry.Id.EndsWith("_AddTransactionScope", StringComparison.Ordinal));
        using (context)
        {
            var up = scopeMigration.Migration.UpOperations.ToList();

            var firstCheck = up.FindIndex(operation => operation is AddCheckConstraintOperation);
            var lastColumn = up.FindLastIndex(operation =>
                operation is AddColumnOperation or SqlOperation);
            Assert.True(
                lastColumn < firstCheck,
                "Kolonlar bütün CHECK kısıtlarından önce eklenmeli.");

            // Zorunlu kapsam kolonları ham SQL ile, varsayılansız ekleniyor:
            // tablolar Aşama 01 Grup 1'de boşaltıldı. Bir defaultValue bırakmak
            // hem kalıcı bir veritabanı varsayılanı hem de [Scope] IN (1, 2)
            // kısıtını ihlal eden bir değer bırakırdı.
            var scopeAdds = up.OfType<SqlOperation>()
                .Where(operation => operation.Sql.Contains("[Scope] tinyint NOT NULL", StringComparison.Ordinal))
                .ToArray();
            Assert.Equal(7, scopeAdds.Length);
            Assert.All(scopeAdds, operation =>
                Assert.DoesNotContain("DEFAULT", operation.Sql, StringComparison.OrdinalIgnoreCase));

            // Varsayılan kapsam boş bırakılabilir: tek hesabıyla her şeyi yöneten
            // esnaf için kapsam kategoriden türer.
            var defaultScopeColumns = up.OfType<AddColumnOperation>()
                .Where(operation => operation.Name == "DefaultScope")
                .ToArray();
            Assert.Equal(3, defaultScopeColumns.Length);
            Assert.All(defaultScopeColumns, operation =>
            {
                Assert.True(operation.IsNullable);
                Assert.Null(operation.DefaultValue);
                Assert.Null(operation.DefaultValueSql);
            });

            Assert.Equal(10, up.OfType<AddCheckConstraintOperation>().Count());
        }
    }

    private static IReadOnlyList<MigrationEntry> LoadMigrations(out BusinessFinanceDbContext context)
    {
        var options = new DbContextOptionsBuilder<BusinessFinanceDbContext>()
            .UseSqlServer("Server=(local);Database=ModelOnly;Integrated Security=true")
            .Options;
        context = new BusinessFinanceDbContext(options);
        var migrationsAssembly = context.GetService<IMigrationsAssembly>();
        var providerName = context.Database.ProviderName!;
        return
        [
            .. migrationsAssembly.Migrations.Select(entry => new MigrationEntry(
                entry.Key,
                migrationsAssembly.CreateMigration(entry.Value, providerName)))
        ];
    }

    private sealed record MigrationEntry(string Id, Migration Migration);
}
