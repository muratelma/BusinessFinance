using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Accounts;

/// <summary>
/// Bir hareketin akıştaki yeri: günü ve gün içinde yazıldığı an. Bakiye "bu
/// hareketten sonra" diye sorulduğunda toplam bu noktada kesilir.
/// </summary>
/// <param name="Date">Hareketin günü.</param>
/// <param name="EntryAtUtc">Hareketin yazıldığı an.</param>
/// <param name="OwnExpenseId">
/// Hareketle aynı yazmada doğan ama saati bir an sonra düşen gider (yatışın
/// kesintisi); kesim noktasından bağımsız olarak toplama girer.
/// </param>
internal sealed record EntryCutoff(DateOnly Date, DateTimeOffset EntryAtUtc, Guid? OwnExpenseId);

/// <summary>
/// Bir hesabın bakiyesini değiştiren bütün hareketlerin toplamı; açılış
/// bakiyesi hariç.
/// </summary>
/// <remarks>
/// Kaynakların listesi <b>tek yerde</b> durur: güncel bakiye
/// (<see cref="EfAccountRepository.CalculateBalanceAsync"/>) ve "işlem sonrası
/// bakiye" aynı listeyi okur, ikincisi yalnız bir kesim noktası ekler. İki ayrı
/// liste olsaydı yeni bir para yolu eklendiğinde biri unutulur ve iki sayı
/// sessizce ayrışırdı.
///
/// Kesim, akışın sırasıyla aynıdır: önceki günler bütünüyle, aynı günde ise
/// giriş anı kesim anından büyük olmayanlar. Giriş anı bilinmeyen eski kayıt
/// günün en eskisi sayılır ve toplama girer.
/// </remarks>
internal static class AccountMovements
{
    public static async Task<decimal> SumAsync(
        BusinessFinanceDbContext dbContext,
        Guid accountId,
        Guid userId,
        EntryCutoff? cutoff,
        CancellationToken cancellationToken)
    {
        var transactions = dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.AccountId == accountId &&
                                  transaction.UserId == userId &&
                                  !transaction.IsCancelled);
        var outgoingTransfers = dbContext.Transfers.AsNoTracking()
            .Where(transfer => transfer.SourceAccountId == accountId &&
                               transfer.UserId == userId &&
                               !transfer.IsCancelled);
        var incomingTransfers = dbContext.Transfers.AsNoTracking()
            .Where(transfer => transfer.DestinationAccountId == accountId &&
                               transfer.UserId == userId &&
                               !transfer.IsCancelled);
        var cardPayments = dbContext.CreditCardPayments.AsNoTracking()
            .Where(payment => payment.AccountId == accountId &&
                              payment.UserId == userId &&
                              !payment.IsCancelled);
        var installments = dbContext.DebtInstallments.AsNoTracking()
            .Where(installment => installment.UserId == userId &&
                                  installment.PaymentAccountId == accountId);

        // Borcun açılışı. Nakit kaynaklı bir borçta para hesaba girmiştir,
        // alacakta çıkmıştır; ikisi de gelir/gider değildir. Bu hareket
        // olmadan taksitler hesabı boşaltıyor ama karşılığında hiçbir şey
        // girmemiş görünüyordu. Gider kaynaklı borç parayı hiç hareket
        // ettirmez: tüketim zaten gider olarak yazılır.
        var debtOpenings = dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           debt.OpeningAccountId == accountId &&
                           debt.SourceType == DebtSourceType.Cash);

        // Cari tahsilat/ödeme parayı taşır: tahsilat kasayı artırır, ödeme
        // azaltır. Gelir/gider üretmediği için rapora değil yalnız buraya
        // girer (ADR 0014).
        var counterpartyPayments = dbContext.CounterpartyPayments.AsNoTracking()
            .Where(payment => payment.AccountId == accountId &&
                              payment.UserId == userId &&
                              !payment.IsCancelled);
        var obligationSettlements = dbContext.ObligationSettlements.AsNoTracking()
            .Where(settlement => settlement.AccountId == accountId &&
                                 settlement.UserId == userId &&
                                 !settlement.IsCancelled);

        // POS tahsilatı hesaba **ancak geçtiği gün** girer ve girdiği tutar
        // nettir (ADR 0015). Tahsilat günü eklenseydi, kullanılabilir bakiye
        // daha bankaya ulaşmamış parayı harcanabilir gösterirdi; brüt
        // eklenseydi bankanın kestiği komisyon kullanıcının cebinde sayılırdı.
        var posTransfers = dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.AccountId == accountId &&
                                 settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.TransferredOn != null);

        if (cutoff is not null)
        {
            var date = cutoff.Date;
            var entryAt = cutoff.EntryAtUtc;
            var ownExpenseId = cutoff.OwnExpenseId;
            transactions = transactions.Where(transaction =>
                transaction.TransactionDate < date ||
                (transaction.TransactionDate == date &&
                 (EF.Property<DateTimeOffset?>(transaction, EntryTimestamp.PropertyName) == null ||
                  EF.Property<DateTimeOffset?>(transaction, EntryTimestamp.PropertyName) <= entryAt)) ||
                transaction.Id == ownExpenseId);
            outgoingTransfers = outgoingTransfers.Where(transfer =>
                transfer.TransferDate < date ||
                (transfer.TransferDate == date &&
                 (EF.Property<DateTimeOffset?>(transfer, EntryTimestamp.PropertyName) == null ||
                  EF.Property<DateTimeOffset?>(transfer, EntryTimestamp.PropertyName) <= entryAt)));
            incomingTransfers = incomingTransfers.Where(transfer =>
                transfer.TransferDate < date ||
                (transfer.TransferDate == date &&
                 (EF.Property<DateTimeOffset?>(transfer, EntryTimestamp.PropertyName) == null ||
                  EF.Property<DateTimeOffset?>(transfer, EntryTimestamp.PropertyName) <= entryAt)));
            cardPayments = cardPayments.Where(payment =>
                payment.PaymentDate < date ||
                (payment.PaymentDate == date &&
                 (EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) == null ||
                  EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) <= entryAt)));
            installments = installments.Where(installment =>
                installment.PaymentDate < date ||
                (installment.PaymentDate == date &&
                 (installment.PaidAtUtc == null || installment.PaidAtUtc <= entryAt)));
            debtOpenings = debtOpenings.Where(debt =>
                debt.StartDate < date ||
                (debt.StartDate == date &&
                 (EF.Property<DateTimeOffset?>(debt, EntryTimestamp.PropertyName) == null ||
                  EF.Property<DateTimeOffset?>(debt, EntryTimestamp.PropertyName) <= entryAt)));
            counterpartyPayments = counterpartyPayments.Where(payment =>
                payment.PaymentDate < date ||
                (payment.PaymentDate == date &&
                 (EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) == null ||
                  EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName) <= entryAt)));
            obligationSettlements = obligationSettlements.Where(settlement =>
                settlement.SettlementDate < date ||
                (settlement.SettlementDate == date && settlement.SettledAtUtc <= entryAt));
            posTransfers = posTransfers.Where(settlement =>
                settlement.TransferredOn < date ||
                (settlement.TransferredOn == date && settlement.TransferredAtUtc <= entryAt));
        }

        var transactionTotal = await transactions.SumAsync(
            transaction => transaction.Type == TransactionType.Income
                ? transaction.Amount.Amount
                : -transaction.Amount.Amount,
            cancellationToken);
        var outgoingTotal = await outgoingTransfers.SumAsync(
            transfer => transfer.Amount.Amount, cancellationToken);
        var incomingTotal = await incomingTransfers.SumAsync(
            transfer => transfer.Amount.Amount, cancellationToken);
        var cardPaymentTotal = await cardPayments.SumAsync(
            payment => payment.Amount.Amount, cancellationToken);
        var installmentTotal = await (
                from installment in installments
                join debt in dbContext.DebtAgreements.AsNoTracking()
                    on new { installment.UserId, DebtId = installment.DebtAgreementId }
                    equals new { debt.UserId, DebtId = debt.Id }
                select debt.Direction == DebtDirection.Receivable
                    ? installment.Amount.Amount
                    : -installment.Amount.Amount)
            .SumAsync(cancellationToken);
        var debtOpeningTotal = await debtOpenings.SumAsync(
            debt => debt.Direction == DebtDirection.Payable
                ? debt.Principal.Amount
                : -debt.Principal.Amount,
            cancellationToken);
        var counterpartyTotal = await counterpartyPayments.SumAsync(
            payment => payment.Direction == DebtDirection.Receivable
                ? payment.Amount.Amount
                : -payment.Amount.Amount,
            cancellationToken);
        var obligationTotal = await obligationSettlements.SumAsync(
            settlement => settlement.Direction == DebtDirection.Receivable
                ? settlement.Amount.Amount
                : -settlement.Amount.Amount,
            cancellationToken);
        var posTotal = await posTransfers.SumAsync(
            settlement => settlement.GrossAmount.Amount - settlement.CommissionAmount,
            cancellationToken);

        return transactionTotal + incomingTotal - outgoingTotal - cardPaymentTotal +
               installmentTotal + debtOpeningTotal + counterpartyTotal + obligationTotal +
               posTotal;
    }
}
