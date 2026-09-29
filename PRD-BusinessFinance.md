# BusinessFinance — Ürün Planı

## 1. Özet

Bu ürün; **şahıs şirketi sahibinin ve esnafın** işletme ve şahsi finansını tek
mobil uygulamada takip etmesini sağlar. Gelir, gider, nakit ve banka hesapları,
kredi kartları, bütçeler, borç ve alacaklar, müşteri/tedarikçi cari hesapları,
faturalar, günlük kasa ve gün sonu, POS tahsilatları ve vergi ödemeleri aynı
yerde durur.

Ürün bir **işletme bütçe uygulamasıdır**; muhasebe ya da ön muhasebe programı
değildir.

Şahıs şirketinin tüzel kişiliği yoktur: işletmenin kasası ile sahibinin cebi
**aynı ceptir**. Ürün bu gerçeği modelin merkezine koyar — işletme ve şahsi
ayrımı tek havuz üzerinde bir **raporlama boyutudur**, ayrı bir veri alanı veya
ayrı bir uygulama modu değildir. Gerekçesi ve reddedilen alternatifler
`documentation/adr/0013-business-and-personal-are-one-pool.md` belgesindedir.

Backend ASP.NET Core ve C# ile, mobil istemci Flutter ve Dart ile geliştirilir.

Bu belge ürünün **ne olacağını** anlatır. Hangi sırayla geliştirileceği
`PROJECT-ROADMAP.md`, o sıradaki somut işler `stages/` belgelerindedir.

## 2. Arka plan ve amaç

Küçük işletmenin finansal verisi banka uygulamaları, kart ekstreleri, fiş
defterleri, elektronik tablolar ve notlar arasında dağılır. Bu dağınıklık şu
soruların tek yerden cevaplanmasını engeller:

- İşletmem bu ay ne kazandı — şahsi harcamam karışmadan?
- Kasada ve hesaplarda toplam ne var?
- Hangi müşteriden ne alacağım, hangi tedarikçiye ne borcum var?
- Hangi fatura ne zaman ödenecek, hangisi gecikti?
- Kredi kartında ne kadar güncel borç ve gelecek taksit var?
- Bugün ne sattım, kasa tuttu mu, kartla satışın parası bankaya ne zaman
  gelecek?
- Hangi vergiyi ne zaman ödeyeceğim, bu yıl ne ödedim?

Amaç, bu sorulara güvenilir cevap veren bir takip ürünü oluşturmak.

### Ürün sınırı: bütçe uygulamasıdır, muhasebe yerine geçmez

Ürün bir **işletme bütçe uygulamasıdır**; muhasebe ya da ön muhasebe programı
değildir. **Beyanname üretmez, vergi hesaplamaz ve muhasebeciye veri paketi
hazırlamaz.** Yaptığı iş, işletmenin parasal hareketlerini eksiksiz ve doğru
biçimde toplayıp sahibine "param nerede, ne kazandım, ne ödeyeceğim"
sorularının cevabını vermektir. Bu sınır bilinçlidir: yanlış hesaplanmış bir
vergi rakamı kullanıcıyı cezaya sokar ve bu sorumluluk bir takip uygulamasının
üstlenebileceği bir şey değildir.

**Vergi bir nakit planıdır** (ADR 0018): vergi ve SGK ödemeleri kendi
ekranında tanımlanan, ödendiği gün hesaptan düşen planlardır. Tutarı kullanıcı
girer; uygulama hiçbir vergi tutarını türetmez.

Aynı gerekçeyle ürün **muhasebe kârı** hesaplamaz. Hesapladığı şey nakit esaslı
**işletme netidir**: işletme geliri eksi işletme gideri. Gerçek kâr satılan
malın maliyetini ve stok takibini gerektirir; ikisi de kapsam dışıdır. Bu yüzden
arayüzde "kâr" kelimesi kullanılmaz.

### Ürün hedefleri

- İşletme ve şahsi finansı tek yerde toplamak, ikisini ayrıştırılabilir tutmak.
- Bakiye ve raporların aynı finansal hareketlerden hesaplanmasını sağlamak.
- Kullanıcıların verilerini birbirinden güvenli biçimde ayırmak.
- Esnafın günlük ritmini (gün sonu, kasa, POS'un bankaya geçişi) az dokunuşla
  karşılamak.
- Vergi ve SGK ödemelerini nakit planının parçası olarak takip etmek.
- Android ile başlayıp Google Play'e, sonra iOS'a ilerlemek.
- Sürümler arasında veri kaybetmeden ilerlemek.

### Başarı ölçütleri

- İşletme neti, şahsi harcamadan etkilenmez.
- Kasa bakiyesi hangi kapsamda bakılırsa bakılsın aynı toplamı gösterir.
- İki farklı kullanıcı birbirinin hiçbir finansal verisine erişemez.
- Tekrarlanan API isteği çift finansal kayıt oluşturmaz.
- Aynı harcama iki kez sayılmaz (transfer, kart ödemesi, taksit).
- Aynı satış iki kez gelir sayılmaz (gün sonu, kartla tahsilat, POS yatışı).
- Yedekleme ve geri yükleme gerçek finansal veri kullanılmadan önce denenir.

## 3. Hedef kullanıcı

**İşini kendi yürüten şahıs şirketi sahibi ve esnaf.** Muhasebe personeli
yoktur, kayıtları kendi tutar; vergi işlerini çoğu zaman bir muhasebeciyle
yürütür.

İki tipik profil ve ikisi de kapsamdadır:

- **Tezgâh üstü işletme** — bakkal, kafe, berber, market. Günlük ritmi nakit
  satış, POS tahsilatı ve gün sonu kasa sayımı.
- **Faturalı iş** — tamirci, müteahhit, toptancı, serbest çalışan. Günlük ritmi
  müşteri/tedarikçi cari hesabı ve vadeli tahsilat.

İkisinin de ortak yanı: işletme gideri ile şahsi gider aynı cepten çıkar. Ürün
ikisini ayrı iki dünya gibi kurgulamaz.

Personel rolleri, onay akışları, bordro ve stok ana kapsamda değildir.

## 4. Ürün ilkeleri

- **İşletme ve şahsi tek havuzda yaşar**; ayrım bir boyuttur, sınır değil.
- **Bakiye, kart borcu ve net varlık bölünmez.** Kapsam filtresi bunlara
  uygulanmaz.
- Finansal hesapların doğruluğu görsel zenginlikten önce gelir.
- Para değerlerinde backend tarafında `decimal` kullanılır; API'de kayıpsız
  ondalık metin taşınır.
- İlk sürümde ana para birimi TRY'dir; para birimi alanları baştan tutulur.
- Mobil istemci finansal iş kurallarının tek kaynağı olmaz.
- Kullanıcı kimliği istek gövdesinden değil güvenli oturumdan belirlenir.
- Hesap bakiyesi ikinci bir gerçek kaynak olarak elle saklanmaz.
- Hesap, kart ve kategori geçmişi fiziksel silmeyle anlamsız hale getirilmez.
- Kategori bir raporlama kovasıdır; kimlik taşımaz. Karşı taraf, kapsam ve
  ödeme yöntemi kategoriyle temsil edilmez.
- Gerçek finansal veri kullanılmadan önce güvenlik ve geri yükleme doğrulanır.
- Mikroservis yerine küçük, katmanlı monolit kullanılır.
- Ürün vergi hesaplamaz ve beyanname üretmez.
- Ürün bir bütçe uygulamasıdır; ön muhasebe özelliği (muhasebeci paketi, KDV
  takibi, fatura kesme) eklenmez.
- Satışlar gün sonu toplamıyla girilebilir; tek tek giriş de mümkündür. Hiçbir
  akış fotoğrafa ya da POS'a bağlı değildir.

## 5. Ürünün bugünkü hâli

Kod tabanı, kişisel bütçe ürünü olarak geliştirilmiş ve testlerle korunan bir
temelden devralındı. Aşağıdakiler **çalışır durumdadır**; kapsam bölümü bunların
üstüne ne ekleneceğini anlatır.

| Alan | Durum |
|---|---|
| Kimlik ve oturum | JWT access + refresh rotation, reuse tespiti, rate limit |
| Kullanıcı izolasyonu | Owner-scoped composite anahtar; Application ve SQL seviyesinde iki kapı |
| Hesaplar | Nakit/banka, açılış bakiyesi, hareketlerden hesaplanan bakiye, transfer |
| Gelir ve gider | Kategori, tarih, açıklama, iptal modeli, filtreleme |
| Bütçeler | Kategori bazlı aylık bütçe ve ilerleme |
| Kredi kartı | Limit, ekstre projeksiyonu, harcama, ödeme, asgari ödeme, taksit planı |
| Tekrarlayan planlar | Hesap veya kart kaynaklı plan, idempotent occurrence, gerçekleştirme |
| Borç ve alacak | Anüite faiz modeli, açılış kaynağı, anapara/faiz ayrımı |
| Hedefler | Manuel ve bakiye izleyen tasarruf hedefleri |
| Birleşik akış | Gerçekleşmiş feed ve planlanan görünüm, tek SQL sorgusunda |
| Veri taşınabilirliği | CSV içe/dışa aktarma, idempotency, yedekleme ve geri yükleme |
| Belge ve fiş | Ek saklama; fiş, fatura ve dekont fotoğrafından öneri üretme |
| Tasarım sistemi | Token'lar, pencere sınıfları, ortak bileşenler, erişilebilirlik kapısı |

Eksik olan, ürünün **kimliğidir**: uygulama bir kaydın işletmeye mi sahibinin
cebine mi ait olduğunu bilmiyor ve yeni kullanıcıyı ev bütçesi kategorileriyle
karşılıyor.

## 6. Ürün kapsamı

### 6.1 Kimlik ve hesap güvenliği

- Kayıt olma ve giriş yapma
- Access token ve yenilenen refresh token
- Kullanıcı verilerinin sahiplik bilgisiyle ayrılması
- Oturum kapatma ve aktif oturumları sonlandırma
- Hatalı giriş ve istek hız sınırları
- Mobil cihazda güvenli token saklama
- Bulut aşamasında e-posta doğrulama ve parola sıfırlama
- İleri aşamada biyometrik uygulama kilidi

### 6.2 İşletme/şahsi kapsam

- Her finansal kayıtta kapsam: işletme veya şahsi
- Hesap, kart ve kategoride isteğe bağlı varsayılan kapsam
- Kapsam türetme zinciri: kullanıcının seçimi → hesap/kart → kategori
- Uygulama genelinde tek kapsam anahtarı: `Hepsi · İşletme · Şahsi`
- Kayıt sırasında tek onboarding sorusu; özellik kapatmaz, ön ayar yapar
- Bakiye, kart borcu ve net varlığın kapsamdan bağımsız kalması

### 6.3 Finansal hesaplar

- Nakit ve banka hesapları
- Açılış bakiyesi ve hesap para birimi
- Aktif/pasif hesap durumu
- Hesaplar arası transfer
- Hareketlerden hesaplanan bakiye
- POS parasının bankaya geçişinin gerçek yatan tutarla eşleştirilmesi (yatış);
  banka hesabı için ayrı bir sayım/mutabakat **yoktur** — kullanıcının bütün
  hareketleri eksiksiz girmesini varsayar

### 6.4 Gelir, gider ve kategoriler

- Gelir ve gider işlemleri; tarih, tutar, açıklama, kapsam
- İşletme ve kişisel varsayılan kategori setleri
- Hesap, kategori, tür, kapsam ve tarih filtreleri
- Arama ve sıralama
- Planlanan ve gerçekleşen işlem ayrımı
- İşlem iptali ve düzeltme (silme yerine iptal)

### 6.5 Kredi kartları ve taksitler

- Kart limiti ve kullanılabilir limit
- Hesap kesim ve son ödeme günü
- Güncel dönem ve ekstre borcu
- Kart harcamaları ve taksit planı
- Kart borcu ödemesi ve ikinci kez gider sayılmaması
- Geciken ödeme ve yaklaşan ekstre uyarıları

### 6.6 Müşteri, tedarikçi ve cari hesap

- Karşı taraf kaydı (müşteri/tedarikçi) — kendi boyutu, kategori değil
- Açık cari hesap: birden çok belge, kısmi tahsilat, yürüyen bakiye
- Sözleşmeli ve taksitli borç/alacak (anapara, faiz, ödeme planı)
- Tahsilat ve ödemenin hesap hareketiyle ilişkilendirilmesi
- Gecikmiş bakiye durumu
- Karşı taraf bazlı geçmiş ve toplam

### 6.7 Fatura, vade ve yükümlülükler

- Tek seferlik yükümlülük kaydı (ödenmemiş fatura)
- Vade tarihi, ödendi/ödenmedi durumu, gecikme
- Tekrarlayan gelir, gider ve abonelikler; "seçilen aylarda" sıklığı
- Tekrarlayan planda bitiş sınırı (bitiş tarihi veya tekrar sayısı)
- Yaklaşan ödeme takvimi ve gecikenler
- Fiş/fatura okumanın "henüz ödemedim" yoluna bağlanması

### 6.8 Kasa, gün sonu ve POS

Kararlar: ADR 0019 (ilkeler); ayrıntı ve başlangıç tasarımı ADR 0019 ile
`research/kasa-pos-gun-sonu/KAPANIS.md`'dedir.

- **Gün sonu:** günün nakit ve kartlı satışı tek adımda, elle ya da Z raporu
  fotoğrafından; var olan kayıtları üretir, aynı satışı iki kez saymaz ve
  düzeltilebilir
- **Kasa sayımı** ve fark; kasanın tek gerçeği hesap bakiyesidir
- **POS tanımı** (yemek kartı dahil) ile az dokunuşlu kartlı satış girişi
- POS tahsilatı: tahsilat, yolda, hesaba geçiş; komisyon ayrı gider
- **Yatış:** POS parasının bankaya gerçek yatan tutarla geçişi ve kesinti;
  düzeltilebilir
- **Kartla tahsil:** veresiye ve alacağın kartla kapatılması gelir yazmaz, para
  yola çıkar
- Tahsil edilen ama henüz hesaba geçmemiş paranın kullanılabilir bakiyeyi
  şişirmemesi; beklenen tahsilatın görünmesi

### 6.9 Vergi ve SGK ödemeleri

Kararlar: ADR 0018 (ilkeler); ayrıntı ve başlangıç tasarımı ADR 0018 ile
`research/vergi/YENI-YAKLASIM.md` §6.6'dadır.

- **Vergi ekranı:** tanımlı vergiler, sıradaki tarihler, gecikenler ve
  ödenenler; tanımlı vergi bir tekrarlayan plandır ve Yaklaşanlar'da görünür
- **Toplu vergi ödemesi:** kullanıcı hiçbir vergiyi tanımlamadan, ödediği vergiyi
  tek tutarla yazabilir; tanımlı vergi yalnız hatırlatma içindir
- Tutarı bilinmeyen vergi meşrudur; tutar ödemede girilir; ödeme düzeltilebilir
- Hazır vergi türleri tarih ve sıklık önerir; öneri kurulduğu an kullanıcının
  verisidir
- Vergi karşılığı için para ayırma (mevcut hedef mekanizmasıyla)
- **Yok:** KDV alanları, indirilebilirlik, muhasebeci paketi (ADR 0018 ile
  kaldırıldı)

### 6.10 Bütçeler ve hedefler

- Kategori bazlı aylık bütçe; harcanan, kalan ve aşılan tutar
- Bütçenin hangi kapsamı sınırladığının açık olması
- Tasarruf ve karşılık hedefleri
- Hedefe ayrılan ve kalan tutar

### 6.11 Raporlar

- Aylık gelir, gider ve işletme neti
- Kapsam kırılımı: işletme neti ve şahsi çekim
- Kategori bazlı harcama dağılımı
- Hesap ve kart bakiyeleri, net varlık
- Önceki dönem karşılaştırması
- Bütçe kullanım ve aşım raporu
- Gelecek taksit ve ödeme yükü
- Nakit akışı eğilimi
- Cari bakiye özeti

### 6.12 Belge, fiş ve veri taşınabilirliği

- Fiş, fatura ve dekont fotoğrafından öneri üretme (öneri katmanı; yönü ve
  ödeme kaynağını model seçmez)
- Z raporu ve banka gün sonu fotoğrafından gün sonu önerisi (öneri katmanı;
  okunan alanlar ADR 0019'un başlangıç tasarımındadır)
- Fiş veya belge ekinin kaydın yanında saklanması
- CSV ekstre içe aktarma, kolon eşleştirme, tekrar kayıt tespiti
- İçe aktarma ön izlemesi, onayı ve hatalı satır raporu
- CSV dışa aktarma
- Uygulama yedeği ve geri yükleme

### 6.13 Offline çalışma

- Online-first çalışma
- Son başarılı verilerin okunabilir cache'i
- Cache zamanının ve bağlantı durumunun gösterilmesi
- Kullanıcı çıkışında yerel finans verisinin güvenli temizlenmesi

Offline **yazma** kapsam dışıdır. Kasa ve POS bulutla birleştiğinde bu karar
yeniden değerlendirilir: dükkânda internet kesikken satış kaydedilemiyorsa
uygulama o an işe yaramaz.

### 6.14 Yatırım ve çoklu para birimi

- Manuel yatırım hesabı ve varlık kaydı
- Alış maliyeti, miktar ve gerçekleşmemiş kazanç/kayıp
- Daha sonra piyasa fiyat sağlayıcısı
- Dövizli hesap ve işlem; kur anlık görüntüsü ve kur geçmişi
- Kur sağlayıcısı kesintisinde son bilinen değer uyarısı

## 7. Temel kullanıcı deneyimi

### Ana navigasyon

Material 3 tabanlı mobil uygulamada dört ana bölüm bulunur. Ana sekme yapısı,
gün sonu kasa ekranı geldiğinde yeniden değerlendirilir (`PROJECT-ROADMAP.md`,
Aşama 04): bugünkü `Bütçeler` sekmesi bir ev bütçesi kavramıdır ve işletme
kullanıcısının ikinci ekranı büyük ihtimalle kasadır.

Hızlı işlem ekleme belirgin bir ana eylem düğmesiyle açılır. Bu menü düz bir
seçenek listesi olarak büyümez; niyet ekseninde gruplanır.

### Temel akışlar

1. Kullanıcı kayıt olur, "işletmeniz var mı?" sorusuna cevap verir ve uygun
   kategori setiyle başlar.
2. Nakit veya banka hesabı oluşturur; isterse hesabı işletme veya şahsi olarak
   etiketler.
3. İşlem ekler; kapsam varsayılan zincirinden dolu gelir, gerekirse düzeltir.
4. Bakiye, bütçe ve raporlar güncellenir.
5. Özet ekranında aylık durum, yaklaşan ödemeler ve gecikenler görünür.
6. İşletme profilinde akşam gün sonunu girer; kartla satışın parası bankaya
   geçince yatışı işaretler.
7. Ödediği vergiyi vergi ekranından yazar — tanımlı bir vergiyi "ödedim" diye
   kapatarak ya da hiçbir şey tanımlamadan tek tutarla.

### Zorunlu ekran durumları

- Yükleniyor
- Boş sonuç
- Doğrulama hatası
- Ağ veya sunucu hatası
- Yetkisiz/oturumu bitmiş kullanıcı
- Eski cache verisi
- Başarılı kayıt veya güncelleme

## 8. Teknik mimari

```text
Flutter Android/iOS uygulaması
            │  HTTPS + JSON
            ▼
ASP.NET Core Web API
            ├── Application
            ├── Domain
            └── Infrastructure
                    │
                    ▼
                SQL Server
```

### Backend katmanları

- `BusinessFinance.Domain`: Entity, value object ve değişmez iş kuralları
- `BusinessFinance.Application`: Kullanım senaryoları ve port'lar
- `BusinessFinance.Infrastructure`: EF Core, Identity, dosya, dış entegrasyonlar
- `BusinessFinance.Api`: HTTP endpoint'leri, authentication, hata cevapları
- Unit ve integration test projeleri

Domain katmanı EF Core, HTTP, Flutter veya dış sağlayıcı bağımlılığı taşımaz.
Genel amaçlı repository veya CQRS/MediatR altyapısı somut ihtiyaç olmadan
eklenmez.

### Flutter yapısı

- Material 3, özellik bazlı klasörler
- View, Controller/ViewModel, Repository ve Service ayrımı
- `ChangeNotifier` ve `provider`; `go_router` ile navigasyon
- `http` ile API erişimi, `flutter_secure_storage` ile oturum bilgisi
- Grafikler için `fl_chart`
- Offline cache aşamasında `drift`

Flutter doğrulamaları kullanıcı deneyimini iyileştirir; finansal kuralları tek
başına korumaz.

## 9. Kavramsal domain modeli

### Kimlik

`User`, `RefreshSession`, `DeviceRegistration`, `AuditEntry`

### Temel finans

`Account`, `Category`, `BudgetTransaction`, `Transfer`, `MonthlyBudget`,
`TransactionScope`

### Kart ve planlama

`CreditCard`, `CreditCardCharge`, `CreditCardPayment`, `CreditCardStatement`,
`InstallmentPlan`, `RecurringTransaction`, `SavingsGoal`

### İşletme

`Counterparty`, `CounterpartyLedger`, `DebtAgreement`, `Obligation` (tek
seferlik yükümlülük/fatura), `CashCount` (kasa sayımı), `PosSettlement`,
POS tanımı, POS yatışı, gün sonu bağlayıcı kimliği; vergi kalemi vergi türü
taşıyan bir `RecurringTransaction`'dır (ayrı kayıt türü değil)

### Veri ve belge

`ImportBatch`, `ImportRow`, `Attachment`

### Gelişmiş finans

`Currency`, `ExchangeRateSnapshot`, `InvestmentAccount`, `Asset`, `Holding`,
`PriceSnapshot`

Her entity için sahiplik, tarihçe, pasifleştirme ve eşzamanlı güncelleme
ihtiyacı ilgili geliştirme aşamasında açıkça değerlendirilir. Model adları
kavramsaldır; nihai şekli aşama belgesinde kararlaştırılır.

## 10. API ve veri sözleşmesi ilkeleri

- API tabanı `/api/v1` olur.
- Kullanıcı kimliği JWT claim'den belirlenir; istek gövdesinden alınmaz.
- Para, kayıpsız ondalık metin ve ISO 4217 para koduyla taşınır.
- İş tarihleri `yyyy-MM-dd`, sistem zaman damgaları UTC ISO 8601 olur.
- Hatalar standart `ProblemDetails` cevabı kullanır.
- Liste endpoint'leri sayfalama, filtreleme ve sıralama destekler.
- Açılış bakiyesi ve hareketler, hesap bakiyesinin tek kaynağıdır.
- Finansal kayıtların iptal/düzeltme geçmişi kaybolmaz.
- API kararlı makine değerleri gönderir; kullanıcıya gösterilecek cümleyi
  istemci üretir.

### Endpoint grupları

`/api/v1/auth`, `/accounts`, `/categories`, `/transactions`, `/transfers`,
`/budgets`, `/credit-cards`, `/recurring-transactions`, `/financial-activities`,
`/counterparties`, `/obligations`, `/debts`, `/goals`, `/cash-counts`,
`/pos-settlements`, `/reports`, `/imports`, `/exports`, `/receipts`

## 11. Veri güvenliği ve gizlilik

- ASP.NET Core Identity tabanlı kullanıcı yönetimi
- Kısa ömürlü access token ve döndürülen refresh token
- Her sorgu ve komutta kullanıcı sahipliği kontrolü; kapsam filtresi bunun
  yerine geçmez
- Kullanıcılar arası erişimi reddeden negatif testler
- Secret değerlerinin yalnız sunucuda tutulması
- Mobil uygulamaya connection string veya özel anahtar konulmaması
- HTTPS zorunluluğu, rate limiting ve brute-force koruması
- Hassas finansal veriyi loglamama
- Dosya eklerinde tür, boyut ve zararlı içerik doğrulaması
- Yedek şifreleme ve geri yükleme testi
- Hesap kapatma, veri dışa aktarma ve veri silme akışı
- Bulut/public beta öncesinde KVKK ve mağaza gizlilik metni incelemesi

## 12. Gerekli yazılımlar ve servisler

### Yerel geliştirme

Visual Studio Community 2026, .NET 10 SDK, ASP.NET Core 10, EF Core 10,
ASP.NET Core Identity, Git ve GitHub, Docker Desktop ve Compose, SQL Server
container, SSMS 22, Flutter stable SDK ve Dart, Android Studio, Android SDK ve
Emulator, OpenAPI ve `.http` dosyaları, xUnit, Flutter test araçları.

Node.js, React, TypeScript ve Vite bu ürünün teknoloji zincirinde bulunmaz.

### Bulut ve yayın aşaması

Azure CLI, App Service for Containers, Azure SQL Database, Key Vault, Blob
Storage, Application Insights ve OpenTelemetry, GitHub Actions, Firebase Cloud
Messaging, Google Play Console, Android upload keystore; iOS aşamasında Mac,
Xcode ve Apple Developer hesabı.

Bulut altyapısı kendi aşaması gelmeden kurulmaz.

## 13. Ortamlar ve dağıtım modeli

### Local development

API yerel makinede, SQL Server Docker container içinde, Flutter Android
emülatörde. Yalnız sentetik veri. İnternete açık port oluşturulmaz.

### Cloud beta

API container olarak Azure'da, Azure SQL Database, secret'lar Key Vault'ta,
log ve metrik Application Insights'ta. Google Play kapalı test kullanıcılarıyla
doğrulama.

### Production

Geri alınabilir dağıtım, migration kontrolü, otomatik ve doğrulanmış yedekleme,
güvenlik ve performans kontrolleri. Google Play production; daha sonra iOS.

## 14. Sürüm yol haritası

Aşama zinciri, her aşamanın ana çıktısı ve bağımlılık kuralları
`PROJECT-ROADMAP.md` belgesindedir. Aşama sırası ve kapsamı yalnız kullanıcı
kararıyla değişir; bu belge sürüm numarası veya süre tahmini taşımaz.

## 15. Test ve kalite planı

### Backend

- Domain kuralları için unit testler
- Kimlik ve kullanıcı izolasyonu için negatif testler
- Gerçek SQL Server ile integration testleri
- API sözleşme ve `ProblemDetails` testleri
- Migration yükseltme yolu kontrolleri
- Import ve yazma idempotency testleri

### Flutter

- Controller ve repository unit testleri
- Form, yükleniyor, boş ve hata widget testleri
- Authentication ve işlem ekleme integration testleri
- Farklı ekran boyutu ve metin ölçeklendirme testleri

### Sistem

- Uçtan uca kullanıcı akışları
- Yedekleme ve geri yükleme tatbikatı
- Secret ve hassas veri kontrolü
- Yetkisiz veri erişimi testleri
- Performans ve sayfalama testleri
- Google Play öncesi erişilebilirlik ve release build kontrolü

### Ana branch kalite kapıları

Backend restore/build/test/format, Flutter dependency/analyze/test, Android
debug ve release build, secret taraması, migration ve API sözleşme incelemesi.

## 16. Riskler ve karar kapıları

### Kapsam etiketinin yakalanamaması

Risk: Kullanıcı kapsamı işaretlemezse işletme neti anlamsızlaşır.

Karar kapısı: Kapsam varsayılan zincirden dolu gelir ve onay ekranında görünür.
Yaygın durumda kullanıcıdan ek dokunuş istenmez.

### Vergi bilgisinin hesaplamaya ya da danışmanlığa dönüşmesi

Risk: Yanlış hesaplanmış bir vergi rakamı ya da yanlış bir ipucu kullanıcıyı
cezaya sokar.

Karar kapısı: Uygulama hiçbir vergi tutarını türetmez; tutar kullanıcıdan gelir.
Hazır vergi türlerinin tarih ve sıklığı bir öneridir, kurulduğu an kullanıcının
verisidir (ADR 0016 §1, ADR 0018). İpuçları "genelde" diliyle yazılır ve
kullanıcıyı muhasebecisine yönlendirir.

### Aynı satışın iki kez sayılması

Risk: Gün sonu toplamı, gün içinde tek tek girilmiş satışı, kartla veresiye
tahsilatını ya da faturalı satışı ikinci kez gelir yazar.

Karar kapısı: Gün sonu o günün zaten girilmiş kayıtlarını hesaba katar ve
kullanıcıya gösterir (ADR 0019). Fotoğraftan okuma, varsayılanları gerçek Z
örnekleriyle doğrulanmadan kullanıma açılmaz.

### Bulut ve gerçek veri

Risk: İnternete açık finansal veri, secret sızıntısı, geri yüklenemeyen yedek.

Karar kapısı: HTTPS, kullanıcı izolasyonu, rate limiting, yedekleme ve geri
yükleme doğrulanmadan gerçek finansal veri kullanılmaz.

### Offline yazma ihtiyacı

Risk: Dükkânda bağlantı kesikken satış kaydedilemezse uygulama o an işe yaramaz.

Karar kapısı: Kasa/POS bulutla birleştiğinde offline yazma kararı yeniden
değerlendirilir. Online ürün kararlı olmadan offline yazma açılmaz.

### Piyasa fiyatları

Risk: Lisans, gecikmeli veri, sağlayıcı kesintisi ve yanlış değerleme.

Karar kapısı: Manuel portföy kararlı olmadan fiyat API'si eklenmez; fiyatın
hangi zamana ait olduğu kullanıcıya gösterilir.

### Kapsam büyümesi

Risk: İlk kullanılabilir sürümün sürekli ertelenmesi.

Karar kapısı: Bir aşamanın çıkış koşulları tamamlanmadan sonraki aşamanın
framework veya servisleri eklenmez.

## 17. Kapsam dışında

- **Read-only açık bankacılık ve banka bağlantısı.** Sağlayıcı, yetkilendirme
  ve sözleşme süreçleri ürünün kontrolü dışında; CSV içe aktarma aynı ihtiyacın
  çalışan karşılığı. Bankadan ödeme veya transfer başlatma hiçbir koşulda
  eklenmez.
- **Tam offline senkronizasyon** (kuyruk, conflict çözümü) — bkz. 6.13.
- **Personel ve bordro.**
- **Stok ve satılan malın maliyeti**; dolayısıyla muhasebe kârı.
- **Muhasebe ve vergi beyannamesi.**
- **Ön muhasebe:** muhasebeciye veri paketi, KDV takibi, fatura kesme, satış
  satış kayıt ve adisyon.
- **Banka hesabı sayımı / bakiye mutabakatı.** Kullanıcının bütün hareketleri
  eksiksiz girmesini varsayar; pratikte olmaz.
- LTD/AŞ ayrımı, ortak cari hesabı, çok kullanıcılı işletme ve onay akışları.
- Hane ve ortak bütçe.
- Kripto cüzdan anahtarı saklama; hisse veya kripto alım-satım emri verme.
- Otomatik finansal danışmanlık.
- Yapay zekânın kullanıcı onayı olmadan finansal kayıt değiştirmesi.

## 18. Resmî referanslar

- [Flutter uygulama mimarisi](https://docs.flutter.dev/app-architecture/guide)
- [Flutter mimari önerileri](https://docs.flutter.dev/app-architecture/recommendations)
- [Flutter Material Design](https://docs.flutter.dev/ui/design/material)
- [Flutter Android kurulumu](https://docs.flutter.dev/platform-integration/android/setup)
- [Flutter test yaklaşımı](https://docs.flutter.dev/testing/overview)
- [Flutter Android yayınlama](https://docs.flutter.dev/deployment/android)
- [Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/overview)
- [Azure SQL Database](https://learn.microsoft.com/en-us/azure/azure-sql/database/sql-database-paas-overview)
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview)
- [Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
