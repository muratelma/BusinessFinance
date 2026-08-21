using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using BusinessFinance.Application.Receipts;

namespace BusinessFinance.ReceiptMeasurement;

internal sealed record ReceiptReference(
    string Id,
    string CounterpartyName,
    DateOnly PurchasedAt,
    decimal SubtotalAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    string PaymentLabel,
    ReceiptVisual Visual,
    bool IsHard);

internal enum ReceiptVisual
{
    Clean,
    Oversized,
    Dark,
    Faded,
    LowContrast,
    Noisy,
    Tilted,
    Long,
    Shadowed,
    Crumpled
}

internal sealed record MeasuredFields(
    string? CounterpartyName,
    string? PurchasedAt,
    string? SubtotalAmount,
    string? TaxAmount,
    string? TotalAmount);

internal sealed record FieldAccuracy(
    bool CounterpartyName,
    bool PurchasedAt,
    bool TaxAmount,
    bool TotalAmount)
{
    [JsonIgnore]
    public int CorrectCount => new[] { CounterpartyName, PurchasedAt, TaxAmount, TotalAmount }
        .Count(value => value);
}

internal sealed record NormalizationMetrics(
    int OriginalWidth,
    int OriginalHeight,
    int Width,
    int Height,
    bool OrientationCorrected,
    bool Resized,
    bool ContrastAdjusted);

internal sealed record MeasurementResult(
    string Phase,
    string CaseId,
    string Variant,
    string Model,
    bool IsHard,
    DateTimeOffset RecordedAt,
    bool IsSuccess,
    string? ErrorCode,
    MeasuredFields Expected,
    MeasuredFields? Actual,
    FieldAccuracy Accuracy,
    int InputTokens,
    int OutputTokens,
    int TotalTokens,
    double LatencyMilliseconds,
    NormalizationMetrics Normalization,
    int PhysicalRequests = 0)
{
    [JsonIgnore]
    public string Key => $"{Phase}|{CaseId}|{Variant}|{Model}";

    public static MeasurementResult Failed(
        string phase,
        ReceiptReference receipt,
        string variant,
        string model,
        string errorCode,
        ReceiptImageNormalization normalization) => new(
            phase,
            receipt.Id,
            variant,
            model,
            receipt.IsHard,
            DateTimeOffset.UtcNow,
            false,
            errorCode,
            AccuracyScoring.Expected(receipt),
            null,
            new FieldAccuracy(false, false, false, false),
            0,
            0,
            0,
            0,
            Metrics(normalization));

    public static MeasurementResult Succeeded(
        string phase,
        ReceiptReference receipt,
        string variant,
        string model,
        RawReceiptReading reading,
        ReceiptImageNormalization normalization)
    {
        var actual = new MeasuredFields(
            reading.CounterpartyName,
            reading.PurchasedAt,
            reading.SubtotalAmount,
            reading.TaxAmount,
            reading.TotalAmount);
        return new MeasurementResult(
            phase,
            receipt.Id,
            variant,
            model,
            receipt.IsHard,
            DateTimeOffset.UtcNow,
            true,
            null,
            AccuracyScoring.Expected(receipt),
            actual,
            AccuracyScoring.Score(receipt, actual),
            reading.Usage.InputTokens,
            reading.Usage.OutputTokens,
            reading.Usage.TotalTokens,
            reading.Usage.Latency.TotalMilliseconds,
            Metrics(normalization));
    }

    private static NormalizationMetrics Metrics(ReceiptImageNormalization value) => new(
        value.OriginalWidth,
        value.OriginalHeight,
        value.Width,
        value.Height,
        value.OrientationCorrected,
        value.Resized,
        value.ContrastAdjusted);
}

internal static class AccuracyScoring
{
    public static MeasuredFields Expected(ReceiptReference receipt) => new(
        receipt.CounterpartyName,
        receipt.PurchasedAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        receipt.SubtotalAmount.ToString("0.00", CultureInfo.InvariantCulture),
        receipt.TaxAmount.ToString("0.00", CultureInfo.InvariantCulture),
        receipt.TotalAmount.ToString("0.00", CultureInfo.InvariantCulture));

    public static FieldAccuracy Score(ReceiptReference expected, MeasuredFields actual) => new(
        MerchantEqual(expected.CounterpartyName, actual.CounterpartyName),
        DateEqual(expected.PurchasedAt, actual.PurchasedAt),
        MoneyEqual(expected.TaxAmount, actual.TaxAmount),
        MoneyEqual(expected.TotalAmount, actual.TotalAmount));

    private static bool MerchantEqual(string expected, string? actual) =>
        actual is not null &&
        NormalizeText(WithoutSyntheticCaseSuffix(expected)) ==
        NormalizeText(WithoutSyntheticCaseSuffix(actual));

    private static string WithoutSyntheticCaseSuffix(string value)
    {
        var trimmed = value.TrimEnd();
        var separator = trimmed.LastIndexOf(' ');
        if (separator < 0)
            return trimmed;

        var suffix = trimmed[(separator + 1)..];
        return suffix.Length == 2 && suffix.All(char.IsDigit)
            ? trimmed[..separator]
            : trimmed;
    }

    private static string NormalizeText(string value)
    {
        var decomposed = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                continue;

            if (char.IsLetterOrDigit(character))
                builder.Append(char.ToUpperInvariant(character));
        }

        return builder.ToString();
    }

    private static bool DateEqual(DateOnly expected, string? actual)
    {
        if (actual is null)
            return false;

        var formats = new[] { "yyyy-MM-dd", "dd.MM.yyyy", "d.M.yyyy" };
        return DateOnly.TryParseExact(
                actual.Trim(),
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed) &&
            parsed == expected;
    }

    private static bool MoneyEqual(decimal expected, string? actual)
    {
        if (actual is null)
            return false;

        var normalized = actual.Trim()
            .Replace("₺", string.Empty, StringComparison.Ordinal)
            .Replace("TRY", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" ", string.Empty, StringComparison.Ordinal);
        if (normalized.Contains(',') && normalized.Contains('.'))
            normalized = normalized.Replace(".", string.Empty, StringComparison.Ordinal);
        normalized = normalized.Replace(',', '.');

        return decimal.TryParse(
                normalized,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var parsed) &&
            parsed == expected;
    }
}
