using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.ToTable("Transfers", table =>
        {
            table.HasCheckConstraint("CK_Transfers_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_Transfers_Currency", "[Currency] = 1");
            table.HasCheckConstraint(
                "CK_Transfers_DifferentAccounts",
                "[SourceAccountId] <> [DestinationAccountId]");
        });

        builder.HasKey(transfer => transfer.Id);
        builder.Property(transfer => transfer.TransferDate).HasColumnType("date");
        builder.Property(transfer => transfer.Description)
            .HasMaxLength(Transfer.MaximumDescriptionLength);
        builder.Property(transfer => transfer.IsCancelled).IsRequired();
        builder.Property(transfer => transfer.CancelledAtUtc).HasColumnType("datetimeoffset");

        builder.OwnsOne(transfer => transfer.Amount, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName("Amount")
                .HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency")
                .HasConversion<byte>()
                .HasColumnType("tinyint");
        });

        builder.HasIndex(transfer => new { transfer.UserId, transfer.TransferDate })
            .HasDatabaseName("IX_Transfers_UserId_TransferDate");
        builder.HasIndex(transfer => new { transfer.UserId, transfer.SourceAccountId })
            .HasDatabaseName("IX_Transfers_UserId_SourceAccountId");
        builder.HasIndex(transfer => new { transfer.UserId, transfer.DestinationAccountId })
            .HasDatabaseName("IX_Transfers_UserId_DestinationAccountId");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(transfer => transfer.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(transfer => new { transfer.UserId, transfer.SourceAccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(transfer => new { transfer.UserId, transfer.DestinationAccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
