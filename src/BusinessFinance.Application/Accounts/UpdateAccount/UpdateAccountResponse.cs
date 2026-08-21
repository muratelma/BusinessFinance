namespace BusinessFinance.Application.Accounts.UpdateAccount;

public sealed record UpdateAccountResponse(Guid Id, string Name, bool IsActive);
