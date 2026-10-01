using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Reports;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Categories;
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
        var periodTransactions = dbContext.Transactions.AsNoTracking().Where(
            transaction => transaction.UserId == userId &&
                           !transaction.IsCancelled &&
                           (scope == null || transaction.Scope == scope) &&
                           transaction.TransactionDate >= start &&
                           transaction.TransactionDate < endExclusive);

        // Toplamlar kapsam kırılımıyla birlikte okunuyor: `SUM` yerine kapsama
        // göre `GROUP BY`. Sorgu sayısı değişmiyor — en fazla iki satır dönüyor
        // ve toplam onların toplamı. Kırılımı ikinci bir tur sorguyla almak,
        // özet ekranının ilk isteğini iki katına çıkarırdı.
        var incomeByScope = ScopeAmounts.From(await periodTransactions
            .Where(transaction => transaction.Type == TransactionType.Income)
            .GroupBy(transaction => transaction.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(transaction => transaction.Amount.Amount)))
            .ToArrayAsync(cancellationToken));
        var transactionExpenseByScope = ScopeAmounts.From(await periodTransactions
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .GroupBy(transaction => transaction.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(transaction => transaction.Amount.Amount)))
            .ToArrayAsync(cancellationToken));
        var periodCardCharges = dbContext.CreditCardCharges.AsNoTracking().Where(
            charge => charge.UserId == userId &&
                      !charge.IsCancelled &&
                      (scope == null || charge.Scope == scope) &&
                      charge.ChargeDate >= start &&
                      charge.ChargeDate < endExclusive);
        var cardExpenseByScope = ScopeAmounts.From(await periodCardCharges
            .GroupBy(charge => charge.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(charge => charge.Amount.Amount)))
            .ToArrayAsync(cancellationToken));

        // Gider kaynaklı borcun açılışı bir giderdir ve tam o gün yazılır —
        // kredi kartı harcamasıyla birebir aynı kural. Taksit ödemeleri gider
        // üretmez; üretselerdi aynı tüketim iki kez sayılırdı.
        var periodDebtOpenings = dbContext.DebtAgreements.AsNoTracking().Where(
            debt => debt.UserId == userId &&
                    debt.SourceType == DebtSourceType.Expense &&
                    (scope == null || debt.Scope == scope) &&
                    debt.StartDate >= start &&
                    debt.StartDate < endExclusive);
        var debtOpeningExpenseByScope = ScopeAmounts.From(await periodDebtOpenings
            .GroupBy(debt => debt.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(debt => debt.Principal.Amount)))
            .ToArrayAsync(cancellationToken));
        var debtOpeningIncomeByScope = await DebtOpeningIncomeAsync(
            userId, start, endExclusive, scope, cancellationToken);
        var debtInterest = await DebtInterestAsync(
            userId, start, endExclusive, scope, cancellationToken);

        // Cari borçlandırma ekonomik olayı tanır: veresiye satış o gün
        // gelir, vadeli alım o gün giderdir (ADR 0014). Tahsilat buraya
        // hiç girmez — girseydi aynı satış iki kez sayılırdı.
        var periodCounterpartyCharges = dbContext.CounterpartyCharges.AsNoTracking().Where(
            charge => charge.UserId == userId &&
                      !charge.IsCancelled &&
                      (scope == null || charge.Scope == scope) &&
                      charge.ChargeDate >= start &&
                      charge.ChargeDate < endExclusive);
        var counterpartyIncomeByScope = ScopeAmounts.From(await periodCounterpartyCharges
            .Where(charge => charge.Direction == DebtDirection.Receivable)
            .GroupBy(charge => charge.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(charge => charge.Amount.Amount)))
            .ToArrayAsync(cancellationToken));
        var counterpartyExpenseByScope = ScopeAmounts.From(await periodCounterpartyCharges
            .Where(charge => charge.Direction == DebtDirection.Payable)
            .GroupBy(charge => charge.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(charge => charge.Amount.Amount)))
            .ToArrayAsync(cancellationToken));
        var periodObligations = dbContext.Obligations.AsNoTracking().Where(
            obligation => obligation.UserId == userId &&
                          !obligation.IsCancelled &&
                          (scope == null || obligation.Scope == scope) &&
                          obligation.IssueDate >= start &&
                          obligation.IssueDate < endExclusive);
        var obligationIncomeByScope = ScopeAmounts.From(await periodObligations
            .Where(obligation => obligation.Direction == DebtDirection.Receivable)
            .GroupBy(obligation => obligation.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(obligation => obligation.Amount.Amount)))
            .ToArrayAsync(cancellationToken));
        var obligationExpenseByScope = ScopeAmounts.From(await periodObligations
            .Where(obligation => obligation.Direction == DebtDirection.Payable)
            .GroupBy(obligation => obligation.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(obligation => obligation.Amount.Amount)))
            .ToArrayAsync(cancellationToken));
        // POS tahsilatı satışı **tahsil edildiği gün** tanır, geçtiği gün
        // değil (ADR 0015): gelir brüt tutar kadar, komisyon ayrı gider.
        // Geçiş günü rapora hiç girmez — girseydi aynı satış iki kez sayılırdı.
        var periodPosSettlements = dbContext.PosSettlements.AsNoTracking().Where(
            settlement => settlement.UserId == userId &&
                          !settlement.IsCancelled &&
                          (scope == null || settlement.Scope == scope) &&
                          settlement.SettlementDate >= start &&
                          settlement.SettlementDate < endExclusive);
        var posIncomeByScope = ScopeAmounts.From(await periodPosSettlements
            .GroupBy(settlement => settlement.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(settlement => settlement.GrossAmount.Amount)))
            .ToArrayAsync(cancellationToken));
        // Komisyon **brüte eklenmez ve ondan düşülmez**: kendi kategorisinde
        // ayrı bir giderdir. Netten hesaplansaydı kullanıcının kestiği fatura
        // küçülür, bankanın kesintisi de görünmez olurdu.
        var posCommissionByScope = ScopeAmounts.From(await periodPosSettlements
            .Where(settlement => settlement.CommissionAmount > 0m)
            .GroupBy(settlement => settlement.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(settlement => settlement.CommissionAmount)))
            .ToArrayAsync(cancellationToken));
        var expenseByScope = transactionExpenseByScope
            .Add(cardExpenseByScope)
            .Add(debtOpeningExpenseByScope)
            .Add(debtInterest.Paid)
            .Add(counterpartyExpenseByScope)
            .Add(obligationExpenseByScope)
            .Add(posCommissionByScope);
        incomeByScope = incomeByScope
            .Add(debtOpeningIncomeByScope)
            .Add(debtInterest.Earned)
            .Add(counterpartyIncomeByScope)
            .Add(obligationIncomeByScope)
            .Add(posIncomeByScope);
        var totalIncome = incomeByScope.Total;
        var totalExpense = expenseByScope.Total;
        var transactionCategoryExpenses = await (
                from transaction in periodTransactions
                join category in dbContext.Categories.AsNoTracking()
                    on new { transaction.UserId, Id = transaction.CategoryId }
                    equals new { category.UserId, category.Id }
                where transaction.Type == TransactionType.Expense
                group transaction by new { category.Id, category.Name }
                into expenseGroup
                orderby expenseGroup.Sum(item => item.Amount.Amount) descending,
                    expenseGroup.Key.Name,
                    expenseGroup.Key.Id
                select new CategoryExpenseDto(
                    expenseGroup.Key.Id,
                    expenseGroup.Key.Name,
                    expenseGroup.Sum(item => item.Amount.Amount)))
            .ToArrayAsync(cancellationToken);
        var cardCategoryExpenses = await (
                from charge in periodCardCharges
                join category in dbContext.Categories.AsNoTracking()
                    on new { charge.UserId, Id = charge.CategoryId }
                    equals new { category.UserId, category.Id }
                group charge by new { category.Id, category.Name }
                into expenseGroup
                select new CategoryExpenseDto(
                    expenseGroup.Key.Id,
                    expenseGroup.Key.Name,
                    expenseGroup.Sum(item => item.Amount.Amount)))
            .ToArrayAsync(cancellationToken);
        var debtCategoryExpenses = await (
                from debt in periodDebtOpenings
                join category in dbContext.Categories.AsNoTracking()
                    on new { debt.UserId, Id = debt.CategoryId!.Value }
                    equals new { category.UserId, category.Id }
                group debt by new { category.Id, category.Name }
                into expenseGroup
                select new CategoryExpenseDto(
                    expenseGroup.Key.Id,
                    expenseGroup.Key.Name,
                    expenseGroup.Sum(item => item.Principal.Amount)))
            .ToArrayAsync(cancellationToken);
        // Faiz gider **toplamına** zaten giriyordu ama dağılımda hiç yoktu:
        // ekranda "Gider" ile kategori listesi açıklamasız biçimde tutmuyordu.
        // Kalıcı bir hareket üretmiyoruz — taksit ödemesi hesabı tutarın
        // tamamı kadar düşürdüğü için ikinci bir kayıt aynı parayı iki kez
        // düşerdi — yalnız dağılımda kendi kovasına yazılıyor.
        var interestCategoryExpenses = await InterestCategoryExpensesAsync(
            userId, debtInterest.Paid.Total, cancellationToken);

        // Vadeli alım dağılımda da kendi kategorisinde durur; toplam ile
        // dağılımın birbirini tutması bunun koşulu.
        var counterpartyCategoryExpenses = await (
                from charge in periodCounterpartyCharges
                join category in dbContext.Categories.AsNoTracking()
                    on new { charge.UserId, Id = charge.CategoryId }
                    equals new { category.UserId, category.Id }
                where charge.Direction == DebtDirection.Payable
                group charge by new { category.Id, category.Name }
                into expenseGroup
                select new CategoryExpenseDto(
                    expenseGroup.Key.Id,
                    expenseGroup.Key.Name,
                    expenseGroup.Sum(item => item.Amount.Amount)))
            .ToArrayAsync(cancellationToken);
        var obligationCategoryExpenses = await (
                from obligation in periodObligations
                join category in dbContext.Categories.AsNoTracking()
                    on new { obligation.UserId, Id = obligation.CategoryId }
                    equals new { category.UserId, category.Id }
                where obligation.Direction == DebtDirection.Payable
                group obligation by new { category.Id, category.Name }
                into expenseGroup
                select new CategoryExpenseDto(
                    expenseGroup.Key.Id,
                    expenseGroup.Key.Name,
                    expenseGroup.Sum(item => item.Amount.Amount)))
            .ToArrayAsync(cancellationToken);
        var posCategoryExpenses = await (
                from settlement in periodPosSettlements
                join category in dbContext.Categories.AsNoTracking()
                    on new { settlement.UserId, Id = settlement.CommissionCategoryId }
                    equals new { category.UserId, Id = (Guid?)category.Id }
                group settlement by new { category.Id, category.Name }
                into expenseGroup
                select new CategoryExpenseDto(
                    expenseGroup.Key.Id,
                    expenseGroup.Key.Name,
                    expenseGroup.Sum(item => item.CommissionAmount)))
            .ToArrayAsync(cancellationToken);
        var categoryExpenses = transactionCategoryExpenses
            .Concat(cardCategoryExpenses)
            .Concat(debtCategoryExpenses)
            .Concat(interestCategoryExpenses)
            .Concat(counterpartyCategoryExpenses)
            .Concat(obligationCategoryExpenses)
            .Concat(posCategoryExpenses)
            .GroupBy(item => new { item.CategoryId, item.CategoryName })
            .Select(group => new CategoryExpenseDto(
                group.Key.CategoryId,
                group.Key.CategoryName,
                group.Sum(item => item.Amount)))
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
                        incomeByScope.Business,
                        expenseByScope.Business,
                        incomeByScope.Business - expenseByScope.Business),
                    new ScopeTotalsDto(
                        incomeByScope.Personal,
                        expenseByScope.Personal,
                        incomeByScope.Personal - expenseByScope.Personal))
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
        // `DebtInterestAsync` ile **ödendikçe** yazılıyor, ama net varlık
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
        var endExclusive = start.AddMonths(1);
        var transactions = dbContext.Transactions.AsNoTracking().Where(
            transaction => transaction.UserId == userId &&
                           !transaction.IsCancelled &&
                           (scope == null || transaction.Scope == scope) &&
                           transaction.TransactionDate >= start &&
                           transaction.TransactionDate < endExclusive);
        var income = await transactions
            .Where(transaction => transaction.Type == TransactionType.Income)
            .SumAsync(transaction => transaction.Amount.Amount, cancellationToken);
        var transactionExpense = await transactions
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .SumAsync(transaction => transaction.Amount.Amount, cancellationToken);
        var cardExpense = await dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             (scope == null || charge.Scope == scope) &&
                             charge.ChargeDate >= start &&
                             charge.ChargeDate < endExclusive)
            .SumAsync(charge => charge.Amount.Amount, cancellationToken);
        var debtOpeningExpense = await dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           debt.SourceType == DebtSourceType.Expense &&
                           (scope == null || debt.Scope == scope) &&
                           debt.StartDate >= start &&
                           debt.StartDate < endExclusive)
            .SumAsync(debt => debt.Principal.Amount, cancellationToken);
        var debtOpeningIncome = await DebtOpeningIncomeAsync(
            userId, start, endExclusive, scope, cancellationToken);
        var debtInterest = await DebtInterestAsync(
            userId, start, endExclusive, scope, cancellationToken);
        var counterpartyCharges = dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             (scope == null || charge.Scope == scope) &&
                             charge.ChargeDate >= start &&
                             charge.ChargeDate < endExclusive);
        var counterpartyExpense = await counterpartyCharges
            .Where(charge => charge.Direction == DebtDirection.Payable)
            .SumAsync(charge => charge.Amount.Amount, cancellationToken);
        var counterpartyIncome = await counterpartyCharges
            .Where(charge => charge.Direction == DebtDirection.Receivable)
            .SumAsync(charge => charge.Amount.Amount, cancellationToken);
        var obligations = dbContext.Obligations.AsNoTracking()
            .Where(obligation => obligation.UserId == userId &&
                                 !obligation.IsCancelled &&
                                 (scope == null || obligation.Scope == scope) &&
                                 obligation.IssueDate >= start &&
                                 obligation.IssueDate < endExclusive);
        var obligationExpense = await obligations
            .Where(obligation => obligation.Direction == DebtDirection.Payable)
            .SumAsync(obligation => obligation.Amount.Amount, cancellationToken);
        var obligationIncome = await obligations
            .Where(obligation => obligation.Direction == DebtDirection.Receivable)
            .SumAsync(obligation => obligation.Amount.Amount, cancellationToken);
        // POS satışı tahsil edildiği gün tanınır: gelir brüt, komisyon ayrı
        // gider. Geçiş günü hiçbir şey yazmaz (ADR 0015).
        var posSettlements = dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 (scope == null || settlement.Scope == scope) &&
                                 settlement.SettlementDate >= start &&
                                 settlement.SettlementDate < endExclusive);
        var posIncome = await posSettlements
            .SumAsync(settlement => (decimal?)settlement.GrossAmount.Amount, cancellationToken)
            ?? 0m;
        var posCommission = await posSettlements
            .SumAsync(settlement => (decimal?)settlement.CommissionAmount, cancellationToken)
            ?? 0m;
        var expense = transactionExpense + cardExpense + debtOpeningExpense +
                      debtInterest.Paid.Total + counterpartyExpense + obligationExpense +
                      posCommission;
        var totalIncome = income + debtOpeningIncome.Total + debtInterest.Earned.Total +
                          counterpartyIncome + obligationIncome + posIncome;
        return new PeriodTotalsDto(year, month, totalIncome, expense, totalIncome - expense);
    }

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
        var transactions = await dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId &&
                                  !transaction.IsCancelled &&
                                  (scope == null || transaction.Scope == scope) &&
                                  transaction.TransactionDate >= start &&
                                  transaction.TransactionDate < endExclusive)
            .Select(transaction => new
            {
                transaction.TransactionDate,
                transaction.Type,
                Amount = transaction.Amount.Amount
            })
            .ToArrayAsync(cancellationToken);
        var cardCharges = await dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             (scope == null || charge.Scope == scope) &&
                             charge.ChargeDate >= start &&
                             charge.ChargeDate < endExclusive)
            .Select(charge => new { charge.ChargeDate, Amount = charge.Amount.Amount })
            .ToArrayAsync(cancellationToken);
        // Gider ve gelir kaynaklı açılışlar tek sorguda; ay döngüsünde kaynağa
        // göre ayrılıyorlar.
        var debtOpenings = await dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           (debt.SourceType == DebtSourceType.Expense ||
                            debt.SourceType == DebtSourceType.Income) &&
                           debt.StartDate >= start &&
                           debt.StartDate < endExclusive)
            .Select(debt => new
            {
                debt.StartDate,
                debt.SourceType,
                Amount = debt.Principal.Amount
            })
            .ToArrayAsync(cancellationToken);
        var debtInterest = await (
                from installment in dbContext.DebtInstallments.AsNoTracking()
                join debt in dbContext.DebtAgreements.AsNoTracking()
                    on new { installment.UserId, DebtId = installment.DebtAgreementId }
                    equals new { debt.UserId, DebtId = debt.Id }
                where installment.UserId == userId &&
                      installment.InterestPortion != null &&
                      installment.PaymentDate >= start &&
                      installment.PaymentDate < endExclusive
                select new
                {
                    PaymentDate = installment.PaymentDate!.Value,
                    debt.Direction,
                    Amount = installment.InterestPortion!.Value
                })
            .ToArrayAsync(cancellationToken);
        var counterpartyCharges = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             (scope == null || charge.Scope == scope) &&
                             charge.ChargeDate >= start &&
                             charge.ChargeDate < endExclusive)
            .Select(charge => new
            {
                charge.ChargeDate,
                charge.Direction,
                Amount = charge.Amount.Amount
            })
            .ToArrayAsync(cancellationToken);
        var obligations = await dbContext.Obligations.AsNoTracking()
            .Where(obligation => obligation.UserId == userId &&
                                 !obligation.IsCancelled &&
                                 (scope == null || obligation.Scope == scope) &&
                                 obligation.IssueDate >= start &&
                                 obligation.IssueDate < endExclusive)
            .Select(obligation => new
            {
                obligation.IssueDate,
                obligation.Direction,
                Amount = obligation.Amount.Amount
            })
            .ToArrayAsync(cancellationToken);
        var posSettlements = await dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 (scope == null || settlement.Scope == scope) &&
                                 settlement.SettlementDate >= start &&
                                 settlement.SettlementDate < endExclusive)
            .Select(settlement => new
            {
                settlement.SettlementDate,
                Gross = settlement.GrossAmount.Amount,
                settlement.CommissionAmount
            })
            .ToArrayAsync(cancellationToken);
        var points = new List<CashFlowPointDto>(trendMonths);

        for (var offset = 0; offset < trendMonths; offset++)
        {
            var period = start.AddMonths(offset);
            var income = transactions
                .Where(item => item.TransactionDate.Year == period.Year &&
                               item.TransactionDate.Month == period.Month &&
                               item.Type == TransactionType.Income)
                .Sum(item => item.Amount) +
                debtOpenings
                    .Where(item => item.StartDate.Year == period.Year &&
                                   item.StartDate.Month == period.Month &&
                                   item.SourceType == DebtSourceType.Income)
                    .Sum(item => item.Amount) +
                debtInterest
                    .Where(item => item.PaymentDate.Year == period.Year &&
                                   item.PaymentDate.Month == period.Month &&
                                   item.Direction == DebtDirection.Receivable)
                    .Sum(item => item.Amount) +
                counterpartyCharges
                    .Where(item => item.ChargeDate.Year == period.Year &&
                                   item.ChargeDate.Month == period.Month &&
                                   item.Direction == DebtDirection.Receivable)
                    .Sum(item => item.Amount) +
                obligations
                    .Where(item => item.IssueDate.Year == period.Year &&
                                   item.IssueDate.Month == period.Month &&
                                   item.Direction == DebtDirection.Receivable)
                    .Sum(item => item.Amount) +
                posSettlements
                    .Where(item => item.SettlementDate.Year == period.Year &&
                                   item.SettlementDate.Month == period.Month)
                    .Sum(item => item.Gross);
            var expense = transactions
                .Where(item => item.TransactionDate.Year == period.Year &&
                               item.TransactionDate.Month == period.Month &&
                               item.Type == TransactionType.Expense)
                .Sum(item => item.Amount) +
                cardCharges
                    .Where(item => item.ChargeDate.Year == period.Year &&
                                   item.ChargeDate.Month == period.Month)
                    .Sum(item => item.Amount) +
                debtOpenings
                    .Where(item => item.StartDate.Year == period.Year &&
                                   item.StartDate.Month == period.Month &&
                                   item.SourceType == DebtSourceType.Expense)
                    .Sum(item => item.Amount) +
                debtInterest
                    .Where(item => item.PaymentDate.Year == period.Year &&
                                   item.PaymentDate.Month == period.Month &&
                                   item.Direction == DebtDirection.Payable)
                    .Sum(item => item.Amount) +
                counterpartyCharges
                    .Where(item => item.ChargeDate.Year == period.Year &&
                                   item.ChargeDate.Month == period.Month &&
                                   item.Direction == DebtDirection.Payable)
                    .Sum(item => item.Amount) +
                obligations
                    .Where(item => item.IssueDate.Year == period.Year &&
                                   item.IssueDate.Month == period.Month &&
                                   item.Direction == DebtDirection.Payable)
                    .Sum(item => item.Amount) +
                posSettlements
                    .Where(item => item.SettlementDate.Year == period.Year &&
                                   item.SettlementDate.Month == period.Month)
                    .Sum(item => item.CommissionAmount);
            points.Add(new CashFlowPointDto(
                period.Year, period.Month, income, expense, income - expense));
        }

        return points;
    }

    /// <summary>
    /// Bütçe sapması. Harcama kategoriyle değil <b>kategori + kapsam
    /// çiftiyle</b> toplanır ve her bütçe kendi kapsamıyla eşleşir.
    /// </summary>
    /// <remarks>
    /// Aynı kategori hem işletme hem şahsi harcama tutabildiği için, ikisini
    /// birden saymak kullanıcının koymadığı bir sınırı aşılmış gösterirdi.
    /// Aynı kural <c>MonthlyBudget.CalculateProgress</c> ve
    /// <c>EfBudgetRepository</c> içinde de yazılı.
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
        var endExclusive = start.AddMonths(1);
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
        var transactionSpent = await dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId &&
                                  !transaction.IsCancelled &&
                                  transaction.Type == TransactionType.Expense &&
                                  transaction.TransactionDate >= start &&
                                  transaction.TransactionDate < endExclusive)
            .GroupBy(transaction => new { transaction.CategoryId, transaction.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Spent = group.Sum(item => item.Amount.Amount)
            })
            .ToDictionaryAsync(item => (item.CategoryId, item.Scope), item => item.Spent, cancellationToken);
        var cardSpent = await dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             charge.ChargeDate >= start &&
                             charge.ChargeDate < endExclusive)
            .GroupBy(charge => new { charge.CategoryId, charge.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Spent = group.Sum(item => item.Amount.Amount)
            })
            .ToDictionaryAsync(item => (item.CategoryId, item.Scope), item => item.Spent, cancellationToken);
        var debtSpent = await dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           debt.SourceType == DebtSourceType.Expense &&
                           debt.StartDate >= start &&
                           debt.StartDate < endExclusive)
            .GroupBy(debt => new { CategoryId = debt.CategoryId!.Value, debt.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Spent = group.Sum(item => item.Principal.Amount)
            })
            .ToDictionaryAsync(item => (item.CategoryId, item.Scope), item => item.Spent, cancellationToken);

        // Vadeli alım da bütçeyi tüketir: tüketim gerçek, kategorili ve o
        // gün tanınmış. Tahsilat tüketmez — tüketseydi aynı alım bütçeden
        // iki kez düşerdi.
        var counterpartySpent = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             charge.Direction == DebtDirection.Payable &&
                             charge.ChargeDate >= start &&
                             charge.ChargeDate < endExclusive)
            .GroupBy(charge => new { charge.CategoryId, charge.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Spent = group.Sum(item => item.Amount.Amount)
            })
            .ToDictionaryAsync(item => (item.CategoryId, item.Scope), item => item.Spent, cancellationToken);
        var obligationSpent = await dbContext.Obligations.AsNoTracking()
            .Where(obligation => obligation.UserId == userId &&
                                 !obligation.IsCancelled &&
                                 obligation.Direction == DebtDirection.Payable &&
                                 obligation.IssueDate >= start &&
                                 obligation.IssueDate < endExclusive)
            .GroupBy(obligation => new { obligation.CategoryId, obligation.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Spent = group.Sum(item => item.Amount.Amount)
            })
            .ToDictionaryAsync(
                item => (item.CategoryId, item.Scope),
                item => item.Spent,
                cancellationToken);

        // POS **komisyonu** bütçeyi tüketir: kendi kategorisi olan, o gün
        // tanınmış gerçek bir gider. Satışın brüt tutarı tüketmez — o bir
        // gelirdir ve bütçe gider bütçesidir.
        var posCommissionSpent = await dbContext.PosSettlements.AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.CommissionCategoryId != null &&
                                 settlement.SettlementDate >= start &&
                                 settlement.SettlementDate < endExclusive)
            .GroupBy(settlement => new
            {
                CategoryId = settlement.CommissionCategoryId!.Value,
                settlement.Scope
            })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                Spent = group.Sum(item => item.CommissionAmount)
            })
            .ToDictionaryAsync(
                item => (item.CategoryId, item.Scope),
                item => item.Spent,
                cancellationToken);

        return budgets.Select(budget =>
        {
            var key = (budget.CategoryId, budget.Scope);
            var spent = transactionSpent.GetValueOrDefault(key) +
                        cardSpent.GetValueOrDefault(key) +
                        debtSpent.GetValueOrDefault(key) +
                        counterpartySpent.GetValueOrDefault(key) +
                        obligationSpent.GetValueOrDefault(key) +
                        posCommissionSpent.GetValueOrDefault(key);
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
            .Where(payment => payment.UserId == userId &&
                              !payment.IsCancelled &&
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
    /// Gelir kaynaklı alacakların açılış geliri.
    /// </summary>
    /// <remarks>
    /// Gider kaynaklı borcun aynadaki hâli: bir şey satıldı, bedeli sonra
    /// alınacak. Gelir satış anında yazılır; tahsilatlar yalnız bakiyeyi
    /// hareket ettirir. Tahsilat da gelir yazsaydı aynı satış iki kez
    /// sayılırdı.
    /// </remarks>
    private async Task<ScopeAmounts> DebtOpeningIncomeAsync(
        Guid userId,
        DateOnly start,
        DateOnly endExclusive,
        TransactionScope? scope,
        CancellationToken cancellationToken) =>
        ScopeAmounts.From(await dbContext.DebtAgreements.AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           debt.SourceType == DebtSourceType.Income &&
                           (scope == null || debt.Scope == scope) &&
                           debt.StartDate >= start &&
                           debt.StartDate < endExclusive)
            .GroupBy(debt => debt.Scope)
            .Select(group => new ScopeAmountRow(
                group.Key,
                group.Sum(debt => debt.Principal.Amount)))
            .ToArrayAsync(cancellationToken));

    /// <summary>
    /// Bir dönemde ödenen taksitlerin faiz payı: borçta gider, alacakta gelir.
    /// </summary>
    /// <remarks>
    /// Borcun gerçek maliyeti faizdir. Anapara geri ödemesi gider değildir —
    /// borç azalır, para azalır, servet değişmez — ama faiz karşılığında
    /// hiçbir şey alınmaz, o yüzden gerçek bir giderdir. Alacakta aynı tutar
    /// gelirdir.
    ///
    /// Ayrımı olmayan taksitler (bu ayrımdan önce oluşmuş, açılışı da kayıtsız
    /// olan borçlar) sıfır faiz katar. Uydurma bir faiz yazmaktansa hiç
    /// yazmamak doğrudur.
    /// </remarks>
    /// <summary>
    /// Ödenen borç faizinin kategori dağılımındaki satırı.
    /// </summary>
    /// <remarks>
    /// Kategori kullanıcının kendi kayıtlarından, kanonik adla bulunuyor;
    /// pasifleştirilmiş olsa da bulunur (silme yerine pasifleştirme kuralı).
    /// Kullanıcı kategoriyi yeniden adlandırdıysa eşleşme olmaz ve faiz
    /// dağılımda görünmez — toplamdaki payı yine doğrudur, yalnız kovasız
    /// kalır. Uydurma bir kimlikle satır üretmek daha kötü olurdu: istemci o
    /// kimlikle filtreleyip boş sonuç alırdı.
    /// </remarks>
    private async Task<CategoryExpenseDto[]> InterestCategoryExpensesAsync(
        Guid userId,
        decimal interestPaid,
        CancellationToken cancellationToken)
    {
        if (interestPaid <= 0m) return [];
        var category = await dbContext.Categories.AsNoTracking()
            .Where(item => item.UserId == userId &&
                           item.Type == CategoryType.Expense &&
                           item.Name == EfCategoryRepository.InterestExpenseCategoryName)
            .Select(item => new { item.Id, item.Name })
            .FirstOrDefaultAsync(cancellationToken);
        return category is null
            ? []
            : [new CategoryExpenseDto(category.Id, category.Name, interestPaid)];
    }

    private async Task<(ScopeAmounts Paid, ScopeAmounts Earned)> DebtInterestAsync(
        Guid userId,
        DateOnly start,
        DateOnly endExclusive,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var totals = await (
                from installment in dbContext.DebtInstallments.AsNoTracking()
                join debt in dbContext.DebtAgreements.AsNoTracking()
                    on new { installment.UserId, DebtId = installment.DebtAgreementId }
                    equals new { debt.UserId, DebtId = debt.Id }
                where installment.UserId == userId &&
                      installment.InterestPortion != null &&
                      (scope == null || debt.Scope == scope) &&
                      installment.PaymentDate >= start &&
                      installment.PaymentDate < endExclusive
                group installment by new { debt.Direction, debt.Scope } into directionGroup
                select new
                {
                    directionGroup.Key.Direction,
                    directionGroup.Key.Scope,
                    Amount = directionGroup.Sum(item => item.InterestPortion!.Value)
                })
            .ToArrayAsync(cancellationToken);

        return (
            ScopeAmounts.From(totals
                .Where(item => item.Direction == DebtDirection.Payable)
                .Select(item => new ScopeAmountRow(item.Scope, item.Amount))),
            ScopeAmounts.From(totals
                .Where(item => item.Direction == DebtDirection.Receivable)
                .Select(item => new ScopeAmountRow(item.Scope, item.Amount))));
    }

    /// <summary>
    /// Tek bir kapsam satırı: gruplanmış sorguların projeksiyon tipi.
    /// </summary>
    private sealed record ScopeAmountRow(TransactionScope Scope, decimal Amount);

    /// <summary>
    /// Bir tutarın iki kapsama dağılmış hâli.
    /// </summary>
    /// <remarks>
    /// Üçüncü bir kova yok: gelir/gider üreten her kayıt tam olarak bir kapsam
    /// taşır, bu yüzden <see cref="Total"/> ikisinin toplamıdır ve filtresiz
    /// okumanın toplamıyla birebir aynıdır.
    /// </remarks>
    private readonly record struct ScopeAmounts(decimal Business, decimal Personal)
    {
        internal decimal Total => Business + Personal;

        internal ScopeAmounts Add(ScopeAmounts other) =>
            new(Business + other.Business, Personal + other.Personal);

        internal static ScopeAmounts From(IEnumerable<ScopeAmountRow> rows)
        {
            var business = 0m;
            var personal = 0m;
            foreach (var row in rows)
            {
                if (row.Scope == TransactionScope.Business)
                {
                    business += row.Amount;
                }
                else
                {
                    personal += row.Amount;
                }
            }
            return new ScopeAmounts(business, personal);
        }
    }

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
