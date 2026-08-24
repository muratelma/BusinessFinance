using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Cari hareketlere bilinmeyen geçmişi uydurmadan opsiyonel vade ekler ve
    /// tek seferlik yükümlülükleri ayrı tanıma/kasa hareketleri olarak saklar.
    /// </summary>
    /// <remarks>
    /// <c>CounterpartyCharges</c> dolu olabileceği için <c>DueDate</c> nullable ve
    /// varsayılansızdır. İki yükümlülük tablosu bu migration'da boş doğduğundan
    /// zorunlu kolonları için backfill gerekmez (AGENTS.md boş tablo istisnası).
    /// </remarks>
    public partial class AddObligationsAndCounterpartyDueDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CounterpartyCharges_UserId_CounterpartyId_Cancelled_Direction",
                table: "CounterpartyCharges");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DueDate",
                table: "CounterpartyCharges",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Obligations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CounterpartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    Scope = table.Column<byte>(type: "tinyint", nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligations", x => x.Id);
                    table.UniqueConstraint("AK_Obligations_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_Obligations_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_Obligations_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_Obligations_Direction", "[Direction] IN (1, 2)");
                    table.CheckConstraint("CK_Obligations_DueDate", "[DueDate] >= [IssueDate]");
                    table.CheckConstraint("CK_Obligations_Scope", "[Scope] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_Obligations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obligations_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obligations_Counterparties_UserId_CounterpartyId",
                        columns: x => new { x.UserId, x.CounterpartyId },
                        principalTable: "Counterparties",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObligationSettlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObligationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    SettlementDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SettledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationSettlements", x => x.Id);
                    table.UniqueConstraint("AK_ObligationSettlements_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_ObligationSettlements_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_ObligationSettlements_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_ObligationSettlements_Direction", "[Direction] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_ObligationSettlements_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObligationSettlements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObligationSettlements_Obligations_UserId_ObligationId",
                        columns: x => new { x.UserId, x.ObligationId },
                        principalTable: "Obligations",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyCharges_UserId_CounterpartyId_Cancelled_Direction",
                table: "CounterpartyCharges",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled", "Direction", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Obligations_UserId_Cancelled_DueDate_Direction",
                table: "Obligations",
                columns: new[] { "UserId", "IsCancelled", "DueDate", "Direction" });

            migrationBuilder.CreateIndex(
                name: "IX_Obligations_UserId_CategoryId_IssueDate",
                table: "Obligations",
                columns: new[] { "UserId", "CategoryId", "IssueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Obligations_UserId_CounterpartyId_Cancelled_DueDate",
                table: "Obligations",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ObligationSettlements_UserId_AccountId_Date",
                table: "ObligationSettlements",
                columns: new[] { "UserId", "AccountId", "SettlementDate" });

            migrationBuilder.CreateIndex(
                name: "UX_ObligationSettlements_UserId_ObligationId",
                table: "ObligationSettlements",
                columns: new[] { "UserId", "ObligationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObligationSettlements");

            migrationBuilder.DropTable(
                name: "Obligations");

            migrationBuilder.DropIndex(
                name: "IX_CounterpartyCharges_UserId_CounterpartyId_Cancelled_Direction",
                table: "CounterpartyCharges");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "CounterpartyCharges");

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyCharges_UserId_CounterpartyId_Cancelled_Direction",
                table: "CounterpartyCharges",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled", "Direction" });
        }
    }
}
