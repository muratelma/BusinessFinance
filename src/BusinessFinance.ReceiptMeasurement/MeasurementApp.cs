using Microsoft.Extensions.Options;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Infrastructure.Receipts;

namespace BusinessFinance.ReceiptMeasurement;

internal static class MeasurementApp
{
    private const string LiteModel = "gemini-3.5-flash-lite";
    private const string FullModel = "gemini-3.7-flash";
    private const int FullModelDailyRequestBudget = 20;
    private static readonly IReadOnlyList<string> Categories =
        ["Market", "Restoran", "Ulaşım", "Fatura", "Sağlık"];

    public static async Task<int> RunAsync(string[] args)
    {
        MeasurementOptions options;
        try
        {
            options = MeasurementOptions.Parse(args, FindRepositoryRoot());
        }
        catch (UsageException exception)
        {
            if (!string.IsNullOrWhiteSpace(exception.Message))
                Console.Error.WriteLine(exception.Message);
            Console.Error.WriteLine(MeasurementOptions.Usage);
            return string.IsNullOrWhiteSpace(exception.Message) ? 0 : 2;
        }

        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellation.Cancel();
        };

        try
        {
            var store = new MeasurementStore(options.OutputPath);
            switch (options.Command)
            {
                case MeasurementCommand.DryRun:
                    DryRun();
                    return 0;
                case MeasurementCommand.Summary:
                    await store.WriteSummaryAsync(cancellation.Token);
                    return 0;
                case MeasurementCommand.Preprocessing:
                    return await RunPreprocessingAsync(store, options, cancellation.Token);
                case MeasurementCommand.Models:
                    return await RunModelsAsync(store, options, cancellation.Token);
                case MeasurementCommand.Field:
                    return await FieldRun.RunAsync(
                        options.ImageDirectory!,
                        options.OutputPath,
                        options.Intent,
                        cancellation.Token);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            Console.Error.WriteLine("Koşum iptal edildi; tamamlanan satırlar sonuç dosyasında kaldı.");
            return 130;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Koşum başarısız: {exception.Message}");
            return 1;
        }
    }

    private static void DryRun()
    {
        var preprocessors = new[]
        {
            (Name: "normalized", Value: CreatePreprocessor(PreprocessorVariant.Normalized)),
            (Name: "noop", Value: CreatePreprocessor(PreprocessorVariant.NoOp))
        };
        var accepted = 0;
        var resized = 0;
        var contrast = 0;

        foreach (var receipt in SyntheticReceiptSet.All)
        {
            var image = SyntheticReceiptSet.Render(receipt);
            foreach (var preprocessor in preprocessors)
            {
                var result = preprocessor.Value.Normalize(image);
                if (!result.IsAccepted)
                {
                    throw new InvalidOperationException(
                        $"{receipt.Id}/{preprocessor.Name} reddedildi: {result.RejectionReason}");
                }

                accepted++;
                resized += result.Resized ? 1 : 0;
                contrast += result.ContrastAdjusted ? 1 : 0;
            }
        }

        var hardSubset = SyntheticReceiptSet.All.Where(receipt => receipt.IsHard).Take(15).ToArray();
        if (SyntheticReceiptSet.All.Count != 30 || hardSubset.Length != 15)
            throw new InvalidOperationException("Referans set 30/15 sözleşmesini sağlamıyor.");

        Console.WriteLine(
            $"Dry-run tamam: 30 fiş, 15 zor vaka, {accepted} ön işleme sonucu; " +
            $"resize={resized}, kontrast={contrast}, ağ çağrısı=0.");
    }

    private static async Task<int> RunPreprocessingAsync(
        MeasurementStore store,
        MeasurementOptions options,
        CancellationToken cancellationToken)
    {
        var jobs = SyntheticReceiptSet.All
            .SelectMany(receipt => new[]
            {
                new MeasurementJob(
                    "preprocessing", receipt, PreprocessorVariant.Normalized, LiteModel),
                new MeasurementJob(
                    "preprocessing", receipt, PreprocessorVariant.NoOp, LiteModel)
            })
            .ToArray();

        return await RunJobsAsync(
            jobs,
            TimeSpan.FromSeconds(4),
            store,
            options,
            cancellationToken);
    }

    private static async Task<int> RunModelsAsync(
        MeasurementStore store,
        MeasurementOptions options,
        CancellationToken cancellationToken)
    {
        var preprocessor = options.Preprocessor!.Value;
        var jobs = SyntheticReceiptSet.All
            .Where(receipt => receipt.IsHard)
            .Take(15)
            .SelectMany(receipt => new[]
            {
                new MeasurementJob("models", receipt, preprocessor, FullModel),
                new MeasurementJob("models", receipt, preprocessor, LiteModel)
            })
            .ToArray();

        return await RunJobsAsync(
            jobs,
            TimeSpan.FromSeconds(12),
            store,
            options,
            cancellationToken);
    }

    private static async Task<int> RunJobsAsync(
        IReadOnlyList<MeasurementJob> jobs,
        TimeSpan minimumDelay,
        MeasurementStore store,
        MeasurementOptions options,
        CancellationToken cancellationToken)
    {
        var apiKey = Environment.GetEnvironmentVariable("BUSINESS_FINANCE_GEMINI_TEST_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.Error.WriteLine(
                "BUSINESS_FINANCE_GEMINI_TEST_KEY tanımlı değil; hiçbir ağ çağrısı yapılmadı.");
            return 2;
        }

        var allExisting = await store.ReadAllAsync(cancellationToken);
        var existing = allExisting
            .GroupBy(result => result.Key, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Last(), StringComparer.Ordinal);
        var pending = jobs.Where(job => ShouldRun(job, existing, options.RetryFailures)).ToArray();
        Console.WriteLine(
            $"Plan: {jobs.Count} logical call; tamamlanmış={jobs.Count - pending.Length}, " +
            $"bekleyen={pending.Length}, çıktı={store.Path}");
        if (pending.Length == 0)
        {
            await store.WriteSummaryAsync(cancellationToken);
            return 0;
        }

        var analyzers = new Dictionary<string, AnalyzerClient>(StringComparer.Ordinal);
        var utcToday = DateTime.UtcNow.Date;
        var fullPhysicalRequestsToday = allExisting
            .Where(result => result.Model == FullModel && result.RecordedAt.UtcDateTime.Date == utcToday)
            .Sum(PhysicalRequestCount);
        DateTimeOffset? previousStart = null;
        for (var index = 0; index < pending.Length; index++)
        {
            var job = pending[index];
            // The production adapter may retry once. Starting at 19 could turn a
            // single logical job into requests 20 and 21, so two slots are
            // reserved before every full-model job.
            if (job.Model == FullModel && fullPhysicalRequestsToday > FullModelDailyRequestBudget - 2)
            {
                Console.Error.WriteLine(
                    $"3.7 Flash günlük fiziksel çağrı kapısı doldu " +
                    $"({fullPhysicalRequestsToday}/{FullModelDailyRequestBudget}); " +
                    "kalan işler sonraki kota gününe bırakıldı.");
                await store.WriteSummaryAsync(cancellationToken);
                return 4;
            }

            if (previousStart is not null)
            {
                var remaining = minimumDelay - (DateTimeOffset.UtcNow - previousStart.Value);
                if (remaining > TimeSpan.Zero)
                    await Task.Delay(remaining, cancellationToken);
            }

            var source = SyntheticReceiptSet.Render(job.Receipt);
            var normalization = CreatePreprocessor(job.Preprocessor).Normalize(source);
            MeasurementResult measurement;
            if (!normalization.IsAccepted || normalization.Image is null)
            {
                measurement = MeasurementResult.Failed(
                    job.Phase,
                    job.Receipt,
                    VariantName(job.Preprocessor),
                    job.Model,
                    "measurement.preprocessing_rejected",
                    normalization);
            }
            else
            {
                if (!analyzers.TryGetValue(job.Model, out var analyzerClient))
                {
                    analyzerClient = CreateAnalyzer(job.Model, apiKey);
                    analyzers.Add(job.Model, analyzerClient);
                }

                Console.WriteLine(
                    $"[{index + 1,2}/{pending.Length}] {job.Phase} {job.Receipt.Id} " +
                    $"{VariantName(job.Preprocessor)} {job.Model}");
                previousStart = DateTimeOffset.UtcNow;
                var requestsBefore = analyzerClient.RequestCount;
                var result = await analyzerClient.Analyzer.AnalyzeAsync(
                    new ReceiptAnalysisRequest(
                        normalization.Image,
                        Categories,
                        ReceiptCaptureIntent.Expense),
                    cancellationToken);
                var physicalRequests = analyzerClient.RequestCount - requestsBefore;
                measurement = result.IsSuccess
                    ? MeasurementResult.Succeeded(
                        job.Phase,
                        job.Receipt,
                        VariantName(job.Preprocessor),
                        job.Model,
                        result.Value,
                        normalization)
                    : MeasurementResult.Failed(
                        job.Phase,
                        job.Receipt,
                        VariantName(job.Preprocessor),
                        job.Model,
                        result.Error.Code,
                        normalization);
                measurement = measurement with { PhysicalRequests = physicalRequests };
                if (job.Model == FullModel)
                    fullPhysicalRequestsToday += physicalRequests;
            }

            await store.AppendAsync(measurement, cancellationToken);
            Console.WriteLine(
                measurement.IsSuccess
                    ? $"  alan={measurement.Accuracy.CorrectCount}/4 " +
                      $"gecikme={measurement.LatencyMilliseconds:0}ms " +
                      $"token={measurement.TotalTokens} http={measurement.PhysicalRequests}"
                    : $"  hata={measurement.ErrorCode} http={measurement.PhysicalRequests}");

            if (measurement.ErrorCode is "receipt.provider_rate_limited" or
                "receipt.provider_unavailable" or "receipt.disabled")
            {
                Console.Error.WriteLine(
                    "Sağlayıcı/kota hatasında kalan çağrılar durduruldu; resume sonraki koşumda devam eder.");
                await store.WriteSummaryAsync(cancellationToken);
                return 3;
            }
        }

        await store.WriteSummaryAsync(cancellationToken);
        return 0;
    }

    private static bool ShouldRun(
        MeasurementJob job,
        IReadOnlyDictionary<string, MeasurementResult> existing,
        bool retryFailures)
    {
        var key = $"{job.Phase}|{job.Receipt.Id}|{VariantName(job.Preprocessor)}|{job.Model}";
        return !existing.TryGetValue(key, out var result) || (retryFailures && !result.IsSuccess);
    }

    private static IReceiptImagePreprocessor CreatePreprocessor(PreprocessorVariant variant) =>
        variant == PreprocessorVariant.Normalized
            ? new ReceiptImagePreprocessor(Options.Create(new ReceiptImageOptions()))
            : new PassThroughReceiptImagePreprocessor();

    private static AnalyzerClient CreateAnalyzer(string model, string apiKey)
    {
        var counter = new CountingHandler(new HttpClientHandler());
        var client = new HttpClient(counter)
        {
            BaseAddress = new Uri("https://generativelanguage.googleapis.com/"),
            Timeout = TimeSpan.FromSeconds(75)
        };
        return new AnalyzerClient(
            new GeminiReceiptAnalyzer(
                client,
                Options.Create(new GeminiOptions
                {
                    ApiKey = apiKey,
                    Model = model,
                    TimeoutSeconds = 75
                }),
                TimeProvider.System),
            counter);
    }

    private static string VariantName(PreprocessorVariant variant) =>
        variant == PreprocessorVariant.Normalized ? "normalized" : "noop";

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(System.IO.Path.Combine(directory.FullName, "BusinessFinance.slnx")))
                return directory.FullName;
            directory = directory.Parent;
        }

        throw new InvalidOperationException("BusinessFinance.slnx depo kökü bulunamadı.");
    }

    private sealed record MeasurementJob(
        string Phase,
        ReceiptReference Receipt,
        PreprocessorVariant Preprocessor,
        string Model);

    private sealed record AnalyzerClient(
        GeminiReceiptAnalyzer Analyzer,
        CountingHandler Counter)
    {
        public int RequestCount => Counter.RequestCount;
    }

    private sealed class CountingHandler(HttpMessageHandler innerHandler) : DelegatingHandler(innerHandler)
    {
        private int _requestCount;

        public int RequestCount => Volatile.Read(ref _requestCount);

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _requestCount);
            return base.SendAsync(request, cancellationToken);
        }
    }

    private static int PhysicalRequestCount(MeasurementResult result)
    {
        if (result.PhysicalRequests > 0)
            return result.PhysicalRequests;

        // Results written before the counter was added are deliberately counted
        // at the adapter's maximum. Over-counting defers work; under-counting can
        // break the provider's daily quota.
        return 2;
    }
}
