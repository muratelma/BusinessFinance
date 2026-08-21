using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Infrastructure.Identity.Tokens;

namespace BusinessFinance.Infrastructure.Tests.Identity;

public sealed class JwtSecurityTokenServiceTests
{
    private const string Issuer = "business-finance-tests";
    private const string Audience = "business-finance-mobile-tests";
    private static readonly string SigningKey = new('k', 64);
    private static readonly DateTimeOffset UtcNow = new(
        2026,
        8,
        7,
        12,
        0,
        0,
        TimeSpan.Zero);

    [Fact]
    public void CreateAccessToken_ProducesValidSignedTokenWithExpectedClaims()
    {
        var service = CreateService();
        var identity = new AuthenticatedIdentity(
            Guid.Parse("2b2997f4-bb5c-4dda-a749-50a87ef47661"),
            "user@example.com");

        var issuedToken = service.CreateAccessToken(identity);

        var tokenHandler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };
        var principal = tokenHandler.ValidateToken(
            issuedToken.Value,
            CreateValidationParameters(),
            out var validatedToken);
        var jwt = Assert.IsType<JwtSecurityToken>(validatedToken);
        Assert.Equal(SecurityAlgorithms.HmacSha256, jwt.Header.Alg);
        Assert.Equal(identity.UserId.ToString(), principal.FindFirstValue("sub"));
        Assert.Equal(identity.Email, principal.FindFirstValue("email"));
        Assert.False(string.IsNullOrWhiteSpace(principal.FindFirstValue("jti")));
        Assert.Equal(UtcNow.AddMinutes(15), issuedToken.ExpiresAtUtc);
    }

    [Fact]
    public void CreateRefreshToken_ProducesDistinctRandomValuesAndStoresOnlyHashes()
    {
        var service = CreateService();

        var first = service.CreateRefreshToken();
        var second = service.CreateRefreshToken();

        Assert.NotEqual(first.Value, second.Value);
        Assert.NotEqual(first.Hash, second.Hash);
        Assert.NotEqual(first.Value, first.Hash);
        Assert.DoesNotContain('=', first.Value);
        Assert.Equal(64, first.Hash.Length);
        Assert.Equal(UtcNow, first.CreatedAtUtc);
        Assert.Equal(UtcNow.AddDays(30), first.ExpiresAtUtc);
    }

    [Fact]
    public void CreateRefreshToken_TruncatesLifecycleTimesToSqlPrecision()
    {
        var fractionalUtcNow = UtcNow.AddMilliseconds(750);
        var service = CreateService(fractionalUtcNow);

        var token = service.CreateRefreshToken();

        Assert.Equal(UtcNow, token.CreatedAtUtc);
        Assert.Equal(UtcNow.AddDays(30), token.ExpiresAtUtc);
    }

    [Fact]
    public void HashRefreshToken_WithSameValue_ReturnsSameHash()
    {
        var service = CreateService();

        var firstHash = service.HashRefreshToken("synthetic-refresh-token");
        var secondHash = service.HashRefreshToken("synthetic-refresh-token");

        Assert.Equal(firstHash, secondHash);
    }

    [Fact]
    public void Constructor_WithShortSigningKey_ThrowsArgumentException()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = Issuer,
            Audience = Audience,
            SigningKey = "too-short"
        });

        Assert.Throws<ArgumentException>(
            () => new JwtSecurityTokenService(options, new FixedTimeProvider(UtcNow)));
    }

    private static JwtSecurityTokenService CreateService(
        DateTimeOffset? utcNow = null)
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = Issuer,
            Audience = Audience,
            SigningKey = SigningKey
        });

        return new JwtSecurityTokenService(
            options,
            new FixedTimeProvider(utcNow ?? UtcNow));
    }

    private static TokenValidationParameters CreateValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(SigningKey)),
            ValidateLifetime = false,
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role
        };
    }
}
