using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.Obligations;

public sealed record CreateObligationRequest(
    string Direction,
    string Amount,
    string Currency,
    Guid CategoryId,
    string IssueDate,
    string DueDate,
    string? Scope = null,
    Guid? CounterpartyId = null,
    string? Description = null,
    // Belgedeki KDV; ikisi de boş bırakılabilir. Sunucu birini diğerinden
    // türetmez (ADR 0016).
    string? VatRate = null,
    string? VatAmount = null,
    // Gider matrahtan düşülebilir mi (ADR 0016). Boş bırakılırsa kategorinin
    // varsayılanı kullanılır; şahsi kayıtta ve gelirde sorulmaz.
    bool? IsTaxDeductible = null);

public sealed record SettleObligationRequest(Guid AccountId, string SettlementDate);

public sealed record ObligationResponse(
    Guid Id,
    Guid? CounterpartyId,
    Guid CategoryId,
    string Direction,
    string Amount,
    string Currency,
    string Scope,
    string IssueDate,
    string DueDate,
    string? Description,
    string Status,
    string? CounterpartyName = null,
    string? CategoryName = null,
    bool IsOverdue = false,
    Guid? SettlementId = null,
    Guid? SettlementAccountId = null,
    string? SettlementDate = null,
    VatContract? Vat = null,
    bool? IsTaxDeductible = null);

public sealed record ObligationListResponse(IReadOnlyList<ObligationResponse> Items);
