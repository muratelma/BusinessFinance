# Handoff: Vergiler ekranı (Brif 1)

## Genel bakış
BusinessFinance Flutter Android istemcisine eklenecek **Vergiler** alt sayfası ve ona bağlı dokunuşlar. Kullanıcı (esnaf / şahıs şirketi) ödediği vergiyi tek tutarla yazar; isterse vergilerini tanımlayıp vadelerini takip eder, bekleyen kalemleri "Ödedim" ile kapatır. Ürün vergi **hesaplamaz**; yalnız kaydı tutar. Ödenene kadar hiçbir vergi bakiye, işletme neti veya bütçeyi etkilemez.

Brif metinleri: `brif/00-ortak-cerceve.md`, `brif/01-vergiler-ekrani.md`.

## Tasarım dosyaları hakkında
Bu klasördeki dosyalar **HTML ile hazırlanmış tasarım referanslarıdır** — görünüşü ve davranışı gösteren prototipler, doğrudan kopyalanacak üretim kodu değil. Görev bu tasarımları mevcut Flutter kod tabanında (`mobile/business_finance_mobile`), oradaki `lib/core/theme/` ve `lib/core/widgets/` bileşenleriyle yeniden kurmaktır. Web bileşen adları Flutter karşılıklarıyla birebir aynıdır (`AppCard`, `AppListRow`, `AppStatusChip`, `AppRowAction`, `AppBottomSheet`, `AppSubmitButton`, `AppSegmentedButton`, `AppSelectField`, `AppConfirmDialog` …).

`Vergiler.html` dosyasını tarayıcıda açın; pannable bir tuval üzerinde tüm çerçeveler yan yana durur.

## Sadakat
**Yüksek sadakat (hi-fi).** Renk, tipografi, boşluk ve yarıçaplar tasarım sistemi token'larından gelir (`_ds/.../tokens/`). Tuvaldeki veri sentetiktir (bugün = 29 Eylül 2026).

## Ekranlar

### V1 · İlk kullanım (boş)
- Tam ekran alt sayfa, geri oklu başlık `Vergiler`, alt çubuk yok.
- Tek kart (padding 24): 56 px kapsül `receipt_long`, başlık 18/600 "Ödediğiniz vergiyi tek tutarla yazın", yardımcı metin 15, kenarlı buton "Vergilerimi tanımla" (`event_repeat`).
- Varyant: Gider formundan girilmiş vergi varsa altta **Ödenenler** bölümü görünür.
- Sabit alt şerit: dolgulu "Vergi ödemesi ekle" (`add`), üstünde 1 px `--border`, zemin `--surface-canvas`.

### V2 · Dolu
Sıra: **Bekleyenler** kartı → **Vergilerim** → **Ödenenler**. Ana eylem her hâlde sabit altta (kaydırmayla kaybolmaz).
- Bekleyenler kartı: başlık "Gecikenler ve 30 gün" + `Gecikti` rozeti sayısıyla. Satır (min 72): tarih yaprağı (gecikmişte kırmızı) · ad "KDV · Ağustos" · durum rozeti (`Gecikti`/`Yaklaşıyor`) + göreli gün · tutar ya da "Tutar ödemede girilecek" · sağda **Ödedim** eylemi: çerçeveli hap buton, yükseklik 36 (dokunma alanı 48), yarıçap 999, 1 px `--border-strong`, zemin `--surface-card`, `check_circle` 18 px + "Ödedim" 14/600 `--ink`. Dokununca V3 paneli açılır; kaydedilince kalem Bekleyenler'den Ödenenler'e geçer (butonun seçili hâli yok). Kart altı gri şerit (`--surface-card-muted`), iki eşit hücre arada 1 px dikey ayırıcı: her hücrede sola yaslı etiket (14, `--ink-muted`) üstte, değer (16/600) altta: sol "30 günde çıkacak" + gider renkli tutar, sağ "Tutarı belli olmayan" + "2 ödeme".
- Vergilerim: 40 px kapsül + ad + "ritim · sıradaki tarih"; sağda tutar ya da rozet (`Şahsi`, `Duraklatıldı`). Başlık sağında "+ Ekle".
- Ödenenler: tarih yaprağı + ad + kaynak (toplu ödemede "Kapattı: …") + eksi tutar. Vergi işaretli kategorilerdeki tüm giderleri içerir.

### V3 · Ödedim paneli (bottom sheet)
Başlık "Bağkur · Eylül", alt başlık vade. Büyük tutar alanı (36/700, alt çizgi 2 px marka) tanımdan dolu; ödeme günü; "Nereden ödendi" seçimi. Kart seçilirse yardımcı: "Kart harcaması olarak yazılır; kart borcunuza eklenir." Gecikmişte tutar boş gelir, gecikme zammını kullanıcı yazar. Gönder: "Ödemeyi kaydet". Gider **ödeme gününe** yazılır.

### V4 / V5 · Vergi ödemesi ekle (toplu)
Tek tutar, tanım gerekmez. Bekleyen varsa "Bu ödeme hangilerini kapatıyor?" çoklu onay listesi; tutar kalemlere dağıtılmaz, seçilenler ödendi sayılır. Hiçbiri seçilmeden de kaydedilebilir. Bekleyen yoksa kategori salt-okunur "Vergi ödemesi".

### V6 · Vergilerimi tanımla (ilk kurulum)
Hazır türlerden çoklu seçim (onay kutulu 56 dp satırlar; alt satır ritim + kime uygun). Altta ayrı kart: "Kendi türüm". Sabit alt buton sayıyı yazar: "3 vergiyi ekle".

### V7 · Vergi tanımı formu
Sıra: **Ad** → **Kapsam** (segmented: `İşletme` · `Şahsi`; ipucu "Araç ve ev vergisinde şahsi seçilebilir.") → **Ritim** (seçim alanı; ilk ve varsayılan seçenek `Seçilen aylarda`, sonra Her ay / Üç ayda bir / Yılda bir) → Seçilen aylarda ise 12 ay çipi (6×2 ızgara, çoklu, seçili = marka dolgusu; 2.0× yazıda 4×3) → Gün (yardımcı metin sıradaki iki vadeyi yazar) → Tutar (isteğe bağlı) → Nereden ödenir (isteğe bağlı) → Başlangıç. Kaydet sabit altta.

### V8 / V9 · Ayrıntı panelleri (ortak sayfa düzeni)
Her ayrıntı paneli aynı dört katmanla okunur:
1. **Başlık:** 40 px kapsül + ad + "Vergi · İşletme" + kapat.
2. **Tutar + durum:** tutar (22/700, ödenmişte metrik boy eksi) solda, durum rozeti sağda.
3. **Ayrıntı bloğu** (`--surface-card-muted`, yarıçap 16, 48 px satırlar). Yalnız bilgi taşır.
   Başka sayfaya giden bağlantı bloğun **altında ayrı bir kartta** eylem satırıdır (ikon 22 + başlık + alt satır + chevron, min 64 px): V8 "KDV ayrıntıları" / "Her ay · 28'i"; V9 "İşlemlerde görüntüle" / "Bonus kartla ödendi · 31 Temmuz"; kapatılmışta "Kapatan ödemeyi aç" / "15 Eyl · Vergi ödemesi".
4. **Alt eylem çubuğu:** üstte 1 px `--border` ayırıcı; en fazla iki eşit genişlikte buton, ikincil kenarlı solda, birincil dolgulu sağda.

- **V8 · Bekleyen kalem:** "Tutar belli değil" (`--ink-muted`) + `1 gün gecikti` rozeti · Vade, Nereden ödenecek · KDV ayrıntıları kartı · alt çubuk [Tutarı gir] [Ödedim].
- **V9 · Ödenmiş kalem:** Ödeme günü, Nereden, Vade · İşlemlerde görüntüle kartı · alt çubukta tek kenarlı buton "Ödemeyi geri al" (`undo`, metin `--expense`, kenar `--expense-fill`) → `AppConfirmDialog` (destructive, vurgu "Motorlu taşıtlar · ₺2.180,00", mesaj "Gider iptal edilir; kalem yeniden bekleyene döner."). Kayıt silinmez, iptal edildi işaretlenir.
- **V9 · Toplu ödeme:** + Not satırı ve "Kapattığı kalemler" listesi; geri alma mesajı "…; kapattığı 2 kalem yeniden bekler."
- **V9 · Kapatılmış kalem:** alt çubuk yok; "Kapatan ödemeyi aç" kartı ve tek satır "Geri alma kapatan ödemeden yapılır."

### V10 · Vergi tanımı ayrıntısı
Başlık vergi adı, sağda "Düzenle" ikon butonu. Detay kartı (Ritim, Tutar, Nereden ödenir, Kapsam) → **Sıradaki** (3 vade) → **Geçmiş ödemeler** → eylem kartı:
- "Duraklat" (`pause_circle`, "Yeni kalem üretmez; istediğinizde sürdürürsünüz.")
- "Sil" (`delete`, başlık ve ikon `--expense`; ödenmiş kalemi varsa devre dışı: sağda `lock` ikonu, alt satır "Ödenmiş kalemleri olduğu için silinemez.")

### Başka ekranlara dokunuşlar
- **Özet · Yaklaşanlar kartı** (uygulamadaki mevcut zaman çizelgesi dili korunur): başlık "Yaklaşanlar", sağda "7 gün ›". Satır ızgarası `40px tarih | 20px nokta+çizgi | 1fr metin | auto tutar`, gün 20/700, ay 13/600; dikey çizgi 2 px `--border`, nokta 12 px halka. Tutarlar gider renginde, işaretsiz. Kart altı gri şerit: "7 günde çıkacak ₺…". Kartın altında açıklama metni **yok**.
  - **Gecikenler listeye girmez** (7 gecikmiş ödeme kartı doldururdu). Kartın en üstünde **tek özet satırı**: `--expense-container` zemin, `error` ikonu, "7 gecikmiş ödeme" + "En eskisi 12 Ağustos", toplam tutar (chevron yok; gecikenler Yaklaşanlar sayfasında zaten en üstte). Geciken yoksa satır hiç çizilmez. Geciken toplamı "7 günde çıkacak"a eklenmez.
- **Kategori formu:** "Vergi" anahtarı (AppSwitch) + "Bu kategorideki giderler Vergiler › Ödenenler'de görünür."
- **Tekrarlayan plan:** "Sıklık" seçim alanı, ilk seçenek `Seçilen aylarda`; seçiliyse aynı 12 ay çipi.
- Diğer menüsündeki "Vergi ve muhasebe" grubu uygulamadaki mevcut hâliyle **kalır** (değişiklik yok). Özet'e işletme neti notu **eklenmez**.

## Etkileşim ve davranış
- Bekleyen satırına dokunmak V8'i açar; satırdaki "Ödedim" doğrudan V3'ü açar.
- Ödenen satırına dokunmak V9'u açar.
- Vergilerim satırı → V10; "+ Ekle" → tek seçimli tür listesi → V7.
- Paneller alttan yükselir (Material varsayılanı ~200 ms, standard easing). Basılı durumda dokunma dalgası; ölçek animasyonu yok.
- Yıkıcı eylemler asla tek başına kırmızı dolgulu buton olarak durmaz: kart içinde satırdır ve sonuç alt satırda yazar; onay diyaloğu da sonucu yazar ("Emin misiniz?" yok).
- Loading / empty / error / unauthorized dördü `AppStateView` ailesiyle ele alınır; hata metni sunucudan gelir.
- Her ekran 2.0× yazı ölçeğinde taşmadan çalışmalı; dokunma hedefi ≥ 48 px, eylem satırı min 64 px.

## Durum / veri
- Bekleyen kalem: `{ taxDefinitionId, period, dueDate, amount?: decimal-string, status: pending|overdue|paid|closed }`.
- Ödeme: `{ amount, paidOn, sourceAccountId, closesItemIds[] }` → normal gider kaydı (kategori vergi işaretli).
- Tanım: `{ name, scope: business|personal, rhythm: selectedMonths|monthly|quarterly|yearly, months[], day: n|end, amount?, sourceAccountId?, startDate, paused }`.
- Tutarlar sunucudan 4 ondalıklı string gelir; istemci yalnız 2 basamağa yuvarlar, toplamı yeniden hesaplamaz.

## Tasarım token'ları
`_ds/design-system-02d8cd45-68c3-45e3-a947-241a11eb81d4/tokens/` altındaki `colors.css`, `typography.css`, `spacing.css`, `radius.css`. Özet: marka `#16191D`, zemin `#F7F8F9`, kart `#FFFFFF`, gelir metin `#137D3F`; boşluk 4·8·16·24·32·48; yarıçap kart 20, alan/buton 16, panel 28, çip 999; font Roboto; gölge yalnız panel/diyalog/FAB.

## Varlıklar
İkonlar Material Symbols Outlined (Flutter'da `Icons.*_outlined`): `receipt_long`, `health_and_safety`, `event_repeat`, `directions_car`, `signpost`, `edit`, `undo`, `swap_vert`, `pause_circle`, `delete`, `lock`, `check`, `add`, `chevron_right`. Görsel/illüstrasyon yok.

## Ekran görüntüleri
`screenshots/` — 412 px genişlik, kod ile birebir; **karşılaştırma için referans budur.** `01–21`: cihaz görünümü (412×892, açık paneller dahil). `tam-*`: kaydırmalı ekranların baştan sona tam boyu (V1, V2, V6, V7, V10). V7 formu ekrandan yalnız ~70 px taştığı için ayrı "alt" görüntüsü yok; tamamı `tam-10`'da. `19–21`: kart/alan kesitleri. Görüntüler ile kod arasında fark varsa ekran görüntüsü esastır.

## Dosyalar
- `Vergiler.html` — tuval, tüm çerçeveler
- `screens-v5/Vergiler.jsx` — Vergiler ekranları ve kesitler
- `screens-v5/Parts5.jsx` — ortak parçalar (Page, DetailBlock, CheckRow, AmountInput, MonthChips, ActionRow, ActionCard …)
- `screens-v2/Common.jsx`, `screens/Screens.jsx` — Son Tasarım'ın ortak dili (Row, DateLeaf, StatusTag, Capsule, TopBar …)
- `_ds/` — tasarım sistemi paketi ve token'lar
- `brif/` — ortak çerçeve ve Vergiler brifi
