namespace BusinessFinance.Application.Counterparties;

/// <summary>
/// Bir karşı tarafın kimliği ve o anki cari bakiyesi.
/// </summary>
/// <remarks>
/// Bakiye kalıcı kolon değildir (ADR 0014): iki tutar da hareketlerden
/// hesaplanır. Kayıt burada birleşiyor ki liste ekranı adı ve bakiyeyi tek
/// sorgudan alsın; ad için ikinci bir okuma karşı taraf başına bir sorgu
/// demek olurdu.
///
/// Bakiye yalnız cari hareketlerden oluşur. Kişiye bağlı açık yükümlülükler
/// (<see cref="OpenReceivableObligations"/>, <see cref="OpenPayableObligations"/>)
/// <b>bilgidir</b>: bakiyeye, nete ve "kapandı" sorusuna girmez; her biri
/// kendi kapanışıyla kapanır.
/// </remarks>
public sealed record CounterpartyBalanceSummary(
    Guid CounterpartyId,
    string Name,
    bool IsActive,
    decimal Receivable,
    decimal Payable,
    decimal OverdueReceivable,
    decimal OverduePayable,
    decimal OpenReceivableObligations = 0m,
    decimal OpenPayableObligations = 0m)
{
    /// <summary>Artı: karşı taraf bize borçlu. Eksi: biz ona borçluyuz.</summary>
    public decimal Net => Receivable - Payable;

    /// <summary>
    /// Kapanmış cari: iki tarafta da açık tutar kalmamış. Fazla tahsilat
    /// kırpılmadığı için eksi bakiye <b>kapanmış sayılmaz</b>; hâlâ konuşulacak
    /// bir para vardır.
    /// </summary>
    public bool IsSettled => Receivable == 0m && Payable == 0m;

    /// <summary>
    /// Vadesi gelmemiş veya vadesi hiç girilmemiş açık alacak. Fazla tahsilat
    /// burada eksi kalabilir; parayı sıfıra kırpmak gerçek bakiyeyi gizlerdi.
    /// </summary>
    public decimal NotOverdueReceivable => Receivable - OverdueReceivable;

    /// <summary>Vadesi gelmemiş veya vadesiz açık borç.</summary>
    public decimal NotOverduePayable => Payable - OverduePayable;

    /// <summary>Karşı tarafın bize borcu, ekranda yazılacak hâliyle.</summary>
    public decimal OwedToYou => CounterpartyDisplay.OwedToYou(Receivable, Payable);

    /// <summary>Bizim karşı tarafa borcumuz, ekranda yazılacak hâliyle.</summary>
    public decimal OwedByYou => CounterpartyDisplay.OwedByYou(Receivable, Payable);
}

/// <summary>
/// İki tarafın ekranda yazılacak tutarları: ikisi de sıfır ya da artıdır.
/// </summary>
/// <remarks>
/// Fazla tahsilat alacağı eksiye düşürür ve kırpılmaz (o para gerçektir). Ama
/// "size borcu −300" yanlış okunur: fazla alınan 300 lira <b>bizim
/// borcumuzdur</b>. Bu iki tutar eksiye düşen tarafı öbür tarafa taşır;
/// aralarındaki fark her zaman <c>Net</c>'e eşittir. İstemci toplama yapmasın
/// diye sunucuda hesaplanır.
/// </remarks>
public static class CounterpartyDisplay
{
    public static decimal OwedToYou(decimal receivable, decimal payable) =>
        Math.Max(receivable, 0m) + Math.Max(-payable, 0m);

    public static decimal OwedByYou(decimal receivable, decimal payable) =>
        Math.Max(payable, 0m) + Math.Max(-receivable, 0m);
}
