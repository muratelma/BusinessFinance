using Microsoft.Extensions.Options;
using BusinessFinance.Application.Attachments;

namespace BusinessFinance.Infrastructure.Attachments;

public sealed class AttachmentStorageOptions
{
    public const string SectionName = "AttachmentStorage";
    public string RootPath { get; init; } = "storage/attachments";
}

internal sealed class LocalAttachmentObjectStore : IAttachmentObjectStore
{
    private readonly string _rootPath;

    public LocalAttachmentObjectStore(IOptions<AttachmentStorageOptions> options)
    {
        _rootPath = Path.GetFullPath(options.Value.RootPath, AppContext.BaseDirectory);
    }

    public async Task WriteAsync(
        string objectKey,
        ReadOnlyMemory<byte> content,
        CancellationToken cancellationToken)
    {
        var target = Resolve(objectKey);
        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        var temporary = target + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (var stream = new FileStream(
                             temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                             81920, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await stream.WriteAsync(content, cancellationToken);
            }
            File.Move(temporary, target, false);
        }
        catch
        {
            if (File.Exists(temporary)) File.Delete(temporary);
            throw;
        }
    }

    public Task<Stream?> OpenReadAsync(string objectKey, CancellationToken cancellationToken)
    {
        var target = Resolve(objectKey);
        Stream? stream = File.Exists(target)
            ? new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.Read,
                81920, FileOptions.Asynchronous | FileOptions.SequentialScan)
            : null;
        return Task.FromResult(stream);
    }

    public Task DeleteIfExistsAsync(string objectKey, CancellationToken cancellationToken)
    {
        var target = Resolve(objectKey);
        if (File.Exists(target)) File.Delete(target);
        return Task.CompletedTask;
    }

    private string Resolve(string objectKey)
    {
        var target = Path.GetFullPath(
            objectKey.Replace('/', Path.DirectorySeparatorChar), _rootPath);
        var rootWithSeparator = _rootPath.TrimEnd(Path.DirectorySeparatorChar) +
                                Path.DirectorySeparatorChar;
        if (!target.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Attachment object key escaped the storage root.");
        return target;
    }
}
