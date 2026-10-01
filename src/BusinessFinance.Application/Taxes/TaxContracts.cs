using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Taxes;

/// <summary>
/// Tanımlı bir verginin bir kalemi: plan ve vadesi. Kalem henüz üretilmemiş
/// olabilir; gün ve planla adreslenir.
/// </summary>
public sealed record TaxItemReference(Guid RecurringTransactionId, DateOnly ScheduledDate);

/// <summary>
/// "Vergi ödemesi ekle" (ADR 0018 T5, İ4): tek tutarla ödenmiş vergi.
/// </summary>
/// <param name="ClientRequestId">
/// İstemcinin bu isteğe verdiği kimlik. Aynı kimlikle tekrar gönderilen istek
/// ikinci bir gider yazmaz, ilk yazılanı döner.
/// </param>
/// <param name="Closes">
/// Bu ödemenin kapattığı tanımlı kalemler; boş olabilir (tanımsız ödeme).
/// Tutar kalemlere dağıtılmaz ve eşleştirilmez.
/// </param>
public sealed record CreateTaxPaymentCommand(
    Guid ClientRequestId,
    decimal Amount,
    DateOnly PaidOn,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid CategoryId,
    TransactionScope? Scope,
    string? Note,
    IReadOnlyList<TaxItemReference> Closes);

/// <summary>Bir ödemenin ödediği ya da kapattığı tanımlı vergi kalemi.</summary>
public sealed record TaxSettledItemDto(
    Guid OccurrenceId,
    Guid RecurringTransactionId,
    DateOnly ScheduledDate,
    string? Name,
    TaxKind? TaxKind);

/// <summary>
/// Ödenmiş bir vergi: vergi işaretli bir kategorideki gider ya da kart
/// harcaması (ADR 0018 T6). Ayrı bir kayıt türü değildir; kimliği giderin ya
/// da harcamanın kimliğidir.
/// </summary>
/// <param name="RealizedItem">
/// Bu ödeme bir kalemin "Ödedim" sonucuysa o kalem; geri alma kalemi bekleyene
/// döndürür.
/// </param>
/// <param name="ClosedItems">Toplu ödemenin kapattığı kalemler.</param>
public sealed record TaxPaymentDto(
    Guid PaymentId,
    RecurringSourceType SourceType,
    Guid SourceId,
    string SourceName,
    Guid CategoryId,
    string CategoryName,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly PaidOn,
    string? Description,
    TransactionScope Scope,
    TaxSettledItemDto? RealizedItem,
    IReadOnlyList<TaxSettledItemDto> ClosedItems,
    bool IsCancelled);

public sealed record TaxPaymentPage(IReadOnlyList<TaxPaymentDto> Items, bool HasMore);

/// <summary>
/// Vergi ekranının tek okuması: tanımlı vergiler, bekleyenler ve son ödenenler.
/// </summary>
/// <param name="Pending">
/// Tanımlı vergilerin gecikmiş ve pencere içindeki kalemleri. Planlanan
/// projection'dan okunur (ADR 0018 İ6); ikinci bir sorgu yoktur.
/// </param>
/// <param name="PendingTotal">
/// Bekleyenlerin tutarı belli olanlarının toplamı; <b>gecikenler dahil</b>.
/// Kart "Gecikenler ve 30 gün"ü gösterir ve toplamı da aynı listeyi okur.
/// </param>
/// <param name="PendingUnknownAmountCount">
/// Bekleyenlerden tutarı belli olmayanların sayısı (gecikenler dahil); toplama
/// tahminle katılmazlar (ADR 0018 İ5).
/// </param>
public sealed record TaxOverviewDto(
    DateOnly AsOfDate,
    int DaysAhead,
    IReadOnlyList<RecurringTransactionDto> Plans,
    IReadOnlyList<PlannedActivityDto> Pending,
    decimal PendingTotal,
    int PendingUnknownAmountCount,
    IReadOnlyList<TaxPaymentDto> RecentPayments,
    bool HasMorePayments);

/// <summary>Bir vergi kaleminin geçmişi: ödendi ya da kapatıldı.</summary>
/// <param name="Amount">
/// Kalemin kendi beklenen tutarı (tanımdan ya da "tutar belli oldu"dan); toplu
/// ödemeyle kapatılan kalemde ödenen tutar kalemlere dağıtılmadığı için tek
/// bilinen tutar budur. Tutarsız vergide boştur.
/// </param>
public sealed record TaxPlanHistoryItemDto(
    Guid OccurrenceId,
    DateOnly ScheduledDate,
    RecurringOccurrenceStatus Status,
    decimal? Amount,
    TaxPaymentDto Payment);

public sealed record TaxPlanDetailDto(
    RecurringTransactionDto Plan,
    IReadOnlyList<PlannedActivityDto> Upcoming,
    IReadOnlyList<TaxPlanHistoryItemDto> History);

public enum TaxPaymentSaveResult
{
    Saved = 1,

    /// <summary>Aynı istek kimliğiyle başka bir istek önce yazdı.</summary>
    Duplicate = 2,

    /// <summary>Kapatılan kalemlerden biri aynı anda değişti.</summary>
    Conflict = 3
}

/// <summary>
/// Vergi ödemesinin yazılması ve okunması. Ödeme yeni bir tablo değildir;
/// gider ya da kart harcamasıdır ve kapattığı kalemlerle tek <c>SaveChanges</c>
/// sınırında yazılır.
/// </summary>
public interface ITaxPaymentRepository
{
    Task<TaxPaymentSaveResult> AddAsync(
        BudgetTransaction? transaction,
        CreditCardCharge? charge,
        CancellationToken cancellationToken);

    /// <summary>
    /// İzlenen değişiklikleri (iptal edilen ödeme, bekleyene dönen kalemler)
    /// yazar; sürüm çakışmasında <see langword="false"/>.
    /// </summary>
    Task<bool> TrySaveAsync(CancellationToken cancellationToken);

    Task<TaxPaymentDto?> FindAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken);

    /// <summary>
    /// Vergi işaretli kategorilerdeki iptal edilmemiş giderler ve kart
    /// harcamaları, en yeniden eskiye.
    /// </summary>
    Task<TaxPaymentPage> ListAsync(Guid userId, int skip, int take, CancellationToken cancellationToken);

    Task<IReadOnlyList<TaxPlanHistoryItemDto>> ListPlanHistoryAsync(
        Guid userId,
        Guid recurringTransactionId,
        CancellationToken cancellationToken);
}

public static class TaxErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static ApplicationError Validation(string message) => new(
        "tax_payments.validation",
        message,
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidSource = new(
        "tax_payments.invalid_source",
        "Exactly one of account or credit card must be supplied.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError AccountUnavailable = new(
        "tax_payments.account_unavailable",
        "The account was not found or is inactive.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CardUnavailable = new(
        "tax_payments.card_unavailable",
        "The credit card was not found or is inactive.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CardLimitInsufficient = new(
        "tax_payments.card_limit_insufficient",
        "The available credit card limit is not enough for this payment.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Ödenen vergi, vergi işaretli bir gider kategorisine yazılır; aksi hâlde
    /// vergi ekranının "Ödenenler" listesinde görünmezdi (ADR 0018 T6).
    /// </summary>
    public static readonly ApplicationError CategoryNotTax = new(
        "tax_payments.category_not_tax",
        "A tax payment requires an active expense category marked as tax.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError PaidOnInFuture = new(
        "tax_payments.paid_on_in_future",
        "The payment date cannot be in the future.",
        ApplicationErrorType.Validation);

    public static ApplicationError ItemNotFound(Guid recurringTransactionId) => new(
        "tax_payments.item_not_found",
        $"No item of tax plan '{recurringTransactionId}' falls on the requested date.",
        ApplicationErrorType.NotFound);

    /// <summary>
    /// Kapatılacak kalem bir vergi planının değil; toplu ödeme yalnız tanımlı
    /// vergileri kapatır. Vergi türü boş eski planlar listelenmez.
    /// </summary>
    public static readonly ApplicationError ItemNotTax = new(
        "tax_payments.item_not_tax",
        "Only items of a tax plan can be closed by a tax payment.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ItemInactive = new(
        "tax_payments.item_inactive",
        "The tax plan is paused, so its items cannot be closed.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Kalem zaten ödendi ya da kapatıldı; bir kalem tek sonuç taşır (İ7).
    /// </summary>
    public static readonly ApplicationError ItemNotPending = new(
        "tax_payments.item_not_pending",
        "The item was already paid or closed.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError ConcurrentChange = new(
        "tax_payments.concurrent_change",
        "An item was changed by another request. Refresh and try again.",
        ApplicationErrorType.Conflict);

    public static ApplicationError PaymentNotFound(Guid paymentId) => new(
        "tax_payments.not_found",
        $"Tax payment '{paymentId}' was not found.",
        ApplicationErrorType.NotFound);

    /// <summary>
    /// Kayıt bir taksit planının sonucudur; geri alması kendi ekranındandır.
    /// </summary>
    public static readonly ApplicationError UndoOriginLocked = new(
        "tax_payments.undo_origin_locked",
        "This record belongs to an installment plan and cannot be undone here.",
        ApplicationErrorType.Conflict);

    public static ApplicationError PlanNotFound(Guid recurringTransactionId) => new(
        "taxes.plan_not_found",
        $"Tax plan '{recurringTransactionId}' was not found.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError InvalidAsOfDate = new(
        "taxes.invalid_as_of_date",
        "As-of date is outside the supported range.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidDaysAhead = new(
        "taxes.invalid_days_ahead",
        "Days ahead must be 7, 30 or 90.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError PlansEmpty = new(
        "taxes.plans_empty",
        "At least one tax plan is required.",
        ApplicationErrorType.Validation);

    public static ApplicationError TooManyPlans(int maximum) => new(
        "taxes.too_many_plans",
        $"At most {maximum} tax plans can be created at once.",
        ApplicationErrorType.Validation);

    /// <summary>Toplu tanımlama yalnız vergi türü dolu planları kurar.</summary>
    public static readonly ApplicationError PlanNotTax = new(
        "taxes.plan_not_tax",
        "Every plan in a batch must carry a tax kind.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidPage = new(
        "tax_payments.invalid_page",
        "Skip cannot be negative and take must be between 1 and 100.",
        ApplicationErrorType.Validation);
}
