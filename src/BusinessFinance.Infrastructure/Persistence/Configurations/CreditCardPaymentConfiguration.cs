using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class CreditCardPaymentConfiguration : IEntityTypeConfiguration<CreditCardPayment>
{
    public void Configure(EntityTypeBuilder<CreditCardPayment> builder)
    {
        builder.ToTable("CreditCardPayments", table =>
        {
            table.HasCheckConstraint("CK_CreditCardPayments_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_CreditCardPayments_Currency", "[Currency] = 1");
        });

        builder.HasKey(payment => payment.Id);
        builder.Property(payment => payment.PaymentDate).HasColumnType("date");
        builder.Property(payment => payment.Description)
            .HasMaxLength(CreditCardPayment.MaximumDescriptionLength);
        builder.Property(payment => payment.IsCancelled).IsRequired();
        builder.Property(payment => payment.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.OwnsOne(payment => payment.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.HasIndex(payment => new { payment.UserId, payment.CreditCardId, payment.PaymentDate })
            .HasDatabaseName("IX_CreditCardPayments_UserId_CardId_Date");
        builder.HasIndex(payment => new { payment.UserId, payment.AccountId, payment.PaymentDate })
            .HasDatabaseName("IX_CreditCardPayments_UserId_AccountId_Date");
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(payment => payment.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CreditCard>().WithMany()
            .HasForeignKey(payment => new { payment.UserId, payment.CreditCardId })
            .HasPrincipalKey(card => new { card.UserId, card.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(payment => new { payment.UserId, payment.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
