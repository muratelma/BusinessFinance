using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Tek borç modeli (ADR 0021): eski <c>Obligations</c> ile <c>DebtAgreements</c>
/// tek tabloda, planı <c>PlanKind</c> kolonunda.
/// </summary>
internal sealed class DebtConfiguration : IEntityTypeConfiguration<Debt>
{
    public void Configure(EntityTypeBuilder<Debt> builder)
    {
        builder.ToTable("Debts", table =>
        {
            table.HasCheckConstraint("CK_Debts_Direction", "[Direction] IN (1, 2)");
            table.HasCheckConstraint("CK_Debts_Scope", "[Scope] IN (1, 2)");
            table.HasCheckConstraint("CK_Debts_Principal", "[Principal] > 0");
            table.HasCheckConstraint("CK_Debts_TotalRepayment", "[TotalRepayment] >= [Principal]");
            table.HasCheckConstraint("CK_Debts_Currency", "[Currency] = 'TRY' AND [TotalCurrency] = [Currency]");
            table.HasCheckConstraint("CK_Debts_InterestRate", "[AnnualInterestRate] >= 0 AND [AnnualInterestRate] <= 1000");
            table.HasCheckConstraint("CK_Debts_DateRange", "[FirstDueDate] IS NULL OR [FirstDueDate] >= [StartDate]");
            table.HasCheckConstraint("CK_Debts_PlanDueDate", "[PlanKind] = 3 OR [FirstDueDate] IS NOT NULL");
            table.HasCheckConstraint("CK_Debts_InstallmentCount", "[InstallmentCount] BETWEEN 1 AND 360");
            table.HasCheckConstraint("CK_Debts_PlanKind", "[PlanKind] IN (1, 2, 3)");
            table.HasCheckConstraint("CK_Debts_SinglePlan", "[PlanKind] = 2 OR [InstallmentCount] = 1");
            table.HasCheckConstraint("CK_Debts_SourceType", "[SourceType] IN (0, 1, 2, 3)");

            // Kaynak tam olarak bir yerdedir: açılışı bilinmeyende hiçbir yerde,
            // nakitte hesapta, mal ya da hizmette kategoride. Uygulamadan önceki
            // borç kaynağını bilir ama açılışı yazmaz: hesabı ve kategorisi boştur.
            table.HasCheckConstraint(
                "CK_Debts_Source",
                "([IsCarriedOver] = 1 AND [SourceType] IN (1, 2, 3) AND [OpeningAccountId] IS NULL AND [CategoryId] IS NULL) OR " +
                "([IsCarriedOver] = 0 AND [SourceType] = 0 AND [OpeningAccountId] IS NULL AND [CategoryId] IS NULL) OR " +
                "([IsCarriedOver] = 0 AND [SourceType] = 1 AND [OpeningAccountId] IS NOT NULL AND [CategoryId] IS NULL) OR " +
                "([IsCarriedOver] = 0 AND [SourceType] IN (2, 3) AND [OpeningAccountId] IS NULL AND [CategoryId] IS NOT NULL)");

            // Borç gider, alacak gelir kaynaklı olabilir; tersi değil.
            table.HasCheckConstraint(
                "CK_Debts_DirectionalSource",
                "([Direction] = 1 AND [SourceType] <> 3) OR ([Direction] = 2 AND [SourceType] <> 2)");
        });

        builder.HasKey(debt => debt.Id);
        builder.HasAlternateKey(debt => new { debt.UserId, debt.Id });
        builder.Property(debt => debt.Direction).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(debt => debt.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(debt => debt.PlanKind).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(debt => debt.SourceType).HasConversion<byte>().HasColumnType("tinyint");
        builder.OwnsOne(debt => debt.Principal, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Principal").HasPrecision(19, 4);
            money.Property(value => value.Currency).HasColumnName("Currency")
                .HasConversion<string>().HasMaxLength(3).IsUnicode(false);
        });
        builder.OwnsOne(debt => debt.TotalRepayment, money =>
        {
            money.Property(value => value.Amount).HasColumnName("TotalRepayment").HasPrecision(19, 4);
            money.Property(value => value.Currency).HasColumnName("TotalCurrency")
                .HasConversion<string>().HasMaxLength(3).IsUnicode(false);
        });
        builder.Property(debt => debt.AnnualInterestRate).HasPrecision(7, 4);
        builder.Property(debt => debt.StartDate).HasColumnType("date");
        builder.Property(debt => debt.FirstDueDate).HasColumnType("date");
        // Setter'ı olmayan özellik kendiliğinden eşlenmez.
        builder.Property(debt => debt.InstallmentCount);
        builder.Property(debt => debt.Description).HasMaxLength(Debt.MaximumDescriptionLength);
        builder.Property(debt => debt.IsCancelled).IsRequired();
        builder.Property(debt => debt.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.Property(debt => debt.IsCarriedOver).IsRequired();
        builder.Property(debt => debt.PlanRevision).IsRequired();
        // Kalan denetimi ile yazma aynı satırda sınanır (ADR 0021 İ6).
        builder.Property(debt => debt.Revision).IsRequired().IsConcurrencyToken();
        builder.Ignore(debt => debt.CurrentItems);
        builder.Ignore(debt => debt.ClosedAmount);
        builder.Ignore(debt => debt.BearsInterest);
        builder.Ignore(debt => debt.Status);
        builder.Ignore(debt => debt.RecognizedType);
        builder.Ignore(debt => debt.DueDate);
        builder.Ignore(debt => debt.Settlement);
        builder.Ignore(debt => debt.RemainingAmount);
        builder.Ignore(debt => debt.IsClosed);
        builder.Ignore(debt => debt.TotalInterest);
        builder.Ignore(debt => debt.InstallmentSplits);
        builder.Ignore(debt => debt.HasUnrecordedOpening);

        builder.HasMany(debt => debt.Items).WithOne()
            .HasForeignKey(item => new { item.UserId, item.DebtId })
            .HasPrincipalKey(debt => new { debt.UserId, debt.Id })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(debt => debt.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Ödemeden borca kaskad yok: SQL Server tahsisler için iki kaskad yolu
        // (borç → kalem → tahsis, borç → ödeme → tahsis) kabul etmez. Borç zaten
        // silinmez, iptal edilir.
        builder.HasMany(debt => debt.Payments).WithOne()
            .HasForeignKey(payment => new { payment.UserId, payment.DebtId })
            .HasPrincipalKey(debt => new { debt.UserId, debt.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(debt => debt.Payments).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(debt => debt.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(debt => new { debt.UserId, debt.OpeningAccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(debt => new { debt.UserId, debt.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Counterparty>().WithMany()
            .HasForeignKey(debt => new { debt.UserId, debt.CounterpartyId })
            .HasPrincipalKey(counterparty => new { counterparty.UserId, counterparty.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(debt => new { debt.UserId, debt.CounterpartyId, debt.IsCancelled })
            .HasDatabaseName("IX_Debts_UserId_CounterpartyId_Cancelled");
        builder.HasIndex(debt => new { debt.UserId, debt.PlanKind, debt.IsCancelled, debt.FirstDueDate })
            .HasDatabaseName("IX_Debts_UserId_PlanKind_Cancelled_FirstDueDate");
        builder.HasIndex(debt => new { debt.UserId, debt.CategoryId, debt.StartDate })
            .HasDatabaseName("IX_Debts_UserId_CategoryId_StartDate");
    }
}

internal sealed class DebtScheduleItemConfiguration : IEntityTypeConfiguration<DebtScheduleItem>
{
    public void Configure(EntityTypeBuilder<DebtScheduleItem> builder)
    {
        builder.ToTable("DebtScheduleItems", table =>
        {
            table.HasCheckConstraint("CK_DebtScheduleItems_Sequence", "[Sequence] >= 1");
            table.HasCheckConstraint("CK_DebtScheduleItems_Amount", "[Amount] > 0");

            // Ayrım ya hiç yoktur (eski, açılışı bilinmeyen borç) ya da tutarı
            // tam olarak böler.
            table.HasCheckConstraint(
                "CK_DebtScheduleItems_Split",
                "([PrincipalPortion] IS NULL AND [InterestPortion] IS NULL) OR " +
                "([PrincipalPortion] IS NOT NULL AND [InterestPortion] IS NOT NULL AND " +
                "[PrincipalPortion] >= 0 AND [InterestPortion] >= 0 AND " +
                "[PrincipalPortion] + [InterestPortion] = [Amount])");
        });

        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).ValueGeneratedNever();
        builder.HasAlternateKey(item => new { item.UserId, item.Id });
        builder.OwnsOne(item => item.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency).HasColumnName("Currency")
                .HasConversion<string>().HasMaxLength(3).IsUnicode(false);
        });
        builder.Property(item => item.DueDate).HasColumnType("date");
        builder.Property(item => item.PlanRevision).IsRequired();
        builder.Property(item => item.IsSuperseded).IsRequired();
        builder.Property(item => item.PrincipalPortion).HasPrecision(19, 4);
        builder.Property(item => item.InterestPortion).HasPrecision(19, 4);
        builder.HasIndex(item => new { item.UserId, item.DueDate });
        builder.HasIndex(item => new { item.UserId, item.DebtId, item.Sequence }).IsUnique();
    }
}

internal sealed class DebtPaymentConfiguration : IEntityTypeConfiguration<DebtPayment>
{
    public void Configure(EntityTypeBuilder<DebtPayment> builder)
    {
        builder.ToTable("DebtPayments", table =>
        {
            table.HasCheckConstraint("CK_DebtPayments_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_DebtPayments_Currency", "[Currency] = 'TRY'");
            table.HasCheckConstraint("CK_DebtPayments_Direction", "[Direction] IN (1, 2)");
            table.HasCheckConstraint("CK_DebtPayments_Kind", "[Kind] IN (1, 2)");

            // Ödeme tam olarak bir yerden yapılır: hesap (POS'la tahsil dahil) ya da
            // kredi kartı. İndirim para taşımaz, yeri yoktur.
            table.HasCheckConstraint(
                "CK_DebtPayments_Source",
                "([Kind] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR " +
                "([Kind] = 1 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL AND [PosSettlementId] IS NULL AND [Direction] = 1) OR " +
                "([Kind] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NULL AND [PosSettlementId] IS NULL)");
        });

        builder.HasKey(payment => payment.Id);
        builder.Property(payment => payment.Id).ValueGeneratedNever();
        builder.HasAlternateKey(payment => new { payment.UserId, payment.Id });
        builder.Property(payment => payment.Direction).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(payment => payment.Kind).HasConversion<byte>().HasColumnType("tinyint");
        builder.OwnsOne(payment => payment.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency).HasColumnName("Currency")
                .HasConversion<string>().HasMaxLength(3).IsUnicode(false);
        });
        builder.Property(payment => payment.PaymentDate).HasColumnType("date");
        builder.Property(payment => payment.PaidAtUtc).HasColumnType("datetimeoffset");
        builder.Property(payment => payment.IsCancelled).IsRequired();
        builder.Property(payment => payment.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.Ignore(payment => payment.SignedAccountEffect);

        builder.HasMany(payment => payment.Allocations).WithOne()
            .HasForeignKey(allocation => new { allocation.UserId, allocation.PaymentId })
            .HasPrincipalKey(payment => new { payment.UserId, payment.Id })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(payment => payment.Allocations).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(payment => payment.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(payment => new { payment.UserId, payment.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CreditCard>().WithMany()
            .HasForeignKey(payment => new { payment.UserId, payment.CreditCardId })
            .HasPrincipalKey(card => new { card.UserId, card.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        builder.HasOne<PosSettlement>().WithMany()
            .HasForeignKey(payment => new { payment.UserId, payment.PosSettlementId })
            .HasPrincipalKey(pos => new { pos.UserId, pos.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(payment => new { payment.UserId, payment.AccountId, payment.PaymentDate })
            .HasDatabaseName("IX_DebtPayments_UserId_AccountId_Date");
        builder.HasIndex(payment => new { payment.UserId, payment.DebtId })
            .HasDatabaseName("IX_DebtPayments_UserId_DebtId");
        builder.HasIndex(payment => new { payment.UserId, payment.CreditCardId, payment.PaymentDate })
            .HasFilter("[CreditCardId] IS NOT NULL")
            .HasDatabaseName("IX_DebtPayments_UserId_CreditCardId_Date");
        builder.HasIndex(payment => new { payment.UserId, payment.PosSettlementId })
            .IsUnique()
            .HasFilter("[PosSettlementId] IS NOT NULL")
            .HasDatabaseName("UX_DebtPayments_UserId_PosSettlementId");
    }
}

internal sealed class DebtPaymentAllocationConfiguration : IEntityTypeConfiguration<DebtPaymentAllocation>
{
    public void Configure(EntityTypeBuilder<DebtPaymentAllocation> builder)
    {
        builder.ToTable("DebtPaymentAllocations", table =>
            table.HasCheckConstraint("CK_DebtPaymentAllocations_Amount", "[Amount] > 0"));

        builder.HasKey(allocation => new { allocation.PaymentId, allocation.ScheduleItemId });
        builder.Property(allocation => allocation.Amount).HasPrecision(19, 4);
        builder.HasOne<DebtScheduleItem>().WithMany()
            .HasForeignKey(allocation => new { allocation.UserId, allocation.ScheduleItemId })
            .HasPrincipalKey(item => new { item.UserId, item.Id })
            .OnDelete(DeleteBehavior.Restrict);

        // Bir kalemi birden çok kapanış kapatabilir (kısmi ödeme, ADR 0021 İ2).
        // Kalanı aşan eşzamanlı yazmayı borcun sürümü durdurur.
        builder.HasIndex(allocation => new { allocation.UserId, allocation.ScheduleItemId })
            .HasDatabaseName("IX_DebtPaymentAllocations_UserId_ScheduleItemId");
    }
}
