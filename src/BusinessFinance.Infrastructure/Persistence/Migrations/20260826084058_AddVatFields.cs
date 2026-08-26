using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVatFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "PosSettlements",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "PosSettlements",
                type: "decimal(5,4)",
                precision: 5,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "Obligations",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "Obligations",
                type: "decimal(5,4)",
                precision: 5,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "CreditCardCharges",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "CreditCardCharges",
                type: "decimal(5,4)",
                precision: 5,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "CounterpartyCharges",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "CounterpartyCharges",
                type: "decimal(5,4)",
                precision: 5,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "BudgetTransactions",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "BudgetTransactions",
                type: "decimal(5,4)",
                precision: 5,
                scale: 4,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_PosSettlements_VatAmount",
                table: "PosSettlements",
                sql: "[VatAmount] IS NULL OR ([VatAmount] >= 0 AND [VatAmount] <= [GrossAmount])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PosSettlements_VatRate",
                table: "PosSettlements",
                sql: "[VatRate] IS NULL OR ([VatRate] >= 0 AND [VatRate] < 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Obligations_VatAmount",
                table: "Obligations",
                sql: "[VatAmount] IS NULL OR ([VatAmount] >= 0 AND [VatAmount] <= [Amount])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Obligations_VatRate",
                table: "Obligations",
                sql: "[VatRate] IS NULL OR ([VatRate] >= 0 AND [VatRate] < 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditCardCharges_VatAmount",
                table: "CreditCardCharges",
                sql: "[VatAmount] IS NULL OR ([VatAmount] >= 0 AND [VatAmount] <= [Amount])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditCardCharges_VatRate",
                table: "CreditCardCharges",
                sql: "[VatRate] IS NULL OR ([VatRate] >= 0 AND [VatRate] < 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CounterpartyCharges_VatAmount",
                table: "CounterpartyCharges",
                sql: "[VatAmount] IS NULL OR ([VatAmount] >= 0 AND [VatAmount] <= [Amount])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CounterpartyCharges_VatRate",
                table: "CounterpartyCharges",
                sql: "[VatRate] IS NULL OR ([VatRate] >= 0 AND [VatRate] < 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BudgetTransactions_VatAmount",
                table: "BudgetTransactions",
                sql: "[VatAmount] IS NULL OR ([VatAmount] >= 0 AND [VatAmount] <= [Amount])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BudgetTransactions_VatRate",
                table: "BudgetTransactions",
                sql: "[VatRate] IS NULL OR ([VatRate] >= 0 AND [VatRate] < 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PosSettlements_VatAmount",
                table: "PosSettlements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PosSettlements_VatRate",
                table: "PosSettlements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Obligations_VatAmount",
                table: "Obligations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Obligations_VatRate",
                table: "Obligations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditCardCharges_VatAmount",
                table: "CreditCardCharges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditCardCharges_VatRate",
                table: "CreditCardCharges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CounterpartyCharges_VatAmount",
                table: "CounterpartyCharges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CounterpartyCharges_VatRate",
                table: "CounterpartyCharges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BudgetTransactions_VatAmount",
                table: "BudgetTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BudgetTransactions_VatRate",
                table: "BudgetTransactions");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "PosSettlements");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "PosSettlements");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "Obligations");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "Obligations");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "CreditCardCharges");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "CreditCardCharges");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "CounterpartyCharges");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "CounterpartyCharges");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "BudgetTransactions");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "BudgetTransactions");
        }
    }
}
