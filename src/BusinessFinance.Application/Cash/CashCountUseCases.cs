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
    /// <param name="accountId">Sayılacak kasa.</param>
    /// <param name="day">
    /// Kullanıcının takvim günü. Sunucunun UTC gününden en çok bir gün uzakta
    /// olabilir (<see cref="LocalDay"/>); boşsa ya da daha uzaksa sunucunun
    /// günü kullanılır. Gece yarısından sonra girilen sayım kullanıcının
    /// gününü taşır; ekran aynı günü sormazsa o sayımı "bugün" diye bulamaz.
    /// </param>
    /// <param name="cancellationToken">İptal belirteci.</param>
    public async Task<ApplicationResult<CashCountTodayDto>> ExecuteAsync(
        Guid accountId,
        DateOnly? day = null,
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

        var utcToday = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var today = day is DateOnly requested &&
                    requested >= utcToday.AddDays(-1) && requested <= utcToday.AddDays(1)
            ? requested
            : utcToday;
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
        // Farkı açıklayan kayıt sonradan iptal edildiyse sayım "kaydedildi"
        // sayılmaz; ama yeniden kaydedilebilir hâle de gelmez (KS3).
        var adjustmentCancelled = count is { IsAdjusted: true } &&
            await repository.IsAdjustmentCancelledAsync(count, cancellationToken);
        var recorded = count is { IsAdjusted: true } && !adjustmentCancelled;
        var balancedNow = count is not null &&
            count.DifferenceFrom(expectedBalance).IsBalanced;

        // Önceki sayımın farkı bir geçmiş bilgisidir (KS4): farkı için duran
        // bir kayıt yoksa taşınır. Bugün kasa tutuyorsa taşınmaz; o zaman
        // söylenecek şey yeni sonuçtur.
        var previousUnrecorded = previous is
        { AdjustmentStatus: not CashCountAdjustmentStatus.Recorded, Difference: decimal previousDifference }
            && previousDifference != 0m
            && !balancedNow
            ? previousDifference
            : (decimal?)null;
        // Bugünkü açık farkın önceki sayımdakiyle aynı olup olmadığını sunucu
        // söyler; istemci iki tutarı karşılaştırmaz.
        var sameAsPrevious = previousUnrecorded is decimal carried &&
            count is not null && !recorded &&
            count.DifferenceFrom(expectedBalance).Amount == carried;
        // Açık farkı olan sayım hâlâ güncel mi? Kuralın kendisi sayımdadır;
        // ekran yalnız sonucu okur.
        var requiresRecount = count is not null && !recorded && !balancedNow &&
            (adjustmentCancelled ||
             count.RequiresRecount(
                 expectedBalance,
                 await repository.HasAccountChangedSinceAsync(count, cancellationToken)));

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
                        recorded && count.ExpectedAtCount is decimal atCount
                            ? atCount
                            : expectedBalance,
                        adjustmentCancelled),
                previous,
                flow.Inflow,
                flow.Outflow,
                ChangeSinceCount(count, expectedBalance, recorded),
                previousUnrecorded,
                sameAsPrevious,
                requiresRecount));
    }

    private static decimal? ChangeSinceCount(
        CashCount? count,
        decimal expectedBalance,
        bool recorded)
    {
        if (count is null)
        {
            return null;
        }

        // Farkı kaydedilmiş sayım bakiyeyi sayılana oturttu; kaydı iptal
        // edilmiş sayımda bakiye sayım anındakine dönmüştür.
        if (recorded)
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

        // Kategori yok: sayımın kendisi bir gelir/gider değildir; taraf
        // istekten, yoksa kasanın etiketinden gelir.
        if (TransactionScopeResolution.Resolve(command.Scope, null, account.DefaultScope).Scope
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
/// <para>
/// Fark kaydı tek ve <b>idempotenttir</b>: bir sayımın iki düzeltmesi olamaz.
/// </para>
/// <para>
/// Fark yalnız <b>güncel</b> sayıma yazılır (kullanıcı kararı, 8 Ekim 2026):
/// sayımdan sonra kasaya kayıt girildiyse ya da daha yeni bir sayım varsa
/// istek <c>cash_counts.recount_required</c> ile reddedilir
/// (<see cref="CashCount.RequiresRecount"/>). Eskiden tutar onay anındaki
/// bakiyeye göre yeniden hesaplanıyordu ve sayımdan sonra girilen bir satış
/// farkı büyütüyordu.
/// </para>
/// <para>
/// Yazılan tutar sayım anındaki farktır. Denetimden hemen sonra araya giren
/// bir kayıt bu yüzden sonucu bozmaz: ortaya çıkan durum "önce fark
/// kaydedildi, sonra o kayıt girildi" sırasıyla aynıdır.
/// </para>
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
        // düzeltmesini engeller. Yazılan kayıt sonradan iptal edildiyse sayım
        // yeniden açılmaz (KS3): iptal kasanın kayıtlarını değiştirdi, yeni
        // fark kaydı yeni bir sayım ister.
        if (cashCount.IsAdjusted)
        {
            return await repository.IsAdjustmentCancelledAsync(cashCount, cancellationToken)
                ? ApplicationResult<CashCountDto>.Failure(CashCountErrors.RecountRequired)
                : ApplicationResult<CashCountDto>.Success(
                    CashCountMapper.ToDto(cashCount, account.Name, expectedBalance));
        }

        if (cashCount.RequiresRecount(
                expectedBalance,
                await repository.HasAccountChangedSinceAsync(cashCount, cancellationToken)) ||
            cashCount.ExpectedAtCount is not decimal expectedAtCount)
        {
            return ApplicationResult<CashCountDto>.Failure(CashCountErrors.RecountRequired);
        }

        var difference = cashCount.DifferenceFrom(expectedAtCount);
        if (difference.IsBalanced)
        {
            return ApplicationResult<CashCountDto>.Failure(CashCountErrors.NothingToAdjust);
        }

        // "Kendime aldım": eksik para sahibine gitti. Tutar farkın tamamı,
        // gün sayımın günüdür (KS2); başka bir gün ya da tutar ayrı bir
        // kayıtla yazılır ve kasa yeniden sayılır.
        if (command.TookForMyself)
        {
            if (command.UnknownReason ||
                difference.RecognizedType != TransactionType.Expense ||
                (command.PersonalAccountId is null) == (command.CategoryId is null))
            {
                return ApplicationResult<CashCountDto>.Failure(
                    CashCountErrors.WithdrawalNotApplicable);
            }

            if (command.PersonalAccountId is Guid personalAccountId)
            {
                return await RecordWithdrawalTransferAsync(
                    cashCount, account, personalAccountId, difference, userId, cancellationToken);
            }
        }

        Category? category;
        if (command.UnknownReason)
        {
            // "Bilmiyorum" yalnız eksik fark içindir ve kategoriyle birlikte
            // gelmez: iki cevap birden gelirse hangisinin doğru olduğunu
            // seçmek sunucunun işi değildir.
            if (command.CategoryId is not null ||
                difference.RecognizedType != TransactionType.Expense)
            {
                return ApplicationResult<CashCountDto>.Failure(
                    CashCountErrors.UnknownReasonNotApplicable);
            }

            category = await FindOrOpenDifferenceCategoryAsync(
                userId, cashCount.Scope, cancellationToken);
        }
        else
        {
            category = command.CategoryId is Guid categoryId
                ? await categoryRepository.FindOwnedByIdAsync(
                    categoryId, userId, cancellationToken)
                : null;
        }

        if (category is null || !category.IsActive ||
            category.Type != RequiredCategoryType(difference.RecognizedType))
        {
            return ApplicationResult<CashCountDto>.Failure(CashCountErrors.CategoryUnavailable);
        }

        // Sahibin aldığı para şahsi giderdir (ADR 0020 T2); işletmeye özel
        // kategoriyle çelişir ve sessizce düzeltilmez.
        if (command.TookForMyself && category.DefaultScope == TransactionScope.Business)
        {
            return ApplicationResult<CashCountDto>.Failure(CashCountErrors.ScopeConflict);
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
                // Farkın tarafını kategori söyler (ADR 0020 T2): kasadan
                // alınıp eğlenceye harcanan para şahsi giderdir. Kategori iki
                // tarafa açıksa sayımın tarafı kalır; sahibin aldığı parada
                // taraf her zaman şahsidir.
                command.TookForMyself
                    ? TransactionScope.Personal
                    : category.DefaultScope ?? cashCount.Scope,
                cashCount.CountDate,
                command.TookForMyself
                    ? CashCountDefaults.OwnerWithdrawalDescription
                    : cashCount.Note);
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

    /// <summary>
    /// Eksik parayı kasadan şahsi hesaba taşır ve aktarımı sayıma bağlar.
    /// Gelir/gider yazılmaz; para yer değiştirir.
    /// </summary>
    private async Task<ApplicationResult<CashCountDto>> RecordWithdrawalTransferAsync(
        CashCount cashCount,
        Account account,
        Guid personalAccountId,
        CashCountDifference difference,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var personal = await accountRepository.FindOwnedByIdAsync(
            personalAccountId, userId, cancellationToken);
        if (personal is null || !personal.IsActive || personal.Id == account.Id ||
            personal.DefaultScope != TransactionScope.Personal)
        {
            return ApplicationResult<CashCountDto>.Failure(
                CashCountErrors.PersonalAccountUnavailable);
        }

        try
        {
            var now = timeProvider.GetUtcNow().ToUniversalTime();
            var transfer = new Transfer(
                Guid.NewGuid(),
                userId,
                account,
                personal,
                difference.ToAdjustmentAmount(),
                cashCount.CountDate,
                CashCountDefaults.OwnerWithdrawalDescription);
            cashCount.RecordTransferAdjustment(transfer.Id, now);
            await repository.SaveTransferAdjustmentAsync(transfer, cancellationToken);

            var balanceAfterTransfer = await accountRepository.CalculateBalanceAsync(
                cashCount.AccountId, userId, cancellationToken);
            return ApplicationResult<CashCountDto>.Success(
                CashCountMapper.ToDto(cashCount, account.Name, balanceAfterTransfer));
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

    /// <summary>
    /// Standart <c>Kasa farkı</c> gider kategorisi; yoksa açılır.
    /// </summary>
    /// <remarks>
    /// Varsayılan setlerde gelir; seti daha önce kurulmuş kullanıcıda ilk
    /// kullanımda açılır ve Kategoriler'de sıradan bir kategori olarak durur.
    /// Aynı adlı kategori pasifse yenisi açılmaz (ad + tür tekildir): boş
    /// döner ve istek "kategori kullanılamıyor" ile reddedilir.
    /// </remarks>
    private async Task<Category?> FindOrOpenDifferenceCategoryAsync(
        Guid userId,
        TransactionScope scope,
        CancellationToken cancellationToken)
    {
        var expenses = await categoryRepository.ListAsync(
            userId, CategoryType.Expense, null, cancellationToken);
        var existing = expenses.FirstOrDefault(item => string.Equals(
            item.Name,
            CashCountDefaults.DifferenceCategoryName,
            StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            return existing;
        }

        var opened = new Category(
            Guid.NewGuid(),
            userId,
            CashCountDefaults.DifferenceCategoryName,
            CategoryType.Expense,
            scope);
        await categoryRepository.AddAsync(opened, cancellationToken);
        return opened;
    }
}

internal static class CashCountMapper
{
    public static CashCountDto ToDto(
        CashCount cashCount,
        string accountName,
        decimal? expectedBalance,
        bool adjustmentCancelled = false)
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
                : null,
            cashCount.AdjustmentTransferId,
            CashCountAdjustmentStatuses.Of(cashCount.IsAdjusted, adjustmentCancelled));
    }
}
