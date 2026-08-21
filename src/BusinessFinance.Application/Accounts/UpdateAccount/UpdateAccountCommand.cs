namespace BusinessFinance.Application.Accounts.UpdateAccount;

public sealed record UpdateAccountCommand(Guid AccountId, string Name, bool IsActive);
