using BusinessFinance.Application.Receipts;

namespace BusinessFinance.ReceiptMeasurement;

internal enum MeasurementCommand
{
    DryRun,
    Preprocessing,
    Models,
    Summary,
    Field
}

internal enum PreprocessorVariant
{
    Normalized,
    NoOp
}

internal sealed record MeasurementOptions(
    MeasurementCommand Command,
    string OutputPath,
    int? ConfirmedCalls,
    PreprocessorVariant? Preprocessor,
    bool RetryFailures,
    string? ImageDirectory,
    ReceiptCaptureIntent Intent)
{
    public const int PreprocessingCallBudget = 60;
    public const int ModelCallBudget = 30;

    public static MeasurementOptions Parse(string[] args, string repositoryRoot)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h")
            throw new UsageException(null);

        var command = args[0].ToLowerInvariant() switch
        {
            "dry-run" => MeasurementCommand.DryRun,
            "preprocessing" => MeasurementCommand.Preprocessing,
            "models" => MeasurementCommand.Models,
            "summary" => MeasurementCommand.Summary,
            "field" => MeasurementCommand.Field,
            _ => throw new UsageException($"Bilinmeyen komut: {args[0]}")
        };

        var outputPath = Path.Combine(
            repositoryRoot,
            "artifacts",
            "receipt-measurement",
            "results.jsonl");
        int? confirmedCalls = null;
        PreprocessorVariant? preprocessor = null;
        var retryFailures = false;
        string? imageDirectory = null;
        var intent = ReceiptCaptureIntent.Expense;

        for (var index = 1; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--output" when index + 1 < args.Length:
                    outputPath = Path.GetFullPath(args[++index], repositoryRoot);
                    break;
                case "--confirm-calls" when index + 1 < args.Length &&
                    int.TryParse(args[++index], out var calls):
                    confirmedCalls = calls;
                    break;
                case "--preprocessor" when index + 1 < args.Length:
                    preprocessor = args[++index].ToLowerInvariant() switch
                    {
                        "normalized" => PreprocessorVariant.Normalized,
                        "noop" => PreprocessorVariant.NoOp,
                        _ => throw new UsageException(
                            "--preprocessor yalnız normalized veya noop olabilir.")
                    };
                    break;
                case "--path" when index + 1 < args.Length:
                    imageDirectory = Path.GetFullPath(args[++index], repositoryRoot);
                    break;
                case "--intent" when index + 1 < args.Length:
                    intent = args[++index].ToLowerInvariant() switch
                    {
                        "expense" => ReceiptCaptureIntent.Expense,
                        "income" => ReceiptCaptureIntent.Income,
                        "transfer" => ReceiptCaptureIntent.Transfer,
                        _ => throw new UsageException(
                            "--intent yalnız expense, income veya transfer olabilir.")
                    };
                    break;
                case "--retry-failures":
                    retryFailures = true;
                    break;
                default:
                    throw new UsageException($"Geçersiz argüman: {args[index]}");
            }
        }

        if (command == MeasurementCommand.Preprocessing &&
            confirmedCalls != PreprocessingCallBudget)
        {
            throw new UsageException(
                $"Ön işleme koşusu gerçek kota harcar; --confirm-calls {PreprocessingCallBudget} zorunludur.");
        }

        if (command == MeasurementCommand.Models)
        {
            if (confirmedCalls != ModelCallBudget)
            {
                throw new UsageException(
                    $"Model koşusu gerçek kota harcar; --confirm-calls {ModelCallBudget} zorunludur.");
            }

            if (preprocessor is null)
            {
                throw new UsageException(
                    "Model koşusunda A turunun kazananı --preprocessor ile belirtilmelidir.");
            }
        }

        if (command == MeasurementCommand.Field && imageDirectory is null)
        {
            throw new UsageException(
                "Saha koşumu gerçek fotoğraf klasörü ister; --path zorunludur.");
        }

        return new MeasurementOptions(
            command,
            outputPath,
            confirmedCalls,
            preprocessor,
            retryFailures,
            imageDirectory,
            intent);
    }

    public static string Usage => """
        Aşama 12.10 sentetik fiş ölçüm aracı

        Komutlar:
          dry-run
              30 fişi üretir ve iki ön işleyiciden geçirir; ağ çağrısı yapmaz.

          preprocessing --confirm-calls 60
              30 fişi gemini-3.5-flash-lite üzerinde normalized/noop karşılaştırır.
              İstekler arasında en az 4 saniye bekler.

          models --confirm-calls 30 --preprocessor normalized|noop
              15 zor vakayı gemini-3.7-flash ve gemini-3.5-flash-lite ile ölçer.
              İstekler arasında en az 12 saniye bekler.

          summary
              Var olan JSONL sonuçlarını alan doğruluğu, gecikme ve token olarak özetler.

          field --path <klasör> [--intent expense|income|transfer]
              Klasördeki gerçek fotoğrafları üretim boru hattından (istemci
              JPEG'i + ön işleme + analiz + belge türü kapısı + doğrulayıcı)
              geçirir. Sentetik set değil, gerçek fiş/dekont/POS slipleri.
              --intent, kullanıcının yakalama anında bildirdiği yöndür ve
              belgeden okunmaz; varsayılan expense. Dekont yalnız transfer
              niyetinde okunur.

        Ortak seçenekler:
          --output <yol>       Varsayılan: artifacts/receipt-measurement/results.jsonl
          --retry-failures     Önceki başarısız logical call kayıtlarını yeniden dener.

        Gerçek koşular yalnız BUSINESS_FINANCE_GEMINI_TEST_KEY environment
        değişkenini okur. Secret hiçbir çıktıya veya sonuç dosyasına yazılmaz.
        """;
}

internal sealed class UsageException(string? message) : Exception(message);
