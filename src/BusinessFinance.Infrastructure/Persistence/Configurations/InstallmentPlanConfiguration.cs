using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class InstallmentPlanConfiguration : IEntityTypeConfiguration<InstallmentPlan>
{
    public void Configure(EntityTypeBuilder<InstallmentPlan> builder)
    {
        builder.ToTable("InstallmentPlans", table =>
        {
            table.HasCheckConstraint("CK_InstallmentPlans_TotalAmount", "[TotalAmount] > 0");
            table.HasCheckConstraint("CK_InstallmentPlans_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_InstallmentPlans_Scope", "[Scope] IN (1, 2)");
            table.HasCheckConstraint(
                "CK_InstallmentPlans_Count",
                $"[InstallmentCount] BETWEEN {InstallmentPlan.MinimumInstallmentCount} AND {InstallmentPlan.MaximumInstallmentCount}");
        });

        builder.HasKey(plan => plan.Id);
        builder.Property(plan => plan.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.HasAlternateKey(plan => new { plan.UserId, plan.Id });
        builder.Property(plan => plan.InstallmentCount).HasColumnType("tinyint");
        builder.Property(plan => plan.FirstInstallmentDate).HasColumnType("date");
        builder.Property(plan => plan.Description)
            .HasMaxLength(InstallmentPlan.MaximumDescriptionLength);
        builder.OwnsOne(plan => plan.TotalAmount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("TotalAmount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.HasIndex(plan => new { plan.UserId, plan.ClientRequestId })
            .IsUnique()
            .HasDatabaseName("UX_InstallmentPlans_UserId_ClientRequestId");
        builder.HasIndex(plan => new { plan.UserId, plan.CreditCardId, plan.FirstInstallmentDate })
            .HasDatabaseName("IX_InstallmentPlans_UserId_CardId_FirstDate");
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(plan => plan.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CreditCard>().WithMany()
            .HasForeignKey(plan => new { plan.UserId, plan.CreditCardId })
            .HasPrincipalKey(card => new { card.UserId, card.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(plan => new { plan.UserId, plan.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(plan => plan.Items).WithOne()
            .HasForeignKey(item => new { item.UserId, item.InstallmentPlanId })
            .HasPrincipalKey(plan => new { plan.UserId, plan.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(plan => plan.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class InstallmentItemConfiguration : IEntityTypeConfiguration<InstallmentItem>
{
    public void Configure(EntityTypeBuilder<InstallmentItem> builder)
    {
        builder.ToTable("InstallmentItems", table =>
        {
            table.HasCheckConstraint("CK_InstallmentItems_Sequence", "[Sequence] > 0");
            table.HasCheckConstraint("CK_InstallmentItems_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_InstallmentItems_Currency", "[Currency] = 1");
        });

        builder.HasKey(item => item.Id);
        builder.Property(item => item.Sequence).HasColumnType("tinyint");
        builder.Property(item => item.ScheduledDate).HasColumnType("date");
        builder.Property(item => item.RealizedAtUtc).HasColumnType("datetimeoffset");
        builder.Ignore(item => item.IsRealized);
        builder.OwnsOne(item => item.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.HasIndex(item => new { item.UserId, item.InstallmentPlanId, item.Sequence })
            .IsUnique()
            .HasDatabaseName("UX_InstallmentItems_UserId_PlanId_Sequence");
        builder.HasIndex(item => new { item.UserId, item.ScheduledDate })
            .HasDatabaseName("IX_InstallmentItems_UserId_ScheduledDate");
        builder.HasIndex(item => new { item.UserId, item.CreditCardChargeId })
            .IsUnique()
            .HasFilter("[CreditCardChargeId] IS NOT NULL")
            .HasDatabaseName("UX_InstallmentItems_UserId_ChargeId");
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CreditCardCharge>().WithMany()
            .HasForeignKey(item => new { item.UserId, item.CreditCardChargeId })
            .HasPrincipalKey(charge => new { charge.UserId, charge.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
