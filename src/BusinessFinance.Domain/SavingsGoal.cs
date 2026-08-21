namespace BusinessFinance.Domain;

public sealed class SavingsGoal
{
    public const int MaximumNameLength = 120;
    public const int MaximumDescriptionLength = 500;
    private readonly List<SavingsGoalContribution> _contributions = [];

    public Guid Id { get; }
    public Guid UserId { get; }
    public string Name { get; }
    public Money TargetAmount { get; }
    public DateOnly TargetDate { get; }
    public SavingsGoalTrackingMode TrackingMode { get; }
    public Guid? AccountId { get; }
    public string? Description { get; }
    public DateTimeOffset CreatedAtUtc { get; }
    public IReadOnlyCollection<SavingsGoalContribution> Contributions => _contributions.AsReadOnly();

    private SavingsGoal()
    {
        Name = null!;
        TargetAmount = null!;
    }

    public SavingsGoal(
        Guid id,
        Guid userId,
        string name,
        Money targetAmount,
        DateOnly targetDate,
        SavingsGoalTrackingMode trackingMode,
        Guid? accountId,
        DateTimeOffset createdAtUtc,
        string? description = null)
    {
        if (id == Guid.Empty) throw new ArgumentException("Goal id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        var normalizedName = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName) || normalizedName.Length > MaximumNameLength)
            throw new ArgumentException($"Goal name is required and cannot exceed {MaximumNameLength} characters.", nameof(name));
        ArgumentNullException.ThrowIfNull(targetAmount);
        if (!Enum.IsDefined(trackingMode)) throw new ArgumentOutOfRangeException(nameof(trackingMode));
        if (createdAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        if (targetDate < DateOnly.FromDateTime(createdAtUtc.UtcDateTime))
            throw new ArgumentOutOfRangeException(nameof(targetDate), "Target date cannot be before the creation date.");
        if ((trackingMode == SavingsGoalTrackingMode.AccountBalance) != accountId.HasValue || accountId == Guid.Empty)
            throw new ArgumentException("Account-balance goals require one account; manual goals cannot have one.", nameof(accountId));
        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalizedDescription?.Length > MaximumDescriptionLength)
            throw new ArgumentException($"Description cannot exceed {MaximumDescriptionLength} characters.", nameof(description));

        Id = id;
        UserId = userId;
        Name = normalizedName;
        TargetAmount = targetAmount;
        TargetDate = targetDate;
        TrackingMode = trackingMode;
        AccountId = accountId;
        CreatedAtUtc = createdAtUtc;
        Description = normalizedDescription;
    }

    public SavingsGoalContribution AddContribution(
        Guid id,
        Money amount,
        DateOnly contributionDate,
        Guid clientRequestId,
        DateTimeOffset createdAtUtc,
        string? note = null)
    {
        if (TrackingMode != SavingsGoalTrackingMode.ManualContributions)
            throw new InvalidOperationException("Account-balance goals cannot accept manual contributions.");
        ArgumentNullException.ThrowIfNull(amount);
        if (amount.Currency != TargetAmount.Currency)
            throw new ArgumentException("Contribution currency must match the goal.", nameof(amount));
        var contribution = new SavingsGoalContribution(
            id, UserId, Id, amount, contributionDate, clientRequestId, createdAtUtc, note);
        _contributions.Add(contribution);
        return contribution;
    }
}

public sealed class SavingsGoalContribution
{
    public const int MaximumNoteLength = 500;

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid SavingsGoalId { get; }
    public Money Amount { get; }
    public DateOnly ContributionDate { get; }
    public Guid ClientRequestId { get; }
    public DateTimeOffset CreatedAtUtc { get; }
    public string? Note { get; }

    private SavingsGoalContribution() { Amount = null!; }

    internal SavingsGoalContribution(
        Guid id,
        Guid userId,
        Guid savingsGoalId,
        Money amount,
        DateOnly contributionDate,
        Guid clientRequestId,
        DateTimeOffset createdAtUtc,
        string? note)
    {
        if (id == Guid.Empty) throw new ArgumentException("Contribution id cannot be empty.", nameof(id));
        if (userId == Guid.Empty || savingsGoalId == Guid.Empty)
            throw new ArgumentException("Contribution ownership is required.");
        ArgumentNullException.ThrowIfNull(amount);
        if (contributionDate == default) throw new ArgumentOutOfRangeException(nameof(contributionDate));
        if (clientRequestId == Guid.Empty)
            throw new ArgumentException("Client request id cannot be empty.", nameof(clientRequestId));
        if (createdAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));
        if (contributionDate > DateOnly.FromDateTime(createdAtUtc.UtcDateTime))
            throw new ArgumentOutOfRangeException(nameof(contributionDate), "Contribution date cannot be in the future.");
        var normalizedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (normalizedNote?.Length > MaximumNoteLength)
            throw new ArgumentException($"Note cannot exceed {MaximumNoteLength} characters.", nameof(note));

        Id = id;
        UserId = userId;
        SavingsGoalId = savingsGoalId;
        Amount = amount;
        ContributionDate = contributionDate;
        ClientRequestId = clientRequestId;
        CreatedAtUtc = createdAtUtc;
        Note = normalizedNote;
    }
}
