namespace BusinessFinance.Application.Receipts;

/// <summary>
/// A receipt photo on its way to the analyzer. Bytes only: the layer that reads
/// receipts never learns where the file came from or who uploaded it.
/// </summary>
public sealed record ReceiptImage(
    ReadOnlyMemory<byte> Content,
    string ContentType);

/// <summary>
/// The outcome of normalization. Mirrors the attachment inspector's shape
/// (accepted plus a reason when it is not) so both gates read the same way.
///
/// The counters are not decoration: a measurement run compares two
/// preprocessor variants and needs to tell from the result which one produced a
/// reading, without re-deriving it from the bytes.
/// </summary>
public sealed record ReceiptImageNormalization(
    bool IsAccepted,
    string? RejectionReason,
    ReceiptImage? Image,
    int OriginalWidth,
    int OriginalHeight,
    int Width,
    int Height,
    bool OrientationCorrected,
    bool Resized,
    bool ContrastAdjusted)
{
    public static ReceiptImageNormalization Reject(string reason) =>
        new(false, reason, null, 0, 0, 0, 0, false, false, false);
}

/// <summary>
/// Geometric normalization applied before a receipt reaches the model.
///
/// Deliberately excludes binarization and thresholding. Those help classic OCR
/// engines that match glyph shapes, but a multimodal model reads faint thermal
/// print from context — and a threshold erases exactly that faint print before
/// the model ever sees it. Stage 12.10 keeps the decision honest rather than
/// assumed: a pass-through implementation stays in the tree permanently so both
/// variants can be measured on the same receipts.
/// </summary>
public interface IReceiptImagePreprocessor
{
    ReceiptImageNormalization Normalize(ReceiptImage image);
}
