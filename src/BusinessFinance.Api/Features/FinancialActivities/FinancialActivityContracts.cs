using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.FinancialActivities;

public sealed record FinancialActivityResponse(
    Guid ActivityId,
    string ActivityKind,
    string Effect,
    string SourceGroup,
    string Origin,
    string Status,
    string ActivityDate,
    string Amount,
    string Currency,
    string Title,
    string? Description,
    Guid? CategoryId,
    string? CategoryName,
    Guid? SourceId,
    string? SourceName,
    Guid? DestinationId,
    string? DestinationName,
    DateTimeOffset? CancelledAtUtc,

    // Kaydın kapsamı; transfer ve kart ödemesinde boş, çünkü ikisi de
    // gelir/gider raporuna sıfır etki eder ve kapsam taşımaz.
    string? Scope,
    bool CanCancel,
    bool SupportsAttachments,

    /// <summary>
    /// Borç taksidinin anapara ve faiz payı; diğer türlerde <c>null</c>.
    /// Ayrı bir hareket değil, bu hareketin bölünmesidir:
    /// <c>principalPortion + interestPortion = amount</c>.
    /// </summary>
    string? PrincipalPortion,

    /// <inheritdoc cref="PrincipalPortion" />
    string? InterestPortion,

    /// <summary>
    /// Kaydın geldiği POS'un adı (POS satışı ve yatışı); POS seçilmeden
    /// girilende ve karışık yatışta <c>null</c>.
    /// </summary>
    string? ChannelName = null,

    /// <summary>
    /// Kaydın parçası olan gider: POS satışında komisyon, yatışta kesinti.
    /// Ayrı satır değildir; sıfırsa <c>null</c>. <c>amount</c> ondan
    /// etkilenmez.
    /// </summary>
    string? FeeAmount = null,

    /// <summary>POS satışında hesaba geçecek (ya da geçmiş) net tutar.</summary>
    string? NetAmount = null,

    /// <summary>POS satışında paranın beklendiği gün.</summary>
    string? ExpectedTransferDate = null,

    /// <summary>POS satışında paranın hesaba geçtiği gün; yoldaysa <c>null</c>.</summary>
    string? TransferredOn = null,

    /// <summary>Yatışın kapattığı tahsilat sayısı.</summary>
    int? SettlementCount = null);

/// <summary>
/// Bir hareketten hemen sonra hesabın bakiyesi ya da kartın borcu.
/// <c>holder</c>: <c>account</c>, <c>credit-card</c>, <c>counterparty</c>
/// (karşı tarafın açık cari bakiyesi) ya da <c>debt</c> (borcun kalan tutarı).
/// <c>change</c>: hareketin bu bakiyeye ne yaptığı — <c>increased</c>,
/// <c>decreased</c> ya da <c>unchanged</c> (POS satışı: para henüz yolda).
/// <c>availableLimit</c> yalnız kartta, <c>side</c> yalnız cari ve borçta
/// dolar: <c>receivable</c>, <c>payable</c> ya da <c>settled</c>.
/// </summary>
public sealed record ActivityBalanceResponse(
    string Holder,
    Guid Id,
    string Name,
    string Balance,
    string Currency,
    string Change,
    string? AvailableLimit = null,
    string? Side = null);

/// <summary>
/// Hareketin hesabı ya da kartı yoksa, iptal edilmişse ya da ne zaman
/// girildiği bilinmiyorsa (giriş anı tutulmadan önce yazılmış kayıt) liste
/// boştur.
/// </summary>
public sealed record ActivityBalanceListResponse(
    IReadOnlyList<ActivityBalanceResponse> Items);

public sealed record FinancialActivityListResponse(
    IReadOnlyList<FinancialActivityResponse> Items,
    PaginationMetadata Pagination);
