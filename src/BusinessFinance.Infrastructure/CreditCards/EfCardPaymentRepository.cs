using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.CreditCards;

internal sealed class EfCardPaymentRepository(BusinessFinanceDbContext dbContext)
    : ICardPaymentRepository
{
    public async Task AddAsync(CreditCardPayment payment, CancellationToken cancellationToken)
    {
        await dbContext.CreditCardPayments.AddAsync(payment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<CreditCardPayment?> FindOwnedByIdAsync(Guid paymentId, Guid userId, bool track, CancellationToken cancellationToken)
    {
        IQueryable<CreditCardPayment> query = dbContext.CreditCardPayments;
        if (!track) query = query.AsNoTracking();
        return query.SingleOrDefaultAsync(
            payment => payment.Id == paymentId && payment.UserId == userId,
            cancellationToken);
    }

    /// <inheritdoc cref="EfCardChargeRepository.ListAsync" />
    public async Task<BoundedList<CreditCardPayment>> ListAsync(
        Guid creditCardId,
        Guid userId,
        HistoryWindow window,
        CancellationToken cancellationToken)
    {
        var query = dbContext.CreditCardPayments.AsNoTracking()
            .Where(payment => payment.CreditCardId == creditCardId && payment.UserId == userId);
        if (window.From is DateOnly from) query = query.Where(payment => payment.PaymentDate >= from);
        if (window.To is DateOnly to) query = query.Where(payment => payment.PaymentDate <= to);
        var rows = await query
            .OrderByDescending(payment => payment.PaymentDate).ThenByDescending(payment => payment.Id)
            .Take(HistoryWindow.MaximumRows + 1)
            .ToArrayAsync(cancellationToken);
        return rows.Length > HistoryWindow.MaximumRows
            ? new BoundedList<CreditCardPayment>(rows[..HistoryWindow.MaximumRows], true)
            : new BoundedList<CreditCardPayment>(rows, false);
    }

    public async Task UpdateOwnedAsync(CreditCardPayment payment, Guid userId, CancellationToken cancellationToken)
    {
        if (payment.UserId != userId) throw new InvalidOperationException("Owned payment was not found.");
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
