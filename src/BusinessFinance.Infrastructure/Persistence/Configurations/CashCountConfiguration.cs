using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class CashCountConfiguration : IEntityTypeConfiguration<CashCount>
{
    public void Configure(EntityTypeBuilder<CashCount> builder)
    {
        builder.ToTable("CashCounts", table =>
        {
            // Sayılan tutar sıfır olabilir: boş kasa da sayılır. Negatif
            // olamaz — kasada eksi nakit bulunmaz.
            table.HasCheckConstraint("CK_CashCounts_CountedAmount", "[CountedAmount] >= 0");
            table.HasCheckConstraint("CK_CashCounts_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_CashCounts_Scope", "[Scope] IN (1, 2)");
            // Düzeltme kaydı ile damgası birlikte bulunur ya da hiç bulunmaz.
            table.HasCheckConstraint(
                "CK_CashCounts_Adjustment",
                "([AdjustmentTransactionId] IS NULL AND [AdjustmentTransferId] IS NULL " +
                "AND [AdjustedAtUtc] IS NULL) OR " +
                "([AdjustmentTransactionId] IS NOT NULL AND [AdjustmentTransferId] IS NULL " +
                "AND [AdjustedAtUtc] IS NOT NULL) OR " +
                "([AdjustmentTransactionId] IS NULL AND [AdjustmentTransferId] IS NOT NULL " +
                "AND [AdjustedAtUtc] IS NOT NULL)");
        });

        builder.HasKey(count => count.Id);
        builder.HasAlternateKey(count => new { count.UserId, count.Id });
        builder.Property(count => count.CountedAmount).HasPrecision(19, 4);
        // Sayım anındaki beklenen bakiye: tarihsel gözlem, nullable (eski
        // sayımlarda bilinmiyor).
        builder.Property(count => count.ExpectedAtCount).HasPrecision(19, 4);
        builder.Property(count => count.Currency).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(count => count.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(count => count.CountDate).HasColumnType("date");
        builder.Property(count => count.CreatedAtUtc).HasColumnType("datetimeoffset");
        builder.Property(count => count.AdjustedAtUtc).HasColumnType("datetimeoffset");
        builder.Property(count => count.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.Property(count => count.Note).HasMaxLength(CashCount.MaximumNoteLength);
        builder.Property(count => count.IsCancelled).IsRequired();
        builder.Ignore(count => count.IsAdjusted);

        // Bir gün ve bir kasa için **tek** açık sayım olur: ikinci sayım
        // öncekini iptal eder. Domain kuralı burada SQL seviyesinde de
        // duruyor, çünkü iki eşzamanlı yazar aynı gün için iki açık sayım
        // bırakabilirdi ve hangisinin geçerli olduğu belirsizleşirdi.
        builder.HasIndex(count => new { count.UserId, count.AccountId, count.CountDate })
            .IsUnique()
            .HasFilter("[IsCancelled] = 0")
            .HasDatabaseName("UX_CashCounts_UserId_AccountId_CountDate_Open");
        builder.HasIndex(count => new { count.UserId, count.CountDate })
            .HasDatabaseName("IX_CashCounts_UserId_CountDate");

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(count => count.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(count => new { count.UserId, count.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        // Düzeltme kaydı sahiplik kapsamıyla bağlanır: başka kullanıcının
        // hareketi bir sayımı kapatamaz.
        builder.HasOne<BudgetTransaction>().WithMany()
            .HasForeignKey(count => new { count.UserId, count.AdjustmentTransactionId })
            .HasPrincipalKey(transaction => new { transaction.UserId, transaction.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        builder.HasOne<Transfer>().WithMany()
            .HasForeignKey(count => new { count.UserId, count.AdjustmentTransferId })
            .HasPrincipalKey(transfer => new { transfer.UserId, transfer.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
