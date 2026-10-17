using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BusinessFinance.Infrastructure.Identity;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Persistence;

public sealed class BusinessFinanceDbContext(
    DbContextOptions<BusinessFinanceDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public const string ConnectionStringName = "BusinessFinance";

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BudgetTransaction> Transactions => Set<BudgetTransaction>();
    public DbSet<MonthlyBudget> MonthlyBudgets => Set<MonthlyBudget>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<CreditCardCharge> CreditCardCharges => Set<CreditCardCharge>();
    public DbSet<CreditCardPayment> CreditCardPayments => Set<CreditCardPayment>();
    public DbSet<InstallmentPlan> InstallmentPlans => Set<InstallmentPlan>();
    public DbSet<InstallmentItem> InstallmentItems => Set<InstallmentItem>();
    public DbSet<RecurringTransaction> RecurringTransactions => Set<RecurringTransaction>();
    public DbSet<RecurringTransactionOccurrence> RecurringTransactionOccurrences =>
        Set<RecurringTransactionOccurrence>();
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();
    public DbSet<ImportRow> ImportRows => Set<ImportRow>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<DebtScheduleItem> DebtScheduleItems => Set<DebtScheduleItem>();
    public DbSet<DebtPayment> DebtPayments => Set<DebtPayment>();
    public DbSet<DebtPaymentAllocation> DebtPaymentAllocations => Set<DebtPaymentAllocation>();

    // Dalga 1 uyum görünümleri (ADR 0021 T10): yükümlülük ile borç planı tek
    // tabloda yaşar, ama ekranlar ve akış türleri dalga 1'de değişmediği için
    // okumalar iki eski biçimi görmeye devam eder. Salt okunurdur; yazma
    // `Debts` üzerinden yapılır. Dalga 2'de okumalar tek modele geçince kalkar.
    internal IQueryable<Infrastructure.Debts.ObligationRow> Obligations => Infrastructure.Debts.DebtViews.Obligations(this);
    internal IQueryable<Infrastructure.Debts.ObligationSettlementRow> ObligationSettlements =>
        Infrastructure.Debts.DebtViews.ObligationSettlements(this);
    internal IQueryable<Debt> DebtAgreements => Infrastructure.Debts.DebtViews.Agreements(this);
    internal IQueryable<Infrastructure.Debts.DebtPaymentRow> DebtPaymentRows => Infrastructure.Debts.DebtViews.Payments(this);
    internal IQueryable<Infrastructure.Debts.DebtOpenItemRow> DebtOpenItems => Infrastructure.Debts.DebtViews.OpenItems(this);
    public DbSet<Counterparty> Counterparties => Set<Counterparty>();
    public DbSet<CounterpartyCharge> CounterpartyCharges => Set<CounterpartyCharge>();
    public DbSet<CounterpartyChargeDiscount> CounterpartyChargeDiscounts => Set<CounterpartyChargeDiscount>();
    public DbSet<CounterpartyPayment> CounterpartyPayments => Set<CounterpartyPayment>();
    public DbSet<CashCount> CashCounts => Set<CashCount>();
    public DbSet<PosSettlement> PosSettlements => Set<PosSettlement>();
    public DbSet<PosDefinition> PosDefinitions => Set<PosDefinition>();
    public DbSet<PosDeposit> PosDeposits => Set<PosDeposit>();
    public DbSet<DayClose> DayCloses => Set<DayClose>();
    public DbSet<DayCloseCountedRecord> DayCloseCountedRecords => Set<DayCloseCountedRecord>();
    public DbSet<DayCloseCountedOverlap> DayCloseCountedOverlaps => Set<DayCloseCountedOverlap>();
    public DbSet<SavingsGoal> SavingsGoals => Set<SavingsGoal>();
    public DbSet<SavingsGoalContribution> SavingsGoalContributions => Set<SavingsGoalContribution>();
    public DbSet<FinancialAttachment> FinancialAttachments => Set<FinancialAttachment>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public DbSet<VerificationCode> VerificationCodes => Set<VerificationCode>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(BusinessFinanceDbContext).Assembly);
        EntryTimestamp.Configure(builder);
    }

    /// <summary>
    /// Yeni kayıtlara giriş anının yazılıp yazılmayacağı. Yalnız yedekten geri
    /// yükleme kapatır: geri yüklenen kayıt dosyadaki anı taşır, dosyada
    /// yoksa boş kalır — geri yükleme anı kaydın girildiği an değildir.
    /// </summary>
    internal bool StampsEntryTime { get; set; } = true;

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        if (StampsEntryTime) EntryTimestamp.Stamp(this);
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        if (StampsEntryTime) EntryTimestamp.Stamp(this);
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
