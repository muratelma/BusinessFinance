using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryRolesAndTransferFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FeeTransactionId",
                table: "Transfers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Role",
                table: "Categories",
                type: "tinyint",
                nullable: true);

            // Kolon -> geçmiş eşlemesi -> CHECK/teklik. Aynı adlı eski
            // kategoriler korunur; bir rol aktif ve en eski kimlikli adaya verilir.
            migrationBuilder.Sql("""
                WITH Candidates AS (
                    SELECT [Id], [UserId], [IsActive],
                        CASE
                            WHEN [Type] = 1 AND ([NameKey] = N'faizgeliri' OR [NameKey] LIKE N'faizgeliri#%') THEN 2
                            WHEN [Type] = 2 AND ([NameKey] = N'faizvefinansmangideri' OR [NameKey] LIKE N'faizvefinansmangideri#%') THEN 1
                            WHEN [Type] = 2 AND ([NameKey] = N'kasafarki' OR [NameKey] LIKE N'kasafarki#%') THEN 3
                            WHEN [Type] = 2 AND ([NameKey] IN (N'bankaveposkomisyonu', N'bankamasrafi') OR [NameKey] LIKE N'bankaveposkomisyonu#%' OR [NameKey] LIKE N'bankamasrafi#%') THEN 4
                        END AS [Role]
                    FROM [Categories]
                ), Ranked AS (
                    SELECT [Id], [Role], ROW_NUMBER() OVER (PARTITION BY [UserId], [Role] ORDER BY [IsActive] DESC, [Id]) AS [Rank]
                    FROM Candidates WHERE [Role] IS NOT NULL
                )
                UPDATE c SET c.[Role] = r.[Role], c.[DefaultScope] = NULL
                FROM [Categories] c INNER JOIN Ranked r ON c.[Id] = r.[Id]
                WHERE r.[Rank] = 1;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_UserId_FeeTransactionId",
                table: "Transfers",
                columns: new[] { "UserId", "FeeTransactionId" },
                unique: true,
                filter: "[FeeTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Categories_UserId_Role",
                table: "Categories",
                columns: new[] { "UserId", "Role" },
                unique: true,
                filter: "[Role] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Categories_Role",
                table: "Categories",
                sql: "[Role] IS NULL OR ([Role] IN (1, 2, 3, 4) AND [DefaultScope] IS NULL AND (([Role] = 2 AND [Type] = 1) OR ([Role] <> 2 AND [Type] = 2)))");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_BudgetTransactions_UserId_FeeTransactionId",
                table: "Transfers",
                columns: new[] { "UserId", "FeeTransactionId" },
                principalTable: "BudgetTransactions",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_BudgetTransactions_UserId_FeeTransactionId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_UserId_FeeTransactionId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "UX_Categories_UserId_Role",
                table: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Categories_Role",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "FeeTransactionId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Categories");
        }
    }
}
