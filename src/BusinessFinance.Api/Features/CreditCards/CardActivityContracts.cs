namespace BusinessFinance.Api.Features.CreditCards;

public sealed record CreateCardChargeRequest(
    Guid CategoryId,
    string Amount,
    string Currency,
    string Scope,
    string ChargeDate,
    string? Description);

public sealed record CreateCardPaymentRequest(
    Guid AccountId,
    string Amount,
    string Currency,
    string PaymentDate,
    string? Description);

public sealed record CardChargeResponse(
    Guid Id,
    Guid CreditCardId,
    Guid CategoryId,
    string Amount,
    string Currency,
    string Scope,
    string ChargeDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc);

public sealed record CardPaymentResponse(
    Guid Id,
    Guid CreditCardId,
    Guid AccountId,
    string Amount,
    string Currency,
    string PaymentDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc);

/// <param name="HasMore">
/// İstenen pencerede satır tavanından fazla kayıt vardı; liste kırpıldı.
/// Sessizce kırpmak ekranı yanlış bir tamlık iddiasında bırakırdı.
/// </param>
public sealed record CardActivityResponse(
    IReadOnlyList<CardChargeResponse> Charges,
    IReadOnlyList<CardPaymentResponse> Payments,
    bool HasMore);

public sealed record CreditCardStatementResponse(
    Guid CreditCardId,
    int Year,
    int Month,
    string PeriodStart,
    string ClosingDate,
    string DueDate,
    string PreviousBalance,
    string PeriodCharges,
    string PaymentsThroughClosing,
    string StatementBalance,
    string PaymentsAfterClosing,
    string RemainingBalance,
    string MinimumPayment,
    string RemainingMinimumPayment,
    string MinimumPaymentRate,
    string Currency,
    string PaymentStatus);

/// <summary>
/// Kartın en son kesilmiş ekstresi; ilk kesim henüz gelmemişse
/// <see cref="Statement"/> <c>null</c> olur ve bu bir hata değildir.
/// </summary>
public sealed record CurrentCreditCardStatementResponse(
    Guid CreditCardId,
    CreditCardStatementResponse? Statement);
