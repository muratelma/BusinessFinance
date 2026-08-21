namespace BusinessFinance.Domain;

public sealed class ImportBatch
{
    public const int MaximumFileNameLength = 255;
    public const int MaximumEncodingNameLength = 32;
    public const int MaximumColumnNameLength = 128;
    public const int FingerprintLength = 64;

    private readonly List<ImportRow> _rows = [];

    public Guid Id { get; }
    public Guid UserId { get; }
    public string FileName { get; }
    public string FileFingerprint { get; }
    public long FileSizeBytes { get; }
    public string EncodingName { get; }
    public string Delimiter { get; }
    public string DateColumn { get; }
    public string AmountColumn { get; }
    public string? DescriptionColumn { get; }
    public string? ReferenceColumn { get; }
    public string DateFormat { get; }
    public string DecimalSeparator { get; }
    public ImportBatchStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; }
    public int TotalRowCount => _rows.Count;
    public int ValidRowCount => _rows.Count(row => row.Status != ImportRowStatus.Invalid);
    public int InvalidRowCount => _rows.Count(row => row.Status == ImportRowStatus.Invalid);
    public IReadOnlyCollection<ImportRow> Rows => _rows.AsReadOnly();

    private ImportBatch()
    {
        FileName = null!;
        FileFingerprint = null!;
        EncodingName = null!;
        Delimiter = null!;
        DateColumn = null!;
        AmountColumn = null!;
        DateFormat = null!;
        DecimalSeparator = null!;
    }

    public ImportBatch(
        Guid id,
        Guid userId,
        string fileName,
        string fileFingerprint,
        long fileSizeBytes,
        string encodingName,
        char delimiter,
        string dateColumn,
        string amountColumn,
        string? descriptionColumn,
        string? referenceColumn,
        string dateFormat,
        char decimalSeparator,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Import batch id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (fileSizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(fileSizeBytes));
        if (createdAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));

        Id = id;
        UserId = userId;
        FileName = Required(fileName, MaximumFileNameLength, nameof(fileName));
        FileFingerprint = Required(fileFingerprint, FingerprintLength, nameof(fileFingerprint)).ToLowerInvariant();
        if (FileFingerprint.Length != FingerprintLength ||
            FileFingerprint.Any(character => !Uri.IsHexDigit(character)))
            throw new ArgumentException("File fingerprint must be a 64-character SHA-256 hex value.", nameof(fileFingerprint));
        FileSizeBytes = fileSizeBytes;
        EncodingName = Required(encodingName, MaximumEncodingNameLength, nameof(encodingName));
        Delimiter = delimiter.ToString();
        DateColumn = Required(dateColumn, MaximumColumnNameLength, nameof(dateColumn));
        AmountColumn = Required(amountColumn, MaximumColumnNameLength, nameof(amountColumn));
        DescriptionColumn = Optional(descriptionColumn, MaximumColumnNameLength, nameof(descriptionColumn));
        ReferenceColumn = Optional(referenceColumn, MaximumColumnNameLength, nameof(referenceColumn));
        DateFormat = Required(dateFormat, 32, nameof(dateFormat));
        DecimalSeparator = decimalSeparator.ToString();
        Status = ImportBatchStatus.Staged;
        CreatedAtUtc = createdAtUtc;
    }

    public void AddRow(ImportRow row)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (row.UserId != UserId || row.ImportBatchId != Id)
            throw new ArgumentException("Import row must belong to the same batch owner.", nameof(row));
        if (_rows.Any(existing => existing.RowNumber == row.RowNumber))
            throw new InvalidOperationException("Import row number must be unique within a batch.");
        _rows.Add(row);
    }

    public void RecordConfirmation()
    {
        if (!_rows.Any(row => row.Status is ImportRowStatus.Imported or ImportRowStatus.SkippedDuplicate))
            throw new InvalidOperationException("A batch cannot progress without resolved rows.");

        Status = _rows.All(row => row.Status is ImportRowStatus.Imported or ImportRowStatus.SkippedDuplicate)
            ? ImportBatchStatus.Imported
            : ImportBatchStatus.PartiallyImported;
    }

    private static string Required(string? value, int maxLength, string parameterName)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > maxLength)
            throw new ArgumentException($"{parameterName} is required and cannot exceed {maxLength} characters.", parameterName);
        return normalized;
    }

    private static string? Optional(string? value, int maxLength, string parameterName) =>
        string.IsNullOrWhiteSpace(value) ? null : Required(value, maxLength, parameterName);
}
