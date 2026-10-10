using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
{
    public void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        builder.ToTable("CreditCards", table =>
        {
            table.HasCheckConstraint("CK_CreditCards_Limit", "[Limit] > 0");
            table.HasCheckConstraint("CK_CreditCards_Currency", "[Currency] = 1");
            table.HasCheckConstraint(
                "CK_CreditCards_DefaultScope",
                "[DefaultScope] IS NULL OR [DefaultScope] IN (1, 2)");
            table.HasCheckConstraint(
                "CK_CreditCards_StatementClosingDay",
                "[StatementClosingDay] BETWEEN 1 AND 28");
            table.HasCheckConstraint(
                "CK_CreditCards_PaymentDueDay",
                "[PaymentDueDay] BETWEEN 1 AND 28");
            table.HasCheckConstraint(
                "CK_CreditCards_MinimumPaymentRate",
                "[MinimumPaymentRate] BETWEEN 0 AND 100");
        });

        builder.HasKey(card => card.Id);
        builder.Property(card => card.DefaultScope).HasConversion<byte?>().HasColumnType("tinyint");
        builder.HasAlternateKey(card => new { card.UserId, card.Id });
        builder.Property(card => card.Name)
            .HasMaxLength(CreditCard.MaximumNameLength)
            .IsRequired();
        builder.Property(card => card.StatementClosingDay).HasColumnType("tinyint");
        builder.Property(card => card.PaymentDueDay).HasColumnType("tinyint");

        // Yüzde olarak saklanıyor (20.0000 = %20); borç faiz oranıyla aynı
        // ölçek ve aynı precision, ikisi farklı okunmasın diye.
        builder.Property(card => card.MinimumPaymentRate)
            .HasPrecision(7, 4)
            .HasDefaultValue(CreditCard.DefaultMinimumPaymentRate);
        builder.Property(card => card.IsActive).IsRequired();

        builder.OwnsOne(card => card.Limit, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName("Limit")
                .HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency")
                .HasConversion<byte>()
                .HasColumnType("tinyint");
        });

        // Ad anahtarı uygulamada hesaplanır (`NameKeys`) ve ikili
        // karşılaştırılır; veritabanının harf kuralı (Türkçe İ/i ve I/ı
        // çiftlerini ayrı sayıyordu) teklik kararına karışmaz. Uzunluk, eski
        // aynı adlı ikinci kaydın ayırt edici ekini de taşır.
        builder.Property(card => card.NameKey)
            .HasMaxLength(CreditCard.MaximumNameLength + NameKeys.ApartSuffixAllowance)
            .UseCollation("Latin1_General_100_BIN2")
            .IsRequired();

        builder.HasIndex(card => new { card.UserId, card.NameKey })
            .IsUnique()
            .HasDatabaseName("UX_CreditCards_UserId_NameKey");
        builder.HasIndex(card => new { card.UserId, card.IsActive, card.Name })
            .HasDatabaseName("IX_CreditCards_UserId_IsActive_Name");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(card => card.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
