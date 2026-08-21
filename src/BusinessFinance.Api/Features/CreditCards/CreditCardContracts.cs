namespace BusinessFinance.Api.Features.CreditCards;

public sealed record CreateCreditCardRequest(
    string Name,
    string Limit,
    string Currency,
    int StatementClosingDay,
    int PaymentDueDay,

    // Gönderilmezse kartın varsayılan oranı kullanılır; güncellemede
    // gönderilmezse mevcut oran korunur.
    string? MinimumPaymentRate = null);

public sealed record UpdateCreditCardRequest(
    string Name,
    string Limit,
    string Currency,
    int StatementClosingDay,
    int PaymentDueDay,
    bool IsActive,
    string? MinimumPaymentRate = null);

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
    bool IsActive);

public sealed record CreditCardListResponse(IReadOnlyList<CreditCardResponse> Items);
