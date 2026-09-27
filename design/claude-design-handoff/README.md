# Handoff: BusinessFinance — ana ekranlar (Flutter)

## Genel bakış
BusinessFinance Flutter Android istemcisi (`mobile/business_finance_mobile`) için yeniden tasarlanan ana ekranlar:
Özet, Bütçeler, İşlemler + işlem detayı, İşlem ekle paneli, Kasa (gün sonu sayımı + POS), Diğer, Hesabım.
Form sayfaları (gelir/gider/transfer formları) bu paketin **dışında**; sonra tasarlanacak.

## Tasarım dosyaları hakkında
Bu paketteki dosyalar **HTML ile yapılmış tasarım referanslarıdır** — görünüşü ve davranışı gösteren prototipler,
doğrudan kopyalanacak üretim kodu değil. Görev: bu tasarımları **mevcut Flutter kod tabanında**, oradaki
`lib/core/theme/` token'ları ve `lib/core/widgets/` bileşenleriyle (AppCard, AppListRow, AppMoneyText,
AppStatusChip, AppFormSheet, AppSectionHeader, AppStateView…) yeniden kurmak. JSX'teki `DS.AppX` bileşenleri
Flutter'daki aynı adlı widget'ların web karşılığıdır.

Açmak için: `Son Tasarim.html` dosyasını bir tarayıcıda açın (yerel sunucu gerekebilir: `npx serve .`).
Tuvalde her telefon çerçevesi bir ekran/durumdur; çerçeveler tıklanabilir.

## Doğruluk düzeyi
**Yüksek doğruluk (hi-fi).** Renkler, tipografi, boşluk ve yarıçaplar son hâlidir ve tasarım sisteminin
token'larından gelir. Ölçüler 412 dp genişliğe göredir. Sayılar/metinler **örnek veridir**.

## Genel kurallar (bütün ekranlar)
- Kabuk: dikeyde çentikli alt çubuk (Özet · İşlemler · [+] · Kasa · Diğer). Ortadaki + "İşlem ekle" panelini açar. Sayfaların kendi FAB'ı yok.
- Başlık: sayfanın kendisinden, 26/700, harf aralığı -0.6. Sağ eylem ikonları 48×48 dokunma alanı.
- Kart: beyaz `#FFFFFF`, 1 px `--border #E5E7EA`, yarıçap 20, gölge yok. Sayfa zemini `#F7F8F9`.
- Kart içi satır listesi: ayırıcı yazının başladığı yerden (ikon kapsülü 40 px + 16 boşluk → inset 72).
- **Alttan açılan paneller (bottom sheet) telefonun en altından açılır ve alt çubuğun ÜSTÜNE biner** (showModalBottomSheet, `useRootNavigator: true`). Karartma rgba(16,17,20,.42–.5).
- Para her zaman `AppMoneyText` (sabit genişlikli rakam, `₺23.185,00`); gelir `+` yeşil `#137D3F`, gider `-` kırmızı `#CF2E1F`, transfer mavi `#0B6AD6`.
- **Durum etiketi (yeni, `StatusTag`)**: liste satırı ve kart köşesindeki durumlar için dolgusuz **ikon 16 + metin 13/600**, rol renginde (Tuttu yeşil, Eksik kırmızı, Fazla/Yolda mavi `--neutral`, Sayılmadı gri `--planned #44515C`). Dolgulu kapsül (`AppStatusChip`) yalnız sayfada tek ve önemli bir durum için (ör. detayda "İptal edildi").
- **Metin eylemi (`TextAction`)**: bölüm başlığı sağındaki eylem; dolgusuz, 14/600 `--ink`, ikon 18, dokunma yüksekliği 48 (`+ Ekle`, `Tümü ›`).
- **Kart başlık şeridi (`CardHead`)**: yükseklik 52, yatay 16, altında 1 px `--border`; solda 15/600 başlık (+ isteğe bağlı 14 gri meta), sağda durum etiketi.

## Ekranlar

### 1. Özet (`screens-v4/OzetV4.jsx` → `OzetV4`)
Üstten alta: başlık + avatar (e-posta doğrulanmamışsa kırmızı nokta) · kapsam anahtarı `Hepsi · İşletme · Şahsi`
(kaydırılan gövdenin DIŞINDA, sabit) · ay seçici · gecikmiş ödeme şeridi (`--expense-container`, 52 yükseklik) ·
Gelir/Gider/Net kartı · Kategori giderleri (halka 104 + satır altı pay çubukları) · Bütçeler (halkalar; 4+ bütçe
→ 2×2, 2–3 → ikili, 1 → tek; en dolu olanlar) · Yaklaşanlar · 7 gün (zaman çizelgesi + "7 günde çıkacak" toplamı) ·
Varlık durumu · Hesap bakiyeleri.

**Kapsam davranışı (önemli):** Kapsam değişince **gecikmiş ödeme sayısı, gelir, gider, net, kategori giderleri,
bütçeler ve yaklaşanlar** o kapsama göre değişir. Varlık durumu ve hesap bakiyeleri **değişmez**; kapsam seçiliyken
"Kapsam filtresinden etkilenmez: işletme ve şahsi toplamıdır." notu gösterilir. Hepsi'de net kartının altında
İşletme neti / Şahsi net kırılımı görünür; İşletme/Şahsi'de görünmez.
Uygulamada: istemci hesaplamaz — her bölüm sunucudan `scope=business|personal|null` parametresiyle hazır toplam
ister (ADR 0013: tek havuz, kapsam bir raporlama boyutu). Tasarımdaki örnek veri `OZ_SCOPE` sabitindedir.

### 2. Bütçeler (`screens-v3/OzetV3.jsx` → `BudgetsV3`)
Özet'teki Bütçeler bölümünden açılır; geri oklu başlık, her bütçe kartında harcanan / limit, pay çubuğu, durum.

### 3. İşlemler (`screens-v4/IslemlerV4.jsx` → `IslemlerSon`)
Arama alanı · tür filtre çipleri · planlananlar şeridi · güne göre gruplu akış (gün başlığı + tam genişlik beyaz
blok, satırlar inset 72). Satıra dokununca **işlem detayı** paneli (`TxnDetailV4`): başlık + tür·kapsam, büyük
işaretli tutar + durum, gri blokta Tarih / Kategori (transferde Hesaplar A → B) / Hesap / Not satırları,
altta "İşlemi iptal et" (iptal edilemeyen türlerde gerekçe metni). İptal edilmiş kayıt: satır `opacity .55` +
"İptal edildi" rozeti, tutar üstü çizili gri. İptal onayı: *"Kayıt silinmez; iptal edildi olarak işaretlenir ve toplamları artık etkilemez."*

### 4. İşlem ekle paneli (`screens-v2/IslemEkleV2.jsx` → `IslemEkleV2`)
Başlık "İşlem ekle". Üstte 3 eşit kutucuk (Gelir / Gider / Transfer): yükseklik 96, yarıçap 20, zemin rol
kapsül rengi (`--income-container #C7E8D4`, `--expense-container #FBD9D5`, `--neutral-container #CFE2FA`),
ortada ikon 24 (`south_west`, `north_east`, `swap_horiz`) + 14/600 etiket, `on-*-container` renginde.
"Belgeden oku": 2 kutucuk (Fiş veya fatura, Banka dekontu), gri zemin, 64 yükseklik. "Diğer": POS tahsilatı,
Ödenmemiş fatura, Kart borcu öde, Tekrarlayan işlem — ikon kapsülü + başlık + tek satır açıklama + chevron.

### 5. Kasa (`screens-v4/KasaV4.jsx` → `KasaV4`)
Sekme YOK; tek akış.
- **Kasa seçici** (`KasaPicker`): iki dilimli ray (gri zemin, 1 px kenar, iç boşluk 4, yarıçap 16); her dilim 56 yükseklik, ikon 20 (`storefront`/`wallet`) + ad 14/600 + altında bakiye 13 gri. Seçili dilim beyaz + `--border-strong` kenar.
- **Bugünkü sayım kartı**: `CardHead` "25 Eylül Perşembe" + sağda durum (Sayılmadı / Tuttu / Eksik / Fazla / Fark kaydedildi). Gövde: "Uygulamaya göre kasada" + hero tutar 36/700; ince çizgi; Dünkü sayım / Bugün nakit giriş (+) / Bugün nakit çıkış (−) satırları (36 yükseklik). Birincil düğme "Sayımı gir". Sayımdan sonra: "Elde sayılan" hero, Uygulamaya göre + Fark satırları, yan yana eşit iki düğme **Yeniden say** (kenarlı) · **Farkı kaydet** (dolu) + kural metni.
- **POS tahsilatları** bölümü: başlık sağında `+ Ekle`. Kart: `CardHead` "Yolda · hesaba geçmedi" + "N tahsilat"; toplam 22/700 + "Net varlığa dahildir"; satırlar: yoldakiler mavi saat kapsülü, "Hesaba geçecek · 27 Eylül", sağda net; geçenler yeşil tik, gri tutar.
- **POS detay paneli** (`KasaPosSheet`): net tutar 28/700 + durum; gri blokta Brüt satış / Komisyon (−) / Beklenen gün / Geçeceği hesap; kural metni; "Hesaba geçti" düğmesi.
- **Son sayımlar** (`KasaHistoryRow`): başlık sağında `Tümü ›`. Satır: tarih yaprağı 44×48 · sayılan tutar (row) + altında "Beklenen ₺…" · sağda fark: "Tuttu" (yeşil ikon+metin) ya da işaretli tutar 600 + altında "Eksik · kaydedildi". **Kapsül kullanılmaz** (dar ekranda taşıyordu).
- **Sayımı gir paneli** (`KasaCountSheet`): alt başlık "Dükkan kasası · 25 Eylül" · segment `Toplamı yaz | Banknotla say` · ortada "Elde sayılan" etiketi + büyük tutar (36/700; toplam modunda altı 2 px marka çizgili giriş alanı) · canlı sonuç şeridi (rol kapsül zemini: Tuttu yeşil / ₺85,00 eksik kırmızı / fazla mavi; altında "Uygulamaya göre ₺…") · banknot modunda ₺200/100/50/20/10/5 satırları: değer · −/+ adım düğmeleri (44 daire) · satır tutarı; en altta "Madeni para" tutar kutusu · kural metni *"Sayım bir gözlemdir; kaydetmek bakiyeyi değiştirmez."* · "Sayımı kaydet". Not: toplam alanına Türkçe binlik biçimlendirici (23.100) eklenmeli.

### 6. Diğer (`screens-v2/DigerV2.jsx` → `DigerV2`)
Üstte Hesabım kartı (avatar 48, "Hesabım", e-posta, doğrulanmadıysa rozet) → Hesabım'ı açar. Altında gruplar:
Para ve hesaplar · Planlama · Vergi ve muhasebe (yalnız işletmesi olanlarda) · Ayarlar. Her grup bir kart, satırlar ikon kapsülü + başlık + chevron.

### 7. Hesabım (`screens-v4/HesabimV5.jsx` → `HesabimV5`) — kaynak: `lib/features/account/presentation/account_page.dart`
Özet'teki avatar ve Diğer'deki Hesabım kartı buraya açılır (tam ekran, geri oklu, alt çubuk yok). Mevcut sayfaya göre değişiklikler:
1. Kimlik kartı: avatar 56 + e-posta 17/600 + "Hesap açılışı · 14 Mart 2026"; **doğrulama uyarısı ayrı kart değil, bu kartın içinde** `AppInlineNotice` + "Adresimi doğrula".
2. "Tercihler" başlığı altında "İşletmem var" satırı + anahtar; alt metin duruma göre değişir.
3. "Açık oturumlar · N" başlığı + sağda **"Diğerlerini kapat"** toplu eylemi; satırlarda göreli zaman ("7 gün önce açıldı · 18 Ekim'e kadar geçerli"); bu cihazda kapatma düğmesi yok.
4. Güvenlik: yalnız "Parolamı değiştir".
5. **Çıkış yap** ayrı, tam genişlik kenarlı düğme.
6. "Hesabı kapat" kartı: önce "Önce yedeğinizi alın" (Diğer › Veri ve yedek'e gider), sonra kırmızı "Hesabımı sil" satırı → onay → parola paneli (mevcut akış).
Onay metinleri sonucu söyler (dosyada `AppConfirmDialog` metinleri).

## Durumlar
Her ekranda loading / empty / error / unauthorized mevcut `AppStateView` ailesiyle ele alınır (tasarımda yalnız dolu hâl çizildi). Hata metinleri sunucudan gelir.

## Tasarım token'ları
`_ds/.../tokens/*.css` — Flutter'daki `lib/core/theme/` ile birebir. Özet:
- Renk: ink `#16191D`, ink-muted `#5C6572`, ink-faint `#646D7A`, canvas `#F7F8F9`, card `#FFFFFF`, card-muted `#F0F1F3`, border `#E5E7EA`, border-strong `#CBCFD4`; income `#137D3F` / fill `#209E54` / container `#C7E8D4` / on `#0C4A26`; expense `#CF2E1F` / `#E5544A` / `#FBD9D5` / `#651A12`; neutral `#0B6AD6` / `#2589FA` / `#CFE2FA` / `#0A3F80`; planned `#44515C` / container `#DDE3E8`.
- Boşluk: 4 · 8 · 16 · 24 · 32 · 48; fab-clearance 96.
- Yarıçap: kart 20, alan/düğme 16, panel/dialog 28, çip 999.
- Tipografi (Roboto): hero 36/700 (-0.8), başlık 26/700, metrik 22/700, bölüm 18/600, satır 16/600, gövde 16 & 14, yardımcı 14, etiket 13/600 (+0.4).
- Gölge yalnız panel/dialog/FAB'da.

## İkonlar
Material Symbols Outlined (Flutter `Icons.*_outlined` karşılıkları, aynı adlar). Seçili gezinme hedefi dolgulu.

## Dosyalar
- `Son Tasarim.html` — bütün son ekranların tuvali (giriş noktası).
- `screens-v4/` — OzetV4, IslemlerV4 (IslemlerSon, TxnDetailV4), KasaV4, HesabimV5.
- `screens-v2/` — Common (TopBar, Row, RowList, Capsule, DateLeaf, ScopeTabs, StatusTag, TextAction, CardHead), IslemEkleV2, DigerV2, IslemlerV2 (yardımcılar).
- `screens-v3/` — OzetParts (halka, yaklaşanlar), OzetV3 (BudgetsV3).
- `screens/Screens.jsx` — örnek veri (`DATA`).
- `_ds/` — tasarım sistemi token'ları ve bileşen paketi.
- HesabimV4.jsx ve KasaV2.jsx yalnız bağımlılık; eski sürümlerdir.

## Claude Code'a önerilen komut
> "design_handoff_business_finance/README.md'yi oku ve Son Tasarim.html'i referans al. Önce Kasa ekranını
> `lib/features/cash/presentation/` altında mevcut widget'larla yeniden kur; yeni ortak widget'ları (StatusTag,
> CardHead, TextAction) `lib/core/widgets/` altına ekle. Sonra sırayla Hesabım, İşlem ekle paneli, İşlemler, Özet."


## Ekran görüntüleri (`screenshots/`)
Her biri `Son Tasarim.html` tuvalindeki bir çerçeve, 2× (828×1788). Ölçü için HTML esas alınır; görüntüler hedef hâli gösterir.
01-ozet-hepsi · 02-ozet-isletme · 03-ozet-sahsi · 05-butceler · 06-islemler · 07-islem-detay-gider · 08-islem-detay-transfer · 09-islem-detay-iptal · 10-islem-ekle · 11-kasa · 12-sayim-toplam · 13-sayim-banknot · 14-kasa-sayimdan-sonra (1×) · 15-pos-detay · 16-diger · 17-hesabim · 18-hesabim-alt
Özet'in alt kısmı (bütçe, yaklaşanlar) için görüntü yok; `Son Tasarim.html` içindeki "Özet · alt" çerçevesine bakın.
