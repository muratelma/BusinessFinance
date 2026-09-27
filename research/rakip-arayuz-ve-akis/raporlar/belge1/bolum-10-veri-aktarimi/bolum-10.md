# Bölüm 10 · Veri aktarımı, yedek ve paylaşım

Belge 1 · Rakip arayüz yaklaşımları

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Veri üründen nasıl çıkıyor, ürüne nasıl giriyor ve kimlerle paylaşılıyor?

Bu bölüm çıktı seçimini, içe aktarma girişlerini, kayda eklenen dosyaları, yedek ve veri konumunu ve üçüncü kişilere açılan girişleri izler.

Bölümün ortak sınırı bir kez yazılır: bir seçeneği görmek dosya üretmek değildir; dosya üretmek de dosyayı alıcıya teslim etmek değildir. Ürün satırlarında yalnız o satıra ait sınır yazılır.

**Bu bölüme girmez**

- Kaydın finansal sonucu → Belge 2
- Rapor ekranı → 9
- Diğer modüller → 11
- Veri merkezi, şifreleme ve güvenlik hükmü

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare | Yedek ve Excel çıktısı incelenmedi. |
| Bluecoins | Canlı kare |  |
| Wallet | Canlı kare, Koşum kaydı | Bulut hesabının oturumlar arası sürekliliği koşum kaydında. |
| Hesap Defterim | Canlı kare | PDF'in içi karede; Excel dosyasının içi görülmedi. |
| Goodbudget | Canlı kare, Koşum kaydı | Dışa aktarma incelenmedi; yedek göstergesi görüldü, ortak hane karesiz. |
| KolayBi | Kaynak görseli, Kaynak beyanı |  |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 10.1 · Çıktı nereden seçiliyor?

Hesap Defterim'de çıktı iki yerden başlıyor: bütün hesaplar için dönem soran bir diyalog ve defter başına dönem sormayan bir liste. Bluecoins üç biçimi tek bir alt sayfada sunuyor.

**Şekil 10.1 · Hesap Defterim · Bildiri-Bütün Hesaplar** (E0169)

1. Dönem: Herşey ya da tarih aralığı.
2. Biçim: PDF ya da EXCEL.

**Şekil 10.2 · Hesap Defterim · üretilen PDF** (E0427)

3. Başlıkta defter adı ve dönem.
4. Önceki denge satırı; kolonlar Tarih, Notlar, Açıklama / Kategori, Gelir, Gider, Denge.
5. Aktarım bacağı Gider sütununda düz bir satır.
6. Altta özet: Toplam Gelir, Toplam Gider, Denge.

**Şekil 10.3 · Hesap Defterim · çıktı sonrası** (E0176)

7. Ürün dosyaların kasadefteri klasörüne kaydedildiğini söylüyor (→ 10.4).
8. Android paylaşım sayfası: 2 dosya paylaşılıyor.

**Şekil 10.4 · Bluecoins · İşlemi seçin** (E0105)

9. PDF veya Yazıcıya gönder, Excel (.csv), HTML.

- Hesap Defterim'in defter başına Bildiri'si yalnız PDF ve Excel soruyor; dönem sormuyor. Excel dosyasının içi ve alıcıya teslim görülmedi.
- Bluecoins'te Excel seçeneği .csv uzantısı taşıyor. Ürünün kendi cümlesi: "Tüm raporları PDF, Excel (cvs) veya Html'e aktarmak için soldaki yazıcı simgesinin olduğu her yerde bulunur." Dosya üretilmedi.
- Wallet'ta dışa aktarma Records menüsünde yok; çekmecedeki katlanmış Others bölümünde (Imports, Exports, Locations). Exports formu hesap, tür, ödeme türü ve tarih aralığı soruyor; biçimler PDF, XLS, CSV. Dosya üretilmedi.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Money Manager** — Toplam sekmesinde Excel(.xlsx) e-posta olarak gönder (→ 9.1).
- *Kaynak görseli* · **KolayBi** — Cari ekstrede tarih, para birimi ve yedi isteğe bağlı kolon; PDF önizleme (→ 8.5).
- *Kaynak beyanı* · **Paraşüt** — KDV dökümünden Excel'e aktarma anlatılıyor.
- *Görülmedi* · **Goodbudget** — Dışa aktarma incelenmedi.

## 10.2 · İçe aktarma nereden başlıyor?

Bluecoins'te içe aktarma, veri yönetimi ekranında iki biçimle yer alıyor. Wallet dosyadan içe aktarmayı hesap eklerken bir hesap türü olarak sunuyor.

**Şekil 10.5 · Bluecoins · Veri Yönetimi** (E0091)

1. Aynı ekranda yedekleme ve geri yükleme (→ 10.4).
2. Verileri İçe Aktar: Excel (.csv) ve QIF.

**Şekil 10.6 · Wallet · hesap türü seçimi** (E0471)

3. Bank Sync: banka bağlantısı (denenmedi).
4. File Import: CSV, Excel, OFX; dosyalar Wallet'a e-postayla gönderiliyor.
5. Manual Input: elle; içe aktarma ya da banka sonradan bağlanabiliyor.

- İçe aktarılan dosyanın şeması, eşleme ve sonucu denenmedi.
- Wallet'ta ayrıca çekmecede Others › Imports, hesap düzenleme formunda içe aktarma e-postası ve Enable automatic imports anahtarı var.
- Bluecoins içe aktarmada yalnız CSV ve QIF okuyor; banka ekstresi ayrıştırması → Belge 2 8.1.

**Aynı soruda diğer ürünler**

- *Görülmedi* · **Money Manager, Hesap Defterim, Goodbudget** — İçe aktarma yüzeyi görülmedi.

## 10.2 · İçe aktarma nereden başlıyor? — kaynakta

KolayBi'nin destek görsellerinde listelerin üstünde İçe Aktar ve Dışarıya Aktar düğmeleri yan yana; kayıt satırında e-Fatura durumu ayrı bir sütun.

**Şekil 10.7 · KolayBi · Genel Giderler** (E0198)

6. İçe Aktar ve Dışarıya Aktar.
7. e-Fatura Durumu sütunu: Aktarıldı / Aktarılmadı.

- e-Fatura sütunu dış bir sistemle bağ olduğunu gösteriyor; bağın işleyişi görülmedi.
- Görseller destek materyalidir; demo verisi taşıyor ve güncel sürüm doğrulanmadı.

**Karesi basılmayan ürünler**

- *Görülmedi* · **Paraşüt, Logo İşbaşı, QuickBooks Solopreneur** — İçe aktarma yüzeyi kaynakta anlatılmıyor ya da görülmedi; kaynaktan kurulan veri akışı → Belge 2 Bölüm 9.

## 10.3 · Kayda dosya eklemek ve dosyadan okumak

Hesap Defterim ve Wallet kayda dosya veya fotoğraf eklemeye izin veriyor. İki üründe de incelenen yolda dosyadan tutar veya tarih okunmadı; ek dosya saklamak, dosyadan veri okumak değildir.

**Şekil 10.8 · Hesap Defterim · Ödendi formu** (E0157)

1. Fatura ekle: Kamera, Fotoğraf Galerisi, PDF.

**Şekil 10.9 · Hesap Defterim · kayıttan sonra** (E0158)

2. Kayıt satırında ataç simgesi.

**Şekil 10.10 · Wallet · Record detail** (E0313)

3. Add receipt: Pick a file / Take a picture.
4. Attachments bölümü.

- Hesap Defterim'de ek tam ekran açılıp silinebiliyor.
- Fiş okumanın bu belgedeki tek yeri bu sorudur.

**Aynı soruda diğer ürünler**

- *Kaynak beyanı* · **Paraşüt** — Fiş fotoğrafından okuma anlatılıyor; çalışırken görülmedi.
- *Kaynak beyanı* · **Logo İşbaşı** — Fiş fotoğrafından okuma anlatılıyor; çalışırken görülmedi.
- *Kaynak beyanı* · **KolayBi** — Ürün özellikleri arasında fiş okuma (Fiş OCR) sayılıyor; incelenen web gider formunda fiş okumaya dair iz yok.
- *Kaynak görseli* · **KolayBi** — Gider formunda dosya alanı: kabul edilen türler ve 5 MB sınırı (→ 4.9).
- *Canlı kare* · **Money Manager** — Formun Detay satırında kamera simgesi; denenmedi.
- *Canlı kare* · **Bluecoins** — Formun üstünde ataç düğmesi; denenmedi.
- *Canlı kare* · **Goodbudget** — İşlem formunda dosya eki girişi görülmedi; form menüsünde yalnız Help.

## 10.4 · Yedek ve veri konumu

Hesap Defterim yedeklemeyi bir diyalogla hatırlatıyor ve kayıtları sunucusunda saklamadığını söylüyor. Money Manager ve Bluecoins'te yedek, ayarlardan açılan girişler.

**Şekil 10.11 · Hesap Defterim · Yedekleme kapalı** (E0149)

1. Kayıtlar sunucuda saklanmıyor; Drive'a kendi kopyası öneriliyor.

**Şekil 10.12 · Money Manager · Ayarlar** (E0254)

2. Yedekle; yanında PC'den Yönet.

**Şekil 10.13 · Bluecoins · Ayarlar** (E0090)

3. Veri Yönetimi ve Bulut Ayarları.

- Hesap Defterim'de Drive yedek ve geri yükleme yolu gerçek bir Google hesabıyla denenmedi.
- Money Manager ve Bluecoins'in araştırılan kurulumları yerel ve girişsiz kullanıldı. Bluecoins'in Veri Yönetimi ekranında yedeğin yeri Telefon hafızası.
- Hesap Defterim'in çıktı uyarısı dosyaların kasadefteri adlı bir klasöre kaydedildiğini söylüyor; dosya sisteminde bu adla bir klasör bulunmadı. PDF'ler Documents altındaki Hesap Defterim klasörüne, Excel uygulamanın kendi dış dizinine yazıldı (koşum kaydı, karesiz).
- Yedek menüsünü görmek geri yükleme denemesi değildir.

**Aynı soruda diğer ürünler**

- *Koşum kaydı* · **Wallet** — Bulut hesabıyla kullanıldı; kayıtlar sonraki oturumda korundu. Çok cihaz ve geri yükleme denenmedi.
- *Canlı kare* · **Goodbudget** — Zarflar ekranının üstünde son yedeğin zamanı yazıyor (Last Backup); yedeğin kendisi ve geri yükleme incelenmedi.
- *Kaynak beyanı* · **KolayBi, Paraşüt, Logo İşbaşı** — Bulut ve web ifadeleri ürün beyanı; veri konumu ve yedek bütünlüğü ölçülmedi.

## 10.5 · Üçüncü kişiye açılan girişler

Canlı incelenen ürünlerde başka bir kişiye erişim veren girişler görüldü ama hiçbiri kullanılmadı. Muhasebeci erişimi yalnız ürün anlatımlarında geçiyor.

**Şekil 10.14 · Wallet · çekmecenin alt kısmı** (E0376)

1. Group sharing.

- Bank Sync hesap eklerken de bir seçenek (→ 10.2). Çekmecenin üst kısmındaki Bank Sync girişinin karesi hesap sahibinin adını taşıdığı için basılmadı.
- Grup paylaşımı ve banka bağlantısı denenmedi.
- KolayBi'nin 2026'da yayımlanan bir videosunun karesinde panoda Müşavirini Davet Et bağlantısı var; bağlantının açtığı akış görülmedi.
- Muhasebecinin ürünlerde nerede durduğu → Belge 2 9.7.

**Aynı soruda diğer ürünler**

- *Koşum kaydı* · **Goodbudget** — Hesap bir household olarak açılıyor; ortak düzenleme ve yetki denenmedi. Kaynakta ücretli pakette banka senkronizasyonu anlatılıyor.
- *Kaynak beyanı* · **KolayBi** — Banka, e-belge, pazaryeri, sanal POS, geliştirici API ve çok müşterili muhasebeci erişimi.
- *Kaynak beyanı* · **Paraşüt** — Banka, e-ticaret, online tahsilat, e-belge; muhasebecinin hesabı canlı görüntülemesi.
- *Kaynak beyanı* · **Logo İşbaşı** — Müşterinin eklediği mali müşavir müşteri adına işlem yapabiliyor.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Banka ve kart hesabından gelen kayıtlar kategori ve Type ile inceleniyor; kural tanımlanabiliyor.

*Dayanak.* Kaynak beyanı: E0008, E0011, E0009, E0012.

## Özet: hangi yol nerede görüldü?

Yeni kanıt yok; önceki sayfaların satır satır dökümü. Dosya eki, içe aktarma, çıktı, yedek ve üçüncü kişi erişimi ayrı yollardır; birinin görülmesi ötekinin çalıştığını göstermez.

| Ürün | Çıktı | İçe aktarma | Ek dosya / okuma | Yedek / konum | Üçüncü kişi |
|---|---|---|---|---|---|
| **Canlı incelenen ürünler** | | | | | |
| Money Manager | *Canlı kare* · Excel(.xlsx) e-posta düğmesi (10.1). | Giriş bulunmadı (10.2). | *Canlı kare* · Kamera simgesi (10.3). | *Canlı kare* · Yedekle girişi; yerel kurulum (10.4). | — |
| Bluecoins | *Canlı kare* · PDF/Yazıcı, Excel (.csv), HTML (10.1). | *Canlı kare* · Excel (.csv), QIF (10.2). | *Canlı kare* · Ataç düğmesi (10.3). | *Canlı kare* · Bulut Ayarları; yedek telefon hafızasında (10.4). | — |
| Wallet | *Canlı kare* · Çekmece › Others › Exports: PDF, XLS, CSV (10.1). | *Canlı kare* · Hesap türü File Import; Imports; e-postayla (10.2). | *Canlı kare* · Dosya veya fotoğraf (10.3). | *Koşum kaydı* · Bulut hesabı (10.4). | *Canlı kare* · Group sharing, Bank Sync (10.5). |
| Hesap Defterim | *Canlı kare* · Dönemli PDF/Excel; PDF'in içi (10.1). | Giriş bulunmadı (10.2). | *Canlı kare* · Kamera, galeri, PDF; ataç (10.3). | *Canlı kare* · Sunucuda saklamama beyanı (10.4). | — |
| Goodbudget | İncelenmedi (10.1). | Giriş bulunmadı (10.2). | *Canlı kare* · Form menüsünde ek yok (10.3). | *Canlı kare* · Last Backup göstergesi (10.4). | *Koşum kaydı* · Household (10.5). |
| **Kaynakla incelenen ürünler** | | | | | |
| KolayBi | *Kaynak görseli* · Cari ekstre PDF (10.1). | *Kaynak görseli* · Listelerde İçe Aktar (10.2). | *Kaynak görseli* · Dosya türleri, 5 MB (10.3). | *Kaynak beyanı* · Bulut beyanı (10.4). | *Kaynak beyanı* · Muhasebeci, banka, e-belge (10.5). |
| Paraşüt | *Kaynak beyanı* · KDV dökümü → Excel (10.1). | — | *Kaynak beyanı* · Fiş okuma (10.3). | *Kaynak beyanı* · Bulut beyanı (10.4). | *Kaynak beyanı* · Muhasebeci, banka, e-belge (10.5). |
| Logo İşbaşı | — | — | *Kaynak beyanı* · Fiş okuma (10.3). | *Kaynak beyanı* · Bulut beyanı (10.4). | *Kaynak beyanı* · Müşavir işlem yetkisi (10.5). |
| QuickBooks Solopreneur | — | — | — | — | *Kaynak beyanı* · Banka ve kart akışı (10.5). |

- Boş hücre (—): o soru bu ürün için incelenmedi ya da kaynakta anlatılmıyor.

## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 10.1 | E0169 | Hesap Defterim | Canlı kare | Hesap Defterim · Bildiri-Bütün Hesaplar |
| 10.2 | E0427 | Hesap Defterim | Canlı kare | Hesap Defterim · üretilen PDF |
| 10.3 | E0176 | Hesap Defterim | Canlı kare | Hesap Defterim · çıktı sonrası |
| 10.4 | E0105 | Bluecoins | Canlı kare | Bluecoins · İşlemi seçin |
| 10.5 | E0091 | Bluecoins | Canlı kare | Bluecoins · Veri Yönetimi |
| 10.6 | E0471 | Wallet | Canlı kare | Wallet · hesap türü seçimi |
| 10.7 | E0198 | KolayBi | Kaynak görseli | KolayBi · Genel Giderler |
| 10.8 | E0157 | Hesap Defterim | Canlı kare | Hesap Defterim · Ödendi formu |
| 10.9 | E0158 | Hesap Defterim | Canlı kare | Hesap Defterim · kayıttan sonra |
| 10.10 | E0313 | Wallet | Canlı kare | Wallet · Record detail |
| 10.11 | E0149 | Hesap Defterim | Canlı kare | Hesap Defterim · Yedekleme kapalı |
| 10.12 | E0254 | Money Manager | Canlı kare | Money Manager · Ayarlar |
| 10.13 | E0090 | Bluecoins | Canlı kare | Bluecoins · Ayarlar |
| 10.14 | E0376 | Wallet | Canlı kare | Wallet · çekmecenin alt kısmı |
