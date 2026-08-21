namespace BusinessFinance.Application.Authentication.RefreshTokens;

public sealed record RefreshTokensResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);
