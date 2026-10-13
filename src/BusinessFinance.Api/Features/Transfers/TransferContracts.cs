namespace BusinessFinance.Api.Features.Transfers;

public sealed record CreateTransferRequest(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    string Amount,
    string Currency,
    string TransferDate,
    string? Description,
    string? FeeAmount = null,
    string? FeeScope = null);

public sealed record TransferResponse(
    Guid Id,
    Guid SourceAccountId,
    Guid DestinationAccountId,
    string Amount,
    string Currency,
    string TransferDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc,
    Guid? FeeTransactionId = null,
    string? FeeAmount = null,
    Guid? FeeCategoryId = null,
    string? FeeScope = null);

/// <param name="HasMore">
/// Pencerede satır tavanından fazla transfer vardı; liste kırpıldı.
/// </param>
public sealed record TransferListResponse(
    IReadOnlyList<TransferResponse> Items,
    bool HasMore);

public sealed record TransferPreviewResponse(Guid SourceAccountId, string SourceAccountName,
    string TotalOutflow, string FeeAmount, Guid? FeeCategoryId, string? FeeCategoryName, string? ResolvedScope, bool NeedsScope);
