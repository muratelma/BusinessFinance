# ADR 0001 — HTTP API Sözleşme Kuralları

- Durum: Kabul edildi
- Tarih: 2026-08-08
- Kapsam: Aşama 05 ve sonraki HTTP endpoint'leri

## Bağlam

Flutter istemcinin C# tiplerine değil, kararlı HTTP/JSON sözleşmesine ihtiyacı
vardır. Finansal tutarlarda hassasiyet kaybı, yerel tarihlerde timezone kayması,
enum'ların sayı olarak sızması, sınırsız listeler ve her endpoint'te farklı hata
gövdesi istemciyi kırabilir. Kullanıcı kimliğinin request'ten alınması ise yatay
yetki yükseltme riski oluşturur.

## Karar

1. Route tabanı `/api/v1` olur. Kırıcı sözleşme değişikliği aynı route altında
   sessizce yapılmaz.
2. Minimal API endpoint'leri feature route gruplarında tutulur; finansal iş
   kuralları endpoint handler'ında bırakılmaz.
3. API request/response DTO'ları Application ve Domain tiplerinden ayrı tutulur.
4. Enum benzeri dış değerler açık, case-insensitive string allow-list ile kabul
   edilir; response kararlı canonical string döndürür.
5. Para response'u JSON number değil kayıpsız decimal string ve ISO 4217 currency
   code olarak taşınır. Örnek: `"amount": "1234.50", "currency": "TRY"`.
6. İş tarihleri `yyyy-MM-dd`, sistem timestamp'leri UTC ISO 8601 biçimindedir.
7. Liste response'ları `items` ile ortak `pagination` metadata'sı taşır. Page size
   en fazla 100'dür; deterministik sıralama pagination'dan önce uygulanır.
8. Beklenen bütün hatalar `application/problem+json`, HTTP status, kararlı `code`
   ve `traceId` taşır. Binding hataları da aynı sözleşmeye çevrilir.
9. Kullanıcı kimliği request body/query/route'tan alınmaz; doğrulanmış JWT `sub`
   claim'inden `ICurrentUser` aracılığıyla türetilir.
10. OpenAPI yalnız Development ortamında sunulur ve response metadata/testleriyle
    gözden geçirilir.
11. Stage 5 transaction list contract'ı önce geçici
    `501 transactions.not_implemented` ile ayrılmıştır. Stage 7'de owner-scoped
    sorgu tamamlanınca aynı filtre sözleşmesi `200` sayfalı sonuca bağlanmış ve
    geçici `501` response'u kaldırılmıştır.

## Transaction liste filtre sözleşmesi

`GET /api/v1/transactions` şu query alanlarını ayırır:

| Alan | Biçim | Varsayılan / sınır |
|---|---|---|
| `pageNumber` | integer | 1, en az 1 |
| `pageSize` | integer | 20, 1–100 |
| `dateFrom` | `yyyy-MM-dd` | optional, inclusive |
| `dateTo` | `yyyy-MM-dd` | optional, inclusive |
| `accountId` | non-empty GUID | optional |
| `categoryId` | non-empty GUID | optional |
| `type` | `income` / `expense` | optional |
| `sort` | `date-desc` | tek desteklenen değer |

`TransactionResponse`, `amount` için string, `currency` için string,
`transactionDate` için `yyyy-MM-dd` string taşır. Balance alanı elle saklanan
ikinci kaynak olarak sözleşmeye eklenmez.

## Geriye uyumluluk kuralları

- Optional response alanı eklemek çoğunlukla additive değişikliktir; yine de
  Flutter parser davranışı test edilir.
- Alan silmek, yeniden adlandırmak, tipini değiştirmek veya anlamını değiştirmek
  kırıcıdır ve yeni API version kararı gerektirir.
- Yeni enum değeri bazı eski istemcileri kırabileceği için otomatik olarak risksiz
  kabul edilmez.
- Page size üst sınırı ve varsayılan sıralama sessizce değiştirilmez.
- Problem `code` değerleri istemci davranışına girdi olabileceği için kararlı
  sözleşmedir; yalnız insan mesajı localization nedeniyle değişebilir.

## Sonuçlar ve trade-off'lar

Açık DTO ve elle eşleme daha fazla kod üretir, fakat C# refactor'unun mobil JSON'u
istemeden değiştirmesini önler. Para string'i istemcide parse adımı gerektirir,
fakat IEEE-754 number hassasiyet riskini kaldırır. Route versioning URL'leri
çoğaltabilir, fakat kırıcı geçişi görünür yapar. Stage 5'teki geçici `501`, sahte
boş `200` ile özellik uygulanmış izlenimi vermedi. Stage 7 gerçek sorgusu aynı
filtre/response sözleşmesini değiştirmeden integration testlerle doldurdu.
