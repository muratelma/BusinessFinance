namespace BusinessFinance.Api.Features.UpcomingPayments;

public sealed record UpcomingPaymentResponse(
    Guid SourceId,
    string SourceType,
    string Title,

    // Tutarı henüz belli olmayan vergi kaleminde boştur.
    string? Amount,
    string Currency,
    string DueDate,
    string Timing,
    string? Description);

public sealed record UpcomingPaymentListResponse(
    string AsOfDate,
    int DaysAhead,
    IReadOnlyList<UpcomingPaymentResponse> Items);
