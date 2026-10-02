using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.FinancialActivities;

/// <summary>
/// İşlem ayrıntısındaki "işlem sonrası bakiye": hareketin dokunduğu hesabın
/// bakiyesi ya da kartın borcu, o hareketten hemen sonra.
/// </summary>
/// <remarks>
/// Akışın tek sorgusuna eklenmez; yalnız ayrıntı açıldığında, o hareket için
/// okunur. İstemci bakiyeyi kendisi hesaplamaz.
/// </remarks>
public sealed class GetActivityBalancesUseCase(
    ICurrentUser currentUser,
    IActivityBalanceReader reader)
{
    public async Task<ApplicationResult<IReadOnlyList<ActivityBalanceAfter>>> ExecuteAsync(
        FinancialActivityKind kind,
        Guid activityId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<ActivityBalanceAfter>>.Failure(
                FinancialActivityErrors.AuthenticationRequired);
        }

        var balances = await reader.GetBalancesAfterAsync(
            userId, kind, activityId, cancellationToken);
        return balances is null
            ? ApplicationResult<IReadOnlyList<ActivityBalanceAfter>>.Failure(
                FinancialActivityErrors.NotFound(activityId))
            : ApplicationResult<IReadOnlyList<ActivityBalanceAfter>>.Success(balances);
    }
}
