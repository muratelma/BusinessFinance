using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCashCountsAndPosSettlements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashCounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CountedAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    Scope = table.Column<byte>(type: "tinyint", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AdjustmentTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdjustedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashCounts", x => x.Id);
                    table.UniqueConstraint("AK_CashCounts_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_CashCounts_Adjustment", "([AdjustmentTransactionId] IS NULL AND [AdjustedAtUtc] IS NULL) OR ([AdjustmentTransactionId] IS NOT NULL AND [AdjustedAtUtc] IS NOT NULL)");
                    table.CheckConstraint("CK_CashCounts_CountedAmount", "[CountedAmount] >= 0");
                    table.CheckConstraint("CK_CashCounts_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_CashCounts_Scope", "[Scope] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_CashCounts_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashCounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashCounts_BudgetTransactions_UserId_AdjustmentTransactionId",
                        columns: x => new { x.UserId, x.AdjustmentTransactionId },
                        principalTable: "BudgetTransactions",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosSettlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommissionCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GrossAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Scope = table.Column<byte>(type: "tinyint", nullable: false),
                    SettlementDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedTransferDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TransferredOn = table.Column<DateOnly>(type: "date", nullable: true),
                    TransferredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosSettlements", x => x.Id);
                    table.UniqueConstraint("AK_PosSettlements_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_PosSettlements_Commission", "[CommissionAmount] >= 0 AND [CommissionAmount] < [GrossAmount]");
                    table.CheckConstraint("CK_PosSettlements_CommissionCategory", "([CommissionAmount] = 0 AND [CommissionCategoryId] IS NULL) OR ([CommissionAmount] > 0 AND [CommissionCategoryId] IS NOT NULL)");
                    table.CheckConstraint("CK_PosSettlements_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_PosSettlements_ExpectedTransferDate", "[ExpectedTransferDate] >= [SettlementDate]");
                    table.CheckConstraint("CK_PosSettlements_GrossAmount", "[GrossAmount] > 0");
                    table.CheckConstraint("CK_PosSettlements_Scope", "[Scope] IN (1, 2)");
                    table.CheckConstraint("CK_PosSettlements_Transfer", "([TransferredOn] IS NULL AND [TransferredAtUtc] IS NULL) OR ([TransferredOn] IS NOT NULL AND [TransferredAtUtc] IS NOT NULL AND [TransferredOn] >= [SettlementDate])");
                    table.ForeignKey(
                        name: "FK_PosSettlements_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosSettlements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosSettlements_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosSettlements_Categories_UserId_CommissionCategoryId",
                        columns: x => new { x.UserId, x.CommissionCategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashCounts_UserId_AdjustmentTransactionId",
                table: "CashCounts",
                columns: new[] { "UserId", "AdjustmentTransactionId" });

            migrationBuilder.CreateIndex(
                name: "IX_CashCounts_UserId_CountDate",
                table: "CashCounts",
                columns: new[] { "UserId", "CountDate" });

            migrationBuilder.CreateIndex(
                name: "UX_CashCounts_UserId_AccountId_CountDate_Open",
                table: "CashCounts",
                columns: new[] { "UserId", "AccountId", "CountDate" },
                unique: true,
                filter: "[IsCancelled] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PosSettlements_UserId_AccountId",
                table: "PosSettlements",
                columns: new[] { "UserId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosSettlements_UserId_Cancelled_TransferredOn",
                table: "PosSettlements",
                columns: new[] { "UserId", "IsCancelled", "TransferredOn" });

            migrationBuilder.CreateIndex(
                name: "IX_PosSettlements_UserId_CategoryId",
                table: "PosSettlements",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosSettlements_UserId_CommissionCategoryId",
                table: "PosSettlements",
                columns: new[] { "UserId", "CommissionCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosSettlements_UserId_SettlementDate",
                table: "PosSettlements",
                columns: new[] { "UserId", "SettlementDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashCounts");

            migrationBuilder.DropTable(
                name: "PosSettlements");
        }
    }
}
