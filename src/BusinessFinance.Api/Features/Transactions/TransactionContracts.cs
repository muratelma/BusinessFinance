using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.Transactions;

public sealed record CreateTransactionRequest(
    Guid AccountId,
    Guid CategoryId,
    string Amount,
    string Currency,
    string Type,
    // İsteğe bağlı: boş bırakılırsa sunucu kapsamı türetir, türetemezse
    // isteği reddeder ve bir değer uydurmaz.
    string? Scope,
    string TransactionDate,
    string? Description,
    // Belgedeki KDV; ikisi de boş bırakılabilir. Sunucu birini diğerinden
    // türetmez (ADR 0016).
    string? VatRate = null,
    string? VatAmount = null,
    // Gider matrahtan düşülebilir mi (ADR 0016). Boş bırakılırsa kategorinin
    // varsayılanı kullanılır; şahsi kayıtta ve gelirde sorulmaz.
    bool? IsTaxDeductible = null);

public sealed record TransactionResponse(
    Guid Id,
    Guid AccountId,
    Guid CategoryId,
    string Amount,
    string Currency,
    string Type,
    string Scope,
    string TransactionDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc,
    VatContract? Vat,
    bool? IsTaxDeductible);

public sealed record TransactionListResponse(
    IReadOnlyList<TransactionResponse> Items,
    PaginationMetadata Pagination);
