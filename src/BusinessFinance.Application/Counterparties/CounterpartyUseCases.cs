using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Counterparties;

public sealed class CreateCounterpartyUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository)
{
    public async Task<ApplicationResult<CounterpartyDto>> ExecuteAsync(
        CreateCounterpartyCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var normalizedName = command.Name?.Trim() ?? string.Empty;
        if (await repository.ExistsByNameAsync(userId, normalizedName, null, cancellationToken))
        {
            return ApplicationResult<CounterpartyDto>.Failure(CounterpartyErrors.DuplicateName);
        }

        try
        {
            var counterparty = new Counterparty(
                Guid.NewGuid(), userId, normalizedName, command.Note);
            await repository.AddAsync(counterparty, cancellationToken);

            // Yeni karşı tarafın hareketi yok; bakiyeyi sormaya gerek yok ve
            // sorulsaydı da cevabı sıfırdı.
            return ApplicationResult<CounterpartyDto>.Success(
                CounterpartyMapper.ToDto(counterparty, 0m, 0m));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CounterpartyDto>.Failure(
                CounterpartyErrors.Validation(exception.Message));
        }
    }
}

public sealed class UpdateCounterpartyUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository)
{
    public async Task<ApplicationResult<CounterpartyDto>> ExecuteAsync(
        UpdateCounterpartyCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var counterparty = await repository.FindOwnedByIdAsync(
            command.CounterpartyId, userId, cancellationToken);
        if (counterparty is null)
        {
            return ApplicationResult<CounterpartyDto>.Failure(
                CounterpartyErrors.NotFound(command.CounterpartyId));
        }

        var normalizedName = command.Name?.Trim() ?? string.Empty;
        if (await repository.ExistsByNameAsync(
                userId, normalizedName, counterparty.Id, cancellationToken))
        {
            return ApplicationResult<CounterpartyDto>.Failure(CounterpartyErrors.DuplicateName);
        }

        try
        {
            counterparty.Rename(normalizedName);
            counterparty.SetNote(command.Note);

            // Pasifleştirme geçmişi silmez, yeni iş yapmayı durdurur; açık
            // bakiyeli bir karşı taraf da pasifleştirilebilir ve borcu
            // tahsil edilmeye devam eder.
            if (command.IsActive)
            {
                counterparty.Activate();
            }
            else
            {
                counterparty.Deactivate();
            }
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CounterpartyDto>.Failure(
                CounterpartyErrors.Validation(exception.Message));
        }

        await repository.UpdateOwnedAsync(counterparty, userId, cancellationToken);
        var balance = await repository.FindBalanceAsync(counterparty.Id, userId, cancellationToken);
        return ApplicationResult<CounterpartyDto>.Success(
            CounterpartyMapper.ToDto(
                counterparty, balance?.Receivable ?? 0m, balance?.Payable ?? 0m));
    }
}

public sealed class ListCounterpartiesUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<CounterpartyDto>>> ExecuteAsync(
        CounterpartyBalanceFilter filter,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<CounterpartyDto>>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        // Liste tek sorgudan geliyor: ad ve bakiye aynı satırda. Not alanı
        // listede yok — ayrıntı ekranının sorusu ve listeyi genişletirdi.
        var balances = await repository.ListBalancesAsync(
            userId, filter, isActive, cancellationToken);

        return ApplicationResult<IReadOnlyList<CounterpartyDto>>.Success(
            [.. balances.Select(item => new CounterpartyDto(
                item.CounterpartyId,
                item.Name,
                null,
                item.IsActive,
                item.Receivable,
                item.Payable,
                item.Net,
                item.IsSettled))]);
    }
}

public sealed class GetCounterpartyUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository)
{
    public async Task<ApplicationResult<CounterpartyDto>> ExecuteAsync(
        Guid counterpartyId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var counterparty = await repository.FindOwnedByIdAsync(
            counterpartyId, userId, cancellationToken);
        if (counterparty is null)
        {
            return ApplicationResult<CounterpartyDto>.Failure(
                CounterpartyErrors.NotFound(counterpartyId));
        }

        var balance = await repository.FindBalanceAsync(counterpartyId, userId, cancellationToken);
        return ApplicationResult<CounterpartyDto>.Success(
            CounterpartyMapper.ToDto(
                counterparty, balance?.Receivable ?? 0m, balance?.Payable ?? 0m));
    }
}

/// <summary>
/// Hiç hareketi olmayan karşı tarafı siler.
/// </summary>
/// <remarks>
/// Hareketi olan kayıt silinmez, pasifleştirilir: geçmiş bir kez yazıldıktan
/// sonra ona bağlı adın kaybolması, feed'de ve raporlarda sahipsiz satır
/// bırakırdı. Boş hesabın silinebilmesiyle birebir aynı kural.
/// </remarks>
public sealed class DeleteCounterpartyUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository)
{
    public async Task<ApplicationResult<bool>> ExecuteAsync(
        Guid counterpartyId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<bool>.Failure(CounterpartyErrors.AuthenticationRequired);
        }

        var counterparty = await repository.FindOwnedByIdAsync(
            counterpartyId, userId, cancellationToken);
        if (counterparty is null)
        {
            return ApplicationResult<bool>.Failure(CounterpartyErrors.NotFound(counterpartyId));
        }

        var deleted = await repository.DeleteIfWithoutHistoryAsync(
            counterpartyId, userId, cancellationToken);

        return deleted
            ? ApplicationResult<bool>.Success(true)
            : ApplicationResult<bool>.Failure(CounterpartyErrors.HasHistory);
    }
}

internal static class CounterpartyMapper
{
    public static CounterpartyDto ToDto(
        Counterparty counterparty,
        decimal receivable,
        decimal payable) => new(
        counterparty.Id,
        counterparty.Name,
        counterparty.Note,
        counterparty.IsActive,
        receivable,
        payable,
        receivable - payable,
        receivable == 0m && payable == 0m);
}
