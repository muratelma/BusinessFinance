using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.FinancialActivities;

public sealed class PlannedActivityUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly DateOnly AsOfDate = new(2026, 8, 15);

    [Theory]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(90)]
    public async Task Execute_AcceptsOnlyTheOfferedHorizons(int daysAhead)
    {
        var repository = new FakePlannedActivityRepository();
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(AsOfDate, daysAhead));

        Assert.True(result.IsSuccess);
        Assert.Equal(AsOfDate.AddDays(daysAhead), repository.LastHorizon);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(90)]
    public async Task Execute_CountsTodayAsFirstCalendarDayWhenRequested(int daysAhead)
    {
        var repository = new FakePlannedActivityRepository();
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(
            new PlannedActivityQuery(AsOfDate, daysAhead, CountTodayInHorizon: true));

        Assert.True(result.IsSuccess);
        Assert.Equal(AsOfDate.AddDays(daysAhead - 1), repository.LastHorizon);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(45)]
    [InlineData(365)]
    public async Task Execute_RejectsAnyOtherHorizonWithoutQuerying(int daysAhead)
    {
        var repository = new FakePlannedActivityRepository();
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(AsOfDate, daysAhead));

        Assert.False(result.IsSuccess);
        Assert.Equal("planned_activities.invalid_days_ahead", result.Error.Code);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.False(repository.WasQueried);
    }

    [Fact]
    public async Task Execute_WithoutAnIdentity_NeverReachesTheRepository()
    {
        var repository = new FakePlannedActivityRepository();
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(null), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(AsOfDate, 30));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.False(repository.WasQueried);
    }

    /// <summary>
    /// Overdue first, then by due date. A single total is deliberately absent, so the
    /// summary is a count and the nearest date.
    /// </summary>
    [Fact]
    public async Task Execute_PutsOverdueFirstAndReportsCountAndNearestDate()
    {
        var repository = new FakePlannedActivityRepository(
            Planned(new DateOnly(2026, 9, 1), PlannedActivityTiming.Upcoming),
            Planned(new DateOnly(2026, 8, 20), PlannedActivityTiming.Upcoming),
            Planned(new DateOnly(2026, 8, 10), PlannedActivityTiming.Overdue),
            Planned(new DateOnly(2026, 8, 15), PlannedActivityTiming.Today));
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(AsOfDate, 30));

        Assert.True(result.IsSuccess);
        var value = result.Value;
        Assert.Equal(4, value.TotalCount);
        Assert.Equal(new DateOnly(2026, 8, 10), value.NearestDueDate);
        Assert.Equal(
            [
                new DateOnly(2026, 8, 10),
                new DateOnly(2026, 8, 15),
                new DateOnly(2026, 8, 20),
                new DateOnly(2026, 9, 1)
            ],
            value.Items.Select(item => item.DueDate));
        Assert.Equal(PlannedActivityTiming.Overdue, value.Items[0].Timing);
    }

    [Fact]
    public async Task Execute_WithNothingPlanned_ReportsNoNearestDate()
    {
        var useCase = new ListPlannedActivitiesUseCase(
            new FakeCurrentUser(UserId), new FakePlannedActivityRepository());

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(AsOfDate, 7));

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.TotalCount);
        Assert.Null(result.Value.NearestDueDate);
        Assert.Empty(result.Value.Items);
    }

    /// <summary>
    /// Gecikmiş ödeme yükümlülükleri kendi toplamını alır; tutarı belli olmayan
    /// gecikmiş kalem toplama girmez, sayısı ayrıca döner. Gelir gecikmiş olsa da
    /// girmez.
    /// </summary>
    [Fact]
    public async Task Execute_SumsOverdueObligationsSeparately()
    {
        var repository = new FakePlannedActivityRepository(
            Planned(new DateOnly(2026, 8, 10), PlannedActivityTiming.Overdue),
            Planned(new DateOnly(2026, 8, 11), PlannedActivityTiming.Overdue) with { Amount = null },
            Planned(new DateOnly(2026, 8, 12), PlannedActivityTiming.Overdue,
                effect: FinancialActivityEffect.Income),
            Planned(new DateOnly(2026, 8, 16), PlannedActivityTiming.Upcoming));
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(AsOfDate, 7));

        Assert.True(result.IsSuccess);
        Assert.Equal(100m, result.Value.OverdueOutgoingTotal);
        Assert.Equal(1, result.Value.OverdueUnknownAmountCount);
        Assert.Equal(100m, result.Value.UpcomingOutgoingTotal);
        Assert.Equal(0, result.Value.UnknownAmountCount);
    }

    /// <summary>
    /// The only total is what will leave the user's hands inside the window: overdue
    /// items, planned income and collections stay out.
    /// </summary>
    [Fact]
    public async Task Execute_SumsOnlyUpcomingPaymentObligations()
    {
        var repository = new FakePlannedActivityRepository(
            Planned(new DateOnly(2026, 8, 15), PlannedActivityTiming.Today),
            Planned(new DateOnly(2026, 8, 18), PlannedActivityTiming.Upcoming,
                PlannedActivityKind.CardStatement,
                FinancialActivityEffect.Neutral,
                PlannedActivityAction.PayCard),
            Planned(new DateOnly(2026, 8, 10), PlannedActivityTiming.Overdue),
            Planned(new DateOnly(2026, 8, 19), PlannedActivityTiming.Upcoming,
                effect: FinancialActivityEffect.Income),
            Planned(new DateOnly(2026, 8, 20), PlannedActivityTiming.Upcoming,
                PlannedActivityKind.ReceivableInstallment,
                FinancialActivityEffect.Neutral,
                PlannedActivityAction.CollectDebt));
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(AsOfDate, 7));

        Assert.True(result.IsSuccess);
        Assert.Equal(200m, result.Value.UpcomingOutgoingTotal);
        Assert.Equal(2, result.Value.UpcomingPaymentCount);
    }

    [Fact]
    public async Task Execute_CountsUnknownPaymentsButExcludesReceiptsAndCardPostings()
    {
        var payment = Planned(AsOfDate, PlannedActivityTiming.Today);
        var repository = new FakePlannedActivityRepository(
            payment with { Amount = 4200m },
            payment with { Amount = 1200m, PlannedKind = PlannedActivityKind.CardStatement },
            payment with { Amount = 1750m },
            payment with { Amount = 350m },
            payment with { Amount = null },
            payment with { Amount = 6800m, Effect = FinancialActivityEffect.Income },
            payment with { Amount = 1200m, PlannedKind = PlannedActivityKind.CardInstallment },
            payment with { Amount = 200m, PostsToCard = true },
            payment with { DueDate = AsOfDate.AddMonths(-1), Timing = PlannedActivityTiming.Overdue });
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(AsOfDate, 7, CountTodayInHorizon: true));

        Assert.Equal(7500m, result.Value.UpcomingOutgoingTotal);
        Assert.Equal(5, result.Value.UpcomingPaymentCount);
        Assert.Equal(1, result.Value.UnknownAmountCount);
        Assert.Equal(AsOfDate.AddMonths(-1), result.Value.Items[0].DueDate);
    }

    /// <summary>
    /// The payment-burden slice keeps obligations and drops what the user is owed or
    /// will receive, which is why the upcoming-payments view is smaller by design.
    /// </summary>
    [Fact]
    public void IsPaymentObligation_KeepsWhatTheUserOwesAndDropsIncomeAndCollections()
    {
        Assert.True(PlannedActivityRules.IsPaymentObligation(Planned(
            AsOfDate, PlannedActivityTiming.Today,
            PlannedActivityKind.RecurringOccurrence,
            FinancialActivityEffect.Expense,
            PlannedActivityAction.Realize)));
        Assert.True(PlannedActivityRules.IsPaymentObligation(Planned(
            AsOfDate, PlannedActivityTiming.Today,
            PlannedActivityKind.CardStatement,
            FinancialActivityEffect.Neutral,
            PlannedActivityAction.PayCard)));
        Assert.True(PlannedActivityRules.IsPaymentObligation(Planned(
            AsOfDate, PlannedActivityTiming.Today,
            PlannedActivityKind.DebtInstallment,
            FinancialActivityEffect.Neutral,
            PlannedActivityAction.PayDebt)));

        Assert.False(PlannedActivityRules.IsPaymentObligation(Planned(
            AsOfDate, PlannedActivityTiming.Today,
            PlannedActivityKind.RecurringOccurrence,
            FinancialActivityEffect.Income,
            PlannedActivityAction.Realize)));
        Assert.False(PlannedActivityRules.IsPaymentObligation(Planned(
            AsOfDate, PlannedActivityTiming.Today,
            PlannedActivityKind.ReceivableInstallment,
            FinancialActivityEffect.Neutral,
            PlannedActivityAction.CollectDebt)));
    }

    [Theory]
    [InlineData("2026-08-14", PlannedActivityTiming.Overdue)]
    [InlineData("2026-08-15", PlannedActivityTiming.Today)]
    [InlineData("2026-08-16", PlannedActivityTiming.Upcoming)]
    public void Classify_ComparesAgainstTheAsOfDateNotToday(string dueDate, PlannedActivityTiming expected)
    {
        Assert.Equal(expected, PlannedActivityRules.Classify(DateOnly.Parse(dueDate), AsOfDate));
    }

    // 19 Kasım kararının örneği: bugün 12 Aralık, seçim 8–21 Aralık.
    private static readonly DateOnly RangeToday = new(2026, 12, 12);

    private static PlannedActivityDto Due(int day, decimal? amount = 100m) =>
        Planned(
            new DateOnly(2026, 12, day),
            PlannedActivityRules.Classify(new DateOnly(2026, 12, day), RangeToday)) with
        { Amount = amount };

    [Fact]
    public async Task Range_KeepsOpenItemsInsideAndOverdueItemsBeforeTheSelection()
    {
        var repository = new FakePlannedActivityRepository(
            Due(1, 480m),
            Due(9, 2800m),
            Due(12, 1750m),
            Due(21, 200m),
            Due(22, 999m),
            Due(14, null),
            Due(15) with { Amount = 6800m, Effect = FinancialActivityEffect.Income },
            Due(16) with { Amount = 1200m, PlannedKind = PlannedActivityKind.CardInstallment });
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(
            RangeToday, 30, From: new DateOnly(2026, 12, 8), To: new DateOnly(2026, 12, 21)));

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateOnly(2026, 12, 21), repository.LastHorizon);
        // 22 Aralık seçimin dışında; 1 Aralık seçimden önce gecikenlerde.
        Assert.DoesNotContain(result.Value.Items, item => item.DueDate.Day == 22);
        var period = result.Value.Period!;
        // Dönem içindeki gecikme (9 Aralık) bir kez, dönemde sayılır.
        Assert.Equal(2800m + 1750m + 200m, period.PaymentTotal);
        Assert.Equal(4, period.PaymentCount);
        Assert.Equal(1, period.UnknownAmountCount);
        Assert.Equal(6800m, period.IncomingTotal);
        Assert.Equal(1, period.IncomingCount);
        Assert.Equal(1200m, period.CardPostingTotal);
        Assert.Equal(1, period.CardPostingCount);
        Assert.Equal(1, period.EarlierOverdueCount);
        Assert.Equal(480m, period.EarlierOverduePaymentTotal);
        Assert.Equal(new DateOnly(2026, 12, 1), result.Value.Items[0].DueDate);
    }

    [Fact]
    public async Task Range_InTheFutureDoesNotCallTheDaysBeforeItOverdue()
    {
        var repository = new FakePlannedActivityRepository(Due(9, 2800m), Due(13, 500m), Due(16, 300m));
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(
            RangeToday, 30, From: new DateOnly(2026, 12, 15), To: new DateOnly(2026, 12, 21)));

        // 13 Aralık bugünden sonra ama seçimden önce: gecikmiş değildir, görünmez.
        Assert.Equal([9, 16], result.Value.Items.Select(item => item.DueDate.Day));
        Assert.Equal(1, result.Value.Period!.EarlierOverdueCount);
        Assert.Equal(300m, result.Value.Period.PaymentTotal);
    }

    [Fact]
    public async Task Range_TypeSelection_BuildsTheListAndTheSummaryFromTheSameItems()
    {
        var tax = Due(14, null) with { TaxKind = TaxKind.WithholdingReturn };
        var repository = new FakePlannedActivityRepository(
            Due(1, 480m) with { TaxKind = TaxKind.WithholdingReturn },
            Due(9, 2800m),
            tax,
            Due(15) with { Amount = 1200m, PlannedKind = PlannedActivityKind.CardStatement });
        var useCase = new ListPlannedActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(
            RangeToday,
            30,
            From: new DateOnly(2026, 12, 8),
            To: new DateOnly(2026, 12, 21),
            Type: PlannedTypeFilter.Tax));

        Assert.Equal([1, 14], result.Value.Items.Select(item => item.DueDate.Day));
        Assert.Equal(1, result.Value.Period!.PaymentCount);
        Assert.Equal(1, result.Value.Period.UnknownAmountCount);
        Assert.Equal(0m, result.Value.Period.PaymentTotal);
        Assert.Equal(480m, result.Value.Period.EarlierOverduePaymentTotal);
    }

    [Theory]
    [InlineData("2026-12-21", "2026-12-08")]
    [InlineData("2026-01-01", "2027-01-02")]
    public async Task Range_RejectsReversedOrTooLongSelections(string from, string to)
    {
        var useCase = new ListPlannedActivitiesUseCase(
            new FakeCurrentUser(UserId), new FakePlannedActivityRepository());

        var result = await useCase.ExecuteAsync(new PlannedActivityQuery(
            RangeToday, 30, From: DateOnly.Parse(from), To: DateOnly.Parse(to)));

        Assert.False(result.IsSuccess);
        Assert.Equal(PlannedActivityErrors.InvalidRange, result.Error);
    }

    private static PlannedActivityDto Planned(
        DateOnly dueDate,
        PlannedActivityTiming timing,
        PlannedActivityKind kind = PlannedActivityKind.RecurringOccurrence,
        FinancialActivityEffect effect = FinancialActivityEffect.Expense,
        PlannedActivityAction action = PlannedActivityAction.Realize) => new(
        Guid.NewGuid(),
        kind,
        effect,
        timing,
        PlannedActivityReadiness.Ready,
        null,
        action,
        dueDate,
        100m,
        CurrencyCode.TRY,
        "Planned",
        null,
        null,
        null,
        null,
        null,
        IsProjected: false,
        ActionTargetId: Guid.NewGuid(),
        ActionSequence: null);

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FakePlannedActivityRepository(params PlannedActivityDto[] items)
        : IPlannedActivityRepository
    {
        public bool WasQueried { get; private set; }
        public DateOnly LastHorizon { get; private set; }

        public Task<IReadOnlyList<PlannedActivityDto>> ListAsync(
            Guid userId,
            DateOnly asOfDate,
            DateOnly horizonDate,
            TransactionScope? scope,
            CancellationToken cancellationToken,
            bool includeScopedStatements = false)
        {
            WasQueried = true;
            LastHorizon = horizonDate;
            return Task.FromResult<IReadOnlyList<PlannedActivityDto>>(items);
        }

        public Task<IReadOnlyList<PlannedActivityDto>> ListForRecurringPlanAsync(
            Guid userId,
            Guid recurringTransactionId,
            DateOnly asOfDate,
            DateOnly horizonDate,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<PlannedActivityDto>>(
                [.. items.Where(item => item.RecurringTransactionId == recurringTransactionId)]);
    }
}
