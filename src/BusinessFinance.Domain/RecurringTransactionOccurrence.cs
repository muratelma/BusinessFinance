using System.Globalization;

namespace BusinessFinance.Domain;

public sealed class RecurringTransactionOccurrence
{
    public const int OccurrenceKeyLength = 41;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid RecurringTransactionId { get; }
    public string OccurrenceKey { get; }

    /// <summary>
    /// Kalemin kaynağı. Planın kopyasıdır; ödeme anında kullanıcı başka bir
    /// hesap ya da kart seçerse değişir (ADR 0018 T4). Kaynaksız vergi planının
    /// bekleyen kalemi kaynaksızdır; gerçekleşen kalemin kaynağı her zaman
    /// doludur.
    /// </summary>
    public RecurringSourceType? SourceType { get; private set; }
    public Guid? AccountId { get; private set; }
    public Guid? CreditCardId { get; private set; }
    public Guid CategoryId { get; private set; }
    public CurrencyCode Currency { get; }

    /// <summary>Tutarın ham değeri; kalıcılık bu alanı yazar.</summary>
    public decimal? AmountValue { get; private set; }

    /// <summary>
    /// Bu dönemin tutarı; bilinmiyorsa boştur (ADR 0018 İ5). Gerçekleşen
    /// kalemin tutarı her zaman doludur.
    /// </summary>
    public Money? Amount => AmountValue is decimal value ? new Money(value, Currency) : null;

    public RecurringTransactionKind Kind { get; }

    /// <summary>
    /// Planın kapsamının üretim anındaki kopyası; gerçekleşme anında yeniden
    /// türetilmez.
    /// </summary>
    /// <remarks>
    /// Tutar, tür ve açıklama gibi kapsam da anlık görüntüdür. Plandan
    /// okumak yerine burada tutmak, planlanan görünümün kapsamı bir join
    /// olmadan SQL'de filtreleyebilmesini de sağlar.
    /// </remarks>
    public TransactionScope Scope { get; private set; }

    public DateOnly ScheduledDate { get; }
    public string? Description { get; private set; }
    public RecurringOccurrenceStatus Status { get; private set; }
    public Guid? BudgetTransactionId { get; private set; }
    public Guid? CreditCardChargeId { get; private set; }
    public DateTimeOffset? RealizedAtUtc { get; private set; }

    /// <summary>Kalemi kapatan hesaptan ödenmiş vergi gideri.</summary>
    public Guid? ClosedByTransactionId { get; private set; }

    /// <summary>Kalemi kapatan kartla ödenmiş vergi (kart harcaması).</summary>
    public Guid? ClosedByChargeId { get; private set; }
    public DateTimeOffset? ClosedAtUtc { get; private set; }

    public bool IsRealized => Status == RecurringOccurrenceStatus.Realized;

    private RecurringTransactionOccurrence()
    {
        OccurrenceKey = null!;
    }

    public static RecurringTransactionOccurrence Create(
        Guid id,
        RecurringTransaction recurringTransaction,
        DateOnly scheduledDate)
    {
        if (id == Guid.Empty) throw new ArgumentException("Occurrence id cannot be empty.", nameof(id));
        ArgumentNullException.ThrowIfNull(recurringTransaction);
        if (!recurringTransaction.IsActive || recurringTransaction.NextOccurrenceDate is null)
        {
            throw new InvalidOperationException("Only an active recurring schedule can create an occurrence.");
        }

        if (scheduledDate != recurringTransaction.NextOccurrenceDate.Value)
        {
            throw new InvalidOperationException("Occurrence date must match the next scheduled date.");
        }

        return new RecurringTransactionOccurrence(
            id,
            recurringTransaction.UserId,
            recurringTransaction.Id,
            recurringTransaction.SourceType,
            recurringTransaction.AccountId,
            recurringTransaction.CreditCardId,
            recurringTransaction.CategoryId,
            recurringTransaction.Currency,
            recurringTransaction.AmountValue,
            recurringTransaction.Kind,
            recurringTransaction.Scope,
            scheduledDate,
            recurringTransaction.Description);
    }

    /// <summary>
    /// Yedekten geri yüklenen bir kalemi olduğu gibi kurar; durum alanlarının
    /// birbirini tutması kalıcılıktaki kısıtın aynısıyla denetlenir.
    /// </summary>
    public static RecurringTransactionOccurrence Restore(
        Guid id,
        RecurringTransaction recurringTransaction,
        DateOnly scheduledDate,
        RecurringSourceType? sourceType,
        Guid? accountId,
        Guid? creditCardId,
        Guid categoryId,
        Money? amount,
        TransactionScope scope,
        string? description,
        RecurringOccurrenceStatus status,
        Guid? budgetTransactionId,
        Guid? creditCardChargeId,
        DateTimeOffset? realizedAtUtc,
        Guid? closedByTransactionId,
        Guid? closedByChargeId,
        DateTimeOffset? closedAtUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Occurrence id cannot be empty.", nameof(id));
        ArgumentNullException.ThrowIfNull(recurringTransaction);

        var occurrence = new RecurringTransactionOccurrence(
            id,
            recurringTransaction.UserId,
            recurringTransaction.Id,
            sourceType,
            accountId,
            creditCardId,
            categoryId,
            recurringTransaction.Currency,
            amount?.Amount,
            recurringTransaction.Kind,
            scope,
            scheduledDate,
            description);

        switch (status)
        {
            case RecurringOccurrenceStatus.Planned:
                if (budgetTransactionId is not null || creditCardChargeId is not null || realizedAtUtc is not null ||
                    closedByTransactionId is not null || closedByChargeId is not null || closedAtUtc is not null)
                {
                    throw new ArgumentException("A planned occurrence carries no result.");
                }

                break;
            case RecurringOccurrenceStatus.Realized:
                if (closedByTransactionId is not null || closedByChargeId is not null || closedAtUtc is not null ||
                    realizedAtUtc is null)
                {
                    throw new ArgumentException("A realized occurrence carries exactly its own result.");
                }

                if (budgetTransactionId is Guid transactionId && creditCardChargeId is null)
                {
                    occurrence.RealizeWithTransaction(transactionId, realizedAtUtc.Value);
                }
                else if (creditCardChargeId is Guid chargeId && budgetTransactionId is null)
                {
                    occurrence.RealizeWithCharge(chargeId, realizedAtUtc.Value);
                }
                else
                {
                    throw new ArgumentException("A realized occurrence carries exactly one result.");
                }

                break;
            case RecurringOccurrenceStatus.Closed:
                if (budgetTransactionId is not null || creditCardChargeId is not null || realizedAtUtc is not null ||
                    closedAtUtc is null)
                {
                    throw new ArgumentException("A closed occurrence carries only its closing payment.");
                }

                if (closedByTransactionId is Guid closingTransactionId && closedByChargeId is null)
                {
                    occurrence.CloseWithTransaction(closingTransactionId, closedAtUtc.Value);
                }
                else if (closedByChargeId is Guid closingChargeId && closedByTransactionId is null)
                {
                    occurrence.CloseWithCharge(closingChargeId, closedAtUtc.Value);
                }
                else
                {
                    throw new ArgumentException("A closed occurrence carries exactly one closing payment.");
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, "Occurrence status is not supported.");
        }

        return occurrence;
    }

    /// <summary>
    /// Plan düzenlendiğinde bekleyen kalemi planın yeni hâline getirir.
    /// </summary>
    /// <param name="previousPlanAmount">
    /// Planın düzenlemeden önceki tutarı. Kalemin tutarı buna eşitse plandan
    /// gelmiştir ve planla birlikte değişir; farklıysa kullanıcı bu dönem için
    /// ayrıca yazmıştır ("tutar belli oldu") ve korunur.
    /// </param>
    /// <remarks>
    /// Yalnız bekleyen kalem değişir: ödenmiş ya da kapatılmış kalem geçmiştir.
    /// </remarks>
    public void FollowPlan(RecurringTransaction plan, decimal? previousPlanAmount)
    {
        ArgumentNullException.ThrowIfNull(plan);
        if (plan.Id != RecurringTransactionId || plan.UserId != UserId)
        {
            throw new ArgumentException("The plan does not own this occurrence.", nameof(plan));
        }

        if (Status != RecurringOccurrenceStatus.Planned) return;

        SourceType = plan.SourceType;
        AccountId = plan.AccountId;
        CreditCardId = plan.CreditCardId;
        CategoryId = plan.CategoryId;
        Scope = plan.Scope;
        Description = plan.Description;
        if (AmountValue == previousPlanAmount)
        {
            AmountValue = plan.AmountValue;
        }
    }

    /// <summary>
    /// Ödemenin yapıldığı hesabı seçer; yalnız bekleyen kalemde.
    /// </summary>
    public void UseAccount(Guid accountId)
    {
        EnsurePlanned("Only a planned occurrence can change its source.");
        if (accountId == Guid.Empty) throw new ArgumentException("Account id cannot be empty.", nameof(accountId));
        SourceType = RecurringSourceType.Account;
        AccountId = accountId;
        CreditCardId = null;
    }

    /// <summary>
    /// Ödemenin yapıldığı kartı seçer; kartla ödenen kalem kart harcaması
    /// olarak gerçekleşir. Gelir karta bağlanamaz.
    /// </summary>
    public void UseCreditCard(Guid creditCardId)
    {
        EnsurePlanned("Only a planned occurrence can change its source.");
        if (creditCardId == Guid.Empty) throw new ArgumentException("Card id cannot be empty.", nameof(creditCardId));
        if (Kind == RecurringTransactionKind.Income)
        {
            throw new ArgumentException("A recurring income cannot be sourced from a credit card.", nameof(creditCardId));
        }

        SourceType = RecurringSourceType.CreditCard;
        CreditCardId = creditCardId;
        AccountId = null;
    }

    /// <summary>
    /// Realizes an account-sourced occurrence into a budget transaction. Repeating
    /// the call with the same id is a no-op so a retry cannot create a second result.
    /// </summary>
    public void RealizeWithTransaction(Guid budgetTransactionId, DateTimeOffset realizedAtUtc)
    {
        if (SourceType != RecurringSourceType.Account)
        {
            throw new InvalidOperationException(
                "Only an account occurrence can be realized into a budget transaction.");
        }

        if (TryBeginRealization(budgetTransactionId, BudgetTransactionId, realizedAtUtc, nameof(budgetTransactionId)))
        {
            BudgetTransactionId = budgetTransactionId;
        }
    }

    /// <summary>
    /// Realizes a credit-card occurrence into a card charge. Repeating the call with
    /// the same id is a no-op so a retry cannot create a second result.
    /// </summary>
    public void RealizeWithCharge(Guid creditCardChargeId, DateTimeOffset realizedAtUtc)
    {
        if (SourceType != RecurringSourceType.CreditCard)
        {
            throw new InvalidOperationException(
                "Only a credit-card occurrence can be realized into a credit card charge.");
        }

        if (TryBeginRealization(creditCardChargeId, CreditCardChargeId, realizedAtUtc, nameof(creditCardChargeId)))
        {
            CreditCardChargeId = creditCardChargeId;
        }
    }

    /// <summary>
    /// Kalemi hesaptan ödenmiş toplu bir vergi ödemesiyle kapatır (ADR 0018
    /// T5). Kalemin kendi sonucu olmaz; aynı ödemeyle tekrar kapatmak etkisizdir.
    /// </summary>
    public void CloseWithTransaction(Guid transactionId, DateTimeOffset closedAtUtc)
    {
        if (TryBeginClosing(transactionId, ClosedByTransactionId, closedAtUtc, nameof(transactionId)))
        {
            ClosedByTransactionId = transactionId;
        }
    }

    /// <summary>Kalemi kartla ödenmiş toplu bir vergi ödemesiyle kapatır.</summary>
    public void CloseWithCharge(Guid chargeId, DateTimeOffset closedAtUtc)
    {
        if (TryBeginClosing(chargeId, ClosedByChargeId, closedAtUtc, nameof(chargeId)))
        {
            ClosedByChargeId = chargeId;
        }
    }

    /// <summary>
    /// Kalemi bekleyene döndürür: ödemesi geri alındı ya da kapatan ödeme
    /// iptal edildi (ADR 0018 İ7). Sonuç kaydını iptal etmek çağıranın işidir;
    /// kalem yalnız bağını bırakır. Bekleyen kalemde etkisizdir.
    /// </summary>
    public void Reopen()
    {
        if (Status == RecurringOccurrenceStatus.Planned) return;

        Status = RecurringOccurrenceStatus.Planned;
        BudgetTransactionId = null;
        CreditCardChargeId = null;
        RealizedAtUtc = null;
        ClosedByTransactionId = null;
        ClosedByChargeId = null;
        ClosedAtUtc = null;
    }

    /// <summary>
    /// Gerçekleşmeden önce tutarı kullanıcının gördüğü gerçek tutarla
    /// değiştirir.
    /// </summary>
    /// <remarks>
    /// Plandaki tutar bir <b>beklentidir</b>; bazı kalemlerde her dönem
    /// değişir — elektrik faturası, KDV beyanı, geçici vergi — ve vergide hiç
    /// olmayabilir. Beklentiyi gerçekleşmiş bir hareket olarak yazmak, olmamış
    /// bir tutarı finansal geçmişe koymak olurdu.
    ///
    /// Yalnız <b>bekleyen</b> bir kalem düzeltilebilir: gerçekleşmiş olan
    /// geçmiştir ve düzeltmesi geri alma + yeni ödemedir. Plan değişmez — bu
    /// dönemin tutarıdır, planın tutarı değil ("tutar belli oldu", ADR 0018 T3).
    /// </remarks>
    public void CorrectAmount(Money amount)
    {
        ArgumentNullException.ThrowIfNull(amount);
        EnsurePlanned("Only a planned occurrence can have its amount corrected.");

        if (amount.Currency != Currency)
        {
            throw new ArgumentException(
                "The corrected amount must use the same currency.",
                nameof(amount));
        }

        AmountValue = amount.Amount;
    }

    private void EnsurePlanned(string message)
    {
        if (Status != RecurringOccurrenceStatus.Planned)
        {
            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Shared realization guard. Marks the occurrence realized and returns
    /// <see langword="true"/> when the caller should store the result link.
    /// Returns <see langword="false"/> when the same result already realized this
    /// occurrence, which keeps a retry idempotent instead of writing a second result.
    /// </summary>
    private bool TryBeginRealization(
        Guid resultId,
        Guid? existingResultId,
        DateTimeOffset realizedAtUtc,
        string parameterName)
    {
        if (resultId == Guid.Empty)
        {
            throw new ArgumentException("Realization result id cannot be empty.", parameterName);
        }

        if (realizedAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Realization time must be UTC.", nameof(realizedAtUtc));
        }

        if (existingResultId is Guid existingId)
        {
            if (existingId != resultId)
            {
                throw new InvalidOperationException("Occurrence was already realized by another result.");
            }

            return false;
        }

        // Bir kalem tam olarak tek sonuç taşır (İ7): kapatılmış kalem ikinci
        // bir ödemeyle gerçekleşemez.
        EnsurePlanned("Occurrence was already settled.");
        if (AmountValue is null)
        {
            throw new InvalidOperationException("An occurrence without an amount cannot be realized.");
        }

        RealizedAtUtc = realizedAtUtc;
        Status = RecurringOccurrenceStatus.Realized;
        return true;
    }

    private bool TryBeginClosing(
        Guid paymentId,
        Guid? existingPaymentId,
        DateTimeOffset closedAtUtc,
        string parameterName)
    {
        if (paymentId == Guid.Empty)
        {
            throw new ArgumentException("Closing payment id cannot be empty.", parameterName);
        }

        if (closedAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Closing time must be UTC.", nameof(closedAtUtc));
        }

        if (existingPaymentId is Guid existingId)
        {
            if (existingId != paymentId)
            {
                throw new InvalidOperationException("Occurrence was already closed by another payment.");
            }

            return false;
        }

        EnsurePlanned("Occurrence was already settled.");
        ClosedAtUtc = closedAtUtc;
        Status = RecurringOccurrenceStatus.Closed;
        return true;
    }

    public TransactionType GetTransactionType()
    {
        return Kind == RecurringTransactionKind.Income
            ? TransactionType.Income
            : TransactionType.Expense;
    }

    public static string CreateOccurrenceKey(Guid recurringTransactionId, DateOnly scheduledDate)
    {
        if (recurringTransactionId == Guid.Empty)
        {
            throw new ArgumentException("Recurring transaction id cannot be empty.", nameof(recurringTransactionId));
        }

        if (scheduledDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(scheduledDate), "Scheduled date is required.");
        }

        return $"{recurringTransactionId:N}:{scheduledDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}";
    }

    private RecurringTransactionOccurrence(
        Guid id,
        Guid userId,
        Guid recurringTransactionId,
        RecurringSourceType? sourceType,
        Guid? accountId,
        Guid? creditCardId,
        Guid categoryId,
        CurrencyCode currency,
        decimal? amountValue,
        RecurringTransactionKind kind,
        TransactionScope scope,
        DateOnly scheduledDate,
        string? description)
    {
        var validSource = sourceType switch
        {
            RecurringSourceType.Account => accountId is not null && creditCardId is null,
            RecurringSourceType.CreditCard => creditCardId is not null && accountId is null,
            null => accountId is null && creditCardId is null,
            _ => false
        };
        if (!validSource)
        {
            throw new ArgumentException(
                "An occurrence must have at most one source: an account or a credit card.",
                nameof(sourceType));
        }

        if (amountValue is decimal value)
        {
            // Para kuralını (sıfırdan büyük, desteklenen para birimi) tek yerde tutar.
            _ = new Money(value, currency);
        }

        Id = id;
        UserId = userId;
        RecurringTransactionId = recurringTransactionId;
        OccurrenceKey = CreateOccurrenceKey(recurringTransactionId, scheduledDate);
        SourceType = sourceType;
        AccountId = accountId;
        CreditCardId = creditCardId;
        CategoryId = categoryId;
        Currency = currency;
        AmountValue = amountValue;
        Kind = kind;
        Scope = TransactionScopeGuard.Validate(scope, nameof(scope));
        ScheduledDate = scheduledDate;
        Description = description;
        Status = RecurringOccurrenceStatus.Planned;
    }
}
