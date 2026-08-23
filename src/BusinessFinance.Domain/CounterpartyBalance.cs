namespace BusinessFinance.Domain;

/// <summary>
/// Bir karşı tarafla olan hesabın o anki hâli: borçlandırmalar eksi tahsilatlar.
/// </summary>
/// <remarks>
/// <b>Kalıcı kolon değildir.</b> Cari bakiye hesap bakiyesi ve kart ekstresiyle
/// aynı ilkeyi izler: hareketlerden hesaplanır, saklanmaz (ADR 0014). Saklansaydı
/// iptal edilen bir hareket kolonu güncellemeyi unuttuğu anda uygulama iki
/// farklı doğru gösterirdi.
///
/// Hesap tek yönlü değil: aynı kişi hem müşteri hem tedarikçi olabilir ve iki
/// tarafı ayrı ayrı durur. <see cref="Net"/> ikisini tek cümleye indirir.
///
/// İptal edilmiş hareketler hiç sayılmaz — silme yerine iptal kuralının
/// buradaki karşılığı.
/// </remarks>
public sealed record CounterpartyBalance
{
    private CounterpartyBalance(decimal receivable, decimal payable)
    {
        Receivable = receivable;
        Payable = payable;
    }

    /// <summary>Karşı tarafın bize kalan borcu (veresiye satışlardan).</summary>
    public decimal Receivable { get; }

    /// <summary>Bizim karşı tarafa kalan borcumuz (vadeli alımlardan).</summary>
    public decimal Payable { get; }

    /// <summary>
    /// Artı: karşı taraf bize borçlu. Eksi: biz ona borçluyuz.
    /// </summary>
    public decimal Net => Receivable - Payable;

    /// <summary>
    /// Fazla tahsilat/ödeme <b>kırpılmaz</b>: eksiye düşen bir taraf, karşı
    /// tarafın bizde alacağı olduğu anlamına gelir ve gerçektir. Sıfıra
    /// çekmek, kullanıcının parasını ekranda yok etmek olurdu — fazla ödenmiş
    /// kart bakiyesinde aynı hata `docs/backlog.md` 1. maddede duruyor.
    /// </summary>
    public static CounterpartyBalance Calculate(
        IEnumerable<CounterpartyCharge> charges,
        IEnumerable<CounterpartyPayment> payments)
    {
        ArgumentNullException.ThrowIfNull(charges);
        ArgumentNullException.ThrowIfNull(payments);

        var receivable = 0m;
        var payable = 0m;

        foreach (var charge in charges)
        {
            if (charge.IsCancelled)
            {
                continue;
            }

            if (charge.Direction == DebtDirection.Receivable)
            {
                receivable += charge.Amount.Amount;
            }
            else
            {
                payable += charge.Amount.Amount;
            }
        }

        foreach (var payment in payments)
        {
            if (payment.IsCancelled)
            {
                continue;
            }

            if (payment.Direction == DebtDirection.Receivable)
            {
                receivable -= payment.Amount.Amount;
            }
            else
            {
                payable -= payment.Amount.Amount;
            }
        }

        return new CounterpartyBalance(receivable, payable);
    }
}
