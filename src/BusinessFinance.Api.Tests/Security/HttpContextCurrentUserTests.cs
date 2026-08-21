using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BusinessFinance.Api.Security;

namespace BusinessFinance.Api.Tests.Security;

public sealed class HttpContextCurrentUserTests
{
    [Fact]
    public void UserId_WithValidSubjectClaim_ReturnsGuid()
    {
        var userId = Guid.NewGuid();
        var context = CreateContext(new Claim(
            JwtRegisteredClaimNames.Sub,
            userId.ToString()));
        var currentUser = new HttpContextCurrentUser(
            new HttpContextAccessor { HttpContext = context });

        Assert.Equal(userId, currentUser.UserId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void UserId_WithoutValidSubjectClaim_ReturnsNull(string? subject)
    {
        var context = subject is null
            ? CreateContext()
            : CreateContext(new Claim(JwtRegisteredClaimNames.Sub, subject));
        var currentUser = new HttpContextCurrentUser(
            new HttpContextAccessor { HttpContext = context });

        Assert.Null(currentUser.UserId);
    }

    private static DefaultHttpContext CreateContext(params Claim[] claims)
    {
        return new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
        };
    }
}
