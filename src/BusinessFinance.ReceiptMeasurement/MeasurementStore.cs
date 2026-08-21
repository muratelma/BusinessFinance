using System.Text;
using System.Text.Json;

namespace BusinessFinance.ReceiptMeasurement;

internal sealed class MeasurementStore(string path)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public string Path { get; } = path;

    public async Task<IReadOnlyDictionary<string, MeasurementResult>> ReadLatestAsync(
        CancellationToken cancellationToken)
    {
        var all = await ReadAllAsync(cancellationToken);
        var results = new Dictionary<string, MeasurementResult>(StringComparer.Ordinal);
        foreach (var result in all)
            results[result.Key] = result;
        return results;
    }

    public async Task<IReadOnlyList<MeasurementResult>> ReadAllAsync(
        CancellationToken cancellationToken)
    {
        var results = new List<MeasurementResult>();
        if (!File.Exists(Path))
            return results;

        var lineNumber = 0;
        foreach (var line in await File.ReadAllLinesAsync(Path, cancellationToken))
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
                continue;

            MeasurementResult? result;
            try
            {
                result = JsonSerializer.Deserialize<MeasurementResult>(line, JsonOptions);
            }
            catch (JsonException exception)
            {
                throw new InvalidOperationException(
                    $"Sonuç dosyasının {lineNumber}. satırı geçerli JSON değil.",
                    exception);
            }

            if (result is null)
                throw new InvalidOperationException($"Sonuç dosyasının {lineNumber}. satırı boş sonuç.");

            // The scoring code is versioned with the harness. Raw expected and
            // actual values are durable; a clarified scoring rule must not spend
            // provider quota just to rewrite old JSONL lines.
            if (result.IsSuccess && result.Actual is not null)
            {
                var receipt = SyntheticReceiptSet.All.Single(item => item.Id == result.CaseId);
                result = result with { Accuracy = AccuracyScoring.Score(receipt, result.Actual) };
            }

            results.Add(result);
        }

        return results;
    }

    public async Task AppendAsync(MeasurementResult result, CancellationToken cancellationToken)
    {
        var directory = System.IO.Path.GetDirectoryName(Path);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var line = JsonSerializer.Serialize(result, JsonOptions) + Environment.NewLine;
        await File.AppendAllTextAsync(Path, line, new UTF8Encoding(false), cancellationToken);
    }

    public async Task WriteSummaryAsync(CancellationToken cancellationToken)
    {
        var latest = await ReadLatestAsync(cancellationToken);
        var groups = latest.Values
            .OrderBy(result => result.Phase, StringComparer.Ordinal)
            .ThenBy(result => result.Variant, StringComparer.Ordinal)
            .ThenBy(result => result.Model, StringComparer.Ordinal)
            .GroupBy(result => new { result.Phase, result.Variant, result.Model })
            .Select(group =>
            {
                var values = group.ToArray();
                return new MeasurementSummary(
                    group.Key.Phase,
                    group.Key.Variant,
                    group.Key.Model,
                    values.Length,
                    values.Count(value => value.IsSuccess),
                    Percentage(values, value => value.Accuracy.CounterpartyName),
                    Percentage(values, value => value.Accuracy.PurchasedAt),
                    Percentage(values, value => value.Accuracy.TaxAmount),
                    Percentage(values, value => value.Accuracy.TotalAmount),
                    values.Average(value => value.Accuracy.CorrectCount / 4d) * 100d,
                    AverageSuccessful(values, value => value.LatencyMilliseconds),
                    AverageSuccessful(values, value => value.TotalTokens));
            })
            .ToArray();

        foreach (var summary in groups)
        {
            Console.WriteLine(
                $"{summary.Phase,-13} {summary.Variant,-10} {summary.Model,-24} " +
                $"n={summary.Attempts,2} ok={summary.Successes,2} " +
                $"alan={summary.OverallAccuracyPercent,6:0.0}% " +
                $"işletme={summary.MerchantAccuracyPercent,6:0.0}% " +
                $"tarih={summary.DateAccuracyPercent,6:0.0}% " +
                $"KDV={summary.TaxAccuracyPercent,6:0.0}% " +
                $"toplam={summary.TotalAccuracyPercent,6:0.0}% " +
                $"gecikme={summary.AverageLatencyMilliseconds,8:0}ms " +
                $"token={summary.AverageTotalTokens,7:0.0}");
        }

        var summaryPath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Path) ?? string.Empty,
            "summary.json");
        var json = JsonSerializer.Serialize(
            groups,
            new JsonSerializerOptions(JsonOptions) { WriteIndented = true });
        await File.WriteAllTextAsync(summaryPath, json + Environment.NewLine, cancellationToken);
        Console.WriteLine($"Özet: {summaryPath}");
    }

    private static double Percentage(
        IReadOnlyCollection<MeasurementResult> values,
        Func<MeasurementResult, bool> predicate) =>
        values.Count == 0 ? 0 : values.Count(predicate) * 100d / values.Count;

    private static double AverageSuccessful(
        IReadOnlyCollection<MeasurementResult> values,
        Func<MeasurementResult, double> selector)
    {
        var successful = values.Where(value => value.IsSuccess).ToArray();
        return successful.Length == 0 ? 0 : successful.Average(selector);
    }
}

internal sealed record MeasurementSummary(
    string Phase,
    string Variant,
    string Model,
    int Attempts,
    int Successes,
    double MerchantAccuracyPercent,
    double DateAccuracyPercent,
    double TaxAccuracyPercent,
    double TotalAccuracyPercent,
    double OverallAccuracyPercent,
    double AverageLatencyMilliseconds,
    double AverageTotalTokens);
