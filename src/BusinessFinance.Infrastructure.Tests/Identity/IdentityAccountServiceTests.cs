using Microsoft.AspNetCore.Identity;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Tests.Identity;

public sealed class IdentityAccountServiceTests
{
    private const string ValidPassword = "Valid-Password-123!";
    private static readonly DateTimeOffset UtcNow = new(
        2026,
        8,
        7,
        12,
        0,
        0,
        TimeSpan.Zero);

    [Fact]
    public async Task RegisterAsync_WithValidCredentials_StoresHashInsteadOfPassword()
    {
        var store = new InMemoryIdentityUserStore();
        using var userManager = IdentityTestFactory.CreateUserManager(store);
        var service = new IdentityAccountService(
            userManager,
            new FixedTimeProvider(UtcNow));

        var result = await service.RegisterAsync(
            "  user@example.com  ",
            ValidPassword,
            CancellationToken.None);

        Assert.Equal(IdentityRegistrationStatus.Succeeded, result.Status);
        var user = Assert.Single(store.Users);
        Assert.Equal(result.UserId, user.Id);
        Assert.Equal("user@example.com", user.Email);
        Assert.Equal("USER@EXAMPLE.COM", user.NormalizedEmail);
        Assert.Equal(UtcNow, user.CreatedAtUtc);
        Assert.NotNull(user.PasswordHash);
        Assert.NotEqual(ValidPassword, user.PasswordHash);
        Assert.Equal(
            PasswordVerificationResult.Success,
            userManager.PasswordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                ValidPassword));
    }

    [Fact]
    public async Task RegisterAsync_WithWeakPassword_DoesNotCreateUser()
    {
        var store = new InMemoryIdentityUserStore();
        using var userManager = IdentityTestFactory.CreateUserManager(store);
        var service = new IdentityAccountService(userManager, TimeProvider.System);

        var result = await service.RegisterAsync(
            "user@example.com",
            "weak",
            CancellationToken.None);

        Assert.Equal(IdentityRegistrationStatus.InvalidPassword, result.Status);
        Assert.Empty(store.Users);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsDuplicateWithoutSecondUser()
    {
        var store = new InMemoryIdentityUserStore();
        using var userManager = IdentityTestFactory.CreateUserManager(store);
        var service = new IdentityAccountService(userManager, TimeProvider.System);
        await service.RegisterAsync("user@example.com", ValidPassword, CancellationToken.None);

        var result = await service.RegisterAsync(
            "USER@example.com",
            ValidPassword,
            CancellationToken.None);

        Assert.Equal(IdentityRegistrationStatus.DuplicateEmail, result.Status);
        Assert.Single(store.Users);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsIdentity()
    {
        var store = new InMemoryIdentityUserStore();
        using var userManager = IdentityTestFactory.CreateUserManager(store);
        var service = new IdentityAccountService(userManager, TimeProvider.System);
        var registration = await service.RegisterAsync(
            "user@example.com",
            ValidPassword,
            CancellationToken.None);

        var identity = await service.AuthenticateAsync(
            "USER@example.com",
            ValidPassword,
            CancellationToken.None);

        Assert.NotNull(identity);
        Assert.Equal(registration.UserId, identity.UserId);
        Assert.Equal("user@example.com", identity.Email);
    }

    [Theory]
    [InlineData("user@example.com", "Wrong-Password-123!")]
    [InlineData("unknown@example.com", ValidPassword)]
    public async Task AuthenticateAsync_WithInvalidCredentials_ReturnsNull(
        string email,
        string password)
    {
        var store = new InMemoryIdentityUserStore();
        using var userManager = IdentityTestFactory.CreateUserManager(store);
        var service = new IdentityAccountService(userManager, TimeProvider.System);
        await service.RegisterAsync("user@example.com", ValidPassword, CancellationToken.None);

        var identity = await service.AuthenticateAsync(
            email,
            password,
            CancellationToken.None);

        Assert.Null(identity);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenUserIsInactive_ReturnsNull()
    {
        var store = new InMemoryIdentityUserStore();
        using var userManager = IdentityTestFactory.CreateUserManager(store);
        var service = new IdentityAccountService(userManager, TimeProvider.System);
        await service.RegisterAsync("user@example.com", ValidPassword, CancellationToken.None);
        store.Users.Single().Deactivate();

        var identity = await service.AuthenticateAsync(
            "user@example.com",
            ValidPassword,
            CancellationToken.None);

        Assert.Null(identity);
    }

    [Fact]
    public async Task FindActiveByIdAsync_WhenUserIsActive_ReturnsIdentity()
    {
        var store = new InMemoryIdentityUserStore();
        using var userManager = IdentityTestFactory.CreateUserManager(store);
        var service = new IdentityAccountService(userManager, TimeProvider.System);
        var registration = await service.RegisterAsync(
            "user@example.com",
            ValidPassword,
            CancellationToken.None);

        var userId = Assert.IsType<Guid>(registration.UserId);
        var identity = await service.FindActiveByIdAsync(
            userId,
            CancellationToken.None);

        Assert.NotNull(identity);
        Assert.Equal(registration.UserId, identity.UserId);
        Assert.Equal("user@example.com", identity.Email);
    }

    [Fact]
    public async Task FindActiveByIdAsync_WhenUserIsInactive_ReturnsNull()
    {
        var store = new InMemoryIdentityUserStore();
        using var userManager = IdentityTestFactory.CreateUserManager(store);
        var service = new IdentityAccountService(userManager, TimeProvider.System);
        var registration = await service.RegisterAsync(
            "user@example.com",
            ValidPassword,
            CancellationToken.None);
        store.Users.Single().Deactivate();

        var userId = Assert.IsType<Guid>(registration.UserId);
        var identity = await service.FindActiveByIdAsync(
            userId,
            CancellationToken.None);

        Assert.Null(identity);
    }

    [Fact]
    public async Task RegisterAsync_WithCanceledToken_ThrowsOperationCanceledException()
    {
        var store = new InMemoryIdentityUserStore();
        using var userManager = IdentityTestFactory.CreateUserManager(store);
        var service = new IdentityAccountService(userManager, TimeProvider.System);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => service.RegisterAsync(
                "user@example.com",
                ValidPassword,
                cancellationTokenSource.Token));
    }
}
