# Test Kapsam Haritası

Bu belge uygulanmış davranışların hangi test katmanında kanıtlandığını gösterir.
Planlanan testler tamamlanmış gibi listelenmez.

## Son doğrulanmış koşum — 21 Ağustos 2026

Repo `BusinessFinance` olarak kurulduktan, isimler değiştikten ve migration'lar
tek `InitialCreate`'e çökertildikten sonra çalıştırıldı:

| Takım | Sonuç |
|---|---|
| `BusinessFinance.Domain.Tests` | 187 geçti |
| `BusinessFinance.Application.Tests` | 263 geçti |
| `BusinessFinance.Api.Tests` | 144 geçti (SQL testleri dahil) |
| `BusinessFinance.Infrastructure.Tests` | 139 geçti, 1 atlandı |
| **Backend toplam** | **733 geçti**, 1 atlandı |
| Flutter | **637 geçti**, `analyze` ve `format` temiz |

Atlanan tek test `GeminiLiveContractTests`: `BUSINESS_FINANCE_GEMINI_TEST_KEY`
yokken açık skip olur.

Bu kontroller her push'ta `.github/workflows/ci.yml` ile otomatik koşar.
İki iş de **windows-latest** üzerinde çalışır: `dotnet format --verify-no-changes`
satır sonuna duyarlıdır ve geliştirme ortamı CRLF kullandığı için Linux runner
aynı kaynağı sebepsiz düşürüyordu. SQL ve Gemini testleri CI'da ortam değişkeni
olmadığı için skip olur.

Taşımada iki test değişti:

- `Stage125Migration_BackfillsExistingRecurringRowsAsAccountSource` **silindi**.
  Migration zincirini yürüyerek v2 satırların backfill'ini doğruluyordu; zincir
  taşınmadığı için çalıştırılamıyor. Koruduğu kural (kaynak invariant'ı) Domain,
  Application ve API seviyesinde testli kalmaya devam ediyor.
- `MigrationHistoryTests` **yeniden yazıldı**: 21 migration'lık zincirin adları
  ve sırası yerine tekliği, modelle örtüşmeyi ve yapısal sayımları doğruluyor.

Şema denkliği ayrıca test dışında da kanıtlandı: gerçek veriyi taşıyan
veritabanı ile tek `InitialCreate`'ten kurulan veritabanının 543 satırlık tam
şema dökümü (kolon, indeks, CHECK, FK, PK/UQ) birebir eşleşti.

## Aşama 01 Grup 2 — kapsam boyutu testleri

22 Ağustos 2026 itibarıyla eklenen ve değişen testler:

| Test | Ne kanıtlıyor |
|---|---|
| `TransactionScopeTests` (Domain, yeni) | Kapsam zorunlu olan altı modelde tanımsız kapsamın reddi ve değerin korunması |
| `TransactionScopeTests.MoneyMovingModels_DoNotCarryScope` | `Transfer` ve `CreditCardPayment` kapsam alanı **taşımıyor**; eklenirse test kırılır |
| `TransactionScopeTests.MonthlyBudget_Progress_CountsOnlyItsOwnScope` | Aynı kategoriye giren şahsi harcama işletme bütçesini tüketmiyor |
| `TransactionScopeTests` varsayılan kapsam testleri | `Account`/`Category`/`CreditCard` varsayılanı boş başlıyor, kurulabiliyor ve kaldırılabiliyor |
| `MigrationHistoryTests` (**yeniden yazıldı**) | Artık tekliği değil zinciri koruyor: beklenen sıra, id'lerin artan olması, `HasPendingModelChanges` yok |
| `MigrationHistoryTests.AddTransactionScope_AddsColumnsBeforeConstraintsAndLeavesNoDefault` | Kolonlar CHECK'lerden önce ekleniyor, zorunlu kolonlar kalıcı veritabanı varsayılanı bırakmıyor, isteğe bağlı olanlar nullable |
| `DataPortabilityTests.BackupBeforeScope_IsRejectedAndWritesNothing` (v2 yükseltme testinin **yerine**) | v5 yedeği `restore.unsupported_version` ile reddediliyor ve hedefe hiçbir şey yazılmıyor |
| `DataPortabilityTests.Backup_ValidatesAndRestoresCompleteSyntheticGraphToEmptyOwner` (genişletildi) | v6 yedeğinde hem kaydın kapsamı hem hesap/kategori varsayılanı geri yüklemede korunuyor |
| `ImportConfirmationUseCaseTests` (genişletildi) | İçe aktarılan satırın kapsamı hesabın varsayılanından çözülüyor |

## Aşama 01 Grup 3 — kapsam türetme zinciri testleri

| Test | Ne kanıtlıyor |
|---|---|
| `TransactionScopeResolutionTests` (Application, yeni) | Zincirin sırası: açık seçim hesabı, hesap kategoriyi yener; üçü de boşsa `null` |
| `...CreateTransaction_WithoutScope_TakesTheAccountLabel` | Kapsam gönderilmeden oluşturulan hareket hesabın etiketini alıyor |
| `...CreateTransaction_WithNothingToGoOn_IsRejectedAndWritesNothing` | Çözülemeyen kapsam isteği reddediyor ve repository'ye **hiçbir şey yazmıyor** |
| `...CreateBudget_WithoutScope_TakesTheCategoryDefaultOrIsRejected` | Bütçenin hesabı olmadığı için zincir kategori ile bitiyor |
| `TransactionScopeEndpointTests` (API, yeni) | Aynı davranışın HTTP sözleşmesindeki hâli: türetme, açık seçimin üstünlüğü, `scope_unresolved` ve `invalid_scope` ayrımı, listede kayıt oluşmaması |
| `...UpdateAccount_WithoutDefaultScope_ClearsTheLabel` | Güncellemede `defaultScope` yetkilidir; boş göndermek etiketi kaldırır |

## Aşama 01 Grup 5 — kategori setleri ve onboarding testleri

| Test | Ne kanıtlıyor |
|---|---|
| `DefaultCategorySetTests` (Infrastructure, yeni) | İki setin de her kalemi kapsam taşıyor, aynı `(ad, tip)` ikilisi bir sette iki kez geçmiyor, raporların kanonik adla aradığı faiz kategorileri iki sette de var |
| `...PersonalSet_IsEntirelyPersonal` | Kapsam arayüzde hiç görünmese bile her kayıt sessizce şahsi oluyor |
| `...BusinessSet_CarriesBothSidesOfTheOwnersLife` ve `...CoversTheTradeItems` | İşletme seti hem işletme kalemlerini hem patronun gündelik şahsi kalemlerini taşıyor |
| `...KeepsPaymentMethodOutOfCategoryNames` | "Satış geliri (kart)" gibi bir kalem açılmadığını koruyor |
| `OnboardingEndpointTests` (API, yeni) | Kaydolurken verilen cevap hangi setin kurulduğunu belirliyor; profil okunabiliyor ve değiştirilebiliyor |
| `...ATransactionWithoutAScope_ResolvesFromTheInstalledCategorySet` | Türetme zincirinin son halkası dolu: istemci kapsam göndermeden kayıt oluşturabiliyor |
| `...ChangingTheAnswerLater_MovesTheProfileAndLeavesTheCategoriesAlone` | Cevap değişince kategoriler değişmiyor |
| `TransactionScopeEndpointTests.CreateTransaction_WithNothingToGoOn_...` (güncellendi) | Reddi görebilmek için artık kullanıcının kendi açtığı, kapsamsız bir kategori gerekiyor — varsayılan setin tamamı kapsam taşıyor |

## Aşama 01 Grup 6 — kapsama duyarlı okuma testleri

| Test | Ne kanıtlıyor |
|---|---|
| `ScopeFilter_SplitsIncomeAndExpenseButLeavesBalanceAndNetWorthWhole` (gerçek SQL, yeni) | Aynı ay üç kapsamda okunduğunda gelir/gider bölünüyor ve iki taraf toplamı veriyor; **hesap bakiyesi, kart borcu ve net varlık üç okumada da aynı** |
| `FinancialActivityFeed_AppliesFiltersWithoutLeakingOtherSources` (genişletildi) | Feed kapsam filtresi alıyor, kapsamsız satırlar (transfer, kart ödemesi) filtreli okumada düşüyor ve iki tarafın toplamı + kapsamsızlar = toplam |

## Aşama 01 Grup 7 — Flutter kapsam anahtarı ve formlar

| Test | Neyi kanıtlıyor |
|---|---|
| `scope_controller_test.dart` (yeni) | Seçim cihazda hatırlanıyor ve `Hepsi` boş değer olarak yazılmıyor; sunucudaki cevap cihazdaki kopyayı tazeliyor; **profil okunamazsa kopya geçerli kalıyor** ve boyut kaybolmuyor; işletmesi olmayan kullanıcıda depoda seçim dursa bile filtre uygulanmıyor; `ensureLoaded` tek okuma yapıyor; çıkışta seçim ve cevap unutuluyor |
| `app_scope_selector_test.dart` (yeni) | Anahtar üç konumu gösteriyor ve dokunulanı bildiriyor; seçili olmak **yalnız renkle değil** onay işaretiyle de taşınıyor; erişilebilirlik kapısı ve 2.0× yazı ölçeği; form alanı iki kapsamı sunuyor, değerin nereden geldiğini yazıyor, hata metni yardımcı metnin yerine geçiyor |
| `dashboard_scope_test.dart` (yeni) | Aktif kapsam aylık **ve** gelişmiş rapora iniyor; `Hepsi` konumunda filtre gönderilmiyor; işletmesi olmayan kullanıcıda hiç gönderilmiyor; konum değişince ekran yeniden okunuyor, aynı konuma dokunmak ikinci istek üretmiyor; anahtar yükleme durumunda da yerinde duruyor; **bölünmeyen bölüm toplam gösterdiğini yazıyor** ve filtre yokken bu not çıkmıyor |
| `activity_scope_test.dart` (yeni) | Kapsam feed ve planlanan sorgusuna query parametresi olarak iniyor; her sayfada uygulanıyor; konum değişince liste **birinci sayfadan** okunuyor; işletmesi olmayan kullanıcıda gönderilmiyor; yetkisiz cevap kapsamla birlikte de görünür ele alınıyor; bölünen ekranların başlığı aktif kapsamı yazıyor |
| `quick_add_scope_test.dart` (yeni) | Çip kaynağın etiketiyle doluyor, kaynak kapsamsızsa kategoriye düşüyor, kullanıcının seçimi ikisini de yeniyor; **yazılan kapsam ekranda görünenle aynı**; zincir çözülemezse kayıt yazılmıyor ve alanın yanında söyleniyor; kart harcaması da kapsam taşıyor; kapsam boyutu görünmeyen kullanıcıda alan hiç çizilmiyor |
| `profile_repository_test.dart` (yeni) | `GET`/`PUT /api/v1/profile` sözleşmesi; eksik alan sessizce "işletmesi yok" diye okunmuyor |
| `business_answer_test.dart` (yeni) | Onboarding sorusu kayıt formunda varsayılan kapalı ve cevap istekle birlikte gidiyor; `Diğer`den değiştirilince kapsam boyutu ona uyuyor; sunucu reddederse boyut değişmiyor ve sebebi söyleniyor |

## Aşama 01 Grup 8 — özet ekranının hero metriği

| Test | Neyi kanıtlıyor |
|---|---|
| `ScopeFilter_SplitsIncomeAndExpense...` (gerçek SQL, genişletildi) | Filtresiz okumanın kırılımı, iki tarafı ayrı ayrı okumakla **birebir aynı** cevabı veriyor; iki net toplamı raporun netini veriyor; filtreli okuma kırılım taşımıyor |
| `TransactionScopeEndpointTests.MonthlyReport_ReportsBothSidesAndTheirSum` (yeni) | Hero'nun üç sayısı **tek istekte** geliyor; şahsi taraf eksi net olarak dönüyor ve istemcinin çıkarma yapmasına gerek kalmıyor |
| `...MonthlyReport_WithAScopeFilter_CarriesNoBreakdown` (yeni) | Filtreli okumada alan `null`; dışlanan taraf sıfır olarak gösterilmiyor |
| `dashboard_hero_test.dart` (yeni) | Kırılım sözleşmeden okunuyor ve yoksa sıfır uydurulmuyor; işletme kullanıcısında üç sayı birlikte ve **birbirini tutuyor**; şahsi taraf artıdayken "çekim" denmiyor; işletmesi olmayan kullanıcıda ekran bugünkü hâlini koruyor; bir taraf seçiliyken hero hangi tarafı okuduğunu yazıyor; **"kâr" kelimesi ekranda geçmiyor**; üç sayılı hero 2.0× ölçekte taşmıyor ve erişilebilirlik kapısını geçiyor |

## Aşama 01 Grup 9 — CSV kapsam kolonu ve yedek tatbikatı

| Test | Neyi kanıtlıyor |
|---|---|
| `DataPortabilityTests.TransactionsCsv_CarriesTheScopeOfEachRow` (yeni) | Dışa aktarılan her satır kapsamını taşıyor, kolon `type`'ın hemen yanında ve değer kararlı makine metni (`business`/`personal`) |
| `csv_import_settings_test.dart` (genişletildi) | Kendi dışa aktarımını tanıma kolon **eklenince de** çalışıyor: eşleşme tam başlık dizesine değil, yalnız bu dosyada bulunan kolonlara bakıyor. Koruma kalkarsa kullanıcı kendi dosyasını içe aktarıp her hareketi ikinci kez yazdırır |
| `DataPortabilityTests.Backup_ValidatesAndRestoresCompleteSyntheticGraphToEmptyOwner` (Grup 2'de genişletilmişti) | v6 yedeği boş kullanıcıya geri yüklendiğinde hem kaydın kapsamı hem hesap/kategori varsayılanı korunuyor — Grup 9'un çıkış ölçütü budur |

Kapsam boyutunun test yüzeyi bu grupla tamamlandı.

## Aşama 02 Grup 2 — cari hesabın domain kuralları

| Test | Neyi kanıtlıyor |
|---|---|
| `CounterpartyTests` (Domain — 18 test) | Ad/not normalizasyonu ve sınırları; borçlandırmanın yöne göre gelir ya da gider tanıması; **yönle çelişen kategorinin reddi**; sahiplik ve aktiflik kuralları; kapsamın borçlandırmada zorunlu olması; opsiyonel vadenin işlem tarihinden önce olamaması |
| `...Payment_CarriesNeitherCategoryNorScope` | Tahsilatta kategori ve kapsam alanı **yok**; alan sonradan eklenirse test kırılır (ADR 0013, ADR 0014) |
| `...Payment_MovesTheAccountAccordingToDirection` | Tahsilat kasaya para koyar, ödeme kasadan alır; işaret tek yerde |
| `...AnInactiveCounterparty_TakesNoNewChargeButCanStillSettle` | Pasif karşı tarafa yeni borçlandırma yazılamaz, tahsilat yazılabilir |
| `CounterpartyBalances_ComeFromOneQueryAndStayInsideTheOwner` (Infrastructure, gerçek SQL) | 53 karşı taraflı sentetik veride cari bakiye **tek okuma komutuyla** geliyor; iptal edilmiş borçlandırma sayılmıyor, fazla tahsilat kırpılmıyor, kapanmış cari listede kalıyor; yabancının birebir aynı grafiği ne listede ne tekil okumada görünüyor |
| `AddCounterparties_OnlyCreatesEmptyTablesWithOwnerScopedKeys` (Infrastructure) | Migration yalnız üç boş tablo kuruyor (mevcut tabloya kolon/kısıt eklemiyor); hareketler karşı tarafa `(UserId, Id)` ile bağlı; tahsilat tablosunda kategori ve kapsam kolonu yok |
| `LinkDebtsToCounterparties_TurnsEveryExistingNameIntoOneOwnedCounterparty` (Infrastructure, gerçek SQL) | **Dolu veritabanında yükseltme yolu**: şema `AddCounterparties` adımında durdurulup eski şemayla borç satırları yazılıyor, sonra yükseltiliyor. Her ad bir karşı taraf oluyor, var olan kayıt yeniden kurulmuyor, aynı ad başka kullanıcıda ayrı kayıt, hiçbir sözleşme karşı tarafını kaybetmiyor |
| `CreateDebt_FindsTheCounterpartyByNameOrCreatesItOnce` (API) | Aynı adla açılan iki sözleşme aynı `counterpartyId`'ye bağlanıyor; farklı ad yeni kayıt üretiyor; başka kullanıcının aynı adı ayrı karşı taraf |
| `CounterpartyWithBothLedgers_CountsEachAmountOnce` (Infrastructure, gerçek SQL) | Aynı karşı tarafın taksitli sözleşmesi ve açık carisi birbirini toplamıyor: net varlık ikisini de bir kez sayıyor, gider tek yazılıyor, kasa yalnız tahsilatı görüyor |
| `CreditSalesAndPartialCollections_AgreeAcrossBalanceReportAndCash` (API) | **Aşamanın çıkış senaryosu**: üç veresiye satış + iki kısmi tahsilat. Gelir bir kez yazılıyor, kasa yalnız tahsilatla değişiyor, cari bakiye tutuyor; feed beş boyutu dolduruyor; tahsilat iptali parayı geri alıp açık bakiyeyi yeniden doğuruyor |
| `CreditPurchase_RecognisesExpenseAndPaymentOnlyMovesCash` (API) | Vadeli alım gideri o gün yazıyor ve kategori dağılımı toplamla tutuyor; ödeme yalnız kasayı azaltıyor; yönle çelişen kategori reddediliyor |
| `DeactivatedCounterparty_TakesNoNewChargeButStillSettlesAndIsNotDeleted` (API) | Pasif tarafa borçlandırma 409, tahsilat geçerli; hareketi olan kayıt silinemiyor, hiç hareketi olmayan silinebiliyor; kapanmış cari ayrı okunuyor |
| `AnotherUsersLedger_IsNeitherReadableNorWritable` (API) | Yabancının karşı tarafı okunamıyor, hareketi iptal edilemiyor, adına tahsilat yazılamıyor; aynı ad iki kullanıcıda iki ayrı kayıt |
| `FinancialActivityFeed_ClassifiesEveryRealizedKindOnce` (Infrastructure, gerçek SQL — genişletildi) | İki cari hareket türü feed'de: borçlandırma `income`/`counterparty`/kapsamlı, tahsilat `neutral`/kapsamsız/kategorisiz; ikisi de `canCancel` |
| `Execute_WhenTheNameMatchesAKnownCounterparty_SuggestsIt` (Application) | Fişten okunan ad kullanıcının kayıtlı karşı tarafıyla eşleşiyor ve **öneri** olarak taslağa giriyor; ad yerinde kalıyor |
| `Execute_WhenOnlyASimilarNameExists_SuggestsNothing` (Application) | Benzeyen ad eşleşme değil: "Sentetik Manav" ile "Sentetik Market" aynı kişi sayılmıyor |
| `Execute_WhenTheMatchingNameBelongsToSomeoneElse_SuggestsNothing` (Application) | Başka kullanıcının aynı adlı karşı tarafı önerilmiyor |
| `Execute_WhenTheNameIsUnreadable_DoesNotAskForAMatch` (Application) | Ad okunamadıysa arama hiç yapılmıyor |
| `counterparties_page_test` (Flutter, 16 test) | Liste: net işareti ve gecikmiş bakiye metinli rozetle okunuyor, sorgu tarihi API'ye taşınıyor, kapanmış cari ayrı filtreyle geliyor, boş/hata/yetkisiz durumları görünür. Ayrıntı iki tarafı ve vade kırılımını ayrı gösteriyor; borçlandırma formu opsiyonel vadeyi isteğe ekliyor; tahsilat kategori/kapsam taşımıyor; erişilebilirlik kapısı |
| `FinancialActivityFeed_ClassifiesEveryRealizedKindOnce` (`counterpartyId` filtresi) | Bir kişinin bütün geçmişi tek soruyla: cari hareketleri **ve** o kişiyle yapılmış sözleşmenin hareketleri |
| `...ThreeSalesAndTwoPartialCollections_LeaveTheRemainderOpen` | Aşamanın çıkış senaryosunun domain hâli: 1.000 satış, 600 tahsilat, 400 açık; tahsilat geliri ikinci kez artırmıyor |
| `...BothSidesOfTheSamePersonAreKeptApart` | Aynı kişinin alacak ve borç tarafı ayrı yürüyor, `Net` ikisini birleştiriyor |
| `...CancelledMovementsLeaveTheBalanceUntouched` | İptal edilmiş hareket bakiyeye girmiyor |
| `...OverCollectingTurnsTheSideNegativeInsteadOfClampingToZero` | Fazla tahsilat kırpılmıyor; para ekranda yok edilmiyor |

Kalan test yüzeyi (projection sorgusu, API sözleşmesi, feed, yedek v7) kendi
gruplarında gelir; bu grup yalnız domain katmanına dokundu ve EF modeline
girmediği için migration üretmedi. `ScopePreferences`'ın kendisinin doğrudan testi yok;
`ReceiptPreferences` ile aynı gerekçe — `flutter_secure_storage` sarmalayıcısı
platform kanalı ister, sözleşme (`ScopeStore`) sahte uygulamayla testli.

## Aşama 03 Grup 1 — yükümlülük domain kuralları

| Test | Neyi kanıtlıyor |
|---|---|
| `ObligationTests` (Domain, yeni — 13 test vakası) | Tek seferlik yükümlülük yöne göre gelir/gider tanıyor ve hesap taşımıyor; karşı taraf isteğe bağlı, verilirse owner-scoped ve aktif |
| `...IssueDateCannotBeFutureButPastAndFutureDueDatesAreValid` | Belge tarihi gelecekte olamıyor; vade geçmişte veya gelecekte olabiliyor ama belge tarihinden önce olamıyor |
| `...OverdueStateIsDerivedFromTheDateAndOnlyWhileOpen` | `Overdue` kalıcı durum değil; aynı kayıt sorgu tarihine göre gecikiyor ve kapanınca gecikmiş sayılmıyor |
| `...SettlementMovesCashWithoutCategoryOrScope` | Kapanış hesabı yöne göre artırıyor/azaltıyor; ikinci gelir/gider üretmemesi için settlement'ta kategori ve kapsam alanı yok |
| `...SettlingTwiceReturnsTheOriginalCashMovement` | İkinci kapanış çağrısı yeni hareket üretmiyor, ilk settlement kimliğini döndürüyor |
| `...CancellationIsIdempotentAndReversesAnExistingSettlement` | Silme yerine UTC damgalı idempotent iptal var; kapanmış kaydın tanıma ve taşıma tarafı birlikte iptal ediliyor |

Grup 1 yalnız Domain katmanına dokunmuştu. EF modeli, migration ve gerçek SQL
kanıtları aşağıdaki Grup 2 checkpoint'inde eklendi; endpoint ve eşzamanlı
kapanış use case'i yükümlülük davranışı açıldığında tamamlanacak.

## Aşama 03 Grup 2 — cari vadesi ve yükümlülük kalıcılığı

| Test | Neyi kanıtlıyor |
|---|---|
| `Charge_CarriesAnOptionalDueDateThatCannotPrecedeTheCharge` | Cari borçlandırma vadesiz kalabiliyor; vade verilirse işlem tarihinden önce olamıyor |
| `DueDatedCharges_ReportOverdueAndNotOverdueBalancesSeparately` (API) | İki gecikmiş, bir ileri vadeli ve bir vadesiz alacakla kısmi tahsilat sonrası toplam `650`, gecikmiş `150`, diğer `500`; liste ve ayrıntı aynı sonucu veriyor, geçersiz vade reddediliyor |
| `CounterpartyBalances_ComeFromOneQueryAndStayInsideTheOwner` (gerçek SQL, genişletildi) | Vade kırılımı 53 karşı taraf için hâlâ tek SQL okumasında; alacak ve borç yönleri ayrı, yabancı kullanıcı görünmüyor |
| `ObligationMapping_UsesOwnerScopedRelationshipsAndOneSettlement` | Yükümlülüğün kategori/karşı tarafı ile settlement'ın hesap/yükümlülük FK'leri owner-scoped; bir yükümlülüğe tek settlement; para `decimal(19,4)` |
| `ObligationRoundTrip_MaterializesItsSettlementFromTheBackingField` | EF, get-only aggregate'i ve `_settlement` backing field'ını kaydedip geri okuyabiliyor |
| `AddObligationsAndCounterpartyDueDates_PreservesUnknownHistoryAndOwnerScope` | Migration'da eski `DueDate` nullable ve varsayılansız; yeni iki tablo boş doğuyor, tanıma ile ödeme alanları karışmıyor ve bileşik FK'ler `Restrict` |
| `AddObligationsAndCounterpartyDueDates_PreservesLegacyDueAndRoundTripsSettlement` (gerçek SQL) | Şema önceki migration'da durdurulup eski cari satırı yazılıyor; yükseltmede vade `null` kalıyor, yeni yükümlülük ve settlement birlikte okunuyor; iki stale kapanış yazarı tekil indeks nedeniyle yalnız bir settlement bırakıyor |
| `counterparties_page_test` (Flutter, genişletildi) | Gecikme ikon+metinle görünür, sorgu tarihi taşınır, opsiyonel vade formdan API gövdesine gider ve 2.0× metin/a11y kapısı korunur |

## Aşama 02 Grup 8 — yedek v7 ve cari defterin dışa aktarımı

| Test | Neyi kanıtlıyor |
|---|---|
| `DataPortabilityTests.Backup_ValidatesAndRestoresCompleteSyntheticGraphToEmptyOwner` (genişletildi) | v7 yedeği karşı tarafı (ad, not, aktiflik), iki borçlandırmayı ve iki tahsilatı kayıpsız geri yüklüyor; **pasif** karşı tarafın geçmişi de dönüyor (pasifleştirme hareketlerden sonra uygulanıyor), iptal edilmiş tahsilat iptal olarak dönüyor |
| `DataPortabilityTests.BackupBeforeCounterpartyLedger_IsRejectedAndWritesNothing` (yeni) | v6 dosyası `restore.unsupported_version` ile reddediliyor ve hedefe hiçbir şey yazılmıyor: o dosyada cari defter yok, yükseltmek kullanıcının alacağını sıfırlamak olurdu |
| `SqlServerPersistenceIntegrationTests.DataPortability_RoundTripAndFailedRestoreAreAtomic` (genişletildi) | **Gerçek SQL**: geri yüklenen hesapta üç karşı taraf, iki borçlandırma ve iki tahsilat var; başarısız restore hâlâ atomik |
| `DataPortabilityTests.CounterpartyLedgerCsv_CarriesBothRecordKindsWithTheirOwnFields` (yeni) | Cari CSV'sinde borçlandırma kategori/kapsam taşıyor ve hesap kolonu boş; tahsilat hesap taşıyor ve kategori/kapsam kolonları boş; iptal edilmiş hareket dosyada kalıp iptal olduğunu söylüyor |
| `DataPortabilityTests.CounterpartyLedgerCsv_IncludesInactiveCounterpartiesAndIsFormulaSafe` (yeni) | Pasif karşı tarafın geçmişi dosyada; formül başlangıcı ve virgüllü metin hücrede güvenli |
| `DataPortabilityEndpointTests.CounterpartyLedgerCsv_IsOwnerScopedAndCarriesBothRecordKinds` (yeni) | Uç nokta kimlik istiyor (401), sahibinin iki hareket türünü de döndürüyor, **başka kullanıcının defteri boş geliyor** |
| `export_file_details_test` (Flutter, yeni) | Cari dosyası kendi özet cümlesini kuruyor (`n cari hareket satırı`); iki dosya ekranda birbirinden ayırt ediliyor |

## Aşama 02 — fiş öneri rozeti ve kabul turunun bulgusu

| Test | Neyi kanıtlıyor |
|---|---|
| `debts_page_test` — `fişten gelen ad defterdeki kişiyle eşleştiğini söyler` | Sunucu eşleşme bulduysa form bunu ekranda **söylüyor**; sessizce bağlamıyor (ADR 0011) |
| `debts_page_test` — `eşleşme yoksa rozet gösterilmez` | O adla ilk kez iş yapılması olağandır; her fişte uyarı göstermek uyarıyı görünmez yapardı |
| `debts_page_test` — `yanlış eşleşme tek dokunuşla reddedilir` | `Bu kişi değil` adı siliyor ve öneri geri gelmiyor — Grup 6'nın ölçütünün arayüz yarısı |
| `debts_page_test` — `ad değiştirilince rozet düşer` | Ad değişince eşleşme hükümsüz; olmayan bir bağ varmış gibi gösterilmiyor |
| `counterparties_page_test` — `ayrıntı okuması sözleşmeler için tarih taşır` | **Kabul turunun bulgusu**: sözleşme listesi tarihsiz sorulunca sunucu `request.invalid_format` döndürüyordu ve ayrıntı ekranı gerçek API'de hiç açılmıyordu. Sahte repository ile geçen bir ekranın gerçek sözleşmeyi tutması ancak böyle sabitlenir |

## Mevcut kabul kanıtı

| Use case | Kural / deny durumu | Beklenen sonuç | Kanıt | Durum |
|---|---|---|---|---|
| Health | SQL olmadan process yaşamaya devam eder | live 200; ready 503, toparlanınca 200 | `HealthEndpointTests`; kontrollü Compose stop/start | Geçti |
| Auth yaşam döngüsü | Eski refresh ve logout sonrası restore reddedilir | Rotation, protected çağrı, local temizlik | `stage9_auth_acceptance_test.dart` | Pixel 8 geçti |
| Kullanıcı izolasyonu | B, A kaynağını okuyamaz | Boş liste/aggregate; A kendi verisini görür | auth ve finance integration | Pixel 8 geçti |
| Finans matematiği | İptal kayıt toplamından çıkar | 1000+250-125,5=1124,5; iptal sonrası 1250 | `stage9_finance_acceptance_test.dart` | Pixel 8 geçti |
| Filtreler | Filtre owner kapsamından sonra uygulanır | Tarih/tür/hesap/kategori doğru satırı döndürür | Finance integration + transaction widget | Geçti |
| Bütçe | Gider ve iptal server-side hesaplanır | spent/remaining/exceeded doğru | Finance integration + API tests | Geçti |
| Pasif kaynak | Yeni hareket için seçilemez/kullanılamaz | Choice'tan düşer; doğrudan istek 400 | Finance integration | Geçti |
| Ağ/timeout/401 | Secret ve iç hata gösterilmez | Güvenli hata, tek refresh/tek retry | `api_client_test.dart`, auth repository tests | Geçti |
| Validation/double submit | Geçersiz veya eşzamanlı ikinci submit yazmaz | Alan hatası ve controller kilidi | transaction/budget widget ve controller tests | Geçti |
| Temiz APK | Eski local session kalmaz | İlk açılış login ekranı | adb uninstall/install + UI Automator | Pixel 8 geçti |
| Transfer sınıflandırması | Hesaplar arası taşıma gelir/gider değildir | Kaynak -250, hedef +250; rapor ±0 | Domain/Application/API + gerçek SQL | Geçti |
| Kartta çifte gider | Kart ödemesi yeni tüketim değildir | 300 harcama, 100 ödeme; gider 300, borç 200 | API + gerçek SQL | Geçti |
| Ekstre dönemi | Kesim ve ödeme zamanları ayrılır | Önceki devir/dönem/kalan/status doğru | Domain/API + gerçek SQL | Geçti |
| Asgari ödeme | Oran karta aittir, tutarı sunucu hesaplar | %40 kartta 400 borç → 160 asgari; kesim sonrası ödeme asgariyi eritir | Domain/Application/API | Geçti |
| Güncel ekstre | Kesimi geçmiş en son dönem döner | 15 Mayıs → mayıs, 5 Mayıs → nisan; kesim yoksa `null` | API + Flutter | Geçti |
| Ekstreden ödeme | Ekstre kayıt değil, ödeme döneme bağlanmaz | Kısayol ödeme panelini kalan/asgari tutarla açar, alan düzenlenebilir | Flutter | Geçti |
| Gecikmiş yükümlülük | Onaylanmayan hareket sessiz kalmaz | Özette sayı ve en eski vade; gelir ve alacak sayılmaz; tutar toplanmaz | API + Flutter | Geçti |
| Planlanan satırın eylemi | Görülen yükümlülük yerinde onaylanabilir | `realize` tek dokunuş + onay; ödeme kendi ekranına yollar; engelli satırda buton kapalı | Flutter | Geçti |
| Borç faizi | Faiz gider/gelirdir ve adı vardır | Toplam ile kategori dağılımı birbirini tutar; faiz `Faiz ve finansman gideri` kovasında | Gerçek SQL | Geçti |
| Hedef modu görünür | Manuel katkı para taşımaz, ekran söyler | Kartta mod rozeti + açıklama; bakiye modunda hesap adı; katkı formunda uyarı | Flutter | Geçti |
| Faiz feed'de bölünmedir | Faiz ayrı kayıt değil | Borç taksidi tek satır; alt satırda faiz, ayrıntıda anapara/faiz; faizsizde satır yok | Flutter | Geçti |
| Dönem seçici yerleşimi | Çubuk satırı birebir kaplar | Genişlik = satır − 2×medium; en büyük yazı ölçeğinde taşma yok | Flutter | Geçti |
| Geçmiş sınırı | Liste tüm geçmişi çekmez | Pencere dışı satır gelmez; tavan aşılınca kırpılır ve `hasMore` bildirilir; `all` tavanı kaldırmaz | Application + gerçek SQL + Flutter | Geçti |
| Taksit/idempotency | Plan gelecektir; yalnız realize giderdir | 100 = 33,3333+33,3333+33,3334; retry tek kayıt | Domain/API/SQL + Pixel 8 | Geçti |
| Recurring idempotency | Aynı dönem ikinci kez üretilmez | İki generate çağrısı, tek occurrence | Domain/Application/API/SQL + Pixel 8 | Geçti |
| Recurring bitiş sınırı | 12 occurrence üreten plan 13'üncüyü üretmez; retry boş döner | Sayaç 12, plan pasif, sonraki tarih `null` | Domain/Application/API + migration/gerçek SQL | Geçti |
| Ay sonu ve pasif plan | 28/29/30/31 ile active sınırı korunur | Clamp/skip doğru; pasif plan üretmez | Domain/Application | Geçti |
| Upcoming birleşimi | Üç plan kaynağı doğru sınıflanır | Gecikmiş/bugün/yaklaşan; yabancı owner dışarıda | Application/API/gerçek SQL | Geçti |
| Gelişmiş rapor | Transfer/kart ödeme çifte sayılmaz | Net varlık, trend, sapma ve gelecek yük fixture ile eşit | Application/API/gerçek SQL | Geçti |
| Büyük veri | Kart sayısı sorgu patlaması üretmez | 5.000 hareket/20 kart, ≤30 reader ve <5 saniye | Gerçek SQL interceptor testi | Geçti |
| Erişilebilir grafik | Görsel olmadan aynı bilgi okunur | Semantics etiketi + görünür aylık metin listesi | Flutter widget + Pixel 8 | Geçti |
| Fiş analizi salt-okuma sınırı | Model çıktısı doğrudan finansal kayıt değildir | Owner kategorileriyle taslak; yabancı kategori yok; bütün owner finans tablo sayıları değişmez | `AnalyzeReceiptUseCaseTests`, `ReceiptEndpointTests`, gerçek SQL API testi | Geçti |

## Önerilen sonraki testler

- Fiziksel Android cihazda gerçek Wi-Fi/USB ve hardware-backed secure storage.
- Production signing/HTTPS tamamlandığında release APK üzerinde kurulum kapısı.
- İdempotency key eklendiğinde aynı transaction POST'un ağ seviyesinde
  tekrarına negatif integration testi.
- Daha geniş ekran, font scaling ve TalkBack ile manuel erişilebilirlik turu.

## Aşama 03 Grup 3 — tekrarlayan plan bitiş sınırı

| Kanıt | Kapsam |
|---|---|
| `RecurringTransactionTests.AdvanceAfter_OccurrenceLimitCompletesAndDeactivatesSchedule` | Sayaç sınırında plan pasifleşir, sonraki tarih temizlenir ve tamamlanmış plan yeniden açılamaz |
| `RecurringUseCaseTests.Generate_WithTwelveOccurrenceLimit_StopsAtTwelveAndRetryCreatesNothing` | 12 aylık plan 12 occurrence üretir; aynı ve daha ileri pencere retry'ı yeni satır açmaz |
| `RecurringEndpointTests.OccurrenceLimitedPlan_GeneratesTwelveThenCompletesAndRetryIsEmpty` | `occurrenceLimit` create/list sözleşmesinden geçer; cevap üretilen sayacı ve tamamlanmış durumu taşır |
| `MigrationHistoryTests.AddRecurringOccurrenceLimit_BackfillsBeforeChecksAndLeavesNoDefault` | Kolonlar → occurrence sayımı backfill'i → `NOT NULL` → CHECK sırası; kalıcı DEFAULT yok |
| `SqlServerPersistenceIntegrationTests.AddRecurringOccurrenceLimit_BackfillsExistingGeneratedCount` | Önceki şemadaki iki occurrence yeni sayaca gerçek SQL yükseltmesinde `2` olarak taşınır |

## Bilinen test boşlukları

- Fiziksel cihaz kanıtı yoktur; emulator kabulü bunun yerine sunulmaz.
- Backend transaction create henüz idempotency key taşımadığı için iki ayrı HTTP
  POST iki finans kaydı üretebilir. Bugünkü test yalnız istemci submit kilidini
  kanıtlar.
- CI pipeline ve `main` merge gate henüz yoktur. Kontroller yerel çalıştırılır;
  branch birleştirmeden önce aynı kalite komutları tekrar edilmelidir.
- Production release signing ve internet-facing deployment test edilmemiştir.

## Domain unit testleri

| Alan | Senaryo sayısı | Doğrulanan ana davranışlar |
|---|---:|---|
| Money | 7 | Pozitif tutar, TRY, değer eşitliği, toplama, null koruması |
| Account | 12 | Kimlik/sahiplik, ad uzunluğu/tür/currency, açılış bakiyesi ve aktif–pasif yaşam döngüsü |
| Category | 9 | Kimlik/sahiplik, ad uzunluğu/tür ve aktif–pasif yaşam döngüsü |
| BudgetTransaction | 19 | Sahiplik, aktiflik, tür uyumu, tarih/açıklama ve UTC idempotent iptal |
| MonthlyBudget | 20 | Gider kategorisi, dönem sınırı, iptal edilen hareketi dışlama, harcanan/kalan/aşılan tutar |
| RefreshSession | 9 | UTC yaşam döngüsü, hash uzunluğu, expiry, rotation, revoke ve reuse tespiti |
| Transfer | 6 | Farklı/aktif/aynı owner hesap, TRY, tarih ve idempotent iptal |
| CreditCard | 5 | Limit, kesim/ödeme günü 1–28, aktiflik, hesaplanan kullanılabilir limit ve 0–100 asgari ödeme oranı |
| Kart activity | 4 | Harcama/ödeme sahipliği, pasif kart ödeme davranışı ve idempotent iptal |
| Ekstre | 11 | Dönem sınırı, son ödeme tarihi, önceki devir, kalan, ödeme durumu; asgari tutarın orandan türemesi, yukarı yuvarlanması, ekstre borcunu aşmaması ve kalanın altında kalması |
| Taksit planı | 5 | 2–60 sınırı, dört ondalık bölme, son parçaya kalan ve realize yaşam döngüsü |
| Recurring plan ve occurrence | 16 | Sıklık, ay sonu, tarih aralığı, active durum, dönem anahtarı ve planned/realized yaşam döngüsü |
| Toplam | 187 | Debug test çalıştırmasında 187/187 başarılı |

Test projesi: `src/BusinessFinance.Domain.Tests`

## Application mimari testleri

| Alan | Senaryo sayısı | Doğrulanan ana davranışlar |
|---|---:|---|
| Bağımlılık yönü | 1 | Application yalnız Domain project reference'ı taşır ve dış paket içermez |
| Fiş taslağı doğrulama | 41 | Tutar ayırıcı çözümü (`847,50`, `1.234,56`, `1,234.56`, `1.234`, `847,5000`, `₺` ve `TL` ekli), sayı olmayan/negatif/aralık dışı tutarın düşürülmesi; ISO ve `gg.aa.yyyy` tarih, gelecek ve 10 yıldan eski tarihin düşürülmesi; ara toplam+KDV tutarlılığı ve yuvarlama toleransı; yabancı para biriminin `Suspect` yapılması; kategorinin yalnız kullanıcının kendi listesinden çözülmesi; modelin okunamadı dediği alanın hiç parse edilmemesi; hiç okunamayan fişin uydurulmuş değil boş taslak üretmesi |
| Fiş analiz use case'i | 12 | Kimliksiz, boş, sınır üstü ve gerçek uzunluğu uyuşmayan dosya; ortak denetçi reddi; PDF kapısı; çözülemeyen görsel; disabled/unavailable/provider-429/unreadable aktarımı; yalnız aktif owner gider kategorileri; tutarsız toplamın `Suspect` olması |
| Hesap oluşturma komutu | 3 | Current user sahipliği, Domain doğrulamasından sonra persistence, cancellation aktarımı |
| Hesap listeleme sorgusu | 6 | Pagination/filter/sort sözleşmesi, DTO eşleme, boş sonuç, cancellation ve geçersiz girdiler |
| Tek hesap okuma | 4 | Current-user sahipliği, DTO eşleme, NotFound, Unauthorized ve geçersiz ID |
| Hesap pasifleştirme | 4 | Sahiplik kapsamlı bulma/update, Domain davranışı, NotFound ve Unauthorized |
| Kullanıcı izolasyonu integration | 4 | A'nın B kaydını okuyamaması/değiştirememesi, kendi kaydı ve User/Admin girdisi bulunmaması |
| Sonuç ve hata sözleşmesi | 6 | Result invariant'ı, hata kod/türleri, validation, unauthorized ve duplicate conflict |
| Kayıt ve giriş use case'leri | 8 | Başarı, duplicate, parola politikası, genel credential hatası ve cancellation |
| Token yenileme ve çıkış | 8 | Rotation, expiry, revoke/reuse, pasif kullanıcı, bilinmeyen token ve idempotent logout |
| Transfer use case'leri | 5 | Current-user sahipliği, iki hesap doğrulaması, create/list/get/cancel |
| Kart/ekstre/taksit use case'leri | 10 | Limit/borç, activity, projection, idempotent plan, atomik realize; oran gönderilmediğinde oluşturmada varsayılan, güncellemede mevcut oranın korunması |
| Recurring/upcoming/advanced report | 16 | Current-user plan üretimi/onayı, bounded tarih girdileri, üç kaynaklı feed ve rapor orkestrasyonu |
| Toplam | 191 | Release test çalıştırmasında 191/191 başarılı |

Test projesi: `src/BusinessFinance.Application.Tests`

## Infrastructure güvenlik testleri

| Alan | Senaryo sayısı | Doğrulanan ana davranışlar |
|---|---:|---|
| Identity modeli ve sınır | 6 | GUID kimlik, UTC zaman, aktiflik, düz parola alanı bulunmaması ve Domain bağımsızlığı |
| Identity kayıt ve giriş | 8 | Hashing, parola politikası, normalizasyon, duplicate, geçerli/geçersiz ve pasif kullanıcı girişi |
| Aktif kullanıcı ID sorgusu | 2 | Aktif kullanıcının dönmesi ve pasif kullanıcının reddedilmesi |
| JWT ve refresh üretimi | 4 | İmza/issuer/audience/claim'ler, süre, rastgelelik, hash ve signing key alt sınırı |
| EF Core/Identity persistence modeli | 3 | SQL Server provider ve EF store DI, Identity tabloları/unique email index'i, eksik connection string reddi |
| Finans/session mapping'leri | 9 | Finans ve planlama tabloları, recurring/occurrence rowversion, composite sahiplik FK'leri ve owner-scoped index'ler |
| Migration tekliği ve model eşitliği | 1 | Tek `InitialCreate`, `HasPendingModelChanges` yok, 27 tablo / 65 indeks / 81 check / 58 FK / 11 unique sayımı ve finansal kuralı taşıyan adlandırılmış kısıt-indeksler |
| Gerçek SQL persistence | 35 | Önceki senaryolar + recurring/upcoming/advanced report, attachment/import/debt/savings kapsamları, owner izolasyonu ve 5.000 hareket/20 kart performans sınırı |
| Fiş görüntüsü ön işleme | 10 | EXIF rotasyonun piksele işlenmesi, uzun kenar ölçekleme, koşullu kontrast (karanlık kalkıyor, aydınlık dokunulmuyor), PNG→JPEG, bozuk içeriğin istisna değil `Reject` dönmesi, pass-through varyantının byte'ları değiştirmemesi |
| Gemini fiş okuyucu | 21 | Anahtar yokken hiç istek çıkmaması, `status != completed` yorumları, bozuk/JSON olmayan/düzyazı yanıt, `number` tipinde tutarın string taşınması, boş alanın `null` olması, kategori sentinel'ı, adımlara bölünmüş metnin birleşmesi, 429'un retry edilmemesi, 5xx'te tek retry, çift timeout, anahtarın URL'ye sızmaması, sağlayıcı etkileşim kaydının `store=false` ile kapatılması, kategori enum'unun kullanıcı listesiyle sınırlı olması, token maliyetinin raporlanması. **Hepsi sahte HTTP handler ile; gerçek ağ çağrısı yok** |
| Gemini canlı sözleşme | 1 | Çizilerek üretilen sentetik fiş görseliyle gerçek çağrı: `completed` dönüyor, alanlar parse ediliyor, token maliyeti raporlanıyor, kategori ve ödeme ipucu kapalı kümenin dışına çıkmıyor. `BUSINESS_FINANCE_GEMINI_TEST_KEY` yoksa **açık skip** |
| Toplam | 130 | Gerçek SQL bağlantılı Release testinde 129/129 başarılı; yalnız secret isteyen Gemini canlı sözleşme testi açık skip |

Test projesi: `src/BusinessFinance.Infrastructure.Tests`

Manuel sağlayıcı ölçümü test paketinin parçası değildir. 30 sentetik fişlik,
resume ve fiziksel kota kapılı koşum `src/BusinessFinance.ReceiptMeasurement`
projesindedir; yöntem ve tarihli sonuçlar
[`receipt-measurement.md`](receipt-measurement.md) belgesindedir.

## API sözleşme ve güvenlik testleri

| Alan | Senaryo sayısı | Doğrulanan ana davranışlar |
|---|---:|---|
| Pipeline ve DI | 2 | Güvenli 500 ProblemDetails, `AnalyzeReceiptUseCase` dahil use case kayıtları |
| Auth HTTP akışları | 7 | Register/login, duplicate, parola, rotation/reuse, logout, 429 |
| Development OpenAPI | 1 | Auth, finans, planlama ve fiş analizi path-metot/response sözleşmelerinin doğrulanması |
| Current-user claim sınırı | 5 | Geçerli sub GUID ve eksik/bozuk/boş claim reddi |
| Secret yapılandırması | 2 | appsettings içinde JWT signing key ve connection string bulunmaması |
| Hesap HTTP ve izolasyon | 9 | 201/400/401/409, pagination/filter/sort, iki kullanıcı ayrımı |
| Transaction contract/query | 11 | 401/200, sekiz filtre hatası, binding formatı ve boş owner-scoped sayfa |
| Finansal yazma HTTP | 3 | Açılış+hareket-iptal bakiyesi, owner-scoped varsayılan kategoriler, çapraz kullanıcı/pasif/tür uyumu reddi |
| Sorgu/bütçe/rapor HTTP | 3 | Ay sınırı ve deterministik sıra; bütçe kullanım/aşım/güncelleme; iptal ve başka kullanıcı verisini dışlayan rapor |
| Bozuk JSON sözleşmesi | 1 | Boş 400 yerine request.invalid_format ProblemDetails |
| Gerçek SQL API akışı | 1 | Register/login, account, hazır category, expense transaction, monthly budget ve report zinciri |
| Finans HTTP | 13 | Transfer/izolasyon, kart CRUD, charge/payment, ekstre, güncel ekstre ucu (sahiplik dâhil), asgari ödeme alanları, 100 üstü oran reddi ve idempotent taksit realize |
| Planlama HTTP | 4 | Recurring create/generate/realize, upcoming sınıflandırma ve advanced report money-string sözleşmesi |
| Fiş analizi HTTP | 9 | Auth 401; JPEG/PNG, magic byte ve 5 MiB kapıları; owner kategori pozitif/negatif; provider hata statüleri; kullanıcı-partition'lı 429; InMemory ve gerçek SQL'de analiz sonrası owner finans satırlarının değişmemesi; sağlayıcı sahte, ağ yok |
| Toplam | 117 | Gerçek SQL bağlantılı Release API testinde 117/117 başarılı |

Test projesi: `src/BusinessFinance.Api.Tests`

## Flutter unit ve widget testleri

| Alan | Senaryo sayısı | Doğrulanan ana davranışlar |
|---|---:|---|
| Config ve network | 10 | Base URL doğrulaması, bearer, ProblemDetails, timeout, güvenli hata ve tek 401 retry |
| Auth ve route guard | 21 | DTO/service/repository, rotation, secure session, form ve üç durumlu redirect akışı |
| App shell ve ortak durum | 6 | Telefon/geniş ekran, tema, semantics, retry ve büyük metin |
| Hesap ve kategori | 7 | DTO/repository, empty/unauthorized, form validation ve çift submit kilidi |
| İşlem ve bütçe | 9 | Query/body sözleşmesi, filtre/pagination, iptal, aylık state, aşım ve form davranışı |
| Dashboard ve para | 4 | String-money biçimi, input normalizasyonu, backend toplamları ve 2× metin ölçeği |
| Transfer/kart/taksit | 20 | Açık DTO para tipi, request body, 401/double-submit, form validation; kart özetindeki ekstre manşeti, kesim gelmemiş kart, ödenmiş ekstre, toplanabilir hesap özeti, ödeme tutarının önceden doldurulması asgari ödeme oranı formu ve özet kutusunun en yoğun hâlinin (ekstre + altı eylem) en büyük yazı ölçeğinde taşmaması |
| Planlama ve gelişmiş rapor | 4 | DTO/query/PATCH sözleşmesi, unauthorized/stale ayrımı, grafik Semantics ve görünür metin alternatifi |
| Toplam | 498 | `flutter test` çalıştırmasında 498/498 başarılı |

Test klasörü: `mobile/business_finance_mobile/test`

## Flutter gerçek API integration testi

| Alan | Senaryo sayısı | Doğrulanan ana davranışlar |
|---|---:|---|
| Android emulator smoke | 1 | Gerçek SQL/API ile register, login, hesap oluşturma, gider kaydı ve dashboard bakiye zinciri |
| Auth kabul | 1 | Restore, refresh rotation, eski token reddi, logout temizliği ve A/B izolasyonu |
| Finans kabul | 1 | Gelir/gider matematiği, dört filtre, bütçe, iptal, pasifleştirme ve A/B izolasyonu |
| Finans kabul | 1 | Transfer, kart harcama/ödeme, ekstre, plan create ve item realize retry idempotency |
| Planlama kabul | 1 | Recurring create, çift generate/tek occurrence, upcoming, realize ve advanced report |
| Toplam | 5 | Pixel 8 emulatoründe her senaryo bağımsız gerçek API penceresinde 5/5 başarılı |

Test klasörü: `mobile/business_finance_mobile/integration_test`

## Kalite kontrolleri

- Release Domain testleri: 128/128 başarılı
- Release Application testleri: 77/77 başarılı
- Release Infrastructure testleri: gerçek SQL ile 68/68 başarılı
- Release API testleri: gerçek SQL ile 73/73 başarılı
- Release solution build: 0 hata, 0 uyarı
- Release backend toplamı: resmi .NET SDK container'ında gerçek SQL ile 346/346 başarılı,
  skip yok
- `dotnet format --verify-no-changes`: Başarılı
- `git diff --check`: Başarılı
- NuGet vulnerability denetimi: Bütün projelerde bilinen açık bulunmadı
- Secret kalıp taraması: Özel anahtar, account key ve shared signature bulunmadı
- Flutter format: kaynak/test kapsamındaki 83 dosyada değişiklik gerektirmedi
- Flutter analyze: Sorun bulunmadı
- Flutter unit/widget testleri: 71/71 başarılı
- Flutter gerçek API emulator integration testleri: smoke 1/1; auth 1/1;
  finance 1/1; kart/transfer 1/1; planning 1/1 başarılı
- Android debug APK build: Başarılı

## Bilinen ortam engeli

Windows Uygulama Denetimi bazı çalıştırmalarda üretilen assembly'leri
`0x800711C7` ile engelliyor. Güvenlik politikası değiştirilmedi. Aynı
kaynak, repository salt bind mount edilerek resmi `.NET SDK 10.0` Linux
container'ında çalıştırıldı. Devralınan tabanın finalinde Compose SQL bağlantısıyla Domain
128/128, Application 77/77, Infrastructure 68/68 ve API 73/73 olmak üzere
346/346 geçti. Migration, constraint, yarış, backup round-trip ve SQL-backed
finans HTTP akışları doğrulandı. API executable wrapper'ı
entegrasyon kabulünde yine policy tarafından engellendi; aynı derlenmiş API DLL'i
`dotnet` hostuyla loopback üzerinde çalıştırıldı.

## Ertelenen test katmanları

- Fiziksel Android cihazda auth ve işlem ekleme akışı; cihaz bulunmadığı için
  yerel MVP kapısında Pixel 8 emulatorü kullanıcı onayıyla kabul edildi
- Geniş gerçek ekran ve erişilebilirlik servisleriyle manuel kontrol

## Import, attachment ve restore test kapsamı

| Katman | Yeni kanıt |
|---|---|
| Domain | Import/debt/goal invariant'ları, dört ondalık plan ve kaynak dışlama |
| Application | Atomik confirm, row sınırı/concurrency mapping, auth ve restore hataları |
| Infrastructure | CSV parser, migration geçmişi, fingerprint/duplicate, SQL yarışları, schema v2 round-trip, attachment composite FK |
| API | CSV stage/map/confirm, duplicate kararı, export/restore, debt/goal/attachment owner izolasyonu ve body sınırları |
| Flutter | Multipart+binary network, controller stale/mutation, beş sekmeli veri araçları widget'ı |

### Frontend kabul sonrası UX otomasyonu — 14 Ağustos 2026

- Kararlı ProblemDetails kodundan Türkçe auth/authorization mesajı eşleme,
  yanlış parola özelinde bağımsız network testiyle doğrulandı.
- CSV import sonucu içe aktarılan/atlanan/hatalı sayaçları controller testinde;
  ayrı `Cihaza kaydet` eylemi enjekte edilen sahte file saver ile widget testinde
  doğrulandı.
- CSV, finans JSON ve backup metadata/önizleme modeli üç bağımsız testle
  doğrulandı; backup payload metni UI önizlemesine verilmedi.
- Hedef testler 24/24, bütün Flutter paketi 102/102 geçti. `flutter analyze`
  temiz ve Android debug APK build başarılıdır. Native `ACTION_CREATE_DOCUMENT`
  akışının gerçek dosya oluşturması Pixel 8 manuel kabulünde açık kalır.

### CSV eşleme profilleri ve export/import sınırı — 14 Ağustos 2026

- Türkçe profilin `Tarih`/`Tutar`/`Açıklama`, `dd.MM.yyyy` ve virgül ondalık;
  İngilizce profilin `date`/`amount`/`description`, `yyyy-MM-dd` ve nokta ondalık
  ayarlarını API alanlarına çevirmesi pure unit testlerle doğrulandı.
- Boş isteğe bağlı açıklama ve referans kolonlarının request'e eklenmediği test
  edildi; bu sayede olmayan bir header yanlışlıkla zorunlu hâle gelmiyor.
- Uygulamanın sabit transaction export header'ı ayrı testte tanınıyor; normal
  Türkçe banka CSV'si yanlış pozitif üretmiyor. Böylece `type` kolonundaki yön
  bilgisinin signed-amount importer tarafından yanlış yorumlanması engelleniyor.
- Bu eklerle hedef testler 28/28, bütün Flutter paketi 106/106 geçti ve
  `flutter analyze` temizdir. Profil seçimi ile engel dialogunun Pixel 8 manuel
  kabulü checklist'te `[~]` olarak açık kalır.

Devralınan taban için final kalite kapısı 11 Ağustos 2026 tarihinde tamamlandı:

- Domain 128/128, Application 77/77, Infrastructure 68/68 ve API 73/73;
  toplam 346/346, skip yok
- Infrastructure gerçek SQL testleri geçici benzersiz veritabanları, API SQL
  kabul testleri migration uygulanmış yerel `BusinessFinance` veritabanını kullandı
- Release solution build 0 hata/uyarı ve `dotnet format --verify-no-changes`
  başarılı
- Flutter format/analyze temiz, unit/widget testleri 71/71 ve debug APK başarılı
- Fiziksel Android cihazda picker/share manuel kabulü çalıştırılmadı; yayın
  öncesi kapıda açık kalır

## Tasarım sistemi test kapsamı

### Token ve kontrast kapısı

- `test/core/theme/app_finance_colors_test.dart`: beş finans rolünün
  (`income`, `expense`, `neutral`, `planned`, `cancelled`) yüzey üzerindeki
  metin rengi, aydınlık ve karanlık temada üç yüzeye karşı (`surface`,
  `surfaceContainerLow`, `surfaceContainerHighest`) WCAG AA 4.5:1 eşiğini
  geçmelidir. Chip çiftleri (`on*Container` / `*Container`) aynı eşikle,
  chip zemininin yüzeyden ayırt edilebilirliği ayrıca ölçülür.
- Kontrast **hesaplanarak** doğrulanır (`test/helpers/contrast.dart`, WCAG 2.1
  bağıl parlaklık); gözle onay veya golden görüntü kullanılmaz.
- Kapı kurulduğunda kodda duran ham renklerin hiçbiri eşiği geçmiyordu:
  `Colors.green.shade700` karanlık kartta 4,16:1 ve aydınlık kartta 3,72:1,
  `Colors.deepPurple.shade400` karanlık kartta 3,28:1, `Colors.green`
  aydınlık yüzeyde 2,65:1.
- Renklerin birbirinden *ayırt edilebilirliği* sayısal olarak test edilmez:
  kontrast oranı parlaklık farkını ölçer, ton farkını değil ve renk körlüğünde
  yeşil/kırmızı ayrımı hiçbir ölçütle garanti edilemez. Bunun yerine rollerin
  aynı token'a düşmediği doğrulanır; gerçek koruma "hiçbir bilgi yalnız renkle
  taşınmaz" kuralıyla ikon + metinden gelir ve widget katmanında test edilir.
- `AppFinanceColors.of()` uzantı yoksa `FlutterError` atar; sessiz varsayılana
  düşüş testle engellenir.

### Pencere sınıfı ve yerleşim

- `test/core/theme/app_breakpoints_test.dart`: eşik değerlerinin hangi sınıfa
  girdiği ve `context.windowSize`'ın genişlikten doğru sınıfı okuması.
- `test/widget_test.dart`: shell üç pencere sınıfında sırasıyla
  `NavigationBar`, daraltılmış `NavigationRail` ve `extended: true` ray kurar;
  geniş ekranda sayfa içeriği 720 dp sınırını aşmaz.
- `test/core/widgets/app_content_width_test.dart`: sınırın altında şeffaftır,
  üstünde sınırlar ve ortalar.
- `test/core/widgets/app_adaptive_sheet_test.dart`: compact'ta `BottomSheet`,
  medium ve expanded'da `Dialog`; dialog içeriği 720 dp'yi aşmaz ve her iki
  kademede de sonuç değeri çağırana döner.
- `test/core/widgets/app_responsive_grid_test.dart`: sütun sayısı eşikleri ve
  gerçek yerleşim genişlikleri. Eski elle hesabın 700 dp üstünde her zaman iki
  sütunda kalması bu testle regresyona kapatıldı.

### Ölçüm tuzağı (yazılırken iki kez düşüldü)

Sarmalayıcı widget'ların kök kutusu (`Align`, `Dialog`) tanım gereği tam
genişliği kaplar; genişlik sınırını içerideki kutu uygular. Bu testlerde
ölçülen daima **sınırlanan içeriktir**, sarmalayıcı değil. Sarmalayıcı
ölçülürse test geçmez ya da geçse bile yanlış şeyi ölçer.

### Adaptif panellerde kapsam kaybı riski

Flutter'ın varsayılan test ekranı 800 dp'dir, yani `expanded` sınıfına düşer.
Ekran boyutu ayarlamayan bir panel testi, panel adaptif hale geldiği anda
sessizce yalnız dialog yolunu sürmeye başlar ve telefonun asıl yolu olan bottom
sheet hiç test edilmez. `QuickAddLauncher.show` testinde tam olarak bu oldu ve
test iki kademede birden çalışacak biçimde parametrelendi. Yeni panel testleri
genişliği açıkça vermelidir.

### Ortak bileşen testleri (Grup 3)

- `app_money_text_test.dart`: işaret kuralı (nötr hareket işaret almaz),
  her etkinin kendi finans token rengini alması, karanlık temada karanlık
  paletten okuması, iptal edilmiş tutarın etkiden bağımsız nötrleşmesi,
  sabit genişlikli rakam ve ekran okuyucu cümlesi.
- `app_status_chip_test.dart`: rozetin ikon **ve** metni birlikte taşıması,
  ton başına container çifti ve 2.0x metin ölçeğinde taşmama.
- `app_submit_button_test.dart`: istek uçarken ikinci dokunuşun yutulması,
  bitince yeniden basılabilmesi, dışarıdan yönetilen meşguliyet ve kaçan
  hatanın hem bildirilmesi hem kilidi açması.
- `app_confirm_dialog_test.dart`: onay, vazgeçme ve barrier ile kapatmanın
  onay sayılmaması.

Bileşenler tema uzantısını okuduğu için test harness'ı `AppTheme.light()` ya
da `AppTheme.dark()` vermek zorundadır. `AppFinanceColors.of()` uzantı yoksa
sessiz varsayılana düşmez, hata verir; bu sayede temasız bir harness sessizce
yanlış renk üretmek yerine testi düşürür.

### Yapısal tasarım kapısı (Grup 4)

`test/architecture/design_tokens_test.dart` kaynak kodu tarar. Bu kurallar tek
tek widget testleriyle korunamaz: kural "hiçbir ekran kendi rengini/boşluğunu
uydurmasın"dır, yani ihlal her zaman henüz testi olmayan **yeni** bir dosyada
ortaya çıkar. Backend'de mimari testlerin katman bağımlılıklarını koruduğu
mantığın Flutter tarafındaki karşılığıdır.

| Kural | Gerekçe |
|---|---|
| Ham Material renk sabiti yok (`Colors.transparent` hariç) | Karanlık temaya tepki vermez ve kontrast kapısından geçmez |
| Boşluklar `AppSpacing` adlarından gelir | Sayı sayı uydurulan boşluk ekranlar arası görünür tutarsızlık üretir |
| Ekran genişliği doğrudan karşılaştırılmaz | Aynı kavramın iki eşiği (720/700) ekranları farklı davrandırmıştı |

Yorum satırları kural dışıdır; bu kararların nedenini anlatan yorumlar
yasaklanan ifadeyi örnek olarak yazar. Grafik/boş durum kutusu gibi yerleşim
yükseklikleri (`SizedBox(height: N, child: …)`) boşluk sayılmaz ve ölçeğe
zorlanmaz.

Kapının çalıştığı, üç ihlali birden içeren geçici bir dosyayla doğrulandı;
üçü de dosya ve satır numarasıyla raporlandı.

### Erişilebilirlik kapısı (Grup 5)

`test/helpers/accessibility.dart` üç yardımcı sunar ve sekiz ana ekranda
uygulanır (Özet, İşlem akışı, Bütçeler, İşlemler, Kartlar, Planlama, Veri
Araçları, Giriş):

- `pumpAtLargestTextScale` — Android'in en büyük ayarına karşılık gelen 2.0×
  ölçekte çizer.
- `expectNoOverflow` — `RenderFlex overflowed` bir exception olarak raporlanır;
  sessizce geçmemesi için açıkça tüketilip iddia edilir.
- `expectMeetsAccessibility` — `androidTapTargetGuideline` (48 dp dokunma
  hedefi), `labeledTapTargetGuideline` (adsız ikon butonu yok) ve
  `textContrastGuideline`. Ayrı kural yazılmadı; asıl kaynak platform
  kılavuzudur.

Kapı kurulduğunda dört gerçek taşma buldu ve hepsi aynı desendi: bir metin iki
buton arasında esnek değildi. Bütçeler 82 px, Özet 36 px, Planlama 152 px
taşıyordu; Planlama'nın rapor sekmesindeki ikinci ay seçici de aynı hatayı
taşıyordu. Bazı ekranların 2.0× ölçekte çalışan testi zaten vardı, ama taşmayı
iddia eden bir kontrol yoktu — ölçekte çizmek ile ölçekte doğru çizildiğini
doğrulamak aynı şey değildir.

Yeni bir ekran eklendiğinde bu kapı da eklenir; aksi hâlde ekran ölçek ve
kontrast kurallarının dışında kalır.

## Form paneli ve seçici test kapsamı

### Panel kapanışı — gerçek bir çökmeyi kapatan regresyon

`test/features/data_tools/data_tools_feature_test.dart` içindeki iki test
(`borç paneli vazgeçildikten sonra kırılmaz`, `borç paneli kaydettikten sonra
kırılmaz`) paneli açar, alanları doldurur ve **kapanışı sonuna kadar sürer**
(`pumpAndSettle`), ardından `tester.takeException()`'ın null olduğunu iddia
eder.

Testler yazıldıktan sonra eski `data_tools_page.dart` geri konularak
çalıştırıldı ve ikisi de düştü:

```text
A TextEditingController was used after being disposed.
'_dependents.isEmpty': is not true.   (framework.dart:6268)
```

Bu, kullanıcının cihazda gördüğü kırmızı ekranın aynısıdır. Sebep, panellerin
`await showDialog` döner dönmez controller'ı bırakmasıydı; o `await`
`Navigator.pop` anında döner, kapanma animasyonu daha bitmemiştir.

Mevcut testler bunu yakalamıyordu çünkü hiçbiri paneli **kapatmıyordu**:
doğrulama testi panel açıkken bitiyordu.

### Bileşen sözleşmeleri

| Test | Neyi sabitler |
|---|---|
| `test/core/widgets/app_form_sheet_test.dart` | Başlık ve iki eylem her zaman var; gönderim `null` dönerse panel açık kalır; vazgeçmek sonuç üretmez; üçüncü eylem kendi sonucunu döndürür; gönderim yokken buton devre dışı; kapanış sonrası hata yok |
| `test/core/widgets/app_date_field_test.dart` | Tarih alanı `InputDecorator` kullanır (metin alanlarıyla aynı dekorasyon), değer yokken durumunu söyler, seçilen günü `yyyy-MM-dd` olarak bildirir, 2.0× ölçekte erişilebilirlik kapısını geçer |
| `test/widget_test.dart` | `MaterialLocalizations` Türkçedir (`okButtonLabel == 'Tamam'`); delege düşerse Material metinleri sessizce İngilizceye dönerdi |

### Liste satırında kategori

`test/features/activities/activity_feed_page_test.dart` iki davranışı sabitler:
kullanıcı serbest metin yazdığında kategori alt satırda kalır ve başlık tek
satıra sınırlanır; kayıt adı zaten kategori adıysa kategori tekrar yazılmaz.

### Borç formu alanları

`borç paneli seçilen tarihleri ve notu gönderir` testi isteğe `startDate`,
`firstDueDate` ve `description` alanlarının gerçekten girdiğini doğrular. Eski
form ilk ikisini `today` sabitliyor, üçüncüsünü hiç göndermiyordu.


## Sekme başına taşma kapısı — geçen ama yanlış şeyi ölçen test

Veri Araçları'nın erişilebilirlik kapısı sayfayı yalnız ilk sekmesinde
çiziyordu; kalan dört sekme hiç ölçülmüyordu. Kullanıcı `Belgeler` sekmesinde
`RenderFlex overflowed` hatası gördü.

Sekme başına ölçen ilk deneme de geçti ve bu daha tehlikeliydi: sekme çubuğu
kaydırılabilir olduğu için 2.0× yazı ölçeğinde sondaki sekmeler görünür alanın
dışında kalıyor, `tester.tap` sessizce ıskalıyor ve test beş sekme için de ilk
sekmeyi ölçüyordu.

Düzeltilmiş kapı üç şey yapıyor:

1. `tester.ensureVisible` ile sekmeyi görünür alana getiriyor,
2. sekmeye özgü bir işaretin göründüğünü **ayrıca** iddia ediyor
   (`reason: '… sekmesine gerçekten geçilmedi.'`),
3. `Belgeler` sekmesinde bir işlem seçiyor — taşma ancak seçici, seçili öğenin
   metnini çizince ortaya çıkıyor.

Sahte veri de gerçeğe yaklaştırıldı: işlem açıklaması ve belge dosya adı artık
gerçekte olabilecekleri kadar uzun. Kısa örnekler (`Lunch`) taşmayı
gizliyordu — sahte verinin kısa olması, testin geçmesinin sebebiydi.

Kapı `isExpanded` geri alınarak doğrulandı: 1717 px taşmayla düştü.

## Borç test kapsamı

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Anüite iki yön | `AmortizationScheduleTests` | Oran 0, n=1, n=360, üst sınır; gidiş-dönüş toleransı; taşma yerine `1/(1+i)` çarpanı |
| Basit faizden ayrım | `AnnuityCostsLessThanSimpleInterest` | 300/%10/3'te 305,0138 < 307,50; iki model karışırsa düşer |
| Kaynak invariant'ı | `DebtAgreementTests` | İki kaynak dolu / hiçbiri dolu / alacakta gider / gelir kategorisi — dördü de reddedilir |
| SQL check constraint'i | `MigrationHistoryTests` | `CK_DebtAgreements_Source`, `CK_DebtInstallments_Split` ve ayrım kolonlarının nullable olması |
| Açılış aritmetiği | `DebtPayment_ConcurrentWriters…` | 1000 + 300 açılış − 400 taksit = 900 (eskiden 600 diyordu) |
| Faiz gideri | `LegacyInstallmentWithoutSplit…` | Ayrım doluyken 100 gider; `null` olunca 0 ve bakiye değişmez |
| Sorgu bütçesi | `AdvancedReport_LargeFixture…` | 44 komut sınırı; borç başına sorgu yüzlere çıkarırdı |
| Açılış tamamlama | `RecordDebtOpeningUseCaseTests` | Tek kez çalışır, ikincisi conflict; yabancı borç 404 |
| API sözleşmesi | `DebtEndpointTests` | Toplam↔oran çevirimi, çelişki reddi, `unrecorded` gönderilemez, taksit ayrımı |
| Flutter form | `data_tools_feature_test` | Kaynağa göre alan değişimi, tek maliyet alanı, kayıtsız açılış notu ve tamamlama |

**İki tuzak yeniden çıktı ve yeniden kayda geçti.** Borç panelinin alanları
varsayılan 800×600 test yüzeyine sığmıyor: `enterText` görünmeyen alanda
çalışır, `tap` sessizce ıskalar. Test yüzeyi büyütülmeden yazılan bir sürüm,
açılır listeyi hiç açmadan "geçiyordu". Kaynak alanına ayrıca bir `Key`
verildi; alanı sırasına göre bulmak, araya yeni bir alan eklendiğinde sessizce
başka bir alanı ölçmeye başlar.

## Yön, belge türleri ve belge başına dallanma

| Ne kanıtlanıyor | Nerede | Nasıl |
|---|---|---|
| Kapı matrisi: her (belge türü × niyet) kombinasyonu | `AnalyzeReceiptUseCaseTests` | Teori; tanınmayan değer reddeden tarafa düşüyor |
| Belge seviyesi retler yönden bağımsız | aynı | İade fişi hem gelir hem transfer niyetinde `refund_document` döndürüyor — **sıra hatası önce yeniden üretildi** |
| `bank_slip` üç banka türünü de okuyor | aynı | Teori; alışveriş belgesi `not_a_bank_slip` ile reddediliyor |
| İade eşleşmesi sahibine kapsamlı | `ReceiptEndpointTests` | Gerçek SQL: başka kullanıcının aynı harcaması eşleşmiyor |
| Kısmi iadenin kalanı sunucuda hesaplanıyor | aynı | 200 iade / 847,50 harcama → `remainingAmount = 647.5000` |
| Harcamadan büyük iade eşleşmiyor | aynı | Aritmetiği tutmayan aday sunulmuyor |
| Vade tarihi belgenin tarihinin yerine geçmiyor | `ReceiptDraftValidatorTests` | Gelecek vade korunuyor, `purchasedAt` değişmiyor |
| Taksit sayısı yalnız 2–360 | aynı | Teori: `1`, `0`, `400`, `üç`, `3 TAKSİT` → `null` |
| Ücret ana kayıtla aynı kaynağa yazılıyor | `quick_add_test` | Hesap ve kart kaynağı ayrı ayrı; ana tutar değişmiyor |
| Ücret kategorisi yoksa söyleniyor | aynı | Ana kayıt yazılı kalıyor, uyarı çıkıyor |
| Ücret satırı yalnız ücret varsa | `bank_document_decision_page_test` | Yok / okunamamış / sıfır → anahtar hiç çizilmiyor |
| Kart ödeme önerisi tek kullanımlık | `finance_feature_test` | Form kendini yeniden açmıyor, ücret bir kez yazılıyor |
| Elle açılan ödeme ücret yazmıyor | aynı | Dekontla ilgisiz ödemeye ücret iliştirilmiyor |
| Taksitli fiş plan formunu toplam tutarla açıyor | aynı | Bölme istemcide yapılmıyor; plan yazılmamış kalıyor |
| Borç verme varsayımları | `debts_page_test` | Tek taksit, vade +1 ay, faiz 0; ücret açılış hesabından |
| İade sayfası hiçbir şeyi kendiliğinden yapmıyor | `refund_decision_page_test` | Onay öncesi çağrı yok; eşleşme yoksa düğme yok |
| Fatura sorusu ön seçim yapmıyor | `invoice_decision_page_test` | İki cevap ayrı raporlanıyor; plan vadeden başlıyor |
| Her karar yolu Navigator'ı patlatmıyor | `bank_document_router_test` | Dört karar seçeneği + iade + faturanın iki cevabı, gerçek router ile |
| Menü fişi ve dekontu ayrı adlandırıyor | `quick_add_test` | `Fiş veya fatura okut` ve `Dekont okut` ayrı satır; `Fiş ile ekle` hiç yok |
| Dekont sayfası yön sormuyor | `receipt_scan_page_test` | Seçici hiç çizilmiyor, niyet dokunmadan `bank_slip` |
| Fiş sayfası yalnız yönü soruyor | aynı | `Harcama`/`Gelir` var, `Dekont`/`Transfer` yok — aynı soru iki kez sorulmuyor |
| Dekont rotası karar sayfasına ulaşıyor | `bank_document_router_test` | `bankSlipScanLocation` → karar sayfası → aktarma yolu, gerçek router ile |
| Borç kartı yönü, kalanı ve faizi ayırıyor | `debts_page_test` | Yön rozetle; faiz satırı yalnız faiz varsa |
| Taksit satırı durumu ve eylemi aynı yerde | aynı | Rozet + `Öde`; tarih okunur biçimde |
| Tahsilat hesabı önceden seçili değil | aynı | `Seç` hesap seçilene kadar pasif |
| Taksit planı satırları aynı boyda | `finance_feature_test` | `Gerçekleşti` rozeti / `Gerçekleştir` metin butonu; dolgulu buton yok |

## Flutter fiş yakalama

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Büyük fotoğraf sınıra iniyor | `receipt_photo_test` | 3000×4000 fotoğraf uzun kenarı 2400'e, boyutu 2 MB kapısının altına iniyor |
| Merdivenin son basamağı | `receipt_photo_test` | Hiç sıkışmayan görselde kalite ve çözünürlük sırayla düşüyor, 1400'de duruyor |
| Küçük fotoğraf büyütülmüyor | `receipt_photo_test` | 800×1200 aynen kalıyor; büyütmek bilgi eklemeden dosyayı şişirirdi |
| EXIF yönü piksele işleniyor | `receipt_photo_test` | `orientation=6` görselde 1200×600 → 600×1200; etiket küçültmede kaybolur, yön kaybolmamalı |
| Orijinal korunuyor | `receipt_photo_test` | Yüklenen kopya küçülürken orijinal byte'lar birebir aynı nesne (Grup 7'nin belgesi) |
| Okunamayan görsel cihazda duruyor | `receipt_photo_test` | Boş ve bozuk byte'lar `ReceiptImageException`; kota ve süre boşa harcanmıyor |
| İki bekleme ayrı | `receipt_scan_controller_test` | `picking` → `analyzing` → `idle`; tek "yükleniyor" ikisini aynı sanmaya iterdi |
| Vazgeçme hata değil | `receipt_scan_controller_test` | Seçici boş dönerse `noImageSelected`, hata mesajı yok |
| Hata kodu taşınıyor | `receipt_scan_controller_test` | `receipt.provider_unavailable` ve 401 ayrı ayrı; 401 `unauthorized` |
| Eski taslak silinmiyor | `receipt_scan_controller_test` | Başarılı okumadan sonraki başarısız deneme taslağı korur ve `isStale` yapar |
| İptal edilen sonuç düşer | `receipt_scan_controller_test` | `cancel()` sonrası geç gelen taslak ekrana basılmaz |
| Yeniden okuma fişi tekrar çektirmez | `receipt_scan_controller_test` | `retryAnalysis` seçiciyi açmadan ikinci analizi yapar (`pickCount` 1, `analyzeCount` 2) |
| Boş taslak "okundu" değil | `receipt_scan_controller_test` | Dört alan da `missing` gelen 200 yanıtı `draftIsEmpty` |
| Bilinmeyen alan durumu reddediliyor | `receipt_repository_test` | `probably` durumu `FormatException`; doğrulanmamışı doğrulanmış göstermez |
| Para biçimi zorunlu | `receipt_repository_test` | `847.5` reddedilir; dört ondalık sözleşmesi istemcide de kapalı |
| Yüklenen dosya JPEG | `receipt_repository_test` | `name="file"`, `filename="fis.jpg"`, `content-type: image/jpeg` |
| İsteğe özel zaman aşımı | `api_client_test` | Varsayılan bütçe 20 ms iken analiz çağrısı 5 sn ile geçiyor, override'sız çağrı `network.timeout` düşüyor |

**Bir kusur testte ortaya çıktı ve düzeltildi.** İlk sürümde fotoğraf yalnız
**başarılı** okumadan sonra saklanıyordu; sağlayıcı hata verdiğinde "yeniden
dene" hiçbir zaman çalışamazdı, çünkü elde fotoğraf kalmıyordu. Fotoğraf artık
okumadan önce saklanıyor.

**Bir test girdisi gerçekçi değildi.** Boyut kapısı önce saf gürültüyle
sınandı; gürültü JPEG'de hiç sıkışmadığı için görsel merdivenin en altına
düşüyordu ve "2400'e iniyor" kapısı yanlış sebeple kırmızıydı. Girdi gerçek
fotoğraf entropisine (yumuşak geçiş + hafif doku) çevrildi, patolojik durum
ayrı bir kapı oldu.

## Fiş onay formu

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Form önü dolu açılıyor | `quick_add_test` | Tutar, tarih, ad ve kategori önerilerden geliyor; her biri "Fişten okundu" diyor |
| Kaynak boş ve zorunlu | `quick_add_test` | Öneriler dolu olsa da `Kaydet` kaynak seçilmeden geçmiyor, hiçbir yazma olmuyor |
| Düzenlenen değer gönderiliyor | `quick_add_test` | Öneri 847,50 iken kullanıcı 900,25 yazınca sunucuya giden 900.25 |
| Okunamayan alan boş | `quick_add_test` | Tarih önerisi yoksa bugüne düşüyor, uydurulmuş fiş tarihi yazılmıyor |
| Şüpheli alan ayrı anlatılıyor | `quick_add_test` | `suspect` alanın yardımcı metni "şüpheli" diyor; sunucu uyarısı ayrıca görünüyor |
| İpucu sıralar, gizlemez | `quick_add_test` | Kart ipucunda kartlar üstte, hesaplar listede duruyor ve not görünüyor |
| Hiç okunmayan taslak | `quick_add_test` | Boş öneri kümesinde form "elle girebilirsiniz" diyor |
| Kamera ve galeri eşit | `receipt_scan_page_test` | İki giriş de ekranda; biri diğerinin arkasında değil |
| İki bekleme ayrı, iptal edilebilir | `receipt_scan_page_test` | "Fotoğraf seçiliyor" → "Fiş okunuyor" → `Vazgeç` sonrası iptal notu |
| Taslak forma öneri olarak geçiyor | `receipt_scan_page_test` | Tutar, ad ve kart ipucu `QuickAddPrefill` olarak çağırana veriliyor |
| Sağlayıcı hatası akışı tıkamıyor | `receipt_scan_page_test` | Hata ekranından `Bilgileri elle gir` formu **öneri olmadan** açıyor |
| Vazgeçme hata tonunda değil | `receipt_scan_page_test` | Seçici boş dönünce nötr not, hata görünümü yok |
| Oturum bitişi ayrı | `receipt_scan_page_test` | 401'de okuma hatası değil, oturum ekranı |
| Çeviri tahmin üretmiyor | `receipt_scan_page_test` | `missing` alan öneri üretmiyor, `suspect` durumu düzleştirilmiyor, bilinmeyen ipucu hiçbir kaynağı seçmiyor |

**İki kapı kod bozularak sınandı.** Kart ipucunun sıralamayı değiştirmesi ve
kategori önerisinin forma yazılması ayrı ayrı kırıldı; ikisi de kırmızıya
düştü, geri alınınca yeşile döndü.

## Fiş kaydı, belge ve rıza kapısı

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Rıza olmadan çağrı yok | `receipt_scan_controller_test` | `consentGranted` false iken `scan` seçiciyi hiç açmıyor, analiz çağrısı olmuyor |
| Rıza ekranı önce geliyor | `receipt_scan_page_test` | Kabul edilmeden `Fotoğraf çek` görünmüyor; kabul sonrası görünüyor ve cihaza yazılıyor |
| Reddetmek elle girişi bozmuyor | `receipt_scan_page_test` | `Bilgileri elle gir` formu açıyor, rıza `false` kalıyor |
| Cihazda hatırlananlar okunuyor | `receipt_scan_controller_test` | Rıza ve `Fişi sakla` son durumu depodan geliyor, değişince yazılıyor |
| Orijinal fotoğraf taşınıyor | `receipt_scan_page_test` | Forma giden byte'lar orijinal; anahtarın hatırlanan durumu da taşınıyor |
| Belge kayıttan sonra ekleniyor | `quick_add_test` | Yazılan işlemin kimliğine, orijinal byte'larla |
| Anahtar kapalıyken hiçbir yere yazılmıyor | `quick_add_test` | Gider yazılıyor, yükleme çağrısı hiç yapılmıyor |
| Belge hatası gideri geri almıyor | `quick_add_test` | Yükleme 503 dönse de işlem duruyor; uyarı form kapandıktan **sonra** görünüyor |
| Kart harcamasında sebep yazıyor | `quick_add_test` | Anahtar kapalı, gerekçe ekranda, harcama yine yazılıyor, yükleme denenmiyor |
| Fotoğraf yoksa anahtar yok | `quick_add_test` | Belgesiz taslakta `Fişi sakla` hiç çizilmiyor |

**Bir test gerçeği sınamıyordu.** Belge hatası uyarısı önce formu **kök
rotaya** koyan yardımcıyla sınandı; sayfa kapanınca altında ekran kalmadığı
için SnackBar da kayboluyordu ve kapı yalnız `pumpAndSettle` yapılmadığında
geçiyordu. Test, formu gerçekte olduğu gibi bir rotanın üstüne açacak biçimde
yeniden yazıldı; artık uyarının form kapandıktan sonra görüldüğünü sınıyor.

**İki kapı kod bozularak sınandı:** anahtar kapalıyken belgenin gönderilmemesi
ve belge hatasının kullanıcıya söylenmesi ayrı ayrı kırıldığında kırmızıya
düştüler.

## Belge türü kapısı (19 Ağustos 2026 cihaz bulgusundan)

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Dekont okunmuyor | `AnalyzeReceiptUseCaseTests` | `bank_document` → `receipt.bank_document`; mesaj transferi işaret ediyor |
| Fiş olmayan her şey reddediliyor | `AnalyzeReceiptUseCaseTests` | `other_document`, `not_a_document` ve **tanınmayan** değer → `receipt.not_a_receipt` |
| Boş okuma tek hata | `AnalyzeReceiptUseCaseTests` | Dört alan da okunamazsa `receipt.unreadable`; alan başına uyarı yok |
| Yarım okuma taslak üretiyor | `AnalyzeReceiptUseCaseTests` | Tek alan okunduysa taslak dönüyor — eksiği kullanıcı doldurur |
| Tür taşınıyor | `GeminiReceiptAnalyzerTests` | `documentType` yanıttan okunuyor |
| Bilinmeyen tür reddeden tarafa düşüyor | `GeminiReceiptAnalyzerTests` | Alan yoksa veya tanınmıyorsa `Unknown` — "fiş" varsayılmıyor |
| Prompt sınıflandırma istiyor | `GeminiReceiptAnalyzerTests` | İstek gövdesinde `bank_document` ve "ASLA toplama" var; eski "sana bir fiş veriliyor" cümlesi yok |
| HTTP sözleşmesi | `ReceiptEndpointTests` | İki yeni kod da `400` ve kod adıyla dönüyor |
| Dekontta yeniden dene yok | `receipt_scan_controller_test` | `canRetry` yalnız geçici hatalarda; dekont/fiş değil hatalarında kapalı |
| Dekont transfer ekranına yolluyor | `receipt_scan_page_test` | `Transfer olarak kaydet` görünüyor, `Yeniden dene` görünmüyor |
| Alakasız fotoğraf tek cümle | `receipt_scan_page_test` | Tek hata cümlesi + `Bilgileri elle gir`; transfer düğmesi yok |

**Kapı kod bozularak sınandı:** `canRetry` eski hâline (her hatada açık)
çevrildiğinde dekont testi kırmızıya düştü.

## Fiş saha koşumu düzeltmeleri (19 Ağustos 2026)

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| İade reddediliyor | `AnalyzeReceiptUseCaseTests` | `refund_receipt` → `receipt.refund_document`; mesaj paranın geri geldiğini söylüyor |
| İade türü taşınıyor | `GeminiReceiptAnalyzerTests` | `refund_receipt` yanıttan okunuyor |
| Prompt iadeyi ve kategori sıkılığını istiyor | `GeminiReceiptAnalyzerTests` | İstek gövdesinde `refund_receipt` ve "restoran, kafe" örneği var |
| Eski tarih siliniyor değil şüpheleniliyor | `ReceiptDraftValidatorTests` | 2016 tarihi korunuyor, `Suspect` ve `receipt.date_too_old` |
| Gelecek tarih hâlâ atılıyor | `ReceiptDraftValidatorTests` | Olmamış aya gider yazılamaz |
| HTTP sözleşmesi | `ReceiptEndpointTests` | `receipt.refund_document` → 400 |

Ayrıntılı saha koşumu: 2026-08-19 saha koşumu kaydında; belge önceki repoda (`Kisisel-Butce-Mobil`).


## Aynı belge koruması dört rafta (20 Ağustos 2026)

Grup 2'nin kapsamı yalnız gelir/gider tablosuydu; dekont ise transfer, kart
ödemesi veya alacak da olabiliyor ve o üç yolda uyarı **hiç** çalışmıyordu.

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Dekont dört rafta aranıyor | `SqlServerPersistenceIntegrationTests.ReceiptDuplicateLookup_FindsSlipRecordsOnEveryShelf` | Aynı gün/tutar/ad ile yazılmış transfer bulunuyor; sahibi olmayan kullanıcıya görünmüyor |
| Üçüncü ayak katı | aynı test | Ad değişince eşleşme yok — güvenilmeyen uyarı sonrakilerin bedelini öder |
| Niyet rafı seçiyor | aynı test | `Expense` niyeti transfer/kart/alacak raflarına hiç bakmıyor |
| Her raf gerçekten okunuyor | `…_FallsThroughToPaymentThenReceivable` | Transfer yokken kart ödemesi, o iptal edilince alacak raporlanıyor |
| Uyarı türü söylüyor | `AnalyzeReceiptUseCaseTests.Execute_BankSlipAlreadyRecorded_NamesTheKindInTheWarning` | `aktarma` / `kart ödemesi` / `alacak kaydı` mesajda geçiyor |
| Transfer niyeti de sorguluyor | `AnalyzeReceiptUseCaseTests.Execute_TransferIntent_IsCheckedForDuplicatesToo` | Erken `return` kalktı; lookup çağrılıyor |
| Kart ödemesi adı taşıyor | `receipt_scan_page_test` | `receiptCardPaymentPrefillFrom(...).description` karşı tarafı taşıyor; okunmamışsa `null` |

**Kapı kod bozularak sınandı:** `EfReceiptDuplicateLookup` eski hâline (yalnız
`Transactions`) çevrildiğinde iki SQL testi de kırmızıya düştü.

## Kart yüzeyi ve satır ritmi (20 Ağustos 2026)

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Menü kutuları yapışmıyor | `more_page_test` | Ardışık `AppCard`'ların dikdörtgenleri değmiyor |
| Menü satırı `AppListRow` | `more_page_test` | Sıra ve adlar satır bileşeninden okunuyor |
| Gecikme rozetle | `planned_activity_test` | `AppStatusChip('Gecikmiş')`, gider tonu; ham `Card` yok |
| Vakti gelmemiş plan tonunda | `planned_activity_test` | `Yaklaşan` rozeti `planned` tonunda |
| Özet kartı okunur tarih | `planned_activity_test` | `En yakını 20 Ağustos` — ham ISO değil |
| İptal edilmiş hareket | `transactions_page_test` | Semantik etiket `AppCard`'tan okunuyor, `İptal edildi` rozeti duruyor |

## Cihaz turu düzeltmeleri — kart sayısı, satır eylemi, rol tonu (20 Ağustos 2026)

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Menü tek kart | `more_page_test` | Yedi kapı için bir `AppCard`, aralarında ayrıcı; ikinci kart yalnız çıkış için |
| Menü sırası korunuyor | `more_page_test` | Sıra ve adlar `AppListRow`'lardan okunuyor |
| Taksit eylemi çerçeveli | `finance_feature_test` | `AppRowAction('Gerçekleştir')`; zemini `secondaryContainer`, birincil dolgu değil |
| Borç eylemi çerçeveli | `debts_page_test` | `AppRowAction('Tahsil et')` |
| Plan kartı tek satır | `planning_feature_test` | `SwitchListTile` ve tam genişlikteki `Sil` şeridi kalktı; `AppRowAction('Sil')` + tek `Switch` |
| Kart borcu gider tonunda | `planning_feature_test` | Kart dağılımı `expense`, hesap dağılımı **rolsuz** (`null`) — `neutral` maviyi getiriyor |

**Kapı kod bozularak sınandı:** `AppRowAction` yerine düz `TextButton`
konduğunda üç ekranın testi de kırmızıya düştü.

## Tekrarlayanlar sekmesinin yeniden tasarımı (20 Ağustos 2026)

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Plan satırı denetimsiz | `planning_feature_test` | Satırda `Switch`, `Sil`, `Duraklat` yok; tutar sağ blokta |
| Panel eylemleri taşıyor | `planning_feature_test` | Satıra dokununca `Duraklat` ve `Sil` çıkıyor |
| Silme onay istiyor | `planning_feature_test` | Panel → `Sil` → onay diyaloğu; `Vazgeç` hiçbir şey silmiyor |
| Onay kaydı tek yerde | `planning_feature_test` | `Planlanan kayıtlar` bölümü yok; onay Yaklaşanlar satırında |
| Onay yalnız onaylanabilirde | `planning_feature_test` | Kart ekstresi satırında `Onayla` yok; `canRealize` kimlikten eşleşiyor |
| Üretme sonucun yanında | `planning_feature_test` | `Bugüne kadar üret` Yaklaşanlar sekmesinde |

## Tekrarlayan gerçekleştirme kendi kaydını üretiyor (20 Ağustos 2026)

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Üretilmemiş tarih tek çağrıda | `RecurringUseCaseTests.RealizeDue_UngeneratedDate_GeneratesThenRealizes` | Occurrence yokken çağrı hem üretiyor hem gerçekleştiriyor |
| Aynı çağrı iki kez, tek hareket | `RecurringUseCaseTests.RealizeDue_Twice_ProducesOneMovement` | Dönem anahtarı ve gerçekleşmiş kısa devresi birlikte |
| İleri tarih reddediliyor | `RecurringUseCaseTests.RealizeDue_FutureDate_IsRefused` | `recurring.not_due_yet`; ne hareket ne occurrence yazılıyor |
| Pasif plan üretmeden reddediliyor | `RecurringUseCaseTests.RealizeDue_InactiveSchedule_IsRefusedBeforeGenerating` | `recurring.schedule_inactive`; geriye satır kalmıyor |
| HTTP sözleşmesi ve izolasyon | `RecurringEndpointTests.RealizeDue_UngeneratedDate_RealizesOnceAndRefusesTheFuture` | İki çağrı tek hareket, ileri tarih 400, başkasının planı 404 |
| Üretilmemiş satır iç durumunu yazmıyor | `planned_activity_test` | "Henüz oluşturulmadı" yok; buton açık |
| Vakti gelmemiş satırda eylem yok | `planned_activity_test` | `Gerçekleştir` hiç çizilmiyor (gri değil) |
| Yaklaşanlar doğru ucu seçiyor | `planning_feature_test` | Üretilmiş occurrence ucu, üretilmemiş plan + tarih ucu |
| Üretme düğmesi yok | `planning_feature_test` | `Bugüne kadar üret` hiçbir sekmede çizilmiyor |
| Uçtan uca tek adım | `stage11_planning_acceptance_test` | Plan açıldıktan sonra occurrence yok; iki `realizeDue` çağrısı tek hareket yazıyor |
| Onaysız para hareketi yok | `planning_feature_test` | Her iki yolda da `Gerçekleştirilsin mi?` açılıyor; onaylanmadan repository çağrılmıyor |

**Kapı kod bozularak sınandı:** `ActionTargetId` yeniden `null` yapıldığında
planlanan ekranının iki testi de kırmızıya düştü.

## Panel katmanı ve onay penceresi (20 Ağustos 2026)

| Kapı | Nerede | Neyi tutuyor |
|---|---|---|
| Panel kök Navigator'da | `app_adaptive_sheet_test` | İç Navigator'ın altında **değil**; FAB panelin üstünde kalmıyor |
| Transfer ayrıntısı panelde | `finance_feature_test` | `Durum: Aktif` metni yok, rozet var; iki uç (`Çıkan hesap`/`Giren hesap`) ve rapor kuralı yazılı |
| Vurgu satırı ayrı duruyor | `app_confirm_dialog_test` | `highlight` ve `message` ayrı ayrı görünüyor |
| Yıkıcı karar hata renginde | `app_confirm_dialog_test` | `destructive` onayı `colorScheme.error`; ilerleten karar varsayılanı kullanıyor |
| Bakiye cümlesi ekranda bir kez | `planned_activity_test` | Satırda görünmez, ekran başlığında ve satır semantiğinde durur |
