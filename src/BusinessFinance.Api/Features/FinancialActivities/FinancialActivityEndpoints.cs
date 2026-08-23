using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.FinancialActivities;

namespace BusinessFinance.Api.Features.FinancialActivities;

public static class FinancialActivityEndpoints
{
    private const int DefaultPageSize = 20;

    public static IEndpointRouteBuilder MapFinancialActivityEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/financial-activities", ListAsync)
            .WithTags("Financial Activities")
            .WithName("ListFinancialActivities")
            .RequireAuthorization()
            .Produces<FinancialActivityListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        endpoints.MapGet("/api/v1/financial-activities/planned", ListPlannedAsync)
            .WithTags("Financial Activities")
            .WithName("ListPlannedFinancialActivities")
            .RequireAuthorization()
            .Produces<PlannedActivityListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        return endpoints;
    }

    private static async Task<IResult> ListPlannedAsync(
        ListPlannedActivitiesUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        string? asOfDate = null,
        int daysAhead = 30,
        string? scope = null)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var parsedAsOfDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "As-of date must use the yyyy-MM-dd format.",
                "planned_activities.invalid_as_of_date");
        }
        if (!FinanceContract.TryParseOptionalScope(scope, out var parsedScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scope must be business, personal or empty.",
                "planned_activities.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(
            new PlannedActivityQuery(parsedAsOfDate, daysAhead, parsedScope), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var value = result.Value;
        return Results.Ok(new PlannedActivityListResponse(
            FinanceContract.Date(value.AsOfDate),
            value.DaysAhead,
            FinanceContract.OptionalScopeValue(value.Scope),
            value.TotalCount,
            value.NearestDueDate is DateOnly nearest ? FinanceContract.Date(nearest) : null,
            value.Items.Select(ToResponse).ToArray()));
    }

    internal static PlannedActivityResponse ToResponse(PlannedActivityDto item) => new(
        item.PlannedActivityId,
        PlannedKindValues[item.PlannedKind],
        EffectValues[item.Effect],
        TimingValues[item.Timing],
        ReadinessValues[item.Readiness],
        item.AttentionCode is PlannedActivityAttention attention
            ? AttentionValues[attention]
            : null,
        ActionValues[item.ActionKind],
        FinanceContract.Date(item.DueDate),
        FinanceContract.Money(item.Amount),
        item.Currency.ToString(),
        item.Title,
        item.Description,
        item.SourceId,
        item.SourceName,
        item.CategoryId,
        item.CategoryName,
        item.IsProjected,
        PlannedActivityRules.IsPaymentObligation(item),
        item.ActionTargetId,
        item.ActionSequence);

    internal static readonly Dictionary<PlannedActivityKind, string> PlannedKindValues = new()
    {
        [PlannedActivityKind.RecurringOccurrence] = "recurring-occurrence",
        [PlannedActivityKind.CardInstallment] = "card-installment",
        [PlannedActivityKind.CardStatement] = "card-statement",
        [PlannedActivityKind.DebtInstallment] = "debt-installment",
        [PlannedActivityKind.ReceivableInstallment] = "receivable-installment"
    };

    internal static readonly Dictionary<PlannedActivityTiming, string> TimingValues = new()
    {
        [PlannedActivityTiming.Overdue] = "overdue",
        [PlannedActivityTiming.Today] = "today",
        [PlannedActivityTiming.Upcoming] = "upcoming"
    };

    internal static readonly Dictionary<PlannedActivityReadiness, string> ReadinessValues = new()
    {
        [PlannedActivityReadiness.Ready] = "ready",
        [PlannedActivityReadiness.NeedsAttention] = "needs-attention"
    };

    internal static readonly Dictionary<PlannedActivityAttention, string> AttentionValues = new()
    {
        [PlannedActivityAttention.CardInactive] = "card-inactive",
        [PlannedActivityAttention.CardLimitInsufficient] = "card-limit-insufficient",
        [PlannedActivityAttention.AccountInactive] = "account-inactive",
        [PlannedActivityAttention.CategoryInactive] = "category-inactive"
    };

    internal static readonly Dictionary<PlannedActivityAction, string> ActionValues = new()
    {
        [PlannedActivityAction.Realize] = "realize",
        [PlannedActivityAction.PayCard] = "pay-card",
        [PlannedActivityAction.PayDebt] = "pay-debt",
        [PlannedActivityAction.CollectDebt] = "collect-debt"
    };

    private static async Task<IResult> ListAsync(
        ListFinancialActivitiesUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        int pageNumber = 1,
        int pageSize = DefaultPageSize,
        string? dateFrom = null,
        string? dateTo = null,
        string? sourceGroup = null,
        string? activityKind = null,
        string? effect = null,
        string? origin = null,
        Guid? accountId = null,
        Guid? creditCardId = null,
        Guid? categoryId = null,
        Guid? counterpartyId = null,
        string? scope = null,
        bool includeCancelled = true)
    {
        if (!TryParseOptionalDate(dateFrom, out var parsedFrom) ||
            !TryParseOptionalDate(dateTo, out var parsedTo))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Dates must use the yyyy-MM-dd format.",
                "financial_activities.invalid_date_range");
        }

        if (!TryParseOptional(sourceGroup, SourceGroupValues, out var parsedSourceGroup) ||
            !TryParseOptional(activityKind, ActivityKindValues, out var parsedKind) ||
            !TryParseOptional(effect, EffectValues, out var parsedEffect) ||
            !TryParseOptional(origin, OriginValues, out var parsedOrigin))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "One of the supplied filter values is not supported.",
                "financial_activities.invalid_filter_value");
        }

        if (!FinanceContract.TryParseOptionalScope(scope, out var parsedScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scope must be business, personal or empty.",
                "financial_activities.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(
            new FinancialActivityListCriteria(
                pageNumber,
                pageSize,
                parsedFrom,
                parsedTo,
                parsedSourceGroup,
                parsedKind,
                parsedEffect,
                parsedOrigin,
                accountId,
                creditCardId,
                categoryId,
                counterpartyId,
                parsedScope,
                includeCancelled),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var value = result.Value;
        var totalPages = value.PageSize == 0
            ? 0
            : (int)Math.Ceiling(value.TotalCount / (double)value.PageSize);
        return Results.Ok(new FinancialActivityListResponse(
            value.Items.Select(ToResponse).ToArray(),
            new PaginationMetadata(
                value.PageNumber,
                value.PageSize,
                value.TotalCount,
                totalPages,
                value.PageNumber > 1,
                value.PageNumber < totalPages)));
    }

    internal static FinancialActivityResponse ToResponse(FinancialActivityDto item)
    {
        var activity = item.Activity;
        return new FinancialActivityResponse(
            activity.ActivityId,
            ActivityKindValues[activity.ActivityKind],
            EffectValues[activity.Effect],
            SourceGroupValues[activity.SourceGroup],
            OriginValues[activity.Origin],
            StatusValues[activity.Status],
            FinanceContract.Date(activity.ActivityDate),
            FinanceContract.Money(activity.Amount),
            activity.Currency.ToString(),
            activity.Title,
            activity.Description,
            activity.CategoryId,
            activity.CategoryName,
            activity.SourceId,
            activity.SourceName,
            activity.DestinationId,
            activity.DestinationName,
            activity.CancelledAtUtc,
            FinanceContract.OptionalScopeValue(activity.Scope),
            item.CanCancel,
            item.SupportsAttachments,
            activity.PrincipalPortion is decimal principal
                ? FinanceContract.Money(principal)
                : null,
            activity.InterestPortion is decimal interest
                ? FinanceContract.Money(interest)
                : null);
    }

    private static bool TryParseOptionalDate(string? value, out DateOnly? date)
    {
        date = null;
        if (value is null) return true;
        if (!FinanceContract.TryParseDate(value, out var parsed)) return false;
        date = parsed;
        return true;
    }

    /// <summary>
    /// An absent filter is valid and means "no filter"; an unrecognised value is not
    /// silently ignored, because quietly dropping a filter would show the caller more
    /// data than it asked for.
    /// </summary>
    private static bool TryParseOptional<TEnum>(
        string? value,
        IReadOnlyDictionary<TEnum, string> values,
        out TEnum? parsed)
        where TEnum : struct, Enum
    {
        parsed = null;
        if (value is null) return true;
        foreach (var pair in values)
        {
            if (string.Equals(pair.Value, value, StringComparison.OrdinalIgnoreCase))
            {
                parsed = pair.Key;
                return true;
            }
        }

        return false;
    }

    // Bu sözlükler enum'la birlikte büyümek zorunda. Eksik bir giriş derleme
    // hatası vermez; feed'i okuyan ilk isteğin `KeyNotFoundException`'ıyla
    // ortaya çıkar. `ContractValues_CoverEveryEnumMember` her üyenin karşılığı
    // olduğunu doğruluyor, çünkü bu tam olarak bir kez başımıza geldi:
    // `debt-opening` eklendi, sözlüğe yazılmadı ve İşlemler sayfası açılmaz
    // oldu.
    internal static readonly Dictionary<FinancialActivityKind, string> ActivityKindValues = new()
    {
        [FinancialActivityKind.AccountTransaction] = "account-transaction",
        [FinancialActivityKind.Transfer] = "transfer",
        [FinancialActivityKind.CardCharge] = "card-charge",
        [FinancialActivityKind.CardPayment] = "card-payment",
        [FinancialActivityKind.DebtPayment] = "debt-payment",
        [FinancialActivityKind.DebtCollection] = "debt-collection",
        [FinancialActivityKind.DebtOpening] = "debt-opening",
        [FinancialActivityKind.CounterpartyCharge] = "counterparty-charge",
        [FinancialActivityKind.CounterpartySettlement] = "counterparty-settlement"
    };

    internal static readonly Dictionary<FinancialActivityEffect, string> EffectValues = new()
    {
        [FinancialActivityEffect.Income] = "income",
        [FinancialActivityEffect.Expense] = "expense",
        [FinancialActivityEffect.Neutral] = "neutral"
    };

    internal static readonly Dictionary<FinancialActivitySourceGroup, string> SourceGroupValues = new()
    {
        [FinancialActivitySourceGroup.Account] = "account",
        [FinancialActivitySourceGroup.CreditCard] = "credit-card",
        [FinancialActivitySourceGroup.Transfer] = "transfer",
        [FinancialActivitySourceGroup.Debt] = "debt",
        [FinancialActivitySourceGroup.Counterparty] = "counterparty"
    };

    internal static readonly Dictionary<FinancialActivityOrigin, string> OriginValues = new()
    {
        [FinancialActivityOrigin.Manual] = "manual",
        [FinancialActivityOrigin.CsvImport] = "csv-import",
        [FinancialActivityOrigin.Recurring] = "recurring",
        [FinancialActivityOrigin.Installment] = "installment"
    };

    internal static readonly Dictionary<FinancialActivityStatus, string> StatusValues = new()
    {
        [FinancialActivityStatus.Realized] = "realized",
        [FinancialActivityStatus.Cancelled] = "cancelled"
    };
}
