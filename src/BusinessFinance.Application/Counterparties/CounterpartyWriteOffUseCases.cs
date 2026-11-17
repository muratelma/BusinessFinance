using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Counterparties;

/// <summary>Vazgeçmenin kalıcılık kapısı.</summary>
public interface ICounterpartyWriteOffRepository
{
    /// <returns>Aynı kimlikle bir kayıt zaten yazılmışsa <c>false</c>.</returns>
    Task<bool> AddAsync(CounterpartyWriteOff writeOff, CancellationToken cancellationToken);

    Task<CounterpartyWriteOff?> FindOwnedAsync(
        Guid writeOffId, Guid userId, CancellationToken cancellationToken);

    Task SaveAsync(CancellationToken cancellationToken);
}

public sealed record CreateCounterpartyWriteOffCommand(
    Guid CounterpartyId,
    Guid CategoryId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    TransactionScope? Scope,
    DateOnly WriteOffDate,
    string? Description,
    Guid? ClientRequestId = null);

public sealed record CounterpartyWriteOffDto(
    Guid Id,
    Guid CounterpartyId,
    Guid CategoryId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    TransactionScope Scope,
    DateOnly WriteOffDate,
    string? Description,
    bool IsCancelled);

/// <summary>
/// Kişinin hesabında kalan tutardan vazgeçer: alacaktan vazgeçmek o gün gider,
/// bağışlanan borç o gün gelir yazar; para hareket etmez (ADR 0021 İ4).
/// </summary>
/// <remarks>
/// O taraftaki kalanı aşamaz: kalmamış bir borçtan vazgeçilmez. Kapsam
/// borçlandırmanın zinciriyle kurulur (kategori → açık seçim → kişinin türü;
/// türü boş eski kişide profil).
/// </remarks>
public sealed class CreateCounterpartyWriteOffUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository counterpartyRepository,
    ICounterpartyWriteOffRepository repository,
    ICategoryRepository categoryRepository,
    IUserProfileRepository profileRepository,
    TimeProvider timeProvider)
{
    internal const string IdPurpose = "counterparty-write-off";

    public async Task<ApplicationResult<CounterpartyWriteOffDto>> ExecuteAsync(
        CreateCounterpartyWriteOffCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyWriteOffDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var today = LocalDay.LatestAllowed(timeProvider.GetUtcNow());
        if (command.WriteOffDate > today)
        {
            return ApplicationResult<CounterpartyWriteOffDto>.Failure(CounterpartyErrors.DateInFuture);
        }

        var counterparty = await counterpartyRepository.FindOwnedByIdAsync(
            command.CounterpartyId, userId, cancellationToken);
        if (counterparty is null)
        {
            return ApplicationResult<CounterpartyWriteOffDto>.Failure(
                CounterpartyErrors.NotFound(command.CounterpartyId));
        }

        Guid? requestScopedId = null;
        if (command.ClientRequestId is Guid clientRequestId && clientRequestId != Guid.Empty)
        {
            requestScopedId = RequestScopedId.Create(IdPurpose, userId, clientRequestId);
            var existing = await repository.FindOwnedAsync(requestScopedId.Value, userId, cancellationToken);
            if (existing is not null)
            {
                return existing.CounterpartyId == command.CounterpartyId
                    ? ApplicationResult<CounterpartyWriteOffDto>.Success(ToDto(existing))
                    : ApplicationResult<CounterpartyWriteOffDto>.Failure(CounterpartyErrors.Validation(
                        "Client request id was already used for another record."));
            }
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (category is null || !category.IsActive ||
            category.Type != CounterpartyWriteOff.RequiredCategoryType(command.Direction))
        {
            return ApplicationResult<CounterpartyWriteOffDto>.Failure(
                CounterpartyErrors.CategoryUnavailable);
        }

        // Kalmamış bir tutardan vazgeçilmez. Pasif kişi bilerek engellenmez:
        // kalan bakiyesi vazgeçmeyle de kapanabilmeli.
        var balance = await counterpartyRepository.FindBalanceAsync(
            counterparty.Id, userId, today, cancellationToken);
        var remaining = command.Direction == DebtDirection.Receivable
            ? balance?.Receivable ?? 0m
            : balance?.Payable ?? 0m;
        if (command.Amount > remaining)
        {
            return ApplicationResult<CounterpartyWriteOffDto>.Failure(
                CounterpartyErrors.WriteOffExceedsBalance);
        }

        var resolution = TransactionScopeResolution.Resolve(
            command.Scope, category.DefaultScope, counterparty.Scope);
        if (resolution.Failure == ScopeResolutionFailure.Unresolved)
        {
            var profile = await profileRepository.FindAsync(userId, track: false, cancellationToken);
            resolution = ScopeResolution.Resolved(profile?.HasBusiness ?? false
                ? TransactionScope.Business
                : TransactionScope.Personal);
        }

        if (resolution.Scope is not TransactionScope scope)
        {
            return ApplicationResult<CounterpartyWriteOffDto>.Failure(resolution.ToError(
                CounterpartyErrors.ScopeUnresolved, CounterpartyErrors.ScopeConflict));
        }

        try
        {
            var writeOff = new CounterpartyWriteOff(
                requestScopedId ?? Guid.NewGuid(),
                userId,
                counterparty,
                category,
                command.Direction,
                new Money(command.Amount, command.Currency),
                scope,
                command.WriteOffDate,
                command.Description);
            if (!await repository.AddAsync(writeOff, cancellationToken))
            {
                var replay = await repository.FindOwnedAsync(writeOff.Id, userId, cancellationToken);
                return replay is not null && replay.CounterpartyId == command.CounterpartyId
                    ? ApplicationResult<CounterpartyWriteOffDto>.Success(ToDto(replay))
                    : ApplicationResult<CounterpartyWriteOffDto>.Failure(CounterpartyErrors.LoanChanged);
            }

            return ApplicationResult<CounterpartyWriteOffDto>.Success(ToDto(writeOff));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CounterpartyWriteOffDto>.Failure(
                CounterpartyErrors.Validation(exception.Message));
        }
    }

    internal static CounterpartyWriteOffDto ToDto(CounterpartyWriteOff writeOff) => new(
        writeOff.Id,
        writeOff.CounterpartyId,
        writeOff.CategoryId,
        writeOff.Direction,
        writeOff.Amount.Amount,
        writeOff.Amount.Currency,
        writeOff.Scope,
        writeOff.WriteOffDate,
        writeOff.Description,
        writeOff.IsCancelled);
}

/// <summary>
/// Vazgeçmeyi iptal eder: kalan bakiye geri gelir, tanınan gelir ya da gider
/// düşer. İdempotenttir.
/// </summary>
public sealed class CancelCounterpartyWriteOffUseCase(
    ICurrentUser currentUser,
    ICounterpartyWriteOffRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CounterpartyWriteOffDto>> ExecuteAsync(
        Guid writeOffId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyWriteOffDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var writeOff = await repository.FindOwnedAsync(writeOffId, userId, cancellationToken);
        if (writeOff is null)
        {
            return ApplicationResult<CounterpartyWriteOffDto>.Failure(
                CounterpartyErrors.WriteOffNotFound(writeOffId));
        }

        if (!writeOff.IsCancelled)
        {
            writeOff.Cancel(timeProvider.GetUtcNow().ToUniversalTime());
            await repository.SaveAsync(cancellationToken);
        }

        return ApplicationResult<CounterpartyWriteOffDto>.Success(
            CreateCounterpartyWriteOffUseCase.ToDto(writeOff));
    }
}
