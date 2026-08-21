using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts.CreateAccount;

public sealed record CreateAccountResponse(
    Guid Id,
    string Name,
    AccountType Type,
    CurrencyCode Currency,
    decimal OpeningBalance);
