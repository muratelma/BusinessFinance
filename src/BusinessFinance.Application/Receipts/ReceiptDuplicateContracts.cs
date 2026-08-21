namespace BusinessFinance.Application.Receipts;

/// <summary>
/// Where a matching record was found.
///
/// <para>
/// The warning has to name the kind. A bank slip can end up as any of four
/// unrelated records, and "you may have recorded this already" is unhelpful if
/// the user cannot tell which shelf to look on — they would go hunting through
/// the expense list for a card payment.
/// </para>
/// </summary>
public enum ReceiptDuplicateKind
{
    /// <summary>An income or expense movement.</summary>
    Transaction = 1,

    /// <summary>A transfer between the user's own accounts.</summary>
    Transfer = 2,

    /// <summary>A credit card payment.</summary>
    CardPayment = 3,

    /// <summary>Money lent out and expected back.</summary>
    Receivable = 4
}

/// <summary>
/// A record that already looks like the document just read.
/// </summary>
public sealed record ReceiptDuplicateMatch(
    Guid TransactionId,
    DateOnly TransactionDate,
    decimal Amount,
    string? Description,
    ReceiptDuplicateKind Kind = ReceiptDuplicateKind.Transaction);

/// <summary>
/// Answers one question — "have I already recorded this?" — and nothing else.
///
/// <para>
/// <b>Why a separate port.</b> <see cref="AnalyzeReceiptUseCase" /> deliberately
/// has no transaction repository: that negative dependency is what makes it
/// architecturally impossible for reading a photo to write a financial record.
/// Handing it the full repository to run one query would trade a structural
/// guarantee for a comment. This port can only look, so the guarantee survives.
/// </para>
/// <para>
/// <b>Why not a photo fingerprint.</b> Matching the same photo byte-for-byte was
/// the obvious design and does not work here. The stored document is the
/// <b>original</b> while analysis only ever sees the downscaled copy, so the two
/// hashes never agree; and remembering the hashes of photos that were read but
/// not kept would mean the analysis endpoint persisting state, which is the one
/// thing it must not do. The fields the user is about to confirm are the honest
/// evidence, and they work whether or not the photo was kept.
/// </para>
/// </summary>
public interface IReceiptDuplicateLookup
{
    /// <summary>
    /// Owner-scoped by construction. A null date or amount means there is
    /// nothing to match on and the lookup reports no match rather than guessing
    /// from the remaining field.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Which shelf is searched follows the intent.</b> An income document
    /// cannot duplicate an expense, and neither can duplicate a transfer. A
    /// bank slip is the one intent that searches all four, because at reading
    /// time nobody knows yet which of the four it will become — that question
    /// is asked on the decision page, after this call has already returned.
    /// </para>
    /// <para>
    /// <b>The three legs stay the same everywhere</b> (date, amount, name).
    /// Matching a transfer on date and amount alone was considered and
    /// rejected: two legitimate transfers of a round number on one day are
    /// ordinary, and a warning nobody trusts costs every later warning. Every
    /// path that writes one of these records from a slip carries the
    /// counterparty into its description, so the third leg is there to match
    /// on.
    /// </para>
    /// </remarks>
    Task<ReceiptDuplicateMatch?> FindAsync(
        Guid userId,
        DateOnly? transactionDate,
        decimal? amount,
        string? counterpartyName,
        ReceiptCaptureIntent intent,
        CancellationToken cancellationToken);
}
