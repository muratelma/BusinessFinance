using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Attachments;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Categories;
using BusinessFinance.Infrastructure.Receipts;
using SkiaSharp;

namespace BusinessFinance.ReceiptMeasurement;

/// <summary>
/// Runs the production reading pipeline over a folder of <b>real</b> photos.
///
/// The synthetic set measured whether the plumbing works; it is drawn in code and
/// cannot show what actual receipts do — folded paper, POS slips, e-invoices,
/// refunds, bank statements. This command exists to bring those in without going
/// through the phone by hand, and it deliberately calls the same use case the API
/// calls, so a difference here is a difference the user would see.
/// </summary>
internal static class FieldRun
{
    /// <summary>
    /// The categories a fresh user actually has, read from the seed itself.
    ///
    /// This used to be a six-name copy, and the copy was wrong: it omitted
    /// <c>Yeme-içme</c>, so the run reported "no food bucket exists" for a döner
    /// receipt the real app would have placed. A hand-kept list measures the
    /// fixture, not the product — the whole point of this tool is that a gap it
    /// finds is a gap the user would hit.
    /// </summary>
    private static readonly (string Name, CategoryType Type)[] DefaultCategories =
        [.. EfCategoryRepository.DefaultCategories];

    public static async Task<int> RunAsync(
        string imageDirectory,
        string outputPath,
        ReceiptCaptureIntent intent,
        CancellationToken cancellationToken)
    {
        var apiKey = ReadApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.Error.WriteLine(
                "Gemini anahtarı bulunamadı (BUSINESS_FINANCE_GEMINI_TEST_KEY veya " +
                "API user-secrets 'Gemini:ApiKey'); hiçbir ağ çağrısı yapılmadı.");
            return 2;
        }

        var files = Directory
            .EnumerateFiles(imageDirectory)
            .Where(file => IsImage(Path.GetExtension(file)))
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (files.Length == 0)
        {
            Console.Error.WriteLine($"{imageDirectory} içinde görsel yok.");
            return 2;
        }

        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://generativelanguage.googleapis.com/"),
            Timeout = TimeSpan.FromSeconds(90)
        };
        var analyzer = new GeminiReceiptAnalyzer(
            httpClient,
            Options.Create(new GeminiOptions
            {
                ApiKey = apiKey,
                Model = "gemini-3.5-flash-lite",
                TimeoutSeconds = 90
            }),
            TimeProvider.System);

        var userId = Guid.NewGuid();
        var useCase = new AnalyzeReceiptUseCase(
            new FixedUser(userId),
            new PermissiveInspector(),
            new ReceiptImagePreprocessor(Options.Create(new ReceiptImageOptions())),
            new FixedCategories(userId, DefaultCategories),
            new NoCounterparties(),
            analyzer,
            new NoDuplicates(),
            new NoRefunds(),
            TimeProvider.System);

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        await using var output = new StreamWriter(outputPath, append: false, Encoding.UTF8);

        for (var index = 0; index < files.Length; index++)
        {
            var file = files[index];
            var name = Path.GetFileName(file);
            // The app never uploads the file as picked: it re-encodes to JPEG at
            // 2400 px. Doing the same here keeps the comparison honest — and it is
            // the only reason a .webp from the gallery reaches the server at all.
            var (jpeg, width, height) = ToClientJpeg(await File.ReadAllBytesAsync(file, cancellationToken));

            var started = DateTimeOffset.UtcNow;
            using var stream = new MemoryStream(jpeg);
            var result = await useCase.ExecuteAsync(
                new AnalyzeReceiptCommand(
                    Path.ChangeExtension(name, ".jpg"),
                    "image/jpeg",
                    stream,
                    jpeg.LongLength,
                    intent),
                cancellationToken);
            var elapsed = DateTimeOffset.UtcNow - started;

            var record = new JsonObject
            {
                ["file"] = name,
                ["intent"] = intent.ToString(),
                ["uploadBytes"] = jpeg.Length,
                ["uploadWidth"] = width,
                ["uploadHeight"] = height,
                ["latencyMs"] = (int)elapsed.TotalMilliseconds,
                ["isSuccess"] = result.IsSuccess,
                ["errorCode"] = result.IsSuccess ? null : result.Error.Code,
                ["errorMessage"] = result.IsSuccess ? null : result.Error.Message
            };
            if (result.IsSuccess)
            {
                var draft = result.Value.Draft;
                record["counterpartyName"] = draft.CounterpartyName;
                record["counterpartyState"] = draft.CounterpartyState.ToString();
                record["purchasedAt"] = draft.PurchasedAt?.ToString("yyyy-MM-dd");
                record["purchasedAtState"] = draft.PurchasedAtState.ToString();
                record["totalAmount"] = draft.TotalAmount?.ToString("0.0000");
                record["totalAmountState"] = draft.TotalAmountState.ToString();
                record["feeAmount"] = draft.FeeAmount?.ToString("0.0000");
                record["feeAmountState"] = draft.FeeAmountState.ToString();
                record["currencyCode"] = draft.CurrencyCode;
                record["paymentHint"] = draft.PaymentHint.ToString();
                record["categoryName"] = draft.CategoryName;
                record["categoryState"] = draft.CategoryState.ToString();
                record["warnings"] = new JsonArray(
                    draft.Warnings.Select(w => (JsonNode)w.Code).ToArray());
                record["totalTokens"] = result.Value.Usage.TotalTokens;
            }

            await output.WriteLineAsync(
                record.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));
            await output.FlushAsync(cancellationToken);
            Console.WriteLine(
                $"[{index + 1}/{files.Length}] {name}: " +
                (result.IsSuccess
                    ? $"{record["counterpartyName"]} | {record["purchasedAt"]} | {record["totalAmount"]} | {record["categoryName"]}"
                    : $"RED {result.Error.Code}"));

            // Free tier allows 15 requests a minute on this model.
            if (index < files.Length - 1)
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }

        Console.WriteLine($"Sonuçlar: {outputPath}");
        return 0;
    }

    /// <summary>
    /// Reads the key without ever printing it. The environment variable comes
    /// first so a run can be pointed at another key; otherwise the API project's
    /// user-secrets file is read directly, which avoids adding a configuration
    /// package to a throwaway tool.
    /// </summary>
    private static string? ReadApiKey()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("BUSINESS_FINANCE_GEMINI_TEST_KEY");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            return fromEnvironment;

        var secretsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft", "UserSecrets", "business-finance-api", "secrets.json");
        if (!File.Exists(secretsPath))
            return null;

        var json = JsonNode.Parse(File.ReadAllText(secretsPath)) as JsonObject;
        return json?["Gemini:ApiKey"]?.GetValue<string>();
    }

    private static bool IsImage(string extension) => extension.ToLowerInvariant()
        is ".jpg" or ".jpeg" or ".png" or ".webp" or ".heic";

    /// <summary>
    /// What the Flutter client does before uploading: bake the orientation, cap
    /// the long edge at 2400 px, encode JPEG at quality 85.
    /// </summary>
    private static (byte[] Bytes, int Width, int Height) ToClientJpeg(byte[] source)
    {
        using var codec = SKCodec.Create(new MemoryStream(source))
            ?? throw new InvalidOperationException("Görsel çözülemedi.");
        using var decoded = SKBitmap.Decode(codec);
        using var upright = Upright(decoded, codec.EncodedOrigin);

        const int maxLongEdge = 2400;
        var longEdge = Math.Max(upright.Width, upright.Height);
        using var scaled = longEdge <= maxLongEdge
            ? upright.Copy()
            : upright.Resize(
                new SKImageInfo(
                    (int)Math.Round(upright.Width * (double)maxLongEdge / longEdge),
                    (int)Math.Round(upright.Height * (double)maxLongEdge / longEdge)),
                new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));

        using var image = SKImage.FromBitmap(scaled);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);
        return (data.ToArray(), scaled.Width, scaled.Height);
    }

    private static SKBitmap Upright(SKBitmap bitmap, SKEncodedOrigin origin)
    {
        if (origin is SKEncodedOrigin.Default or SKEncodedOrigin.TopLeft)
            return bitmap.Copy();

        var swapsAxes = origin
            is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
            or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var rotated = new SKBitmap(
            swapsAxes ? bitmap.Height : bitmap.Width,
            swapsAxes ? bitmap.Width : bitmap.Height);
        using var canvas = new SKCanvas(rotated);
        switch (origin)
        {
            case SKEncodedOrigin.BottomRight:
                canvas.RotateDegrees(180, rotated.Width / 2f, rotated.Height / 2f);
                break;
            case SKEncodedOrigin.RightTop:
                canvas.Translate(rotated.Width, 0);
                canvas.RotateDegrees(90);
                break;
            case SKEncodedOrigin.LeftBottom:
                canvas.Translate(0, rotated.Height);
                canvas.RotateDegrees(270);
                break;
        }

        canvas.DrawBitmap(
            bitmap,
            0,
            0,
            new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
        return rotated;
    }

    /// <summary>
    /// The measurement tool has no database, so nothing can be a duplicate of
    /// anything. Reporting "no match" is honest here; the check itself is covered
    /// by the application tests.
    /// </summary>
    private sealed class NoDuplicates : IReceiptDuplicateLookup
    {
        public Task<ReceiptDuplicateMatch?> FindAsync(
            Guid userId,
            DateOnly? transactionDate,
            decimal? amount,
            string? counterpartyName,
            ReceiptCaptureIntent intent,
            CancellationToken cancellationToken) =>
            Task.FromResult<ReceiptDuplicateMatch?>(null);
    }

    /// <inheritdoc cref="NoDuplicates" />
    /// <remarks>
    /// Ölçüm koşusu okumanın kendisini ölçer; kullanıcının kayıtlı karşı
    /// tarafı yoktur ve öneri katmanı sonuca karışmaz.
    /// </remarks>
    private sealed class NoCounterparties : ICounterpartyRepository
    {
        public Task<Counterparty?> FindOwnedByNameAsync(
            Guid userId, string name, CancellationToken cancellationToken) =>
            Task.FromResult<Counterparty?>(null);

        public Task<Counterparty?> FindOwnedByIdAsync(
            Guid counterpartyId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<CounterpartyBalanceSummary>> ListBalancesAsync(
            Guid userId,
            CounterpartyBalanceFilter filter,
            bool? isActive,
            DateOnly asOfDate,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<CounterpartyBalanceSummary?> FindBalanceAsync(
            Guid counterpartyId,
            Guid userId,
            DateOnly asOfDate,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Counterparty> FindOrCreateByNameAsync(
            Guid userId, string name, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyDictionary<Guid, string>> ListNamesAsync(
            Guid userId,
            IReadOnlyCollection<Guid> counterpartyIds,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> ExistsByNameAsync(
            Guid userId,
            string normalizedName,
            Guid? exceptCounterpartyId,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task AddAsync(Counterparty counterparty, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task UpdateOwnedAsync(
            Counterparty counterparty, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> DeleteIfWithoutHistoryAsync(
            Guid counterpartyId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task AddChargeAsync(CounterpartyCharge charge, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task AddPaymentAsync(CounterpartyPayment payment, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<CounterpartyCharge?> FindOwnedChargeAsync(
            Guid chargeId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<CounterpartyPayment?> FindOwnedPaymentAsync(
            Guid paymentId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task SaveChargeAsync(CounterpartyCharge charge, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task SavePaymentAsync(CounterpartyPayment payment, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class NoRefunds : IReceiptRefundLookup
    {
        public Task<ReceiptRefundMatch?> FindAsync(
            Guid userId,
            DateOnly? refundDate,
            decimal? refundAmount,
            string? counterpartyName,
            CancellationToken cancellationToken) =>
            Task.FromResult<ReceiptRefundMatch?>(null);
    }

    private sealed class FixedUser(Guid userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    /// <summary>
    /// The real inspector lives inside Infrastructure and is not visible here.
    /// This one accepts what the command already produced — JPEG bytes — so the
    /// run measures reading, not file screening, which the API tests cover.
    /// </summary>
    private sealed class PermissiveInspector : IAttachmentFileInspector
    {
        public AttachmentInspection Inspect(
            string fileName,
            string contentType,
            ReadOnlySpan<byte> content) =>
            new(true, null, "image/jpeg", ".jpg", null);
    }

    /// <summary>
    /// Serves the seeded list in memory. The <paramref name="type" /> filter is
    /// honoured because the seed holds income buckets too, and the use case asks
    /// for expenses only — handing the model "Maaş" as a spending option would
    /// invent a choice the app never offers.
    /// </summary>
    private sealed class FixedCategories(
        Guid userId,
        IReadOnlyList<(string Name, CategoryType Type)> categories)
        : ICategoryRepository
    {
        public Task<IReadOnlyList<Category>> ListAsync(
            Guid ownerId,
            CategoryType? type,
            bool? isActive,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Category>>(
                categories
                    .Where(item => type is null || item.Type == type)
                    .Select(item => new Category(
                        Guid.NewGuid(), userId, item.Name, item.Type))
                    .ToArray());

        public Task EnsureDefaultsAsync(Guid id, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task AddAsync(Category category, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task<bool> ExistsByNameAndTypeAsync(
            Guid id,
            string name,
            CategoryType type,
            CancellationToken cancellationToken) =>
            Task.FromResult(false);

        public Task<Category?> FindOwnedByIdAsync(
            Guid categoryId,
            Guid id,
            CancellationToken cancellationToken) =>
            Task.FromResult<Category?>(null);

        public Task UpdateOwnedAsync(
            Category category,
            Guid id,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
