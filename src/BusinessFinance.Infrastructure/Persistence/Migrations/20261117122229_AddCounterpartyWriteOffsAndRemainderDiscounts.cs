using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterpartyWriteOffsAndRemainderDiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "CounterpartyChargeDiscounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CounterpartyWriteOffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CounterpartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    Scope = table.Column<byte>(type: "tinyint", nullable: false),
                    WriteOffDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterpartyWriteOffs", x => x.Id);
                    table.UniqueConstraint("AK_CounterpartyWriteOffs_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_CounterpartyWriteOffs_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_CounterpartyWriteOffs_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_CounterpartyWriteOffs_Direction", "[Direction] IN (1, 2)");
                    table.CheckConstraint("CK_CounterpartyWriteOffs_Scope", "[Scope] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_CounterpartyWriteOffs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounterpartyWriteOffs_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounterpartyWriteOffs_Counterparties_UserId_CounterpartyId",
                        columns: x => new { x.UserId, x.CounterpartyId },
                        principalTable: "Counterparties",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyChargeDiscounts_UserId_PaymentId",
                table: "CounterpartyChargeDiscounts",
                columns: new[] { "UserId", "PaymentId" },
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyWriteOffs_UserId_CategoryId",
                table: "CounterpartyWriteOffs",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyWriteOffs_UserId_CounterpartyId_Cancelled_Direction",
                table: "CounterpartyWriteOffs",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled", "Direction" });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyWriteOffs_UserId_Date",
                table: "CounterpartyWriteOffs",
                columns: new[] { "UserId", "WriteOffDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_CounterpartyChargeDiscounts_CounterpartyPayments_UserId_PaymentId",
                table: "CounterpartyChargeDiscounts",
                columns: new[] { "UserId", "PaymentId" },
                principalTable: "CounterpartyPayments",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Vazgeçme ya da ödemeyle yazılmış indirim varsa geri dönüş onları
            // ya da bağlarını siler; veri kaybettirmek yerine durur.
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [CounterpartyWriteOffs])
                   OR EXISTS (SELECT 1 FROM [CounterpartyChargeDiscounts] WHERE [PaymentId] IS NOT NULL)
                    THROW 50026, 'Counterparties carry write-offs or remainder discounts; the downgrade would lose them.', 1;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_CounterpartyChargeDiscounts_CounterpartyPayments_UserId_PaymentId",
                table: "CounterpartyChargeDiscounts");

            migrationBuilder.DropTable(
                name: "CounterpartyWriteOffs");

            migrationBuilder.DropIndex(
                name: "IX_CounterpartyChargeDiscounts_UserId_PaymentId",
                table: "CounterpartyChargeDiscounts");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "CounterpartyChargeDiscounts");
        }
    }
}
