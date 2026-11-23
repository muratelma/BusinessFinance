using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Features.Pos;

namespace BusinessFinance.Api.Features.Counterparties;

// `scope`: kişinin türü (`business` / `personal`). İşletmesi olmayan
// kullanıcıda boş bırakılırsa `personal` yazılır; işletmesi olanda boş kalır.
public sealed record CreateCounterpartyRequest(string Name, string? Note = null, string? Scope = null);

/// <summary>
/// Karşı tarafın tam güncel hâli. <c>IsActive</c> pasifleştirmeyi de taşır:
/// pasif karşı tarafa yeni borçlandırma yazılamaz, tahsilat yazılabilir.
/// </summary>
// `scope` boşsa kişinin türü değişmez.
public sealed record UpdateCounterpartyRequest(
    string Name, bool IsActive, string? Note = null, string? Scope = null);

public sealed record CounterpartyResponse(
    Guid Id,
    string Name,
    string? Note,
    bool IsActive,
    string Receivable,
    string Payable,
    string OverdueReceivable,
    string OverduePayable,
    string NotOverdueReceivable,
    string NotOverduePayable,
    string Net,
    bool IsSettled,
    // İki tarafın ekranda yazılacak hâli; ikisi de sıfır ya da artıdır ve
    // farkları `net`'e eşittir. Fazla tahsilatta `receivable` eksiye düşer
    // (kırpılmaz); o tutar burada `owedByYou`'ya eklenmiş gelir.
    string OwedToYou = "0.0000",
    string OwedByYou = "0.0000",
    string OpeningReceivable = "0.0000",
    string OpeningPayable = "0.0000",
    // Kişinin türü: `business` (cari), `personal` ya da bilinmiyorsa boş.
    // Listeyi böler; kaydın rapordaki kapsamı değildir (ADR 0022 İ5).
    string? Scope = null);

public sealed record CounterpartyListResponse(IReadOnlyList<CounterpartyResponse> Items);

/// <summary>
/// Veresiye satış / vadeli alım. <c>direction</c>: <c>receivable</c> alacak
/// doğurur (gelir kategorisi ister), <c>payable</c> borç doğurur (gider).
/// </summary>
public sealed record CreateCounterpartyChargeRequest(
    string Direction,
    string Amount,
    string Currency,
    Guid CategoryId,
    string ChargeDate,

    // İsteğe bağlı. Kapsamı kategori sınırlar; kategori iki kapsama açıksa
    // bu seçim, o da yoksa kişinin türü geçerlidir (ADR 0022 İ5). Kategoriyle
    // çelişen seçim `counterparties.scope_conflict` ile reddedilir.
    string? Scope = null,
    string? Description = null,
    string? DueDate = null,
    Guid? ClientRequestId = null,

    // İsteğe bağlı: satış ya da alımla birlikte o gün alınan ya da verilen
    // peşin veya avans tutarı. Kişinin hesabına tahsilat ya da ödeme olarak,
    // satışla aynı yazmada yazılır; satışı aşamaz.
    CounterpartyPaidNowRequest? PaidNow = null);

public sealed record CounterpartyPaidNowRequest(string Amount, Guid AccountId);

/// <summary>Tahsilat ya da ödeme panelinin kısayolları: <c>Tamamı</c>, <c>Gecikmiş</c>.</summary>
public sealed record CounterpartyShortcutsResponse(string Full, string Overdue);

/// <summary>
/// Kişi sayfası (S6). Tutarlar kişinin hesabı ve kişili taksitli kayıtlarının
/// toplamıdır; kısayollar yalnız hesabındır. Hareketler
/// <c>GET /api/v1/financial-activities?counterpartyId=&amp;direction=</c> ile okunur.
/// </summary>
public sealed record CounterpartyPageResponse(
    Guid Id,
    string Name,
    string? Note,
    bool IsActive,
    string? Scope,
    string OpeningReceivable,
    string OpeningPayable,
    string OwedToYou,
    string OwedByYou,
    string OverdueReceivable,
    string OverduePayable,
    string? NextReceivableDueDate,
    string NextReceivableDueAmount,
    string? NextPayableDueDate,
    string NextPayableDueAmount,
    CounterpartyShortcutsResponse Collect,
    CounterpartyShortcutsResponse Pay,
    IReadOnlyList<BusinessFinance.Api.Features.Debts.DebtBookItemResponse> Records);

// Borç para. `direction`: `receivable` verdim (hesaptan çıkar), `payable`
// aldım (hesaba girer). Avans ayrı kayıt değildir; tahsilat ya da ödemedir.
public sealed record CreateCounterpartyLoanRequest(
    string Direction,
    string Amount,
    string Currency,
    Guid AccountId,
    string LoanDate,
    string? DueDate = null,
    string? Description = null,
    Guid? ClientRequestId = null);

public sealed record CounterpartyLoanResponse(
    Guid Id,
    Guid CounterpartyId,
    Guid AccountId,
    string Direction,
    string Amount,
    string Currency,
    string LoanDate,
    string? DueDate,
    string? Description,
    bool IsCancelled);

/// <summary>
/// Tahsilat / ödeme. Kategori ve kapsam **taşımaz**: gelir/gider raporuna
/// girmez, yalnız kasayı değiştirir.
/// </summary>
/// <remarks>
/// <c>card</c> doluysa tahsilat kartla (POS) alınmıştır (ADR 0019 T5): para
/// yola çıkar, hesaba yatışla geçer; <c>accountId</c> boş kalabilir (POS'un
/// hesabı). Kart yalnız tahsilatta (<c>receivable</c>) geçerlidir.
/// <c>creditCardId</c> doluysa ödeme kişiye kredi kartıyla yapılmıştır (yalnız
/// <c>payable</c>): kart borcu artar, hesap verilmez, gider yazılmaz.
/// </remarks>
public sealed record CreateCounterpartyPaymentRequest(
    string Direction,
    string Amount,
    string Currency,
    Guid? AccountId,
    string PaymentDate,
    string? Description = null,
    CardCollectionRequest? Card = null,
    Guid? ClientRequestId = null,
    Guid? CreditCardId = null,

    // `Kalanı indirim say` (senaryo 20): ödemeden sonra o tarafta kalan tutar
    // en yeni açık alımlara indirim olarak yazılır, ödemeyle aynı yazmada.
    bool CloseRemainderAsDiscount = false);

// Vazgeçme. `direction`: `receivable` alacaktan vazgeçtim (gider kategorisi),
// `payable` borcum bağışlandı (gelir kategorisi). Kalanı aşamaz.
public sealed record CreateCounterpartyWriteOffRequest(
    string Direction,
    string Amount,
    string Currency,
    Guid CategoryId,
    string WriteOffDate,
    string? Scope = null,
    string? Description = null,
    Guid? ClientRequestId = null);

public sealed record CounterpartyWriteOffResponse(
    Guid Id,
    Guid CounterpartyId,
    Guid CategoryId,
    string Direction,
    string Amount,
    string Currency,
    string Scope,
    string WriteOffDate,
    string? Description,
    bool IsCancelled);

public sealed record CounterpartyChargeResponse(
    Guid Id,
    Guid CounterpartyId,
    Guid CategoryId,
    string Direction,
    string Amount,
    string Currency,
    string Scope,
    string ChargeDate,
    string? Description,
    bool IsCancelled,
    string? DueDate,
    string NetAmount,
    IReadOnlyList<CounterpartyChargeDiscountResponse> Discounts,

    // Bu istekle birlikte yazılan peşin ya da avans tahsilatı/ödemesi; yoksa boş.
    Guid? PaidNowPaymentId = null);

public sealed record CounterpartyChargeDiscountResponse(
    Guid Id,
    string Amount,
    string DiscountDate,
    bool IsCancelled);

/// <summary>Cari alıma indirim; para hareketi yoktur (ADR 0021 T3).</summary>
public sealed record CounterpartyChargeDiscountRequest(
    string Amount,
    string DiscountDate,
    Guid? ClientRequestId = null);

/// <summary>Uygulamadan önce devreden bakiye (ADR 0021 T5); ikisi de sıfır ya da artı.</summary>
public sealed record CounterpartyOpeningBalanceRequest(
    string Receivable,
    string Payable);

public sealed record CounterpartyPaymentResponse(
    Guid Id,
    Guid CounterpartyId,
    // Kişiye kredi kartıyla ödemede boştur; kart `creditCardId`dedir.
    Guid? AccountId,
    string Direction,
    string Amount,
    string Currency,
    string PaymentDate,
    string? Description,
    bool IsCancelled,
    // Kartla tahsilde paranın yoldaki POS kaydı; değilse boş.
    Guid? PosSettlementId = null,
    Guid? CreditCardId = null,
    // Bu ödemeyle indirim olarak kapatılan kalan; yoksa "0.0000".
    string RemainderDiscount = "0.0000");
