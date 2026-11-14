using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.FinancePages;

/// <summary>Okumalar oturumdan türetilen sahibin verisiyle yürütülür.</summary>
public sealed class FinancePageUseCases(ICurrentUser currentUser, IFinancePageReader reader,
    ICreditCardRepository cards, ICreditCardStatementRepository statements, TimeProvider clock)
{
    private async Task<ApplicationResult<T>> ReadAsync<T>(Func<Guid, Task<T?>> read) where T : class
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<T>.Failure(new("authentication.required", "Authentication required.", ApplicationErrorType.Unauthorized));
        var value = await read(userId);
        return value is null
            ? ApplicationResult<T>.Failure(new("finance_pages.not_found", "The owned resource was not found.", ApplicationErrorType.NotFound))
            : ApplicationResult<T>.Success(value);
    }

    public Task<ApplicationResult<CategorySummary>> SummaryAsync(CategoryPageQuery query, CancellationToken ct) =>
        ReadAsync<CategorySummary>(async id => await reader.CategorySummaryAsync(id, query, ct));
    public Task<ApplicationResult<CategoryPage>> CategoryAsync(Guid? id, CategoryPageQuery query, CancellationToken ct) =>
        ReadAsync<CategoryPage>(user => reader.CategoryAsync(user, id, query, ct));
    public Task<ApplicationResult<YearMonths>> CategoryMonthsAsync(Guid? id, bool summary, CategoryPageQuery query, CancellationToken ct) =>
        ReadAsync<YearMonths>(user => reader.CategoryMonthsAsync(user, id, summary, query, ct));
    public Task<ApplicationResult<AccountPage>> AccountAsync(Guid id, AccountPageQuery query, CancellationToken ct) =>
        ReadAsync<AccountPage>(user => reader.AccountAsync(user, id, query, ct));
    public Task<ApplicationResult<YearMonths>> AccountMonthsAsync(Guid id, int year, CancellationToken ct) =>
        ReadAsync<YearMonths>(user => reader.AccountMonthsAsync(user, id, year, ct));
    public Task<ApplicationResult<TransitPage>> TransitAsync(Guid id, int offset, CancellationToken ct) =>
        ReadAsync<TransitPage>(user => reader.TransitAsync(user, id, offset, ct));
    public Task<ApplicationResult<AccountsOverview>> AccountsAsync(CancellationToken ct) =>
        ReadAsync<AccountsOverview>(async id => await reader.AccountsAsync(id, ct));
    public Task<ApplicationResult<ActivityParts>> PartsAsync(BusinessFinance.Application.FinancialActivities.FinancialActivityKind kind, Guid id, CancellationToken ct) =>
        ReadAsync<ActivityParts>(user => reader.PartsAsync(user, kind, id, ct));

    /// <summary>
    /// Kartın hareketleri. Dönem verilmezse açık ekstredir (bugünü içeren
    /// dönem); ekstre seçiliyse durumu da döner. Ekstre listesi ok düğmeleri
    /// içindir: açık ekstre ve ondan önceki 23 ekstre, eskiden yeniye.
    /// </summary>
    public async Task<ApplicationResult<CardPage>> CardAsync(Guid cardId, CardPageSelection selection, CancellationToken ct)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<CardPage>.Failure(new("authentication.required", "Authentication required.", ApplicationErrorType.Unauthorized));
        var card = await cards.FindOwnedByIdAsync(cardId, userId, false, ct);
        if (card is null)
            return ApplicationResult<CardPage>.Failure(new("finance_pages.not_found", "The owned resource was not found.", ApplicationErrorType.NotFound));
        var today = LocalDay.Today(clock.GetUtcNow());
        var thisClosing = new DateOnly(today.Year, today.Month, card.StatementClosingDay);
        var current = today <= thisClosing ? thisClosing : thisClosing.AddMonths(1);
        var ranges = new List<CardStatementRange>();
        for (var back = 23; back >= 0; back--)
        {
            var closing = current.AddMonths(-back);
            if (closing.Year < 2000) continue;
            var range = CreditCardStatementPeriod.ForClosingMonth(card, closing.Year, closing.Month);
            ranges.Add(new(closing.Year, closing.Month, range.PeriodStart, range.ClosingDate));
        }

        DateOnly? from = null, to = null;
        CardPagePeriod period;
        if (selection.All)
        {
            period = new("all", null, null, null, null, null);
        }
        else if (selection.From is not null || selection.To is not null)
        {
            from = selection.From; to = selection.To;
            period = new("range", null, null, from, to, null);
        }
        else
        {
            var year = selection.Year ?? current.Year;
            var month = selection.Month ?? current.Month;
            if (year < 2000 || new DateOnly(year, month, 1) > new DateOnly(current.Year, current.Month, 1))
                return ApplicationResult<CardPage>.Failure(new("finance_pages.invalid_filter", "The page filter is invalid.", ApplicationErrorType.Validation));
            var range = CreditCardStatementPeriod.ForClosingMonth(card, year, month);
            from = range.PeriodStart; to = range.ClosingDate;
            string status = "current";
            if (today > range.ClosingDate)
            {
                var snapshot = await statements.GetActivityAsync(card.Id, userId, range, today, ct);
                var statement = CreditCardStatement.Create(card, year, month, today, snapshot.PreviousBalance,
                    snapshot.PeriodCharges, snapshot.PaymentsThroughClosing, snapshot.PaymentsAfterClosing);
                status = statement.PaymentStatus.ToString().ToLowerInvariant();
            }
            period = new("statement", year, month, from, to, status);
        }

        var data = await reader.CardAsync(userId, cardId, new(from, to, selection.Kind, selection.Scope,
            selection.CategoryId, selection.Offset, selection.Limit), ct);
        if (data is null)
            return ApplicationResult<CardPage>.Failure(new("finance_pages.not_found", "The owned resource was not found.", ApplicationErrorType.NotFound));
        return ApplicationResult<CardPage>.Success(new(card.Id, card.Name, period, ranges, data.Spending, data.Payments,
            data.Categories, data.Items, data.TotalCount, data.Offset, data.HasMore));
    }
}

/// <summary>
/// Kart sayfasının seçimi: ekstre (<see cref="Year"/>/<see cref="Month"/>),
/// tarih aralığı ya da bütün zamanlar; boşsa açık ekstre.
/// </summary>
public sealed record CardPageSelection(int? Year = null, int? Month = null, DateOnly? From = null,
    DateOnly? To = null, bool All = false, string? Kind = null, TransactionScope? Scope = null,
    Guid? CategoryId = null, int Offset = 0, int Limit = 50);
