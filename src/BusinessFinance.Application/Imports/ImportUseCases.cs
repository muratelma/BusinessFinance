using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Imports;

public sealed class StageCsvImportUseCase(
    ICurrentUser currentUser,
    ICsvImportParser parser,
    IImportBatchRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<ImportBatchDto>> ExecuteAsync(
        StageCsvImportCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.AuthenticationRequired);

        try
        {
            var parsed = await parser.ParseAsync(command.Content, command.Options, cancellationToken);
            var batch = new ImportBatch(
                Guid.NewGuid(), userId, command.FileName, parsed.FileFingerprint, command.FileSizeBytes,
                parsed.EncodingName, parsed.Delimiter,
                command.Options.Columns.DateColumn, command.Options.Columns.AmountColumn,
                command.Options.Columns.DescriptionColumn, command.Options.Columns.ReferenceColumn,
                command.Options.DateFormat, command.Options.DecimalSeparator,
                timeProvider.GetUtcNow());

            foreach (var row in parsed.Rows)
            {
                batch.AddRow(new ImportRow(
                    Guid.NewGuid(), userId, batch.Id, row.RowNumber, row.RawData,
                    row.TransactionDate, row.SignedAmount, command.Currency,
                    row.Description, row.ExternalReference, row.Errors));
            }

            var persisted = await repository.AddOrGetExistingAsync(batch, cancellationToken);
            return ApplicationResult<ImportBatchDto>.Success(ToDto(persisted));
        }
        catch (CsvImportParsingException exception)
        {
            return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.Validation(exception.Message));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.Validation(exception.Message));
        }
    }

    internal static ImportBatchDto ToDto(ImportBatch batch) => new(
        batch.Id, batch.FileName, batch.FileFingerprint, batch.FileSizeBytes, batch.EncodingName, batch.Delimiter,
        batch.DateColumn, batch.AmountColumn, batch.DescriptionColumn, batch.ReferenceColumn,
        batch.DateFormat, batch.DecimalSeparator, batch.Status, batch.CreatedAtUtc,
        batch.TotalRowCount, batch.ValidRowCount, batch.InvalidRowCount,
        batch.Rows.OrderBy(row => row.RowNumber).Select(row => new ImportRowDto(
            row.Id, row.RowNumber, row.RawData, row.TransactionDate, row.SignedAmount,
            row.Currency, row.Description, row.ExternalReference, row.AccountId, row.CategoryId,
            row.SignedAmount is decimal amount
                ? amount > 0m ? TransactionType.Income : TransactionType.Expense
                : null,
            row.Status, row.ErrorMessage, row.BudgetTransactionId,
            row.DuplicateTransactionId, row.DuplicateReason)).ToArray());
}

public sealed class GetImportBatchUseCase(
    ICurrentUser currentUser,
    IImportBatchRepository repository)
{
    public async Task<ApplicationResult<ImportBatchDto>> ExecuteAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.AuthenticationRequired);

        var batch = await repository.FindOwnedByIdAsync(batchId, userId, false, cancellationToken);
        return batch is null
            ? ApplicationResult<ImportBatchDto>.Failure(ImportErrors.NotFound(batchId))
            : ApplicationResult<ImportBatchDto>.Success(StageCsvImportUseCase.ToDto(batch));
    }
}

public sealed class UpdateImportCandidateUseCase(
    ICurrentUser currentUser,
    IImportBatchRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<ApplicationResult<ImportRowDto>> ExecuteAsync(
        UpdateImportCandidateCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.AuthenticationRequired);

        var batch = await repository.FindOwnedByIdAsync(command.BatchId, userId, true, cancellationToken);
        var row = batch?.Rows.SingleOrDefault(item => item.Id == command.RowId);
        if (batch is null || row is null)
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.NotFound(command.BatchId));

        var account = await accountRepository.FindOwnedByIdAsync(command.AccountId, userId, cancellationToken);
        var category = await categoryRepository.FindOwnedByIdAsync(command.CategoryId, userId, cancellationToken);
        if (account is null || category is null || !account.IsActive || !category.IsActive)
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.MappingUnavailable);

        try
        {
            row.ApplyCorrection(
                command.TransactionDate, command.SignedAmount, command.Description,
                command.ExternalReference, account, category);
            var duplicate = await repository.FindDuplicateAsync(
                userId, row.ExternalReference, row.TransactionDate!.Value,
                row.SignedAmount!.Value > 0m ? TransactionType.Income : TransactionType.Expense,
                decimal.Abs(row.SignedAmount.Value), row.Description, cancellationToken);
            if (duplicate is not null)
                row.FlagDuplicate(duplicate.TransactionId, duplicate.Reason);
            await repository.UpdateAsync(batch, cancellationToken);
            return ApplicationResult<ImportRowDto>.Success(
                StageCsvImportUseCase.ToDto(batch).Rows.Single(item => item.Id == row.Id));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.Conflict(exception.Message));
        }
        catch (ImportConcurrencyException exception)
        {
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.Conflict(exception.Message));
        }
    }
}

public sealed class ConfirmImportBatchUseCase(
    ICurrentUser currentUser,
    IImportBatchRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository)
{
    public const int MaximumConfirmationRows = 5000;

    public async Task<ApplicationResult<ImportBatchDto>> ExecuteAsync(
        ConfirmImportBatchCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.AuthenticationRequired);
        if (command.RowIds.Count == 0 || command.RowIds.Count > MaximumConfirmationRows)
            return ApplicationResult<ImportBatchDto>.Failure(
                ImportErrors.Validation($"Select between 1 and {MaximumConfirmationRows} rows."));

        var selectedIds = command.RowIds.ToHashSet();
        if (selectedIds.Count != command.RowIds.Count || selectedIds.Contains(Guid.Empty))
            return ApplicationResult<ImportBatchDto>.Failure(
                ImportErrors.Validation("Every row id must be unique and non-empty."));

        var batch = await repository.FindOwnedByIdAsync(command.BatchId, userId, true, cancellationToken);
        if (batch is null)
            return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.NotFound(command.BatchId));

        var rowLookup = batch.Rows.ToDictionary(row => row.Id);
        if (selectedIds.Any(id => !rowLookup.ContainsKey(id)))
            return ApplicationResult<ImportBatchDto>.Failure(
                ImportErrors.Validation("Every selected row must belong to the import batch."));
        var selected = command.RowIds.Select(id => rowLookup[id]).ToArray();
        if (selected.All(row => row.Status == ImportRowStatus.Imported))
            return ApplicationResult<ImportBatchDto>.Success(StageCsvImportUseCase.ToDto(batch));
        if (selected.Any(row => row.Status != ImportRowStatus.Ready))
            return ApplicationResult<ImportBatchDto>.Failure(
                ImportErrors.Conflict("Every selected row must be corrected and mapped before confirmation."));

        var transactions = new List<BudgetTransaction>(selected.Length);
        foreach (var row in selected)
        {
            var account = await accountRepository.FindOwnedByIdAsync(row.AccountId!.Value, userId, cancellationToken);
            var category = await categoryRepository.FindOwnedByIdAsync(row.CategoryId!.Value, userId, cancellationToken);
            if (account is null || category is null || !account.IsActive || !category.IsActive)
                return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.MappingUnavailable);

            try
            {
                var transaction = row.CreateTransaction(account, category, Guid.NewGuid());
                row.MarkImported(transaction.Id);
                transactions.Add(transaction);
            }
            catch (ArgumentException exception)
            {
                return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.Validation(exception.Message));
            }
            catch (InvalidOperationException exception)
            {
                return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.Conflict(exception.Message));
            }
        }

        batch.RecordConfirmation();
        try
        {
            await repository.ConfirmAsync(batch, transactions, cancellationToken);
            return ApplicationResult<ImportBatchDto>.Success(StageCsvImportUseCase.ToDto(batch));
        }
        catch (ImportConcurrencyException exception)
        {
            return ApplicationResult<ImportBatchDto>.Failure(ImportErrors.Conflict(exception.Message));
        }
    }
}

public sealed class ResolveImportDuplicateUseCase(
    ICurrentUser currentUser,
    IImportBatchRepository repository)
{
    public async Task<ApplicationResult<ImportRowDto>> ExecuteAsync(
        ResolveImportDuplicateCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.AuthenticationRequired);
        var batch = await repository.FindOwnedByIdAsync(command.BatchId, userId, true, cancellationToken);
        var row = batch?.Rows.SingleOrDefault(item => item.Id == command.RowId);
        if (batch is null || row is null)
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.NotFound(command.BatchId));
        try
        {
            row.ResolveDuplicate(command.Decision == DuplicateDecision.ImportAnyway);
            if (row.Status == ImportRowStatus.SkippedDuplicate) batch.RecordConfirmation();
            await repository.UpdateAsync(batch, cancellationToken);
            return ApplicationResult<ImportRowDto>.Success(
                StageCsvImportUseCase.ToDto(batch).Rows.Single(item => item.Id == row.Id));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.Conflict(exception.Message));
        }
        catch (ImportConcurrencyException exception)
        {
            return ApplicationResult<ImportRowDto>.Failure(ImportErrors.Conflict(exception.Message));
        }
    }
}
