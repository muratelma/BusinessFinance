namespace BusinessFinance.Api.Features.Authentication;

public sealed record RegisterRequest(string Email, string Password);

public sealed record RegisterResponse(Guid UserId, string Email);

public sealed record LoginRequest(string Email, string Password);

public sealed record TokenPairResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record RefreshTokenResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);

public sealed record LogoutRequest(string RefreshToken);
