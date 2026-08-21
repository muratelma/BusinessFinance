using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts;

public sealed record AccountListPage(
    IReadOnlyList<Account> Items,
    int TotalCount);
