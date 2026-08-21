using BusinessFinance.Domain;

namespace BusinessFinance.Application.CreditCards;

public sealed record InstallmentItemDto(
    Guid Id,
    int Sequence,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly ScheduledDate,
    bool IsRealized,
    Guid? CreditCardChargeId,
    DateTimeOffset? RealizedAtUtc);

public sealed record InstallmentPlanDto(
    Guid Id,
    Guid CreditCardId,
    Guid CategoryId,
    Guid ClientRequestId,
    decimal TotalAmount,
    CurrencyCode Currency,
    int InstallmentCount,
    DateOnly FirstInstallmentDate,
    string? Description,
    IReadOnlyList<InstallmentItemDto> Items);

public sealed record CreateInstallmentPlanCommand(
    Guid CreditCardId,
    Guid CategoryId,
    Guid ClientRequestId,
    decimal TotalAmount,
    CurrencyCode Currency,
    int InstallmentCount,
    DateOnly FirstInstallmentDate,
    string? Description);

public sealed record RealizeInstallmentCommand(Guid InstallmentPlanId, int Sequence);

public interface IInstallmentPlanRepository
{
    Task<InstallmentPlan?> FindByClientRequestIdAsync(
        Guid userId,
        Guid clientRequestId,
        bool track,
        CancellationToken cancellationToken);
    Task<InstallmentPlan?> FindOwnedByIdAsync(
        Guid installmentPlanId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<InstallmentPlan>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken);
    Task AddAsync(InstallmentPlan plan, CancellationToken cancellationToken);
    Task RealizeAsync(
        InstallmentItem item,
        CreditCardCharge charge,
        CancellationToken cancellationToken);
}
