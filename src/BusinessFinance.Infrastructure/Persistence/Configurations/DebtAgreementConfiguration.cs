using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class DebtAgreementConfiguration : IEntityTypeConfiguration<DebtAgreement>
{
    public void Configure(EntityTypeBuilder<DebtAgreement> builder)
    {
        builder.ToTable("DebtAgreements", table =>
        {
            table.HasCheckConstraint("CK_DebtAgreements_Direction", "[Direction] IN (1, 2)");
            table.HasCheckConstraint("CK_DebtAgreements_Principal", "[Principal] > 0");
            table.HasCheckConstraint("CK_DebtAgreements_TotalRepayment", "[TotalRepayment] >= [Principal]");
            table.HasCheckConstraint("CK_DebtAgreements_Currency", "[Currency] = 'TRY' AND [TotalCurrency] = [Currency]");
            table.HasCheckConstraint("CK_DebtAgreements_InterestRate", "[AnnualInterestRate] >= 0 AND [AnnualInterestRate] <= 1000");
            table.HasCheckConstraint("CK_DebtAgreements_DateRange", "[FirstDueDate] >= [StartDate]");
            table.HasCheckConstraint("CK_DebtAgreements_InstallmentCount", "[InstallmentCount] BETWEEN 1 AND 360");
            table.HasCheckConstraint("CK_DebtAgreements_SourceType", "[SourceType] IN (0, 1, 2, 3)");

            // Tam olarak bir kaynak. 0 = açılışı kayıtsız (yalnız bu ayrımdan
            // önce açılmış kayıtlar), 1 = nakit hesabı, 2 = gider kategorisi,
            // 3 = gelir kategorisi. İkisi birden dolarsa açılış hem parayı
            // hareket ettirir hem gider/gelir yazar ve aynı olay iki kez
            // sayılır.
            table.HasCheckConstraint(
                "CK_DebtAgreements_Source",
                "([SourceType] = 0 AND [OpeningAccountId] IS NULL AND [CategoryId] IS NULL) OR " +
                "([SourceType] = 1 AND [OpeningAccountId] IS NOT NULL AND [CategoryId] IS NULL) OR " +
                "([SourceType] IN (2, 3) AND [OpeningAccountId] IS NULL AND [CategoryId] IS NOT NULL)");

            // Kategorili kaynak yöne bağlıdır: borç (1) tüketir, alacak (2)
            // satar. Ters eşleşme parayı yanlış tarafa yazardı.
            table.HasCheckConstraint(
                "CK_DebtAgreements_DirectionalSource",
                "([Direction] = 1 AND [SourceType] <> 3) OR ([Direction] = 2 AND [SourceType] <> 2)");
        });
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.UserId, x.Id });
        builder.Property(x => x.CounterpartyName).HasMaxLength(DebtAgreement.MaximumNameLength);
        builder.Property(x => x.Direction).HasConversion<byte>().HasColumnType("tinyint");
        builder.OwnsOne(x => x.Principal, money =>
        {
            money.Property(x => x.Amount).HasColumnName("Principal").HasPrecision(19, 4);
            money.Property(x => x.Currency).HasColumnName("Currency").HasConversion<string>().HasMaxLength(3).IsUnicode(false);
        });
        builder.OwnsOne(x => x.TotalRepayment, money =>
        {
            money.Property(x => x.Amount).HasColumnName("TotalRepayment").HasPrecision(19, 4);
            money.Property(x => x.Currency).HasColumnName("TotalCurrency").HasConversion<string>().HasMaxLength(3).IsUnicode(false);
        });
        builder.Property(x => x.AnnualInterestRate).HasPrecision(7, 4);
        builder.Property(x => x.SourceType).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(x => x.InstallmentCount);

        // Sahiplik kapsamı foreign key'in kendisinde: (UserId, hesap/kategori).
        // Başka kullanıcının hesabına ya da kategorisine bağlanan bir borç
        // veritabanı seviyesinde yazılamaz.
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(x => new { x.UserId, x.OpeningAccountId })
            .HasPrincipalKey(x => new { x.UserId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(x => new { x.UserId, x.CategoryId })
            .HasPrincipalKey(x => new { x.UserId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.StartDate).HasColumnType("date");
        builder.Property(x => x.FirstDueDate).HasColumnType("date");
        builder.Property(x => x.Description).HasMaxLength(DebtAgreement.MaximumDescriptionLength);
        builder.HasMany(x => x.Installments).WithOne()
            .HasForeignKey(x => new { x.UserId, x.DebtAgreementId })
            .HasPrincipalKey(x => new { x.UserId, x.Id })
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.UserId, x.Direction });
    }
}

internal sealed class DebtInstallmentConfiguration : IEntityTypeConfiguration<DebtInstallment>
{
    public void Configure(EntityTypeBuilder<DebtInstallment> builder)
    {
        builder.ToTable("DebtInstallments", table =>
        {
            table.HasCheckConstraint("CK_DebtInstallments_Sequence", "[Sequence] >= 1");
            table.HasCheckConstraint("CK_DebtInstallments_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_DebtInstallments_Payment", "([PaymentAccountId] IS NULL AND [PaymentDate] IS NULL AND [PaidAtUtc] IS NULL) OR ([PaymentAccountId] IS NOT NULL AND [PaymentDate] IS NOT NULL AND [PaidAtUtc] IS NOT NULL)");

            // Ayrım ya tamamen yoktur (bu ayrımdan önce oluşmuş taksit) ya da
            // iki payı birden taşır ve toplamları taksit tutarını verir. Tek
            // payı dolu bir satır, faizi ya iki kez saydırır ya da kaybeder.
            table.HasCheckConstraint(
                "CK_DebtInstallments_Split",
                "([PrincipalPortion] IS NULL AND [InterestPortion] IS NULL) OR " +
                "([PrincipalPortion] IS NOT NULL AND [InterestPortion] IS NOT NULL AND " +
                "[PrincipalPortion] >= 0 AND [InterestPortion] >= 0 AND " +
                "[PrincipalPortion] + [InterestPortion] = [Amount])");
        });
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(x => x.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(x => x.Currency).HasColumnName("Currency").HasConversion<string>().HasMaxLength(3).IsUnicode(false);
        });
        builder.Property(x => x.DueDate).HasColumnType("date");
        builder.Property(x => x.PaymentDate).HasColumnType("date");
        builder.Property(x => x.PrincipalPortion).HasPrecision(19, 4);
        builder.Property(x => x.InterestPortion).HasPrecision(19, 4);
        builder.Property<byte[]>("Version").IsRequired().IsRowVersion();
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(x => new { x.UserId, x.PaymentAccountId })
            .HasPrincipalKey(x => new { x.UserId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.UserId, x.DueDate });
        builder.HasIndex(x => new { x.UserId, x.DebtAgreementId, x.Sequence }).IsUnique();
    }
}
