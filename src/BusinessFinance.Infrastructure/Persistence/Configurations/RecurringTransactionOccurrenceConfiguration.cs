using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class RecurringTransactionOccurrenceConfiguration
    : IEntityTypeConfiguration<RecurringTransactionOccurrence>
{
    public void Configure(EntityTypeBuilder<RecurringTransactionOccurrence> builder)
    {
        builder.ToTable("RecurringTransactionOccurrences", table =>
        {
            table.HasCheckConstraint("CK_RecurringOccurrences_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_RecurringOccurrences_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_RecurringOccurrences_Kind", "[Kind] IN (1, 2, 3)");
            table.HasCheckConstraint("CK_RecurringOccurrences_Scope", "[Scope] IN (1, 2)");
            table.HasCheckConstraint("CK_RecurringOccurrences_Status", "[Status] IN (1, 2)");
            table.HasCheckConstraint("CK_RecurringOccurrences_SourceType", "[SourceType] IN (1, 2)");

            // Exactly one funding source, mirroring the parent schedule.
            table.HasCheckConstraint(
                "CK_RecurringOccurrences_Source",
                "([SourceType] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR " +
                "([SourceType] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL)");

            // Planned rows carry no result. A realized row carries exactly one result,
            // and it must be the result type its source can produce: an account
            // occurrence becomes a budget transaction, a card occurrence a card charge.
            table.HasCheckConstraint(
                "CK_RecurringOccurrences_Realization",
                "([Status] = 1 AND [BudgetTransactionId] IS NULL AND " +
                "[CreditCardChargeId] IS NULL AND [RealizedAtUtc] IS NULL) OR " +
                "([Status] = 2 AND [RealizedAtUtc] IS NOT NULL AND " +
                "(([SourceType] = 1 AND [BudgetTransactionId] IS NOT NULL AND [CreditCardChargeId] IS NULL) OR " +
                "([SourceType] = 2 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NOT NULL)))");
        });

        builder.HasKey(occurrence => occurrence.Id);
        builder.HasAlternateKey(occurrence => new { occurrence.UserId, occurrence.Id });
        builder.Property(occurrence => occurrence.OccurrenceKey)
            .HasMaxLength(RecurringTransactionOccurrence.OccurrenceKeyLength)
            .IsFixedLength();
        builder.Property(occurrence => occurrence.Kind).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(occurrence => occurrence.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(occurrence => occurrence.Status).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(occurrence => occurrence.SourceType).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(occurrence => occurrence.ScheduledDate).HasColumnType("date");
        builder.Property(occurrence => occurrence.Description)
            .HasMaxLength(RecurringTransaction.MaximumDescriptionLength);
        builder.Property(occurrence => occurrence.RealizedAtUtc).HasColumnType("datetimeoffset");
        builder.Property<byte[]>("Version").IsRequired().IsRowVersion();
        builder.Ignore(occurrence => occurrence.IsRealized);
        builder.OwnsOne(occurrence => occurrence.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.HasIndex(occurrence => new { occurrence.UserId, occurrence.OccurrenceKey })
            .IsUnique()
            .HasDatabaseName("UX_RecurringOccurrences_UserId_OccurrenceKey");
        builder.HasIndex(occurrence => new
        {
            occurrence.UserId,
            occurrence.Status,
            occurrence.ScheduledDate
        })
            .HasDatabaseName("IX_RecurringOccurrences_UserId_Status_ScheduledDate");
        builder.HasIndex(occurrence => new { occurrence.UserId, occurrence.BudgetTransactionId })
            .IsUnique()
            .HasFilter("[BudgetTransactionId] IS NOT NULL")
            .HasDatabaseName("UX_RecurringOccurrences_UserId_TransactionId");
        builder.HasIndex(occurrence => new { occurrence.UserId, occurrence.CreditCardChargeId })
            .IsUnique()
            .HasFilter("[CreditCardChargeId] IS NOT NULL")
            .HasDatabaseName("UX_RecurringOccurrences_UserId_ChargeId");

        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(occurrence => occurrence.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<RecurringTransaction>().WithMany()
            .HasForeignKey(occurrence => new { occurrence.UserId, occurrence.RecurringTransactionId })
            .HasPrincipalKey(recurring => new { recurring.UserId, recurring.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(occurrence => new { occurrence.UserId, occurrence.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(occurrence => new { occurrence.UserId, occurrence.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CreditCard>().WithMany()
            .HasForeignKey(occurrence => new { occurrence.UserId, occurrence.CreditCardId })
            .HasPrincipalKey(card => new { card.UserId, card.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BudgetTransaction>().WithMany()
            .HasForeignKey(occurrence => new { occurrence.UserId, occurrence.BudgetTransactionId })
            .HasPrincipalKey(transaction => new { transaction.UserId, transaction.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CreditCardCharge>().WithMany()
            .HasForeignKey(occurrence => new { occurrence.UserId, occurrence.CreditCardChargeId })
            .HasPrincipalKey(charge => new { charge.UserId, charge.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
