# Bölüm 3 · Kart: harcama, taksit, borç, ödeme

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Kartla harcanan para nereye yazılıyor, ne zaman gider sayılıyor ve borç hangi sayıyla gösteriliyor?

Kart, bu araştırmanın ürünleri en çok ayrıştıran konusu. Aynı 6.000'lik harcama bir üründe altı aya bölünmüş altı kayıt, bir üründe bir kayıt artı beş hatırlatıcı, bir üründe tek parça bir borç oluyor.

Bölüm dört soruyu sırayla izliyor: kart üründe ne olarak kuruluyor, kartla harcayınca ne oluşuyor, taksit aylara nasıl dağılıyor ve ödeme yapıldığında hangi sayı düşüyor.

**Bu bölüme girmez**

- Karşı tarafa borç ve tahsilat → Bölüm 4
- Tekrarlayan planın kurulması → Bölüm 5
- Kart ekranlarının görsel düzeni → Belge 1 §5
- Ay raporunun dönem seçimi → Bölüm 7

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare | En derin ürün: kart formu, iki dönem sütunu, taksit zinciri ve kart defteri kareli. |
| Bluecoins | Canlı kare | Kart alanları, taksit kaydı ve kısmi ödeme kareli; dönem davranışı görülmedi. |
| Wallet | Canlı kare | Kart hesabı ve ayarları kareli; taksit alanı görünen bölümde yok. |
| Hesap Defterim | Canlı kare | Kart ayrı bir kavram değil; defter ve jenerik aktarım kareli. |
| Goodbudget | Görülmedi | Kart hesabı ücretsiz pakette açılamadı. |
| KolayBi | Kaynak görseli | Kredi kartı formu ve listesi destek görselinde. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 3.1 · Kart üründe ne olarak kuruluyor

Üç canlı üründe de kartın kendi formu var ve üçü de kesim günü ya da limit soruyor. Ayrışma alanların varlığında değil, o alanların bir dönem üretip üretmediğinde.

**Şekil 3.1 · Kredi Kartı hesap formu** (E0233)

- Hesap Kesim Tarihi ve Son Ödeme Tarihi.
- Altta iki dönem kutusu: Bu Ay 01/08 ~ 31/08
- (Ödeme 01/09) ve Gelecek Ay.

**Şekil 3.2 · Kart hesabı alanları** (E0048)

- Hesap kesim günü ve limit alanları var.
- Bitiş tarihi etiketinin anlamı karede açılmıyor.
- Dönemin nasıl hesaplandığı görülmedi.

**Şekil 3.3 · Edit account · Is Karti** (E0298)

- Type Credit card; limit alanı 0.
- Balance Display Options: Available Credit.
- Payment Due Date: Not set.

- Hesap Defterim'de kart ayrı bir kavram değil. Kart da diğerleri gibi bir defter; kesim günü, limit veya son ödeme tarihi alanı yok ve kart ödemesi jenerik bir aktarım olarak yazılıyor.
- Goodbudget'ta kredi kartı hesabı ücretsiz pakette açılamadı: tek hesap sınırı dolduğu için kart türünde hesap oluşturulamadı. Bu üründe kart modeli hiç görülmedi.
- Money Manager'ın iki dönem kutusu bu bölümün en ayırt edici alanı: kartın borcu tek sayı değil, iki dönem olarak tutuluyor.
- Wallet'ta kartın limiti 0 bırakıldı; Credit sekmesi borç varken kullanımı %0 gösteriyor. Ödeme günü ayın bir günü olarak seçiliyor ve koşumda boş kaldı.

## 3.1b · Kaynakta görünen kart formu

KolayBi'nin destek görseli kart formunun alanlarını adıyla gösteriyor. Canlı üç üründe bulunmayan iki alan burada var: kart limiti ve minimum ödeme oranı. Alanların varlığı kanıtlı; kaydın sonucu görülmedi.

**Şekil 3.4 · KolayBi · Yeni Kredi Kartı formu** (E0208)

- Ad, Kart Numarası, Hesap Kesim Günü ve Son Ödeme Günü zorunlu.
- Kart Limiti ve Minimum Ödeme Oranı (%) ayrı alanlar.
- Açılış Tarihi ve Para Birimi de zorunlu.

- Minimum ödeme oranı canlı dört ürünün hiçbirinde bulunmadı. Kart listesinde ayrıca Kalan Limit kolonu var; limitten kullanılan tutarın düşülmesiyle hesaplandığı çıkarımdır, davranış görülmedi.
- Kart burada finans yüzeyinin altı sekmesinden biri: Banka Hesapları, Kasalar, Kredi Kartları, Online Banka Hesapları, Çekler, Senetler.
- Kart listesinin kolonları: Kredi Kartı Adı, Etiketler, Kart Numarası, Hesap Kesim Günü, Son Ödeme Günü, Kart Limiti, Açılış Tarihi, Kalan Limit.

**Aynı soruda kaynaktan okunan üç ürün**

- *Kaynak beyanı* · **Paraşüt** — Kaynak kasa ve bankanın yanında ayrı bir kart kavramından söz etmiyor; ödeme adımı kasa/banka bakiyesini azaltıyor.
- *Kaynak beyanı* · **Logo İşbaşı** — Kaynak kasa-banka yönetimi ve çek giriş/çıkışı sayıyor; kart dönemi veya ekstre anlatılmıyor.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Kaynağa göre kart bir bağlı hesap: işlemler karttan otomatik iniyor ve sınıflanıyor; kart borcu veya ekstre modeli belgelenmemiş.

## 3.2 · Kartla harcayınca borç nereye yazılıyor

Üç üründe üç ayrı yer: dönemli bir borç sütunu, eksiye inen bir hesap bakiyesi ve defterin yürüyen dengesi. Harcamanın ayın giderine de girdiği Money Manager, Bluecoins ve Hesap Defterim'de ölçüldü.

**Şekil 3.5 · Hesaplar · ödeme öncesi** (E0236)

- Is Karti: Bu Ay 1.200 (kırmızı) / Gelecek Ay 0.
- Borçlar 1.200; Varlıklar 46.150; Toplam 44.950.
- Borç iki dönem sütununda duruyor.

**Şekil 3.6 · Account Detail · Is Karti** (E0297)

- Kart bir hesap; TODAY −6.000,00.
- Grafik bugün −6k'ya iniyor.
- Dönem yok; borç tek parça bakiye.

**Şekil 3.7 · Is Karti defteri** (E0153)

- Kart sıradan bir defter; Denge −1.000.
- Toplamlar 1.600 / 2.200 / −600.
- Kart ödemesi jenerik aktarım bacağı.

- Wallet kart bakiyesi eksiye inince bir uyarı gösterdi: "Your balance on Is Karti dropped below the minimum threshold." Eşik hesabın kendi formunda: en az bakiye bildirimi açık ve tutarı 0. Uyarı kart mantığından değil, her hesapta bulunan bu bildirimden geliyor.
- Bluecoins'te kart harcaması dönem raporunun giderini 1.000 artırdı; kart borcu ile gider aynı anda yazılıyor. Kartın dönem davranışı bu koşumda görülmedi.
- Money Manager'da kart harcaması ayın gider toplamına da giriyor: Ağustos'un 2.050'sinin 1.200'ü karttan.
- Bluecoins'te taksit kaydından sonra dönem gideri 3.050 okundu.

## 3.3 · Taksit: bir plan mı, altı kayıt mı, hiç mi

Aynı 6.000'lik taksitli harcama üç üründe üç ayrı şeye dönüşüyor. Ayrışma formda başlıyor: ikisinde taksit sayısı sorulan bir alan, birinde kaydın görünen bölümünde böyle bir alan yok.

**Şekil 3.8 · Gider formu · 6 Ay rozeti** (E0245)

- 6.000, Is Karti, "6 Ay" rozeti formda.
- Tekrar/Taksit menüsünden seçiliyor.

**Şekil 3.9 · Listede ilk taksit** (E0246)

- "Tasarim ekipmani (1/6)" 1.000 olarak düşüyor.
- Ağustos gideri 2.050 → 3.650.
- Kayıt 6.000 değil, 1.000 yazıyor.

**Şekil 3.10 · Kalan beş taksit** (E0037)

- İlk taksit gerçek kayıt oldu.
- Kalan beşi hatırlatıcı listesinde bekliyor.
- Liste niyeti gösteriyor, kaydı değil.

**Şekil 3.11 · Kayıt ayrıntısı** (E0294)

- 6.000 tek kayıt olarak durdu.
- Görünen bölümde taksit alanı yok.
- Payment Type Cash olarak kalmış.

- Money Manager taksidi kaydın bir özelliği yapıyor: tek form, altı ayrı satır. Bluecoins ilk taksidi kayda, kalanını hatırlatıcıya yazıyor — yani yarısı gerçek, yarısı niyet. Wallet'ta 6.000 bölünmeden tek parça kaldı.
- Hesap Defterim'de taksit planı kavramı yok; kullanıcı ya tek 6.000 kaydı ya da elle altı ayrı kayıt açıyor ve "6 taksit" bilgisi hiçbir yere taşınmıyor.
- Bluecoins'in taksit şartları sayfasında oran alanı var; oranın faiz mi yüzde mi olduğu karede açılmıyor.
- Bluecoins'te kalan taksitler kendiliğinden kayda dönüşmüyor: hatırlatıcıya dokununca Kaydet ve Düzenle çıkıyor, taksit ancak Kaydet ile gerçekleşiyor; ileri tarihli taksitte bugüne mi taksit gününe mi yazılacağı soruluyor. Bu bir kullanıcı kontrolüyle kayıtlı; karesi yok.

## 3.4 · Taksit aylara nasıl düşüyor (Money Manager)

Taksit zinciri tek üründe uçtan uca izlenebildi. Dört ay ileri gidildiğinde her ayın gideri tam olarak o ayın taksidi kadar; zincir altıncı taksitte bitiyor ve sonraki ay boşalıyor.

**Şekil 3.12 · Taksit 3/6** (E0407)

- Gider 1.000,00.
- 15 Ekim: Tasarim ekipmani (3/6).

**Şekil 3.13 · Taksit 5/6** (E0409)

- Gider 1.000,00.
- Aynı tutar, aynı gün.

**Şekil 3.14 · Taksit 6/6** (E0410)

- Zincirin son taksidi.
- Gider yine 1.000,00.

**Şekil 3.15 · Zincir bitti** (E0411)

- Gelir/Gider/Toplam 0,00 — Veri yok.
- Yalnız tekrarlayan plan önizlemesi kalıyor.

- Altı taksit altı aya eşit dağılıyor ve her ay yalnız kendi taksidini gider yazıyor. Harcamanın tamamı ilk aya yazılmıyor; 6.000'in aylık görünümü 1.000.
- Ekim ve sonrasındaki aylarda listenin üstünde ayrı bir tekrarlayan plan bölümü duruyor ve o bölüm ayın toplamına girmiyor — plan ile gerçekleşmiş kayıt ayrı tutuluyor. Bu ayrımın kendisi Bölüm 5'te.

## 3.5 · Aynı kart olayları, beş üründe sonuç

Kartla harcama, taksit ve ödeme üç ayrı olay; her biri farklı bir sayıyı kıpırdatıyor. Tabloda ürünlerin bu üç olaya verdiği cevaplar yan yana.

|  | Money Manager | Bluecoins | Wallet | Hesap Defterim | Goodbudget |
|---|---|---|---|---|---|
| Kartın dönemi var mı | *Canlı kare* · Evet — Bu Ay / Gelecek Ay iki sütun | *Canlı kare* · Alanlar var; dönem davranışı görülmedi | *Canlı kare* · Hayır — tek parça bakiye | *Canlı kare* · Hayır — kart kavramı yok | *Görülmedi* · Kart hesabı açılamadı |
| Kart harcaması gider sayılıyor mu | *Canlı kare* · Evet — Ağustos giderinin 1.200'ü karttan | *Canlı kare* · Evet — dönem gideri 1.000 arttı | *Çıkarım* · Evet — 30 günlük gider 21.600'ün 6.000'i kart harcaması | *Canlı kare* · Evet — defterde Ödendi satırı | *Görülmedi* · Ölçülemedi |
| Taksit ne üretiyor | *Canlı kare* · Altı ayrı kayıt, aylara bölünmüş | *Canlı kare* · Bir kayıt + beş hatırlatıcı | *Canlı kare* · Bölünmüyor; 6.000 tek parça | *Canlı kare* · Taksit kavramı yok; elle girilir | *Görülmedi* · Ölçülemedi |
| Kısmi ödeme neyi düşürüyor | *Canlı kare* · Yalnız Bu Ay: 1.000 → 600 | *Canlı kare* · Kart bakiyesi −1.000 → −500 | *Canlı kare* · Tek parça bakiye: −5.600 → −5.300 | *Canlı kare* · Denge −1.000 → −600 | *Görülmedi* · Ölçülemedi |
| Ödeme gider sayılıyor mu | *Canlı kare* · Hayır — ayrı satır: Gider (Kredi Kartı, Ödeme) | *Canlı kare* · Hayır — aktarım; net 43.350 korunuyor | *Canlı kare* · Hayır — aktarım; 30 günlük gider 21.600'de kaldı | *Canlı kare* · Hayır — jenerik aktarım bacağı | *Görülmedi* · Ölçülemedi |

- Ödemenin gider sayılmaması ölçülen dört üründe de aynı: ödeme parayı taşıyor, harcama zaten kart kullanıldığı gün gider yazılmıştı. İkisini birden saymak aynı harcamayı iki kez gider yapardı.

## 3.6 · Kısmi ödeme: hangi sayı düşüyor, hangisi durur

Kart borcunun bir kısmı ödendiğinde dört üründe de net varlık değişmiyor — para yer değiştiriyor. Ayrışma hangi sayının düştüğünde: bir üründe yalnız kesilmiş dönem, üçünde tek parça bakiye.

**Şekil 3.16 · Ödeme formu** (E0248)

- Tutar 400; Kaynak Ana Hesap, Giriş Is Karti.
- Ödeme düğmesi formu ön dolduruyor,
- tutar serbestçe değiştirilebiliyor.

**Şekil 3.17 · Ödeme sonrası hesaplar** (E0249)

- Is Karti Bu Ay 1.000 → 600.
- Gelecek Ay 1.000 değişmedi.
- Toplam 41.750 ödeme öncesiyle aynı.

**Şekil 3.18 · 500 kısmi ödeme** (E0050)

- Kart −1.000 → −500; banka 40.200 → 39.700.
- Net 43.350 korunuyor.

**Şekil 3.19 · Toplam sekmesi · Eylül** (E0405)

- Gider (Nakit, Banka Hesapları) 600,00.
- Gider (Kredi Kartı, Ödeme) 1.000,00 (400,00).
- Ödenen tutar parantezde, gidere girmiyor.

- Money Manager ödemeyi yalnız kesilmiş döneme yazıyor: Bu Ay 1.000'den 600'e indi, Gelecek Ay'daki 1.000 yerinde kaldı. Ödemenin hangi döneme sayılacağı sorusuna ürün açık bir cevap vermiş.
- Aynı ürünün Toplam sekmesi gideri ödeme kaynağına göre ayırıyor ve kart ödemesini gider satırının içine değil, parantezine koyuyor.
- Hesap Defterim'de kısmi ödeme jenerik bir aktarım: Denge −1.000'den −600'e indi ve ekstre veya asgari tutar kavramı devreye girmedi.
- Wallet'ta kısmi ödeme hesaptan karta bir aktarım: 300 ödendi, kart −5.600'den −5.300'e indi, 30 günlük gider yerinde kaldı. Kart bakiyesi tek parça olduğu için ödemenin hangi döneme yazılacağı sorusu üründe oluşmuyor.

## 3.7 · Aynı kart, aynı an, iki farklı borç sayısı

Money Manager'ın iki ekranı aynı kart için iki ayrı borç gösteriyor: Hesaplar 1.600 derken kart defterinin Ocak satırı 5.600 diyor. İkisi de doğru — ama hangisinin ne anlattığı ekranda yazmıyor.

**Şekil 3.20 · Borçlar 1.600** (E0414)

- Is Karti: Bu Ay 600 / Gelecek Ay 1.000.
- Borçlar 1.600; Toplam 42.350.

**Şekil 3.21 · Bakiye 2.600** (E0412)

- Çekme 1.000; dönem toplamı −1.000.
- Yürüyen bakiye ileriye uzuyor.

**Şekil 3.22 · Bakiye −5.600** (E0413)

- Zincirin son taksidi işlenmiş hâli.
- Aynı kart, aynı an, farklı sayı.

- Aradaki 4.000 tam olarak Ekim, Kasım, Aralık ve Ocak taksitlerinin toplamı. Hesaplar ekranı kesilmiş dönem ile bir sonraki dönemi topluyor; kart defteri ise gelecekte yazılmış bütün taksitleri yürüyen bakiyeye katıyor.
- İki yüzey farklı dönem kapsamı taşıyor ve bu kapsam hiçbir ekranda yazılı değil. Kullanıcı "kartıma ne kadar borcum var" sorusuna baktığı ekrana göre iki ayrı cevap alıyor.

## 3.8 · Kart parasının yolu ve ayrıldığı noktalar

Kart kurulduğu andan borç ödendiği ana kadar dört adım. İlk adımdaki tercih sonraki üç adımın hepsini belirliyor.

**1. Kart kuruluyor**

**→** *Money Manager* — Dönemli hesap: kesim günü ve son ödeme günü iki dönem kutusu üretiyor — Bu Ay ve Gelecek Ay. (E0233)
- *Bluecoins* — Alanlar var (kesim günü, limit) ama dönem davranışı bu koşumda görülmedi. (E0048)
- *Wallet · Hesap Defterim* — Dönemsiz: kart sıradan bir hesap veya sıradan bir defter. (E0297)

**2. Kartla harcanıyor**

Dört üründe de harcama aynı anda iki şey yapıyor: kartın borcunu artırıyor ve ayın giderine giriyor. Gider harcandığı gün yazılıyor, ödendiği gün değil.


**3. Taksitse ne oluyor**

**→** *Money Manager* — Altı aya bölünüp altı kayıt olarak yazılıyor; her ay yalnız kendi taksidini gider sayıyor. (E0410)
- *Bluecoins* — İlk taksit kayıt, kalan beşi hatırlatıcı — yarısı gerçekleşmiş, yarısı niyet. (E0037)
- *Wallet · Hesap Defterim* — Bölünmüyor. Tutarın tamamı tek kayıt olarak tek aya yazılıyor. (E0294)

**4. Borç ödeniyor**

**→** *Money Manager* — Yalnız kesilmiş dönem düşüyor: Bu Ay 1.000 → 600, Gelecek Ay yerinde kalıyor. (E0249)
- *Bluecoins · Wallet · Hesap Defterim* — Tek parça bakiye düşüyor; ödeme jenerik bir aktarım. (E0050 · E0438)
- *Ölçülen dört üründe de* — Ödeme gider sayılmıyor — aynı harcama iki kez gidere girmiyor. (E0405)

- Zincirin tamamı tek üründe koşulabildi. Diğerlerinde adımların bir kısmı ürünün kavramı olmadığı için değil, o kavram hiç bulunmadığı için boş kalıyor.
- Goodbudget bu yolun hiçbir adımında yok: ücretsiz pakette kart hesabı açılamadı.

## 3.9 · Neden ayrışıyorlar

Kartta ayrışmanın kaynağı tek bir soruya verilen cevap: kartın bir dönemi var mı. Dönemi olan ürün borcu ikiye bölmek, taksidi aylara yazmak ve ödemeyi bir döneme saymak zorunda; dönemi olmayan ürün bunların hiçbirini yapmıyor.

### Dönem varsa borç tek sayı olmaktan çıkıyor *(Çıkarım)*

Money Manager kesim günü ve son ödeme gününü bir dönem kutusuna çeviriyor ve kartın borcunu Bu Ay ile Gelecek Ay olarak ikiye bölüyor. Taksitli harcamada iki taksit iki döneme düştü, kalan dördü borçta hiç görünmedi.

Wallet ve Hesap Defterim'de dönem yok; borç tek parça. Wallet'ta 6.000 tek seferde kart bakiyesine yazıldı, Hesap Defterim'de defterin yürüyen dengesine. İkisinde de "bu ay ne ödemem gerekiyor" sorusunun ekranda karşılığı yok.

Bluecoins ikisinin arasında: alanlar var, davranış bu koşumda görülmedi.

*Dayanağı: E0233, E0247, E0297, E0153, E0048*

### Aynı kart için iki borç sayısı, ikisi de doğru *(Çıkarım)*

Money Manager'ın Hesaplar ekranı 1.600 derken aynı kartın defteri Ocak satırında 5.600 gösteriyor. Aradaki 4.000 tam olarak Ekim, Kasım, Aralık ve Ocak taksitlerinin toplamı.

Yani iki ekran iki farklı soruyu cevaplıyor: Hesaplar "şu an ve bir sonraki dönemde ne borçlusun", defter "bugüne kadar yazılmış bütün taksitlerle nereye varıyorsun". İkisi de kendi tanımına göre doğru.

Sorun sayıların yanlışlığı değil, kapsamın hiçbir ekranda yazmaması. Kullanıcı hangi ekrana baktığına göre iki ayrı cevap alıyor ve ikisinin neden farklı olduğunu ekrandan anlayamıyor.

*Dayanağı: E0414, E0412, E0413, E0410*

### Taksit yalnız bir üründe kendiliğinden aylık gider eğrisine dönüşüyor *(Çıkarım)*

Aynı 6.000, Money Manager'da altı ayın her birine 1.000 olarak yazıldı ve altıncı taksitten sonraki ay boşaldı. Bluecoins'te yalnız ilk 1.000 gider oldu; kalan beşi hatırlatıcıda bekliyor ve ancak kullanıcı her birini kaydettikçe gidere giriyor. Wallet ve Hesap Defterim'de 6.000'in tamamı tek aya yazıldı.

Sonuç: aynı harcama üç üründe üç ayrı aylık gider eğrisi üretiyor. Bir üründe altı ay boyunca kendiliğinden 1.000; birinde kullanıcı her ay kaydettiği sürece 1.000, kaydetmediği ay sıfır; birinde bir ay 6.000 sonra sıfır.

Bu fark yalnız görünümü değil, ayın gider toplamıyla alınan her kararı etkiliyor.

*Dayanağı: E0246, E0407, E0410, E0411, E0037, E0294, kullanıcı kontrolü (Bluecoins)*

### Ödeme dört üründe de gider değil *(Çıkarım)*

Ödemenin sonucu ölçülebilen dört üründe de aynı: kart ödemesi ayın giderine girmedi. Money Manager bunu ekranda açıkça ayırıyor — gider satırı ödeme kaynağına göre bölünmüş ve ödenen tutar gider rakamının içine değil, parantezine yazılmış.

Gerekçe dördünde de aynı: harcama kart kullanıldığı gün zaten gider yazıldı. Ödemeyi de gider saymak aynı parayı iki kez saymak olurdu.

*Dayanağı: E0405, E0050, E0153, E0249, E0438*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Money Manager** | Kartın dönemi var: borç Bu Ay / Gelecek Ay olarak bölünüyor, taksit aylara dağılıyor ve ödeme yalnız kesilmiş döneme yazılıyor; gider ödeme kaynağına göre ayrı okunabiliyor | Aynı kart için iki ekran iki farklı borç gösteriyor ve hangisinin hangi dönemi kapsadığı hiçbir yerde yazmıyor |
| **Bluecoins** | Taksit formda bir alan ve kalan taksitler hatırlatıcı listesinde görünür kalıyor; kısmi ödeme net varlığı bozmuyor | Taksitin yalnız ilki gerçek kayıt; kalan beşi kendiliğinden gerçekleşmiyor ve her biri elle kaydediliyor — kaydedilmeyen taksit gidere hiç girmiyor |
| **Wallet** | Kart bir hesap olduğu için borç tek bakışta okunuyor; Available Credit gösterimi ve ödeme günü alanı kart hesabına özgü | Dönem yok ve taksit bölünmüyor: 6.000 tek aya yazılıyor, "bu ay ne ödemeliyim" sorusunun karşılığı bulunmuyor |
| **Hesap Defterim** | Kart da diğer defterler gibi çalıştığı için öğrenilecek ayrı bir kavram yok; kısmi ödeme doğal olarak serbest tutarlı | Kesim günü, limit, ekstre ve taksit kavramlarının hiçbiri yok; kartın diğer hesaplardan farkı yalnız adı |
| **Goodbudget** | — | Ücretsiz pakette tek hesap açılabiliyor; banka hesabının yanına kart eklenemediği için kart kullanan biri bu pakette kart borcunu ayrı izleyemiyor |

**Belge 3'e taşınan soru**

- Kart borcu tek sayı mı olmalı, dönemlere bölünmüş mü? Dönem, "bu ay ne ödemeliyim" sorusunu cevaplıyor ama iki ekranın iki farklı sayı göstermesi riskini getiriyor.
- Bir sayının hangi dönemi kapsadığı ekranda yazmalı mı? Money Manager'ın 1.600 ile 5.600'ü aynı anda doğru; ayrımı kullanıcı göremiyor.
- Taksit bir plan mı, önceden yazılmış kayıtlar mı? Üç ürün üç ayrı cevap veriyor ve cevap doğrudan aylık gider eğrisini değiştiriyor.
- Kart ödemesi hiçbir üründe gider sayılmıyor — bu ortak davranışın istisnası var mı, yoksa kural mı?


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 3.1 | E0233 | Money Manager | Canlı kare | Kredi Kartı hesap formu |
| 3.2 | E0048 | Bluecoins | Canlı kare | Kart hesabı alanları |
| 3.3 | E0298 | Wallet | Canlı kare | Edit account · Is Karti |
| 3.4 | E0208 | KolayBi | Kaynak görseli | KolayBi · Yeni Kredi Kartı formu |
| 3.5 | E0236 | Money Manager | Canlı kare | Hesaplar · ödeme öncesi |
| 3.6 | E0297 | Wallet | Canlı kare | Account Detail · Is Karti |
| 3.7 | E0153 | Hesap Defterim | Canlı kare | Is Karti defteri |
| 3.8 | E0245 | Money Manager | Canlı kare | Gider formu · 6 Ay rozeti |
| 3.9 | E0246 | Money Manager | Canlı kare | Listede ilk taksit |
| 3.10 | E0037 | Bluecoins | Canlı kare | Kalan beş taksit |
| 3.11 | E0294 | Wallet | Canlı kare | Kayıt ayrıntısı |
| 3.12 | E0407 | Money Manager | Canlı kare | Taksit 3/6 |
| 3.13 | E0409 | Money Manager | Canlı kare | Taksit 5/6 |
| 3.14 | E0410 | Money Manager | Canlı kare | Taksit 6/6 |
| 3.15 | E0411 | Money Manager | Canlı kare | Zincir bitti |
| 3.16 | E0248 | Money Manager | Canlı kare | Ödeme formu |
| 3.17 | E0249 | Money Manager | Canlı kare | Ödeme sonrası hesaplar |
| 3.18 | E0050 | Bluecoins | Canlı kare | 500 kısmi ödeme |
| 3.19 | E0405 | Money Manager | Canlı kare | Toplam sekmesi · Eylül |
| 3.20 | E0414 | Money Manager | Canlı kare | Borçlar 1.600 |
| 3.21 | E0412 | Money Manager | Canlı kare | Bakiye 2.600 |
| 3.22 | E0413 | Money Manager | Canlı kare | Bakiye −5.600 |
