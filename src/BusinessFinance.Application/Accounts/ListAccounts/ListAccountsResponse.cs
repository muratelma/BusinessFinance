namespace BusinessFinance.Application.Accounts.ListAccounts;

public sealed record ListAccountsResponse(
    IReadOnlyList<AccountListItemDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount);
