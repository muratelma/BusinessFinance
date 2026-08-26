# Proje Durumu

Bu belge yalnız doğrulanmış proje gerçeğini kaydeder. Planlanan işler,
uygulanmış veya tamamlanmış gibi gösterilmez.

## Aktif konum

- **Repo 21 Ağustos 2026'da kuruldu.** Kod tabanı, kişisel bütçe projesinden
  (`Kisisel-Butce-Mobil`) tek temiz commit olarak taşındı; ürün yönü şahıs
  şirketi ve esnaf finansına çevrildi. Eski repo dokunulmadan arşiv olarak
  duruyor ve geçmiş kaydı orada
- **22 Ağustos 2026: ürün yönü ve aşama zinciri kararlaştırıldı.** Belgeler
  yeniden yazıldı; kod değişmedi
- **Aşama 01 — Kapsam boyutu ve işletme kimliği: tamamlandı** (22 Ağustos'ta
  açıldı, 23 Ağustos 2026'da kullanıcı onayıyla kapandı). Dokuz çalışma
  grubunun hepsi bitti, cihaz kabul turu yürütüldü. Belge
  `docs/archive/stages/01-kapsam-boyutu-ve-isletme-kimligi.md` altına taşındı
  ve tamamlanma kaydı orada
- **Aşama 02 — Cari hesap: karşı taraf ve açık bakiye: tamamlandı**
  (23 Ağustos'ta açıldı, 24 Ağustos 2026'da kullanıcı onayıyla kapandı).
  Belgesi `docs/archive/stages/02-cari-hesap-ve-karsi-taraf.md` altına taşındı
  ve tamamlanma kaydı orada. Sekiz çalışma grubu: **Grup 1–5
  tamamlandı**: karar kapısı (ADR 0014) kabul edildi, cari hesabın domain
  katmanı yazıldı, üç tablo kalıcılığa girdi, cari bakiye tek sorgulu bir
  okuma modeli olarak gerçek SQL üzerinde ölçüldü, taksitli borç modeli
  karşı tarafa bağlandı (yükseltme yolu dolu bir veritabanında test edildi)
  ve cari hesabın yazma yolu, birleşik feed'e ve raporlara katılması
  tamamlandı. **Grup 6** fiş okumanın karşı taraf önerisini bağladı ve
  **Grup 7** cari hesabın Flutter ekranlarını yazdı (liste, ayrıntı, dört
  form, `Diğer` menüsünde kendi kapısı). **Grup 8** yedek şemasını v7'ye
  taşıdı ve cari deftere kendi CSV dışa aktarımını verdi. **Sekiz çalışma
  grubunun hepsi bitti.** Fiş önerisinin görünür kabul/red rozeti eklendi ve
  cihaz kabul turu tamamlandı
- **Aşama 03 — Yükümlülük ve vade: tamamlandı** (24 Ağustos 2026'da açıldı ve
  aynı gün kullanıcı onayıyla kapandı). Belgesi
  `docs/archive/stages/03-yukumluluk-ve-vade.md` altına taşındı ve tamamlanma
  kaydı orada. Yedi çalışma grubunun hepsi bitti: yükümlülüğün domain'i ve
  kalıcılığı, cari borçlandırmaya vade, tekrarlayan planda bitiş sınırı,
  kanonik planlanan projection'a katılım, fiş okumanın "ödemedim" yolunun
  bağlanması, Flutter liste + idempotent kapanış akışı ve yedek şemasının
  v8'e taşınması. Kapanış öncesi kod denetimi iki arayüz boşluğu buldu ve
  ikisi de kapatıldı: yükümlülüğün fotoğrafsız (elle) girişi — yön seçimiyle —
  ve tekrarlayan plan formunun bitiş tarihi alanı
- **Aşama 04 — Kasa, POS ve gezinme: tamamlandı** (24 Ağustos 2026'da açıldı,
  26 Ağustos 2026'da cihaz kabul turuyla kapandı). Belgesi
  `docs/archive/stages/04-kasa-pos-ve-gezinme.md` altına taşındı ve tamamlanma
  kaydı orada. Sekiz çalışma grubunun hepsi bitti: ADR 0015, gün sonu kasa
  sayımı, POS tahsilatı ve yoldaki para, üçüncü ana sekmenin ön ayara göre
  değişmesi, `İşlem ekle` menüsünün niyet eksenine taşınması ve yedek v9
- Aktif aşama: **05 — Vergi ve muhasebeci.** 26 Ağustos 2026'da kullanıcı
  onayıyla açıldı. Belgesi `stages/05-vergi-ve-muhasebeci.md`; sekiz çalışma
  grubu. **Grup 1'in ADR'si yazıldı** (ADR 0016: vergi alanları taşır,
  hesaplamaz) ve **kullanıcı kabulü bekliyor**; kabul edilene kadar aşamanın
  koduna başlanmaz
- Kalan bir aşamanın belgesi de yazılı, durumu `Planlandı`
- Zincir: 01 kapsam boyutu → 02 cari → 03 yükümlülük/vade → 04 kasa/POS →
  05 vergi/muhasebeci → 06 bulut (`PROJECT-ROADMAP.md`)

## Taşımada yapılan ve doğrulanan işler

21 Ağustos 2026, tek oturum:

- **Yeniden adlandırma.** `PersonalBudget.*` → `BusinessFinance.*` (404 C#
  dosyası), Flutter paketi `business_finance_mobile`, Android
  `com.nef.business_finance_mobile`, veritabanı `BusinessFinance`,
  `UserSecretsId` `business-finance-api`. MethodChannel adı Dart ve Kotlin
  yakalarında birlikte değişti
- **Belge budaması.** 80 Markdown → 35. Arşiv aşama belgeleri, öğrenme
  notları, oturum notları ve taslak 13–18 aşamaları taşınmadı; eski repoda
  duruyorlar
- **Migration çökertmesi.** 43 dosya / 36.766 satır → 3 dosya / 5.924 satır,
  tek `InitialCreate`. Şema denkliği, gerçek veriyi taşıyan veritabanı ile
  sıfırdan kurulan veritabanının **543 satırlık tam dökümü** karşılaştırılarak
  kanıtlandı (kolon, indeks, CHECK, FK, PK/UQ — hepsi birebir). Backfill
  migration'larından kalan 5 artık DEFAULT kısıtı düşürüldü; şema artık modelle
  tam örtüşüyor
- **Veri korundu.** Eski veritabanı `COPY_ONLY` yedekle kopyalandı; 28 tablonun
  satır sayıları birebir aynı. `__EFMigrationsHistory` 21 satırdan 1 satıra
  indi, `database update` no-op
- **Ayrı Docker container.** `business-finance` compose projesi, kendi volume'u
  ve `127.0.0.1:14334` portu. Eski container ve volume ile hiçbir ortak nokta
  yok; yeni SA parolası ve yeni JWT imzalama anahtarı üretildi
- **CI kuruldu.** Önceki repoda `.github/workflows` boştu

### Taşımada kaybedilen test kapsamı — bilinçli

Migration zinciri taşınmadığı için ona bağlı iki test geçersiz kaldı:

- `Stage125Migration_BackfillsExistingRecurringRowsAsAccountSource` **silindi**.
  Yükseltme yolunu (v2 satırların `SourceType=Account` olarak dönüşmesi) gerçek
  SQL üzerinde doğruluyordu. O dönüşüm kopyalanan veritabanında zaten uygulanmış
  durumda ve yükseltilecek başka veritabanı yok
- `MigrationHistoryTests` **yeniden yazıldı**. Eskisi 21 migration'lık zincirin
  adlarını, sırasını ve yükseltme güvenliği kurallarını doğruluyordu; yenisi
  tekliği, modelle örtüşmeyi (`HasPendingModelChanges`) ve yapısal sayımları
  doğruluyor
- Zincirin taşıdığı **yükseltme güvenliği bilgisi kaybolmadı**: kurallar
  `AGENTS.md` içindeki "Migration kuralları" bölümüne yazıldı ve bundan sonraki
  her migration için bağlayıcı

`SourceType` invariant'ının kendisi Domain, Application ve API seviyesinde
testli kalmaya devam ediyor; kaybolan yalnız bir kereye mahsus geçmiş
dönüşümün testiydi.

## Son doğrulamalar

21 Ağustos 2026 itibarıyla, taşınmış kod tabanı üzerinde:

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **733 geçti**, 1 atlandı |
| Flutter analyze | No issues found |
| Flutter format | Temiz |
| Flutter test | **637 geçti** |
| Şema denkliği | 543/543 satır birebir |
| Veri bütünlüğü | 28 tablo, satır sayıları birebir |

Atlanan tek test `GeminiLiveContractTests` — canlı API anahtarı ortam değişkeni
istiyor, yokken kendiliğinden skip oluyor.

## Doğrulanan ortam

- .NET SDK: net10.0, çalışıyor
- Docker/Compose: `business-finance-sqlserver-1` healthy, `127.0.0.1:14334`
- SQL Server: `BusinessFinance` (veri) ve `BusinessFinanceApiSqlTests`
  (API SQL testlerinin hedefi; şema uygulandı). Infrastructure SQL testleri
  bağlantıdan yalnız sunucuyu alıp kendi geçici veritabanını kurar
- Flutter/Dart: çalışıyor, 637 test geçiyor
- Android emulator: bu oturumda çalıştırılmadı

## Güvenlik ve veri durumu

- Gerçek finansal veri kullanılmıyor; veritabanındaki her kayıt sentetiktir
- `.env` ve user-secrets Git dışında; `.gitignore` `.env`'i kapsıyor
- Eski reponun Git geçmişi denetlendi: parola, connection string veya API
  anahtarı sızıntısı **yok**
- İnternete açık servis yok; SQL yalnız loopback'e bind'lı

## 22 Ağustos 2026 — planlama turu

Kod değişmedi; ürün yönü ve zincir kararlaştırıldı ve belgelere yazıldı:

- **ADR 0013 yazıldı** — işletme ve şahsi tek havuzda bir boyuttur. Giriş modu
  seçimi ve iki ayrı veri alanı gerekçeleriyle reddedildi. Karar, uygulamanın
  bugünkü yüzeyi (endpoint'ler, Flutter ekranları, domain modeli) okunarak ve
  şahıs şirketinin tüzel kişiliği olmaması gerçeğinden türetildi
- **Altı aşamalık zincir kuruldu**, sırası bağımlılığa göre belirlendi (kapsam
  boyutu en altta, cari onun üstünde, fatura cari'nin üstünde) ve **altısının
  da ayrıntılı belgesi yazıldı**. Üçü bir ADR ile açılıyor: 02, 04 ve 05
- **Yeni bir çakışma bulundu ve 02–03'e bağlandı:** kod tabanı ekonomik olayı
  iki farklı zamanda tanıyor. Kart harcaması ve borç açılışı gideri **anında**
  tanırken, fiş okumanın "faturayı ödemedim" yolu hiçbir şey tanımıyor. Aynı
  fatura, hangi ekrandan girildiğine göre farklı davranıyor. Kural (ekonomik
  olay tanır, ödeme taşır) 02'de ADR olarak yazılacak, tutarsızlık 03'te
  kapatılacak
- **`PRD-BusinessFinance.md` yeniden yazıldı.** Açık bankacılık kapsam dışına
  alındı; MVP-1/1.5/2 bölümleri kaldırıldı (devralınan tabanda zaten
  uygulanmışlardı ve PRD onları "yapılacak" diye gösteriyordu)
- **Kod okumasından çıkan dört çakışma kayda geçti:** ödenmemiş faturanın
  tekrarlayan plan olmaya zorlanması, "kredi kartı"nın POS ile ters anlam
  taşıması, cari ile taksitli borç modelinin çakışma riski, varsayılan
  kategori setinin tamamen ev bütçesi olması
- **İki kural çelişkisi düzeltildi:** `main` üzerinde çalışma yasağı ile branch
  açma yasağı aynı anda uygulanamıyordu; planlama belgeleri için tek başına
  commit istisnası tanımlandı

## 22 Ağustos 2026 — Aşama 01, Grup 1: veri sıfırlama ve temiz zemin

Kullanıcı onayıyla uygulandı; geri alınamaz adımdı.

- **Yerel `BusinessFinance` veritabanı düşürüldü ve `InitialCreate` ile sıfırdan
  kuruldu.** Sonuç doğrulandı: 28 tablo ayakta, iş verisi taşıyan satır yok
  (tek satır `__EFMigrationsHistory` kaydı). Silinen her kayıt kişisel bütçe
  uygulamasından kopyalanmış sentetik veriydi
- Bu, Grup 4'ün ön koşuludur: tablolar boş olduğu için `AddTransactionScope`
  migration'ı kapsam kolonlarını backfill'siz `NOT NULL` ekleyebilir. Aynı sıra
  dolu bir tabloda kullanılamaz (`AGENTS.md`, "Migration kuralları")
- **`manual-test-data/` gözden geçirildi; değişiklik gerekmedi.** İçerik ürün
  yönünden bağımsız: iki CSV yalnız `date,amount` kolonlarıyla içe aktarma
  ayrıştırıcısını ve yinelenen satır tespitini zorluyor, üç PDF ek dosya imza
  doğrulamasının fixture'ı, `New-CorruptedBackup.ps1` geri yükleme reddini
  test ediyor. Hiçbiri ev bütçesi kategorisi veya kişisel bütçe kimliği
  taşımıyor
- **`BUSINESS_FINANCE_SQL_TEST_CONNECTION`'ın iki tüketicisinin farklı beklentisi
  olduğu bu turda ortaya çıktı** ve `documentation/variables.md` içine yazıldı:
  Infrastructure testleri bağlantıdan yalnız sunucuyu alıp kendi geçici
  veritabanını kurup migrate ediyor, API SQL testi ise bağlantıyı olduğu gibi
  kullanıyor ve şeması önceden uygulanmış bir veritabanı istiyor
  (`BusinessFinanceApiSqlTests`)

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **733 geçti**, 1 atlandı (`GeminiLiveContractTests`, canlı anahtar yok) |
| Temiz veritabanı | 28 tablo, 0 iş verisi satırı |

## 22 Ağustos 2026 — Aşama 01, Grup 2 ve 4: kapsam boyutu

`TransactionScope` (`Business = 1`, `Personal = 2`) domainden veritabanına
kadar eklendi. Üçüncü bir "bilinmiyor" değeri yok: boyut boş bir veritabanına
girdiği için yorumlanacak geçmiş de yok.

- **Kapsamı zorunlu taşıyanlar:** `BudgetTransaction`, `CreditCardCharge`,
  `MonthlyBudget`, `InstallmentPlan`, `RecurringTransaction` ve occurrence'ı,
  `DebtAgreement`. **Taşımayanlar:** `Transfer` ve `CreditCardPayment` — bir
  test alanın sonradan eklenmediğini koruyor
- **`Account`, `Category`, `CreditCard` nullable `DefaultScope` taşır** ve bu
  alan API yüzeyine de çıktı. Boş olması meşrudur; "kapsamı bilmiyorum" değil,
  "bu kaynak kapsamı belirlemiyor" demektir
- **Bütçe ilerlemesi kapsama duyarlı hâle geldi** ve kural iki yerde birden
  yazılı: `MonthlyBudget.CalculateProgress` ve `EfBudgetRepository`. İkisi de
  harcamayı kategori + kapsam çiftiyle topluyor
- **Occurrence kapsamı plandan kopyalanır**, gerçekleşmede yeniden türetilmez.
  Kopya olması planlanan projection'ın kapsamı join'siz filtrelemesini de
  sağlayacak
- **Grup 4 bu checkpoint'e alındı.** `MigrationHistoryTests` modelle şemanın
  örtüşmesini doğruluyor; kapsam modele girip migration üretilmezse grup
  kırmızı kalırdı. Aynı zorunlulukla Grup 9'un **yalnız sürüm kapısı** da
  buraya girdi — geri yükleme kodu kapsam alanı olmadan derlenmiyordu
- **`20260822120440_AddTransactionScope`**, `InitialCreate`'ten sonra zincirin
  ilk gerçek yükseltme adımı. EF'in ürettiği `defaultValue: 0` kaldırıldı:
  kalıcı bir veritabanı varsayılanı bırakıyordu ve bıraktığı değer tablonun
  kendi `[Scope] IN (1, 2)` kısıtını ihlal ediyordu. Zorunlu kolonlar ham SQL
  ile varsayılansız ekleniyor ve bu, boş tablo ön koşulunu **denetliyor**
- **Yedek şeması v6**; v2–v5 `restore.unsupported_version` ile reddediliyor ve
  yükseltilmiyor. Eksik kapsam alanını doldurmak, kullanıcının işletme ile cebi
  arasındaki ayrımını uydurmak olurdu (ADR 0013)
- **Bilinen ve kabul edilen boşluk:** kapsam sunucuda **türetilmiyor**; istek
  onu açıkça göndermek zorunda. Bu yüzden Flutter istemcisi bu commit'te API'ye
  karşı çalışmıyor — create istekleri `*.invalid_scope` ile 400 alır. Boşluğu
  Grup 3 (türetme zinciri) kapatıyor; Flutter'ın kendi kontrolleri
  (`analyze`, 637 test, `format`) bu commit'te de temiz
- **Yerel SQL test hedefleri yeniden kuruldu.** `BusinessFinanceApiSqlTests`
  önceki koşumlardan kalan satırlar taşıyordu ve boş tablo ön koşulunu
  karşılamıyordu; düşürülüp migration zinciriyle yeniden kuruldu. Kullanılmayan
  `BusinessFinanceSqlTests` silindi

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **753 geçti**, 1 atlandı |
| Flutter analyze / format / test | Temiz, temiz, **637 geçti** |
| Migration | `AddTransactionScope` iki veritabanına uygulandı; `HasPendingModelChanges` yok |
| Şema | 7 tabloda `Scope NOT NULL`, 3 tabloda `DefaultScope` nullable, hiçbirinde DEFAULT kısıtı yok |

## 22 Ağustos 2026 — Aşama 01, Grup 3: kapsam türetme zinciri

Kapsam artık sunucuda çözülüyor; istemcinin göndermesi zorunlu değil.

- **Sıra tek bir yerde yazılı** (`TransactionScopeResolution`): kullanıcının
  açık seçimi → hesabın/kartın etiketi → kategorinin varsayılanı. Altı oluşturma
  yolu (hareket, kart harcaması, bütçe, taksit planı, tekrarlayan plan, borç) ve
  CSV içe aktarma aynı fonksiyonu çağırıyor; farklı cevap vermeleri, aynı
  harcamanın hangi ekrandan girildiğine göre farklı etiketlenmesi demekti
- **Üçü de boşsa istek reddediliyor** ve hiçbir kayıt yazılmıyor:
  `transactions.scope_unresolved` ve özellik başına karşılıkları. Sunucu kapsam
  uydurmuyor — yanlış etiketlenmiş bir kayıt işletme netini sessizce bozar
- **API sözleşmesinde `scope` isteğe bağlı oldu.** Tanınmayan bir metin hâlâ
  `*.invalid_scope` ile reddediliyor; boş olmakla yanlış olmak ayrı şeyler
- **`defaultScope` güncellemede yetkilidir**: boş göndermek etiketi kaldırır,
  "dokunma" demek değildir. Kaynağın tam güncel hâlini gönderen mevcut
  güncelleme sözleşmesiyle tutarlı
- **Bilinen ve kabul edilen boşluk:** hiçbir varsayılan kategori kapsam
  taşımadığı için zincir pratikte yalnız hesabına ya da kartına elle etiket
  koyan kullanıcı için çözülüyor. Bunu Grup 5'in kategori setleri kapatıyor;
  Flutter istemcisi de o noktada API'ye karşı yeniden çalışır hâle gelecek

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **767 geçti**, 1 atlandı |

## 22 Ağustos 2026 — Aşama 01, Grup 5: kategori setleri ve işletme kimliği

Onboarding'in tek sorusu eklendi ve türetme zincirinin son halkası doldu.

- **`UserProfile` tablosu**, kullanıcı başına tek satır, tek alan:
  `HasBusiness`. Cihazda değil sunucuda duruyor — uygulamayı silip yeniden kuran
  ya da ikinci cihazdan giren kullanıcı işletme sahibi olmayı kaybetmemeli.
  Profili olmayan kullanıcı "işletmesi yok" sayılıyor; bu bir varsayım değil,
  sorunun sorulmadığı hâlin doğru cevabı
- **Cevap kayıt akışında yazılıyor** (`POST /api/v1/auth/register` gövdesinde
  `hasBusiness`). Sonraya bırakılamazdı: varsayılan set ilk kategori okumasında
  kuruluyor ve yalnız hiç kategorisi olmayan kullanıcıya bir kez uygulanıyor
- **İki set:** kişisel setin tamamı `Şahsi` (devralınan liste olduğu gibi kaldı);
  işletme seti 22 işletme kalemi (`İşletme`) **ve** patronun gündelik hayatı için
  12 şahsi kalem taşıyor. İkisi birden gerekiyor, çünkü esnafın market alışverişi
  de aynı uygulamaya giriyor
- **Türetme zinciri artık pratikte çözülüyor:** her kategori kapsam taşıdığı için
  istemci kapsam göndermeden kayıt oluşturabiliyor. Bir API testi bunu iki set
  için de kanıtlıyor. Reddi görebilmek için artık kullanıcının kendi açtığı,
  kapsamsız bir kategori kurmak gerekiyor
- **`GET`/`PUT /api/v1/profile`** cevabı okuyup değiştiriyor. Değiştirmek yalnız
  arayüzü etkiliyor; kategoriler olduğu gibi kalıyor — o noktada liste artık
  kullanıcınındır ve sildiği bir kategoriyi geri getirmek silme eylemini
  anlamsız kılardı
- **Üçüncü migration: `AddUserProfile`.** Yeni tablo olduğu için backfill sorusu
  yok

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **800 geçti**, 1 atlandı |
| Flutter analyze / test | No issues found, **637 geçti** |

## 22 Ağustos 2026 — Aşama 01, Grup 6: kapsama duyarlı okuma modelleri

Kapsam artık raporları ve listeleri bölüyor; parayı bölmüyor.

- **Filtre alan okumalar:** aylık rapor (gelir, gider, kategori dağılımı),
  gelişmiş rapor (dönem karşılaştırması, nakit akışı eğilimi, bütçe sapması),
  birleşik feed ve planlanan görünüm. Hepsinde isteğe bağlı `scope` query
  parametresi; tanınmayan değer `*.invalid_scope` ile reddediliyor
- **Filtre almayan okumalar ve gerekçesi:** hesap bakiyeleri, net varlık, kart
  dağılımı ve yaklaşan ödemeler. Kasadaki para ve karta olan borç tek havuzdur;
  anahtarın konumuna göre değişseydi "ne kadar param var" sorusunun aynı anda
  iki farklı doğru cevabı olurdu
- **Filtre her yerde SQL'e iniyor.** Feed'de tek `UNION ALL` sorgusunun içinde,
  planlanan görünümde her kaynağın kendi sorgusunda; bellekte eleme yok
- **Kapsam filtresi kapsamsız satırları da eliyor.** Transfer, kart ödemesi ve
  kart ekstresi kapsam taşımaz (ADR 0002, ADR 0003); ikisini birden iki tarafta
  göstermek, kullanıcı tarafları karşılaştırdığında aynı para hareketini iki kez
  saydırırdı. Bu yüzden iki tarafın toplamı filtresiz toplamdan küçük ve olması
  gereken bu
- **Bütçe sapması da kategori + kapsam çiftiyle toplanıyor.** Kural artık üç
  yerde birden yazılı: domain hesabı, bütçe listesi ve gelişmiş rapor
- **Bölünmezlik testle korunuyor.** Gerçek SQL üzerinde çalışan yeni test aynı
  ayı üç kapsamda okuyor: gelir/gider bölünüyor ve iki taraf toplamı veriyor,
  hesap bakiyesi ile net varlık üç okumada da aynı kalıyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **801 geçti**, 1 atlandı |

## 23 Ağustos 2026 — Aşama 01, Grup 7: Flutter kapsam anahtarı ve formlar

Kapsam boyutu artık kullanıcının gördüğü yerde. Backend Grup 6'da hazırdı;
istemci bu turda hem gönderiyor hem gösteriyor.

- **Tek denetim, tek anahtar** (`ScopeController`): anahtarın konumu ve
  onboarding cevabı. Sekme başına ayrı filtre yok (ADR 0013). Anahtar Özet
  ekranında, kaydırılan gövdenin **dışında** duruyor — yükleme, hata ve boş
  durumda da yerinde; bölünen diğer ekranlar (feed, planlanan) onu uygulayıp
  başlıklarında yazıyor, denetimi kopyalamıyor
- **Seçim oturumlar arası hatırlanıyor**, cihaz deposunda (`ScopePreferences`,
  `flutter_secure_storage` — `ReceiptPreferences` ile aynı gerekçe: iki değer
  için ikinci bir depolama paketi eklemek bakımı olan yeni bir bağımlılıktı).
  Onboarding cevabının **kaynağı sunucu**; cihazdaki kopya yalnız profil
  okunamadığında boyutun sessizce kaybolmasını engelliyor. "Hayır" varsaymak,
  işletme sahibinin boyutunu bir ağ hatasına kurban ederdi
- **Çıkışta seçim ve cevap unutuluyor**; aynı cihazdan giren ikinci kullanıcı
  birincisinin anahtar konumunu devralmıyor. Bağlama işi kompozisyon kökünde,
  çünkü kimliği ve kapsamı birlikte tanıması gereken tek yer orası
- **Bölünmeyen bölümler bölünmediklerini yazıyor.** Net varlık ve hesap
  bakiyelerinin altında toplam gösterdiklerini söyleyen bir satır duruyor;
  sessizce aynı kalan bir sayı filtrelenmiş sanılır ve kullanıcı iki tarafı
  toplamaya çalışırdı
- **Formdaki çip zincirin önizlemesi.** Kararın sahibi sunucu
  (`TransactionScopeResolution`); form aynı sırayı yalnız **gösterebilmek** için
  uyguluyor ve gösterdiği değeri açıkça gönderiyor — ekranda okunan ile yazılan
  aynı olmalı. Alanın altında değerin nereden geldiği yazılı. Zincir
  çözülemezse istek sunucuya gitmeden duruyor; sunucu da reddederdi
  (`*.scope_unresolved`), ama hata kullanıcının düzeltebileceği yerde görünmeli
- **`FinancialDataChanges`'e bağlanmadı** ve gerekçesi belgeye yazıldı: o sinyal
  "veri değişti" der, anahtar veriyi değiştirmez. Oraya bağlansaydı her kapsam
  dokunuşu kapsamdan etkilenmeyen ekranları (hesaplar, kartlar) da boşuna
  yükletirdi
- **Grup 5'in Flutter yüzü de bu checkpoint'e girdi:** kayıt formundaki tek soru
  (`hasBusiness` artık istekle gidiyor, varsayılan kapalı) ve `Diğer`
  menüsündeki `İşletmem var` anahtarı (`PUT /api/v1/profile`). İkisi olmadan
  hiçbir Flutter kullanıcısı işletme sahibi olamıyor ve kapsam boyutunu hiç
  göremiyordu
- **Bilinen boşluk:** `ScopePreferences`'ın kendisinin doğrudan testi yok
  (`ReceiptPreferences` ile aynı gerekçe — platform kanalı ister); sözleşmesi
  (`ScopeStore`) sahte uygulamayla testli

| Kontrol | Sonuç |
|---|---|
| Flutter analyze | No issues found |
| Flutter format (`--set-exit-if-changed lib test`) | Temiz |
| Flutter test | **692 geçti** (637 → +55) |
| Flutter debug APK | Derlendi (`app-debug.apk`) |
| Backend | Bu turda değişmedi |

## 23 Ağustos 2026 — Aşama 01, Grup 8: özet ekranının hero metriği

Kapsam varken tek bir "net" hangi neti sorduğunu söylemiyordu; artık ay iki
tarafıyla birlikte okunuyor.

- **Aylık rapor filtresiz okunduğunda kırılım da döndürüyor**
  (`scopeBreakdown`): işletme ve şahsi tarafın gelir/gider/net tabloları ayrı
  ayrı. İki tarafın toplamı raporun kendi toplamına eşit — gelir/gider üreten
  her kayıt tam olarak bir kapsam taşıyor ve üçüncü bir kova yok
- **Kırılım sunucudan hazır geliyor** çünkü istemci finansal toplamı ikinci kez
  hesaplamaz. "İşletme neti" ile "şahsi çekim" bir çıkarma değil, ayrı ayrı
  toplanmış iki tablo; istemci çıkarsaydı ekrandaki sayı sunucununkiyle
  tutmayabilirdi
- **Sorgu sayısı değişmedi.** Toplamlar `SUM` yerine kapsama göre `GROUP BY`
  ile okunuyor; en fazla iki satır dönüyor ve toplam onların toplamı. Kırılımı
  ikinci bir tur sorguyla almak özet ekranının ilk isteğini iki katına
  çıkarırdı. Bounded query-count ölçüsü olduğu gibi geçiyor
- **Filtreli okuma kırılım taşımıyor:** rapor zaten tek tarafı anlatıyor,
  kırılım göndermek dışlanan tarafı sıfır gösterip "o tarafta hiç hareket yok"
  dedirtirdi
- **Ekranda üç sayı:** `İşletme neti` (hero), `Şahsi çekim` ve `Bu ayın neti`.
  Kapsam boyutu görünmeyen kullanıcıda ekran bugünkü hâlini koruyor; bir taraf
  seçiliyken hero o tarafın netini adıyla gösteriyor
- **Aşama belgesinden bilerek sapıldı:** üçüncü sayı `kasa değişimi` diye
  planlanmıştı, `Bu ayın neti` oldu. Rapor gideri harcandığı gün tanıyor — kart
  harcaması aynı ay gider yazılır, borcu bir sonraki ay ödenir — dolayısıyla ilk
  iki sayının toplamı kasadaki değişim değil. "Kasa değişimi" demek, ekrandaki
  üç sayıyı toplayan kullanıcıya kasada olmayan bir para söylemek olurdu.
  Kasanın gerçek hâli aynı ekranda `Hesap bakiyeleri` ve `Varlık durumu`
  bölümlerinde zaten duruyor
- **Şahsi tarafın adı sayının yönüne göre değişiyor** (`Şahsi çekim` /
  `Şahsi net`): çoğu ayda şahsi taraf yalnız harcamadır, ama şahsi bir gelir
  girilen ayda "çekim" demek artı bir sayıyı eksi gibi okuturdu
- **"Kâr" kelimesi ekranda hiç geçmiyor** ve bunu bir test koruyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **803 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze / format | No issues found, temiz |
| Flutter test | **700 geçti** (692 → +8) |
| Flutter debug APK | Derlendi |

## 23 Ağustos 2026 — Aşama 01, Grup 9: CSV kapsam kolonu ve yedek tatbikatı

Aşamanın son grubu. Kapsam artık kullanıcının dosyalarında da yazılı.

- **İşlem CSV'si `scope` kolonu taşıyor**, `type`'ın hemen yanında: ikisi de
  kaydın ne olduğunu söyleyen boyutlar ve satırı okuyan kişi "expense,
  personal" diye yan yana okuyor. Kapsamsız bir dosya, kullanıcının kendi
  arşivinde aynı hesaptan aynı kategoriye yazılmış iki kaydı bir daha ayıramaz
  hâle getirirdi
- **İçe aktarma kolonu okumuyor** ve okumayacak: o ayrıştırıcı banka ekstresi
  içindir, kapsamı zincirden çözer. Dışa aktarma okumak ve arşivlemek için;
  veri taşımanın tek yolu yedek/geri yükleme
- **Yolda sessiz bir kırılma bulundu ve kapatıldı.** İstemci, kendi dışa
  aktarımını içe aktarma ekranında **tam başlık dizesiyle** tanıyordu. Kapsam
  kolonu o dizeyi değiştirdiği anda koruma sessizce kalkacak, kullanıcı kendi
  dosyasını banka importer'ına verip her hareketi ikinci kez yazdırabilecekti.
  Tanıma artık yalnız bu dosyada bulunan kolonlara bakıyor
  (`transactionDate` + `isCancelled`; hiçbir banka ekstresinde `isCancelled`
  yoktur) ve bir test hem eski hem de gelecekte bir kolon daha eklenmiş
  başlığın tanındığını koruyor
- **Runbook** v6'ya göre zaten yazılmıştı (Grup 2); bu turda tatbikata kapsam
  doğrulama adımı eklendi, hata tablosundaki eskimiş `v1` satırı v2–v5
  reddine güncellendi, CSV ile yedeğin aynı şey olmadığı açıkça yazıldı.
  Geri yüklemede kapsamın korunduğunu gerçek SQL üzerinde çalışan
  `Backup_ValidatesAndRestoresCompleteSyntheticGraphToEmptyOwner` kanıtlıyor
- Eskimiş bir kod yorumu da düzeltildi: doğrulama özeti "v2 hâlâ kabul
  ediliyor" diyordu; yalnız v6 okunuyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **804 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze / format | No issues found, temiz |
| Flutter test | **701 geçti** |
| Flutter debug APK | Derlendi |

## 23 Ağustos 2026 — Aşama 01 kabul turu (Pixel 8 + gerçek API + gerçek SQL)

Aşamanın son çıkış koşulu karşılandı. Kurulum: SQL container healthy, API
güncel derlemeyle `http://localhost:5284` (live/ready 200), Pixel 8 emulator,
debug APK `API_BASE_URL=http://10.0.2.2:5284`.

**Otomatik senaryolar** — `integration_test/stage01_scope_acceptance_test.dart`,
cihazda çalıştı, üçü de geçti. Her kayıt kapsam **göndermeden** oluşturuldu;
kapsamı sunucu türetti:

- **Kasap:** dükkân kasası, 600 satış + 200 mal alımı → işletme neti 400.
  Ardından aynı kasadan 300 market alışverişi → **işletme neti kıpırdamadı**,
  şahsi taraf −300, ayın neti 100. Bakiye üç kapsam okumasında da 1.100.
  Filtreli okuma kırılım taşımadı; feed işletme tarafında iki, şahsi tarafta
  bir satır gösterdi
- **Manav + terzi:** işletme kategorisine yazılan bir gider çipten şahsi
  seçilerek istisna edildi (işletme neti 1.000, şahsi −150). İkinci esnaf
  kaydolduğunda kendi tablosu boş geldi ve birincinin kaydını **hiçbir
  kapsamda** görmedi
- **Ev hâli:** "işletmem yok" diyen kullanıcıda kayıt sessizce şahsi tarafa
  yazıldı; işletme tarafı 0 kaldı

**Elle gezilen ekran** — aynı verinin uygulamadaki hâli:

- Özet başlığının altında `Hepsi · İşletme · Şahsi` anahtarı; hero
  `İşletme neti ₺4.250,00`, altında `Şahsi çekim −₺1.275,50` ve
  `Bu ayın neti ₺2.974,50`. Üç sayı birbirini tutuyor
- Anahtar `İşletme`ye alınınca gider 3.425,50 → 2.150,00 düştü, kategori
  listesi 2 kategoriden 1'e indi, hero `Yalnız işletme tarafı` yazdı
- `Varlık durumu` ve `Hesap bakiyeleri` filtre açıkken **toplam gösterdiklerini
  yazdı** ve değişmedi (₺3.974,50)
- `İşlemler` başlığı `İşlemler · İşletme` oldu ve market satırı listeden düştü
- Gider formunda kategori seçilince kapsam çipi `İşletme` olarak doldu ve
  altında "Kategorinin varsayılanından geldi" yazdı; `Şahsi`ye dokununca
  "Bu kayıt için siz seçtiniz." oldu

**Kabul turunun iki bulgusu:**

- **Hesap, kart ve kategorinin varsayılan kapsamı uygulamadan ayarlanamıyor.**
  Alan API'de var, Flutter istemcisi ne gönderiyor ne gösteriyor. Zincirin orta
  halkası bu yüzden yalnız API'den kurulabiliyor; uygulamada kapsam kategoriden
  çözülüyor ve istisna çiple düzeltiliyor. Aşamanın vaadi bu hâliyle
  karşılanıyor — ama tek hesabına "dükkân kasası" deyip her kaydı oradan
  işletme saymak isteyen esnaf bunu yapamıyor. `docs/backlog.md` 5. madde
- **Pixel 8 AVD'de uygulama Impeller ile ilk kareyi çizmiyor**; süreç yaşıyor,
  Dart VM açılıyor, ekran Flutter logosunda kalıyor.
  `--ez enable-impeller false` ile açılıyor. Uygulama hatası değil, emulator
  grafik yığını; `documentation/local-setup-and-acceptance.md` sorun giderme
  bölümüne yazıldı

Turda 08:35'ten beri çalışan **eski derlemeli** bir API örneği bulundu ve
durduruldu; kabul güncel derlemeye karşı yürütüldü ve API o hâliyle çalışır
bırakıldı.

## 23 Ağustos 2026 — Aşama 01 kapandı, Aşama 02 açıldı

- **Aşama 01 kullanıcı onayıyla kapandı.** Sekiz çıkış koşulunun sekizi
  karşılandı; belge `docs/archive/stages/` altına taşındı ve tamamlanma kaydı
  (commit zinciri, son kontroller, sapmalar, çıkan açık işler) oraya yazıldı.
  `stages/README.md` ve `PROJECT-ROADMAP.md` durumu `Tamamlandı`
- **Aşama 02 açıldı** ve zincir belgelerinde **Aktif** olarak işaretlendi
- **ADR 0014 yazıldı: "ekonomik olay tanır, ödeme taşır."** Aşamanın karar
  kapısı ve Grup 1'i. Kural yeni değil — kart harcaması, kart ödemesi, transfer
  ve borç açılışı bugün zaten böyle davranıyor; ADR bunu ilk kez yazıya geçirdi
  ve cari hesaba nasıl uygulanacağını sabitledi (borçlandırma tanır, tahsilat
  taşır; cari bakiye projection'dır; tahsilat kategori ve kapsam taşımaz).
  Dört alternatif gerekçesiyle reddedildi. Fiş okumanın "faturayı ödemedim"
  yolundaki tutarsızlık kayda geçti ve Aşama 03'e devredildi
- **ADR'nin durumu `Öneri`**: kabul edilmeden Aşama 02'nin koduna
  başlanmıyor (`AGENTS.md`, "Kalite ve aşama geçişi"). Kod tarafında bu turda
  hiçbir değişiklik yapılmadı

## 23 Ağustos 2026 — Aşama 02, Grup 1 ve 2: karar kapısı ve cari domaini

- **ADR 0014 kullanıcı onayıyla kabul edildi.** Bir kayıt ya ekonomik olayı
  tanır ya ödemeyi taşır; ikisini birden yapması ancak olay ile ödemenin aynı
  ana düşmesidir. Kural yeni değil — kart, transfer ve borç modelleri bugün
  zaten böyle davranıyor — ama ilk kez yazıya geçti ve cari hesaba nasıl
  uygulanacağını sabitledi
- **Domain katmanı yazıldı:** `Counterparty`, `CounterpartyCharge`,
  `CounterpartyPayment`, `CounterpartyBalance`. 17 yeni domain testi
- **Grup yalnız domaine dokundu.** Tipler EF modeline girmediği için migration
  üretilmedi ve `HasPendingModelChanges` temiz kaldı; kalıcılık Grup 3'ün işi.
  Aşama 01'de migration'ın domain checkpoint'ine girme sebebi (model/şema
  örtüşme testi) burada oluşmadı
- **Yön için ikinci bir enum açılmadı:** `DebtDirection` yeniden kullanılıyor,
  çünkü sorduğu soru aynı — yükümlülük kimin üzerinde
- **Yön kategorinin türünü belirliyor**: alacak doğuran borçlandırma gelir
  kategorisi, borç doğuran gider kategorisi ister; tutmayan istek reddediliyor
- **Tahsilat ne kategori ne kapsam taşıyor** ve bunu bir test koruyor: kategori
  "ne satıldı" sorusunu cevaplar ve o soru borçlandırmada sorulmuştur; kapsam
  gelir/gider raporunu böler, tahsilat o rapora hiç girmez
- **Aşama belgesinin yazmadığı iki karar verildi ve gerekçesiyle yazıldı:**
  pasif karşı tarafa yeni borçlandırma yazılamaz ama **tahsilat yazılabilir**
  (aksi hâlde açık bakiye kapatılamaz hâle gelirdi); **fazla tahsilat
  kırpılmaz**, taraf eksiye düşer (kırpmak kullanıcının parasını ekranda yok
  ederdi — aynı hatanın kart tarafındaki hâli backlog 1. maddede)

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dâhil) | **821 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Migration | Üretilmedi ve gerekmedi; `HasPendingModelChanges` temiz |
| Flutter | Bu turda değişmedi |

## 24 Ağustos 2026 — Aşama 02, Grup 8: yedek v7 ve cari defterin dışa aktarımı

- **Yedek şeması v7.** Karşı tarafın kendisi (ad, not, aktiflik) ve cari
  defterin iki hareket türü yedeğe girdi. Geri yükleme gerçek SQL üzerinde
  kayıpsız doğrulandı: üç karşı taraf, iki borçlandırma, iki tahsilat
- **Sözleşme karşı tarafı artık adla değil kimlikle gösteriyor.** v6 adı
  taşıyor ve geri yüklerken addan yeniden kuruyordu; karşı taraf kendi
  kaydıyla dosyaya girdiğine göre ad yedeğin içinde tek yerde durmalı
- **v6 yükseltilmiyor, `restore.unsupported_version` ile reddediliyor.** O
  dosyada cari defter hiç yok; karşı tarafı bakiyesiz kurmak kullanıcının
  alacağını sessizce sıfırlamak olurdu. v2–v5 için kapsam boyutunda verilen
  kararın aynısı (ADR 0013)
- **Pasifleştirme geri yüklemede hareketlerden sonra uygulanıyor** (hesap ve
  kategorilerdeki sıranın aynısı). Ters sıra, pasif bir müşterinin geçmişini
  geri yüklenemez yapardı — borçlandırma yalnız aktif karşı tarafa yazılabilir
- **Kullanıcı kararı: işlem CSV'sine karşı taraf kolonu eklenmedi; cari defter
  kendi dosyasını aldı** (`GET /api/v1/exports/counterparty-ledger.csv`).
  İşlem CSV'si `BudgetTransaction` tablosunun dökümüdür ve cari hareket orada
  hiç bulunmaz — kolon her satırda boş kalırdı. İki kayıt türünün alan listesi
  dosyada da bilerek farklı: borçlandırma kategori ve kapsam taşır, hesap
  kolonu boştur; tahsilat hesap taşır, kategori ve kapsam kolonları boştur
  (ADR 0014). Cari CSV'si de işlem CSV'si gibi **geri yüklenemez**
- Cari dosyası Veri araçları > Yedek sekmesine kendi kartıyla girdi ve kendi
  özet cümlesini kuruyor (`n cari hareket satırı`)

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dâhil) | **838 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze | No issues found |
| Flutter format | Temiz |
| Flutter test | **715 geçti** |
| Flutter debug APK | Derlendi |
| Migration | Üretilmedi ve gerekmedi; şema değişmedi (yedek dosya biçimi değişti) |

## 24 Ağustos 2026 — Aşama 02 kabul turu (Pixel 8 + gerçek API + gerçek SQL)

Tur **Aşama 01'in kabul hesabıyla** yürütüldü (`stage01.kabul@example.test`):
dükkânın zaten bir geçmişi var ve aşamanın vaadi o geçmişin üstüne geldi. Her
ölçü fark olarak alındı.

**Otomatik senaryolar** — `integration_test/stage02_counterparty_acceptance_test.dart`,
cihazda çalıştı, ikisi de geçti:

- **Veresiye defteri:** üç veresiye satış (400+350+250) → gelir **1.000 arttı**,
  kasa kıpırdamadı, net varlık 1.000 arttı ve alacak bir kez sayıldı. İki kısmi
  tahsilat (400+200) → kasa 600 oldu, **gelir ikinci kez sayılmadı**, net varlık
  değişmedi (alacak kasaya taşındı). Cari bakiye 400. Feed beş boyutu doldurdu:
  üç `income`/`counterparty`, iki `neutral`; hepsi `canCancel`. 200'lük tahsilat
  iptal edilince kasa 400'e döndü ve açık bakiye 600 olarak yeniden doğdu
- **Yedek v7 ve cari dosyası:** yedek `schemaVersion 7` ve üç yeni koleksiyonu
  dolu taşıdı; her sözleşme karşı tarafını **kimlikle** gösterdi (`counterpartyName`
  alanı dosyada yok). Cari CSV'sinde borçlandırma satırı kategori ve kapsam
  taşıdı, hesap kolonu boştu; tahsilat satırı hesap taşıdı, kategori ve kapsam
  kolonları boştu

**Elle gezilen ekran:**

- `Diğer > Cari hesap` kapısı `Borç ve alacaklar`ın hemen üstünde; liste iki
  karşı tarafı net işaretiyle gösterdi (`₺600,00`, `₺400,00`, ikisi de
  "Sizden alacağı yok, size borçlu")
- Ayrıntı: `Size borcu ₺600,00 / Sizin borcunuz ₺0,00 / Net ₺600,00`, dört
  eylem (veresiye satış, vadeli alım, tahsilat, ödeme), hareket geçmişinde üç
  satış, iptal edilmiş tahsilat `İptal edildi` rozetiyle soluk, geçerli
  tahsilat nötr mavi tonda (ADR 0008)
- `Veri araçları > Yedek`: yeni `Cari hareket CSV dosyası` kartı; önizleme
  `10 cari hareket satırı` dedi ve başlık satırı `counterpartyName` taşıdı.
  Uygulama yedeğinin özeti `Yedek sürümü: 7 — 53 kayıt`

**Kabul turunun bulgusu — düzeltildi:**

- **Karşı taraf ayrıntısı gerçek API'de hiç açılmıyordu.** Ekran sözleşmeleri
  `GET /api/v1/debts` ile okuyor ama **zorunlu** `asOfDate` parametresini
  göndermiyordu; sunucu `request.invalid_format` döndürüyor, ayrıntı ekranı
  hataya düşüyordu. Widget testleri sahte repository kullandığı için bunu
  göremezdi. Repository artık borç ekranıyla aynı parametreyi gönderiyor ve
  bir test tarihin gittiğini sabitliyor. Kalan tutar kalıcı bir kolon değil,
  bir tarihe göre hesaplanan projection'dır; tarihsiz sorulamaz

**Kabul turunun verisi sentetiktir** ve kabul hesabının defterinde kalmıştır:
iki `Kabul Manavı …` karşı tarafı, altı borçlandırma ve dört tahsilat. Aynı
tur yeniden çalıştırılabilir; her tur kendi kasasını ve kendi karşı tarafını
açar.

## Açık kararlar ve riskler

- ~~Eski repoda 16 commit push edilmemiş~~ — kapandı: commit'ler
  `origin/feat/mobile-data-tools-ux`'e gönderildi ve `main`'e merge edildi
  (`1af11f9`). Eski repo artık eksiksiz ve arşiv olarak tam
- Devralınan açık işler `docs/backlog.md` içinde (fazla ödenmiş kart bakiyesi,
  bütçe ekranı, fiş akışının cihaz kabul turu, fiş veri sınırı kararı)
- Repo sahipliği (kişisel hesap mı organizasyon mu) ve lisans kararı
  verilmedi; ürün ticari olarak sunulacaksa ikisi de netleşmeli

## Sıradaki tek küçük görev

- **Aşama 03, Grup 3: tekrarlayan planda bitiş sınırı.** Plan isteğe bağlı
  bitiş tarihi ve/veya tekrar sayısı alacak; önce dolan sınırdan sonra yeni
  occurrence üretmeyecek ve geçmişi silmeden pasifleşecek.

## 24 Ağustos 2026 — Aşama 02 kapandı, Aşama 03 açıldı

- **Aşama 02 kullanıcı onayıyla kapandı.** Dokuz çıkış koşulunun dokuzu
  karşılandı; belge `docs/archive/stages/` altına taşındı ve tamamlanma kaydı
  (commit zinciri, son kontroller, belgede yazmayan kararlar, kabul turunun
  bulgusu) oraya yazıldı. `stages/README.md` ve `PROJECT-ROADMAP.md` durumu
  `Tamamlandı`
- **Aşama 03 — Yükümlülük ve vade açıldı** ve zincir belgelerinde **Aktif**
  olarak işaretlendi. ADR kapısı yok; kod Grup 1'den başlayabilir
- Bu turda kod değişmedi; yalnız karar belgeleri güncellendi

## 24 Ağustos 2026 — Aşama 03, Grup 1: yükümlülük domaini

- **`Obligation` ekonomik olayı tanıyor:** ödenecek yön gider, tahsil edilecek
  yön gelir; kategori ve kapsam zorunlu, karşı taraf isteğe bağlı. Hesap alanı
  taşımadığı için kayıt anında kasa değişmiyor
- **`ObligationSettlement` nakdi taşıyor:** kategori ve kapsam taşımıyor;
  tahsilat hesabı artırıyor, ödeme azaltıyor ve aynı gelir/gideri ikinci kez
  yazmıyor
- **Kapanış idempotent:** ikinci `Settle` çağrısı ilk settlement'ı döndürüyor.
  Veritabanı tekilliği ve eşzamanlı istek kanıtı kalıcılığın ekleneceği Grup
  2'de tamamlanacak
- **Gecikme saklanmıyor:** durum `Open`, `Settled`, `Cancelled`; gecikme
  `IsOverdueOn(asOfDate)` ile vade ve sorgu tarihinden türetiliyor
- Belge tarihi sistemin UTC kayıt tarihinden ileri olamıyor. Vade geçmişte veya
  gelecekte olabiliyor fakat belge tarihinden önce olamıyor
- İptal, yükümlülükle varsa settlement'ı birlikte UTC damgasıyla iptal ediyor;
  fiziksel silme yok
- Grup yalnız Domain'e dokundu; EF modeline tip eklenmedi, migration/API/Flutter
  değişikliği oluşmadı

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend test (gerçek SQL dâhil) | **851 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **235 geçti**; `ObligationTests` 13 yeni vaka |
| Migration | Üretilmedi ve gerekmedi; EF modeli değişmedi |
| Flutter | Bu turda değişmedi |

## 24 Ağustos 2026 — Aşama 04, Grup 2: gün sonu kasa sayımı (Domain)

- `CashCount` bir para hareketi değil **gözlemdir**: hesap bakiyesine dokunmaz,
  gelir/gider yazmaz, tek başına hiçbir rapora girmez
- **Beklenen tutar saklanmıyor.** Fark `DifferenceFrom(expectedBalance)` ile
  okunduğu anda türetiliyor; aynı sayım, bir hareket sonradan iptal edilince
  farklı bir fark veriyor ve testi bunu kanıtlıyor. Saklansaydı kayıt doğduğu
  andan itibaren eskiyen ikinci bir gerçek olurdu
- **Onaysız hiçbir finansal kayıt üretilmiyor** (grubun çıkış ölçütü). Düzeltme
  ikinci ve ayrı bir eylem: `RecordAdjustment` tek `BudgetTransaction` kimliği
  bağlıyor, idempotent ve bir sayımın ikinci düzeltmesi reddediliyor
- Aynı gün + aynı hesabın ikinci sayımı öncekini **iptal ediyor, üzerine
  yazmıyor**; eski gözlem tutarıyla birlikte kalıyor
- Sayılan tutar `Money` değil: sıfır meşru (boş kasa da sayılır), negatif değil.
  Yalnız kullanıcının kendi, aktif ve **nakit** hesabı sayılabiliyor
- Bu grup yalnız Domain'e dokundu; `CashCount` ve Grup 3'ün `PosSettlement`
  tipi için EF modeli, migration ve yazma yolu Grup 4'e alındı — ikisini SQL'den
  okuması gereken ilk grup odur ve kalıcılığı bölmek migration zincirini
  gereksiz uzatırdı. Bu kapsam netleştirmesi aşama belgesine yazıldı

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **887 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **255 geçti** (17'si yeni `CashCountTests`) |

Flutter tarafına dokunulmadı; bu grup yalnız Domain katmanıdır.

## 24 Ağustos 2026 — Aşama 04, Grup 3: POS tahsilatı (Domain)

- `PosSettlement` ADR 0014'ün ayrımını tek kaydın **iki anına** koyuyor:
  tahsilat günü gelir **brüt** tutar kadar tanınıyor ve komisyon ayrı gider
  yazılıyor, hesap bakiyesi kıpırdamıyor; geçiş günü hesap **net** tutar kadar
  artıyor ve hiçbir gelir/gider yeniden yazılmıyor. Grubun çıkış ölçütü tek
  testte duruyor: gelir bir kez, komisyon bir kez
- **Komisyon brüte eklenmiyor ve ondan düşülerek gizlenmiyor.** Net tutarı gelir
  yazmak, kesilen faturayı küçültür ve komisyonu görünmez bir gidere çevirirdi.
  Komisyon `Money` değil: sıfır meşru, her kartta komisyon kesilmez
- **Oran saklanmıyor, paradan çözülüyor** (ADR 0009'un aynı kararı). Oran ve
  tutar ayrı ayrı saklansaydı ikisi ayrı ayrı düzenlenip sessizce çelişirdi
- `PosTransitBalance` yoldaki parayı veriyor: bekleyenlerin **net** toplamı,
  kalıcı kolon değil ve hesap türü değil (ADR 0015). İptal edilmiş ve geçmiş
  tahsilatlar sayılmıyor — aynı parayı iki yerde göstermek olurdu
- Para banka hesabına geçiyor; kasa kart ödemesi alamıyor. `MarkTransferred`
  idempotent ve ikinci bir günle işaretleme reddediliyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **904 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **272 geçti** (17'si yeni `PosSettlementTests`) |

Flutter tarafına dokunulmadı; bu grup da yalnız Domain katmanıdır.

## 24 Ağustos 2026 — Aşama 04, Grup 4 (1/2): kalıcılık, bakiye ve net varlık

- `AddCashCountsAndPosSettlements` iki tabloyu kurdu. İkisi de **boş doğuyor**;
  zorunlu kolonlar backfill istemiyor, kalıcı DEFAULT bırakılmıyor ve mevcut
  hiçbir tabloya kolon eklenmiyor. Yükseltme `BusinessFinance` ve
  `BusinessFinanceApiSqlTests` veritabanlarına uygulandı
- **Türetilen hiçbir şey kolon değil**: net tutar, komisyon oranı, "yolda mı",
  beklenen bakiye ve fark şemada yok. Migration testi bu yokluğu açıkça sınıyor
- `CashCounts` üzerindeki filtreli tekil indeks (`IsCancelled = 0`) bir gün ve
  bir kasa için tek açık sayım bırakıyor; gerçek SQL testinde ikinci açık sayım
  reddediliyor, iptal edip yenisini yazma yolu çalışıyor
- Geçmiş POS tahsilatı hesap bakiyesine **net** giriyor, yoldaki hiç girmiyor.
  Net varlık `moneyInTransit` taşıyor ve **iki sayının farkı tam olarak yoldaki
  tutar** — grubun ölçütü gerçek SQL testinde kapıya bağlandı
- Özet ekranına `Yolda` satırı eklendi (alt başlık `POS tahsilatı`), yolda para
  yokken çizilmiyor. Arayüz ADR 0015'in kelimesini kullanıyor, `bloke` demiyor
- Gelişmiş raporun sabit SQL komut kapısı 61'den **63**'e çıktı: biri geçmiş
  tahsilatı hesap bakiyesine koyan, biri yoldakini toplayan iki gruplanmış
  sorgu. İkisi de tahsilat adediyle büyümüyor
- **Kalan:** POS'un gelir/gider raporunda tanınması (brüt gelir + komisyon
  gideri) ve yazma uçları. Bugün tablo boş olduğu için gözlenebilir bir
  tutarsızlık yok, ama yazma açılmadan önce kapatılacak

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **907 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze | No issues found |
| Flutter format | 227 dosya, değişiklik gerektirmedi |
| Flutter test | **735 geçti** |
| Android debug build | `app-debug.apk` üretildi |

Not: migration'ı üretebilmek için yerelde çalışan `BusinessFinance.Api` süreci
kullanıcı onayıyla durduruldu; yeniden başlatılmadı.

## 24 Ağustos 2026 — Aşama 04, Grup 4 (2/2): POS satışının raporda tanınması

- POS satışı **tahsil edildiği gün** rapora giriyor, geçtiği gün değil: brüt
  tutar gelire, komisyon ayrı gider olarak dönem toplamlarına, kategori
  dağılımına ve nakit akışı eğilimine katılıyor. Geçiş günü rapora hiçbir şey
  eklemiyor — eklerse aynı satış iki kez sayılırdı
- **Komisyon bütçeyi tüketiyor**: kendi kategorisi olan, o gün tanınmış gerçek
  bir gider. Brüt tutar tüketmiyor çünkü o bir gelir ve bütçe gider bütçesi
- Gerçek SQL testi tanımayı kapıya bağladı: iki açık tahsilatın brütü (1500)
  gelire, komisyonları (30) gidere giriyor; geçmiş olanın geçiş günü hiçbir şey
  eklemiyor ve iptal edilen hiç görünmüyor
- Sabit SQL komut kapısı 63'ten **69**'a çıktı: iki dönemin gelir ve komisyon
  toplamları, trend ve bütçe sapması. Altı gruplanmış okuma, hiçbiri tahsilat
  adediyle büyümüyor
- Yazma uçları bu grupta açılmadı; tükettikleri ekranlarla birlikte **Grup 7**'ye
  alındı (Aşama 03'te yükümlülük uçlarında verilen kararın aynısı)

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **907 geçti**, 1 atlandı (`GeminiLiveContractTests`) |

Flutter tarafına dokunulmadı; bu adım yalnız sunucu raporlarıdır.

## 24 Ağustos 2026 — Aşama 04, Grup 7 (1/2): kasa ve POS yazma uçları

- **Sıra kararı**: Grup 7, Grup 5'ten önce yapılıyor. Grup 5'in üçüncü sekmesi
  `Kasa` ekranına işaret ediyor; o ekran ve onu besleyen uç burada doğmadan
  sekme boş bir yere açılır ve grubun kendi ölçütü ilk günden ihlal olurdu
- Yedi uç açıldı: sayımın günü/geçmişi/oluşturulması/farkın onaylanması ve
  tahsilatın listesi/oluşturulması/geçişin işaretlenmesi
- **Sayım bir gözlemdir**: yazıldığında ne bakiye ne rapor kıpırdıyor. Fark
  ancak açık onayla **tek** gelir/gider kaydına dönüşüyor ve o kayıt kapsamını
  sayımdan alıyor — kategoriden yeniden türetilseydi aynı sayım farklı günlerde
  farklı kapsam üretebilirdi
- Beklenen bakiye ve fark **yalnız günün açık sayımında** dönüyor: geçmiş bir
  günün farkını bugünkü bakiyeye karşı hesaplamak, aradaki bütün hareketleri o
  günün farkına yazmak olurdu
- Aynı gün ikinci sayım öncekini iptal ediyor ve ikisi tek `SaveChanges`
  sınırında yazılıyor; SQL'deki filtreli tekil indeks aynı kuralı zaten tutuyor
- POS tarafında tahsilat günü tanıyor, geçiş günü taşıyor: geçişten sonra hesap
  net kadar artıyor, yoldaki toplam sıfırlanıyor ve rapora hiçbir şey eklenmiyor
- Komisyon **ya tutar ya oran** olarak alınıyor; ikisi birden gönderilirse
  istek reddediliyor. Oran tutara çevriliyor, saklanan tek şey tutar
- `moneyInTransit` liste penceresinden bağımsız okunuyor: yolda olan para,
  kullanıcının hangi aya baktığından etkilenmemeli

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **914 geçti**, 1 atlandı (`GeminiLiveContractTests`) |

Grubun ikinci yarısı Flutter ekranlarıdır (`features/cash/`, POS tahsilatı) ve
sıradaki iştir.

## 24 Ağustos 2026 — Aşama 04, Grup 7 (2/2) ve Grup 5

- Kasa/POS uçları açık Dart modelleri, repository ve controller katmanlarıyla
  Flutter'a bağlandı. `Kasa` sayfasında `Gün sonu` ve `POS tahsilatları` alt
  sekmeleri var; sayım farkı istemcide hesaplanmıyor, POS brüt/komisyon/neti
  ayrı okunuyor
- `FinancialDataChanges` iş anlamına göre ayrıldı: sayım yalnız Kasa'yı,
  farkın açık onayı finansal yüzeyleri, POS tanıma ile geçiş farklı hedefleri
  yeniliyor. Feed, yeni kaynakları Grup 8'de öğrenene kadar yükseltilmiyor
- Controller'lar shell profil/kapsam değişimlerinde yeniden çizilen sayfadan
  daha uzun yaşıyor ve tek yerde dispose ediliyor; yeniden çizim ağ isteği ve
  listener sızıntısı üretmiyor
- Ana shell dört hedefi koruyor. Üçüncü hedef işletme profilinde `Kasa`, kişisel
  profilde `Bütçeler`; yerinden inen hedef `Diğer` altında. Profil cevabı
  sonradan değişince ikisi birlikte yer değiştiriyor, özellik kapanmıyor
- `Kredi kartları` arayüz adı ADR 0015 uyarınca `Kredi kartlarım` oldu;
  tahsilat tarafı yalnız `POS tahsilatları` adını kullanıyor
- Yeni Flutter testleri para string hassasiyetini, kasa/POS mutation hedeflerini,
  iki profilde gezinmeyi ve Kasa'nın iki alt ekranını 2.0× metin ölçeğinde
  erişilebilirlik kapısıyla doğruluyor

| Kontrol | Sonuç |
|---|---|
| Flutter analyze | No issues found |
| Flutter format | Temiz |
| Flutter test | **742 geçti** |
| Flutter debug APK | Oluşturuldu (`API_BASE_URL=http://10.0.2.2:5284`) |

Sıradaki görev Aşama 04 Grup 6'dır: `İşlem ekle` menüsünü dört niyet başlığına
taşımak.

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 03 **Grup 1 tamamlandı**; yükümlülüğün tanıyan
  kaydı, nakdi taşıyan tek settlement'ı, türetilen gecikmesi ve idempotent
  kapanışı Domain katmanına eklendi
- Geçen kontroller: backend build + format + **851 test** (gerçek SQL dâhil),
  1 canlı Gemini testi atlandı; Flutter değişmedi
- Sıradaki görev: Aşama 03 Grup 2 — cari borçlandırmaya vade ve yükümlülük
  kalıcılığı

## 24 Ağustos 2026 — Aşama 03, Grup 2: cari vadesi ve kalıcılık

- **Cari borçlandırma isteğe bağlı vade taşıyor.** Eski hareketlerin bilinmeyen
  vadesi `null` bırakıldı; migration uydurma tarih veya kalıcı DEFAULT eklemedi
- **Bakiye vade kırılımı tek owner-scoped SQL projection'ında hesaplanıyor.**
  API liste ve ayrıntıda toplamın yanında vadesi geçmiş ile vadesi
  geçmemiş/vadesiz alacak-borç tutarlarını kararlı para dizeleriyle döndürüyor.
  Satıra bağlı olmayan tahsilat/ödemeler önce gecikmiş tutarı kapatıyor
- **Flutter cari akışı vadeyi uçtan uca taşıyor.** Formda isteğe bağlı tarih
  seçilebiliyor; listede gecikme yalnız renge bırakılmadan ikon+tutar metniyle,
  ayrıntıda ise iki ayrı bakiye satırıyla okunuyor
- Grup 1'in `Obligation` ve `ObligationSettlement` tipleri owner-scoped bileşik
  foreign key'lerle EF modeline ve SQL şemasına alındı. Bir yükümlülüğe tek
  settlement kuralı tekil indeksle korunuyor; iki stale SQL yazarından yalnız
  biri kapanışı kaydedebiliyor
- `AddObligationsAndCounterpartyDueDates` migration'ı önceki migration'da
  durdurulmuş sentetik veritabanından yükseltilerek doğrulandı; eski cari
  satırının vadesi `null` kaldı, yükümlülük ve settlement geri okunabildi

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **857 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **236 geçti** |
| Flutter analyze | No issues found |
| Flutter format | 227 dosya, değişiklik gerektirmedi |
| Flutter test | **722 geçti** |
| Android debug build | `app-debug.apk` üretildi |

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 03 **Grup 2 tamamlandı**; cari vadesi ve vade
  kırılımlı bakiye backend/API/Flutter boyunca eklendi, Grup 1 yükümlülükleri
  kalıcılaştırıldı
- Geçen kontroller: backend build + format + **857 test** (gerçek SQL dâhil),
  1 canlı Gemini testi atlandı; Flutter analyze + format + **722 test** +
  Android debug build geçti
- Sıradaki görev: Aşama 03 Grup 3 — tekrarlayan planda bitiş sınırı

## 24 Ağustos 2026 — Aşama 03, Grup 3: tekrarlayan plan bitiş sınırı

- Tekrarlayan plan artık isteğe bağlı toplam `occurrenceLimit` alıyor. Mevcut
  `endDate` ile birlikte verilebiliyor; önce dolan sınır geçerli oluyor
- `GeneratedOccurrenceCount` gerçekleşen para hareketini değil üretilen
  occurrence satırını sayıyor. Sınıra ulaşan plan silinmeden pasifleşiyor,
  `NextOccurrenceDate` temizleniyor ve geçmiş occurrence'lar korunuyor
- `generate(throughDate)` 12 tekrarlı planda yalnız 12 satır üretiyor; aynı veya
  daha ileri pencereyle retry yeni kayıt açmıyor. Planlanan projection da
  sınırdan sonra hayali tarih göstermiyor
- API create/list cevabı `occurrenceLimit` ile `generatedOccurrenceCount`
  alanlarını taşıyor. Flutter form alanı Grup 6'nın, yedek v8 kapsamı Grup 7'nin
  işi olarak kaldı
- `AddRecurringOccurrenceLimit` migration'ı mevcut plan sayaçlarını occurrence
  tablosundan backfill ediyor. Kolonlar backfill'den, backfill `NOT NULL` ve
  CHECK kısıtlarından önce çalışıyor; kalıcı DEFAULT bırakılmıyor. Yükseltme
  önceki şemadaki iki occurrence ile gerçek SQL üzerinde doğrulandı ve hem
  `BusinessFinance` hem `BusinessFinanceApiSqlTests` sentetik veritabanına
  uygulandı

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **863 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **238 geçti** |
| Flutter analyze | No issues found |
| Flutter format | 220 dosya, değişiklik gerektirmedi |
| Flutter test | **722 geçti** |
| Android debug build | `app-debug.apk` üretildi |

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 03 **Grup 3 tamamlandı**; tekrarlayan plan toplam
  occurrence sınırında kendiliğinden ve geçmişi koruyarak kapanıyor
- Geçen kontroller: backend build + format + **863 test** (gerçek SQL dâhil),
  1 canlı Gemini testi atlandı; Flutter analyze + format + **722 test** +
  Android debug build geçti
- Sıradaki görev: Aşama 03 Grup 4 — planlanan görünüm ve gecikenler

## 24 Ağustos 2026 — Aşama 03, Grup 4: planlanan yükümlülük ve gecikme

- Açık tek seferlik yükümlülükler kanonik planlanan projection'a iki ayrı yönle
  katıldı: `payable-obligation` / `pay-obligation` ve
  `receivable-obligation` / `collect-obligation`. Beklenen settlement nötrdür;
  ekonomik olay düzenleme tarihinde zaten tanınmıştır
- Yükümlülükler kategori ve isteğe bağlı karşı tarafla owner-scoped tek SQL
  komutunda okunuyor. Gecikme, readiness ve attention kalıcı alan değil; güncel
  iptal/settlement durumu ile sorgu tarihinden türetiliyor
- Settlement oluştuğu anda satır hem planlanan görünümden hem onun daraltılmış
  yaklaşan ödemeler görünümünden düşüyor. `IUpcomingPaymentRepository` ikinci
  yükümlülük sorgusu taşımıyor; tahsil edilecek yön ödeme yükü sayılmıyor
- Flutter yeni tür ve eylem kodlarını açık modellerle okuyor; gecikmiş ödenecek
  yükümlülük mevcut Özet uyarı bandını besliyor. Hesap seçen ödeme/tahsilat
  ekranı Grup 6'nın işi olarak kaldı ve satır yanlış bir forma yönlendirilmedi
- Pozitif/negatif owner izolasyonu API ve gerçek SQL testlerinde kanıtlandı;
  kapsam filtresi yükümlülüğün ekonomik olay kapsamına uygulanıyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **865 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **238 geçti** |
| Flutter analyze | No issues found |
| Flutter format | 220 dosya, değişiklik gerektirmedi |
| Flutter test | **724 geçti** |
| Android debug build | `app-debug.apk` üretildi |

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 03 **Grup 4 tamamlandı**; tek seferlik
  yükümlülükler planlanan/yaklaşan görünüm ve Özet gecikme bandına bağlandı
- Geçen kontroller: backend build + format + **865 test** (gerçek SQL dâhil),
  1 canlı Gemini testi atlandı; Flutter analyze + format + **724 test** +
  Android debug build geçti
- Sıradaki görev: Aşama 03 Grup 5 — fiş okumanın “ödemedim” yolunu
  yükümlülüğe bağlamak

## 24 Ağustos 2026 — Aşama 03, Grup 5: ödenmemiş fatura yükümlülüğü

- Fiş okumanın “Henüz ödemedim” dalı planlama formundan ayrıldı ve
  `/transactions/new/obligation` formuna bağlandı. Belge tarihi giderin tanınma,
  son ödeme tarihi borcun vade günü olarak ayrı taşınıyor; hesap ve sıklık
  sorulmuyor
- `POST /api/v1/obligations` current user sahipliğiyle açıldı. Kategori zorunlu,
  karşı taraf isteğe bağlı ve owner-scoped; kapsam sunucudaki aynı çözüm zincirini
  kullanıyor
- Açılan yükümlülük hesap bakiyesini değiştirmeden gerçekleşen hareket feed'i,
  aylık/gelişmiş rapor, bütçe gerçekleşeni ve net varlığa belge tarihinde giriyor.
  Flutter açık DTO/repository/controller katmanlarıyla bu sözleşmeye bağlandı
- Eski `BillPrefill` ve planlama formundaki tek fatura özel yolu kaldırıldı;
  normal tekrarlayan plan formunun davranışı değişmedi
- Gelişmiş raporun sabit SQL komut kapısı yeni yükümlülük toplamları, kategori,
  trend, net varlık ve bütçe dilimleri için 53'ten 60'a güncellendi; komut sayısı
  kayıt adediyle büyümüyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **867 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **238 geçti** |
| Flutter analyze | No issues found |
| Flutter format | 224 dosya, değişiklik gerektirmedi |
| Flutter test | **725 geçti** |
| Android debug build | `app-debug.apk` üretildi |

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 03 **Grup 5 tamamlandı**; ödenmemiş fatura gideri
  belge tarihinde tanıyan ve nakdi ödeme anına bırakan yükümlülük akışına bağlandı
- Geçen kontroller: backend build + format + **867 test** (gerçek SQL dâhil),
  1 canlı Gemini testi atlandı; Flutter analyze + format + **725 test** + Android
  debug build geçti
- Sıradaki görev: Aşama 03 Grup 6 — yükümlülük listesi, ödeme/kapatma akışı,
  cari ayrıntıda vadeli bakiye ve tekrarlayan plan bitiş sınırı alanı

## 24 Ağustos 2026 — Aşama 03, Grup 6: yükümlülük kapanışı ve Flutter

- `GET /api/v1/obligations` yaklaşan/geciken/kapanan kayıtları current user
  kapsamında döndürüyor; gecikme `asOfDate` ile türetiliyor. `POST
  /api/v1/obligations/{id}/settlement` aktif hesabı çözüp tek ve idempotent
  ödeme/tahsilat yazıyor
- Settlement hesap bakiyesi, feed, planlanan görünüm ve net varlığı güncelliyor;
  kategori/kapsam taşımadığı için gelir-gideri ikinci kez tanımıyor. Karşı taraflı
  açık yükümlülük cari bakiyeye bir kez katılıyor ve kapanınca düşüyor
- `Diğer > Yükümlülükler` ekranı yaklaşan, geciken ve kapanan sekmeleriyle;
  loading, empty, error, unauthorized ve stale-cache durumlarıyla eklendi. Açık
  satır hesap seçilen tek panelden kapatılıyor; gecikme ikon + metinle gösteriliyor
- Tekrarlayan plan formu isteğe bağlı toplam tekrar sınırını gönderiyor; liste
  üretilen/toplam sayaç ilerlemesini gösteriyor
- Gelişmiş raporun bounded SQL kapısı settlement hesap etkisinin tek grouped
  sorgusuyla 60'tan 61'e çıktı; kayıt sayısıyla büyümüyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release, tek iş parçacığı) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **867 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze | No issues found |
| Flutter format | 226 dosya biçimlendirildi |
| Flutter test | **728 geçti** |
| Android debug build | `app-debug.apk` üretildi |

## Son oturum kapanışı — Grup 6

- Yapılan değişiklik: Aşama 03 **Grup 6 tamamlandı**; yükümlülük listeleme,
  idempotent ödeme/kapatma, cari vade bakiyesi ve tekrar sınırı Flutter'a bağlandı
- Geçen kontroller: backend build + format + **867 test** (gerçek SQL dâhil),
  1 canlı Gemini testi atlandı; Flutter analyze + format + **728 test** + Android
  debug build geçti
- Sıradaki görev: Aşama 03 Grup 7 — yedek şemasını v8'e yükseltmek ve yükümlülük
  ile tekrar sınırlarını kayıpsız geri yüklemek

## 24 Ağustos 2026 — Aşama 03, Grup 7: yedek şeması v8

- Yedek artık **v8** yazar ve **yalnız v8** okur. Üç yeni bilgi taşınıyor:
  `obligations`, `counterpartyCharges[].dueDate` ve planın `occurrenceLimit` +
  `generatedOccurrenceCount` alanları. Yükümlülüğü olmayan bir v7 dosyası
  `restore.unsupported_version` ile reddediliyor — yükseltmek gideri uydurulmuş
  bir aya ve tarafa yazmak olurdu
- Yükümlülüğü kapatan nakit hareketi ayrı bir koleksiyon değil, yükümlülüğün
  içindeki `settlement` alanı. Bağ bire bir olduğu için dosyada da öyle duruyor;
  ayrı yazılsaydı sahipsiz bir ödeme taşınabilirdi. **Gecikme dosyaya hiç
  yazılmıyor**, geri yüklenen vadeden türüyor
- Occurrence sayacı dosyadan kopyalanmıyor: occurrence geçmişi yeniden oynanarak
  türetiliyor, dosyadaki değer yalnız doğrulama için okunuyor. Elle büyütülmüş
  sayaç, restore'a basılmadan **doğrulama adımında** `restore.invalid_backup`
  ile düşüyor
- Karşı taraf, kategori ve hesap bağları hedef kullanıcının kendi kayıtlarına
  yeniden bağlanıyor; gerçek SQL round-trip'inde iki yükümlülük, bire bir bağlı
  tek settlement, vadeli borçlandırma ve sınırlı plan kayıpsız dönüyor ve
  başarısız restore hâlâ atomik
- Sözleşme belgeleri (`financial-activity-api-contract.md`,
  `receipt-analysis-api-contract.md`) Grup 4–5'te yeni davranışa göre yazılmıştı;
  bu grupta denetlendi, düzeltme gerekmedi. Yedek sürümünün tek kaynağı
  `documentation/restore-runbook.md` olarak korundu

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **870 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **238 geçti** |
| Flutter analyze | No issues found |
| Flutter format | 226 dosya, değişiklik gerektirmedi |
| Flutter test | **728 geçti** |
| Android debug build | `app-debug.apk` üretildi |

## 24 Ağustos 2026 — Aşama 03, kapanış öncesi kod denetimi

Aşama kapatılmadan önce kapsam maddeleri koda karşı denetlendi. Backend tarafı
eksiksiz çıktı; iki arayüz boşluğu bulundu ve kapatıldı.

- **Yükümlülük artık kamerayla sınırlı değil.** Form önerisiz de çalıştığı hâlde
  rota `extra` zorunlu tutuyor, kayıt yalnız fiş okumanın "ödemedim" dalından
  açılabiliyordu. `İşlem ekle > Ödenmemiş fatura` satırı ve `Yükümlülükler`
  ekranındaki ekleme eylemi eklendi; menü satırı paranın bugün hareket
  etmediğini yazıyor
- **Yön elle girişte soruluyor.** İstemci `payable` değerini sabit gönderiyordu;
  API, planlanan görünüm ve yedek iki yönü de taşıdığı hâlde `Tahsil edilecek`
  üretilemiyordu. Yön değişince kategori listesi türüne göre yeniden okunuyor.
  Fiş dalında yön **sorulmuyor**: cevap bir adım önce verildi (ADR 0011)
- **Plan formu bitiş tarihini gönderiyor.** Grup 3 iki sınırı da kurmuştu ama
  form `endDate` alanını sabit `null` bırakıyordu; "31 Aralık'ta bitsin" diyen
  kullanıcı planı elle kapatmak zorundaydı. İki sınır birlikte verilebiliyor ve
  form önce dolanın geçerli olduğunu yazıyor
- Backend'e dokunulmadı; 870 test regresyon kontrolü olarak yeniden koştu

| Kontrol | Sonuç |
|---|---|
| Backend test (gerçek SQL dâhil) | **870 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze | No issues found |
| Flutter format | 227 dosya, değişiklik gerektirmedi |
| Flutter test | **732 geçti** |
| Android debug build | `app-debug.apk` üretildi |

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 04 **Grup 7 ve Grup 5 tamamlandı**; kasa/POS
  Flutter ekranları yazma uçlarına bağlandı ve üçüncü ana hedef profil ön
  ayarına göre `Kasa`/`Bütçeler` olarak yer değiştiriyor
- Geçen kontroller: Flutter analyze + format + **742 test** + Android debug
  build
- Sıradaki görev: Aşama 04 Grup 6 — `İşlem ekle` menüsünü para girdi / para
  çıktı / belge okut / plan kur niyet eksenine taşımak

## 26 Ağustos 2026 — Aşama 04, Grup 6: `İşlem ekle` niyet ekseni

- Menü düz listeden **beş başlığa** geçti: `Para girdi`, `Para çıktı`,
  `Para taşı`, `Belge okut`, `Plan kur`. Aşama belgesindeki dört eksene
  beşincisi eklendi çünkü transfer ile kart borcu ödemesi gider **değildir**
  (ADR 0014); `Para çıktı` altına konsalardı menü raporu yanlış anlatırdı
- POS tahsilatı menüye `Para girdi` altında girdi ve `Kasa` ekranını POS sekmesi
  seçili açıyor (`/more/cash?tab=pos`). İkinci bir kopya form açılmadı; tahsilat
  formu listeyle aynı yerde kalıyor
- **Gün sonu kasa sayımı menüye alınmadı**: para hareketi üretmeyen bir gözlem,
  `Kasa > Gün sonu` ekranında durur. Hesap açma ve CSV içe aktarmanın menüde
  olmamasıyla aynı gerekçe
- Açıklama alt satırı her satırdan kaldırılıp yalnız yanlış anlaşılabilecek
  satırlarda bırakıldı; dokuz açıklama listeyi ekranın dışına taşırıyordu
- Aşamanın ölçütü teste bağlandı: 400×800 telefonda dokuz satırın hepsi
  **kaydırmadan** hit-test edilebiliyor ve aynı testte erişilebilirlik kapısı
  geçiyor; 2× yazı ölçeğinde taşma yok. Tek launcher kuralı korundu

| Kontrol | Sonuç |
|---|---|
| Flutter analyze | No issues found |
| Flutter format | 235 dosya, 1 dosya biçimlendirildi |
| Flutter test | **749 geçti** |
| Android debug build | `app-debug.apk` üretildi |

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 04 **Grup 6 tamamlandı**; `İşlem ekle` menüsü niyet
  eksenine taşındı, POS tahsilatı satırı eklendi, gün sonu sayımı bilinçli
  olarak menü dışında bırakıldı
- Geçen kontroller: Flutter analyze + format + **749 test** + Android debug
  build (backend'e dokunulmadı)
- Sıradaki görev: Aşama 04 Grup 8 — yedek şemasını v9'a yükseltmek, feed
  sözleşmesini kasa sayımı ve POS tahsilatıyla genişletmek

## 26 Ağustos 2026 — Aşama 04, Grup 8: yedek v9 ve feed sözleşmesi

- Yedek **v9** yazıyor ve **yalnız v9** okuyor: `cashCounts` ve
  `posSettlements` eklendi, v8 `restore.unsupported_version` ile reddediliyor.
  Boş dizi yazarak yükseltmek, elle girilmiş bir POS gelirini ikinci kez
  saydırabilirdi — hangi gelirin POS satışı olduğunu yalnız kullanıcı bilir
- Türetilen hiçbir şey dosyada yok: sayımın beklenen tutarı ve farkı,
  tahsilatın net tutarı, komisyon oranı ve "yolda mı" geri yüklenen kayıttan
  çözülüyor. Sayım fark hareketine **yeni** kimliğiyle bağlanıyor; aynı günün
  kapatılmış ikinci sayımı iptal damgasıyla dönüyor. Bu grup şema değiştirmedi
- Feed POS tahsilatını **üç satır** olarak öğrendi: `pos-sale` (brüt gelir,
  tahsilat günü), `pos-commission` (aynı gün gider; komisyon sıfırsa satır yok)
  ve `pos-transfer` (net tutar, geçiş günü, kapsamsız ve `neutral`). Üçü aynı
  kimliği taşıyor, istemci `tür + kimlik` ile anahtarlıyor. Yeni `pos` kaynak
  grubu kredi kartından ayrı (ADR 0015); üçü de feed'den iptal edilemiyor
- **Kasa sayımı feed'e bilerek girmedi**: para hareketi üretmeyen bir gözlem.
  Onaylanan fark zaten normal bir işlem satırı olarak görünüyor
- Yol boyunca gerçek bir hata bulundu ve düzeltildi: `obligation-settlement`
  sunucuda vardı, istemci enum'unda yoktu; yükümlülüğünü ödeyen kullanıcının
  İşlemler sayfası `FormatException` ile açılmıyordu. Sunucunun kablo
  değerlerini sabitleyen yeni bir test bu boşluğu kapattı

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **918 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze | No issues found |
| Flutter format | 235 dosya, değişiklik gerektirmedi |
| Flutter test | **750 geçti** |
| Android debug build | `app-debug.apk` üretildi |

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 04 **Grup 6 ve Grup 8 tamamlandı** — `İşlem ekle`
  menüsü niyet eksenine taşındı; yedek v9'a yükseltildi ve POS tahsilatı
  birleşik feed'e üç satır olarak girdi
- Geçen kontroller: backend build + format + **918 test** (gerçek SQL dâhil),
  1 canlı Gemini testi atlandı; Flutter analyze + format + **750 test** +
  Android debug build geçti
- Sıradaki görev: Aşama 04'ün **cihaz kabul turu** — bir günlük perakende
  senaryosu (gün sonu farkı, POS parası geçene kadar bakiyenin şişmemesi,
  komisyonun gider olarak görünmesi). Kalan tek çıkış koşulu budur; sonrasında
  aşama kapatılıp Aşama 05 kullanıcı onayıyla açılır

## 26 Ağustos 2026 — Aşama 04 cihaz kabul turu ve kapanış

Pixel 8 emulator üzerinde, güncel build'den üretilmiş debug APK ve yerel API
ile bir günlük perakende senaryosu girildi. Emulator Impeller kapalı hâlde de
kare üretmedi; AVD `-gpu swiftshader_indirect` ile yeniden başlatılınca çizdi
ve tur tamamlandı (not `documentation/local-setup-and-acceptance.md` içinde).

- `+` menüsü beş niyet başlığıyla kaydırmasız okundu; `POS tahsilatı` satırı
  Kasa ekranını POS sekmesi seçili açtı. Banka hesabı yokken hesap alanı pasif
  kaldı — POS parası yalnız bankaya geçer
- Brüt ₺1.000 / komisyon ₺25 tahsilat: `Yolda ₺975,00` yazdı, banka hesabı
  ₺5.000,00'de kaldı. Rapor geliri **brüt** kadar artırdı (₺8.400 → ₺9.400) ve
  komisyonu ayrı gider yazdı (₺3.425,50 → ₺3.450,50, `Banka ve POS komisyonu`)
- Net varlık ₺11.949,50 iken likit ₺9.974,50: yoldaki para net varlığa girdi,
  harcanabilire girmedi. `Geçti olarak işaretle` sonrası likit ₺10.949,50 oldu
  ve **net varlık değişmedi** — para yer değiştirdi, yeniden tanınmadı
- Gün sonu sayımı ₺3.950 (beklenen ₺3.974,50) `Eksik −24,50 lira gider` verdi
  ve **onaydan önce hiçbir kayıt üretmedi**; onaylanınca tek gider yazıldı
  (₺3.475,00) ve kasa sayılan tutara oturdu
- Feed POS'u üç satır olarak gösterdi; sayımın kendisi feed'de yok, farkı
  yazan kayıt normal bir işlem satırı olarak duruyor
- Sekmeler iki profilde de dolu: işletmede `Kasa` ana sekme ve `Diğer >
  Bütçeler`, kişiselde `Bütçeler` ana sekme ve `Diğer > Kasa` açılıyor

**Turda iki gerçek kusur bulundu ve aynı gün düzeltildi:**

- **Feed, POS mutasyonlarından sonra yenilenmiyordu.** Geçiş satırı ancak elle
  aşağı çekince göründü. Grup 7 feed hedefini bilerek yükseltmiyordu; Grup 8
  feed'e POS'u öğretti ama `FinancialDataChanges` hedefi güncellenmemişti.
  Tahsilat ve geçiş artık `feed` hedefini de yükseltiyor — geçiş `budgets`
  yükseltmiyor, yoksa aynı satış iki kez sayılırdı
- **Komisyon seçicisinde etiket kelime ortasından bölünüyordu**
  (`Komisyo / n yok`). Grubun adı kapsam seçicisindeki gibi üste alındı,
  segmentler `Yok · Tutar · Oran` oldu

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **918 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze | No issues found |
| Flutter format | 235 dosya, değişiklik gerektirmedi |
| Flutter test | **750 geçti** |
| Android debug build | `app-debug.apk` üretildi |
| Cihaz kabul turu | Pixel 8 emulator, bir günlük perakende senaryosu geçti |

**Aşama 04 kapandı.** Belge `docs/archive/stages/04-kasa-pos-ve-gezinme.md`
altına taşındı; `stages/README.md` ve `PROJECT-ROADMAP.md` güncellendi.

Yerel veritabanında kabul turunun bıraktığı sentetik kayıtlar duruyor: bir
`Ziraat` banka hesabı, bir POS tahsilatı ve bir gün sonu sayımı + fark kaydı.
Hepsi sentetiktir ve istenirse silinebilir.

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 04 **tamamlandı ve kapatıldı**. Bu oturumda Grup 6
  (`İşlem ekle` niyet ekseni), Grup 8 (yedek v9 + POS'un birleşik feed'e
  girmesi) ve cihaz kabul turu yapıldı; turda bulunan iki kusur düzeltildi
- Geçen kontroller: backend build + format + **918 test** (gerçek SQL dâhil);
  Flutter analyze + format + **750 test** + Android debug build; Pixel 8
  emulator üzerinde bir günlük perakende senaryosu
- Sıradaki görev: **aktif aşama yok.** Aşama 05 (Vergi ve muhasebeci) yalnız
  kullanıcının açık onayıyla açılır ve kendi ADR'si yazılmadan koduna
  başlanmaz. Onaya kadar kod değişmez

## 26 Ağustos 2026 — Aşama 05 açıldı, ADR 0016 yazıldı

Kullanıcı Aşama 05'i (Vergi ve muhasebeci) açıkça onayladı; belge **Aktif**
oldu. Aşamanın karar kapısı olan ADR yazıldı:
`documentation/adr/0016-tax-fields-carry-they-do-not-calculate.md`.

ADR'nin karara bağladıkları:

1. Oran ve tarih koda gömülmez, kullanıcınındır. Uygulama başlangıç önerisi
   sunar; öneri kurulduğu an kullanıcının verisi olur ve kendiliğinden
   güncellenmez. Mevzuat takibi yapılmadığı ekranda yazılıdır
2. Uygulama hiçbir vergi tutarını hesaplamaz veya türetmez. KDV oranı ve tutarı
   ayrı ayrı taşınan nullable alanlardır; ikisi uyuşmuyorsa uygulama söyler ama
   düzeltmez. KDV kayıt tutarını, bakiyeyi, bütçeyi ve işletme netini etkilemez
3. İndirilebilirlik kapsamdan ayrı, yalnız işletme kapsamında anlamlı, iki
   durumlu bir alandır; işletme netini değiştirmez, yalnız muhasebeci paketini
   etkiler. Kısmi oran modellenmez
4. Takvim bir hatırlatmadır (mevcut tekrarlayan yükümlülük altyapısı üzerine
   kurulur) ve muhasebeci paketi ikinci bir hesaplama yolu değil, aynı ayın
   işletme raporunun okumasıdır; şahsi kayıt pakete girmez

Kod değişmedi. **Grup 1'in ölçütü ADR'nin kabul edilmesidir**; kullanıcı kabul
edene kadar Grup 2 ve sonrasının koduna başlanmaz.

## 26 Ağustos 2026 — Aşama 05 Grup 2: KDV taşıyan alanlar

- `VatDetails` değer nesnesi eklendi: `Rate` ve `Amount` iki bağımsız nullable
  alandır, ikisi de belgeden okunur ve **biri diğerinden türetilmez**. Nesne
  boş olamaz; KDV yoksa alan `null`'dır
- Gelir/gider **tanıyan** beş kayıt KDV taşıyor: `BudgetTransaction`,
  `CreditCardCharge`, `CounterpartyCharge`, `Obligation`, `PosSettlement`.
  Parayı yalnız taşıyan kayıtlar (transfer, kart ödemesi, cari tahsilat,
  yükümlülük kapanışı) taşımıyor
- Oranla tutar uyuşmasa bile kayıt olduğu gibi duruyor; `ImpliedAmount` yalnız
  uyarı içindir ve hiçbir alanı doldurmuyor. KDV kayıt tutarını, bakiyeyi,
  bütçeyi ve işletme netini etkilemiyor — rapor brüt kalıyor
- `AddVatFields` migration'ı beş tabloya iki nullable kolon
  (`VatRate decimal(5,4)`, `VatAmount decimal(19,4)`) ve iki CHECK kısıtı ekledi;
  yerel geliştirme ve test veritabanlarına uygulandı
- Sözleşme: isteklerde isteğe bağlı `vatRate`/`vatAmount`, cevaplarda tek `vat`
  nesnesi (KDV yoksa `null`). Okunamayan değer `*.invalid_vat` ile reddediliyor
- Geçen kontroller: backend build (0 uyarı) + format temiz + **942 test geçti**,
  1 atlandı (canlı Gemini)

## 26 Ağustos 2026 — Aşama 05 Grup 3: indirilebilirlik

- `IsTaxDeductible` gider **tanıyan** dört kayda eklendi: `BudgetTransaction`
  (yalnız gider), `CreditCardCharge`, `CounterpartyCharge` ve `Obligation`
  (yalnız borç yönünde). `PosSettlement` taşımıyor — o bir satış; komisyonu
  zaten ayrı bir gider olarak yazılıyor
- Alan kapsamdan ayrı ve iki durumlu. Şahsi kayıtta ve gelirde soru sorulmuyor;
  açık cevap gönderilirse istek **reddediliyor**, sessizce saklanmıyor
- `Category.DefaultIsTaxDeductible` ve `TaxDeductibilityResolution` zinciri
  (kullanıcının açık seçimi → kategorinin varsayılanı) eklendi. Soru sorulmayan
  kayıtta kategorinin varsayılanı sessizce düşüyor; kullanıcının kendi cevabı
  düşmüyor
- İşletme kategori seti gider kalemlerini indirilebilirlik önerisiyle açıyor;
  cevabı muhasebecinin takdirinde olan `SGK ve vergi ödemesi` boş kalıyor
- `AddTaxDeductibility` migration'ı beş tabloya nullable kolon ve beş CHECK
  kısıtı ekledi; iki yerel veritabanına da uygulandı
- **İşletme neti etkilenmiyor**: indirilemeyen gider de gider olarak sayılıyor.
  Etkilenecek tek çıktı muhasebeci paketi (Grup 5)
- Yükseltme yolunu sınayan iki test artık kategoriyi ham SQL ile yazıyor: güncel
  model, canlandırdıkları eski şemada olmayan bir kolon taşıyor
- Geçen kontroller: backend build (0 uyarı) + format temiz + **961 test geçti**,
  1 atlandı (canlı Gemini)

## 26 Ağustos 2026 — Aşama 05 Grup 4: vergi ve SGK takvimi

- Takvim kalemi **tekrarlayan bir plandır**: yeni tablo, yeni zamanlayıcı ve
  ikinci bir "yaklaşanlar" kaynağı yok. Kalem planlanan projection'a diğer
  planlarla aynı yoldan düşüyor, aynı yoldan duraklatılıp siliniyor
- `GET /api/v1/tax-calendar/suggestions` dört hazır kalemi öneriyor (KDV beyanı,
  muhtasar, SGK/Bağkur primi, geçici vergi) ve **hiçbir şey yazmıyor**; kurulum
  mevcut `POST /api/v1/recurring-transactions` ucundan yapılıyor
- Öneri **tutar taşımıyor**: bir sayı önermek hesaplanmış vergi tutarı iddia
  etmek olurdu. Önerilen gün başlangıç noktası; kurulduktan sonra tarih de tutar
  da kullanıcınındır
- `RecurrenceFrequency.Quarterly` eklendi (geçici verginin ritmi). Ayrı bir
  hesap yolu değil, aylık adımın üç aylık hâli; ay sonu davranışı aynı.
  `AddQuarterlyRecurrence` migration'ı CHECK kısıtını genişletti
- **Plandaki tutar bir beklenti**: gerçekleştirme isteği isteğe bağlı bir tutar
  taşıyor ve kayda o yazılıyor. Yalnız bekleyen occurrence düzeltilebiliyor;
  planın kendi tutarı değişmiyor. Bu yalnız verginin sorunu değildi — her dönem
  değişen her kalem (elektrik faturası) beklentiyi gerçekleşmiş hareket olarak
  yazıyordu
- Geçen kontroller: backend build (0 uyarı) + format temiz + **970 test geçti**,
  1 atlandı (canlı Gemini)

## 26 Ağustos 2026 — Aşama 05 Grup 5: ay sonu muhasebeci paketi

- `GET /api/v1/accountant-package?year=&month=` önizlemeyi,
  `GET /api/v1/exports/accountant-package.zip` tek dosyayı veriyor. İkincisi
  mevcut dışa aktarma ailesinin içinde ve `PortableFile` kullanıyor
- **Toplamlar aynı ayın işletme raporundan** okunuyor; paket ikinci bir
  hesaplama yolu açmıyor. Satır listesi o toplamın dökümü ve toplamına eşit
  olduğu hem API hem gerçek SQL testiyle tutuluyor
- **Kapsam parametresi yok**: filtre `Business` olarak sabit, çünkü paket
  muhasebeciye gidiyor. Şahsi kayıt kimliğinin cevabın hiçbir yerinde
  geçmediğini ayrı bir test tutuyor
- Satırlar raporun okuduğu aynı yedi kaynağı aynı filtrelerle okuyor: işlem,
  kart harcaması, cari borçlandırma, yükümlülük, POS satışı + komisyonu, borç
  açılışı, ödenen taksitin faiz payı. POS iki satır (ADR 0015)
- Dosyada `summary.csv`, `lines.csv`, `attachments.csv` ve `attachments/`
  altında kayda bağlı belgeler var. Eklerin toplam boyutu 20 MB tavanı taşıyor;
  aşan ek listede kalıyor ama dosyası konmuyor (`isIncluded=false`)
- KDV özeti taşınan alanların toplamı; KDV yazılmamış satırlar ve
  indirilebilirliği cevaplanmamış giderler ayrıca sayılıyor
- Geçen kontroller: backend build (0 uyarı) + format temiz + **977 test geçti**,
  1 atlandı (canlı Gemini)

## 26 Ağustos 2026 — Aşama 05 Grup 6: karşılık olarak hedefler

- `SavingsGoal` **nullable** bir kapsam kazandı (`AddSavingsGoalScope`). İşletme
  kapsamlı hedef bir karşılıktır; boş kapsam meşrudur ve eksik veri değildir
- `GET /api/v1/goals?scope=business` filtreli okumadır ve **kapsamsız hedefleri
  de eler** — filtrenin her yerdeki kuralı. Filtresiz okuma üç kovalı bir
  kırılım döner: işletme, şahsi, etiketsiz. Üçüncü kova aylık raporda yok ama
  burada var, çünkü hedef kapsam taşımak zorunda değil
- Kırılımın toplamlarını sunucu verir; istemci çıkarma yapmaz
- **Yeni modül yazılmadı**: karşılık, mevcut manuel katkı mekanizmasının kapsam
  etiketli hâli. "Vergi karşılığı" bir hedef türü değil, istemcinin ön
  dolduracağı bir addır
- Geçen kontroller: backend build (0 uyarı) + format temiz + **981 test geçti**,
  1 atlandı (canlı Gemini)

## 26 Ağustos 2026 — Aşama 05 Grup 7: Flutter

- İşlem formuna **katlanmış** bir vergi bölümü eklendi: KDV oranı ve tutarı,
  ikisi de isteğe bağlı, boşken tek satır (`KDV girilmedi`). Bölüm yalnız
  kapsam boyutunu gören kullanıcıda çiziliyor
- Oranla tutar uyuşmazlığı için **uyarı yazılmadı**: o uyarı istemcide bir vergi
  tutarı hesaplamak olurdu ve bu üründe finansal değeri istemci hesaplamaz
  (ADR 0016 uyarıyı zorunlu kılmıyor). Tek istemci doğrulaması bir sınır: KDV
  kaydın tutarını aşamaz
- İndirilebilirlik anahtarı yalnız **işletme kapsamlı giderde** görünüyor;
  kapsam şahsiye çevrilince kayboluyor ve alan istekte hiç gitmiyor
- `Diğer` menüsüne iki kapı eklendi (yalnız işletme ön ayarında): **Vergi
  takvimi** ve **Muhasebeci paketi**
- Takvim ekranı mevzuat takibi yapılmadığını yazıyor; kaleme dokunmak
  tekrarlayan plan formunu **önü dolu** açıyor. Ekranın kendi yazma yolu yok —
  olsaydı aynı plan iki ayrı biçimde oluşabilirdi. Başlangıç günü önerilen günün
  bugünden sonraki ilk düşüşü
- Paket ekranı varsayılan olarak **geçen ayı** açıyor; toplamları, kayıt/belge
  sayısını ve cevapsız kalemleri sunucudan gösteriyor, dosyayı cihazdan
  paylaştırıyor. Boyut tavanını aşan belge ekranda yazılıyor
- Planlama formuna `Üç ayda bir` sıklığı eklendi (geçici verginin ritmi)
- Geçen kontroller: flutter analyze temiz, `dart format` temiz, **770 test
  geçti**, Android debug build üretildi

## 26 Ağustos 2026 — Aşama 05 Grup 8: yedek v10 ve belgeler

- Yedek şeması **v10** yazıyor ve **yalnız v10** okuyor. Yeni koleksiyon yok:
  var olan kayıtlara `vatRate`/`vatAmount` (işlem, kart harcaması, cari
  borçlandırma, yükümlülük, POS), `isTaxDeductible` (gider tanıyanlar),
  kategoride `defaultIsTaxDeductible` ve hedefte `scope` eklendi
- Vergi takvimi kaleminin dosyada ayrı bir koleksiyonu **yok**: kalem
  tekrarlayan bir plandır ve `recurringTransactions` içinde durur; `frequency`
  artık `quarterly` de olabiliyor
- Oranla tutarın uyuşmadığı kayıt olduğu gibi geri geliyor — geri yüklerken
  düzeltmek, kullanıcının belgesini yeniden yorumlamak olurdu
- **v9 dosyası reddediliyor** (`restore.unsupported_version`) ve hedefe hiçbir
  şey yazılmıyor: v9 KDV'yi, indirilebilirliği ve hedef kapsamını bilmiyordu;
  üçünü de uydurmak yanlış bir geçmiş yazmak olurdu
- `documentation/variables.md` yeni bir yapılandırma kaynağı **olmadığını**
  kaydetti: oran ve tarih koda değil kullanıcının verisine yazılır, hazır
  kalemler yalnız formun ön dolumudur
- Geçen kontroller: backend build (0 uyarı) + format temiz + **983 test geçti**,
  1 atlandı (canlı Gemini)

