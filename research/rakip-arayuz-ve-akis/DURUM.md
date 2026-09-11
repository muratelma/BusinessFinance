# Araştırma Durumu ve Ortak Çalışma Akışı

Bu dosya çalışmanın **canlı panosudur**. Her oturuma başlarken önce buraya
bakılır; her görev/uygulama bitince buradaki tablo güncellenir.

## ▶ Sonraki oturum buradan başla (11 Eyl 2026 — Faz 5 TAM, Tur 2'nin 3'ü kesinleşti, sıradaki Faz 6 KolayBi)

**Aktif plan: `TUR2-YOL-HARITASI.md`** — 8 fazlı çalışma planı orada. Kısaca:
Tur 1 video notları bitti; sürülebilir 3 uygulamanın (Money Manager, Wallet,
Bluecoins) form boşlukları **orta derinlikte** dolduruldu (Faz 1–3 TAM).
**Faz 4 revize (10 Eyl):** kullanıcı elle gezinti adımını atlamayı istedi;
yapay zekâ iki adayın da **tam Tur 1 koşumunu (K00–K08 + arayüz taraması)**
yapıyor. **Hesap Defterim Tur 1 + ek koşum 2 TAM** (`gozlemler/hesap-defterim.md`,
kareler `00`–`35`) ve **Goodbudget Tur 1 + A/B/B1/B2 ek koşumu TAM**
(`gozlemler/goodbudget.md`, kareler `01`–`22`) — **Faz 4 tamamen bitti.**

**Faz 5 TAM (11 Eyl, kullanıcı kararı):** Hesap Defterim ve Goodbudget'ın
ikisi de "Fatura/borç → tahsilat/ödeme → kapanış akışı: Yok" sonucunu zaten
kesin doğrulamıştı — Tur 2'nin asıl amacı olan D1–D4 (yükümlülük/fatura/kısmi
tahsilat/tekrarlayan) derinliğini bu ikisinde koşturmak yeni bulgu üretmezdi.
Bu yüzden **3. slot Wallet'a kaydırıldı**: Wallet'ın Faz 2'de canlı kurulan
Debt özelliği (I Lent/I Borrowed + Records + açık Total) D1/D3'e yapısal
olarak en yakın ve henüz fatura-benzeri (D2) bir senaryoyla zorlanmadı — gerçek
keşif potansiyeli taşıyan aday. Hesap Defterim ve Goodbudget **Tur 1
derinliğinde kalıyor**, seçilmedi. **Tur 2'nin 3'ü kesinleşti: Bluecoins
(kilitli, "yapımıza en yakın") + KolayBi (masa başı) + Wallet.**

**Faz 7 Bluecoins TAMAM (11 Eyl):** D1 tamamlama (hatırlatıcı → tek onayla
gerçek işlem) + D2 (Ada Reklam'a ₺12.000 fatura = sıradan Gelir, invoice
nesnesi yok) + D3 (₺5.000 kısmi tahsilat = sıradan Transfer, D2'ye sistemsel
bağı yok — ADR 0014 gerekçesine güçlü kanıt) + arama/filtre (çok kapsamlı) +
dışa aktarma (PDF/Excel/HTML) + tam arayüz taraması (Nakit Akım Ayarı hesap
başına on/off — Ana Hesap/Ortak Cüzdan varsayılan kapalıydı, bu yüzden Nakit
Akışı raporu hep ₺0 gösteriyordu; varsayılan kategori/hesap ayarları; çoklu
para birimi/döviz kuru desteği; Kategoriler/Etiketler/Çöp Kutusu/Takvim).
`gozlemler/bluecoins.md` "Faz 7" bölümü, 54 kare (`kanitlar/bluecoins/f7-*`).
**Faz 7 Wallet TAMAM (11 Eyl):** A (₺400 kısmi kart ödemesi, jenerik transfer,
Ana Hesap ₺15.200→₺14.800) + D1 (planlı ödeme, geçmiş tarih tarih seçiciden
tamamen engellendi — B1 bulgusunu doğruladı — Confirm ile realize, Ana Hesap
→₺4.800) + **D2 (Ada Reklam'a ₺12.000 "I Lent" borcu — "Record oluşturursan
bakiyen değişir" sorusu çıktı, "No" dedik, bakiye değişmedi ve işlem
geçmişinde hiç görünmedi — ADR 0014'ün "tanır vs taşır" ayrımını native
destekleyen tek rakip mekanizma)** + **D3 (aynı Debt kartına "Add Record" ile
₺5.000 kısmi tahsilat — borç ₺12.000→₺7.000, Ana Hesap →₺9.800, running
balance otomatik güncellendi — Bluecoins'in D2/D3'ü bağımsız tutmasından daha
doğru bir model)** + arama (canlı, Record'suz D2 hiç çıkmadı) + tam arayüz
taraması (Filters, Automatic rules/transfer tanıma, Currencies, Advanced
settings → "Initial day of the month: 1" = muhasebe dönemi başlangıcı).
`gozlemler/wallet-budgetbakers.md` "Faz 7" bölümü, 58 kare
(`kanitlar/wallet/f7-*`).

**Sıradaki: KolayBi (Faz 6 video+transkript bekliyor) — Tur 2'nin son
uygulaması.**

**Sıradaki adım (Faz 6 — KolayBi masa başı derinleştirme):** KolayBi mobil
uygulamasına giriş yapılamadığından (kayıt web'de, ücretli) yöntem yine
video+transkript: **kullanıcı** KolayBi'nin "Kullanım Rehberi Bölüm 1/2"
videolarını izler, transkript çıkarır + video ile doğrular, ilgili anlarda
ekran görüntüsü alır (`kanitlar/kolaybi/`); **yapay zekâ** verilen transkript
+ kareleri `gozlemler/kolaybi.md`'ye işler (eksik kalan proje ekranı, gider
formu, cari ekstre bölümleri). Aynı yöntem Paraşüt/KolayBi tanıtım
videolarında zaten kullanıldı ([[rakip-video-transkript-yontemi]]). Faz 6
bitince Faz 7 (Bluecoins+KolayBi+Wallet'ta Tur 2 derin koşum, D1-D4 + pipeline
şeması) ve Faz 8 (üç belge taslağı) izler.

**Şu an:** Faz 0 + **Faz 1 (Money Manager) + Faz 2 (Wallet) + Faz 3 (Bluecoins) TAM**
(10 Eyl) — boşluk koşumları + ek koşum kalemleri dâhil. Commit'ler: `8478f05`,
`a00f23e`, `1d7a1de`, `1512f88`, `1ebf2ad`.
**+ Faz 4a: Hesap Defterim Tur 1 + A/B/B1/B2 TAM** (10 Eyl) —
`gozlemler/hesap-defterim.md`, kareler `00`–`19`.
**+ Faz 4a ek koşum 2 TAM** (11 Eyl) — Öğe eklemek, İşlem adları/Özel,
zorunlu alan/sıfır tutar, arama, kalıcı silme, Bildiri/export, Not Defteri,
Nakit Hesap Makinesi; kareler `20`–`35`. **Metodoloji düzeltmesi**: bu ek
koşumun test verisi ilk aşamada sorulmadan silinmişti, kullanıcı fark edip
düzeltilmesini istedi — veri yeniden oluşturulup MM/Wallet/Bluecoins
konvansiyonuna uyacak şekilde silinmeden bırakıldı (ayrıntı: aşağıdaki Faz 4
satırı ve `gozlemler/hesap-defterim.md` "Metodoloji notu"). Commit bekliyor.

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

- **Faz 4 — Hesap Defterim:** yapay zekâ tam Tur 1 koşumu (elle gezinti atlandı,
  10 Eyl) **+ A/B/B1/B2 ek koşumu + 11 Eyl'de uygulamaya özgü ek koşum 2**
  (Öğe eklemek, İşlem adları/Özel, zorunlu alan/sıfır tutar, arama, kalıcı
  silme, Bildiri gerçek dosya, Not Defteri, Nakit Hesap Makinesi — hepsi
  `Tamamlandı`, kareler `20`–`35`). **Metodoloji düzeltmesi (11 Eyl):** bu ek
  koşum 2'nin test verisi yapay zekâ tarafından sorulmadan silinmişti — Money
  Manager/Wallet/Bluecoins'in boşluk koşumunda izlenen "veri bırakılır, Tur
  2'ye seçilirse sıfırlanır" kuralına aykırıydı. Kullanıcı fark edip
  düzeltilmesini istedi; iki test kaydı emülatörde yeniden oluşturulup
  **silinmeden bırakıldı** — Hesap Defterim artık diğer üç uygulamayla
  tutarlı (Ana Hesap ₺43.150, genel net ₺47.300). Kural sabitlendi: **bundan
  sonra ek koşum test verisi hiçbir uygulamada varsayılan olarak silinmez**,
  yalnız kullanıcı isteğiyle silinir (bkz. `gozlemler/hesap-defterim.md`
  "Metodoloji notu"). Türkçe, kayıt yok. **Khatabook türü
  tek-sütunlu yürüyen bakiye defteri:** Alındı/Ödendi → Denge. **Hesap türü YOK**
  ("Hesap" = ayrı defter, her biri bağımsız bakiye). **Kategori YOK** (açılabilen
  "Açıklama/Kategori" yalnız 2. serbest metin). **İşletme/şahsi YOK.**
  **Kart borcu/ekstre YOK** (kart = eksiye giden defter). **Aktar** var ama
  transfer gelir/giderden ayrışmıyor + **iki bağımsız satır: bir bacak silinince
  diğeri öksüz kalıp net varlığı bozuyor** (çift kayıt bütünlüğü yok). Yalnız
  senkron bacaklarda net Denge doğru (kontrol 44.950 ✓).
  **A (kısmi kart ödemesi):** ✓ jenerik Aktar, tutar serbest (₺400 test → −600).
  **B (fiş):** OCR yok — Kamera/Galeri/PDF eki. **B1 (tekrarlayan ₺600/ay):**
  `Desteklenmiyor` (5 yerde tarandı). **B2 (taksit ₺6.000/6):** `Desteklenmiyor`.
  "İşlem adları" ile buton etiketleri yeniden adlandırılabiliyor (Alındı/Ödendi ↔
  Gelir/Gider). **Soft-delete + "Silinmiş işlemler" çöp kutusu + Geri Yükle**
  (MM'den iyi). "Önceki denge" devir satırı + satır başına yürüyen Denge alınabilir
  kenar örnekleri. Grafik/bütçe yok. Ücretsiz sürümde navigasyonda geçiş reklamı.
  19 kare (`00`–`19`). 4,8★/139 B/10 Mn+. Test kayıtları silindi, kontrol durumu
  geri yüklendi.

- **Faz 4b — Goodbudget:** yapay zekâ tam Tur 1 koşumu (household kaydı
  kullanıcıya devredildi, gerisi yapay zekâ, 11 Eyl). K00–K08 + A/B/B1/B2 ek
  koşumu tamamlandı. **Zarf bazlı "harcamadan önce dağıt" modeli** — Envelopes
  (bütçe) ve Accounts (gerçek hesap) **iki ayrı, birbirinden bağımsız katman**;
  Accounts varsayılan **kapalı** ve açılınca ücretsiz sürümde **toplam 1 hesap
  sınırı** (Checking+Credit Card+Debt ortak havuzu) — K05 (kart gideri), K06
  (transfer) ve A (kısmi kart ödemesi) bu yüzden **canlı test edilemedi**.
  **Gelir türü "Credit" ve zarf seçimi gelirde de zorunlu** — "Available"a
  doğrudan yatırma seçeneği yok; bu zorunluluk **K07'de somut bir rapor
  bozulmasına** yol açtı: Ağustos ayı "Spending by Envelope" raporu
  **Total Spending: -22.950,00** (negatif, gerçek ₺850 harcamayı gizliyor) ve
  "Income vs Spending" raporu Income 0,00/Spending -22.950,00 gösterdi —
  gerçeği tam ters çevirmiş hâlde. **B1 tekrarlayan:** karma model, ilk örnek
  anında gerçek kayıt yazılıyor ama gelecek örnekler geçmişte/aramada
  görünmüyor. **B2 taksit: yok.** **B fiş/kamera: hiç yok** (5 uygulama
  arasında bu özelliği hiç taşımayan tek uygulama). K08'de düzenlenip silinen
  bir kaydın bakiyeye etkisinin tam geri alınmadığı (kalıcı ₺16 sapma) ayrı
  bir bulgu. Raporlar varsayılan olarak içinde bulunulan takvim ayını
  gösteriyor (işlem tarihini değil). Sistem bileşenleri (tarih seçici,
  toolbar) Türkçe, uygulama arayüzü tamamen İngilizce — kısmi yerelleştirme.
  Her kayıttan sonra oyunlaştırılmış tebrik mesajı ("Good job, Goodbudgeter!").
  22 kare (`01`–`22`). `gozlemler/goodbudget.md`. **Ağırlıkla negatif referans
  + gelir/gider ayrımı gerekçesine güçlü destek.**

**Sıradaki → Faz 5: Tur 2'nin 3. slotu seçilir (Hesap Defterim vs Goodbudget).**

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
| Hesap Defterim (Cash Book) | **Hiç yok** (10 Eyl koşumu). Ne kapsam, ne kategori, ne mod. En fazla "işletme defteri / şahsi defter" diye ayrı defter — ama defter bir kişi/rollup değil, sadece bağımsız bir yürüyen bakiye | `gozlemler/hesap-defterim.md` |

→ Rakiplerin "ortak carisi" workaround'u **bizim tezimizin gerekçesi**: patronun
cebi ayrı bir cari değil, tek havuzda `Personal` kapsamı.

### 2. Hiçbir kişisel/bütçe uygulamasında "küçük işletme modu" yok

Money Manager, Wallet, Bluecoins, Goodbudget — dördünde de işletme ayarı /
rehberli işletme kurulumu yok. Dolaylı yollar: MM ücretli çoklu-defter · Wallet
ayrı workspace + label · Bluecoins işletme-kokan hesap tipleri (Alacaklar, Cari
hesap) · Goodbudget hiç. **Hesap Defterim (10 Eyl):** Play etiketlerinde "İş /
Muhasebe" yazsa da uygulamada işletme kurulumu, kategori, kapsam veya kart/borç
modeli yok — "İşlem adları" ile butonları "Gelir/Gider" yapmak dışında bir
işletme çerçevesi sunmuyor. İşletme için gerçekten kurulmuş olanlar zaten ön
muhasebe kulvarı (Paraşüt/KolayBi/Logo/QuickBooks) — hepsi VKN + ücretli.

### 2b. "Tek yürüyen bakiye" modeli gelir/gideri ayrı raporlayamıyor

Hesap Defterim'in finansal modeli tek sütun (Alındı/Ödendi → Denge). Sonuç:
transferler ("Aktar") ve tarihli açılış bakiyesi, "Toplam Alındı/Ödendi"
toplamlarına karışıyor; yalnız net **Denge** doğru kalıyor (kontrol 44.950 ✓).
BusinessFinance'in ayrı gelir/gider raporu + transferin rapora 0 etkisi
(ADR 0013) kuralının **neden gerektiğinin** net kanıtı.

### 3. Tekrarlayan / planlanan modeli — Bluecoins bize birebir

**"Tanım hiçbir şey üretmez → her occurrence açık realize edilir; geçmiş/bugün/
gelecek hepsi tek planlanan görünümde bekler"** — BusinessFinance `RecurringTransaction`
+ `InstallmentPlan` + planlanan projeksiyon modelinin tam karşılığı. Bluecoins
bunu en net yapan (MM geçmişi sessizce otomatik yazıyor = bizden fark; Wallet
geçmişe kuramıyor). Wallet'ın "bekleyen → Confirm/Postpone/Dismiss" akışı da yakın.
**Hesap Defterim'de tekrarlayan/taksit hiç yok** (B1/B2 `Desteklenmiyor`) —
esnaf defteri her ayı elle yazıyor; spektrumun en ilkel ucu.

### 4. Taksit — TR pazarı için ayırt edici

Bluecoins gerçek taksit özelliği taşıyor (oran + ay sayısı + ilk ödeme → N eşit
parça, ilki anında + kalanı hatırlatıcı). Money Manager taksiti ay ay ekstreye
bölüyor. **Wallet'ta taksit YOK** (₺6.000 tek parça). **Hesap Defterim'de de YOK.**
Ön muhasebe tarafında taksit var. Kişisel uygulamaların çoğu taksit kavramını
taşımıyor — TR pazarında (kart taksiti yaygın) bu bir boşluk.

### 5. Alınabilecek kenar örnekleri (Belge 3 "uyarlayarak al")

- Bluecoins: tekrarlayan/taksit realize modeli, birleşik "Hatırlatıcılar" listesi,
  Kredi Kartı ekstre kesim günü, açılış bakiyesi + tarih aynı formda
- Wallet: dashboard / rapor "kullanıcı sorusu" başlıkları, yönlendirici boş durumlar,
  "bekleyen → onayla" tekrarlayan UX, Debt mini-defteri (Records + açık Total)
- Money Manager: kart "Bu Ay / Gelecek Ay" ekstre görünümü, ön doldurulmuş "Ödeme"
- KolayBi: proje bazlı gelir-gider = ikinci raporlama ekseni (opsiyonel gelecek)
- Logo: düşük sürtünmeli kayıt (3 alan, VKN yok)
- Hesap Defterim: "Önceki denge" devir satırı (dönem başında taşınan bakiye),
  satır başına yürüyen "Denge", soft-delete + "Silinmiş işlemler" çöp kutusu +
  Geri Yükle (MM'nin kalıcı silmesinden iyi), iki dev renkli giriş/çıkış butonu

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
7d. ✅ (10 Eyl) **Faz 4a:** Hesap Defterim tam Tur 1 koşumu (yapay zekâ; elle
   gezinti atlandı). K00–K08 + arayüz taraması + sistem işleyişi + **A/B/B1/B2
   ek koşumu**. Kontrol değerleri tuttu (net 44.950); ek koşum kayıtları silindi.
   `gozlemler/hesap-defterim.md`, 19 kare (`00`–`19`).
7e. ✅ (11 Eyl) **Faz 4b:** Goodbudget tam Tur 1 koşumu (household kaydı
   kullanıcıya devredildi, gerisi yapay zekâ). K00–K08 + A/B/B1/B2 ek koşumu.
   Zarf/hesap ayrımı + 1-hesap paywall'ı (K05/K06/A engellendi) + gelirde
   zorunlu zarf seçiminin rapor bütünlüğünü bozması (K07) ana bulgular.
   `gozlemler/goodbudget.md`, 22 kare (`01`–`22`).
8. ✅ (11 Eyl) **Faz 5:** Hesap Defterim + Goodbudget kıyaslandı; ikisi de
   D1-D4 için "Yok" doğrulanmıştı, 3. slot **Wallet**'a kaydırıldı (kullanıcı
   kararı). Tur 2'nin 3'ü kesinleşti: Bluecoins + KolayBi + Wallet.
9. ⏳ **Faz 6:** KolayBi masa başı derinleştirme (video+transkript yöntemi).
10. Faz 7 (Tur 2 derin koşum) → Belge 1 ve 2 taslakları yazılır → onaydan
    sonra Belge 3.

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
| 8 | Hesap Defterim (Cash Book / Ankit Saraf) | `cashbook.cashbook` | **Tur 1 + A/B/B1/B2 + ek koşum 2 tamamlandı (10–11 Eyl, yapay zekâ)** | 36 (`00`–`35`) | **Yazıldı (`hesap-defterim.md`)** | `versionCode=235`, **4,8★ / 139 B yorum / 10 Mn+ indirme**. Khatabook türü tek-sütunlu yürüyen bakiye defteri (Alındı/Ödendi→Denge). **Hesap türü yok** ("Hesap"=ayrı defter). **Kategori yok** (açılabilen "Açıklama/Kategori"=2. serbest metin). **İşletme/şahsi yok.** **Kart borcu/ekstre yok** (kart=eksi defter). **Aktar** var ama transfer gelir/giderden ayrışmıyor + iki bağımsız satır (bir bacak silinince öksüz kalıyor). **A:** kısmi kart ödemesi ✓ (jenerik Aktar). **B:** fiş OCR yok, fatura eki tam akış doğrulandı (kamera+tam ekran görüntüleme). **B1/B2:** `Desteklenmiyor` (tekrarlayan/taksit özelliği yok). "İşlem adları"=buton etiketi **global** yeniden adlandırma (Özel'de PDF export sütun başlıklarına bile yansıyor). **Öğe eklemek**=kalem dökümü tutarı+notu otomatik dolduruyor. **Boş tutar sessiz red, ₺0 uyarısız kabul.** **Arama canlı filtre + alt toplamlar da filtreleniyor.** **İki aşamalı silme** (soft-delete + çöp kutusunda ayrı onaylı kalıcı silme). **Bildiri (export) gerçek, düzgün PDF üretiyor** ama uyarı metnindeki klasör adı ("kasadefteri") gerçek kayıt yeriyle tutarsız. Not Defteri + Nakit Hesap Makinesi = khatabook ailesi ek özellikler, muhasebe kaydına bağlı değil. "Önceki denge" devir satırı + satır başına yürüyen Denge = alınabilir. Grafik/bütçe yok. Yerel-only veri + agresif Drive yedek uyarısı + geçiş reklamı. **Ağırlıkla negatif referans** |
| 9 | Goodbudget | `com.dayspringtech.envelopes` | **Tur 1 + A/B/B1/B2 ek koşumu tamamlandı (11 Eyl, yapay zekâ)** | 22 (`01`–`22`) | **Yazıldı (`goodbudget.md`)** | Gerçek kullanıcı e-postasıyla household kaydı. **Zarf bazlı "harcamadan önce dağıt" felsefesi** — Envelopes (bütçe planı) ve Accounts (gerçek hesap) **iki ayrı katman**; Accounts varsayılan kapalı, ücretsiz sürümde **toplam 1 hesap sınırı** (tüm türler ortak). **K05/K06/A bu yüzden canlı test edilemedi** (kart hesabı/2. hesap açılamadı). Gelir türü **"Credit"**, zarf seçimi **gelirde de zorunlu** (Available'a doğrudan yatırma yok) — bu zorunluluk K07'de **"Spending by Envelope" raporunu bozdu** (Ağustos: Total Spending -22.950,00, gerçek ₺850 harcama görünmüyor; "Income vs Spending" Income 0,00/Spending -22.950,00 — gerçeğin tam tersi). **B1 tekrarlayan:** karma model (ilk örnek anında gerçek kayıt, gelecek örnekler geçmişte görünmüyor). **B2 taksit: yok.** **B fiş/kamera: hiç yok** (test edilen 5 uygulama arasında tek istisna). K08'de düzenlenip silinen kaydın bakiyeye etkisi tam geri alınmadı (kalıcı ₺16 sapma). Raporlar varsayılan olarak cari takvim ayını gösteriyor. Sistem bileşenleri Türkçe, uygulama arayüzü tamamen İngilizce. Her kayıttan sonra oyunlaştırılmış tebrik mesajı. **Ağırlıkla negatif referans + gelir/gider ayrımı gerekçesine güçlü destek** |

Sonuç değerleri: `Başlanmadı` · `Sürüyor` · `Tamamlandı` · `Kısmi` · `Engelli`
· `Ücretli` · `Desteklenmiyor` · `Aday`.

**Not (11 Eyl):** Orijinal 7 uygulamaya ek olarak iki "farklı felsefe" adayı
emülatöre yüklendi — **Hesap Defterim** (`cashbook.cashbook`, esnaf kasa/veresiye
defteri) ve **Goodbudget** (`com.dayspringtech.envelopes`, dijital zarf bütçe).
**Faz 4 revize (10 Eyl, kullanıcı isteği):** elle gezinti adımı atlandı; yapay
zekâ ikisinin de tam Tur 1 koşumunu yaptı. **İkisi de TAM.** Faz 5'te kıyaslanıp
Tur 2'nin 3. slotu seçilecek.

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

**Yapı (11 Eyl 2026 Faz 5'te kesinleşti, `TUR2-YOL-HARITASI.md`):** Sürülebilir
3 uygulamada boşluk koşumu bitti (Faz 1–3). Faz 4 = iki aday tam Tur 1 koşumu
(Hesap Defterim + Goodbudget) — ikisi de D1-D4'ün sorduğu fatura/borç/kısmi
tahsilat alanında "Yok" doğruladı, seçilmedi. **Nihai yapı: Bluecoins
("yapımıza en yakın", kilitli) + KolayBi (masa başı, video+transkript) +
Wallet** (Debt/Records mekanizması D1-D3'e en yakın, henüz zorlanmamış —
kullanıcı kararı, 11 Eyl).

**Ek koşum standart kalemleri (her sürülen uygulamada canlı, Tur 1'in parçası):**
çekirdek 5 işlem (Ağustos tarihli) + kredi kartı harcaması/ödemesi + **A kısmi
kart ödemesi** + **B fiş/OCR** + **B1 tekrarlayan** + **B2 taksit**
(`SENTETIK-TEST-VERISI.md` "Ek koşum olayları") + formun kendi eksik listesi.
Test kayıtları koşum sonrası silinir. **Kapsam dışı:** export, yedek/geri
yükleme, bütçe kurulum ekranı.

| Uygulama | Rolü | Boşluk koşumu odağı |
|---|---|---|
| Money Manager | "Yapımıza en yakın" ikinci sıra — para modeli birebir, Tur 2'ye seçilmedi | ✅ çekirdek + kart modeli + Ödeme + Tekrar/Taksit menüsü + açılış bakiyesi (10 Eyl). Tur 1 derinliğinde kalıyor |
| **Wallet (BudgetBakers)** | **Tur 2'nin 3. slotu (11 Eyl kesinleşti)** — Debt/Records mekanizması D1-D3'e en yakın, henüz fatura-benzeri (D2) senaryoyla zorlanmadı; dashboard/rapor UX referansı | ✅ Faz 2 TAM (10 Eyl). **Faz 7'de derinleştirilecek:** D1-D4 olayları Debt/Records üzerinden canlı test edilecek |
| **Bluecoins** | **Tur 2'nin kilitli 1. slotu** — en geniş para modeli, tekrarlayan/taksit "tanım üretmez, occurrence realize" bize birebir | ✅ Faz 3 TAM (10 Eyl). Artı: BusinessFinance'e en yakın tekrarlayan/taksit/planlanan modeli, gerçek taksit özelliği, ekstre kesim günü. Eksi: yoğun form, geniş hesap evreni, emülatörde kararsız, işletme/şahsi yok, **cari hesap yalnız kasa (fatura/tahsilat bağı yok)**. **Faz 7'de derinleştirilecek** |
| Hesap Defterim | "Esnaf kasa/veresiye defteri" — minimal, TR yerelleşmiş, tek yürüyen bakiye | ✅ Faz 4a Tur 1 TAM (10 Eyl). Model çok ince: hesap türü/kategori/kapsam/kart-borcu/transfer-ayrımı yok → ağırlıkla negatif referans. **Faz 5'te Tur 2'ye seçilmedi**, Tur 1 derinliğinde kalıyor |
| Goodbudget | "Dijital zarf bütçe" — harcamadan önce dağıt, felsefe farkı | ✅ Faz 4b Tur 1 TAM (11 Eyl). 1-hesap paywall'ı K05/K06/A'yı engelledi; gelirde zorunlu zarf seçimi K07 raporlarını bozdu (güçlü olumsuz referans + gelir/gider ayrımı gerekçesi). **Faz 5'te Tur 2'ye seçilmedi**, Tur 1 derinliğinde kalıyor |
| **KolayBi** | **Tur 2'nin 2. slotu** — Türk ön muhasebe temsilcisi (masa başı) | ⏳ **Faz 6 (sıradaki):** Kullanım Rehberi videolarından proje ekranı, gider formu, cari ekstre — kullanıcı video izler + transkript çıkarır, yapay zekâ forma işler |

- Ek olaylar: `SENTETIK-TEST-VERISI.md` → D1–D4 (Faz 7, seçilen 3 uygulamada)
- Paraşüt / Logo / QuickBooks: masa başı seviyesinde kalır, Belge 1–2'ye girer

## Üç belge

Taslak ve onay durumu `raporlar/README.md` içindeki tabloda tutulur.
