using System.Security.Cryptography;
using System.Text;
using BusinessFinance.Application.Attachments;

namespace BusinessFinance.Infrastructure.Attachments;

internal sealed class AttachmentFileInspector : IAttachmentFileInspector
{
    private static readonly byte[] PngSignature = [137, 80, 78, 71, 13, 10, 26, 10];
    private static readonly string[] DangerousPdfTokens =
        ["/JavaScript", "/JS", "/Launch", "/EmbeddedFile"];

    public AttachmentInspection Inspect(
        string fileName,
        string contentType,
        ReadOnlySpan<byte> content)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var mediaType = contentType.Split(';', 2)[0].Trim().ToLowerInvariant();
        var expected = extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => null
        };
        if (expected is null || mediaType != expected)
            return Reject("Yalnız türü ve uzantısı eşleşen PDF, JPEG ve PNG dosyaları yüklenebilir.");
        if (content.IsEmpty)
            return Reject("Belge boş olamaz.");
        if (ContainsAscii(content, "EICAR-STANDARD-ANTIVIRUS-TEST-FILE"))
            return Reject("Belge engellenen bir zararlı yazılım test imzasıyla eşleşti.");

        var signatureMatches = extension switch
        {
            ".pdf" => content.StartsWith("%PDF-"u8),
            ".jpg" or ".jpeg" => content.Length >= 4 &&
                content[0] == 0xFF && content[1] == 0xD8 && content[2] == 0xFF &&
                content[^2] == 0xFF && content[^1] == 0xD9,
            ".png" => content.StartsWith(PngSignature),
            _ => false
        };
        if (!signatureMatches)
            return Reject("Dosya içeriği, bildirilen dosya türüyle eşleşmiyor.");
        if (extension == ".pdf")
        {
            foreach (var token in DangerousPdfTokens)
            {
                if (ContainsAscii(content, token))
                    return Reject("Aktif veya gömülü içerik barındıran PDF dosyaları yüklenemez.");
            }
        }

        return new AttachmentInspection(
            true, null, expected, extension,
            Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant());
    }

    private static AttachmentInspection Reject(string reason) =>
        new(false, reason, null, null, null);

    private static bool ContainsAscii(ReadOnlySpan<byte> content, string value) =>
        Encoding.ASCII.GetString(content).Contains(value, StringComparison.OrdinalIgnoreCase);
}
