using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class DayCloseCountedRecordConfiguration
    : IEntityTypeConfiguration<DayCloseCountedRecord>
{
    public void Configure(EntityTypeBuilder<DayCloseCountedRecord> builder)
    {
        // Bağ tutar taşımaz (ADR 0019 İ3): yalnız "şu gün sonu şu kaydı saydı".
        builder.ToTable("DayCloseCountedRecords", table =>
            table.HasCheckConstraint("CK_DayCloseCountedRecords_Kind", "[Kind] IN (1, 2, 3, 4)"));

        builder.HasKey(counted => new { counted.DayCloseId, counted.Kind, counted.RecordId });
        builder.Property(counted => counted.Kind).HasConversion<byte>().HasColumnType("tinyint");

        // Bir kayıt en çok bir gün sonunda sayılır. Geri alınan gün sonunun
        // bağları silinir; bu yüzden indeks süzgeçsizdir. "Sayıldı mı" okuması
        // ve birleşik akıştaki alt sorgu da bu indeksten gider.
        builder.HasIndex(counted => new { counted.UserId, counted.Kind, counted.RecordId })
            .IsUnique()
            .HasDatabaseName("UX_DayCloseCountedRecords_UserId_Kind_RecordId");

        // Sayılan kaydın kendi tablosuna foreign key yoktur: dört ayrı
        // tabloyu gösterir. Sayılan kayıt silinemez ve iptal edilemez; bağ
        // yalnız gün sonu geri alınınca kalkar.
        builder.HasOne<DayClose>().WithMany()
            .HasForeignKey(counted => new { counted.UserId, counted.DayCloseId })
            .HasPrincipalKey(dayClose => new { dayClose.UserId, dayClose.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
