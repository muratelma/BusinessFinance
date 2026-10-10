using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class DayCloseCountedOverlapConfiguration
    : IEntityTypeConfiguration<DayCloseCountedOverlap>
{
    public void Configure(EntityTypeBuilder<DayCloseCountedOverlap> builder)
    {
        // Kullanıcının verdiği bilgi: sayılan kayıtlardan yeniden hesaplanamaz.
        // Para okumaları bu tabloyu okumaz; günün ekranı ve geri alma okur.
        builder.ToTable("DayCloseCountedOverlaps", table =>
            table.HasCheckConstraint("CK_DayCloseCountedOverlaps_Amount", "[Amount] > 0"));

        // Bir gün sonunda grup başına tek ortak tutar.
        builder.HasKey(overlap => new { overlap.DayCloseId, overlap.GroupId });
        builder.Property(overlap => overlap.Amount).HasPrecision(19, 4);
        builder.HasIndex(overlap => new { overlap.UserId, overlap.DayCloseId })
            .HasDatabaseName("IX_DayCloseCountedOverlaps_UserId_DayCloseId");

        // Grubun kendi tablosuna foreign key yoktur: kişi ya da fatura olabilir.
        builder.HasOne<DayClose>().WithMany()
            .HasForeignKey(overlap => new { overlap.UserId, overlap.DayCloseId })
            .HasPrincipalKey(dayClose => new { dayClose.UserId, dayClose.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
