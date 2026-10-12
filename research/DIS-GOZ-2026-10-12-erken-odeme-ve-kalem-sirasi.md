**Bence erken ödeme kalsın; aynı planın ödeme sırası atlandığında açık uyarı verilsin. C’deki kesin yasağı ilk tercih olarak önermem.**

Önce temel ayrım: **“Gerçekleştir” bankadan ödeme yapmıyor; senin zaten yaptığın ödemeyi uygulamaya kaydediyor.** Para henüz çıkmadıysa kalem plan olarak kalmalı. Vadesi gelmemiş olsa bile para gerçekten bugün çıktıysa bugünkü bakiyede görünmesi doğru.

Senin örneğinde iki farklı durum var:

- **İki ayrı plan:** Bir ay sonraki kira ile iki ay sonraki sigorta ödemesi birbirini engellememeli.
- **Aynı planın iki dönemi:** Kasım ve Aralık kirası bekliyorsa normal kullanımda önce Kasım seçilir. Aralık seçildiğinde ise uygulama yanlış satıra basılmış olabileceğini açıkça göstermeli.

Aralık için önerdiğim onay şöyle:

> **Aralık kirasını kaydediyorsunuz.**  
> Kasım kirası henüz ödenmiş görünmüyor. Bu kayıt yalnız Aralık kirasını kapatacak; Kasım beklemeye devam edecek.  
> Ödeme günü: 12 Ekim.

Kullanıcı gerçekten Aralık için ödeme yaptıysa kaydedebilmeli. Kasım da kendiliğinden kapanmamalı. Böylece hem yanlış seçimi fark ettiririz hem de gerçekleşmiş bir ödemenin doğru kaydedilmesini engellemeyiz.

**C neden kusursuz değil?** Kasım ödemesi uygulamaya henüz girilmemiş olabilir. Kesin sıra yasağı, gerçekten yapılmış Aralık ödemesini kaydetmeyi engeller. Ayrıca C’nin tanımı eksik: yalnız gelecekteki kalemlere bakılırsa geçmişte ödenmemiş Ekim dururken Kasım açılabilir. Gerçek sıra zorunluluğu isteniyorsa aynı planın **gecikmişler dahil en eski bekleyen kalemi** esas alınmalı. Bu da daha kısıtlayıcı bir ürün kararıdır; finansal doğruluğun zorunlu sonucu değildir.

**Benim tercihim A’nın uyarılı hâli:** erken kayıt mümkün, önceki bekleyen kalem varsa belirgin onay, kayıt gerçek ödeme gününe, yalnız seçilen kalem kapanır. Uyarı önceki bekleyen kalemleri sunucudan öğrenmeli; yalnız ekranda görünen 30/90 günlük listeye bakmamalı.

Önceki değerlendirmelerimde erken ödeme ile sıra atlamayı ayrıca ayırmalıydım. Güncel kodda ödeme günü ile vade ayrılmış; incelenen gerçekleştirme yollarında önceki kalemin ödenmesini şart koşan bir kontrol bulunmuyor. [Kaynak kod](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/RecurringTransactions/RecurringUseCases.cs:618).

Yalnız bu değerlendirmeyi kaydettim; uygulama kodunu değiştirmedim ve test çalıştırmadım. Sıradaki karar, sıra atlamanın uyarıyla mı yoksa kesin yasakla mı ele alınacağı.

[Değerlendirme dosyası](/C:/Users/elma6/Documents/BusinessFinance/research/DIS-GOZ-2026-10-12-erken-odeme-ve-kalem-sirasi.md).
