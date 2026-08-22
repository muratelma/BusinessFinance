# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Oturum başlangıcı

Kod veya dosya değişikliğinden önce **sırasıyla** oku:

1. **`AGENTS.md` — zorunlu, ilk sırada.** Reponun kalıcı çalışma kuralları
   (iletişim, git, mimari, Flutter, güvenlik, belge sorumluluğu, kalite kapıları)
   oradadır. Bu dosyayı okumadan kod veya belge değiştirme.
2. `docs/project-status.md` — gerçekte nerede kalındığı (en güncel gerçek kaynak)
3. `stages/README.md` — aşama zinciri; hangi belgenin **Aktif** olduğu
4. O aktif `stages/<numara>-*.md` belgesi — aşamanın ayrıntılı planı
5. `git status --short --branch` ve `git log -3 --oneline` ile Git gerçeğini doğrula

Ürün kapsamı veya aşama sırası tartışma konusuysa `PRD-BusinessFinance.md`
ve `PROJECT-ROADMAP.md` de okunur. Kullanıcının mevcut değişikliklerini silme,
taşıma veya üzerine yazma.

**Aktif aşama yoksa kod değişmez.** Zincirdeki altı belge de yazılı; belgenin
var olması onu başlatma izni değildir.

Çelişen bilgi görülürse `AGENTS.md` içindeki **"Çelişen bilgi: iki ayrı eksen"**
bölümü uygulanır. Kısaca: "kod ne durumda?" sorusunda Git gerçeği en üsttedir
ve hiçbir belge onu ezemez; "ne yapılmalı?" sorusunda PRD ve ADR'ler en
üsttedir. İkisi tek listede sıralanmaz.

Bu dosya `AGENTS.md` kurallarının bir **özetini** taşır, çünkü her oturumda
yüklenir; ayrıntı ve gerekçe `AGENTS.md` içindedir ve ikisi çeliştiğinde
`AGENTS.md` geçerlidir.

## Proje modeli

Flutter istemcisi ve ASP.NET Core backend'iyle çalışan, çok kullanıcılı bir
işletme finansı uygulaması. Hedef kitle şahıs şirketleri ve esnaf: şirketin
kasası ile sahibinin cebi hukuken ayrılmadığı için gündelik gider takibi ile
işletme takibi aynı üründe yaşar. Backend ve Flutter tarafı aynı şekilde
geliştirilir; katmanlar arasında farklı bir çalışma biçimi yoktur.

Kurucu karar: **işletme ve şahsi para tek havuzda yaşar**, ayrım bir raporlama
boyutudur (ADR 0013). Geliştirme zinciri altı aşamadır; sırası ve gerekçesi
`PROJECT-ROADMAP.md` içinde.

## Build, test ve çalıştırma komutları

### Backend (.NET, kök dizinden)

```bash
dotnet build BusinessFinance.slnx --configuration Release --no-restore
dotnet format BusinessFinance.slnx --verify-no-changes --no-restore
dotnet test BusinessFinance.slnx --configuration Release --no-restore
# Tek katman testi:
dotnet test src/BusinessFinance.Domain.Tests --configuration Release
# Tek test:
dotnet test src/BusinessFinance.Domain.Tests --filter "FullyQualifiedName~Money"
```

Gerçek SQL bağımlı testler `[SqlServerFact]` ile işaretlidir ve
`BUSINESS_FINANCE_SQL_TEST_CONNECTION` ortam değişkeni yoksa otomatik skip
olur (sessizce başarısız değil — skip). Yerel SQL Server çalışıyorsa:

```bash
docker compose up -d          # sqlserver container'ını başlatır (127.0.0.1:14334)
docker compose ps             # healthy durumunu kontrol et
```

EF Core migration (design-time context, `ConnectionStrings__BusinessFinance`
environment değişkeninden bağlanır — secret kaynak kodda tutulmaz):

```bash
dotnet ef migrations add <Name> --project src/BusinessFinance.Infrastructure --startup-project src/BusinessFinance.Api
dotnet ef database update --project src/BusinessFinance.Infrastructure --startup-project src/BusinessFinance.Api
```

Şema `InitialCreate` ile kuruldu; devralınan yükseltme zinciri taşınmadı
(ADR 0012). **Zincirin güncel hâli migration klasörüdür**, bu dosya sayı tutmaz.
Bundan sonra eklenen her migration gerçek bir yükseltme yoludur ve `AGENTS.md`
içindeki migration kurallarına uyar (backfill CHECK'ten önce, kolonlar kısıttan
önce, geçmişi bilinmeyen bilgi nullable; tek istisna gerçekten boş tablodur).

### Flutter (`mobile/business_finance_mobile/` içinden)

```bash
flutter analyze
flutter test
flutter test test/features/accounts/account_controller_test.dart   # tek dosya
flutter build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5284
dart format --set-exit-if-changed lib test
```

`dart format .` kullanılmaz: `build/` altındaki APK artefaktlarını da tarar,
uzun Gradle yollarında `PathNotFoundException` ile yarıda kalır ve yine de
exit 0 döner. Kaynak dizinleri açıkça verilir.

`API_BASE_URL` build-time `--dart-define`'dır, secret değildir. Android
emulator'den Windows host API'sine ulaşmak için `10.0.2.2` kullanılır (Pixel 8
AVD ana geliştirme/kabul cihazıdır).

### Yerel ortam ilkeleri

- **Uygulamanın henüz gerçek kullanıcısı ve gerçek verisi yok**; yerel
  veritabanındaki her kayıt sentetiktir ve gerektiğinde sıfırlanabilir. Ancak
  ürün ticari olarak sunulabilir: veri kaybettiren veya geri döndürülemeyen bir
  şema kararı artık varsayılan olarak kabul edilmez, gerekçesiyle kullanıcıya
  sorulur ve `docs/project-status.md` içine yazılır. Ayrıntı ve migration
  yükseltme kuralları `AGENTS.md` içindedir. Davranış kurallarını hiç
  gevşetmez — silme yerine iptal, çifte sayım yasağı ve sahiplik izolasyonu
  ürün kurallarıdır.
- Gerçek SA parolası ve connection string **yalnız** Git dışı `.env` /
  local user-secrets'ta tutulur; terminale veya belgeye asla yazılmaz.
  `scripts/New-LocalSqlEnvironment.ps1` değeri ekrana basmadan üretir.
- Geliştirme/test verisi daima sentetik veridir; gerçek finansal veri
  güvenlik ve geri yükleme kapısı tamamlanmadan kullanılmaz.
- Ürün Aşama 06 tamamlanmadan internete açılmaz; API ve SQL yalnız
  loopback'e bind'lıdır.

## Mimari — katmanlı monolit (mikroservis yok)

```
Flutter uygulaması
      │
      ▼
ASP.NET Core API (composition root, ProblemDetails, JWT bearer, rate limit)
      │
      ├── Application  (use case'ler, port'lar: I*Repository, ICurrentUser,
      │                 IIdentityAccountService, ISecurityTokenService)
      ├── Domain        (framework bağımsız: Money, Account, Category,
      │                 BudgetTransaction, MonthlyBudget, Transfer,
      │                 CreditCard/Charge/Payment, InstallmentPlan,
      │                 RecurringTransaction/Occurrence (+RecurringSourceType),
      │                 DebtAgreement/Installment, RefreshSession)
      └── Infrastructure (EF Core SQL Server, ASP.NET Core Identity,
                          `BusinessFinanceDbContext`, Ef*Repository adapter'ları)
              │
              ▼
          SQL Server (Docker named volume, 127.0.0.1:14334)
```

**Bağımlılık yönü katıdır:** Domain hiçbir framework/EF Core/HTTP/Flutter
bağımlılığı taşımaz (mimari testle korunur). Application yalnız Domain'e
referans verir ve DI framework'ü içermez. Infrastructure Application'a
bağımlıdır; port implementasyonları (Ef*Repository, Identity adapter, JWT
servisleri) burada yaşar. API, Application + Infrastructure'ı composition
root'ta birleştirir.

Genel repository, MediatR/CQRS gibi soyutlamalar somut ihtiyaç olmadan
eklenmez — bu proje kuralı `AGENTS.md`'de sabittir, "gelecekte lazım olur"
gerekçesiyle bozulmaz.

### Kullanıcı izolasyonu / yetkilendirme deseni

- Kullanıcı kimliği **asla** request body/query string'den alınmaz; her
  use case `ICurrentUser` (JWT `sub` claim'inden türetilir) üzerinden alır.
- Tek kayıt okuma/güncelleme repository çağrıları `(UserId, kayıt ID)`
  composite anahtar/foreign key ile sahiplik kapsamı taşır. Başka kullanıcıya
  ait veya var olmayan kayıt aynı `NotFound` sonucuna gider — admin/rol bypass
  yolu yoktur.
- Bu izolasyon hem Application seviyesinde (owner predicate) hem SQL
  seviyesinde (composite FK, unique index) iki bağımsız kapıyla doğrulanır.

### Finansal domain kuralları (sık karışan noktalar)

- **Bakiye kalıcı kolon değildir.** `Account.OpeningBalance` + owner-scoped
  iptal edilmemiş hareketlerden her sorguda hesaplanır. Bunu ikinci bir
  "current balance" alanı olarak eklemeyin.
- **Transfer, kredi kartı ödemesi ve taksit gerçekleşmesi normal
  gelir/gider değildir** — raporları bozmadan ayrı modellenir:
  - `Transfer`: kaynak −tutar, hedef +tutar, gelir/gider raporuna 0 etki.
  - `CreditCardCharge`: kart borcu +tutar VE aylık gider +tutar (gider budur).
  - `CreditCardPayment`: hesap bakiyesi −tutar, kart borcu −tutar, gider 0
    (aynı harcamanın iki kez sayılmasını önler).
  - `InstallmentPlan`: yalnız niyettir; yalnız `realize()` edilen item gerçek
    `CreditCardCharge` ve gider üretir.
- **Silme yerine pasifleştirme/iptal.** Account/Category/BudgetTransaction
  fiziksel silinmez (finansal geçmişi bozar); pasifleştirilir veya UTC
  zaman damgalı idempotent iptal edilir. Düzeltme = eski hareketi iptal et +
  yeni doğru hareket oluştur. İstisna: hiçbir finansal kayıt tarafından
  referans edilmeyen boş hesap `DELETE` edilebilir (409 conflict + pasife
  alma yönlendirmesi diğer durumda).
- **Para her yerde `decimal(19,4)`** (backend) / dört ondalıklı **string**
  (API sözleşmesi, JSON number değil — hassasiyet kaybını önler). Flutter
  tarafı finansal toplamı ikinci kez hesaplamaz; backend cevabını gösterir.
  **Ekranda iki basamak gösterilir** (`MoneyText`); yuvarlama yalnız
  gösterimdedir, saklanan ve gönderilen değer hep dört basamaktır.
- **Recurring/planning**: tanım (`RecurringTransaction`) bakiye/rapor etkisi
  üretmez; `generate(throughDate)` owner+dönem anahtarıyla idempotenttir
  (retry ikinci kayıt oluşturmaz); yalnız açık onayla `BudgetTransaction`
  olur (`RecurringTransactionOccurrence` → realize).
- Ekstre (statement) kalıcı tablo değildir — kesim/ödeme tarihlerinden asOf
  anında hesaplanan projection'dır.
- **Tekrarlayan planın kaynağı hesap veya kredi karttır** (`RecurringSourceType`,
  tam olarak biri dolu; hem Domain invariant'ı hem SQL check constraint'i).
  Gelir yalnız hesaba bağlanır. Gerçekleşme kaynağa göre dallanır: hesap →
  `BudgetTransaction`, kart → `CreditCardCharge`; occurrence tam olarak tek
  sonuç kimliği taşır ve tek `SaveChanges` sınırında yazılır.
- **Pasif plan para üretmez.** Bekleyen (gerçekleşmemiş) occurrence, planı
  pasifse listelerden ve planlanan görünümden düşer; gerçekleştirme
  `409 recurring.schedule_inactive` ile reddedilir. Gerçekleşmiş occurrence
  geçmiştir, kalır.
- **Plan silme**: hiç gerçekleşmiş occurrence yoksa plan ve bekleyen kayıtları
  silinebilir; varsa `409 recurring.has_realized_history` + duraklatma
  yönlendirmesi (boş hesap silme kuralının aynısı).
- **Kayıt adı kategoriden değil kullanıcının yazdığından gelir**: `title =
  açıklama ?? kategori adı`. Kategori paylaşılan bir raporlama kovasıdır;
  kimlik taşımaz (aynı kategorideki üç abonelik aksi halde ayırt edilemez).
- **Kapsam (`TransactionScope`) ayrı bir boyuttur**, kategoriyle temsil
  edilmez. `Business = 1`, `Personal = 2`; üçüncü bir "bilinmiyor" değeri
  yoktur. Gelir/gider raporunu etkileyen kayıt kapsamı **zorunlu** taşır
  (`BudgetTransaction`, `CreditCardCharge`, `MonthlyBudget`, `InstallmentPlan`,
  `RecurringTransaction` + occurrence'ı, `DebtAgreement`). `Transfer` ve
  `CreditCardPayment` **taşımaz** — gelir/gider raporuna sıfır etki ederler,
  kapsam sormak cevabı hiçbir yerde kullanılmayan bir soru olurdu.
  `Account`, `Category` ve `CreditCard` **nullable** bir varsayılan kapsam
  (`DefaultScope`) taşır; boş olması meşrudur, eksik veri değildir.
- **Kapsam raporu böler, parayı bölmez** (ADR 0013): bakiye, kart borcu ve net
  varlık kapsam filtresinden etkilenmez. Bölünen tek şey gelir/gider
  toplamlarıdır. Bütçe ilerlemesi kapsama duyarlıdır: harcama kategoriyle
  değil, **kategori + kapsam çiftiyle** toplanır.
- **Plan kapsamı gerçekleşmede yeniden türetilmez.** Tekrarlayan plan ve taksit
  planının ürettiği kayıt kapsamı plandan alır; aksi hâlde aynı plan farklı
  aylarda farklı kapsam üretebilirdi.

> **Henüz uygulanmadı — Aşama 01'in kalanı.** Varsayılan kategori seti hâlâ
> tamamen ev bütçesi (Grup 5) ve kapsam sunucuda **türetilmiyor**: istek
> kapsamı açıkça göndermek zorunda (Grup 3). Flutter tarafında kapsam anahtarı
> ve formdaki kapsam çipi de yok (Grup 7). Bu satır o gruplar bitene kadar
> burada durur.

### Birleşik okuma modelleri

Gerçekleşmiş bütün ekonomik olaylar tek bir okuma projection'ında birleşir;
**kalıcı `FinancialActivity` tablosu yoktur.**

```text
GET /api/v1/financial-activities          gerçekleşmiş geçmiş
GET /api/v1/financial-activities/planned  henüz gerçekleşmemişler (7/30/90 gün)
```

- Gerçekleşmiş feed altı yazma modelini EF Core `Concat` (`UNION ALL`) ile **tek
  SQL sorgusunda** birleştirir; `OrderBy`/`Skip`/`Take`/`Count` veritabanına
  iner. Bellekte birleştirme yasaktır: bounded query-count ölçüsünü geçerken
  her sayfada tüm geçmişi okur.
- Hareket beş bağımsız boyutla sınıflanır: `activityKind`, `effect`,
  `sourceGroup`, `origin`, `status`. Tek enum'a indirgenmez.
- `canCancel` yalnız `activityKind + origin + status`'tan hesaplanır ve hem
  feed'de raporlanır hem iptal endpoint'lerinde uygulanır: recurring/installment
  kaynaklı hareket `409 *.cancel_origin_locked` döner.
- Planlanan projection kanonik kaynaktır; `IUpcomingPaymentRepository` ikinci
  bir sorgu tutmaz, aynı projection'ın **daraltılmış görünümünü** okur.
  `readiness`/`attentionCode` kalıcı değil, kaynağın güncel durumundan türetilir.
- Backup şemasının **güncel sürümü ve hangi sürümleri okuduğu tek yerde**
  tutulur: `documentation/restore-runbook.md`. Buraya kopyalanmaz — aşama
  zinciri boyunca her aşama sürümü ilerletiyor ve iki yerde tutulan sürüm
  numarası kaçınılmaz olarak ayrışır.

### Auth

Access token: HS256 JWT, 15 dk, yalnız `sub`/`email`/`jti`. Refresh token:
64 rastgele byte, sunucuda yalnız SHA-256 hash'i saklanır, rotation + reuse
tespiti var (reuse → kullanıcının tüm aktif sessionları kapatılır). Bilinmeyen
e-posta/yanlış parola/pasif kullanıcı aynı genel `authentication.invalid_credentials`
sonucuna döner (enumeration önleme). Auth endpoint'leri IP başına dakikada 10
istekle sınırlıdır.

### Flutter katmanları

```
XxxPage → XxxController (state/loading/error/unauthorized/stale) → XxxRepository → authenticated ApiClient
```

`lib/features/activities/` birleşik feed, planlanan görünüm, ayrıntı bottom
sheet'i ve ortak `İşlem ekle` launcher'ını barındırır. Ekranda gösterilen tür
etiketleri (`Kart harcaması`, `Tekrarlayan plan`) **Flutter'da** üretilir; API
kararlı makine değerleri gönderir, kullanıcı cümlesi göndermez.

`FinancialDataChanges` altı hedef taşır (`activityFeed`, `dashboard`, `budgets`,
`accounts`, `cards`, `planning`); her mutation yalnız etkileyebileceğini
yükseltir (ör. kart ödemesi bütçeyi yükseltmez — aynı harcama iki kez sayılırdı).

`AppDependencies` composition root'unda kurulur (provider ile). Her feature
API DTO'larını açık Dart sınıflarıyla temsil eder — ham `Map`/JSON geçirilmez.
Loading/empty/error/unauthorized/stale-cache durumları her ekranda görünür ele
alınmalıdır (proje genelinde tekrarlayan kabul kriteri). Token/oturum bilgisi
yalnız `flutter_secure_storage`'da tutulur; connection string veya sunucu
secret'ı uygulamaya konmaz.

## Belge haritası

- `documentation/architecture.md` — doğrulanmış mimari, güven sınırları, ADR'ler
- `documentation/flows.md`, `permissions.md`, `variables.md`, `tests.md` —
  akışlar, izin/izolasyon kanıtı, secret envanteri, test kapsam haritası
- `documentation/local-setup-and-acceptance.md` — Windows/SQL/API/emulator
  kurulumu ve manuel kabul adımları
- `documentation/restore-runbook.md` — backup/restore prosedürü
- `documentation/financial-activity-api-contract.md` — birleşik feed, planlanan
  görünüm ve recurring source sözleşmeleri, örnek JSON, hata kodları
- `documentation/design-system.md` — token'lar, pencere sınıfları, bileşen
  kataloğu, erişilebilirlik kuralları ve yeni ekran kontrol listesi
- `documentation/adr/0008-two-tone-financial-roles.md` — finansal rol başına
  metin ve dolgu tonu ayrımı, nötr rolün maviye dönmesi
- `documentation/adr/0011-receipt-reading-is-a-suggestion-layer.md` — fiş
  okumada modelin sınırları: öneri katmanıdır, yönü ve ödeme kaynağını seçmez
- `documentation/adr/0012-single-initial-migration.md` — zincirin neden
  taşınmadığı ve bu serbestliğin bir daha kullanılmayacağı
- `documentation/adr/0013-business-and-personal-are-one-pool.md` — **zincirin
  kurucu kararı**: işletme ve şahsi tek havuzda bir boyuttur; mod seçimi ve
  iki veri alanı reddedildi
- `documentation/receipt-analysis-api-contract.md`,
  `documentation/receipt-measurement.md` — fiş analizi sözleşmesi ve ölçüm yöntemi
- `PROJECT-ROADMAP.md` — altı aşamalık zincir, bağımlılık kuralları, kapsam
  dışı bırakılanlar ve gerekçeleri, yedek şeması sürüm politikası
- `stages/README.md` — aşama zinciri, hangi belge aktif, yeni aşama açma ve
  biten aşamayı kapatma adımları; **aktif aşama kullanıcı onayı olmadan
  değişmez**
- `stages/01-*.md` … `stages/06-*.md` — altı aşamanın çalışma grupları,
  testleri ve çıkış koşulları
- `docs/backlog.md` — aşamaya bağlanmamış açık işler
- `templates/STAGE-TEMPLATE.md` — yeni aşama belgesi iskeleti

Hangi değişiklikte hangi belgenin güncelleneceği `AGENTS.md` içindeki **belge
güncelleme haritası** tablosundadır; kod ve belge aynı commit'te güncellenir.

## Kritik kısıtlar (ihlal etmeyin)

- **Aşama belgesinin var olması onu başlatma izni değildir.** Kod yalnız durumu
  `Aktif` olan aşamada değişir; aktif aşama kullanıcı onayı olmadan değişmez.
- **Bazı aşamalar bir ADR ile açılır** (`PROJECT-ROADMAP.md` tablosu). O ADR
  yazılıp kabul edilmeden ilgili aşamanın koduna başlanmaz.
- **Uygulama vergi hesaplamaz, beyanname üretmez ve "kâr" demez.** Vergi
  alanları taşır ve raporlar; hesaplanan şey nakit esaslı **işletme netidir**.
- **İşletme/şahsi havuzu bölünmez** (ADR 0013): mod seçimi, ayrı veri alanı ve
  kapsamın kategoriyle temsili reddedildi. Bakiye, kart borcu ve net varlık
  kapsam filtresinden etkilenmez.
- Yeni aşamanın paket/servis/altyapısını erkenden ekleme (bulut ve offline
  cache bağımlılıkları kendi aşamaları gelmeden kurulmaz).
- **Banka bağlantısı / açık bankacılık kapsam dışıdır**; sağlayıcı SDK'sı,
  adapter veya sandbox eklenmez, ödeme başlatma hiçbir koşulda eklenmez.
- Migration ve API sözleşme değişikliklerini incelemeden uygulamayın.
- **Kullanıcı istemedikçe yeni branch açmayın.** Çalışma, kullanıcının o an
  bulunduğu branch üzerinde yapılır; branch/worktree oluşturma, branch değiştirme
  ve silme yalnız kullanıcı açıkça istediğinde yapılır.
- **Commit mesajları İngilizce, tipi dürüst.** Özellik eklenmediyse `feat`
  yazılmaz — taşıma/yeniden adlandırma/altyapı `chore`, davranış değiştirmeyen
  düzenleme `refactor`, düzeltme `fix`.
- **Tek başına belge commit'i atmayın.** Commit yalnız uygulamada gerçek bir
  geliştirme veya düzeltme olduğunda atılır; belge güncellemesi o kod
  değişikliğiyle aynı commit'e girer. **İstisna: karar belgeleri**
  (`PRD-BusinessFinance.md`, `PROJECT-ROADMAP.md`, `stages/`,
  `documentation/adr/`, `AGENTS.md`, `CLAUDE.md`) kendi başlarına commit
  edilebilir — kodun kaydı değil, kodun kararıdırlar. `documentation/`
  altındaki diğer belgeler girmez. Ayrıntı `AGENTS.md` "Git sorumluluğu".
- **Yapay zekâ imzası bırakmayın.** Commit mesajı, commit gövdesi, branch adı,
  tag, PR başlığı/gövdesi, issue veya kod yorumu — hiçbirinde `Claude`,
  `Claude Code`, `Anthropic`, `AI`/`agent` imzası, `Co-Authored-By:` satırı ya
  da "Generated with …" ibaresi yer almaz. Bu kural, aracın kendi varsayılan
  commit/PR şablonlarını (`Co-Authored-By: Claude …`, `🤖 Generated with
  Claude Code`) **geçersiz kılar**: o satırlar hiçbir koşulda eklenmez.
  Commit yazarı daima kullanıcıdır; depo GitHub'da tamamen insan katkısı
  olarak görünür. `Co-authored-by` yalnız gerçek bir insan katkıda bulunduysa
  kullanılır. `CLAUDE.md` dosya adı bu kuralın istisnasıdır (dosyanın kendi
  adıdır); yine de commit mesajında `docs(claude)` gibi bir kapsam yerine
  nötr bir kapsam (ör. `docs(guide)`) tercih edilir.
- Gerçek stage/test durumu için her zaman `docs/project-status.md`'nin en
  güncel bölümüne ve `git log`'a bakın — bu dosyadaki mimari özet donmuş bir
  anlık görüntüdür, canlı ilerleme kaydı değildir.
