# Mimari Kayıt

## Ürün özeti ve temel varsayımlar

BusinessFinance; şahıs şirketi ve esnaf için işletme finansı uygulamasıdır.
Hesap, kategori, gelir/gider, kredi kartı ve taksit, tekrarlayan plan,
borç/alacak, hedef, birleşik finansal hareket akışı, CSV içe/dışa aktarma,
yedekleme ve fiş okuma akışlarını sunar. Katmanlı monolit ASP.NET Core API
finansal gerçeğin kaynağıdır; Flutter istemci sonuçları gösterir; SQL Server
kalıcı depodur. Bugün yalnız sentetik veri, tek host ve Pixel 8 emulatorü
varsayılır; ürün internete açık değildir.

```text
Pixel 8 app --HTTP debug / 10.0.2.2:5284--> Windows loopback API
                                                   |
                                      EF Core -> SQL Server
                                                 127.0.0.1:14334
```

## Belgenin durumu

Bu belge repository'de doğrulanmış mimari yapıyı kaydeder. Planlanan fakat
henüz uygulanmayan katmanlar ayrıca belirtilir.

## Mevcut yapı

```text
BusinessFinance/
├── compose.yaml
├── mobile/
│   └── business_finance_mobile/
│       ├── lib/app/
│       ├── lib/core/{config,files,network,routing,storage,theme,widgets}/
│       └── lib/features/{auth,dashboard,accounts,categories,transactions,budgets,cards,planning,more}/
├── BusinessFinance.slnx
├── scripts/
│   └── New-LocalSqlEnvironment.ps1
└── src/
    ├── BusinessFinance.Application/
    ├── BusinessFinance.Application.Tests/
    ├── BusinessFinance.Api/
    ├── BusinessFinance.Api.Tests/
    ├── BusinessFinance.CSharpSandbox/
    ├── BusinessFinance.Domain/
    ├── BusinessFinance.Domain.Tests/
    ├── BusinessFinance.Infrastructure/
    └── BusinessFinance.Infrastructure.Tests/
```

- `BusinessFinance.slnx`, mevcut .NET projelerini bir arada düzenler.
- `BusinessFinance.CSharpSandbox`, C# öğrenme ve küçük deneyler için konsol
  uygulamasıdır.
- Sandbox production uygulama katmanı değildir ve finansal iş kurallarının
  kalıcı evi olarak kullanılmaz.
- `BusinessFinance.Domain`, framework bağımsız finansal tip ve kuralları taşır.
- `BusinessFinance.Domain.Tests`, Domain'e tek yönlü project reference veren
  xUnit test projesidir.
- `BusinessFinance.Application`, kullanıcı senaryolarını yürütecek framework
  bağımsız katmandır ve yalnız `BusinessFinance.Domain` projesine reference verir.
- `BusinessFinance.Application.Tests`, Application kaynak sınırını ve ilerleyen
  gruplarda use case davranışlarını doğrulayan xUnit projesidir.
- `BusinessFinance.Api`, Minimal API composition root'unu, HTTP pipeline'ını,
  endpoint sözleşmelerini ve current-user adapter'ını taşır.
- `BusinessFinance.Api.Tests`, ProblemDetails ve gerçek test-server HTTP
  sözleşmelerini doğrular.
- `BusinessFinance.Infrastructure`, ASP.NET Core Identity gibi framework ve dış
  sistem ayrıntılarını taşır; Application'a bağımlıdır. EF Core SQL Server
  `BusinessFinanceDbContext` ve Identity EF store burada bulunur.
- `BusinessFinance.Infrastructure.Tests`, Identity modeli ve ilerleyen güvenlik
  adaptörlerini altyapı sınırında doğrular.
- Domain'de `Money`, hesap/kategori/hareket/bütçe, transfer, kredi kartı,
  kart charge/payment, taksit, recurring tanımı ve occurrence uygulanmıştır.
- Flutter istemcisi Material 3 açık/koyu tema, `go_router` tabanlı dört dallı
  navigation shell ve ortak loading/empty/error/unauthorized bileşenlerini taşır.
  Telefon genişliğinde `NavigationBar`, 720 ve üzerindeki genişlikte
  `NavigationRail` kullanır. Network, secure authentication ve finans
  feature'ları ile planlama/rapor ekranları gerçek API sözleşmesine bağlıdır.
- Yerel SQL Server sabit CU7 image,
  loopback port, health check ve named volume ile Compose üzerinde çalışır.
  Identity, hesap, kategori, hareket, bütçe ve refresh session verileri EF Core
  adapter'larıyla SQL Server'da kalıcıdır.

## Katman ve altyapı yönü

PRD'ye göre backend küçük, katmanlı bir monolit olacaktır:

```text
Flutter uygulaması
        │
        ▼
ASP.NET Core API
        │
        ├── Application
        ├── Domain
        └── Infrastructure
                │
                ▼
            SQL Server
     (Docker named volume)
```

API, Application, Domain, Infrastructure ve yerel SQL Server bileşenleri vardır.
Identity, Account, Category, BudgetTransaction, MonthlyBudget, RefreshSession,
Transfer, CreditCard, CreditCardCharge, CreditCardPayment, InstallmentPlan,
RecurringTransaction ve RecurringTransactionOccurrence persistence akışları
scoped EF Core adapter'ları üzerinden SQL Server kullanır.
Şema tek bir `InitialCreate` migration'ıyla kurulur (27 tablo); ürünün
yayımlanmış bir sürümü olmadığı için önceki yükseltme zinciri bu repoya
taşınmadı. Ekstre ayrı tablo değildir;
kalıcı kart hareketlerinden hesaplanan projection'dır.

## Geçerli mimari sınırlar

- Domain, xUnit, EF Core, HTTP, Identity veya Flutter bağımlılığı taşımaz.
- Test bağımlılığı yalnız `BusinessFinance.Domain.Tests` →
  `BusinessFinance.Domain` yönündedir.
- Application, Infrastructure ve API katmanları uygulanmıştır. API, use case
  kayıtları ile Infrastructure adapter'larını composition root'ta birleştirir.
- Persistence ve rapor port'ları Application içinde tanımlanır;
  `EfAccountRepository`, `EfCategoryRepository`, `EfTransactionRepository`,
  `EfBudgetRepository`, `EfFinancialReportRepository` ve
  `EfRefreshSessionRepository` implementasyonları Infrastructure'da bulunur.
- İlk current-user port'u `ICurrentUser`, authentication teknolojisini
  Application'dan ayırır; ilk persistence port'u `IAccountRepository` yalnız
  hesap ekleme ihtiyacını taşır.
- `CreateAccountUseCase`, kullanıcı kimliğini komuttan değil `ICurrentUser`
  üzerinden alır ve geçerli Domain entity'sini repository port'una iletir.
- `ListAccountsUseCase`, kullanıcıya ait pagination/filtre/sıralama kriterini
  repository port'una taşır; Account entity'lerini liste DTO'larına eşler.
- Tek hesap okuma ve pasifleştirme request'leri yalnız AccountId taşır. UserId,
  `ICurrentUser` üzerinden alınır ve repository bulma/update işlemlerinde zorunlu
  sahiplik kapsamı olarak kullanılır.
- Başka kullanıcıya ait veya bulunmayan hesap aynı NotFound sonucuna gider;
  mevcut MVP'de admin/rol bypass yolu bulunmaz.
- Hesap listeleme sözleşmesinde sıralama persistence tarafında pagination'dan
  önce uygulanmalıdır; Application yalnız istenen sıralamayı belirtir.
- Application use case'leri başarı DTO'su veya kararlı kod/tür taşıyan
  `ApplicationResult<T>` döndürür; HTTP durum koduna dönüşüm API katmanının
  ilerideki sorumluluğudur.
- Current user bulunmadığında repository çağrısı yapılmaz. Duplicate hesap adı
  Application ön kontrolünde Conflict olur; owner-scoped unique database index'i
  eşzamanlı yazma yarışında kesin son kapıdır.
- Identity modeli Infrastructure içindedir. Domain ve Application Identity
  tiplerine bağımlı olmaz; kullanıcı sahipliği katmanlar arasında GUID ile
  eşlenir.
- `ApplicationUser`, `IdentityUser<Guid>` tabanında e-posta, UTC oluşturulma
  zamanı ve aktiflik taşır; düz metin parola alanı içermez.
- Application kayıt/giriş use case'leri `IIdentityAccountService` port'una
  bağımlıdır; Infrastructure implementasyonu Identity `UserManager` kullanır.
- Parola politikası ve hashing Identity tarafından uygulanır. Bilinmeyen
  e-posta, yanlış parola ve pasif kullanıcı dışarıya aynı genel credential
  hatasını verir.
- `ISecurityTokenService` ve `IRefreshSessionRepository` port'ları Application
  içindedir; JWT/kriptografi ayrıntısı Infrastructure'da kalır.
- Access token HS256 imzalıdır, yalnız `sub`, `email` ve `jti` claim'lerini taşır
  ve varsayılan 15 dakika yaşar. Signing key yapılandırmadan alınır.
- Refresh token 64 kriptografik rastgele byte'tan üretilir; Domain session ham
  tokenı değil SHA-256 hash'ini, yaşam süresini ve rotation/reuse durumunu taşır.
- Login session oluşturur; refresh eski sessionı yeni sessionla değiştirir;
  logout sessionı revoke eder. Revoke edilmiş token reuse edilirse kullanıcının
  aktif refresh sessionlarını kapatma sözleşmesi uygulanır.
- Identity kullanıcı store'u, Account ve RefreshSession portları production'da
  scoped EF Core adapter'larıyla çalışır. Read-only Account liste/varlık sorguları
  `AsNoTracking`; değişiklik akışları tracking kullanır.
- `BusinessFinanceDbContext`, Identity GUID kullanıcı/rol modelini içerir ve
  production'da EF Core SQL Server provider'ını kullanır. Account, Category,
  BudgetTransaction ve MonthlyBudget mapping'leri Infrastructure configuration
  sınıflarında bulunur. Tek `InitialCreate` migration'ı Identity, finans, kart,
  planlama, borç, hedef, import ve ek şemalarını birlikte kurar. Snapshot
  sonraki model farklarının kaynağıdır; bundan sonra eklenen her migration
  gerçek bir yükseltme yoludur ve `AGENTS.md` içindeki migration kurallarına
  uyar.
- `BusinessFinanceDesignTimeDbContextFactory`, yalnız EF CLI tasarım zamanı için
  context üretir ve bağlantıyı `ConnectionStrings__BusinessFinance` environment
  değişkeninden alır. Secret kaynak kodda veya tool manifest'te tutulmaz.
- Transaction→Account/Category ve MonthlyBudget→Category ilişkileri yalnız kayıt
  ID'siyle değil `(UserId, kayıt ID)` composite foreign key'iyle modellenir.
  Database modeli böylece çapraz kullanıcı referansını reddedecek biçimdedir.
- `Money` ayrı tablo değildir; owner tablosunda `decimal(19,4)` tutar ve `tinyint`
  currency kolonlarıyla owned value object olarak saklanır.
- MonthlyBudget `PeriodStart/PeriodEnd` kolonlarını saklamaz; değerler Year/Month
  üzerinden Domain'de hesaplanır.
- JWT bearer doğrulaması issuer, audience, HS256 imza ve lifetime kontrolü yapar;
  `HttpContextCurrentUser` yalnız doğrulanmış `sub` claim'ini Application'a taşır.
- JWT signing key repoya yazılmaz; local Development değeri user-secrets
  deposundadır ve eksik/zayıf key uygulama başlangıcını durdurur.
- Auth endpoint'leri IP bölümlü sabit pencere limiter ile dakikada 10 istekle
  sınırlıdır; ret cevabı `429 ProblemDetails` olur.
- Fiş analizi endpoint'i kimlik doğrulamadan sonra JWT `sub` değerine göre
  bölümlenen sabit pencere limiter ile kullanıcı başına dakikada 5 istekle
  sınırlıdır. Bu nedenle pipeline sırası authentication → rate limiter →
  authorization biçimindedir.
- Transaction liste/detay/yazma/iptal endpoint'leri owner-scoped Application
  use case'leri ve EF adapter'ıyla uygulanmıştır. Liste sorgusu optional
  filtrelerden önce `UserId` sınırını, pagination'dan önce deterministik
  `TransactionDate DESC, Id DESC` sıralamasını uygular.
- `Account.OpeningBalance` başlangıç noktasıdır; güncel bakiye ayrı kolon olarak
  tutulmaz. EF sorgusu açılış + gelir - gider formülünü iptal edilmemiş ve aynı
  kullanıcıya ait hareketler üzerinde hesaplar.
- Account, Category ve BudgetTransaction fiziksel silinmez. Hesap/kategori
  pasifleştirilir; hareket UTC zamanıyla idempotent iptal edilir. Düzeltme eski
  hareketi iptal edip yeni doğru hareket oluşturur.
- MonthlyBudget ilerlemesi saklanmaz; seçili ayın iptal edilmemiş giderlerinden
  `spent`, `remaining` ve `exceeded` projection'ı üretilir. Aylık rapor aynı
  owner-scoped hareket kaynağından gelir, gider, net ve kategori gruplarını;
  tüm zaman hareketlerinden hesap bakiyelerini üretir.
- Ortak pagination metadata API Contracts altında tutulur; para decimal string
  + currency, iş tarihi `yyyy-MM-dd` olarak taşınır.
- Mikroservis, MediatR/CQRS veya genel repository altyapısı eklenmez.
- SQL Server container yalnız `127.0.0.1:14334` üzerinden erişilir; verisi
  `business-finance-sqlserver-data` named volume'unda tutulur. Sabit
  `2025-CU7-ubuntu-24.04` image etiketi kullanılır.
- Compose health check'i kimlik doğrulamalı `SELECT 1` ile SQL motorunun hazır
  olduğunu doğrular; migration veya uygulama tablolarının hazır olduğunu
  göstermez.
- Hızlı API HTTP testleri factory-izole EF InMemory provider kullanır; bu provider
  ilişkisel constraint kanıtı sayılmaz. Ayrı SQL API testi gerçek provider ile
  register/login, hesap, kategori, gider hareketi, aylık bütçe ve rapor zincirini
  çalıştırır.
- Flutter bağımlılıkları `AppDependencies` composition root'unda kurulur.
  View → ViewModel/Controller → Repository → ApiClient yönü korunur; auth token
  yaşam döngüsü `AuthRepository` ve secure storage sınırında kalır.
- Finans para DTO'ları dört ondalıklı string olarak korunur. Dashboard toplamı,
  bakiye, kalan ve aşım istemcide ikinci kez hesaplanmaz; backend cevabı gösterilir.
- Gerçek finansal veri ve secret repository içinde tutulmaz.

## Finansal hareket mimarisi

```text
Transfer ───────────────┬─ kaynak Account bakiyesi: -tutar
                       ├─ hedef Account bakiyesi:  +tutar
                       └─ aylık gelir/gider:         0

CreditCardCharge ──────┬─ kart borcu:              +tutar
                       └─ aylık gider:              +tutar

CreditCardPayment ─────┬─ Account bakiyesi:        -tutar
                       ├─ kart borcu:               -tutar
                       └─ aylık gider:               0

InstallmentPlan ───────┬─ planlı item:               0 gerçek hareket
                       └─ realize(item) → CreditCardCharge
```

`Transfer` iki ayrı gelir/gider satırı değildir; aynı kullanıcıya ait iki hesabı
etkileyen tek iş olayıdır. Repository tek `SaveChangesAsync` çağrısıyla olayı
yazar. Bakiye sorgusu kaynak ve hedef yönünü ayrı toplar, rapor sorgusu transferi
gelir/gider toplamına katmaz. İptal fiziksel silme yapmaz ve hesaplanan etkileri
geri alır.

`CreditCard`, `Account` alt türü değildir. Kart limiti ve borcu bir yükümlülüğü,
Account bakiyesi eldeki varlığı temsil eder. Güncel kart borcu
`aktif harcamalar - aktif ödemeler`, kullanılabilir limit ise
`limit - güncel borç` olarak hesaplanır. Kart ödemesinin rapora yeniden gider
yazılmaması aynı satın almanın iki kez sayılmasını önler.

Ekstre dönemi önceki kesim gününün ertesi ile seçilen kesim günü arasındadır.
Önceki devir, dönem harcamaları, kesime kadar ödemeler ve kesim sonrası ödemeler
owner-scoped SQL toplamlarından alınır; Domain `Open`, `Paid` veya `Overdue`
durumunu seçilen `asOf` tarihinde üretir. Kalıcı bir ekstre snapshot'ı olmadığı
için geçmiş hareket düzeltmelerinde iki veri kaynağı uzlaşmazlığı oluşmaz.

`InstallmentPlan` ile `InstallmentItem` gelecekteki niyeti saklar; rapora doğrudan
girmez. Gerçekleştirme sırasında item'ın charge bağlantısı ile yeni
`CreditCardCharge` aynı `SaveChangesAsync` transaction'ında yazılır. Owner ve
`ClientRequestId` üzerindeki unique index aynı mobil retry'ın ikinci planı kalıcı
hale getirmesini engeller. Eşzamanlı aynı istek yarışında database constraint son
kapıdır; kullanıcı dostu conflict/retry eşlemesi ileride sertleştirilebilir.

Flutter finans feature'ı aşağıdaki bağımlılık yönünü korur:

```text
FinancePage → FinanceController → FinanceRepository → authenticated ApiClient
                                      ↓
                               açık Dart DTO'ları
```

Para JSON number'a çevrilmeden string tutulur. Form kontrolleri erken kullanıcı
geri bildirimi sağlar; sahiplik, limit, borç, sınıflandırma ve taksit matematiğinde
son otorite backend'dir.

## Kapsam boyutu

Şahıs şirketinde kasa ile cep hukuken ayrılmadığı için para tek havuzda yaşar;
kapsam o havuzu bölmez, yalnız gelir/gider raporlarını böler (ADR 0013).
`TransactionScope` iki değerlidir: `Business = 1`, `Personal = 2`. Üçüncü bir
"bilinmiyor" değeri **yoktur** — boyut boş bir veritabanına eklendi, yorumlanacak
bir geçmiş olmadığı için belirsizliği temsil edecek bir değere de ihtiyaç yok.

Kapsamı **zorunlu** taşıyanlar ve ortak ölçütü — gelir/gider raporunu etkileyen
ya da etkileyecek kayıt üreten her model:

| Model | Neden taşır |
|---|---|
| `BudgetTransaction` | Gelir/gider satırının kendisi |
| `CreditCardCharge` | Kart harcaması gideri anında tanır |
| `MonthlyBudget` | Sınırın hangi tarafa konduğunu söyler |
| `InstallmentPlan` | Gerçekleşen her item bir kart harcaması üretir |
| `RecurringTransaction` | Gerçekleşen her occurrence hareket ya da harcama üretir |
| `RecurringTransactionOccurrence` | Planın kapsamının üretim anındaki kopyası |
| `DebtAgreement` | Açılışı gider ya da gelir yazar |

Kapsam **taşımayanlar**: `Transfer` ve `CreditCardPayment`. İkisi de gelir/gider
raporuna sıfır etki eder (ADR 0002, ADR 0003); kapsam sormak, cevabı hiçbir yerde
kullanılmayan bir soru sormak olurdu.

`Account`, `Category` ve `CreditCard` **nullable** bir `DefaultScope` taşır. Boş
olması meşrudur ve eksik veri değildir: tek hesabıyla her şeyi yöneten esnaf için
kapsam kategoriden türer. Boş bırakmak "kapsamı bilmiyorum" değil, "bu kaynak
kapsamı belirlemiyor" demektir.

### Bölünen ve bölünmeyen

Kapsam **raporu böler, parayı bölmez**. Hesap bakiyesi, kart borcu ve net varlık
kapsam filtresinden etkilenmez; bunlar tek havuzun tutarıdır ve kullanıcının
cebindeki para kapsam anahtarının konumuna göre değişmez.

Bütçe ilerlemesi kapsama duyarlıdır ve bu kural **iki yerde birden** yazılıdır:
`MonthlyBudget.CalculateProgress` (bellek içi) ve `EfBudgetRepository`
(SQL). İkisi de harcamayı kategoriyle değil **kategori + kapsam çiftiyle**
toplar; aynı kategori hem işletme hem şahsi harcama tutabildiği için, ikisini
birden saymak kullanıcının koymadığı bir sınırı aşılmış gösterirdi.

### Plan kapsamı gerçekleşmede yeniden türetilmez

Tekrarlayan plan ve taksit planının ürettiği kayıt kapsamını **plandan** alır.
`RecurringTransactionOccurrence` kapsamı üretim anında plandan kopyalar — tutar,
tür ve açıklama gibi. Kopya olmasının ikinci bir faydası var: planlanan
projection kapsamı bir join olmadan SQL'de filtreleyebilir. Kapsamı gerçekleşme
anında yeniden türetmek, aynı planın farklı aylarda farklı kapsam üretmesi
demekti.

### Kalıcılık

Kapsam `tinyint` kolondur ve `[Scope] IN (1, 2)` CHECK kısıtıyla korunur;
`DefaultScope` nullable'dır ve `[DefaultScope] IS NULL OR [DefaultScope] IN (1, 2)`
ile. Kolonları ekleyen `AddTransactionScope`, `InitialCreate`'ten sonra zincirin
**ilk gerçek yükseltme adımıdır** (ADR 0012'nin bir daha kullanılmayacağını
yazdığı serbestlik burada bitti). Zorunlu kolonlar varsayılansız `NOT NULL`
eklenir; bu, migration kurallarının istisnası değil ön koşulunun sağlanmış
hâlidir — tablolar Aşama 01 Grup 1'de boşaltıldı. Varsayılansız ekleme aynı
zamanda o ön koşulu denetler: tablo boş değilse SQL Server komutu reddeder ve
yükseltme sessizce yanlış veri üretmek yerine durur.

## Planlama ve read-model mimarisi

```text
RecurringTransaction (tanım)
        │ generate(throughDate), occurrence key ile idempotent
        v
RecurringTransactionOccurrence (planned)
        │ açık kullanıcı onayı / atomik realize
        v
BudgetTransaction (gerçek finans olayı)

UpcomingPayments read model
  ├─ planned recurring occurrence
  ├─ son kapanmış kredi kartı ekstresi
  └─ gerçekleşmemiş installment item

AdvancedReport read model
  ├─ hesap hareketleri + transfer + kart ödemesi → likit varlık
  ├─ card charge - card payment → kart borcu
  ├─ aylık gelir/gider + kart charge → dönem/trend/bütçe
  └─ upcoming candidate → gelecek ödeme yükü
```

Recurring tanımı bakiye veya rapor etkisi üretmez. Dönem anahtarı owner
kapsamında unique olduğu için aynı üretim aralığının retry edilmesi ikinci kayıt
oluşturmaz. Occurrence gerçekleşirken durum değişikliği ve `BudgetTransaction`
aynı SQL `SaveChanges` sınırındadır; yarım onay bırakılmaz. Cloud scheduler yoktur;
üretim açık use case olarak çağrılır.

Raporlar kalıcı ikinci bakiye kaynakları değildir. Bütün toplamlar owner-scoped,
`AsNoTracking` projection/aggregate sorgularından hesaplanır. Yaklaşan kart
ekstrelerinde kart başına SQL çağrısı yapılmaz: kart dönemleri bellekte belirlenir,
charge/payment verileri tarih penceresinde toplu alınır ve `CardId` lookup'larıyla
Domain ekstre hesabına verilir. `MonthlyBudgets(UserId, Year, Month)` indexi gerçek
rapor filtresini destekler.

Flutter bağımlılık yönü şöyledir:

```text
PlanningPage → PlanningController → PlanningRepository → authenticated ApiClient
      │                 │                    │
Material 3 +        yükleme/hata/         açık Dart DTO,
Semantics +         unauthorized/         money string,
metin özeti         stale snapshot        API query/body
```

Grafik yalnız görselleştirme için `double` kullanır; finansal tutar hesapları ve
HTTP sözleşmesi dört ondalıklı string kalır. `CustomPainter` ek paket gerektirmez,
Material color scheme kullanır ve aynı veri görünür aylık metin listesi ile
Semantics açıklamasında tekrar sunulur. Offline persistence yoktur; stale-cache
yalnız controller belleğindeki son başarılı snapshot'tır ve banner ile işaretlenir.

## Doğrulanan teknik kararlar

- Solution biçimi: `.slnx`
- İlk .NET hedef framework'ü: `net10.0`
- Nullable reference types: Açık
- Implicit usings: Açık
- Domain tipi ayrımı: değerler için `record`, kimlik sahibi entity'ler için
  gerektiğinde `class`
- Kullanıcı sahipliği: Domain'de Identity bağımlılığı olmadan `Guid UserId`
- Identity tipi: Infrastructure'da `ApplicationUser : IdentityUser<Guid>`
- Access token: HS256 JWT, 15 dakika, `sub`/`email`/`jti`
- Refresh token: 64 rastgele byte, sunucuda SHA-256 hash, 30 günlük varsayılan ömür
- Object-level authorization: `AccountId + current UserId` kapsamlı lookup ve update
- İşlem sınırı: aynı kullanıcıya ait aktif hesap ve kategori, uyumlu
  gelir/gider türü ve pozitif `Money`
- Bütçe sınırı: aktif gider kategorisi, ay/yıl dönemi ve aynı
  kullanıcı/kategori/dönemdeki giderlerden hesaplanan ilerleme
- Domain doğrulaması: Release 128/128 unit test; solution build başarılı
- Kaynak kontrol yaklaşımı: Tek amaçlı feature branch ve küçük checkpoint'ler

## Güven sınırları

| Sınır | Güvenilmeyen girdi | Uygulanan kontrol |
|---|---|---|
| Flutter → API | JSON, filtreler, kimlik iddiası | DTO validation, bearer doğrulama, current-user sorgusu |
| API → Application/Domain | HTTP ve route değerleri | Use case, Domain invariant'ı, `decimal` para |
| Application → SQL | Kullanıcıya ait kimlikler | Owner predicate, composite FK, unique constraint |
| Cihaz depolaması | İstemci state'i | Secure storage; parola yok; logout/401 temizliği |
| Local host → dış ağ | Yanlış bind/firewall | API ve SQL loopback bind; internet yayını yok |
| API → Gemini | Fiş byte'ları ve güvenilmeyen model yanıtı | Sunucuda secret, `store=false`, timeout/retry, yapılandırılmış JSON, altı kademe doğrulama; fotoğraf/yanıt loglanmaz veya sağlayıcı etkileşim geçmişinde tutulmaz |

Native Android istemci tarayıcı olmadığı için CORS güvenlik sınırı değildir.
Debug HTTP yerel geliştirme içindir; release/production HTTPS ve signing ayrı
yayın kapısıdır. `/health/live` process'i, `/health/ready` SQL erişimini ölçer.

## Bilinen riskler ve varsayımlar

- Fiziksel cihaz, gerçek Wi-Fi/USB, üretici Android katmanı ve hardware-backed
  secure storage test edilmedi; haricî testten önce zorunlu kapıdır.
- Release yapılandırması geliştirme application ID'si ve debug signing kullanır;
  mevcut debug APK production artefact değildir.
- İstemci hızlı çift submit'i kilitler, fakat transaction POST için sunucu
  idempotency key'i yoktur. Ağ seviyesindeki tekrar riski, offline yazma kararıyla birlikte değerlendirilir
  (`PROJECT-ROADMAP.md`).
- Database row-level security yoktur; izolasyon owner predicate ve composite
  foreign key'lerle sağlanır.
- Offline cache/sync yoktur; planlama ekranı yalnız process belleğindeki
  son snapshot'ı ağ hatasında açık “son güncel veri” uyarısıyla gösterebilir.

E-posta gönderimi, scheduled/background job, public/indexable web/SEO ve gömülü
agent/LLM/webhook otomasyonu yoktur; bunlar için ayrı shipping belgesi oluşturulmaz.

## İlgili belgeler

- [Kullanıcı ve hata akışları](flows.md)
- [İzin ve izolasyon](permissions.md)
- [Değişkenler ve secret sınırı](variables.md)
- [Test kapsam haritası](tests.md)
- [Yerel kurulum ve kabul](local-setup-and-acceptance.md)
- [HTTP API sözleşme kararı](adr/0001-http-api-contracts.md)
- [Transfer ve raporlama kararı](adr/0002-transfer-reporting.md)
- [Kredi kartı, ekstre ve taksit kararı](adr/0003-credit-card-statements-installments.md)
- [Backup restore runbook](restore-runbook.md)
- [Fiş analizi API sözleşmesi](receipt-analysis-api-contract.md)
- [Fiş okuma öneri katmanıdır kararı](adr/0011-receipt-reading-is-a-suggestion-layer.md)

## Fiş analizi backend mimarisi

```text
multipart JPEG/PNG + JWT
        │
        ├─ IAttachmentFileInspector (MIME + magic byte + EICAR)
        ├─ IReceiptImagePreprocessor (EXIF + ölçek + yumuşak kontrast)
        ├─ ICategoryRepository (current user + expense + active)
        ├─ IReceiptAnalyzer ──HTTPS──> Gemini Interactions API
        └─ ReceiptDraftValidator (şekil → tip → anlam → tutarlılık → sahiplik → karar)
                                 │
                                 └─ ReceiptDraft; kalıcı yazma yok
```

`AnalyzeReceiptUseCase` Application katmanındadır ve akışı orkestre eder.
Görüntü ön işleme ile Gemini istemcisi Infrastructure adapter'larıdır; Domain'e
fiş veya AI tipi eklenmemiştir. Use case'in transaction repository,
`DbContext`, attachment store veya unit-of-work bağımlılığı bulunmaz. Bu
negatif bağımlılık analiz endpoint'inin öneri üretirken finansal kayıt
yazamamasını mimari olarak da sınırlar.

Çift kayıt uyarısı bu sınırı **bozmadan** geçer:
`IReceiptDuplicateLookup` tek soruya ("bunu zaten kaydettim mi?") cevap veren
salt-okunur bir porttur. Tam repository'yi vermek, yapısal bir güvenceyi bir
yorum satırıyla değiştirmek olurdu. Fotoğraf parmak izi bilerek kullanılmıyor:
saklanan belge **orijinal**, analize giden ise küçültülmüş kopyadır — iki hash
hiçbir zaman eşleşmez — ve saklanmayan fotoğrafların hash'ini hatırlamak analiz
uç noktasının durum yazması demekti.

Bu katmanın kalıcı kararları — modelin karar verici değil öneri katmanı olması,
yönün (gider/gelir) belgeden okunmayıp üründe sabitlenmesi, belge türü kapısının
iddiaya değil reddetmeye çalışması, kategorinin kapalı bir şema `enum`'undan
seçilmesi, ödeme kaynağının hiçbir koşulda modelce seçilmemesi, aritmetik
yasağı ve olasılık skoru yerine üç durumlu alan güveni — 
`adr/0011-receipt-reading-is-a-suggestion-layer.md` belgesindedir.

Uç nokta önce bildirilen ve gerçek byte uzunluğunu karşılaştırır, ardından
mevcut attachment denetçisini yeniden kullanır. Denetçi güvenli PDF'i genel
belge yüklemesinde kabul edebilir; fiş akışı bunun üstüne ayrı JPEG/PNG kapısı
koyar. Normalize edilen görsel sağlayıcıya gider, orijinal byte'lar bu istekte
saklanmaz. Sağlayıcı kullanımı ve normalizasyon ölçümleri Application sonucunda
ölçüm turu için taşınır fakat HTTP cevabına veya kalıcı depoya yazılmaz.

`BusinessFinance.ReceiptMeasurement` yalnız manuel sentetik veri toplama
aracıdır. Üretimdeki Infrastructure adapter'larını doğrudan kullanır; sonuçları
Git dışındaki `artifacts/` alanına artımlı yazar ve uygulama/API dependency
graph'ına eklenmez.

Yanıt `ReceiptDraft`tır: para dört ondalıklı string, tarih `yyyy-MM-dd`, alan
durumu `read/suspect/missing` ve kategori owner-scoped kimliktir. Model çıktısı
veri kabul edilmez; saf doğrulayıcıdan geçer. Finansal yazma sonraki Flutter
onay akışında mevcut transaction veya card-charge endpoint'ine yapılacaktır.

### Fiş yakalamanın istemci tarafı (Grup 5)

```text
ReceiptScanController -> ReceiptImageSource   (image_picker: kamera | galeri)
        |                      \-> orijinal byte'lar
        |-> ReceiptImageNormalizer (isolate: EXIF bake, uzun kenar 2400, JPEG q85)
        \-> ReceiptRepository -> ApiClient.postMultipart(75 sn) -> /receipts/analyze
```

Fotoğrafın **iki hâli** taşınır ve biri diğerinin yerine geçmez: analize
küçültülmüş kopya gider, belge olarak saklanacak olan orijinaldir (Grup 7).
Bu yüzden `image_picker`'a `maxWidth`/`imageQuality` verilmez — verilseydi
eklenti yalnız küçültülmüş yeni dosyayı döndürür ve orijinale erişim kalmazdı.

Küçültme saf Dart'ta (`image` paketi) ve `compute` ile ayrı isolate'te yapılır;
aynı iş `normalizeSync` ile testten doğrudan çağrılabilir. Boyut merdiveni önce
kaliteyi, sonra çözünürlüğü düşürür ve son basamakta yine de sınırı aşarsa
dosyayı gönderir: asıl kapı sunucunun 5 MiB sınırıdır, istemci onun yerine
karar vermez.

İptal, isteği gerçekten kesmez — sunucu okumayı bitirir — yalnız sonucu
geçersiz kılar. Controller bunu bir koşum sayacıyla yapar ve arayüz iptali
"istek durduruldu" diye anlatmaz.

Bu tek çağrının istemci zaman aşımı 75 saniyedir: sunucunun sağlayıcıya
tanıdığı 60 saniyenin üstüne yükleme payı ekler. Varsayılan 15 saniyelik
istemci bütçesi, sunucu fişi okumuşken isteği keserdi.

## Import ve dosya mimarisi

```text
CSV bytes -> strict parser -> ImportBatch/Rows -> mapping/duplicate review
                                              -> atomic BudgetTransaction confirm

DebtAgreement -> generated DebtInstallments -> payment account movement
SavingsGoal   -> exactly one progress source: account OR contributions

multipart attachment -> signature/threat inspector -> local object store bytes
                                               \-> SQL owner-scoped metadata

schema v2 backup -> checksum + strict graph -> GUID remap -> Serializable SQL
                                      \-> attachment re-scan/object compensation
```

Import staging gerçek transaction üretmez. Confirm seçilmiş ve ready satırları
tek SQL transaction'da üretir; rowversion aynı satırın yarışan confirm'ini engeller.
Borç ödemesi hesap likiditesini etkiler fakat income/expense değildir. Hedefte
account balance ve manual contribution kaynakları karşılıklı dışlanır.

Attachment storage `IAttachmentObjectStore` portudur. Uygulanan local adapter
`AttachmentStorage:RootPath` altında çalışır; SQL yalnız metadata tutar. Object
store ile SQL arasında distributed transaction yoktur, save hatasında compensating
delete uygulanır. Backup schema v2 attachment byte'ını da taşıdığı için payload
10 MiB sınırı tüm içerik toplamına uygulanır.

Flutter bağımlılık yönü:

```text
DataToolsPage -> DataToolsController -> DataToolsRepository -> ApiClient
 file_selector      state/error          explicit DTO       JSON/multipart/binary
 share_plus         submit lock          endpoint map        bearer + one 401 retry
```

## Presentation katmanı tasarım sistemi

Bu aşama Domain, Application, Infrastructure ve API katmanlarına dokunmaz;
yalnız Flutter presentation katmanının içini düzenler. Katman ayrımı
(`Page → Controller → Repository → ApiClient`) değişmemiştir.

```text
lib/core/theme/            token'lar
├── app_spacing.dart       boşluk ölçeği + fabClearance
├── app_radius.dart        köşe yarıçapları
├── app_breakpoints.dart   pencere sınıfları + context.windowSize
├── app_finance_colors.dart ThemeExtension (bkz. ADR 0006)
└── app_typography.dart    sabit genişlikli rakam

lib/core/widgets/          ortak bileşenler
├── app_money_text.dart        tutar: renk + hizalama + ekran okuyucu cümlesi
├── app_status_chip.dart       durum rozeti (ikon + etiket zorunlu)
├── app_section_header.dart    başlık (semantics header)
├── app_submit_button.dart     gönderim kilidi
├── app_confirm_dialog.dart    açık onay
├── app_content_width.dart     geniş ekranda okunabilir genişlik
├── app_adaptive_sheet.dart    sheet / dialog seçimi
├── app_responsive_grid.dart   sütun sayısı hesabı
└── app_state_views.dart       loading / error / empty / unauthorized
```

Kalıcı karar: finansal anlam renkleri `ColorScheme` rollerine bindirilmez,
ayrı bir `ThemeExtension` taşır — `adr/0006-financial-color-theme-extension.md`.

Kuralların uygulanması iki kapıya bağlıdır: kontrast ve pencere sınıfı birim
testleriyle, "ham renk / ölçek dışı boşluk / doğrudan genişlik karşılaştırması
yok" kuralı ise kaynak kodu tarayan yapısal bir testle
(`test/architecture/design_tokens_test.dart`). İkincisi backend'deki mimari
testlerin Flutter tarafındaki karşılığıdır: ihlal her zaman henüz testi olmayan
yeni bir dosyada ortaya çıkar.

Ayrıntı ve yeni ekran kontrol listesi: `documentation/design-system.md`.

## Borç açılışı ve faiz mimarisi

Borç, para hareketi olan kavramlar arasında **hiçbir yazma modeli
üretmeyen** tek kavramdı. Taksit ödemesi yalnız `PaymentAccountId` yazıyordu ve
bakiye etkisi altı ayrı repository'de elle yazılmış LINQ ile hesaplanıyordu;
borcun *açılışı* ise hiçbir kayıt üretmiyordu. Sonuç, bakiye ile gider
raporunun birbirini tutmamasıydı.

### Açılış birinci sınıf bir olay

`DebtAgreement` artık bir `DebtSourceType` taşır ve tam olarak bir kaynağı
doludur — `RecurringSourceType` deseninin aynısı, hem Domain invariant'ı hem
SQL check constraint'i:

```text
Cash      -> OpeningAccountId dolu, CategoryId boş
             açılış: hesap ± anapara, gelir/gider yok
Expense   -> CategoryId dolu, OpeningAccountId boş  (yalnız borç yönünde)
             açılış: kategorili gider + anapara, bakiye etkisi yok
Unrecorded-> ikisi de boş; yalnız migration ve şema 4 öncesi yedekler üretir
```

Ayrı bir tablo eklenmedi: sözleşmenin kendisi açılış kaydıdır, tıpkı
`CreditCardCharge`'ın harcamanın kendisi olması gibi. Okuma modelleri bu
satırları birleştirir.

### Faiz

Anüite (azalan bakiye), nominal yıllık oran, aylık = yıllık / 12. Saf Domain'de
`AmortizationSchedule`; iki yön de var, ters yön kapalı formülü olmadığı için
ikiye bölme kullanır. Para kanonik, oran türetilmiştir.

Taksit başına anapara/faiz payı **saklanır**. Bu bir projeksiyon önbelleği
değil, sözleşme anında sabitlenen bir özelliktir; aylık gider raporu faiz
payını SQL'de toplamak zorunda ve anüite ayrımı EF LINQ'e çevrilmiyor.
Ayrımı olmayan (kolonlardan önce yazılmış) satırlar sıfır faiz katar.

Gerekçelerin tamamı: `adr/0009-debt-source-and-annuity-interest.md`.

### Net varlıkta borç kalan anaparadır

`netWorth = likit varlık − kart borcu + kalan alacak anaparası − kalan borç
anaparası`. Gelecekteki faiz yükümlülük sayılmaz: doğmamış bir maliyettir ve
ödendiği ay hem gidere yazılır hem borcu azaltır, iki kez düşülemez. Ayrımı
olmayan bir taksitte tamamı anapara sayılır — bilinmeyen bir ayrımda borcu
olduğundan büyük göstermek, yok saymaktan güvenlidir.

`DebtResponse.RemainingAmount` bundan ayrıdır ve kalan **ödemelerin** toplamı
olmaya devam eder: o alan "daha ne kadar ödeyeceğim" sorusunu yanıtlar.

Gerekçe: `adr/0010-net-worth-measures-debt-at-principal.md`.
