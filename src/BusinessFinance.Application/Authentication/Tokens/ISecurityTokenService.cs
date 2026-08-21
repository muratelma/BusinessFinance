namespace BusinessFinance.Application.Authentication.Tokens;

public interface ISecurityTokenService
{
    IssuedAccessToken CreateAccessToken(AuthenticatedIdentity identity);
    IssuedRefreshToken CreateRefreshToken();
    string HashRefreshToken(string refreshToken);
}
