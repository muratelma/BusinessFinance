# Gün sonu (Brif 4) — teslim paketi

Kasa → Gün sonu panelinde, gün içinde zaten girilmiş satış, kartlı satış, tahsilat, veresiye satış ve alacak faturasının nakit/kart tutarında nasıl sayılacağı.

## İçerik
- `Gun Sonu Brif4.html` — bütün hâller tek canvasta (G1–G6 + G4 fatura / iki kişi). `?stress=1` = 999.999 taşma testi, `?scale=2` = 2.0× yazı testi.
- `gunsonu/GSParts.jsx` — parçalar (satır, ray, çift sorusu, Yazılacak özeti, üst alanlar, panel).
- `gunsonu/GSFrames.jsx` — hesap ve hâller (`GunSonu`, `GunSonuPos`).
- `gunsonu/README.md` — ayrıntılı kurallar, metinler, ölçüler. **Uygulama için ana belge.**
- `brif/` — 00 ortak çerçeve, 04 gün sonu brifi.
- `screenshots/` — 412 px genişlik, 1×, her hâlin tam boy görüntüsü (`_shot.html?id=g1` ile yeniden üretilir).
- `_ds/` — tasarım sistemi (token'lar + bileşen paketi).

## Ekran görüntüleri
| Dosya | Hâl |
|---|---|
| 01-g1-cevap-bekleyen | Üç kayıt cevapsız; Kaydet kapalı, nedeni Yazılacak'ta |
| 02-g2-cevaplanmis-ve-hesap | Hepsi içinde, Ayşe Terzi çıkarıldı; ₺1.670 − ₺670 = ₺1.000 |
| 03-g3-bazilari-dahil | Yalnız Ahmet Bakkal içinde; ₺1.120 |
| 04-g4-satis-ve-tahsilat-birlikte | Mehmet Usta ₺500 satış + ₺300 tahsilat, ₺200 ikisinde de |
| 05-g5-toplam-farki | Nakit boş, POS ₺500, Toplam ₺1.800 |
| 06-g4b-alacak-faturasi-ve-tahsilat | Fatura ve tahsilatı eşit; "Tahsilat faturanın içinde" |
| 07-g4c-ayni-gun-iki-kisi | Her çiftin altında ayrı soru |
| 08-g6-nakit-bos-kartli-kayit | Yalnız Ziraat POS ₺1.300; ₺1.300 − ₺800 = ₺500 |

## Son metinler
- Giriş alanı: `Nakit tutarı` · Yazılacak sonucu: `Yeni nakit satış` · döküm: `Nakit tutarı` / `Kart tutarı` / `Zaten kayıtlı`.
- Kural: *“İşaretli kayıtlar yazılan tutarın içindedir; tekrar kaydedilmez.”* (tek satır, 365/380 px)
- Toplu soru: *“Bunlar yazdığınız nakit tutarın içinde mi?”* · çift sorusu: *“Satış ve tahsilat yazdığınız nakit tutarda nasıl sayıldı?”*
- Komisyon iki satır: `Komisyon ₺10,00` / `12 Eki Pazartesi hesaba geçer`. Ortak düşülen tutar dökümde ayrı satır.

## Kontroller
- 1×: kural, döküm ve komisyon satırları tek satır; Vazgeç / Gün sonunu kaydet tek satır, 48 px.
- 2.0×: taşma yok; düğmeler alt alta iner, ad kısalır, tutar kısalmaz.
