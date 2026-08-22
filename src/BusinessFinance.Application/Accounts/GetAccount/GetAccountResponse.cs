using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts.GetAccount;

public sealed record GetAccountResponse(
    Guid Id,
    string Name,
    AccountType Type,
    CurrencyCode Currency,
    bool IsActive,
    decimal OpeningBalance,
    decimal Balance,
    TransactionScope? DefaultScope);
