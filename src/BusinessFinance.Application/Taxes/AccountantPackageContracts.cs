using BusinessFinance.Application.DataPortability;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Taxes;

/// <summary>
/// Muhasebeciye giden paketin bir satırı: ayın <b>işletme</b> kapsamlı bir
/// gelir veya gideri.
/// </summary>
/// <param name="Source">
/// Kaydın hangi yazma modelinden geldiği; kararlı makine değeri
/// (<c>transaction</c>, <c>card-charge</c>, <c>counterparty-charge</c>,
/// <c>obligation</c>, <c>pos-sale</c>, <c>pos-commission</c>,
/// <c>debt-opening</c>, <c>debt-interest</c>). Kullanıcıya gösterilecek cümleyi
/// istemci kurar.
/// </param>
/// <param name="IsTaxDeductible">
/// Boş olması üçüncü bir durum değil, sorunun <b>cevaplanmamış</b> olmasıdır;
/// paket bunu ayrı sayar ki muhasebeci neyi soracağını bilsin.
/// </param>
public sealed record AccountantPackageLineDto(
    string Source,
    Guid SourceId,
    DateOnly Date,
    TransactionType Type,
    decimal Amount,
    string? CategoryName,
    string? CounterpartyName,
    string? Description,
    decimal? VatRate,
    decimal? VatAmount,
    bool? IsTaxDeductible,
    int AttachmentCount);

/// <summary>
/// Paketin taşıdığı belge eki.
/// </summary>
/// <param name="IsIncluded">
/// Dosya paketin içine kondu mu. Paket bir boyut tavanı taşır; tavanı aşan ek
/// <b>listede kalır ama dosyası konmaz</b> — sessizce düşürmek, muhasebeciye
/// eksik bir paketi tam sanarak göndermek olurdu.
/// </param>
public sealed record AccountantPackageAttachmentDto(
    Guid Id,
    Guid TransactionId,
    string FileName,
    string ContentType,
    long SizeBytes,
    bool IsIncluded);

/// <summary>
/// Ay sonu muhasebeci paketi.
/// </summary>
/// <remarks>
/// Toplamlar <b>aynı ayın işletme raporundan</b> gelir; paket ikinci bir
/// hesaplama yolu açmaz. Satır listesi o toplamın dökümüdür ve toplamına eşit
/// olduğu testle sabitlenir — ayrıştıkları gün hangisinin doğru olduğunu kimse
/// bilemezdi.
///
/// Pakete <b>şahsi hiçbir kayıt girmez</b>. Bu bir yorum değil, çıkış
/// koşuludur.
/// </remarks>
public sealed record AccountantPackageDto(
    int Year,
    int Month,
    CurrencyCode Currency,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Net,
    decimal VatOnIncome,
    decimal VatOnExpense,
    int LinesWithoutVat,
    decimal NonDeductibleExpense,
    int NonDeductibleCount,
    int DeductibilityUnansweredCount,
    IReadOnlyList<AccountantPackageLineDto> Lines,
    IReadOnlyList<AccountantPackageAttachmentDto> Attachments);

public interface IAccountantPackageRepository
{
    /// <summary>
    /// Ayın işletme kapsamlı gelir/gider satırları.
    /// </summary>
    Task<IReadOnlyList<AccountantPackageLineDto>> ListBusinessLinesAsync(
        Guid userId,
        int year,
        int month,
        CancellationToken cancellationToken);

    /// <summary>
    /// Satırlara bağlı belge ekleri; boyut tavanını aşanlar
    /// <see cref="AccountantPackageAttachmentDto.IsIncluded"/> ile işaretlenir.
    /// </summary>
    Task<IReadOnlyList<AccountantPackageAttachmentDto>> ListAttachmentsAsync(
        Guid userId,
        IReadOnlyCollection<Guid> transactionIds,
        CancellationToken cancellationToken);

    /// <summary>
    /// Paketi tek dosyaya yazar.
    /// </summary>
    /// <remarks>
    /// Toplamlar parametre olarak gelir: dosya da ekran da aynı rapordan
    /// beslenir, dosya kendi toplamını hesaplamaz.
    /// </remarks>
    Task<PortableFile> BuildPackageAsync(
        Guid userId,
        AccountantPackageDto package,
        CancellationToken cancellationToken);
}
