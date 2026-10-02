using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class PosDepositConfiguration : IEntityTypeConfiguration<PosDeposit>
{
    public void Configure(EntityTypeBuilder<PosDeposit> builder)
    {
        builder.ToTable("PosDeposits", table =>
        {
            table.HasCheckConstraint("CK_PosDeposits_DepositedAmount", "[DepositedAmount] > 0");
            table.HasCheckConstraint("CK_PosDeposits_Currency", "[Currency] = 1");
            // Kesinti ile onu yazan gider kaydı birlikte bulunur ya da hiç
            // bulunmaz; kesinti eksi olamaz (fazla yatan tutar reddedilir).
            table.HasCheckConstraint(
                "CK_PosDeposits_Deduction",
                "([DeductionAmount] = 0 AND [DeductionTransactionId] IS NULL) OR " +
                "([DeductionAmount] > 0 AND [DeductionTransactionId] IS NOT NULL)");
        });

        builder.HasKey(deposit => deposit.Id);
        builder.HasAlternateKey(deposit => new { deposit.UserId, deposit.Id });
        // Tahsilat yatışa günüyle birlikte bağlanır: tahsilattaki geçiş günü
        // yatışın gününden ayrışamaz (PosSettlementConfiguration).
        builder.HasAlternateKey(deposit => new { deposit.UserId, deposit.Id, deposit.DepositDate });
        builder.Property(deposit => deposit.DeductionAmount).HasPrecision(19, 4);
        builder.Property(deposit => deposit.DepositDate).HasColumnType("date");
        builder.Property(deposit => deposit.CreatedAtUtc).HasColumnType("datetimeoffset");
        builder.Property(deposit => deposit.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.Property(deposit => deposit.IsCancelled).IsRequired();

        // Beklenen tutar kolon değildir: yatan tutar ile kesintinin toplamıdır.
        builder.Ignore(deposit => deposit.ExpectedAmount);
        builder.Ignore(deposit => deposit.Currency);

        builder.OwnsOne(deposit => deposit.DepositedAmount, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName("DepositedAmount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.HasIndex(deposit => new { deposit.UserId, deposit.DepositDate })
            .HasDatabaseName("IX_PosDeposits_UserId_DepositDate");
        builder.HasIndex(deposit => new { deposit.UserId, deposit.AccountId })
            .HasDatabaseName("IX_PosDeposits_UserId_AccountId");
        // Bir gider kaydı en çok bir yatışın kesintisidir; birleşik akıştaki
        // köken sorgusu da bu indeksten gider.
        builder.HasIndex(deposit => new { deposit.UserId, deposit.DeductionTransactionId })
            .IsUnique()
            .HasFilter("[DeductionTransactionId] IS NOT NULL")
            .HasDatabaseName("UX_PosDeposits_UserId_DeductionTransactionId");

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(deposit => deposit.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(deposit => new { deposit.UserId, deposit.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BudgetTransaction>().WithMany()
            .HasForeignKey(deposit => new { deposit.UserId, deposit.DeductionTransactionId })
            .HasPrincipalKey(transaction => new { transaction.UserId, transaction.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
