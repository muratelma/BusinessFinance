# Uygulama Gözlem Formu — Logo İşbaşı

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Logo İşbaşı / Logo Yazılım |
| Sürüm | 3.20.0 |
| Test tarihi | 1 Eylül 2026 |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | Arayüz İngilizce etiketli (Login/Register/e-mail), içerik Türkçe |
| Hesap veya plan türü | — |
| Erişim kısıtı | **Kayıt engeli — iki katmanlı.** (1) Kayıt formu telefon + SMS doğrulaması istiyor, sahte numara sessizce reddediliyor. (2) 2 Eyl 2026: kullanıcı gerçek bilgiyle kayıt oldu; kayıttan sonra **"Hesabınız firmanıza özel hazırlanmaktadır, işlemler tamamlandığında e-posta ile bilgilendirileceksiniz"** pop-up'ı çıkıyor — hesap satış/hazırlık sürecine giriyor, uygulamaya giriş açılmadı. **Bu çalışmada uygulamaya girilemedi; inceleme tümüyle resmî kaynak.** |
| İnceleme türü | **Resmî kaynak** (giriş/kayıt ekranları + kullanıcının izlediği tanıtım videosu 2 kare manuel gözlem) |

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | **Firma bilgisi tanımlamadan cep telefonundan fatura kesmeye** başlatan, e-Fatura + ön muhasebe + stok + cari birleşik bulut programı |
| Asıl hedef kullanıcı | Mikro işletme, tek kişilik girişimci, esnaf, serbest meslek — kayıt sektör listesinde **"Kurye" ve "Öğrenci"** bile var (mikro/bireysel vurgu) |
| Çözdüğü ana iş | e-Fatura/e-Arşiv kesme + gelir-gider + cari + kasa-banka + stok + çek-senet; hepsi tek panelde |
| Açıkça kapsam dışı bıraktığı | Kişisel/gündelik bütçe, yatırım takibi; bordro sınırlı |
| İş modeli | Ücretli abonelik: Ön Muhasebe 463₺/ay, e-Dönüşüm 463₺/ay, birleşik 738₺/ay (+1 yıl e-imza). Kayıt sonrası satış/hazırlık süreci |
| BusinessFinance ile aynı kulvarda mı | **Kısmen.** Gelir-gider + cari + fiş okuma + tekrarlayan ortak; ama Logo İşbaşı **e-belge + firma defteri + hafif ERP** (stok, sipariş). İşletme/şahsi tek havuz kavramı yok |

## Görev gözlemleri

| Görev | Sonuç | Not |
|---|---|---|
| K00 İlk açılış ve kayıt | Kısmi (kayıt öncesi) | Giriş ekranı: e-mail + parola + **Google/Apple SSO** + Register + Forgot Password. Kayıt formu **3 alan**: Company name*, Sector* (açılır liste), Phone Number* + ToS onayı. **VKN/TCKN sorulmuyor.** Telefon alanı otomatik `0090` ön eki ekliyor |
| K00 (sahte numara denemesi) | Engelli | `Deniz Tasarim` / `Hizmet Sektörü` / sahte numara girildi → "LOGO Bulut Hizmetleri Çerçeve Sözleşmesi" tam metni → I Agree → ~10 sn spinner → **sessizce giriş ekranına dönüyor** (hata mesajı yok, hesap oluşmuyor). Gerçek telefon + SMS kodu şart |
| K00 (gerçek kayıt, 2 Eyl) | Engelli | Kullanıcı gerçek telefon + SMS ile kayıt oldu; "hesabınız hazırlanıyor, e-posta ile bilgilendirileceksiniz" pop-up'ı → uygulamaya giriş açılmadı. Ayrıca e-posta ile ~1 dk'lık tanıtım videosu geldi (özellikler resmî kaynak olarak buradan çıkarılacak) |
| K01–K08 | **Engelli** | Doğrulanmış hesap olmadan devam edilemiyor; özellikler resmî kaynak (tanıtım videosu + logo.com.tr) ile |

**Sektör listesi (K00 arayüz bulgusu):** Ticaret ve Perakende / İmalat ve Üretim /
Kurye / Öğrenci / İnşaat ve Taahhüt / Hizmet Sektörü / Sağlık ve Medikal /
Bilişim ve Teknoloji / Eğitim ve Danışmanlık / Otelcilik, Restoran ve Kafe …
"Kurye" ve "Öğrenci" seçenekleri hedef kitlenin mikro/bireysel olduğunu gösteriyor.

## Arayüz incelemesi (giriş öncesi, sınırlı)

| Başlık | Kısa gözlem |
|---|---|
| Giriş ekranı | Bulut/gökyüzü arka planı, kırmızı (Logo marka) vurgu; çay bardağı ikonu (yerel dokunuş) |
| Kayıt sürtünmesi | 3 alan az görünüyor ama arkasında SMS doğrulama + tam sözleşme metni onayı var |
| Hata geri bildirimi | **Zayıf** — başarısız kayıt hiçbir mesaj vermeden giriş ekranına atıyor |
| SSO | Google + Apple ile giriş var (Paraşüt'te yoktu) |

## Resmî kaynak — özellikler (isbasi.com, 2 Eyl 2026)

Tanıtım videosu (YouTube `yVPlmkgHHmc`): transkript/altyazı çekilemedi; içerik
isbasi.com ana sayfasıyla örtüşüyor. Aşağıdakiler `Resmî kaynak` etiketlidir,
manuel gözlem değildir.

| Grup | Özellik | BusinessFinance karşılığı / not |
|---|---|---|
| Fatura/e-belge | Online fatura kesme, e-Fatura, e-Arşiv, e-SMM, e-İrsaliye; **sesli komutla fatura** | Bizde e-belge **kapsam dışı** (PRD). Fatura yerine `Obligation`/`CounterpartyCharge` tanıma modeli var |
| Finans takibi | Gelir-gider takibi, cari hesap takibi, tahsilat ve ödeme takibi, nakit (kasa-banka) yönetimi, çek giriş/çıkış | Cari + tahsilat/ödeme bizde ADR 0014 ile var; çek takibi yok |
| Operasyon | Stok takibi (otomatik güncelleme), teklif ve sipariş oluşturma, özet raporlar | Stok ve sipariş **kapsam dışı** — biz finans uygulamasıyız, ERP değil |
| Entegrasyon | 17 banka entegrasyonu, **akıllı fiş okuma (OCR)**, e-ticaret, online tahsilat, CRM | Banka bağlantısı bizde **kesin kapsam dışı**. Fiş okuma bizde öneri katmanı (ADR 0011) |
| Hedef kitle | Mikro işletme: tek kişilik girişimci ve küçük ekipler | Bizimkiyle aynı segment (şahıs şirketi, esnaf) |
| Fiyat | 3 paket, 463₺/ay+KDV'den başlıyor; en popüler "e-Dönüşüm + Ön Muhasebe" 738₺/ay; yıllık %25 indirim (resmî kaynak) | Ücretli ürün; kayıttan sonra "hesabınız hazırlanıyor" satış/hazırlık sürecine giriyor, uygulamaya anında giriş açılmıyor — bu çalışmada uygulamaya girilemedi, inceleme resmî kaynakla |

### Tanıtım videosundan (`yVPlmkgHHmc`, kullanıcı izledi 2 Eyl 2026)

Video anlatımı: işletme sahipleri için tasarlanmış; ana amaç **e-fatura + ön
muhasebe**. İnternet olan her yerden e-fatura kesme, müşteri/tedarikçi borç-alacak
(cari) takibi, anlık stok takibi. Sonunda deneme çağrısı → **ücretli ürün**.

**`05-video-entegrasyonlar.png` — "Entegrasyonlarımız ile verimliliğinizi artırın"**
sekiz entegrasyon kutucuğu:

| Entegrasyon | Not (BusinessFinance açısından) |
|---|---|
| Pazaryeri ve e-Ticaret | Kapsam dışı |
| GİB e-Arşiv Portal | Kapsam dışı (e-belge yok) |
| Banka Hesap Hareketleri | **Kesin kapsam dışı** — banka bağlantısı bizde yasak |
| Akıllı Fiş Okuma | Bizde öneri katmanı var (ADR 0011); onlar entegrasyon diyor |
| Online Tahsilat | Ödeme başlatma bizde kapsam dışı |
| **Müşavir Portal** | Muhasebeci tarafı — bizim "muhasebeci paketi" (Aşama 05) ile aynı ihtiyaç, farklı çözüm (bkz. `06-video-musavir-portal.png`) |
| İşbaşı POS | POS tahsilat — bizde `PosSettlement` var (ADR 0015) |
| Kargo Yönetimi | Kapsam dışı |

**`06-video-musavir-portal.png` — "Alış-Satış Faturalarınızı, Gider Fişlerinizi
Muhasebecinize Kolayca Aktarın"**: ayrı bir **Müşavir Portal** web arayüzü;
muhasebeci kendi ekranından müşterinin fatura/fiş satırlarını liste hâlinde
görüyor, satır başına düzenle / detay / sil ikonları var. Yani muhasebeci
müşterinin verisine **canlı portal** üzerinden erişiyor.

- **BusinessFinance farkı:** bizde muhasebeciye veri aktarımı tek yönlü, dosya
  tabanlı bir **dışa aktarma paketidir** (`/api/v1/exports/accountant-package.zip`,
  Aşama 05 Grup 5) — ikinci bir hesaplama yolu değil, o ayın işletme raporunun
  okuması. Logo'nun canlı çok-taraflı portalına karşılık bizim yaklaşımımız daha
  dar ama sahiplik izolasyonunu bozmuyor. Portal ihtiyacının gerçek olduğu
  doğrulandı; çözüm biçimi bilinçli olarak farklı.

**Karşılaştırma notu:** Logo İşbaşı bir **ön muhasebe + e-dönüşüm** ürünü;
kapsamı bizden geniş (stok, sipariş, e-fatura, banka) ama **işletme/şahsi tek
havuz** gibi bir kavramı yok — klasik "işletme defteri" yaklaşımı. Bizim
farkımız yine kapsam boyutu ve gündelik hayat + işletmeyi tek üründe tutmak.

## Sistem işleyişi / pipeline

`Resmî kaynak` — isbasi.com + logo.com.tr blog (9 Eyl 2026). Canlı ürün
davranışı doğrulanmadı.

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Kayıt mekanizması | Gelir ve gider **fatura ve fişlerle** kaydediliyor. Türkiye'de ilk: **sesli komutla** işlem girişi (elle yazmak yerine) | Resmî kaynak |
| Kayıt → entegre güncelleme | Bir işlem kaydedilince otomatik güncelleniyor: **cari hesap** (borç/alacak), **kasa-banka bakiyeleri**, **stok seviyeleri** (alış/satışa göre). Üç defter tek kayıttan besleniyor | Resmî kaynak |
| Fatura kesme akışı | Satış faturası ekranı: müşteri listeden seç → **bakiyesi anında görünür** → ürün listeden seç (veya ekranda yeni ürün bilgisi gir) → kaydet. Firma bilgisi tanımlamadan da kesilebiliyor | Resmî kaynak |
| Gider akışı | Gider fişi fotoğrafı → **Akıllı Fiş Okuma (OCR)** → otomatik gider kaydı | Resmî kaynak |
| Cari hesap | Tüm tahsilat, borç-alacak ve ödeme işlemleri cari hesap içinde; "dilediğiniz yerden" eklenebiliyor, cari içinde kontrol ediliyor. Ekstre + rapor | Resmî kaynak |
| Tahsilat/ödeme | Nakit tahsilat ve banka hesap bilgisi telefondan eklenebiliyor; cari bakiyeyi kapatır, kasa/banka bakiyesini oynatır | Resmî kaynak |
| Raporlama | Tarih aralığına göre kategorize raporlar + görsel grafik; firma performansı | Resmî kaynak |
| İşletme/şahsi ayrım | **Yok.** Klasik firma defteri; patronun şahsi harcaması ancak dolaylı (ortak cari / çekilen para) | Resmî kaynak + Yorum |
| Entegrasyon temas noktaları | 17 banka hesap hareketi, GİB e-Arşiv Portal, pazaryeri/e-ticaret, Online Tahsilat, İşbaşı POS, Kargo, **Müşavir Portal** (muhasebeci canlı erişim), CRM | Resmî kaynak (`kanitlar/logo-isbasi/05`) |
| Muhasebeci tarafı | **Müşavir Portal** — muhasebeci kendi ekranından müşterinin fatura/fiş satırlarını liste hâlinde görüyor, satır başına düzenle/detay/sil. Canlı çok-taraflı erişim | Manuel gözlem (video karesi `06`) |
| Veri nereye yazılıyor | Bulut (web + mobil), gerçek zamanlı senkron, "her alandan erişim" | Resmî kaynak |

**Pipeline şeması (satış → tahsilat):**
`müşteri seç (bakiye görünür) → ürün/kalem seç → fatura kaydet (e-Fatura/e-Arşiv) →
cari borç +tutar, stok −adet → Tahsilat Ekle (nakit/banka) → cari kapanır,
kasa/banka +tutar → rapor + grafik güncellenir → (Müşavir Portal muhasebeciye canlı yansır)`

**Pipeline şeması (gider):**
`fiş fotoğrafı → Akıllı Fiş Okuma (OCR) → gider kaydı (tür/kategori) →
gider raporu +tutar, tedarikçi cari +tutar → Ödeme Ekle → kasa/banka −tutar`

## Video/doküman akış yeniden kurulumu

Kullanıcı 2 Eyl 2026'da e-postayla gelen ~1 dk'lık tanıtım videosunu izledi;
2 kare teslim edildi (`kanitlar/logo-isbasi/05-video-entegrasyonlar.png`,
`06-video-musavir-portal.png` — yukarıda "Tanıtım videosundan" bölümünde işlendi).

| Akış | Kaynak | Adımlar (yeniden kurulmuş) | BusinessFinance karşılığı |
|---|---|---|---|
| Entegrasyon panosu | Video karesi `05` | 8 entegrasyon kutucuğu tek ekranda (pazaryeri, GİB, banka, fiş okuma, online tahsilat, Müşavir Portal, POS, kargo) | Çoğu kapsam dışı; fiş okuma + POS bizde var |
| Muhasebeci aktarımı | Video karesi `06` | Müşavir Portal: muhasebeci müşterinin fatura/fiş satırlarını listeler, satır başına düzenle/detay/sil | Bizde tek yönlü dosya paketi (`accountant-package.zip`), canlı portal değil |
| e-Fatura kesme, cep telefonundan tahsilat | isbasi.com kullanım videoları (kullanıcı notu bekleniyor) | — | Video notu gelince doldurulacak |

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| Kayıtta sadece 3 alan (şirket adı, sektör, telefon), VKN yok | Uyarlayarak al | Düşük sürtünmeli kayıt iyi; bizde de onboarding tek soru (`HasBusiness`). Sektör sormak bize gerekmiyor | Kayıt/onboarding |
| Başarısız işlemde sessiz geri dönüş, hata mesajı yok | Alma | Bizim proje kuralı: her ekranda görünür error durumu | Tüm formlar |
| Müşavir Portal: muhasebeci müşteri verisine canlı web portalından erişip satır düzenliyor | Alma (biçim), doğrula (ihtiyaç) | Muhasebeci ihtiyacı gerçek — bizde Aşama 05 muhasebeci paketiyle karşılandı. Canlı çok-taraflı portal sahiplik izolasyonunu ve "tek hesaplama yolu" kuralını zorlar; bizim dosya tabanlı dışa aktarma yeterli | Muhasebeci paketi / dışa aktarma |
| 8 entegrasyon (banka, e-arşiv, pazaryeri, kargo, POS, online tahsilat…) | Çoğunu alma | Banka bağlantısı ve e-belge kesin kapsam dışı; POS tahsilat ve fiş okuma bizde zaten var | — |

## Kanıt ve güven düzeyi

- Manuel gözlem: Giriş + kayıt ekranı, sektör listesi, sözleşme akışı
  (`kanitlar/logo-isbasi/01`–`04`). Sahte numarayla kayıt denendi, reddedildi.
  Tanıtım videosu 2 karesi (`05`–`06`, kullanıcı izledi 2 Eyl 2026)
- Resmî kaynak: isbasi.com özellik + fiyat listesi; logo.com.tr blog "kontrollü
  gelir-gider takibi"; isbasi.com/muhasebe-programi (9 Eyl 2026)
- Doğrulanamadı: Uygulama içi asıl akışların **canlı davranışı** ve ekran
  tasarımı (giriş yapılamadı); işleyiş adımları resmî kaynaktan çıkarıldı,
  Müşavir Portal ve entegrasyonların çalışma biçimi pazarlama görselinden
- Kullanıcıdan bekleniyor: isbasi.com kullanım videolarından ekran seviyesi not

## Tek cümlelik sonuç

Logo İşbaşı düşük alanlı bir kayıt formu ve yerel marka diliyle mikro işletmeyi hedefliyor; ama SMS doğrulaması olmadan uygulama içine girilemediği için asıl akışlar resmî kaynakla incelenmeli.
