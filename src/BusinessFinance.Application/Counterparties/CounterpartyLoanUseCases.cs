using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Counterparties;

/// <summary>
/// Borç paranın kalıcılık kapısı. Cari defterin geri kalanından ayrı
/// durur: bu kayıt gelir ya da gider yazmaz ve borçlandırmayla aynı yoldan
/// okunmamalıdır (ADR 0022 İ3).
/// </summary>
public interface ICounterpartyLoanRepository
{
    /// <returns>Aynı kimlikle bir kayıt zaten yazılmışsa <c>false</c>.</returns>
    Task<bool> AddAsync(CounterpartyLoan loan, CancellationToken cancellationToken);

    Task<CounterpartyLoan?> FindOwnedAsync(
        Guid loanId, Guid userId, CancellationToken cancellationToken);

    /// <returns>Kayıt bu arada değiştiyse <c>false</c>.</returns>
    Task<bool> TrySaveAsync(CounterpartyLoan loan, CancellationToken cancellationToken);
}

/// <summary>
/// Kişiye borç para verir ya da ondan alır: para o gün hesaptan
/// çıkar ya da hesaba girer, kişinin bakiyesi artar, gelir/gider yazılmaz.
/// </summary>
public sealed class CreateCounterpartyLoanUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository counterpartyRepository,
    ICounterpartyLoanRepository repository,
    IAccountRepository accountRepository,
    TimeProvider timeProvider)
{
    internal const string IdPurpose = "counterparty-loan";

    public async Task<ApplicationResult<CounterpartyLoanDto>> ExecuteAsync(
        CreateCounterpartyLoanCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyLoanDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        // Olmuş olan kayıt, olacak olan plandır: para ileri bir günde el
        // değiştiremez. Geri ödeme günü (vade) ileride olabilir.
        if (command.LoanDate > LocalDay.LatestAllowed(timeProvider.GetUtcNow()))
        {
            return ApplicationResult<CounterpartyLoanDto>.Failure(
                CounterpartyErrors.DateInFuture);
        }

        var counterparty = await counterpartyRepository.FindOwnedByIdAsync(
            command.CounterpartyId, userId, cancellationToken);
        if (counterparty is null)
        {
            return ApplicationResult<CounterpartyLoanDto>.Failure(
                CounterpartyErrors.NotFound(command.CounterpartyId));
        }

        Guid? requestScopedId = null;
        if (command.ClientRequestId is Guid clientRequestId && clientRequestId != Guid.Empty)
        {
            requestScopedId = RequestScopedId.Create(IdPurpose, userId, clientRequestId);
            var replay = await ReplayAsync(requestScopedId.Value, userId, command, cancellationToken);
            if (replay is not null)
            {
                return replay;
            }
        }

        if (!counterparty.IsActive)
        {
            return ApplicationResult<CounterpartyLoanDto>.Failure(CounterpartyErrors.Inactive);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive)
        {
            return ApplicationResult<CounterpartyLoanDto>.Failure(
                CounterpartyErrors.AccountUnavailable);
        }

        try
        {
            var loan = new CounterpartyLoan(
                requestScopedId ?? Guid.NewGuid(),
                userId,
                counterparty,
                account,
                command.Direction,
                new Money(command.Amount, command.Currency),
                command.LoanDate,
                command.DueDate,
                command.Description);
            if (!await repository.AddAsync(loan, cancellationToken))
            {
                return await ReplayAsync(loan.Id, userId, command, cancellationToken)
                    ?? ApplicationResult<CounterpartyLoanDto>.Failure(CounterpartyErrors.LoanChanged);
            }

            return ApplicationResult<CounterpartyLoanDto>.Success(ToDto(loan));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CounterpartyLoanDto>.Failure(
                CounterpartyErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<CounterpartyLoanDto>.Failure(
                CounterpartyErrors.Validation(exception.Message));
        }
    }

    private async Task<ApplicationResult<CounterpartyLoanDto>?> ReplayAsync(
        Guid loanId,
        Guid userId,
        CreateCounterpartyLoanCommand command,
        CancellationToken cancellationToken)
    {
        var existing = await repository.FindOwnedAsync(loanId, userId, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        return existing.CounterpartyId == command.CounterpartyId
            ? ApplicationResult<CounterpartyLoanDto>.Success(ToDto(existing))
            : ApplicationResult<CounterpartyLoanDto>.Failure(
                CounterpartyErrors.Validation("Client request id was already used for another record."));
    }

    internal static CounterpartyLoanDto ToDto(CounterpartyLoan loan) => new(
        loan.Id,
        loan.CounterpartyId,
        loan.AccountId,
        loan.Direction,
        loan.Amount.Amount,
        loan.Amount.Currency,
        loan.LoanDate,
        loan.DueDate,
        loan.Description,
        loan.IsCancelled);
}

/// <summary>
/// Borç parayı iptal eder: kişinin bakiyesi ve hesap bakiyesi
/// yeniden hesaplanır, başka hiçbir kayıt değişmez. İdempotenttir.
/// </summary>
public sealed class CancelCounterpartyLoanUseCase(
    ICurrentUser currentUser,
    ICounterpartyLoanRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CounterpartyLoanDto>> ExecuteAsync(
        Guid loanId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyLoanDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var loan = await repository.FindOwnedAsync(loanId, userId, cancellationToken);
        if (loan is null)
        {
            return ApplicationResult<CounterpartyLoanDto>.Failure(
                CounterpartyErrors.LoanNotFound(loanId));
        }

        if (!loan.IsCancelled)
        {
            loan.Cancel(timeProvider.GetUtcNow().ToUniversalTime());
            if (!await repository.TrySaveAsync(loan, cancellationToken))
            {
                return ApplicationResult<CounterpartyLoanDto>.Failure(
                    CounterpartyErrors.LoanChanged);
            }
        }

        return ApplicationResult<CounterpartyLoanDto>.Success(
            CreateCounterpartyLoanUseCase.ToDto(loan));
    }
}
