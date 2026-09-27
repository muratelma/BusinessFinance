# İddia tablosu — Belge 2 · Bölüm 6 · Sınıflandırma: işletme mi, şahsi mi

Bu tablo elle tutulmaz: `icerik.py` içindeki işaret ve not metinlerinden üretilir.
PDF'te okunan her cümle burada birebir görünür; metin değişirse tablo da değişir.

**Kural.** Kanıt niteliği ürüne değil ifadeye bağlıdır. Bir işaret yalnız bağlı olduğu
karede görüneni anlatır. Cümlenin kapsamı kareden genişse o kapsamı taşıyan kanıt ayrıca
gösterilir; gösterilemiyorsa cümle gözlenen kapsamla sınırlanır.

## 1 · Şekil işaretleri

| Yer | İfade | Dayanak | Kanıt türü |
|---|---|---|---|
| Şekil 6.1 | Tutar, Kategori, Hesap, Not. | E0229 | Canlı kare |
| Şekil 6.1 | On bir kategori: Yiyecek, Eğlence, Taşıma… | E0229 | Canlı kare |
| Şekil 6.1 | Hepsi kişisel gider kategorisi. | E0229 | Canlı kare |
| Şekil 6.2 | Kategori, hesap, Planlı İşlemler, Bölmek. | E0020 | Canlı kare |
| Şekil 6.2 | Durum ve Etiket ayrı alanlar. | E0020 | Canlı kare |
| Şekil 6.2 | Kapsam alanı yok. | E0020 | Canlı kare |
| Şekil 6.3 | Note, Labels, Payee, Date, Time. | E0294 | Canlı kare |
| Şekil 6.3 | Payment Type, Warranty, Status, Place. | E0294 | Canlı kare |
| Şekil 6.3 | Görünen bölümde kapsam alanı yok. | E0294 | Canlı kare |
| Şekil 6.4 | Kategori bir seçici değil, serbest metin. | E0151 | Canlı kare |
| Şekil 6.4 | Kullanıcı ne yazarsa o. | E0151 | Canlı kare |
| Şekil 6.4 | İki değerli bir ayrım alanı yok. | E0151 | Canlı kare |
| Şekil 6.5 | Rapor ekranında Categories / Labels geçişi. | E0425 | Canlı kare |
| Şekil 6.5 | Labels seçili: bu ay ₺150. | E0425 | Canlı kare |
| Şekil 6.5 | Etiketsiz kayıtlar bu görünümde yok. | E0425 | Canlı kare |
| Şekil 6.6 | Etiketlerin kendi yönetim ekranı var. | E0088 | Canlı kare |
| Şekil 6.6 | Arama ve silme eylemleri burada. | E0088 | Canlı kare |
| Şekil 6.6 | Listede İş ve Kişisel de var. | E0088 | Canlı kare |
| Şekil 6.7 | Üst başlıklar: Araba, Eve Ait, Eğlence. | E0087 | Canlı kare |
| Şekil 6.7 | Altlarında alt kategoriler. | E0087 | Canlı kare |
| Şekil 6.7 | Üst katman kapsam gibi kullanılabilir. | E0087 | Canlı kare |
| Şekil 6.8 | Para amacına göre bölünmüş. | E0417 | Canlı kare |
| Şekil 6.8 | Zarf adı kaydın ne için olduğunu söylüyor. | E0417 | Canlı kare |
| Şekil 6.8 | Kimin parası olduğunu söylemiyor. | E0417 | Canlı kare |
| Şekil 6.9 | Alındı/Ödendi yerine Tahsilat/FaturaOdemesi. | E0161 | Canlı kare |
| Şekil 6.9 | Bütün ekran başlıkları birlikte değişti. | E0161 | Canlı kare |
| Şekil 6.9 | Ayrım değil, dil değişikliği. | E0161 | Canlı kare |
| Şekil 6.10 | Kolonlar: kod, ad, etiket, para birimi, durum, tarih. | E0188 | Kaynak görseli |
| Şekil 6.10 | Ve üç toplam: gelir, gider, net. | E0188 | Kaynak görseli |
| Şekil 6.10 | Proje adları kullanıcının koyduğu adlar. | E0188 | Kaynak görseli |
| Şekil 6.11 | Kâr/Zarar ve Nakit Durumu iki ayrı sekme. | E0190 | Kaynak görseli |
| Şekil 6.11 | Toplam, tahsil edilen, ödenen, bekleyen kırılımı. | E0190 | Kaynak görseli |
| Şekil 6.11 | Aktif/Pasif menüsü ve ayrıca silme var. | E0190 | Kaynak görseli |
| 6.4 · Alan: Type | İşlem başına tek bir alan: Type sütunu Business veya Personal değerini alıyor. Üçüncü bir değer yok. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Alan: Type | İşlem listesi bu başlıktan süzülebiliyor; şahsi işaretlenen kayıt işletme raporuna girmiyor ama silinmiyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Kısmen işletme: Split | Bir gider kısmen işletme kısmen şahsiyse kayıt bölünüyor: Edit → Split transaction → tutar parçalara ayrılıyor ve her parça ayrı ayrı işaretleniyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Kısmen işletme: Split | Parçaların toplamı özgün tutara eşitleniyor; yani kapsam kayıt düzeyinde değil, kalem düzeyinde tanımlanabiliyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Kısmen işletme: Split | Kaynak bir istisna da yazıyor: araç ve yakıt gideri bölünmüyor, tamamı işletme işaretleniyor ve oranı muhasebeci hesaplıyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Silme yerine hariç tutma | Yinelenen veya ilgisiz kayıt silinmiyor, Exclude ile hariç tutuluyor. Kayıt veride kalıyor, toplamlardan çıkıyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Silme yerine hariç tutma | Bu, şahsi işaretlemenin de mantığı: kayıt duruyor, yalnız hangi toplama gireceği değişiyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Sınıflamayı kim yapıyor | Kayıtlar bağlı banka ve kart hesaplarından otomatik iniyor; ürün geçmişe ve başka kullanıcıların davranışına bakarak kategori ve tür öneriyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Sınıflamayı kim yapıyor | Kullanıcının işi kaydı oluşturmak değil, inen kaydı gözden geçirip düzeltmek. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Sınıflamayı kim yapıyor | Tekrar eden düzeltmeler bir kural motoruna dönüşüyor: en çok otuz kural, benzer işlemleri otomatik sınıflıyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Kategoriler nereye bağlı | Kategoriler ülkeye özgü bir vergi formunun kalemleriyle hizalı; kategorize edilen her işlem o formda bir satıra eşleniyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Kategoriler nereye bağlı | Yani sınıflandırmanın amacı raporlama değil, yıl sonunda üretilecek belge. Ayrımın keskinliği ve üçüncü değerin olmaması buradan geliyor. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Bu sayfanın sınırı | Ürünün iç arayüzü hiç görülmedi: elde yalnız açılış ekranları ve ödeme duvarı var. Yukarıdaki her cümle ürünün kendi anlatımıdır. | Intuit yardım merkezi | Kaynak beyanı |
| 6.4 · Bu sayfanın sınırı | Alanın gerçekte nasıl davrandığı, şahsi kaydın hangi toplamlardan çıktığı ve bölmenin sonucunun raporda nasıl göründüğü ölçülmedi. | Intuit yardım merkezi | Kaynak beyanı |
| 6.5 · Bluecoins · Ayrımın sonucu | Üst kategori kapsam gibi kullanılırsa kırılım kayboluyor | E0087 | Çıkarım |
| 6.5 · Goodbudget · Ayrımın sonucu | Zarf ne için olduğunu söylüyor, kimin parası olduğunu değil | E0417 | Çıkarım |
| 6.5 · KolayBi · Kapsam alanı | Proje ekseni | E0188 | Kaynak görseli |
| 6.5 · KolayBi · En yakın araç | Kullanıcının tanımladığı proje; gelir/gider/net proje başına | E0188 | Kaynak görseli |
| 6.5 · KolayBi · Ayrımın sonucu | Eksenin anlamı kullanıcının disiplinine bağlı | E0188 | Çıkarım |
| 6.5 · QuickBooks Solopreneur · Kapsam alanı | Var — Type: Business / Personal | Intuit yardım merkezi | Kaynak beyanı |
| 6.5 · QuickBooks Solopreneur · En yakın araç | Kayıt başına iki değerli alan; kısmen işletme için Split | Intuit yardım merkezi | Kaynak beyanı |
| 6.5 · QuickBooks Solopreneur · Ayrımın sonucu | Şahsi kayıt silinmiyor, toplamdan çıkıyor | Intuit yardım merkezi | Kaynak beyanı |
| 6.5 · Paraşüt · Kapsam alanı | Yok | parasut.com kılavuzu | Kaynak beyanı |
| 6.5 · Paraşüt · En yakın araç | Ortak/personel carisi üzerinden dolaylı | parasut.com kılavuzu | Kaynak beyanı |
| 6.5 · Paraşüt · Ayrımın sonucu | Şahsi harcama bir alacak-borç kalemine dönüşüyor | parasut.com kılavuzu | Çıkarım |
| 6.5 · Logo İşbaşı · Kapsam alanı | Yok | isbasi.com | Kaynak beyanı |
| 6.5 · Logo İşbaşı · En yakın araç | Ortak carisi veya çekilen para | isbasi.com | Kaynak beyanı |
| 6.5 · Logo İşbaşı · Ayrımın sonucu | Klasik firma defteri; şahsi taraf ürünün dışında | isbasi.com | Çıkarım |
| 6.5 · Money Manager · Kapsam alanı | Yok | E0229 | Canlı kare |
| 6.5 · Money Manager · En yakın araç | Kategori; on bir kutunun hepsi kişisel | E0229 | Canlı kare |
| 6.5 · Money Manager · Ayrımın sonucu | Kapsam raporda hiç görünmüyor | E0231 | Canlı kare |
| 6.5 · Bluecoins · Kapsam alanı | Yok | E0020 | Canlı kare |
| 6.5 · Bluecoins · En yakın araç | Etiket (listede İş ve Kişisel) ve iki katmanlı kategori ağacı | E0087 · E0088 | Canlı kare |
| 6.5 · Wallet · Kapsam alanı | Yok | E0294 | Canlı kare |
| 6.5 · Wallet · En yakın araç | Labels — kategoriden bağımsız ikinci eksen | E0278 | Canlı kare |
| 6.5 · Wallet · Ayrımın sonucu | Rapor Categories/Labels olarak ayrılıyor; Labels yalnız etiketli parayı sayıyor | E0278 · E0425 | Canlı kare |
| 6.5 · Hesap Defterim · Kapsam alanı | Yok | E0151 | Canlı kare |
| 6.5 · Hesap Defterim · En yakın araç | Serbest metin kategori; düğme adlarını değiştirme | E0161 | Canlı kare |
| 6.5 · Hesap Defterim · Ayrımın sonucu | Raporda kategori kırılımı da yok | E0142 | Canlı kare |
| 6.5 · Goodbudget · Kapsam alanı | Yok | E0417 | Canlı kare |
| 6.5 · Goodbudget · En yakın araç | Zarf — amaç ekseni | E0417 | Canlı kare |
| 6.6 · adım 2 · Canlı beş ürün | Hiçbir yerde. Kayıt formunda, kayıt ayrıntısında ve kategori yönetiminde böyle bir alan bulunamadı. | E0294 | Canlı kare |
| 6.6 · adım 2 · KolayBi | Kullanıcının açtığı bir proje ekseninde. Eksen sabit değil, adları kullanıcı koyuyor. | E0188 | Canlı kare |
| 6.6 · adım 2 · QuickBooks Solopreneur | Kayıt başına sabit bir alanda: Business ya da Personal. Üçüncü değer yok. | Intuit yardım merkezi | Canlı kare |
| 6.6 · adım 3 · Wallet | Kaydı bölmek mümkün (Split record) ama parçalara kapsam verilemiyor; bölme kategori içindir. | E0311 | Canlı kare |
| 6.6 · adım 3 · Bluecoins | Her parça kendi etiketini taşıyabiliyor; İş ve Kişisel etiketleriyle gider parça parça işaretlenebilir. Raporun parçaları etikete göre ayırdığı ölçülmedi. | E0443 · E0088 | Canlı kare |
| 6.6 · adım 3 · QuickBooks Solopreneur | Kayıt tutara göre bölünüyor ve her parça ayrı işaretleniyor; kapsam kalem düzeyine iniyor. | Intuit yardım merkezi | Canlı kare |
| 6.6 · adım 4 · Canlı beş ürün | Rapor kapsamdan habersiz. Ayrım ancak etiket veya kategori disipliniyle taklit edilebiliyor; Wallet'ta etiketlenmeyen kayıt etiket raporundan tamamen düşüyor. | E0278 · E0425 | Canlı kare |
| 6.6 · adım 4 · KolayBi | Her proje kendi gelir, gider ve net toplamını taşıyor. | E0188 | Canlı kare |
| 6.6 · adım 4 · QuickBooks Solopreneur | Şahsi işaretlenen kayıt işletme raporundan çıkıyor ama veride kalıyor — silinmiyor, hariç tutuluyor. | Intuit yardım merkezi | Canlı kare |
| 6.7 · Kapsam alanının yokluğu ürünlerin hedef kitlesinden geliyor | Canlı beş ürün kişisel finans uygulaması ve kayıt düzeyinde tek bir cep varsayıyor. Money Manager'ın on bir kategorisinin hepsi kişisel gider başlığı taşıyor — işletme tarafına ayrılmış tek bir grup bile yok. Bluecoins'in etiket listesinde İş ve Kişisel'in bulunması, ayrımın akla geldiğini ama bir alana dönüşmediğini gösteriyor. | E0229, E0020, E0294, E0151, parasut.com kılavuzu, isbasi.com | Çıkarım |
| 6.7 · Yerine kullanılan araçların hepsi başka bir soru için yapılmış | Etiket çok değerli ve serbest; iki değerli bir ayrım için kullanıldığında sonucun doğruluğu tamamen kullanıcının disiplinine kalıyor. Wallet'ta bunun sonucu ölçüldü: etiket raporu yalnız etiketli ₺150'yi saydı, etiketlenmemiş her kayıt o rapordan düştü. Kategori ağacının üst katmanı kapsam gibi kullanılabilir, ama o zaman kategorinin asıl işi — harcamayı türüne göre kırmak — kayboluyor. | E0278, E0425, E0087, E0088, E0417, E0161 | Çıkarım |
| 6.7 · Sabit alan, ayrımın amacından doğuyor | QuickBooks'un alanı iki değerli ve üçüncü değeri yok. Kaynağa göre bunun nedeni kategorilerin bir vergi formunun kalemleriyle hizalı olması: kategorize edilen her işlem o formda bir satıra eşleniyor. | Intuit yardım merkezi, E0188, E0190 | Çıkarım |
| 6.7 · Şahsi kayıt silinmiyor, hariç tutuluyor | Kaynağa göre şahsi işaretlenen kayıt işletme raporuna girmiyor ama veride kalıyor. Aynı mantık ilgisiz kayıtlar için de geçerli: silme değil, hariç tutma. | Intuit yardım merkezi, E0252, E0253, E0437 | Çıkarım |

## 2 · Sentez cümleleri

Tek bir işarete değil, sayfadaki karelerin karşılaştırılmasına dayanır.

| Yer | Cümle | Dayanak |
|---|---|---|
| 6.1 · not | Goodbudget'ta da kayıt formunda böyle bir alan yok; kaydı sınıflayan tek şey hangi zarfa yazıldığı. Zarf ise amaç ekseni, kapsam ekseni değil. | E0020, E0151, E0229, E0294 |
| 6.1 · not | Bu bölümdeki yokluk ifadeleri incelenen sürümler ve taranan yüzeylerle sınırlı: kayıt formu, kayıt ayrıntısı, kategori yönetimi ve ayarlar. Ücretli paketlerde açılan yüzeyler bu taramaya girmedi. | E0020, E0151, E0229, E0294 |
| 6.1 · not | Money Manager'ın kategori paneli on bir kutunun hepsini kişisel gider başlığıyla dolduruyor; işletme tarafına ayrılmış bir grup yok. | E0229 |
| 6.1 · not | Hesap Defterim'de kategori alanı ayarlardan kapatılabiliyor. | E0178 |
| 6.2 · not | Dördü de kısmi çözüm. Etiket serbest ve çok değerli — iki değerli bir ayrım için kullanıldığında kullanıcının disiplinine bağlı. Kategori ağacının üst katmanı kapsam gibi kullanılabilir ama o zaman kategori kırılımı kaybediliyor. | E0087, E0088, E0161, E0417, E0425 |
| 6.2 · not | Disiplinin bedeli Wallet'ta ölçüldü: koşumda iki kayda Isletme ve Sahsi etiketi verildi. Labels görünümü yalnız bu ikisini, ₺150'yi saydı; aynı ayın ₺21.750'lik giderinin etiketlenmemiş kısmı bu görünümde hiç yok. | E0425 · E0439 |
| 6.2 · not | Hesap Defterim'inki bir sınıflandırma aracı değil: düğme ve sütun adlarının kullanıcının diline çevrilmesi. Ayrımı değil, ayrımın adını değiştiriyor. | E0087, E0088, E0161, E0417, E0425 |
| 6.2 · not | Wallet'ın ayarlarında bir kural motoru var: kayıtlara kategori ve etiket atayan Automatic rules. Kapsam için kullanılıp kullanılamayacağı denenmedi. | E0378 |
| 6.2 · not | Bir başka dolaylı yol hesabı ayırmak: işletme parası için ayrı hesap açıp kapsamı hesap düzeyine taşımak. Bu koşumda denenmedi. | E0027 |
| 6.2 · not | Bluecoins'in etiket listesi, araştırmada hiç etiket eklenmemişken de İş ve Kişisel değerlerini taşıyordu. Etiket çoklu seçiliyor, filtre panelinde ayrı bir boyut ve bölünmüş kaydın her parçası kendi etiketini alabiliyor. | E0088 · E0426 · E0443 |
| 6.3 · not | Eksen kullanıcı tanımlı olduğu için "işletme" ve "şahsi" iki proje olarak açılabilir — ama ürün bunu önermiyor; proje bir iş kalemi olarak kurgulanmış ve listedeki adlar da öyle. | E0188, E0190 |
| 6.3 · not | Kullanıcı tanımlı eksenin bedeli: iki proje mi, yirmi proje mi olacağı kullanıcıya kalıyor ve raporun anlamı kullanıcının disiplinine bağlanıyor. | E0188, E0190 |
| 6.3 · not | Projenin belge kırılımı satış, alış, iade, para girişi ve genel gider kartlarına ayrılmış. | E0191 |
| 6.3 · not | Yeni proje formunda kod, ad ve para birimi zorunlu; başlangıç-bitiş tarihi var. | E0189 |
| 6.4 · not | Bu sayfa bölümün tek karesiz sayfası ve bilinçli olarak öyle: basılacak bir ekran yok. Kaynak metninin kendisi bulgu, çünkü canlı beş üründe karşılığı hiç çıkmadı. | — |
| 6.5 · not | Dokuz üründe kayıt düzeyinde kapsam alanı yalnız birinde var ve o ürünün hiçbir iç ekranı görülmedi. Canlı koşulan beş üründe sıfır. | — |
| 6.5 · not | Üç Türk ön muhasebe ürününde de alan yok; ikisinde şahsi harcamanın dolaylı yolu ortak veya personel carisinden geçiyor — yani şahsi harcama bir borç kalemine dönüşüyor. | — |
| 6.6 · adım 1 | Dokuz üründe de kayıt tutar, tarih ve bir sınıflandırma taşıyor. Ayrışma o sınıflandırmanın hangi soruyu cevapladığında. | — |
| 6.6 · not | Yolun ilk adımı ortak, ikincisinden sonrası tamamen ayrışıyor. Alanı olmayan beş üründe sonraki iki adım da boş kalıyor — kavram olmayınca ne bölme ne raporlama sorusu oluşuyor. | — |
| 6.6 · not | Bu akışın iki dalı kaynak metnine dayanıyor ve ölçülmüş davranış değil. | — |
| 6.7 · Wallet · kazanç | Labels kategoriden bağımsız ikinci bir eksen ve rapor bu eksende de okunabiliyor | — |
| 6.7 · Wallet · kayıp | Etiket serbest ve çok değerli; etiketlenmeyen kayıt etiket raporundan tamamen düşüyor | — |
| 6.7 · Bluecoins · kazanç | Kategori iki katmanlı; etiket listesinde İş ve Kişisel var ve bölünmüş kaydın her parçası kendi etiketini alabiliyor | — |
| 6.7 · Bluecoins · kayıp | Üst katman kapsama ayrılırsa kategorinin asıl kırılımı kayboluyor | — |
| 6.7 · Goodbudget · kazanç | Zarf kaydın amacını net söylüyor ve para zaten o amaca ayrılmış durumda | — |
| 6.7 · Goodbudget · kayıp | Amaç ekseni sahibi söylemiyor; aynı zarf iki cebe de ait olabilir | — |
| 6.7 · Money Manager · Hesap Defterim · kazanç | — | — |
| 6.7 · Money Manager · Hesap Defterim · kayıp | Kapsam için kullanılabilecek ikinci bir eksen yok; biri yalnız kategori, diğeri yalnız serbest metin sunuyor | — |
| 6.7 · KolayBi · kazanç | Eksen kullanıcı tanımlı; her proje kendi gelir, gider ve net toplamını taşıyor | — |
| 6.7 · KolayBi · kayıp | Esnekliğin bedeli belirsizlik: raporun anlamı kullanıcının eksen disiplinine bağlı | — |
| 6.7 · QuickBooks Solopreneur · kazanç | Kayıt başına sabit, iki değerli bir alan; kısmen işletme olan gider bölünebiliyor ve şahsi kayıt silinmeden toplamdan çıkıyor | — |
| 6.7 · QuickBooks Solopreneur · kayıp | Ayrım ülkeye özgü bir vergi formuna bağlı; alan iki değerli olduğu için kısmi durum ancak kaydı bölerek çözülüyor | — |
| 6.7 · Belge 3 sorusu | Kapsam kayıt düzeyinde sabit bir alan mı olmalı, yoksa kullanıcının tanımladığı bir eksen mi? İki kaynak iki ayrı cevap veriyor ve ikisinin bedeli farklı. | — |
| 6.7 · Belge 3 sorusu | Kısmen işletme olan bir gider ne olmalı — bölünmeli mi, tek kapsamda mı kalmalı? Kaynakta Split kapsamı kalem düzeyine indiriyor; canlı tarafta Bluecoins'in parça başına etiketi aynı işe yarayabilir ama sonucu ölçülmedi. | — |
| 6.7 · Belge 3 sorusu | Şahsi işaretlenen kayıt silinmeli mi, toplamdan mı çıkmalı? Kaynaktaki model kaydı tutup filtreliyor. | — |
| 6.7 · Belge 3 sorusu | İkinci eksen kategoriden ayrı mı olmalı? Canlı beş üründe kategoriden bağımsız ikinci eksen yalnız ikisinde var ve ikisi de etiket. | — |

## 3 · Çıkarılmayan sonuçlar

Bu bölüm aşağıdaki cümlelerin hiçbirini kurmaz.

| Yer | Kurulmayan cümle |
|---|---|
