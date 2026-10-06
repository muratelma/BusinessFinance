namespace BusinessFinance.Api.Features.Cash;

public sealed record CreateCashCountRequest(
    Guid AccountId,
    string CountedAmount,
    string CountDate,
    string? Scope = null,
    string? Note = null);

// `unknownReason` yalnız eksik farkta ve kategorisiz gönderilir: kayıt standart
// `Kasa farkı` gider kategorisine yazılır (yoksa açılır). Diğer durumda
// `categoryId` zorunludur.
public sealed record ConfirmCashCountDifferenceRequest(
    Guid? CategoryId = null,
    bool UnknownReason = false);

public sealed record CashCountResponse(
    Guid Id,
    Guid AccountId,
    string AccountName,
    string CountDate,
    string CountedAmount,
    string Currency,
    string Scope,
    string? Note,
    bool IsCancelled,
    Guid? AdjustmentTransactionId,
    string? ExpectedBalance = null,
    string? Difference = null);

public sealed record CashCountListResponse(IReadOnlyList<CashCountResponse> Items);

public sealed record CashCountTodayResponse(
    Guid AccountId,
    string AccountName,
    string ExpectedBalance,
    string Currency,
    CashCountResponse? Count,

    // Beklenen tutarın nereden geldiği: son sayım ve bugünkü nakit akışı.
    CashCountResponse? PreviousCount,
    string TodayInflow,
    string TodayOutflow,

    // Sayımdan bu yana kasa bakiyesindeki değişim; bilinmiyorsa boş.
    string? ChangeSinceCount = null,

    // Önceki sayımın kaydedilmemiş farkı (işaretli); yoksa boş. Yalnız bilgi:
    // bugünkü farktan düşülmez.
    string? PreviousUnrecordedDifference = null,

    // Bugünkü açık fark önceki sayımın kaydedilmemiş farkına eşit.
    bool DifferenceSameAsPrevious = false);
