using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
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
                command.DueDate,
                command.Vat?.ToDomain(),
                TaxDeductibilityResolution.Resolve(
                    command.IsTaxDeductible,
                    category.DefaultIsTaxDeductible,
                    scope,
                    command.Direction == DebtDirection.Payable));
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
        charge.DueDate,
        VatDto.From(charge.Vat),
        charge.IsTaxDeductible);
}

/// <summary>
/// Tahsilat ya da ödeme yazar: kasa değişir, gelir/gider üretilmez.
/// </summary>
public sealed class CreateCounterpartyPaymentUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    IAccountRepository accountRepository)
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
        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive)
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.AccountUnavailable);
        }

        try
        {
            var payment = new CounterpartyPayment(
                Guid.NewGuid(),
                userId,
                counterparty,
                account,
                command.Direction,
                new Money(command.Amount, command.Currency),
                command.PaymentDate,
                command.Description);
            await repository.AddPaymentAsync(payment, cancellationToken);
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
        payment.IsCancelled);
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
public sealed class CancelCounterpartyPaymentUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
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

        payment.Cancel(timeProvider.GetUtcNow().ToUniversalTime());
        await repository.SavePaymentAsync(payment, cancellationToken);
        return ApplicationResult<CounterpartyPaymentDto>.Success(
            CreateCounterpartyPaymentUseCase.ToDto(payment));
    }
}
