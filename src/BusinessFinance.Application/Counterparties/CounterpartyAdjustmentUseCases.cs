using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.DayCloses;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Counterparties;

public sealed record ApplyCounterpartyChargeDiscountCommand(
    Guid ChargeId,
    decimal Amount,
    DateOnly DiscountDate,
    Guid? ClientRequestId = null);

public sealed record SetCounterpartyOpeningBalanceCommand(
    Guid CounterpartyId,
    decimal Receivable,
    decimal Payable);

/// <summary>
/// Cari alıma indirim yazar (ADR 0021 İ4, T3): alımın bedeli düşer, gelir ya da
/// gider alım gününde azalır, cari bakiye azalır; para hareketi yoktur.
/// </summary>
/// <remarks>
/// Gün sonunda sayılmış satış değişmez: gün sonu onu girilen nakitten düşmüştür
/// (iptaldeki kuralın aynısı). Aynı istek kimliği ikinci indirim yazmaz.
/// </remarks>
public sealed class ApplyCounterpartyChargeDiscountUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    IDayCloseCountReader dayCloseCountReader,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CounterpartyChargeDto>> ExecuteAsync(
        ApplyCounterpartyChargeDiscountCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.AuthenticationRequired);

        var now = timeProvider.GetUtcNow().ToUniversalTime();
        if (command.DiscountDate > LocalDay.LatestAllowed(now))
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.DateInFuture);

        var charge = await repository.FindOwnedChargeAsync(command.ChargeId, userId, cancellationToken);
        if (charge is null)
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.ChargeNotFound(command.ChargeId));

        var discountId = command.ClientRequestId is Guid requestId
            ? RequestScopedId.Create("counterparty-discount", userId, requestId)
            : Guid.NewGuid();
        if (charge.Discounts.Any(discount => discount.Id == discountId))
            return ApplicationResult<CounterpartyChargeDto>.Success(CreateCounterpartyChargeUseCase.ToDto(charge));

        if (charge.IsCancelled)
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.ChargeCancelled);

        if (await dayCloseCountReader.IsCountedAsync(
                userId, DayCloseRecordKind.CounterpartyCharge, charge.Id, cancellationToken))
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.ChargeDayCloseCounted);

        try
        {
            charge.ApplyDiscount(discountId, command.Amount, command.DiscountDate, now);
        }
        catch (DebtAmountExceedsRemainingException)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.DiscountExceedsCharge);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.Validation(exception.Message));
        }

        await repository.SaveChargeAsync(charge, cancellationToken);
        return ApplicationResult<CounterpartyChargeDto>.Success(CreateCounterpartyChargeUseCase.ToDto(charge));
    }
}

/// <summary>İndirimi geri alır; alımın bedeli eski hâline döner. İdempotent.</summary>
public sealed class ReverseCounterpartyChargeDiscountUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    IDayCloseCountReader dayCloseCountReader,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CounterpartyChargeDto>> ExecuteAsync(
        Guid chargeId,
        Guid discountId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.AuthenticationRequired);

        var charge = await repository.FindOwnedChargeAsync(chargeId, userId, cancellationToken);
        if (charge is null)
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.ChargeNotFound(chargeId));

        var discount = charge.Discounts.SingleOrDefault(item => item.Id == discountId);
        if (discount is null)
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.DiscountNotFound(discountId));

        if (discount.IsCancelled)
            return ApplicationResult<CounterpartyChargeDto>.Success(CreateCounterpartyChargeUseCase.ToDto(charge));

        if (charge.IsCancelled)
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.ChargeCancelled);

        if (await dayCloseCountReader.IsCountedAsync(
                userId, DayCloseRecordKind.CounterpartyCharge, charge.Id, cancellationToken))
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.ChargeDayCloseCounted);

        charge.ReverseDiscount(discountId, timeProvider.GetUtcNow().ToUniversalTime());
        await repository.SaveChargeAsync(charge, cancellationToken);
        return ApplicationResult<CounterpartyChargeDto>.Success(CreateCounterpartyChargeUseCase.ToDto(charge));
    }
}

/// <summary>
/// Uygulamadan önce devreden bakiyeyi yazar (ADR 0021 İ5, T5): gelir ya da gider
/// yazmaz, cari bakiyeye en eski kalem olarak girer. Yeniden yazılabilir; sıfır
/// devri kaldırır. Pasif kişide de yazılır: açık bakiye kapatılabilmelidir.
/// </summary>
public sealed class SetCounterpartyOpeningBalanceUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CounterpartyDto>> ExecuteAsync(
        SetCounterpartyOpeningBalanceCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<CounterpartyDto>.Failure(CounterpartyErrors.AuthenticationRequired);

        var counterparty = await repository.FindOwnedByIdAsync(command.CounterpartyId, userId, cancellationToken);
        if (counterparty is null)
            return ApplicationResult<CounterpartyDto>.Failure(CounterpartyErrors.NotFound(command.CounterpartyId));

        try
        {
            counterparty.SetOpeningBalance(command.Receivable, command.Payable);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CounterpartyDto>.Failure(CounterpartyErrors.Validation(exception.Message));
        }

        await repository.UpdateOwnedAsync(counterparty, userId, cancellationToken);
        var balance = await repository.FindBalanceAsync(
            counterparty.Id, userId, CounterpartyMapper.Today(timeProvider), cancellationToken);
        return ApplicationResult<CounterpartyDto>.Success(CounterpartyMapper.ToDto(counterparty, balance));
    }
}
