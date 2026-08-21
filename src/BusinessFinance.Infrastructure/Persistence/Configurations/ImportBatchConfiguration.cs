using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class ImportBatchConfiguration : IEntityTypeConfiguration<ImportBatch>
{
    public void Configure(EntityTypeBuilder<ImportBatch> builder)
    {
        builder.ToTable("ImportBatches", table =>
        {
            table.HasCheckConstraint("CK_ImportBatches_FileSize", "[FileSizeBytes] > 0 AND [FileSizeBytes] <= 2097152");
            table.HasCheckConstraint("CK_ImportBatches_Status", "[Status] IN (1, 2, 3)");
            table.HasCheckConstraint("CK_ImportBatches_Delimiter", "[Delimiter] IN (',', ';', CHAR(9))");
            table.HasCheckConstraint("CK_ImportBatches_DecimalSeparator", "[DecimalSeparator] IN ('.', ',')");
        });
        builder.HasKey(batch => batch.Id);
        builder.HasAlternateKey(batch => new { batch.UserId, batch.Id });
        builder.Property(batch => batch.FileName).HasMaxLength(ImportBatch.MaximumFileNameLength);
        builder.Property(batch => batch.FileFingerprint)
            .HasMaxLength(ImportBatch.FingerprintLength).IsFixedLength();
        builder.Property(batch => batch.FileSizeBytes);
        builder.Property(batch => batch.EncodingName).HasMaxLength(ImportBatch.MaximumEncodingNameLength);
        builder.Property(batch => batch.Delimiter).HasMaxLength(1).IsFixedLength();
        builder.Property(batch => batch.DateColumn).HasMaxLength(ImportBatch.MaximumColumnNameLength);
        builder.Property(batch => batch.AmountColumn).HasMaxLength(ImportBatch.MaximumColumnNameLength);
        builder.Property(batch => batch.DescriptionColumn).HasMaxLength(ImportBatch.MaximumColumnNameLength);
        builder.Property(batch => batch.ReferenceColumn).HasMaxLength(ImportBatch.MaximumColumnNameLength);
        builder.Property(batch => batch.DateFormat).HasMaxLength(32);
        builder.Property(batch => batch.DecimalSeparator).HasMaxLength(1).IsFixedLength();
        builder.Property(batch => batch.Status).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(batch => batch.CreatedAtUtc).HasColumnType("datetimeoffset");
        builder.Ignore(batch => batch.TotalRowCount);
        builder.Ignore(batch => batch.ValidRowCount);
        builder.Ignore(batch => batch.InvalidRowCount);
        builder.HasMany(batch => batch.Rows).WithOne()
            .HasForeignKey(row => new { row.UserId, row.ImportBatchId })
            .HasPrincipalKey(batch => new { batch.UserId, batch.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(batch => batch.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(batch => new { batch.UserId, batch.CreatedAtUtc })
            .HasDatabaseName("IX_ImportBatches_UserId_CreatedAtUtc");
        builder.HasIndex(batch => new { batch.UserId, batch.FileFingerprint })
            .IsUnique().HasDatabaseName("UX_ImportBatches_UserId_FileFingerprint");
    }
}
