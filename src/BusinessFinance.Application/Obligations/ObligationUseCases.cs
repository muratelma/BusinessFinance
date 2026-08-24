using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Obligations;

public sealed class ListObligationsUseCase(
    ICurrentUser currentUser,
    IObligationRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<ObligationDto>>> ExecuteAsync(
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<ObligationDto>>.Failure(
                ObligationErrors.AuthenticationRequired);
        }

        return ApplicationResult<IReadOnlyList<ObligationDto>>.Success(
            await repository.ListAsync(userId, asOfDate, cancellationToken));
    }
}

public sealed class SettleObligationUseCase(
    ICurrentUser currentUser,
    IObligationRepository repository,
    IAccountRepository accountRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<ObligationDto>> ExecuteAsync(
        SettleObligationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.AuthenticationRequired);
        }

        var obligation = await repository.FindOwnedByIdAsync(
            command.ObligationId, userId, true, cancellationToken);
        if (obligation is null)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.NotFound(command.ObligationId));
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive || account.Currency != obligation.Amount.Currency)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.AccountUnavailable);
        }

        try
        {
            obligation.Settle(
                Guid.NewGuid(),
                account,
                command.SettlementDate,
                timeProvider.GetUtcNow());
            await repository.SaveSettlementAsync(cancellationToken);
            return ApplicationResult<ObligationDto>.Success(ToDto(obligation));
        }
        catch (ObligationConcurrencyException)
        {
            var current = await repository.FindOwnedByIdAsync(
                command.ObligationId, userId, false, cancellationToken);
            return current?.Settlement is null
                ? ApplicationResult<ObligationDto>.Failure(
                    ObligationErrors.Conflict(
                        "The obligation changed while it was being settled."))
                : ApplicationResult<ObligationDto>.Success(ToDto(current));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.Conflict(exception.Message));
        }
    }

    private static ObligationDto ToDto(Obligation obligation) => new(
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
        obligation.Status,
        SettlementId: obligation.Settlement?.Id,
        SettlementAccountId: obligation.Settlement?.AccountId,
        SettlementDate: obligation.Settlement?.SettlementDate);
}
