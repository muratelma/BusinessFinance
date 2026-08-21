using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

public sealed class FinancialEntityMappingTests
{
    [Fact]
    public void ImportMapping_UsesOwnerScopedBatchRelationshipAndExactSignedAmount()
    {
        using var context = CreateSqlServerModelContext();
        var batch = context.Model.FindEntityType(typeof(ImportBatch));
        var row = context.Model.FindEntityType(typeof(ImportRow));

        Assert.NotNull(batch);
        Assert.NotNull(row);
        var batchForeignKey = row.GetForeignKeys().Single(
            foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(ImportBatch));
        Assert.Equal(
            [nameof(ImportRow.UserId), nameof(ImportRow.ImportBatchId)],
            batchForeignKey.Properties.Select(property => property.Name));
        Assert.Equal(DeleteBehavior.Restrict, batchForeignKey.DeleteBehavior);
        Assert.Equal(5, row.GetForeignKeys().Count());
        Assert.All(row.GetForeignKeys(), foreignKey =>
            Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
        var version = row.FindProperty("Version");
        Assert.NotNull(version);
        Assert.True(version.IsConcurrencyToken);
        Assert.Equal(
            Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate,
            version.ValueGenerated);
        Assert.Contains(row.GetIndexes(), index => index.IsUnique &&
            index.Properties.Select(property => property.Name).SequenceEqual(
                [nameof(ImportRow.UserId), nameof(ImportRow.ImportBatchId), nameof(ImportRow.RowNumber)]));
        Assert.Contains(batch.GetIndexes(), index => index.IsUnique &&
            index.Properties.Select(property => property.Name).SequenceEqual(
                [nameof(ImportBatch.UserId), nameof(ImportBatch.FileFingerprint)]));

        var designTimeModel = context.GetService<IDesignTimeModel>().Model;
        var rowTable = designTimeModel.GetRelationalModel().Tables.Single(table => table.Name == "ImportRows");
        Assert.Equal("decimal(19,4)", rowTable.Columns.Single(column => column.Name == "SignedAmount").StoreType);
        Assert.Contains(
            rowTable.CheckConstraints,
            constraint => constraint.Name == "CK_ImportRows_SignedAmountRange" &&
                          constraint.Sql.Contains("999999999999999.9999", StringComparison.Ordinal));
    }

    [Fact]
    public void Model_UsesExpectedTablesColumnShapesAndNonPersistedPeriodValues()
    {
        using var context = CreateSqlServerModelContext();
        var account = context.Model.FindEntityType(typeof(Account));
        var category = context.Model.FindEntityType(typeof(Category));
        var transaction = context.Model.FindEntityType(typeof(BudgetTransaction));
        var monthlyBudget = context.Model.FindEntityType(typeof(MonthlyBudget));
        var transfer = context.Model.FindEntityType(typeof(Transfer));
        var creditCard = context.Model.FindEntityType(typeof(CreditCard));
        var cardCharge = context.Model.FindEntityType(typeof(CreditCardCharge));
        var cardPayment = context.Model.FindEntityType(typeof(CreditCardPayment));
        var installmentPlan = context.Model.FindEntityType(typeof(InstallmentPlan));
        var installmentItem = context.Model.FindEntityType(typeof(InstallmentItem));
        var recurringTransaction = context.Model.FindEntityType(typeof(RecurringTransaction));
        var recurringOccurrence = context.Model.FindEntityType(typeof(RecurringTransactionOccurrence));
        var relationalModel = context.Model.GetRelationalModel();

        Assert.Equal("Accounts", account?.GetTableName());
        Assert.Equal(Account.MaximumNameLength, account?.FindProperty(nameof(Account.Name))?.GetMaxLength());
        Assert.Equal("Categories", category?.GetTableName());
        Assert.Equal(Category.MaximumNameLength, category?.FindProperty(nameof(Category.Name))?.GetMaxLength());
        Assert.Equal("BudgetTransactions", transaction?.GetTableName());
        Assert.Equal("date", transaction?.FindProperty(nameof(BudgetTransaction.TransactionDate))?.GetColumnType());
        Assert.Equal(
            BudgetTransaction.MaximumDescriptionLength,
            transaction?.FindProperty(nameof(BudgetTransaction.Description))?.GetMaxLength());
        Assert.Equal("MonthlyBudgets", monthlyBudget?.GetTableName());
        Assert.Equal("Transfers", transfer?.GetTableName());
        Assert.Equal("date", transfer?.FindProperty(nameof(Transfer.TransferDate))?.GetColumnType());
        Assert.Equal("CreditCards", creditCard?.GetTableName());
        Assert.Equal(
            CreditCard.MaximumNameLength,
            creditCard?.FindProperty(nameof(CreditCard.Name))?.GetMaxLength());
        Assert.Equal("CreditCardCharges", cardCharge?.GetTableName());
        Assert.Equal("date", cardCharge?.FindProperty(nameof(CreditCardCharge.ChargeDate))?.GetColumnType());
        Assert.Equal("CreditCardPayments", cardPayment?.GetTableName());
        Assert.Equal("date", cardPayment?.FindProperty(nameof(CreditCardPayment.PaymentDate))?.GetColumnType());
        Assert.Equal("InstallmentPlans", installmentPlan?.GetTableName());
        Assert.Equal("InstallmentItems", installmentItem?.GetTableName());
        Assert.Equal("RecurringTransactions", recurringTransaction?.GetTableName());
        Assert.Equal("RecurringTransactionOccurrences", recurringOccurrence?.GetTableName());
        Assert.Equal(
            "date",
            recurringTransaction?.FindProperty(nameof(RecurringTransaction.NextOccurrenceDate))?.GetColumnType());
        Assert.Null(monthlyBudget?.FindProperty(nameof(MonthlyBudget.PeriodStart)));
        Assert.Null(monthlyBudget?.FindProperty(nameof(MonthlyBudget.PeriodEnd)));

        var transactionTable = relationalModel.Tables.Single(
            table => table.Name == "BudgetTransactions");
        var budgetTable = relationalModel.Tables.Single(table => table.Name == "MonthlyBudgets");

        Assert.Equal("decimal(19,4)", transactionTable.Columns.Single(
            column => column.Name == "Amount").StoreType);
        Assert.Equal("decimal(19,4)", budgetTable.Columns.Single(
            column => column.Name == "Limit").StoreType);
    }

    [Fact]
    public void RecurringOccurrenceMapping_UsesUniqueKeyRowVersionAndOwnerScopedRelationships()
    {
        using var context = CreateSqlServerModelContext();
        var occurrence = context.Model.FindEntityType(typeof(RecurringTransactionOccurrence));

        Assert.NotNull(occurrence);
        Assert.Contains(
            occurrence.GetIndexes(),
            index => index.IsUnique && index.Properties.Select(property => property.Name).SequenceEqual(
                [nameof(RecurringTransactionOccurrence.UserId), nameof(RecurringTransactionOccurrence.OccurrenceKey)]));
        var version = occurrence.FindProperty("Version");
        Assert.NotNull(version);
        Assert.True(version.IsConcurrencyToken);
        Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate, version.ValueGenerated);
        Assert.All(occurrence.GetForeignKeys(), foreignKey =>
            Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
        // user, schedule, category, account, credit card, budget transaction, card charge
        Assert.Equal(7, occurrence.GetForeignKeys().Count());
    }

    [Fact]
    public void RecurringMapping_UsesOwnerScopedRelationshipsAndDueDateIndex()
    {
        using var context = CreateSqlServerModelContext();
        var recurring = context.Model.FindEntityType(typeof(RecurringTransaction));

        Assert.NotNull(recurring);
        Assert.Contains(
            recurring.GetIndexes(),
            index => index.Properties.Select(property => property.Name).SequenceEqual(
                [
                    nameof(RecurringTransaction.UserId),
                    nameof(RecurringTransaction.IsActive),
                    nameof(RecurringTransaction.NextOccurrenceDate)
                ]));
        Assert.All(recurring.GetForeignKeys(), foreignKey =>
            Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));

        var accountForeignKey = recurring.GetForeignKeys().Single(
            foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(Account));
        var categoryForeignKey = recurring.GetForeignKeys().Single(
            foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(Category));
        Assert.Equal(
            [nameof(RecurringTransaction.UserId), nameof(RecurringTransaction.AccountId)],
            accountForeignKey.Properties.Select(property => property.Name));
        Assert.Equal(
            [nameof(RecurringTransaction.UserId), nameof(RecurringTransaction.CategoryId)],
            categoryForeignKey.Properties.Select(property => property.Name));
    }

    [Fact]
    public void CreditCardMapping_UsesOwnerScopedUniqueNameAndRestrictedUserRelationship()
    {
        using var context = CreateSqlServerModelContext();
        var card = context.Model.FindEntityType(typeof(CreditCard));

        Assert.NotNull(card);
        AssertUniqueIndex(card, nameof(CreditCard.UserId), nameof(CreditCard.Name));
        Assert.All(
            card.GetForeignKeys(),
            foreignKey => Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
        var cardTable = context.Model.GetRelationalModel().Tables.Single(
            table => table.Name == "CreditCards");
        Assert.Equal(
            "decimal(19,4)",
            cardTable.Columns.Single(column => column.Name == "Limit").StoreType);
    }

    [Fact]
    public void TransferMapping_UsesTwoOwnerScopedAccountForeignKeys()
    {
        using var context = CreateSqlServerModelContext();
        var transfer = context.Model.FindEntityType(typeof(Transfer));

        Assert.NotNull(transfer);
        var accountForeignKeys = transfer.GetForeignKeys()
            .Where(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(Account))
            .ToArray();

        Assert.Equal(2, accountForeignKeys.Length);
        Assert.Contains(accountForeignKeys, foreignKey => foreignKey.Properties.Select(property => property.Name)
            .SequenceEqual([nameof(Transfer.UserId), nameof(Transfer.SourceAccountId)]));
        Assert.Contains(accountForeignKeys, foreignKey => foreignKey.Properties.Select(property => property.Name)
            .SequenceEqual([nameof(Transfer.UserId), nameof(Transfer.DestinationAccountId)]));
        Assert.All(accountForeignKeys, foreignKey => Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
    }

    [Fact]
    public void TransactionMapping_UsesOwnerScopedAccountAndCategoryForeignKeys()
    {
        using var context = CreateSqlServerModelContext();
        var transaction = context.Model.FindEntityType(typeof(BudgetTransaction));
        Assert.NotNull(transaction);

        var accountForeignKey = transaction.GetForeignKeys().Single(
            foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(Account));
        var categoryForeignKey = transaction.GetForeignKeys().Single(
            foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(Category));

        Assert.Equal(
            [nameof(BudgetTransaction.UserId), nameof(BudgetTransaction.AccountId)],
            accountForeignKey.Properties.Select(property => property.Name));
        Assert.Equal(
            [nameof(Account.UserId), nameof(Account.Id)],
            accountForeignKey.PrincipalKey.Properties.Select(property => property.Name));
        Assert.Equal(
            [nameof(BudgetTransaction.UserId), nameof(BudgetTransaction.CategoryId)],
            categoryForeignKey.Properties.Select(property => property.Name));
        Assert.Equal(DeleteBehavior.Restrict, accountForeignKey.DeleteBehavior);
        Assert.Equal(DeleteBehavior.Restrict, categoryForeignKey.DeleteBehavior);
    }

    [Fact]
    public void Model_DefinesOwnerScopedUniqueAccountCategoryAndBudgetRules()
    {
        using var context = CreateSqlServerModelContext();
        var account = context.Model.FindEntityType(typeof(Account));
        var category = context.Model.FindEntityType(typeof(Category));
        var monthlyBudget = context.Model.FindEntityType(typeof(MonthlyBudget));

        AssertUniqueIndex(
            account,
            nameof(Account.UserId),
            nameof(Account.Name));
        AssertUniqueIndex(
            category,
            nameof(Category.UserId),
            nameof(Category.Type),
            nameof(Category.Name));
        AssertUniqueIndex(
            monthlyBudget,
            nameof(MonthlyBudget.UserId),
            nameof(MonthlyBudget.CategoryId),
            nameof(MonthlyBudget.Year),
            nameof(MonthlyBudget.Month));
    }

    [Fact]
    public void RefreshSessionMapping_DefinesUniqueHashAndRestrictedRelationships()
    {
        using var context = CreateSqlServerModelContext();
        var session = context.Model.FindEntityType(typeof(RefreshSession));

        Assert.NotNull(session);
        Assert.Equal("RefreshSessions", session.GetTableName());
        Assert.Equal(
            RefreshSession.MaximumTokenHashLength,
            session.FindProperty(nameof(RefreshSession.TokenHash))?.GetMaxLength());
        Assert.Contains(
            session.GetIndexes(),
            index => index.IsUnique &&
                     index.Properties.Single().Name == nameof(RefreshSession.TokenHash));
        Assert.All(
            session.GetForeignKeys(),
            foreignKey => Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
    }

    [Fact]
    public async Task InMemoryRoundTrip_MaterializesGetOnlyEntitiesAndMoneyValues()
    {
        var options = new DbContextOptionsBuilder<BusinessFinanceDbContext>()
            .UseInMemoryDatabase($"mapping-round-trip-{Guid.NewGuid():N}")
            .Options;
        var userId = Guid.NewGuid();
        var account = new Account(
            Guid.NewGuid(), userId, "Daily Cash", AccountType.Cash, CurrencyCode.TRY);
        var category = new Category(
            Guid.NewGuid(), userId, "Groceries", CategoryType.Expense);
        var transaction = new BudgetTransaction(
            Guid.NewGuid(),
            userId,
            account,
            category,
            new Money(125.50m, CurrencyCode.TRY),
            TransactionType.Expense,
            new DateOnly(2026, 8, 8),
            "Synthetic expense");
        var budget = new MonthlyBudget(
            Guid.NewGuid(),
            userId,
            category,
            new Money(5000m, CurrencyCode.TRY),
            2026,
            8);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            account,
            category,
            new Money(250m, CurrencyCode.TRY),
            RecurringTransactionKind.BillPayment,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 31),
            monthEndBehavior: MonthEndBehavior.ClampToLastDay,
            description: "Monthly bill");
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);

        await using (var writeContext = new BusinessFinanceDbContext(options))
        {
            writeContext.AddRange(account, category, transaction, budget, recurring, occurrence);
            writeContext.Entry(occurrence).Property<byte[]>("Version").CurrentValue = [1];
            await writeContext.SaveChangesAsync();
        }

        await using var readContext = new BusinessFinanceDbContext(options);
        var persistedTransaction = await readContext.Transactions.AsNoTracking().SingleAsync();
        var persistedBudget = await readContext.MonthlyBudgets.AsNoTracking().SingleAsync();
        var persistedRecurring = await readContext.RecurringTransactions.AsNoTracking().SingleAsync();
        var persistedOccurrence = await readContext.RecurringTransactionOccurrences.AsNoTracking().SingleAsync();

        Assert.Equal(new Money(125.50m, CurrencyCode.TRY), persistedTransaction.Amount);
        Assert.Equal("Synthetic expense", persistedTransaction.Description);
        Assert.Equal(new DateOnly(2026, 8, 8), persistedTransaction.TransactionDate);
        Assert.Equal(new Money(5000m, CurrencyCode.TRY), persistedBudget.Limit);
        Assert.Equal(new DateOnly(2026, 8, 1), persistedBudget.PeriodStart);
        Assert.Equal(new DateOnly(2026, 8, 31), persistedBudget.PeriodEnd);
        Assert.Equal(new Money(250m, CurrencyCode.TRY), persistedRecurring.Amount);
        Assert.Equal(new DateOnly(2026, 8, 31), persistedRecurring.NextOccurrenceDate);
        Assert.Equal(RecurringTransactionKind.BillPayment, persistedRecurring.Kind);
        Assert.Equal(RecurringOccurrenceStatus.Planned, persistedOccurrence.Status);
        Assert.Equal(new Money(250m, CurrencyCode.TRY), persistedOccurrence.Amount);
        Assert.Null(persistedOccurrence.BudgetTransactionId);
    }

    private static BusinessFinanceDbContext CreateSqlServerModelContext()
    {
        var options = new DbContextOptionsBuilder<BusinessFinanceDbContext>()
            .UseSqlServer("Server=(local);Database=ModelOnly;Integrated Security=true")
            .Options;

        return new BusinessFinanceDbContext(options);
    }

    private static void AssertUniqueIndex(
        Microsoft.EntityFrameworkCore.Metadata.IEntityType? entityType,
        params string[] propertyNames)
    {
        Assert.NotNull(entityType);
        Assert.Contains(
            entityType.GetIndexes(),
            index => index.IsUnique &&
                     index.Properties.Select(property => property.Name)
                         .SequenceEqual(propertyNames));
    }
}
