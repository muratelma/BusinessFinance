using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.CreditCards;

public sealed class CreateInstallmentPlanUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository cardRepository,
    ICategoryRepository categoryRepository,
    IInstallmentPlanRepository planRepository)
{
    public async Task<ApplicationResult<InstallmentPlanDto>> ExecuteAsync(
        CreateInstallmentPlanCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<InstallmentPlanDto>.Failure(CreditCardErrors.AuthenticationRequired);
        }

        var existing = await planRepository.FindByClientRequestIdAsync(
            userId, command.ClientRequestId, false, cancellationToken);
        if (existing is not null)
        {
            return ApplicationResult<InstallmentPlanDto>.Success(ToDto(existing));
        }

        var card = await cardRepository.FindOwnedByIdAsync(
            command.CreditCardId, userId, false, cancellationToken);
        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (card is null)
        {
            return ApplicationResult<InstallmentPlanDto>.Failure(
                CreditCardErrors.NotFound(command.CreditCardId));
        }
        if (category is null)
        {
            return ApplicationResult<InstallmentPlanDto>.Failure(
                CreditCardErrors.Validation("An active expense category is required."));
        }

        if (TransactionScopeResolution.Resolve(
                command.Scope,
                card.DefaultScope,
                category.DefaultScope) is not TransactionScope scope)
        {
            return ApplicationResult<InstallmentPlanDto>.Failure(
                CreditCardErrors.ScopeUnresolved);
        }

        try
        {
            var plan = new InstallmentPlan(
                Guid.NewGuid(),
                userId,
                card,
                category,
                command.ClientRequestId,
                new Money(command.TotalAmount, command.Currency),
                scope,
                command.InstallmentCount,
                command.FirstInstallmentDate,
                command.Description);
            await planRepository.AddAsync(plan, cancellationToken);
            return ApplicationResult<InstallmentPlanDto>.Success(ToDto(plan));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<InstallmentPlanDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<InstallmentPlanDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }
    }

    internal static InstallmentPlanDto ToDto(InstallmentPlan plan) => new(
        plan.Id,
        plan.CreditCardId,
        plan.CategoryId,
        plan.ClientRequestId,
        plan.TotalAmount.Amount,
        plan.TotalAmount.Currency,
        plan.Scope,
        plan.InstallmentCount,
        plan.FirstInstallmentDate,
        plan.Description,
        plan.Items.OrderBy(item => item.Sequence).Select(item => new InstallmentItemDto(
            item.Id,
            item.Sequence,
            item.Amount.Amount,
            item.Amount.Currency,
            item.ScheduledDate,
            item.IsRealized,
            item.CreditCardChargeId,
            item.RealizedAtUtc)).ToArray());
}

public sealed class ListInstallmentPlansUseCase(
    ICurrentUser currentUser,
    IInstallmentPlanRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<InstallmentPlanDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<InstallmentPlanDto>>.Failure(
                CreditCardErrors.AuthenticationRequired);
        }

        var plans = await repository.ListAsync(userId, cancellationToken);
        return ApplicationResult<IReadOnlyList<InstallmentPlanDto>>.Success(
            plans.Select(CreateInstallmentPlanUseCase.ToDto).ToArray());
    }
}

public sealed class RealizeInstallmentUseCase(
    ICurrentUser currentUser,
    IInstallmentPlanRepository planRepository,
    ICreditCardRepository cardRepository,
    ICategoryRepository categoryRepository,
    ICardChargeRepository chargeRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<CardChargeDto>> ExecuteAsync(
        RealizeInstallmentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CardChargeDto>.Failure(CreditCardErrors.AuthenticationRequired);
        }

        var plan = await planRepository.FindOwnedByIdAsync(
            command.InstallmentPlanId, userId, true, cancellationToken);
        if (plan is null)
        {
            return ApplicationResult<CardChargeDto>.Failure(
                CreditCardErrors.InstallmentPlanNotFound(command.InstallmentPlanId));
        }

        InstallmentItem item;
        try
        {
            item = plan.GetItem(command.Sequence);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return ApplicationResult<CardChargeDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }

        if (item.CreditCardChargeId is Guid existingChargeId)
        {
            var existingCharge = await chargeRepository.FindOwnedByIdAsync(
                existingChargeId, userId, false, cancellationToken);
            if (existingCharge is null)
            {
                throw new InvalidOperationException("Realized installment charge was not found.");
            }
            return ApplicationResult<CardChargeDto>.Success(
                CreateCardChargeUseCase.ToDto(existingCharge));
        }

        var card = await cardRepository.FindOwnedByIdAsync(
            plan.CreditCardId, userId, false, cancellationToken);
        var category = await categoryRepository.FindOwnedByIdAsync(
            plan.CategoryId, userId, cancellationToken);
        if (card is null || category is null)
        {
            return ApplicationResult<CardChargeDto>.Failure(
                CreditCardErrors.Validation("Plan card or category is unavailable."));
        }

        var debt = await cardRepository.CalculateCurrentDebtAsync(
            card.Id, userId, cancellationToken);
        if (item.Amount.Amount > card.Limit.Amount - debt)
        {
            return ApplicationResult<CardChargeDto>.Failure(CreditCardErrors.LimitExceeded);
        }

        try
        {
            var charge = new CreditCardCharge(
                Guid.NewGuid(),
                userId,
                card,
                category,
                item.Amount,
                // Kapsam plandan gelir, gerçekleşme anında yeniden türetilmez:
                // aynı plan farklı aylarda farklı kapsam üretemez.
                plan.Scope,
                item.ScheduledDate,
                BuildDescription(plan, item));
            item.Realize(charge.Id, timeProvider.GetUtcNow());
            await planRepository.RealizeAsync(item, charge, cancellationToken);
            return ApplicationResult<CardChargeDto>.Success(
                CreateCardChargeUseCase.ToDto(charge));
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

    /// <summary>
    /// Gerçekleşen taksitin kart harcamasına yazılacak açıklaması.
    /// </summary>
    /// <remarks>
    /// Bu metin kullanıcının ekranında göründüğü hâliyle saklanıyor, o yüzden
    /// Türkçe. Sunucunun kullanıcı cümlesi üretmemesi kuralının istisnası
    /// değil de sınırı: burada üretilen şey bir etiket değil, harcamanın
    /// kalıcı açıklaması — kullanıcı kendi yazmadığı için sunucu dolduruyor.
    /// </remarks>
    private static string BuildDescription(InstallmentPlan plan, InstallmentItem item)
    {
        var prefix = $"Taksit {item.Sequence}/{plan.InstallmentCount}";
        var description = plan.Description is null ? prefix : $"{prefix}: {plan.Description}";
        return description.Length <= CreditCardCharge.MaximumDescriptionLength
            ? description
            : description[..CreditCardCharge.MaximumDescriptionLength];
    }
}
