using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Vergi bir nakit planıdır (ADR 0018, Aşama 06.3 Grup 3): tekrarlayan plan
    /// vergi türü, "seçilen aylarda" ritmi ve ayın günü taşır; vergi planının
    /// tutarı ve kaynağı boş olabilir; kalem toplu bir ödemeyle "kapatılabilir";
    /// kategori bir vergi işareti taşır.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Veri kaybettirmez.</b> Mevcut her satır yeni kısıtları olduğu gibi
    /// sağlar: tutar ve kaynak doludur, sıklık 1–5'tir, vergi türü ve ay kümesi
    /// boştur. Yeni kolonlar nullable eklenir; hangi planın vergi olduğu
    /// geçmişte bilinmiyor ve uydurulmaz (<c>AGENTS.md</c> migration kuralı).
    /// </para>
    /// <para>
    /// Sıra kurallara uyar: eski CHECK'ler düşer, kolonlar eklenir ve gevşer,
    /// backfill çalışır, yeni CHECK'ler en son eklenir.
    /// </para>
    /// <para>
    /// <c>IsTax</c> EF'in ürettiği <c>defaultValue: false</c> ile eklenmez:
    /// kalıcı bir DEFAULT bırakırdı ve model onu istemiyor. Kolon nullable
    /// eklenir, doldurulur, sonra <c>NOT NULL</c> yapılır. Backfill varsayılan
    /// setlerin iki vergi kategorisini (işletme: "SGK ve vergi ödemesi",
    /// kişisel: "Vergi ve harç") bir kez işaretler — kategorinin varsayılan
    /// setten geldiği bilinen tek bilgi adıdır; kullanıcı sonra değiştirebilir
    /// (ADR 0018 T6).
    /// </para>
    /// </remarks>
    public partial class AddTaxPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Amount",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Frequency",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Source",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_SourceType",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_Amount",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_Realization",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_Source",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_SourceType",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_Status",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.AlterColumn<byte>(
                name: "SourceType",
                table: "RecurringTransactions",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RecurringTransactions",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)",
                oldPrecision: 19,
                oldScale: 4);

            migrationBuilder.AddColumn<byte>(
                name: "DayOfMonth",
                table: "RecurringTransactions",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SelectedMonths",
                table: "RecurringTransactions",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TaxKind",
                table: "RecurringTransactions",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "SourceType",
                table: "RecurringTransactionOccurrences",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RecurringTransactionOccurrences",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)",
                oldPrecision: 19,
                oldScale: 4);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ClosedAtUtc",
                table: "RecurringTransactionOccurrences",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClosedByChargeId",
                table: "RecurringTransactionOccurrences",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClosedByTransactionId",
                table: "RecurringTransactionOccurrences",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTax",
                table: "Categories",
                type: "bit",
                nullable: true);

            // Backfill, NOT NULL'dan ve CK_Categories_IsTax'tan önce.
            migrationBuilder.Sql(
                "UPDATE [Categories] SET [IsTax] = CASE WHEN [Type] = 2 AND " +
                "[Name] IN (N'SGK ve vergi ödemesi', N'Vergi ve harç') THEN 1 ELSE 0 END;");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTax",
                table: "Categories",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Amount",
                table: "RecurringTransactions",
                sql: "([Amount] IS NULL AND [TaxKind] IS NOT NULL) OR ([Amount] IS NOT NULL AND [Amount] > 0)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_DayOfMonth",
                table: "RecurringTransactions",
                sql: "[DayOfMonth] IS NULL OR ([DayOfMonth] BETWEEN 1 AND 31 AND [Frequency] NOT IN (1, 2))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Frequency",
                table: "RecurringTransactions",
                sql: "[Frequency] IN (1, 2, 3, 4, 5, 6)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_SelectedMonths",
                table: "RecurringTransactions",
                sql: "([Frequency] = 6 AND [SelectedMonths] IS NOT NULL AND [SelectedMonths] BETWEEN 1 AND 4095) OR ([Frequency] <> 6 AND [SelectedMonths] IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Source",
                table: "RecurringTransactions",
                sql: "([SourceType] IS NOT NULL AND [SourceType] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR ([SourceType] IS NOT NULL AND [SourceType] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL) OR ([SourceType] IS NULL AND [TaxKind] IS NOT NULL AND [AccountId] IS NULL AND [CreditCardId] IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_SourceType",
                table: "RecurringTransactions",
                sql: "[SourceType] IS NULL OR [SourceType] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_TaxKind",
                table: "RecurringTransactions",
                sql: "[TaxKind] IS NULL OR ([TaxKind] BETWEEN 1 AND 9 AND [Kind] = 2)");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringOccurrences_UserId_ClosedByChargeId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "ClosedByChargeId" },
                filter: "[ClosedByChargeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringOccurrences_UserId_ClosedByTransactionId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "ClosedByTransactionId" },
                filter: "[ClosedByTransactionId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_Amount",
                table: "RecurringTransactionOccurrences",
                sql: "[Amount] IS NULL OR [Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_Realization",
                table: "RecurringTransactionOccurrences",
                sql: "([Status] = 1 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NULL AND [RealizedAtUtc] IS NULL AND [ClosedByTransactionId] IS NULL AND [ClosedByChargeId] IS NULL AND [ClosedAtUtc] IS NULL) OR ([Status] = 2 AND [RealizedAtUtc] IS NOT NULL AND [Amount] IS NOT NULL AND [SourceType] IS NOT NULL AND [ClosedByTransactionId] IS NULL AND [ClosedByChargeId] IS NULL AND [ClosedAtUtc] IS NULL AND (([SourceType] = 1 AND [BudgetTransactionId] IS NOT NULL AND [CreditCardChargeId] IS NULL) OR ([SourceType] = 2 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NOT NULL))) OR ([Status] = 3 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NULL AND [RealizedAtUtc] IS NULL AND [ClosedAtUtc] IS NOT NULL AND (([ClosedByTransactionId] IS NOT NULL AND [ClosedByChargeId] IS NULL) OR ([ClosedByTransactionId] IS NULL AND [ClosedByChargeId] IS NOT NULL)))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_Source",
                table: "RecurringTransactionOccurrences",
                sql: "([SourceType] IS NOT NULL AND [SourceType] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR ([SourceType] IS NOT NULL AND [SourceType] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL) OR ([SourceType] IS NULL AND [AccountId] IS NULL AND [CreditCardId] IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_SourceType",
                table: "RecurringTransactionOccurrences",
                sql: "[SourceType] IS NULL OR [SourceType] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_Status",
                table: "RecurringTransactionOccurrences",
                sql: "[Status] IN (1, 2, 3)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Categories_IsTax",
                table: "Categories",
                sql: "[IsTax] = 0 OR [Type] = 2");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringTransactionOccurrences_BudgetTransactions_UserId_ClosedByTransactionId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "ClosedByTransactionId" },
                principalTable: "BudgetTransactions",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringTransactionOccurrences_CreditCardCharges_UserId_ClosedByChargeId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "ClosedByChargeId" },
                principalTable: "CreditCardCharges",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurringTransactionOccurrences_BudgetTransactions_UserId_ClosedByTransactionId",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringTransactionOccurrences_CreditCardCharges_UserId_ClosedByChargeId",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Amount",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_DayOfMonth",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Frequency",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_SelectedMonths",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Source",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_SourceType",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_TaxKind",
                table: "RecurringTransactions");

            migrationBuilder.DropIndex(
                name: "IX_RecurringOccurrences_UserId_ClosedByChargeId",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropIndex(
                name: "IX_RecurringOccurrences_UserId_ClosedByTransactionId",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_Amount",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_Realization",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_Source",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_SourceType",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_Status",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Categories_IsTax",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DayOfMonth",
                table: "RecurringTransactions");

            migrationBuilder.DropColumn(
                name: "SelectedMonths",
                table: "RecurringTransactions");

            migrationBuilder.DropColumn(
                name: "TaxKind",
                table: "RecurringTransactions");

            migrationBuilder.DropColumn(
                name: "ClosedAtUtc",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropColumn(
                name: "ClosedByChargeId",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropColumn(
                name: "ClosedByTransactionId",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropColumn(
                name: "IsTax",
                table: "Categories");

            migrationBuilder.AlterColumn<byte>(
                name: "SourceType",
                table: "RecurringTransactions",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RecurringTransactions",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)",
                oldPrecision: 19,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "SourceType",
                table: "RecurringTransactionOccurrences",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RecurringTransactionOccurrences",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)",
                oldPrecision: 19,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Amount",
                table: "RecurringTransactions",
                sql: "[Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Frequency",
                table: "RecurringTransactions",
                sql: "[Frequency] IN (1, 2, 3, 4, 5)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Source",
                table: "RecurringTransactions",
                sql: "([SourceType] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR ([SourceType] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_SourceType",
                table: "RecurringTransactions",
                sql: "[SourceType] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_Amount",
                table: "RecurringTransactionOccurrences",
                sql: "[Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_Realization",
                table: "RecurringTransactionOccurrences",
                sql: "([Status] = 1 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NULL AND [RealizedAtUtc] IS NULL) OR ([Status] = 2 AND [RealizedAtUtc] IS NOT NULL AND (([SourceType] = 1 AND [BudgetTransactionId] IS NOT NULL AND [CreditCardChargeId] IS NULL) OR ([SourceType] = 2 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NOT NULL)))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_Source",
                table: "RecurringTransactionOccurrences",
                sql: "([SourceType] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR ([SourceType] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_SourceType",
                table: "RecurringTransactionOccurrences",
                sql: "[SourceType] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_Status",
                table: "RecurringTransactionOccurrences",
                sql: "[Status] IN (1, 2)");
        }
    }
}
