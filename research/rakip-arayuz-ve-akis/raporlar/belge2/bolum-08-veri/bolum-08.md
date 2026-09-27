# Bölüm 8 · Veri: dışa aktarma, yedek ve yardımcı araçlar

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Kayıtlar uygulamanın dışına nasıl çıkıyor, nerede duruyor ve kaybolursa ne oluyor?

Önceki yedi bölüm kayıtların içeride nasıl davrandığını izledi. Bu bölüm dışarı bakıyor: veri hangi biçimde çıkıyor, nereye yazılıyor ve cihaz kaybolursa ne oluyor.

Sonunda bölümün ikinci yarısı var: ürünlerin finansal akışın dışında taşıdığı yardımcı araçlar. Küçük görünüyorlar ama kullanıcının uygulamadan ne beklediğini gösteriyorlar.

**Bu bölüme girmez**

- Raporun içeriği ve dönem seçimi → Bölüm 7
- Silinen kaydın toplamlara etkisi → Bölüm 1
- Ekranların görsel düzeni → Belge 1 §10

| Ürün | Kanıt | Not |
|---|---|---|
| Hesap Defterim | Canlı kare | Dışa aktarma, klasör uyarısı ve yedek diyaloğu kareli. |
| Bluecoins | Canlı kare | İçe aktarma girişleri, çöp kutusu ve seyahat modu kareli. |
| Money Manager | Canlı kare | Yedekleme ve PC bağlantısı girişleri kareli. |
| Goodbudget | Canlı kare | Ek ve dosya yüzeyi menüde arandı. |
| Wallet | Canlı kare | Dışa aktarma formu çekmecenin katlanmış menüsünde kareli. |
| KolayBi | Kaynak görseli | Cari ekstresi ve PDF önizlemesi destek görselinde. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 8.1 · Veri dışarı nasıl çıkıyor

Çıkış yolları ürüne göre değişiyor: dosya olarak cihaza, e-posta eki olarak, ya da üç biçimden birinde. İçeri alma tarafında ise yalnız dosya biçimleri var; Bluecoins'in içe aktarması iki biçimle sınırlı.

**Şekil 8.1 · Üretilen PDF** (E0427)

- Tarih · Notlar · Gelir · Gider · Denge.
- Açılış 20.000 Gelir sütununda.
- Aktarım bacakları Gider satırı.

**Şekil 8.2 · Dosya nereye yazıldı** (E0176)

- "Veriler … kasadefteri adlı bir klasöre kaydedilir."
- Uyarı bir klasör adı veriyor.
- Altında Android paylaşma sayfası.

**Şekil 8.3 · Ayarlar ızgarası** (E0254)

- Yedekle ve PC'den Yönet ayrı girişler.
- CalcBox ve Giriş Kodu da burada.
- Dışa aktarma rapor ekranından e-postayla.

**Şekil 8.4 · Dışa aktarma formu** (E0428)

- Hesap · tür · ödeme tipi · tarih aralığı.
- Aktarımlar dahil edilsin mi ayrıca soruluyor.
- PDF / XLS / CSV.

**Şekil 8.5 · Veri Yönetimi** (E0445)

- Yedek: telefon hafızası.
- İçe aktarma yalnız Excel (.csv) ve QIF.
- Banka ekstresi okuyan bir giriş yok.

- Hesap Defterim iki ayrı çıkış yüzeyi taşıyor: bütün hesapları kapsayan dönemli bir dışa aktarma ve defter başına dönemsiz bir tane. İkisi de PDF ve Excel sunuyor.
- Money Manager'ın rapor ekranında ayrıca "Excel(.xlsx) e-posta olarak gönder" girişi var; dosya cihaza yazılmak yerine doğrudan paylaşılıyor.
- Hesap Defterim'in PDF'i ekrandaki tanımı dosyaya da taşıyor: Ana Hesap defterinde açılış bakiyesi Gelir sütununda, iki aktarım bacağı Gider satırı olarak duruyor; Toplam Gelir 47.500 açılışı da içeriyor. Bölüm 1'deki toplam dosyada da aynı.
- Ürünün söylediği klasör ise oluşmuyor: cihazın dosya sisteminde kasadefteri adlı bir klasör bulunamadı; PDF'ler Documents altındaki Hesap Defterim klasörüne, Excel uygulamanın kendi veri dizinine yazılmış. Bu bir koşum kaydıdır; dosya sistemi araması kareye alınmadı.
- Hesap Defterim'de defter başına dışa aktarmada dönem seçimi yok, yalnız PDF ve Excel satırları var.
- Wallet'ta dışa aktarma Ayarlar'da ve kayıt listesinin menüsünde yok; çekmecenin katlanmış Others bölümünde duruyor.
- Bluecoins'in yazıcı simgesi üç çıktı sunuyor: PDF veya yazıcı, Excel (.csv) ve HTML. Dosya üretilmedi.

## 8.2 · Veri nerede duruyor, kaybolursa ne oluyor

Bir ürün bu soruyu kendisi soruyor ve cevabı açıkça yazıyor. Diğerlerinde cevap ayarların içinde bir giriş olarak duruyor; silinen kaydın ikinci bir şansı olup olmadığı da ürüne göre değişiyor.

**Şekil 8.6 · Yedekleme kapalı uyarısı** (E0149)

- "Kayıtlarınızı sunucularımızda saklamıyoruz…"
- Telefon değişiminde kayıt kaybı uyarısı.
- Atla / Yedeklemeyi Aç.

**Şekil 8.7 · Silinmiş işlemler** (E0143)

- Silinen kayıt ayrı bir listede.
- Geri yükleme bu listeden.
- Kalıcı silme ikinci onay istiyor.

**Şekil 8.8 · Çöp kutusu** (E0089)

- Ayrı bir çöp kutusu yüzeyi var.
- Karede kutu boş.
- Boş durumda yönlendirme yok.

**Şekil 8.9 · Kayıt menüsü** (E0128)

- Kaydın menüsünde yalnız Help var.
- Bu yüzeyde dosya veya fotoğraf eki yok.

- Hesap Defterim'in uyarısı bu bölümün en açık cümlesi: ürün verinin yalnız cihazda durduğunu ve yedek kapalıysa geri yüklemenin mümkün olmadığını kendi ağzıyla söylüyor. Bulut bir varsayım değil, bir tercih olarak sunuluyor.
- Geri alma iki üründe ayrı bir yüzey: Hesap Defterim'in Silinmiş işlemler listesi ve Bluecoins'in çöp kutusu. Money Manager, Wallet ve Goodbudget'ta menüler ve ayarlar tarandı; böyle bir katman yok.
- Bluecoins'in çöp kutusu çalışıyor: silinen kayıt oraya düşüyor ve geri yükleniyor, ama geri yüklenen kayıt işlem listesine ancak uygulama yeniden açılınca geliyor. Bu iki ayrı kullanıcı kontrolüyle kayıtlı; rapor toplamı ise geri yüklemeyi hemen sayıyor.
- Hesap Defterim'de kalıcı silme ikinci bir onay diyaloğu açıyor.
- Money Manager'ın ayarlarında Yedekle girişi var; içeriği açılmadı.

## 8.3 · Kaynakta çıktı: dosya değil, belge

KolayBi'de dışa aktarma bir yedek alma işi değil, bir belge üretme işi. Ekstre oluşturmadan önce hangi sütunların görüneceği seçiliyor, sonra çıktı önizleniyor ve e-postayla gönderilebiliyor. Alanların varlığı kanıtlı; çıktının gerçek içeriği ölçülmedi.

**Şekil 8.10 · KolayBi · Cari Ekstresi Oluştur** (E0196)

- Tarih aralığı, para birimi ve açıklama.
- Yedi isteğe bağlı sütun anahtarı.
- Kapat / Yazdır / Oluştur.

**Şekil 8.11 · KolayBi · PDF önizlemesi** (E0197)

- Tek sayfalık Cari Hesap Ekstresi.
- Borç, alacak ve bakiye kolonları.
- Yeni sekme, e-posta ve kapat.

- Sütun anahtarları çıktının biçimini kullanıcıya bırakıyor: aynı ekstre farklı alıcılar için farklı görünebiliyor. Canlı beş üründe dışa aktarmanın biçimi sabit — yalnız PDF mi Excel mi ve hangi dönem sorulabiliyor.
- Çek ve senet listelerinde de ayrı bir Dışarıya Aktar girişi ve bordro kavramı var; belge üretme bu üründe tek bir yerde değil, yüzeylere dağılmış.
- Çekler listesinde Toplu Çek Ekle, Bordrolar ve Dışarıya Aktar girişleri birlikte duruyor.

**Aynı soruda kaynaktan okunan üç ürün**

- *Kaynak beyanı* · **Paraşüt** — Kaynak KDV raporunun Excel'e aktarılabildiğini yazıyor; ayrıca muhasebeciye canlı erişim veriliyor — dosya alışverişi yerine aynı veriye ortak erişim. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **Logo İşbaşı** — Kaynak bir müşavir portalından söz ediyor: müşavir müşterinin verisine erişiyor ve müşteri onu eklediğinde adına işlem yapabiliyor. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Kaynağa göre veri buluta bağlı hesaplardan geliyor; muhasebeci erişimi ve dışa aktarma yardım merkezinde net belgelenmemiş.

## 8.4 · Veri yüzeyleri, ürün ürün

Dışa aktarma biçimi, verinin nerede durduğu, geri alma katmanı ve içe aktarma.

|  | Hesap Defterim | Money Manager | Bluecoins | Goodbudget | Wallet |
|---|---|---|---|---|---|
| Dışa aktarma biçimi | *Canlı kare* · PDF ve Excel; dönemli ve dönemsiz iki yol | *Canlı kare* · Excel, e-posta eki olarak | *Canlı kare* · PDF veya yazıcı · Excel (.csv) · HTML; dosya üretilmedi | *Görülmedi* · Bu taramada bulunamadı | *Canlı kare* · PDF · XLS · CSV; katlanmış bir menüde |
| Dosya nereye yazılıyor | *Koşum kaydı* · Ürün kasadefteri klasörünü söylüyor, ama bu klasör oluşmuyor | *Canlı kare* · Doğrudan paylaşılıyor; cihaza yazma görülmedi | *Görülmedi* · Ölçülmedi | *Görülmedi* · Ölçülmedi | *Görülmedi* · Ölçülmedi |
| Veri nerede duruyor | *Canlı kare* · Yalnız cihazda; yedek kapalıysa geri yükleme yok | *Canlı kare* · Yedekle ve PC'den Yönet girişleri var | *Canlı kare* · Ayarlarda yedek girişleri var; denenmedi | *Canlı kare* · Hesap tabanlı; Last Backup satırı ekranda | *Canlı kare* · Hesap tabanlı; menüde Bank Sync girişi |
| Silinen kaydın ikinci şansı | *Canlı kare* · Silinmiş işlemler listesi + Geri Yükle | *Canlı kare* · Yok — menü ızgarasında çöp kutusu kalemi yok | *Koşum kaydı* · Çöp kutusu çalışıyor; geri yüklenen kayıt listeye yeniden açılınca geliyor | *Canlı kare* · Tek onayla siliniyor; ayarlarda geri alma yok | *Canlı kare* · Yok — çekmece ve ayarlarda çöp kutusu yok |
| Kayda dosya eklenebiliyor mu | *Canlı kare* · Evet — Kamera / Galeri / PDF; okuma yok | *Canlı kare* · Formda kamera simgesi var | *Canlı kare* · Formun üstünde ataç simgesi; okuma görülmedi | *Görülmedi* · Bu yüzeyde bulunamadı | *Canlı kare* · Kayıt ayrıntısında Add receipt girişi |

- İki ürün verinin cihazda mı bulutta mı durduğunu ekranda söylüyor: biri yedek uyarısıyla, öteki hesap ekranındaki Last Backup satırıyla. Diğer üçünde bu bilgi ayarların içinde kalıyor.
- Dosya eki dört üründe var ama hiçbirinde okunan bir şey değil: ek yalnız saklanıyor, tutar veya tarih ondan çıkarılmıyor.

## 8.5 · Finansal akışın dışındaki araçlar

Üç ürün, para akışıyla doğrudan ilgisi olmayan küçük araçlar taşıyor. Bunlar ürünün kullanıcı hakkındaki varsayımını gösteriyor: kim, nerede, hangi koşulda kullanıyor.

**Şekil 8.12 · Not Defteri** (E0165)

- Dönem çipli bir yapılacaklar listesi.
- Tamamlandı / Beklemede / Toplam sayaçları.
- Finansal kayıtla bağı yok.

**Şekil 8.13 · Nakit Hesap Makinesi** (E0166)

- Kupür × adet tablosu; canlı toplam.
- 200 × 5 = 1.000 satırı görünüyor.
- Kasa sayan kullanıcı için.

**Şekil 8.14 · Seyahat modu** (E0100)

- Çekmecede bir anahtar; Ayarlar ayrı bir kalem. Koşumda kapalı.
- Açıldığında etiket seçici çıkıyor.
- Yolculukta ayrı bir kip varsayımı.

- Nakit Hesap Makinesi bu araçların en anlatıcısı: kupürleri tek tek sayan bir kullanıcı varsayıyor. Gün sonunda kasa sayan esnafın işi, finansal kayıt uygulamasının değil — ama ürün ikisini aynı yere koymuş.
- Money Manager'ın CalcBox ve PC'den Yönet girişleri uygulamanın içinde değil: CalcBox ayrı bir ürünün mağaza sayfasını açıyor, PC'den Yönet ücretli sürüme yükseltme ekranına gidiyor.
- Bluecoins'in seyahat modu anahtarı açılınca bir etiket seçici çıkıyor. Etiket seçildikten sonra ürünün ne yaptığı görülmedi.
- Hesap Defterim menüsünde ayrıca iki ayrı ürüne çapraz tanıtım var: Veresiye Defteri ve Gelir Gider.
- KolayBi'de de finansal olmayan bir araç var: hatırlatıcısı ve görünürlük ayarı olan notlar.

## 8.6 · Verinin uygulamadan çıkış yolu

Kayıt yazıldıktan sonra uygulamadan çıkana kadar üç adım. İlk adımda ürünler zaten ayrışmış oluyor: veri nerede duruyor.

**1. Veri bir yerde duruyor**

**→** *Hesap Defterim* — Yalnız cihazda. Ürün bunu kendisi yazıyor: yedek kapalıysa geri yükleme yok. (E0149)
- *Goodbudget · Wallet* — Hesap tabanlı; veri bir hesaba bağlı ve ekranda yedek ya da senkron izi görünüyor. (E0417)
- *Money Manager · Bluecoins* — Ayarlarda yedek girişleri var; nereye yedeklendiği bu koşumda açılmadı. (E0254)

**2. Dışarı çıkarılıyor**

- *Hesap Defterim* — Dosya olarak cihaza: PDF veya Excel, dönemli ya da dönemsiz. Ürün bir klasör adı söylüyor ama dosya başka yere yazılıyor. (E0427 · E0447)
- *Money Manager* — E-posta eki olarak Excel; dosya cihaza yazılmadan paylaşılıyor. (E0250)
- *Bluecoins · Wallet* — Üç biçim sunuluyor; Wallet'ta form katlanmış bir menüde. Dosya üretilmedi. (E0444 · E0428)

**3. Geri alınabiliyor mu**

- *Hesap Defterim · Bluecoins* — Silinen kayıt ayrı bir listede bekliyor; geri yükleme oradan. (E0143)
**→** *Goodbudget* — Tek onayla siliniyor; ayarlarda kurtarma katmanı yok. (E0125 · E0453)

- Bu bölümün akışı diğerlerinden kısa, çünkü ölçülen şey çoğunlukla davranış değil yüzeyin varlığı: dışa aktarılan dosya yalnız bir üründe açıldı.
- Kaynak taraf bu yolu bambaşka kuruyor: dosya çıkarmak yerine muhasebeciye aynı veriye erişim veriyor. Bu Bölüm 9'da kurulur.

## 8.7 · Neden ayrışıyorlar

Bu bölümde ayrışmanın kaynağı ürünün verinin kime ait olduğu hakkındaki varsayımı: cihazdaki bir defter mi, bir hesabın içeriği mi, yoksa paylaşılacak bir belge mi.

### Cihazdaki defter, verinin sahibini kullanıcı yapıyor *(Çıkarım)*

Hesap Defterim verinin yalnız cihazda durduğunu ekranda yazıyor ve yedek kapalıysa geri yüklemenin mümkün olmadığını söylüyor. Dışa aktarma da bu mantığın devamı: dosya cihaza yazılıyor ve ürün bir klasör adı veriyor — ama o klasör oluşmuyor, dosya başka bir yere yazılıyor.

Bu, kullanıcıyı tam sahibi yapıyor — ve tam sorumlu. Telefon kaybolursa kayıt da kayboluyor ve ürünün yapabileceği bir şey yok. Dosyasını bulmak da kullanıcıya kalıyor: ürünün gösterdiği yerde dosya yok.

Aynı ürünün Silinmiş işlemler listesi ve iki aşamalı kalıcı silme onayı bu sorumluluğun karşılığı: veri kolay kaybolmasın diye ürün içinde iki kapı var.

*Dayanağı: E0149, E0176, E0447, E0143, E0168, koşum kaydı (dosya sistemi)*

### Hesap tabanlı ürünlerde soru hiç sorulmuyor *(Çıkarım)*

Goodbudget ve Wallet'ta veri bir hesaba bağlı; ekranda yedek zamanı ya da senkron girişi görünüyor. Kullanıcı verinin nerede durduğunu sormuyor, çünkü cevap ürünün kendisinde.

Bedeli şu: dışa aktarma bu iki üründe öne çıkan bir yüzey değil. Goodbudget'ta bu taramada hiç bulunamadı; Wallet'ta var ama Ayarlar'da ve kayıt menüsünde değil, çekmecenin katlanmış bir bölümünde. Veri ürünün içinde güvende olduğu için dışarı çıkarmanın aciliyeti azalıyor.

*Dayanağı: E0417, E0275, E0128, E0450, E0428*

### Kaynakta çıktı bir dosya değil, bir belge *(Çıkarım)*

KolayBi'de ekstre üretmeden önce hangi sütunların görüneceği seçiliyor, sonra çıktı önizleniyor ve e-postayla gönderilebiliyor. Yani çıktı bir yedek değil, karşı tarafa gidecek bir belge.

Canlı ürünlerde dışa aktarma bir süzgeç: biçim, dönem ve en çok hesap ile kayıt türü soruluyor. Çıktının kime gideceği ürünün sorusu değil.

Kaynak taraf bunu bir adım daha ileri götürüyor: iki üründe muhasebeciye dosya göndermek yerine aynı veriye erişim veriliyor.

*Dayanağı: E0196, E0197, E0209, E0169, E0428, parasut.com kılavuzu, isbasi.com*

### Dosya eki her yerde var, okunan hiçbir yerde *(Çıkarım)*

Dört üründe kayda dosya veya fotoğraf eklenebiliyor: kamera simgesi, galeri seçimi, PDF ekleme, Add receipt girişi. Hiçbirinde ekten tutar ya da tarih okunmuyor.

Ek bu ürünlerde bir kanıt saklama yeri: kaydı doğrulayan belge duruyor ama kaydı o belge üretmiyor. Fişten kayıt üreten tek yol kaynak metinlerinde anlatılıyor ve canlı hiçbir üründe görülmedi.

*Dayanağı: E0157, E0229, E0294, E0448, E0128*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Hesap Defterim** | Verinin nerede durduğunu ve kaybolabileceğini açıkça yazıyor; iki ayrı dışa aktarma yolu, ekranla aynı tanımı taşıyan bir PDF ve silinen kayıt için ayrı bir liste var | Veri yalnız cihazda: yedek kapalıysa geri yükleme yok ve sorumluluk tamamen kullanıcıda; dışa aktarma uyarısı oluşmayan bir klasörü gösteriyor |
| **Money Manager** | Dışa aktarma rapor ekranından e-posta ekiyle tek adımda | Masaüstünden bağlanma ücretli sürümde; yedek girişinin içeriği yüzeyden okunmuyor |
| **Bluecoins** | Üç biçimde çıktı ve çalışan bir çöp kutusu var | İçe aktarma yalnız CSV ve QIF, banka ekstresi okumuyor; geri yüklenen kayıt listeye ancak uygulama yeniden açılınca geliyor |
| **Goodbudget** | Veri hesaba bağlı; yedek zamanı zarf ekranının üstünde yazılı | Dışa aktarma bu taramada bulunamadı ve silme tek onayla yapılıyor |
| **KolayBi** | Çıktı bir belge: sütunları seçilebiliyor, önizleniyor ve e-postayla gönderilebiliyor | Çıktının biçimi her seferinde soruluyor: tarih aralığı, para birimi, açıklama ve yedi sütun anahtarı |

**Belge 3'e taşınan soru**

- Veri cihazda mı durmalı, hesapta mı? İki model iki ayrı sorumluluk dağılımı üretiyor ve kullanıcı bunu ancak ürün yazarsa öğreniyor.
- Dışa aktarma bir yedek mi, bir belge mi? Kaynak taraf ikisini ayırıyor; canlı ürünlerde çıktı yalnız bir dosya.
- Silinen kaydın ikinci bir şansı olmalı mı? İki üründe var, üçünde taranan yüzeylerde yok.
- Kayda eklenen belge okunmalı mı, yoksa yalnız saklanmalı mı? Canlı beş üründe okuma yok; ek bir kanıt, bir girdi değil.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 8.1 | E0427 | Hesap Defterim | Canlı kare | Üretilen PDF |
| 8.2 | E0176 | Hesap Defterim | Canlı kare | Dosya nereye yazıldı |
| 8.3 | E0254 | Money Manager | Canlı kare | Ayarlar ızgarası |
| 8.4 | E0428 | Wallet | Canlı kare | Dışa aktarma formu |
| 8.5 | E0445 | Bluecoins | Canlı kare | Veri Yönetimi |
| 8.6 | E0149 | Hesap Defterim | Canlı kare | Yedekleme kapalı uyarısı |
| 8.7 | E0143 | Hesap Defterim | Canlı kare | Silinmiş işlemler |
| 8.8 | E0089 | Bluecoins | Canlı kare | Çöp kutusu |
| 8.9 | E0128 | Goodbudget | Canlı kare | Kayıt menüsü |
| 8.10 | E0196 | KolayBi | Kaynak görseli | KolayBi · Cari Ekstresi Oluştur |
| 8.11 | E0197 | KolayBi | Kaynak görseli | KolayBi · PDF önizlemesi |
| 8.12 | E0165 | Hesap Defterim | Canlı kare | Not Defteri |
| 8.13 | E0166 | Hesap Defterim | Canlı kare | Nakit Hesap Makinesi |
| 8.14 | E0100 | Bluecoins | Canlı kare | Seyahat modu |
