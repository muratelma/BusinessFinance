# Backup Restore Runbook

Bu runbook yalnız sentetik yerel veridir. Yazılan şema **v11**; okunabilen
şema **yalnız v11**.

**v11 Aşama 06.3 boyunca şekil değiştirir** (`PROJECT-ROADMAP.md` yedek şeması
politikası): Grup 2, 3 ve 5 onu ayrı ayrı değiştirir ve ara checkpoint'te
alınan bir yedeğin sonraki checkpoint'te okunamaması kabul edilir (veri
sentetik). Şekil Grup 5 sonunda sabitlenir.

v11'in bugünkü hâli (Aşama 06.3 Grup 2) v10'un **KDV ve indirilebilirlik
alanları olmadan** aynısıdır (ADR 0018): işlem, kart harcaması, cari
borçlandırma, yükümlülük ve POS tahsilatı `vatRate` / `vatAmount` taşımaz;
gider kayıtları `isTaxDeductible`, kategoriler `defaultIsTaxDeductible` taşımaz.
Yeni koleksiyon yoktur. v10'dan kalan alanlar yerinde:

- `savingsGoals[].scope` — işletme karşılığını şahsi birikimden ayıran etiket.

Vergi takvimi kaleminin dosyada **ayrı bir koleksiyonu yoktur**: kalem
tekrarlayan bir plandır ve `recurringTransactions` içinde durur; `frequency`
artık `quarterly` de olabilir.

v9'un taşıdığı iki koleksiyon aynen yerinde:

- `cashCounts` — gün sonu kasa sayımı. Sayım bir **gözlemdir**: hesap
  bakiyesine dokunmaz, gelir/gider yazmaz. **Beklenen tutar ve fark dosyada
  yoktur** — ikisi de kalıcı alan değil, sayım okunduğu anda hesabın kendi
  bakiyesinden türer. Yazılsalardı geri yüklenen veritabanında sayımın
  yanındaki sayı hesabın gerçek bakiyesiyle çelişebilirdi. Farkı onaylanmış
  sayım, o farkı yazan harekete `adjustmentTransactionId` ile bağlıdır ve bağ
  geri yüklerken **yeni** kimliğe çevrilir. Aynı gün ve aynı kasa için ikinci
  bir sayım varsa öncekisi iptal edilmiş olarak durur; SQL'deki filtreli tekil
  indeks bunu geri yüklemede de doğrular.
- `posSettlements` — POS tahsilatı. Tek kayıt **iki an** taşır (ADR 0014):
  tahsilat günü gelir brüt tutar kadar tanınır ve komisyon ayrı gider yazılır,
  hesap kıpırdamaz; `transferredOn` dolduğu gün hesap net tutar kadar artar ve
  gelir/gider yeniden yazılmaz. **Net tutar, komisyon oranı ve "yolda mı"
  dosyada yoktur**: net brütten komisyon düşülerek, oran ikisinden, yolda olma
  ise iptal ve geçiş bilgisinden çözülür. Oran yazılsaydı kuruşa yuvarlanmış
  komisyonla çelişen ikinci bir gerçek kaynağı doğardı (ADR 0009).

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
  planın toplam tekrar sınırı ve üretilmiş occurrence sayacı. Sayaç geri
  yüklerken dosyadan **kopyalanmaz**, occurrence geçmişi yeniden oynanarak
  türetilir ve dosyadaki değer yalnız doğrulama için okunur; aksi hâlde elle
  değiştirilmiş bir sayaç, sınırı dolmuş bir planı yeniden üretir hâle
  getirirdi.

Sözleşme karşı tarafı adla değil kimlikle gösterir; ad yedeğin içinde tek yerde
durur. Restore merge,
overwrite veya kullanıcı seçerek silme yapmaz; hedef kullanıcının finans alanı
boş olmalıdır. Yeni hesapta uygulamanın otomatik oluşturduğu, hiç değiştirilmemiş
başlangıç kategorileri boş alan sayılır ve yedekteki kategorilerle atomik olarak
değiştirilir.

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
