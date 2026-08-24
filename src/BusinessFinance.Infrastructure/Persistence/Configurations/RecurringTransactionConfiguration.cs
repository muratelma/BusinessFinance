using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class RecurringTransactionConfiguration : IEntityTypeConfiguration<RecurringTransaction>
{
    public void Configure(EntityTypeBuilder<RecurringTransaction> builder)
    {
        builder.ToTable("RecurringTransactions", table =>
        {
            table.HasCheckConstraint("CK_RecurringTransactions_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_RecurringTransactions_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_RecurringTransactions_Kind", "[Kind] IN (1, 2, 3)");
            table.HasCheckConstraint("CK_RecurringTransactions_Scope", "[Scope] IN (1, 2)");
            table.HasCheckConstraint("CK_RecurringTransactions_Frequency", "[Frequency] IN (1, 2, 3, 4)");
            table.HasCheckConstraint("CK_RecurringTransactions_MonthEndBehavior", "[MonthEndBehavior] IN (1, 2)");
            table.HasCheckConstraint(
                "CK_RecurringTransactions_DateRange",
                "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
            table.HasCheckConstraint(
                "CK_RecurringTransactions_OccurrenceLimit",
                "[OccurrenceLimit] IS NULL OR [OccurrenceLimit] > 0");
            table.HasCheckConstraint(
                "CK_RecurringTransactions_GeneratedOccurrenceCount",
                "[GeneratedOccurrenceCount] >= 0 AND " +
                "([OccurrenceLimit] IS NULL OR [GeneratedOccurrenceCount] <= [OccurrenceLimit])");
            table.HasCheckConstraint("CK_RecurringTransactions_SourceType", "[SourceType] IN (1, 2)");

            // Exactly one funding source: an account (1) or a credit card (2).
            table.HasCheckConstraint(
                "CK_RecurringTransactions_Source",
                "([SourceType] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR " +
                "([SourceType] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL)");

            // A card cannot receive income; recurring income must be account sourced.
            table.HasCheckConstraint(
                "CK_RecurringTransactions_IncomeSource",
                "[Kind] <> 1 OR [SourceType] = 1");
        });

        builder.HasKey(recurring => recurring.Id);
        builder.HasAlternateKey(recurring => new { recurring.UserId, recurring.Id });
        builder.Property(recurring => recurring.Kind).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(recurring => recurring.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(recurring => recurring.SourceType).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(recurring => recurring.Frequency).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(recurring => recurring.MonthEndBehavior).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(recurring => recurring.StartDate).HasColumnType("date");
        builder.Property(recurring => recurring.EndDate).HasColumnType("date");
        builder.Property(recurring => recurring.OccurrenceLimit);
        builder.Property(recurring => recurring.NextOccurrenceDate).HasColumnType("date");
        builder.Property(recurring => recurring.Description)
            .HasMaxLength(RecurringTransaction.MaximumDescriptionLength);
        builder.OwnsOne(recurring => recurring.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.HasIndex(recurring => new
        {
            recurring.UserId,
            recurring.IsActive,
            recurring.NextOccurrenceDate
        })
            .HasDatabaseName("IX_RecurringTransactions_UserId_IsActive_NextDate");
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(recurring => recurring.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(recurring => new { recurring.UserId, recurring.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CreditCard>().WithMany()
            .HasForeignKey(recurring => new { recurring.UserId, recurring.CreditCardId })
            .HasPrincipalKey(card => new { card.UserId, card.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(recurring => new { recurring.UserId, recurring.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
