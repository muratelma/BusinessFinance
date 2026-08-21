namespace BusinessFinance.Api.Features.CreditCards;

public sealed record CreateInstallmentPlanRequest(
    Guid CreditCardId,
    Guid CategoryId,
    Guid ClientRequestId,
    string TotalAmount,
    string Currency,
    int InstallmentCount,
    string FirstInstallmentDate,
    string? Description);

public sealed record InstallmentItemResponse(
    Guid Id,
    int Sequence,
    string Amount,
    string Currency,
    string ScheduledDate,
    bool IsRealized,
    Guid? CreditCardChargeId,
    DateTimeOffset? RealizedAtUtc);

public sealed record InstallmentPlanResponse(
    Guid Id,
    Guid CreditCardId,
    Guid CategoryId,
    Guid ClientRequestId,
    string TotalAmount,
    string Currency,
    int InstallmentCount,
    string FirstInstallmentDate,
    string? Description,
    IReadOnlyList<InstallmentItemResponse> Items);

public sealed record InstallmentPlanListResponse(IReadOnlyList<InstallmentPlanResponse> Items);
