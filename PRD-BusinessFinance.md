# BusinessFinance — Ürün ve Proje Planı

## 1. Özet

Bu proje; **şahıs şirketi ve esnafın** gelirlerini, giderlerini, nakit ve banka
hesaplarını, kredi kartlarını, bütçelerini, borç ve alacaklarını, hedeflerini ve
ileride yatırım varlıklarını tek mobil uygulamada takip etmeyi amaçlar.

Şahıs şirketinde işletmenin kasası ile sahibinin cebi hukuken ayrılmaz; bu
yüzden ürün gündelik gider takibini işletme takibinden ayırmaz, ikisini aynı
finansal tabloda tutar ve gerektiğinde ayrıştırılabilir kılar.

Backend ASP.NET Core ve C# ile, mobil istemci Flutter ve Dart ile geliştirilir.
İlk çalışan sürüm küçük tutulur; ancak veri modeli ve güvenlik temeli sonraki
sürümlerde kredi kartı, offline senkronizasyon, bulut yayını ve read-only banka
bağlantısı eklenebilecek şekilde tasarlanır.

Bu belge ürünün kapsamını, mimarisini, gerekli araçları ve sürüm basamaklarını
tanımlar. Öğrenme yöntemi, görev paylaşımı ve günlük ilerleme düzeni ayrı
belgelerde ele alınacaktır.

## 2. Arka plan ve amaç

Küçük işletmenin finansal verisi çoğu zaman banka uygulamaları, kart
ekstreleri, fiş defterleri, elektronik tablolar ve notlar arasında dağılır. Bu
durum aşağıdaki soruların tek yerden cevaplanmasını zorlaştırır:

- Toplam kullanılabilir varlık ne kadar?
- Belirli bir ayda ne kadar gelir ve gider oluştu?
- Hangi kategori bütçeyi aştı?
- Kredi kartında ne kadar güncel borç ve gelecek taksit bulunuyor?
- Yaklaşan faturalar, kart ödemeleri ve borçlar neler?
- Tasarruf hedeflerine ne kadar yaklaşıldı?
- Banka ekstresindeki bir hareket daha önce kaydedildi mi?

Projenin amacı, önce bu sorulara güvenilir cevap veren bir temel takip ürünü
oluşturmak; ardından ürünü küçük işletmenin gündelik finans sistemine
dönüştürmektir.

### Ürün sınırı: muhasebe yerine geçmez

Ürün **beyanname üretmez ve vergi hesaplamaz.** Yaptığı iş, işletmenin parasal
hareketlerini eksiksiz ve doğru biçimde toplayıp muhasebeciye giden veriyi
hazırlamaktır. Bu sınır bilinçlidir: yanlış hesaplanmış bir vergi rakamı
kullanıcıyı cezaya sokar ve bu sorumluluk bir takip uygulamasının
üstlenebileceği bir şey değildir. Vergiye dair alanlar (ör. oran bilgisi)
ileride eklenirse, hesaplayan değil **taşıyan ve raporlayan** alanlar olarak
eklenir.

### Ürün hedefleri

- Finansal veriyi tek ve anlaşılır bir yerde toplamak.
- Bakiye ve raporların aynı finansal hareketlerden hesaplanmasını sağlamak.
- Kullanıcıların verilerini birbirinden güvenli biçimde ayırmak.
- Manuel veri girişinden CSV ve banka bağlantısına kontrollü geçiş yapmak.
- Android ile başlayıp Google Play ve daha sonra iOS'a ilerlemek.
- İlk MVP'den sonraki sürümlere veri kaybetmeden geçebilmek.

### Başarı ölçütleri

- Temel hesap, gelir, gider ve bütçe akışları mobil uygulamadan tamamlanabilir.
- İki farklı kullanıcı birbirinin hiçbir finansal verisine erişemez.
- Hesap bakiyesi ve aylık raporlar sentetik senaryolarla doğrulanır.
- Tekrarlanan API veya senkronizasyon isteği çift finansal kayıt oluşturmaz.
- Yedekleme ve geri yükleme gerçek finansal veri kullanılmadan önce denenir.
- Her sürüm kendi test, güvenlik ve çıkış koşullarını karşılar.

## 3. Hedef kullanıcı ve değer önerisi

İlk hedef, **işini kendi yürüten şahıs şirketi sahibi ve esnaftır**: muhasebe
personeli yoktur, kayıtları kendi tutar, ay sonunda muhasebecisine belge
gönderir. Kullanıcının aşağıdaki ihtiyaçları olduğu varsayılır:

- Birden fazla nakit, banka ve kredi kartı hesabını izlemek
- Harcamaları kategori ve bütçelerle kontrol etmek
- Kart ekstresi ve taksit yükünü görmek
- Düzenli ödeme ve gelirleri unutmamak
- Borç, alacak ve hedef durumunu tek toplamda değerlendirmek
- Fiş ve dekont gibi belgeleri kaydın yanında saklamak
- Verisini dışarı aktarabilmek ve yedekleyebilmek

Bu kullanıcının işletme gideri ile şahsi gideri aynı cepten çıkar; ürün ikisini
ayrı iki dünya gibi kurgulamaz.

Çok kullanıcılı işletme (personel rolleri, onay akışları), bordro ve stok ana
kapsamda değildir. Bunlar ancak ürün oturduktan sonra ayrı bir ürün kararıyla
yeniden değerlendirilebilir.

## 4. Ürün ilkeleri

- Finansal hesapların doğruluğu görsel zenginlikten önce gelir.
- Para değerlerinde backend tarafında `decimal` kullanılır.
- İlk sürümde ana para birimi TRY'dir.
- Para birimi alanları baştan tutulur; gerçek çoklu para birimi ileri sürümdür.
- Mobil istemci finansal iş kurallarının tek kaynağı olmaz.
- Kullanıcı kimliği istek gövdesinden değil güvenli oturumdan belirlenir.
- Hesap bakiyesi ikinci bir gerçek kaynak olarak elle saklanmaz.
- Hesap, kart ve kategori geçmişi fiziksel silmeyle anlamsız hale getirilmez.
- Gerçek finansal veri kullanılmadan önce güvenlik ve geri yükleme doğrulanır.
- Girişsiz veya yalnız yerel geliştirme sürümü internete açılmaz.
- Mikroservis yerine küçük, katmanlı monolit kullanılır.
- Ürün vergi hesaplamaz ve beyanname üretmez; muhasebeciye giden veriyi
  hazırlar (bkz. "Ürün sınırı: muhasebe yerine geçmez").
- İşletme ve şahsi harcama tek finansal tabloda yaşar; ayrıştırma bir
  raporlama boyutudur, ayrı bir uygulama değil.

## 5. Ürün kapsamı

### 5.1 Kimlik ve hesap güvenliği

- Kayıt olma ve giriş yapma
- Access token ve yenilenen refresh token
- Kullanıcı verilerinin sahiplik bilgisiyle ayrılması
- Oturum kapatma ve aktif oturumları sonlandırma
- Bulut aşamasında e-posta doğrulama
- Parola sıfırlama
- Hatalı giriş ve istek hız sınırları
- Mobil cihazda güvenli token saklama
- İleri aşamada biyometrik uygulama kilidi

### 5.2 Finansal hesaplar

- Nakit hesapları
- Banka hesapları
- Açılış bakiyesi
- Hesap para birimi
- Aktif/pasif hesap durumu
- Hesaplar arası transfer
- Hareketlerden hesaplanan bakiye
- Hesap mutabakatı

### 5.3 Gelir, gider ve kategoriler

- Gelir ve gider işlemleri
- İşlem tarihi, oluşturulma zamanı, tutar ve açıklama
- Gelir ve gider kategorileri
- Hesap, kategori, tür ve tarih filtreleri
- Arama ve sıralama
- Planlanan ve gerçekleşen işlem ayrımı
- İşlem iptali ve değişiklik geçmişi
- Fiş veya belge eki

### 5.4 Kredi kartları ve taksitler

- Kart limiti ve kullanılabilir limit
- Hesap kesim ve son ödeme günü
- Güncel dönem ve ekstre borcu
- Kart harcamaları
- Taksit planı ve gelecek ayların yükü
- Kart borcu ödemesi
- Kart ödemesinin ikinci kez gider sayılmaması
- Geciken ödeme ve yaklaşan ekstre uyarıları

### 5.5 Bütçeler ve planlama

- Kategori bazlı aylık bütçe
- Harcanan, kalan ve aşılan tutar
- Aylık toplam gelir ve gider hedefi
- Tekrarlayan gelir, gider ve abonelikler
- Yaklaşan ödeme takvimi
- Tasarruf hedefleri
- Hedefe ayrılan ve kalan tutar

### 5.6 Borç, alacak ve krediler

- Borç ve alacak kaydı
- Kredi anapara, faiz ve ödeme planı
- Kalan borç ve yaklaşan taksitler
- Yapılan ödemenin hesap hareketiyle ilişkilendirilmesi
- Gecikmiş ödeme durumu

### 5.7 Raporlar

- Aylık gelir, gider ve net fark
- Kategori bazlı harcama dağılımı
- Hesap ve kart bakiyeleri
- Net varlık
- Önceki dönem karşılaştırması
- Bütçe kullanım ve aşım raporu
- Gelecek taksit ve ödeme yükü
- Nakit akışı eğilimi
- Tasarruf hedefi ilerlemesi
- Para birimi ve yatırım raporları

### 5.8 Veri içe ve dışa aktarma

- CSV ekstre içe aktarma
- Bankaya göre kolon eşleştirme profilleri
- Dosya ve satır doğrulama
- Tekrar kayıt tespiti
- İçe aktarma ön izlemesi ve onayı
- Hatalı satır raporu
- CSV dışa aktarma
- Uygulama yedeği ve geri yükleme

### 5.9 Offline çalışma ve senkronizasyon

- İlk adımda online-first çalışma
- Son başarılı verilerin okunabilir cache'i
- Cache zamanı ve bağlantı durumunun gösterilmesi
- Sonraki adımda offline işlem kuyruğu
- Tekrar gönderimde çift kayıt önleme
- Sunucu değişiklik sürümü
- Çakışma tespiti ve kullanıcıya görünür çözüm
- Kullanıcı çıkışında yerel finans verisinin güvenli temizlenmesi

### 5.10 Read-only banka bağlantısı

- Türkiye'deki uygun ve yetkili sağlayıcıların araştırılması
- Sandbox ile bağlantı denemesi
- Kullanıcı rızası ve bağlantı yaşam döngüsü
- Hesap ve işlem bilgilerinin okunması
- Rıza/token yenileme ve iptal
- Gelen hareketleri mevcut kayıtlarla eşleştirme
- Mükerrer hareketleri engelleme
- Senkronizasyon hata ve gecikmelerini görünür gösterme

Uygulama bankadan ödeme veya transfer başlatmaz. Canlı bağlantı, uygun ve
yetkili bir sağlayıcı bulunmasına bağlıdır. Sağlayıcı bulunamazsa sandbox ve
CSV entegrasyonu çalışır durumda kalır.

### 5.11 Yatırım ve çoklu para birimi

- Manuel yatırım hesabı
- Hisse, fon, altın ve kripto varlığı kaydı
- Alış maliyeti, miktar ve gerçekleşmemiş kazanç/kayıp
- Daha sonra piyasa fiyat sağlayıcısı
- Kullanıcıya ait temel raporlama para birimi
- Dövizli hesap ve işlem
- Kur anlık görüntüsü ve kur geçmişi
- Kur sağlayıcısı kesintisinde son bilinen değer uyarısı

## 6. Temel kullanıcı deneyimi

### Ana navigasyon

Material 3 tabanlı mobil uygulamada aşağıdaki ana bölümler bulunur:

```text
Özet | İşlemler | Bütçeler | Diğer
```

`Diğer` bölümü hesaplar, kartlar, kategoriler, hedefler, raporlar ve ayarları
içerir. Hızlı işlem ekleme belirgin bir ana eylem düğmesiyle açılır.

### Temel akışlar

1. Kullanıcı kayıt olur ve giriş yapar.
2. Nakit veya banka hesabı oluşturur.
3. Gelir/gider kategorilerini hazır listeden seçer veya düzenler.
4. İşlem ekler; bakiye ve bütçe bilgisi güncellenir.
5. Ana ekranda aylık durum ve yaklaşan ödemeler görüntülenir.
6. İleri sürümlerde kart, taksit, transfer ve hedefler aynı akışa eklenir.
7. CSV veya banka bağlantısından gelen hareketler önce eşleştirilip onaylanır.

### Zorunlu ekran durumları

- Yükleniyor
- Boş sonuç
- Doğrulama hatası
- Ağ veya sunucu hatası
- Yetkisiz/oturumu bitmiş kullanıcı
- Kısmi senkronizasyon
- Eski cache verisi
- Başarılı kayıt veya güncelleme

## 7. Teknik mimari

### 7.1 Genel yapı

```text
Flutter Android/iOS uygulaması
            │
            │ HTTPS + JSON
            ▼
ASP.NET Core Web API
            │
            ├── Application
            ├── Domain
            └── Infrastructure
                    │
                    ▼
                SQL Server
```

### 7.2 Backend katmanları

- `BusinessFinance.Domain`: Entity, value object ve değişmez iş kuralları
- `BusinessFinance.Application`: Kullanım senaryoları ve dış servis sözleşmeleri
- `BusinessFinance.Infrastructure`: EF Core, Identity, dosya, bildirim ve dış entegrasyonlar
- `BusinessFinance.Api`: HTTP endpoint'leri, authentication ve hata cevapları
- Unit ve integration test projeleri

Domain katmanı EF Core, HTTP, Flutter veya dış sağlayıcı bağımlılığı taşımaz.
Genel amaçlı repository veya gereksiz CQRS/MediatR altyapısı başlangıçta
eklenmez. Application katmanı kullanım senaryoları büyüdüğü için planın
başından itibaren bulunur.

### 7.3 Flutter yapısı

- Material 3
- Özellik bazlı klasörler
- View, ViewModel, Repository ve Service ayrımı
- `ChangeNotifier` ve `provider`
- `go_router` ile navigasyon
- `http` ile API erişimi
- `flutter_secure_storage` ile oturum bilgisi
- Küçük modellerde elle JSON dönüşümü; tekrar arttığında `json_serializable`
- Offline aşamasında `drift`
- Grafikler için `fl_chart`

Flutter doğrulamaları kullanıcı deneyimini iyileştirir; finansal kuralları
tek başına korumaz.

## 8. Kavramsal domain modeli

### Kimlik

- `User`
- `RefreshSession`
- `DeviceRegistration`
- `AuditEntry`

### Temel finans

- `Account`
- `Category`
- `BudgetTransaction`
- `Transfer`
- `MonthlyBudget`

### Kart ve planlama

- `CreditCard`
- `CreditCardStatement`
- `InstallmentPlan`
- `RecurringTransaction`
- `SavingsGoal`
- `DebtOrLoan`

### Veri ve entegrasyon

- `ImportBatch`
- `ImportRow`
- `Attachment`
- `BankConnection`
- `ExternalBankAccount`
- `ExternalTransactionCandidate`
- `SyncOperation`

### Gelişmiş finans

- `Currency`
- `ExchangeRateSnapshot`
- `InvestmentAccount`
- `Asset`
- `Holding`
- `PriceSnapshot`

Her entity için sahiplik, tarihçe, pasifleştirme ve eşzamanlı güncelleme
ihtiyacı ilgili geliştirme aşamasında açıkça değerlendirilir.

## 9. API ve veri sözleşmesi ilkeleri

- API tabanı `/api/v1` olur.
- Kullanıcı kimliği JWT claim'den belirlenir; istek gövdesinden alınmaz.
- Para, kayıpsız ondalık metin ve ISO 4217 para koduyla taşınır.
- İş tarihleri `yyyy-MM-dd` olur.
- Sistem zaman damgaları UTC ISO 8601 biçiminde taşınır.
- Hatalar standart `ProblemDetails` cevabı kullanır.
- Liste endpoint'leri sayfalama, filtreleme ve sıralama destekler.
- Offline yazma işlemleri idempotency anahtarı taşır.
- Eşzamanlı düzenleme için concurrency token kullanılır.
- Açılış bakiyesi ve hareketler, hesap bakiyesinin tek kaynağıdır.
- Finansal kayıtların iptal/düzeltme geçmişi kaybolmaz.

### Endpoint grupları

- `/api/v1/auth`
- `/api/v1/accounts`
- `/api/v1/categories`
- `/api/v1/transactions`
- `/api/v1/transfers`
- `/api/v1/budgets`
- `/api/v1/credit-cards`
- `/api/v1/recurring-transactions`
- `/api/v1/goals`
- `/api/v1/debts`
- `/api/v1/reports`
- `/api/v1/imports`
- `/api/v1/exports`
- `/api/v1/sync`
- `/api/v1/bank-connections`
- `/api/v1/investments`
- `/api/v1/exchange-rates`

## 10. Veri güvenliği ve gizlilik

- ASP.NET Core Identity tabanlı kullanıcı yönetimi
- Kısa ömürlü access token ve döndürülen refresh token
- Her sorgu ve komutta kullanıcı sahipliği kontrolü
- Kullanıcılar arası erişimi reddeden negatif testler
- Secret değerlerinin yalnız sunucuda tutulması
- Mobil uygulamaya connection string, banka secret'ı veya özel anahtar koymama
- HTTPS zorunluluğu
- Rate limiting ve brute-force koruması
- Hassas finansal veriyi loglamama
- Dosya eklerinde tür, boyut ve zararlı içerik doğrulaması
- Audit kaydı ve kritik değişiklik geçmişi
- Yedek şifreleme ve geri yükleme testi
- Hesap kapatma, veri dışa aktarma ve veri silme akışı
- Bulut/public beta öncesinde KVKK ve mağaza gizlilik metni incelemesi

## 11. Gerekli yazılımlar ve servisler

### Yerel geliştirme

- Visual Studio Community 2026
- .NET 10 LTS SDK
- ASP.NET Core 10
- Entity Framework Core 10
- ASP.NET Core Identity
- Git ve GitHub
- Docker Desktop ve Docker Compose
- Microsoft SQL Server container
- SQL Server Management Studio 22
- Flutter stable SDK ve Dart
- Android Studio
- Android SDK ve Android Emulator
- OpenAPI ve Visual Studio `.http` dosyaları
- xUnit
- Flutter unit, widget ve integration test araçları

Node.js, React, TypeScript ve Vite bu mobil ürünün ana teknoloji zincirinde
bulunmaz.

### Bulut ve yayın aşaması

- Azure CLI
- Azure App Service for Containers
- Azure SQL Database
- Azure Key Vault
- Azure Blob Storage
- Application Insights ve OpenTelemetry
- GitHub Actions
- Firebase Cloud Messaging
- Google Play Console
- Android upload keystore
- iOS aşamasında Mac, Xcode ve Apple Developer hesabı

Azure ilk MVP'de kurulmaz. Yerel MVP'ler tamamlandıktan ve gerçek kullanıcı
verisine geçiş koşulları karşılandıktan sonra ayrı aşamada eklenir.

## 12. Ortamlar ve dağıtım modeli

### Local development

- API yerel makinede çalışır.
- SQL Server Docker container içindedir.
- Flutter Android emülatör veya fiziksel cihazda çalışır.
- Yalnız sentetik veri kullanılır.

### Local MVP

- API ve SQL Server güvenilen yerel makinededir.
- Mobil cihaz aynı güvenilen ağ veya kontrollü bağlantıyla API'ye erişir.
- Bilgisayar kapalıyken uygulama sunucu verisine erişemez.
- İnternete açık port oluşturulmaz.

### Cloud beta

- API container olarak Azure'da çalışır.
- SQL Server yerine uyumlu Azure SQL Database kullanılır.
- Secret değerleri Key Vault'ta tutulur.
- Log, metrik ve hata takibi Application Insights'a gider.
- Google Play kapalı test kullanıcılarıyla doğrulama yapılır.

### Production

- Geri alınabilir dağıtım
- Veritabanı migration kontrolü
- Otomatik ve doğrulanmış yedekleme
- Güvenlik ve performans kontrolleri
- Google Play production; daha sonra iOS

## 13. MVP ve sürüm yol haritası

Süreler aktif geliştirme için yaklaşık aralıklardır. Öğrenme hızı, dış servis
onayları ve mağaza incelemeleri bu aralıklara dahil değildir.

### Hazırlık — Teknik temel

Yapılacaklar:

- Repo, solution ve project yapısı
- Backend katmanları ve test projeleri
- Flutter project'i
- Docker SQL Server
- Ortam ve secret yapılandırması
- Başlangıç build/test kontrolleri
- CI temelinin hazırlanması

Çıkış koşulu:

- Backend, testler ve boş Flutter uygulaması temiz ortamda derlenebilir.
- Secret ve üretilmiş dosyalar Git'e girmez.

### MVP-1 — Yerel temel bütçe çekirdeği

Tahmini aktif geliştirme: **4–7 hafta**

Yapılacaklar:

- Kayıt ve giriş
- JWT access ve refresh token
- Kullanıcı verilerinin ayrılması
- Nakit ve banka hesapları
- Açılış bakiyesi
- Gelir/gider kategorileri
- Gelir/gider işlemleri
- Aylık kategori bütçeleri
- Ana gösterge ekranı
- Aylık gelir, gider ve kategori raporu
- Android APK
- Yerel API ve SQL Server
- Online-first kullanım

Çıkış koşulu:

- İki kullanıcı birbirinin verisine erişemez.
- Temel bütçe işlemleri Android cihazdan tamamlanır.
- Bakiye ve bütçe sonuçları testlerle doğrulanır.

### MVP-1.5 — Güçlü işletme finansı

Tahmini ek geliştirme: **4–7 hafta**

Yapılacaklar:

- Hesaplar arası transfer
- Kredi kartları
- Kart limiti ve kullanılabilir limit
- Ekstre kesim ve son ödeme tarihi
- Taksitli işlemler
- Kart borcu ödemesi
- Tekrarlayan gelir/gider
- Yaklaşan ödemeler
- Net varlık raporu
- Gelişmiş filtreleme

Çıkış koşulu:

- Transferler toplam gelir ve gideri bozmaz.
- Kart ödemesi ikinci kez gider sayılmaz.
- Ekstre, taksit ve limit sonuçları testlerle doğrulanır.

### MVP-2 — Planlama ve veri taşınabilirliği

Tahmini ek geliştirme: **4–7 hafta**

Yapılacaklar:

- Tasarruf hedefleri
- Borç, alacak ve kredi takibi
- CSV ekstre içe aktarma
- Kolon eşleştirme profilleri
- Tekrar kayıt tespiti
- Veri dışa aktarma
- Yerel yedekleme ve geri yükleme
- Fiş/belge eki
- Ayrıntılı dönem karşılaştırmaları
- Çoklu para birimine hazır API sözleşmeleri

Çıkış koşulu:

- Aynı ekstre iki kez işlendiğinde mükerrer hareket oluşmaz.
- Hatalı dosya satırları açıkça raporlanır.
- Yedek geri yükleme sentetik veriyle doğrulanır.

### MVP-3 — Azure güvenli beta

Tahmini ek geliştirme: **4–8 hafta**

Yapılacaklar:

- Container olarak API yayını
- Azure SQL
- HTTPS
- Key Vault
- E-posta doğrulama
- Parola sıfırlama
- Refresh token rotation
- Rate limiting
- Blob Storage
- Application Insights
- Otomatik bulut yedekleri
- Push bildirimleri
- Google Play kapalı test

Çıkış koşulu:

- Uygulama yerel bilgisayar kapalıyken çalışır.
- Secret değerleri istemciye veya repoya girmez.
- Yedek geri yükleme başarılıdır.
- Kapalı testte kritik kullanıcı akışları tamamlanır.

### MVP-4A — Offline görüntüleme

Tahmini ek geliştirme: **2–4 hafta**

Yapılacaklar:

- Son başarılı verilerin cihazda cache edilmesi
- Bağlantısız hesap, işlem, bütçe ve rapor görüntüleme
- Cache yaşı ve bağlantı durumunun gösterilmesi
- Oturum kapanınca yerel finans verisinin temizlenmesi

Çıkış koşulu:

- Kullanıcı bağlantısızken son veriyi görebilir.
- Eski cache güncel veri gibi gösterilmez.

### MVP-4B — Tam offline senkronizasyon

Tahmini ek geliştirme: **6–12 hafta**

Yapılacaklar:

- Offline gelir/gider ekleme ve düzenleme
- Yerel işlem kuyruğu
- Idempotency anahtarı
- Güvenli tekrar gönderme
- Sunucu değişiklik sürümü
- Çakışma tespiti ve çözüm ekranı
- Silme/iptal senaryoları
- Birden fazla cihaz testi

Çıkış koşulu:

- Aynı offline işlem yeniden gönderildiğinde çift kayıt oluşmaz.
- Çakışmalar sessiz biçimde veri ezmez.
- Kesilen senkronizasyon güvenli biçimde devam eder.

### MVP-5 — Read-only açık bankacılık

Tahmini teknik geliştirme: **10–20+ hafta**. Sağlayıcı, sözleşme ve
yetkilendirme süreleri dahil değildir.

Yapılacaklar:

- Türkiye sağlayıcı ve yetkilendirme fizibilitesi
- Sandbox adapter'ı
- Kullanıcı rızası ve bağlantı yaşam döngüsü
- Hesap ve işlem okuma
- Token/rıza yenileme ve iptal
- Webhook veya periyodik senkronizasyon
- Manuel ve dış hareket eşleştirme
- Mükerrer işlem önleme
- Bağlantı hatalarını görünür kılma

Çıkış koşulu:

- Sağlayıcı sandbox'ında hesap ve hareketler güvenli okunur.
- Rıza iptal edilince erişim sona erer.
- Gelen işlemler çift finansal kayıt oluşturmaz.

Canlı sağlayıcı mümkün olmazsa bu aşama sandbox adapter'ı ve CSV ile
tamamlanır; gerçek bağlantı koşullu kapsam olarak kalır.

### MVP-6 — Gelişmiş finans ve platformlar

Tahmini ek geliştirme: **6–12+ hafta**

Yapılacaklar:

- Manuel yatırım portföyü
- Hisse, fon, altın ve kripto varlıkları
- Maliyet ve performans hesabı
- Daha sonra fiyat API'si
- Gerçek çoklu para birimi
- Kur geçmişi ve raporlama
- Gelişmiş bildirimler
- Google Play production
- iOS uygulaması
- Performans, erişilebilirlik ve güvenlik sertleştirmesi

Çıkış koşulu:

- Mağaza sürümü izlenebilir ve geri alınabilir.
- Finansal raporlar para birimi ve yatırım senaryolarıyla doğrulanır.
- Android production akışları tamamlanır; iOS kapsamı ayrı cihazlarda test edilir.

## 14. Test ve kalite planı

### Backend

- Domain kuralları için unit testler
- Kimlik ve kullanıcı izolasyonu için negatif testler
- Gerçek SQL Server ile integration testleri
- API sözleşme ve `ProblemDetails` testleri
- Migration ileri/geri uyumluluk kontrolleri
- Import idempotency testleri
- Banka adapter sandbox testleri

### Flutter

- ViewModel ve repository unit testleri
- Form, yükleniyor, boş ve hata widget testleri
- Authentication ve işlem ekleme integration testleri
- Farklı ekran boyutu ve metin ölçeklendirme testleri
- Offline cache ve senkronizasyon testleri

### Sistem

- Temel uçtan uca kullanıcı akışları
- Yedekleme ve geri yükleme tatbikatı
- Secret ve hassas veri kontrolü
- Yetkisiz veri erişimi testleri
- Performans ve sayfalama testleri
- Google Play öncesi erişilebilirlik ve release build kontrolü

### Ana branch kalite kapıları

- Backend restore, build, unit ve integration testleri
- Backend format doğrulaması
- Flutter dependency, analyze, unit/widget testleri
- Android debug ve release build
- Secret taraması
- Migration ve OpenAPI değişiklik incelemesi

## 15. Riskler ve karar kapıları

### Canlı banka entegrasyonu

Risk: Yetkilendirme, sağlayıcı erişimi, sözleşme ve mevzuat gereksinimleri.

Karar kapısı: Sandbox başarıyla tamamlanmadan ve uygun sağlayıcı doğrulanmadan
canlı entegrasyon yapılmaz.

### Offline senkronizasyon

Risk: Çift kayıt, veri çakışması ve eski verinin yeni veriyi ezmesi.

Karar kapısı: Online sürüm kararlı olmadan offline yazma açılmaz.

### Bulut ve gerçek veri

Risk: İnternete açık finansal veri, secret sızıntısı ve geri yüklenemeyen yedek.

Karar kapısı: HTTPS, kullanıcı izolasyonu, rate limiting, yedekleme ve geri
yükleme doğrulanmadan gerçek finansal veri kullanılmaz.

### Piyasa fiyatları

Risk: Lisans, gecikmeli veri, sağlayıcı kesintisi ve yanlış değerleme.

Karar kapısı: Manuel portföy kararlı olmadan fiyat API'si eklenmez; fiyatın
hangi zamana ait olduğu kullanıcıya gösterilir.

### Kapsam büyümesi

Risk: İlk kullanılabilir sürümün sürekli ertelenmesi.

Karar kapısı: Bir MVP'nin çıkış koşulları tamamlanmadan sonraki MVP'nin
framework veya servisleri eklenmez.

## 16. Kapsam dışında

- Hane ve ortak bütçe
- Banka üzerinden ödeme veya transfer başlatma
- İlk sürümlerde internet üzerinden production yayını
- Mikroservis ve mesaj kuyruğu mimarisi
- Kripto cüzdan anahtarı saklama
- Hisse veya kripto alım-satım emri verme
- Muhasebe ve vergi beyannamesi
- Otomatik finansal danışmanlık
- Yapay zekânın kullanıcı onayı olmadan finansal kayıt değiştirmesi

## 17. Resmî referanslar

- [Flutter uygulama mimarisi](https://docs.flutter.dev/app-architecture/guide)
- [Flutter mimari önerileri](https://docs.flutter.dev/app-architecture/recommendations)
- [Flutter Material Design](https://docs.flutter.dev/ui/design/material)
- [Flutter editör desteği](https://docs.flutter.dev/tools/editors)
- [Flutter Android kurulumu](https://docs.flutter.dev/platform-integration/android/setup)
- [Flutter test yaklaşımı](https://docs.flutter.dev/testing/overview)
- [Flutter Android yayınlama](https://docs.flutter.dev/deployment/android)
- [TCMB Açık Bankacılık duyurusu](https://www.tcmb.gov.tr/wps/wcm/connect/TR/TCMB+TR/Main+Menu/Duyurular/Basin/2022/DUY2022-48)
- [Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/overview)
- [Azure SQL Database](https://learn.microsoft.com/en-us/azure/azure-sql/database/sql-database-paas-overview)
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview)
- [Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)

## 18. Sonraki planlama belgeleri

Bu ürün planı tamamlandıktan sonra aşağıdaki konular ayrı belgelerde
hazırlanacaktır:

- Oturum ve checkpoint düzeni
- Proje durum belgesi
- Git ve branch çalışma yöntemi
- Mimari karar kayıtları

Bu başlıkların ayrıntıları bu ürün planına eklenmez; ürün kapsamı ile çalışma
yöntemi birbirinden ayrı tutulur.
