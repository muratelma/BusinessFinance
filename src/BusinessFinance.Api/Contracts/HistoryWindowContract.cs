using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Abstractions.Queries;

namespace BusinessFinance.Api.Contracts;

/// <summary>
/// Geçmiş listelerinin ortak <c>from</c> / <c>to</c> / <c>all</c> sözleşmesi.
/// </summary>
/// <remarks>
/// Kart hareketleri ve transferler aynı kuralı paylaşıyor; ayrı ayrı
/// yazılsaydı birinde düzeltilen varsayılan ötekinde eskirdi.
///
/// Tarih verilmezse sunucu son üç ayı uygular. "Tümü" ayrı ve **açık** bir
/// niyet (<c>all=true</c>): eksik parametreyle sınırsız sorguya düşmek,
/// istemcinin bir alanı unutmasını sessiz bir performans sorununa çevirirdi.
/// Satır tavanı her durumda geçerlidir, <c>all=true</c> dâhil.
/// </remarks>
internal static class HistoryWindowContract
{
    public static bool TryParse(
        string? from,
        string? to,
        bool all,
        TimeProvider timeProvider,
        HttpContext httpContext,
        out HistoryWindow window,
        out IResult? error)
    {
        window = HistoryWindow.Unbounded;
        error = null;

        DateOnly? fromDate = null;
        DateOnly? toDate = null;
        if (from is not null)
        {
            if (!FinanceContract.TryParseDate(from, out var parsed))
            {
                error = InvalidRange(httpContext, "from");
                return false;
            }
            fromDate = parsed;
        }

        if (to is not null)
        {
            if (!FinanceContract.TryParseDate(to, out var parsed))
            {
                error = InvalidRange(httpContext, "to");
                return false;
            }
            toDate = parsed;
        }

        if (fromDate is DateOnly start && toDate is DateOnly end && start > end)
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "The from date cannot be later than the to date.",
                "history.invalid_range");
            return false;
        }

        if (fromDate is null && toDate is null)
        {
            window = all
                ? HistoryWindow.Unbounded
                : HistoryWindow.DefaultFor(
                    DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));
            return true;
        }

        window = new HistoryWindow(fromDate, toDate);
        return true;
    }

    private static IResult InvalidRange(HttpContext httpContext, string field) =>
        ApiProblemResults.Validation(
            httpContext,
            $"The {field} date must use the yyyy-MM-dd format.",
            "history.invalid_range");
}
