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

**Şu an:** Faz 0 tamam (plan yazıldı, commit). Sıradaki → **Faz 1: Money Manager
boşluk koşumu** (emülatörde, yapay zekâ sürer). Ayrıntı yol haritasında.

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
7. ⏳ **Faz 1–3:** Money Manager → Wallet → Bluecoins boşluk koşumu (emülatör).
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
| 1 | Money Manager (Realbyte) | `com.realbyteapps.moneymanagerfree` | Tamamlandı | 11 (+arayüz turu) | Yazıldı | Pilot bitti 1 Eyl; K00–K08 + arayüz taraması; kontrol değerleri birebir tuttu; işletme/şahsi ayrımı `Desteklenmiyor` |
| 2 | Paraşüt | `com.parasut` | Girilemedi → resmî kaynak | 3 karusel + 7 video karesi (web sürümü) | Yazıldı + 9 Eyl derinleştirildi + **10 Eyl video akışı** | Mobilde kayıt yok, web'de ücretli. 5 gider türü, kayıt≠ödeme ayrımı (video: "Tahsil edildi" vs "Kalan"), otomatik mahsup, nakit akışı ≠ gelir-gider ayrı rapor, KDV faturadan hesaplanıyor (ADR 0016 farkı). İşletme/şahsi havuz kavramı yok — video baştan sona firma defteri. **Tur 2:** 36 dk eğitim videosundan gerçek arayüz |
| 3 | Logo İşbaşı | `com.isbasi` | Girilemedi → resmî kaynak | 6 (giriş/kayıt + 2 video karesi) | Yazıldı + 9 Eyl derinleştirildi | Kayıt 3 alan (VKN yok); SMS + "hesabınız hazırlanıyor" satış süreci. Tek kayıt → cari + kasa-banka + stok üç defteri besler; sesli komut; Müşavir Portal canlı. Ücretli. **isbasi.com kullanım videoları notu bekleniyor** |
| 4 | KolayBi | `com.kolaybi.mobil` | Girilemedi → resmî kaynak | 1 giriş + 7 video karesi (~2020 web + 1 güncel 2026) + transkript | Yazıldı + 9 Eyl derinleştirildi + **10 Eyl video + transkript** | Mobilde kayıt yok. Güncel Durum panosu (nakit akışı + vadesi gelmemiş/geçmiş/belirsiz), kurulum sırası (cari→ürün→finans), **"Ortaklar/Personel Carileri"** ile patron parası (ADR 0013 farkının 2. kanıtı), KDV üründen hesaplanıyor, kısmi ödeme ekranda. Proje ekranı videoda yok → Tur 2 |
| 5 | QuickBooks Solopreneur | `com.intuit.quickbooks` | Girilemedi → resmî kaynak | 4 (onboarding→paywall) | **Yazıldı (9 Eyl)** — yeni şablonla | İşlem başına tek "Type: Business/Personal" alanı → **ADR 0013'ün kavramsal en yakın rakibi**. Ama ayrım ABD Schedule C vergi eksenli; banka bağlantısı + vergi hesaplama bizde kapsam dışı. Split transaction (kalem başına işletme/şahsi) + Rules motoru + tahmini vergi |
| 6 | Wallet by BudgetBakers | `com.droid4you.application.wallet` | Tamamlandı | 9 (+arayüz turu) | Yazıldı | 1 Eyl; K01–K08 + arayüz taraması. K00 önceden açık deneme hesabı nedeniyle kısmi. Transfer ve kart ödemesi raporda nötr; işletme/şahsi ayrımı `Desteklenmiyor`; cash/checking açılış bakiyesi almıyor |
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
Money Manager oturumunda işlemler yanlışlıkla 01.09.2026'ya girildi (ay içi
toplamlar etkilenmedi, kontrol değerleri tuttu, yeniden girilmedi — form notu var).
Bundan sonraki her uygulama ve Tur 2 Ağustos 2026 + spec gün tarihlerini kullanır.

## Tur 2 (derin akış)

**Yapı (10 Eyl 2026, `TUR2-YOL-HARITASI.md`):** Önce sürülebilir 3 uygulamada
boşluk koşumu (Faz 1–3), sonra Goodbudget (Faz 4), sonra Tur 2'nin 3 uygulaması
seçilir (Faz 5). Nihai yapı: **KolayBi (masa başı) + {Money Manager | Wallet |
Bluecoins}'ten 1 + Goodbudget (elle bakıştan geçerse).**

| Uygulama | Rolü | Boşluk koşumu odağı |
|---|---|---|
| Money Manager | "Yapımıza en yakın" adayı — para modeli birebir; kart ekstre projeksiyon modeli hiç sınanmadı | Kart "Pay"/Settlement/Payment, Balance Payable vs Outstanding, Rep/Inst, bütçe, export; **tarih düzeltmesi** |
| Wallet (BudgetBakers) | Finansal UX referansı — dashboard, rapor okunabilirliği | K00 onboarding, Planned/Debts/Goals/Budgets oluşturma, split, export |
| Bluecoins | En geniş para modeli — net varlık, borç, hatırlatıcı, CSV | Planlı işlem, taksit, cari↔tahsilat, hatırlatıcı, yedek/geri yükleme, export |
| Goodbudget (yeni) | "Ön muhasebe dışı, bütçe odaklı, bizden farklı" — dijital zarf, "harcamadan önce dağıt" | Faz 4: K00–K08 + arayüz taraması (kullanıcı önce elle bakar) |
| KolayBi | Türk ön muhasebe temsilcisi (masa başı) | Faz 6: Kullanım Rehberi videolarından proje ekranı, gider formu, cari ekstre |

- Ek olaylar: `SENTETIK-TEST-VERISI.md` → D1–D4 (Faz 7, seçilen 3 uygulamada)
- Paraşüt / Logo / QuickBooks: masa başı seviyesinde kalır, Belge 1–2'ye girer

## Üç belge

Taslak ve onay durumu `raporlar/README.md` içindeki tabloda tutulur.
