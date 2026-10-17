using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.DayCloses;
using BusinessFinance.Application.Pos;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Debts;

/// <summary>Borcun kendisi ve kapanışları, her planda (ADR 0021).</summary>
public sealed class GetDebtUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    ICounterpartyRepository counterpartyRepository)
{
    public async Task<ApplicationResult<DebtDto>> ExecuteAsync(
        Guid debtId,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AuthenticationRequired);
        var debt = await repository.FindAnyOwnedAsync(debtId, userId, false, cancellationToken);
        return debt is null
            ? ApplicationResult<DebtDto>.Failure(DebtErrors.NotFound(debtId))
            : ApplicationResult<DebtDto>.Success(CreateDebtUseCase.ToDto(
                debt, await DebtNames.OfAsync(counterpartyRepository, debt, userId, cancellationToken), asOfDate));
    }
}

/// <summary>
/// Borca ödeme ya da tahsilat yazar (ADR 0021 İ2, İ6, T2, T4).
/// </summary>
/// <remarks>
/// Tutar istenen kadardır, kalanı aşamaz ve en eski açık kalemden başlar.
/// Aynı istek kimliği aynı ödemeyi döner. Yer: borçta hesap ya da kredi kartı,
/// alacakta hesap ya da POS. Ödeme günü ileride olamaz (vadeden önce olabilir).
/// </remarks>
public sealed class RecordDebtPaymentUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    IAccountRepository accountRepository,
    ICreditCardRepository creditCardRepository,
    ICategoryRepository categoryRepository,
    ICounterpartyRepository counterpartyRepository,
    CardCollectionBuilder cardCollectionBuilder,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<DebtDto>> ExecuteAsync(
        RecordDebtPaymentCommand command,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AuthenticationRequired);
        var now = timeProvider.GetUtcNow().ToUniversalTime();
        if (command.PaymentDate > LocalDay.LatestAllowed(now))
            return ApplicationResult<DebtDto>.Failure(DebtErrors.DateInFuture);
        var sources = (command.AccountId is null ? 0 : 1) + (command.CreditCardId is null ? 0 : 1);
        if (command.Card is null ? sources != 1 : command.CreditCardId is not null)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.PaymentSourceInvalid);

        var debt = await repository.FindAnyOwnedAsync(command.DebtId, userId, true, cancellationToken);
        if (debt is null) return ApplicationResult<DebtDto>.Failure(DebtErrors.NotFound(command.DebtId));

        var paymentId = command.ClientRequestId is Guid requestId
            ? RequestScopedId.Create("debt-payment", userId, requestId)
            : Guid.NewGuid();
        if (debt.Payments.Any(payment => payment.Id == paymentId))
        {
            return await SuccessAsync(debt, userId, asOfDate, cancellationToken);
        }

        if ((command.CreditCardId is not null && debt.Direction != DebtDirection.Payable) ||
            (command.Card is not null && debt.Direction != DebtDirection.Receivable))
            return ApplicationResult<DebtDto>.Failure(DebtErrors.PaymentSourceInvalid);

        Account? account = null;
        CreditCard? card = null;
        PosSettlement? cardSettlement = null;
        if (command.Card is CardCollectionInput pos)
        {
            var built = await cardCollectionBuilder.BuildAsync(
                userId, pos with { AccountId = pos.AccountId ?? command.AccountId },
                new Money(command.Amount, debt.Principal.Currency), command.PaymentDate, now,
                debt.Description, cancellationToken);
            if (built.Error is ApplicationError error) return ApplicationResult<DebtDto>.Failure(error);
            cardSettlement = built.Settlement;
            account = built.Account;
        }
        else if (command.AccountId is Guid accountId)
        {
            account = await accountRepository.FindOwnedByIdAsync(accountId, userId, cancellationToken);
            if (account is null || !account.IsActive || account.Currency != debt.Principal.Currency)
                return ApplicationResult<DebtDto>.Failure(DebtErrors.AccountUnavailable);
        }
        else
        {
            card = await creditCardRepository.FindOwnedByIdAsync(
                command.CreditCardId!.Value, userId, false, cancellationToken);
            if (card is null || !card.IsActive)
                return ApplicationResult<DebtDto>.Failure(DebtErrors.CardUnavailable);
        }

        try
        {
            if (debt.BearsInterest)
            {
                // Faiz payı ödendiği gün gelir ya da gider olur; rolün kategorisi yoksa açılır.
                var category = await categoryRepository.FindOrOpenAsync(userId,
                    debt.Direction == DebtDirection.Receivable
                        ? CategoryRole.InterestIncome : CategoryRole.InterestExpense,
                    cancellationToken);
                if (!category.IsActive)
                    return ApplicationResult<DebtDto>.Failure(DebtErrors.CategoryUnavailable);
            }

            var payment = debt.Pay(
                paymentId, command.Amount, command.PaymentDate, now, account, card, cardSettlement);
            await repository.SaveChangesAsync(payment, cardSettlement, cancellationToken);
            return await SuccessAsync(debt, userId, asOfDate, cancellationToken);
        }
        catch (DebtAmountExceedsRemainingException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AmountExceedsRemaining(exception.Remaining));
        }
        catch (DebtConcurrencyException)
        {
            // Aynı istek bu sırada yazıldıysa onu döner; değilse borç değişmişti.
            var current = await repository.FindAnyOwnedAsync(command.DebtId, userId, false, cancellationToken);
            return current is not null && current.Payments.Any(payment => payment.Id == paymentId)
                ? await SuccessAsync(current, userId, asOfDate, cancellationToken)
                : ApplicationResult<DebtDto>.Failure(DebtErrors.Changed);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException) when (debt.BearsInterest)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.InterestBearing);
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Conflict(exception.Message));
        }
    }

    private async Task<ApplicationResult<DebtDto>> SuccessAsync(
        Debt debt, Guid userId, DateOnly asOfDate, CancellationToken cancellationToken) =>
        ApplicationResult<DebtDto>.Success(CreateDebtUseCase.ToDto(
            debt, await DebtNames.OfAsync(counterpartyRepository, debt, userId, cancellationToken), asOfDate));
}

/// <summary>
/// İndirim yazar: bedel düşer, gelir ya da gider borcun açıldığı gün azalır,
/// para hareket etmez (ADR 0021 İ4, T3). Aynı istek kimliği aynı indirimi döner.
/// </summary>
public sealed class ApplyDebtDiscountUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    ICounterpartyRepository counterpartyRepository,
    IDayCloseCountReader dayCloseCountReader,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<DebtDto>> ExecuteAsync(
        ApplyDebtDiscountCommand command,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AuthenticationRequired);
        var now = timeProvider.GetUtcNow().ToUniversalTime();
        if (command.DiscountDate > LocalDay.LatestAllowed(now))
            return ApplicationResult<DebtDto>.Failure(DebtErrors.DateInFuture);
        var debt = await repository.FindAnyOwnedAsync(command.DebtId, userId, true, cancellationToken);
        if (debt is null) return ApplicationResult<DebtDto>.Failure(DebtErrors.NotFound(command.DebtId));

        var discountId = command.ClientRequestId is Guid requestId
            ? RequestScopedId.Create("debt-discount", userId, requestId)
            : Guid.NewGuid();
        if (!debt.Payments.Any(payment => payment.Id == discountId))
        {
            if (debt.SourceType is not (DebtSourceType.Expense or DebtSourceType.Income) || debt.IsCarriedOver)
                return ApplicationResult<DebtDto>.Failure(DebtErrors.DiscountUnavailable);

            // Gün sonunda sayılmış satış ya da faturanın bedeli değişmez (T3).
            if (await dayCloseCountReader.IsCountedAsync(
                    userId, DayCloseRecordKind.Obligation, debt.Id, cancellationToken))
                return ApplicationResult<DebtDto>.Failure(DebtErrors.DayCloseCounted);

            try
            {
                var discount = debt.ApplyDiscount(discountId, command.Amount, command.DiscountDate, now);
                await repository.SaveChangesAsync(discount, null, cancellationToken);
            }
            catch (DebtAmountExceedsRemainingException exception)
            {
                return ApplicationResult<DebtDto>.Failure(DebtErrors.AmountExceedsRemaining(exception.Remaining));
            }
            catch (DebtConcurrencyException)
            {
                var current = await repository.FindAnyOwnedAsync(command.DebtId, userId, false, cancellationToken);
                if (current is null || !current.Payments.Any(payment => payment.Id == discountId))
                    return ApplicationResult<DebtDto>.Failure(DebtErrors.Changed);
                debt = current;
            }
            catch (ArgumentException exception)
            {
                return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
            }
            catch (InvalidOperationException) when (debt.BearsInterest)
            {
                return ApplicationResult<DebtDto>.Failure(DebtErrors.InterestBearing);
            }
            catch (InvalidOperationException exception)
            {
                return ApplicationResult<DebtDto>.Failure(DebtErrors.Conflict(exception.Message));
            }
        }

        return ApplicationResult<DebtDto>.Success(CreateDebtUseCase.ToDto(
            debt, await DebtNames.OfAsync(counterpartyRepository, debt, userId, cancellationToken), asOfDate));
    }
}

/// <summary>
/// Bir ödemeyi ya da indirimi geri alır: yalnız kendi kapattığını açar (ADR 0021
/// İ7, T2). Gün sonunda sayılmış ya da yatışa bağlanmış ödeme geri alınmaz; önce
/// o bağ geri alınır. Kartla tahsilin POS kaydı ödemeyle birlikte iptal olur.
/// Geri alınmış kayıt aynı sonucu döner.
/// </summary>
public sealed class ReverseDebtPaymentUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    ICounterpartyRepository counterpartyRepository,
    IDayCloseCountReader dayCloseCountReader,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<DebtDto>> ExecuteAsync(
        Guid debtId,
        Guid paymentId,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AuthenticationRequired);
        var debt = await repository.FindAnyOwnedAsync(debtId, userId, true, cancellationToken);
        if (debt is null) return ApplicationResult<DebtDto>.Failure(DebtErrors.NotFound(debtId));
        var payment = debt.Payments.SingleOrDefault(item => item.Id == paymentId);
        if (payment is null) return ApplicationResult<DebtDto>.Failure(DebtErrors.PaymentNotFound(paymentId));

        if (!payment.IsCancelled)
        {
            if (debt.IsCancelled)
                return ApplicationResult<DebtDto>.Failure(DebtErrors.Conflict("A cancelled debt cannot change."));
            var locked = await DebtLocks.CheckAsync(
                repository, dayCloseCountReader, userId, payment, cancellationToken);
            if (locked.Error is ApplicationError error) return ApplicationResult<DebtDto>.Failure(error);

            var now = timeProvider.GetUtcNow().ToUniversalTime();
            locked.CardSettlement?.Cancel(now);
            debt.ReversePayment(paymentId, now);
            try
            {
                await repository.SaveChangesAsync(null, null, cancellationToken);
            }
            catch (DebtConcurrencyException)
            {
                return ApplicationResult<DebtDto>.Failure(DebtErrors.Changed);
            }
        }

        return ApplicationResult<DebtDto>.Success(CreateDebtUseCase.ToDto(
            debt, await DebtNames.OfAsync(counterpartyRepository, debt, userId, cancellationToken), asOfDate));
    }
}

/// <summary>
/// Planı değiştirir (ADR 0021 İ1, T2): o anki kalan yeni takvime bölünür;
/// gelir ya da gider yazılmaz, yapılmış ödeme silinmez. Faizli borcun planı
/// değişmez (kullanıcı kararı, 16 Ekim 2026).
/// </summary>
public sealed class ChangeDebtPlanUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    ICounterpartyRepository counterpartyRepository)
{
    public async Task<ApplicationResult<DebtDto>> ExecuteAsync(
        ChangeDebtPlanCommand command,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AuthenticationRequired);
        var debt = await repository.FindAnyOwnedAsync(command.DebtId, userId, true, cancellationToken);
        if (debt is null) return ApplicationResult<DebtDto>.Failure(DebtErrors.NotFound(command.DebtId));
        if (debt.BearsInterest) return ApplicationResult<DebtDto>.Failure(DebtErrors.InterestBearing);
        if (command.PlanKind != DebtPlanKind.None && command.FirstDueDate is null)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.DueDateRequired);

        try
        {
            debt.ChangePlan(
                command.PlanKind, command.FirstDueDate,
                command.PlanKind == DebtPlanKind.Installments ? command.InstallmentCount : 1);
            await repository.SaveChangesAsync(null, null, cancellationToken);
        }
        catch (DebtConcurrencyException)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Changed);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Conflict(exception.Message));
        }

        return ApplicationResult<DebtDto>.Success(CreateDebtUseCase.ToDto(
            debt, await DebtNames.OfAsync(counterpartyRepository, debt, userId, cancellationToken), asOfDate));
    }
}

/// <summary>
/// Borcu bir bütün olarak iptal eder: açılış ve bütün kapanışları birlikte
/// (ADR 0021 İ7). Gün sonunda sayılmış ya da yatışa bağlanmış kapanışı varsa
/// reddedilir. İptal edilmiş borç aynı sonucu döner.
/// </summary>
public sealed class CancelDebtUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    ICounterpartyRepository counterpartyRepository,
    IDayCloseCountReader dayCloseCountReader,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<DebtDto>> ExecuteAsync(
        Guid debtId,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AuthenticationRequired);
        var debt = await repository.FindAnyOwnedAsync(debtId, userId, true, cancellationToken);
        if (debt is null) return ApplicationResult<DebtDto>.Failure(DebtErrors.NotFound(debtId));

        if (!debt.IsCancelled)
        {
            if (await dayCloseCountReader.IsCountedAsync(
                    userId, DayCloseRecordKind.Obligation, debt.Id, cancellationToken))
                return ApplicationResult<DebtDto>.Failure(DebtErrors.DayCloseCounted);

            var now = timeProvider.GetUtcNow().ToUniversalTime();
            foreach (var payment in debt.Payments.Where(payment => !payment.IsCancelled))
            {
                var locked = await DebtLocks.CheckAsync(
                    repository, dayCloseCountReader, userId, payment, cancellationToken);
                if (locked.Error is ApplicationError error) return ApplicationResult<DebtDto>.Failure(error);
                locked.CardSettlement?.Cancel(now);
            }

            debt.Cancel(now);
            try
            {
                await repository.SaveChangesAsync(null, null, cancellationToken);
            }
            catch (DebtConcurrencyException)
            {
                return ApplicationResult<DebtDto>.Failure(DebtErrors.Changed);
            }
        }

        return ApplicationResult<DebtDto>.Success(CreateDebtUseCase.ToDto(
            debt, await DebtNames.OfAsync(counterpartyRepository, debt, userId, cancellationToken), asOfDate));
    }
}

internal static class DebtNames
{
    public static async Task<string> OfAsync(
        ICounterpartyRepository counterpartyRepository,
        Debt debt,
        Guid userId,
        CancellationToken cancellationToken) =>
        debt.CounterpartyId is Guid counterpartyId &&
        await counterpartyRepository.FindOwnedByIdAsync(counterpartyId, userId, cancellationToken) is Counterparty person
            ? person.Name
            : string.Empty;
}

/// <summary>
/// Bir kapanışın geri alınmasını engelleyen bağlar: gün sonu sayımı ve kartla
/// tahsilin yatışı (eski yükümlülük iptaliyle aynı iki kural).
/// </summary>
internal static class DebtLocks
{
    public static async Task<(ApplicationError? Error, PosSettlement? CardSettlement)> CheckAsync(
        IDebtRepository repository,
        IDayCloseCountReader dayCloseCountReader,
        Guid userId,
        DebtPayment payment,
        CancellationToken cancellationToken)
    {
        if (await dayCloseCountReader.IsCountedAsync(
                userId, DayCloseRecordKind.ObligationSettlement, payment.Id, cancellationToken) ||
            payment.PosSettlementId is Guid countedSettlementId &&
            await dayCloseCountReader.IsCountedAsync(
                userId, DayCloseRecordKind.PosSettlement, countedSettlementId, cancellationToken))
        {
            return (DebtErrors.DayCloseCounted, null);
        }

        if (payment.PosSettlementId is not Guid cardSettlementId)
        {
            return (null, null);
        }

        var cardSettlement = await repository.FindCardSettlementAsync(cardSettlementId, userId, cancellationToken)
            ?? throw new InvalidOperationException("A card collection lost its pos settlement.");
        return cardSettlement is { IsCancelled: false, PosDepositId: not null }
            ? (DebtErrors.DepositLocked, null)
            : (null, cardSettlement);
    }
}
