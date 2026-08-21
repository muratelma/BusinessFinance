# ADR 0002 — Transferi Tek İş Olayı Olarak Modelleme

- Durum: Kabul edildi
- Tarih: 2026-08-10
- Kapsam: Hesaplar arası transfer, bakiye ve aylık rapor

## Bağlam

Aynı kişinin banka hesabından nakit hesabına 250 TRY taşıması kişinin gelirini
veya giderini değiştirmez. Bunu bir gider ve bir gelir olarak kaydetmek aylık
ciroyu yapay biçimde şişirir. İki bağımsız hareket yazmak, ilk yazı başarılı olup
ikincisi başarısız olduğunda paranın sistemde kaybolmuş veya çoğalmış görünmesine
de yol açabilir.

## Karar

Transfer, kaynak ve hedef hesap kimliklerini taşıyan tek `Transfer` aggregate'ı
ve tek veritabanı satırıdır. İki hesap aynı authenticated kullanıcıya ait, aktif,
TRY ve birbirinden farklı olmalıdır. Kullanıcı kimliği request'ten alınmaz.

Hesap bakiyesi projection'ı aktif transferleri yönüne göre uygular:

```text
kaynak bakiye = ... - giden transferler
hedef bakiye  = ... + gelen transferler
```

Aylık gelir/gider raporu transfer satırını her iki toplamdan da dışlar. Transfer
iptali satırı silmez; UTC iptal zamanını kaydeder ve bakiye projection'ı artık o
satırı hesaba katmaz. Düzeltme, iptal ve yeni doğru transfer olarak yapılır.

## Reddedilen seçenekler

- İki `BudgetTransaction` yazmak: Mevcut gelir/gider türleri transfer anlamını
  doğru taşımaz ve raporu bozma riski yaratır.
- Hesapların `CurrentBalance` kolonlarını doğrudan güncellemek: Hareket geçmişiyle
  ikinci gerçek kaynak oluşturur ve yarım güncelleme/uzlaşma sorunu doğurur.
- Kaynak ve hedef için iki transfer satırı: Aynı iş olayının kimliği ve atomikliği
  zayıflar.

## Sonuçlar ve trade-off'lar

Bakiye sorgusu gelen ve giden yönlerini toplamak zorundadır; buna karşılık finans
geçmişi tek olay olarak okunur ve rapor sınıflandırması nettir. Tek repository
`SaveChangesAsync` sınırı atomikliği sağlar. Gelecekte farklı para birimleri
desteklenirse kur dönüşümü ayrı ve açık bir iş olayı gerektirir; Stage 10 transferi
yalnız TRY'dir.

## Kanıt

Domain ve Application testleri farklı/aktif/owner hesap sınırlarını doğrular.
Gerçek SQL testi tek transfer satırından 1000→750 ve 100→350 bakiyelerini üretir;
aynı aylık rapor gelir, gider ve net için sıfır döndürür. API negatif testi başka
kullanıcının hesabını ve transfer detayını erişilemez tutar.
