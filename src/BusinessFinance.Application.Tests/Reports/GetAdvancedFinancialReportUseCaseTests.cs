using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Reports;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Reports;

public sealed class GetAdvancedFinancialReportUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task Execute_WithValidQuery_ReturnsRepositoryProjection()
    {
        var expected = EmptyReport(new DateOnly(2026, 8, 11));
        var repository = new FakeReportRepository(expected);

        var result = await new GetAdvancedFinancialReportUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new GetAdvancedFinancialReportQuery(2026, 8, expected.AsOfDate, 6, 30));

        Assert.True(result.IsSuccess);
        Assert.Same(expected, result.Value);
        Assert.Equal(1, repository.AdvancedCalls);
    }

    [Theory]
    [InlineData(1, 30)]
    [InlineData(13, 30)]
    [InlineData(6, 0)]
    [InlineData(6, 91)]
    public async Task Execute_WithInvalidRange_ReturnsValidationBeforeRepository(
        int trendMonths,
        int daysAhead)
    {
        var repository = new FakeReportRepository(EmptyReport(new DateOnly(2026, 8, 11)));

        var result = await new GetAdvancedFinancialReportUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new GetAdvancedFinancialReportQuery(
                2026, 8, new DateOnly(2026, 8, 11), trendMonths, daysAhead));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.Equal(0, repository.AdvancedCalls);
    }

    [Fact]
    public async Task Execute_WithoutUser_ReturnsUnauthorizedBeforeRepository()
    {
        var repository = new FakeReportRepository(EmptyReport(new DateOnly(2026, 8, 11)));

        var result = await new GetAdvancedFinancialReportUseCase(
            new FakeCurrentUser(null), repository).ExecuteAsync(
            new GetAdvancedFinancialReportQuery(
                2026, 8, new DateOnly(2026, 8, 11), 6, 30));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.Equal(0, repository.AdvancedCalls);
    }

    [Fact]
    public async Task Execute_WithOutOfRangeAsOfDate_ReturnsValidationBeforeRepository()
    {
        var repository = new FakeReportRepository(EmptyReport(new DateOnly(2026, 8, 11)));
        var result = await new GetAdvancedFinancialReportUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new GetAdvancedFinancialReportQuery(
                2026, 8, new DateOnly(9999, 12, 31), 6, 30));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.Equal(0, repository.AdvancedCalls);
    }

    private static AdvancedFinancialReportDto EmptyReport(DateOnly asOfDate) => new(
        asOfDate,
        CurrencyCode.TRY,
        null,
        new NetWorthDto(0m, 0m, 0m, 0m, 0m, 0m),
        new PeriodComparisonDto(
            new PeriodTotalsDto(2026, 8, 0m, 0m, 0m),
            new PeriodTotalsDto(2026, 7, 0m, 0m, 0m),
            0m,
            0m,
            0m),
        [],
        [],
        new FutureLoadDto(asOfDate, asOfDate.AddDays(30), 0m, 0m, 0m, 0m, 0m),
        [],
        []);

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FakeReportRepository(AdvancedFinancialReportDto advancedReport)
        : IFinancialReportRepository
    {
        public int AdvancedCalls { get; private set; }

        public Task<MonthlyReportDto> GetMonthlyAsync(
            Guid userId,
            int year,
            int month,
            TransactionScope? scope,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<AdvancedFinancialReportDto> GetAdvancedAsync(
            Guid userId,
            int year,
            int month,
            DateOnly asOfDate,
            int trendMonths,
            int daysAhead,
            TransactionScope? scope,
            CancellationToken cancellationToken)
        {
            AdvancedCalls++;
            return Task.FromResult(advancedReport);
        }
    }
}
