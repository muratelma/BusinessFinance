using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.DayCloses;
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
    IPosDefinitionRepository definitionRepository,
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

        // Tanım formu doldurur; açıkça gönderilen alan tanımı ezer (ADR 0019 T4).
        PosDefinition? definition = null;
        if (command.PosDefinitionId is Guid definitionId)
        {
            definition = await definitionRepository.FindOwnedByIdAsync(
                definitionId, userId, false, cancellationToken);
            if (definition is null)
            {
                return ApplicationResult<PosSettlementDto>.Failure(
                    PosSettlementErrors.DefinitionUnavailable);
            }

            if (!definition.IsActive)
            {
                return ApplicationResult<PosSettlementDto>.Failure(PosDefinitionErrors.Inactive);
            }
        }

        if ((command.AccountId ?? definition?.AccountId) is not Guid accountId ||
            (command.CategoryId ?? definition?.SalesCategoryId) is not Guid categoryId ||
            (command.ExpectedTransferDate ?? definition?.ExpectedTransferDate(command.SettlementDate))
                is not DateOnly expectedTransferDate)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.DetailsRequired);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            accountId, userId, cancellationToken);
        if (account is null || !account.IsActive || account.Type != AccountType.Bank ||
            account.Currency != command.Currency)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.AccountUnavailable);
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            categoryId, userId, cancellationToken);
        if (category is null || !category.IsActive || category.Type != CategoryType.Income)
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.CategoryUnavailable);
        }

        // Komisyon açıkça gelmediyse tanımın oranı uygulanır; kategori de
        // tanımdan gelir.
        var commissionRate = command.CommissionRate;
        var commissionCategoryFromDefinition = false;
        var requestedCommissionCategoryId = command.CommissionCategoryId;
        if (definition is not null)
        {
            if (command.CommissionAmount is null && commissionRate is null)
            {
                commissionRate = definition.CommissionRate;
            }

            if (requestedCommissionCategoryId is null)
            {
                requestedCommissionCategoryId = definition.CommissionCategoryId;
                commissionCategoryFromDefinition = true;
            }
        }

        Category? commissionCategory = null;
        if (requestedCommissionCategoryId is Guid commissionCategoryId)
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

        // POS satışı işletme satışıdır (ADR 0020 T2): hesabın etiketi
        // sayılmaz; şahsi kategori ya da şahsi istek reddedilir. Komisyon
        // satışın parçasıdır ve aynı kurala uyar.
        var resolution = TransactionScopeResolution.ResolveInContext(
            TransactionScope.Business, command.Scope, category.DefaultScope);
        if (resolution.Scope is TransactionScope && commissionCategory is not null)
        {
            resolution = TransactionScopeResolution.ResolveInContext(
                TransactionScope.Business, command.Scope, commissionCategory.DefaultScope);
        }

        if (resolution.Scope is not TransactionScope scope)
        {
            return ApplicationResult<PosSettlementDto>.Failure(resolution.ToError(
                PosSettlementErrors.ScopeUnresolved, PosSettlementErrors.ScopeConflict));
        }

        try
        {
            var grossAmount = new Money(command.GrossAmount, command.Currency);
            var commissionAmount = commissionRate is decimal rate
                ? PosSettlement.CommissionFromRate(grossAmount, rate)
                : command.CommissionAmount ?? 0m;
            // Tanımın kategorisi yalnız komisyon varsa taşınır; komisyonsuz
            // tahsilat kategori taşımaz.
            if (commissionAmount == 0m && commissionCategoryFromDefinition)
            {
                commissionCategory = null;
            }

            var settlement = new PosSettlement(
                Guid.NewGuid(),
                userId,
                account,
                category,
                grossAmount,
                commissionAmount,
                scope,
                command.SettlementDate,
                expectedTransferDate,
                timeProvider.GetUtcNow().ToUniversalTime(),
                commissionCategory,
                command.Description,
                definition);
            await repository.AddAsync(settlement, cancellationToken);

            return ApplicationResult<PosSettlementDto>.Success(
                PosSettlementMapper.ToDto(
                    settlement,
                    account.Name,
                    category.Name,
                    commissionCategory?.Name,
                    DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime),
                    definition?.Name));
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
/// Silme yerine iptal: yanlış girilen POS tahsilatı hem tanıdığı satışı ve
/// komisyonu hem varsa hesaba taşıdığı parayı birlikte kaybeder.
/// </summary>
/// <remarks>
/// İdempotenttir; iptal edilmiş kaydı yeniden iptal etmek aynı cevabı döner.
/// 28 Eylül denetimi U12: domain'de iptal vardı, ona ulaşan bir yol yoktu.
///
/// Yatışa bağlı tahsilat iptal edilemez (ADR 0019 T5): yatış o tahsilatın
/// netini hesaba taşımıştır. Kullanıcı önce yatışı geri alır; cevap bunu
/// kendi koduyla söyler ki istemci doğru yönlendirmeyi gösterebilsin.
/// </remarks>
public sealed class CancelPosSettlementUseCase(
    ICurrentUser currentUser,
    IDayCloseCountReader dayCloseCountReader,
    IPosSettlementRepository repository,
    IPosDefinitionRepository definitionRepository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<PosSettlementDto>> ExecuteAsync(
        CancelPosSettlementCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        // Bir gün sonunda sayılan tahsilat tek başına iptal edilemez. Kayıt
        // bu kullanıcıya ait değilse sonuç zaten boştur ve aşağıda 404 döner.
        if (currentUser.UserId is Guid userId &&
            await dayCloseCountReader.IsCountedAsync(
                userId, DayCloseRecordKind.PosSettlement, command.SettlementId,
                cancellationToken))
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.DayCloseCounted);
        }

        return await PosSettlementMutation.ApplyAsync(
            currentUser, repository, definitionRepository, accountRepository,
            categoryRepository, timeProvider,
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
        IPosDefinitionRepository definitionRepository,
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

        if (settlement is { IsCancelled: false, PosDepositId: not null })
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.DepositLocked);
        }

        if (settlement is { IsCancelled: false, DayCloseId: not null })
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.DayCloseLocked);
        }

        if (settlement is { IsCancelled: false, Kind: PosSettlementKind.Collection })
        {
            return ApplicationResult<PosSettlementDto>.Failure(
                PosSettlementErrors.CollectionLocked);
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
                definitionRepository, timeProvider, cancellationToken));
    }
}

public static class PosSettlementMapper
{
    /// <summary>Adları sahiplik kapsamında okuyup cevabı kurar.</summary>
    internal static async Task<PosSettlementDto> ToDtoAsync(
        PosSettlement settlement,
        Guid userId,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        IPosDefinitionRepository definitionRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var definition = settlement.PosDefinitionId is Guid definitionId
            ? await definitionRepository.FindOwnedByIdAsync(
                definitionId, userId, false, cancellationToken)
            : null;
        var account = await accountRepository.FindOwnedByIdAsync(
            settlement.AccountId, userId, cancellationToken);
        var category = settlement.CategoryId is Guid categoryId
            ? await categoryRepository.FindOwnedByIdAsync(categoryId, userId, cancellationToken)
            : null;
        var commissionCategory = settlement.CommissionCategoryId is Guid commissionCategoryId
            ? await categoryRepository.FindOwnedByIdAsync(
                commissionCategoryId, userId, cancellationToken)
            : null;

        return ToDto(
            settlement,
            account?.Name ?? string.Empty,
            category?.Name,
            commissionCategory?.Name,
            DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime),
            definition?.Name);
    }

    public static PosSettlementDto ToDto(
        PosSettlement settlement,
        string accountName,
        string? categoryName,
        string? commissionCategoryName,
        DateOnly asOfDate,
        string? definitionName = null,
        string? counterpartyName = null)
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
            settlement.PosDefinitionId,
            definitionName,
            settlement.PosDepositId,
            settlement.DayCloseId,
            Kind: settlement.Kind,
            CounterpartyName: counterpartyName);
    }
}
