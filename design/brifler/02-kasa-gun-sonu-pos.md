# Brif 2 — Kasa sekmesi, gün sonu ve POS

`00-ortak-cerceve.md` ile birlikte okunur. Başlangıç noktası birlikte tasarladığımız **KasaV4**'tür
(kasa seçici, bugünkü sayım kartı, POS tahsilatları, Son sayımlar, Sayımı gir paneli, POS detay paneli).
Bu brif Kasa'yı **gerçek bir esnaf gününe** göre yeniden kurar.

## Neden

Uygulamayı bir haftalık esnaf senaryosuyla denedik. Gördüklerimiz:

- Esnaf satışları tek tek girmez; akşam **gün sonu toplamını** (ya da yazar kasanın Z raporunu) bilir.
  Satışlar girilmeyince kasa sayımı bir haftanın satışını "fazla" gösterdi.
- POS parası bankaya **eksik** yatınca fark hiçbir yere yazılamadı; birkaç günün parası tek seferde yatınca
  her kayıt tek tek işaretlendi.
- Her akşam aynı POS formunda aynı beş seçim (~15 dokunuş).
- Müşteri veresiye borcunu kartla ödediğinde bunu yazmanın yolu yoktu.

## Değişmez kurallar

1. **Gün sonu yeni bir kayıt türü değildir.** Var olan kayıtları üretir: nakit satış → kasaya **gelir**;
   kartla satış → **POS tahsilatı** (gelir bugün, para yolda). Gün sonu bir bütün olarak **geri alınır**.
2. **Aynı satış iki kez sayılmaz.** Gün içinde tek tek girilmiş satışlar ve kartla tahsilatlar gün sonunda
   hesaba katılır.
3. **Yatış ve kartla tahsil taşır:** bakiyeyi değiştirir, gelir yazmaz. Beklenenle yatan arasındaki fark
   ayrı bir **kesinti** gideridir.
4. **Yoldaki para bir hesap değildir;** net varlığa dahildir, kullanılabilir bakiyeye dahil değildir.
   İki kaynağı var: POS satışı ve kartla tahsil.
5. **Kasada tek gerçek hesap bakiyesidir.** "Kasada olması gereken" = kasanın hesap bakiyesi. Sayım bir
   gözlemdir; kaydetmek bakiyeyi değiştirmez, fark kaydı isteğe bağlıdır.
6. **Hiçbir akış fotoğrafa ya da POS'a bağlı değildir.** POS'u olmayan, elle kasa defteri tutan esnaf da
   aynı gün sonunu elle kullanır.
7. **Kelimeler:** satış tarafında "kartla satış", "Kartla gelecek", "POS". Harcama tarafında "kredi kartı",
   "kartla ödedim" — orada "POS" geçmez. Tahsilat tarafında "kart" kelimesi tek başına kullanılmaz (aynı
   ekranda borçlandığın kredi kartıyla karışır).
8. Kasa sekmesinde **işletme kapsamlı ve kapsamsız** nakit hesaplar görünür; **şahsi etiketli** nakit
   hesaplar (şahsi cüzdan) görünmez. KasaV4'teki `Dükkan kasası | Şahsi cüzdan` seçicisi bu yüzden değişir:
   yalnız işletme/kapsamsız kasalar; tek kasa varsa seçici hiç yok.

## Kasa sekmesinin yapısı

Dört parça: **Bugün · Nakit · Kartla gelecek · Son günler**. KasaV4'te "sekme yok, tek akış" kararı
vermiştik. **Önerim: tek akışta dört bölüm** (sekme değil). Sekmeli bir alternatifi de yan yana çizip
karşılaştır.

### Bugün
- Kart başlığı `29 Eylül Salı` + durum etiketi: `Gün sonu girilmedi` (gri) / `Kapatıldı` (yeşil tik).
- Girilmemişken: gün içinde tek tek girilmiş satışlar bilgi olarak (`Gün içinde girilen · nakit ₺1.250,00 ·
  kartla ₺800,00`) ve birincil eylem **Gün sonunu gir**.
- Girilmişken: Nakit satış · Kartla satış (POS başına satır) · Toplam; meta olarak `Z 3143`. Dokununca gün
  ayrıntısı.
- Atlanan Z varsa ince bir uyarı: *"Z 3142 girilmedi."* (yalnız Z numaralı günler arasında; kaydı denetler,
  cihazı değil).

### Nakit
- Kasa seçici (gerekiyorsa) ve **Kasada olması gereken** hero tutarı (= hesap bakiyesi).
- Son sayım satırı: `Son sayım · 27 Eylül · Tuttu` ya da `… · ₺85,00 eksik`.
- **Kaydedilmemiş son fark ayrı bir satırda** durur (*"27 Eylül sayımında ₺85,00 eksik, kaydedilmedi"*) ve
  sonraki sayımda yeniden sorulmaz.
- Eylemler: **Kasayı say** · Kasadan öde (gider formu, kasa seçili açılır) · Bankaya yatır (transfer,
  kaynak kasa) · **Kendime aldım**. Dört eylemin hiyerarşisini sen öner (ekranda tek birincil eylem var:
  Bugün kartındaki "Gün sonunu gir" ile yarışmamalı).

### Kartla gelecek
- `Yolda` toplamı + *"Net varlığa dahildir"*; satırlar: POS adı ya da müşteri adı (kartla tahsil),
  `Hesaba geçecek · 30 Eylül`, sağda net (mavi, işaretsiz). Gecikenler `Gecikti` etiketiyle.
- Eylem: **Hesaba geçenleri işaretle** (yatış paneli). Bölüm başlığında `POS'larım ›`.
- Tek POS tahsilatı girişi buradan (`+ Ekle`) yapılır.

### Son günler
- Gün satırı: takvim yaprağı · `Satış ₺4.380,00` ve altında `nakit ₺2.100 · kartla ₺2.280` (başka bir günün örneği) · sağda sayım
  sonucu (`Tuttu` / işaretli fark / `Sayılmadı`). Dar ekranda kapsül yok (KasaV4 dersi).
- Dokununca gün ayrıntısı. `Tümü ›`.

## Çizilecek çerçeveler

### K1 · Kasa — akşam, gün sonu girilmemiş (dolu)
### K2 · Kasa — gün sonu girilmiş, bir kaydedilmemiş fark ve 3 yolda kayıt var
### K3 · Kasa — ilk kullanım (POS tanımı yok, hiç sayım yok)
Kartla gelecek bölümü "POS'unuzu tanımlayın" daveti olur; gün sonu yalnız nakitle çalışır.

### K4 · Gün sonu paneli — elle giriş
Başlık `Gün sonu · 29 Eylül` (gün değiştirilebilir). Üstte **"Z raporundan oku"** (kamera) girişi.
- **Satış tutarları:** Nakit · her POS tanımı için bir satır (ör. `Garanti POS`, `Yemek kartı`) · Toplam.
  **Üçünden ikisini yazmak yeter, üçüncüsü hesaplanır** — hesaplanan alan bunu belli etmeli.
  Birden çok POS varsa kart tutarı varsayılan olarak ana POS'a yazılır.
- **Zaten girilmiş kayıtlar:** o gün tek tek girilmiş kayıtlar onay kutusuyla, başlık *"Gün sonu tutarında
  var mı?"*. Varsayılan: satışlar (gelir ve POS, faturalı dahil) ve kartla tahsilatlar **işaretli**
  (düşülür); nakit cari tahsilat **işaretsiz** (çoğu zaman yazar kasadan geçmez). Her satırda varsayılanın
  ne olduğu görünür olmalı.
- **Yazılacaklar özeti:** *"Nakit satış ₺2.100,00 → Dükkan kasası'na gelir"*, *"Kartla satış ₺1.480,00 →
  Garanti POS, komisyon ₺29,60, 30 Eylül'de hesaba geçecek"*. Formül açık: `Nakit 3.350 − girilmiş 1.250
  = 2.100`, `Kart 2.680 − girilmiş POS 800 − kartla tahsil 400 = 1.480`.
- Birincil eylem **Gün sonunu kaydet**.

### K5 · Gün sonu paneli — Z'den okunmuş
Aynı panel; okunan alanlar bir işaretle (`Z'den okundu`), Z no görünür, değerler düzeltilebilir. Okunan
değer bir **öneridir**; kullanıcı onaylar.
- **TOP farkı:** NAKİT + KART, Z'deki TOP'tan farklıysa `AppInlineNotice`: *"NAKİT ve KART toplamı TOP'tan
  ₺450,00 farklı. Genelde faturalı satış ya da veresiye tahsilatıdır; ayrıca kaydedilmez."* TOP hiçbir
  kayıt üretmez.
- **Toplu Z** (birkaç günlük): *"26–28 Eylül · 3 gün"*; toplam son güne yazılır (değiştirilebilir);
  aralıktaki günler kapalı sayılır. Aralık ay dönümünü geçerse uyarı ve **"İki aya böl"** (iki tutar).

### K6 · Gün zaten kapatılmış
Aynı güne ikinci gün sonu: *"29 Eylül'ün gün sonu girildi (Z 3143). İkinci bir cihazın gün sonuysa 'Ek gün
sonu' olarak kaydedin."* Aynı Z no ikinci kez: kaydetme engellenir, mevcut kayda bağlantı.

### K7 · Gün ayrıntısı + gün sonunu geri al
O günün gün sonunun ürettiği kayıtlar, sayım, Z no. **"Gün sonunu geri al"** → onay: *"Bu gün sonunun
yazdığı 3 kayıt birlikte iptal edilir ve gün yeniden açılır."* (Gün sonundan gelen tek bir kayıt
İşlemler'den tek başına iptal edilemez; geri alma buradan yapılır.)

### K8 · Yatış paneli — "Hesaba geçenleri işaretle"
- Yoldaki kayıtlar onay kutusuyla (geçeceği hesaba göre gruplu), seçilenlerin **beklenen** toplamı canlı.
- **Hesaba geçen tutar** (bankada gördüğü gerçek tutar) ve **geçtiği gün**.
- Fark varsa: *"Kesinti ₺38,40 · Banka ve POS komisyonu gideri olarak yazılır."*
- Birincil eylem **İşaretle**. Onay metni sonucu söyler.
- Yatan tutar beklenenden **fazlaysa** ne olacağı henüz kararlaştırılmadı; bu hâli nötr bir notla çiz,
  kural kodda belirlenecek.

### K9 · Yatış ayrıntısı + geri alma
(İşlemler'den ve Kasa'dan açılır.) Yatan tutar, gün, hesap, kapattığı kayıtların listesi, kesinti satırı.
**"Yatışı geri al"** → *"Kapattığı 4 kayıt yeniden yolda görünür; kesinti gideri iptal edilir."*
Yatışa bağlı bir POS kaydı, yatış geri alınmadan iptal edilemez; POS ayrıntısında bunu söyle.

### K10 · POS'larım listesi + POS tanımı formu
Liste: ad, geçeceği hesap, oran, `1 iş günü`; ana POS işaretli. Form:
- Ad · Geçeceği hesap · Varsayılan komisyon oranı · **Komisyon kategorisi** (varsayılan
  "Banka ve POS komisyonu", kendiliğinden dolu) · Geçiş süresi (gün) · **"Hafta sonu hesaba geçmez"**
  anahtarı (tatil takvimi yok; beklenen gün her kayıtta elle düzeltilebilir).
- Yemek kartı ayrı bir özellik değil, bir POS tanımıdır (ör. `Yemek kartı · %6 · 15 gün`).

### K11 · Tek POS tahsilatı formu (tanımdan dolu)
Hedef: **tutar + kaydet.** POS seçimi (ana POS seçili), tutar; altında canlı `Komisyon ₺16,00 · Net
₺784,00 · 30 Eylül Çarşamba hesaba geçecek`. Hesap, kategori, oran ve gün katlanmış "Ayrıntılar" altında.

### K12 · Kasayı say (KasaV4'teki panel)
Toplamı yaz | Banknotla say aynen kalır. Değişen: beklenen = hesap bakiyesi; **fark kaydı isteğe
bağlıdır** ve kaydedilirse sebep sorulur: `Girilmemiş gider · Kendime aldım · Bilmiyorum`. Sayım sıklığı
ayarı (`Her gün · Haftada bir · İstediğimde`) — yerini sen öner.

### K13 · Kendime aldım
Tutar + *"Nasıl yazılsın?"*: **Şahsi hesabıma aktardım** (transfer; hedef şahsi hesap) · **Şahsi
harcamam** (şahsi gider; kategori). Varsayılan transfer; şahsi hesap yoksa varsayılan şahsi gider ve
*"Şahsi cüzdan aç"* bağlantısı. Her seferinde sorulur.

### K14 · Kartla tahsil (cari hesap tahsilatı ve alacak kapatma formlarında)
*"Nasıl ödendi?"*: `Nakit / hesaba · Kartla (POS)`. POS seçilince: POS seçimi, canlı net ve gün, kural
metni *"Gelir yazılmaz; alacak kapanır ve para yolda görünür. Hesaba geçince Kasa'dan işaretlersiniz."*

## Başka ekranlara dokunuşlar

- **İşlem ekle paneli:** işletme profilinde `Diğer` altındaki **POS tahsilatı** satırının yerini
  **Gün sonu** alır (tek POS tahsilatı Kasa › Kartla gelecek'ten girilir).
- **Yaklaşanlar:** beklenen POS girişi `girecek` satırıyla görünür (mavi, taşıma); `7 günde çıkacak`
  toplamına girmez.
- **İşlemler:** yatış kendi satırıyla görünür (mavi, taşıma; ikon önerisi: bankaya giren para); ayrıntıda
  K9'daki içerik. Gün sonundan gelen satışlarda köken bilgisi (`Gün sonu · Z 3143`).

## Senden karar önerisi beklediğim yerler

1. **Tek akış mı, sekmeler mi?** (Öneri: tek akış, dört bölüm.)
2. **Nakit bölümündeki dört eylemin hiyerarşisi** ve "Gün sonunu gir" ile yarışmaması.
3. **Z'den okunan alanın işareti** — nasıl belli olsun, düzeltilince ne olsun?
4. **"Üçünden ikisi" alanları** — hesaplanan alan nasıl görünsün (kilitli mi, gri mi, dokununca yazılabilir mi)?
5. **Sayım sıklığı ayarının yeri.**

## Örnek veri (sentetik; 29 Eylül 2026 Salı akşamı)

- Kasalar: **Dükkan kasası** (işletme) — olması gereken ₺23.185,00; son sayım 27 Eylül, ₺85,00 eksik,
  kaydedilmedi. Şahsi cüzdan Kasa'da görünmez.
- POS'lar: **Garanti POS** (ana; Dükkan hesabı; %2; 1 iş günü; hafta sonu geçmez) · **Yemek kartı**
  (%6; 15 gün).
- Gün içinde girilenler: 11:20 nakit satış ₺1.250,00 · 14:05 Garanti POS ₺800,00 · 16:40 Ayşe Yılmaz
  veresiye tahsilatı nakit ₺600,00 · 17:15 Mehmet Kaya veresiye tahsilatı kartla ₺400,00.
- Z raporu (Z 3143): NAKİT ₺3.350,00 · KART ₺2.680,00 · TOP ₺6.480,00.
- Yolda: 25 Eyl Cuma Garanti ₺2.450,00 net → 28 Eyl Pazartesi bekleniyordu, **Gecikti** (hafta sonu
  atlandığı için Cumartesi'ye değil Pazartesi'ye düştü) · 28 Eyl Garanti ₺1.960,00 net → bugün · 29 Eyl
  kartla tahsil Mehmet Kaya ₺392,00 net → 30 Eyl.
- Bugün yazılacaklar (Z'den): nakit satış ₺2.100,00 · kartla satış ₺1.480,00 (komisyon ₺29,60). Ayşe'nin
  nakit tahsilatı işaretsiz kalır, Z'nin TOP farkı ₺450,00.
