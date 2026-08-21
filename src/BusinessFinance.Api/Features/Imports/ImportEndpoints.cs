using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Imports;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Imports;
using Microsoft.AspNetCore.Mvc;

namespace BusinessFinance.Api.Features.Imports;

public static class ImportEndpoints
{
    private const long MaximumMultipartBodyBytes = CsvImportParser.MaximumFileSizeBytes + 65_536;
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "text/csv", "application/csv", "application/vnd.ms-excel"
    };

    public static IEndpointRouteBuilder MapImportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/imports")
            .WithTags("Imports")
            .RequireAuthorization();

        group.MapPost("/csv/stage", StageAsync)
            .WithName("StageCsvImport")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<ImportBatchResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
            .WithMetadata(new RequestSizeLimitAttribute(MaximumMultipartBodyBytes))
            .DisableAntiforgery();

        group.MapGet("/{batchId:guid}", GetAsync)
            .WithName("GetImportBatch")
            .Produces<ImportBatchResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPatch("/{batchId:guid}/rows/{rowId:guid}", UpdateCandidateAsync)
            .WithName("UpdateImportCandidate")
            .Produces<ImportRowResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{batchId:guid}/confirm", ConfirmAsync)
            .WithName("ConfirmImportBatch")
            .Produces<ImportBatchResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithMetadata(new RequestSizeLimitAttribute(256 * 1024));

        group.MapPatch("/{batchId:guid}/rows/{rowId:guid}/duplicate-decision", ResolveDuplicateAsync)
            .WithName("ResolveImportDuplicate")
            .Produces<ImportRowResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> StageAsync(
        HttpRequest request,
        StageCsvImportUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request.ContentLength > MaximumMultipartBodyBytes)
            return ApiProblemResults.Create(httpContext, 413, "Payload is too large.",
                "Multipart request exceeds the allowed size.", "imports.file_too_large");
        if (!request.HasFormContentType)
            return ApiProblemResults.Validation(httpContext,
                "Content-Type must be multipart/form-data.", "imports.invalid_content_type");

        var form = await request.ReadFormAsync(cancellationToken);
        var file = form.Files.GetFile("file");
        if (file is null || file.Length == 0)
            return ApiProblemResults.Validation(httpContext, "A non-empty CSV file is required.", "imports.file_required");
        if (file.Length > CsvImportParser.MaximumFileSizeBytes)
            return ApiProblemResults.Create(httpContext, 413, "Payload is too large.",
                $"CSV file cannot exceed {CsvImportParser.MaximumFileSizeBytes} bytes.", "imports.file_too_large");
        if (!string.Equals(Path.GetExtension(file.FileName), ".csv", StringComparison.OrdinalIgnoreCase))
            return ApiProblemResults.Validation(httpContext, "File extension must be .csv.", "imports.invalid_file_type");

        var mediaType = file.ContentType.Split(';', 2)[0].Trim();
        if (!AllowedContentTypes.Contains(mediaType))
            return ApiProblemResults.Validation(httpContext,
                "CSV content type must be text/csv, application/csv, or application/vnd.ms-excel.",
                "imports.invalid_file_type");

        var dateColumn = form["dateColumn"].ToString();
        var amountColumn = form["amountColumn"].ToString();
        if (string.IsNullOrWhiteSpace(dateColumn) || string.IsNullOrWhiteSpace(amountColumn))
            return ApiProblemResults.Validation(httpContext,
                "dateColumn and amountColumn mappings are required.", "imports.missing_column_mapping");

        var dateFormat = ValueOrDefault(form["dateFormat"].ToString(), "yyyy-MM-dd");
        if (dateFormat is not ("yyyy-MM-dd" or "dd.MM.yyyy" or "dd/MM/yyyy"))
            return ApiProblemResults.Validation(httpContext,
                "dateFormat must be yyyy-MM-dd, dd.MM.yyyy, or dd/MM/yyyy.", "imports.invalid_date_format");

        var decimalSeparatorText = ValueOrDefault(form["decimalSeparator"].ToString(), ".");
        if (decimalSeparatorText is not ("." or ","))
            return ApiProblemResults.Validation(httpContext,
                "decimalSeparator must be . or ,.", "imports.invalid_decimal_separator");

        var currency = ValueOrDefault(form["currency"].ToString(), "TRY");
        if (!string.Equals(currency, "TRY", StringComparison.OrdinalIgnoreCase))
            return ApiProblemResults.Validation(httpContext,
                "Only TRY currency is currently supported.", "imports.invalid_currency");

        await using var stream = file.OpenReadStream();
        var result = await useCase.ExecuteAsync(new StageCsvImportCommand(
            stream,
            SafeFileName(file.FileName),
            file.Length,
            CurrencyCode.TRY,
            new CsvParsingOptions(
                ValueOrDefault(form["encoding"].ToString(), "auto"),
                ValueOrDefault(form["delimiter"].ToString(), "auto"),
                dateFormat,
                decimalSeparatorText[0],
                new CsvColumnMapping(
                    dateColumn,
                    amountColumn,
                    NullIfWhiteSpace(form["descriptionColumn"].ToString()),
                    NullIfWhiteSpace(form["referenceColumn"].ToString())))),
            cancellationToken);

        if (!result.IsSuccess) return result.Error.ToProblemResult(httpContext);
        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/imports/{response.Id}", response);
    }

    private static async Task<IResult> GetAsync(
        Guid batchId,
        GetImportBatchUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(batchId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> UpdateCandidateAsync(
        Guid batchId,
        Guid rowId,
        UpdateImportCandidateRequest request,
        UpdateImportCandidateUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(request.TransactionDate, out var transactionDate))
            return ApiProblemResults.Validation(httpContext,
                "Transaction date must use the yyyy-MM-dd format.", "imports.invalid_date");
        if (!FinanceContract.TryParseAmount(request.SignedAmount, out var signedAmount) || signedAmount == 0m)
            return ApiProblemResults.Validation(httpContext,
                "Signed amount must be non-zero and have at most four decimal places.", "imports.invalid_amount");

        var result = await useCase.ExecuteAsync(new UpdateImportCandidateCommand(
            batchId, rowId, transactionDate, signedAmount, request.Description,
            request.ExternalReference, request.AccountId, request.CategoryId), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToRowResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ConfirmAsync(
        Guid batchId,
        ConfirmImportBatchRequest request,
        ConfirmImportBatchUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request.RowIds is null)
            return ApiProblemResults.Validation(httpContext,
                "rowIds is required.", "imports.invalid_row_selection");
        var result = await useCase.ExecuteAsync(
            new ConfirmImportBatchCommand(batchId, request.RowIds), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ResolveDuplicateAsync(
        Guid batchId,
        Guid rowId,
        ResolveImportDuplicateRequest request,
        ResolveImportDuplicateUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var decision = request.Decision?.Trim().ToLowerInvariant() switch
        {
            "skip" => DuplicateDecision.Skip,
            "import-anyway" => DuplicateDecision.ImportAnyway,
            _ => (DuplicateDecision?)null
        };
        if (decision is null)
            return ApiProblemResults.Validation(httpContext,
                "Decision must be skip or import-anyway.", "imports.invalid_duplicate_decision");
        var result = await useCase.ExecuteAsync(
            new ResolveImportDuplicateCommand(batchId, rowId, decision.Value), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToRowResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    internal static ImportBatchResponse ToResponse(ImportBatchDto batch) => new(
        batch.Id, batch.FileName, batch.FileFingerprint, batch.FileSizeBytes, batch.EncodingName,
        batch.Delimiter == "\t" ? "tab" : batch.Delimiter,
        batch.DateColumn, batch.AmountColumn, batch.DescriptionColumn, batch.ReferenceColumn,
        batch.DateFormat, batch.DecimalSeparator,
        batch.Status == ImportBatchStatus.PartiallyImported
            ? "partially-imported"
            : batch.Status.ToString().ToLowerInvariant(),
        batch.CreatedAtUtc, batch.TotalRowCount, batch.ValidRowCount, batch.InvalidRowCount,
        batch.Rows.Select(ToRowResponse).ToArray());

    private static ImportRowResponse ToRowResponse(ImportRowDto row) => new(
        row.Id, row.RowNumber,
        row.RawData,
        row.TransactionDate is DateOnly date ? FinanceContract.Date(date) : null,
        row.SignedAmount is decimal amount ? FinanceContract.Money(amount) : null,
        row.Currency.ToString(), row.Description, row.ExternalReference,
        row.AccountId, row.CategoryId,
        row.TransactionType?.ToString().ToLowerInvariant(),
        RowStatusValue(row.Status), row.ErrorMessage, row.BudgetTransactionId,
        row.DuplicateTransactionId, row.DuplicateReason switch
        {
            ImportDuplicateReason.BankReference => "bank-reference",
            ImportDuplicateReason.DateAmountDescription => "date-amount-description",
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(row))
        });

    private static string RowStatusValue(ImportRowStatus status) => status switch
    {
        ImportRowStatus.PendingDuplicateReview => "pending-duplicate-review",
        ImportRowStatus.SkippedDuplicate => "skipped-duplicate",
        _ => status.ToString().ToLowerInvariant()
    };

    private static string ValueOrDefault(string value, string defaultValue) =>
        string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();

    private static string? NullIfWhiteSpace(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string SafeFileName(string value) =>
        Path.GetFileName(value.Replace('\\', '/'));
}
