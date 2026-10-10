**Raporun bulduğu açık gerçek; fakat “listeye veresiye satırı ekleyelim, küçük bir iş” önerisi sorunu bütünüyle çözmüyor.** Mevcut kodu, karar belgelerini, GİB’in teknik kılavuzunu ve üretici/ürün belgelerini karşılaştırdım. Senden “esnaf olarak nasıl kullanıyorsun?” diye cevap beklemek burada doğru yaklaşım değil. Önce desteklenecek senaryoları araştırıp sana gerekçeli bir ürün önerisi sunmalıydı.

Önce son sorunu cevaplayayım: **Kutuyu işaretlemek parayı ikinci kez girmek değildir. Gün sonunda oluşturulacak yeni satıştan o tutarı çıkarmaktır.**

300 TL cari tahsilatın uygulamaya zaten girildiğini düşün:

| Gün sonuna yazdığın nakit tutarı | Tahsilat kutusu | Gün sonunun ekleyeceği satış | Önceden girilen tahsilatla toplam kasa girişi |
|---|---|---:|---:|
| 1.500 TL; eski borç tahsilatı bunun **dışında** | İşaretsiz | 1.500 TL | 1.800 TL |
| 1.500 TL; eski borç tahsilatı bunun **içinde** | İşaretli | 1.200 TL | 1.500 TL |

İlk durumda yanlışlıkla işaretlersen 300 TL **eksik satış** yazarsın. İkinci durumda işaretlemezsen 300 TL **fazladan satış ve kasa girişi** yazarsın. Mevcut [hesaplama kodu](C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/DayCloses/DayClosePlan.cs:263) tam olarak böyle çalışıyor.

Dolayısıyla “dokunmazsın” açıklaması eksik. Doğru açıklama şu olmalı:

> “Bu tahsilat, yukarıya yazdığın nakit tutarının içindeyse işaretle. Uygulama tahsilatı yeniden kaydetmez; yeni satıştan düşer.”

**Araştırmanın temel bulgusu: Z raporu tek bir “bugün kazanılan para” sayısı değildir.**

Cihaz ve sürüme göre düzen değişse de şu bilgileri ayırmak gerekiyor:

| Rapordaki bilgi | Anlamı | Uygulamamız açısından |
|---|---|---|
| Günlük satış ve vergi toplamları | Cihazda belgelenen satışlar | Satışın kapsamını anlamaya yarar; vergi hesaplamayız |
| Ödeme türleri | Nakit, banka/kredi kartı ve desteklenen diğer türler | Paranın hangi yoldan geldiğini veya satışın açık hesap olduğunu ayırır |
| Bilgi fişleri | Faturalı işlemler, avans, cari hesap tahsilatı gibi ayrı işlem bilgileri | Hepsi yeni satış kabul edilemez |
| Kümülatif toplamlar | Cihazın birikmiş toplamları | Günlük gelir olarak alınamaz |
| Tarih, Z numarası, cihaz bilgileri | Raporun kimliği ve dönemi | Doğru dönemi ve tekrarları kontrol etmeye yarar |

GİB, bilgi fişlerinin X/Z raporlarında faturalı satış, avans ve **cari hesap tahsilatı** gibi ayrı sayaç ve tutarlarda gösterilmesini açıkça tarif ediyor. Cari tahsilat için ayrıca bilgi fişi örneği var. Bu fişin kullanılmasının koşulları da tanımlı; uygulamadaki her “cari” kaydın kendiliğinden aynı mali belge türüne karşılık geldiğini varsayamayız. [GİB Bilgi Fişleri Teknik Kılavuzu, §8 ve §9.6](https://ynokc.gib.gov.tr/UploadedFiles/Files/Bilgi_Fisi_Teknik_Kilavuzu_24072020.pdf)

Veresiyenin cihazda ayrı işlenebilmesi de varsayım değil: Ingenico Move/5000 F–iDE280 kılavuzunda **“Açık hesap” ayrı ödeme seçeneği** olarak bulunuyor. Bununla birlikte bütün cihazların aynı seçenekleri ve aynı rapor düzenini kullandığı sonucuna varamayız. Örneğin incelediğim Beko 300 TR kılavuz sürümü üç ödeme şeklini anlatıyor. [Ingenico kılavuzu](https://ikasa.com.tr/wp-content/uploads/2024/12/document_ffd2e887-0ae9-4b92-b891-32dbbb0cb97a.pdf), [Beko 300 TR kılavuzu, §6.7](https://download.arcelik.com.tr/Download.UsageManualsBeko/300tr-yazar-kasa-tuketici-elektronigi-kullanim-kilavuzu-tr_TR_201505281111678_User-20Manual-20-20Filetur-A.pdf)

Buradan çıkan sonuç: **“Yazar kasadan geçti” ile “girdiğin nakit tutarının içinde” aynı bilgi değil.** İşlem raporda başka bir bölümde görünüyor olabilir. Yalnız “Z’de var mı?” sorusu da bu yüzden yeterince açık değil.

**Diğer ürünler bunu satış ve tahsilatı ayrı izleyerek çözüyor.**

- **Lightspeed:** Veresiye satış, oluşturulduğunda satış raporuna giriyor; ödeme, yapıldığı tarihte ödeme raporuna giriyor. Kapanış raporu satışın açılmasını ve alınan ödemeleri ayrıca gösteriyor. [Lightspeed’in açıklaması](https://x-series-support.lightspeedhq.com/hc/en-us/articles/25534308715547-What-is-the-difference-between-On-Account-and-layaway-payment-types)
- **Lightspeed–Xero aktarımı:** Veresiye satışın kendi faturasına zaten işlenmiş ödeme, gün sonu aktarımında tekrar ödeme olarak sayılmıyor; kapanışta dengeleme satırıyla ayrıştırılıyor. [Kapanış ve mutabakat belgesi](https://x-series-support.lightspeedhq.com/hc/en-us/articles/25534183802779-Register-closure-and-reconciliation-with-Xero)
- **Loyverse:** Beklenen kasa, nakit satışların yanında iade ve satış dışı para giriş/çıkışlarını da dikkate alıyor; sayılan para ayrıca karşılaştırılıyor. Buradaki vardiya raporunu Türkiye’deki mali Z raporuyla eşitlemiyorum; yararlı olan kasa hesabının ayrımı. [Vardiya yönetimi](https://help.loyverse.com/help/shift-management-loyverse-pos)
- **Mikro:** Z raporu veri yapısında nakit/kart satış tutarları ile cari tahsilat bilgi fişi tutarı ayrı alanlar. [Mikro Z raporu alanları](https://apidocs.mikro.com.tr/tablo-alan-adlari/zraporu_hareketleri)

Bizim alacağımız ders bu ayrım. Stok, fatura kesme veya satış satış POS sistemi kurmamız gerekmiyor.

**Raporda düzeltilmesi gereken dört nokta var.**

1. **Veresiye satışın listede olmadığı tespiti doğru.** Mevcut [kayıt listesi](C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Infrastructure/DayCloses/EfDayCloseRepository.cs:363), nakit gelirleri, POS kayıtlarını ve tahsilatları getiriyor; cari borçlandırmayı getirmiyor. Nakitte gösterilmiş fakat tahsil edilmemiş satış için eksikliği kapatmak gerekiyor.

2. **“Tahsilat günü sorunsuz” fazla kesin.** Hesaplama doğru kullanılabiliyor ama kullanıcı kutunun ne anlama geldiğini anlayamıyorsa ürün açısından sorun devam ediyor. Nakit tahsilatı her zaman işaretsiz, kartla tahsilatı her zaman işaretli hazırlamak, girilen toplamın kapsamı bilinmeden güvence sağlamıyor.

3. **Daha geniş bir açık var: “Toplam − kart = nakit”.** [Mevcut kod](C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/DayCloses/DayClosePlan.cs:234) bu hesabı yapıyor. Örneğin satışlar 1.200 TL nakit + 500 TL kart + 300 TL açık hesapsa toplam 2.000 TL’dir. Kullanıcı toplam ve kartı yazdığında uygulama nakdi **1.500 TL** hesaplar. Veresiye cihazda doğru ayrılmış olsa bile 300 TL’yi kasaya girmiş sayar. Aynı sorun başka ödeme türlerinde de doğabilir.

4. **“Kasa sayımı mutlaka gösterir” yeterli güvence değil.** Sayım yapılmayabilir; başka eksik/fazla kayıtlar birbirini dengeleyebilir. Kasanın tutması gelirin de doğru olduğunu kanıtlamaz.

Ayrıca rapordaki “o gün fiş kesmedin → doğru” ifadesi yalnız örneğin uygulama hesabı açısından değerlendirilebilir. Belgenin para gelene kadar ertelenmesini genel bir doğru kullanım olarak sunmamalıyız.

**Benim önerim, mevcut modeli koruyarak gün sonunu şu şekilde düzeltmek.**

**1. Önce girilen tutarın anlamını kesinleştirelim.** Elle girişte ve fotoğraftan okumada “satış toplamı”, “nakit ödeme toplamı”, “cari tahsilat” birbirine karışmamalı. Fotoğraftan yalnız sayı değil, sayının ait olduğu bölüm de anlaşılmalı. Anlaşılamayan alan otomatik olarak sıfır kabul edilmemeli.

**2. Genel toplamdan eksik ödeme türü türetmeyi kaldıralım veya kesin koşula bağlayalım.** Sadece nakit ve karttan oluştuğu bilinmeyen bir toplamdan nakit/kart hesaplanmasın. Z’nin toplamı kontrol için kullanılabilir; açıklanamayan farkın nedeni de tahminle “genelde veresiye tahsilatıdır” diye söylenmesin.

**3. Veresiye satışlar görünsün; fakat her veresiye otomatik olarak nakitten düşmesin.** Davranış şu olmalı:

| Veresiye satışın rapordaki karşılığı | Yapılacak işlem |
|---|---|
| Ayrı açık hesap/veresiye bölümünde | Nakit veya karttan düşülmez; mevcut satışla karşılaştırılır |
| Girilen nakit tutarına dahil edilmiş | Dahil olan tahsil edilmemiş bölüm nakitten ayrıştırılır |
| Girilen tutarlarda yok | Gün sonu hesabını değiştirmez |
| Nerede olduğu bilinmiyor | Otomatik parasal düzeltme yapılmaz; belirsizlik görünür olur |

Bu ihtiyaç, yalnız cari veresiye için değil, uygulamada daha önce gelir yazılmış diğer vadeli satışlar için de aynı ilkeyle ele alınmalı.

**4. Seçimin sonucunu rakamla gösterelim.** Örneğin eski borç tahsilatı için:

> Girilen nakit: **1.500 TL**  
> Bu tutara dahil, önceden kaydedilmiş tahsilat: **300 TL**  
> Eklenecek yeni nakit satış: **1.200 TL**

Veresiye satış içinse açıklama farklı olmalı:

> “Bu satışın parası henüz alınmadı. Girilen nakit tutarına dahil olan 300 TL ayrıldı.”

Kullanıcı bir kutunun muhasebesini zihninde çözmemeli.

**5. Kısmi ödemeyi özellikle koruyalım.** 500 TL satışın 200 TL’si alınmış, 300 TL’si veresiye olabilir. Hem satışın tamamını hem 200 TL tahsilatı aynı toplamdan düşersek bu kez eksik gelir üretiriz. **Aynı tutar iki kez çıkarılmamalı.** Bu nedenle değişiklik sadece ekranda yeni bir satır değildir; kayıt bağlantısı, iptal/geri alma ve testleri de kapsar.

Güvenli otomasyonun sınırı da açık: Uygulama, dış cihazdaki belirsiz bir 1.500 TL’nin içeriğini yalnız kendi kayıtlarından kesin bilemez. Doğrulanmış rapor biçimlerinde hesaplamayı otomatik yapabilir; diğerlerinde kullanıcıya yalnız eksik bilgiyi açıkça sorar. **Bu bilgiyi geliştirici olarak senin esnafmış gibi cevaplaman gerekmez.**

Bir belge çelişkisi de buldum: PRD “nakit esaslı işletme neti” derken [ADR 0014](C:/Users/elma6/Documents/BusinessFinance/documentation/adr/0014-economic-event-recognizes-payment-carries.md) veresiyeyi satış gününde gelir yazmayı açıkça kabul ediyor. Bu incelemedeki örnekler mevcut ADR davranışına göre. Gün sonu düzeltmesi sırasında gelir tarihini sessizce değiştirmemeliyiz; bu terminoloji/model çelişkisi ayrıca netleştirilmeli.

**Önerdiğim öncelik: kategori ve hesap düzenlemelerinden önce, aktif 06.3 içinde bu parasal doğruluk açığını tek bir düzeltme işi olarak ele almak.** İlk adım; veresiye satış, sonraki tahsilat, kısmi ödeme ve farklı rapor toplamlarını kapsayan kabul senaryolarını karar belgesine yazmak olmalı. “Sadece veresiye kutusu ekleme” çözümünü bu haliyle onaylamanı önermiyorum.

Kod veya belge değiştirmedim; build/test çalıştırmadım. İnceleme mevcut kod ve dış kaynaklar üzerinden yapıldı.
