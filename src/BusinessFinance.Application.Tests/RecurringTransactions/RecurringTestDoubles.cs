using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.RecurringTransactions;

// Tekrarlayan plan ve vergi kullanım senaryolarının paylaştığı sahte depolar.

internal sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
{
    public Guid? UserId { get; } = userId;
}

internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}

internal sealed class FakeRecurringRepository : IRecurringTransactionRepository
{
    private readonly FakeTransactionRepository? transactions;
    private readonly FakeCardChargeRepository? charges;

    public FakeRecurringRepository(
        RecurringTransaction? recurring = null,
        FakeTransactionRepository? transactions = null,
        FakeCardChargeRepository? charges = null,
        params RecurringTransactionOccurrence[] occurrences)
    {
        if (recurring is not null) Recurring.Add(recurring);
        this.transactions = transactions;
        this.charges = charges;
        Occurrences.AddRange(occurrences);
    }

    public List<RecurringTransaction> Recurring { get; } = [];
    public List<RecurringTransactionOccurrence> Occurrences { get; } = [];
    public List<RecurringTransactionOccurrence> Removed { get; } = [];
    public int SaveCount { get; private set; }

    public Task<RecurringTransaction?> FindOwnedByIdAsync(
        Guid recurringTransactionId, Guid userId, bool track, CancellationToken cancellationToken) =>
        Task.FromResult(Recurring.SingleOrDefault(item =>
            item.Id == recurringTransactionId && item.UserId == userId));

    public Task<IReadOnlyList<RecurringTransaction>> ListAsync(
        Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<RecurringTransaction>>(
            Recurring.Where(item => item.UserId == userId).ToArray());

    public Task<IReadOnlyList<RecurringTransactionOccurrence>> ListPlanOccurrencesAsync(
        Guid recurringTransactionId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<RecurringTransactionOccurrence>>(Occurrences
            .Where(item => item.UserId == userId && item.RecurringTransactionId == recurringTransactionId)
            .OrderBy(item => item.ScheduledDate)
            .ToArray());

    public Task SaveEditAsync(
        RecurringTransaction recurring,
        IReadOnlyCollection<RecurringTransactionOccurrence> removedOccurrences,
        CancellationToken cancellationToken)
    {
        foreach (var occurrence in removedOccurrences) Occurrences.Remove(occurrence);
        Removed.AddRange(removedOccurrences);
        SaveCount++;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<RecurringTransactionOccurrence>> ListClosedByAsync(
        Guid userId, Guid? transactionId, Guid? chargeId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<RecurringTransactionOccurrence>>(Occurrences
            .Where(item => item.UserId == userId &&
                           ((transactionId != null && item.ClosedByTransactionId == transactionId) ||
                            (chargeId != null && item.ClosedByChargeId == chargeId)))
            .ToArray());

    public Task<bool> TrySaveOccurrenceChangeAsync(CancellationToken cancellationToken)
    {
        SaveCount++;
        return Task.FromResult(true);
    }

    public Task<IReadOnlyList<RecurringTransaction>> ListDueAsync(
        Guid userId, DateOnly throughDate, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<RecurringTransaction>>(Recurring.Where(item =>
            item.UserId == userId && item.IsActive &&
            item.NextOccurrenceDate is DateOnly next && next <= throughDate).ToArray());

    public Task AddAsync(RecurringTransaction recurring, CancellationToken cancellationToken)
    {
        Recurring.Add(recurring);
        return Task.CompletedTask;
    }

    public int AddRangeCalls { get; private set; }

    public Task AddRangeAsync(IReadOnlyCollection<RecurringTransaction> plans, CancellationToken cancellationToken)
    {
        AddRangeCalls++;
        Recurring.AddRange(plans);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RecurringTransaction recurring, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task<bool> TrySaveGeneratedAsync(
        IReadOnlyCollection<RecurringTransactionOccurrence> occurrences,
        CancellationToken cancellationToken)
    {
        if (occurrences.Any(candidate => Occurrences.Any(existing =>
                existing.UserId == candidate.UserId && existing.OccurrenceKey == candidate.OccurrenceKey)))
        {
            return Task.FromResult(false);
        }

        Occurrences.AddRange(occurrences);
        return Task.FromResult(true);
    }

    public RecurringDeletionResult DeletionResult { get; set; } =
        RecurringDeletionResult.Deleted;
    public readonly List<Guid> Deleted = [];

    public Task<RecurringDeletionResult> DeleteOwnedIfUnrealizedAsync(
        Guid recurringTransactionId, Guid userId, CancellationToken cancellationToken)
    {
        if (DeletionResult == RecurringDeletionResult.Deleted)
        {
            Deleted.Add(recurringTransactionId);
            Recurring.RemoveAll(item => item.Id == recurringTransactionId);
        }

        return Task.FromResult(DeletionResult);
    }

    public bool ScheduleIsActive { get; set; } = true;

    public Task<bool> IsScheduleActiveAsync(
        Guid recurringTransactionId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(ScheduleIsActive);

    public Task<RecurringTransactionOccurrence?> FindOccurrenceOwnedByIdAsync(
        Guid occurrenceId, Guid userId, bool track, CancellationToken cancellationToken) =>
        Task.FromResult(Occurrences.SingleOrDefault(item =>
            item.Id == occurrenceId && item.UserId == userId));

    public Task<RecurringTransactionOccurrence?> FindOccurrenceOwnedByDateAsync(
        Guid recurringTransactionId,
        DateOnly scheduledDate,
        Guid userId,
        CancellationToken cancellationToken,
        bool track = false) =>
        Task.FromResult(Occurrences.SingleOrDefault(item =>
            item.UserId == userId &&
            item.RecurringTransactionId == recurringTransactionId &&
            item.ScheduledDate == scheduledDate));

    public Task<IReadOnlyList<RecurringTransactionOccurrence>> ListOccurrencesAsync(
        Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<RecurringTransactionOccurrence>>(
            Occurrences.Where(item => item.UserId == userId).ToArray());

    public Task<BudgetTransaction> RealizeAsync(
        RecurringTransactionOccurrence occurrence,
        BudgetTransaction transaction,
        CancellationToken cancellationToken)
    {
        transactions!.Items.Add(transaction);
        return Task.FromResult(transaction);
    }

    public Task<CreditCardCharge> RealizeWithChargeAsync(
        RecurringTransactionOccurrence occurrence,
        CreditCardCharge charge,
        CancellationToken cancellationToken)
    {
        charges!.Items.Add(charge);
        return Task.FromResult(charge);
    }
}

internal sealed class FakeCreditCardRepository(
    decimal currentDebt,
    params CreditCard[] cards) : ICreditCardRepository
{
    public FakeCreditCardRepository(params CreditCard[] cards) : this(0m, cards) { }

    public Task<CreditCard?> FindOwnedByIdAsync(
        Guid creditCardId, Guid userId, bool track, CancellationToken cancellationToken) =>
        Task.FromResult(cards.SingleOrDefault(item => item.Id == creditCardId && item.UserId == userId));
    public Task<decimal> CalculateCurrentDebtAsync(
        Guid creditCardId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(currentDebt);
    public Task AddAsync(CreditCard creditCard, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task<bool> ExistsByNameAsync(Guid userId, string normalizedName, Guid? exceptCreditCardId, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task<IReadOnlyList<CreditCard>> ListAsync(Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task UpdateOwnedAsync(CreditCard creditCard, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
}

internal sealed class FakeCardChargeRepository : ICardChargeRepository
{
    public List<CreditCardCharge> Items { get; } = [];
    public Task AddAsync(CreditCardCharge charge, CancellationToken cancellationToken)
    {
        Items.Add(charge);
        return Task.CompletedTask;
    }
    public Task<CreditCardCharge?> FindOwnedByIdAsync(Guid chargeId, Guid userId, bool track, CancellationToken cancellationToken) =>
        Task.FromResult(Items.SingleOrDefault(item => item.Id == chargeId && item.UserId == userId));
    public Task<BoundedList<CreditCardCharge>> ListAsync(Guid creditCardId, Guid userId, HistoryWindow window, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task UpdateOwnedAsync(CreditCardCharge charge, Guid userId, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal sealed class FakeAccountRepository(params Account[] accounts) : IAccountRepository
{
    public Task<Account?> FindOwnedByIdAsync(
        Guid accountId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(accounts.SingleOrDefault(item => item.Id == accountId && item.UserId == userId));
    public Task AddAsync(Account account, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task<bool> ExistsByNameAsync(Guid userId, string normalizedName, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task<AccountListPage> ListAsync(Guid userId, AccountListCriteria criteria, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task UpdateOwnedAsync(Account account, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task<decimal> CalculateBalanceAsync(Guid accountId, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
}

internal sealed class FakeCategoryRepository(params Category[] categories) : ICategoryRepository
{
    public Task<Category?> FindOwnedByIdAsync(
        Guid categoryId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(categories.SingleOrDefault(item => item.Id == categoryId && item.UserId == userId));
    public Task EnsureDefaultsAsync(Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task AddAsync(Category category, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task<bool> ExistsByNameAndTypeAsync(Guid userId, string name, CategoryType type, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task<IReadOnlyList<Category>> ListAsync(Guid userId, CategoryType? type, bool? isActive, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task UpdateOwnedAsync(Category category, Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
}

internal sealed class FakeTransactionRepository : ITransactionRepository
{
    public List<BudgetTransaction> Items { get; } = [];
    public Task AddAsync(BudgetTransaction transaction, CancellationToken cancellationToken)
    {
        Items.Add(transaction);
        return Task.CompletedTask;
    }
    public Task<BudgetTransaction?> FindOwnedByIdAsync(Guid transactionId, Guid userId, bool track, CancellationToken cancellationToken) =>
        Task.FromResult(Items.SingleOrDefault(item => item.Id == transactionId && item.UserId == userId));
    public Task UpdateOwnedAsync(BudgetTransaction transaction, Guid userId, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task<TransactionListPage> ListAsync(Guid userId, TransactionListCriteria criteria, CancellationToken cancellationToken) => throw new NotSupportedException();
}

internal sealed class FakeUserProfileRepository(bool? hasBusiness) : IUserProfileRepository
{
    public Task<UserProfile?> FindAsync(Guid userId, bool track, CancellationToken cancellationToken) =>
        Task.FromResult(hasBusiness is bool value ? new UserProfile(userId, value) : null);
    public Task AddAsync(UserProfile profile, CancellationToken cancellationToken) => throw new NotSupportedException();
    public Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken) => throw new NotSupportedException();
}

/// <summary>
/// Ödemeyi sahte gider ve harcama depolarına yazar; okuması aynı depolardan
/// ve sahte plan deposundaki kalemlerden kurulur.
/// </summary>
internal sealed class FakeTaxPaymentRepository(
    FakeTransactionRepository transactions,
    FakeCardChargeRepository charges,
    FakeRecurringRepository recurring) : ITaxPaymentRepository
{
    public int AddCount { get; private set; }

    public Task<TaxPaymentSaveResult> AddAsync(
        BudgetTransaction? transaction,
        CreditCardCharge? charge,
        CancellationToken cancellationToken)
    {
        AddCount++;
        if (transaction is not null) transactions.Items.Add(transaction);
        if (charge is not null) charges.Items.Add(charge);
        return Task.FromResult(TaxPaymentSaveResult.Saved);
    }

    public Task<bool> TrySaveAsync(CancellationToken cancellationToken) => Task.FromResult(true);

    public Task<TaxPaymentDto?> FindAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken)
    {
        var transaction = transactions.Items.SingleOrDefault(item => item.Id == paymentId && item.UserId == userId);
        var charge = charges.Items.SingleOrDefault(item => item.Id == paymentId && item.UserId == userId);
        if (transaction is null && charge is null) return Task.FromResult<TaxPaymentDto?>(null);

        TaxSettledItemDto Item(RecurringTransactionOccurrence occurrence) => new(
            occurrence.Id,
            occurrence.RecurringTransactionId,
            occurrence.ScheduledDate,
            occurrence.Description,
            recurring.Recurring.Single(plan => plan.Id == occurrence.RecurringTransactionId).TaxKind);

        var realized = recurring.Occurrences.SingleOrDefault(item =>
            item.BudgetTransactionId == paymentId || item.CreditCardChargeId == paymentId);
        var closed = recurring.Occurrences
            .Where(item => item.ClosedByTransactionId == paymentId || item.ClosedByChargeId == paymentId)
            .Select(Item)
            .ToArray();
        return Task.FromResult<TaxPaymentDto?>(transaction is not null
            ? new TaxPaymentDto(
                paymentId, RecurringSourceType.Account, transaction.AccountId, "Account",
                transaction.CategoryId, "Category", transaction.Amount.Amount, transaction.Amount.Currency,
                transaction.TransactionDate, transaction.Description, transaction.Scope,
                realized is null ? null : Item(realized), closed, transaction.IsCancelled)
            : new TaxPaymentDto(
                paymentId, RecurringSourceType.CreditCard, charge!.CreditCardId, "Card",
                charge.CategoryId, "Category", charge.Amount.Amount, charge.Amount.Currency,
                charge.ChargeDate, charge.Description, charge.Scope,
                realized is null ? null : Item(realized), closed, charge.IsCancelled));
    }

    public Task<TaxPaymentPage> ListAsync(Guid userId, int skip, int take, CancellationToken cancellationToken) =>
        Task.FromResult(new TaxPaymentPage([], false));

    public Task<IReadOnlyList<TaxPlanHistoryItemDto>> ListPlanHistoryAsync(
        Guid userId, Guid recurringTransactionId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<TaxPlanHistoryItemDto>>([]);
}
