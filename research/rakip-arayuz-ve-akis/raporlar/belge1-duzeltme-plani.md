# Belge 1 — düzeltme turu planı

**Durum:** uygulandı (24 Eylül 2026). Kullanıcı tam yetki verdi; K1–K11 §9'daki önerilerle
uygulandı. Sonuç ve plandan ayrılan kararlar §12'de.

**Girdi:** [belge1-hazirlik.md](belge1-hazirlik.md): Aşama 0 (kaynak doğrulama), kullanıcının
23 Eylül kararları (1–10) ve Aşama 1 (bölüm bölüm okuma). Satır satır değişiklik tabloları
orada; bu plan onları özetler, seçer ve sıraya koyar.
**Hedef:** `belge1/tam/belge1.pdf` (bugün 98 sayfa, 145 kare). Belge arayüz belgesi olarak kalır;
aynı kalıp ve motorla, yeni kapılarla yeniden üretilir.

---

## 1 · Turun amacı ve yöntemi

Belge 1 17 Eylül kanıtıyla yazıldı. 20 ve 22 Eylül koşumları ile 23 Eylül kullanıcı gözlemleri
yeni kanıt getirdi. Tur, bu kanıtın kapattığı "görülmedi"leri ve ortaya çıkan yanlışları düzeltir
(karar 1). Çıkarım katmanı ve Belge 3 soruları eklenmez (karar 2); rozet yapısına dokunulmaz
(karar 8).

Yöntem Belge 2 turunun aynısı: kareden değil sayfadan başlandı. Belge 1'in 13 bölümü baştan
okundu, her yeni bulgu etkilediği **sayfa ve cümleye** bağlandı. Kare yalnız bir sayfa onu
gerçekten istiyorsa aday yapıldı (§6).

İkinci tarama (kullanıcının sorusu üzerine): Belge 1'in **hiç anmadığı** 227 envanter karesi
ve Aşama 1'de anılmayan 14 envanter dışı kare de okundu. 17 kare açıldı; bir yanlış ve 20
güncelleme daha çıktı (hazırlık dosyası, "Aşama 1 ek").

| Tür | Anlamı | Sayı |
|---|---|---|
| **D** · yanlış | Basılı bir ifade kanıtla ya da belgenin başka bir sayfasıyla çelişiyor | 12 |
| **G** · güncelleme | "Görülmedi / denenmedi / doğrulanmadı" diyen bir yer artık kanıtlı; ya da sayfanın sorusuna doğrudan cevap veren yeni bulgu | ~100 |
| **T** · temizlik | Üretim artığı dil, "Çıkarılmayan sonuç" satırları, tarih, "koşum" sözcüğü (karar 5, 9, A1) | bölüm başına 5–10 |
| **Y** · yapı | Bölüm 1–2 birleşmesi (karar 6) | 1 |

## 2 · Doğrulama: koşum özeti yine kareden fazlasını söylüyor

Belge 2 turundaki ders ("koşum özetine güvenme, kareyi aç") burada da tuttu. Aşama 0'da 34,
Aşama 1 ve 2'de 8 kare daha açıldı. Belge 1'e dayanak yapılmadan önce ortak listede düzeltilecekler:

| ID | Ortak listede | Karede | Sonuç |
|---|---|---|---|
| **WL-19** | "İki tutar yazımının nedeni: kapandı" | `f7-96`: `Number format` tek anahtar, "Use decimals within amounts". `f7-106` kapalıyken tutarlar ondalıksız. Belge 1 4.1'in sorduğu fark ise **para kodu ve ayraç** (TRY 20,800.00 ↔ ₺20.800,00) | Belge 1 için **kısmen.** 4.1'in "nedeni doğrulanmadı" cümlesi doğru kalır; ayarın yalnız ondalığı değiştirdiği eklenir |
| **BC-16** | "CSV/HTML çıktısının üretimi: kapandı" | Seçenek listesi ve ürünün kendi cümlesi kareli; dosya üretilmedi (gözlem formu satır 586 da böyle diyor) | Belge 1'in eksiği (dosyanın üretimi) **açık kalır** |
| **WL-14** | "altında Add Record ve Manage debt" | `f7-84`: Debt Records, Total ₺5.000,00, tek satır, altta Add Record. Manage debt yok | "Manage debt" yazılmaz |
| **MM-01** | "sağ alttaki simge" | Simge tuş takımının sağ sütununda; sağ alttaki düğme "Bitti" | Konum düzelir |
| **BC-16** | Alıntı "Excel (csv)" | Karede "Excel (cvs)" | Alıntı karedeki yazımla birebir |

Ek: `gozlemler/bluecoins.md` satır 579, bölünmüş kaydın listesini "`f7-73` (E0444)" diye
gösteriyor. E0444 çıktı seçimi karesi; liste `f7-65`.

Aynı karelerde ortak listede olmayan üç yan bulgu çıktı; önce gözlem formuna yazılır (G2):

- `f7-96`: `Active module after launch` ve `Initial day of the month: 1` aynı ekranda.
- `f7-65` (E0437): "Minimum balance — Get notified when balance drops below this amount" (açık,
  0.00) ve "Enable automatic imports".
- `f7-84`: tahsilattan sonra borcun kayıt listesinde yalnız tahsilat satırı var (Ada Reklam → Me,
  +5.000).

## 3 · Yapı: Bölüm 1 ile 2 birleşiyor (karar 6)

**Yeni Bölüm 1 · Giriş ve incelenen ürünler**

| Alt bölüm | İçerik | Nereden |
|---|---|---|
| 1.1 · Giriş | Belgenin amacı; dokuz ürün ve hangi kanıtla bulundukları (bugünkü s.3'ün sağ altındaki kutu); belgenin neyi ölçmediği (karar 5-i cümlesi); sınır cümlesi (Belge 2, Belge 3) | Bugünkü s.3 girişi, 1.4'ün "neyi ölçmez" ve "sıradaki belgeler" blokları |
| 1.2 · Canlı incelenen ürünler | Beş ürün kartı, fotoğraflı. Kartta sürüm, erişim, kare sayısı, gözlenen yaklaşım, kapsam | Bugünkü 2.1 kartları + 1.1 tablosunun sürüm ve erişim sütunları |
| 1.3 · Kaynakla incelenen ürünler | KolayBi (görselli) ve üç metin kartı. Girişte karelerin neden basılmadığı ve "kaynaktan kurulan akış modeli Belge 2 Bölüm 9'dadır" | Bugünkü 2.2 ve KolayBi kartı, 1.3'ün blokları |

**Kalkanlar:** 1.1 tablosu (içeriği kartlara geçer), 1.2 "Kanıt türleri", 1.4 "Sayfalar nasıl
okunur" (karar 6: yazılmaz), bütün koşum tarihleri (A1), "Kişisel veri" bloğu (üretim ayrıntısı).

**Bölüm 3–13, 2–12 olur.** Bugünkü 104 iç gönderme ("→ N.M") betikle kaydırılır. Aşama 1
tablolarındaki numaralar bugünkü numaralardır.

**Sayfa etkisi:** bugün s.3–12 (10 sayfa) → tahmini 6 sayfa (giriş 1, canlı kartlar 3, kaynak
kartları 2). Belge toplamı 98 → **93 ± 3**.

## 4 · Bölüm bölüm değişiklikler

Satır satır tablo: `belge1-hazirlik.md` "Aşama 1" altında, bölüm başlıklarıyla. Burada yalnız D
satırları ve en önemli G'ler var. Numaralar bugünküdür; parantezde yeni numara.

### Bölüm 1–2 (→ 1) · Giriş ve ürünler

- **D** KolayBi "31 destek görseli basılıyor"; basılan 17 (not 1). Sayı turdan sonra yeniden
  sayılır.
- **G** Money Manager kartı "bütçe kurulumu inceleme dışında" → bütçe kareli (MM-05). Goodbudget
  kartı "yedek inceleme dışında" → Zarflar ekranında Last Backup göstergesi (Belge 1'in kendi
  karesi E0115'te zaten okunuyor).
- **T** Kartlardaki koşum tarihleri kalkar (not 8 kapanır). Hesap Defterim kartında "Kaynaklarda
  belirtilen odak" → "Gözlenen odak" (not 6). "Canlı incelenen ürünler — 2" başlığı (not 5) ve
  KolayBi'nin yanlış grubu (not 4) yeni düzende kendiliğinden kapanır.

### Bölüm 3 (→ 2) · Gezinme ve ana ekran

- **D** "Sağ altta tek yuvarlak düğme" (üç yer; not 10). Bluecoins ve Wallet'ınki köşeli.
- **D** 3.1 "ilk ekranda hesap isteyen Goodbudget": ilk ekran yalnız "hanen var mı?" diye soruyor;
  e-posta ve parolayla hane kaydı bütçe kurulup zarflar doldurulduktan sonra geliyor ve LATER
  taşıyor. Yeni haneden sonraki ilk ekran Setup Budget (E0109, E0110).
- **G** Bluecoins soğuk açılışta Hesaplar sekmesiyle açılıyor (BC-06). Hesap Defterim son
  kullanılan defterle açılıyor (HD-03). Bluecoins çekmecesindeki iki Hesaplar kaleminin hedefi
  (BC-07). Eksik listesinden iki satır düşer. MM'nin yardımcı araçları Daha sekmesinde tek
  ızgarada (E0254, 11.4'te zaten basılı).

### Bölüm 4 (→ 3) · Görsel dil

- **D** Sözcükle durum "yalnız Bluecoins'te"; Wallet'ın plan ayrıntısında da var (not 18).
- **G** Goodbudget gelir rengi kareli (GB-01). Wallet'ın iki tutar yazımı: ayar yalnız ondalığı
  değiştiriyor, asıl farkın nedeni hâlâ bilinmiyor (WL-19, §2). Bluecoins'te ayrı bir Durum alanı
  (dört değer). Goodbudget'ın tarih sırası ve ondalık işareti bir ayar (E0453); Hesap Defterim'de
  "Para birimi biçimi" ayarı (E0150).
- **T** 4.4 başlığındaki "seçili filtre" (not 15; K5). Şekil 4.8 kırpması (not 14).

### Bölüm 5 (→ 4) · İşlem ekleme

- **D** "Silme iki üründe onay istiyor" → onayın karesi beş üründe (not 3; MM-11, WL-17).
- **D** 5.4 "tutar dört üründe"; hangi dört belirsiz (not 11). Ürün adları yazılır.
- **G** Varsayılan tür: Bluecoins ve Money Manager kareli (BC-01, `47`). MM tutar tuş takımı ve
  tam ekran hesap makinesi (MM-01). Goodbudget'ın hesap makinesi bir ayar (GB-04). MM sıfır tutarı
  uyarısız kabul ediyor (g1). Money Manager, Wallet ve Goodbudget'ın incelenen menülerinde geri
  alma kalemi yok (MM-11, WL-17, GB-07).

### Bölüm 6 (→ 5) · Hesap, kart, transfer

- **D** "(→ Belge 2 §3)" → "(→ Belge 2 2.1)".
- **G** Wallet'ın hesap formu: yeni hesap ücretsiz pakette açılamıyor, düzenleme formunda açılış
  alanı yok (WL-02). Payment Due Date ayın günü seçicisi (WL-18). Wallet uyarısının olası kaynağı
  Minimum balance ayarı (bağ karede kurulmuyor, cümle bunu söyler). Bluecoins transfer ücreti
  kendi tutarı, hesabı ve kategorisi olan ayrı bir blok (g2); Money Manager havale formunda Harç.

### Bölüm 7 (→ 6) · Sınıflandırma

- **D** "Bluecoins ve Wallet'ta Türkçe arayüzde" (not 2).
- **G** Bluecoins etiket seçici ve İş etiketli kayıt (BC-04). Wallet Labels denemesi (WL-07).
  KolayBi Ortaklar sekmesi resmî kaynakla: cari kartının alt türü (WB-01). Wallet'ta Automatic
  rules: kategori ve etiketi otomatik atama (E0378; 7.5'te QuickBooks satırının canlı karşılığı).
  KolayBi gider kategorileri ve tipleri (E0193).

### Bölüm 8 (→ 7) · Plan, tekrar, bekleyen

- **G** Money Manager: kurulumda tek onay, örnek başına onay yok; ne zaman uygulanacağı bir ayar
  (g3). Bütçe kategori başına, Toplam sekmesinde (g4). Bluecoins taksitleri kendiliğinden
  gerçekleşmiyor, her biri hatırlatıcıdan kaydediliyor (g5, kullanıcı kontrolü). Wallet ana ekran
  kartı yüklenmiş hâlde (WL-01); hedef 0 / 20.000 (WL-20). Bluecoins Bütçe Özeti ayrıntısı (BC-18).
  15 Eylül kareleri E kimliği aldı (E0399, E0400). MM'nin Ayarlar'daki Tekrarlayan İşlemler
  listesi (E0416). Bluecoins'te bütçe kurulumunun girişi Kategoriler'deki "Bütçe Kur" (E0087).
- **T** 8.6'da Goodbudget'ın 1 numaralı işareti Total yerine Last Backup satırının yanında.

### Bölüm 9 (→ 8) · Borç ve tahsilat

- **D** Wallet "kayıttan önce ve sonra iki soru" → "formdan önce ve kaydederken" (not 9).
- **G** Debt action iki değer (WL-12). Bluecoins formunda fatura bağlama alanı yok, Durum dört
  değer, ek yalnız ataçla (BC-03). Tahsilattan sonra borcun kayıt listesi (`f7-84`). Wallet'ta
  borç iki yönle başlıyor (I Lent · I Borrowed), vade alanının varsayılanı bir yıl sonrası
  (E0358, E0362). Bluecoins cari hesap formunda vade alanı yok (E0449).

### Bölüm 10 (→ 9) · Rapor ve dönem

- **G** MM pastadan kategori ayrıntısı (MM-02). Bluecoins "Net Kazançlar" ve "Net Kazanç" iki blok
  (BC-12; 10.2'nin sorusunun tam örneği). Wallet dönem seçicisinin üç sayfası (WL-08). Hesap
  Defterim çiplerinde Önceki denge satırı (HD-05). Wallet filtre formu (WL-15). Bluecoins filtre
  kaydetme sayfası (BC-13). MM filtrede toplamın anında değişmesi (MM-06). MM'nin takvim
  görünümü (E0234; 10.1b bugün takvimi yalnız Bluecoins ve HD için anlatıyor). Goodbudget'ta işlem
  araması (E0127).
- **T** "KDV matrisi" → "KDV raporu" (B2).

### Bölüm 11 (→ 10) · Veri aktarımı

- **D** d1: 11.1 işaret 4, Hesap Defterim'in "kasadefteri klasörü" sözünü olgu gibi yazıyor
  (HD-02; Belge 2'nin düzelttiği yanlışın aynısı).
- **D** d2: 11.2 Wallet için "içe aktarma yüzeyi görülmedi"; hesap türü seçiminde "File Import —
  Import CSV, Excel, OFX", çekmecede Imports, hesapta e-postayla içe aktarma var (WL-02, WL-09).
- **G** Wallet dışa aktarması çekmecedeki katlanmış Others altında, PDF · XLS · CSV (WL-09).
  Hesap Defterim PDF'inin içi (HD-01). Goodbudget yedek göstergesi (g8). KolayBi "Fiş OCR" beyanı
  (not 19). Özet tablo buna göre.

### Bölüm 12 (→ 11) · Diğer modüller

- **G** CalcBox ve PC'den Yönet uygulama içi modül değil (MM-08). Wallet'ın üç modülünün içi
  (WL-10). Bluecoins seyahat modu anahtarı bir etiket seçici açıyor (BC-17).

### Bölüm 13 (→ 12) · Ortak tercihler

- **D** "tek yuvarlak düğme" (not 10); "Sözcükle yazılan durum" 13.3'ten çıkar (not 18); 13.1
  "Tutar girişi" satırı 5.4 ile çelişiyor (Hesap Defterim eksik).
- **G** Goodbudget renk satırı canlıya döner. 13.2 "Otomatik kayıt tercihi"ne Money Manager'ın
  ayarı. Yeni satır adayları (K4).

## 5 · Temizlik ve kapılar

**"Çıkarılmayan sonuç" (karar 5).** Bütün maddeler Aşama 1'de bir yola atandı:

- **(i)** ölçülmeyen etki: sayfadan kalkar; yeni 1.1'de tek cümle.
- **(ii)** finansal sonuç: Belge 2'nin alt bölümüne gönderme olur. Numaralar tablolarda yazılı.
- **(iii)** sayfaya özgü sınır: çoğu sayfada zaten tam cümle olan bir notun tekrarı, kalkar.
  Kalanlar eksik listesine iner.

Sayfa altında yalnız kanıt kimlikleri kalır.

**Yeni kapılar** (Belge 2'nin kapılarından uyarlanır, `tam/uret.py` ve bölüm üretimi):

| Kapı | Ne arar | Muafiyet |
|---|---|---|
| G4 | "Çıkarılmayan sonuç", "matrisi", "koşum", koşum tarihi ("Eylül 2026") | Yok |
| G7 | Kendi kavramlarımız: ADR numarası, kendi sınıf adlarımız | Ürün adı (kapak ve "→ Belge 3" sınır cümleleri; karar 3) |
| G8 | Açılış kutusundaki ürün–kanıt satırı ile gövdedeki "Aynı soruda" rozetlerinin tutarlılığı | Yok |
| Tarih | `kalip.TARIH` ve `.md` başları (not 16) | Kapak üretim tarihi |

**Motor işleri:** şekilsiz bölümde boş "Şekil dizini" yazılmaz (not 17; yeni 1, 11, 12).
`kalip.TARIH` ortak motorda; Belge 2 `.md`'lerini etkileyip etkilemediğine uygulamada bakılır.

## 6 · Kare seçimi

**İlke** (Belge 2'nin ilkesi): yeni kare yalnız (a) bir yanlışı görünür biçimde düzeltiyorsa ya da
(b) sayfanın sorusuna metnin taşıyamayacağı bir kanıt getiriyorsa basılır. Mümkünse aynı iddiayı
daha güçlü gösteren kare eskisinin **yerine** geçer. Aşama 1 ve ekindeki ~33 adaydan 12'si seçildi.

| # | Kare | Sayfa | Nasıl | Gerekçe | Not |
|---|---|---|---|---|---|
| 1 | Wallet `f7-62` hesap türü seçimi | 11.2 | ekleme (sayfada 2 şekil var) | d2'nin karesi: File Import — CSV, Excel, OFX | Aşama 0'da açıldı; kişisel veri yok |
| 2 | HD E0427 üretilen PDF | 11.1 | **E0175'in yerine**; E0175'in iddiası (defter başına Bildiri dönem sormuyor) metinde kimlikle kalır | "Dosya içeriği görülmedi"nin cevabı | PDF toplamları (47.500 / 4.350 / 43.150) E0137'dekilerden farklı; Belge 1'in kuralı zaten "kareler aynı an değildir" |
| 3 | GB `39` işlem listesi | 4.2 şeridi | ekleme (altıncı kırpıntı) | Gelir rengi kareli; 4.2 ve 13.1'de koşum rozeti kalkar | Kırpıntı yalnız Mavi Yazilim–Ada Reklam satırları; üst çubuk (hane adı) dışarıda, karartma gerekmez |
| 4 | BC `f7-57` işlem formu | 5.3 şeridi | E0020 kırpıntısının **yerine** | "Varsayılan GİDER" kendi karesine dayanır | Kutu koordinatı kareye göre yeniden |
| 5 | MM `47` tutar tuş takımı | 5.4 kırpıntı satırı | ekleme (üçüncü kırpıntı: sağ sütundaki simge) | MM satırı canlıya döner; D (dört ürün) düzeltmesine görsel dayanak | Alttaki reklam kırpıntı dışında |
| 6 | BC E0448 Durum alanı | 5.2 (Bluecoins sayfası) | ekleme, önizlemeyle; taşarsa metin | Formun Durum alanının dört değeri; 4.4 ve 9.3 buraya gönderir | — |
| 7 | BC E0426 etiket seçici | 7.2 | ekleme (2 → 3 şekil) | "Bu yolla ayrım denenmedi" kapanıyor | E0088 kalır: "etiketin kendi ekranı var" iddiası onun |
| 8 | Wallet `f7-59` ana ekran | 8.4 | **E0274'ün yerine**; E0274 4.5'te yükleme iskeleti olarak kalır | Kartın yüklenmiş içeriği | Balance Trend başka anın sayısı; karşılaştırılmaz |
| 9 | MM E0234 takvim görünümü | 10.1b | ekleme (3 → 4), önizlemeyle | Sayfanın konusu (tablo ve takvim) üç üründe görülüyor; bugün MM'nin takvimi yok sayılıyor | 17 Eylül öncesi kare; Ağustos verisi, belgenin öteki MM kareleriyle aynı an |
| 10 | BC E0053 hesaplar panosu | 10.2 | ekleme (2 → 3 şekil) | "Net Kazançlar" ve "Net Kazanç" aynı karede alt alta ve okunaklı; iki ölçünün bir harfle ayrılan adları | Aşama 1'de E0459 düşünülmüştü; onun üst bloğu çubuğun arkasında soluk. E0053 bu sorunu taşımıyor |
| 11 | Wallet `f7-74` filtre formu | 10.4 | **E0377'nin yerine** | Filtre yüzeyinin kendisi; "filtre kurulmadı" yerine formun alanları | Kişisel veri yok |
| 12 | GB E0109 Setup Budget | 3.1 | ekleme (3 → 4) ya da E0106'nın yanında küçük şekil | Yeni D'nin (3.1) karesi: yeni haneden sonra ürün önce bütçe istiyor | E0110 (hane kaydı, LATER) metinde kimlikle |

**Aşama 1'deki adaydan düşen:** MM `53` (pastadan kategori ayrıntısı) 10.1b'ye E0234 girdiği için
metinde kalır; envantere girer.

**Basılmayan ama envantere girecek kareler.** Metinde canlı rozetle anılan her kare E kimliği
ister (Belge 1 1.3: basılmayan karenin kimliği dayanak bandında kalır). Aşama 1'de anılan ve
envanterde olmayan kareler: MM `48`, `49`, `52`, `53`, `54`, `56`, `59`, `67` (`60` gerekmez: aynı
Daha ızgarası E0254 olarak basılı) · Wallet `f7-60`,
`f7-63`, `f7-69`–`f7-72` (biri), `f7-73`, `f7-75`, `f7-77`, `f7-84`, `f7-89`, `f7-91`, `f7-93`,
`f7-95`, `f7-96`, `f7-97`, `f7-99`, `f7-101`, `f7-102`, `f7-106` · Bluecoins `f7-55`, `f7-61`,
`f7-62`, `f7-65`, `f7-68`, `f7-70`, `f7-72`, `f7-74` · Goodbudget `38`, `41` · Hesap Defterim `44`,
`46`. Seçilen 11'in envanterde olmayanları da buna eklenir. **Toplam ~45 kare** (K7).

**Basılmaz:** Wallet `f7-64` (import e-postası; aynı bilgi E0437'de) · Wallet E0437 (import
e-postası; metinde kimlikle) · MM `59`, Wallet `f7-102` (silme onayları; sayfada iki onay diyaloğu
zaten basılı, metin yeterli) · Wallet E0450, E0428 (11.1 dört şekille dolu; metin ve özet tablo
taşır, K9) · E0395–E0402 (not 20; K6).

**Karartma** (basılırsa ya da kırpıntı çubuğu içeriyorsa): Goodbudget `38`, `41` hane adı ·
Wallet `f7-64`, `f7-65` import e-postası · MM `61` Play hesap baş harfi. Seçilen 11 karenin hiçbiri
karartma gerektirmiyor (`39` kırpıntısı çubuğu dışarıda bırakıyor).

**Sayfa düzeni riski:** ekleme yapılan yedi sayfa (3.1, 5.2, 5.4, 7.2, 10.1b, 10.2, 11.2) üretimde
önizlemeyle denenir. Taşarsa ekleme yerine metin; `EN_AZ_KARE` kapısı düşmez.

## 7 · Kontrol değerleri

22 Eylül koşumunda test verisi silinmedi. Yeni kareler Belge 1'deki karelerden farklı sayılar
taşıyabilir (ör. Hesap Defterim 45.000 → 47.500, Money Manager Eylül toplamları). Belge 1'in
mevcut kuralı ("kareler farklı anlara ve veri durumlarına aittir; sayılar birbiriyle
karşılaştırılmaz") bu turda da geçerli. Ek kural: yeni bir sayı metinde geçiyorsa cümle o sayıyı
yalnız kendi karesinin gösterdiği şey olarak yazar ve başka bir karedeki sayıyla ilişki kurmaz.
Tarih yazılmaz.

## 8 · Uygulama sırası (onaydan sonra)

0. **Yedek:** `_yedek/<tarih>-belge1-asama3-oncesi/` (belge1/, ortak listesi, gözlem formları).
1. **Ortak liste ve gözlem formları (G2):** §2'deki beş düzeltme; `bluecoins.md` satır 579; üç yan
   bulgu (`f7-96`, `f7-65`, `f7-84`) ilgili formlara.
2. **Envanter:** §6'daki kareler E0463'ten başlayarak `KANIT-ENVANTERI.md`'ye (yol, boyut, SHA-256,
   **kare açılarak yazılan** açıklama) → `kanit-dizini-uret.py`.
3. **Kesme (karar 4):** Belge 1'e özgü ayarla durum çubuğu atılır. İşaret koordinatları kırpma
   sonrası uzayda yazılı (`motor.isaretle`); motor bunu ham ekran koordinatına göre kaydırır. Üst
   130 pikseldeki 9 işaret tek tek taşınır. Motora dokunulduğu için **Belge 2 geçici klasöre
   üretilir ve sayfa görüntüleri eskisiyle otomatik karşılaştırılır**; fark çıkmamalı.
4. **Kapılar:** G4, G7, G8, tarih kapısı ve boş şekil dizini (§5). İlk çalıştırma bugünkü belgede
   yapılır ve bulduğu sayı kaydedilir (temizliğin ölçüsü).
5. **Yapı (karar 6):** yeni Bölüm 1 kurulur; bölüm numaraları kayar; 104 gönderme betikle.
   Klasör adları K8'e göre.
6. **Bölüm dosyaları:** `icerik.py` bölüm bölüm; önce D, sonra G, en son T. Her bölüm kendi
   `uret.py`'siyle üretilir, kapılar temiz, `onizleme/` gözle incelenir. Karelerin eklendiği beş
   sayfada taşma kontrolü.
7. **Eksik listeleri:** kapananlar düşer; açık kalanlar (BC-X1, BC-19, HD-04, GB-04 tebrik, WB-02,
   BC-16 dosya üretimi, WL-19 asıl neden) ve Aşama 1'de eksik listesine inen sınırlar eklenir.
8. **`tam/` yeniden üretilir** (arka planda; 2 dakikayı geçer). Kartlardaki kare sayıları
   envanterin son hâlinden. Sayfa, kare, hash, kırık atıf raporu.
9. **Belgeler:** `belge1/README.md`, `DURUM.md`, devir notu.
10. **Belge 2'ye dönük iş (karar 7):** yedi "→ Belge 1 §N" göndermesi yeni numaralara çekilir;
    ilgili Belge 2 bölümleri ve `tam/` yeniden üretilir, yalnız o sayfalara bakılır.

## 9 · Açık kararlar

Önerim karar değildir; kullanıcı ve dış inceleme için yazıldı. **Uygulandı: K1–K11 önerileriyle**
(24 Eylül, kullanıcının tam yetkisi). K11'in sonucu: açılış sayfası girişin kendisi oldu, kartlar
1.1 (canlı) ve 1.2 (kaynak) numarasını aldı; §3 tablosundaki 1.2/1.3 numaraları bu yüzden birer azaldı.

| # | Karar | Seçenekler | Önerim |
|---|---|---|---|
| **K1** | Yeni 1.1'de "görülmedi ile yok ayrıdır" ve nicelik kuralı | A · iki cümle olarak kalır · B · kalkar (rozet yeter) | **A.** Rozet "görülmedi"yi gösteriyor ama okurun onu "yok" diye okumaması için tek cümle gerekiyor; belgenin her yokluk ifadesi bu kurala dayanıyor |
| **K2** | "Bir tanıtım karesi ne kadar uzaktır?" bloğu (Paraşüt videosu, E0262) | A · Paraşüt kartına tek cümle · B · kalkar | **A.** Kaynak görselinin sınırını en somut anlatan örnek; tek cümleye iner |
| **K3** | d1'in (kasadefteri) ayrıntısı nerede | A · işaret ve kısa not 11.1'de, dosya yolları 11.4'te (veri konumu) · B · hepsi 11.1'de | **A.** 11.1 "çıktı nereden seçiliyor", 11.4 "veri nerede duruyor" sorusu; dosya yolları ikincisinin cevabı |
| **K4** | 13.1'e "Silme onayı" (beş ürün), 13.2'ye "Silineni geri almak" (iki ürün var · üçünde menüde yok) satırları | A · eklenir · B · eklenmez, 13 yalnız düzeltilir | **A.** Sentez 5.8'in yeni hâlini izlemeli; eklenmezse 13.1 "çoğunda aynı olanlar" en güçlü ortaklıklardan birini atlar |
| **K5** | 4.4 başlığındaki "seçili filtre" | A · başlıktan çıkar ve 10.4'e gönder · B · sayfaya filtre örneği eklenir | **A.** Filtre yüzeyi 10.4'ün konusu ve bu turda orada güçleniyor |
| **K6** | 15 Eylül'ün sekiz karesi (E0395–E0402, not 20) | A · basılmaz, dayanakta kimlik ve rozet canlıya döner · B · ilgili sayfalarda basılır | **A.** Wallet menüleri (E0399, E0400) cümleye yalnız bir seçenek adı ekliyor; Goodbudget ve Bluecoins kareleri Belge 2'nin konusu |
| **K7** | Basılmayan ama anılan kareler envantere girsin mi (~45) | A · hepsi girer; açıklama kare açılarak · B · yalnız basılanlar girer, ötekiler ortak liste kimliğiyle (ör. "WL-15") ve koşum rozetiyle · C · A, ama aynı iddia için tek kare (seriden biri) | **C.** Rozet dürüst kalır (kare var, canlı); iş yükü seriler tek kareye inince ~35'e düşer. B, karesi olan bir bulguyu "karesiz" gibi gösterir |
| **K8** | Klasör adları numaralar kayınca | A · yeni `bolum-01-giris`; 03–13 klasörleri 02–12 olarak yeniden adlandırılır · B · klasör adları kalır, yalnız `NO` değişir | **A.** Klasör adı ile bölüm numarası ayrışırsa sonraki oturumlar yanlış dosyayı açar. Kullanılmayan üç `bolum-03-*` denemesi yerinde kalır ama adları yeni `bolum-02-*` ile karışmasın diye README'de not edilir |
| **K9** | 11.1'e Wallet dışa aktarma karesi (E0428 ya da E0450) | A · basılmaz; metin ve özet tablo · B · E0450 bir şeklin yerine | **A.** 11.1 dört şekille dolu; Wallet'ın cevabı ("katlanmış menüde, üç biçim") tek cümleye sığıyor. Karesi Belge 2 8.1'de basılı |
| **K10** | Üç kaynak ürüne Belge 2 Bölüm 9 göndermesi (karar 10) nerede | A · yeni 1.3 girişinde bir kez · B · A + 9.5b'de (bugünkü) · C · her kaynak ürün satırında | **B.** 1.3 okuru baştan yönlendirir; 9.5b borç ve fatura akışının kaynağa en çok dayandığı sayfa |
| **K11** | Yeni 1.1 bölüm açılış sayfası mı olsun | A · açılış sayfası 1.1'dir (kutu, giriş, sınır ve "neyi ölçmez" tek sayfada) · B · açılış + ayrı 1.1 sayfası | **A.** Karar 6 "tek giriş sayfası" diyor; bugünkü s.3 zaten bu işi yapıyor |

## 10 · Dış inceleme için sorular

1. §2'deki doğrulamada gözden kaçan bir çelişki var mı? Özellikle WL-19'un "kısmen" sayılması:
   *Uygulamada:* 3.1'de Wallet notu ayarın yalnız ondalığı değiştirdiğini söylüyor (E0482, E0485); asıl
   farkın nedeni "bilinmiyor" olarak kaldı, açılış kutusu da bunu yazıyor.
2. §4'teki D satırlarından herhangi biri dar okunmuş bir ifade mi, yoksa gerçekten basılı bir
   yanlış mı? (5.8 silme onayı, 5.4 "dört ürün", 13.1 tutar girişi, 3.1 Goodbudget'ın hesap
   istemesi: ilk ekrandaki LOG IN / CREATE NEW HOUSEHOLD "hesap istemek" sayılır mı?)
   *Uygulamada:* on iki D'nin hepsi yazıldı (yeni numaralarla 4.8, 4.4, 12.1, 2.1 …). Goodbudget için
   2.1, hane kaydının bütçe kurulumundan sonra geldiğini ve LATER taşıdığını yazıyor (E0109, E0110).
3. §6'daki yer değiştirmeler (E0175 → E0427, E0274 → `f7-59`, E0377 → `f7-74`, E0020 → `f7-57`)
   eski karenin taşıdığı bir iddiayı kaybettiriyor mu?
   *Uygulamada:* dört eski kare de metinde kimliğiyle kalıyor ve iddiasını taşıyor: E0175 (10.1, defter
   başına Bildiri dönem sormuyor), E0274 (3.5 şekli ve 7.4 notu, yükleme iskeleti), E0377 (9.4 notu),
   E0020 (4.2 şekli; 4.3 kırpıntısı E0487 oldu). Ayrıca 6.2'de E0426 yerine E0491 basıldı (§12).
4. Wallet'ın Minimum balance ayarı (6.4) ile karttaki eşik uyarısı arasındaki bağ yazılmadı; bağ
   karede kurulmuyor. Bu temkin gereksiz mi?
   *Uygulamada:* temkin korundu; 5.4 notu ayarın metnini ve uyarının metnini yan yana veriyor, bağı
   kurmuyor.
5. "Çıkarılmayan sonuç" satırlarının kalkması, okurun bir sayfadan fazla sonuç çıkarmasına yol
   açar mı? Yol (i) cümlesinin yalnız girişte bir kez yazılması yeterli mi?
   *Uygulamada:* satırların hepsi kalktı; yol (i) Bölüm 1 açılışında tek cümle ("ölçülmemiş bir etki için
   cümle kurulmaz"), yol (ii) Belge 2 alt bölüm göndermesi. Uygulama sonrası ek bir kural geldi:
   "Görülmedi" rozetinin tekrarı azaltıldı (§12'deki devir kaydı); dış göz bunun sınırı gevşetip
   gevşetmediğine de bakabilir.

## 11 · Bu planın kapsamı dışında

- **Açık kalan eksikler:** BC-X1 (kart ekstresi), BC-19 (otomatik kol), HD-04 (dönem ayarı), GB-04
  tebrik mesajı, WB-02 (KolayBi periyot değerleri), BC-16 dosya üretimi, WL-19'un asıl nedeni,
  erişim yok satırları (MM-X1, WL-X1, WL-X2, WL-X4, BC-X3, GB-X1, GB-X2, HD-X2). Yeni koşum kullanıcı
  kararı; bu tur koşum açmaz.
- **Belge 2:** yalnız yedi gönderme (§8 adım 10). Belge 2 yeniden incelenmez.
- **Git:** `raporlar/` Git'te değil; commit yapılmaz.

## 12 · Uygulama sonucu

**Sonuç (24 Eylül 2026):** `belge1/tam/belge1.pdf` **97 sayfa**: kapak ve içindekiler 2, 12 bölüm
88, kanıt eki 7. **154 basılı kare** (önceki 145); ekte ayrıca **126 anılan ama basılmayan kare**
listelenir; hash denetimi 280 kimlik. Kırık gönderme, bozuk karakter ve gizli sözcük yok; bütün
belge metni b1 kapılarından geçti. §3'ün 93 ± 3 tahmininden fark kanıt ekinde: ek artık metinde
anılan her basılmayan kareyi listeliyor (eskiden 4 satırdı).

**Birleşik belgenin üretimi değişti:** `tam/uret.py` bölümleri `b1.Bolum` ile basıyor (önceden
`kalip.Bolum`: kesme birleşik belgede uygulanmazdı). Kapakta tarih yok (A1; §5'teki "kapak üretim
tarihi" muafiyeti kaldırıldı, Belge 2 kapağıyla aynı). 89 bölüm sayfası tekil bölüm PDF'leriyle
metin ve resim boyutu bakımından birebir aynı.

**Belge 2 (§8 adım 10):** yedi "→ Belge 1 §N" birer azaldı; 105 sayfada yalnız o yedi satır
değişti, resim farkı yok. Ortak motordaki karartma eklemesi Belge 2'yi etkilemedi.

**Yeni Bölüm 1:** 6 sayfa (açılış = giriş; 1.1 üç sayfa, 1.2 iki sayfa). Tek kartı kalan sayfa
(Goodbudget) `b1.py`'deki `yuva` ayarıyla ötekilerle aynı kart genişliğinde. KolayBi kartı:
31 destek görselinin 17'si basılıyor (dış göz notu 1).

Ayrıntılı kayıt: [devir-2026-09-24-belge1-asama3.md](devir-2026-09-24-belge1-asama3.md),
"Uygulama denetimi" bölümü.
Ayrıntılı kayıt: [devir-2026-09-24-belge1-asama3.md](devir-2026-09-24-belge1-asama3.md),
"Uygulama denetimi" bölümü. Bu plandan ayrılan kararlar:

- **§6 #7:** 6.2'de E0426 yerine **E0491** basıldı (etiketin kayıtta çip olarak görünüşü). E0426
  dayanakta kalır.
- **§6 envanter:** K7-C'ye iki kare daha eklendi: E0497 (Wallet etiket formu), E0498 (Wallet planlı
  ödeme sıralaması). Envanter 461 kare.
- **§4 Bölüm 10 (→ 9):** Money Manager kategori ayrıntısındaki grafik çubuk değil **çizgi** (E0466).
- **§8 adım 4, ilk kapı taraması:** 72 "Çıkarılmayan sonuç", 24 "koşum", 21 tarih, 2 "matrisi".
