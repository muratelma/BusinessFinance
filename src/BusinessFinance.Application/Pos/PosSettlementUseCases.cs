using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Pos;

public sealed class ListPosSettlementsUseCase(
    ICurrentUser currentUser,
    IPosSettlementRepository repository)
{
    public async Task<ApplicationResult<PosSettlementListDto>> ExecuteAsync(
        PosSettlementListCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosSettlementListDto>.Failure(
                PosSettlementErrors.AuthenticationRequired);
        }

        if (criteria.To < criteria.From)
        {
            return ApplicationResult<PosSettlementListDto>.Failure(
                PosSettlementErrors.Validation(
                    "The end date cannot be before the start date."));
        }

        return ApplicationResult<PosSettlementListDto>.Success(
            await repository.ListAsync(userId, criteria, cancellationToken));
    }
}

/// <summary>
/// Tahsilatı yazar: satış <b>bugün tanınır</b>, para henüz hiçbir hesaba
/// girmez (ADR 0015).
/// </summary>
public sealed class CreatePosSettlementUseCase(
    ICurrentUser currentUser,
    IPosSettlementRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<PosSettlementDto>> ExecuteAsync(
        CreatePosSettlementCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.AuthenticationRequired);
        }

        if (command.CommissionAmount is not null && command.CommissionRate is not null)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.CommissionAmbiguous);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive || account.Type != AccountType.Bank ||
            account.Currency != command.Currency)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.AccountUnavailable);
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (category is null || !category.IsActive || category.Type != CategoryType.Income)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.CategoryUnavailable);
        }

        Category? commissionCategory = null;
        if (command.CommissionCategoryId is Guid commissionCategoryId)
        {
            commissionCategory = await categoryRepository.FindOwnedByIdAsync(
                commissionCategoryId, userId, cancellationToken);
            if (commissionCategory is null || !commissionCategory.IsActive ||
                commissionCategory.Type != CategoryType.Expense)
            {
                return ApplicationResult<PosSettlementDto>.Failure(
                    PosSettlementErrors.CommissionCategoryUnavailable);
            }
        }

        if (TransactionScopeResolution.Resolve(
                command.Scope, account.DefaultScope, category.DefaultScope)
            is not TransactionScope scope)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.ScopeUnresolved);
        }

        try
        {
            var grossAmount = new Money(command.GrossAmount, command.Currency);
            var commissionAmount = command.CommissionRate is decimal rate
                ? PosSettlement.CommissionFromRate(grossAmount, rate)
                : command.CommissionAmount ?? 0m;

            var settlement = new PosSettlement(
                Guid.NewGuid(),
                userId,
                account,
                category,
                grossAmount,
                commissionAmount,
                scope,
                command.SettlementDate,
                command.ExpectedTransferDate,
                timeProvider.GetUtcNow().ToUniversalTime(),
                commissionCategory,
                command.Description,
                command.Vat?.ToDomain());
            await repository.AddAsync(settlement, cancellationToken);

            return ApplicationResult<PosSettlementDto>.Success(
                PosSettlementMapper.ToDto(
                    settlement,
                    account.Name,
                    category.Name,
                    commissionCategory?.Name,
                    DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime)));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.Conflict(exception.Message));
        }
    }
}

/// <summary>
/// Paranın hesaba geçtiğini işaretler.
/// </summary>
/// <remarks>
/// Bu adım <b>hiçbir gelir veya gider yazmaz</b>: satış tahsilat gününde
/// tanındı. Yazsaydı aynı satış iki kez sayılırdı (ADR 0014).
/// </remarks>
public sealed class MarkPosSettlementTransferredUseCase(
    ICurrentUser currentUser,
    IPosSettlementRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<PosSettlementDto>> ExecuteAsync(
        MarkPosSettlementTransferredCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.AuthenticationRequired);
        }

        var settlement = await repository.FindOwnedByIdAsync(
            command.SettlementId, userId, true, cancellationToken);
        if (settlement is null)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.NotFound(command.SettlementId));
        }

        try
        {
            settlement.MarkTransferred(
                command.TransferDate, timeProvider.GetUtcNow().ToUniversalTime());
            await repository.SaveAsync(cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.Conflict(exception.Message));
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            settlement.AccountId, userId, cancellationToken);
        var category = await categoryRepository.FindOwnedByIdAsync(
            settlement.CategoryId, userId, cancellationToken);
        var commissionCategory = settlement.CommissionCategoryId is Guid commissionCategoryId
            ? await categoryRepository.FindOwnedByIdAsync(
                commissionCategoryId, userId, cancellationToken)
            : null;

        return ApplicationResult<PosSettlementDto>.Success(
            PosSettlementMapper.ToDto(
                settlement,
                account?.Name ?? string.Empty,
                category?.Name ?? string.Empty,
                commissionCategory?.Name,
                DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime)));
    }
}

internal static class PosSettlementMapper
{
    public static PosSettlementDto ToDto(
        PosSettlement settlement,
        string accountName,
        string categoryName,
        string? commissionCategoryName,
        DateOnly asOfDate)
    {
        return new PosSettlementDto(
            settlement.Id,
            settlement.AccountId,
            accountName,
            settlement.CategoryId,
            categoryName,
            settlement.CommissionCategoryId,
            commissionCategoryName,
            settlement.GrossAmount.Amount,
            settlement.CommissionAmount,
            settlement.NetAmount.Amount,
            settlement.CommissionRate,
            settlement.Currency,
            settlement.Scope,
            settlement.SettlementDate,
            settlement.ExpectedTransferDate,
            settlement.TransferredOn,
            settlement.Description,
            settlement.IsInTransit,
            settlement.IsCancelled,
            settlement.IsInTransit && settlement.ExpectedTransferDate < asOfDate,
            VatDto.From(settlement.Vat));
    }
}
