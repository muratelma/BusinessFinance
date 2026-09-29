using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.Pos;

public sealed record CreatePosSettlementRequest(
    Guid AccountId,
    Guid CategoryId,
    string GrossAmount,
    string Currency,
    string SettlementDate,
    string ExpectedTransferDate,
    string? CommissionAmount = null,
    string? CommissionRate = null,
    Guid? CommissionCategoryId = null,
    string? Scope = null,
    string? Description = null);

public sealed record MarkPosSettlementTransferredRequest(string TransferDate);

public sealed record PosSettlementResponse(
    Guid Id,
    Guid AccountId,
    string AccountName,
    Guid CategoryId,
    string CategoryName,
    Guid? CommissionCategoryId,
    string? CommissionCategoryName,
    string GrossAmount,
    string CommissionAmount,
    string NetAmount,
    string CommissionRate,
    string Currency,
    string Scope,
    string SettlementDate,
    string ExpectedTransferDate,
    string? TransferredOn,
    string? Description,
    bool IsInTransit,
    bool IsCancelled,
    bool IsLate);

public sealed record PosSettlementListResponse(
    IReadOnlyList<PosSettlementResponse> Items,
    string MoneyInTransit,
    int InTransitCount);
