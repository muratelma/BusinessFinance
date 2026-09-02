# Araştırma Durumu ve Ortak Çalışma Akışı

Bu dosya çalışmanın **canlı panosudur**. Her oturuma başlarken önce buraya
bakılır; her görev/uygulama bitince buradaki tablo güncellenir.

## ▶ Sonraki oturum buradan başla (2 Eyl 2026 akşamı bırakıldı)

**Bırakılan nokta:** Üç Türk uygulaması (Paraşüt, Logo İşbaşı, KolayBi) için
resmî kaynak derlemesi bitti → `raporlar/turk-on-muhasebe-vs-businessfinance.md`.
Video linkleri kullanıcıya iletildi (aşağıda "Kullanıcıda bekleyen iş").

**Kullanıcıda bekleyen iş:** Kullanıcı Paraşüt / Logo İşbaşı / KolayBi
tanıtım-eğitim videolarını izleyip **ekran görüntüleri aldı** (henüz teslim
etmedi). Sonraki oturumda kullanıcı bu görselleri + notlarını verecek; ekran
seviyesi işleyiş (gelir-gider formu, cari, dashboard, fiş okutma, varsa
işletme/şahsi veya proje ayrımı) ilgili `gozlemler/*.md`'ye ve fark notuna
işlenecek. Görseller `kanitlar/<uygulama>/` altına protokol adıyla kaydedilir.

**Yapılacaklar sırası:**
1. Kullanıcının video ekran görüntülerini işle (3 uygulama).
2. **QuickBooks resmî kaynak** — tek kalan iş: Solopreneur işletme/şahsi işlem
   sınıflandırması (quickbooks.intuit.com/solopreneur, yardım merkezi
   "business vs personal"). ADR 0013 ile karşılaştırma için kritik.
3. Tur 1 tam biter → Tur 2 için 3 uygulama seç (`DURUM.md` "Tur 2" bölümü),
   gerekçesini yaz.
4. Deneme hesabı açılabilirse (Paraşüt/KolayBi 14 gün, kart yok — ama üyelik
   2. adımda kart isteyebilir) mobilde gerçek K01–K08.

**Not:** `stages/README.md` 2 Eyl'de güncellendi — **Aşama 06.2 artık Aktif**
(dar yerel web deneme checkpoint'i). Bu araştırma 06.2'nin geniş arayüz
gruplarını beslemeye devam ediyor; aşağıdaki "Sınır" satırı buna göre
güncellenmeli (araştırma hâlâ kod değiştirmez, yalnız kapsamı besler).

- Durum: Tur 1 — 3/7 manuel tamamlandı (Money Manager, Wallet, Bluecoins);
  4/7 erişim engelli. Resmî kaynak yazıldı: Paraşüt, Logo İşbaşı, KolayBi (2 Eyl).
  Kalan: **QuickBooks** resmî kaynak (işletme/şahsi sınıflandırma)
- **QuickBooks güncellemesi (2 Eyl 2026):** kullanıcı Intuit hesabı açtı; onboarding
  ödeme kapısında durdu (deneme bile kart istiyor). Hâlâ `Engelli → resmî kaynak`,
  ama onboarding akışı 4 ekran görüntüsüyle kanıtlandı
- **Erişim ayrımı (1 Eyl 2026):** Yerel uygulamalar manuel test ediliyor
  (Money Manager ✓, Wallet, Bluecoins). Türk ön muhasebe uygulamaları
  (Paraşüt, Logo İşbaşı, KolayBi) ve QuickBooks kayıt/bölge engelli →
  `resmî kaynak` masa başı araştırmasıyla incelenecek. Bu, protokolün
  öngördüğü yol; bulgular `Resmî kaynak` etiketi taşıyacak
- Emülatör: `emulator-5554` bağlı; yedi rakip uygulama ve iki BusinessFinance
  uygulaması kurulu (adb ile doğrulandı, 1 Eylül 2026)
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
| 1 | Money Manager (Realbyte) | `com.realbyteapps.moneymanagerfree` | Tamamlandı | 11 (+arayüz turu) | Yazıldı | Pilot bitti 1 Eyl; K00–K08 + arayüz taraması; kontrol değerleri birebir tuttu; işletme/şahsi ayrımı `Desteklenmiyor` |
| 2 | Paraşüt | `com.parasut` | Engelli → resmî kaynak | 1 (giriş ekranı) | Resmî kaynak yazıldı | Mobilde kayıt yok. 2 Eyl: parasut.com ile özellik/kapsam/kitle işlendi (geniş kapsamlı bulut ön muhasebe + e-dönüşüm; KOBİ/freelancer/e-ticaret; canlı muhasebeci erişimi; fiş OCR; tekrarlayan fatura; 14 gün ücretsiz deneme, kart istemiyor). İşletme/şahsi havuz kavramı yok |
| 3 | Logo İşbaşı | `com.isbasi` | Engelli → resmî kaynak | 4 (giriş/kayıt) | Taslak | Kayıt formu 3 alan (VKN yok); SMS doğrulama şart. 2 Eyl: kullanıcı gerçek kayıt oldu ama "hesabınız hazırlanıyor" pop-up'ı → anında giriş yok. Özellikler isbasi.com + tanıtım videosundan işlendi (8 entegrasyon, Müşavir Portal; `kanitlar/logo-isbasi/05`–`06`). 14 gün ücretsiz deneme var (kart istemiyor) — deneme maili gelirse gerçek teste alınabilir |
| 4 | KolayBi | `com.kolaybi.mobil` | Engelli → resmî kaynak | 1 (giriş) | Resmî kaynak yazıldı | Mobilde kayıt yok (kullanıcı doğruladı 2 Eyl). kolaybi.com ile işlendi: geniş kapsamlı bulut ön muhasebe + e-dönüşüm; KOBİ/start-up/**şahıs şirketi** paketleri; 25+ banka, sanal POS, **proje bazlı gelir-gider**, tekrarlayan işlem; 14 gün ücretsiz deneme. İşletme/şahsi havuz kavramı yok |
| 5 | QuickBooks | `com.intuit.quickbooks` | Engelli → resmî kaynak | 4 (onboarding→paywall) | Taslak | 2 Eyl: kullanıcı Intuit hesabı açtı (e-posta+SMS), onboarding'e girildi. "Basic info" = tek alan Business name. "Choose a plan" ödeme kapısı: Simple Start TRY 244,99/ay, 1 ay ücretsiz deneme bile kart istiyor. K01–K08 resmî kaynakla. (1 Eyl notu: auth webview siyah ekran) |
| 6 | Wallet by BudgetBakers | `com.droid4you.application.wallet` | Tamamlandı | 9 (+arayüz turu) | Yazıldı | 1 Eyl; K01–K08 + arayüz taraması. K00 önceden açık deneme hesabı nedeniyle kısmi. Transfer ve kart ödemesi raporda nötr; işletme/şahsi ayrımı `Desteklenmiyor`; cash/checking açılış bakiyesi almıyor |
| 7 | Bluecoins | `com.rammigsoftware.bluecoins` | Tamamlandı | 10 (+arayüz turu) | Yazıldı | 1 Eyl; K00–K08 + arayüz taraması. Kontrol bakiyeleri birebir; transfer/kart ödemesi nötr; işletme/şahsi ayrımı `Desteklenmiyor`; sıfır tutarlı kayıt kabul ediliyor |

Sonuç değerleri: `Başlanmadı` · `Sürüyor` · `Tamamlandı` · `Kısmi` · `Engelli`
· `Ücretli` · `Desteklenmiyor`.

## Türk ön muhasebe fark notu

Paraşüt / Logo İşbaşı / KolayBi içine girilemedi. Resmî kaynak derlemesi ve
BusinessFinance ile özellik/kapsam farkları:
`raporlar/turk-on-muhasebe-vs-businessfinance.md` (2 Eyl 2026). Tanıtım/eğitim
videosu linkleri kullanıcıya iletildi; kullanıcı izleyip ekran notu ekleyecek.

## Sentetik veri tarihi (uyulacak)

Kanonik dönem **Ağustos 2026**, işlem tarihleri 3/5/8/12/18 Ağu (`SENTETIK-TEST-VERISI.md`).
Money Manager oturumunda işlemler yanlışlıkla 01.09.2026'ya girildi (ay içi
toplamlar etkilenmedi, kontrol değerleri tuttu, yeniden girilmedi — form notu var).
Bundan sonraki her uygulama ve Tur 2 Ağustos 2026 + spec gün tarihlerini kullanır.

## Tur 2 (derin akış)

Tur 1 bitince en çok kanıt üreten üç uygulama seçilir. Adaylar ve seçim
gerekçesi buraya yazılır.

- Seçilen üç uygulama: (Tur 1 sonrası)
- Ek olaylar: `SENTETIK-TEST-VERISI.md` → D1–D4

## Üç belge

Taslak ve onay durumu `raporlar/README.md` içindeki tabloda tutulur.
