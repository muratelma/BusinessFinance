using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transactions;

public sealed record TransactionDto(
    Guid Id,
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    CurrencyCode Currency,
    TransactionType Type,
    TransactionScope Scope,
    DateOnly TransactionDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc,
    VatDto? Vat);
