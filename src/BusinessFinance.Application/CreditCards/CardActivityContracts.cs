using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.CreditCards;

public sealed record CardChargeDto(
    Guid Id,
    Guid CreditCardId,
    Guid CategoryId,
    decimal Amount,
    CurrencyCode Currency,
    TransactionScope Scope,
    DateOnly ChargeDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc,
    VatDto? Vat);

public sealed record CardPaymentDto(
    Guid Id,
    Guid CreditCardId,
    Guid AccountId,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly PaymentDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc);

public sealed record CreateCardChargeCommand(
    Guid CreditCardId,
    Guid CategoryId,
    decimal Amount,
    CurrencyCode Currency,

    // Kullanıcının açık seçimi. Boşsa kartın, yoksa kategorinin varsayılanı
    // kullanılır; üçü de boşsa istek reddedilir.
    TransactionScope? Scope,
    DateOnly ChargeDate,
    string? Description,

    // Belgedeki KDV; yoksa boştur (ADR 0016). Sunucu hiçbir vergi tutarını
    // hesaplamaz — ne geldiyse o taşınır.
    VatDto? Vat = null);

public sealed record CreateCardPaymentCommand(
    Guid CreditCardId,
    Guid AccountId,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly PaymentDate,
    string? Description);

/// <summary>
/// Bir kartın sınırlanmış hareket listesi.
/// </summary>
/// <param name="HasMore">
/// Pencerede satır tavanından fazla kayıt vardı; ekran tamlık iddia etmemeli.
/// </param>
public sealed record CardActivityDto(
    IReadOnlyList<CardChargeDto> Charges,
    IReadOnlyList<CardPaymentDto> Payments,
    bool HasMore);

public interface ICardChargeRepository
{
    Task AddAsync(CreditCardCharge charge, CancellationToken cancellationToken);
    Task<CreditCardCharge?> FindOwnedByIdAsync(
        Guid chargeId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    Task<BoundedList<CreditCardCharge>> ListAsync(
        Guid creditCardId,
        Guid userId,
        HistoryWindow window,
        CancellationToken cancellationToken);
    Task UpdateOwnedAsync(CreditCardCharge charge, Guid userId, CancellationToken cancellationToken);
}

public interface ICardPaymentRepository
{
    Task AddAsync(CreditCardPayment payment, CancellationToken cancellationToken);
    Task<CreditCardPayment?> FindOwnedByIdAsync(
        Guid paymentId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    Task<BoundedList<CreditCardPayment>> ListAsync(
        Guid creditCardId,
        Guid userId,
        HistoryWindow window,
        CancellationToken cancellationToken);
    Task UpdateOwnedAsync(CreditCardPayment payment, Guid userId, CancellationToken cancellationToken);
}
