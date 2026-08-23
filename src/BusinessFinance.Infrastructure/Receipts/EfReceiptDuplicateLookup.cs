using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Receipts;

/// <summary>
/// Finds a record that already looks like the document just read.
///
/// <para>
/// The match is deliberately strict — same day, same amount, same name — because
/// the cost of the two errors is not symmetric. A missed duplicate leaves the
/// user with the double entry they already had; a false alarm teaches them to
/// ignore the warning, which costs every future one.
/// </para>
/// <para>
/// Four shelves are searched, not one. A bank slip becomes an expense, a
/// transfer, a card payment or a receivable depending on an answer the user
/// gives <b>after</b> this call; searching only the expense list meant the
/// three other paths had no protection at all, even though the stage promised
/// it for "the same document".
/// </para>
/// </summary>
internal sealed class EfReceiptDuplicateLookup(BusinessFinanceDbContext dbContext)
    : IReceiptDuplicateLookup
{
    public async Task<ReceiptDuplicateMatch?> FindAsync(
        Guid userId,
        DateOnly? transactionDate,
        decimal? amount,
        string? counterpartyName,
        ReceiptCaptureIntent intent,
        CancellationToken cancellationToken)
    {
        // Two of the three legs must be readable. With only an amount there is
        // no duplicate to speak of: the same number appears in a budget all the
        // time and warning on it would be noise.
        if (transactionDate is not DateOnly date ||
            amount is not decimal total ||
            string.IsNullOrWhiteSpace(counterpartyName))
        {
            return null;
        }

        var name = counterpartyName.Trim();

        // The intent decides which shelves are searched. Only a bank slip is
        // still undecided at this point, so only it looks at all four.
        var slip = intent is ReceiptCaptureIntent.BankSlip;

        if (intent is ReceiptCaptureIntent.Income or ReceiptCaptureIntent.Expense || slip)
        {
            var type = intent is ReceiptCaptureIntent.Income
                ? TransactionType.Income
                : TransactionType.Expense;

            // Cancelled records are not duplicates: the user already undid that
            // one and writing the correct version is exactly what they are
            // doing now.
            var transaction = await dbContext.Transactions
                .AsNoTracking()
                .Where(item =>
                    item.UserId == userId &&
                    !item.IsCancelled &&
                    item.Type == type &&
                    item.TransactionDate == date &&
                    item.Amount.Amount == total &&
                    item.Description != null &&
                    item.Description == name)
                .Select(item => new ReceiptDuplicateMatch(
                    item.Id,
                    item.TransactionDate,
                    item.Amount.Amount,
                    item.Description,
                    ReceiptDuplicateKind.Transaction))
                .FirstOrDefaultAsync(cancellationToken);
            if (transaction is not null)
                return transaction;
        }

        if (intent is ReceiptCaptureIntent.Transfer || slip)
        {
            var transfer = await dbContext.Transfers
                .AsNoTracking()
                .Where(item =>
                    item.UserId == userId &&
                    !item.IsCancelled &&
                    item.TransferDate == date &&
                    item.Amount.Amount == total &&
                    item.Description != null &&
                    item.Description == name)
                .Select(item => new ReceiptDuplicateMatch(
                    item.Id,
                    item.TransferDate,
                    item.Amount.Amount,
                    item.Description,
                    ReceiptDuplicateKind.Transfer))
                .FirstOrDefaultAsync(cancellationToken);
            if (transfer is not null)
                return transfer;
        }

        if (slip)
        {
            var payment = await dbContext.CreditCardPayments
                .AsNoTracking()
                .Where(item =>
                    item.UserId == userId &&
                    !item.IsCancelled &&
                    item.PaymentDate == date &&
                    item.Amount.Amount == total &&
                    item.Description != null &&
                    item.Description == name)
                .Select(item => new ReceiptDuplicateMatch(
                    item.Id,
                    item.PaymentDate,
                    item.Amount.Amount,
                    item.Description,
                    ReceiptDuplicateKind.CardPayment))
                .FirstOrDefaultAsync(cancellationToken);
            if (payment is not null)
                return payment;

            // A receivable is matched on the counterparty's own name rather
            // than on a description: lending is the one path where the name is
            // a first-class field, and it is what the user typed.
            var receivable = await (
                    from item in dbContext.DebtAgreements.AsNoTracking()
                    join counterparty in dbContext.Counterparties.AsNoTracking()
                        on new { item.UserId, Id = item.CounterpartyId }
                        equals new { counterparty.UserId, counterparty.Id }
                    where item.UserId == userId &&
                          item.Direction == DebtDirection.Receivable &&
                          item.StartDate == date &&
                          item.Principal.Amount == total &&
                          counterparty.Name == name
                    select new ReceiptDuplicateMatch(
                        item.Id,
                        item.StartDate,
                        item.Principal.Amount,
                        counterparty.Name,
                        ReceiptDuplicateKind.Receivable))
                .FirstOrDefaultAsync(cancellationToken);
            if (receivable is not null)
                return receivable;
        }

        return null;
    }
}
