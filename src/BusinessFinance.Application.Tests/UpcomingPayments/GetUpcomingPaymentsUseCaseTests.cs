using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.UpcomingPayments;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.UpcomingPayments;

public sealed class GetUpcomingPaymentsUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task Execute_ClassifiesBoundariesAndSortsByDueDate()
    {
        var asOfDate = new DateOnly(2026, 8, 11);
        var repository = new FakeUpcomingPaymentRepository(
            Candidate(asOfDate.AddDays(1), UpcomingPaymentSourceType.Installment),
            Candidate(asOfDate.AddDays(-1), UpcomingPaymentSourceType.CreditCardStatement),
            Candidate(asOfDate, UpcomingPaymentSourceType.RecurringOccurrence));
        var useCase = new GetUpcomingPaymentsUseCase(
            new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new GetUpcomingPaymentsQuery(asOfDate, 30));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            [UpcomingPaymentTiming.Overdue, UpcomingPaymentTiming.Today, UpcomingPaymentTiming.Upcoming],
            result.Value.Select(item => item.Timing));
        Assert.Equal(
            [asOfDate.AddDays(-1), asOfDate, asOfDate.AddDays(1)],
            result.Value.Select(item => item.DueDate));
        Assert.Equal(asOfDate.AddDays(30), repository.LastHorizonDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(91)]
    public async Task Execute_WithUnsupportedHorizon_ReturnsValidation(int daysAhead)
    {
        var repository = new FakeUpcomingPaymentRepository();
        var result = await new GetUpcomingPaymentsUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new GetUpcomingPaymentsQuery(new DateOnly(2026, 8, 11), daysAhead));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.Equal(0, repository.Calls);
    }

    [Fact]
    public async Task Execute_WithoutUser_ReturnsUnauthorizedBeforeQuery()
    {
        var repository = new FakeUpcomingPaymentRepository();
        var result = await new GetUpcomingPaymentsUseCase(
            new FakeCurrentUser(null), repository).ExecuteAsync(
            new GetUpcomingPaymentsQuery(new DateOnly(2026, 8, 11), 30));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.Equal(0, repository.Calls);
    }

    [Fact]
    public async Task Execute_WithOutOfRangeAsOfDate_ReturnsValidationBeforeDateArithmetic()
    {
        var repository = new FakeUpcomingPaymentRepository();
        var result = await new GetUpcomingPaymentsUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new GetUpcomingPaymentsQuery(new DateOnly(9999, 12, 31), 30));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.Equal(0, repository.Calls);
    }

    private static UpcomingPaymentCandidate Candidate(
        DateOnly dueDate,
        UpcomingPaymentSourceType sourceType) => new(
        Guid.NewGuid(),
        sourceType,
        sourceType.ToString(),
        100m,
        CurrencyCode.TRY,
        dueDate,
        null);

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FakeUpcomingPaymentRepository(params UpcomingPaymentCandidate[] candidates)
        : IUpcomingPaymentRepository
    {
        public int Calls { get; private set; }
        public DateOnly? LastHorizonDate { get; private set; }

        public Task<IReadOnlyList<UpcomingPaymentCandidate>> ListCandidatesAsync(
            Guid userId,
            DateOnly asOfDate,
            DateOnly horizonDate,
            CancellationToken cancellationToken)
        {
            Calls++;
            LastHorizonDate = horizonDate;
            return Task.FromResult<IReadOnlyList<UpcomingPaymentCandidate>>(candidates);
        }
    }
}
