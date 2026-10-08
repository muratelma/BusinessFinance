using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

/// <summary>
/// Bir kategorinin hangi tarafta kullanıldığını söyler; kategorinin tarafı
/// daraltılırken ya da çevrilirken sorulur (ADR 0020 İ6).
/// </summary>
/// <remarks>
/// <see cref="ICategoryRepository"/> içinde değil, çünkü cevap kategorinin
/// kendi tablosundan değil onu taşıyan bütün kayıt türlerinden okunur ve yalnız
/// tek bir use case sorar.
/// </remarks>
public interface ICategoryUsageReader
{
    /// <summary>
    /// Kategorinin <paramref name="side"/> dışında kalan bir kaydı, planı ya da
    /// bütçesi var mı.
    /// </summary>
    /// <remarks>
    /// İptal edilmiş kayıt sayılmaz. POS'un satış ya da komisyon kategorisi
    /// olarak kullanım işletme tarafı sayılır: POS satışı işletmenindir.
    /// </remarks>
    Task<bool> IsUsedOutsideAsync(
        Guid categoryId,
        Guid userId,
        TransactionScope side,
        CancellationToken cancellationToken);
}
