using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts.DeleteAccount;

namespace BusinessFinance.Application.Tests.Accounts;

public sealed class DeleteAccountUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid AccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Theory]
    [InlineData(UnusedAccountDeletionResult.NotFound, ApplicationErrorType.NotFound)]
    [InlineData(UnusedAccountDeletionResult.InUse, ApplicationErrorType.Conflict)]
    public async Task ExecuteAsync_WhenDeletionIsRejected_ReturnsExpectedError(
        UnusedAccountDeletionResult deletionResult,
        ApplicationErrorType expectedErrorType)
    {
        var deletion = new RecordingUnusedAccountDeletion(deletionResult);
        var useCase = new DeleteAccountUseCase(new FakeCurrentUser(UserId), deletion);

        var result = await useCase.ExecuteAsync(new DeleteAccountCommand(AccountId));

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedErrorType, result.Error.Type);
        if (deletionResult == UnusedAccountDeletionResult.InUse)
        {
            Assert.Equal("accounts.in_use", result.Error.Code);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WhenAccountIsUnused_DeletesWithCurrentOwner()
    {
        var deletion = new RecordingUnusedAccountDeletion(
            UnusedAccountDeletionResult.Deleted);
        var useCase = new DeleteAccountUseCase(new FakeCurrentUser(UserId), deletion);
        using var cancellationTokenSource = new CancellationTokenSource();

        var result = await useCase.ExecuteAsync(
            new DeleteAccountCommand(AccountId),
            cancellationTokenSource.Token);

        Assert.True(result.IsSuccess);
        Assert.Equal(AccountId, result.Value.AccountId);
        Assert.Equal(AccountId, deletion.ReceivedAccountId);
        Assert.Equal(UserId, deletion.ReceivedUserId);
        Assert.Equal(cancellationTokenSource.Token, deletion.ReceivedCancellationToken);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutAuthenticatedUser_DoesNotTryToDelete()
    {
        var deletion = new RecordingUnusedAccountDeletion(
            UnusedAccountDeletionResult.Deleted);
        var useCase = new DeleteAccountUseCase(new FakeCurrentUser(null), deletion);

        var result = await useCase.ExecuteAsync(new DeleteAccountCommand(AccountId));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.False(deletion.WasCalled);
    }

    [Fact]
    public void Constructor_WithEmptyAccountId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DeleteAccountCommand(Guid.Empty));
    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class RecordingUnusedAccountDeletion(
        UnusedAccountDeletionResult result) : IUnusedAccountDeletion
    {
        public bool WasCalled { get; private set; }
        public Guid ReceivedAccountId { get; private set; }
        public Guid ReceivedUserId { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task<UnusedAccountDeletionResult> DeleteOwnedIfUnusedAsync(
            Guid accountId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            WasCalled = true;
            ReceivedAccountId = accountId;
            ReceivedUserId = userId;
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(result);
        }
    }
}
