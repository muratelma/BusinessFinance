namespace BusinessFinance.Application.Accounts.DeleteAccount;

public sealed record DeleteAccountCommand
{
    public DeleteAccountCommand(Guid accountId)
    {
        if (accountId == Guid.Empty)
        {
            throw new ArgumentException("Account id must not be empty.", nameof(accountId));
        }

        AccountId = accountId;
    }

    public Guid AccountId { get; }
}
