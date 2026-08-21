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
| Hero metrik | `displaySmall` 34/700 | Ekranın söylediği tek sayı |
| Ekran başlığı | `headlineSmall` 24/700 | AppBar |
| Kart metriği | `titleLarge` 20/600 | Kart içi tutar |
| Bölüm başlığı | `titleMedium` 16/600 | `AppSectionHeader` |
| Satır başlığı | `titleSmall` 15/600 | Liste satırı |
| Gövde | `bodyLarge/Medium` 16/14 | Açıklama metni |
| Yardımcı | `bodySmall` 13 | Tarih, kaynak, alt satır |
| Etiket | `labelMedium/Small` 12/11, harf aralıklı | Metrik etiketi, rozet |

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
| `AppLoadingView` / `AppErrorView` / `AppEmptyView` / `AppUnauthorizedView` | Ortak durum ekranları |

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
- **`AppFormSheet` panelin kapanışını kendi üstlenir.** Panel içeriği
  `Navigator.pop` çağırmaz; sonucu döndürür, kabuk kapatır. `null` dönmek
  "doğrulama düştü, açık kal" demektir. Bu, kapanışın **tek** bir yerde
  olmasını sağlar ve aşağıdaki tuzağı ortadan kaldırır.
- **`AppFormSheet` üçüncü bir eylem taşıyabilir** (`secondaryLabel` /
  `onSecondary`). Filtre panelindeki `Temizle` bunun tek örneğidir:
  temizlemek vazgeçmekten farklıdır — vazgeçmek hiçbir şey değiştirmez,
  temizlemek boş filtreyi uygular.
- **`AppDateField` kendi görünümünü tanımlamaz.** `InputDecorator` ile tema
  dekorasyonunu okur; tarih alanı yanındaki metin alanlarından ayrı bir tür
  gibi görünmemelidir.

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

İlk kullanımı: açılışı kayıtsız borçlar (Aşama 12.8). O borç hesaba ve
raporlara hiç girmiyor ve bunu yalnız kullanıcı düzeltebilir.

### Ekran iskeleti

- Başlık **sayfanın kendisinden** gelir. Shell bir başlık daha çizseydi aynı
  ad ekranda iki kez görünürdü.
- Uzun listeler kart yüzeyine oturur, satır araları ince ayırıcıyla ayrılır.
  Her satırı ayrı karta koymak listeyi parçalıyordu.
- Kısa liste grupları (özet bölümleri) tek kart içinde toplanır.
- Durum ekranları (yüklenen / boş / hata / yetkisiz) tek iskeleti paylaşır:
  ikon kapsülü, başlık, açıklama, isteğe bağlı eylem.

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
| Türkçe Material metinleri | `test/widget_test.dart` |
| Ham renk / ölçek dışı boşluk / doğrudan genişlik karşılaştırması | `test/architecture/design_tokens_test.dart` |
| Dokunma hedefi, adlandırılmış hedef, metin kontrastı, 2.0× taşma | `test/helpers/accessibility.dart` + ekran testleri |

Yapısal kapı kaynak kodu tarar. Bunun nedeni, kuralın doğası: ihlal her zaman
henüz testi olmayan **yeni** bir dosyada ortaya çıkar, dolayısıyla widget
testleriyle korunamaz.
