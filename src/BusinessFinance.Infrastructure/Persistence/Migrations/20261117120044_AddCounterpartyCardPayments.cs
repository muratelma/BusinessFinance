using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterpartyCardPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "CounterpartyPayments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardId",
                table: "CounterpartyPayments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyPayments_UserId_CreditCardId_Date",
                table: "CounterpartyPayments",
                columns: new[] { "UserId", "CreditCardId", "PaymentDate" },
                filter: "[CreditCardId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CounterpartyPayments_Source",
                table: "CounterpartyPayments",
                sql: "([AccountId] IS NULL AND [CreditCardId] IS NOT NULL AND [Direction] = 1 AND [PosSettlementId] IS NULL) OR ([AccountId] IS NOT NULL AND [CreditCardId] IS NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_CounterpartyPayments_CreditCards_UserId_CreditCardId",
                table: "CounterpartyPayments",
                columns: new[] { "UserId", "CreditCardId" },
                principalTable: "CreditCards",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Kişiye kartla ödeme hesapsızdır; eski şema onu taşıyamaz. Varsa
            // geri dönüş veri kaybettirmek yerine durur.
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [CounterpartyPayments] WHERE [CreditCardId] IS NOT NULL)
                    THROW 50025, 'Counterparties were paid by credit card; the downgrade would lose those payments.', 1;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_CounterpartyPayments_CreditCards_UserId_CreditCardId",
                table: "CounterpartyPayments");

            migrationBuilder.DropIndex(
                name: "IX_CounterpartyPayments_UserId_CreditCardId_Date",
                table: "CounterpartyPayments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CounterpartyPayments_Source",
                table: "CounterpartyPayments");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                table: "CounterpartyPayments");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "CounterpartyPayments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
