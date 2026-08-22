# Yapılacaklar

Konuşma sırasında biriken, henüz bir aşama belgesine bağlanmamış işler.
Sıra buradan takip edilir; bir iş alındığında **Alındı**, bitince satır
silinip ilgili aşama belgesine/commit'e geçer.

Bu dosya bir aşama belgesi değildir: kapsamı kullanıcı onaylamadan buradaki
hiçbir madde uygulanmaz. Aktif aşama `stages/README.md` tablosundadır.

## Devralınan açık işler

Kod tabanıyla birlikte gelen, kapanmamış işler. Hepsi önceki repoda kayda
geçmişti; buraya taşındılar çünkü hâlâ geçerliler.

| # | İş | Kaynak | Durum |
|---|---|---|---|
| 1 | **Fazla ödenmiş kart bakiyesi net varlıkta kayboluyor** — kart borcu `Math.Max(0, harcama − ödeme)` ile kart başına kırpılıyor. Kartı fazla ödersen oluşan alacaklı bakiye senin paran ama net varlıkta 0 sayılıyor. Mevcut veride tetiklenmiyor; denetimde görüldü, tetiklenmeden finansal anlam değiştirmemek için kayda geçirildi | 2026-08-18 denetim | Öneri |
| 2 | **Bütçe ekranı iyileştirmeleri** — kapsamı belirsiz; alınmadan önce kısa bir öneri listesine dönmesi gerekiyor. Bütçenin hangi kapsamı sınırladığı sorusu Aşama 01'e girdi; geri kalan iyileştirmeler burada duruyor | kullanıcı listesi | Öneri |
| 3 | **Fiş/dekont akışının cihaz kabul turu** — belge yönü (harcama/gelir/iade/fatura-ödeme/taksit/borç verme) için Pixel 8'de manuel kabul turu yapılmadı. Kod ve testler yerinde; eksik olan cihaz doğrulaması | devralınan | Açık |
| 4 | **Fiş okumada veri sınırı kararı** — hangi belgenin hangi katmana gönderileceği kararı bilinçli olarak ertelenmişti. Gerçek ve değerli belgeye geçmeden önce alınmalı ve ADR 0011'e işlenmeli | devralınan | Açık |

## Notlar (iş değil, kayıt)

- **`Msg 8624`** — `BudgetTransactions` / `CreditCardCharges` üzerinden tek
  satır `DELETE`, cascade grafiği karmaşık olduğu için sorgu planı
  üretilemeden düşüyor. Uygulamayı etkilemiyor: hareket silinmiyor, iptal
  ediliyor. Şema sadeleşirse kendiliğinden düzelir.

## Aşamaya bağlananlar

Konuşmada çıkan ve artık bir aşama belgesine bağlanmış işler; burada iş olarak
durmuyorlar, izleri kayıt olsun diye yazılı:

- Ödenmemiş faturanın tekrarlayan plan olmaya zorlanması → Aşama 03
- Varsayılan kategori setinin ev bütçesi olması → Aşama 01
- `İşlem ekle` menüsünün taşması → Aşama 04
- Ana sekme yapısının işletme kullanıcısına göre kurulmamış olması → Aşama 04
- "Kredi kartı" kelimesinin POS ile ters anlam taşıması → Aşama 04

## Park edilenler (bu tur değil)

- Gerçek portföy takibi (Borsa/Kripto varlık sınıfı olarak — kategori olarak
  değil; gerekçe `EfCategoryRepository.Defaults` doc yorumunda).
