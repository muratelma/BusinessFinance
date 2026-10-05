using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.DayCloses;
using BusinessFinance.Application.Pos;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Counterparties;

/// <summary>
/// Veresiye satış ya da vadeli alım yazar: gelir/gider tanınır, kasa kıpırdamaz.
/// </summary>
public sealed class CreateCounterpartyChargeUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    ICategoryRepository categoryRepository)
{
    public async Task<ApplicationResult<CounterpartyChargeDto>> ExecuteAsync(
        CreateCounterpartyChargeCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var counterparty = await repository.FindOwnedByIdAsync(
            command.CounterpartyId, userId, cancellationToken);
        if (counterparty is null)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.NotFound(command.CounterpartyId));
        }

        if (!counterparty.IsActive)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.Inactive);
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);

        // Yön kategorinin türünü belirler: alacak doğuran kayıt gelir, borç
        // doğuran kayıt gider kategorisi ister. Domain de aynı kuralı bağımsız
        // uyguluyor; buradaki kontrol kullanıcıya net bir hata kodu döndürmek
        // için.
        if (category is null || !category.IsActive ||
            category.Type != CounterpartyCharge.RequiredCategoryType(command.Direction))
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.CategoryUnavailable);
        }

        // Zincirin ortasındaki halka yok: borçlandırmanın hesabı olmadığı için
        // kapsam ya istekten ya kategoriden gelir. Karşı taraf kapsam ipucu
        // taşımaz (Aşama 02 kapsam dışı).
        if (TransactionScopeResolution.Resolve(command.Scope, category.DefaultScope)
            is not TransactionScope scope)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.ScopeUnresolved);
        }

        try
        {
            var charge = new CounterpartyCharge(
                Guid.NewGuid(),
                userId,
                counterparty,
                category,
                command.Direction,
                new Money(command.Amount, command.Currency),
                scope,
                command.ChargeDate,
                command.Description,
                command.DueDate);
            await repository.AddChargeAsync(charge, cancellationToken);
            return ApplicationResult<CounterpartyChargeDto>.Success(ToDto(charge));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.Validation(exception.Message));
        }
    }

    internal static CounterpartyChargeDto ToDto(CounterpartyCharge charge) => new(
        charge.Id,
        charge.CounterpartyId,
        charge.CategoryId,
        charge.Direction,
        charge.Amount.Amount,
        charge.Amount.Currency,
        charge.Scope,
        charge.ChargeDate,
        charge.Description,
        charge.IsCancelled,
        charge.DueDate);
}

/// <summary>
/// Tahsilat ya da ödeme yazar: kasa değişir, gelir/gider üretilmez.
/// </summary>
/// <remarks>
/// Kartla tahsilde (ADR 0019 T5) cari brüt tutarla bugün kapanır, para POS
/// kaydıyla yola çıkar; komisyon tahsil günü gider yazılır, gelir yazılmaz.
/// </remarks>
public sealed class CreateCounterpartyPaymentUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    IAccountRepository accountRepository,
    CardCollectionBuilder cardCollectionBuilder,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CounterpartyPaymentDto>> ExecuteAsync(
        CreateCounterpartyPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var counterparty = await repository.FindOwnedByIdAsync(
            command.CounterpartyId, userId, cancellationToken);
        if (counterparty is null)
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.NotFound(command.CounterpartyId));
        }

        // Pasif karşı taraf **bilerek** engellenmiyor: artık iş yapılmayan bir
        // müşteri kalan borcunu ödeyebilmeli, yoksa bakiye kapatılamaz hâle
        // gelirdi.
        try
        {
            var amount = new Money(command.Amount, command.Currency);
            Account? account;
            PosSettlement? cardSettlement = null;
            if (command.Card is CardCollectionInput card)
            {
                if (command.Direction != DebtDirection.Receivable)
                {
                    return ApplicationResult<CounterpartyPaymentDto>.Failure(
                        CounterpartyErrors.CardRequiresCollection);
                }

                // Hesap formda ayrıca seçilmişse kartın hesabı odur.
                var built = await cardCollectionBuilder.BuildAsync(
                    userId,
                    card with { AccountId = card.AccountId ?? command.AccountId },
                    amount,
                    command.PaymentDate,
                    timeProvider.GetUtcNow().ToUniversalTime(),
                    command.Description,
                    cancellationToken);
                if (built.Error is ApplicationError error)
                {
                    return ApplicationResult<CounterpartyPaymentDto>.Failure(error);
                }

                cardSettlement = built.Settlement;
                account = built.Account;
            }
            else
            {
                if (command.AccountId is not Guid accountId)
                {
                    return ApplicationResult<CounterpartyPaymentDto>.Failure(
                        CounterpartyErrors.AccountUnavailable);
                }

                account = await accountRepository.FindOwnedByIdAsync(
                    accountId, userId, cancellationToken);
                if (account is null || !account.IsActive)
                {
                    return ApplicationResult<CounterpartyPaymentDto>.Failure(
                        CounterpartyErrors.AccountUnavailable);
                }
            }

            var payment = new CounterpartyPayment(
                Guid.NewGuid(),
                userId,
                counterparty,
                account!,
                command.Direction,
                amount,
                command.PaymentDate,
                command.Description,
                cardSettlement);
            await repository.AddPaymentAsync(payment, cardSettlement, cancellationToken);
            return ApplicationResult<CounterpartyPaymentDto>.Success(ToDto(payment));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.Validation(exception.Message));
        }
    }

    internal static CounterpartyPaymentDto ToDto(CounterpartyPayment payment) => new(
        payment.Id,
        payment.CounterpartyId,
        payment.AccountId,
        payment.Direction,
        payment.Amount.Amount,
        payment.Amount.Currency,
        payment.PaymentDate,
        payment.Description,
        payment.IsCancelled,
        payment.PosSettlementId);
}

/// <summary>
/// Borçlandırmayı iptal eder: tanınan gelir/gider ve cari bakiye birlikte geri
/// alınır, çünkü ikisi de aynı kaydın türevi.
/// </summary>
public sealed class CancelCounterpartyChargeUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CounterpartyChargeDto>> ExecuteAsync(
        Guid chargeId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var charge = await repository.FindOwnedChargeAsync(chargeId, userId, cancellationToken);
        if (charge is null)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.ChargeNotFound(chargeId));
        }

        // İptal idempotent: ikinci istek aynı sonucu döner, ilk iptalin zaman
        // damgasını değiştirmez.
        charge.Cancel(timeProvider.GetUtcNow().ToUniversalTime());
        await repository.SaveChargeAsync(charge, cancellationToken);
        return ApplicationResult<CounterpartyChargeDto>.Success(
            CreateCounterpartyChargeUseCase.ToDto(charge));
    }
}

/// <summary>
/// Tahsilatı iptal eder: kasadan çıkan/giren para geri alınır, açık bakiye
/// yeniden doğar.
/// </summary>
/// <remarks>
/// Kartla tahsilde POS kaydı da birlikte iptal olur: yoldaki para ve komisyon
/// gideri düşer (K4). Para yatışla hesaba geçtiyse önce yatış geri alınır; gün
/// sonunda sayıldıysa önce gün sonu.
/// </remarks>
public sealed class CancelCounterpartyPaymentUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    IDayCloseCountReader dayCloseCountReader,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CounterpartyPaymentDto>> ExecuteAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.AuthenticationRequired);
        }

        var payment = await repository.FindOwnedPaymentAsync(paymentId, userId, cancellationToken);
        if (payment is null)
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.PaymentNotFound(paymentId));
        }

        if (payment.IsCancelled)
        {
            return ApplicationResult<CounterpartyPaymentDto>.Success(
                CreateCounterpartyPaymentUseCase.ToDto(payment));
        }

        if (await dayCloseCountReader.IsCountedAsync(
                userId, DayCloseRecordKind.CounterpartyPayment, payment.Id, cancellationToken) ||
            payment.PosSettlementId is Guid countedSettlementId &&
            await dayCloseCountReader.IsCountedAsync(
                userId, DayCloseRecordKind.PosSettlement, countedSettlementId, cancellationToken))
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.PaymentDayCloseCounted);
        }

        var cancelledAtUtc = timeProvider.GetUtcNow().ToUniversalTime();
        if (payment.PosSettlementId is Guid settlementId)
        {
            var cardSettlement = await repository.FindCardSettlementAsync(
                settlementId, userId, cancellationToken)
                ?? throw new InvalidOperationException("A card collection lost its pos settlement.");
            if (cardSettlement is { IsCancelled: false, PosDepositId: not null })
            {
                return ApplicationResult<CounterpartyPaymentDto>.Failure(
                    CounterpartyErrors.PaymentDepositLocked);
            }

            cardSettlement.Cancel(cancelledAtUtc);
        }

        payment.Cancel(cancelledAtUtc);
        if (!await repository.TrySavePaymentAsync(payment, cancellationToken))
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.PaymentChanged);
        }

        return ApplicationResult<CounterpartyPaymentDto>.Success(
            CreateCounterpartyPaymentUseCase.ToDto(payment));
    }
}
