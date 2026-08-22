# ADR 0013 — İşletme ve şahsi, tek havuzda bir boyuttur

- Durum: Kabul edildi (22 Ağustos 2026, Aşama 01 öncesi)
- Bağlam: Ürün yönünün kişisel bütçeden şahıs şirketi/esnaf finansına çevrilmesi
- İlgili: ADR 0002 (transferin gelir/gider olmaması), ADR 0003 (kart ödemesinin
  ikinci kez gider sayılmaması), ADR 0012 (tek InitialCreate)

## Bağlam

Ürün şahıs şirketi sahibine ve esnafa çalışıyor. Bu kitlenin tanımlayıcı
özelliği, uygulamanın bugün hiç bilmediği şey: **işletme gideri ile şahsi
gider aynı cepten çıkar.**

Hukuki durum belirleyici. Şahıs şirketinin tüzel kişiliği yoktur; işletmenin
malvarlığı ile sahibinin malvarlığı **aynı malvarlığıdır**. Kasadan alıp
markete harcamak bir "şirketten para çekme" işlemi değildir: vergi doğurmaz,
gelir/gider olayı değildir, muhasebeci onu görmez. LTD/AŞ'de durum tersidir —
iki ayrı tüzel kişi vardır ve arada gerçek bir ortak cari hesabı bulunur.

Kullanıcının uygulamaya sorduğu her şey üç soruya iniyor ve üçü aynı cevabı
istemiyor:

| Soru | Ayrım gerekli mi |
|---|---|
| İşletmem bu ay ne kazandı? | **Zorunlu** — şahsi harcama karışırsa sayı yalan olur |
| Muhasebeciye ne gidecek? | **Zorunlu** — yalnız işletme tarafı gider |
| Kasada ne kaldı? | **Kesinlikle hayır** — tek cep, tek bakiye |

Buradan tek bir kısıt çıkıyor ve bu kararın tamamı onun üstüne kurulu:

> Bakiye, kasa ve net varlık asla bölünmez. Gelir/gider raporu her zaman
> bölünebilir olmalıdır.

## Karar

**İşletme/şahsi ayrımı tek havuz üzerinde bir boyuttur; ayrı bir veri alanı,
ayrı bir mod veya ayrı bir uygulama değildir.**

### 1. Veri: tek havuz, her kayıtta kapsam

- Her finansal harekette `scope` bulunur: `İşletme` veya `Şahsi`.
- Hesap havuzu tektir. Kasa, banka, kart borcu ve net varlık **kapsamdan
  bağımsızdır** ve hiçbir görünümde bölünmez.
- Kasadan yapılan şahsi harcama **tek kayıttır**: `scope=Şahsi` gider. Transfer
  değildir. İşletme netine girmez, muhasebeci paketine girmez, kasadan düşer.

Son madde bu kararın çekirdeği. Aynı eylemi "işletmeden şahsa transfer + şahsi
gider" diye iki kayda bölmek, şahıs şirketini LTD gibi modellemek — olmayan bir
ortak cari hesabı uydurmak — olurdu. Repo bu hatanın bedelini bir kez ödedi:
kart ödemesinin ikinci kez gider sayılmaması kuralı (ADR 0003) tam olarak
aynı çifte sayım probleminin çözümü.

### 2. Kapsam nereden gelir

Öncelik sırası, en özelden en genele:

1. Kullanıcının formdaki açık seçimi
2. Hesabın veya kartın kapsam etiketi
3. Kategorinin varsayılan kapsamı

Hesap ve kart **isteğe bağlı** bir kapsam etiketi taşır: esnafın çoğunda dükkân
kasası ile kişisel cüzdan fiziksel olarak ayrıdır ve ayrılmışsa bu, kategoriden
daha güçlü bir sinyaldir — ayrı hesap açmak bilinçli bir ayrımdır. Alan boş
bırakılabilir; tek hesabıyla her şeyi yöneten esnaf için kapsam kategoriden
türer.

Etiket bir **ipucudur, sınır değil**. "Dükkân Kasası"ndan şahsi harcama
yapılabilir ve tek dokunuşla düzeltilir.

### 3. Görünüm: tek global kapsam anahtarı

- `Hepsi · İşletme · Şahsi`, tek yerde, uygulama genelinde geçerli ve
  hatırlanır.
- Bölünen ekranlar: gelir/gider raporu, kategori dağılımı, bütçeler, işletme
  neti, muhasebeci paketi.
- Bölünmeyen ekranlar (hesaplar, kartlar, net varlık) anahtar hangi konumdaysa
  olsun toplamı gösterir ve **bunu yazar**; sessizce aynı kalmaz.

### 4. Mod yerine onboarding ön ayarı

Kayıt sırasında tek soru sorulur: *işletmeniz var mı?* Cevap **hiçbir özelliği
kapatmaz.** Yalnız iki şeyi belirler: hangi varsayılan kategori setinin
kurulacağı ve kapsam anahtarının başlangıç konumu. Sonradan değiştirilebilir ve
veri kaybettirmez.

"Yok" diyen kullanıcıda kapsam boyutu tamamen gizlenir; uygulama bugünkü
hâline benzer ve bütün kayıtlar sessizce şahsi olur.

### 5. Kapsam, indirilebilirlik değildir

`scope` "bu para dükkân için mi harcandı" sorusunu cevaplar. "Muhasebeci bunu
indirebilir mi" **ayrı bir sorudur** ve bu alanla temsil edilmez. İkisi çoğu
zaman örtüşür ama ayrıldıkları yerler var: binek otomobil gideri işletmenindir
ama tamamı indirilemez; trafik cezası işletme aracına kesilir ve hiç
indirilemez; fişsiz alım dükkân içindir ve indirilemez.

Tek alanda birleştirilseydi, kullanıcı kutuyu işaretlerken hangi soruyu
cevapladığını bilmezdi ve vergi kapsamı geldiğinde girilmiş her kaydın yeniden
yorumlanması gerekirdi. İndirilebilirlik bayrağı vergi aşamasında, temiz kapsam
verisinin **üstüne** eklenir.

### 6. "Kâr" denmez

Hesaplanan şey nakit esaslı **işletme netidir**: işletme geliri eksi işletme
gideri. Muhasebe kârı satılan malın maliyetini ister, o da stok demektir ve
ürün sınırının dışındadır. Ekranda "kâr" yazmak, kullanıcıyı vergi beyanında
yanıltır.

## Reddedilen seçenekler

### Girişte mod seçimi — "kişisel" / "işletme"

Reddedildi, çünkü **hedef kullanıcı ikisi birdendir.** Şahıs şirketi sahibine
"işletme misin, birey misin?" diye sormak, tam da bu ürünün müşterisine yanlış
bir seçim dayatmaktır. "İşletme" seçerse market alışverişi aynı kasadan çıkmaya
devam eder ve kayıt dışı kalır; "şahsi" seçerse zaten müşterimiz değildir.
Pratikte herkes "işletme" seçer ve kimsenin kullanmadığı ikinci bir ürünün
bakımı ve testi sırtta kalır. Kişisel kullanıcıya hafif bir uygulama sunma
faydası, madde 4'teki ön ayarla veri bölmeden zaten elde ediliyor.

### İki ayrı veri alanı (işletme alanı / şahsi alan)

En tehlikeli seçenek. Kasadaki nakit hangi alanda duruyor? İşletme alanındaysa,
markete gitmek "alanlar arası transfer + şahsi gider" olur: tek eylem için iki
kayıt. Kullanıcı bunu girmez, bir süre sonra hiç etiketlemez ve veri
değersizleşir. Ayrıca bir kez bölünen havuz sonradan birleştirilemez — geri
dönüşü olmayan taraf budur.

### Sekme başına bağımsız kapsam filtresi

İşlemler'de "İşletme", Bütçeler'de "Hepsi" seçili kalır ve kullanıcı aynı anda
iki farklı gerçeğe bakar. Ayrıca bölünemeyen ekranlarla (hesaplar) bölünenlerin
karışması yanlış zihinsel model öğretir: kullanıcı bakiyenin de bölündüğünü
sanır.

### Karşı tarafı ve kapsamı kategoriyle temsil etmek

"İşletme Giderleri" diye bir kategori açmak ilk bakışta bedavadır. Ama kategori
bir raporlama kovasıdır, kimlik taşımaz — repo bu dersi zaten aldı (`title =
açıklama ?? kategori adı`). Kapsam kategoriyle temsil edilseydi her kategorinin
iki kopyası gerekir ve kategori dağılımı raporu ikiye bölünmüş hâlde okunamaz
hale gelirdi.

## Sonuçlar

- Üç soru da doğru cevaplanır: işletme neti şahsi harcamadan etkilenmez, kasa
  bakiyesi her kapsamda aynı toplamı gösterir, muhasebeciye yalnız işletme
  tarafı gider.
- Kullanıcıya çift kayıt yükü binmez; hukuki gerçekle model birebir örtüşür.
- Etiketin yakalanması gerekir. Varsayılan zinciri (hesap → kategori) yaygın
  durumda bunu sıfır dokunuşa indirir, ama **yanlış varsayılan sessizce yanlış
  etiketler**: bu yüzden kapsam onay ekranında görünür durur.
- Geçmiş kayıt sorunu yok: veri sıfırlanıyor ve `scope` baştan zorunlu alan
  oluyor. Belirsiz üçüncü durum, backfill ve toplu atama ekranı gerekmiyor.
  Yedek şeması **v6** yazar ve yalnız v6 okur; v2–v5 yedeklerinde kapsam alanı
  yoktur ve onlara bir değer uydurmak, olmamış bir geçmiş uydurmak olurdu.
- Özet ekranının hero metriği yeniden tasarlanmak zorunda: kapsam varken "bu
  ayın neti" tek başına hangi neti sorduğunu söylemiyor.
- LTD/AŞ desteği bu kararla kapanmıyor. `scope` bugün tek havuzdaki bir
  etiket; o gün gelirse aynı alan bir sınıra dönüşebilir. Sınırı bugünden
  kurmak, hiçbir müşterinin ihtiyacı olmayan karmaşıklığı bugünden ödemek
  olurdu.
