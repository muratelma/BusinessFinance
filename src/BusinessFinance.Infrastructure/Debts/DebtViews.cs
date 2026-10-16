using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Debts;

/// <summary>
/// Tek borç modelinin (ADR 0021) eski iki modelin biçiminde okunması.
/// </summary>
/// <remarks>
/// <para>
/// Dalga 1'de ekranlar ve akış türleri değişmez: tek ödemeli borç hâlâ
/// <c>obligation</c>, kapanışı <c>obligation-settlement</c>, taksitli borcun
/// açılışı <c>debt-opening</c>, taksit ödemesi <c>debt-payment</c> olarak
/// görünür. Okumalar bu görünümlerden okur; tabloların kendisi birdir.
/// </para>
/// <para>
/// Taksit ödemesinin kimliği taksidinkidir ve dalga 1'de bir taksit tek ödemeyle
/// kapanır (tahsisin tekliği). Bu yüzden taksit görünümü her taksit için tek
/// satır verir; iptal edilmiş ödeme ödenmemiş sayılır.
/// </para>
/// </remarks>
internal static class DebtViews
{
    /// <summary>Tek ödemeli borçlar, eski yükümlülük biçiminde.</summary>
    public static IQueryable<ObligationRow> Obligations(BusinessFinanceDbContext dbContext) =>
        dbContext.Debts.AsNoTracking()
            .Where(debt => debt.PlanKind == DebtPlanKind.Single)
            .Select(debt => new ObligationRow
            {
                Id = debt.Id,
                UserId = debt.UserId,
                CounterpartyId = debt.CounterpartyId,
                CategoryId = debt.CategoryId!.Value,
                Direction = debt.Direction,
                Amount = debt.Principal.Amount,
                Currency = debt.Principal.Currency,
                Scope = debt.Scope,
                IssueDate = debt.StartDate,
                DueDate = debt.FirstDueDate,
                Description = debt.Description,
                CreatedAtUtc = debt.CreatedAtUtc,
                IsCancelled = debt.IsCancelled,
                CancelledAtUtc = debt.CancelledAtUtc,
            });

    /// <summary>Tek ödemeli borçların kapanışları, eski kapanış biçiminde.</summary>
    public static IQueryable<ObligationSettlementRow> ObligationSettlements(BusinessFinanceDbContext dbContext) =>
        from payment in dbContext.DebtPayments.AsNoTracking()
        join debt in dbContext.Debts.AsNoTracking()
            on new { payment.UserId, Id = payment.DebtId } equals new { debt.UserId, debt.Id }
        where debt.PlanKind == DebtPlanKind.Single
        select new ObligationSettlementRow
        {
            Id = payment.Id,
            UserId = payment.UserId,
            ObligationId = payment.DebtId,
            CounterpartyId = debt.CounterpartyId,
            AccountId = payment.AccountId,
            Direction = payment.Direction,
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency,
            SettlementDate = payment.PaymentDate,
            SettledAtUtc = payment.PaidAtUtc,
            PosSettlementId = payment.PosSettlementId,
            IsCancelled = payment.IsCancelled,
            CancelledAtUtc = payment.CancelledAtUtc,
        };

    /// <summary>Taksitli borçlar (eski borç planları).</summary>
    public static IQueryable<Debt> Agreements(BusinessFinanceDbContext dbContext) =>
        dbContext.Debts.AsNoTracking().Where(debt => debt.PlanKind == DebtPlanKind.Installments);

    /// <summary>Taksitli borçların taksitleri, ödemeleriyle birlikte (eski taksit biçimi).</summary>
    public static IQueryable<InstallmentRow> Installments(BusinessFinanceDbContext dbContext)
    {
        // Önce kalemi kapatan iptal edilmemiş ödemeler, sonra kaleme tek bir sol
        // birleşim: boş tahsisin alanı hiç okunmaz ve her kalem tek satır verir.
        var paid =
            from allocation in dbContext.DebtPaymentAllocations.AsNoTracking()
            join payment in dbContext.DebtPayments.AsNoTracking()
                on new { allocation.UserId, Id = allocation.PaymentId }
                equals new { payment.UserId, payment.Id }
            where !payment.IsCancelled
            // Alanlar boş olabilir türde: sol birleşimin eşsiz satırında
            // okunurlar (bellek içi sağlayıcı aksi hâlde düşer).
            select new
            {
                allocation.UserId,
                allocation.ScheduleItemId,
                AccountId = (Guid?)payment.AccountId,
                PaymentDate = (DateOnly?)payment.PaymentDate,
                PaidAtUtc = (DateTimeOffset?)payment.PaidAtUtc,
            };

        return
            from item in dbContext.DebtScheduleItems.AsNoTracking()
            join debt in dbContext.Debts.AsNoTracking()
                on new { item.UserId, Id = item.DebtId } equals new { debt.UserId, debt.Id }
            join payment in paid
                on new { item.UserId, ItemId = item.Id }
                equals new { payment.UserId, ItemId = payment.ScheduleItemId }
                into payments
            from payment in payments.DefaultIfEmpty()
            where debt.PlanKind == DebtPlanKind.Installments
            select new InstallmentRow
            {
                Id = item.Id,
                UserId = item.UserId,
                DebtAgreementId = item.DebtId,
                Sequence = item.Sequence,
                Amount = item.Amount.Amount,
                Currency = item.Amount.Currency,
                PrincipalPortion = item.PrincipalPortion,
                InterestPortion = item.InterestPortion,
                DueDate = item.DueDate,
                PaymentAccountId = payment == null ? null : payment.AccountId,
                PaymentDate = payment == null ? null : payment.PaymentDate,
                PaidAtUtc = payment == null ? null : payment.PaidAtUtc,
            };
    }
}

internal sealed class ObligationRow
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid? CounterpartyId { get; init; }
    public Guid CategoryId { get; init; }
    public DebtDirection Direction { get; init; }
    public decimal Amount { get; init; }
    public CurrencyCode Currency { get; init; }
    public TransactionScope Scope { get; init; }
    public DateOnly IssueDate { get; init; }
    public DateOnly DueDate { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset? CreatedAtUtc { get; init; }
    public bool IsCancelled { get; init; }
    public DateTimeOffset? CancelledAtUtc { get; init; }
}

internal sealed class ObligationSettlementRow
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid ObligationId { get; init; }

    /// <summary>Borcun karşı tarafı; görünüm borçla zaten birleşir.</summary>
    public Guid? CounterpartyId { get; init; }

    public Guid AccountId { get; init; }
    public DebtDirection Direction { get; init; }
    public decimal Amount { get; init; }
    public CurrencyCode Currency { get; init; }
    public DateOnly SettlementDate { get; init; }
    public DateTimeOffset SettledAtUtc { get; init; }
    public Guid? PosSettlementId { get; init; }
    public bool IsCancelled { get; init; }
    public DateTimeOffset? CancelledAtUtc { get; init; }
}

internal sealed class InstallmentRow
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid DebtAgreementId { get; init; }
    public int Sequence { get; init; }
    public decimal Amount { get; init; }
    public CurrencyCode Currency { get; init; }
    public decimal? PrincipalPortion { get; init; }
    public decimal? InterestPortion { get; init; }
    public DateOnly DueDate { get; init; }
    public Guid? PaymentAccountId { get; init; }
    public DateOnly? PaymentDate { get; init; }
    public DateTimeOffset? PaidAtUtc { get; init; }
}
