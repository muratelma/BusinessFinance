using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuarterlyRecurrence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Frequency",
                table: "RecurringTransactions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Frequency",
                table: "RecurringTransactions",
                sql: "[Frequency] IN (1, 2, 3, 4, 5)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Frequency",
                table: "RecurringTransactions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Frequency",
                table: "RecurringTransactions",
                sql: "[Frequency] IN (1, 2, 3, 4)");
        }
    }
}
