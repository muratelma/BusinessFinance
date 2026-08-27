namespace BusinessFinance.Application.Authentication.RefreshTokens;

public sealed record RefreshTokensResponse(
    Guid SessionId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);
