using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Verification;

/// <summary>
/// Doğrulama akışının ortak test ikizleri.
/// </summary>
/// <remarks>
/// Kod üretimi testte tahmin edilebilir olmalı — üretilen kodu bilmeden
/// doğrulama akışını uçtan uca denemek mümkün olmaz. Hash de gerçek SHA-256
/// olmak zorunda değil: test edilen şey kodun eşleşip eşleşmediği, hangi
/// algoritmayla eşleştiği değil.
/// </remarks>
internal sealed class FakeVerificationCodeService : IVerificationCodeService
{
    private int _next;

    /// <summary>Sırayla üretilen kodlar; testin bildiği değerler.</summary>
    public List<string> IssuedCodes { get; } = [];

    public string CreateCode()
    {
        var code = (100_000 + _next++).ToString();
        IssuedCodes.Add(code);
        return code;
    }

    public string HashCode(string code) => $"hash-of-{code}".PadRight(64, '.');
}

internal sealed class InMemoryVerificationCodeRepository : IVerificationCodeRepository
{
    public List<VerificationCode> Codes { get; } = [];

    public Task AddAsync(VerificationCode code, CancellationToken cancellationToken)
    {
        Codes.Add(code);
        return Task.CompletedTask;
    }

    public Task<VerificationCode?> FindLatestAsync(
        Guid userId,
        VerificationPurpose purpose,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Codes
            .Where(code => code.UserId == userId && code.Purpose == purpose)
            .OrderByDescending(code => code.CreatedAtUtc)
            .FirstOrDefault());
    }

    public Task UpdateAsync(VerificationCode code, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task ConsumeAllAsync(
        Guid userId,
        VerificationPurpose purpose,
        DateTimeOffset consumedAtUtc,
        CancellationToken cancellationToken)
    {
        foreach (var code in Codes.Where(code =>
            code.UserId == userId && code.Purpose == purpose && !code.IsConsumed))
        {
            code.Consume(consumedAtUtc);
        }

        return Task.CompletedTask;
    }
}

internal sealed class RecordingVerificationEmailSender : IVerificationEmailSender
{
    public EmailDeliveryStatus Status { get; set; } = EmailDeliveryStatus.Sent;

    public List<(string Email, VerificationPurpose Purpose, string Code)> Sent { get; } = [];

    public Task<EmailDeliveryStatus> SendCodeAsync(
        string email,
        VerificationPurpose purpose,
        string code,
        DateTimeOffset expiresAtUtc,
        CancellationToken cancellationToken)
    {
        Sent.Add((email, purpose, code));
        return Task.FromResult(Status);
    }
}

internal sealed class MutableTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    private DateTimeOffset _utcNow = utcNow;

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public void Advance(TimeSpan amount) => _utcNow = _utcNow.Add(amount);
}
