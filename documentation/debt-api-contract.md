# Borç ve alacak API sözleşmesi

ADR 0021 dalga 2'nin sunucu sözleşmesi (16 Ekim 2026). Yükümlülük ve borç
planı tek borçtur; borç kendi kapanışlarıyla (ödeme, kartla ödeme, indirim)
kapanır. Bütün uçlar kimlik doğrulaması ister; kullanıcı kimliği yalnız
token'dan okunur. Başka kullanıcının kaydı ile olmayan kayıt aynı `404`
cevabını alır. Para dört ondalıklı **metin**dir, gün `yyyy-MM-dd`'dir.

Eski uçlar (`/api/v1/obligations`, `/api/v1/debts/{id}/installments/{n}/pay`,
`/api/v1/debts/{id}/opening`) Flutter yeni sayfaya geçene kadar aynı
sözleşmeyle çalışır; o adımda kalkar.

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

- Satır ya bir borçtur (`kind: debt`, kimlik borcun) ya bir kişinin carisidir
  (`kind: ledger`, kimlik kişinin). Cari ayrı kalır (ADR 0021 İ3).
- `section`: `overdue` (gecikmiş tutarı var) · `upcoming` (sıradaki vadesi var)
  · `undated` (vadesi yok). Sıra: bölüm, sonra yakın vade, aynı günde büyük kalan.
- Toplamlar ve kişi toplamları sunucudandır; istemci toplama yapmaz.

### `GET /api/v1/debt-book/closed` — kapananlar

Kapanmış (iptal edilmemiş) borçlar: `title`, `amount` (bedel), `closedOn`.

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

Yeni tür eklenmedi (istemci bilinmeyen türü reddediyor):

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

## Yedek

Yedek v13 borcu durumuyla taşır (`debtRecords`); sürüm ve okunan sürümler
`restore-runbook.md` içindedir.
