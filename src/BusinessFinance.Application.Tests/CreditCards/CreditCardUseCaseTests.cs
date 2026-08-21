using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.CreditCards;

public sealed class CreditCardUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task Create_WithValidCommand_PersistsCurrentUsersCard()
    {
        var repository = new FakeRepository();
        var useCase = new CreateCreditCardUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new CreateCreditCardCommand(
            " Main Card ", 10000m, CurrencyCode.TRY, 10, 20, null));

        Assert.True(result.IsSuccess);
        var card = Assert.Single(repository.Items);
        Assert.Equal(UserId, card.UserId);
        Assert.Equal("Main Card", card.Name);
        Assert.Equal(0m, result.Value.CurrentDebt);
        Assert.Equal(10000m, result.Value.AvailableLimit);
    }

    [Fact]
    public async Task Create_WithDuplicateName_ReturnsConflict()
    {
        var repository = new FakeRepository(CreateCard(UserId));
        var useCase = new CreateCreditCardUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new CreateCreditCardCommand(
            "Main Card", 10000m, CurrencyCode.TRY, 10, 20, null));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
        Assert.Single(repository.Items);
    }

    [Fact]
    public async Task Get_ForeignCard_ReturnsNotFound()
    {
        var foreign = CreateCard(Guid.NewGuid());
        var repository = new FakeRepository(foreign);

        var result = await new GetCreditCardUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(foreign.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task Update_ChangesTermsWithoutChangingOwner()
    {
        var card = CreateCard(UserId);
        var repository = new FakeRepository(card) { CurrentDebt = 2500m };

        var result = await new UpdateCreditCardUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new UpdateCreditCardCommand(
                card.Id, "Updated", 12000m, CurrencyCode.TRY, 12, 24, null, false));

        Assert.True(result.IsSuccess);
        Assert.Equal(UserId, card.UserId);
        Assert.Equal("Updated", card.Name);
        Assert.Equal(2500m, result.Value.CurrentDebt);
        Assert.Equal(9500m, result.Value.AvailableLimit);
        Assert.False(result.Value.IsActive);
    }

    [Fact]
    public async Task Create_WithoutARate_UsesTheDefaultMinimumPaymentRate()
    {
        var repository = new FakeRepository();

        var result = await new CreateCreditCardUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new CreateCreditCardCommand(
                "Rate Card", 10000m, CurrencyCode.TRY, 10, 20, null));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            CreditCard.DefaultMinimumPaymentRate,
            result.Value.MinimumPaymentRate);
    }

    [Fact]
    public async Task Update_WithoutARate_KeepsTheRateTheUserAlreadyChose()
    {
        // Oran gönderilmediğinde varsayılana düşülseydi, kart adını
        // değiştirmek kullanıcının girdiği %40'ı sessizce %20'ye çevirirdi.
        var card = new CreditCard(
            Guid.NewGuid(), UserId, "Main Card",
            new Money(10000m, CurrencyCode.TRY), 10, 20, 40m);
        var repository = new FakeRepository(card);

        var result = await new UpdateCreditCardUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new UpdateCreditCardCommand(
                card.Id, "Renamed", 10000m, CurrencyCode.TRY, 10, 20, null, true));

        Assert.True(result.IsSuccess);
        Assert.Equal(40m, result.Value.MinimumPaymentRate);
    }

    [Fact]
    public async Task Update_WithARate_ChangesIt()
    {
        var card = CreateCard(UserId);
        var repository = new FakeRepository(card);

        var result = await new UpdateCreditCardUseCase(
            new FakeCurrentUser(UserId), repository).ExecuteAsync(
            new UpdateCreditCardCommand(
                card.Id, "Main Card", 10000m, CurrencyCode.TRY, 10, 20, 40m, true));

        Assert.True(result.IsSuccess);
        Assert.Equal(40m, result.Value.MinimumPaymentRate);
        Assert.Equal(40m, card.MinimumPaymentRate);
    }

    private static CreditCard CreateCard(Guid userId) => new(
        Guid.NewGuid(), userId, "Main Card", new Money(10000m, CurrencyCode.TRY), 10, 20);

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FakeRepository(params CreditCard[] cards) : ICreditCardRepository
    {
        public List<CreditCard> Items { get; } = [.. cards];
        public decimal CurrentDebt { get; init; }

        public Task AddAsync(CreditCard creditCard, CancellationToken cancellationToken)
        {
            Items.Add(creditCard);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByNameAsync(Guid userId, string normalizedName, Guid? exceptCreditCardId, CancellationToken cancellationToken) =>
            Task.FromResult(Items.Any(card => card.UserId == userId &&
                card.Id != exceptCreditCardId &&
                string.Equals(card.Name, normalizedName, StringComparison.OrdinalIgnoreCase)));

        public Task<CreditCard?> FindOwnedByIdAsync(Guid creditCardId, Guid userId, bool track, CancellationToken cancellationToken) =>
            Task.FromResult(Items.SingleOrDefault(card => card.Id == creditCardId && card.UserId == userId));

        public Task<IReadOnlyList<CreditCard>> ListAsync(Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<CreditCard>>(Items.Where(card => card.UserId == userId).ToArray());

        public Task<decimal> CalculateCurrentDebtAsync(Guid creditCardId, Guid userId, CancellationToken cancellationToken) =>
            Task.FromResult(CurrentDebt);

        public Task UpdateOwnedAsync(CreditCard creditCard, Guid userId, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
