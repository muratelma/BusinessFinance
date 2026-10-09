using BusinessFinance.Application.Pos;
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
    bool IsSettled,
    // Kişiye bağlı açık yükümlülüklerin toplamı; bilgi amaçlıdır, yukarıdaki
    // hiçbir tutara girmez.
    decimal OpenReceivableObligations = 0m,
    decimal OpenPayableObligations = 0m,
    // İki tarafın ekranda yazılacak, eksiye düşmeyen hâli: fazla tahsilat
    // bizim borcumuza, fazla ödeme karşı tarafın borcuna eklenir.
    decimal OwedToYou = 0m,
    decimal OwedByYou = 0m);

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
/// <remarks>
/// <see cref="Card"/> doluysa tahsilat kartla (POS) alınmıştır (ADR 0019 T5):
/// para yola çıkar ve hesaba yatışla geçer; hesap POS'tan ya da
/// <see cref="CardCollectionInput.AccountId"/>'den gelir ve
/// <see cref="AccountId"/> boş kalabilir. Doluysa <see cref="AccountId"/>
/// zorunludur.
/// </remarks>
public sealed record CreateCounterpartyPaymentCommand(
    Guid CounterpartyId,
    Guid? AccountId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly PaymentDate,
    string? Description,
    CardCollectionInput? Card = null);

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
    bool IsCancelled,
    // Kartla tahsilde paranın yoldaki POS kaydı; değilse boş.
    Guid? PosSettlementId = null);
