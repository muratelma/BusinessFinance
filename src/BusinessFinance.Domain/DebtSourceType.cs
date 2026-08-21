namespace BusinessFinance.Domain;

/// <summary>
/// Borcu ne doğurdu — açılış anında ne olduğunu söyleyen ayrım.
/// </summary>
/// <remarks>
/// Borç açılışı bugüne kadar hiçbir kayıt üretmiyordu; para hesaptan taksit
/// taksit çıkıyor ama hiçbir rapor bunu görmüyordu. Eksik olan ödeme değil
/// açılıştı: borç doğduğunda ya para bize girmiştir (gider yok) ya da bir şey
/// tüketilmiştir (gider var). Ayrımı taşıyan alan budur.
///
/// Kredi kartındaki ayrımın aynısı: <see cref="CreditCardCharge"/> gider ve
/// kategori taşır, <see cref="CreditCardPayment"/> taşımaz. Borçta bu ayrım
/// yoktu.
/// </remarks>
public enum DebtSourceType : byte
{
    /// <summary>
    /// Açılışı kaydedilmemiş borç. Yalnız bu ayrımdan önce açılmış kayıtlar
    /// ve eski sürüm yedekler bu durumda olabilir.
    /// </summary>
    /// <remarks>
    /// Üçüncü bir durum istemeye istemeye eklendi, çünkü iki alternatifi de
    /// yanlıştı: eski borçlara "nakit" demek o tarihte olmamış bir para
    /// girişi uydurur ve sözleşme tarihinden bugüne bütün bakiyeleri
    /// değiştirir; hiçbir şey yapmamak ise kalıcı ve görünmez bir tutarsızlık
    /// bırakır. Bu durum eksiği <b>görünür</b> kılar: uygulama sorar, gerçeği
    /// bilen kullanıcı <see cref="DebtAgreement.RecordOpening"/> ile tamamlar.
    ///
    /// Yeni borç bu durumda açılamaz — kurucu reddeder.
    /// </remarks>
    Unrecorded = 0,

    /// <summary>
    /// Para hesaba girdi (borç) ya da hesaptan çıktı (alacak). Gider veya
    /// gelir yoktur — para el değiştirdi, tüketilmedi.
    /// </summary>
    Cash = 1,

    /// <summary>
    /// Bir şey tüketildi, karşılığı borçlanıldı. Anapara açılış anında
    /// kategorili gider olur; taksit ödemeleri yalnız bakiye hareketidir.
    /// Yalnız borç yönünde geçerlidir.
    /// </summary>
    Expense = 2,

    /// <summary>
    /// Bir şey satıldı ya da hizmet verildi, bedeli sonra alınacak. Anapara
    /// açılış anında kategorili <b>gelir</b> olur; tahsilatlar yalnız bakiye
    /// hareketidir. Yalnız alacak yönünde geçerlidir.
    /// </summary>
    /// <remarks>
    /// <see cref="Expense"/>'in aynadaki hâli. Bu kaynak olmadan "telefonumu
    /// sattım, üç ayda ödeyecek" durumu ancak nakit alacak olarak
    /// girilebiliyordu — uygulama hesaptan para çıkmış gibi davranıyor, satış
    /// da hiçbir zaman gelir olarak görünmüyordu.
    ///
    /// "Birinin yerine bir gideri ben ödedim" durumu bu kaynak <b>değildir</b>;
    /// o hâlâ nakit alacaktır (kendi payınız gider, karşı tarafın payı nakit
    /// alacak). Onu gider kaynağı yapmak gideri iki kez saydırırdı.
    /// </remarks>
    Income = 3
}
