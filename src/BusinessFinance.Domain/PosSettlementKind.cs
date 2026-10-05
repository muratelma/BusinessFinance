namespace BusinessFinance.Domain;

/// <summary>
/// POS'tan geçen paranın ne olduğu.
/// </summary>
/// <remarks>
/// İkisi de aynı fiziksel olaydır — kart POS'tan geçer, banka komisyonu keser,
/// neti birkaç gün sonra yatırır — ve aynı yoldan (yatış) hesaba geçer. Ayrıldıkları
/// tek yer <b>tanımadır</b> (ADR 0014, ADR 0019 İ1):
/// <list type="bullet">
/// <item><see cref="Sale"/> bir satıştır ve gelir yazar.</item>
/// <item><see cref="Collection"/> daha önce tanınmış bir alacağın (veresiye,
/// tek seferlik alacak) kartla tahsilidir ve gelir <b>yazmaz</b>; yazsaydı aynı
/// satış iki kez gelir sayılırdı (İ2).</item>
/// </list>
/// Komisyon ikisinde de tahsil günü yazılan bir giderdir.
/// </remarks>
public enum PosSettlementKind : byte
{
    Sale = 1,
    Collection = 2
}
