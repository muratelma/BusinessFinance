# ADR 0010 — Net varlıkta borç kalan anaparayla ölçülür

- Durum: Kabul edildi (18 Ağustos 2026, Aşama 12.9)
- Bağlam: Aşama 12.9 belgesi — önceki repoda (`Kisisel-Butce-Mobil`), bu repoya taşınmadı
- İlgili: ADR 0009 (borcun kaynağı ve anüite faiz modeli)

## Bağlam

Aşama 12.9'da özet ekranındaki varlık kartına alacak ve borç satırları
eklendi. O ana kadar `netWorth` hesaplanıp gönderiliyor ama dökümünün yalnız
iki terimi ekranda gösteriliyordu; kalan iki terim görünmediği için kimse
sayılara bakmamıştı.

Satırlar ekrana çıkar çıkmaz tutarsızlık görüldü: **bilanço ile gelir tablosu
birbirini tutmuyordu.**

Açık borç, ödenmemiş taksitlerin **tutarları** toplanarak bulunuyordu.
`DebtInstallment.Amount` anapara + faizdir, dolayısıyla borç kalan anaparayı
**ve gelecekteki bütün faizi** içeriyordu. Faiz ise gider tarafında
`DebtInterestAsync` ile **ödendikçe** yazılıyor (ADR 0009, karar 5).

Aynı olayın iki ölçüsü çelişiyordu:

| | 1.000 anapara / 1.200 toplam, kredi çekildiği gün |
|---|---|
| Likit varlık | +1.000 |
| Açık borç (eski) | 1.200 |
| **Net varlık** | **−200** |
| Gider raporu | 0 |

Kullanıcı parayı yeni almış, hiçbir şey ödememiş, hiçbir faiz tahakkuk
etmemişti — ama ekran onu 200 TL fakirleşmiş gösteriyordu.

Gerçek bir API testinde de aynı hata sabitlenmişti: 1.000 anapara / 1.200
toplam, bir taksit ödenmiş senaryoda net varlık **800** çıkıyordu. Kullanıcının
kendi parası 1.000 ve tahakkuk eden faiz 97,0103 olduğuna göre doğru sayı
**902,9897**'dir; aradaki 102,9897 tam olarak henüz ödenmemiş faizdi. Testin
kendi yorumu "net durumu bozulmamalı" diyordu ama sabitlediği sayı bunu
sağlamıyordu.

## Karar

**Net varlıktaki açık borç ve alacak, ödenmemiş taksitlerin anapara
paylarının toplamıdır.**

```
netWorth = likit varlık − kart borcu + kalan alacak anaparası − kalan borç anaparası
```

Gelecekteki faiz **yükümlülük sayılmaz.** Doğmamış bir maliyettir; ödendiği ay
hem gidere yazılır hem borcu azaltır. İki kez düşülemez.

Ayrım 12.8'den beri her taksitte saklı (`PrincipalPortion`), yani bu ölçü SQL
tarafında tek sorguda ifade edilebiliyor. Ayrımı olmayan bir kayıtta taksitin
tamamı anapara sayılır: bilinmeyen bir ayrımda borcu **olduğundan büyük**
göstermek, sıfır sayıp yok saymaktan güvenlidir.

## Sonuçlar

- Bilanço ile gelir tablosu artık aynı şeyi söylüyor: net varlığın düşüşü,
  tahakkuk etmiş faiz kadardır.
- Borç almak net varlığı değiştirmez. Doğru davranış budur: eline geçen para
  kadar yükümlülük doğar.
- `DebtResponse.RemainingAmount` **değişmedi** ve kalan ödemelerin toplamı
  olmaya devam ediyor. O alan "daha ne kadar ödeyeceğim" sorusunun yanıtıdır
  ve nakit planlaması için doğru sayı odur. İki alan iki farklı soruyu
  yanıtlıyor; birini diğerine eşitlemek ikisinden birini yanlış yapardı.
- Gelecekteki faiz yükü ekranda hiçbir yerde toplanmıyor. İhtiyaç doğarsa
  yeri bilanço değil, yaklaşan ödemeler tarafıdır.
- Varlık kartındaki borç ve alacak satırları `faiz hariç` alt satırıyla
  yazılıyor. Sayının faizi içerip içermediği ekrandan okunamıyordu ve bu
  gerçek bir soru: 1.000 anapara / 1.200 toplam bir kredide hangisinin
  gösterildiği belli değildi. Ölçüyü açıklamak ölçüyü değiştirmekten ucuz.

## Faizi geri eklemek neden düzeltmez

Sezgi "faizi de yazalım ki tam doğru olsun" der. Aksine: faizli ölçü **iki
ayrı yerde** yanlışa düşer, anaparalı ölçü ikisinde de tutar.

1.000 anapara / 1.200 toplam, üç taksit × 400 (302,99 anapara + 97,01 faiz):

| Olay | Ölçü | Net varlık | Gider raporu |
|---|---|---|---|
| Kredi çekildi | anapara | **0** | 0 ✅ |
| Kredi çekildi | faizli | **−200** | 0 ❌ |
| Bir taksit ödendi | anapara | **−97,01** | 97,01 ✅ |
| Bir taksit ödendi | faizli | **0** | 97,01 ❌ |

İkinci satır çifti belirleyicidir: faizli ölçüde 97,01 TL faiz ödediğin ay net
varlığın **hiç düşmez** — yani faizi bedavaya ödemiş görünürsün. Anaparalı
ölçüde net varlık tam faiz kadar düşer ve gider raporuyla birebir aynı sayıyı
verir.

Sebep basit: faizli ölçüde faiz iki kez sayılır (bilançoda peşin, gider
raporunda ödendikçe) ve bu iki sayım birbirini götürerek ödeme anını
görünmez kılar.

## Reddedilenler

**Kalan ödemelerin toplamı (eski hâli).** "Bütün yükümlülüklerimi
karşıladıktan sonra elimde ne kalır" sorusunun yanıtı olarak savunulabilirdi
ama o zaman kartın adı da "net varlık" olamazdı. Yukarıdaki tablo asıl
sorunu gösteriyor: ölçü, faiz ödemesini görünmez kılıyor.

**Anaparayla hesaplayıp kalan faizi ayrı satır göstermek.** Daha tam bir resim
verirdi ama varlık kartına bilanço kalemi olmayan bir sayı sokardı. Kalan
faiz bir tahmin değil taahhüttür, ve taahhütlerin yeri yaklaşan ödemelerdir.
İhtiyaç doğarsa oraya eklenir; bu ADR onu yasaklamıyor, yalnız bilançodan
uzak tutuyor.

## Doğrulama

`OutstandingDebt_IsRemainingPrincipal_NotRemainingPayments` gerçek SQL'e karşı
çalışan bir kapıdır: kredi çekildiği gün likit +1.000, borç 300 (anapara),
net varlık değişmemiş 1.000 ve aylık gider 0. Kapı, sorgu eski hâline
döndürülerek sınandı; borç 400 çıkıp test düştü.
