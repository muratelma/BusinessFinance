# Bölüm 5 · Zaman: tekrar, plan ve bütçe

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Gelecekte olacak bir ödeme ne zaman gerçek bir kayda dönüşüyor ve o ana kadar hangi toplamda görünüyor?

Tekrarlayan bir ödeme kurulduğunda ortada iki şey oluyor: bir tanım ve o tanımın üreteceği kayıtlar. Ürünler bu ikisinin arasına koydukları kapıyla ayrışıyor — kapı otomatik mi açılıyor, kullanıcı onayı mı istiyor, yoksa hiç kapı yok mu.

Bölüm üç soruyu izliyor: plan nasıl kuruluyor, gerçek kayda hangi anda dönüşüyor ve dönüşene kadar ayın toplamında görünüyor mu. Sonunda aynı zaman ekseninin ikinci yüzü var: bütçe.

**Bu bölüme girmez**

- Taksitin aylara dağılması → Bölüm 3
- Vadeli alacağın takibi → Bölüm 4
- Raporun dönem seçimi → Bölüm 7
- Plan ekranlarının görsel düzeni → Belge 1 §7

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare | Plan kurulumu, gerçekleşme ve gelecek önizlemesi kareli. |
| Wallet | Canlı kare | Onay kapısı, otomatik/onaylı seçimi ve bütçe kareli. |
| Bluecoins | Canlı kare | Hatırlatıcı listesi ve kayda çevirme adımı kareli. |
| Goodbudget | Canlı kare | Tekrar sıklığı kareli; bütçe zarfın kendisi. |
| Hesap Defterim | Görülmedi | Tekrar, plan ve hatırlatıcı beş yüzeyde arandı, bulunamadı. |
| KolayBi | Kaynak görseli | Tekrarlı maaş formu destek görselinde. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 5.1 · Plan nasıl kuruluyor

Üç üründe plan kaydın bir özelliği: aynı forma bir tekrar rozeti ekleniyor. Bir üründe ise plan ayrı bir nesne ve kendi formu var. Sıklık listeleri de aynı zenginlikte değil.

**Şekil 5.1 · Aylık rozetli gider formu** (E0241)

- Sıradan gider formu; üstte "Aylık" rozeti.
- 600, Ana Hesap, "Bulut yazilim aboneligi".
- Plan kaydın bir özelliği, ayrı nesne değil.

**Şekil 5.2 · Tekrarlama seçenekleri** (E0238)

- On dört seçenek: Günlük'ten Yıllık'a.
- Haftanın günleri, Haftasonu, Ayın Son Günü de var.

**Şekil 5.3 · Planlı işlem sayfası** (E0039)

- Plan işlem formundan açılıyor.
- Otomatik kol seçenek olarak duruyor.
- Çalışması bu koşumda denenmedi.

**Şekil 5.4 · Add Planned payment** (E0288)

- Plan ayrı bir nesne; kendi formu var.
- Income / Expense / Transfer seçimi.
- Frequency: Recurrent payment.

- Hesap Defterim'de tekrar, plan, hatırlatıcı veya abonelik kavramı beş ayrı yüzeyde arandı — işlem formu, formun menüsü, ana ekran menüsü, çekmecenin on yedi kalemi ve ayarların yirmi bir kalemi — ve hiçbirinde bulunamadı. Kullanıcı her ay eliyle giriyor.
- Goodbudget'ta tekrar işlem formunun bir kutusu: "Schedule this…" işaretlenince sıklık açılıyor. Seçenekler arasında Once, Weekly ve Every 2 Weeks görüldü.
- Wallet'ta planın tarih seçicisinde geçmiş günler soluk; plan ileri tarihli kuruluyor.
- Goodbudget'ta kurulan plan Every 2 Weeks sıklığıyla iki tarihe düştü.

## 5.2 · Plan gerçek kayda hangi anda dönüşüyor

Bölümün ayrım noktası burası. Bir üründe kapı tek dokunuşluk bir onay, ve ürün o kapının hep açık mı kalacağını kullanıcıya soruyor. Bir üründe her vade elle kayda çevriliyor ve hangi tarihin yazılacağı soruluyor. Bir üründe kapı hiç yok: vade gelince kayıt kendiliğinden oluşuyor.

**Şekil 5.5 · Bekleyen örnek: Due today** (E0289)

- Plan detayında turuncu "Due today".
- Tek dokunuşluk Confirm düğmesi.
- Gerçekleşme kullanıcının onayıyla.

**Şekil 5.6 · Kapı hep açık mı kalsın** (E0291)

- "…create transactions automatically…?"
- Yes (Recommended) önceden seçili.
- No: "will wait for your approving."

**Şekil 5.7 · Hangi tarihe yazılsın** (E0043)

- Kayda çevirirken iki tarih seçeneği.
- Bugün mü, planlanan gün mü.
- Hangisine basıldığı karede görünmüyor.

**Şekil 5.8 · Kapı sonradan değişebiliyor** (E0318)

- Aynı soru yeniden açılıyor.
- Bu kez No önceden seçili.
- Seçilen mod saklanıyor.

**Şekil 5.9 · Kurulumda tek soru** (E0430)

- "Tarihte tekrar eden işlemler uygulanır."
- Tekrarlı kaydedilsin mi: Hayır / Evet.
- Soru kurulumda, vadede değil.

- Wallet kapının davranışını kullanıcıya soruyor ve iki seçeneğin sonucunu da yazıyor: otomatik kolda gelecek ödemeler kendiliğinden kayda dönüşüyor, diğerinde onay bekliyor. Varsayılan otomatik tarafta.
- Money Manager'da kapı yok: tekrarlı kayıt kurulurken ürün bir kez soruyor ve sonucunu aynı cümlede yazıyor; vadesi gelen örnek onay beklemeden gerçek kayıt oluyor ve ayın giderine giriyor. Ne zaman uygulanacağı bir ayar: tarihinde ya da her ayın ilk günü.
- Bekleyen bir örnek ertelenebiliyor ya da atılabiliyor: plan menüsünde Postpone ve Dismiss girişleri var; sonuçları denenmedi.
- Wallet'ta onay sonrası örnek "Paid Today" olarak geçmişe düşüyor ve sıradaki vade 30 gün sonrasına kayıyor.
- Bluecoins'te tekrar ve taksitler hatırlatıcı listesinde bekliyor ve kendiliğinden gerçekleşmiyor; her biri Kaydet ile elle kayda dönüşüyor. Bu bir kullanıcı kontrolüyle kayıtlı. Planlı işlem sayfasındaki otomatik kolun açık olduğu bir plan izlenmedi.

## 5.3 · Gerçekleşmemiş plan hangi toplamda görünüyor

Gelecekteki bir ödeme ayın giderine girmemeli — ama görünmemeli de değil. Dört ürün bu dengeyi dört ayrı yerde kuruyor: ayrı bir bölüm, ayrı bir liste, ayrı bir ekran veya ortak bir hatırlatıcı listesi.

**Şekil 5.10 · Gelecek ay önizlemesi** (E0244)

- Ekim toplamı 0 / 0 / 0 — "Veri yok".
- Üstte ayrı "Tekrarlama" satırı −600.
- Plan görünür ama toplama girmiyor.

**Şekil 5.11 · Tekrarlayan İşlemler listesi** (E0416)

- Ayarlar altında planların kendi listesi.
- Sıradaki vade, sıklık, hesap ve kategori.
- Taksit planı bu listede yok.

**Şekil 5.12 · Planned payments** (E0306)

- Planlar kendi ekranında listeleniyor.
- Yalnız sıradaki vade görünüyor.
- All / Income / Expense / Transfer filtresi.

**Şekil 5.13 · Hatırlatıcı listesi** (E0047)

- Üç tür aynı listede:
- bağımsız hatırlatıcı, tekrar ve taksit.
- Gerçekleşen kayıt listeden çıkıyor.

- Dördü de aynı ilkede buluşuyor: gerçekleşmemiş plan ayın gelir/gider toplamına girmiyor. Ayrışan şey planın nerede görüneceği — Money Manager aynı ekranın üst bölümünde, Wallet ve Money Manager ayrıca kendi listelerinde, Bluecoins bütün plan türlerini tek bir hatırlatıcı listesinde topluyor.
- Bluecoins'in ortak listesi üç ayrı kavramı yan yana koyuyor: bir kez olacak hatırlatıcı, tekrarlayan plan ve taksit. Üçünün tek listede durması ürünün bunları aynı şey saydığı anlamına gelmiyor; yalnız aynı yerde gösteriyor.
- Money Manager'ın "Tekrar ne zaman uygulanır?" ayarı iki değer taşıyor: Tarihte ve Her ayın ilk günü. Kayıt başına onay seçeneği yok.
- Bluecoins'te bağımsız bir hatırlatıcı "Bir Defa" sıklığıyla da kurulabiliyor.

## 5.4 · Plan ve gerçekleşme, ürün ürün

Aynı abonelik planı beş üründe kuruldu ya da kurulamadı. Tabloda planın tanımı, kapısı ve toplamlardaki yeri.

|  | Money Manager | Wallet | Bluecoins | Goodbudget | Hesap Defterim |
|---|---|---|---|---|---|
| Plan nerede tanımlanıyor | *Canlı kare* · Kaydın bir özelliği; forma rozet ekleniyor | *Canlı kare* · Ayrı bir nesne; kendi formu ve ekranı | *Canlı kare* · İşlem formundan açılan planlı işlem sayfası | *Canlı kare* · İşlem formunda Schedule this… kutusu | *Görülmedi* · Beş yüzeyde arandı, bulunamadı |
| Sıklık seçenekleri | *Canlı kare* · On dört seçenek; Ayın Son Günü dâhil | *Canlı kare* · Every 1 month koşuldu; liste açılmadı | *Canlı kare* · Aylık koşuldu; tam liste görülmedi | *Canlı kare* · Once, Weekly, Every 2 Weeks görüldü | *Görülmedi* · Kavram yok |
| Gerçekleşme kapısı | *Canlı kare* · Kapı yok — vade gelince kendiliğinden kayıt; zamanı bir ayar | *Canlı kare* · Tek dokunuşluk Confirm; kapının hep açık kalması sorularak seçiliyor | *Koşum kaydı* · Her vade elle kaydediliyor; kaydederken tarih soruluyor | *Görülmedi* · Ölçülmedi | *Görülmedi* · Kavram yok |
| Gerçekleşmemiş plan toplamda mı | *Canlı kare* · Hayır — ayrı bölümde önizleme | *Canlı kare* · Hayır — ayrı ekranda liste | *Canlı kare* · Hayır — hatırlatıcı listesinde | *Görülmedi* · Ölçülmedi | *Görülmedi* · Kavram yok |
| Bütçe var mı | *Canlı kare* · Evet — kategori başına; kurulu tek bütçe Yiyecek 1.400 | *Canlı kare* · Evet — dönemli bütçe, aşım uyarısı ve tahmin | *Canlı kare* · Bütçe Özeti var; hiçbir kategoriye bütçe kurulmamış | *Canlı kare* · Bütçe zarfın kendisi | *Görülmedi* · Bulunamadı |

- Gerçekleşmemiş planın ayın toplamına girmemesi ölçülebilen üç üründe de aynı. Ayrışan şey planın nerede durduğu, sayılıp sayılmadığı değil.

## 5.5 · Bütçe: aynı zaman ekseninin ikinci yüzü

Plan geleceğe yazılmış bir ödeme; bütçe ise geleceğe konmuş bir sınır. İki üründe bütçe dönemli bir üst sınır, birinde paranın kendisi zaten zarflara bölünmüş durumda.

**Şekil 5.14 · Bütçe aşıldı** (E0301)

- Aylık bütçe 5.000; kalan −1.600.
- Kırmızı çubuk ve Over Budget etiketi.
- Ayrıca aşım uyarısı çıkıyor.

**Şekil 5.15 · Bütçe detayı ve tahmin** (E0302)

- Harcanan 6.600; günlük ortalama 660.
- Forecasted Spend 6.600 / 30 gün.
- Geçen döneme göre +%222.

**Şekil 5.16 · Toplam sekmesi · bütçe bloğu** (E0405)

- Bütçe bloğu Toplam sekmesinin içinde.
- Kurulu bütçe yalnız Yiyecek 1.400.
- Yiyecek'te harcama yok: %0.

**Şekil 5.17 · Zarflar** (E0417)

- Her zarfın altında bütçe tutarı.
- Market 23.784 / bütçe 850.
- Bütçe ayrı bir ekran değil, paranın kendisi.

- Wallet bütçeyi bir tahmin motoruna bağlamış: harcanan tutarın yanında günlük ortalama, dönem sonu tahmini ve önceki dönemle karşılaştırma duruyor. Bütçe yalnız bir sınır değil, bir gidişat göstergesi.
- Goodbudget'ta bütçe ile bakiye aynı şey: zarfın içindeki para hem harcanabilir tutar hem bütçe sınırı. Bölüm 1'de görülen iki defterli yapının kaynağı da bu.
- Money Manager'ın bütçesi kurulu: kategori başına, bir varsayılan tutar ve ay ay değerle; ekran değişikliğin önümüzdeki aydan geçerli olduğunu söylüyor. %0'ın nedeni ayın bütün giderinin Diğer'de olması, bütçeli tek kategori Yiyecek'te harcama olmaması.
- Bluecoins'in Bütçe Özeti hiçbir kategoriye bütçe kurulmadığı için ayın bütün giderini tek dilimde, Others olarak gösteriyor; bütçe sütunu 0.

## 5.6 · Kaynakta tekrar: mevcut kaydı tekrarlıya çevirmek

KolayBi'de tekrar, sıfırdan kurulan bir plan değil; var olan bir kaydın menüsünden çıkan bir dönüştürme. Canlı ürünlerde plan kayıttan önce kuruluyor, burada kayıttan sonra.

**Şekil 5.18 · KolayBi · Tekrarlı Maaş Oluştur** (E0205)

- Arkada İşlemler menüsünde "Tekrarlı Maaşa Dönüştür" vurgulu.
- Oluşturma Periyodu ve Maaş Oluşturma Tekrar Sayısı zorunlu.
- Maaş Ödeme Tarihi: Belirsiz / Belirli.

- Tekrar sayısı burada bir alan: plan sonsuza kadar sürmüyor, kaç kez oluşacağı baştan yazılıyor. Canlı dört üründe böyle bir alan görülmedi.
- Aynı dönüştürme genel gider tarafında da var: gider detayının İşlemler menüsünde "Tekrarlı Genel Gidere Dönüştür" duruyor. Tekrar, kayıt türünden bağımsız bir eylem olarak kurgulanmış.
- Notlar sayfasında ayrıca hatırlatıcısı olan not kavramı var: başlık, not, hatırlatıcı aktif/pasif ve görünürlük.

**Aynı soruda kaynaktan okunan üç ürün**

- *Kaynak beyanı* · **Paraşüt** — Kaynak tekrarlayan gideri ve tekrarlayan faturayı iki ayrı akış sayıyor; abonelik faturalaması için otomatik oluşturma anlatılıyor. Maaş ve prim ise kendi başına otomatik tekrarlayan bir gider türü. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **Logo İşbaşı** — Kaynakta tekrarlayan kayıt ayrı bir başlık olarak anlatılmıyor; vurgu fatura ve tahsilat takvimine veriliyor. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Kaynağa göre tekrar bir plan değil, bir kural: en çok otuz kural tanımlanıyor ve kurallar inen benzer işlemleri otomatik sınıflıyor. Kural kayıt üretmiyor, inen kaydı etiketliyor. Bölüm 6'da kurulur.

## 5.7 · Planın yolu ve ayrıldığı noktalar

Plan kurulduğu andan ayın toplamına girdiği ana kadar dört adım. Asıl çatallanma üçüncü adımda: tanım ile kayıt arasındaki kapı.

**1. Plan tanımlanıyor**

- *Money Manager · Bluecoins · Goodbudget* — Plan kaydın bir özelliği: aynı forma bir tekrar rozeti ya da kutusu ekleniyor. (E0241)
- *Wallet* — Plan ayrı bir nesne; kendi formu, kendi ekranı ve kendi listesi var. (E0288)
**→** *Hesap Defterim* — Kavram yok. Beş ayrı yüzey tarandı; tekrar, plan veya hatırlatıcı bulunamadı. (E0171)

**2. Vade geliyor**

Ölçülebilen üç üründe de gerçekleşmemiş plan ayın gelir/gider toplamına girmiyor; ayrı bir bölümde, ayrı bir listede ya da hatırlatıcı listesinde bekliyor.


**3. Kayda dönüşüyor**

**→** *Wallet* — Tek dokunuşluk Confirm. Ürün ayrıca kapının hep açık mı kalacağını soruyor: otomatik kol mu, onay kolu mu. (E0291)
**→** *Bluecoins* — Her vade elle kaydediliyor; kaydederken hangi tarihin yazılacağı soruluyor: bugün mü, planlanan gün mü. (E0043 · kullanıcı kontrolü)
**→** *Money Manager* — Kapı yok. Soru yalnız kurulumda soruluyor; vade gelince kayıt kendiliğinden oluşuyor. (E0430 · E0243)

**4. Bütçeyle karşılaşıyor**

- *Wallet* — Dönemli bütçe: aşım uyarısı, günlük ortalama ve dönem sonu tahmini birlikte geliyor. (E0302)
**→** *Goodbudget* — Bütçe ayrı bir sınır değil, paranın kendisi: zarfın içindeki tutar hem harcanabilir para hem sınır. (E0417)
- *Money Manager* — Kategori başına bütçe, Toplam sekmesinin içinde; bütçeli kategoride harcama olmayınca ilerleme %0. (E0405 · E0431)

- Üçüncü adımdaki kapı bu bölümün ayrım noktası ve üç ürün üç uçta duruyor: biri kapının hep açık kalıp kalmayacağını kullanıcıya soruyor, biri her vadeyi kullanıcıya bırakıyor, biri kapıyı hiç kurmuyor.
- Dördüncü adımda Goodbudget yolu tamamen kısaltıyor: zarf hem plan hem bütçe hem bakiye olduğu için ayrı bir bütçe kavramına ihtiyaç duymuyor.

## 5.8 · Neden ayrışıyorlar

Tekrarlayan bir ödemede iki nesne var: tanım ve kayıt. Ürünler bu ikisinin arasına koydukları kapıyla, ve o kapının kime ait olduğuyla ayrışıyor.

### Kapının kime ait olduğu ürünün en açık tercihi *(Çıkarım)*

Wallet kapıyı kullanıcıya veriyor ve iki kez soruyor: önce her vadede tek dokunuşluk bir Confirm, sonra "bu kapı hep açık mı kalsın" sorusu. İki seçeneğin sonucu da ekranda yazılı — otomatik kolda gelecek ödemeler kendiliğinden kayda dönüşüyor, diğerinde onay bekliyor.

Bluecoins kapıyı her vadede kullanıcıya bırakıyor: tekrar ve taksitler hatırlatıcı listesinde bekliyor, kendiliğinden gerçekleşmiyor ve Kaydet'e basılınca hangi tarihin yazılacağını soruyor. Bugün mü, planlanan gün mü — aynı kayıt iki ayrı aya düşebilir ve bu seçim ayın toplamını değiştirir.

Money Manager kapıyı hiç kurmuyor: kurulumda bir kez "Tarihte tekrar eden işlemler uygulanır" diyor ve vadesi gelen örnek onaysız kayda dönüşüyor. Kullanıcıya kalan tek seçim zamanlama: tarihinde ya da ayın ilk günü.

Üç uç, aynı sorunun üç cevabı. Bluecoins'te unutulan onay gideri eksik bırakıyor; Money Manager'da hiç sorulmadan yazılmış bir kayıt duruyor; Wallet hangisinin olacağını kullanıcının seçimine bağlıyor.

*Dayanağı: E0291, E0289, E0043, E0430, E0429, E0243, kullanıcı kontrolü (Bluecoins)*

### Gerçekleşmemiş plan ölçülen hiçbir üründe toplama girmiyor *(Çıkarım)*

Ölçülebilen üç üründe de aynı sonuç: gelecekteki bir ödeme ayın gelir/gider toplamına dâhil değil. Money Manager'ın Ekim listesi toplamı 0/0/0 gösterirken üst bölümde plan satırı duruyor.

Ayrışan şey planın nerede görüneceği: aynı ekranın üst bölümünde, ayrı bir ekranda, ya da bütün plan türlerinin toplandığı ortak bir hatırlatıcı listesinde. Üçü de planı görünür tutuyor ama sayıya katmıyor.

Bu ortak davranış, planın bir niyet olduğunun ürünler arası bir kabul olduğunu gösteriyor.

Taksit bu kuralın dışında kalıyor: Money Manager gelecek taksitleri plan değil, önceden yazılmış kayıt olarak tutuyor ve her biri kendi ayının giderine giriyor (Bölüm 3). Aynı ürün tekrarlayan planı toplama katmıyor, taksidi katıyor.

*Dayanağı: E0244, E0306, E0047, E0416, E0407*

### Bütçe iki ayrı şey olarak kurulmuş *(Çıkarım)*

Wallet'ta bütçe dönemli bir üst sınır ve kendi ekranında yaşıyor: aşıldığında kırmızı çubuk, uyarı, günlük ortalama ve dönem sonu tahmini birlikte geliyor. Bütçe bir sınırın yanında bir gidişat göstergesi.

Money Manager da aynı tarafta ama gidişat göstergesi olmadan: bütçe kategori başına bir sınır, Toplam sekmesinde bir ilerleme çubuğuyla ölçülüyor.

Goodbudget'ta bütçe ayrı bir nesne değil — zarfın içindeki para hem harcanabilir tutar hem sınır. Bu yüzden aşımın ayrı bir bütçe ekranı yok; sınır zarfın kendi bakiyesi. Zarfı aşan bir harcama bu koşumda denenmedi.

İki yaklaşım aynı soruyu farklı yerde cevaplıyor: biri harcamayı sonradan ölçüyor, öteki parayı baştan bölüyor.

*Dayanağı: E0301, E0302, E0405, E0431, E0417, E0424*

### Kavramın hiç olmaması da bir sonuç üretiyor *(Çıkarım)*

Hesap Defterim'de tekrar beş ayrı yüzeyde arandı ve bulunamadı. Bunun sonucu kullanıcının her ay aynı kaydı eliyle girmesi — ve girilmediği ay defterde hiçbir iz kalmaması.

Defter mantığı burada da kendini koruyor: yalnız olmuş olan yazılıyor. Olacak olanı gösterecek bir yer olmadığı için, unutulan ödeme sessizce kayboluyor.

*Dayanağı: E0171, E0150, E0178*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Money Manager** | On dört sıklık seçeneği ve planların kendi listesi var; gelecek ay planı aynı ekranda ayrı bir bölümde gösterip toplama katmıyor | Vadesi gelen tekrar sorulmadan kayda dönüşüyor; bütçe bloğu Toplam sekmesinin içinde saklı ve %0'ın nedeni ekranda yazmıyor |
| **Wallet** | Kapı kullanıcının: her vadede tek dokunuşluk onay, ve kapının hep açık kalıp kalmayacağı sorularak seçiliyor; bütçe aşım uyarısı ve dönem sonu tahminiyle birlikte geliyor | Planlar yalnız sıradaki vadeyle listeleniyor; ileri dönemlerin toplu görünümü yok |
| **Bluecoins** | Bağımsız hatırlatıcı, tekrar ve taksit tek listede toplanıyor; kayda çevirirken hangi tarihe yazılacağı soruluyor | Tekrar ve taksitler kendiliğinden gerçekleşmiyor, her vade bir dokunuş istiyor; üç ayrı kavramın tek listede durması hangisinin ne olduğunu okumayı zorlaştırıyor |
| **Goodbudget** | Bütçe ile zarfın bakiyesi aynı şey olduğu için ayrı bir bütçe ekranı gerekmiyor | Zarf katmanı hesap katmanıyla uyuşmuyor: bütçenin bıraktığı para ile hesaptaki para iki ayrı sayı |
| **Hesap Defterim** | — | Tekrar, plan, hatırlatıcı ve bütçe kavramlarının hiçbiri yok; unutulan ödeme defterde hiç iz bırakmıyor |

**Belge 3'e taşınan soru**

- Plan gerçek kayda dönerken onay istenmeli mi? Wallet bunu bir tercih yapıp sonucunu yazıyor; varsayılanı otomatik tarafta.
- Gerçekleşme hangi tarihe yazılmalı — bugüne mi, planlanan güne mi? Bluecoins soruyor ve cevap ayın toplamını değiştiriyor.
- Gerçekleşmemiş plan nerede görünmeli? Üç ürün üç ayrı yer seçmiş ama üçü de aynı kuralda birleşiyor: toplama girmiyor. Money Manager'ın taksitleri bunun dışında.
- Bütçe harcamayı sonradan mı ölçmeli, parayı baştan mı bölmeli? İki model iki ayrı kullanıcı davranışı üretiyor.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 5.1 | E0241 | Money Manager | Canlı kare | Aylık rozetli gider formu |
| 5.2 | E0238 | Money Manager | Canlı kare | Tekrarlama seçenekleri |
| 5.3 | E0039 | Bluecoins | Canlı kare | Planlı işlem sayfası |
| 5.4 | E0288 | Wallet | Canlı kare | Add Planned payment |
| 5.5 | E0289 | Wallet | Canlı kare | Bekleyen örnek: Due today |
| 5.6 | E0291 | Wallet | Canlı kare | Kapı hep açık mı kalsın |
| 5.7 | E0043 | Bluecoins | Canlı kare | Hangi tarihe yazılsın |
| 5.8 | E0318 | Wallet | Canlı kare | Kapı sonradan değişebiliyor |
| 5.9 | E0430 | Money Manager | Canlı kare | Kurulumda tek soru |
| 5.10 | E0244 | Money Manager | Canlı kare | Gelecek ay önizlemesi |
| 5.11 | E0416 | Money Manager | Canlı kare | Tekrarlayan İşlemler listesi |
| 5.12 | E0306 | Wallet | Canlı kare | Planned payments |
| 5.13 | E0047 | Bluecoins | Canlı kare | Hatırlatıcı listesi |
| 5.14 | E0301 | Wallet | Canlı kare | Bütçe aşıldı |
| 5.15 | E0302 | Wallet | Canlı kare | Bütçe detayı ve tahmin |
| 5.16 | E0405 | Money Manager | Canlı kare | Toplam sekmesi · bütçe bloğu |
| 5.17 | E0417 | Goodbudget | Canlı kare | Zarflar |
| 5.18 | E0205 | KolayBi | Kaynak görseli | KolayBi · Tekrarlı Maaş Oluştur |
