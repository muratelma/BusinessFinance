using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

/// <summary>
/// Kullanıcının son yazdığı bir kategori, yazdığı kapsamla birlikte.
/// </summary>
/// <remarks>
/// Çift olarak taşınır: iki kapsama açık kategori iki ayrı satırdır
/// (`Faiz geliri · İşletme`, `Faiz geliri · Şahsi`) ve seçici ikisini birden
/// seçer (kullanıcı kararı, 28 Ekim 2026).
/// </remarks>
public sealed record RecentCategoryDto(Guid CategoryId, TransactionScope Scope);

/// <summary>
/// Kategori seçicinin `Son kullanılanlar` bölümünü okur.
/// </summary>
/// <remarks>
/// <see cref="ICategoryRepository"/> içinde değil, çünkü cevap kategorinin
/// kendi tablosundan değil, gelir ve gider yazan kayıtlardan okunur.
/// </remarks>
public interface IRecentCategoryReader
{
    /// <summary>
    /// <paramref name="type"/> türündeki etkin kategorilerden en yeni
    /// <paramref name="take"/> çift; en yeni önce.
    /// </summary>
    /// <remarks>
    /// Yalnız kullanıcının kategoriyi kendisinin seçtiği kayıtlar sayılır:
    /// transfer masrafı, yatış kesintisi, POS komisyonu ve borç faizi bir
    /// kaydın parçasıdır, seçilmiş bir kategori değildir. İptal edilmiş kayıt
    /// sayılmaz.
    /// </remarks>
    Task<IReadOnlyList<RecentCategoryDto>> ListAsync(
        Guid userId,
        CategoryType type,
        int take,
        CancellationToken cancellationToken);
}

public sealed record ListRecentCategoriesQuery(CategoryType Type);

public sealed class ListRecentCategoriesUseCase(
    ICurrentUser currentUser,
    IRecentCategoryReader reader)
{
    /// <summary>Seçicinin gösterdiği en çok satır (teslim: dört).</summary>
    public const int Limit = 4;

    public async Task<ApplicationResult<IReadOnlyList<RecentCategoryDto>>> ExecuteAsync(
        ListRecentCategoriesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<RecentCategoryDto>>.Failure(
                CategoryErrors.AuthenticationRequired);
        }

        var items = await reader.ListAsync(userId, query.Type, Limit, cancellationToken);
        return ApplicationResult<IReadOnlyList<RecentCategoryDto>>.Success(items);
    }
}
