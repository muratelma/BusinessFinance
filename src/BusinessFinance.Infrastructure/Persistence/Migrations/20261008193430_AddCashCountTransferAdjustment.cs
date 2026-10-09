using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCashCountTransferAdjustment : Migration
    {
        /// <summary>
        /// Kasa sayımının farkı bir aktarımla da açıklanabilir ("Kendime
        /// aldım", şahsi hesaba).
        /// </summary>
        /// <remarks>
        /// Var olan sayımlarda bağ bilinmez ve boş kalır; doldurma yoktur.
        /// Kolon kısıttan önce eklenir. Yeni kısıt eskisini kapsar: bir sayım
        /// ya açıklamasızdır ya tam olarak bir kayıtla (gelir/gider ya da
        /// aktarım) açıklanmıştır. Aktarımlara eklenen (UserId, Id) anahtarı
        /// bağın sahiplik kapsamını veritabanında tutar; var olan satırlarda
        /// zaten tekildir (Id birincil anahtar).
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CashCounts_Adjustment",
                table: "CashCounts");

            migrationBuilder.AddColumn<Guid>(
                name: "AdjustmentTransferId",
                table: "CashCounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Transfers_UserId_Id",
                table: "Transfers",
                columns: new[] { "UserId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_CashCounts_UserId_AdjustmentTransferId",
                table: "CashCounts",
                columns: new[] { "UserId", "AdjustmentTransferId" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_CashCounts_Adjustment",
                table: "CashCounts",
                sql: "([AdjustmentTransactionId] IS NULL AND [AdjustmentTransferId] IS NULL AND [AdjustedAtUtc] IS NULL) OR ([AdjustmentTransactionId] IS NOT NULL AND [AdjustmentTransferId] IS NULL AND [AdjustedAtUtc] IS NOT NULL) OR ([AdjustmentTransactionId] IS NULL AND [AdjustmentTransferId] IS NOT NULL AND [AdjustedAtUtc] IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_CashCounts_Transfers_UserId_AdjustmentTransferId",
                table: "CashCounts",
                columns: new[] { "UserId", "AdjustmentTransferId" },
                principalTable: "Transfers",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashCounts_Transfers_UserId_AdjustmentTransferId",
                table: "CashCounts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Transfers_UserId_Id",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_CashCounts_UserId_AdjustmentTransferId",
                table: "CashCounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CashCounts_Adjustment",
                table: "CashCounts");

            migrationBuilder.DropColumn(
                name: "AdjustmentTransferId",
                table: "CashCounts");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CashCounts_Adjustment",
                table: "CashCounts",
                sql: "([AdjustmentTransactionId] IS NULL AND [AdjustedAtUtc] IS NULL) OR ([AdjustmentTransactionId] IS NOT NULL AND [AdjustedAtUtc] IS NOT NULL)");
        }
    }
}
