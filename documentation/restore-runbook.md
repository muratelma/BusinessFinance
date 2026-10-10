# Backup Restore Runbook

Bu runbook yalnız sentetik yerel veridir. Yazılan şema **v11**; okunabilen
şema **yalnız v11**.

**v11 Aşama 06.3 boyunca şekil değiştirir** (`PROJECT-ROADMAP.md` yedek şeması
politikası): Grup 2, 3 ve 5 onu ayrı ayrı değiştirir ve ara checkpoint'te
alınan bir yedeğin sonraki checkpoint'te okunamaması kabul edilir (veri
sentetik). Şekil Grup 5 sonunda sabitlenir.

v11'in bugünkü hâli (Aşama 06.3 Grup 4) bir yeni koleksiyon taşır:
`posDefinitions` (aşağıda). Bu koleksiyonu taşımayan, Grup 3 ve öncesinde
alınmış bir v11 yedeği **reddedilir** ("Every backup collection is required").
Geri kalanı v10'un **KDV ve indirilebilirlik
alanları olmadan** aynısıdır (ADR 0018, Grup 2): işlem, kart harcaması, cari
borçlandırma, yükümlülük ve POS tahsilatı `vatRate` / `vatAmount` taşımaz;
gider kayıtları `isTaxDeductible`, kategoriler `defaultIsTaxDeductible` taşımaz.
Yeni koleksiyon yoktur. v10'dan kalan alanlar yerinde:

- `savingsGoals[].scope` — işletme karşılığını şahsi birikimden ayıran etiket.

**Grup 3 vergi planını ekledi.** Vergi ayrı bir koleksiyon değildir (ADR 0018
İ6): tanımlı vergi `recurringTransactions` içinde, ödenen vergi `transactions`
ya da `charges` içinde durur. Eklenen alanlar:

- `categories[].isTax` — vergi işareti; "Ödenenler" bu kategorilerden okunur.
- `recurringTransactions[]` — `taxKind`, `dayOfMonth`, `selectedMonths` (ay
  kümesi, bit 0 Ocak); `amount` ve `sourceType` yalnız vergi planında boş
  olabilir; `frequency` artık `selected-months` da olabilir.
- `recurringTransactions[].occurrences[]` — kalemin **anlık görüntüsü**:
  `status` (`planned`, `realized`, `closed`), `amount` ("tutar belli oldu" ile
  yazılan dahil, boş olabilir), kaynak (`sourceType`, `accountId`,
  `creditCardId`; ödeme anında seçilen), `categoryId`, `scope`, `description` ve
  kapatılmış kalemde `closedByTransactionId` / `closedByChargeId` /
  `closedAtUtc`. Kapatan ödemenin bağı geri yüklerken **yeni** kimliğine
  çevrilir.

**Tekrarlayan plan geçmişi yeniden oynanarak kurulmaz.** Ritmi sonradan
değişmiş bir planın eski kalemleri yeni ritme uymaz; plan ve kalemleri anlık
görüntüden kurulur ve iki şey doğrulanır: `generatedOccurrenceCount` dosyadaki
kalem sayısına eşittir (elle büyütülmüş bir sayaç sınırı dolmuş bir planı
yeniden üretir hâle getirirdi) ve hiçbir kalem planın `nextOccurrenceDate`
gününden sonra değildir. Kalemler ayrı günlerde olmalıdır. Grup 2'de alınmış ve
tekrarlayan kalem içeren bir v11 yedeği, kalemlerin `status` alanını taşımadığı
için reddedilir (veri sentetik; aşamanın yedek politikası).

v9'un taşıdığı iki koleksiyon aynen yerinde:

- `cashCounts` — gün sonu kasa sayımı. Sayım bir **gözlemdir**: hesap
  bakiyesine dokunmaz, gelir/gider yazmaz. **Beklenen tutar ve fark dosyada
  yoktur** — ikisi de kalıcı alan değil, sayım okunduğu anda hesabın kendi
  bakiyesinden türer. Yazılsalardı geri yüklenen veritabanında sayımın
  yanındaki sayı hesabın gerçek bakiyesiyle çelişebilirdi. Farkı onaylanmış
  sayım, o farkı yazan harekete `adjustmentTransactionId` ile ya da farkı
  açıklayan aktarıma `adjustmentTransferId` ile ("Kendime aldım", şahsi
  hesaba; 8 Ekim 2026'da v11'e eklendi, alanı taşımayan v11 yedekte boş
  okunur) bağlıdır; ikisi birden dolu yedek reddedilir ve bağ
  geri yüklerken **yeni** kimliğe çevrilir. Aynı gün ve aynı kasa için ikinci
  bir sayım varsa öncekisi iptal edilmiş olarak durur; SQL'deki filtreli tekil
  indeks bunu geri yüklemede de doğrular.
- Adın karşılaştırma anahtarı (`NameKey`; kişi, hesap, kredi kartı, kategori
  ve POS) **yedekte taşınmaz**; addan yeniden hesaplanır. Eski bir yedek, ad
  tekliği sıkılaşmadan önce (kişilerde 8 Ekim, beş türde 9 Ekim 2026) açılmış
  aynı adlı kayıtlar taşıyabilir ("İş Bankası" ve "İŞ BANKASI", "Ali Can" ve
  "Alican"). Geri yükleme hepsini getirir ve birleştirmez; dosyadaki sırayla
  ilki adı tutar, sonrakiler kimlikleriyle ayrılan bir anahtar alır.
  Kategoride teklik türle birliktedir. Şema sürümü değişmedi. Migration
  `AddNameKeys` var olan satırların anahtarını SQL'de doldurur; eski aynı adlı
  kayıtlardan aktif olan (sonra kimlik sırası) adı tutar.
- `posDefinitions` — POS tanımı (Aşama 06.3 Grup 4, ADR 0019 T4): ad, banka
  hesabı, satış ve komisyon kategorisi, varsayılan oran, geçiş günü, iş günü
  seçeneği, aktiflik ve ana POS işareti (`isDefault`). Para taşımaz. Oran burada **yazılır** çünkü tanımın kendi
  alanıdır. Geri yüklemede tahsilatlardan önce kurulur ve kimliği yeniden
  eşlenir.
- `posSettlements` — POS tahsilatı. `posDefinitionId` tahsilatın yazıldığı
  tanımı gösterir ve geri yüklerken **yeni** kimliğe çevrilir; tanımsız
  girilen ve tanımlardan önce yazılmış tahsilatta boştur. Tek kayıt **iki an** taşır (ADR 0014):
  tahsilat günü gelir brüt tutar kadar tanınır ve komisyon ayrı gider yazılır,
  hesap kıpırdamaz; bir yatışa (`posDepositId`) bağlandığı gün hesap net tutar
  kadar artar ve gelir/gider yeniden yazılmaz. **Geçiş günü tahsilatta
  yazılmaz** (Aşama 06.3 Grup 5): yatışın günüdür ve ondan okunur. **Net tutar,
  komisyon oranı ve "yolda mı" dosyada yoktur**: net brütten komisyon
  düşülerek, oran ikisinden, yolda olma ise iptal ve yatış bağından çözülür.
  Oran yazılsaydı kuruşa yuvarlanmış komisyonla çelişen ikinci bir gerçek
  kaynağı doğardı (ADR 0009). İptal edilmiş tahsilat yatışa bağlı olamaz.
  `kind` (`Sale` | `Collection`, Aşama 06.3 Grup 5 teslim 3/3) kartla tahsili
  ayırır: tahsil türünde `categoryId` boştur ve `scope` yalnız komisyon varsa
  doludur. Alanı taşımayan önceki v11 dosyalarında tür yoktur ve satış sayılır.
  Her tahsil kaydı tam olarak bir `counterpartyPayments` satırına ya da bir
  yükümlülük kapanışına (`posSettlementId`) aittir; sahipsiz ya da iki kez
  bağlanan tahsil kaydı yedeği geçersiz kılar. Geri yüklemede tahsil kayıtları
  tahsilatlardan önce kurulur.
- `posDeposits` — POS yatışı (Aşama 06.3 Grup 5, ADR 0019 T5): hesap, yatış
  günü, **gerçekten yatan tutar** (`depositedAmount`), kesinti
  (`deductionAmount`), kesinti giderinin kimliği (`deductionTransactionId`;
  gider `transactions` arasındadır) ve iptal damgası. **Beklenen tutar dosyada
  yoktur**: yatan ile kesintinin toplamıdır. Geri yüklemede yatış, kapattığı
  tahsilatlarla birlikte domain kurallarından geçerek kurulur ve dosyadaki
  kesinti tahsilatların netinden yeniden hesaplananla **karşılaştırılır**;
  tutmuyorsa yedek reddedilir (dosyadaki sayı olduğu gibi yazılsaydı hesaba
  giren tutar yatışın söylediğinden ayrışırdı). Geri alınmış yatış tahsilat
  taşımaz: kaydı ve iptal edilmiş kesinti gideri kalır. Yatışın ve kesinti
  giderinin kimlikleri geri yüklerken **yeni** kimliklere çevrilir.

  Grup 4'te alınmış bir v11 yedeği `posDeposits` koleksiyonunu ve tahsilatın
  `posDepositId` alanını taşımadığı için reddedilir (veri sentetik; aşamanın
  yedek politikası).
- `dayCloses` — gün sonu (Aşama 06.3 Grup 5, ADR 0019 T1): kapatılan gün
  (`closedOn`), birkaç günlük Z'nin ilk günü (`rangeStart`, boş olabilir), Z
  numarası (`zNumber`, boş olabilir), "ek gün sonu" işareti (`isAdditional`),
  yazıldığı an ve iptal damgası. **Tutar taşımaz** (ADR 0019 İ3): ürettiği
  gelir `transactions`, ürettiği tahsilat `posSettlements` arasındadır ve ona
  `dayCloseId` ile bağlanır; iki alan da elle girilen kayıtta boştur. Hiç kayıt
  üretmemiş gün sonu da dosyadadır — günün kapalı olduğu bilgisi yalnız ondadır.
  Geri yüklemede gün sonu, kayıtlarıyla birlikte domain kurallarından geçerek
  kurulur (kayıtlar canlı ve kapatılan günlerin içinde); geri alınmış gün
  sonunun kayıtları iptal edilmiş olmalıdır, değilse yedek reddedilir. Var
  olmayan bir gün sonuna bağlı kayıt da reddedilir. Kimlikler geri yüklerken
  **yeni** kimliklere çevrilir. `dayCloses[].countedRecords` gün sonunun
  saydığı (tek tek girilmiş ve tutardan düşülmüş) kayıtları taşır: türü
  (`kind`) ve dosyadaki kimliği (`recordId`). Kaydın kendisi kendi
  dizisindedir; burada yalnız bağ durur. Geri yüklemede sayılan kayıt canlı ve
  elle girilmiş olmalı, bir kayıt en çok bir gün sonunda sayılmalı ve geri
  alınmış gün sonu sayılan kayıt taşımamalıdır; aksi hâlde yedek reddedilir.

  2 Ekim 2026'da (yatış tesliminde) alınmış bir v11 yedeği `dayCloses`
  koleksiyonunu taşımadığı için reddedilir (veri sentetik; aşamanın yedek
  politikası).

v8 ise v7'nin taşıdığı her şeyin (karşı tarafın kendisi — ad, not, aktiflik —
ve cari defterin iki hareket türü: borçlandırma `counterpartyCharges`,
tahsilat `counterpartyPayments`) üstüne üç bilgi eklemişti:

- `obligations` — tek seferlik yükümlülük. Ekonomik olayı **tanır**: kategori,
  kapsam, düzenleme tarihi ve **vade** taşır, hesap taşımaz. Onu kapatan nakit
  hareketi ayrı bir koleksiyon değil, yükümlülüğün içindeki `settlement`
  alanıdır: hesap taşır, kategori ve kapsam taşımaz (ADR 0014). Bire bir bağ
  dosyada da böyle durduğu için sahipsiz bir ödeme yazılamaz. **Gecikme dosyada
  yoktur** — kalıcı bir alan değil, vade ile okunduğu günün karşılaştırmasıdır.
- `counterpartyCharges[].dueDate` — cari borçlandırmanın isteğe bağlı vadesi.
  Yokluğu meşrudur; `null` gelen satıra tarih uydurulmaz.
- `recurringTransactions[].occurrenceLimit` ve `generatedOccurrenceCount` —
  planın toplam tekrar sınırı ve üretilmiş occurrence sayacı. Sayaç dosyadaki
  kalem sayısıyla doğrulanır (Grup 3'ten beri; öncesinde geçmiş yeniden
  oynanıyordu); aksi hâlde elle değiştirilmiş bir sayaç, sınırı dolmuş bir
  planı yeniden üretir hâle getirirdi.

Sözleşme karşı tarafı adla değil kimlikle gösterir; ad yedeğin içinde tek yerde
durur. Restore merge,
overwrite veya kullanıcı seçerek silme yapmaz; hedef kullanıcının finans alanı
boş olmalıdır. Yeni hesapta uygulamanın otomatik oluşturduğu, hiç değiştirilmemiş
başlangıç kategorileri boş alan sayılır ve yedekteki kategorilerle atomik olarak
değiştirilir.

## Veritabanı yükseltme notu — Aşama 06.3 Grup 5 (gün sonunun saydığı kayıtlar)

`AddDayCloseCountedRecords` migration'ı **veri kaybettirmez** ve mevcut hiçbir
tabloya dokunmaz: yalnız yeni, boş `DayCloseCountedRecords` tablosunu ve iki
indeksini kurar. Tablo tutar taşımaz. Tekil indeks (`UserId, Kind, RecordId`)
bir kaydın iki gün sonunda sayılmasını SQL'de de engeller. Bu migration'dan
önce girilmiş gün sonlarının (yalnız 4 Ekim 2026 deneme verisi) saydığı
kayıtlar bilinmez ve uydurulmaz: o kayıtlar bağsız kalır.

## Veritabanı yükseltme notu — Aşama 06.3 Grup 5 (kartla tahsil)

`AddCardCollections` migration'ı **veri kaybettirmez**. `PosSettlements`'a
`Kind` eklenir: nullable eklenir, mevcut her satır satış (`1`) olarak
doldurulur, sonra `NOT NULL` yapılır; kalıcı DEFAULT bırakılmaz.
`CategoryId` ve `Scope` yalnız gevşer (nullable); `CounterpartyPayments` ve
`ObligationSettlements`'a nullable `PosSettlementId` ile filtreli tekil indeks
ve foreign key eklenir. Yeni CHECK'ler (`CK_PosSettlements_Kind`,
`CK_PosSettlements_KindShape`, gevşeyen `CK_PosSettlements_Scope`) kolonlardan
ve backfill'den sonra gelir. Geri dönüş (`Down`) kartla tahsil kaydı varken
çalışmaz: kategorisi olmayan satır `NOT NULL` kategoriye dönemez.

## Veritabanı yükseltme notu — Aşama 06.3 Grup 5 (gün sonu)

`AddDayCloses` migration'ı **veri kaybettirmez** ve mevcut veriye dokunmaz:
`BudgetTransactions` ve `PosSettlements` tablolarına nullable, varsayılansız
bir `DayCloseId` kolonu ekler ve yeni, boş `DayCloses` tablosunu kurar.
**Backfill yoktur**: mevcut hiçbir kayıt bir gün sonundan gelmedi. Sıra kurala
uyar: önce kolonlar, sonra tablo ve indeksler, en son foreign key'ler.

`DayCloses` tutar kolonu taşımaz. İki filtreli tekil indeks kuralı SQL'de de
tutar: kullanıcı ve gün başına geri alınmamış, "ek" olmayan tek gün sonu; kullanıcı
başına geri alınmamış tek Z numarası.

Geri dönüş (`Down`) iki kolonu ve tabloyu düşürür; gün sonunun ürettiği
kayıtlar sıradan gelir ve tahsilat olarak kalır, hangi gün sonundan geldikleri
bilgisi geri gelmez — geri dönüş yalnız geliştirme içindir.

## Veritabanı yükseltme notu — Aşama 06.3 Grup 5 (giriş anı)

`AddEntryTimestamps` migration'ı **veri kaybettirmez**: yedi tabloya
(`BudgetTransactions`, `Transfers`, `CreditCardCharges`, `CreditCardPayments`,
`CounterpartyCharges`, `CounterpartyPayments`, `DebtAgreements`) nullable ve
varsayılansız bir `CreatedAtUtc` kolonu ekler. **Backfill yoktur**: mevcut
kayıtların ne zaman girildiği bilinmez; bugünün saatini yazmak bütün geçmişi
aynı ana girilmiş gösterirdi. Boş değer İşlemler'de günün sonuna düşer ve o
kayıt için "işlem sonrası bakiye" gösterilmez.

**Yedek** bu yedi kaydın `createdAtUtc` alanını taşır (v11, isteğe bağlı alan).
Geri yüklemede dosyadaki an yazılır; dosyada yoksa kolon boş kalır — geri
yükleme anı kaydın girildiği an değildir.

## Veritabanı yükseltme notu — Aşama 06.3 Grup 5 (yatış)

`AddPosDeposits` migration'ı **veri kaybettirmez**, ama mevcut veriyi yeni
biçime **taşır**. Sıra kurala uyar: önce `PosDeposits` tablosu ve
`PosSettlements`'a iki kolon (`PosDepositId` nullable, `Version` rowversion;
ikisi de varsayılansız), sonra backfill, en son kısıtlar ve tahsilat → yatış
foreign key'i.

- **Her "hesaba geçti" bir yatışa dönüşür**: `TransferredOn` dolu her tahsilat
  için aynı gün, tahsilatın netiyle ve **kesintisiz** bir yatış yazılır (eski
  yolda fark yazılamıyordu; uydurulmaz). Tahsilat o yatışa bağlanır;
  `TransferredOn` yerinde kalır ve artık yatışın gününün kopyasıdır.
- **Hesaba geçtikten sonra iptal edilmiş tahsilat** yeni kuralda bir yatışa
  bağlı kalamaz. Geçiş bilgisi kaybolmaz: aynı gün ve tutarla **iptal edilmiş
  bir yatış** olarak durur; tahsilattaki geçiş günü ve damgası boşalır.
- Hiçbir bakiye değişmez: yükseltme testi dolu bir veritabanında (geçmiş,
  yoldaki, geçip iptal edilmiş ve yoldayken iptal edilmiş tahsilat) hesap
  bakiyesini, yoldaki parayı ve aylık raporu yükseltmeden sonra ölçer ve
  taşınan yatışın yeni kuralla geri alınabildiğini doğrular.
- Yeni kısıtlar: tahsilatta yatış, geçiş günü ve damgası birlikte bulunur ya
  da hiç bulunmaz; iptal edilmiş tahsilat yatışa bağlı olamaz; yatışta kesinti
  ile gider kaydı birlikte bulunur ya da hiç bulunmaz; bir gider en çok bir
  yatışın kesintisidir.

Geri dönüş (`Down`) yatış tablosunu düşürür ve eski geçiş kısıtını yeniden
kurar; canlı tahsilatlar geçiş gününü korur. Kesinti giderleri sıradan gider
olarak kalır ve iptal edilmiş yatışların bilgisi geri gelmez — geri dönüş
yalnız geliştirme içindir.

## Veritabanı yükseltme notu — Aşama 06.3 Grup 3

`AddTaxPlans` migration'ı **veri kaybettirmez**. `RecurringTransactions` ve
`RecurringTransactionOccurrences` tablolarında `Amount` ile `SourceType`
nullable olur; plana `TaxKind`, `DayOfMonth`, `SelectedMonths`, kaleme
`ClosedByTransactionId`, `ClosedByChargeId`, `ClosedAtUtc` eklenir (hepsi
nullable, varsayılansız); `Categories.IsTax` nullable eklenir, varsayılan
setlerin iki vergi kategorisi ("SGK ve vergi ödemesi", "Vergi ve harç") bir
kez işaretlenerek doldurulur, sonra `NOT NULL` yapılır — kalıcı bir DEFAULT
kalmaz. Sıra kurala uyar: eski kısıtlar düşer, kolonlar eklenir ve gevşer,
backfill çalışır, yeni kısıtlar en son kurulur. Mevcut her satır yeni kısıtları
olduğu gibi sağlar; yükseltme testi dolu bir veritabanında bunu ve kısıtların
NULL'u "bilinmiyor" diye geçirmediğini doğrular.

## Veritabanı yükseltme notu — Aşama 06.3 Grup 2

`RemoveVatAndTaxDeductibility` migration'ı **veri kaybettiren** bir adımdır
(kullanıcı kararı, `docs/project-status.md`; ADR 0018). Beş tablodaki
`VatRate`/`VatAmount` kolonları, dört tablodaki `IsTaxDeductible` kolonu,
`Categories.DefaultIsTaxDeductible` kolonu ve bunları koruyan `CK_*` kısıtları
düşer. Sıra kurala uyar: kısıtlar kolonlardan önce düşer; geri dönüşte kolonlar
nullable ve varsayılansız eklenir, kısıtlar sonra kurulur (geri dönüş veriyi
geri getirmez). Kayıtların kendisi ve tutarları kalır; yükseltme testi dolu bir
veritabanında bunu doğrular.

## Veritabanı yükseltme notu — Aşama 06 Grup 2

`AddVerificationCodes` migration'ı **yalnız yeni bir tablo kurar**: mevcut hiçbir
tabloya kolon veya kısıt eklenmez, dolayısıyla backfill sorusu doğmaz. Tablo
doğrulama ve parola sıfırlama kodlarının **hash'ini** tutar; kodun kendisi
hiçbir kolonda durmaz.

**Yedek şeması ilerlemez** (o gün v10'du). Doğrulama kodu finansal bir kayıt
değil, on beş dakika yaşayan geçici bir kimlik durumudur; yedeklenip geri
yüklenecek bir geçmişi yoktur. Aynı gerekçeyle `RefreshSessions` de yedeğin
dışındadır. Geri yüklenen bir hesap kodlarını taşımaz ve taşımamalıdır —
taşısaydı, eski bir yedeği eline geçiren biri o kodları da eline geçirirdi.

Hesap silindiğinde bu tablodaki satırlar da silinir (ADR 0017); silme sırası
`EfUserAccountEraser` içindedir.

## Veritabanı yükseltme notu — Aşama 03 Grup 2

`AddObligationsAndCounterpartyDueDates` migration'ı yedek şeması sürümünden
ayrı bir SQL yükseltmesidir. Dolu olabilen `CounterpartyCharges` tablosuna
`DueDate date NULL` **varsayılansız** eklenir; geçmiş hareketlerin bilinmeyen
vadesi doldurulmaz. `Obligations` ve `ObligationSettlements` bu adımda boş
doğduğu için zorunlu kolonları backfill istemez. Yükseltme testi önceki
`LinkDebtsToCounterparties` şemasına gerçek bir cari satırı yazar, migration'ı
uygular ve vadenin `null` kaldığını doğrular.

Bu migration kendi checkpoint'inde backup biçimini değiştirmedi; yükümlülük ve
cari vadesi Aşama 03 Grup 7'de **v8** kapsamına alındı ve artık kayıpsız
taşınır.

## Veritabanı yükseltme notu — Aşama 03 Grup 3

`AddRecurringOccurrenceLimit` migration'ı `OccurrenceLimit int NULL` ve
`GeneratedOccurrenceCount int NOT NULL` alanlarını ekler. Sayaç önce nullable
eklenir, mevcut occurrence satırları owner + plan anahtarıyla sayılarak backfill
edilir, sonra zorunlu hâle getirilir ve CHECK kısıtları en son kurulur. Kalıcı
DEFAULT bırakılmaz. Mevcut planların sınırı bilinmediği için `OccurrenceLimit`
uydurulmaz ve `null` kalır.

Sınır ve sayaç Grup 7'de **v8** kapsamına alındı; sınırı dolmuş bir plan geri
yüklendiğinde pasif ve `nextOccurrenceDate` alanı boş döner.

## Veritabanı yükseltme notu — Aşama 06.2 (Kasa tasarımı)

`AddCashCountExpectedSnapshot` migration'ı `CashCounts` tablosuna tek bir
nullable kolon ekler: `ExpectedAtCount decimal(19,4)`. Kolon sayım anındaki
beklenen bakiyenin **gözlemidir**; hiçbir hesap onu okumaz. Varsayılan ve
backfill yoktur: eski sayımlarda boş kalır, çünkü o günün beklenen tutarını
bugünkü bakiyeden türetmek olmamış bir geçmiş uydurmak olurdu.

Yedek biçimi (**v9**) bu kolonu **taşımaz**. Geri yüklenen sayımlarda kolon boş
kalır ve `Son sayımlar` o satırlar için beklenen tutarı/farkı göstermez; sayılan
tutar ve düzeltme kaydı kayıpsızdır. Kolonu yedeğe almak bir sonraki biçim
sürümünün işidir.

## Veritabanı yükseltme notu — Aşama 04 Grup 4

`AddCashCountsAndPosSettlements` migration'ı `CashCounts` ve `PosSettlements`
tablolarını kurar. İkisi de bu adımda **boş doğar**, bu yüzden zorunlu kolonlar
backfill istemez ve kalıcı bir DEFAULT bırakılmaz; migration kuralının "gerçekten
boş tablo" istisnası burada bilinçli olarak kullanılıyor. Mevcut hiçbir tabloya
kolon eklenmez, dolayısıyla dolu bir veritabanında yorumlanacak bir geçmiş
yoktur.

Türetilen hiçbir şey kolon değildir: net tutar, komisyon oranı, "yolda mı" ve
kasa sayımının beklenen tutarı ile farkı şemada **bulunmaz**. Migration testi bu
yokluğu açıkça sınar. `CashCounts` üzerindeki filtreli tekil indeks
(`IsCancelled = 0`) bir gün ve bir kasa için tek açık sayım bırakır.

Bu migration kendi checkpoint'inde backup biçimini değiştirmedi; kasa sayımı
ve POS tahsilatı Aşama 04 Grup 8'de **v9** kapsamına alındı ve artık kayıpsız
taşınır. Grup 8 **şema değiştirmez**: yeni migration yoktur, yalnız var olan
iki tablo dosyaya girer.

## Ön koşullar

- SQL Server `healthy`, API `/health/ready` cevabı 200 olmalıdır.
- Backup dosyası `business-finance-backup` formatında ve şeması **v11**
  olmalıdır. v11, v6'nın taşıdığı her şeyin (her finansal kaydın kapsamı
  `scope`, hesap/kategori/kart varsayılan kapsamı `defaultScope`) üstüne cari
  defteri, yükümlülükleri, kasa sayımlarını, POS tahsilatlarını ve hedeflerin
  kapsamını ekler; KDV ve indirilebilirlik taşımaz. Tanıyan kayıt kategori ve kapsam taşır,
  hesap taşımaz; taşıyan kayıt hesap taşır, kategori ve kapsam taşımaz
  (ADR 0014) — iki kaydın alan listesi dosyada da bilerek farklıdır.
- **Yedek kullanıcı profilini (işletmeniz var mı) taşımaz.** Profil finansal
  bir kayıt değil, bir arayüz tercihidir; geri yüklenen hesabın kendi cevabı
  geçerli kalır. Kategoriler yedekten geldiği için kapsam varsayılanları da
  yedekten gelir ve raporlar doğru bölünür.
- **v2–v10 yedekleri `restore.unsupported_version` ile reddedilir ve
  yükseltilmez.** v10 KDV ve indirilebilirlik taşıyordu; Aşama 06.3 yalnız
  v11'i okur ve alanları sessizce düşürmek yerine dosyayı reddeder. v8'de kasa sayımı ve POS tahsilatı hiç yoktu; boş dizi
  yazarak yükseltmek dürüst olmazdı, çünkü o dosyayı yazan kullanıcı kartla
  yaptığı satışı elle bir gelir kaydı olarak girmiş olabilir ve hangi gelirin
  POS satışı olduğunu yalnız kendisi bilir — yükseltilseydi aynı satış iki kez
  sayılabilirdi. v2–v5'te kapsam alanı yoktu; v6'da cari defter yoktu —
  o dosya karşı tarafı yalnız sözleşmenin taşıdığı ad olarak biliyordu, açık
  bakiyesi ve hareketleri hiç yoktu; v7'de yükümlülük yoktu — ödenmemiş
  faturayı tanıyan ekonomik olay o dosyada hiç bulunmuyor. Eksik alanı
  doldurmak için bir değer seçmek, kullanıcının işletme ile cebi arasındaki
  ayrımını (ADR 0013), alacağını ya da bir gideri hangi ay ve hangi tarafta
  tanıyacağını uydurmak olurdu; hepsini yalnız kullanıcı bilir. Reddetme,
  sessizce yanlış bir geçmiş üretmekten iyidir. Eski bir yedeği taşımanın
  yolu yoktur; o veri sentetiktir ve yeniden girilir.
- Dosya en fazla 14 MiB envelope, decoded payload en fazla 10 MiB olmalıdır.
- Hedef kullanıcıda herhangi bir finans veya attachment metadata kaydı olmamalıdır.
  Yalnızca eksiksiz, aktif ve değiştirilmemiş başlangıç kategori setine izin verilir;
  özel, yeniden adlandırılmış veya pasif kategori hedefi dolu yapar.
- Gerçek veri kullanılmaz; dış paylaşım için backup şifreli değildir.

> **Yedek ile CSV dışa aktarma aynı şey değildir.** İşlem CSV'si okumak ve
> arşivlemek içindir; kaydın kapsamını (`scope` kolonu) taşır ama geri
> yüklenemez — gelir/gider türü ve kapsam ayrı kolonlarda durduğu için banka
> ekstresi importer'ına güvenle verilemez ve verilirse her hareket ikinci kez
> yazılır. Veriyi bir hesaptan diğerine taşımanın **tek** yolu bu runbook'taki
> yedek/geri yükleme akışıdır. İstemci, kendi dışa aktarımını içe aktarma
> ekranında tanır ve reddeder.

> **Cari defterin kendi CSV'si vardır** (`/api/v1/exports/counterparty-ledger.csv`).
> İşlem CSV'sine karşı taraf kolonu **eklenmedi**: o dosya `BudgetTransaction`
> tablosunun dökümüdür ve cari hareket orada hiç bulunmaz — kolon her satırda
> boş kalırdı. Her dışa aktarma tek kaydın dökümü olduğu sürece kullanıcı ne
> okuduğunu bilir. Cari CSV'si de geri yüklenemez.

> **Önceki uygulamanın yedekleri okunmaz.** Kişisel bütçe uygulaması
> `personal-budget-backup` format kimliğiyle ve `.pbbackup.json` uzantısıyla
> yazıyordu; bu uygulama `business-finance-backup` ve `.bfbackup.json`
> kullanır ve farklı format kimliğini `restore.invalid` ile reddeder. Bu
> bilinçlidir: iki uygulamanın verisi karışmaz. Devralınan veri dosya üzerinden
> değil, SQL seviyesinde `BACKUP`/`RESTORE` ile taşındı (bkz. ADR 0012).

## Tatbikat

1. Kaynak sentetik kullanıcıyla Flutter `Veri araçları > Yedek` ekranından backup
   oluştur ve güvenli yerel hedefe paylaş.
2. Boş ikinci sentetik kullanıcıyla giriş yap.
3. `Backup doğrula ve geri yükle` seçeneğiyle dosyayı seç.
4. Schema, entity count ve checksum özetini kontrol et; yalnız beklenen dosyada
   ikinci onayı ver.
5. Restore sonrası hesap/transaction, borç, hedef ve attachment listelerini aç.
   **Cari hesabı da aç:** karşı taraf listesi, notu, pasif olanlar ve her
   birinin açık bakiyesi kaynaktakiyle aynı olmalı. Bakiye kalıcı kolon
   değildir; hareketler eksik gelseydi bakiye sessizce küçülürdü.
   **`Diğer > Yükümlülükler` ekranını da aç:** yaklaşan, geciken ve kapanan
   sekmelerinin sayıları kaynaktakiyle aynı olmalı. Kapanmış bir yükümlülük
   yeniden açık görünüyorsa kapanış dosyadan gelmemiştir. Gecikme sekmesini
   vadeye göre kontrol et — gecikme dosyada taşınmaz, geri yüklenen vadeden
   yeniden türer. Tekrarlayan plan listesinde bitiş sınırlı bir planın
   `üretilen/toplam` sayacı da kaynaktakiyle aynı olmalıdır.
6. **Kapsamı doğrula:** Özet ekranında anahtarı `İşletme` ve `Şahsi`
   konumlarına al; iki tarafın gelir/gider toplamları kaynaktakiyle aynı
   olmalı. Bir hesabın ve bir kategorinin varsayılan kapsamının da geri
   geldiğini kontrol et — gelmezse yeni kayıtlar zinciri çözemez ve
   `*.scope_unresolved` ile reddedilir.
7. En az bir attachment'ı indir; kaynakla SHA-256/byte eşitliğini otomasyon doğrular.
8. Kaynak kullanıcıya geri dönerek kaynak kayıtların değişmediğini kontrol et.

## Beklenen hata davranışları

| Durum | Beklenen sonuç |
|---|---|
| Bozuk byte/hash veya eksik attachment | 400; hedefe yazma yok |
| Schema v2–v7 veya gelecek bir schema | 422 `restore.unsupported_version`; yükseltme denenmez |
| Dolu hedef kullanıcı | 409 destination not empty; mevcut kayıt korunur |
| SQL constraint/save hatası | Transaction rollback; hedef SQL graph'ı boş |
| Object write sonrası SQL hatası | O çağrıda yazılan object key'ler silinir |
| API/SQL bağlantı kesilmesi | Başarı varsayılmaz; hedef ve trace tekrar incelenir |

## Geri dönüş ve inceleme

Restore “undo” endpoint'i içermez. Başarısız restore atomik rollback yapmalıdır;
manuel tablo silme uygulanmaz. Beklenmeyen kısmi durum görülürse servisi durdur,
sentetik hedefi izole et, trace id ve DB/object metadata sayımlarını kaydet. Kaynak
backup dosyasını değiştirme. Gerçek veri için olay müdahalesi, şifreleme anahtarı,
retention ve yönetilen object store prosedürü bulut aşamasından (Aşama 07) önce ayrıca yazılmalıdır.
