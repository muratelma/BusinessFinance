using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transactions;

public sealed record TransactionListCriteria(
    int PageNumber,
    int PageSize,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    Guid? AccountId,
    Guid? CategoryId,
    TransactionType? Type);

public sealed record TransactionListPage(
    IReadOnlyList<BudgetTransaction> Items,
    int TotalCount);
