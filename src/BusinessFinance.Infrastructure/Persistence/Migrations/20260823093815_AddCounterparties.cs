using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterparties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Counterparties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counterparties", x => x.Id);
                    table.UniqueConstraint("AK_Counterparties_UserId_Id", x => new { x.UserId, x.Id });
                    table.ForeignKey(
                        name: "FK_Counterparties_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CounterpartyCharges",
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
                    ChargeDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterpartyCharges", x => x.Id);
                    table.UniqueConstraint("AK_CounterpartyCharges_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_CounterpartyCharges_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_CounterpartyCharges_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_CounterpartyCharges_Direction", "[Direction] IN (1, 2)");
                    table.CheckConstraint("CK_CounterpartyCharges_Scope", "[Scope] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_CounterpartyCharges_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounterpartyCharges_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounterpartyCharges_Counterparties_UserId_CounterpartyId",
                        columns: x => new { x.UserId, x.CounterpartyId },
                        principalTable: "Counterparties",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CounterpartyPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CounterpartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterpartyPayments", x => x.Id);
                    table.UniqueConstraint("AK_CounterpartyPayments_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_CounterpartyPayments_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_CounterpartyPayments_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_CounterpartyPayments_Direction", "[Direction] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_CounterpartyPayments_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounterpartyPayments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounterpartyPayments_Counterparties_UserId_CounterpartyId",
                        columns: x => new { x.UserId, x.CounterpartyId },
                        principalTable: "Counterparties",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Counterparties_UserId_IsActive_Name",
                table: "Counterparties",
                columns: new[] { "UserId", "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "UX_Counterparties_UserId_Name",
                table: "Counterparties",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyCharges_UserId_CategoryId_Date",
                table: "CounterpartyCharges",
                columns: new[] { "UserId", "CategoryId", "ChargeDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyCharges_UserId_CounterpartyId_Cancelled_Direction",
                table: "CounterpartyCharges",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled", "Direction" });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyPayments_UserId_AccountId_Date",
                table: "CounterpartyPayments",
                columns: new[] { "UserId", "AccountId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyPayments_UserId_CounterpartyId_Cancelled_Direction",
                table: "CounterpartyPayments",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled", "Direction" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CounterpartyCharges");

            migrationBuilder.DropTable(
                name: "CounterpartyPayments");

            migrationBuilder.DropTable(
                name: "Counterparties");
        }
    }
}
