using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDayCloseCountedRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DayCloseCountedRecords",
                columns: table => new
                {
                    DayCloseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<byte>(type: "tinyint", nullable: false),
                    RecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayCloseCountedRecords", x => new { x.DayCloseId, x.Kind, x.RecordId });
                    table.CheckConstraint("CK_DayCloseCountedRecords_Kind", "[Kind] IN (1, 2, 3, 4)");
                    table.ForeignKey(
                        name: "FK_DayCloseCountedRecords_DayCloses_UserId_DayCloseId",
                        columns: x => new { x.UserId, x.DayCloseId },
                        principalTable: "DayCloses",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayCloseCountedRecords_UserId_DayCloseId",
                table: "DayCloseCountedRecords",
                columns: new[] { "UserId", "DayCloseId" });

            migrationBuilder.CreateIndex(
                name: "UX_DayCloseCountedRecords_UserId_Kind_RecordId",
                table: "DayCloseCountedRecords",
                columns: new[] { "UserId", "Kind", "RecordId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayCloseCountedRecords");
        }
    }
}
