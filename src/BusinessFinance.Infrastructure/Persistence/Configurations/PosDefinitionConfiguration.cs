using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class PosDefinitionConfiguration : IEntityTypeConfiguration<PosDefinition>
{
    public void Configure(EntityTypeBuilder<PosDefinition> builder)
    {
        builder.ToTable("PosDefinitions", table =>
        {
            table.HasCheckConstraint(
                "CK_PosDefinitions_CommissionRate",
                "[CommissionRate] >= 0 AND [CommissionRate] < 1");
            // Oran varsa komisyonun yazılacağı kategori de vardır.
            table.HasCheckConstraint(
                "CK_PosDefinitions_CommissionCategory",
                "[CommissionRate] = 0 OR [CommissionCategoryId] IS NOT NULL");
            table.HasCheckConstraint(
                "CK_PosDefinitions_TransferDays",
                $"[TransferDays] >= 0 AND [TransferDays] <= {PosDefinition.MaximumTransferDays}");
        });

        builder.HasKey(definition => definition.Id);
        builder.HasAlternateKey(definition => new { definition.UserId, definition.Id });
        builder.Property(definition => definition.Name)
            .HasMaxLength(PosDefinition.MaximumNameLength).IsRequired();
        // Ad anahtarı uygulamada hesaplanır (`NameKeys`) ve ikili
        // karşılaştırılır; veritabanının harf kuralı (Türkçe İ/i ve I/ı
        // çiftlerini ayrı sayıyordu) teklik kararına karışmaz. Uzunluk, eski
        // aynı adlı ikinci kaydın ayırt edici ekini de taşır.
        builder.Property(definition => definition.NameKey)
            .HasMaxLength(PosDefinition.MaximumNameLength + NameKeys.ApartSuffixAllowance)
            .UseCollation("Latin1_General_100_BIN2")
            .IsRequired();

        // 9 Ekim 2026'ya kadar POS adında hiç teklik yoktu: aynı adla iki POS
        // açılabiliyordu.
        builder.HasIndex(definition => new { definition.UserId, definition.NameKey })
            .IsUnique()
            .HasDatabaseName("UX_PosDefinitions_UserId_NameKey");
        builder.Property(definition => definition.CommissionRate)
            .HasPrecision(5, PosSettlement.RateDecimals);
        builder.Property(definition => definition.TransferDays).IsRequired();
        builder.Property(definition => definition.BusinessDaysOnly).IsRequired();
        builder.Property(definition => definition.IsActive).IsRequired();
        builder.Property(definition => definition.IsDefault).IsRequired();
        builder.Property(definition => definition.CreatedAtUtc).HasColumnType("datetimeoffset");

        builder.HasIndex(definition => new { definition.UserId, definition.IsActive })
            .HasDatabaseName("IX_PosDefinitions_UserId_IsActive");

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(definition => definition.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(definition => new { definition.UserId, definition.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(definition => new { definition.UserId, definition.SalesCategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(definition => new { definition.UserId, definition.CommissionCategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
