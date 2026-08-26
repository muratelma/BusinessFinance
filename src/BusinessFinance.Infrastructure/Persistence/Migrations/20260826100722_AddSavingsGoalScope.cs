using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSavingsGoalScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Scope",
                table: "SavingsGoals",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_SavingsGoals_Scope",
                table: "SavingsGoals",
                sql: "[Scope] IS NULL OR [Scope] IN (1, 2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SavingsGoals_Scope",
                table: "SavingsGoals");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "SavingsGoals");
        }
    }
}
