# Fiş Analizi API Sözleşmesi

Bu belge uygulanmış `POST /api/v1/receipts/analyze` sözleşmesini kaydeder.
Yanıt bir finansal kayıt değil, kullanıcının düzenleyip ayrıca onaylayacağı bir
öneridir. Bu uç nokta veritabanına veya dosya deposuna hiçbir şey yazmaz.

## İstek

```http
POST /api/v1/receipts/analyze
Authorization: Bearer <access-token>
Content-Type: multipart/form-data; boundary=...

file=<JPEG veya PNG byte'ları>
```

- Form alanının adı `file`dır.
- Azami dosya boyutu 5 MiB'dir; multipart zarfı için ayrıca 64 KiB pay vardır.
- Uzantı, bildirilen MIME ve magic byte birbiriyle eşleşmelidir.
- Mevcut `IAttachmentFileInspector` magic byte, MIME, EICAR ve dosya türü
  denetimini yapar. Denetçi PDF kabul edebilse de bu uç nokta ayrıca yalnız
  `image/jpeg` ve `image/png` kabul eder.
- İstek veya query içinde `UserId` yoktur. Kimlik doğrulanmış JWT `sub`
  claim'inden türetilir.

## Yakalama niyeti (`intent`)

İstek `multipart/form-data`'dır ve `file` alanının yanında isteğe bağlı bir
`intent` alanı taşır:

| Değer | Anlamı |
|---|---|
| `expense` | Kullanıcı ödeyen taraftır (varsayılan) |
| `income` | Kullanıcı parayı alan taraftır |
| `transfer` | Para kullanıcının kendi hesapları arasında taşındı |
| `bank_slip` | Yön değil, **belge sınıfı**: "bu bir dekont" |

İlk üçü yön bildirir. **`bank_slip` bildirmez** ve bilerek: bir dekontun ne
olduğu fotoğraf çekilmeden bilinemez — aynı 5.000 TL bir ödeme de olabilir,
borç verme de, kendi hesabına aktarma da, kart borcu ödemesi de. Yön bu yolda
okuma bittikten sonra, tutar ve karşı taraf ekrandayken **karar sayfasında**
sorulur; yine kullanıcıdan gelir (ADR 0011 madde 2 bozulmaz), yalnız daha geç.

Bu niyet 20 Ağustos 2026'da eklendi. Sebebi ölçümle görüldü: gerçek dekontların
çoğu `bank_payment`'tır (gönderen ile alıcı farklı kişiler) ve o tür yalnız
`expense` niyetinde okunuyordu. Kullanıcı dekontu Transfer seçeneğiyle
okutmaya çalıştığında reddediliyor, pratikte dekont **yalnız Harcama**
seçeneğinden geçebiliyordu — üstelik yanlış tahmin belgeyi hiç okutmuyordu.

Alan **boş bırakılırsa `expense`** varsayılır: 12.11 öncesi ürünün varsayımı
buydu ve güncellenmemiş bir istemci hâlâ bunu kastediyor. Tanınmayan bir değer
ise varsayılana düşmez, `400 receipt.unsupported_file` ile reddedilir — istemci
sunucunun anlamadığı bir şey söylüyorsa yönü tahmin etmek, bu özelliğin var olma
sebebine aykırıdır.

Yön modele **söylenir, sorulmaz**. Gerekçesi: bir belgenin yönü belgede yazmaz.
Aynı kira makbuzu kiracı için gider, ev sahibi için gelirdir; cevabı bilmek
belgedeki taraflardan hangisinin kullanıcı olduğunu bilmeyi gerektirir — okuma
değil kimlik sorusudur. Model yönü yalnız **hangi tarafı `counterpartyName`
olarak okuyacağını** bilmek için kullanır.

Yön ayrıca kategori kümesini belirler: `expense` → kullanıcının aktif **gider**
kovaları, `income` → aktif **gelir** kovaları.

`transfer` niyetinde kategori **hiç gönderilmez**: para harcanmadı, yer
değiştirdi. Bu niyette `feeAmount` alanı anlamlıdır — dekonttaki işlem ücreti
taşınan tutardan **ayrı** okunur ve ona asla eklenmez. `5.000` ile `4,50`
birleştirildiğinde hiç harcanmamış bir `5.004,50` gideri üretilmişti; transfer
parayı taşır, harcanan yalnız bu ücrettir.

## Fatura, iade ve taksit alanları

Üç alan 20 Ağustos 2026'da eklendi; üçü de istemcinin **bir sonraki adımını**
belirliyor.

| Alan | Ne zaman dolu | İstemci ne yapar |
|---|---|---|
| `dueDate` / `dueDateState` | Belgede "SON ÖDEME TARİHİ" / "VADE" yazıyorsa | "Bu faturayı ödediniz mi?" diye sorar |
| `installmentCount` | Fişte taksit yazıyorsa (2–360) | Tek seferlik gider yerine taksit planı önerir |
| `refundMatch` | `documentType` = `refund_receipt` ve eşleşen gider bulunduysa | "Bu harcamanızı iptal edeyim mi?" diye sorar |

**`dueDate` belgenin kendi tarihinin yerine geçmez.** İkisi ayrı satırlardır ve
karıştırılırsa henüz ödenmemiş bir fatura düzenlendiği gün harcanmış görünür.
Vade tarihinin doğrulaması da farklıdır: bir fiş yarından olamaz ama bir
faturanın vadesi tam da gelecektedir, geçmiş vade de meşrudur (gecikmiş fatura
hâlâ ödenmemiştir).

**`installmentCount` sayıdır, para değil.** Prompt `totalAmount`'ı taksit
sayısına bölmeyi açıkça yasaklar; bölme uygulamanın işidir. "1 taksit" tek
çekimdir ve `null` döner — anlaşılmayan bir değer de `null` döner, sessizce 1'e
düşmez.

**`refundMatch` bir öneridir, eylem değil.** Uç nokta hiçbir şey yazmaz: aday
kullanıcıya gösterilir, kullanıcı tarihi, tutarı ve adı görüp onaylar. Kısmi
iadede `remainingAmount` harcamanın ne olması gerektiğini taşır ve **sunucuda**
hesaplanır — istemci finansal toplamı ikinci kez hesaplamaz. Tam iadede `null`
ve iptal tek başına yeterlidir.

```json
"refundMatch": {
  "transactionId": "…",
  "transactionDate": "2026-08-10",
  "amount": "847.5000",
  "description": "Sentetik Market",
  "remainingAmount": "647.5000"
}
```

Eşleşme ölçütü: aynı ad, iptal edilmemiş gider, iade tarihinden geriye 180 gün,
tutar iadeden büyük ya da eşit; sıralama önce tam tutar sonra en yeni. Çift
kayıt kontrolünden **bilerek daha gevşek**: o uyarı kendiliğinden çıkar ve
yanlış alarm sonraki bütün uyarıların güvenini harcar; iade adayı ise
gösterilip onaylanır.

## Belge türü ile yönün tutarlılığı

| `documentType` | `expense` | `income` |
|---|---|---|
| `purchase_receipt` (tezgâh üstü satın alma) | ✅ | ❌ `receipt.intent_mismatch` |
| `invoice_or_voucher` (iki tarafı adıyla yazan fatura/makbuz/bordro) | ✅ | ✅ |
| `refund_receipt` | ✅ okunur, `refundMatch` taşır | ❌ `receipt.refund_document` |
| `bank_document` (kendi hesaplar arası aktarma) | ❌ `receipt.bank_document` | ❌ aynı |
| `bank_payment` (üçüncü tarafa ödeme) | ✅ | ❌ `receipt.intent_mismatch` |
| `bank_card_payment` (kart borcu ödemesi) | ✅ | ❌ `receipt.intent_mismatch` |
| diğer / tanınmayan | ❌ `receipt.not_a_receipt` | ❌ aynı |

**Banka belgesi iki ayrı türdür** ve ikisi ekonomik olarak birbirinin zıddıdır:
`bank_document` parayı kullanıcının kendi hesapları arasında taşır (ATM çekimi,
virman, gönderenle alıcının aynı olduğu havale); `bank_payment` ise başkasına
ödeme yapar (farklı adlı alıcıya havale/EFT, kurum faturası). İkincisinde para
geri gelmeyecek şekilde çıkmıştır — yani harcamadır. Ayrım belgede **yazan
adlardan** okunur, kullanıcının kim olduğundan değil: iki ad farklıysa
`bank_payment`.

`bank_card_payment` ayrı bir tür çünkü kart ödemesi **gider üretmemeli**:
harcamalar kart harcaması olarak zaten sayıldı, ödeme onların ikinci kez
sayılması olurdu. Tür kendiliğinden hiçbir şeyi reddetmez — kararı kullanıcı
verir — ama uygulamanın doğru cevabı başa alıp ne gördüğünü söylemesini sağlar.

`transfer` niyeti bu tabloya üçüncü bir sütun ekler: yalnız `bank_document`
kabul edilir. `bank_payment` bu niyette `receipt.bank_payment_not_transfer` ile
reddedilir ("başkasına yapılan bir ödeme, Harcama seçeneğiyle okutun"), diğer
bütün türler `receipt.not_a_transfer_document` ile. `bank_document` gider olarak
okunsaydı hiç harcanmamış bir gider yazılır, üstelik o parayla yapılan alışveriş
de fişlenince aynı para iki kez sayılırdı.

`bank_slip` niyeti dördüncü sütundur ve **üç banka türünü birden** kabul eder:

| `documentType` | `bank_slip` |
|---|---|
| `bank_document` | ✅ |
| `bank_payment` | ✅ |
| `bank_card_payment` | ✅ |
| `purchase_receipt`, `invoice_or_voucher` | ❌ `receipt.not_a_bank_slip` |
| `refund_receipt` | ❌ `receipt.refund_document` |
| diğer / tanınmayan | ❌ `receipt.not_a_receipt` |

Bu sütunda `receipt.bank_payment_not_transfer` ve `receipt.intent_mismatch`
**hiç oluşmaz**: yön bildirilmediği için çelişecek bir şey yok. Alışveriş
belgesi yine reddedilir, çünkü bu yolun sonunda "bu tutar ne?" sorusu var ve o
soru bir market fişi için anlamsızdır.

Belgenin kendisiyle ilgili retler (`refund_receipt`, tanınmayan belge) niyetten
bağımsızdır ve niyet kollarından **önce** değerlendirilir: iade fişi hangi
seçenekle okutulursa okutulsun iade fişidir ve gerekçesi de öyle raporlanır.

Yazar kasa fişini elinde tutan taraf daima alıcıdır, bu yüzden onu gelir olarak
çekmek bir çelişkidir ve **sessizce dönüştürülmez**, kullanıcıya söylenir.
`invoice_or_voucher` bilerek tek türdür: kâğıt hangi tarafın kullanıcı olduğunu
söyleyemez, ikiye bölmek modelden tam da yapmaması gereken çıkarımı istemek
olurdu.

## Başarılı yanıt

```json
{
  "documentKind": "purchase_receipt",
  "counterpartyName": "Sentetik Market",
  "counterpartyState": "read",
  "purchasedAt": "2026-08-18",
  "purchasedAtState": "read",
  "totalAmount": "847.5000",
  "totalAmountState": "suspect",
  "currencyCode": "TRY",
  "paymentHint": "card",
  "categoryId": "11111111-1111-1111-1111-111111111111",
  "categoryName": "Market Alışverişi",
  "categoryState": "read",
  "warnings": [
    {
      "code": "receipt.totals_do_not_add_up",
      "message": "Ara toplam ve KDV, genel toplamı tutmuyor. Tutarı kontrol edin."
    }
  ]
}
```

`documentKind` okunan belgenin türüdür: `purchase_receipt`,
`invoice_or_voucher`, `bank_document`, `bank_payment` veya
`bank_card_payment`. Yalnız kabul edilen
türler görünür; gerisi kapıda reddedilir. Alan sözleşmede duruyor çünkü
istemcinin bir sonraki adımı buna bağlı: banka belgesinde ana tutarın ne olduğu
belgeden okunamaz (ödeme mi, kendi hesabına aktarma mı, kart ödemesi mi, borç
verme mi) ve kullanıcıya sorulur; alışveriş belgesinde böyle bir soru yoktur.

`counterpartyName` **karşı taraftır**, "satıcı" değil: işlemin öteki ucundaki
kişi veya kurum. Bir belgenin yönü belgede yazmaz, bakan tarafa göre değişir —
aynı kira makbuzu kiracı için gider, ev sahibi için gelirdir. "Satıcı" adı gelir
belgesinde kullanıcının kendisini gösterir ve kayıt listesinde işe yaramazdı.
Bugün üretilen taslak daima gider olduğu için bu alan pratikte işletme adını
taşır; alan adı ileriki yönleri de karşılayacak biçimde seçilmiştir.

### Uyarılar

`warnings`, taslağı reddetmeyen ama kullanıcının görmesi gereken notlardır.
Bunlardan biri kaydın kendisiyle ilgilidir:

| Kod | Anlamı |
|---|---|
| `receipt.possible_duplicate` | Aynı gün, aynı tutar ve aynı karşı taraf adıyla **iptal edilmemiş** bir kayıt zaten var |

Bu bir **uyarıdır, engel değil**: aynı gün iki özdeş alışveriş olağandır ve
engellemek, önlediği çift kayıttan daha sık yanlış olurdu. Eşleşme bilerek
katıdır (aynı gün + aynı tutar + aynı ad); güvenilmeyen bir uyarı sonraki
bütün uyarıların bedelini öder. Eşleşme daima **sahibine kapsamlıdır** —
başka kullanıcının aynı kaydı bu kullanıcının kopyası değildir ve varlığı
sızdırılmaz.

**Hangi kayıtlar aranıyor, niyete bağlıdır.** Bir gelir belgesi bir gideri
kopyalayamaz; ikisi de bir transferi kopyalayamaz.

| `captureIntent` | Aranan kayıtlar |
|---|---|
| `expense`, `income` | Gelir/gider hareketleri |
| `transfer` | Transferler |
| `bank_slip` | Hareketler, transferler, kart ödemeleri ve alacaklar — dördü |

Yalnız dekont dördüne birden bakıyor çünkü okuma anında hangisi olacağı henüz
belli değil: o soru karar sayfasında, bu çağrı döndükten sonra soruluyor.
Mesaj bulunduğu rafı **adıyla** söyler (`aktarma`, `kart ödemesi`,
`alacak kaydı`); türü söylemeyen bir uyarı kullanıcıyı kart ödemesini gider
listesinde aratır ve uyarıyı yanlış sandırır.

Alan durumları:

| Değer | Anlam |
|---|---|
| `read` | Alan okundu ve bütün kontrolleri geçti |
| `suspect` | Alan okundu fakat çapraz kontrol veya para birimi kontrolü şüpheli |
| `missing` | Alan okunamadı ya da reddedildi; değer `null`, tahmin yok |

`paymentHint` kapalı bir kümedir:

| Değer | Anlamı | Flutter ne yapar |
|---|---|---|
| `cash` | Fişte "NAKİT" yazıyor | Hesaplar önde, "Fişte nakit yazıyor" notu |
| `credit_card` | Fişte "KREDİ KARTI" yazıyor | Kredi kartları öne alınır |
| `debit_card` | Fişte "BANKA KARTI" / "DEBİT" yazıyor | Hesaplar önde kalır |
| `card` | Kartla ödenmiş ama **türü yazmıyor** | Sıra değişmez, not türün bilinmediğini söyler |
| `unknown` | Ödeme biçimi okunamadı | Not yok, sıra değişmez |

Kredi ve banka kartı ayrı değerlerdir çünkü bu uygulamada ayrı yazma
modelleridir: kredi kartı ödemesi `CreditCardCharge`, banka kartı ödemesi ise
bir `Account` üzerinde normal giderdir (kullanıcı bunun için "banka kartı" adlı
bir hesap açar). İkisi tek `card` değerinde birleştiğinde her kart fişi kredi
kartlarına yönlendiriliyordu.

`card` değeri korunuyor çünkü çoğu yazar kasa fişi yalnız "KART" veya "POS"
basar, bazı ÖKC cihazları banka kartı ödemesine de "KREDİ KARTI" yazar. Fiş
türü söylemiyorsa sözleşme de söylemez — kart markası (Visa, Mastercard, Troy)
tür kanıtı değildir ve modele bundan çıkarım yapması yasaklanmıştır.

Bu alan hiçbir durumda hesap veya kart **seçmez**; yalnız kaynak listesini
sıralar ve ekranda bir not olarak görünür. Para, diğer finans sözleşmelerinde
olduğu gibi dört ondalıklı JSON string'dir.

## Belge türü kapısı

Uç nokta önce **ne olduğunu** sorar, sonra okur. Model yanıtı zorunlu bir
`documentType` alanı taşır: `purchase_receipt`, `refund_receipt`,
`bank_document`, `other_document`, `not_a_document`. Yalnız `purchase_receipt`
taslağa dönüşür.

Kapı iddiaya değil reddetmeye çalışır: **tanınmayan veya eksik bir değer
`purchase_receipt` sayılmaz**, reddedilir. Aksi hâlde sağlayıcı tarafında bir
alan adı değişikliği, kapatılan deliği sessizce yeniden açardı.

**Neden dekont reddediliyor.** Havale/EFT gider değildir; kullanıcının kendi
hesapları arasında para taşır ve gider olan tek kalem işlem ücretidir. Dekont
"fiş gibi" okunduğunda hem hiç yapılmamış bir harcama yazılır, hem gerçek
harcama (ücret) o tutarın içinde kaybolur. Yani iyi okunmuş bir dekont bile
yanlış kayıt üretir — sorun okuma kalitesi değil, belge türüdür.

**Neden iade reddediliyor.** İade fişinde para geri gelmiştir. Fiş gibi
okunduğunda tutar aynı, **işaret ters** olur: kullanıcı iade ettiği alışverişin
bedelini ikinci kez harcamış görünür. 19 Ağustos 2026 saha koşumunda gerçek bir
eczane iadesi 320 TL'lik sağlık gideri olarak önerildi.

**Hiçbir alanın okunamaması bir hatadır**, yarım okunmuş fiş değil. Bu durumda
`200` + boş taslak yerine `400 receipt.unreadable` döner; eskiden alan başına
bir uyarı üretiliyor ve kullanıcı aynı şeyi söyleyen beş kutu görüyordu. Bir
alan okunup diğerleri okunamadıysa taslak yine döner: eksik alanı kullanıcı
doldurur.

## Sahiplik ve yan etki sınırı

`AnalyzeReceiptUseCase`, yalnız current user'ın aktif gider kategorilerini
`ICategoryRepository.ListAsync(userId, Expense, true)` ile okur. Modele giden
kapalı kategori listesi de, dönen adın `categoryId`ye çözülmesi de aynı listeyi
kullanır. Başka kullanıcının kategorisi prompt'a girmez ve taslağa çözülemez.

Use case'in transaction repository, attachment store veya unit-of-work
bağımlılığı yoktur. Başarılı ya da başarısız analiz:

- `BudgetTransaction` veya `CreditCardCharge` oluşturmaz,
- fotoğrafı ya da model yanıtını saklamaz,
- sağlayıcı isteğinde `store=false` göndererek etkileşim geçmişini kapatır,
- attachment yazmaz,
- taslak tablosu oluşturmaz.

Finansal kayıt ancak sonraki onay akışında mevcut transaction/card-charge
endpoint'lerinden yazılır.

## Hatalar

Hatalar standart ProblemDetails gövdesinde `code` uzantısıyla döner.

| HTTP | Kod | Anlam |
|---:|---|---|
| 400 | `receipt.unsupported_file` | Boş, türü/uzantısı/imzası uyuşmayan veya JPEG/PNG olmayan dosya |
| 400 | `receipt.unreadable` | Görsel ya da sağlayıcı yanıtı güvenilir taslağa çevrilemedi; **hiçbir alan okunamadıysa** da bu döner |
| 400 | `receipt.bank_document` | Fotoğraf banka dekontu/havale-EFT makbuzu; okunmaz |
| 400 | `receipt.refund_document` | Fotoğraf iade fişi/POS iade slipi; para geri gelmiştir, gider değildir |
| 400 | `receipt.not_a_receipt` | Fotoğrafta alışveriş fişi yok (başka belge ya da belge değil) |
| 401 | authentication challenge | Bearer token yok/geçersiz |
| 413 | `receipt.file_too_large` | Fotoğraf 5 MiB sınırını aştı |
| 429 | `rate_limit.exceeded` | Yerel kullanıcı kotası: kullanıcı başına dakikada 5 analiz |
| 429 | `receipt.provider_rate_limited` | Dış sağlayıcı kotası doldu |
| 503 | `receipt.provider_unavailable` | Dış sağlayıcıya ulaşılamadı |
| 503 | `receipt.disabled` | Sunucuda Gemini anahtarı yapılandırılmadı |

Yerel rate limit JWT `sub` değerine göre partition edilir. Bir kullanıcının
kotayı doldurması diğer kullanıcının kovasını tüketmez.

## Gizlilik

Fotoğraf Google Gemini güven sınırını geçer. API anahtarı yalnız sunucudadır ve
`x-goog-api-key` header'ında taşınır; URL'ye, response'a, loga veya APK'ya
girmez. Aşama 12.10 geliştirmesinde yalnız sentetik fiş kullanılabilir. Gerçek
fiş, ücretli katman ve Stage 13 güvenlik kapısı tamamlanmadan gönderilmez.
