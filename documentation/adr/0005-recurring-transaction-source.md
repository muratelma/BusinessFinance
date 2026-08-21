# ADR 0005 — Tekrarlayan Planın Kaynağı: Hesap veya Kredi Kartı

- Durum: Kabul edildi ve uygulandı
- Tarih: 2026-08-14
- Kapsam: `RecurringTransaction`, `RecurringTransactionOccurrence`, backup şeması

## Bağlam

Stage 11'de `RecurringTransaction` yalnız bir `AccountId` taşıyor ve
gerçekleştiğinde her zaman `BudgetTransaction` üretiyor. Ancak gerçek hayatta
tekrarlayan giderlerin büyük bölümü — Netflix, Spotify, telefon faturası, spor
salonu — **kredi kartından** çekilir, banka hesabından değil.

Bugün kullanıcı bunu ancak iki yanlış yoldan biriyle modelleyebiliyor: planı
banka hesabına bağlayıp gerçek kaynağı kaybetmek, ya da hiç plan kurmayıp her
ay elle kart harcaması girmek. İlk yol hesap bakiyesini yanlış düşürür ve kart
borcunu hiç etkilemez.

## Karar

`RecurringTransaction` ve `RecurringTransactionOccurrence` bir `SourceType`
alanı kazanır; kaynak ya bir hesap ya da bir kredi kartıdır.

```text
RecurringSourceType
├── Account
└── CreditCard

RecurringTransaction
├── SourceType
├── AccountId?
├── CreditCardId?
└── tam olarak biri dolu

RecurringTransactionOccurrence
├── SourceType
├── AccountId?
├── CreditCardId?
├── BudgetTransactionId?
├── CreditCardChargeId?
└── gerçekleştiğinde sonuç kimliklerinden tam olarak biri dolu
```

### Ürün sınırları

- Tekrarlayan **gelir** yalnız nakit/banka hesabına bağlanabilir. Kart geliri ve
  kart iadesi bu aşamada modellenmez.
- Tekrarlayan **gider** ve `Fatura / abonelik` hesaba veya kredi kartına
  bağlanabilir.
- `Fatura / abonelik` mevcut ortak tür olarak kalır. Ayrı `Subscription`
  aggregate'ı, merchant kataloğu veya servis logosu eklenmez.

### Gerçekleştirme dalları

```text
Account source  ──► BudgetTransaction   (hesap bakiyesi + rapor/bütçe)
CreditCard source ──► CreditCardCharge  (kart borcu + rapor/bütçe/ekstre)
```

Kart kaynaklı plan gerçekleştiğinde normal kategorili bir `CreditCardCharge`
üretir — yani ADR 0003'teki "kart harcaması giderdir" kuralına uyar ve ikinci
bir gider yaratmaz. Kart recurring planı **taksit planı değildir**;
`InstallmentPlan` toplam tutar, kesin parça sayısı ve rounding invariant'larıyla
ayrı kalır.

Occurrence state değişimi ile sonuç hareketinin insert'i aynı SQL transaction ve
aynı `SaveChangesAsync` sınırındadır. Rowversion ve sonuç bağlantısındaki
filtered unique index, çift dokunma veya ağ retry'ında tek sonuç bırakır.

Kart kaynağı gerçekleştirilirken kart ve kategori current user'a ait olmalı,
kart aktif olmalı, kategori aktif gider kategorisi olmalı, para birimi eşleşmeli
ve kullanılabilir limit yeterli olmalıdır.

### Kalıcı `failed` durumu eklenmez

Kart pasifse veya limit yetersizse charge da occurrence state değişimi de
oluşmaz; occurrence `planned` ve tekrar denenebilir kalır. Planlanan okuma
modeli `ready`/`needs-attention` bilgisini **güncel** kart ve limit durumundan
türetir. Bu sayede kullanıcı limiti boşalttığında bayat bir hata state'ini elle
temizlemek gerekmez.

### Migration

Mevcut bütün recurring plan ve occurrence satırları `SourceType=Account` olarak
backfill edilir. Ardından nullable kolonlar, owner-scoped composite foreign
key'ler, filtered unique index'ler ve exact-one check constraint'leri uygulanır.
Commit edilmiş migration geçmişi yeniden yazılmaz.

### Backup uyumluluğu

Recurring source ve `CreditCardChargeId` bağlantısı kayıpsız restore için backup
şemasına eklenir: yeni export schema **v3** üretir, mevcut **v2** backup kabul
edilmeye devam eder ve recurring kayıtlarını `SourceType=Account` olarak
yükseltir. Bozuk source kombinasyonu, veri yazılmadan önce restore validation
aşamasında reddedilir.

## Reddedilen seçenekler

- **`CreditCard`'ı `Account` alt türü yapmak**: Limit, borç, ekstre ve ödeme
  kuralları hesap modeline sığmaz; bakiye anlamı ters yönde çalışır. Bu, planın
  değiştirilmeyecek mimari kararları arasındadır.
- **Ayrı `RecurringCardCharge` aggregate'ı**: Sıklık, ay sonu davranışı,
  occurrence üretimi ve idempotency mantığı birebir kopyalanırdı; iki yerde
  bakım borcu doğardı.
- **Nullable olmayan `AccountId`'yi koruyup karta özel bir "sanal hesap"
  açmak**: Bakiye ve raporu bozan sahte bir hesap kaydı üretir.
- **Occurrence'a kalıcı `Failed` durumu eklemek**: Koşul düzeldiğinde bayat
  state temizleme sorumluluğu doğar; türetilen readiness bunu gereksiz kılar.
- **Kart aboneliğini `InstallmentPlan` ile modellemek**: Abonelik süresizdir ve
  toplam tutarı yoktur; taksit invariant'larıyla çelişir.

## Sonuçlar ve trade-off'lar

Nullable `AccountId`/`CreditCardId` çifti, tek zorunlu kolona göre daha zayıf
bir tip güvenliği sunar; bu nedenle "tam olarak biri dolu" kuralı hem Domain
invariant'ı hem SQL check constraint'i olarak iki bağımsız kapıda uygulanır.

Realize use case'i artık dallanır ve iki farklı sonuç türü üretir; test yükü
artar. Buna karşılık kullanıcı gerçek ödeme kaynağını doğru modelleyebilir,
kart borcu ve ekstre doğru hesaplanır.

Plan düzenleme bu aşamada yalnız mevcut aktif/pasif yaşam döngüsünü korur.
Tutar, kaynak, kategori ve sıklık değişikliğinin gelecekteki occurrence'lara
etkisi ayrı kapsamdır; geçmiş occurrence ve gerçekleşmiş hareketler
değiştirilemez.

## Kanıt

Bu bölüm Grup 2 ve Grup 3 doğrulandığında testlerle doldurulur. Beklenen
kanıtlar: account/card source'tan tam birinin seçilmesi, gelir planında kartın
reddi, inactive/foreign kart ve yanlış kategori/currency reddi, occurrence'ın
tek sonuç bağlantısı, yarışan iki realization'da tek sonuç, migration backfill
sonrası eski kayıtların account source kalması, limit yetersizliğinde kısmi
kayıt oluşmaması, backup v2 restore ve v3 round-trip.
