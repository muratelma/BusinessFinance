# ADR 0007 — Akromatik Marka Rengi ve Kendi Yüzey Ölçeğimiz

- Durum: Kabul edildi; Aşama 12.7 Grup 1'de uygulandı
- Tarih: 2026-08-15
- Kapsam: `AppTheme`, `AppSurfaces`, bütün Flutter yüzeyleri

## Bağlam

Uygulama on iki aşama boyunca tek bir yeşil seed renginden (`#006C4C`)
türetilen Material 3 varsayılanlarıyla çalıştı. İki sorun birikti.

**1. Primary ile gelir aynı aileden görünüyordu.** Aşama 12.5'te sabitlenen
ürün kuralı açıktır: *yeşil yalnız gelirdir*. Ancak seed yeşil olduğu için
`colorScheme.primary` de yeşildi. Birincil buton ile bir gelir tutarı aynı
ekranda yan yana durduğunda ikisi de "yeşil" okunuyordu ve kuralın görsel
gücü zayıflıyordu. Kural koda yazılmıştı ama palet onu çürütüyordu.

**2. Kart zeminden ayrılmıyordu.** M3'ün `surfaceContainer*` merdiveni tonal
olarak tutarlıdır fakat komşu basamakları birbirine çok yakındır. Kart ile
sayfa zemini arasındaki fark göz tarafından ayırt edilemiyordu; ekran tek bir
düzlem gibi okunuyor, hiyerarşi yalnız boşluktan anlaşılıyordu.

## Karar

**Marka rengi akromatik olur** (`#16191D`; karanlık temada `#EDEFF2`).

Bu karara iki adımda varıldı ve ilk adım yeterli değildi:

1. Önce seed **lacivert** yapıldı (`#1E3A8A`, UI/UX Pro Max'in
   `Banking/Traditional Finance` paletinden). Bu, primary ile gelir yeşilinin
   aynı aileden görünmesini çözdü.
2. Fakat lacivert **üçüncü bir doygun renk ailesi** ekledi. Ekranda lacivert
   buton, yeşil gelir ve kırmızı gider aynı anda yarışıyordu; hiçbiri diğerine
   göre öne çıkmıyor ve renk bütünlüğü kurulmuyordu. Kullanıcı bunu doğrudan
   bildirdi ve referans olarak akromatik bir finans uygulaması gösterdi.

Akromatik marka rengiyle ekranda **anlamlı renk yalnız ikidir**: yeşil gelir,
kırmızı gider. Renk artık dekorasyon değil, yalnız bilgi taşır. Yüzeyler de
mavimsi griden nötr griye çevrildi ki tek renkli şeyler para göstergeleri
olsun.

Marka rolleri (`primary`, `secondary` ve container'ları) elle veriliyor: tonal
üretici akromatik bir seedden bile hafif renkli tonlar çıkarıyor ve o tonlar
gelir/gider renkleriyle yarışıyordu.

Yeşil, gelire iade edilir.

**Yüzeyler kendi uzantımızdan gelir** (`AppSurfaces`): `canvas`, `card`,
`cardMuted`, `border`, `borderStrong`, `overlay`. `ColorScheme`'in yüzey
alanları bu değerlerle geçersiz kılınır, böylece `Card` gibi hazır bileşenler
de aynı yüzeyleri kullanır.

**Kart zeminden kenarlıkla ayrılır, gölgeyle değil.** Gölge yalnız gerçekten
yüzen katmanlara ayrılmıştır: kayan eylem butonu, bottom sheet, dialog
(`AppElevation.flat / raised / floating`).

## Gerekçe

- Kenarlık tonal farktan daha güvenilirdir: açık temada da koyu temada da
  aynı işi yapar ve kontrastı ölçülebilir.
- Her kartın gölge taşıması derinlik bilgisini anlamsızlaştırır; her şey
  yüzüyorsa hiçbir şey yüzmüyordur.
- Gölge mobilde bedava değildir; kenarlık boyama maliyeti getirmez.

## Sonuçlar

### Kazanılan

- "Yeşil = gelir" kuralı artık paletle çatışmıyor ve ekranda onunla yarışan
  üçüncü bir renk yok.
- Kart, liste ve panel hiyerarşisi tek yerden yönetiliyor.
- Yeni palet 12.6'nın kontrast kapısından geçmek zorunda kaldı ve kapı bunu
  ilk çalıştırmada denetledi (aşağıya bakınız).

### Ödenen bedel

- Yüzeyleri elle vermek, Material'ın gelecekteki tonal iyileştirmelerinden
  otomatik faydalanmayı bırakmak demek. Karşılığında ölçülebilir ve
  öngörülebilir bir hiyerarşi alındı.
- İki palet (aydınlık/karanlık) elle bakımlanıyor.

### Kapıların yakaladıkları

Palet değişikliği iki gerçek hatayı anında ortaya çıkardı:

| Hata | Ölçülen | Kaynak |
|---|---|---|
| Gider rozeti yeni açık zeminde görünmez | 1,20:1 | `expenseContainer` eski zemine göre seçilmişti |
| Seçili filtre chip'inin etiketi okunmuyor | 1,45:1 | Seçili zemin `primary` yapılmış, etiket rengi onunla değişmiyordu |

İkisi de düzeltildi: container tonu doygunlaştırıldı, seçili chip zemini
`primaryContainer` oldu.

## Bu kararın dışında kalan

- Özel font paketi. `google_fonts` ağdan indirir; bu uygulama yerel ve
  çevrimdışı çalışacak biçimde kuruldu. Tipografi hiyerarşisi boyut, ağırlık
  ve harf aralığıyla kuruldu.
- Kullanıcının tema seçmesi. `ThemeMode.system` olduğu gibi kalır.
- Marka kimliği çalışması (logo, isim, ikon seti). Bu ADR yalnız uygulama içi
  renk ve yüzey sistemini kapsar.
