using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class BudgetTransactionConfiguration : IEntityTypeConfiguration<BudgetTransaction>
{
    public void Configure(EntityTypeBuilder<BudgetTransaction> builder)
    {
        builder.ToTable("BudgetTransactions", table =>
        {
            table.HasCheckConstraint("CK_BudgetTransactions_Type", "[Type] IN (1, 2)");
            table.HasCheckConstraint("CK_BudgetTransactions_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_BudgetTransactions_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_BudgetTransactions_Scope", "[Scope] IN (1, 2)");
        });

        builder.HasKey(transaction => transaction.Id);
        builder.HasAlternateKey(transaction => new { transaction.UserId, transaction.Id });
        builder.Property(transaction => transaction.Type).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(transaction => transaction.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(transaction => transaction.TransactionDate).HasColumnType("date");
        builder.Property(transaction => transaction.Description)
            .HasMaxLength(BudgetTransaction.MaximumDescriptionLength);
        builder.Property(transaction => transaction.IsCancelled).IsRequired();
        builder.Property(transaction => transaction.CancelledAtUtc).HasColumnType("datetimeoffset");

        builder.OwnsOne(transaction => transaction.Amount, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName("Amount")
                .HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency")
                .HasConversion<byte>()
                .HasColumnType("tinyint");
        });

        builder.HasIndex(transaction => new { transaction.UserId, transaction.TransactionDate })
            .HasDatabaseName("IX_BudgetTransactions_UserId_TransactionDate");
        builder.HasIndex(transaction => new
        {
            transaction.UserId,
            transaction.AccountId,
            transaction.TransactionDate
        })
            .HasDatabaseName("IX_BudgetTransactions_UserId_AccountId_TransactionDate");
        builder.HasIndex(transaction => new
        {
            transaction.UserId,
            transaction.CategoryId,
            transaction.TransactionDate
        })
            .HasDatabaseName("IX_BudgetTransactions_UserId_CategoryId_TransactionDate");

        // Gün sonunun ürettiği gelirler; birleşik akıştaki köken ve "zaten
        // girilmiş kayıtlar" okuması bu indeksten gider.
        builder.HasIndex(transaction => new { transaction.UserId, transaction.DayCloseId })
            .HasFilter("[DayCloseId] IS NOT NULL")
            .HasDatabaseName("IX_BudgetTransactions_UserId_DayCloseId");

        // Elle girilen kayıtta boştur (ADR 0019 T1).
        builder.HasOne<DayClose>()
            .WithMany()
            .HasForeignKey(transaction => new { transaction.UserId, transaction.DayCloseId })
            .HasPrincipalKey(dayClose => new { dayClose.UserId, dayClose.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(transaction => transaction.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(transaction => new { transaction.UserId, transaction.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(transaction => new { transaction.UserId, transaction.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
