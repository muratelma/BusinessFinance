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
    string? Description = null);

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
    string? SettlementDate = null);

public sealed record ObligationListResponse(IReadOnlyList<ObligationResponse> Items);
