using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.FinancialActivities;

public sealed record FinancialActivityResponse(
    Guid ActivityId,
    string ActivityKind,
    string Effect,
    string SourceGroup,
    string Origin,
    string Status,
    string ActivityDate,
    string Amount,
    string Currency,
    string Title,
    string? Description,
    Guid? CategoryId,
    string? CategoryName,
    Guid? SourceId,
    string? SourceName,
    Guid? DestinationId,
    string? DestinationName,
    DateTimeOffset? CancelledAtUtc,

    // Kaydın kapsamı; transfer ve kart ödemesinde boş, çünkü ikisi de
    // gelir/gider raporuna sıfır etki eder ve kapsam taşımaz.
    string? Scope,
    bool CanCancel,
    bool SupportsAttachments,

    /// <summary>
    /// Borç taksidinin anapara ve faiz payı; diğer türlerde <c>null</c>.
    /// Ayrı bir hareket değil, bu hareketin bölünmesidir:
    /// <c>principalPortion + interestPortion = amount</c>.
    /// </summary>
    string? PrincipalPortion,

    /// <inheritdoc cref="PrincipalPortion" />
    string? InterestPortion);

public sealed record FinancialActivityListResponse(
    IReadOnlyList<FinancialActivityResponse> Items,
    PaginationMetadata Pagination);
