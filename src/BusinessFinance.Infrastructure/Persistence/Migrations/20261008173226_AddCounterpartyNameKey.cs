using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterpartyNameKey : Migration
    {
        /// <summary>
        /// Kişi adına karşılaştırma anahtarı ekler ve tekliği ona taşır.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ad kolonundaki teklik veritabanının harf kuralına bağlıydı; o kural
        /// Türkçe İ/i ve I/ı çiftlerini ayrı saydığı için aynı kişi iki kez
        /// açılabiliyordu. Anahtar uygulamada hesaplanır
        /// (<c>Counterparty.NameKeyOf</c>); buradaki ifade onun SQL karşılığıdır
        /// ve yalnız var olan satırları doldurur.
        /// </para>
        /// <para>
        /// Sıra: kolon boş bırakılabilir eklenir, doldurulur, çakışanlar
        /// ayrılır, sonra zorunlu yapılır ve teklik kurulur. Yeni kurala göre
        /// aynı adı taşıyan eski kişiler <b>birleştirilmez</b>: ikisi de
        /// hareket taşıyor olabilir. İlki (aktif olan, sonra kimlik sırası)
        /// adı tutar; ötekiler kimlikleriyle ayrılan bir anahtar alır ve
        /// oldukları gibi kullanılmaya devam eder.
        /// </para>
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameKey",
                table: "Counterparties",
                type: "nvarchar(190)",
                maxLength: 190,
                nullable: true,
                collation: "Latin1_General_100_BIN2");

            // Dört i harfi tek harfe iner, kalan harfler küçülür. Değiştirme
            // ikili karşılaştırmayla yapılır; veritabanının kendi harf kuralı
            // sonucu etkilemez.
            migrationBuilder.Sql(
                """
                UPDATE [Counterparties]
                SET [NameKey] = LOWER(
                    REPLACE(REPLACE(REPLACE(REPLACE(
                        LTRIM(RTRIM([Name])) COLLATE Latin1_General_100_BIN2,
                        NCHAR(304), N'i'), NCHAR(305), N'i'), N'I', N'i'), NCHAR(775), N''));
                """);

            // Art arda boşluklar tek boşluğa iner.
            migrationBuilder.Sql(
                """
                WHILE EXISTS (SELECT 1 FROM [Counterparties] WHERE [NameKey] LIKE N'%  %')
                BEGIN
                    UPDATE [Counterparties]
                    SET [NameKey] = REPLACE([NameKey], N'  ', N' ')
                    WHERE [NameKey] LIKE N'%  %';
                END
                """);

            migrationBuilder.Sql(
                """
                WITH [Ranked] AS (
                    SELECT [Id], [NameKey],
                           ROW_NUMBER() OVER (
                               PARTITION BY [UserId], [NameKey]
                               ORDER BY [IsActive] DESC, [Id]) AS [Position]
                    FROM [Counterparties])
                UPDATE [Ranked]
                SET [NameKey] = [NameKey] + N'#' + LOWER(CONVERT(nvarchar(36), [Id]))
                WHERE [Position] > 1;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "NameKey",
                table: "Counterparties",
                type: "nvarchar(190)",
                maxLength: 190,
                nullable: false,
                collation: "Latin1_General_100_BIN2",
                oldClrType: typeof(string),
                oldType: "nvarchar(190)",
                oldMaxLength: 190,
                oldNullable: true,
                oldCollation: "Latin1_General_100_BIN2");

            migrationBuilder.DropIndex(
                name: "UX_Counterparties_UserId_Name",
                table: "Counterparties");

            migrationBuilder.CreateIndex(
                name: "UX_Counterparties_UserId_NameKey",
                table: "Counterparties",
                columns: new[] { "UserId", "NameKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Counterparties_UserId_NameKey",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "NameKey",
                table: "Counterparties");

            migrationBuilder.CreateIndex(
                name: "UX_Counterparties_UserId_Name",
                table: "Counterparties",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }
    }
}
