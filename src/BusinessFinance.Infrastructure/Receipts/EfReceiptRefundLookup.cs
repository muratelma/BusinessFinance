using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Receipts;

/// <summary>
/// Finds the expense an iade fişi appears to reverse.
///
/// <para>
/// Looser than <see cref="EfReceiptDuplicateLookup" /> on purpose, and the
/// asymmetry is the reason. A duplicate warning fires on its own and a false one
/// teaches the user to ignore every future warning, so that match is strict. A
/// refund candidate is <b>shown and confirmed</b> before anything happens — the
/// user reads the date, amount and name and says yes. The cost of offering a
/// wrong candidate is a declined question; the cost of finding nothing is that a
/// real refund goes unrecorded.
/// </para>
/// </summary>
internal sealed class EfReceiptRefundLookup(BusinessFinanceDbContext dbContext)
    : IReceiptRefundLookup
{
    /// <summary>
    /// How far back a refund may reach. Longer than a shop's return window on
    /// purpose — the slip carries the refund date, not the purchase date, and a
    /// warranty return months later is still a return. Unbounded would start
    /// matching unrelated spending at the same shop.
    /// </summary>
    private const int LookbackDays = 180;

    public async Task<ReceiptRefundMatch?> FindAsync(
        Guid userId,
        DateOnly? refundDate,
        decimal? refundAmount,
        string? counterpartyName,
        CancellationToken cancellationToken)
    {
        // All three legs are required. Without the name a refund matches every
        // expense of that size; without the amount there is nothing to give
        // back; without the date there is no window.
        if (refundDate is not DateOnly date ||
            refundAmount is not decimal refund ||
            refund <= 0 ||
            string.IsNullOrWhiteSpace(counterpartyName))
        {
            return null;
        }

        var name = counterpartyName.Trim();
        var earliest = date.AddDays(-LookbackDays);

        // An expense smaller than the refund cannot be the one being reversed;
        // matching it would hand the user a candidate that makes no arithmetic
        // sense. Already-cancelled records are out: that refund was handled.
        var candidates = await dbContext.Transactions
            .AsNoTracking()
            .Where(transaction =>
                transaction.UserId == userId &&
                !transaction.IsCancelled &&
                transaction.Type == TransactionType.Expense &&
                transaction.TransactionDate <= date &&
                transaction.TransactionDate >= earliest &&
                transaction.Amount.Amount >= refund &&
                transaction.Description != null &&
                transaction.Description == name)
            // Exact amount first: a full refund is the common case and the
            // unambiguous one. Then the most recent, because a repeat purchase
            // at the same shop is more likely the one just returned.
            .OrderBy(transaction => transaction.Amount.Amount == refund ? 0 : 1)
            .ThenByDescending(transaction => transaction.TransactionDate)
            .Select(transaction => new
            {
                transaction.Id,
                transaction.TransactionDate,
                Amount = transaction.Amount.Amount,
                transaction.Description
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (candidates is null)
            return null;

        var remaining = candidates.Amount - refund;
        return new ReceiptRefundMatch(
            candidates.Id,
            candidates.TransactionDate,
            candidates.Amount,
            candidates.Description,
            remaining > 0 ? remaining : null);
    }
}
