# Bölüm 7 · Raporun neyi saydığı

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Rapordaki sayı hangi kayıtları, hangi dönemi ve hangi tanımı içeriyor?

Önceki bölümler kayıtların nasıl oluştuğunu izledi. Bu bölüm o kayıtların toplandığı yere bakıyor ve tek bir şeyi sorguluyor: ekrandaki sayı tam olarak neyin toplamı.

Üç ayrı yerde ayrışma çıkıyor. Raporun sorduğu soru, varsayılan dönemi, ve aynı görünen iki etiketin aynı şeyi saymaması.

**Bu bölüme girmez**

- Toplamların içine neyin girdiği → Bölüm 1 ve 2
- Kart borcunun iki ayrı sayıyla görünmesi → Bölüm 3
- Kapsam kırılımı → Bölüm 6
- Raporun dosyaya aktarılması → Bölüm 8

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare | İstatistik, Toplam sekmesi ve filtre paneli kareli. |
| Bluecoins | Canlı kare | Net Kazançlar ve net varlık ekranları kareli. |
| Wallet | Canlı kare | Cash-flow ve Spending ekranları kareli. |
| Goodbudget | Canlı kare | İki rapor ekranı ve varsayılan dönem kareli. |
| Hesap Defterim | Canlı kare | Okuma yüzeyi dönem filtreli liste olarak kareli. |
| KolayBi | Kaynak görseli | Nakit akış ve gelir/gider raporları destek görselinde. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 7.1 · Rapor hangi soruyu soruyor

Dört ürünün rapor ekranı dört ayrı soruyla açılıyor. Biri doğrudan kullanıcının cümlesiyle konuşuyor, biri kategoriye bölüyor, biri dönemleri yan yana koyuyor, biri geliri ve harcamayı karşılaştırıyor.

**Şekil 7.1 · Cash-flow** (E0280)

- Başlık bir soru: "Am I spending less than I make?"
- Net 22.950; Income 25.000; Expenses −2.050.
- Rapor kullanıcının diliyle konuşuyor.

**Şekil 7.2 · İstatistik · Ay** (E0231)

- Gelir ve Gider iki ayrı sekme.
- Pasta: Diğer %58,5 · Yiyecek %41,5.
- Soru kategoriye bölünmüş harcama.

**Şekil 7.3 · Net Kazançlar** (E0023)

- İki dönem yan yana sütun olarak.
- Kategori ve alt kategori satırları.
- Soru dönemler arası karşılaştırma.

**Şekil 7.4 · Reports** (E0423)

- İki rapor alt alta: zarf harcaması ve
- gelir-harcama karşılaştırması.
- Net Total tek satırda veriliyor.

- Wallet raporu bir soru cümlesiyle açıyor ve cevabı tek satırda veriyor. Diğer üçü sayıyı önce gösterip yorumu kullanıcıya bırakıyor.
- Hesap Defterim'de ayrı bir rapor ekranı yok: "Aylık" seçildiğinde açılan şey dönem filtresi uygulanmış aynı liste. Adı rapor olsa da içeriği liste.
- Money Manager'da aynı veri üçüncü bir yüzeyde de okunuyor: takvim görünümü günleri renklendirip gün başına tutar yazıyor.
- Bluecoins'te de takvim günlük gelir, gider ve net ayrımı gösteriyor.

## 7.2 · "Bu dönem" her üründe başka bir dönem

Rapor açıldığında hangi aralığın seçili geldiği küçük bir ayrıntı gibi duruyor ama sonucu doğrudan değiştiriyor. Dört üründe dört ayrı varsayılan.

**Şekil 7.5 · Son 12 hafta** (E0284)

- Seçili aralık 12W / LAST 12 WEEKS.
- Takvim ayı değil, kayan bir pencere.
- 6M ve 1Y ücretli pakette.

**Şekil 7.6 · Cari takvim ayı** (E0121)

- Rapor içinde bulunulan ayda açılıyor.
- Dönem seçimi üst bardaki takvim simgesinden.

**Şekil 7.7 · Toplam · ay** (E0250)

- Dönem bir takvim ayı: 1.08 ~ 31.08.
- Gider ödeme kaynağına göre bölünmüş.
- Altta Excel olarak gönderme girişi.

**Şekil 7.8 · Özet · gün gün** (E0148)

- Çipler: Herşey / Haftalık / Aylık / Yıllık.
- Günlük çipi yok.
- Gün gün Alındı, Ödendi ve Tasarruf.

- Wallet'ın raporu takvim ayıyla değil kayan bir pencereyle açılıyor: bu karede son 12 hafta, Şekil 7.1'de son 30 gün. Aynı veriyle "bu ay ne harcadım" sorusunun cevabı, kullanıcı aralığa bakmazsa bir aydan uzun bir toplam olabiliyor.
- Bluecoins'in dönem seçicisi 30 Gün gibi kayan pencereler de sunuyor; aynı ekranda hem takvim ayı hem gün sayısı seçilebiliyor.
- Hesap Defterim'in "Aylık" görünümü ayrı bir rapor değil, dönem filtresi uygulanmış liste.
- Bluecoins'in dönem ve işlem tipi seçicileri ana ekranın üstünde.
- Hesap Defterim'in Haftalık ve Yıllık çipleri yalnız kendi aralığının hareketlerini topluyor; öncesi ayrı bir "Önceki denge" satırıyla taşınıyor. Hareketsiz bir haftada Ana Hesap defterinin geliri ve gideri 0, dengesi yine 43.150.

## 7.3 · Aynı görünen iki etiket, iki ayrı toplam

Bu bölümün en sessiz tuzağı. İki ekran neredeyse aynı adı taşıyor ama biri dönem içindeki akışı, öteki elde kalan varlığı sayıyor. Hangisinin hangisi olduğu ekranda yazmıyor.

**Şekil 7.9 · Net Kazanç · varlık** (E0032)

- Benzer iki etiket, iki ayrı toplam.
- Biri dönem akışı, öteki varlık.
- Ayrım ekranda yazılı değil.

**Şekil 7.10 · İkisi aynı ekranda** (E0053)

- Net Kazançlar: dönem akışı 21.350.
- Net Kazanç: Varlıklar 43.850, Cari hesap −500.
- İki blok alt alta duruyor.

**Şekil 7.11 · Filtre paneli** (E0251)

- Sütunlar: Gelir / Giden Havale · Gider / Gelen Havale.
- Aktarım gelir ve giderden ayrı kolonlarda.
- Hesap başına dört sayı.

**Şekil 7.12 · Gider ödeme kaynağına göre** (E0405)

- Gider (Nakit, Banka Hesapları) ayrı.
- Gider (Kredi Kartı, Ödeme) ayrı ve parantezli.
- Havale kendi satırında, 0,00.

- Money Manager aynı tuzağı tersine çeviriyor: toplamı tek bir sayı olarak göstermek yerine, neyin dâhil olduğunu satır adlarıyla yazıyor. Aktarım kendi satırında duruyor ve sıfır olduğu görülüyor.
- Bluecoins'te ise iki blok alt alta ve adları birbirine çok yakın. Kullanıcının hangi bloğa baktığını ad değil, blokta görünen kalemler belli ediyor.
- Money Manager'ın Toplam sekmesi aynı ayrımı Ağustos için de yapıyor: nakit gideri ayrı, kart gideri ayrı ve ödenen tutar parantezde.
- Bluecoins'in tümünü göster raporu iki tarihi yan yana koyarak VARLIKLAR ve CARİ HESAP bölümlerini, altındaki grup ve hesaplarla ayrı ayrı veriyor.
- Money Manager'ın filtre paneli toplamı anında yeniden kuruyor: yalnız kart seçilince gider o kartın harcamasına, 1.000'e iniyor ve payı %45 yazılıyor; havale yine ayrı bir satırda.

## 7.4 · Raporun kendi içinde tutmadığı bir an

Goodbudget'ın iki rapor ekranı aynı anda çekildi. Harcama raporu negatif bir toplam gösteriyor ve o negatif tutarın içinde gelir var — yani bir gelir kaydı harcama raporuna düşmüş.

**Şekil 7.13 · Toplam negatif** (E0123)

- Harcama raporunun toplamı eksi çıkıyor.
- −22.950 = 1.200 + 850 − 25.000.
- 25.000'lik gelir bu toplamın içinde.

**Şekil 7.14 · Aynı andaki tablo** (E0124)

- Aynı dönem, ikinci rapor ekranı.
- Grafikteki çubuk ile tablodaki sayı
- görsel olarak örtüşmüyor.

- Aritmetik tutuyor: harcama raporunun toplamı, iki gideri ve bir geliri aynı kovaya koyduğunda tam olarak −22.950 veriyor. Yani ekran bir hesap hatası yapmıyor; kaydı yanlış kovaya koyuyor.
- Bunun bir ürün kusuru mu yoksa zarf modelinin bir sonucu mu olduğu bu koşumdan çıkmıyor. Ölçülen şey sonucun kendisi: harcama raporu gelir içeriyor.
- Aynı dönemin Income vs Spending tablosunda da aynı −22.950 duruyor; iki ekran birbiriyle tutarlı.
- Bölüm 1'de görülen iki defterli yapı bu ürünün raporunu da etkiliyor: rapor zarf katmanından besleniyor.

## 7.5 · Rapor yüzeyleri, ürün ürün

Raporun sorduğu soru, varsayılan dönemi, aktarımı nasıl gösterdiği ve kategori kırılımı.

|  | Money Manager | Bluecoins | Wallet | Goodbudget | Hesap Defterim |
|---|---|---|---|---|---|
| Ayrı bir rapor ekranı var mı | *Canlı kare* · Evet — İstatistik ve Toplam sekmeleri | *Canlı kare* · Evet — Net Kazançlar ve net varlık | *Canlı kare* · Evet — Cash-flow ve Spending | *Canlı kare* · Evet — iki ayrı rapor ekranı | *Canlı kare* · Hayır — dönem filtreli liste |
| Varsayılan dönem | *Canlı kare* · Takvim ayı | *Canlı kare* · Kayan pencere ve takvim ayı birlikte | *Canlı kare* · Kayan pencere: son 30 gün ya da 12 hafta | *Canlı kare* · Cari takvim ayı | *Canlı kare* · Çip seçimi: Herşey / Haftalık / Aylık / Yıllık |
| Aktarım raporda nasıl görünüyor | *Canlı kare* · Kendi satırında ve kendi kolonlarında | *Canlı kare* · Gün içinde netleşiyor; toplamda yok | *Canlı kare* · Çift satır; dönem toplamına etkisi yok | *Görülmedi* · Aktarım koşulamadı | *Canlı kare* · Alındı ve Ödendi toplamlarının içinde |
| Kategori kırılımı | *Canlı kare* · Pasta ve yüzde listesi | *Canlı kare* · Kategori ve alt kategori satırları | *Canlı kare* · Donut; Categories/Labels geçişi | *Canlı kare* · Zarf başına | *Canlı kare* · Yok |
| Dikkat gerektiren yer | *Canlı kare* · Gider ödeme kaynağına göre bölünmüş; okumadan önce satır adı okunmalı | *Canlı kare* · Benzer adlı iki blok: dönem akışı ve varlık | *Canlı kare* · Varsayılan aralık takvim ayı değil | *Canlı kare* · Harcama raporunun toplamı gelir içeriyor | *Canlı kare* · Toplamlar açılış ve aktarım bacaklarını içeriyor |

- Son satır bu bölümün özeti: beş ürünün beşinde de rapordaki sayıyı doğru okumak için ekranda yazmayan bir şeyi bilmek gerekiyor.

## 7.6 · Kaynakta rapor: nakit akışı ve dönem kovaları

KolayBi'nin rapor yüzeyi on sekmeye ayrılmış. Nakit akış raporu canlı beş üründe bulunmayan bir şey yapıyor: gelecek dönemleri kova kova gösterip tahmini dönem sonu bakiyesini yazıyor. Alanların varlığı kanıtlı; davranış görülmedi.

**Şekil 7.15 · KolayBi · Nakit Akış Raporu** (E0216)

- Güncel Bakiye, Tahsilatlar, Ödemeler üstte.
- Tahmini Dönem Sonu Bakiyesi ayrı bir sayı.
- Tabloda aylık kovalar ve bir Belirsiz kovası.

**Şekil 7.16 · KolayBi · Gelir/Gider Raporu** (E0215)

- Gelirler ve Giderler satır, dört para birimi kolon.
- Kolonlar arasında Ödeme Yöntemi ve Banka da var.
- Gelirler ve Giderler ayrı alt sekmeler.

- "Belirsiz" kovası dikkat çekici: vadesi belli olmayan tahsilat ayrı bir kovada duruyor ve aylık kovalara dağılmıyor. Canlı ürünlerde vadesi belirsiz para için böyle bir yer yok.
- Gelir/gider raporunun kolonları arasında ödeme yöntemi ve banka bulunuyor; rapor yalnız tutarı değil, paranın hangi yoldan geçtiğini de taşıyor.
- Rapor yüzeyi on sekmeye bölünmüş ve filtreler arasında cari, proje, etiket, şube ve vade aralığı var.

**Aynı soruda kaynaktan okunan üç ürün**

- *Kaynak beyanı* · **Paraşüt** — Kaynak nakit akışı ve yaşlandırma raporlarından söz ediyor; tahsilat kaydedilince nakit akışı ve yaşlandırma raporunun güncellendiği yazılı. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **Logo İşbaşı** — Kaynak tarih aralığına göre kategorize raporlar ve görsel grafik vaat ediyor; rapor içeriği anlatılmıyor. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Kaynağa göre kâr-zarar, nakit akışı geçmişi ve genel bakış panosu kategorize edilen kayıtlardan türetiliyor; ayrıca üç aylık tahmini vergi hesaplanıyor.

## 7.7 · Sayının rapora gelene kadarki yolu

Bir kayıt yazıldıktan sonra rapordaki sayıya dönüşene kadar üç eşikten geçiyor. Her eşikte ürünler ayrı kararlar vermiş.

**1. Kayıt yazılıyor**

Beş üründe de kayıt bir tutar, bir tarih ve bir sınıflandırma taşıyor. Buraya kadar rapor devrede değil.


**2. Hangi dönemin içinde sayılıyor**

- *Money Manager · Goodbudget* — Takvim ayı. Rapor içinde bulunulan ayda açılıyor. (E0121)
**→** *Wallet* — Kayan bir pencere: son 30 gün ya da son 12 hafta. Takvim ayı ayrı bir sayfadan seçiliyor. (E0284 · E0280 · E0439)
- *Bluecoins · Hesap Defterim* — Kullanıcı seçiyor: kayan pencere ile takvim dönemi aynı ekranda. (E0148)

**3. Hangi toplama giriyor**

**→** *Money Manager* — Gider ödeme kaynağına göre bölünüyor ve aktarım kendi satırında duruyor; neyin dâhil olduğu satır adında yazılı. (E0405)
- *Bluecoins · Wallet* — Aktarım netleşiyor, gelir/gider toplamına girmiyor. (E0279)
**→** *Hesap Defterim* — Her satır toplama giriyor: açılış bakiyesi ve aktarım bacakları dâhil. (E0142)
**→** *Goodbudget* — Bir gelir kaydı harcama raporunun toplamına girmiş; toplam eksi çıkıyor. (E0123)

**4. Ekranda okunuyor**

- *Wallet* — Sayının yanında soru cümlesi: "Am I spending less than I make?" (E0280)
**→** *Bluecoins* — Benzer adlı iki blok alt alta: biri dönem akışı, öteki varlık. Ayrım ekranda yazılı değil. (E0032)
- *Money Manager · Hesap Defterim* — Sayı önce, yorum kullanıcıda. Money Manager satır adlarıyla kapsamı yazıyor. (E0250)

- İkinci ve üçüncü eşik bu bölümün ayrım noktaları: aynı kayıt farklı dönemlere düşebiliyor ve aynı adlı toplam farklı şeyleri sayabiliyor.
- Dördüncü eşikte tek fark okunabilirlik: hiçbir ürün yanlış hesaplamıyor, ama hesabın tanımını yazan ürün sayısı az.

## 7.8 · Neden ayrışıyorlar

Rapor bir hesap makinesi değil, bir tanım. Ürünler aynı kayıtları farklı tanımlarla topluyor ve o tanımı ekranda yazıp yazmamakta da ayrışıyorlar.

### Varsayılan dönem sessizce sonucu değiştiriyor *(Çıkarım)*

Wallet'ın raporu takvim ayıyla değil kayan bir pencereyle açılıyor — karelerde son 30 gün ve son 12 hafta; takvim ayı ancak ayrı bir sayfadan seçiliyor. Goodbudget ve Money Manager takvim ayıyla açılıyor. Aynı veriyle "bu ay ne harcadım" sorusunun cevabı, kullanıcı aralığa bakmazsa üründen ürüne değişiyor.

Fark ekranda görünüyor ama okunmayı bekliyor: aralık bir çip ya da bir başlık. Sayıya bakan kullanıcı aralığa bakmayabilir.

*Dayanağı: E0284, E0280, E0439, E0438, E0121, E0250, E0148*

### Aynı adlı iki toplam aynı şeyi saymıyor *(Çıkarım)*

Bluecoins'te iki blok alt alta duruyor ve adları birbirine çok yakın. Biri dönem içindeki akışı — gelir eksi gider — öteki elde kalan varlığı sayıyor. Koşumda biri 21.350, öteki 43.350 okundu: Varlıklar 43.850 ile "Cari hesap" −500'ün toplamı. "Cari hesap" satırı Bluecoins'in cariyi ve kartı birlikte topladığı bölüm; o anda içindeki tek tutar kart borcuydu.

İkisi de doğru ve ikisi de gerekli; sorun adların ayırt edici olmaması. Kullanıcı hangi bloğa baktığını addan değil, bloğun içindeki kalemlerden anlıyor.

Bu, kart bölümündeki iki borç sayısı ve borç bölümündeki ters işaretle aynı desen: ürün iki farklı soruyu cevaplıyor, hangisinin hangisi olduğunu yazmıyor.

*Dayanağı: E0032, E0053, E0052*

### Money Manager tanımı ekrana yazmayı seçmiş *(Çıkarım)*

Aynı tuzağın tersi de var. Money Manager toplamı tek bir sayı olarak göstermek yerine, gideri ödeme kaynağına göre bölüyor: nakit ve banka gideri ayrı satır, kart gideri ayrı satır ve ödenen tutar parantezde. Aktarım da kendi satırında duruyor ve sıfır olduğu okunuyor.

Filtre panelinde aynı ilke daha da açık: sütunlar "Gelir / Giden Havale" ve "Gider / Gelen Havale" olarak adlandırılmış, yani aktarımın hangi tarafta sayıldığı adın içinde.

Bedeli okuma yükü: kullanıcı tek bir sayı yerine dört satır okuyor.

*Dayanağı: E0405, E0251, E0250*

### Bir raporda gelir, harcama toplamının içine girmiş *(Çıkarım)*

Goodbudget'ın harcama raporu negatif bir toplam gösteriyor ve aritmetik bunun nedenini veriyor: iki gider ile bir gelir aynı kovada toplanınca −22.950 çıkıyor. Aynı dönemin ikinci rapor ekranı da aynı sayıyı gösteriyor, yani iki ekran kendi aralarında tutarlı.

Ürün hesap hatası yapmıyor; kaydı yanlış kovaya koyuyor. Bunun bir kusur mu yoksa zarf modelinin bir sonucu mu olduğu koşumdan çıkmıyor — ölçülen şey sonucun kendisi.

*Dayanağı: E0123, E0124*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Money Manager** | Toplamın tanımını satır adlarına yazıyor: gider ödeme kaynağına göre bölünmüş, aktarım kendi satırında ve sıfır olduğu görülüyor | Tek bakışta okunan bir sayı yok; kullanıcı dört satır okumak zorunda |
| **Bluecoins** | Dönem akışı ile varlığı ayrı ayrı veriyor ve iki dönemi yan yana koyup karşılaştırma yapıyor | İki bloğun adı birbirine çok yakın; hangisinin akış hangisinin varlık olduğu ekranda yazmıyor |
| **Wallet** | Rapor kullanıcının cümlesiyle açılıyor ve cevabı tek satırda veriyor; kategori ile etiket iki ayrı kırılım olarak okunabiliyor | Rapor aralığı takvim ayı değil kayan bir pencere; aralığa bakmayan kullanıcı son 30 günü ya da 12 haftayı "bu ay" sanabilir |
| **Goodbudget** | Gelir ile harcamayı tek grafikte karşılaştırıyor ve zarf başına kırılım veriyor | Harcama raporunun toplamı bir gelir kaydını içeriyor ve eksi çıkıyor |
| **Hesap Defterim** | Gün gün Alındı, Ödendi ve Tasarruf veren bir özet var | Ayrı bir rapor ekranı yok; toplamlar açılış ve aktarım bacaklarını içeriyor ve kategori kırılımı hiç bulunmuyor |

**Belge 3'e taşınan soru**

- Raporun varsayılan dönemi takvim ayı mı olmalı, kayan bir pencere mi? İki seçenek aynı veriyle iki ayrı sayı veriyor.
- Bir toplamın neyi içerdiği ekranda yazmalı mı? Money Manager yazıyor ve okuma yükü artıyor; diğerleri yazmıyor ve yanlış okuma riski kalıyor.
- Akış ile varlık aynı ekranda durabilir mi? Duruyorsa adları nasıl ayrışmalı?
- Rapor ekranları birbiriyle tutarlı olmalı mı, yoksa her ekran kendi tanımını mı taşımalı? Bir üründe iki ekran tutarlı ama ikisi de aynı kovalama sorununu taşıyor.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 7.1 | E0280 | Wallet | Canlı kare | Cash-flow |
| 7.2 | E0231 | Money Manager | Canlı kare | İstatistik · Ay |
| 7.3 | E0023 | Bluecoins | Canlı kare | Net Kazançlar |
| 7.4 | E0423 | Goodbudget | Canlı kare | Reports |
| 7.5 | E0284 | Wallet | Canlı kare | Son 12 hafta |
| 7.6 | E0121 | Goodbudget | Canlı kare | Cari takvim ayı |
| 7.7 | E0250 | Money Manager | Canlı kare | Toplam · ay |
| 7.8 | E0148 | Hesap Defterim | Canlı kare | Özet · gün gün |
| 7.9 | E0032 | Bluecoins | Canlı kare | Net Kazanç · varlık |
| 7.10 | E0053 | Bluecoins | Canlı kare | İkisi aynı ekranda |
| 7.11 | E0251 | Money Manager | Canlı kare | Filtre paneli |
| 7.12 | E0405 | Money Manager | Canlı kare | Gider ödeme kaynağına göre |
| 7.13 | E0123 | Goodbudget | Canlı kare | Toplam negatif |
| 7.14 | E0124 | Goodbudget | Canlı kare | Aynı andaki tablo |
| 7.15 | E0216 | KolayBi | Kaynak görseli | KolayBi · Nakit Akış Raporu |
| 7.16 | E0215 | KolayBi | Kaynak görseli | KolayBi · Gelir/Gider Raporu |
