# Araştırma Durumu ve Ortak Çalışma Akışı

Bu dosya çalışmanın **canlı panosudur**. Her oturuma başlarken önce buraya
bakılır; her görev/uygulama bitince buradaki tablo güncellenir.

## ▶ Sonraki oturum buradan başla (10 Eyl 2026 — Faz 1–3 bitti, sıradaki Faz 4)

**Aktif plan: `TUR2-YOL-HARITASI.md`** — 8 fazlı çalışma planı orada. Kısaca:
Tur 1 video notları bitti; sürülebilir 3 uygulamanın (Money Manager, Wallet,
Bluecoins) form boşlukları **orta derinlikte** dolduruldu (Faz 1–3 TAM), sıradaki
Faz 4 = **iki aday uygulamayı elle gez** (Hesap Defterim + Goodbudget), sonra
Tur 2'nin 3 uygulaması seçilir (Faz 5). KolayBi masa başı derinleştirmesi paralel
(Faz 6). Tur 2 derin koşum Faz 7, belgeler Faz 8.

**Sıradaki adım (kullanıcı):** Emülatörde `cashbook.cashbook` (Hesap Defterim) +
`com.dayspringtech.envelopes` (Goodbudget) yüklü. Önce **Hesap Defterim'i** ~15 dk
gez (kayıt istemiyor), sonra **Goodbudget** (household hesabı açman gerekecek).
İkisini kıyasla → Tur 2'nin 3. ("bize benzemeyen") slotu için hangisi Belge 3'e
daha çok katıyor. **Yapay zekâ önerisi:** Bluecoins'i "yapımıza en yakın" slota
kilitle; 3. slot için Hesap Defterim (TR/esnaf ilgisi) Goodbudget'a göre daha
güçlü aday. Faz 5'te kesinleşir. Ayrıntı: aşağıdaki "Kilit bulgular" + `TUR2-YOL-HARITASI.md`.

**Şu an:** Faz 0 + **Faz 1 (Money Manager) + Faz 2 (Wallet) + Faz 3 (Bluecoins) TAM**
(10 Eyl) — boşluk koşumları + ek koşum kalemleri dâhil. Commit'ler: `8478f05`,
`a00f23e`, `1d7a1de`, `1512f88`.

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
  Budget + Goal canlı kuruldu; **Debt canlı** (I Lent ₺5.000; kaydederken "Record
  oluştur → bakiye değişir" No/Yes seçeneği — bakiye hareketi borç bazında opsiyon,
  zorunlu değil); split akışı görüldü; **fiş OCR yok** (dosya/foto eki); plan
  silmede gerçekleşmiş occurrence uyarısı yok. UI İngilizce. 38 kare (`10`–`47`).
  Veri sonu: net ₺11.350.

- **Faz 3 — Bluecoins:** bulut yok → tam yeniden koşum. 3 hesap + 5 çekirdek
  (kontrol ₺44.950 birebir). **B2 taksit** = oran + ay sayısı (2–24 + Özel) +
  ilk ödeme; ₺6.000 → 6×₺1.000, taksit 1/6 anında + 5 aylık hatırlatıcı
  (InstallmentPlan'a çok yakın). **B1 tekrarlayan** = "Planlı İşlemler"; geçmiş
  tarihe kurulabiliyor, **tanım hiçbir şey üretmez**, tüm occurrence'lar
  Hatırlatıcılar'da bekliyor, Kaydet → "bugün/planlanan tarih" → materyalize.
  **Tüm rakiplerin BusinessFinance'e en yakın tekrarlayan/taksit modeli.**
  Ek koşum (10 Eyl): bağımsız hatırlatıcı canlı; **cari hesap = sıradan bakiye
  hesabı, fatura/tahsilat bağı YOK**; Bölmek/split; Kredi Kartı ekstre kesim günü
  alanı; kısmi kart ödemesi canlı. Emülatörde kararsız (modal diyaloglar). 34 kare
  (`10`–`34`). Veri sonu: net ₺43.350.

**Sıradaki → Faz 4: Hesap Defterim + Goodbudget elle gezinti (kullanıcı) → 3. slot kararı.**

---

## Kilit bulgular (10 Eyl 2026 — Belge 3'ün omurgası)

Bu bölüm tek tek gözlem formlarına dağılmış ana sonuçları konsolide eder.
Belge 3 (öneri) bunların üstüne kurulur.

### 1. Pazar boşluğu — çalışmanın manşet bulgusu

**"Şahıs şirketi / esnaf; işletme ve şahsi para hukuken tek havuz; ayrım bir
raporlama boyutudur" (ADR 0013) konumunu taşıyan uygulama pazarda YOK.**

| Grup | İşletme/şahsi ayrımı nasıl ele alıyor | Kaynak |
|---|---|---|
| Kişisel/bütçe uygulamaları (Money Manager, Wallet, Bluecoins, Goodbudget) | **Hiç yok.** Kapsam boyutu yok; en fazla etiket/ayrı defter | `gozlemler/money-manager.md`, `wallet-budgetbakers.md`, `bluecoins.md` |
| Türk ön muhasebe (Paraşüt, KolayBi, Logo İşbaşı) | İşletme ayrı tüzel kişi. Patronun parası **"ortak/personel carisi" workaround'uyla** giriyor | `kolaybi.md` ("Ortaklar/Personel Carileri"), `logo-isbasi.md` ("çekilen para / ortak cari"), `parasut.md` |
| QuickBooks Solopreneur | **En yakını:** tek akışta işlem başına `Type: Business/Personal`. Ama **ABD vergi ekseni** (Schedule C) + banka bağlama merkezli + ABD dışına kapalı | `gozlemler/quickbooks.md` |
| Hesap Defterim (Cash Book) | **3. bir yol:** işletme/şahsi ayrı **defterlerle** ayrılıyor (boyut değil) | masabaşı analiz, henüz koşulmadı |

→ Rakiplerin "ortak carisi" workaround'u **bizim tezimizin gerekçesi**: patronun
cebi ayrı bir cari değil, tek havuzda `Personal` kapsamı.

### 2. Hiçbir kişisel/bütçe uygulamasında "küçük işletme modu" yok

Money Manager, Wallet, Bluecoins, Goodbudget — dördünde de işletme ayarı /
rehberli işletme kurulumu yok. Dolaylı yollar: MM ücretli çoklu-defter · Wallet
ayrı workspace + label · Bluecoins işletme-kokan hesap tipleri (Alacaklar, Cari
hesap) · Goodbudget hiç. İşletme için gerçekten kurulmuş olanlar zaten ön
muhasebe kulvarı (Paraşüt/KolayBi/Logo/QuickBooks) — hepsi VKN + ücretli.

### 3. Tekrarlayan / planlanan modeli — Bluecoins bize birebir

**"Tanım hiçbir şey üretmez → her occurrence açık realize edilir; geçmiş/bugün/
gelecek hepsi tek planlanan görünümde bekler"** — BusinessFinance `RecurringTransaction`
+ `InstallmentPlan` + planlanan projeksiyon modelinin tam karşılığı. Bluecoins
bunu en net yapan (MM geçmişi sessizce otomatik yazıyor = bizden fark; Wallet
geçmişe kuramıyor). Wallet'ın "bekleyen → Confirm/Postpone/Dismiss" akışı da yakın.

### 4. Taksit — TR pazarı için ayırt edici

Bluecoins gerçek taksit özelliği taşıyor (oran + ay sayısı + ilk ödeme → N eşit
parça, ilki anında + kalanı hatırlatıcı). Money Manager taksiti ay ay ekstreye
bölüyor. **Wallet'ta taksit YOK** (₺6.000 tek parça). Ön muhasebe tarafında
taksit var. Kişisel uygulamaların çoğu taksit kavramını taşımıyor.

### 5. Alınabilecek kenar örnekleri (Belge 3 "uyarlayarak al")

- Bluecoins: tekrarlayan/taksit realize modeli, birleşik "Hatırlatıcılar" listesi,
  Kredi Kartı ekstre kesim günü, açılış bakiyesi + tarih aynı formda
- Wallet: dashboard / rapor "kullanıcı sorusu" başlıkları, yönlendirici boş durumlar,
  "bekleyen → onayla" tekrarlayan UX, Debt mini-defteri (Records + açık Total)
- Money Manager: kart "Bu Ay / Gelecek Ay" ekstre görünümü, ön doldurulmuş "Ödeme"
- KolayBi: proje bazlı gelir-gider = ikinci raporlama ekseni (opsiyonel gelecek)
- Logo: düşük sürtünmeli kayıt (3 alan, VKN yok)

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
   B2 taksit (özellik yok) + Budget/Goal canlı + **Debt canlı** (Record/bakiye
   opsiyonu) + split + fiş OCR yok + plan silme uyarısız. 38 kare (`10`–`47`).
7c. ✅ (10 Eyl) **Faz 3:** Bluecoins boşluk koşumu TAM — tam yeniden koşum
   (bulut yok), çekirdek doğrulama + B2 taksit (oran/ay/ilk-ödeme, 6×₺1.000,
   1/6 anında + hatırlatıcılar) + B1 tekrarlayan (geçmişe kurulur, tanım üretmez,
   occurrence realize) + **bağımsız hatırlatıcı** + **cari hesap** (fatura bağı
   yok) + Bölmek + kesim günü + kısmi ödeme. 34 kare (`10`–`34`).
7d. ⏳ **Faz 4:** Goodbudget (kullanıcı indirir + hesap açar + elle bakar).
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
| 6 | Wallet by BudgetBakers | `com.droid4you.application.wallet` | Tamamlandı + **Faz 2** | 9 + **38 (`10`–`47`)** | Yazıldı + **10 Eyl Faz 2** | 1 Eyl K01–K08 + arayüz taraması. **Faz 2 (10 Eyl):** çekirdek doğrulama (net ₺22.950); kredi kartı = dönemsiz negatif bakiye; B1 tekrarlayan = Planned payments/Recurrent — geçmişe kurulamaz, her örnek bekleyen + Confirm/Postpone/Dismiss (realize'e en yakın), ilk onayda otomatik/onaylı plan-bazlı seçim; **B2 taksit özelliği YOK** (tek parça); Budget (kategori+hesap filtresi) + Goal + **Debt canlı** ("Record oluştur → bakiye değişir" opsiyonu, "Loan interests" gideri); fiş OCR yok; plan silme uyarısız. UI İngilizce. İşletme/şahsi `Desteklenmiyor` |
| 7 | Bluecoins | `com.rammigsoftware.bluecoins` | Tamamlandı + **Faz 3** | 10 + **34 (`10`–`34`)** | Yazıldı + **10 Eyl Faz 3** | 1 Eyl K00–K08 + arayüz taraması. **Faz 3 (10 Eyl):** bulut yok → tam yeniden koşum; kontrol ₺44.950 birebir. **B2 taksit** = oran + ay sayısı (2–24+Özel) + ilk-ödeme; ₺6.000→6×₺1.000, 1/6 anında + 5 hatırlatıcı. **B1 tekrarlayan** = geçmişe kurulur, **tanım hiçbir şey üretmez**, occurrence'lar Hatırlatıcılar'da bekler, Kaydet→materyalize. **Tekrarlayan/taksit/bağımsız-hatırlatıcı tüm rakiplerin BusinessFinance'e en yakını.** **Cari hesap = sıradan bakiye hesabı, fatura/tahsilat bağı YOK.** Kredi Kartı'nda ekstre kesim günü var. Emülatörde kararsız. İşletme/şahsi `Desteklenmiyor` |
| 8 | Hesap Defterim (Cash Book / Ankit Saraf) | `cashbook.cashbook` | **Aday — henüz koşulmadı** | 0 (emülatörde yüklü, 10 Eyl) | Yok | **10 Eyl eklendi (kullanıcı buldu).** Hint kökenli "khatabook" türü, Türkçe'ye yerelleşmiş dijital kasa defteri. 5M+ indirme, 4,7★/~127K yorum. Model: **Alındı/Ödendi → Denge** (tek yürüyen bakiye); çok defter; **veresiye/müşteri kredisi** takibi; fiş foto; PDF+Excel rapor; Google Drive yedek. **Kayıt yok** (direkt kullanılır). Banka bağlama yok. Tekrarlayan/bütçe: doğrulanmadı (bu türde genelde yok). Tur 2'nin 3. slotu için Goodbudget'a alternatif aday — Faz 4'te gezilecek |

Sonuç değerleri: `Başlanmadı` · `Sürüyor` · `Tamamlandı` · `Kısmi` · `Engelli`
· `Ücretli` · `Desteklenmiyor` · `Aday`.

**Not (10 Eyl):** Orijinal 7 uygulamaya ek olarak iki "farklı felsefe" adayı
emülatöre yüklendi — **Hesap Defterim** (`cashbook.cashbook`, esnaf kasa/veresiye
defteri) ve **Goodbudget** (`com.dayspringtech.envelopes`, dijital zarf bütçe).
Faz 4'te ikisi de elle gezilip Tur 2'nin 3. slotu için biri seçilecek.

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

**Yapı (10 Eyl 2026, `TUR2-YOL-HARITASI.md`):** Sürülebilir 3 uygulamada boşluk
koşumu bitti (Faz 1–3). Faz 4 = **iki aday elle gezinti** (Hesap Defterim +
Goodbudget). Faz 5 = Tur 2'nin 3 uygulaması seçilir.
Nihai yapı adayı: **KolayBi (masa başı) + Bluecoins (yapımıza en yakın — yapay zekâ
önerisi, kilitlenmeli) + {Hesap Defterim | Goodbudget}'ten 1 ("bize benzemeyen"
slot).** Faz 5'te kesinleşir.

**Boşluk koşumu standart kalemleri (her uygulamada canlı):** çekirdek 5 işlem
(Ağustos tarihli) + kredi kartı harcaması/ödemesi + **B1 tekrarlayan** + **B2
taksit** (`SENTETIK-TEST-VERISI.md`) + formun kendi eksik listesi.
**Kapsam dışı:** export, yedek/geri yükleme, bütçe kurulum ekranı.

| Uygulama | Rolü | Boşluk koşumu odağı |
|---|---|---|
| Money Manager | "Yapımıza en yakın" adayı — para modeli birebir | ✅ çekirdek + kart modeli + Ödeme + Tekrar/Taksit menüsü + açılış bakiyesi (10 Eyl). **Kalan:** B1/B2 canlı kurma |
| Wallet (BudgetBakers) | Finansal UX referansı — dashboard, rapor okunabilirliği; tekrarlayan "bekleyen→onayla" bize çok yakın | ✅ Faz 2 TAM (10 Eyl). Artı: BusinessFinance'e yakın tekrarlayan akış, Debt mini-defteri (Records + açık Total). Eksi: taksit yok, dönemsiz kart, İngilizce UI, işletme/şahsi yok, plan silme uyarısız |
| Bluecoins | En geniş para modeli; tekrarlayan/taksit "tanım üretmez, occurrence realize" bize birebir | ✅ Faz 3 TAM (10 Eyl). Artı: BusinessFinance'e en yakın tekrarlayan/taksit/planlanan modeli, gerçek taksit özelliği, ekstre kesim günü. Eksi: yoğun form, geniş hesap evreni, emülatörde kararsız, işletme/şahsi yok, **cari hesap yalnız kasa (fatura/tahsilat bağı yok)** |
| Hesap Defterim (aday) | "Esnaf kasa/veresiye defteri" — minimal, TR yerelleşmiş, tek yürüyen bakiye | Faz 4: kullanıcı ~15 dk elle gezer (kayıt yok) → 3. slot kıyası |
| Goodbudget (aday) | "Dijital zarf bütçe" — harcamadan önce dağıt, felsefe farkı | Faz 4: kullanıcı household hesabı açar + ~15 dk gezer → 3. slot kıyası |
| KolayBi | Türk ön muhasebe temsilcisi (masa başı) | Faz 6: Kullanım Rehberi videolarından proje ekranı, gider formu, cari ekstre |

- Ek olaylar: `SENTETIK-TEST-VERISI.md` → D1–D4 (Faz 7, seçilen 3 uygulamada)
- Paraşüt / Logo / QuickBooks: masa başı seviyesinde kalır, Belge 1–2'ye girer

## Üç belge

Taslak ve onay durumu `raporlar/README.md` içindeki tabloda tutulur.
