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

        return ApplicationResult<PosSettlementDto>.Success(
            await PosSettlementMapper.ToDtoAsync(
                settlement, userId, accountRepository, categoryRepository,
                timeProvider, cancellationToken));
    }
}

/// <summary>
/// "Hesaba geçti" yanlışlıkla işaretlendiyse tahsilatı yeniden yola döndürür.
/// </summary>
/// <remarks>
/// Yalnız hesaba yazılmış net tutar geri çekilir; satış ve komisyon tahsilat
/// gününde tanındı ve olduğu gibi kalır (ADR 0014). 28 Eylül denetimi U12.
/// Aşama 06.3 Grup 5'te yatış kaydı gelince bu davranış yatışın geri
/// alınmasına taşınır.
/// </remarks>
public sealed class RevertPosSettlementTransferUseCase(
    ICurrentUser currentUser,
    IPosSettlementRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<PosSettlementDto>> ExecuteAsync(
        RevertPosSettlementTransferCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await PosSettlementMutation.ApplyAsync(
            currentUser, repository, accountRepository, categoryRepository, timeProvider,
            command.SettlementId,
            settlement => settlement.RevertTransfer(),
            cancellationToken);
    }
}

/// <summary>
/// Silme yerine iptal: yanlış girilen POS tahsilatı hem tanıdığı satışı ve
/// komisyonu hem varsa hesaba taşıdığı parayı birlikte kaybeder.
/// </summary>
/// <remarks>
/// İdempotenttir; iptal edilmiş kaydı yeniden iptal etmek aynı cevabı döner.
/// 28 Eylül denetimi U12: domain'de iptal vardı, ona ulaşan bir yol yoktu.
/// </remarks>
public sealed class CancelPosSettlementUseCase(
    ICurrentUser currentUser,
    IPosSettlementRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<PosSettlementDto>> ExecuteAsync(
        CancelPosSettlementCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await PosSettlementMutation.ApplyAsync(
            currentUser, repository, accountRepository, categoryRepository, timeProvider,
            command.SettlementId,
            settlement => settlement.Cancel(timeProvider.GetUtcNow().ToUniversalTime()),
            cancellationToken);
    }
}

/// <summary>
/// Sahip olunan bir tahsilatı bulur, üzerinde tek bir domain adımı uygular ve
/// kaydeder. Başka kullanıcının ya da var olmayan kaydın cevabı aynıdır.
/// </summary>
internal static class PosSettlementMutation
{
    public static async Task<ApplicationResult<PosSettlementDto>> ApplyAsync(
        ICurrentUser currentUser,
        IPosSettlementRepository repository,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        TimeProvider timeProvider,
        Guid settlementId,
        Action<PosSettlement> change,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.AuthenticationRequired);
        }

        var settlement = await repository.FindOwnedByIdAsync(
            settlementId, userId, true, cancellationToken);
        if (settlement is null)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.NotFound(settlementId));
        }

        try
        {
            change(settlement);
            await repository.SaveAsync(cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.Conflict(exception.Message));
        }

        return ApplicationResult<PosSettlementDto>.Success(
            await PosSettlementMapper.ToDtoAsync(
                settlement, userId, accountRepository, categoryRepository,
                timeProvider, cancellationToken));
    }
}

internal static class PosSettlementMapper
{
    /// <summary>Adları sahiplik kapsamında okuyup cevabı kurar.</summary>
    public static async Task<PosSettlementDto> ToDtoAsync(
        PosSettlement settlement,
        Guid userId,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var account = await accountRepository.FindOwnedByIdAsync(
            settlement.AccountId, userId, cancellationToken);
        var category = await categoryRepository.FindOwnedByIdAsync(
            settlement.CategoryId, userId, cancellationToken);
        var commissionCategory = settlement.CommissionCategoryId is Guid commissionCategoryId
            ? await categoryRepository.FindOwnedByIdAsync(
                commissionCategoryId, userId, cancellationToken)
            : null;

        return ToDto(
            settlement,
            account?.Name ?? string.Empty,
            category?.Name ?? string.Empty,
            commissionCategory?.Name,
            DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));
    }

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
