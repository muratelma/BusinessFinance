namespace BusinessFinance.Application.Authentication.Tokens;

public sealed record IssuedRefreshToken(
    string Value,
    string Hash,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc);
