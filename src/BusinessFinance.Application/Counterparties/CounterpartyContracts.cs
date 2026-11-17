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
    decimal OwedByYou = 0m,
    // Uygulamadan önce devreden bakiye; yukarıdaki tutarların içindedir.
    decimal OpeningReceivable = 0m,
    decimal OpeningPayable = 0m,
    TransactionScope? Scope = null);

public sealed record CreateCounterpartyCommand(
    string Name, string? Note, TransactionScope? Scope = null);

public sealed record UpdateCounterpartyCommand(
    Guid CounterpartyId,
    string Name,
    string? Note,
    bool IsActive,

    // Boşsa kişinin türü değişmez; tür boşaltılamaz, yalnız değiştirilir.
    TransactionScope? Scope = null);

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
    DateOnly? DueDate,
    Guid? ClientRequestId = null,
    CounterpartyPaidNow? PaidNow = null);

/// <summary>
/// Satış ya da alımla birlikte o gün alınan ya da verilen peşin veya avans
/// tutarı: kişinin hesabına tahsilat ya da ödeme olarak, satışla aynı yazmada
/// yazılır (ADR 0022; `02-TASARIM.md` §7a).
/// </summary>
public sealed record CounterpartyPaidNow(decimal Amount, Guid AccountId);

/// <summary>
/// Kişinin bir tarafındaki açık kalem: alım ya da borç para; tutar indirimler
/// düşülmüş bedeldir.
/// </summary>
public sealed record CounterpartyOpenItem(Guid Id, bool IsCharge, DateOnly Date, decimal Amount);

/// <summary>Borç para: para taşır, gelir/gider yazmaz (ADR 0022 İ3).</summary>
public sealed record CreateCounterpartyLoanCommand(
    Guid CounterpartyId,
    Guid AccountId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly LoanDate,
    DateOnly? DueDate,
    string? Description,
    Guid? ClientRequestId = null);

public sealed record CounterpartyLoanDto(
    Guid Id,
    Guid CounterpartyId,
    Guid AccountId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly LoanDate,
    DateOnly? DueDate,
    string? Description,
    bool IsCancelled);

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
    CardCollectionInput? Card = null,
    Guid? ClientRequestId = null,
    // Kişiye kredi kartıyla ödeme (yalnız borç yönünde); hesap verilmez.
    Guid? CreditCardId = null,
    // `Kalanı indirim say`: ödemeden sonra o tarafta kalan tutar, ödeme gününe
    // kadarki en yeni açık alımlara indirim olarak yazılır (senaryo 20).
    bool CloseRemainderAsDiscount = false);

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
    DateOnly? DueDate,
    // Bedel: indirimler düşülmüş; gelir/gider bu kadardır.
    decimal NetAmount,
    IReadOnlyList<CounterpartyChargeDiscountDto> Discounts,

    // Bu istekle birlikte yazılan peşin ya da avans tahsilatı/ödemesi.
    Guid? PaidNowPaymentId = null);

public sealed record CounterpartyChargeDiscountDto(
    Guid Id,
    decimal Amount,
    DateOnly DiscountDate,
    bool IsCancelled);

public sealed record CounterpartyPaymentDto(
    Guid Id,
    Guid CounterpartyId,
    Guid? AccountId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly PaymentDate,
    string? Description,
    bool IsCancelled,
    // Kartla tahsilde paranın yoldaki POS kaydı; değilse boş.
    Guid? PosSettlementId = null,
    // Kişiye kredi kartıyla ödemede kart; hesap o zaman boştur.
    Guid? CreditCardId = null,
    // Bu ödemeyle birlikte indirim olarak kapatılan kalan; yoksa sıfır.
    decimal RemainderDiscount = 0m);
