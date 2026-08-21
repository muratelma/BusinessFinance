using Microsoft.AspNetCore.Mvc;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Attachments;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Attachments;

public static class AttachmentEndpoints
{
    private const long MaximumMultipartBodyBytes = FinancialAttachment.MaximumSizeBytes + 65_536;

    public static IEndpointRouteBuilder MapAttachmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1").WithTags("Attachments")
            .RequireAuthorization();
        group.MapPost("/transactions/{transactionId:guid}/attachments", UploadAsync)
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<AttachmentResponse>(201).ProducesProblem(400).ProducesProblem(401)
            .ProducesProblem(404).ProducesProblem(413)
            .WithMetadata(new RequestSizeLimitAttribute(MaximumMultipartBodyBytes))
            .DisableAntiforgery();
        group.MapGet("/transactions/{transactionId:guid}/attachments", ListAsync)
            .Produces<AttachmentListResponse>().ProducesProblem(401).ProducesProblem(404);
        group.MapGet("/attachments/{attachmentId:guid}/content", DownloadAsync)
            .ProducesProblem(401).ProducesProblem(404);
        return endpoints;
    }

    private static async Task<IResult> UploadAsync(
        Guid transactionId,
        HttpRequest request,
        UploadAttachmentUseCase useCase,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        if (request.ContentLength > MaximumMultipartBodyBytes)
            return ApiProblemResults.Create(
                context, 413, "Payload is too large.",
                "Belge yükleme isteği izin verilen boyutu aşıyor.", "attachment.file_too_large");
        if (!request.HasFormContentType)
            return ApiProblemResults.Validation(
                context, "Content-Type must be multipart/form-data.", "attachment.invalid_content_type");
        var form = await request.ReadFormAsync(cancellationToken);
        var file = form.Files.GetFile("file");
        if (file is null || file.Length == 0)
            return ApiProblemResults.Validation(
                context, "Boş olmayan bir belge seçilmelidir.", "attachment.file_required");
        if (file.Length > FinancialAttachment.MaximumSizeBytes)
            return ApiProblemResults.Create(
                context, 413, "Payload is too large.",
                "Belge 5 MiB boyut sınırını aşıyor.", "attachment.file_too_large");

        await using var stream = file.OpenReadStream();
        var result = await useCase.ExecuteAsync(
            new UploadAttachmentCommand(
                transactionId, file.FileName, file.ContentType, stream, file.Length),
            cancellationToken);
        return result.IsSuccess
            ? Results.Created($"/api/v1/attachments/{result.Value.Id}/content", ToResponse(result.Value))
            : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> ListAsync(
        Guid transactionId,
        ListAttachmentsUseCase useCase,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(transactionId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new AttachmentListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> DownloadAsync(
        Guid attachmentId,
        DownloadAttachmentUseCase useCase,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(attachmentId, cancellationToken);
        return result.IsSuccess
            ? Results.Stream(
                result.Value.Content,
                result.Value.Metadata.ContentType,
                result.Value.Metadata.FileName,
                enableRangeProcessing: false)
            : result.Error.ToProblemResult(context);
    }

    internal static AttachmentResponse ToResponse(AttachmentDto item) => new(
        item.Id, item.TransactionId, item.FileName, item.ContentType,
        item.SizeBytes, item.Sha256, item.CreatedAtUtc);
}
