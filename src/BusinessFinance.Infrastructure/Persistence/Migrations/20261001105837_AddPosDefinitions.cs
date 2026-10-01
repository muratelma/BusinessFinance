using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPosDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PosDefinitionId",
                table: "PosSettlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PosDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalesCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommissionCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CommissionRate = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    TransferDays = table.Column<int>(type: "int", nullable: false),
                    BusinessDaysOnly = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosDefinitions", x => x.Id);
                    table.UniqueConstraint("AK_PosDefinitions_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_PosDefinitions_CommissionCategory", "[CommissionRate] = 0 OR [CommissionCategoryId] IS NOT NULL");
                    table.CheckConstraint("CK_PosDefinitions_CommissionRate", "[CommissionRate] >= 0 AND [CommissionRate] < 1");
                    table.CheckConstraint("CK_PosDefinitions_TransferDays", "[TransferDays] >= 0 AND [TransferDays] <= 60");
                    table.ForeignKey(
                        name: "FK_PosDefinitions_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosDefinitions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosDefinitions_Categories_UserId_CommissionCategoryId",
                        columns: x => new { x.UserId, x.CommissionCategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosDefinitions_Categories_UserId_SalesCategoryId",
                        columns: x => new { x.UserId, x.SalesCategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosSettlements_UserId_PosDefinitionId",
                table: "PosSettlements",
                columns: new[] { "UserId", "PosDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosDefinitions_UserId_AccountId",
                table: "PosDefinitions",
                columns: new[] { "UserId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosDefinitions_UserId_CommissionCategoryId",
                table: "PosDefinitions",
                columns: new[] { "UserId", "CommissionCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosDefinitions_UserId_IsActive",
                table: "PosDefinitions",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PosDefinitions_UserId_SalesCategoryId",
                table: "PosDefinitions",
                columns: new[] { "UserId", "SalesCategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PosSettlements_PosDefinitions_UserId_PosDefinitionId",
                table: "PosSettlements",
                columns: new[] { "UserId", "PosDefinitionId" },
                principalTable: "PosDefinitions",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosSettlements_PosDefinitions_UserId_PosDefinitionId",
                table: "PosSettlements");

            migrationBuilder.DropTable(
                name: "PosDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_PosSettlements_UserId_PosDefinitionId",
                table: "PosSettlements");

            migrationBuilder.DropColumn(
                name: "PosDefinitionId",
                table: "PosSettlements");
        }
    }
}
