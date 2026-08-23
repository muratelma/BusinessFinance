namespace BusinessFinance.Application.Counterparties;

/// <summary>
/// Bir karşı tarafın kimliği ve o anki cari bakiyesi.
/// </summary>
/// <remarks>
/// Bakiye kalıcı kolon değildir (ADR 0014): iki tutar da hareketlerden
/// hesaplanır. Kayıt burada birleşiyor ki liste ekranı adı ve bakiyeyi tek
/// sorgudan alsın; ad için ikinci bir okuma karşı taraf başına bir sorgu
/// demek olurdu.
/// </remarks>
public sealed record CounterpartyBalanceSummary(
    Guid CounterpartyId,
    string Name,
    bool IsActive,
    decimal Receivable,
    decimal Payable)
{
    /// <summary>Artı: karşı taraf bize borçlu. Eksi: biz ona borçluyuz.</summary>
    public decimal Net => Receivable - Payable;

    /// <summary>
    /// Kapanmış cari: iki tarafta da açık tutar kalmamış. Fazla tahsilat
    /// kırpılmadığı için eksi bakiye <b>kapanmış sayılmaz</b>; hâlâ konuşulacak
    /// bir para vardır.
    /// </summary>
    public bool IsSettled => Receivable == 0m && Payable == 0m;
}
