using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.RecurringTransactions;

public sealed class CreateRecurringTransactionUseCase(
    ICurrentUser currentUser,
    IAccountRepository accountRepository,
    ICreditCardRepository cardRepository,
    ICategoryRepository categoryRepository,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<RecurringTransactionDto>> ExecuteAsync(
        CreateRecurringTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.AuthenticationRequired);
        }

        var isCardSource = command.SourceType == RecurringSourceType.CreditCard;
        if (isCardSource
            ? command.CreditCardId is null || command.AccountId is not null
            : command.AccountId is null || command.CreditCardId is not null)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.InvalidSource);
        }

        if (isCardSource && command.Kind == RecurringTransactionKind.Income)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.IncomeCardSourceNotSupported);
        }

        Account? account = null;
        CreditCard? card = null;
        if (isCardSource)
        {
            card = await cardRepository.FindOwnedByIdAsync(
                command.CreditCardId!.Value, userId, false, cancellationToken);
            if (card is null || !card.IsActive)
            {
                return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.CardUnavailable);
            }
        }
        else
        {
            account = await accountRepository.FindOwnedByIdAsync(
                command.AccountId!.Value, userId, cancellationToken);
            if (account is null || !account.IsActive)
            {
                return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.AccountUnavailable);
            }
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (category is null || !category.IsActive)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.CategoryUnavailable);
        }

        if (TransactionScopeResolution.Resolve(
                command.Scope,
                isCardSource ? card!.DefaultScope : account!.DefaultScope,
                category.DefaultScope) is not TransactionScope scope)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.ScopeUnresolved);
        }

        try
        {
            var amount = new Money(command.Amount, command.Currency);
            var recurring = isCardSource
                ? new RecurringTransaction(
                    Guid.NewGuid(),
                    userId,
                    card!,
                    category,
                    amount,
                    command.Kind,
                    scope,
                    command.Frequency,
                    command.StartDate,
                    command.EndDate,
                    command.MonthEndBehavior,
                    command.Description,
                    command.OccurrenceLimit)
                : new RecurringTransaction(
                    Guid.NewGuid(),
                    userId,
                    account!,
                    category,
                    amount,
                    command.Kind,
                    scope,
                    command.Frequency,
                    command.StartDate,
                    command.EndDate,
                    command.MonthEndBehavior,
                    command.Description,
                    command.OccurrenceLimit);
            await repository.AddAsync(recurring, cancellationToken);
            return ApplicationResult<RecurringTransactionDto>.Success(ToDto(recurring));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
    }

    internal static RecurringTransactionDto ToDto(RecurringTransaction recurring) => new(
        recurring.Id,
        recurring.SourceType,
        recurring.AccountId,
        recurring.CreditCardId,
        recurring.CategoryId,
        recurring.Amount.Amount,
        recurring.Amount.Currency,
        recurring.Kind,
        recurring.Scope,
        recurring.Frequency,
        recurring.StartDate,
        recurring.EndDate,
        recurring.OccurrenceLimit,
        recurring.GeneratedOccurrenceCount,
        recurring.NextOccurrenceDate,
        recurring.MonthEndBehavior,
        recurring.Description,
        recurring.IsActive);
}

public sealed class ListRecurringTransactionsUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<RecurringTransactionDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        var recurring = await repository.ListAsync(userId, cancellationToken);
        return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Success(
            recurring.Select(CreateRecurringTransactionUseCase.ToDto).ToArray());
    }
}

public sealed class SetRecurringActiveUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<RecurringTransactionDto>> ExecuteAsync(
        SetRecurringActiveCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.AuthenticationRequired);
        }

        var recurring = await repository.FindOwnedByIdAsync(
            command.RecurringTransactionId, userId, true, cancellationToken);
        if (recurring is null)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.NotFound(command.RecurringTransactionId));
        }

        try
        {
            if (command.IsActive) recurring.Activate();
            else recurring.Deactivate();
            await repository.UpdateAsync(recurring, cancellationToken);
            return ApplicationResult<RecurringTransactionDto>.Success(
                CreateRecurringTransactionUseCase.ToDto(recurring));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
    }
}

public sealed class GenerateRecurringOccurrencesUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public const int MaximumBatchSize = 1000;

    public async Task<ApplicationResult<GenerateRecurringOccurrencesResult>> ExecuteAsync(
        GenerateRecurringOccurrencesCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<GenerateRecurringOccurrencesResult>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        if (command.ThroughDate == default)
        {
            return ApplicationResult<GenerateRecurringOccurrencesResult>.Failure(
                RecurringErrors.Validation("Through date is required."));
        }

        return await GenerateOnceAsync(userId, command.ThroughDate, true, cancellationToken);
    }

    private async Task<ApplicationResult<GenerateRecurringOccurrencesResult>> GenerateOnceAsync(
        Guid userId,
        DateOnly throughDate,
        bool allowRetry,
        CancellationToken cancellationToken)
    {
        var schedules = await repository.ListDueAsync(userId, throughDate, cancellationToken);
        var generated = new List<RecurringTransactionOccurrence>();
        var hasMoreDue = false;

        foreach (var schedule in schedules)
        {
            while (schedule.IsActive &&
                   schedule.NextOccurrenceDate is DateOnly nextDate &&
                   nextDate <= throughDate)
            {
                if (generated.Count == MaximumBatchSize)
                {
                    hasMoreDue = true;
                    break;
                }

                generated.Add(RecurringTransactionOccurrence.Create(
                    Guid.NewGuid(), schedule, nextDate));
                schedule.AdvanceAfter(nextDate);
            }

            if (hasMoreDue) break;
        }

        if (generated.Count > 0 && !await repository.TrySaveGeneratedAsync(generated, cancellationToken))
        {
            if (allowRetry)
            {
                return await GenerateOnceAsync(userId, throughDate, false, cancellationToken);
            }

            return ApplicationResult<GenerateRecurringOccurrencesResult>.Failure(
                RecurringErrors.Validation("Occurrence generation conflicted with another request."));
        }

        return ApplicationResult<GenerateRecurringOccurrencesResult>.Success(new(
            generated.Select(ToDto).ToArray(),
            hasMoreDue));
    }

    internal static RecurringOccurrenceDto ToDto(RecurringTransactionOccurrence occurrence) => new(
        occurrence.Id,
        occurrence.RecurringTransactionId,
        occurrence.OccurrenceKey,
        occurrence.SourceType,
        occurrence.AccountId,
        occurrence.CreditCardId,
        occurrence.CategoryId,
        occurrence.Amount.Amount,
        occurrence.Amount.Currency,
        occurrence.Kind,
        occurrence.Scope,
        occurrence.ScheduledDate,
        occurrence.Description,
        occurrence.Status,
        occurrence.BudgetTransactionId,
        occurrence.CreditCardChargeId,
        occurrence.RealizedAtUtc);
}

public sealed class ListRecurringOccurrencesUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<RecurringOccurrenceDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<RecurringOccurrenceDto>>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        var occurrences = await repository.ListOccurrencesAsync(userId, cancellationToken);
        return ApplicationResult<IReadOnlyList<RecurringOccurrenceDto>>.Success(
            occurrences.Select(GenerateRecurringOccurrencesUseCase.ToDto).ToArray());
    }
}

public sealed class RealizeRecurringOccurrenceUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository,
    IAccountRepository accountRepository,
    ICreditCardRepository cardRepository,
    ICategoryRepository categoryRepository,
    ITransactionRepository transactionRepository,
    ICardChargeRepository chargeRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<RealizeRecurringOccurrenceResult>> ExecuteAsync(
        RealizeRecurringOccurrenceCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        var occurrence = await repository.FindOccurrenceOwnedByIdAsync(
            command.OccurrenceId, userId, true, cancellationToken);
        if (occurrence is null)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.OccurrenceNotFound(command.OccurrenceId));
        }

        // Checked before either branch, but after the already-realized shortcut
        // inside them stays reachable: switching a plan off must not rewrite the
        // history it already produced.
        if (occurrence.Status == RecurringOccurrenceStatus.Planned &&
            !await repository.IsScheduleActiveAsync(
                occurrence.RecurringTransactionId, userId, cancellationToken))
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.ScheduleInactive);
        }

        // Tutar düzeltmesi gerçekleşmeden önce yapılır ve yalnız bekleyen
        // occurrence'a uygulanır: gerçekleşmiş olan geçmiştir ve kısa devre
        // aşağıda aynı sonucu geri verir.
        if (command.Amount is decimal correctedAmount &&
            occurrence.Status == RecurringOccurrenceStatus.Planned)
        {
            try
            {
                occurrence.CorrectAmount(
                    new Money(correctedAmount, occurrence.Amount.Currency));
            }
            catch (ArgumentException exception)
            {
                return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                    RecurringErrors.Validation(exception.Message));
            }
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            occurrence.CategoryId, userId, cancellationToken);

        return occurrence.SourceType == RecurringSourceType.CreditCard
            ? await RealizeCardAsync(occurrence, category, userId, cancellationToken)
            : await RealizeAccountAsync(occurrence, category, userId, cancellationToken);
    }

    private async Task<ApplicationResult<RealizeRecurringOccurrenceResult>> RealizeAccountAsync(
        RecurringTransactionOccurrence occurrence,
        Category? category,
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Already realized: hand back the existing result so a retry stays a no-op.
        if (occurrence.BudgetTransactionId is Guid existingTransactionId)
        {
            var existing = await transactionRepository.FindOwnedByIdAsync(
                existingTransactionId, userId, false, cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException("Realized occurrence transaction was not found.");
            }

            return Success(CreateTransactionUseCase.ToDto(existing));
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            occurrence.AccountId!.Value, userId, cancellationToken);
        if (account is null || !account.IsActive)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.AccountUnavailable);
        }

        if (category is null || !category.IsActive)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.CategoryUnavailable);
        }

        try
        {
            var transaction = new BudgetTransaction(
                Guid.NewGuid(),
                userId,
                account,
                category,
                occurrence.Amount,
                occurrence.GetTransactionType(),
                occurrence.Scope,
                occurrence.ScheduledDate,
                occurrence.Description);
            occurrence.RealizeWithTransaction(transaction.Id, timeProvider.GetUtcNow());
            var persisted = await repository.RealizeAsync(occurrence, transaction, cancellationToken);
            return Success(CreateTransactionUseCase.ToDto(persisted));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
    }

    private async Task<ApplicationResult<RealizeRecurringOccurrenceResult>> RealizeCardAsync(
        RecurringTransactionOccurrence occurrence,
        Category? category,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (occurrence.CreditCardChargeId is Guid existingChargeId)
        {
            var existing = await chargeRepository.FindOwnedByIdAsync(
                existingChargeId, userId, false, cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException("Realized occurrence charge was not found.");
            }

            return Success(CreateCardChargeUseCase.ToDto(existing));
        }

        var card = await cardRepository.FindOwnedByIdAsync(
            occurrence.CreditCardId!.Value, userId, false, cancellationToken);
        if (card is null || !card.IsActive)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.CardUnavailable);
        }

        if (category is null || !category.IsActive)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.CategoryUnavailable);
        }

        // Short limit produces no charge and no occurrence state change. The
        // occurrence stays planned and stays retryable once the limit recovers,
        // which is why there is no persisted failure state to clean up later.
        var currentDebt = await cardRepository.CalculateCurrentDebtAsync(
            card.Id, userId, cancellationToken);
        if (occurrence.Amount.Amount > card.Limit.Amount - currentDebt)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.CardLimitInsufficient);
        }

        try
        {
            var charge = new CreditCardCharge(
                Guid.NewGuid(),
                userId,
                card,
                category,
                occurrence.Amount,
                occurrence.Scope,
                occurrence.ScheduledDate,
                occurrence.Description);
            occurrence.RealizeWithCharge(charge.Id, timeProvider.GetUtcNow());
            var persisted = await repository.RealizeWithChargeAsync(
                occurrence, charge, cancellationToken);
            return Success(CreateCardChargeUseCase.ToDto(persisted));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
    }

    private static ApplicationResult<RealizeRecurringOccurrenceResult> Success(
        TransactionDto transaction)
    {
        return ApplicationResult<RealizeRecurringOccurrenceResult>.Success(
            new RealizeRecurringOccurrenceResult(
                RecurringSourceType.Account, transaction, null));
    }

    private static ApplicationResult<RealizeRecurringOccurrenceResult> Success(
        CardChargeDto charge)
    {
        return ApplicationResult<RealizeRecurringOccurrenceResult>.Success(
            new RealizeRecurringOccurrenceResult(
                RecurringSourceType.CreditCard, null, charge));
    }
}

public sealed class DeleteRecurringTransactionUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<Guid>> ExecuteAsync(
        DeleteRecurringTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<Guid>.Failure(RecurringErrors.AuthenticationRequired);
        }

        var result = await repository.DeleteOwnedIfUnrealizedAsync(
            command.RecurringTransactionId, userId, cancellationToken);

        return result switch
        {
            RecurringDeletionResult.Deleted =>
                ApplicationResult<Guid>.Success(command.RecurringTransactionId),
            RecurringDeletionResult.NotFound =>
                ApplicationResult<Guid>.Failure(
                    RecurringErrors.NotFound(command.RecurringTransactionId)),
            RecurringDeletionResult.HasRealizedHistory =>
                ApplicationResult<Guid>.Failure(RecurringErrors.HasRealizedHistory),
            _ => throw new InvalidOperationException("Unknown recurring deletion result.")
        };
    }
}

/// <summary>
/// Realizes the recurring item that falls on one date, generating its occurrence
/// row first when nobody has generated it yet.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why this exists.</b> The planned view lists two kinds of recurring row:
/// occurrences that exist, and dates projected from the schedule that were never
/// generated. Only the first kind could be realized, and generating was a button
/// on another screen — so the projected rows carried a permanently disabled
/// action and the screen never said why. Generating is bookkeeping the system
/// needs for idempotency, not a decision the user should have to make; the
/// decision is to realize.
/// </para>
/// <para>
/// <b>Why it composes rather than duplicates.</b> Generation advances the
/// schedule and keys occurrences by owner and period; realization branches on the
/// source and writes either a movement or a card charge. Re-implementing either
/// here would be a second copy of a financial rule, and the two copies would
/// drift. The cost is two save boundaries instead of one: if realization fails
/// after generation succeeded, what is left is a pending occurrence — exactly the
/// state the old "generate" button produced, and the next attempt picks it up.
/// </para>
/// </remarks>
public sealed class RealizeDueRecurringUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository,
    GenerateRecurringOccurrencesUseCase generate,
    RealizeRecurringOccurrenceUseCase realize,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<RealizeRecurringOccurrenceResult>> ExecuteAsync(
        RealizeDueRecurringCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        var schedule = await repository.FindOwnedByIdAsync(
            command.RecurringTransactionId, userId, false, cancellationToken);
        if (schedule is null)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.NotFound(command.RecurringTransactionId));
        }

        // Checked before generating: an inactive plan must not produce money, and
        // generating first would leave rows behind for a plan the user switched
        // off.
        if (!schedule.IsActive)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.ScheduleInactive);
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (command.ScheduledDate > today)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.NotDueYet);
        }

        var occurrence = await repository.FindOccurrenceOwnedByDateAsync(
            command.RecurringTransactionId, command.ScheduledDate, userId, cancellationToken);

        if (occurrence is null)
        {
            // Generation walks every schedule that is due, so the horizon is the
            // requested date and nothing beyond it: realizing an overdue row must
            // not quietly create the months after it.
            var generated = await generate.ExecuteAsync(
                new GenerateRecurringOccurrencesCommand(command.ScheduledDate),
                cancellationToken);
            if (!generated.IsSuccess)
            {
                return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                    generated.Error);
            }

            occurrence = await repository.FindOccurrenceOwnedByDateAsync(
                command.RecurringTransactionId, command.ScheduledDate, userId, cancellationToken);
        }

        // Still nothing: the date the client projected is not one this schedule
        // actually falls on. Reporting it as missing beats realizing the nearest
        // one, which would move money to a day the user never chose.
        if (occurrence is null)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.OccurrenceNotFound(command.RecurringTransactionId));
        }

        return await realize.ExecuteAsync(
            new RealizeRecurringOccurrenceCommand(occurrence.Id, command.Amount),
            cancellationToken);
    }
}
