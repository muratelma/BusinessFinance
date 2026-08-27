using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerificationCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Purpose = table.Column<int>(type: "int", nullable: false),
                    CodeHash = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", precision: 0, nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", precision: 0, nullable: false),
                    ConsumedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", precision: 0, nullable: true),
                    FailedAttemptCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCodes", x => x.Id);
                    table.CheckConstraint("CK_VerificationCodes_Consumption", "[ConsumedAtUtc] IS NULL OR [ConsumedAtUtc] >= [CreatedAtUtc]");
                    table.CheckConstraint("CK_VerificationCodes_Expiry", "[ExpiresAtUtc] > [CreatedAtUtc]");
                    table.CheckConstraint("CK_VerificationCodes_FailedAttempts", "[FailedAttemptCount] >= 0");
                    table.CheckConstraint("CK_VerificationCodes_Purpose", "[Purpose] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_VerificationCodes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCodes_UserId_Purpose_CreatedAtUtc",
                table: "VerificationCodes",
                columns: new[] { "UserId", "Purpose", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationCodes");
        }
    }
}
