**Raporun ana yönüne katılıyorum; fakat bu hâliyle uygulama planını onaylamazdım.** Gerçekleşmiş kayıtlarla gelecek planlarını ayırmak doğru. Ancak rapor bazı denetimleri olduğundan kapsamlı gösteriyor ve önerdiği değişikliğin sağlayamayacağı bir bakiye garantisi veriyor.

Değerlendirmeyi mevcut çalışma ağacındaki kod üzerinden yaptım (`ui-trials`, HEAD `a4df4ed`). Raporda sözü edilen canlı API denemesini ve veritabanındaki “dört kaydı” ayrıca doğrulamadım.

1. **Ana sorun gerçek: ileri tarihli kayıt bugünkü bakiyeyi etkiliyor.**

   Hesap bakiyesi ve güncel kart borcu hesaplanırken tarih kesimi verilmiyor: ikisi de `cutoff: null` kullanıyor. Dolayısıyla geleceğe yazılmış bir hareket bugünkü toplama giriyor. Raporun temel gerekçesi kodla destekleniyor. [Hesap bakiyesi](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Infrastructure/Accounts/EfAccountRepository.cs:104), [kart borcu](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Infrastructure/CreditCards/EfCreditCardRepository.cs:63).

2. **“Tekrarlayanlarda denetim var” satırı eksik; açık kalan başka bir API yolu var.**

   Plan ve tarih üzerinden gerçekleştirmede, ödeme tarihi verilmezse gelecek vade reddediliyor. Fakat doğrudan kalem kimliğiyle çalışan diğer gerçekleştirme ucunda aynı kontrol yok. Ödeme tarihi gönderilmediğinde kayıt kalemin planlanan tarihine yazılıyor; bu tarihin gelecekte olup olmadığı burada denetlenmiyor. Kalem üretme işlemi de ileri bir tarihe kadar üretime izin veriyor.

   Bu yüzden kapsam “eksik on kayıt türü” diye kapatılmamalı; **gerçek kayıt üreten bütün yollar** üzerinden çıkarılmalı. Yalnız normal gelir/gider ekleme servisine kontrol koymak da yetmez: tekrarlayanlar ve CSV onayı kaydı ayrı yollardan oluşturuyor. [İki gerçekleştirme ucu](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Api/Features/RecurringTransactions/RecurringEndpoints.cs:45), [kalem gerçekleştirme](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/RecurringTransactions/RecurringUseCases.cs:618), [CSV onayı](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/Imports/ImportUseCases.cs:206).

3. **İşlem tarihi ile vade aynı şey değil.**

   Örneğin 20 Ekim vadeli bir borcu 12 Ekim’de gerçekten ödemişsem, ödeme günü 12 Ekim’dir. Gelecek işlem yasağı bu ödemeyi engellememeli.

   Üstelik vergi akışı bunu zaten destekliyor: açık ödeme tarihi verildiğinde erken ödeme mümkün. Dolayısıyla “tekrarlayan planla aynı şekilde vadesi gelmedi diye reddet” gerekçesi genellenemez. Cari tahsilat tablosunda da ayrım eksik: kartla tahsilat POS yoluna giriyor ve formu bugünle sınırlı; hesaptan tahsilat aynı sınıra sahip değil. [Erken ödeme kuralı](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/RecurringTransactions/RecurringUseCases.cs:895), [cari ödeme formu](/C:/Users/elma6/Documents/BusinessFinance/mobile/business_finance_mobile/lib/features/counterparties/presentation/counterparty_forms.dart:387).

4. **“Eski kayıtlar kalır” ile “bakiye hep bugünü gösterir” birlikte sağlanmıyor.**

   Eski veya desteklenen bir yedekten geri gelen ileri tarihli kayıtlar aynı hesaplamaya katılmaya devam eder. Yeni kayıtları engellemek mevcut etkiyi düzeltmez.

   Kayıtları korumaya katılıyorum; fakat geçiş davranışı açıkça seçilmeli. Ya bu etki geçici olarak kabul edilip vaat daraltılmalı ya da güncel bakiye okumalarının tarih sınırı ayrıca tasarlanmalı. İkinci seçenek hesap, kart, cari ve net varlık tutarlılığı açısından incelenmeli. **Kayıtları sessizce bugüne taşımak veya silmek uygun değil.**

   İleri ayları kapatmak da bu çözümden önce yapılmamalı: bakiyeyi etkileyen eski kayıt, kullanıcının erişemediği bir ayda kalabilir.

5. **Bir günlük pay, kesin bir gelecek tarih yasağı değildir.**

   Sunucudaki kural gerçekten UTC gününe bir gün ekliyor. Örneğin Türkiye’de 12 Ekim öğlen, API 13 Ekim’i kabul edebilir. Bu, gece yarısı sorununu gideren bir tolerans; kullanıcının “bugün”ünü kesin olarak belirlemiyor.

   Tarih seçiciyi sınırlamak yararlı, fakat sunucu garantisinin yerini tutmaz. Kesin sınır isteniyorsa ürünün hangi saat dilimine göre gün değiştirdiği belirlenmeli ve sunucu bunu uygulamalı. Bu değişiklikte mevcut tolerans korunacaksa, rapor bunu kalan sınırlama olarak söylemeli. [Yerel gün kuralı](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Domain/LocalDay.cs:15).

6. **Tek seferlik plan ihtiyacı doğru; yükümlülük onun genel karşılığı değil.**

   “Gelecek ay bir şey almayı düşünüyorum” ile “aldım, parasını gelecek ay ödeyeceğim” farklı olaylar. İkincisi bugünden doğmuş yükümlülüktür; ilkinde henüz tanınacak alışveriş yoktur. Yükümlülüğün gideri belge/olay tarihinde tanıması bu nedenle kusur değil, mevcut ürün kararıdır. [ADR 0014](/C:/Users/elma6/Documents/BusinessFinance/documentation/adr/0014-economic-event-recognizes-payment-carries.md).

   Tekrar sınırı 1 geçici olarak yeterli olabilir. Ben kullanıcıya görünen **“Tek seferlik” seçeneğini** öneririm; mevcut plan altyapısıyla uygulanabilirliği incelenebilir, baştan ayrı bir modül gerektirdiği varsayılmamalı. Transfer ve kart ödemesi gibi hareketlerin de normal gelir/gider planıyla karşılanamayacağı unutulmamalı: bunları “günü gelince gir” demek, planlama ihtiyacını karşılamaktan vazgeçme tercihidir.

**Rapordaki iki soruya cevabım:**

- **Kart taksitini gerçekleştirme:** Mevcut işlem taksiti planlanan gününe yazdığı için, gelecekteki taksitin bu yolla gerçekleştirilmesini şimdilik reddetmek daha tutarlı. Otomatik olarak bugüne yazılmasını önermem. Gerçek erken ödeme gerekiyorsa kart borcu ödemesiyle taksit harcamasını gerçekleştirme ayrımı korunarak ele alınmalı. [Mevcut taksit davranışı](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/CreditCards/InstallmentUseCases.cs:197).
- **Tek seferlik plan:** Görünür seçenek tercih ederim. Tarih denetimi düzeltmesinden ayrı teslim edilebilir; arada tekrar sınırı 1 geçici çözüm olarak kullanılabilir.

Uygulamaya geçmeden kabul senaryoları da yazılmalı: yarın tarihli kayıt reddedildiğinde hiçbir finansal etki oluşmaması; gelecekteki vadeye bugünkü gerçek ödemenin kabul edilmesi; iki tekrarlayan gerçekleştirme yolunun tutarlılığı; gece yarısı sınırı; CSV düzeltme ve onay yolları; desteklenen yedeğin geri yüklenmesi; mevcut ileri tarihli kayda erişim ve tekrar isteğinde ikinci kayıt oluşmaması. Mevcut testlerdeki tarihleri değiştirmek tek başına yeterli doğrulama değildir.

Uygulama kodunu ve mevcut değişiklikleri değiştirmedim; build/test/analyze çalıştırmadım. Sıradaki tek görev, bu bulgularla tarih kuralını ve eski kayıtların geçiş davranışını netleştirmek.

Değerlendirmenin tamamı: [Markdown raporu](/C:/Users/elma6/Documents/BusinessFinance/research/DIS-GOZ-2026-10-12-gelecek-tarihli-kayit-kurali.md).
