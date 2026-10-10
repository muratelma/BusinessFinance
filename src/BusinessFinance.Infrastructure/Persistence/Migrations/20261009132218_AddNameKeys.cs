using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNameKeys : Migration
    {
        private const string Collation = "Latin1_General_100_BIN2";

        /// <summary>
        /// Hesap, kredi kartı, kategori ve POS adına karşılaştırma anahtarı
        /// ekler, tekliği ona taşır ve kişilerin anahtarını yeni kurala göre
        /// yeniden hesaplar.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Kural (kullanıcı kararı, 9 Ekim 2026): anahtarda yalnız harfler ve
        /// rakamlar kalır; harf büyüklüğü, boşluk ve noktalama ad farkı
        /// değildir ("İş Bankası" = "işbankası" = "İş-Bankası."). Anahtar
        /// uygulamada hesaplanır (<c>NameKeys.Of</c>); buradaki ifade onun SQL
        /// karşılığıdır ve yalnız var olan satırları doldurur. SQL tarafı
        /// harf olmayanları sabit bir listeden atar (klavyeden yazılabilen
        /// noktalama ve boşluklar); uygulama <c>char.IsLetterOrDigit</c>
        /// kullanır. İkisi yalnız o listede olmayan nadir simgelerde
        /// ayrışabilir ve fark yalnız bu yükseltmeden önce yazılmış satırı
        /// etkiler.
        /// </para>
        /// <para>
        /// Sıra her tabloda aynıdır: kolon boş bırakılabilir eklenir,
        /// doldurulur, çakışanlar ayrılır, sonra zorunlu yapılır ve teklik
        /// kurulur. Yeni kurala göre aynı adı taşıyan eski kayıtlar
        /// <b>birleştirilmez</b>: ikisi de hareket taşıyor olabilir. İlki
        /// (aktif olan, sonra kimlik sırası) adı tutar; ötekiler kimlikleriyle
        /// ayrılan bir anahtar alır ve oldukları gibi kullanılmaya devam eder.
        /// </para>
        /// <para>
        /// POS adında bugüne kadar hiç teklik yoktu. Kişilerde teklik kolonu
        /// zaten vardı (<c>AddCounterpartyNameKey</c>); kural değiştiği için
        /// indeks düşürülür, bütün anahtarlar addan yeniden hesaplanır ve
        /// indeks yeniden kurulur.
        /// </para>
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddNullableKey(migrationBuilder, "Accounts", 140);
            AddNullableKey(migrationBuilder, "CreditCards", 140);
            AddNullableKey(migrationBuilder, "Categories", 140);
            AddNullableKey(migrationBuilder, "PosDefinitions", 120);

            migrationBuilder.DropIndex(
                name: "UX_Counterparties_UserId_NameKey",
                table: "Counterparties");

            FillKeys(migrationBuilder, "Accounts", "[UserId]");
            FillKeys(migrationBuilder, "CreditCards", "[UserId]");
            FillKeys(migrationBuilder, "Categories", "[UserId], [Type]");
            FillKeys(migrationBuilder, "PosDefinitions", "[UserId]");
            FillKeys(migrationBuilder, "Counterparties", "[UserId]");

            RequireKey(migrationBuilder, "Accounts", 140);
            RequireKey(migrationBuilder, "CreditCards", 140);
            RequireKey(migrationBuilder, "Categories", 140);
            RequireKey(migrationBuilder, "PosDefinitions", 120);

            migrationBuilder.DropIndex(
                name: "UX_CreditCards_UserId_Name",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "UX_Categories_UserId_Type_Name",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "UX_Accounts_UserId_Name",
                table: "Accounts");

            migrationBuilder.CreateIndex(
                name: "UX_PosDefinitions_UserId_NameKey",
                table: "PosDefinitions",
                columns: new[] { "UserId", "NameKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_CreditCards_UserId_NameKey",
                table: "CreditCards",
                columns: new[] { "UserId", "NameKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Categories_UserId_Type_NameKey",
                table: "Categories",
                columns: new[] { "UserId", "Type", "NameKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Accounts_UserId_NameKey",
                table: "Accounts",
                columns: new[] { "UserId", "NameKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Counterparties_UserId_NameKey",
                table: "Counterparties",
                columns: new[] { "UserId", "NameKey" },
                unique: true);
        }

        /// <remarks>
        /// Kişilerin anahtarı eski kurala döndürülmez: yeni anahtarlar da
        /// tekildir ve eski sürüm onları yalnız karşılaştırma için okur.
        /// </remarks>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_PosDefinitions_UserId_NameKey",
                table: "PosDefinitions");

            migrationBuilder.DropIndex(
                name: "UX_CreditCards_UserId_NameKey",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "UX_Categories_UserId_Type_NameKey",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "UX_Accounts_UserId_NameKey",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "NameKey",
                table: "PosDefinitions");

            migrationBuilder.DropColumn(
                name: "NameKey",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "NameKey",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "NameKey",
                table: "Accounts");

            migrationBuilder.CreateIndex(
                name: "UX_CreditCards_UserId_Name",
                table: "CreditCards",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Categories_UserId_Type_Name",
                table: "Categories",
                columns: new[] { "UserId", "Type", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Accounts_UserId_Name",
                table: "Accounts",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        private static void AddNullableKey(MigrationBuilder migrationBuilder, string table, int length) =>
            migrationBuilder.AddColumn<string>(
                name: "NameKey",
                table: table,
                type: $"nvarchar({length})",
                maxLength: length,
                nullable: true,
                collation: Collation);

        private static void RequireKey(MigrationBuilder migrationBuilder, string table, int length) =>
            migrationBuilder.AlterColumn<string>(
                name: "NameKey",
                table: table,
                type: $"nvarchar({length})",
                maxLength: length,
                nullable: false,
                collation: Collation,
                oldClrType: typeof(string),
                oldType: $"nvarchar({length})",
                oldMaxLength: length,
                oldNullable: true,
                oldCollation: Collation);

        /// <summary>
        /// Anahtarı addan hesaplar ve aynı anahtarı taşıyan eski kayıtları
        /// ayırır. <paramref name="partition"/> tekliğin kapsamıdır.
        /// </summary>
        private static void FillKeys(MigrationBuilder migrationBuilder, string table, string partition)
        {
            // Dört i harfi tek harfe iner, kalan harfler küçülür; sonra boşluk
            // ve noktalama atılır. Değiştirme ikili karşılaştırmayla yapılır;
            // veritabanının kendi harf kuralı sonucu etkilemez. Hiç harf ya da
            // rakam taşımayan adın anahtarı boşluksuz, küçük harfli hâlidir.
            migrationBuilder.Sql(
                $"""
                DECLARE @strip nvarchar(200) =
                    N'!"#$%&''()*+,-./:;<=>?@[\]^_`|~' + NCHAR(123) + NCHAR(125) +
                    NCHAR(9) + NCHAR(10) + NCHAR(13) + NCHAR(160) + NCHAR(161) + NCHAR(163) +
                    NCHAR(167) + NCHAR(169) + NCHAR(171) + NCHAR(174) + NCHAR(176) + NCHAR(180) +
                    NCHAR(183) + NCHAR(187) + NCHAR(191) + NCHAR(8211) + NCHAR(8212) +
                    NCHAR(8216) + NCHAR(8217) + NCHAR(8220) + NCHAR(8221) + NCHAR(8226) +
                    NCHAR(8230) + NCHAR(8364) + NCHAR(8378) + NCHAR(8482) + NCHAR(32);
                DECLARE @blank nvarchar(200) = REPLICATE(NCHAR(1), DATALENGTH(@strip) / 2);

                WITH [Folded] AS (
                    SELECT [Id],
                           LOWER(REPLACE(REPLACE(REPLACE(REPLACE(
                               [Name] COLLATE {Collation},
                               NCHAR(304), N'i'), NCHAR(305), N'i'), N'I', N'i'), NCHAR(775), N'')) AS [Lowered]
                    FROM [{table}]),
                [Keyed] AS (
                    SELECT [Id], [Lowered],
                           REPLACE(TRANSLATE([Lowered], @strip, @blank), NCHAR(1), N'') AS [Stripped]
                    FROM [Folded])
                UPDATE [Target]
                SET [NameKey] = CASE
                    WHEN DATALENGTH([Keyed].[Stripped]) = 0
                        THEN REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                            [Keyed].[Lowered], N' ', N''), NCHAR(9), N''), NCHAR(10), N''),
                            NCHAR(13), N''), NCHAR(160), N'')
                    ELSE [Keyed].[Stripped] END
                FROM [{table}] AS [Target]
                INNER JOIN [Keyed] ON [Keyed].[Id] = [Target].[Id];
                """);

            migrationBuilder.Sql(
                $"""
                WITH [Ranked] AS (
                    SELECT [Id], [NameKey],
                           ROW_NUMBER() OVER (
                               PARTITION BY {partition}, [NameKey]
                               ORDER BY [IsActive] DESC, [Id]) AS [Position]
                    FROM [{table}])
                UPDATE [Ranked]
                SET [NameKey] = [NameKey] + N'#' + LOWER(CONVERT(nvarchar(36), [Id]))
                WHERE [Position] > 1;
                """);
        }
    }
}
