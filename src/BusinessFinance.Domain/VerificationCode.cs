namespace BusinessFinance.Domain;

/// <summary>
/// E-postayla gönderilen tek kullanımlık, süreli doğrulama kodu.
/// </summary>
/// <remarks>
/// Kodun kendisi burada durmaz; yalnız <b>hash'i</b> saklanır — refresh
/// token'da olduğu gibi. Veritabanını okuyan biri kodu öğrenemez.
///
/// Altı haneli bir kod tek başına zayıftır (bir milyon olasılık). Onu güvenli
/// kılan üç sınır birlikte çalışır ve üçü de bu sınıfın içindedir: <b>süre</b>,
/// <b>tek kullanım</b> ve <b>deneme sayısı</b>. Sonuncusu olmadan saldırgan
/// aynı kodu sürekli tahmin edebilirdi; beşinci yanlış denemede kod ölür ve
/// kullanıcı yenisini ister.
/// </remarks>
public sealed class VerificationCode
{
    public const int CodeHashLength = 64;
    public const int MaximumFailedAttempts = 5;

    public Guid Id { get; }
    public Guid UserId { get; }
    public VerificationPurpose Purpose { get; }
    public string CodeHash { get; }
    public DateTimeOffset CreatedAtUtc { get; }
    public DateTimeOffset ExpiresAtUtc { get; }
    public DateTimeOffset? ConsumedAtUtc { get; private set; }
    public int FailedAttemptCount { get; private set; }

    public bool IsConsumed => ConsumedAtUtc is not null;

    /// <summary>
    /// Kod tükendi: doğru cevabı beklemeyi bırakır. Kullanılmış olmasıyla çok
    /// denenmiş olması arasında kullanıcı açısından fark yoktur — ikisinde de
    /// yeni kod istenir.
    /// </summary>
    public bool IsExhausted => IsConsumed || FailedAttemptCount >= MaximumFailedAttempts;

    private VerificationCode()
    {
        CodeHash = null!;
    }

    public VerificationCode(
        Guid id,
        Guid userId,
        VerificationPurpose purpose,
        string codeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Verification code id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        if (!Enum.IsDefined(purpose))
        {
            throw new ArgumentException("Verification purpose is invalid.", nameof(purpose));
        }

        if (string.IsNullOrWhiteSpace(codeHash) || codeHash.Length != CodeHashLength)
        {
            throw new ArgumentException(
                $"Code hash must be exactly {CodeHashLength} characters.",
                nameof(codeHash));
        }

        EnsureUtc(createdAtUtc, nameof(createdAtUtc));
        EnsureUtc(expiresAtUtc, nameof(expiresAtUtc));

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new ArgumentException(
                "Code expiry must be after creation time.",
                nameof(expiresAtUtc));
        }

        Id = id;
        UserId = userId;
        Purpose = purpose;
        CodeHash = codeHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public bool IsExpired(DateTimeOffset utcNow)
    {
        EnsureUtc(utcNow, nameof(utcNow));
        return utcNow >= ExpiresAtUtc;
    }

    public bool IsUsable(DateTimeOffset utcNow) => !IsExhausted && !IsExpired(utcNow);

    /// <summary>
    /// Kodu kullanılmış işaretler. İkinci çağrı ilkini değiştirmez: tek kullanım
    /// kuralı burada, tek yerde durur.
    /// </summary>
    public void Consume(DateTimeOffset consumedAtUtc)
    {
        if (IsConsumed)
        {
            return;
        }

        EnsureLifecycleTime(consumedAtUtc, nameof(consumedAtUtc));
        ConsumedAtUtc = consumedAtUtc;
    }

    public void RegisterFailedAttempt()
    {
        if (IsConsumed)
        {
            return;
        }

        FailedAttemptCount++;
    }

    private void EnsureLifecycleTime(DateTimeOffset value, string parameterName)
    {
        EnsureUtc(value, parameterName);

        if (value < CreatedAtUtc)
        {
            throw new ArgumentException(
                "Verification lifecycle time cannot be before creation time.",
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
