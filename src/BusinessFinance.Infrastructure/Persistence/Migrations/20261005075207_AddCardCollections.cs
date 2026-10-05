using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Kartla tahsil (ADR 0019 T5, Aşama 06.3 Grup 5 teslim 3/3): POS tahsilatı
    /// bir satış ya da daha önce tanınmış bir alacağın kartla tahsili olabilir;
    /// cari tahsilat ve tek seferlik alacağın kapanışı paranın yoldaki POS
    /// kaydına bağlanabilir.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Veri kaybettirmez.</b> Mevcut her POS tahsilatı bir satıştır: tür
    /// satış olarak doldurulur, kategori ve kapsam dolu kalır. Kategori ve
    /// kapsam yalnız kartla tahsilde boş kalabilmek için nullable olur; iki yeni
    /// bağ kolonu nullable eklenir ve mevcut satırlarda boştur (o tahsilatlar
    /// nakit ya da havaleydi).
    /// </para>
    /// <para>
    /// Sıra kurallara uyar: eski kapsam CHECK'i düşer, kolonlar gevşer ve
    /// eklenir, backfill çalışır, yeni CHECK'ler en son eklenir. <c>Kind</c>
    /// EF'in ürettiği <c>defaultValue: 0</c> ile eklenmez — sıfır geçerli bir tür
    /// değildir ve kalıcı bir DEFAULT bırakırdı. Kolon nullable eklenir,
    /// doldurulur, sonra <c>NOT NULL</c> yapılır.
    /// </para>
    /// <para>
    /// <c>Down</c> kartla tahsil kaydı varken çalışmaz: kategorisi olmayan satır
    /// <c>NOT NULL</c> kategoriye dönemez. Geri dönüş önce o kayıtların iptali ve
    /// silinmesini ister; bu bilinçli olarak otomatik yapılmaz.
    /// </para>
    /// </remarks>
    public partial class AddCardCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PosSettlements_Scope",
                table: "PosSettlements");

            migrationBuilder.AlterColumn<byte>(
                name: "Scope",
                table: "PosSettlements",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "PosSettlements",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<byte>(
                name: "Kind",
                table: "PosSettlements",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PosSettlementId",
                table: "ObligationSettlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PosSettlementId",
                table: "CounterpartyPayments",
                type: "uniqueidentifier",
                nullable: true);

            // Bugüne kadar yazılmış her POS tahsilatı bir satıştır.
            migrationBuilder.Sql("UPDATE [PosSettlements] SET [Kind] = 1 WHERE [Kind] IS NULL;");

            migrationBuilder.AlterColumn<byte>(
                name: "Kind",
                table: "PosSettlements",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_PosSettlements_Kind",
                table: "PosSettlements",
                sql: "[Kind] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PosSettlements_KindShape",
                table: "PosSettlements",
                sql: "([Kind] = 1 AND [CategoryId] IS NOT NULL AND [Scope] IS NOT NULL) OR ([Kind] = 2 AND [CategoryId] IS NULL AND [DayCloseId] IS NULL AND (([CommissionAmount] = 0 AND [Scope] IS NULL) OR ([CommissionAmount] > 0 AND [Scope] IS NOT NULL)))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PosSettlements_Scope",
                table: "PosSettlements",
                sql: "[Scope] IS NULL OR [Scope] IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "UX_ObligationSettlements_UserId_PosSettlementId",
                table: "ObligationSettlements",
                columns: new[] { "UserId", "PosSettlementId" },
                unique: true,
                filter: "[PosSettlementId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_CounterpartyPayments_UserId_PosSettlementId",
                table: "CounterpartyPayments",
                columns: new[] { "UserId", "PosSettlementId" },
                unique: true,
                filter: "[PosSettlementId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CounterpartyPayments_PosSettlements_UserId_PosSettlementId",
                table: "CounterpartyPayments",
                columns: new[] { "UserId", "PosSettlementId" },
                principalTable: "PosSettlements",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ObligationSettlements_PosSettlements_UserId_PosSettlementId",
                table: "ObligationSettlements",
                columns: new[] { "UserId", "PosSettlementId" },
                principalTable: "PosSettlements",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CounterpartyPayments_PosSettlements_UserId_PosSettlementId",
                table: "CounterpartyPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ObligationSettlements_PosSettlements_UserId_PosSettlementId",
                table: "ObligationSettlements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PosSettlements_Kind",
                table: "PosSettlements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PosSettlements_KindShape",
                table: "PosSettlements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PosSettlements_Scope",
                table: "PosSettlements");

            migrationBuilder.DropIndex(
                name: "UX_ObligationSettlements_UserId_PosSettlementId",
                table: "ObligationSettlements");

            migrationBuilder.DropIndex(
                name: "UX_CounterpartyPayments_UserId_PosSettlementId",
                table: "CounterpartyPayments");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "PosSettlements");

            migrationBuilder.DropColumn(
                name: "PosSettlementId",
                table: "ObligationSettlements");

            migrationBuilder.DropColumn(
                name: "PosSettlementId",
                table: "CounterpartyPayments");

            migrationBuilder.AlterColumn<byte>(
                name: "Scope",
                table: "PosSettlements",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "PosSettlements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_PosSettlements_Scope",
                table: "PosSettlements",
                sql: "[Scope] IN (1, 2)");
        }
    }
}
