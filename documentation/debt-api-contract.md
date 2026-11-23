# Borç ve alacak API sözleşmesi

ADR 0021 dalga 2'nin sunucu sözleşmesi (16 Ekim 2026). Yükümlülük ve borç
planı tek borçtur; borç kendi kapanışlarıyla (ödeme, kartla ödeme, indirim)
kapanır. Bütün uçlar kimlik doğrulaması ister; kullanıcı kimliği yalnız
token'dan okunur. Başka kullanıcının kaydı ile olmayan kayıt aynı `404`
cevabını alır. Para dört ondalıklı **metin**dir, gün `yyyy-MM-dd`'dir.

**Kalkan uçlar (23 Kasım 2026, ADR 0022 Faz 8):** `/api/v1/obligations`
(liste, açma, kapanış, iptal) ve `POST
/api/v1/debts/{id}/installments/{n}/pay`. Tek ödemeli kayıt `POST
/api/v1/debts` (`planKind: single`) ile açılır, `/payments` ile ödenir,
`DELETE /api/v1/debts/{id}` ile iptal edilir; iptalin kilitleri
`debt.day_close_counted` ve `debt.deposit_locked`'dur. Kişi cevabındaki
`openReceivableObligations` / `openPayableObligations` ve profildeki
`hasCounterpartyLedger` da kalktı. `GET /api/v1/debts` ve
`/api/v1/debts/{id}/opening` yerinde (istemci kullanmıyor).

## Borç

### `POST /api/v1/debts` — borç açma

Eski istek (taksitli borç, karşı taraf adıyla) aynen geçerlidir. Eklenen
alanların hepsi isteğe bağlıdır:

| Alan | Anlam |
|---|---|
| `planKind` | `installments` (varsayılan) · `single` (tek ödeme, son gün `firstDueDate`) · `none` (plan yok; `firstDueDate` isteğe bağlı) |
| `counterpartyId` | Kişi kimlikle seçilir. `counterpartyName` ile birlikte gönderilmez; ikisi de boşsa borç kişisizdir ve `description` ister (`debt.description_required`) |
| `isCarriedOver` | Uygulamadan önceki borç (ADR 0021 İ5): hesap ve kategori boştur, para hareketi ve gelir/gider yazılmaz; `principal` "itibarıyla" kalandır |
| `upfrontPayment` | `{ amount, accountId? , creditCardId? }` — açılışta peşin ödenen kısım; yeri tam olarak biridir |
| `clientRequestId` | Aynı kimlikle tekrarlanan istek ikinci borç yazmaz |
| `installmentAmount` | K17 (23 Kasım 2026): bankanın taksit tutarı; toplam geri ödeme `installmentAmount × installmentCount` olarak sunucuda kurulur (oran ondan türetilir). `totalRepayment` ya da `annualInterestRate` ile birlikte gönderilmez (`debt.invalid_contract`) |

### `POST /api/v1/debts/schedule-preview` — takvim önizlemesi

`{ "firstDueDate", "installmentCount", "amount"? , "installmentAmount"? }` —
tutar ya toplam (`amount`) ya taksit tutarıdır, tam olarak biri. Kaydetmeden
açılacak planın taksitlerini döner: `{ "totalRepayment", "installments": [
{ "sequence", "dueDate", "amount" } ] }`. Plan borç açılışıyla **aynı
kuraldan** kurulur (`Debt.PlanAmounts`: toplam eşit bölünür, yuvarlama farkı
son taksitte, aylık). Taksit sayısı 1–360. Yeni kayıt formunun `Taksitli`
bloğu okur.

`sourceType`: `cash` (borç para; `openingAccountId` ister), `expense` /
`income` (ödenecek alım ya da tahsil edilecek satış; `categoryId` ister).
Faizli borç (`totalRepayment > principal`) plansız olamaz.

### `GET /api/v1/debts/{id}?asOfDate=` — borcun ayrıntısı

`DebtResponse` (eski alanlar korunur, eklenenler):

- `planKind`, `isCarriedOver`, `isCancelled`
- `counterpartyId`, `firstDueDate` boş olabilir; `counterpartyName` kişisizde boş metindir
- `installments[]` yalnız **geçerli plandaki** kalemler; her kalemde `paidAmount`
  ve `openAmount`. `status`: `paid` · `open` · `overdue`
- `closings[]`: her kapanış — `kind` (`payment` · `discount`), `amount`,
  `date`, `recordedAtUtc`, `accountId` / `creditCardId` / `posSettlementId`
  (hangisi doluysa paranın yeri odur; indirimde hepsi boş), `interestAmount`,
  `isCancelled`, `cancelledAtUtc`
- `remainingAmount`: geçerli plandaki açık tutar; `isClosed` ondan türer
- `remainingPrincipal` (23 Kasım 2026): kalan anapara — açık kalemlerin
  planlı anapara payı (`principalPortion`). Yeni bir faiz hesabı değildir,
  planın taşıdığı payın okunmasıdır; faizsiz borçta `remainingAmount`a
  eşittir. Kayıt sayfası faizli kayıtta `Kalan anapara` satırını bundan
  çizer; istemci taksitlerden toplamaz

### `POST /api/v1/debts/{id}/payments` — ödeme ya da tahsilat

```json
{ "amount": "500.0000", "paymentDate": "2026-10-28", "asOfDate": "2026-10-28",
  "accountId": "…", "creditCardId": null, "card": null, "clientRequestId": "…" }
```

Yer tam olarak biridir: `accountId`, `creditCardId` (yalnız borçta; kart borcu
artar, gider yazılmaz) ya da `card` (POS; yalnız alacakta, para yola çıkar).
Ödeme en eski açık kalemden başlar ve kalanı aşamaz. Faizli borçta yalnız tam
taksitler ödenir (`debt.interest_bearing`).

### `DELETE /api/v1/debts/{id}/payments/{paymentId}?asOfDate=` — geri alma

Ödeme ya da indirim geri alınır; yalnız kendi kapattığını açar. Eski plana
yapılmışsa tutar geçerli planın ilk açık kalemine eklenir. İdempotenttir.

### `POST /api/v1/debts/{id}/discounts` — indirim

`{ "amount", "discountDate", "asOfDate", "clientRequestId"? }`. Yalnız kayıtlı
alım ya da satışta (`expense` / `income`) ve devredilmemiş, faizsiz borçta
(`debt.discount_unavailable`). Bedel düşer: gider ya da gelir **borcun açıldığı
günde** azalır, para hareketi yoktur.

### `PUT /api/v1/debts/{id}/plan` — plan değişikliği

`{ "planKind", "firstDueDate", "installmentCount", "asOfDate" }`. O anki kalan
yeni takvime bölünür; eski kalemler kalır ama kalana girmez. Gelir/gider
yazılmaz. Faizli borcun planı değişmez (kullanıcı kararı, 16 Ekim 2026).

### `DELETE /api/v1/debts/{id}?asOfDate=` — iptal

Borç bütün kapanışlarıyla iptal olur. Kapanışı gün sonunda sayılmışsa
`debt.day_close_counted`, kartla tahsilin parası yatışla hesaba geçtiyse
`debt.deposit_locked`.

## Liste

### `GET /api/v1/debt-book?asOfDate=` — borç ve alacaklar

```json
{ "payableTotal": "500.0000", "receivableTotal": "16000.0000",
  "items": [ { "kind": "ledger", "id": "<kişi>", "direction": "receivable",
               "section": "overdue", "title": "Ayşe Terzi", "remaining": "3500.0000",
               "overdue": "2000.0000", "nextDueDate": "2026-11-30", "nextDueAmount": "1500.0000",
               "hasClosings": false, "isCarriedOver": false, "startDate": null, … } ],
  "people": [ { "counterpartyId": "…", "name": "Ayşe Terzi", "owedToYou": "8000.0000",
                "owedByYou": "0.0000", "ledgerOwedToYou": "3500.0000",
                "ledgerOwedByYou": "0.0000", "openDebtCount": 1 } ],
  "closedPayableCount": 1, "closedReceivableCount": 0 }
```

- Satır ya bir kişinin hesabıdır (`kind: ledger`, kimlik kişinin; yalnız
  taksitsiz hesap) ya taksitli ya da kişisiz bir kayıttır (`kind: debt`, kimlik
  borcun). Kişi listede bir kez durur (ADR 0022 İ6, K14).
- **17 Kasım 2026 (Faz 3, S7):**
  - `?scope=business|personal` (isteğe bağlı): kişi satırı kişinin türüyle,
    kayıt satırı kendi kapsamıyla süzülür; türü bilinmeyen eski kişi yalnız o
    kapsamda alım-satımı ya da vazgeçmesi varsa görünür. Toplamlar, gecikmiş
    toplamlar, kişiler ve kapanan sayıları seçili kapsamındır. Tanınmayan
    değer `debt.invalid_scope` (400).
  - `section`: `overdue` (gecikmiş tutarı var) · `upcoming` (sıradaki vade
    30 gün içinde) · **`other`** (vadesi daha ileride ya da vadesiz; eski
    `undated` kalktı). Sıra: bölüm; gecikenlerde en eski gecikmiş vade
    (`oldestOverdueDate`), yaklaşanlarda en yakın vade, diğerlerinde büyük
    kalan; sonra başlık.
  - Satırda `scope` (kişinin türü ya da kaydın kapsamı; `Hepsi` etiketi),
    `advanceAmount` (kişi satırında tutarın karşı yöndeki fazla ödemeden
    gelen kısmı; tamamıysa ekran "Alınan avans" / "Verilen avans" yazar),
    `oldestOverdueDate`, `installmentCount` ve `openInstallmentCount`
    (taksitli kayıtta "3/12 taksit").
  - Özette `payableOverdue` ve `receivableOverdue`.
  - `people`: **bütün** kişiler, ada göre; bakiyesi olmayan ve pasif olan
    dahil (`isActive`, `scope`). Pasif kişiyi yalnız aramada göstermek
    istemcinin işidir.
- Toplamlar ve kişi toplamları sunucudandır; istemci toplama yapmaz.
  Sabit sayıda sorguyla okunur (`DebtBook_ReadsWithAFixedNumberOfQueries`).

### `GET /api/v1/counterparties/{id}/page?asOfDate=` — kişi sayfası (S6)

`asOfDate` isteğe bağlı (boşsa bugün). Kişinin bilgisi (`name`, `note`,
`isActive`, `scope`, açılış bakiyesi) ve özet: `owedToYou` / `owedByYou`,
`overdueReceivable` / `overduePayable`, en yakın vadeler — kişinin hesabı
**ve** kişili taksitli kayıtlarının kalanıyla (K14). `collect` / `pay`:
tahsilat ve ödeme panelinin kısayolları (`full` = `Tamamı`, `overdue` =
`Gecikmiş`) — **yalnız hesabın**, çünkü düğmeler yalnız hesaba yazar (ADR
0022 İ7). `records`: kişili taksitli kayıtlar, listenin satır biçiminde.
Hareketler akıştan okunur: `GET /api/v1/financial-activities?counterpartyId=&direction=&dateFrom=&dateTo=&activityKind=`
(`direction` yeni: `receivable` · `payable`; ay seçici için gün aralığı, tür
süzgeci için `counterparty-charge` / `counterparty-settlement`). Başkasının
kişisi `404`.

### `GET /api/v1/debt-book/closed` — kapananlar

Kapanmış (iptal edilmemiş) borçlar: `title`, `amount` (bedel), `closedOn`.

## Kişinin hesabı (ADR 0022, 17 Kasım 2026)

Kişiyle olan taksitsiz her şey kişinin hesabına yazılır. Bu bölüm Faz 2'nin
ilk yarısını anlatır; kişi sayfası ve liste okumaları Faz 3'tedir.

### Kişinin türü — `POST` / `PUT /api/v1/counterparties`

İstek ve cevapta `scope`: `business`, `personal` ya da boş. İşletmesi olmayan
kullanıcıda boş gönderilirse `personal` yazılır; işletmesi olanda boş kalır ve
uydurulmaz. Güncellemede boş göndermek türü **değiştirmez** (tür yalnız
değiştirilir, boşaltılamaz). Tür hiçbir tutarı, geliri ya da gideri
değiştirmez; yalnız listeyi böler. Tanınmayan değer
`counterparties.invalid_scope` (400).

### Borçlandırmanın kapsamı — `POST /api/v1/counterparties/{id}/charges`

Kapsamı her kayıt gibi kategori sınırlar: tek kapsamlı kategori kapsamı
söyler; iki kapsama açık kategoride açık seçim, o yoksa kişinin türü; türü boş
eski kişide işletmesi olan kullanıcının kaydı `business`, olmayanınki
`personal`. Kategoriyle çelişen açık seçim `counterparties.scope_conflict`
(400). **Değişen:** eskiden borçlandırma her zaman `business` yazılır, şahsi
kategori reddedilirdi (ADR 0020 İ9; ADR 0022 ile kalktı).

İstek `clientRequestId` alabilir: aynı kimlikle tekrar gönderilen istek var
olan kaydı döner (ikinci kayıt yazılmaz); başka kişide kullanılmış kimlik
`counterparties.validation` (400).

**Peşin ya da avans tutarı** (`paidNow`, isteğe bağlı): `{ "amount", "accountId" }`.
Satış ya da alımla **aynı yazmada** kişinin hesabına tahsilat (satışta) ya da
ödeme (alımda) yazılır: satışın günü, yönü ve açıklamasıyla; tahsilat satışa
bağlıdır (`paidWithChargeId`). Örnek: 10.000 satış, vade 45 gün sonra, bugün
4.000 avans → gelir 10.000, hesap +4.000, alacak 6.000; vade geçince gecikmiş
6.000. Satışı aşan tutar `counterparties.paid_now_exceeds_charge` (400);
hesap yok, pasif ya da başkasının `counterparties.account_unavailable` (400).
Aynı `clientRequestId` ikisini de bir kez yazar. Cevap `paidNowPaymentId`
taşır (bu istekle yazılan tahsilat; yoksa boş).

### `POST /api/v1/counterparties/{id}/payments` — kişiye kredi kartıyla ödeme

İstek `creditCardId` alabilir (17 Kasım 2026): ödeme kişiye kredi kartıyla
yapılmıştır. Yalnız `payable` yönünde ve tek başına (`accountId` ve `card`
boş); aksi `counterparties.card_requires_payment` (400). Kart borcu tutar
kadar artar, hesap kıpırdamaz, **gider yazılmaz** (gider alım günü yazıldı;
senaryo 6). Limit kart harcamasıyla aynı kuraldır (`limit − borç − karta
yansımamış taksitler`); aşan tutar `credit_cards.limit_exceeded` (409). Kart
yok, pasif ya da başkasının: `counterparties.card_unavailable` (400). Cevapta
`accountId` boş, `creditCardId` doludur. Kart sayfasının hareketlerinde ve
akışta (`counterparty-settlement`, `sourceName` kartın adı) görünür; işlem
sonrası bakiye kart borcunu ve kalan limiti döner. İptali kart borcunu geri
alır.

### `POST /api/v1/counterparties/{id}/payments` — `Kalanı indirim say`

İstek `closeRemainderAsDiscount: true` alabilir (senaryo 20): ödemeden sonra o
tarafta kalan tutar indirim yazılır, ödemeyle aynı yazmada. İndirim alımın
bedelini düşürür; gelir ya da gider **alım gününde** azalır. Kalanın hangi
kalemlere ait olduğu ödemelerin sırasıyla bulunur (en eski önce kapanır, kalan
en yeni kalemlerdedir; gün, sonra giriş anı). Bu kalemlerin içinde borç para,
devreden bakiye, gün sonunda sayılmış ya da ödeme gününden sonraki bir alım
varsa `counterparties.discount_exceeds_charges` (400) ve hiçbir şey yazılmaz:
kalan vazgeçmeyle kapatılır. Örnek: 1.000 alım (5 Ağu), 500 borç para (11 Ağu),
1.300 ödeme → kalan 200 borç paraya aittir, reddedilir. Kalan yoksa bayrak etkisizdir. Cevap
`remainderDiscount` taşır (yoksa `"0.0000"`).

İndirimler ödemeye bağlıdır (ADR 0021 İ7): ödemenin iptali onları da geri
alır; tek başına geri alınmaları `counterparty_charges.discount_belongs_to_payment`
(409).

### `POST /api/v1/counterparties/{id}/write-offs` — vazgeçme

```json
{ "direction": "payable", "amount": "1000.0000", "currency": "TRY",
  "categoryId": "…", "writeOffDate": "2026-08-20", "scope": null,
  "description": "Ali kalanı istemedi", "clientRequestId": "…" }
```

Kalan tutardan vazgeçmek (ADR 0021 İ4): `receivable` alınmayacak alacak —
**gider**, gider kategorisi; `payable` bağışlanan borç — **gelir**, gelir
kategorisi. Para hareket etmez; kişinin bakiyesi ödeme gibi düşer (en eski
kalemden). O taraftaki kalanı aşamaz (`counterparties.write_off_exceeds_balance`,
400); kategori türü uymuyorsa `counterparties.category_unavailable`. Kapsam
borçlandırmanın zinciriyle kurulur. Akışta `counterparty-write-off` (etkisi
gelir ya da gider, kategorili). Pasif kişide de yazılır.

### `DELETE /api/v1/counterparty-write-offs/{id}` — vazgeçmenin iptali

İdempotent; kalan geri gelir, tanınan gelir ya da gider düşer. Yok ya da
başkasının: `counterparties.write_off_not_found` (404).

### `DELETE /api/v1/counterparty-charges/{id}?withPaidNow=` — satışın iptali

Varsayılan (`false`): yalnız satış iptal olur; birlikte yazılmış avans
kişinin hesabında **kalır** (iş iptal oldu, para alınmıştı → "borcunuz").
`withPaidNow=true`: ikisi tek yazmada iptal olur (yanlış girilmiş kayıt).
Avans gün sonunda sayıldıysa `counterparty_payments.day_close_counted` (409)
ve hiçbir şey değişmez. Akış satırı satışa bağlı iptal edilmemiş peşin
tutarı `paidNowAmount` alanında taşır (yoksa `null`; 23 Kasım 2026);
istemci iptalde "peşin de iptal edilsin mi" sorusunu ondan kurar.

### `POST /api/v1/counterparties/{id}/loans` — borç para

```json
{ "direction": "receivable", "amount": "5000.0000", "currency": "TRY",
  "accountId": "…", "loanDate": "2026-08-05", "dueDate": "2026-09-01",
  "description": "Ev taşıma", "clientRequestId": "…" }
```

`receivable`: verdim (alacak artar, para hesaptan çıkar); `payable`: aldım
(borç artar, para hesaba girer). Gelir ya da gider **yazmaz**; kategori ve
kapsam taşımaz (ADR 0014). `dueDate` isteğe bağlıdır, `loanDate`ten önce
olamaz; vadesi geçen kısım gecikmiş tutara girer (en eski kalemden başlayarak).
Geri ödeme sıradan bir tahsilat ya da ödemedir. Cevap `201`, gövde:
`id`, `counterpartyId`, `accountId`, `direction`, `amount`, `currency`,
`loanDate`, `dueDate`, `description`, `isCancelled`.

Avans ayrı bir kayıt değildir: tahsilat ya da ödemedir ve sonraki satış ya da
alımdan kendiliğinden düşer (`stages/06.3-borc-alacak/02-TASARIM.md` §7a).

Hatalar: gün ileride `counterparties.date_in_future`; pasif kişi
`counterparties.inactive` (409; kalan bakiye ödenebilir); hesap yok, pasif ya
da başkasının `counterparties.account_unavailable` (400); kişi yok ya da
başkasının `counterparties.not_found` (404).

### `DELETE /api/v1/counterparty-loans/{id}` — borç paranın iptali

İdempotenttir; cevap kaydın kendisidir (`isCancelled: true`). Kişinin ve
hesabın bakiyesi yeniden hesaplanır; başka kayıt değişmez. Yok ya da
başkasının: `counterparties.loan_not_found` (404). Borç parası olan kişi
silinemez (`counterparties.has_history`, 409).

## Cari

### Gecikme (değişti)

Cari ödemeler **önce en eski alımı** kapatır (alım günü, sonra giriş anı;
ADR 0021 T8). Gecikmiş tutar, vadesi geçmiş alımların bu sırayla kapanmamış
kısmıdır. Kişinin cevabına `nextReceivableDueDate/Amount` ve
`nextPayableDueDate/Amount` eklendi.

### `POST /api/v1/counterparty-charges/{chargeId}/discounts` — cari alıma indirim

`{ "amount", "discountDate", "clientRequestId"? }`. Alımın bedeli düşer, gelir
ya da gider alım gününde azalır, cari bakiye azalır; para hareketi yoktur.
İndirim alımın kalanını aşamaz (`counterparty_charges.discount_exceeds_amount`);
gün sonunda sayılmış satış değişmez (`counterparty_charges.day_close_counted`);
iptal edilmiş alım değişmez (`counterparty_charges.cancelled`).

### `DELETE /api/v1/counterparty-charges/{chargeId}/discounts/{discountId}` — indirimi geri alma

İdempotenttir. Cevap alımın kendisidir (`CounterpartyChargeResponse`): eklenen
`netAmount` (bedel) ve `discounts[]` (`id`, `amount`, `discountDate`,
`isCancelled`).

### `PUT /api/v1/counterparties/{id}/opening-balance` — devreden bakiye

`{ "receivable": "0.0000", "payable": "300.0000" }` — ikisi de sıfır ya da artı.
Gelir ya da gider yazmaz; cari bakiyeye **en eski kalem** olarak girer (ödeme
önce onu kapatır). Yeniden yazılabilir; sıfır devri kaldırır. Kişinin cevabına
`openingReceivable` ve `openingPayable` eklendi (bakiyenin içindedir).

## İşlemler akışı

**17 Kasım 2026:** yeni türler `counterparty-write-off` (vazgeçme; `effect`
gelir ya da gider, kategorili) ve `counterparty-loan` (borç para; `effect: neutral`,
`sourceGroup: counterparty`, yönü `direction`). İşlem sonrası bakiye hesabı ve
kişinin açık bakiyesini döner. **Bugünkü Flutter istemcisi bu türü tanımıyor**
ve bilinmeyen türde akışın tamamını reddeder; istemciden borç para yazılamadığı
için bugün tetiklenmez. İstemci türü Faz 5–7'de tanır.

16 Ekim teslimi (istemci bilinmeyen türü reddettiği için yeni tür eklenmedi):

- Kartla ödenen borç: tek ödemelide `obligation-settlement`, taksitlide
  `debt-payment`; `sourceName` kartın adıdır. İşlem sonrası bakiye kart borcunu
  döner (`credit-card`, `increased`); hesap satırı yoktur.
- Tek ödemeli borcun, kayıtlı alım/satış borcunun ve cari alımın satırı
  **indirim düşülmüş bedeli** yazar (raporun saydığı); indirim `discountAmount`
  alanındadır (yoksa `null`) ve istemci onu tutarın altında yazar (kullanıcı
  kararı, 16 Ekim 2026).
- Kişisiz borcun satırları görünür; başlık açıklamadır.
- Devreden borcun açılış satırı yoktur; ödemeleri görünür.

## Hata kodları

| Kod | HTTP | Ne zaman |
|---|---|---|
| `debt.not_found` | 404 | Borç yok ya da başkasının |
| `debt.payment_not_found` | 404 | Kapanış bu borçta yok |
| `debt.amount_exceeds_remaining` | 400 | Ödeme ya da indirim kalanı aşıyor |
| `debt.interest_bearing` | 400 | Faizli borçta kısmi taksit ya da plan değişikliği |
| `debt.payment_source_invalid` | 400 | Yer yok, birden çok, ya da alacak kartla |
| `debt.card_unavailable` / `debt.account_unavailable` | 400 | Kart ya da hesap kullanılamıyor |
| `debt.discount_unavailable` | 400 | Nakit, devredilmiş ya da faizli borçta indirim |
| `debt.description_required` | 400 | Kişisiz borçta açıklama yok |
| `debt.due_date_required` | 400 | Planlı borçta vade yok |
| `debt.date_in_future` | 400 | Ödeme ya da indirim günü ileride |
| `debt.day_close_counted` | 409 | Kapanış gün sonunda sayılmış |
| `debt.deposit_locked` | 409 | Kartla tahsilin parası yatışla geçmiş |
| `debt.changed` | 409 | Aynı borç eşzamanlı değişti; yeniden okuyup dene |
| `counterparty_charges.discount_exceeds_amount` | 400 | Cari indirim alımı aşıyor |
| `counterparty_charges.discount_not_found` | 404 | İndirim bu alımda yok |
| `counterparty_charges.cancelled` | 409 | İptal edilmiş alım |
| `counterparties.invalid_scope` | 400 | Kişinin türü tanınmıyor |
| `counterparties.scope_conflict` | 400 | Borçlandırmada açık seçim kategoriyle çelişiyor |
| `counterparties.loan_not_found` | 404 | Borç para yok ya da başkasının |
| `counterparties.paid_now_exceeds_charge` | 400 | Peşin ya da avans tutarı satışı aşıyor |
| `counterparties.card_requires_payment` | 400 | Kredi kartı tahsilatta ya da hesapla birlikte verilmiş |
| `counterparties.card_unavailable` | 400 | Kart yok, pasif ya da başkasının |
| `counterparties.write_off_exceeds_balance` | 400 | Vazgeçilen tutar o taraftaki kalanı aşıyor |
| `counterparties.write_off_not_found` | 404 | Vazgeçme yok ya da başkasının |
| `counterparties.discount_exceeds_charges` | 400 | Kalan, indirim düşülebilecek alımlardan büyük |
| `counterparty_charges.discount_belongs_to_payment` | 409 | Ödemeyle yazılmış indirim tek başına geri alınmak istendi |
| `counterparties.concurrent_change` | 409 | Borç para eşzamanlı değişti |

## Yedek

Yedek v13 borcu durumuyla taşır (`debtRecords`); v14 kişinin türünü ve borç
parayı (`counterpartyLoans`) ekler. Sürüm ve okunan sürümler
`restore-runbook.md` içindedir.
