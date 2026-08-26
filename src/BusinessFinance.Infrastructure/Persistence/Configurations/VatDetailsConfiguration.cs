using System.Linq.Expressions;
using BusinessFinance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

/// <summary>
/// KDV alanlarının beş kayıt türünde de aynı biçimde eşlenmesi.
/// </summary>
/// <remarks>
/// ADR 0016 gereği KDV taşınan bir bilgidir; sütunları <b>nullable</b>'dır ve
/// ikisi de boş olmak "KDV yok" demektir. Domain invariant'ı (en az biri dolu)
/// SQL'e taşınmaz: SQL'de "yok" hâli tam olarak iki boş sütundur.
/// </remarks>
internal static class VatDetailsConfiguration
{
    public static void OwnsVat<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, VatDetails?>> navigation)
        where TEntity : class
    {
        builder.OwnsOne(navigation!, (OwnedNavigationBuilder<TEntity, VatDetails> vat) =>
        {
            vat.Property(value => value.Rate)
                .HasColumnName("VatRate")
                .HasPrecision(5, VatDetails.RateDecimals);
            vat.Property(value => value.Amount)
                .HasColumnName("VatAmount")
                .HasPrecision(19, VatDetails.MoneyDecimals);
        });
    }

    /// <summary>
    /// KDV sütunlarının SQL tarafındaki ikinci kapısı: oran sınırı, negatif
    /// olmama ve tutarın kaydın tutarını aşmaması.
    /// </summary>
    public static string RateConstraint => "[VatRate] IS NULL OR ([VatRate] >= 0 AND [VatRate] < 1)";

    public static string AmountConstraint(string amountColumn) =>
        $"[VatAmount] IS NULL OR ([VatAmount] >= 0 AND [VatAmount] <= [{amountColumn}])";
}
