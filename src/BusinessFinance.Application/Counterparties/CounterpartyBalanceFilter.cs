namespace BusinessFinance.Application.Counterparties;

/// <summary>
/// Cari listesinin hangi tarafını okuduğu.
/// </summary>
/// <remarks>
/// Sıfır bakiyeli karşı taraf silinmez ve listeden düşmez; yalnız ayrı okunur.
/// Bir müşteriyle hesabın kapanmış olması onunla iş yapılmadığı anlamına
/// gelmez — <see cref="Settled"/> geçmişi arayan kullanıcının sorusudur.
/// </remarks>
public enum CounterpartyBalanceFilter
{
    /// <summary>Bütün karşı taraflar, bakiyesi olsun olmasın.</summary>
    All = 0,

    /// <summary>Yalnız açık hesabı olanlar.</summary>
    Open = 1,

    /// <summary>Yalnız kapanmış cariler.</summary>
    Settled = 2
}
