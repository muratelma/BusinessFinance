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
| Erişim kısıtı | **Kayıt engeli — iki katmanlı.** (1) Kayıt formu telefon + SMS doğrulaması istiyor, sahte numara sessizce reddediliyor. (2) 2 Eyl 2026: kullanıcı gerçek bilgiyle kayıt oldu; kayıttan sonra **"Hesabınız firmanıza özel hazırlanmaktadır, işlemler tamamlandığında e-posta ile bilgilendirileceksiniz"** pop-up'ı çıkıyor — anında deneme girişi yok, hesap satış/hazırlık sürecine giriyor (büyük ihtimalle ücretli). Uygulamaya hâlâ girilemedi |

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
| Fiyat/deneme | **14 gün ücretsiz deneme (kart istemiyor)**; 3 paket, 463₺/ay+KDV'den başlıyor; en popüler "e-Dönüşüm + Ön Muhasebe" 738₺/ay; yıllık %25 indirim | Kayıt sonrası "hazırlanıyor" pop-up'ı bu deneme akışının kapısı olabilir — kullanıcının deneme mailini beklemesi gerekiyor |

### Tanıtım videosundan (`yVPlmkgHHmc`, kullanıcı izledi 2 Eyl 2026)

Video anlatımı: işletme sahipleri için tasarlanmış; ana amaç **e-fatura + ön
muhasebe**. İnternet olan her yerden e-fatura kesme, müşteri/tedarikçi borç-alacak
(cari) takibi, anlık stok takibi. Sonunda "14 gün ücretsiz deneyin" → **ücretli**.

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

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| Kayıtta sadece 3 alan (şirket adı, sektör, telefon), VKN yok | Uyarlayarak al | Düşük sürtünmeli kayıt iyi; bizde de onboarding tek soru (`HasBusiness`). Sektör sormak bize gerekmiyor | Kayıt/onboarding |
| Başarısız işlemde sessiz geri dönüş, hata mesajı yok | Alma | Bizim proje kuralı: her ekranda görünür error durumu | Tüm formlar |
| Müşavir Portal: muhasebeci müşteri verisine canlı web portalından erişip satır düzenliyor | Alma (biçim), doğrula (ihtiyaç) | Muhasebeci ihtiyacı gerçek — bizde Aşama 05 muhasebeci paketiyle karşılandı. Canlı çok-taraflı portal sahiplik izolasyonunu ve "tek hesaplama yolu" kuralını zorlar; bizim dosya tabanlı dışa aktarma yeterli | Muhasebeci paketi / dışa aktarma |
| 8 entegrasyon (banka, e-arşiv, pazaryeri, kargo, POS, online tahsilat…) | Çoğunu alma | Banka bağlantısı ve e-belge kesin kapsam dışı; POS tahsilat ve fiş okuma bizde zaten var | — |

## Kanıt ve güven düzeyi

- Manuel gözlem: Giriş + kayıt ekranı, sektör listesi, sözleşme akışı (emülatör). Sahte numarayla kayıt denendi, reddedildi
- Resmî kaynak: isbasi.com özellik + fiyat listesi + tanıtım videosu
  (`youtube.com/watch?v=yVPlmkgHHmc`, kullanıcı izledi 2 Eyl 2026;
  `kanitlar/logo-isbasi/05`–`06` video kareleri)
- Doğrulanamadı: Uygulama içi asıl akışların **canlı davranışı** (giriş yapılamadı);
  Müşavir Portal ve entegrasyonların çalışma biçimi yalnız pazarlama görselinden

## Tek cümlelik sonuç

Logo İşbaşı düşük alanlı bir kayıt formu ve yerel marka diliyle mikro işletmeyi hedefliyor; ama SMS doğrulaması olmadan uygulama içine girilemediği için asıl akışlar resmî kaynakla incelenmeli.
