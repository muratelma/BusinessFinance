using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Taxes;

/// <summary>
/// Vergi ödemesinin yazılması ve okunması (ADR 0018 T5, T6).
/// </summary>
/// <remarks>
/// Vergi ödemesinin kendi tablosu yoktur: hesaptan ödenmişse bir gider, kartla
/// ödenmişse bir kart harcamasıdır. "Ödenenler" bu iki tablonun vergi işaretli
/// kategorilerdeki satırlarıdır ve birleşik feed gibi tek SQL sorgusunda
/// (<c>UNION ALL</c>) birleşir; sayfalama veritabanına iner.
/// </remarks>
internal sealed class EfTaxPaymentRepository(BusinessFinanceDbContext dbContext) : ITaxPaymentRepository
{
    private const int AccountSource = (int)RecurringSourceType.Account;
    private const int CardSource = (int)RecurringSourceType.CreditCard;

    public async Task<TaxPaymentSaveResult> AddAsync(
        BudgetTransaction? transaction,
        CreditCardCharge? charge,
        CancellationToken cancellationToken)
    {
        if ((transaction is null) == (charge is null))
        {
            throw new ArgumentException("A tax payment is exactly one transaction or one card charge.");
        }

        try
        {
            // Kapatılan kalemler izleniyor; ödemeyle aynı SaveChanges'te yazılır.
            if (transaction is not null) await dbContext.Transactions.AddAsync(transaction, cancellationToken);
            if (charge is not null) await dbContext.CreditCardCharges.AddAsync(charge, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return TaxPaymentSaveResult.Saved;
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            return TaxPaymentSaveResult.Conflict;
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Aynı istek kimliği aynı ödeme kimliğini üretir; eşzamanlı ikinci
            // istek birincil anahtara çarpar ve ilkinin sonucunu okur.
            dbContext.ChangeTracker.Clear();
            return TaxPaymentSaveResult.Duplicate;
        }
    }

    public async Task<bool> TrySaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task<TaxPaymentDto?> FindAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken)
    {
        var row = await PaymentRows(userId)
            .Where(item => item.Id == paymentId)
            .FirstOrDefaultAsync(cancellationToken);
        if (row is null) return null;

        var settled = await LoadSettledItemsAsync(userId, [paymentId], cancellationToken);
        return Map(row, settled);
    }

    public async Task<TaxPaymentPage> ListAsync(Guid userId, int skip, int take, CancellationToken cancellationToken)
    {
        var rows = await PaymentRows(userId)
            .Where(item => item.IsTaxCategory && !item.IsCancelled)
            .OrderByDescending(item => item.PaidOn)
            .ThenByDescending(item => item.Id)
            .Skip(skip)
            .Take(take + 1)
            .ToArrayAsync(cancellationToken);
        var page = rows.Take(take).ToArray();
        var settled = await LoadSettledItemsAsync(userId, [.. page.Select(item => item.Id)], cancellationToken);
        return new TaxPaymentPage([.. page.Select(item => Map(item, settled))], rows.Length > take);
    }

    public async Task<IReadOnlyList<TaxPlanHistoryItemDto>> ListPlanHistoryAsync(
        Guid userId,
        Guid recurringTransactionId,
        CancellationToken cancellationToken)
    {
        var occurrences = await dbContext.RecurringTransactionOccurrences.AsNoTracking()
            .Where(occurrence => occurrence.UserId == userId &&
                                 occurrence.RecurringTransactionId == recurringTransactionId &&
                                 occurrence.Status != RecurringOccurrenceStatus.Planned)
            .OrderByDescending(occurrence => occurrence.ScheduledDate)
            .Select(occurrence => new
            {
                occurrence.Id,
                occurrence.ScheduledDate,
                occurrence.Status,
                occurrence.AmountValue,
                PaymentId = occurrence.BudgetTransactionId ?? occurrence.CreditCardChargeId ??
                            occurrence.ClosedByTransactionId ?? occurrence.ClosedByChargeId
            })
            .ToArrayAsync(cancellationToken);
        var paymentIds = occurrences
            .Where(item => item.PaymentId is not null)
            .Select(item => item.PaymentId!.Value)
            .Distinct()
            .ToArray();
        var rows = await PaymentRows(userId)
            .Where(item => paymentIds.Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, cancellationToken);
        var settled = await LoadSettledItemsAsync(userId, paymentIds, cancellationToken);

        return [.. occurrences
            .Where(item => item.PaymentId is Guid id && rows.ContainsKey(id))
            .Select(item => new TaxPlanHistoryItemDto(
                item.Id,
                item.ScheduledDate,
                item.Status,
                item.AmountValue,
                Map(rows[item.PaymentId!.Value], settled)))];
    }

    /// <summary>
    /// Gider ve kart harcamasının aynı biçime indirgenmiş hâli; <c>Concat</c>
    /// tek bir <c>UNION ALL</c> olur.
    /// </summary>
    private IQueryable<PaymentRow> PaymentRows(Guid userId)
    {
        var transactions =
            from transaction in dbContext.Transactions.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { transaction.UserId, Id = transaction.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { transaction.UserId, Id = transaction.CategoryId }
                equals new { category.UserId, category.Id }
            where transaction.UserId == userId && transaction.Type == TransactionType.Expense
            select new PaymentRow
            {
                Id = transaction.Id,
                SourceType = AccountSource,
                SourceId = account.Id,
                SourceName = account.Name,
                CategoryId = category.Id,
                CategoryName = category.Name,
                IsTaxCategory = category.IsTax,
                Amount = transaction.Amount.Amount,
                Currency = (int)transaction.Amount.Currency,
                PaidOn = transaction.TransactionDate,
                Description = transaction.Description,
                Scope = (int)transaction.Scope,
                IsCancelled = transaction.IsCancelled
            };
        var charges =
            from charge in dbContext.CreditCardCharges.AsNoTracking()
            join card in dbContext.CreditCards.AsNoTracking()
                on new { charge.UserId, Id = charge.CreditCardId }
                equals new { card.UserId, card.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { charge.UserId, Id = charge.CategoryId }
                equals new { category.UserId, category.Id }
            where charge.UserId == userId
            select new PaymentRow
            {
                Id = charge.Id,
                SourceType = CardSource,
                SourceId = card.Id,
                SourceName = card.Name,
                CategoryId = category.Id,
                CategoryName = category.Name,
                IsTaxCategory = category.IsTax,
                Amount = charge.Amount.Amount,
                Currency = (int)charge.Amount.Currency,
                PaidOn = charge.ChargeDate,
                Description = charge.Description,
                Scope = (int)charge.Scope,
                IsCancelled = charge.IsCancelled
            };
        return transactions.Concat(charges);
    }

    private sealed record SettledItem(Guid PaymentId, bool IsRealization, TaxSettledItemDto Item);

    /// <summary>
    /// Ödemelerin ödediği ve kapattığı kalemler, tek sorguda.
    /// </summary>
    private async Task<ILookup<Guid, SettledItem>> LoadSettledItemsAsync(
        Guid userId,
        IReadOnlyCollection<Guid> paymentIds,
        CancellationToken cancellationToken)
    {
        if (paymentIds.Count == 0) return Array.Empty<SettledItem>().ToLookup(item => item.PaymentId);

        var rows = await (
                from occurrence in dbContext.RecurringTransactionOccurrences.AsNoTracking()
                join plan in dbContext.RecurringTransactions.AsNoTracking()
                    on new { occurrence.UserId, Id = occurrence.RecurringTransactionId }
                    equals new { plan.UserId, plan.Id }
                where occurrence.UserId == userId &&
                      ((occurrence.BudgetTransactionId != null &&
                        paymentIds.Contains(occurrence.BudgetTransactionId.Value)) ||
                       (occurrence.CreditCardChargeId != null &&
                        paymentIds.Contains(occurrence.CreditCardChargeId.Value)) ||
                       (occurrence.ClosedByTransactionId != null &&
                        paymentIds.Contains(occurrence.ClosedByTransactionId.Value)) ||
                       (occurrence.ClosedByChargeId != null &&
                        paymentIds.Contains(occurrence.ClosedByChargeId.Value)))
                select new
                {
                    occurrence.Id,
                    PlanId = plan.Id,
                    occurrence.ScheduledDate,
                    occurrence.Status,
                    Name = plan.Description,
                    plan.TaxKind,
                    PaymentId = occurrence.BudgetTransactionId ?? occurrence.CreditCardChargeId ??
                                occurrence.ClosedByTransactionId ?? occurrence.ClosedByChargeId
                })
            .ToArrayAsync(cancellationToken);

        return rows
            .OrderBy(row => row.ScheduledDate)
            .Select(row => new SettledItem(
                row.PaymentId!.Value,
                row.Status == RecurringOccurrenceStatus.Realized,
                new TaxSettledItemDto(row.Id, row.PlanId, row.ScheduledDate, row.Name, row.TaxKind)))
            .ToLookup(item => item.PaymentId);
    }

    private static TaxPaymentDto Map(PaymentRow row, ILookup<Guid, SettledItem> settled)
    {
        var items = settled[row.Id].ToArray();
        return new TaxPaymentDto(
            row.Id,
            (RecurringSourceType)row.SourceType,
            row.SourceId,
            row.SourceName,
            row.CategoryId,
            row.CategoryName,
            row.Amount,
            (CurrencyCode)row.Currency,
            row.PaidOn,
            row.Description,
            (TransactionScope)row.Scope,
            items.Where(item => item.IsRealization).Select(item => item.Item).FirstOrDefault(),
            [.. items.Where(item => !item.IsRealization).Select(item => item.Item)],
            row.IsCancelled);
    }

    private sealed class PaymentRow
    {
        public Guid Id { get; init; }
        public int SourceType { get; init; }
        public Guid SourceId { get; init; }
        public string SourceName { get; init; } = string.Empty;
        public Guid CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public bool IsTaxCategory { get; init; }
        public decimal Amount { get; init; }
        public int Currency { get; init; }
        public DateOnly PaidOn { get; init; }
        public string? Description { get; init; }
        public int Scope { get; init; }
        public bool IsCancelled { get; init; }
    }
}
