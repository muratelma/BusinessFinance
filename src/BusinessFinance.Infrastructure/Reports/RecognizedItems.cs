using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Categories;
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
    /// Kalemin kategorisi. Yalnız iki durumda boştur: kategorisi bulunamayan
    /// borç faizi ve alacaktan kazanılan faiz. İkisi de toplama girer,
    /// kategori dağılımına girmez.
    /// </summary>
    public Guid? CategoryId { get; init; }

    public decimal Amount { get; init; }
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
        // Hesaptan gelir ve gider. Yatışın kesintisi de buradadır: sıradan bir
        // gider olarak yazılır (ADR 0019 T5).
        var transactions = dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId && !transaction.IsCancelled)
            .Select(transaction => new RecognizedItem
            {
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
                Date = charge.ChargeDate,
                Type = Expense,
                Scope = (int)charge.Scope,
                CategoryId = charge.CategoryId,
                Amount = charge.Amount.Amount
            });

        // Gider ya da gelir kaynaklı borcun açılışı tam o gün tanınır: borçta
        // tüketim, alacakta satış. Nakit kaynaklı açılış taşır, burada yoktur.
        // Taksit ödemeleri tanımaz; tanısalardı aynı olay iki kez sayılırdı.
        var debtOpenings = dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           (debt.SourceType == DebtSourceType.Expense ||
                            debt.SourceType == DebtSourceType.Income))
            .Select(debt => new RecognizedItem
            {
                Date = debt.StartDate,
                Type = debt.SourceType == DebtSourceType.Income ? Income : Expense,
                Scope = (int)debt.Scope,
                CategoryId = debt.CategoryId,
                Amount = debt.Principal.Amount
            });

        // Ödenen taksidin faiz payı: borçta gider, alacakta gelir. Anapara
        // geri ödemesi tanımaz. Kalıcı bir hareket yoktur; faiz ödendiği gün
        // tanınır ve giderse kendi kategorisinin kovasına düşer. Kategori
        // kanonik adıyla bulunur; yoksa faiz toplamda kalır, dağılımda görünmez.
        var debtInterest =
            from installment in dbContext.DebtInstallments.AsNoTracking()
            join debt in dbContext.DebtAgreements.AsNoTracking()
                on new { installment.UserId, DebtId = installment.DebtAgreementId }
                equals new { debt.UserId, DebtId = debt.Id }
            where installment.UserId == userId &&
                  installment.InterestPortion != null &&
                  installment.InterestPortion > 0m &&
                  installment.PaymentDate != null
            select new RecognizedItem
            {
                Date = installment.PaymentDate!.Value,
                Type = debt.Direction == DebtDirection.Receivable ? Income : Expense,
                Scope = (int)debt.Scope,
                CategoryId = debt.Direction == DebtDirection.Receivable
                    ? null
                    : dbContext.Categories
                        .Where(category =>
                            category.UserId == userId &&
                            category.Type == CategoryType.Expense &&
                            category.Name == EfCategoryRepository.InterestExpenseCategoryName)
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
                Date = charge.ChargeDate,
                Type = charge.Direction == DebtDirection.Receivable ? Income : Expense,
                Scope = (int)charge.Scope,
                CategoryId = charge.CategoryId,
                Amount = charge.Amount.Amount
            });

        // Tek seferlik alacak ve borç doğduğu gün tanınır; kapanışı taşır.
        var obligations = dbContext.Obligations.AsNoTracking()
            .Where(obligation => obligation.UserId == userId && !obligation.IsCancelled)
            .Select(obligation => new RecognizedItem
            {
                Date = obligation.IssueDate,
                Type = obligation.Direction == DebtDirection.Receivable ? Income : Expense,
                Scope = (int)obligation.Scope,
                CategoryId = obligation.CategoryId,
                Amount = obligation.Amount.Amount
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
