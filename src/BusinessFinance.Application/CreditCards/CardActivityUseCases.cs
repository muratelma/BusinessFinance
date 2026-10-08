using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.CreditCards;

public sealed class CreateCardChargeUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository cardRepository,
    ICategoryRepository categoryRepository,
    ICardChargeRepository chargeRepository,
    IUserProfileRepository profileRepository)
{
    public async Task<ApplicationResult<CardChargeDto>> ExecuteAsync(
        CreateCardChargeCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CardChargeDto>.Failure(CreditCardErrors.AuthenticationRequired);
        }

        var card = await cardRepository.FindOwnedByIdAsync(
            command.CreditCardId, userId, false, cancellationToken);
        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (card is null)
        {
            return ApplicationResult<CardChargeDto>.Failure(
                CreditCardErrors.NotFound(command.CreditCardId));
        }

        if (category is null)
        {
            return ApplicationResult<CardChargeDto>.Failure(
                CreditCardErrors.Validation("An active expense category is required."));
        }

        var currentDebt = await cardRepository.CalculateCurrentDebtAsync(
            card.Id, userId, cancellationToken);
        if (command.Amount > card.Limit.Amount - currentDebt)
        {
            return ApplicationResult<CardChargeDto>.Failure(CreditCardErrors.LimitExceeded);
        }

        var resolution = await TransactionScopeResolution.ResolveAsync(
            command.Scope,
            category.DefaultScope,
            card.DefaultScope,
            profileRepository,
            userId,
            cancellationToken);
        if (resolution.Scope is not TransactionScope scope)
        {
            return ApplicationResult<CardChargeDto>.Failure(resolution.ToError(
                CreditCardErrors.ScopeUnresolved, CreditCardErrors.ScopeConflict));
        }

        try
        {
            var charge = new CreditCardCharge(
                Guid.NewGuid(),
                userId,
                card,
                category,
                new Money(command.Amount, command.Currency),
                scope,
                command.ChargeDate,
                command.Description);
            await chargeRepository.AddAsync(charge, cancellationToken);
            return ApplicationResult<CardChargeDto>.Success(ToDto(charge));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CardChargeDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<CardChargeDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }
    }

    internal static CardChargeDto ToDto(CreditCardCharge charge) => new(
        charge.Id,
        charge.CreditCardId,
        charge.CategoryId,
        charge.Amount.Amount,
        charge.Amount.Currency,
        charge.Scope,
        charge.ChargeDate,
        charge.Description,
        charge.IsCancelled,
        charge.CancelledAtUtc);
}

public sealed class CreateCardPaymentUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository cardRepository,
    IAccountRepository accountRepository,
    ICardPaymentRepository paymentRepository)
{
    public async Task<ApplicationResult<CardPaymentDto>> ExecuteAsync(
        CreateCardPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CardPaymentDto>.Failure(CreditCardErrors.AuthenticationRequired);
        }

        var card = await cardRepository.FindOwnedByIdAsync(
            command.CreditCardId, userId, false, cancellationToken);
        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId, userId, cancellationToken);
        if (card is null)
        {
            return ApplicationResult<CardPaymentDto>.Failure(
                CreditCardErrors.NotFound(command.CreditCardId));
        }

        if (account is null)
        {
            return ApplicationResult<CardPaymentDto>.Failure(
                CreditCardErrors.Validation("An active owned account is required."));
        }

        var currentDebt = await cardRepository.CalculateCurrentDebtAsync(
            card.Id, userId, cancellationToken);
        if (command.Amount > currentDebt)
        {
            return ApplicationResult<CardPaymentDto>.Failure(CreditCardErrors.PaymentExceedsDebt);
        }

        try
        {
            var payment = new CreditCardPayment(
                Guid.NewGuid(),
                userId,
                account,
                card,
                new Money(command.Amount, command.Currency),
                command.PaymentDate,
                command.Description);
            await paymentRepository.AddAsync(payment, cancellationToken);
            return ApplicationResult<CardPaymentDto>.Success(ToDto(payment));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CardPaymentDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<CardPaymentDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }
    }

    internal static CardPaymentDto ToDto(CreditCardPayment payment) => new(
        payment.Id,
        payment.CreditCardId,
        payment.AccountId,
        payment.Amount.Amount,
        payment.Amount.Currency,
        payment.PaymentDate,
        payment.Description,
        payment.IsCancelled,
        payment.CancelledAtUtc);
}

public sealed class ListCardActivityUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository cardRepository,
    ICardChargeRepository chargeRepository,
    ICardPaymentRepository paymentRepository)
{
    public async Task<ApplicationResult<CardActivityDto>> ExecuteAsync(
        Guid creditCardId,
        HistoryWindow window,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(window);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CardActivityDto>.Failure(
                CreditCardErrors.AuthenticationRequired);
        }

        var card = await cardRepository.FindOwnedByIdAsync(
            creditCardId, userId, false, cancellationToken);
        if (card is null)
        {
            return ApplicationResult<CardActivityDto>.Failure(
                CreditCardErrors.NotFound(creditCardId));
        }

        var charges = await chargeRepository.ListAsync(creditCardId, userId, window, cancellationToken);
        var payments = await paymentRepository.ListAsync(creditCardId, userId, window, cancellationToken);

        // İki listeden biri kırpıldıysa ekran "hepsi bu" diyemez; tek bayrak
        // yeterli çünkü kullanıcının yapacağı şey aynı: pencereyi daraltmak.
        return ApplicationResult<CardActivityDto>.Success(new CardActivityDto(
            charges.Items.Select(CreateCardChargeUseCase.ToDto).ToArray(),
            payments.Items.Select(CreateCardPaymentUseCase.ToDto).ToArray(),
            charges.HasMore || payments.HasMore));
    }
}

public sealed class CancelCardChargeUseCase(
    ICurrentUser currentUser,
    ICardChargeRepository repository,
    IActivityOriginReader originReader,
    IRecurringTransactionRepository recurringRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CardChargeDto>> ExecuteAsync(
        Guid chargeId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CardChargeDto>.Failure(CreditCardErrors.AuthenticationRequired);
        }

        var charge = await repository.FindOwnedByIdAsync(chargeId, userId, true, cancellationToken);
        if (charge is null)
        {
            return ApplicationResult<CardChargeDto>.Failure(CreditCardErrors.ChargeNotFound(chargeId));
        }

        // A charge produced by a recurring occurrence or an installment item cannot be
        // cancelled: each holds exactly one result id with no way back, so cancelling
        // the charge would strand the source in a realized state.
        var origin = await originReader.GetCardChargeOriginAsync(userId, charge.Id, cancellationToken);
        if (!FinancialActivityCapabilities.CanCancel(
                FinancialActivityKind.CardCharge,
                origin,
                FinancialActivityStatus.Realized))
        {
            return ApplicationResult<CardChargeDto>.Failure(CreditCardErrors.CancelOriginLocked);
        }

        // Kartla ödenmiş bir vergi, kapattığı kalemlerle birlikte geri alınır
        // (ADR 0018 İ7); tek SaveChanges ikisini birlikte yazar.
        var closed = await recurringRepository.ListClosedByAsync(
            userId, null, charge.Id, cancellationToken);
        charge.Cancel(timeProvider.GetUtcNow());
        foreach (var occurrence in closed) occurrence.Reopen();
        await repository.UpdateOwnedAsync(charge, userId, cancellationToken);
        return ApplicationResult<CardChargeDto>.Success(CreateCardChargeUseCase.ToDto(charge));
    }
}

public sealed class CancelCardPaymentUseCase(
    ICurrentUser currentUser,
    ICardPaymentRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CardPaymentDto>> ExecuteAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CardPaymentDto>.Failure(CreditCardErrors.AuthenticationRequired);
        }

        var payment = await repository.FindOwnedByIdAsync(paymentId, userId, true, cancellationToken);
        if (payment is null)
        {
            return ApplicationResult<CardPaymentDto>.Failure(CreditCardErrors.PaymentNotFound(paymentId));
        }

        payment.Cancel(timeProvider.GetUtcNow());
        await repository.UpdateOwnedAsync(payment, userId, cancellationToken);
        return ApplicationResult<CardPaymentDto>.Success(CreateCardPaymentUseCase.ToDto(payment));
    }
}
