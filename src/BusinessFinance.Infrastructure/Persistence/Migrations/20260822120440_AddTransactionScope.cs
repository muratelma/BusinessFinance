using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Kapsam boyutunu ekler: gelir/gider raporunu etkileyen her kayıt işletmeye
    /// mi sahibinin cebine mi ait olduğunu bilir; hesap, kart ve kategori de
    /// isteğe bağlı bir varsayılan taşıyabilir (ADR 0013).
    /// </summary>
    /// <remarks>
    /// Zorunlu kolonlar <c>NOT NULL</c> ve <b>varsayılansız</b> eklenir. Bu,
    /// <c>AGENTS.md</c> migration kurallarının istisnası değil, ön koşulunun
    /// sağlanmış hâlidir: tablolar Aşama 01 Grup 1'de boşaltıldı, yorumlanacak
    /// bir geçmiş yok. Dolu bir tabloda aynı sıra kullanılamaz — orada kolon
    /// nullable eklenir, backfill CHECK'ten önce çalışır.
    ///
    /// EF'in ürettiği <c>defaultValue: 0</c> kasıtlı olarak kaldırıldı. Kalıcı
    /// bir veritabanı varsayılanı bırakıyordu ve bıraktığı değer tablonun kendi
    /// <c>[Scope] IN (1, 2)</c> kısıtını ihlal ediyordu. Varsayılansız ekleme
    /// aynı zamanda ön koşulu da denetler: tablo boş değilse SQL Server bu
    /// komutu reddeder ve yükseltme sessizce yanlış veri üretmek yerine durur.
    /// </remarks>
    public partial class AddTransactionScope : Migration
    {
        private static readonly string[] ScopedTables =
        [
            "BudgetTransactions",
            "CreditCardCharges",
            "MonthlyBudgets",
            "InstallmentPlans",
            "RecurringTransactions",
            "RecurringTransactionOccurrences",
            "DebtAgreements"
        ];

        private static readonly string[] DefaultScopeTables =
        [
            "Accounts",
            "Categories",
            "CreditCards"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kolonlar kısıtlardan önce eklenir.
            foreach (var table in ScopedTables)
            {
                migrationBuilder.Sql($"ALTER TABLE [{table}] ADD [Scope] tinyint NOT NULL;");
            }

            foreach (var table in DefaultScopeTables)
            {
                migrationBuilder.AddColumn<byte>(
                    name: "DefaultScope",
                    table: table,
                    type: "tinyint",
                    nullable: true);
            }

            migrationBuilder.AddCheckConstraint(
                name: "CK_BudgetTransactions_Scope",
                table: "BudgetTransactions",
                sql: "[Scope] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditCardCharges_Scope",
                table: "CreditCardCharges",
                sql: "[Scope] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MonthlyBudgets_Scope",
                table: "MonthlyBudgets",
                sql: "[Scope] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InstallmentPlans_Scope",
                table: "InstallmentPlans",
                sql: "[Scope] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Scope",
                table: "RecurringTransactions",
                sql: "[Scope] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringOccurrences_Scope",
                table: "RecurringTransactionOccurrences",
                sql: "[Scope] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DebtAgreements_Scope",
                table: "DebtAgreements",
                sql: "[Scope] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_DefaultScope",
                table: "Accounts",
                sql: "[DefaultScope] IS NULL OR [DefaultScope] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Categories_DefaultScope",
                table: "Categories",
                sql: "[DefaultScope] IS NULL OR [DefaultScope] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditCards_DefaultScope",
                table: "CreditCards",
                sql: "[DefaultScope] IS NULL OR [DefaultScope] IN (1, 2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditCards_DefaultScope",
                table: "CreditCards");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Categories_DefaultScope",
                table: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_DefaultScope",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DebtAgreements_Scope",
                table: "DebtAgreements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringOccurrences_Scope",
                table: "RecurringTransactionOccurrences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Scope",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_InstallmentPlans_Scope",
                table: "InstallmentPlans");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MonthlyBudgets_Scope",
                table: "MonthlyBudgets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditCardCharges_Scope",
                table: "CreditCardCharges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BudgetTransactions_Scope",
                table: "BudgetTransactions");

            foreach (var table in DefaultScopeTables)
            {
                migrationBuilder.DropColumn(name: "DefaultScope", table: table);
            }

            foreach (var table in ScopedTables)
            {
                migrationBuilder.DropColumn(name: "Scope", table: table);
            }
        }
    }
}
