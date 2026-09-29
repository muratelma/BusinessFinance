using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Counterparties;

public sealed record CounterpartyDto(
    Guid Id,
    string Name,
    string? Note,
    bool IsActive,
    decimal Receivable,
    decimal Payable,
    decimal OverdueReceivable,
    decimal OverduePayable,
    decimal NotOverdueReceivable,
    decimal NotOverduePayable,
    decimal Net,
    bool IsSettled);

public sealed record CreateCounterpartyCommand(string Name, string? Note);

public sealed record UpdateCounterpartyCommand(
    Guid CounterpartyId,
    string Name,
    string? Note,
    bool IsActive);

/// <summary>
/// Veresiye satış ya da tedarikçiden vadeli alım: gelir/gider tanır, kasaya
/// dokunmaz (ADR 0014).
/// </summary>
public sealed record CreateCounterpartyChargeCommand(
    Guid CounterpartyId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    Guid CategoryId,

    /// <summary>
    /// Kullanıcının açık seçimi. Boşsa kategorinin varsayılanı kullanılır;
    /// ikisi de boşsa istek reddedilir ve sunucu kapsam uydurmaz. Karşı taraf
    /// kapsam ipucu <b>taşımaz</b> — kategori "ne satıldı" sorusunu zaten
    /// cevaplıyor.
    /// </summary>
    TransactionScope? Scope,
    DateOnly ChargeDate,
    string? Description,
    DateOnly? DueDate);

/// <summary>
/// Tahsilat ya da ödeme: kasayı değiştirir, gelir/gider üretmez. Bu yüzden ne
/// kategori ne kapsam taşır.
/// </summary>
public sealed record CreateCounterpartyPaymentCommand(
    Guid CounterpartyId,
    Guid AccountId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly PaymentDate,
    string? Description);

public sealed record CounterpartyChargeDto(
    Guid Id,
    Guid CounterpartyId,
    Guid CategoryId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    TransactionScope Scope,
    DateOnly ChargeDate,
    string? Description,
    bool IsCancelled,
    DateOnly? DueDate);

public sealed record CounterpartyPaymentDto(
    Guid Id,
    Guid CounterpartyId,
    Guid AccountId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly PaymentDate,
    string? Description,
    bool IsCancelled);
