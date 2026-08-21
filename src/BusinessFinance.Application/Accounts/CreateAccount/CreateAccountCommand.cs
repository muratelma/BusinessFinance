using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts.CreateAccount;

public sealed record CreateAccountCommand(
    string Name,
    AccountType Type,
    CurrencyCode Currency,
    decimal OpeningBalance = 0m);
