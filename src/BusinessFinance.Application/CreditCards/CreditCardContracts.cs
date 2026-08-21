using BusinessFinance.Domain;

namespace BusinessFinance.Application.CreditCards;

public sealed record CreditCardDto(
    Guid Id,
    string Name,
    decimal Limit,
    decimal CurrentDebt,
    decimal AvailableLimit,
    CurrencyCode Currency,
    int StatementClosingDay,
    int PaymentDueDay,
    decimal MinimumPaymentRate,
    bool IsActive);

public sealed record CreateCreditCardCommand(
    string Name,
    decimal Limit,
    CurrencyCode Currency,
    int StatementClosingDay,
    int PaymentDueDay,
    decimal? MinimumPaymentRate);

public sealed record UpdateCreditCardCommand(
    Guid CreditCardId,
    string Name,
    decimal Limit,
    CurrencyCode Currency,
    int StatementClosingDay,
    int PaymentDueDay,
    decimal? MinimumPaymentRate,
    bool IsActive);

public interface ICreditCardRepository
{
    Task AddAsync(CreditCard creditCard, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(
        Guid userId,
        string normalizedName,
        Guid? exceptCreditCardId,
        CancellationToken cancellationToken);
    Task<CreditCard?> FindOwnedByIdAsync(
        Guid creditCardId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<CreditCard>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken);
    Task<decimal> CalculateCurrentDebtAsync(
        Guid creditCardId,
        Guid userId,
        CancellationToken cancellationToken);
    Task UpdateOwnedAsync(
        CreditCard creditCard,
        Guid userId,
        CancellationToken cancellationToken);
}
