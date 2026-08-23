using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Borç sözleşmesi karşı tarafın adını taşımayı bırakır, kimliğine bağlanır.
    /// </summary>
    /// <remarks>
    /// Adım elle yazıldı; üretilen hâli önce ad kolonunu düşürüp yerine
    /// <c>Guid.Empty</c> varsayılanlı zorunlu bir kolon koyuyordu — dolu bir
    /// veritabanında bu, her sözleşmenin karşı tarafını kaybetmek demekti.
    /// Sıra AGENTS.md yükseltme kurallarına göre: kolon önce, backfill sonra,
    /// kısıt en sonda; ad kolonu ancak veri taşındıktan sonra düşer.
    /// </remarks>
    public partial class LinkDebtsToCounterparties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Karşı taraf adı iki kaynağın en genişini taşımalı: sözleşme adı
            // 150 karaktere kadar çıkabiliyordu ve daha dar bir kolon taşınan
            // adı kırpardı.
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Counterparties",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            // Kolon önce ve nullable: backfill çalışana kadar hangi sözleşmenin
            // hangi karşı tarafa ait olduğu bilinmiyor.
            migrationBuilder.AddColumn<Guid>(
                name: "CounterpartyId",
                table: "DebtAgreements",
                type: "uniqueidentifier",
                nullable: true);

            // Sözleşmelerdeki her ad bir karşı taraf olur. Zaten var olan ad
            // yeniden kurulmaz: (UserId, Name) tekil ve aynı kişi iki kez
            // yaşamamalı.
            migrationBuilder.Sql("""
                INSERT INTO [Counterparties] ([Id], [UserId], [Name], [Note], [IsActive])
                SELECT NEWID(), source.[UserId], source.[CounterpartyName], NULL, 1
                FROM (
                    SELECT DISTINCT [UserId], [CounterpartyName]
                    FROM [DebtAgreements]
                ) AS source
                WHERE NOT EXISTS (
                    SELECT 1 FROM [Counterparties] AS existing
                    WHERE existing.[UserId] = source.[UserId]
                      AND existing.[Name] = source.[CounterpartyName]);
                """);

            migrationBuilder.Sql("""
                UPDATE debt
                SET debt.[CounterpartyId] = counterparty.[Id]
                FROM [DebtAgreements] AS debt
                INNER JOIN [Counterparties] AS counterparty
                    ON counterparty.[UserId] = debt.[UserId]
                   AND counterparty.[Name] = debt.[CounterpartyName];
                """);

            // Zorunluluk backfill'den sonra. Varsayılan verilmiyor: verilseydi
            // kalıcı bir veritabanı varsayılanı kalır ve modelle şema sessizce
            // ayrışırdı.
            migrationBuilder.AlterColumn<Guid>(
                name: "CounterpartyId",
                table: "DebtAgreements",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebtAgreements_UserId_CounterpartyId",
                table: "DebtAgreements",
                columns: new[] { "UserId", "CounterpartyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DebtAgreements_Counterparties_UserId_CounterpartyId",
                table: "DebtAgreements",
                columns: new[] { "UserId", "CounterpartyId" },
                principalTable: "Counterparties",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);

            // Ad ancak veri taşındıktan sonra düşer.
            migrationBuilder.DropColumn(
                name: "CounterpartyName",
                table: "DebtAgreements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri dönüşte ad karşı taraftan okunup kolona yazılır; kurulan
            // karşı taraf kayıtları silinmez, çünkü hangilerinin bu adımda
            // doğduğu ile kullanıcının kendi kaydettiği ayırt edilemez.
            migrationBuilder.AddColumn<string>(
                name: "CounterpartyName",
                table: "DebtAgreements",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE debt
                SET debt.[CounterpartyName] = counterparty.[Name]
                FROM [DebtAgreements] AS debt
                INNER JOIN [Counterparties] AS counterparty
                    ON counterparty.[UserId] = debt.[UserId]
                   AND counterparty.[Id] = debt.[CounterpartyId];
                """);

            migrationBuilder.AlterColumn<string>(
                name: "CounterpartyName",
                table: "DebtAgreements",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_DebtAgreements_Counterparties_UserId_CounterpartyId",
                table: "DebtAgreements");

            migrationBuilder.DropIndex(
                name: "IX_DebtAgreements_UserId_CounterpartyId",
                table: "DebtAgreements");

            migrationBuilder.DropColumn(
                name: "CounterpartyId",
                table: "DebtAgreements");

            // 150 karakterlik bir ad varsa bu adım gürültüyle düşer; sessizce
            // kırpmaktansa düşmesi doğrudur.
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Counterparties",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);
        }
    }
}
