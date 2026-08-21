using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Attachments;

public static class AttachmentErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "auth.authentication_required", "Authentication is required.", ApplicationErrorType.Unauthorized);
    public static readonly ApplicationError FileTooLarge = new(
        "attachment.file_too_large", "Belge 5 MiB boyut sınırını aşıyor.", ApplicationErrorType.Validation);
    public static ApplicationError TransactionNotFound(Guid id) => new(
        "attachment.transaction_not_found", $"'{id}' kimlikli işlem bulunamadı.", ApplicationErrorType.NotFound);
    public static ApplicationError NotFound(Guid id) => new(
        "attachment.not_found", $"'{id}' kimlikli belge bulunamadı.", ApplicationErrorType.NotFound);
    public static ApplicationError Unsafe(string message) => new(
        "attachment.unsafe_file", message, ApplicationErrorType.Validation);
}

public sealed class UploadAttachmentUseCase(
    ICurrentUser currentUser,
    ITransactionRepository transactionRepository,
    IAttachmentRepository repository,
    IAttachmentFileInspector inspector,
    IAttachmentObjectStore objectStore,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<AttachmentDto>> ExecuteAsync(
        UploadAttachmentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<AttachmentDto>.Failure(AttachmentErrors.AuthenticationRequired);
        if (command.DeclaredLength is < 1 or > FinancialAttachment.MaximumSizeBytes)
            return ApplicationResult<AttachmentDto>.Failure(AttachmentErrors.FileTooLarge);
        if (await transactionRepository.FindOwnedByIdAsync(
                command.TransactionId, userId, false, cancellationToken) is null)
            return ApplicationResult<AttachmentDto>.Failure(
                AttachmentErrors.TransactionNotFound(command.TransactionId));

        await using var buffer = new MemoryStream((int)command.DeclaredLength);
        var chunk = new byte[81920];
        long total = 0;
        int read;
        while ((read = await command.Content.ReadAsync(chunk, cancellationToken)) > 0)
        {
            total += read;
            if (total > FinancialAttachment.MaximumSizeBytes)
                return ApplicationResult<AttachmentDto>.Failure(AttachmentErrors.FileTooLarge);
            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
        }
        if (total != command.DeclaredLength)
            return ApplicationResult<AttachmentDto>.Failure(
                AttachmentErrors.Unsafe("Declared and actual file lengths do not match."));
        var content = buffer.ToArray();
        var inspection = inspector.Inspect(command.FileName, command.ContentType, content);
        if (!inspection.IsAccepted)
            return ApplicationResult<AttachmentDto>.Failure(
                AttachmentErrors.Unsafe(inspection.RejectionReason ?? "Attachment was rejected."));

        var id = Guid.NewGuid();
        var objectKey = $"attachments/{userId:N}/{id:N}{inspection.Extension}";
        var attachment = new FinancialAttachment(
            id, userId, command.TransactionId, command.FileName,
            inspection.NormalizedContentType!, total, inspection.Sha256!, objectKey,
            timeProvider.GetUtcNow());
        await objectStore.WriteAsync(objectKey, content, cancellationToken);
        try
        {
            await repository.AddAsync(attachment, cancellationToken);
        }
        catch
        {
            await objectStore.DeleteIfExistsAsync(objectKey, CancellationToken.None);
            throw;
        }
        return ApplicationResult<AttachmentDto>.Success(ToDto(attachment));
    }

    internal static AttachmentDto ToDto(FinancialAttachment item) => new(
        item.Id, item.TransactionId, item.OriginalFileName, item.ContentType,
        item.SizeBytes, item.Sha256, item.CreatedAtUtc);
}

public sealed class ListAttachmentsUseCase(
    ICurrentUser currentUser,
    ITransactionRepository transactionRepository,
    IAttachmentRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<AttachmentDto>>> ExecuteAsync(
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<IReadOnlyList<AttachmentDto>>.Failure(
                AttachmentErrors.AuthenticationRequired);
        if (await transactionRepository.FindOwnedByIdAsync(
                transactionId, userId, false, cancellationToken) is null)
            return ApplicationResult<IReadOnlyList<AttachmentDto>>.Failure(
                AttachmentErrors.TransactionNotFound(transactionId));
        var items = await repository.ListAsync(transactionId, userId, cancellationToken);
        return ApplicationResult<IReadOnlyList<AttachmentDto>>.Success(
            items.Select(UploadAttachmentUseCase.ToDto).ToArray());
    }
}

public sealed class DownloadAttachmentUseCase(
    ICurrentUser currentUser,
    IAttachmentRepository repository,
    IAttachmentObjectStore objectStore)
{
    public async Task<ApplicationResult<AttachmentDownload>> ExecuteAsync(
        Guid attachmentId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<AttachmentDownload>.Failure(AttachmentErrors.AuthenticationRequired);
        var attachment = await repository.FindOwnedByIdAsync(attachmentId, userId, cancellationToken);
        if (attachment is null)
            return ApplicationResult<AttachmentDownload>.Failure(AttachmentErrors.NotFound(attachmentId));
        var stream = await objectStore.OpenReadAsync(attachment.ObjectKey, cancellationToken);
        if (stream is null)
            return ApplicationResult<AttachmentDownload>.Failure(AttachmentErrors.NotFound(attachmentId));
        return ApplicationResult<AttachmentDownload>.Success(
            new AttachmentDownload(UploadAttachmentUseCase.ToDto(attachment), stream));
    }
}
