using BusinessFinance.Application.FinancePages;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Accounts;
using BusinessFinance.Infrastructure.FinancialActivities;
using BusinessFinance.Infrastructure.Persistence;
using BusinessFinance.Infrastructure.Reports;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.FinancePages;

internal sealed class EfFinancePageReader(BusinessFinanceDbContext db,
    EfFinancialActivityRepository activities, TimeProvider clock) : IFinancePageReader
{
    private const int PageSize = 50;
    private DateOnly Today => LocalDay.Today(clock.GetUtcNow());

    private IQueryable<RecognizedItem> Items(Guid owner, CategoryPageQuery q, Guid? category, bool summary,
        bool source = true, bool person = true, bool scope = true)
    {
        var start = new DateOnly(q.Year, q.Month, 1);
        var query = RecognizedItems.InPeriod(db, owner, start, start.AddMonths(1), scope ? q.Scope : null)
            .Where(item => item.Type == (int)q.Type);
        if (!summary) query = query.Where(item => item.CategoryId == category);
        return FilterItems(query, source ? q.Source : null, person ? q.Person : null);
    }

    private static IQueryable<RecognizedItem> FilterItems(IQueryable<RecognizedItem> items, string? source, string? person)
    {
        if (source == "deferred") items = items.Where(i => i.AccountId == null && i.CreditCardId == null);
        else if (source?.StartsWith("account:", StringComparison.Ordinal) == true && Guid.TryParse(source[8..], out var account))
            items = items.Where(i => i.AccountId == account);
        else if (source?.StartsWith("card:", StringComparison.Ordinal) == true && Guid.TryParse(source[5..], out var card))
            items = items.Where(i => i.CreditCardId == card);
        if (person == "none") items = items.Where(i => i.CounterpartyId == null);
        else if (person?.StartsWith("person:", StringComparison.Ordinal) == true && Guid.TryParse(person[7..], out var id))
            items = items.Where(i => i.CounterpartyId == id);
        return items;
    }

    public async Task<CategorySummary> CategorySummaryAsync(Guid userId, CategoryPageQuery query, CancellationToken ct) =>
        await SummaryAsync(userId, query, null, true, ct);

    private async Task<CategorySummary> SummaryAsync(Guid owner, CategoryPageQuery q, Guid? category, bool summary, CancellationToken ct)
    {
        var rows = await Items(owner, q, category, summary).GroupBy(i => i.CategoryId)
            .Select(g => new { Id = g.Key, Amount = g.Sum(i => i.Amount) }).ToArrayAsync(ct);
        var names = await db.Categories.AsNoTracking().Where(c => c.UserId == owner)
            .ToDictionaryAsync(c => c.Id, c => c.Name, ct);
        var total = rows.Sum(r => r.Amount);
        var prev = new DateOnly(q.Year, q.Month, 1).AddMonths(-1);
        var previous = await Items(owner, q with { Year = prev.Year, Month = prev.Month }, category, summary)
            .SumAsync(i => i.Amount, ct);
        var slices = rows.OrderByDescending(r => r.Amount).ThenBy(r => r.Id)
            .Select(r => new CategorySlice(r.Id, r.Id is Guid id ? names.GetValueOrDefault(id) : null,
                r.Amount, total == 0m ? 0m : decimal.Round(r.Amount / total * 100m, 0, MidpointRounding.AwayFromZero),
                total == 0m ? 0m : r.Amount / total)).ToArray();
        var options = await SourcesAsync(owner, Items(owner, q, category, summary, source: false), q.Source, ct);
        return new(q.Year, q.Month, q.Type == TransactionType.Income ? "income" : "expense",
            q.Scope?.ToString().ToLowerInvariant(), total, previous, rows.Count(r => r.Id != null), slices, options);
    }

    private async Task<IReadOnlyList<PageOption>> SourcesAsync(Guid owner, IQueryable<RecognizedItem> items, string? selected, CancellationToken ct)
    {
        var sums = await items.GroupBy(i => new { i.AccountId, i.CreditCardId })
            .Select(g => new { g.Key.AccountId, g.Key.CreditCardId, Amount = g.Sum(i => i.Amount) }).ToArrayAsync(ct);
        var accounts = await db.Accounts.AsNoTracking().Where(a => a.UserId == owner)
            .Select(a => new { a.Id, a.Name }).ToDictionaryAsync(a => a.Id, ct);
        var cards = await db.CreditCards.AsNoTracking().Where(a => a.UserId == owner)
            .Select(a => new { a.Id, a.Name }).ToDictionaryAsync(a => a.Id, ct);
        var result = new List<PageOption> { new("all", "all", null, sums.Sum(s => s.Amount)) };
        foreach (var row in sums)
        {
            if (row.AccountId is Guid account && accounts.TryGetValue(account, out var a))
                result.Add(new($"account:{account:D}", "accounts", a.Name, row.Amount, Id: account));
            else if (row.CreditCardId is Guid card && cards.TryGetValue(card, out var c))
                result.Add(new($"card:{card:D}", "cards", c.Name, row.Amount, Id: card));
            else result.Add(new("deferred", "deferred", null, row.Amount));
        }
        if (selected is not null && result.All(r => r.Key != selected))
        {
            if (selected == "deferred") result.Add(new(selected, "deferred", null, 0m));
            else if (selected.StartsWith("account:", StringComparison.Ordinal) && Guid.TryParse(selected[8..], out var id) && accounts.TryGetValue(id, out var account))
                result.Add(new(selected, "accounts", account.Name, 0m, Id: id));
            else if (selected.StartsWith("card:", StringComparison.Ordinal) && Guid.TryParse(selected[5..], out id) && cards.TryGetValue(id, out var card))
                result.Add(new(selected, "cards", card.Name, 0m, Id: id));
        }
        return result.OrderBy(r => r.Group switch { "all" => 0, "accounts" => 1, "cards" => 2, _ => 3 })
            .ThenBy(r => r.Name).ThenBy(r => r.Key).ToArray();
    }

    public async Task<CategoryPage?> CategoryAsync(Guid owner, Guid? categoryId, CategoryPageQuery q, CancellationToken ct)
    {
        var category = categoryId is Guid id
            ? await db.Categories.AsNoTracking().SingleOrDefaultAsync(c => c.UserId == owner && c.Id == id, ct) : null;
        if (categoryId is not null && category is null) return null;
        var summary = await SummaryAsync(owner, q, categoryId, false, ct);
        var whole = Items(owner, q, categoryId, false, source: false, person: false, scope: false);
        var wholeTotal = await whole.SumAsync(i => i.Amount, ct);
        var scopes = await whole.Select(i => i.Scope).Distinct().ToArrayAsync(ct);
        var hasPeople = await whole.AnyAsync(i => i.CounterpartyId != null, ct);
        var personSums = await Items(owner, q, categoryId, false, person: false).GroupBy(i => i.CounterpartyId)
            .Select(g => new { Id = g.Key, Amount = g.Sum(i => i.Amount) }).ToArrayAsync(ct);
        var peopleNames = await db.Counterparties.AsNoTracking().Where(p => p.UserId == owner)
            .ToDictionaryAsync(p => p.Id, p => p.Name, ct);
        var people = new List<PageOption> { new("all", "all", null, personSums.Sum(p => p.Amount)) };
        people.AddRange(personSums.Select(p => new PageOption(p.Id is Guid pid ? $"person:{pid:D}" : "none", "people",
            p.Id is Guid personId ? peopleNames.GetValueOrDefault(personId) : null, p.Amount, Id: p.Id)));
        if (q.Person == "none" && people.All(p => p.Key != "none")) people.Add(new("none", "people", null, 0m));
        else if (q.Person?.StartsWith("person:", StringComparison.Ordinal) == true && Guid.TryParse(q.Person[7..], out var selected) &&
            peopleNames.TryGetValue(selected, out var selectedName) && people.All(p => p.Id != selected))
            people.Add(new(q.Person, "people", selectedName, 0m, Id: selected));
        var query = Items(owner, q, categoryId, false);
        var count = await query.CountAsync(ct);
        var items = await query.OrderByDescending(i => i.Date).ThenByDescending(i => i.EntryAtUtc)
            .ThenBy(i => i.SourceKind).ThenBy(i => i.SourceId).Skip(q.Offset).Take(PageSize).ToArrayAsync(ct);
        var rows = await activities.ReadPageRowsAsync(owner, items.Select(i => i.ParentId ?? i.SourceId).Distinct().ToArray(), ct);
        var rowMap = rows.ToDictionary(r => (r.ActivityKind, r.ActivityId));
        var obligationIds = items.Where(i => i.SourceKind == (int)FinancialActivityKind.Obligation).Select(i => i.SourceId).ToArray();
        var payments = await (from s in db.ObligationSettlements.AsNoTracking()
                              join a in db.Accounts.AsNoTracking() on new { s.UserId, s.AccountId } equals new { a.UserId, AccountId = a.Id }
                              where s.UserId == owner && !s.IsCancelled && obligationIds.Contains(s.ObligationId)
                              select new { s.ObligationId, s.SettlementDate, a.Name }).ToDictionaryAsync(s => s.ObligationId, ct);
        var page = items.Select(i =>
        {
            var row = rowMap[((FinancialActivityKind)(i.ParentKind ?? i.SourceKind), i.ParentId ?? i.SourceId)];
            var paid = payments.GetValueOrDefault(i.SourceId);
            var state = i.SourceKind == (int)FinancialActivityKind.Obligation ? paid is null ? "open" : "settled" :
                i.SourceKind == (int)FinancialActivityKind.PosSale && i.Part is null ? row.TransferredOn is null ? "transit" : "deposited" : null;
            return new PageActivity(row, i.Type == (int)TransactionType.Expense ? -i.Amount : i.Amount, i.Part,
                i.Part is not null ? i.Amount : null, i.ParentAmount, state, paid?.SettlementDate, paid?.Name,
                q.Scope is null && scopes.Length > 1, Scope: ((TransactionScope)i.Scope).ToString().ToLowerInvariant());
        }).ToArray();
        return new(summary, categoryId, category?.Name, wholeTotal, scopes.Length > 1, hasPeople, people,
            page, count, q.Offset, q.Offset + page.Length < count);
    }

    public async Task<YearMonths?> CategoryMonthsAsync(Guid owner, Guid? categoryId, bool summary, CategoryPageQuery q, CancellationToken ct)
    {
        if (!summary && categoryId is Guid id && !await db.Categories.AnyAsync(c => c.UserId == owner && c.Id == id, ct)) return null;
        var start = new DateOnly(q.Year, 1, 1);
        var query = RecognizedItems.InPeriod(db, owner, start, start.AddYears(1), q.Scope).Where(i => i.Type == (int)q.Type);
        if (!summary) query = query.Where(i => i.CategoryId == categoryId);
        var sums = await FilterItems(query, q.Source, q.Person).GroupBy(i => i.Date.Month)
            .Select(g => new { Month = g.Key, Amount = g.Sum(i => i.Amount), Count = g.Count() }).ToArrayAsync(ct);
        var max = sums.Select(s => s.Amount).DefaultIfEmpty(0m).Max();
        return new(q.Year, Enumerable.Range(1, 12).Select(month =>
        {
            var sum = sums.SingleOrDefault(s => s.Month == month);
            return new MonthValue(month, sum?.Amount ?? 0m, max == 0m ? 0m : (sum?.Amount ?? 0m) / max,
                new DateOnly(q.Year, month, 1) <= Today || sum?.Count > 0);
        }).ToArray());
    }

    private static IQueryable<AccountMovement> FilterMovements(IQueryable<AccountMovement> query, AccountPageQuery q)
    {
        if (q.Direction == "in") query = query.Where(m => m.Amount > 0m);
        if (q.Direction == "out") query = query.Where(m => m.Amount < 0m);
        if (q.Selection is not null && q.Selection != "all")
        {
            var parts = q.Selection.Split(':');
            if (parts[0] is "in" or "out")
            {
                query = parts[0] == "in" ? query.Where(m => m.Amount > 0m) : query.Where(m => m.Amount < 0m);
                var group = parts[1] switch { "category" => 1, "transfer" => 3, _ => 2 };
                query = query.Where(m => m.Group == group);
                if (parts.Length == 3 && Guid.TryParse(parts[2], out var id))
                    query = group == 1 ? query.Where(m => m.CategoryId == id) : query.Where(m => m.CounterAccountId == id);
                else if (parts.Length == 3 && int.TryParse(parts[2], out var kind)) query = query.Where(m => m.ActivityKind == kind);
            }
        }
        return query;
    }

    public async Task<AccountPage?> AccountAsync(Guid owner, Guid accountId, AccountPageQuery q, CancellationToken ct)
    {
        var account = await db.Accounts.AsNoTracking().SingleOrDefaultAsync(a => a.UserId == owner && a.Id == accountId, ct);
        if (account is null) return null;
        var start = new DateOnly(q.Year, q.Month, 1); var end = start.AddMonths(1);
        var all = AccountMovements.Query(db, owner).Where(m => m.AccountId == accountId);
        var currentBalance = account.OpeningBalance + await all.SumAsync(m => m.Amount, ct);
        var currentMonth = start.Year == Today.Year && start.Month == Today.Month;
        var balance = currentMonth ? currentBalance : account.OpeningBalance + await all.Where(m => m.Date < end).SumAsync(m => m.Amount, ct);
        var month = all.Where(m => m.Date >= start && m.Date < end);
        var sums = await month.GroupBy(m => new { Incoming = m.Amount > 0m, m.Group, m.CategoryId, m.CounterAccountId, m.ActivityKind })
            .Select(g => new { g.Key.Incoming, g.Key.Group, g.Key.CategoryId, g.Key.CounterAccountId, g.Key.ActivityKind, Amount = g.Sum(m => m.Amount) }).ToArrayAsync(ct);
        var inflow = sums.Where(s => s.Incoming).Sum(s => s.Amount); var outflow = -sums.Where(s => !s.Incoming).Sum(s => s.Amount);
        var categoryNames = await db.Categories.AsNoTracking().Where(c => c.UserId == owner).ToDictionaryAsync(c => c.Id, c => c.Name, ct);
        var accountNames = await db.Accounts.AsNoTracking().Where(a => a.UserId == owner).ToDictionaryAsync(a => a.Id, a => a.Name, ct);
        var options = new List<PageOption> { new("all", "all", null, q.Direction == "in" ? inflow : q.Direction == "out" ? -outflow : inflow + outflow) };
        var visible = sums.Where(s => q.Direction is null || (q.Direction == "in") == s.Incoming);
        if (q.Direction is null)
            options.AddRange(visible.GroupBy(s => new { s.Incoming, s.Group }).Select(g => new PageOption(
                $"{(g.Key.Incoming ? "in" : "out")}:{GroupName(g.Key.Group, g.Key.Incoming)}", g.Key.Incoming ? "in" : "out", null,
                g.Sum(s => s.Amount), GroupName(g.Key.Group, g.Key.Incoming))));
        else
            options.AddRange(visible.GroupBy(s => new
            {
                s.Group,
                Id = s.Group == 1 ? s.CategoryId : s.CounterAccountId,
                Kind = s.Group == 2 ? s.ActivityKind : 0
            }).Select(g => new PageOption(
                $"{q.Direction}:{GroupName(g.Key.Group, q.Direction == "in")}:{(g.Key.Id is Guid id ? id.ToString("D") : g.Key.Kind.ToString())}",
                GroupName(g.Key.Group, q.Direction == "in"), g.Key.Id is Guid key ? (g.Key.Group == 1 ? categoryNames.GetValueOrDefault(key) : accountNames.GetValueOrDefault(key)) : null,
                g.Sum(s => s.Amount), g.Key.Kind > 0 ? KindValue((FinancialActivityKind)g.Key.Kind) : null, g.Key.Id)));
        var filtered = FilterMovements(month, q);
        var count = await filtered.CountAsync(ct);
        var selectedTotal = await filtered.SumAsync(m => m.Amount < 0m ? -m.Amount : m.Amount, ct);
        var items = await filtered.OrderByDescending(m => m.Date).ThenByDescending(m => m.EntryAtUtc)
            .ThenBy(m => m.ActivityKind).ThenBy(m => m.ActivityId).Skip(q.Offset).Take(PageSize).ToArrayAsync(ct);
        var rows = await activities.ReadPageRowsAsync(owner, items.Select(i => i.ActivityId).Distinct().ToArray(), ct);
        var map = rows.ToDictionary(r => (r.ActivityKind, r.ActivityId));
        var depositIds = items.Where(i => i.ActivityKind == (int)FinancialActivityKind.PosDeposit).Select(i => i.ActivityId).ToArray();
        var dates = await db.PosSettlements.AsNoTracking().Where(s => s.UserId == owner && s.PosDepositId != null && depositIds.Contains(s.PosDepositId.Value))
            .GroupBy(s => s.PosDepositId).Select(g => new { Id = g.Key, First = g.Min(s => s.SettlementDate), Last = g.Max(s => s.SettlementDate) }).ToDictionaryAsync(s => s.Id!.Value, ct);
        var page = items.Select(i => new PageActivity(map[((FinancialActivityKind)i.ActivityKind, i.ActivityId)], i.Amount,
            i.Part, i.PartAmount, FirstSaleDate: dates.GetValueOrDefault(i.ActivityId)?.First, LastSaleDate: dates.GetValueOrDefault(i.ActivityId)?.Last)).ToArray();
        var transit = await ReadTransitAsync(owner, accountId, account.Name, 0, 1, ct);
        return new(account.Id, account.Name, account.Type.ToString().ToLowerInvariant(), account.DefaultScope?.ToString().ToLowerInvariant(),
            account.IsActive, q.Year, q.Month, balance, currentBalance, currentMonth, inflow, outflow, inflow - outflow,
            selectedTotal, options, new(transit!.Total, transit.TotalCount, transit.Items.FirstOrDefault()), page, count, q.Offset, q.Offset + page.Length < count);
    }

    private static string GroupName(int group, bool incoming) => group switch { 1 => "category", 3 => "transfer", _ => incoming ? "collection" : "payment" };
    internal static string KindValue(FinancialActivityKind kind) => kind switch
    {
        FinancialActivityKind.AccountTransaction => "account-transaction",
        FinancialActivityKind.Transfer => "transfer",
        FinancialActivityKind.CardCharge => "card-charge",
        FinancialActivityKind.CardPayment => "card-payment",
        FinancialActivityKind.DebtPayment => "debt-payment",
        FinancialActivityKind.DebtCollection => "debt-collection",
        FinancialActivityKind.DebtOpening => "debt-opening",
        FinancialActivityKind.CounterpartyCharge => "counterparty-charge",
        FinancialActivityKind.CounterpartySettlement => "counterparty-settlement",
        FinancialActivityKind.Obligation => "obligation",
        FinancialActivityKind.ObligationSettlement => "obligation-settlement",
        FinancialActivityKind.PosSale => "pos-sale",
        FinancialActivityKind.PosDeposit => "pos-deposit",
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    public async Task<YearMonths?> AccountMonthsAsync(Guid owner, Guid accountId, int year, CancellationToken ct)
    {
        var account = await db.Accounts.AsNoTracking().SingleOrDefaultAsync(a => a.UserId == owner && a.Id == accountId, ct);
        if (account is null) return null;
        var start = new DateOnly(year, 1, 1);
        var all = AccountMovements.Query(db, owner).Where(m => m.AccountId == accountId);
        var opening = account.OpeningBalance + await all.Where(m => m.Date < start).SumAsync(m => m.Amount, ct);
        var sums = await all.Where(m => m.Date >= start && m.Date < start.AddYears(1)).GroupBy(m => m.Date.Month)
            .Select(g => new { Month = g.Key, Amount = g.Sum(m => m.Amount), Count = g.Count() }).ToArrayAsync(ct);
        var values = new List<MonthValue>(); var running = opening;
        for (var month = 1; month <= 12; month++)
        {
            var sum = sums.SingleOrDefault(s => s.Month == month); running += sum?.Amount ?? 0m;
            values.Add(new(month, running, 0m, new DateOnly(year, month, 1) <= Today || sum?.Count > 0));
        }
        var max = values.Max(v => Math.Abs(v.Amount));
        return new(year, values.Select(v => v with { BarRatio = max == 0m ? 0m : Math.Abs(v.Amount) / max }).ToArray());
    }

    public async Task<TransitPage?> TransitAsync(Guid owner, Guid accountId, int offset, CancellationToken ct)
    {
        var name = await db.Accounts.AsNoTracking().Where(a => a.UserId == owner && a.Id == accountId).Select(a => a.Name).SingleOrDefaultAsync(ct);
        if (name is null) return null;
        return await ReadTransitAsync(owner, accountId, name, offset, PageSize, ct);
    }

    private async Task<TransitPage> ReadTransitAsync(Guid owner, Guid accountId, string name, int offset, int limit, CancellationToken ct)
    {
        var query = db.PosSettlements.AsNoTracking().Where(s => s.UserId == owner && s.AccountId == accountId && !s.IsCancelled && s.PosDepositId == null);
        var stats = await query.GroupBy(s => s.AccountId).Select(g => new
        { Total = g.Sum(s => s.GrossAmount.Amount - s.CommissionAmount), Count = g.Count() }).SingleOrDefaultAsync(ct);
        var total = stats?.Total ?? 0m; var count = stats?.Count ?? 0;
        if (count == 0) return new(accountId, name, total, count, [], offset, false);
        var items = await query.OrderBy(s => s.ExpectedTransferDate).ThenByDescending(s => s.SettlementDate).ThenBy(s => s.Id)
            .Skip(offset).Take(limit).Select(s => new
            {
                s.Id,
                s.Kind,
                Amount = s.GrossAmount.Amount - s.CommissionAmount,
                s.CommissionAmount,
                ParentId = s.Kind == PosSettlementKind.Sale ? s.Id :
                    db.CounterpartyPayments.Where(p => p.UserId == owner && p.PosSettlementId == s.Id).Select(p => (Guid?)p.Id).FirstOrDefault() ??
                    db.ObligationSettlements.Where(p => p.UserId == owner && p.PosSettlementId == s.Id).Select(p => (Guid?)p.Id).FirstOrDefault(),
                ParentKind = s.Kind == PosSettlementKind.Sale ? (int)FinancialActivityKind.PosSale :
                    db.CounterpartyPayments.Any(p => p.UserId == owner && p.PosSettlementId == s.Id) ? (int)FinancialActivityKind.CounterpartySettlement : (int)FinancialActivityKind.ObligationSettlement
            }).ToArrayAsync(ct);
        var rows = await activities.ReadPageRowsAsync(owner, items.Select(i => i.ParentId!.Value).ToArray(), ct);
        var map = rows.ToDictionary(r => (r.ActivityKind, r.ActivityId));
        var page = items.Select(i => new PageActivity(map[((FinancialActivityKind)i.ParentKind, i.ParentId!.Value)], i.Amount,
            i.CommissionAmount > 0m ? "commission" : null, i.CommissionAmount > 0m ? i.CommissionAmount : null, State: "transit")).ToArray();
        return new(accountId, name, total, count, page, offset, offset + page.Length < count);
    }

    public async Task<AccountsOverview> AccountsAsync(Guid owner, CancellationToken ct)
    {
        var accounts = await db.Accounts.AsNoTracking().Where(a => a.UserId == owner).OrderBy(a => a.Type).ThenBy(a => a.Name).ThenBy(a => a.Id).ToArrayAsync(ct);
        var changes = await AccountMovements.SumByAccountAsync(db, owner, null, ct);
        var items = accounts.Select(a => new AccountListItem(a.Id, a.Name, a.Type.ToString().ToLowerInvariant(),
            a.DefaultScope?.ToString().ToLowerInvariant(), a.IsActive, a.OpeningBalance + changes.GetValueOrDefault(a.Id))).ToArray();
        return new(items.Sum(a => a.Balance), items.Where(a => a.IsActive && a.Type == "cash").Sum(a => a.Balance),
            items.Where(a => a.IsActive && a.Type == "bank").Sum(a => a.Balance), items.Where(a => !a.IsActive).Sum(a => a.Balance), items);
    }

    public async Task<ActivityParts?> PartsAsync(Guid owner, FinancialActivityKind kind, Guid activityId, CancellationToken ct)
    {
        if (!await activities.HasOwnedActivityAsync(owner, kind, activityId, ct)) return null;
        var row = (await activities.ReadPageRowsAsync(owner, [activityId], ct)).SingleOrDefault(r => r.ActivityKind == kind);
        if (row is null) return null;
        Guid? categoryId = null; decimal? amount = null; string? part = null;
        if (row.InterestPortion > 0m)
        {
            amount = row.InterestPortion; part = "interest";
            var role = kind == FinancialActivityKind.DebtCollection ? CategoryRole.InterestIncome : CategoryRole.InterestExpense;
            categoryId = await db.Categories.Where(c => c.UserId == owner && c.Role == role).Select(c => (Guid?)c.Id).SingleOrDefaultAsync(ct);
        }
        else if (row.FeeAmount > 0m)
        {
            amount = row.FeeAmount;
            switch (kind)
            {
                case FinancialActivityKind.Transfer:
                    part = "fee";
                    categoryId = await db.Transfers.Where(t => t.UserId == owner && t.Id == activityId)
                        .Select(t => t.FeeTransaction == null ? (Guid?)null : t.FeeTransaction.CategoryId).SingleAsync(ct);
                    break;
                case FinancialActivityKind.PosDeposit:
                    part = "deduction";
                    categoryId = await (from d in db.PosDeposits
                                        join t in db.Transactions
                        on new { d.UserId, Id = d.DeductionTransactionId } equals new { t.UserId, Id = (Guid?)t.Id }
                                        where d.UserId == owner && d.Id == activityId
                                        select (Guid?)t.CategoryId).SingleOrDefaultAsync(ct);
                    break;
                default:
                    part = "commission";
                    var settlements = db.PosSettlements.Where(s => s.UserId == owner);
                    categoryId = kind switch
                    {
                        FinancialActivityKind.PosSale => await settlements.Where(s => s.Id == activityId).Select(s => s.CommissionCategoryId).SingleOrDefaultAsync(ct),
                        FinancialActivityKind.CounterpartySettlement => await (from p in db.CounterpartyPayments
                                                                               join s in settlements
                            on p.PosSettlementId equals s.Id
                                                                               where p.UserId == owner && p.Id == activityId
                                                                               select s.CommissionCategoryId).SingleOrDefaultAsync(ct),
                        FinancialActivityKind.ObligationSettlement => await (from p in db.ObligationSettlements
                                                                             join s in settlements
                            on p.PosSettlementId equals s.Id
                                                                             where p.UserId == owner && p.Id == activityId
                                                                             select s.CommissionCategoryId).SingleOrDefaultAsync(ct),
                        _ => null
                    };
                    break;
            }
        }
        var name = categoryId is Guid id ? await db.Categories.Where(c => c.UserId == owner && c.Id == id).Select(c => c.Name).SingleAsync(ct) : null;
        return new(part is not null && amount is decimal value ? [new(part, value, categoryId, name)] : []);
    }
}
