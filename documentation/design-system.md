# Tasarım Sistemi

Bu belge uygulamanın görsel ve etkileşim kurallarını tanımlar. Amacı bir stil
kılavuzu olmaktan çok, **yeni ekran eklerken hangi kararların zaten verilmiş
olduğunu** göstermektir: renk, boşluk, kırılım noktası ve erişilebilirlik
kararları ekran başına yeniden verilmez.

Temel Material 3'tür. Özel bir font paketi yoktur.

**Marka rengi akromatiktir** (`#16191D`; karanlık temada `#EDEFF2`). Bu estetik
değil yapısal bir karardır: renkli bir marka rengi, gelir yeşili ve gider
kırmızısıyla birlikte ekranda üç ayrı doygun renk ailesi oluşturuyor ve
hiçbiri öne çıkmıyordu. Akromatik markayla ekranda **anlamlı renk yalnız
ikidir** — yeşil gelir, kırmızı gider — ve renk yalnız bilgi taşır
(bkz. `adr/0007-achromatic-brand-and-own-surfaces.md`).

Buton, kayan eylem butonu ve seçili durum gibi vurgular bu akromatik renkle
boyanır; para göstergeleri dışında renk kullanılmaz.

## Token'lar

Hepsi `lib/core/theme/` altındadır.

### Boşluk — `AppSpacing`

| Ad | Değer | Kullanım |
|---|---|---|
| `xxSmall` | 2 | Başlık ile alt satırı arası (tasarımda `gap: 2`) |
| `xSmall` | 4 | Satır içi küçük ayrım |
| `small` | 8 | Liste öğeleri arası, chip iç boşluğu |
| `medium` | 16 | Sayfa ve kart iç boşluğu, bölüm arası |
| `large` | 24 | Bölüm grubu ayrımı |
| `xLarge` | 32 | Büyük ayrım |
| `xxLarge` | 48 | Boş durum çevresi |
| `fabClearance` | 96 | Kaydırılabilir listenin altına bırakılan pay |

`fabClearance` ölçeğin bir adımı değildir; son satırın kayan eylem butonunun
altında kalmaması için tek amaçlı bir yerleşim sabitidir.

Ölçekte olmayan bir değere ihtiyaç duyduğunuzu düşünüyorsanız önce yakın adıma
yuvarlayın. Ölçeği büyütmek, her ekranın kendi ara değerini uydurmasına giden
ilk adımdır.

### Köşe yarıçapı — `AppRadius`

| Ad | Değer | Kullanım |
|---|---|---|
| `card` | 20 | Kart, panel, liste kutusu |
| `field` | 16 | Form alanı, buton yüzeyi |
| `sheet` | 28 | Bottom sheet ve dialog |
| `chip` | 999 | Rozet, etiket |

### Pencere sınıfı — `AppBreakpoints`

Material 3 pencere boyut sınıfları:

```text
< 600 dp    compact    telefon dikey
600–839 dp  medium     telefon yatay, küçük tablet
>= 840 dp   expanded   tablet ve üzeri
```

`contentMaxWidth` = 720 dp: ray açıldığında sayfa içeriğinin sınırı.

Genişlik **doğrudan karşılaştırılmaz**. `context.windowSize` okunur; yapısal
test `maxWidth >= <sayı>` biçimindeki karşılaştırmaları reddeder. Bunun nedeni
somut: shell 720, planlama 700 kullanıyordu ve aynı kavramın iki eşiği
ekranları birbirinden farklı davrandırıyordu.

### Yüzeyler — `AppSurfaces`

| Token | Kullanım |
|---|---|
| `canvas` | Sayfa zemini |
| `card` | Kart, panel, liste yüzeyi |
| `cardMuted` | Kart içi ikincil bölge: ikon kapsülü, girdi alanı, ilerleme yolu |
| `border` | Kartı zeminden ayıran ince kenarlık |
| `borderStrong` | Seçili kart, odaklı girdi |
| `overlay` | Yüzen katmanların gölgesi |

**Kart zeminden kenarlıkla ayrılır, gölgeyle değil.** Gölge yalnız gerçekten
yüzen katmanlara ayrılmıştır (`AppElevation`):

| Ad | Değer | Nerede |
|---|---|---|
| `flat` | 0 | Kart, panel, liste |
| `raised` | 3 | Kayan eylem butonu |
| `floating` | 6 | Bottom sheet, dialog |

Her kart gölge taşısaydı derinlik bilgisi anlamını yitirirdi: her şey
yüzüyorsa hiçbir şey yüzmüyordur.

### Mürekkep merdiveni — `AppSurfaces.ink*`

Bir ekranda üç okuma kademesi vardır ve hepsi birden koyu olamaz.

| Token | Nerede | Aydınlık | Karanlık |
|---|---|---|---|
| `ink` | satır başlığı, tutar, ekran başlığı | `#16191D` | `#EDEFF2` |
| `inkMuted` | tarih, kaynak, açıklama, alt satır | `#5C6572` | `#A5ADB8` |
| `inkFaint` | bölüm etiketi, yer tutucu, sayaç | `#646D7A` | `#8E97A3` |

`colorScheme.onSurface` ve `onSurfaceVariant` bu paletten verilir. Önceki
sürümde `ColorScheme.fromSeed`'in ürettikleri kullanılıyordu; akromatik seed
ile ikisi de koyu çıkıyor, aralarında okunabilir bir kademe farkı olmuyor ve
ekran baştan sona siyah okunuyordu.

**Soluk olmak silik olmak değildir:** üç kademe de AA normal metin eşiğini
geçer (en soluğu 4,63:1). Kademelerin gerçekten ayrıştığı testle sabitlenir —
en güçlü ile en soluk arasında en az iki kat kontrast farkı olmalı.

Renkli zeminde (tintli metrik kutusu) bu merdiven **kullanılmaz**; orada
`on*Container` geçerlidir, çünkü merdiven nötr yüzeylere göre ölçülmüştür.

### Finansal renkler — `AppFinanceColors`

Her rolün **tam olarak iki tonu** vardır ve ikisi de adı konmuş token'dır
(bkz. ADR 0008). Ton seçimi tesadüf değildir; hangisinin nerede kullanılacağı
adından okunur.

| Token | Nerede | Eşik |
|---|---|---|
| `income` / `expense` / `neutral` | metin ve ikon | 4.5:1 |
| `incomeFill` / `expenseFill` / `neutralFill` | halka dilimi, çubuk, ilerleme dolgusu | 3:1 |
| `*Container` + `on*Container` | rozet, ikon kapsülü, tintli kutu zemini | çift olarak 4.5:1 |
| `planned`, `cancelled` (+ çiftleri) | plan ve iptal durumları | 4.5:1 |
| `categorySlices` | kategori dağılımı dilimleri | 3:1 |

Aydınlık palet (karanlık palet ayrıdır, aynı değerler koyu yüzeyde okunmaz):

| Rol | Metin | Dolgu |
|---|---|---|
| Gelir | `#137D3F` | `#209E54` |
| Gider | `#CF2E1F` | `#E5544A` |
| Nötr | `#0B6AD6` | `#2589FA` |

Metin tonları eşiğin **hemen üstünde** seçilmiştir; daha koyu bir ton daha
güvenli değil, yalnızca daha ölüdür.

Üç kural:

1. **Alfa'dan ton türetilmez.** `income.withValues(alpha: 0.12)` gibi bir
   kapsül zemini, altındaki yüzey değiştikçe farklı bir renge dönüşür ve
   hiçbir yerde ölçülmüş bir değer olmaz. Kapsül `*Container`, üstündeki
   ikon/tutar `on*Container` kullanır.
2. **Marka rengi dolgu değildir.** `colorScheme.primary` akromatiktir ve bir
   ilerleme şeridinde veya grafik sütununda siyah kütleye dönüşür. Marka
   rengi buton ve seçili sekme gibi **eylem** yüzeylerinde kalır.
3. **Nötr rol mavidir, mor değil.** Mor, gelir yeşili ve gider kırmızısıyla
   aynı ailede olmayan üçüncü bir doygun hue açıyordu.

Renkli zeminde gösterilen tutar `AppMoneyText(onContainer: true)` ister:
yüzeye göre seçilmiş rol tonu kendi container'ı üzerinde 3,95:1'e düşer.

### Tipografi — `AppTypography`

Metin ölçeği ürün rollerine göre tanımlıdır; ekranlar ham `textTheme` seçimi
yapmaz.

| Rol | Stil | Nerede |
|---|---|---|
| Hero metrik | `displaySmall` 36/700 | Ekranın söylediği tek sayı |
| Ekran başlığı | `headlineSmall` 24/700; ana ekranlarda `AppPageHeader` 26/700 (−0,6) | Sayfa başlığı |
| Kart metriği | `titleLarge` 22/700 | Kart içi tutar |
| Bölüm başlığı | `titleMedium` 18/600 | `AppSectionHeader` |
| Satır başlığı | `titleSmall` 16/600 | Liste satırı |
| Gövde | `bodyLarge/Medium` 16/14 | Açıklama metni |
| Yardımcı | `bodySmall` 14 | Tarih, kaynak, alt satır |
| Etiket | `labelMedium/Small` 13/13, harf aralıklı | Metrik etiketi, rozet, alt çubuk |

Ölçek 27 Eylül 2026'da Claude Design teslim paketinin token'larıyla
(`tokens/typography.css`) birebir eşitlendi; önceki ölçek bir kademe
küçüktü.

`AppMoneyText` tasarım sisteminin para ölçeğini `size` ile alır: `hero`
(36/700), `metric` (22/600), `row` (16/600), `body` (14/400). İptal edilmiş
tutar soluk renkte ve **üstü çizili** yazılır.

Para için iki yardımcı vardır: `AppTypography.money` (sabit genişlikli rakam)
ve `heroMoney` (ek ağırlık ve sıkı harf aralığı). Orantılı rakamlarda tutar
sütunu kayar ve `1.000,00` ile `9.999,99` göz kaydırmadan karşılaştırılamaz.

Etiketler büyük harfe çevrilmez: Türkçede `i/İ` dönüşümü tuzaklıdır ve ekran
okuyucu büyük harfli metni farklı okuyabilir. Ayrım harf aralığı ve ağırlıkla
yapılır.

## Bileşenler

`lib/core/widgets/` altında.

| Bileşen | Ne yapar |
|---|---|
| `AppCard` | Tek kart yüzeyi: kenarlık, yarıçap, seçili durumu |
| `AppMetricTile` | Bir sayı ve ne olduğu; `standard` ve `hero` boyları; `tinted` ile rolünün zemini |
| `AppListRow` | İkon kapsülü + başlık + alt satır + sağ blok; `titleMaxLines` ile başlık satır sayısı |
| `AppMoneyText` | Tutarı biçimler, rengini etkiden alır, sabit genişlikli rakam kullanır, ekran okuyucuya cümle verir |
| `MoneyText` | Biçimlendirmenin kendisi: dört ondalıklı sunucu dizesini **iki** basamağa yuvarlar, binlik ayırır; `percent` oranlar, `editable` düzenlenebilir alana önceden doldurulan tutar için (yuvarlamaz, yalnız son sıfırları atar) |
| `DateText` | `yyyy-MM-dd` sunucu tarihini okunur hâle getirir: `dayMonth`, `dayMonthYear`, `monthYear`. Ay adlarının tek kaynağı |
| `AppStatusChip` | Durum rozeti; ikon **ve** etiket zorunlu; en az 32 dp yüksek |
| `AppRowAction` | Bir liste satırının kendi eylemi (`Öde`, `Tahsil et`, `Gerçekleştir`, `Sil`); çerçeveli, rozetle aynı yükseklikte |
| `AppSectionHeader` | Bölüm başlığı; ekran okuyucuya `header` olarak bildirilir |
| `AppMenuGroupLabel` | Açılır listedeki grup başlığı; seçeneklerden boyut, renk ve harf aralığıyla ayrılır |
| `AppSubmitButton` | Gönderim butonu; istek uçarken ikinci dokunuşu yutar |
| `AppConfirmDialog` | Geri alınması zor eylemden önce açık onay |
| `AppFormSheet` | Veri giren panellerin kabuğu: başlık, kaydırılan gövde, eylem satırı |
| `AppFormField` | Panel alanlarının ortak dikey aralığı |
| `AppDateField` | Tarih seçen form alanı; metin alanlarıyla aynı dekorasyonu kullanır |
| `AppContentWidth` | Geniş ekranda içeriği okunabilir genişlikle sınırlar |
| `AppAdaptiveSheet` | Telefonda bottom sheet, geniş ekranda ortalanmış dialog |
| `AppResponsiveGrid` | Sütun sayısını kartın en küçük okunabilir genişliğinden türetir |
| `AppScopeSwitch` | Uygulamanın tek kapsam anahtarı: `Hepsi · İşletme · Şahsi` |
| `AppScopeField` | Formdaki düzeltilebilir kapsam çipi; altında değerin nereden geldiği yazılı |
| `AppScopeSection` | Formun taraf bölümü: tek taraflı kategoride `AppScopeInfoRow`, iki tarafa açık kategoride `AppScopeField`. Formlar çipi doğrudan değil bunu kullanır |
| `AppScopeInfoRow` | Sorulmayan tarafın bilgi satırı: `Şahsi · kategoriden`. Taraf vurgulu, nedeni soluk; dokunulmaz, ekran okuyucu tek cümle okur |
| `AppLoadingView` / `AppErrorView` / `AppEmptyView` / `AppUnauthorizedView` | Ortak durum ekranları |
| `AppPageHeader` | Ana ekranların başlık satırı: 26/700 başlık, sağda 48 dp ikon eylemleri, isteğe bağlı geri oku; en az 64 dp + iç boşluk |
| `AppAvatar` | Hesabın avatarı: marka zemininde e-postanın baş harfleri; doğrulanmamış e-postada kırmızı nokta |
| `AppIconCapsule` | Satır başındaki yuvarlak kapsül; zemin rolün `*Container`, ikon `on*Container` tonu. Kapsüllü listelerde ayırıcı 72 dp'den başlar (`rowInset`) |
| `AppRow` | Kart içi satır: serbest baş (kapsül, tarih yaprağı), 16/600 başlık, iki satırlık alt metin, sağ öğe; büyük yazıda sağ öğe alta iner |
| `AppDividedColumn` | Satırlar arasına yazının başladığı yerden ayırıcı çizer |
| `AppStatusTag` | Dolgusuz durum etiketi: ikon 16 + metin 13/600, rolün metin renginde (Tuttu, Eksik, Yolda, Sayılmadı). Dolgulu `AppStatusChip` yalnız sayfadaki tek önemli durum için |
| `AppCardHead` | Kartın başlık şeridi: 52 dp, solda 15/600 başlık + gri meta, sağda durum, altında kenar çizgisi |
| `AppTextAction` | Bölüm başlığının sağındaki dolgusuz metin eylemi (`+ Ekle`, `Tümü ›`), 48 dp dokunma alanı |
| `AppDateLeaf` | Takvim yaprağı 44×48: gün 17/700, ay 11/600 |
| `AppDetailBlock` / `AppDetailRow` | Panel içindeki gri etiket–değer bloğu (işlem detayı, POS detayı) |
| `AppSegmentRail` | Tek parça seçim rayı: gri zemin, seçili dilim beyaz + güçlü kenar. `selected: null` seçimsizdir; kapsam anahtarı, kasa seçici ve gün sonu toplu cevabı bunu kullanır |

Birkaç bileşenin sözleşmesinde neden şu kararların olduğu:

- **`AppMoneyText` işaret koyarken nötr hareketi atlar.** Transfer ne kazanç ne
  kayıptır; işaret koymak onu gelir/gidere benzetirdi.
- **`AppStatusChip` ikonu ve etiketi zorunlu alan yapar.** "Hiçbir bilgi yalnız
  renkle taşınmaz" kuralı böylece hatırlanması gereken bir şey olmaktan çıkıp
  imzanın parçası olur.
- **`AppSubmitButton` kaçan hatayı bildirir.** `onPressed` senkrondur ve dönen
  future'ı kimse beklemez; sessizce yutulsa hata kaybolurdu. Kilit `finally`
  içinde açılır, çünkü bir ağ hatası butonu kalıcı olarak ölü bırakmamalı.
- **`AppConfirmDialog` `null`'ı asla onay saymaz.** Barrier'a dokunup kapatmak
  bir karar değildir.
- **`AppListRow` ikonu sabit boyutlu bir kapsüle koyar.** Farklı uzunluktaki
  başlıklar arasında sol kenar hizası böyle korunur.
- **`AppMetricTile` kendini tek cümlede duyurur.** Parçalar ayrı okunsaydı
  hangi sayının neye ait olduğu kaybolurdu.
- **Özet ekranının hero kartı** üstte işaretli gelir ve gider satırlarını,
  kalın bir çizginin altında ayın netini (hero boyu) taşır; anahtar `Hepsi`
  konumundaysa altındaki gri blokta `İşletme neti` ve şahsi taraf durur. Üç
  sayı da sunucudan gelir; ekran aralarında çıkarma yapmaz. Geçen ayla fark da
  sunucudandır (`periodComparison.netChange`). Şahsi tarafın etiketi
  her zaman `Şahsi net`tir; yön işaretten ve renkten okunur (tasarım teslimi,
  27 Eylül 2026 — eskiden eksi ayda `Şahsi çekim` yazıyordu). **"Kâr" kelimesi hiçbir ekranda kullanılmaz.**
- **`AppFormSheet` panelin kapanışını kendi üstlenir.** Panel içeriği
  `Navigator.pop` çağırmaz; sonucu döndürür, kabuk kapatır. `null` dönmek
  "doğrulama düştü, açık kal" demektir. Bu, kapanışın **tek** bir yerde
  olmasını sağlar ve aşağıdaki tuzağı ortadan kaldırır.
- **`AppFormSheet` üçüncü bir eylem taşıyabilir** (`secondaryLabel` /
  `onSecondary`). Filtre panelindeki `Temizle` bunun tek örneğidir:
  temizlemek vazgeçmekten farklıdır — vazgeçmek hiçbir şey değiştirmez,
  temizlemek boş filtreyi uygular.
- **Kapsam anahtarı tek parça bir raydır** (`AppSegmentRail`). Etiket büyük
  yazıda dilim içinde küçülür, ray taşmaz. Seçili dilim beyaz yüzey ve güçlü
  kenarla ayrılır ve ekran okuyucuya `seçili` olarak söylenir. Dilim 40 dp,
  dokunma alanı rayın iç boşluğunu da kapsar ve 48 dp olur.
- **Anahtar Özet ekranında, kaydırılan gövdenin dışındadır.** Uygulamanın tek
  kapsam denetimi odur ve yükleme, hata ya da boş durumda da yerinde durmalı;
  kullanıcı boş bir liste görüp anahtarı aramamalı. Bölünen diğer ekranlar
  aktif kapsamı **başlıklarında yazar**, denetimi kopyalamaz.
- **Bölünmeyen bölüm, bölünmediğini yazar.** Filtre açıkken sessizce aynı kalan
  bir sayı filtrelenmiş sanılır; net varlık ve hesap bakiyelerinin altında
  toplam gösterdiklerini söyleyen bir satır durur.
- **`AppDateField` kendi görünümünü tanımlamaz.** `InputDecorator` ile tema
  dekorasyonunu okur; tarih alanı yanındaki metin alanlarından ayrı bir tür
  gibi görünmemelidir. Etiketi **her zaman üstte** durur
  (`FloatingLabelBehavior.always`): değer yokken yer tutucu ("Seçilmedi")
  içerik yerinde çizilir ve etiket aşağı inseydi ikisi üst üste binerdi.

### Gezinme ve birincil eylem

- Telefon dikeyde alt çubuk **çentiklidir** (`BottomAppBar` +
  `CircularNotchedRectangle`) ve dört sekme ikiye ikiye ayrılır. Birincil
  eylem (`İşlem ekle`) çentiğe oturur; başparmağın doğal durduğu yerdir.
- Ortadaki buton **her sekmede kalır**. Sekmeye göre kaybolsaydı çentik boş
  bir oyuk olarak dururdu.
- Bu yüzden **alt çubuğun olduğu sekmelerde sayfaların kendi kayan butonu
  olmaz**; sayfaya özel ekleme eylemi başlık çubuğuna taşınır. İki kayan buton
  aynı ekranda birbiriyle yarışıyordu.
- Alt çubuğun olmadığı ekranlarda (Kategoriler, Hesaplar, …) sayfanın kendi
  kayan butonu sağ altta kalır: daire, ikon ve tooltip.
- Geniş ekranda çentik yoktur; birincil eylem gezinme rayının başında durur.
- **Üçten fazla segmentli bir seçicide etiket segmentin içinde durmaz.**
  Segmentler genişliği eşit paylaşır; `Komisyon yok · Tutar · Oran` üçlüsünde
  ilk etiket telefonda kelimenin ortasından bölünüyordu (`Komisyo / n yok`).
  Grubun adı `labelMedium` ile üstte durur (`Komisyon`) ve segmentler kısalır
  (`Yok · Tutar · Oran`) — kapsam seçicisiyle aynı desen. Cihaz kabulünde
  görüldü ve orada düzeltildi.
- **Birleşik feed POS'u iki satır olarak çizer**: `POS satışı` ve
  `POS yatışı`. İki etikette de `kart` kelimesi tek başına geçmez (ADR 0015):
  borçlandığın kart aynı listede yan yana görünüyor. İkonlar
  `point_of_sale_outlined` ve `move_to_inbox_outlined`; ikincisi paranın artık
  yolda olmadığını söyler. Yatış satırına dokununca genel ayrıntı yerine yatış
  ayrıntısı açılır.
- **İşlem satırının adı** (2 Ekim 2026, kullanıcı kararı). Üç kural:
  1. **Üst satır kaydın ne olduğunu söyler, hesap adı olmaz.** Açıklama
     yazıldıysa o; yoksa gelir/giderde kategori, kişiyle ilgili kayıtta kişi,
     para taşıyan kayıtta türün adı (`Transfer`, `Kart ödemesi`, `POS yatışı`).
     Sunucu bu türlerde boş başlık gönderir; adı istemci yazar.
  2. **Sol alt satır nerede olduğunu söyler ve üst satırı tekrar etmez**:
     kategori • POS adı • hesap ya da "kaynak → hedef". POS satışında hesap
     yazılmaz (ayrıntıdadır); yatışta birden çok tahsilat varsa `N tahsilat`.
  3. **Bir kaydın parçası ayrı satır olmaz.** Komisyon satışın, kesinti
     yatışın tutarının **altında** `bodySmall` ile yazılır (`komisyon ₺97,50`,
     `kesinti ₺14,00`). Sol alt satıra konmaz: orası taşardı. İptal edilmiş
     satırda tutarla birlikte üstü çizilir.
  Aynı günün satırları giriş sırasına göre dizilir, en yeni üstte.
- **İşlem ayrıntısı.** `Köken` satırı yoktur (kullanıcı kararı, 2 Ekim 2026:
  ayrıntı kalabalıktı; banka bağlantısı gelirse geri eklenir) — neden iptal
  edilemediğini alttaki kilit notu söyler. Para taşıyan harekette son satır
  **`Bakiye`**dir: o hareketten hemen sonraki hesap bakiyesi (kartta
  `Kart borcu`; transferde iki satır, değer `hesap · tutar`). **Sayının rengi
  paranın yönüdür** (kullanıcı kararı, 2 Ekim 2026): hesaba para girdiyse
  yeşil, çıktıysa kırmızı; kart borcunda ters (borç arttıysa kırmızı). POS
  satışında bakiye **mavidir**: satış gelir yazdı ama hesaba dokunmadı, para
  yolda. Yönü sunucu söyler (`change`), istemci türden türetmez. Sunucudan gelir
  ve panel açılırken bir kez istenir; cevap beklenirken satır `…` ile
  **yerinde durur**, panel sonradan büyümez. POS satışının ayrıntısı
  `POS / Komisyon / Net tutar / Hesap / Geçiş günü` (yoldaysa `Beklenen`)
  satırlarını taşır. Etiketler kısadır: `AppDetailRow` etiketi esnemez ve
  2.0× yazıda uzun etiket tutarı sıkıştırır.
- **İşlem ayrıntısında hesabın dışındaki "sonrası" satırları** (2 Ekim 2026,
  kullanıcı kararı). Üçü de düz yazılır; yeşil/kırmızı yalnız hesap
  bakiyesinde ve kart borcundadır ("alacağım arttı"nın iyi mi kötü mü olduğu
  belli değildir):
  - Kişinin açık bakiyesi — **etiket ve ikon tarafı söyler**: `Alacağın`
    (`call_received`) ya da `Borcun` (`call_made`), yanında tutar; açık tutar
    yoksa `Cari — Kapandı`. Veresiye/vadeli kayıt, tahsilat/ödeme ve karşı
    tarafı olan tek seferlik borç/alacakta. İlk teslimde satır
    `Cari — ₺100,00 borç` idi; kullanıcı emülatörde fazla tahsilat sonrası
    kimin kime borçlu olduğunu okuyamadı. Dört seçenek çizildi; seçilen bu.
  - **Taraf çevrildiyse tek satır açıklama** gri bloğun hemen altında,
    `swap_vert` ikonuyla: `Alacağın kapandı; ₺100,00 borcun var.` (tersi:
    `Borcun kapandı; … alacağın var.`). Yalnız o hareket tarafı çevirdiğinde
    çıkar (`previousSide` ≠ `side`); ilk kayıt ve kapanış çevirme değildir.
    Cümle nedeni varsaymaz: fazla tahsilat da olabilir, eskiden kalan bir
    borç da.
  - `Kalan` — borç anlaşmasının kalanı; sıfırsa `Kapandı`. Borç açılışı ve
    taksitte.
  - `Limit` — kartın o andaki borca göre kalan limiti; `Kart borcu`nun
    altında. Bugünkü limitle hesaplanır.
  Satırların ikonu ve etiketi tek yerdedir (`_BalanceSlot`): cevap beklenirken
  çizilen `…` satırı ile dolu satır aynıdır, panel boyut değiştirmez. `Kalan
  limit` denendi ve 2.0× yazıda taştı; etiket bu yüzden `Limit`.
- **Tahsilat ile ödeme adını yönden alır.** `Cari tahsilat` / `Cari ödeme`,
  `Alacak tahsilatı` / `Borç ödemesi`; yön gelmezse iki yönü karşılayan eski
  ad kalır. Liste satırında ok paranın yönüdür: tahsilatta `kişi → hesap`,
  ödemede `hesap → kişi`. Ayrıntıda `Hesap` hep hesap, `Karşı taraf` hep
  kişidir.
- **`İşlem ekle` launcher'ı başlıklıdır.** Dokuz satır düz listede telefonda
  kaydırmadan okunmuyordu; satırlar niyet başlıklarının (`Para girdi`,
  `Para çıktı`, `Para taşı`, `Belge okut`, `Plan kur`) altında yoğun
  (`dense`) satırlar olarak durur. Açıklama alt satırı her satırda değil,
  yalnız adı yön veya zaman konusunda tek başına yetmeyen satırlarda bulunur —
  dokuz açıklama listeyi ekranın dışına taşırıyordu. Başlık rengi
  `colorScheme.primary`, tipografi `labelLarge`.

### Bir ekrandaki eylem grupları

Ekran başına **tek birincil eylem** kuralı, tek bir *eyleme* uygulanır — tek
bir butona değil. Aynı eylemin iki biçimi varsa ikisi de dolgulu olur ve bir
grup gibi yan yana durur; ikinciyi tonal yapmak onu daha az önemli bir iş gibi
gösterir.

Kredi kartı sayfası bunun örneği:

| Grup | Butonlar | Ağırlık |
|---|---|---|
| Para harcama | `Harcama` · `Taksitli harcama` | İkisi de dolgulu |
| Kartı yönetme | `Ödeme` · `Ekstre` | Tonal ve çerçeveli |

Taksitli harcama da bir harcamadır, yalnız tek seferde değil taksitle.

Eylemler tek bir `Wrap` içinde toplanacaksa **`runSpacing` verilmesi
zorunludur**: verilmediğinde alt satıra düşen butonun kenarı üsttekine yapışır
ve iki satır tek bir blok gibi görünür.

### Liste satırında durum ve eylem

Bir liste satırı bazen "bu iş oldu" der, bazen "bunu yap" düğmesi taşır. İkisi
**aynı yerde ve aynı boyda** durur: rozet başlığın altındaki `badge` alanında,
eylem onun yanında `AppRowAction` olarak. Satırın sağ bloğu her zaman tutardır.

Bunun kuralı olmasının sebebi, kural olmadığında olan: taksit listelerinde
gerçekleşmiş satır kocaman bir `Chip`, gerçekleşmemiş satır dolgulu bir buton
taşıyordu. Aynı listedeki iki satır farklı yükseklikte ve farklı ağırlıkta
görünüyor, göz sütunu takip edemiyordu — oysa ikisi aynı bilginin iki hâli.

- Dolgulu buton (`FilledButton`) satır içinde kullanılmaz; o ağırlık ekranın
  birincil eylemine ayrılmıştır.
- **Çerçevesiz metin butonu da kullanılmaz.** Cihazda görüldü: rozetin
  yanındaki çerçevesiz `Gerçekleştir`, ikinci bir durum etiketi gibi okunuyor
  ve tıklanabilir olduğu fark edilmiyordu.
- **İnce çizgili hap da kullanılmaz.** İkinci deneme buydu ve o da cihazda
  yanlış çıktı: içi boş, soluk bir hayalet gibi duruyordu. `AppRowAction`
  **yumuşak dolguludur** (`secondaryContainer`) — dokunulacak yeri söyler,
  birincil eylemin ağırlığına çıkmaz.
- **Eylem ve rozet aynı yükseklikte** (32 dp) ve aynı hap biçiminde: satır bir
  "durum + eylem" çifti taşır ve ikisi birbirine benzemediğinde satır dağılmış
  görünüyordu. Yerleşim `Wrap(alignment: spaceBetween)`: durum solda, eylem
  sağda; sığmayan durumda alt satıra iner, kırpılmaz.
- **Eylem ikonu varsayılan olarak yoktur.** Rozet zaten bir ikon taşıyor;
  yan yana iki küçük ikon ikisini de okunmaz yapıyordu.
- Eylem butonu `visualDensity: compact` ve `minimumSize: Size.zero` ile
  rozetle aynı yükseklikte kalır; dokunma hedefi
  `tapTargetSize: padded` ile Material'in kendi payıyla korunur — yani
  görünen kutu küçük, dokunulan alan değil.
- Rozet **her** satırda vardır (`Ödendi`, `Gecikmiş`, `Planlandı`); eylem
  yalnız gereken satırda. Durumun kaybolduğu satır, kullanıcıya neden buton
  olmadığını söylemez.

### Onay penceresi

`AppConfirmDialog` üç kademe taşır; düz metin değildir.

| Kademe | Ne söyler |
|---|---|
| **İkon** (48 dp kapsül, ortada) | Kararın türü: yıkıcı mı, ilerleten mi |
| **Vurgu satırı** (`highlight`) | Kararın konusu: hangi kayıt, ne kadar |
| **Paragraf** (`message`) | Sonuç: deftere ne olacak |

Önceki hâli başlık + paragraf + iki butondu ve ekranda bir metin belgesi gibi
duruyordu: en önemli bilgi — ne kadar para, hangi yönde — paragrafın
ortasındaki bir cümlenin içinde kayboluyordu.

Yerleşim tasarım sistemindeki diyalogdur: en çok 360 dp genişlik, 24 dp iç
boşluk, kademeler arası 16 dp; onay dolgulu, `Vazgeç` tonlu dolgulu ve ikisi
alt alta tam genişlikte. **Ayrı bir başlık yoktur**: `title` verilmezse büyük
satır (22/700) vurgunun kendisidir (`Motorlu taşıtlar · ₺2.180,00`) ve eylemin
adı yalnız onay butonunda yazar. `title` veren eski çağrılarda büyük satır başlıktır, vurgu
kendi zemininde altında kalır.

Karar tek bir kayıt hakkındaysa **kayıt kartı** (`AppConfirmSubject`)
kullanılır: ad (16/600), tarih ve kaynak, tutar (22/700) ayrı satırlarda ve
sola yaslı; ikon ve büyük satır çizilmez, mesaj da sola yaslanır. Ad ile tutar
aynı satıra sıkışmadığı için uzun adlar satırı bölmez. Vergi ödemesini geri
alma ve vergi tanımını silme bu kartla açılır.

- **`destructive: true` yalnız gerçekten yıkıcı kararda.** Onay butonunu her
  pencerede kırmızıya boyamak, akromatik markanın bıraktığı az sayıdaki anlamlı
  rengi tüketir. Silme ve iptal yıkıcı; gerçekleştirme ve çıkış değil.
- **Butonlar tam genişlikte ve alt alta.** Yıkıcı bir kararda yan yana iki
  küçük hedef, yanlış olanına dokunmayı kolaylaştırıyordu. Onay üstte, çünkü
  asıl sorulan o; vazgeçmek her zaman geri dönüştür.
- "Emin misiniz?" gibi neyin olacağını söylemeyen bir kalıp kullanılmaz.

### Panel hangi katmanda açılır

`AppAdaptiveSheet` panelleri **kök Navigator'da** açar
(`useRootNavigator: true`). Kabuk her sekmeye kendi Navigator'ını veriyor ve o
Navigator, kayan eylem butonunu ve alt gezinme çubuğunu taşıyan `Scaffold`'un
**gövdesinin içinde**. Varsayılanıyla (`false`) açılan panel o iç katmanda
çiziliyor ve altı artı butonunun arkasında kalıyordu.

Yeni bir panel açarken `showModalBottomSheet` doğrudan çağırılmaz;
`AppAdaptiveSheet` (ya da onu kullanan `AppFormSheet`) kullanılır. Kuralın tek
bir yerde durmasının sebebi bu: her çağıranın hatırlaması gereken bir bayrak,
er geç unutulur.

### Kaç kart: liste mi, grup mu

Kart sayısı içeriğin ne olduğuna göre seçilir; ikisi karıştırıldığında ekran
ya parçalanıyor ya da yapışıyor.

| İçerik | Biçim |
|---|---|
| Birbirinin alternatifi olan kapılar (ayarlar menüsü, bölüm listesi) | **Tek kart**, satırlar arasında ince ayrıcı |
| Ayrı ayrı nesneler (hesap, kart, borç, plan) | **Kart başına bir kutu**, aralarında `AppSpacing.small` |

Menü satırı bir liste kaydı değil bir kapıdır: yedi kapıyı yedi ayrı kutuya
bölmek, birbiriyle ilgili şeyleri ilgisiz gösteriyordu. Bunları yapışık
kartlarla çizmek de yanlıştı: yan yana iki kenarlık kalın bir çizgi gibi
görünüyor. Ayrıcı ikon sütununun sağından başlar
(`indent: medium + 40 + medium`), böylece ikon sütunu kesintisiz kalır.

### Kart arası boşluk

Tema `cardTheme.margin`'i **sıfırlar**: boşluğu listeyi kuran verir, kartın
kendisi değil. Bunun bir bedeli var ve iki kez ödendi — boşluğu vermeyi
unutan liste, kartları birbirine yapışık çiziyor ve alt alta yedi giriş tek
bir blok gibi okunuyor.

- `ListView.separated` kullanılıyorsa `separatorBuilder` ile
  `SizedBox(height: AppSpacing.small)`.
- `for` döngüsüyle `Column` içine yazılıyorsa her kart
  `Padding(bottom: AppSpacing.small)` ile sarılır.
- Ham `Card` yerine `AppCard` kullanılır: `shape`'i yerel olarak ezen kart
  ekrandaki diğer kartlardan farklı bir yarıçapla çizilir ve dokunma dalgası
  kartın şeklini tutmaz.
- Bir satırın durumu kartın **zeminini** boyayarak anlatılmaz (gecikmiş
  ödemeyi `errorContainer` ile doldurmak gibi): dolu zemin satırdaki tutarın
  kendi finansal rengini bastırır. Durum rozetle söylenir.

### Grafikler

| Soru | Biçim |
|---|---|
| Bir dönem nasıl bölündü? | Halka (`AppDonutChart`) |
| Dönemler nasıl değişti? | Dönem başına tek net çubuk (`AppTrendChart`) |
| Bir kalem toplamın ne kadarı? | Satır altı pay çubuğu (`AppShareBar`) |

Sütun grafiği bir **zaman serisi** içindir; tek bir dönemin iki parçaya
bölünmesi için değil. Gelir ve gideri yan yana sütun olarak çizmek, verisi
olmayan aylarda ekranı boş sütunlarla dolduruyordu.

Grafikler tek başına erişilebilir değildir: her dilim ve her sütun kendi ekran
okuyucu cümlesini taşır, değerler efsanede metin olarak da yazılır ve
eğilimde yön çizginin altı/üstü ile renkten bağımsız da anlaşılır.

**Yerleşim tuzağı:** `FractionallySizedBox` yalnız verilen eksende oran
uygular; diğer eksende çocuğunun boyutunu alır. Boyutsuz bir `DecoratedBox`
ile birlikte kullanıldığında çubuk sıfır boyutta çizilir ve grafik "var ama
boş" görünür. Bu hata pay çubuğunda ve eğilim grafiğinde birer kez yapıldı;
ikisi de artık çubuğun gerçekten çizildiğini ölçen testlerle korunuyor.

### Form panelleri

Veri giren her panel `AppFormSheet` kabuğunu kullanır: üstte başlık (ve
gerekirse tek cümlelik kural), ortada kaydırılan alanlar, altta ayırıcı
çizgiyle ayrılmış eylem satırı. Alanlar `AppFormField` ile dizilir.

Kabuk üç şeyi tek yerde çözer:

1. **Controller ömrü.** Panel içeriği `StatefulWidget` olmak zorundadır ve
   `TextEditingController` onun `State.dispose()`'unda bırakılır.
2. **Klavye alanı.** Gövde `viewInsets` kadar pay bırakır; başlık ve eylem
   satırı yerinde kalır.
3. **Gönderim kilidi.** Eylem `AppSubmitButton` olduğu için ikinci dokunuş
   ikinci yazma olmaz.

**Yerleşim tuzağı — controller'ı erken bırakmak.** `await showDialog` çağrısı
`Navigator.pop` anında döner, **kapanma animasyonu bitince değil**. Panelin
widget ağacı bu sırada hâlâ canlıdır ve kare çizer. `await`'ten hemen sonra
`controller.dispose()` çağıran paneller bu yüzden `A TextEditingController was
used after being disposed` hatası veriyor, ardından ağaç yarım sökülürken
`'_dependents.isEmpty': is not true` kırmızı ekranını gösteriyordu. Hata
zamanlamaya bağlı olduğu için her seferinde çıkmıyordu. Doğru yer
`State.dispose()`'tur; kabuk bunu zorunlu kılar.

### Liste satırında kayıt adı ve kategori

Bir hareketin adı **kullanıcının yazdığı metindir**, kategori adı değil
(`title = açıklama ?? kategori adı`, sunucuda üretilir). Kategori paylaşılan
bir raporlama kovasıdır; aynı kategorideki üç abonelik aksi hâlde birbirinden
ayırt edilemezdi.

Bunun bedeli listede görülür: kullanıcı bir cümle yazdığında satır başlığı o
cümle olur ve kategori satırdan tamamen kaybolurdu. İki kural bunu çözer,
adlandırma kuralını bozmadan:

1. **Başlık tek satırdır** (`AppListRow.titleMaxLines: 1`). Serbest metin iki
   satıra yayılıp satırın asıl bilgisini aşağı itmez; tamamı ayrıntı
   panelindedir.
2. **Kategori alt satırın ilk bilgisidir** — `Ulaşım • Banka • 16 Ağustos`.
   Başlık zaten kategori adına eşitse (kullanıcı açıklama yazmamıştır) tekrar
   yazılmaz.

Form tarafında karşılığı: alanın etiketi `Ad (isteğe bağlı)`, yardımcı metni
"Boş bırakılırsa kategori adı kullanılır". Etiket `Açıklama` ve altında
`0/500` sayacı olduğunda alan uzun cümle yazmaya davet ediyordu, oysa girilen
şey kaydın adı oluyor.

### Dil

Uygulamanın kendi metinleri kaynak kodda Türkçedir, fakat Material'in **kendi**
ürettiği metinler (tarih seçici butonları, ay ve gün adları, metin alanı
bağlam menüsü) delege verilmedikçe İngilizce kalır. `AppLocale` üç global
delegeyi ve `tr_TR` yerelini tek yerde tutar; `MaterialApp` ve testler aynı
kaynağı kullanır.

### `AppInlineNotice` — kaydın yanındaki eksik notu

Bir kaydın **eksiğini o kaydın yanında** söyler ve eksiği kapatan eylemi
yanına koyar. Hata görünümü kullanılmaz: veri geçerli, yalnız yarım; hata gibi
gösterilirse kullanıcı bir şeyin bozulduğunu sanır ve düzeltmek yerine korkar.

- Nötr rolün kapsül tonu (`neutralContainer` / `onNeutralContainer`).
- İkon, cümle ve eylem etiketi aynı şeyi ayrı ayrı söyler; bilgi yalnız renkle
  taşınmaz.
- Eylem isteğe bağlıdır, ama varsa **not ile aynı blokta** durur. Kullanıcıyı
  "bir yerlerde bir ayar vardır" aramasına bırakmak, eksiği hiç söylememekten
  iyi değildir.

İlk kullanımı: açılışı kayıtsız borçlar. O borç hesaba ve
raporlara hiç girmiyor ve bunu yalnız kullanıcı düzeltebilir.

### `AppFormError` — kaydın reddi, formun içinde

Sunucunun reddettiği kaydın nedenini **formun içinde**, `Kaydet`in hemen
üstünde söyler (10 Ekim 2026). Reddedilen kayıt formu kapatmaz: form kapanıp
cümle arkadaki sayfada çıkarsa kullanıcı kaydın yazıldığını sanabilir ve
yazdıkları kaybolur.

- `AppInlineNotice` ile aynı biçim (ikon + cümle, kapsül zemin), hata tonunda
  (`errorContainer` / `onErrorContainer`, `error_outline`). Finansal gider
  tonu kullanılmaz: ret bir gider değildir.
- Tam sayfa formda `Kaydet`in üstünde durur. Panelde `AppFormSheet`'in
  `errorMessage` alanına verilir: kaydırılan gövdenin dışında, eylem satırının
  üstünde çizilir, gövde nerede olursa olsun görünür.
- Panel formu kaydı kendisi yazdırır (`onSave` cümle döner, yazıldıysa `null`);
  cümleyi form söylediği için arkadaki sayfanın şeridine bırakılmaz
  (`refusalOf`).
- Bir alana ait doğrulama (boş ad, geçersiz tutar) yine alanın `errorText`'idir.

Kullanan formlar: hesap, kategori, POS, kredi kartı, kişi. Öbür formlardaki
düz kırmızı cümleler henüz bu bileşene taşınmadı.

### Ekran iskeleti

- Başlık **sayfanın kendisinden** gelir. Shell bir başlık daha çizseydi aynı
  ad ekranda iki kez görünürdü.
- Uzun listeler kart yüzeyine oturur, satır araları ince ayırıcıyla ayrılır.
  Her satırı ayrı karta koymak listeyi parçalıyordu.
- Kısa liste grupları (özet bölümleri) tek kart içinde toplanır.
- Durum ekranları (yüklenen / boş / hata / yetkisiz) tek iskeleti paylaşır:
  ikon kapsülü, başlık, açıklama, isteğe bağlı eylem.

### Cari hesap ekranları

Listede her satır tek bakışta üç soruyu cevaplar: kim, hangi tarafta, ne
kadar. Net tek sayıya iner ama **işaret kaybolmaz** — eksi, bizim ona
borçlu olduğumuz demektir ve gider tonunda okunur; kapanmış hesap nötr
tondadır ve "Hesap kapandı" yazar (renk tek başına bilgi taşımaz).

Vadesi geçmiş bakiye satırda saat ikonlu `AppStatusChip` ve “Vadesi geçmiş
alacak/borç” metniyle gösterilir; renk tek başına gecikme anlatmaz. Aynı kişi
iki yönde de gecikmiş olabilir, bu yüzden rozetler `Wrap` içinde yeniden akar.
2.0× metin ölçeğinde etiket kırpılmaz. Ayrıntı kartında toplamın altında
vadesi geçmiş ve vadesi geçmemiş/vadesiz tutarlar ayrı satırlardır.

Ayrıntıda iki taraf **ayrı satırlarda** durur, net üçüncü satırdır: aynı
kişi hem müşteri hem tedarikçi olabilir ve tek sayıya indirmek hangi
tarafın açık olduğunu gizlerdi. Taksitli sözleşmeler ayrı kartta ve
"bakiyeye eklenmez" notuyla durur.

Kartta hiçbir tutar eksi işaretle yazılmaz (9 Ekim 2026). İki satır kısa
adlarını korur: `Size borcu` gelir tonunda, `Sizin borcunuz` gider tonunda;
fazla tahsilat borcumuz satırına, fazla ödeme alacak satırına yazılır
(tutarlar sunucudan). Eskiden `Size borcu −₺300,00` gelir tonundaydı ve alacak
gibi okunuyordu: ton işaretin tersini söylüyordu. `Net` satırı tarafı adıyla
taşır (`Net - Borcunuz` gider tonunda, `Net - Alacağınız` gelir tonunda);
renk tek başına taraf anlatmaz. İlk denemedeki uzun etiket (`Fazla tahsilat ·
sizin borcunuz`) ve `Net · siz borçlusunuz` kullanıcı tarafından reddedildi:
etiketler kısa kalır.

Yükümlülüğün iptali var olan iki kalıbı kullanır: kapanış panelinin altında
hata tonunda, 48 dp `Kaydı iptal et` metin düğmesi (POS tahsilatı ayrıntısındaki
düğmenin aynısı) ve `AppConfirmDialog` (ikon, başlık, kaydı gösteren vurgu, tek
cümle, yıkıcı birincil düğme).

Kişiye bağlı açık faturalar da ayrı karttadır (`Bekleyen faturalar`, 9 Ekim
2026): düğmelerin altında, sözleşmelerin üstünde. Satırlar `AppListRow`
(yön ikonu, ad, `Ödenecek · Vade 20 Ağustos · Gecikmiş`, tutar), altında
ayırıcı ve sunucudan gelen toplam satırı. Kartta düğme yoktur; tek satırlık
notu nereden kapatılacağını söyler. Bu, mevcut dille kurulmuş geçici hâldir:
cari borç, faturalar ve borç planlarının birlikte duruşu cari sayfası
çizilirken kararlaştırılır.

`Diğer` menüsünde `Cari hesap` satırı işletmesi olana ve cari hareketi olana
çizilir (`ScopeController.showsCounterpartyLedger`); diğer kullanıcıda satır
yoktur, grup bir satır kısalır.

Pasif karşı tarafta borçlandırma butonları kapalı, tahsilat açıktır ve
sebebi `AppInlineNotice` ile satırın yanında yazar — kapalı bir butonun
neden kapalı olduğu ekranda görünmezse kullanıcı hatayı kendinde arar.

## Profil ön ayarlı ana gezinme ve Kasa ekranı

Ana gezinme dört hedefi korur. Üçüncü hedef işletme profilinde `Kasa`
(`point_of_sale`), kişisel profilde `Bütçeler` (`donut_small`) olur; yerinden
inen hedef `Diğer` listesinde aynı görünür metin ve vektör ikonla yer alır.
Compact görünümde çentikli alt çubuk, medium/expanded görünümde gezinme rayı
aynı hedef modelini okur. Seçili durum ikon dolgusu, yazı kalınlığı ve semantic
`selected` ile birlikte anlatılır; yalnız renge bırakılmaz.

`Kasa` sayfası sekmesiz **tek akıştır** (Claude Design teslimi, KasaV4):
kasa seçici rayı (ad + bakiye, alan yarıçapı, 56 dp dilim), bugünün sayım
kartı, `POS tahsilatları` bölümü ve `Son sayımlar` (bugünkü sayım dahil). Sayım kartı sayımdan önce
beklenen tutarın kaynağını (son sayım, bugünkü nakit giriş/çıkış), sonra elde
sayılanı, uygulamaya göre tutarı ve farkı gösterir. `Sayımı gir` paneli
`Toplamı yaz | Banknotla say` segmentiyle açılır; toplam alanı Türkçe binlik
ayırıcı taşır (`TurkishAmountInputFormatter`), canlı sonuç şeridi rol kapsül
zemininde `Tuttu` / `₺… eksik` / `₺… fazla` der. Şerit yalnız önizlemedir ve
`MoneyMath` (10⁴ ölçekli tam sayı) ile hesaplanır; kaydedilen fark sunucudan
gelir. Takvim yaprağı (`AppDateLeaf`) sabit 44×48 bir işarettir; metni 1,2×
ölçekte durur ve tarihi satırın kendisi okur. Gün sonunda
beklenen/sayılan/fark, POS tarafında brüt/komisyon/net ayrı bilgi
hiyerarşileridir. Durumlar `AppStatusChip` içinde
`Yolda`, `Gecikti` veya `Hesaba geçti` metni ve ikonuyla gösterilir. Sayım farkı
finansal sonuç doğurduğu için görünür onay ister; hesaba geçiş bir onay
penceresi değil, tutarı ve günü soran yatış panelidir (aşağıda). Birincil form
eylemleri gönderim sırasında devre dışıdır. Yeni renk, boşluk, yarıçap veya
tipografi token'ı eklenmemiştir.

Kasa'da sayım kartının altında `Kendime aldım` metin düğmesi durur (kişi ikonu).
Paneli ve `Eksiği kaydet` paneli aynı kalıbı kullanır: `AppFormSheet`, üstte
seçim rayı (`AppSegmentRail`; `Şahsi hesaba aktar · Şahsi gider`, `Gider ·
Kendime aldım · Bilmiyorum`), altında rayın seçimine göre tek alan. Rayda uzun
ad kullanılmaz; açıklama alanın yardım satırındadır.

Özet'in Net varlık kartında `Yolda` satırı aynı işareti taşır: tutarın altında
en erken beklenen gün (takvim ikonu, soluk); gün geçmişse `27 Eylül · Gecikti`
(uyarı ikonu, gider tonu). Satır ok taşır ve Kasa'yı açar. Satırın sağ tarafı
200 dp ile sınırlıdır; sığmayan gün yazısı kısalır, başlık sıkışmaz.

Ekran ve sayım panelinin iki modu 2.0× metin ölçeğinde test edilir. Alt gezinme/ray
hedefleri Material'ın en az 48 dp dokunma alanını, görünür etiketi ve ekran
okuyucu anlamını korur; sabit genişlikli özel bir mobil yerleşim eklenmez.

## Hesabın kapısı ve `Hesabım` sayfası (Aşama 06 Grup 1)

`Özet` ekranının `AppBar`'ı tek bir action taşır: `account_circle_outlined`
ikonu, `Hesabım` tooltip'i ve semantic etiketiyle. İkon bir profil fotoğrafı
gibi okunmaz — uygulamada avatar yoktur ve kullanıcı görseli hiç saklanmaz.
Dokunma hedefi `IconButton`'ın Material varsayılanıdır (48 dp).

Sayfa mevcut bileşenlerle kurulur; yeni token, renk veya yarıçap eklenmedi:
`AppCard` bölümler için, `AppListRow` satırlar için, `AppSectionHeader` bölüm
başlıkları için, `AppConfirmDialog` onay için, `AppFormSheet` parola ve silme
panelleri için. Yıkıcı bölüm rengini temadan alır (`colorScheme.error`);
ayrı bir "tehlike" token'ı tanımlanmadı.

Parola panelinde yeni parolanın kuralı `helperText` olarak yazılıdır ve
doğrulama **istek gitmeden** çalışır: kural ekranda yazılıyken sunucuya
sordurmak kullanıcıyı bekletirdi.

## Doğrulama uyarısı ve kod alanı (Aşama 06 Grup 2)

Doğrulanmamış adresin uyarısı Material'ın `Badge` bileşeniyle hesap ikonunun
üstünde durur (`smallSize: 8`, etiketsiz). Nokta **tek başına** bilgi taşımaz:
aynı gerçeği `Hesabım` sayfasındaki kart cümleyle söyler, dolayısıyla rengi
göremeyen kullanıcı da uyarıyı okur.

Kod alanı altı hane sınırlıdır (`maxLength: 6`, `counterText: ''` ile sayaç
gizli), sayı klavyesi açar ve doğrulaması **istek gitmeden** çalışır. Panel
`AppFormSheet`'in üçüncü eylemini (`secondaryLabel`) `Kodu yeniden gönder` için
kullanır — vazgeçmek de göndermek de olmayan, kendi sonucunu üreten seçenek.

Parola sıfırlama kendi sayfasıdır (`/password-reset`) ve giriş/kayıt ile aynı
oturumsuz kabuğu paylaşır; `PasswordField` sıfırlama akışında `Yeni parola`
etiketini alır, çünkü "Parola" demek mevcut parolayı soruyormuş gibi görünürdü.

## Varsayılan kapsam alanı (Aşama 06 Grup 4)

Kapsamın artık **üç** denetimi var ve üçü ayrı şeyler sorar; ortak çipi
(`AppScopeChoiceChip`) paylaşırlar ama tek bir bileşende toplanmazlar:

| Bileşen | Nerede | Boşluğun anlamı |
|---|---|---|
| `AppScopeSwitch` | Özet ekranının kapsam barı | `Hepsi` — iki tarafı birden oku |
| `AppScopeField` | İşlem formu, yalnız iki tarafa açık kategoride | Boş kalamaz; hiçbir işaret yoksa alan zorunlu |
| `AppScopeInfoRow` | İşlem formu, tek taraflı kategoride | Soru yok: taraf kategoriden bellidir |
| `AppScopeDefaultField` | Hesap, kart ve kategori formu | Hesapta ve kartta `Belirtilmedi` (bu kaynak taraf söylemiyor); kategoride başlık `Kapsam`, boş değer `İkisi de` (kayıt girerken sorulur) |

Üçüncüsü Grup 4'te eklendi. Üç çipi de sarmalanır (`Wrap`), seçili olan yalnız
renkle değil onay işaretiyle bildirilir ve alan kendi yardımcı cümlesini taşır —
cümleyi çağıran yazar, çünkü hesapta, kartta ve kategoride farklı okunur.

Alan yalnız kapsam boyutunu gören kullanıcıda çizilir.

## Hatırlatma ayarı ekranı (Aşama 06 Grup 3)

Ekranın tamamı cihaz ayarıdır; hiçbir alanı sunucuya yazılmaz. Kapısı `Diğer`
menüsünde, `Planlama ve raporlar` ile `Veri ve yedek` arasında —
kurduğunuz şeyler ile dosya işleri arasındaki doğal yer.

Yapı **kademelidir**: kapalıyken tek bir anahtar durur, tür ve saat bölümleri
hiç çizilmez. Kullanıcı henüz "istiyorum" demeden ona beş kova ve bir saat
seçtirmek, cevabı kullanılmayacak bir soru sormak olurdu.

Bölümler standart bileşenlerle kurulur: `AppCard` içinde `SwitchListTile`
satırları, saat için `ListTile` + Material `showTimePicker`. İki `AppInlineNotice`
kullanılır — izin reddi ve liste okunamaması. Üçüncü bir not (kilit ekranında
tutar yazmadığı) **her hâlde** durur: gizlilik sözü, ancak görünürse sözdür.

## Erişilebilirlik kuralları

1. **Hiçbir bilgi yalnız renkle taşınmaz.** Her durum ikon veya metinle de
   bildirilir. Renk körlüğünde yeşil/kırmızı ayrımı hiçbir sayısal ölçütle
   garanti edilemez; kontrast oranı parlaklık farkını ölçer, ton farkını değil.
2. **Her dokunulabilir öğe en az 48×48 dp.**
3. **İkon-yalnız her eylemin erişilebilir adı vardır** (`tooltip` veya
   `semanticLabel`).
4. **Her ekran 2.0× yazı ölçeğinde taşmadan çalışır.** En sık hata deseni: bir
   metnin iki buton arasında esnek olmaması. `Flexible` kullanın; başlık+eylem
   satırlarında `Wrap` kullanın ki eylem alt satıra inebilsin.
5. **Kontrast eşiği** normal metin için 4.5:1, büyük metin ve metin dışı öğeler
   için 3:1.
6. **Loading / empty / error / unauthorized / stale** durumları her ekranda
   görünür ele alınır.

## Yeni ekran eklerken kontrol listesi

- [ ] Boşluklar `AppSpacing`, yarıçaplar `AppRadius` adlarından geliyor.
- [ ] Ham `Colors.*` yok; anlamlı renk `AppFinanceColors`, yüzey rengi
      `Theme.of(context).colorScheme`.
- [ ] Genişlik doğrudan karşılaştırılmıyor; gerekiyorsa `context.windowSize`.
- [ ] Tutarlar `AppMoneyText` ile gösteriliyor. Bileşen kullanılamayan bir
      yerde (bir cümlenin içinde geçen tutar) en azından `MoneyText.format`
      ile — ham sunucu dizesi `1000.0000` olarak ekrana yazılmıyor.
- [ ] Durumlar `AppStatusChip` ile, ikon ve metinle birlikte.
- [ ] Liste satırındaki durum ve eylem aynı yerde ve aynı boyda; satır içinde
      dolgulu buton yok.
- [ ] Tarihler `DateText`'ten geçiyor; ham `2026-09-30` ekrana yazılmıyor
      (ekran okuyucuya verilen cümleler de dahil).
- [ ] Gönderim butonları `AppSubmitButton`, onaylar `AppConfirmDialog`.
- [ ] Form paneli açılıyorsa `AppFormSheet`; alanlar `AppFormField` ile
      diziliyor ve panel içeriği `Navigator.pop` çağırmıyor.
- [ ] Tarih alanı `AppDateField`; ekranda çıplak `showDatePicker` yok.
- [ ] Form paneli açılmıyorsa ve yalnız içerik gösteriliyorsa
      `AppAdaptiveSheet`.
- [ ] Test `MaterialApp`'i `AppLocale` delegelerini kuruyor (tarih seçiciye
      dokunan testlerde zorunlu).
- [ ] Yardımcı metin `bodySmall`, bölüm etiketi `labelMedium`/`labelSmall`
      kullanıyor; hepsi tek koyulukta değil.
- [ ] Açılır listede grup başlığı varsa `AppMenuGroupLabel`.
- [ ] Uzun satır listeleri tek kartta ve ayırıcılarla gruplanmış.
- [ ] Loading / empty / error / unauthorized durumları görünür.
- [ ] Test dosyası `MaterialApp`'i `AppTheme.light()` ile kuruyor.
- [ ] Ekran testine erişilebilirlik kapısı eklendi
      (`expectNoOverflow` + `expectMeetsAccessibility`,
      `test/helpers/accessibility.dart`).

### Ödenmemiş fatura formu

`/transactions/new/obligation` tam sayfa bir finans formudur; `Yükümlülükler`
listesinden açıldığında aynı form `/more/obligations/new` adresindedir (liste
shell'in dışında olduğu için kendi altındaki rotaya açar). Okunan değerler
öneri olarak görünür; belge tarihi ile son ödeme tarihi ayrı alanlardır. Form
hesap ve sıklık alanı çizmez. Loading, error ve unauthorized durumları mevcut
durum bileşenlerini; gönderim kilidi `AppSubmitButton`'ı kullanır. Yeni renk,
boşluk veya tipografi token'ı eklenmemiştir.

### Yükümlülük listesi ve kapanış paneli

`/more/obligations` üç metin etiketli sekme kullanır: yaklaşan, geciken,
kapanan. Durum `AppStatusChip` içinde ikon + metinle verilir; gecikme yalnız
renkle anlatılmaz. Satırlar mevcut `AppCard`, `AppListRow` ve `AppMoneyText`
bileşenlerini kullanır. Kapanış `AppFormSheet` içinde hesap seçimi ve gönderim
kilidiyle yapılır. Loading, empty, error, unauthorized ve stale-cache durumları
görünürdür; yeni tasarım token'ı eklenmemiştir.

## Kuralları uygulayan testler

| Kapı | Yer |
|---|---|
| Kontrast: 5 rol metni + 3 dolgu + 6 kategori dilimi × 3 yüzey × 2 tema | `test/core/theme/app_finance_colors_test.dart` |
| Rol başına metin ve dolgu tonunun ayrı kalması | `test/core/theme/app_finance_colors_test.dart` |
| Mürekkep merdiveni: üç kademe × 3 zemin × 2 tema + kademelerin ayrışması | `test/core/theme/app_surfaces_test.dart` |
| Tintli metrik kutusunun kendi zeminine karşı okunurluğu | `test/core/widgets/app_metric_tile_test.dart` |
| Pencere sınıfı eşikleri | `test/core/theme/app_breakpoints_test.dart` |
| Yerleşim bileşenleri | `test/core/widgets/` |
| Form paneli sözleşmeleri (kapanış, doğrulama, üçüncü eylem) | `test/core/widgets/app_form_sheet_test.dart` |
| Tarih alanı dekorasyonu ve API biçimi | `test/core/widgets/app_date_field_test.dart` |
| Kapsam anahtarı ve kapsam çipi (üç konum, onay işareti, 2.0×, erişilebilirlik) | `test/core/widgets/app_scope_selector_test.dart` |
| Üç sayılı hero: tutarlılık, etiketin yönü, 2.0× taşma, `kâr` yasağı | `test/features/dashboard/dashboard_hero_test.dart` |
| Türkçe Material metinleri | `test/widget_test.dart` |
| Profil ön ayarlı dört ana hedef ve karşı hedefin `Diğer` erişimi | `test/widget_test.dart` |
| Kasa/POS 2.0× metin, dokunma hedefi, ad ve kontrast | `test/features/cash/cash_pos_feature_test.dart` |
| Ham renk / ölçek dışı boşluk / doğrudan genişlik karşılaştırması | `test/architecture/design_tokens_test.dart` |
| Dokunma hedefi, adlandırılmış hedef, metin kontrastı, 2.0× taşma | `test/helpers/accessibility.dart` + ekran testleri |

Yapısal kapı kaynak kodu tarar. Bunun nedeni, kuralın doğası: ihlal her zaman
henüz testi olmayan **yeni** bir dosyada ortaya çıkar, dolayısıyla widget
testleriyle korunamaz.

## Ay/dönem seçici (`AppMonthPicker`)

Ay gezinmesi iki denetimden oluşur ve ikisi ayrı işler için:

- **Oklar** komşu aya gider. Yerinde kalıyorlar; bir ay geri gitmek için panel
  açmak fazladan iki dokunuş olurdu.
- **Ay adının kendisi bir butondur** ve dönem seçici panelini açar: yıl için
  iki ok, altında on iki ay çipi. Uzağa gitmek için oklar yanlış araçtı —
  sekiz ay geri bakmak sekiz dokunuş ve sekiz ağ isteği demekti.

Takvim (`showDatePicker`) kullanılmaz: gün seçtirir, oysa burada gün diye bir
şey yoktur ve kullanıcı olmayan bir kararı vermek zorunda kalırdı.

## Bütçenin eşik uyarısı

Bütçe iki eşikte konuşur ve ikisi **birbirine benzemez**:

| Durum | Rozet tonu | Çubuk dolgusu |
|---|---|---|
| Limit içinde | `planned` + `check_circle_outline` (`Limit içinde`) | `neutralFill` |
| Eşiğe yaklaştı (`>= budgetWarningThreshold`) | `planned` + `info_outline` (`Limite yakın`) | `neutralFill` — değişmez |
| Limit aşıldı | `expense` + `warning_amber_rounded` (`Aşıldı`) | `expenseFill` |

Bütçeler ekranı (Claude Design teslimi): geri oklu `AppPageHeader` ve `+`,
ay seçici, doluluğa göre dizilmiş kartlar. Kart: kategori kapsülü (aşımda
gider tonu), ad ve kapsam, sağ üstte durum kapsülü; `metric` boyunda
harcanan `/ limit`, 8 dp çubuk, altında `%NN` ve `₺… kaldı` / `₺… fazla`
(aşımda gider rengi). Tasarımdaki %88 bütçe `Limit içinde` görünüyordu;
Aşama 06'nın "aşılmadan önce uyar" kararı korunarak eşiği geçen bütçe
`Limite yakın` kapsülü alır.

Uyarı için altıncı bir renk rolü **açılmadı** (ADR 0008). İki gerekçe: yeni bir
hue paleti finansal rollerden çıkarıp dekoratif bir uyarı ailesi kurardı; ve
uyarı ile aşım aynı renge boyansaydı ikisini ayırt eden tek şey metin kalır,
renk de aşılmamış bir sınır için alarm verirdi. Uyarıyı taşıyan şey sözdür,
renk değil — rozet zaten ikon **ve** metin taşımak zorundadır.

Eşiğin değeri tek yerdedir: `lib/core/models/budget_threshold.dart`. Bütçe
ekranı ve Özet'in `Bütçe durumu` kartı aynı sabiti okur; iki ayrı eşik, Özet
"limit içinde" derken bütçe ekranının uyarmasına yol açardı.

## Özet ekranı (Claude Design teslimi, 27 Eylül 2026)

Üstten alta: `AppPageHeader` + avatar · kapsam rayı (kaydırılan gövdenin
dışında) · ay seçici · gecikmiş ödeme şeridi (`expenseContainer`, 52 dp) ·
gelir/gider/net kartı · kategori giderleri (104'lük halka + satır altı pay
çubukları) · bütçe halkaları · yaklaşanlar zaman çizelgesi · varlık durumu ·
hesap bakiyeleri.

- **Bütçe halkaları** en dolu bütçeleri gösterir: 4 ve fazlası 2×2, 2–3
  ikili, 1 tek. Aşılan bütçe gider zemininde, halka gider dolgusunda. Yüzde
  ve halka boyu yalnız çizim oranıdır; kalan/aşım tutarı sunucunun
  `remaining`'idir.
- **Yaklaşanlar** 7 günün ödemelerini zaman çizelgesinde (en fazla beş kalem)
  ve göreli vadeyle (`Yarın`, `3 gün sonra`) yazar; altta sunucunun
  `upcomingOutgoingTotal`'ı (`7 günde çıkacak`). Gecikmişler bu toplama
  girmez, şeridin konusudur.
- **Varlık durumu** net varlık, varlık/borç oran çubuğu ve iki taraf:
  `Varlıklar` (likit, yolda + geçiş günü, alacak, varsa kart alacağı; nötr
  mavi) ve `Borçlar` (kart borcu, borç; gider kırmızısı). İki tarafın
  toplamı sunucudan gelir (`totalAssets`, `totalLiabilities`).
- Dokununca başka ekrana giden özet kartları (bütçe halkaları, yaklaşanlar)
  ekran okuyucuya **tek durak**tır: etiket içeriğin cümlelerini taşır, eylem
  aynı düğümdedir.

### Ekran görüntüsüyle karşılaştırma

`test/screenshots/` ekranları teslim paketindeki çerçeveyle aynı biçimde
(412×892 dp, 2×, gerçek Roboto ve Material ikon fontu) PNG'ye çizer; örnek
veri paketteki `DATA` ile aynıdır. Normal test koşusunda atlanır:

```bash
SCREENSHOT_DIR=/tmp/shots flutter test test/screenshots
```

## İşlemler ekranı (Claude Design teslimi, 27 Eylül 2026)

- `AppPageHeader` + filtre ikonu; gri arama alanı (48 dp, `cardMuted`);
  yatay kayan çipler (DS `AppScopeChoiceChip`: 14/600, 32 dp); planlananlar
  şeridi (`plannedContainer`, 48 dp).
- Akış günlere bölünür: gün başlığı sayfa zemininde 15/600 tarih + gri
  göreli gün, `PinnedHeaderSliver` ile üstte durur; günün kayıtları tam
  genişlik beyaz blokta, üst/alt kenar çizgisi, ayırıcı 72 dp'den.
- Satır: yuvarlak rol kapsülü, 16/600 başlık, alt satırda kategori ve hesap
  (tarih gün başlığında), sağda `row` boyunda işaretli tutar. İptal edilmiş
  satır 0,55 soluk, tutar işaretsiz ve üstü çizili, altında `İptal edildi`.
- Ayrıntı paneli: kapsül + başlık + `Tür · Kapsam`, 28/700 tutar ve dolgulu
  durum kapsülü (`Gerçekleşti` / `İptal edildi`), `AppDetailBlock`,
  tam genişlik kenarlı `Hareketi iptal et`.

## İşlem ekle paneli (Claude Design teslimi, 27 Eylül 2026)

Kök Navigator'da açılır, alt çubuğun üstüne biner. Başlık `İşlem ekle`
(24/700). Üç katman: rol kapsül renginde üç kutucuk (Gelir / Gider /
Transfer; 96 dp, kart yarıçapı, ikon 24 + 14/600), `Belgeden oku` altında iki
gri kutucuk (Fiş veya fatura, Banka dekontu; 64 dp), `Diğer` altında kapsüllü
satırlar (POS tahsilatı, Ödenmemiş fatura, Kart borcu öde, Tekrarlayan işlem).
Açıklama yalnız yanlış anlaşılabilecek satırda; kutucukta görünmeyen ayrıntı
ekran okuyucu cümlesindedir (`QuickAddOption.spokenLabel`). Niyet başlıkları
(`Para girdi`, `Para taşı`…) panelden kalktı; `QuickAddIntent` modelde
duruyor ve transferin gider sayılmadığını (ADR 0014) yine o taşır.

## Vergi takibi (Claude Design teslimi, 30 Eylül 2026, Aşama 06.3 Grup 3)

Kaynak `design/vergiler-handoff/` (ölçü `screens-v5/*.jsx`, kıyaslama referansı
`screenshots/`). Teslimin
örnek verisi tasarımı göstermek içindir; dönem etiketi ("KDV · Ağustos") gibi
anlamlar uygulanmadı, satırda vergi adı ve tarih yaprağı durur.

- **Sayfa kabuğu** (`TaxPageScaffold`): geri oklu `AppPageHeader`, kayan gövde
  (yan 16, alt 32) ve isteğe bağlı sabit alt şerit (üstte 1 dp çizgi, tuval
  zemini). Vergi takibinin alt şeridi her hâlde "Vergi ödemesi ekle"dir; bir
  gönderim değil panel açtığı için `AppSubmitButton` değil `FilledButton`.
- **Bekleyenler kartı**: `AppCardHead` "Gecikenler ve 30 gün" + `N gecikti`;
  satır en az 72 dp (tarih yaprağı, ad, durum + göreli gün, tutar ya da
  "Tutar ödemede girilecek"); sağda kenarlı hap `TaxPayButton` (36 dp, dokunma
  48). Alt şerit gri, iki eşit hücre: "Ödenecek" (gecikenler dahil) ve
  "Tutarı belli olmayan · N ödeme"; ikincisi sıfırsa çizilmez.
- **Paneller**: form panelleri 24/700 başlık + tek satır (`TaxSheetTitle`) ve
  büyük tutar alanı (`TaxAmountInput`, "Sayımı gir" dili); ayrıntı panelleri
  kapsül + ad + `Vergi · İşletme` + kapat (`TaxSheetHead`), tutar + durum,
  `AppDetailBlock`, başka sayfaya giden bağlantı ayrı kartta eylem satırı
  (`TaxActionCard` / `TaxActionRow`, en az 64 dp), altta en fazla iki eşit
  buton (`TaxSheetFooter`, büyük yazıda alt alta). Yıkıcı eylem kenarlı ve
  kırmızı metinli; onay diyaloğu sonucu yazar.
- **Tanım formu**: kapsam rayı en üstte ve başlıksız (`AppSegmentRail`,
  Özet'teki anahtarın iki dilimli hâli; yalnız `ScopeController.isVisible`).
  Ay seçimi `AppMonthChips` (6×2, büyük yazıda 4×3, seçili marka dolgusu);
  tekrarlayan plan formu da aynı çipleri kullanır. Başlangıç, ritimden
  türetilen tarihlerin seçimidir. Ritim değişince nötr `AppInlineNotice`.
- **Ortak yeni parçalar**: `AppUnknownAmount` (tutarı belli olmayan kalemin
  tutar yeri; Planlananlar, Özet, Planlama), `AppMonthChips`,
  `AppDetailBlock.background`, `AppDateField.valueText` (Türkçe tarih).
- **Özet**: Yaklaşanlar kartının başında gecikenler satırı (gider zemini, sayı,
  en eski vade, sunucunun gecikmiş toplamı; ok yok, kart Planlananlar'ı açar).
  Üstteki gecikme şeridi tutarsız kalır; ok ile tarih arasında 8 dp.

### Vergi panellerinin kabuğu (1 Ekim 2026, emülatör turu)

- **`TaxSheetBody`** paneli ekranın tamamına yaymaz: tutamaç ve alt güvenli
  alan dahil panel, durum çubuğunun altındaki alanın en çok %91,7'sini kaplar
  (tasarımdaki 818/892); arkada sayfanın başlığı görünür kalır. İçerik
  sığmıyorsa gövde kayar. Form panellerinde gönderim butonu `footer` olarak
  **kaydırmanın dışında** durur ve gövde kayıyorsa üstünde 1 dp çizgi çıkar.
- **`AppAdaptiveSheet`** panelleri `useSafeArea` ile açar: uzun panel durum
  çubuğunun altına girmez, tutamaç her zaman tutulabilir (bütün paneller).
- **`TaxAmountInput`** simge ve tutarı tek grup olarak çizginin ortasına
  koyar; alan yalnız yazılan kadar yer kaplar (temanın alan dolgusu burada
  kapalıdır), etiketle tutar arası 4 + 4 dp.
- **İki panel üst üste açılmaz**: bekleyen kalemin "Tutarı gir"i panelin
  yerine açılır ve kapanınca kalem paneli yeni tutarla geri gelir; ikincisi
  birincisinin kapanış animasyonu ve klavye indikten sonra açılır.
- **`AppDateField.iconLeading`**: takvim simgesi değerin solunda (tasarımdaki
  panel alanları); varsayılan sağdadır, diğer formlar öyle çizilmiştir.
- **`AppConfirmSubject`**: karar tek bir kayıt hakkındaysa kayıt kartı (yukarıda
  "Onay penceresi").
- Ödenenler ana sayfada son 5 ödemeyi, `Tümü` hepsini ay başlıkları altında
  gösterir; başlık toplam yazmaz.

## POS'larım ve POS'tan dolan tahsilat formu (1 Ekim 2026, Aşama 06.3 Grup 4)

Tasarım teslimi yok; mevcut dille kuruldu. Kasa sekmesi yeniden tasarlanınca
(Grup 6) kapı "Kartla gelecek" bölümüne taşınır.

- **Kapı**: Kasa'daki "POS tahsilatları" başlığında `POS'larım` ve `+ Ekle`.
- **POS'larım**: kart içinde `AppRow` (POS kapsülü, ad, `hesap · %oran · süre`).
  Sağda **yıldız**: dolu yıldız ana POS'tur, boş yıldıza dokunmak onu ana POS
  yapar; pasif POS'ta yıldız yerine `Pasif` etiketi durur. Boş durum bir
  sonraki adımı sunar ("POS ekle").
- **POS formu** ayrı sayfadır: ad, paranın geçeceği hesap, satış kategorisi,
  komisyon oranı (%), komisyon gider kategorisi (yalnız oran yazılınca), kaç
  günde geçtiği, "İş günü say" anahtarı. İşletme setindeki "Satış geliri" ve
  "Banka ve POS komisyonu" hazır seçili gelir. Düzenlemede altta `Pasife al` /
  `Etkinleştir` ve kırmızı metinli `Sil`.
- **Tahsilat formu**: POS seçiliyken yalnız POS, tutar ve gün sorulur; altında
  `AppDetailBlock` ile komisyon, hesaba geçecek, beklenen gün ve hesap
  (sunucunun önizlemesi; tutar yazılana kadar `—`). POS seçiminde **"Elle gir"
  bir POS değildir**: `AppMenuGroupLabel` ile ayrı grup başlığının ("POS
  seçmeden") altında ve kalem simgesiyle durur.
- **Kelime**: arayüzde "tanım" geçmez; "POS", "POS ekle", "Elle gir".

## Yatış panelleri (2 Ekim 2026, Aşama 06.3 Grup 5)

Tasarım teslimi yok; mevcut dille kuruldu (`lib/features/pos/presentation/
pos_deposit_sheets.dart`). Kasa sekmesi yeniden tasarlanınca (Grup 6) kapı
"Kartla gelecek" bölümüne taşınır.

- **Kapı**: "POS tahsilatları" kartında, yoldaki toplamın altında
  `AppTextAction` — `Hesaba geçenleri işaretle`. Yalnız yolda tahsilat varken
  görünür. Yoldaki satırın ayrıntısındaki `Hesaba geçti` aynı paneli yalnız o
  tahsilat seçili açar.
- **Panel üst üste açılmaz**: tahsilat ayrıntısı bir sonuçla kapanır
  (`PosSettlementSheetAction`), açan taraf sıradaki paneli açar — vergi
  panellerindeki kuralın aynısı.
- **"Hesaba geçenleri işaretle"** bir `AppFormSheet`'tir: yoldaki tahsilatlar
  onay kutulu satırlarla (başlık, `hesap · beklenen gün`, sağda net tutar),
  `AppDetailBlock` içinde `Beklenen` ve `Hesap`, `Yatan tutar` alanı (beklenen
  tutarla dolu gelir), `Yattığı gün`. Günü gelmiş tahsilatlar seçili gelir.
  Bir yatış tek hesaba düşer: ilk seçimden sonra başka hesabın satırları
  soluklaşır ve seçilemez; birden çok hesap varsa altında tek cümle yazar.
  Satır `CheckboxListTile` değildir — o kendi yazı stilini taşır; satırın
  tamamı dokunma hedefidir ve yazısı `bodyMedium` / `bodySmall`'dır.
- **Kesinti yalnız fark varsa görünür**: yatan tutar beklenenden azsa
  `Kesinti` satırı (gider tonunda, işaretli) ve `Kesinti kategorisi` alanı
  çıkar; kategori POS'un komisyon kategorisiyle dolu gelir. Yatan tutar
  beklenenden fazlaysa alanın altında hata metni durur ve kayıt gönderilmez.
  Beklenen ve kesinti **sunucunun önizlemesidir**; tutar yazılırken kısa bir
  duraklamadan sonra istenir.
- **Yatış ayrıntısı**: tahsilat ayrıntısıyla aynı iskelet — kapsül + başlık
  (`POS yatışı`, altında hesap), büyük tutar (yatan), durum etiketi
  (`Hesaba geçti · gün` ya da `Geri alındı`), `AppDetailBlock` (Satış,
  Komisyon, Beklenen, Kesinti, Kategori, Tarih, Hesap, Bakiye),
  `Kapattığı tahsilatlar`, tek cümle
  kural metni ve çerçeveli `Yatışı geri al` (görünür onayla). Geri alınmış
  yatışta eylem yoktur. Kapattığı tahsilatların başlığı kullanıcının yazdığı
  metindir ve uzun olabilir; `AppDetailRow` yerine saran bir satırla çizilir
  (`AppDetailRow` etiketi esnemez ve 2.0× yazıda taşar).
- **Hesaba geçmiş tahsilatın ayrıntısı** `Kaydı iptal et` sunmaz; yerinde
  `Yatışı gör` ve "Kaydı iptal etmek için önce yatışı geri alın." durur.

## Gün sonu paneli, ayrıntısı ve Kasa kartı (4 Ekim 2026, Aşama 06.3 Grup 5)

Panel 10 Ekim 2026'da `design/gun-sonu-handoff/gunsonu/README.md` teslimine
göre yeniden kuruldu (`lib/features/day_close/`). README, karelerle çelişirse
geçerlidir. Ayrıntı ve Kasa kartı mevcut dille kalır.
Kasa sekmesi yeniden tasarlanınca (Grup 6) kart "Bugün" bölümüne taşınır.
Kullanıcı paneli ekran görüntüsünde onayladı (4 Ekim).

- **Kelime**: `Gün sonu` günün **satışının** girilmesidir. Çekmecedeki nakdin
  sayılması `Kasa sayımı` / `Kasayı say`dır; ikisi aynı adı taşımaz.
- **Kapılar**: "+" menüsünde işletme profilinde `POS tahsilatı`nın yerinde
  `Gün sonu` (alt yazı: `Günün nakit ve kartlı satışı`); Kasa'nın en üstünde
  gün sonu kartı. Menüden gelen kullanıcı vazgeçerse geldiği ekrana döner.
- **Panel** bir `AppFormSheet`'tir: `Gün`, `Nakit tutarı`, ana POS'un alanı,
  `Diğer POS'lar (n)`, `Toplam (isteğe bağlı)`. Gün doğal Türkçeyle, para girişi
  mevcut Türkçe binlik biçimleyicisiyle gösterilir; dört ondalık kayıpsız
  gönderilir. Boş para alanında da para birimi görünür. Toplamdan alan
  hesaplanmaz; toplam yalnız sunucunun verdiği farkı gösterir.
- **Yan boşluk 16 dp** (`AppFormSheet.horizontalPadding`; öbür formlar 24 dp).
  Teslim 16 dp çerçeveyle ve harf aralıksız çizildi; kural cümlesi ve soru
  cümlesi bu yüzden `letterSpacing: 0` taşır ve tek satıra sığar. Deneme
  niteliğindedir (kullanıcı, 10 Ekim 2026): beğenilirse öbür formlara da
  uygulanır, beğenilmezse bu panel 24 dp'ye döner. Soru bloğunda kişinin adı,
  satırda saat yazılmaz.
- **Gün içinde girilenler**: etiket 13 / `inkFaint`, altında
  `İşaretli kayıtlar yazılan tutarın içindedir; tekrar kaydedilmez.` Önce hazır
  işaretli kayıtlar, ince çizgi, toplu soru ve sorulan kayıtlar. Nakit boşken
  nakit kayıtları ve soru gizlidir; kartlı kayıtlar görünür. Görünür kayıt
  kalmadıysa bölüm çizilmez. Satır en az 56 dp, sol hedef 48 dp; ad 16/600,
  tür ve yer ikinci satırda, saat yok; tutar sağda ve kırpılmaz.
- **Toplu cevap**: `Bunlar yazdığınız nakit tutarın içinde mi?`, seçimsiz
  `AppSegmentRail<bool>` (`Hepsi içinde` / `Hiçbiri`), altında
  `Tek tek değiştirmek için satıra dokunun.` Karışık veya cevapsız satırlarda
  seçimsizdir. Sorulan satır `?` → işaretli → işaretsiz → işaretli; hazır
  işaretli satır iki hâllidir. Satırın tamamı onay kutusu anlamı taşır;
  cevapsız satır ekran okuyucuda karışıktır.
- **Satış ve tahsilat sorusu**: dahil olan aynı kişinin/faturanın kayıtları
  bitişik dizilir; sorusu kendi satırlarının altında `cardMuted`, 16 dp
  yarıçaplı bloktadır. Soru, sağda `Kayıtlı sayılan` sütun başlığı, üç 48 dp
  radyo satırı ve sağda sunucunun verdiği tutar. Blokta ad yazılmaz: o
  kişinin satırlarının hemen altındadır. `İkisi ayrı ayrı`, küçük olanın büyüğün içinde
  olduğunu söyleyen seçenek (satış/tahsilat veya fatura/tahsilat),
  `Bir kısmı ikisinde de var`. Son seçenek `İkisinde de sayılan` alanını ve
  `En çok ₺…` yardımını açar; sağdaki sonucu yeni önizleme gelince gösterir.
  Seçenek panel durumudur; tutar kıyaslayarak türetilmez. Bir kaydın seçimi
  değişince soru yeniden cevaplanır. İşaretli gelen satır yoksa kural
  cümlesinin altına çizgi çizilmez.
- **`Yazılacak`**: yalnız yazılan alanın gri kartı çizilir. `Yeni nakit satış`,
  kasa ve kategori, sağda gelir yeşiliyle `+₺…`; ince çizgi altında
  `Nakit tutarı` / `Zaten kayıtlı −₺…`; sıfır olmayan `cash.deductions`
  parçalarının 13 / `inkFaint` dökümü. İkisinde de sayılan parça eksiyle
  ayrı satırdır. POS kartı satış adı, komisyon ve hesaba geçiş günü taşır;
  kayıtlı tutar varsa `Kart tutarı` / `Zaten kayıtlı` dökümü açılır.
  Cevapsızken tutar yerine `n kayıt için seçim yapılınca hesaplanır.` veya
  `Yukarıdaki soru cevaplanınca hesaplanır.` yazılır. `düşüldü` kullanılmaz.
  Bütün yazılacaklar sıfırsa `Yazılacak kayıt yok; gün kapatılır.` görünür.
- **Kasa ve kategori sorulmaz**: özetin altındaki `Kasayı ya da kategoriyi
  değiştir` iki açılır alanı gösterir. Sunucu seçemediyse alanlar kendiliğinden
  açılır.
- **Cevapsız durum**: `Gün sonunu kaydet` kapalı; kırmızı veya uyarı kutusu
  yoktur. Önizleme beklenirken de kaydet kapalıdır; nakit kartında
  `Hesaplanıyor…` görünür. 2.0× yazıda tutar başlığın altına iner; ad ve tutar
  kırpılmaz. Yeni renk, token veya paket eklenmedi.
- **Toplam farkı**: mevcut `AppInlineNotice` ile
  `Yalnız kart satışı kaydedilir. Toplamla arasındaki ₺… kaydedilmez.`;
  yalnız nakitte `Yalnız nakit satış kaydedilir.`, iki alan yazıldıysa
  `Nakit ve kart satışı kaydedilir.`. Neden tahmin edilmez.
- **Engeller alanın yanında söylenir** (alanın `errorText`'i); bir alana ait
  olmayanlar listenin altında tek bir `AppInlineNotice`'tir. "Tutar yazılmadı"
  yalnız `Gün sonunu kaydet`e basıldıktan sonra söylenir: boş açılan panel
  uyarıyla karşılamaz.
- **Kapalı gün**: `Gün`ün altında bildirim ve `Ek gün sonu` onay satırı;
  alanlar kilitli, kayıt listesi gizli. Kutu işaretlenince ikisi de açılır.
- **Gün ayrıntısı günü gösterir, tek gün sonunu değil** (`showDayCloseDay`;
  4 Ekim emülatör turundan sonra). Yatış ayrıntısıyla aynı iskelet: kapsül +
  başlık (`Gün sonu`, altında gün), durum etiketi (`Gün kapatıldı` / `Gün sonu
  girilmedi`), `AppDetailBlock` içinde günün `Nakit` ve `Kart` toplamı.
  Altında her gün sonu için bir bölüm (`Gün sonu`, `Ek gün sonu`, varsa
  `· Z 3143`): gri blokta kayıt satırları, alt yazı kaydın bağını söyler
  (`Yazıldı · Dükkan kasası`, `Yazıldı · yolda`, `Sayıldı · nakit`), altında
  çerçeveli `Gün sonunu geri al` / `Eki geri al`. Eki olan ana gün sonunda
  düğme yerine tek cümle durur. En altta `Gün sonunun dışında` (sayılmamış,
  tek tek girilmiş kayıtlar). Büyük tutar yoktur: gün tek bir para hareketi
  değildir.
- **Kasa kartı** `AppCard`'dır: kapsül, `Gün sonu`, altında `Bugün girilmedi`
  ya da `Bugün girildi`. Gün açıkken tam genişlik `Gün sonunu gir`; kapalıyken
  günün toplamını taşıyan dokunulabilir tek satır (`Nakit ₺… · Kart ₺…`,
  sağda ok; günün ayrıntısını açar) ve metin eylemi `Ek gün sonu gir`.
- **Gün sonuna bağlı kayıt İşlemler'de okunur**: gün sonunun yazdığı kayıt
  satırın alt yazısında `Gün sonu` taşır. Ayrıntıda `Hareketi iptal et`
  yerine kilit satırı (yazılan: "Gün sonundan gelen kayıt…", sayılan: "Gün
  sonunda sayıldı…") ve çerçeveli `Gün sonunu gör` durur. Kasa'daki tahsilat
  ayrıntısı gün sonundan gelen tahsilatta `Kaydı iptal et` yerine tek cümleyi
  gösterir.
- Hata durumları görünür: yatış panelinde ve yatış ayrıntısında
  `AppInlineNotice`; ayrıntı okunamazsa `AppErrorView` ("Tekrar dene"), oturum
  bittiyse `AppUnauthorizedView`.
- **Yatış ayrıntısı diğer işlem ayrıntılarının dilini kullanır** (2 Ekim
  2026, kullanıcı bildirimi): kapsül ve büyük tutar **nötr rolde** (mavi) —
  listedeki satırla aynı; satırlar ikonludur; `Bakiye` yeşildir (yatış parayı
  hesaba sokar). İlk teslimde kapsül gri, tutar siyahtı ve panel diğer
  ayrıntıların yanında yarım görünüyordu. Gün etiketi `Tarih`tir: ikonlu
  satırda uzun etiket 2.0× yazıda taşıyor.
- **Yatış ayrıntısı yatan tutarın nereden geldiğini söyler** (2 Ekim 2026,
  kullanıcı isteği): `Satış` (kapattığı tahsilatların brüt toplamı) ve
  `Komisyon` satırları `Beklenen`in üstünde durur — satış − komisyon =
  beklenen. Günler sonra yatan küsuratlı bir tutarda ne kadar komisyon
  kesildiği başka türlü okunmuyordu. İki toplam sunucudan gelir
  (`grossAmount`, `commissionAmount`); komisyon satış günü gider yazılmıştır,
  burada yeniden yazılmaz. Sıfır komisyonda da satır durur (panel yüklenince
  boyut değiştirmesin); geri alınmış yatışta iki satır da yoktur. `Komisyon`
  bankanın anlaşmalı payı, `Kesinti` beklenenin altında yatan farktır; ikonları
  ayrıdır (`percent`, `remove_circle_outline`).
- **Yatış ayrıntısı diğer ayrıntılar gibi açılır** (2 Ekim 2026): ekranı
  kaplayan bir "yükleniyor" perdesi yoktur. İşlemler'den açılınca satırın
  taşıdıklarıyla (`PosDepositSummary`: yatan tutar, gün, hesap, kesinti,
  tahsilat sayısı) **hemen ve son boyutunda** çizilir; `Beklenen`, `Bakiye` ve
  kapattığı tahsilatlar `…` ile yerlerinde bekler, `Yatışı geri al` yüklenene
  kadar kapalıdır. Kasa'daki tahsilattan açılınca (özet yok) sabit yükseklikte
  küçük bir bekleme kutusu gösterilir. `AppLoadingView` bir panelin içinde
  çıplak kullanılmaz: ortalandığı için paneli tam yüksekliğe çıkarır.

## Kartla tahsil (5 Ekim 2026, Aşama 06.3 Grup 5 teslim 3/3)

Tasarım teslimi yok; mevcut dille kuruldu.

- **`CollectionMethodRail`** (`lib/features/pos/presentation/card_collection_fields.dart`):
  `Nakit / hesaba · Kartla (POS)` rayı, `AppSegmentRail` üstünde. Yalnız
  tahsilatta çıkar (cari tahsilat formu, `Yükümlülükler > Tahsil et ve kapat`);
  ödemede yoktur ("POS" yalnız satış tarafında, ADR 0019 T7).
- **`CardCollectionFields`**: kartla seçilince hesap alanının yerine gelir.
  POS seçici (ana POS seçili; `Elle gir` kendi grup başlığında) ve POS
  seçiliyken POS tahsilatı formundaki önizleme bloğunun aynısı (`Komisyon`,
  `Hesaba geçecek`, `Beklenen gün`, `Geçeceği hesap`); elle girişte hesap,
  beklenen gün ve `Yok · Tutar · Oran` komisyon. Tutar ve gün formun kendi
  alanlarıdır.
- **Kasa POS listesi**: tahsil satırının başlığı kişi, alt yazısı
  `Tahsilat · Hesaba geçecek · …`; ayrıntıda `Tahsil edilen` (brüt satış
  yerine), alt başlıkta `Kartla tahsil`, iptal düğmesi yerine "Tahsilatın
  parçası. İptal için tahsilatı kişinin hareketlerinden iptal edin."
- **Yatış ayrıntısı**: `Satış` ve `Tahsilat` iki ayrı satır (tutarlar
  sunucudan); tahsil yoksa yalnız `Satış`.
- **İşlemler**: satır `Ahmet → Garanti POS` (POS yoksa `hesap (kartla)`),
  tutarın altında `komisyon ₺…`; ayrıntıda `POS`, `Komisyon`, `Net tutar`,
  `Hesap`, `Beklenen`/`Geçiş günü`.
- **Gün sonu listesi**: kart tarafındaki tahsil `Kartla tahsil · POS`.
