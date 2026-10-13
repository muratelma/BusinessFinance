# BusinessFinance — Tasarım Sistemi

**Kaynak:** [github.com/muratelma/BusinessFinance](https://github.com/muratelma/BusinessFinance)
(dal `main`). Bu sistem tamamen o repodan okundu; en çok
[`documentation/design-system.md`](https://github.com/muratelma/BusinessFinance/blob/main/documentation/design-system.md),
[`mobile/business_finance_mobile/lib/core/theme/`](https://github.com/muratelma/BusinessFinance/tree/main/mobile/business_finance_mobile/lib/core/theme)
ve `lib/core/widgets/` ile `lib/features/*/presentation/` altındaki ekranlardan.
Daha derine inmek — form sayfaları, kredi kartı ekranı, fiş okuma akışı — isteyen
okuyucu doğrudan o repoya bakmalı; buradaki her karar orada bir gerekçeyle
birlikte yazılı (`documentation/adr/`).

## Ürün nedir

**Şahıs şirketi ve esnaf için işletme finansı uygulaması.** ASP.NET Core + EF Core
+ SQL Server bir monolit ve **tek istemci: Flutter Android uygulaması**
(`business_finance_mobile`). Marketing sitesi, web uygulaması veya ikinci bir
yüzey yok — bu yüzden bu sistemde tek UI kit var.

Kurucu kararlar, çünkü arayüzün yarısı bunlardan çıkıyor:

- **İşletme ve şahsi tek havuzda yaşar**; ayrım bir raporlama boyutudur (ADR 0013).
  Şahıs şirketinin tüzel kişiliği olmadığı için işletmenin kasası ile sahibinin
  cebi aynı ceptir. Arayüzdeki karşılığı tek bir kapsam anahtarıdır:
  `Hepsi · İşletme · Şahsi`.
- **Ürün vergi hesaplamaz, beyanname üretmez, muhasebe kârı hesaplamaz**;
  muhasebeciye giden veriyi hazırlar. `Kâr` kelimesi hiçbir ekranda geçmez.
- **İstemci parayı ikinci kez hesaplamaz.** Toplamlar sunucudan dört ondalıklı
  string olarak gelir; ekran yalnız iki basamağa yuvarlayıp gösterir.
- **Uygulama Türkçedir** ve tek dillidir; para ₺, tarihler `10 Ağustos 2026`.

Ana ekranlar: Özet (dashboard), İşlemler (birleşik hareket akışı), Kasa
(gün sonu sayımı + POS tahsilatları) veya Bütçeler, ve Diğer (tek kartta on dört kapılı menü).

## Kaynakta olmayan şeyler

- **Logo yok.** Repoda yalnız Flutter'ın varsayılan launcher ikonu var. Marka
  öğesi olarak giriş ekranındaki `account_balance_wallet` ikonu ve düz tipte
  yazılmış `BusinessFinance` adı kullanılıyor. **Logo çizilmedi.**
- **Özel font yok.** `AppTypography`: "Yeni font paketi eklenmez". Flutter
  Android istemcisi platform varsayılanını kullanır, yani Roboto. Bu sistem de
  Roboto'yu Google Fonts'tan yükler — bu bir ikame değil, aynı fontun web
  karşılığıdır. Gerçek bir marka fontu varsa gönderin, tek satırda değişir.
- **Görsel, illüstrasyon, arka plan deseni yok.** Ürün baştan sona nötr
  yüzeyler ve tipografi; `samples/documents/` altındaki fiş fotoğrafları ürün
  görseli değil, test verisidir ve kopyalanmadı.

## İÇERİK TEMELLERİ (content fundamentals)

**Dil Türkçe, ton sakin ve doğrudan.** Uygulama kullanıcıya ne olduğunu söyler,
onu övmez ve heyecanlandırmaya çalışmaz.

- **Cümle kasası.** Başlıklar da butonlar da BÜYÜK HARFE ÇEVRİLMEZ: Türkçede
  `i/İ` dönüşümü tuzaklıdır ve ekran okuyucu büyük harfli metni farklı okur.
  Ayrım harf aralığı ve ağırlıkla yapılır. `Kaydet`, `Tekrar dene`, `Öde`.
- **Kişi zamiri neredeyse yok.** Etiketler adlandırır (`İşletme neti`,
  `Yaklaşanlar`, `Vadesi geçmiş alacak`), cümleler gerektiğinde ikinci tekil
  nazik biçimi kullanır (`Devam etmek için lütfen yeniden giriş yapın.`,
  `İlk gelir veya giderinizi ekleyebilirsiniz.`). "Biz" hiç kullanılmaz.
- **Emoji kullanılmaz.** Hiçbir ekranda, hiçbir bildirimde. Unicode süs
  karakteri de yok; tek istisna yön anlatan `→` (`Nakit kasa → Ziraat işletme`)
  ve ayırıcı `•` / `·`.
- **Kelimeler para gibi seçilir.** `Kâr` yasak (muhasebe kârı ürünün dışında).
  `Şahsi çekim` ile `Şahsi net` sayının yönüne göre değişir — artı bir sayıya
  "çekim" demek onu eksi gibi okuturdu. `Kart borcu` negatifse satır
  `Kart alacağı` olur. `Alacak` "sana girecek, gelir değil"; `Borç` "senden
  çıkacak, gider değil".
- **Onay metni sonucu yazar, soru sormaz.** "Emin misiniz?" gibi neyin olacağını
  söylemeyen bir kalıp kullanılmaz. Örnek:
  *"Kayıt silinmez; iptal edildi olarak işaretlenir ve toplamları artık
  etkilemez."*
- **Yardımcı metin kuralı yazar.** `Ad (isteğe bağlı)` + *"Boş bırakılırsa
  kategori adı kullanılır."*; borç satırında *"faiz hariç"*; POS satırında
  *"Kartla satış; para birkaç gün sonra hesaba geçer"*.
- **Eksik bilgi hata gibi anlatılmaz.** `AppInlineNotice` nötr tondadır: veri
  geçerli, yalnız yarım. Kullanıcı korkutulmaz, eksiği kapatan eylem yanına
  konur.
- **Boş durumda bir sonraki adım vardır:** *"Henüz işlem yok — İlk gelir veya
  giderinizi ekleyebilirsiniz."* Hata metinleri **sunucudan** gelir ve
  istemcide yeniden yazılmaz.
- **Sayı ve etiket birlikte konuşur.** Bir bölüm sayı gösteriyorsa penceresini
  de yazar (`Yaklaşanlar · 7 gün`), kapsam filtresinden etkilenmiyorsa bunu
  söyler (*"Kapsam filtresinden etkilenmez: işletme ve şahsi toplamıdır."*).

## GÖRSEL TEMELLER (visual foundations)

**Marka rengi akromatiktir** (`#16191D`; karanlık temada `#EDEFF2`) ve bu estetik
değil yapısal bir karardır (ADR 0007): renkli bir marka rengi, gelir yeşili ve
gider kırmızısıyla birlikte ekranda üç doygun renk ailesi kuruyor ve hiçbiri öne
çıkmıyordu. **Ekranda anlamlı renk yalnız ikidir** — yeşil gelir, kırmızı gider —
ve renk yalnız bilgi taşır, dekorasyon değildir.

- **Renk.** Her finansal rolün tam olarak iki tonu var: metin tonu (`--income`,
  4.5:1) ve dolgu tonu (`--income-fill`, 3:1), artı kapsül çifti
  (`--income-container` / `--on-income-container`). Alfa'dan ton türetilmez.
  Nötr rol **mavidir**, mor değil. Kategori dilimleri ayrı hue'lardır ve rol
  renkleriyle kasten farklı ton ailesindedir (kategori yeşili `#679612`, gelir
  yeşili `#137D3F`).
- **Tipografi.** Tek aile (Roboto), hiyerarşi boyut/ağırlık/harf aralığıyla.
  Hero metrik 36/700, ekran başlığı 26/700, kart metriği 22/700, bölüm başlığı
  18/600, satır başlığı 16/600, gövde 16 ve 14, yardımcı 14, çip 14/600, etiket 13
  (harf aralıklı). Ölçüler gerçek cihaz ekran görüntülerinden (411 dp genişlik) ölçüldü. Para **sabit genişlikli rakam** kullanır; hero'da ek ağırlık
  ve sıkı aralık.
- **Mürekkep merdiveni.** Üç okuma kademesi: `--ink` (satır başlığı, tutar,
  başlık), `--ink-muted` (tarih, kaynak, açıklama), `--ink-faint` (bölüm
  etiketi, yer tutucu, sayaç). Üçü de AA eşiğini geçer; soluk olmak silik olmak
  değildir. Renkli zeminde bu merdiven kullanılmaz, `on*Container` geçerlidir.
- **Zemin ve arka plan.** Yüzeyler **nötr gri**, mavimsi değil. Sayfa zemini
  `#F7F8F9`, kart `#FFFFFF`. **Gradyan yok. Görsel yok. Doku, desen, gürültü
  yok. Full-bleed görsel yok. Koruma gradyanı yok** — çünkü altında görsel yok.
- **Kart.** Zeminden **kenarlıkla** ayrılır (1 px `--border`), gölgeyle değil;
  yarıçap 20 px. Seçili kart 2 px marka kenarlığı alır. Kartın kendi kenar
  boşluğu sıfırdır; boşluğu (`--space-sm`) listeyi kuran verir.
- **Gölge.** Yalnız gerçekten yüzen katmanlarda: kayan eylem butonu (raised),
  bottom sheet ve dialog (floating). Kart, panel ve liste **düz**. Her şey
  yüzüyorsa hiçbir şey yüzmüyordur. **İç gölge (inner shadow) hiç yok.**
- **Yarıçap.** Kart 20, form alanı ve buton 16, panel/dialog 28, çip ve rozet
  tam yuvarlak (999). Ara değer uydurulmaz.
- **Boşluk.** 4 · 8 · 16 · 24 · 32 · 48. Ölçek dışında iki sabit var ve ikisi de
  yerleşim: `--fab-clearance` 96 (listenin altındaki pay), `--nav-notch` 72
  (çentiğin genişliği).
- **Şeffaflık ve bulanıklık.** Yok. Tek yarı geçirgen yüzey modal panelin
  arkasındaki karartmadır; `backdrop-filter` hiçbir yerde kullanılmaz. Solukluk
  yalnız iptal edilmiş satırda (`opacity: .55`) ve her zaman bir rozetle
  birlikte.
- **Animasyon.** Material'in kendi varsayılanları: panel alttan yükselir,
  dokunma dalgası (ink ripple), sekme geçişi. Özel easing eğrisi, bounce, spring
  veya dekoratif giriş animasyonu **tanımlı değil** — bir tasarımda animasyon
  gerekiyorsa Material standardı (~200 ms, standard easing) varsayılır ve
  ekranda hiçbir şey kendiliğinden hareket etmez.
- **Hover.** Dokunmatik ürün; hover Material'in yüzey vurgusudur (çok hafif
  koyulaşma), renk değişimi değil. Web'e taşırken: kart ve satırda `--surface-card-muted`
  düzeyinde bir vurgu, buton ve çipte `opacity` değil ton değişimi.
- **Basılı durum.** Dokunma dalgası; **küçülme (scale) yok**, renk sıçraması yok.
- **Odak.** Form alanı odakta 2 px marka kenarlığına döner. Dokunma hedefi her
  zaman en az 48 px; rozet ve satır eylemi görünürde 32 px, dokunulan alan
  büyüktür.
- **Yerleşim.** Başlık sayfanın kendisinden gelir; kabuk ikinci bir başlık
  çizmez. Telefon dikeyde çentikli alt çubuk sabittir ve o sekmelerde sayfanın
  kendi kayan butonu olmaz. Geniş ekranda çentik yoktur, birincil eylem gezinme
  rayının başındadır ve içerik 720 px ile sınırlanır. Kapsam anahtarı
  **kaydırılan gövdenin dışındadır**.
- **Grafikler.** Üç soru, üç biçim: bir dönem nasıl bölündü → halka; dönemler
  nasıl değişti → dönem başına tek net çubuk; bir kalem toplamın ne kadarı →
  satır altı pay çubuğu. Grafikler asla tek başına bilgi taşımaz: her dilim
  efsanede adıyla ve tutarıyla yazılır.
- **Erişilebilirlik, tasarımın parçası.** Hiçbir bilgi yalnız renkle taşınmaz
  (her durum ikon **ve** metinle); her ekran 2.0× yazı ölçeğinde taşmadan
  çalışır; kontrast eşiği 4.5:1 (metin dışı 3:1); loading/empty/error/
  unauthorized her ekranda görünür ele alınır.

## İKONOGRAFİ (iconography)

- **Set: Material Icons** — Flutter'ın gömülü seti. Repoda özel ikon fontu, SVG
  sprite'ı veya PNG ikon **yok**; `Icons.receipt_long_outlined` gibi doğrudan
  Material adları kullanılıyor. Bu sistem web karşılığı olan **Material Symbols
  Outlined**'ı Google Fonts'tan yükler ve ikonlar Flutter'daki adla çağrılır
  (`receipt_long`, `south_west`, `point_of_sale`). Bu bir ikame değil, aynı
  setin web dağıtımıdır.
- **Varsayılan biçim çizgili (outlined).** Dolgulu varyant yalnız **seçili**
  gezinme hedefinde kullanılır; seçim ayrıca yazı kalınlığıyla da bildirilir.
- **Boyutlar.** Satır ikonu 20 (40 px kapsül içinde), satır içi meta ve efsane
  18, kapsül/rozet ikonu 16, gezinme 24, durum ekranı 32 (72 px kapsül), giriş
  ekranındaki marka ikonu 64.
- **İkon ancak ayırt ediyorsa bilgi taşır.** Her hesabın cüzdan, her kategorinin
  aynı ikonu taşıması listeyi okunmaz yapıyordu. Hesap ikonu **sunucudan gelen
  türden** (`cash` → `payments`, `bank` → `account_balance`), kategori ikonu
  **kanonik kategori adından** (`groceries` → `shopping_basket`, `bills` →
  `receipt_long`) türetilir; kullanıcının kendi kategorisinde ada bakılır
  (`fatura`, `yakıt`, `kira`…), o da tutmazsa nötr `local_offer`.
- **İkon-yalnız her eylemin erişilebilir adı vardır** (tooltip veya
  `semanticLabel`). Dekoratif ikon ekran okuyucudan gizlenir.
- **Emoji ikon olarak kullanılmaz.** Unicode karakter de ikon yerine geçmez.

## Kasıtlı eklemeler (intentional additions)

Kaynak envanterinde bileşen olarak yoktur, tasarım sisteminde vardır:

- **`Icon`** — Material glif sarmalayıcısı. Flutter'da `Icons.*` sabitleri
  kullanılıyor; web'de bir bileşene ihtiyaç var.
- **`AppTextField`** — Flutter tarafında Material'in `TextFormField`'ı temanın
  `inputDecorationTheme`'ini okuyor; o dekorasyonun web karşılığı.
- **`AppNavBar`** — `main_shell.dart` içindeki `_NotchedNavigationBar`; orada
  ekran koduna gömülü, burada yeniden kullanılabilir bileşen.
- **`AppStateView`** — `app_state_views.dart` içindeki özel `_StateScaffold`;
  dört durum ekranının ortak iskeleti.
- **`AppSelectField`** ve **`AppSegmentedButton`** — Flutter tarafında Material'in
  `DropdownButtonFormField` ve `SegmentedButton`'ı temanın dekorasyonuyla
  kullanılıyor; web karşılıkları.

Kaynakta olup burada **olmayan**: `AppAdaptiveSheet` (panelin hangi
Navigator'da açıldığına dair bir Flutter davranışı; web'de karşılığı yok) ve
`AppLocale` (yerelleştirme delegesi).

## Bileşenler

`components/core/` — **AppCard**, **AppSectionHeader**, **AppMenuGroupLabel**,
**AppResponsiveGrid**, **AppContentWidth**, **Icon**

`components/money/` — **AppMoneyText**, **AppMetricTile** (+ `Formatters.jsx`:
`MoneyText`, `DateText`, `formatMoney`, `formatDayMonth`, `formatMonthYear`,
`formatPercent`, `editableMoney`)

`components/lists/` — **AppListRow**, **AppStatusChip**, **AppRowAction**,
**AppKeyValueList**, **AppExpansionTile**

`components/forms/` — **AppFormSheet**, **AppFormField**, **AppTextField**,
**AppDateField**, **AppSelectField**, **AppSegmentedButton**, **AppSubmitButton**,
**AppScopeSwitch**, **AppScopeField**, **AppScopeDefaultField**,
**AppScopeChoiceChip**, **AppFilterChips**, **AppSwitch**

`components/feedback/` — **AppInlineNotice**, **AppStateView**,
**AppEmptyView**, **AppErrorView**, **AppUnauthorizedView**, **AppLoadingView**,
**AppConfirmDialog**, **AppBottomSheet**

`components/charts/` — **AppDonutChart**, **AppTrendChart**, **AppShareBar**

`components/navigation/` — **AppNavBar**, **AppMonthPicker**, **AppTabBar**,
**AppExtendedFab**

Her dizinde bileşenlerin `.d.ts` props sözleşmesi, `.prompt.md` kullanım notu ve
Design System sekmesine düşen bir kart dosyası var.

## Dizin

| Yol | Ne var |
|---|---|
| `styles.css` | Tek giriş noktası; yalnız `@import` satırları |
| `tokens/` | `fonts`, `colors`, `typography`, `spacing`, `radius`, `elevation`, `base` |
| `components/` | Yeniden kullanılabilir bileşenler (yukarıdaki gruplar) |
| `ui_kits/mobile/` | Flutter istemcisinin tıklanabilir yeniden yapımı — `index.html` |
| `guidelines/` | Renk, tipografi, boşluk ve marka örnek kartları |
| `thumbnail.html` | Ana sayfadaki kutucuk |
| `SKILL.md` | Claude Code / Agent Skills uyumlu giriş |
| `github.md` | Kaynak repo bağlantısı ve son eşitleme kaydı |

## Yeni ekran eklerken

1. Boşluklar `--space-*`, yarıçaplar `--radius-*` adlarından; ham değer yok.
2. Anlamlı renk finansal rollerden, yüzey rengi yüzey token'larından.
3. Tutarlar **her zaman** `AppMoneyText`, tarihler `DateText`'ten geçer.
4. Durumlar `AppStatusChip` ile — ikon **ve** metin.
5. Satırda durum ve eylem aynı yerde, aynı boyda; satır içinde dolgulu buton yok.
6. Form paneli `AppFormSheet`, alanlar `AppFormField`; gönderim `AppSubmitButton`.
7. Loading / empty / error / unauthorized dördü de görünür ele alınır.
8. Ekran başına tek birincil eylem; alt çubuk varsa sayfanın kendi FAB'ı olmaz.
