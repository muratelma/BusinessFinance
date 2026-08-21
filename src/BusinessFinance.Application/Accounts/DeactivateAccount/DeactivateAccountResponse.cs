namespace BusinessFinance.Application.Accounts.DeactivateAccount;

public sealed record DeactivateAccountResponse(Guid Id, bool IsActive);
