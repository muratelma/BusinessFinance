using Microsoft.AspNetCore.Identity;
using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Tests.Identity;

public sealed class ApplicationUserTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesActiveIdentityUser()
    {
        var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var createdAtUtc = new DateTimeOffset(2026, 8, 7, 12, 0, 0, TimeSpan.Zero);

        var user = new ApplicationUser(userId, "  user@example.com  ", createdAtUtc);

        Assert.IsAssignableFrom<IdentityUser<Guid>>(user);
        Assert.Equal(userId, user.Id);
        Assert.Equal("user@example.com", user.Email);
        Assert.Equal("user@example.com", user.UserName);
        Assert.Equal(createdAtUtc, user.CreatedAtUtc);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Constructor_WithEmptyId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new ApplicationUser(Guid.Empty, "user@example.com", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WithNonUtcCreatedAt_ThrowsArgumentException()
    {
        var nonUtcTime = new DateTimeOffset(2026, 8, 7, 15, 0, 0, TimeSpan.FromHours(3));

        Assert.Throws<ArgumentException>(
            () => new ApplicationUser(Guid.NewGuid(), "user@example.com", nonUtcTime));
    }

    [Fact]
    public void Deactivate_MarksUserInactive()
    {
        var user = new ApplicationUser(
            Guid.NewGuid(),
            "user@example.com",
            DateTimeOffset.UtcNow);

        user.Deactivate();

        Assert.False(user.IsActive);
    }

    [Fact]
    public void IdentityBoundary_UsesGuidWithoutDomainDependingOnIdentity()
    {
        var currentUserIdType = typeof(ICurrentUser)
            .GetProperty(nameof(ICurrentUser.UserId))!
            .PropertyType;
        var domainReferences = typeof(Account)
            .Assembly
            .GetReferencedAssemblies();

        Assert.Equal(typeof(Guid?), currentUserIdType);
        Assert.DoesNotContain(
            domainReferences,
            reference => reference.Name?.Contains("Identity", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void ApplicationUser_DoesNotExposePlainTextPasswordProperty()
    {
        var publicProperties = typeof(ApplicationUser).GetProperties();

        Assert.DoesNotContain(
            publicProperties,
            property => string.Equals(
                property.Name,
                "Password",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            publicProperties,
            property => property.Name == nameof(IdentityUser<Guid>.PasswordHash));
    }
}
