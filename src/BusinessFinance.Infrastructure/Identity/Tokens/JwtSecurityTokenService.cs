using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Tokens;

namespace BusinessFinance.Infrastructure.Identity.Tokens;

public sealed class JwtSecurityTokenService : ISecurityTokenService
{
    private const int RefreshTokenByteLength = 64;
    private const long TicksPerSecond = TimeSpan.TicksPerSecond;

    private readonly JwtOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly SigningCredentials _signingCredentials;

    public JwtSecurityTokenService(
        IOptions<JwtOptions> options,
        TimeProvider timeProvider)
    {
        _options = options.Value;
        _timeProvider = timeProvider;

        if (string.IsNullOrWhiteSpace(_options.Issuer))
        {
            throw new ArgumentException("JWT issuer is required.", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(_options.Audience))
        {
            throw new ArgumentException("JWT audience is required.", nameof(options));
        }

        if (Encoding.UTF8.GetByteCount(_options.SigningKey) < 32)
        {
            throw new ArgumentException(
                "JWT signing key must contain at least 32 bytes.",
                nameof(options));
        }

        if (_options.AccessTokenLifetime <= TimeSpan.Zero ||
            _options.RefreshTokenLifetime <= TimeSpan.Zero)
        {
            throw new ArgumentException("Token lifetimes must be positive.", nameof(options));
        }

        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            SecurityAlgorithms.HmacSha256);
    }

    public IssuedAccessToken CreateAccessToken(AuthenticatedIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(identity);

        var issuedAtUtc = _timeProvider.GetUtcNow();
        var expiresAtUtc = issuedAtUtc.Add(_options.AccessTokenLifetime);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, identity.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, identity.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            issuedAtUtc.UtcDateTime,
            expiresAtUtc.UtcDateTime,
            _signingCredentials);

        return new IssuedAccessToken(
            _tokenHandler.WriteToken(token),
            expiresAtUtc);
    }

    public IssuedRefreshToken CreateRefreshToken()
    {
        var utcNow = _timeProvider.GetUtcNow();
        var createdAtUtc = utcNow.AddTicks(-(utcNow.Ticks % TicksPerSecond));
        var expiresAtUtc = createdAtUtc.Add(_options.RefreshTokenLifetime);
        var value = WebEncoders.Base64UrlEncode(
            RandomNumberGenerator.GetBytes(RefreshTokenByteLength));

        return new IssuedRefreshToken(
            value,
            HashRefreshToken(value),
            createdAtUtc,
            expiresAtUtc);
    }

    public string HashRefreshToken(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Refresh token is required.", nameof(refreshToken));
        }

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
}
