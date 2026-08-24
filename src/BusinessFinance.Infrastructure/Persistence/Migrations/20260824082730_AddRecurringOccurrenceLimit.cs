using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringOccurrenceLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GeneratedOccurrenceCount",
                table: "RecurringTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OccurrenceLimit",
                table: "RecurringTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [RecurringTransactions]
                SET [GeneratedOccurrenceCount] =
                (
                    SELECT COUNT(*)
                    FROM [RecurringTransactionOccurrences]
                    WHERE [RecurringTransactionOccurrences].[UserId] = [RecurringTransactions].[UserId]
                      AND [RecurringTransactionOccurrences].[RecurringTransactionId] = [RecurringTransactions].[Id]
                );
                """);

            migrationBuilder.AlterColumn<int>(
                name: "GeneratedOccurrenceCount",
                table: "RecurringTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_GeneratedOccurrenceCount",
                table: "RecurringTransactions",
                sql: "[GeneratedOccurrenceCount] >= 0 AND ([OccurrenceLimit] IS NULL OR [GeneratedOccurrenceCount] <= [OccurrenceLimit])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_OccurrenceLimit",
                table: "RecurringTransactions",
                sql: "[OccurrenceLimit] IS NULL OR [OccurrenceLimit] > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_GeneratedOccurrenceCount",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_OccurrenceLimit",
                table: "RecurringTransactions");

            migrationBuilder.DropColumn(
                name: "GeneratedOccurrenceCount",
                table: "RecurringTransactions");

            migrationBuilder.DropColumn(
                name: "OccurrenceLimit",
                table: "RecurringTransactions");
        }
    }
}
