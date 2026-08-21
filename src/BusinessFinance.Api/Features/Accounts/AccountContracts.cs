using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.Accounts;

public sealed record CreateAccountRequest(
    string Name,
    string Type,
    string Currency,
    string OpeningBalance = "0");

public sealed record UpdateAccountRequest(string Name, bool IsActive);

public sealed record AccountResponse(
    Guid Id,
    string Name,
    string Type,
    string Currency,
    bool IsActive,
    string OpeningBalance,
    string Balance);

public sealed record AccountListResponse(
    IReadOnlyList<AccountResponse> Items,
    PaginationMetadata Pagination);
