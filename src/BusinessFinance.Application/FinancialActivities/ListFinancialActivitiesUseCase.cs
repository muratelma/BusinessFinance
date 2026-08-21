using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.FinancialActivities;

public sealed class ListFinancialActivitiesUseCase(
    ICurrentUser currentUser,
    IFinancialActivityRepository repository)
{
    public const int MaximumPageSize = 100;

    public async Task<ApplicationResult<FinancialActivityListResult>> ExecuteAsync(
        FinancialActivityListCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            // No owner, no query: the repository is never reached without an identity.
            return ApplicationResult<FinancialActivityListResult>.Failure(
                FinancialActivityErrors.AuthenticationRequired);
        }

        if (criteria.PageNumber < 1 ||
            criteria.PageSize < 1 ||
            criteria.PageSize > MaximumPageSize)
        {
            return ApplicationResult<FinancialActivityListResult>.Failure(
                FinancialActivityErrors.InvalidPage);
        }

        if (criteria.DateFrom is DateOnly from &&
            criteria.DateTo is DateOnly to &&
            from > to)
        {
            return ApplicationResult<FinancialActivityListResult>.Failure(
                FinancialActivityErrors.InvalidDateRange);
        }

        var page = await repository.ListAsync(userId, criteria, cancellationToken);
        return ApplicationResult<FinancialActivityListResult>.Success(
            new FinancialActivityListResult(
                page.Items.Select(ToDto).ToArray(),
                criteria.PageNumber,
                criteria.PageSize,
                page.TotalCount));
    }

    internal static FinancialActivityDto ToDto(FinancialActivityRow row) => new(
        row,
        FinancialActivityCapabilities.CanCancel(row.ActivityKind, row.Origin, row.Status),
        FinancialActivityCapabilities.SupportsAttachments(row.ActivityKind));
}
