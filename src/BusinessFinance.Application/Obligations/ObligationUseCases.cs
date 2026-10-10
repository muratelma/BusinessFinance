using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.DayCloses;
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
        obligation.Status,
        SettlementId: obligation.Settlement?.Id,
        SettlementAccountId: obligation.Settlement?.AccountId,
        SettlementDate: obligation.Settlement?.SettlementDate);
}

/// <summary>
/// Yanlış yazılmış yükümlülüğü iptal eder: silme yerine iptal.
/// </summary>
/// <remarks>
/// <para>
/// İptal bir <b>bütündür</b>: kapanmış yükümlülük iptal edilirse kapanışı da
/// aynı yazmada iptal olur (para hesaba geri döner ya da hesaptan düşer) ve
/// alacak kartla tahsil edildiyse yoldaki POS kaydı da iptal olur. Kapanışın
/// tek başına geri alınması yoktur; düzeltme, kaydı iptal edip doğrusunu
/// yeniden yazmaktır.
/// </para>
/// <para>
/// İki durumda reddedilir ve önce o işlem geri alınır: kartla tahsilin parası
/// bir yatışla hesaba geçtiyse (yatış başka tahsilatları da kapatmış
/// olabilir) ve kapanış bir gün sonunda sayıldıysa (gün sonu onu satıştan
/// düşmüştür). Cari tahsilatın iptaliyle aynı iki kural.
/// </para>
/// <para>
/// Çağrı idempotenttir: iptal edilmiş kayıt aynı sonucu döner.
/// </para>
/// </remarks>
public sealed class CancelObligationUseCase(
    ICurrentUser currentUser,
    IObligationRepository repository,
    IDayCloseCountReader dayCloseCountReader,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<ObligationDto>> ExecuteAsync(
        Guid obligationId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.AuthenticationRequired);
        }

        var obligation = await repository.FindOwnedByIdAsync(
            obligationId, userId, true, cancellationToken);
        if (obligation is null)
        {
            return ApplicationResult<ObligationDto>.Failure(
                ObligationErrors.NotFound(obligationId));
        }

        if (obligation.IsCancelled)
        {
            return ApplicationResult<ObligationDto>.Success(
                SettleObligationUseCase.ToDto(obligation));
        }

        // Faturanın kendisi bir gün sonunda sayıldıysa (nakit gibi geçirilmiş
        // satış olarak düşüldüyse) kapanışı sayılmamış olsa da iptal edilemez.
        if (await dayCloseCountReader.IsCountedAsync(
                userId, DayCloseRecordKind.Obligation, obligation.Id, cancellationToken))
        {
            return ApplicationResult<ObligationDto>.Failure(ObligationErrors.DayCloseCounted);
        }

        var cancelledAtUtc = timeProvider.GetUtcNow().ToUniversalTime();
        if (obligation.Settlement is { IsCancelled: false } settlement)
        {
            if (await dayCloseCountReader.IsCountedAsync(
                    userId, DayCloseRecordKind.ObligationSettlement, settlement.Id,
                    cancellationToken) ||
                settlement.PosSettlementId is Guid countedSettlementId &&
                await dayCloseCountReader.IsCountedAsync(
                    userId, DayCloseRecordKind.PosSettlement, countedSettlementId,
                    cancellationToken))
            {
                return ApplicationResult<ObligationDto>.Failure(
                    ObligationErrors.DayCloseCounted);
            }

            if (settlement.PosSettlementId is Guid cardSettlementId)
            {
                var cardSettlement = await repository.FindCardSettlementAsync(
                    cardSettlementId, userId, cancellationToken)
                    ?? throw new InvalidOperationException(
                        "A card collection lost its pos settlement.");
                if (cardSettlement is { IsCancelled: false, PosDepositId: not null })
                {
                    return ApplicationResult<ObligationDto>.Failure(
                        ObligationErrors.DepositLocked);
                }

                cardSettlement.Cancel(cancelledAtUtc);
            }
        }

        obligation.Cancel(cancelledAtUtc);
        if (!await repository.TrySaveCancellationAsync(cancellationToken))
        {
            return ApplicationResult<ObligationDto>.Failure(ObligationErrors.Changed);
        }

        return ApplicationResult<ObligationDto>.Success(
            SettleObligationUseCase.ToDto(obligation));
    }
}
