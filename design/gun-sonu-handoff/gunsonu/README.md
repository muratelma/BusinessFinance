# Gün sonu paneli — Brif 4

Çerçeveler: `Gun Sonu Brif4.html` (önerilen G1–G5, ek hâller G4b/G4c/G6, karşılaştırma B1–B3).
Seçimden sonra son hâl teslim klasörüne ekran görüntüleriyle taşınır.

## Yerleşim (yukarıdan aşağı)

`AppFormSheet` (başlık `Gün sonu`) · Gün · `Nakit tutarı` · `Ziraat POS` + `Diğer POS'lar (2)` · `Toplam (isteğe bağlı)` ·
**Gün içinde girilenler** · **Yazılacak** · `Kasayı ya da kategoriyi değiştir` · `Vazgeç` / `Gün sonunu kaydet`.
Bölümler arası 16; satır en az 56 dp, sol 48 dp dokunma alanı (kutu 20); tutar sağda, kısalmaz.

## Gün içinde girilenler

- Başlık `Gün içinde girilenler` (etiket 13, `--ink-faint`) seçilen güne bağlıdır; geçmiş gün seçilince de doğrudur.
- Altında tek kural cümlesi: *“İşaretli kayıtlar yazılan tutarın içindedir; tekrar kaydedilmez.”*
- Satır: ad (16/600) · ikinci satırda tür ve yer (`Kartla · Ziraat POS`, `Tahsilat · Dükkan kasası`, `Veresiye satış`) · tutar. Saat yazılmaz (uygulamada yok).
- **Sıra:** işaretli gelenler (nakit satış, kartlı satış) üstte; altında 1 px çizgi ve sorulanlar (tahsilat, veresiye satış, alacak faturası).

### Görünürlük (kural 4)

| Durum | Nakit satış satırları | Kartlı satırlar | Soru ve sorulan satırlar |
|---|---|---|---|
| Nakit yazıldı | görünür | görünür | o gün varsa görünür |
| Nakit boş | gizli | görünür | gizli |
| Hiç kayıt yok | bölüm hiç çizilmez | | |

### Toplu cevap

- Sorulanların üstünde: *“Bunlar yazdığınız nakit tutarın içinde mi?”* ve iki dilimli `AppSegmentRail`: `Hepsi içinde` / `Hiçbiri`.
- Ray **seçimsiz gelir** (kural 2). Dokunulan dilim bütün sorulan satırlara uygulanır.
- Satırlar karışıksa ray seçimsiz görünür; ayrıca `Bazıları` dilimi yoktur.
- Altında: *“Tek tek değiştirmek için satıra dokunun.”*

### Satıra dokunma

- **Cevapsız (?)** → dokununca **içinde** (kutu dolu).
- **İçinde** → dokununca **değil** (kutu boş).
- **Değil** → dokununca **içinde**.
- Bir satıra “değil” demek: iki dokunuş ya da raydan `Hiçbiri` ve sonra içinde olanlara dokunmak.
- Cevapsıza bir daha dönülmez; cevaplanan satır hep kutulu görünür.
- İşaretli gelen satırlar (nakit/kartlı satış) iki hâllidir: işaretli ↔ işaretsiz.
- Erişilebilirlik: `role=checkbox`; cevapsız `aria-checked=mixed`.

## G4 · Aynı kişide (ya da aynı alacak faturasında) satış ve tahsilat

- Koşul: aynı kişinin veresiye satışı ve tahsilatı (ya da bir alacak faturası ve kendi tahsilatı) ikisi de **içinde**.
- Yer: iki satırın hemen altında, `--surface-card-muted` zeminli, 16 yarıçaplı blok. İki çift varsa her birinin altında ayrı blok.
- Metin: ad (14/600) · *“Satış ve tahsilat yazdığınız nakit tutarda nasıl sayıldı?”* (faturada *“Fatura ve tahsilatı …”*).
- Seçenekler (radyo, 48 dp), sağdaki sayı **bu ikisinden kayıtlı sayılacak tutar** (sunucudan):

| Seçenek | Örnek (₺500 satış + ₺300 tahsilat) |
|---|---:|
| İkisi ayrı ayrı | ₺800,00 |
| Tahsilat satışın içinde | ₺500,00 |
| Bir kısmı ikisinde de var → alan `İkisinde de sayılan`, yardımcı `En çok ₺300,00.` | ₺600,00 (₺200 yazılınca) |

- İkinci seçeneğin adı küçük olanın büyüğün içinde olduğunu söyler: tahsilat büyükse **`Satış tahsilatın içinde`**,
  faturada **`Tahsilat faturanın içinde`** / **`Fatura tahsilatın içinde`**. Tutar ikisinden büyük olanıdır.
- “Ortak tutar” ekranda geçmez; alan adı `İkisinde de sayılan`.
- Cevaplanmadan (ya da üçüncü seçenekte tutar boşken) kaydedilmez.
- Satırlardan biri “değil” yapılınca blok kaybolur, cevabı unutulur.

## Yazılacak

- Nakit kartı: `Yeni nakit satış` · `Dükkan kasası · Satış geliri` · sağda `+₺1.000,00` (gelir yeşili).
  Altında ince çizgi ve iki satır: `Nakit tutarı ₺1.670,00` · `Zaten kayıtlı −₺670,00`.
  En altta döküm (13, `--ink-faint`): `₺250,00 satış + ₺420,00 tahsilat`; G4'te ikinci satır `− ₺200,00 ikisinde de`.
- Kart kartı (POS yazıldıysa): `Ziraat POS satışı` · `Komisyon ₺10,00` / `12 Eki Pazartesi hesaba geçer` (iki satır) · tutar.
  Kartlı kayıt işaretliyse aynı yapıyla: `Kart tutarı` / `Zaten kayıtlı`.
- Yalnız yazılan alanın kartı çizilir (kural 7). Nakit boşsa nakit kartı yoktur.
- “düşüldü” kelimesi kullanılmaz.

## Cevapsız hâl

- Nakit kartında tutar yerine: *“3 kayıt için seçim yapılınca hesaplanır.”* / *“Yukarıdaki soru cevaplanınca hesaplanır.”*
- `Gün sonunu kaydet` kapalı (`AppSubmitButton` disabled). Kırmızı, uyarı kutusu yok.

## G5 · Toplam farkı

Nakit boş, `Ziraat POS` ₺500, `Toplam` ₺1.800. `AppInlineNotice`: *“Yalnız kart satışı kaydedilir. Toplamla arasındaki
₺1.300,00 kaydedilmez.”* Neden tahmin edilmez.

## Bileşenler

`AppFormSheet`, `AppTextField`, `AppSegmentRail` (2 dilim), onay kutulu satır, radyo satırı, `AppInlineNotice`,
`AppSubmitButton`. Yeni token yok. Bütün tutarlar sunucudan; çerçevedeki hesap yalnız gösterim içindir.
