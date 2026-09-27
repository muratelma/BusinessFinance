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
