# Bölüm 9 · Ön muhasebe programları: belge, vergi ve müşavir

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Kayıt bir satır değil bir belge olunca ürünün kapsamı nereye kadar genişliyor?

Önceki sekiz bölüm defter tutan beş ürünü izledi: kayıt bir satır ve yazıldığı anda bakiyeyi değiştiriyor. Bu bölüm üç ön muhasebe ürününe bakıyor: KolayBi, Paraşüt ve Logo İşbaşı.

Orada kaydın yerine bir belge geçiyor — ve bu tek bir fark değil, bir zincirin başı. Belge bir karşı tarafa kesiliyor, üstünde vergi taşıyor, içinde kalem taşıyor, ödemesi ayrı bir adım ve sonunda onu işleyen bir muhasebeci var. Bölüm bu zinciri kaynaktan kuruyor.

Bu bölümün kanıt tavanı diğerlerinden düşük ve bu bir kez burada söyleniyor: KolayBi'nin destek görselleri ürünün gerçek ekranları, ama davranışı ölçülmedi. Paraşüt ve Logo İşbaşı'nın iç arayüzü hiç görülmedi; aşağıdaki model onların kendi anlatımından kuruldu.

**Bu bölüme girmez**

- Canlı beş ürünün davranışı → Bölüm 1–8
- Kart dönemi ve taksit → Bölüm 3
- Kapsam ayrımı → Bölüm 6
- Vergi oranlarının doğruluğu ve hesabın denetimi → bu belge hüküm vermez

| Ürün | Kanıt | Not |
|---|---|---|
| KolayBi | Kaynak görseli | Destek görselleri gerçek ürün ekranları; davranış ölçülmedi. |
| Paraşüt | Kaynak beyanı | Kullanım kılavuzu ve ürün sayfası; iç arayüz görülmedi. |
| Logo İşbaşı | Kaynak beyanı | Ürün sayfası ve blog; iç arayüz görülmedi. |


## 9.1 · Üç üründe aynı iskelet: belge doğuyor, ödeme sonra geliyor

Üç kaynağın anlattığı yol neredeyse aynı. Ortak olan şey kaydın iki adıma bölünmesi: önce ekonomik olay bir belgeyle kayda geçiyor, sonra para ayrı bir adımda hareket ediyor.

### Paraşüt · satıştan tahsilata *(Kaynak beyanı)*

Müşteri seçiliyor, satış faturası kesiliyor ve e-belge olarak gönderiliyor. Fatura kesildiği anda cari borç artıyor; kasa ve banka kıpırdamıyor.

Vade geldiğinde otomatik hatırlatma e-postası gidiyor. Tahsilat ayrı bir adım: banka hesabı ve tutar giriliyor, cari bakiye kapanıyor ve banka artıyor.

Mahsuplaştırma otomatik: gelen para en çok gecikmiş açık faturadan başlayarak eşleşiyor.

### Paraşüt · gider tarafı *(Kaynak beyanı)*

Gider beş türe ayrılmış: detaylı fiş/fatura, hızlı fiş/fatura, maaş/prim, vergi/SGK primi, banka gideri.

Kayıt oluşturulduğunda gider raporu artıyor ve tedarikçi carisi borçlanıyor. Ödeme ayrı bir adım ve kasa/banka ancak o adımda azalıyor. Kısmi ödeme destekleniyor.

Cari borçlandırmanın üç yolu var: müşteri kaydında açılış bakiyesi, satış faturası, ve borç yokken ödeme eklemek — sonuncusu avans oluyor.

### Logo İşbaşı · tek kayıt, üç defter *(Kaynak beyanı)*

Fatura kesilirken müşteri seçiliyor ve bakiyesi anında görünüyor; ürün seçiliyor ve kayıt tamamlanıyor.

Kaynağın en ayırt edici cümlesi burada: bir işlem kaydedildiğinde cari hesap, kasa-banka bakiyeleri ve stok seviyeleri birlikte güncelleniyor. Üç defter tek kayıttan besleniyor.

Gider tarafı fişin fotoğrafıyla başlıyor: okuma adımından geçip gider kaydına dönüşüyor, tedarikçi carisi borçlanıyor, ödeme ayrı adımda kasayı azaltıyor.

### KolayBi · formda görünen ikilik *(Kaynak görseli)*

Bu üründe modelin izi doğrudan formda: gider kaydedilirken ödenip ödenmediği ayrıca soruluyor ve cari takibinin açık olup olmadığı ayrı bir seçim.

Kayıt oluştuktan sonra ödeme ayrı bir eylem olarak menüde duruyor. Kayıt ile ödeme arasındaki boşlukta kayıt bir durum rozetiyle bekliyor.

### Logo İşbaşı · sesle fatura *(Kaynak beyanı)*

Kaynak, faturanın butonlarla değil sesle oluşturulduğunu yazıyor ve iki mekanik ayrıntı veriyor: özellik telefonu sallayarak tetikleniyor ve sistemde kayıtlı hazır fatura şablonları üzerinden çalışıyor.

Şablona bağlanması sesin çözmesi gereken alan sayısını azaltıyor; "saniyeler içinde" vaadi bununla tutarlı. Şablonu olmayan bir satışta ne olduğu yazmıyor.

İncelenen öteki sekiz üründe benzeri bir giriş yolu görülmedi. Bir onay adımı ne duyuruda ne rehberde geçiyor; arandı, bulunamadı.

### Ortak sonuç *(Çıkarım)*

Üç kaynakta da aynı iskelet var: belge ekonomik olayı kayda geçiriyor, ödeme parayı taşıyor ve ikisi ayrı anlar.

Bu, canlı beş üründe hiç karşılaşılmayan bir ayrım. Orada kayıt ile para hareketi aynı an — Bölüm 1'de görüldüğü gibi kaydettiğiniz anda bakiye değişiyor.

### Bu bölümün sınırı *(Kaynak beyanı)*

Yukarıdaki akışların hiçbiri çalışırken görülmedi. KolayBi'de form alanları kareyle doğrulandı, ama seçimlerin kasaya ve rapora etkisi ölçülmedi.

Paraşüt'ün tanıtım videosundan alınan kareler ürünün kendi kurgusudur ve bu belgede basılmaz; yalnız anılır.

- Üç ürünün üçü de aynı ayrımı yapıyor ama farklı kelimelerle: biri ödeme durumu, biri ödeme ekleme adımı, biri cari borcun oluşması. Ortak olan, ekonomik olayın para hareketinden önce kaydedilmesi.

## 9.2 · Belgenin kendisi: formda ne var

Modelin izini formda görmek mümkün. Alış faturası formu, canlı beş ürünün işlem formunda hiç bulunmayan alanlar taşıyor: cari zorunlu, ödeme durumu ayrı bir seçim, ve bir seri numarası.

**Şekil 9.1 · KolayBi · Yeni Alış Faturası** (E0200)

- Cari zorunlu; belgenin bir karşı tarafı olmak zorunda.
- Düzenlenme tarihi ve saati ayrı alanlar.
- Seri No ve Ödeme Durumu formda.

**Şekil 9.2 · KolayBi · Genel Giderler listesi** (E0198)

- Üç sekme ve detaylı arama.
- Her satırda Cari Bilgisi kolonu.
- Carisi olmayan kayıtta "Genel Gider" yazıyor.

- Cari alanının zorunlu olması modelin merkezinde duruyor: belge her zaman birine kesiliyor. Canlı ürünlerde sıradan kayıt kimseye ait değil; yalnız bir hesaba ve bir sınıflandırmaya bağlı.
- Carisi olmayan gider için ürün ayrı bir yol bırakmış ve listede "Genel Gider" olarak işaretliyor — yani karşı tarafsız kayıt istisna, kural değil.
- Satış tarafında da aynı iskelet var; form e-arşiv fatura bilgileri bölümü taşıyor.
- Gider detayında ödeme ayrı bir eylem olarak menüde duruyor.

## 9.3 · Belgenin kendisi para taşıyınca: çek ve senet

Modelin en uç noktası burada. Çek ve senet, vadesi olan ve elden ele geçebilen belgeler — yani belgenin kendisi bir ödeme aracı. Canlı beş ürünün hiçbirinde bu kavram yok.

**Şekil 9.3 · KolayBi · Çekler** (E0209)

- Kolonlarda keşideci, hamil ve keşide (vade) tarihi.
- Durum kolonu çekin hangi aşamada olduğunu taşıyor.
- Toplu Çek Ekle, Bordrolar ve Dışarıya Aktar.

**Şekil 9.4 · KolayBi · Senetler** (E0210)

- Aynı kolon yapısı senet tarafında da var.
- Ciro edilmiş senet için ayrı bir not.
- Senet Gösterimi anahtarı toplam özeti açıyor.

- Keşideci ve hamil kolonları belgenin el değiştirdiğini varsayıyor: çeki yazan ile elinde tutan farklı kişiler olabiliyor. Ciro notu da bunun devamı.
- Vade tarihi burada kaydın değil, belgenin özelliği. Bölüm 4'te Wallet vadeyi borcun kendisine, KolayBi ise karşı tarafa bağlamıştı; burada üçüncü bir yer çıkıyor — belgenin üstü.
- Çek ve senet, finans yüzeyinin altı sekmesinden ikisi; banka hesabı ve kasayla aynı düzeyde duruyorlar.

## 9.4 · Vergi belgenin üstünde yaşıyor

Bölüm 1'de beş canlı ürünün işlem formu tek tek okundu; vergi diye bir alan hiçbirinde bulunmadı. İncelenen üç üründen ikisinde ise vergi kendi raporu olan bir konu ve raporun kaynağı ayrı bir kayıt değil, faturanın kendisi. Alanların varlığı kanıtlı; sayının nasıl üretildiği ölçülmedi.

**Şekil 9.5 · KolayBi · KDV Raporu** (E0214)

- Oran sütunları: %20, %18, %10, %8, %1, Diğer ve Toplam.
- Her oranın altında KDV Matrahı ile KDV Tutarı ayrı iki kolon.
- Üst blok Satış Faturası + Alış İade; son satır Hesaplanan KDV Toplam.

**Şekil 9.6 · KolayBi · Alış/Satış Raporu** (E0213)

- Sağ üstte KDV Dahil anahtarı.
- Satırlar belge türü: Satış Faturası 20.000, Alış Faturası −12.000, Toplam 8.000.
- Filtreler arasında Şube, Proje, Etiket ve ayrı bir vade aralığı var.

- Matrah ile tutarın ayrı kolonlar olması, ürünün ikisini ayrı bilgi saydığını gösteriyor. Karedeki tek dolu hücrede 12.000 matraha 2.400 tutar düşüyor — ama tutarın orandan mı hesaplandığı yoksa girilenin mi toplandığı ekrandan çıkmıyor.
- Cevap ürünün geliştirici dokümanında: fatura kaleminde KDV oranı zorunlu ve kalem düzeyinde bir tutar alanı yok; genel giderde ise KDV oran veya tutar olarak verilebiliyor. Yani tek bir kural yok, kayıt tipine göre değişiyor.
- KDV Dahil anahtarı aynı dönemi brüt ve net okutuyor: rapor tek bir doğru sayı dayatmıyor, hangisini istediğinizi soruyor.
- Oran listesi ürünün içinde yaşıyor: %20'den %1'e altı sütun sabit başlık olarak duruyor; izin verilen oranların listesi yayımlanmamış.
- Rapor sekmelerinden biri de Stok Raporu; vergi ve stok aynı yüzeyin iki sekmesi.

**Aynı soruda kaynaktan okunan iki ürün**

- *Kaynak beyanı* · **Paraşüt** — Kılavuzdaki ekran ay ay üç kolon veriyor: Hesaplanan KDV, İndirilecek KDV ve Net KDV. İndirilecek negatif işaretli ve Net KDV negatife düşebiliyor — devreden KDV ekranda gizlenmiyor. Aya tıklanınca satır dökümü açılıyor.
- *Kaynak beyanı* · **Paraşüt · giriş yolu** — Kalem bazında vergi oranı değiştiriliyor ve birim fiyat ile KDV dahil toplam birbirinden hesaplanabiliyor. Elle KDV tutarı girme yolu kaynaklarında geçmiyor: kullanıcı oranı seçiyor, tutarı ürün hesaplıyor.
- *Görülmedi* · **Logo İşbaşı** — Kaynakta vergi raporu anlatılmıyor; e-fatura ve e-arşiv tarafından söz ediliyor ama verginin nerede toplandığı yazılı değil.

## 9.5 · Belge kalem taşıyor: ürün, hizmet ve stok

Belge yalnız bir tutar değil, kalemler taşıyor. Kalem bir ürünse aynı kayıt stoğu da ilgilendiriyor — canlı beş üründe hiç karşılaşılmayan bir zincir.

### KolayBi · varyant, depo ve raf kodu *(Kaynak beyanı)*

Kaynak varyantı kendi cümlesiyle tanımlıyor: aynı ürünün farklı renk ve biçimleri; özellikle tekstilde renk ve beden takibi için.

Kurulum sırası yazılı: önce ayarlardan varyant kullanımı açılıyor, sonra varyant kategorileri ve değerleri tanımlanıyor, sonra ürün üç aşamada oluşturuluyor ve en fazla dört varyant değeri seçiliyor.

Hiç depo açılmasa da bir Ana Depo geliyor; yeni depo açılabiliyor ve depolar Excel ile aktarılabiliyor.

### Stok birimi ürün değil *(Çıkarım)*

Varyantın altında ayrıca "stok" kaydı açılıyor ve o kayıt bir depo ile bir raf kodu istiyor. Stok giriş-çıkışı ve depolar arası transfer varyant kartından yürüyor.

Yani sayılan şey ürün değil, varyant ile deponun çifti. Ürün hareketleri tablosunun ayrı bir Stok/Depo kolonu taşıması da bunu gösteriyor.

### Paraşüt · stok varsayılan açık *(Kaynak beyanı)*

Kaynağa göre stok takibi hesap ilk açıldığında zaten devrede ve abonelik dışında ayrıca ücretlendirilmiyor.

Varsayılan depo, giriş-çıkış deposu, depolar arası transfer ve depo bazında kritik stok seviyesi anlatılıyor; transferin kendi fişi çıkıyor.

### Logo İşbaşı · tek kayıt, üç defter *(Kaynak beyanı)*

Kaynağın kendi cümlesi: bir işlem kaydedildiğinde cari hesap, kasa-banka bakiyeleri ve stok seviyeleri birlikte güncelleniyor.

Ama stok burada tek düzlemde anlatılıyor: ürün kartı, stok hareketi ve bir stok miktarı kolonu. Çoklu depo anlatan bir sayfa arandı, bulunamadı — yokluğu söyleyen bir cümle de yok.

### Raporun içine giriyor *(Kaynak görseli)*

Alış/Satış raporunun alt sekmelerinden biri Ürün ve Hizmetler: aynı dönem ürün kırılımıyla da okunabiliyor. Ayrıca ayrı bir Stok Raporu duruyor.

Kaynağa göre stok raporu yalnız miktar vermiyor: giriş, çıkış, kalan, kritik miktar ve ortalama birim maliyetle birlikte kâr-zarar da taşıyor. Filtrede depo zorunlu görünüyor.

### Canlı beş üründe karşılığı *(Çıkarım)*

Beş üründe kaydın kalemi yok: kayıt bir tutar, bir hesap ve bir sınıflandırma taşıyor. Ne alındığı yalnız serbest metinde yaşıyor.

Bölüm 1'de görüldüğü gibi kaydın adı kullanıcının yazdığı nottan geliyor; ürün adı diye ayrı bir alan hiçbirinde bulunmadı.

### Kalemin bedeli *(Çıkarım)*

Kalem kavramı ürünün kapsamını genişletiyor: aynı kayıt hem parayı hem malı ilgilendiriyor ve "ne kadar kaldı" sorusu kendiliğinden cevaplanıyor.

Bedeli kurulumda görünüyor: varyant önce ayarlardan açılmak zorunda, en fazla dört varyant türü seçilebiliyor ve varyantlı ürün, birleştirme işleminin dışında kalıyor. Sıra yanlışsa geri dönüş maliyetli.

### Bu sayfanın sınırı *(Kaynak beyanı)*

Üç üründe de stok hareketinin kayıtla nasıl bağlandığı çalışırken görülmedi; yukarıdaki her cümle kaynağın kendi anlatımı.

KolayBi'nin depo ve varyant ekranları destek görselinde boş; yapının kendisi metinden kuruldu.

- Kalem, vergi ve cari aynı yerden çıkıyor: belgenin üstünden. Üçü ayrı özellik değil, aynı kurgunun üç sonucu.

## 9.6 · Bölüm 1–4'ün soruları, ön muhasebe tarafında

Önceki bölümlerde canlı ürünlere sorulan sorular, aynı biçimde bu üç ürüne soruldu. Cevaplar kaynak metinlerinden ve destek görsellerinden geliyor.

| Soru | KolayBi | Paraşüt | Logo İşbaşı |
|---|---|---|---|
| Kayıt ne zaman oluşuyor | *Kaynak görseli* · Gider veya fatura kaydedilince; ödeme durumu ayrıca soruluyor | *Kaynak beyanı* · Belge kesilince; ödeme sonra ekleniyor | *Kaynak beyanı* · Fatura veya fişle; fiş fotoğraftan okunuyor |
| Kaydedince hangi defter değişiyor | *Görülmedi* · Ölçülmedi; formda cari takibi ayrı bir seçim | *Kaynak beyanı* · Gider raporu ve tedarikçi carisi; kasa değişmiyor | *Kaynak beyanı* · Cari, kasa-banka ve stok birlikte |
| Para ne zaman hareket ediyor | *Kaynak görseli* · Ödeme Ekle ayrı bir eylem | *Kaynak beyanı* · Ödeme adımında; kısmi ödeme destekleniyor | *Kaynak beyanı* · Tahsilat veya ödeme adımında |
| Karşı taraf zorunlu mu | *Kaynak görseli* · Faturada evet; genel giderde hayır | *Kaynak beyanı* · Tedarikçi eşleştirmesi isteğe bağlı | *Kaynak beyanı* · Firma bilgisi tanımlamadan da fatura kesilebiliyor |
| Tahsilat açık faturaya nasıl bağlanıyor | *Görülmedi* · Ölçülmedi | *Kaynak beyanı* · Otomatik mahsup: en gecikmiş açık faturadan başlayarak | *Kaynak beyanı* · Cari hesap içinde toplanıyor; eşleme kuralı yazılmamış |
| Vergi nereye yazılıyor | *Kaynak görseli* · Oran başına matrah ve tutar; kendi raporu var | *Kaynak beyanı* · Ay bazında hesaplanan, indirilecek ve net | *Görülmedi* · Kaynakta vergi raporu anlatılmıyor |
| Kaydın kalemi var mı | *Kaynak görseli* · Ürün, hizmet, depo ve varyant ayrı bir yüzey | *Görülmedi* · Kaynakta stok anlatılmıyor | *Kaynak beyanı* · Stok, cari ve kasa tek kayıttan besleniyor |
| İşletme/şahsi ayrımı | *Kaynak görseli* · Proje ekseni; kapsam alanı yok | *Kaynak beyanı* · Yok; şahsi harcama ortak/personel carisinden geçiyor | *Kaynak beyanı* · Yok; klasik firma defteri |

- İkinci satır bu tablonun merkezinde: iki kaynakta da kayıt anında kasa kıpırdamıyor. Canlı beş üründe aynı soruya verilen cevap her zaman "bakiye değişiyor" idi.
- Son satır Bölüm 6'yı tamamlıyor: üç ön muhasebe ürününde de kapsam alanı yok ve şahsi tarafın yolu bir borç kaleminden geçiyor.
- Vergi ve kalem satırlarının canlı beş üründe karşılığı yok: Bölüm 1–8 boyunca ne bir vergi alanı ne de bir ürün adı alanı bulundu.

## 9.7 · Muhasebeci nerede duruyor

Üç kaynağın da ortak bir yüzeyi var ve canlı beş üründe karşılığı yok: muhasebeci. Üçü de onu ürünün içine alıyor, ama üç ayrı yetkiyle.

### Logo İşbaşı · müşavir portalı *(Kaynak beyanı)*

Müşavir, müşterisinin verisine erişiyor. Kaynağa göre müşteri onu mali müşavir olarak ekledikten sonra müşavir müşteri adına işlem yapabiliyor ve başka müşterilerini de ürüne davet edebiliyor.

Bu en geniş yetki: veriyi gören ile veriyi değiştirebilen ayrışıyor.

### Paraşüt · canlı erişim *(Kaynak beyanı)*

Kaynak muhasebeciye canlı erişim verildiğini yazıyor; ay sonunda dosya gönderme adımı ortadan kalkıyor.

Erişimin kapsamı — yalnız görme mi, işlem yapma mı — kaynakta ayrıntılı anlatılmıyor.

### KolayBi · davet *(Kaynak görseli)*

Panoda müşavir daveti girişi görünüyor; davetin sonrasında müşavirin ne yapabildiği bu karelerden çıkmıyor.

Ayrıca cari yapısının içinde personel carileri var: çalışan da bir karşı taraf ve maaş ödemesi o cariden geçiyor.

### Canlı beş üründe karşılığı *(Çıkarım)*

Beş üründe muhasebeci diye bir kavram yok. Veri dışarı ancak bir dosyayla çıkıyor: PDF, Excel veya e-posta eki.

Yani ilişki tek yönlü ve zamansız: kullanıcı dosyayı gönderiyor, muhasebeci o anın kopyasını alıyor. Ön muhasebe tarafında ilişki çift yönlü ve sürekli.

### Yetkinin bedeli *(Çıkarım)*

"Müşteri adına işlem yapma" yetkisi, verinin sahibi ile onu değiştirebilen kişiyi birbirinden ayırıyor.

Bu ayrım bir soru doğuruyor: kaydı kimin yazdığı ve kimin sildiği nerede tutuluyor? Kaynaklarda bir iz katmanından söz edilmiyor.

### Bu sayfanın sınırı *(Kaynak beyanı)*

Üç ürünün hiçbirinde müşavir yüzeyi çalışırken görülmedi. Logo'nun video karesinde bir portal arayüzü görünüyor ama içeriği boş şablon çubukları.

Yetkilerin gerçekte nasıl sınırlandığı ölçülmedi.

- Muhasebecinin ürünün içinde olması, bu üç ürünün hedef kitlesinin bir uzantısı: belge üreten bir işletmenin bir de belgeleri işleyen biri var.

## 9.8 · Ön muhasebe yolu ve canlı ürünlerden ayrıldığı nokta

Aynı yol, bu kez kaynaktan. Ayrışma ikinci adımda başlıyor ve sonraki her adımı belirliyor.

**1. Ekonomik olay oluyor**

Bir iş yapılıyor, bir mal alınıyor ya da bir hizmet veriliyor. Sekiz bölümün hepsinde yol buradan başlıyor.


**2. Kayda nasıl geçiyor**

- *Canlı beş ürün* — Bir satır olarak. Kaydedildiği anda hesap bakiyesi değişiyor. (E0142)
**→** *KolayBi · Paraşüt · Logo İşbaşı* — Bir belge olarak: fatura ya da fiş. Cari borç o anda oluşuyor, kasa kıpırdamıyor. (E0200)

**3. Vergi nereye yazılıyor**

- *Canlı beş ürün* — Hiçbir yere. Dokuz bölümde vergi alanı hiç bulunmadı. (E0229)
**→** *KolayBi · Paraşüt* — Belgenin üstüne. Oran ve matrah faturanın kalemi; rapor kayıtları yeniden okumaktan ibaret. (E0214)

**4. Para ne zaman hareket ediyor**

- *Canlı beş ürün* — Aynı anda. Kayıt ile para hareketi tek bir olay. (E0075)
**→** *KolayBi · Paraşüt · Logo İşbaşı* — Ayrı bir ödeme veya tahsilat adımında. Arada kayıt ödenmemiş olarak bekliyor. (E0199)

**5. Kim görüyor**

- *Canlı beş ürün* — Yalnız kullanıcı. Veri dışarı ancak bir dosyayla çıkıyor. (E0169)
**→** *Paraşüt · Logo İşbaşı* — Muhasebeci de. Erişim sürekli ve bir üründe müşteri adına işlem yapmaya kadar gidiyor. (isbasi.com)

- Bu akışın iki dalı da aynı ekonomik olayı anlatıyor; ayrışan şey olayın ne zaman paraya dönüştüğü.
- Dördüncü adımdaki bekleme süresi, ön muhasebe tarafının bütün cari ve vade kavramlarının kaynağı: ödenmemiş kayıtlar bir yerde durmak zorunda.

## 9.9 · Neden ayrışıyorlar

Bu üç ürün aynı işi daha kötü yapmıyor; başka bir işi yapıyor. Fark, kullanıcının kim olduğu varsayımından çıkıyor ve bütün modeli belirliyor.

### Belge, olayı paradan ayırıyor *(Çıkarım)*

Üç kaynakta da kayıt iki adıma bölünmüş: belge ekonomik olayı kayda geçiriyor, ödeme parayı taşıyor. Fatura kesildiğinde cari borç oluşuyor ama kasa kıpırdamıyor; kasa ancak ödeme adımında değişiyor.

Canlı ürünlerde bu ayrım genel bir kural değil, yalnız iki yerde beliriyor: kart harcaması borcu yazıp ödemeyi sonraya bırakıyor (Bölüm 3), Bluecoins'in cari hesabı faturayı gelire yazıp tahsilatı sonraya bırakıyor (Bölüm 4). Sıradan bir gelir ya da gider kaydında ise Bölüm 1'de görüldüğü gibi kayıt yazıldığı anda bakiye değişiyor — tek adım, tek an.

Ayrımın bedeli ve kazancı aynı yerden geliyor: iki adım olduğu için ödenmemiş kayıtların bir yerde durması gerekiyor, ve cari kavramı tam da o yer.

*Dayanağı: E0192, E0199, E0200, parasut.com kılavuzu, isbasi.com*

### Belgenin bir karşı tarafı olmak zorunda *(Çıkarım)*

Fatura formunda cari alanı zorunlu: belge her zaman birine kesiliyor. Carisi olmayan gider için ayrı bir yol bırakılmış ve listede "Genel Gider" olarak işaretleniyor — yani karşı tarafsız kayıt istisna.

Canlı ürünlerde sıradan kayıt kimseye ait değil; yalnız bir hesaba ve bir sınıflandırmaya bağlı. Wallet'ın borç nesnesi ve Bluecoins'in cari hesabı istisna, ama ikisinde de karşı taraf kaydın kendisinde değil, ayrı bir nesnede ya da hesapta duruyor.

Karşı tarafın zorunlu olması, cari bakiyesini ve vadeyi kendiliğinden mümkün kılıyor: kime ne kadar borçlu olduğunuz zaten kayıtlı.

*Dayanağı: E0200, E0198, E0316*

### Bir kaydın kaç defteri beslediği ürünün kapsamını gösteriyor *(Çıkarım)*

Logo'nun kaynağı en açık cümleyi kuruyor: bir işlem kaydedildiğinde cari, kasa-banka ve stok birlikte güncelleniyor. Üç defter tek kayıttan besleniyor.

Canlı ürünlerin dördünde defter sayısı bir. Goodbudget'ta iki — zarf ve hesap — ve Bölüm 1'de görüldüğü gibi ikisi tutmuyor.

Defter sayısı arttıkça kaydın tek bir anda doğru olması zorlaşıyor; bunun karşılığında stok ve cari gibi sorular kendiliğinden cevaplanıyor.

*Dayanağı: isbasi.com, E0417, E0421*

### Vergi belgenin üstünde durduğu için raporlanabiliyor *(Çıkarım)*

Oran ve matrah faturanın kalemi olduğu için vergi raporu kayıtları yeniden okumaktan ibaret; ürün ayrıca bir yere bakmıyor. Biri oran başına matrah ve tutarı ayrı sütunlarda topluyor, ötekinin kaynağı ay bazında üç sayı ürettiğini yazıyor.

Canlı beş üründe vergi diye bir alan Bölüm 1'in form taramasında bulunmadı; dolayısıyla böyle bir rapor da yok. Bu bir eksiklik değil, ayrı bir kullanıcı varsayımı — beyanname veren biri ile ayını takip eden biri aynı kişi değil.

Bedeli sorumluluk: oran listesi ürünün içinde yaşıyor. Oran değiştiğinde listeyi güncel tutmak ürüne ya da kullanıcıya düşüyor; güncellenmezse yeni belgeler eski oranla kesiliyor. Kazancı, ayın en zahmetli işinin hazır çıkması.

*Dayanağı: E0214, E0213, parasut.com kılavuzu*

### Üründeki belge ile resmî belge aynı şey değil *(Çıkarım)*

Üründe bir fatura oluşturmak, onu resmî bir fatura yapmıyor. Bir kaynak bunu kendi cümlesiyle söylüyor: üründe oluşturulan fatura kayıtlarının resmî değeri yok; resmîlik ya matbu çıktıyla ya da ayrıca satın alınan bir e-belge modülüyle geliyor.

Üçünde de e-belge abonelikten ayrı fiyatlanıyor ve kontör denen ikinci bir sayaca bağlanıyor. İki kaynak gelen faturanın da kontör yaktığını yazıyor — yani gider tarafı, kullanıcının kendi kararına bağlı olmayan bir tüketim kalemi taşıyor.

Bu, bölümün ana ayrımını bir kat daha açıyor: kayıt ile ödemenin ayrılması üründe olan bir şey, belgenin resmîleşmesi ise ürünün dışında — vergi idaresiyle ve ayrı bir bedelle.

*Dayanağı: isbasi.com işlem rehberi, parasut.com kılavuzu, kolaybi.com fiyat sayfası*

### Muhasebecinin ürünün içinde olması modeli tamamlıyor *(Çıkarım)*

Belge üreten bir işletmenin bir de o belgeleri işleyen biri var. Üç kaynak da muhasebeciyi ürünün içine almış: biri davetle, biri canlı erişimle, biri müşteri adına işlem yapabilen bir portalla.

Canlı ürünlerde muhasebeciye ayrılmış bir erişim yok; veri dışarı bir dosyayla çıkıyor. İlişki tek yönlü ve zamansız: kullanıcı o anın kopyasını gönderiyor.

En geniş yetkiyi veren kaynak "müşteri adına işlemleri gerçekleştirebilir" deyip orada duruyor: yetkinin sınırını, bir rol tanımını ve kaydı kimin yazdığını gösteren bir iz katmanını anlatan sayfa arandı, bulunamadı.

*Dayanağı: isbasi.com, parasut.com kılavuzu, E0187*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **KolayBi** | Model formda görünüyor: ödeme durumu ve cari takibi ayrı seçimler; cari, proje, çek-senet ve personel carisi birinci sınıf kavramlar; stok birimi varyant ile deponun çifti; vergi oran başına matrah ve tutar olarak raporlanıyor | Fatura her zaman bir cariye kesiliyor ve e-belge abonelikten ayrı, kontörle fiyatlanıyor; kayıt canlı ürünlerdeki tek satırdan uzun bir iş |
| **Paraşüt** | Kaynağa göre gider beş türe ayrılmış, kısmi ödeme destekleniyor, tahsilat en gecikmiş açık faturadan başlayarak otomatik mahsuplaşıyor, stok ek ücretsiz ve çoklu depolu, vergi ay bazında hesaplanan/indirilecek/net olarak çıkıyor | Kaynağa göre e-belge abonelikten ayrı bir modül ve kontörle fiyatlanıyor; kayıt ile ödeme iki ayrı adım olduğu için her gider iki kez ele alınıyor |
| **Logo İşbaşı** | Kaynağa göre tek kayıt üç defteri birden besliyor; fiş fotoğraftan okunuyor, fatura telefon sallanarak ve hazır şablonla sesle kesilebiliyor, müşavir portalı ay sonu dosya alışverişini kaldırıyor | Ürünün kendi ifadesiyle üründe oluşturulan fatura kayıtlarının resmî değeri yok; e-fatura ayrı modülle geliyor. Müşavir müşteri adına işlem yapabiliyor ama yetki sınırı ve kaydı kimin yazdığını gösteren iz kaynakta tanımlı değil |

**Belge 3'e taşınan soru**

- Uygulama vergi tutarını hesaplamalı mı, yoksa kullanıcının girdiğini taşıyıp raporlamalı mı? Hesaplayan ürün ayın en zahmetli işini üstleniyor; karşılığında oran listesini güncel tutma sorumluluğunu ve yanlış sayının sonucunu alıyor.
- Kaydın bir kalemi olmalı mı? Kalem taşıyan üründe "ne kadar kaldı" sorusu kendiliğinden cevaplanıyor; taşımayanda kayıt daha kısa ve ne alındığı serbest metinde kalıyor.
- Ekonomik olayın kaydı ile para hareketi ayrı iki adım mı olmalı? Üç kaynak her kayıtta ayırıyor; canlı ürünler yalnız kartta ve caride ayırıyor, ve ikisi de kendi kullanıcısı için tutarlı.
- Kaydın bir karşı tarafı olmak zorunda mı? Zorunlu olduğunda cari bakiyesi ve vade kendiliğinden geliyor; olmadığında kayıt daha hızlı giriliyor.
- Bir kayıt kaç defteri beslemeli? Defter sayısı arttıkça kapsam genişliyor, tutarlılık zorlaşıyor.
- Muhasebeci ürünün içinde mi olmalı, dışında mı? İçindeyse veriyi değiştirebilen kişiyle sahibini ayıran bir iz katmanı gerekiyor.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 9.1 | E0200 | KolayBi | Kaynak görseli | KolayBi · Yeni Alış Faturası |
| 9.2 | E0198 | KolayBi | Kaynak görseli | KolayBi · Genel Giderler listesi |
| 9.3 | E0209 | KolayBi | Kaynak görseli | KolayBi · Çekler |
| 9.4 | E0210 | KolayBi | Kaynak görseli | KolayBi · Senetler |
| 9.5 | E0214 | KolayBi | Kaynak görseli | KolayBi · KDV Raporu |
| 9.6 | E0213 | KolayBi | Kaynak görseli | KolayBi · Alış/Satış Raporu |
