using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Pos;

public sealed class ListPosDefinitionsUseCase(
    ICurrentUser currentUser,
    IPosDefinitionRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<PosDefinitionDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<PosDefinitionDto>>.Failure(
                PosDefinitionErrors.AuthenticationRequired);
        }

        return ApplicationResult<IReadOnlyList<PosDefinitionDto>>.Success(
            await repository.ListAsync(userId, cancellationToken));
    }
}

/// <summary>
/// POS tanımını oluşturur. Tanım hiçbir para hareketi yazmaz; yalnız tahsilat
/// formunu doldurur (ADR 0019 T4).
/// </summary>
public sealed class CreatePosDefinitionUseCase(
    ICurrentUser currentUser,
    IPosDefinitionRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<PosDefinitionDto>> ExecuteAsync(
        SavePosDefinitionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.AuthenticationRequired);
        }

        var parts = await PosDefinitionParts.LoadAsync(
            command, userId, accountRepository, categoryRepository, cancellationToken);
        if (!parts.IsSuccess)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(parts.Error);
        }

        try
        {
            var definition = new PosDefinition(
                Guid.NewGuid(),
                userId,
                command.Name,
                parts.Value.Account,
                parts.Value.SalesCategory,
                command.CommissionRate,
                parts.Value.CommissionCategory,
                command.TransferDays,
                command.BusinessDaysOnly,
                timeProvider.GetUtcNow().ToUniversalTime());
            // İlk POS kendiliğinden ana POS olur; kullanıcı sonra değiştirir.
            if (!await repository.HasDefaultAsync(userId, cancellationToken))
            {
                definition.SetDefault(true);
            }

            await repository.AddAsync(definition, cancellationToken);
            return ApplicationResult<PosDefinitionDto>.Success(
                PosDefinitionParts.ToDto(definition, parts.Value));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.Conflict(exception.Message));
        }
    }
}

/// <summary>
/// Tanımı düzenler. Geçmiş tahsilatlar değişmez: yazıldıkları hesabı, kategoriyi
/// ve tutarı kendileri taşır.
/// </summary>
public sealed class UpdatePosDefinitionUseCase(
    ICurrentUser currentUser,
    IPosDefinitionRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<ApplicationResult<PosDefinitionDto>> ExecuteAsync(
        Guid definitionId,
        SavePosDefinitionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.AuthenticationRequired);
        }

        var definition = await repository.FindOwnedByIdAsync(
            definitionId, userId, true, cancellationToken);
        if (definition is null)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.NotFound(definitionId));
        }

        var parts = await PosDefinitionParts.LoadAsync(
            command, userId, accountRepository, categoryRepository, cancellationToken);
        if (!parts.IsSuccess)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(parts.Error);
        }

        try
        {
            definition.Update(
                command.Name,
                parts.Value.Account,
                parts.Value.SalesCategory,
                command.CommissionRate,
                parts.Value.CommissionCategory,
                command.TransferDays,
                command.BusinessDaysOnly);
            await repository.SaveAsync(cancellationToken);
            return ApplicationResult<PosDefinitionDto>.Success(
                PosDefinitionParts.ToDto(definition, parts.Value));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.Conflict(exception.Message));
        }
    }
}

/// <summary>
/// Silme yerine pasifleştirme. Pasif tanım yeni tahsilatta seçilemez; geçmiş
/// tahsilatların bağı kalır.
/// </summary>
public sealed class SetPosDefinitionActiveUseCase(
    ICurrentUser currentUser,
    IPosDefinitionRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<ApplicationResult<PosDefinitionDto>> ExecuteAsync(
        Guid definitionId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.AuthenticationRequired);
        }

        var definition = await repository.FindOwnedByIdAsync(
            definitionId, userId, true, cancellationToken);
        if (definition is null)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.NotFound(definitionId));
        }

        definition.SetActive(isActive);
        await repository.SaveAsync(cancellationToken);
        return ApplicationResult<PosDefinitionDto>.Success(
            await PosDefinitionParts.ToDtoAsync(
                definition, userId, accountRepository, categoryRepository, cancellationToken));
    }
}

/// <summary>
/// Ana POS'u seçer: tahsilat formunda seçili gelen budur. Önceki ana POS'un
/// işareti aynı kayıtta kalkar; kullanıcı başına en çok bir tane vardır.
/// </summary>
public sealed class SetDefaultPosDefinitionUseCase(
    ICurrentUser currentUser,
    IPosDefinitionRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<ApplicationResult<PosDefinitionDto>> ExecuteAsync(
        Guid definitionId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.AuthenticationRequired);
        }

        var definition = await repository.FindOwnedByIdAsync(
            definitionId, userId, true, cancellationToken);
        if (definition is null)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(
                PosDefinitionErrors.NotFound(definitionId));
        }

        if (!definition.IsActive)
        {
            return ApplicationResult<PosDefinitionDto>.Failure(PosDefinitionErrors.Inactive);
        }

        foreach (var other in await repository.FindOtherDefaultsAsync(
                     userId, definitionId, cancellationToken))
        {
            other.SetDefault(false);
        }

        definition.SetDefault(true);
        await repository.SaveAsync(cancellationToken);
        return ApplicationResult<PosDefinitionDto>.Success(
            await PosDefinitionParts.ToDtoAsync(
                definition, userId, accountRepository, categoryRepository, cancellationToken));
    }
}

/// <summary>
/// Hiç tahsilatı olmayan tanım silinebilir; varsa pasife alınır (boş hesap
/// silme kuralının aynısı).
/// </summary>
public sealed class DeletePosDefinitionUseCase(
    ICurrentUser currentUser,
    IPosDefinitionRepository repository)
{
    public async Task<ApplicationResult<bool>> ExecuteAsync(
        Guid definitionId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<bool>.Failure(PosDefinitionErrors.AuthenticationRequired);
        }

        var definition = await repository.FindOwnedByIdAsync(
            definitionId, userId, true, cancellationToken);
        if (definition is null)
        {
            return ApplicationResult<bool>.Failure(PosDefinitionErrors.NotFound(definitionId));
        }

        if (await repository.HasSettlementsAsync(definitionId, userId, cancellationToken))
        {
            return ApplicationResult<bool>.Failure(PosDefinitionErrors.HasSettlements);
        }

        await repository.RemoveAsync(definition, cancellationToken);
        return ApplicationResult<bool>.Success(true);
    }
}

/// <summary>
/// Tanımla yazılacak tahsilatın komisyonunu, netini ve beklenen gününü döner.
/// </summary>
/// <remarks>
/// Hiçbir şey yazmaz. Form canlı net gösterir ama parayı istemci hesaplamaz:
/// yuvarlama tek yerde, tahsilatı yazan kuralla aynı yerde yapılır.
/// </remarks>
public sealed class PreviewPosSettlementUseCase(
    ICurrentUser currentUser,
    IPosDefinitionRepository repository)
{
    public async Task<ApplicationResult<PosSettlementPreviewDto>> ExecuteAsync(
        Guid definitionId,
        decimal grossAmount,
        DateOnly settlementDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosSettlementPreviewDto>.Failure(
                PosDefinitionErrors.AuthenticationRequired);
        }

        var definition = await repository.FindOwnedByIdAsync(
            definitionId, userId, false, cancellationToken);
        if (definition is null)
        {
            return ApplicationResult<PosSettlementPreviewDto>.Failure(
                PosDefinitionErrors.NotFound(definitionId));
        }

        try
        {
            var gross = new Money(grossAmount, CurrencyCode.TRY);
            var commission = PosSettlement.CommissionFromRate(gross, definition.CommissionRate);
            return ApplicationResult<PosSettlementPreviewDto>.Success(
                new PosSettlementPreviewDto(
                    gross.Amount,
                    commission,
                    gross.Amount - commission,
                    gross.Currency,
                    settlementDate,
                    definition.ExpectedTransferDate(settlementDate)));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<PosSettlementPreviewDto>.Failure(
                PosDefinitionErrors.Validation(exception.Message));
        }
    }
}

/// <summary>Tanımın bağlandığı hesap ve kategoriler, sahiplik kapsamında okunmuş.</summary>
internal sealed record PosDefinitionParts(
    Account Account,
    Category SalesCategory,
    Category? CommissionCategory)
{
    public static async Task<ApplicationResult<PosDefinitionParts>> LoadAsync(
        SavePosDefinitionCommand command,
        Guid userId,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        CancellationToken cancellationToken)
    {
        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive || account.Type != AccountType.Bank)
        {
            return ApplicationResult<PosDefinitionParts>.Failure(
                PosDefinitionErrors.AccountUnavailable);
        }

        var salesCategory = await categoryRepository.FindOwnedByIdAsync(
            command.SalesCategoryId, userId, cancellationToken);
        if (salesCategory is null || !salesCategory.IsActive ||
            salesCategory.Type != CategoryType.Income)
        {
            return ApplicationResult<PosDefinitionParts>.Failure(
                PosDefinitionErrors.SalesCategoryUnavailable);
        }

        Category? commissionCategory = null;
        if (command.CommissionCategoryId is Guid commissionCategoryId)
        {
            commissionCategory = await categoryRepository.FindOwnedByIdAsync(
                commissionCategoryId, userId, cancellationToken);
            if (commissionCategory is null || !commissionCategory.IsActive ||
                commissionCategory.Type != CategoryType.Expense)
            {
                return ApplicationResult<PosDefinitionParts>.Failure(
                    PosDefinitionErrors.CommissionCategoryUnavailable);
            }
        }

        return ApplicationResult<PosDefinitionParts>.Success(
            new PosDefinitionParts(account, salesCategory, commissionCategory));
    }

    public static PosDefinitionDto ToDto(PosDefinition definition, PosDefinitionParts parts) =>
        ToDto(
            definition,
            parts.Account.Name,
            parts.SalesCategory.Name,
            parts.CommissionCategory?.Name);

    public static async Task<PosDefinitionDto> ToDtoAsync(
        PosDefinition definition,
        Guid userId,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        CancellationToken cancellationToken)
    {
        var account = await accountRepository.FindOwnedByIdAsync(
            definition.AccountId, userId, cancellationToken);
        var salesCategory = await categoryRepository.FindOwnedByIdAsync(
            definition.SalesCategoryId, userId, cancellationToken);
        var commissionCategory = definition.CommissionCategoryId is Guid commissionCategoryId
            ? await categoryRepository.FindOwnedByIdAsync(
                commissionCategoryId, userId, cancellationToken)
            : null;
        return ToDto(
            definition,
            account?.Name ?? string.Empty,
            salesCategory?.Name ?? string.Empty,
            commissionCategory?.Name);
    }

    public static PosDefinitionDto ToDto(
        PosDefinition definition,
        string accountName,
        string salesCategoryName,
        string? commissionCategoryName) => new(
        definition.Id,
        definition.Name,
        definition.AccountId,
        accountName,
        definition.SalesCategoryId,
        salesCategoryName,
        definition.CommissionCategoryId,
        commissionCategoryName,
        definition.CommissionRate,
        definition.TransferDays,
        definition.BusinessDaysOnly,
        definition.IsActive,
        definition.IsDefault);
}
