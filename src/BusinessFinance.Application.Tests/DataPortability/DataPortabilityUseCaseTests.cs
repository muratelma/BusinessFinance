using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.DataPortability;

namespace BusinessFinance.Application.Tests.DataPortability;

public sealed class DataPortabilityUseCaseTests
{
    [Fact]
    public async Task CreateBackup_WithoutCurrentUser_DoesNotReadRepository()
    {
        var repository = new StubRepository();
        var useCase = new CreateBackupUseCase(new StubCurrentUser(null), repository);

        var result = await useCase.ExecuteAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.Equal(0, repository.Calls);
    }

    [Fact]
    public async Task Restore_MapsDestinationConflictAndUsesSessionOwner()
    {
        var userId = Guid.NewGuid();
        var repository = new StubRepository
        {
            RestoreError = new DataPortabilityException(
                "restore.destination_not_empty",
                "Restore requires an empty destination.",
                ApplicationErrorType.Conflict)
        };
        var useCase = new RestoreBackupUseCase(
            new StubCurrentUser(userId), repository, TimeProvider.System);

        var result = await useCase.ExecuteAsync([1, 2, 3]);

        Assert.False(result.IsSuccess);
        Assert.Equal("restore.destination_not_empty", result.Error.Code);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
        Assert.Equal(userId, repository.LastUserId);
    }

    private sealed class StubCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class StubRepository : IDataPortabilityRepository
    {
        public int Calls { get; private set; }
        public Guid? LastUserId { get; private set; }
        public DataPortabilityException? RestoreError { get; init; }

        public Task<PortableFile> ExportTransactionsCsvAsync(Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<PortableFile> ExportFinancialJsonAsync(Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<PortableFile> CreateBackupAsync(Guid userId, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(new PortableFile("backup", "application/json", []));
        }
        public Task<BackupValidationDto> ValidateBackupAsync(byte[] content, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<RestoreSummaryDto> RestoreBackupAsync(
            Guid userId,
            byte[] content,
            DateTimeOffset restoredAtUtc,
            CancellationToken cancellationToken)
        {
            Calls++;
            LastUserId = userId;
            return RestoreError is null
                ? Task.FromResult(new RestoreSummaryDto(1, 0, restoredAtUtc))
                : Task.FromException<RestoreSummaryDto>(RestoreError);
        }
    }
}
