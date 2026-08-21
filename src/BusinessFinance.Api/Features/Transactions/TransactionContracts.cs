using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.Transactions;

public sealed record CreateTransactionRequest(
    Guid AccountId,
    Guid CategoryId,
    string Amount,
    string Currency,
    string Type,
    string TransactionDate,
    string? Description);

public sealed record TransactionResponse(
    Guid Id,
    Guid AccountId,
    Guid CategoryId,
    string Amount,
    string Currency,
    string Type,
    string TransactionDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc);

public sealed record TransactionListResponse(
    IReadOnlyList<TransactionResponse> Items,
    PaginationMetadata Pagination);
