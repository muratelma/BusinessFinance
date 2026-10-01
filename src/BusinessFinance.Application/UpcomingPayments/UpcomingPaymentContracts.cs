using BusinessFinance.Domain;

namespace BusinessFinance.Application.UpcomingPayments;

public enum UpcomingPaymentSourceType
{
    RecurringOccurrence = 1,
    CreditCardStatement = 2,
    Installment = 3,
    DebtInstallment = 4,
    Obligation = 5
}

public enum UpcomingPaymentTiming
{
    Overdue = 1,
    Today = 2,
    Upcoming = 3
}

public sealed record UpcomingPaymentCandidate(
    Guid SourceId,
    UpcomingPaymentSourceType SourceType,
    string Title,
    decimal? Amount,
    CurrencyCode Currency,
    DateOnly DueDate,
    string? Description);

/// <param name="Amount">Tutarı henüz belli olmayan vergi kaleminde boştur.</param>
public sealed record UpcomingPaymentDto(
    Guid SourceId,
    UpcomingPaymentSourceType SourceType,
    string Title,
    decimal? Amount,
    CurrencyCode Currency,
    DateOnly DueDate,
    UpcomingPaymentTiming Timing,
    string? Description);

public sealed record GetUpcomingPaymentsQuery(
    DateOnly AsOfDate,
    int DaysAhead);

public interface IUpcomingPaymentRepository
{
    Task<IReadOnlyList<UpcomingPaymentCandidate>> ListCandidatesAsync(
        Guid userId,
        DateOnly asOfDate,
        DateOnly horizonDate,
        CancellationToken cancellationToken);
}
