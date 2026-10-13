using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transfers;

public sealed record TransferPreview(Guid SourceAccountId, string SourceAccountName, decimal TotalOutflow,
    decimal FeeAmount, Guid? FeeCategoryId, string? FeeCategoryName, TransactionScope? ResolvedScope, bool NeedsScope);

public sealed class PreviewTransferUseCase(ICurrentUser currentUser, IAccountRepository accounts,
    ICategoryRepository categories, IUserProfileRepository profiles, TimeProvider clock)
{
    public async Task<ApplicationResult<TransferPreview>> ExecuteAsync(CreateTransferCommand command, CancellationToken ct)
    {
        if (currentUser.UserId is not Guid owner || owner == Guid.Empty)
            return ApplicationResult<TransferPreview>.Failure(TransferErrors.AuthenticationRequired);
        if (command.TransferDate > LocalDay.LatestAllowed(clock.GetUtcNow()))
            return ApplicationResult<TransferPreview>.Failure(TransferErrors.DateInFuture);
        var source = await accounts.FindOwnedByIdAsync(command.SourceAccountId, owner, ct);
        var destination = await accounts.FindOwnedByIdAsync(command.DestinationAccountId, owner, ct);
        if (source is null || destination is null || !source.IsActive || !destination.IsActive)
            return ApplicationResult<TransferPreview>.Failure(TransferErrors.AccountUnavailable);
        try
        {
            _ = new Transfer(Guid.NewGuid(), owner, source, destination, new Money(command.Amount, command.Currency), command.TransferDate, command.Description);
            if (command.FeeAmount < 0m || decimal.Round(command.FeeAmount, 4) != command.FeeAmount)
                return ApplicationResult<TransferPreview>.Failure(TransferErrors.Validation("Fee must be nonnegative with at most four decimals."));
            var total = new Money(command.Amount + command.FeeAmount, command.Currency).Amount;
            var category = command.FeeAmount > 0m ? (await categories.ListAsync(owner, CategoryType.Expense, null, ct))
                .SingleOrDefault(c => c.Role == CategoryRole.BankFee) : null;
            if (category is { IsActive: false })
                return ApplicationResult<TransferPreview>.Failure(TransferErrors.Validation("The fee category is inactive."));
            var resolution = command.FeeAmount > 0m
                ? await TransactionScopeResolution.ResolveAsync(command.FeeScope, null, source.DefaultScope, profiles, owner, ct)
                : ScopeResolution.Resolved(TransactionScope.Personal);
            return ApplicationResult<TransferPreview>.Success(new(source.Id, source.Name, total, command.FeeAmount,
                category?.Id, command.FeeAmount > 0m ? category?.Name ?? CategoryRoles.Default(CategoryRole.BankFee).Name : null,
                command.FeeAmount > 0m ? resolution.Scope : null, command.FeeAmount > 0m && resolution.Scope is null));
        }
        catch (ArgumentException e) { return ApplicationResult<TransferPreview>.Failure(TransferErrors.Validation(e.Message)); }
        catch (InvalidOperationException e) { return ApplicationResult<TransferPreview>.Failure(TransferErrors.Validation(e.Message)); }
    }
}
