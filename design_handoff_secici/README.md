# Teslim · Brif 5 (kategori) + Brif 6 (hesap) + Diğer → Kategoriler

Kaynak tuval: `Secici Onerileri.html`. Her telefon çerçevesinin tam boy görüntüsü (412×892) `screenshots/` altında; dosya adı = çerçeve kimliği. Görüntüler `_shot.html?id=<kimlik>` ile üretildi — aynı kod, aynı veri.

Brifler: `brif/00-ortak-cerceve.md`, `brif/05-kategori-ozeti-ve-kategori-sayfasi.md`, `brif/06-hesap-sayfasi-ve-hesaplar.md`. Bu dosyadaki kararlar briften sonra alındı; çelişirse **bu dosya geçerlidir**.

## Kod
- `screens-v6/Secici.jsx` — temel parçalar (I6 ikon, Card6, Cap6 kapsül, fmt6).
- `screens-v6/Menu.jsx` — eski açılır menü (yalnız bağımlılık).
- `screens-v9/KHParts.jsx` — ortak: Head9, Chip9, Menu9 (ara başlıklı), Tx9 satır, Days9, Tag9, Empty9, Sum9, Sheet9, KV9, Btn9, MonthDlg9.
- `screens-v9/Kategori.jsx` — A (Kategori özeti) ve B (kategori sayfası).
- `screens-v9/Hesap.jsx` — H (hesap sayfası), D (ayrıntı, transfer sayfası, Hesaplar).
- `screens-v9/DigerKat.jsx` — K0 (Diğer).

## Kararlar
**Diğer (K0)**
- "Para ve hesaplar": Hesaplar · Kredi kartlarım · Borç ve alacaklar · Cari hesap. ("Hesaplar ve transferler" → "Hesaplar".)
- Yeni başlık "Kategoriler": Kategori özeti (`leaderboard`) · Kategori yönetimi (`category`, Ayarlar'dan taşındı; ekranı bugünkü Kategoriler ekranı, değişmedi).
- "Vergi" başlığı, tek satır "Vergi takibi". Muhasebeci paketi kaldırıldı.

**Ortak**
- Gün başlığı: tarih (koyu) + gün adı (soluk) — "25 Ekim Pazar".
- Kategori sayfaları **kutucuklu**: bir günün işlemleri tek kartta, aralarında çizgi. Hesap sayfaları **İşlemler gibi**: tam genişlik beyaz blok.
- İşlem satırı: kapsül · başlık (en çok 2 satır) · alt yazı (en çok 2 satır) · durum satırı · sağda tutar (kısalmaz) · tutarın altı.
- Durum satırı ikonsuz, tek satır: "Ödendi · 20 Eki · Ziraat Vadesiz" (yeşil), "Açık" (gri).
- Menü: satır ikon · ad · tutar; seçili satırda ✓ ikonun yerine geçer; "Hepsi" `layers` ikonu. Ara başlık yalnız etikettir — ikon ve tutar yok, seçilemez. Alt satırlar girintisiz. Tutar rengi yönü söyler.
- Özet kartı alt satırı her zaman "Geçen ay ₺…" (seçim olsa da) — kart yüksekliği değişmez. Kategori sayfasında seçim varken "Kategorinin tamamı ₺…".
- Ay seçici: ortada dialog (K2). Kategoride çubuk rol rengi (gider kırmızı / gelir yeşil), hesapta gri (H15).
- Bilgi notu: mavi kutu + `info` ikonu (AppInlineNotice nötr). Ünlem kullanılmaz.

**Kategori (A, B)**
- A8: "Kategorisi yok" kendi kesik çizgili kartında, `category` ikonu gri kapsülde. Dokununca kayıtlar sayfası (A8b) → kayda dokun → "Kategori seç" paneli (A8c) → kayıt listeden çıkar, "Geri al" bildirimi (A8d).
- B5: durum satırı tek satır, tutar başlık hizasında.

**Hesap (H, D)**
- Bakiye kartı: Bakiye · Giren · Çıkan · Net. Kart yolda yazmaz.
- Yolda: bakiye kartının altında mavi kutu — "Yolda" + toplam, içinde en yakın tek satış (beyaz), fazlası "Tümünü gör · N satış". Tutar **hesaba geçecek tutardır** (komisyon düşülmüş), altında "₺326,00 komisyon sonrası". Satırların toplamı başlıktaki toplama eşittir.
- Yolda sayfası (H10): hesap sayfasıyla aynı iskelet — kart, gün başlıkları, tam genişlik işlem satırları. Para hesaba geçince satış listeden çıkar, hesapta POS yatışı olur.
- Başlıktaki `tune` ikonu = Hesap ayarları (48 px, erişilebilir adı "Hesap ayarları").
- Yeni transfer **sayfadır** (panel değil). Sıra: Tarih · Çıkan hesap · Giren hesap · Tutar · Masraf · Açıklama; DS alanları (56 px). Kaydet altta sabit. Geri / kaydet sonrası geldiği yere döner: İşlemler'den geldiyse İşlemler ana ekranı, + düğmesinden geldiyse oraya.
- Hesaplar (D5): toplam kartında Nakit / Banka; grup başlığında grup toplamı; satır alt yazısı kapsam (İşletme / Şahsi); yolda bilgisi yok; pasif hesaplar tek satır ("Pasif hesaplar · 1 · Toplama girmez"); başlıkta + = Hesap ekle; "Transfer yap" kartın altında.

## Ekran listesi
01 K0 · 02–16 A1–A12, A8b–A8d · 17–30 B1–B10 (B4b, B5b, B8b, B9b) · 31–45 H1–H13, H10b, H15 · 46–53 D1–D6 (D2b, D3b).
