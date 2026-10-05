using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Pos;

/// <summary>
/// Kartla tahsil edilen paranın kimden geldiği: tahsil kaydı ile onu doğuran
/// tahsilatın (cari ya da tek seferlik alacak) kişisi.
/// </summary>
/// <remarks>
/// Bağ tahsilattadır (<c>PosSettlementId</c>), POS kaydında değil; POS listesi,
/// yatış ayrıntısı ve gün sonu listesi adı bu tek okumadan alır. Karşı tarafı
/// olmayan tek seferlik alacakta alacağın açıklaması yazılır, o da yoksa boştur.
/// </remarks>
internal static class CardCollectionPayers
{
    public static IQueryable<CardCollectionPayer> Query(
        BusinessFinanceDbContext dbContext,
        Guid userId) =>
        (from payment in dbContext.CounterpartyPayments
         join counterparty in dbContext.Counterparties
             on new { payment.UserId, Id = payment.CounterpartyId }
             equals new { counterparty.UserId, counterparty.Id }
         where payment.UserId == userId && payment.PosSettlementId != null
         select new CardCollectionPayer
         {
             SettlementId = payment.PosSettlementId!.Value,
             Name = counterparty.Name
         })
        .Concat(
            from settlement in dbContext.ObligationSettlements
            join obligation in dbContext.Obligations
                on new { settlement.UserId, Id = settlement.ObligationId }
                equals new { obligation.UserId, obligation.Id }
            join counterparty in dbContext.Counterparties
                on new { obligation.UserId, Id = obligation.CounterpartyId }
                equals new { counterparty.UserId, Id = (Guid?)counterparty.Id }
                into counterparties
            from counterparty in counterparties.DefaultIfEmpty()
            where settlement.UserId == userId && settlement.PosSettlementId != null
            select new CardCollectionPayer
            {
                SettlementId = settlement.PosSettlementId!.Value,
                Name = counterparty == null ? obligation.Description : counterparty.Name
            });
}

internal sealed class CardCollectionPayer
{
    public Guid SettlementId { get; init; }
    public string? Name { get; init; }
}
