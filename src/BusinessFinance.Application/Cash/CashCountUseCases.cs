using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Cash;

/// <summary>
/// Kasa ekranının açılışı: beklenen bakiye ve varsa o günün sayımı.
/// </summary>
public sealed class GetCashCountTodayUseCase(
    ICurrentUser currentUser,
    ICashCountRepository repository,
    IAccountRepository accountRepository,
    IAccountDayFlowReader dayFlowReader,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CashCountTodayDto>> ExecuteAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CashCountTodayDto>.Failure(
                CashCountErrors.AuthenticationRequired);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            accountId, userId, cancellationToken);
        if (account is null || !account.IsActive || account.Type != AccountType.Cash)
        {
            return ApplicationResult<CashCountTodayDto>.Failure(
                CashCountErrors.AccountUnavailable);
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var expectedBalance = await accountRepository.CalculateBalanceAsync(
            accountId, userId, cancellationToken);
        var count = await repository.FindOpenAsync(
            userId, accountId, today, false, cancellationToken);
        // Beklenen tutarın nereden geldiğini gösteren iki bilgi: son sayım ve
        // bugünkü nakit akışı. İkisi de sunucuda okunur.
        var flow = await dayFlowReader.CalculateDayFlowAsync(
            accountId, userId, today, cancellationToken);
        var previous = (await repository.ListAsync(
                userId,
                new CashCountListCriteria(accountId, today.AddDays(-366), today.AddDays(-1)),
                cancellationToken))
            .FirstOrDefault(item => !item.IsCancelled);

        return ApplicationResult<CashCountTodayDto>.Success(
            new CashCountTodayDto(
                account.Id,
                account.Name,
                expectedBalance,
                account.Currency,
                count is null
                    ? null
                    // Farkı kaydedilmiş sayım bakiyeyi sayılana oturttu; bugünkü
                    // bakiyeye karşı fark artık sıfırdır. Kullanıcının kaydettiği
                    // fark sayım anının gözleminde durur.
                    : CashCountMapper.ToDto(
                        count,
                        account.Name,
                        count.AdjustmentTransactionId is not null && count.ExpectedAtCount is decimal atCount
                            ? atCount
                            : expectedBalance),
                previous,
                flow.Inflow,
                flow.Outflow,
                ChangeSinceCount(count, expectedBalance)));
    }

    private static decimal? ChangeSinceCount(CashCount? count, decimal expectedBalance)
    {
        if (count is null)
        {
            return null;
        }

        if (count.AdjustmentTransactionId is not null)
        {
            return expectedBalance - count.CountedAmount;
        }

        return count.ExpectedAtCount is decimal atCount ? expectedBalance - atCount : null;
    }
}

public sealed class ListCashCountsUseCase(
    ICurrentUser currentUser,
    ICashCountRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<CashCountDto>>> ExecuteAsync(
        CashCountListCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<CashCountDto>>.Failure(
                CashCountErrors.AuthenticationRequired);
        }

        if (criteria.To < criteria.From)
        {
            return ApplicationResult<IReadOnlyList<CashCountDto>>.Failure(
                CashCountErrors.Validation("The end date cannot be before the start date."));
        }

        return ApplicationResult<IReadOnlyList<CashCountDto>>.Success(
            await repository.ListAsync(userId, criteria, cancellationToken));
    }
}

/// <summary>
/// Gün sonu sayımını yazar.
/// </summary>
/// <remarks>
/// Sayım <b>hiçbir finansal kayıt üretmez</b>: bir gözlemdir. Fark varsa bile
/// para o yüzden hareket etmez; düzeltme ikinci ve açık bir eylemdir.
/// </remarks>
public sealed class CreateCashCountUseCase(
    ICurrentUser currentUser,
    ICashCountRepository repository,
    IAccountRepository accountRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CashCountDto>> ExecuteAsync(
        CreateCashCountCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CashCountDto>.Failure(
                CashCountErrors.AuthenticationRequired);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive || account.Type != AccountType.Cash)
        {
            return ApplicationResult<CashCountDto>.Failure(CashCountErrors.AccountUnavailable);
        }

        // Kategori yok: sayımın kendisi bir gelir/gider değildir ve zincirin
        // son halkası sorulmaz.
        if (TransactionScopeResolution.Resolve(command.Scope, account.DefaultScope, null)
            is not TransactionScope scope)
        {
            return ApplicationResult<CashCountDto>.Failure(CashCountErrors.ScopeUnresolved);
        }

        try
        {
            // Aynı gün ikinci sayım öncekini kapatır: gün sonunda kasayı iki kez
            // saymak düzeltmedir, iki ayrı gözlem değil. Eski kayıt silinmez,
            // iptal edilmiş hâliyle durur.
            var superseded = await repository.FindOpenAsync(
                userId, command.AccountId, command.CountDate, true, cancellationToken);
            var now = timeProvider.GetUtcNow().ToUniversalTime();
            // Sayım bakiyeyi değiştirmez: beklenen bakiye yazmadan önce de
            // sonra da aynıdır ve sayımın yanına o anın gözlemi olarak düşer.
            var expectedBalance = await accountRepository.CalculateBalanceAsync(
                command.AccountId, userId, cancellationToken);
            var cashCount = new CashCount(
                Guid.NewGuid(),
                userId,
                account,
                command.CountedAmount,
                scope,
                command.CountDate,
                now,
                command.Note,
                expectedBalance);
            superseded?.SupersedeWith(cashCount, now);
            await repository.AddAsync(cashCount, superseded, cancellationToken);
            return ApplicationResult<CashCountDto>.Success(
                CashCountMapper.ToDto(cashCount, account.Name, expectedBalance));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CashCountDto>.Failure(
                CashCountErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<CashCountDto>.Failure(
                CashCountErrors.Conflict(exception.Message));
        }
    }
}

/// <summary>
/// Farkı gerçek bir gelir/gider kaydına çeviren ikinci eylem.
/// </summary>
/// <remarks>
/// Fark kaydı tek ve <b>idempotenttir</b>: bir sayımın iki düzeltmesi olamaz.
/// Tutar onay anındaki bakiyeye göre yeniden hesaplanır, sayımın yanında
/// saklanmaz — arada iptal edilen bir hareket varsa doğru fark yeni olandır.
/// </remarks>
public sealed class ConfirmCashCountDifferenceUseCase(
    ICurrentUser currentUser,
    ICashCountRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CashCountDto>> ExecuteAsync(
        ConfirmCashCountDifferenceCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CashCountDto>.Failure(
                CashCountErrors.AuthenticationRequired);
        }

        var cashCount = await repository.FindOwnedByIdAsync(
            command.CashCountId, userId, true, cancellationToken);
        if (cashCount is null)
        {
            return ApplicationResult<CashCountDto>.Failure(
                CashCountErrors.NotFound(command.CashCountId));
        }

        if (cashCount.IsCancelled)
        {
            return ApplicationResult<CashCountDto>.Failure(
                CashCountErrors.Conflict(
                    "A superseded cash count cannot record an adjustment."));
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            cashCount.AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive)
        {
            return ApplicationResult<CashCountDto>.Failure(CashCountErrors.AccountUnavailable);
        }

        var expectedBalance = await accountRepository.CalculateBalanceAsync(
            cashCount.AccountId, userId, cancellationToken);

        // Düzeltme zaten yazılmışsa ikinci kez yazılmaz; çağrı aynı sonuca
        // döner. Bu, ağ hatasından sonra tekrar denemenin kasayı ikinci kez
        // düzeltmesini engeller.
        if (cashCount.IsAdjusted)
        {
            return ApplicationResult<CashCountDto>.Success(
                CashCountMapper.ToDto(cashCount, account.Name, expectedBalance));
        }

        var difference = cashCount.DifferenceFrom(expectedBalance);
        if (difference.IsBalanced)
        {
            return ApplicationResult<CashCountDto>.Failure(CashCountErrors.NothingToAdjust);
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (category is null || !category.IsActive ||
            category.Type != RequiredCategoryType(difference.RecognizedType))
        {
            return ApplicationResult<CashCountDto>.Failure(CashCountErrors.CategoryUnavailable);
        }

        try
        {
            var now = timeProvider.GetUtcNow().ToUniversalTime();
            var adjustment = new BudgetTransaction(
                Guid.NewGuid(),
                userId,
                account,
                category,
                difference.ToAdjustmentAmount(),
                difference.RecognizedType,
                // Kapsam sayımdan gelir, kategoriden yeniden türetilmez: aynı
                // sayım iki farklı günde iki farklı kapsam üretemez.
                cashCount.Scope,
                cashCount.CountDate,
                cashCount.Note);
            cashCount.RecordAdjustment(adjustment.Id, now);
            await repository.SaveAdjustmentAsync(adjustment, cancellationToken);

            // Düzeltmeden sonra beklenen bakiye sayılan tutara eşitlenir; fark
            // sıfırdır ve ekran bunu gösterir.
            var balanceAfterAdjustment = await accountRepository.CalculateBalanceAsync(
                cashCount.AccountId, userId, cancellationToken);
            return ApplicationResult<CashCountDto>.Success(
                CashCountMapper.ToDto(cashCount, account.Name, balanceAfterAdjustment));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CashCountDto>.Failure(
                CashCountErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<CashCountDto>.Failure(
                CashCountErrors.Conflict(exception.Message));
        }
    }

    private static CategoryType RequiredCategoryType(TransactionType type) =>
        type == TransactionType.Income ? CategoryType.Income : CategoryType.Expense;
}

internal static class CashCountMapper
{
    public static CashCountDto ToDto(
        CashCount cashCount,
        string accountName,
        decimal? expectedBalance)
    {
        return new CashCountDto(
            cashCount.Id,
            cashCount.AccountId,
            accountName,
            cashCount.CountDate,
            cashCount.CountedAmount,
            cashCount.Currency,
            cashCount.Scope,
            cashCount.Note,
            cashCount.IsCancelled,
            cashCount.AdjustmentTransactionId,
            expectedBalance,
            expectedBalance is decimal balance
                ? cashCount.DifferenceFrom(balance).Amount
                : null);
    }
}
