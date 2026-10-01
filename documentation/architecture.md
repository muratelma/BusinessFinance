# Mimari Kayıt

## Ürün özeti ve temel varsayımlar

BusinessFinance; şahıs şirketi ve esnaf için işletme finansı uygulamasıdır.
Hesap, kategori, gelir/gider, kredi kartı ve taksit, tekrarlayan plan,
borç/alacak, hedef, birleşik finansal hareket akışı, CSV içe/dışa aktarma,
yedekleme ve fiş okuma akışlarını sunar. Katmanlı monolit ASP.NET Core API
finansal gerçeğin kaynağıdır; Flutter istemci sonuçları gösterir; SQL Server
kalıcı depodur. Bugün yalnız sentetik veri ve tek host varsayılır; Android
akışları Pixel 8 emulatoründe, dar web denemesi yerel Chrome/Edge üzerinde
çalışır. Ürün internete açık değildir.

```text
Pixel 8 app --HTTP debug / 10.0.2.2:5284--> Windows loopback API
Local web  --HTTP debug / localhost:5284----^ (Development CORS)
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
  `spent`, `remaining` ve `exceeded` projection'ı üretilir.
- **MonthlyBudget silinebilir** (Aşama 06 Grup 7) ve bu "silme yerine iptal"
  kuralının bir istisnası değildir: o kural para hareketlerini korur, bütçe ise
  bir olay değil kullanıcının kendine koyduğu bir **sınırdır**. Hiçbir
  bakiyeyi, raporu veya net varlığı beslemez; silinmesi hiçbir tutarı
  değiştirmez. Pasifleştirme yolu bırakılmadı çünkü tekil indeks
  (`UserId, CategoryId, Year, Month`) o kategoriyi ay boyunca kapatıyor —
  yanlış kurulmuş bir sınırın doğrusu kurulamıyordu. Aylık rapor aynı
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

**Kart borcu negatif olabilir ve kırpılmaz** (Aşama 06 Grup 5). Ödeme borcu
aşamaz — uygulama bunu reddeder — ama ödenmiş bir harcamanın **iptali** kartı
alacaklı bırakır. O tutar kullanıcının parasıdır: net varlığa girer
(`- kart borcu` teriminde negatif bir borç varlığı artırır) ve kullanılabilir
limiti limitin üstüne çıkarır. Eskiden borç kart başına `Math.Max(0, …)` ile
sıfıra çekiliyordu; bu, cari hesabın "fazla tahsilat kırpılmaz" kararıyla
doğrudan çelişen ikinci bir cevaptı.

**Taban yalnız iki yerde durur ve ikisi de ayrı bir soruyu cevaplar:**
kullanılabilir limit limiti aşmış kartta sıfırdır ("eksi harcayabilirsiniz"
diye bir şey yok), ekstre borcu da negatif olmaz — ekstre "bu ay ne kadar
ödemeliyim"in cevabıdır. Ekstrenin **devri** ise kırpılmaz: geçen dönemden
kalan alacaklı bakiye bu dönemin ekstresinden düşer, yoksa kartında parası olan
kullanıcıdan borçlu olmadığı bir tutar istenirdi.

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

### Türetme zinciri

Kapsam sunucuda **tek bir yerde** çözülür (`TransactionScopeResolution`) ve sıra
her oluşturma yolunda aynıdır:

```text
kullanıcının açık seçimi → hesabın/kartın etiketi → kategorinin varsayılanı
```

Sıra keyfi değil, en özelden en genele gider. Kullanıcının o kayıt için yazdığı
şey her şeyi yener. Hesap ya da kart kategoriden daha çok bilgi taşır: dükkânın
kasası hangi kategoriye girerse girsin işletmenin parasıdır. Kategori paylaşılan
bir kovadır ve en zayıf ipucudur.

**Üçü de boşsa istek reddedilir.** Sunucu kapsam uydurmaz; yanlış etiketlenmiş
bir kayıt kullanıcının işletme netini sessizce bozar ve düzeltilene kadar fark
edilmez. Hata kodları özelliğe göredir: `transactions.scope_unresolved`,
`budgets.scope_unresolved`, `credit_cards.scope_unresolved`,
`recurring.scope_unresolved`, `debt.scope_unresolved`,
`imports.scope_unresolved`.

Bütçenin bir hesabı yoktur; zinciri açık seçim ve kategori ile sınırlıdır.
Aşama 06 Grup 7'ye kadar istemci bu halkayı hiç göstermiyordu: kategorisinin
varsayılanı olmayan işletme kullanıcısında istek `budgets.scope_unresolved` ile
reddediliyor ve ekranda sunucunun İngilizce cümlesi görünüyordu. Zincir artık
formda önizleniyor ve çözülemediğinde **istek gitmeden** söyleniyor.

Bütçe **kategori + kapsam çiftini** sınırlar ama tekil indeksi kapsam
taşımaz: bir kategorinin bir ayda tek bütçesi olur ve o bütçe tek bir tarafı
sınırlar. İkiye bölmek (indekse kapsamı eklemek) bilinçli olarak
**yapılmadı** — bunun yerine satır hangi tarafı sınırladığını yazıyor, çünkü
sessiz bir yarım toplam, olmayan bir ikinci limitten daha yanıltıcıydı. CSV
içe aktarmada ilk halka hiç dolmaz — dosyada kapsam kolonu yok — kalan iki halka
aynen işler.

### İşletme kimliği ve iki kategori seti

Kaydolurken tek bir soru sorulur: işletmeniz var mı. Cevap `UserProfile`
tablosunda, kullanıcı başına tek satırda durur — cihazda değil, çünkü uygulamayı
silip yeniden kuran ya da ikinci cihazdan giren kullanıcı işletme sahibi olmayı
kaybetmemeli. Profili olmayan kullanıcı "işletmesi yok" sayılır; bu bir varsayım
değil, sorunun sorulmadığı hâlin doğru cevabıdır.

Cevap **hiçbir özelliği kapatmaz.** Yalnız iki şeyi belirler: hangi varsayılan
kategori setiyle başlanacağı ve kapsam boyutunun arayüzde görünüp
görünmeyeceği. İşletmesi olmayan kullanıcı için kapsam gerçek bir soru
değildir — her kaydı şahsidir — ve ona bir anahtar göstermek, cevabı belli olan
bir soruyu her ekranda tekrar sormak olurdu.

İki set de her kalemine bir varsayılan kapsam koyar; **türetme zincirinin son
halkasını dolduran budur**. Kişisel setin tamamı `Şahsi`. İşletme seti iki
parçalıdır ve olmak zorundadır: işletme kalemleri (`İşletme`) ve patronun
gündelik hayatı için derli toplu bir şahsi alt küme (`Şahsi`). Esnafın market
alışverişi de aynı uygulamaya giriyor (ADR 0013); yalnız işletme kalemleri
koymak, kullanıcıyı ilk şahsi harcamasında kategori uydurmaya zorlardı.

Set **yalnız hiç kategorisi olmayan kullanıcıya bir kez** uygulanır. Cevabını
sonradan değiştiren kullanıcının kategorileri değişmez: o noktada liste artık
kullanıcınındır ve sildiği bir kategoriyi geri getirmek silme eylemini anlamsız
kılardı. Bu yüzden cevap kayıt anında yazılır — ilk kategori okuması setin hangi
olacağını o satırdan öğrenir.

**Ödeme yöntemi kategori değildir**: "Satış geliri (kart)" gibi bir kalem
açılmaz, tahsilatın kartla mı nakit mi olduğu hesaptan bellidir. Bir test set
adlarında ödeme yöntemi kelimelerini arıyor.

### Bölünen ve bölünmeyen

Kapsam **raporu böler, parayı bölmez**. Hesap bakiyesi, kart borcu ve net varlık
kapsam filtresinden etkilenmez; bunlar tek havuzun tutarıdır ve kullanıcının
cebindeki para kapsam anahtarının konumuna göre değişmez.

Kapsam filtresi alan okuma modelleri ve **almayanlar**:

| Okuma | Kapsam filtresi | Neden |
|---|---|---|
| Aylık rapor gelir/gider ve kategori dağılımı | **Alır** | Bölünen şey budur |
| Aylık rapordaki hesap bakiyeleri | Almaz | Kasadaki para tek havuz |
| Gelişmiş rapor dönem karşılaştırması, nakit akışı, bütçe sapması | **Alır** | Gelir/gider tarafı |
| Gelişmiş rapor net varlık, hesap ve kart dağılımı | Almaz | Tek havuz; filtreye takılsaydı "ne kadar param var" sorusunun aynı anda iki doğru cevabı olurdu |
| Birleşik feed | **Alır**, tek SQL sorgusunda | Geçmişin bir tarafını okumak |
| Planlanan görünüm | **Alır**, her kaynağın kendi sorgusunda | Aynı gerekçe |
| Yaklaşan ödemeler | Almaz | Ödenecek para tek havuzdan çıkar; anahtarın konumu neyi ödeyeceğini değiştirmez |

**Kapsam filtresi kapsamsız satırları da eler.** Transfer, kart ödemesi ve kart
ekstresi kapsam taşımaz; ikisini birden her iki kapsamda göstermek, kullanıcı
tarafları karşılaştırdığında aynı para hareketini iki kez saydırırdı. Bu yüzden
iki tarafın toplamı, filtresiz okumanın toplamından küçüktür ve olması gereken
budur.

Bütçe ilerlemesi kapsama duyarlıdır ve bu kural **üç yerde birden** yazılıdır:
`MonthlyBudget.CalculateProgress` (bellek içi), `EfBudgetRepository` (bütçe
listesi) ve `GetBudgetVariancesAsync` (gelişmiş rapor). Üçü de harcamayı
kategoriyle değil **kategori + kapsam çiftiyle** toplar; aynı kategori hem
işletme hem şahsi harcama tutabildiği için, ikisini birden saymak kullanıcının
koymadığı bir sınırı aşılmış gösterirdi.

### Plan kapsamı gerçekleşmede yeniden türetilmez

Tekrarlayan plan ve taksit planının ürettiği kayıt kapsamını **plandan** alır.
`RecurringTransactionOccurrence` kapsamı üretim anında plandan kopyalar — tutar,
tür ve açıklama gibi. Kopya olmasının ikinci bir faydası var: planlanan
projection kapsamı bir join olmadan SQL'de filtreleyebilir. Kapsamı gerçekleşme
anında yeniden türetmek, aynı planın farklı aylarda farklı kapsam üretmesi
demekti.

### İstemcide kapsam: tek anahtar, düzeltilebilir çip

Kapsam denetimi Flutter tarafında **tek** yerdedir: `ScopeController`
(`lib/core/presentation/scope_controller.dart`). İki şeyi tutar — anahtarın
konumu (`Hepsi · İşletme · Şahsi`) ve onboarding cevabı. Sekme başına ayrı
filtre yoktur (ADR 0013); bölünen ekranlar aynı denetimi dinler ve konum
değişince kendilerini yeniden okur.

Denetim iki dar bağımlılık alır: cihaz deposu (`ScopeStore`, uygulamada
`ScopePreferences` — `flutter_secure_storage`, `ReceiptPreferences` ile aynı
gerekçe) ve profili okuyan tek fonksiyon. Onboarding cevabının **kaynağı
sunucudur**; cihazdaki kopya yalnız sunucu okunamadığında boyutun sessizce
kaybolmasını engeller. Profil isteği düşerse en son bilinen cevap geçerli kalır:
"hayır" varsaymak, işletme sahibinin boyutunu bir ağ hatasına kurban ederdi.

Oturum kapanınca seçim ve cevap **unutulur** (`forget`); aynı cihazdan giren
ikinci kullanıcı birincisinin anahtar konumunu devralmaz. Bağlama işi
kompozisyon kökündedir (`BusinessFinanceApp`): kimliği ve kapsamı birlikte
tanıması gereken tek yer orasıdır.

Kapsam **`FinancialDataChanges`'e bağlanmaz**. O sinyal "veri değişti" der;
anahtar veriyi değiştirmez, aynı veriye başka bir soru sorar. Oraya bağlansaydı
her kapsam dokunuşu, kapsamdan etkilenmeyen ekranları (hesaplar, kartlar) da
boşuna yeniden yükletirdi.

Bölünen ekranların başlığında aktif kapsam yazılıdır (`İşlemler · İşletme`);
bölünmeyen bölümler (net varlık, hesap bakiyeleri) filtre açıkken **toplam
gösterdiklerini yazar**. Sessizce aynı kalan bir sayı, filtrelenmiş sanılır ve
kullanıcı iki tarafı toplamaya çalışır.

**Formdaki çip zincirin önizlemesidir.** Kararın sahibi sunucudur
(`TransactionScopeResolution`); form aynı sırayı (açık seçim → kaynağın etiketi
→ kategorinin varsayılanı) yalnız **gösterebilmek** için uygular ve gösterdiği
değeri açıkça gönderir. Çip boş dursaydı kullanıcı kaydın hangi tarafa
yazıldığını ancak listeye düştükten sonra görürdü. Zincir çözülemezse form
sunucuya gitmeden durur ve alanın yanında söyler — sunucu da reddederdi
(`*.scope_unresolved`), ama hata kullanıcının düzeltebileceği yerde görünmeli.
Kapsam boyutu görünmeyen kullanıcıda alan hiç çizilmez ve istek kapsam
göndermez; sunucu kategoriden türetir.

### Ayın iki tarafı: özet ekranının hero metriği

Kapsam varken tek bir "net" hangi neti sorduğunu söylemiyordu. Aylık rapor bu
yüzden filtresiz okunduğunda **kırılım** da döndürür: işletme ve şahsi tarafın
gelir/gider/net tabloları ayrı ayrı (`scopeBreakdown`). İki tarafın toplamı
raporun kendi toplamına eşittir — gelir/gider üreten her kayıt tam olarak bir
kapsam taşır ve üçüncü bir kova yoktur.

Kırılım **sunucudan hazır gelir** çünkü istemci finansal toplamı ikinci kez
hesaplamaz. "İşletme neti" ile "şahsi net" bir çıkarma değil, ayrı ayrı
toplanmış iki tablodur; istemci çıkarsaydı ekrandaki sayı sunucununkiyle
tutmayabilirdi.

Kırılım **yalnız filtresiz okumada** döner. Kapsam verilmişse rapor zaten tek
tarafı anlatıyordur; kırılım göndermek dışlanan tarafı sıfır gösterip "o
tarafta hiç hareket yok" dedirtirdi.

Toplamlar `SUM` yerine kapsama göre `GROUP BY` ile okunur; sorgu sayısı
değişmez (en fazla iki satır döner ve toplam onların toplamıdır). Kırılımı
ikinci bir tur sorguyla almak, özet ekranının ilk isteğini iki katına
çıkarırdı.

Ekranda üç sayı durur: **işletme neti**, **şahsi net** ve **bu ayın neti**
(hero). Üçüncüsü ilk ikisinin toplamıdır ve nakit hareketi değildir — kart
harcaması harcandığı gün gider yazılır, ödendiği gün değil. Şahsi tarafın adı
sayının yönüne göre değişmez: eskiden eksi ayda `Şahsi çekim` yazıyordu;
tasarım teslimiyle (27 Eylül 2026) tek ad `Şahsi net` oldu, yön işaretten ve
renkten okunur. **"Kâr" kelimesi hiçbir yerde kullanılmaz** —
muhasebe kârı satılan malın maliyetini ister ve ürün sınırının dışındadır.

Kapsam boyutu görünmeyen kullanıcıda ekran bugünkü davranışını korur: tek `Bu
ayın neti`. Bir taraf seçiliyken hero o tarafın netini adıyla gösterir.

## Cari hesap: karşı taraf ve açık bakiye

> Aşama 02 ve Aşama 03 Grup 2 — domain, kalıcılık, vade bazlı bakiye
> projection'ı, borç modelinin bağlanması ve **yazma yolu + feed + raporlar**.

Karşı taraf (`Counterparty`) müşteri, tedarikçi ya da ikisi birden. **Ayrı tip
yok:** mahalle esnafında aynı kişi hem alıcı hem satıcıdır ve ikiye bölmek
"Ahmet'le hesabım ne?" sorusunu cevapsız bırakırdı. Yön kaydın kendisinde durur.
Tutulan alanlar ad ve kullanıcının kendi notu; adres, vergi numarası ve telefon
Aşama 02'nin kapsamı dışında.

İki hareket türü ADR 0014'ü birebir uygular:

| Kayıt | Tanır | Taşır | Kategori | Kapsam | Vade |
|---|---|---|---|---|---|
| `CounterpartyCharge` | **Gelir/gider** | Hayır | **Zorunlu** | **Zorunlu** | İsteğe bağlı |
| `CounterpartyPayment` | Hayır | **Hesap bakiyesi** | Yok | Yok | Yok |

`DebtDirection` yeniden kullanılıyor çünkü sorduğu soru aynı: yükümlülük kimin
üzerinde. Borçlandırmada yön kategorinin türünü **belirler** — alacak doğuran
kayıt gelir kategorisi, borç doğuran kayıt gider kategorisi ister. Yön ile
kategori birbirini tutmazsa kayıt raporun yanlış tarafına düşerdi, o yüzden
tutmaması bir hata değil, reddedilen bir istek.

Tahsilatın kategori ve kapsam taşımaması ikisi de bilinçli ve bir test alanların
sonradan eklenmediğini koruyor: kategori "ne satıldı" sorusunu cevaplar ve o
soru borçlandırmada sorulmuştur; kapsam gelir/gider raporunu böler ve tahsilat
o rapora hiç girmez (ADR 0013). Kart ödemesinin ikisini de taşımamasıyla aynı
yapı.

**Cari bakiye kalıcı kolon değildir.** `CounterpartyBalance` hareketlerden
hesaplar: iki taraf ayrı ayrı durur (`Receivable`, `Payable`) ve `Net` ikisini
tek cümleye indirir. İptal edilmiş hareket hiç sayılmaz. **Fazla tahsilat
kırpılmaz**: eksiye düşen taraf, karşı tarafın bizde alacağı olduğu anlamına
gelir ve gerçektir — sıfıra çekmek kullanıcının parasını ekranda yok ederdi.
Kart tarafı Aşama 06 Grup 5'te aynı cevaba getirildi; ürün artık bu soruya tek
yerde tek cevap veriyor.

Pasifleştirme yeni iş yapmayı durdurur, geçmişi silmez: **pasif karşı tarafa
yeni borçlandırma yazılamaz, tahsilat yazılabilir.** Aksi hâlde artık iş
yapılmayan bir müşterinin kalan borcu kapatılamaz hâle gelirdi.

#### Cari bakiye okuma modeli

Bakiye üç tabloda duran hareketlerden okunur; `ICounterpartyRepository`
(Application portu) iki soru sorar ve ikisi de aynı projeksiyondan çıkar:
karşı taraf başına liste (`ListBalancesAsync`) ve tek karşı taraf
(`FindBalanceAsync`). Dönen `CounterpartyBalanceSummary` kimliği ve iki tarafı
birlikte taşır; `Net` ikisini tek cümleye, `IsSettled` "kapanmış cari mi"
sorusuna indirir.

Projection sorgulanan `asOfDate` değerini de alır ve her iki yönü
`Overdue*` / `NotOverdue*` olarak ayırır. Gecikme kalıcı kolon değildir:
yalnız iptal edilmemiş, vadesi sorgu tarihinden eski borçlandırmalar gecikmiş
sayılır. Tahsilat ve ödemeler tek bir borçlandırmaya bağlanmadığı için
deterministik mahsup kuralı **önce vadesi geçmiş hareketi kapatır**; gecikmiş
tutar `max(0, gecikmiş borçlandırmalar − o yöndeki bütün ödemeler)` olur.
Vadesi girilmemiş ve ileri vadeli kalan, `NotOverdue*` tarafındadır.

**Liste tek SQL ifadesidir.** Toplamlar karşı tarafın satırının içinde
ilişkili alt sorgular olarak durur; filtre (`All`/`Open`/`Settled`, aktiflik)
ve sıralama da veritabanında çalışır. Kişi başına ayrı bir toplam sorgusu,
elli kayıtlık bir listede yüzün üzerinde sorgu demek olurdu — ölçü
`CounterpartyBalances_ComeFromOneQueryAndStayInsideTheOwner` testiyle
sabitlendi (53 karşı taraf, tek okuma komutu).

**Sıfır bakiyeli karşı taraf listeden düşmez**, yalnız ayrı okunur: hesabın
kapanmış olması o kişiyle iş yapılmadığı anlamına gelmez. Eksi bakiye
(fazla tahsilat) **kapanmış sayılmaz** — hâlâ konuşulacak bir para vardır.

Kalıcılıkta sahiplik izolasyonu foreign key'in kendisindedir: hareketler
karşı tarafa `(UserId, CounterpartyId) → (UserId, Id)` ile bağlanır, yani
başka kullanıcının karşı tarafına yazılan bir hareket veritabanı seviyesinde
reddedilir. `(UserId, Name)` tekil indeksi aynı kişinin iki kez oluşmasını
engeller. Tahsilat tablosunda **kategori ve kapsam kolonu hiç yoktur**;
yokluğu migration testiyle korunuyor.

`AddObligationsAndCounterpartyDueDates` migration'ı mevcut cari geçmiş için
`DueDate` kolonunu **nullable ve varsayılansız** ekler; geçmiş satırların vadesi
bilinmediği için bir tarih uydurulmaz. Aynı adımda boş doğan `Obligations` ve
`ObligationSettlements` tabloları Grup 1 modelini kalıcılaştırır. Yükümlülük
kategoriye ve isteğe bağlı karşı tarafa, settlement hesaba ve yükümlülüğe
`(UserId, Id)` bileşik anahtarlarıyla bağlanır. `(UserId, ObligationId)` tekil
indeksi bir yükümlülüğün en fazla bir nakit kapanışı olmasını veritabanında da
korur. Bu checkpoint yalnız kalıcılığı kurar; yükümlülük endpoint'i henüz yoktur.

#### Taksitli sözleşme de aynı karşı tarafa bağlı

`DebtAgreement` karşı tarafın **adını taşımayı bıraktı**, kimliğine bağlandı
(`CounterpartyId`, aynı composite foreign key deseni). Ad tek yerde,
`Counterparty` kaydında yaşıyor; ikinci bir kopya, karşı taraf yeniden
adlandırıldığında sözleşmeyi eski adla bırakırdı. Okuma yolları (birleşik
feed, planlanan görünüm, fiş yinelenme araması, dışa aktarma) adı karşı
taraftan **join ile** alır; feed yine tek SQL sorgusudur.

Sözleşme açarken kullanıcı ad yazar: sunucu karşı tarafı **bulur ya da
kurar** ve iki adımlı bir akışa zorlamaz. Yeni karşı taraf, sözleşmeyle
**aynı kaydetme sınırında** yazılır; ayrı kaydetmek, sözleşme doğrulamada
düştüğünde ortada sahipsiz bir karşı taraf bırakırdı.

Aynı kişinin iki kaynağı birbirini toplamaz: net varlık taksitli sözleşmenin
**kalan anaparasını** sayar (ADR 0010), açık cari kendi projection'ında
durur. Cari borçlandırmanın gelir/gider raporuna ve feed'e katılması Grup
5'in işi; bugün oraya girmiyor.

Yedek dosyası **adı taşımaya devam ediyor**, kimliği değil: karşı taraf
tabloları şemaya kendi sürümüyle (v7) girecek. Geri yükleme addan karşı
tarafı yeniden kurar ve aynı ad tek kayıt olur.

#### Cari hareket nereye girer

| Kayıt | Gelir/gider | Kasa | Bütçe | Net varlık | Feed |
|---|---|---|---|---|---|
| `CounterpartyCharge` (alacak) | **+gelir** | — | — | **+alacak** | `counterparty-charge` |
| `CounterpartyCharge` (borç) | **+gider** | — | **+harcama** | **+borç** | `counterparty-charge` |
| `CounterpartyPayment` | — | **±tutar** | — | bakiyeyi kapatır | `counterparty-settlement` |

Aylık rapor, dönem karşılaştırması, nakit akışı eğilimi, kategori dağılımı ve
bütçe ilerlemesi borçlandırmayı sayar; tahsilat hiçbirine girmez. Kasa
bakiyesi bunun tersi: yalnız tahsilatı görür. **Aynı satış iki kez
sayılmasın diye bölünme bu.**

Net varlıkta açık cari, taksitli sözleşmeyle aynı iki kovaya girer
(`receivableDebt` / `payableDebt`): sordukları soru aynı — ne alacağım var,
ne borcum. İki kaynak birbirini toplamaz; sözleşme kendi kalan anaparasını
(ADR 0010), cari kendi hareketlerini sayar.

Feed sekiz yazma modelini tek `UNION ALL` sorgusunda birleştirmeye devam
ediyor. Cari hareketin iki türü de **iptal edilebilir**: her biri tek başına
duran bir kayıttır, geri dönüşü olmayan bir planın sonucu değil.

Cari borçlandırma artık isteğe bağlı vade taşır. Bu vade cari listedeki gecikme
ayrımını besler; ortak planlanan projection'a katılması Aşama 03 Grup 4'ün
işidir ve bu checkpoint'te erkenden eklenmemiştir.

#### İstemci: `features/counterparties/`

Liste (bakiyeye göre sıralı, `Tümü / Açık hesap / Kapanmış` filtresi) ve
ayrıntı ekranı **tek controller** paylaşır: ayrıntıdan alınan bir tahsilat
listedeki bakiyeyi de değiştirir ve iki ayrı controller aynı yazımdan sonra
birbirini tazelemek zorunda kalırdı.

Liste sorgu tarihini API'ye gönderir; gecikmiş alacak/borç saat ikonlu ve açık
metinli rozetle görünür, renk tek başına anlam taşımaz. Ayrıntı kartı toplamın
altında vadesi geçmiş ve vadesi geçmemiş/vadesiz tutarı ayrı satırlarda gösterir.
Borçlandırma formundaki vade isteğe bağlıdır ve işlem tarihinden önce seçilemez.

Ayrıntı üç bloğu birlikte gösterir — açık cari bakiyesi, hareket geçmişi ve
varsa taksitli sözleşmeler — ama **hiçbiri diğerinin toplamına karışmaz**.
Geçmiş, ikinci bir okuma modelinden değil **birleşik feed'den** gelir
(`GET /api/v1/financial-activities?counterpartyId=…`): aynı hareketi iki
yerden okumak iki farklı sıralama ve iki farklı iptal kuralı demek olurdu.
Filtre, o kişiyle yapılmış **taksitli sözleşmenin** hareketlerini de
getirir; kullanıcı tek soru sorar, cevabı iki listeye bölünmez.

Formlar kayıt türünün kuralını yüzeyde tekrar eder: borçlandırma formu
**hesap sormaz** (para el değiştirmiyor), tahsilat formu **kategori ve
kapsam sormaz** (gelir/gider üretmiyor). Tahsilat alanı açık bakiyeyle dolu
açılır ama kilitli değildir — kısmi tahsilat kuraldır, istisna değil.
`FinancialDataChanges` yeni bir hedef aldı (`counterparties`): kişi
listesini değiştiren yazım yalnız onu, para yazan hareket ise gider/kasa
hedeflerini de yükseltir.

## Tek seferlik yükümlülük

> Aşama 03, Grup 1–7 — Domain modeli, kalıcılık, kanonik planlanan projection,
> ödenmemiş fatura, settlement akışları ve yedek şeması v8 uygulanmıştır.

Ödenmemiş tek seferlik fatura artık tekrarlayan plan kavramına sokulmadan
temsil edilebilir. Model ADR 0014'ün ayrımını iki ayrı kayıtla korur:

| Kayıt | Tanır | Taşır | Kategori | Kapsam | Hesap |
|---|---|---|---|---|---|
| `Obligation` | Yöne göre gelir/gider | Hayır | **Zorunlu** | **Zorunlu** | Yok |
| `ObligationSettlement` | Hayır | **Hesap bakiyesi** | Yok | Yok | **Zorunlu** |

`DebtDirection.Payable` gider, `Receivable` gelir tanır. Yönle çelişen kategori
reddedilir. Karşı taraf isteğe bağlıdır; elektrik faturası kaydetmek için cari
hesap açma zorunluluğu yoktur. Karşı taraf verilirse aynı kullanıcıya ait ve
aktif olmalıdır.

Durum ikinci bir değişebilir gerçek olarak tutulmaz: `Open`, `Settled` ve
`Cancelled`, settlement ve iptal alanlarından hesaplanır. **Gecikme durum
değeri değildir**; `IsOverdueOn(asOfDate)` açık kayıt için vade ile sorgu
tarihini karşılaştırır. Vade bugün geçmişte veya gelecekte olabilir, fakat
düzenleme tarihinden önce olamaz. Düzenleme tarihi de sistemin UTC kayıt
zamanından ileri olamaz.

Kapanış aggregate üzerinde idempotenttir. İlk `Settle` çağrısı tek
`ObligationSettlement` üretir; sonraki çağrılar aynı hareketi döndürür ve ikinci
nakit etkisi oluşturmaz. Settlement yükümlülüğün tutarını ve yönünü aynen alır:
tahsilat hesabı artırır, ödeme azaltır. İptal, yükümlülükle varsa settlement'ı
birlikte UTC zaman damgasıyla iptal eder; finansal geçmiş fiziksel silinmez.

`Obligations` ve `ObligationSettlements` owner-scoped bileşik ilişkilerle
kalıcıdır; `(UserId, ObligationId)` tekilliği iki stale yazarın ikinci settlement
oluşturmasını engeller. Açık yükümlülükler planlanan görünümde kategori ve
isteğe bağlı karşı tarafla tek SQL projection'ında okunur. `Open`, gecikme,
`readiness` ve `attentionCode` saklanmaz: iptal/settlement varlığı ile sorgu
tarihinden türetilir. Settlement oluştuğu anda satır hem kanonik görünümden hem
onun daraltılmış yaklaşan ödemeler görünümünden düşer.

`CreateObligationUseCase` current user'ı oturumdan alır; kategori, isteğe bağlı
aktif karşı taraf ve kapsam zincirini doğruladıktan sonra aggregate'i yazar.
`POST /api/v1/obligations` açılan kaydı gerçekleşen hareket feed'ine
`obligation` türüyle, raporlara ve net varlığa düzenleme tarihinde katar. Hesap
tablosuna yazmadığı için kasa bakiyesi değişmez.

`GET /api/v1/obligations?asOfDate=...` durum ve gecikmeyi sorgu tarihinde
türetir. `POST /api/v1/obligations/{id}/settlement` current user'ın aktif ve
aynı para birimindeki hesabını çözer; tek settlement'ı yazar. Nakit etkisi hesap
bakiyesi, rapor bakiyeleri ve `obligation-settlement` feed satırında görünür;
kategori/kapsam taşımadığı için aylık gelir-gider ikinci kez değişmez. Karşı
taraflı açık yükümlülük cari bakiyeye katılır, settlement sonrasında düşer.

## Gün sonu kasa sayımı

> Aşama 04, Grup 2 — yalnız **Domain** katmanı uygulanmıştır. EF modeli,
> migration ve yazma yolu Grup 4'te, kasa sayımını ve POS tahsilatını SQL'den
> okuması gereken ilk grupla birlikte eklenecektir.

`CashCount` bir para hareketi **değil, bir gözlemdir**: hesap bakiyesine
dokunmaz, gelir/gider yazmaz ve tek başına hiçbir rapora girmez. Kasadaki para
zaten oradaydı; sayması onu değiştirmez.

**Beklenen tutar kaynak gerçek olarak saklanmaz.** Günün açık sayımının
farkı `DifferenceFrom(expectedBalance)` ile, sayım okunduğu anda hesap bakiyesi
projection'ından türetilir. Bu, bakiyenin kalıcı kolon olmama gerekçesinin
aynısıdır: fark kaydın yanına yazılsaydı, sonradan iptal edilen bir hareket
bakiyeyi değiştirdiği anda o sayı sessizce yanlışa dönerdi. Aynı sayım, bir
hareket iptal edildikten sonra **farklı bir fark verir** — doğru davranış budur.

**Sayım anının gözlemi ayrıca tutulur** (`ExpectedAtCount`, 27 Eylül 2026,
`AddCashCountExpectedSnapshot`). Kasa ekranının `Son sayımlar` listesi her
sayımın yanında "o gün uygulama ne diyordu" sorusunu cevaplar; bu bir türetme
değil, sayılan tutar gibi o anda yapılmış bir **gözlemdir** ve sonradan
değişmemesi gerekir. Kaynak gerçek olmadığı için hiçbir hesap onu okumaz:
bakiye, rapor ve düzeltme tutarı yine canlı projection'dan gelir. Kolon
nullable'dır; bu migration'dan önceki sayımlarda boştur ve bugünkü bakiyeden
doldurulmaz (olmamış bir geçmiş uydurmak olurdu). Farkı kaydedilmiş günün
sayımı da bu gözlemi okur: düzeltme bakiyeyi sayılana oturttuğu için canlı
fark sıfırdır, kullanıcının kaydettiği fark ise sayım anındaki tutardır.

`CashCountDifference` yönü anlamıyla taşır: fazla çıkan nakit gelir, eksik
çıkan nakit gider tarafındadır. Tutarı `Money`'ye çevirirken işaret düşer,
çünkü yönü `TransactionType` taşır. Dengede fark yoktur: `RecognizedType`
sorulursa hata verir ve sıfır tutarlı bir düzeltme kaydı yazılamaz.

Sayılan tutar `Money` **değildir**: sıfır meşrudur, kasası boşalan esnaf da
sayım yapar. Negatif sayım reddedilir — kasada eksi nakit bulunmaz.

**Fark otomatik düzeltme hareketi üretmez.** Sayım hatasını gerçek bir para
hareketi gibi yazmak, olmamış bir gideri kayda geçirmek olurdu. Düzeltme
ikinci ve ayrı bir eylemdir: kullanıcı açıkça onaylarsa `RecordAdjustment`
tek bir `BudgetTransaction` kimliğini bağlar. Çağrı idempotenttir (aynı
kimlikle tekrar ikinci kayıt üretmez) ve farklı bir kimlikle ikinci düzeltme
reddedilir — bir sayımın iki düzeltmesi olamaz.

**Aynı gün ve aynı hesap için ikinci sayım öncekini iptal eder, üzerine
yazmaz** (`SupersedeWith`). Eski gözlem gerçekten yapılmıştı ve iptal edilmiş
hâliyle kalır. Başka bir günün ya da başka bir hesabın sayımı önceki sayımı
kapatamaz.

Yalnız **kullanıcının kendi, aktif ve nakit** hesabı sayılır: banka bakiyesi
elle sayılmaz, sayım fiziksel bir gözlemdir. Kapsam sayım anında sabitlenir ve
düzeltme anında yeniden türetilmez; aksi hâlde aynı sayım iki farklı günde iki
farklı kapsam üretebilirdi (tekrarlayan planın kapsamında verilen kararın
aynısı).

## POS tahsilatı ve yoldaki para

> Aşama 04, Grup 3–4 — Domain, kalıcılık, hesap bakiyesi, net varlık ve
> gelir/gider raporunda tanıma; Grup 7 — yazma uçları ve ekranlar; Grup 8 —
> birleşik feed ve yedek v9. **Tamamı uygulanmıştır.**

`PosSettlement` ADR 0015'in kaydıdır ve **kredi kartı değildir**: `CreditCard`
borçlandığın karttır, bu ise tahsilat aracıdır. İkisi aynı kelimeyle anıldığı
sürece "kartla 800 lira aldım" cümlesi borç olarak kaydediliyordu.

ADR 0014'ün ayrımı burada tek kaydın **iki anına** düşer:

| An | Tanır | Taşır |
|---|---|---|
| Tahsilat günü | Gelir **brüt** tutar kadar, komisyon ayrı gider | Hesap bakiyesi kıpırdamaz |
| Geçiş günü | Hiçbir gelir/gider yazılmaz | Hesap **net** tutar kadar artar |

`SignedAccountEffect` geçiş gerçekleşene kadar sıfırdır; para henüz bankada
değildir. `MarkTransferred` idempotenttir ve ikinci bir günle işaretlemeyi
reddeder — para bir kez geçer.

**Komisyon brüt tutardan ayrı okunur ve ona eklenmez.** Net tutarı gelir olarak
yazmak, kullanıcının gerçekten kestiği faturayı küçültür ve komisyonu görünmez
bir gidere çevirirdi; dekonttaki işlem ücretinde öğrenilen dersin aynısı.
Komisyon `Money` değildir çünkü **sıfır meşrudur** — her kartta komisyon
kesilmez. Komisyon ile gider kategorisi birlikte bulunur ya da birlikte
bulunmaz; komisyonun tamamı brütü yiyemez, yoksa hesaba hiçbir şey geçmezdi.

**Komisyon oranı saklanmaz, paradan çözülür** (`CommissionRate`). Bu ADR
0009'un aynı kararıdır: oran ve tutar ayrı ayrı saklansaydı ikisi ayrı ayrı
düzenlenebilir ve sessizce çelişirdi. Kullanıcı oran girdiğinde
`CommissionFromRate` onu tutara çevirir; yuvarlama para tarafında yapılır,
çünkü hesaba geçecek olan paradır.

Para **banka hesabına** geçer: kasa bir kart ödemesi alamaz. Satış bir gelir
kategorisi ister; POS tahsilatı bir satışı tanır.

### POS tanımı (Aşama 06.3 Grup 4, ADR 0019 T4)

`PosDefinition` kullanıcının bir kez girdiği ayardır: ad, paranın geçeceği
banka hesabı, satış kategorisi, varsayılan komisyon oranı, komisyon kategorisi
(oran sıfırdan büyükse zorunlu), geçiş günü ve iş günü seçeneği. **Para
hareketi üretmez**; yalnız tahsilatı doldurur. Yemek kartı ayrı bir özellik
değil, kendi oranı ve süresi olan bir tanımdır.

- Tahsilat isteği `posDefinitionId` taşıyabilir; boş bırakılan alanlar (hesap,
  satış kategorisi, komisyon, komisyon kategorisi, beklenen gün) tanımdan dolar,
  açıkça gönderilen alan tanımı ezer. Tanımsız istekte hesap, kategori ve
  beklenen gün zorunludur (`pos_settlements.details_required`); sunucu
  uydurmaz.
- Oran **tanımda** saklanır, tahsilatta saklanmaz: tahsilat tutarı taşır ve
  oran ondan çözülür (ADR 0009). Tanım sonradan değişse de yazılmış tahsilat
  yazıldığı tutarla kalır.
- Beklenen gün `PosDefinition.ExpectedTransferDate` ile hesaplanır. İş günü
  seçeneği açıksa süre hafta içi günlerle sayılır ve sonuç hafta sonuna düşmez
  (cuma satışının ertesi günü pazartesidir); tatil takvimi yoktur.
- Önizleme (`GET /api/v1/pos-definitions/{id}/preview`) komisyonu, neti ve
  beklenen günü tahsilatı yazan kuralla aynı yerden döner; istemci parayı
  kendisi hesaplamaz.
- **Ana POS** (`IsDefault`): tahsilat formunda seçili gelen POS. Kullanıcı
  başına en çok bir tanedir ve bu, işareti koyan use case'te korunur (öncekinin
  işareti aynı `SaveChanges`'te kalkar). İlk eklenen POS kendiliğinden ana POS
  olur; pasife alınan POS işaretini kaybeder ve pasif POS seçilemez.
- `PosSettlement.PosDefinitionId` nullable'dır: tanımlardan önce yazılmış ve
  tanımsız girilen tahsilatın hangi POS'tan geldiği bilinmiyor ve uydurulmaz.
  Tahsilatı olan tanım silinemez (`409 pos_definitions.has_settlements`),
  pasife alınır; pasif tanımla yeni tahsilat yazılamaz.

`PosTransitBalance` yoldaki parayı verir: geçişi gerçekleşmemiş tahsilatların
**net** toplamı. Kalıcı kolon değildir ve bir hesap türü de değildir (ADR
0015). İptal edilmiş ve geçmiş tahsilatlar sayılmaz — ilki hiç olmadı, ikincisi
artık hesabın kendi bakiyesinde duruyor; ikisini de saymak aynı parayı iki
yerde göstermek olurdu. Brüt toplamak da bankanın kestiği komisyonu
kullanıcının cebinde sayardı.

### POS tahsilatı birleşik feed'de üç satırdır

Tek yazma modeli, feed'de üç okuma satırı üretir (Grup 8): `pos-sale` tahsilat
günü brüt geliri, `pos-commission` aynı gün komisyon giderini, `pos-transfer`
ise geçiş günü hesaba giren net tutarı gösterir. Üçü de aynı kaydın kimliğini
taşır; istemci satırı `tür + kimlik` ikilisiyle anahtarlar.

Tek satıra indirilseydi ya komisyon görünmez olurdu ya da hesabın bakiyesindeki
artışın günü yanlış yazılırdı — gelir tahsilat günü, para ise geçiş günü
gerçektir. Geçiş satırı **kapsam taşımaz** (parayı taşır, gelir/gider üretmez)
ve kapsam filtreli okumada düşer; diğer ikisi kapsam taşır ve kalır.

Üç satırın hiçbiri feed üzerinden iptal edilemez: birini iptal etmek diğer
ikisini sahipsiz bırakırdı. İptal, kaydın kendi ekranından tek eylemle yapılır.

**Gün sonu kasa sayımı feed'de yoktur** ve bu bir eksiklik değildir: sayım
hiçbir para hareketi üretmeyen bir gözlemdir. Farkı onaylandığında üretilen
düzeltme kaydı normal bir `account-transaction` olarak zaten görünür.

### Kalıcılık: iki yeni tablo, türetilen hiçbir şey kolon değil

`AddCashCountsAndPosSettlements` `CashCounts` ve `PosSettlements` tablolarını
kurar. İkisi de bu adımda **boş doğar**: yorumlanacak bir geçmiş yoktur, bu
yüzden zorunlu kolonlar backfill istemez ve kalıcı bir DEFAULT bırakılmaz.
Dolu bir tabloda bu yol kullanılmaz (migration kuralı, `AGENTS.md`).

Türetilen hiçbir şey kolon değildir: `PosSettlements` brüt tutarı, komisyonu ve
geçiş gününü saklar; net tutar, komisyon oranı ve "yolda mı" bundan çıkar.
`CashCounts` yalnız sayılan tutarı saklar; beklenen bakiye ve fark kolon
değildir. Migration testi bu yokluğu açıkça sınar — saklansalardı sessizce
eskiyen ikinci bir gerçek olurlardı.

Sahiplik kapısı SQL seviyesinde de duruyor: her iki tablo da hesabına ve
kategorilerine `(UserId, Id)` bileşik anahtarıyla bağlanır. `CashCounts`
üzerinde **filtreli tekil indeks** (`IsCancelled = 0`) bir gün ve bir kasa için
tek açık sayım bırakır; iki eşzamanlı yazar aynı gün için iki açık sayım
bıraksaydı hangisinin geçerli olduğu belirsizleşirdi.

### Kullanılabilir bakiye ile net varlık ayrı sorulardır

Geçmiş POS tahsilatı hesap bakiyesine **net** tutarıyla girer; geçmemiş olan
hiç girmez. Bakiye iki yerde hesaplanıyor (`EfAccountRepository` tek hesap
için, `EfFinancialReportRepository` rapor dağılımları için) ve ikisi de aynı
kuralı uygular.

Net varlık ayrıca **yoldaki parayı** taşır (`NetWorthDto.MoneyInTransit`):

```text
kullanılabilir bakiye = hesapların bakiyesi (yoldaki para hariç)
net varlık            = kullanılabilir + yolda − kart borcu + alacak − borç
```

İki sayının farkı **tam olarak** yoldaki tutardır ve bu bir test kapısıdır,
yorum değil. Yoldaki toplam tek bir gruplanmış sorguyla okunur; tahsilat başına
sorgu, gelişmiş raporun sabit komut kapısının tam da reddettiği şekildir
(61 → 63).

### POS satışı raporda nereye girer

Tanıma **tahsilat gününde**dir; geçiş günü hiçbir gelir/gider yazmaz.

| Rapor | Brüt tutar | Komisyon | Geçiş günü |
|---|---|---|---|
| Aylık gelir/gider | **+gelir** | **+gider** | — |
| Kategori dağılımı | — (gelir tarafı) | komisyon kategorisinde | — |
| Nakit akışı eğilimi | **+gelir** | **+gider** | — |
| Bütçe sapması | — | **+harcama** | — |
| Hesap bakiyesi | — | — | **+net** |
| Net varlık | yolda iken `moneyInTransit`, geçince likit | — | kova değişir |

Komisyon bütçeyi tüketir çünkü kendi kategorisi olan, o gün tanınmış gerçek bir
giderdir. Brüt tutar bütçeyi tüketmez: o bir gelirdir ve bütçe gider bütçesidir.

Kapsam filtresi gelir/gider tarafını daraltır; net varlık ve yoldaki para
kapsamdan **etkilenmez** (ADR 0013). Tanıma altı sabit gruplanmış sorgu ekledi
(63 → 69) ve hiçbiri tahsilat adediyle büyümüyor.

Net varlık cevabı iki tarafın toplamını da taşır (`totalAssets`: likit + yolda
+ alacak + varsa kart alacağı; `totalLiabilities`: kart borcu + borç) ve
yoldaki paranın en yakın geçiş gününü (`nextTransitDate`). Üçü de sunucuda
hesaplanır; Özet ekranının varlık kartı kalemleri kendisi toplamaz. Geçiş günü
yoldaki tutarı okuyan filtrenin aynısıyla tek bir `MIN` sorgusudur.

### Kasa ve POS yazma uçları

```text
GET  /api/v1/cash-counts/today?accountId=   beklenen bakiye + o günün sayımı
                                            + son sayım + bugünkü nakit giriş/çıkış
GET  /api/v1/cash-counts                    geçmiş sayımlar
POST /api/v1/cash-counts                    gün sonu sayımı (gözlem)
POST /api/v1/cash-counts/{id}/adjustment    farkı tek kayda çevirir

GET  /api/v1/pos-settlements                tahsilatlar + yoldaki toplam
POST /api/v1/pos-settlements                tahsilat (tanır, taşımaz)
POST /api/v1/pos-settlements/{id}/transfer  geçiş (taşır, tanımaz)
DELETE /api/v1/pos-settlements/{id}/transfer geçişin geri alınması (net tutar
                                            hesaptan geri çekilir; satış kalır)
DELETE /api/v1/pos-settlements/{id}         iptal (satış, komisyon ve varsa
                                            geçiş birlikte düşer)

GET  /api/v1/pos-definitions                kullanıcının POS'ları (ana POS önce)
POST /api/v1/pos-definitions                POS ekler (para hareketi yazmaz)
PUT  /api/v1/pos-definitions/{id}           düzenler; yazılmış tahsilat değişmez
PATCH /api/v1/pos-definitions/{id}/active   pasife alır / etkinleştirir
PUT  /api/v1/pos-definitions/{id}/default   ana POS'u seçer (kullanıcı başına bir)
DELETE /api/v1/pos-definitions/{id}         siler; tahsilatı varsa 409
GET  /api/v1/pos-definitions/{id}/preview   komisyon, net ve beklenen gün
```

`today` cevabı ayrıca `changeSinceCount` taşır: günün sayımından bu yana kasa
bakiyesindeki değişim (farkı kaydedilmiş sayımda güncel bakiye − sayılan,
kaydedilmemişte güncel bakiye − sayım anındaki beklenen; gözlem yoksa boş).
Farkın anlamını değiştirmez; ekranın "oturdu" diyememesi için vardır.

Canlı beklenen bakiye ve fark **yalnız günün açık sayımında** hesaplanır.
Geçmiş bir günün farkını bugünkü bakiyeye karşı yeniden hesaplamak, aradaki
bütün hareketleri o günün farkına yazmak olurdu; sayı doğru görünür, anlamı
yanlış olurdu. Listede geçmiş sayımın beklenen tutarı ve farkı, sayım anında
saklanan gözlemden (`ExpectedAtCount`) gelir; gözlem yoksa (eski sayım, geri
yüklenmiş yedek) ikisi boş döner.

`today` cevabı beklenen tutarın nereden geldiğini de taşır: bugünden önceki son
geçerli sayım (`previousCount`) ve bugünkü nakit giriş/çıkış (`todayInflow`,
`todayOutflow`). Giriş ve çıkış hesap bakiyesiyle aynı kaynaklardan, yalnız o
günün tarihine süzülerek sunucuda toplanır (`IAccountDayFlowReader`); istemci
toplama yapmaz.

Aynı gün ve aynı kasa için ikinci sayım öncekini **iptal eder** ve ikisi tek
`SaveChanges` sınırında yazılır; ayrı yazılsaydı SQL'deki filtreli tekil indeks
araya düşen ikinci açık sayımı zaten reddederdi. Düzeltme idempotenttir: bir
sayımın iki düzeltmesi olamaz, tekrarlanan onay aynı kaydı döndürür.

Komisyon **ya tutar ya oran** olarak gelir, ikisi birden değil: ikisi de
gönderilseydi hangisinin doğru olduğu sorusu doğardı. Oran gelirse tutara
çevrilir ve saklanan tek şey tutardır (ADR 0009'un aynı kararı).
`moneyInTransit` liste penceresinden bağımsız okunur — yolda olan paranın
toplamı, kullanıcının hangi aya baktığından etkilenmemelidir.

Flutter tarafında iki özellik aynı bağımlılık yönünü korur:

```text
CashPage
  ├─ CashCountView → CashCountController → CashRepository → ApiClient
  └─ PosSettlementsView → PosController → PosRepository → ApiClient
```

DTO'lar açık Dart modelleridir; para alanları JSON number veya `double`'a
çevrilmez. Yön kontrolü bile kanonik para metninin işaretinden okunur, tutar
hesabı istemcide yapılmaz. `_CashPageHost` controller'ları shell'in profil ve
kapsam kaynaklı yeniden çizimlerinden daha uzun yaşatır; sayfa her çizildiğinde
yeni istek/listener üretmez ve ikisini tek sahip olarak dispose eder.

Üçüncü `StatefulShellBranch` sabittir; `ScopeController.isVisible` yalnız dalın
`Kasa` veya `Bütçeler` içeriğini ve gezinme etiketini seçer. Yerinden inen ekran
`/more/cash` ya da `/more/budgets` rotasında kalır. Böylece profil ön ayarı
Navigator yığınını yeniden kurmaz ve özellik yetkisine dönüşmez.

### Taşınabilirlik: dışa aktarma ile yedek ayrı şeylerdir

İşlem CSV'si kaydın kapsamını `type`'ın yanında bir kolonda taşır. Kapsamsız
bir dosya, kullanıcının kendi arşivinde işletme ile cebini bir daha ayıramaz
hâle getirirdi: aynı hesaptan, aynı kategoriye yazılmış iki kayıt arasındaki
tek fark kapsamdır.

CSV yine de **geri yüklenemez**; okumak ve arşivlemek içindir. İçe aktarma
banka ekstresi ayrıştırıcısıdır ve kapsam kolonunu okumaz — zincir hesabın ya
da kategorinin varsayılanından çözülür. İstemci kendi dışa aktarımını içe
aktarma ekranında tanıyıp reddeder; tanıma **tam başlık dizesine değil, yalnız
bu dosyada bulunan kolonlara** bakar, çünkü tam eşleşme bir kolon eklendiğinde
sessizce yanlışa döner ve koruma kalkardı (kapsam kolonu eklenirken tam olarak
bu oldu). Veriyi bir hesaptan diğerine taşımanın tek yolu yedek/geri yükleme
akışıdır (`documentation/restore-runbook.md`).

Yükümlülük yedeğe kendi koleksiyonuyla girer ve onu kapatan nakit hareketi
**ayrı bir koleksiyon değildir**: `settlement` yükümlülüğün içinde durur, çünkü
bağ bire birdir ve ayrı yazılsaydı dosya sahipsiz bir ödeme taşıyabilirdi.
Gecikme dosyaya yazılmaz — vade ile okuma gününden türeyen bir sonuçtur ve
saklanan hâli dosyanın açıldığı gün yanlış olurdu. Aynı gerekçeyle tekrarlayan
planın occurrence sayacı geri yüklerken dosyadan kopyalanmaz, occurrence
geçmişi yeniden oynanarak türetilir; dosyadaki değer yalnız doğrulama içindir.
Yedek şemasının **güncel sürümü ve hangi sürümleri okuduğu** tek yerde,
`documentation/restore-runbook.md` içinde tutulur.

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

## Vergi: kayıtta vergi alanı yok

ADR 0016 vergiye dair bilgiyi kayıtların üstüne taşınan alanlar olarak kurmuştu:
gelir veya gider tanıyan beş kayıtta KDV (`VatDetails`), gider tarafında
indirilebilirlik, kategoride indirilebilirlik varsayılanı ve bunları okuyan ay
sonu muhasebeci paketi. **ADR 0018 bunların hepsini kaldırdı** (Aşama 06.3
Grup 2): ürün bir bütçe uygulamasıdır, ön muhasebe değildir; KDV'yi muhasebeci
GİB'e zaten elektronik giden belgelerden kurar.

- **Şema:** `RemoveVatAndTaxDeductibility` migration'ı `BudgetTransactions`,
  `CreditCardCharges`, `CounterpartyCharges`, `Obligations` ve `PosSettlements`
  tablolarındaki `VatRate`/`VatAmount` kolonlarını, dört tablodaki
  `IsTaxDeductible` kolonunu, `Categories.DefaultIsTaxDeductible` kolonunu ve
  bunları koruyan `CK_*_VatRate`, `CK_*_VatAmount`, `CK_*_IsTaxDeductible`
  kısıtlarını düşürür. Kısıtlar kolonlardan önce düşer; geri dönüşte kolonlar
  kısıtlardan önce, nullable ve varsayılansız gelir. **Veri kaybettiren bir
  adımdır** ve kullanıcı kararıdır (`docs/project-status.md`).
- **Sözleşme:** istekte `vatRate`, `vatAmount`, `isTaxDeductible`,
  `defaultIsTaxDeductible` alanları yoktur; cevapta `vat` nesnesi dönmez.
  `/api/v1/accountant-package` ve `/api/v1/exports/accountant-package.zip`
  uçları kalktı.
- **Fiş okuma:** model belgedeki KDV tutarını okur, ama yalnız "ara toplam +
  KDV = genel toplam" denetimi için; taslakta KDV alanı yoktur ve KDV forma
  hiç ulaşmaz.
- **Yedek:** şema v11; KDV ve indirilebilirlik taşınmaz
  (`documentation/restore-runbook.md`).

Yürürlükte kalan ilke: uygulama **hiçbir vergi tutarını türetmez** (ADR 0016
§1, ADR 0018 İ1). Vergi bir nakit çıkışıdır — tanımlıysa bir tekrarlayan plan,
ödendiyse o gün yazılan bir giderdir.

### Vergi bir nakit planıdır (Aşama 06.3 Grup 3, ADR 0018)

Vergi **ayrı bir kayıt türü değildir** (İ6). Yeni aggregate, yeni tablo ve
ikinci bir "yaklaşanlar" kaynağı yoktur; mevcut modeller genişledi:

- **Tanımlı vergi** `RecurringTransaction.TaxKind` dolu bir plandır. Vergi planında
  beklenen tutar (`AmountValue`) ve kaynak (`SourceType`) boş olabilir; sıradan
  planda ikisi de zorunlu kalır (ADR 0005). Tutar ile para birimi artık ayrı
  alanlardır (`Money?` hesaplanır), çünkü "bilinmiyor" sıfır değildir (İ5).
- **Ritim** iki alanla genişledi: `DayOfMonth` (boşsa başlangıç günü; `31` ay
  sonu) ve `SelectedMonths` (ay kümesi, `RecurrenceFrequency.SelectedMonths`).
  Mevcut planlarda ikisi de boştur ve davranış değişmez.
- **Kalem** (`RecurringTransactionOccurrence`) tutarı boş olabilir ("tutar belli
  oldu" onu yazar); kaynağı ödeme anında değişebilir; yeni `Closed` durumu toplu
  ödemeyle kapatılmış kalemdir ve kendi sonucu yerine kapatan ödemenin
  kimliğini (`ClosedByTransactionId` ya da `ClosedByChargeId`) taşır. Bir kalem
  tam olarak tek sonuç taşır (İ7); SQL'deki `CK_RecurringOccurrences_Realization`
  üç durumu da ayrı ayrı denetler.
- **Ödenen vergi** vergi işaretli (`Category.IsTax`) bir kategorideki gider ya da
  kart harcamasıdır. "Ödenenler" bu iki tablonun `UNION ALL` okumasıdır
  (`EfTaxPaymentRepository`); gider formundan girilen vergi de oradadır (T6).
- **"Ödedim"** kalemi ödeme gününe ve seçilen kaynaktan gerçekleştirir; kaynağın
  tek dallanması (hesap → gider, kart → kart harcaması) değişmedi.
  **Geri alma** sonucu iptal eder ve kalemi bekleyene döndürür, tek
  `SaveChanges` sınırında. Birleşik akıştaki köken kilidi yerinde kalır.
- **Toplu vergi ödemesi** tek gider (ya da kart harcaması) yazar ve seçilen
  kalemleri kapatır. Ödeme kimliği istek kimliği ile kullanıcı kimliğinden
  türetilir (`RequestScopedId`): tekrar gönderilen istek ikinci gider yazmaz ve
  gider tablosuna bir istek kolonu eklenmez. Ödemenin iptali — vergi ekranından
  da İşlemler'den de — kapattığı kalemleri aynı sınırda açar.
- **Kalemi yerinde üretmek** (`RecurringOccurrenceMaterializer`): "Ödedim",
  "tutar belli oldu" ve kapatma henüz üretilmemiş bir güne yazabilir; yalnız o
  plan o güne kadar ilerletilir, gün ritme düşmüyorsa plana dokunulmaz.
- **Düzenleme** ritmi de değiştirebilir (kullanıcı kararı, 30 Eylül 2026):
  bekleyen kalemler yeniden kurulur, ödenmiş ve kapatılmış kalemler kalır, yeni
  başlangıç son ödenen kalemden sonradır. Ritim değişmezse bekleyenler planı
  izler; kullanıcının o dönem için yazdığı tutar korunur.
- **Kapsam** açık seçimden, yoksa profilin tarafından gelir
  (`TransactionScopeResolution.ResolveTax`); ödeme kaynağının etiketine bakılmaz
  (ADR 0018 İ9, 30 Eylül 2026). Vergide ADR 0013 zincirinin hesap/kart basamağı
  yoktur.
- **Toplu tanımlama** (`CreateTaxPlansUseCase`) her planı tek plan oluşturmanın
  kuralıyla kurar (`CreateRecurringTransactionUseCase.BuildAsync`: doğrular,
  yazmaz) ve hepsini tek `SaveChanges` ile yazar.
- **Planlanan projection** tutarsız kalemi `amount = null` ile taşır; "7 günde
  çıkacak" toplamı onu saymaz, `UnknownAmountCount` ile ayrıca söyler. Gecikmiş
  yükümlülüklerin toplamı ayrı bir alandadır (`OverdueOutgoingTotal`). Satır
  planın kimliğini ve vergi türünü taşır; vergi türü kalemin sorgusuna join ile
  gelir, projection'ın sorgu sayısı değişmedi. Vergi ekranının toplamı
  (`PendingTotal`) aynı satırlardan, gecikenler dahil kurulur.
- **Tanım ayrıntısı** projection'ı yalnız o plan için izdüşürür
  (`IPlannedActivityRepository.ListForRecurringPlanAsync`, üç yıllık ufuk) ve
  gecikmişleri + vadesi gelmemiş ilk üç kalemi gösterir; seyrek ritimli vergide
  sabit bir pencere listeyi boş bırakırdı.
- **Geri yükleme** tekrarlayan planı artık geçmişi yeniden oynayarak değil anlık
  görüntüden kurar (`RecurringTransaction.Restore`,
  `RecurringTransactionOccurrence.Restore`): ritmi değişmiş bir planın eski
  kalemleri yeni ritme uymaz. Sayacın kalem sayısına eşit olduğu ve hiçbir
  kalemin sıradaki günden sonra olmadığı doğrulanır.

### Vergi ve SGK takvimi: yeni bir zamanlayıcı yok

Takvim kalemi **tekrarlayan bir plandır**. Yeni tablo, yeni zamanlayıcı ve
ikinci bir "yaklaşanlar" kaynağı yoktur; kalem planlanan projection'a diğer
planlarla aynı yoldan düşer, aynı yoldan duraklatılır ve silinir.

`GET /api/v1/tax-calendar/suggestions` hazır vergi türlerini döner (Aşama 06.3
Grup 3'ten beri sekiz tür: Bağkur, KDV, muhtasar ve prim hizmet, geçici vergi,
yıllık gelir vergisi, emlak, motorlu taşıtlar, ilan-reklam). Bu uç **hiçbir şey
yazmaz** — tür, önerinin doldurduğu formla mevcut
`POST /api/v1/recurring-transactions` ucundan `taxKind` ile kurulur. İkinci bir
yazma yolu, aynı planın iki ayrı biçimde oluşabilmesi olurdu.

Öneri **tutar taşımaz**: bir sayı önermek, hesaplanmış bir vergi tutarı iddia
etmek olurdu (ADR 0016). Önerilen gün bir başlangıç noktasıdır; kurulduğu andan
sonra tarih de tutar da kullanıcınındır ve uygulama onları kendiliğinden
güncellemez. Mevzuat takibi yapılmadığı arayüzde yazılıdır.

**Çeyreklik dönem** (`RecurrenceFrequency.Quarterly`) eklendi: geçici verginin
ritmi budur. Ayrı bir hesap yolu değil, aylık adımın üç aylık hâlidir — ay sonu
davranışı (kısa ayda son güne çekme) aynı kuralı izler.

**Plandaki tutar bir beklentidir.** Gerçekleştirme isteği isteğe bağlı bir tutar
taşır ve kayda o yazılır; boş bırakmak "plandaki tutar doğru" demektir. Yalnız
**bekleyen** occurrence düzeltilebilir — gerçekleşmiş olan geçmiştir ve
düzeltmesi iptal + yeni kayıttır. **Planın kendi tutarı değişmez**: düzeltilen
bu dönemdir, plan değil. Bu yalnız verginin sorunu değildi; elektrik faturası
gibi her dönem değişen her kalem beklentiyi gerçekleşmiş hareket olarak
yazıyordu.

### Karşılık olarak hedefler

`SavingsGoal` **nullable** bir kapsam taşır. İşletme kapsamlı hedef bir
*karşılıktır* — vergi için kenara konan para — ve şahsi hedeflerden ayrı
raporlanır. Boş olması meşrudur: kapsamı arayüzünde hiç görmeyen kişisel
kullanıcının hedefi etiket taşımaz.

Hedef gelir/gider üretmez; kapsam burada da **parayı bölmez**, yalnız hedeflerin
listesini ve toplamını böler (ADR 0013). Yeni bir modül yazılmadı: karşılık,
mevcut manuel katkı mekanizmasının kapsam etiketli hâlidir.

`GET /api/v1/goals?scope=business` filtreli okumadır ve **kapsamsız hedefleri de
eler** — filtrenin her yerdeki kuralı bu. Filtresiz okuma bir kırılım döner
(`scopeBreakdown`): işletme, şahsi ve **etiketsiz** üç kova. Üçüncü kova aylık
raporda yoktur ama burada vardır, çünkü hedef kapsam taşımak zorunda değildir ve
etiketsizleri bir tarafa saymak olmayan bir cevabı uydurmak olurdu. Toplamı
istemci çıkarmaz, sunucu verir.

### Doğrulama kodu: taşınan tek sır

E-posta doğrulama ve parola sıfırlama tek bir yazma modelini paylaşır
(`VerificationCode`) ve amaç (`VerificationPurpose`) kodun arandığı her sorgunun
parçasıdır: doğrulama için üretilmiş bir kodla parola sıfırlanamaz.

Altı haneli bir kod tek başına zayıftır. Onu güvenli kılan üç sınır aggregate'in
kendi içindedir ve birlikte çalışır: **süre** (15 dakika), **tek kullanım** ve
**deneme sayısı** (beş yanlıştan sonra kod ölür). Sonuncusu olmadan hız sınırı
tek başına yetmezdi — saldırgan aynı kodu günlerce tahmin edebilirdi.

Kodun kendisi saklanmaz; refresh token'da olduğu gibi yalnız **SHA-256 hash'i**
kolonda durur, API cevabında dönmez ve loglanmaz. Yeni kod üretmek eskisini
tüketilmiş sayar: aynı anda iki geçerli kod, birini gören saldırgana ikinci bir
şans verirdi.

**Gönderici bir port'tur** (`IVerificationEmailSender`). Application yalnız ne
gönderileceğini bilir; metin, sağlayıcı ve HTTP çağrısı Infrastructure'dadır
(`BrevoVerificationEmailSender`). Bu ayrım testlerin ağa çıkmamasının sebebidir:
tek canlı sözleşme testi ortam değişkeni yokken açıkça skip olur. Anahtar yoksa
gönderim `NotConfigured` döner — bir hata değil, kapalı bir kapı: yerelde posta
göndermeden çalışmak mümkün olmalı.

**Doğrulanmamış hesap kilitlenmez.** Kayıtla birlikte kod gider ama kayıt onu
rehin almaz; posta servisi ulaşılamazsa bile hesap açılır ve kullanıcı giriş
yapar. Doğrulama, uygulama içindeki kalıcı bir uyarıyı kaldıran adımdır.

**Enumeration iki yolda da kapalıdır.** Bilinmeyen adres, bilinen adres, pasif
hesap ve az önce kod istemiş adres aynı `202`'ye döner; sıfırlamayı tamamlarken
bilinmeyen adres, yanlış kod ve süresi geçmiş kod aynı hata koduna gider.
Kullanıcıya yararlı olan tek ayrım oturum açmış kullanıcının kendi yeniden
gönderim isteğindedir (`account.verification_code_too_soon`) — orada kimlik
zaten biliniyor.

### Hesabın kendisi: oturumlar ve kapatma

`RefreshSession` bu aşamaya kadar yalnız kimlik akışının içinden okunuyordu;
artık kullanıcının görebildiği bir liste. Okuma **sahiplik kapsamlıdır**
(`FindOwnedByIdAsync`) ve satır token ya da token hash'i taşımaz — oturumun ne
zaman açıldığını ve ne zaman düşeceğini söyler, ne olduğunu değil.

Token çiftine `sessionId` eklendi. Sır değildir (token olmadan işe yaramaz) ama
istemcinin **hangi satırın kendisi olduğunu** bilmesini sağlar; bu bilgi
olmadan kullanıcı kendi oturumunu kapatıp uygulamadan düşerdi.

**Parola değişimi kimlik akışının bir parçasıdır**: başarılı olduğunda
kullanıcının bütün refresh oturumları iptal edilir ve isteği yapan cihaz
cevapla birlikte yeni bir oturum alır. Çalınmış bir parolayla açılmış oturumun,
parola değiştikten sonra yaşamaya devam etmemesi kuralı budur; taze çift ise
parolasını değiştiren kullanıcının kendi uygulamasından atılmamasıdır.

**Hesap silme ADR 0017'yi uygular.** `IUserAccountEraser` Application'da bir
port, `EfUserAccountEraser` Infrastructure'da tek transaction'lı gerçek silme.
Şemadaki bütün foreign key'ler `Restrict` olduğu için sıra açıkça yazılıdır
(çocuktan ebeveyne); dosyalar veritabanının dışında yaşadığı için anahtarları
silmeden önce okunur, dosyaların kendisi transaction kapandıktan sonra silinir.
Sıra yalnız bu tek sınıfta bilinir ve gerçek SQL üzerinde çalışan bir testle
korunur.

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
  ├─ gerçekleşmemiş installment item
  ├─ ödenecek borç taksidi
  └─ açık tek seferlik ödenecek yükümlülük

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

Plan isteğe bağlı iki bitiş sınırı taşır: `EndDate` ve toplam occurrence sayısı
`OccurrenceLimit`. İkisi birlikte verilebilir; üretim tarihi veya sayaçtan önce
dolana göre biter. `GeneratedOccurrenceCount` gerçekleşen para hareketini değil,
üretilmiş occurrence satırını sayar. Sınıra ulaşınca `NextOccurrenceDate=null`
olur ve plan pasifleşir; geçmiş occurrence'lar silinmez. Planlanan projection da
aynı sayaçtan sonra tarih türetmez. Mevcut planların sayacı migration sırasında
owner + plan anahtarıyla bağlı occurrence satırları sayılarak backfill edilir;
kalıcı veritabanı varsayılanı bırakılmaz.

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

## Cihaz üstü hatırlatma mimarisi (Aşama 06 Grup 3)

Hatırlatma tamamen **istemci tarafındadır**; backend'de tek satır kod veya tek
kolon şema değişmedi.

```text
ActivityRepository.listPlanned(30 gün)   ← kanonik planlanan projection
        │
        ▼
ReminderPlanner (saf)  ── gün başına tek bildirim, kova sayıları
        │
        ▼
NotificationSchedulerContract (port)
        │
        ▼
LocalNotificationScheduler → flutter_local_notifications → Android AlarmManager
```

Kurallar:

- **İkinci bir vade mantığı yoktur.** Hangi kaydın ne zaman ödeneceğine
  planlanan projection karar verir; istemci o listeyi zamanlayıcı biçimine
  çevirir. Vade kuralı istemcide tekrarlansaydı iki gerçek doğardı.
- **Eklenti kenardadır.** `ReminderPlanner` saf bir sınıftır ve
  `NotificationSchedulerContract` bir porttur; controller ve planlayıcı
  testlerinin hiçbiri platform kanalına dokunmaz.
- **Yeniden kurma, tek tek iptalin yerine geçer.** Her senkronizasyon
  `cancelAll` ile başlar ve listeyi baştan yazar; ödenen kalemin bildirimi
  listede olmadığı için kendiliğinden düşer. Senkronizasyon iki yerden tetiklenir:
  oturum açılışı ve `FinancialDataChanges.planningRevision`. Yalnız planlanan
  hedefe bağlıdır — gün sonu sayımı gibi hatırlatmayı ilgilendirmeyen bir
  mutation telefonun bildirimlerini yeniden yazdırmamalı.
- **Ayar cihazda yaşar** (`flutter_secure_storage`, `ScopePreferences` ile aynı
  gerekçe) ve oturum kapanınca silinir.
- **Bildirim gövdesi finansal veri taşımaz**: tutar ve karşı taraf adı geçmez.
- **Kesin alarm istenmez.** `inexactAllowWhileIdle` kullanılıyor;
  `SCHEDULE_EXACT_ALARM`/`USE_EXACT_ALARM` bildirilmiyor. Sabah hatırlatmasının
  saniye hassasiyetine ihtiyacı yok ve kesin alarm Android 14'ten beri
  kullanıcıyı ikinci bir izin ekranına sokuyor.
- **Zaman dilimi cihazın o anki UTC farkından kuruluyor**; IANA konum adını
  öğrenmek için üçüncü bir native bağımlılık (`flutter_timezone`) eklenmedi.
  Bedeli tek ve sınırlı: yaz saati geçişinin öbür tarafına kurulmuş bir
  hatırlatma bir saat kayar. Kendi kendini toparlar — liste uygulama her
  açıldığında ve veri her değiştiğinde o anki farkla yeniden kurulur.

Sunucudan cihaza push (FCM) bu aşamada **eklenmedi**: sunucu ayakta değilken
hiçbir şey göndermez ve uygulama bugün yalnız geliştirme makinesi açıkken
çalışıyor. Karar Aşama 07'de yeniden açılır.

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
| Uygulama → cihaz bildirimi | Kilit ekranında görünen metin | Gövdede tutar ve karşı taraf adı yok; yalnız tür ve adet |
| API → Gemini | Fiş byte'ları ve güvenilmeyen model yanıtı | Sunucuda secret, `store=false`, timeout/retry, yapılandırılmış JSON, altı kademe doğrulama; fotoğraf/yanıt loglanmaz veya sağlayıcı etkileşim geçmişinde tutulmaz |

Native Android istemci tarayıcı olmadığı için CORS güvenlik sınırı değildir.
Yerel web istemcisi tarayıcı sınırındadır: yalnız Development ortamında,
`WebClient:AllowedOrigins` altındaki kesin origin'ler kabul edilir; politika
authentication veya authorization yerine geçmez.
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

## Hata cümlesini istemci üretir

Sunucu kararlı bir **makine kodu** gönderir (`transactions.not_found`,
`budgets.scope_unresolved`); o kodun hangi dilde, hangi tonda ve hangi
yönlendirmeyle okunacağı istemcinin işidir. `ProblemDetails` içindeki `detail`
alanı bu yüzden **hiçbir koşulda ekrana yazılmaz**.

Kural yeni değil ama uygulaması eksikti: 27 Ağustos 2026 kabul turunda
`The scope could not be resolved from the request, the account or the category.`
cümlesi Türkçe arayüzde göründü. Sözlükte karşılığı olmayan her kod sunucunun
kendi cümlesine düşüyordu ve o cümleler geliştiriciye konuşuyor, çoğu İngilizce
ve hiçbiri kullanıcının o ekranda ne yapabileceğini bilmiyor.

Çeviri tek yerde (`lib/core/network/api_error_messages.dart`) ve üç katmanlı;
ilk tutan kazanır:

| Katman | Ne yapar | Örnek |
|---|---|---|
| Tam kod | Kendi cümlesini hak eden kod. Ölçüt: kalıptan çıkan cümle kullanıcıya **ne yapacağını** söyleyemiyorsa buraya yazılır | `recurring.has_realized_history` → "…silinemez. Durdurmak için planı duraklatabilirsiniz." |
| Kalıp | Kodlar `<alan>.<sebep>` biçiminde ve sebep tarafı tekrar ediyor. Alanın Türkçe adı sebebin şablonuna konur | `counterparties.not_found` → "Cari hesap bulunamadı…" |
| Nötr yedek | Hiçbiri tutmazsa. Bilmediği hatayı tarif etmeye çalışmaz | "İşlem tamamlanamadı. Lütfen tekrar deneyin." |

Üçüncü katman kuralın **kapısıdır**: istemci güncellenmeden sunucuya yeni bir
kod eklendiğinde kullanıcı İngilizce bir cümle değil, anlamlı ve nötr bir cümle
görür. Envanteri tek tek eşlemek yerine kalıp katmanının olması da bunun içindir
— backend bugün ~250 kod üretiyor ve her birine elle cümle yazan bir sözlük,
ilk yeni koddan sonra yine sunucunun cümlesine düşerdi.

`ApiException.message` bu yüzden daima sözlükten gelir; `ApiException.local`
istemcinin kendi hatalarını (ağ, zaman aşımı, biçimsiz cevap) aynı yoldan
geçirir. Kapı testi: hiçbir kod İngilizce bir cümleye dönmemeli
(`test/core/network/api_error_messages_test.dart`).

## Log ve hata sınırı (Aşama 06.1 Grup 4)

Kullanıcının parası, kimliği ve sırrı **log gövdesine girmez**; hata cevabı da
**sunucunun iç yapısını anlatmaz**. İkisi ayrı sınırdır ve ayrı kapıları var
(`LeakageTests`).

Uygulamanın kendi log satırları bilerek yoksuldur: ele geçmemiş bir istisna
yalnız **türünü** ve `traceId`'sini yazar, mesajını ve yığınını değil; posta
sağlayıcısı yalnız sonucu yazar, adresi ve kodu değil. Bu, log seviyesinden
bağımsız bir kuraldır ve kapısı da öyle ölçer: bütün süzgeçler kaldırılıp
`Trace`'e inildiğinde bile `BusinessFinance.*` kategorilerinde hassas bir değer
geçmemeli.

Framework tarafında bir sınır daha var ve o **yapılandırmadadır**: ASP.NET Core
istek satırını **sorgu dizesiyle birlikte** `Information` seviyesinde loglar.
Bugün sorgu dizesinde yalnız kimlik, tarih, enum ve bayrak taşınıyor — hiçbir
tutar, açıklama veya ad oraya konmuyor. `Microsoft.AspNetCore` kategorisinin
`Warning`'de kalması bu yüzden bir gürültü tercihi değil, sınırın kendisidir ve
testle tutulur.

Hata cevabı tarafında ProblemDetails yalnız `title`, `detail`, `code` ve
`traceId` taşır; yığın izi, SQL metni, iç dosya yolu ya da framework sürümü
taşımaz. **Development ve Production ayrı ayrı** ölçülür: geliştirme profiline
özel bir ayrıntılı hata sayfası yoktur ve yanlış ortamda çalışan bir sunucu bu
yüzden fazladan bir şey anlatmaz. `traceId` bilerek dışarıda tutulur — içerik
taşımaz ve kullanıcının destekle konuşurken söyleyebileceği tek şeydir.

İstemci tarafında ekrana çıkan cümlenin kaynağı ikidir: `ApiErrorMessages`
sözlüğü ya da ekranın kendi sabit Türkçe metni. Yakalanan hatanın kendisini
metne çevirmek (`$error`, `error.toString()`) sunucunun iç yapısını ekrana
taşırdı; kaynağı tarayan bir test bunu tutar
(`test/core/network/error_surface_test.dart`). `error.message` serbesttir,
çünkü o değer sunucudan gelmez — sözlüğün ürettiği cümledir.

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
