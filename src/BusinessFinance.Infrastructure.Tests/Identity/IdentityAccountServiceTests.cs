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

    private static IdentityAccountService CreateService(
        InMemoryIdentityUserStore store,
        out UserManager<ApplicationUser> userManager)
    {
        userManager = IdentityTestFactory.CreateUserManager(store);
        return new IdentityAccountService(userManager, new FixedTimeProvider(UtcNow));
    }

    [Fact]
    public async Task RegisterAsync_WithUsername_StoresItAsTheUserName()
    {
        var store = new InMemoryIdentityUserStore();
        var service = CreateService(store, out var userManager);
        using (userManager)
        {
            var result = await service.RegisterAsync(
                "user@example.com", "ahmet.yilmaz", ValidPassword, CancellationToken.None);

            Assert.Equal(IdentityRegistrationStatus.Succeeded, result.Status);
            var user = Assert.Single(store.Users);
            Assert.Equal("ahmet.yilmaz", user.UserName);
            Assert.Equal("user@example.com", user.Email);
            Assert.True(await service.IsUsernameTakenAsync("ahmet.yilmaz", CancellationToken.None));
            Assert.False(await service.IsUsernameTakenAsync("ahmet", CancellationToken.None));
        }
    }

    [Fact]
    public async Task RegisterAsync_WithTakenUsername_ReportsTheUsernameNotTheEmail()
    {
        var store = new InMemoryIdentityUserStore();
        var service = CreateService(store, out var userManager);
        using (userManager)
        {
            await service.RegisterAsync(
                "first@example.com", "murat", ValidPassword, CancellationToken.None);

            var taken = await service.RegisterAsync(
                "second@example.com", "murat", ValidPassword, CancellationToken.None);
            var sameEmail = await service.RegisterAsync(
                "first@example.com", "baska", ValidPassword, CancellationToken.None);

            Assert.Equal(IdentityRegistrationStatus.DuplicateUsername, taken.Status);
            Assert.Equal(IdentityRegistrationStatus.DuplicateEmail, sameEmail.Status);
            Assert.Single(store.Users);
        }
    }

    [Theory]
    [InlineData("murat")]
    [InlineData("MURAT")]
    [InlineData("  Murat ")]
    [InlineData("murat@example.com")]
    public async Task AuthenticateAsync_AcceptsTheEmailOrTheUsername(string identifier)
    {
        var store = new InMemoryIdentityUserStore();
        var service = CreateService(store, out var userManager);
        using (userManager)
        {
            await service.RegisterAsync(
                "murat@example.com", "murat", ValidPassword, CancellationToken.None);

            var identity = await service.AuthenticateAsync(
                identifier, ValidPassword, CancellationToken.None);

            Assert.NotNull(identity);
            Assert.Equal("murat@example.com", identity.Email);
        }
    }

    [Fact]
    public async Task AuthenticateAsync_UnknownUsernameAndWrongPasswordLookTheSame()
    {
        var store = new InMemoryIdentityUserStore();
        var service = CreateService(store, out var userManager);
        using (userManager)
        {
            await service.RegisterAsync(
                "murat@example.com", "murat", ValidPassword, CancellationToken.None);

            Assert.Null(await service.AuthenticateAsync(
                "kimse", ValidPassword, CancellationToken.None));
            Assert.Null(await service.AuthenticateAsync(
                "murat", "Wrong-Password-123!", CancellationToken.None));
        }
    }

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
            null,
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
            null,
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
        await service.RegisterAsync("user@example.com", null, ValidPassword, CancellationToken.None);

        var result = await service.RegisterAsync(
            "USER@example.com",
            null,
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
            null,
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
        await service.RegisterAsync("user@example.com", null, ValidPassword, CancellationToken.None);

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
        await service.RegisterAsync("user@example.com", null, ValidPassword, CancellationToken.None);
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
            null,
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
            null,
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
                null,
                ValidPassword,
                cancellationTokenSource.Token));
    }
}
