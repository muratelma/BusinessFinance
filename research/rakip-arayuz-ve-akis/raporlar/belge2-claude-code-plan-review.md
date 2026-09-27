# Claude Code Plan Review — Belge 2

20 Eylül 2026 · GPT incelemesi

> Önceki değerlendirme aşağıda değiştirilmeden kaydedilmiştir. Sonundaki ayrı ek, kullanıcının incelemeden sonra verdiği bağlayıcı kurgu düzeltmesini içerir. A/C sıralamasında bu son kullanıcı kararı geçerlidir; önceki değerlendirmenin toplu A → toplu C yaklaşımına ilişkin önerileri tarihsel kalır. Metin değişmedi; yalnız bağlantılar 20 Eylül devir kontrolünde mutlak Windows yolundan göreli yola çevrildi. Plana verilen satır numaraları **v1'e aittir** ve v2'de aynı yere düşmez; gözlem formlarına ve üretim planına verilenler kontrol edildi, hâlâ doğru satırı gösteriyor.

## Önceki değerlendirme — aynen

**Ben olsam A → C kurgusunu korurdum; fakat bu planı mevcut hâliyle doğrudan bölüm yazımına vermezdim.** Konu ve kanıt eşlemesi güçlü. Asıl sorun, bazı yerlerde kanıtın taşıdığından daha geniş sonuç yazılması ve biçim kurallarının araştırmanın önüne geçmesi.

[Planı](belge2-bolum-plani.md), özgün üretim planını, ilgili tema kayıtlarını, kanıt envanterini ve kritik gözlem notlarını karşılaştırdım. Bu değerlendirme belge ve kayıt incelemesidir; bütün ekran görüntülerünü yeniden açarak yapılmış ikinci bir görsel denetim değildir.

**En önemli önerim:** Belge 2’nin merkezini “sayı tabloları” olarak değil, **“hangi koşulda, hangi kullanıcı eylemi, hangi gözlenebilir sonucu doğurdu?”** olarak tanımlamak. Sayılar bunun önemli bir parçası; kayıt oluşması, beklemesi, bağlanması, silinmesi ve dışarı aktarılması da aynı derecede önemli.

**1. Planın koruyacağım güçlü tarafları**

- **Önce olaylar, sonra ürünler:** Okur önce aynı işi ürünler arasında karşılaştırabiliyor, ardından tek ürünün işleyişini bütün olarak görebiliyor. İki farklı okuma ihtiyacını karşılıyor.
- **Kanıt derinliğine göre farklı ağırlık:** Her ürüne eşit sayfa vermemek doğru. Kaynakla bilinen bir ürünü iki sayfaya tamamlamak için çıkarım üretmekten kaçınılıyor.
- **Bölümlerin dışında kalan konuların belirtilmesi:** Uzun bir raporda tekrarları kontrol etmek için iyi bir başlangıç.
- **Seçenek, gerçekleşme ve sonuç ayrımı:** Özellikle “seçenek ≠ dosya ≠ teslim”, “ek dosya ≠ OCR”, “menü ≠ hesaplama doğrulaması” ayrımları çok değerli.
- **Eksiklerin açıkça yazılması:** Belirsizliklerin saklanmaması raporun güvenilirliğini artırır.
- **Senaryoyu kanıttan seçme yaklaşımı:** Önce hikâye yazıp sonra onu destekleyecek ekran aramak yerine mevcut zincirden hareket edilmesi doğru.

Bunlar küçük biçim tercihleri değil; araştırmanın sağlam temeli. Değişiklik önerilerim bu temeli güçlendirmeye yönelik.

**2. Önce düzelteceğim konu: “Altı etki” özgün kapsamı tam karşılamıyor**

Plan, üretim belgesini şöyle aktarıyor:

> “hesap, borç, gelir/gider, rapor, zaman ve geri alma etkileri ayrı anlatılır.”

Ancak özgün cümlede önemli bir koşul var:

> “Her senaryoda **uygun olan** hesap, borç, gelir/gider, rapor, zaman ve geri alma etkileri ayrı anlatılır.”

Bu ifade [üretim planında](../FAZ7-8-UYGULAMA-PLANI.md) (satır 321) bulunuyor. Yeni planda “uygun olan” kaldırılarak her olay sayfasına zorunlu altı satır ve üretimi durduran bir kontrol eklenmiş.

Bunun iki sonucu var:

- PDF dışa aktarma, belge ekleme veya sınıflandırma gibi işlere zorla borç/bakiye satırları koyulabilir.
- “Bu olay için uygulanmaz” ile “araştırılmadığı için bilinmiyor” birbirine karışabilir.

Ayrıca özgün **“rapor etkisi”**, yeni tanımda **“ekrandaki genel toplam”** olmuş. Bunlar eşdeğer değil. Bir kayıt kategori raporunun içeriğini değiştirebilir ama genel toplamı değiştirmeyebilir. Kapsam değişikliği, filtre, bütçe tüketimi veya rapordaki kayıt sayısı da yalnız genel toplamla anlatılamaz.

**Benim çözümüm:**

- Altı boyutu her olay için **değerlendirme kontrol listesi** olarak korurdum.
- “Rapor ve diğer göstergeler” boyutunda genel toplam, bütçe/zarf kalanı, rapora dahil olma ve kırılımları ayırırdım.
- Görünür tabloda olayın gerektirdiği satırları kullanırdım.
- Denetim, “altı satır basılmış mı?” yerine “boyutlar değerlendirilmiş mi; uygulanmayanın gerekçesi var mı?” sorusunu kontrol ederdi.

Kullanıcının seçtiği altı satırlı ana karşılaştırma düzeni korunacaksa da en azından **“görülmedi” ve “uygulanmaz” ayrı değerler olmalı.** Etkisiz olduğu ölçülen bir sonuç ayrıca “değişmedi” diye yazılmalı.

**3. Asimetri önemli, fakat plandaki teşhisi yanlış**

[Kısım II’de](belge2-bolum-plani.md) (v1, satır 348) şu genelleme yapılıyor:

> Pipeline bölümü kaynakla incelenen altı üründe yazılı, en derin koşulan üç üründe yazılı değil.

Burada **belge yapısı ile kanıt yöntemi karıştırılmış.**

Pipeline başlığı bulunan altı ürünün ikisi **Hesap Defterim ve Goodbudget**. Bunlar canlı incelenmiş ürünler. İlgili bölümler de manuel gözlem ve çıkarım taşıyor:

- [Hesap Defterim pipeline bölümü](../gozlemler/hesap-defterim.md) (satır 259)
- [Goodbudget pipeline bölümü](../gozlemler/goodbudget.md) (satır 200)

Dolayısıyla “başlık varsa ürünün kendi anlatımı, yoksa bizim çıkarımımız” sonucu geçerli değil.

**Ben bunu iki ayrı eksende anlatırdım:**

| Eksen | Sorusu |
|---|---|
| Belgeleme durumu | Gözlemler tek bir işleyiş bölümünde toplanmış mı, dağınık mı? |
| İddianın dayanağı | Canlı gözlem mi, koşum kaydı mı, resmî kaynak mı, araştırmacı çıkarımı mı? |

MM/BC/Wallet için yapılacak iş, mevcut gözlemleri aynı şablonda birleştirmek. Bu işlem otomatik olarak yeni kanıt toplamak veya bütün modeli “çıkarım” saymak demek değil.

**Asimetri raporun kusuru değildir; asimetriyi yanlış sınıflandırmak kusurdur.** Bir ürünün aynı sayfasında farklı dayanaklar olabilir. Etiket ürünün tamamına değil, ilgili iddiaya bağlanmalı.

**4. Sayısal anlatımda düzeltilmesi gereken somut örnekler var**

Burası benim için biçim tartışmalarından daha öncelikli.

**Money Manager taksit örneğinde neden–sonuç eksik aktarılmış.**

Planın 5.4 satırında:

> Ağustos’ta 1.000 gider — 2.050 → 3.650

yazıyor. Fark **1.600**. Gözlem kaydı ve envanter bunun **1.000 taksit + 600 abonelik** olduğunu açıklıyor. Plan bu bağlamı düşürmüş. [Gözlem kaydı](../gozlemler/money-manager.md) (satır 205)

Doğru anlatım şu olur:

> Ağustos listesinde 1.000 tutarlı ilk taksit görülüyor. Toplam giderin 2.050’den 3.650’ye yükselmesi, taksit ile 600 tutarlı aboneliğin birleşik sonucudur.

Arada ayrı ölçüm yoksa yalnız taksidin etkisini kanıtlayan temiz bir önce/sonra çifti varmış gibi gösterilmemeli.

**Gün/hafta netinin sıfır olması, gelir ve gider toplamlarının ayrı ayrı değişmediğini kanıtlamaz.**

5.2’de Bluecoins için gün başlığı, Wallet için hafta toplamı gelir/gider etkisinin dayanağı olarak kullanılıyor. Oysa iki karşıt hareket neti sıfır tutarken brüt giriş ve çıkış toplamlarını artırabilir. Hesap Defterim karşılaştırması zaten bu ayrımın neden önemli olduğunu gösteriyor.

Üstelik E0022 ve E0279 eski işlem listesi kanıtları; ilgili kısmi ödeme deneyinin doğrudan rapor sonucu gibi kullanılamazlar.

Burada şu iki iddia ayrılmalı:

- “İşlem listesindeki net değişmedi.”
- “Gelir ve gider raporları ayrı ayrı değişmedi.”

İkincisi için ilgili rapor ve koşum bağlantısı gerekir.

**Sonradan alınan rapor, geçmiş deneyin eksik sonrasını kendiliğinden tamamlamaz.**

§7’de Money Manager için bugün Eylül raporunu açmak öneriliyor. Bu faydalı olabilir; ancak sonraki kayıtlar, silmeler ve düzenlemeler varsa 10 Eylül’deki 400 ödemenin etkisini tek başına izole etmez.

Yeni raporun hangi iddiayı kapatabileceği önceden yazılmalı. “Kare aldık, eski boşluk kapandı” yaklaşımı yeterli değil.

**5. Plan bazı yerlerde kendi tarafsızlık kuralını ihlal ediyor**

En açık örnek [D3’ün gerekçesi](belge2-bolum-plani.md) (v1, satır 511):

> HD’de Denge doğru kalırken toplamlar bozuluyor.

“Toplam Alındı/Ödendi” kendi tanımı gereği transferleri kapsıyor olabilir. Bunları saf gelir/gider toplamı gibi kullanmak yanlış olabilir; fakat bu, ürünün kendi toplamının bozuk olduğunu tek başına göstermez.

Ben şöyle yazardım:

> Aktarım, birleşik görünümde Alındı ve Ödendi toplamlarını artırırken Denge’yi değiştirmiyor. Bu toplamlar saf gelir/gider ölçüsüyle aynı içeriği taşımıyor.

Benzer biçimde:

| Plandaki ifade | Önerdiğim düzeltme |
|---|---|
| “MM’de dört taksit hiçbir toplamda yok” — 13.3 | “İncelenen Bu Ay/Gelecek Ay borç toplamlarında sonraki dört taksit görünmüyor.” |
| “MM hiçbiri” — 8.2 | Hangi araçların hangi yüzeylerde aranıp görülmediğini belirt. |
| “MM/HD’de ayrım yok” — 13.2 | “İncelenen akışlarda ayrı bir doğum–ödeme zinciri kurulmadı/gözlenmedi.” |
| “Bulunmayan nesneler” — T8 | “İncelenen yüzeylerde gözlenmeyen kavramlar.” |
| “Dönemsiz / kavram yok” — 5.5 | Gözlenen bakiye biçimini ve tarama sınırını ayrı anlat. |

**§9’daki “Plan rakibi puanlıyor mu?” sorusuna cevabım:** Genel kurgu puanlamıyor; fakat bazı cümleler BusinessFinance’in beklediği anlamı rakibe ölçüt yapıyor. Bunlar yazım öncesinde temizlenmeli.

**6. Benim ekleyeceğim ana yapı: her olayın kısa bir işlem zinciri**

Şu an plan “kaydedildikten sonra paraya ne oldu?” sorusunda güçlü. Fakat özgün soru aynı zamanda **“kullanıcı işi nasıl tamamlıyor?”**

Bunu yalnız Belge 1’e bırakmak doğru olmaz. Belge 1 formun görünüşünü anlatır; Belge 2 işin tamamlanma koşulunu anlatmalı.

Her olayın başına kısa bir şerit koyardım:

**Başlangıç durumu → kullanıcı eylemi → onay/otomatik adım → oluşan kayıt veya durum → gözlenen sonuç**

Altına şu bilgileri eklerdim:

- İş hangi noktada tamamlanmış sayılıyor?
- Kullanıcının ayrıca yapması gereken bir adım var mı?
- Oluşan kayıt nereden tekrar bulunabiliyor?
- İşlem hangi kayıtla bağlantılı?
- Hangi hata, vazgeçme veya düzeltme yolu gerçekten denendi?

Örneğin planlı ödeme için “600 gider oluştu” tek başına yeterli değil. Planın kendisi mi değişti, bir örnek mi kapandı, yeni bir işlem mi göründü, sonraki vade mi ilerledi? Bunlar Belge 2’nin esas malzemesi.

**“Listede nasıl görünüyor?” ayrı yedinci mali etki olmak zorunda değil.** Fakat **“hangi kayıt/durum oluştu?”** başlığı mutlaka olmalı. Satırın rengi Belge 1’e, kaydın varlığı ve bağlantısı Belge 2’ye aittir.

**7. Bölüm sınırlarında yapacağım değişiklikler**

14 bölümü hemen kaldırmazdım. Fakat bazı içeriklerin sahibini daha kesin belirlerdim.

| Konu | Ana sahibi | Diğer bölümlerde kullanım |
|---|---|---|
| Kart harcaması, kart borcu ve ödeme | Bölüm 5 | Borç/cari bölümünde kısa ayrım |
| Plan tanımı, örnek üretimi, onay ve atlama | Bölüm 7 | Kart taksitinde ilgili bağlantı |
| Bir olayın rapora etkisi | Olayın kendi bölümü | Bölüm 9’da toplamın genel tanımı |
| Cari bakiyenin ve tahsis bağının anlamı | Bölüm 6 | Ekstre çıktısında kısa bağlam |
| Ekstre dosyası, kolonları ve teslimi | Bölüm 10 | Bölüm 6’dan gönderme |
| Tek ürünün bütünsel işleyişi | Bölüm 12 | Önceki tabloların tekrarından kaçınma |
| Bölümler arası sonuçlar | Bölüm 13 | Yeni kanıt veya yeni kesin hüküm üretmeme |

Özellikle üç ekleme yapardım:

- **Borç/alacak yönü:** Bölüm 6 ağırlıkla alacağın doğması ve tahsilatı anlatıyor. Borçlanma ve ödeme yönünün hangi ölçüde incelendiğini ayrıca belirtirdim. Alacak deneyinin sonucunu ters yöne otomatik taşımam.
- **Yaşam döngüsünün kapanışı:** Kısmi ödeme kadar tam kapanış, iptal, silme ve geri yükleme sınırını da görünür kılardım. Kanıt yoksa yeni büyük bölüm değil, kısa kapsam kaydı yeterli.
- **Belgeden kayıt üretimi:** Bölüm 10’da ek dosya ile OCR ayrımı var; fakat Paraşüt/Logo’nun kaynakta anlatılan fiş okuma akışları açık bir alt soruyla temsil edilmeli. Tema 09’da bu malzeme mevcut.

Bölüm 11’deki **“finans dışı modüller”** ifadesini de değiştirirdim. Maaş, çek ve senet finans dışı değil. “Diğer modüller ve yardımcı araçlar” daha doğru.

Bunları değerlendirirken araştırmayı yalnız Aşama 06.2’ye daraltmadım. Araştırma README’si açıkça bütün ürün için yapıldığını söylüyor; rakiplerin stok, banka veya vergi özelliklerini anlatmak Belge 2 kapsamında kalmalı.

**8. Dokuz tablo tipi kullanılabilir; biçim kuralları fazla katı**

Dokuz tipin tamamı gereksiz değil. Ancak hepsi tablo değil:

- T1/T2/T4/T5/T6 gerçek tablo yapıları.
- T3 terim şeridi.
- T7 kaynak açıklaması.
- T8 şema.
- T9 sınır notu.

Bunları **“anlatım bileşenleri”** olarak adlandırırdım.

Özellikle şu kuralı kaldırırdım:

> Arka arkaya ikiden fazla aynı tip tablo gelirse bölüm yanlış kurulmuştur.

Üç ardışık sorunun aynı yapıda olması mümkündür. Sırf çeşitlilik için tablo değiştirmek karşılaştırmayı zorlaştırabilir. Ölçüt, tip sayısı değil **sorunun en açık nasıl anlatıldığı** olmalı.

Başka değişikliklerim:

- Beş canlı ürünü her tabloda zorunlu sütun yapmam. Konuya göre ilgili ürünler karşılaştırılır; diğerlerinin durumu kısa notta belirtilir.
- Her sayfada zorunlu iki–üç bilinmeyen istemem. Gerçekten bulunan sınır yazılır; sayfa doldurmak için belirsizlik üretilmez.
- Kanıt numaralarını yalnız sayfanın altına yığmam. Hücrede kısa dipnot işareti, altta karşılığı yeterli olur.
- T3’te ürün terimlerini eşanlamlıymış gibi sıralamam. “Bu Ay”, “Kesim Günü” ve “Payment Due Date” aynı tür kavramlar değil; farkları belirtilmeli.

**Sayfa tahmini de yeniden hesaplanmalı.** Bölüm 3–11’in kendi tahminleri toplam **48 sayfa** ediyor. Bölüm 12 ve 13 eklenince **64–66**; senaryolarla **68–72**. Okuma kısmı ve kanıt eki bunun dışında. Dolayısıyla 40–46 tahmini mevcut alt tahminlerle uyuşmuyor.

D9’da da küçük bir çelişki var: Kısım II için dokuz kare sayılıyor, fakat Paraşüt, Logo ve QuickBooks karelerinin basılmayacağı söyleniyor. Mevcut kuralla burada en fazla altı ürün karesi beklenir.

**9. Kanıt toplama listesini “hangi iddiayı kapatıyor?” sorusuyla yeniden sıralardım**

§7’deki listenin varlığı iyi. Ancak “kolayca ekran alınır” ile “ana iddia doğrulanır” aynı şey değil.

Benim sıralamam:

| Öncelik | İş | Gerekçe |
|---|---|---|
| Önce mevcut kayıtlar | Taksit rakamları, koşum tarihleri, ödeme kanıtları ve yokluk ifadelerini uzlaştır | Yeni koşum gerektirmeyen yanlışlar önce temizlenmeli |
| Yazımı etkileyen eksikler | MM kalan taksitlerin görünümü; ilgili ödeme–rapor bağlantıları | Kart bölümünün ana sonucunu etkiliyor |
| Bölüme göre önemli | Goodbudget normal gelir yolu | “Gelir kaydı” anlatımının yalnız Credit yoluna dayanmasını önler |
| İkinci tur | Wallet Postpone/Dismiss, BC geri yükleme, otomatik tekrar | Yaşam döngüsünü derinleştirir |
| Tamamlayıcı | Formdaki hazır tutar, dışa aktarma girişinin yeri | Daha sınırlı iddiaları kapatır |

Goodbudget’ta **From New Income formunu açmak**, o yolun gelir/hesap/zarf etkisini doğrulamaz. Benzer şekilde BC’de otomatik kutuyu işaretlemek, otomatik üretimin gerçekleştiğini göstermez. Her testin **başlangıç, yapılacak eylem, bakılacak sonuç ve yeterli sayılma koşulu** yazılmalı.

“Önerilmez” listesini de ikiye ayırırdım:

- **Önemli ama mevcut koşum kısıtlarıyla yapılamayanlar.**
- **Bu raporun ana iddiasına katkısı düşük olduğu için ertelenenler.**

Kart borcunu tam kapatma ilk gruba girebilir. Mevcut bakiyeyi değiştirmesi testin önemsiz olduğunu göstermez; mevcut veriyi koruma kuralıyla çatıştığını gösterir. Kullanıcının koruma kararını değiştirmeden sınır olarak tutulmalı.

HD’nin PDF içeriği için de dosya üretme, cihazda inceleme ve üçüncü kişiye gönderme birbirinden ayrılmalı. Paylaşım gerekliliği gerçekten kaçınılmazsa belirtilmeli; dosya incelemesi doğrudan teslimle eşitlenmemeli.

**10. Senaryo bölümü kalabilir; “tam zincir” daha sıkı tanımlanmalı**

D6’daki mevcut kanıtlardan zincir taraması önerisine katılıyorum.

Ancak Money Manager örneğinde iki farklı tarihte alınmış kareleri birleştirmek için yalnız tarihleri söylemek yeterli değil:

- Aynı kayıtlar mı izleniyor?
- Arada silme/düzenleme olmuş mu?
- Filtre ve dönem aynı mı?
- Gözlenen şey zamanın ilerlemesi mi, başka dönemin ekranını açmak mı?

Envanter E0256 için **15 Eylül tarihli taksidin 12 Eylül’de zaten görüldüğünü** kaydediyor. Dolayısıyla bu kare “sonraki tarihte otomatik oluştu” sonucunu kanıtlamaz; sonraki dönem görünümünü kanıtlar.

Ben senaryoları şöyle ayırırdım:

- **Kesintisiz gözlenen zincir**
- **Farklı oturumlardaki kanıtlarla yeniden kurulan zincir**
- **Son adımı veya etkisi doğrulanmamış zincir**

Yalnız ilk türe “tam gözlenmiş” derdim. İkincisi de faydalıdır; doğru etiketle sunulmalı. Tek güçlü zincir çıkarsa, sırf bölüm dolsun diye üç–dört senaryo zorlamam.

**§8’deki 14 karar için benim tercihlerim**

| Karar | Benim değerlendirmem |
|---|---|
| **D1 — Bölüm sayısı** | 14 başlık şimdilik korunabilir. Bölüm 14 koşullu; bölüm sayısı kalite ölçütü değil. |
| **D2 — Ürün sütunları** | Beş canlı/dört kaynak erişim ayrımı doğru. Her tabloda beş zorunlu sütun yerine konuya göre sütun seçimi. |
| **D3 — Altı etki** | Boyutları koru; rapor boyutunu geri getir, “uygulanmaz”ı ayır. Altı satırın varlığı tek başına yeterlilik sayılmasın. |
| **D4 — Kanıt gösterimi** | İstisnada rozet uygun; fakat her iddiadan dayanağına kısa dipnot bağlantısı olmalı. |
| **D5 — Ürün anlatımı** | Dokuz ürün, kanıt derinliğine göre farklı uzunluk. Şema, gözlenen işleyişi anlatmalı. |
| **D6 — Senaryolar** | Tarama yapılsın; sayı hedeflenmesin. Koşum sürekliliği kontrol edilsin. |
| **D7 — Rapor etkisi** | Hem olay içinde hem rapor bölümünde olsun; birinde sonuç, diğerinde ölçünün tanımı anlatılsın. |
| **D8 — Kaynak ürünler** | İlgili olay yanında yer alsın. Her sayfada aynı kaynak metni tekrarlanmasın. |
| **D9 — Kare bütçesi** | Yaklaşık 15 başlangıç tahmini olabilir; alt hesap düzeltilmeli. Kareyi iddianın ihtiyacı belirlesin. |
| **D10 — Yeni koşum** | İlk iki maddeyi otomatik yeterli saymam. Önce hangi iddiaların mevcut kanıtla kapanmadığı çıkarılmalı. |
| **D11 — Ölçek** | 40–46 yeniden hesaplanmalı. Gerekli uzunluk kabul edilebilir; tekrar için gerekçe olamaz. |
| **D12 — Belge 1 tekrarı** | Kısa bağlam + gönderme. Alan adını tekrar etmeyi tümden yasaklamak bağımsız okumayı zorlaştırır. |
| **D13 — Sözlük** | Toplu sözlük + gerektiğinde yerel açıklama. Terimler arasındaki anlam farkı korunmalı. |
| **D14 — “Olay” sözcüğü** | Kullanılabilir; araştırmacının karşılaştırma birimi olduğu tanımlanmalı. Ek dosya/bütçe ayarı gibi her eyleme “ekonomik olay” denmemeli. |

Bu kararlara üç tane daha eklerdim: **olayların karşılaştırılabilirlik koşulları**, **farklı koşumların birleştirilme kuralı**, **ana iddiası kanıtsız kalan bölümün ne yapılacağı**. Bunlar sayfa sayısından daha belirleyici.

**Ben olsam çalışmayı nasıl yürütürdüm?**

1. Önce yukarıdaki somut çelişkileri ve aşırı genellemeleri planda düzeltirdim.
2. Her ana iddiayı kanıt, koşum, tarih, filtre ve sınırıyla eşlerdim. Kullanıcının çıkardığı A2 sayfasını geri getirmeden, mevcut denetim dosyalarında tutardım.
3. Mevcut zincirleri tarar, yeni kanıt gerektiren iddiaları belirlerdim.
4. Kullanıcının koşacağı kısa test listesini bu iddialara bağlardım.
5. Üç farklı örnek sayfada anlatım biçimini sınardım: transfer, kart taksiti ve plan gerçekleşmesi.
6. Bölümleri yazdıktan sonra ürün şemalarını bunlardan türetirdim; şemalarda yeni ve kanıtsız sonuç üretmezdim.
7. Son kontrolde yalnız kaynak dosyasının varlığını değil, **iddianın gerçekten o kaynaktan çıkıp çıkmadığını** denetlerdim.

**Planın esas ihtiyacı daha fazla bölüm değil; mevcut kanıttan çıkarılan sonuçların daha sıkı kurulması.** Bu düzeltmelerle Belge 2, uzun bir özellik listesinden çok daha değerli, Belge 3’te karar verirken güvenilebilecek bir başvuru belgesi olur.

Dosya değiştirmedim; kod/build/test çalıştırmadım. Sıradaki tek iş, bu bulguları işleyen **Belge 2 planı v2 revizyonu** olmalı.

---

## Sonraki kullanıcı kararı — konu içinde A → C, sonra sonraki konu

20 Eylül 2026 · Önceki değerlendirmeden sonra verilen kullanıcı düzeltmesi.

**Mevcut v1 planın düzeni:** Önce bütün konuların A bölümleri (Kısım I, Bölüm 3–11), ardından ürünlerin toplu C anlatımı (Kısım II, Bölüm 12), sonra bağlar ve varsa senaryolar. K2, Ö2 ve §2 bu toplu düzeni tarif ediyor.

**Kullanıcı bu düzeni istemiyor.** İstenen okuma ve bölüm bölüm ilerleme sırası:

1. Bir konunun A kısmı: karşılaştırmalı tablo veya tablolar, olaylar ve gözlenebilir etkiler.
2. Hemen ardından aynı konunun C kısmı: ilgili ürünlerin o konudaki işleyişi, akışları ve kanıtlı davranış şemaları.
3. O konu tamamlandıktan sonra sıradaki konunun A kısmı ve ardından onun C kısmı.

Örnek: **Hesap/transfer A → hesap/transfer C → kart A → kart C → borç/cari A → borç/cari C.** Bu örnek kesin konu sırasını kararlaştırmaz; konu içindeki A/C yerleşimini gösterir.

Belge 1'deki gibi konu konu ilerlenir; bütün A'lar veya bütün ürün motorları bir defada topluca sunulmaz. C içeriği korunur, ancak her konunun hemen arkasında o konuyla sınırlı olarak anlatılır.

**Claude Code için revizyon girdisi:** K2, Ö2, §2'deki iskelet ve yazım sırası, ayrı Kısım II/Bölüm 12, bölüm sınırları, D5 ve ilgili sayfa/kare tahminleri bu kullanıcı kararına göre yeniden düzenlenmeli. Önceki incelemedeki toplu ürün anlatımını koruyan öneriler bu kararla geçersiz kalır; kanıt, tarafsızlık, sayı ve yöntem bulguları geçerliliğini korur. Bu karar diğer inceleme önerilerinin topluca onaylandığı anlamına gelmez.

Bu dosya inceleme ve sonraki kullanıcı kararının kaydıdır; mevcut bölüm planı bu kayıt sırasında değiştirilmemiştir.


---

## Kurgu tercihinin gerekçesi — konu içinde A → C neden daha uygun?

**Bu belgenin amacı için senin önerdiğin, konu içinde A → C şeklinde ilerlemek daha mantıklı.** Çünkü okur bir konunun karşılaştırmasını gördüğünde, hemen ardından “Bu fark ürünlerde nasıl oluşuyor?” sorusunun cevabını alıyor. Önceki değerlendirmemde tüm A’ların ardından toplu C’yi koruma önerimi bu nedenle değiştiriyorum.

İki yaklaşımın güçlü tarafları farklı:

| Yaklaşım | Güçlü yanı | Zayıf yanı |
|---|---|---|
| Bütün A’lar → bütün C’ler | Önce bütün karşılaştırmaları, sonra her ürünün bütünsel modelini sunar. | Okur bir tablonun açıklamasını bulmak için ilerideki bölüme gider; önceki tutarları, koşulları ve soruları hatırlamak zorunda kalır. |
| Her konuda A → ilgili C | Karşılaştırma ve açıklama yan yana kalır; konu tamamlanarak ilerlenir. | Bir ürünün farklı konulardaki işleyişi farklı bölümlere dağılır; ortak mekanizmaların tekrar edilmesine dikkat etmek gerekir. |

**Belge 2’nin ana ekseni konular olduğu için ikinci yaklaşımın faydası daha büyük.** Kart ödemesini anlamaya çalışan okurun, açıklamaya ulaşmak için transfer, sınıflandırma ve veri aktarımı bölümlerini geçmesi gerekmemeli.

Örneğin kart konusunda şöyle ilerlemek daha anlaşılır:

1. **Kısa giriş:** Hangi soruyu inceliyoruz, hangi koşumlara dayanıyoruz?
2. **A — Karşılaştırma:** Harcama, taksit ve ödeme hesapları, borcu ve raporları nasıl etkiliyor?
3. **C — İşleyiş:** Money Manager bu sonucu hangi dönem ayrımıyla gösteriyor? Bluecoins’te hangi kayıtlar ve bekleyen örnekler görülüyor? Wallet’ta borç nasıl izleniyor?
4. **Konu kapanışı:** Kanıtlanan farklar, bilinmeyenler ve gerekiyorsa ilgili kısa senaryo.
5. **Sonraki konu.**

Burada önemli nokta, **C’nin A’daki tabloyu cümlelere çevirerek tekrarlamaması.** A, sonuçları karşılaştırır; C, bu sonuçları anlamak için gereken işlem zincirini ve ürünün kendine özgü davranışını açıklar. İkisi birbirini tamamlar.

Ayrıca çok uzun konularda açıklamayı bölümün en sonuna kadar bekletmemek de iyi olur. Örneğin kart bölümünde gerekirse:

**Kart harcaması karşılaştırması → ilgili işleyiş → taksit karşılaştırması → ilgili işleyiş → ödeme karşılaştırması → ilgili işleyiş.**

Bunu her küçük tabloya zorunlu bir kalıp olarak uygulamamak gerekir. Ölçüt şu olmalı: **Okurun anlayabilmek için ihtiyaç duyduğu açıklama, ilgili karşılaştırmadan uzaklaşmasın.**

Ürünlerin bütünsel görünümünü de kaybetmek zorunda değiliz. Bölümler arasında kısa göndermeler kullanılabilir; ihtiyaç kalırsa sonda tekrara düşmeyen kısa bir ürün özeti bulunabilir. Asıl açıklamalar yine ilgili konunun içinde durur.

**Dolayısıyla bütünlüğü sağlayan şey burada bütün tabloları bir araya toplamak değil, her soruyu karşılaştırması, açıklaması ve sınırlarıyla birlikte tamamlamak.** Senin önerdiğin sıra bunu daha iyi sağlıyor.
