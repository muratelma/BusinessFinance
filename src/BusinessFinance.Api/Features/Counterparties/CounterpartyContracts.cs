using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Features.Pos;

namespace BusinessFinance.Api.Features.Counterparties;

public sealed record CreateCounterpartyRequest(string Name, string? Note = null);

/// <summary>
/// Karşı tarafın tam güncel hâli. <c>IsActive</c> pasifleştirmeyi de taşır:
/// pasif karşı tarafa yeni borçlandırma yazılamaz, tahsilat yazılabilir.
/// </summary>
public sealed record UpdateCounterpartyRequest(string Name, bool IsActive, string? Note = null);

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
    // Kişiye bağlı açık yükümlülüklerin (ödenmemiş fatura, tek seferlik
    // alacak) toplamı. Bilgi amaçlıdır: `receivable`, `payable` ve `net`
    // yalnız cari hareketlerden oluşur ve bu tutarları içermez.
    string OpenReceivableObligations = "0.0000",
    string OpenPayableObligations = "0.0000",
    // İki tarafın ekranda yazılacak hâli; ikisi de sıfır ya da artıdır ve
    // farkları `net`'e eşittir. Fazla tahsilatta `receivable` eksiye düşer
    // (kırpılmaz); o tutar burada `owedByYou`'ya eklenmiş gelir.
    string OwedToYou = "0.0000",
    string OwedByYou = "0.0000");

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

    // İsteğe bağlı. Cari borçlandırma her zaman işletme yazılır (ADR 0020
    // İ9); `personal` ya da şahsi bir kategori `counterparties.scope_conflict`
    // ile reddedilir.
    string? Scope = null,
    string? Description = null,
    string? DueDate = null);

/// <summary>
/// Tahsilat / ödeme. Kategori ve kapsam **taşımaz**: gelir/gider raporuna
/// girmez, yalnız kasayı değiştirir.
/// </summary>
/// <remarks>
/// <c>card</c> doluysa tahsilat kartla (POS) alınmıştır (ADR 0019 T5): para
/// yola çıkar, hesaba yatışla geçer; <c>accountId</c> boş kalabilir (POS'un
/// hesabı). Kart yalnız tahsilatta (<c>receivable</c>) geçerlidir.
/// </remarks>
public sealed record CreateCounterpartyPaymentRequest(
    string Direction,
    string Amount,
    string Currency,
    Guid? AccountId,
    string PaymentDate,
    string? Description = null,
    CardCollectionRequest? Card = null);

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
    string? DueDate);

public sealed record CounterpartyPaymentResponse(
    Guid Id,
    Guid CounterpartyId,
    Guid AccountId,
    string Direction,
    string Amount,
    string Currency,
    string PaymentDate,
    string? Description,
    bool IsCancelled,
    // Kartla tahsilde paranın yoldaki POS kaydı; değilse boş.
    Guid? PosSettlementId = null);
