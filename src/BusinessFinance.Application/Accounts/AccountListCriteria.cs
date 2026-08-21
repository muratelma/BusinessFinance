using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts;

public sealed record AccountListCriteria(
    int PageNumber,
    int PageSize,
    bool? IsActive,
    AccountType? Type,
    AccountSortOrder SortOrder);
