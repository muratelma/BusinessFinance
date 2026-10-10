using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDayCloseOverlaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DayCloseCountedRecords_Kind",
                table: "DayCloseCountedRecords");

            migrationBuilder.CreateTable(
                name: "DayCloseCountedOverlaps",
                columns: table => new
                {
                    DayCloseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayCloseCountedOverlaps", x => new { x.DayCloseId, x.GroupId });
                    table.CheckConstraint("CK_DayCloseCountedOverlaps_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_DayCloseCountedOverlaps_DayCloses_UserId_DayCloseId",
                        columns: x => new { x.UserId, x.DayCloseId },
                        principalTable: "DayCloses",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_DayCloseCountedRecords_Kind",
                table: "DayCloseCountedRecords",
                sql: "[Kind] IN (1, 2, 3, 4, 5, 6)");

            migrationBuilder.CreateIndex(
                name: "IX_DayCloseCountedOverlaps_UserId_DayCloseId",
                table: "DayCloseCountedOverlaps",
                columns: new[] { "UserId", "DayCloseId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayCloseCountedOverlaps");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DayCloseCountedRecords_Kind",
                table: "DayCloseCountedRecords");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DayCloseCountedRecords_Kind",
                table: "DayCloseCountedRecords",
                sql: "[Kind] IN (1, 2, 3, 4)");
        }
    }
}
