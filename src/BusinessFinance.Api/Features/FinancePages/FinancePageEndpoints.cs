using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.FinancePages;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.FinancePages;

public sealed class CategoryPageRequest
{
    public int? Year { get; init; }
    public int? Month { get; init; }
    public string? Direction { get; init; }
    public string? Scope { get; init; }
    public string? Source { get; init; }
    public string? Person { get; init; }
    public int? Offset { get; init; }
}
public sealed class AccountPageRequest
{
    public int? Year { get; init; }
    public int? Month { get; init; }
    public string? Direction { get; init; }
    public string? Selection { get; init; }
    public int? Offset { get; init; }
}

public static class FinancePageEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = CreateOptions();

    public static IEndpointRouteBuilder MapFinancePageEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1").WithTags("Finance pages").RequireAuthorization();
        group.MapGet("/category-summary", SummaryAsync);
        group.MapGet("/category-summary/months", SummaryMonthsAsync);
        group.MapGet("/categories/{categoryId:guid}/page", CategoryAsync);
        group.MapGet("/categories/{categoryId:guid}/months", CategoryMonthsAsync);
        group.MapGet("/categories/uncategorized/page", UncategorizedAsync);
        group.MapGet("/categories/uncategorized/months", UncategorizedMonthsAsync);
        group.MapGet("/accounts/overview", AccountsAsync);
        group.MapGet("/accounts/{accountId:guid}/page", AccountAsync);
        group.MapGet("/accounts/{accountId:guid}/months", AccountMonthsAsync);
        group.MapGet("/accounts/{accountId:guid}/in-transit", TransitAsync);
        group.MapGet("/financial-activities/{activityKind}/{activityId:guid}/parts", PartsAsync);
        return endpoints;
    }

    private static bool CategoryQuery(CategoryPageRequest request, TimeProvider clock, out CategoryPageQuery query)
    {
        query = null!;
        var today = LocalDay.Today(clock.GetUtcNow());
        var year = request.Year ?? today.Year;
        var month = request.Month ?? today.Month;
        if (year is < 2 or > 9998 || month is < 1 or > 12 || request.Offset < 0 ||
            request.Direction is not (null or "income" or "expense") ||
            request.Scope is not (null or "business" or "personal") ||
            !ValidChoice(request.Source, "deferred", "account:", "card:") ||
            !ValidChoice(request.Person, "none", "person:")) return false;
        query = new(year, month, request.Direction == "income" ? TransactionType.Income : TransactionType.Expense,
            request.Scope switch { "business" => TransactionScope.Business, "personal" => TransactionScope.Personal, _ => null },
            request.Source, request.Person, request.Offset ?? 0);
        return true;
    }

    private static bool ValidChoice(string? value, string special, params string[] prefixes) =>
        value is null || value == special || prefixes.Any(p => value.StartsWith(p, StringComparison.Ordinal) && Guid.TryParse(value[p.Length..], out var id) && id != Guid.Empty);
    private static IResult Invalid(HttpContext context) => ApiProblemResults.Validation(context,
        "The page filter is invalid.", "finance_pages.invalid_filter");
    private static IResult Response<T>(ApplicationResult<T> result, HttpContext context) where T : notnull => result.IsSuccess
        ? Results.Json(result.Value, JsonOptions) : result.Error.ToProblemResult(context);

    private static async Task<IResult> SummaryAsync([AsParameters] CategoryPageRequest request,
        FinancePageUseCases useCases, TimeProvider clock, HttpContext context, CancellationToken ct) =>
        CategoryQuery(request, clock, out var q) ? Response(await useCases.SummaryAsync(q, ct), context) : Invalid(context);
    private static async Task<IResult> SummaryMonthsAsync([AsParameters] CategoryPageRequest request,
        FinancePageUseCases useCases, TimeProvider clock, HttpContext context, CancellationToken ct) =>
        CategoryQuery(request, clock, out var q) ? Response(await useCases.CategoryMonthsAsync(null, true, q, ct), context) : Invalid(context);
    private static async Task<IResult> CategoryAsync(Guid categoryId, [AsParameters] CategoryPageRequest request,
        FinancePageUseCases useCases, TimeProvider clock, HttpContext context, CancellationToken ct) =>
        CategoryQuery(request, clock, out var q) ? Response(await useCases.CategoryAsync(categoryId, q, ct), context) : Invalid(context);
    private static async Task<IResult> CategoryMonthsAsync(Guid categoryId, [AsParameters] CategoryPageRequest request,
        FinancePageUseCases useCases, TimeProvider clock, HttpContext context, CancellationToken ct) =>
        CategoryQuery(request, clock, out var q) ? Response(await useCases.CategoryMonthsAsync(categoryId, false, q, ct), context) : Invalid(context);
    private static async Task<IResult> UncategorizedAsync([AsParameters] CategoryPageRequest request,
        FinancePageUseCases useCases, TimeProvider clock, HttpContext context, CancellationToken ct) =>
        CategoryQuery(request, clock, out var q) ? Response(await useCases.CategoryAsync(null, q, ct), context) : Invalid(context);
    private static async Task<IResult> UncategorizedMonthsAsync([AsParameters] CategoryPageRequest request,
        FinancePageUseCases useCases, TimeProvider clock, HttpContext context, CancellationToken ct) =>
        CategoryQuery(request, clock, out var q) ? Response(await useCases.CategoryMonthsAsync(null, false, q, ct), context) : Invalid(context);
    private static async Task<IResult> AccountsAsync(FinancePageUseCases useCases, HttpContext context, CancellationToken ct) =>
        Response(await useCases.AccountsAsync(ct), context);

    private static async Task<IResult> AccountAsync(Guid accountId, [AsParameters] AccountPageRequest request,
        FinancePageUseCases useCases, TimeProvider clock, HttpContext context, CancellationToken ct)
    {
        var today = LocalDay.Today(clock.GetUtcNow());
        var year = request.Year ?? today.Year; var month = request.Month ?? today.Month;
        if (year is < 2 or > 9998 || month is < 1 or > 12 || request.Offset < 0 || request.Direction is not (null or "in" or "out") || !ValidSelection(request.Selection)) return Invalid(context);
        return Response(await useCases.AccountAsync(accountId, new(year, month, request.Direction, request.Selection, request.Offset ?? 0), ct), context);
    }
    private static bool ValidSelection(string? selection)
    {
        if (selection is null or "all") return true;
        var parts = selection.Split(':');
        return parts.Length is 2 or 3 && parts[0] is "in" or "out" &&
            parts[1] is "category" or "transfer" or "payment" or "collection" &&
            (parts.Length == 2 || Guid.TryParse(parts[2], out var id) && id != Guid.Empty ||
             int.TryParse(parts[2], out var kind) && Enum.IsDefined((FinancialActivityKind)kind));
    }
    private static async Task<IResult> AccountMonthsAsync(Guid accountId, FinancePageUseCases useCases,
        TimeProvider clock, HttpContext context, CancellationToken ct, int year = 0)
    {
        if (year == 0) year = LocalDay.Today(clock.GetUtcNow()).Year;
        return year is < 2 or > 9998 ? Invalid(context) : Response(await useCases.AccountMonthsAsync(accountId, year, ct), context);
    }
    private static async Task<IResult> TransitAsync(Guid accountId, FinancePageUseCases useCases,
        HttpContext context, CancellationToken ct, int offset = 0) => offset < 0 ? Invalid(context) :
        Response(await useCases.TransitAsync(accountId, offset, ct), context);

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new PageDecimalConverter());
        options.Converters.Add(new PageActivityConverter());
        return options;
    }
    private static async Task<IResult> PartsAsync(string activityKind, Guid activityId, FinancePageUseCases useCases,
        HttpContext context, CancellationToken ct)
    {
        var pair = FinancialActivityEndpoints.ActivityKindValues.SingleOrDefault(p => p.Value == activityKind);
        if (pair.Key == 0) return Invalid(context);
        return Response(await useCases.PartsAsync(pair.Key, activityId, ct), context);
    }
    private sealed class PageDecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => throw new NotSupportedException();
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString("F4", CultureInfo.InvariantCulture));
    }
    private sealed class PageActivityConverter : JsonConverter<FinancialActivityRow>
    {
        public override FinancialActivityRow Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => throw new NotSupportedException();
        public override void Write(Utf8JsonWriter writer, FinancialActivityRow value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, FinancialActivityEndpoints.ToResponse(ListFinancialActivitiesUseCase.ToDto(value)), options);
    }
}
