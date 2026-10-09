using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Accounts;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Counterparties;

/// <summary>
/// Bir karşı tarafın belirli bir hareketten <b>hemen sonraki</b> net cari
/// bakiyesi: alacak eksi borç. Artı: karşı taraf bize borçlu.
/// </summary>
/// <remarks>
/// Güncel bakiye (<see cref="EfCounterpartyRepository"/> içindeki projection)
/// bütün karşı tarafları tek sorguda okur ve o yüzden ayrı durur; buradaki
/// toplam aynı kaynakları (borçlandırma, tahsilat/ödeme) aynı işaretlerle
/// okur ve yalnız bir kesim noktası ekler. İkisinin ayrışmadığını test tutar:
/// son hareketin sonrası güncel bakiyeye eşittir.
///
/// Kişiye bağlı yükümlülük burada <b>yoktur</b>: cari bakiyeye girmez, kendi
/// kapanışıyla kapanır.
///
/// Kesim akışın sırasıdır: önceki günler bütünüyle, aynı günde giriş anı
/// kesim anından büyük olmayanlar. Giriş anı bilinmeyen eski kayıt günün en
/// eskisi sayılır.
/// </remarks>
internal static class CounterpartyNet
{
    public static async Task<decimal> SumAsync(
        BusinessFinanceDbContext dbContext,
        Guid counterpartyId,
        Guid userId,
        EntryCutoff cutoff,
        CancellationToken cancellationToken)
    {
        var date = cutoff.Date;
        var entryAt = cutoff.EntryAtUtc;

        var charges = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             charge.CounterpartyId == counterpartyId &&
                             !charge.IsCancelled &&
                             (charge.ChargeDate < date ||
                              (charge.ChargeDate == date &&
                               (EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName) == null ||
                                EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName) <= entryAt))))
            .SumAsync(
                charge => charge.Direction == DebtDirection.Receivable
                    ? charge.Amount.Amount
                    : -charge.Amount.Amount,
                cancellationToken);

        // Tahsilat alacağı, ödeme borcu kapatır: işaret borçlandırmanın tersi.
        var payments = await dbContext.CounterpartyPayments.AsNoTracking()
            .Where(payment => payment.UserId == userId &&
                              payment.CounterpartyId == counterpartyId &&
                              !payment.IsCancelled &&
                              (payment.PaymentDate < date ||
                               (payment.PaymentDate == date &&
                                (EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) == null ||
                                 EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) <= entryAt))))
            .SumAsync(
                payment => payment.Direction == DebtDirection.Receivable
                    ? -payment.Amount.Amount
                    : payment.Amount.Amount,
                cancellationToken);

        return charges + payments;
    }
}
