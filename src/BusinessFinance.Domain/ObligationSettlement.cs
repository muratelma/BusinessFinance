namespace BusinessFinance.Domain;

/// <summary>
/// Bir yükümlülüğü kapatan tahsilat veya ödeme.
/// </summary>
/// <remarks>
/// <b>Taşır, tanımaz</b> (ADR 0014). Hesap bakiyesini değiştirir; ekonomik
/// olay <see cref="Obligation"/> tarafından daha önce tanındığı için kategori
/// ve kapsam taşımaz.
/// </remarks>
public sealed class ObligationSettlement
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid ObligationId { get; }
    public Guid AccountId { get; }
    public DebtDirection Direction { get; }
    public Money Amount { get; }
    public DateOnly SettlementDate { get; }
    public DateTimeOffset SettledAtUtc { get; }
    public bool IsCancelled { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private ObligationSettlement()
    {
        Amount = null!;
    }

    internal ObligationSettlement(
        Guid id,
        Guid userId,
        Guid obligationId,
        Account account,
        DebtDirection direction,
        Money amount,
        DateOnly issueDate,
        DateOnly settlementDate,
        DateTimeOffset settledAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Settlement id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty || obligationId == Guid.Empty)
        {
            throw new ArgumentException("Settlement ownership is required.");
        }

        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(amount);
        if (account.UserId != userId)
        {
            throw new ArgumentException(
                "Account must belong to the obligation user.",
                nameof(account));
        }

        if (!account.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive account cannot settle an obligation.");
        }

        if (!Enum.IsDefined(direction))
        {
            throw new ArgumentOutOfRangeException(
                nameof(direction),
                direction,
                "Settlement direction is not supported.");
        }

        if (amount.Currency != account.Currency)
        {
            throw new ArgumentException(
                "Settlement and account must use the same currency.",
                nameof(amount));
        }

        if (settledAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Settlement time must be UTC.", nameof(settledAtUtc));
        }

        if (settlementDate == default || settlementDate < issueDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(settlementDate),
                "Settlement date cannot be before the issue date.");
        }

        if (settlementDate > DateOnly.FromDateTime(settledAtUtc.UtcDateTime))
        {
            throw new ArgumentOutOfRangeException(
                nameof(settlementDate),
                "Settlement date cannot be in the future.");
        }

        Id = id;
        UserId = userId;
        ObligationId = obligationId;
        AccountId = account.Id;
        Direction = direction;
        // EF Core owned values cannot be shared by two owners. The settlement
        // carries the same monetary value, but owns its own immutable snapshot.
        Amount = new Money(amount.Amount, amount.Currency);
        SettlementDate = settlementDate;
        SettledAtUtc = settledAtUtc;
    }

    /// <summary>Tahsilat hesabı artırır, ödeme hesabı azaltır.</summary>
    public decimal SignedAccountEffect => Direction == DebtDirection.Receivable
        ? Amount.Amount
        : -Amount.Amount;

    internal void Cancel(DateTimeOffset cancelledAtUtc)
    {
        if (cancelledAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Cancellation time must be UTC.", nameof(cancelledAtUtc));
        }

        if (IsCancelled)
        {
            return;
        }

        IsCancelled = true;
        CancelledAtUtc = cancelledAtUtc;
    }
}
