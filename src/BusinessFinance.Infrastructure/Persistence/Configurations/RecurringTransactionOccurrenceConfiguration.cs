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
            table.HasCheckConstraint("CK_RecurringOccurrences_Amount", "[Amount] IS NULL OR [Amount] > 0");
            table.HasCheckConstraint("CK_RecurringOccurrences_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_RecurringOccurrences_Kind", "[Kind] IN (1, 2, 3)");
            table.HasCheckConstraint("CK_RecurringOccurrences_Scope", "[Scope] IN (1, 2)");
            table.HasCheckConstraint("CK_RecurringOccurrences_Status", "[Status] IN (1, 2, 3)");
            table.HasCheckConstraint(
                "CK_RecurringOccurrences_SourceType",
                "[SourceType] IS NULL OR [SourceType] IN (1, 2)");

            // At most one funding source. A pending item of a tax plan without a
            // source has none; the source is chosen when it is paid.
            table.HasCheckConstraint(
                "CK_RecurringOccurrences_Source",
                "([SourceType] IS NOT NULL AND [SourceType] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR " +
                "([SourceType] IS NOT NULL AND [SourceType] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL) OR " +
                "([SourceType] IS NULL AND [AccountId] IS NULL AND [CreditCardId] IS NULL)");

            // Planned rows carry no result. A realized row carries exactly one result
            // and an amount, and the result must be the type its source can produce:
            // an account occurrence becomes a budget transaction, a card occurrence a
            // card charge. A closed row (ADR 0018 T5) has no result of its own, only
            // the one tax payment that closed it.
            table.HasCheckConstraint(
                "CK_RecurringOccurrences_Realization",
                "([Status] = 1 AND [BudgetTransactionId] IS NULL AND " +
                "[CreditCardChargeId] IS NULL AND [RealizedAtUtc] IS NULL AND " +
                "[ClosedByTransactionId] IS NULL AND [ClosedByChargeId] IS NULL AND [ClosedAtUtc] IS NULL) OR " +
                "([Status] = 2 AND [RealizedAtUtc] IS NOT NULL AND [Amount] IS NOT NULL AND [SourceType] IS NOT NULL AND " +
                "[ClosedByTransactionId] IS NULL AND [ClosedByChargeId] IS NULL AND [ClosedAtUtc] IS NULL AND " +
                "(([SourceType] = 1 AND [BudgetTransactionId] IS NOT NULL AND [CreditCardChargeId] IS NULL) OR " +
                "([SourceType] = 2 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NOT NULL))) OR " +
                "([Status] = 3 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NULL AND " +
                "[RealizedAtUtc] IS NULL AND [ClosedAtUtc] IS NOT NULL AND " +
                "(([ClosedByTransactionId] IS NOT NULL AND [ClosedByChargeId] IS NULL) OR " +
                "([ClosedByTransactionId] IS NULL AND [ClosedByChargeId] IS NOT NULL)))");
        });

        builder.HasKey(occurrence => occurrence.Id);
        builder.HasAlternateKey(occurrence => new { occurrence.UserId, occurrence.Id });
        builder.Property(occurrence => occurrence.OccurrenceKey)
            .HasMaxLength(RecurringTransactionOccurrence.OccurrenceKeyLength)
            .IsFixedLength();
        builder.Property(occurrence => occurrence.Kind).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(occurrence => occurrence.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(occurrence => occurrence.Status).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(occurrence => occurrence.SourceType).HasConversion<byte?>().HasColumnType("tinyint");
        builder.Property(occurrence => occurrence.ScheduledDate).HasColumnType("date");
        builder.Property(occurrence => occurrence.Description)
            .HasMaxLength(RecurringTransaction.MaximumDescriptionLength);
        builder.Property(occurrence => occurrence.RealizedAtUtc).HasColumnType("datetimeoffset");
        builder.Property(occurrence => occurrence.ClosedAtUtc).HasColumnType("datetimeoffset");
        builder.Property<byte[]>("Version").IsRequired().IsRowVersion();
        builder.Ignore(occurrence => occurrence.IsRealized);
        builder.Property(occurrence => occurrence.AmountValue).HasColumnName("Amount").HasPrecision(19, 4);
        builder.Property(occurrence => occurrence.Currency).HasConversion<byte>().HasColumnType("tinyint");
        builder.Ignore(occurrence => occurrence.Amount);

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

        // Not unique: one tax payment may close several items (ADR 0018 T5).
        builder.HasIndex(occurrence => new { occurrence.UserId, occurrence.ClosedByTransactionId })
            .HasFilter("[ClosedByTransactionId] IS NOT NULL")
            .HasDatabaseName("IX_RecurringOccurrences_UserId_ClosedByTransactionId");
        builder.HasIndex(occurrence => new { occurrence.UserId, occurrence.ClosedByChargeId })
            .HasFilter("[ClosedByChargeId] IS NOT NULL")
            .HasDatabaseName("IX_RecurringOccurrences_UserId_ClosedByChargeId");

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
        builder.HasOne<BudgetTransaction>().WithMany()
            .HasForeignKey(occurrence => new { occurrence.UserId, occurrence.ClosedByTransactionId })
            .HasPrincipalKey(transaction => new { transaction.UserId, transaction.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CreditCardCharge>().WithMany()
            .HasForeignKey(occurrence => new { occurrence.UserId, occurrence.ClosedByChargeId })
            .HasPrincipalKey(charge => new { charge.UserId, charge.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
