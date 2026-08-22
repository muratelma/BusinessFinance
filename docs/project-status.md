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
- Aktif aşama: **01 — Kapsam boyutu ve işletme kimliği.** 22 Ağustos 2026'da
  kullanıcı onayıyla açıldı. Belgesi `stages/01-kapsam-boyutu-ve-isletme-kimligi.md`;
  dokuz çalışma grubu; **Grup 1–6 tamamlandı**, kalan üçü Flutter ve yedek
  tarafında
- Kalan beş aşamanın belgesi de yazılı, durumları `Planlandı`
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

## Açık kararlar ve riskler

- **Yerel veritabanının silinmesi onay bekliyor.** Aşama 01 Grup 1'in ilk işi;
  geri alınamaz. Mevcut 28 tablodaki her kayıt kişisel bütçe uygulamasından
  kopyalanmış sentetik veri
- ~~Eski repoda 16 commit push edilmemiş~~ — kapandı: commit'ler
  `origin/feat/mobile-data-tools-ux`'e gönderildi ve `main`'e merge edildi
  (`1af11f9`). Eski repo artık eksiksiz ve arşiv olarak tam
- Devralınan açık işler `docs/backlog.md` içinde (fazla ödenmiş kart bakiyesi,
  bütçe ekranı, fiş akışının cihaz kabul turu, fiş veri sınırı kararı)
- Repo sahipliği (kişisel hesap mı organizasyon mu) ve lisans kararı
  verilmedi; ürün ticari olarak sunulacaksa ikisi de netleşmeli

## Sıradaki tek küçük görev

- **Aşama 01, Grup 7:** Flutter tarafı — Özet ekranının başındaki
  `Hepsi · İşletme · Şahsi` anahtarı, formda düzeltilebilir kapsam çipi ve
  "işletmem yok" kullanıcısında boyutun tamamen gizlenmesi. Backend hazır;
  istemci henüz kapsamı ne gönderiyor ne gösteriyor.

## Son oturum kapanışı

- Yapılan değişiklik: Aşama 01 açıldı ve **Grup 1–6 uygulandı**: veri
  sıfırlama, kapsam boyutu, migration, türetme zinciri, kategori setleri ile
  işletme kimliği, kapsama duyarlı okuma modelleri. Kapsam boyutunun backend
  tarafı tamam
- Geçen kontroller: backend build + format + 801 test; Flutter analyze +
  format + 637 test (Flutter kodu değişmedi)
- Sıradaki görev: Aşama 01 Grup 7 — Flutter kapsam anahtarı ve kapsam çipi
