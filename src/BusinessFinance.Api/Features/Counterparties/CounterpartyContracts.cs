using BusinessFinance.Api.Contracts;

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
    bool IsSettled);

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

    // İsteğe bağlı: boşsa kategorinin varsayılanı kullanılır, o da boşsa istek
    // `counterparties.scope_unresolved` ile reddedilir.
    string? Scope = null,
    string? Description = null,
    string? DueDate = null,
    // Belgedeki KDV; ikisi de boş bırakılabilir. Sunucu birini diğerinden
    // türetmez (ADR 0016).
    string? VatRate = null,
    string? VatAmount = null,
    // Gider matrahtan düşülebilir mi (ADR 0016). Boş bırakılırsa kategorinin
    // varsayılanı kullanılır; şahsi kayıtta ve gelirde sorulmaz.
    bool? IsTaxDeductible = null);

/// <summary>
/// Tahsilat / ödeme. Kategori ve kapsam **taşımaz**: gelir/gider raporuna
/// girmez, yalnız kasayı değiştirir.
/// </summary>
public sealed record CreateCounterpartyPaymentRequest(
    string Direction,
    string Amount,
    string Currency,
    Guid AccountId,
    string PaymentDate,
    string? Description = null);

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
    VatContract? Vat,
    bool? IsTaxDeductible);

public sealed record CounterpartyPaymentResponse(
    Guid Id,
    Guid CounterpartyId,
    Guid AccountId,
    string Direction,
    string Amount,
    string Currency,
    string PaymentDate,
    string? Description,
    bool IsCancelled);
