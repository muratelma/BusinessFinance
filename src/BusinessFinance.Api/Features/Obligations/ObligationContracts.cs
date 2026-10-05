using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Features.Pos;

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

/// <remarks>
/// <c>card</c> doluysa alacak kartla (POS) tahsil edilmiştir (ADR 0019 T5);
/// <c>accountId</c> boş kalabilir (POS'un hesabı).
/// </remarks>
public sealed record SettleObligationRequest(
    Guid? AccountId,
    string SettlementDate,
    CardCollectionRequest? Card = null);

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
