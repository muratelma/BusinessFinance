using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.FinancePages;

/// <summary>Okumalar oturumdan türetilen sahibin verisiyle yürütülür.</summary>
public sealed class FinancePageUseCases(ICurrentUser currentUser, IFinancePageReader reader)
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
}
