using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxDeductibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTaxDeductible",
                table: "Obligations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxDeductible",
                table: "CreditCardCharges",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxDeductible",
                table: "CounterpartyCharges",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DefaultIsTaxDeductible",
                table: "Categories",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxDeductible",
                table: "BudgetTransactions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Obligations_IsTaxDeductible",
                table: "Obligations",
                sql: "[IsTaxDeductible] IS NULL OR ([Scope] = 1 AND [Direction] = 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditCardCharges_IsTaxDeductible",
                table: "CreditCardCharges",
                sql: "[IsTaxDeductible] IS NULL OR [Scope] = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CounterpartyCharges_IsTaxDeductible",
                table: "CounterpartyCharges",
                sql: "[IsTaxDeductible] IS NULL OR ([Scope] = 1 AND [Direction] = 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Categories_IsTaxDeductible",
                table: "Categories",
                sql: "[DefaultIsTaxDeductible] IS NULL OR [Type] = 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BudgetTransactions_IsTaxDeductible",
                table: "BudgetTransactions",
                sql: "[IsTaxDeductible] IS NULL OR ([Scope] = 1 AND [Type] = 2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Obligations_IsTaxDeductible",
                table: "Obligations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditCardCharges_IsTaxDeductible",
                table: "CreditCardCharges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CounterpartyCharges_IsTaxDeductible",
                table: "CounterpartyCharges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Categories_IsTaxDeductible",
                table: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BudgetTransactions_IsTaxDeductible",
                table: "BudgetTransactions");

            migrationBuilder.DropColumn(
                name: "IsTaxDeductible",
                table: "Obligations");

            migrationBuilder.DropColumn(
                name: "IsTaxDeductible",
                table: "CreditCardCharges");

            migrationBuilder.DropColumn(
                name: "IsTaxDeductible",
                table: "CounterpartyCharges");

            migrationBuilder.DropColumn(
                name: "DefaultIsTaxDeductible",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsTaxDeductible",
                table: "BudgetTransactions");
        }
    }
}
