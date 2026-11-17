using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterpartyPaidNow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaidWithChargeId",
                table: "CounterpartyPayments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyPayments_UserId_PaidWithChargeId",
                table: "CounterpartyPayments",
                columns: new[] { "UserId", "PaidWithChargeId" },
                filter: "[PaidWithChargeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CounterpartyPayments_CounterpartyCharges_UserId_PaidWithChargeId",
                table: "CounterpartyPayments",
                columns: new[] { "UserId", "PaidWithChargeId" },
                principalTable: "CounterpartyCharges",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Satışa bağlanmış peşin ya da avans tutarı varsa geri dönüş bağı
            // siler; veri kaybettirmek yerine durur.
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [CounterpartyPayments] WHERE [PaidWithChargeId] IS NOT NULL)
                    THROW 50024, 'Payments are linked to the charges they were written with; the downgrade would lose the link.', 1;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_CounterpartyPayments_CounterpartyCharges_UserId_PaidWithChargeId",
                table: "CounterpartyPayments");

            migrationBuilder.DropIndex(
                name: "IX_CounterpartyPayments_UserId_PaidWithChargeId",
                table: "CounterpartyPayments");

            migrationBuilder.DropColumn(
                name: "PaidWithChargeId",
                table: "CounterpartyPayments");
        }
    }
}
