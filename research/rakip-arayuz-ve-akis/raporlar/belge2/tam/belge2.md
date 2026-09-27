# Belge 2 · Rakip finansal akışlar

> Birleşik okunabilir kopya: bölüm Markdown dosyaları sırayla ve kanıt eki. İşaretler, rozetler ve sayfa düzeni yalnız PDF'te görünür.


---

# Bölüm 1 · Gelir ve gider kaydı

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Aynı parayı beş ürüne yazdığımızda hangi sayılar değişiyor, hangileri değişmiyor?

Beş üründe aynı çekirdek veri var: 22.000 açılış, 25.000 hizmet geliri, 850 ve 1.200 gider, 3.000 hesap aktarımı, 1.200 kart ödemesi. Bu bölüm tek soruyu izliyor — kayıt tuşuna basıldıktan sonra ekrandaki hangi sayı kıpırdıyor.

Kaydın kendisi beş üründe de sorunsuz oluşuyor. Ayrışma kayıttan sonra başlıyor ve iki yerde toplanıyor: ayın gelir/gider toplamının neyi saydığı, ve kaydın kaç deftere birden yazıldığı.

**Bu bölüme girmez**

- Kart harcaması ve taksit → Bölüm 3
- Hesaplar arası aktarımın kendisi → Bölüm 2
- Raporun dönem ve filtre davranışı → Bölüm 7
- Formun alan yerleşimi ve görsel dili → Belge 1 §4

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare | Form, liste, istatistik ve hesap toplamı kareli. |
| Bluecoins | Canlı kare | Form, liste ve dönem raporu kareli. |
| Wallet | Canlı kare | Hızlı form, Records ve Cash-flow kareli. |
| Hesap Defterim | Canlı kare | Defter modeli ve birleşik rapor kareli. |
| Goodbudget | Canlı kare | Gelir yolu ayrı bir koşumda uçtan uca izlendi. |
| KolayBi | Kaynak görseli | Fatura formu destek görselinde; davranış görülmedi. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 1.1 · Form ne soruyor: aynı kaydı beş ayrı biçimde yazmak

Üç üründe form aynı üç şeyi soruyor — tutar, hesap, kategori. Dördüncüsü kategoriyi hiç sormuyor, beşincisi hesabın yerine zarf soruyor. Ayrışma buradan değil, bu formların sonucundan çıkıyor.

**Şekil 1.1 · İşlem formu** (E0229)

- Üstte Gelir / Gider / Havale seçici; karede Gider açık.
- Tutar · Kategori · Hesap · Not.
- Kategori 11 kutuluk panelden seçiliyor.

**Şekil 1.2 · İşlem formu, GELİR seçili** (E0020)

- +25.000 TRY, 3 Ağustos 2026.
- Kategori Diğer/Diğer, hesap Banka/Ana Hesap.
- Altta Planlı İşlemler · Bölmek · Durum · Etiket.

**Şekil 1.3 · Hızlı form** (E0277)

- Tür bir sekme; karede Expense seçili ve zeminle birleşik.
- Tutar, Account ANA HESAP, Category SALE.
- Ayrıntı kaydettikten sonra açılıyor.

**Şekil 1.4 · Alındı formu** (E0138)

- Alındı / Ödendi seçici; kategori alanı yok.
- Tutar 25.000 ve serbest metin Notlar.
- Kaydın adı yalnız bu nottan geliyor.

**Şekil 1.5 · From New Income, dolu** (E0419)

- Received from · Amount · Account · Date.
- Hesabın yanında How to fill Envelopes.
- Zarf seçilmeden kayıt tamamlanmıyor.

- Money Manager ve Bluecoins türü bir seçiciyle, Wallet bir sekmeyle, Hesap Defterim iki adlandırılmış düğmeyle soruyor. Hesap Defterim'de bu iki düğmenin adı kullanıcı tarafından değiştirilebiliyor — koşumda Alındı/Ödendi, Tahsilat/FaturaOdemesi oldu ve bütün ekrandaki başlıklar onunla birlikte değişti.
- Goodbudget'ta hesap alanı formda var ama kaydı tamamlayan şey zarf seçimi; zarf seçilmeden gelir reddediliyor.
- Hesap Defterim kategoriyi ayrı bir serbest metin kutusunda soruyor (Açıklama / Kategori) ve bu alan seçici değil, yazılan metin.
- Bluecoins'in Bölmek alanı tek kaydı birden çok satıra bölüyor: her parça kendi tutarını, kategorisini, hesabını, durumunu ve etiketini taşıyor. Koşumda yazılan 50 + 60'lık kayıt listede tek satır olarak, "2 Kategoriler" diye duruyor.
- Wallet'ta tür ile kategori birbirini denetlemiyor: Expense seçiliyken kategori SALE duruyor; yanlış tür seçilmiş kayıt formda kendini ele vermiyor.

## 1.2 · Kayıttan sonra hangi sayı kıpırdadı

Aynı çekirdek veri girildikten sonra her üründe okunan değerler. Beş üründen dördü aynı iki sayıda buluşuyor; ayrışan iki ürün iki ayrı nedenle ayrışıyor.

|  | Money Manager | Bluecoins | Wallet | Hesap Defterim | Goodbudget |
|---|---|---|---|---|---|
| Hesap bakiyesi | *Canlı kare* · Varlıklar 44.950 | *Canlı kare* · Net 44.950 | *Canlı kare* · Üç hesap toplamı 22.950 (kart öncesi) | *Canlı kare* · Denge 44.950 | *Canlı kare* · 41.734 — gelir kaydı bakiyeyi değiştirmedi |
| Ayın gelir toplamı | *Canlı kare* · Gelir 25.000 (Ağu) | *Canlı kare* · 25.000 (Ağu) | *Canlı kare* · Income 25.000 | *Canlı kare* · Toplam Alındı 51.200 — açılış ve aktarım bacakları dâhil | *Canlı kare* · Income 3.284 (Eyl) — zarfa giren para |
| Ayın gider toplamı | *Canlı kare* · Gider 2.050 | *Canlı kare* · GİDER −2.050 | *Canlı kare* · Expenses −2.050 | *Canlı kare* · Toplam Ödendi 6.250 | *Canlı kare* · Spending 0 (Eyl) |
| Kategori kırılımı | *Canlı kare* · Pasta: Diğer %58,5 · Yiyecek %41,5 | *Canlı kare* · Kategori ve alt kategori satırları | *Canlı kare* · Donut: Communication, PC, Food & Drinks | *Canlı kare* · Yok — okuma yüzeyi dönem filtreli liste, kırılım taşımıyor | *Canlı kare* · Zarf başına kalan |
| Aktarım ay toplamına giriyor mu | *Canlı kare* · Hayır — aktarım satırının gün başlığı 0/0 | *Canlı kare* · Hayır — iki bacak aynı günde netleşiyor | *Canlı kare* · Hayır — çift satır, net etki 0 | *Canlı kare* · Evet — iki bacak da Alındı/Ödendi'ye giriyor | *Görülmedi* · Transfer ücretli pakette; tek hesapla denenemedi |
| Silinen kayıt geri alınabiliyor mu | *Canlı kare* · Hayır — silme onaylı; menü ızgarasında çöp kutusu yok | *Koşum kaydı* · Evet — çöp kutusundan geri yükleniyor; kayıt listeye yeniden açılınca dönüyor | *Canlı kare* · Hayır — silme onaylı; çekmece ve ayarlarda çöp kutusu yok | *Canlı kare* · Silinmiş işlemler listesi + Geri Yükle | *Canlı kare* · Hayır — tek onayla siliniyor; ayarlarda geri alma yok |
| Kaydın adı nereden geliyor | *Canlı kare* · Not alanı | *Canlı kare* · Not alanı | *Canlı kare* · Note / Payee | *Canlı kare* · Notlar (tek kimlik kaynağı) | *Canlı kare* · Received from |

- Money Manager, Bluecoins ve Wallet'ın satırları Ağustos 2026 dönemine, Goodbudget'ınki Eylül 2026'ya ait; bu ürünün gelir yolu ayrı bir koşumda izlendi. Dönem farkı yalnız bu satırda sonucu değiştirdiği için yazıldı.
- Wallet'ın hesap toplamı kart ödemesi öncesi karede okundu; diğer dördü koşum sonu değerleridir.
- Tutar düzenlemesi yalnız Money Manager'da ölçüldü: sonradan eklenen bir kaydın tutarı 0'dan 580'e çıkarılınca ayın gideri aynı anda 580 arttı; onay veya yeniden hesaplama adımı yok. Diğer dört üründe denenmedi.

## 1.3 · Aynı ay, aynı veri, iki ayrı gider toplamı

Ağustos'un gideri üç üründe 2.050, birinde 6.250. İkisi de kendi tanımına göre doğru; aradaki 4.200 tam olarak hesaplar arası aktarımın ve kart ödemesinin iki bacağı.

**Şekil 1.6 · İstatistik · Ağustos** (E0231)

- Gelir 25.000 · Gider 2.050.
- Aktarım ve kart ödemesi toplamda yok.

**Şekil 1.7 · Net Kazançlar · Ağustos** (E0023)

- GİDER −2.050; aynı iki gider.
- Aktarımın iki bacağı gün netini 0 yapıyor.

**Şekil 1.8 · Bütün Hesaplar · Aylık** (E0142)

- Toplam Alındı 51.200 · Ödendi 6.250.
- Denge 44.950 — diğer ürünlerle aynı net.
- Kategori kırılımı yok.

- Hesap Defterim'in 6.250'si şu toplam: gerçek gider 2.050 + aktarımın çıkış bacağı 3.000 + kart ödemesinin çıkış bacağı 1.200 — dördü de karede okunuyor. 51.200'ün içindeki 22.000'lik açılış satırları ise listenin kesilen alanında kalıyor ve aritmetikle bulunuyor: 25.000 gelir + 4.200 giriş bacağı + 22.000 açılış.
- Hesap Defterim yine de doğru nete varıyor: 51.200 − 6.250 = 44.950. Money Manager'ın Toplam satırı ve Bluecoins'in neti de aynı sayıyı veriyor. Ayrışan şey net değil, netin iki bileşeni.
- Wallet aynı dönemde Income 25.000 / Expenses −2.050 okuyor; üç ürün bu ikilide birebir aynı.
- Hesap Defterim'in gözlem formu bu toplamı kendi ölçüsünde "karışık" diye işaretliyor: açılış + transfer + gerçek gelir/gider.

## 1.4 · Goodbudget: gelir girdi, rapora yazıldı, hesap kıpırdamadı

Bu ürünün gelir yolu ayrı bir koşumda uçtan uca izlendi: tek bir kayıt girildi ve sonucu her katmanda ayrı ayrı okundu. Kayıt 1.234, Beta Tasarim, hesap Ana Hesap, tamamı Tasarim Yazilimi zarfına. Dört kare kaydın önce ve sonrasını gösteriyor.

**Şekil 1.9 · Total 43.784,00** (E0417)

- Tasarim Yazilimi zarfı 0,00.
- Available 20.000,00.

**Şekil 1.10 · Total 45.018,00** (E0420)

- Tasarim Yazilimi 1.234,00.
- Available değişmedi.

**Şekil 1.11 · All Accounts 41.734,00** (E0418)

- Tek hesap: Ana Hesap.
- Zarf toplamıyla farkı 2.050.

**Şekil 1.12 · All Accounts 41.734,00** (E0421)

- Değişmedi.
- Fark 2.050'den 3.284'e çıktı.

- İşlem listesi kaydı hesaba atfedilmiş gösteriyor: 09/20 Beta Tasarim +1.234,00 Ana Hesap. Rapor da geliri sayıyor: Eylül Income 2.050 → 3.284.
- Uygulama kapatılıp yeniden açıldıktan sonra hesap yine 41.734,00. Bunun nedeni — senkronizasyon, ücretsiz paket sınırı veya tasarım tercihi — bu koşumdan çıkmıyor.
- Zarf toplamı ile hesap arasındaki fark tam olarak kaydın tutarı kadar büyüdü: 2.050 → 3.284.
- Eylül raporu Income 3.284 · Spending 0 · Net Total 3.284.
- Sıradan gider kayıtları hesaba işliyor: açılış 20.000 → 25.000 gelir → −850 → −1.200 zinciri hesap ekranında doğrulanmıştı.

## 1.5 · Kayıt yanlışsa: ürün önce mi durduruyor, sonra mı geri alıyor

Hatalı kayıt iki ayrı yerde yakalanabilir — kaydetmeden önce formda, veya kaydettikten sonra geri almada. Beş ürün bu ikisini farklı dağıtmış.

**Şekil 1.13 · Sıfır tutar reddediliyor** (E0281)

- 0 TRY'de "Please fill in the amount."
- Uyarı alan dışında, snackbar olarak.

**Şekil 1.14 · Hesap boşken toast** (E0232)

- Hesap seçilmeden kaydetme durduruluyor.
- Uyarı yine alan dışında.

**Şekil 1.15 · Sıfır tutar kabul edildi** (E0434)

- Tutarı boş kayıt ₺0,00 olarak listeye düştü.
- Uyarı yok; ayın toplamı değişmedi.

**Şekil 1.16 · Sıfır tutar kabul edildi** (E0163)

- Boş tutar sessizce reddediliyor…
- …ama 0 kabul ediliyor ve başlıksız satır düşüyor.

**Şekil 1.17 · Silinmiş işlemler** (E0167)

- Silinen kayıt ayrı bir listede duruyor.
- Bağlam menüsü: Geri Yükle / Silme.

- Geri alma yalnız iki üründe var: Hesap Defterim'in Silinmiş işlemler listesi ve Bluecoins'in çöp kutusu. Wallet, Money Manager ve Goodbudget'ta silme bir onay diyaloğundan geçiyor; menüleri ve ayarları tarandı, kurtarma yüzeyi bulunamadı.
- Money Manager ve Wallet'ta hata mesajı formun alanının yanında değil, ekranın altında beliriyor. Ama sıfır tutarda ikisi ayrılıyor: Wallet kaydı durduruyor, Money Manager tutarı boş kaydı ₺0 olarak kabul ediyor. Hesap Defterim de sıfırı kabul ediyor ve hiç mesaj vermiyor.
- Bluecoins'te sıfır tutarlı kayıt ve silme onayı aynı karede; kaydetme sırasında hiç uyarı çıkmaması koşum notudur.
- Goodbudget tek onaylı silme kullanıyor.
- Hesap Defterim'de kalıcı silme ikinci bir onay istiyor.

## 1.6 · Aynı soru ön muhasebe tarafında: kayıt ile ödeme ayrı iki adım

KolayBi'nin destek görselleri ürünün gerçek ekranlarıdır; davranışı ölçülmedi, yalnız yüzeyi okunabiliyor. Gider formunun alanları Belge 1'de tek tek gösterilmişti ve oradaki not şunu devretmişti: Ödendi seçiminin kasaya ve rapora etkisi görülmedi. Soldaki kare o kaydın sonrasını, sağdaki ise kaydın kategorisinin nereden geldiğini gösteriyor.

**Şekil 1.18 · Gider detayı** (E0199)

- Kayıt oluştuktan sonra Ödeme Ekle ayrı bir eylem.
- Durum rozetleri: Bedelsiz · Yeni · Tahsilata Kapalı.
- Tekrarlı Genel Gidere Dönüştür aynı menüde.

**Şekil 1.19 · Gider tipleri** (E0193)

- Kategori kartları: Ulaşım/Konaklama, Temel Giderler, Vergi, Diğer.
- Her kartın altında tip satırları — kategori iki katmanlı.
- Kullanıcı hem yeni tip hem yeni kategori ekleyebiliyor.

- Canlı beş üründe kayıt ile ödeme tek adımdır: kaydettiğiniz anda bakiye değişir. Burada iki ayrı adım var ve ikisi arasında kayıt bir durum rozetiyle bekliyor. Bunun para akışında ne anlama geldiği Bölüm 9'da kurulur.
- Kategori burada iki katmanlı: üstte kategori kartı, altında tip satırları. Canlı beşte kategori düz bir listedir — Money Manager'ın Ağustos pastasındaki tek "Diğer" başlığının altında iki ayrı gider birleşiyor.

**Aynı soruda kaynaktan okunan üç ürün**

- *Kaynak beyanı* · **Paraşüt** — Kaynak beş gider kaydı türü sayıyor ve kaydın ödemeden ayrı olduğunu yazıyor: önce gider oluşturulur, sonra ödeme eklenir; kasa/banka ancak ödeme adımında azalır. Kısmi ödeme destekleniyor. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **Logo İşbaşı** — Kaynak gelir ve giderin fatura ve fişlerle kaydedildiğini, tek kaydın cari, kasa-banka ve stok defterlerini birlikte güncellediğini yazıyor. Bölüm 9'da kurulur.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Kaynağa göre kayıt elle değil, bağlı banka ve kart hesaplarından otomatik iniyor; kullanıcının işi kaydı oluşturmak değil, inen kaydı sınıflamak. Bölüm 6'da kurulur.

## 1.7 · Kaydın yolu ve ayrıldığı noktalar

Beş üründe kayıt aynı yolda başlıyor. Yol dört adımda ilerliyor ve ilk adımdan sonra üç kez çatallanıyor. Aşağıdaki her dal, önceki sayfalarda kareyle gösterilen davranışın yoldaki yeridir.

**1. Kayıt giriliyor**

Tutar, tarih ve tür beş üründe de soruluyor. Kategori üçünde seçiciden, birinde serbest metinden geliyor, birinde yerini zarf alıyor. Yol buraya kadar ortak.


**2. Kayıt kaç deftere yazılıyor**

- *Money Manager · Bluecoins · Wallet · Hesap Defterim* — Tek defter. Kayıt hesabın defterine düşüyor, bakiye aynı anda değişiyor. (E0235)
**→** *Goodbudget* — İki defter: zarf ve hesap. Zarfı dolduran kayıt hesap adını taşısa bile hesap defterine girmiyor; aradaki fark her doldurmada büyüyor. (E0421)

**3. Ayın toplamına ne giriyor**

- *Money Manager · Bluecoins · Wallet* — Yalnız gelir ve gider. Aktarım ile kart ödemesinin iki bacağı netleşiyor: Ağustos gideri 2.050. (E0231)
**→** *Hesap Defterim* — Deftere giren her satır. Açılış bakiyesi ve aktarım bacakları da toplama giriyor: aynı ayın gideri 6.250. (E0142)
**→** *Goodbudget* — Zarfa giren para gelir sayılıyor; hesap bakiyesi bu toplamın dışında kalıyor. (E0423)

**4. Kayıt yanlışsa ne oluyor**

- *Bluecoins · Hesap Defterim* — Silinen kayıt ikinci bir listede bekliyor ve geri yüklenebiliyor. (E0167)
- *Wallet* — Form sıfır tutarı durduruyor; silinen kaydın kurtarma yüzeyi yok. (E0281 · E0452)
- *Money Manager* — Boş hesabı durduruyor ama sıfır tutarı kabul ediyor; kurtarma yüzeyi yok. (E0434 · E0415)
- *Goodbudget* — Tek onayla siliniyor; ayarlarda kurtarma yüzeyi yok. (E0125 · E0453)

- Üç çatallanmanın üçü de aynı yerde toplanıyor: ürünün kaydı kaç deftere yazdığı, sonraki iki adımı da belirliyor. Tek defter tutan dört üründen üçü ayın toplamında da buluşuyor; ayrışan ikisi kendi defter tanımını toplamlara taşıyor.
- Bu sayfa yeni bir kanıt getirmez; 1.1–1.5'te kareyle gösterilen davranışları sürecin üzerine yerleştirir.

## 1.8 · Neden ayrışıyorlar

Beş üründe form neredeyse aynı şeyi soruyor. Ayrışma formda değil, ürünün kaydı kaç deftere yazdığında ve o defterlerin toplamına ne ad verdiğinde.

### Toplamın adı aynı, tanımı değil *(Çıkarım)*

Hesap Defterim'in Alındı/Ödendi'si gelir ve gider değil, deftere giren ve çıkan her satırdır. Açılış bakiyesi, aktarımın iki bacağı ve kart ödemesi de bu toplamlara giriyor. Bu yüzden Ağustos'un gideri 6.250 okunuyor, oysa gerçek gider 2.050.

Aradaki fark tesadüfi değil: 6.250 − 2.050 = 4.200, ve bu tam olarak iki çıkış bacağıdır — 3.000 aktarım + 1.200 kart ödemesi. Aynı iki hareketin giriş bacakları da 51.200'ün içinde duruyor. Ürün yanlış toplamıyor — farklı bir şey topluyor ve ona aynı adı veriyor.

Netin kendisi doğru kalıyor: 51.200 − 6.250 = 44.950, ve bu sayı Money Manager ile Bluecoins'in net değerine birebir eşit. Kullanılamaz hâle gelen şey net değil, netin iki bileşeni.

*Dayanağı: E0142, E0140, E0235, E0032*

### Goodbudget'ta zarf ve hesap iki ayrı defter *(Çıkarım)*

Zarf katmanı ile hesap katmanı arasındaki fark, hesaba yazılmayan kayıtların toplamına eşit. Gelir kaydından önce 2.050'ydi ve karşılığı 09/11 tarihli Initial Envelope Fill +2.050 satırıydı; yeni kayıttan sonra tam 1.234 büyüyerek 3.284 oldu.

Sıradan gider kayıtları hesaba işliyor — açılıştan başlayan zincir hesap ekranında doğrulanıyor. Ayrışan şey kayıt türü: zarf dolduran kayıt, formda hesap adını taşısa bile hesap defterine girmiyor.

Bunun nedeni koşumdan çıkmıyor ve ürün kusuru olarak yazılmıyor; ölçülen şey sonucun kendisi.

*Dayanağı: E0417, E0418, E0420, E0421, E0422, E0130*

### Kaydın kimliği kategoriden değil, kullanıcının yazdığından geliyor *(Çıkarım)*

Seçicisi olan dört üründe kategori — Goodbudget'ta zarf — paylaşılan bir kovadır: Money Manager'ın Ağustos pastasında Mavi Yazılım gideri yalnız "Diğer %58,5" olarak okunuyor; aynı "Diğer" adını Ada Reklam geliri de taşıyor. Kaydı ayırt eden şey Not, Payee veya Received from alanına yazılan metin.

Hesap Defterim bu ayrımı tersinden kuruyor: kimlik Notlar alanında, kategori ise seçici değil, adı "Açıklama / Kategori" olan ikinci bir serbest metin kutusu. Açıklama ile kovayı tek kutuda birleştirmenin sonucu, kovanın kaybolması: aynı harcama her seferinde başka yazılabiliyor ve okuma yüzeyinde kategori kırılımı hiç yok.

*Dayanağı: E0231, E0138, E0142, E0151*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Money Manager** | Gelir/gider toplamı ile net varlık iki ayrı ekranda ve ikisi de tutarlı; aktarım toplamlara karışmıyor | Kategori kırılımı tek kovada toplanınca aynı kategorideki iki farklı gider ayırt edilemiyor |
| **Bluecoins** | Aktarımın iki bacağını aynı günde netleyip gün netini 0 gösteriyor; dönem raporu gerçek gideri veriyor | Form yoğun: tek ekranda kategori, hesap, durum, etiket, bölme ve planlama birlikte soruluyor |
| **Wallet** | Hızlı form tutar ve hesapla kaydı bitiriyor, ayrıntı sonraya kalıyor; Cash-flow sorusu kullanıcının dilinde | Tür bir sekme olduğu için seçili sekme zeminle birleşiyor ve yanlış tür seçmek kolay |
| **Hesap Defterim** | Defter mantığı tek sayıda doğru neti veriyor ve satır başına yürüyen denge gösteriyor; düğme adları kullanıcının diline çevrilebiliyor | Ayın gelir ve gider toplamı kullanılamıyor: açılış ve aktarım bacakları aynı toplamın içinde |
| **Goodbudget** | Para amaca bağlanıyor; kullanıcı "ne kadar harcayabilirim" sorusunu zarftan doğrudan okuyor | "Param nerede" sorusunun iki cevabı var ve ikisi tutmuyor; fark her zarf doldurmada büyüyor |

**Belge 3'e taşınan soru**

- Bir ayın "gelir" ve "gider" toplamı hangi kayıtları içermeli? Hesap Defterim'in örneği, açılış bakiyesi ve aktarım bacaklarının toplama girmesinin neti bozmadığını ama iki bileşeni kullanılamaz kıldığını gösteriyor.
- Bütçe ilerlemesi ile hesap bakiyesi aynı kaynaktan mı beslenmeli? Goodbudget bu ikisini ayırdığında kullanıcıya iki farklı "toplam paran" sayısı kalıyor.
- Kaydın adı nereden gelmeli — kategoriden mi, kullanıcının yazdığı metinden mi? Seçicisi olan dört üründe kategori kovadır, kimlik değildir.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 1.1 | E0229 | Money Manager | Canlı kare | İşlem formu |
| 1.2 | E0020 | Bluecoins | Canlı kare | İşlem formu, GELİR seçili |
| 1.3 | E0277 | Wallet | Canlı kare | Hızlı form |
| 1.4 | E0138 | Hesap Defterim | Canlı kare | Alındı formu |
| 1.5 | E0419 | Goodbudget | Canlı kare | From New Income, dolu |
| 1.6 | E0231 | Money Manager | Canlı kare | İstatistik · Ağustos |
| 1.7 | E0023 | Bluecoins | Canlı kare | Net Kazançlar · Ağustos |
| 1.8 | E0142 | Hesap Defterim | Canlı kare | Bütün Hesaplar · Aylık |
| 1.9 | E0417 | Goodbudget | Canlı kare | Total 43.784,00 |
| 1.10 | E0420 | Goodbudget | Canlı kare | Total 45.018,00 |
| 1.11 | E0418 | Goodbudget | Canlı kare | All Accounts 41.734,00 |
| 1.12 | E0421 | Goodbudget | Canlı kare | All Accounts 41.734,00 |
| 1.13 | E0281 | Wallet | Canlı kare | Sıfır tutar reddediliyor |
| 1.14 | E0232 | Money Manager | Canlı kare | Hesap boşken toast |
| 1.15 | E0434 | Money Manager | Canlı kare | Sıfır tutar kabul edildi |
| 1.16 | E0163 | Hesap Defterim | Canlı kare | Sıfır tutar kabul edildi |
| 1.17 | E0167 | Hesap Defterim | Canlı kare | Silinmiş işlemler |
| 1.18 | E0199 | KolayBi | Kaynak görseli | Gider detayı |
| 1.19 | E0193 | KolayBi | Kaynak görseli | Gider tipleri |


---

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


---

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


---

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


---

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


---

# Bölüm 6 · Sınıflandırma: işletme mi, şahsi mi

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Bir kaydın işletmeye mi şahsi hayata mı ait olduğu nerede yazıyor?

Bu bölüm bir yokluğun bölümü. Canlı koşulan beş üründe kayıt düzeyinde işletme/şahsi ayrımı yapan bir alan arandı ve hiçbirinde bulunamadı. Bulunan şey, aynı işi dolaylı yoldan yapmaya çalışan dört ayrı araç.

Yokluğun kendisi de bir bulgu, ama tek başına az şey söyler. Bu yüzden bölüm iki yöne gidiyor: canlı ürünlerde bu işe en yakın duran araçlar, ve kaynaktan okunan iki ayrı çözüm — biri kullanıcının tanımladığı bir eksen, öteki kayıt başına iki değerli bir alan.

**Bu bölüme girmez**

- Kategorinin rapordaki kırılımı → Bölüm 7
- Zarf modelinin para üzerindeki etkisi → Bölüm 1
- Kategori ekranlarının görsel düzeni → Belge 1 §6

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare | Form ve kategori paneli kareli; kapsam alanı bulunamadı. |
| Bluecoins | Canlı kare | Form alanları ve kategori ağacı kareli. |
| Wallet | Canlı kare | Kayıt ayrıntısı ve Labels katmanı kareli. |
| Hesap Defterim | Canlı kare | Serbest metin kategorisi ve yeniden adlandırma kareli. |
| Goodbudget | Canlı kare | Zarf amaç ekseni olarak kareli. |
| KolayBi | Kaynak görseli | Proje ekseni destek görselinde. |
| QuickBooks Solopreneur | Kaynak beyanı | Kayıt başına Business/Personal alanı; yalnız kaynak metni. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |


## 6.1 · Kayıt formunda böyle bir alan var mı

Dört üründe kayıt formunun ve kayıt ayrıntısının tamamı tarandı. Tutar, tarih, kategori, hesap, not, etiket, durum — hepsi var. İşletme ile şahsiyi ayıran bir alan yok.

**Şekil 6.1 · Form ve kategori paneli** (E0229)

- Tutar, Kategori, Hesap, Not.
- On bir kategori: Yiyecek, Eğlence, Taşıma…
- Hepsi kişisel gider kategorisi.

**Şekil 6.2 · Formun bütün alanları** (E0020)

- Kategori, hesap, Planlı İşlemler, Bölmek.
- Durum ve Etiket ayrı alanlar.
- Kapsam alanı yok.

**Şekil 6.3 · Kayıt ayrıntısı** (E0294)

- Note, Labels, Payee, Date, Time.
- Payment Type, Warranty, Status, Place.
- Görünen bölümde kapsam alanı yok.

**Şekil 6.4 · Açıklama / Kategori** (E0151)

- Kategori bir seçici değil, serbest metin.
- Kullanıcı ne yazarsa o.
- İki değerli bir ayrım alanı yok.

- Goodbudget'ta da kayıt formunda böyle bir alan yok; kaydı sınıflayan tek şey hangi zarfa yazıldığı. Zarf ise amaç ekseni, kapsam ekseni değil.
- Bu bölümdeki yokluk ifadeleri incelenen sürümler ve taranan yüzeylerle sınırlı: kayıt formu, kayıt ayrıntısı, kategori yönetimi ve ayarlar. Ücretli paketlerde açılan yüzeyler bu taramaya girmedi.
- Money Manager'ın kategori paneli on bir kutunun hepsini kişisel gider başlığıyla dolduruyor; işletme tarafına ayrılmış bir grup yok.
- Hesap Defterim'de kategori alanı ayarlardan kapatılabiliyor.

## 6.2 · Aynı işe en yakın duran dört araç

Kapsam alanı olmayınca kullanıcı eldekiyle idare ediyor. Dört üründe dört ayrı araç bu işe en yakın duran şey — ama dördü de başka bir soru için yapılmış.

**Şekil 6.5 · Yalnız etiketli para** (E0425)

- Rapor ekranında Categories / Labels geçişi.
- Labels seçili: bu ay ₺150.
- Etiketsiz kayıtlar bu görünümde yok.

**Şekil 6.6 · Etiket kendi listesinde** (E0088)

- Etiketlerin kendi yönetim ekranı var.
- Arama ve silme eylemleri burada.
- Listede İş ve Kişisel de var.

**Şekil 6.7 · İki katmanlı kategori** (E0087)

- Üst başlıklar: Araba, Eve Ait, Eğlence.
- Altlarında alt kategoriler.
- Üst katman kapsam gibi kullanılabilir.

**Şekil 6.8 · Amaç ekseni** (E0417)

- Para amacına göre bölünmüş.
- Zarf adı kaydın ne için olduğunu söylüyor.
- Kimin parası olduğunu söylemiyor.

**Şekil 6.9 · Sütun adları değişti** (E0161)

- Alındı/Ödendi yerine Tahsilat/FaturaOdemesi.
- Bütün ekran başlıkları birlikte değişti.
- Ayrım değil, dil değişikliği.

- Dördü de kısmi çözüm. Etiket serbest ve çok değerli — iki değerli bir ayrım için kullanıldığında kullanıcının disiplinine bağlı. Kategori ağacının üst katmanı kapsam gibi kullanılabilir ama o zaman kategori kırılımı kaybediliyor.
- Disiplinin bedeli Wallet'ta ölçüldü: koşumda iki kayda Isletme ve Sahsi etiketi verildi. Labels görünümü yalnız bu ikisini, ₺150'yi saydı; aynı ayın ₺21.750'lik giderinin etiketlenmemiş kısmı bu görünümde hiç yok.
- Hesap Defterim'inki bir sınıflandırma aracı değil: düğme ve sütun adlarının kullanıcının diline çevrilmesi. Ayrımı değil, ayrımın adını değiştiriyor.
- Wallet'ın ayarlarında bir kural motoru var: kayıtlara kategori ve etiket atayan Automatic rules. Kapsam için kullanılıp kullanılamayacağı denenmedi.
- Bir başka dolaylı yol hesabı ayırmak: işletme parası için ayrı hesap açıp kapsamı hesap düzeyine taşımak. Bu koşumda denenmedi.
- Bluecoins'in etiket listesi, araştırmada hiç etiket eklenmemişken de İş ve Kişisel değerlerini taşıyordu. Etiket çoklu seçiliyor, filtre panelinde ayrı bir boyut ve bölünmüş kaydın her parçası kendi etiketini alabiliyor.

## 6.3 · Kaynakta birinci çözüm: kullanıcının tanımladığı eksen

KolayBi kapsamı sabit bir alan yapmıyor — kullanıcının açtığı bir proje ekseni sunuyor. Her belge bir projeye bağlanıyor ve proje kendi gelir, gider ve net toplamını taşıyor. Alanların varlığı kanıtlı; davranış görülmedi.

**Şekil 6.10 · KolayBi · Proje listesi** (E0188)

- Kolonlar: kod, ad, etiket, para birimi, durum, tarih.
- Ve üç toplam: gelir, gider, net.
- Proje adları kullanıcının koyduğu adlar.

**Şekil 6.11 · KolayBi · Proje özeti** (E0190)

- Kâr/Zarar ve Nakit Durumu iki ayrı sekme.
- Toplam, tahsil edilen, ödenen, bekleyen kırılımı.
- Aktif/Pasif menüsü ve ayrıca silme var.

- Eksen kullanıcı tanımlı olduğu için "işletme" ve "şahsi" iki proje olarak açılabilir — ama ürün bunu önermiyor; proje bir iş kalemi olarak kurgulanmış ve listedeki adlar da öyle.
- Kullanıcı tanımlı eksenin bedeli: iki proje mi, yirmi proje mi olacağı kullanıcıya kalıyor ve raporun anlamı kullanıcının disiplinine bağlanıyor.
- Projenin belge kırılımı satış, alış, iade, para girişi ve genel gider kartlarına ayrılmış.
- Yeni proje formunda kod, ad ve para birimi zorunlu; başlangıç-bitiş tarihi var.

## 6.4 · Kaynakta ikinci çözüm: kayıt başına iki değerli bir alan

QuickBooks Solopreneur, incelenen dokuz ürün içinde kayıt düzeyinde işletme/şahsi ayrımı yapan tek ürün. Ürünün hiçbir iç ekranı görülmedi; aşağıdakilerin tamamı yardım merkezi metninden okunmuştur ve ölçülmüş davranış değildir.

### Alan: Type *(Kaynak beyanı)*

İşlem başına tek bir alan: Type sütunu Business veya Personal değerini alıyor. Üçüncü bir değer yok.

İşlem listesi bu başlıktan süzülebiliyor; şahsi işaretlenen kayıt işletme raporuna girmiyor ama silinmiyor.

### Kısmen işletme: Split *(Kaynak beyanı)*

Bir gider kısmen işletme kısmen şahsiyse kayıt bölünüyor: Edit → Split transaction → tutar parçalara ayrılıyor ve her parça ayrı ayrı işaretleniyor.

Parçaların toplamı özgün tutara eşitleniyor; yani kapsam kayıt düzeyinde değil, kalem düzeyinde tanımlanabiliyor.

Kaynak bir istisna da yazıyor: araç ve yakıt gideri bölünmüyor, tamamı işletme işaretleniyor ve oranı muhasebeci hesaplıyor.

### Silme yerine hariç tutma *(Kaynak beyanı)*

Yinelenen veya ilgisiz kayıt silinmiyor, Exclude ile hariç tutuluyor. Kayıt veride kalıyor, toplamlardan çıkıyor.

Bu, şahsi işaretlemenin de mantığı: kayıt duruyor, yalnız hangi toplama gireceği değişiyor.

### Sınıflamayı kim yapıyor *(Kaynak beyanı)*

Kayıtlar bağlı banka ve kart hesaplarından otomatik iniyor; ürün geçmişe ve başka kullanıcıların davranışına bakarak kategori ve tür öneriyor.

Kullanıcının işi kaydı oluşturmak değil, inen kaydı gözden geçirip düzeltmek.

Tekrar eden düzeltmeler bir kural motoruna dönüşüyor: en çok otuz kural, benzer işlemleri otomatik sınıflıyor.

### Kategoriler nereye bağlı *(Kaynak beyanı)*

Kategoriler ülkeye özgü bir vergi formunun kalemleriyle hizalı; kategorize edilen her işlem o formda bir satıra eşleniyor.

Yani sınıflandırmanın amacı raporlama değil, yıl sonunda üretilecek belge. Ayrımın keskinliği ve üçüncü değerin olmaması buradan geliyor.

### Bu sayfanın sınırı *(Kaynak beyanı)*

Ürünün iç arayüzü hiç görülmedi: elde yalnız açılış ekranları ve ödeme duvarı var. Yukarıdaki her cümle ürünün kendi anlatımıdır.

Alanın gerçekte nasıl davrandığı, şahsi kaydın hangi toplamlardan çıktığı ve bölmenin sonucunun raporda nasıl göründüğü ölçülmedi.

- Bu sayfa bölümün tek karesiz sayfası ve bilinçli olarak öyle: basılacak bir ekran yok. Kaynak metninin kendisi bulgu, çünkü canlı beş üründe karşılığı hiç çıkmadı.

## 6.5 · Dokuz üründe kapsam ayrımı

Aynı soru dokuz ürüne soruldu: bir kaydın işletmeye mi şahsi hayata mı ait olduğu nerede yazıyor?

| Ürün | Kapsam alanı | En yakın araç | Ayrımın sonucu |
|---|---|---|---|
| Money Manager | *Canlı kare* · Yok | *Canlı kare* · Kategori; on bir kutunun hepsi kişisel | *Canlı kare* · Kapsam raporda hiç görünmüyor |
| Bluecoins | *Canlı kare* · Yok | *Canlı kare* · Etiket (listede İş ve Kişisel) ve iki katmanlı kategori ağacı | *Çıkarım* · Üst kategori kapsam gibi kullanılırsa kırılım kayboluyor |
| Wallet | *Canlı kare* · Yok | *Canlı kare* · Labels — kategoriden bağımsız ikinci eksen | *Canlı kare* · Rapor Categories/Labels olarak ayrılıyor; Labels yalnız etiketli parayı sayıyor |
| Hesap Defterim | *Canlı kare* · Yok | *Canlı kare* · Serbest metin kategori; düğme adlarını değiştirme | *Canlı kare* · Raporda kategori kırılımı da yok |
| Goodbudget | *Canlı kare* · Yok | *Canlı kare* · Zarf — amaç ekseni | *Çıkarım* · Zarf ne için olduğunu söylüyor, kimin parası olduğunu değil |
| KolayBi | *Kaynak görseli* · Proje ekseni | *Kaynak görseli* · Kullanıcının tanımladığı proje; gelir/gider/net proje başına | *Çıkarım* · Eksenin anlamı kullanıcının disiplinine bağlı |
| QuickBooks Solopreneur | *Kaynak beyanı* · Var — Type: Business / Personal | *Kaynak beyanı* · Kayıt başına iki değerli alan; kısmen işletme için Split | *Kaynak beyanı* · Şahsi kayıt silinmiyor, toplamdan çıkıyor |
| Paraşüt | *Kaynak beyanı* · Yok | *Kaynak beyanı* · Ortak/personel carisi üzerinden dolaylı | *Çıkarım* · Şahsi harcama bir alacak-borç kalemine dönüşüyor |
| Logo İşbaşı | *Kaynak beyanı* · Yok | *Kaynak beyanı* · Ortak carisi veya çekilen para | *Çıkarım* · Klasik firma defteri; şahsi taraf ürünün dışında |

- Dokuz üründe kayıt düzeyinde kapsam alanı yalnız birinde var ve o ürünün hiçbir iç ekranı görülmedi. Canlı koşulan beş üründe sıfır.
- Üç Türk ön muhasebe ürününde de alan yok; ikisinde şahsi harcamanın dolaylı yolu ortak veya personel carisinden geçiyor — yani şahsi harcama bir borç kalemine dönüşüyor.

## 6.6 · Kapsam sorusunun yolu ve ayrıldığı noktalar

Aynı soru dokuz üründe üç ayrı cevapla karşılaşıyor: alan yok, eksen kullanıcıya bırakılmış, ya da alan sabit ve iki değerli.

**1. Kayıt giriliyor**

Dokuz üründe de kayıt tutar, tarih ve bir sınıflandırma taşıyor. Ayrışma o sınıflandırmanın hangi soruyu cevapladığında.


**2. Kapsam nerede yazılıyor**

**→** *Canlı beş ürün* — Hiçbir yerde. Kayıt formunda, kayıt ayrıntısında ve kategori yönetiminde böyle bir alan bulunamadı. (E0294)
- *KolayBi* — Kullanıcının açtığı bir proje ekseninde. Eksen sabit değil, adları kullanıcı koyuyor. (E0188)
- *QuickBooks Solopreneur* — Kayıt başına sabit bir alanda: Business ya da Personal. Üçüncü değer yok. (Intuit yardım merkezi)

**3. Kısmen işletme olan gider**

- *Wallet* — Kaydı bölmek mümkün (Split record) ama parçalara kapsam verilemiyor; bölme kategori içindir. (E0311)
- *Bluecoins* — Her parça kendi etiketini taşıyabiliyor; İş ve Kişisel etiketleriyle gider parça parça işaretlenebilir. Raporun parçaları etikete göre ayırdığı ölçülmedi. (E0443 · E0088)
**→** *QuickBooks Solopreneur* — Kayıt tutara göre bölünüyor ve her parça ayrı işaretleniyor; kapsam kalem düzeyine iniyor. (Intuit yardım merkezi)

**4. Raporda ne oluyor**

- *Canlı beş ürün* — Rapor kapsamdan habersiz. Ayrım ancak etiket veya kategori disipliniyle taklit edilebiliyor; Wallet'ta etiketlenmeyen kayıt etiket raporundan tamamen düşüyor. (E0278 · E0425)
- *KolayBi* — Her proje kendi gelir, gider ve net toplamını taşıyor. (E0188)
- *QuickBooks Solopreneur* — Şahsi işaretlenen kayıt işletme raporundan çıkıyor ama veride kalıyor — silinmiyor, hariç tutuluyor. (Intuit yardım merkezi)

- Yolun ilk adımı ortak, ikincisinden sonrası tamamen ayrışıyor. Alanı olmayan beş üründe sonraki iki adım da boş kalıyor — kavram olmayınca ne bölme ne raporlama sorusu oluşuyor.
- Bu akışın iki dalı kaynak metnine dayanıyor ve ölçülmüş davranış değil.

## 6.7 · Neden ayrışıyorlar

Bu bölümde ayrışmanın kaynağı ürünlerin kime hizmet ettiği. Kişisel finans defterinde kapsam sorusu hiç oluşmuyor; ön muhasebede şahsi taraf ürünün dışında; vergi ekseninde ise ayrım ürünün asıl işi.

### Kapsam alanının yokluğu ürünlerin hedef kitlesinden geliyor *(Çıkarım)*

Canlı beş ürün kişisel finans uygulaması ve kayıt düzeyinde tek bir cep varsayıyor. Money Manager'ın on bir kategorisinin hepsi kişisel gider başlığı taşıyor — işletme tarafına ayrılmış tek bir grup bile yok. Bluecoins'in etiket listesinde İş ve Kişisel'in bulunması, ayrımın akla geldiğini ama bir alana dönüşmediğini gösteriyor.

Üç Türk ön muhasebe ürününde de alan yok, ama nedeni tersi: onlar firma defteri tutuyor ve şahsi harcama zaten ürünün konusu değil. Kaynakların gösterdiği dolaylı yol, şahsi harcamayı ortak veya personel carisine yazmak — yani onu bir borç kalemine çevirmek.

İki uçta da kapsam sorulmuyor, ama iki ayrı nedenle: birinde ikinci cep yok sayılıyor, ötekinde ikinci cep başka bir kavrama dönüştürülüyor.

*Dayanağı: E0229, E0020, E0294, E0151, parasut.com kılavuzu, isbasi.com*

### Yerine kullanılan araçların hepsi başka bir soru için yapılmış *(Çıkarım)*

Etiket çok değerli ve serbest; iki değerli bir ayrım için kullanıldığında sonucun doğruluğu tamamen kullanıcının disiplinine kalıyor. Wallet'ta bunun sonucu ölçüldü: etiket raporu yalnız etiketli ₺150'yi saydı, etiketlenmemiş her kayıt o rapordan düştü. Kategori ağacının üst katmanı kapsam gibi kullanılabilir, ama o zaman kategorinin asıl işi — harcamayı türüne göre kırmak — kayboluyor.

Zarf amacı söylüyor, sahibi değil: "Market" zarfı hem işletmenin hem evin marketi olabilir. Hesap Defterim'in sunduğu şey ise bir ayrım değil, ayrımın adını değiştirme imkânı.

Ortak nokta şu: kategori, zarf ve adlandırma tek eksenli. Kapsam ikinci bir eksen istiyor ve canlı ürünlerde bu ekseni taşıyan tek araç etiket — Wallet'ta raporun kendi görünümü, Bluecoins'te bir filtre boyutu ve etiket listesinde İş ile Kişisel.

*Dayanağı: E0278, E0425, E0087, E0088, E0417, E0161*

### Sabit alan, ayrımın amacından doğuyor *(Çıkarım)*

QuickBooks'un alanı iki değerli ve üçüncü değeri yok. Kaynağa göre bunun nedeni kategorilerin bir vergi formunun kalemleriyle hizalı olması: kategorize edilen her işlem o formda bir satıra eşleniyor.

Ayrımın amacı raporlama değil, yıl sonunda üretilecek bir belge. Belge "kısmen" kabul etmediği için alan da kabul etmiyor — kısmi durum kaydı bölerek çözülüyor, alana üçüncü bir değer eklenerek değil.

KolayBi'nin çözümü bunun tersi: eksen sabit değil, kullanıcı tanımlı. Esneklik kazanılıyor, ama raporun anlamı kullanıcının kaç proje açtığına ve neyi hangi projeye yazdığına bağlanıyor.

*Dayanağı: Intuit yardım merkezi, E0188, E0190*

### Şahsi kayıt silinmiyor, hariç tutuluyor *(Çıkarım)*

Kaynağa göre şahsi işaretlenen kayıt işletme raporuna girmiyor ama veride kalıyor. Aynı mantık ilgisiz kayıtlar için de geçerli: silme değil, hariç tutma.

Bu, kapsamı bir filtre olarak kurmak demek — kayıt tek, toplam çok. Kaydın kendisiyle o kaydın hangi toplama gireceği birbirinden ayrılıyor.

Canlı ürünlerde bu ayrımın karşılığı hesap düzeyinde duruyor: Money Manager'ın Toplama Dahil Et ve Wallet'ın Exclude from stats anahtarı bütün bir hesabı toplamdan çıkarıyor, hesabı silmeden. Kayıt düzeyinde bir karşılığı görülmedi.

*Dayanağı: Intuit yardım merkezi, E0252, E0253, E0437*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Wallet** | Labels kategoriden bağımsız ikinci bir eksen ve rapor bu eksende de okunabiliyor | Etiket serbest ve çok değerli; etiketlenmeyen kayıt etiket raporundan tamamen düşüyor |
| **Bluecoins** | Kategori iki katmanlı; etiket listesinde İş ve Kişisel var ve bölünmüş kaydın her parçası kendi etiketini alabiliyor | Üst katman kapsama ayrılırsa kategorinin asıl kırılımı kayboluyor |
| **Goodbudget** | Zarf kaydın amacını net söylüyor ve para zaten o amaca ayrılmış durumda | Amaç ekseni sahibi söylemiyor; aynı zarf iki cebe de ait olabilir |
| **Money Manager · Hesap Defterim** | — | Kapsam için kullanılabilecek ikinci bir eksen yok; biri yalnız kategori, diğeri yalnız serbest metin sunuyor |
| **KolayBi** | Eksen kullanıcı tanımlı; her proje kendi gelir, gider ve net toplamını taşıyor | Esnekliğin bedeli belirsizlik: raporun anlamı kullanıcının eksen disiplinine bağlı |
| **QuickBooks Solopreneur** | Kayıt başına sabit, iki değerli bir alan; kısmen işletme olan gider bölünebiliyor ve şahsi kayıt silinmeden toplamdan çıkıyor | Ayrım ülkeye özgü bir vergi formuna bağlı; alan iki değerli olduğu için kısmi durum ancak kaydı bölerek çözülüyor |

**Belge 3'e taşınan soru**

- Kapsam kayıt düzeyinde sabit bir alan mı olmalı, yoksa kullanıcının tanımladığı bir eksen mi? İki kaynak iki ayrı cevap veriyor ve ikisinin bedeli farklı.
- Kısmen işletme olan bir gider ne olmalı — bölünmeli mi, tek kapsamda mı kalmalı? Kaynakta Split kapsamı kalem düzeyine indiriyor; canlı tarafta Bluecoins'in parça başına etiketi aynı işe yarayabilir ama sonucu ölçülmedi.
- Şahsi işaretlenen kayıt silinmeli mi, toplamdan mı çıkmalı? Kaynaktaki model kaydı tutup filtreliyor.
- İkinci eksen kategoriden ayrı mı olmalı? Canlı beş üründe kategoriden bağımsız ikinci eksen yalnız ikisinde var ve ikisi de etiket.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 6.1 | E0229 | Money Manager | Canlı kare | Form ve kategori paneli |
| 6.2 | E0020 | Bluecoins | Canlı kare | Formun bütün alanları |
| 6.3 | E0294 | Wallet | Canlı kare | Kayıt ayrıntısı |
| 6.4 | E0151 | Hesap Defterim | Canlı kare | Açıklama / Kategori |
| 6.5 | E0425 | Wallet | Canlı kare | Yalnız etiketli para |
| 6.6 | E0088 | Bluecoins | Canlı kare | Etiket kendi listesinde |
| 6.7 | E0087 | Bluecoins | Canlı kare | İki katmanlı kategori |
| 6.8 | E0417 | Goodbudget | Canlı kare | Amaç ekseni |
| 6.9 | E0161 | Hesap Defterim | Canlı kare | Sütun adları değişti |
| 6.10 | E0188 | KolayBi | Kaynak görseli | KolayBi · Proje listesi |
| 6.11 | E0190 | KolayBi | Kaynak görseli | KolayBi · Proje özeti |


---

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


---

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


---

# Bölüm 9 · Ön muhasebe programları: belge, vergi ve müşavir

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Kayıt bir satır değil bir belge olunca ürünün kapsamı nereye kadar genişliyor?

Önceki sekiz bölüm defter tutan beş ürünü izledi: kayıt bir satır ve yazıldığı anda bakiyeyi değiştiriyor. Bu bölüm üç ön muhasebe ürününe bakıyor: KolayBi, Paraşüt ve Logo İşbaşı.

Orada kaydın yerine bir belge geçiyor — ve bu tek bir fark değil, bir zincirin başı. Belge bir karşı tarafa kesiliyor, üstünde vergi taşıyor, içinde kalem taşıyor, ödemesi ayrı bir adım ve sonunda onu işleyen bir muhasebeci var. Bölüm bu zinciri kaynaktan kuruyor.

Bu bölümün kanıt tavanı diğerlerinden düşük ve bu bir kez burada söyleniyor: KolayBi'nin destek görselleri ürünün gerçek ekranları, ama davranışı ölçülmedi. Paraşüt ve Logo İşbaşı'nın iç arayüzü hiç görülmedi; aşağıdaki model onların kendi anlatımından kuruldu.

**Bu bölüme girmez**

- Canlı beş ürünün davranışı → Bölüm 1–8
- Kart dönemi ve taksit → Bölüm 3
- Kapsam ayrımı → Bölüm 6
- Vergi oranlarının doğruluğu ve hesabın denetimi → bu belge hüküm vermez

| Ürün | Kanıt | Not |
|---|---|---|
| KolayBi | Kaynak görseli | Destek görselleri gerçek ürün ekranları; davranış ölçülmedi. |
| Paraşüt | Kaynak beyanı | Kullanım kılavuzu ve ürün sayfası; iç arayüz görülmedi. |
| Logo İşbaşı | Kaynak beyanı | Ürün sayfası ve blog; iç arayüz görülmedi. |


## 9.1 · Üç üründe aynı iskelet: belge doğuyor, ödeme sonra geliyor

Üç kaynağın anlattığı yol neredeyse aynı. Ortak olan şey kaydın iki adıma bölünmesi: önce ekonomik olay bir belgeyle kayda geçiyor, sonra para ayrı bir adımda hareket ediyor.

### Paraşüt · satıştan tahsilata *(Kaynak beyanı)*

Müşteri seçiliyor, satış faturası kesiliyor ve e-belge olarak gönderiliyor. Fatura kesildiği anda cari borç artıyor; kasa ve banka kıpırdamıyor.

Vade geldiğinde otomatik hatırlatma e-postası gidiyor. Tahsilat ayrı bir adım: banka hesabı ve tutar giriliyor, cari bakiye kapanıyor ve banka artıyor.

Mahsuplaştırma otomatik: gelen para en çok gecikmiş açık faturadan başlayarak eşleşiyor.

### Paraşüt · gider tarafı *(Kaynak beyanı)*

Gider beş türe ayrılmış: detaylı fiş/fatura, hızlı fiş/fatura, maaş/prim, vergi/SGK primi, banka gideri.

Kayıt oluşturulduğunda gider raporu artıyor ve tedarikçi carisi borçlanıyor. Ödeme ayrı bir adım ve kasa/banka ancak o adımda azalıyor. Kısmi ödeme destekleniyor.

Cari borçlandırmanın üç yolu var: müşteri kaydında açılış bakiyesi, satış faturası, ve borç yokken ödeme eklemek — sonuncusu avans oluyor.

### Logo İşbaşı · tek kayıt, üç defter *(Kaynak beyanı)*

Fatura kesilirken müşteri seçiliyor ve bakiyesi anında görünüyor; ürün seçiliyor ve kayıt tamamlanıyor.

Kaynağın en ayırt edici cümlesi burada: bir işlem kaydedildiğinde cari hesap, kasa-banka bakiyeleri ve stok seviyeleri birlikte güncelleniyor. Üç defter tek kayıttan besleniyor.

Gider tarafı fişin fotoğrafıyla başlıyor: okuma adımından geçip gider kaydına dönüşüyor, tedarikçi carisi borçlanıyor, ödeme ayrı adımda kasayı azaltıyor.

### KolayBi · formda görünen ikilik *(Kaynak görseli)*

Bu üründe modelin izi doğrudan formda: gider kaydedilirken ödenip ödenmediği ayrıca soruluyor ve cari takibinin açık olup olmadığı ayrı bir seçim.

Kayıt oluştuktan sonra ödeme ayrı bir eylem olarak menüde duruyor. Kayıt ile ödeme arasındaki boşlukta kayıt bir durum rozetiyle bekliyor.

### Logo İşbaşı · sesle fatura *(Kaynak beyanı)*

Kaynak, faturanın butonlarla değil sesle oluşturulduğunu yazıyor ve iki mekanik ayrıntı veriyor: özellik telefonu sallayarak tetikleniyor ve sistemde kayıtlı hazır fatura şablonları üzerinden çalışıyor.

Şablona bağlanması sesin çözmesi gereken alan sayısını azaltıyor; "saniyeler içinde" vaadi bununla tutarlı. Şablonu olmayan bir satışta ne olduğu yazmıyor.

İncelenen öteki sekiz üründe benzeri bir giriş yolu görülmedi. Bir onay adımı ne duyuruda ne rehberde geçiyor; arandı, bulunamadı.

### Ortak sonuç *(Çıkarım)*

Üç kaynakta da aynı iskelet var: belge ekonomik olayı kayda geçiriyor, ödeme parayı taşıyor ve ikisi ayrı anlar.

Bu, canlı beş üründe hiç karşılaşılmayan bir ayrım. Orada kayıt ile para hareketi aynı an — Bölüm 1'de görüldüğü gibi kaydettiğiniz anda bakiye değişiyor.

### Bu bölümün sınırı *(Kaynak beyanı)*

Yukarıdaki akışların hiçbiri çalışırken görülmedi. KolayBi'de form alanları kareyle doğrulandı, ama seçimlerin kasaya ve rapora etkisi ölçülmedi.

Paraşüt'ün tanıtım videosundan alınan kareler ürünün kendi kurgusudur ve bu belgede basılmaz; yalnız anılır.

- Üç ürünün üçü de aynı ayrımı yapıyor ama farklı kelimelerle: biri ödeme durumu, biri ödeme ekleme adımı, biri cari borcun oluşması. Ortak olan, ekonomik olayın para hareketinden önce kaydedilmesi.

## 9.2 · Belgenin kendisi: formda ne var

Modelin izini formda görmek mümkün. Alış faturası formu, canlı beş ürünün işlem formunda hiç bulunmayan alanlar taşıyor: cari zorunlu, ödeme durumu ayrı bir seçim, ve bir seri numarası.

**Şekil 9.1 · KolayBi · Yeni Alış Faturası** (E0200)

- Cari zorunlu; belgenin bir karşı tarafı olmak zorunda.
- Düzenlenme tarihi ve saati ayrı alanlar.
- Seri No ve Ödeme Durumu formda.

**Şekil 9.2 · KolayBi · Genel Giderler listesi** (E0198)

- Üç sekme ve detaylı arama.
- Her satırda Cari Bilgisi kolonu.
- Carisi olmayan kayıtta "Genel Gider" yazıyor.

- Cari alanının zorunlu olması modelin merkezinde duruyor: belge her zaman birine kesiliyor. Canlı ürünlerde sıradan kayıt kimseye ait değil; yalnız bir hesaba ve bir sınıflandırmaya bağlı.
- Carisi olmayan gider için ürün ayrı bir yol bırakmış ve listede "Genel Gider" olarak işaretliyor — yani karşı tarafsız kayıt istisna, kural değil.
- Satış tarafında da aynı iskelet var; form e-arşiv fatura bilgileri bölümü taşıyor.
- Gider detayında ödeme ayrı bir eylem olarak menüde duruyor.

## 9.3 · Belgenin kendisi para taşıyınca: çek ve senet

Modelin en uç noktası burada. Çek ve senet, vadesi olan ve elden ele geçebilen belgeler — yani belgenin kendisi bir ödeme aracı. Canlı beş ürünün hiçbirinde bu kavram yok.

**Şekil 9.3 · KolayBi · Çekler** (E0209)

- Kolonlarda keşideci, hamil ve keşide (vade) tarihi.
- Durum kolonu çekin hangi aşamada olduğunu taşıyor.
- Toplu Çek Ekle, Bordrolar ve Dışarıya Aktar.

**Şekil 9.4 · KolayBi · Senetler** (E0210)

- Aynı kolon yapısı senet tarafında da var.
- Ciro edilmiş senet için ayrı bir not.
- Senet Gösterimi anahtarı toplam özeti açıyor.

- Keşideci ve hamil kolonları belgenin el değiştirdiğini varsayıyor: çeki yazan ile elinde tutan farklı kişiler olabiliyor. Ciro notu da bunun devamı.
- Vade tarihi burada kaydın değil, belgenin özelliği. Bölüm 4'te Wallet vadeyi borcun kendisine, KolayBi ise karşı tarafa bağlamıştı; burada üçüncü bir yer çıkıyor — belgenin üstü.
- Çek ve senet, finans yüzeyinin altı sekmesinden ikisi; banka hesabı ve kasayla aynı düzeyde duruyorlar.

## 9.4 · Vergi belgenin üstünde yaşıyor

Bölüm 1'de beş canlı ürünün işlem formu tek tek okundu; vergi diye bir alan hiçbirinde bulunmadı. İncelenen üç üründen ikisinde ise vergi kendi raporu olan bir konu ve raporun kaynağı ayrı bir kayıt değil, faturanın kendisi. Alanların varlığı kanıtlı; sayının nasıl üretildiği ölçülmedi.

**Şekil 9.5 · KolayBi · KDV Raporu** (E0214)

- Oran sütunları: %20, %18, %10, %8, %1, Diğer ve Toplam.
- Her oranın altında KDV Matrahı ile KDV Tutarı ayrı iki kolon.
- Üst blok Satış Faturası + Alış İade; son satır Hesaplanan KDV Toplam.

**Şekil 9.6 · KolayBi · Alış/Satış Raporu** (E0213)

- Sağ üstte KDV Dahil anahtarı.
- Satırlar belge türü: Satış Faturası 20.000, Alış Faturası −12.000, Toplam 8.000.
- Filtreler arasında Şube, Proje, Etiket ve ayrı bir vade aralığı var.

- Matrah ile tutarın ayrı kolonlar olması, ürünün ikisini ayrı bilgi saydığını gösteriyor. Karedeki tek dolu hücrede 12.000 matraha 2.400 tutar düşüyor — ama tutarın orandan mı hesaplandığı yoksa girilenin mi toplandığı ekrandan çıkmıyor.
- Cevap ürünün geliştirici dokümanında: fatura kaleminde KDV oranı zorunlu ve kalem düzeyinde bir tutar alanı yok; genel giderde ise KDV oran veya tutar olarak verilebiliyor. Yani tek bir kural yok, kayıt tipine göre değişiyor.
- KDV Dahil anahtarı aynı dönemi brüt ve net okutuyor: rapor tek bir doğru sayı dayatmıyor, hangisini istediğinizi soruyor.
- Oran listesi ürünün içinde yaşıyor: %20'den %1'e altı sütun sabit başlık olarak duruyor; izin verilen oranların listesi yayımlanmamış.
- Rapor sekmelerinden biri de Stok Raporu; vergi ve stok aynı yüzeyin iki sekmesi.

**Aynı soruda kaynaktan okunan iki ürün**

- *Kaynak beyanı* · **Paraşüt** — Kılavuzdaki ekran ay ay üç kolon veriyor: Hesaplanan KDV, İndirilecek KDV ve Net KDV. İndirilecek negatif işaretli ve Net KDV negatife düşebiliyor — devreden KDV ekranda gizlenmiyor. Aya tıklanınca satır dökümü açılıyor.
- *Kaynak beyanı* · **Paraşüt · giriş yolu** — Kalem bazında vergi oranı değiştiriliyor ve birim fiyat ile KDV dahil toplam birbirinden hesaplanabiliyor. Elle KDV tutarı girme yolu kaynaklarında geçmiyor: kullanıcı oranı seçiyor, tutarı ürün hesaplıyor.
- *Görülmedi* · **Logo İşbaşı** — Kaynakta vergi raporu anlatılmıyor; e-fatura ve e-arşiv tarafından söz ediliyor ama verginin nerede toplandığı yazılı değil.

## 9.5 · Belge kalem taşıyor: ürün, hizmet ve stok

Belge yalnız bir tutar değil, kalemler taşıyor. Kalem bir ürünse aynı kayıt stoğu da ilgilendiriyor — canlı beş üründe hiç karşılaşılmayan bir zincir.

### KolayBi · varyant, depo ve raf kodu *(Kaynak beyanı)*

Kaynak varyantı kendi cümlesiyle tanımlıyor: aynı ürünün farklı renk ve biçimleri; özellikle tekstilde renk ve beden takibi için.

Kurulum sırası yazılı: önce ayarlardan varyant kullanımı açılıyor, sonra varyant kategorileri ve değerleri tanımlanıyor, sonra ürün üç aşamada oluşturuluyor ve en fazla dört varyant değeri seçiliyor.

Hiç depo açılmasa da bir Ana Depo geliyor; yeni depo açılabiliyor ve depolar Excel ile aktarılabiliyor.

### Stok birimi ürün değil *(Çıkarım)*

Varyantın altında ayrıca "stok" kaydı açılıyor ve o kayıt bir depo ile bir raf kodu istiyor. Stok giriş-çıkışı ve depolar arası transfer varyant kartından yürüyor.

Yani sayılan şey ürün değil, varyant ile deponun çifti. Ürün hareketleri tablosunun ayrı bir Stok/Depo kolonu taşıması da bunu gösteriyor.

### Paraşüt · stok varsayılan açık *(Kaynak beyanı)*

Kaynağa göre stok takibi hesap ilk açıldığında zaten devrede ve abonelik dışında ayrıca ücretlendirilmiyor.

Varsayılan depo, giriş-çıkış deposu, depolar arası transfer ve depo bazında kritik stok seviyesi anlatılıyor; transferin kendi fişi çıkıyor.

### Logo İşbaşı · tek kayıt, üç defter *(Kaynak beyanı)*

Kaynağın kendi cümlesi: bir işlem kaydedildiğinde cari hesap, kasa-banka bakiyeleri ve stok seviyeleri birlikte güncelleniyor.

Ama stok burada tek düzlemde anlatılıyor: ürün kartı, stok hareketi ve bir stok miktarı kolonu. Çoklu depo anlatan bir sayfa arandı, bulunamadı — yokluğu söyleyen bir cümle de yok.

### Raporun içine giriyor *(Kaynak görseli)*

Alış/Satış raporunun alt sekmelerinden biri Ürün ve Hizmetler: aynı dönem ürün kırılımıyla da okunabiliyor. Ayrıca ayrı bir Stok Raporu duruyor.

Kaynağa göre stok raporu yalnız miktar vermiyor: giriş, çıkış, kalan, kritik miktar ve ortalama birim maliyetle birlikte kâr-zarar da taşıyor. Filtrede depo zorunlu görünüyor.

### Canlı beş üründe karşılığı *(Çıkarım)*

Beş üründe kaydın kalemi yok: kayıt bir tutar, bir hesap ve bir sınıflandırma taşıyor. Ne alındığı yalnız serbest metinde yaşıyor.

Bölüm 1'de görüldüğü gibi kaydın adı kullanıcının yazdığı nottan geliyor; ürün adı diye ayrı bir alan hiçbirinde bulunmadı.

### Kalemin bedeli *(Çıkarım)*

Kalem kavramı ürünün kapsamını genişletiyor: aynı kayıt hem parayı hem malı ilgilendiriyor ve "ne kadar kaldı" sorusu kendiliğinden cevaplanıyor.

Bedeli kurulumda görünüyor: varyant önce ayarlardan açılmak zorunda, en fazla dört varyant türü seçilebiliyor ve varyantlı ürün, birleştirme işleminin dışında kalıyor. Sıra yanlışsa geri dönüş maliyetli.

### Bu sayfanın sınırı *(Kaynak beyanı)*

Üç üründe de stok hareketinin kayıtla nasıl bağlandığı çalışırken görülmedi; yukarıdaki her cümle kaynağın kendi anlatımı.

KolayBi'nin depo ve varyant ekranları destek görselinde boş; yapının kendisi metinden kuruldu.

- Kalem, vergi ve cari aynı yerden çıkıyor: belgenin üstünden. Üçü ayrı özellik değil, aynı kurgunun üç sonucu.

## 9.6 · Bölüm 1–4'ün soruları, ön muhasebe tarafında

Önceki bölümlerde canlı ürünlere sorulan sorular, aynı biçimde bu üç ürüne soruldu. Cevaplar kaynak metinlerinden ve destek görsellerinden geliyor.

| Soru | KolayBi | Paraşüt | Logo İşbaşı |
|---|---|---|---|
| Kayıt ne zaman oluşuyor | *Kaynak görseli* · Gider veya fatura kaydedilince; ödeme durumu ayrıca soruluyor | *Kaynak beyanı* · Belge kesilince; ödeme sonra ekleniyor | *Kaynak beyanı* · Fatura veya fişle; fiş fotoğraftan okunuyor |
| Kaydedince hangi defter değişiyor | *Görülmedi* · Ölçülmedi; formda cari takibi ayrı bir seçim | *Kaynak beyanı* · Gider raporu ve tedarikçi carisi; kasa değişmiyor | *Kaynak beyanı* · Cari, kasa-banka ve stok birlikte |
| Para ne zaman hareket ediyor | *Kaynak görseli* · Ödeme Ekle ayrı bir eylem | *Kaynak beyanı* · Ödeme adımında; kısmi ödeme destekleniyor | *Kaynak beyanı* · Tahsilat veya ödeme adımında |
| Karşı taraf zorunlu mu | *Kaynak görseli* · Faturada evet; genel giderde hayır | *Kaynak beyanı* · Tedarikçi eşleştirmesi isteğe bağlı | *Kaynak beyanı* · Firma bilgisi tanımlamadan da fatura kesilebiliyor |
| Tahsilat açık faturaya nasıl bağlanıyor | *Görülmedi* · Ölçülmedi | *Kaynak beyanı* · Otomatik mahsup: en gecikmiş açık faturadan başlayarak | *Kaynak beyanı* · Cari hesap içinde toplanıyor; eşleme kuralı yazılmamış |
| Vergi nereye yazılıyor | *Kaynak görseli* · Oran başına matrah ve tutar; kendi raporu var | *Kaynak beyanı* · Ay bazında hesaplanan, indirilecek ve net | *Görülmedi* · Kaynakta vergi raporu anlatılmıyor |
| Kaydın kalemi var mı | *Kaynak görseli* · Ürün, hizmet, depo ve varyant ayrı bir yüzey | *Görülmedi* · Kaynakta stok anlatılmıyor | *Kaynak beyanı* · Stok, cari ve kasa tek kayıttan besleniyor |
| İşletme/şahsi ayrımı | *Kaynak görseli* · Proje ekseni; kapsam alanı yok | *Kaynak beyanı* · Yok; şahsi harcama ortak/personel carisinden geçiyor | *Kaynak beyanı* · Yok; klasik firma defteri |

- İkinci satır bu tablonun merkezinde: iki kaynakta da kayıt anında kasa kıpırdamıyor. Canlı beş üründe aynı soruya verilen cevap her zaman "bakiye değişiyor" idi.
- Son satır Bölüm 6'yı tamamlıyor: üç ön muhasebe ürününde de kapsam alanı yok ve şahsi tarafın yolu bir borç kaleminden geçiyor.
- Vergi ve kalem satırlarının canlı beş üründe karşılığı yok: Bölüm 1–8 boyunca ne bir vergi alanı ne de bir ürün adı alanı bulundu.

## 9.7 · Muhasebeci nerede duruyor

Üç kaynağın da ortak bir yüzeyi var ve canlı beş üründe karşılığı yok: muhasebeci. Üçü de onu ürünün içine alıyor, ama üç ayrı yetkiyle.

### Logo İşbaşı · müşavir portalı *(Kaynak beyanı)*

Müşavir, müşterisinin verisine erişiyor. Kaynağa göre müşteri onu mali müşavir olarak ekledikten sonra müşavir müşteri adına işlem yapabiliyor ve başka müşterilerini de ürüne davet edebiliyor.

Bu en geniş yetki: veriyi gören ile veriyi değiştirebilen ayrışıyor.

### Paraşüt · canlı erişim *(Kaynak beyanı)*

Kaynak muhasebeciye canlı erişim verildiğini yazıyor; ay sonunda dosya gönderme adımı ortadan kalkıyor.

Erişimin kapsamı — yalnız görme mi, işlem yapma mı — kaynakta ayrıntılı anlatılmıyor.

### KolayBi · davet *(Kaynak görseli)*

Panoda müşavir daveti girişi görünüyor; davetin sonrasında müşavirin ne yapabildiği bu karelerden çıkmıyor.

Ayrıca cari yapısının içinde personel carileri var: çalışan da bir karşı taraf ve maaş ödemesi o cariden geçiyor.

### Canlı beş üründe karşılığı *(Çıkarım)*

Beş üründe muhasebeci diye bir kavram yok. Veri dışarı ancak bir dosyayla çıkıyor: PDF, Excel veya e-posta eki.

Yani ilişki tek yönlü ve zamansız: kullanıcı dosyayı gönderiyor, muhasebeci o anın kopyasını alıyor. Ön muhasebe tarafında ilişki çift yönlü ve sürekli.

### Yetkinin bedeli *(Çıkarım)*

"Müşteri adına işlem yapma" yetkisi, verinin sahibi ile onu değiştirebilen kişiyi birbirinden ayırıyor.

Bu ayrım bir soru doğuruyor: kaydı kimin yazdığı ve kimin sildiği nerede tutuluyor? Kaynaklarda bir iz katmanından söz edilmiyor.

### Bu sayfanın sınırı *(Kaynak beyanı)*

Üç ürünün hiçbirinde müşavir yüzeyi çalışırken görülmedi. Logo'nun video karesinde bir portal arayüzü görünüyor ama içeriği boş şablon çubukları.

Yetkilerin gerçekte nasıl sınırlandığı ölçülmedi.

- Muhasebecinin ürünün içinde olması, bu üç ürünün hedef kitlesinin bir uzantısı: belge üreten bir işletmenin bir de belgeleri işleyen biri var.

## 9.8 · Ön muhasebe yolu ve canlı ürünlerden ayrıldığı nokta

Aynı yol, bu kez kaynaktan. Ayrışma ikinci adımda başlıyor ve sonraki her adımı belirliyor.

**1. Ekonomik olay oluyor**

Bir iş yapılıyor, bir mal alınıyor ya da bir hizmet veriliyor. Sekiz bölümün hepsinde yol buradan başlıyor.


**2. Kayda nasıl geçiyor**

- *Canlı beş ürün* — Bir satır olarak. Kaydedildiği anda hesap bakiyesi değişiyor. (E0142)
**→** *KolayBi · Paraşüt · Logo İşbaşı* — Bir belge olarak: fatura ya da fiş. Cari borç o anda oluşuyor, kasa kıpırdamıyor. (E0200)

**3. Vergi nereye yazılıyor**

- *Canlı beş ürün* — Hiçbir yere. Dokuz bölümde vergi alanı hiç bulunmadı. (E0229)
**→** *KolayBi · Paraşüt* — Belgenin üstüne. Oran ve matrah faturanın kalemi; rapor kayıtları yeniden okumaktan ibaret. (E0214)

**4. Para ne zaman hareket ediyor**

- *Canlı beş ürün* — Aynı anda. Kayıt ile para hareketi tek bir olay. (E0075)
**→** *KolayBi · Paraşüt · Logo İşbaşı* — Ayrı bir ödeme veya tahsilat adımında. Arada kayıt ödenmemiş olarak bekliyor. (E0199)

**5. Kim görüyor**

- *Canlı beş ürün* — Yalnız kullanıcı. Veri dışarı ancak bir dosyayla çıkıyor. (E0169)
**→** *Paraşüt · Logo İşbaşı* — Muhasebeci de. Erişim sürekli ve bir üründe müşteri adına işlem yapmaya kadar gidiyor. (isbasi.com)

- Bu akışın iki dalı da aynı ekonomik olayı anlatıyor; ayrışan şey olayın ne zaman paraya dönüştüğü.
- Dördüncü adımdaki bekleme süresi, ön muhasebe tarafının bütün cari ve vade kavramlarının kaynağı: ödenmemiş kayıtlar bir yerde durmak zorunda.

## 9.9 · Neden ayrışıyorlar

Bu üç ürün aynı işi daha kötü yapmıyor; başka bir işi yapıyor. Fark, kullanıcının kim olduğu varsayımından çıkıyor ve bütün modeli belirliyor.

### Belge, olayı paradan ayırıyor *(Çıkarım)*

Üç kaynakta da kayıt iki adıma bölünmüş: belge ekonomik olayı kayda geçiriyor, ödeme parayı taşıyor. Fatura kesildiğinde cari borç oluşuyor ama kasa kıpırdamıyor; kasa ancak ödeme adımında değişiyor.

Canlı ürünlerde bu ayrım genel bir kural değil, yalnız iki yerde beliriyor: kart harcaması borcu yazıp ödemeyi sonraya bırakıyor (Bölüm 3), Bluecoins'in cari hesabı faturayı gelire yazıp tahsilatı sonraya bırakıyor (Bölüm 4). Sıradan bir gelir ya da gider kaydında ise Bölüm 1'de görüldüğü gibi kayıt yazıldığı anda bakiye değişiyor — tek adım, tek an.

Ayrımın bedeli ve kazancı aynı yerden geliyor: iki adım olduğu için ödenmemiş kayıtların bir yerde durması gerekiyor, ve cari kavramı tam da o yer.

*Dayanağı: E0192, E0199, E0200, parasut.com kılavuzu, isbasi.com*

### Belgenin bir karşı tarafı olmak zorunda *(Çıkarım)*

Fatura formunda cari alanı zorunlu: belge her zaman birine kesiliyor. Carisi olmayan gider için ayrı bir yol bırakılmış ve listede "Genel Gider" olarak işaretleniyor — yani karşı tarafsız kayıt istisna.

Canlı ürünlerde sıradan kayıt kimseye ait değil; yalnız bir hesaba ve bir sınıflandırmaya bağlı. Wallet'ın borç nesnesi ve Bluecoins'in cari hesabı istisna, ama ikisinde de karşı taraf kaydın kendisinde değil, ayrı bir nesnede ya da hesapta duruyor.

Karşı tarafın zorunlu olması, cari bakiyesini ve vadeyi kendiliğinden mümkün kılıyor: kime ne kadar borçlu olduğunuz zaten kayıtlı.

*Dayanağı: E0200, E0198, E0316*

### Bir kaydın kaç defteri beslediği ürünün kapsamını gösteriyor *(Çıkarım)*

Logo'nun kaynağı en açık cümleyi kuruyor: bir işlem kaydedildiğinde cari, kasa-banka ve stok birlikte güncelleniyor. Üç defter tek kayıttan besleniyor.

Canlı ürünlerin dördünde defter sayısı bir. Goodbudget'ta iki — zarf ve hesap — ve Bölüm 1'de görüldüğü gibi ikisi tutmuyor.

Defter sayısı arttıkça kaydın tek bir anda doğru olması zorlaşıyor; bunun karşılığında stok ve cari gibi sorular kendiliğinden cevaplanıyor.

*Dayanağı: isbasi.com, E0417, E0421*

### Vergi belgenin üstünde durduğu için raporlanabiliyor *(Çıkarım)*

Oran ve matrah faturanın kalemi olduğu için vergi raporu kayıtları yeniden okumaktan ibaret; ürün ayrıca bir yere bakmıyor. Biri oran başına matrah ve tutarı ayrı sütunlarda topluyor, ötekinin kaynağı ay bazında üç sayı ürettiğini yazıyor.

Canlı beş üründe vergi diye bir alan Bölüm 1'in form taramasında bulunmadı; dolayısıyla böyle bir rapor da yok. Bu bir eksiklik değil, ayrı bir kullanıcı varsayımı — beyanname veren biri ile ayını takip eden biri aynı kişi değil.

Bedeli sorumluluk: oran listesi ürünün içinde yaşıyor. Oran değiştiğinde listeyi güncel tutmak ürüne ya da kullanıcıya düşüyor; güncellenmezse yeni belgeler eski oranla kesiliyor. Kazancı, ayın en zahmetli işinin hazır çıkması.

*Dayanağı: E0214, E0213, parasut.com kılavuzu*

### Üründeki belge ile resmî belge aynı şey değil *(Çıkarım)*

Üründe bir fatura oluşturmak, onu resmî bir fatura yapmıyor. Bir kaynak bunu kendi cümlesiyle söylüyor: üründe oluşturulan fatura kayıtlarının resmî değeri yok; resmîlik ya matbu çıktıyla ya da ayrıca satın alınan bir e-belge modülüyle geliyor.

Üçünde de e-belge abonelikten ayrı fiyatlanıyor ve kontör denen ikinci bir sayaca bağlanıyor. İki kaynak gelen faturanın da kontör yaktığını yazıyor — yani gider tarafı, kullanıcının kendi kararına bağlı olmayan bir tüketim kalemi taşıyor.

Bu, bölümün ana ayrımını bir kat daha açıyor: kayıt ile ödemenin ayrılması üründe olan bir şey, belgenin resmîleşmesi ise ürünün dışında — vergi idaresiyle ve ayrı bir bedelle.

*Dayanağı: isbasi.com işlem rehberi, parasut.com kılavuzu, kolaybi.com fiyat sayfası*

### Muhasebecinin ürünün içinde olması modeli tamamlıyor *(Çıkarım)*

Belge üreten bir işletmenin bir de o belgeleri işleyen biri var. Üç kaynak da muhasebeciyi ürünün içine almış: biri davetle, biri canlı erişimle, biri müşteri adına işlem yapabilen bir portalla.

Canlı ürünlerde muhasebeciye ayrılmış bir erişim yok; veri dışarı bir dosyayla çıkıyor. İlişki tek yönlü ve zamansız: kullanıcı o anın kopyasını gönderiyor.

En geniş yetkiyi veren kaynak "müşteri adına işlemleri gerçekleştirebilir" deyip orada duruyor: yetkinin sınırını, bir rol tanımını ve kaydı kimin yazdığını gösteren bir iz katmanını anlatan sayfa arandı, bulunamadı.

*Dayanağı: isbasi.com, parasut.com kılavuzu, E0187*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **KolayBi** | Model formda görünüyor: ödeme durumu ve cari takibi ayrı seçimler; cari, proje, çek-senet ve personel carisi birinci sınıf kavramlar; stok birimi varyant ile deponun çifti; vergi oran başına matrah ve tutar olarak raporlanıyor | Fatura her zaman bir cariye kesiliyor ve e-belge abonelikten ayrı, kontörle fiyatlanıyor; kayıt canlı ürünlerdeki tek satırdan uzun bir iş |
| **Paraşüt** | Kaynağa göre gider beş türe ayrılmış, kısmi ödeme destekleniyor, tahsilat en gecikmiş açık faturadan başlayarak otomatik mahsuplaşıyor, stok ek ücretsiz ve çoklu depolu, vergi ay bazında hesaplanan/indirilecek/net olarak çıkıyor | Kaynağa göre e-belge abonelikten ayrı bir modül ve kontörle fiyatlanıyor; kayıt ile ödeme iki ayrı adım olduğu için her gider iki kez ele alınıyor |
| **Logo İşbaşı** | Kaynağa göre tek kayıt üç defteri birden besliyor; fiş fotoğraftan okunuyor, fatura telefon sallanarak ve hazır şablonla sesle kesilebiliyor, müşavir portalı ay sonu dosya alışverişini kaldırıyor | Ürünün kendi ifadesiyle üründe oluşturulan fatura kayıtlarının resmî değeri yok; e-fatura ayrı modülle geliyor. Müşavir müşteri adına işlem yapabiliyor ama yetki sınırı ve kaydı kimin yazdığını gösteren iz kaynakta tanımlı değil |

**Belge 3'e taşınan soru**

- Uygulama vergi tutarını hesaplamalı mı, yoksa kullanıcının girdiğini taşıyıp raporlamalı mı? Hesaplayan ürün ayın en zahmetli işini üstleniyor; karşılığında oran listesini güncel tutma sorumluluğunu ve yanlış sayının sonucunu alıyor.
- Kaydın bir kalemi olmalı mı? Kalem taşıyan üründe "ne kadar kaldı" sorusu kendiliğinden cevaplanıyor; taşımayanda kayıt daha kısa ve ne alındığı serbest metinde kalıyor.
- Ekonomik olayın kaydı ile para hareketi ayrı iki adım mı olmalı? Üç kaynak her kayıtta ayırıyor; canlı ürünler yalnız kartta ve caride ayırıyor, ve ikisi de kendi kullanıcısı için tutarlı.
- Kaydın bir karşı tarafı olmak zorunda mı? Zorunlu olduğunda cari bakiyesi ve vade kendiliğinden geliyor; olmadığında kayıt daha hızlı giriliyor.
- Bir kayıt kaç defteri beslemeli? Defter sayısı arttıkça kapsam genişliyor, tutarlılık zorlaşıyor.
- Muhasebeci ürünün içinde mi olmalı, dışında mı? İçindeyse veriyi değiştirebilen kişiyle sahibini ayıran bir iz katmanı gerekiyor.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 9.1 | E0200 | KolayBi | Kaynak görseli | KolayBi · Yeni Alış Faturası |
| 9.2 | E0198 | KolayBi | Kaynak görseli | KolayBi · Genel Giderler listesi |
| 9.3 | E0209 | KolayBi | Kaynak görseli | KolayBi · Çekler |
| 9.4 | E0210 | KolayBi | Kaynak görseli | KolayBi · Senetler |
| 9.5 | E0214 | KolayBi | Kaynak görseli | KolayBi · KDV Raporu |
| 9.6 | E0213 | KolayBi | Kaynak görseli | KolayBi · Alış/Satış Raporu |


---

# Bölüm 10 · Ürünlerin ayrıldığı yer

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Dokuz bölümde ayrı ayrı görülen farklar tek bir yere mi çıkıyor?

Bu bölüm yeni kanıt getirmez. Önceki dokuz bölümde ayrı ayrı ölçülen davranışları yan yana koyup üç tekrar eden deseni gösterir ve belgenin bıraktığı soruları toplar.

Her cümlenin dayanağı ilgili bölümdedir; burada tekrar kare basılmaz.

**Bu bölüme girmez**

- Yeni gözlem veya yeni kare → yok
- Kararlar ve öneriler → Belge 3

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare | Dokuz bölümün sekizinde ölçüldü. |
| Bluecoins | Canlı kare | Dokuz bölümün sekizinde ölçüldü. |
| Wallet | Canlı kare | Dokuz bölümün sekizinde ölçüldü. |
| Hesap Defterim | Canlı kare | Dokuz bölümün yedisinde ölçüldü. |
| Goodbudget | Canlı kare | Ücretsiz paket sınırı üç bölümü kapattı. |
| KolayBi | Kaynak görseli | Sekiz bölümde yüzey olarak; Bölüm 9'da modeliyle. |
| Paraşüt | Kaynak beyanı | Model olarak Bölüm 9'da. |
| Logo İşbaşı | Kaynak beyanı | Model olarak Bölüm 9'da. |
| QuickBooks Solopreneur | Kaynak beyanı | Kendi sayfası Bölüm 6'da. |


## 10.1 · Dokuz bölüm, dokuz ayrım noktası

Her bölümün bulduğu asıl ayrım tek cümleyle. Sağdaki sütun o ayrımın hangi ürünü diğerlerinden en çok uzaklaştırdığını gösteriyor.

| Bölüm | Ayrımın çıktığı yer | En uçtaki ürün |
|---|---|---|
| 1 · Gelir ve gider | *Canlı kare* · Ürünün kaydı kaç deftere yazdığı ve o defterlerin toplamına ne ad verdiği | *Canlı kare* · Hesap Defterim — ayın gideri 6.250 okunuyor, gerçek gider 2.050 |
| 2 · Hesaplar ve aktarım | *Canlı kare* · Açılış parasının kayıt mı alan mı olduğu; aktarımın iki ucunun bağlı olup olmadığı | *Koşum kaydı* · Hesap Defterim — bacaklar bağımsız, biri silinince net varlık sessizce bozuluyor |
| 3 · Kart | *Canlı kare* · Kartın bir dönemi olup olmadığı | *Canlı kare* · Money Manager — aynı kart için iki ekranda 1.600 ve 5.600 |
| 4 · Borç ve tahsilat | *Canlı kare* · Alacağın bir hesap mı ayrı bir nesne mi olduğu | *Canlı kare* · Wallet — borcun bakiyeye dokunup dokunmayacağını kullanıcıya soruyor |
| 5 · Zaman ve plan | *Canlı kare* · Tanım ile kayıt arasındaki kapının kime ait olduğu | *Görülmedi* · Hesap Defterim — kapı yok, çünkü plan kavramı hiç yok |
| 6 · Sınıflandırma | *Canlı kare* · Kayıt düzeyinde işletme/şahsi alanının olup olmadığı | *Kaynak beyanı* · QuickBooks — dokuz üründe böyle bir alanı olan tek ürün |
| 7 · Rapor | *Canlı kare* · Toplamın tanımının ve döneminin ekranda yazılıp yazılmadığı | *Canlı kare* · Goodbudget — harcama raporunun toplamı bir gelir kaydını içeriyor |
| 8 · Veri | *Canlı kare* · Verinin cihazda mı hesapta mı durduğu ve bunun söylenip söylenmediği | *Canlı kare* · Hesap Defterim — "yedek kapalıysa geri yükleyemeyiz" diye yazıyor |
| 9 · Ön muhasebe tarafı | *Kaynak görseli* · Kaydın bir satır mı belge mi olduğu — ve belgenin yanında vergi, kalem ve müşaviri de getirip getirmediği | *Kaynak görseli* · Üç ön muhasebe ürünü — kayıt ile ödeme ayrı anlar, vergi belgenin üstünde |

- Dokuz ayrımın yedisi tek bir soruya bağlanıyor: ürün bir kaydı kaç yere yazıyor ve o yerlerin her birine ne ad veriyor.
- Kalan ikisi kullanıcıya sorulan sorularla ilgili — ve ikisi de aynı üründen çıkıyor.

## 10.2 · Üç tekrar eden desen

Dokuz bölüm boyunca aynı üç şey farklı konularda tekrar etti. Üçü de tek tek küçük görünüyor ama birlikte ürünlerin karakterini belirliyor.

### 1 · Aynı şeyin iki sayısı *(Çıkarım)*

Üç ayrı bölümde aynı desen çıktı. Kart bölümünde aynı kart için Hesaplar ekranı 1.600, kart defteri 5.600 gösterdi. Borç bölümünde borç kartı "OWES ME 5.000" derken kayıt listesi "Total −5.000" dedi. Rapor bölümünde benzer adlı iki blok biri dönem akışını, öteki varlığı saydı.

Üçünde de iki sayı da doğru. Eksik olan şey aynı: her sayının hangi soruyu cevapladığı ekranda yazmıyor.

### 2 · Kaç defter, o kadar kural *(Çıkarım)*

Bir kaydı tek deftere yazan ürünün toplamı kendiliğinden tutuyor. İkinci bir defter açan ürün, iki defterin nasıl uyuşacağını ayrıca çözmek zorunda.

Goodbudget zarf ile hesabı ayırdı ve ikisi tutmadı: fark her zarf doldurmada büyüdü. Hesap Defterim her şeyi tek deftere yazdı ve bu kez toplamların tanımı kirlendi.

Kaynak taraf üç defteri birden besliyor ve bunu ürünün kapsamı olarak sunuyor — cari, kasa ve stok.

### 3 · Kararı kim veriyor *(Çıkarım)*

Üç yerde bir ürün kararı kendisi vermek yerine kullanıcıya sordu ve sorunun sonucunu aynı cümlede yazdı: borç bakiyeye dokunsun mu, gelecek ödemeler kendiliğinden kayda dönüşsün mü.

Bir başka üründe de benzer bir soru var: plan kayda çevrilirken hangi tarihe yazılsın — bugüne mi, planlanan güne mi. Money Manager ise tekrarlı kaydı kurarken sonucunu yazan bir soru soruyor, ama sorduğu şey kurulumun kendisi; gerçekleşmeyi kullanıcıya bırakmıyor.

Diğer yerlerde bu kararlar verilmiş ve yazılmamış. Kullanıcı sonucu ancak toplamlara bakarak anlıyor.

### Desenlerin ortak kökü *(Çıkarım)*

Üç desen de aynı yere çıkıyor: ürünün kendi tanımını kullanıcıya gösterip göstermediği.

Money Manager bunun tersini yapan tek örnek: toplamı tek sayı yerine ödeme kaynağına göre bölüp satır adına yazıyor. Bedeli okuma yükü — kullanıcı tek sayı yerine dört satır okuyor.

### Ortak olan az şey *(Çıkarım)*

Dokuz bölümde yalnız iki davranış bütün ölçülen ürünlerde aynı çıktı.

Birincisi: kart ödemesi hiçbir üründe ayın giderine girmiyor — harcama zaten kart kullanıldığı gün gider yazıldı.

İkincisi: gerçekleşmemiş plan hiçbir üründe ayın toplamına girmiyor. Plan görünür kalıyor ama sayılmıyor.

### Bu belgenin sınırı *(Kaynak beyanı)*

Beş ürün canlı koşuldu, biri destek görselleriyle, üçü yalnız kendi anlatımıyla incelendi. Kanıt düzeyi her sayfada rozetle yazılı.

Ücretsiz paket sınırları üç konuyu tamamen kapattı: Goodbudget'ta kart, aktarım ve ikinci hesap. Bunlar üründe yokluk değil, erişim eksikliğidir.

- Üç desenin hiçbiri bir ürünün hatası değil; her biri bir tasarım tercihinin görünen yüzü. Ürünlerin nasıl çalıştığı 10.3'te, tercihlerin bedeli 10.4'te.

## 10.3 · Beş ürünün motoru, aynı beş soruyla

Dokuz bölüm boyunca ürünler konu konu okundu. Bu tablo aynı kanıtı ürün ürün okuyor: beş soru beşine de aynı biçimde soruluyor, böylece bir sütun baştan aşağı tek bir ürünün modeli olarak okunabiliyor. Yeni kanıt yok; her hücre ilgili bölüme dayanıyor.

|  | Money Manager | Bluecoins | Wallet | Hesap Defterim | Goodbudget |
|---|---|---|---|---|---|
| Para nerede yaşıyor | *Canlı kare* · Hesap defterlerinde; kartın kesim günü olan kendi dönemi var | *Canlı kare* · Hesap gruplarında: banka, nakit, cari ve kart aynı listede | *Canlı kare* · Hesaplarda; borç ayrı bir nesne ve kendi ekranında | *Canlı kare* · Tek defterde; kart, taksit, plan ve cari kavramı yok | *Canlı kare* · İki defterde: zarf ve hesap; ikisi tutmuyor |
| Kayıt ile ödeme ayrılıyor mu | *Canlı kare* · Kartta evet: harcama gider yazıyor, ödeme ayrı yüzeyden geçiyor ve gidere girmiyor | *Canlı kare* · Cari hesapta evet: borçlandırma banka değişmeden gelir yazıyor | *Canlı kare* · Kullanıcıya soruluyor: borç kayıtla mı kayıtsız mı oluşsun | *Canlı kare* · Hayır; kayıt yazıldığı anda denge değişiyor | *Görülmedi* · Ölçülemedi; ücretsiz pakette kart ve aktarım kapalı |
| Gelecek nasıl tutuluyor | *Canlı kare* · İki ayrı biçim: taksit gelecek ayın gider toplamına giriyor, tekrarlayan plan önizlemede kalıyor ve vadesi gelince sorulmadan kayda dönüşüyor | *Koşum kaydı* · Hatırlatıcı listesi; hiçbir vade kendiliğinden gerçekleşmiyor, kaydederken hangi tarihe yazılacağı soruluyor | *Canlı kare* · Planned payments; kapının açık kalıp kalmayacağı soruluyor ve sonradan değişebiliyor | *Görülmedi* · Gelecek diye bir yer yok; unutulan ödeme iz bırakmıyor | *Canlı kare* · İşlem formunda Schedule kutusu var; gerçekleşmesi ölçülmedi |
| Ayın toplamı neyi sayıyor | *Canlı kare* · Kaynağına göre bölünmüş: nakit-banka gideri, kart harcaması ve ödeme ayrı satırlar | *Canlı kare* · Dönem akışını; ama benzer adlı ikinci bir blok varlığı sayıyor | *Canlı kare* · Kayan bir pencereyi — son 30 gün ya da 12 hafta; takvim ayı ayrıca seçiliyor | *Canlı kare* · Her satırı: açılış ve aktarım bacakları dâhil; ayın gideri 6.250 okunurken gerçek gider 2.050 | *Canlı kare* · Zarf başına kırılım; harcama raporunun toplamı bir gelir kaydını içeriyor |
| Toplamın tanımı ekranda yazıyor mu | *Canlı kare* · Evet — satır adlarında yazılı; dokuz üründe tek örnek | *Canlı kare* · Hayır; iki blok aynı ekranda, hangisinin neyi saydığı yazmıyor | *Canlı kare* · Hayır; ama kararı sorarken sonucunu aynı cümlede yazıyor | *Canlı kare* · Hayır; sütun adları değişiyor, tanım değişmiyor | *Canlı kare* · Hayır |

- Sütunlar baştan aşağı okunduğunda her ürünün kendi mantığı çıkıyor; satırlar yan yana okunduğunda aynı sorunun beş cevabı. Tablonun işi bu iki okumayı aynı anda mümkün kılmak.
- Dördüncü satır belgenin en çok tekrarlanan bulgusunu taşıyor: beş üründe "ayın toplamı" beş ayrı şeyi sayıyor ve beşi de kendi içinde tutarlı.
- Kaynaktan incelenen dört ürün bu tabloda yok; onların modeli Bölüm 9'da kendi bölümünde kuruldu ve kanıt düzeyi farklı olduğu için yan yana konmadı.

## 10.4 · Ürün ürün: neyi iyi yapıyor, neyi bırakıyor

Her ürün için iki satır: dokuz bölümde en güçlü olduğu yer ve en çok kaybettirdiği yer.

| Ürün | En güçlü olduğu yer | En çok kaybettirdiği yer |
|---|---|---|
| Money Manager | *Canlı kare* · Toplamın tanımını ekrana yazan tek ürün: gider ödeme kaynağına göre bölünmüş, aktarım kendi satırında; kart dönemli ve taksit aylara dağılıyor | *Canlı kare* · Aynı kart için iki ekran iki farklı borç gösteriyor ve hangisinin hangi dönemi kapsadığı yazmıyor |
| Bluecoins | *Canlı kare* · Cari bir hesap türü olduğu için tahsilat doğal bir aktarım; aktarımın iki bacağı günde netleşiyor; plan kayda çevrilirken tarih soruluyor | *Canlı kare* · Benzer adlı iki toplam aynı ekranda: biri dönem akışı, öteki varlık |
| Wallet | *Canlı kare* · Bir kaydın bakiyeye ve kapının davranışına dair kararı kullanıcıya soran ve sonucunu yazan tek ürün; borç ayrı bir nesne, vadesi var; bütçe tahminle birlikte geliyor | *Canlı kare* · Rapor aralığı takvim ayı değil kayan bir pencere; kart dönemsiz ve taksit bölünmüyor |
| Hesap Defterim | *Canlı kare* · Defter mantığı tek sayıda doğru neti veriyor; verinin cihazda durduğunu açıkça yazıyor; silinen kayıt için ayrı bir liste var | *Canlı kare* · Ayın gelir ve gider toplamı kullanılamıyor; kart, taksit, plan ve cari kavramlarının hiçbiri yok |
| Goodbudget | *Canlı kare* · Para amacına göre bölünmüş; bütçe ile bakiye aynı şey olduğu için ayrı bir bütçe kavramı gerekmiyor | *Canlı kare* · Zarf ile hesap tutmuyor ve fark her doldurmada büyüyor; harcama raporunun toplamı bir gelir kaydını içeriyor |
| KolayBi | *Kaynak görseli* · Cari, proje, çek-senet ve personel carisi birinci sınıf kavramlar; stok birimi varyant ile deponun çifti; vergi oran başına matrah ve tutar olarak raporlanıyor | *Görülmedi* · Yalnız yüzey görüldü; hiçbir alanın kasaya, cariye veya rapora etkisi ölçülemedi |
| QuickBooks Solopreneur | *Kaynak beyanı* · Kayıt başına işletme/şahsi alanı ve kısmen işletme gider için bölme; şahsi kayıt silinmeden toplamdan çıkıyor | *Görülmedi* · Ürünün hiçbir iç ekranı görülmedi; ayrım ülkeye özgü bir vergi formuna bağlı |
| Paraşüt | *Kaynak beyanı* · Kaynağa göre gider beş türe ayrılmış, tahsilat en gecikmiş açık faturadan başlayarak otomatik mahsuplaşıyor, vergi ay bazında net KDV'ye kadar gidiyor ve stok ek ücretsiz | *Görülmedi* · İç arayüz hiç görülmedi; elde yalnız mağaza ve tanıtım videosu kareleri var |
| Logo İşbaşı | *Kaynak beyanı* · Kaynağa göre tek kayıt cari, kasa-banka ve stoku birlikte güncelliyor; fatura sesle kesilebiliyor; müşavir portalı dosya alışverişini kaldırıyor | *Görülmedi* · İç arayüz hiç görülmedi; müşavirin yetki sınırı ve iz katmanı arandı, kaynakta yok. Ürünün kendi ifadesiyle oluşturulan fatura kayıtlarının resmî değeri de yok |

- Sağdaki sütun bir puanlama değil: her satır bir tasarım tercihinin bedeli. İki sütun da aynı tercihten doğuyor.
- Son dört ürünün kaybettirdiği yer aynı: erişim. Bu, ürünün değil araştırmanın sınırı.

## 10.5 · Bir paranın belgedeki yolu

Dokuz bölümün akış sayfaları tek bir yolda birleştiğinde şu çıkıyor. Her adımda ürünlerin ayrıldığı nokta, ilgili bölümde kareyle gösterilmişti.

**1. Olay oluyor, kayda geçiyor**

- *Canlı beş ürün* — Bir satır. Yazıldığı anda hesap bakiyesi değişiyor ve ayın toplamına giriyor. (E0142)
**→** *Üç ön muhasebe ürünü* — Bir belge. Cari borç oluşuyor, kasa kıpırdamıyor; ödeme ayrı bir adım. (E0200)

**2. Kaç deftere yazılıyor**

- *Dört canlı ürün* — Tek defter. Toplam kendiliğinden tutuyor. (E0235)
**→** *Goodbudget* — İki defter: zarf ve hesap. İkisi tutmuyor ve fark büyüyor. (E0421)
- *Logo İşbaşı* — Üç defter: cari, kasa-banka ve stok birlikte. (isbasi.com)

**3. Gelecekteki para ne oluyor**

- *Money Manager · Wallet · Bluecoins* — Plan görünür kalıyor ama ayın toplamına girmiyor; gerçekleşme ürüne göre kendiliğinden, onayla ya da elle. (E0244 · E0291 · E0430)
**→** *Hesap Defterim* — Gelecek diye bir yer yok. Unutulan ödeme hiç iz bırakmıyor. (E0171)
- *Ön muhasebe tarafı* — Ödenmemiş belgeler cari ve vade kavramlarında bekliyor. (parasut.com kılavuzu)

**4. Toplamda okunuyor**

**→** *Money Manager* — Toplamın tanımı satır adlarında yazılı; aktarım kendi satırında. (E0405)
- *Bluecoins · Wallet · Goodbudget* — Tek sayı veriliyor; neyi saydığı ekranda yazmıyor. (E0032)
- *Hesap Defterim* — Toplam her satırı içeriyor: açılış ve aktarım bacakları dâhil. (E0142)

- Bu yol dokuz bölümün akış sayfalarının birleşimidir; her dalın kanıtı ilgili bölümdedir.
- İlk adımdaki çatallanma diğer üçünü de belirliyor: kaydı bir satır mı yoksa bir belge mi sayıyorsunuz.

## 10.6 · Belgenin bıraktığı yer

Bu belge rakipleri anlattı ve puanlamadı. Ortaya çıkan şey bir sıralama değil, bir soru listesi — ve o liste Belge 3'ün girdisi. Bölümlerde 37 soru var; aşağıdaki liste her bölümü en az bir soruyla temsil ediyor.

### Ayrışmanın tek bir kökü var *(Çıkarım)*

Dokuz bölümde ölçülen farkların çoğu — 10.1'de dokuz ayrımın yedisi — tek bir soruya bağlanıyor: ürün bir kaydı kaç yere yazıyor ve o yerlerin her birine ne ad veriyor.

Tek deftere yazan üründe toplam kendiliğinden tutuyor ama defterin tanımı kirlenebiliyor. İki deftere yazanda tanım temiz kalıyor ama iki defterin uyuşması ayrıca çözülmesi gereken bir iş oluyor.

Üç deftere yazan taraf bunu bir kapsam genişlemesi olarak sunuyor — cari, kasa ve stok birlikte — ve karşılığında kaydın bir karşı tarafı olmasını zorunlu kılıyor.

*Dayanağı: E0142, E0421, isbasi.com, E0200*

### Ürünler sayıyı veriyor, tanımı vermiyor *(Çıkarım)*

Üç ayrı bölümde aynı sonuç çıktı: aynı şeyin iki ekranda iki sayısı var ve ikisi de doğru. Eksik olan tanım — hangi sayının hangi soruyu cevapladığı.

Bunu ekranda yazan tek örnek, toplamı ödeme kaynağına göre bölüp satır adına yazan yüzey. Bedeli okuma yükü; kazancı yanlış okumanın zorlaşması.

Bu belgenin en çok tekrarlanan bulgusu bu: kart, borç ve rapor bölümlerinde üç ayrı konuda çıktı.

*Dayanağı: E0413, E0317, E0032, E0405*

### Kararı kullanıcıya sormak nadir ama mümkün *(Çıkarım)*

İki üründe üç ayrı yerde ürün kendi kararını vermek yerine kullanıcıya sordu: borç bakiyeye dokunsun mu, gelecek ödemeler kendiliğinden kayda dönüşsün mü, plan hangi tarihe yazılsın.

Üçünde de soru, cevabın sonucunu aynı cümlede söylüyor. Bu, tanımı ekranda yazmanın bir başka biçimi: ürün kendi davranışını kullanıcıya açıklıyor.

Money Manager'ın tekrarlı kayıt sorusu da sonucunu yazıyor — "Tarihte tekrar eden işlemler uygulanır" — ama kullanıcıya bir kapı vermiyor, kapının olmadığını haber veriyor.

Diğer yerlerde bu kararlar verilmiş ve yazılmamış; kullanıcı sonucu ancak toplamlara bakarak anlıyor.

*Dayanağı: E0315, E0291, E0043, E0430*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Tek defter** | Toplam kendiliğinden tutuyor; ikinci bir uyuşma kuralı gerekmiyor | Defterin tanımı kirlenebiliyor: açılış ve aktarım bacakları aynı toplama giriyor |
| **İki defter** | Amaç ile para ayrı okunabiliyor; kullanıcı ne kadar harcayabileceğini doğrudan görüyor | İki defterin uyuşması ayrı bir iş; uyuşmazsa fark sessizce büyüyor |
| **Üç defter · belge tabanlı** | Cari, stok ve kasa tek kayıttan besleniyor; vade ve karşı taraf kendiliğinden geliyor | Kaydın bir karşı tarafı olmak zorunda ve kayıt iki adıma bölünüyor |
| **Tanımı yazan yüzey** | Yanlış okuma zorlaşıyor: toplamın neyi içerdiği satır adında yazılı | Okuma yükü artıyor: tek sayı yerine birkaç satır |
| **Kararı soran yüzey** | Ürünün davranışı kullanıcıya açıklanıyor ve iki yol da açık kalıyor | Her kararda bir soru daha; akış uzuyor |

**Belge 3'e taşınan soru**

- Kayıt kaç deftere yazılmalı ve o defterlerin uyuşması nasıl garanti edilmeli?
- Ayın gelir ve gider toplamı hangi kayıtları içermeli? Açılış bakiyesi, aktarım bacakları ve kart ödemesi üç ayrı sınav ve ürünler üç ayrı cevap veriyor.
- Aynı şeyin iki sayısı olabilir mi? Üç bölümde de olabildiği görüldü; sorun sayılar değil, aralarındaki farkın görünmemesi.
- Bir sayının neyi ve hangi dönemi kapsadığı ekranda yazmalı mı? Yazmanın bedeli okuma yükü, yazmamanın bedeli yanlış okuma.
- Bir kaydın bakiyeye dokunup dokunmayacağı kullanıcıya sorulmalı mı, yoksa ürünün kararı mı olmalı?
- Ekonomik olayın kaydı ile para hareketi ayrı iki adım mı olmalı? Ayrıldığında cari ve vade kavramları gerekiyor; ayrılmadığında henüz gelmemiş para kayda giremiyor.
- İşletme ile şahsi ayrımı sabit bir alan mı, kullanıcı tanımlı bir eksen mi olmalı? Dokuz üründe yalnız biri sabit alan kullanıyor ve o da hiç görülemedi.
- Plan gerçek kayda hangi kapıdan ve hangi tarihle dönmeli? Ürünler planı kendiliğinden, onayla ya da elle dönüştürüyor; seçilen tarih ayın toplamını değiştiriyor.
- Veri kimin sorumluluğunda durmalı: cihazın mı, hesabın mı? İki model iki ayrı sorumluluk dağılımı üretiyor ve kullanıcı bunu ancak ürün ekranda yazarsa öğreniyor.
- Uygulama vergi tutarını hesaplamalı mı, yoksa kullanıcının girdiğini taşıyıp raporlamalı mı? Hesaplayan ürün ayın en zahmetli işini üstleniyor; karşılığında oran listesini güncel tutma sorumluluğunu ve yanlış sayının sonucunu alıyor.
- Kaydın bir kalemi olmalı mı? Kalem taşıyan üründe "ne kadar kaldı" sorusu kendiliğinden cevaplanıyor; taşımayanda kayıt daha kısa ve ne alındığı serbest metinde kalıyor.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|


---

# Kanıt eki

| Kimlik | Ürün | Tür | Dosya | Basıldığı şekiller |
|---|---|---|---|---|
| E0020 | Bluecoins | Canlı kare | `bluecoins/04-islem-formu.png` | 1.2, 6.2 |
| E0022 | Bluecoins | Canlı kare | `bluecoins/06-islem-listesi.png` | 2.10 |
| E0023 | Bluecoins | Canlı kare | `bluecoins/07-rapor.png` | 1.7, 7.3 |
| E0032 | Bluecoins | Canlı kare | `bluecoins/16-kontrol-degerleri-net-kazanc-44950.png` | 7.9 |
| E0037 | Bluecoins | Canlı kare | `bluecoins/21-b2-kalan-5-taksit-hatirlatici.png` | 3.10 |
| E0039 | Bluecoins | Canlı kare | `bluecoins/23-b1-planli-islem-aylik-sheet.png` | 5.3 |
| E0043 | Bluecoins | Canlı kare | `bluecoins/27-b1-islem-olarak-kaydet-bugun-mu.png` | 5.7 |
| E0047 | Bluecoins | Canlı kare | `bluecoins/31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png` | 5.13 |
| E0048 | Bluecoins | Canlı kare | `bluecoins/32-kart-hesap-kesim-gunu-limit-alanlari.png` | 3.2 |
| E0049 | Bluecoins | Canlı kare | `bluecoins/33-cari-hesap-olusturuldu.png` | 4.1 |
| E0050 | Bluecoins | Canlı kare | `bluecoins/34-kismi-kart-odemesi-500-transfer.png` | 3.18 |
| E0053 | Bluecoins | Canlı kare | `bluecoins/f7-02-hesaplar-scroll2.png` | 7.10 |
| E0055 | Bluecoins | Canlı kare | `bluecoins/f7-04-tum-hesaplar.png` | 4.12 |
| E0074 | Bluecoins | Canlı kare | `bluecoins/f7-23-hesap-dogru-secildi.png` | 4.9 |
| E0075 | Bluecoins | Canlı kare | `bluecoins/f7-24-d2-kaydedildi.png` | 4.10 |
| E0079 | Bluecoins | Canlı kare | `bluecoins/f7-28-d3-kaydedildi.png` | 4.11 |
| E0087 | Bluecoins | Canlı kare | `bluecoins/f7-36-kategoriler2.png` | 6.7 |
| E0088 | Bluecoins | Canlı kare | `bluecoins/f7-37-etiketler.png` | 6.6 |
| E0089 | Bluecoins | Canlı kare | `bluecoins/f7-38-cop-kutusu.png` | 8.8 |
| E0100 | Bluecoins | Canlı kare | `bluecoins/f7-49-seyahat-modu.png` | 8.14 |
| E0113 | Goodbudget | Canlı kare | `goodbudget/06-ana-hesap-created.png` | 2.4 |
| E0119 | Goodbudget | Canlı kare | `goodbudget/12-account-transfer-screen.png` | 2.9 |
| E0121 | Goodbudget | Canlı kare | `goodbudget/14-reports-default-current-month.png` | 7.6 |
| E0123 | Goodbudget | Canlı kare | `goodbudget/16-spending-by-envelope-negative-bug.png` | 7.13 |
| E0124 | Goodbudget | Canlı kare | `goodbudget/17-income-vs-spending-bug.png` | 7.14 |
| E0128 | Goodbudget | Canlı kare | `goodbudget/21-no-receipt-attachment.png` | 8.9 |
| E0138 | Hesap Defterim | Canlı kare | `hesap-defterim/04-islem-formu-alindi.png` | 1.4 |
| E0140 | Hesap Defterim | Canlı kare | `hesap-defterim/06b-islemler-butun-hesaplar.png` | 2.12 |
| E0142 | Hesap Defterim | Canlı kare | `hesap-defterim/07-rapor-aylik-butun-hesaplar.png` | 1.8 |
| E0143 | Hesap Defterim | Canlı kare | `hesap-defterim/08-silinmis-islemler.png` | 8.7 |
| E0145 | Hesap Defterim | Canlı kare | `hesap-defterim/10-hesap-eklem-formu.png` | 2.3 |
| E0147 | Hesap Defterim | Canlı kare | `hesap-defterim/12-aktar-transfer-formu.png` | 2.8 |
| E0148 | Hesap Defterim | Canlı kare | `hesap-defterim/13-ozet-tasarruf-agustos.png` | 7.8 |
| E0149 | Hesap Defterim | Canlı kare | `hesap-defterim/14-yedekleme-nag-dialog.png` | 8.6 |
| E0151 | Hesap Defterim | Canlı kare | `hesap-defterim/16-kategori-alani-acik-form.png` | 6.4 |
| E0153 | Hesap Defterim | Canlı kare | `hesap-defterim/18-kismi-odeme-is-karti.png` | 3.7 |
| E0161 | Hesap Defterim | Canlı kare | `hesap-defterim/27-yeniden-adlandirilmis-basliklar.png` | 6.9 |
| E0163 | Hesap Defterim | Canlı kare | `hesap-defterim/28-sifir-tutar-kabul-edildi.png` | 1.16 |
| E0165 | Hesap Defterim | Canlı kare | `hesap-defterim/30-not-defteri-checklist.png` | 8.12 |
| E0166 | Hesap Defterim | Canlı kare | `hesap-defterim/31-nakit-hesap-makinesi.png` | 8.13 |
| E0167 | Hesap Defterim | Canlı kare | `hesap-defterim/32-silinmis-islemler-context-menu.png` | 1.17 |
| E0176 | Hesap Defterim | Canlı kare | `hesap-defterim/41-bildiri-kasadefteri-uyarisi.png` | 8.2 |
| E0177 | Hesap Defterim | Canlı kare | `hesap-defterim/42-drawer-diger-uygulamalar-veresiye-gelirgider.png` | 4.4 |
| E0188 | KolayBi | Kaynak görseli | `kolaybi/d01-destek-proje-listesi.png` | 6.10 |
| E0190 | KolayBi | Kaynak görseli | `kolaybi/d03-destek-proje-detay-ozet.png` | 6.11 |
| E0193 | KolayBi | Kaynak görseli | `kolaybi/d06-destek-gider-tipleri.png` | 1.19 |
| E0194 | KolayBi | Kaynak görseli | `kolaybi/d07-destek-cari-listesi.png` | 4.18 |
| E0195 | KolayBi | Kaynak görseli | `kolaybi/d08-destek-cari-olusturma-formu.png` | 4.17 |
| E0196 | KolayBi | Kaynak görseli | `kolaybi/d09-destek-cari-detay-ekstre-dialog.png` | 8.10 |
| E0197 | KolayBi | Kaynak görseli | `kolaybi/d10-destek-cari-ekstre-onizleme.png` | 8.11 |
| E0198 | KolayBi | Kaynak görseli | `kolaybi/d11-destek-gider-listesi.png` | 9.2 |
| E0199 | KolayBi | Kaynak görseli | `kolaybi/d12-destek-gider-detay-islemler.png` | 1.18 |
| E0200 | KolayBi | Kaynak görseli | `kolaybi/d13-destek-alis-faturasi-formu.png` | 9.1 |
| E0205 | KolayBi | Kaynak görseli | `kolaybi/d18-destek-tekrarli-maas-formu.png` | 5.18 |
| E0208 | KolayBi | Kaynak görseli | `kolaybi/d21-destek-yeni-kredi-karti-formu.png` | 3.4 |
| E0209 | KolayBi | Kaynak görseli | `kolaybi/d22-destek-cekler.png` | 9.3 |
| E0210 | KolayBi | Kaynak görseli | `kolaybi/d23-destek-senetler.png` | 9.4 |
| E0213 | KolayBi | Kaynak görseli | `kolaybi/d26-destek-alis-satis-raporu.png` | 9.6 |
| E0214 | KolayBi | Kaynak görseli | `kolaybi/d27-destek-kdv-raporu.png` | 9.5 |
| E0215 | KolayBi | Kaynak görseli | `kolaybi/d28-destek-gelir-gider-raporu.png` | 7.16 |
| E0216 | KolayBi | Kaynak görseli | `kolaybi/d29-destek-nakit-akis-raporu.png` | 7.15 |
| E0229 | Money Manager | Canlı kare | `money-manager/04-islem-formu-ve-kategori.png` | 1.1, 6.1 |
| E0231 | Money Manager | Canlı kare | `money-manager/07-rapor-agustos.png` | 1.6, 7.2 |
| E0232 | Money Manager | Canlı kare | `money-manager/08-hata-toast-hesap-sec.png` | 1.14 |
| E0233 | Money Manager | Canlı kare | `money-manager/09-kart-hesap-ekstre-modeli.png` | 3.1 |
| E0236 | Money Manager | Canlı kare | `money-manager/12-hesaplar-kart-borcu-bu-ay.png` | 3.5 |
| E0238 | Money Manager | Canlı kare | `money-manager/14-tekrarlama-secenekleri.png` | 5.2 |
| E0239 | Money Manager | Canlı kare | `money-manager/15-acilis-bakiye-farki-dialog.png` | 2.1 |
| E0240 | Money Manager | Canlı kare | `money-manager/16-acilis-bakiye-farki-defter.png` | 2.2 |
| E0241 | Money Manager | Canlı kare | `money-manager/17-tekrarlayan-aylik-form.png` | 5.1 |
| E0244 | Money Manager | Canlı kare | `money-manager/20-tekrarlayan-ekim-onizleme.png` | 5.10 |
| E0245 | Money Manager | Canlı kare | `money-manager/21-taksit-6ay-form.png` | 3.8 |
| E0246 | Money Manager | Canlı kare | `money-manager/22-taksit-1-6-agustos.png` | 3.9 |
| E0248 | Money Manager | Canlı kare | `money-manager/24-kismi-kart-odemesi-400.png` | 3.16 |
| E0249 | Money Manager | Canlı kare | `money-manager/25-kismi-odeme-sonrasi-borc.png` | 3.17 |
| E0250 | Money Manager | Canlı kare | `money-manager/26-toplam-sekmesi-agustos.png` | 7.7 |
| E0251 | Money Manager | Canlı kare | `money-manager/27-filtre-paneli-agustos-hesap.png` | 7.11 |
| E0252 | Money Manager | Canlı kare | `money-manager/28-toplama-dahil-et-anahtari.png` | 2.13 |
| E0253 | Money Manager | Canlı kare | `money-manager/29-toplama-dahil-kapali-net-varlik.png` | 2.14 |
| E0254 | Money Manager | Canlı kare | `money-manager/30-ayarlar-izgarasi.png` | 8.3 |
| E0277 | Wallet | Canlı kare | `wallet-budgetbakers/04-islem-formu.png` | 1.3 |
| E0280 | Wallet | Canlı kare | `wallet-budgetbakers/07-rapor.png` | 7.1 |
| E0281 | Wallet | Canlı kare | `wallet-budgetbakers/08-hata-veya-bos-durum.png` | 1.13 |
| E0284 | Wallet | Canlı kare | `wallet-budgetbakers/11-kontrol-cash-flow-agustos.png` | 7.5 |
| E0285 | Wallet | Canlı kare | `wallet-budgetbakers/12-kontrol-records-listesi.png` | 2.11 |
| E0288 | Wallet | Canlı kare | `wallet-budgetbakers/15-b1-tekrarlayan-form.png` | 5.4 |
| E0289 | Wallet | Canlı kare | `wallet-budgetbakers/16-b1-plan-detay-due-today.png` | 5.5 |
| E0291 | Wallet | Canlı kare | `wallet-budgetbakers/18-b1-otomatik-mi-onayli-mi-secimi.png` | 5.6 |
| E0294 | Wallet | Canlı kare | `wallet-budgetbakers/21-b2-islem-detay-taksit-alani-yok.png` | 3.11, 6.3 |
| E0297 | Wallet | Canlı kare | `wallet-budgetbakers/24-kart-hesap-detay-negatif-bakiye.png` | 3.6 |
| E0298 | Wallet | Canlı kare | `wallet-budgetbakers/25-kart-hesap-ayarlari.png` | 3.3 |
| E0301 | Wallet | Canlı kare | `wallet-budgetbakers/28-butce-olusturuldu-over-budget.png` | 5.14 |
| E0302 | Wallet | Canlı kare | `wallet-budgetbakers/29-butce-detay-6600-harcama.png` | 5.15 |
| E0306 | Wallet | Canlı kare | `wallet-budgetbakers/33-planned-payments-b1-siradaki.png` | 5.12 |
| E0308 | Wallet | Canlı kare | `wallet-budgetbakers/35-debt-kayit-baglama-sorusu.png` | 4.5 |
| E0309 | Wallet | Canlı kare | `wallet-budgetbakers/36-debt-i-lent-formu.png` | 4.6 |
| E0315 | Wallet | Canlı kare | `wallet-budgetbakers/42-debt-kayit-olustur-mu-bakiye-degisir.png` | 4.7 |
| E0316 | Wallet | Canlı kare | `wallet-budgetbakers/43-debt-olusturuldu-i-lent.png` | 4.2 |
| E0317 | Wallet | Canlı kare | `wallet-budgetbakers/44-debt-records-loan-interests-kaydi.png` | 4.8 |
| E0318 | Wallet | Canlı kare | `wallet-budgetbakers/45-planned-otomatik-onayli-toggle-her-zaman.png` | 5.8 |
| E0358 | Wallet | Canlı kare | `wallet-budgetbakers/f7-37-debts-fab.png` | 4.3 |
| E0367 | Wallet | Canlı kare | `wallet-budgetbakers/f7-46-add-record-form.png` | 4.13 |
| E0368 | Wallet | Canlı kare | `wallet-budgetbakers/f7-47-new-record-form.png` | 4.14 |
| E0370 | Wallet | Canlı kare | `wallet-budgetbakers/f7-49-d3-saved.png` | 4.15 |
| E0398 | Wallet | Canlı kare | `wallet-budgetbakers/48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png` | 4.16 |
| E0405 | Money Manager | Canlı kare | `money-manager/35-toplam-sekmesi-eylul.png` | 3.19, 5.16, 7.12 |
| E0407 | Money Manager | Canlı kare | `money-manager/37-ekim-islemler-taksit-3-6.png` | 3.12 |
| E0409 | Money Manager | Canlı kare | `money-manager/39-aralik-islemler-taksit-5-6.png` | 3.13 |
| E0410 | Money Manager | Canlı kare | `money-manager/40-ocak-islemler-taksit-6-6.png` | 3.14 |
| E0411 | Money Manager | Canlı kare | `money-manager/41-subat-islemler-veri-yok.png` | 3.15 |
| E0412 | Money Manager | Canlı kare | `money-manager/42-kart-defteri-ekim-bakiye-2600.png` | 3.21 |
| E0413 | Money Manager | Canlı kare | `money-manager/43-kart-defteri-ocak-bakiye-5600.png` | 3.22 |
| E0414 | Money Manager | Canlı kare | `money-manager/44-hesaplar-borclar-1600.png` | 3.20 |
| E0416 | Money Manager | Canlı kare | `money-manager/46-ayarlar-tekrarlayan-islemler.png` | 5.11 |
| E0417 | Goodbudget | Canlı kare | `goodbudget/30-oncesi-zarflar.png` | 1.9, 5.17, 6.8 |
| E0418 | Goodbudget | Canlı kare | `goodbudget/31-oncesi-hesap-bakiyesi.png` | 1.11 |
| E0419 | Goodbudget | Canlı kare | `goodbudget/32-gelir-formu-dolu.png` | 1.5 |
| E0420 | Goodbudget | Canlı kare | `goodbudget/33-sonrasi-zarflar.png` | 1.10 |
| E0421 | Goodbudget | Canlı kare | `goodbudget/34-sonrasi-hesap-bakiyesi-degismedi.png` | 1.12 |
| E0423 | Goodbudget | Canlı kare | `goodbudget/36-rapor-income-vs-spending-3284.png` | 7.4 |
| E0425 | Wallet | Canlı kare | `wallet-budgetbakers/f7-80-labels-gorunumu-isletme-sahsi-yalniz-150.png` | 6.5 |
| E0427 | Hesap Defterim | Canlı kare | `hesap-defterim/49-uretilen-pdf-icerigi.png` | 8.1 |
| E0428 | Wallet | Canlı kare | `wallet-budgetbakers/f7-105-disa-aktarma-formu-pdf-xls-csv.png` | 8.4 |
| E0430 | Money Manager | Canlı kare | `money-manager/66-tekrarli-kaydetme-onay-diyalogu.png` | 5.9 |
| E0434 | Money Manager | Canlı kare | `money-manager/50-sifir-tutarli-kayit-kabul-edildi-eylul-1600.png` | 1.15 |
| E0435 | Bluecoins | Canlı kare | `bluecoins/f7-75-transfer-ucreti-kendi-hesabi-ve-kategorisi.png` | 2.6 |
| E0436 | Wallet | Canlı kare | `wallet-budgetbakers/f7-66-aktarim-formu.png` | 2.7 |
| E0445 | Bluecoins | Canlı kare | `bluecoins/f7-71-veri-yonetimi-ice-aktarma-csv-qif.png` | 8.5 |
| E0462 | Money Manager | Canlı kare | `money-manager/57-havale-formu-harc-alani.png` | 2.5 |

## Basılmayan dayanaklar

- E0010 · Money Manager gözlem formu · `gozlemler/money-manager.md`
- E0005 · Bluecoins gözlem formu · `gozlemler/bluecoins.md`
- E0014 · Wallet gözlem formu · `gozlemler/wallet-budgetbakers.md`
- E0007 · Hesap Defterim gözlem formu · `gozlemler/hesap-defterim.md`
- E0006 · Goodbudget gözlem formu · `gozlemler/goodbudget.md`
- E0008 · KolayBi gözlem formu · `gozlemler/kolaybi.md`
- E0011 · Paraşüt gözlem formu · `gozlemler/parasut.md`
- E0009 · Logo İşbaşı gözlem formu · `gozlemler/logo-isbasi.md`
- E0012 · QuickBooks Solopreneur gözlem formu · `gozlemler/quickbooks.md`
- E0375 · Wallet çekmecesinin üstü; hesap sahibinin adını taşıyor
- E0262 · Paraşüt tanıtım videosu karesi; ürünün iç arayüzü değil (→ 9.1)
- E0187 · KolayBi panosu; destek görseli değil, güncel arayüz yakalaması (→ 9.4)
