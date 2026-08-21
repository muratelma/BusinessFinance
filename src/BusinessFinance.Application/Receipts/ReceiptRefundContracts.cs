namespace BusinessFinance.Application.Receipts;

/// <summary>
/// The expense an iade fişi appears to reverse.
/// </summary>
/// <param name="RemainingAmount">
/// What the expense should become if the refund is partial; <c>null</c> when the
/// refund covers the whole thing and cancelling is the entire correction.
/// <para>
/// Computed here rather than on the client because it is money: the client
/// contract is that Flutter shows the server's numbers and never recomputes a
/// financial total. The arithmetic is the application's, not the model's — the
/// model is never asked to add or subtract anything (ADR 0011).
/// </para>
/// </param>
public sealed record ReceiptRefundMatch(
    Guid TransactionId,
    DateOnly TransactionDate,
    decimal Amount,
    string? Description,
    decimal? RemainingAmount);

/// <summary>
/// Answers one question — "which expense is this refund giving back?" — and
/// nothing else.
///
/// <para>
/// <b>Why a separate read-only port.</b> The same reason
/// <see cref="IReceiptDuplicateLookup" /> exists: <see cref="AnalyzeReceiptUseCase" />
/// deliberately owns no transaction repository, and that missing dependency is
/// what makes it structurally impossible for reading a photo to write a
/// financial record. A port that can only look keeps the guarantee.
/// </para>
/// <para>
/// <b>The match is a suggestion, never an action.</b> Nothing is cancelled here.
/// The candidate is handed to the user, who sees the record and confirms it —
/// cancelling the wrong expense silently would be worse than not finding it at
/// all, because the user would have no reason to look.
/// </para>
/// </summary>
public interface IReceiptRefundLookup
{
    /// <summary>
    /// Owner-scoped by construction. Returns <c>null</c> when the refund cannot
    /// be tied to one expense with confidence; the caller then tells the user
    /// plainly rather than inventing a cancellation.
    /// </summary>
    Task<ReceiptRefundMatch?> FindAsync(
        Guid userId,
        DateOnly? refundDate,
        decimal? refundAmount,
        string? counterpartyName,
        CancellationToken cancellationToken);
}
