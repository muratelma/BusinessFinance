**Bu sürüm önceki rapora göre belirgin biçimde daha tutarlı. Ana kapsamı destekliyorum; uygulamadan önce üç noktayı netleştirirdim.**

**1. Erken ödeme açılsın; kayıt otomatik olarak bugüne sabitlenmesin.**

Kirayı veya aboneliği vadesinden önce gerçekten ödemiş kullanıcı bunu planın üzerinden kaydedebilmeli. Buna katılıyorum. Ancak “Gerçekleştir çıkar ve kaydı bugüne yazar” eksik bir çözüm:

- 20 Ekim vadeli kirayı 11 Ekim’de ödedim.
- Uygulamaya 12 Ekim’de girdim.
- Doğru kayıt tarihi 11 Ekim; bugün de vade günü de değil.

Önerim: **gerçekleşme günü varsayılan olarak bugün gelsin, kullanıcı geçmiş bir gün seçebilsin.** Onayda tarih, tutar ve hesap/kart görünsün. Seçilen plan kalemi gerçekleşsin; ödeme ayrıca bağımsız gider olarak yazılıp kalem açık bırakılmasın. Aynı kaleme tekrar basıldığında ikinci kayıt oluşmasın.

Sunucuda ödeme günüyle çalışan yol var; sıradan planlara kapalı bir vergi koşulu bu bölümde bulunmuyor. Fakat Flutter’daki mevcut iki gerçekleştirme çağrısı ödeme gününü göndermiyor. Dolayısıyla iş yalnız düğmeyi görünür yapmak değil; iki çağrının da aynı davranışı sağlaması ve bunun test edilmesi gerekiyor. [Sunucuda kayıt tarihi](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/RecurringTransactions/RecurringUseCases.cs:631), [istemcide iki çağrı](/C:/Users/elma6/Documents/BusinessFinance/mobile/business_finance_mobile/lib/features/planning/data/planning_repository.dart:126).

**2. “Üçü için tek kural” cümlesi kart taksidi açısından düzeltilmeli.**

Metin önce gelecekteki kart taksidini reddetmeyi, sonra üçünün de erken ödenebilmesini öneriyor. Bunlar aynı kapsam değil.

Benim önerim şu ayrım:

| İşlem | Önerdiğim davranış |
|---|---|
| Vergi ödeme | Mevcut erken ödeme davranışı korunsun |
| Kira/abonelik gerçekleştirme | Gerçekleşme günü alınarak erken kayıt açılsın |
| Gelecekteki kart taksidini gerçekleştirme | Bu iş kapsamında reddedilsin; otomatik bugüne taşınmasın |

Kart taksidini harcama olarak gerçekleştirmek ile kart borcunu ödemek ayrı işlemler. Kart tarafındaki ret önerisine katılmam, gerçek erken kart ödemesinin yasaklanmasını desteklediğim anlamına gelmiyor.

Ayrıca erken ödeme açılırsa **“Planlananlar’da hiçbir şey değişmiyor”** doğru olmaz. Liste ufku ve planların görünmesi korunabilir; fakat satırın düğmesi, onayı ve gerçekleştirme davranışı değişir. Bugünkü arayüz gelecek tekrarlayan satırların düğmesini özellikle gizliyor. [Mevcut görünürlük kuralı](/C:/Users/elma6/Documents/BusinessFinance/mobile/business_finance_mobile/lib/features/planning/presentation/planning_page.dart:320).

**3. Bir günlük pay kabul edilebilir bir geçici sınır; vaat ona uygun yazılmalı.**

“Yarının tarihi API’den geçer” denirken **“yeni kayıt ileri güne yazılamaz”** denemez. Tarih seçici API’nin diğer kullanım yollarını sınırlandırmaz.

Bu teslim için doğru teknik kapsam şöyle:

> Gerçekleşmiş kayıt formlarında en ileri gün bugün olur. Sunucuda bütün yeni kayıt üretim yolları mevcut UTC günü + 1 gün sınırına uyar. Bu tolerans nedeniyle API üzerinden kullanıcının yarınına kayıt açılması tamamen engellenmiş olmaz.

Mevcut `LocalDay` tam olarak bu toleransı uyguluyor. Bunun kesin yerel gün denetiminden ayrı bırakılmasına, sınırlaması açıkça kabul edildiği sürece itirazım yok. [Sunucu sınırı](/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Domain/LocalDay.cs:15).

**Diğer düzeltmelere katılıyorum:**

- KÖ11’in, yani kaydı olan ileri ayın seçilebilmesinin korunması doğru.
- Eski kayıtları ve desteklenen yedekleri değiştirmeden, bakiyeye mevcut etkilerini şimdilik kabul etmek açıkça tanımlanmış bir geçiş tercihi olabilir.
- Tek seferlik plan seçeneğinin ayrı iş olması tarih denetimini bekletmek zorunda değil.
- İleri tarihli transferin yerine planlama yolu sunulmaması bilerek kabul edilen bir kapsam sınırı olarak yazılmalı.
- “İşlemler’den kalkan” mevcut kayıtlar değil, **yeni ileri tarihli kayıt oluşturma imkânı** olmalı; eski kayıtlar erişilebilir kalmalı.

**İki karar sorusuna benim cevabım:** Kira/abonelik için erken gerçekleştirmeyi destekliyorum; gerçekleşme günü seçilebilmeli. Yukarıdaki ayrımlar nihai kapsama işlendiğinde uygulamaya geçilmesini de destekliyorum. Son kapsam listesinde erken ödeme akışı açıkça yer almalı; şu an anlatımda öneriliyor ama kapanıştaki kapsam listesinde görünmüyor.

Bu turda hedefli kaynak kod incelemesi yaptım; canlı API denemesi ve build/test/analyze çalıştırmadım. Yalnız bu değerlendirme dosyasını ekledim. Sıradaki tek görev, erken ödeme akışını ve sunucunun tarih sınırını nihai kapsam metninde kesinleştirmek.

Tam metin: [İkinci dış göz değerlendirmesi](/C:/Users/elma6/Documents/BusinessFinance/research/DIS-GOZ-2026-10-12-gelecek-tarihli-kayit-kurali-ikinci-degerlendirme.md).
