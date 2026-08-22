using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts.ListAccounts;

public sealed record AccountListItemDto(
    Guid Id,
    string Name,
    AccountType Type,
    CurrencyCode Currency,
    bool IsActive,
    decimal OpeningBalance,
    decimal Balance,
    TransactionScope? DefaultScope);
