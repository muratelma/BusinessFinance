using Microsoft.EntityFrameworkCore;
using BusinessFinance.Infrastructure.Accounts;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.CreditCards;

/// <summary>
/// Kartın borcu: harcamalar ve kartla ödenen borçlar, eksi kart ödemeleri.
/// Güncel borç ve "işlem sonrası borç" aynı toplamı okur; ikincisi yalnız bir
/// kesim noktası ekler (<see cref="EntryCutoff"/>).
/// </summary>
/// <remarks>
/// Kartla ödenen borç gider yazmaz (gider borcun açıldığı gün yazıldı), ama kart
/// borcunu artırır (ADR 0021 T4).
/// </remarks>
internal static class CardDebt
{
    /// <summary>
    /// Karta yazılan tutarlar: harcamalar ve kartla ödenen borçlar, gününe göre.
    /// Ekstre, kalan limit ve net varlıktaki kart borcu harcamayı buradan okur.
    /// </summary>
    public static IQueryable<CardChargeRow> ChargeRows(BusinessFinanceDbContext dbContext, Guid userId) =>
        dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId && !charge.IsCancelled)
            .Select(charge => new CardChargeRow
            {
                CardId = charge.CreditCardId,
                Date = charge.ChargeDate,
                Amount = charge.Amount.Amount
            })
            .Concat(dbContext.DebtPayments.AsNoTracking()
                .Where(payment => payment.UserId == userId && !payment.IsCancelled && payment.CreditCardId != null)
                .Select(payment => new CardChargeRow
                {
                    CardId = payment.CreditCardId!.Value,
                    Date = payment.PaymentDate,
                    Amount = payment.Amount.Amount
                }));

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
        var debtPayments = dbContext.DebtPayments.AsNoTracking()
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
            debtPayments = debtPayments.Where(payment =>
                payment.PaymentDate < date ||
                (payment.PaymentDate == date && !(payment.PaidAtUtc > entryAt)));
        }

        var chargeTotal = await charges.SumAsync(charge => charge.Amount.Amount, cancellationToken);
        var paymentTotal = await payments.SumAsync(payment => payment.Amount.Amount, cancellationToken);
        var debtPaymentTotal = await debtPayments.SumAsync(
            payment => (decimal?)payment.Amount.Amount, cancellationToken) ?? 0m;

        // **Kırpılmaz.** Negatif sonuç kartın alacaklı bakiyesidir: kullanıcı
        // kartına borcundan fazlasını ödemiş ya da ödediği harcama sonradan
        // iptal edilmiştir. Sıfıra çekmek, kullanıcının parasını yok saymak
        // olurdu — cari hesapta "fazla tahsilat kırpılmaz" kararı (Aşama 02)
        // aynı soruya zaten bu cevabı veriyordu.
        return chargeTotal + debtPaymentTotal - paymentTotal;
    }
}

internal sealed class CardChargeRow
{
    public Guid CardId { get; init; }
    public DateOnly Date { get; init; }
    public decimal Amount { get; init; }
}
