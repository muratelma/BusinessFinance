namespace BusinessFinance.Api.Features.FinancialActivities;

public sealed record PlannedActivityResponse(
    Guid PlannedActivityId,
    string PlannedKind,
    string Effect,
    string Timing,
    string Readiness,
    string? AttentionCode,
    string ActionKind,
    string DueDate,

    /// <summary>
    /// Kalemin tutarı; tutarı ödeme gününe kadar bilinmeyen vergide boştur
    /// (ADR 0018 İ5). İstemci "Tutar ödemede girilecek" der, tahmin göstermez.
    /// </summary>
    string? Amount,
    string Currency,
    string Title,
    string? Description,
    Guid? SourceId,
    string? SourceName,
    Guid? CategoryId,
    string? CategoryName,
    bool IsProjected,

    /// <summary>
    /// Bu satır kullanıcının **ödemesi gereken** bir yükümlülük mü.
    /// </summary>
    /// <remarks>
    /// Tekrarlanan gelir ve kullanıcıya ödenecek alacak da planlanan
    /// hareketlerdir ama borç değildir. Ayrımı istemcinin `effect` ve
    /// `actionKind`'dan kendi kurallarıyla türetmesi, aynı kuralın ikinci bir
    /// kopyasını doğururdu; sunucu zaten
    /// <c>PlannedActivityRules.IsPaymentObligation</c> ile biliyor.
    /// </remarks>
    bool IsPaymentObligation,

    /// <summary>
    /// Eylemin çağıracağı uç noktanın adreslediği kayıt; satırın kendi
    /// kimliğinden farklı olabilir. Tekrarlanan satırda occurrence, kart
    /// taksidinde plan, ekstrede kart, borçta borç kimliğidir. Üretilmemiş
    /// tekrarlanan satırda <c>null</c>: gerçekleştirilecek kayıt yoktur.
    /// </summary>
    Guid? ActionTargetId,

    /// <summary>Aggregate içindeki sıra; uç nokta istemiyorsa <c>null</c>.</summary>
    int? ActionSequence,

    /// <summary>
    /// Tekrarlanan satırın planı (üretilmiş olsun olmasın); diğer türlerde
    /// <c>null</c>. "Ödedim" ve "tutar belli oldu" plan + vade ile adreslenir.
    /// </summary>
    Guid? RecurringTransactionId,

    /// <summary>Plan bir vergiyse türü (<c>social-security-premium</c> …); değilse <c>null</c>.</summary>
    string? TaxKind);

/// <summary>
/// No single total is reported: adding planned income, expenses, statements and neutral
/// obligations into one number would mislead. The client gets a count and the nearest
/// due date instead.
/// </summary>
public sealed record PlannedActivityListResponse(
    string AsOfDate,
    int DaysAhead,

    // Hangi kapsamla okunduğu; boşsa toplam. Doluysa kapsamsız satırlar
    // (kart ekstresi) listeye girmez.
    string? Scope,
    int TotalCount,
    string? NearestDueDate,
    IReadOnlyList<PlannedActivityResponse> Items,

    // Pencere içinde vadesi gelmemiş ödeme yükümlülüklerinin toplamı; gelir,
    // tahsilat ve gecikmişler girmez.
    string UpcomingOutgoingTotal,

    // Aynı dilimde tutarı belli olmayan kalem sayısı (ADR 0018 T3): toplam
    // onları tahminle saymaz, istemci "2 kalemin tutarı belli değil" der.
    int UnknownAmountCount,

    // Gecikmiş ödeme yükümlülüklerinin tutarı belli olanlarının toplamı ve
    // tutarı belli olmayanların sayısı; pencereden bağımsız.
    string OverdueOutgoingTotal,
    int OverdueUnknownAmountCount);
