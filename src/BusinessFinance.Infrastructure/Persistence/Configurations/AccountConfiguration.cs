using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", table =>
        {
            table.HasCheckConstraint("CK_Accounts_Type", "[Type] IN (1, 2)");
            table.HasCheckConstraint("CK_Accounts_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_Accounts_OpeningBalance", "[OpeningBalance] >= 0");
            table.HasCheckConstraint(
                "CK_Accounts_DefaultScope",
                "[DefaultScope] IS NULL OR [DefaultScope] IN (1, 2)");
        });

        builder.HasKey(account => account.Id);
        builder.HasAlternateKey(account => new { account.UserId, account.Id });

        builder.Property(account => account.Name)
            .HasMaxLength(Account.MaximumNameLength)
            .IsRequired();
        builder.Property(account => account.Type).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(account => account.DefaultScope).HasConversion<byte?>().HasColumnType("tinyint");
        builder.Property(account => account.Currency).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(account => account.OpeningBalance).HasPrecision(19, 4);
        builder.Property(account => account.IsActive).IsRequired();

        // Ad anahtarı uygulamada hesaplanır (`NameKeys`) ve ikili
        // karşılaştırılır; veritabanının harf kuralı (Türkçe İ/i ve I/ı
        // çiftlerini ayrı sayıyordu) teklik kararına karışmaz. Uzunluk, eski
        // aynı adlı ikinci kaydın ayırt edici ekini de taşır.
        builder.Property(account => account.NameKey)
            .HasMaxLength(Account.MaximumNameLength + NameKeys.ApartSuffixAllowance)
            .UseCollation("Latin1_General_100_BIN2")
            .IsRequired();

        builder.HasIndex(account => new { account.UserId, account.NameKey })
            .IsUnique()
            .HasDatabaseName("UX_Accounts_UserId_NameKey");
        builder.HasIndex(account => new { account.UserId, account.IsActive, account.Type, account.Name })
            .HasDatabaseName("IX_Accounts_UserId_IsActive_Type_Name");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(account => account.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
