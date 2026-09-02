# Rakip Arayüz ve Akış Araştırması

## Belge durumu

- Durum: Taslak araştırma planı
- Amaç: Aşama 06.2 arayüz düzeni için karar girdisi üretmek
- Sınır: Bu çalışma Aşama 06.2'yi açmaz ve uygulama kodunu değiştirmez
- Veri: Yalnız sentetik test verisi kullanılır

## Beklenen çıktılar

Araştırma sonunda birbiriyle bağlantılı üç belge hazırlanacak:

1. **Rakip Uygulamalarda Arayüz Yaklaşımları**
   - Bilgi hiyerarşisi, gezinme, renk, tipografi, kartlar, grafikler ve form
     desenleri
   - Güçlü ve zayıf yaklaşımlar
2. **Rakip Uygulamalarda Finansal Akışlar ve Özellikler**
   - Ana ekran, gelir/gider kaydı, işletme/şahsi ayrımı, planlama ve
     raporlama akışları
   - Akışlar arası bağlantılar ve kullanıcı sürtünmeleri
3. **BusinessFinance Arayüz ve Akış Önerisi**
   - Bulguların `doğrudan al`, `uyarlayarak al`, `alma` kararlarına
     dönüştürülmesi
   - Etkilenen ekran, akış ve ortak bileşenler

Nihai belgeler düzenlenebilir Word dosyası olarak hazırlanacak, onaylanan
sürümleri ayrıca PDF olarak dışa aktarılacak. Word ve PDF farklı içerikler
değil, aynı belgenin iki biçimi olacak. Taslak dosyaları, dosya adları ve
onay durumu `raporlar/README.md` içindedir.

## İlk karar: incelenecek uygulamalar

Her uygulamayı aynı tür rakip saymak yerine, BusinessFinance'in farklı bir
sorusuna cevap veren yedi uygulama kullanılacak.

### Çekirdek manuel test adayları

| Uygulama | Araştırmadaki rolü | Seçim gerekçesi |
|---|---|---|
| Paraşüt | Yerel doğrudan rakip | Türkiye'deki ön muhasebe, tahsilat/ödeme ve mobil işlem yaklaşımı |
| Logo İşbaşı | Yerel doğrudan rakip | Mikro işletme ve serbest meslek odağı; mobil nakit akışı ve belge girişi |
| KolayBi | Yerel akış referansı | Haftalık nakit akışı; fatura→tahsilat ve fatura→ödeme bağlantısı |
| QuickBooks Online / Solopreneur | Kavramsal yakın rakip | Tek kişilik işletmede işletme/şahsi işlem sınıflandırması |
| Money Manager (Realbyte) | Para modeli referansı | Manuel hesaplar, kart borcu/ödemesi, transfer, tekrarlayan işlem ve işletme/şahsi kayıtlar |
| Wallet by BudgetBakers | Finansal UX referansı | Günlük para takibi, ana ekran, kategori dağılımı ve rapor okunabilirliği |
| Bluecoins | Geniş para modeli referansı | Hesap, kart, borç, hatırlatma, nakit akışı, net varlık ve CSV akışları |

### Erişim veya ek karşılaştırma adayları

| Uygulama | Ne zaman kullanılır? |
|---|---|
| Bizim Hesap | Yerel örneklerden biri test edilemezse veya kapsamlı cari/vade yaklaşımı için ek kanıt gerekirse |
| Spendee | Wallet test edilemezse veya alternatif bir görsel para takibi yaklaşımı gerekirse |

QuickBooks Solopreneur'a bölgesel ya da ödeme kaynaklı erişilemezse manuel
test yerine resmî yardım sayfaları ve ürün videoları kullanılır. Bu durumda
bulgular `manuel gözlem` değil `resmî kaynak` olarak işaretlenir.

### Seçim ölçütleri

Bir uygulama listeye yalnız aşağıdaki sorulardan en az ikisine anlamlı cevap
veriyorsa girer:

- Şahıs şirketi veya mikro işletmenin gündelik finans işlerini kolaylaştırıyor mu?
- Gelir, gider, tahsilat, ödeme veya nakit akışı için incelenebilir bir akışı var mı?
- BusinessFinance'ten farklı ve öğretici bir arayüz yaklaşımı sunuyor mu?
- Deneme hesabı, demo veya güvenilir resmî kaynakla incelenebiliyor mu?

Stok, bordro, banka bağlantısı veya vergi hesaplama tek başına seçim nedeni
değildir; bunlar BusinessFinance kapsamının dışındadır.

## BusinessFinance karar filtresi

Rakipte bulunması bir özelliğin BusinessFinance'e alınacağı anlamına gelmez.
Her bulgu aşağıdaki sabit kararlarla karşılaştırılır:

- İşletme ve şahsi para tek havuzda yaşar; ayrım raporlama boyutudur.
- Bakiye, kart borcu ve net varlık kapsam filtresinden etkilenmez.
- Kapsam ile indirilebilirlik ayrı alanlardır.
- Transfer ve kart ödemesi gider değildir.
- Vergi hesaplanmaz ve beyanname üretilmez.
- Muhasebe kârı yerine nakit esaslı işletme neti kullanılır.
- Banka bağlantısı ve açık bankacılık kapsam dışıdır.

## Yapay zekâ ve manuel test iş bölümü

### Yapay zekânın yapacağı

- Resmî ürün ve yardım sayfalarından masa başı araştırma
- Ekran görüntülerinin ortak başlıklarla incelenmesi
- Manuel notların tablolara ve akış şemalarına dönüştürülmesi
- Uygulamalar arası benzerlik, fark ve çelişkilerin bulunması
- Üç nihai belgenin ilk taslağının yazılması

### Kullanıcının yapacağı

- Uygulamaya girip akışın gerçekten nasıl çalıştığını doğrulamak
- Sentetik verilerle belirlenen görevleri tamamlamak
- Kritik adımlarda ekran görüntüsü almak ve kısa sesli not tutmak
- Yapay zekânın yorumlarını ve BusinessFinance için önerilen kararları onaylamak

## Kanıt kuralları

Her bulgu aşağıdaki etiketlerden birini taşır:

- **Manuel gözlem:** Uygulamada sentetik veriyle doğrulandı.
- **Resmî kaynak:** Ürünün kendi sitesi, yardım merkezi veya resmî videosunda anlatıldı.
- **Yorum:** Gözlem veya kaynaktan BusinessFinance için yapılan çıkarım.
- **Doğrulanamadı:** Erişim, ücretli plan veya bölge sınırı nedeniyle kanıtlanamadı.

Yorum, gözlemlenmiş ürün davranışı gibi yazılmaz.

## Uygulama sırası

Yedi rakip uygulama `emulator-5554` cihazına kuruldu (adb ile doğrulandı,
1 Eylül 2026); aday listesi manuel teste hazırdır. Test yöntemi önce
**Money Manager (Realbyte)** üzerinde kısa bir pilotla sınanır. Bunun nedeni
ürün üstünlüğü değil, şirket/VKN kaydı gerektirmeden temel para hareketlerinin
hızlı kurulabilmesidir. Ardından doğrudan rakip akışını görmek için **Paraşüt**
test edilir. Pilot formu yeterliyse kalan uygulamalara aynı yöntem uygulanır.

Manuel test yapay zekâ ve kullanıcı tarafından **aynı anda** yürütülür: yapay
zekâ emülatörü sürer ve not alır, kayıt/doğrulama kapılarında durup kullanıcıya
sorar, kullanıcı devam ettirir. Ortak akış ve canlı durum tablosu `DURUM.md`
içindedir.

Pilot testte yalnız şu beş alan incelenecek:

1. Ana ekran
2. Gelir ekleme
3. Gider ekleme
4. İşletme/şahsi ayrımı veya en yakın sınıflandırma davranışı
5. Aylık rapor veya nakit akışı

## Karar sonucu

Manuel test listesi:

- İndirilecek adaylar: **Paraşüt, Logo İşbaşı, KolayBi, QuickBooks
  Online, Money Manager (Realbyte), Wallet by BudgetBakers, Bluecoins**
- QuickBooks hesabında Solopreneur planı bölge nedeniyle açılamazsa uygulama
  manuel test listesinden çıkar ve resmî kaynakla incelenir
- Yedek veya ek masa başı referansı: **Bizim Hesap, Spendee**

Test yöntemi ve ortak veri seti şu iki belgede tanımlıdır:

- `MANUEL-TEST-PROTOKOLU.md`
- `SENTETIK-TEST-VERISI.md`

Her uygulama tamamlandığında `UYGULAMA-GOZLEM-SABLONU.md` kopyalanarak ayrı bir
gözlem formu oluşturulur.
