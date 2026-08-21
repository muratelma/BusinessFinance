# ADR 0003 — Kart Harcaması, Ödeme, Ekstre ve Taksit Ayrımı

- Durum: Kabul edildi
- Tarih: 2026-08-10
- Kapsam: Kredi kartı borcu, aylık rapor, ekstre ve taksit planı

## Bağlam

Kredi kartıyla 300 TRY alışveriş yapıldığında ekonomik gider alışveriş anında
oluşur. Daha sonra banka hesabından karta 100 TRY ödemek yeni bir tüketim değildir;
önceden oluşmuş borcun kapatılmasıdır. İki hareketi de gider saymak aynı satın
almayı iki kez raporlar. Benzer biçimde gelecek üç aya planlanan taksitlerin
tamamını bugün gider yazmak gerçekleşmemiş hareketleri rapora sokar.

## Karar

`CreditCard`, limit ve dönem kuralları olan ayrı aggregate'dır; `Account` değildir.
`CreditCardCharge` kart borcunu ve aylık gideri artırır. `CreditCardPayment` aktif
bir Account bakiyesini ve kart borcunu azaltır, fakat aylık gidere girmez.
Pasif kart yeni harcama/plan kabul etmez; mevcut borcun kapatılabilmesi için ödeme
kabul eder. Borcu aşan ödeme ve limiti aşan harcama reddedilir.

Ekstre kalıcı snapshot değildir. Seçilen kesim ayı için önceki devir, dönem
harcamaları ve ödemeler SQL'den owner-scoped toplanır; son ödeme tarihi ile
`asOf` karşılaştırılarak `Open`, `Paid` veya `Overdue` hesaplanır.

`InstallmentPlan` gelecekteki planı, `InstallmentItem` ise her vadeyi taşır.
Toplam dört ondalık hassasiyette eşit parçalara bölünür; bölme kalanı son item'a
eklenir. Yalnız `realize` edilen item aynı transaction içinde bir
`CreditCardCharge` üretir ve rapora girer. `(UserId, ClientRequestId)` unique
index'i plan oluşturma retry'ını, charge bağlantısındaki unique index aynı item'ın
ikinci kez gerçekleşmesini engeller.

## Yan etki tablosu

| Olay | Account bakiyesi | Kart borcu | Aylık gider | Ekstre |
|---|---:|---:|---:|---|
| Kart harcaması | 0 | +tutar | +tutar | Dönemine göre dahil |
| Kart ödemesi | -tutar | -tutar | 0 | Tarihine göre borcu azaltır |
| Planlı taksit item'ı | 0 | 0 | 0 | Dahil değil |
| Gerçekleşen taksit | 0 | +tutar | +tutar | Normal kart harcaması gibi |

## Reddedilen seçenekler

- Kredi kartını negatif bakiyeli Account yapmak: Limit, kesim, son ödeme ve ekstre
  kurallarını banka hesabı davranışıyla karıştırır.
- Kart ödemesini normal gider yapmak: Çifte gider üretir.
- Tüm taksit planını create anında charge yapmak: Geleceği gerçekleşmiş gibi
  gösterir ve iptal/değişiklikleri zorlaştırır.
- Ekstre toplamlarını ayrı tabloda elle saklamak: Hareketlerle ikinci gerçek
  kaynak ve uzlaştırma yükü oluşturur.

## Sonuçlar ve trade-off'lar

Modelde daha fazla açık entity ve sorgu vardır, fakat her finans olayı doğru
anlamı taşır. Dynamic ekstre geçmiş düzeltmeleri otomatik yansıtır; buna karşılık
bankanın hukuken dondurulmuş ekstre snapshot'ı veya faiz/ceza hesabı değildir.
Limit kontrolü ve idempotency database unique constraint'leriyle desteklenir;
yüksek eşzamanlılıkta kullanıcı dostu retry/conflict eşlemesi ileride ayrıca
sertleştirilebilir.

## Kanıt

Gerçek SQL testi 300 TRY harcama ve 100 TRY ödeme sonunda kart borcunu 200,
hesap bakiyesini 900 ve aylık gideri yalnız 300 üretir. Taksit testi 100 TRY'yi
`33.3333 + 33.3333 + 33.3334` olarak korur ve yalnız ilk gerçekleşen item'ı
33.3333 gider yapar. Pixel 8 kabul testi tekrarlanan plan/realize isteklerinde tek
plan ve tek taksit charge'ı kaldığını gerçek HTTP/API/SQL zincirinde doğrular.
