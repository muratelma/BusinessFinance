# Hesap seçici · Özet hesap bakiyeleri — teslim

Kaynak tasarım: `Hesap Secici ve Ozet.html` (yalnız seçilen ekranlar). Kod: `hesap/HesapSec.jsx`, `hesap/OzetHes.jsx` (ortak parçalar `hesap/Secici.jsx`, `hesap/KHParts.jsx`). Görüntüler `screenshots/` altında, tasarım dosyasından alındı (`_shot.html?id=H1`).
Bu dosyalar tasarım referansıdır; Flutter'da mevcut DS bileşenleriyle (AppFormField, AppMoneyText, Material Symbols) yeniden kurulur.

## 1 · Hesap seçici (formlardaki "Hesap" alanı)

Bugünkü açılır liste (yalnız ad, kenardan kenara) tamamen bunun yerine geçer. Gelir ekle, Gider ekle, Transfer ve hesap seçilen her formda aynı bileşen.

### Kapalı alan
- Değer yokken: yer tutucu "Hesap", sağda `arrow_drop_down`.
- Değer varken (H1b, `02`): etiket kenara çıkar; içerik **simge 32 · ad (16/600) · alt yazı "tür · kapsam" (13, ink-muted) · bakiye (14/500, ink-muted)**. Tür: Nakit / Banka hesabı / Kredi kartı. Ad kısalır, bakiye kısalmaz.
- Açıkken alan 2 px marka kenarlığına döner, ok `arrow_drop_up`.

### Açılır menü (H1, `01`)
- Alanın **altında, alan genişliğinde** (ekran kenarından 16 px), alanla arasında **8 px**.
- Çerçeve: yarıçap 16, **kenarlık yok**, yalnız `--elevation-floating` gölgesi; üst iç boşluk 4, alt 8. En fazla 600 px, sığmazsa içeride kayar (kaydırma çubuğu görünmez).
- Açılış ~150 ms, standard easing, 4 px yukarıdan inerek.
- **Menüde başlık/açıklama yazısı yok**; doğrudan hesaplar.
- Gruplar türe göre: **Nakit**, **Banka**, giderde ayrıca **Kredi kartı**. Grup başlığı 12/600, harf aralıklı, ink-faint; gruplar arası 1 px çizgi.
- Satır (min 56 px): **simge 20 (türe göre renkli) · ad 15 · alt yazı kapsam (İşletme / Şahsi) · bakiye 15/600**.
- **Seçili satır (S3):** solda 4 px marka çubuğu (dikeyde 8 px içeriden, sağ köşeleri yuvarlak) + `--surface-card-muted` zemin + ad 600, bakiye 700. **✓ yok; simge yerinde kalır.**
- Pasif hesaplar listede yer almaz. "Yeni hesap" satırı menüde **yok**.

### Gider · kredi kartı (H1g, `03`)
- Kredi kartı ayrı grupta; alt yazı "Kart borcu · İşletme"; borç **eksi ve gider kırmızısı** (`-₺3.930,00`).

### Transfer (H6, `04`)
- Nereden ve Nereye aynı bileşen. Nereye menüsünde **kaynak hesap** soluk (ink-faint), seçilemez, alt yazı "Para buradan çıkıyor", bakiye gösterilmez. Kredi kartı grubu yok.

### Hiç hesap yok (H7 `05`, H7o `06`)
- Alan boş ("Hesap"). Dokununca menü yerine alanın **altında beyaz kart** açılır (1 px border, yarıçap 16, gölge yok), **form aşağı kayar**; hiçbir şeyin üstüne binmez. Tekrar dokununca kapanır. Açılış ~200 ms.
- İçerik: `account_balance_wallet` 20 (ink-muted) · **"Henüz hesap yok"** (15/600) · "Kayıt için önce bir hesap ekleyin." (14) · metin düğmesi **"+ Hesap ekle"** → Hesaplar'daki **hesap ekleme sayfası**.

### Simge renkleri (Hesaplar sayfasıyla aynı)
- Nakit `payments` → `--category-3` · Banka `account_balance` → `--category-5` · Kredi kartı `credit_card` → `--neutral` · zemin `--surface-card-muted`.

## 2 · Özet · Hesap bakiyeleri (V1, `07`)

- Bölüm başlığı "Hesap bakiyeleri" (18/600) + `chevron_right` → Hesaplar sayfası. **"Kapsam filtresinden etkilenmez…" cümlesi yok.** Hesap sayısı yazılmaz.
- Tek kart. İçinde **Nakit** ve **Banka** grup satırları: renkli nokta 8 px (simge rengiyle aynı) + ad 15/600. **Grup toplamı yok; başlık altında çizgi yok.** Gruplar arasında 1 px çizgi.
- Hesap satırı (min 56 px): simge 40 kapsül (türe göre renkli) · ad 16/600 · alt yazı kapsam · bakiye 16/600. Satırlar arası çizgi yok. Grup içinde bakiyeye göre büyükten küçüğe.
- **Pasif hesaplar Özet'te gösterilmez.**
- Kapsam anahtarının (Hepsi · İşletme · Şahsi) bu bölüme etkisi ayrıca konuşulacak; şimdilik bölüm bütün hesapları gösterir.

## Genel kurallar
- Tutarlar AppMoneyText, sabit genişlikli rakam; istemci toplam hesaplamaz.
- 2.0× yazıda ad kısalır, tutar kısalmaz.
- Loading / error durumları mevcut AppFormField ve bölüm davranışıyla.
