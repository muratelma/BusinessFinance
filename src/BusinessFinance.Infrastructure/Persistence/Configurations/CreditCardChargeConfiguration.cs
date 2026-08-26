using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class CreditCardChargeConfiguration : IEntityTypeConfiguration<CreditCardCharge>
{
    public void Configure(EntityTypeBuilder<CreditCardCharge> builder)
    {
        builder.ToTable("CreditCardCharges", table =>
        {
            table.HasCheckConstraint("CK_CreditCardCharges_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_CreditCardCharges_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_CreditCardCharges_Scope", "[Scope] IN (1, 2)");
            table.HasCheckConstraint(
                "CK_CreditCardCharges_VatRate",
                VatDetailsConfiguration.RateConstraint);
            table.HasCheckConstraint(
                "CK_CreditCardCharges_VatAmount",
                VatDetailsConfiguration.AmountConstraint("Amount"));
            table.HasCheckConstraint(
                "CK_CreditCardCharges_IsTaxDeductible",
                "[IsTaxDeductible] IS NULL OR [Scope] = 1");
        });

        builder.HasKey(charge => charge.Id);
        builder.Property(charge => charge.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.HasAlternateKey(charge => new { charge.UserId, charge.Id });
        builder.Property(charge => charge.ChargeDate).HasColumnType("date");
        builder.Property(charge => charge.Description)
            .HasMaxLength(CreditCardCharge.MaximumDescriptionLength);
        builder.Property(charge => charge.IsCancelled).IsRequired();
        builder.Property(charge => charge.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.OwnsOne(charge => charge.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.OwnsVat(charge => charge.Vat);
        builder.Property(charge => charge.IsTaxDeductible);

        builder.HasIndex(charge => new { charge.UserId, charge.CreditCardId, charge.ChargeDate })
            .HasDatabaseName("IX_CreditCardCharges_UserId_CardId_Date");
        builder.HasIndex(charge => new { charge.UserId, charge.CategoryId, charge.ChargeDate })
            .HasDatabaseName("IX_CreditCardCharges_UserId_CategoryId_Date");
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(charge => charge.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CreditCard>().WithMany()
            .HasForeignKey(charge => new { charge.UserId, charge.CreditCardId })
            .HasPrincipalKey(card => new { card.UserId, card.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(charge => new { charge.UserId, charge.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
