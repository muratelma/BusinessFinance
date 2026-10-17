using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Debts;

/// <summary>
/// Tek borç modelinin (ADR 0021) okumaları.
/// </summary>
/// <remarks>
/// <para>
/// Borç kendi kapanışlarıyla kapanır: bir kalemi birden çok ödeme kapatabilir,
/// ödeme hesaptan ya da kredi kartından olabilir, indirim para taşımaz ve yerine
/// yeni plan gelmiş kalem kalana girmez. Bu yüzden okumalar "taksit başına tek
/// ödeme" varsaymaz; iki görünümden okur:
/// <see cref="Payments"/> (her kapanış tek satır) ve <see cref="OpenItems"/>
/// (geçerli plandaki her kalemin açık tutarı).
/// </para>
/// <para>
/// Ekranlar ve akış türleri dalga 2'nin Flutter adımına kadar değişmez: tek
/// ödemeli borç hâlâ <c>obligation</c>, ödemesi <c>obligation-settlement</c>,
/// öbür planlar <c>debt-opening</c>, <c>debt-payment</c>, <c>debt-collection</c>
/// olarak görünür. <see cref="Obligations"/> ve <see cref="ObligationSettlements"/>
/// o eski biçimi verir.
/// </para>
/// </remarks>
internal static class DebtViews
{
    /// <summary>
    /// Tek ödemeli borçlar, eski yükümlülük biçiminde. Uygulamadan önce devreden
    /// borç da buradadır ama kategorisi yoktur ve gelir ya da gider tanımaz
    /// (ADR 0021 İ5): tanıma okumaları onu <see cref="ObligationRow.IsCarriedOver"/>
    /// ile dışlar, kalan ve vade okumaları (net varlık, planlananlar) sayar.
    /// </summary>
    public static IQueryable<ObligationRow> Obligations(BusinessFinanceDbContext dbContext) =>
        dbContext.Debts.AsNoTracking()
            .Where(debt => debt.PlanKind == DebtPlanKind.Single)
            .Select(debt => new ObligationRow
            {
                Id = debt.Id,
                UserId = debt.UserId,
                CounterpartyId = debt.CounterpartyId,
                CategoryId = debt.CategoryId,
                IsCarriedOver = debt.IsCarriedOver,
                Direction = debt.Direction,
                Amount = debt.Principal.Amount,
                Currency = debt.Principal.Currency,
                Scope = debt.Scope,
                IssueDate = debt.StartDate,
                DueDate = debt.FirstDueDate!.Value,
                Description = debt.Description,
                CreatedAtUtc = debt.CreatedAtUtc,
                IsCancelled = debt.IsCancelled,
                CancelledAtUtc = debt.CancelledAtUtc,
                RecognizedAmount = debt.Principal.Amount -
                    (dbContext.DebtPayments
                        .Where(closing => closing.UserId == debt.UserId && closing.DebtId == debt.Id &&
                                          !closing.IsCancelled && closing.Kind == DebtPaymentKind.Discount)
                        .Sum(closing => (decimal?)closing.Amount.Amount) ?? 0m),
                RemainingAmount =
                    (dbContext.DebtScheduleItems
                        .Where(item => item.UserId == debt.UserId && item.DebtId == debt.Id && !item.IsSuperseded)
                        .Sum(item => (decimal?)item.Amount.Amount) ?? 0m) -
                    (dbContext.DebtPaymentAllocations
                        .Where(allocation => allocation.UserId == debt.UserId &&
                                             dbContext.DebtScheduleItems.Any(item =>
                                                 item.UserId == debt.UserId && item.Id == allocation.ScheduleItemId &&
                                                 item.DebtId == debt.Id && !item.IsSuperseded) &&
                                             dbContext.DebtPayments.Any(payment =>
                                                 payment.UserId == debt.UserId && payment.Id == allocation.PaymentId &&
                                                 !payment.IsCancelled))
                        .Sum(allocation => (decimal?)allocation.Amount) ?? 0m),
            });

    /// <summary>
    /// Tek ödemeli borçların hesaptan yapılmış ödemeleri, eski kapanış biçiminde.
    /// Kartla ödeme ve indirim burada değildir: hesabı değiştirmezler.
    /// </summary>
    public static IQueryable<ObligationSettlementRow> ObligationSettlements(BusinessFinanceDbContext dbContext) =>
        from payment in dbContext.DebtPayments.AsNoTracking()
        join debt in dbContext.Debts.AsNoTracking()
            on new { payment.UserId, Id = payment.DebtId } equals new { debt.UserId, debt.Id }
        where debt.PlanKind == DebtPlanKind.Single &&
              payment.Kind == DebtPaymentKind.Payment &&
              payment.AccountId != null
        select new ObligationSettlementRow
        {
            Id = payment.Id,
            UserId = payment.UserId,
            ObligationId = payment.DebtId,
            CounterpartyId = debt.CounterpartyId,
            AccountId = payment.AccountId!.Value,
            Direction = payment.Direction,
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency,
            SettlementDate = payment.PaymentDate,
            SettledAtUtc = payment.PaidAtUtc,
            PosSettlementId = payment.PosSettlementId,
            IsCancelled = payment.IsCancelled,
            CancelledAtUtc = payment.CancelledAtUtc,
        };

    /// <summary>
    /// Tek ödemeli olmayan borçlar (taksitli ve planı olmayan). İptal edilmiş
    /// borç bu okumalarda yoktur.
    /// </summary>
    public static IQueryable<Debt> Agreements(BusinessFinanceDbContext dbContext) =>
        dbContext.Debts.AsNoTracking()
            .Where(debt => debt.PlanKind != DebtPlanKind.Single && !debt.IsCancelled);

    /// <summary>
    /// Borçların iptal edilmemiş kapanışları, her biri tek satır; içindeki faiz
    /// payıyla. Faizli borç tam taksitle ödendiği için faiz payı kalemin payıdır.
    /// </summary>
    public static IQueryable<DebtPaymentRow> Payments(BusinessFinanceDbContext dbContext) =>
        from payment in dbContext.DebtPayments.AsNoTracking()
        join debt in dbContext.Debts.AsNoTracking()
            on new { payment.UserId, Id = payment.DebtId } equals new { debt.UserId, debt.Id }
        where !payment.IsCancelled
        select new DebtPaymentRow
        {
            Id = payment.Id,
            UserId = payment.UserId,
            DebtId = payment.DebtId,
            PlanKind = debt.PlanKind,
            Kind = payment.Kind,
            Direction = payment.Direction,
            Scope = debt.Scope,
            CounterpartyId = debt.CounterpartyId,
            AccountId = payment.AccountId,
            CreditCardId = payment.CreditCardId,
            PosSettlementId = payment.PosSettlementId,
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency,
            PaymentDate = payment.PaymentDate,
            PaidAtUtc = payment.PaidAtUtc,
            InterestAmount =
                (from allocation in dbContext.DebtPaymentAllocations
                 join item in dbContext.DebtScheduleItems
                     on new { allocation.UserId, Id = allocation.ScheduleItemId }
                     equals new { item.UserId, item.Id }
                 where allocation.UserId == payment.UserId && allocation.PaymentId == payment.Id &&
                       item.InterestPortion != null
                 select allocation.Amount == item.Amount.Amount
                     ? item.InterestPortion!.Value
                     : item.InterestPortion!.Value * allocation.Amount / item.Amount.Amount)
                .Sum(interest => (decimal?)interest),
        };

    /// <summary>
    /// İptal edilmemiş borçların geçerli plandaki kalemleri, açık tutarıyla. Açık
    /// anapara, kalemin anapara payının açık kısma düşen bölümüdür; ayrımı
    /// olmayan eski kalemde tamamı anaparadır.
    /// </summary>
    public static IQueryable<DebtOpenItemRow> OpenItems(BusinessFinanceDbContext dbContext) =>
        from row in
            from item in dbContext.DebtScheduleItems.AsNoTracking()
            join debt in dbContext.Debts.AsNoTracking()
                on new { item.UserId, Id = item.DebtId } equals new { debt.UserId, debt.Id }
            where !item.IsSuperseded && !debt.IsCancelled
            select new
            {
                Item = item,
                Debt = debt,
                Paid = (from allocation in dbContext.DebtPaymentAllocations
                        join payment in dbContext.DebtPayments
                            on new { allocation.UserId, Id = allocation.PaymentId }
                            equals new { payment.UserId, payment.Id }
                        where allocation.UserId == item.UserId && allocation.ScheduleItemId == item.Id &&
                              !payment.IsCancelled
                        select allocation.Amount)
                    .Sum(amount => (decimal?)amount) ?? 0m,
            }
        where row.Item.Amount.Amount - row.Paid > 0m
        select new DebtOpenItemRow
        {
            Id = row.Item.Id,
            UserId = row.Item.UserId,
            DebtId = row.Item.DebtId,
            PlanKind = row.Debt.PlanKind,
            Direction = row.Debt.Direction,
            Scope = row.Debt.Scope,
            CounterpartyId = row.Debt.CounterpartyId,
            Description = row.Debt.Description,
            Sequence = row.Item.Sequence,
            DueDate = row.Item.DueDate,
            Amount = row.Item.Amount.Amount,
            Currency = row.Item.Amount.Currency,
            OpenAmount = row.Item.Amount.Amount - row.Paid,
            OpenPrincipal = row.Item.PrincipalPortion == null
                ? row.Item.Amount.Amount - row.Paid
                : row.Paid == 0m
                    ? row.Item.PrincipalPortion.Value
                    : row.Item.PrincipalPortion.Value * (row.Item.Amount.Amount - row.Paid) / row.Item.Amount.Amount,
        };
}

internal sealed class ObligationRow
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid? CounterpartyId { get; init; }
    /// <summary>Tanınan gelir ya da giderin kategorisi; devredilmiş borçta boş.</summary>
    public Guid? CategoryId { get; init; }

    /// <summary>Uygulamadan önceki borç: tanınmaz, yalnız kalanı ve vadesi sayılır.</summary>
    public bool IsCarriedOver { get; init; }
    public DebtDirection Direction { get; init; }

    /// <summary>Faturanın ilk bedeli.</summary>
    public decimal Amount { get; init; }

    public CurrencyCode Currency { get; init; }
    public TransactionScope Scope { get; init; }
    public DateOnly IssueDate { get; init; }
    public DateOnly DueDate { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset? CreatedAtUtc { get; init; }
    public bool IsCancelled { get; init; }
    public DateTimeOffset? CancelledAtUtc { get; init; }

    /// <summary>Tanınan gelir ya da gider: bedel, indirimler düşülmüş (fatura gününde).</summary>
    public decimal RecognizedAmount { get; init; }

    /// <summary>Henüz kapanmamış tutar; sıfırsa fatura kapanmıştır.</summary>
    public decimal RemainingAmount { get; init; }
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

internal sealed class DebtPaymentRow
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid DebtId { get; init; }
    public DebtPlanKind PlanKind { get; init; }
    public DebtPaymentKind Kind { get; init; }
    public DebtDirection Direction { get; init; }
    public TransactionScope Scope { get; init; }
    public Guid? CounterpartyId { get; init; }
    public Guid? AccountId { get; init; }
    public Guid? CreditCardId { get; init; }
    public Guid? PosSettlementId { get; init; }
    public decimal Amount { get; init; }
    public CurrencyCode Currency { get; init; }
    public DateOnly PaymentDate { get; init; }
    public DateTimeOffset PaidAtUtc { get; init; }

    /// <summary>
    /// Ödemenin içindeki faiz: ödendiği gün gider (alacakta gelir) olur. Anapara
    /// ve faiz ayrımı olmayan eski kalemde boştur, faizsiz kalemde sıfırdır.
    /// </summary>
    public decimal? InterestAmount { get; init; }
}

internal sealed class DebtOpenItemRow
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid DebtId { get; init; }
    public DebtPlanKind PlanKind { get; init; }
    public DebtDirection Direction { get; init; }
    public TransactionScope Scope { get; init; }
    public Guid? CounterpartyId { get; init; }
    public string? Description { get; init; }
    public int Sequence { get; init; }
    public DateOnly? DueDate { get; init; }
    public decimal Amount { get; init; }
    public CurrencyCode Currency { get; init; }
    public decimal OpenAmount { get; init; }
    public decimal OpenPrincipal { get; init; }
}
