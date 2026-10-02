using Microsoft.EntityFrameworkCore;
using BusinessFinance.Infrastructure.Accounts;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.CreditCards;

/// <summary>
/// Kartın borcu: harcamalar eksi ödemeler. Güncel borç ve "işlem sonrası
/// borç" aynı toplamı okur; ikincisi yalnız bir kesim noktası ekler
/// (<see cref="EntryCutoff"/>).
/// </summary>
internal static class CardDebt
{
    public static async Task<decimal> SumAsync(
        BusinessFinanceDbContext dbContext,
        Guid creditCardId,
        Guid userId,
        EntryCutoff? cutoff,
        CancellationToken cancellationToken)
    {
        var charges = dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.CreditCardId == creditCardId &&
                             charge.UserId == userId &&
                             !charge.IsCancelled);
        var payments = dbContext.CreditCardPayments.AsNoTracking()
            .Where(payment => payment.CreditCardId == creditCardId &&
                              payment.UserId == userId &&
                              !payment.IsCancelled);

        if (cutoff is not null)
        {
            var date = cutoff.Date;
            var entryAt = cutoff.EntryAtUtc;
            charges = charges.Where(charge =>
                charge.ChargeDate < date ||
                (charge.ChargeDate == date &&
                 (EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName) == null ||
                  EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName) <= entryAt)));
            payments = payments.Where(payment =>
                payment.PaymentDate < date ||
                (payment.PaymentDate == date &&
                 (EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) == null ||
                  EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) <= entryAt)));
        }

        var chargeTotal = await charges.SumAsync(charge => charge.Amount.Amount, cancellationToken);
        var paymentTotal = await payments.SumAsync(payment => payment.Amount.Amount, cancellationToken);

        // **Kırpılmaz.** Negatif sonuç kartın alacaklı bakiyesidir: kullanıcı
        // kartına borcundan fazlasını ödemiş ya da ödediği harcama sonradan
        // iptal edilmiştir. Sıfıra çekmek, kullanıcının parasını yok saymak
        // olurdu — cari hesapta "fazla tahsilat kırpılmaz" kararı (Aşama 02)
        // aynı soruya zaten bu cevabı veriyordu.
        return chargeTotal - paymentTotal;
    }
}
