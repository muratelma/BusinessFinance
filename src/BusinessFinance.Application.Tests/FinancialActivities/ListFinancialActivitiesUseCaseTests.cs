using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.FinancialActivities;

namespace BusinessFinance.Application.Tests.FinancialActivities;

public sealed class ListFinancialActivitiesUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    /// <summary>
    /// The search reaches the one feed query trimmed, so " kira " and "kira" find
    /// the same rows and the page count follows the search.
    /// </summary>
    [Fact]
    public async Task Execute_TrimsTheSearchBeforeTheQuery()
    {
        var repository = new CapturingRepository();
        var useCase = new ListFinancialActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(Criteria("  kira  "));

        Assert.True(result.IsSuccess);
        Assert.Equal("kira", repository.Criteria?.Search);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Execute_BlankSearchIsNoFilter(string? search)
    {
        var repository = new CapturingRepository();
        var useCase = new ListFinancialActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(Criteria(search));

        Assert.True(result.IsSuccess);
        Assert.Null(repository.Criteria?.Search);
    }

    [Fact]
    public async Task Execute_RejectsAnOverlongSearchWithoutQuerying()
    {
        var repository = new CapturingRepository();
        var useCase = new ListFinancialActivitiesUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(
            Criteria(new string('a', ListFinancialActivitiesUseCase.MaximumSearchLength + 1)));

        Assert.False(result.IsSuccess);
        Assert.Equal(FinancialActivityErrors.InvalidSearch.Code, result.Error.Code);
        Assert.Null(repository.Criteria);
    }

    private static FinancialActivityListCriteria Criteria(string? search) => new(
        PageNumber: 1,
        PageSize: 20,
        DateFrom: null,
        DateTo: null,
        SourceGroup: null,
        ActivityKind: null,
        Effect: null,
        Origin: null,
        AccountId: null,
        CreditCardId: null,
        CategoryId: null,
        CounterpartyId: null,
        Scope: null,
        IncludeCancelled: true,
        Search: search);

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class CapturingRepository : IFinancialActivityRepository
    {
        public FinancialActivityListCriteria? Criteria { get; private set; }

        public Task<FinancialActivityPage> ListAsync(
            Guid userId,
            FinancialActivityListCriteria criteria,
            CancellationToken cancellationToken)
        {
            Criteria = criteria;
            return Task.FromResult(new FinancialActivityPage([], 0));
        }
    }
}
