using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.FinancialActivities;

public static class FinancialActivityErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError InvalidDateRange = new(
        "financial_activities.invalid_date_range",
        "The date range is invalid.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidPage = new(
        "financial_activities.invalid_page",
        "Page number and page size are out of range.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidSearch = new(
        "financial_activities.invalid_search",
        "Search text must be at most 100 characters.",
        ApplicationErrorType.Validation);
}
