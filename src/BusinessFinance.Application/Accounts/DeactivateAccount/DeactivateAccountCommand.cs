namespace BusinessFinance.Application.Accounts.DeactivateAccount;

public sealed record DeactivateAccountCommand
{
    public Guid AccountId { get; }

    public DeactivateAccountCommand(Guid accountId)
    {
        if (accountId == Guid.Empty)
        {
            throw new ArgumentException("Account id cannot be empty.", nameof(accountId));
        }

        AccountId = accountId;
    }
}
