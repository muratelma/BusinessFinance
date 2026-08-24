using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Obligations;

/// <summary>
/// Tek seferlik ekonomik olayı tanır; ödeme kaynağı almaz ve kasaya dokunmaz.
/// </summary>
public sealed class CreateObligationUseCase(
    ICurrentUser currentUser,
    IObligationRepository repository,
    ICategoryRepository categoryRepository,
    ICounterpartyRepository counterpartyRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<ObligationDto>> ExecuteAsync(
        CreateObligationCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.AuthenticationRequired);
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (category is null || !category.IsActive ||
            category.Type != Obligation.RequiredCategoryType(command.Direction))
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.CategoryUnavailable);
        }

        Counterparty? counterparty = null;
        if (command.CounterpartyId is Guid counterpartyId)
        {
            counterparty = await counterpartyRepository.FindOwnedByIdAsync(
                counterpartyId, userId, cancellationToken);
            if (counterparty is null || !counterparty.IsActive)
            {
                return ApplicationResult<ObligationDto>.Failure(
                    ObligationErrors.CounterpartyUnavailable);
            }
        }

        if (TransactionScopeResolution.Resolve(command.Scope, category.DefaultScope)
            is not TransactionScope scope)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.ScopeUnresolved);
        }

        try
        {
            var obligation = new Obligation(
                Guid.NewGuid(),
                userId,
                category,
                command.Direction,
                new Money(command.Amount, command.Currency),
                scope,
                command.IssueDate,
                command.DueDate,
                timeProvider.GetUtcNow().ToUniversalTime(),
                counterparty,
                command.Description);
            await repository.AddAsync(obligation, cancellationToken);
            return ApplicationResult<ObligationDto>.Success(ToDto(obligation));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.Validation(exception.Message));
        }
    }

    internal static ObligationDto ToDto(Obligation obligation) => new(
        obligation.Id,
        obligation.CounterpartyId,
        obligation.CategoryId,
        obligation.Direction,
        obligation.Amount.Amount,
        obligation.Amount.Currency,
        obligation.Scope,
        obligation.IssueDate,
        obligation.DueDate,
        obligation.Description,
        obligation.Status);
}
