# Borç, alacak ve cari — ekran tasarımı (Faz 4, 23 Kasım 2026)

İçerik ve davranış `02-TASARIM.md` §6'dadır; bu belge **nasıl göründüğünü**
söyler: bilgi hiyerarşisi, yerleşim, ortak bileşen ve ölçü adları, durumlar,
erişilebilirlik. Ölçüler ortak kuraldan gelir (`documentation/design-system.md`);
kuraldan ayrılan yer "Ortak kuraldan ayrılanlar" tablosundadır.

Görsel dilin kaynağı uygulamanın en yeni ekranlarıdır: Planlananlar (dönem
kartı, satır), plan ayrıntısı (koyu özet kartı, açık vade kartı) ve kredi kartı
sayfası. Kareler: `tasarim-onizleme/borc-alacak-2026-11-23/`.

Sunucu sözleşmeleri hazırdır (`documentation/debt-api-contract.md`); ekranda
görünen her toplam sunucudandır, istemci toplama ve çıkarma yapmaz.

## 1. Başvuru incelemesi

Kaynaklar `01-ARASTIRMA.md` §3 ve §6'dadır; burada ekran ekran hangi kalıbın
alındığı ve hangisinin alınmadığı yazılır. Uygulamalar cihazda denenmedi;
kalıplar yardım sayfaları, mağaza görselleri ve yaygın bilinen ekranlardan.

### 1.1 Liste

| Başvuru | Kalıp | Karar | Gerekçe |
|---|---|---|---|
| Khatabook, OkCredit | Üstte iki toplam (`alacağınız` / `vereceğiniz`), altında kişi listesi, satırda kişinin tutarı | **Alındı** | Esnafın ilk sorusu "toplam ne kadar, kimde" |
| Khatabook, OkCredit | Her satırdaki tutar tarafına göre yeşil/kırmızı | Alınmadı | Bizde taraf sekmeyle ayrılır; her tutarı boyamak kırmızıyı gecikmeden ayırt edilemez yapar. Renk yalnız gecikmeye ayrılır |
| Splitwise | Kişiler listesinde tutarın yanında küçük `size borçlu` / `borçlusunuz` etiketi | **Alındı** (`Kişiler` sekmesi) | İki yönlü kişide renk tek başına tarafı anlatmaz |
| Apple HIG — listeler | Bölümlenmiş, gruplanmış liste; bölüm başlığı | **Alındı** | `Geciken · Yaklaşan · Diğer` |
| Banka uygulamaları (Kredilerim) | Kredi satırında `kalan` + `3/12 taksit` + sıradaki gün | **Alındı** (kayıt satırı) | Taksitli kaydın kişiden farklı okunması gerekir |
| Khatabook | Arama alanı her zaman açık | Alınmadı | Özet, sekme ve arama birlikte ilk satırı ekranın üçte birine itiyordu; arama başlıkta ikon, dokununca alan olur |

### 1.2 Kişi sayfası

| Başvuru | Kalıp | Karar | Gerekçe |
|---|---|---|---|
| Khatabook, Splitwise | Üstte kişinin bakiyesi büyük yazıyla, altında eylem | **Alındı** | İlk bakışta okunacak tek sayı |
| Apple Card | Bakiye koyu kartta, ödeme eylemi kartın hemen altında; taksitler kartın sayfasında ayrı satırlar | **Alındı** | Plan ayrıntısının koyu kartıyla aynı dil; taksitli kayıt K14 |
| Khatabook | Altta sabit iki renkli düğme (`Verdim` / `Aldım`) | Alınmadı | Bizde sayfanın altında yüzen alt çubuk var; ikinci bir sabit şerit onunla yarışır (`design-system.md` "Gezinme"). Yeni borçlandırma `+`tan gelir, düğmeler yalnız para hareketidir (K6) |
| Khatabook | Her harekette yürüyen bakiye sütunu | Alınmadı | İşlem sonrası bakiye ayrıntı panelinde sunucudan gelir; satır başına bakiye listede sıralama (giriş anı / gün) karışıklığı yaratır ve ikinci bir hesap yolu olur |
| Splitwise | `Hatırlat` düğmesi | Sonra | `02-TASARIM.md` §8 |

### 1.3 Tahsilat / ödeme paneli

| Başvuru | Kalıp | Karar | Gerekçe |
|---|---|---|---|
| Apple Card ödeme ekranı | Önerilen tutarlar (`Tamamı`, `Asgari`) tek dokunuşla | **Alındı** | `Tamamı`, `Gecikmiş` (sunucudan) |
| Khatabook `Aldım` | Tutar önce, sonra gün ve not; tek adım | **Alındı** | Tek panel, tek adım (§6.3) |
| Apple HIG — veri girişi | Ön değerle doldur, yazdırmak yerine seçtir, hatayı yazarken söyle | **Alındı** | Gün bugün, yer son kullanılan; fazla tutar uyarısı alanın altında |

### 1.4 Yeni kayıt

| Başvuru | Kalıp | Karar | Gerekçe |
|---|---|---|---|
| Wallet (BudgetBakers) | `Borç verdim` / `Borç aldım` cümle biçiminde seçenek | **Alındı** | `Ne oldu?` paneli; seçenekler kullanıcının ağzından |
| Apple HIG — modallık | Kısa, tek amaçlı modal; modalın üstüne modal açma | **Alındı** | Seçim paneli kapanır, form tam sayfa açılır |
| Ön muhasebe formları | Bütün alanlar baştan açık | Alınmadı | İsteğe bağlı bloklar (`Peşin / avans`, `Taksitli`, `Uygulamadan önce`) kapalı satır olarak durur, açılınca genişler |

### 1.5 Kayıt sayfası

| Başvuru | Kalıp | Karar | Gerekçe |
|---|---|---|---|
| Apple Card taksitleri | Toplam, ödenen, kalan; ilerleme çubuğu; sıradaki taksit | **Alındı** | Koyu özet kartında |
| Banka kredi ayrıntısı | Ödeme planı tablosunun tamamı | Kısaltıldı | K18: sıradaki üç taksit, gerisi `Tümünü göster` |

## 2. Ortak dil

- **Sayfa iskeleti:** `AppPageHeader` (geri oklu, 22/600), gövde
  `AppPageInsets.list`, kartlar arası `AppGaps.card`, bölüm arası
  `AppGaps.section`, bölüm başlığı `AppSectionHeader`.
- **Satır** (liste, kişi sayfasındaki kayıtlar): Planlananlar satırının dili.
  Baş 36 dp (`AppSizes.rowButton`): kişide baş harfli daire (`cardMuted`
  zemin, `ink` 13/600), kayıtta ikon kapsülü (`AppIconCapsule`, taksitli
  `event_repeat`, kişisiz `receipt_long`). Başlık `titleSmall` tek satır,
  tutar 16/700 renksiz; alt satır 13 soluk. Bir bölümün satırları tek kartta,
  ayırıcı yazının başladığı yerden.
- **Renk:** tutarlar renksizdir; taraf sekmeden, ikon ve etiketten okunur
  (`call_received` alacak, `call_made` borç — işlem ayrıntısıyla aynı
  ikonlar). Kırmızı yalnız gecikmedir ve her zaman `gecikmiş` kelimesiyle
  yazılır.
- **Koyu özet kartı** (kişi ve kayıt sayfası): plan ayrıntısının kartı
  (`surfaces.ink`, `AppRadius.card`, 16 iç boşluk). Üstündeki yazı
  `onInverseSurface`; ikincil yazı %80. Gecikme kırmızısı koyu zeminde
  zıt temanın gider rengidir (aydınlık temada `AppFinanceColors.dark.expense`).
- **Tutar biçimi:** `MoneyText.format`, iki basamak.

## 3. Ekranlar

### 3.1 `Borç ve alacak` (liste)

**İlk bakış:** iki toplam ve varsa gecikmiş. **İkinci bakış:** gecikenler,
kişiler.

```
‹  Borç ve alacak                 🔍  +
┌─ Toplam ─────────────── Hepsi ⌄ ─┐   (yalnız kapsam görünürse başlık şeridi)
│ ↙ Alacağınız     │ ↗ Borcunuz     │
│ ₺16.000,00       │ ₺4.700,00      │   22/700
│ ⏱ ₺2.000 gecikmiş│                │   13, gider rengi
└──────────────────────────────────┘
[ Alacaklar | Borçlar | Kişiler ]        AppSegmentRail
( Yalnız gecikenler )                     yalnız seçili tarafta gecikme varsa
Geciken
┌ AT  Ayşe Terzi              ₺3.500 ┐
│     ₺2.000 gecikmiş · İşletme      │
└────────────────────────────────────┘
Yaklaşan  30 gün içinde
┌ ...                                 ┐
Diğer
┌ ...                                 ┐
┌ Kapananlar                     3  › ┐
```

- **Başlık:** `Borç ve alacak`, eylemler `Ara` (search) ve `Borç veya alacak
  ekle` (`+`). İlk kullanımda `+` gizlenir (ekleme ortadaki düğmede).
- **Özet kartı:** iki eşit sütun, arada ince dikey çizgi. Etiket 13 soluk +
  yön ikonu (16), tutar 22/700 (`AppTextSizes.metric`), altında varsa
  `⏱ ₺… gecikmiş` (13/600, gider rengi). Kapsam görünen kullanıcıda kartın
  üstünde Planlananlar'ın dönem kartındaki gri şerit: `Toplam` + kapsam menüsü
  (`Hepsi ⌄`; uygulamanın genel kapsamını değiştirir, Planlananlar'daki gibi).
  Kapsam görünmeyen kullanıcıda şerit yoktur.
- **Sekmeler:** `AppSegmentRail` (`Alacaklar · Borçlar · Kişiler`), özetin
  altında `AppGaps.block`. Sekme değişince kaydırma başa döner.
- **`Yalnız gecikenler`:** tek `FilterChip`, sayfanın kenar payında, üstünde
  ve altında `AppGaps.notice` (`AppFilterStrip` kullanılmaz: o şerit kenardan
  kenara kayan çipler içindir ve tek çipi 16 dp içeri itiyordu). Seçili
  sekmenin tarafında gecikmiş tutar yoksa çizilmez. Açıkken yalnız `Geciken`
  bölümü kalır.
- **Bölümler:** `Geciken`, `Yaklaşan` (yanında soluk `30 gün içinde`),
  `Diğer`. Boş bölüm çizilmez.
- **Kişi satırı alt yazısı** (`·` ile; `Hepsi`de **sonda** kapsam, Planlananlar
  satırındaki gibi): gecikmiş varsa `₺2.000 gecikmiş` (gider rengi, 600);
  yoksa `Vade 30 Kasım` (sıradaki vadenin tutarı kalandan küçükse `Vade 30
  Kasım · ₺1.500`); vade yoksa `Vadesiz`. Tutarın tamamı avanssa `Alınan
  avans` / `Verilen avans`.
- **Kayıt satırı:** başlık açıklama, yoksa kişi; alt yazı `3/12 taksit ·
  sıradaki 15 Aralık` (kişisizde `Vade 15 Aralık` ya da `Vadesiz`); gecikmişse
  `₺500 gecikmiş` başa gelir. Ay her satırda uzun yazılır (`Aralık`).
- **`Kişiler` sekmesi:** bütün kişiler ada göre, tek kart. Satır öbür
  satırlarla aynı iki katlıdır: sağ üstte tutar, onun altında (alt yazı
  satırının sağında) soluk `size borçlu` / `borçlusunuz`. İki yönlü kişide
  ikinci taraf satırın en altında sağa yaslı tek satırdır (`₺400,00
  borçlusunuz`, tutar 700). Bakiyesiz kişide sağda soluk `Hesap kapalı`. Pasif kişi bu sekmede yoktur, yalnız aramada `Pasif`
  etiketiyle.
- **Liste sonu:** seçili tarafta kapanan kayıt varsa `Kapananlar` satırı
  (56 dp kart satırı, sağda sayı ve `›`; Tekrarlayanlar'daki `Duraklatılan ve
  bitenler` satırının aynısı).
- **Arama:** başlıktaki ikon başlığı arama alanına çevirir (Tekrarlayanlar'ın
  arama alanı: `cardMuted`, 48 dp, `AppRadius.field`); özet ve sekmeler
  gizlenir, sonuç tek kartta kişiler ve kayıtlardır. `✕` aramayı kapatır.

**Durumlar:**

| Durum | Görünüm |
|---|---|
| Yükleniyor | `AppLoadingView` |
| İlk kullanım (hiç kişi ve kayıt yok) | `AppFirstUseView` (`rows`), `Henüz borç ya da alacak yok` · `Veresiye, borç para ya da krediyi ekleyin.` · `Borç veya alacak ekle`; özet ve sekmeler çizilmez |
| Seçili tarafta açık tutar yok | `AppSectionEmpty`: `Açık alacağınız yok.` / `Açık borcunuz yok.`; `Kapananlar` satırı varsa altında kalır |
| Arama boş | `AppNoResultsView` (`search`): `“…” için sonuç yok` · `Aramayı temizle` |
| Hata | `AppErrorView` + `Tekrar dene` |
| Oturum düştü | `AppUnauthorizedView` |
| Eski veri (yenileme başarısız) | Liste yerinde kalır, üstte Planlananlar'ın yenileme uyarısı |

**Erişilebilirlik:** özet her sütunu tek cümle okur (`Alacağınız 16.000 lira,
2.000 lirası gecikmiş`). Satır tek düğmedir: ad, kapsam, tutar, durum. Sekme
seçili olduğunu söyler (`AppSegmentRail`).

### 3.2 Kişi sayfası

**İlk bakış:** kişinin bakiyesi ve gecikmiş. **İkinci:** eylem. **Üçüncü:**
taksitli kayıtlar ve hareketler.

```
‹  Ayşe Terzi                     +  ⋮
┌──────────────────────────────────┐   koyu kart
│ İşletme · cari hesap             │   13, %80
│ Alacağınız                       │   15/600
│ ₺8.000,00                        │   28/700
│ ⏱ ₺2.000 gecikmiş · Vade 30 Kas  │   13
│ Taksitli kayıtlar dahil          │   13, %80 (yalnız kayıt varsa)
└──────────────────────────────────┘
[        Tahsilat        ]               48 dp dolu düğme
Taksitli kayıtlar
┌ ⟳  Mobilya takımı           ₺4.500 ┐
│     3/12 taksit · sıradaki 15 Ara  │
└────────────────────────────────────┘
Hareketler                      Filtre
┌ ActivityTile × n                    ┐
```

- **Başlık:** kişinin adı (uzunsa iki satır), eylemler `+` (yeni kayıt, kişi
  dolu) ve `⋮` (`Düzenle`, `Açılış bakiyesi`, `Pasife al`).
- **Koyu özet kartı:** üst satır kişinin türü (`İşletme · cari hesap`,
  `Şahsi`; K12, kapsam görünmeyen kullanıcıda satır yok). Bir taraf açıksa:
  etiket 15/600, tutar 28/700 (`AppTextSizes.amount`), altında gecikmiş ve
  sıradaki vade. İki taraf açıksa iki eşit sütun, tutarlar 22/700. Hiçbiri
  açık değilse `Hesap kapalı` (22/700) ve `Bakiye yok`. Taksitli kayıt varsa
  son satır `Taksitli kayıtlar dahil`.
- **Eylemler:** yalnız açık taraf: `Tahsilat` (alacak) ve/veya `Ödeme`
  (borç); tam genişlik 48 dp dolu düğme, ikisi varsa iki eşit dolu düğme (aynı
  eylemin iki yönü; `design-system.md` "Bir ekrandaki eylem grupları"). Hesap
  kapalıysa düğme yoktur; avans `+`tan yazılır. Pasif kişide `+` yeni
  borçlandırma seçeneklerini kapatır, düğmeler açık kalır ve kartın altında
  `AppInlineNotice`: `Pasif kişi. Yeni satış ve alım yazılmaz; tahsilat ve
  ödeme yazılır.`
- **Taksitli kayıtlar:** listenin kayıt satırı; altında 4 dp ilerleme çizgisi
  yok (kayıt sayfasında var).
- **Hareketler:** `AppSectionHeader` + sağda altı çizili `Filtre`
  (`AppTextAction.underlined`). Filtre paneli `AppFilterSheet`: `Dönem` (Tümü /
  ay), `Tür` (`Hepsi · Satış ve alım · Tahsilat ve ödeme`), iki yönlü kişide
  `Taraf` (`Hepsi · Alacak · Borç`). Seçili filtre listenin üstünde
  kaldırılabilir çiptir. Satırlar ortak `ActivityTile`, tek kartta, tarih alt
  yazıda (kredi kartı sayfasının `Hareketler` bölümü gibi; gün başlığı bu
  kart dilinde ağır duruyordu), en yeni üstte, kaydırdıkça yüklenir. **Başlık
  kişinin adını tekrar etmez:** sunucunun başlığı kişinin adıysa yerine
  kaydın kısa türü yazılır (`Tahsilat`, `Ödeme`, `Veresiye satış`, `Vadeli
  alım`; `personMovementTitle`).

**Sapma:** K15 "ay seçici" diyordu; hareketler varsayılan olarak **tüm
zamanları** gösterir, ay filtrenin bir seçeneğidir. Gerekçe: kişide hareket
seyrek; ay seçiciyle açılan sayfa çoğu zaman boş ay gösterir.

**Durumlar:** yükleniyor (kart yerinde iskelet yok, `AppLoadingView`); hata;
hareket yok (`AppSectionEmpty`: `Henüz hareket yok.`); filtre boş
(`AppNoResultsView` `filter`, `Filtreyi temizle`); kişi bulunamadı
(`AppErrorView`: `Kişi artık bulunamıyor.`).

**Erişilebilirlik:** koyu kart tek cümle (`Ayşe Terzi. Alacağınız 8.000 lira,
2.000 lirası gecikmiş. Sıradaki vade 30 Kasım.`).

### 3.3 Tahsilat / ödeme paneli

`AppFormSheet`. Başlık `Tahsilat` / `Ödeme`, başlığın altında soluk tek satır
`Ayşe Terzi · alacağınız ₺3.500` (yalnız hesabın tutarı; kısayolla aynı
sayı).

1. `Tutar` (`TurkishAmountInputFormatter`). Altında çipler: `Tamamı ₺3.500`,
   varsa `Gecikmiş ₺2.000` (`ActionChip`; seçilen çip tutarı yazar).
2. Tutar hesabı aşıyorsa alanın hemen altında tek satır soluk not (ikon
   `info_outline`, kutu değil): `Kalandan fazla; fazlası kişinin sizdeki
   alacağı olur.` (ödemede `… sizin alacağınız olur.`). Kayıt engellenmez.
   Not fark tutarını yazmaz: tutar ile kısayol yalnız **karşılaştırılır**,
   istemci çıkarma yapmaz (kalan zaten başlığın altında ve `Tamamı` çipinde).
3. `Gün` (`AppDateField`, `lastDate` bugün).
4. `Hesap` (`AppAccountField`): tahsilatta hesaplar ve `POS` grubu, ödemede
   hesaplar ve `Kredi kartı` grubu. Ön değer kişiyle son kullanılan, yoksa
   varsayılan hesap.
5. `Açıklama (isteğe bağlı)`.
6. Yalnız ödemede ve tutar kalandan küçükken: `Kalanı indirim say` satırı
   (başlık, altında soluk `Kalan tutar alım indirimi olarak kapanır.`, sağda
   anahtar; tutar yazılmaz, aynı gerekçe).

Eylem satırı `Vazgeç` · `Kaydet`. Ret `AppFormSheet.errorMessage`.

### 3.4 Yeni kayıt

**`Ne oldu?` paneli** (`AppAdaptiveSheet`, ayrıntı paneli biçimi): beş satır,
her biri ikon kapsülü + başlık + soluk alt yazı (`02-TASARIM.md` §6.4
tablosu). Kişi sayfasından açılınca başlığın altında kişinin adı. Pasif kişide
satış, alım ve borç para satırları soluk (%45) ve dokunulmaz; alt yazının
yerinde `Pasif kişiye yeni kayıt yazılmaz`. İkonlar ayrıdır: satış
`sell_outlined`, alım `shopping_bag_outlined`, borç verme
`volunteer_activism_outlined`, borç alma `account_balance_outlined`, avans
`swap_horiz` (ilk denemede aynı taraftaki iki seçenek aynı ok ikonunu
taşıyordu).

**Form** (tam sayfa, alt çubuk yok): başlık seçilen satırın adı. Alanlar
`AppFormField` aralığıyla: avansta en üstte `Aldım · Verdim` rayı; `Kişi`
(baş harf dairesi ve ad; kişi sayfasından açılınca kilitli, okusuz), `Tutar`,
`Gün`, `Kategori` (satış/alım; kapsamı `AppCategoryField` kendisi gösterir ve
iki kapsama açık kategoride sorar) ya da `Giren hesap` / `Çıkan hesap` (borç
para, avans), `Vade (isteğe bağlı)` (taksitli değilse), `Açıklama (isteğe
bağlı)`. Altında tek kartta üç **kapalı satır** (en az 56 dp; başlık, soluk
ipucu, sağda anahtar; satırın tamamı dokunulur):

| Satır | Açılınca |
|---|---|
| `Peşin / avans alındı` (satış) · `Peşin ödendi` (alım) — ipucu `Bugün alınan kısım` / `Bugün ödenen kısım` | `Alınan tutar` / `Ödenen tutar`, `Giren hesap` / `Çıkan hesap`; satışta altında tek satır not: `Satış bugünün geliri olur. İş bitince yazmak isterseniz yalnız avansı kaydedin.` (uygulamanın "siz" diliyle; §7a'daki taslak "sen" diliydi) |
| `Taksitli` — `Kendi takvimiyle takip edilir` | `Taksit sayısı`, (borç parada) `Taksit tutarı` (K17: bankanın tutarı yazılır), `İlk taksit günü`; altında gri kutuda sunucunun takvim önizlemesi: ilk üç taksit ve `12 taksit · son taksit 23 Kasım 2027` |
| `Uygulamadan önce oluştu` — `Yalnız kalan tutar yazılır` | Tutar `Kalan tutar`, gün `İtibarıyla` olur; hesap ve kategori alanları ile `Peşin` satırı kalkar; not: `Hesap bakiyesi ve raporlar değişmez.` |

`Kaydet` formun sonunda, tam genişlik 48 dp. Kişi seçicisi (arama,
`"…" kişisini ekle`, yeni kişinin türü) Faz 7'de bağlanır; karede alanın
kapalı hâli çizildi.

### 3.5 Kayıt sayfası (taksitli ya da kişisiz)

```
‹  Mobilya takımı                    ⋮
┌──────────────────────────────────┐   koyu kart
│ Ayşe Terzi ›          İşletme    │
│ Kalan                            │
│ ₺4.500,00                        │   28/700
│ ███████░░░░░  3/12 taksit ödendi │   6 dp çubuk
│ Sıradaki 15 Aralık · ₺500        │
└──────────────────────────────────┘
[        Tahsilat kaydet        ]
Takvim
┌ [15 Ara] 4. taksit         ₺500  ┐  sıradaki üç; takvim yaprağı
│ [15 Oca] 5. taksit         ₺500  │
│ [15 Şub] 6. taksit         ₺500  │
│ Tümünü göster        12 taksit › │
└──────────────────────────────────┘
Tahsilatlar                            (borçta `Ödemeler`)
┌ [14 Kas] Tahsilat          ₺500  ┐  alt yazıda paranın yeri;
│          Ana POS                 │  dokununca ayrıntı ve geri al
```

- Kişi adı kartın içinde dokunulur bağlantıdır (kişi sayfası); kapsam aynı
  satırın sağında. Kişisiz kayıtta kapsam satırın başındadır.
- Çubuk `incomeFill`/`expenseFill` değil kartın yazı rengindedir (iz %20);
  ödenmişlik bir gelir ya da gider değildir. Oran çizimdir; altında
  `3/12 taksit ödendi` yazar.
- Sıradaki taksit gecikmişse satır `4. taksit gecikti · 15 Kasım` olur
  (koyu zeminde zıt temanın gider rengi). Kişisiz tek ödemede `Vade 10 Ocak`
  ya da `Vadesi geçti · …`; takvim bölümü çizilmez.
- Takvim ve ödeme satırları ortak takvim yaprağıyla (`AppDateLeaf`) başlar;
  alt yazısız satırda yaprak ve başlık ortalanır. Gecikmiş taksitin yaprağı
  gider tonundadır, alt yazısı `Gecikti`; kısmen ödenmiş taksitte `₺200,00
  ödendi` (sunucunun `paidAmount`'u).
- Faizli kayıtta kart `Kalan anapara` satırını da taşır; düğme `Taksit öde`.
  **`Kalan anapara` bugün sunucu cevabında yok** (`remainingPrincipal`):
  istemci taksitlerden toplamaz, alan Faz 7'de sunucuya eklenir; o zamana
  kadar satır çizilmez. Ödenen tutar da aynı nedenle yazılmaz; sayı olarak
  yalnız taksit adedi yazar.
- `⋮`: `Planı değiştir`, `İndirim`, `Kaydı iptal et` (hata renginde).
- Kalan sıfırsa kartta `Kapandı`, düğme çizilmez.

## 4. Ortak kuraldan ayrılanlar

| Yer | Kural | Seçilen | Gerekçe |
|---|---|---|---|
| — | — | — | Bu turda yok; çıkarsa buraya yazılır |

## 5. Denetim bulguları

Yöntem `03-PLAN.md` Faz 4: kareler dosyadan açılır, her ekran denetim
listesinden geçer (tek ana eylem, ilk bakışta en önemli sayı, tekrar eden
bilgi, tek kelime taşması, renk tek başına anlam, boş ve hata durumu, doğal
Türkçe). Önem: **kusur** (yanlış ya da okunmaz), **zayıf** (doğru ama daha iyi
olabilir), **tercih**.

| Tur | Ekran | Bulgu | Önem | Sonuç |
|---|---|---|---|---|
| 1 | Liste | Kişiler sekmesinde tutar ve tarafın adı alt alta, alt yazı onların altına itiliyor: satırlar üç-dört kat, iki yönlü kişi beş kat | kusur | Düzeltildi: taraf adı alt yazı satırının sağında, ikinci taraf tek satır |
| 1 | Liste | Kapsam bazı satırlarda başta, gecikmeli satırda gecikmeden sonra: aynı listede sıra değişiyor | zayıf | Düzeltildi: kapsam her satırda sonda |
| 1 | Liste | `Yalnız gecikenler` çipi içerikten 16 dp içeride (`AppFilterStrip` kendi kenar payını ekliyor) | kusur | Düzeltildi: çip kenar payında, üstte ve altta 8 |
| 1 | Liste | Taksitte `15 Ara`, kişide `18 Aralık`: ay iki biçimde | zayıf | Düzeltildi: uzun ay |
| 1 | Kişi sayfası | Hareketlerde başlık kişinin adı (`Ayşe Terzi`), sayfanın başlığını tekrar ediyor | kusur | Düzeltildi: kaydın kısa türü |
| 1 | Panel | Fazla tutar notu iki satırlık mavi kutu; formun en baskın öğesi oluyor | zayıf | Düzeltildi: tek satır soluk not, tutar yazmaz |
| 1 | Panel | `Kalanı indirim say` `SwitchListTile`: başlığı test motorunda fontsuz çiziliyor (cihazda değil), satır form dilinden ayrı | zayıf | Düzeltildi: alanlarla aynı yazı kademesiyle kurulan satır |
| 1 | Hepsi | Düğmeler ve başlık eylemleri soluk: karede geri çağırma verilmemişti | — | Kare hatası; düzeltildi |
| 2 | Liste, 360 dp | `₺1.250.700,00` sütuna sığmak için küçülüyor (`FittedBox`); iki sütunun tutarı farklı boyda | tercih | Kaldı: yedi haneli borçta tutar kırpılmadan tek çözüm |
| 2 | Kişi sayfası, karanlık | Koyu kart karanlık temada açık renge döner ve altındaki dolu düğmeyle aynı tonda durur | tercih | Kaldı: plan ayrıntısının kartıyla aynı davranış; ayrı yüzey rengi kararı kredi kartı yüzünün açık işiyle birlikte (`design-system.md` "Kredi kartları sayfası") |
| 2 | Hepsi | Büyük yazı (1.3–2.0) bu turda ölçülmedi | — | Faz 5–6'nın taşma testlerinde (`03-PLAN.md`) |

**İkinci turun sonucu (23 Kasım):** kusur yok. Kareler `412/`, `360/`,
`412-karanlik/`.

Yeni kayıt ve kayıt sayfası (aynı gün, ikinci bölüm):

| Tur | Ekran | Bulgu | Önem | Sonuç |
|---|---|---|---|---|
| 1 | Kayıt sayfası | Takvim satırında 48 dp yaprak, tek satırlık başlık üstte: satırın altı boş kalıyor | kusur | Düzeltildi: alt yazısız satırda baş ve başlık ortalanır |
| 1 | Kayıt sayfası | Kartın üst satırında kapsam ortada kalıyor (iki `Spacer`) | kusur | Düzeltildi: kişi solda, kapsam sağda |
| 1 | Kayıt sayfası, liste, kişi sayfası | `AppSectionEmpty` zaten kart çiziyor; dışına bir kart daha sarılmış, iç içe iki kenar | kusur | Düzeltildi (üç yerde) |
| 1 | `Ne oldu?` | Satışla borç verme, alımla borç alma aynı ok ikonunu taşıyor | zayıf | Düzeltildi: beş ayrı ikon |
| 1 | Form | Satış notu "sen" diliyle (`istersen … kaydet`), uygulamanın geri kalanı "siz" | kusur | Düzeltildi |
| 1 | Form | Borç alırken taksitli açılınca taksit tutarı sorulmuyor (K17) | kusur | Düzeltildi: `Taksit tutarı` alanı |
| 2 | Kayıt sayfası | Kişisiz kayıtta kapsam etiketi sağda yalnız duruyor | zayıf | Düzeltildi: satırın başında |
| 2 | Form | `Uygulamadan önce` notu ipucunu tekrar ediyor | zayıf | Düzeltildi: `Hesap bakiyesi ve raporlar değişmez.` |
| 2 | Form | Kilitli kişi alanı (baş harf dairesiyle) öbür alanlardan 16 dp yüksek | tercih | Kaldı: daire kişiyi listedeki satırla aynı tanıtır |

**Faz 4'ün sonucu:** beş ekranın tarifi yazılı, 20 kare × 3 çıktı, iki turun
sonunda kusur yok. Kullanıcı ekranlara emülatörde, sunucuya bağlandıktan
sonra bakacak (23 Kasım kararı); K15 sapması da o incelemede değerlendirilir.

