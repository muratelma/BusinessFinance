# Uygulama Gözlem Formu — Bluecoins

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Bluecoins Finance & Budget / Mabuhay Software |
| Sürüm | 13.1.45 (`versionCode=33111`) · Faz 3'te aynı sürüm, kurulum 2026-09-07 |
| Test tarihi | 1 Eylül 2026 (Tur 1, eski PC) · **10 Eylül 2026 (Faz 3 boşluk koşumu, yeni emülatör)** |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | Türkçe / TRY (sistem dilini alıyor) |
| Hesap veya plan türü | Yerel ücretsiz sürüm; **bulut senkronizasyonu / giriş yok** → Faz 3'te yeni emülatörde Tur 1 verisi **yoktu**, tam yeniden koşum yapıldı (Money Manager Faz 1 gibi). Hazır gelen ₺0 örnek hesaplar (Birikimler, Çek, Cüzdan, Kredi Kartı, Ev İpoteği) silinmedi, net varlığa etkisi yok. |
| Erişim kısıtı | Kayıt/giriş yok; PDF/yazıcı, CSV ve HTML dışa aktarma Premium. **Kararsızlık:** transfer kaydından sonra "Varsayılan klasörü bulamıyor" diyaloğu UI'yi kilitledi; force-stop + yeniden başlatma ile kurtarıldı (yerel DB'de veri kaldı). |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Tamamlandı | 2 | Kayıt, e-posta veya telefon istemiyor; Türkçe dil seçili geliyor. İlk ekranda “İlk İşlemi Ekle” ve örnek veriyle “Demo Dosyasını Dene” yolları ayrılmış | Demo önerisi ilk `+` dokunuşunda yeniden soruluyor; gerçek test verisiyle karışmaması için reddedildi | `00-magaza.png`, `01-ilk-acilis.png` |
| K01 Ana ekran | Tamamlandı | 0 | Hesaplar / İşlemler / Hatırlatıcılar / Tümünü göster yatay geçişleri; günlük özet, bütçe ve takvim gibi kartlar; kart listesi kullanıcı tarafından düzenlenebiliyor | İlk bakış yoğun; ekrandaki kartların bir kısmı aynı dönemde “işlem yok” mesajını tekrar ediyor | `02-bos-ana-ekran.png`, `09-ozgun-ozellik.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı | ~8/hesap | Açılış bakiyesi **ve açılış tarihi** aynı formda. Banka, nakit, kredi kartı, kredi, ipotek, alacak, yatırım, sanal hesap ve dış varlık dahil geniş tür ağacı var | Uygulama Birikimler, Cüzdan, Kredi Kartı ve Ev İpoteği gibi birçok sıfır bakiyeli örnek hesabı hazır getiriyor; hesap seçici kalabalıklaşıyor | `03-dolu-ana-ekran.png` |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~9 | Tek formda ad, tarih/saat, planlama, tutar, para birimi, kategori, hesap, bölme, durum, etiket ve not var; ayrı `GELİR` türü | Form yoğun ve varsayılan `GİDER`; işletme hizmet geliri kategorisi/kapsamı yok, `Diğer` kullanıldı | `04-islem-formu.png` |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~8 | İşlem adı listede birincil başlık; kategori ve hesap ikinci satırlarda. Tutar ve tarih aynı formda değişiyor | **İşletme/şahsi kapsam alanı yok.** Etiketle taklit edilmedi; bu boyut `Desteklenmiyor` | `05-siniflandirma.png` |
| K05 İşletme kart gideri | Tamamlandı | ~9 | Kredi kartı hesabı seçilince `Taksit şartlarını seçin` alanı beliriyor; ₺1.200 gider harcama tarihinde rapora girdi ve kart bakiyesi −₺1.200 oldu | İşletme kapsamı ve tedarikçi alanı yok; tedarikçi ancak ad/not/etiketle taşınabilir | `06-islem-listesi.png` |
| K06 Transfer | Tamamlandı | ~10 | Ayrı `TRANSFER`; kaynak/hedef, `Takas` ve transfer ücreti alanları var. Listede iki bağlı satır ve her iki hesabın işlem sonrası bakiyesi gösteriliyor. Ana Hesap→İş Kartı transferi kartı sıfırladı ve gün toplamı ₺0 kaldı | Kaynak ve hedef aynı varsayılan hesapla açılıyor; iki hesap da ayrı ayrı yeniden seçilmeli. Aynı olayın iki satırı ilk bakışta çift kayıt sanılabilir | `06-islem-listesi.png` |
| K07 Liste ve rapor | Tamamlandı | ~4 | Liste tarih/gün netiyle gruplanıyor; her satırda işlem sonrası hesap bakiyesi var. Raporlar dönem karşılaştırması yapıyor: Ağustos gideri −₺2.050; hesap raporunda toplam varlık ₺44.950. Filtre ve yazdır/dışa aktar yüzeyi mevcut | Rapor sekmeleri yatay ve çok sayıda; bazıları ekran dışında kalıyor. CSV/PDF/HTML yüzeyi Premium yükseltme başlığı altında açıldı | `06-islem-listesi.png`, `07-rapor.png` |
| K08 Düzeltme/iptal | Tamamlandı | ~4 | Satır → detay → Düzenle ile aynı form açılıyor; kart giderinin tarihi bu yolla düzeltildi. Silmede Türkçe onay var | Düzenleme kaydı yerinde değiştiriyor; silme kalıcı ve void/iptal/geri alma yok. **Sıfır tutarlı gider uyarısız kaydedildi**, sonra Sil→Tamam ile temizlendi | `08-hata-veya-bos-durum.png` |

## Kontrol değeri doğrulaması

| Değer | Beklenen | Bluecoins | Durum |
|---|---:|---:|---|
| Ana Hesap bakiyesi | 40.800,00 | ₺40.800,00 | ✓ |
| Ortak Cüzdan bakiyesi | 4.150,00 | ₺4.150,00 | ✓ |
| İş Kartı bakiyesi/borcu | 0,00 | ₺0,00 | ✓ |
| Toplam net varlık | 44.950,00 | ₺44.950,00 | ✓ |
| Gelir işlemi | 25.000,00 | ₺25.000,00 | ✓ |
| Gider toplamı | 2.050,00 | −₺2.050,00 | ✓ |

Transfer ve kart ödemesi listede kaynak/hedef için ayrı satırlar oluşturdu,
fakat tarih başlığındaki toplamı ve gider raporunu ikinci kez etkilemedi.

**Faz 3 (10 Eyl 2026)** — yeni emülatörde 3 hesap (Ana Hesap ₺20.000 Banka, Ortak
Cuzdan ₺2.000 Nakit, Is Karti ₺0 Kredi Kartı) + 5 çekirdek işlem Ağustos 2026
tarihleriyle yeniden girildi. Kontrol birebir tuttu: **Net Kazançlar Ağustos —
Gelir ₺25.000,00 / Gider −₺2.050,00 / Net ₺22.950,00**; **Net Kazanç Eylül —
Varlıklar ₺44.950,00 / Cari hesap ₺0,00 / Net Kazanç ₺44.950,00** (açılış
bakiyeleri "bugün" tarihli düştüğü için Ağustos satırı ₺22.950, güncel ₺44.950).
İş Kartı ₺0, Ana Hesap ₺40.800. Transfer + kart ödemesi gün toplamında ₺0.
Kareler `11`, `15`, `16`. Not: açılış tarihi emülatörde içinde bulunulan aydan
öncesine çekilemedi (takvim/hesap-makinesi etkileşimi); açılışlar 10 Eyl tarihli
kaldı — güncel bakiyeyi ve aylık gelir/gider raporunu etkilemiyor.

## Arayüz taraması (görev dışı, ~10 dk)

| Alan | Gezildi mi | Kısa gözlem |
|---|---|---|
| Tüm ana sekmeler / alt görünümler | ✓ | Hesaplar, İşlemler, Hatırlatıcılar, Tümünü göster, Bütçe, Net Kazançlar, Öğeler Özeti ve Etiketler görüldü; yatay sekme listesi ekranı aşacak kadar geniş | — |
| Bir raporun içine tıklama | ✓ | Net Kazançlar dönem karşılaştırması ve Tümünü göster hesap/varlık raporu açıldı; satırlar hesap türü→hesap hiyerarşisinde açılıyor | `07-rapor.png` |
| Bütçe / hedef / planlama ekranı | Kısmen | Bütçe Özeti, Hatırlatıcılar ve işlem formunda `Planlı İşlemler` alanı görüldü; derin oluşturma Tur 2'ye bırakıldı | — |
| Ayarların derinliği | Kısmen | Ana ekran kartları: Günlük Özet, Takvim, Bütçe Özeti, Net Kazançlar, Kredi Kartı Özeti, Net Kazanç, Nakit Akışı ve Favori Hesaplar. Dört favori hesap yuvası var | `09-ozgun-ozellik.png` |
| Arama ve filtre davranışı | Kısmen | Hesap seçicide arama; raporlarda Filter; işlem listesinin serbest metin araması bu turda denenmedi | — |
| Boş durum ekranları | ✓ | İlk kurulum “İlk İşlemi Ekle” / “Demo Dosyasını Dene” diye iki yön veriyor; dashboard kartları dönem boşsa açıkça söylüyor | `01-ilk-acilis.png`, `02-bos-ana-ekran.png` |
| Hata / uç durum | ✓ | Sıfır tutar **hata vermeden kaydedildi**. Silme bottom sheet'i “Silmek istediğinize emin misiniz?” + İptal/Tamam sunuyor | `08-hata-veya-bos-durum.png` |
| Widget / hızlı giriş / kısayol | Kısmen | Ana ekranda sabit `+`; düzenlenebilir dashboard kartları var. Android ana ekran widget'ı denenmedi | `09-ozgun-ozellik.png` |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Dashboard kart tabanlı; işlem listesi gün toplamı → işlem adı/tutar → kategori/hesap → işlem sonrası bakiye hiyerarşisi kuruyor |
| Alt/üst gezinme | Alt bar yerine ekran üstünde yatay ve kaydırılabilir çok sayıda sekme; sağ altta bağlama göre `+`. Sekmeler büyüdükçe hedefler ekran dışına taşıyor |
| Renklerin anlamı ve tutarlılığı | Gelir yeşil, gider pembe/kırmızı, transfer mavi; seçili tür dolu arka planla ve tutar alanındaki +/− işaretiyle destekleniyor |
| Tipografi ve para değerlerinin okunması | Para sağda, iki ondalık ve ₺ ile; günlük toplam ile satır tutarı ayrışıyor. Çok sayıda küçük ikincil metin yoğunluk yaratıyor |
| Kart, liste ve grafik kullanımı | Dashboard kartları; tarih gruplu işlem listesi; iki dönem sütunlu rapor. Varlık raporu tür ve hesap düzeyinde açılıyor |
| Form alanları ve varsayılanlar | Tek ekran çok güçlü ama yoğun. Gider varsayılan; kart seçimi taksit alanını dinamik ekliyor; transferde kaynak/hedef + kur/takas + ücret var |
| Loading, boş, hata ve başarı geri bildirimi | Boş durumlar açıklayıcı; kayıt başarısı sessizce önceki ekrana dönüyor. Sıfır tutar doğrulaması yok; silme onaylı ama geri alınamaz |
| Erişilebilirlik / dokunma alanları / metin yoğunluğu | Dokunma hedefleri genelde büyük; ancak yatay sekme keşfi, yoğun tek form ve bazı çeviriler (`Yinelenmek`, `Takas`) bilişsel yük yaratıyor |

## Akış özeti

- En kısa ve güçlü akış: Tek ekranda ayrıntılı işlem oluşturma; tarih, planlama, bölme ve etiket başka bir ayrıntı sayfasına dağılmıyor.
- En fazla sürtünme yaratan akış: Hesap seçimi — hazır gelen çok sayıda sıfır hesap arasında kaynak/hedefi iki ayrı bottom sheet'ten bulmak.
- Uygulamanın hedef kullanıcı varsayımı: Finansal model ayrıntısı isteyen ileri seviye kişisel bütçe kullanıcısı; çok hesap, kredi, ipotek ve rapor kullanıyor.
- İşletme ve şahsi para yaklaşımı: **Yok.** Kategori/etiket mevcut ama özel kapsam boyutu değil.
- Transfer ve kart ödemesi yaklaşımı: Ayrı transfer türü, çift bağlı satır, gün toplamında sıfır; kart ödemesi ikinci kez gider değil.
- Planlama, borç ve tahsilat yaklaşımı: Planlı işlem, hatırlatıcı, kredi/ipotek ve taksit alanları var; B1/B2 + bağımsız hatırlatıcı canlı kuruldu (hepsi tek Hatırlatıcılar listesi). **Cari hesap tipi var ama sıradan bakiye hesabı** — fatura nesnesi ve tahsilat→fatura bağı yok (Faz 3'te canlı doğrulandı).

## Faz 3 boşluk koşumu (10 Eyl 2026) — taksit, tekrarlayan, kart, hatırlatıcı

Yeni emülatörde tam yeniden koşum (bulut yok). Kareler `kanitlar/bluecoins/10`–`28`.

### İşlem formu — tek yoğun ekran

İsim + tarih/saat + **Planlı İşlemler** (tekrar) düğmesi + tutar (kırmızı −
GİDER / yeşil + GELİR, hesap makinesi widget'ı) + kategori + hesap + (kart
seçilince) **Taksit şartlarını seçin** + Bölmek + Durum + Etiket + Not. Alt bar:
GİDER / GELİR / TRANSFER + yeşil kaydet. Kare `12`. Hesap makinesi widget'ı bazen
donuyor (emülatör tuzağı) — tutar alanına doğrudan dokunup klavyeyle yazmak çalışır.

### Kredi kartı modeli

Kart, "Cari Hesap" grubunda **negatif bakiyeli** bir hesap. Kart hesabı seçilince
işlem formunda **"Taksit şartlarını seçin"** alanı dinamik olarak beliriyor
(kare `12`). Kart ödemesi Tur 1'deki gibi Ana Hesap → İş Kartı **transferi**
(raporda nötr); ayrı "kartı öde" akışı yok → **kısmi ödeme** = daha küçük transfer
(Wallet ile aynı, önemsiz). Ekstre kesim/dönem kavramı Faz 3'te aranmadı.

### Taksit (B2 — tasarım ekipmanı ₺6.000, canlı test 10 Eyl) — Bluecoins'in ayırt edici özelliği

"Taksit şartlarını seçin" → alt sayfa (`17`): **Taksit oranı** (faiz %,
düzenlenebilir, 0 bırakıldı) + **ay sayısı** dropdown (2/3/6/9/12/15/18/21/24 ay
+ **Özel**) + **İlk ödeme** tarihi. Kuruldu: 6 ay, %0, ilk ödeme 15 Ağustos 2026.
Onaylayınca form üstünde açıklama: *"15 Ağustos 2026 günü başlayıp 1.000,00'e
kadar 6 aylık hatırlatıcılar oluşturulacak"* (`19`).

Kaydettikten sonra gözlemler:
- **₺6.000 → 6 × ₺1.000'e bölündü.** Tam tutar bir kerede yazılmadı.
- **Taksit 1/6 anında gerçek harcama:** ilk-ödeme tarihinde (15 Ağu) İşlemler
  listesine "Tasarim ekipmani · Others · 1 / 6 · −₺1.000" olarak düştü; kart
  borcu −₺1.000, **Ağustos gideri +₺1.000** (rapor ₺2.050 → ₺3.050). Kareler
  `20`, `22`.
- **Taksit 2/6–6/6 aylık hatırlatıcı:** Hatırlatıcılar sekmesinde "15 Eyl 2/6",
  "15 Eki 3/6", "15 Kas 4/6", "15 Ara 5/6", "15 Oca 6/6" — her biri −₺1.000,
  İş Kartı. Kare `21`.
- **BusinessFinance `InstallmentPlan`'a çok yakın** (per-item realize): plan
  niyet, her taksit ayrı gerçekleşir. **Tek fark:** ilk taksit otomatik yazılıyor,
  BusinessFinance'te ilki de açık realize ister.

### Tekrarlayan işlem (B1 — aylık ₺600 abonelik, canlı test 10 Eyl)

Formda **"Planlı İşlemler"** → alt sayfa (`23`): sıklık çipleri **Bir Defa /
Günlük / Haftalık / Aylık / Yıllık**; Aylık'ta → "Her ay tekrarla" (her N ay) +
Tekrarla: **Ayın günü / Haftanın günü** + Tarih + **Son Ödeme Tarihi: Asla /
1 etkinlik sonra / Son Tarih** + checkbox **"Vade tarihinde otomatik olarak
işlem olarak girin"** (otomatik mi onaylı mı). Kuruldu: Aylık, başlangıç
**10 Ağustos 2026** (geçmiş), otomatik checkbox **KAPALI**. Form üstünde banner:
"(Yenilenen işlem) Her ay tekrarla 10 Ağustos 2026" (`24`).

Gözlemler:
- **Geçmiş tarihe kurulabiliyor** (Wallet'ın aksine — Wallet içinde bulunulan
  aydan öncesine izin vermiyordu). Kare `23`.
- **Tanım tek başına hiçbir şey üretmez** — kaydettikten sonra Ağustos gideri
  değişmedi, İşlemler listesine kayıt düşmedi (`28` öncesi rapor hâlâ ₺3.050).
  **BusinessFinance `RecurringTransaction` ile birebir.**
- Geçmiş + bugünkü + gelecek **tüm occurrence'lar Hatırlatıcılar sekmesinde
  bekliyor:** geçmiş = *"31 gün gecikmeli"* (kırmızı), bugünkü = *"Bugün süresi
  doluyor"* (turuncu), gelecek = tarihli. Otomatik yazılan **yok**. Kare `25`.
  (Money Manager geçmiş/bugünkü occurrence'ı **otomatik** yazıyordu — Bluecoins yazmaz.)
- Occurrence detayında **[Kaydet] [Düzenle]** (`26`). **Kaydet** → *"İşlem Olarak
  Kaydet? **Bugün** / **10 Ağustos 2026**"* sorusu (`27`) → seçilen tarihle
  gerçek işleme dönüşür, hatırlatıcı listesinden düşer, seri devam eder.
- 10 Ağu occurrence'ı "10 Ağustos" tarihiyle onaylandı → **Ağustos gideri
  ₺3.050 → ₺3.650**, Ana Hesap −₺600, net varlık ₺44.350 → ₺43.350. Kare `28`.
- **Tüm rakiplerin içinde BusinessFinance'e en yakın tekrarlayan model.**

### Transfer detayı

Kayıt detayı: iki bağlı satır + her satırda işlem sonrası bakiye + **"Benzer
işlemler göster"** + **"Yinelenmek"** (transferi tekrarlı yapma) + Düzenle.

### Hatırlatıcılar sekmesi = birleşik bekleyen görünüm

Hem tekrarlayan hem taksit occurrence'ları burada, **tarih sıralı**, tek listede;
gecikmeli / bugün / gelecek etiketli. BusinessFinance'in "planlanan projeksiyonu"
ile aynı iş (`readiness`/`attentionCode` benzeri gecikme etiketleri).

### Kredi Kartı hesap tipi — ekstre kesim günü VAR

Hesap oluştururken **Hesap Tipi = Kredi Kartı** seçilince ek alanlar açılıyor:
**Kredi Limiti + Hesap Kesim Günü (Ayın 1. Günü...) + Bitiş tarihi**. Kare `32`.
Yani Bluecoins'te kredi kartının **ekstre kesim günü kavramı var** (Wallet'ta yok,
Money Manager'da "Hesap Kesim Tarihi" olarak vardı). Faz 3'te İş Kartı bu alanlar
varsayılan bırakılarak kurulmuştu; kesim günü davranışı ekstre projeksiyonuna
Tur 2'de bakılabilir.

### Bölmek (split transaction)

İşlem formunda **"Bölmek"** → çok satırlı mod: **"Hepsini temizle" / "+ Ekle"**
+ **Toplam tutar** göstergesi; her satır kendi tutarı + kategorisi + hesabı +
notu + çöp ikonuyla. Tek fiş içindeki farklı kalemleri ayrı kategori/hesaba
bölmek için. Kare `29`.

### Taslak uyarısı

İşlem formundan kaydetmeden çıkınca **"Değişiklik kaydetmeden işlem ekranından
çıkın mı?"** onayı çıkıyor (Money Manager'da taslak uyarısı **yoktu**; Bluecoins'te var).

### Bağımsız hatırlatıcı (canlı oluşturuldu)

Hatırlatıcılar sekmesi → **+** → **aynı işlem formu** + üstte "*11 Eylül 2026
günü bir kez program yapın*" banner'ı, tarih yarına ayarlı. Yani bağımsız
hatırlatıcı = **"Bir Defa"** frekanslı bir Planlı İşlem. Kuruldu: "Ofis kirasi"
₺10.000, Ana Hesap → Hatırlatıcılar listesinde **"Yarın borçlanacak"** etiketiyle,
tekrarlayan/taksit occurrence'larıyla **aynı listede** göründü. Kareler `30`, `31`.
**Bulgu:** Bluecoins'te bağımsız hatırlatıcı + tekrarlayan occurrence + taksit
occurrence hepsi tek "Planlı İşlemler / Hatırlatıcılar" yapısı.

### Cari hesap → tahsilat bağı (canlı oluşturuldu)

Hesap tipi listesinde **"Alacaklar"** (Varlıklar grubu) ve **"Cari hesap" /
"Cari hesaplar"** (Cari Hesap grubu) var. Kuruldu: "Ada Reklam cari" (Cari hesap
tipi). **Cari hesap tipinin özel alanı yok** — Kredi Kartı'nın aksine limit/kesim
günü taşımıyor; sıradan bir bakiye taşıyan hesap. Kareler `32`, `33`.
- **Fatura / e-fatura nesnesi YOK.** "Bu tahsilatı şu faturaya bağla" özelliği yok.
- Veresiye satışı modellemek için: cari hesaba GELİR kaydı → cari bakiye +₺X;
  tahsilat = cari hesap → banka **transferi** → cari 0'a döner, banka +₺X, gelir
  ikinci kez sayılmaz. **Net cari bakiye açık tutarı verir**, o kadar.
- BusinessFinance `Counterparty` + `CounterpartyCharge` (tanır) + `CounterpartyPayment`
  (taşır) + fatura bağı modelinin karşılığı **yok**; cari hesap yalnız bir kasa gibi.

### Kısmi kart ödemesi (canlı test)

Kart ödemesi ayrı akış değil, Ana Hesap → İş Kartı **transferi**. Kısmi ödeme
= daha küçük transfer: ₺500 transfer girildi → İş Kartı −₺1.000 → **−₺500**.
Herhangi bir tutar çalışıyor; ekstre kesim/dönem mantığı devreye girmiyor. Kare `34`.

### Fiş / kamera

İsim alanının yanında ataç ikonu = **ek dosya** (Tur 1 ile tutarlı). OCR /
fiş-okuma özelliği görülmedi, pazarlanmıyor. Ataç butonu emülatörde adb koordinat
dokunuşuyla açılamadı (İsim metin alanı + Android el yazısı katmanı butonun hit
alanının üstüne biniyor; gerçek dokunmatik cihazda sorun olmaz).

### Faz 3 sonrası veri durumu

Ana Hesap ₺39.700 · Ortak Cuzdan ₺4.150 · İş Kartı −₺500 · Ada Reklam cari ₺0 ·
net ₺43.350. Ek: B1 tekrarlayan serisi (10 Ağu occurrence onaylandı, sonraki
vade 10 Eyl), B2 taksit hatırlatıcıları 2/6–6/6, "Ofis kirasi" ₺10.000 bağımsız
hatırlatıcı (11 Eyl), ₺500 kısmi kart ödemesi. Bluecoins Tur 2'ye seçilirse
dosya sıfırlanıp yeniden girilir.

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşlem satırında işlem sonrası hesap bakiyesi | Uyarlayarak al | Kullanıcı hareketin etkisini anında görüyor; birleşik feed'de isteğe bağlı ikincil bilgi olabilir | Aktivite feed'i |
| Gün başlığında o günün neti | Uyarlayarak al | Gelir/gider/transfer yoğunluğunu gün düzeyinde özetliyor; transferlerin sıfır etkisi görünür oluyor | Aktivite feed'i |
| Açılış bakiyesi + açılış tarihi aynı hesap formunda | Doğrudan al / mevcut yaklaşımı doğrular | Açılış bakiyesi raporu kirletmeden zaman bağlamı kazanıyor | Hesap oluşturma |
| Dinamik kart alanı (`Taksit şartlarını seçin`) | Uyarlayarak al | Yalnız seçilen hesap türüne ilişkin alanları açmak form yoğunluğunu azaltır | İşlem/kart formu |
| Kullanıcı tarafından düzenlenen dashboard kartları | Uyarlayarak al | Farklı kullanıcı önceliklerine uyar; ancak ilk sürümde az sayıda anlamlı ön ayar yeterli | Ana ekran |
| Tek ekrana bütün ayrıntıları yığmak | Alma / dikkat | Güçlü ama esnaf için fazla yoğun; temel alanlar üstte, vergi/etiket/not gibi ayrıntılar kontrollü açılmalı | İşlem formu |
| Çok geniş hesap türü ve hazır örnek hesap listesi | Alma | Kredi/ipotek/yatırım kapsamı çekirdek işi gömer; BusinessFinance yalnız kendi domain türlerini göstermeli | Hesap listesi ve oluşturma |
| Sıfır tutarı kabul etmek | Alma | Anlamsız finansal hareket oluşturuyor; istemci ve sunucu birlikte reddetmeli | Tüm para formları |
| Doğrudan düzenleme + kalıcı silme | Alma | Finansal geçmiş korunmuyor; BusinessFinance düzeltme ve iptal kaydı kullanır | Kayıt detayı |
| Transferi iki bağlı satır ve iki running balance ile gösterme | Uyarlayarak al | Kaynak/hedef etkisi güçlü; tek olay oldukları görsel bağla daha açık tutulmalı | Transfer detayı ve feed |
| İşletme/şahsi boyutunun olmaması | Alma | ADR 0013'ün temel ihtiyacını karşılamıyor | İşlem formu ve rapor filtresi |
| Tekrarlayan tanımının hiçbir şey üretmemesi + occurrence'ların bekleyen hatırlatıcı olması | Doğrudan al | BusinessFinance `RecurringTransaction` + occurrence → realize modeliyle birebir; tüm rakiplerin en yakını | Planlanan görünüm / tekrarlayan plan |
| Geçmiş tarihli tekrarlayanı otomatik yazmayıp "X gün gecikmeli" hatırlatıcı yapma | Doğrudan al | Geçmiş occurrence geçmiştir ama yine de açık onay ister; MM'in sessiz otomatik yazımının tersi (doğru olan bu) | Planlanan görünüm |
| Occurrence onayında "bugün mü / planlanan tarih mi" sorusu | Uyarlayarak al | Gecikmeli occurrence hangi döneme yazılacak — kullanıcıya sormak doğru; bizde realize isteği tutar/tarih taşıyor | Occurrence realize |
| Taksit planını N eşit parçaya bölüp her ayına bir kayıt/hatırlatıcı üretme | Doğrudan al | BusinessFinance `InstallmentPlan` per-item realize ile aynı; TR pazarında taksit yaygın | Kart harcaması / taksit planı |
| İlk taksiti otomatik yazma (kalanları hatırlatıcı) | Uyarlayarak al | Kolaylık ama bizde ilk taksit de açık realize ister — plan hiçbir şey üretmez ilkesi | Taksit planı kurulumu |
| Taksitte faiz oranı alanı | Henüz karar verme | KDV gibi taşınan bir alan olabilir; hesaplama yapılmamalı (ADR 0016 mantığı) | Taksit planı |
| Hatırlatıcılar sekmesi = tekrarlayan + taksit occurrence'larının tarih sıralı birleşik listesi | Doğrudan al | Bizim planlanan projeksiyonumuzla aynı iş; gecikme etiketleri `attentionCode`'a karşılık | Planlanan görünüm |
| "Yinelenmek" (kaydı çoğaltma) transfer detayında | Henüz karar verme | Benzer transferi hızlı tekrar; küçük kolaylık | Kayıt detayı |
| Fiş = ataç eki, OCR yok | Not | Bluecoins'te fiş okuma yok; ADR 0011 öneri katmanı bizde ayrı | İşlem eki |
| Kredi kartında ekstre kesim günü alanı | Uyarlayarak al | Bizde ekstre kesim/ödeme tarihinden asOf projeksiyon; kesim günü kullanıcının, kodda gömülü değil (ADR 0016 mantığı) | Kart hesabı |
| Bölmek: tek kaydı çok satıra bölme (her satır kendi kategori/hesap) | Henüz karar verme | Tek fişteki farklı kalemleri ayırmak için güçlü; bizde henüz yok | Kayıt detayı |
| Taslak uyarısı (kaydetmeden çıkışta onay) | Doğrudan al | Money Manager'da yoktu, veri kaybettiriyordu; bizde form terk edilirken uyarı olmalı | Tüm formlar |
| Cari hesabın sıradan bakiye hesabı olması, fatura nesnesi/tahsilat bağı olmaması | Alma | ADR 0014 cari modelimiz "tanır/taşır" ayrımı + fatura bağı ister; net bakiye tek başına yetmez | Cari hesap / fatura akışı |
| Bağımsız hatırlatıcı = "Bir Defa" Planlı İşlem, hepsi tek Hatırlatıcılar listesinde | Doğrudan al | Bizim planlanan projeksiyonumuzla aynı: tekrarlayan/taksit/borç/vergi hepsi tek `financial-activities/planned` görünümü | Planlanan görünüm |

## Kanıt ve güven düzeyi

- Manuel gözlem (Tur 1): K00–K08 emülatörde sentetik veriyle tamamlandı; bütün bakiye kontrol değerleri birebir tuttu.
- Manuel gözlem (Faz 3, 10 Eyl): tam yeniden koşum — 3 hesap + 5 çekirdek (kontrol ₺44.950 birebir) + **B2 taksit** (6 ay, ₺6.000 → 6×₺1.000, ilk taksit anında + 5 hatırlatıcı) + **B1 tekrarlayan** (aylık, geçmiş tarihli, tanım hiçbir şey üretmiyor, occurrence'lar bekliyor, Kaydet → materyalize). Kareler `10`–`28`.
- Manuel gözlem (Faz 3 ek, 10 Eyl): Bölmek/split modu + taslak uyarısı + Kredi Kartı kesim günü alanı + **bağımsız hatırlatıcı** (canlı) + **cari hesap** (canlı, fatura bağı yok) + **kısmi kart ödemesi** (canlı ₺500). Kareler `29`–`34`.
- Resmî kaynak: —
- Yorum: Bluecoins'in tekrarlayan + taksit + bağımsız hatırlatıcı modeli **tüm rakipler içinde BusinessFinance'e en yakın olanı** (tanım üretmez, occurrence realize edilir, hepsi tek planlanan görünüm). Buna karşılık cari hesap yalnız bir kasa (fatura/tahsilat bağı yok), bilgi mimarisi hedef esnaf için gereğinden geniş ve uygulama emülatörde kararsız (modal diyaloglar dokunuşu işlemiyor).
- Doğrulanamadı: Taksitte auto-checkbox ile "otomatik" kol, ekstre kesim gününün projeksiyona etkisi, yedek/geri yükleme, Android widget'ı, fiş ek akışının ekranı (emülatör overlay tuzağı), Premium dışa aktarmanın son adımı.

## Tek cümlelik sonuç

Bluecoins açılış bakiyesi, running balance, nötr transfer, dinamik kart/taksit
alanları ve **BusinessFinance'e birebir oturan tekrarlayan/taksit "tanım üretmez,
occurrence realize edilir" modeliyle** en güçlü para modeli referansı; ancak yoğun
formu, geniş hesap evreni, sıfır tutarı kabul etmesi, silme yaklaşımı ve emülatör
kararsızlığı BusinessFinance için sadeleştirilmesi ve dikkat edilmesi gereken noktalar.
