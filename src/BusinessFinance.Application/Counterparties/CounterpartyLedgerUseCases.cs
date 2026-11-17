using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.DayCloses;
using BusinessFinance.Application.Pos;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.CreditCards;
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
    ICategoryRepository categoryRepository,
    IUserProfileRepository profileRepository,
    IAccountRepository accountRepository,
    TimeProvider timeProvider)
{
    internal const string IdPurpose = "counterparty-charge";
    internal const string PaidNowIdPurpose = "counterparty-charge-paid-now";

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

        // Satışın ya da alımın günü ileride olamaz; vadesi olabilir.
        if (command.ChargeDate > LocalDay.LatestAllowed(timeProvider.GetUtcNow()))
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.DateInFuture);
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

        // Aynı istek kimliği tek kayıt yazar: tekrar gönderilen istek var olanı
        // döner, başka kişide kullanılmış kimlik reddedilir.
        Guid? requestScopedId = null;
        Guid? paidNowScopedId = null;
        if (command.ClientRequestId is Guid clientRequestId && clientRequestId != Guid.Empty)
        {
            requestScopedId = RequestScopedId.Create(IdPurpose, userId, clientRequestId);
            paidNowScopedId = RequestScopedId.Create(PaidNowIdPurpose, userId, clientRequestId);
            var replay = await ReplayAsync(requestScopedId.Value, userId, command, cancellationToken);
            if (replay is not null)
            {
                return replay;
            }
        }

        // Peşin ya da avans tutarı: aynı gün, aynı kişiye, satışı aşmadan.
        Account? paidNowAccount = null;
        if (command.PaidNow is CounterpartyPaidNow paidNow)
        {
            if (paidNow.Amount > command.Amount)
            {
                return ApplicationResult<CounterpartyChargeDto>.Failure(
                    CounterpartyErrors.PaidNowExceedsCharge);
            }

            paidNowAccount = await accountRepository.FindOwnedByIdAsync(
                paidNow.AccountId, userId, cancellationToken);
            if (paidNowAccount is null || !paidNowAccount.IsActive)
            {
                return ApplicationResult<CounterpartyChargeDto>.Failure(
                    CounterpartyErrors.AccountUnavailable);
            }
        }

        // Kapsamı her kayıt gibi kategori sınırlar; kişinin türü yalnız ön
        // değerdir (ADR 0022 İ5). Türü bilinmeyen eski kişide işletmesi olan
        // kullanıcının kaydı işletme sayılır: cari bu ayrımdan önce işletmeye
        // özeldi. İşletmesi olmayanın kaydı şahsidir.
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
            return ApplicationResult<CounterpartyChargeDto>.Failure(resolution.ToError(
                CounterpartyErrors.ScopeUnresolved, CounterpartyErrors.ScopeConflict));
        }

        try
        {
            var charge = new CounterpartyCharge(
                requestScopedId ?? Guid.NewGuid(),
                userId,
                counterparty,
                category,
                command.Direction,
                new Money(command.Amount, command.Currency),
                scope,
                command.ChargeDate,
                command.Description,
                command.DueDate);
            var paidNowPayment = command.PaidNow is CounterpartyPaidNow paid
                ? new CounterpartyPayment(
                    paidNowScopedId ?? Guid.NewGuid(),
                    userId,
                    counterparty,
                    paidNowAccount!,
                    command.Direction,
                    new Money(paid.Amount, command.Currency),
                    command.ChargeDate,
                    command.Description,
                    paidWithCharge: charge)
                : null;
            if (!await repository.AddChargeAsync(charge, cancellationToken, paidNowPayment))
            {
                return await ReplayAsync(charge.Id, userId, command, cancellationToken)
                    ?? ApplicationResult<CounterpartyChargeDto>.Failure(CounterpartyErrors.PaymentChanged);
            }

            return ApplicationResult<CounterpartyChargeDto>.Success(ToDto(charge, paidNowPayment?.Id));
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

    private async Task<ApplicationResult<CounterpartyChargeDto>?> ReplayAsync(
        Guid chargeId,
        Guid userId,
        CreateCounterpartyChargeCommand command,
        CancellationToken cancellationToken)
    {
        var existing = await repository.FindOwnedChargeAsync(chargeId, userId, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        if (existing.CounterpartyId != command.CounterpartyId)
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.Validation("Client request id was already used for another charge."));
        }

        var paidNow = await repository.FindPaidWithChargeAsync(existing.Id, userId, cancellationToken);
        return ApplicationResult<CounterpartyChargeDto>.Success(
            ToDto(existing, paidNow.Count > 0 ? paidNow[0].Id : null));
    }

    internal static CounterpartyChargeDto ToDto(CounterpartyCharge charge, Guid? paidNowPaymentId = null) => new(
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
        charge.NetAmount,
        [.. charge.Discounts
            .OrderBy(discount => discount.RecordedAtUtc)
            .Select(discount => new CounterpartyChargeDiscountDto(
                discount.Id, discount.Amount, discount.DiscountDate, discount.IsCancelled))],
        paidNowPaymentId);
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
    ICreditCardRepository creditCardRepository,
    IDayCloseCountReader dayCloseCountReader,
    TimeProvider timeProvider)
{
    /// <summary>
    /// <c>Kalanı indirim say</c> (senaryo 20): ödemeden sonra o tarafta kalan
    /// tutarı en yeni açık alımlardan başlayarak indirim yazar. İndirim alımın
    /// bedelini düşürür; gelir ya da gider alım gününde azalır. Gün sonunda
    /// sayılmış alım değişmez. Kalan alımlarla kapanmıyorsa (devreden bakiye,
    /// borç para) hiçbir şey yazılmaz: kalan vazgeçmeyle kapatılır.
    /// </summary>
    private async Task<(IReadOnlyCollection<CounterpartyCharge>? Charges, decimal Total, ApplicationError? Error)>
        DiscountRemainderAsync(
            CreateCounterpartyPaymentCommand command,
            CounterpartyPayment payment,
            Guid userId,
            CancellationToken cancellationToken)
    {
        if (!command.CloseRemainderAsDiscount)
        {
            return (null, 0m, null);
        }

        var now = timeProvider.GetUtcNow().ToUniversalTime();
        var balance = await repository.FindBalanceAsync(
            command.CounterpartyId, userId, LocalDay.LatestAllowed(now), cancellationToken);
        var side = command.Direction == DebtDirection.Receivable
            ? balance?.Receivable ?? 0m
            : balance?.Payable ?? 0m;
        var remainder = side - command.Amount;
        if (remainder <= 0m)
        {
            return (null, 0m, null);
        }

        var candidates = await repository.FindRemainderCandidatesAsync(
            command.CounterpartyId, userId, command.Direction, command.PaymentDate, cancellationToken);
        var discounted = new List<CounterpartyCharge>();
        var left = remainder;
        foreach (var charge in candidates)
        {
            if (left <= 0m)
            {
                break;
            }

            if (charge.NetAmount <= 0m ||
                await dayCloseCountReader.IsCountedAsync(
                    userId, DayCloseRecordKind.CounterpartyCharge, charge.Id, cancellationToken))
            {
                continue;
            }

            var take = Math.Min(charge.NetAmount, left);
            charge.ApplyDiscount(Guid.NewGuid(), take, command.PaymentDate, now, payment.Id);
            discounted.Add(charge);
            left -= take;
        }

        return left > 0m
            ? (null, 0m, CounterpartyErrors.DiscountExceedsCharges)
            : (discounted, remainder, null);
    }

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

        // Kartla tahsil POS kaydının kendi denetiminden de geçer; hesaptan
        // tahsilat ve ödeme yalnız buradan.
        if (command.PaymentDate > LocalDay.LatestAllowed(timeProvider.GetUtcNow()))
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.DateInFuture);
        }

        var counterparty = await repository.FindOwnedByIdAsync(
            command.CounterpartyId, userId, cancellationToken);
        if (counterparty is null)
        {
            return ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.NotFound(command.CounterpartyId));
        }

        // İstek kimliği varsa tahsilatın kimliği ondan türetilir: zaman aşımından
        // sonra aynı formdan yeniden gönderilen istek ikinci bir para hareketi
        // yazmaz, ilkini döner. Kimlik isteğe bağlıdır; eski istemci kimliksiz
        // gönderir ve her istek yeni kayıttır.
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

        // Pasif karşı taraf **bilerek** engellenmiyor: artık iş yapılmayan bir
        // müşteri kalan borcunu ödeyebilmeli, yoksa bakiye kapatılamaz hâle
        // gelirdi.
        try
        {
            var amount = new Money(command.Amount, command.Currency);

            // Kişiye kredi kartıyla ödeme (S2): kart borcu artar, hesap
            // kıpırdamaz, gider yazılmaz. Kart harcamasıyla aynı limit kuralı:
            // limit − borç − karta yansımamış taksitler.
            if (command.CreditCardId is Guid creditCardId)
            {
                if (command.Direction != DebtDirection.Payable ||
                    command.AccountId is not null ||
                    command.Card is not null)
                {
                    return ApplicationResult<CounterpartyPaymentDto>.Failure(
                        CounterpartyErrors.CardRequiresPayment);
                }

                var creditCard = await creditCardRepository.FindOwnedByIdAsync(
                    creditCardId, userId, false, cancellationToken);
                if (creditCard is null || !creditCard.IsActive)
                {
                    return ApplicationResult<CounterpartyPaymentDto>.Failure(
                        CounterpartyErrors.CardUnavailable);
                }

                var currentDebt = await creditCardRepository.CalculateCurrentDebtAsync(
                    creditCard.Id, userId, cancellationToken);
                var pendingInstallments = await creditCardRepository.CalculatePendingInstallmentsAsync(
                    creditCard.Id, userId, cancellationToken);
                if (command.Amount > creditCard.Limit.Amount - currentDebt - pendingInstallments)
                {
                    return ApplicationResult<CounterpartyPaymentDto>.Failure(
                        CreditCardErrors.LimitExceeded);
                }

                var cardPayment = CounterpartyPayment.PayByCard(
                    requestScopedId ?? Guid.NewGuid(),
                    userId,
                    counterparty,
                    creditCard,
                    amount,
                    command.PaymentDate,
                    command.Description);
                var cardRemainder = await DiscountRemainderAsync(command, cardPayment, userId, cancellationToken);
                if (cardRemainder.Error is ApplicationError cardRemainderError)
                {
                    return ApplicationResult<CounterpartyPaymentDto>.Failure(cardRemainderError);
                }

                if (!await repository.AddPaymentAsync(
                        cardPayment, null, cancellationToken, cardRemainder.Charges))
                {
                    return await ReplayAsync(cardPayment.Id, userId, command, cancellationToken)
                        ?? ApplicationResult<CounterpartyPaymentDto>.Failure(CounterpartyErrors.PaymentChanged);
                }

                return ApplicationResult<CounterpartyPaymentDto>.Success(
                    ToDto(cardPayment, cardRemainder.Total));
            }

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
                requestScopedId ?? Guid.NewGuid(),
                userId,
                counterparty,
                account!,
                command.Direction,
                amount,
                command.PaymentDate,
                command.Description,
                cardSettlement);
            var remainder = await DiscountRemainderAsync(command, payment, userId, cancellationToken);
            if (remainder.Error is ApplicationError remainderError)
            {
                return ApplicationResult<CounterpartyPaymentDto>.Failure(remainderError);
            }

            if (!await repository.AddPaymentAsync(payment, cardSettlement, cancellationToken, remainder.Charges))
            {
                return await ReplayAsync(payment.Id, userId, command, cancellationToken)
                    ?? ApplicationResult<CounterpartyPaymentDto>.Failure(CounterpartyErrors.PaymentChanged);
            }

            return ApplicationResult<CounterpartyPaymentDto>.Success(ToDto(payment, remainder.Total));
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

    internal const string IdPurpose = "counterparty-payment";

    /// <summary>
    /// Aynı istek kimliğiyle yazılmış tahsilat varsa onu döner. Kimlik başka bir
    /// kişinin tahsilatına aitse istek reddedilir: istemci kimliği yeniden
    /// kullanmış demektir ve ilk kaydı dönmek yanlış kişiyi gösterirdi.
    /// </summary>
    private async Task<ApplicationResult<CounterpartyPaymentDto>?> ReplayAsync(
        Guid paymentId,
        Guid userId,
        CreateCounterpartyPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var existing = await repository.FindOwnedPaymentAsync(paymentId, userId, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        return existing.CounterpartyId == command.CounterpartyId
            ? ApplicationResult<CounterpartyPaymentDto>.Success(ToDto(existing))
            : ApplicationResult<CounterpartyPaymentDto>.Failure(
                CounterpartyErrors.Validation("Client request id was already used for another payment."));
    }

    internal static CounterpartyPaymentDto ToDto(CounterpartyPayment payment, decimal remainderDiscount = 0m) => new(
        payment.Id,
        payment.CounterpartyId,
        payment.AccountId,
        payment.Direction,
        payment.Amount.Amount,
        payment.Amount.Currency,
        payment.PaymentDate,
        payment.Description,
        payment.IsCancelled,
        payment.PosSettlementId,
        payment.CreditCardId,
        remainderDiscount);
}

/// <summary>
/// Borçlandırmayı iptal eder: tanınan gelir/gider ve cari bakiye birlikte geri
/// alınır, çünkü ikisi de aynı kaydın türevi.
/// </summary>
/// <remarks>
/// Bir gün sonunda sayılan veresiye satış tek başına iptal edilemez: gün sonu
/// onu girilen nakitten düşmüştür ve iptal günün gelirini sessizce eksiltirdi.
/// Önce gün sonu geri alınır.
/// </remarks>
public sealed class CancelCounterpartyChargeUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    IDayCloseCountReader dayCloseCountReader,
    TimeProvider timeProvider)
{
    /// <param name="withPaidNow">
    /// <c>true</c>: satışla birlikte yazılmış peşin ya da avans tutarı da iptal
    /// olur (yanlış girilmiş kayıt). <c>false</c>: tutar kişinin hesabında kalır
    /// (iş iptal oldu, para alınmıştı); varsayılan budur.
    /// </param>
    public async Task<ApplicationResult<CounterpartyChargeDto>> ExecuteAsync(
        Guid chargeId,
        CancellationToken cancellationToken = default,
        bool withPaidNow = false)
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

        if (!charge.IsCancelled &&
            await dayCloseCountReader.IsCountedAsync(
                userId, DayCloseRecordKind.CounterpartyCharge, charge.Id, cancellationToken))
        {
            return ApplicationResult<CounterpartyChargeDto>.Failure(
                CounterpartyErrors.ChargeDayCloseCounted);
        }

        // Birlikte yazılmış tutar da iptal edilecekse onun kilitleri de sınanır;
        // ikisi tek yazmada iptal olur, yarım kalmaz.
        var cancelledAt = timeProvider.GetUtcNow().ToUniversalTime();
        if (withPaidNow)
        {
            var paidNow = await repository.FindPaidWithChargeAsync(charge.Id, userId, cancellationToken);
            foreach (var payment in paidNow)
            {
                if (await dayCloseCountReader.IsCountedAsync(
                        userId, DayCloseRecordKind.CounterpartyPayment, payment.Id, cancellationToken))
                {
                    return ApplicationResult<CounterpartyChargeDto>.Failure(
                        CounterpartyErrors.PaymentDayCloseCounted);
                }
            }

            foreach (var payment in paidNow)
            {
                payment.Cancel(cancelledAt);
            }
        }

        // İptal idempotent: ikinci istek aynı sonucu döner, ilk iptalin zaman
        // damgasını değiştirmez.
        charge.Cancel(cancelledAt);
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

        // Bağlı etkiler birlikte geri alınır (ADR 0021 İ7): ödemeyle yazılan
        // "kalanı indirim say" indirimleri de geri alınır, aynı yazmada.
        foreach (var charge in await repository.FindChargesDiscountedByPaymentAsync(
                     payment.Id, userId, cancellationToken))
        {
            if (charge.IsCancelled)
            {
                continue;
            }

            foreach (var discount in charge.Discounts.Where(
                         item => item.PaymentId == payment.Id && !item.IsCancelled).ToArray())
            {
                charge.ReverseDiscount(discount.Id, cancelledAtUtc);
            }
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
