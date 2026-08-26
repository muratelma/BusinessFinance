using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transactions;

public sealed record CreateTransactionCommand(
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    CurrencyCode Currency,
    TransactionType Type,

    // Kullanıcının açık seçimi. Boşsa hesabın, yoksa kategorinin varsayılanı
    // kullanılır; üçü de boşsa istek reddedilir.
    TransactionScope? Scope,
    DateOnly TransactionDate,
    string? Description,

    // Belgedeki KDV; yoksa boştur (ADR 0016). Sunucu hiçbir vergi tutarını
    // hesaplamaz — ne geldiyse o taşınır.
    VatDto? Vat = null,
    // Gider matrahtan düşülebilir mi (ADR 0016). Boşsa kategorinin varsayılanı
    // kullanılır; soru yalnız işletme kapsamlı giderde sorulur.
    bool? IsTaxDeductible = null);

public sealed class CreateTransactionUseCase(
    ICurrentUser currentUser,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    ITransactionRepository transactionRepository)
{
    public async Task<ApplicationResult<TransactionDto>> ExecuteAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TransactionDto>.Failure(TransactionErrors.AuthenticationRequired);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId,
            userId,
            cancellationToken);
        if (account is null || !account.IsActive)
        {
            return ApplicationResult<TransactionDto>.Failure(TransactionErrors.AccountUnavailable);
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId,
            userId,
            cancellationToken);
        if (category is null || !category.IsActive)
        {
            return ApplicationResult<TransactionDto>.Failure(TransactionErrors.CategoryUnavailable);
        }

        if (TransactionScopeResolution.Resolve(
                command.Scope,
                account.DefaultScope,
                category.DefaultScope) is not TransactionScope scope)
        {
            return ApplicationResult<TransactionDto>.Failure(TransactionErrors.ScopeUnresolved);
        }

        BudgetTransaction transaction;
        try
        {
            transaction = new BudgetTransaction(
                Guid.NewGuid(),
                userId,
                account,
                category,
                new Money(command.Amount, command.Currency),
                command.Type,
                scope,
                command.TransactionDate,
                command.Description,
                command.Vat?.ToDomain(),
                TaxDeductibilityResolution.Resolve(
                    command.IsTaxDeductible,
                    category.DefaultIsTaxDeductible,
                    scope,
                    command.Type == TransactionType.Expense));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<TransactionDto>.Failure(
                TransactionErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<TransactionDto>.Failure(
                TransactionErrors.Validation(exception.Message));
        }

        await transactionRepository.AddAsync(transaction, cancellationToken);
        return ApplicationResult<TransactionDto>.Success(ToDto(transaction));
    }

    internal static TransactionDto ToDto(BudgetTransaction transaction) => new(
        transaction.Id,
        transaction.AccountId,
        transaction.CategoryId,
        transaction.Amount.Amount,
        transaction.Amount.Currency,
        transaction.Type,
        transaction.Scope,
        transaction.TransactionDate,
        transaction.Description,
        transaction.IsCancelled,
        transaction.CancelledAtUtc,
        VatDto.From(transaction.Vat),
        transaction.IsTaxDeductible);
}
