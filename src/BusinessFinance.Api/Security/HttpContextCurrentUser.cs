using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BusinessFinance.Application.Abstractions.Authentication;

namespace BusinessFinance.Api.Security;

public sealed class HttpContextCurrentUser(
    IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var subject = httpContextAccessor.HttpContext?.User.FindFirstValue(
                JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(subject, out var userId) && userId != Guid.Empty
                ? userId
                : null;
        }
    }
}
