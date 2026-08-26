using BusinessFinance.Application.Taxes;

namespace BusinessFinance.Api.Features.Taxes;

/// <summary>
/// Kurulacak takvim kaleminin önerisi. <b>Tutar taşımaz</b> ve kullanıcıya
/// gösterilecek cümleyi istemci kurar; sunucu kararlı makine değerleri gönderir.
/// </summary>
public sealed record TaxCalendarSuggestionResponse(
    string Key,
    string Frequency,
    int SuggestedDayOfMonth,
    string SuggestedCategoryName,
    string Kind,
    string Scope);

public sealed record TaxCalendarSuggestionListResponse(
    IReadOnlyList<TaxCalendarSuggestionResponse> Items);

public static class TaxCalendarEndpoints
{
    public static IEndpointRouteBuilder MapTaxCalendarEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/tax-calendar/suggestions", ListSuggestions)
            .WithTags("TaxCalendar")
            .WithName("ListTaxCalendarSuggestions")
            .RequireAuthorization()
            .Produces<TaxCalendarSuggestionListResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    /// <summary>
    /// Hazır kalemleri döner.
    /// </summary>
    /// <remarks>
    /// Bu uç <b>hiçbir şey yazmaz</b>: kalem, önerinin doldurduğu formla mevcut
    /// tekrarlayan plan ucundan kurulur. İkinci bir yazma yolu, aynı planın iki
    /// ayrı biçimde oluşabilmesi demek olurdu.
    /// </remarks>
    private static IResult ListSuggestions() => Results.Ok(
        new TaxCalendarSuggestionListResponse(
            [.. TaxCalendarSuggestions.All.Select(suggestion => new TaxCalendarSuggestionResponse(
                suggestion.Key,
                suggestion.Frequency.ToString().ToLowerInvariant(),
                suggestion.SuggestedDayOfMonth,
                suggestion.SuggestedCategoryName,
                KindValue(suggestion.Kind),
                suggestion.Scope.ToString().ToLowerInvariant()))]));

    private static string KindValue(Domain.RecurringTransactionKind kind) => kind switch
    {
        Domain.RecurringTransactionKind.Income => "income",
        Domain.RecurringTransactionKind.Expense => "expense",
        Domain.RecurringTransactionKind.BillPayment => "bill-payment",
        _ => throw new InvalidOperationException("Recurring kind is not supported.")
    };
}
