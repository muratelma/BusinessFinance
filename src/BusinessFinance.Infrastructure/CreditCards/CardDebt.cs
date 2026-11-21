using Microsoft.EntityFrameworkCore;
using BusinessFinance.Infrastructure.Accounts;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.CreditCards;

/// <summary>
/// Kartın borcu: harcamalar, kartla ödenen borçlar ve kişilere kartla yapılan
/// ödemeler, eksi kart ödemeleri.
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
                Scope = charge.Scope,
                Amount = charge.Amount.Amount
            })
            .Concat(dbContext.DebtPayments.AsNoTracking()
                .Where(payment => payment.UserId == userId && !payment.IsCancelled && payment.CreditCardId != null)
                .Select(payment => new CardChargeRow
                {
                    CardId = payment.CreditCardId!.Value,
                    Date = payment.PaymentDate,
                    Scope = dbContext.Debts.Where(debt => debt.UserId == userId && debt.Id == payment.DebtId)
                        .Select(debt => (TransactionScope?)debt.Scope).FirstOrDefault(),
                    Amount = payment.Amount.Amount
                }))
            // Kişiye kartla ödeme (S2): gider değildir, kart borcudur.
            .Concat(dbContext.CounterpartyPayments.AsNoTracking()
                .Where(payment => payment.UserId == userId && !payment.IsCancelled && payment.CreditCardId != null)
                .Select(payment => new CardChargeRow
                {
                    CardId = payment.CreditCardId!.Value,
                    Date = payment.PaymentDate,
                    Scope = dbContext.Counterparties.Where(person => person.UserId == userId && person.Id == payment.CounterpartyId)
                        .Select(person => person.Scope).FirstOrDefault(),
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
        var counterpartyPayments = dbContext.CounterpartyPayments.AsNoTracking()
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
            counterpartyPayments = counterpartyPayments.Where(payment =>
                payment.PaymentDate < date ||
                (payment.PaymentDate == date &&
                 (EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) == null ||
                  EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) <= entryAt)));
        }

        var chargeTotal = await charges.SumAsync(charge => charge.Amount.Amount, cancellationToken);
        var paymentTotal = await payments.SumAsync(payment => payment.Amount.Amount, cancellationToken);
        var debtPaymentTotal = await debtPayments.SumAsync(
            payment => (decimal?)payment.Amount.Amount, cancellationToken) ?? 0m;
        var counterpartyPaymentTotal = await counterpartyPayments.SumAsync(
            payment => (decimal?)payment.Amount.Amount, cancellationToken) ?? 0m;

        // **Kırpılmaz.** Negatif sonuç kartın alacaklı bakiyesidir: kullanıcı
        // kartına borcundan fazlasını ödemiş ya da ödediği harcama sonradan
        // iptal edilmiştir. Sıfıra çekmek, kullanıcının parasını yok saymak
        // olurdu — cari hesapta "fazla tahsilat kırpılmaz" kararı (Aşama 02)
        // aynı soruya zaten bu cevabı veriyordu.
        return chargeTotal + debtPaymentTotal + counterpartyPaymentTotal - paymentTotal;
    }

    /// <summary>
    /// Kartın taksit planlarında henüz karta yansımamış taksitlerin toplamı.
    /// Borca girmez (taksit yansıdığı gün borç ve gider olur); yalnız
    /// kullanılabilir limitten düşer: banka taksitli alışverişin tamamını
    /// alışveriş günü limitten ayırır.
    /// </summary>
    public static async Task<decimal> PendingInstallmentsAsync(
        BusinessFinanceDbContext dbContext,
        Guid creditCardId,
        Guid userId,
        CancellationToken cancellationToken) =>
        await (from item in dbContext.InstallmentItems.AsNoTracking()
               join plan in dbContext.InstallmentPlans.AsNoTracking()
                   on item.InstallmentPlanId equals plan.Id
               where item.UserId == userId && plan.UserId == userId &&
                     plan.CreditCardId == creditCardId && item.CreditCardChargeId == null
               select (decimal?)item.Amount.Amount).SumAsync(cancellationToken) ?? 0m;
}

internal sealed class CardChargeRow
{
    public TransactionScope? Scope { get; init; }
    public Guid CardId { get; init; }
    public DateOnly Date { get; init; }
    public decimal Amount { get; init; }
}
