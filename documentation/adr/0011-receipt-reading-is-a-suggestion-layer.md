# ADR 0011 — Fiş okuma bir öneri katmanıdır, karar verici değildir

- Durum: Kabul edildi (19 Ağustos 2026, Aşama 12.10)
- Bağlam: Aşama 12.10 belgesi — önceki repoda (`Kisisel-Butce-Mobil`), bu repoya taşınmadı
- İlgili: ADR 0001 (HTTP sözleşmeleri), ADR 0002 (transfer raporlaması),
  ADR 0004 (birleşik okuma modeli)
- Ölçüm kayıtları: `documentation/receipt-measurement.md` (sentetik set),
  2026-08-19 saha koşumu (gerçek fişler; kaydı önceki repoda)

## Bağlam

Uygulamaya bir fotoğraftan gider yazma yeteneği eklendi: kullanıcı fişi
çekiyor, sunucu görüntüyü Gemini'ye okutuyor, dönen alanlar bir taslağa
çevriliyor. Bu, ürüne giren **ilk olasılıklı bileşen**. Diğer her şey
belirlenimci: aynı girdi aynı kaydı üretir, hata varsa koddadır ve testle
yakalanır. Bir dil modeli bu güvenceyi vermez — aynı fotoğrafa iki kez farklı
cevap verebilir, emin görünerek yanılabilir ve yanıldığını söylemez.

Bir finans defterinde bunun bedeli ağırdır, çünkü hatanın ucuz ve pahalı
biçimleri aynı görünür: yanlış okunan tutar %10 sapmadır, **yanlış yön %200
sapmadır** — raporun işareti döner. Sentetik fiş seti boru hattının çalıştığını
gösterdi ama bunu gösteremezdi; gerçek fişlerle yapılan ilk saha koşumu dört
kusur buldu ve dördü de sentetik sette görünmüyordu.

Bu ADR, o koşumlardan çıkan ve geri alınması pahalı olan kararları sabitler.

## Karar

### 1. Model önerir, kullanıcı karar verir, sunucu doğrular

Analiz uç noktası **hiçbir finansal kayıt yazmaz**; `ReceiptDraft` döner ve
taslak hiçbir yerde saklanmaz. Gider ancak kullanıcı formu onayladığında,
mevcut yazma uç noktalarından geçerek oluşur. Bu, "yeni yazma kodu yok"
kuralının sonucudur: fiş akışı yeni bir para yolu açmaz, var olan yolu besler.

Otomatik yazma reddedildi. Doğruluk %100 olmadığı sürece otomatik kayıt,
kullanıcının hiç görmediği bir hatayı deftere yazar; hatanın maliyeti onu
düzeltmenin maliyetinden yüksektir.

### 2. Yön belgeden okunmaz, kullanıcıdan gelir

> **20 Ağustos 2026 güncellemesi (Aşama 12.11).** Maddenin gerekçesi aynen
> geçerli; **sonucu** değişti. "Yön üründe sabittir, kullanıcı daima alıcıdır"
> kararı kalktı: yön artık kullanıcının **yakalama anındaki niyetinden**
> geliyor — maddenin kendisinin öngördüğü çözüm. Aşağıdaki metin özgün
> gerekçeyi korur; sonundaki karar paragrafı güncellenmiştir.

Bir fiş iki taraf arasındaki olayın kaydıdır ve **gider mi gelir mi olduğu
belgeye hangi taraftan bakıldığına bağlıdır.** Aynı kira makbuzu kiracı için
gider, ev sahibi için gelirdir; kâğıt birebir aynıdır.

Bu yüzden yön bir okuma problemi değil, **kimlik problemidir**: cevaplamak için
belgedeki taraflardan hangisinin kullanıcı olduğu bilinmelidir. Kullanıcı
kimliği modele gönderilmez (madde 5), gönderilse bile bu çıkarım kırılgan
olurdu.

Bu kararın sözleşmedeki izi `counterpartyName` alan adıdır (Aşama 12.11, Grup 3):
"satıcı" adı yönü varsayar ve gelir belgesinde kullanıcının kendisini gösterirdi;
**karşı taraf**, yön ne olursa olsun işlemin öteki ucudur.

**Karar (12.11 sonrası):** yön modelden değil **kullanıcıdan** gelir ve
fotoğraftan önce sorulur — `Harcama / Gelir / Transfer`. Model yönü yalnız
hangi tarafı `counterpartyName` olarak okuyacağını bilmek için kullanır; belge
türü kapısı niyeti *doğrular*, üretmez (madde 3). Çelişki sessizce
dönüştürülmez, kullanıcıya söylenir.

**Bir istisna, aynı gerekçenin devamı olarak:** dekontta yön fotoğraftan önce
de bilinemez. Aynı 5.000 TL bir ödeme de olabilir, borç verme de, kendi
hesabına aktarma da, kart borcu ödemesi de — ve kullanıcı fotoğrafı çekmeden
belgede ne yazdığını bilmiyor. Bu yüzden dördüncü bir yakalama seçeneği var
(`Dekont` / `intent=bank_slip`) ve o **yön değil belge sınıfı** bildirir; yön,
okuma bittikten sonra tutar ve karşı taraf ekrandayken karar sayfasında
sorulur. Yön yine kullanıcıdan gelir, yalnız daha geç.

İade fişi de artık reddedilmiyor (Grup 9): okunuyor, geri verdiği harcama
aranıyor ve **kullanıcının onayıyla** o harcama iptal ediliyor. Yönü burada da
model belirlemiyor — iade fişinin yönü zaten belgede yazar ("İADE") ve sonucu
yeni bir kayıt değil, eskisinin iptalidir.

### 3. Belge türü kapısı iddiaya değil reddetmeye çalışır

Model önce fotoğrafın ne olduğunu söyler (`documentType`), sonra alanlar
okunur. Prompt fotoğrafın fiş olduğunu **iddia etmez**: iddia eden bir prompt
modele katılmama imkânı bırakmıyordu — havale dekontu verildiğinde uyum sağlayıp
dekontu fiş alanlarına eşliyordu.

Aşağıdaki tablo 12.10'daki hâlidir; 12.11 türleri çoğalttı ve kapıyı
`(belge türü × yakalama niyeti)` matrisine çevirdi. Güncel matris
`documentation/receipt-analysis-api-contract.md` içindedir.

| Tür | 12.10'daki sonuç | 12.11 sonrası |
|---|---|---|
| `purchase_receipt` | Taslağa dönüşür | Aynı (yalnız gider niyetinde) |
| `refund_receipt` | `receipt.refund_document` | **Okunur**; geri verdiği harcama aranır (Grup 9) |
| `bank_document` | `receipt.bank_document` | Transfer ve `bank_slip` niyetlerinde okunur |
| `other_document`, `not_a_document`, **tanınmayan/eksik değer** | `receipt.not_a_receipt` | Aynı |

Değişmeyen şey kapının **yönü**: model yalnız reddetmek için dinlenir, kabul
etmek için değil. Tanınmayan bir değer hâlâ reddeden tarafa düşer.

**Tanınmayan değer `purchase_receipt` sayılmaz.** Varsayılanın kabul tarafında
olması, sağlayıcı tarafındaki bir değişikliğin bu kapıyı sessizce açması
demekti.

Reddedilen türler *desteklenmiyor*, yanlış sayılmıyor: ATM makbuzu bir
transferdir ve gider yazılsaydı o parayla yapılan alışverişle birlikte aynı para
iki kez sayılırdı. Kredi kartı ekstresi ise onlarca harcamanın özetidir.

### 4. Kategori kapalı bir kümeden seçilir; kaynak hiç seçilmez

Modele kategori **kimlikleri** değil **adları** gönderilir ve adlar prompt
metnine değil **JSON şemasının `enum`'una** konur. Kısıt böylece ricadan çıkıp
yapısal hâle gelir: liste dışında bir değer üretmek mümkün değildir. "Bilmiyorum"
da kümenin bir üyesidir (`__bilinmiyor__`), serbest metne kaçış yoktur. Dönen ad,
sunucuda yine kullanıcının kendi listesiyle eşleştirilerek kimliğe çevrilir.

Kategori, fişten **çıkarım yapılan tek alandır** ve prompt'ta kendi maddesinde
yaşar. Transkripsiyon maddesinin altındayken o maddenin "ASLA tahmin etme"
kuralıyla çelişiyordu: kategori fişte yazmaz. Marka adı kanıt taşımayan bir
fişte model çelişkiyi güvenli tarafa çözüp altı koşumun altısında
`__bilinmiyor__` dedi. Kendi maddesi kanıt sırasını verir (önce ürün kalemleri,
sonra işletme adı ve iş türü ibareleri) ve kova adı **hardcode etmez** — liste
kullanıcıya göre değişir.

Eşik "en uygun" değil **"yalnız açıkça uyan"**dır. "En uygun" kuralı döner
alışverişini market kovasına yazıyordu; kısmen uyan kategori sessiz bir hatadır,
rapor bozulur ve kimse fark etmez.

**Ödeme kaynağı hiçbir koşulda modelce seçilmez.** Fiş hangi karttan veya
hesaptan ödendiğini söylemez; üstelik kullanıcı fişte yazanın aksine ödemiş
olabilir. `paymentHint` yalnız kaynak listesini sıralar ve ekranda not olarak
görünür. İpucu kart *türünü* ayırır (`credit_card` / `debit_card`) çünkü ikisi
ayrı yazma modelidir — banka kartı bir `Account`, kredi kartı bir
`CreditCardCharge` üretir. Tür yazmayan fiş için `card` değeri korunur: çoğu
yazar kasa fişi yalnız "KART" basar, bazı ÖKC cihazları banka kartı ödemesine de
"KREDİ KARTI" yazar ve kart markası (Visa, Mastercard, Troy) tür kanıtı
değildir.

### 5. Modele giden veri asgaridir ve saklanmaz

İstek gövdesinde yalnız üç şey vardır: sabit prompt, fotoğraf, ve şemadaki
kategori adları. Gitmeyenler: kullanıcı kimliği, e-posta, kategori/hesap
kimlikleri, hesap ve kart adları, tutar geçmişi. Kategori adları da dardır —
yalnız **aktif** ve **gider** tipindeki kovalar; gelir kovaları hiç gönderilmez.

`store: false` gönderilir: sağlayıcı etkileşimi saklamaz. Fotoğraf sunucuda
kalıcılaşmaz; yalnız kullanıcı açıkça isterse, **orijinal** byte'lar mevcut
belge uç noktasına gider (küçültülmüş analiz kopyası değil — belge kanıttır).

Fotoğrafın Google'a gideceği ilk okumadan **önce** anlatılır ve rıza alınmadan
çağrı yapılmaz; rıza kapısı ekranda değil controller'da olduğu için ekran
atlansa bile çağrı çıkmaz.

### 6. Model aritmetik yapmaz

Hesaplanmış bir toplam, okunmuş bir toplamla birebir aynı görünür. Dekontta
`5.000` ve `4,50` satırları birleştirilip hiç harcanmamış bir `5.004,50`
gideri üretildi. Prompt toplama/çıkarma/birleştirmeyi yasaklar; `totalAmount`
yalnız fişte **tek bir satırda basılı** olan genel toplamdır, öyle bir satır
yoksa alan boş kalır.

### 7. Alan güveni üç durumludur, olasılık skoru yoktur

Her alan `read` / `suspect` / `missing` taşır. `suspect`, "okundu ama çapraz
kontrol tutmadı" demektir ve değeri **saklar**: 2016 tarihli gerçek bir fişin
tarihi "çok eski" diye atılıyor, kullanıcıya doğru okunmuş veriyi yeniden
yazdırıyordu. Gelecek tarih hâlâ atılır — olmamış bir aya gider yazılamaz.

Modelin kendi ürettiği bir güven skoru (`confidence: 0.94`) reddedildi:
kalibre değildir, ölçülebilir bir şeye karşılık gelmez ama arayüzde ölçülmüş
gibi görünür. Üç durum ise modelin beyanına değil **doğrulanabilir çapraz
kontrollere** dayanır.

## Sonuçlar

**Kazanılan:** olasılıklı bir bileşen, belirlenimci bir deftere yanlış kayıt
yazamadan bağlandı. Yanlış okuma en kötü ihtimalle kullanıcının düzelttiği bir
öneridir; sessiz bir defter hatası değildir.

**Ödenen bedel:** gelir, transfer, iade ve taksit belgeleri bugün yalnız
reddediliyor. Her biri kendi işini hak ediyor ve Aşama 12.11'in konusudur.
Ayrıca reddetme dürüst ama yardımcı değil: kullanıcı "neden okumadı" sorusunun
cevabını hata mesajından alıyor, bir yol öneriden almıyor.

**Bilinen açık:** aynı fişin iki kez okutulması bugün iki gider üretir. İçe
aktarma tarafındaki `FindDuplicateAsync` deseninin karşılığı fiş yolunda yok.

**Sağlayıcıya bağımlılık:** SDK kullanılmıyor, tek uç noktaya ham `HttpClient`
ile gidiliyor. Sözleşme değişikliği derleme hatası değil, **testte kırılma**
olarak görünür; yanıt bu yüzden savunmacı ayrıştırılır ve skip'li canlı
sözleşme testi bu yüzden vardır.

## Reddedilen alternatifler

**Kategori/hesap kimliklerini modele gönderip sonradan doğrulamak.** Yaygın
öneri budur ve riski *sonradan* kapatır. Kimlik hiç göndermemek riski *baştan*
kapatır ve kullanıcının hesap envanterini üçüncü tarafa göndermekten de kurtarır.

**Ödeme yönteminden hesabı otomatik eşlemek** ("nakit ise Nakit hesabı"). Fiş
hangi hesabın kullanıldığını söylemez; yanlış kaynak, kart harcamasını hesap
giderine çevirir ve çifte sayım üretir.

**Kart son dört hanesinden kartı bulmak.** Uygulamada kart numarasına dair
hiçbir alan yoktur; eklemek yeni bir PII alanını domain'e, SQL'e ve backup
şemasına sokar. Kazanç tek bir seçici dokunuşudur.

**Cihaz üstü / çevrimdışı OCR.** Aşama kapsamının dışında bırakıldı, ölçülerek
elenmedi. Gerekçe ölçüm değil kapsam: klasik OCR karakteri şekil olarak eşler ve
yalnız metin verir; bu akışın ihtiyacı olan kategori çıkarımı ile belge türü
ayrımı zaten bir dil modeli gerektiriyor. İleride yeniden değerlendirilirse
karşılaştırma ölçülmelidir.
