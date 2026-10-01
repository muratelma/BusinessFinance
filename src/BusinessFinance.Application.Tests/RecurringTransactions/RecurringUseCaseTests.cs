using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.RecurringTransactions;

public sealed class RecurringUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task Generate_TwiceForSameThroughDate_CreatesEachOccurrenceOnce()
    {
        var (recurring, _, _) = CreateRecurring(UserId, new DateOnly(2026, 1, 31));
        var repository = new FakeRecurringRepository(recurring);
        var useCase = new GenerateRecurringOccurrencesUseCase(
            new FakeCurrentUser(UserId), repository);
        var command = new GenerateRecurringOccurrencesCommand(new DateOnly(2026, 3, 31));

        var first = await useCase.ExecuteAsync(command);
        var second = await useCase.ExecuteAsync(command);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(3, first.Value.GeneratedOccurrences.Count);
        Assert.Empty(second.Value.GeneratedOccurrences);
        Assert.Equal(3, repository.Occurrences.Count);
        Assert.Equal(3, repository.Occurrences.Select(item => item.OccurrenceKey).Distinct().Count());
        Assert.Equal(new DateOnly(2026, 4, 30), recurring.NextOccurrenceDate);
    }

    [Fact]
    public async Task Generate_ForInactiveDefinition_CreatesNothing()
    {
        var (recurring, _, _) = CreateRecurring(UserId, new DateOnly(2026, 8, 1));
        recurring.Deactivate();
        var repository = new FakeRecurringRepository(recurring);

        var result = await new GenerateRecurringOccurrencesUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new GenerateRecurringOccurrencesCommand(new DateOnly(2026, 8, 31)));

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.GeneratedOccurrences);
        Assert.Empty(repository.Occurrences);
    }

    [Fact]
    public async Task Generate_WithTwelveOccurrenceLimit_StopsAtTwelveAndRetryCreatesNothing()
    {
        var (recurring, _, _) = CreateRecurring(
            UserId,
            new DateOnly(2026, 1, 1),
            occurrenceLimit: 12);
        var repository = new FakeRecurringRepository(recurring);
        var useCase = new GenerateRecurringOccurrencesUseCase(
            new FakeCurrentUser(UserId), repository);
        var command = new GenerateRecurringOccurrencesCommand(new DateOnly(2027, 1, 1));

        var first = await useCase.ExecuteAsync(command);
        var retry = await useCase.ExecuteAsync(command);

        Assert.True(first.IsSuccess);
        Assert.True(retry.IsSuccess);
        Assert.Equal(12, first.Value.GeneratedOccurrences.Count);
        Assert.Empty(retry.Value.GeneratedOccurrences);
        Assert.Equal(12, repository.Occurrences.Count);
        Assert.Equal(12, recurring.GeneratedOccurrenceCount);
        Assert.False(recurring.IsActive);
        Assert.Null(recurring.NextOccurrenceDate);
    }

    [Fact]
    public async Task Realize_Twice_ReturnsOneFinancialTransaction()
    {
        var (recurring, account, category) = CreateRecurring(UserId, new DateOnly(2026, 8, 31));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        var transactions = new FakeTransactionRepository();
        var repository = new FakeRecurringRepository(recurring, transactions, null, occurrence);
        var now = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero);
        var useCase = new RealizeRecurringOccurrenceUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FakeAccountRepository(account),
            new FakeCreditCardRepository(),
            new FakeCategoryRepository(category),
            transactions,
            new FakeCardChargeRepository(),
            new FixedTimeProvider(now));
        var command = new RealizeRecurringOccurrenceCommand(occurrence.Id);

        var first = await useCase.ExecuteAsync(command);
        var second = await useCase.ExecuteAsync(command);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(RecurringSourceType.Account, first.Value.SourceType);
        Assert.Null(first.Value.Charge);
        Assert.Equal(first.Value.Transaction!.Id, second.Value.Transaction!.Id);
        var transaction = Assert.Single(transactions.Items);
        Assert.Equal(TransactionType.Expense, transaction.Type);
        Assert.Equal(occurrence.ScheduledDate, transaction.TransactionDate);
        Assert.Equal(now, occurrence.RealizedAtUtc);
    }

    /// <summary>
    /// A date nobody generated yet is realized in one call: the server does the
    /// bookkeeping the decision needs.
    /// </summary>
    /// <remarks>
    /// The planned view projects dates from the schedule long before anyone
    /// generates their occurrence rows, and those rows used to carry a
    /// permanently disabled action — the only way forward was a "generate"
    /// button on another screen. Generating is not a decision; realizing is.
    /// </remarks>
    [Fact]
    public async Task RealizeDue_UngeneratedDate_GeneratesThenRealizes()
    {
        var (recurring, account, category) = CreateRecurring(UserId, new DateOnly(2026, 8, 31));
        var transactions = new FakeTransactionRepository();
        var repository = new FakeRecurringRepository(recurring, transactions, null);
        var now = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero);
        var useCase = BuildRealizeDue(recurring, account, category, transactions, repository, now);

        Assert.Empty(repository.Occurrences);

        var result = await useCase.ExecuteAsync(
            new RealizeDueRecurringCommand(recurring.Id, new DateOnly(2026, 8, 31)));

        Assert.True(result.IsSuccess);
        var transaction = Assert.Single(transactions.Items);
        Assert.Equal(new DateOnly(2026, 8, 31), transaction.TransactionDate);
        var occurrence = Assert.Single(repository.Occurrences);
        Assert.Equal(RecurringOccurrenceStatus.Realized, occurrence.Status);
    }

    /// <summary>
    /// The same call twice produces one movement: generation is keyed by owner
    /// and period, and realization short-circuits on an occurrence it already
    /// realized.
    /// </summary>
    [Fact]
    public async Task RealizeDue_Twice_ProducesOneMovement()
    {
        var (recurring, account, category) = CreateRecurring(UserId, new DateOnly(2026, 8, 31));
        var transactions = new FakeTransactionRepository();
        var repository = new FakeRecurringRepository(recurring, transactions, null);
        var now = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero);
        var useCase = BuildRealizeDue(recurring, account, category, transactions, repository, now);
        var command = new RealizeDueRecurringCommand(recurring.Id, new DateOnly(2026, 8, 31));

        var first = await useCase.ExecuteAsync(command);
        var second = await useCase.ExecuteAsync(command);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(first.Value.Transaction!.Id, second.Value.Transaction!.Id);
        Assert.Single(transactions.Items);
    }

    /// <summary>
    /// A date that has not arrived is refused on the server too.
    /// </summary>
    /// <remarks>
    /// The screen already hides the action on future rows, but a financial rule
    /// is not enforced where the button is. Realizing next month's rent today
    /// would put money in a month it never left, and every report that reads by
    /// date would be wrong from then on.
    /// </remarks>
    [Fact]
    public async Task RealizeDue_FutureDate_IsRefused()
    {
        var (recurring, account, category) = CreateRecurring(UserId, new DateOnly(2026, 8, 31));
        var transactions = new FakeTransactionRepository();
        var repository = new FakeRecurringRepository(recurring, transactions, null);
        var now = new DateTimeOffset(2026, 8, 20, 10, 0, 0, TimeSpan.Zero);
        var useCase = BuildRealizeDue(recurring, account, category, transactions, repository, now);

        var result = await useCase.ExecuteAsync(
            new RealizeDueRecurringCommand(recurring.Id, new DateOnly(2026, 8, 31)));

        Assert.False(result.IsSuccess);
        Assert.Equal("recurring.not_due_yet", result.Error.Code);
        Assert.Empty(transactions.Items);
        Assert.Empty(repository.Occurrences);
    }

    /// <summary>
    /// A paused plan produces nothing, and is refused before anything is
    /// generated: generating first would leave rows behind for a plan the user
    /// switched off.
    /// </summary>
    [Fact]
    public async Task RealizeDue_InactiveSchedule_IsRefusedBeforeGenerating()
    {
        var (recurring, account, category) = CreateRecurring(UserId, new DateOnly(2026, 8, 31));
        recurring.Deactivate();
        var transactions = new FakeTransactionRepository();
        var repository = new FakeRecurringRepository(recurring, transactions, null);
        var now = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero);
        var useCase = BuildRealizeDue(recurring, account, category, transactions, repository, now);

        var result = await useCase.ExecuteAsync(
            new RealizeDueRecurringCommand(recurring.Id, new DateOnly(2026, 8, 31)));

        Assert.False(result.IsSuccess);
        Assert.Equal("recurring.schedule_inactive", result.Error.Code);
        Assert.Empty(repository.Occurrences);
    }

    private static RealizeDueRecurringUseCase BuildRealizeDue(
        RecurringTransaction recurring,
        Account account,
        Category category,
        FakeTransactionRepository transactions,
        FakeRecurringRepository repository,
        DateTimeOffset now)
    {
        var currentUser = new FakeCurrentUser(UserId);
        var time = new FixedTimeProvider(now);
        return new RealizeDueRecurringUseCase(
            currentUser,
            new RecurringOccurrenceMaterializer(repository),
            new RealizeRecurringOccurrenceUseCase(
                currentUser,
                repository,
                new FakeAccountRepository(account),
                new FakeCreditCardRepository(),
                new FakeCategoryRepository(category),
                transactions,
                new FakeCardChargeRepository(),
                time),
            time);
    }

    [Fact]
    public async Task Realize_CardOccurrenceTwice_CreatesOneCardChargeAndNoTransaction()
    {
        var (recurring, card, category) = CreateCardRecurring(UserId, new DateOnly(2026, 8, 31));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        var transactions = new FakeTransactionRepository();
        var charges = new FakeCardChargeRepository();
        var repository = new FakeRecurringRepository(recurring, transactions, charges, occurrence);
        var now = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero);
        var useCase = new RealizeRecurringOccurrenceUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FakeAccountRepository(),
            new FakeCreditCardRepository(card),
            new FakeCategoryRepository(category),
            transactions,
            charges,
            new FixedTimeProvider(now));
        var command = new RealizeRecurringOccurrenceCommand(occurrence.Id);

        var first = await useCase.ExecuteAsync(command);
        var second = await useCase.ExecuteAsync(command);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(RecurringSourceType.CreditCard, first.Value.SourceType);
        Assert.Null(first.Value.Transaction);
        Assert.Equal(first.Value.Charge!.Id, second.Value.Charge!.Id);
        var charge = Assert.Single(charges.Items);
        Assert.Equal(card.Id, charge.CreditCardId);
        Assert.Equal(occurrence.ScheduledDate, charge.ChargeDate);
        Assert.Equal(occurrence.CreditCardChargeId, charge.Id);
        // A card occurrence must never also produce a budget transaction: that would
        // count the same expense twice.
        Assert.Empty(transactions.Items);
    }

    [Fact]
    public async Task Realize_CardOccurrenceWithoutEnoughLimit_LeavesOccurrencePlanned()
    {
        var (recurring, card, category) = CreateCardRecurring(UserId, new DateOnly(2026, 8, 31));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        var charges = new FakeCardChargeRepository();
        var repository = new FakeRecurringRepository(
            recurring, new FakeTransactionRepository(), charges, occurrence);
        // Card limit is 10.000 and the occurrence is 1.000, so 9.500 of existing debt
        // leaves only 500 available.
        var useCase = new RealizeRecurringOccurrenceUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FakeAccountRepository(),
            new FakeCreditCardRepository(9_500m, card),
            new FakeCategoryRepository(category),
            new FakeTransactionRepository(),
            charges,
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero)));

        var result = await useCase.ExecuteAsync(
            new RealizeRecurringOccurrenceCommand(occurrence.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal("recurring.card_limit_insufficient", result.Error.Code);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
        Assert.Empty(charges.Items);
        Assert.False(occurrence.IsRealized);
        Assert.Null(occurrence.CreditCardChargeId);
        Assert.Null(occurrence.RealizedAtUtc);
    }

    [Fact]
    public async Task Realize_CardOccurrenceWithInactiveCard_LeavesOccurrencePlanned()
    {
        var (recurring, card, category) = CreateCardRecurring(UserId, new DateOnly(2026, 8, 31));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        card.Update(
            card.Name, card.Limit, card.StatementClosingDay, card.PaymentDueDay,
            card.MinimumPaymentRate, isActive: false);
        var charges = new FakeCardChargeRepository();
        var useCase = new RealizeRecurringOccurrenceUseCase(
            new FakeCurrentUser(UserId),
            new FakeRecurringRepository(
                recurring, new FakeTransactionRepository(), charges, occurrence),
            new FakeAccountRepository(),
            new FakeCreditCardRepository(card),
            new FakeCategoryRepository(category),
            new FakeTransactionRepository(),
            charges,
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero)));

        var result = await useCase.ExecuteAsync(
            new RealizeRecurringOccurrenceCommand(occurrence.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal("recurring.card_unavailable", result.Error.Code);
        Assert.Empty(charges.Items);
        Assert.False(occurrence.IsRealized);
    }


    /// <summary>
    /// Switching a plan off must stop it producing money. A pending occurrence
    /// generated earlier is only a forecast, so realizing it after the plan was
    /// disabled would create a movement the user explicitly turned off.
    /// </summary>
    [Fact]
    public async Task Realize_WhenTheScheduleWasDeactivated_IsRefused()
    {
        var (recurring, account, category) = CreateRecurring(UserId, new DateOnly(2026, 8, 31));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        var transactions = new FakeTransactionRepository();
        var repository = new FakeRecurringRepository(recurring, transactions, null, occurrence)
        {
            ScheduleIsActive = false,
        };
        var useCase = new RealizeRecurringOccurrenceUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FakeAccountRepository(account),
            new FakeCreditCardRepository(),
            new FakeCategoryRepository(category),
            transactions,
            new FakeCardChargeRepository(),
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero)));

        var result = await useCase.ExecuteAsync(
            new RealizeRecurringOccurrenceCommand(occurrence.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal("recurring.schedule_inactive", result.Error.Code);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
        Assert.Empty(transactions.Items);
        Assert.False(occurrence.IsRealized);
    }

    /// <summary>
    /// Deactivating must not rewrite history: an occurrence that already produced
    /// a movement keeps returning that same result.
    /// </summary>
    [Fact]
    public async Task Realize_AlreadyRealized_StillReturnsItsResultAfterDeactivation()
    {
        var (recurring, account, category) = CreateRecurring(UserId, new DateOnly(2026, 8, 31));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        var transactions = new FakeTransactionRepository();
        var repository = new FakeRecurringRepository(recurring, transactions, null, occurrence);
        var useCase = new RealizeRecurringOccurrenceUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FakeAccountRepository(account),
            new FakeCreditCardRepository(),
            new FakeCategoryRepository(category),
            transactions,
            new FakeCardChargeRepository(),
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero)));
        var command = new RealizeRecurringOccurrenceCommand(occurrence.Id);
        var first = await useCase.ExecuteAsync(command);

        repository.ScheduleIsActive = false;
        var afterDeactivation = await useCase.ExecuteAsync(command);

        Assert.True(afterDeactivation.IsSuccess);
        Assert.Equal(
            first.Value.Transaction!.Id,
            afterDeactivation.Value.Transaction!.Id);
        Assert.Single(transactions.Items);
    }


    /// <summary>
    /// A plan that never produced a movement is only an intention, so removing it
    /// keeps the list honest instead of forcing a growing pile of disabled rows.
    /// </summary>
    [Fact]
    public async Task Delete_WhenThePlanNeverProducedAMovement_RemovesIt()
    {
        var (recurring, _, _) = CreateRecurring(UserId, new DateOnly(2026, 8, 1));
        var repository = new FakeRecurringRepository(recurring);
        var useCase = new DeleteRecurringTransactionUseCase(
            new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(
            new DeleteRecurringTransactionCommand(recurring.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal([recurring.Id], repository.Deleted);
        Assert.Empty(repository.Recurring);
    }

    /// <summary>
    /// Once a plan produced real movements, deleting it would strand them: the
    /// feed reads their origin and cancel lock from this link.
    /// </summary>
    [Fact]
    public async Task Delete_WhenThePlanAlreadyProducedMovements_IsRefused()
    {
        var (recurring, _, _) = CreateRecurring(UserId, new DateOnly(2026, 8, 1));
        var repository = new FakeRecurringRepository(recurring)
        {
            DeletionResult = RecurringDeletionResult.HasRealizedHistory,
        };
        var useCase = new DeleteRecurringTransactionUseCase(
            new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(
            new DeleteRecurringTransactionCommand(recurring.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal("recurring.has_realized_history", result.Error.Code);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
        Assert.Single(repository.Recurring);
    }

    [Fact]
    public async Task Delete_WithoutAnIdentity_NeverReachesTheRepository()
    {
        var repository = new FakeRecurringRepository();
        var useCase = new DeleteRecurringTransactionUseCase(
            new FakeCurrentUser(null), repository);

        var result = await useCase.ExecuteAsync(
            new DeleteRecurringTransactionCommand(Guid.NewGuid()));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.Empty(repository.Deleted);
    }

    [Fact]
    public async Task Create_WithForeignAccount_ReturnsValidationWithoutPersisting()
    {
        var ownedCategory = new Category(
            Guid.NewGuid(), UserId, "Rent", CategoryType.Expense);
        var foreignAccount = new Account(
            Guid.NewGuid(), Guid.NewGuid(), "Foreign", AccountType.Bank, CurrencyCode.TRY);
        var repository = new FakeRecurringRepository();
        var useCase = new CreateRecurringTransactionUseCase(
            new FakeCurrentUser(UserId),
            new FakeAccountRepository(foreignAccount),
            new FakeCreditCardRepository(),
            new FakeCategoryRepository(ownedCategory),
            new FakeUserProfileRepository(true),
            repository);

        var result = await useCase.ExecuteAsync(AccountCommand(
            foreignAccount.Id, ownedCategory.Id, RecurringTransactionKind.Expense));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.Empty(repository.Recurring);
    }

    [Fact]
    public async Task Create_WithCardSource_StoresCardAndLeavesAccountEmpty()
    {
        var category = new Category(Guid.NewGuid(), UserId, "Subscriptions", CategoryType.Expense);
        var card = CreateCard(UserId);
        var repository = new FakeRecurringRepository();
        var useCase = new CreateRecurringTransactionUseCase(
            new FakeCurrentUser(UserId),
            new FakeAccountRepository(),
            new FakeCreditCardRepository(card),
            new FakeCategoryRepository(category),
            new FakeUserProfileRepository(true),
            repository);

        var result = await useCase.ExecuteAsync(CardCommand(
            card.Id, category.Id, RecurringTransactionKind.BillPayment));

        Assert.True(result.IsSuccess);
        Assert.Equal(RecurringSourceType.CreditCard, result.Value.SourceType);
        Assert.Equal(card.Id, result.Value.CreditCardId);
        Assert.Null(result.Value.AccountId);
        Assert.Single(repository.Recurring);
    }

    [Fact]
    public async Task Create_WithCardSourceAndIncomeKind_IsRejectedBeforeAnyLookup()
    {
        var category = new Category(Guid.NewGuid(), UserId, "Salary", CategoryType.Income);
        var card = CreateCard(UserId);
        var repository = new FakeRecurringRepository();
        var useCase = new CreateRecurringTransactionUseCase(
            new FakeCurrentUser(UserId),
            new FakeAccountRepository(),
            new FakeCreditCardRepository(card),
            new FakeCategoryRepository(category),
            new FakeUserProfileRepository(true),
            repository);

        var result = await useCase.ExecuteAsync(CardCommand(
            card.Id, category.Id, RecurringTransactionKind.Income));

        Assert.False(result.IsSuccess);
        Assert.Equal("recurring.income_card_source_not_supported", result.Error.Code);
        Assert.Empty(repository.Recurring);
    }

    [Fact]
    public async Task Create_WithForeignCard_ReturnsCardUnavailableWithoutPersisting()
    {
        var category = new Category(Guid.NewGuid(), UserId, "Subscriptions", CategoryType.Expense);
        var foreignCard = CreateCard(Guid.NewGuid());
        var repository = new FakeRecurringRepository();
        var useCase = new CreateRecurringTransactionUseCase(
            new FakeCurrentUser(UserId),
            new FakeAccountRepository(),
            new FakeCreditCardRepository(foreignCard),
            new FakeCategoryRepository(category),
            new FakeUserProfileRepository(true),
            repository);

        var result = await useCase.ExecuteAsync(CardCommand(
            foreignCard.Id, category.Id, RecurringTransactionKind.Expense));

        Assert.False(result.IsSuccess);
        Assert.Equal("recurring.card_unavailable", result.Error.Code);
        Assert.Empty(repository.Recurring);
    }

    [Theory]
    [InlineData(true, true)]   // both supplied
    [InlineData(false, false)] // neither supplied
    public async Task Create_WithoutExactlyOneSource_IsRejected(bool withAccount, bool withCard)
    {
        var category = new Category(Guid.NewGuid(), UserId, "Subscriptions", CategoryType.Expense);
        var repository = new FakeRecurringRepository();
        var useCase = new CreateRecurringTransactionUseCase(
            new FakeCurrentUser(UserId),
            new FakeAccountRepository(),
            new FakeCreditCardRepository(),
            new FakeCategoryRepository(category),
            new FakeUserProfileRepository(true),
            repository);

        var result = await useCase.ExecuteAsync(new CreateRecurringTransactionCommand(
            RecurringSourceType.Account,
            withAccount ? Guid.NewGuid() : null,
            withCard ? Guid.NewGuid() : null,
            category.Id,
            100m,
            CurrencyCode.TRY,
            RecurringTransactionKind.Expense,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 1),
            null,
            MonthEndBehavior.ClampToLastDay,
            null));

        Assert.False(result.IsSuccess);
        Assert.Equal("recurring.invalid_source", result.Error.Code);
        Assert.Empty(repository.Recurring);
    }

    private static CreateRecurringTransactionCommand AccountCommand(
        Guid accountId, Guid categoryId, RecurringTransactionKind kind) =>
        new(RecurringSourceType.Account, accountId, null, categoryId, 100m, CurrencyCode.TRY,
            kind, TransactionScope.Business, RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 1), null,
            MonthEndBehavior.ClampToLastDay, null);

    private static CreateRecurringTransactionCommand CardCommand(
        Guid cardId, Guid categoryId, RecurringTransactionKind kind) =>
        new(RecurringSourceType.CreditCard, null, cardId, categoryId, 100m, CurrencyCode.TRY,
            kind, TransactionScope.Business, RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 1), null,
            MonthEndBehavior.ClampToLastDay, null);

    private static CreditCard CreateCard(Guid userId) => new(
        Guid.NewGuid(), userId, "Card", new Money(10_000m, CurrencyCode.TRY), 15, 25);

    private static (RecurringTransaction Recurring, CreditCard Card, Category Category) CreateCardRecurring(
        Guid userId,
        DateOnly startDate)
    {
        var card = CreateCard(userId);
        var category = new Category(Guid.NewGuid(), userId, "Subscriptions", CategoryType.Expense);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            card,
            category,
            new Money(1000m, CurrencyCode.TRY),
            RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            startDate,
            description: "Streaming");
        return (recurring, card, category);
    }

    private static (RecurringTransaction Recurring, Account Account, Category Category) CreateRecurring(
        Guid userId,
        DateOnly startDate,
        int? occurrenceLimit = null)
    {
        var account = new Account(
            Guid.NewGuid(), userId, "Bills", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(
            Guid.NewGuid(), userId, "Rent", CategoryType.Expense);
        var recurring = new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            account,
            category,
            new Money(1000m, CurrencyCode.TRY),
            RecurringTransactionKind.BillPayment,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            startDate,
            description: "Rent",
            occurrenceLimit: occurrenceLimit);
        return (recurring, account, category);
    }
}
