using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDayCloses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DayCloseId",
                table: "PosSettlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DayCloseId",
                table: "BudgetTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DayCloses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClosedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    RangeStart = table.Column<DateOnly>(type: "date", nullable: true),
                    ZNumber = table.Column<int>(type: "int", nullable: true),
                    IsAdditional = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayCloses", x => x.Id);
                    table.UniqueConstraint("AK_DayCloses_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_DayCloses_Range", "[RangeStart] IS NULL OR [RangeStart] < [ClosedOn]");
                    table.CheckConstraint("CK_DayCloses_ZNumber", "[ZNumber] IS NULL OR [ZNumber] > 0");
                    table.ForeignKey(
                        name: "FK_DayCloses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosSettlements_UserId_DayCloseId",
                table: "PosSettlements",
                columns: new[] { "UserId", "DayCloseId" },
                filter: "[DayCloseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_UserId_DayCloseId",
                table: "BudgetTransactions",
                columns: new[] { "UserId", "DayCloseId" },
                filter: "[DayCloseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_DayCloses_UserId_ClosedOn",
                table: "DayCloses",
                columns: new[] { "UserId", "ClosedOn" },
                unique: true,
                filter: "[IsCancelled] = 0 AND [IsAdditional] = 0");

            migrationBuilder.CreateIndex(
                name: "UX_DayCloses_UserId_ZNumber",
                table: "DayCloses",
                columns: new[] { "UserId", "ZNumber" },
                unique: true,
                filter: "[ZNumber] IS NOT NULL AND [IsCancelled] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetTransactions_DayCloses_UserId_DayCloseId",
                table: "BudgetTransactions",
                columns: new[] { "UserId", "DayCloseId" },
                principalTable: "DayCloses",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosSettlements_DayCloses_UserId_DayCloseId",
                table: "PosSettlements",
                columns: new[] { "UserId", "DayCloseId" },
                principalTable: "DayCloses",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetTransactions_DayCloses_UserId_DayCloseId",
                table: "BudgetTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PosSettlements_DayCloses_UserId_DayCloseId",
                table: "PosSettlements");

            migrationBuilder.DropTable(
                name: "DayCloses");

            migrationBuilder.DropIndex(
                name: "IX_PosSettlements_UserId_DayCloseId",
                table: "PosSettlements");

            migrationBuilder.DropIndex(
                name: "IX_BudgetTransactions_UserId_DayCloseId",
                table: "BudgetTransactions");

            migrationBuilder.DropColumn(
                name: "DayCloseId",
                table: "PosSettlements");

            migrationBuilder.DropColumn(
                name: "DayCloseId",
                table: "BudgetTransactions");
        }
    }
}
