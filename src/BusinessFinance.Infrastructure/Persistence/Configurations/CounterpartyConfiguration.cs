using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class CounterpartyConfiguration : IEntityTypeConfiguration<Counterparty>
{
    public void Configure(EntityTypeBuilder<Counterparty> builder)
    {
        builder.ToTable("Counterparties");

        builder.HasKey(counterparty => counterparty.Id);

        // Hareketler karşı tarafa (UserId, Id) ile bağlanır; başka kullanıcının
        // karşı tarafına yazılan bir hareket veritabanı seviyesinde reddedilir.
        builder.HasAlternateKey(counterparty => new { counterparty.UserId, counterparty.Id });

        builder.Property(counterparty => counterparty.Name)
            .HasMaxLength(Counterparty.MaximumNameLength)
            .IsRequired();
        builder.Property(counterparty => counterparty.Note)
            .HasMaxLength(Counterparty.MaximumNoteLength);
        builder.Property(counterparty => counterparty.IsActive).IsRequired();

        // Aynı kişinin iki kez oluşmasını engelleyen teklik. Müşteri/tedarikçi
        // ayrımı yok: aynı ad tek kayıttır, yön hareketin kendisinde durur.
        builder.HasIndex(counterparty => new { counterparty.UserId, counterparty.Name })
            .IsUnique()
            .HasDatabaseName("UX_Counterparties_UserId_Name");
        builder.HasIndex(counterparty => new { counterparty.UserId, counterparty.IsActive, counterparty.Name })
            .HasDatabaseName("IX_Counterparties_UserId_IsActive_Name");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(counterparty => counterparty.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class CounterpartyChargeConfiguration : IEntityTypeConfiguration<CounterpartyCharge>
{
    public void Configure(EntityTypeBuilder<CounterpartyCharge> builder)
    {
        builder.ToTable("CounterpartyCharges", table =>
        {
            table.HasCheckConstraint("CK_CounterpartyCharges_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_CounterpartyCharges_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_CounterpartyCharges_Direction", "[Direction] IN (1, 2)");

            // Borçlandırma gelir/gider tanır, bu yüzden kapsam taşır (ADR 0013).
            table.HasCheckConstraint("CK_CounterpartyCharges_Scope", "[Scope] IN (1, 2)");
            table.HasCheckConstraint(
                "CK_CounterpartyCharges_VatRate",
                VatDetailsConfiguration.RateConstraint);
            table.HasCheckConstraint(
                "CK_CounterpartyCharges_VatAmount",
                VatDetailsConfiguration.AmountConstraint("Amount"));
        });

        builder.HasKey(charge => charge.Id);
        builder.HasAlternateKey(charge => new { charge.UserId, charge.Id });
        builder.Property(charge => charge.Direction).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(charge => charge.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(charge => charge.ChargeDate).HasColumnType("date");
        builder.Property(charge => charge.DueDate).HasColumnType("date");
        builder.Property(charge => charge.Description)
            .HasMaxLength(CounterpartyCharge.MaximumDescriptionLength);
        builder.Property(charge => charge.IsCancelled).IsRequired();
        builder.Property(charge => charge.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.OwnsOne(charge => charge.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.OwnsVat(charge => charge.Vat);

        // Cari bakiye sorgusunun okuduğu indeks: karşı taraf başına, iptal
        // edilmemiş satırlar. Yön sütunu indekste olduğu için alacak/borç
        // toplamları aynı taramadan çıkar.
        builder.HasIndex(charge => new
        { charge.UserId, charge.CounterpartyId, charge.IsCancelled, charge.Direction, charge.DueDate })
            .HasDatabaseName("IX_CounterpartyCharges_UserId_CounterpartyId_Cancelled_Direction");
        builder.HasIndex(charge => new { charge.UserId, charge.CategoryId, charge.ChargeDate })
            .HasDatabaseName("IX_CounterpartyCharges_UserId_CategoryId_Date");

        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(charge => charge.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Counterparty>().WithMany()
            .HasForeignKey(charge => new { charge.UserId, charge.CounterpartyId })
            .HasPrincipalKey(counterparty => new { counterparty.UserId, counterparty.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(charge => new { charge.UserId, charge.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class CounterpartyPaymentConfiguration : IEntityTypeConfiguration<CounterpartyPayment>
{
    public void Configure(EntityTypeBuilder<CounterpartyPayment> builder)
    {
        builder.ToTable("CounterpartyPayments", table =>
        {
            table.HasCheckConstraint("CK_CounterpartyPayments_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_CounterpartyPayments_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_CounterpartyPayments_Direction", "[Direction] IN (1, 2)");
        });

        // Tahsilat yalnız parayı taşır: ne kategori ne kapsam kolonu var
        // (ADR 0014). Kolonu eklemek, gelir/gider raporuna hiç girmeyen bir
        // kaydı bölünebilir gibi göstermek olurdu.
        builder.HasKey(payment => payment.Id);
        builder.HasAlternateKey(payment => new { payment.UserId, payment.Id });
        builder.Property(payment => payment.Direction).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(payment => payment.PaymentDate).HasColumnType("date");
        builder.Property(payment => payment.Description)
            .HasMaxLength(CounterpartyPayment.MaximumDescriptionLength);
        builder.Property(payment => payment.IsCancelled).IsRequired();
        builder.Property(payment => payment.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.OwnsOne(payment => payment.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.HasIndex(payment => new { payment.UserId, payment.CounterpartyId, payment.IsCancelled, payment.Direction })
            .HasDatabaseName("IX_CounterpartyPayments_UserId_CounterpartyId_Cancelled_Direction");
        builder.HasIndex(payment => new { payment.UserId, payment.AccountId, payment.PaymentDate })
            .HasDatabaseName("IX_CounterpartyPayments_UserId_AccountId_Date");

        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(payment => payment.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Counterparty>().WithMany()
            .HasForeignKey(payment => new { payment.UserId, payment.CounterpartyId })
            .HasPrincipalKey(counterparty => new { counterparty.UserId, counterparty.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(payment => new { payment.UserId, payment.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
