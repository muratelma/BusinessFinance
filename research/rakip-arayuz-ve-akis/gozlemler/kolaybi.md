# Uygulama Gözlem Formu — KolayBi

## P1-kolaybi-G01 — 14 Eylül 2026 inceleme notu

E0180–E0197 arasındaki 18 görsel açılıp metinle karşılaştırıldı. Gözlem
sonuçları KANIT-ENVANTERI.md içinde görsel başına kayıtlıdır. Bu paket yeni
canlı veya resmî web testi değildir; mevcut video/destek karelerini inceler.
G02'nin 21 karesi henüz bu denetimde açılmadı. Aşağıdaki eski Faz 7.5 tamamlanma
beyanları tarihsel kayıttır, yeni G02 incelemesinin yerine geçmez.

Doğrudan düzeltilebilen metinler düzeltildi: demo adları kullanıcı talebi
sayılmaz; farklı projelerdeki Net ve Kar/Zarar aynı sayı diye sunulmaz;
silme simgesinin yanında Pasif bulunması silme yasağı değildir; PDF önizlemesi
e-posta teslim kanıtı değildir. d09 bitiş tarihi 26.09.2023, d10 çıktı aralığı
25.09.2023 ile biter: iki kare aynı tamamlanmış işlem zinciri sayılmaz.
Kare d35'teki Kısmen Ödendi demo satırı gerçek veri doğrulaması, Ortaklar
sekmesi patron parası akışı, iki pano karesi altı yıllık değişmezlik kanıtı
olarak yazılmaz. Düzeltme listesi E0392 P1-kolaybi-G01 bölümündedir.
Kalan çapraz yayılım B08/B15 ve G02 kayıtlarında izlenir; ek emülatör işi yok.

## P1-kolaybi-G02 — 14 Eylül 2026 inceleme notu

E0198–E0218 arasındaki 21 görsel açılıp metinle karşılaştırıldı; sonuçlar
KANIT-ENVANTERI.md içinde. KolayBi'nin 39 görselinin tamamı bu denetimde
incelendi. Düzeltmeler `G02 düzeltmesi` notlarıyla işaretli; liste E0392
P1-kolaybi-G02 bölümünde.

**Kare adları (14 Eyl, kullanıcı isteği):** video ve mobil giriş kareleri (eski
01–08) d32–d39 olarak destek serisinin sonuna eklendi; KolayBi'nin 39 karesi tek
`d` serisinde. Bu formdaki tam adlar ve kısa kodlar yeni adlara çevrildi; eşleme
KANIT-ENVANTERI.md P1-kolaybi-G02 bölümünde.

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | KolayBi' / KolayBi Yazılım. **QNB Finansbank ekosistemi** — QNBEYOND Ventures yatırımı, QNB "Dijital Köprü" programında sunuluyor. Eski adı **NKolayOfis** (2019'da KolayBi' oldu; karelerdeki `noffix.com` altyapı URL'si bu addan kalma) |
| Sürüm | 3.3.1 (React Native) |
| Test tarihi | **1 Eyl 2026** (emülatörde giriş ekranı) · **2 ve 9 Eyl 2026** (kolaybi.com) · **10 Eyl 2026** (tanıtım videosu + transkript) · **12 Eyl 2026** (destek merkezi taraması + Faz 7.5 doğrulama turu) |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` |
| Dil / para birimi | Türkçe (TR/EN seçilebilir) |
| Erişim kısıtı | **Kayıt engeli** — mobilde kayıt yok; yalnız "Giriş Yap", "QR Kod İle Giriş Yap", "Şifremi Unuttum". Kayıt web'de (kolaybi.com), ücretli |
| İnceleme türü | **Resmî kaynak** (giriş ekranı manuel gözlem + "KolayBi' Nedir, Neden Kullanmalıyım?" tanıtım videosu — Dijital Köprü Akademi, ~4:19, kullanıcı 10 Eyl 2026 izledi + transkript çıkardı) **+ 2026 başında yüklendiği aktarılan ikinci bir videodan tek kare** (`d39-guncel-arayuz-2026.png`, transkriptsiz; yükleme tarihi sürüm tarihi değil) |

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
| K00 İlk açılış | Kısmi | Emülatör açılışında "16 KB uyumlu değil" uyarısı (RN kütüphaneleri). Giriş ekranı: E-Posta + Şifre + Beni Hatırla + Giriş Yap + **QR Kod ile giriş** (düğme görüldü; eşleşme davranışı denenmedi) + Şifremi Unuttum + TR/EN bayrak |
| K00 (kayıt denemesi, 2 Eyl) | Engelli | Kullanıcı doğruladı: mobil uygulamada **yalnız "Giriş Yap"** var, "Kayıt Ol / Hesap Oluştur" bağlantısı **hiç yok**. Kayıt sadece web'de (kolaybi.com) |
| K01–K08 | **Engelli** | Mobilde kayıt yok, hesap gerekiyor. Özellikler resmî kaynakla; **web arayüzü akışı tanıtım videosundan yeniden kuruldu** (`kanitlar/kolaybi/d33`–`d39-guncel-arayuz-2026.png`, bkz. aşağı) |

## Arayüz incelemesi (giriş öncesi)

| Başlık | Kısa gözlem |
|---|---|
| Giriş ekranı | Sade, açık gri-mavi zemin, **indigo/mavi** (marka) birincil buton, bol beyaz alan; "Hoş Geldiniz!" başlığı; alanlar `*` ile zorunlu işaretli; şifre alanında göz (göster/gizle) düğmesi |
| Özgün öğe | "QR Kod İle Giriş Yap" düğmesi — masaüstü eşleşmesi önceki yorumdur, bu karede denenmedi |

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
| İşletme/şahsi ayrım | **Yok.** Firma defteri; ayrım ekseni **proje**, işletme/şahsi değil. `Ortaklar` / `Personel Carileri` cari sekmeleri var; patron parasının bu sekmelerden girdiği çıkarımdır *(P1-B08)* | Resmî kaynak + `Çıkarım` (sekmenin varlığı gözlem, kullanım biçimi çıkarım) |
| Entegrasyon temas noktaları | 25+ banka (KolayBi' Banka), GİB e-belge, pazaryeri (Trendyol/Hepsiburada), sanal POS, geliştirici API | Resmî kaynak |
| Muhasebeci tarafı | Çok müşterili muhasebeci erişimi (canlı) | Resmî kaynak |
| Veri nereye yazılıyor | Bulut, web + mobil senkron | Resmî kaynak |

**Pipeline şeması (proje bazlı):**
`kayıt oluştur (fatura/gider/tahsilat) → projeye etiketle → proje ekranında
alınan ödeme / gider / personel maliyeti tek listede → proje kâr marjı →
"en kârlı proje" raporu`

Bu, bizim `TransactionScope` (Business/Personal) boyutumuzla **benzer bir raporlama yaklaşımı**; depolama veya mimari eşdeğerlik kanıtlanmış değildir:
bir kaydı ikinci bir eksende gruplayıp o eksende ayrı rapor almak. Eksen farklı
(proje ≠ işletme/şahsi) ve KolayBi'de bu bir kâr analizi aracı; bizde kapsam
parayı bölmeyen bir raporlama boyutu (ADR 0013).

## Video/doküman akış yeniden kurulumu

**Kaynak:** "KolayBi' Nedir, Neden Kullanmalıyım?" (Dijital Köprü Akademi, ~4:19).
Kullanıcı 10 Eyl 2026'da izledi, transkriptini bir araçla çıkarıp doğruladı,
kareleri aktardı. Etiket: `Resmî kaynak (tanıtım videosu)`. **Video kareleri (d33–d38) ~2020 içerik işaretleri taşıyor** (Takvim widget'ı Mayıs 2020, "Dijital Köprü kampanyası ile
2022'ye kadar ücretsiz" ibaresi); arayüzün o dönemin web sürümü olduğu çıkarımdır,
videonun yayın tarihi kaynakta yok. `d32-giris-ekrani.png` bu videodan değil, emülatördeki mobil
giriş ekranıdır. Kullanıcı ayrıca **2026 başında yüklendiğini aktardığı ayrı bir
videodan tek kare** ekledi (`d39-guncel-arayuz-2026.png`); yükleme tarihi arayüzün üretim tarihini
kanıtlamaz *(P1-B15)*. Daha yeni arayüz farkı aşağıda. (Not: `d39-guncel-arayuz-2026.png`'deki grafik "1.8.2024" tarihi videodaki demo
verisidir, videonun tarihi değil.)

Transkript tam metni bu formun sonundaki **"Ek — video transkripti"**
bölümündedir; alıntılar `Resmî kaynak (video transkript)` etiketlidir.

**Kanıt gücü uyarısı — kareler bir demo/eğitim hesabından:** kare d37'nin alt
barındaki URL `test-ofis.noffix.com`, kare d39'un üst başlığı "KolayBi' Eğitim".
Ürün-hizmet tablosu tamamen boş ("Tabloda herhangi bir veri mevcut değil"),
cari listesinde tek kayıt (`CAR000001 KolayBi A.Ş`, ₺0,00), kasa tek kayıt
(₺0,00), 2026 karesinde nakit akışı tamamen ₺0. Yani bu karelerden **alan ve
sekme adları** güvenle okunur; **davranış** (hangi alan neyi tetikliyor)
okunamaz. Kare d35 dolu bir demo listesi gösterir; işlem öncesi/sonrası kanıtı değildir. Aşağıdaki tabloda
davranışa dair her satır `çıkarım` olarak işaretlidir.

**Destek mockup'ları için ek not (12 Eyl):** video karelerinin aksine
`d01-destek-proje-listesi.png` – `d31-destek-urun-varyantlar.png` karelerinde **önceki incelemede bazı sayılar aritmetik olarak karşılaştırılmıştır; bütün kareler için tutarlılık garantisi verilmez**. Üç
bağımsız kontrol: `d27-destek-kdv-raporu.png` KDV matrisi (₺12.000 matrah × %20 = ₺2.400 ✓),
`d26-destek-alis-satis-raporu.png` Alış/Satış raporu (₺20.000 − ₺12.000 = ₺8.000 ✓), `d29-destek-nakit-akis-raporu.png` nakit akışı
(₺19.543,53 + ₺143.252,50 = ₺162.796,03 ✓). Ayrıca `d19-destek-banka-hesaplari.png`'daki banka toplamı
(₺19.543,53) `d29-destek-nakit-akis-raporu.png`'un `Güncel Bakiye` kartıyla birebir aynı — kareler **bazı değerleri eşleşen örneklerdir; aynı
demo kiracı veya aynı an oldukları kanıtlanmadı**. Kontrol edilen bu sayılar, Paraşüt'ün animasyonlu tanıtım karelerinden
(dört karede aritmetik tutmuyor) farklıdır; yalnız kontrol edilen KolayBi
karelerinden **görünen sayı** aktarılabilir *(P1-B15)*; bu, hesaplamanın veya bütün veri kümesinin doğruluğunu kanıtlamaz. **Davranış yine de okunamaz** — bir
alanın ne tetiklediği hâlâ doğrulanmadı.

### Videonun anlattığı (transkript özeti)

- **Ön muhasebe tanımı** (00:08): "işletmelerin ücretle ifade edilen işlemleri —
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

### Ekran seviyesi akış (video kareleri — d33–d38 ~2020 tanıtım videosu, d39 ayrı 2026 videosu)

| # | Kare | Ekranda görünen | BusinessFinance açısından |
|---|---|---|---|
| d33 | `d33-video-guncel-durum-panosu.png` | **Güncel Durum** panosu. (a) "Son 4 Haftalık Nakit Akışı" çizgi grafiği — tooltip **iki seri**: `TRY Gelir` / `TRY Gider` (21.5.2020, ikisi de 0). (b) Takvim (Mayıs 2020, 21'i işaretli). (c) **"Tahsilat Ve Ödeme Özetleri"**: `Tahsilatlar` ve `Ödemeler` satırlarının her biri **üç donut** — **Vadesi Gelmemiş** (yeşil, ₺0,00) / **Vadesi Geçmiş** (kırmızı, ₺801,70) / **Vadesi Belirsiz** (gri, ₺322.835,33). (d) Sağda "Günü Gelen İşlemler". Sekmeler: Güncel Durum / Stok Akışı / Notlar. Üst sağ "Hızlı İşlemler". Üstte `#EVDEKALTÜRKİYE` şeridi. **Sol menü tam 13 modül:** Güncel Durum, Satış Yönetimi, Satın Alma Yönetimi, Genel Gider Yönetimi, Ürün ve Hizmetler, Cari Hesaplar, Finans, Projeler, Raporlar, Ek Özellikler, Abonelik, Destek, Ayarlar (+ altta "Menüyü Sakla") | Bizim **Özet ekranı**nın karşılığı; özet + upcoming-payments birleşimi. **Üçlü vade halinin ekran kanıtı burada** — transkriptteki iddia donut etiketleriyle doğrulanıyor ve kalem sayısı değil **tutar** gösteriliyor. Nakit akışı grafiğinin gelir/gider iki serisi bizim aylık rapor kırılımımıza yakın. "Hızlı İşlemler" = bizim "İşlem ekle" launcher. `#EVDEKALTÜRKİYE` ve takvim eski içerik bağlamını destekler; video yayın tarihini tek başına doğrulamaz |
| d34 | `d34-video-gunu-gelen-islemler.png` | "Günü Gelen İşlemler" kartı yakın çekim: **Bugün / Yaklaşanlar / Tarihi Geçenler** sekmeleri (Tarihi Geçenler seçili, yanında `*`); üç açılır satır — Faturalar / Genel Giderler / Çalışan Maaşları — her birinde **yeşil ve kırmızı iki sayaç** (Faturalar `1`/`0`, diğer ikisi `0`/`1`). Faturalar açılınca satır: **"$118,00 — 71 Gün Gecikti"** | Bizim `readiness` / `attentionCode` ayrımıyla birebir. Kalem türü kırılımı (fatura / gider / maaş) = bizim `sourceGroup`. **Sayaçlar adet, üstteki donutlar tutar** — aynı panoda iki farklı okuma; bizim planlanan görünümde yalnız tutar var. Tutarın `$` ile gösterilmesi çoklu para birimini ikinci kez doğruluyor (kare d35'teki "Yerel Tutar" kolonuyla birlikte) |
| d35 | `d35-video-vadesi-belirsiz-tahsilatlar.png` | "Vadesi Belirsiz Tahsilatlar" tam liste: Cari Bilgisi / Seri No / Düzenlenme Tarihi / Fatura Tutarı / **Yerel Tutar** / Bakiye / **Durum** (Ödenmedi / **Kısmen Ödendi**). Dışarıya Aktar + filtre + Ara | **Video grubundaki dolu tahsilat listesi** (~13 satır, çoğu `Ödenmedi`). **Kısmen Ödendi durumu demo satırında görünüyor**: `TEST MAİLSİZ CARİ`, seri `NKA2020000000030`, 05.02.2020, Fatura ₺118.000,00 / Yerel ₺118.000,00 / **Bakiye ₺116.741,00** → **Kısmen Ödendi**; bizde `CounterpartyPayment` de kısmi tutar kabul eder, ama KolayBi'nin bakiyesi **fatura satırında** duruyor; bizim `CounterpartyBalance` karşı taraf toplamıdır ve tahsilatı belirli borçlandırmaya bağlamaz *(P1-B01 düzeltmesi, 14 Eyl 2026: önceki metin Bakiye kolonunu projection'ımızın karşılığı sayıyordu)*. **Fatura tutarı ile bakiye ayrı iki kolon** — kayıt tutarı değişmiyor, ödeme yalnız bakiyeyi düşürüyor: ADR 0014'ün "tanır vs taşır" ayrımıyla uyumlu (`çıkarım`). "Vadesi belirsiz" = vade girilmemiş kayıt, bizde ayrı bir hal yok. "Yerel Tutar" = çoklu döviz; bizde tek para birimi. Seri öneki `NKA` = NKolayOfis dönemi |
| d36 | `d36-video-cari-hesaplar.png` | "Genel Cari Hesapları": sekmeler **Genel Cariler / Potansiyel Müşteriler / Personel Carileri / Ortaklar**. Kolonlar: Kod / Unvan / Cari Tipi / Etiketler / VKN-TCKN / **Yerel Bakiye**. Cari Oluştur / İçe Aktar / Dışarıya Aktar | **"Personel Carileri" ve "Ortaklar" ayrı sekme** — Paraşüt'te de benzer cari yapısı var. İşletme/şahsi tek havuz görülmediği için patron/ortak parasının ayrı bir cari borç-alacağına dönüştüğü `çıkarım`dır; kullanım kaynakta anlatılmıyor, kullanıcı davranışı gözlenmedi *(P1-B08)*. **Kanıt seviyesi: sekme adı.** Tabloda tek demo kayıt var (`CAR000001 / KolayBi A.Ş / Müşteri / ₺0,00`) ve Ortaklar sekmesi açılmadı — "patron parası buradan giriyor" bir `çıkarım`, gözlem değil; ADR 0013 farkımızın kanıtı olarak **sekmenin varlığı** sayılır, işleyişi değil. `Cari Tipi` ayrı bir alan (burada "Müşteri"). İçe Aktar düğmesi transkriptteki toplu içeri aktarma iddiasını doğruluyor; ayrıca `Detaylı Arama` + filtre + Ara var. **Bu kare `d07-destek-cari-listesi.png` tarafından her eksende aşıldı** (5 sekme, dolu tablo, dört `Cari Tipi` değeri, Telefon kolonu); `d36-video-cari-hesaplar.png`'in kalan tek benzersiz katkısı **`Ortaklar` sekmesinin ~2020 videosunda da görüldüğünü** göstermesi — ürünün başlangıç tarihi veya ortak-carisi işleyişi bu kareyle kanıtlanmaz |
| d37 | `d37-video-urun-ve-hizmetler.png` | "Ürün Ve Hizmetler": sekmeler Tümü / Ürünler / Hizmetler; Ürün/Hizmet Oluştur + İçe Aktar + Dışarıya Aktar + filtre + Ara. Kolonlar: Tür / Kod / Ad / Etiketler / Alış Fiyatı / Satış Fiyatı / **KDV** / **İndirim**. **Tablo boş** ("Tabloda herhangi bir veri mevcut değil"). Alt barda URL: `https://test-ofis.noffix.com/product/all` | Ürün kartında **KDV kolonu var** (gözlem); "faturada KDV bu alandan hesaplanıyor" ise `çıkarım` — boş tabloda hesaplama görülmedi. Fark yine de duruyor: KDV burada ürüne bağlı bir **oran alanı**, bizde kaydın taşıdığı bir bilgi (ADR 0016: taşırız, hesaplamayız). Ürün/hizmet kataloğu bizde yok (kayıt adı serbest metin). `test-ofis` = demo kiracı, `noffix` = NKolayOfis mirası |
| d38 | `d38-video-finans-kasalar.png` | "Kasalar" (Finans): sekmeler **Banka Hesapları / Kasalar / Online Banka Hesapları / Çekler**. Kolonlar: Adı / Etiketler / **Açılış Tarihi** / Para Birimi / Bakiye. "Ana Kasa", açılış 03.03.2020 | Kasa = elle, **"Online Banka Hesapları" ayrı sekme** = entegrasyonla çekilen; ikisinin ayrı sekmede durması, bizim "banka bağlantısı kapsam dışı" kararımızın rakipte nasıl izole edildiğini gösteriyor. **"Açılış Tarihi + Bakiye" = bizim `Account.OpeningBalance` modeli** (tek kayıt: Ana Kasa / 03.03.2020 / TRY / ₺0,00). Çek ayrı defter (bizde yok). Hesapta **Etiketler** kolonu var — bizde aynı işi tek eksende `DefaultScope` yapıyor |
| d39 | `d39-guncel-arayuz-2026.png` | **Daha yeni bir sürüm** (2026 başında yüklendiği aktarılan ayrı videodan; bugünkü sürüm olduğu doğrulanmadı — P1-B15; kiracı adı "KolayBi' Eğitim"): marka "KolayBi **Ofis**"; üstte **"Size Özel Ayrıcalıklar"** şeridi (TotalEnergies `YENİ`, Oyak Grup Sigorta `YENİ`, Dijital Köprü, KolayBi Banka); sol menüde yeni **"Fatura Ödeme"** — 13 → **14 modül**, diğer 13'ü aynı sırada; grafik başlığı **"Son 1 Haftalık Nakit Akışı"** (~2020 karesinde "Son 4 Haftalık"), tooltip yine `TRY Gelir` / `TRY Gider`; sağda **"KolayBi' Yolu Var!"** beş satır (Davet Et & Kazan, **iWallet**, Müşavirini Davet Et, Blog', Sosyal Medya) + "Mobil Uygulamaya Giriş Yap"; "Günü Gelen İşlemler" burada sekme değil **düz bağlantı listesi**; üstte 17 okunmamış bildirim rozeti | ~2020 karesinden d39'a pano **daha pazarlama-yoğun** (çapraz satış şeritleri), cüzdan (iWallet) ve fatura ödeme eklenmiş → banka/ödeme yönünde genişleme. İki karede çekirdek menü sırası ve pano bölümleri benzer görünüyor; aradaki sürümler görülmediği için bu, iskeletin altı yıl değişmediğinin kanıtı veya kararımız için istikrar sinyali değildir. Özet alanının altı kesik; görünen grafik başlığı 4 hafta → 1 hafta farklı. *(P1-kolaybi-G01 düzeltmesi, 14 Eyl 2026.)* "Müşavirini Davet Et" = canlı muhasebeci erişimi hâlâ ürünün merkezinde |

### Ekrandan ekrana pipeline (videodan)

`kurulum: Cari Hesaplar (manuel/toplu) + Ürün-Hizmetler + Finans (kasa/banka) →
Hızlı İşlemler ile fatura/gider/tahsilat kaydı → projeye etiketle (ops.) →
Güncel Durum panosu: nakit akışı grafiği + Tahsilat/Ödeme Özetleri (vadesi
gelmemiş/geçmiş/belirsiz) + Günü Gelen İşlemler → Raporlar / proje kâr marjı →
ay sonu müşavire`

### Karşılaştırma notu

Video, metin kaynaklı okumayı doğruluyor ve iki noktayı keskinleştiriyor:

- **İşletme/şahsi tek havuz görülmedi; "Ortaklar / Personel Carileri" ayrı
  cari sekmeleri var** (birincil kanıt `d07-destek-cari-listesi.png`;
  `d36-video-cari-hesaplar.png` aynı sekmenin ~2020 videosunda da bulunduğunu gösteriyor).
  Ortaklar sekmesi hiçbir karede açılmadı; patron parasının bu sekmeden ortak
  carisi borç-alacağı olarak girdiği `çıkarım`dır *(P1-kolaybi-G01 düzeltmesi,
  14 Eyl 2026)*. Paraşüt'te de benzer sekme var. Bu, muhasebe dünyasının bu soruna verdiği
  **yerleşik cevap**: sahibi ile şirketi iki ayrı taraf sayıp aradaki akışı
  borç-alacak olarak yürütmek. Bizim tek havuz + kapsam yaklaşımımızla aynı
  soruna (patron parasının izlenmesi) verilebilecek iki farklı cevap; rakipte
  bu yolun gerçekten kullanıldığı çıkarımdır *(P1-B08)*. Karşılaştırma Belge 3'te.
- **"Vadesi gelmemiş / geçmiş / belirsiz" üçlü vade hali** yalnız transkriptte
  değil, **kare d33'te donut etiketi olarak ekranda** — bizim
  `readiness`/`attentionCode` ayrımıyla neredeyse aynı. Üstelik hem tutar
  (donut) hem adet (kare d34 sayaçları) olarak iki ayrı okuma sunuluyor.
  "Belirsiz" (vade girilmemiş) bizde ayrı bir hal değil — düşünülebilir.
- **Kısmen Ödendi durumu dolu bir demo satırında görülüyor** (kare 04: ₺118.000
  fatura, ₺116.741 bakiye). Ödeme öncesi/sonrası görülmedi; gerçek müşteri
  verisi veya kısmi ödeme davranış testi değildir *(P1-kolaybi-G01 düzeltmesi,
  14 Eyl 2026: önceki metin "gerçek veriyle doğrulandı" diyordu)*. Fatura tutarı
  ile bakiyenin ayrı kolon olması ADR 0014 ayrımıyla uyumlu bir arayüz
  desenidir (`çıkarım`). KDV'nin ürün kartından geldiği ise **kolon
  adından çıkarım** (kare d37'nin tablosu boş) — ADR 0016 farkı duruyor ama
  kanıt seviyesi "alan var", "şöyle hesaplanıyor" değil.
- Proje bazlı gelir-gider bu videoda **ekran olarak gösterilmedi** (menüde
  "Projeler" var, transkriptte adı geçiyor). Proje ekranı Tur 2 kaynağı.

## Faz 6 — resmî destek merkezi taraması (masa başı, 12 Eyl 2026)

`Resmî kaynak (destek dokümanı)`. Kaynak: `kolaybi.com/destek/<modül>` modül
sayfaları. **Yöntem değişikliği:** Faz 6 için planlanan "Kullanım Rehberi
videosu" yerine önce resmî destek merkezi tarandı. Gerekçe: sayfaların **metni
sığ** ama içlerindeki **ekran görüntüleri güncel ve alan seviyesinde okunaklı**
(video karelerinden farklı, daha çok sekmeli arayüz; mockup'lardaki 2023 işlem tarihleri demo verisidir, arayüzün üretim tarihini kanıtlamaz — P1-B15). **Tarama tamamlandı (12 Eyl):** `destek/` altında **12 modül sayfası** indirildi,
toplam **117 mockup** bulundu ve tamamı gözden geçirildi; ayırt edici olan
**31'i** `kanitlar/kolaybi/d01`–`d31-destek-urun-varyantlar.png` olarak repoya alındı.

Bu tarama Tur 1'de açık kalan **üç boşluğun üçünü de** kapatıyor (proje ekranı,
gider formu, cari ekstre) ve ayrıca ~2020 videosundan gelen bir tespiti
**düzeltiyor** (kredi kartı — aşağıya bakın). **Kanıt seviyesi: alan ve ekran adı** — resmî
mockup'lar gerçek arayüzü gösteriyor ama davranış (ne yazınca ne oluyor) yine
doğrulanmadı; davranış cümleleri `çıkarım` etiketli.

### Proje modülü

| Ekran | Alanlar / içerik | BusinessFinance açısından |
|---|---|---|
| **Projeler listesi** (`d01-destek-proje-listesi.png`) | Proje Oluştur / İçe Aktar / Dışarıya Aktar / filtre / Ara. Kolonlar: Proje Kodu / Proje Adı / Etiketler / Para Birimi / Açıklama / **Durum** (Aktif-Pasif) / Başlangıç Tarihi / Bitiş Tarihi / **Gelir / Gider / Net** | Liste seviyesinde üçlü **Gelir / Gider / Net** — bizim aylık rapor üçlüsüyle aynı. Listede "Net", detayda "Kar / Zarar" deniyor: **farklı ekranlarda iki farklı etiket**; d01 listesi ile d03 Proje1 aynı kaydı göstermiyor, değerlerin eşitliği kanıtlanmadı. Biz tek ad kullanıyoruz ve "kâr" demiyoruz (kritik kısıt) |
| **Yeni Proje formu** (`d02-destek-yeni-proje-formu.png`) | Proje Kodu* (karede `PRJ000003` dolu; otomatik üretim denenmedi), Proje Adı*, **Para Birimi*** (TRY), Etiketler, Başlangıç Tarihi, Bitiş Tarihi, Açıklama, **Devam Eden Proje** anahtarı (Kapalı) | Boyutun kendisi bir **kayıt** (kodu, tarihi, para birimi olan bir varlık). Bizim kapsamımız iki değerli bir enum — kullanıcı yeni kapsam yaratamaz. Fark kasıtlı: proje sayısı sınırsız, kapsam ikili |
| **Proje detayı — Özet** (`d03-destek-proje-detay-ozet.png`) | Başlıkta **Aktif** rozeti; `Etiket Düzenle`, **`Durum Değiştir → Aktif / Pasif`**, `İşlemler`, sil. Sekmeler: **Özet / Toplam Gelir / Toplam Gider / Tahsilatlar Toplamı / Ödemeler Toplamı / Detaylar / Notlar / Dosyalar**. Özet altında iki alt sekme: **Kar / Zarar** ve **Nakit Durumu**. Kartlar: Kar/Zarar; Toplam Gelir → alt kırılım **Tahsil Edilen / Bekleyen**; Toplam Gider → alt kırılım **Ödenen / Bekleyen** | **Bu formun en önemli bulgusu.** KolayBi tahakkuk ile nakdi aynı ekranda ayırıyor: "Toplam Gelir" (tanınan) ve onun içinden "Tahsil Edilen" (taşınan). **ADR 0014'ün "tanır vs taşır" ayrımının rakipteki en açık görsel karşılığı** — üstelik iki ayrı sekme olarak (`Kar/Zarar` = tahakkuk görünümü, `Nakit Durumu` = nakit görünümü). Bizde veresiye satış ve vadeli alım olay gününde gelir/gider yazılır, tahsil edilmemiş kısım cari bakiyede açık alacak olarak ayrı görünür; ancak aynı rapor kartında "tanınan / tahsil edilen" kırılımı yok. KolayBi ikisini aynı kartta veriyor. *(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki metin bizdeki işletme netini "yalnız nakit esaslı" diye anlatıyordu.)* **Ne kazandırıyor:** "kazandım ama tahsil etmedim" ile "param arttı" aynı ekranda ayrı okunuyor — veresiye çalışan esnafın en çok karıştırdığı iki sayı. **Ne kaybettiriyor:** iki sayı iki kavram demek; hangisinin "işin durumu" olduğunu kullanıcı seçmek zorunda kalıyor ve tahakkuk görünümü "kâr" kelimesini gerektiriyor. Belge 3'te tartışılacak gerçek bir tasarım farkı. `Durum Değiştir → Pasif` seçeneği var; yanında silme simgesi de var. Silme yerine yalnız pasifleştirme kuralı olduğu kanıtlanmaz |
| **Proje detayı — belge kırılımı** (`d04-destek-proje-belge-kirilimi.png`) | Kartlar belge türüne göre: **Satış Faturaları** (Tahsil Edilen/Bekleyen) · **Satın Alma Faturaları** (Ödenen/Bekleyen) · **Satış İade Faturaları (−)** · **Satın Alma İade Faturaları (+)** · **Para Girişi** (Tahsil Edilen) · **Genel Giderler** (Ödenen/Bekleyen) | Proje neti **kaynak belge türüne göre parçalanıyor** — bizim kaynak türü kırılımıyla karşılaştırılabilecek arayüz deseni; birebir model eşdeğerliği değil. İade faturalarının işaretinin (`−`/`+`) kart başlığında yazması iyi bir okunabilirlik fikri. Ayrıca kart rengi **belge ailesine değil nakit yönüne** bağlı: `Satış Faturaları` yeşil ama `Satış İade Faturaları ( - )` kırmızı, `Satın Alma Faturaları` kırmızı ama `Satın Alma İade Faturaları ( + )` yeşil — renk her zaman "para bana mı geliyor" sorusunu cevaplıyor |

**Demo proje adları — gözlem ile ihtiyaç hipotezi ayrımı (G01 düzeltmesi).**
`d01-destek-proje-listesi.png` içinde Ev Elektrik, Ev Su, Bebek Bakım, Kira
Ödemesi ve İnşaat Masrafları gibi adlar bulunuyor. Bu, destek materyalindeki
örnek adların gözlemidir. Kayıtları gerçek kullanıcıların hangi amaçla
oluşturduğu bilinmiyor; hane harcaması ihtiyacı veya amaç dışı kullanım
kanıtı sayılmaz (B08 / KB-Q11).

Yeni Proje formu ad, kod, tarih ve para birimi alanlarını gösteriyor.
Kullanıcı tanımlı proje ile sabit kapsam boyutu farklı tasarım seçenekleridir.
Şube/araç gibi başka amaçlara uyarlama olasılığı bir hipotezdir; projenin
sınırsızlığı, bu kullanımların gerçekleştiği veya raporların ayrım yapamadığı
bu karelerden çıkarılmaz. B08 paketinde diğer form/özetlere yayılım tamamlanacak.

### Genel Gider modülü

Sekmeler: **Genel Giderler / Genel Gider Tipleri / Tekrarlı Genel Giderler**.

**`d05-destek-yeni-gider-formu.png` — "Yeni Genel Gider" formu, `TEMEL BİLGİLER` bölümü:**

| Alan | Değer / davranış | BusinessFinance açısından |
|---|---|---|
| **Cari Takibi** | Radyo: **Yok / Var** | Cari Takibi seçeneği aynı formda sunuluyor. Yok/Var seçiminin cari ve kasa etkisi bu karede ölçülmedi; iç kayıt türleri çıkarılmaz |
| **Ödeme Durumu** | Radyo: **Ödenmedi / Ödendi** | Ödenmedi/Ödendi seçimi arayüzde var. Bunun hangi finansal kayıtları oluşturduğu ve çifte sayım davranışı bu mockup ile bilinmez; ADR 0014 ile model eşdeğerliği kurulmaz |
| **Son Ödeme Tarihi** | Varsayılan **"Vade Günü Girilmemiştir"** | Videodaki "Vadesi Belirsiz" halinin kaynağı burası: vade **boş bırakılabilir bir alan**, ayrı bir durum değil. Bizde `attentionCode` böyle bir hâl taşımıyor |
| **Gider Tipi\*** | Zorunlu, açılır liste | Bizim kategorimiz |
| **Proje** | Açılır liste + not: *"Ayarlar sayfasından Proje Takip seçeneğini kapatabilirsiniz"* | **İkinci raporlama boyutu ayarlardan tamamen kapatılabiliyor.** Bizim onboarding cevabının kapsam boyutunu arayüzde gösterip gizlemesiyle **aynı desen** — bağımsız olarak aynı çözüme varılmış. **Aynı not dört ayrı formda birebir tekrarlıyor** (`d05-destek-yeni-gider-formu.png` gider, `d08-destek-cari-olusturma-formu.png` cari açılış bakiyesi, `d13-destek-alis-faturasi-formu.png` alış faturası, `d30-destek-satis-fatura-formu.png` satış/iade) — yani anahtar tek yerde ama etkisi ürün genelinde ve tutarlı |
| Düzenlenme Tarihi / Seri No / Etiketler / Açıklama | Açıklama açılır liste + serbest metin + **"Açıklamayı şablon olarak kaydet"** | Şablon fikri bizde yok; tekrar eden açıklamalar için ucuz bir kolaylık |
| **Para Birimi / Takip Para Birimi** | İki ayrı alan, ikisi de TRY | Çoklu döviz; bizde tek para birimi |
| **Dosya Yükle** | jpg/png/jpeg/bmp/gif/doc/docx/xlsx/csv/pdf, **maks. 5 MB** | Yalnız **ek dosya** — form üzerinde fiş OCR'ına dair bir iz yok (`çıkarım`: web tarafında fiş okuma yok; ADR 0011 karşılaştırması için mobilde doğrulanmalı) |
| Alt toplam bandı | **Ara Toplam / Toplam KDV / Toplam KDV / Genel Toplam** | İki Toplam KDV başlığı görünüyor; alt bant kesik. Hesaplama yöntemi ve tekrar eden başlıkların anlamı doğrulanmadı. ADR 0016 farkımız: biz KDV'yi taşırız, hesaplamayız |

**`d06-destek-gider-tipleri.png` — Gider Tipleri yönetimi:** kategoriler **iki seviyeli** (Kategori →
Tip), `Kategoriyi Düzenle` + `Yeni Tip Ekle` + `Yeni Kategori Ekle`.
Görünen kategoriler ve örnek tipleri: *(Doğalgaz, Kömür, İnternet)* ·
`Faiz / Komisyon` · *(Donanım, Temizlik)* · **Ulaşım/Konaklama** (Bilet,
Seyahat Harcaması, Akaryakıt, Araç Kiralama, Otopark Ücreti) · **Temel
Giderler** (Kira, Yemek Harcaması, Muhasebe/Mali Müşavir, İletişim Gideri,
Ağırlama Gideri) · **Vergi** (MTV, Gerçek Usulde Katma Değer Vergisi, Kurumlar
Vergisi, AGİ, Stopaj) · **Diğer** (Market, Kargo, Reklam/Tanıtım).

İki not: (1) bizim kategori modelimiz **tek seviyeli**, KolayBi iki seviyeli —
Belge 3'e alt kategori tartışması olarak girer. (2) **Bu ekranda Vergi adlı gider kategorisi var**; bu, üründe ayrı vergi altyapısı bulunmadığını kanıtlamaz — MTV/KDV/Kurumlar/AGİ/Stopaj birer
"gider tipi". Bu, Aşama 05'te aldığımız kararla (vergi takvimi ayrı altyapı
değil, tekrarlayan plandır) aynı yönde ve onu destekliyor.

### Cari modülü ve cari ekstre

| Ekran | İçerik | BusinessFinance açısından |
|---|---|---|
| **Cari listesi** (`d07-destek-cari-listesi.png`) | Sekmeler **beş**: Genel Cariler / Potansiyel Müşteriler / Personel Carileri / Ortaklar / **Tekrarlı Maaşlar** (~2020 video karesinde dört sekme görünüyor). Kolonlar: Kod / Unvan / **Cari Tipi** (`Müşteri`, `Tedarikçi`, `Müşteri / Tedarikçi`, `Yurt Dışı`) / Telefon / Etiketler / VKN-TCKN / **Yerel Bakiye** (pozitif yeşil, negatif kırmızı). `Toplu Seç`, favori yıldızı, "1000 adet kayıt listelenmektedir; daha fazlası için detaylı arama" notu; toplam kayıt kapasitesi değil | **Yerel Bakiye tek işaretli sayı** — yön işaretle kodlanmış; bizim `CounterpartyBalance` + `DebtDirection` modelimizle aynı sonuç. `Müşteri / Tedarikçi` birleşik tipi: aynı karşı tarafla hem alacak hem borç ilişkisi — bizde de tek `Counterparty` iki yönde hareket taşıyabiliyor |
| **Cari oluşturma** (`d08-destek-cari-olusturma-formu.png`) | İki adım: `Cari Bilgileri` → `Cari Detay Bilgileri`. Detayda: **Vade Günü** (Yok/Var), **Sabit İskonto** (Yok/Var), **Açılış Bakiyesi** bloğu (Tutar, Para Birimi*, **Durumu**, **Proje**, İşlem Tarihi, **Vade Tarihi Var Mı?**), `Borç Alacak Ekle`, `Banka Ekle` | **Cariye açılış bakiyesi** verilebiliyor — bizim `Account.OpeningBalance`'ın cari karşılığı; bizde cari bakiyesi yalnız hareketlerden doğuyor, açılış devri yok. `Vade Günü` cari bazında varsayılan vade = bizde yok |
| **Cari detayı + ekstre diyaloğu** (`d09-destek-cari-detay-ekstre-dialog.png`) | Üst eylem çubuğu: **`Borç/Alacak Ekle` · `Fatura Ekle` · `Ödeme/Tahsilat Ekle` · `İşlemler`**. `İşlemler` menüsü: **Mahsuplaştır**, **Cari Ekstresi Oluştur**, Sık Kullanılanlara Ekle, **Cariyi Pasifleştir**, Sil. Sekmeler: Hareketler / Bilgiler / Notlar / … ; `TRY` ⇄ `Toplam` para birimi anahtarı; **Toplam Bakiye ₺1.800,00** kartı. **Cari Hareketleri** tablosu: İşlem Tarihi / **Vade Tarihi** / **İşlem Türü** (ör. `Satış Faturası`) / Evrak-Seri No / Açıklama / **Borç** / **Alacak** / Döviz Kuru / Yerel Tutar / **Bakiye** (yürüyen) / **Banka/Kasa** / **Proje** | `Borç/Alacak Ekle` ile `Ödeme/Tahsilat Ekle` ayrı düğmelerdir. Arayüzde işlerin ayrılması kanıtlıdır; kayıtların tanıma/taşıma kuralları veya model eşdeğerliği bu kareden çıkarılmaz. `Cariyi Pasifleştir` = pasifleştirme kuralımız (yanında `Sil` de duruyor). **`Mahsuplaştır`** (alacakla borcu netleştirme) bizde **yok** — Belge 3'e aday. Hareket satırının `Banka/Kasa` ve `Proje` kolonu taşıması: ödemenin hangi kasadan çıktığı harekette görünüyor |
| **Cari ekstre üretimi** (`d09-destek-cari-detay-ekstre-dialog.png` diyalog + `d10-destek-cari-ekstre-onizleme.png` önizleme) | Diyalog: Başlangıç/Bitiş Tarihi, **Sayfa Düzeni** (Dikey), **Ekstre Para Birimi***, Açıklama + **yedi isteğe bağlı kolon anahtarı**: Döviz Kuru · Döviz Karşılığı · **Proje** · **Banka/Kasa** · Açıklama · Etiket · Stok Bilgisi (karede hepsi **Kapalı**; varsayılan olduğu denenmedi). Düğmeler: Kapat / **Yazdır** / Oluştur. Önizleme: destek karesinde PDF görüntüleyici — başlıkta firma adres/telefon/e-posta, `Cari Hesap Ekstresi`, Cari Kodu, Cari Adı, **Tarih Aralığı**, **Rapor Tarihi**; kolonlar İşlem Tarihi / Vade Tarihi / İşlem Tipi / Evrak-Seri No / Borç / Alacak / Para Birimi / Bakiye; `Toplam (TRY)` satırı. Eylemler: Yeni Sekmede Görüntüle / **E-Posta ile Gönder** / Kapat | **Ekstre bir belge, ekran değil** — tarih aralığı seçilip PDF üretiliyor ve önizlemede **E-Posta ile Gönder düğmesi var; gönderim yapılmadı**. Bizde cari ekstresi yok; en yakın akraba muhasebeci paketi (dosya tabanlı dışa aktarma). "Karşı tarafa hesap özeti gönderme" işlevi bizde yok (ihtiyacın büyüklüğü ölçülmedi) — Belge 3'e aday. Kolon anahtarlarının **karede hepsinin kapalı** olması iyi bir varsayılan: ekstre önce sade, isteyen açıyor |


### Finans modülü — **video kaynaklı hatanın düzeltmesi**

Finans altı sekme taşıyor: **Banka Hesapları / Kasalar / `Kredi Kartları` /
Online Banka Hesapları / Çekler / Senetler.** ~2020 videosunun karesinde
(`d38-video-finans-kasalar.png`, Kasalar sekmesi) yalnız **dört** sekme vardı
(Banka Hesapları / Kasalar / Online Banka Hesapları / Çekler) ve bu forma
"KolayBi'de kredi kartı kavramı yok" diye geçmişti. **Bu tespit yanlış** —
destek mockup'larındaki sürümde kredi kartı ayrı bir finans sekmesi. **Aradaki fark iki
sekme:** `Kredi Kartları` **ve `Senetler`** destek karesinde var, video karesinde yok
(`d19-destek-banka-hesaplari.png`); ne zaman eklendikleri bilinmiyor *(P1-B15)*.

| Ekran | İçerik | BusinessFinance açısından |
|---|---|---|
| **Banka Hesapları** (`d19-destek-banka-hesaplari.png`) | Kolonlar: Banka Adı / Etiketler / Banka-Şube / **IBAN** / Açılış Tarihi / Para Birimi / Bakiye. Sağ üstte **Toplam Bakiye** + **TRY Bakiye** (her sekmede) | "Açılış Tarihi + Bakiye" deseni her hesap türünde aynı = bizim `Account.OpeningBalance` |
| **Kredi Kartları listesi** (`d20-destek-kredi-kartlari-listesi.png`) | Kolonlar: Kredi Kartı Adı / Etiketler / Kart Numarası / **Hesap Kesim Günü** / **Son Ödeme Günü** / **Kart Limiti** / Ek Bilgiler / Açılış Tarihi / Para Birimi / **Kalan Limit** | **Tablo boş** ("Kayıt yok"), yani yalnız **kolon adları** gözlem. `Kalan Limit`'in limit − borç olarak hesaplandığı bir **`çıkarım`**; ekranda hesaplanmış tek bir değer görülmedi. Bizde kart borcu hareketlerden hesaplanıyor; kullanılabilir limit domain'de hesaplanıp kart listesinde ve ayrıntısında "Kullanılabilir" olarak gösteriliyor *(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki metin bu gösterimin bizde olmadığını söylüyordu)* |
| **Yeni Kredi Kartı formu** (`d21-destek-yeni-kredi-karti-formu.png`) | Ad\* · Etiketler · Kart Numarası\* · **Hesap Kesim Günü\*** · **Son Ödeme Günü\*** · Açılış Tarihi\* · Para Birimi\* · **Kart Limiti\*** · **Minimum Ödeme Oranı (%)\*** · "Detay ekle" | **Bizim `CreditCard` modelimize en yakın rakip tanımı** — kesim günü + son ödeme günü ikilisi bizde de var. Asgari ödeme oranı bizde de var (varsayılan %20, kullanıcı değiştirir; asgari tutar ekstreden hesaplanır ve "Asgariyi öde" ile ödenebilir) ve kart limiti bizde de **zorunlu**. Görünen fark kart numarası ve açılış tarihi alanlarıdır. *(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki metin asgari ödeme oranının bizde olmadığını ve limitin bizde zorunlu olmadığını söylüyordu.)* |
| **Çekler** (`d22-destek-cekler.png`) | Kolonlar: Alış/Veriliş Tarihi / İşlem Türü / **Proje** / Cari / Seri No / **Keşideci** / **Hamil** / **Keşide (Vade) Tarihi** / Tutar / Durum / İşlem Tarihi / Etiketler. `Toplu Çek Ekle`, `Bordrolar` | Ayrı bir kıymetli evrak defteri; bizde yok ve kapsam dışı |
| **Senetler** (`d23-destek-senetler.png`) | Kolonlar: … **Kefil** / Tutar / **Ödenen Toplam Senet Tutarı** / **Taksit Durumu** / **Kalan Toplam Senet Tutarı**. `Senet Gösterimi: Toplam Özet` anahtarı; ciro (endosman) notu | **Tablo boş**; senedin taksitli ve kısmi ödenebilir olduğu **`çıkarım`** — yalnız `Ödenen Toplam` / `Taksit Durumu` / `Kalan Toplam` **kolon adlarından** okunuyor, tek bir senet kaydı görülmedi. Doğruysa bizim `DebtAgreement` + taksit modelimizin muhasebe dünyasındaki karşılığı. Not: `Çekler`de `Proje` kolonu var, `Senetler`de yok |

### Personel carileri ve maaş

`Personel Carileri` gerçek bir modül: `Cari Tipi` değerleri **Serbest Çalışan /
Yarı Zamanlı Çalışan / Tam Zamanlı Çalışan**, her personelin **Yerel Bakiye**'si
var (`d14-destek-personel-carileri.png`). Kod öneki çoğunlukla `EMP…` ama **tek tip değil** — aynı listede
iki satır `CAR00001` kodunu taşıyor, yani personel carisi genel cari kod
uzayını paylaşabiliyor. Listede Helin Kınay `Serbest Çalışan` ve ₺0,00;
detay karesinde aynı ad `Yarı Zamanlı Çalışan` ve -₺100,00 gösteriyor — iki
kare aynı veri anının ardışık görüntüsü sayılmaz *(G02)*.

Personel cari detayında (`d15-destek-personel-cari-detay.png`, `d16-destek-maas-odeme-secimi.png`) düğmeler: `Borç/Alacak Ekle` ·
`Ödeme/Tahsilat Ekle` · **`Maaş / Prim Oluştur`** · **`Ödeme Yap ▾`**
(Maaş Ödemesi Yap / Prim Ödemesi Yap / **Avans Ver**) · `İşlemler ▾`
(Mahsuplaştır / **Per. Cari Ekstresi Oluştur** / Sık Kullanılanlara Ekle /
Cariyi Pasifleştir / Sil). Kartlar: **Toplam Borç / Toplam Alacak / Toplam Bakiye**
(`d15-destek-personel-cari-detay.png`'te ₺0,00 / ₺100,00 / **-₺100,00**). Renkler firmanın pozisyonuna göre:
personel carisinde `Toplam Borç` **yeşil**, `Toplam Alacak` **kırmızı** —
çalışana borçlu olmak kırmızı tarafta. Başlıktaki rozetler üç tane:
çalışma tipi (`Yarı Zamanlı Çalışan`), `Aktif Cari` ve **serbest bir unvan
etiketi** (`Genel Müdür Yardımcısı`).

**En önemli davranış kanıtı:** `Maaş Ödemesi Yap` bir diyalog açıyor —
*"Ödeme yapmak istediğiniz maaşı seçiniz."* — ve iki maaş satırı listeliyor
(22.04.2023 ₺1.000,00 ×2). Diyalog satırların ödenmemiş olduğunu yazmıyor;
ödenmemiş/kalan tahakkuk listesi olduğu `çıkarım`dır ve aynı karedeki -₺100
toplam bakiyeyle uzlaşmıyor *(P1-kolaybi-G02 düzeltmesi, 14 Eyl 2026)*. Yani
ödeme, **hangi maaş kaydını kapattığını seçiyor** gibi görünüyor; seçim
sonrası ekran yok. Rakiplerde ödemeyi belirli tahakkuka bağlayan somut bir
örnek — Bluecoins'te bu bağ görülmedi (D2 ile D3 bağımsız iki hareketti), Wallet'ta
Debt/Record ile görüldü, KolayBi'de **seçim diyaloğu** olarak görünüyor.
*(P1-B01 düzeltmesi, 14 Eyl 2026: önceki metin bunu ADR 0014'ün "tanıyan ve taşıyan
kayıt ayrı ve birbirine bağlı" tezinin uygulaması sayıyordu. ADR 0014 iki kaydı
ayırır ama bağlanmalarını istemez. BusinessFinance'te cari tahsilat belirli
borçlandırmaya bağlanmaz; belirli kayda bağlı kapanış tek seferlik yükümlülükte
(tam tutar) ve borç taksitinde vardır. Diyaloğun davranışı mockup'tan okunur,
canlı doğrulanmadı.)*

Cari hareketleri tablosunun üstündeki uyarı bandı (`d15-destek-personel-cari-detay.png`, tam metin):
> • Cariye eklenen **borç ve ödeme** kayıtları proje ile ilişkilendirildiği
>   durumda projede **'Nakit Ödemeler'** bölümünde gösterilecektir.
> • Cariye eklenen **alacak ve tahsilat** kayıtları proje ile
>   ilişkilendirildiği durumda projede **'Nakit Tahsilatlar'** bölümünde
>   gösterilecektir.

Yani cari hareketi → proje **nakit** bölümüne akıyor; proje ekranındaki
`Nakit Durumu` sekmesinin kaynağı bu.

`Çalışan Maaşı` kaydı (`d17-destek-calisan-maasi-detay.png`): rozet **Ödenmedi**; `Ödeme Ekle ▾`; `İşlemler ▾`
= Gönder / **Tekrarlı Maaşa Dönüştür** / **Taslağa Al** / Düzenle / Sil.
Sekmeler: Detaylar / **Ödeme Planı** / Notlar / Dosyalar. Toplamlar:
Ara Toplam / **Brüt Toplam** / **Toplam Net Ücret** / Genel Toplam
(alan adları brüt/net ayrımı gösteriyor; karede dördü de ₺1.000,00, kesinti
hesabı görülmedi — bordro mantığı `çıkarım`; bizde bordro yok). Cari bilgisi
boş olduğu için kaydın hangi personele ait olduğu karede yok *(G02 düzeltmesi)*.

**`Tekrarlı Maaş Oluştur` diyaloğu (`d18-destek-tekrarli-maas-formu.png`) — karelerdeki tek tekrar formu; mevcut `Çalışan Maaşı` kaydında `İşlemler > Tekrarlı Maaşa Dönüştür` ile açılıyor (arkada menü vurgulu):**

| Alan | Not |
|---|---|
| **Oluşturma Periyodu\*** | Zorunlu, açılır liste (değerleri görünmüyor) |
| **Başlangıç Tarihi** | Karede 22.04.2023 — kaynak maaş kaydının tarihiyle aynı; "varsayılan bugün" desteklenmiyor *(G02 düzeltmesi)* |
| **Maaş Ödeme Tarihi** | Radyo: **Belirsiz / Belirli** |
| **Maaş Oluşturma Tekrar Sayısı\*** | **Zorunlu** — plan **sonlu** |

İki fark: (1) KolayBi planı **kaç kez üreteceğini zorunlu tutuyor**; bizim
`RecurringTransaction`'da bitiş tarihi ve toplam tekrar sınırı **isteğe bağlıdır**,
ikisi birlikte verilebilir ve önce dolan geçerlidir; ikisi de boşsa plan süresizdir.
*(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki metin planımızı yalnız süresiz
tanımlanır diye anlatıyordu.)* (2) Plan **tahakkuk üretiyor** (maaş kaydı), ödeme ayrı
adım — bizim "plan realize edilince `BudgetTransaction` doğar" modelimizle aynı
yönde. Onay adımı formda yok (`çıkarım`: üretim otomatik, Bluecoins'in
"Kaydet → materyalize" onayı gibi bir kapı görünmüyor).

### Genel Gider listesi ve kayıt eylemleri

**Liste kolonları** (`d11-destek-gider-listesi.png`): **e-Fatura Durumu** (`Aktarıldı`/`Aktarılmadı`) /
Cari Bilgisi / Etiketler / **Gider Tipi** / Seri No / **Proje** / **Durum**
(`Yeni`) / Düzenlenme Tarihi / Son Ödeme Tarihi / **Fatura Tutarı** / **Bakiye**
(altında **Ödendi / Ödenmedi / Bedelsiz** durum yazısı). `Detaylı Arama`,
`Toplu Seç`, `İçe/Dışarıya Aktar`, "1000 adet kayıt listelenmektedir" sınırı.

`Bedelsiz` üçüncü bir ödeme hâli — bizde karşılığı yok; ₺0 tutarlı kayıt da
açılamaz, tutar sıfırdan büyük olmalıdır. *(P1-B02-B04 düzeltmesi, 14 Eyl 2026:
önceki metin bunun bizde ₺0 tutarlı kayıt olacağını söylüyordu.)*

**`Bakiye` kolonu muhtemelen "kalan" demek değil (12 Eyl'de keskinleştirildi).**
Karedeki sekiz satırda desen **sistematik**: `Ödendi` satırlarında
`Bakiye = Fatura Tutarı` (₺30.000/₺30.000, ₺2.000/₺2.000), `Ödenmedi`
satırlarında `Bakiye = ₺0` (₺50.000/₺0, ₺500/₺0, ₺5.600/₺0, ₺30.000/₺0, ₺1.250/₺0). e-Fatura
`Aktarıldı` yalnız iki `Ödendi` satırında görünüyor; ilişki gözlemdir, kural değil.
Bu, "kalan borç" okumasının **tam tersi**. En tutarlı açıklama kolonun
**ödenen tutarı** göstermesi; ama `Bedelsiz` satırı (₺1.250/₺1.250) bu okumayı
da tam oturtmuyor. **Kolonun anlamı doğrulanmadı** — kareden çıkarılamaz,
davranış testi gerektirir.

**Yan doğrulama:** `Son Ödeme Tarihi` kolonu **sekiz satırın hepsinde `-`**.
Bu, `d05-destek-yeni-gider-formu.png`'teki "Vade Günü Girilmemiştir" varsayılanının gerçek veriye nasıl
yansıdığını gösteriyor ve videodaki **"Vadesi Belirsiz"** kovasının kaynağını
ekranda doğruluyor.

**Kayıt detayı eylemleri** (`d12-destek-gider-detay-islemler.png`): rozetler `Bedelsiz` · `Yeni` · `Doğalgaz` ·
**`Tahsilata Kapalı`**. Düğmeler: **`Durum Değiştir ▾`** · **`Ödeme Ekle ▾`** ·
`İşlemler ▾` = Gönder / **`Tekrarlı Genel Gidere Dönüştür`** / **`Tahsilata Aç`**
/ Düzenle / **`Kopyala`** / Sil. Sekmeler: Detaylar / **`Ödeme Planı`** / Notlar
/ Dosyalar. Bölümler: **`GENEL GELİR/GİDER BİLGİLERİ`** + **`PROJE BİLGİLERİ`** (+ sağ altta
`Yerel Toplamlar` kartı: Ara Toplam / Genel Toplam). **Bölüm başlığındaki
"GELİR/GİDER" ibaresi dikkat çekici:** modülün adı "Genel Gider Yönetimi" ama
kaydın kendisi gelir tarafını da taşıyor — yani bu, cariye bağlanmayan
**serbest gelir-gider** kaydı.

Dört fikir bizde yok:

1. **"…'a Dönüştür"** — mevcut bir kayıttan tekrarlayan plan üretmek. Aynı
   deyim maaşta da var (`Tekrarlı Maaşa Dönüştür`), yani ürün genelinde tutarlı
   bir kalıp. Bizde plan **sıfırdan** tanımlanır; "bu kaydı her ay tekrarla"
   yolu yok. **Belge 3 adayı.**
2. **`Ödeme Planı` sekmesi** — tek bir gidere bağlı ödeme takvimi; bizim
   `InstallmentPlan`'ımızın gider tarafındaki karşılığı.
3. **`Kopyala`** — kaydı çoğaltma; tekrar eden ama planlanamayan kalemler için
   ucuz çözüm.
4. **`Taslağa Al` / `Durum Değiştir`** — kaydın taslak hâli. Bizde taslak
   kavramı yok (Money Manager ve Bluecoins'te de yoktu).

### Satış / Satın Alma formları

Satış sekmeleri: Satış Faturaları / Satış İrsaliyeleri / Alınan Siparişler /
Satış Proformaları-Teklifler / **Tekrarlı Satış İşlemleri**.
Satın alma sekmeleri: Alış Faturaları / Alış İrsaliyeleri / Verilen Siparişler /
Alış Proformaları / **Tekrarlı Alış İşlemleri**.

Fatura formu ile gider formu **aynı iskeleti paylaşıyor** ama birebir aynı
değil. Alış faturası (`d13-destek-alis-faturasi-formu.png`): `TEMEL BİLGİLER` → **`Cari *`** (zorunlu) ·
Cari Adresi · Düzenlenme Tarihi + **Düzenlenme Saati \*** · Seri No ·
**`Ödeme Durumu` Ödenmedi/Ödendi** · **`Vade Tarihi`** ("Vade Günü
Girilmemiştir" açılırı) · Para Birimi · Takip Para Birimi | sağda Proje ·
Etiketler · Açıklama + şablon · Dosya Yükle; altta **`ÜRÜN/HİZMET BİLGİLERİ`**
satır tablosu.

**Üç fark (`d30-destek-satis-fatura-formu.png`, 12 Eyl'de fark edildi; G02'de kapsamı daraltıldı):** karedeki `Yeni Alış İade Faturası` formunda (a)
**`Ödeme Durumu` radyosu yok**, (b) `Vade Tarihi` bir açılır değil **`Yok/Var`
radyosu**, (c) `Düzenlenme Saati` yok. Buna karşılık satış tarafında ek olarak
**`E-ARŞİV FATURA BİLGİLERİ`** bölümü var. Farklar bu iade formu ile alış
faturası arasında gözlenir; satış faturası formu karelerde yok, onun hakkında
sonuç çıkarılmaz.

> **Kare adı uyarısı:** `d30-destek-satis-fatura-formu.png` dosya adı satış
> faturası diyor ama karedeki başlık **`Yeni Alış İade Faturası`**; üstteki
> sekmeler ve sol menü ise **Satış Yönetimi**'ni gösteriyor. Tutarsızlık
> KolayBi'nin **kendi destek görselinde**; kare adı içeriğe göre okunmalı.

Yapısal sonuç: **fatura = cari'si zorunlu, satır kalemli gider.** Gider formunda
cari opsiyonel (`Cari Takibi Yok/Var`) ve satır yerine `Gider Tipi` var. Bizde
bu ayrım kayıt türüyle değil, kaydın cariye bağlı olup olmamasıyla yapılıyor —
aynı fikir.

### Raporlar

**On rapor** (`d27-destek-kdv-raporu.png` üst barında tamamı görünüyor): Alış/Satış Raporları · Cari Bakiye Raporları · Banka/Kasa
Raporları · **KDV Raporu** · Stok Raporu · Ödeme/Tahsilat Raporu · Çek Raporu ·
Senet Raporu · **Gelir/Gider Raporu** · **Nakit Akış Raporu**.

| Rapor | İçerik | BusinessFinance açısından |
|---|---|---|
| **Alış/Satış Raporu** (`d26-destek-alis-satis-raporu.png`) | Alt sekmeler: Alış/Satış / Cari / **Proje** / Ürün ve Hizmetler / Etiket; tablonun **altında** ayrıca belge türü sekmeleri (Satış Faturası / Alış Faturası / Satış İade / Alış İade / Genel Gider) ile satır dökümüne inilebiliyor. Filtreler: Cari · Ödeme Durumu · Para Birimi · **Şube** · Fatura tarih aralığı · Fatura/Ürün Tipi · Etiketler · **Proje** · **Vade tarih aralığı**. Çıktı: satırlar Satış Faturası / Alış Faturası / Satış İade / Alış İade / **Genel Gider**, kolonlar TRY-USD-EUR-GBP + **Yerel Toplam**; **`KDV Dahil`** anahtarı; Dışarıya Aktar | **`Şube` dördüncü bir boyut** (proje ve etiketin yanında). Rakipte boyut sayısı artıyor; bizde bilinçli olarak tek boyut (kapsam) + kategori var. `KDV Dahil` anahtarı: aynı rapor brüt/net okunabiliyor — bizde kayıt tutarı **hep brüt** |
| **KDV Raporu** (`d27-destek-kdv-raporu.png`) | Matris: **KDV %20 / %18 / %10 / %8 / %1 / Diğer**, her biri **KDV Matrahı** + **KDV Tutarı**. Üst blok Satış Faturası + Alış İade → **Hesaplanan KDV Toplam**; alt blok Alış Faturası + Satış İade + adı kesik üçüncü satır; alt bloğun toplam satırı ve "İndirilecek" etiketi karede görünmüyor. `Matrahlı` anahtarı. Her satır başında **`+` genişletici** var (belge dökümüne iniliyor). Karedeki tek dolu hücre kendi içinde tutarlı: Satış Faturası %20 → matrah ₺12.000,00, tutar ₺2.400,00 (%20 ✓) | **ADR 0016 ile en keskin ayrıldığımız yer.** KolayBi oranlara göre matrah/tutar matrisi ve `Hesaplanan KDV Toplam` satırı gösteriyor; tutarın orandan mı hesaplandığı yoksa girilen KDV'nin mi toplandığı kareden çıkmaz *(G02 düzeltmesi: önceki metin "KDV'yi hesaplıyor" ve karede görünmeyen İndirilecek ayrımını kesin yazıyordu)*. **Ne kazandırıyor:** aylık KDV beyannamesi veren bir şahıs şirketi için ayın en zahmetli işini üstleniyor — kullanıcı matrahı ve tutarı elle toplamıyor, müşavire gidecek sayı hazır. Hedef kitlemiz tam olarak bu beyannameyi veren kitle. **Ne kaybettiriyor:** oran listesini (%20/%18/%10/%8/%1) üründe güncel tutma sorumluluğu doğuyor; oran değişince rapor sessizce eskir ve yanlış sayı kullanıcıyı yanıltır — mali sorumluluk da doğurabilir. Bizim ADR 0016 kararımız bu sorumluluğu **almamayı** seçti ve KDV'yi taşınan bir alan bıraktı. **Ödünleşim Belge 3'te tartışılacak; bu hücre hüküm vermez** |
| **Gelir/Gider Raporu** (`d28-destek-gelir-gider-raporu.png`) | Tarih aralığı; matris Gelirler/Giderler × dört para birimi + Yerel Toplam; alt sekmeler `Gelirler` / `Giderler`, kolonlar: İşlem Türü / Cari Adı / Gelir-Gider İşlem Türü / Belge No / Açıklama / İşlem Tarihi / Tutar / **Ödeme Yöntemi** / **İşleme Verilen Banka** | Rapor bu karede boş (19–26.09.2023); aynı aralıklı KDV karesinde ₺12.000 satış faturası görünüyor, faturaların bu rapora girip girmediği veya karelerin farklı veri anında çekildiği ayırt edilemez *(G02 düzeltmesi)*. Yapı olarak aylık raporumuzla karşılaştırılabilir — ama **kapsam boyutu yok**, ayrım para birimi ve belge türü üzerinden. `scopeBreakdown`'ımızın rakipte karşılığı yok |
| **Nakit Akış Raporu** (`d29-destek-nakit-akis-raporu.png`) | Dört kart: **Güncel Bakiye** ₺19.543,53 · **Tahsilatlar** ₺143.252,50 · **Ödemeler** ₺0,00 · **Tahmini Dönem Sonu Bakiyesi** ₺162.796,03. Altında vade matrisi: **Belirsiz / Geçmiş / Eylül 2023 … Ağustos 2024 / Toplam**. Not: *"Belgelerini görmek istediğiniz döneme tıklayıp, belgeleri görüntüleyebilirsiniz"* | Bizde karşılığı olmayan bir rapor. `Tahmini Dönem Sonu = Güncel Bakiye + Tahsilatlar − Ödemeler` (19.543,53 + 143.252,50 = 162.796,03 ✓ aritmetik tutuyor). Bizim planlanan görünümümüz kalemleri listeliyor, **tek bir tahmini bakiye sayısı üretmiyor**. **Ama bu karede bir uyarı var (12 Eyl):** ₺143.252,50'nin **tamamı `Belirsiz` sütununda**; Eylül 2023 – Ağustos 2024 arasındaki on iki ay kovasının hepsi boş. Yani bu demo verisinde rapor **zaman-fazlı bir projeksiyon değil**, "güncel bakiye + bütün açık alacak" toplamı. Vade girilmediği sürece (bkz. `d05-destek-yeni-gider-formu.png` varsayılanı "Vade Günü Girilmemiştir") ay kovaları dolmuyor ve "dönem sonu" ifadesi anlamını kaybediyor. Fikir güçlü, **ama gücü kullanıcının vade girme disiplinine bağlı** |

### Güncel Durum panosu ve diğerleri

- **Pano** (`d24-destek-guncel-durum-panosu.png`): sekmeler Güncel Durum / **Stok Akışı** / **Notlar**.
  "Son 1 Haftalık Nakit Akışı" **iki serili** (yeşil `TRY Gelir`, kırmızı
  `TRY Gider`); `d39-guncel-arayuz-2026.png`'in aksine **grafik burada dolu** (tooltip
  `5.1.2023 · TRY Gelir: 2 B ₺ · TRY Gider: 1 B ₺`). Altta "Tahsilat Ve Ödeme
  Özetleri". Sağda "KolayBi' Yolu Var!" ve "Günü Gelen İşlemler".
  **`d39-guncel-arayuz-2026.png` ile birebir aynı değil** (12 Eyl düzeltmesi): `d24-destek-guncel-durum-panosu.png`'ün promosyon
  şeridinde **yedi** kutucuk var (TotalEnergies, OYAK Grup Sigorta,
  DijitalKöprü, **hepsiburada**, KolayBi Banka, **ÇiçekSepeti**, **paynet**),
  `d39-guncel-arayuz-2026.png`'de dört; "KolayBi' Yolu Var!" listesinde `d24-destek-guncel-durum-panosu.png` **`BiLink`** derken `d39-guncel-arayuz-2026.png`
  **`Müşavirini Davet Et`** diyor; ve `d24-destek-guncel-durum-panosu.png`'ün sol menüsünde **`Fatura Ödeme`
  yok** — yani `d24-destek-guncel-durum-panosu.png`, `d39-guncel-arayuz-2026.png`'den **daha eski** bir sürüm olmalı (`çıkarım`: menü ve kampanya farkından; aradaki sürüm geçmişi görülmedi, grafik tarihi yayın tarihi değil). Değişmeyen şey
  **iskelet**: nakit akışı + Tahsilat/Ödeme Özetleri + Günü Gelen İşlemler.
  Şeridin içeriği ve sağ sütun kampanyaya göre değişen bir yüzey.
- **Notlar** (`d25-destek-notlar-hatirlatici.png`): `Yeni Not Ekle` — Başlık\* · Not\* · **Hatırlatıcı
  (Pasif/Aktif)** · **Görünürlük (Özel Not / Şirket Notu)**. Filtreler: Not
  Grubu, Hatırlatma/Oluşturma/Okunma Tarihi. Çok kullanıcılı ürün varsayımı
  (özel vs şirket notu); bizde hesap başına tek kullanıcı (paylaşılan şirket alanı yok), not özelliği
yok. Hatırlatıcının bildirim davranışı görülmedi.
- **Ürün ve Hizmetler** (`d31-destek-urun-varyantlar.png`): sekmeler Tümü / Ürünler / Hizmetler /
  **Depolar** / **Varyantlar**. `Varyantlar` sekmesi açık ama **ekran tamamen
  boş** — tek içerik `⊕ Yeni Varyant` düğmesi. Yani kanıt seviyesi **sekme ve
  düğme adı**; varyantın ne olduğu, nasıl tanımlandığı görülmedi
  (`çıkarım`: ürün varyantı + çoklu depo = hafif ERP). Kapsamımızın dışında.

### Faz 6'nın Tur 2 konularına katkısı

| Tur 2 konusu | Destek taramasından çıkan |
|---|---|
| 1. Kart borcu / kart ödemesi | **Var — önceki "yok" tespiti düzeltildi.** Finans modülünde ayrı `Kredi Kartları` sekmesi; kart kaydı kesim günü + son ödeme günü + limit + **minimum ödeme oranı** taşıyor (`d20-destek-kredi-kartlari-listesi.png`, `d21-destek-yeni-kredi-karti-formu.png`). Ödeme davranışı doğrulanmadı |
| 2. Tekrarlayan ödeme | **Kapandı (form seviyesinde).** Dört ayrı yerde: `Tekrarlı Genel Giderler`, `Tekrarlı Alış İşlemleri`, `Tekrarlı Satış İşlemleri`, `Tekrarlı Maaşlar`. Form alanları `d18-destek-tekrarli-maas-formu.png`'de görüldü: **Oluşturma Periyodu\*, Başlangıç Tarihi, Ödeme Tarihi (Belirsiz/Belirli), Tekrar Sayısı\*** — plan **sonlu**, kaç kez üreteceği zorunlu. Onay adımı görünmüyor (`çıkarım`: otomatik üretim) |
| 3. Fatura → tahsilat bağı | **Arayüz seviyesinde görüldü, bağ doğrulanmadı** *(G02 düzeltmesi)*: cari detayında iki ayrı düğme (`Borç/Alacak Ekle` vs `Ödeme/Tahsilat Ekle`), hareket tablosunda Borç/Alacak/yürüyen Bakiye |
| 4. Kısmi tahsilat | **Arayüz seviyesinde görüldü** (video kare d35 demo satırı + `d09-destek-cari-detay-ekstre-dialog.png` yürüyen bakiye kolonu); ödeme öncesi/sonrası görülmedi, bakiyeye etkisi doğrulanmadı *(G02 düzeltmesi)* |
| 5. Arama / filtre / dışa aktarma | **Kapandı**: her listede Ara + filtre + İçe/Dışarıya Aktar; ekstre PDF önizlemesi + e-posta düğmesi (gönderim yapılmadı); gider formunda 5 MB dosya eki |
| 6. Tam arayüz taraması | **Büyük ölçüde kapandı** — 12 modülün 117 mockup'ı tarandı: Güncel Durum, Satış, Satın Alma, Genel Gider, Ürün-Hizmetler, Depo, Cari, Finans, Projeler, Raporlar, Ek Özellikler. Açık kalan tek yer **Ayarlar** (destek sayfasında hiç mockup yok) ve her ekranın **davranışı** |

### Kaynak envanteri ve kalan boşluklar

Taranan modül sayfaları ve mockup sayıları: `satis-yonetimi` 29 ·
`kolaybi-finans` 16 · `kolaybi-urun-ve-hizmetler` 16 · `cari-hesaplar` 13 ·
`kolaybi-raporlar` 10 · `guncel-durum` 8 · `depo-takip` 7 · `ek-ozellikler` 7 ·
`projeler` 5 · `genel-gider-yonetimi` 4 · `satin-alma-yonetimi` 2 = **117**.
`destek/ayarlar` sayfasında mockup **yok**.

İndirme yöntemi (tekrarlanabilir): sayfanın HTML'inde
`src="…desteksayfası-mockuplar.webp"` kalıbı aranır, webp indirilip png'ye
çevrilir.

**Masa başı yöntemiyle kapanmayan iki şey:**

1. **Ayarlar ekranı** — destek sayfasında hiç görsel yok. Oysa iki kritik
   ayarın orada olduğunu biliyoruz: **Proje Takip aç/kapa** (gider ve fatura
   formlarındaki nottan) ve muhtemelen varsayılan kasa/para birimi.
2. **Davranış** — her ekranın *ne yapınca ne olduğu*. Özellikle: `Ödeme
   Durumu` radyosu işaretlenince kasa/banka bakiyesi değişiyor mu; `Tekrarlı`
   planın onay adımı var mı; kısmi tahsilatın bakiyeye etkisi.

**Kullanıcıdan beklenen görsel doğrulama** (video karesi gelirse öncelik
sırası): (1) `Ayarlar` sayfası, özellikle Proje Takip anahtarı; (2) `Tekrarlı
Genel Giderler` listesi ve bir planın ürettiği kayıt; (3) `Ödeme/Tahsilat Ekle`
akışının kısmi tutar davranışı; (4) `Ödeme Durumu: Ödendi` işaretlemenin
kasaya etkisi.

## Faz 7.5 doğrulama turu (12 Eylül 2026)

Bu tur formu baştan okudu ve **39 karenin hepsini tek tek açıp** metinle
karşılaştırdı. KolayBi sürülemeyen bir ürün olduğu için emülatör adımı yok;
`Doğrulanamadı` satırları yerinde kaldı ve genişletildi.

**Sayım ve etiket düzeltmeleri:**

- **"On bir rapor" → on rapor.** Form on bir diyordu, listesinde on kalem
  vardı; `d27-destek-kdv-raporu.png` üst barında raporların tamamı görünüyor ve sayı **on**.
- **`d24-destek-guncel-durum-panosu.png` ile `d39-guncel-arayuz-2026.png` "birebir aynı" değil.** Form destek panosu ile 2026 video
  karesini özdeş sayıyordu. Üç fark var: promosyon şeridi **yedi** vs
  **dört** kutucuk, sağ sütunda **`BiLink`** vs **`Müşavirini Davet Et`**, ve
  `d24-destek-guncel-durum-panosu.png`'ün sol menüsünde **`Fatura Ödeme` yok**. `d24-destek-guncel-durum-panosu.png`'ün `d39-guncel-arayuz-2026.png`'den daha
  eski olduğu bu farklardan çıkarımdır *(P1-B15)*. Değişmeyen şey iskelet; değişen şey kampanya yüzeyi.
- **Finans'a iki sekme eklenmiş, bir değil.** Form yalnız `Kredi Kartları`nın
  sonradan geldiğini yazıyordu; ~2020 karesinde (`d38-video-finans-kasalar.png`) dört sekme var, `d19-destek-banka-hesaplari.png`'da
  altı — **`Senetler` de yeni**.
- **`d14-destek-personel-carileri.png`'te kod öneki tek tip değil:** personel listesinde `EMP…` yanında iki
  satır `CAR00001` taşıyor.
- Ekran etiketleri ekrandaki hâline çekildi: `VKN/TCKN No`,
  `Telefon Numarası`, gider formunda **`Düzenleme Tarihi`** (liste ve fatura
  formunda `Düzenlenme Tarihi`).

**Kanıt seviyesi düzeltmeleri (gözlem sanılan çıkarımlar):**

- **`d20-destek-kredi-kartlari-listesi.png` `Kalan Limit`** — tablo boş ("Kayıt yok"); kolonun limit − borç
  hesapladığı **çıkarım**, hesaplanmış tek bir değer görülmedi.
- **`d23-destek-senetler.png` senet taksitleri** — tablo boş; "taksitli ve kısmi ödenebilir"
  yalnız `Ödenen Toplam` / `Taksit Durumu` / `Kalan Toplam` **kolon
  adlarından** okunuyor.
- **`d31-destek-urun-varyantlar.png` varyantlar** — sekme açık ama ekran tamamen boş; kanıt yalnız sekme
  ve `⊕ Yeni Varyant` düğmesinin adı.
- Sistem işleyişi tablosundaki `Yorum` etiketi `Çıkarım`'a çevrildi.

**Keskinleşen bulgular:**

- **`d11-destek-gider-listesi.png`'deki `Bakiye` kolonu muhtemelen "kalan" demek değil.** Form bunu
  "tutarsız demo verisi" diye geçmişti; oysa desen **sistematik**: `Ödendi`
  satırlarında Bakiye = Fatura Tutarı, `Ödenmedi` satırlarında ₺0 — "kalan
  borç" okumasının tam tersi. Kolonun **ödenen tutarı** göstermesi en tutarlı
  açıklama ama `Bedelsiz` satırı onu da tam oturtmuyor. Anlamı doğrulanmadı.
- **`d29-destek-nakit-akis-raporu.png`'un gücü kullanıcının vade disiplinine bağlı.** Form `Tahmini Dönem
  Sonu Bakiyesi`ni "Belge 3'ün en güçlü adayı" diye işaretlemişti. Karede
  tutarın **tamamı `Belirsiz` sütununda** ve on iki ay kovasının hepsi boş —
  yani rapor bu veride zaman-fazlı bir projeksiyon değil, "güncel bakiye +
  bütün açık alacak". Fikir hâlâ güçlü, ama karar satırına bu ödünleşim
  yazıldı.
- **`d30-destek-satis-fatura-formu.png` kare adı içeriğiyle çelişiyor:** dosya adı satış faturası diyor,
  karedeki başlık **`Yeni Alış İade Faturası`**, sekmeler Satış Yönetimi.
  Tutarsızlık KolayBi'nin kendi destek görselinde. Ayrıca satış/iade formunda
  **`Ödeme Durumu` radyosu yok** ve `Vade Tarihi` bir **Yok/Var radyosu** —
  formun "neredeyse aynı iskelet" iddiası bu üç farkla birlikte yazıldı.
- **Kontrol edilen üç destek karesinin aritmetiği tutuyor.** `d27-destek-kdv-raporu.png` (%20 × ₺12.000 =
  ₺2.400), `d26-destek-alis-satis-raporu.png` (20.000 − 12.000 = 8.000) ve `d29-destek-nakit-akis-raporu.png` (19.543,53 + 143.252,50 =
  162.796,03) kendi içinde doğru; ayrıca `d19-destek-banka-hesaplari.png`'daki banka bakiyesi `d29-destek-nakit-akis-raporu.png`'un
  `Güncel Bakiye` kartıyla birebir aynı. Paraşüt'ün animasyon karelerinde dört
  ayrı aritmetik hatası bulunmuştu. Bu yalnız kontrol edilen sayılar için
  geçerli bir farktır; kareler farklı tarih aralıklarından gelir, 39 karenin
  tamamının tutarlı veya aynı kiracı/an olduğu kanıtlanmaz *(P1-B15)*. Belge
  1/2'de kaynak gücü bu sınırla anlatılır.

**Eklenen küçük gözlemler:** `d04-destek-proje-belge-kirilimi.png`'te kart rengi belge ailesine değil **nakit
yönüne** bağlı (Satış İade kırmızı, Alış İade yeşil); `d12-destek-gider-detay-islemler.png`'de bölüm başlığı
**`GENEL GELİR/GİDER BİLGİLERİ`** — modül gider kadar geliri de taşıyor;
`d15-destek-personel-cari-detay.png`'te personel carisinde `Toplam Borç` yeşil / `Toplam Alacak` kırmızı
(renk firmanın pozisyonuna göre) ve üçüncü rozet serbest bir unvan etiketi;
`Proje Takip` uyarı notu **dört ayrı formda** birebir tekrarlıyor; `d11-destek-gider-listesi.png`'de
`Son Ödeme Tarihi` sekiz satırın hepsinde `-` — "Vadesi Belirsiz" kovasının
kaynağı ekranda doğrulandı; `d26-destek-alis-satis-raporu.png` ve `d27-destek-kdv-raporu.png`'de satır dökümüne inen
drill-down sekmeleri/genişleticileri var.

**Kare temizliği:** silinecek süreç karesi çıkmadı — 39 karenin tamamı bir
iddiayı destekliyor ve denetim ölü atıf/yetim kare bulmuyor. *(12 Eyl beyanı;
B17 mekanik kapısı nedeniyle güncel onay değildir — P1-B15.)*

### Eski (video) ve yeni (destek) karelerin örtüşme denetimi

Faz 7.5'in ilk geçişinde her kare **metinle** karşılaştırıldı ama kareler
**birbiriyle** karşılaştırılmadı. Kullanıcı uyarısı üzerine video ve mobil
kareler (E0180 mobil giriş, E0181–E0186 ~2020 videosu, E0187 ayrı 2026 videosu)
ile destek kareleri (E0188–E0218) tek tek karşılaştırıldı. **Dört eşleşme**
denetlendi: ikisi aynı ekranın farklı görünümü (pano d33/d24/d39, cari listesi
d36/d07), ikisi aynı modülün farklı sekmesi (d37/d31, d38/d19) *(P1-B15
düzeltmesi: önceki metin d32–d39'u tek 2020 grubu, dördünü aynı ekranın iki
sürümü sayıyordu)*:

| Eski kare | Karşılaştırılan yeni kare | Eski karenin benzersiz katkısı | Sonuç |
|---|---|---|---|
| `d33-video-guncel-durum-panosu.png` | `d24-destek-guncel-durum-panosu.png`, `d39-guncel-arayuz-2026.png` | **Tahsilat/Ödeme Özetleri donut'ları tutarlarıyla dolu** (₺0,00 / ₺801,70 / ₺322.835,33) — yeni karelerin ikisinde de bu blok ya kesik ya boş. Ayrıca **`Takvim` widget'ı** yalnız burada var; ~2020 karesinin 13 modüllü menüsü | **Kalır** — üçlü vade hâlinin tutar kanıtı yalnız bu karede |
| `d36-video-cari-hesaplar.png` | `d07-destek-cari-listesi.png` | Neredeyse hiçbiri: `d07-destek-cari-listesi.png` 5 sekme (4 değil), dolu tablo, `Telefon Numarası` kolonu, dört `Cari Tipi` değeri, `Detaylı Arama` ve 1000-kayıt notuyla **her eksende üstün**. Geriye kalan: **`Ortaklar` sekmesinin ~2020 videosunda da görüldüğü** | **Fazlalığa en yakın kare.** Silinmedi çünkü `Ortaklar` sekmesinin **yeni bir ekleme olmadığını**, eski görünümde de bulunduğunu gösteriyor; sekmenin patron parası için kullanıldığı çıkarımdır *(P1-B08)*; ürün başlangıcı ve işleyişi kanıtlanmıyor. Birincil atıf `d07-destek-cari-listesi.png`'ye devredildi |
| `d37-video-urun-ve-hizmetler.png` | `d31-destek-urun-varyantlar.png` | Farklı sekmeler: `d37-video-urun-ve-hizmetler.png` = `Tümü` (ürün tablosunun **`KDV` ve `İndirim` kolonları** — ADR 0016 karşılaştırmasının dayanağı), `d31-destek-urun-varyantlar.png` = `Varyantlar` (boş). Ayrıca `test-ofis.noffix.com` URL'i | **İkisi de kalır** — örtüşmüyorlar, tamamlıyorlar |
| `d38-video-finans-kasalar.png` | `d19-destek-banka-hesaplari.png` | ~2020 karesinde Finans'ın **dört sekmesi**. Bu kare olmadan "KolayBi'de kredi kartı yok" tespitinin **yanlış olduğu gösterilemez** — düzeltmenin öncesi-sonrası kanıtı | **Kalır, taşıyıcı kare.** Seçili sekmeler farklı (d38 Kasalar, d19 Banka Hesapları): aynı ekranın iki sürümü değil, Finans sekme çubuğunun karşılaştırması *(P1-kolaybi-G02 düzeltmesi, KB-Q03)* |

Eşleşmeye girmeyen üç kare (`d32-giris-ekrani.png` mobil giriş, `d34-video-gunu-gelen-islemler.png` açılmış "Günü Gelen İşlemler"
satırı, `d35-video-vadesi-belirsiz-tahsilatlar.png` dolu demo listesi) zaten benzersiz. `d39-guncel-arayuz-2026.png` pano eşleşmesinde yeni
taraftadır, ayrıca "örtüşmeyen" sayılmaz *(P1-B15)*.

**Sonuç: silinecek kare yok**, ama `d36-video-cari-hesaplar.png` artık birincil
kanıt değil, **tarihsel çapa** olarak konumlandırıldı. Bu bir seçim önerisidir;
hiçbir kare için silme veya arşive taşıma yetkisi değildir *(P1-B15)*.

### Kanıt atıfı borcu — `denetim.sh` bunu göremiyordu

Aynı geçişte ikinci bir eksik çıktı: **39 karenin 31'i kısa kodla anılıyordu**
(`` `d07-destek-cari-listesi.png` ``), tam dosya adıyla değil. `MANUEL-TEST-PROTOKOLU.md` yazım kuralı
2 tam dosya adı istiyor; dahası **altı yerde yasak aralık atıfı** vardı
(`` `d01-destek-proje-listesi.png`–`d31-destek-urun-varyantlar.png` ``, `` `d07-destek-cari-listesi.png`–`d10-destek-cari-ekstre-onizleme.png` ``, `` `d19-destek-banka-hesaplari.png`–`d23-destek-senetler.png` `` …).

`denetim.sh` bunların hiçbirini yakalamadı ve formu "temiz" gösterdi. Sebep
iki kör nokta:

- Yetim kare kontrolü (satır 44) `` `$n` `` kalıbını kabul ediyor; `dNN-…`
  karelerinde `$n` = `dNN` olduğu için kısa kod **geçerli atıf** sayılıyor.
- Belirsiz aralık kontrolü (satır 50) yalnız `` `NN`–`NN` `` ve
  `` `f7-NN`–`f7-NN` `` kalıplarını arıyor; `` `dNN`–`dNN` `` desenine
  bakmıyor.

**118 kısa kod atıfının tamamı tam dosya adına çevrildi**, altı aralık açık
listeye dönüştürüldü, bölüm başlıklarındaki dosya adı listeleri kaldırıldı
(alttaki tablolar tam adı taşıyor). **Script'in kör noktası kalan iki
uygulamayı da etkiliyor** — Bluecoins ve Wallet'ta aynı denetim yeşil
görünebilir; o tur başlamadan `denetim.sh` düzeltilmeli. *(14 Eyl 2026,
P1-K: düzeltildi. Yeni kapı Bluecoins'te 80, Wallet'ta 97 tam adsız kare ve
üçer belirsiz aralık gösteriyor; bu borç P2/P3 G paketlerinde kapanır.)*

## BusinessFinance için kararlar

Bu tablo Belge 3'ün ön değerlendirmesidir; kesin karar Belge 3'te. "Alma" bir
öneri kararıdır, gözlem etiketi değil — özellik Belge 1/2'de yine anlatılır.
**`kararı yeniden sor`**, rakibin yaklaşımının bir boyutta bizimkinden iyi
göründüğü ve ilgili ADR'nin ödünleşimiyle birlikte patrona taşınması gereken
bulguları işaretler (bkz. `README.md` → "Bu liste ölçüt değil"). Gerekçe
sütunu yalnız "ADR'miz böyle diyor" diyemez; rakibin çözümünün **ne
kazandırdığını ve ne kaybettirdiğini** söylemek zorundadır.

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| QR kod ile hızlı giriş | Henüz karar verme | İlginç ama bizim tek-cihaz/mobil-öncelikli senaryoda önceliği düşük | Auth |
| Proje bazlı gelir-gider takibi (ikinci bir raporlama boyutu) | Henüz karar verme | Bizim kapsam boyutuyla karşılaştırılabilir farklı bir eksen; ürün kapsamımızda proje yok. Rakipte ikinci boyutun bulunması ürün tasarım seçimidir, kullanıcıların birden çok raporlama boyutu talep ettiğinin kanıtı değildir *(P1-B08)* | Rapor / kapsam |
| "Ortaklar / Personel Carileri" sekmeleri; patron parasının bu yoldan izlendiği `çıkarım` *(P1-B08)* (`d07-destek-cari-listesi.png`; ~2020 videosunda da görüldü: `d36-video-cari-hesaplar.png`) | Alma | **Kazandırdığı:** sahibi ile şirketi iki taraf sayıp aradaki akışı borç-alacak yürütmek muhasebenin yerleşik cevabı; müşavir bu dili konuşuyor ve patronun şirketten çektiği para izlenebilir kalıyor. **Kaybettirdiği:** bu yol kullanılıyorsa gündelik kullanıcı için iki ayrı defter demek — "kendi paramı kendi işletmemden çektim" işlemi cari kaydı açmayı gerektiriyor. ADR 0013 ikinci maliyeti kabul etmemeyi seçti | Kapsam / cari |
| Üçlü vade hali: vadesi gelmemiş / geçmiş / **belirsiz** (kare d34–d35) | Henüz karar verme | "Belirsiz" (vade girilmemiş) bizde ayrı hal değil; upcoming-payments'a eklenebilir mi | upcoming-payments / attentionCode |
| "Günü Gelen İşlemler" panosu (Bugün/Yaklaşanlar/Tarihi Geçenler + tür kırılımı) | Uyarlayarak al | Bizim planlanan feed + özet ekranıyla örtüşüyor; tür kırılımı (`sourceGroup`) zaten var | Özet / planlanan feed |
| Ürün/hizmet kataloğu + KDV alanı | Alma | Kayıt adı bizde serbest metin; KDV taşınır, hesaplanmaz (ADR 0016) | — |
| Mobilde kayıt yok, yalnız giriş + QR | Alma | Bizde mobil ana istemci; kayıt mobilde olmalı | Onboarding |
| Canlı çok kullanıcılı muhasebeci erişimi | Alma (biçim) | Sahiplik izolasyonu + tek hesaplama yolu kuralı; bizde dosya tabanlı dışa aktarma paketi | Muhasebeci paketi |
| **Nakit Akış Raporu'nun `Tahmini Dönem Sonu Bakiyesi`** (`d29-destek-nakit-akis-raporu.png`) | Uyarlayarak al | **Kazandırdığı:** açık alacak ve borçtan ileriye dönük **tek bir bakiye sayısı** üretiyor; "ay sonunda kasamda ne olur" sorusu kalem listesi okumadan cevaplanıyor. Bizim planlanan görünümümüz kalemleri listeliyor, bu sayıyı üretmiyor. **Kaybettirdiği (12 Eyl'de kareden görüldü):** sayı yalnız vadeler girildiği ölçüde anlamlı — demo karesinde tutarın **tamamı `Belirsiz` kovasında** ve on iki ay kovası boş, yani rapor fiilen "güncel bakiye + bütün açık alacak" oluyor. Vade girmeyen kullanıcıya **yanlış bir kesinlik** duygusu veriyor. Uyarlanırsa vadesizlerin toplama nasıl katıldığı ekranda yazmalı | Planlanan görünüm / Özet |
| **"Tekrarlı …'a Dönüştür"** — mevcut kayıttan plan üretme (`d12-destek-gider-detay-islemler.png`, `d17-destek-calisan-maasi-detay.png`) | Uyarlayarak al | Bizde plan sıfırdan tanımlanır; "bu kaydı her ay tekrarla" yolu yok. Ürün genelinde tutarlı bir kalıp (gider + maaş) | Tekrarlayan plan / işlem detayı |
| Tekrarlayan planda **zorunlu tekrar sayısı** (`d18-destek-tekrarli-maas-formu.png`) | Henüz karar verme | **Kazandırdığı:** plan ne zaman biteceğini biliyor; süresiz plan unutulup yıllarca kayıt üretmiyor ve kullanıcı "kaç taksit kaldı" sorusunu plandan okuyabiliyor. **Kaybettirdiği:** abonelik/kira gibi gerçekten süresiz kalemlerde kullanıcıyı uydurma bir sayı girmeye zorluyor. Bizde bitiş tarihi ve toplam tekrar sınırı isteğe bağlı ve birlikte kullanılabilir; plan listesi üretilen/sınır sayısını gösterir. Fark yalnız KolayBi'nin sayıyı zorunlu tutması. *(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki gerekçe isteğe bağlı bitişin iki üründe de olmadığını söylüyordu.)* | Tekrarlayan plan |
| `Mahsuplaştır` — alacakla borcu netleştirme (`d09-destek-cari-detay-ekstre-dialog.png`) | Henüz karar verme | Aynı karşı tarafla iki yönlü ilişkide bizde netleştirme yok; `CounterpartyBalance` zaten işaretli tek sayı ürettiği için ihtiyaç sınırlı olabilir | Cari |
| **Cari ekstresi PDF + `E-Posta ile Gönder`** (`d09-destek-cari-detay-ekstre-dialog.png`, `d10-destek-cari-ekstre-onizleme.png`) | Henüz karar verme | "Karşı tarafa hesap özeti gönderme" işlevi bizde yok (ihtiyacın büyüklüğü ölçülmedi); muhasebeci paketi farklı bir amaç | Cari / dışa aktarma |
| Proje ekranının `Kar / Zarar` + `Nakit Durumu` iki sekmesi (`d03-destek-proje-detay-ozet.png`) | **Kararı yeniden sor** | **Kazandırdığı:** "kazandım ama tahsil etmedim" ile "param arttı" ayrı ayrı okunuyor — veresiye çalışan esnafın en çok karıştırdığı iki sayı, ve `Tahsil Edilen / Bekleyen` kırılımı bunu tek kartta veriyor. **Kaybettirdiği:** iki sayı iki kavram; hangisinin "işin durumu" olduğunu kullanıcı seçmek zorunda ve tahakkuk tarafı "kâr" kelimesini gerektiriyor. Bizde **işletme neti** veresiye satışı olay gününde gelire katıyor ve tahsil edilmemiş kısım cari bakiyede ayrı görünüyor; ama aynı kartta tanınan ile tahsil edileni ayırmıyor. Hedef kitle veresiye satıyorsa bu iki sayının aynı kartta gösterilmesi gerekip gerekmediği **açık bir soru** *(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki gerekçe netimizi "yalnız nakit esaslı" sayıyordu)* | Rapor / özet |
| **KDV Raporu** — oran/belge türüne göre matrah ve tutar matrisi (`d27-destek-kdv-raporu.png`) | **Kararı yeniden sor** | **Kazandırdığı:** girilmiş KDV bilgisini oran ve belge türüne göre inceleme yüzeyi. **Kaybettirdiği:** daha yoğun bir rapor ve alanların anlamını açık anlatma ihtiyacı. **P4-tema-08 düzeltmesi (15 Eyl):** karede alt toplam ve İndirilecek etiketi görünmüyor; KDV tutarının orandan türetildiği kanıtlanmadı. BF `AccountantPackageUseCases` zaten girilmiş `VatAmount` değerlerini gelir/gider bazında topluyor ve eksik KDV satırlarını sayıyor; “yalnız toplama orta yolu tartışılmadı” öncülü yanlıştı. Açık soru, mevcut özeti oran/belge kırılımıyla sunma ihtiyacıdır. Orandan vergi türetme ve beyanname üretme bununla aynı karar değildir; ADR 0016 yürürlüktedir | Rapor / muhasebeci paketi |
| Kredi kartında **Minimum Ödeme Oranı (%)** (`d21-destek-yeni-kredi-karti-formu.png`) | Doğrudan al | BusinessFinance'te zaten var: `CreditCard` asgari ödeme oranını taşır (varsayılan %20), asgari tutarı ekstreden hesaplar ve istemci "Asgariyi öde" sunar; yeni özellik değildir. *(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki karar Henüz karar verme, gerekçe modelimizde olmadığıydı.)* | Kredi kartı |
| `Taslağa Al` / `Kopyala` (`d12-destek-gider-detay-islemler.png`, `d17-destek-calisan-maasi-detay.png`) | Henüz karar verme | Taslak kavramı bizde yok (MM ve Bluecoins'te de yoktu). `Kopyala` planlanamayan ama tekrar eden kalemler için ucuz çözüm | İşlem formu |
| İki seviyeli kategori (Kategori → Tip) (`d06-destek-gider-tipleri.png`) | Henüz karar verme | Bizim kategori modelimiz tek seviyeli; alt kategori yaklaşımının rakipteki örneği; talep kanıtı değil *(P1-B08)* | Kategori |
| **Kullanıcı tanımlı proje ekseni** (`d01-destek-proje-listesi.png`, `d02-destek-yeni-proje-formu.png`) | Henüz karar verme | Demo adları ve proje tanımlama alanları gözlendi. Esnek gruplama olasılığı bir hipotezdir; gerçek hane ihtiyacı, amaç dışı kullanım veya sınırsız proje sayısı kanıtlanmadı. Sabit kapsam boyutuyla kıyas Belge 3'te bu sınırla yapılır | Kapsam / rapor |
| `Şube` — dördüncü raporlama boyutu (`d26-destek-alis-satis-raporu.png`) | Alma | **Kazandırdığı:** çok şubeli işletme her şubeyi ayrı okuyabiliyor. **Kaybettirdiği:** dört boyut (kapsam yok ama proje + etiket + şube + kategori) her formu uzatıyor ve tek şubeli esnaf için hepsi boş geçilen alan. Hedef kitlemiz tek noktada çalışıyor | Rapor |

## Kanıt ve güven düzeyi

- Manuel gözlem: Yalnız giriş ekranı (`kanitlar/kolaybi/d32-giris-ekrani.png`).
  Kullanıcı doğruladı: mobilde kayıt yok
- Resmî kaynak (tanıtım videosu): `kanitlar/kolaybi/d33-video-guncel-durum-panosu.png`
  … `d38-video-finans-kasalar.png` — "KolayBi' Nedir, Neden Kullanmalıyım?"
  (Dijital Köprü Akademi, YouTube `nn4-hShn_kw`, ~4:19; ~2020 içerik işaretlerinden çıkarım, yayın tarihi kaynakta yok).
  Transkript kullanıcı tarafından araçla çıkarıldı ve doğrulandı (10 Eyl 2026)
- Resmî kaynak (daha yeni video): `d39-guncel-arayuz-2026.png` — 2026 başında
  yüklendiği kullanıcı tarafından aktarılan başka bir videodan tek kare; "KolayBi
  Ofis" arayüzü, bugünkü sürüm olduğu doğrulanmadı *(P1-B15)*
- Resmî kaynak: kolaybi.com ana sayfa + özellik listesi (2 Eyl 2026); kullanıcı
  kılavuzu — modül yapısı, proje bazlı gelir-gider, tekrarlayan işlem (9 Eyl 2026)
- **Resmî kaynak (destek dokümanı, 12 Eyl 2026):** `kolaybi.com/destek/*`
  altındaki **12 modül sayfası**, toplam **117 resmî mockup**; ayırt edici 31'i
  `kanitlar/kolaybi/d01`–`d31-destek-urun-varyantlar.png`. Mockup'lardaki işlem tarihleri 2023; arayüzün
  video karelerinden yeni olduğu menü/sekme farkından çıkarım, üretim tarihi
  bilinmiyor *(P1-B15)*. Sayfa metni sığ, **bilgi görsellerde**. Bu tarama ~2020
  videosundan gelen
  "kredi kartı yok" tespitini **düzeltti**
- **Kareler demo/eğitim kiracısından** (`test-ofis.noffix.com`, "KolayBi'
  Eğitim"): tablolar boş veya tek demo kayıtlı. Alan/sekme adları güvenilir,
  **davranış okunamaz**; tek dolu ekran kare d35'tir. Bu yüzden formdaki
  davranış cümleleri `çıkarım` etiketli
- **Doğrulanamadı (12 Eyl'de güncellendi):** canlı ürün **davranışının
  tamamı** — hiçbir alanın neyi tetiklediği görülmedi. Özellikle:
  `Ödeme Durumu: Ödendi` işaretlemenin kasa/banka bakiyesine etkisi;
  `Tekrarlı …` planlarının onay adımı olup olmadığı; kısmi tahsilatın
  bakiyeye etkisi; `d11-destek-gider-listesi.png`'deki `Bakiye` kolonunun anlamı; `Kalan Limit` ve
  senet `Ödenen/Kalan` kolonlarının hesabı; `Varyantlar`ın ne olduğu.
  Ayrıca **`Ayarlar` ekranı** (destek sayfasında hiç mockup yok — `Proje
  Takip` anahtarının yeri) ve **güncel mobil arayüz**.
  *(Faz 6 taraması proje ekranı, gider formu ve cari ekstre boşluklarını
  kapattı; bu üçü artık `Doğrulanamadı` değil.)*
- **Kullanıcıdan bekleniyor (opsiyonel, öncelik sırasıyla):** Faz 6 destek
  taraması planlanan boşlukları kapattığı için bu artık bir tıkanma değil.
  Video karesi gelirse sırasıyla: (1) `Ayarlar` sayfası, özellikle **Proje
  Takip** anahtarı; (2) `Tekrarlı Genel Giderler` listesi ve bir planın
  ürettiği kayıt; (3) `Ödeme/Tahsilat Ekle` akışının kısmi tutar davranışı;
  (4) `Ödeme Durumu: Ödendi` işaretlemenin kasaya etkisi

## Tek cümlelik sonuç

KolayBi, şahıs şirketini de hedefleyen geniş kapsamlı bir bulut ön muhasebe +
e-dönüşüm ürünü; tahakkuk ile nakdi her ekranda ayırması (`Ödeme Durumu`
radyosu, projede `Tahsil Edilen / Bekleyen`, cariden `Borç-Alacak Ekle` ile
`Ödeme/Tahsilat Ekle`nin ayrı düğmeler olması) ADR 0014'ün rakipteki en olgun
karşılığı ve `Nakit Akış Raporu`nun **tahmini dönem sonu bakiyesi** bizde hiç
yok — işletme/şahsi tek havuz görülmedi; yerine ortak carisi ve kullanıcı
tanımlı proje ekseni gibi araçlar sunuyor. Bu araçların patron parası veya
hane gideri için kullanıldığı çıkarımdır, kullanıcı davranışı gözlenmedi;
kapsam boyutumuzla karşılaştırması ve hedef kitle için hangisinin daha iyi
olduğu Belge 3'ün sorusudur *(P1-B08 düzeltmesi, 14 Eyl 2026)*.

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

---

## Faz 8 — hedefli kaynak taraması (masa başı, 22 Eylül 2026)

Belge 2 Bölüm 9 yazılırken açık kalan sorular için resmî kaynaklar tarandı.
Hepsi `Resmî kaynak`; ürünün içi yine görülmedi.

### KDV oran mı, tutar mı — API dokümanı cevap veriyor

Destek metni KDV raporunu tek cümleyle tanımlıyor: *"KDV raporunda
faturalarınızda kullanmış olduğunuz KDV oranlarına göre hesaplanan ve
indirilecek KDV toplamı bilgilerini detaylı olarak görüntüleyebilirsiniz."*
(`kolaybi.com/destek/kolaybi-raporlar`).

Asıl cevap geliştirici portalında:

| Kayıt tipi | KDV nasıl giriliyor | Kaynak |
|---|---|---|
| **Fatura kalemi** | `items[][vat_rate]`, tip `VATRate`, **zorunlu**. Kalem düzeyinde KDV **tutarı** alanı tanımlı değil | `developer.kolaybi.com/docs/invoices/create/` |
| **Genel gider** | `vat_type` = `percentage` **veya** `numeric`; `vat_value` orana ya da tutara karşılık geliyor; ayrıca `total_vat` ile tutar doğrudan verilebiliyor | aynı sayfa |
| **Ürün kartı** | `vat_type` = `PERCENTAGE`/`NUMERIC`, `vat_value` = KDV oranı | `developer.kolaybi.com/docs/products/list/` |

`çıkarım`: Üründe **tek bir kural yok**. Ürün/hizmet kalemi taşıyan faturada
KDV orandan türetiliyor (oran zorunlu, tutar alanı yok); genel giderde tutar
doğrudan girilebiliyor. Dayanağı: iki alan setinin aynı API'de birlikte
bulunması.

**Açık kalan:** raporun `KDV Tutarı` kolonu genel gider satırlarında *girilen
tutarı* mı taşıyor yoksa yeniden mi hesaplıyor — kaynaklardan çıkmıyor, canlı
erişim gerekir. `VATRate` enum'unun izin verdiği oran listesi de yayımlanmamış.
Raporun ikinci (indirilecek) bloğunun toplam satırı destek görselinde kesiliyor.

### Depolar ve Varyantlar — boş görülen ekranların karşılığı

`d31-destek-urun-varyantlar.png` boş bir Varyantlar sekmesi gösteriyordu;
destek metni yapıyı anlatıyor (`kolaybi.com/destek/depo-takip`,
`kolaybi.com/destek/kolaybi-urun-ve-hizmetler`):

- **Ana Depo (varsayılan)** hiç depo oluşturulmasa da geliyor; yeni depo
  `Depo Oluştur` ile açılıyor, depolar Excel ile içe/dışa aktarılıyor.
- Varyant tanımı kaynağın kendi cümlesi: *"aynı temelde aynı ürünün farklı
  renk ve biçimlerini belirlemek için kullanılan ifade"*, *"özellikle tekstil
  sektöründe Renk ve Beden takibi amaçlı"*.
- Kurulum sırası: `Ayarlar`'da **ürün varyant kullanımı** açılır → `Varyantlar`
  sekmesinde varyant kategorileri ve değerleri tanımlanır → ürün üç aşamalı
  oluşturulur (`Bilgiler` / `Fiyat` / `Varyant-Stok Bilgileri`) → varyant
  değerleri **en fazla 4 adet** seçilip `Varyantları Oluştur`.
- Stok birimi ürün değil: *"Varyanta ait stok ve depo bilgilerini 'Yeni Stok'a
  tıklayarak oluşturabilirsiniz. **Stok adı, stoğa ilişkin depo seçimi,
  başlangıç stok miktarı, raf kodu**…"*; varyant kartının `İşlemler`
  menüsünden **stok giriş/çıkış ve transfer** yapılıyor.
- Kısıt: *"Ürün Birleştirme işlemi yalnızca **varyant kullanımı olmayan** ve
  aynı birimdeki ürünler için yapılmaktadır… geri alınamayan bir işlemdir."*
- `Ürün Hareketleri` tablosunun kolonları: İşlem Tarihi, Seri No, Hareket,
  Cari Adı, Ürün Satır Açıklaması, **Stok/Depo**, Değişim Miktarı, Kalan Stok
  Miktarı, Kritik Miktar, Birim Fiyat, Toplam Tutar, Toplam Giriş/Çıkış
  Bakiye, Ortalama Birim Maliyet.

`çıkarım`: Stok birimi ürün değil, **varyant × depo çifti**. Dayanağı: "Yeni
Stok" adımının depo ve raf kodu istemesi + hareket tablosunun `Stok/Depo`
kolonu taşıması.

**Ne kazandırıyor:** renk/beden kırılımı olan işte tek ürün kartı altında çok
depolu gerçek stok, depolar arası transfer, raf kodu.
**Ne kaybettiriyor:** varyant önce `Ayarlar`'dan açılmak zorunda, en fazla dört
varyant türü, varyantlı ürün birleştirmenin dışında kalıyor — kurulum sırası
yanlışsa geri dönüş maliyetli.

### Stok raporu

*"Stok raporunda faturada kullanılan ürünlerin stok bazlı giriş çıkışlarını
detaylı olarak görüntüleyebilirsiniz."* Filtrelerde **`Depo *` zorunlu**
görünüyor. Kolonlar maliyet ve kâr taşıyor: Giriş, Çıkış, Toplam Alış/Satış
Tutarı, Kalan Stok Miktarı, Ortalama Birim Maliyet, Ortalama Kâr/Zarar.
API'de ürünün `total_stock_quantity` alanı için not: *"negatif olabilir"*.

### e-Belge ayrı bir kalem

Menüde e-Fatura, e-Arşiv, e-İrsaliye, e-İhracat, e-SMM, e-İmza var; ayrıca
yalnız e-fatura için **KolayBi Jet** adında ayrı bir ürün. Fiyatlandırma
**kontör** üzerinden (300 / 100 / "Sınırsız e-Fatura Kontörü"). Faturada kayıt
ile gönderim iki ayrı düğme: *"Kaydet"* veya *"Kaydet ve GİB'e Gönder"*; senaryo
ve fatura tipi kullanıcı tarafından seçiliyor (`kolaybi.com/fiyatlar`,
`kolaybi.com/destek/satis-yonetimi`).

### Ortaklar sekmesi — cari kartının alt türü (WB-01)

`Ortaklar`, **Cari Hesaplar** modülünün beşli sekme şeridinde duruyor: `Genel Cariler ·
Potansiyel Müşteriler · Personel Carileri · Ortaklar · Tekrarlı Maaşlar`. Resmî metin:
*"Şirketinize ait ortakları cari oluşturma adımına benzer şekilde oluşturunuz … Daha sonra
bu ortağınıza ilişkin maaş ödemesi, prim ödemesi vb. işlemler yapabilirsiniz."* Yani ayrı
bir modül değil, **cari kartının bir alt türü**. Ortak ile şirket arasındaki para, sıradan
bir karşı tarafla olduğu gibi borç-alacak olarak yürüyor (`çıkarım`; P1-B08'deki "patron
parası bu sekmeden giriyor" çıkarımının kaynak dayanağı). Ortaklar sekmesinin kendi ekran
görseli destek sayfasında yayımlanmamış: paragrafın altındaki görselde `Personel Carileri`
aktif ve `carihesaplar-11` numaralı görsel atlanmış. Kaynak: `kolaybi.com/destek/cari-hesaplar`.
Belge 2'de kullanılmadı; Belge 1 turunda (B1 7.4) değerlendirilir. `Resmî kaynak`
