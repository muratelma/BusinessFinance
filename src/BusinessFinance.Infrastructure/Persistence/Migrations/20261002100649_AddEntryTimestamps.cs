using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEntryTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "Transfers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "DebtAgreements",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "CreditCardPayments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "CreditCardCharges",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "CounterpartyPayments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "CounterpartyCharges",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "BudgetTransactions",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "DebtAgreements");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "CreditCardPayments");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "CreditCardCharges");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "CounterpartyPayments");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "CounterpartyCharges");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "BudgetTransactions");
        }
    }
}
