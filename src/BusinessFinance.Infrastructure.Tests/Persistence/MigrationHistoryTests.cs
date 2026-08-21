using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

/// <summary>
/// Şema tek bir <c>InitialCreate</c> ile kurulur; ürünün yayımlanmış bir sürümü
/// olmadığı için yükseltme zinciri taşınmadı. Bu test o tekliği ve şemanın
/// modelle birebir örtüştüğünü korur: ikinci bir migration eklendiğinde ilk
/// madde bilerek kırılır ve o an bu testin yerini zincir testi alır.
/// </summary>
public sealed class MigrationHistoryTests
{
    [Fact]
    public void Migrations_AreASingleInitialCreateThatMatchesTheModel()
    {
        var options = new DbContextOptionsBuilder<BusinessFinanceDbContext>()
            .UseSqlServer("Server=(local);Database=ModelOnly;Integrated Security=true")
            .Options;
        using var context = new BusinessFinanceDbContext(options);
        var migrationsAssembly = context.GetService<IMigrationsAssembly>();
        var migrations = migrationsAssembly.Migrations
            .Select(entry => new
            {
                Id = entry.Key,
                Migration = migrationsAssembly.CreateMigration(
                    entry.Value,
                    context.Database.ProviderName!)
            })
            .ToArray();

        var only = Assert.Single(migrations);
        Assert.EndsWith("_InitialCreate", only.Id, StringComparison.Ordinal);

        // Asıl kapı: migration ile model arasında fark kalmamalı. Bir entity
        // değişip migration üretilmediğinde burası kırılır.
        Assert.False(context.Database.HasPendingModelChanges());

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
