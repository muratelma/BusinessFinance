# Bölüm 6 · Sınıflandırma: işletme mi, şahsi mi

Belge 2 · Rakip finansal akışlar

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Bir kaydın işletmeye mi şahsi hayata mı ait olduğu nerede yazıyor?

Bu bölüm bir yokluğun bölümü. Canlı koşulan beş üründe kayıt düzeyinde işletme/şahsi ayrımı yapan bir alan arandı ve hiçbirinde bulunamadı. Bulunan şey, aynı işi dolaylı yoldan yapmaya çalışan dört ayrı araç.

Yokluğun kendisi de bir bulgu, ama tek başına az şey söyler. Bu yüzden bölüm iki yöne gidiyor: canlı ürünlerde bu işe en yakın duran araçlar, ve kaynaktan okunan iki ayrı çözüm — biri kullanıcının tanımladığı bir eksen, öteki kayıt başına iki değerli bir alan.

**Bu bölüme girmez**

- Kategorinin rapordaki kırılımı → Bölüm 7
- Zarf modelinin para üzerindeki etkisi → Bölüm 1
- Kategori ekranlarının görsel düzeni → Belge 1 §6

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare | Form ve kategori paneli kareli; kapsam alanı bulunamadı. |
| Bluecoins | Canlı kare | Form alanları ve kategori ağacı kareli. |
| Wallet | Canlı kare | Kayıt ayrıntısı ve Labels katmanı kareli. |
| Hesap Defterim | Canlı kare | Serbest metin kategorisi ve yeniden adlandırma kareli. |
| Goodbudget | Canlı kare | Zarf amaç ekseni olarak kareli. |
| KolayBi | Kaynak görseli | Proje ekseni destek görselinde. |
| QuickBooks Solopreneur | Kaynak beyanı | Kayıt başına Business/Personal alanı; yalnız kaynak metni. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |


## 6.1 · Kayıt formunda böyle bir alan var mı

Dört üründe kayıt formunun ve kayıt ayrıntısının tamamı tarandı. Tutar, tarih, kategori, hesap, not, etiket, durum — hepsi var. İşletme ile şahsiyi ayıran bir alan yok.

**Şekil 6.1 · Form ve kategori paneli** (E0229)

- Tutar, Kategori, Hesap, Not.
- On bir kategori: Yiyecek, Eğlence, Taşıma…
- Hepsi kişisel gider kategorisi.

**Şekil 6.2 · Formun bütün alanları** (E0020)

- Kategori, hesap, Planlı İşlemler, Bölmek.
- Durum ve Etiket ayrı alanlar.
- Kapsam alanı yok.

**Şekil 6.3 · Kayıt ayrıntısı** (E0294)

- Note, Labels, Payee, Date, Time.
- Payment Type, Warranty, Status, Place.
- Görünen bölümde kapsam alanı yok.

**Şekil 6.4 · Açıklama / Kategori** (E0151)

- Kategori bir seçici değil, serbest metin.
- Kullanıcı ne yazarsa o.
- İki değerli bir ayrım alanı yok.

- Goodbudget'ta da kayıt formunda böyle bir alan yok; kaydı sınıflayan tek şey hangi zarfa yazıldığı. Zarf ise amaç ekseni, kapsam ekseni değil.
- Bu bölümdeki yokluk ifadeleri incelenen sürümler ve taranan yüzeylerle sınırlı: kayıt formu, kayıt ayrıntısı, kategori yönetimi ve ayarlar. Ücretli paketlerde açılan yüzeyler bu taramaya girmedi.
- Money Manager'ın kategori paneli on bir kutunun hepsini kişisel gider başlığıyla dolduruyor; işletme tarafına ayrılmış bir grup yok.
- Hesap Defterim'de kategori alanı ayarlardan kapatılabiliyor.

## 6.2 · Aynı işe en yakın duran dört araç

Kapsam alanı olmayınca kullanıcı eldekiyle idare ediyor. Dört üründe dört ayrı araç bu işe en yakın duran şey — ama dördü de başka bir soru için yapılmış.

**Şekil 6.5 · Yalnız etiketli para** (E0425)

- Rapor ekranında Categories / Labels geçişi.
- Labels seçili: bu ay ₺150.
- Etiketsiz kayıtlar bu görünümde yok.

**Şekil 6.6 · Etiket kendi listesinde** (E0088)

- Etiketlerin kendi yönetim ekranı var.
- Arama ve silme eylemleri burada.
- Listede İş ve Kişisel de var.

**Şekil 6.7 · İki katmanlı kategori** (E0087)

- Üst başlıklar: Araba, Eve Ait, Eğlence.
- Altlarında alt kategoriler.
- Üst katman kapsam gibi kullanılabilir.

**Şekil 6.8 · Amaç ekseni** (E0417)

- Para amacına göre bölünmüş.
- Zarf adı kaydın ne için olduğunu söylüyor.
- Kimin parası olduğunu söylemiyor.

**Şekil 6.9 · Sütun adları değişti** (E0161)

- Alındı/Ödendi yerine Tahsilat/FaturaOdemesi.
- Bütün ekran başlıkları birlikte değişti.
- Ayrım değil, dil değişikliği.

- Dördü de kısmi çözüm. Etiket serbest ve çok değerli — iki değerli bir ayrım için kullanıldığında kullanıcının disiplinine bağlı. Kategori ağacının üst katmanı kapsam gibi kullanılabilir ama o zaman kategori kırılımı kaybediliyor.
- Disiplinin bedeli Wallet'ta ölçüldü: koşumda iki kayda Isletme ve Sahsi etiketi verildi. Labels görünümü yalnız bu ikisini, ₺150'yi saydı; aynı ayın ₺21.750'lik giderinin etiketlenmemiş kısmı bu görünümde hiç yok.
- Hesap Defterim'inki bir sınıflandırma aracı değil: düğme ve sütun adlarının kullanıcının diline çevrilmesi. Ayrımı değil, ayrımın adını değiştiriyor.
- Wallet'ın ayarlarında bir kural motoru var: kayıtlara kategori ve etiket atayan Automatic rules. Kapsam için kullanılıp kullanılamayacağı denenmedi.
- Bir başka dolaylı yol hesabı ayırmak: işletme parası için ayrı hesap açıp kapsamı hesap düzeyine taşımak. Bu koşumda denenmedi.
- Bluecoins'in etiket listesi, araştırmada hiç etiket eklenmemişken de İş ve Kişisel değerlerini taşıyordu. Etiket çoklu seçiliyor, filtre panelinde ayrı bir boyut ve bölünmüş kaydın her parçası kendi etiketini alabiliyor.

## 6.3 · Kaynakta birinci çözüm: kullanıcının tanımladığı eksen

KolayBi kapsamı sabit bir alan yapmıyor — kullanıcının açtığı bir proje ekseni sunuyor. Her belge bir projeye bağlanıyor ve proje kendi gelir, gider ve net toplamını taşıyor. Alanların varlığı kanıtlı; davranış görülmedi.

**Şekil 6.10 · KolayBi · Proje listesi** (E0188)

- Kolonlar: kod, ad, etiket, para birimi, durum, tarih.
- Ve üç toplam: gelir, gider, net.
- Proje adları kullanıcının koyduğu adlar.

**Şekil 6.11 · KolayBi · Proje özeti** (E0190)

- Kâr/Zarar ve Nakit Durumu iki ayrı sekme.
- Toplam, tahsil edilen, ödenen, bekleyen kırılımı.
- Aktif/Pasif menüsü ve ayrıca silme var.

- Eksen kullanıcı tanımlı olduğu için "işletme" ve "şahsi" iki proje olarak açılabilir — ama ürün bunu önermiyor; proje bir iş kalemi olarak kurgulanmış ve listedeki adlar da öyle.
- Kullanıcı tanımlı eksenin bedeli: iki proje mi, yirmi proje mi olacağı kullanıcıya kalıyor ve raporun anlamı kullanıcının disiplinine bağlanıyor.
- Projenin belge kırılımı satış, alış, iade, para girişi ve genel gider kartlarına ayrılmış.
- Yeni proje formunda kod, ad ve para birimi zorunlu; başlangıç-bitiş tarihi var.

## 6.4 · Kaynakta ikinci çözüm: kayıt başına iki değerli bir alan

QuickBooks Solopreneur, incelenen dokuz ürün içinde kayıt düzeyinde işletme/şahsi ayrımı yapan tek ürün. Ürünün hiçbir iç ekranı görülmedi; aşağıdakilerin tamamı yardım merkezi metninden okunmuştur ve ölçülmüş davranış değildir.

### Alan: Type *(Kaynak beyanı)*

İşlem başına tek bir alan: Type sütunu Business veya Personal değerini alıyor. Üçüncü bir değer yok.

İşlem listesi bu başlıktan süzülebiliyor; şahsi işaretlenen kayıt işletme raporuna girmiyor ama silinmiyor.

### Kısmen işletme: Split *(Kaynak beyanı)*

Bir gider kısmen işletme kısmen şahsiyse kayıt bölünüyor: Edit → Split transaction → tutar parçalara ayrılıyor ve her parça ayrı ayrı işaretleniyor.

Parçaların toplamı özgün tutara eşitleniyor; yani kapsam kayıt düzeyinde değil, kalem düzeyinde tanımlanabiliyor.

Kaynak bir istisna da yazıyor: araç ve yakıt gideri bölünmüyor, tamamı işletme işaretleniyor ve oranı muhasebeci hesaplıyor.

### Silme yerine hariç tutma *(Kaynak beyanı)*

Yinelenen veya ilgisiz kayıt silinmiyor, Exclude ile hariç tutuluyor. Kayıt veride kalıyor, toplamlardan çıkıyor.

Bu, şahsi işaretlemenin de mantığı: kayıt duruyor, yalnız hangi toplama gireceği değişiyor.

### Sınıflamayı kim yapıyor *(Kaynak beyanı)*

Kayıtlar bağlı banka ve kart hesaplarından otomatik iniyor; ürün geçmişe ve başka kullanıcıların davranışına bakarak kategori ve tür öneriyor.

Kullanıcının işi kaydı oluşturmak değil, inen kaydı gözden geçirip düzeltmek.

Tekrar eden düzeltmeler bir kural motoruna dönüşüyor: en çok otuz kural, benzer işlemleri otomatik sınıflıyor.

### Kategoriler nereye bağlı *(Kaynak beyanı)*

Kategoriler ülkeye özgü bir vergi formunun kalemleriyle hizalı; kategorize edilen her işlem o formda bir satıra eşleniyor.

Yani sınıflandırmanın amacı raporlama değil, yıl sonunda üretilecek belge. Ayrımın keskinliği ve üçüncü değerin olmaması buradan geliyor.

### Bu sayfanın sınırı *(Kaynak beyanı)*

Ürünün iç arayüzü hiç görülmedi: elde yalnız açılış ekranları ve ödeme duvarı var. Yukarıdaki her cümle ürünün kendi anlatımıdır.

Alanın gerçekte nasıl davrandığı, şahsi kaydın hangi toplamlardan çıktığı ve bölmenin sonucunun raporda nasıl göründüğü ölçülmedi.

- Bu sayfa bölümün tek karesiz sayfası ve bilinçli olarak öyle: basılacak bir ekran yok. Kaynak metninin kendisi bulgu, çünkü canlı beş üründe karşılığı hiç çıkmadı.

## 6.5 · Dokuz üründe kapsam ayrımı

Aynı soru dokuz ürüne soruldu: bir kaydın işletmeye mi şahsi hayata mı ait olduğu nerede yazıyor?

| Ürün | Kapsam alanı | En yakın araç | Ayrımın sonucu |
|---|---|---|---|
| Money Manager | *Canlı kare* · Yok | *Canlı kare* · Kategori; on bir kutunun hepsi kişisel | *Canlı kare* · Kapsam raporda hiç görünmüyor |
| Bluecoins | *Canlı kare* · Yok | *Canlı kare* · Etiket (listede İş ve Kişisel) ve iki katmanlı kategori ağacı | *Çıkarım* · Üst kategori kapsam gibi kullanılırsa kırılım kayboluyor |
| Wallet | *Canlı kare* · Yok | *Canlı kare* · Labels — kategoriden bağımsız ikinci eksen | *Canlı kare* · Rapor Categories/Labels olarak ayrılıyor; Labels yalnız etiketli parayı sayıyor |
| Hesap Defterim | *Canlı kare* · Yok | *Canlı kare* · Serbest metin kategori; düğme adlarını değiştirme | *Canlı kare* · Raporda kategori kırılımı da yok |
| Goodbudget | *Canlı kare* · Yok | *Canlı kare* · Zarf — amaç ekseni | *Çıkarım* · Zarf ne için olduğunu söylüyor, kimin parası olduğunu değil |
| KolayBi | *Kaynak görseli* · Proje ekseni | *Kaynak görseli* · Kullanıcının tanımladığı proje; gelir/gider/net proje başına | *Çıkarım* · Eksenin anlamı kullanıcının disiplinine bağlı |
| QuickBooks Solopreneur | *Kaynak beyanı* · Var — Type: Business / Personal | *Kaynak beyanı* · Kayıt başına iki değerli alan; kısmen işletme için Split | *Kaynak beyanı* · Şahsi kayıt silinmiyor, toplamdan çıkıyor |
| Paraşüt | *Kaynak beyanı* · Yok | *Kaynak beyanı* · Ortak/personel carisi üzerinden dolaylı | *Çıkarım* · Şahsi harcama bir alacak-borç kalemine dönüşüyor |
| Logo İşbaşı | *Kaynak beyanı* · Yok | *Kaynak beyanı* · Ortak carisi veya çekilen para | *Çıkarım* · Klasik firma defteri; şahsi taraf ürünün dışında |

- Dokuz üründe kayıt düzeyinde kapsam alanı yalnız birinde var ve o ürünün hiçbir iç ekranı görülmedi. Canlı koşulan beş üründe sıfır.
- Üç Türk ön muhasebe ürününde de alan yok; ikisinde şahsi harcamanın dolaylı yolu ortak veya personel carisinden geçiyor — yani şahsi harcama bir borç kalemine dönüşüyor.

## 6.6 · Kapsam sorusunun yolu ve ayrıldığı noktalar

Aynı soru dokuz üründe üç ayrı cevapla karşılaşıyor: alan yok, eksen kullanıcıya bırakılmış, ya da alan sabit ve iki değerli.

**1. Kayıt giriliyor**

Dokuz üründe de kayıt tutar, tarih ve bir sınıflandırma taşıyor. Ayrışma o sınıflandırmanın hangi soruyu cevapladığında.


**2. Kapsam nerede yazılıyor**

**→** *Canlı beş ürün* — Hiçbir yerde. Kayıt formunda, kayıt ayrıntısında ve kategori yönetiminde böyle bir alan bulunamadı. (E0294)
- *KolayBi* — Kullanıcının açtığı bir proje ekseninde. Eksen sabit değil, adları kullanıcı koyuyor. (E0188)
- *QuickBooks Solopreneur* — Kayıt başına sabit bir alanda: Business ya da Personal. Üçüncü değer yok. (Intuit yardım merkezi)

**3. Kısmen işletme olan gider**

- *Wallet* — Kaydı bölmek mümkün (Split record) ama parçalara kapsam verilemiyor; bölme kategori içindir. (E0311)
- *Bluecoins* — Her parça kendi etiketini taşıyabiliyor; İş ve Kişisel etiketleriyle gider parça parça işaretlenebilir. Raporun parçaları etikete göre ayırdığı ölçülmedi. (E0443 · E0088)
**→** *QuickBooks Solopreneur* — Kayıt tutara göre bölünüyor ve her parça ayrı işaretleniyor; kapsam kalem düzeyine iniyor. (Intuit yardım merkezi)

**4. Raporda ne oluyor**

- *Canlı beş ürün* — Rapor kapsamdan habersiz. Ayrım ancak etiket veya kategori disipliniyle taklit edilebiliyor; Wallet'ta etiketlenmeyen kayıt etiket raporundan tamamen düşüyor. (E0278 · E0425)
- *KolayBi* — Her proje kendi gelir, gider ve net toplamını taşıyor. (E0188)
- *QuickBooks Solopreneur* — Şahsi işaretlenen kayıt işletme raporundan çıkıyor ama veride kalıyor — silinmiyor, hariç tutuluyor. (Intuit yardım merkezi)

- Yolun ilk adımı ortak, ikincisinden sonrası tamamen ayrışıyor. Alanı olmayan beş üründe sonraki iki adım da boş kalıyor — kavram olmayınca ne bölme ne raporlama sorusu oluşuyor.
- Bu akışın iki dalı kaynak metnine dayanıyor ve ölçülmüş davranış değil.

## 6.7 · Neden ayrışıyorlar

Bu bölümde ayrışmanın kaynağı ürünlerin kime hizmet ettiği. Kişisel finans defterinde kapsam sorusu hiç oluşmuyor; ön muhasebede şahsi taraf ürünün dışında; vergi ekseninde ise ayrım ürünün asıl işi.

### Kapsam alanının yokluğu ürünlerin hedef kitlesinden geliyor *(Çıkarım)*

Canlı beş ürün kişisel finans uygulaması ve kayıt düzeyinde tek bir cep varsayıyor. Money Manager'ın on bir kategorisinin hepsi kişisel gider başlığı taşıyor — işletme tarafına ayrılmış tek bir grup bile yok. Bluecoins'in etiket listesinde İş ve Kişisel'in bulunması, ayrımın akla geldiğini ama bir alana dönüşmediğini gösteriyor.

Üç Türk ön muhasebe ürününde de alan yok, ama nedeni tersi: onlar firma defteri tutuyor ve şahsi harcama zaten ürünün konusu değil. Kaynakların gösterdiği dolaylı yol, şahsi harcamayı ortak veya personel carisine yazmak — yani onu bir borç kalemine çevirmek.

İki uçta da kapsam sorulmuyor, ama iki ayrı nedenle: birinde ikinci cep yok sayılıyor, ötekinde ikinci cep başka bir kavrama dönüştürülüyor.

*Dayanağı: E0229, E0020, E0294, E0151, parasut.com kılavuzu, isbasi.com*

### Yerine kullanılan araçların hepsi başka bir soru için yapılmış *(Çıkarım)*

Etiket çok değerli ve serbest; iki değerli bir ayrım için kullanıldığında sonucun doğruluğu tamamen kullanıcının disiplinine kalıyor. Wallet'ta bunun sonucu ölçüldü: etiket raporu yalnız etiketli ₺150'yi saydı, etiketlenmemiş her kayıt o rapordan düştü. Kategori ağacının üst katmanı kapsam gibi kullanılabilir, ama o zaman kategorinin asıl işi — harcamayı türüne göre kırmak — kayboluyor.

Zarf amacı söylüyor, sahibi değil: "Market" zarfı hem işletmenin hem evin marketi olabilir. Hesap Defterim'in sunduğu şey ise bir ayrım değil, ayrımın adını değiştirme imkânı.

Ortak nokta şu: kategori, zarf ve adlandırma tek eksenli. Kapsam ikinci bir eksen istiyor ve canlı ürünlerde bu ekseni taşıyan tek araç etiket — Wallet'ta raporun kendi görünümü, Bluecoins'te bir filtre boyutu ve etiket listesinde İş ile Kişisel.

*Dayanağı: E0278, E0425, E0087, E0088, E0417, E0161*

### Sabit alan, ayrımın amacından doğuyor *(Çıkarım)*

QuickBooks'un alanı iki değerli ve üçüncü değeri yok. Kaynağa göre bunun nedeni kategorilerin bir vergi formunun kalemleriyle hizalı olması: kategorize edilen her işlem o formda bir satıra eşleniyor.

Ayrımın amacı raporlama değil, yıl sonunda üretilecek bir belge. Belge "kısmen" kabul etmediği için alan da kabul etmiyor — kısmi durum kaydı bölerek çözülüyor, alana üçüncü bir değer eklenerek değil.

KolayBi'nin çözümü bunun tersi: eksen sabit değil, kullanıcı tanımlı. Esneklik kazanılıyor, ama raporun anlamı kullanıcının kaç proje açtığına ve neyi hangi projeye yazdığına bağlanıyor.

*Dayanağı: Intuit yardım merkezi, E0188, E0190*

### Şahsi kayıt silinmiyor, hariç tutuluyor *(Çıkarım)*

Kaynağa göre şahsi işaretlenen kayıt işletme raporuna girmiyor ama veride kalıyor. Aynı mantık ilgisiz kayıtlar için de geçerli: silme değil, hariç tutma.

Bu, kapsamı bir filtre olarak kurmak demek — kayıt tek, toplam çok. Kaydın kendisiyle o kaydın hangi toplama gireceği birbirinden ayrılıyor.

Canlı ürünlerde bu ayrımın karşılığı hesap düzeyinde duruyor: Money Manager'ın Toplama Dahil Et ve Wallet'ın Exclude from stats anahtarı bütün bir hesabı toplamdan çıkarıyor, hesabı silmeden. Kayıt düzeyinde bir karşılığı görülmedi.

*Dayanağı: Intuit yardım merkezi, E0252, E0253, E0437*

| Ürün | Kazandırdığı | Kaybettirdiği |
|---|---|---|
| **Wallet** | Labels kategoriden bağımsız ikinci bir eksen ve rapor bu eksende de okunabiliyor | Etiket serbest ve çok değerli; etiketlenmeyen kayıt etiket raporundan tamamen düşüyor |
| **Bluecoins** | Kategori iki katmanlı; etiket listesinde İş ve Kişisel var ve bölünmüş kaydın her parçası kendi etiketini alabiliyor | Üst katman kapsama ayrılırsa kategorinin asıl kırılımı kayboluyor |
| **Goodbudget** | Zarf kaydın amacını net söylüyor ve para zaten o amaca ayrılmış durumda | Amaç ekseni sahibi söylemiyor; aynı zarf iki cebe de ait olabilir |
| **Money Manager · Hesap Defterim** | — | Kapsam için kullanılabilecek ikinci bir eksen yok; biri yalnız kategori, diğeri yalnız serbest metin sunuyor |
| **KolayBi** | Eksen kullanıcı tanımlı; her proje kendi gelir, gider ve net toplamını taşıyor | Esnekliğin bedeli belirsizlik: raporun anlamı kullanıcının eksen disiplinine bağlı |
| **QuickBooks Solopreneur** | Kayıt başına sabit, iki değerli bir alan; kısmen işletme olan gider bölünebiliyor ve şahsi kayıt silinmeden toplamdan çıkıyor | Ayrım ülkeye özgü bir vergi formuna bağlı; alan iki değerli olduğu için kısmi durum ancak kaydı bölerek çözülüyor |

**Belge 3'e taşınan soru**

- Kapsam kayıt düzeyinde sabit bir alan mı olmalı, yoksa kullanıcının tanımladığı bir eksen mi? İki kaynak iki ayrı cevap veriyor ve ikisinin bedeli farklı.
- Kısmen işletme olan bir gider ne olmalı — bölünmeli mi, tek kapsamda mı kalmalı? Kaynakta Split kapsamı kalem düzeyine indiriyor; canlı tarafta Bluecoins'in parça başına etiketi aynı işe yarayabilir ama sonucu ölçülmedi.
- Şahsi işaretlenen kayıt silinmeli mi, toplamdan mı çıkmalı? Kaynaktaki model kaydı tutup filtreliyor.
- İkinci eksen kategoriden ayrı mı olmalı? Canlı beş üründe kategoriden bağımsız ikinci eksen yalnız ikisinde var ve ikisi de etiket.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 6.1 | E0229 | Money Manager | Canlı kare | Form ve kategori paneli |
| 6.2 | E0020 | Bluecoins | Canlı kare | Formun bütün alanları |
| 6.3 | E0294 | Wallet | Canlı kare | Kayıt ayrıntısı |
| 6.4 | E0151 | Hesap Defterim | Canlı kare | Açıklama / Kategori |
| 6.5 | E0425 | Wallet | Canlı kare | Yalnız etiketli para |
| 6.6 | E0088 | Bluecoins | Canlı kare | Etiket kendi listesinde |
| 6.7 | E0087 | Bluecoins | Canlı kare | İki katmanlı kategori |
| 6.8 | E0417 | Goodbudget | Canlı kare | Amaç ekseni |
| 6.9 | E0161 | Hesap Defterim | Canlı kare | Sütun adları değişti |
| 6.10 | E0188 | KolayBi | Kaynak görseli | KolayBi · Proje listesi |
| 6.11 | E0190 | KolayBi | Kaynak görseli | KolayBi · Proje özeti |
