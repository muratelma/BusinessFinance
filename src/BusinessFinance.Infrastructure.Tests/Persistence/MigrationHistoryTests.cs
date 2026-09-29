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
        "AddUserProfile",
        "AddCounterparties",
        "LinkDebtsToCounterparties",
        "AddObligationsAndCounterpartyDueDates",
        "AddRecurringOccurrenceLimit",
        "AddCashCountsAndPosSettlements",
        "AddVatFields",
        "AddTaxDeductibility",
        "AddQuarterlyRecurrence",
        "AddSavingsGoalScope",
        "AddVerificationCodes",
        "AddCashCountExpectedSnapshot",
        "RemoveVatAndTaxDeductibility"
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

    /// <summary>
    /// Cari hesabın üç tablosu boş doğuyor: adım yalnız tablo kuruyor, mevcut
    /// bir tabloya dokunmuyor. Backfill kuralı bu yüzden devreye girmiyor —
    /// yorumlanacak bir geçmiş yok (AGENTS.md, "gerçekten boş tablo" istisnası).
    /// </summary>
    [Fact]
    public void AddCounterparties_OnlyCreatesEmptyTablesWithOwnerScopedKeys()
    {
        var migration = LoadMigrations(out var context)
            .Single(entry => entry.Id.EndsWith("_AddCounterparties", StringComparison.Ordinal));
        using (context)
        {
            var up = migration.Migration.UpOperations.ToList();
            var tables = up.OfType<CreateTableOperation>().ToArray();

            Assert.Equal(
                ["Counterparties", "CounterpartyCharges", "CounterpartyPayments"],
                tables.Select(table => table.Name).OrderBy(name => name, StringComparer.Ordinal).ToArray());

            // Mevcut hiçbir tabloya kolon veya kısıt eklenmiyor.
            Assert.Empty(up.OfType<AddColumnOperation>());
            Assert.Empty(up.OfType<AddCheckConstraintOperation>());
            Assert.Equal(
                tables.Length + up.OfType<CreateIndexOperation>().Count(),
                up.Count);

            // Sahiplik izolasyonu foreign key'in kendisinde: hareketler karşı
            // tarafa (UserId, Id) ile bağlanır, başka kullanıcının karşı tarafı
            // veritabanı seviyesinde yazılamaz.
            foreach (var name in new[] { "CounterpartyCharges", "CounterpartyPayments" })
            {
                var ownerScoped = tables.Single(table => table.Name == name).ForeignKeys
                    .Single(key => key.PrincipalTable == "Counterparties");
                Assert.Equal(["UserId", "CounterpartyId"], ownerScoped.Columns);
                Assert.NotNull(ownerScoped.PrincipalColumns);
                Assert.Equal(["UserId", "Id"], ownerScoped.PrincipalColumns);
            }

            // Tahsilat parayı taşır, gelir/gider tanımaz: kategori ve kapsam
            // kolonu hiç yok (ADR 0014).
            var payments = tables.Single(table => table.Name == "CounterpartyPayments");
            Assert.DoesNotContain(payments.Columns, column => column.Name is "CategoryId" or "Scope");

            // Aynı kişi iki kez oluşamaz.
            Assert.Contains(
                up.OfType<CreateIndexOperation>(),
                index => index.Name == "UX_Counterparties_UserId_Name" && index.IsUnique);
        }
    }

    /// <summary>
    /// Eski cari hareketlerin vadesi bilinemez; kolon bu yüzden nullable ve
    /// varsayılansızdır. Yükümlülük tabloları ise bu adımda boş doğar, dolayısıyla
    /// kendi zorunlu alanları backfill gerektirmez.
    /// </summary>
    [Fact]
    public void AddObligationsAndCounterpartyDueDates_PreservesUnknownHistoryAndOwnerScope()
    {
        var migration = LoadMigrations(out var context).Single(entry =>
            entry.Id.EndsWith(
                "_AddObligationsAndCounterpartyDueDates",
                StringComparison.Ordinal));
        using (context)
        {
            var up = migration.Migration.UpOperations.ToList();
            var dueDate = Assert.Single(up.OfType<AddColumnOperation>(), operation =>
                operation.Table == "CounterpartyCharges" && operation.Name == "DueDate");
            Assert.True(dueDate.IsNullable);
            Assert.Null(dueDate.DefaultValue);
            Assert.Null(dueDate.DefaultValueSql);

            var tables = up.OfType<CreateTableOperation>().ToArray();
            Assert.Equal(
                ["ObligationSettlements", "Obligations"],
                tables.Select(table => table.Name)
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToArray());

            var obligations = tables.Single(table => table.Name == "Obligations");
            var settlements = tables.Single(table => table.Name == "ObligationSettlements");
            Assert.DoesNotContain(obligations.Columns, column => column.Name == "AccountId");
            Assert.DoesNotContain(
                settlements.Columns,
                column => column.Name is "CategoryId" or "Scope");

            var categoryKey = obligations.ForeignKeys.Single(key =>
                key.PrincipalTable == "Categories");
            var counterpartyKey = obligations.ForeignKeys.Single(key =>
                key.PrincipalTable == "Counterparties");
            var accountKey = settlements.ForeignKeys.Single(key =>
                key.PrincipalTable == "Accounts");
            var obligationKey = settlements.ForeignKeys.Single(key =>
                key.PrincipalTable == "Obligations");

            Assert.Equal(["UserId", "CategoryId"], categoryKey.Columns);
            Assert.Equal(["UserId", "CounterpartyId"], counterpartyKey.Columns);
            Assert.Equal(["UserId", "AccountId"], accountKey.Columns);
            Assert.Equal(["UserId", "ObligationId"], obligationKey.Columns);
            Assert.All(
                new[] { categoryKey, counterpartyKey, accountKey, obligationKey },
                key => Assert.Equal(ReferentialAction.Restrict, key.OnDelete));

            Assert.Contains(
                up.OfType<CreateIndexOperation>(),
                index => index.Name == "UX_ObligationSettlements_UserId_ObligationId" &&
                         index.IsUnique);
        }
    }

    [Fact]
    public void AddRecurringOccurrenceLimit_BackfillsBeforeChecksAndLeavesNoDefault()
    {
        var migration = LoadMigrations(out var context).Single(entry =>
            entry.Id.EndsWith("_AddRecurringOccurrenceLimit", StringComparison.Ordinal));
        using (context)
        {
            var up = migration.Migration.UpOperations.ToList();
            var generatedCount = Assert.Single(up.OfType<AddColumnOperation>(), operation =>
                operation.Table == "RecurringTransactions" &&
                operation.Name == "GeneratedOccurrenceCount");
            var occurrenceLimit = Assert.Single(up.OfType<AddColumnOperation>(), operation =>
                operation.Table == "RecurringTransactions" &&
                operation.Name == "OccurrenceLimit");
            Assert.True(generatedCount.IsNullable);
            Assert.True(occurrenceLimit.IsNullable);
            Assert.Null(generatedCount.DefaultValue);
            Assert.Null(generatedCount.DefaultValueSql);

            var backfillIndex = up.FindIndex(operation => operation is SqlOperation);
            var requiredIndex = up.FindIndex(operation =>
                operation is AlterColumnOperation alter &&
                alter.Name == "GeneratedOccurrenceCount" &&
                !alter.IsNullable);
            var firstCheckIndex = up.FindIndex(operation =>
                operation is AddCheckConstraintOperation);
            Assert.True(backfillIndex >= 0 && backfillIndex < requiredIndex);
            Assert.True(requiredIndex < firstCheckIndex);

            var backfill = Assert.IsType<SqlOperation>(up[backfillIndex]);
            Assert.Contains("COUNT(*)", backfill.Sql, StringComparison.Ordinal);
            Assert.Contains("RecurringTransactionOccurrences", backfill.Sql, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// İki tablo da bu adımda <b>boş doğar</b>: yorumlanacak bir geçmiş yoktur,
    /// bu yüzden zorunlu kolonlar backfill istemez ve kalıcı bir DEFAULT
    /// bırakılmaz. Dolu bir tabloda bu yol kullanılmaz.
    /// </summary>
    [Fact]
    public void AddCashCountsAndPosSettlements_CreatesEmptyTablesWithOwnerScopedGuards()
    {
        var migration = LoadMigrations(out var context).Single(entry =>
            entry.Id.EndsWith("_AddCashCountsAndPosSettlements", StringComparison.Ordinal));
        using (context)
        {
            var up = migration.Migration.UpOperations.ToList();

            // Yeni tablo yaratılıyor; mevcut bir tabloya zorunlu kolon
            // eklenmiyor, yani backfill sorusu hiç doğmuyor.
            Assert.Empty(up.OfType<AddColumnOperation>());
            var cashCounts = Assert.Single(
                up.OfType<CreateTableOperation>(), table => table.Name == "CashCounts");
            var posSettlements = Assert.Single(
                up.OfType<CreateTableOperation>(), table => table.Name == "PosSettlements");
            Assert.All(
                cashCounts.Columns.Concat(posSettlements.Columns),
                column =>
                {
                    Assert.Null(column.DefaultValue);
                    Assert.Null(column.DefaultValueSql);
                });

            // Sahiplik kapısı SQL seviyesinde de duruyor: her iki tablo da
            // bileşik `(UserId, Id)` anahtarıyla bağlanıyor.
            Assert.All(
                new[] { cashCounts, posSettlements },
                table => Assert.Contains(
                    table.ForeignKeys,
                    key => key.Columns.SequenceEqual(new[] { "UserId", "AccountId" }) &&
                           key.PrincipalColumns!.SequenceEqual(new[] { "UserId", "Id" })));

            // Bir gün ve bir kasa için tek **açık** sayım: filtreli tekil indeks.
            var openCount = Assert.Single(
                up.OfType<CreateIndexOperation>(),
                index => index.Name == "UX_CashCounts_UserId_AccountId_CountDate_Open");
            Assert.True(openCount.IsUnique);
            Assert.Equal("[IsCancelled] = 0", openCount.Filter);

            // Yoldaki paranın türetilebilmesi için gereken alanlar kolon;
            // türetilenler (net tutar, oran) kolon değil.
            var posColumns = posSettlements.Columns.Select(column => column.Name).ToArray();
            Assert.Contains("GrossAmount", posColumns);
            Assert.Contains("CommissionAmount", posColumns);
            Assert.Contains("TransferredOn", posColumns);
            Assert.DoesNotContain("NetAmount", posColumns);
            Assert.DoesNotContain("CommissionRate", posColumns);
            Assert.DoesNotContain("IsInTransit", posColumns);

            // Sayımda beklenen tutar ve fark kolon değildir.
            var countColumns = cashCounts.Columns.Select(column => column.Name).ToArray();
            Assert.Contains("CountedAmount", countColumns);
            Assert.DoesNotContain("ExpectedBalance", countColumns);
            Assert.DoesNotContain("Difference", countColumns);
        }
    }

    /// <summary>
    /// Doğrulama kodları tablosu <b>boş doğar</b>: mevcut hiçbir tabloya kolon
    /// eklenmez, yorumlanacak bir geçmiş yoktur ve kalıcı bir DEFAULT
    /// bırakılmaz. Kod açık saklanmadığı için kolon da kodun kendisi değil
    /// hash'idir.
    /// </summary>
    [Fact]
    public void AddVerificationCodes_CreatesOneEmptyTableThatStoresOnlyTheHash()
    {
        var migration = LoadMigrations(out var context).Single(entry =>
            entry.Id.EndsWith("_AddVerificationCodes", StringComparison.Ordinal));
        using (context)
        {
            var up = migration.Migration.UpOperations.ToList();

            Assert.Empty(up.OfType<AddColumnOperation>());
            Assert.Empty(up.OfType<AlterColumnOperation>());
            Assert.Empty(up.OfType<SqlOperation>());

            var table = Assert.Single(up.OfType<CreateTableOperation>());
            Assert.Equal("VerificationCodes", table.Name);
            Assert.All(table.Columns, column =>
            {
                Assert.Null(column.DefaultValue);
                Assert.Null(column.DefaultValueSql);
            });

            // Kodun kendisi hiçbir kolonda durmaz.
            var columns = table.Columns.Select(column => column.Name).ToArray();
            Assert.Contains("CodeHash", columns);
            Assert.DoesNotContain("Code", columns);

            // Süre, tek kullanım ve deneme sayısı SQL seviyesinde de bağlı.
            var checks = table.CheckConstraints.Select(check => check.Name).ToArray();
            Assert.Contains("CK_VerificationCodes_Expiry", checks);
            Assert.Contains("CK_VerificationCodes_Consumption", checks);
            Assert.Contains("CK_VerificationCodes_FailedAttempts", checks);
            Assert.Contains("CK_VerificationCodes_Purpose", checks);

            var ownerKey = Assert.Single(table.ForeignKeys);
            Assert.Equal("AspNetUsers", ownerKey.PrincipalTable);
            Assert.Equal(ReferentialAction.Restrict, ownerKey.OnDelete);
        }
    }

    /// <summary>
    /// Sayım anındaki beklenen bakiye tarihsel bir gözlemdir: eski sayımlarda
    /// bilinmez, bu yüzden kolon nullable ve varsayılansızdır; backfill yoktur
    /// (bugünkü bakiyeden doldurmak olmamış bir geçmiş uydurmak olurdu).
    /// </summary>
    [Fact]
    public void AddCashCountExpectedSnapshot_AddsOneNullableColumnWithoutBackfill()
    {
        var migration = LoadMigrations(out var context).Single(entry =>
            entry.Id.EndsWith("_AddCashCountExpectedSnapshot", StringComparison.Ordinal));
        using (context)
        {
            var up = migration.Migration.UpOperations.ToList();
            var column = Assert.IsType<AddColumnOperation>(Assert.Single(up));
            Assert.Equal("CashCounts", column.Table);
            Assert.Equal("ExpectedAtCount", column.Name);
            Assert.True(column.IsNullable);
            Assert.Null(column.DefaultValue);
            Assert.Null(column.DefaultValueSql);
            Assert.Equal("decimal(19,4)", column.ColumnType);
        }
    }

    /// <summary>
    /// KDV ve indirilebilirlik kalkıyor (ADR 0018): veri kaybettiren bir adım,
    /// kullanıcı kararıyla. Kısıtlar kolonlardan önce düşer; geri dönüşte
    /// kolonlar kısıtlardan önce, nullable ve varsayılansız geri gelir.
    /// Başka hiçbir şeye dokunulmaz.
    /// </summary>
    [Fact]
    public void RemoveVatAndTaxDeductibility_DropsChecksBeforeColumnsAndTouchesNothingElse()
    {
        var migration = LoadMigrations(out var context).Single(entry =>
            entry.Id.EndsWith("_RemoveVatAndTaxDeductibility", StringComparison.Ordinal));
        using (context)
        {
            var up = migration.Migration.UpOperations.ToList();
            Assert.All(up, operation => Assert.True(
                operation is DropCheckConstraintOperation or DropColumnOperation));

            var dropped = up.OfType<DropColumnOperation>()
                .Select(operation => $"{operation.Table}.{operation.Name}")
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
            Assert.Equal(
                [
                    "BudgetTransactions.IsTaxDeductible",
                    "BudgetTransactions.VatAmount",
                    "BudgetTransactions.VatRate",
                    "Categories.DefaultIsTaxDeductible",
                    "CounterpartyCharges.IsTaxDeductible",
                    "CounterpartyCharges.VatAmount",
                    "CounterpartyCharges.VatRate",
                    "CreditCardCharges.IsTaxDeductible",
                    "CreditCardCharges.VatAmount",
                    "CreditCardCharges.VatRate",
                    "Obligations.IsTaxDeductible",
                    "Obligations.VatAmount",
                    "Obligations.VatRate",
                    "PosSettlements.VatAmount",
                    "PosSettlements.VatRate",
                ],
                dropped);
            Assert.True(
                up.FindLastIndex(operation => operation is DropCheckConstraintOperation) <
                up.FindIndex(operation => operation is DropColumnOperation));

            var down = migration.Migration.DownOperations.ToList();
            Assert.True(
                down.FindLastIndex(operation => operation is AddColumnOperation) <
                down.FindIndex(operation => operation is AddCheckConstraintOperation));
            Assert.All(down.OfType<AddColumnOperation>(), column =>
            {
                Assert.True(column.IsNullable);
                Assert.Null(column.DefaultValue);
                Assert.Null(column.DefaultValueSql);
            });
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
