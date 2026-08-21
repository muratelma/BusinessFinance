using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class FinancialAttachmentConfiguration
    : IEntityTypeConfiguration<FinancialAttachment>
{
    public void Configure(EntityTypeBuilder<FinancialAttachment> builder)
    {
        builder.ToTable("FinancialAttachments", table =>
        {
            table.HasCheckConstraint(
                "CK_FinancialAttachments_Size", $"[SizeBytes] BETWEEN 1 AND {FinancialAttachment.MaximumSizeBytes}");
            table.HasCheckConstraint(
                "CK_FinancialAttachments_ContentType",
                "[ContentType] IN ('application/pdf', 'image/jpeg', 'image/png')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(FinancialAttachment.MaximumOriginalFileNameLength);
        builder.Property(x => x.ContentType)
            .HasMaxLength(FinancialAttachment.MaximumContentTypeLength).IsUnicode(false);
        builder.Property(x => x.SizeBytes);
        builder.Property(x => x.Sha256)
            .HasMaxLength(FinancialAttachment.Sha256HexLength).IsUnicode(false).IsFixedLength();
        builder.Property(x => x.ObjectKey)
            .HasMaxLength(FinancialAttachment.MaximumObjectKeyLength).IsUnicode(false);
        builder.HasOne<BudgetTransaction>().WithMany()
            .HasForeignKey(x => new { x.UserId, x.TransactionId })
            .HasPrincipalKey(x => new { x.UserId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.UserId, x.TransactionId, x.CreatedAtUtc });
        builder.HasIndex(x => x.ObjectKey).IsUnique();
    }
}
