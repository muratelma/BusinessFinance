using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Reports;

/// <summary>
/// Tanınmış tek bir gelir ya da gider kalemi: günü, yönü, kapsamı, kategorisi
/// ve tutarı.
/// </summary>
/// <remarks>
/// Enum'lar <c>int</c> taşınır ki birleşimin her dalı aynı kolon tipini üretsin
/// (birleşik akıştaki <c>ActivityRow</c> ile aynı gerekçe).
/// </remarks>
internal sealed class RecognizedItem
{
    public DateOnly Date { get; init; }

    /// <summary><see cref="TransactionType"/> değeri: gelir ya da gider.</summary>
    public int Type { get; init; }

    /// <summary><see cref="TransactionScope"/> değeri; tanıyan her kayıt taşır.</summary>
    public int Scope { get; init; }

    /// <summary>
    /// Kalemin kategorisi. Eski veride rolü bulunamayan faiz boş kalabilir;
    /// tutar toplamda ve kategorisiz satırda korunur.
    /// </summary>
    public Guid? CategoryId { get; init; }

    public decimal Amount { get; init; }
    public Guid SourceId { get; init; }
    public int SourceKind { get; init; }
    public Guid? ParentId { get; init; }
    public int? ParentKind { get; init; }
    public decimal? ParentAmount { get; init; }
    public string? Part { get; init; }
    public Guid? AccountId { get; init; }
    public Guid? CreditCardId { get; init; }
    public Guid? CounterpartyId { get; init; }
    public DateTimeOffset? EntryAtUtc { get; init; }

}

/// <summary>
/// Gelir ya da gider yazan bütün kayıtların <b>tek</b> listesi.
/// </summary>
/// <remarks>
/// <para>
/// Bir kayıt ya ekonomik olayı tanır ya ödemeyi taşır (ADR 0014). Burası
/// tanıyanların listesidir; taşıyanların listesi
/// <see cref="Accounts.AccountMovements"/> içindedir. Aylık raporun toplamı ve
/// kategori dağılımı, dönem karşılaştırması, nakit akışı eğilimi, Bütçeler
/// listesi ve bütçe sapması hep bu listeyi okur.
/// </para>
/// <para>
/// Liste tek yerde durur çünkü aynı toplam beş ayrı yerde yeniden yazıldığında
/// kaynaklar sessizce ayrışıyordu: bütçe tek seferlik borcu ve POS komisyonunu
/// saymıyor, eğilim borç faizini kapsamla süzmüyordu. Yeni bir tanıma yolu
/// <b>buraya</b> eklenir ve bütün okumalar onu aynı anda görür.
/// </para>
/// <para>
/// Kaynaklar <c>UNION ALL</c> ile birleşir; süzme, gruplama ve toplama
/// veritabanında yapılır.
/// </para>
/// </remarks>
internal static class RecognizedItems
{
    private const int Income = (int)TransactionType.Income;
    private const int Expense = (int)TransactionType.Expense;

    public static IQueryable<RecognizedItem> Query(
        BusinessFinanceDbContext dbContext,
        Guid userId)
    {
        // Yükümlülük ile borç planı tek tabloda (ADR 0021); görünümler eski iki
        // biçimi verir ve satır türleri dalga 1'de değişmez.
        var agreements = Debts.DebtViews.Agreements(dbContext);
        var installments = Debts.DebtViews.Installments(dbContext);
        var obligationRows = Debts.DebtViews.Obligations(dbContext);
        var obligationSettlements = Debts.DebtViews.ObligationSettlements(dbContext);

        // Hesaptan gelir ve gider. Yatışın kesintisi de buradadır: sıradan bir
        // gider olarak yazılır (ADR 0019 T5).
        var transactions = dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId && !transaction.IsCancelled)
            .Select(transaction => new RecognizedItem
            {
                SourceId = transaction.Id,
                SourceKind = (int)FinancialActivityKind.AccountTransaction,
                ParentId = dbContext.Transfers.Where(t => t.UserId == userId && t.FeeTransactionId == transaction.Id).Select(t => (Guid?)t.Id).FirstOrDefault() ?? dbContext.PosDeposits.Where(d => d.UserId == userId && d.DeductionTransactionId == transaction.Id).Select(d => (Guid?)d.Id).FirstOrDefault(),
                ParentKind = dbContext.Transfers.Any(t => t.UserId == userId && t.FeeTransactionId == transaction.Id) ? (int?)FinancialActivityKind.Transfer : dbContext.PosDeposits.Any(d => d.UserId == userId && d.DeductionTransactionId == transaction.Id) ? (int?)FinancialActivityKind.PosDeposit : null,
                ParentAmount = dbContext.Transfers.Where(t => t.UserId == userId && t.FeeTransactionId == transaction.Id).Select(t => (decimal?)t.Amount.Amount).FirstOrDefault() ?? dbContext.PosDeposits.Where(d => d.UserId == userId && d.DeductionTransactionId == transaction.Id).Select(d => (decimal?)d.DepositedAmount.Amount).FirstOrDefault(),
                Part = dbContext.Transfers.Any(t => t.UserId == userId && t.FeeTransactionId == transaction.Id) ? "fee" : dbContext.PosDeposits.Any(d => d.UserId == userId && d.DeductionTransactionId == transaction.Id) ? "deduction" : null,
                AccountId = transaction.AccountId,
                CreditCardId = (Guid?)null,
                CounterpartyId = (Guid?)null,
                EntryAtUtc = EF.Property<DateTimeOffset?>(transaction, EntryTimestamp.PropertyName),

                Date = transaction.TransactionDate,
                Type = transaction.Type == TransactionType.Income ? Income : Expense,
                Scope = (int)transaction.Scope,
                CategoryId = transaction.CategoryId,
                Amount = transaction.Amount.Amount
            });

        // Kart harcaması harcandığı gün giderdir; kart ödemesi taşır.
        var cardCharges = dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId && !charge.IsCancelled)
            .Select(charge => new RecognizedItem
            {
                SourceId = charge.Id,
                SourceKind = (int)FinancialActivityKind.CardCharge,
                ParentId = (Guid?)null,
                ParentKind = (int?)null,
                ParentAmount = (decimal?)null,
                Part = (string?)null,
                AccountId = (Guid?)null,
                CreditCardId = charge.CreditCardId,
                CounterpartyId = (Guid?)null,
                EntryAtUtc = EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName),

                Date = charge.ChargeDate,
                Type = Expense,
                Scope = (int)charge.Scope,
                CategoryId = charge.CategoryId,
                Amount = charge.Amount.Amount
            });

        // Gider ya da gelir kaynaklı borcun açılışı tam o gün tanınır: borçta
        // tüketim, alacakta satış. Nakit kaynaklı açılış taşır, burada yoktur.
        // Taksit ödemeleri tanımaz; tanısalardı aynı olay iki kez sayılırdı.
        var debtOpenings = agreements
            .Where(debt => debt.UserId == userId &&
                           (debt.SourceType == DebtSourceType.Expense ||
                            debt.SourceType == DebtSourceType.Income))
            .Select(debt => new RecognizedItem
            {
                SourceId = debt.Id,
                SourceKind = (int)FinancialActivityKind.DebtOpening,
                ParentId = (Guid?)null,
                ParentKind = (int?)null,
                ParentAmount = (decimal?)null,
                Part = (string?)null,
                AccountId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = debt.CounterpartyId,
                EntryAtUtc = EF.Property<DateTimeOffset?>(debt, EntryTimestamp.PropertyName),

                Date = debt.StartDate,
                Type = debt.SourceType == DebtSourceType.Income ? Income : Expense,
                Scope = (int)debt.Scope,
                CategoryId = debt.CategoryId,
                Amount = debt.Principal.Amount
            });

        // Ödenen taksidin faiz payı: borçta gider, alacakta gelir. Anapara
        // geri ödemesi tanımaz. Kalıcı bir hareket yoktur; faiz ödendiği gün
        // tanınır ve gelir/gider rolünün kovasına düşer. Rol yoksa geçmiş
        // faiz kategorisiz kalır; yeni ödeme rolü ilk gerektiğinde açar.
        var debtInterest =
            from installment in installments
            join debt in agreements
                on new { installment.UserId, DebtId = installment.DebtAgreementId }
                equals new { debt.UserId, DebtId = debt.Id }
            where installment.UserId == userId &&
                  installment.InterestPortion != null &&
                  installment.InterestPortion > 0m &&
                  installment.PaymentDate != null
            select new RecognizedItem
            {
                SourceId = installment.Id,
                SourceKind = debt.Direction == DebtDirection.Receivable ? (int)FinancialActivityKind.DebtCollection : (int)FinancialActivityKind.DebtPayment,
                ParentId = installment.Id,
                ParentKind = debt.Direction == DebtDirection.Receivable ? (int?)FinancialActivityKind.DebtCollection : (int?)FinancialActivityKind.DebtPayment,
                ParentAmount = installment.Amount,
                Part = "interest",
                AccountId = installment.PaymentAccountId,
                CreditCardId = (Guid?)null,
                CounterpartyId = debt.CounterpartyId,
                EntryAtUtc = installment.PaidAtUtc,

                Date = installment.PaymentDate!.Value,
                Type = debt.Direction == DebtDirection.Receivable ? Income : Expense,
                Scope = (int)debt.Scope,
                CategoryId = dbContext.Categories
                        .Where(category =>
                            category.UserId == userId &&
                            category.Role == (debt.Direction == DebtDirection.Receivable
                                ? CategoryRole.InterestIncome : CategoryRole.InterestExpense))
                        .Select(category => (Guid?)category.Id)
                        .FirstOrDefault(),
                Amount = installment.InterestPortion!.Value
            };

        // Cari borçlandırma tanır: veresiye satış o gün gelir, vadeli alım o
        // gün giderdir. Tahsilat ve ödeme taşır.
        var counterpartyCharges = dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId && !charge.IsCancelled)
            .Select(charge => new RecognizedItem
            {
                SourceId = charge.Id,
                SourceKind = (int)FinancialActivityKind.CounterpartyCharge,
                ParentId = (Guid?)null,
                ParentKind = (int?)null,
                ParentAmount = (decimal?)null,
                Part = (string?)null,
                AccountId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = charge.CounterpartyId,
                EntryAtUtc = EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName),

                Date = charge.ChargeDate,
                Type = charge.Direction == DebtDirection.Receivable ? Income : Expense,
                Scope = (int)charge.Scope,
                CategoryId = charge.CategoryId,
                Amount = charge.Amount.Amount
            });

        // Tek seferlik alacak ve borç doğduğu gün tanınır; kapanışı taşır.
        var obligations = obligationRows
            .Where(obligation => obligation.UserId == userId && !obligation.IsCancelled)
            .Select(obligation => new RecognizedItem
            {
                SourceId = obligation.Id,
                SourceKind = (int)FinancialActivityKind.Obligation,
                ParentId = (Guid?)null,
                ParentKind = (int?)null,
                ParentAmount = (decimal?)null,
                Part = (string?)null,
                AccountId = (Guid?)null,
                CreditCardId = (Guid?)null,
                CounterpartyId = obligation.CounterpartyId,
                EntryAtUtc = obligation.CreatedAtUtc,

                Date = obligation.IssueDate,
                Type = obligation.Direction == DebtDirection.Receivable ? Income : Expense,
                Scope = (int)obligation.Scope,
                CategoryId = obligation.CategoryId,
                Amount = obligation.Amount
            });

        // POS satışı tahsil edildiği gün brüt tutarla tanınır, hesaba geçtiği
        // gün değil (ADR 0015). Kartla tahsil gelir yazmaz: gelir alacakta
        // tanındı (ADR 0019 T5).
        var posSales = dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.Kind == PosSettlementKind.Sale)
            .Select(settlement => new RecognizedItem
            {
                SourceId = settlement.Id,
                SourceKind = (int)FinancialActivityKind.PosSale,
                ParentId = (Guid?)null,
                ParentKind = (int?)null,
                ParentAmount = (decimal?)null,
                Part = (string?)null,
                AccountId = settlement.AccountId,
                CreditCardId = (Guid?)null,
                CounterpartyId = (Guid?)null,
                EntryAtUtc = settlement.CreatedAtUtc,

                Date = settlement.SettlementDate,
                Type = Income,
                Scope = (int)settlement.Scope!.Value,
                CategoryId = settlement.CategoryId,
                Amount = settlement.GrossAmount.Amount
            });

        // Komisyon brüte eklenmez ve ondan düşülmez: kendi kategorisinde ayrı
        // bir giderdir, satışta da kartla tahsilde de. Komisyonlu kaydın
        // kapsamı doludur.
        var posCommissions = dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.CommissionAmount > 0m)
            .Select(settlement => new RecognizedItem
            {
                SourceId = settlement.Id,
                SourceKind = (int)FinancialActivityKind.PosSale,
                ParentId = settlement.Kind == PosSettlementKind.Sale ? (Guid?)settlement.Id :
                    dbContext.CounterpartyPayments.Where(p => p.UserId == userId && p.PosSettlementId == settlement.Id)
                        .Select(p => (Guid?)p.Id).FirstOrDefault() ??
                    obligationSettlements.Where(p => p.UserId == userId && p.PosSettlementId == settlement.Id)
                        .Select(p => (Guid?)p.Id).FirstOrDefault(),
                ParentKind = settlement.Kind == PosSettlementKind.Sale ? (int?)FinancialActivityKind.PosSale :
                    dbContext.CounterpartyPayments.Any(p => p.UserId == userId && p.PosSettlementId == settlement.Id)
                        ? (int?)FinancialActivityKind.CounterpartySettlement : (int?)FinancialActivityKind.ObligationSettlement,
                ParentAmount = settlement.GrossAmount.Amount,
                Part = "commission",
                AccountId = settlement.AccountId,
                CreditCardId = (Guid?)null,
                CounterpartyId = (Guid?)null,
                EntryAtUtc = settlement.CreatedAtUtc,

                Date = settlement.SettlementDate,
                Type = Expense,
                Scope = (int)settlement.Scope!.Value,
                CategoryId = settlement.CommissionCategoryId,
                Amount = settlement.CommissionAmount
            });

        return transactions
            .Concat(cardCharges)
            .Concat(debtOpenings)
            .Concat(debtInterest)
            .Concat(counterpartyCharges)
            .Concat(obligations)
            .Concat(posSales)
            .Concat(posCommissions);
    }

    /// <summary>
    /// Bir dönemin kalemleri; <paramref name="scope"/> verilirse yalnız o kapsam.
    /// </summary>
    public static IQueryable<RecognizedItem> InPeriod(
        BusinessFinanceDbContext dbContext,
        Guid userId,
        DateOnly start,
        DateOnly endExclusive,
        TransactionScope? scope)
    {
        var items = Query(dbContext, userId)
            .Where(item => item.Date >= start && item.Date < endExclusive);
        return scope is TransactionScope selected
            ? items.Where(item => item.Scope == (int)selected)
            : items;
    }

    /// <summary>
    /// Bir dönemin giderleri, <b>kategori + kapsam çiftiyle</b>. Bütçenin
    /// harcaması bu sayıdır: raporda o kategorinin o kapsamdaki gideri.
    /// </summary>
    public static async Task<Dictionary<(Guid CategoryId, TransactionScope Scope), decimal>>
        ExpenseByCategoryAndScopeAsync(
            BusinessFinanceDbContext dbContext,
            Guid userId,
            DateOnly start,
            DateOnly endExclusive,
            CancellationToken cancellationToken)
    {
        var rows = await InPeriod(dbContext, userId, start, endExclusive, null)
            .Where(item => item.Type == Expense && item.CategoryId != null)
            .GroupBy(item => new { item.CategoryId, item.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Amount = group.Sum(item => item.Amount)
            })
            .ToArrayAsync(cancellationToken);
        return rows.ToDictionary(
            row => (row.CategoryId!.Value, (TransactionScope)row.Scope),
            row => row.Amount);
    }
}
