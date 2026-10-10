using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.DayCloses;

/// <summary>
/// Gün sonu panelinin önizlemesi: yazılacak tutarlar, düşülen kayıtlar,
/// komisyon ve beklenen gün. Hiçbir şey yazmaz.
/// </summary>
public sealed class PreviewDayCloseUseCase(
    ICurrentUser currentUser,
    IDayCloseRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<DayClosePreviewDto>> ExecuteAsync(
        DayCloseInput input,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<DayClosePreviewDto>.Failure(
                DayCloseErrors.AuthenticationRequired);
        }

        var (plan, error) = await DayClosePlan.BuildAsync(
            userId, input, LocalDay.LatestAllowed(timeProvider.GetUtcNow()),
            repository, accountRepository, categoryRepository, cancellationToken);
        return plan is null
            ? ApplicationResult<DayClosePreviewDto>.Failure(error!)
            : ApplicationResult<DayClosePreviewDto>.Success(plan.ToPreview());
    }
}

/// <summary>
/// Gün sonunu yazar: nakit satış kasaya bir gelir, her POS'un kartlı satışı
/// bir POS tahsilatı olur (ADR 0019 T1).
/// </summary>
/// <remarks>
/// <para>
/// Gün sonu yeni bir kayıt türü üretmez (İ3) ve o gün zaten girilmiş kayıtları
/// düşer (İ2); yazdığı kayıtlar raporlara, bütçeye ve İşlemler'e başka her
/// gelir ve POS tahsilatı gibi girer.
/// </para>
/// <para>
/// Gün sonu ve ürettiği kayıtlar tek <c>SaveChanges</c> ile yazılır. İstek
/// kimliği gün sonunun kimliğini belirler (<see cref="RequestScopedId"/>):
/// tekrar gönderilen istek ilk gün sonunu döner.
/// </para>
/// </remarks>
public sealed class CreateDayCloseUseCase(
    ICurrentUser currentUser,
    IDayCloseRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider)
{
    internal const string IdPurpose = "day-close";

    public async Task<ApplicationResult<DayCloseDto>> ExecuteAsync(
        CreateDayCloseCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<DayCloseDto>.Failure(DayCloseErrors.AuthenticationRequired);
        }

        if (command.ClientRequestId == Guid.Empty)
        {
            return ApplicationResult<DayCloseDto>.Failure(
                DayCloseErrors.Validation("Client request id is required."));
        }

        var dayCloseId = RequestScopedId.Create(IdPurpose, userId, command.ClientRequestId);
        if (await repository.GetAsync(dayCloseId, userId, cancellationToken) is DayCloseDto existing)
        {
            return ApplicationResult<DayCloseDto>.Success(existing);
        }

        var now = timeProvider.GetUtcNow().ToUniversalTime();
        var (plan, error) = await DayClosePlan.BuildAsync(
            userId, command.Input, LocalDay.LatestAllowed(now),
            repository, accountRepository, categoryRepository, cancellationToken);
        if (plan is null || plan.Blocker is not null)
        {
            return ApplicationResult<DayCloseDto>.Failure(error ?? plan!.Blocker!);
        }

        try
        {
            var input = command.Input;
            var incomes = new List<BudgetTransaction>();
            if (plan.Cash.ToWrite > 0m)
            {
                incomes.Add(new BudgetTransaction(
                    Guid.NewGuid(),
                    userId,
                    plan.Cash.Account!,
                    plan.Cash.Category!,
                    new Money(plan.Cash.ToWrite, plan.Cash.Account!.Currency),
                    TransactionType.Income,
                    plan.Cash.Scope!.Value,
                    input.Date,
                    dayCloseId: dayCloseId));
            }

            var settlements = plan.PosLines
                .Where(line => line.ToWrite > 0m)
                .Select(line => new PosSettlement(
                    Guid.NewGuid(),
                    userId,
                    line.Account!,
                    line.SalesCategory!,
                    new Money(line.ToWrite, line.Account!.Currency),
                    line.Commission,
                    line.Scope!.Value,
                    input.Date,
                    line.Definition.ExpectedTransferDate(input.Date),
                    now,
                    line.Commission > 0m ? line.CommissionCategory : null,
                    definition: line.Definition,
                    dayCloseId: dayCloseId))
                .ToArray();

            var dayClose = DayClose.Record(
                dayCloseId, userId, input.Date, now, incomes, settlements,
                input.RangeStart, input.ZNumber, input.IsAdditional);
            // Düşülen kayıtlar sahiplenilir: ikinci kez düşülemez ve tek
            // başına iptal edilemez.
            var counted = plan.Counted
                .Select(record => new DayCloseCountedRecord(dayClose, record.Kind, record.Id))
                .ToArray();
            // Ortak tutar kayıtlardan yeniden hesaplanamaz; günün ekranı ve
            // geri alma için saklanır. Sıfır ("ayrı ayrı") saklanmaz.
            var sharedAmounts = plan.OverlapGroups
                .Where(group => group.OverlapAmount > 0m)
                .Select(group => new DayCloseCountedOverlap(
                    dayClose, group.GroupId, group.OverlapAmount!.Value))
                .ToArray();
            if (!await repository.TryAddAsync(
                    dayClose, incomes, settlements, counted, sharedAmounts, cancellationToken))
            {
                // Aynı istek eşzamanlı ikinci kez geldiyse ilki yazılmıştır.
                return await repository.GetAsync(dayCloseId, userId, cancellationToken)
                    is DayCloseDto raced
                    ? ApplicationResult<DayCloseDto>.Success(raced)
                    : ApplicationResult<DayCloseDto>.Failure(DayCloseErrors.ConcurrentChange);
            }
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<DayCloseDto>.Failure(
                DayCloseErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<DayCloseDto>.Failure(
                DayCloseErrors.Conflict(exception.Message));
        }

        var persisted = await repository.GetAsync(dayCloseId, userId, cancellationToken)
            ?? throw new InvalidOperationException("Saved day close could not be read back.");
        return ApplicationResult<DayCloseDto>.Success(persisted);
    }
}

public sealed class GetDayCloseUseCase(ICurrentUser currentUser, IDayCloseRepository repository)
{
    public async Task<ApplicationResult<DayCloseDto>> ExecuteAsync(
        Guid dayCloseId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<DayCloseDto>.Failure(DayCloseErrors.AuthenticationRequired);
        }

        return await repository.GetAsync(dayCloseId, userId, cancellationToken) is DayCloseDto dayClose
            ? ApplicationResult<DayCloseDto>.Success(dayClose)
            : ApplicationResult<DayCloseDto>.Failure(DayCloseErrors.NotFound(dayCloseId));
    }
}

/// <summary>
/// Bir aralıktaki günleri kapatan gün sonları: Kasa "bu gün kapatıldı mı"
/// sorusunu ve bir kaydın hangi gün sonundan geldiğini buradan okur.
/// </summary>
public sealed class ListDayClosesUseCase(ICurrentUser currentUser, IDayCloseRepository repository)
{
    public const int MaximumRangeDays = 366;

    public async Task<ApplicationResult<IReadOnlyList<DayCloseDto>>> ExecuteAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<DayCloseDto>>.Failure(
                DayCloseErrors.AuthenticationRequired);
        }

        if (to < from || to.DayNumber - from.DayNumber > MaximumRangeDays)
        {
            return ApplicationResult<IReadOnlyList<DayCloseDto>>.Failure(
                DayCloseErrors.InvalidRange);
        }

        return ApplicationResult<IReadOnlyList<DayCloseDto>>.Success(
            await repository.ListAsync(userId, from, to, cancellationToken));
    }
}

/// <summary>
/// Bir günün bütününü okur: o günü kapatan gün sonları, yazdıkları ve
/// saydıkları kayıtlar, dışarıda kalan kayıtlar ve günün toplamı.
/// </summary>
public sealed class GetDayCloseDayUseCase(
    ICurrentUser currentUser,
    IDayCloseRepository repository)
{
    public async Task<ApplicationResult<DayCloseDayDto>> ExecuteAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<DayCloseDayDto>.Failure(
                DayCloseErrors.AuthenticationRequired);
        }

        if (date == default)
        {
            return ApplicationResult<DayCloseDayDto>.Failure(DayCloseErrors.InvalidDate);
        }

        // Depo en yeniyi önce verir; günün ilk gün sonu üstte, ekler altında.
        var closes = (await repository.ListAsync(userId, date, date, cancellationToken))
            .Reverse()
            .OrderBy(dayClose => dayClose.IsAdditional)
            .ToArray();
        var outside = await repository.ListExistingRecordsAsync(
            userId, date, date, cancellationToken);
        return ApplicationResult<DayCloseDayDto>.Success(new DayCloseDayDto(
            date,
            closes,
            outside,
            closes.Sum(dayClose => dayClose.CashAmount + dayClose.CountedCashAmount),
            closes.Sum(dayClose => dayClose.CardGrossAmount + dayClose.CountedCardAmount),
            CurrencyCode.TRY));
    }
}

/// <summary>
/// Gün sonunu bir bütün olarak geri alır: ürettiği gelir ve POS tahsilatları
/// birlikte iptal olur, gün yeniden açılır (ADR 0019 T1, İ8).
/// </summary>
/// <remarks>
/// Silme yerine iptal: gün sonu ve kayıtları kalır. Hepsi tek
/// <c>SaveChanges</c> ile yazılır. İdempotenttir. Ürettiği bir tahsilat bir
/// yatışla hesaba geçtiyse reddedilir; önce yatış geri alınır.
/// </remarks>
public sealed class RevertDayCloseUseCase(
    ICurrentUser currentUser,
    IDayCloseRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<DayCloseDto>> ExecuteAsync(
        Guid dayCloseId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<DayCloseDto>.Failure(DayCloseErrors.AuthenticationRequired);
        }

        var dayClose = await repository.FindOwnedByIdAsync(
            dayCloseId, userId, true, cancellationToken);
        if (dayClose is null)
        {
            return ApplicationResult<DayCloseDto>.Failure(DayCloseErrors.NotFound(dayCloseId));
        }

        if (!dayClose.IsCancelled)
        {
            var (incomes, settlements) = await repository.FindRecordsAsync(
                dayCloseId, userId, cancellationToken);
            if (settlements.Any(settlement => settlement.PosDepositId is not null))
            {
                return ApplicationResult<DayCloseDto>.Failure(DayCloseErrors.DepositLocked);
            }

            // Ana gün sonu, ekleri geri alınmadan geri alınamaz.
            if (!dayClose.IsAdditional)
            {
                var covering = await repository.ListCoveringAsync(
                    userId, dayClose.FirstDay, dayClose.ClosedOn, cancellationToken);
                if (covering.Any(other => other.IsAdditional))
                {
                    return ApplicationResult<DayCloseDto>.Failure(
                        DayCloseErrors.AdditionalExists);
                }
            }

            try
            {
                dayClose.Revert(
                    incomes, settlements, timeProvider.GetUtcNow().ToUniversalTime());
            }
            catch (InvalidOperationException exception)
            {
                return ApplicationResult<DayCloseDto>.Failure(
                    DayCloseErrors.Conflict(exception.Message));
            }

            // Saydığı kayıtlar serbest kalır: yeni bir gün sonunda yeniden
            // sayılabilir ya da tek başına iptal edilebilirler.
            await repository.ReleaseCountedAsync(dayCloseId, userId, cancellationToken);
            if (!await repository.TrySaveAsync(cancellationToken))
            {
                return ApplicationResult<DayCloseDto>.Failure(DayCloseErrors.ConcurrentChange);
            }
        }

        var reverted = await repository.GetAsync(dayCloseId, userId, cancellationToken)
            ?? throw new InvalidOperationException("Reverted day close could not be read back.");
        return ApplicationResult<DayCloseDto>.Success(reverted);
    }
}
