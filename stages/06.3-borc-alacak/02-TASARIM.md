# Borç, alacak ve cari — tasarım (16 Kasım 2026)

Durum: **öneri, kullanıcı onayı bekliyor**; 16 Kasım görüşmesinin kararları
(K3–K4'ün yeni hâli, K14–K18) işlendi (`03-PLAN.md` Faz 0). Onaylanınca
ADR 0022 olarak yazılır ve bağlayıcı olur. Gerekçe `01-ARASTIRMA.md`.

## 1. Tek cümle

**Kişiyle olan her şey kişinin hesabında yürür; yalnız taksitli olan ya da
kişisi olmayan borç ayrı bir kayıttır. Hangisinin nereye gideceğini kullanıcı
değil uygulama bilir.**

## 2. Kararlar

Her kararın yanında önceki karardan ayrıldığı yer yazılı. Ayrılan kararlar
ADR 0020 İ9–İ10 ile ADR 0021 İ3, İ8, T7 ve T8'i değiştirir; ADR 0021'in geri
kalanı (tek borç modeli, kısmi ödeme, istek kimliği, bağlı etkiler, devir)
yerinde durur.

| # | Karar | Önceki durum | Neden |
|---|---|---|---|
| K1 | Kullanıcıya "nasıl takip edilsin" sorulmaz. Kural sabittir: **taksitli ya da kişisiz** ise ayrı kayıt (planlı borç), **diğer her şey** kişinin hesabına | Girişte `Cari hesaba yaz` / `Ayrı borç olarak takip et` sorusu (ADR 0021 İ3, T7) | İncelenen hiçbir ürün bu soruyu sormuyor; soru bir muhasebe yöntemi sorusu. Apple Card aynı kuralı kullanıyor: taksitliyse ayrı |
| K2 | Kişinin hesabı **borç parayı da** tutar: `Borç verdim` / `Borç aldım`. Para hesaptan çıkar ya da girer, gelir/gider yazılmaz | Borç para yalnız `Debt` olarak, plan seçilerek açılıyordu | "Arkadaşa 5.000 verdim" bir plan değil; en basit kayıt en ağır formu istiyordu |
| K3 | **Kapsamı kişi taşır.** İşletmesi olan kullanıcı kişiyi eklerken `İşletme` (müşteri, tedarikçi) ya da `Şahsi` (aile, arkadaş) seçer; ön değer girişe göre gelir ve sonradan değiştirilebilir. İşletme kişilerinin hesabı **cari hesaptır**. Gelir ya da gider yazan kaydın rapordaki kapsamı ise her kayıt gibi kategori kuralından gelir; ikisi ayrı şeylerdir. Taksitli ve kişisiz kayıt kendi kapsamını taşır (satış ve alımda kategoriden, kredide formdaki seçimden; ön değer kişinin türü): aynı bankadan işletme kredisi ve konut kredisi ayrı görünümlerde durur | Cari yalnız İşletme (ADR 0020 İ9); kişide kapsam yoktu, kayıt her zaman `İşletme` yazılıyordu | Kullanıcı kararı (16 Kasım). Ödeme kapsam taşımadığı için bakiye kayıt kayıt bölünemiyor; kişi üzerinden temiz bölünür. Kişinin türünü değiştirmek hiçbir tutarı değiştirmez, yalnız listeyi yeniden gruplar |
| K4 | `Borç ve alacak` uygulamanın kapsam anahtarını izler: `Hepsi · İşletme · Şahsi`. `İşletme` görünümü cari hesaptır. Kişi satırı kişinin türüne, kişisiz kayıt kategorisinin kapsamına göre süzülür. `Hepsi`de satırda kapsam etiketi yazar | Borçlarım/Alacaklarım'da kapsam filtresi + ayrı Cari hesap listesi | Cari, tanımı gereği işletmenin borç-alacağıdır; ayrı sayfa değil, aynı sayfanın görünümü |
| K5 | Tek sayfa: `Borç ve alacak`. Ayrı `Cari hesap` listesi, ayrı cari sayfası ve `Yükümlülükler` kalkar | Borç ve alacaklar + Cari hesap listesi + kişi sayfasının içinde cari sayfası | Aynı kişi dört listede görünüyordu |
| K6 | Kişi sayfasında **tek** `Tahsilat` ve **tek** `Ödeme` düğmesi vardır; kişinin hesabına yazar ve hedef sormaz. Taksitli kaydın ödemesi kendi sayfasından yapılır | `Hangi alacağa kaydedeceksiniz?` sorusu | Khata kalıbı; Apple Card'da olağan ödeme bakiyeye, taksit kendi sayfasından |
| K7 | Kişinin hesabında ödeme hiçbir kayda bağlanmaz. Hangi kaydın açık, hangisinin gecikmiş olduğu okunurken **en eski vadeden başlayarak** hesaplanır | Aynı (ADR 0021 T8); 29 Ekim'de dağıtım modeli denendi | Yürüyen bakiye; dağıtım kaydı ve geri alma sırası gerekmez. İptal edilen her şey yalnız yeniden hesaplatır |
| K8 | Fazla ödeme reddedilmez, kırpılmaz; karşı yönde bakiye olur. Ekran kaydetmeden önce söyler | Aynı (cari); borçta reddediliyordu ve yönlendirme metni vardı | Para gerçekten verildiyse yazılabilmeli |
| K9 | Kişiye bağlı, taksitsiz yeni kayıt artık `Debt` olarak **açılmaz**. Sunucu eski kayıtları ve yedekleri okumaya devam eder; kişi sayfası onları ayrı satır olarak gösterir | Kişili tek ödemeli ya da plansız borç açılabiliyordu | K1'in sonucu. Eski veri taşınmaz, uydurulmaz |
| K10 | Fişten "henüz ödemedim" yolu kurala uyar: kişi eşleşti ya da kullanıcı kişiyi kabul etti ise kişinin hesabına vadeli alım; kişi yoksa kişisiz kayıt | Her zaman yükümlülük yazıyordu | K1 |
| K11 | Listeler sade: üç sekme (`Alacaklar · Borçlar · Kişiler`), bölümler (`Geciken · Yaklaşan · Diğer`), arama, tek anahtar (`Yalnız gecikenler`). Sıralama sabittir | Beş `Göster` seçeneği, üç sıralama, kapsam, dönem menüsü | Seçenek sayısı kararların uzamasının nedeniydi; ihtiyaç çıkarsa eklenir |
| K12 | Arayüzde "cari" kelimesi yalnız işletmesi olan kullanıcıya, kişi sayfasında alt başlık olarak yazılabilir; menü adı ve sayfa adı değildir | `Cari hesap` menüsü ve ön ayarı (`hasCounterpartyLedger`) | Tek sayfa; kelime bir sayfayı değil bir alışkanlığı karşılıyor |
| K13 | Ekranları Claude Code tasarlar ve kurar; ayrı bir Claude Design turu açılmaz. Tasarım hazır bileşenlerin hızlıca dizilmesi değildir: her ekran için önce başvuru incelemesi ve yazılı ekran tarifi yapılır (`05-EKRAN-TASARIMI.md`), sonra kurulur, kareleri çıkarılır ve denetim listesinden geçer. Ölçüler ortak kuraldan gelir; ortak kural bir yerde daha kötü sonuç veriyorsa daha iyi ölçü seçilir ve raporda yazılır | Brif 07 → Claude Design → uygulama | Kullanıcı kararı (16 Kasım): "üstüne düşünülmüş, profesyonel tasarım; ortak ölçülere özen, daha iyisi varsa seç ve belirt" |

| K14 | Taksitli kayıt listede **kendi satırıdır**; kişi satırı yalnız taksitsiz hesabı gösterir. Kişi sayfasının üstündeki toplam ve gecikmiş tutar taksitli kayıtların kalanını **içerir**. Kayıt sayfası nereden açılırsa açılsın aynı sayfadır | Kişi bir kez, cari satırı tek satır | Taksidin kendi takvimi var; aynı tutar iki satırda sayılmaz |
| K15 | Kişinin hareketlerinde ay seçici, tür filtresi (`Hepsi · Satış ve alım · Tahsilat ve ödeme`) ve iki yönlü kişide yön filtresi (`Alacak tarafı · Borç tarafı`) vardır | Yalnız tarih | Kullanıcı önerisi (16 Kasım) |
| K16 | Çek ve senet ilk sürümde ayrı kayıt değildir: borç durur, vadesi çekin gününe çekilir, çek ödenince ödeme yazılır | — | Kullanıcı kararı (16 Kasım) |
| K17 | Kredide uygulama faiz hesaplamaz; kullanıcı bankanın planını girer (çekilen tutar, taksit sayısı, taksit tutarı, ilk taksit günü). İlk sürüm eşit taksitli krediyi ve ödemesiz dönemi karşılar | ADR 0009'un anüite dağılımı sunucuda duruyor | "Tutar türetilmez" ürün ilkesi; bugünkü davranış Faz 1'de bu kuralla karşılaştırılır |
| K18 | Uzun takvim kısaltılır: sıradaki üç taksit görünür, gerisi `Tümünü göster` ile açılır; ödenenler kapalı gelir | — | 120 taksitli konut kredisi |

## 3. Bilinerek kabul edilen bedeller

- **Seçerek ödeme yok.** "Yeni faturayı ödedim, eskisi dursun" denemez; ödeme
  en eski vadeyi kapatmış sayılır. Toplam bakiye her zaman doğrudur; etkilenen
  yalnız hangi kaydın gecikmiş göründüğüdür. İhtiyaç çıkarsa "bu ödemeyi şu
  kayda say" işareti eklenir (`Sonra` listesi).
- **Kişili tek fatura kendi başına kapanmaz**, kişinin hesabında bir satırdır.
- **Kişinin taksitsiz hesabı tek kapsamdadır** (K3). Hem müşteriniz olan hem
  şahsen borç verdiğiniz kişide iki tutar aynı hesapta toplanır ve kişi tek
  görünümde durur. Rapordaki gelir ve gider bundan etkilenmez (kategoriden
  gelir); taksitli kayıtlar da etkilenmez (kendi kapsamları var). Değerlendirilen
  seçenek "kişi başına iki hesap" (işletme ve şahsi) reddedildi: bu, nadir
  olan karma ilişkiyi tam çözer ama yaygın olan durumu bozar — toptancıdan eve
  alınan mal toptancıya olan borcu iki hesaba böler ve tek ödeme iki kayıt
  ister; ayrıca "hangi hesaba" sorusunu geri getirir. Karma ilişkiyi gerçekten
  ayrı tutmak isteyen kullanıcı ayırt edici adla ikinci bir kişi açabilir
  (`Ayşe Terzi` / `Ayşe Terzi şahsi`); ihtiyaç görülürse "bu kişiyle şahsi
  hesabı ayrı tut" seçeneği `Sonra` listesindedir.
- **İki temsil bir süre birlikte yaşar** (K9): eski kişili borçlar ayrı satır
  olarak görünür.

## 4. Kavramlar

| Kavram | Ne | Sunucudaki karşılığı |
|---|---|---|
| Kişi | Müşteri, tedarikçi, arkadaş, banka. Tek kayıt; türü `İşletme` ya da `Şahsi` | `Counterparty` (+ tür) |
| Kişinin hesabı | Kişiyle yürüyen iki bakiye: alacağınız, borcunuz | `CounterpartyCharge` + `CounterpartyPayment` üstünden hesaplanan `CounterpartyBalance` |
| Taksitli kayıt | Kendi takvimi olan borç ya da alacak (kredi, taksitli satış) | `Debt` (plan `Taksitli`) |
| Kişisiz kayıt | Kişisi olmayan tek borç (fatura, sigorta) | `Debt` (plan `Tek ödeme` ya da `Plan yok`) |

**Hesap kayıtları:**

| Kayıt | Bakiye | Para | Gelir/gider | Alanlar |
|---|---|---|---|---|
| Veresiye satış | Alacağınız + | — | Gelir, o gün | kişi, tutar, gün, kategori, vade (isteğe bağlı) |
| Vadeli alım | Borcunuz + | — | Gider, o gün | aynı |
| Borç verdim | Alacağınız + | Hesaptan çıkar | — | kişi, tutar, gün, hesap, vade (isteğe bağlı) |
| Borç aldım | Borcunuz + | Hesaba girer | — | aynı |
| Tahsilat | Alacağınız − | Hesaba girer ya da POS'la yola çıkar | — | tutar, gün, hesap ya da POS |
| Ödeme | Borcunuz − | Hesaptan çıkar ya da kart borcu artar | — | tutar, gün, hesap ya da kredi kartı |
| Açılış bakiyesi | Alacağınız / borcunuz | — | — | iki tutar, "itibarıyla" günü |
| İndirim | İlgili alım ya da satışın bedeli düşer | — | Kendi gününde azalır | tutar |
| Vazgeçme | Kalan düşer | — | O gün gelir ya da gider | tutar, kategori |
| Avans aldım / verdim | Alacak yoksa borcunuz + / borç yoksa alacağınız + | Hesaba girer / çıkar | — | **Tahsilat ya da ödemedir** (17 Kasım, sapma): ayrı kayıt olsaydı sonraki satış ya da alımdan düşmezdi (senaryo 22) |

"Tanır / taşır" ayrımı (ADR 0014) aynen geçerli: satış ve alım tanır, borç para
ve ödeme taşır.

## 5. Kural: kayıt nereye gider

```
Taksitli mi?            → evet → Taksitli kayıt (kişi isteğe bağlı)
Kişi var mı?            → hayır → Kişisiz kayıt
                         → evet  → Kişinin hesabı
```

Kullanıcı bu kuralı görmez; formda yalnız `Taksitli` anahtarı ve kişi alanı
vardır.

## 6. Ekranlar

Bu bölüm ekranların **içeriğini ve davranışını** söyler; görsel tasarım
(hiyerarşi, yerleşim, ölçü, durumlar) `05-EKRAN-TASARIMI.md` içinde, Faz 4'te
yazılır (K13). Ortak bileşenler ve ölçü kuralları temeldir (`AppPageHeader`,
`ActivityTile`, `AppFormSheet`, `AppFilterSheet`, `AppAccountField`,
`AppCategoryField`, `AppPickField`, üç boş durum, `MoneyText`); bir yerde daha
iyi sonuç veren ölçü ya da yeni bileşen gerekiyorsa seçilir, kurala adıyla
eklenir ve raporda yazılır.

### 6.1 `Borç ve alacak` (liste)

- **Başlık:** ad, arama, `+`.
- **Özet:** iki sayı yan yana: `Alacağınız`, `Borcunuz`. Altında varsa
  `₺… gecikmiş`. Sayılar sunucudan ve **her şeyi içerir**: kişilerin
  hesapları, taksitli kayıtların kalanı ve kişisiz kayıtlar. Listedeki
  satırların toplamı bu sayıya eşittir; hiçbir tutar iki satırda yer almaz.
- **Sekmeler:** `Alacaklar · Borçlar · Kişiler`.
- **Alacaklar / Borçlar satırları:**
  - Kişi satırı: ad · alt satırda durum (`₺2.000 gecikmiş`, `Vade 30 Kas`,
    `Vadesiz`) · tutar (yalnız taksitsiz hesabı). Dokununca kişi sayfası.
    Kişi listede bir kez durur.
  - `Hepsi` görünümünde alt satırın başında kapsam etiketi yazar (`İşletme`,
    `Şahsi`); `İşletme` ve `Şahsi` görünümlerinde yazmaz.
  - Kayıt satırı (taksitli ya da kişisiz): başlık (açıklama, yoksa kişi) ·
    `3/12 taksit · sıradaki 15 Ara` · kalan. Dokununca kayıt sayfası.
- **Bölümler:** `Geciken` · `Yaklaşan` (30 gün) · `Diğer`. Bölüm içinde sıra
  sabit: en eski vade üstte, vadesizlerde büyük tutar üstte.
- **Kişiler:** bütün kişiler ada göre; satırda iki yönün tutarı (olan).
  Bakiyesi olmayan da görünür. Pasif kişi yalnız aramada, `Pasif` etiketiyle.
- **Kapsam:** sayfa kapsam anahtarını izler ve seçili kapsamı başlığında
  yazar; özet sayıları da o kapsamındır (sunucudan).
- **Anahtar:** `Yalnız gecikenler`.
- **Liste sonu:** `Kapananlar` (kapanmış taksitli ve kişisiz kayıtlar).
- **Boş durum:** ilk kullanımda ortak ilk kullanım görünümü, tek eylem
  `Borç veya alacak ekle`.

### 6.2 Kişi sayfası

- **Başlık:** ad, `+` (yeni kayıt; kişi hazır gelir), `⋮` (düzenle, açılış
  bakiyesi, pasife al).
- **Özet kartı:** `Alacağınız` ve/veya `Borcunuz` (yalnız olan). Gecikmiş tutar
  ve sıradaki vade. Hiçbir tutar eksi yazılmaz; fazla ödeme karşı yönün
  satırında görünür.
- **Eylemler:** `Tahsilat` (alacağınız varsa), `Ödeme` (borcunuz varsa).
- **Taksitli kayıtlar** (varsa): kayıt satırları; dokununca kayıt sayfası.
  Bu kayıtların kalanı ve gecikmiş taksitleri özet kartındaki tutarlara
  dahildir; düğmeler yalnız hesaba yazar.
- **Hareketler:** giriş sırasına göre, en yeni üstte; ay seçici, tür filtresi
  ve (iki yönlü kişide) yön filtresi yalnız bu listeyi süzer, özet güncel
  kalır. Açık kalan satış ve alımlarda vade rozeti.
  Satıra dokununca ortak işlem ayrıntısı paneli (işlem sonrası bakiye, iptal,
  indirim).
- **Boş kişi:** bölüm boş görünümü ve `+`.

### 6.3 Tahsilat / Ödeme paneli

Tek panel, tek adım.

- `Tutar`. Altında kısayollar: `Tamamı ₺3.500`, varsa `Gecikmiş ₺2.000`
  (tutarlar sunucudan).
- `Gün` (ön değer bugün; ileri gün yazılamaz).
- Yer: tahsilatta `Hesap` ya da `POS`; ödemede `Hesap` ya da `Kredi kartı`.
  Ön değer kişiyle son kullanılan.
- `Açıklama` (isteğe bağlı).
- Ödemede `Kalanı indirim say` seçeneği: erken ödeme iskontosunda (4.200
  borca 4.000) kalan tutar indirim olarak kapanır ve gider olmaktan çıkar.
- Tutar bakiyeyi aşıyorsa alanın altında: `₺200 fazla; kişinin sizdeki
  alacağı olarak kalır.` Kayıt engellenmez.

### 6.4 Yeni kayıt

`+` bir panel açar: **Ne oldu?**

| Seçenek | Alt yazı |
|---|---|
| Veresiye sattım | Sonra tahsil edeceğim |
| Vadeli aldım | Sonra ödeyeceğim |
| Borç verdim | Geri alacağım para |
| Borç aldım | Geri ödeyeceğim para, kredi |
| Avans aldım / verdim | İş ya da mal öncesi alınan, verilen para |

Seçim tam sayfa formu açar (kayıt formlarının ortak kuralı: alt çubuk yok).

- **Alanlar:** kişi, tutar, gün, kategori (yalnız satış ve alımda), hesap
  (yalnız borç parada), vade (isteğe bağlı), açıklama (isteğe bağlı).
- **Kişi** ortak seçicidir; yazılan ad listede yoksa `"…" kişisini ekle`
  satırı çıkar ve kişi aynı adımda oluşur. İşletmesi olan kullanıcıda yeni
  kişinin türü (`İşletme` / `Şahsi`) seçicinin içinde sorulur; ön değer
  satış ve alımda `İşletme`, borç parada `Şahsi`dir.
- **`Peşin ödenen`** (satış ve alımda, isteğe bağlı): tutar ve hesap; kayıtla
  birlikte kişinin hesabına ödeme ya da tahsilat olarak yazılır. Yalnız `Vadeli aldım`da boş bırakılabilir (kişisiz fatura).
- **`Taksitli` anahtarı** açılırsa taksit sayısı, ilk taksit günü ve (borç
  parada) aylık taksit tutarı ya da faiz sorulur; takvim önizlemesi
  sunucudandır. Kayıt taksitli kayıt olur.
- **`Uygulamadan önce oluştu` anahtarı:** gelir/gider ve para hareketi
  yazılmaz; kalan tutar ve "itibarıyla" günü yazılır.
- **Kapsam** bölümü yalnız satış ve alımda, ortak kuralla çizilir
  (`AppScopeSection`): yalnız iki kapsama açık kategoride sorulur. Borç
  parada kapsam sorulmaz; kişinin türü yeter.
- Kişi sayfasından açılınca kişi dolu gelir ve değiştirilemez.

Girişler: `Borç ve alacak` `+`, kişi sayfası `+`, `İşlem ekle` menüsünde
`Borç veya alacak`, fiş okumada "henüz ödemedim".

### 6.5 Kayıt sayfası (taksitli ya da kişisiz)

- Özet: kalan, ödenen, sıradaki taksit ve günü; faizlide `Kalan anapara`.
- Ana eylem: `Ödeme kaydet` / `Tahsilat kaydet` (faizlide `Taksit öde`).
- Takvim: sıradaki üç taksit; gerisi `Tümünü göster`. Ödenenler kapalı gelir.
- Ödemeler: her biri geri alınabilir.
- `⋮`: planı değiştir, indirim, iptal.
- Kişi adı kişi sayfasını açar.

### 6.6 Başka sayfalardaki karşılıklar

- **İşlemler:** tür adları `Veresiye satış`, `Vadeli alım`, `Borç verdim`,
  `Borç aldım`, `Tahsilat`, `Ödeme`, `Taksit ödemesi`. Başlık kuralı aynı:
  açıklama, yoksa kişi.
- **Özet ve planlananlar:** vadesi olan açık tutarlar ve taksitler bugünkü
  gibi planlanan görünüme girer.
- **Gün sonu:** yalnız İşletme kapsamlı veresiye satışları ve nakit
  tahsilatları sorar (bugünkü davranış).
- **Diğer menüsü:** `Borç ve alacak` tek giriş; `Cari hesap` ve
  `Yükümlülükler` kalkar.

## 7. Sunucu: bugünkü koda göre fark

Ayrıntılı şema ve uç tasarımı Faz 1'de kod okunarak yazılır
(`04-SUNUCU-TASARIMI.md`). Aşağısı kapsamdır.

| # | İş | Not |
|---|---|---|
| S1 | Hesapta borç para kaydı (`Borç verdim` / `Borç aldım`) | Para taşır, gelir/gider ve kapsam taşımaz, vade alabilir. Faz 1 kararı: borçlandırmaya tür değil **ayrı kayıt** (`04-SUNUCU-TASARIMI.md` §3.1) |
| S2 | Hesaba ödemede kredi kartı | Kart borcu artar, gider yazılmaz; `CardDebt` ve kalan limit okur (borçtaki yolun aynısı) |
| S3 | Borçlandırma isteğinde istek kimliği | Tekrar gönderilen istek ikinci kayıt yazmaz |
| S4 | Kişide tür (`İşletme` / `Şahsi`) ve kaydın kapsamının kategori kuralından gelmesi (K3) | Tür nullable başlar (eski kişinin türü uydurulmaz; Faz 1'de eski verinin nasıl okunacağı yazılır); `TransactionScopeResolution`; gün sonu yalnız İşletme kapsamlı satışı okur |
| S5 | Vazgeçme; ödemede kalanı indirim sayma | O gün gelir ya da gider; kalan düşer |
| S5b | Kredi kuralları (K17) | Bugünkü faizli borç davranışı (anapara/faiz dağılımı, devir, erken ödeme) kullanıcının girdiği planla karşılaştırılır; fark varsa kullanıcıya getirilir |
| S6 | Kişi sayfası okuması | Özet, gecikmiş, sıradaki vade, taksitli kayıt satırları, sayfalı hareketler; tek uç |
| S7 | Liste okuması | `debt-book`un yeni biçimi: iki toplam, yön başına kişi ve kayıt satırları (bölüm bilgisiyle), kişiler, kapanan sayısı |
| S8 | Fiş "henüz ödemedim" yönlendirmesi (K10) | Tekrar araması değişmez |
| S9 | Kalkanlar | `/api/v1/obligations` uçları; eski taksit ödeme ucu; profilde `hasCounterpartyLedger` |
| S11 | Hesaptaki vadeler planlananlara ve hatırlatmalara girer | Faz 1 bulgusu: planlanan projection bugün cari vadeleri okumuyor |
| S10 | Yedek | Şema sürümü ilerler; eski sürümler okunur. Tek kaynak `documentation/restore-runbook.md` |

Dokunulmayanlar: `Debt` kümesi, kısmi ödeme, plan değişikliği, kartla ödeme ve
POS'la tahsil (borçta), devir, `CounterpartyBalance` hesabı, gecikmenin en
eskiden okunması, gün sonu bağları, POS yatışı.

Para tarafının tek listeleri güncellenir: `RecognizedItems`,
`AccountMovements`, `CardDebt`, planlanan projection, net varlık,
`EfReceiptDuplicateLookup`, `ICategoryUsageReader`.

## 7a. Avans (17 Kasım 2026 araştırması)

Tanım (kullanıcı): avans, bir işin tamamlanmasından, bir hizmetin görülmesinden
ya da maaş gününden önce, **hak edilecek tutara mahsuben** yapılan ön ödemedir.
Yani sonra gelen satıştan, alımdan ya da maaştan düşülmelidir.

Sorun: §4 tablosu avansı "borç parayla aynı kayıt" sayıyordu, senaryo 22 ise
satıştan düşülmesini istiyor. Kişinin bakiyesi iki kovadır (alacak, borç); borç
para kendi kovasına yazılır ve öbür kovadan düşmez. Avans borç para gibi
yazılırsa senaryo 22 "alacağınız 5.000 · borcunuz 2.000" gösterir.

Dış inceleme: Tek Düzen Hesap Planı avansı ayrı hesapta (340 / 159) tutar ve
fatura kesilince mahsup eder; uygulamada çoğu kayıt doğrudan müşterinin
hesabına alacak yazılır. Paraşüt avansı "alacağı olmayan cariye tahsilat",
"borcu olmayan cariye ödeme" olarak yazar; Zoho Books ve ERPNext müşterinin
hesabında kredi olarak tutar ve sonraki faturadan düşer.

| Seçenek | Sonucu |
|---|---|
| **A — Avans tahsilat ya da ödemedir** (uygulanan) | Mahsup kendiliğinden olur; senaryo 22 tutar; yeni kayıt türü yok. Formda `Avans aldım / verdim` kalır. Akışta satır `Tahsilat` / `Ödeme` yazar |
| B — Ayrı avans kaydı; kişinin bakiyesi tek net tutar olur | Senaryo 22 tutar, ama borç para ile veresiye satış da birbirinden düşer; iki satırlı kart (`Size borcu` / `Sizin borcunuz`) ve bütün bakiye okumaları değişir |
| C — Ayrı avans kaydı; fatura gelince elle mahsup | "Hangi satıştan düşeyim" sorusu; ADR 0022'nin reddettiği ödemeyi kayda bağlama ve ön muhasebe yükü |

**Öneri: A.** Akışta adın görünmesi istenirse tahsilat ve ödemeye yalnız
etiket taşıyan bir `avans` işareti eklenir (tutara etkisi yok); Faz 7'de form
kurulurken karar verilir. **Maaş avansı** da A ile çalışır, koşulu: maaş
çalışanın hesabına gider olarak yazılır (5.000 avans ödendi → ay sonu 20.000
maaş kişinin hesabına → kalan 15.000 ödenir). Maaş kişiye bağlanmadan sıradan
gider yazılırsa avans hesapta açık kalır; form bunu yönlendirir (Faz 7).

**Anlaşma + avans tek formda** (kullanıcı önerisi, 17 Kasım): "10.000'e
anlaştık, vade 45 gün, bugün 4.000 avans" → satış formunda `Peşin / avans
alınan` (bu belgedeki `Peşin ödenen`in satış tarafı; sunucuda `paidNow`): 10.000
veresiye satış + bugün 4.000 tahsilat; kalan 6.000, vadesi kayıtlı.

| | Tek form (satış + avans) | Tek başına `Avans aldım` (satış sonra) |
|---|---|---|
| Adım | Tek | İki (avans şimdi, satış iş bitince) |
| Kişi listede | Baştan `Alacaklar`: 6.000 | Satışa kadar `Borçlar`: "borcunuz 4.000" |
| Vade, gecikme, hatırlatma | Kalan 6.000'in vadesi kayıtlı | Satış girilene kadar vade yok |
| Gelirin ayı | Bugün (kayıt ileri güne yazılmaz) | İşin bittiği gün |
| Fiyat belli değilse | Sonra iptal + yeniden kayıt | Sorun yok |
| İş iptal olursa | Satış iptal edilir, tahsilat kalır → "borcunuz 4.000"; birlikte silinmez | Tahsilat kalır; geri ödeme ayrı ödeme |
| Gün sonu | Aynı gün satış + tahsilat → ortak tutar sorulur | Yalnız tahsilat |

**Öneri:** ikisi de kalır. Satış formunda `Peşin / avans alınan` alanı (fiyat
kesin, gelir bugün yazılabilir); ayrıca tek başına `Avans aldım` (fiyat belli
değil ya da gelir iş bitince yazılmalı). Form ayrımı sormaz; hangi kayıttan
başlandığı ayırır. **Kullanıcı onayladı (17 Kasım 2026).**

Eksilerin karşılığı (kullanıcı kararı: "eksileri daha iyi hâle getir"):

| Eksi | Karşılık | Nerede |
|---|---|---|
| Gelir bugün yazılır | Alanın altında tek satır: "Satış bugünün geliri olur. İş bitince yazmak istersen yalnız avansı kaydet." İki yol açık kalır | Faz 7 (form) |
| Fiyat kesin değil | İptal + yeniden kayıt gerekmez: fiyat düşerse satışa **indirim** (var), artarsa fark için ek satış; kayıt sayfası önerir | Faz 7 (kayıt sayfası) |
| İş iptal olunca avans kalır | Avans satışa bağlıdır (`PaidWithChargeId`); iptalde sorulur: "Avans da silinsin (yanlış girdim)" / "Avans kalsın (iş iptal oldu)". Varsayılan "kalsın" | **Sunucu yapıldı** (17 Kasım): `paidNow`, `?withPaidNow=true`; soru Faz 7 |
| Gün sonunda ortak tutar sorusu | Bağ bilindiği için ortak tutar avans tutarıyla dolu gelir; kullanıcı yalnız gerekirse düzeltir | Gün sonu işi (bağ hazır; okuma değişmedi) |
| Tek başına avansta müşteri `Borçlar`da | Liste, borç tarafındaki tutarın fazla tahsilattan (avans) geldiğini ayırır; satır "Alınan avans 4.000" yazar | Faz 3 (liste okuması, S7) |

## 8. İlk sürümde olmayanlar (`Sonra`)

| Konu | Neden sonra | O zamana kadar doğru kayıt yolu |
|---|---|---|
| Hatırlatma mesajı ve ekstre paylaşımı | Yeni paket (paylaşım) gerekir; çekirdek değil | Cihaz üstü hatırlatma var |
| Ödemeyi belirli kayda sayma | Bedel bölümü | Toplam doğru; gecikme en eskiden |
| Gider ve gelir formunda kişi alanı | Kullanıcı kararı: bu sayfalar denendikten sonra | Peşin alım kişide görünmez |
| Çek ve senet takibi | K16 | Vade + ödeme |
| Altın ve döviz borcu | İlk sürüm yalnız TL | — |
| Kartla ödemede taksit | Tek çekim | — |
| Kişiyle şahsi hesabı ayrı tutma | Nadir; bedel bölümünde | İkinci kişi, ayırt edici adla |
| Karşılıklı düşme (mahsup) | Nadir | İki yön ayrı görünür; tahsilat ve ödeme ayrı yazılır |
| İade alacağı | ADR 0021 T3'te tanımlı, kapsamı büyük | İndirim + tahsilat |
| Kişi birleştirme | Ayrı konu | — |
| Faizli borçta kısmi ödeme, erken kapatma | ADR 0021'de zaten sonra | Tam taksit |
| Yaşlandırma raporu, fatura eşleştirme | Ön muhasebe; ürün sınırı dışı | — |

## 9. Kabul senaryoları

Faz 9'da gerçek API ve emülatörde bire bir denenir; sunucu testleri aynı
sayıları korur.

| # | Senaryo | Beklenen |
|---|---|---|
| 1 | Ayşe'ye 1.000 veresiye satış (vade 30 Kas), 600 tahsilat | Alacağınız 400; gelir 1.000 bir kez; hesap +600 |
| 2 | 1.000 vadeli alıma 1.200 ödeme | Borcunuz 0, alacağınız 200; uyarı gösterilir, kayıt yazılır |
| 3 | Anneme 5.000 borç verdim, vade yok | Alacağınız 5.000; hesap −5.000; gelir/gider 0; kapsam sorulmaz |
| 4 | Arkadaştan 3.000 borç aldım, 2.000 ödedim, 1.000'inden vazgeçti | Borcunuz 0; hesap +3.000 −2.000; vazgeçme günü 1.000 gelir |
| 5 | İki satış (10 Kas vadeli 500, 20 Kas vadeli 700), 25 Kas'ta 500 tahsilat | Gecikmiş 700 (ilk satış kapanmış sayılır) |
| 6 | 950'lik vadeli alımı kendi kredi kartıyla ödeme, sonra kart ödemesi | Gider bir kez 950; kart borcu +950; banka kart ödemesinde −950 |
| 7 | Veresiye alacağını POS'tan kartla tahsil | Alacak o gün düşer; para yolda; yatışla hesaba net geçer; gelir yeniden yazılmaz |
| 8 | Bankadan 18.000 kredi, 12 taksit | Taksitli kayıt; kişinin hesabı değişmez; kişi toplamında görünür; taksit kendi sayfasından ödenir |
| 9 | Kişisiz 1.200 sigorta faturası, 400 kısmi ödeme | Kişisiz kayıt; kalan 800 |
| 10 | Aynı tahsilat isteği iki kez | Tek kayıt |
| 11 | Tahsilatı iptal | Bakiye ve gecikme yeniden hesaplanır; başka kayıt değişmez |
| 12 | Gün sonunda sayılmış veresiye satışı iptal etme | `*.day_close_counted` |
| 13 | Şahsi kategoriyle vadeli alım | Gider Şahsi yazılır; gün sonu sormaz; borç listesinde görünür |
| 14 | Uygulamadan önce kalan 4.000 alacak | Gelir ve hesap hareketi yok; alacağınız 4.000 |
| 15 | Eski kişili tek ödemeli borcu olan yedek | Geri yüklenir; kişi sayfasında ayrı satır; kendi sayfasından ödenir |
| 16 | Başka kullanıcının kişisi | `NotFound` |
| 17 | Pasif kişi | Yeni kayıt yazılmaz; tahsilat ve ödeme yazılır |
| 18 | Toptancıdan üç vadeli alım (1.500, 1.700, 1.000), 2.000 ödeme | Borçlar'da tek satır 2.200; ilk alım kapanmış, ikincisinden 500 düşmüş sayılır |
| 19 | 10.000 alım, 3.000 peşin ödenen (tek form) | Gider 10.000; hesap −3.000; borcunuz 7.000; ikisi de kişinin hareketlerinde |
| 20 | 4.200 borca 4.000 ödeme + `Kalanı indirim say` | Borç 0; hesap −4.000; gider alım gününde 200 azalır |
| 21 | Vade farkı: toptancı 150 ekledi | Yeni vadeli alım 150 (kategori faiz); borç ve gider +150 |
| 22 | Müşteriden iş öncesi 2.000 avans | Borcunuz 2.000; hesap +2.000; gelir 0. Sonra 5.000 veresiye satış → alacağınız 3.000 |
| 23 | Kişinin 3.500 hesabı ve 4.500 kalan taksitli satışı var | Alacaklar'da iki satır (3.500 ve 4.500); kişi sayfasında toplam 8.000; listenin toplamı 8.000'i bir kez sayar |
| 24 | İşletme kişisi 4.200, şahsi kişi 5.000 borç | `İşletme` görünümünde 4.200, `Şahsi`de 5.000, `Hepsi`de 9.200 ve etiketler |
| 25 | Kişinin türü Şahsi'den İşletme'ye çevrilir | Hiçbir tutar, gelir ya da gider değişmez; kişi öbür görünüme geçer |
| 26 | İşletme kişisinden şahsi kategoriyle alım | Gider raporda Şahsi; borç İşletme görünümünde |
| 27 | Konut kredisi: 120 taksitin 37'si uygulamadan önce ödenmiş | 83 taksitli kayıt; geçmiş taksit, gelir/gider ve hesap açılışı yok; takvimde üç taksit + `Tümünü göster` |
| 28 | Borç çekle ödendi (çek 15 Aralık) | Ödeme yazılmaz; alımın vadesi 15 Aralık olur; çek ödenince ödeme |
| 30 | Aynı bankadan işletme kredisi ve konut kredisi | İki taksitli kayıt; biri `İşletme`, öbürü `Şahsi` görünümünde; banka kişi olarak bir kez |
| 31 | Müşterim Ayşe'ye ayrıca 2.000 borç verdim (kişi `İşletme`) | Tek hesap: alacağınız satışlar + 2.000; `İşletme` görünümünde; gelir yalnız satışlardan |
| 29 | Kredi taksitini kredi kartıyla ödeme | Kart borcu taksit kadar artar; kartın taksitlerine girmez; anapara gider olmaz, faiz payı olur |

## 10. Riskler

| Risk | Karşılık |
|---|---|
| K3 gün sonunu etkiler | Faz 2'de gün sonu testleri değişmeden geçmeli; yalnız İşletme süzgeci eklenir |
| Borç para kaydı hesap hareketleri listesine eksik girer | `AccountMovements` tek liste; `MoneyFoundation_*` testleri genişler |
| İki temsil (K9) kişi toplamında çifte sayım | Toplam tek okumadan gelir (S6/S7); istemci toplamaz; test senaryo 15 |
| Kapsam büyür | Faz başına tek checkpoint; plan dışı fikir `Sonra`ya yazılır |
| Kullanıcı ekranı emülatörde beğenmez | Faz 4–7'nin her biri kendi içinde kapanır; düzeltme o fazda yapılır, sonraki faz beklemez |
