using Microsoft.Extensions.Options;
using BusinessFinance.Application.Receipts;
using SkiaSharp;

namespace BusinessFinance.Infrastructure.Receipts;

public sealed class ReceiptImageOptions
{
    public const string SectionName = "ReceiptImage";

    /// <summary>
    /// Longest edge after scaling. The model splits an image into 768x768 tiles
    /// and charges per tile, so a multiple of 768 wastes no partial tile. Beyond
    /// this size a receipt gains no readable detail, only tiles.
    /// </summary>
    public int MaxLongEdgePixels { get; init; } = 1536;

    public int JpegQuality { get; init; } = 85;

    /// <summary>
    /// Mean luminance (0..1) below which the photo counts as underexposed. Only
    /// then is contrast touched — a well-lit receipt is left alone, because every
    /// adjustment is a chance to lose faint thermal print.
    /// </summary>
    public float DarkImageLuminanceThreshold { get; init; } = 0.38f;

    /// <summary>
    /// Slope and offset of the contrast nudge applied to an underexposed photo.
    /// Deliberately mild: this brightens a dark receipt, it does not threshold it.
    /// </summary>
    public float ContrastAmount { get; init; } = 1.15f;
    public float BrightnessAmount { get; init; } = 0.06f;
}

/// <summary>
/// Geometric normalization only: orientation, scale, and a conditional contrast
/// nudge. No thresholding — see <see cref="IReceiptImagePreprocessor"/> for why,
/// and <see cref="PassThroughReceiptImagePreprocessor"/> for the variant this one
/// is measured against.
///
/// SkiaSharp rather than ImageSharp: ImageSharp 4 fails the build without a
/// license key, and a receipt scanner is not worth a licensing dependency. Skia
/// is MIT. Deploying to Linux later additionally needs the
/// SkiaSharp.NativeAssets.Linux package; Windows carries its natives already.
/// </summary>
internal sealed class ReceiptImagePreprocessor : IReceiptImagePreprocessor
{
    private readonly ReceiptImageOptions _options;

    public ReceiptImagePreprocessor(IOptions<ReceiptImageOptions> options)
    {
        _options = options.Value;
    }

    public ReceiptImageNormalization Normalize(ReceiptImage image)
    {
        using var data = SKData.CreateCopy(image.Content.Span);
        using var codec = SKCodec.Create(data);
        if (codec is null)
        {
            return ReceiptImageNormalization.Reject(
                "Fotoğraf çözümlenemedi; dosya bozuk veya desteklenmeyen bir biçimde.");
        }

        using var decoded = SKBitmap.Decode(codec);
        if (decoded is null)
        {
            return ReceiptImageNormalization.Reject(
                "Fotoğraf çözümlenemedi; dosya bozuk veya desteklenmeyen bir biçimde.");
        }

        var originalWidth = decoded.Width;
        var originalHeight = decoded.Height;

        // EXIF orientation is metadata, not pixels: a sideways photo decodes
        // sideways unless it is baked in. The model reads a rotated receipt
        // measurably worse, and this is the cheapest correction available.
        var origin = codec.EncodedOrigin;
        var orientationCorrected = origin != SKEncodedOrigin.TopLeft;
        using var upright = orientationCorrected ? ApplyOrigin(decoded, origin) : Copy(decoded);

        var longEdge = Math.Max(upright.Width, upright.Height);
        var resized = longEdge > _options.MaxLongEdgePixels;
        using var scaled = resized ? Scale(upright, longEdge) : Copy(upright);

        var contrastAdjusted = MeanLuminance(scaled) < _options.DarkImageLuminanceThreshold;
        using var finished = contrastAdjusted ? Brighten(scaled) : Copy(scaled);

        // Always JPEG on the way out, even for a PNG input. The receipt is a
        // photograph, the loss is invisible at this quality, and one output
        // format keeps the request body predictable. The attachment kept for the
        // user is the untouched original, so nothing lossy is archived.
        using var encoded = finished.Encode(SKEncodedImageFormat.Jpeg, _options.JpegQuality);
        if (encoded is null)
        {
            return ReceiptImageNormalization.Reject(
                "Fotoğraf yeniden kodlanamadı.");
        }

        return new ReceiptImageNormalization(
            true,
            null,
            new ReceiptImage(encoded.ToArray(), "image/jpeg"),
            originalWidth,
            originalHeight,
            finished.Width,
            finished.Height,
            orientationCorrected,
            resized,
            contrastAdjusted);
    }

    private static SKBitmap Copy(SKBitmap source) =>
        source.Copy() ?? throw new InvalidOperationException("Bitmap could not be copied.");

    private SKBitmap Scale(SKBitmap source, int longEdge)
    {
        var scale = (double)_options.MaxLongEdgePixels / longEdge;
        var width = Math.Max(1, (int)Math.Round(source.Width * scale));
        var height = Math.Max(1, (int)Math.Round(source.Height * scale));
        var target = new SKBitmap(width, height, source.ColorType, source.AlphaType);
        using var canvas = new SKCanvas(target);
        using var paint = new SKPaint { IsAntialias = true };
        canvas.DrawBitmap(
            source,
            SKRect.Create(width, height),
            new SKSamplingOptions(SKCubicResampler.Mitchell),
            paint);
        return target;
    }

    /// <summary>
    /// Bakes the EXIF orientation into the pixels. The eight cases are the eight
    /// EXIF values; writing them out beats a clever matrix nobody can check.
    /// </summary>
    private static SKBitmap ApplyOrigin(SKBitmap source, SKEncodedOrigin origin)
    {
        var swapsAxes = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
            or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var width = swapsAxes ? source.Height : source.Width;
        var height = swapsAxes ? source.Width : source.Height;

        var target = new SKBitmap(width, height, source.ColorType, source.AlphaType);
        using var canvas = new SKCanvas(target);
        switch (origin)
        {
            case SKEncodedOrigin.TopRight:
                canvas.Scale(-1, 1, width / 2f, height / 2f);
                break;
            case SKEncodedOrigin.BottomRight:
                canvas.RotateDegrees(180, width / 2f, height / 2f);
                break;
            case SKEncodedOrigin.BottomLeft:
                canvas.Scale(1, -1, width / 2f, height / 2f);
                break;
            case SKEncodedOrigin.LeftTop:
                canvas.Translate(width, 0);
                canvas.RotateDegrees(90);
                canvas.Scale(1, -1, source.Width / 2f, source.Height / 2f);
                break;
            case SKEncodedOrigin.RightTop:
                canvas.Translate(width, 0);
                canvas.RotateDegrees(90);
                break;
            case SKEncodedOrigin.RightBottom:
                canvas.Translate(0, height);
                canvas.RotateDegrees(270);
                canvas.Scale(1, -1, source.Width / 2f, source.Height / 2f);
                break;
            case SKEncodedOrigin.LeftBottom:
                canvas.Translate(0, height);
                canvas.RotateDegrees(270);
                break;
        }

        canvas.DrawBitmap(source, 0, 0, new SKSamplingOptions(SKFilterMode.Nearest));
        return target;
    }

    private SKBitmap Brighten(SKBitmap source)
    {
        var slope = _options.ContrastAmount;
        var shift = _options.BrightnessAmount;
        // Row-major 4x5 colour matrix: each channel is scaled around zero and then
        // offset, which lifts a dark photo without collapsing the tonal range the
        // model reads faint print from.
        var matrix = new[]
        {
            slope, 0f, 0f, 0f, shift,
            0f, slope, 0f, 0f, shift,
            0f, 0f, slope, 0f, shift,
            0f, 0f, 0f, 1f, 0f
        };

        var target = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using var canvas = new SKCanvas(target);
        using var paint = new SKPaint { ColorFilter = SKColorFilter.CreateColorMatrix(matrix) };
        canvas.DrawBitmap(source, 0, 0, new SKSamplingOptions(SKFilterMode.Nearest), paint);
        return target;
    }

    /// <summary>
    /// Measured on a thumbnail: the mean does not need every pixel, and a full
    /// pass over a multi-megapixel photo would cost more than the adjustment it
    /// decides on.
    /// </summary>
    private static float MeanLuminance(SKBitmap image)
    {
        var height = Math.Max(1, (int)Math.Round(image.Height * (64d / image.Width)));
        using var thumbnail = new SKBitmap(64, height, image.ColorType, image.AlphaType);
        using (var canvas = new SKCanvas(thumbnail))
        {
            canvas.DrawBitmap(
                image,
                SKRect.Create(64, height),
                new SKSamplingOptions(SKFilterMode.Linear));
        }

        double total = 0;
        for (var y = 0; y < thumbnail.Height; y++)
        {
            for (var x = 0; x < thumbnail.Width; x++)
            {
                var pixel = thumbnail.GetPixel(x, y);
                total += (0.2126 * pixel.Red + 0.7152 * pixel.Green + 0.0722 * pixel.Blue) / 255d;
            }
        }

        return (float)(total / (thumbnail.Width * thumbnail.Height));
    }
}

/// <summary>
/// The measurement baseline. Kept in the tree permanently, not as dead code: the
/// claim that geometric normalization helps a multimodal model is reasoned, not
/// measured, and this is the variant that can disprove it on real receipts.
/// </summary>
internal sealed class PassThroughReceiptImagePreprocessor : IReceiptImagePreprocessor
{
    public ReceiptImageNormalization Normalize(ReceiptImage image)
    {
        using var data = SKData.CreateCopy(image.Content.Span);
        using var codec = SKCodec.Create(data);
        if (codec is null)
        {
            return ReceiptImageNormalization.Reject(
                "Fotoğraf çözümlenemedi; dosya bozuk veya desteklenmeyen bir biçimde.");
        }

        return new ReceiptImageNormalization(
            true, null, image,
            codec.Info.Width, codec.Info.Height,
            codec.Info.Width, codec.Info.Height,
            false, false, false);
    }
}
