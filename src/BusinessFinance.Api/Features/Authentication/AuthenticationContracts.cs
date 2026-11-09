namespace BusinessFinance.Api.Features.Authentication;

// hasBusiness onboarding'in tek sorusudur ve hiçbir özelliği kapatmaz:
// yalnız hangi kategori setiyle başlanacağını ve kapsam boyutunun arayüzde
// görünüp görünmeyeceğini belirler. Gönderilmezse "işletmesi yok" sayılır.
//
// username isteğe bağlıdır ve sondadır: gönderilmezse hesabın kullanıcı adı
// e-postası olur. Uygulama her zaman gönderir.
public sealed record RegisterRequest(
    string Email,
    string Password,
    bool HasBusiness = false,
    string? Username = null);

public sealed record RegisterResponse(Guid UserId, string Email);

// email alanı e-posta ya da kullanıcı adı taşır: içinde "@" yoksa kullanıcı
// adıyla aranır. Ad sözleşmeyi kırmamak için değişmedi (8 Kasım 2026).
public sealed record LoginRequest(string Email, string Password);

public sealed record UsernameAvailabilityResponse(string Username, bool Available);

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

// Google ile giriş. İlk istek yalnız idToken taşır; hesap yoksa cevap
// "signup-required" olur ve "Bir adım kaldı"daki Devam et aynı belirteci
// kullanıcı adı ve işletme cevabıyla yeniden yollar.
public sealed record GoogleSignInRequest(
    string IdToken,
    string? Username = null,
    bool HasBusiness = false);

public sealed record GoogleSignInResponse(
    string Status,
    TokenPairResponse? Session,
    GoogleSignupResponse? Signup);

public sealed record GoogleSignupResponse(string Email, string? SuggestedUsername);
