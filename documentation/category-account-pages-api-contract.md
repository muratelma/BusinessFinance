# Kategori ve hesap sayfaları API sözleşmesi

13 Ekim 2026. Bütün uçlar JWT ister; sahip oturumdan gelir. Para, pay ve
oranlar dört ondalıklı dizedir. Cümleler ve işlem adları istemcide üretilir.
Listeler 50 satırlık sayfalarla gelir; `offset`, `totalCount`, `hasMore` taşır.

## Okumalar

| Uç | Seçimler / anlam |
|---|---|
| `GET /api/v1/category-summary` | `year`, `month`, `direction=expense|income`, `scope=business|personal`, `source` |
| `GET /api/v1/category-summary/months` | Aynı seçimle yılın 12 ayı |
| `GET /api/v1/categories/{id}/page` | Aynı seçimler, ayrıca `person`, `offset` |
| `GET /api/v1/categories/{id}/months` | Kategori sayfasının seçimleriyle yılın ayları |
| `GET /api/v1/categories/uncategorized/page` | Kategorisiz geçmiş; salt okunur |
| `GET /api/v1/categories/uncategorized/months` | Kategorisiz geçmişin ayları |
| `GET /api/v1/accounts/{id}/page` | `year`, `month`, `direction=in|out`, `selection`, `offset`; kapsam yok |
| `GET /api/v1/accounts/{id}/months` | `year`; ay sonu bakiyeleri, kapsam yok |
| `GET /api/v1/accounts/{id}/in-transit` | `offset`; bugünkü yoldaki satış ve kartla tahsilatlar |
| `GET /api/v1/accounts/overview` | Bütün hesaplar; aktif Nakit/Banka ve Pasif toplamları ile toplam içindeki payları |
| `GET /api/v1/financial-activities/{kind}/{id}/parts` | Ortak işlem ayrıntısındaki masraf/faiz/komisyon/kesinti ve kaydın gerçek kategorisi |

Yıl/ay verilmezse sunucunun Türkiye günü kullanılır. Geçersiz seçim 400
`finance_pages.invalid_filter`; başka sahibin kategori/hesap kimliği ile hiç
olmayan kimlik aynı 404 `finance_pages.not_found` cevabını alır.

`source`: boş = hepsi; `account:<guid>`, `card:<guid>`, `deferred`.
`person`: boş = hepsi; `person:<guid>`, `none`. Her menü öteki seçimin
kesişimini toplar. Seçili ve sahibine ait bir kaynak kayıtsız ayda sıfırla
korunur. Pasif kaynak, kaydı olan ayın menüsünde görünür.

`accounts/overview` cevabındaki `cashShare`, `bankShare` ve `inactiveShare`
tam yüzdedir (`"58.0000"` = %58) ve Hesaplar'daki pay çubuğu ile yüzdeleri
besler; istemci pay hesaplamaz. Toplam sıfır ya da eksiyse veya parçalardan
biri eksiyse üçü de `null` gelir ve çubuk çizilmez.

Hesap `selection` anahtarı `in|out:category|collection|payment|transfer`
biçimindedir. Alt seçenek kategori veya karşı hesap kimliğini, ödeme/tahsilat
türünde `FinancialActivityKind` sayısını ekler (`out:payment:5`, borç taksidi).
`options.kind` kararlı işlem türü adıdır; görünen adı istemci ortak sözlüğünden
okur. `all` menü satırının tutarı seçeneklerin mutlak toplamıdır. Hesap menüsünün
sırası sabittir: `all`, sonra giren, sonra çıkan; gruplar `category`,
`collection`/`payment`, `transfer`; grubun içinde ada göre (Türkçe sıralama).

## Örnek kategori özeti

```json
{
  "year": 2026, "month": 9, "direction": "expense", "scope": "business",
  "total": "26930.0000", "previousTotal": "0.0000", "categoryCount": 7,
  "categories": [
    {"id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "name": "Ticari mal alımı",
     "amount": "12480.0000", "share": "46.0000", "barRatio": "0.4634"}
  ],
  "sources": [
    {"key": "all", "group": "all", "name": null, "amount": "26930.0000", "kind": null, "id": null},
    {"key": "deferred", "group": "deferred", "name": null, "amount": "3610.0000", "kind": null, "id": null}
  ]
}
```

Örnek listeler kısaltılmıştır. Gerçek cevapta bütün kategori ve menü
satırlarının toplamı `total` / `all` tutarına eşittir; kategorisiz tutar da
`categories` içinde `id=null`, `name=null` satırıyla bulunur.

Kategori sayfası `summary`, `categoryId`, `name`, `wholeTotal`, `showScope`,
`hasPeople`, `people`, `items`, `totalCount`, `offset`, `hasMore` taşır.
`wholeTotal` aynı yön/ayın kategori toplamıdır; kapsam, kaynak ve kişi seçimi
yoktur. `items.activity` mevcut birleşik işlem cevabının aynısıdır; ayrıntının
kimliği ve kilitleri korunur. `amount` sayfada sayılan işaretli tutardır;
`part=commission|deduction|interest|fee`, `partAmount` ve `parentAmount` parçayı
ve bağlı işlemi açıklar. `state=open|settled|transit|deposited` gerektiğinde
gelir; `settledOn`, `settledSource` kapanış bilgisidir. Cari borçlandırmada
kapanış durumu yoktur. `showScope` iki kapsam birlikte gösterildiğinde gelir.
`items.scope=business|personal|null` kalemin kendi kapsamıdır; nötr bağlı
transfer/POS/taksit ebeveyninin kapsamından alınmaz. İstemci satır kapsamını
bu alanla gösterir; `settled` kapanış, `fee` transfer masrafı değeridir.

Sentetik tam cevaplar ve filtreli örnekler
`mobile/business_finance_mobile/test/fixtures/category-account-contract.json`
dosyasındadır; yalnız örnek veridir, secret içermez.

## Örnek hesap özeti

```json
{
  "balance": "21376.0000", "currentBalance": "21376.0000",
  "isCurrentMonth": false, "inflow": "27936.0000", "outflow": "31560.0000",
  "net": "-3624.0000", "selectionTotal": "59496.0000",
  "transit": {"total": "8074.0000", "count": 1, "nearest": null},
  "totalCount": 13, "offset": 0, "hasMore": false
}
```

Örnek satırlar kısaltılmıştır (`nearest` gerçek cevapta en yakın kaydı taşır).
Hesap cevabı ayrıca hesabın kimliği/adı/türü/varsayılan kapsamı/aktifliğini,
yıl/ayı, `options` ve `items` taşır. Geçmiş ayda `balance` ay sonudur;
`currentBalance` bugünkü havuz bakiyesidir. Satırın `amount`u hesabın gerçek
değişimidir; transfer çıkışı masraf dahil, yatış kesinti sonrası, taksit faiz
dahildir. Yatış satırı `firstSaleDate`, `lastSaleDate` taşır. Yolda özeti bütün
geçmemiş kayıtları sayar; ay seçimi onu daraltmaz.

Ay cevabı `year`, 12 `months` satırı (`month`, `amount`, `barRatio`, `enabled`)
taşır. Kayıtsız ileri ay kapalıdır. Kategoride çubuk gelir/gider tutarı;
hesapta ay sonu bakiyesidir. Hesabın açıldığı gün saklanmadığı için ilk kaydın ayından
(kaydı yoksa bu aydan) önceki aylar `enabled` kalır ve ay sonu bakiyesini
taşır ama `barRatio` 0'dır; oran yalnız sonraki aylardan hesaplanır.

## Transfer masrafı ve rol

`POST /api/v1/transfers` isteğine isteğe bağlı `feeAmount` ve `feeScope`
eklenmiştir. Boş masraf eski davranıştır. Masraf negatif olamaz; ileri gün,
pasif/yabancı hesap ve çözülemeyen kapsam reddedilir. Masraf çıkan hesaptan,
banka masrafı rolündeki gider kategorisine tek yazmada kaydedilir. Cevap
`feeTransactionId`, `feeAmount`, `feeCategoryId`, `feeScope` taşır. Transferle
birlikte iptal olur; gider kimliğiyle tek başına iptal 409 döner.

`POST /api/v1/transfers/preview` aynı isteği alır, kayıt yazmaz:

```json
{"sourceAccountId":"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
 "sourceAccountName":"Ziraat Vadesiz","totalOutflow":"10050.0000",
 "feeAmount":"50.0000","feeCategoryId":null,"feeCategoryName":"Banka masrafı",
 "resolvedScope":"business","needsScope":false}
```

Kategori cevabında `role=interest-expense|interest-income|cash-difference|bank-fee|null`
vardır. Rol kategorisinin adı değişebilir; kapsamı iki tarafa açık kalır.
`POST /api/v1/categories/bank-fee` oturum sahibinin rol kategorisini bulur,
yoksa açar ve kategori cevabını döner (dekontun bağımsız ücret gideri yolu).

`parts` cevabı `items` dizisinde `kind`, dört ondalıklı `amount`,
`categoryId`, `categoryName` taşır. Parça yoksa dizi boştur. Transferin masrafı,
taksidin faizi, kartlı satış/tahsilatın komisyonu ve yatışın kesintisi aynı
okumadan ayrıntıya eklenir; ad, rol ya da kategori üzerinden istemcide
tahmin edilmez. Başka sahibin işlem kimliği ve hiç olmayan kimlik aynı 404'tür.

## Kaynak ve sayfalama sınırı

Gelir/gider `RecognizedItems`, hesap tutarları `AccountMovements` üzerinden
SQL'de süzülür ve toplanır. Geçmişin tamamı istemciye veya belleğe çekilmez.
Sayfa görüntüsündeki ortak işlem bilgisi yalnız o sayfanın kimlikleriyle
birleşik projeksiyondan okunur; kayıt başına sorgu yoktur. Hesap sayfası
yoldaki özet için yalnız en yakın kaydı okur; tam liste ayrı sayfalıdır.

## Kaynak ikonları için hesap türü — 13 Ekim 2026

Kaynak seçeneklerinde group=accounts iken mevcut kind alanı cash ya da bank döner. Alan aynı kaynakta sıfır sonuçlu seçimi korurken de doldurulur. Kimlik ve ad yalnız oturum sahibinin hesabından gelir. Eski istemciler isteğe bağlı alanı yok sayabilir; şema veya kırılan sözleşme değişikliği yoktur.
