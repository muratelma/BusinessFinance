using BusinessFinance.Domain;

namespace BusinessFinance.Application.CreditCards;

public sealed record GetCreditCardStatementQuery(
    Guid CreditCardId,
    int Year,
    int Month,
    DateOnly AsOfDate);

public sealed record CreditCardStatementDto(
    Guid CreditCardId,
    int Year,
    int Month,
    DateOnly PeriodStart,
    DateOnly ClosingDate,
    DateOnly DueDate,
    decimal PreviousBalance,
    decimal PeriodCharges,
    decimal PaymentsThroughClosing,
    decimal StatementBalance,
    decimal PaymentsAfterClosing,
    decimal RemainingBalance,
    decimal MinimumPayment,
    decimal RemainingMinimumPayment,
    decimal MinimumPaymentRate,
    CurrencyCode Currency,
    StatementPaymentStatus PaymentStatus);

/// <summary>
/// Kartın en son kesilmiş ekstresi; hiç kesim geçmemişse <c>null</c>.
/// </summary>
/// <remarks>
/// Yeni açılmış bir kartın ilk kesim tarihi henüz gelmemiş olabilir. Bunu
/// hata saymak yanlış olurdu: ortada bir kusur yok, sadece henüz ekstre yok.
/// </remarks>
public sealed record CurrentCreditCardStatementDto(
    Guid CreditCardId,
    CreditCardStatementDto? Statement);

public sealed record StatementActivitySnapshot(
    decimal PreviousBalance,
    decimal PeriodCharges,
    decimal PaymentsThroughClosing,
    decimal PaymentsAfterClosing);

public interface ICreditCardStatementRepository
{
    Task<StatementActivitySnapshot> GetActivityAsync(
        Guid creditCardId,
        Guid userId,
        CreditCardStatementPeriod period,
        DateOnly asOfDate,
        CancellationToken cancellationToken);
}
