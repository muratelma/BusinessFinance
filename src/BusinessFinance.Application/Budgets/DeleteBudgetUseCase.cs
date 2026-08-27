using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Budgets;

/// <summary>
/// Aylık bütçeyi gerçekten siler.
/// </summary>
/// <remarks>
/// <para>
/// <b>Neden silme, iptal değil.</b> "Silme yerine iptal" kuralı para
/// hareketlerini korur: iptal edilen bir harcama olmuş bir olaydır ve
/// geçmişten düşürülemez. Bütçe bir olay değil, kullanıcının kendine koyduğu
/// bir <i>sınırdır</i>; hiçbir bakiyeyi, raporu veya net varlığı beslemez ve
/// silinmesi hiçbir tutarı değiştirmez. Yanlış kategoriye kurulmuş bir sınırı
/// pasifleştirmek, ondan kurtulmanın yolunu bırakmazdı: tekil indeks
/// (kullanıcı + kategori + yıl + ay) o kategoriyi ay boyunca kapatıyor, yani
/// kullanıcı doğrusunu da kuramıyordu.
/// </para>
/// </remarks>
public sealed class DeleteBudgetUseCase(
    ICurrentUser currentUser,
    IBudgetRepository repository)
{
    public async Task<ApplicationResult<Guid>> ExecuteAsync(
        DeleteBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<Guid>.Failure(BudgetErrors.AuthenticationRequired);
        }

        return await repository.DeleteOwnedAsync(command.BudgetId, userId, cancellationToken)
            ? ApplicationResult<Guid>.Success(command.BudgetId)
            : ApplicationResult<Guid>.Failure(BudgetErrors.NotFound(command.BudgetId));
    }
}
