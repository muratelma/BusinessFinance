# Brif 3 — Tasarım turu: formlar, kartlar, hesaplar, planlama

`00-ortak-cerceve.md` ile birlikte okunur. İlk teslimde (27 Eylül) ana ekranları bitirdik ve form sayfalarını
"sonra" diye bırakmıştık. Bu brif o kalan beş ekranı kapsar.

> **Zamanlama notu:** Bu tur Aşama 06.3'ün son grubudur. İşlemler filtresi ve kategori görünümü kararı
> (Grup 8) bu ekranların bağlantılarını etkiler; brif o karardan sonra bir kez güncellenir. Vergi Planlama'dan
> ayrılmış kabul edilir (Brif 1).
>
> **Ekle:** her ekranın uygulamadaki bugünkü hâlinin ekran görüntüsü (Pixel 8). Tasarımcı mevcut alanları
> ondan okur; aşağıdaki liste yalnız özettir.

## Beş ekranın hepsinde

- **Türkçe tarih ve binlik ayırıcı** her alanda aynı biçimde. Bugün gelir formunda `2026-09-28` ve `20790`
  görünüyor; hedef `28 Eylül 2026` ve `20.790`.
- **Form açıldığı yerden önden dolar:** Kasa'dan açılan gider formunda kasa seçili; bir kartın ayrıntısından
  açılan harcamada o kart seçili; bir hesabın ayrıntısından açılan transferde kaynak o hesap.
- Formlar KDV ve "Vergiden düşülebilir" alanlarını **artık taşımıyor** (kalktı).
- Kapsam çipi (`AppScopeField`): kullanıcı seçmezse hesabın/kartın etiketinden, o da yoksa kategorinin
  varsayılanından gelir; çipin altında değerin nereden geldiği yazar (`Hesabın etiketi`). Çözülemezse istek
  gitmeden alanın yanında söylenir. İşletmesi olmayan kullanıcıda çip hiç görünmez.
- Kayıt adı: `Ad (isteğe bağlı)` — boşsa kategori adı kullanılır.

## Ekranlar

### T1 · Gelir / gider formu
Bugün: Tutar, Tarih, Ödeme kaynağı (Hesaplar | Kredi kartları), Kategori, Kapsam, Ad, "Fişi sakla".
Fişten okunmuş hâli de çiz (okunan alanlar işaretli, düzeltilebilir). Hedef: en sık iş (tutar + kategori +
kaydet) tek ekranda, kaydırmadan.

### T2 · Kredi kartlarım
Liste: kart adı, güncel borç, kullanılabilir limit, son ödeme günü. Kart ayrıntısı: Güncel borç ·
Kullanılabilir · Hesap kesim günü · Son ödeme günü; ekstre kartı (Dönem borcu, Asgari ödeme, Son ödeme
tarihi, `Açık / Ödendi / Gecikmiş`); Harcamalar ve Ödemeler ayrı; taksit planları.
Eylem grupları: **Harcama · Taksitli harcama** (ikisi de dolgulu; aynı eylemin iki biçimi) ve **Ödeme ·
Ekstre** (tonal/kenarlı).

### T3 · Kart harcaması ve kart borcu ödemesi formları
- Harcama: harcandığı gün **gider** yazılır, kart borcu artar. Taksitli harcamada taksit sayısı ve aylık
  tutar önizlemesi; her taksit ayında gider yazılır.
- Ödeme: hesaptan karta; **gider değildir** (aynı harcama iki kez sayılmasın). `Ekstreyi öde` ve
  `Asgariyi öde` kısayolları.
- Kelimeler: "kredi kartı", "kartla ödedim"; burada "POS" geçmez.

### T4 · Hesaplar ve transfer
Hesap listesi (bakiyeler **kapsamdan etkilenmez**; bunu yazan satır), hesap ayrıntısı, transfer formu
(kaynak → hedef, tutar, gün, isteğe bağlı işlem ücreti). Transfer mavi ve işaretsizdir; gelir/gider değildir.
Hesap kapatma / pasife alma akışının yeri.

### T5 · Planlama (vergi ayrıldıktan sonra)
Bugün üç parça: **Tekrarlayanlar · Yaklaşanlar · Raporlar**.
- Tekrarlayanlar: plan satırı (ritim, sıradaki kalem, duraklatılmış), plan formu (Tür, Tutar, Kaynak, Kategori,
  Sıklık: `Günlük · Haftalık · Aylık · Üç ayda bir · Yılda bir · Seçilen aylarda`, Ay sonu davranışı,
  Başlangıç, Bitiş, Tekrar sınırı, Açıklama). Form uzun; hangi alanların "Ayrıntılar" altına ineceğini öner.
- Yaklaşanlar: vade ufku (7/30/90 gün), gecikmiş ve onay bekleyen kalemler, **Gerçekleştir** eylemi;
  tutarı belli olmayan vergi kalemi ve beklenen POS girişi (`girecek`) de burada görünür.
- Raporlar: nakit akışı eğilimi, aylık döküm, bütçe sapmaları, hesap dağılımı, net varlık.

## Senden karar önerisi beklediğim yerler

1. Formlar tam sayfa mı, alttan panel mi? (Bugün ikisi de var; tek kural öner.)
2. Uzun formlarda "Ayrıntılar" katlaması hangi alanları saklar?
3. Planlama üç parçası sekme mi, ayrı sayfalar mı?
