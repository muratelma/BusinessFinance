using Microsoft.AspNetCore.Mvc;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.DataPortability;

namespace BusinessFinance.Api.Features.DataPortability;

public static class DataPortabilityEndpoints
{
    internal const int MaximumBackupEnvelopeBytes = 14 * 1024 * 1024;
    private const long MaximumMultipartBodyBytes = MaximumBackupEnvelopeBytes + 65_536;

    public static IEndpointRouteBuilder MapDataPortabilityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var exports = endpoints.MapGroup("/api/v1/exports")
            .WithTags("Data Portability")
            .RequireAuthorization();
        exports.MapGet("/transactions.csv", ExportTransactionsCsvAsync)
            .WithName("ExportTransactionsCsv")
            .Produces(StatusCodes.Status200OK, contentType: "text/csv")
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        exports.MapGet("/financial-data.json", ExportFinancialJsonAsync)
            .WithName("ExportFinancialDataJson")
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        var backups = endpoints.MapGroup("/api/v1/backups")
            .WithTags("Backups")
            .RequireAuthorization();
        backups.MapGet("/download", CreateBackupAsync)
            .WithName("CreateFinancialBackup")
            .Produces(StatusCodes.Status200OK, contentType: "application/vnd.business-finance.backup+json")
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        backups.MapPost("/validate", ValidateBackupAsync)
            .WithName("ValidateFinancialBackup")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<BackupValidationResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
            .WithMetadata(new RequestSizeLimitAttribute(MaximumMultipartBodyBytes))
            .DisableAntiforgery();
        backups.MapPost("/restore", RestoreBackupAsync)
            .WithName("RestoreFinancialBackup")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<RestoreSummaryResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
            .WithMetadata(new RequestSizeLimitAttribute(MaximumMultipartBodyBytes))
            .DisableAntiforgery();
        return endpoints;
    }

    private static async Task<IResult> ExportTransactionsCsvAsync(
        ExportTransactionsCsvUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ExportFinancialJsonAsync(
        ExportFinancialJsonUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CreateBackupAsync(
        CreateBackupUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ValidateBackupAsync(
        HttpRequest request,
        ValidateBackupUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var content = await ReadBackupAsync(request, httpContext, cancellationToken);
        if (content.Result is not null) return content.Result;
        var result = await useCase.ExecuteAsync(content.Content!, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new BackupValidationResponse(
                result.Value.SchemaVersion,
                result.Value.CreatedAtUtc,
                result.Value.EntityCount,
                result.Value.PayloadSha256))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> RestoreBackupAsync(
        HttpRequest request,
        RestoreBackupUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var content = await ReadBackupAsync(request, httpContext, cancellationToken);
        if (content.Result is not null) return content.Result;
        var result = await useCase.ExecuteAsync(content.Content!, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new RestoreSummaryResponse(
                result.Value.SchemaVersion,
                result.Value.RestoredEntityCount,
                result.Value.RestoredAtUtc))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<(byte[]? Content, IResult? Result)> ReadBackupAsync(
        HttpRequest request,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request.ContentLength > MaximumMultipartBodyBytes)
            return (null, ApiProblemResults.Create(httpContext, 413, "Payload is too large.",
                "Backup request exceeds the allowed size.", "backup.file_too_large"));
        if (!request.HasFormContentType)
            return (null, ApiProblemResults.Validation(httpContext,
                "Content-Type must be multipart/form-data.", "backup.invalid_content_type"));
        var form = await request.ReadFormAsync(cancellationToken);
        var file = form.Files.GetFile("file");
        if (file is null || file.Length == 0)
            return (null, ApiProblemResults.Validation(httpContext,
                "A non-empty backup file is required.", "backup.file_required"));
        if (file.Length > MaximumBackupEnvelopeBytes)
            return (null, ApiProblemResults.Create(httpContext, 413, "Payload is too large.",
                "Backup file exceeds the allowed size.", "backup.file_too_large"));

        await using var source = file.OpenReadStream();
        using var buffer = new MemoryStream((int)file.Length);
        await source.CopyToAsync(buffer, cancellationToken);
        if (buffer.Length > MaximumBackupEnvelopeBytes)
            return (null, ApiProblemResults.Create(httpContext, 413, "Payload is too large.",
                "Backup file exceeds the allowed size.", "backup.file_too_large"));
        return (buffer.ToArray(), null);
    }
}

public sealed record BackupValidationResponse(
    int SchemaVersion,
    DateTimeOffset CreatedAtUtc,
    int EntityCount,
    string PayloadSha256);

public sealed record RestoreSummaryResponse(
    int SchemaVersion,
    int RestoredEntityCount,
    DateTimeOffset RestoredAtUtc);
