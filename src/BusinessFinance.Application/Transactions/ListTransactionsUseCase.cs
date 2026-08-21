using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transactions;

public sealed record ListTransactionsQuery(
    int PageNumber,
    int PageSize,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    Guid? AccountId,
    Guid? CategoryId,
    TransactionType? Type);

public sealed record ListTransactionsResponse(
    IReadOnlyList<TransactionDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount);

public sealed class ListTransactionsUseCase(
    ICurrentUser currentUser,
    ITransactionRepository repository)
{
    public async Task<ApplicationResult<ListTransactionsResponse>> ExecuteAsync(
        ListTransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<ListTransactionsResponse>.Failure(
                TransactionErrors.AuthenticationRequired);
        }

        if (query.PageNumber < 1 || query.PageSize is < 1 or > 100)
        {
            return ApplicationResult<ListTransactionsResponse>.Failure(
                TransactionErrors.Validation("Invalid pagination values."));
        }
        if (query.DateFrom is DateOnly from && query.DateTo is DateOnly to && from > to)
        {
            return ApplicationResult<ListTransactionsResponse>.Failure(
                TransactionErrors.Validation("Date-from cannot be after date-to."));
        }

        var page = await repository.ListAsync(
            userId,
            new TransactionListCriteria(
                query.PageNumber,
                query.PageSize,
                query.DateFrom,
                query.DateTo,
                query.AccountId,
                query.CategoryId,
                query.Type),
            cancellationToken);
        return ApplicationResult<ListTransactionsResponse>.Success(
            new ListTransactionsResponse(
                page.Items.Select(CreateTransactionUseCase.ToDto).ToArray(),
                query.PageNumber,
                query.PageSize,
                page.TotalCount));
    }
}
