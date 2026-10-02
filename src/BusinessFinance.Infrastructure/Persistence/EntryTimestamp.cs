using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Persistence;

/// <summary>
/// Kaydın veritabanına yazıldığı an: İşlemler'de gün içi sırayı ve "işlem
/// sonrası bakiye"yi belirleyen tek bilgi.
/// </summary>
/// <remarks>
/// <para>
/// Kullanıcıdan istenmez ve Domain'in bilgisi değildir: kaydın tarihi
/// kullanıcının seçtiği gündür, giriş anı ise kalıcılığın bir gözlemidir. Bu
/// yüzden bir gölge kolondur ve <c>SaveChanges</c> sırasında yazılır.
/// </para>
/// <para>
/// Bu kolondan önce yazılmış kayıtlarda <b>boştur</b>: ne zaman girildikleri
/// bilinmiyor ve uydurulmaz. Boş değer sıralamada günün en eskisi sayılır.
/// Yedekten geri yüklenen kayıt dosyadaki anı taşır; değer zaten doluysa
/// üzerine yazılmaz.
/// </para>
/// </remarks>
internal static class EntryTimestamp
{
    public const string PropertyName = "CreatedAtUtc";

    /// <summary>
    /// Kendi giriş anını taşımayan finansal kayıtlar. POS tahsilatı, yatış,
    /// yükümlülük ve kapanışı ile borç taksidi bu bilgiyi zaten taşıyor.
    /// </summary>
    private static readonly Type[] StampedTypes =
    [
        typeof(BudgetTransaction),
        typeof(Transfer),
        typeof(CreditCardCharge),
        typeof(CreditCardPayment),
        typeof(CounterpartyCharge),
        typeof(CounterpartyPayment),
        typeof(DebtAgreement),
    ];

    public static void Configure(ModelBuilder builder)
    {
        foreach (var type in StampedTypes)
        {
            builder.Entity(type)
                .Property<DateTimeOffset?>(PropertyName)
                .HasColumnType("datetimeoffset");
        }
    }

    public static void Stamp(DbContext context)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added ||
                Array.IndexOf(StampedTypes, entry.Metadata.ClrType) < 0)
            {
                continue;
            }

            var property = entry.Property(PropertyName);
            property.CurrentValue ??= now;
        }
    }
}
