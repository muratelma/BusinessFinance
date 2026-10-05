using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Pos;

/// <summary>
/// Yeni bir POS tahsilatı.
/// </summary>
/// <remarks>
/// Komisyon ya tutar ya oran olarak gelir, <b>ikisi birden değil</b>: ikisi de
/// gönderilseydi hangisinin doğru olduğu sorusu doğardı ve iki gerçek arasında
/// seçim yapmak sunucunun işi değildir. Oran gelirse tutara çevrilir ve
/// saklanan tek şey tutardır (ADR 0009'un aynı kararı).
///
/// <see cref="PosDefinitionId"/> verilirse boş bırakılan alanlar tanımdan
/// dolar (ADR 0019 T4): hesap, satış kategorisi, komisyon (tanımın oranıyla),
/// komisyon kategorisi ve beklenen gün. Açıkça gönderilen alan tanımı ezer.
/// Tanım verilmezse hesap, kategori ve beklenen gün zorunludur.
/// </remarks>
public sealed record CreatePosSettlementCommand(
    Guid? AccountId,
    Guid? CategoryId,
    decimal GrossAmount,
    CurrencyCode Currency,
    decimal? CommissionAmount,
    decimal? CommissionRate,
    Guid? CommissionCategoryId,
    TransactionScope? Scope,
    DateOnly SettlementDate,
    DateOnly? ExpectedTransferDate,
    string? Description,
    Guid? PosDefinitionId = null);

public sealed record CancelPosSettlementCommand(Guid SettlementId);

public sealed record PosSettlementListCriteria(bool InTransitOnly, DateOnly From, DateOnly To);

public sealed record PosSettlementDto(
    Guid Id,
    Guid AccountId,
    string AccountName,
    Guid CategoryId,
    string CategoryName,
    Guid? CommissionCategoryId,
    string? CommissionCategoryName,
    decimal GrossAmount,
    decimal CommissionAmount,
    decimal NetAmount,
    decimal CommissionRate,
    CurrencyCode Currency,
    TransactionScope Scope,
    DateOnly SettlementDate,
    DateOnly ExpectedTransferDate,
    DateOnly? TransferredOn,
    string? Description,
    bool IsInTransit,
    bool IsCancelled,
    // Beklenen gün geçti, para hâlâ gelmedi. Kalıcı değil, okurken türetilir.
    bool IsLate,
    // Tahsilatın yazıldığı POS tanımı; tanımsız girilende boştur.
    Guid? PosDefinitionId = null,
    string? PosDefinitionName = null,
    // Parayı hesaba geçiren yatış; para yoldaysa boştur.
    Guid? PosDepositId = null,
    // Tahsilatı üreten gün sonu; tek tek girilende boştur.
    Guid? DayCloseId = null);

/// <summary>
/// Tahsilat listesi ve yanında yoldaki toplam.
/// </summary>
/// <remarks>
/// <see cref="MoneyInTransit"/> <b>tarih penceresinden bağımsızdır</b>: yolda
/// olan paranın toplamı, kullanıcının hangi aya baktığından etkilenmemelidir.
/// Pencereye göre daralsaydı geçen ayın tahsilatına bakan kullanıcı yoldaki
/// parasının bir kısmını yok sayardı.
/// </remarks>
public sealed record PosSettlementListDto(
    IReadOnlyList<PosSettlementDto> Items,
    decimal MoneyInTransit,
    int InTransitCount);

public interface IPosSettlementRepository
{
    Task AddAsync(PosSettlement settlement, CancellationToken cancellationToken);

    Task<PosSettlementListDto> ListAsync(
        Guid userId,
        PosSettlementListCriteria criteria,
        CancellationToken cancellationToken);

    Task<PosSettlement?> FindOwnedByIdAsync(
        Guid settlementId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);

    Task SaveAsync(CancellationToken cancellationToken);
}
