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
| `pos-commission` | `PosSettlement` — aynı gün tanınan komisyon gideri; komisyon `0` ise satır **yoktur** |
| `pos-transfer` | `PosSettlement` — paranın hesaba geçtiği an; **net** tutar, `neutral`. Geçmemiş tahsilatta satır yoktur |

Bir POS tahsilatı feed'de **üç satırdır** ve üçü de aynı `activityId`'yi taşır.
Tek satıra indirilseydi ya komisyon görünmez olurdu ya da hesabın bakiyesindeki
artışın günü yanlış yazılırdı: gelir tahsilat günü, para ise geçiş günü
gerçektir. İstemci satırı `activityKind + activityId` ikilisiyle anahtarlar.

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
| POS komisyonu | `pos-commission` | `expense` | `pos` | **hayır** |
| POS parasının hesaba geçmesi | `pos-transfer` | `neutral` | `pos` | **hayır** |

`canCancel` formülü:

```text
canCancel = status == realized
         && activityKind ∉ { debt-payment, debt-collection, debt-opening,
                             obligation, obligation-settlement,
                             pos-sale, pos-commission, pos-transfer }
         && origin ∉ { recurring, installment }
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

POS tahsilatının üç satırı da feed üzerinden **iptal edilemez**: üçü tek
kaydın anlarıdır ve birini iptal etmek diğer ikisini sahipsiz bırakırdı. İptal,
kaydın kendi ekranından tek eylemle yapılır ve üç satırı birlikte kapatır.

`pos-transfer` **kapsam taşımaz** (`scope: null`): parayı taşır, gelir/gider
üretmez (ADR 0014). Kapsam filtreli okumada düşer; `pos-sale` ve
`pos-commission` kapsam taşır ve kalır. Komisyon brüt tutardan **düşülmez**:
gelir brüt kadar tanınır, komisyon kendi kategorisinde ayrı bir giderdir.

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
  ]
}
```

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

`frequency` alanı `daily`, `weekly`, `monthly`, `quarterly` ve `yearly`
değerlerini alır; `quarterly` üç ayda birdir (geçici verginin ritmi) ve ay sonu
davranışı aylıkla aynıdır.

Sunucu eksik occurrence'ı kendi üretip gerçekleştirir; iki adım tek karara
iner. Önceki sözleşmede `actionTargetId` burada `null` idi ve istemcinin
gönderecek kimliği olmadığı için eylem hiç çalışmıyordu.

Tarihi gelmemiş bir kalem `recurring.not_due_yet` ile reddedilir: plan tarihi
gelene kadar bir tahmindir.

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
çalışmaya devam eder** — bu bir regresyon kapısıdır. Borç/alacak endpoint'inde
zaten geri alma yolu yoktur; `canCancel=false` sabittir.

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
