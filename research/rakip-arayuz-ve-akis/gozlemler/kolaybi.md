# Uygulama Gözlem Formu — KolayBi

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | KolayBi' / KolayBi Yazılım (Türk Ekonomi Bankası iştiraki) |
| Sürüm | 3.3.1 (React Native) |
| Test tarihi | 1 Eylül 2026 |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` |
| Dil / para birimi | Türkçe (TR/EN seçilebilir) |
| Erişim kısıtı | **Kayıt engeli** — mobilde kayıt yok; yalnız "Giriş Yap", "QR Kod İle Giriş Yap", "Şifremi Unuttum". Kayıt web'de (kolaybi.com), ücretli |
| İnceleme türü | **Resmî kaynak** (giriş ekranı manuel gözlem + "KolayBi' Nedir, Neden Kullanmalıyım?" tanıtım videosu — Dijital Köprü Akademi, ~4:19, kullanıcı 10 Eyl 2026 izledi + transkript çıkardı) |

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | "En kolay online ön muhasebe" — e-Fatura + gelir-gider + cari + stok + **proje bazlı gelir-gider** birleşik bulut programı |
| Asıl hedef kullanıcı | **Şahıs şirketi** (adıyla sayılıyor), KOBİ, start-up, çok müşterili muhasebeci. 40.000+ aktif işletme iddiası |
| Çözdüğü ana iş | e-Fatura/e-Arşiv + gelir-gider + cari (borç-alacak) + kasa-banka-çek-senet + stok + proje maliyet/kâr takibi |
| Açıkça kapsam dışı bıraktığı | Kişisel/gündelik bütçe, yatırım takibi |
| İş modeli | Ücretli abonelik, 14 gün deneme (web); e-fatura kontör + banka entegrasyonu kampanyası |
| BusinessFinance ile aynı kulvarda mı | **Kısmen.** Gelir-gider + cari + tekrarlayan işlem ortak; **proje bazlı gelir-gider** ikinci bir raporlama boyutu olarak bizim kapsam boyutumuzla kavramsal akraba (farklı eksen). Ama e-belge + firma defteri + hafif ERP; işletme/şahsi tek havuz kavramı yok |

## Görev gözlemleri

| Görev | Sonuç | Not |
|---|---|---|
| K00 İlk açılış | Kısmi | Emülatör açılışında "16 KB uyumlu değil" uyarısı (RN kütüphaneleri). Giriş ekranı: E-Posta + Şifre + Beni Hatırla + Giriş Yap + **QR Kod ile giriş** (masaüstünden hızlı eşleşme) + Şifremi Unuttum + TR/EN bayrak |
| K00 (kayıt denemesi, 2 Eyl) | Engelli | Kullanıcı doğruladı: mobil uygulamada **yalnız "Giriş Yap"** var, "Kayıt Ol / Hesap Oluştur" bağlantısı **hiç yok**. Kayıt sadece web'de (kolaybi.com) |
| K01–K08 | **Engelli** | Mobilde kayıt yok, hesap gerekiyor. Özellikler resmî kaynakla; **web arayüzü akışı tanıtım videosundan yeniden kuruldu** (`kanitlar/kolaybi/02`–`08`, bkz. aşağı) |

## Arayüz incelemesi (giriş öncesi)

| Başlık | Kısa gözlem |
|---|---|
| Giriş ekranı | Sade, açık gri zemin, mor (marka) birincil buton, bol beyaz alan; alanlar `*` ile zorunlu işaretli |
| Özgün öğe | "QR Kod İle Giriş Yap" — masaüstü oturumundan mobili hızlı bağlama |

## Resmî kaynak — özellikler ve kapsam (kolaybi.com, 2 Eyl 2026)

`Resmî kaynak` etiketli; emülatörde doğrulanmadı. Not: "kapsam dışı" ibaresi
bir **öneri kararıdır** (Belge 3), gözlem değil — her özellik Belge 1/2'de
anlatılır, patron kapsamı genişletebilir.

### Ne bu ürün, kimin için

- **Web + mobil bulut ön muhasebe programı.** "Yeni nesil" olarak konumlanıyor.
- Hedef kitle: **KOBİ, start-up ve şahıs şirketi** için ayrı paketler. Hizmet
  sektörü, üretim, toptan, perakende. Şahıs şirketini adıyla saymaları bizim
  segmentimize en yakın ifade.
- Sınırsız kullanıcı ekleme; destek ve kurulum ücretsiz.

### Özellikler

| Grup | Özellik | BusinessFinance açısından |
|---|---|---|
| Finans | Gelir-gider takibi, nakit akışı, cari hesap (borç-alacak) takibi, **tekrarlayan işlem yönetimi**, tahsilat/ödeme | Cari + tekrarlayan bizde var; tekrarlayan "işlem" demeleri bizim modelimize Logo/Paraşüt'ün "tekrarlayan fatura"sından daha yakın |
| Fatura/e-belge | e-Fatura, e-Arşiv, e-İrsaliye, e-İhracat, e-SMM, **e-İmza** | **Kapsam dışı** |
| Operasyon | Stok/envanter yönetimi, sipariş yönetimi, **proje bazlı gelir-gider takibi** | Stok/sipariş kapsam dışı. "Proje bazlı gelir-gider" ilginç — bizde kapsam (Business/Personal) boyutu var, proje boyutu yok |
| Entegrasyon | **25+ banka** hesap senkronizasyonu, e-ticaret pazaryeri, **sanal POS** (tahsilat) | Banka bağlantısı bizde **kesin kapsam dışı**; sanal POS ≠ bizim `PosSettlement` (o fiziksel POS gün sonu) |
| Fiş | Fiş OCR | Bizde öneri katmanı (ADR 0011) |
| Muhasebeci | Muhasebeci için çok kullanıcılı erişim | Paraşüt/Logo ile aynı canlı erişim modeli; bizimki dosya tabanlı dışa aktarma |
| Raporlama | Finansal raporlar ve panolar | — |

### Fiyat

- **Ücretli ürün.** Kayıt yalnız web'de (kolaybi.com); mobilde kayıt yok.
- PLUS paketiyle kısıtsız e-fatura kontör + 1 yıl e-imza + banka entegrasyonu
  kampanyası.
- Bu çalışmada uygulamaya girilemedi; inceleme tümüyle resmî kaynak.

### Karşılaştırma notu

Logo İşbaşı ve Paraşüt ile aynı kategoride: geniş kapsamlı ön muhasebe +
e-dönüşüm. **İşletme/şahsi tek havuz** kavramı yok. İki ayırt edici nokta:
(1) şahıs şirketini hedef olarak açıkça sayması, (2) **proje bazlı gelir-gider**
boyutu — bizim kapsam boyutumuzla kavramsal akraba ama farklı eksen.

## Sistem işleyişi / pipeline

`Resmî kaynak` — kolaybi.com + kullanıcı kılavuzu (9 Eyl 2026). Canlı ürün
davranışı doğrulanmadı.

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Modül yapısı | Satış (fatura, sipariş, proforma/teklif), Alış (fatura, irsaliye, **tekrarlayan işlem**), Genel Gider, Cari/Cariler (müşteri, personel, **tekrarlayan maaş**), Finans (banka, kasa, çek, senet), Proje, Raporlama | Resmî kaynak |
| Kayıt → entegre güncelleme | Modüller birbirine bağlı; fatura/gider kaydı cari + kasa-banka + stok + (varsa) proje defterine yansıyor. Backend kademe mekaniği kılavuzda ayrıntılı değil | Resmî kaynak + Doğrulanamadı |
| Gelir-gider | Muhasebe bilgisi gerektirmeden ödeme/tahsilat takibi + gelir-gider raporu + cari takibi. Mobilde nakit akışı, fatura, cari "hareket hâlinde" | Resmî kaynak |
| Cari hesap | Müşteri + tedarikçi; tüm tahsilat/ödeme/borç-alacak cari içinde; hesap bakiye raporu | Resmî kaynak |
| Tahsilat/ödeme | Bilgisayar başında beklemeden, gerçek zamanlı; sanal POS ile uzaktan tahsilat | Resmî kaynak |
| **Proje bazlı gelir-gider** | Tek ekranda proje bazında: alınan ödemeler, giderler, projede çalışan personel maliyeti listeleniyor → **proje kâr marjı** ve "en kârlı proje" görünüyor. İkinci bir gruplama/raporlama ekseni | Resmî kaynak |
| Tekrarlayan | Alışta "tekrarlayan işlem", caride "tekrarlayan maaş" — abonelik/periyodik kayıt otomatik üretimi | Resmî kaynak |
| İşletme/şahsi ayrım | **Yok.** Firma defteri; ayrım ekseni **proje**, işletme/şahsi değil | Resmî kaynak + Yorum |
| Entegrasyon temas noktaları | 25+ banka (KolayBi' Banka), GİB e-belge, pazaryeri (Trendyol/Hepsiburada), sanal POS, geliştirici API | Resmî kaynak |
| Muhasebeci tarafı | Çok müşterili muhasebeci erişimi (canlı) | Resmî kaynak |
| Veri nereye yazılıyor | Bulut, web + mobil senkron | Resmî kaynak |

**Pipeline şeması (proje bazlı):**
`kayıt oluştur (fatura/gider/tahsilat) → projeye etiketle → proje ekranında
alınan ödeme / gider / personel maliyeti tek listede → proje kâr marjı →
"en kârlı proje" raporu`

Bu, bizim `TransactionScope` (Business/Personal) boyutumuzla **aynı mimari fikir**:
bir kaydı ikinci bir eksende gruplayıp o eksende ayrı rapor almak. Eksen farklı
(proje ≠ işletme/şahsi) ve KolayBi'de bu bir kâr analizi aracı; bizde kapsam
parayı bölmeyen bir raporlama boyutu (ADR 0013).

## Video/doküman akış yeniden kurulumu

**Kaynak:** "KolayBi' Nedir, Neden Kullanmalıyım?" (Dijital Köprü Akademi, ~4:19).
Kullanıcı 10 Eyl 2026'da izledi, transkriptini bir araçla çıkarıp doğruladı,
kareleri aktardı. Etiket: `Resmî kaynak (tanıtım videosu)`. **Video ~5 yıl
öncesine ait** (Takvim widget'ı Mayıs 2020, "Dijital Köprü kampanyası ile
2022'ye kadar ücretsiz" ibaresi) — arayüz o günün web sürümü. Kullanıcı ayrıca
**2026 başında yüklenmiş güncel bir videodan tek kare** ekledi (`08`); bugünkü
arayüz farkı aşağıda. (Not: `08`'deki grafik "1.8.2024" tarihi videodaki demo
verisidir, videonun tarihi değil.)

Transkript tam metni bu formun sonundaki **"Ek — video transkripti"**
bölümündedir; alıntılar `Resmî kaynak (video transkript)` etiketlidir.

### Videonun anlattığı (transkript özeti)

- **Ön muhasebe tanımı** (00:08): "işletmelerin parayla ifade edilen işlemleri —
  nakit, banka hesapları, çekler, müşteriler, stoklar, senetler". KolayBi bunu
  KOBİ için bulutta toplayan program.
- **Bulut gerekçesi** (00:40–01:35): kurulum yok, bakım/onarım maliyeti yok, her
  cihazdan erişim, **otomatik yedekleme** (fiziksel kayıp riski yok).
- **Özellik listesi** (01:35): cari hesap takibi, gelir-gider takibi, ön muhasebe
  raporlama, fatura takibi, stok takibi, **proje gelir-gider takibi**. Ek
  çözümler: online banka entegrasyonu, e-Fatura/e-Arşiv, sanal POS, pazaryeri.
- **Dashboard okuması** (02:01): "üstteki grafikte dilediğiniz dönem aralığı için
  **nakit akışı**; aşağısında **vadesi gelmemiş, vadesi geçmiş veya vadesi
  belirsiz** ödeme ve tahsilatlar".
- **Kurulum sırası** (02:31–03:00): (1) sol menü **Cari Hesaplar** → carileri
  manuel tek tek **veya toplu içeri aktarma**, (2) **Ürün ve Hizmetler** →
  ürün/hizmet kayıtları, (3) **Finans** → banka hesabı ve kasalar.
- **Ek entegrasyonlar** (03:00): online banka / sanal POS / pazaryeri başvurusu
  **Ek Özellikler** sekmesinden → destek ekibi yönlendirmesiyle aktif.
- **e-Fatura** (03:32): QNB Finansbank özel entegratörü e-Finans üzerinden;
  "tek uygulamada faturalama + cari + stok tek ekrandan".

### Ekran seviyesi akış (video kareleri — 2020 web sürümü)

| # | Kare | Ekranda görünen | BusinessFinance açısından |
|---|---|---|---|
| 02 | `02-video-guncel-durum-panosu.png` | **Güncel Durum** panosu: "Son 4 Haftalık Nakit Akışı" çizgi grafiği + Takvim (Mayıs 2020) + "Tahsilat Ve Ödeme Özetleri" (Tahsilatlar/Ödemeler donut'ları) + sağda "Günü Gelen İşlemler". Sekmeler: Güncel Durum / Stok Akışı / Notlar. Sol menü 13 modül; üst sağ "Hızlı İşlemler" | Bizim **Özet ekranı**nın karşılığı. Nakit akışı grafiği + yaklaşan/geciken kalemler tek ekranda — bizim özet + upcoming-payments birleşimi. "Hızlı İşlemler" = bizim "İşlem ekle" launcher |
| 03 | `03-video-gunu-gelen-islemler.png` | "Günü Gelen İşlemler": **Bugün / Yaklaşanlar / Tarihi Geçenler** sekmeleri; Faturalar / Genel Giderler / Çalışan Maaşları satırları, yeşil-kırmızı sayaç, "$118,00 — 71 Gün Gecikti" | Bizim `readiness` / `attentionCode` ayrımıyla birebir. Kalem türü kırılımı (fatura / gider / maaş) = bizim `sourceGroup` |
| 04 | `04-video-vadesi-belirsiz-tahsilatlar.png` | "Vadesi Belirsiz Tahsilatlar" tam liste: Cari Bilgisi / Seri No / Düzenlenme Tarihi / Fatura Tutarı / **Yerel Tutar** / Bakiye / **Durum** (Ödenmedi / **Kısmen Ödendi**). Dışarıya Aktar + filtre + Ara | **Kısmi ödeme** ekranda görünür (Fatura 118.000 / Bakiye 116.741 → "Kısmen Ödendi") — bizim `CounterpartyPayment` kısmi tahsilatıyla aynı. "Vadesi belirsiz" = vade girilmemiş kayıt, bizde ayrı bir hal yok. "Yerel Tutar" = çoklu döviz; bizde tek para birimi |
| 05 | `05-video-cari-hesaplar.png` | "Genel Cari Hesapları": sekmeler **Genel Cariler / Potansiyel Müşteriler / Personel Carileri / Ortaklar**. Kolonlar: Kod / Unvan / Cari Tipi / Etiketler / VKN-TCKN / **Yerel Bakiye**. Cari Oluştur / İçe Aktar / Dışarıya Aktar | **"Personel Carileri" ve "Ortaklar" ayrı sekme** — patron/ortak cebinden para buradan, cari borç-alacak olarak giriyor. Paraşüt'le **birebir aynı** workaround; işletme/şahsi tek havuz olmadığı için ortak carisi kullanılıyor. ADR 0013 farkımızın 2. somut kanıtı. Toplu içe aktarma = transkriptteki iddia doğrulandı |
| 06 | `06-video-urun-ve-hizmetler.png` | "Ürün Ve Hizmetler": sekmeler Tümü / Ürünler / Hizmetler. Kolonlar: Tür / Kod / Ad / Etiketler / Alış Fiyatı / Satış Fiyatı / **KDV** / **İndirim**. (URL: `noffix.com` — altyapı) | Ürün kaydında **KDV alanı** → fatura KDV'yi buradan hesaplıyor; ADR 0016 farkımız (biz KDV taşırız, hesaplamayız). Ürün/hizmet kataloğu bizde yok (kayıt adı serbest metin) |
| 07 | `07-video-finans-kasalar.png` | "Kasalar" (Finans): sekmeler **Banka Hesapları / Kasalar / Online Banka Hesapları / Çekler**. Kolonlar: Adı / Etiketler / **Açılış Tarihi** / Para Birimi / Bakiye. "Ana Kasa", açılış 03.03.2020 | Kasa = elle, **"Online Banka Hesapları" ayrı sekme** = entegrasyonla çekilen. "Açılış Tarihi + Bakiye" = bizim `Account.OpeningBalance` modeli. Çek ayrı defter (bizde yok) |
| 08 | `08-guncel-arayuz-2026.png` | **Bugünkü sürüm** (2026 başı videodan): marka "KolayBi **Ofis**"; üstte "Size Özel Ayrıcalıklar" reklam şeridi (TotalEnergies, Oyak Sigorta, KolayBi Banka); menüde yeni **"Fatura Ödeme"**; sağda "KolayBi Yolu Var!" (Davet Et & Kazan, **iWallet**, Müşavirini Davet Et); "Mobil Uygulamaya Giriş Yap" | ~2020'den bugüne: pano **daha pazarlama-yoğun** (çapraz satış şeritleri), cüzdan (iWallet) ve fatura ödeme eklenmiş → banka/ödeme yönünde genişleme. Nakit akışı + Günü Gelen İşlemler iskeleti aynı kalmış |

### Ekrandan ekrana pipeline (videodan)

`kurulum: Cari Hesaplar (manuel/toplu) + Ürün-Hizmetler + Finans (kasa/banka) →
Hızlı İşlemler ile fatura/gider/tahsilat kaydı → projeye etiketle (ops.) →
Güncel Durum panosu: nakit akışı grafiği + Tahsilat/Ödeme Özetleri (vadesi
gelmemiş/geçmiş/belirsiz) + Günü Gelen İşlemler → Raporlar / proje kâr marjı →
ay sonu müşavire`

### Karşılaştırma notu

Video, metin kaynaklı okumayı doğruluyor ve iki noktayı keskinleştiriyor:

- **İşletme/şahsi tek havuz yok — "Ortaklar / Personel Carileri" workaround'u
  ekranda görünür.** Patronun cebinden para bir ortak carisi borç-alacağı
  oluyor; Paraşüt'te de aynı. Bu, ADR 0013 farkımızın en net iki kanıtından
  biri (diğeri Paraşüt).
- **"Vadesi gelmemiş / geçmiş / belirsiz" üçlü vade hali** bizim
  `readiness`/`attentionCode` ayrımıyla neredeyse aynı; "belirsiz" (vade
  girilmemiş) bizde ayrı bir hal değil — düşünülebilir.
- KDV ürün kaydından hesaplanıyor (ADR 0016 farkı), kısmi ödeme ekranda
  "Kısmen Ödendi" olarak var (ADR 0014 ile uyumlu).
- Proje bazlı gelir-gider bu videoda **ekran olarak gösterilmedi** (menüde
  "Projeler" var, transkriptte adı geçiyor). Proje ekranı Tur 2 kaynağı.

## BusinessFinance için kararlar

Bu tablo Belge 3'ün ön değerlendirmesidir; kesin karar Belge 3'te. "Alma" bir
öneri kararıdır, gözlem etiketi değil — özellik Belge 1/2'de yine anlatılır.

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| QR kod ile hızlı giriş | Henüz karar verme | İlginç ama bizim tek-cihaz/mobil-öncelikli senaryoda önceliği düşük | Auth |
| Proje bazlı gelir-gider takibi (ikinci bir raporlama boyutu) | Henüz karar verme | Bizim kapsam boyutuyla aynı fikrin farklı ekseni; ürün kapsamımızda proje yok ama "birden çok raporlama boyutu" talebinin kanıtı | Rapor / kapsam |
| "Ortaklar / Personel Carileri" ile patron parası (video kare 05) | Alma | Bizde bu tam olarak ADR 0013'ün çözdüğü sorun — patronun cebi ayrı cari değil, tek havuzda `Personal` kapsamı. Rakibin workaround'u bizim tezimizin gerekçesi | Kapsam / cari |
| Üçlü vade hali: vadesi gelmemiş / geçmiş / **belirsiz** (kare 03–04) | Henüz karar verme | "Belirsiz" (vade girilmemiş) bizde ayrı hal değil; upcoming-payments'a eklenebilir mi | upcoming-payments / attentionCode |
| "Günü Gelen İşlemler" panosu (Bugün/Yaklaşanlar/Tarihi Geçenler + tür kırılımı) | Uyarlayarak al | Bizim planlanan feed + özet ekranıyla örtüşüyor; tür kırılımı (`sourceGroup`) zaten var | Özet / planlanan feed |
| Ürün/hizmet kataloğu + KDV alanı | Alma | Kayıt adı bizde serbest metin; KDV taşınır, hesaplanmaz (ADR 0016) | — |
| Mobilde kayıt yok, yalnız giriş + QR | Alma | Bizde mobil ana istemci; kayıt mobilde olmalı | Onboarding |
| Canlı çok kullanıcılı muhasebeci erişimi | Alma (biçim) | Sahiplik izolasyonu + tek hesaplama yolu kuralı; bizde dosya tabanlı dışa aktarma paketi | Muhasebeci paketi |

## Kanıt ve güven düzeyi

- Manuel gözlem: Yalnız giriş ekranı (`kanitlar/kolaybi/01-giris-ekrani.png`).
  Kullanıcı doğruladı: mobilde kayıt yok
- Resmî kaynak (tanıtım videosu): `kanitlar/kolaybi/02-video-guncel-durum-panosu.png`
  … `07-video-finans-kasalar.png` — "KolayBi' Nedir, Neden Kullanmalıyım?"
  (Dijital Köprü Akademi, YouTube `nn4-hShn_kw`, ~4:19, ~2020 web sürümü).
  Transkript kullanıcı tarafından araçla çıkarıldı ve doğrulandı (10 Eyl 2026)
- Resmî kaynak (güncel): `08-guncel-arayuz-2026.png` — 2026 başında yüklenmiş
  başka bir videodan tek kare; bugünkü "KolayBi Ofis" arayüzü
- Resmî kaynak: kolaybi.com ana sayfa + özellik listesi (2 Eyl 2026); kullanıcı
  kılavuzu — modül yapısı, proje bazlı gelir-gider, tekrarlayan işlem (9 Eyl 2026)
- Doğrulanamadı: Canlı ürün davranışı, backend kademe mekaniği, **proje bazlı
  gelir-gider ekranı** (videoda gösterilmedi), güncel mobil arayüz
- Kullanıcıdan bekleniyor: (opsiyonel, Tur 2) "Kullanım Rehberi Bölüm 1/2"
  videolarından proje ekranı, gider formu, cari ekstre kareleri

## Tek cümlelik sonuç

KolayBi, şahıs şirketini de hedefleyen geniş kapsamlı bir bulut ön muhasebe +
e-dönüşüm ürünü; proje bazlı gelir-gider boyutu ve tekrarlayan işlem modeli bize
kavramsal kıyas olur, ama işletme/şahsi tek havuz kavramı olmadığı için yine
"klasik firma defteri" ailesinde.

## Ek — video transkripti

`Resmî kaynak (video transkript)`. "KolayBi' Nedir, Neden Kullanmalıyım?" —
Dijital Köprü Akademi, YouTube `nn4-hShn_kw`, ~4:19, ~2020. Kullanıcı araçla
çıkardı ve videoyla karşılaştırıp doğru olduğunu teyit etti (10 Eyl 2026).
Zaman damgaları kabaca yerleştirilmiştir.

> (00:08) İşletmelerin ücretle ifade edilen işlemlerini, yani işletmelerin nakit
> parası, banka hesapları, çekleri, müşterileri, stokları, senetleri gibi temel
> unsurların kayıtlarını ifade eder. Firmalar bu temel unsurların hareketlerini
> takip etmek için bir ön muhasebe programı kullanmaya ihtiyaç duyarlar. KolayBi,
> özellikle küçük ve orta büyüklükteki işletmelerin dijital dönüşümlerini
> hızlandırmayı hedefleyen ve bu doğrultuda özellikler, çözümler geliştiren bulut
> tabanlı bir ön muhasebe programı.
>
> (00:40) E-ticaret, teknoloji, hizmet, ticaret, üretim gibi farklı sektörlerden
> firmaların kullanımına uygun bir ön muhasebe programı ve farklı sektörlerin
> farklı ihtiyaçlarına özel çözümler ve özellikler geliştirmeye özellikle dikkat
> ediyor. Bulut tabanlı programlar bir masaüstü kurulum gerektirmezler. Bu sebeple
> bakım, onarım gibi maliyetlerden de aslında firmaları kurtarmış olurlar.
>
> (01:07) Aynı zamanda internet olan her yerden bilgisayar, tablet veya akıllı
> telefonlarla dilediğiniz bilgiye her anda ulaşabilmenin kolaylığını yaşarsınız.
> Aynı zamanda bulut tabanlı bir ön muhasebe programı kullandığınızda tüm
> verileriniz her an yedeklenir. Bu sayede fiziksel ortamda verilerinizi
> saklamanın yarattığı o verilerin kaybolması, zarar görmesi risklerini ortadan
> kaldırmış olursunuz.
>
> (01:35) KolayBi ile cari hesap takibi, gelir gider takibi, ön muhasebe
> raporlama, fatura takibi, stok takibi ve proje gelir gider takibi gibi
> özelliklerden yararlanılır. Temel ön muhasebe özelliklerinin yanında online
> banka entegrasyonu, e-fatura e-arşiv uygulaması, sanal pos ve pazar yeri
> entegrasyonu ek çözümleriyle tüm bu özelliklerin daha gelişmiş versiyonu
> kullanılır.
>
> (02:01) Bu sayede söz konusu firma için nakit akışı yönetiminin daha sağlıklı,
> daha verimli ve daha hızlı yapılabildiği bir senaryo çizilmiş olur. Bu ekranda
> üstteki grafikte dilediğiniz dönem aralığı için nakit akışınızı
> inceleyebilirsiniz. Bu grafiğin aşağısında ise vadesi gelmemiş, vadesi geçmiş
> veya vadesi belirsiz ödeme ve tahsilatlarınızı görebilirsiniz.
>
> (02:31) KolayBi kullanmaya başlamak için internete erişebildiğiniz herhangi bir
> cihazın yeterli olduğunu lütfen unutmayın. Uygulamayı kullanmaya cari
> hesaplarınızı kaydederek hemen başlayabilirsiniz. Bunun için soldaki menüde
> cari hesaplar sekmesi aracılığıyla carilerinizi manuel olarak tek tek
> kaydedebileceğiniz gibi, çok sayıda hesabın kaydını gerçekleştirmek için ise
> toplu içeri aktarma özelliğini kullanabilirsiniz.
>
> (03:00) Aynı şekilde ürün hizmet kayıtlarınızı ürün ve hizmetler sekmesinden,
> şirket banka hesabı ve kasalarınızı ise finans sekmesi üzerinden
> tanımlayabilirsiniz. Online banka entegrasyonu, sanal pos, pazar yeri
> entegrasyonu gibi çözümleri, başvurularınızı ek özellikler sekmesi üzerinden
> oluşturduktan sonra destek ekibimizin yönlendirmeleri sonucunda
> kullanabileceksiniz.
>
> (03:32) Dijital Köprü kampanyası ile 2022'ye kadar KolayBi ücretsiz
> kullanabileceğiniz gibi, QNB Finansbank e-Fatura özel entegratörü e-Finance ile
> e-Fatura ve e-Arşiv uygulamalarını da KolayBi ön muhasebe programı üzerinden
> kullanabilirsiniz. Bu sayede tüm faturalama süreçleriniz için tek bir uygulama
> kullanırken, kestiğiniz faturalarla ilişkili olarak cari hesap ve stok
> takibinizi de tek ekrandan yönetmiş olursunuz.
>
> (04:04) KolayBi ile şirketinizin nakit akışı yönetimi sizin elinizde olsun.
> Kullanımı kolay arayüzü ile işlemlerinizi hızlandırın, vaktinizi işinizi
> büyütmeye ayırın.

**Yöntem notu:** Tanıtım/eğitim videolarında bundan sonra bu yol kullanılacak —
kullanıcı transkripti araçla çıkarır ve doğrular, yapay zekâ transkripti +
kareleri eşleyerek forma işler. YouTube kare/transkript çekimi yapay zekâ
tarafında hâlâ çalışmıyor.
