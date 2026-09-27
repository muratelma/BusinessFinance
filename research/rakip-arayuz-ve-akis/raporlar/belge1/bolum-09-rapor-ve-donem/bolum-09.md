# Bölüm 9 · Rapor ve dönem seçimi

Belge 1 · Rakip arayüz yaklaşımları

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Rapor ekranı neyi nasıl gösteriyor; dönem ve filtre nasıl seçiliyor?

Bu bölüm rapor ekranlarının biçimini, toplamların ekranda hangi etiketle yazıldığını, dönemin ve filtrenin nereden seçildiğini ve bir hesabı toplamın dışında bırakan denetimleri izler.

Toplamların neyi içerdiği ve doğruluğu bu bölümde anlatılmaz; ekranda görünen etiket, düğme ve alan anlatılır. Kareler farklı anlara ve veri durumlarına aittir; sayılar birbiriyle karşılaştırılmaz.

**Bu bölüme girmez**

- Dosya çıktısı ve teslim → 10
- Toplamların içeriği ve doğruluğu → Belge 2
- Vergi mevzuatı yorumu

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare |  |
| Bluecoins | Canlı kare | Kaydedilen filtrenin geri açılması görülmedi. |
| Wallet | Canlı kare | Filtre formu görüldü; filtre kaydedilmedi. |
| Hesap Defterim | Canlı kare | Grafik yerine tablo ve takvim. |
| Goodbudget | Canlı kare |  |
| KolayBi | Kaynak görseli | Rapor ailesi ve KDV raporu. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 9.1 · Rapor ekranı ne gösteriyor?

Dört üründe rapor grafikle açılıyor: pasta, dönem sütunları, çubuklar ya da rapor kartları. Hesap Defterim'de grafik görülmedi; rapor, dönem çipli bir liste.

**Şekil 9.1 · İstatistik** (E0231)

- Gelir / Gider sekmesi, pasta ve kategori listesi.

**Şekil 9.2 · Net Kazançlar** (E0023)

- İki dönem sütunu; kategori satırları.

**Şekil 9.3 · Statistics › Cash-flow** (E0280)

- Soru başlıklı kart; gelir ve gider çubukları.

**Şekil 9.4 · REPORTS** (E0121)

- İki rapor kartı: Spending by Envelope, Income vs Spending.

**Şekil 9.5 · İşlemler-Bütün Hesaplar** (E0142)

- Grafik yok; dönem çipli liste, altta sabit toplam.

- Beş kare aynı anın veya aynı veri durumunun görüntüsü değildir.
- Wallet'ta 6M ve 1Y aralıkları kilitli.
- Wallet'ın Statistics ekranında Balance, Outlook, Cash-flow, Spending ve Credit sekmeleri var.
- Bluecoins raporunda Net Kazançlar, Öğeler Özeti ve Etiketler sekmeleri var.
- Money Manager'da pasta dilimine dokununca kategori ayrıntısı açılıyor: kategori toplamı, aylar boyunca çizgi grafik ve o kategorinin kayıtları.

## 9.1 · Tablo, takvim ve ayrı satırlar

Money Manager'ın Toplam sekmesi gideri ödeme yöntemine göre iki satıra ayırıyor. Money Manager, Bluecoins ve Hesap Defterim'de dönem bir takvim olarak da okunuyor.

**Şekil 9.6 · Money Manager · Toplam sekmesi, Ağustos** (E0250)

1. Hesaplar kutusu: nakit-banka gideri 850; kredi kartı gideri 2.200 (1.200).
2. Excel(.xlsx) e-posta olarak gönder (→ 10.1).

**Şekil 9.7 · Bluecoins · Takvim, bir gün seçili** (E0098)

3. Ay takvimi; kayıtlı günlerde renkli noktalar.
4. Seçili günün GİDER, GELİR ve NET KAZANÇLAR kırılımı.

**Şekil 9.8 · Hesap Defterim · Takvim, Ağustos** (E0144)

5. Gün hücrelerinde yeşil giriş, kırmızı çıkış tutarı.
6. Altta sabit bant: Toplam Alındı, Toplam Ödendi, Denge (→ 9.2).

**Şekil 9.9 · Money Manager · İşlemler › Takvim, Ağustos** (E0234)

7. Gün hücresinde gelir mavi, gider kırmızı tutar; üstte ayın toplamları.

- Money Manager'ın kart satırındaki parantezli 1.200, dönem içindeki kart ödemesi; kartın dönem sonu borcu değil.
- Hesap Defterim takvimi seçili defterin toplamını gösteriyor.
- Hesap Defterim karesinin altındaki bant bir reklam.

## 9.2 · Toplamın etiketi ne söylüyor?

Hesap Defterim toplamları üç etiketle veriyor: Toplam Alındı, Toplam Ödendi, Denge; etiket, açılış bakiyesini veya aktarımı kapsayıp kapsamadığını yazmıyor. Bluecoins iki ayrı ölçüye bir harfle ayrılan adlar veriyor: Net Kazançlar ve Net Kazanç. Neyin dahil olduğu → Belge 2 1.3, 7.3.

**Şekil 9.10 · Hesap Defterim · İşlemler-Bütün Hesaplar, Aylık** (E0142)

1. Aktarım iki satır: Kime ve Kimden.
2. Toplam Alındı, Toplam Ödendi, Denge.

**Şekil 9.11 · Hesap Defterim · Is Karti, gün gün özet** (E0148)

3. Üçüncü sütunun adı Tasarruf.
4. Aynı sütun altta Denge adıyla.

**Şekil 9.12 · Bluecoins · Hesaplar panosu, iki blok** (E0053)

5. Birinci blok: Net Kazançlar.
6. Satırları Gelir, Gider, Net Kazançlar.
7. İkinci blok: Net Kazanç.
8. Satırları Varlıklar, Cari hesap, Net Kazanç.

- Hesap Defterim'de aynı ölçü iki farklı adla (Tasarruf, Denge) görünüyor.
- Bluecoins'te ayrı iki ölçünün adları bir harfle ayrılıyor; blok başlığı dışında ölçüyü anlatan bir metin karede yok.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Money Manager** — Üst bantta Gelir, Gider, Toplam.
- *Canlı kare* · **Wallet** — Cash-flow kartının başlığı bir soru: Am I spending less than I make?
- *Canlı kare* · **Goodbudget** — Income vs Spending kartında Net Total.

## 9.3 · Dönem nereden seçiliyor?

Beş üründe dönem denetimi farklı yerde duruyor: başlıkta ay gezgini, çip satırı, rapor kartının içi, rapor başlığının yanı ya da ekranın altı.

- **Şekil 9.13 · Money Manager** (E0231) — *Ekranın üstü.* Oklu ay gezgini; sağda Ay seçicisi. Tam ekran: Şekil 9.1.
- **Şekil 9.14 · Hesap Defterim** (E0142) — *Başlık çubuğunun altı.* Çipler: Herşey, Günlük, Haftalık, Aylık, Yıllık; altında oklu tarih aralığı. Tam ekran: Şekil 9.5.
- **Şekil 9.15 · Bluecoins** (E0023) — *Rapor başlığının sağı.* Dönem düğmesi: Bu Ay. Tam ekran: Şekil 9.2.
- **Şekil 9.16 · Goodbudget** (E0121) — *Rapor kartının içi.* Kart içinde dönem yazıyor; rapor açılışta içinde bulunulan ayı gösterdi. Tam ekran: Şekil 9.4.
- **Şekil 9.17 · Wallet** (E0280) — *Ekranın altı.* Aralık çipleri: 7D, 30D, 12W, 6M, 1Y; son ikisi kilitli. Tam ekran: Şekil 9.3.

- Hesap Defterim ayarlarında ayın, haftanın ve yılın ilk günü ile varsayılan süre ayarlanıyor.
- Wallet'ın gelişmiş ayarlarında muhasebe döneminin başladığı gün: 1.
- Money Manager'ın Toplam görünümü dönemi tarih aralığıyla yazıyor.
- Hesap Defterim'in Haftalık ve Yıllık çiplerinde listenin üstünde bir Önceki denge satırı çıkıyor; önceki denge ayarla açılıp kapanıyor.
- Wallet'ın dönem seçicisi üç sayfalı: göreli çipler, adlandırılmış dönem (Today, This week, This month; This year kilitli) ve özel tarih aralığı.
- Dönem ayarlarının raporlara etkisi denenmedi.
- Goodbudget'ın her açılışta içinde bulunulan ayla açıldığı tek açılışla sınırlı bir gözlem.

## 9.4 · Filtre yüzeyi

Bluecoins filtreyi tek bir alt sayfada topluyor. Money Manager filtreyi rapor üstündeki bir panelde sekmelerle veriyor. Wallet'ta özel filtreler ayarlardan tanımlanıyor.

**Şekil 9.18 · Bluecoins · filtre alt sayfası** (E0082)

1. Sıfırla, kaydet, aç simgeleri.
2. Metin araması ve tutar aralığı.
3. Tarih, işlem tipi, kategori, hesap, etiket, durum; yanlarında süzgeç.

**Şekil 9.19 · Money Manager · filtre paneli** (E0251)

4. GELİR, GİDER, HESAP sekmeleri.
5. Hesap başına iki sütun çifti: gelir/havale ve gider/havale.

**Şekil 9.20 · Wallet · Settings › Filters › Add filter** (E0476)

6. Filtreye ad veriliyor: kaydedilen bir süzgeç.
7. Kaydın onay durumu da bir süzgeç.
8. Etikete göre süzme.
9. Transfers: dahil ya da hariç.

- Bluecoins'te Ada araması iki transfer bacağını ve geliri buldu. Kaydet simgesi ad soran bir sayfa açıyor; kaydedilen filtrenin geri açılması görülmedi.
- Money Manager'da panelde bir hesap seçilince paneldeki toplam hemen değişiyor; Filtre düğmesi aynı süzmeyi listeye uyguluyor ve seçili filtre listenin altında yazıyor.
- Hesap Defterim'de arama çubuğu alt toplamları süzülen kayıtlara göre yeniden yazıyor.
- Money Manager filtre panelinde havale sütun başlıkları verinin yönüyle ters görünüyor.
- Wallet formunun devamında Debts (dahil / hariç) ve metin araması var; filtre Settings › Filters'tan açılıyor, kaydedilmedi.

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Alış/satış raporunda cari, ödeme durumu, para birimi, şube, tarih ve vade filtreleri.
- *Görülmedi* · **Goodbudget** — Filtre yüzeyi görülmedi.
- *Canlı kare* · **Goodbudget** — İşlem araması var: Transaction Search.

## 9.5 · Bir hesabı toplamın dışında bırakmak

İki üründe bir hesabı bir toplamın dışında bırakan denetim var ve ikisi farklı ölçülere bağlı. Money Manager'ın anahtarı hesap formunda, Bluecoins'inki nakit akışı ayarında.

**Şekil 9.21 · Money Manager · Hesap Bilgisi** (E0252)

1. Toplama Dahil Et anahtarı; altında Göster/Gizle.

**Şekil 9.22 · Money Manager · Hesaplar, anahtar kapalı** (E0253)

2. Üst bantta Varlıklar, Borçlar, Toplam.
3. Grup satırında 0,00.
4. Kapalı hesabın satırı gri.

**Şekil 9.23 · Bluecoins · Nakit Akım Ayarı** (E0086)

5. Nakit akışına katılan hesaplar tek tek seçiliyor.

- İki denetim eşdeğer sayılmaz: biri hesap listesinin toplamına, öteki nakit akışı raporuna bağlı.
- Bluecoins ayarının kendi cümlesi: "Nakit akışı hesaplarken kullanılacak nakit hesapları seçiniz." Görülen seçimlerin ürün varsayılanı olup olmadığı doğrulanmadı.
- Kapalı anahtarın toplamlara etkisi → Belge 2 2.5.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Wallet** — Hesap ayarında Exclude from stats anahtarı (→ 5.3).
- *Görülmedi* · **Hesap Defterim, Goodbudget** — Bu tür bir denetim görülmedi.

## 9.6 · Boş rapor ve kaynaktaki rapor aileleri

Goodbudget'ta kaydı olmayan dönemin raporu boş bir grafik ve tek satırlık bir metin gösteriyor. KolayBi'nin rapor sayfasında on rapor üst barda yan yana.

**Şekil 9.24 · Goodbudget · Spending by Envelope, boş dönem** (E0122)

1. Başlıkta dönem; üst çubukta takvim simgesi.
2. Gri pasta, Total Spending 0.00, No transactions found.

**Şekil 9.25 · KolayBi · KDV Raporu** (E0214)

3. Üst barda on rapor.
4. Fatura başlangıç ve bitiş tarihi; Filtrele.
5. Matrahlı anahtarı.
6. Oran sütunlarında matrah ve tutar; satırlarda belge türleri.

- Karedeki matrah ile tutarın oranı, ürünün vergiyi kendisinin hesapladığını göstermez.
- Aynı tarih aralığında KolayBi Gelir/Gider Raporu boş; iki karenin veri anı bilinmiyor.

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Alış/satış raporunda cari, proje, ürün ve etiket kırılımları.
- *Kaynak beyanı* · **Paraşüt** — Gelir/Gider ile Kasa/Banka raporları ayrı.
- *Kaynak beyanı* · **Logo İşbaşı** — Tarih aralıklı, kategorize raporlar ve grafikler.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — İşletme/şahsi ayrımı vergi raporuna bağlanıyor (→ 6.5).

*Dayanak.* Kaynak beyanı: E0011, E0009, E0012.

## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 9.1 | E0231 | Money Manager | Canlı kare | İstatistik |
| 9.2 | E0023 | Bluecoins | Canlı kare | Net Kazançlar |
| 9.3 | E0280 | Wallet | Canlı kare | Statistics › Cash-flow |
| 9.4 | E0121 | Goodbudget | Canlı kare | REPORTS |
| 9.5 | E0142 | Hesap Defterim | Canlı kare | İşlemler-Bütün Hesaplar |
| 9.6 | E0250 | Money Manager | Canlı kare | Money Manager · Toplam sekmesi, Ağustos |
| 9.7 | E0098 | Bluecoins | Canlı kare | Bluecoins · Takvim, bir gün seçili |
| 9.8 | E0144 | Hesap Defterim | Canlı kare | Hesap Defterim · Takvim, Ağustos |
| 9.9 | E0234 | Money Manager | Canlı kare | Money Manager · İşlemler › Takvim, Ağustos |
| 9.10 | E0142 | Hesap Defterim | Canlı kare | Hesap Defterim · İşlemler-Bütün Hesaplar, Aylık |
| 9.11 | E0148 | Hesap Defterim | Canlı kare | Hesap Defterim · Is Karti, gün gün özet |
| 9.12 | E0053 | Bluecoins | Canlı kare | Bluecoins · Hesaplar panosu, iki blok |
| 9.13 | E0231 | Money Manager | Canlı kare | Money Manager · Ekranın üstü |
| 9.14 | E0142 | Hesap Defterim | Canlı kare | Hesap Defterim · Başlık çubuğunun altı |
| 9.15 | E0023 | Bluecoins | Canlı kare | Bluecoins · Rapor başlığının sağı |
| 9.16 | E0121 | Goodbudget | Canlı kare | Goodbudget · Rapor kartının içi |
| 9.17 | E0280 | Wallet | Canlı kare | Wallet · Ekranın altı |
| 9.18 | E0082 | Bluecoins | Canlı kare | Bluecoins · filtre alt sayfası |
| 9.19 | E0251 | Money Manager | Canlı kare | Money Manager · filtre paneli |
| 9.20 | E0476 | Wallet | Canlı kare | Wallet · Settings › Filters › Add filter |
| 9.21 | E0252 | Money Manager | Canlı kare | Money Manager · Hesap Bilgisi |
| 9.22 | E0253 | Money Manager | Canlı kare | Money Manager · Hesaplar, anahtar kapalı |
| 9.23 | E0086 | Bluecoins | Canlı kare | Bluecoins · Nakit Akım Ayarı |
| 9.24 | E0122 | Goodbudget | Canlı kare | Goodbudget · Spending by Envelope, boş dönem |
| 9.25 | E0214 | KolayBi | Kaynak görseli | KolayBi · KDV Raporu |
