namespace BusinessFinance.Api.Features.Cash;

public sealed record CreateCashCountRequest(
    Guid AccountId,
    string CountedAmount,
    string CountDate,
    string? Scope = null,
    string? Note = null);

public sealed record ConfirmCashCountDifferenceRequest(Guid CategoryId);

public sealed record CashCountResponse(
    Guid Id,
    Guid AccountId,
    string AccountName,
    string CountDate,
    string CountedAmount,
    string Currency,
    string Scope,
    string? Note,
    bool IsCancelled,
    Guid? AdjustmentTransactionId,
    string? ExpectedBalance = null,
    string? Difference = null);

public sealed record CashCountListResponse(IReadOnlyList<CashCountResponse> Items);

public sealed record CashCountTodayResponse(
    Guid AccountId,
    string AccountName,
    string ExpectedBalance,
    string Currency,
    CashCountResponse? Count);
