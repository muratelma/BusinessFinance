# Uygulama Gözlem Formu — KolayBi

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | KolayBi' / KolayBi Yazılım (Türk Ekonomi Bankası iştiraki) |
| Sürüm | 3.3.1 (React Native) |
| Test tarihi | 1 Eylül 2026 |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` |
| Dil / para birimi | Türkçe (TR/EN seçilebilir) |
| Erişim kısıtı | **Kayıt engeli** — mobilde kayıt yok; yalnız "Giriş Yap", "QR Kod İle Giriş Yap", "Şifremi Unuttum". Kayıt web'de (kolaybi.com) |

## Görev gözlemleri

| Görev | Sonuç | Not |
|---|---|---|
| K00 İlk açılış | Kısmi | Emülatör açılışında "16 KB uyumlu değil" uyarısı (RN kütüphaneleri). Giriş ekranı: E-Posta + Şifre + Beni Hatırla + Giriş Yap + **QR Kod ile giriş** (masaüstünden hızlı eşleşme) + Şifremi Unuttum + TR/EN bayrak |
| K00 (kayıt denemesi, 2 Eyl) | Engelli | Kullanıcı doğruladı: mobil uygulamada **yalnız "Giriş Yap"** var, "Kayıt Ol / Hesap Oluştur" bağlantısı **hiç yok**. Kayıt sadece web'de (kolaybi.com) |
| K01–K08 | **Engelli** | Mobilde kayıt yok, hesap gerekiyor. Özellikler resmî kaynakla |

## Arayüz incelemesi (giriş öncesi)

| Başlık | Kısa gözlem |
|---|---|
| Giriş ekranı | Sade, açık gri zemin, mor (marka) birincil buton, bol beyaz alan; alanlar `*` ile zorunlu işaretli |
| Özgün öğe | "QR Kod İle Giriş Yap" — masaüstü oturumundan mobili hızlı bağlama |

## Resmî kaynak — özellikler ve kapsam (kolaybi.com, 2 Eyl 2026)

`Resmî kaynak` etiketli; emülatörde doğrulanmadı.

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

### Fiyat / deneme

- **14 gün ücretsiz deneme, kısıtsız.** Kayıt yalnız web'de. Yani **ücretli**.
- PLUS paketiyle kısıtsız e-fatura kontör + 1 yıl e-imza + banka entegrasyonu
  kampanyası.

### Karşılaştırma notu

Logo İşbaşı ve Paraşüt ile aynı kategoride: geniş kapsamlı ön muhasebe +
e-dönüşüm. **İşletme/şahsi tek havuz** kavramı yok. İki ayırt edici nokta:
(1) şahıs şirketini hedef olarak açıkça sayması, (2) **proje bazlı gelir-gider**
boyutu — bizim kapsam boyutumuzla kavramsal akraba ama farklı eksen.

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| QR kod ile hızlı giriş | Henüz karar verme | İlginç ama bizim tek-cihaz/mobil-öncelikli senaryoda önceliği düşük | Auth |
| Proje bazlı gelir-gider takibi (ikinci bir raporlama boyutu) | Henüz karar verme | Bizim kapsam boyutuyla aynı fikrin farklı ekseni; ürün kapsamımızda proje yok ama "birden çok raporlama boyutu" talebinin kanıtı | Rapor / kapsam |
| Mobilde kayıt yok, yalnız giriş + QR | Alma | Bizde mobil ana istemci; kayıt mobilde olmalı | Onboarding |
| Canlı çok kullanıcılı muhasebeci erişimi | Alma (biçim) | Sahiplik izolasyonu + tek hesaplama yolu kuralı; bizde dosya tabanlı dışa aktarma paketi | Muhasebeci paketi |

## Kanıt ve güven düzeyi

- Manuel gözlem: Yalnız giriş ekranı (emülatör). Kullanıcı doğruladı: mobilde kayıt yok
- Resmî kaynak: kolaybi.com ana sayfa + özellik listesi (2 Eyl 2026)
- Doğrulanamadı: Tüm asıl akışlar ve ekran tasarımı

## Tek cümlelik sonuç

KolayBi, şahıs şirketini de hedefleyen geniş kapsamlı bir bulut ön muhasebe +
e-dönüşüm ürünü; proje bazlı gelir-gider boyutu ve tekrarlayan işlem modeli bize
kavramsal kıyas olur, ama işletme/şahsi tek havuz kavramı olmadığı için yine
"klasik firma defteri" ailesinde.
