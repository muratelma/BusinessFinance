using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts.ListAccounts;

public sealed record ListAccountsQuery
{
    public const int MaximumPageSize = 100;

    public int PageNumber { get; }
    public int PageSize { get; }
    public bool? IsActive { get; }
    public AccountType? Type { get; }

    public ListAccountsQuery(
        int pageNumber,
        int pageSize,
        bool? isActive = null,
        AccountType? type = null)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                pageNumber,
                "Page number must be at least 1.");
        }

        if (pageSize is < 1 or > MaximumPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                pageSize,
                $"Page size must be between 1 and {MaximumPageSize}.");
        }

        if (type is not null && type is not AccountType.Cash and not AccountType.Bank)
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Account type is not supported.");
        }

        PageNumber = pageNumber;
        PageSize = pageSize;
        IsActive = isActive;
        Type = type;
    }
}
