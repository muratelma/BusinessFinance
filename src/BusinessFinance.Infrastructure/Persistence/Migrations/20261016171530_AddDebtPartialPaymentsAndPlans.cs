using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDebtPartialPaymentsAndPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Debts_DateRange",
                table: "Debts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Debts_PlanKind",
                table: "Debts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Debts_SinglePlan",
                table: "Debts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Debts_Source",
                table: "Debts");

            migrationBuilder.DropIndex(
                name: "UX_DebtPaymentAllocations_UserId_ScheduleItemId",
                table: "DebtPaymentAllocations");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DueDate",
                table: "DebtScheduleItems",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperseded",
                table: "DebtScheduleItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanRevision",
                table: "DebtScheduleItems",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FirstDueDate",
                table: "Debts",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<bool>(
                name: "IsCarriedOver",
                table: "Debts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanRevision",
                table: "Debts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Revision",
                table: "Debts",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "DebtPayments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardId",
                table: "DebtPayments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Kind",
                table: "DebtPayments",
                type: "tinyint",
                nullable: true);

            // Kolonlar önce boş bırakılabilir eklenir, doldurulur, sonra zorunlu
            // olur: `defaultValue` kalıcı bir DEFAULT bırakırdı. Değerler eski
            // satırlar için bilinen gerçektir, uydurma değildir: hiçbir kalem
            // yerini yeni plana bırakmadı, hiçbir borç devir değildi, her
            // kapanış gerçek bir ödemeydi (indirim bu adımla geldi).
            migrationBuilder.Sql(
                """
                UPDATE [DebtScheduleItems] SET [IsSuperseded] = 0, [PlanRevision] = 0;
                UPDATE [Debts] SET [IsCarriedOver] = 0, [PlanRevision] = 0, [Revision] = 0;
                UPDATE [DebtPayments] SET [Kind] = 1;
                """);

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuperseded",
                table: "DebtScheduleItems",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlanRevision",
                table: "DebtScheduleItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCarriedOver",
                table: "Debts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlanRevision",
                table: "Debts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Revision",
                table: "Debts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Kind",
                table: "DebtPayments",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Debts_DateRange",
                table: "Debts",
                sql: "[FirstDueDate] IS NULL OR [FirstDueDate] >= [StartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Debts_PlanDueDate",
                table: "Debts",
                sql: "[PlanKind] = 3 OR [FirstDueDate] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Debts_PlanKind",
                table: "Debts",
                sql: "[PlanKind] IN (1, 2, 3)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Debts_SinglePlan",
                table: "Debts",
                sql: "[PlanKind] = 2 OR [InstallmentCount] = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Debts_Source",
                table: "Debts",
                sql: "([IsCarriedOver] = 1 AND [SourceType] IN (1, 2, 3) AND [OpeningAccountId] IS NULL AND [CategoryId] IS NULL) OR ([IsCarriedOver] = 0 AND [SourceType] = 0 AND [OpeningAccountId] IS NULL AND [CategoryId] IS NULL) OR ([IsCarriedOver] = 0 AND [SourceType] = 1 AND [OpeningAccountId] IS NOT NULL AND [CategoryId] IS NULL) OR ([IsCarriedOver] = 0 AND [SourceType] IN (2, 3) AND [OpeningAccountId] IS NULL AND [CategoryId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_UserId_CreditCardId_Date",
                table: "DebtPayments",
                columns: new[] { "UserId", "CreditCardId", "PaymentDate" },
                filter: "[CreditCardId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DebtPayments_Kind",
                table: "DebtPayments",
                sql: "[Kind] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DebtPayments_Source",
                table: "DebtPayments",
                sql: "([Kind] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR ([Kind] = 1 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL AND [PosSettlementId] IS NULL AND [Direction] = 1) OR ([Kind] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NULL AND [PosSettlementId] IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_DebtPaymentAllocations_UserId_ScheduleItemId",
                table: "DebtPaymentAllocations",
                columns: new[] { "UserId", "ScheduleItemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DebtPayments_CreditCards_UserId_CreditCardId",
                table: "DebtPayments",
                columns: new[] { "UserId", "CreditCardId" },
                principalTable: "CreditCards",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eski şema planı olmayan borcu, hesapsız ödemeyi (kart, indirim) ve
            // yerini yeni plana bırakmış kalemi temsil edemez. Böyle kayıt varsa
            // geri dönüş veri kaybettirirdi; sessizce yapılmaz.
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [Debts] WHERE [PlanKind] = 3 OR [FirstDueDate] IS NULL OR [IsCarriedOver] = 1)
                   OR EXISTS (SELECT 1 FROM [DebtPayments] WHERE [Kind] <> 1 OR [AccountId] IS NULL)
                   OR EXISTS (SELECT 1 FROM [DebtScheduleItems] WHERE [IsSuperseded] = 1 OR [DueDate] IS NULL)
                    THROW 50021, 'Debt records use partial payments, cards, discounts or plan changes; the downgrade would lose them.', 1;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_DebtPayments_CreditCards_UserId_CreditCardId",
                table: "DebtPayments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Debts_DateRange",
                table: "Debts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Debts_PlanDueDate",
                table: "Debts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Debts_PlanKind",
                table: "Debts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Debts_SinglePlan",
                table: "Debts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Debts_Source",
                table: "Debts");

            migrationBuilder.DropIndex(
                name: "IX_DebtPayments_UserId_CreditCardId_Date",
                table: "DebtPayments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DebtPayments_Kind",
                table: "DebtPayments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DebtPayments_Source",
                table: "DebtPayments");

            migrationBuilder.DropIndex(
                name: "IX_DebtPaymentAllocations_UserId_ScheduleItemId",
                table: "DebtPaymentAllocations");

            migrationBuilder.DropColumn(
                name: "IsSuperseded",
                table: "DebtScheduleItems");

            migrationBuilder.DropColumn(
                name: "PlanRevision",
                table: "DebtScheduleItems");

            migrationBuilder.DropColumn(
                name: "IsCarriedOver",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "PlanRevision",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "Revision",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                table: "DebtPayments");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "DebtPayments");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DueDate",
                table: "DebtScheduleItems",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FirstDueDate",
                table: "Debts",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "DebtPayments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Debts_DateRange",
                table: "Debts",
                sql: "[FirstDueDate] >= [StartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Debts_PlanKind",
                table: "Debts",
                sql: "[PlanKind] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Debts_SinglePlan",
                table: "Debts",
                sql: "[PlanKind] <> 1 OR [InstallmentCount] = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Debts_Source",
                table: "Debts",
                sql: "([SourceType] = 0 AND [OpeningAccountId] IS NULL AND [CategoryId] IS NULL) OR ([SourceType] = 1 AND [OpeningAccountId] IS NOT NULL AND [CategoryId] IS NULL) OR ([SourceType] IN (2, 3) AND [OpeningAccountId] IS NULL AND [CategoryId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_DebtPaymentAllocations_UserId_ScheduleItemId",
                table: "DebtPaymentAllocations",
                columns: new[] { "UserId", "ScheduleItemId" },
                unique: true);
        }
    }
}
