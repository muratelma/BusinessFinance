namespace BusinessFinance.Application.Authentication.LoginUser;

public sealed record LoginUserResponse(
    Guid UserId,
    Guid SessionId,
    string Email,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);
