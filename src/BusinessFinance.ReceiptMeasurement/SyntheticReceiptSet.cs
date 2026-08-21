using System.Globalization;
using BusinessFinance.Application.Receipts;
using SkiaSharp;

namespace BusinessFinance.ReceiptMeasurement;

internal static class SyntheticReceiptSet
{
    public static IReadOnlyList<ReceiptReference> All { get; } =
    [
        R("R01", "TEST MARKET 01", 1, 125.00m, 25.00m, "KREDI KARTI", ReceiptVisual.Clean),
        R("R02", "TEST FIRIN 02", 2, 87.50m, 17.50m, "NAKIT", ReceiptVisual.Clean),
        R("R03", "TEST LOKANTA 03", 3, 312.40m, 62.48m, "KREDI KARTI", ReceiptVisual.Clean),
        R("R04", "TEST KIRTASIYE 04", 4, 456.75m, 91.35m, "BANKA KARTI", ReceiptVisual.Oversized),
        R("R05", "TEST ECZANE 05", 5, 210.00m, 42.00m, "NAKIT", ReceiptVisual.Oversized),
        R("R06", "TEST MANAV 06", 6, 143.25m, 28.65m, "KREDI KARTI", ReceiptVisual.Clean),
        R("R07", "TEST KAFE 07", 7, 98.80m, 19.76m, "NAKIT", ReceiptVisual.Clean),
        R("R08", "TEST MAGAZA 08", 8, 1024.50m, 204.90m, "KREDI KARTI", ReceiptVisual.Oversized),
        R("R09", "TEST OTOPARK 09", 9, 75.00m, 15.00m, "NAKIT", ReceiptVisual.Clean),
        R("R10", "TEST BAKKAL 10", 10, 236.70m, 47.34m, "KREDI KARTI", ReceiptVisual.Clean),
        R("R11", "TEST TERMAL 11", 11, 667.25m, 133.45m, "KREDI KARTI", ReceiptVisual.Faded, true),
        R("R12", "TEST AKSAM 12", 12, 184.90m, 36.98m, "NAKIT", ReceiptVisual.Dark, true),
        R("R13", "TEST UZUN 13", 13, 1450.00m, 290.00m, "KREDI KARTI", ReceiptVisual.Long, true),
        R("R14", "TEST GOLGE 14", 14, 349.60m, 69.92m, "BANKA KARTI", ReceiptVisual.Shadowed, true),
        R("R15", "TEST BURUSUK 15", 15, 522.35m, 104.47m, "NAKIT", ReceiptVisual.Crumpled, true),
        R("R16", "TEST SOLUK 16", 1, 278.45m, 55.69m, "KREDI KARTI", ReceiptVisual.LowContrast, true),
        R("R17", "TEST GURULTU 17", 2, 905.10m, 181.02m, "BANKA KARTI", ReceiptVisual.Noisy, true),
        R("R18", "TEST EGIK 18", 3, 118.75m, 23.75m, "NAKIT", ReceiptVisual.Tilted, true),
        R("R19", "TEST TERMAL 19", 4, 731.40m, 146.28m, "KREDI KARTI", ReceiptVisual.Faded, true),
        R("R20", "TEST GECE 20", 5, 264.30m, 52.86m, "NAKIT", ReceiptVisual.Dark, true),
        R("R21", "TEST UZUN 21", 6, 1888.80m, 377.76m, "KREDI KARTI", ReceiptVisual.Long, true),
        R("R22", "TEST GOLGE 22", 7, 414.15m, 82.83m, "BANKA KARTI", ReceiptVisual.Shadowed, true),
        R("R23", "TEST BURUSUK 23", 8, 619.90m, 123.98m, "NAKIT", ReceiptVisual.Crumpled, true),
        R("R24", "TEST SOLUK 24", 9, 157.35m, 31.47m, "KREDI KARTI", ReceiptVisual.LowContrast, true),
        R("R25", "TEST GURULTU 25", 10, 842.60m, 168.52m, "BANKA KARTI", ReceiptVisual.Noisy, true),
        R("R26", "TEST EGIK 26", 11, 296.25m, 59.25m, "NAKIT", ReceiptVisual.Tilted, true),
        R("R27", "TEST BUYUK 27", 12, 2134.55m, 426.91m, "KREDI KARTI", ReceiptVisual.Oversized, true),
        R("R28", "TEST KARANLIK 28", 13, 375.80m, 75.16m, "BANKA KARTI", ReceiptVisual.Dark, true),
        R("R29", "TEST TERMAL 29", 14, 489.95m, 97.99m, "NAKIT", ReceiptVisual.Faded, true),
        R("R30", "TEST UZUN 30", 15, 1675.25m, 335.05m, "KREDI KARTI", ReceiptVisual.Long, true)
    ];

    public static ReceiptImage Render(ReceiptReference receipt)
    {
        var (width, height) = receipt.Visual switch
        {
            ReceiptVisual.Oversized => (1800, 3000),
            ReceiptVisual.Long => (900, 2600),
            _ => (700, 1100)
        };
        var scale = width / 700f;
        var (background, ink) = Colours(receipt.Visual);

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(background);
        DrawPaperTexture(canvas, receipt, width, height);

        var checkpoint = canvas.Save();
        if (receipt.Visual == ReceiptVisual.Tilted)
            canvas.RotateDegrees(2.8f, width / 2f, height / 2f);

        using var paint = new SKPaint { Color = ink, IsAntialias = true };
        using var heading = new SKFont(SKTypeface.Default, 30 * scale);
        using var body = new SKFont(SKTypeface.Default, 22 * scale);
        using var total = new SKFont(SKTypeface.Default, 27 * scale);

        var left = 42 * scale;
        var y = 65 * scale;
        Draw(canvas, receipt.CounterpartyName, left, y, heading, paint);
        y += 44 * scale;
        Draw(
            canvas,
            $"TARIH: {receipt.PurchasedAt:dd.MM.yyyy}   SAAT: 14:32",
            left,
            y,
            body,
            paint);
        y += 38 * scale;
        Draw(canvas, $"FIS NO: {receipt.Id[1..]}", left, y, body, paint);
        y += 42 * scale;
        Draw(canvas, "--------------------------------", left, y, body, paint);

        var itemCount = receipt.Visual == ReceiptVisual.Long ? 27 : 6;
        var itemStep = receipt.Visual == ReceiptVisual.Long ? 45f : 39f;
        for (var index = 1; index <= itemCount; index++)
        {
            y += itemStep * scale;
            var itemAmount = 7.25m + index * 3.15m;
            Draw(
                canvas,
                $"TEST URUN {index:00}        {itemAmount,8:0.00}",
                left,
                y,
                body,
                paint);
        }

        y += 43 * scale;
        Draw(canvas, "--------------------------------", left, y, body, paint);
        y += 46 * scale;
        Draw(
            canvas,
            $"ARA TOPLAM             {Money(receipt.SubtotalAmount),10}",
            left,
            y,
            body,
            paint);
        y += 43 * scale;
        Draw(
            canvas,
            $"TOPLAM KDV              {Money(receipt.TaxAmount),10}",
            left,
            y,
            body,
            paint);
        y += 52 * scale;
        Draw(
            canvas,
            $"GENEL TOPLAM            {Money(receipt.TotalAmount),10}",
            left,
            y,
            total,
            paint);
        y += 48 * scale;
        Draw(canvas, receipt.PaymentLabel, left, y, body, paint);

        canvas.RestoreToCount(checkpoint);
        DrawForegroundDefects(canvas, receipt, width, height);

        using var encoded = bitmap.Encode(SKEncodedImageFormat.Jpeg, 92);
        if (encoded is null)
            throw new InvalidOperationException($"{receipt.Id} sentetik görseli kodlanamadı.");

        return new ReceiptImage(encoded.ToArray(), "image/jpeg");
    }

    private static ReceiptReference R(
        string id,
        string merchant,
        int day,
        decimal subtotal,
        decimal tax,
        string payment,
        ReceiptVisual visual,
        bool hard = false) => new(
            id,
            merchant,
            new DateOnly(2026, 8, day),
            subtotal,
            tax,
            subtotal + tax,
            payment,
            visual,
            hard);

    private static (SKColor Background, SKColor Ink) Colours(ReceiptVisual visual) => visual switch
    {
        ReceiptVisual.Dark => (new SKColor(42, 44, 48), new SKColor(158, 160, 162)),
        ReceiptVisual.Faded => (new SKColor(248, 246, 238), new SKColor(174, 171, 163)),
        ReceiptVisual.LowContrast => (new SKColor(190, 188, 180), new SKColor(105, 104, 100)),
        _ => (new SKColor(248, 246, 238), new SKColor(32, 32, 30))
    };

    private static void DrawPaperTexture(
        SKCanvas canvas,
        ReceiptReference receipt,
        int width,
        int height)
    {
        var random = new Random(receipt.Id.GetHashCode(StringComparison.Ordinal));
        if (receipt.Visual == ReceiptVisual.Noisy)
        {
            using var noise = new SKPaint { Color = new SKColor(80, 78, 72, 55) };
            for (var index = 0; index < 1400; index++)
                canvas.DrawCircle(random.Next(width), random.Next(height), random.Next(1, 4), noise);
        }

        if (receipt.Visual == ReceiptVisual.Shadowed)
        {
            using var shadow = new SKPaint { Color = new SKColor(25, 30, 35, 90) };
            canvas.DrawRect(width * 0.58f, 0, width * 0.42f, height, shadow);
            shadow.Color = new SKColor(25, 30, 35, 45);
            canvas.DrawRect(width * 0.42f, 0, width * 0.16f, height, shadow);
        }
    }

    private static void DrawForegroundDefects(
        SKCanvas canvas,
        ReceiptReference receipt,
        int width,
        int height)
    {
        if (receipt.Visual != ReceiptVisual.Crumpled)
            return;

        using var crease = new SKPaint
        {
            Color = new SKColor(85, 82, 76, 80),
            StrokeWidth = Math.Max(2, width / 280f),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
        using var highlight = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 105),
            StrokeWidth = Math.Max(4, width / 140f),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        var points = new[]
        {
            new SKPoint(width * 0.05f, height * 0.72f),
            new SKPoint(width * 0.24f, height * 0.63f),
            new SKPoint(width * 0.43f, height * 0.68f),
            new SKPoint(width * 0.62f, height * 0.78f),
            new SKPoint(width * 0.78f, height * 0.75f),
            new SKPoint(width * 0.95f, height * 0.66f)
        };
        for (var index = 1; index < points.Length; index++)
        {
            canvas.DrawLine(points[index - 1], points[index], highlight);
            canvas.DrawLine(points[index - 1], points[index], crease);
        }
    }

    private static string Money(decimal value) =>
        value.ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',');

    private static void Draw(
        SKCanvas canvas,
        string text,
        float x,
        float y,
        SKFont font,
        SKPaint paint) =>
        canvas.DrawText(text, x, y, SKTextAlign.Left, font, paint);
}
