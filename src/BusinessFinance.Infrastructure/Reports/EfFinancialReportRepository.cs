using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Reports;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using BusinessFinance.Application.UpcomingPayments;

namespace BusinessFinance.Infrastructure.Reports;

internal sealed class EfFinancialReportRepository(
    BusinessFinanceDbContext dbContext,
    IUpcomingPaymentRepository upcomingPaymentRepository)
    : IFinancialReportRepository
{
    /// <summary>
    /// Bir ayın gelir/gider tablosu; <paramref name="scope"/> verilirse yalnız o
    /// kapsam.
    /// </summary>
    /// <remarks>
    /// Kapsam filtresi <b>yalnız gelir/gider tarafına</b> uygulanır. Hesap
    /// bakiyeleri filtreden etkilenmez: kasadaki para tek havuzdur ve kapsam
    /// anahtarının konumuna göre değişmez (ADR 0013). Bölünen ile bölünmeyeni
    /// aynı sorguda tutmak bu ayrımı kolayca bozardı, o yüzden bakiye
    /// sorgularına kapsam hiç girmiyor ve bunu bir test koruyor.
    /// </remarks>
    public async Task<MonthlyReportDto> GetMonthlyAsync(
        Guid userId,
        int year,
        int month,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var start = new DateOnly(year, month, 1);
        var endExclusive = start.AddMonths(1);

        // Tanınmış bütün gelir ve gider kalemleri tek listeden okunur
        // (RecognizedItems); kaynakların her biri orada bir kez tanımlıdır.
        var items = RecognizedItems.InPeriod(dbContext, userId, start, endExclusive, scope);

        // Toplamlar kapsam kırılımıyla birlikte okunuyor: tek gruplu sorgu, en
        // fazla dört satır dönüyor ve toplam onların toplamı. Kırılımı ikinci
        // bir tur sorguyla almak, özet ekranının ilk isteğini iki katına
        // çıkarırdı. Üçüncü bir kova yok: gelir/gider üreten her kayıt tam
        // olarak bir kapsam taşır, bu yüzden iki taraf filtresiz okumanın
        // toplamını birebir verir.
        var totals = await items
            .GroupBy(item => new { item.Type, item.Scope })
            .Select(group => new
            {
                group.Key.Type,
                group.Key.Scope,
                Amount = group.Sum(item => item.Amount)
            })
            .ToArrayAsync(cancellationToken);
        decimal Total(TransactionType type, TransactionScope side) => totals
            .Where(row => row.Type == (int)type && row.Scope == (int)side)
            .Sum(row => row.Amount);
        var businessIncome = Total(TransactionType.Income, TransactionScope.Business);
        var personalIncome = Total(TransactionType.Income, TransactionScope.Personal);
        var businessExpense = Total(TransactionType.Expense, TransactionScope.Business);
        var personalExpense = Total(TransactionType.Expense, TransactionScope.Personal);
        var totalIncome = businessIncome + personalIncome;
        var totalExpense = businessExpense + personalExpense;

        // Dağılım toplamla aynı kalemlerden gelir; "Gider" ile kategori
        // listesinin birbirini tutması bunun sonucudur. Kategorisi bulunamayan
        // borç faizi toplamda kalır, burada görünmez: uydurma bir kimlikle
        // satır üretmek daha kötü olurdu, istemci o kimlikle filtreleyip boş
        // sonuç alırdı.
        var categoryExpenses = (await (
                    from item in items
                    join category in dbContext.Categories.AsNoTracking()
                            .Where(category => category.UserId == userId)
                        on item.CategoryId equals (Guid?)category.Id
                    where item.Type == (int)TransactionType.Expense
                    group item by new { category.Id, category.Name }
                    into expenseGroup
                    select new CategoryExpenseDto(
                        expenseGroup.Key.Id,
                        expenseGroup.Key.Name,
                        expenseGroup.Sum(item => item.Amount)))
                .ToArrayAsync(cancellationToken))
            .OrderByDescending(item => item.Amount)
            .ThenBy(item => item.CategoryName)
            .ThenBy(item => item.CategoryId)
            .ToArray();

        var accounts = await dbContext.Accounts.AsNoTracking()
            .Where(account => account.UserId == userId)
            .OrderBy(account => account.Name)
            .ThenBy(account => account.Id)
            .Select(account => new { account.Id, account.Name, account.OpeningBalance, account.Type })
            .ToArrayAsync(cancellationToken);
        var movements = await dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId && !transaction.IsCancelled)
            .GroupBy(transaction => transaction.AccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Balance = group.Sum(transaction => transaction.Type == TransactionType.Income
                    ? transaction.Amount.Amount
                    : -transaction.Amount.Amount)
            })
            .ToDictionaryAsync(value => value.AccountId, value => value.Balance, cancellationToken);
        var outgoingTransfers = await dbContext.Transfers.AsNoTracking()
            .Where(transfer => transfer.UserId == userId && !transfer.IsCancelled)
            .GroupBy(transfer => transfer.SourceAccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Amount = group.Sum(transfer => transfer.Amount.Amount)
            })
            .ToDictionaryAsync(value => value.AccountId, value => value.Amount, cancellationToken);
        var incomingTransfers = await dbContext.Transfers.AsNoTracking()
            .Where(transfer => transfer.UserId == userId && !transfer.IsCancelled)
            .GroupBy(transfer => transfer.DestinationAccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Amount = group.Sum(transfer => transfer.Amount.Amount)
            })
            .ToDictionaryAsync(value => value.AccountId, value => value.Amount, cancellationToken);
        var cardPayments = await dbContext.CreditCardPayments.AsNoTracking()
            .Where(payment => payment.UserId == userId && !payment.IsCancelled)
            .GroupBy(payment => payment.AccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Amount = group.Sum(payment => payment.Amount.Amount)
            })
            .ToDictionaryAsync(value => value.AccountId, value => value.Amount, cancellationToken);
        var debtMovements = await (
                from installment in dbContext.DebtInstallments.AsNoTracking()
                join debt in dbContext.DebtAgreements.AsNoTracking()
                    on new { installment.UserId, DebtId = installment.DebtAgreementId }
                    equals new { debt.UserId, DebtId = debt.Id }
                where installment.UserId == userId && installment.PaymentAccountId != null
                group new { installment, debt } by installment.PaymentAccountId into movementGroup
                select new
                {
                    AccountId = movementGroup.Key!.Value,
                    Amount = movementGroup.Sum(item => item.debt.Direction == DebtDirection.Receivable
                        ? item.installment.Amount.Amount
                        : -item.installment.Amount.Amount)
                })
            .ToDictionaryAsync(value => value.AccountId, value => value.Amount, cancellationToken);
        var debtOpenings = await DebtOpeningsByAccountAsync(userId, null, cancellationToken);
        var counterpartySettlements = await CounterpartySettlementsByAccountAsync(
            userId, null, cancellationToken);
        var obligationSettlements = await ObligationSettlementsByAccountAsync(
            userId, null, cancellationToken);
        var posTransfers = await PosTransfersByAccountAsync(userId, null, cancellationToken);
        var accountBalances = accounts.Select(account => new AccountBalanceDto(
            account.Id,
            account.Name,
            account.OpeningBalance +
            movements.GetValueOrDefault(account.Id) +
            incomingTransfers.GetValueOrDefault(account.Id) -
            outgoingTransfers.GetValueOrDefault(account.Id) -
            cardPayments.GetValueOrDefault(account.Id) +
            debtMovements.GetValueOrDefault(account.Id) +
            debtOpenings.GetValueOrDefault(account.Id) +
            counterpartySettlements.GetValueOrDefault(account.Id) +
            obligationSettlements.GetValueOrDefault(account.Id) +
            posTransfers.GetValueOrDefault(account.Id),
            account.Type)).ToArray();

        return new MonthlyReportDto(
            year,
            month,
            scope,
            totalIncome,
            totalExpense,
            totalIncome - totalExpense,
            CurrencyCode.TRY,
            categoryExpenses,
            ToSlices(categoryExpenses),
            accountBalances,
            // Kırılım yalnız filtresiz okumada anlamlı: filtre verilmişse
            // rapor zaten tek tarafı anlatıyor ve dışlanan taraf sıfır
            // görünürdü — "o tarafta hiç hareket yok" demek olurdu.
            scope is null
                ? new MonthlyScopeBreakdownDto(
                    new ScopeTotalsDto(
                        businessIncome,
                        businessExpense,
                        businessIncome - businessExpense),
                    new ScopeTotalsDto(
                        personalIncome,
                        personalExpense,
                        personalIncome - personalExpense))
                : null);
    }

    /// <summary>
    /// Halka grafiğin dilimleri: en büyük <see cref="SliceCategoryCount"/>
    /// kategori ve geri kalanının toplamı.
    /// </summary>
    /// <remarks>
    /// Toplama burada, sunucuda: "Diğer" bir finansal toplamdır ve istemci
    /// parayı ikinci kez hesaplamaz. Eşik de burada çünkü kaç dilimin
    /// okunabildiği tek bir yerde karara bağlanmalı — istemci kendi sayısını
    /// seçseydi "Diğer" ile grafik birbirini tutmazdı.
    ///
    /// Kalan tek bir kategoriyse "Diğer" satırı üretilmiyor: bir kategoriyi
    /// adını gizleyip "Diğer" diye göstermek bilgi kaybıdır.
    /// </remarks>
    internal const int SliceCategoryCount = 4;

    internal static IReadOnlyList<CategoryExpenseSliceDto> ToSlices(
        IReadOnlyList<CategoryExpenseDto> categoryExpenses)
    {
        ArgumentNullException.ThrowIfNull(categoryExpenses);
        if (categoryExpenses.Count <= SliceCategoryCount + 1)
        {
            return [.. categoryExpenses.Select(item =>
                new CategoryExpenseSliceDto(item.CategoryId, item.CategoryName, item.Amount))];
        }

        var top = categoryExpenses.Take(SliceCategoryCount);
        var rest = categoryExpenses.Skip(SliceCategoryCount).Sum(item => item.Amount);
        return
        [
            .. top.Select(item =>
                new CategoryExpenseSliceDto(item.CategoryId, item.CategoryName, item.Amount)),
            new CategoryExpenseSliceDto(null, OtherSliceName, rest)
        ];
    }

    internal const string OtherSliceName = "Diğer";

    /// <summary>
    /// Gelişmiş rapor; <paramref name="scope"/> verilirse gelir/gider tarafı o
    /// kapsamla daralır.
    /// </summary>
    /// <remarks>
    /// <b>Net varlık, hesap dağılımı ve kart dağılımı kapsam filtresinden
    /// etkilenmez</b> (ADR 0013). Kullanıcının kasasındaki para ve kartına
    /// olan borcu tek havuzdur; kapsam anahtarının konumuna göre değişseydi,
    /// aynı anda iki farklı "ne kadar param var" cevabı doğru olurdu.
    /// Bölünen dönem karşılaştırması, nakit akışı eğilimi ve bütçe sapmasıdır.
    /// </remarks>
    public async Task<AdvancedFinancialReportDto> GetAdvancedAsync(
        Guid userId,
        int year,
        int month,
        DateOnly asOfDate,
        int trendMonths,
        int daysAhead,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var current = await GetPeriodTotalsAsync(userId, year, month, scope, cancellationToken);
        var previousPeriod = new DateOnly(year, month, 1).AddMonths(-1);
        var previous = await GetPeriodTotalsAsync(
            userId, previousPeriod.Year, previousPeriod.Month, scope, cancellationToken);
        var comparison = new PeriodComparisonDto(
            current,
            previous,
            current.Income - previous.Income,
            current.Expense - previous.Expense,
            current.Net - previous.Net);
        var trend = await GetCashFlowTrendAsync(
            userId, year, month, trendMonths, scope, cancellationToken);
        var budgetVariances = await GetBudgetVariancesAsync(
            userId, year, month, scope, cancellationToken);
        var accountDistribution = await GetAccountBalancesAsOfAsync(
            userId, asOfDate, cancellationToken);
        var cardDistribution = await GetCardDebtsAsOfAsync(
            userId, asOfDate, cancellationToken);
        var liquidAssets = accountDistribution.Sum(item => item.Balance);
        var cardDebt = cardDistribution.Sum(item => item.Debt);
        // Açık borç/alacak **kalan anaparadır**, kalan ödemelerin toplamı değil.
        //
        // Önce taksitin tamamı (anapara + faiz) toplanıyordu ve bu, bilançoyu
        // gelir tablosuyla çelişkiye düşürüyordu: faiz gider olarak
        // `RecognizedItems` içinde **ödendikçe** yazılıyor, ama net varlık
        // faizin tamamını borcun doğduğu gün düşüyordu. 1.000 anapara /
        // 1.200 toplam bir kredi çekildiğinde hesap 1.000 artıyor, borç 1.200
        // görünüyor ve net varlık daha ilk gün 200 azalıyordu — hiçbir faiz
        // tahakkuk etmemişken.
        //
        // Gelecekteki faiz henüz doğmamış bir yükümlülüktür; ödendiği ay hem
        // gider yazılır hem borcu azaltır. İki taraf ancak böyle tutuyor.
        //
        // Ayrım 12.8'den beri taksit üzerinde saklı. Yine de `?? Amount`
        // var: ayrımı olmayan bir kayıt sessizce sıfır sayılırsa borç
        // olduğundan küçük görünür — bilinmeyen bir ayrımda taksitin tamamını
        // anapara saymak, borcu yok saymaktan güvenlidir.
        var outstandingDebts = await dbContext.DebtInstallments.AsNoTracking()
            .Join(
                dbContext.DebtAgreements.AsNoTracking(),
                installment => new { installment.UserId, DebtId = installment.DebtAgreementId },
                debt => new { debt.UserId, DebtId = debt.Id },
                (installment, debt) => new { installment, debt })
            .Where(item => item.installment.UserId == userId &&
                           (item.installment.PaymentDate == null ||
                            item.installment.PaymentDate > asOfDate))
            .GroupBy(item => item.debt.Direction)
            .Select(group => new
            {
                Direction = group.Key,
                Amount = group.Sum(item =>
                    item.installment.PrincipalPortion ?? item.installment.Amount.Amount)
            })
            .ToDictionaryAsync(item => item.Direction, item => item.Amount, cancellationToken);
        // Açık cari de net varlığın parçasıdır: veresiye satılan mal artık
        // stokta değil, alacak olarak duruyor. Taksitli sözleşmeyle aynı
        // kovalara giriyor çünkü soruları aynı — ne alacağım var, ne
        // borcum. İki kaynak birbirini toplamaz: sözleşme kendi kalan
        // anaparasını, cari kendi hareketlerini sayar.
        var counterpartyBalances = await dbContext.Counterparties.AsNoTracking()
            .Where(counterparty => counterparty.UserId == userId)
            .Select(counterparty => new
            {
                Receivable =
                    (dbContext.CounterpartyCharges
                        .Where(charge => charge.UserId == userId &&
                                         charge.CounterpartyId == counterparty.Id &&
                                         !charge.IsCancelled &&
                                         charge.Direction == DebtDirection.Receivable &&
                                         charge.ChargeDate <= asOfDate)
                        .Sum(charge => (decimal?)charge.Amount.Amount) ?? 0m) -
                    (dbContext.CounterpartyPayments
                        .Where(payment => payment.UserId == userId &&
                                          payment.CounterpartyId == counterparty.Id &&
                                          !payment.IsCancelled &&
                                          payment.Direction == DebtDirection.Receivable &&
                                          payment.PaymentDate <= asOfDate)
                        .Sum(payment => (decimal?)payment.Amount.Amount) ?? 0m),
                Payable =
                    (dbContext.CounterpartyCharges
                        .Where(charge => charge.UserId == userId &&
                                         charge.CounterpartyId == counterparty.Id &&
                                         !charge.IsCancelled &&
                                         charge.Direction == DebtDirection.Payable &&
                                         charge.ChargeDate <= asOfDate)
                        .Sum(charge => (decimal?)charge.Amount.Amount) ?? 0m) -
                    (dbContext.CounterpartyPayments
                        .Where(payment => payment.UserId == userId &&
                                          payment.CounterpartyId == counterparty.Id &&
                                          !payment.IsCancelled &&
                                          payment.Direction == DebtDirection.Payable &&
                                          payment.PaymentDate <= asOfDate)
                        .Sum(payment => (decimal?)payment.Amount.Amount) ?? 0m)
            })
            .ToArrayAsync(cancellationToken);
        var obligationBalances = await dbContext.Obligations.AsNoTracking()
            .Where(obligation => obligation.UserId == userId &&
                                 !obligation.IsCancelled &&
                                 obligation.IssueDate <= asOfDate &&
                                 !dbContext.ObligationSettlements.Any(settlement =>
                                     settlement.UserId == userId &&
                                     settlement.ObligationId == obligation.Id &&
                                     !settlement.IsCancelled &&
                                     settlement.SettlementDate <= asOfDate))
            .GroupBy(obligation => obligation.Direction)
            .Select(group => new
            {
                Direction = group.Key,
                Amount = group.Sum(item => item.Amount.Amount)
            })
            .ToDictionaryAsync(item => item.Direction, item => item.Amount, cancellationToken);
        var receivableDebt = outstandingDebts.GetValueOrDefault(DebtDirection.Receivable) +
                             counterpartyBalances.Sum(item => item.Receivable) +
                             obligationBalances.GetValueOrDefault(DebtDirection.Receivable);
        var payableDebt = outstandingDebts.GetValueOrDefault(DebtDirection.Payable) +
                          counterpartyBalances.Sum(item => item.Payable) +
                          obligationBalances.GetValueOrDefault(DebtDirection.Payable);
        var futureLoad = await GetFutureLoadAsync(
            userId, asOfDate, daysAhead, cancellationToken);
        // Yoldaki para: tahsil edilmiş ama hesaba geçmemiş POS tutarlarının
        // **net** toplamı. Kalıcı kolon değil, bir hesap türü de değil
        // (ADR 0015); geçen ve iptal edilen satırlar düşer.
        var moneyInTransit = await dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.SettlementDate <= asOfDate &&
                                 (settlement.TransferredOn == null ||
                                  settlement.TransferredOn > asOfDate))
            .SumAsync(
                settlement => (decimal?)(settlement.GrossAmount.Amount -
                                         settlement.CommissionAmount),
                cancellationToken) ?? 0m;
        var nextTransitDate = await dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.SettlementDate <= asOfDate &&
                                 (settlement.TransferredOn == null ||
                                  settlement.TransferredOn > asOfDate))
            .MinAsync(settlement => (DateOnly?)settlement.ExpectedTransferDate, cancellationToken);
        // Fazla ödenmiş kartın negatif borcu kullanıcının alacağıdır ve varlık
        // tarafına geçer; iki taraf da burada toplanır, istemcide değil.
        var cardAsset = cardDebt < 0m ? -cardDebt : 0m;
        var cardLiability = cardDebt > 0m ? cardDebt : 0m;
        var totalAssets = liquidAssets + moneyInTransit + receivableDebt + cardAsset;
        var totalLiabilities = cardLiability + payableDebt;

        return new AdvancedFinancialReportDto(
            asOfDate,
            CurrencyCode.TRY,
            scope,
            // Yoldaki para net varlığa girer ama kullanılabilir bakiyeye
            // girmez (ADR 0015): kullanıcının parasıdır, bugün harcanamaz.
            // İki sayının farkı tam olarak bu tutardır.
            new NetWorthDto(
                liquidAssets,
                cardDebt,
                receivableDebt,
                payableDebt,
                liquidAssets + moneyInTransit - cardDebt + receivableDebt - payableDebt,
                moneyInTransit,
                totalAssets,
                totalLiabilities,
                nextTransitDate),
            comparison,
            trend,
            budgetVariances,
            futureLoad,
            accountDistribution,
            cardDistribution);
    }

    private async Task<PeriodTotalsDto> GetPeriodTotalsAsync(
        Guid userId,
        int year,
        int month,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var start = new DateOnly(year, month, 1);
        var totals = await RecognizedItems
            .InPeriod(dbContext, userId, start, start.AddMonths(1), scope)
            .GroupBy(item => item.Type)
            .Select(group => new { Type = group.Key, Amount = group.Sum(item => item.Amount) })
            .ToArrayAsync(cancellationToken);
        var income = totals
            .Where(row => row.Type == (int)TransactionType.Income)
            .Sum(row => row.Amount);
        var expense = totals
            .Where(row => row.Type == (int)TransactionType.Expense)
            .Sum(row => row.Amount);
        return new PeriodTotalsDto(year, month, income, expense, income - expense);
    }

    /// <summary>
    /// Son <paramref name="trendMonths"/> ayın gelir ve gideri.
    /// </summary>
    /// <remarks>
    /// Eğilim, dönem toplamı ve aylık raporla <b>aynı kalemleri</b> okur
    /// (<see cref="RecognizedItems"/>): aynı ay üç yerde aynı sayıyı verir.
    /// Aylara bölme veritabanında yapılır; dönen satır sayısı ay sayısının iki
    /// katını geçmez.
    /// </remarks>
    private async Task<IReadOnlyList<CashFlowPointDto>> GetCashFlowTrendAsync(
        Guid userId,
        int year,
        int month,
        int trendMonths,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var endExclusive = new DateOnly(year, month, 1).AddMonths(1);
        var start = endExclusive.AddMonths(-trendMonths);
        var totals = await RecognizedItems
            .InPeriod(dbContext, userId, start, endExclusive, scope)
            .GroupBy(item => new { item.Date.Year, item.Date.Month, item.Type })
            .Select(group => new
            {
                group.Key.Year,
                group.Key.Month,
                group.Key.Type,
                Amount = group.Sum(item => item.Amount)
            })
            .ToArrayAsync(cancellationToken);
        var points = new List<CashFlowPointDto>(trendMonths);

        for (var offset = 0; offset < trendMonths; offset++)
        {
            var period = start.AddMonths(offset);
            decimal Sum(TransactionType type) => totals
                .Where(row => row.Year == period.Year &&
                              row.Month == period.Month &&
                              row.Type == (int)type)
                .Sum(row => row.Amount);
            var income = Sum(TransactionType.Income);
            var expense = Sum(TransactionType.Expense);
            points.Add(new CashFlowPointDto(
                period.Year, period.Month, income, expense, income - expense));
        }

        return points;
    }

    /// <summary>
    /// Bütçe sapması. Her bütçe, raporda kendi kategorisinin kendi
    /// kapsamındaki gideriyle eşleşir.
    /// </summary>
    /// <remarks>
    /// Harcama kategoriyle değil <b>kategori + kapsam çiftiyle</b> okunur: aynı
    /// kategori hem işletme hem şahsi harcama tutabildiği için, ikisini birden
    /// saymak kullanıcının koymadığı bir sınırı aşılmış gösterirdi. Sayı
    /// Bütçeler listesiyle aynı yerden gelir
    /// (<see cref="RecognizedItems.ExpenseByCategoryAndScopeAsync"/>); iki
    /// ekran aynı bütçeye iki ayrı harcama gösteremez.
    /// <paramref name="scope"/> ayrıca listeyi daraltır: kapsam anahtarı
    /// işletmedeyken şahsi bütçeler görünmez.
    /// </remarks>
    private async Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVariancesAsync(
        Guid userId,
        int year,
        int month,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var start = new DateOnly(year, month, 1);
        var budgets = await (
                from budget in dbContext.MonthlyBudgets.AsNoTracking()
                join category in dbContext.Categories.AsNoTracking()
                    on new { budget.UserId, Id = budget.CategoryId }
                    equals new { category.UserId, category.Id }
                where budget.UserId == userId && budget.Year == year && budget.Month == month &&
                      (scope == null || budget.Scope == scope)
                orderby category.Name, category.Id
                select new
                {
                    budget.CategoryId,
                    CategoryName = category.Name,
                    budget.Scope,
                    Limit = budget.Limit.Amount
                })
            .ToArrayAsync(cancellationToken);
        var spentByCategory = await RecognizedItems.ExpenseByCategoryAndScopeAsync(
            dbContext, userId, start, start.AddMonths(1), cancellationToken);

        return budgets.Select(budget =>
        {
            var spent = spentByCategory.GetValueOrDefault((budget.CategoryId, budget.Scope));
            return new BudgetVarianceDto(
                budget.CategoryId,
                budget.CategoryName,
                budget.Limit,
                spent,
                budget.Limit - spent,
                spent > budget.Limit);
        }).ToArray();
    }

    private async Task<IReadOnlyList<AccountBalanceDto>> GetAccountBalancesAsOfAsync(
        Guid userId,
        DateOnly asOfDate,
        CancellationToken cancellationToken)
    {
        var accounts = await dbContext.Accounts.AsNoTracking()
            .Where(account => account.UserId == userId)
            .OrderBy(account => account.Name)
            .ThenBy(account => account.Id)
            .Select(account => new { account.Id, account.Name, account.OpeningBalance, account.Type })
            .ToArrayAsync(cancellationToken);
        var movements = await dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId &&
                                  !transaction.IsCancelled &&
                                  transaction.TransactionDate <= asOfDate)
            .GroupBy(transaction => transaction.AccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Balance = group.Sum(transaction => transaction.Type == TransactionType.Income
                    ? transaction.Amount.Amount
                    : -transaction.Amount.Amount)
            })
            .ToDictionaryAsync(item => item.AccountId, item => item.Balance, cancellationToken);
        var outgoing = await dbContext.Transfers.AsNoTracking()
            .Where(transfer => transfer.UserId == userId &&
                               !transfer.IsCancelled &&
                               transfer.TransferDate <= asOfDate)
            .GroupBy(transfer => transfer.SourceAccountId)
            .Select(group => new { AccountId = group.Key, Amount = group.Sum(item => item.Amount.Amount) })
            .ToDictionaryAsync(item => item.AccountId, item => item.Amount, cancellationToken);
        var incoming = await dbContext.Transfers.AsNoTracking()
            .Where(transfer => transfer.UserId == userId &&
                               !transfer.IsCancelled &&
                               transfer.TransferDate <= asOfDate)
            .GroupBy(transfer => transfer.DestinationAccountId)
            .Select(group => new { AccountId = group.Key, Amount = group.Sum(item => item.Amount.Amount) })
            .ToDictionaryAsync(item => item.AccountId, item => item.Amount, cancellationToken);
        var cardPayments = await dbContext.CreditCardPayments.AsNoTracking()
            .Where(payment => payment.UserId == userId &&
                              !payment.IsCancelled &&
                              payment.PaymentDate <= asOfDate)
            .GroupBy(payment => payment.AccountId)
            .Select(group => new { AccountId = group.Key, Amount = group.Sum(item => item.Amount.Amount) })
            .ToDictionaryAsync(item => item.AccountId, item => item.Amount, cancellationToken);
        var debtMovements = await (
                from installment in dbContext.DebtInstallments.AsNoTracking()
                join debt in dbContext.DebtAgreements.AsNoTracking()
                    on new { installment.UserId, DebtId = installment.DebtAgreementId }
                    equals new { debt.UserId, DebtId = debt.Id }
                where installment.UserId == userId &&
                      installment.PaymentAccountId != null &&
                      installment.PaymentDate <= asOfDate
                group new { installment, debt } by installment.PaymentAccountId into movementGroup
                select new
                {
                    AccountId = movementGroup.Key!.Value,
                    Amount = movementGroup.Sum(item => item.debt.Direction == DebtDirection.Receivable
                        ? item.installment.Amount.Amount
                        : -item.installment.Amount.Amount)
                })
            .ToDictionaryAsync(item => item.AccountId, item => item.Amount, cancellationToken);
        var debtOpenings = await DebtOpeningsByAccountAsync(userId, asOfDate, cancellationToken);
        var counterpartySettlements = await CounterpartySettlementsByAccountAsync(
            userId, asOfDate, cancellationToken);
        var obligationSettlements = await ObligationSettlementsByAccountAsync(
            userId, asOfDate, cancellationToken);
        var posTransfers = await PosTransfersByAccountAsync(
            userId, asOfDate, cancellationToken);

        return accounts.Select(account => new AccountBalanceDto(
            account.Id,
            account.Name,
            account.OpeningBalance +
            movements.GetValueOrDefault(account.Id) +
            incoming.GetValueOrDefault(account.Id) -
            outgoing.GetValueOrDefault(account.Id) -
            cardPayments.GetValueOrDefault(account.Id) +
            debtMovements.GetValueOrDefault(account.Id) +
            debtOpenings.GetValueOrDefault(account.Id) +
            counterpartySettlements.GetValueOrDefault(account.Id) +
            obligationSettlements.GetValueOrDefault(account.Id) +
            posTransfers.GetValueOrDefault(account.Id),
            account.Type)).ToArray();
    }

    /// <summary>
    /// POS tahsilatının hesap başına etkisi: yalnız <b>geçmiş</b> olanlar ve
    /// yalnız <b>net</b> tutar.
    /// </summary>
    /// <remarks>
    /// Tahsilat günü hesaba hiçbir şey girmez (ADR 0015): para henüz bankada
    /// değildir ve o gün eklemek, ulaşmamış parayı harcanabilir gösterirdi.
    /// Brüt eklemek de bankanın kestiği komisyonu kullanıcının cebinde
    /// sayardı; hesaba geçen tutar nettir.
    /// </remarks>
    private Task<Dictionary<Guid, decimal>> PosTransfersByAccountAsync(
        Guid userId,
        DateOnly? asOfDate,
        CancellationToken cancellationToken) =>
        dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.TransferredOn != null &&
                                 (asOfDate == null || settlement.TransferredOn <= asOfDate))
            .GroupBy(settlement => settlement.AccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Amount = group.Sum(settlement =>
                    settlement.GrossAmount.Amount - settlement.CommissionAmount)
            })
            .ToDictionaryAsync(item => item.AccountId, item => item.Amount, cancellationToken);

    /// <summary>
    /// Cari tahsilat/ödemenin hesap başına net etkisi: tahsilat artırır,
    /// ödeme azaltır.
    /// </summary>
    /// <remarks>
    /// Tahsilat parayı <b>taşır</b> (ADR 0014). Bakiyeye katılmasaydı
    /// tahsil edilen para kasada hiç görünmez, cari bakiye düşerken karşılığı
    /// hiçbir yere girmemiş olurdu. Gelir/gider tarafına ise hiç dokunmaz.
    /// </remarks>
    private Task<Dictionary<Guid, decimal>> CounterpartySettlementsByAccountAsync(
        Guid userId,
        DateOnly? asOfDate,
        CancellationToken cancellationToken) =>
        dbContext.CounterpartyPayments.AsNoTracking()
            // Kartla tahsil hesaba yatışla girer; burada sayılsaydı aynı para
            // iki kez girerdi (ADR 0019 T5).
            .Where(payment => payment.UserId == userId &&
                              !payment.IsCancelled &&
                              payment.PosSettlementId == null &&
                              (asOfDate == null || payment.PaymentDate <= asOfDate))
            .GroupBy(payment => payment.AccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Amount = group.Sum(payment =>
                    payment.Direction == DebtDirection.Receivable
                        ? payment.Amount.Amount
                        : -payment.Amount.Amount)
            })
            .ToDictionaryAsync(item => item.AccountId, item => item.Amount, cancellationToken);

    private Task<Dictionary<Guid, decimal>> ObligationSettlementsByAccountAsync(
        Guid userId,
        DateOnly? asOfDate,
        CancellationToken cancellationToken) =>
        dbContext.ObligationSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.PosSettlementId == null &&
                                 (asOfDate == null || settlement.SettlementDate <= asOfDate))
            .GroupBy(settlement => settlement.AccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Amount = group.Sum(settlement =>
                    settlement.Direction == DebtDirection.Receivable
                        ? settlement.Amount.Amount
                        : -settlement.Amount.Amount)
            })
            .ToDictionaryAsync(item => item.AccountId, item => item.Amount, cancellationToken);


    /// <summary>
    /// Nakit kaynaklı borçların açılış hareketi, hesap başına.
    /// </summary>
    /// <remarks>
    /// Borç alındığında para hesaba girer, alacak verildiğinde çıkar; ikisi de
    /// gelir/gider değildir. Bu hareket olmadan taksitler hesabı boşaltıyor
    /// ama karşılığında hiçbir şey girmemiş görünüyordu. Gider kaynaklı borç
    /// burada yer almaz: orada para değil tüketim vardır.
    /// </remarks>
    private Task<Dictionary<Guid, decimal>> DebtOpeningsByAccountAsync(
        Guid userId,
        DateOnly? asOfDate,
        CancellationToken cancellationToken) =>
        dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           debt.OpeningAccountId != null &&
                           debt.SourceType == DebtSourceType.Cash &&
                           (asOfDate == null || debt.StartDate <= asOfDate))
            .GroupBy(debt => debt.OpeningAccountId)
            .Select(group => new
            {
                AccountId = group.Key!.Value,
                Amount = group.Sum(debt => debt.Direction == DebtDirection.Payable
                    ? debt.Principal.Amount
                    : -debt.Principal.Amount)
            })
            .ToDictionaryAsync(item => item.AccountId, item => item.Amount, cancellationToken);

    private async Task<IReadOnlyList<CardDebtDto>> GetCardDebtsAsOfAsync(
        Guid userId,
        DateOnly asOfDate,
        CancellationToken cancellationToken)
    {
        var cards = await dbContext.CreditCards.AsNoTracking()
            .Where(card => card.UserId == userId)
            .OrderBy(card => card.Name)
            .ThenBy(card => card.Id)
            .Select(card => new
            {
                card.Id,
                card.Name,
                Limit = card.Limit.Amount
            })
            .ToArrayAsync(cancellationToken);
        var charges = await dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             charge.ChargeDate <= asOfDate)
            .GroupBy(charge => charge.CreditCardId)
            .Select(group => new { CardId = group.Key, Amount = group.Sum(item => item.Amount.Amount) })
            .ToDictionaryAsync(item => item.CardId, item => item.Amount, cancellationToken);
        var payments = await dbContext.CreditCardPayments.AsNoTracking()
            .Where(payment => payment.UserId == userId &&
                              !payment.IsCancelled &&
                              payment.PaymentDate <= asOfDate)
            .GroupBy(payment => payment.CreditCardId)
            .Select(group => new { CardId = group.Key, Amount = group.Sum(item => item.Amount.Amount) })
            .ToDictionaryAsync(item => item.CardId, item => item.Amount, cancellationToken);

        return cards.Select(card =>
        {
            // Kırpılmaz: negatif borç kartın alacaklı bakiyesidir ve net
            // varlığa girer. `- cardDebt` toplamında negatif bir borç varlığı
            // **artırır**; kullanıcının kartta duran parası da onun parasıdır.
            var debt = charges.GetValueOrDefault(card.Id) -
                       payments.GetValueOrDefault(card.Id);
            return new CardDebtDto(
                card.Id,
                card.Name,
                debt,
                Math.Max(0m, card.Limit - debt));
        }).ToArray();
    }

    private async Task<FutureLoadDto> GetFutureLoadAsync(
        Guid userId,
        DateOnly asOfDate,
        int daysAhead,
        CancellationToken cancellationToken)
    {
        var throughDate = asOfDate.AddDays(daysAhead);
        var candidates = await upcomingPaymentRepository.ListCandidatesAsync(
            userId, asOfDate, throughDate, cancellationToken);
        var future = candidates.Where(item => item.DueDate >= asOfDate).ToArray();
        var recurring = future
            .Where(item => item.SourceType == UpcomingPaymentSourceType.RecurringOccurrence)
            // Tutarı belli olmayan vergi tahminle sayılmaz (ADR 0018 İ5).
            .Sum(item => item.Amount ?? 0m);
        var statements = future
            .Where(item => item.SourceType == UpcomingPaymentSourceType.CreditCardStatement)
            .Sum(item => item.Amount ?? 0m);
        var installments = future
            .Where(item => item.SourceType == UpcomingPaymentSourceType.Installment)
            .Sum(item => item.Amount ?? 0m);
        var debtInstallments = future
            .Where(item => item.SourceType == UpcomingPaymentSourceType.DebtInstallment)
            .Sum(item => item.Amount ?? 0m);
        return new FutureLoadDto(
            asOfDate,
            throughDate,
            recurring,
            statements,
            installments,
            debtInstallments,
            recurring + statements + installments + debtInstallments);
    }
}
