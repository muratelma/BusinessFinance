using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Pos;

/// <summary>
/// Bir alacağın kartla (POS) tahsil edildiğini söyleyen istek parçası (ADR
/// 0019 T5).
/// </summary>
/// <remarks>
/// POS satışıyla aynı kural: <see cref="PosDefinitionId"/> verilirse boş
/// bırakılan alanlar POS'tan dolar (hesap, komisyon oranı ve kategorisi,
/// beklenen gün); açıkça gönderilen alan onu ezer. POS verilmezse ("Elle gir")
/// hesap ve beklenen gün zorunludur, komisyon isteğe bağlıdır.
/// </remarks>
public sealed record CardCollectionInput(
    Guid? PosDefinitionId,
    Guid? AccountId,
    decimal? CommissionAmount,
    decimal? CommissionRate,
    Guid? CommissionCategoryId,
    DateOnly? ExpectedTransferDate);

/// <summary>
/// Kartla tahsilin POS kaydını kurar: cari tahsilat ve tek seferlik alacağın
/// kapanışı aynı yoldan geçer.
/// </summary>
/// <remarks>
/// Komisyon tahsil günü POS'un oranıyla yazılır (Aşama 06.3 Grup 5 açılış
/// kararı 1); kapsamı hesabın etiketi → komisyon kategorisinin varsayılanı
/// zinciriyle çözülür ve çözülemezse istek reddedilir — sunucu kapsam
/// uydurmaz. Komisyonsuz tahsilin kapsamı yoktur. Kayıt yazılmaz; çağıran onu
/// doğuran tahsilatla tek <c>SaveChanges</c> sınırında yazar.
/// </remarks>
public sealed class CardCollectionBuilder(
    IPosDefinitionRepository definitionRepository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<(PosSettlement? Settlement, Account? Account, ApplicationError? Error)> BuildAsync(
        Guid userId,
        CardCollectionInput input,
        Money amount,
        DateOnly collectedOn,
        DateTimeOffset createdAtUtc,
        string? description,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(amount);
        if (input.CommissionAmount is not null && input.CommissionRate is not null)
        {
            return (null, null, PosSettlementErrors.CommissionAmbiguous);
        }

        PosDefinition? definition = null;
        if (input.PosDefinitionId is Guid definitionId)
        {
            definition = await definitionRepository.FindOwnedByIdAsync(
                definitionId, userId, false, cancellationToken);
            if (definition is null)
            {
                return (null, null, PosSettlementErrors.DefinitionUnavailable);
            }

            if (!definition.IsActive)
            {
                return (null, null, PosDefinitionErrors.Inactive);
            }
        }

        if ((input.AccountId ?? definition?.AccountId) is not Guid accountId ||
            (input.ExpectedTransferDate ?? definition?.ExpectedTransferDate(collectedOn))
                is not DateOnly expectedTransferDate)
        {
            return (null, null, PosSettlementErrors.DetailsRequired);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            accountId, userId, cancellationToken);
        if (account is null || !account.IsActive || account.Type != AccountType.Bank ||
            account.Currency != amount.Currency)
        {
            return (null, null, PosSettlementErrors.AccountUnavailable);
        }

        var commissionRate = input.CommissionRate;
        var requestedCommissionCategoryId = input.CommissionCategoryId;
        var commissionCategoryFromDefinition = false;
        if (definition is not null)
        {
            if (input.CommissionAmount is null && commissionRate is null)
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
                return (null, null, PosSettlementErrors.CommissionCategoryUnavailable);
            }
        }

        var commissionAmount = commissionRate is decimal rate
            ? PosSettlement.CommissionFromRate(amount, rate)
            : input.CommissionAmount ?? 0m;
        // POS'un kategorisi yalnız komisyon varsa taşınır.
        if (commissionAmount == 0m && commissionCategoryFromDefinition)
        {
            commissionCategory = null;
        }

        // Komisyon POS'un maliyetidir ve işletmenindir (ADR 0020 T2);
        // hesabın etiketi sayılmaz, şahsi kategori reddedilir.
        TransactionScope? scope = null;
        if (commissionAmount > 0m)
        {
            scope = TransactionScopeResolution.ResolveInContext(
                TransactionScope.Business, null, commissionCategory?.DefaultScope).Scope;
            if (scope is null)
            {
                return (null, null, PosSettlementErrors.ScopeConflict);
            }
        }

        var settlement = PosSettlement.Collect(
            Guid.NewGuid(),
            userId,
            account,
            amount,
            commissionAmount,
            scope,
            collectedOn,
            expectedTransferDate,
            createdAtUtc,
            commissionCategory,
            description,
            definition);
        return (settlement, account, null);
    }
}
