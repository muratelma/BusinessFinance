**Bu sürümün ana yönünü destekliyorum. İlk beş sayısal tablodaki 19 satırın hesabını yeniden kontrol ettim; hepsi tutuyor.** G1, G4, G5, G6 ve G7’nin yönü uygun. G2’deki birleştirme de Tablo 5’in koşullarında doğru. Ancak G3’te hâlâ doğru sonucu seçmenin mümkün olmadığı bir durum var; koda geçmeden önce bunu tamamlamak gerekiyor.

[Aşama belgesindeki güncel karar bölümünü](C:/Users/elma6/Documents/BusinessFinance/stages/06.3-butunsel-duzenleme.md) ve mevcut seçim sözleşmesini inceledim. Bu değerlendirme ürün kararı veya uygulama onayı yerine geçmez.

**G3’ün üç seçeneği, tek tahsilatın kısmen bugünkü satışa ait olduğu durumda yetmiyor.**

Somut örnek:

- Günün ayrıca kaydedilmemiş normal nakit satışı: **1.000 TL**.
- Uygulamada kayıtlı yeni veresiye satış: **500 TL**.
- Müşteri tek seferde **300 TL** ödüyor: **200 TL bugünkü satış için, 100 TL eski borç için**. Uygulamaya tek tahsilat kaydı giriliyor.
- İncelenen sorunlu cihaz kullanımında bugünkü satışın tamamı nakitte gösterilmiş; eski borçtan gelen 100 TL de girilen nakit toplamına dahil. Bugünkü satış için alınan 200 TL bu toplamda ayrıca tekrar yer almıyor.
- Gün sonuna yazılan tutar: **1.000 + 500 + 100 = 1.600 TL**.

Doğru sonuç: yeni satış **1.000 TL**, günün toplam geliri **1.500 TL**, kasa girişi **1.300 TL**. Bunun için **600 TL** düşülmeli.

| Seçenek | Düşülen | Yeni satış | Günün geliri | Kasa girişi |
|---|---:|---:|---:|---:|
| Yalnız tahsilat | 300 | 1.300 | 1.800 | 1.600 |
| Yalnız satış | 500 | 1.100 | 1.600 | 1.400 |
| İkisi ayrı tutarlar | 800 | 800 | 1.300 | 1.100 |
| **Ortak 200 TL bir kez sayılır** | **600** | **1.000** | **1.500** | **1.300** |

**“Bazıları” ile satır seçmek de bunu çözmüyor.** İki kayıt 500 ve 300 TL; tam kayıtları seçerek yalnız 0, 300, 500 veya 800 TL düşülebilir. Doğru olan 600 TL hiçbir seçimde yok. Bu, yalnız birden fazla satış olduğunda ortaya çıkan bir sorun da değil; bir satış ve bir tahsilat yeterli.

Önerim: üç basit seçeneğe ek olarak **kısmi örtüşmeyi ifade eden bir yol** olsun. Bu örnekte kullanıcı iki kayıtta ortak olan tutarı 200 TL olarak belirtir; hesap **500 + 300 − 200 = 600 TL** olur. Bu ayrıntı yalnız gerektiğinde açılabilir.

Bunun için cari hesapta bütün ödemeleri satışlara dağıtan bir sistem kurmak gerekmiyor. Ancak **gün sonundaki düşümün hangi kayıtlardan ve hangi ortak tutardan oluştuğu saklanmalı**. Kullanıcının seçimi sonradan açıklanabilir ve geri alınabilir olmalı. Sadece kişi başına “ayrı tutarlar” onayı bu bilgiyi taşımaya yetmez.

Mevcut [istek sözleşmesi](C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/DayCloses/DayCloseContracts.cs) dahil/dışarıda seçimi, [sayılan kayıt bağı](C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Domain/DayCloseCountedRecord.cs) ise yalnız kayıt kimliği taşıyor. Dolayısıyla “iki kayıt türü ve bir onay eklemek yeter” diye uygulama kapsamı kesinleştirilmemeli; kısmi düşümün sözleşme, saklama ve yedek etkisi de önceden belirtilmeli.

**G5’te bir bilgi sınırı daha korunmalı.**

Cari tahsilat belirli bir satışa bağlanmıyorsa, uygulama tek tek veresiye satışların altında “200 TL alındı” veya “tahsilat yok” bilgisini kesin olarak söyleyemez. Aynı kişinin o günkü tahsilatı eski borca ait olabilir; bu zaten G3’ün gerekçesi.

Bu nedenle:

- Bağlantısı bilinen alacak faturasında gerçek tahsilat durumu gösterilebilir.
- Bağlantısız cari satışta satış tutarı ve tarihi gösterilir; satışa ait tahsilat durumu uydurulmaz.
- Kişinin tahsilatları ayrı gösterilebilir. Gün sonu için verilen eşleştirme cevabı, kendiliğinden cari defterin kesin ödeme dağılımına dönüşmez.

**Belgenin sonundaki üç soruya cevabım şu:**

1. **G3 kişi/gün toplamlarıyla yeterli mi?** Basit örneklerde evet; genel çözüm olarak hayır. Satır seçimiyle birlikte gerektiğinde kısmi ortak tutarı ifade edebilmek gerekiyor. Yalnız satış başına soru sormak da tek başına bu eksikliği kapatmıyor.

2. **G4’te bölüm başına soru taşınabilir mi?** Başlangıç için makul. Yalnız ilgili kayıtlar ve ilgili nakit girdisi varsa sorulsun; kartla sınırlı bir girişte nakit soruları kayıt işlemini engellemesin. “Bazıları” veya çakışma halinde ek işlem gerektiği için “her zaman tek dokunuş” denmemeli. Kullanım yükü gerçek bir akış denemesinde ölçülmeli. Soru yorucu bulunursa önce gruplama ve anlatım iyileştirilsin; dünkü cevabın bugüne otomatik uygulanması güvenilir veri yerine geçmez.

3. **Aynı gün kapatılan alacak faturası tek satır olabilir mi?** Tablo 5’teki gibi, faturanın ve kendi kapanışının aynı rapor tutarını temsil ettiği durumda evet. Fakat “tek satır” ile “girilen nakde dahil” farklı kararlardır. Bu birleşik satır da G4’teki Hepsi/Hiçbiri/Bazıları cevabına tabi olmalı. Gün sonunda kullanıldıysa hem fatura hem bağlı kapanış için iptal/geri alma bütünlüğü korunmalı.

**Tablo 6’nın açıklaması da daraltılmalı.** “Kredili satış” ile “yemek kartı satışı” aynı örnekte birbirinin yerine kullanılmamalı. Önceden gelir yazılmış 300 TL veresiye yeniden yazılırsa gelir şişer. Henüz kaydedilmemiş 300 TL yemek kartı satışı yanlışlıkla nakde yazılırsa sorun her zaman fazla toplam gelir değildir; yanlış kasa, ödeme kanalı ve muhtemel komisyon hesabıdır. İki örnek, önceki kayıtları açıkça belirtilerek ayrı test olmalı.

Aynı tabloda yalnız toplam 1.800 ve kart 500 girildiğinde nakde dokunulmaması G1’e uygundur. Ancak ekran, yalnız 500 TL kart kaydedildiğini ve 1.300 TL farkın kaydedilmediğini açıkça göstermeli; bütün satışların kaydedildiği izlenimini vermemeli.

**Uygulama için son iki güvenceyi de kabul listesine eklerdim:**

- “Hepsi” cevabı, kullanıcının gördüğü kayıtlar için geçerli olsun. Önizleme ile kayıt arasında yeni satış/tahsilat gelirse veya tutarlar değişirse uygulama yeni durumu yeniden gösterip gerekli cevabı alsın; yeni kayıt sessizce eski cevaba dahil edilmesin.
- Düşüm, ortak tutar ve çakışma kuralları sunucuda da doğrulansın. Aynı kayıt/tutar başka bir gün sonunda tekrar düşülemesin; geri alma bağlantıları doğru serbest bıraksın. Ortak tutar ilgili kayıtların tutarlarını aşamasın.

**Benim önerim: ana yönü kabul edilebilir say; G3’e bu tek somut karşı örneği ve çözümünü eklet, G5’in bilgi sınırını düzelt, ardından sözleşme ve kabul testleriyle uygulamaya geç.** Yeni bir geniş araştırma turu gerekmiyor. Kalan iş, doğru olduğu söylenen sonucun gerçekten seçilebilir ve saklanabilir olmasını sağlamak.

Uygulama kodunu ve karar belgesini değiştirmedim. İlk beş tablonun 19 satırını ve yeni karşı örneğin dört sonucunu aritmetik olarak doğruladım; build veya uygulama testi çalıştırmadım.
