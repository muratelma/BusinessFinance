namespace BusinessFinance.Api.Features.CreditCards;

public sealed record CreateCreditCardRequest(
    string Name,
    string Limit,
    string Currency,
    int StatementClosingDay,
    int PaymentDueDay,

    // Gönderilmezse kartın varsayılan oranı kullanılır; güncellemede
    // gönderilmezse mevcut oran korunur.
    string? MinimumPaymentRate = null,
    string? DefaultScope = null);

/// <summary>
/// Kartın tam güncel hâli; <see cref="DefaultScope"/> boş gönderilirse
/// varsayılan kapsam kaldırılır.
/// </summary>
public sealed record UpdateCreditCardRequest(
    string Name,
    string Limit,
    string Currency,
    int StatementClosingDay,
    int PaymentDueDay,
    bool IsActive,
    string? MinimumPaymentRate = null,
    string? DefaultScope = null);

public sealed record CreditCardResponse(
    Guid Id,
    string Name,
    string Limit,
    string CurrentDebt,
    string AvailableLimit,
    string Currency,
    int StatementClosingDay,
    int PaymentDueDay,
    string MinimumPaymentRate,
    bool IsActive,
    string? DefaultScope);

public sealed record CreditCardListResponse(IReadOnlyList<CreditCardResponse> Items);
