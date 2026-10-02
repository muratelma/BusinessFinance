using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class PosSettlementConfiguration : IEntityTypeConfiguration<PosSettlement>
{
    public void Configure(EntityTypeBuilder<PosSettlement> builder)
    {
        builder.ToTable("PosSettlements", table =>
        {
            table.HasCheckConstraint("CK_PosSettlements_GrossAmount", "[GrossAmount] > 0");
            table.HasCheckConstraint("CK_PosSettlements_Currency", "[Currency] = 1");
            table.HasCheckConstraint("CK_PosSettlements_Scope", "[Scope] IN (1, 2)");
            // Komisyon sıfır olabilir ama brütün tamamını yiyemez: hesaba
            // hiçbir şey geçmeyen bir tahsilat tahsilat değildir.
            table.HasCheckConstraint(
                "CK_PosSettlements_Commission",
                "[CommissionAmount] >= 0 AND [CommissionAmount] < [GrossAmount]");
            // Komisyon ile gider kategorisi birlikte bulunur ya da hiç bulunmaz.
            table.HasCheckConstraint(
                "CK_PosSettlements_CommissionCategory",
                "([CommissionAmount] = 0 AND [CommissionCategoryId] IS NULL) OR " +
                "([CommissionAmount] > 0 AND [CommissionCategoryId] IS NOT NULL)");
            table.HasCheckConstraint(
                "CK_PosSettlements_ExpectedTransferDate",
                "[ExpectedTransferDate] >= [SettlementDate]");
            // Yatış, geçiş günü ve damgası birlikte bulunur ya da hiç bulunmaz:
            // tahsilat hesaba yalnız bir yatışla geçer (ADR 0019 T5). Geçiş
            // satıştan önce olamaz.
            table.HasCheckConstraint(
                "CK_PosSettlements_Transfer",
                "([PosDepositId] IS NULL AND [TransferredOn] IS NULL AND [TransferredAtUtc] IS NULL) OR " +
                "([PosDepositId] IS NOT NULL AND [TransferredOn] IS NOT NULL AND " +
                "[TransferredAtUtc] IS NOT NULL AND [TransferredOn] >= [SettlementDate])");
            // İptal edilmiş tahsilat bir yatışa bağlı kalamaz; iptalden önce
            // yatış geri alınır.
            table.HasCheckConstraint(
                "CK_PosSettlements_CancelledNotDeposited",
                "[IsCancelled] = 0 OR [PosDepositId] IS NULL");
        });

        builder.HasKey(settlement => settlement.Id);
        builder.HasAlternateKey(settlement => new { settlement.UserId, settlement.Id });
        builder.Property(settlement => settlement.CommissionAmount).HasPrecision(19, 4);
        builder.Property(settlement => settlement.Scope)
            .HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(settlement => settlement.SettlementDate).HasColumnType("date");
        builder.Property(settlement => settlement.ExpectedTransferDate).HasColumnType("date");
        builder.Property(settlement => settlement.TransferredOn).HasColumnType("date");
        builder.Property(settlement => settlement.TransferredAtUtc).HasColumnType("datetimeoffset");
        builder.Property(settlement => settlement.CreatedAtUtc).HasColumnType("datetimeoffset");
        builder.Property(settlement => settlement.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.Property(settlement => settlement.Description)
            .HasMaxLength(PosSettlement.MaximumDescriptionLength);
        builder.Property(settlement => settlement.IsCancelled).IsRequired();
        // Yatış ile iptal aynı tahsilat için yarışabilir; kaybeden yazma
        // hiçbir şey değiştirmez.
        builder.Property<byte[]>("Version").IsRequired().IsRowVersion();

        // Net tutar, oran ve yolda olma durumu **türetilir**; kolon değildir
        // (ADR 0015). Saklansalardı brüt veya komisyon düzeltildiğinde
        // sessizce eskiyen ikinci bir gerçek olurlardı.
        builder.Ignore(settlement => settlement.NetAmount);
        builder.Ignore(settlement => settlement.CommissionRate);
        builder.Ignore(settlement => settlement.Currency);
        builder.Ignore(settlement => settlement.IsTransferred);
        builder.Ignore(settlement => settlement.IsInTransit);
        builder.Ignore(settlement => settlement.SignedAccountEffect);

        builder.OwnsOne(settlement => settlement.GrossAmount, money =>
        {
            money.Property(value => value.Amount).HasColumnName("GrossAmount").HasPrecision(19, 4);
            money.Property(value => value.Currency)
                .HasColumnName("Currency").HasConversion<byte>().HasColumnType("tinyint");
        });

        // Yoldaki parayı okuyan sorgu bu indeksten gider: geçmemiş ve iptal
        // edilmemiş satırlar.
        builder.HasIndex(settlement => new
        { settlement.UserId, settlement.IsCancelled, settlement.TransferredOn })
            .HasDatabaseName("IX_PosSettlements_UserId_Cancelled_TransferredOn");
        builder.HasIndex(settlement => new { settlement.UserId, settlement.SettlementDate })
            .HasDatabaseName("IX_PosSettlements_UserId_SettlementDate");
        builder.HasIndex(settlement => new { settlement.UserId, settlement.AccountId })
            .HasDatabaseName("IX_PosSettlements_UserId_AccountId");
        builder.HasIndex(settlement => new { settlement.UserId, settlement.PosDefinitionId })
            .HasDatabaseName("IX_PosSettlements_UserId_PosDefinitionId");
        builder.HasIndex(settlement => new
        { settlement.UserId, settlement.PosDepositId, settlement.TransferredOn })
            .HasDatabaseName("IX_PosSettlements_UserId_PosDepositId_TransferredOn");

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(settlement => settlement.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany()
            .HasForeignKey(settlement => new { settlement.UserId, settlement.AccountId })
            .HasPrincipalKey(account => new { account.UserId, account.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(settlement => new { settlement.UserId, settlement.CategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(settlement => new { settlement.UserId, settlement.CommissionCategoryId })
            .HasPrincipalKey(category => new { category.UserId, category.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        // Yatışa günüyle birlikte bağlanır: `TransferredOn` yatışın gününün
        // kopyasıdır ve SQL onun ayrışmasına izin vermez.
        builder.HasOne<PosDeposit>().WithMany()
            .HasForeignKey(settlement => new
            { settlement.UserId, settlement.PosDepositId, settlement.TransferredOn })
            .HasPrincipalKey(deposit => new { deposit.UserId, deposit.Id, deposit.DepositDate })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        // Tanımlar gelmeden önceki ve tanımsız girilen tahsilatlarda boştur
        // (ADR 0019 T4); tahsilatı olan tanım silinemez.
        builder.HasOne<PosDefinition>().WithMany()
            .HasForeignKey(settlement => new { settlement.UserId, settlement.PosDefinitionId })
            .HasPrincipalKey(definition => new { definition.UserId, definition.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
