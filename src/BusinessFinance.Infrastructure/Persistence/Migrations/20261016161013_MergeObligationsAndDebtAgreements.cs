using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MergeObligationsAndDebtAgreements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Debts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CounterpartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    Scope = table.Column<byte>(type: "tinyint", nullable: false),
                    Principal = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    TotalRepayment = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    TotalCurrency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    AnnualInterestRate = table.Column<decimal>(type: "decimal(7,4)", precision: 7, scale: 4, nullable: false),
                    SourceType = table.Column<byte>(type: "tinyint", nullable: false),
                    OpeningAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PlanKind = table.Column<byte>(type: "tinyint", nullable: false),
                    FirstDueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InstallmentCount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debts", x => x.Id);
                    table.UniqueConstraint("AK_Debts_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_Debts_Currency", "[Currency] = 'TRY' AND [TotalCurrency] = [Currency]");
                    table.CheckConstraint("CK_Debts_DateRange", "[FirstDueDate] >= [StartDate]");
                    table.CheckConstraint("CK_Debts_Direction", "[Direction] IN (1, 2)");
                    table.CheckConstraint("CK_Debts_DirectionalSource", "([Direction] = 1 AND [SourceType] <> 3) OR ([Direction] = 2 AND [SourceType] <> 2)");
                    table.CheckConstraint("CK_Debts_InstallmentCount", "[InstallmentCount] BETWEEN 1 AND 360");
                    table.CheckConstraint("CK_Debts_InterestRate", "[AnnualInterestRate] >= 0 AND [AnnualInterestRate] <= 1000");
                    table.CheckConstraint("CK_Debts_PlanKind", "[PlanKind] IN (1, 2)");
                    table.CheckConstraint("CK_Debts_Principal", "[Principal] > 0");
                    table.CheckConstraint("CK_Debts_Scope", "[Scope] IN (1, 2)");
                    table.CheckConstraint("CK_Debts_SinglePlan", "[PlanKind] <> 1 OR [InstallmentCount] = 1");
                    table.CheckConstraint("CK_Debts_Source", "([SourceType] = 0 AND [OpeningAccountId] IS NULL AND [CategoryId] IS NULL) OR ([SourceType] = 1 AND [OpeningAccountId] IS NOT NULL AND [CategoryId] IS NULL) OR ([SourceType] IN (2, 3) AND [OpeningAccountId] IS NULL AND [CategoryId] IS NOT NULL)");
                    table.CheckConstraint("CK_Debts_SourceType", "[SourceType] IN (0, 1, 2, 3)");
                    table.CheckConstraint("CK_Debts_TotalRepayment", "[TotalRepayment] >= [Principal]");
                    table.ForeignKey(
                        name: "FK_Debts_Accounts_UserId_OpeningAccountId",
                        columns: x => new { x.UserId, x.OpeningAccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Debts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Debts_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Debts_Counterparties_UserId_CounterpartyId",
                        columns: x => new { x.UserId, x.CounterpartyId },
                        principalTable: "Counterparties",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DebtPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DebtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaidAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PosSettlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtPayments", x => x.Id);
                    table.UniqueConstraint("AK_DebtPayments_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_DebtPayments_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_DebtPayments_Currency", "[Currency] = 'TRY'");
                    table.CheckConstraint("CK_DebtPayments_Direction", "[Direction] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_DebtPayments_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DebtPayments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DebtPayments_Debts_UserId_DebtId",
                        columns: x => new { x.UserId, x.DebtId },
                        principalTable: "Debts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DebtPayments_PosSettlements_UserId_PosSettlementId",
                        columns: x => new { x.UserId, x.PosSettlementId },
                        principalTable: "PosSettlements",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DebtScheduleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DebtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    PrincipalPortion = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    InterestPortion = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtScheduleItems", x => x.Id);
                    table.UniqueConstraint("AK_DebtScheduleItems_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_DebtScheduleItems_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_DebtScheduleItems_Sequence", "[Sequence] >= 1");
                    table.CheckConstraint("CK_DebtScheduleItems_Split", "([PrincipalPortion] IS NULL AND [InterestPortion] IS NULL) OR ([PrincipalPortion] IS NOT NULL AND [InterestPortion] IS NOT NULL AND [PrincipalPortion] >= 0 AND [InterestPortion] >= 0 AND [PrincipalPortion] + [InterestPortion] = [Amount])");
                    table.ForeignKey(
                        name: "FK_DebtScheduleItems_Debts_UserId_DebtId",
                        columns: x => new { x.UserId, x.DebtId },
                        principalTable: "Debts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebtPaymentAllocations",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtPaymentAllocations", x => new { x.PaymentId, x.ScheduleItemId });
                    table.CheckConstraint("CK_DebtPaymentAllocations_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_DebtPaymentAllocations_DebtPayments_UserId_PaymentId",
                        columns: x => new { x.UserId, x.PaymentId },
                        principalTable: "DebtPayments",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebtPaymentAllocations_DebtScheduleItems_UserId_ScheduleItemId",
                        columns: x => new { x.UserId, x.ScheduleItemId },
                        principalTable: "DebtScheduleItems",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebtPaymentAllocations_UserId_PaymentId",
                table: "DebtPaymentAllocations",
                columns: new[] { "UserId", "PaymentId" });

            migrationBuilder.CreateIndex(
                name: "UX_DebtPaymentAllocations_UserId_ScheduleItemId",
                table: "DebtPaymentAllocations",
                columns: new[] { "UserId", "ScheduleItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_UserId_AccountId_Date",
                table: "DebtPayments",
                columns: new[] { "UserId", "AccountId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_UserId_DebtId",
                table: "DebtPayments",
                columns: new[] { "UserId", "DebtId" });

            migrationBuilder.CreateIndex(
                name: "UX_DebtPayments_UserId_PosSettlementId",
                table: "DebtPayments",
                columns: new[] { "UserId", "PosSettlementId" },
                unique: true,
                filter: "[PosSettlementId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Debts_UserId_CategoryId_StartDate",
                table: "Debts",
                columns: new[] { "UserId", "CategoryId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Debts_UserId_CounterpartyId_Cancelled",
                table: "Debts",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled" });

            migrationBuilder.CreateIndex(
                name: "IX_Debts_UserId_OpeningAccountId",
                table: "Debts",
                columns: new[] { "UserId", "OpeningAccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_Debts_UserId_PlanKind_Cancelled_FirstDueDate",
                table: "Debts",
                columns: new[] { "UserId", "PlanKind", "IsCancelled", "FirstDueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DebtScheduleItems_UserId_DebtId_Sequence",
                table: "DebtScheduleItems",
                columns: new[] { "UserId", "DebtId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebtScheduleItems_UserId_DueDate",
                table: "DebtScheduleItems",
                columns: new[] { "UserId", "DueDate" });

            // Veri taşıma (ADR 0021 dalga 1). Kimlikler korunur: yükümlülük → borç,
            // tek kalemi borcun kimliğini taşır; kapanış → ödeme; taksit → kalem ve
            // o taksidin ödemesi. Gün sonunun saydığı kayıtlar (FK'siz) bu yüzden
            // bağlı kalır. Yeni tablolar boşken kurulur, eskiler en son düşer.
            migrationBuilder.Sql(
                """
                INSERT INTO [Debts]
                    ([Id], [UserId], [CounterpartyId], [Direction], [Scope], [Principal], [Currency],
                     [TotalRepayment], [TotalCurrency], [AnnualInterestRate], [SourceType],
                     [OpeningAccountId], [CategoryId], [StartDate], [PlanKind], [FirstDueDate],
                     [InstallmentCount], [Description], [CreatedAtUtc], [IsCancelled], [CancelledAtUtc])
                SELECT [Id], [UserId], [CounterpartyId], [Direction], [Scope], [Principal], [Currency],
                       [TotalRepayment], [TotalCurrency], [AnnualInterestRate], [SourceType],
                       [OpeningAccountId], [CategoryId], [StartDate], 2, [FirstDueDate],
                       [InstallmentCount], [Description], [CreatedAtUtc], 0, NULL
                FROM [DebtAgreements];

                -- Eski para birimi tinyint 1 = TRY. Yön kaynağı belirler:
                -- borç (1) gider (2), alacak (2) gelir (3) tanır.
                INSERT INTO [Debts]
                    ([Id], [UserId], [CounterpartyId], [Direction], [Scope], [Principal], [Currency],
                     [TotalRepayment], [TotalCurrency], [AnnualInterestRate], [SourceType],
                     [OpeningAccountId], [CategoryId], [StartDate], [PlanKind], [FirstDueDate],
                     [InstallmentCount], [Description], [CreatedAtUtc], [IsCancelled], [CancelledAtUtc])
                SELECT [Id], [UserId], [CounterpartyId], [Direction], [Scope], [Amount], 'TRY',
                       [Amount], 'TRY', 0, CASE [Direction] WHEN 1 THEN 2 ELSE 3 END,
                       NULL, [CategoryId], [IssueDate], 1, [DueDate],
                       1, [Description], [CreatedAtUtc], [IsCancelled], [CancelledAtUtc]
                FROM [Obligations];

                INSERT INTO [DebtScheduleItems]
                    ([Id], [UserId], [DebtId], [Sequence], [Amount], [Currency],
                     [PrincipalPortion], [InterestPortion], [DueDate])
                SELECT [Id], [UserId], [DebtAgreementId], [Sequence], [Amount], [Currency],
                       [PrincipalPortion], [InterestPortion], [DueDate]
                FROM [DebtInstallments];

                INSERT INTO [DebtScheduleItems]
                    ([Id], [UserId], [DebtId], [Sequence], [Amount], [Currency],
                     [PrincipalPortion], [InterestPortion], [DueDate])
                SELECT [Id], [UserId], [Id], 1, [Amount], 'TRY', [Amount], 0, [DueDate]
                FROM [Obligations];

                -- Ödenmiş taksit: ödemenin kimliği taksidinkidir.
                INSERT INTO [DebtPayments]
                    ([Id], [UserId], [DebtId], [Direction], [AccountId], [Amount], [Currency],
                     [PaymentDate], [PaidAtUtc], [PosSettlementId], [IsCancelled], [CancelledAtUtc])
                SELECT installment.[Id], installment.[UserId], installment.[DebtAgreementId],
                       agreement.[Direction], installment.[PaymentAccountId], installment.[Amount],
                       installment.[Currency], installment.[PaymentDate], installment.[PaidAtUtc],
                       NULL, 0, NULL
                FROM [DebtInstallments] AS installment
                INNER JOIN [DebtAgreements] AS agreement
                    ON agreement.[UserId] = installment.[UserId]
                   AND agreement.[Id] = installment.[DebtAgreementId]
                WHERE installment.[PaymentAccountId] IS NOT NULL;

                INSERT INTO [DebtPaymentAllocations] ([PaymentId], [ScheduleItemId], [UserId], [Amount])
                SELECT [Id], [Id], [UserId], [Amount]
                FROM [DebtInstallments]
                WHERE [PaymentAccountId] IS NOT NULL;

                INSERT INTO [DebtPayments]
                    ([Id], [UserId], [DebtId], [Direction], [AccountId], [Amount], [Currency],
                     [PaymentDate], [PaidAtUtc], [PosSettlementId], [IsCancelled], [CancelledAtUtc])
                SELECT [Id], [UserId], [ObligationId], [Direction], [AccountId], [Amount], 'TRY',
                       [SettlementDate], [SettledAtUtc], [PosSettlementId], [IsCancelled], [CancelledAtUtc]
                FROM [ObligationSettlements];

                INSERT INTO [DebtPaymentAllocations] ([PaymentId], [ScheduleItemId], [UserId], [Amount])
                SELECT [Id], [ObligationId], [UserId], [Amount]
                FROM [ObligationSettlements];
                """);

            migrationBuilder.DropTable(
                name: "DebtInstallments");

            migrationBuilder.DropTable(
                name: "ObligationSettlements");

            migrationBuilder.DropTable(
                name: "DebtAgreements");

            migrationBuilder.DropTable(
                name: "Obligations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DebtAgreements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnnualInterestRate = table.Column<decimal>(type: "decimal(7,4)", precision: 7, scale: 4, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CounterpartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    FirstDueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InstallmentCount = table.Column<int>(type: "int", nullable: false),
                    OpeningAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Scope = table.Column<byte>(type: "tinyint", nullable: false),
                    SourceType = table.Column<byte>(type: "tinyint", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Principal = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    TotalRepayment = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    TotalCurrency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtAgreements", x => x.Id);
                    table.UniqueConstraint("AK_DebtAgreements_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_DebtAgreements_Currency", "[Currency] = 'TRY' AND [TotalCurrency] = [Currency]");
                    table.CheckConstraint("CK_DebtAgreements_DateRange", "[FirstDueDate] >= [StartDate]");
                    table.CheckConstraint("CK_DebtAgreements_Direction", "[Direction] IN (1, 2)");
                    table.CheckConstraint("CK_DebtAgreements_DirectionalSource", "([Direction] = 1 AND [SourceType] <> 3) OR ([Direction] = 2 AND [SourceType] <> 2)");
                    table.CheckConstraint("CK_DebtAgreements_InstallmentCount", "[InstallmentCount] BETWEEN 1 AND 360");
                    table.CheckConstraint("CK_DebtAgreements_InterestRate", "[AnnualInterestRate] >= 0 AND [AnnualInterestRate] <= 1000");
                    table.CheckConstraint("CK_DebtAgreements_Principal", "[Principal] > 0");
                    table.CheckConstraint("CK_DebtAgreements_Scope", "[Scope] IN (1, 2)");
                    table.CheckConstraint("CK_DebtAgreements_Source", "([SourceType] = 0 AND [OpeningAccountId] IS NULL AND [CategoryId] IS NULL) OR ([SourceType] = 1 AND [OpeningAccountId] IS NOT NULL AND [CategoryId] IS NULL) OR ([SourceType] IN (2, 3) AND [OpeningAccountId] IS NULL AND [CategoryId] IS NOT NULL)");
                    table.CheckConstraint("CK_DebtAgreements_SourceType", "[SourceType] IN (0, 1, 2, 3)");
                    table.CheckConstraint("CK_DebtAgreements_TotalRepayment", "[TotalRepayment] >= [Principal]");
                    table.ForeignKey(
                        name: "FK_DebtAgreements_Accounts_UserId_OpeningAccountId",
                        columns: x => new { x.UserId, x.OpeningAccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DebtAgreements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DebtAgreements_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DebtAgreements_Counterparties_UserId_CounterpartyId",
                        columns: x => new { x.UserId, x.CounterpartyId },
                        principalTable: "Counterparties",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Obligations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CounterpartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Scope = table.Column<byte>(type: "tinyint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligations", x => x.Id);
                    table.UniqueConstraint("AK_Obligations_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_Obligations_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_Obligations_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_Obligations_Direction", "[Direction] IN (1, 2)");
                    table.CheckConstraint("CK_Obligations_DueDate", "[DueDate] >= [IssueDate]");
                    table.CheckConstraint("CK_Obligations_Scope", "[Scope] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_Obligations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obligations_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obligations_Counterparties_UserId_CounterpartyId",
                        columns: x => new { x.UserId, x.CounterpartyId },
                        principalTable: "Counterparties",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DebtInstallments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DebtAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InterestPortion = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    PaidAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PaymentAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PrincipalPortion = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtInstallments", x => x.Id);
                    table.CheckConstraint("CK_DebtInstallments_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_DebtInstallments_Payment", "([PaymentAccountId] IS NULL AND [PaymentDate] IS NULL AND [PaidAtUtc] IS NULL) OR ([PaymentAccountId] IS NOT NULL AND [PaymentDate] IS NOT NULL AND [PaidAtUtc] IS NOT NULL)");
                    table.CheckConstraint("CK_DebtInstallments_Sequence", "[Sequence] >= 1");
                    table.CheckConstraint("CK_DebtInstallments_Split", "([PrincipalPortion] IS NULL AND [InterestPortion] IS NULL) OR ([PrincipalPortion] IS NOT NULL AND [InterestPortion] IS NOT NULL AND [PrincipalPortion] >= 0 AND [InterestPortion] >= 0 AND [PrincipalPortion] + [InterestPortion] = [Amount])");
                    table.ForeignKey(
                        name: "FK_DebtInstallments_Accounts_UserId_PaymentAccountId",
                        columns: x => new { x.UserId, x.PaymentAccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DebtInstallments_DebtAgreements_UserId_DebtAgreementId",
                        columns: x => new { x.UserId, x.DebtAgreementId },
                        principalTable: "DebtAgreements",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObligationSettlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    ObligationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosSettlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SettledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SettlementDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationSettlements", x => x.Id);
                    table.UniqueConstraint("AK_ObligationSettlements_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_ObligationSettlements_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_ObligationSettlements_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_ObligationSettlements_Direction", "[Direction] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_ObligationSettlements_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObligationSettlements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObligationSettlements_Obligations_UserId_ObligationId",
                        columns: x => new { x.UserId, x.ObligationId },
                        principalTable: "Obligations",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObligationSettlements_PosSettlements_UserId_PosSettlementId",
                        columns: x => new { x.UserId, x.PosSettlementId },
                        principalTable: "PosSettlements",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebtAgreements_UserId_CategoryId",
                table: "DebtAgreements",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_DebtAgreements_UserId_CounterpartyId",
                table: "DebtAgreements",
                columns: new[] { "UserId", "CounterpartyId" });

            migrationBuilder.CreateIndex(
                name: "IX_DebtAgreements_UserId_Direction",
                table: "DebtAgreements",
                columns: new[] { "UserId", "Direction" });

            migrationBuilder.CreateIndex(
                name: "IX_DebtAgreements_UserId_OpeningAccountId",
                table: "DebtAgreements",
                columns: new[] { "UserId", "OpeningAccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_DebtInstallments_UserId_DebtAgreementId_Sequence",
                table: "DebtInstallments",
                columns: new[] { "UserId", "DebtAgreementId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebtInstallments_UserId_DueDate",
                table: "DebtInstallments",
                columns: new[] { "UserId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DebtInstallments_UserId_PaymentAccountId",
                table: "DebtInstallments",
                columns: new[] { "UserId", "PaymentAccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_Obligations_UserId_Cancelled_DueDate_Direction",
                table: "Obligations",
                columns: new[] { "UserId", "IsCancelled", "DueDate", "Direction" });

            migrationBuilder.CreateIndex(
                name: "IX_Obligations_UserId_CategoryId_IssueDate",
                table: "Obligations",
                columns: new[] { "UserId", "CategoryId", "IssueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Obligations_UserId_CounterpartyId_Cancelled_DueDate",
                table: "Obligations",
                columns: new[] { "UserId", "CounterpartyId", "IsCancelled", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ObligationSettlements_UserId_AccountId_Date",
                table: "ObligationSettlements",
                columns: new[] { "UserId", "AccountId", "SettlementDate" });

            migrationBuilder.CreateIndex(
                name: "UX_ObligationSettlements_UserId_ObligationId",
                table: "ObligationSettlements",
                columns: new[] { "UserId", "ObligationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ObligationSettlements_UserId_PosSettlementId",
                table: "ObligationSettlements",
                columns: new[] { "UserId", "PosSettlementId" },
                unique: true,
                filter: "[PosSettlementId] IS NOT NULL");

            // Ters kopya: dalga 1'in verisi eski dört tabloda kayıpsız temsil edilir.
            migrationBuilder.Sql(
                """
                INSERT INTO [DebtAgreements]
                    ([Id], [AnnualInterestRate], [CategoryId], [CounterpartyId], [CreatedAtUtc],
                     [Description], [Direction], [FirstDueDate], [InstallmentCount], [OpeningAccountId],
                     [Scope], [SourceType], [StartDate], [UserId], [Principal], [Currency],
                     [TotalRepayment], [TotalCurrency])
                SELECT [Id], [AnnualInterestRate], [CategoryId], [CounterpartyId], [CreatedAtUtc],
                       [Description], [Direction], [FirstDueDate], [InstallmentCount], [OpeningAccountId],
                       [Scope], [SourceType], [StartDate], [UserId], [Principal], [Currency],
                       [TotalRepayment], [TotalCurrency]
                FROM [Debts]
                WHERE [PlanKind] = 2;

                INSERT INTO [Obligations]
                    ([Id], [CancelledAtUtc], [CategoryId], [CounterpartyId], [CreatedAtUtc],
                     [Description], [Direction], [DueDate], [IsCancelled], [IssueDate], [Scope],
                     [UserId], [Amount], [Currency])
                SELECT [Id], [CancelledAtUtc], [CategoryId], [CounterpartyId], [CreatedAtUtc],
                       [Description], [Direction], [FirstDueDate], [IsCancelled], [StartDate], [Scope],
                       [UserId], [Principal], 1
                FROM [Debts]
                WHERE [PlanKind] = 1;

                INSERT INTO [DebtInstallments]
                    ([Id], [DebtAgreementId], [DueDate], [InterestPortion], [PaidAtUtc],
                     [PaymentAccountId], [PaymentDate], [PrincipalPortion], [Sequence], [UserId],
                     [Amount], [Currency])
                SELECT item.[Id], item.[DebtId], item.[DueDate], item.[InterestPortion], payment.[PaidAtUtc],
                       payment.[AccountId], payment.[PaymentDate], item.[PrincipalPortion], item.[Sequence],
                       item.[UserId], item.[Amount], item.[Currency]
                FROM [DebtScheduleItems] AS item
                INNER JOIN [Debts] AS debt
                    ON debt.[UserId] = item.[UserId] AND debt.[Id] = item.[DebtId]
                LEFT JOIN [DebtPaymentAllocations] AS allocation
                    ON allocation.[UserId] = item.[UserId] AND allocation.[ScheduleItemId] = item.[Id]
                LEFT JOIN [DebtPayments] AS payment
                    ON payment.[UserId] = allocation.[UserId] AND payment.[Id] = allocation.[PaymentId]
                   AND payment.[IsCancelled] = 0
                WHERE debt.[PlanKind] = 2;

                INSERT INTO [ObligationSettlements]
                    ([Id], [AccountId], [CancelledAtUtc], [Direction], [IsCancelled], [ObligationId],
                     [PosSettlementId], [SettledAtUtc], [SettlementDate], [UserId], [Amount], [Currency])
                SELECT payment.[Id], payment.[AccountId], payment.[CancelledAtUtc], payment.[Direction],
                       payment.[IsCancelled], payment.[DebtId], payment.[PosSettlementId], payment.[PaidAtUtc],
                       payment.[PaymentDate], payment.[UserId], payment.[Amount], 1
                FROM [DebtPayments] AS payment
                INNER JOIN [Debts] AS debt
                    ON debt.[UserId] = payment.[UserId] AND debt.[Id] = payment.[DebtId]
                WHERE debt.[PlanKind] = 1;
                """);

            migrationBuilder.DropTable(
                name: "DebtPaymentAllocations");

            migrationBuilder.DropTable(
                name: "DebtPayments");

            migrationBuilder.DropTable(
                name: "DebtScheduleItems");

            migrationBuilder.DropTable(
                name: "Debts");
        }
    }
}
