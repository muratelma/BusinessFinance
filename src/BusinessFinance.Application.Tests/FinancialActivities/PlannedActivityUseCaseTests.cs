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
            CancellationToken cancellationToken)
        {
            WasQueried = true;
            LastHorizon = horizonDate;
            return Task.FromResult<IReadOnlyList<PlannedActivityDto>>(items);
        }
    }
}
