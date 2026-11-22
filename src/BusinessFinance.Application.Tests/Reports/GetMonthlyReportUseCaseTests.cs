using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Reports;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Reports;

public sealed class GetMonthlyReportUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    // 22 Kasım 2026, Türkiye günü (UTC+3).
    private static readonly DateTimeOffset Now = new(2026, 11, 22, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CurrentMonth_ComparesWithThePreviousMonthUpToTheSameDay()
    {
        var repository = new FakeReportRepository(currentExpense: 8901m, previousExpense: 7660m);
        var useCase = new GetMonthlyReportUseCase(
            new FakeCurrentUser(UserId), repository, new FixedTimeProvider(Now));

        var result = await useCase.ExecuteAsync(2026, 11, TransactionScope.Personal);

        Assert.True(result.IsSuccess);
        // 1–22 Ekim: bitiş 23 Ekim hariç.
        Assert.Equal((new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 23)), repository.ExpenseRange);
        Assert.Equal(TransactionScope.Personal, repository.ExpenseScope);
        var comparison = result.Value.ExpenseComparison!;
        Assert.Equal(7660m, comparison.PreviousExpense);
        Assert.Equal(1241m, comparison.Change);
        Assert.True(comparison.ThroughToday);
    }

    [Fact]
    public async Task PastMonth_ComparesTwoWholeMonths()
    {
        var repository = new FakeReportRepository(currentExpense: 8901m, previousExpense: 9870m);
        var useCase = new GetMonthlyReportUseCase(
            new FakeCurrentUser(UserId), repository, new FixedTimeProvider(Now));

        var result = await useCase.ExecuteAsync(2026, 10);

        Assert.Equal((new DateOnly(2026, 9, 1), new DateOnly(2026, 10, 1)), repository.ExpenseRange);
        Assert.Equal(-969m, result.Value.ExpenseComparison!.Change);
        Assert.False(result.Value.ExpenseComparison.ThroughToday);
    }

    [Fact]
    public async Task ShorterPreviousMonth_StopsAtItsLastDay()
    {
        var repository = new FakeReportRepository(currentExpense: 100m, previousExpense: 50m);
        var useCase = new GetMonthlyReportUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FixedTimeProvider(new DateTimeOffset(2026, 3, 31, 9, 0, 0, TimeSpan.Zero)));

        await useCase.ExecuteAsync(2026, 3);

        Assert.Equal((new DateOnly(2026, 2, 1), new DateOnly(2026, 3, 1)), repository.ExpenseRange);
    }

    [Fact]
    public async Task NoPreviousExpenseOrFutureMonth_HasNoComparison()
    {
        var empty = new FakeReportRepository(currentExpense: 100m, previousExpense: 0m);
        var useCase = new GetMonthlyReportUseCase(
            new FakeCurrentUser(UserId), empty, new FixedTimeProvider(Now));
        Assert.Null((await useCase.ExecuteAsync(2026, 11)).Value.ExpenseComparison);

        var future = new FakeReportRepository(currentExpense: 0m, previousExpense: 500m);
        var futureCase = new GetMonthlyReportUseCase(
            new FakeCurrentUser(UserId), future, new FixedTimeProvider(Now));
        Assert.Null((await futureCase.ExecuteAsync(2026, 12)).Value.ExpenseComparison);
        Assert.Null(future.ExpenseRange);
    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class FakeReportRepository(decimal currentExpense, decimal previousExpense)
        : IFinancialReportRepository
    {
        public (DateOnly Start, DateOnly EndExclusive)? ExpenseRange { get; private set; }
        public TransactionScope? ExpenseScope { get; private set; }

        public Task<MonthlyReportDto> GetMonthlyAsync(
            Guid userId,
            int year,
            int month,
            TransactionScope? scope,
            CancellationToken cancellationToken) =>
            Task.FromResult(new MonthlyReportDto(
                year, month, scope, 0m, currentExpense, -currentExpense, CurrencyCode.TRY, [], [], []));

        public Task<decimal> GetExpenseAsync(
            Guid userId,
            DateOnly start,
            DateOnly endExclusive,
            TransactionScope? scope,
            CancellationToken cancellationToken)
        {
            ExpenseRange = (start, endExclusive);
            ExpenseScope = scope;
            return Task.FromResult(previousExpense);
        }

        public Task<AdvancedFinancialReportDto> GetAdvancedAsync(
            Guid userId,
            int year,
            int month,
            DateOnly asOfDate,
            int trendMonths,
            int daysAhead,
            TransactionScope? scope,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
