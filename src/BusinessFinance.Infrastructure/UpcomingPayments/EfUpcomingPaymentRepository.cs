using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Application.UpcomingPayments;

namespace BusinessFinance.Infrastructure.UpcomingPayments;

/// <summary>
/// A narrowed view of the planned activity projection, not a second query.
/// </summary>
/// <remarks>
/// This used to hold its own copy of the recurring, installment, statement and debt
/// queries. Two independent sources of the same obligations would drift apart as one
/// side gained a rule the other did not, so the upcoming-payments feed now filters the
/// shared projection down to what the user owes. Recurring income and money owed to the
/// user are planned movements but not obligations, so they stay out; everything that
/// does appear carries the same amount, date and order as the planned view.
/// </remarks>
internal sealed class EfUpcomingPaymentRepository(IPlannedActivityRepository plannedActivities)
    : IUpcomingPaymentRepository
{
    public async Task<IReadOnlyList<UpcomingPaymentCandidate>> ListCandidatesAsync(
        Guid userId,
        DateOnly asOfDate,
        DateOnly horizonDate,
        CancellationToken cancellationToken)
    {
        var planned = await plannedActivities.ListAsync(
            userId, asOfDate, horizonDate, cancellationToken);

        return planned
            .Where(PlannedActivityRules.IsPaymentObligation)
            .Select(item => new UpcomingPaymentCandidate(
                item.PlannedActivityId,
                MapSourceType(item.PlannedKind),
                item.Title,
                item.Amount,
                item.Currency,
                item.DueDate,
                item.Description))
            .ToArray();
    }

    private static UpcomingPaymentSourceType MapSourceType(PlannedActivityKind kind) => kind switch
    {
        PlannedActivityKind.RecurringOccurrence => UpcomingPaymentSourceType.RecurringOccurrence,
        PlannedActivityKind.CardInstallment => UpcomingPaymentSourceType.Installment,
        PlannedActivityKind.CardStatement => UpcomingPaymentSourceType.CreditCardStatement,
        PlannedActivityKind.DebtInstallment => UpcomingPaymentSourceType.DebtInstallment,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };
}
