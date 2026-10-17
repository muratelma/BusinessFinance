using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterpartyDiscountsAndOpeningBalances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kolon önce boş eklenir, eski kişiler sıfır devirle doldurulur, sonra
            // zorunlu olur: veritabanında DEFAULT kalmaz, değeri uygulama yazar.
            migrationBuilder.AddColumn<decimal>(
                name: "OpeningPayable",
                table: "Counterparties",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OpeningReceivable",
                table: "Counterparties",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE [Counterparties] SET [OpeningReceivable] = 0, [OpeningPayable] = 0;");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpeningPayable",
                table: "Counterparties",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)",
                oldPrecision: 19,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OpeningReceivable",
                table: "Counterparties",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)",
                oldPrecision: 19,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CounterpartyChargeDiscounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CounterpartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    DiscountDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RecordedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterpartyChargeDiscounts", x => x.Id);
                    table.CheckConstraint("CK_CounterpartyChargeDiscounts_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_CounterpartyChargeDiscounts_Direction", "[Direction] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_CounterpartyChargeDiscounts_Counterparties_UserId_CounterpartyId",
                        columns: x => new { x.UserId, x.CounterpartyId },
                        principalTable: "Counterparties",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounterpartyChargeDiscounts_CounterpartyCharges_UserId_ChargeId",
                        columns: x => new { x.UserId, x.ChargeId },
                        principalTable: "CounterpartyCharges",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Counterparties_OpeningBalance",
                table: "Counterparties",
                sql: "[OpeningReceivable] >= 0 AND [OpeningPayable] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyChargeDiscounts_UserId_ChargeId",
                table: "CounterpartyChargeDiscounts",
                columns: new[] { "UserId", "ChargeId" });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyChargeDiscounts_UserId_CounterpartyId_Cancelled_Direction",
                table: "CounterpartyChargeDiscounts",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled", "Direction" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eski şema indirimi ve devreden bakiyeyi temsil edemez; böyle kayıt
            // varsa geri dönüş veri kaybettirirdi, sessizce yapılmaz.
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [CounterpartyChargeDiscounts])
                   OR EXISTS (SELECT 1 FROM [Counterparties] WHERE [OpeningReceivable] <> 0 OR [OpeningPayable] <> 0)
                    THROW 50022, 'Counterparties carry discounts or opening balances; the downgrade would lose them.', 1;
                """);

            migrationBuilder.DropTable(
                name: "CounterpartyChargeDiscounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Counterparties_OpeningBalance",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "OpeningPayable",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "OpeningReceivable",
                table: "Counterparties");
        }
    }
}
