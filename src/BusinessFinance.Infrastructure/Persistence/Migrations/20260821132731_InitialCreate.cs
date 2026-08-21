using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.UniqueConstraint("AK_Accounts_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_Accounts_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_Accounts_OpeningBalance", "[OpeningBalance] >= 0");
                    table.CheckConstraint("CK_Accounts_Type", "[Type] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_Accounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.UniqueConstraint("AK_Categories_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_Categories_Type", "[Type] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_Categories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Limit = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    StatementClosingDay = table.Column<byte>(type: "tinyint", nullable: false),
                    PaymentDueDay = table.Column<byte>(type: "tinyint", nullable: false),
                    MinimumPaymentRate = table.Column<decimal>(type: "decimal(7,4)", precision: 7, scale: 4, nullable: false, defaultValue: 20m),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.Id);
                    table.UniqueConstraint("AK_CreditCards_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_CreditCards_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_CreditCards_Limit", "[Limit] > 0");
                    table.CheckConstraint("CK_CreditCards_MinimumPaymentRate", "[MinimumPaymentRate] BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_CreditCards_PaymentDueDay", "[PaymentDueDay] BETWEEN 1 AND 28");
                    table.CheckConstraint("CK_CreditCards_StatementClosingDay", "[StatementClosingDay] BETWEEN 1 AND 28");
                    table.ForeignKey(
                        name: "FK_CreditCards_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileFingerprint = table.Column<string>(type: "nchar(64)", fixedLength: true, maxLength: 64, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    EncodingName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Delimiter = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: false),
                    DateColumn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AmountColumn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DescriptionColumn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceColumn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DateFormat = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DecimalSeparator = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportBatches", x => x.Id);
                    table.UniqueConstraint("AK_ImportBatches_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_ImportBatches_DecimalSeparator", "[DecimalSeparator] IN ('.', ',')");
                    table.CheckConstraint("CK_ImportBatches_Delimiter", "[Delimiter] IN (',', ';', CHAR(9))");
                    table.CheckConstraint("CK_ImportBatches_FileSize", "[FileSizeBytes] > 0 AND [FileSizeBytes] <= 2097152");
                    table.CheckConstraint("CK_ImportBatches_Status", "[Status] IN (1, 2, 3)");
                    table.ForeignKey(
                        name: "FK_ImportBatches_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", precision: 0, nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", precision: 0, nullable: false),
                    RevokedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", precision: 0, nullable: true),
                    ReplacedBySessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReuseDetectedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshSessions", x => x.Id);
                    table.CheckConstraint("CK_RefreshSessions_Expiry", "[ExpiresAtUtc] > [CreatedAtUtc]");
                    table.CheckConstraint("CK_RefreshSessions_ReuseDetection", "[ReuseDetectedAtUtc] IS NULL OR [ReuseDetectedAtUtc] >= [CreatedAtUtc]");
                    table.CheckConstraint("CK_RefreshSessions_Revocation", "[RevokedAtUtc] IS NULL OR [RevokedAtUtc] >= [CreatedAtUtc]");
                    table.ForeignKey(
                        name: "FK_RefreshSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefreshSessions_RefreshSessions_ReplacedBySessionId",
                        column: x => x.ReplacedBySessionId,
                        principalTable: "RefreshSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavingsGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TrackingMode = table.Column<byte>(type: "tinyint", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsGoals", x => x.Id);
                    table.UniqueConstraint("AK_SavingsGoals_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_SavingsGoals_Currency", "[Currency] = 'TRY'");
                    table.CheckConstraint("CK_SavingsGoals_ProgressSource", "([TrackingMode] = 1 AND [AccountId] IS NOT NULL) OR ([TrackingMode] = 2 AND [AccountId] IS NULL)");
                    table.CheckConstraint("CK_SavingsGoals_TargetAmount", "[TargetAmount] > 0");
                    table.CheckConstraint("CK_SavingsGoals_TrackingMode", "[TrackingMode] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_SavingsGoals_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavingsGoals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    TransferDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.CheckConstraint("CK_Transfers_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_Transfers_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_Transfers_DifferentAccounts", "[SourceAccountId] <> [DestinationAccountId]");
                    table.ForeignKey(
                        name: "FK_Transfers_Accounts_UserId_DestinationAccountId",
                        columns: x => new { x.UserId, x.DestinationAccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Accounts_UserId_SourceAccountId",
                        columns: x => new { x.UserId, x.SourceAccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    TransactionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetTransactions", x => x.Id);
                    table.UniqueConstraint("AK_BudgetTransactions_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_BudgetTransactions_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_BudgetTransactions_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_BudgetTransactions_Type", "[Type] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DebtAgreements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CounterpartyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    Principal = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    TotalRepayment = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    TotalCurrency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    AnnualInterestRate = table.Column<decimal>(type: "decimal(7,4)", precision: 7, scale: 4, nullable: false),
                    SourceType = table.Column<byte>(type: "tinyint", nullable: false),
                    OpeningAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FirstDueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InstallmentCount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBudgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Limit = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyBudgets", x => x.Id);
                    table.CheckConstraint("CK_MonthlyBudgets_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_MonthlyBudgets_Limit", "[Limit] > 0");
                    table.CheckConstraint("CK_MonthlyBudgets_Month", "[Month] BETWEEN 1 AND 12");
                    table.CheckConstraint("CK_MonthlyBudgets_Year", "[Year] BETWEEN 2000 AND 2100");
                    table.ForeignKey(
                        name: "FK_MonthlyBudgets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonthlyBudgets_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardCharges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    ChargeDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardCharges", x => x.Id);
                    table.UniqueConstraint("AK_CreditCardCharges_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_CreditCardCharges_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_CreditCardCharges_Currency", "[Currency] = 1");
                    table.ForeignKey(
                        name: "FK_CreditCardCharges_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardCharges_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardCharges_CreditCards_UserId_CreditCardId",
                        columns: x => new { x.UserId, x.CreditCardId },
                        principalTable: "CreditCards",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPayments", x => x.Id);
                    table.CheckConstraint("CK_CreditCardPayments_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_CreditCardPayments_Currency", "[Currency] = 1");
                    table.ForeignKey(
                        name: "FK_CreditCardPayments_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardPayments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardPayments_CreditCards_UserId_CreditCardId",
                        columns: x => new { x.UserId, x.CreditCardId },
                        principalTable: "CreditCards",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstallmentPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    InstallmentCount = table.Column<byte>(type: "tinyint", nullable: false),
                    FirstInstallmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentPlans", x => x.Id);
                    table.UniqueConstraint("AK_InstallmentPlans_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_InstallmentPlans_Count", "[InstallmentCount] BETWEEN 2 AND 60");
                    table.CheckConstraint("CK_InstallmentPlans_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_InstallmentPlans_TotalAmount", "[TotalAmount] > 0");
                    table.ForeignKey(
                        name: "FK_InstallmentPlans_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstallmentPlans_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstallmentPlans_CreditCards_UserId_CreditCardId",
                        columns: x => new { x.UserId, x.CreditCardId },
                        principalTable: "CreditCards",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceType = table.Column<byte>(type: "tinyint", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreditCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    Kind = table.Column<byte>(type: "tinyint", nullable: false),
                    Frequency = table.Column<byte>(type: "tinyint", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NextOccurrenceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MonthEndBehavior = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringTransactions", x => x.Id);
                    table.UniqueConstraint("AK_RecurringTransactions_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_RecurringTransactions_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_RecurringTransactions_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_RecurringTransactions_DateRange", "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
                    table.CheckConstraint("CK_RecurringTransactions_Frequency", "[Frequency] IN (1, 2, 3, 4)");
                    table.CheckConstraint("CK_RecurringTransactions_IncomeSource", "[Kind] <> 1 OR [SourceType] = 1");
                    table.CheckConstraint("CK_RecurringTransactions_Kind", "[Kind] IN (1, 2, 3)");
                    table.CheckConstraint("CK_RecurringTransactions_MonthEndBehavior", "[MonthEndBehavior] IN (1, 2)");
                    table.CheckConstraint("CK_RecurringTransactions_Source", "([SourceType] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR ([SourceType] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL)");
                    table.CheckConstraint("CK_RecurringTransactions_SourceType", "[SourceType] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_RecurringTransactions_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTransactions_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTransactions_CreditCards_UserId_CreditCardId",
                        columns: x => new { x.UserId, x.CreditCardId },
                        principalTable: "CreditCards",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavingsGoalContributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavingsGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    ContributionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ClientRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsGoalContributions", x => x.Id);
                    table.CheckConstraint("CK_SavingsGoalContributions_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_SavingsGoalContributions_Currency", "[Currency] = 'TRY'");
                    table.ForeignKey(
                        name: "FK_SavingsGoalContributions_SavingsGoals_UserId_SavingsGoalId",
                        columns: x => new { x.UserId, x.SavingsGoalId },
                        principalTable: "SavingsGoals",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Sha256 = table.Column<string>(type: "char(64)", unicode: false, fixedLength: true, maxLength: 64, nullable: false),
                    ObjectKey = table.Column<string>(type: "varchar(180)", unicode: false, maxLength: 180, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAttachments", x => x.Id);
                    table.CheckConstraint("CK_FinancialAttachments_ContentType", "[ContentType] IN ('application/pdf', 'image/jpeg', 'image/png')");
                    table.CheckConstraint("CK_FinancialAttachments_Size", "[SizeBytes] BETWEEN 1 AND 5242880");
                    table.ForeignKey(
                        name: "FK_FinancialAttachments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialAttachments_BudgetTransactions_UserId_TransactionId",
                        columns: x => new { x.UserId, x.TransactionId },
                        principalTable: "BudgetTransactions",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    RawData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TransactionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SignedAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BudgetTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DuplicateTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DuplicateReason = table.Column<byte>(type: "tinyint", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportRows", x => x.Id);
                    table.CheckConstraint("CK_ImportRows_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_ImportRows_DuplicateReason", "[DuplicateReason] IS NULL OR [DuplicateReason] IN (1, 2)");
                    table.CheckConstraint("CK_ImportRows_DuplicateReview", "([Status] IN (5, 6) AND [DuplicateTransactionId] IS NOT NULL AND [DuplicateReason] IS NOT NULL) OR ([Status] IN (1, 2) AND [DuplicateTransactionId] IS NULL AND [DuplicateReason] IS NULL) OR ([Status] IN (3, 4) AND (([DuplicateTransactionId] IS NULL AND [DuplicateReason] IS NULL) OR ([DuplicateTransactionId] IS NOT NULL AND [DuplicateReason] IS NOT NULL)))");
                    table.CheckConstraint("CK_ImportRows_RowNumber", "[RowNumber] >= 2");
                    table.CheckConstraint("CK_ImportRows_SignedAmountRange", "[SignedAmount] IS NULL OR ([SignedAmount] >= -999999999999999.9999 AND [SignedAmount] <= 999999999999999.9999)");
                    table.CheckConstraint("CK_ImportRows_Status", "[Status] IN (1, 2, 3, 4, 5, 6)");
                    table.CheckConstraint("CK_ImportRows_Validation", "([Status] = 1 AND [TransactionDate] IS NOT NULL AND [SignedAmount] IS NOT NULL AND [SignedAmount] <> 0 AND [ErrorMessage] IS NULL AND [AccountId] IS NULL AND [CategoryId] IS NULL AND [BudgetTransactionId] IS NULL) OR ([Status] = 2 AND [ErrorMessage] IS NOT NULL AND [AccountId] IS NULL AND [CategoryId] IS NULL AND [BudgetTransactionId] IS NULL) OR ([Status] = 3 AND [TransactionDate] IS NOT NULL AND [SignedAmount] IS NOT NULL AND [SignedAmount] <> 0 AND [ErrorMessage] IS NULL AND [AccountId] IS NOT NULL AND [CategoryId] IS NOT NULL AND [BudgetTransactionId] IS NULL) OR ([Status] = 4 AND [TransactionDate] IS NOT NULL AND [SignedAmount] IS NOT NULL AND [SignedAmount] <> 0 AND [ErrorMessage] IS NULL AND [AccountId] IS NOT NULL AND [CategoryId] IS NOT NULL AND [BudgetTransactionId] IS NOT NULL) OR ([Status] IN (5, 6) AND [TransactionDate] IS NOT NULL AND [SignedAmount] IS NOT NULL AND [SignedAmount] <> 0 AND [ErrorMessage] IS NULL AND [AccountId] IS NOT NULL AND [CategoryId] IS NOT NULL AND [BudgetTransactionId] IS NULL)");
                    table.ForeignKey(
                        name: "FK_ImportRows_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportRows_BudgetTransactions_UserId_BudgetTransactionId",
                        columns: x => new { x.UserId, x.BudgetTransactionId },
                        principalTable: "BudgetTransactions",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportRows_BudgetTransactions_UserId_DuplicateTransactionId",
                        columns: x => new { x.UserId, x.DuplicateTransactionId },
                        principalTable: "BudgetTransactions",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportRows_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportRows_ImportBatches_UserId_ImportBatchId",
                        columns: x => new { x.UserId, x.ImportBatchId },
                        principalTable: "ImportBatches",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DebtInstallments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DebtAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    PrincipalPortion = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    InterestPortion = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PaidAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
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
                name: "InstallmentItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstallmentPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<byte>(type: "tinyint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreditCardChargeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RealizedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentItems", x => x.Id);
                    table.CheckConstraint("CK_InstallmentItems_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_InstallmentItems_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_InstallmentItems_Sequence", "[Sequence] > 0");
                    table.ForeignKey(
                        name: "FK_InstallmentItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstallmentItems_CreditCardCharges_UserId_CreditCardChargeId",
                        columns: x => new { x.UserId, x.CreditCardChargeId },
                        principalTable: "CreditCardCharges",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstallmentItems_InstallmentPlans_UserId_InstallmentPlanId",
                        columns: x => new { x.UserId, x.InstallmentPlanId },
                        principalTable: "InstallmentPlans",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringTransactionOccurrences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecurringTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OccurrenceKey = table.Column<string>(type: "nchar(41)", fixedLength: true, maxLength: 41, nullable: false),
                    SourceType = table.Column<byte>(type: "tinyint", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreditCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<byte>(type: "tinyint", nullable: false),
                    Kind = table.Column<byte>(type: "tinyint", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    BudgetTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreditCardChargeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RealizedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringTransactionOccurrences", x => x.Id);
                    table.UniqueConstraint("AK_RecurringTransactionOccurrences_UserId_Id", x => new { x.UserId, x.Id });
                    table.CheckConstraint("CK_RecurringOccurrences_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_RecurringOccurrences_Currency", "[Currency] = 1");
                    table.CheckConstraint("CK_RecurringOccurrences_Kind", "[Kind] IN (1, 2, 3)");
                    table.CheckConstraint("CK_RecurringOccurrences_Realization", "([Status] = 1 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NULL AND [RealizedAtUtc] IS NULL) OR ([Status] = 2 AND [RealizedAtUtc] IS NOT NULL AND (([SourceType] = 1 AND [BudgetTransactionId] IS NOT NULL AND [CreditCardChargeId] IS NULL) OR ([SourceType] = 2 AND [BudgetTransactionId] IS NULL AND [CreditCardChargeId] IS NOT NULL)))");
                    table.CheckConstraint("CK_RecurringOccurrences_Source", "([SourceType] = 1 AND [AccountId] IS NOT NULL AND [CreditCardId] IS NULL) OR ([SourceType] = 2 AND [AccountId] IS NULL AND [CreditCardId] IS NOT NULL)");
                    table.CheckConstraint("CK_RecurringOccurrences_SourceType", "[SourceType] IN (1, 2)");
                    table.CheckConstraint("CK_RecurringOccurrences_Status", "[Status] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_RecurringTransactionOccurrences_Accounts_UserId_AccountId",
                        columns: x => new { x.UserId, x.AccountId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTransactionOccurrences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTransactionOccurrences_BudgetTransactions_UserId_BudgetTransactionId",
                        columns: x => new { x.UserId, x.BudgetTransactionId },
                        principalTable: "BudgetTransactions",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTransactionOccurrences_Categories_UserId_CategoryId",
                        columns: x => new { x.UserId, x.CategoryId },
                        principalTable: "Categories",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTransactionOccurrences_CreditCardCharges_UserId_CreditCardChargeId",
                        columns: x => new { x.UserId, x.CreditCardChargeId },
                        principalTable: "CreditCardCharges",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTransactionOccurrences_CreditCards_UserId_CreditCardId",
                        columns: x => new { x.UserId, x.CreditCardId },
                        principalTable: "CreditCards",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTransactionOccurrences_RecurringTransactions_UserId_RecurringTransactionId",
                        columns: x => new { x.UserId, x.RecurringTransactionId },
                        principalTable: "RecurringTransactions",
                        principalColumns: new[] { "UserId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId_IsActive_Type_Name",
                table: "Accounts",
                columns: new[] { "UserId", "IsActive", "Type", "Name" });

            migrationBuilder.CreateIndex(
                name: "UX_Accounts_UserId_Name",
                table: "Accounts",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail",
                unique: true,
                filter: "[NormalizedEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_UserId_AccountId_TransactionDate",
                table: "BudgetTransactions",
                columns: new[] { "UserId", "AccountId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_UserId_CategoryId_TransactionDate",
                table: "BudgetTransactions",
                columns: new[] { "UserId", "CategoryId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_UserId_TransactionDate",
                table: "BudgetTransactions",
                columns: new[] { "UserId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId_IsActive_Type_Name",
                table: "Categories",
                columns: new[] { "UserId", "IsActive", "Type", "Name" });

            migrationBuilder.CreateIndex(
                name: "UX_Categories_UserId_Type_Name",
                table: "Categories",
                columns: new[] { "UserId", "Type", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardCharges_UserId_CardId_Date",
                table: "CreditCardCharges",
                columns: new[] { "UserId", "CreditCardId", "ChargeDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardCharges_UserId_CategoryId_Date",
                table: "CreditCardCharges",
                columns: new[] { "UserId", "CategoryId", "ChargeDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayments_UserId_AccountId_Date",
                table: "CreditCardPayments",
                columns: new[] { "UserId", "AccountId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayments_UserId_CardId_Date",
                table: "CreditCardPayments",
                columns: new[] { "UserId", "CreditCardId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_UserId_IsActive_Name",
                table: "CreditCards",
                columns: new[] { "UserId", "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "UX_CreditCards_UserId_Name",
                table: "CreditCards",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebtAgreements_UserId_CategoryId",
                table: "DebtAgreements",
                columns: new[] { "UserId", "CategoryId" });

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
                name: "IX_FinancialAttachments_ObjectKey",
                table: "FinancialAttachments",
                column: "ObjectKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAttachments_UserId_TransactionId_CreatedAtUtc",
                table: "FinancialAttachments",
                columns: new[] { "UserId", "TransactionId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportBatches_UserId_CreatedAtUtc",
                table: "ImportBatches",
                columns: new[] { "UserId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "UX_ImportBatches_UserId_FileFingerprint",
                table: "ImportBatches",
                columns: new[] { "UserId", "FileFingerprint" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportRows_UserId_AccountId",
                table: "ImportRows",
                columns: new[] { "UserId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportRows_UserId_BudgetTransactionId",
                table: "ImportRows",
                columns: new[] { "UserId", "BudgetTransactionId" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportRows_UserId_CategoryId",
                table: "ImportRows",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportRows_UserId_DuplicateTransactionId",
                table: "ImportRows",
                columns: new[] { "UserId", "DuplicateTransactionId" });

            migrationBuilder.CreateIndex(
                name: "UX_ImportRows_UserId_BatchId_RowNumber",
                table: "ImportRows",
                columns: new[] { "UserId", "ImportBatchId", "RowNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentItems_UserId_ScheduledDate",
                table: "InstallmentItems",
                columns: new[] { "UserId", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "UX_InstallmentItems_UserId_ChargeId",
                table: "InstallmentItems",
                columns: new[] { "UserId", "CreditCardChargeId" },
                unique: true,
                filter: "[CreditCardChargeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_InstallmentItems_UserId_PlanId_Sequence",
                table: "InstallmentItems",
                columns: new[] { "UserId", "InstallmentPlanId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentPlans_UserId_CardId_FirstDate",
                table: "InstallmentPlans",
                columns: new[] { "UserId", "CreditCardId", "FirstInstallmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentPlans_UserId_CategoryId",
                table: "InstallmentPlans",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "UX_InstallmentPlans_UserId_ClientRequestId",
                table: "InstallmentPlans",
                columns: new[] { "UserId", "ClientRequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBudgets_UserId_Year_Month",
                table: "MonthlyBudgets",
                columns: new[] { "UserId", "Year", "Month" });

            migrationBuilder.CreateIndex(
                name: "UX_MonthlyBudgets_UserId_CategoryId_Year_Month",
                table: "MonthlyBudgets",
                columns: new[] { "UserId", "CategoryId", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecurringOccurrences_UserId_Status_ScheduledDate",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "Status", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactionOccurrences_UserId_AccountId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactionOccurrences_UserId_CategoryId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactionOccurrences_UserId_CreditCardId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "CreditCardId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactionOccurrences_UserId_RecurringTransactionId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "RecurringTransactionId" });

            migrationBuilder.CreateIndex(
                name: "UX_RecurringOccurrences_UserId_ChargeId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "CreditCardChargeId" },
                unique: true,
                filter: "[CreditCardChargeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_RecurringOccurrences_UserId_OccurrenceKey",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "OccurrenceKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_RecurringOccurrences_UserId_TransactionId",
                table: "RecurringTransactionOccurrences",
                columns: new[] { "UserId", "BudgetTransactionId" },
                unique: true,
                filter: "[BudgetTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_UserId_AccountId",
                table: "RecurringTransactions",
                columns: new[] { "UserId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_UserId_CategoryId",
                table: "RecurringTransactions",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_UserId_CreditCardId",
                table: "RecurringTransactions",
                columns: new[] { "UserId", "CreditCardId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_UserId_IsActive_NextDate",
                table: "RecurringTransactions",
                columns: new[] { "UserId", "IsActive", "NextOccurrenceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_ReplacedBySessionId",
                table: "RefreshSessions",
                column: "ReplacedBySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_UserId_RevokedAtUtc",
                table: "RefreshSessions",
                columns: new[] { "UserId", "RevokedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "UX_RefreshSessions_TokenHash",
                table: "RefreshSessions",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoalContributions_UserId_ContributionDate",
                table: "SavingsGoalContributions",
                columns: new[] { "UserId", "ContributionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoalContributions_UserId_SavingsGoalId_ClientRequestId",
                table: "SavingsGoalContributions",
                columns: new[] { "UserId", "SavingsGoalId", "ClientRequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoals_UserId_AccountId",
                table: "SavingsGoals",
                columns: new[] { "UserId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoals_UserId_TargetDate",
                table: "SavingsGoals",
                columns: new[] { "UserId", "TargetDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_UserId_DestinationAccountId",
                table: "Transfers",
                columns: new[] { "UserId", "DestinationAccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_UserId_SourceAccountId",
                table: "Transfers",
                columns: new[] { "UserId", "SourceAccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_UserId_TransferDate",
                table: "Transfers",
                columns: new[] { "UserId", "TransferDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CreditCardPayments");

            migrationBuilder.DropTable(
                name: "DebtInstallments");

            migrationBuilder.DropTable(
                name: "FinancialAttachments");

            migrationBuilder.DropTable(
                name: "ImportRows");

            migrationBuilder.DropTable(
                name: "InstallmentItems");

            migrationBuilder.DropTable(
                name: "MonthlyBudgets");

            migrationBuilder.DropTable(
                name: "RecurringTransactionOccurrences");

            migrationBuilder.DropTable(
                name: "RefreshSessions");

            migrationBuilder.DropTable(
                name: "SavingsGoalContributions");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DebtAgreements");

            migrationBuilder.DropTable(
                name: "ImportBatches");

            migrationBuilder.DropTable(
                name: "InstallmentPlans");

            migrationBuilder.DropTable(
                name: "BudgetTransactions");

            migrationBuilder.DropTable(
                name: "CreditCardCharges");

            migrationBuilder.DropTable(
                name: "RecurringTransactions");

            migrationBuilder.DropTable(
                name: "SavingsGoals");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "CreditCards");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
