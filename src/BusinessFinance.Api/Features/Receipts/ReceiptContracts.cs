using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.Receipts;

public sealed record ReceiptWarningResponse(string Code, string Message);

/// <summary>
/// An editable suggestion. Money remains a string at the HTTP boundary so the
/// mobile client never passes a financial amount through a binary float.
/// </summary>
public sealed record ReceiptAnalysisResponse(
    string DocumentKind,
    string? CounterpartyName,
    string CounterpartyState,

    /// <summary>
    /// Okunan adın eşleştiği mevcut karşı taraf; <b>yalnız bir öneri</b>.
    /// Dolu olması hiçbir şeyin yazıldığı anlamına gelmez (ADR 0011) ve
    /// kullanıcı reddedebilir. Boş olması da hata değildir: o adla ilk kez
    /// iş yapılıyor olabilir.
    /// </summary>
    Guid? CounterpartyId,
    string? PurchasedAt,
    string PurchasedAtState,

    /// <summary>
    /// Faturanın son ödeme tarihi; yalnız faturalarda dolu. Dolu olması
    /// istemciye "ödendi mi?" diye sordurur: ödenmemiş fatura gider değil,
    /// planlanan ödemedir.
    /// </summary>
    string? DueDate,
    string DueDateState,
    string? TotalAmount,
    string TotalAmountState,

    /// <summary>
    /// Belgede yazan KDV; okunamadıysa <c>null</c>. Kaydın KDV alanıyla
    /// <b>aynı</b> biçimdedir (<c>rate</c> + <c>amount</c>, dört ondalıklı
    /// string), çünkü istemci bunu doğrudan forma taşır.
    /// </summary>
    /// <remarks>
    /// ADR 0016: taşınır, hesaplanmaz. Oran ile tutar bağımsızdır ve ikisi de
    /// tek başına dolabilir — market fişi toplam KDV'yi basar ama tek bir oranı
    /// yoktur (%1, %10, %20 aynı fişte), hizmet faturasında ise oran basılıyken
    /// tutar okunamayabilir. Eksik olan boş kalır, diğerinden üretilmez.
    /// </remarks>
    VatContract? Vat,
    string VatState,
    string? FeeAmount,
    string FeeAmountState,

    /// <summary>
    /// Fişte yazan taksit sayısı; iki ve üstü, tek çekimde <c>null</c>. Dolu
    /// olduğunda istemci tek seferlik tam tutar gideri yerine taksit planı
    /// önerir — o ay bütçeden çıkan yalnız bir taksittir.
    /// </summary>
    int? InstallmentCount,
    string? CurrencyCode,
    string PaymentHint,
    Guid? CategoryId,
    string? CategoryName,
    string CategoryState,
    IReadOnlyList<ReceiptWarningResponse> Warnings,

    /// <summary>
    /// İade fişinde, geri verildiği anlaşılan harcama; bulunamadıysa
    /// <c>null</c>. Dolu olması bile bir **öneridir** — iptal ancak kullanıcı
    /// kaydı görüp onayladığında olur ve bu uç nokta hiçbir şey yazmaz.
    /// </summary>
    ReceiptRefundMatchResponse? RefundMatch);

/// <param name="RemainingAmount">
/// İade kısmiyse harcamanın ne olması gerektiği; tam iadede <c>null</c> ve
/// iptal tek başına yeterlidir. Sunucuda hesaplanır: istemci finansal toplamı
/// ikinci kez hesaplamaz.
/// </param>
public sealed record ReceiptRefundMatchResponse(
    Guid TransactionId,
    string TransactionDate,
    string Amount,
    string? Description,
    string? RemainingAmount);
