using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.CreditCards;

internal sealed class EfCreditCardStatementRepository(BusinessFinanceDbContext dbContext)
    : ICreditCardStatementRepository
{
    public async Task<StatementActivitySnapshot> GetActivityAsync(
        Guid creditCardId,
        Guid userId,
        CreditCardStatementPeriod period,
        DateOnly asOfDate,
        CancellationToken cancellationToken)
    {
        var charges = dbContext.CreditCardCharges.AsNoTracking().Where(
            charge => charge.CreditCardId == creditCardId &&
                      charge.UserId == userId &&
                      !charge.IsCancelled);
        var payments = dbContext.CreditCardPayments.AsNoTracking().Where(
            payment => payment.CreditCardId == creditCardId &&
                       payment.UserId == userId &&
                       !payment.IsCancelled);

        var previousCharges = await charges
            .Where(charge => charge.ChargeDate < period.PeriodStart)
            .SumAsync(charge => charge.Amount.Amount, cancellationToken);
        var previousPayments = await payments
            .Where(payment => payment.PaymentDate < period.PeriodStart)
            .SumAsync(payment => payment.Amount.Amount, cancellationToken);
        var periodCharges = await charges
            .Where(charge => charge.ChargeDate >= period.PeriodStart &&
                             charge.ChargeDate <= period.ClosingDate)
            .SumAsync(charge => charge.Amount.Amount, cancellationToken);
        var paymentsThroughClosing = await payments
            .Where(payment => payment.PaymentDate >= period.PeriodStart &&
                              payment.PaymentDate <= period.ClosingDate)
            .SumAsync(payment => payment.Amount.Amount, cancellationToken);
        var paymentsAfterClosing = await payments
            .Where(payment => payment.PaymentDate > period.ClosingDate &&
                              payment.PaymentDate <= asOfDate)
            .SumAsync(payment => payment.Amount.Amount, cancellationToken);

        return new StatementActivitySnapshot(
            Math.Max(0m, previousCharges - previousPayments),
            periodCharges,
            paymentsThroughClosing,
            paymentsAfterClosing);
    }
}
