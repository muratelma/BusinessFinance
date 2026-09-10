# Araştırma Durumu ve Ortak Çalışma Akışı

Bu dosya çalışmanın **canlı panosudur**. Her oturuma başlarken önce buraya
bakılır; her görev/uygulama bitince buradaki tablo güncellenir.

## ▶ Sonraki oturum buradan başla (10 Eyl 2026 — boşluk koşumu + Tur 2 planı)

**Aktif plan: `TUR2-YOL-HARITASI.md`** — 8 fazlı çalışma planı orada. Kısaca:
Tur 1 video notları bitti; şimdi sürülebilir 3 uygulamanın (Money Manager,
Wallet, Bluecoins) form boşlukları **orta derinlikte** dolduruluyor (Faz 1–3),
sonra yeni uygulama Goodbudget test ediliyor (Faz 4), sonra Tur 2'nin 3 uygulaması
seçiliyor (Faz 5). KolayBi masa başı derinleştirmesi paralel (Faz 6). Tur 2
derin koşum Faz 7, belgeler Faz 8.

**Şu an:** Faz 0 + **Faz 1 (Money Manager) + Faz 2 (Wallet) TAM tamam** (10 Eyl).

- **Faz 1 — Money Manager:** yeni emülatörde Türkçe; 5 çekirdek + B1 tekrarlayan +
  B2 taksit + kısmi kart ödemesi canlı; kontrol ₺44.950 tuttu. Kart ekstre modeli
  = bizim projeksiyon modeli; tekrarlayan geçmişi otomatik yazıyor (bizden fark),
  taksit ay ay bölünüyor (bizimle aynı). 25 Türkçe kare (`02`–`25`).
- **Faz 2 — Wallet:** kullanıcı giriş yaptı, bulut verisi geri geldi; çekirdek 5
  doğrulandı (net ₺22.950). **Kredi kartı = basit negatif bakiye** (ekstre dönemi
  yok). **B1 tekrarlayan** = Planned payments/Recurrent; geçmiş tarihe kurulamıyor;
  her örnek bekleyen + Confirm/Postpone/Dismiss (BusinessFinance realize'ine en
  yakın rakip); ilk onayda "otomatik mi/onaylı mı" plan bazında soruluyor.
  **B2 taksit = özellik YOK** (₺6.000 tek parça borç + tek parça gider).
  Budget + Goal canlı kuruldu; Debt formu görüldü (hesap zorunlu, bakiye hareket
  eder — ADR 0014'ün tersi); split akışı görüldü; **fiş OCR yok** (dosya/foto eki).
  UI İngilizce (sistem dilini almıyor). 31 kare (`10`–`40`). Veri sonu: net ₺16.350.

**Sıradaki → Faz 3: Bluecoins boşluk koşumu.**

---

## Eski giriş (9 Eyl 2026 — yeni PC, kapsam derinleştirildi)

**Kapsam açıklaması (10 Eyl 2026):** Raporlar patronlara sunulacak ve **ürün
kapsamını patron belirler**. Bu yüzden Belge 1 (arayüz) ve Belge 2 (akış)
rakibin **tüm özellik yüzeyini** tarafsız anlatır — stok/depo, e-İrsaliye,
e-ticaret pazaryeri entegrasyonu, banka bağlama, KDV hesaplama dahil. "Bizde
kapsam dışı" damgası bu iki belgeye **girmez**; karar filtresi yalnız Belge
3'te uygulanır (`alma` / `henüz karar verme` bir öneri kararıdır, bir gözlem
değil). Kurucu ADR'yle çakışan özellik sessizce atılmaz, gerekçesiyle yazılır.
Belgeleme irtifası: özellik alanı başına 1 temsili kare + kısa not; çok ekranlı
derin pipeline yalnız ürüne yakın akışlara saklı (fatura→tahsilat, cari mahsup,
tekrarlayan).

**Kapsam kararı (9 Eyl 2026):** Bu çalışmanın amacı üç Word/PDF rapor üretmek;
raporlar arayüz + işleyiş + **arka plan olay modeli / pipeline** + akışların
birbirine bağlanışı boyutlarını taşımalı. Bu yüzden:
- Gözlem şablonuna üç bölüm eklendi: **Ürün kimliği ve asıl amaç**,
  **Sistem işleyişi / pipeline**, **Video/doküman akış yeniden kurulumu**
  (yalnız resmî kaynak uygulamalar).
- İçine girilemeyen uygulamalarda (Paraşüt, Logo İşbaşı, KolayBi, QuickBooks)
  akış, yardım merkezi adım adım makaleleri + ürün turu + kullanıcının izlediği
  tanıtım/eğitim videolarından yeniden kurulur; hepsi `Resmî kaynak` etiketli.
- **Deneme hesabı yolu kapatıldı:** Paraşüt/KolayBi kaydı web'de ve
  ücretli/hazırlık kapısı var; Logo kaydı "hesabınız hazırlanıyor" satış
  sürecine giriyor. Bu uygulamalara girilmeyecek — inceleme tümüyle resmî kaynak.
- Ekran görüntüleri artık **repoya dâhil** (`kanitlar/` altındaki PNG'ler;
  `.gitignore` yalnız video/ses tutuyor). Araştırma bitince `research/` klasörü
  silinecek.

**Tur 1 video notları tamam (10 Eyl 2026):** Logo (`logo-isbasi/05`–`06`,
2 Eyl), Paraşüt (`parasut/02`–`08`, 10 Eyl), KolayBi (`kolaybi/02`–`08` +
transkript, 10 Eyl). QuickBooks'ta video yok (paywall; onboarding kareleri
`quickbooks/01`–`04`).

**Transkript yöntemi (10 Eyl'den itibaren):** YouTube kare/transkript çekimi
yapay zekâ tarafında çalışmıyor. Kullanıcı transkripti bir araçla çıkarır ve
videoyla karşılaştırıp doğrular; ekran kareleri + transkripti verir; yapay zekâ
ikisini eşleyerek forma işler. Görseller `kanitlar/<uygulama>/` altına protokol
adıyla, transkript ilgili gözlem formunun "Ek — video transkripti" bölümüne.

**Yapılacaklar sırası:**
1. ✅ (9 Eyl) Gözlem şablonu + `README.md` + `raporlar/README.md` + protokol
   kapsamı güncellendi (yeni bölümler).
2. ✅ (9 Eyl) Paraşüt, Logo İşbaşı, KolayBi formları metin kaynaklarından
   derinleştirildi (ürün kimliği + sistem işleyişi/pipeline).
3. ✅ (9 Eyl) QuickBooks Solopreneur resmî kaynak formu yeni şablonla yazıldı
   (işletme/şahsi = işlem başına "Type" alanı; ADR 0013 kıyası).
4. ✅ (9 Eyl) Ekran görüntüleri repoya alındı (44 PNG stage'lendi; commit izni bekliyor).
5a. ✅ (10 Eyl) Paraşüt tanıtım videosu işlendi — 7 kare (`kanitlar/parasut/02`–`08`,
   web sürümü), `gozlemler/parasut.md` video bölümü dolduruldu.
5b. ✅ (10 Eyl) KolayBi tanıtım videosu + transkript işlendi — 7 kare
   (`kanitlar/kolaybi/02`–`08`, ~2020 web + 1 güncel 2026 kare),
   `gozlemler/kolaybi.md` video bölümü + transkript eki dolduruldu.
6. ✅ (10 Eyl) Tur 1 video notları kapandı; boşluk koşumu + Tur 2 planı
   `TUR2-YOL-HARITASI.md` içine yazıldı, Goodbudget yeni uygulama olarak seçildi.
7a. ✅ (10 Eyl) **Faz 1:** Money Manager boşluk koşumu TAM — Türkçe arayüz,
   Ağustos tarihli 5 işlem + B1 tekrarlayan + B2 taksit + kısmi kart ödemesi,
   kart ekstre modeli, açılış bakiyesi. 25 Türkçe kare, form güncellendi.
7b. ✅ (10 Eyl) **Faz 2:** Wallet boşluk koşumu TAM — çekirdek doğrulama +
   kredi kartı negatif-bakiye modeli + B1 tekrarlayan (bekleyen/Confirm) +
   B2 taksit (özellik yok) + Budget/Goal canlı kurulum + Debt formu + split +
   fiş OCR yok. 31 kare (`10`–`40`), form güncellendi.
7c. ⏳ **Faz 3:** Bluecoins boşluk koşumu (emülatör).
8. ⏳ **Faz 4:** Goodbudget (kullanıcı önce elle bakar) → Faz 5 Tur 2 seçimi.
7. Belge 1 ve 2 taslakları yazılır → onaydan sonra Belge 3.

**Not:** `stages/README.md` 2 Eyl'de güncellendi — **Aşama 06.2 Aktif** (dar
yerel web deneme checkpoint'i). Bu araştırma 06.2'nin geniş arayüz gruplarını
beslemeye devam ediyor; araştırma hâlâ kod değiştirmez, yalnız kapsamı besler.

- Durum: Tur 1 — 3/7 manuel tamamlandı (Money Manager, Wallet, Bluecoins).
  4/7 uygulamaya girilemedi → resmî kaynak formları yazıldı, 9 Eyl'de
  pipeline boyutuyla derinleştirildi, 10 Eyl'de video akışlarıyla tamamlandı
  (Paraşüt + KolayBi tanıtım videosu; Logo 2 Eyl'de; QuickBooks'ta video yok).
  **Kalan:** Tur 1 gözden geçirme + commit + Tur 2 seçimi.
- **QuickBooks (2 Eyl 2026):** kullanıcı Intuit hesabı açtı; onboarding ödeme
  kapısında durdu (ücretli plan zorunlu). `Engelli → resmî kaynak`; onboarding
  akışı 4 ekran görüntüsüyle kanıtlı (`kanitlar/quickbooks/01`–`04`)
- **Erişim ayrımı:** Yerel uygulamalar manuel test edildi (Money Manager,
  Wallet, Bluecoins). Türk ön muhasebe uygulamaları ve QuickBooks kayıt/bölge
  engelli → `resmî kaynak`. Bu, protokolün öngördüğü yol
- Emülatör: Pixel_8 AVD (yeni PC'de kuruldu, 9 Eyl 2026); yedi rakip + iki
  BusinessFinance uygulaması yüklü. Sürüş yöntemi: ham adb (bkz. proje hafızası
  "Emülatör rakip araştırma ortamı")
- Sınır: Bu çalışma Aşama 06.2'yi açmaz, uygulama kodunu değiştirmez, yalnız
  sentetik veri kullanır

## Ortak çalışma akışı

Yapay zekâ ve kullanıcı **aynı anda** çalışır; kullanıcı emülatörü canlı izler.

### Uygulama başına döngü

1. **Yapay zekâ** uygulamayı emülatörde açar, `MANUEL-TEST-PROTOKOLU.md`
   sırasını (K00→K08) izler.
2. Her görevde: ekran görüntüsü alınır → `kanitlar/<uygulama>/` içine protokol
   adıyla kaydedilir (`00-magaza.png` …). Gözlem satırı forma yazılır.
3. **Kayıt / SMS / e-posta / VKN / ücretli plan kapısına gelince yapay zekâ
   durur** ve kullanıcıya söyler ("K00'da telefon doğrulaması istiyor").
   Kullanıcı o adımı yapar, "devam" der, gerekiyorsa kaldığı yeri söyler.
4. **Kullanıcı** her an araya girip yönlendirebilir ("şu ekrana bak", "burada
   şu garip", "şunu da dene"). Yapay zekâ forma ekler.
5. Görev bitince `gozlemler/<uygulama>.md` içindeki ilgili satır doldurulur.
6. Uygulama bitince form tamamlanır, bu dosyadaki tablo güncellenir, kullanıcı
   formu onaylar. Sonra sıradaki uygulamaya geçilir.

### Roller

| Yapay zekâ | Kullanıcı |
|---|---|
| Emülatörü sürme, ekran görüntüsü, not alma | Kayıt / SMS / e-posta / VKN / ödeme kapıları |
| Gözlem formlarını ve üç belgeyi yazma | "Şunu da dene" yönlendirmesi, kaldığı yeri bildirme |
| Resmî kaynak masa başı araştırması | Yapay zekânın yorumlarını ve kararları onaylama |

### Kanıt etiketi (her bulguda zorunlu)

`Manuel gözlem` · `Resmî kaynak` · `Yorum` · `Doğrulanamadı`.
Yorum, gözlenmiş ürün davranışı gibi yazılmaz.

## Tur 1 durum tablosu

| # | Uygulama | Paket | Tur 1 (K00–K08) | Ekran görüntüleri | Gözlem formu | Not |
|---|---|---|---|---|---|---|
| 1 | Money Manager (Realbyte) | `com.realbyteapps.moneymanagerfree` | Tamamlandı + **Faz 1** | 15 (Türkçe, `02`–`16`) | Yazıldı + **10 Eyl Faz 1** | Pilot 1 Eyl; **Faz 1 (10 Eyl):** yeni emülatörde Türkçe, Ağustos tarihli 5 işlem, kontrol değerleri tuttu (net ₺44.950). Kart ekstre modeli "Bu Ay/Gelecek Ay" = Balance Payable vs Outstanding; "Ödeme" butonu = ön doldurulmuş Havale; Tekrarlama (14 seçenek)/Taksit ayrı; açılış bakiyesi "Bakiye Farkı". İşletme/şahsi `Desteklenmiyor` |
| 2 | Paraşüt | `com.parasut` | Girilemedi → resmî kaynak | 3 karusel + 7 video karesi (web sürümü) | Yazıldı + 9 Eyl derinleştirildi + **10 Eyl video akışı** | Mobilde kayıt yok, web'de ücretli. 5 gider türü, kayıt≠ödeme ayrımı (video: "Tahsil edildi" vs "Kalan"), otomatik mahsup, nakit akışı ≠ gelir-gider ayrı rapor, KDV faturadan hesaplanıyor (ADR 0016 farkı). İşletme/şahsi havuz kavramı yok — video baştan sona firma defteri. **Tur 2:** 36 dk eğitim videosundan gerçek arayüz |
| 3 | Logo İşbaşı | `com.isbasi` | Girilemedi → resmî kaynak | 6 (giriş/kayıt + 2 video karesi) | Yazıldı + 9 Eyl derinleştirildi | Kayıt 3 alan (VKN yok); SMS + "hesabınız hazırlanıyor" satış süreci. Tek kayıt → cari + kasa-banka + stok üç defteri besler; sesli komut; Müşavir Portal canlı. Ücretli. **isbasi.com kullanım videoları notu bekleniyor** |
| 4 | KolayBi | `com.kolaybi.mobil` | Girilemedi → resmî kaynak | 1 giriş + 7 video karesi (~2020 web + 1 güncel 2026) + transkript | Yazıldı + 9 Eyl derinleştirildi + **10 Eyl video + transkript** | Mobilde kayıt yok. Güncel Durum panosu (nakit akışı + vadesi gelmemiş/geçmiş/belirsiz), kurulum sırası (cari→ürün→finans), **"Ortaklar/Personel Carileri"** ile patron parası (ADR 0013 farkının 2. kanıtı), KDV üründen hesaplanıyor, kısmi ödeme ekranda. Proje ekranı videoda yok → Tur 2 |
| 5 | QuickBooks Solopreneur | `com.intuit.quickbooks` | Girilemedi → resmî kaynak | 4 (onboarding→paywall) | **Yazıldı (9 Eyl)** — yeni şablonla | İşlem başına tek "Type: Business/Personal" alanı → **ADR 0013'ün kavramsal en yakın rakibi**. Ama ayrım ABD Schedule C vergi eksenli; banka bağlantısı + vergi hesaplama bizde kapsam dışı. Split transaction (kalem başına işletme/şahsi) + Rules motoru + tahmini vergi |
| 6 | Wallet by BudgetBakers | `com.droid4you.application.wallet` | Tamamlandı + **Faz 2** | 9 + **31 (`10`–`40`)** | Yazıldı + **10 Eyl Faz 2** | 1 Eyl K01–K08 + arayüz taraması. **Faz 2 (10 Eyl):** çekirdek doğrulama (net ₺22.950); kredi kartı = dönemsiz negatif bakiye; B1 tekrarlayan = Planned payments/Recurrent — geçmişe kurulamaz, her örnek bekleyen + Confirm/Postpone/Dismiss (realize'e en yakın), ilk onayda otomatik/onaylı plan-bazlı seçim; **B2 taksit özelliği YOK** (tek parça); Budget (kategori+hesap filtresi) + Goal canlı; Debt hesap zorunlu → bakiye hareket eder; fiş OCR yok. UI İngilizce. İşletme/şahsi `Desteklenmiyor` |
| 7 | Bluecoins | `com.rammigsoftware.bluecoins` | Tamamlandı | 10 (+arayüz turu) | Yazıldı | 1 Eyl; K00–K08 + arayüz taraması. Kontrol bakiyeleri birebir; transfer/kart ödemesi nötr; işletme/şahsi ayrımı `Desteklenmiyor`; sıfır tutarlı kayıt kabul ediliyor |

Sonuç değerleri: `Başlanmadı` · `Sürüyor` · `Tamamlandı` · `Kısmi` · `Engelli`
· `Ücretli` · `Desteklenmiyor`.

## Türk ön muhasebe fark notu

Paraşüt / Logo İşbaşı / KolayBi içine girilemedi (kayıt web'de, ücretli).
Resmî kaynak derlemesi ve BusinessFinance ile özellik/kapsam farkları:
`raporlar/turk-on-muhasebe-vs-businessfinance.md` (2 Eyl 2026, 9 Eyl'de sistem
işleyişi/pipeline boyutuyla genişletildi). Logo, Paraşüt ve KolayBi tanıtım
videoları işlendi (10 Eyl); raporun video bulgularıyla güncellenmesi Tur 1
kapanışında.

## Sentetik veri tarihi (uyulacak)

Kanonik dönem **Ağustos 2026**, işlem tarihleri 3/5/8/12/18 Ağu (`SENTETIK-TEST-VERISI.md`).
Money Manager Tur 1'de işlemler yanlışlıkla 01.09'a girilmişti; **Faz 1'de (10 Eyl)
yeni emülatörde doğru Ağustos tarihleriyle yeniden girildi** — sapma kapandı.
Bundan sonraki her uygulama ve Tur 2 Ağustos 2026 + spec gün tarihlerini kullanır.

## Tur 2 (derin akış)

**Yapı (10 Eyl 2026, `TUR2-YOL-HARITASI.md`):** Önce sürülebilir 3 uygulamada
boşluk koşumu (Faz 1–3), sonra Goodbudget (Faz 4), sonra Tur 2'nin 3 uygulaması
seçilir (Faz 5). Nihai yapı: **KolayBi (masa başı) + {Money Manager | Wallet |
Bluecoins}'ten 1 + Goodbudget (elle bakıştan geçerse).**

**Boşluk koşumu standart kalemleri (her uygulamada canlı):** çekirdek 5 işlem
(Ağustos tarihli) + kredi kartı harcaması/ödemesi + **B1 tekrarlayan** + **B2
taksit** (`SENTETIK-TEST-VERISI.md`) + formun kendi eksik listesi.
**Kapsam dışı:** export, yedek/geri yükleme, bütçe kurulum ekranı.

| Uygulama | Rolü | Boşluk koşumu odağı |
|---|---|---|
| Money Manager | "Yapımıza en yakın" adayı — para modeli birebir | ✅ çekirdek + kart modeli + Ödeme + Tekrar/Taksit menüsü + açılış bakiyesi (10 Eyl). **Kalan:** B1/B2 canlı kurma |
| Wallet (BudgetBakers) | Finansal UX referansı — dashboard, rapor okunabilirliği; tekrarlayan "bekleyen→onayla" bize çok yakın | ✅ Faz 2 TAM (10 Eyl). Artı: BusinessFinance'e yakın tekrarlayan akış. Eksi: taksit yok, dönemsiz kart, İngilizce UI, işletme/şahsi yok |
| Bluecoins | En geniş para modeli — net varlık, borç, hatırlatıcı | Çekirdek + kart + B1/B2 + cari↔tahsilat + hatırlatıcı |
| Goodbudget (yeni) | "Ön muhasebe dışı, bütçe odaklı, bizden farklı" — dijital zarf | Faz 4: K00–K08 + arayüz taraması (kullanıcı önce elle bakar) |
| KolayBi | Türk ön muhasebe temsilcisi (masa başı) | Faz 6: Kullanım Rehberi videolarından proje ekranı, gider formu, cari ekstre |

- Ek olaylar: `SENTETIK-TEST-VERISI.md` → D1–D4 (Faz 7, seçilen 3 uygulamada)
- Paraşüt / Logo / QuickBooks: masa başı seviyesinde kalır, Belge 1–2'ye girer

## Üç belge

Taslak ve onay durumu `raporlar/README.md` içindeki tabloda tutulur.
