using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPosDeposits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Sıra: önce yeni tablo ve kolonlar, sonra backfill, en son kısıtlar
            // ve bağ. Kısıtlar backfill'den önce eklenseydi dolu bir
            // veritabanında yükseltme ilk satırda dururdu.
            migrationBuilder.CreateTable(
                name: "PosDeposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepositedAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    DeductionAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    DeductionTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepositDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosDeposits", x => x.Id);
                    table.UniqueConstraint("AK_PosDeposits_UserId_Id", x => new { x.UserId, x.Id });
                    table.UniqueConstraint("AK_PosDeposits_UserId_Id_DepositDate", x => new { x.UserId, x.Id, x.DepositDate });
                    table.CheckConstraint("CK_PosDeposits_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_PosDeposits_Deduction", "([DeductionAmount] = 0 AND [DeductionTransactionId] IS NULL) OR ([DeductionAmount] > 0 AND [DeductionTransactionId] IS NOT NULL)");
                    table.CheckConstraint("CK_PosDeposits_DepositedAmount", "[DepositedAmount] > 0");
                    table.ForeignKey(
                        name: "FK_PosDeposits_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosDeposits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosDeposits_BudgetTransactions_UserId_DeductionTransactionId",
                        columns: x => new { x.UserId, x.DeductionTransactionId },
                        principalTable: "BudgetTransactions",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.DropCheckConstraint(
                name: "CK_PosSettlements_Transfer",
                table: "PosSettlements");

            migrationBuilder.AddColumn<Guid>(
                name: "PosDepositId",
                table: "PosSettlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "PosSettlements",
                type: "rowversion",
                rowVersion: true,
                nullable: false);

            // Mevcut "hesaba geçti" verisi yatışa taşınır (ADR 0019 T5, KP12):
            // her geçiş bir yatıştır. Yatan tutar tahsilatın netidir ve kesinti
            // sıfırdır — eski yolda fark yazılamıyordu, uydurulmaz. Yatışın
            // kimliği önce tahsilata yazılır ki ikisi aynı satırdan eşleşsin.
            migrationBuilder.Sql(
                """
                UPDATE [PosSettlements]
                SET [PosDepositId] = NEWID()
                WHERE [TransferredOn] IS NOT NULL;

                INSERT INTO [PosDeposits]
                    ([Id], [UserId], [AccountId], [DepositedAmount], [Currency],
                     [DeductionAmount], [DeductionTransactionId], [DepositDate],
                     [CreatedAtUtc], [IsCancelled], [CancelledAtUtc])
                SELECT [PosDepositId], [UserId], [AccountId],
                       [GrossAmount] - [CommissionAmount], [Currency],
                       0, NULL, [TransferredOn],
                       [TransferredAtUtc], [IsCancelled], [CancelledAtUtc]
                FROM [PosSettlements]
                WHERE [PosDepositId] IS NOT NULL;
                """);

            // Hesaba geçtikten sonra iptal edilmiş tahsilat yeni kuralda bir
            // yatışa bağlı kalamaz. Geçiş bilgisi kaybolmaz: yukarıda aynı gün
            // ve tutarla, iptal edilmiş bir yatış olarak yazıldı.
            migrationBuilder.Sql(
                """
                UPDATE [PosSettlements]
                SET [PosDepositId] = NULL, [TransferredOn] = NULL, [TransferredAtUtc] = NULL
                WHERE [IsCancelled] = 1 AND [PosDepositId] IS NOT NULL;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_PosSettlements_UserId_PosDepositId_TransferredOn",
                table: "PosSettlements",
                columns: new[] { "UserId", "PosDepositId", "TransferredOn" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_PosSettlements_CancelledNotDeposited",
                table: "PosSettlements",
                sql: "[IsCancelled] = 0 OR [PosDepositId] IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PosSettlements_Transfer",
                table: "PosSettlements",
                sql: "([PosDepositId] IS NULL AND [TransferredOn] IS NULL AND [TransferredAtUtc] IS NULL) OR ([PosDepositId] IS NOT NULL AND [TransferredOn] IS NOT NULL AND [TransferredAtUtc] IS NOT NULL AND [TransferredOn] >= [SettlementDate])");

            migrationBuilder.CreateIndex(
                name: "IX_PosDeposits_UserId_AccountId",
                table: "PosDeposits",
                columns: new[] { "UserId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosDeposits_UserId_DepositDate",
                table: "PosDeposits",
                columns: new[] { "UserId", "DepositDate" });

            migrationBuilder.CreateIndex(
                name: "UX_PosDeposits_UserId_DeductionTransactionId",
                table: "PosDeposits",
                columns: new[] { "UserId", "DeductionTransactionId" },
                unique: true,
                filter: "[DeductionTransactionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PosSettlements_PosDeposits_UserId_PosDepositId_TransferredOn",
                table: "PosSettlements",
                columns: new[] { "UserId", "PosDepositId", "TransferredOn" },
                principalTable: "PosDeposits",
                principalColumns: new[] { "UserId", "Id", "DepositDate" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Yatış tablosu düşer; canlı tahsilatlar geçiş gününü korur ve eski
            // kısıt yeniden sağlanır. Kesinti giderleri sıradan gider olarak
            // kalır: geri dönüş yalnız geliştirme içindir.
            migrationBuilder.DropForeignKey(
                name: "FK_PosSettlements_PosDeposits_UserId_PosDepositId_TransferredOn",
                table: "PosSettlements");

            migrationBuilder.DropTable(
                name: "PosDeposits");

            migrationBuilder.DropIndex(
                name: "IX_PosSettlements_UserId_PosDepositId_TransferredOn",
                table: "PosSettlements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PosSettlements_CancelledNotDeposited",
                table: "PosSettlements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PosSettlements_Transfer",
                table: "PosSettlements");

            migrationBuilder.DropColumn(
                name: "PosDepositId",
                table: "PosSettlements");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PosSettlements");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PosSettlements_Transfer",
                table: "PosSettlements",
                sql: "([TransferredOn] IS NULL AND [TransferredAtUtc] IS NULL) OR ([TransferredOn] IS NOT NULL AND [TransferredAtUtc] IS NOT NULL AND [TransferredOn] >= [SettlementDate])");
        }
    }
}
