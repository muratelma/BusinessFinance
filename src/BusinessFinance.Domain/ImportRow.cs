namespace BusinessFinance.Domain;

public sealed class ImportRow
{
    public const int MaximumRawDataLength = 4000;
    public const int MaximumErrorLength = 2000;
    public const int MaximumExternalReferenceLength = 200;
    public const decimal MaximumAbsoluteAmount = 999999999999999.9999m;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid ImportBatchId { get; }
    public int RowNumber { get; }
    public string RawData { get; }
    public DateOnly? TransactionDate { get; private set; }
    public decimal? SignedAmount { get; private set; }
    public CurrencyCode Currency { get; }
    public string? Description { get; private set; }
    public string? ExternalReference { get; private set; }
    public Guid? AccountId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public ImportRowStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Guid? BudgetTransactionId { get; private set; }
    public Guid? DuplicateTransactionId { get; private set; }
    public ImportDuplicateReason? DuplicateReason { get; private set; }

    private ImportRow()
    {
        RawData = null!;
    }

    public ImportRow(
        Guid id,
        Guid userId,
        Guid importBatchId,
        int rowNumber,
        string rawData,
        DateOnly? transactionDate,
        decimal? signedAmount,
        CurrencyCode currency,
        string? description,
        string? externalReference,
        IReadOnlyCollection<string> errors)
    {
        if (id == Guid.Empty) throw new ArgumentException("Import row id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (importBatchId == Guid.Empty) throw new ArgumentException("Import batch id cannot be empty.", nameof(importBatchId));
        if (rowNumber < 2) throw new ArgumentOutOfRangeException(nameof(rowNumber), "The header is row 1.");
        if (rawData.Length > MaximumRawDataLength) throw new ArgumentException("Raw row is too long.", nameof(rawData));
        if (currency != CurrencyCode.TRY) throw new ArgumentOutOfRangeException(nameof(currency));
        if (description?.Length > BudgetTransaction.MaximumDescriptionLength)
            throw new ArgumentException("Description is too long.", nameof(description));
        if (externalReference?.Length > MaximumExternalReferenceLength)
            throw new ArgumentException("External reference is too long.", nameof(externalReference));

        var normalizedErrors = errors.Where(error => !string.IsNullOrWhiteSpace(error))
            .Select(error => error.Trim()).ToList();
        if (transactionDate is null) normalizedErrors.Add("Transaction date is required.");
        if (signedAmount is null or 0m) normalizedErrors.Add("A non-zero signed amount is required.");
        if (signedAmount is decimal value && decimal.Round(value, 4) != value)
            normalizedErrors.Add("Signed amount cannot have more than four decimal places.");
        if (signedAmount is decimal boundedValue && decimal.Abs(boundedValue) > MaximumAbsoluteAmount)
            normalizedErrors.Add("Signed amount exceeds decimal(19,4) storage limits.");
        var distinctErrors = normalizedErrors.Distinct(StringComparer.Ordinal).ToArray();
        var errorMessage = distinctErrors.Length == 0 ? null : string.Join(" | ", distinctErrors);
        if (errorMessage?.Length > MaximumErrorLength) errorMessage = errorMessage[..MaximumErrorLength];

        Id = id;
        UserId = userId;
        ImportBatchId = importBatchId;
        RowNumber = rowNumber;
        RawData = rawData;
        TransactionDate = transactionDate;
        SignedAmount = signedAmount is decimal acceptedAmount &&
                       acceptedAmount != 0m &&
                       decimal.Round(acceptedAmount, 4) == acceptedAmount &&
                       decimal.Abs(acceptedAmount) <= MaximumAbsoluteAmount
            ? acceptedAmount
            : null;
        Currency = currency;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim();
        Status = errorMessage is null ? ImportRowStatus.Valid : ImportRowStatus.Invalid;
        ErrorMessage = errorMessage;
    }

    public void ApplyCorrection(
        DateOnly transactionDate,
        decimal signedAmount,
        string? description,
        string? externalReference,
        Account account,
        Category category)
    {
        if (Status is ImportRowStatus.Imported or ImportRowStatus.SkippedDuplicate)
            throw new InvalidOperationException("A resolved row cannot be changed.");
        if (transactionDate == default)
            throw new ArgumentOutOfRangeException(nameof(transactionDate), "Transaction date is required.");
        if (signedAmount == 0m || decimal.Round(signedAmount, 4) != signedAmount ||
            decimal.Abs(signedAmount) > MaximumAbsoluteAmount)
            throw new ArgumentOutOfRangeException(nameof(signedAmount),
                "Signed amount must fit decimal(19,4), be non-zero, and have at most four decimal places.");
        if (description?.Trim().Length > BudgetTransaction.MaximumDescriptionLength)
            throw new ArgumentException("Description is too long.", nameof(description));
        if (externalReference?.Trim().Length > MaximumExternalReferenceLength)
            throw new ArgumentException("External reference is too long.", nameof(externalReference));
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(category);
        if (account.UserId != UserId || category.UserId != UserId)
            throw new ArgumentException("Account and category must belong to the import owner.");
        if (!account.IsActive || !category.IsActive)
            throw new InvalidOperationException("Account and category must be active.");

        var requiredCategoryType = signedAmount > 0m ? CategoryType.Income : CategoryType.Expense;
        if (category.Type != requiredCategoryType)
            throw new ArgumentException("Category type must match the signed amount direction.", nameof(category));

        TransactionDate = transactionDate;
        SignedAmount = signedAmount;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim();
        AccountId = account.Id;
        CategoryId = category.Id;
        ErrorMessage = null;
        Status = ImportRowStatus.Ready;
        DuplicateTransactionId = null;
        DuplicateReason = null;
    }

    public void FlagDuplicate(Guid transactionId, ImportDuplicateReason reason)
    {
        if (Status != ImportRowStatus.Ready)
            throw new InvalidOperationException("Only a ready row can enter duplicate review.");
        if (transactionId == Guid.Empty) throw new ArgumentException("Duplicate transaction id cannot be empty.", nameof(transactionId));
        if (!Enum.IsDefined(reason)) throw new ArgumentOutOfRangeException(nameof(reason));
        DuplicateTransactionId = transactionId;
        DuplicateReason = reason;
        Status = ImportRowStatus.PendingDuplicateReview;
    }

    public void ResolveDuplicate(bool importAnyway)
    {
        if (Status != ImportRowStatus.PendingDuplicateReview)
            throw new InvalidOperationException("Only a pending duplicate can be resolved.");
        Status = importAnyway ? ImportRowStatus.Ready : ImportRowStatus.SkippedDuplicate;
    }

    /// <summary>
    /// Onaylanmış satırdan gerçek hareketi üretir. Kapsam satırda taşınmaz,
    /// çağıran türetip verir: CSV dosyasında kapsam kolonu olmayabilir ve
    /// satırın kendisi hesabı/kategoriyi okuyup karar verecek yerde değildir.
    /// </summary>
    public BudgetTransaction CreateTransaction(
        Account account,
        Category category,
        TransactionScope scope,
        Guid transactionId)
    {
        if (Status != ImportRowStatus.Ready || TransactionDate is null || SignedAmount is null)
            throw new InvalidOperationException("Only a ready import row can create a transaction.");
        if (account.Id != AccountId || category.Id != CategoryId)
            throw new InvalidOperationException("Confirmed mappings must match the staged candidate.");

        return new BudgetTransaction(
            transactionId,
            UserId,
            account,
            category,
            new Money(decimal.Abs(SignedAmount.Value), Currency),
            SignedAmount.Value > 0m ? TransactionType.Income : TransactionType.Expense,
            scope,
            TransactionDate.Value,
            Description);
    }

    public void MarkImported(Guid transactionId)
    {
        if (Status != ImportRowStatus.Ready)
            throw new InvalidOperationException("Only a ready row can be marked imported.");
        if (transactionId == Guid.Empty)
            throw new ArgumentException("Transaction id cannot be empty.", nameof(transactionId));
        BudgetTransactionId = transactionId;
        Status = ImportRowStatus.Imported;
    }
}
