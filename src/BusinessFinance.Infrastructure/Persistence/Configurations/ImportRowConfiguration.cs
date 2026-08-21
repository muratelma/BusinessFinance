using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class ImportRowConfiguration : IEntityTypeConfiguration<ImportRow>
{
    public void Configure(EntityTypeBuilder<ImportRow> builder)
    {
        builder.ToTable("ImportRows", table =>
        {
            table.HasCheckConstraint("CK_ImportRows_RowNumber", "[RowNumber] >= 2");
            table.HasCheckConstraint("CK_ImportRows_Currency", "[Currency] = 1");
            table.HasCheckConstraint(
                "CK_ImportRows_SignedAmountRange",
                "[SignedAmount] IS NULL OR ([SignedAmount] >= -999999999999999.9999 AND [SignedAmount] <= 999999999999999.9999)");
            table.HasCheckConstraint("CK_ImportRows_Status", "[Status] IN (1, 2, 3, 4, 5, 6)");
            table.HasCheckConstraint(
                "CK_ImportRows_DuplicateReview",
                "([Status] IN (5, 6) AND [DuplicateTransactionId] IS NOT NULL AND [DuplicateReason] IS NOT NULL) OR " +
                "([Status] IN (1, 2) AND [DuplicateTransactionId] IS NULL AND [DuplicateReason] IS NULL) OR " +
                "([Status] IN (3, 4) AND (([DuplicateTransactionId] IS NULL AND [DuplicateReason] IS NULL) OR " +
                "([DuplicateTransactionId] IS NOT NULL AND [DuplicateReason] IS NOT NULL)))");
            table.HasCheckConstraint(
                "CK_ImportRows_DuplicateReason",
                "[DuplicateReason] IS NULL OR [DuplicateReason] IN (1, 2)");
            table.HasCheckConstraint(
                "CK_ImportRows_Validation",
                "([Status] = 1 AND [TransactionDate] IS NOT NULL AND [SignedAmount] IS NOT NULL AND [SignedAmount] <> 0 AND [ErrorMessage] IS NULL AND [AccountId] IS NULL AND [CategoryId] IS NULL AND [BudgetTransactionId] IS NULL) OR " +
                "([Status] = 2 AND [ErrorMessage] IS NOT NULL AND [AccountId] IS NULL AND [CategoryId] IS NULL AND [BudgetTransactionId] IS NULL) OR " +
                "([Status] = 3 AND [TransactionDate] IS NOT NULL AND [SignedAmount] IS NOT NULL AND [SignedAmount] <> 0 AND [ErrorMessage] IS NULL AND [AccountId] IS NOT NULL AND [CategoryId] IS NOT NULL AND [BudgetTransactionId] IS NULL) OR " +
                "([Status] = 4 AND [TransactionDate] IS NOT NULL AND [SignedAmount] IS NOT NULL AND [SignedAmount] <> 0 AND [ErrorMessage] IS NULL AND [AccountId] IS NOT NULL AND [CategoryId] IS NOT NULL AND [BudgetTransactionId] IS NOT NULL) OR " +
                "([Status] IN (5, 6) AND [TransactionDate] IS NOT NULL AND [SignedAmount] IS NOT NULL AND [SignedAmount] <> 0 AND [ErrorMessage] IS NULL AND [AccountId] IS NOT NULL AND [CategoryId] IS NOT NULL AND [BudgetTransactionId] IS NULL)");
        });
        builder.HasKey(row => row.Id);
        builder.Property(row => row.RawData).HasMaxLength(ImportRow.MaximumRawDataLength);
        builder.Property(row => row.TransactionDate).HasColumnType("date");
        builder.Property(row => row.SignedAmount).HasPrecision(19, 4);
        builder.Property(row => row.Currency).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(row => row.Description).HasMaxLength(BudgetTransaction.MaximumDescriptionLength);
        builder.Property(row => row.ExternalReference).HasMaxLength(ImportRow.MaximumExternalReferenceLength);
        builder.Property(row => row.Status).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(row => row.ErrorMessage).HasMaxLength(ImportRow.MaximumErrorLength);
        builder.Property(row => row.DuplicateReason).HasConversion<byte?>().HasColumnType("tinyint");
        builder.Property<byte[]>("Version").IsRequired().IsRowVersion();
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(row => new { row.UserId, row.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(row => new { row.UserId, row.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BudgetTransaction>().WithMany()
            .HasForeignKey(row => new { row.UserId, row.BudgetTransactionId })
            .HasPrincipalKey(transaction => new { transaction.UserId, transaction.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BudgetTransaction>().WithMany()
            .HasForeignKey(row => new { row.UserId, row.DuplicateTransactionId })
            .HasPrincipalKey(transaction => new { transaction.UserId, transaction.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(row => new { row.UserId, row.ImportBatchId, row.RowNumber })
            .IsUnique().HasDatabaseName("UX_ImportRows_UserId_BatchId_RowNumber");
    }
}
