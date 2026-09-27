# Uygulama Gözlem Formu — [Uygulama adı]

> **Doldurmadan önce:** `MANUEL-TEST-PROTOKOLU.md` → **"Gözlem formu yazım
> kuralları"**. Özet: ekran etiketleri **ekranda göründüğü dilde** yazılır ·
> kanıt atıfı **tam dosya adıyla** yapılır · kare değişirse atıf da değişir ·
> önceki koşumdan gelen iddia ya doğrulanır ya `Doğrulanamadı` olur ·
> kareler **dolu** ve dönemi **Ağustos 2026** olmalıdır.

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | |
| Sürüm | |
| Test tarihi | |
| Cihaz / işletim sistemi | |
| Dil / para birimi | |
| Hesap veya plan türü | |
| Erişim kısıtı | Yok / Ücretli / Bölgesel / Kayıt engeli |
| İnceleme türü | Manuel gözlem / Resmî kaynak / Karma |

## Ürün kimliği ve asıl amaç

Uygulama neyi çözüyor, kimin için, ne DEĞİL. Bir bulguyu değerlendirmeden önce
ürünün kendi tezini doğru koymak için. (Ör. Paraşüt tamamen şirket/ön muhasebe
tarafı — kişisel bütçe ürünü değil; Money Manager tam tersi.)

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | |
| Asıl hedef kullanıcı | |
| Çözdüğü ana iş | |
| Açıkça kapsam dışı bıraktığı | |
| İş modeli (ücretsiz / ücretli / freemium / reklam) | |
| BusinessFinance ile aynı kulvarda mı | Evet / Kısmen / Hayır — tek cümle gerekçe |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | | | | | |
| K01 Ana ekran | | | | | |
| K02 Hesap/cüzdan oluşturma | | | | | |
| K03 İşletme geliri | | | | | |
| K04 Şahsi gider | | | | | |
| K05 İşletme kart gideri | | | | | |
| K06 Transfer | | | | | |
| K07 Liste ve rapor | | | | | |
| K08 Düzeltme/iptal | | | | | |

Sonuç değerleri: `Tamamlandı`, `Desteklenmiyor`, `Ücretli`, `Engelli`,
`Belirsiz`.

## Hızlı arayüz gezintisi (Tur 1, görev dışı, ~10 dk)

K00–K08 bittikten sonra görevlerden bağımsız olarak gezilir. Amaç Belge 1 için
genişlik: her ekranı bir kez aç, ekran görüntüsü al, tek satır not düş.

**Tur 2'nin "tam arayüz taraması"yla karıştırılmamalı** — o, seçilen üç
uygulamada *her* ekranın ve alt menünün açılmasını isteyen ayrı ve çok daha
uzun bir adımdır (`MANUEL-TEST-PROTOKOLU.md` → Tur 2, konu 6).

| Alan | Gezildi mi | Kısa gözlem |
|---|---|---|
| Tüm ana sekmeler / alt görünümler (liste dışı) | | |
| Bir raporun içine tıklama (drill-down) | | |
| Bütçe / hedef / planlama ekranı (varsa) | | |
| Ayarların derinliği (kategori, tema, dışa aktarma, yedek) | | |
| Arama ve filtre davranışı | | |
| Boş durum ekranları | | |
| Hata / uç durum (negatif tutar, zorunlu alan boş, bozan düzenleme) | | |
| Widget / hızlı giriş / kısayol (varsa) | | |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | |
| Alt/üst gezinme | |
| Renklerin anlamı ve tutarlılığı | |
| Tipografi ve para değerlerinin okunması | |
| Kart, liste ve grafik kullanımı | |
| Form alanları ve varsayılanlar | |
| Loading, boş, hata ve başarı geri bildirimi | |
| Erişilebilirlik / dokunma alanları / metin yoğunluğu | |

## Sistem işleyişi / pipeline

Arayüzün arkasındaki model. Bir ekranın ne gösterdiği değil, bir kaydın sistemde
**ne yaptığı** ve akışların birbirine nasıl bağlandığı. Manuel testte davranıştan,
resmî kaynakta yardım merkezi + ürün turundan çıkarılır; her satır kanıt etiketli.

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Bir gelir/gider kaydı arka planda ne üretir (bakiye, rapor, başka kayıt) | | |
| Kart harcaması → kart borcu → kart ödemesi zinciri nasıl kurulur | | |
| Transfer / kart ödemesi gelir-gider raporundan ayrışıyor mu | | |
| Fatura/borç → tahsilat/ödeme → kapanış akışı (varsa) | | |
| Tekrarlayan/planlı kayıt: tanım mı üretir, onay mı bekler | | |
| İşletme/şahsi (veya en yakın) ayrım hangi katmanda tutuluyor | | |
| Ekrandan ekrana tipik yol (pipeline aşamaları) | | |
| Entegrasyon/dış sistem temas noktaları (banka, e-belge, POS, muhasebeci) | | |
| Veri nereye yazılıyor (yerel / bulut / senkron) | | |

**Pipeline şeması (kısa):** kilit akışın adımları ok dizisiyle
(ör. `+ → tür seç → tutar → kategori → hesap → kaydet → feed + bakiye + rapor`).

## Video/doküman akış yeniden kurulumu

**Yalnız içine girilemeyen (resmî kaynak) uygulamalar için.** Tanıtım/eğitim
videosu ve yardım merkezi adım adım makalelerinden akışların yeniden kurulumu.
Tümü `Resmî kaynak` etiketlidir; canlı ürün davranışı değildir.

Kolon kuralı: **"Bizdeki en yakın yapı"** bir *eşleme* notudur, değerlendirme
değil — hangisinin iyi olduğu burada yazılmaz (bkz. `README.md` → "Gözlem
formlarında dil kuralı").

| Akış | Kaynak (video zaman damgası / makale başlığı) | Adımlar (yeniden kurulmuş) | Bizdeki en yakın yapı (eşleme) |
|---|---|---|---|
| Gelir/fatura girişi | | | |
| Gider/fiş girişi | | | |
| Cari (borç-alacak) + tahsilat/ödeme | | | |
| Ana ekran / dashboard okuması | | | |
| Rapor / nakit akışı | | | |
| Muhasebeci aktarımı | | | |

Kullanıcının verdiği video ekran görüntüleri `kanitlar/<uygulama>/` altına
protokol adıyla kaydedilir; her satırda ilgili görsel adı yazılır.

## Ne kazandırıyor / ne kaybettiriyor

**Zorunlu bölüm.** Bu formdaki her ayırt edici yaklaşım için doldurulur.
Rakibi puanlamıyoruz; ödünleşimini yazıyoruz (`README.md` → "Gözlem
formlarında dil kuralı"). Bizim kararımız bir ölçüt değil, karşılaştırmanın
öbür tarafıdır.

| Yaklaşım (rakip ne yapıyor) | Ne kazandırıyor | Ne kaybettiriyor | Bizdeki karşılığı (eşleme) |
|---|---|---|---|
| | | | |

## Akış özeti

- En kısa ve güçlü akış:
- En fazla sürtünme yaratan akış:
- Uygulamanın hedef kullanıcı varsayımı:
- İşletme ve şahsi para yaklaşımı:
- Transfer ve kart ödemesi yaklaşımı:
- Planlama, borç ve tahsilat yaklaşımı:

## BusinessFinance için kararlar

Belge 3'ün ön değerlendirmesi; kesin karar Belge 3'te. **Beş sonucun tanımı
`README.md` → "Karar sonuçları — tek kaynak" bölümündedir**, buraya
kopyalanmaz. `Gerekçe` sütunu yalnız "ADR'miz böyle" diyemez — rakibin
çözümünün **ne kazandırdığını ve ne kaybettirdiğini** söylemek zorundadır.

| Bulgu | Karar | Gerekçe (kazandırdığı + kaybettirdiği) | Etkilenecek ekran/akış |
|---|---|---|---|
| | Doğrudan al / Uyarlayarak al / Alma / Henüz karar verme / **Kararı yeniden sor** | | |

## Kanıt ve güven düzeyi

Etiket tanımları `README.md` → "Kanıt etiketleri — tek kaynak" bölümündedir.

- Manuel gözlem:
- Resmî kaynak (alt tür parantezle):
- Çıkarım:
- Doğrulanamadı:
- Kullanıcıdan bekleniyor:

## Tek cümlelik sonuç

[Bu uygulamadan BusinessFinance için öğrenilen en önemli şey.]
