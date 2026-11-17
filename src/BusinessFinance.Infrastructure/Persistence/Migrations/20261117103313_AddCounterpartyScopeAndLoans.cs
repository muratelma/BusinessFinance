using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterpartyScopeAndLoans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Scope",
                table: "Counterparties",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CounterpartyLoans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CounterpartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    LoanDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterpartyLoans", x => x.Id);
                    table.UniqueConstraint("AK_CounterpartyLoans_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_CounterpartyLoans_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_CounterpartyLoans_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_CounterpartyLoans_Direction", "[Direction] IN (1, 2)");
                    table.CheckConstraint("CK_CounterpartyLoans_DueDate", "[DueDate] IS NULL OR [DueDate] >= [LoanDate]");
                    table.ForeignKey(
                        name: "FK_CounterpartyLoans_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounterpartyLoans_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounterpartyLoans_Counterparties_UserId_CounterpartyId",
                        columns: x => new { x.UserId, x.CounterpartyId },
                        principalTable: "Counterparties",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Counterparties_Scope",
                table: "Counterparties",
                sql: "[Scope] IS NULL OR [Scope] IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyLoans_UserId_AccountId_Date",
                table: "CounterpartyLoans",
                columns: new[] { "UserId", "AccountId", "LoanDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyLoans_UserId_CounterpartyId_Cancelled_Direction",
                table: "CounterpartyLoans",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled", "Direction", "DueDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Borç para ya da kişinin türü yazılmışsa geri dönüş onları siler;
            // veri kaybettirmek yerine durur.
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [CounterpartyLoans])
                   OR EXISTS (SELECT 1 FROM [Counterparties] WHERE [Scope] IS NOT NULL)
                    THROW 50023, 'Counterparties carry a kind or loans; the downgrade would lose them.', 1;
                """);

            migrationBuilder.DropTable(
                name: "CounterpartyLoans");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Counterparties_Scope",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Counterparties");
        }
    }
}
