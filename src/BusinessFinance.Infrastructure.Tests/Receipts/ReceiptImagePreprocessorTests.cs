using Microsoft.Extensions.Options;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Infrastructure.Receipts;
using SkiaSharp;

namespace BusinessFinance.Infrastructure.Tests.Receipts;

public sealed class ReceiptImagePreprocessorTests
{
    [Fact]
    public void Normalize_PhotoLongerThanTheCap_ScalesTheLongEdgeAndKeepsTheAspectRatio()
    {
        var result = Normalize(JpegOf(3000, 1500, SKColors.White));

        Assert.True(result.IsAccepted);
        Assert.True(result.Resized);
        Assert.Equal(3000, result.OriginalWidth);
        Assert.Equal(1536, result.Width);
        Assert.Equal(768, result.Height);
    }

    [Fact]
    public void Normalize_PhotoAlreadyUnderTheCap_IsLeftAtItsOwnSize()
    {
        var result = Normalize(JpegOf(800, 1200, SKColors.White));

        Assert.True(result.IsAccepted);
        Assert.False(result.Resized);
        Assert.Equal(800, result.Width);
        Assert.Equal(1200, result.Height);
    }

    /// <summary>
    /// The whole point of baking orientation in: a portrait receipt shot with the
    /// phone rotated arrives as landscape pixels plus a tag, and the model reads
    /// the pixels. Orientation 6 means "rotate 90° clockwise", so the axes swap.
    /// </summary>
    [Fact]
    public void Normalize_JpegCarryingAnExifRotation_BakesItIntoThePixels()
    {
        var sideways = WithExifOrientation(JpegOf(1200, 600, SKColors.White), orientation: 6);

        var result = Normalize(sideways);

        Assert.True(result.IsAccepted);
        Assert.True(result.OrientationCorrected);
        Assert.Equal(600, result.Width);
        Assert.Equal(1200, result.Height);
    }

    [Fact]
    public void Normalize_JpegWithoutAnExifRotation_LeavesTheAxesAlone()
    {
        var result = Normalize(JpegOf(1200, 600, SKColors.White));

        Assert.False(result.OrientationCorrected);
        Assert.Equal(1200, result.Width);
        Assert.Equal(600, result.Height);
    }

    [Fact]
    public void Normalize_UnderexposedPhoto_IsLifted()
    {
        var dark = JpegOf(400, 400, new SKColor(40, 40, 40));

        var result = Normalize(dark);

        Assert.True(result.ContrastAdjusted);
        Assert.True(
            MeanLuminanceOf(result.Image!.Content.Span) > MeanLuminanceOf(dark),
            "An underexposed receipt should come out brighter than it went in.");
    }

    /// <summary>
    /// A well-lit receipt is left alone on purpose. Every adjustment is a chance
    /// to lose faint thermal print, so the nudge is conditional, not routine.
    /// </summary>
    [Fact]
    public void Normalize_WellLitPhoto_IsNotTouched()
    {
        var result = Normalize(JpegOf(400, 400, new SKColor(230, 230, 230)));

        Assert.False(result.ContrastAdjusted);
    }

    [Fact]
    public void Normalize_PngInput_ComesOutAsJpeg()
    {
        var png = Encode(600, 600, SKColors.White, SKEncodedImageFormat.Png);

        var result = Normalize(png);

        Assert.True(result.IsAccepted);
        Assert.Equal("image/jpeg", result.Image!.ContentType);
        Assert.Equal(SKEncodedImageFormat.Jpeg, FormatOf(result.Image.Content.Span));
    }

    [Fact]
    public void Normalize_ContentThatIsNotAnImage_IsRejectedInsteadOfThrowing()
    {
        var result = Normalize("bu bir fotoğraf değil"u8.ToArray());

        Assert.False(result.IsAccepted);
        Assert.NotNull(result.RejectionReason);
        Assert.Null(result.Image);
    }

    [Fact]
    public void PassThrough_ReportsTheSizeButReturnsTheSameBytes()
    {
        var original = JpegOf(3000, 1500, SKColors.White);
        var image = new ReceiptImage(original, "image/jpeg");

        var result = new PassThroughReceiptImagePreprocessor().Normalize(image);

        Assert.True(result.IsAccepted);
        Assert.Equal(3000, result.Width);
        Assert.False(result.Resized);
        Assert.False(result.OrientationCorrected);
        Assert.False(result.ContrastAdjusted);
        Assert.True(result.Image!.Content.Span.SequenceEqual(original));
    }

    [Fact]
    public void PassThrough_ContentThatIsNotAnImage_IsRejected()
    {
        var result = new PassThroughReceiptImagePreprocessor()
            .Normalize(new ReceiptImage("bu bir fotoğraf değil"u8.ToArray(), "image/jpeg"));

        Assert.False(result.IsAccepted);
    }

    private static ReceiptImageNormalization Normalize(
        byte[] content,
        ReceiptImageOptions? options = null)
    {
        var preprocessor = new ReceiptImagePreprocessor(
            Options.Create(options ?? new ReceiptImageOptions()));
        return preprocessor.Normalize(new ReceiptImage(content, "image/jpeg"));
    }

    private static byte[] JpegOf(int width, int height, SKColor color) =>
        Encode(width, height, color, SKEncodedImageFormat.Jpeg);

    private static byte[] Encode(int width, int height, SKColor color, SKEncodedImageFormat format)
    {
        using var bitmap = new SKBitmap(width, height);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(color);
        }

        using var data = bitmap.Encode(format, 95);
        return data.ToArray();
    }

    private static SKEncodedImageFormat FormatOf(ReadOnlySpan<byte> content)
    {
        using var data = SKData.CreateCopy(content);
        using var codec = SKCodec.Create(data);
        return codec.EncodedFormat;
    }

    private static double MeanLuminanceOf(ReadOnlySpan<byte> content)
    {
        using var data = SKData.CreateCopy(content);
        using var bitmap = SKBitmap.Decode(data);
        double total = 0;
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                total += (0.2126 * pixel.Red + 0.7152 * pixel.Green + 0.0722 * pixel.Blue) / 255d;
            }
        }

        return total / (bitmap.Width * bitmap.Height);
    }

    /// <summary>
    /// Splices a minimal EXIF APP1 segment carrying only the orientation tag in
    /// right after the SOI marker. Skia encodes no EXIF of its own, so a fixture
    /// with a rotation has to be assembled by hand — and hand-assembling it here
    /// beats checking a binary blob into the repository.
    /// </summary>
    private static byte[] WithExifOrientation(byte[] jpeg, ushort orientation)
    {
        var tiff = new List<byte>
        {
            0x49, 0x49, 0x2A, 0x00,          // little-endian TIFF header
            0x08, 0x00, 0x00, 0x00,          // IFD0 starts 8 bytes in
            0x01, 0x00,                      // one directory entry
            0x12, 0x01,                      // tag 0x0112 (Orientation)
            0x03, 0x00,                      // type 3 (SHORT)
            0x01, 0x00, 0x00, 0x00,          // count 1
            (byte)(orientation & 0xFF), (byte)(orientation >> 8), 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00           // no next IFD
        };

        var payload = new List<byte> { 0x45, 0x78, 0x69, 0x66, 0x00, 0x00 }; // "Exif\0\0"
        payload.AddRange(tiff);
        var length = payload.Count + 2;

        var segment = new List<byte> { 0xFF, 0xE1, (byte)(length >> 8), (byte)(length & 0xFF) };
        segment.AddRange(payload);

        var result = new List<byte>(jpeg.Length + segment.Count);
        result.AddRange(jpeg[..2]); // SOI
        result.AddRange(segment);
        result.AddRange(jpeg[2..]);
        return result.ToArray();
    }
}
