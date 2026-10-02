# Finansal Hareket API Sözleşmesi

- Durum: **Uygulandı.** Bu belge çalışan davranışı tarif eder; birleşik feed
  (bölüm 1), planlanan görünüm (bölüm 2), recurring source (bölüm 3) ve
  origin-aware iptal (bölüm 4) backend ve Flutter tarafında yerindedir.
- İlgili kararlar: `documentation/adr/0004-unified-financial-activity-read-model.md`,
  `documentation/adr/0005-recurring-transaction-source.md`

Sözleşme değişirse `documentation/tests.md`, `flows.md` ve `permissions.md`
aynı commit'te güncellenir.

## Ortak sözleşme kuralları

Mevcut API sözleşmesi aynen korunur; yeni endpoint'ler bu kurallara uyar.

| Konu | Kural |
|---|---|
| Para | Dört ondalıklı **string**: `"625.5000"`. JSON number kullanılmaz. |
| Para birimi | Ayrı alan: `"TRY"` |
| Tarih | `yyyy-MM-dd` string: `"2026-08-14"` |
| Zaman damgası | ISO-8601 UTC offset: `"2026-08-14T09:12:31.4820000+00:00"` |
| Enum değerleri | Küçük harf **kebab-case** string: `"card-charge"`, `"csv-import"` |
| JSON alan adları | camelCase (ASP.NET Core varsayılanı) |
| Kimlik | `sub` claim'inden türetilir; hiçbir request alanı `userId` taşımaz |
| Hata | `application/problem+json`, kararlı `code` alanı |
| Sayfalama | Mevcut `PaginationMetadata` kaydı yeniden kullanılır |

Yabancı veya var olmayan kayıt aynı `404 NotFound` görünümüne gider.

## Sınıflandırma enum'ları

Bu beş boyut bağımsızdır; tek eksene indirgenmez (bkz. ADR 0004).

### `activityKind`

| Değer | Kaynak yazma modeli |
|---|---|
| `account-transaction` | `BudgetTransaction` |
| `transfer` | `Transfer` |
| `card-charge` | `CreditCardCharge` |
| `card-payment` | `CreditCardPayment` |
| `debt-payment` | Ödenmiş `DebtInstallment`, `direction=payable` |
| `debt-collection` | Tahsil edilmiş `DebtInstallment`, `direction=receivable` |
| `debt-opening` | Borcun doğduğu an. Nakit kaynakta `neutral`, gider kaynakta `expense` |
| `counterparty-charge` | `CounterpartyCharge` — veresiye satış (`income`) ya da vadeli alım (`expense`) |
| `counterparty-settlement` | `CounterpartyPayment` — cari tahsilat/ödeme; her zaman `neutral` |
| `obligation` | `Obligation` — tek seferlik yükümlülüğün doğuşu |
| `obligation-settlement` | `ObligationSettlement` — onu kapatan nakit hareketi; her zaman `neutral` |
| `pos-sale` | `PosSettlement` — satışın tanındığı an; gelir **brüt** tutar kadar |
| `pos-deposit` | `PosDeposit` — paranın hesaba yattığı an; **gerçekten yatan** tutar, `neutral`. `activityId` yatışın kimliğidir; bir yatış birkaç tahsilatı kapatabilir |

Bir POS tahsilatı feed'de **tek satırdır** (`pos-sale`, tahsilatın
`activityId`'siyle, brüt tutarla). Paranın hesaba geçişi tahsilatın değil
**yatışın** satırıdır (`pos-deposit`, Aşama 06.3 Grup 5): gelir tahsilat günü,
para ise yatış günü gerçektir. İstemci satırı `activityKind + activityId`
ikilisiyle anahtarlar.

**Bir kaydın parçası ayrı satır olmaz** (2 Ekim 2026, kullanıcı kararı).
Komisyon satışın, kesinti yatışın parçasıdır ve bağlı olduğu satırda taşınır:

| Alan | `pos-sale` | `pos-deposit` |
|---|---|---|
| `feeAmount` | komisyon; sıfırsa `null` | kesinti; sıfırsa `null` |
| `channelName` | tahsilatın POS'u; POS seçilmeden girilende `null` | kapattığı tahsilatların hepsi aynı POS'tansa o; değilse `null` |
| `netAmount` | hesaba geçecek (ya da geçmiş) net | — |
| `expectedTransferDate`, `transferredOn` | beklenen gün; geçtiyse geçtiği gün | — |
| `settlementCount` | — | kapattığı tahsilat sayısı (geri alınmış yatışta `0`) |

`feeAmount` gider olarak **tanınmıştır**: raporlarda ve bütçede sayılır.
`amount` ondan etkilenmez — satışta brüt, yatışta gerçekten yatan tutardır.
Bedeli: akışı `effect=expense` ya da komisyon kategorisiyle süzmek bu
tutarları ayrı satır olarak getirmez.

`pos-commission` değeri **kalktı** (2 Ekim 2026): beş satışın beş komisyonu
alt alta beş ayrı satır olunca hangisinin hangi satışa ait olduğu okunmuyordu.
Bu değerle süzmek `400 financial_activities.invalid_filter_value` döner.

`pos-transfer` değeri **kalktı** (2 Ekim 2026): yerini `pos-deposit` aldı.
Tahsilat başına bir satır yerine yatış başına bir satır vardır ve tutar
tahsilatın neti değil bankanın yatırdığı tutardır.

### `effect`

| Değer | Anlamı |
|---|---|
| `income` | Aylık gelir toplamına girer |
| `expense` | Aylık gider toplamına ve kategori bütçesine girer |
| `neutral` | Gelir/gider raporunu değiştirmez; likiditeyi etkileyebilir |

### `sourceGroup`

`account`, `credit-card`, `transfer`, `debt`, `counterparty`, `obligation`,
`pos`

`pos`, `credit-card`'dan ayrı bir gruptur ve olmak zorundadır: biri
borçlandığın kart, diğeri müşterinin ödediği paradır (ADR 0015).

Açık cari (`counterparty`) ile taksitli sözleşme (`debt`) ayrı gruplardır:
aynı kişiye ait olsalar bile biri yürüyen bir hesap, diğeri vadesi belli bir
plandır ve kullanıcı ikisini ayrı sorar.

### `origin`

| Değer | Tespit kuralı |
|---|---|
| `csv-import` | `ImportRow.BudgetTransactionId` eşleşiyor |
| `recurring` | `RecurringTransactionOccurrence` sonuç bağlantısı eşleşiyor |
| `installment` | `InstallmentItem.CreditCardChargeId` eşleşiyor |
| `pos-deposit` | `PosDeposit.DeductionTransactionId` eşleşiyor — bir POS yatışının kesinti gideri |
| `manual` | Hiçbiri eşleşmiyor |

Aynı sonuç hareketinin birden fazla kökene bağlanmasını filtered unique
index'ler ve Domain invariant'ları engeller.

### `status`

`realized`, `cancelled`

## Kaynak olay → sınıflandırma matrisi

| Kaynak olay | activityKind | effect | sourceGroup | canCancel |
|---|---|---|---|---|
| Hesaba gelir | `account-transaction` | `income` | `account` | origin'e bağlı |
| Hesaptan gider | `account-transaction` | `expense` | `account` | origin'e bağlı |
| Hesaplar arası transfer | `transfer` | `neutral` | `transfer` | evet |
| Kredi kartı harcaması | `card-charge` | `expense` | `credit-card` | origin'e bağlı |
| Kredi kartı ödemesi | `card-payment` | `neutral` | `credit-card` | evet |
| Gerçekleşen kart taksidi | `card-charge` | `expense` | `credit-card` | **hayır** (`installment`) |
| Gerçekleşen hesap recurring | `account-transaction` | `income`/`expense` | `account` | **hayır** (`recurring`) |
| Gerçekleşen kart recurring | `card-charge` | `expense` | `credit-card` | **hayır** (`recurring`) |
| CSV confirm işlemi | `account-transaction` | `income`/`expense` | `account` | evet (`csv-import`) |
| Borç taksidi ödemesi | `debt-payment` | `neutral` | `debt` | **hayır** |
| Alacak taksidi tahsilatı | `debt-collection` | `neutral` | `debt` | **hayır** |
| Borç açılışı, nakit kaynak | `debt-opening` | `neutral` | `debt` | **hayır** |
| Borç açılışı, gider kaynak | `debt-opening` | `expense` | `debt` | **hayır** |
| Veresiye satış | `counterparty-charge` | `income` | `counterparty` | evet |
| Vadeli alım | `counterparty-charge` | `expense` | `counterparty` | evet |
| Cari tahsilat/ödeme | `counterparty-settlement` | `neutral` | `counterparty` | evet |
| Tek seferlik yükümlülük doğuşu | `obligation` | `income`/`expense` | `obligation` | **hayır** |
| Yükümlülük ödeme/tahsilatı | `obligation-settlement` | `neutral` | `obligation` | **hayır** |
| POS satışının tanınması | `pos-sale` | `income` | `pos` | **hayır** |
| POS yatışı | `pos-deposit` | `neutral` | `pos` | **hayır** |

`canCancel` formülü:

```text
canCancel = status == realized
         && activityKind ∉ { debt-payment, debt-collection, debt-opening,
                             obligation, obligation-settlement,
                             pos-sale, pos-deposit }
         && origin ∉ { recurring, installment, pos-deposit }
```

Borç açılışı iptal edilemez: o satır sözleşmenin kendisidir, iptali borcu
silmek olurdu ve ödenmiş taksitler sahipsiz kalırdı. Borcu bitirmenin yolu bu
satır değil, sözleşme akışıdır.

Aynı `debt-opening` iki farklı `effect` taşır ve bu kasıtlıdır: nakit kaynakta
para el değiştirir, gider kaynakta tüketim olur. Etki tek başına türden
okunamaz — beş boyutun bağımsız olmasının sebebi tam olarak budur. Aynı şey
`counterparty-charge` için de geçerli: yön alacaksa gelir, borçsa giderdir.

Cari hareketin iki türü de **iptal edilebilir** ve sözleşmeden farkı burada:
her biri tek başına duran bir kayıttır, geri dönüşü olmayan bir planın sonucu
değil. Borçlandırmayı iptal etmek tanınan gelir/gideri ve açık bakiyeyi
birlikte geri alır; tahsilatı iptal etmek parayı kasaya geri koyar ve açık
bakiyeyi yeniden doğurur.

POS satışı feed üzerinden **iptal edilemez**: iptal, kaydın kendi ekranından
tek eylemle yapılır ve satışı, komisyonu ve yoldaki tutarı birlikte kaldırır;
yatışa bağlı tahsilat `409 pos_settlements.deposit_locked` döner.

`pos-deposit` de feed üzerinden iptal edilmez: yatış kendi ucundan
(`DELETE /api/v1/pos-deposits/{id}`) geri alınır ve geri alma kapattığı
tahsilatları yola döndürür. Geri alınmış yatış feed'de `cancelled` durumuyla
kalır. Yatışın **kesinti gideri** bir `BudgetTransaction`'dır ama akışta
**satır değildir** (yatışın `feeAmount`'ıdır); kökeni `pos-deposit`'tir ve tek
başına iptal edilemez (`409 transactions.cancel_origin_locked`): iptal
edilseydi yatış, hesaba gerçekte geçmemiş bir tutarı geçmiş gösterirdi. Vergi
ödemesini geri alma ucu da aynı kökeni reddeder. `origin=pos-deposit` değeri
sözleşmede durur ama bugün hiçbir akış satırında dönmez.

`pos-deposit` **kapsam taşımaz** (`scope: null`): parayı taşır, gelir/gider
üretmez (ADR 0014). Kapsam filtreli okumada düşer; `pos-sale` kapsam taşır ve
kalır. Komisyon brüt tutardan **düşülmez**: gelir brüt kadar tanınır, komisyon
kendi kategorisinde ayrı bir giderdir.

### Yön (`direction`)

Cari ve yükümlülük satırları yönünü taşır: `receivable` (karşı taraf bize
borçlu) ya da `payable` (biz ona borçluyuz); diğer türlerde `null`.
`counterparty-settlement` ve `obligation-settlement` iki yönü de taşıyan
türlerdir: aynı tür hem **tahsilat** hem **ödeme**dir, hangisi olduğunu yön
söyler. İstemci adı (`Cari tahsilat` / `Cari ödeme`, `Alacak tahsilatı` /
`Borç ödemesi`) ve paranın akış yönünü (`kişi → hesap` ya da `hesap → kişi`)
bundan kurar.

Bu iki türde **`sourceName` hep hesap, `destinationName` hep karşı taraftır**
(2 Ekim 2026). `obligation-settlement` daha önce adları paranın akış yönüyle
gönderiyordu; alacak tahsilatında hesap ile karşı taraf yer değiştiriyor ve
ayrıntı "Hesap: Ahmet Bakkal" yazıyordu.

### Sıra

```text
activityDate DESC, girişAnı DESC, activityKind, activityId
```

Aynı günün kayıtları türe göre değil **giriş sırasına** göre dizilir, en yeni
üstte (2 Ekim 2026). Giriş anını sunucu yazar (kaydın veritabanına yazıldığı
an); kullanıcıdan istenmez ve cevapta taşınmaz. Kaydın günü kullanıcının
seçtiği tarihtir: dün için bugün girilen kayıt dünün altında durur. Bu alandan
önce yazılmış kayıtların giriş anı **bilinmez ve uydurulmaz**; günün sonuna
düşer ve kendi aralarında eski sıralarını (tür, kimlik) korurlar.

### Başlık

`title` kullanıcının yazdığı açıklamadır. Açıklama yoksa yedek, kaydın **ne
olduğunu** söyleyen addır ve bir hesap ya da kart adı **olmaz**:

| Tür | Açıklama yoksa `title` |
|---|---|
| gelir/gider, kart harcaması, POS satışı | kategori adı |
| borç, cari, yükümlülük | karşı tarafın adı (yoksa kategori) |
| transfer, kart ödemesi, POS yatışı | **boş dize** — istemci türün adını yazar |

Para taşıyan kaydın hesabı ve kartı zaten `sourceName` / `destinationName`
içindedir; başlıkta tekrar edilince satır "Ziraat / Ziraat" diye okunuyordu.
Türün adı istemcinindir (API cümle göndermez).

### İşlem sonrası bakiye

```text
GET /api/v1/financial-activities/{activityKind}/{activityId}/balances
```

```json
{ "items": [
  { "holder": "account",     "id": "…", "name": "Ziraat Vadesiz", "balance": "630.0000", "currency": "TRY", "change": "decreased" },
  { "holder": "credit-card", "id": "…", "name": "Bonus",          "balance": "180.0000", "currency": "TRY", "change": "decreased" }
] }
```

Hareketin dokunduğu hesabın bakiyesi ve kartın borcu, o hareketten **hemen
sonra**. "Sonra", akışın sırasıdır: önceki günler bütünüyle, aynı günde giriş
anı bu hareketten büyük olmayanlar. Akışın tek sorgusuna eklenmez; yalnız
ayrıntı açıldığında okunur. Kalıcı bir alan değildir.

- Transfer iki hesap, kart ödemesi bir hesap ve bir kart döner.
- **`change`** hareketin o bakiyeye ne yaptığını söyler: `increased`,
  `decreased` ya da `unchanged`. Kartta bakiye **borçtur**: harcama
  `increased`, ödeme `decreased`. İstemci bunu türden türetmez (cari
  tahsilatın ve yükümlülük kapanışının yönü satırda yoktur); yalnız renge
  çevirir.
- **POS satışı hesabını `unchanged` ile döner** (2 Ekim 2026): satış gelir
  yazar ama hesaba dokunmaz, para yatışla geçer. Dönen sayı hesabın o anki
  bakiyesidir; satışın tutarı içinde değildir.
- **Kart kalan limiti de taşır** (`availableLimit`): o andaki borca göre
  `limit − borç`, sıfırın altına inmez. Limitin geçmişi tutulmaz; kartın
  **bugünkü** limitiyle hesaplanır (kullanıcı kararı, 2 Ekim 2026: limit sık
  değişmez). Limit sonradan değiştirilirse eski harcamanın ayrıntısındaki
  sayı o günkü gerçeği değil, bugünkü limite göre hesabı gösterir.
- **Cari kayıtlar karşı tarafın açık bakiyesini döner** (`holder:
  "counterparty"`): veresiye/vadeli kayıt, cari tahsilat/ödeme, karşı tarafı
  olan tek seferlik yükümlülük ve kapanışı. `balance` **hep artıdır**; kimin
  kime borçlu olduğunu `side` söyler: `receivable` (karşı taraf bize borçlu),
  `payable` (biz ona borçluyuz), `settled` (açık tutar yok). Fazla tahsilat
  kırpılmaz, tarafı çevirir. Kaynakları kişinin güncel bakiyesiyle aynıdır
  (borçlandırma, tahsilat/ödeme, açık yükümlülük).
- Cari bakiye **önceki tarafı** da taşır (`previousSide`): hareketten hemen
  önce kim kime borçluydu. Sonrakinden farklıysa hareket tarafı çevirmiştir
  (alacak tahsil edildi, geriye borç kaldı); istemci bunu tek satırla söyler.
  İlk kayıtta `settled`'dır.
- **Borç kayıtları anlaşmanın kalanını döner** (`holder: "debt"`): açılış ve
  taksit. `balance` o an henüz ödenmemiş taksitlerin toplamıdır; `side`
  anlaşmanın yönüdür, kalan sıfırsa `settled`.
- `change` bu iki yerde gösterilen (artı) tutarın büyüdüğünü ya da
  küçüldüğünü söyler. `availableLimit` yalnız kartta, `side` yalnız cari ve
  borçta dolar; diğerlerinde `null`.
- **Boş liste** döner: hareketin hesabı, kartı, karşı tarafı ya da borcu
  yoksa (karşı tarafsız yükümlülük), iptal edilmişse ya da giriş anı
  bilinmiyorsa. Bilinmeyen bir sıra için bakiye uydurulmaz.
- Başkasının hareketi, olmayan hareket ve türü tutmayan kimlik aynı `404
  financial_activities.not_found` cevabını alır; bilinmeyen tür `400`.
- Yatışın sonrası kesinti giderini de içerir. `GET /api/v1/pos-deposits/{id}`
  aynı sayıyı `balanceAfter` alanında taşır.

`counterparty-settlement` **kapsam taşımaz** (`scope: null`): kart ödemesiyle
birebir aynı gerekçe — gelir/gider raporuna hiç girmediği için bölünecek bir
tarafı yok. Kapsam filtreli okumada bu satırlar da düşer.

`csv-import` iptal edilebilir; `ImportRow.Status=Imported` değişmeden kalır, yani
yeniden import engeli bozulmaz.

### Feed'de görünmeyen kayıtlar

Planlanan occurrence ve projeksiyonlar, recurring tanımının kendisi, taksit
planının kendisi, kart ekstresi projection'ı, import batch/row teknik kayıtları,
hesap/kart/kategori/bütçe tanımları, tasarruf hedefi katkıları, **gün sonu kasa
sayımı** ve **açılış bakiyesi**.

Kasa sayımı feed'de **yoktur** ve bu bir eksiklik değildir: sayım bir
gözlemdir, hiçbir para hareketi üretmez. Kullanıcı farkı açıkça onaylarsa
üretilen düzeltme kaydı normal bir `account-transaction` olarak zaten görünür;
sayımın kendisini de satır yapmak, olmamış bir hareketi kayda geçirmek olurdu.

İptal edilmiş hareketler varsayılan listede kalır, bakiyeye ve rapora katılmaz,
gelişmiş filtreden gizlenebilir. İptal geçmişi fiziksel silinmez.

---

## 1. Gerçekleşmiş hareket feed'i

> **Uygulandı (Grup 4).** Endpoint, filtreler, sıralama, SQL `UNION ALL`
> birleştirmesi ve `canCancel`/`supportsAttachments` çalışır durumdadır.
> `title` sözleşmesi uygulama sırasında düzeltildi: backend Türkçe sabit metin
> döndürmez, yalnız kullanıcının kendi girdiği adları taşır (bkz. Alan notları).

```text
GET /api/v1/financial-activities
```

Owner-scoped, `RequireAuthorization`.

### Query parametreleri

| Parametre | Tip | Varsayılan | Kural |
|---|---|---|---|
| `pageNumber` | int | `1` | ≥ 1 |
| `pageSize` | int | `20` | 1–100 |
| `dateFrom` | `yyyy-MM-dd` | yok | dahil |
| `dateTo` | `yyyy-MM-dd` | yok | dahil |
| `counterpartyId` | guid | yok | tek değer; cari hareketleri **ve** o kişinin sözleşme hareketlerini getirir |
| `sourceGroup` | enum | yok | tek değer |
| `activityKind` | enum | yok | tek değer |
| `effect` | enum | yok | tek değer |
| `origin` | enum | yok | tek değer |
| `accountId` | guid | yok | owner-scoped |
| `creditCardId` | guid | yok | owner-scoped |
| `categoryId` | guid | yok | owner-scoped |
| `includeCancelled` | bool | `true` | |
| `search` | string | — | Kayıt adı, açıklama, kategori ve hesap/karşı taraf adında geçen metin; kırpılır, boşsa filtre yok, en fazla 100 karakter (`400 financial_activities.invalid_search`). Aynı tek sorgunun filtresidir, sayım da ona göre yapılır |

Filtreler v1'de **tek değerlidir** (çoklu seçim yok); `UNION ALL` sorgusunu ve
index kullanımını basit tutar. Serbest metin araması yoktur.

**Varsayılan tarih davranışı:** Flutter, kullanıcı tarih filtresine dokunmadıkça
`dateFrom`/`dateTo` göndermez. Gizli varsayılan pencere yoktur; daraltma
gelişmiş filtredeki görünür kısayollarla yapılır (`Bu ay`, `Son 3 ay`,
`Son 12 ay`, `Tümü` = varsayılan).

### Sıralama

İş tarihine göre azalan; aynı tarihte `activityKind`, sonra `activityId` ile
kararlı tie-breaker. Sayfalama ve toplam sayı SQL tarafında hesaplanır.

### Örnek cevap — `200 OK`

```json
{
  "items": [
    {
      "activityId": "8f1c6d2a-4b7e-4a51-9c33-2d5e8a7b1f04",
      "activityKind": "card-charge",
      "effect": "expense",
      "sourceGroup": "credit-card",
      "origin": "manual",
      "status": "realized",
      "activityDate": "2026-08-14",
      "amount": "625.5000",
      "currency": "TRY",
      "title": "Market",
      "description": "Haftalık alışveriş",
      "categoryId": "3a9b0c11-5d2f-4e88-b7a6-0c1d4e5f6a72",
      "categoryName": "Groceries",
      "sourceId": "c7e2f4a8-9b13-4d6e-8f52-1a3b5c7d9e01",
      "sourceName": "Test Kart",
      "destinationId": null,
      "destinationName": null,
      "cancelledAtUtc": null,
      "canCancel": true,
      "supportsAttachments": false
    },
    {
      "activityId": "1d4e7a90-2c58-4b36-9e01-7f8a2b3c4d5e",
      "activityKind": "transfer",
      "effect": "neutral",
      "sourceGroup": "transfer",
      "origin": "manual",
      "status": "realized",
      "activityDate": "2026-08-13",
      "amount": "250.0000",
      "currency": "TRY",
      "title": "Nakit",
      "description": null,
      "categoryId": null,
      "categoryName": null,
      "sourceId": "5b8c1e02-6a4d-4f79-8c13-9d0e2f4a6b81",
      "sourceName": "Banka",
      "destinationId": "9e0f2a13-7b5c-4d81-a624-0e1f3a5b7c92",
      "destinationName": "Nakit",
      "cancelledAtUtc": null,
      "canCancel": true,
      "supportsAttachments": false
    },
    {
      "activityId": "6c2b8f45-3e17-49da-8b60-4a1c5d7e9f23",
      "activityKind": "account-transaction",
      "effect": "expense",
      "sourceGroup": "account",
      "origin": "recurring",
      "status": "cancelled",
      "activityDate": "2026-08-01",
      "amount": "125.0000",
      "currency": "TRY",
      "title": "Spor salonu",
      "description": null,
      "categoryId": "3a9b0c11-5d2f-4e88-b7a6-0c1d4e5f6a72",
      "categoryName": "Subscriptions",
      "sourceId": "5b8c1e02-6a4d-4f79-8c13-9d0e2f4a6b81",
      "sourceName": "Banka",
      "destinationId": null,
      "destinationName": null,
      "cancelledAtUtc": "2026-08-02T07:41:18.9310000+00:00",
      "canCancel": false,
      "supportsAttachments": true
    }
  ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 143,
    "totalPages": 8,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

### Alan notları

- `title`: Kullanıcının kendi girdiği ad — kategorili hareketlerde kategori adı,
  transferde hedef hesap adı, kart ödemesinde kart adı, borç/alacakta karşı taraf
  adı. Backend **sabit cümle döndürmez**; tür etiketini (`"Hesaplar arası
  transfer"` gibi) `activityKind`'dan Flutter üretir. Aksi halde API tek bir dile
  bağlanır ve mevcut istemci-tarafı yerelleştirme kararıyla çelişirdi.
- `sourceId`/`sourceName`: Paranın çıktığı hesap veya kart.
  `destinationId`/`destinationName` yalnız transfer ve kart ödemesinde doludur.
- `supportsAttachments`: Yalnız `BudgetTransaction` kaynaklı hareketlerde
  `true`. Mevcut attachment modeli başka türü desteklemez.
- Adlar backend projection'ında döner. Flutter'ın ayrı listelerden isim join
  etmesi veya finansal etki hesaplaması yasaktır.

### Hata kodları

| HTTP | `code` |
|---|---|
| 400 | `financial_activities.invalid_date_range` |
| 400 | `financial_activities.invalid_page` |
| 400 | `financial_activities.invalid_filter_value` |
| 401 | `authentication.unauthorized` |

---

## 2. Planlanan hareket görünümü

> **Uygulandı (Grup 5).** Endpoint, allowlist, readiness/attention türetmesi,
> duplicate bastırma ve yaklaşan ödemelerin daraltılmış görünüme yeniden
> bağlanması çalışır durumdadır. Sözleşmeye uygulama sırasında `effect` alanı
> eklendi: daraltma kuralı (`gelir değil ve tahsilat değil`) buna dayanır ve
> istemci planlanan gelir ile gideri ayırt edebilir.

```text
GET /api/v1/financial-activities/planned?asOfDate=2026-08-14&daysAhead=30
```

`daysAhead` yalnız allowlist'teki **7, 30, 90** değerlerinden biridir.

### Enum'lar

- `plannedKind`: `recurring-occurrence`, `card-installment`, `card-statement`,
  `debt-installment`, `receivable-installment`, `payable-obligation`,
  `receivable-obligation`
- `timing`: `overdue`, `today`, `upcoming`
- `readiness`: `ready`, `needs-attention`
- `actionKind`: `realize`, `pay-card`, `pay-debt`, `collect-debt`,
  `pay-obligation`, `collect-obligation`
- `attentionCode`: `card-inactive`, `card-limit-insufficient`,
  `account-inactive`, `category-inactive`

`attentionCode`, Flutter'ın ağ sınırında kullanıcı mesajına çevirdiği **kararlı
makine kodudur**; teknik exception metni veya hassas veri taşımaz.

### Kimlik alanlarının adlandırması

Planlanan item iki farklı kimlik taşır ve bunlar karıştırılmamalıdır:

| Alan | Anlamı |
|---|---|
| `plannedActivityId` | Planlanan **kaydın** kimliği: occurrence id, installment item id, ekstre için kart id, borç/alacak taksidi id veya yükümlülük id |
| `sourceId` / `sourceName` | Paranın çıkacağı/gireceği **hesap veya kart** |

`sourceId`/`sourceName` böylece gerçekleşmiş feed'deki alanlarla aynı anlamı
taşır; Flutter iki ekranda aynı widget'ı kullanabilir. Eylem dispatch'i
`plannedActivityId` + `plannedKind` bileşimiyle yapılır — tıpkı feed'in
`activityKind:activityId` anahtarı gibi, kimlik tek başına tür belirtmez.

### Örnek cevap — `200 OK`

```json
{
  "asOfDate": "2026-08-14",
  "daysAhead": 30,
  "totalCount": 3,
  "nearestDueDate": "2026-08-15",
  "items": [
    {
      "plannedActivityId": "b31f7c48-0d92-4e56-a1c7-3f8b2d5e6a09",
      "plannedKind": "recurring-occurrence",
      "timing": "overdue",
      "readiness": "needs-attention",
      "attentionCode": "card-limit-insufficient",
      "actionKind": "realize",
      "dueDate": "2026-08-10",
      "amount": "149.9000",
      "currency": "TRY",
      "title": "Streaming aboneliği",
      "description": null,
      "sourceId": "c7e2f4a8-9b13-4d6e-8f52-1a3b5c7d9e01",
      "sourceName": "Test Kart",
      "categoryId": "3a9b0c11-5d2f-4e88-b7a6-0c1d4e5f6a72",
      "categoryName": "Subscriptions",
      "isProjected": false
    },
    {
      "plannedActivityId": "c7e2f4a8-9b13-4d6e-8f52-1a3b5c7d9e01",
      "plannedKind": "card-statement",
      "timing": "upcoming",
      "readiness": "ready",
      "attentionCode": null,
      "actionKind": "pay-card",
      "dueDate": "2026-08-15",
      "amount": "233.3333",
      "currency": "TRY",
      "title": "Test Kart ekstresi",
      "description": "2026-07 ekstresi",
      "sourceId": "c7e2f4a8-9b13-4d6e-8f52-1a3b5c7d9e01",
      "sourceName": "Test Kart",
      "categoryId": null,
      "categoryName": null,
      "isProjected": false
    },
    {
      "plannedActivityId": "7f0c3e91-8a25-4b64-9d17-2e6a4b8c0d53",
      "plannedKind": "recurring-occurrence",
      "timing": "upcoming",
      "readiness": "ready",
      "attentionCode": null,
      "actionKind": "realize",
      "dueDate": "2026-09-01",
      "amount": "18500.0000",
      "currency": "TRY",
      "title": "Maaş",
      "description": null,
      "sourceId": "5b8c1e02-6a4d-4f79-8c13-9d0e2f4a6b81",
      "sourceName": "Banka",
      "categoryId": "0b7d1a52-4c68-4e93-b5f0-1a2c3d4e5f60",
      "categoryName": "Salary",
      "isProjected": true
    }
  ],
  "upcomingOutgoingTotal": "233.3333",
  "unknownAmountCount": 0,
  "overdueOutgoingTotal": "149.9000",
  "overdueUnknownAmountCount": 0
}
```

`upcomingOutgoingTotal` pencere içinde vadesi henüz gelmemiş (bugün dahil)
**ödeme yükümlülüklerinin** toplamıdır: "bu pencerede benden ne çıkacak".
Tutarı henüz belli olmayan kalem (tutarsız vergi, ADR 0018 İ5) bu toplama
**tahminle katılmaz**; aynı dilimde kaç tane olduğu `unknownAmountCount`
alanında ayrıca döner ve istemci "2 kalemin tutarı belli değil" der. Böyle bir
kalemin `amount` alanı `null`'dır; vergi planının kalemi ayrıca `taxKind` ve
`recurringTransactionId` taşır (§5).
Tek karışık toplam değildir; gelir, tahsilat ve gecikmişler girmez (örnekte
gecikmiş abonelik ve maaş dışarıda kalır). Toplam sunucuda yapılır; Özet
ekranı `7 günde çıkacak` satırını buradan yazar.

`overdueOutgoingTotal` gecikmiş **ödeme yükümlülüklerinin** tutarı belli
olanlarının toplamıdır; tutarı belli olmayan gecikmiş kalemlerin sayısı
`overdueUnknownAmountCount`'tadır. Gecikmişlerin alt sınırı olmadığı için bu
toplam `daysAhead`'den bağımsızdır; `upcomingOutgoingTotal`'a eklenmez. Özet'in
Yaklaşanlar kartındaki "N gecikmiş ödeme" satırı bunu yazar.

Ekstre kaleminde `plannedActivityId` ile `sourceId` aynı kart kimliğidir; bu
beklenen durumdur, çünkü ekstrenin kendi kalıcı kimliği yoktur (ekstre bir
projection'dır, tablo değil).

`isProjected=true` olan kalemde hem `plannedActivityId` hem `actionTargetId`,
henüz occurrence üretilmediği için **recurring plan** kimliğidir. İstemci bu
satırı plan ve tarih adresleyen uçla gerçekleştirir:

```text
POST /api/v1/recurring-transactions/{planId}/occurrences/realize
{ "scheduledDate": "2026-08-31", "amount": "2450.7500" }
```

`amount` **isteğe bağlıdır** ve bu dönemin gerçek tutarıdır; boş bırakmak
"plandaki tutar doğru" demektir. Plandaki tutar bir **beklentidir** ve bazı
kalemlerde her dönem değişir (elektrik faturası, KDV beyanı, geçici vergi);
gönderilen tutar yalnız bu dönemin kaydına geçer, **planın tutarı değişmez**.
Aynı isteğe bağlı alan occurrence adresleyen uçta da vardır
(`POST /api/v1/recurring-transactions/occurrences/{occurrenceId}/realize`).
Okunamayan bir tutar `recurring.invalid_amount` ile reddedilir; sunucu bir değer
uydurmaz. Gerçekleşmiş bir occurrence'ın tutarı **yeniden yazılamaz** —
düzeltmesi iptal + yeni kayıttır.

`frequency` alanı `daily`, `weekly`, `monthly`, `quarterly`, `yearly` ve
`selected-months` değerlerini alır; `quarterly` üç ayda birdir ve ay sonu
davranışı aylıkla aynıdır. `selected-months` yalnız `months` listesindeki
aylarda, `dayOfMonth` gününde düşer (§5).

Sunucu eksik occurrence'ı kendi üretip gerçekleştirir; iki adım tek karara
iner. Önceki sözleşmede `actionTargetId` burada `null` idi ve istemcinin
gönderecek kimliği olmadığı için eylem hiç çalışmıyordu.

Tarihi gelmemiş bir kalem `recurring.not_due_yet` ile reddedilir: plan tarihi
gelene kadar bir tahmindir. **İstisna — ödeme günü verilirse** (`paidOn`,
vergi ekranının "Ödedim"i): kayıt vade gününe değil ödeme gününe yazılır ve
ödeme günü gelecekte olamaz, bu yüzden vadesi gelmemiş kalem de ödenebilir (§5).

### Sıralama ve kurallar

Gecikmişler önce; sonra vade artan; aynı tarihte tür ve kimlikle kararlı sıra.
Tek bir toplam **gösterilmez** — planlanan gelir, gider, ekstre ve nötr ödeme
yükümlülüğünü tek sayıda toplamak yanıltıcıdır.

`isProjected=true`, henüz kalıcı occurrence üretilmemiş recurring tarihi ayırt
eder; eylem dispatch'i önce occurrence generation yapar, sonra açık onayla
gerçekleştirir. Üretilmiş occurrence ile projected tarih **duplicate
gösterilmez**.

Tek seferlik yükümlülükte beklenen hareket settlement'tır ve `effect=neutral`
olur: gelir/gider `issueDate` tarihinde yükümlülük tarafından zaten tanınmıştır.
`sourceId`/`sourceName` boştur; ödeme/tahsilat hesabı henüz seçilmemiştir.
`actionTargetId` yükümlülük kimliğidir. Açık ve iptal edilmemiş satır `ready`,
settlement oluşmuş veya iptal edilmiş satır ise projection dışında olur.

### Kanonik kaynak

Bu projection planlanan hareketlerin tek gerçeğidir. Mevcut
`IUpcomingPaymentRepository` ayrı bir ikinci kaynak olarak korunmaz; aynı
projection'ın **daraltılmış görünümünü** okuyacak biçimde yeniden bağlanır.

Kapsam eşitliği beklenmez: yaklaşan ödemeler yalnız ödeme yükümlülüğü türlerini
(`recurring-occurrence` gider/fatura, `card-installment`, `card-statement`,
`debt-installment`, `payable-obligation`) içerir; recurring gelir,
`receivable-installment` ve `receivable-obligation` bu görünümün dışındadır.
Doğrulanan şey **ortak kalemlerin** tutar, tarih ve sıra bakımından sapmamasıdır.

`GET /api/v1/upcoming-payments` sözleşmesi geriye uyumlu kalır.

### Hata kodları

| HTTP | `code` |
|---|---|
| 400 | `planned_activities.invalid_as_of_date` |
| 400 | `planned_activities.invalid_days_ahead` |
| 401 | `authentication.unauthorized` |

---

## 3. Recurring source sözleşme genişlemesi

> **Uygulandı (Grup 3).** Bu bölümdeki create/list/realize sözleşmeleri ve hata
> kodları çalışır durumdadır.

Mevcut `RecurringTransaction` sözleşmesi **geriye uyumlu** biçimde genişler.

### İstek — geriye uyumluluk

```jsonc
// Mevcut istemci (sourceType yok) — kabul edilmeye devam eder
{ "accountId": "5b8c…", "categoryId": "3a9b…", "amount": "125.0000", … }

// Yeni: hesap kaynağı, açık
{ "sourceType": "account", "accountId": "5b8c…", "creditCardId": null, … }

// Yeni: kredi kartı kaynağı
{ "sourceType": "credit-card", "accountId": null, "creditCardId": "c7e2…", … }

// İsteğe bağlı toplam occurrence sınırı; endDate ile birlikte de kullanılabilir
{ "occurrenceLimit": 12, "endDate": null, … }
```

`sourceType` gönderilmediğinde `account` varsayılır ve `accountId` zorunludur.

### Doğrulama kuralları

| Kural | Hata kodu |
|---|---|
| `accountId` ve `creditCardId`'den tam olarak biri dolu | `recurring.invalid_source` |
| `kind=income` iken `sourceType=credit-card` | `recurring.income_card_source_not_supported` |
| Kart pasif | `recurring.card_inactive` |
| Kart/kategori/hesap yabancı veya yok | `404` |
| Kategori türü uyumsuz | `recurring.category_type_mismatch` |
| Para birimi eşleşmiyor | `recurring.currency_mismatch` |
| `occurrenceLimit <= 0` | `recurring.validation` |

`occurrenceLimit` toplam üretilecek occurrence sayısıdır. `endDate` ile birlikte
verilirse önce dolan sınır geçerlidir. Sınıra ulaşan plan silinmez;
`isActive=false`, `nextOccurrenceDate=null` olur. Liste cevabı sınırı
`occurrenceLimit`, bugüne kadar üretilen sayıyı `generatedOccurrenceCount`
alanında taşır.

### Örnek plan cevabı — kart kaynağı

```json
{
  "id": "b31f7c48-0d92-4e56-a1c7-3f8b2d5e6a09",
  "sourceType": "credit-card",
  "accountId": null,
  "creditCardId": "c7e2f4a8-9b13-4d6e-8f52-1a3b5c7d9e01",
  "categoryId": "3a9b0c11-5d2f-4e88-b7a6-0c1d4e5f6a72",
  "amount": "149.9000",
  "currency": "TRY",
  "kind": "bill-payment",
  "frequency": "monthly",
  "startDate": "2026-08-10",
  "endDate": null,
  "occurrenceLimit": 12,
  "generatedOccurrenceCount": 1,
  "nextOccurrenceDate": "2026-09-10",
  "monthEndBehavior": "clamp-to-last-day",
  "description": "Streaming aboneliği",
  "isActive": true
}
```

### Örnek occurrence cevabı — kart kaynağı, gerçekleşmiş

```json
{
  "id": "d92a5c07-1e38-4b6f-a054-8c2d7e9f1b46",
  "recurringTransactionId": "b31f7c48-0d92-4e56-a1c7-3f8b2d5e6a09",
  "occurrenceKey": "b31f7c480d924e56a1c73f8b2d5e6a09:20260810",
  "sourceType": "credit-card",
  "accountId": null,
  "creditCardId": "c7e2f4a8-9b13-4d6e-8f52-1a3b5c7d9e01",
  "categoryId": "3a9b0c11-5d2f-4e88-b7a6-0c1d4e5f6a72",
  "amount": "149.9000",
  "currency": "TRY",
  "kind": "bill-payment",
  "scheduledDate": "2026-08-10",
  "description": "Streaming aboneliği",
  "status": "realized",
  "budgetTransactionId": null,
  "creditCardChargeId": "4e8b1f26-7a03-4c95-8d61-3b0f5a7c2e18",
  "realizedAtUtc": "2026-08-10T06:03:44.1270000+00:00"
}
```

Gerçekleştirmede `budgetTransactionId` ve `creditCardChargeId`'den **tam olarak
biri** doludur.

### Realize cevabı

```text
POST /api/v1/recurring-transactions/occurrences/{occurrenceId}/realize
```

Sonuç türü kaynağa göre değiştiği için cevap iki tarafı olan bir zarftır;
`sourceType` hangi tarafın dolu olduğunu söyler.

```json
{
  "sourceType": "credit-card",
  "transaction": null,
  "charge": {
    "id": "4e8b1f26-7a03-4c95-8d61-3b0f5a7c2e18",
    "creditCardId": "c7e2f4a8-9b13-4d6e-8f52-1a3b5c7d9e01",
    "categoryId": "3a9b0c11-5d2f-4e88-b7a6-0c1d4e5f6a72",
    "amount": "149.9000",
    "currency": "TRY",
    "chargeDate": "2026-08-10",
    "description": "Streaming aboneliği",
    "isCancelled": false,
    "cancelledAtUtc": null
  }
}
```

Hesap kaynağında tam tersi olur: `sourceType` `"account"`, `charge` null ve
`transaction` doludur. Aynı occurrence yeniden gerçekleştirildiğinde ikinci
istek mevcut sonucu döndürür, yeni kayıt üretmez.

### Realize hata kodları

| HTTP | `code` | Durum |
|---|---|---|
| 409 | `recurring.card_limit_insufficient` | Kullanılabilir limit yetersiz; occurrence `planned` kalır |
| 409 | `recurring.card_inactive` | Kart pasif; occurrence `planned` kalır |
| 409 | `recurring.already_realized` | Farklı bir sonuçla zaten gerçekleşmiş |

Limit veya pasiflik hatasında **hiçbir kısmi kayıt** oluşmaz.

---

## 4. Origin-aware iptal sözleşmesi

> **Uygulandı (Grup 4).**

Mevcut iptal endpoint'leri korunur, davranışları daraltılır:

```text
DELETE /api/v1/transactions/{transactionId}
DELETE /api/v1/credit-card-charges/{chargeId}
```

Hareketin `RecurringTransactionOccurrence` veya `InstallmentItem` bağlantısı
varsa iptal reddedilir:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflict",
  "status": 409,
  "detail": "Tekrarlayan plandan üretilmiş hareket iptal edilemez.",
  "code": "transactions.cancel_origin_locked"
}
```

Kart harcaması için `credit_card_charges.cancel_origin_locked`.

Manuel, `csv-import`, transfer ve kart ödemesi iptali **mevcut kurallarla
çalışmaya devam eder** — bu bir regresyon kapısıdır. Bir vergi ödemesinin
iptali, kapattığı kalemleri aynı `SaveChanges` sınırında bekleyene döndürür
(§5). Borç/alacak endpoint'inde
zaten geri alma yolu yoktur; `canCancel=false` sabittir.

---

## 5. Vergi planı (Aşama 06.3 Grup 3, ADR 0018)

> **Uygulandı (backend).** Vergi ayrı bir kayıt türü değildir (İ6): tanımlı
> vergi, `taxKind` alanı dolu bir tekrarlayan plandır; ödenen vergi vergi
> işaretli kategorideki bir gider ya da kart harcamasıdır. Aşağıdaki alanlar
> §3'ün sözleşmesini geriye uyumlu biçimde genişletir; sıradan plan eskisi gibi
> çalışır.

### Plan alanları

| Alan | Anlamı |
|---|---|
| `taxKind` | `social-security-premium`, `vat-return`, `withholding-return`, `advance-tax`, `annual-income-tax`, `property-tax`, `motor-vehicle-tax`, `advertising-tax`, `custom`; sıradan planda `null`. Adı ve ipucunu istemci kurar |
| `amount` | Beklenen tutar; **yalnız vergi planında `null` olabilir** (tutarı ödeme gününe kadar bilinmeyen vergi, İ5). Sıfır hâlâ reddedilir |
| `sourceType`, `accountId`, `creditCardId` | Yalnız vergi planında üçü birden boş olabilir; kaynak ödemede seçilir (T4). Sıradan planda kural değişmedi (ADR 0005) |
| `frequency` | Yeni değer `selected-months` |
| `months` | `selected-months` ritminin ayları (1–12); diğer ritimlerde `null` |
| `dayOfMonth` | Ayın günü (1–31); boşsa başlangıç günü. `31` ay sonudur: kısa ayda son gün, başlangıç 30 Eylül olsa bile sonraki kalem 31 Ekim |

`startDate` ritmin ilk kalemidir ve ritme uymalıdır (seçili bir ayda, `dayOfMonth`
gününde ya da ay sonuna sıkıştırılmış). Vergi planının adı (`description`)
zorunludur: aynı kategoride Bağkur, KDV ve geçici vergi yan yana durur. Vergi
planının kategorisi **vergi işaretli bir gider kategorisi** olmalıdır
(`recurring.category_not_tax`).

**Kapsam** (İ9, 30 Eylül 2026'da güncellendi): açık seçim → profilin tarafı
(işletmesi olan kullanıcıda `business`, olmayanda `personal`). **Hesabın ya da
kartın etiketine bakılmaz**; işletme vergisi şahsi kartla ödenebilir. Sonuç hiç
boş değildir. Bu kural vergi planına, toplu tanımlamaya ve toplu vergi ödemesine
uygulanır; istemci seçimi vergi tanımında sunar.

### Uçlar

```text
PUT  /api/v1/recurring-transactions/{planId}                         düzenleme
POST /api/v1/recurring-transactions/{planId}/occurrences/realize     "Ödedim" (paidOn, accountId|creditCardId)
POST /api/v1/recurring-transactions/occurrences/{occurrenceId}/realize
POST /api/v1/recurring-transactions/{planId}/occurrences/amount      "tutar belli oldu"
POST /api/v1/recurring-transactions/occurrences/{occurrenceId}/undo  "Ödedim"i geri al
GET  /api/v1/tax-calendar/suggestions                                hazır türler
GET  /api/v1/taxes?asOfDate=…&daysAhead=30                           vergi ekranı
POST /api/v1/taxes/plans                                             "Vergilerimi tanımla" (toplu)
GET  /api/v1/taxes/plans/{planId}?asOfDate=…                         vergi tanımı ayrıntısı
GET  /api/v1/tax-payments?skip=0&take=20                             Ödenenler › Tümü
POST /api/v1/tax-payments                                            "Vergi ödemesi ekle" (toplu)
POST /api/v1/tax-payments/{paymentId}/undo                           ödemeyi geri al
```

**Düzenleme** planın tam hâlidir (tür, vergi türü, para birimi değişmez). Ritim
alanlarından biri (`frequency`, `startDate`, `endDate`, `monthEndBehavior`,
`dayOfMonth`, `months`) değişirse plan yeni ritimle `startDate` gününden yeniden
başlar: bekleyen kalemler tahmindir ve yeniden kurulur, ödenmiş ve kapatılmış
kalemler geçmiş olarak kalır. Yeni başlangıç son ödenen ya da kapatılan
kalemden sonra olmalıdır (`recurring.reschedule_before_history`; kullanıcı
kararı, 30 Eylül 2026). Ritim değişmezse bekleyen kalemler planın yeni adını,
kaynağını, kategorisini ve kapsamını alır; tutar yalnız plandan gelmişse değişir
— kullanıcının o dönem için yazdığı tutar korunur.

**"Ödedim"** gövdesi `{ "scheduledDate", "amount"?, "paidOn"?, "accountId"?,
"creditCardId"? }`. `paidOn` verilirse kayıt **ödeme gününe** yazılır ve vadesi
gelmemiş kalem de ödenebilir; `paidOn` gelecekte olamaz
(`recurring.paid_on_in_future`; sunucu UTC günü bir gün payla karşılaştırır).
Kaynak verilirse kalemin kaynağını geçersiz kılar; kartla ödeme kart harcaması
yazar. Tutarı olmayan kalemde `amount` zorunludur
(`recurring.amount_required`), kaynağı olmayan kalemde hesap ya da kart
(`recurring.source_required`).

**"Tutar belli oldu"** gövdesi `{ "scheduledDate", "amount" }`; bekleyen kaleme
yazar, ileri bir tarih için de. Plan değişmez.

**Geri alma** ürettiği kaydı iptal eder (silmez) ve kalemi bekleyene döndürür;
kalemin tutarı kalır. Birleşik akıştaki köken kilidi
(`*.cancel_origin_locked`) yerinde kalır: planın ürettiği kayıt İşlemler'den
iptal edilmez, geri alma bu uçtandır. Toplu ödemeyle kapatılmış kalem buradan
geri alınmaz (`recurring.closed_by_payment`).

**Toplu tanımlama** gövdesi `{ "items": [ … ] }`; her öğe tek plan
oluşturmanın isteğidir (`POST /api/v1/recurring-transactions`) ve `taxKind`
taşır. En çok 20 öğe. Her öğe tek planla aynı kuralla doğrulanır; biri
geçersizse **hiçbiri yazılmaz** (tek `SaveChanges`) ve ilk hatanın kodu döner.
Cevap `201 Created` + `{ "items": [plan…] }`. Vergi türü boş öğe
`taxes.plan_not_tax` ile reddedilir.

### Kalem durumu `closed`

Occurrence `status` alanı yeni `closed` değerini alır: kalem toplu bir vergi
ödemesiyle kapatıldı, kendi sonucu yoktur, kapatan ödemenin kimliğini
`closedByTransactionId` ya da `closedByChargeId` alanında, zamanını
`closedAtUtc`'de taşır. Bir ödeme birden çok kalemi kapatabilir; bir kalem tam
olarak tek sonuç taşır (İ7). Kapatılmış kalem "Ödedim" olamaz
(`recurring.already_settled`) ve planlanan görünümde görünmez.

### Toplu vergi ödemesi

```json
{
  "clientRequestId": "0e6f8a2c-5b1d-4c7e-9f30-2a4b6c8d0e12",
  "amount": "17900.0000",
  "paidOn": "2026-09-28",
  "categoryId": "3a9b0c11-5d2f-4e88-b7a6-0c1d4e5f6a72",
  "accountId": "5b8c1e02-6a4d-4f79-8c13-9d0e2f4a6b81",
  "creditCardId": null,
  "scope": null,
  "note": "Temmuz–Ağustos Bağkur",
  "closes": [
    { "recurringTransactionId": "b31f7c48-0d92-4e56-a1c7-3f8b2d5e6a09", "scheduledDate": "2026-07-31" },
    { "recurringTransactionId": "b31f7c48-0d92-4e56-a1c7-3f8b2d5e6a09", "scheduledDate": "2026-08-31" }
  ]
}
```

Hiçbir vergi tanımlamadan tek tutarla ödeme (İ4): `closes` boş olabilir. Hesap
ya da kart tam olarak biri; kategori vergi işaretli olmalıdır. Tutar kalemlere
dağıtılmaz ve eşleştirilmez. Seçilen kalemler — henüz üretilmemiş olsalar da —
`closed` olur. Ödeme ve kapatma tek `SaveChanges` sınırında yazılır.

**İdempotentlik:** ödemenin kimliği `clientRequestId` ile kullanıcı kimliğinden
türetilir; aynı istek tekrar gelirse ikinci gider yazılmaz, ilk ödeme döner. İki
kullanıcının aynı istek kimliği iki ayrı ödeme kimliği üretir.

Cevap (`201 Created`) ve Ödenenler satırı aynı biçimdedir:

```json
{
  "paymentId": "8f1a2b3c-4d5e-8f60-9a1b-2c3d4e5f6a7b",
  "sourceType": "account",
  "sourceId": "5b8c1e02-6a4d-4f79-8c13-9d0e2f4a6b81",
  "sourceName": "Dükkan hesabı",
  "categoryId": "3a9b0c11-5d2f-4e88-b7a6-0c1d4e5f6a72",
  "categoryName": "SGK ve vergi ödemesi",
  "amount": "17900.0000",
  "currency": "TRY",
  "paidOn": "2026-09-28",
  "description": "Temmuz–Ağustos Bağkur",
  "scope": "business",
  "realizedItem": null,
  "closedItems": [
    { "occurrenceId": "…", "recurringTransactionId": "b31f…", "scheduledDate": "2026-07-31", "name": "Bağkur", "taxKind": "social-security-premium" },
    { "occurrenceId": "…", "recurringTransactionId": "b31f…", "scheduledDate": "2026-08-31", "name": "Bağkur", "taxKind": "social-security-premium" }
  ],
  "isCancelled": false
}
```

`realizedItem` doluysa ödeme bir kalemin "Ödedim" sonucudur. **Geri alma**
(`POST /api/v1/tax-payments/{paymentId}/undo`) doğru yolu kendisi seçer:
"Ödedim" sonucuysa kalemin geri alması, değilse ödemenin iptali ve kapattığı
kalemlerin bekleyene dönmesi. Aynı sonuç İşlemler'den iptalle de alınır:
`DELETE /api/v1/transactions/{id}` ve `DELETE /api/v1/credit-card-charges/{id}`
bir vergi ödemesinin kapattığı kalemleri aynı sınırda açar. Taksit planının
kaydı bu uçla geri alınmaz (`tax_payments.undo_origin_locked`).

### Vergi ekranı okuması

`GET /api/v1/taxes` tanımlı vergileri (`plans`), gecikmiş ve pencere içindeki
kalemleri (`pending`) ve son beş ödemeyi (`recentPayments`, `hasMorePayments`)
döner. `pending` **planlanan projection'ın daraltılmış görünümüdür** (İ6): vergi
için ikinci bir sorgu yoktur; satırlar §2'nin şeklindedir. `pendingTotal`
bekleyenlerin tutarı belli olanlarının toplamıdır ve **gecikenleri de içerir**
(ekranın kartı "Gecikenler ve 30 gün"dür); `pendingUnknownAmountCount` tutarı
belli olmayanların sayısıdır, yine gecikenler dahil. "Ödenenler" vergi
işaretli kategorilerdeki iptal edilmemiş giderler ve kart harcamalarıdır; gider
formundan girilen vergi de oradadır (T6).

Tanım ayrıntısı (`/api/v1/taxes/plans/{planId}`) `upcoming`'de gecikmiş
kalemlerin hepsini ve vadesi gelmemiş **ilk üç** kalemi döner (pencere değil
sayı: yılda bir ödenen vergide de dolar; ufuk üç yıl, yalnız o plan
izdüşürülür). `history` ödenen ya da kapatılan her kalemi ödemesiyle ve kalemin
kendi beklenen tutarıyla (`amount`, boş olabilir) döner.

### Hazır türler

`GET /api/v1/tax-calendar/suggestions` her tür için `taxKind`, `frequency`,
`months` ve `dayOfMonth` döner; **tutar, kategori ve kapsam taşımaz** (V-K3).
Önceki şekil (`key`, `suggestedCategoryName`, `kind`, `scope`) ve 30 Eylül'de
`personalScopeAllowed` kalktı (kapsam her vergide seçilebilir, İ9); bu uç yalnız
bu uygulamanın istemcisi tarafından okunuyor.

### Hata kodları

| HTTP | `code` |
|---|---|
| 400 | `recurring.category_not_tax`, `recurring.amount_required`, `recurring.source_required`, `recurring.paid_on_in_future`, `recurring.reschedule_before_history`, `recurring.invalid_tax_kind`, `recurring.invalid_months`, `recurring.invalid_paid_on` |
| 409 | `recurring.already_settled`, `recurring.closed_by_payment`, `recurring.concurrent_change` |
| 400 | `tax_payments.invalid_source`, `tax_payments.category_not_tax`, `tax_payments.paid_on_in_future`, `tax_payments.item_not_tax`, `tax_payments.invalid_amount`, `tax_payments.invalid_page` |
| 400 | `taxes.plans_empty`, `taxes.too_many_plans`, `taxes.plan_not_tax` (ve tek plan oluşturmanın bütün `recurring.*` kodları) |
| 404 | `tax_payments.item_not_found`, `tax_payments.not_found`, `taxes.plan_not_found` |
| 409 | `tax_payments.item_not_pending`, `tax_payments.item_inactive`, `tax_payments.card_limit_insufficient`, `tax_payments.concurrent_change`, `tax_payments.undo_origin_locked` |

---

## Migration ve backup notu

Bu sözleşmeyi kuran migration'lar artık ayrı dosyalar olarak **yok**: şema tek
bir `InitialCreate` ile kuruluyor ve devralınan zincir taşınmadı (ADR 0012).
Sözleşmenin şema karşılığı bugünkü modelde birebir duruyor; buradaki adım adım
yükseltme planı tarihsel değerini yitirdiği için kaldırıldı.

Yedek şemasının güncel sürümü ve hangi sürümlerin okunduğu tek yerde,
`documentation/restore-runbook.md` içinde tutulur.

## Test matrisi

### Domain

| # | Beklenti |
|---|---|
| D1 | Recurring planda account/card source'tan tam biri seçilir |
| D2 | `kind=income` + kart source reddedilir |
| D3 | Gider/fatura + aktif owner kart kabul edilir |
| D4 | Inactive/foreign kart reddedilir |
| D5 | Yanlış kategori türü ve currency reddedilir |
| D6 | Occurrence tam olarak tek sonuç kimliği taşır |
| D7 | İkinci realization aynı sonucu döndürür veya conflict'i güvenli çözer |

### Application ve API

| # | Beklenti |
|---|---|
| A1 | Her activity türü doğru effect/sourceGroup/origin/status'e eşlenir |
| A2 | Owner kimliği yokken repository hiç çağrılmaz |
| A3 | Foreign account/card/category/activity `404` görünür |
| A4 | Inclusive tarih sınırı ve bütün filtre kombinasyonları doğru |
| A5 | Amount/currency/date string sözleşmesi kayıpsız |
| A6 | Kart recurring limit yetersizliğinde kısmi kayıt oluşmaz |
| A7 | `canCancel` yalnız desteklenen tür+origin+status'te `true` |
| A8 | Planned `actionKind` türleri doğru üretilir |
| A9 | `recurring`/`installment`/borç kaynaklı iptal isteği `409` döner |
| A10 | Manual/csv-import/transfer/kart ödemesi iptali regresyonsuz çalışır |
| A11 | `daysAhead` allowlist dışı değer `400` döner |

### Infrastructure ve gerçek SQL

| # | Beklenti |
|---|---|
| I1 | Migration backfill sonrası eski recurring kayıtları account source kalır |
| I2 | Source ve realization check constraint'leri ihlali reddeder |
| I3 | Composite owner FK negatif senaryoları reddeder |
| I4 | Aynı occurrence için yarışan iki realization tek sonuç bırakır |
| I5 | Feed deterministik sayfalama ve doğru total count üretir |
| I6 | N+1 yok; bounded query-count |
| I7 | **Sabit sayfa isteğinde okunan satır sayısı, fixture toplam kayıt sayısı büyürken sabit kalır** (bellekte birleştirmeyi yakalar) |
| I8 | Backup v2 restore ve v3 round-trip geçer |
| I9 | Restore hatasında SQL/object state rollback veya telafi uygulanır |

### Finansal doğruluk

| # | Beklenti |
|---|---|
| F1 | Account gideri hesabı ve gideri etkiler |
| F2 | Card charge kart borcunu, gideri ve bütçeyi etkiler; account bakiyesini etkilemez |
| F3 | Card payment account ve kart borcunu etkiler; gideri etkilemez |
| F4 | Transfer iki hesabı etkiler; gelir/gider/net toplamı bozulmaz |
| F5 | Borç/alacak likiditeyi etkiler; gelir/gideri bozmaz |
| F6 | Planned kayıt hiçbir gerçekleşmiş toplama girmez |
| F7 | Cancelled hareket bütün hesaplanan etkilerden çıkar |
| F8 | Aynı ekonomik olay feed ve raporda yalnız bir kez sayılır |

### Flutter

| # | Beklenti |
|---|---|
| M1 | Activity JSON parse; bilinmeyen enum/bozuk para/tarih reddi |
| M2 | Loading, empty, error, retry, unauthorized, stale snapshot |
| M3 | Hızlı filtreler ve gelişmiş filtre temizleme |
| M4 | Varsayılan açılışta tarih filtresi gönderilmez; `Bu ay`/`Son 3 ay`/`Son 12 ay`/`Tümü` doğru aralık üretir |
| M5 | Pagination ve duplicate activity key savunması |
| M6 | Sade kart içeriği ve türe göre Semantics |
| M7 | Bottom sheet alanları; desteklenmeyen eylemler gizli |
| M8 | İptal onayı/vazgeçme/başarı yenilemesi |
| M9 | Launcher doğru forma ve doğru repository metoduna dispatch eder |
| M10 | Gider source picker hesap/kart gruplarını gösterir |
| M11 | Recurring income'da kart seçeneği bulunmaz |
| M12 | Recurring expense/bill kart seçimi; limit/inactive hata görünümü |
| M13 | Planlanan özet kartı, ayrı route ve 7/30/90 filtreleri |
| M14 | Büyük metin, dar/geniş ekran, NavigationBar/Rail, ekran okuyucu |
| M15 | İlgili mutation'lardan sonra feed otomatik yenilenir |

## Değişiklik bildirimi hedefleri

Mevcut üç revision sayacı (`dashboard`, `budgets`, `transactions`) yetersizdir.
Hedefli kapsam:

```text
activityFeed · dashboard · budgets · accounts · cards · planning
```

| Mutation | Yenilenecek hedefler |
|---|---|
| Gelir/gider oluşturma, iptal | activityFeed, dashboard, budgets, accounts |
| Transfer oluşturma, iptal | activityFeed, dashboard, accounts |
| Kart harcaması, iptal | activityFeed, dashboard, budgets, cards |
| Kart ödemesi, iptal | activityFeed, dashboard, accounts, cards |
| Taksit gerçekleştirme | activityFeed, dashboard, budgets, cards, planning |
| Recurring occurrence gerçekleştirme | activityFeed, dashboard, budgets, accounts, cards, planning |
| CSV confirm | activityFeed, dashboard, budgets, accounts |
| Borç ödeme / alacak tahsilatı | activityFeed, dashboard, accounts, planning |
| Backup restore | hepsi |

Bildirim yalnız yeniden okuma sinyalidir; bakiye veya raporun gerçek kaynağı
değildir.
