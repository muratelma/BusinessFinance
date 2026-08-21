namespace BusinessFinance.Api.Features.Transfers;

public sealed record CreateTransferRequest(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    string Amount,
    string Currency,
    string TransferDate,
    string? Description);

public sealed record TransferResponse(
    Guid Id,
    Guid SourceAccountId,
    Guid DestinationAccountId,
    string Amount,
    string Currency,
    string TransferDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc);

/// <param name="HasMore">
/// Pencerede satır tavanından fazla transfer vardı; liste kırpıldı.
/// </param>
public sealed record TransferListResponse(
    IReadOnlyList<TransferResponse> Items,
    bool HasMore);
