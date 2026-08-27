namespace BusinessFinance.Api.Features.Authentication;

// hasBusiness onboarding'in tek sorusudur ve hiçbir özelliği kapatmaz:
// yalnız hangi kategori setiyle başlanacağını ve kapsam boyutunun arayüzde
// görünüp görünmeyeceğini belirler. Gönderilmezse "işletmesi yok" sayılır.
public sealed record RegisterRequest(
    string Email,
    string Password,
    bool HasBusiness = false);

public sealed record RegisterResponse(Guid UserId, string Email);

public sealed record LoginRequest(string Email, string Password);

public sealed record TokenPairResponse(
    Guid UserId,
    Guid SessionId,
    string Email,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record RefreshTokenResponse(
    Guid SessionId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);

public sealed record LogoutRequest(string RefreshToken);
