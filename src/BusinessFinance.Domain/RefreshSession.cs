namespace BusinessFinance.Domain;

public sealed class RefreshSession
{
    public const int MaximumTokenHashLength = 128;

    public Guid Id { get; }
    public Guid UserId { get; }
    public string TokenHash { get; }
    public DateTimeOffset CreatedAtUtc { get; }
    public DateTimeOffset ExpiresAtUtc { get; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }
    public Guid? ReplacedBySessionId { get; private set; }
    public DateTimeOffset? ReuseDetectedAtUtc { get; private set; }

    public bool IsRevoked => RevokedAtUtc is not null;

    private RefreshSession()
    {
        TokenHash = null!;
    }

    public RefreshSession(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Session id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException("Token hash is required.", nameof(tokenHash));
        }

        if (tokenHash.Length > MaximumTokenHashLength)
        {
            throw new ArgumentException(
                $"Token hash cannot exceed {MaximumTokenHashLength} characters.",
                nameof(tokenHash));
        }

        EnsureUtc(createdAtUtc, nameof(createdAtUtc));
        EnsureUtc(expiresAtUtc, nameof(expiresAtUtc));

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new ArgumentException(
                "Session expiry must be after creation time.",
                nameof(expiresAtUtc));
        }

        Id = id;
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public bool IsExpired(DateTimeOffset utcNow)
    {
        EnsureUtc(utcNow, nameof(utcNow));
        return utcNow >= ExpiresAtUtc;
    }

    public void Rotate(Guid replacementSessionId, DateTimeOffset revokedAtUtc)
    {
        if (replacementSessionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Replacement session id cannot be empty.",
                nameof(replacementSessionId));
        }

        if (IsRevoked)
        {
            throw new InvalidOperationException("Session is already revoked.");
        }

        EnsureLifecycleTime(revokedAtUtc, nameof(revokedAtUtc));
        RevokedAtUtc = revokedAtUtc;
        ReplacedBySessionId = replacementSessionId;
    }

    public void Revoke(DateTimeOffset revokedAtUtc)
    {
        if (IsRevoked)
        {
            return;
        }

        EnsureLifecycleTime(revokedAtUtc, nameof(revokedAtUtc));
        RevokedAtUtc = revokedAtUtc;
    }

    public void MarkReuseDetected(DateTimeOffset detectedAtUtc)
    {
        EnsureLifecycleTime(detectedAtUtc, nameof(detectedAtUtc));
        ReuseDetectedAtUtc = detectedAtUtc;
        RevokedAtUtc ??= detectedAtUtc;
    }

    private void EnsureLifecycleTime(DateTimeOffset value, string parameterName)
    {
        EnsureUtc(value, parameterName);

        if (value < CreatedAtUtc)
        {
            throw new ArgumentException(
                "Session lifecycle time cannot be before creation time.",
                parameterName);
        }
    }

    private static void EnsureUtc(DateTimeOffset value, string parameterName)
    {
        if (value.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Time must use the UTC offset.", parameterName);
        }
    }
}
