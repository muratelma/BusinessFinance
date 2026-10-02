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
    public DbSet<DebtAgreement> DebtAgreements => Set<DebtAgreement>();
    public DbSet<DebtInstallment> DebtInstallments => Set<DebtInstallment>();
    public DbSet<Counterparty> Counterparties => Set<Counterparty>();
    public DbSet<CounterpartyCharge> CounterpartyCharges => Set<CounterpartyCharge>();
    public DbSet<CounterpartyPayment> CounterpartyPayments => Set<CounterpartyPayment>();
    public DbSet<Obligation> Obligations => Set<Obligation>();
    public DbSet<ObligationSettlement> ObligationSettlements => Set<ObligationSettlement>();
    public DbSet<CashCount> CashCounts => Set<CashCount>();
    public DbSet<PosSettlement> PosSettlements => Set<PosSettlement>();
    public DbSet<PosDefinition> PosDefinitions => Set<PosDefinition>();
    public DbSet<PosDeposit> PosDeposits => Set<PosDeposit>();
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
