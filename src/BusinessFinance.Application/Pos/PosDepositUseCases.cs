using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Pos;

/// <summary>
/// Yatışın seçimi: sahip olunan, hâlâ yolda ve aynı hesaba geçen tahsilatlar.
/// Önizleme ve kayıt aynı kapıdan geçer; ikisi farklı cevap verseydi form
/// kaydedilemeyecek bir seçimi geçerli gösterirdi.
/// </summary>
internal sealed record PosDepositSelection(
    IReadOnlyList<PosSettlement> Settlements,
    Account Account)
{
    public const int MaximumSettlements = 100;

    public static async Task<(PosDepositSelection? Selection, ApplicationError? Error)> LoadAsync(
        Guid userId,
        IReadOnlyList<Guid>? settlementIds,
        IPosDepositRepository repository,
        IAccountRepository accountRepository,
        CancellationToken cancellationToken)
    {
        if (settlementIds is null || settlementIds.Count == 0 ||
            settlementIds.Distinct().Count() != settlementIds.Count)
        {
            return (null, PosDepositErrors.SettlementsRequired);
        }

        if (settlementIds.Count > MaximumSettlements)
        {
            return (null, PosDepositErrors.TooManySettlements(MaximumSettlements));
        }

        var settlements = await repository.FindOwnedSettlementsAsync(
            settlementIds.ToArray(), userId, cancellationToken);
        if (settlements.Count != settlementIds.Count)
        {
            return (null, PosDepositErrors.SettlementNotFound);
        }

        if (settlements.Any(settlement => !settlement.IsInTransit))
        {
            return (null, PosDepositErrors.SettlementNotInTransit);
        }

        if (settlements.Select(settlement => settlement.AccountId).Distinct().Count() != 1)
        {
            return (null, PosDepositErrors.MixedAccounts);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            settlements[0].AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive)
        {
            return (null, PosDepositErrors.AccountUnavailable);
        }

        return (new PosDepositSelection(settlements, account), null);
    }

    public DateOnly EarliestDepositDate =>
        Settlements.Max(settlement => settlement.SettlementDate);

    /// <summary>
    /// Kesinti kategorisi: açık seçim, yoksa seçilen tahsilatların ortak
    /// adayı. Adaylar birden çoksa sonuç boştur; sunucu aralarından seçmez.
    /// </summary>
    public async Task<(Category? Category, ApplicationError? Error)> ResolveDeductionCategoryAsync(
        Guid userId,
        Guid? requestedCategoryId,
        IPosDepositRepository repository,
        ICategoryRepository categoryRepository,
        CancellationToken cancellationToken)
    {
        if (requestedCategoryId is Guid requested)
        {
            var category = await categoryRepository.FindOwnedByIdAsync(
                requested, userId, cancellationToken);
            return IsUsable(category)
                ? (category, null)
                : (null, PosDepositErrors.DeductionCategoryUnavailable);
        }

        var candidates = await repository.ListDeductionCategoryCandidatesAsync(
            Settlements.Select(settlement => settlement.Id).ToArray(), userId, cancellationToken);
        if (candidates.Count != 1)
        {
            return (null, null);
        }

        var candidate = await categoryRepository.FindOwnedByIdAsync(
            candidates[0], userId, cancellationToken);
        return IsUsable(candidate) ? (candidate, null) : (null, null);
    }

    private static bool IsUsable(Category? category) =>
        category is { IsActive: true, Type: CategoryType.Expense };
}

/// <summary>
/// Yatış formunun önizlemesi: beklenen toplam, kesinti ve dolu gelecek kesinti
/// kategorisi. Hiçbir şey yazmaz.
/// </summary>
public sealed class PreviewPosDepositUseCase(
    ICurrentUser currentUser,
    IPosDepositRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<ApplicationResult<PosDepositPreviewDto>> ExecuteAsync(
        PreviewPosDepositQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosDepositPreviewDto>.Failure(
                PosDepositErrors.AuthenticationRequired);
        }

        var (selection, error) = await PosDepositSelection.LoadAsync(
            userId, query.SettlementIds, repository, accountRepository, cancellationToken);
        if (selection is null)
        {
            return ApplicationResult<PosDepositPreviewDto>.Failure(error!);
        }

        var expected = PosDeposit.ExpectedFor(selection.Settlements);
        var deposited = query.DepositedAmount ?? expected;
        if (deposited <= 0m)
        {
            return ApplicationResult<PosDepositPreviewDto>.Failure(PosDepositErrors.InvalidAmount);
        }

        var exceeds = deposited > expected;
        var (category, _) = await selection.ResolveDeductionCategoryAsync(
            userId, null, repository, categoryRepository, cancellationToken);

        return ApplicationResult<PosDepositPreviewDto>.Success(new PosDepositPreviewDto(
            selection.Account.Id,
            selection.Account.Name,
            selection.Settlements.Count,
            expected,
            deposited,
            exceeds ? 0m : expected - deposited,
            exceeds,
            category?.Id,
            category?.Name,
            selection.Account.Currency,
            selection.EarliestDepositDate));
    }
}

/// <summary>
/// Yatışı yazar: seçilen tahsilatlar hesaba geçer, eksik yatan kısım kesinti
/// gideri olur (ADR 0019 T5).
/// </summary>
/// <remarks>
/// <para>
/// Yatış <b>gelir yazmaz</b>: satış tahsilat gününde tanındı (ADR 0014).
/// Kesinti yeni bir kayıt türü değildir; sıradan bir giderdir ve raporlara,
/// bütçeye ve İşlemler'e başka her gider gibi girer.
/// </para>
/// <para>
/// Yatış, kesinti gideri ve tahsilatların bağı tek <c>SaveChanges</c> ile
/// yazılır. İstek kimliği yatışın kimliğini belirler
/// (<see cref="RequestScopedId"/>): tekrar gönderilen istek ilk yatışı döner.
/// </para>
/// </remarks>
public sealed class CreatePosDepositUseCase(
    ICurrentUser currentUser,
    IPosDepositRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    internal const string IdPurpose = "pos-deposit";

    public async Task<ApplicationResult<PosDepositDto>> ExecuteAsync(
        CreatePosDepositCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.AuthenticationRequired);
        }

        if (command.ClientRequestId == Guid.Empty)
        {
            return ApplicationResult<PosDepositDto>.Failure(
                PosDepositErrors.Validation("Client request id is required."));
        }

        var depositId = RequestScopedId.Create(IdPurpose, userId, command.ClientRequestId);
        if (await repository.GetAsync(depositId, userId, cancellationToken) is PosDepositDto existing)
        {
            return ApplicationResult<PosDepositDto>.Success(existing);
        }

        var (selection, selectionError) = await PosDepositSelection.LoadAsync(
            userId, command.SettlementIds, repository, accountRepository, cancellationToken);
        if (selection is null)
        {
            return ApplicationResult<PosDepositDto>.Failure(selectionError!);
        }

        if (command.DepositedAmount <= 0m)
        {
            return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.InvalidAmount);
        }

        var now = timeProvider.GetUtcNow().ToUniversalTime();
        if (command.DepositDate == default ||
            command.DepositDate > LocalDay.LatestAllowed(now) ||
            command.DepositDate < selection.EarliestDepositDate)
        {
            return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.InvalidDepositDate);
        }

        var expected = PosDeposit.ExpectedFor(selection.Settlements);
        if (command.DepositedAmount > expected)
        {
            return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.AmountExceedsExpected);
        }

        try
        {
            var depositedAmount = new Money(command.DepositedAmount, selection.Account.Currency);
            var deduction = PosDeposit.DeductionFor(selection.Settlements, depositedAmount);

            BudgetTransaction? deductionTransaction = null;
            if (deduction > 0m)
            {
                var (category, categoryError) = await selection.ResolveDeductionCategoryAsync(
                    userId, command.DeductionCategoryId, repository, categoryRepository,
                    cancellationToken);
                if (category is null)
                {
                    return ApplicationResult<PosDepositDto>.Failure(
                        categoryError ?? PosDepositErrors.DeductionCategoryRequired);
                }

                // Kesinti, kapattığı satışların komisyonunun devamıdır ve
                // işletmenindir (ADR 0020 T2): kapatılan tahsilatların ve
                // hesabın etiketi sayılmaz; şahsi kategori ya da şahsi istek
                // reddedilir.
                if (TransactionScopeResolution.ResolveInContext(
                        TransactionScope.Business, command.Scope, category.DefaultScope).Scope
                    is not TransactionScope scope)
                {
                    return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.ScopeConflict);
                }

                deductionTransaction = new BudgetTransaction(
                    Guid.NewGuid(),
                    userId,
                    selection.Account,
                    category,
                    new Money(deduction, selection.Account.Currency),
                    TransactionType.Expense,
                    scope,
                    command.DepositDate);
            }

            var deposit = PosDeposit.Record(
                depositId,
                userId,
                selection.Account,
                selection.Settlements,
                depositedAmount,
                command.DepositDate,
                now,
                deductionTransaction);
            if (!await repository.TryAddAsync(deposit, deductionTransaction, cancellationToken))
            {
                return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.ConcurrentChange);
            }
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<PosDepositDto>.Failure(
                PosDepositErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<PosDepositDto>.Failure(
                PosDepositErrors.Conflict(exception.Message));
        }

        var persisted = await repository.GetAsync(depositId, userId, cancellationToken)
            ?? throw new InvalidOperationException("Saved pos deposit could not be read back.");
        return ApplicationResult<PosDepositDto>.Success(persisted);
    }

}

public sealed class GetPosDepositUseCase(
    ICurrentUser currentUser,
    IPosDepositRepository repository,
    IActivityBalanceReader balanceReader)
{
    public async Task<ApplicationResult<PosDepositDto>> ExecuteAsync(
        Guid depositId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.AuthenticationRequired);
        }

        if (await repository.GetAsync(depositId, userId, cancellationToken) is not PosDepositDto deposit)
        {
            return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.NotFound(depositId));
        }

        // Ayrıntı "işlem sonrası bakiye"yi de gösterir; İşlemler'deki diğer
        // hareketlerle aynı okumadan gelir.
        var balances = await balanceReader.GetBalancesAfterAsync(
            userId, FinancialActivityKind.PosDeposit, depositId, cancellationToken);
        return ApplicationResult<PosDepositDto>.Success(
            deposit with { BalanceAfter = balances?.FirstOrDefault()?.Balance });
    }
}

/// <summary>
/// Yatışı geri alır: kapattığı tahsilatlar yeniden yola çıkar, kesinti gideri
/// iptal olur (ADR 0019 T5, İ8).
/// </summary>
/// <remarks>
/// Silme yerine iptal: yatış kaydı kalır. Üç değişiklik tek
/// <c>SaveChanges</c> ile yazılır. İdempotenttir; geri alınmış yatışı yeniden
/// geri almak aynı cevabı döner.
/// </remarks>
public sealed class RevertPosDepositUseCase(
    ICurrentUser currentUser,
    IPosDepositRepository repository,
    ITransactionRepository transactionRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<PosDepositDto>> ExecuteAsync(
        Guid depositId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.AuthenticationRequired);
        }

        var deposit = await repository.FindOwnedByIdAsync(depositId, userId, true, cancellationToken);
        if (deposit is null)
        {
            return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.NotFound(depositId));
        }

        if (!deposit.IsCancelled)
        {
            var settlements = await repository.FindSettlementsOfDepositAsync(
                depositId, userId, cancellationToken);
            var deductionTransaction = deposit.DeductionTransactionId is Guid transactionId
                ? await transactionRepository.FindOwnedByIdAsync(
                    transactionId, userId, track: true, cancellationToken)
                : null;
            try
            {
                deposit.Revert(
                    settlements, deductionTransaction, timeProvider.GetUtcNow().ToUniversalTime());
            }
            catch (InvalidOperationException exception)
            {
                return ApplicationResult<PosDepositDto>.Failure(
                    PosDepositErrors.Conflict(exception.Message));
            }

            if (!await repository.TrySaveAsync(cancellationToken))
            {
                return ApplicationResult<PosDepositDto>.Failure(PosDepositErrors.ConcurrentChange);
            }
        }

        var reverted = await repository.GetAsync(depositId, userId, cancellationToken)
            ?? throw new InvalidOperationException("Reverted pos deposit could not be read back.");
        return ApplicationResult<PosDepositDto>.Success(reverted);
    }
}
