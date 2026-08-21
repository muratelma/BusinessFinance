using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.SavingsGoals;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.SavingsGoals;

public sealed class DeleteSavingsGoalUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid GoalId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Theory]
    [InlineData(SavingsGoalDeletionResult.NotFound, ApplicationErrorType.NotFound)]
    [InlineData(SavingsGoalDeletionResult.HasContributions, ApplicationErrorType.Conflict)]
    public async Task ExecuteAsync_WhenDeletionIsRejected_ReturnsExpectedError(
        SavingsGoalDeletionResult deletionResult,
        ApplicationErrorType expectedErrorType)
    {
        var repository = new RecordingRepository(deletionResult);
        var useCase = new DeleteSavingsGoalUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new DeleteSavingsGoalCommand(GoalId));

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedErrorType, result.Error.Type);
        if (deletionResult == SavingsGoalDeletionResult.HasContributions)
        {
            Assert.Equal("goal.has_contributions", result.Error.Code);
            Assert.Equal(
                "Katkı geçmişi bulunan tasarruf hedefi silinemez.",
                result.Error.Message);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WhenGoalHasNoContributions_DeletesWithCurrentOwner()
    {
        var repository = new RecordingRepository(SavingsGoalDeletionResult.Deleted);
        var useCase = new DeleteSavingsGoalUseCase(new FakeCurrentUser(UserId), repository);

        var result = await useCase.ExecuteAsync(new DeleteSavingsGoalCommand(GoalId));

        Assert.True(result.IsSuccess);
        Assert.Equal(GoalId, result.Value);
        Assert.Equal((GoalId, UserId), (repository.ReceivedGoalId, repository.ReceivedUserId));
    }

    [Fact]
    public async Task ExecuteAsync_WithoutAuthenticatedUser_DoesNotDelete()
    {
        var repository = new RecordingRepository(SavingsGoalDeletionResult.Deleted);
        var useCase = new DeleteSavingsGoalUseCase(new FakeCurrentUser(null), repository);

        var result = await useCase.ExecuteAsync(new DeleteSavingsGoalCommand(GoalId));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.False(repository.WasCalled);
    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class RecordingRepository(SavingsGoalDeletionResult result)
        : ISavingsGoalRepository
    {
        public bool WasCalled { get; private set; }
        public Guid ReceivedGoalId { get; private set; }
        public Guid ReceivedUserId { get; private set; }

        public Task<SavingsGoalDeletionResult> DeleteOwnedIfWithoutContributionsAsync(
            Guid goalId, Guid userId, CancellationToken cancellationToken)
        {
            WasCalled = true;
            ReceivedGoalId = goalId;
            ReceivedUserId = userId;
            return Task.FromResult(result);
        }

        public Task AddAsync(SavingsGoal goal, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<IReadOnlyList<SavingsGoal>> ListAsync(
            Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<SavingsGoal?> FindOwnedByIdAsync(
            Guid goalId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<SavingsGoalContribution?> FindContributionAsync(
            Guid goalId, Guid userId, Guid clientRequestId,
            CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<SavingsGoalContribution> AddOrGetContributionAsync(
            SavingsGoalContribution contribution, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
