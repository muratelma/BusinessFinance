using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Pos;
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
    CardCollectionBuilder cardCollectionBuilder,
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

        try
        {
            Account? account;
            PosSettlement? cardSettlement = null;
            // Kapanmış yükümlülükte ikinci kez POS kaydı kurulmaz: Settle aynı
            // kapanışı döndürür.
            if (command.Card is CardCollectionInput card && obligation.Settlement is null)
            {
                if (obligation.Direction != DebtDirection.Receivable)
                {
                    return ApplicationResult<ObligationDto>.Failure(
                        ObligationErrors.CardRequiresReceivable);
                }

                var built = await cardCollectionBuilder.BuildAsync(
                    userId,
                    card with { AccountId = card.AccountId ?? command.AccountId },
                    obligation.Amount,
                    command.SettlementDate,
                    timeProvider.GetUtcNow().ToUniversalTime(),
                    obligation.Description,
                    cancellationToken);
                if (built.Error is ApplicationError error)
                {
                    return ApplicationResult<ObligationDto>.Failure(error);
                }

                cardSettlement = built.Settlement;
                account = built.Account;
            }
            else
            {
                account = command.AccountId is Guid accountId
                    ? await accountRepository.FindOwnedByIdAsync(
                        accountId, userId, cancellationToken)
                    : null;
                if (account is null || !account.IsActive ||
                    account.Currency != obligation.Amount.Currency)
                {
                    return ApplicationResult<ObligationDto>.Failure(
                        ObligationErrors.AccountUnavailable);
                }
            }

            obligation.Settle(
                Guid.NewGuid(),
                account!,
                command.SettlementDate,
                timeProvider.GetUtcNow(),
                cardSettlement);
            await repository.SaveSettlementAsync(cardSettlement, cancellationToken);
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
