# Bölüm 2 · Hesaplar ve para aktarımı

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Para hesaplar arasında gezdiğinde ne üretiyor — bir kayıt mı, iki mi, hiç mi?

Bu bölüm iki soruyu izliyor. Birincisi hesabın başlangıç parasının nereye yazıldığı; ikincisi aynı paranın bir hesaptan diğerine geçerken kaç satır ürettiği ve o satırların ayın toplamına ne yaptığı.

İkisi birbirine bağlı: açılış bakiyesini bir kayıt olarak yazan ürün, o kaydı ayın toplamına da sokma riskini alır. Bölüm 1'de görülen 51.200'ün kaynağı burasıdır.

**Bu bölüme girmez**

- Kart borcu ve kart ödemesi → Bölüm 3
- Karşı tarafa borç ve tahsilat → Bölüm 4
- Hesap listesinin görsel düzeni → Belge 1 §5
- Raporun dönem seçimi → Bölüm 7

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare | Açılış diyaloğu, defter karşılığı ve toplam anahtarı kareli. |
| Bluecoins | Canlı kare | Aktarım formu ve listedeki iki bacak kareli. |
| Wallet | Canlı kare | Aktarımın listedeki izi kareli; açılış alanı bulunamadı. |
| Hesap Defterim | Canlı kare, Koşum kaydı | Aktar formu kareli; bacak bağımsızlığı koşum kaydı. |
| Goodbudget | Canlı kare | Hesap kurulumu kareli; aktarım ücretli pakette. |
| KolayBi | Kaynak görseli | Finans sekmeleri destek görselinde. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 2.1 · Açılış bakiyesi bir alan mı, bir kayıt mı

Hesabın başlangıç parası dört üründe dört ayrı yere yazılıyor. Money Manager bunu kullanıcıya açıkça soruyor; Hesap Defterim defterin ilk satırı yapıyor; Goodbudget hesabın bir özelliği olarak tutuyor. Wallet'ta böyle bir alan aranıp bulunamadı.

**Şekil 2.1 · Hesap formu ve sorulan soru** (E0239)

- Tutar 2.000 ve Toplama Dahil Et anahtarı.
- Kaydedince çıkan diyalog soruyor:
- fark İşlemler'de gösterilsin mi?

**Şekil 2.2 · Aynı açılışın defterdeki karşılığı** (E0240)

- Para Yatırma 2.000 / Çekme 0.
- Açılış defterde bir satır olarak duruyor.

**Şekil 2.3 · Hesap Eklem** (E0145)

- Açılış bilançosu [İsteğe bağlı].
- + / − radyosu: açılış eksi de olabiliyor.
- Tarih alanı açılışa ait.

**Şekil 2.4 · Ana Hesap kuruldu** (E0113)

- Açılış 20.000 hesabın kendi değeri.
- İşlem listesinde ayrı bir satır olarak görünmüyor.

- Bluecoins hesap açılışlarını işlem listesine ayrı satırlar olarak yazmış; açılış satırlarının düştüğü günün başlığı bu yüzden sıfır çıkmıyor.
- Wallet'ta açılış bakiyesi alanı yok. Dördüncü hesap ücretsiz pakette açılamadığı için mevcut hesabın düzenleme formu tarandı: renk, içe aktarma e-postası, istatistikten çıkarma, arşiv, en az ve en çok bakiye bildirimi var; açılış bakiyesi yok.
- Hesap Defterim'de açılış defterin ilk satırıdır ve Ağustos listesinde 20.000 olarak görünüyor; ayın Alındı toplamına da bu yüzden giriyor.
- Money Manager'ın sorduğu soru bir tercih: açılış farkı isteğe bağlı olarak işlem listesinde gösteriliyor.

## 2.2 · Aktarım formu: aynı iş, beş ayrı form

Beş üründe de aktarım birinci sınıf: üçünde işlem formunun üçüncü türü, ikisinde kendi ekranı. Ayrışma formun sorduğu alanlarda — ikisi aktarım ücreti soruyor, biri zarf sormuyor, ikisi yalnız iki hesap ve tutar istiyor.

**Şekil 2.5 · Havale formu** (E0462)

- Havale, Gelir ve Gider'in yanında üçüncü tür.
- Kaynak ve Giriş; tutarın yanında Harç.
- Kategori alanı yok.

**Şekil 2.6 · Transfer formu** (E0435)

- Kaynak ve hedef hesap.
- Transfer ücreti ayrı blok:
- kendi tutarı, hesabı ve kategorisi.

**Şekil 2.7 · Aktarım formu** (E0436)

- Income / Expense / Transfer tek form.
- From account → To account.
- Ücret veya kategori alanı yok.

**Şekil 2.8 · Aktar formu** (E0147)

- Miktar, Kimden, Kime, tarih, notlar.
- Tek form; sonuç iki ayrı defter satırı.
- Ayrı bir Aktar menü girişi var.

**Şekil 2.9 · Account Transfer** (E0119)

- From · To · Amount · Description · Date.
- Açıklama otomatik dolduruluyor.
- Zarf alanı yok — aktarım zarfa uğramıyor.

- Goodbudget'ta aktarım birinci sınıf ve ayrı bir ekran, ama formda zarf alanı bulunmuyor. Bölüm 1'de zarf ile hesabın iki ayrı defter olduğu görülmüştü; aktarım bu iki defterden yalnız hesabı ilgilendiriyor.
- Ücretsiz pakette tek hesap açılabildiği için gerçek aktarım koşulamadı: Kimden ve Kime açılırları aynı tek kaydı listeliyor.
- Bluecoins'te ücret aktarımın parçası değil, kendi hesabı ve kategorisi olan ayrı bir gider olarak soruluyor. Ücretli bir aktarım kaydedilmedi; iki bakiyeye etkisi ölçülmedi.
- Goodbudget'ta aktarımın engellendiği an kareyle kayıtlı.

## 2.3 · 3.000 bir hesaptan diğerine geçince ne oldu

Aynı aktarımın dört üründeki sonucu; beşincisinde aktarım hiç çalıştırılamadı. Toplam para dördünde de aynı kalıyor — ayrışma satır sayısında ve ayın toplamında.

|  | Money Manager | Bluecoins | Wallet | Hesap Defterim | Goodbudget |
|---|---|---|---|---|---|
| Toplam paraya etkisi | *Canlı kare* · Yok — Toplam 44.950 | *Canlı kare* · Yok — net 44.950 | *Canlı kare* · Yok — dönem toplamı 22.950 sabit | *Canlı kare* · Yok — Denge 44.950 | *Görülmedi* · Aktarım koşulamadı; tek hesap sınırı |
| Listede kaç satır | *Canlı kare* · Tek Havale satırı, gün başlığı 0/0 | *Canlı kare* · İki bacak, gün neti 0 | *Canlı kare* · İki satır: biri yeşil artı, biri kırmızı eksi | *Canlı kare* · İki satır: Kime X / Kimden Y | *Görülmedi* · Tek hesap sınırı; aktarım kurulamadı |
| Ayın gelir/gider toplamı | *Canlı kare* · Etkilenmedi | *Canlı kare* · Etkilenmedi | *Canlı kare* · Etkilenmedi | *Canlı kare* · Her iki bacak da toplama girdi: +3.000 ve −3.000 | *Görülmedi* · Tek hesap sınırı; ölçülemedi |
| Aktarım ayrı bir tür mü | *Canlı kare* · Evet — Havale, Gelir/Gider'in yanında | *Canlı kare* · Evet — TRANSFER sekmesi | *Canlı kare* · Evet — ekleme menüsünde ayrı giriş; formun Transfer sekmesi | *Canlı kare* · Evet — ayrı Aktar ekranı | *Canlı kare* · Evet — ayrı Account Transfer ekranı |
| Form ne soruyor | *Canlı kare* · Kaynak · Giriş · tutar · Harç · not | *Canlı kare* · Kaynak · hedef · tutar · transfer ücreti | *Canlı kare* · From · To · tutar | *Canlı kare* · Kimden · Kime · miktar · not | *Canlı kare* · From · To · tutar · açıklama — zarf yok |

- Money Manager aktarımı listede tek bir Havale satırı olarak gösteriyor ve o günün gelir/gider başlığını 0/0 yazıyor; Bluecoins ve Wallet iki bacağı ayrı ayrı gösterip aynı günde netliyor. Üç ürün de aynı sonuca farklı yoldan varıyor.

## 2.4 · İki bacak listede: netleşen üç ürün, bağımsız duran bir ürün

Aktarımın listedeki izi ürünün toplamı nasıl koruduğunu da gösteriyor. Üç üründe iki bacak aynı günde birbirini götürüyor; birinde iki satır birbirinden habersiz duruyor.

**Şekil 2.10 · İşlem listesi · Ağustos** (E0022)

- 3.000 aktarım ve 1.200 kart ödemesi ikişer bacak.
- İki bacağın olduğu günlerin neti 0.

**Şekil 2.11 · Records** (E0285)

- Aynı tarih ve notla iki satır.
- Biri yeşil artı, biri kırmızı eksi.
- Dönem toplamı 22.950 değişmiyor.

**Şekil 2.12 · Bütün Hesaplar** (E0140)

- Kime Is Karti / Kimden Ana Hesap iki ayrı satır.
- İkisi de Alındı ve Ödendi toplamlarına giriyor.
- Toplam Alındı 51.200 / Ödendi 6.250.

- Hesap Defterim'de iki bacak birbirine bağlı değil. Koşumda bir bacak silindi, karşı bacak silinmeden kaldı ve net varlık sessizce yanlışa döndü. Bu bir koşum kaydıdır: kare yalnız tek defteri gösteriyor, silme anı kareye alınmadı.
- Aynı şekilde bir bacağın tutarı veya tarihi düzenlendiğinde diğerinin güncellenmediği koşumda görüldü; bu da kareyle değil koşumla kayıtlı.

**Aynı soruda kaynaktan okunan dört ürün**

- *Kaynak görseli* · **KolayBi** — Finans altı sekmeye bölünmüş: Banka Hesapları, Kasalar, Kredi Kartları, Online Banka Hesapları, Çekler, Senetler. Kasa ile banka ayrı iki kavram; canlı beşte ikisi de aynı hesap listesinde durur.
- *Kaynak beyanı* · **Paraşüt** — Kaynağa göre kasa ve banka ayrı tutuluyor ve ödeme adımı kasa/banka bakiyesini azaltıyor; banka entegrasyonu mutabakatı otomatikleştiriyor.
- *Kaynak beyanı* · **Logo İşbaşı** — Kaynağa göre tek kayıt cari, kasa-banka ve stok defterlerini birlikte güncelliyor; nakit tahsilat ve banka bilgisi telefondan eklenebiliyor.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Kaynakta hesaplar arası aktarım için ayrı bir model belgelenmemiş; ürün çift taraflı muhasebe yapmıyor.

## 2.5 · Hesabı toplamdan çıkarmak: aynı para, iki net varlık

Money Manager hesap formunda bir anahtar taşıyor. Anahtar kapatıldığında hesap listede kalıyor ama net varlığa girmiyor — aynı veriyle iki farklı toplam okunuyor.

**Şekil 2.13 · Toplama Dahil Et açık** (E0252)

- Hesap Bilgisi formunda iki anahtar.
- Toplama Dahil Et ve Göster/Gizle ayrı şeyler.
- Ortak Cuzdan tutarı 4.150.

**Şekil 2.14 · Anahtar kapalıyken hesaplar** (E0253)

- Varlıklar 39.800 / Toplam 38.200.
- Nakit grubu 0,00 siyah; Ortak Cuzdan 4.150 gri.
- Hesap listede duruyor ama toplamda yok.

- Anahtar açıkken aynı ekran Varlıklar 44.950 / Toplam 44.950 diyordu. Fark tam olarak dışarıda bırakılan hesabın 4.150'si ve kart borcunun 1.600'ü kadar; iki sayı da aynı veriden okunuyor.
- Bluecoins'te hesap listesinin üstündeki Nakit Akım Ayarı hesap başına bir anahtar. Ekranın kendi cümlesi: "Nakit akışı hesaplarken kullanılacak nakit hesapları seçiniz." Anahtarı değiştirip raporun değiştiği ölçülmedi.
- Gri gösterim hesabın pasif olduğunu değil, toplama girmediğini anlatıyor; hesap silinmemiş, listede duruyor.

## 2.6 · Paranın hesaplar arasındaki yolu

Hesap kurulduğu andan aktarımın toplamlara yansıdığı ana kadar dört adım. İlk çatallanma daha hesap açılırken oluyor.

**1. Hesap açılıyor**

Beş üründe de hesabın adı ve türü soruluyor. Ayrışma bir sonraki adımda, başlangıç parasının nereye yazıldığında başlıyor.


**2. Açılış parası nereye yazılıyor**

**→** *Money Manager · Bluecoins · Hesap Defterim* — Deftere bir kayıt olarak. Money Manager bunu kullanıcıya soruyor; Hesap Defterim listenin ilk satırı yapıyor. (E0239)
- *Goodbudget* — Hesabın kendi değeri olarak; işlem listesinde ayrı satır yok. (E0113)
- *Wallet* — Hesap formunda böyle bir alan yok; düzenleme formu tarandı. (E0437)

**3. Aktarım kaç satır üretiyor**

- *Money Manager* — Tek Havale satırı; iki hesabı da o satır taşıyor. (E0228)
- *Bluecoins · Wallet* — İki bacak, aynı gün içinde birbirini götürüyor. (E0279)
**→** *Hesap Defterim* — İki bağımsız satır; biri silinince diğeri kalıyor. (E0140)

**4. Ayın toplamına ne oluyor**

- *Money Manager · Bluecoins · Wallet* — Hiçbir şey. Aktarım gelir/gider toplamına girmiyor. (E0231)
**→** *Hesap Defterim* — Her iki bacak da giriyor: Alındı +3.000, Ödendi −3.000. (E0142)

- İki çatallanma da aynı tercihten doğuyor: ürün defterine ne kadar çok şeyi kayıt olarak yazarsa, o kayıtları toplamların dışında tutmak için o kadar çok kurala ihtiyaç duyuyor.
- Goodbudget bu yolun üçüncü adımında hiç görünmüyor; ücretsiz pakette tek hesap açılabildiği için aktarım koşulamadı.

## 2.7 · Neden ayrışıyorlar

Bu bölümün iki sorusu tek bir tercihte birleşiyor: ürün neyi kayıt sayıyor. Kayıt sayılan her şey deftere girer, ve deftere giren her şeyin toplamlardan nasıl çıkarılacağı ayrı bir karar hâline gelir.

### Açılış bakiyesi bir kayıt olunca ayın toplamına da giriyor *(Çıkarım)*

Üç üründe açılış parası deftere bir satır olarak yazılıyor. Money Manager bunu kullanıcıya açıkça soruyor ve hesabın defterinde Para Yatırma 2.000 olarak gösteriyor; Hesap Defterim Ağustos listesinin ilk satırı yapıyor.

Fark burada değil, sonrasında: Money Manager açılışı ayın gelir toplamına sokmuyor, Hesap Defterim sokuyor. Bölüm 1'deki 51.200'ün içindeki 22.000 tam olarak budur. Aynı tasarım tercihi, iki ayrı sonuç.

*Dayanağı: E0239, E0240, E0137, E0142, E0231*

### Aktarım bir üründe tek kayıt, üçünde iki satır *(Çıkarım)*

Money Manager aktarımı tek bir Havale kaydı olarak tutuyor: iki hesabı aynı satır taşıyor ve günün başlığı 0/0 kalıyor. Bluecoins ve Wallet iki bacak gösteriyor ve ikisini aynı günde netliyor. Toplamı koruyan şey ya tek kayıt olması ya da iki bacağın birlikte yazılması.

Hesap Defterim'de bu netleme yok: iki satır birbirinden habersiz. Koşumda bir bacak silindiğinde karşı bacak kaldı ve net varlık sessizce yanlışa döndü. Bu gözlem kareyle değil koşumla kayıtlı; kare yalnız tek defteri gösteriyor.

Sessizlik burada asıl sorun: yanlış sayı bir hata mesajı vermiyor, yalnız toplamda duruyor.

*Dayanağı: E0228, E0140, E0022, E0279, koşum kaydı (Hesap Defterim A testi)*

### Goodbudget'ta aktarım zarf katmanına hiç uğramıyor *(Çıkarım)*

Aktarım formu birinci sınıf ve ayrı bir ekran, ama zarf alanı taşımıyor. Bölüm 1'de zarf ile hesabın iki ayrı defter olduğu görülmüştü; aktarım bu iki defterden yalnız hesabı ilgilendiriyor.

Bu tutarlı bir tasarım: para amacını değiştirmeden yer değiştiriyorsa zarf kalanının değişmesi için bir neden yok. Karşılığında kullanıcı, paranın hangi hesapta durduğunu zarf ekranından hiçbir zaman göremiyor.

*Dayanağı: E0119, E0120, E0417, E0421*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Money Manager** | Aktarımı tek satırda gösterip günün toplamını 0/0 yazıyor; hesabı toplamdan çıkarma anahtarı hesabı silmeden raporu sadeleştiriyor | Aynı veriden iki farklı net varlık okunabiliyor ve hangisinin geçerli olduğu ekranda yazmıyor |
| **Bluecoins** | İki bacağı da gösterip günde netliyor — kullanıcı paranın nereden nereye gittiğini satırdan okuyabiliyor; aktarım ücreti için ayrı alan var | Ücret aktarımın içinde değil, kendi hesabı ve kategorisi olan ayrı bir gider olarak soruluyor; aktarımın maliyeti aktarımın kendisinde görünmüyor |
| **Wallet** | Aktarımın iki satırı renk ve yönle ayrışıyor, dönem toplamı bozulmuyor | Hesap formunda açılış bakiyesi alanı yok; başlangıç parası ilk kayıtla girilmek zorunda |
| **Hesap Defterim** | Aktar ayrı bir ekran ve tek formla iki deftere yazıyor; satır başına yürüyen denge paranın izini kolay okutuyor | İki bacak bağımsız: biri silinince veya düzenlenince diğeri güncellenmiyor ve net varlık sessizce bozuluyor |
| **Goodbudget** | Aktarım birinci sınıf bir işlem türü ve zarf kalanına dokunmuyor — para amacını değiştirmeden yer değiştiriyor | Ücretsiz pakette tek hesap açılabildiği için aktarım hiç çalıştırılamıyor |

**Belge 3'e taşınan soru**

- Açılış bakiyesi bir kayıt mı olmalı, hesabın bir özelliği mi? Kayıt olursa ayın toplamından çıkarmak için ayrı bir kural gerekiyor.
- Aktarımın iki ucu tek bir nesne mi olmalı? Money Manager tek kayıt tutuyor; Hesap Defterim'in örneği iki bağımsız satırın sessizce tutarsızlaşabildiğini gösteriyor.
- Bir hesabı toplamdan çıkarma anahtarı gerekli mi? Gerekliyse, hangi toplamın geçerli olduğu ekranda nasıl yazılmalı?


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 2.1 | E0239 | Money Manager | Canlı kare | Hesap formu ve sorulan soru |
| 2.2 | E0240 | Money Manager | Canlı kare | Aynı açılışın defterdeki karşılığı |
| 2.3 | E0145 | Hesap Defterim | Canlı kare | Hesap Eklem |
| 2.4 | E0113 | Goodbudget | Canlı kare | Ana Hesap kuruldu |
| 2.5 | E0462 | Money Manager | Canlı kare | Havale formu |
| 2.6 | E0435 | Bluecoins | Canlı kare | Transfer formu |
| 2.7 | E0436 | Wallet | Canlı kare | Aktarım formu |
| 2.8 | E0147 | Hesap Defterim | Canlı kare | Aktar formu |
| 2.9 | E0119 | Goodbudget | Canlı kare | Account Transfer |
| 2.10 | E0022 | Bluecoins | Canlı kare | İşlem listesi · Ağustos |
| 2.11 | E0285 | Wallet | Canlı kare | Records |
| 2.12 | E0140 | Hesap Defterim | Canlı kare | Bütün Hesaplar |
| 2.13 | E0252 | Money Manager | Canlı kare | Toplama Dahil Et açık |
| 2.14 | E0253 | Money Manager | Canlı kare | Anahtar kapalıyken hesaplar |
