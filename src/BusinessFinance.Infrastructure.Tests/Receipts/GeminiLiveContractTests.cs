using Microsoft.Extensions.Options;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Infrastructure.Receipts;
using SkiaSharp;

namespace BusinessFinance.Infrastructure.Tests.Receipts;

/// <summary>
/// The one test that actually calls Google. It exists because the client is
/// hand-written against a documented shape rather than generated from it: a
/// silent contract change would otherwise surface as a user seeing "fiş
/// okunamadı" instead of a red build.
///
/// It asserts the contract, not the model's cleverness — that a well-formed
/// request comes back completed, parsed, and with a token count. Asserting the
/// exact merchant string would make the suite flaky for no gain.
///
/// Skipped unless BUSINESS_FINANCE_GEMINI_TEST_KEY is set, and it spends real
/// free-tier quota (RPD 500 on the Lite line) when it runs.
/// </summary>
public sealed class GeminiLiveContractTests
{
    [GeminiFact]
    public async Task Analyze_RealSyntheticReceipt_ComesBackParsedAndCosted()
    {
        var analyzer = new GeminiReceiptAnalyzer(
            new HttpClient
            {
                BaseAddress = new Uri("https://generativelanguage.googleapis.com/"),
                Timeout = TimeSpan.FromSeconds(60)
            },
            Options.Create(new GeminiOptions
            {
                ApiKey = Environment.GetEnvironmentVariable("BUSINESS_FINANCE_GEMINI_TEST_KEY"),
                Model = Environment.GetEnvironmentVariable("BUSINESS_FINANCE_GEMINI_TEST_MODEL")
                        ?? "gemini-3.5-flash-lite"
            }),
            TimeProvider.System);

        var result = await analyzer.AnalyzeAsync(
            new ReceiptAnalysisRequest(
                new ReceiptImage(SyntheticReceipt(), "image/jpeg"),
                ["Market", "Ulaşım", "Restoran"],
                ReceiptCaptureIntent.Expense),
            CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsSuccess ? "" : result.Error.Code);
        var reading = result.Value;

        // The shape has to survive, field by field, or the client is out of date.
        Assert.False(string.IsNullOrWhiteSpace(reading.TotalAmount));
        Assert.False(string.IsNullOrWhiteSpace(reading.CounterpartyName));
        Assert.True(reading.Usage.TotalTokens > 0, "usage is missing from the response");
        Assert.True(reading.Usage.Latency > TimeSpan.Zero);

        // The closed category set is the guard that matters most: a value outside
        // it means the schema stopped constraining the model.
        Assert.True(
            reading.CategoryName is null or "Market" or "Ulaşım" or "Restoran",
            $"category escaped the closed set: {reading.CategoryName}");

        // Only "cash", "card" and "unknown" are offered by the schema.
        Assert.True(
            reading.PaymentMethodHint is null or "cash" or "card" or "unknown",
            $"payment hint escaped the closed set: {reading.PaymentMethodHint}");
    }

    /// <summary>
    /// Drawn rather than checked in: a committed photo is a binary nobody reviews,
    /// and this makes what the model is asked to read reviewable in the diff. It
    /// is also unmistakably synthetic — no real merchant, no real card.
    /// </summary>
    private static byte[] SyntheticReceipt()
    {
        using var bitmap = new SKBitmap(520, 700);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.White);
            using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
            using var heading = new SKFont(SKTypeface.Default, 30);
            using var body = new SKFont(SKTypeface.Default, 22);

            var y = 60f;
            canvas.DrawText("TEST MARKET A.S.", 40, y, SKTextAlign.Left, heading, paint);
            foreach (var line in Lines)
            {
                y += 40;
                canvas.DrawText(line, 40, y, SKTextAlign.Left, body, paint);
            }
        }

        using var data = bitmap.Encode(SKEncodedImageFormat.Jpeg, 90);
        return data.ToArray();
    }

    private static readonly string[] Lines =
    [
        "TARIH: 18.08.2026  SAAT: 14:32",
        "FIS NO: 0042",
        "--------------------------------",
        "EKMEK              x2      30,00",
        "SUT 1L             x1      48,50",
        "PEYNIR             x1     185,00",
        "DETERJAN           x1     442,42",
        "--------------------------------",
        "ARA TOPLAM               705,92",
        "KDV %20                  141,58",
        "TOPLAM                   847,50",
        "KREDI KARTI              847,50"
    ];
}

public sealed class GeminiFactAttribute : FactAttribute
{
    public GeminiFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(
                "BUSINESS_FINANCE_GEMINI_TEST_KEY")))
        {
            Skip = "BUSINESS_FINANCE_GEMINI_TEST_KEY is not configured.";
        }
    }
}
