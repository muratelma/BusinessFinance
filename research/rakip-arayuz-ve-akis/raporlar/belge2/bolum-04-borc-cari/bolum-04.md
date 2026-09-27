# Bölüm 4 · Borç, cari ve tahsilat

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Henüz tahsil edilmemiş bir alacak nerede duruyor ve hesap bakiyesine dokunuyor mu?

Bir iş yapıldı, fatura kesildi, para henüz gelmedi. Bu an ürünler arasındaki en keskin ayrımı ortaya çıkarıyor: kimi üründe alacak bir hesap bakiyesi, kiminde ayrı bir nesne, kiminde hiç yok.

Bölüm üç adımı izliyor: alacak nasıl doğuyor, doğduğu anda hangi sayıyı değiştiriyor, ve tahsilat yapıldığında para nereden nereye geçiyor.

**Bu bölüme girmez**

- Kart borcu ve kart ödemesi → Bölüm 3
- Vadeli ödemenin takvimde görünmesi → Bölüm 5
- Cari ekstresinin belge olarak dışa aktarılması → Bölüm 8
- Cari ekranlarının görsel düzeni → Belge 1 §8

| Ürün | Kanıt | Not |
|---|---|---|
| Bluecoins | Canlı kare | Alacağın doğması ve tahsilat zinciri uçtan uca kareli. |
| Wallet | Canlı kare | Borç nesnesi, yön seçimi ve bakiye sorusu kareli. |
| Money Manager | Canlı kare | Cari kavramı hesap listesinde bulunamadı. |
| Hesap Defterim | Canlı kare | Veresiye ayrı bir uygulamaya yönlendiriliyor. |
| Goodbudget | Görülmedi | Debt hesap türü var ama ücretsiz pakette açılamadı. |
| KolayBi | Kaynak görseli | Cari formu ve personel carisi destek görselinde. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 4.1 · Alacak nerede yaşıyor

İki canlı üründe iki bambaşka cevap: birinde alacak bir hesap ve bakiyesi listede diğer hesapların yanında duruyor; ötekinde alacak ayrı bir nesne ve kendi ekranında yaşıyor. Üçüncüsünde kavram ürünün dışına çıkarılmış.

**Şekil 4.1 · Alacak bir hesap türü** (E0049)

- Cari hesap, hesap listesinde kendi grubunda.
- Grubu, kredi kartıyla aynı CARİ HESAP bölümünde.
- Özel bir alanı veya fatura bağı görülmedi.

**Şekil 4.2 · Borç ayrı bir nesne** (E0316)

- Debts / Active sekmesinde bir kart.
- "ADA REKLAM OWES ME" 5.000,00.
- Kartın altında Add Record girişi.

**Şekil 4.3 · Borç yönle başlıyor** (E0358)

- Ekleme yalnız iki yönle başlıyor:
- I Lent ve I Borrowed.
- Fatura veya alacak türü seçeneği yok.

**Şekil 4.4 · Veresiye ayrı uygulamada** (E0177)

- Menüde "Diğer uygulamalar" bölümü.
- Veresiye Defteri ayrı bir indirme.
- Bu üründe cari kavramı yok.

- Money Manager'ın hesap listesinde Nakit, Banka Hesapları ve Kredi Kartı grupları var; cari veya karşı taraf türünde bir grup bulunamadı. Alacak bu üründe ancak sıradan bir hesap açılarak taklit edilebilir.
- Goodbudget'ın hesap türleri arasında bir Debt grubu var, ama ücretsiz pakette tek hesap sınırı dolduğu için bu türde hesap açılamadı; borç modeli görülemedi.
- Bluecoins'te cari, hesap seçicide kendi grubu olarak duruyor: Banka, Nakit, Cari hesap, Kredi Kartı.
- Goodbudget'ın hesap grupları arasında Debt başlığı kareli.

## 4.2 · Wallet kullanıcıya soruyor: bu borç bakiyeye dokunsun mu

Bu araştırmanın en açık sözlü ekranı. Ürün, borcun hesap bakiyesini değiştirip değiştirmeyeceğini kendi kararı yapmıyor — kullanıcıya soruyor ve sonucunu cümleyle yazıyor.

**Şekil 4.5 · Mevcut kayda bağla ya da atla** (E0308)

- "Do you already have this debt record in Wallet?"
- Yes, select record · No, skip.

**Şekil 4.6 · Alanlar ve varsayılan vade** (E0309)

- Name, Description, Account, Amount.
- Due date varsayılanı bir yıl sonrası.
- Hesap alanı formda zorunlu duruyor.

**Şekil 4.7 · Kayıt oluşturulsun mu** (E0315)

- "Do you want to create a Record for this Debt?"
- "If you create a Record your balance will change."
- No · Yes, create record.

**Şekil 4.8 · Debt Records** (E0317)

- Ana Hesap'ta −5.000 kayıt oluştu.
- Kategori: Loan, interests.
- Liste toplamı −5.000,00.

- Ürün iki yolu da açık tutuyor: borç yalnız bir takip kaydı olarak kalabilir, ya da hesap bakiyesini değiştiren gerçek bir işlem üretebilir. Seçim kullanıcıya bırakılmış ve sonucu tek cümleyle yazılmış.
- Bağlama kolu da işletildi: Select Record bütün mevcut kayıtları etiketleriyle listeliyor ve borç o kayıtlardan birine bağlanabiliyor.
- Borç kartı "OWES ME 5.000,00" derken kayıt listesi "Total −5.000,00" diyor — aynı borç, iki ekranda ters işaret.
- Borç eklemenin başlangıcı yalnız iki yön: I Lent / I Borrowed.
- Aynı soru ikinci borç kurulurken de soruldu.

## 4.3 · Tahsilat: alacak hesabından bankaya

Bluecoins'te zincir uçtan uca koşuldu. Alacak bir hesap olduğu için tahsilat da iki hesap arasında bir aktarım — ve tam da bu yüzden gelir/gider toplamlarına hiç dokunmuyor.

**Şekil 4.9 · Cari hesaba gelir formu** (E0074)

- Gelir kaydı cari hesaba yazılıyor.
- Durum ve fatura bağı bu karede açık değil.

**Şekil 4.10 · Cari 12.000** (E0075)

- Cari bakiyesi 12.000.
- Ana Hesap 29.700 — banka kıpırdamadı.

**Şekil 4.11 · Cari 7.000** (E0079)

- Cari 12.000 → 7.000.
- Banka 29.700 → 34.700.
- Aktarım neti sıfır.

**Şekil 4.12 · Dönem karşılaştırması** (E0055)

- VARLIKLAR 22.350 → 43.850.
- CARİ HESAP −1.000 → −500: cari 0, kart −500.
- Cari, varlıklarda değil, kartla aynı bölümde.

- Tahsilat 5.000'lik kısmi bir ödemeydi: alacağın tamamı kapanmadan cari bakiyesi 7.000'de kaldı. Kısmi tahsilat ayrı bir kavram değil, sıradan bir aktarım tutarı.
- Alacağın doğduğu an banka bakiyesi hiç değişmedi; para ancak tahsilat adımında bankaya geçti. Gelir ise fatura anında yazıldı.
- Geçmiş bir satırdaki 12.000 güncel cari bakiyesi değildir; liste satırı o anın tutarını taşıyor.
- Formun Durum alanı dört değer taşıyor: Yok, Kontrol, Mutabık, İptal edildi. Fatura bağlama alanı yok; belge yalnız formun üstündeki ataçla ekleniyor.

## 4.3b · Wallet'ta tahsilat ve borcun rapora düşen izi

Aynı zincir bu üründe de uçtan uca koşuldu ve sonucu diğerinden farklı çıktı. Tahsilat borcu düşürüyor — ama borç kayıtları gelir ve gider toplamlarına da giriyor.

**Şekil 4.13 · Kayıt nasıl eklensin** (E0367)

- Select Record: mevcut bir kaydı borca bağla.
- Create new Record: borcu öde ya da artır.
- Borcu artırma da bir seçenek.

**Şekil 4.14 · Kalan tutar yer tutucuda** (E0368)

- Debt action: Repay debt.
- Yer tutucu: 12.000,00 to Repay debt.
- Kalan tutar formun içinde yazılı.

**Şekil 4.15 · 12.000 → 7.000** (E0370)

- 5.000 girildi; borç 12.000'den 7.000'e indi.
- Diğer kart 5.000'de değişmeden kaldı.
- Tahsilat yalnız bağlandığı borcu düşürdü.

**Şekil 4.16 · Borç kayıtları toplamda** (E0398)

- Income 5.000 · Expenses −21.600 · net −16.600.
- Record taşıyan borç kayıtları gelir/gidere dâhil.
- Tahsilat gelire, borç verme gidere yazıldı.

- Kayıt listesinde iki satır bu iki yönü taşıyor: tahsilat Lending, renting kategorisiyle +5.000, borç verme Loan, interests kategorisiyle −5.000. Yani borç, kategori sisteminin içinden geçip gelir/gider toplamına giriyor.
- Bu, önceki sayfadaki ürünle taban tabana zıt bir sonuç: orada tahsilat iki hesap arasında bir aktarımdı ve gelir/gider toplamına hiç dokunmuyordu.
- Ödeme formundaki Debt action tam iki değer taşıyor: Repay debt ve Increase debt.
- Aynı karşı tarafa iki ayrı borç kartı açılabiliyor ve ikisi birleştirilmiyor; karşı taraf başına toplam gösterilmiyor.
- Hizmet faturası alacağı da I Lent formuyla giriliyor; ürün alacak ile borç vermeyi ayrı kavramlar olarak ayırmıyor.

## 4.4 · Alacak ve tahsilat, ürün ürün

Aynı üç soru: alacak nerede duruyor, doğduğu an hangi sayı değişiyor, tahsilat ne üretiyor.

|  | Bluecoins | Wallet | Money Manager | Hesap Defterim |
|---|---|---|---|---|
| Alacak nerede | *Canlı kare* · Bir hesap; listede kendi grubunda | *Canlı kare* · Ayrı bir nesne; Debts ekranında | *Canlı kare* · Cari türü bulunamadı; sıradan hesapla taklit edilir | *Canlı kare* · Ürün içinde yok; ayrı uygulamaya yönlendiriyor |
| Doğduğu an bakiye değişir mi | *Canlı kare* · Cari hesabın bakiyesi artar; banka değişmez | *Canlı kare* · Kullanıcıya soruluyor: kayıt oluşturulursa değişir | *Görülmedi* · Ölçülemedi — kavram yok | *Görülmedi* · Ölçülemedi — kavram yok |
| Alacak gelir sayılıyor mu | *Canlı kare* · Evet — fatura anında gelir yazıldı | *Canlı kare* · Evet — Record taşıyan borç kayıtları gelir/gidere giriyor | *Görülmedi* · Ölçülemedi | *Görülmedi* · Ölçülemedi |
| Tahsilat ne üretiyor | *Canlı kare* · Aktarım: cari −5.000, banka +5.000 | *Canlı kare* · Repay debt kaydı; borç 12.000 → 7.000 | *Görülmedi* · Ölçülemedi | *Görülmedi* · Ölçülemedi |
| Vade kavramı var mı | *Canlı kare* · Yok — cari hesap formu tarandı, vade alanı yok | *Canlı kare* · Evet — Due date, varsayılanı bir yıl sonrası | *Görülmedi* · Yok | *Görülmedi* · Yok |

- Goodbudget tabloda yok: hesap türleri arasında Debt grubu bulunmasına rağmen ücretsiz pakette ikinci hesap açılamadığı için hiçbir satır ölçülemedi.
- İki canlı üründe iki ayrı model olduğu için satırların çoğu yan yana karşılaştırılamıyor; ayrışma cevaplarda değil, sorunun ürüne uyup uymamasında.

## 4.5 · Kaynakta cari: vadesi ve iskontosu olan bir taraf

KolayBi'de cari birinci sınıf bir nesne ve formu canlı iki üründe bulunmayan alanlar taşıyor: vade günü, sabit iskonto ve durumu olan bir açılış bakiyesi. Alanların varlığı kanıtlı; kaydın sonucu görülmedi.

**Şekil 4.17 · KolayBi · Cari Detay Bilgileri** (E0195)

- Vade Günü ve Sabit İskonto: Yok / Var.
- Açılış Bakiyesi tutar, para birimi, durum, proje ve tarih taşıyor.
- Formda Borç Alacak Ekle ve Banka Ekle girişleri var.

**Şekil 4.18 · KolayBi · Cari listesi (demo adlar karartıldı)** (E0194)

- Cariler beş sekmeye ayrılmış.
- Her satırda renkli yerel bakiye ve cari tipi.
- Toplu seçim ve favori işaretleme var.

- Vade günü cari tarafında tanımlanıyor: taraf başına bir ödeme beklentisi. Wallet vadeyi borcun kendisine, KolayBi ise karşı tarafa bağlamış.
- Personel carileri ayrı bir sekme ve maaş ödemesi bu cari üzerinden yürüyor; listede bakiyeler hem artı hem eksi olabiliyor. Çalışan da bir karşı taraf.
- Personel carilerinde tipler Serbest / Yarı Zamanlı / Tam Zamanlı.

**Aynı soruda kaynaktan okunan üç ürün**

- *Kaynak beyanı* · **Paraşüt** — Kaynak üç borçlandırma yolu sayıyor: müşteri kaydında açılış bakiyesi, satış faturası, ve borç yokken ödeme ekleme (avans). Cari bakiye otomatik hesaplanıyor ve tahsilat en gecikmiş açık faturadan başlayarak mahsuplaşıyor. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **Logo İşbaşı** — Kaynağa göre tahsilat, borç-alacak ve ödeme işlemleri cari hesap içinde toplanıyor ve fatura kesilirken müşterinin bakiyesi anında görünüyor. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Kaynakta karşı taraf cari hesabı anlatılmıyor; ürün fatura kesip ödeme hatırlatması gönderiyor ama alacak bakiyesi kavramı belgelenmemiş.

## 4.6 · Alacağın yolu ve ayrıldığı noktalar

Alacak doğduğu andan tahsil edildiği ana kadar üç adım. İlk çatallanma daha alacağın nereye yazılacağında oluyor ve sonraki her şeyi belirliyor.

**1. Alacak doğuyor**

**→** *Bluecoins* — Cari bir hesap türü. Gelir kaydı o hesaba yazılıyor ve hesabın bakiyesi alacağı gösteriyor. (E0075)
**→** *Wallet* — Borç ayrı bir nesne. Yönle başlıyor (I Lent / I Borrowed) ve kendi ekranında yaşıyor. (E0316)
- *Money Manager · Hesap Defterim* — Kavram yok. Biri sıradan hesapla taklit gerektiriyor, diğeri ayrı bir uygulamaya yönlendiriyor. (E0177)

**2. Hesap bakiyesi kıpırdıyor mu**

- *Bluecoins* — Banka kıpırdamıyor; hareket eden şey cari hesabın bakiyesi. Gelir ise fatura anında yazılıyor. (E0075)
**→** *Wallet* — Ürün karar vermiyor, soruyor: kayıt oluşturulursa bakiye değişecek. İki yol da açık bırakılmış. (E0315)

**3. Tahsilat yapılıyor**

- *Bluecoins* — İki hesap arası aktarım: cari −5.000, banka +5.000. Gelir/gider toplamlarına dokunmuyor, çünkü gelir zaten yazılmıştı. (E0079)
**→** *Wallet* — Repay debt kaydı borcu düşürüyor — ama aynı kayıt gelir/gider toplamına da giriyor. (E0398)

- İki üründe iki ayrı model olduğu için yol tek bir çizgide birleşmiyor. Ortak olan tek şey şu: alacağın doğması ile paranın gelmesi iki ayrı an, ve ikisi aynı sayıyı değiştirmiyor.
- Vade iki üründe iki ayrı yere bağlanmış: Wallet borcun kendisine, KolayBi karşı tarafa. Bluecoins'in cari hesap formunda vade alanı yok.

## 4.7 · Neden ayrışıyorlar

Bu bölümde ürünler aynı soruya farklı cevap vermiyor — soruyu farklı soruyorlar. Alacağı bir hesap sayan ürünle ayrı bir nesne sayan ürün, sonrasındaki her adımı da farklı kurmak zorunda kalıyor.

### Alacağı hesap yapmak, tahsilatı aktarıma çeviriyor *(Çıkarım)*

Bluecoins'te cari bir hesap türü olduğu için alacak bir hesap bakiyesi, tahsilat da iki hesap arasındaki bir aktarım oluyor. Fatura anında gelir yazıldı ve banka kıpırdamadı; tahsilat anında banka arttı ve gelir kıpırdamadı.

Bu yapı aynı parayı iki kez gelir saymayı kendiliğinden engelliyor: para iki kez hareket ediyor ama yalnız biri gelir.

Karşılığında alacak hesap listesinde duruyor, ama VARLIKLAR bölümünde değil: kredi kartı ve ipotekle aynı CARİ HESAP bölümünde, yani borç tarafında. Net değere giriyor, varlık toplamına girmiyor; "tahsil edeceğim para" elimdeki paranın değil, kartın yanında okunuyor.

*Dayanağı: E0074, E0075, E0079, E0055, E0049, E0072, kullanıcı kontrolü (Bluecoins)*

### Wallet kararı kullanıcıya bırakıyor ve sonucunu yazıyor *(Çıkarım)*

Borç kaydedilirken ürün şunu soruyor: "Do you want to create a Record for this Debt? If you create a Record your balance will change." İki seçenek de açık; borç yalnız bir takip kaydı olarak kalabiliyor ya da hesap bakiyesini değiştiren gerçek bir işleme dönüşebiliyor.

Bu, incelenen dokuz ürün içinde bir kaydın bakiyeye dokunup dokunmayacağını kullanıcıya açıkça soran tek yüzey. Soru, cevabın sonucunu da aynı cümlede söylüyor.

Evet kolu seçildiğinde oluşan kayıt Loan, interests kategorisine düşüyor — yani borç, kategori sisteminin içinden geçiyor.

*Dayanağı: E0308, E0309, E0315, E0317*

### İki model, raporda taban tabana zıt iki sonuç *(Çıkarım)*

Alacağı hesap yapan üründe tahsilat bir aktarım ve gelir/gider toplamına hiç dokunmuyor. Borcu ayrı bir nesne yapan üründe ise tam tersi: borç kayıtları kategori sisteminin içinden geçiyor ve dönem raporuna giriyor.

Kayıt listesinde iki yön iki ayrı kategoriyle duruyor — tahsilat Lending, renting ile artı, borç verme Loan, interests ile eksi. Son otuz günün raporunda Income 5.000 ve Expenses −21.600 okunuyor.

Sonuç: aynı ekonomik olay bir üründe ayın gelirini değiştiriyor, ötekinde değiştirmiyor. İkisi de kendi modeline göre tutarlı; farkı üreten şey alacağın hesap mı yoksa ayrı bir nesne mi olduğu.

*Dayanağı: E0398, E0372, E0079, E0370*

### Aynı borç, iki ekranda ters işaret *(Çıkarım)*

Borç kartı "ADA REKLAM OWES ME 5.000,00" derken aynı borcun kayıt listesi "Total −5.000,00" gösteriyor. İkisi de kendi bakış açısından doğru: kart alacağı, liste o alacağı doğuran kaydın hesaptan çıkışını yazıyor.

Bu, kart bölümündeki iki borç sayısıyla aynı desen: bir ekran sonucu, öteki o sonucu üreten hareketi gösteriyor ve hangisinin hangisi olduğu ekranda yazmıyor.

*Dayanağı: E0316, E0317*

### Kavramın yokluğu da bir tasarım kararı *(Çıkarım)*

Hesap Defterim carileri ürünün dışına çıkarmış ve menüsünde ayrı bir uygulamaya yönlendiriyor. Money Manager'ın hesap listesinde böyle bir grup hiç yok; alacak ancak sıradan bir hesap açılarak taklit edilebilir.

İkisi de defter mantığını korumayı seçmiş: her satır gerçekleşmiş bir para hareketi. Henüz gelmemiş para bu mantığa girmiyor, bu yüzden ya dışarı çıkarılıyor ya hiç yok.

*Dayanağı: E0177, E0235*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Bluecoins** | Alacak bir hesap olduğu için tahsilat doğal bir aktarım; gelir bir kez yazılıyor ve kısmi tahsilat ayrı bir kavram gerektirmiyor | Tahsil edilmemiş alacak kartla aynı bölümde, borç tarafında duruyor; varlık toplamı onu göstermiyor ve cari hesap formunda vade alanı yok |
| **Wallet** | Borç ayrı bir nesne, yönü ve vadesi var; bakiyeye dokunup dokunmayacağını kullanıcıya soruyor ve sonucunu aynı cümlede yazıyor | Aynı borç iki ekranda ters işaretle görünüyor; borç kayıtları ayın gelir ve gider toplamına giriyor ve aynı karşı tarafa açılan iki borç birleştirilmiyor |
| **Money Manager** | — | Cari kavramı yok; alacak ancak sıradan bir hesap açılarak taklit edilebilir |
| **Hesap Defterim** | Defter mantığı bozulmuyor: her satır gerçekleşmiş bir para hareketi | Veresiye ürünün dışına çıkarılmış; kullanıcı ikinci bir uygulama kurmaya yönlendiriliyor |
| **KolayBi** | Cari birinci sınıf: vade günü, sabit iskonto ve durumu olan açılış bakiyesi taşıyor; personel de bir karşı taraf | Cari kartı çok alanlı: vade, iskonto, durumu olan açılış bakiyesi ve proje tek formda soruluyor; karşı taraf açmak canlı ürünlerdeki bir hesap açmaktan uzun bir iş |

**Belge 3'e taşınan soru**

- Tahsil edilmemiş alacak, elimdeki parayla aynı listede mi durmalı? Bluecoins ikisini aynı listede tutuyor ama alacağı kartla aynı bölüme koyuyor; Wallet ayrı bir ekrana alıyor.
- Bir kaydın bakiyeye dokunup dokunmayacağı kullanıcıya sorulmalı mı? Wallet bunu soruyor ve sonucunu yazıyor — dokuz üründe tek örnek.
- Vade kime ait olmalı: borcun kendisine mi, karşı tarafa mı? İki üründe iki ayrı yer.
- Alacak ve tahsilat ayın gelir/gider toplamına girmeli mi? İki ürün iki zıt cevap veriyor ve ikisi de kendi modeline göre tutarlı.
- Aynı tutarın iki ekranda ters işaretle görünmesi kaçınılabilir mi? Kart bölümündeki iki borç sayısıyla aynı desen burada da çıkıyor.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 4.1 | E0049 | Bluecoins | Canlı kare | Alacak bir hesap türü |
| 4.2 | E0316 | Wallet | Canlı kare | Borç ayrı bir nesne |
| 4.3 | E0358 | Wallet | Canlı kare | Borç yönle başlıyor |
| 4.4 | E0177 | Hesap Defterim | Canlı kare | Veresiye ayrı uygulamada |
| 4.5 | E0308 | Wallet | Canlı kare | Mevcut kayda bağla ya da atla |
| 4.6 | E0309 | Wallet | Canlı kare | Alanlar ve varsayılan vade |
| 4.7 | E0315 | Wallet | Canlı kare | Kayıt oluşturulsun mu |
| 4.8 | E0317 | Wallet | Canlı kare | Debt Records |
| 4.9 | E0074 | Bluecoins | Canlı kare | Cari hesaba gelir formu |
| 4.10 | E0075 | Bluecoins | Canlı kare | Cari 12.000 |
| 4.11 | E0079 | Bluecoins | Canlı kare | Cari 7.000 |
| 4.12 | E0055 | Bluecoins | Canlı kare | Dönem karşılaştırması |
| 4.13 | E0367 | Wallet | Canlı kare | Kayıt nasıl eklensin |
| 4.14 | E0368 | Wallet | Canlı kare | Kalan tutar yer tutucuda |
| 4.15 | E0370 | Wallet | Canlı kare | 12.000 → 7.000 |
| 4.16 | E0398 | Wallet | Canlı kare | Borç kayıtları toplamda |
| 4.17 | E0195 | KolayBi | Kaynak görseli | KolayBi · Cari Detay Bilgileri |
| 4.18 | E0194 | KolayBi | Kaynak görseli | KolayBi · Cari listesi (demo adlar karartıldı) |
