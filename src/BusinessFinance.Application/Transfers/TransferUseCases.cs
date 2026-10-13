using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transfers;

public sealed class CreateTransferUseCase(
    ICurrentUser currentUser,
    IAccountRepository accountRepository,
    ITransferRepository transferRepository,
    TimeProvider timeProvider,
    ICategoryRepository categoryRepository,
    IUserProfileRepository profileRepository)
{
    public async Task<ApplicationResult<TransferDto>> ExecuteAsync(
        CreateTransferCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TransferDto>.Failure(TransferErrors.AuthenticationRequired);
        }

        // İleri tarihli transferin yerine bir plan yolu yoktur; günü gelince
        // yazılır (bilerek kabul edilen sınır, 12 Ekim 2026).
        if (command.TransferDate > LocalDay.LatestAllowed(timeProvider.GetUtcNow()))
        {
            return ApplicationResult<TransferDto>.Failure(TransferErrors.DateInFuture);
        }

        var source = await accountRepository.FindOwnedByIdAsync(
            command.SourceAccountId,
            userId,
            cancellationToken);
        var destination = await accountRepository.FindOwnedByIdAsync(
            command.DestinationAccountId,
            userId,
            cancellationToken);
        if (source is null || destination is null || !source.IsActive || !destination.IsActive)
        {
            return ApplicationResult<TransferDto>.Failure(TransferErrors.AccountUnavailable);
        }

        try
        {
            if (command.FeeAmount < 0m || decimal.Round(command.FeeAmount, 4) != command.FeeAmount)
                return ApplicationResult<TransferDto>.Failure(TransferErrors.Validation("Fee must be nonnegative with at most four decimals."));
            _ = new Money(command.Amount + command.FeeAmount, command.Currency);
            BudgetTransaction? fee = null;
            if (command.FeeAmount > 0m)
            {
                var category = await categoryRepository.FindOrOpenAsync(userId, CategoryRole.BankFee, cancellationToken);
                if (!category.IsActive)
                    return ApplicationResult<TransferDto>.Failure(TransferErrors.Validation("The fee category is inactive."));
                var scope = await TransactionScopeResolution.ResolveAsync(command.FeeScope,
                    category.DefaultScope, source.DefaultScope, profileRepository, userId, cancellationToken);
                if (scope.Scope is not TransactionScope resolved)
                    return ApplicationResult<TransferDto>.Failure(TransferErrors.ScopeUnresolved);
                fee = new BudgetTransaction(Guid.NewGuid(), userId, source, category,
                    new Money(command.FeeAmount, command.Currency), TransactionType.Expense,
                    resolved, command.TransferDate);
            }
            var transfer = new Transfer(
                Guid.NewGuid(),
                userId,
                source,
                destination,
                new Money(command.Amount, command.Currency),
                command.TransferDate,
                command.Description,
                fee);
            await transferRepository.AddAsync(transfer, cancellationToken);
            return ApplicationResult<TransferDto>.Success(ToDto(transfer));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<TransferDto>.Failure(
                TransferErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<TransferDto>.Failure(
                TransferErrors.Validation(exception.Message));
        }
    }

    internal static TransferDto ToDto(Transfer transfer) => new(
        transfer.Id,
        transfer.SourceAccountId,
        transfer.DestinationAccountId,
        transfer.Amount.Amount,
        transfer.Amount.Currency,
        transfer.TransferDate,
        transfer.Description,
        transfer.IsCancelled,
        transfer.CancelledAtUtc,
        transfer.FeeTransactionId,
        transfer.FeeTransaction?.Amount.Amount ?? 0m,
        transfer.FeeTransaction?.CategoryId,
        transfer.FeeTransaction?.Scope);
}

public sealed class GetTransferUseCase(
    ICurrentUser currentUser,
    ITransferRepository transferRepository)
{
    public async Task<ApplicationResult<TransferDto>> ExecuteAsync(
        Guid transferId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TransferDto>.Failure(TransferErrors.AuthenticationRequired);
        }

        var transfer = await transferRepository.FindOwnedByIdAsync(
            transferId,
            userId,
            false,
            cancellationToken);
        return transfer is null
            ? ApplicationResult<TransferDto>.Failure(TransferErrors.NotFound(transferId))
            : ApplicationResult<TransferDto>.Success(CreateTransferUseCase.ToDto(transfer));
    }
}

public sealed class ListTransfersUseCase(
    ICurrentUser currentUser,
    ITransferRepository transferRepository)
{
    public async Task<ApplicationResult<BoundedList<TransferDto>>> ExecuteAsync(
        HistoryWindow window,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(window);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<BoundedList<TransferDto>>.Failure(
                TransferErrors.AuthenticationRequired);
        }

        var transfers = await transferRepository.ListAsync(userId, window, cancellationToken);
        return ApplicationResult<BoundedList<TransferDto>>.Success(
            new BoundedList<TransferDto>(
                transfers.Items.Select(CreateTransferUseCase.ToDto).ToArray(),
                transfers.HasMore));
    }
}

public sealed class CancelTransferUseCase(
    ICurrentUser currentUser,
    ITransferRepository transferRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<TransferDto>> ExecuteAsync(
        CancelTransferCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TransferDto>.Failure(TransferErrors.AuthenticationRequired);
        }

        var transfer = await transferRepository.FindOwnedByIdAsync(
            command.TransferId,
            userId,
            true,
            cancellationToken);
        if (transfer is null)
        {
            return ApplicationResult<TransferDto>.Failure(
                TransferErrors.NotFound(command.TransferId));
        }

        transfer.Cancel(timeProvider.GetUtcNow());
        await transferRepository.UpdateOwnedAsync(transfer, userId, cancellationToken);
        return ApplicationResult<TransferDto>.Success(CreateTransferUseCase.ToDto(transfer));
    }
}
