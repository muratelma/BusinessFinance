namespace BusinessFinance.Application.Authentication.Tokens;

public sealed record IssuedAccessToken(
    string Value,
    DateTimeOffset ExpiresAtUtc);
