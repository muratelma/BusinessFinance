using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.DataPortability;

public sealed class ExportTransactionsCsvUseCase(
    ICurrentUser currentUser,
    IDataPortabilityRepository repository)
{
    public Task<ApplicationResult<PortableFile>> ExecuteAsync(CancellationToken cancellationToken = default) =>
        ExecuteOwnedAsync(currentUser, id => repository.ExportTransactionsCsvAsync(id, cancellationToken));

    internal static async Task<ApplicationResult<T>> ExecuteOwnedAsync<T>(
        ICurrentUser currentUser,
        Func<Guid, Task<T>> action) where T : notnull
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<T>.Failure(DataPortabilityErrors.AuthenticationRequired);
        try
        {
            return ApplicationResult<T>.Success(await action(userId));
        }
        catch (DataPortabilityException exception)
        {
            return ApplicationResult<T>.Failure(new ApplicationError(
                exception.Code, exception.Message, exception.ErrorType));
        }
    }
}

/// <summary>
/// Cari defterin kendi dosyası.
/// </summary>
/// <remarks>
/// İşlem CSV'sine kolon eklenmedi: o dosya <c>BudgetTransaction</c> tablosunun
/// dökümüdür ve cari hareket orada hiç bulunmaz — kolon her satırda boş kalır,
/// dosya da eksik kalmaya devam ederdi. Her dışa aktarma tek kaydın dökümü
/// olduğu sürece kullanıcı ne okuduğunu bilir.
/// </remarks>
public sealed class ExportCounterpartyLedgerCsvUseCase(
    ICurrentUser currentUser,
    IDataPortabilityRepository repository)
{
    public Task<ApplicationResult<PortableFile>> ExecuteAsync(CancellationToken cancellationToken = default) =>
        ExportTransactionsCsvUseCase.ExecuteOwnedAsync(
            currentUser,
            id => repository.ExportCounterpartyLedgerCsvAsync(id, cancellationToken));
}

public sealed class ExportFinancialJsonUseCase(
    ICurrentUser currentUser,
    IDataPortabilityRepository repository)
{
    public Task<ApplicationResult<PortableFile>> ExecuteAsync(CancellationToken cancellationToken = default) =>
        ExportTransactionsCsvUseCase.ExecuteOwnedAsync(
            currentUser,
            id => repository.ExportFinancialJsonAsync(id, cancellationToken));
}

public sealed class CreateBackupUseCase(
    ICurrentUser currentUser,
    IDataPortabilityRepository repository)
{
    public Task<ApplicationResult<PortableFile>> ExecuteAsync(CancellationToken cancellationToken = default) =>
        ExportTransactionsCsvUseCase.ExecuteOwnedAsync(
            currentUser,
            id => repository.CreateBackupAsync(id, cancellationToken));
}

public sealed class ValidateBackupUseCase(
    ICurrentUser currentUser,
    IDataPortabilityRepository repository)
{
    public Task<ApplicationResult<BackupValidationDto>> ExecuteAsync(
        byte[] content,
        CancellationToken cancellationToken = default) =>
        ExportTransactionsCsvUseCase.ExecuteOwnedAsync(
            currentUser,
            _ => repository.ValidateBackupAsync(content, cancellationToken));
}

public sealed class RestoreBackupUseCase(
    ICurrentUser currentUser,
    IDataPortabilityRepository repository,
    TimeProvider timeProvider)
{
    public Task<ApplicationResult<RestoreSummaryDto>> ExecuteAsync(
        byte[] content,
        CancellationToken cancellationToken = default) =>
        ExportTransactionsCsvUseCase.ExecuteOwnedAsync(
            currentUser,
            id => repository.RestoreBackupAsync(
                id, content, timeProvider.GetUtcNow(), cancellationToken));
}
