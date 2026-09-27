# Bölüm 6 · Sınıflandırma yüzeyleri

Belge 1 · Rakip arayüz yaklaşımları

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Kayıt hangi eksenlerde sınıflandırılıyor ve bu arayüzde nasıl görünüyor?

Bu bölüm kategoriyi, etiketi, proje eksenini, karşı tarafı ve işletme/şahsi ayrımını arayüzdeki görünümüyle izler: formda nasıl seçildikleri ve hangi ekranlarda göründükleri.

Bir eksenin üründe bulunması, onun hangi amaçla kullanıldığını göstermez. Demo verisindeki adlardan kullanım amacı çıkarılmadı.

**Bu bölüme girmez**

- Kapsamın BusinessFinance için anlamı → Belge 3
- Rapor kırılımı → 9
- Karşı taraf hesabının borç ve tahsilat akışı → 8
- Demo proje ve cari adlarından kullanım amacı

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare |  |
| Bluecoins | Canlı kare |  |
| Wallet | Canlı kare |  |
| Hesap Defterim | Canlı kare | Kategori seçicisi yok. |
| Goodbudget | Canlı kare | Kategori yerine zarf. |
| KolayBi | Kaynak görseli | Proje ve cari ekseni. |
| Paraşüt, Logo İşbaşı | Görülmedi | Kaynakta anlatılmıyor. |
| QuickBooks Solopreneur | Kaynak beyanı | İşlem başına Business / Personal. |


## 6.1 · Kategori formda nasıl seçiliyor?

Üç üründe kategori ayrı bir seçicide: simgeli ızgara, üst başlıklara gruplu liste ya da sık kullanılanların öne alındığı liste.

**Şekil 6.1 · Money Manager · formun altındaki kategori paneli** (E0229)

1. Panel formun altında açılıyor; başlıkta düzenleme.
2. Simge ve addan oluşan 11 kutu.

**Şekil 6.2 · Bluecoins · kategori seçici** (E0071)

3. Arama ve Yeni aynı satırda.
4. Kategoriler üst başlık altında gruplu.
5. Alt kategoriler simgeyle.

**Şekil 6.3 · Wallet · Category** (E0349)

6. Sık kullanılanlar üstte.
7. Altında bütün ana kategoriler.
8. Ana kategori alt kategorilere açılıyor.

- Wallet'ta gider formunda gelir türünde bir kategori (Sale) seçili durabiliyor.
- Bluecoins'in Türkçe arayüzünde grup adları Türkçe (Araba, Eve Ait, Eğlence), alt kategori adları İngilizce (Fuel, Grocery, Others).

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Genel Gider Yönetimi'nde kategori kartları, altlarında tipler; Kategoriyi Düzenle, Yeni Tip Ekle ve Yeni Kategori Ekle düğmeleri. Kategoriler demo verisidir.

## 6.1 · Kategori seçicisi olmayan iki ürün

Hesap Defterim'de kategori listesi yok: ayarlardan açılan serbest bir metin kutusu ve düğme adlarını değiştiren bir diyalog var. Goodbudget kaydı kategori yerine zarfa bağlıyor.

**Şekil 6.4 · Hesap Defterim · Alındı formu** (E0151)

1. Açıklama / Kategori: serbest metin, seçici yok.

**Şekil 6.5 · Hesap Defterim · İşlem adları** (E0139)

2. Düğme adı seçenekleri: Ödendi Alındı, Gelir Gider, Özel.

**Şekil 6.6 · Goodbudget · Add Transaction** (E0116)

3. Kategori yerine zarf.

- Hesap Defterim'in İşlem adları yalnız düğme ve sütun adlarını değiştiriyor (→ 4.5); kayda bir sınıf eklemiyor.
- Goodbudget'ta zarf hem bütçe kovası hem sınıf işi görüyor (→ 7.6).

## 6.2 · Etiket yüzeyi

Bluecoins'te etiketlerin kendi ekranı var ve etiket kayıt satırında çip olarak görünüyor; Wallet'ta etiket raporda kategoriyle aynı düzeyde bir kırılım.

**Şekil 6.7 · Bluecoins · Etiketler** (E0088)

1. Etiket araması.
2. Listede İş ve Kişisel adlı değerler.

**Şekil 6.8 · Bluecoins · İşlemler, etiketli kayıt** (E0491)

3. Kayda verilen İş etiketi satırın altında çip.

**Şekil 6.9 · Wallet · Statistics › Spending** (E0278)

4. Rapor kategoriye ya da etikete göre bölünüyor.

- Bluecoins'te etiket formda çoklu seçimle veriliyor; seçicide beş değer var (Doğum günü, Film, İş, Kişisel, Tatil). Filtre panelinde de ayrı bir boyut (→ 9.4). İş ve Kişisel değerlerinin kapsam boyutu olarak kullanıldığı iddia edilmez.
- Wallet'ta etiketi kullanıcı oluşturuyor: formda ad, renk ve Auto assign to new records anahtarı. İki kayda Isletme ve Sahsi etiketi verildi; Labels görünümü yalnız etiketli kayıtları gösteriyor (toplamın anlamı → Belge 2 6.2).
- Wallet'ta etiket, kayıttan sonra açılan ayrıntı ekranında seçiliyor (→ 4.2).

**Aynı soruda diğer ürünler**

- *Görülmedi* · **Money Manager, Hesap Defterim, Goodbudget** — Etiket yüzeyi görülmedi.
- *Kaynak görseli* · **KolayBi** — Formlarda Etiketler alanı (→ 4.9).

## 6.3 · Proje ekseni

KolayBi kayıtları kullanıcının tanımladığı projelere bağlıyor. Proje listesi her projenin gelirini, giderini ve netini gösteriyor; proje sayfasında toplamlar sekmelere ayrılıyor.

**Şekil 6.10 · KolayBi · Projeler** (E0188)

1. Proje oluşturma, içe ve dışa aktarma.
2. Durum: Aktif / Pasif.
3. Listede Gelir, Gider ve Net sütunları.

**Şekil 6.11 · KolayBi · proje sayfası** (E0190)

4. Durum Değiştir: Aktif / Pasif; yanında silme simgesi.
5. Toplam gelir, gider, tahsilat ve ödeme sekmeleri.
6. "Kar / Zarar" ve "Nakit Durumu" ayrı alt sekmeler.

- Formlarda Proje alanı ve "Ayarlar sayfasından Proje Takip seçeneğini kapatabilirsiniz" notu var (→ 4.9).
- Proje kodu, adı, para birimi, başlangıç ve bitiş tarihleri listede ayrı sütunlar.
- Listedeki proje adları destek materyalinin örnek verisidir; kullanım amacı bu adlardan çıkarılmaz.
- Liste ile proje sayfası aynı projeye ait değil; iki ekranın toplamları karşılaştırılmadı.
- Proje ekseninin işletme/şahsi ayrımıyla ilişkisi → Belge 2 6.3.

## 6.4 · Karşı taraf bir eksen olarak

KolayBi'de cariler tipine göre ayrılıyor: müşteri, tedarikçi, ikisi birden, yurt dışı. Personel ve ortaklar ayrı sekmelerde.

**Şekil 6.12 · KolayBi · Genel Cari Hesapları (demo adlar karartıldı)** (E0194)

1. Sekmeler: Genel, Potansiyel, Personel, Ortaklar, Tekrarlı Maaşlar.
2. Cari Tipi: Müşteri, Tedarikçi, ikisi birden, Yurt Dışı.
3. Yerel bakiye işaretli ve renkli.

**Şekil 6.13 · KolayBi · Personel Cari Hesapları (demo adlar karartıldı)** (E0201)

4. Personel Carileri sekmesi.
5. Çalışma tipi cari tipinde.
6. Personelin de yerel bakiyesi var.

- Ortaklar sekmesinin kendi görseli destek sayfasında yayımlanmamış. Destek metnine göre ortak, cari oluşturmaya benzer biçimde tanımlanıyor ve ortağa maaş, prim gibi ödemeler yapılıyor: cari kartının bir alt türü.
- Bu cari türlerinin patronun şahsi harcaması için kullanıldığı kaynakta anlatılmıyor.
- Borçlandırma ve tahsilat → 8. Cari tiplerinin işletme/şahsi ayrımıyla ilişkisi → Belge 2 6.5.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Bluecoins** — Hesaplar arasında CARİ HESAP başlığı; karşı taraf bir hesap olarak (→ 8.1).
- *Canlı kare* · **Wallet** — Borçlar ayrı bir yüzeyde, kişi adıyla (→ 8.1).

## 6.5 · İşletme/şahsi ayrımı nerede var?

Kayıt düzeyinde bir işletme/şahsi alanı yalnız QuickBooks Solopreneur'ün yardım merkezinde anlatılıyor. Canlı incelenen beş üründe ve kaynakla incelenen üç Türk ön muhasebe ürününde böyle bir alan görülmedi; yokluk ifadeleri incelenen sürüm, yüzey ve kaynakla sınırlıdır. Tablo her ürünün bu ayrıma en yakın aracını gösteriyor.

| Ürün | Ayrıma en yakın araç |
|---|---|
| **Canlı incelenen ürünler** | |
| Money Manager | *Canlı kare* · Kategori listesi kişisel; etiket yüzeyi görülmedi. |
| Bluecoins | *Canlı kare* · Kategori ve etiket; bir kayda İş etiketi verilip listede görüldü (→ 6.2). |
| Wallet | *Canlı kare* · Labels; Isletme ve Sahsi etiketiyle denendi (→ 6.2). Ayarlarda Automatic rules: kategori ve etiketi kurala göre atama (kurulmadı). |
| Hesap Defterim | *Canlı kare* · Ayrı bir defter açmak; düğme adlarını değiştirmek. İkinci defterle ayrım denenmedi. |
| Goodbudget | *Canlı kare* · Zarflar (→ 6.1). |
| **Kaynakla incelenen ürünler** | |
| KolayBi | *Kaynak görseli* · Kullanıcı tanımlı proje; Ortaklar ve Personel carileri (→ 6.3, 6.4). |
| Paraşüt | *Görülmedi* · Kaynakta anlatılmıyor. |
| Logo İşbaşı | *Görülmedi* · Kaynakta anlatılmıyor. |
| QuickBooks Solopreneur | *Kaynak beyanı* · Alanın kendisi: işlem başına tek Type alanı, Business / Personal; üçüncü değer yok. Type ve Category ayrı sütun; listede Type'a göre süzme. Ayrım ABD vergi formuna hizalı. Yakın araçlar: Split, tutarı parçalara bölüp her parçaya ayrı tür ve kategori; Rules ile otomatik etiketleme; Exclude. |

- QuickBooks Solopreneur'ün hiçbir ekranı görülmedi; satır yalnız yardım merkezi anlatımıdır.
- Etiketin ve öteki araçların kapsam ayrımına ne kadar yaklaştığı → Belge 2 6.2.

*Dayanak.* Canlı kare: E0229, E0088, E0491, E0278, E0146, E0139, E0115. Kaynak görseli: E0188, E0194. Kaynak beyanı: E0011, E0009, E0012.

## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 6.1 | E0229 | Money Manager | Canlı kare | Money Manager · formun altındaki kategori paneli |
| 6.2 | E0071 | Bluecoins | Canlı kare | Bluecoins · kategori seçici |
| 6.3 | E0349 | Wallet | Canlı kare | Wallet · Category |
| 6.4 | E0151 | Hesap Defterim | Canlı kare | Hesap Defterim · Alındı formu |
| 6.5 | E0139 | Hesap Defterim | Canlı kare | Hesap Defterim · İşlem adları |
| 6.6 | E0116 | Goodbudget | Canlı kare | Goodbudget · Add Transaction |
| 6.7 | E0088 | Bluecoins | Canlı kare | Bluecoins · Etiketler |
| 6.8 | E0491 | Bluecoins | Canlı kare | Bluecoins · İşlemler, etiketli kayıt |
| 6.9 | E0278 | Wallet | Canlı kare | Wallet · Statistics › Spending |
| 6.10 | E0188 | KolayBi | Kaynak görseli | KolayBi · Projeler |
| 6.11 | E0190 | KolayBi | Kaynak görseli | KolayBi · proje sayfası |
| 6.12 | E0194 | KolayBi | Kaynak görseli | KolayBi · Genel Cari Hesapları (demo adlar karartıldı) |
| 6.13 | E0201 | KolayBi | Kaynak görseli | KolayBi · Personel Cari Hesapları (demo adlar karartıldı) |
