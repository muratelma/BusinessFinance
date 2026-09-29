using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class ObligationConfiguration : IEntityTypeConfiguration<Obligation>
{
    public void Configure(EntityTypeBuilder<Obligation> builder)
    {
        builder.ToTable("Obligations", table =>
        {
            table.HasCheckConstraint("CK_Obligations_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_Obligations_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_Obligations_Direction", "[Direction] IN (1, 2)");
            table.HasCheckConstraint("CK_Obligations_Scope", "[Scope] IN (1, 2)");
            table.HasCheckConstraint("CK_Obligations_DueDate", "[DueDate] >= [IssueDate]");
        });

        builder.HasKey(obligation => obligation.Id);
        builder.HasAlternateKey(obligation => new { obligation.UserId, obligation.Id });
        builder.Property(obligation => obligation.Direction).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(obligation => obligation.Scope).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(obligation => obligation.IssueDate).HasColumnType("date");
        builder.Property(obligation => obligation.DueDate).HasColumnType("date");
        builder.Property(obligation => obligation.CreatedAtUtc).HasColumnType("datetimeoffset");
        builder.Property(obligation => obligation.Description)
            .HasMaxLength(Obligation.MaximumDescriptionLength);
        builder.Property(obligation => obligation.IsCancelled).IsRequired();
        builder.Property(obligation => obligation.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.Ignore(obligation => obligation.Status);
        builder.Ignore(obligation => obligation.RecognizedType);
        builder.OwnsOne(obligation => obligation.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.HasIndex(obligation => new
        { obligation.UserId, obligation.IsCancelled, obligation.DueDate, obligation.Direction })
            .HasDatabaseName("IX_Obligations_UserId_Cancelled_DueDate_Direction");
        builder.HasIndex(obligation => new
        { obligation.UserId, obligation.CounterpartyId, obligation.IsCancelled, obligation.DueDate })
            .HasDatabaseName("IX_Obligations_UserId_CounterpartyId_Cancelled_DueDate");
        builder.HasIndex(obligation => new
        { obligation.UserId, obligation.CategoryId, obligation.IssueDate })
            .HasDatabaseName("IX_Obligations_UserId_CategoryId_IssueDate");

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(obligation => obligation.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(obligation => new { obligation.UserId, obligation.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Counterparty>().WithMany()
            .HasForeignKey(obligation => new { obligation.UserId, obligation.CounterpartyId })
            .HasPrincipalKey(counterparty => new { counterparty.UserId, counterparty.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(obligation => obligation.Settlement)
            .WithOne()
            .HasForeignKey<ObligationSettlement>(
                settlement => new { settlement.UserId, settlement.ObligationId })
            .HasPrincipalKey<Obligation>(
                obligation => new { obligation.UserId, obligation.Id })
            .OnDelete(DeleteBehavior.Restrict);

        var settlementNavigation = builder.Metadata.FindNavigation(nameof(Obligation.Settlement));
        settlementNavigation?.SetField("_settlement");
        settlementNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class ObligationSettlementConfiguration
    : IEntityTypeConfiguration<ObligationSettlement>
{
    public void Configure(EntityTypeBuilder<ObligationSettlement> builder)
    {
        builder.ToTable("ObligationSettlements", table =>
        {
            table.HasCheckConstraint("CK_ObligationSettlements_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_ObligationSettlements_Currency", "[Currency] = 1");
            table.HasCheckConstraint(
                "CK_ObligationSettlements_Direction",
                "[Direction] IN (1, 2)");
        });

        builder.HasKey(settlement => settlement.Id);
        builder.HasAlternateKey(settlement => new { settlement.UserId, settlement.Id });
        builder.Property(settlement => settlement.Direction)
            .HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(settlement => settlement.SettlementDate).HasColumnType("date");
        builder.Property(settlement => settlement.SettledAtUtc).HasColumnType("datetimeoffset");
        builder.Property(settlement => settlement.IsCancelled).IsRequired();
        builder.Property(settlement => settlement.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.Ignore(settlement => settlement.SignedAccountEffect);
        builder.OwnsOne(settlement => settlement.Amount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("Amount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        builder.HasIndex(settlement => new { settlement.UserId, settlement.ObligationId })
            .IsUnique()
            .HasDatabaseName("UX_ObligationSettlements_UserId_ObligationId");
        builder.HasIndex(settlement => new
        { settlement.UserId, settlement.AccountId, settlement.SettlementDate })
            .HasDatabaseName("IX_ObligationSettlements_UserId_AccountId_Date");

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(settlement => settlement.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(settlement => new { settlement.UserId, settlement.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
