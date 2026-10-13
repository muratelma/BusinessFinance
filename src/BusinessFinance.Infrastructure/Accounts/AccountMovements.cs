using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;
using BusinessFinance.Application.FinancialActivities;
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
/// Bir hesabın bakiyesini değiştiren tek bir hareket: hangi hesap, hangi gün,
/// ne zaman yazıldı ve bakiyeyi ne kadar değiştirdi.
/// </summary>
internal sealed class AccountMovement
{
    public Guid AccountId { get; init; }
    public Guid ActivityId { get; init; }
    public int ActivityKind { get; init; }
    public Guid? CounterAccountId { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? CreditCardId { get; init; }
    public Guid? CounterpartyId { get; init; }
    public decimal? PartAmount { get; init; }
    public string? Part { get; init; }
    public int Group { get; init; }


    public DateOnly Date { get; init; }

    /// <summary>Yazıldığı an; eski kayıtlarda boş.</summary>
    public DateTimeOffset? EntryAtUtc { get; init; }

    /// <summary>İşaretli tutar: hesaba giren artı, hesaptan çıkan eksi.</summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Hareket bir gelir/gider kaydıysa kimliği; yatışın kesintisini kesim
    /// noktasından bağımsız saymak için. Diğer kaynaklarda boş.
    /// </summary>
    public Guid? TransactionId { get; init; }
}

/// <summary>
/// Bir hesabın bakiyesini değiştiren bütün hareketlerin <b>tek</b> listesi;
/// açılış bakiyesi hariç.
/// </summary>
/// <remarks>
/// <para>
/// Bir kayıt ya ekonomik olayı tanır ya ödemeyi taşır (ADR 0014). Burası
/// parayı hareket ettirenlerin listesidir; gelir ve gider yazanların listesi
/// <see cref="Reports.RecognizedItems"/> içindedir.
/// </para>
/// <para>
/// Güncel bakiye, "işlem sonrası bakiye", raporlardaki hesap bakiyeleri ve
/// kasanın günlük giren/çıkanı <b>hep bu listeyi</b> okur. Dört ayrı liste
/// olduğunda yeni bir para yolu eklenirken biri unutulur ve sayılar sessizce
/// ayrışır; yeni bir para yolu <b>buraya</b> eklenir.
/// </para>
/// <para>
/// Kaynaklar <c>UNION ALL</c> ile birleşir; süzme ve toplama veritabanında
/// yapılır.
/// </para>
/// </remarks>
internal static class AccountMovements
{
    public static IQueryable<AccountMovement> Query(
        BusinessFinanceDbContext dbContext,
        Guid userId)
    {
        // Gelir hesaba girer, gider çıkar.
        var transactions = dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId && !transaction.IsCancelled &&
                !dbContext.Transfers.Any(t => t.UserId == userId && t.FeeTransactionId == transaction.Id) &&
                !dbContext.PosDeposits.Any(d => d.UserId == userId && d.DeductionTransactionId == transaction.Id))
            .Select(transaction => new AccountMovement
            {
                ActivityId = transaction.Id,
                ActivityKind = (int)FinancialActivityKind.AccountTransaction,
                CounterAccountId = (Guid?)null,
                CategoryId = transaction.CategoryId,
                CreditCardId = (Guid?)null,
                CounterpartyId = (Guid?)null,
                PartAmount = (decimal?)null,
                Part = (string?)null,
                Group = 1,

                AccountId = transaction.AccountId,
                Date = transaction.TransactionDate,
                EntryAtUtc = EF.Property<DateTimeOffset?>(transaction, EntryTimestamp.PropertyName),
                Amount = transaction.Type == TransactionType.Income
                    ? transaction.Amount.Amount
                    : -transaction.Amount.Amount,
                TransactionId = transaction.Id
            });

        // Transfer tek olaydır ama iki hesabı değiştirir: kaynaktan çıkar,
        // hedefe girer.
        var outgoingTransfers = dbContext.Transfers.AsNoTracking()
            .Where(transfer => transfer.UserId == userId && !transfer.IsCancelled)
            .Select(transfer => new AccountMovement
            {
                ActivityId = transfer.Id,
                ActivityKind = (int)FinancialActivityKind.Transfer,
                CounterAccountId = transfer.DestinationAccountId,
                CategoryId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = (Guid?)null,
                PartAmount = transfer.FeeTransaction == null ? (decimal?)null : transfer.FeeTransaction.Amount.Amount,
                Part = transfer.FeeTransactionId != null ? "fee" : null,
                Group = 3,

                AccountId = transfer.SourceAccountId,
                Date = transfer.TransferDate,
                EntryAtUtc = EF.Property<DateTimeOffset?>(transfer, EntryTimestamp.PropertyName),
                Amount = -transfer.Amount.Amount - (transfer.FeeTransaction == null ? 0m : transfer.FeeTransaction.Amount.Amount),
                TransactionId = null
            });
        var incomingTransfers = dbContext.Transfers.AsNoTracking()
            .Where(transfer => transfer.UserId == userId && !transfer.IsCancelled)
            .Select(transfer => new AccountMovement
            {
                ActivityId = transfer.Id,
                ActivityKind = (int)FinancialActivityKind.Transfer,
                CounterAccountId = transfer.SourceAccountId,
                CategoryId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = (Guid?)null,
                PartAmount = (decimal?)null,
                Part = (string?)null,
                Group = 3,

                AccountId = transfer.DestinationAccountId,
                Date = transfer.TransferDate,
                EntryAtUtc = EF.Property<DateTimeOffset?>(transfer, EntryTimestamp.PropertyName),
                Amount = transfer.Amount.Amount,
                TransactionId = null
            });

        var cardPayments = dbContext.CreditCardPayments.AsNoTracking()
            .Where(payment => payment.UserId == userId && !payment.IsCancelled)
            .Select(payment => new AccountMovement
            {
                ActivityId = payment.Id,
                ActivityKind = (int)FinancialActivityKind.CardPayment,
                CounterAccountId = (Guid?)null,
                CategoryId = (Guid?)null,
                CreditCardId = payment.CreditCardId,
                CounterpartyId = (Guid?)null,
                PartAmount = (decimal?)null,
                Part = (string?)null,
                Group = 2,

                AccountId = payment.AccountId,
                Date = payment.PaymentDate,
                EntryAtUtc = EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName),
                Amount = -payment.Amount.Amount,
                TransactionId = null
            });

        // Ödenen taksit: alacakta hesaba girer, borçta çıkar. Tutarın tamamı
        // (anapara + faiz) hareket eder.
        var debtInstallments =
            from installment in dbContext.DebtInstallments.AsNoTracking()
            join debt in dbContext.DebtAgreements.AsNoTracking()
                on new { installment.UserId, DebtId = installment.DebtAgreementId }
                equals new { debt.UserId, DebtId = debt.Id }
            where installment.UserId == userId && installment.PaymentAccountId != null
            select new AccountMovement
            {
                ActivityId = installment.Id,
                ActivityKind = debt.Direction == DebtDirection.Receivable ? (int)FinancialActivityKind.DebtCollection : (int)FinancialActivityKind.DebtPayment,
                CounterAccountId = (Guid?)null,
                CategoryId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = debt.CounterpartyId,
                PartAmount = installment.InterestPortion,
                Part = installment.InterestPortion > 0m ? "interest" : null,
                Group = 2,

                AccountId = installment.PaymentAccountId!.Value,
                Date = installment.PaymentDate!.Value,
                EntryAtUtc = installment.PaidAtUtc,
                Amount = debt.Direction == DebtDirection.Receivable
                    ? installment.Amount.Amount
                    : -installment.Amount.Amount,
                TransactionId = null
            };

        // Borcun açılışı. Nakit kaynaklı bir borçta para hesaba girmiştir,
        // alacakta çıkmıştır; ikisi de gelir/gider değildir. Bu hareket
        // olmadan taksitler hesabı boşaltıyor ama karşılığında hiçbir şey
        // girmemiş görünüyordu. Gider kaynaklı borç parayı hiç hareket
        // ettirmez: tüketim zaten gider olarak yazılır.
        var debtOpenings = dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           debt.OpeningAccountId != null &&
                           debt.SourceType == DebtSourceType.Cash)
            .Select(debt => new AccountMovement
            {
                ActivityId = debt.Id,
                ActivityKind = (int)FinancialActivityKind.DebtOpening,
                CounterAccountId = (Guid?)null,
                CategoryId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = debt.CounterpartyId,
                PartAmount = (decimal?)null,
                Part = (string?)null,
                Group = 2,

                AccountId = debt.OpeningAccountId!.Value,
                Date = debt.StartDate,
                EntryAtUtc = EF.Property<DateTimeOffset?>(debt, EntryTimestamp.PropertyName),
                Amount = debt.Direction == DebtDirection.Payable
                    ? debt.Principal.Amount
                    : -debt.Principal.Amount,
                TransactionId = null
            });

        // Cari tahsilat/ödeme parayı taşır: tahsilat kasayı artırır, ödeme
        // azaltır. Gelir/gider üretmediği için rapora değil yalnız buraya
        // girer (ADR 0014). Kartla tahsil burada sayılmaz: parası POS
        // kaydıyla yoldadır ve hesaba yatışla girer (ADR 0019 T5).
        var counterpartyPayments = dbContext.CounterpartyPayments.AsNoTracking()
            .Where(payment => payment.UserId == userId &&
                              !payment.IsCancelled &&
                              payment.PosSettlementId == null)
            .Select(payment => new AccountMovement
            {
                ActivityId = payment.Id,
                ActivityKind = (int)FinancialActivityKind.CounterpartySettlement,
                CounterAccountId = (Guid?)null,
                CategoryId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = payment.CounterpartyId,
                PartAmount = (decimal?)null,
                Part = (string?)null,
                Group = 2,

                AccountId = payment.AccountId,
                Date = payment.PaymentDate,
                EntryAtUtc = EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName),
                Amount = payment.Direction == DebtDirection.Receivable
                    ? payment.Amount.Amount
                    : -payment.Amount.Amount,
                TransactionId = null
            });
        var obligationSettlements = dbContext.ObligationSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.PosSettlementId == null)
            .Select(settlement => new AccountMovement
            {
                ActivityId = settlement.Id,
                ActivityKind = (int)FinancialActivityKind.ObligationSettlement,
                CounterAccountId = (Guid?)null,
                CategoryId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = dbContext.Obligations.Where(o => o.UserId == userId && o.Id == settlement.ObligationId)
                    .Select(o => o.CounterpartyId).FirstOrDefault(),
                PartAmount = (decimal?)null,
                Part = (string?)null,
                Group = 2,

                AccountId = settlement.AccountId,
                Date = settlement.SettlementDate,
                EntryAtUtc = (DateTimeOffset?)settlement.SettledAtUtc,
                Amount = settlement.Direction == DebtDirection.Receivable
                    ? settlement.Amount.Amount
                    : -settlement.Amount.Amount,
                TransactionId = null
            });

        // Bir yatış tek harekettir: gerçekten yatan tutar girer, kesinti
        // bağlı giderden ikinci kez çıkarılmaz (KH3). Tahsil günü hesap
        // değişmez; gelir ve komisyon RecognizedItems'ta ayrı tanınır.
        var posTransfers = dbContext.PosDeposits.AsNoTracking()
            .Where(deposit => deposit.UserId == userId && !deposit.IsCancelled)
            .Select(deposit => new AccountMovement
            {
                ActivityId = deposit.Id,
                ActivityKind = (int)FinancialActivityKind.PosDeposit,
                CounterAccountId = (Guid?)null,
                CategoryId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = (Guid?)null,
                PartAmount = deposit.DeductionAmount > 0m ? (decimal?)deposit.DeductionAmount : null,
                Part = deposit.DeductionAmount > 0m ? "deduction" : null,
                Group = 2,

                AccountId = deposit.AccountId,
                Date = deposit.DepositDate,
                EntryAtUtc = deposit.CreatedAtUtc,
                Amount = deposit.DepositedAmount.Amount,
                TransactionId = null
            });

        return transactions
            .Concat(outgoingTransfers)
            .Concat(incomingTransfers)
            .Concat(cardPayments)
            .Concat(debtInstallments)
            .Concat(debtOpenings)
            .Concat(counterpartyPayments)
            .Concat(obligationSettlements)
            .Concat(posTransfers);
    }

    /// <summary>
    /// Bir hesabın hareketlerinin toplamı; <paramref name="cutoff"/> verilirse
    /// o hareketin hemen sonrasına kadar.
    /// </summary>
    /// <remarks>
    /// Kesim, akışın sırasıyla aynıdır: önceki günler bütünüyle, aynı günde ise
    /// giriş anı kesim anından büyük olmayanlar. Giriş anı bilinmeyen eski kayıt
    /// günün en eskisi sayılır ve toplama girer.
    /// </remarks>
    public static Task<decimal> SumAsync(
        BusinessFinanceDbContext dbContext,
        Guid accountId,
        Guid userId,
        EntryCutoff? cutoff,
        CancellationToken cancellationToken)
    {
        var movements = Query(dbContext, userId)
            .Where(movement => movement.AccountId == accountId);
        if (cutoff is not null)
        {
            var date = cutoff.Date;
            var entryAt = cutoff.EntryAtUtc;
            var ownExpenseId = cutoff.OwnExpenseId;
            movements = movements.Where(movement =>
                movement.Date < date ||
                (movement.Date == date &&
                 (movement.EntryAtUtc == null || movement.EntryAtUtc <= entryAt)) ||
                (ownExpenseId != null && movement.TransactionId == ownExpenseId));
        }

        return movements.SumAsync(movement => movement.Amount, cancellationToken);
    }

    /// <summary>
    /// Kullanıcının bütün hesaplarının hareket toplamı, hesap başına;
    /// <paramref name="asOfDate"/> verilirse o gün dahil.
    /// </summary>
    public static Task<Dictionary<Guid, decimal>> SumByAccountAsync(
        BusinessFinanceDbContext dbContext,
        Guid userId,
        DateOnly? asOfDate,
        CancellationToken cancellationToken)
    {
        var movements = Query(dbContext, userId);
        if (asOfDate is DateOnly asOf)
        {
            movements = movements.Where(movement => movement.Date <= asOf);
        }

        return movements
            .GroupBy(movement => movement.AccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Amount = group.Sum(movement => movement.Amount)
            })
            .ToDictionaryAsync(row => row.AccountId, row => row.Amount, cancellationToken);
    }

    /// <summary>
    /// Bir hesaba bir dönemde giren ve çıkan toplam; iki uç da dahil.
    /// </summary>
    public static async Task<(decimal Inflow, decimal Outflow)> FlowAsync(
        BusinessFinanceDbContext dbContext,
        Guid accountId,
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        var flow = await Query(dbContext, userId)
            .Where(movement => movement.AccountId == accountId &&
                               movement.Date >= from &&
                               movement.Date <= to)
            .GroupBy(movement => movement.AccountId)
            .Select(group => new
            {
                Inflow = group.Sum(movement => movement.Amount > 0m ? movement.Amount : 0m),
                Outflow = group.Sum(movement => movement.Amount < 0m ? -movement.Amount : 0m)
            })
            .SingleOrDefaultAsync(cancellationToken);
        return flow is null ? (0m, 0m) : (flow.Inflow, flow.Outflow);
    }
}
