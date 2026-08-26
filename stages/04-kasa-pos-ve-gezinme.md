# Aşama 04 — Kasa, POS ve gezinme

## Belge durumu

- Durum: **Aktif** (24 Ağustos 2026'da kullanıcı onayıyla açıldı)
- Ön koşul: Aşama 03 — Yükümlülük ve vade
  (**tamamlandı**, `docs/archive/stages/03-yukumluluk-ve-vade.md`)
- Sonraki aşama: Aşama 05 — Vergi ve muhasebeci
- Dokunulacak kalıcı belgeler: `documentation/architecture.md`,
  `documentation/flows.md`, `documentation/permissions.md`,
  `documentation/tests.md`, `documentation/design-system.md`,
  `documentation/financial-activity-api-contract.md`,
  `documentation/restore-runbook.md`,
  `documentation/adr/0015-card-debt-and-card-collection-are-two-things.md`
  (**yazıldı ve kabul edildi**)
- Doğrulanmış ilerleme: `docs/project-status.md`

## Amaç

Tezgâh üstü esnafın günlük ritmi uygulamada yok. Gün sonunda kasayı sayar —
uygulamada karşılığı yok. Müşteri kartla öder, para birkaç gün sonra komisyon
düşülmüş olarak bankaya geçer — uygulamada karşılığı yok; üstelik uygulamadaki
`CreditCard` tam **tersi** şeydir, borçlandığın karttır.

Bu aşama günlük ritmi ekler ve gezinmeyi yeniden kurar. Gezinme burada, çünkü
kasa ekranının oturacağı yer burasıdır ve `İşlem ekle` menüsü tam bu aşamada
taşar.

## Kullanıcıya katkı

- Gün sonunda kasayı sayıp uygulamanın beklediği tutarla karşılaştırmak
- Kartla yapılan tahsilatı girmek; paranın ne zaman hesaba geçeceğini görmek
- POS komisyonunu gerçek gider olarak görmek — bugün görünmüyor
- Henüz hesaba geçmemiş parayı kullanılabilir bakiye sanmamak
- Aradığı ekranı dört sekmede bulmak; `İşlem ekle` menüsünde ne seçeceğini
  bilmek

## Değiştirilmeyecek mimari kararlar

- **Ekonomik olay tanır, ödeme taşır** (Aşama 02 ADR'si). POS tahsilatı bu
  kuralın kapsamındadır.
- Bakiye kalıcı kolon değildir; bloke tutar da kalıcı kolon değildir.
- Kapsam boyutu ve türetme zinciri (ADR 0013). Onboarding ön ayarı **özellik
  kapatmaz**.
- Net varlık borcu anaparayla ölçer (ADR 0010).
- Silme yerine iptal.
- Tasarım sistemi token'ları ve erişilebilirlik kapısı.

## Bu aşamanın karar kapısı: "kredi kartı" iki şeydir — **kapatıldı**

Aşama başlamadan **bir ADR yazıldı ve kabul edildi**
(`documentation/adr/0015-card-debt-and-card-collection-are-two-things.md`). Bugünkü `CreditCard` borçlandığın karttır;
esnafın gündelik dilinde "kart" ise tahsilat aracıdır. İkisi aynı ekranda aynı
kelimeyle görünürse kullanıcı hangi yöne baktığını bilemez.

ADR'nin karara bağlayacağı üç şey:

1. Arayüz adlandırması: borç tarafı ve tahsilat tarafı **hangi kelimelerle**
   ayrılır.
2. POS tahsilatının bir hesap türü **olmadığı**: bloke edilmiş para bir
   projection'dır, `AccountType`'a yeni değer eklenmez.
3. Kullanılabilir bakiye ile net varlığın farkı: yoldaki para net varlığa
   girer, kullanılabilir bakiyeye girmez.

## Kapsam

### Dahil

- Gün sonu nakit sayımı ve fark kaydı
- POS tahsilatı: brüt tutar, komisyon, net tutar, hesaba geçiş
- Bloke (yoldaki) tutarın projection olarak hesaplanması
- Ana sekme yapısının yeniden kurulması
- `İşlem ekle` menüsünün niyet eksenine geçmesi
- Yedek şeması v9

### Açıkça kapsam dışında

- Gerçek POS cihazı veya banka entegrasyonu — **kapsam dışı**, veriler elle
  girilir
- Z raporu okuma, yazarkasa entegrasyonu
- Vardiya, kasiyer, personel ayrımı
- Otomatik komisyon oranı çekme; oran kullanıcıdan alınır
- KDV alanları — Aşama 05

## Çalışma grupları

### Grup 1 — ADR: kart borcu ile kart tahsilatı ayrımı — **Tamamlandı**

- Yukarıdaki üç karar yazılır; reddedilen seçenekler (yeni `AccountType`,
  bloke tutarın kalıcı kolon olması) gerekçeleriyle kaydedilir.
- Ölçüt: ADR kabul edildi ve `CLAUDE.md` belge haritasına eklendi.

Kabul edilen kararlar (`documentation/adr/0015-*.md`):

- **Adlandırma:** borç tarafı `Kredi kartlarım` adını korur; tahsilat tarafı
  `POS tahsilatları` olur ve o ekranda `kart` kelimesi tek başına hiç
  kullanılmaz. Bekleyen para arayüzde `yolda` diye adlandırılır — `bloke`
  bankacılık jargonudur, kullanıcının kelimesi değildir.
- **POS tahsilatı bir hesap türü değildir:** `AccountType` genişlemez, yoldaki
  tutar her sorguda hesaplanan bir projection'dır. Hesap yapılsaydı kullanıcı
  oradan transfer edebilir, kart borcu ödeyebilir ve onu kasa sayımına
  katabilirdi; üçü de olmamış parayı harcamaktır.
- **Kullanılabilir bakiye yoldaki parayı içermez, net varlık içerir**; ikisi
  arasındaki fark tam olarak yoldaki tutardır ve bu bir test kapısıdır.
- **Üçüncü ana sekme ön ayara göre değişir:** işletmede `Kasa`, kişiselde
  `Bütçeler`; yerini veren sekme `Diğer` altına iner ve kaybolmaz.

### Grup 2 — Domain: gün sonu kasa sayımı — **Tamamlandı**

- `CashCount`: tarih, nakit hesabı, sayılan tutar, kapsam, not. **Beklenen
  tutar saklanmaz** — sayım anındaki bakiye projection'dan okunur ve fark
  türetilir.
- Fark **otomatik düzeltme hareketi üretmez.** Sayım hatasını gerçek para
  hareketi gibi yazmak, olmamış bir gideri kayda geçirmek olurdu.
- Kullanıcı açıkça onaylarsa fark, `Kasa farkı` kategorisiyle bir gelir/gider
  kaydı üretir. Bu ikinci ve ayrı bir eylemdir.
- Aynı gün ve aynı hesap için ikinci sayım öncekini iptal eder, üzerine
  yazmaz.
- Ölçüt: fark doğru hesaplanıyor; onaysız hiçbir finansal kayıt üretilmiyor.

Uygulananlar:

- `CashCount` bir gözlemdir: hesap bakiyesine dokunmaz, gelir/gider yazmaz.
  Beklenen tutar **saklanmaz**; fark `DifferenceFrom(expectedBalance)` ile
  okunduğu anda türetilir. Aynı sayım, bir hareket sonradan iptal edilince
  farklı bir fark verir ve bu doğru davranıştır.
- `CashCountDifference` yönü anlamıyla taşır: fazla gelir, eksik gider. Düzeltme
  tutarı `Money` sözleşmesi gereği pozitiftir; yönü `TransactionType` taşır.
  Dengede tür sorulamaz ve sıfır tutarlı kayıt yazılamaz.
- Sayılan tutar `Money` değildir: **sıfır meşrudur** (boş kasa da sayılır),
  negatif değildir.
- `RecordAdjustment` idempotenttir ve bir sayımın ikinci bir düzeltmesi olamaz.
  Sayım kendiliğinden hiçbir finansal kayıt üretmez.
- `SupersedeWith` aynı gün + aynı hesabın ikinci sayımında öncekini iptal eder;
  başka gün, başka hesap veya kendisi bir sayımı kapatamaz.
- Yalnız kullanıcının kendi, aktif ve **nakit** hesabı sayılır. Kapsam sayım
  anında sabitlenir, düzeltme anında yeniden türetilmez.
- **Bu grup yalnız Domain'e dokundu.** `CashCount` ve Grup 3'ün üreteceği
  `PosSettlement` için EF modeli, migration ve yazma yolu **Grup 4'te**
  eklenecek: ikisini SQL'den okuması gereken ilk grup odur ve kalıcılığı iki
  ayrı yere bölmek migration zincirini gereksiz uzatırdı.

### Grup 3 — Domain: POS tahsilatı — **Tamamlandı**

- `PosSettlement`: tahsilat tarihi, brüt tutar, komisyon oranı veya tutarı,
  net tutar, beklenen geçiş tarihi, hedef hesap, gerçekleşme durumu, kapsam.
- **Ekonomik olay tahsilat anındadır:** gelir brüt tutar kadar tanınır,
  komisyon gider olarak yazılır. Hesap bakiyesi **değişmez**.
- **Geçiş anında:** hedef hesap net tutar kadar artar, gelir/gider **sıfır**.
  Aynı satışın ikinci kez sayılması böyle önlenir.
- Komisyon brüt tutardan **ayrı okunur ve ona eklenmez** — dekonttaki işlem
  ücretinde öğrenilen dersin aynısı.
- Bloke (yoldaki) tutar = geçmemiş tahsilatların net toplamı; **projection**.
- Ölçüt: tahsilat + geçiş senaryosunda gelir bir kez, komisyon bir kez sayılıyor.

Uygulananlar:

- `PosSettlement` ADR 0014'ün ayrımını tek kaydın **iki anına** koyar: tahsilat
  günü gelir brüt tutar kadar tanınır ve komisyon ayrı gider yazılır, hesap
  kıpırdamaz; geçiş günü hesap net tutar kadar artar ve hiçbir gelir/gider
  yeniden yazılmaz. `SignedAccountEffect` geçişe kadar sıfırdır.
- `MarkTransferred` idempotenttir ve ikinci bir günle işaretlemeyi reddeder;
  para bir kez geçer. Geçiş günü satıştan önce ve gelecekte olamaz.
- Komisyon `Money` değildir: **sıfır meşrudur**. Komisyon ile gider kategorisi
  birlikte bulunur ya da hiç bulunmaz; komisyon brütün tamamını yiyemez.
- **Komisyon oranı saklanmaz, paradan çözülür** (ADR 0009'un aynı kararı).
  `CommissionFromRate` oranı tutara çevirir ve yuvarlama para tarafında yapılır.
- Para **banka hesabına** geçer (ADR 0015): kasa bir kart ödemesi alamaz. Satış
  bir gelir kategorisi ister.
- `PosTransitBalance` yoldaki parayı verir: bekleyen tahsilatların **net**
  toplamı, kalıcı kolon değil. İptal edilmiş ve geçmiş tahsilatlar sayılmaz.
- Bu grup da yalnız Domain'e dokundu; kalıcılık Grup 4'tedir.

### Grup 4 — Kalıcılık, bakiye, net varlık ve Özet ekranı — **Tamamlandı**

- Grup 2 ve 3'ün domain tipleri (`CashCount`, `PosSettlement`) bu grupta
  kalıcılığa girer: EF yapılandırması, owner-scoped bileşik anahtarlar ve tek
  migration. İki modelin kalıcılığını ayrı gruplara bölmek migration zincirini
  gereksiz uzatırdı.
- Kullanılabilir bakiye: yoldaki parayı **içermez**.
- Net varlık: yoldaki parayı **içerir**, ayrı satır olarak gösterilir.
- Özet ekranına "yolda olan" satırı eklenir; gün sonu farkı varsa uyarı bandına
  düşer.
- Ölçüt: iki sayı arasındaki fark tam olarak yoldaki tutara eşit.

Uygulananlar:

- `AddCashCountsAndPosSettlements` iki tabloyu kurdu. İkisi de **boş doğuyor**,
  bu yüzden zorunlu kolonlar backfill istemiyor ve kalıcı DEFAULT bırakılmıyor;
  migration kuralının "gerçekten boş tablo" istisnası burada bilinçli olarak
  kullanıldı. Yükseltme iki sentetik veritabanına da uygulandı.
- Türetilen hiçbir şey kolon değil: net tutar, komisyon oranı, "yolda mı",
  beklenen bakiye ve fark şemada **yok**. Migration testi bu yokluğu sınıyor.
- `CashCounts` üzerinde filtreli tekil indeks (`IsCancelled = 0`) bir gün ve bir
  kasa için tek açık sayım bırakıyor; domain kuralı SQL seviyesinde de duruyor.
- Geçmiş POS tahsilatı hesap bakiyesine **net** giriyor, geçmemiş hiç girmiyor.
  Bakiyeyi hesaplayan iki yer de (tek hesap ve rapor dağılımı) aynı kuralı
  uyguluyor.
- Net varlık `moneyInTransit` taşıyor; kullanılabilir bakiye taşımıyor. İki
  sayının farkı tam olarak yoldaki tutar ve bu gerçek SQL testiyle kapıya
  bağlandı. Sabit SQL komut kapısı 61'den 63'e çıktı, kayıt adediyle büyümüyor.
- Özet ekranına `Yolda` satırı eklendi (alt başlık `POS tahsilatı`); yolda para
  yokken çizilmiyor. Arayüz ADR 0015'in kelimesini kullanıyor, `bloke` demiyor.

- POS satışı **tahsil edildiği gün** rapora giriyor: brüt gelir ve komisyon
  gideri dönem toplamlarına, kategori dağılımına, nakit akışı eğilimine ve
  (komisyon) bütçe sapmasına katılıyor. Geçiş günü rapora hiçbir şey eklemiyor;
  eklerse aynı satış iki kez sayılırdı. Sabit SQL komut kapısı 63'ten 69'a
  çıktı ve hiçbiri tahsilat adediyle büyümüyor.
- Komisyon bütçeyi tüketiyor (kendi kategorisi olan, o gün tanınmış gerçek bir
  gider); brüt tutar tüketmiyor çünkü o bir gelirdir.

Yazma uçları bu grupta **açılmadı**: kasa sayımı ve POS tahsilatı uçları,
tükettikleri ekranlarla birlikte Grup 7'ye alındı — Aşama 03'te yükümlülük
uçlarının ekranlarıyla aynı checkpoint'te açılmasındaki gerekçenin aynısı.

### Grup 5 — Ana sekme yapısı — **Tamamlandı**

- Bugünkü sekmeler: `Özet · İşlemler · Bütçeler · Diğer`. `Bütçeler` ana sekme
  olarak bir ev bütçesi kavramı; işletme kullanıcısının ikinci ekranı kasadır.
- **Karar (ADR 0015):** üçüncü sekme onboarding ön ayarına göre değişir —
  işletme kullanıcısında `Kasa`, kişisel kullanıcıda `Bütçeler`. Yerini
  değiştiren sekme **kaybolmaz**, `Diğer` altına iner.
- Bu, ADR 0013'ün "ön ayar özellik kapatmaz" kuralını bozmaz: hiçbir özellik
  kapanmıyor, yalnız hangisinin bir dokunuş uzakta olduğu değişiyor.
- Ölçüt: iki profilde de dört sekme dolu ve hiçbir ekran erişilemez değil.

Uygulananlar:

- Üçüncü shell dalı profil cevabını dinliyor: işletmede `Kasa`, kişiselde
  `Bütçeler`. Aynı karar compact alt çubukta ve medium/expanded gezinme rayında
  uygulanıyor; dal sayısı ve geri yığını değişmiyor.
- Yerinden inen hedef `Diğer` altında profile göre gösteriliyor: işletmede
  `Bütçeler`, kişiselde `Kasa`. Profil ayarı sonradan değişince hem ana hedef
  hem ikincil kapı aynı anda yer değiştiriyor; hiçbir özellik kapanmıyor.
- İki profil için yönlendirme testi dört ana hedefi, üçüncü hedefin gerçek
  içeriğini ve `Diğer` altındaki karşı hedefin açılabildiğini doğruluyor.

### Grup 6 — `İşlem ekle` menüsünün yeniden kurgusu — **Tamamlandı**

- Bugün yedi düz seçenek: gider, fiş, gelir, dekont, transfer, kart ödemesi,
  tekrarlayan plan. Bu aşamada kasa ve POS eklenince liste taşıyor.
- Düz liste yerine **niyet ekseni**: para girdi / para çıktı / belge okut /
  plan kur. Alt seçenekler bu dört başlığın altına dağılır.
- Menü hâlâ tek giriş noktasıdır; shell ve İşlemler ekranındaki iki eylem aynı
  launcher'ı açmaya devam eder.
- Ölçüt: sekiz üstü seçenek tek ekranda kaydırmadan okunabiliyor; erişilebilirlik
  kapısı geçiyor.

Uygulananlar:

- Menü **beş başlıkla** kuruldu: `Para girdi`, `Para çıktı`, `Para taşı`,
  `Belge okut`, `Plan kur`. Belgedeki dört eksene beşincisi eklendi çünkü
  transfer ile kart borcu ödemesi **gider değildir** (ADR 0014): ödemeyi
  taşırlar, gelir/gider yazmazlar. `Para çıktı` altına konsalardı menünün
  kendisi raporu yanlış anlatırdı.
- POS tahsilatı menüye `Para girdi` altında girdi ve `Kasa` ekranını POS
  sekmesi seçili açıyor (`/more/cash?tab=pos`). Menü ikinci bir kopya form
  açmıyor; tahsilat formu listeyle aynı yerde kalıyor.
- **Gün sonu kasa sayımı menüye alınmadı.** Hiçbir para hareketi üretmeyen bir
  gözlemdir ve `Kasa > Gün sonu` ekranında durur; menüye alınsaydı `İşlem ekle`
  işlem olmayan bir şeyi işlem gibi gösterirdi. Hesap açma ve CSV içe aktarma
  ile aynı gerekçe.
- Açıklama satırı her satırdan kaldırıldı, yalnız yanlış anlaşılabilecek
  satırlarda bırakıldı (`POS tahsilatı`, `Ödenmemiş fatura`, `Kredi kartı borcu
  öde`, iki belge satırı, tekrarlayan plan). Dokuz açıklama listeyi ekranın
  dışına taşırıyordu.
- Ölçüt teste bağlandı: 400×800 telefonda dokuz satırın hepsi **kaydırmadan**
  hit-test edilebiliyor ve aynı testte erişilebilirlik kapısı geçiyor; 2× yazı
  ölçeğinde taşma yok (o ölçekte kaydırma meşrudur).
- Tek launcher kuralı korundu: kabuktaki çentikli buton ve İşlemler ekranındaki
  eylem aynı menüyü açmaya devam ediyor.

### Grup 7 — Yazma uçları ve Flutter ekranları — **Tamamlandı**

**Sıra kararı:** bu grup Grup 5'ten **önce** yapıldı. Grup 5'in üçüncü sekmesi
`Kasa` ekranına işaret ediyor ve o ekran ile onu besleyen uç burada doğuyor;
sırayla gidilseydi sekme ilk günden boş bir yere açılır ve grubun kendi ölçütü
("iki profilde de dört sekme dolu") daha yazıldığı gün ihlal edilirdi.

- Kasa sayımı ve POS tahsilatının yazma uçları (oluşturma, farkı onaylama,
  geçişi işaretleme) bu grupta açılır: uç ile onu tüketen ekran aynı
  checkpoint'te doğar.
- `features/cash/`: gün sonu sayım ekranı, geçmiş sayımlar, fark kaydı onayı.
- POS tahsilatı girişi ve bekleyen geçişler listesi; geçişi işaretleme.
- `Diğer` menüsü yeni yerleşime göre düzenlenir.
- `FinancialDataChanges` yeni hedefler alır; kasa sayımı bütçeyi yükseltmez.
- Ölçüt: zorunlu ekran durumları; tasarım sistemi kontrol listesi tamamlandı.

Uygulananlar:

- Sunucu uçlarının yanına açık Dart modelleri, repository ve controller
  katmanları eklendi; para JSON'da ve istemcide string olarak korunuyor.
- `Kasa` ekranı `Gün sonu` ve `POS tahsilatları` alt sekmelerini taşıyor.
  Gün sonu beklenen/sayılan/farkı sunucudan okuyor; fark yalnız ayrı onayla
  kayda dönüşüyor. POS ekranı brüt, komisyon ve neti ayrı gösteriyor; yoldaki
  tahsilat görünür onayla hesaba geçmiş işaretleniyor.
- Loading, empty, error, unauthorized ve stale-cache durumları görünür;
  controller yaşam döngüsü shell yeniden çizimlerinden bağımsız tutuluyor.
- `FinancialDataChanges` sayım gözlemini yalnız kasa ekranına, fark onayını
  finansal yüzeylere, POS tanıma ve geçişini farklı hedeflere yayıyor. Feed,
  iki yeni kaynağı Grup 8'de öğrenene kadar bilerek yükseltilmiyor.
- Repository sözleşmesi, mutation hedefleri, iki profil gezinmesi ve iki alt
  ekranın 2× yazı ölçeği/erişilebilirlik kapısı Flutter testleriyle korunuyor.

### Grup 8 — Yedek v9 ve sözleşme belgeleri

- Şema **v9** yazar, yalnız v9 okur.
- Feed sözleşmesi iki yeni kaynakla genişletilir.
- `documentation/design-system.md` yeni bileşenlerle güncellenir.
- Ölçüt: sayımlar ve tahsilatlar kayıpsız geri yükleniyor.

## Zorunlu testler

### Domain / Application

- Kasa farkı otomatik finansal kayıt üretmez; onaylanınca tek kayıt üretir.
- Aynı gün ikinci sayım öncekini iptal eder.
- POS tahsilatı geliri tanır, hesabı değiştirmez; geçiş hesabı değiştirir,
  geliri **tekrar tanımaz**.
- Komisyon brüt tutara eklenmez; ayrı gider olarak görünür.
- Yoldaki tutar projection'dır; kalıcı kolon yok.

### API ve gerçek SQL

- Kullanıcı izolasyonu; pozitif **ve** negatif senaryo.
- Kullanılabilir bakiye ile net varlık farkı = yoldaki tutar.
- Feed ve planlanan görünüm tek sorguda kalır.
- Yedek v9 yazma/okuma; v8 reddi.

### Flutter

- Gün sonu akışı: sayım gir, farkı gör, onayla veya onaylama.
- POS tahsilatı gir, geçişi işaretle; bakiyenin ne zaman değiştiğini doğrula.
- Sekme yapısı iki profilde de doğru.
- `İşlem ekle` menüsünün yeni yerleşimi ve erişilebilirliği.

## Kalite komutları

```bash
dotnet build BusinessFinance.slnx --configuration Release --no-restore
dotnet test BusinessFinance.slnx --configuration Release --no-restore
dotnet format BusinessFinance.slnx --verify-no-changes --no-restore
```

```bash
flutter analyze
flutter test
dart format --set-exit-if-changed lib test
flutter build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5284
```

## Belge güncellemeleri

- Yeni ADR: kart borcu ile kart tahsilatı ayrımı, sekme kararı
- `documentation/architecture.md`: kasa, POS ve bloke projection'ı
- `documentation/flows.md`: gün sonu, POS tahsilatı ve geçiş akışları
- `documentation/permissions.md`: yeni endpoint'ler
- `documentation/design-system.md`: yeni bileşenler ve gezinme
- `documentation/financial-activity-api-contract.md`, `tests.md`,
  `restore-runbook.md`
- `docs/project-status.md`: yalnız doğrulanmış checkpoint'ler

## Güvenlik ve veri sınırları

- Bütün sorgular current-user kapsamlıdır.
- Yalnız sentetik veri kullanılır; gerçek POS verisi girilmez.
- Banka veya POS sağlayıcısına bağlantı kurulmaz.

## Riskler ve azaltımlar

| Risk | Azaltım |
|---|---|
| POS tahsilatının satışı iki kez saydırması | Tanıma/geçiş ayrımı testi, çıkış koşulu |
| Yoldaki paranın kullanılabilir bakiyeye karışması | İki sayının farkını doğrulayan test |
| Kasa farkının sessizce finansal kayda dönüşmesi | Onaysız kayıt üretilmediğini doğrulayan test |
| Sekme değişikliğinin bir ekranı erişilemez bırakması | İki profilde de bütün ekranların erişilebilirliği testi |
| Menü yeniden kurgusunun mevcut akışları kırması | Tek launcher kuralı korunur; yönlendirme testleri |
| "Kredi kartı" kelimesinin iki yerde birden kalması | ADR'de adlandırma kararı; arayüz metni testi |

## Çıkış koşulları

- [ ] ADR yazıldı ve kabul edildi.
- [ ] Bütün çalışma grupları tamamlandı.
- [ ] Backend build, test ve format kontrolleri geçti.
- [ ] Flutter analyze, test, format ve debug build kontrolleri geçti.
- [ ] Kullanıcı izolasyonu negatif senaryolarla kanıtlandı.
- [ ] Bir günlük perakende senaryosu girildi: gün sonu farkı doğru, POS parası
      geçene kadar kullanılabilir bakiye şişmiyor, komisyon gider olarak
      görünüyor.
- [ ] İki profilde de dört sekme dolu ve hiçbir ekran erişilemez değil.
- [ ] `documentation/` ve `docs/project-status.md` güncel.
- [ ] Kullanıcı Aşama 05'i açıkça onayladı.

## Tamamlanma kaydı

Aşama kapandığında burada: hangi commit'lerle bitti, hangi kontroller geçti,
belge `docs/archive/stages/` altına taşındı mı.
