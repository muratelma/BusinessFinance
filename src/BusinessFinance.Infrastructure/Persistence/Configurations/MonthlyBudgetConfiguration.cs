using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class MonthlyBudgetConfiguration : IEntityTypeConfiguration<MonthlyBudget>
{
    public void Configure(EntityTypeBuilder<MonthlyBudget> builder)
    {
        builder.ToTable("MonthlyBudgets", table =>
        {
            table.HasCheckConstraint(
                "CK_MonthlyBudgets_Year",
                $"[Year] BETWEEN {MonthlyBudget.MinimumYear} AND {MonthlyBudget.MaximumYear}");
            table.HasCheckConstraint("CK_MonthlyBudgets_Month", "[Month] BETWEEN 1 AND 12");
            table.HasCheckConstraint("CK_MonthlyBudgets_Limit", "[Limit] > 0");
            table.HasCheckConstraint("CK_MonthlyBudgets_Currency", "[Currency] = 1");
        });

        builder.HasKey(budget => budget.Id);
        builder.Ignore(budget => budget.PeriodStart);
        builder.Ignore(budget => budget.PeriodEnd);

        builder.OwnsOne(budget => budget.Limit, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName("Limit")
                .HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency")
                .HasConversion<byte>()
                .HasColumnType("tinyint");
        });

        builder.HasIndex(budget => new
        {
            budget.UserId,
            budget.CategoryId,
            budget.Year,
            budget.Month
        })
            .IsUnique()
            .HasDatabaseName("UX_MonthlyBudgets_UserId_CategoryId_Year_Month");

        builder.HasIndex(budget => new { budget.UserId, budget.Year, budget.Month })
            .HasDatabaseName("IX_MonthlyBudgets_UserId_Year_Month");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(budget => budget.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(budget => new { budget.UserId, budget.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
