namespace BusinessFinance.Application.Accounts.GetAccount;

public sealed record GetAccountQuery
{
    public Guid AccountId { get; }

    public GetAccountQuery(Guid accountId)
    {
        if (accountId == Guid.Empty)
        {
            throw new ArgumentException("Account id cannot be empty.", nameof(accountId));
        }

        AccountId = accountId;
    }
}
