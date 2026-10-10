using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.CreditCards;

public sealed class CreateCreditCardUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository repository)
{
    public async Task<ApplicationResult<CreditCardDto>> ExecuteAsync(
        CreateCreditCardCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CreditCardDto>.Failure(CreditCardErrors.AuthenticationRequired);
        }

        var normalizedName = command.Name?.Trim() ?? string.Empty;
        if (await repository.ExistsByNameAsync(
                userId,
                normalizedName,
                null,
                cancellationToken))
        {
            return ApplicationResult<CreditCardDto>.Failure(CreditCardErrors.DuplicateName);
        }

        try
        {
            var card = new CreditCard(
                Guid.NewGuid(),
                userId,
                normalizedName,
                new Money(command.Limit, command.Currency),
                command.StatementClosingDay,
                command.PaymentDueDay,
                command.MinimumPaymentRate ?? CreditCard.DefaultMinimumPaymentRate,
                command.DefaultScope);
            await repository.AddAsync(card, cancellationToken);
            return ApplicationResult<CreditCardDto>.Success(ToDto(card, 0m));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CreditCardDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }
    }

    internal static CreditCardDto ToDto(CreditCard card, decimal currentDebt) => new(
        card.Id,
        card.Name,
        card.Limit.Amount,
        currentDebt,
        card.CalculateAvailableLimit(currentDebt),
        card.Limit.Currency,
        card.StatementClosingDay,
        card.PaymentDueDay,
        card.MinimumPaymentRate,
        card.IsActive,
        card.DefaultScope);
}

public sealed class GetCreditCardUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository repository)
{
    public async Task<ApplicationResult<CreditCardDto>> ExecuteAsync(
        Guid creditCardId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CreditCardDto>.Failure(CreditCardErrors.AuthenticationRequired);
        }

        var card = await repository.FindOwnedByIdAsync(
            creditCardId,
            userId,
            false,
            cancellationToken);
        if (card is null)
        {
            return ApplicationResult<CreditCardDto>.Failure(CreditCardErrors.NotFound(creditCardId));
        }

        var debt = await repository.CalculateCurrentDebtAsync(
            card.Id,
            userId,
            cancellationToken);
        return ApplicationResult<CreditCardDto>.Success(CreateCreditCardUseCase.ToDto(card, debt));
    }
}

public sealed class ListCreditCardsUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<CreditCardDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<CreditCardDto>>.Failure(
                CreditCardErrors.AuthenticationRequired);
        }

        var cards = await repository.ListAsync(userId, cancellationToken);
        var result = new List<CreditCardDto>(cards.Count);
        foreach (var card in cards)
        {
            var debt = await repository.CalculateCurrentDebtAsync(
                card.Id,
                userId,
                cancellationToken);
            result.Add(CreateCreditCardUseCase.ToDto(card, debt));
        }

        return ApplicationResult<IReadOnlyList<CreditCardDto>>.Success(result);
    }
}

public sealed class UpdateCreditCardUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository repository)
{
    public async Task<ApplicationResult<CreditCardDto>> ExecuteAsync(
        UpdateCreditCardCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CreditCardDto>.Failure(CreditCardErrors.AuthenticationRequired);
        }

        var card = await repository.FindOwnedByIdAsync(
            command.CreditCardId,
            userId,
            true,
            cancellationToken);
        if (card is null)
        {
            return ApplicationResult<CreditCardDto>.Failure(
                CreditCardErrors.NotFound(command.CreditCardId));
        }

        // Adı değişmeyen (ya da yalnız yazımı düzeltilen) kartta "bu ad var
        // mı?" sorulmaz: kural sıkılaşmadan önce açılmış aynı adlı ikinci kart
        // limiti ya da ekstre günü için düzenlenebilmelidir.
        var normalizedName = command.Name?.Trim() ?? string.Empty;
        var nameChanges = !string.Equals(
            NameKeys.Of(normalizedName), NameKeys.Of(card.Name), StringComparison.Ordinal);
        if (nameChanges && await repository.ExistsByNameAsync(
                userId,
                normalizedName,
                card.Id,
                cancellationToken))
        {
            return ApplicationResult<CreditCardDto>.Failure(CreditCardErrors.DuplicateName);
        }

        try
        {
            card.Update(
                normalizedName,
                new Money(command.Limit, command.Currency),
                command.StatementClosingDay,
                command.PaymentDueDay,

                // Oran gönderilmediğinde mevcut oran korunur; varsayılana
                // düşürmek, kullanıcının girdiği %40'ı adı değişti diye
                // sessizce %20'ye çevirirdi.
                command.MinimumPaymentRate ?? card.MinimumPaymentRate,
                command.IsActive);
            card.SetDefaultScope(command.DefaultScope);
            await repository.UpdateOwnedAsync(card, userId, cancellationToken);
            var debt = await repository.CalculateCurrentDebtAsync(
                card.Id,
                userId,
                cancellationToken);
            return ApplicationResult<CreditCardDto>.Success(
                CreateCreditCardUseCase.ToDto(card, debt));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CreditCardDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }
    }
}
