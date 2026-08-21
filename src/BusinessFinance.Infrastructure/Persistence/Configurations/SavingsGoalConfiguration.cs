using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class SavingsGoalConfiguration : IEntityTypeConfiguration<SavingsGoal>
{
    public void Configure(EntityTypeBuilder<SavingsGoal> builder)
    {
        builder.ToTable("SavingsGoals", table =>
        {
            table.HasCheckConstraint("CK_SavingsGoals_TargetAmount", "[TargetAmount] > 0");
            table.HasCheckConstraint("CK_SavingsGoals_Currency", "[Currency] = 'TRY'");
            table.HasCheckConstraint("CK_SavingsGoals_TrackingMode", "[TrackingMode] IN (1, 2)");
            table.HasCheckConstraint(
                "CK_SavingsGoals_ProgressSource",
                "([TrackingMode] = 1 AND [AccountId] IS NOT NULL) OR ([TrackingMode] = 2 AND [AccountId] IS NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.UserId, x.Id });
        builder.Property(x => x.Name).HasMaxLength(SavingsGoal.MaximumNameLength);
        builder.OwnsOne(x => x.TargetAmount, money =>
        {
            money.Property(x => x.Amount).HasColumnName("TargetAmount").HasPrecision(19, 4);
            money.Property(x => x.Currency).HasColumnName("Currency")
                .HasConversion<string>().HasMaxLength(3).IsUnicode(false);
        });
        builder.Property(x => x.TargetDate).HasColumnType("date");
        builder.Property(x => x.TrackingMode).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(x => x.Description).HasMaxLength(SavingsGoal.MaximumDescriptionLength);
        builder.Property(x => x.CreatedAtUtc);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(x => new { x.UserId, x.AccountId })
            .HasPrincipalKey(x => new { x.UserId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Contributions).WithOne()
            .HasForeignKey(x => new { x.UserId, x.SavingsGoalId })
            .HasPrincipalKey(x => new { x.UserId, x.Id })
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.UserId, x.TargetDate });
        builder.HasIndex(x => new { x.UserId, x.AccountId });
    }
}

internal sealed class SavingsGoalContributionConfiguration
    : IEntityTypeConfiguration<SavingsGoalContribution>
{
    public void Configure(EntityTypeBuilder<SavingsGoalContribution> builder)
    {
        builder.ToTable("SavingsGoalContributions", table =>
        {
            table.HasCheckConstraint("CK_SavingsGoalContributions_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_SavingsGoalContributions_Currency", "[Currency] = 'TRY'");
        });
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(x => x.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(x => x.Currency).HasColumnName("Currency")
                .HasConversion<string>().HasMaxLength(3).IsUnicode(false);
        });
        builder.Property(x => x.ContributionDate).HasColumnType("date");
        builder.Property(x => x.Note).HasMaxLength(SavingsGoalContribution.MaximumNoteLength);
        builder.Property(x => x.CreatedAtUtc);
        builder.HasIndex(x => new { x.UserId, x.SavingsGoalId, x.ClientRequestId }).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.ContributionDate });
    }
}
