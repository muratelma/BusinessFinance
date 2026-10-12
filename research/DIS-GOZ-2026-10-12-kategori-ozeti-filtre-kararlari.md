# Dış göz — Kategori özeti: filtre kararları

**Tarih:** 12 Ekim 2026 · **Dal:** `ui-trials` (`a4df4ed`) · **Kapsam:** Aşama 06.3
Grup 8 §3, `Kategori özeti` sayfasının sekiz kararı.

**Yöntem:** kod ve belgeler okundu; hiçbir şey çalıştırılmadı (test, API, cihaz
yok). Satır numaraları çalışma ağacındaki hâle göredir. Çizimler:
`tasarim-onizleme/liste-sistemi/kategori/tur-4-filtre/01–04.png` ve depo
kökündeki üç Claude Design görseli.

---

## 1 · Kısa özet

| # | Karar | Hüküm | Tek cümle |
|---|---|---|---|
| 1 | `Vadeli` satırı | **Koşullu** | Toplamı tutturur; ama alt yazısı ("Sonra ödenecek…") Karar 2 ile birlikte yanlış olur ve aynı güvenlik satırı kategori ekseninde de gerekir |
| 2 | Sonradan ödenen `Vadeli`de kalır | **Koşullu** | Cari için tek doğru yol; fatura (yükümlülük) için gerekçe yanlış — kapanış hangi faturayı kapattığını **biliyor** |
| 3 | POS, bağlı hesabın altında (A) | **Koşullu** | A tutarlı, B'ye karşı iki gerekçe doğru; ama "parasının girdiği/çıktığı hesap" cümlesi POS'ta doğru değil, kural yeniden yazılmalı |
| 4 | Gelir/gider yazmayan gösterilmez | **Sağlam** | Tek uyarı: Karar 5 taşıyan kaydı *satır* olarak geri getiriyor; "gösterilmez" tutar için geçerli |
| 5 | Parça kalemler | **Sağlam** | Doğru yön; kartla tahsilin komisyonunda "bağlı kayıt" henüz tanımsız |
| 6 | Faiz | **Koşullu** | Rol ve `Faiz geliri` bağlaması doğru; ad kilidi rol varken gereksiz, asıl kilitlenmesi gereken **taraf** |
| 7 | Masraf bağlı kaydın parçası | **Koşullu** | Yön doğru; model olarak yatış kesintisinin deseni seçilmeli; bugünkü yazım işletme kullanıcısında masrafı `Şahsi` yazıyor |
| 8 | Çubuklar seçili hesabı izler | **Sağlam** | Bir koşul: ay değişince seçili hesabın o ayda kaydı yoksa ne olacağı yazılmalı |

**En önemli üç bulgu.**

1. **Sayfanın temel vaadi ("satırlar toplamı tutar") bugün yalnız hesap
   ekseninde düşünülmüş; kategori ekseninde garanti yok.** Kategorisi boş kalem
   bugün iki yoldan doğabiliyor ve Karar 6 uygulandıktan sonra da doğabilir
   (rolü taşıyan kategori yoksa). `Vadeli` neyse, kategori listesinde de bir
   `Kategorisi yok` satırı odur.
2. **`Hesap / kart` menüsü para akışı değildir.** POS satışı brüt ve tahsil
   günüyle, komisyon hiç hesaptan geçmeden, gün sonunda sayılan veresiye de
   kasaya girdiği hâlde `Vadeli`de görünür. Kural "kaydın **yazıldığı** hesap ya
   da kart"tır; "parasının girdiği/çıktığı" diye yazılırsa hem ekip hem
   kullanıcı hesap sayfasıyla aynı sayıyı bekler ve bulamaz.
3. **`Kişi` menüsü ile `Hesap / kart` menüsü neredeyse hiç kesişmez.** Kişi
   taşıyan ve gelir/gider yazan üç kayıt türünün üçü de hesapsızdır; yani kişi
   seçilince sonuç hemen hep `Vadeli`nin içindedir. Claude Design çizimindeki
   "Test Toptancı · Dükkan Kasası −₺2.600" satırının modelde karşılığı yok.

---

## 2 · Olgusal iddiaların doğrulaması

| İddia | Sonuç | Kanıt |
|---|---|---|
| Gelir/gider yazan sekiz kaynak tek listede | **Doğru** | `RecognizedItems.cs:195-202` |
| Cari borçlandırma, yükümlülük ve gider/gelir kaynaklı borç açılışı hesap taşımaz | **Doğru** | `CounterpartyCharge.cs:25-43` (hesap alanı yok); `Obligation.cs:17-30`; `DebtAgreement.cs:266-275` (kategorili kaynakta `openingAccount` boş olmak zorunda) |
| Bunların dışında hesapsız tanıyan kayıt yok | **Doğru** | Kalan beş kaynağın her biri `AccountId` ya da `CreditCardId` taşıyor; borç faizi `PaymentAccountId` üzerinden (`RecognizedItems.cs:112-136`) |
| Cari ödeme hangi alımın karşılığı olduğunu bilmez | **Doğru** | `CounterpartyPayment` borçlandırmaya bağ taşımıyor; bakiye hareketlerden hesaplanıyor |
| Aynısı fatura (yükümlülük) için de geçerli | **Yanlış** | `Obligation.Settle` tek kapanış, tam tutar, faturanın kimliğiyle (`Obligation.cs:163-192`). Kapanış hangi faturayı, hangi hesaptan kapattığını biliyor |
| Bir yatış aynı hesaba bağlı birkaç POS'un tahsilatını kapatabilir | **Doğru** | `PosDeposit` POS kimliği taşımıyor (`PosDeposit.cs:25-53`); `Record` yalnız hesabı denetliyor (`:204-230`). Akış da POS adını yalnız tek POS varsa yazıyor (`EfFinancialActivityRepository.cs:1397-1409`) |
| POS'u seçilmeden yazılmış **eski** tahsilatlar var | **Eksik** | Yalnız eski değil: `PosDefinitionId` bugün de isteğe bağlı (`PosSettlement.cs:62`, `PosSettlementUseCases.cs:70`). İstemcinin bunu zorlayıp zorlamadığına bakmadım |
| POS komisyonu, kesinti ve taksit faizi akışta ayrı satır değil | **Doğru** | Kesinti gideri akıştan çıkarılıyor (`EfFinancialActivityRepository.cs:560-563`); `FeeAmount` satış (`:1336-1338`), yatış (`:1410-1412`), kartla cari tahsil (`:1092-1094`) ve kartla fatura tahsilinde (`:1258-1260`); faiz `InterestPortion` (`:838`) |
| Akışın kategori süzgeci parçaları getirmez | **Doğru** | Satışta `MatchCategoryId` satış kategorisi (`:1354`), taksitte ve yatışta boş (`:852`, `:1424`) |
| Alacak taksidinin faizi gelir toplamına girer, kategorisi `null` | **Doğru** | `RecognizedItems.cs:124-127` |
| Gider faizi kategoriyi adından bulur | **Doğru, bir ek** | `RecognizedItems.cs:128-134`: `category.Name ==` ile, `NameKey` ile değil. Teklik kuralı `NameKey`'de; yalnız noktalama düzelten ("…gideri.") bir yeniden adlandırma tekliğe göre "aynı ad"dır ama bu aramayı düşürür. `Kasa farkı` araması `NameKey` kullanıyor (`CashCountUseCases.cs:513`); iki ada bağlı kategori iki ayrı kuralla aranıyor |
| `InterestIncomeCategoryName` var | **Doğru, kullanılmıyor** | `EfCategoryRepository.cs:27`; üretim kodunda tek kullanım yok, yalnız bir set testi okuyor |
| İşletme setinde `Faiz geliri` yalnız `Şahsi` | **Doğru** | `DefaultCategorySets.cs:135` |
| İşletme setinde `Diğer gider` `Şahsi`, `Banka ve POS komisyonu` `İşletme` | **Doğru, bir ek** | `:177`, `:153`. Sette ayrıca `Diğer işletme gideri` (`İşletme`) var (`:162`); karar metninde anılmıyor |
| Kişisel sette banka masrafı kategorisi yok | **Doğru** | `DefaultCategorySets.cs:86-123` |
| Masraf yalnız dekont okununca, ana kayıttan sonra ikinci istekle, ayrı gider olarak, adından bulunan `Diğer gider`e yazılır | **Doğru, iki ek** | `receipt_fee_writer.dart:12`, `:76-97`, `:121-129`; dört yol: gider, transfer, kart ödemesi, borç verme (`app_router.dart:1214-1261`). **Ek 1:** gider yolunda kaynak kartsa masraf **kart harcaması** olarak yazılıyor (`:76-84`). **Ek 2:** istek `scope` göndermiyor (aşağıda Karar 7) |
| Aylık raporda kategori dağılımı var | **Doğru, yalnız gider** | `EfFinancialReportRepository.cs:70-86`, süzgeç `:75`. Gelir tarafının kategori dağılımı bugün hiçbir okumada yok; sayfanın `Gelir` yüzü yeni bir okumadır |

---

## 3 · Karar karar

### Karar 1 — `Vadeli` satırı · **Koşullu**

**Sağlam olan.** Tanıyan her kalem tam olarak bir yere düşer: bir hesap, bir
kart ya da hiçbiri. "Hiçbiri" için tek satır açmak menünün toplamını `Hepsi`ye
eşitler ve bu, çizim 01'deki 3.610'luk açığı gerçekten kapatır. Üç kaynağın
dışında hesapsız kalem olmadığını doğruladım.

**Koşullar.**

1. **Alt yazı yanlış olacak.** Çizimde "Sonra ödenecek alım ve faturalar"
   yazıyor. Karar 2 kabul edilirse bu satırın içinde ödenmiş fatura ve tahsil
   edilmiş veresiye de durur; satır "hâlâ borcum ₺3.610" diye okunur ve bu
   yanlıştır. Satır bir **yazım biçimini** anlatmalı, bir **durumu** değil:
   grup başlığı `Hesaptan geçmeyen` zaten doğru; alt yazı ya kalksın ya
   "Vadeli yazılan alım ve faturalar" gibi durum söylemeyen bir cümle olsun.
   Açık borcu gösteren yer Cari ve Yükümlülükler'dir.
2. **Üçüncü kaynak alt yazıda yok.** Gider/gelir kaynaklı borç açılışı (taksitle
   alınan mal, taksitle yapılan satış) da buraya düşer; "alım ve faturalar"
   onu kapsamıyor.
3. **Aynı güvenlik kategori ekseninde yok.** Menüde toplamı tutturan bir "artık"
   satırı var; kategori listesinde yok. Kategorisi boş kalem (Karar 6, bulgu
   D1) listede görünmez, toplamda kalır. Bu karar tek başına sayfanın vaadini
   tamamlamıyor.
4. **Pasif hesap ve kart menüde kalmalı.** `RecognizedItems` pasifliğe
   bakmıyor (doğru); menü "o ayda kaydı olanları" listelerken aktiflik
   süzgeci eklenirse toplam yine ayrışır.

**Kişi menüsüyle ilişkisi (düşünülmemiş).** Gelir/gider yazan ve kişi taşıyan
kayıtlar: cari borçlandırma, kişili yükümlülük, gider/gelir kaynaklı borç
açılışı. Üçü de hesapsızdır. `BudgetTransaction` ve `CreditCardCharge` kişi
alanı taşımıyor. Sonuç: kategori sayfasında `Kişi = X` seçilince kalemlerin
tamamı `Vadeli`dedir; `Hesap / kart = Ziraat` ile birlikte seçilirse liste
boştur (tek istisna: o kişiyle yapılmış borç anlaşmasının faizi ve kartla
tahsilin komisyonu). İki çip yan yana durduğunda kullanıcı ikisini birlikte
dener. Öneri: biri seçiliyken öbürünün menüsü yalnız kesişimde kaydı olanları
listelesin; kesişim boşsa çip çizilmesin. Aşama belgesindeki "kişi yalnız cari
kayıtta bulunur" cümlesi de eksik: yükümlülük ve borç anlaşması da kişi taşır.

### Karar 2 — Sonradan ödenen de `Vadeli`de kalır · **Koşullu**

**Cari için: sağlam, başka yol yok.** Ödeme borçlandırmaya bağlanmıyor; "bu
ödeme şu alımı kapattı" diyen bir veri yok ve uydurulamaz. Gider alımın
gününde yazıldı, ödeme gider yazmıyor (ADR 0014). Ödenen kısmı hesabın altına
taşımak için bir dağıtım kuralı gerekir (ilk giren ilk çıkar gibi) ve o kural
bir muhasebe kararıdır; ürünün sınırının dışında.

**Fatura (yükümlülük) için: gerekçe tutmuyor.** Kapanış tektir, tam tutardır ve
faturanın kimliğini taşır. Yani "ödenmiş faturayı ödendiği hesabın altına
koy" teknik olarak kesin yapılabilir. Karar yine de savunulabilir, ama
gerekçesi "bilemeyiz" değil, "tek kural olsun"dur. Bunu ayırmak önemli, çünkü
kullanıcı tarafında şu fark doğuyor:

| Aynı elektrik faturası, aynı ay | Yazılan gün | Menüde nerede |
|---|---|---|
| 10'unda ödendi, o gün "ödedim" diye girildi | 10 Ekim | `Ziraat Vadesiz` |
| 3'ünde "henüz ödemedim" diye girildi, 10'unda kapatıldı | 3 Ekim | `Vadeli` |

Aynı olay, girildiği yola göre iki ayrı yerde. Bu, Z6'da kapsam için
giderilmeye çalışılan sorunun ("aynı olay girildiği ekrana göre farklı
yazılıyor") hesap eksenindeki kopyası.

**Esnafın beklentisi.** Bu menüye bakan kişi iki sorudan birini sorar: "bu ay
ne harcadım, nereden" ya da "bu hesaptan ne çıktı". Sayfa birincisini
cevaplıyor; ikincisinin yeri hesap sayfası (Karar 4). Sorun, `Vadeli` satırının
üçüncü bir soru gibi okunması: "ne kadar borcum kaldı". Ödenmiş faturanın
orada durması bu okumayı yanlış çıkarır.

**Seçenekler.**

| | Ne olur | Sonucu |
|---|---|---|
| **A — Öneri olduğu gibi** | Kalem yazıldığı yerde kalır | Tek kural; ay kapandıktan sonra satırlar değişmez. Yukarıdaki tabloda iki yol ayrışır; `Vadeli` "açık borç" diye okunabilir |
| **B — Kapanmış fatura ödendiği hesaba geçer** | Yalnız yükümlülükte; cari `Vadeli`de kalır | Fatura iki yolda aynı yere düşer. Bedeli: iki kural (fatura / cari); geçmiş ayın menü satırları ödeme yapılınca değişir (toplam değişmez); kasımda ödenen ekim faturası ekim menüsünde hesaba kayar |
| **C — A + durum satırda yazar** | Kural A; kategori sayfasında `Vadeli` kalemi "Ödendi · 10 Eki · Ziraat Vadesiz" ya da "Açık" diye okunur (yalnız faturada; caride yazılamaz) | Tek kural kalır, yanlış okuma kapanır; menü satırının adı durum söylemez (Karar 1, koşul 1) |

**Önerim C.** A'nın kararlılığını korur, B'nin geriye dönük kaymasını
getirmez. B'yi seçmenin tek sağlam nedeni "aynı fatura iki yolda aynı yerde
görünsün" olur; bunu kullanıcı kendi kullanımında ne kadar önemsediğine göre
söyleyebilir.

**Gün sonunda sayılan veresiye.** Kodla tutarlı: gün sonu nakit satışı
"girilen − sayılan" kadar yazıyor (`DayClosePlan.cs:54`), veresiye kendi kaydı
olarak kalıyor, tahsilat parayı kasaya taşıyor. Günün geliri doğru. Ama
menüde `Dükkan Kasası` satırı o günün Z raporundaki nakitten, sayılan veresiye
kadar **eksik** görünür ve aynı tutar `Vadeli`de durur — para kasadayken. Bu
bir hata değil, menünün para akışı olmadığının en görünür örneği; sayfada tek
cümlelik bir açıklama ister (bulgu 2).

### Karar 3 — POS satışı ve komisyonu nerede · **Koşullu (A)**

**B'ye karşı iki gerekçe doğru.** Yatış POS kimliği taşımıyor; kesinti bir
POS'a bağlanamaz. POS'suz tahsilat da var (ve yalnız eski değil). B seçilirse
`Banka ve POS komisyonu` kategorisi ikiye bölünür (komisyon POS grubunda,
kesinti hesapta) ve POS'suz satışlar yine hesabın altında kalır; iki istisnalı
bir kural olur.

**A'nın gerekçesi ise yanlış yazılmış.** "Her kalem parasının girdiği/çıktığı
hesabın altında" POS'ta üç yerde tutmuyor:

- Satış **brüt** ve **tahsil günüyle** sayılır; hesaba **net** ve **yatış
  günü** girer (`AccountMovements.cs:193-204`). Ay sonuna yakın satışın parası
  o ay hesaba hiç girmemiş olabilir.
- Komisyon hesaptan hiç çıkmaz; banka eksik yatırır. Banka ekstresinde böyle
  bir satır yoktur.
- Kartla tahsilin komisyonu da aynı.

Kural kelimesi kelimesine uygulansa komisyon ve yoldaki satış "hesaptan
geçmeyen"e düşerdi. Fiilen uygulanan kural şudur ve böyle yazılmalı: **kalem,
kaydının taşıdığı hesap ya da kartın altındadır; hiçbirini taşımayan
`Vadeli`dedir.** Bu aynı zamanda sunucu için tek satırlık bir tanımdır
(`AccountId` / `CreditCardId` / ikisi de boş).

Bir ayrıntı: "POS'un bağlı hesabı" diye değil, **tahsilatın kendi hesabı**
diye okunmalı (`PosSettlement.AccountId`). POS'un hesabı sonradan
değiştirilirse yazılmış tahsilat değişmiyor; menü POS'un bugünkü hesabını
okursa eski satışlar yer değiştirir.

**Esnafın beklentisi.** Dükkân sahibi için gelirin ilk kırılımı "nakit / kart"tır;
POS cihazının gün sonu fişi de öyle konuşur. B'nin gelir yüzü bu soruyu
doğrudan cevaplıyor, A'da `Ziraat Vadesiz ₺24.900` içinde kartlı satış
havaleyle gelen parayla karışıyor. Buna karşılık sayfa "kategori özeti",
süzgeç "hesap / kart" ve POS ikisi de değil. A'yı seçerken kaybedilen şeyin
adı konmalı: **bu sayfa nakit/kart ayrımını vermez**; çizim 03 bunu "POS
sayfasında okunur" diye not etmiş. O sayfa gelene kadar bu soru cevapsız kalır.

**İkinci risk: hesap sayfasıyla aynı ad, farklı sayı.** Hesap sayfası
`AccountMovements` üstüne kurulacak (Z4). Aynı ay, aynı hesap için kategori
özeti "gelir ₺24.900", hesap sayfası "giren" olarak başka bir sayı gösterecek
(net, yatış günü, artı transferler ve tahsilatlar). İki sayı da doğru; yan
yana konduğunda hata gibi görünür. Menünün başlığı ("Ekim geliri · İşletme")
bunu taşıyor; hesap sayfası "giren / çıkan" derken bu sayfa "gelir / gider"
demeyi hiç bırakmamalı.

**Hüküm:** A, kural yukarıdaki gibi yeniden yazılırsa. B'yi gelir yüzünde
yeniden düşünmek için tek neden nakit/kart sorusudur ve onun doğru yeri
ayrı bir kırılım (POS sayfası), bu menü değil.

### Karar 4 — Gelir/gider yazmayan kayıtlar gösterilmez · **Sağlam**

Sayılan yedi tür `RecognizedItems`te yok, `AccountMovements`te var; iki liste
bu ayrımı zaten kodda taşıyor. Girselerdi vadeli alım hem `Vadeli`de hem
ödendiği hesapta sayılırdı; gerekçe doğru.

İki not:

- **Metin Karar 5 ile çelişir görünüyor.** Yatış, taksit ödemesi ve kartla
  tahsil taşıyan kayıtlardır ve Karar 5'te kategori sayfasına **satır** olarak
  geliyorlar (parçalarının taşıyıcısı olarak). Karar şöyle yazılmalı: bu
  kayıtların **tutarı** sayılmaz ve kendi başlarına satır olmazlar; bir parça
  kalem onları satır olarak getirebilir.
- **Sonucu açık söylenmeli.** Vadeli alımı çok olan bir dükkânda
  `Ziraat Vadesiz` satırı küçük, `Vadeli` büyük görünür; tedarikçiye ay içinde
  yapılan ödemeler bu sayfada hiçbir yerde yoktur. Doğru, ama hesap sayfası
  gelmeden "bu ay Ziraat'ten ne çıktı" sorusunun cevabı uygulamada yok.

### Karar 5 — Parça kalemler kategori sayfasında · **Sağlam**

Listeyi akışın kategori süzgecinden değil toplamı veren kalem listesinden
kurmak doğru ve bence tek doğru yol: akışta `MatchCategoryId` parçaları
tanımıyor, o yüzden akıştan kurulan liste çizim 02'deki gibi 1.140'ın yalnız
250'sini gösterirdi.

**Uygulamada karşılaşılacaklar.**

1. **`RecognizedItem` bugün beş alan taşıyor** (gün, tür, kapsam, kategori,
   tutar). Sayfa için gerekenler: kaynağın türü ve kimliği, bağlı kaydın türü
   ve kimliği, hesap, kart, kişi, giriş anı (gün içi sıra). Bunlar eklenince
   var olan altı okuma etkilenmemeli; `MoneyFoundation_*` testleri bunu
   koruyor.
2. **Kartla tahsilin komisyonunda bağlı kayıt tanımsız.** Karar "bağlı kayıt
   satır olarak gelir" diyor ve örnek satış. Kartla tahsilde satış yok: POS
   kaydı (`Kind = Collection`) akışta kendi satırı değil
   (`EfFinancialActivityRepository.cs:1303`); satır cari tahsilat ya da
   yükümlülük kapanışıdır ve komisyonu oradan okur. Kategori listesi bağlı
   kaydı `PosSettlementId` üzerinden bulmalı; satır metni de "satışın
   komisyonu" değil "tahsilatın komisyonu" olmalı.
3. **Aynı kayıt iki kategori sayfasında görünür**: satış `Satış geliri`nde
   brüt tutarıyla, `Banka ve POS komisyonu`nda komisyon tutarıyla. Doğru;
   ayrıntı paneli ikisinde de aynı kaydı açar.
4. **İşlemler'in kategori süzgeci ayrışık kalır.** Aynı kategori için kategori
   sayfası tam listeyi, İşlemler eksik listeyi verir (Z7 açık). Kategoriye
   İşlemler'den de kapı açılacaksa (§2 tablosu) iki liste yan yana gelir. Ya
   Z7 bu sayfayla birlikte kapanır ya da kategori kapısı yalnız bu sayfaya
   gider.

### Karar 6 — Faiz · **Koşullu**

**Katıldıklarım.** Alacak faizini `Faiz geliri`ne bağlamak ve kategoriyi
kalıcı bir işaretle bulmak. İkisi de bugün gerçek birer açık: gelir yüzünde
faiz hiçbir satırda yok; gider yüzünde ad değişince düşüyor (ve yukarıdaki
`Name` / `NameKey` farkı yüzünden yalnız noktalama düzeltmesinde bile
düşebilir).

**Koşullar.**

1. **Rol, kategorinin her zaman bulunacağını garanti etmez.** Backfill ada göre
   yapılacak; adı değiştirmiş ya da o kategori hiç kurulmamış kullanıcıda rol
   boş kalır ve faiz yine kategorisizdir. İki seçenek: rol taşıyan kategori
   yoksa ilk faizde açılır (`Kasa farkı`nın bugünkü deseni,
   `CashCountUseCases.cs:503-529`), ya da kategori listesinde `Kategorisi yok`
   satırı kalıcı bir güvenlik olarak çizilir. Bence ikisi birlikte: ilki
   normal durumu, ikincisi unutulan bir sonraki kaynağı kapatır.
2. **Asıl kilitlenmesi gereken taraf, ad değil.** Faiz kalemi kapsamını borç
   anlaşmasından alıyor (`RecognizedItems.cs:125`), kategorinin tarafına hiç
   bakmıyor. Kategori tek taraflıysa öbür taraftaki borcun faizi ADR 0020'nin
   "kategori tarafı sınırlar" ilkesini okuma tarafında bozar: `Şahsi`ye özel
   bir kategori `İşletme` süzgecinde tutar gösterir (çizim 04'ün "öneri"
   karesi tam bu durumu çiziyor). `Faiz geliri`ni iki tarafa açmak bu yüzden
   **gerekli**, ek bulgu doğru. Ama yetmez:
   - `EfCategoryUsageReader` faizi tanımıyor (`:21-111`): işletme borcu varken
     `Faiz ve finansman gideri` bugün `Şahsi`ye daraltılabilir. Rol taşıyan
     kategori daraltılamamalı.
   - Kişisel sette ikisi de `Şahsi` (`DefaultCategorySets.cs:93`, `:117`).
     Sonradan "işletmem var" diyen kullanıcı işletme borcu açarsa aynı çelişki
     doğar. Rol taşıyan kategori iki sette de iki tarafa açık olabilir;
     kişisel sette taraf zaten görünmüyor.
   - Set yalnız yeni kullanıcıya kuruluyor; var olan `Faiz geliri` bir
     backfill ile genişletilmeli. Genişletme her zaman serbest, kural buna
     izin veriyor.
3. **Adın sabitlenmesi.** Kullanıcının isteği; sonucu şu: rol varken ad kilidi
   doğruluğa bir şey katmaz (arama artık ada bakmıyor), yalnız kullanıcının
   `Kredi faizi` gibi kendi kelimesini yazmasını engeller. Rol **olmadan**
   yalnız ad kilidi ise yetmez: var olan yeniden adlandırılmış kayıtları ve
   yedekten gelen adları çözmez. İkisi birlikte yapılacaksa tutarlı biçimi:
   arama yalnız rolle; rol taşıyan kategoride ad değişikliği sunucuda
   reddedilir; `Kasa farkı` da aynı kurala girer (bugün üçüncü ada bağlı
   kategori o). Yalnız rolü seçip adı serbest bırakmak daha basit ve ADR
   0018'in "kimlik bir ada bağlanmaz" ilkesiyle aynı yönde; karar
   kullanıcının.
4. **Rol tekil olmalı** (kullanıcı başına, rol başına en çok bir kategori) ve
   **yedek şemasına girer**; rolsüz eski yedek geri yüklenirken ada göre
   eşleme orada da çalışmalı.
5. **Pasife alınan faiz kategorisi** faiz almaya devam eder (okuma aktifliğe
   bakmıyor). Doğru davranış bu; yalnız kategori özeti pasif kategoriyi de
   listelemeli.

### Karar 7 — Masraf bağlı kaydın parçası · **Koşullu; kapsamı için öneri**

**Önce bugünkü durumdan bir bulgu.** Masraf isteği `scope` göndermiyor
(`receipt_fee_writer.dart:76-97`) ve kategori `Diğer gider`. İşletme setinde
bu kategori `Şahsi`ye özel; kural gereği kayıt kategorinin tarafına yazılır
(`TransactionScopeResolution.cs:49-54`). Yani işletme hesabından tedarikçiye
yapılan havalenin masrafı bugün **şahsi gider** olarak yazılıyor. Koddan
okudum, çalıştırıp görmedim. Karar 7 ertelense bile bu ayrı bir düzeltme
konusu.

**Model: iki yol var, biri belirgin biçimde ucuz.**

| | Nasıl | Sonucu |
|---|---|---|
| **M1 — Kayıtta alan** (POS komisyonu gibi) | Her bağlı türe `FeeAmount` + kategori + kapsam | Altı-yedi varlığa şema; `RecognizedItems`e ve `AccountMovements`e tür başına birer yeni kaynak. İki tek-liste de büyür, her biri unutulabilir |
| **M2 — Bağlı gider kaydı** (yatış kesintisi gibi) | Masraf sıradan bir `BudgetTransaction`; bağlı kayıt onu kimliğiyle taşır; akışta ayrı satır olmaz, tek başına iptal edilemez | İki tek-listeye yeni kaynak **gerekmez** (gider zaten ikisinde de var). Var olan desenin (`PosDeposit.DeductionTransactionId`, `transactions.cancel_origin_locked`) genellemesi. Karar 5'in parça satırı aynı yoldan çalışır |

**Önerim M2.** Tek yazmada (bugünkü iki istek yerine) yazılması da bununla
gelir; bugün masraf isteği düşerse ana kayıt masrafsız kalıyor ve kullanıcıya
yalnız bir uyarı gösteriliyor.

**Hangi türler.** Parayı bir hesaptan çıkaran her kayıt aday: transfer, kart
ödemesi, cari ödeme, yükümlülük kapanışı (ödeme yönü), borç verme ve taksit
ödemesi, hesaptan gider. Tahsilat yönünde (cari tahsilat, alacak taksidi)
masrafı karşı taraf öder; orada gerekmez. Kart harcamasında "masraf" kartın
kendi ücretidir ve ayrı bir harcamadır. Bugün dekont yolunda kaynak kartsa
masraf kart harcaması olarak yazılıyor; bu yol korunacak mı, karar ister.

**Kategori ve taraf.** Faizden farkı: faizi kullanıcı girmiyor, uygulama
türetiyor; masrafı kullanıcı giriyor (ya da dekonttan onaylıyor). Bu yüzden
gizli bir rol araması yerine formda kategori alanı (son seçilen hatırlanır)
daha az sürpriz üretir. Yine de hazır bir kategori gerekir:

- İşletme setinde `Banka ve POS komisyonu` iki tarafa açılabilir (POS
  bağlamında kayıt yine `İşletme` yazılır; bağlam kazanıyor). Şahsi
  hesaptan yapılan havalenin masrafı da oraya düşer.
- Kişisel sete bir banka masrafı kalemi eklenir; var olan kullanıcıya set
  yeniden kurulmadığı için ya backfill ya ilk kullanımda açma gerekir.
- Taraf: kategori iki tarafa açıksa kaynağın etiketi ön değer olur; transfer
  kapsamsız olduğu için bağlamdan gelemez. İşletme hesabından şahsi hesaba
  transferin masrafında çip çıkar.

**Sıra.** Kategori özeti **önce**; şu koşulla: Karar 5'in parça mekanizması
"bir giderin bağlı olduğu kayıt" diye genel kurulsun (kesinti bugün zaten bu),
yalnız üç bilinen parçaya özel yazılmasın. O zaman masraf sonradan bir tür
olarak eklenir, sayfa yeniden yazılmaz. Arada dekont masrafları kendi
satırlarıyla görünür (çizim 02'deki "Havale masrafı · Kendi satırı"); bu
tutarlı bir ara durumdur.

**Dikkat:** bağlı kayıt kapsamsızsa (transfer, kart ödemesi) İşlemler'de kapsam
süzgeci satırı düşürür ve masraf sahipsiz kalır. Bu, Z7'de yazılı "parça
giderler sahipsiz kalır" sorununun aynısı; masraf eklendikçe büyür.

### Karar 8 — Çubuklar seçili hesabı izler · **Sağlam**

Çubuklar tür ve kapsamı izliyorsa hesabı da izlemeli; aksi hâlde seçili ayın
çubuğu sayfadaki sayıyla tutmaz. Maliyeti düşük: aynı kalem listesinden aya
göre gruplanmış tek sorgu (yılda on iki satır). Seçicinin başlığındaki sayı da
("Gider · İşletme · ₺26.330") hesabın adını taşımalı.

**Yazılması gerekenler.**

- **Ay değişince seçim.** Menü "yalnız o ayda kaydı olanları" listeliyor.
  Kullanıcı `Bonus Kart` seçiliyken kartın hiç kullanılmadığı bir aya geçerse
  çip ne yazar? Önerim: seçim korunur, çip adı yazmaya devam eder, sayfa boş
  durumunu gösterir, menü seçili olanı sıfır tutarla da listeler. Çubuklar
  hesabı izlediği için boş aylar önceden görünür; bu kararı destekleyen şey de
  bu.
- **Gelecek aylar.** Claude Design çiziminde Kasım ve Aralık seçilemez
  görünüyor. Gelecek tarihli gelir/gider yazılabiliyor (D5); o aylarda kayıt
  varsa çubuk çizilir mi, ay seçilebilir mi?
- **Ölçek.** Çubuk boyu yılın en büyük ayına göre mi, bütün geçmişe göre mi;
  süzgeç değişince ölçek de değişir. Küçük ama çizimde karar ister.

---

## 4 · Düşünülmemiş durumlar

| # | Durum | Ne olur | Öneri |
|---|---|---|---|
| D1 | **Kategorisi boş kalem.** Bugün: alacak faizi her zaman; gider faizi kategori bulunamazsa. Karar 6'dan sonra: rolü taşıyan kategori yoksa | Toplamda var, hiçbir kategori satırında yok. Çizim 04'ün iki kırmızı kutusu | Rol + yoksa açma + kategori listesinde kalıcı `Kategorisi yok` satırı. Gider için var olan sınama (`…MoneyFoundation.cs:57`) gelir yüzüne de yazılsın; bugün gelir için böyle bir sınama yok |
| D2 | **Gelir yüzü yeni bir okuma.** Kategori dağılımı bugün yalnız gider | Sayfanın `Gelir` sekmesi ilk kez hesaplanacak | `RecognizedItems` üstünden; ayrı bir kaynak listesi yazılmasın |
| D3 | **Borç anlaşması iptal edilemiyor.** Uçlar yalnız oluşturma, açılış kaydı ve taksit ödeme (`DebtEndpoints.cs:15-22`); varlıkta iptal yok | Yanlış girilmiş gider kaynaklı borcun gideri ve ödenmiş taksitin faizi kalıcıdır; `Vadeli`de düzeltilemeyen bir tutar durur | Bu sayfanın işi değil; ama kategori sayfasında bu kaleme dokunan kullanıcı "iptal"i arayacak. Kayda geçsin |
| D4 | **Açılışı sonradan kaydedilen borç** (eski kayıt, eski yedek) | `RecordOpening` geçmiş ödenmiş taksitlere faiz payı yazar (`DebtAgreement.cs:218-242`, `:326-335`) ve açılış gideri başlangıç gününe düşer: **kapanmış ayların** toplamı ve çubukları geriye dönük değişir | Bilinen ve kabul edilmiş bir davranışsa yazılsın; değilse kullanıcıya sorulsun |
| D5 | **Gelecek tarihli kayıt.** Gelir/gider, kart harcaması, cari borçlandırma ve borç başlangıcında "gelecekte olamaz" denetimi görmedim (`BudgetTransaction.cs:109`, `CreditCardCharge.cs:69`, `CounterpartyCharge.cs:97`; yükümlülük, POS ve yatışta var) | Gelecek ayın toplamında ve çubuğunda görünür; ay seçici o ayı kapalı çizerse kayıt ulaşılamaz | Ay seçici: kaydı olan gelecek ay seçilebilir olsun |
| D6 | **Pasif kategori, hesap, kart** | Okuma pasifliğe bakmıyor (doğru). Yeni uçta ya da menüde aktiflik süzgeci eklenirse toplam ayrışır | Yeni okumada aktiflik süzgeci olmasın; sınamaya pasif bir hesap ve kategori eklensin |
| D7 | **Kartla tahsilin komisyonu** | Gider olarak tanınıyor (`RecognizedItems.cs:182-193`, tür ayırmıyor), hesabı POS kaydının hesabı. Bağlı kaydı satış değil tahsilat | Karar 5, madde 2 |
| D8 | **Kasa farkı.** Eksik: `Kasa farkı` ya da seçilen kategori. Fazla: kullanıcı gelir kategorisi seçmek zorunda (`CashCountUseCases.cs:359-387`). `Kendime aldım` şahsi hesaba ise aktarım | Kategorisiz kalem doğmuyor. Aktarım bu sayfada görünmez (Karar 4). Şahsi kategoriyle yazılan fark kasadan çıkan şahsi giderdir: `İşletme` süzgecinde `Dükkan Kasası` satırında yoktur | Sorun yok; Y9'daki "öbür tarafta sayılmayan satır" bu sayfada uygulanmıyor, bilinçli olsun |
| D9 | **İade** | İade modeli yok; gelir olarak yazılıyor. Kişisel sette `İade ve geri ödeme` var, işletme setinde yok. Kategorinin gideri brüt kalır | Bu sayfa çözmez. İşletme kullanıcısının iadeyi nereye yazacağı açık bir soru |
| D10 | **Taksitli kart harcaması** | Yalnız gerçekleşen taksit kart harcaması olur, ay ay. 12 taksitli alım hiçbir ayda tek kalem değildir | Kural gereği doğru; sorun yok |
| D11 | **Tekrarlayan plan, vergi toplu ödemesi** | İkisi de sıradan gelir/gider ya da kart harcaması üretir; vergi ödemesi vergi işaretli kategori ister (`TaxUseCases.cs:122-126`) | Sorun görmedim |
| D12 | **Gün sonu** | Nakit satış kasaya, kartlı satış tahsilatın hesabına; sayılan veresiye `Vadeli`de. `Dükkan Kasası` satırı Z raporundaki nakitten sayılanlar kadar eksik | Karar 2 sonu; tek cümlelik açıklama |
| D13 | **Yedekten geri yükleme** | Rol alanı yedek şemasına girer. Eski kayıtlarda giriş anı boş; kategori sayfasının gün içi sırası o kayıtlarda belirsiz (akışta "günün sonuna düşer" kuralı var) | Kategori sayfası akışın sıralama kuralını aynen kullansın |
| D14 | **İşletmesi olmayan kullanıcı** | Kapsam çipleri çizilmez. `Vadeli` yine doğabilir (ödenmemiş fatura herkese açık). Faiz kategorileri `Şahsi`, tutarlı. Sonradan "işletmem var" derse Karar 6, koşul 2 | "Vadeli" kelimesi ev kullanıcısının ödenmemiş faturası için de okunaklı mı, çizimde bakılsın |
| D15 | **`Kişi` × `Hesap / kart`** | Kesişim neredeyse hep boş (Karar 1 sonu) | Menüler birbirini daraltsın ya da kesişim boşsa çip çizilmesin |
| D16 | **Aynı hesap, iki sayfa, iki sayı** | Kategori özeti brüt ve tahsil günü; hesap sayfası net ve yatış günü | "Gelir / gider" ile "giren / çıkan" kelimeleri karışmasın |
| D17 | **Komisyon ile kesinti farklı aylara düşer** | Komisyon satış gününde, kesinti yatış gününde tanınır; ay sonu satışının kesintisi ertesi ayın gideri | Kural gereği doğru; "geçen ay" kıyasında küçük oynama yapar |
| D18 | **`Banka ve POS komisyonu`nun bütçesi** | Bütçe aynı sayıyı okuyor; masraf (Karar 7) bu kategoriye girerse bütçesi olan kullanıcıda harcama artar | Karar 7 yazılırken anılsın |

---

## 5 · Doğrulayamadıklarım

- **Hiçbir şey çalıştırılmadı.** Karar 7'deki "masraf şahsi yazılıyor" bulgusu
  ve D5'teki "gelecek tarih kabul ediliyor" bulgusu kod okumasıdır; API'de
  denenmedi. D5 için uç katmanında ayrı bir doğrulayıcı olup olmadığına
  bakmadım.
- **İstemcinin POS'suz tahsilata izin verip vermediği.** Sunucu kabul ediyor;
  formun POS seçimini zorunlu tutup tutmadığını okumadım.
- **Veritabanının varsayılan harf kuralı.** `Name ==` aramasının büyük/küçük
  harf değişikliğinde tutup tutmadığı buna bağlı; `Name` kolonunda açık bir
  kural tanımlı değil (`CategoryConfiguration.cs:26-28`), sunucunun
  varsayılanını görmedim. Noktalama farkında düşeceği her durumda kesin.
- **Yedek/geri yükleme yolunun** kategori adlarını ve borç açılışını tam
  olarak nasıl kurduğu (D4, D13); `documentation/restore-runbook.md` ve geri
  yükleme kodu okunmadı.
- **`UNION ALL` üstünde yeni alanlarla gruplamanın** SQL'e çevrilip
  çevrilmeyeceği (Karar 5, madde 1); aşama belgesi bu riski Z3 için zaten
  yazmış, yeni alanlarla yeniden sınanmalı.
- **Esnafın beklentisi** (Karar 2 ve 3) akıl yürütmedir; kullanıcı
  görüşmesine ya da rakip gözlemine dayanmıyor.
- **Claude Design görsellerindeki sayılar** örnek veridir; yalnız yerleşim ve
  menü mantığı için baktım.

---

## 6 · Kullanıcıya gidecek sorular

1. `Vadeli` satırının alt yazısı durum söylemeyecek biçimde değişsin mi
   (Karar 1)?
2. Ödenmiş fatura: yazıldığı yerde kalıp satırda "Ödendi" mi yazsın (C), yoksa
   ödendiği hesabın altına mı geçsin (B) (Karar 2)?
3. A seçilirken "bu sayfa nakit / kart ayrımını vermez" kabul mü; POS sayfası
   gelene kadar bu soru cevapsız kalır (Karar 3).
4. Kategori listesinde kalıcı bir `Kategorisi yok` satırı olsun mu (D1)?
5. Faiz kategorilerinde ad kilidi rolle birlikte de isteniyor mu; `Kasa farkı`
   aynı kurala girsin mi (Karar 6)?
6. Dekont masrafının işletme kullanıcısında `Şahsi` yazılması bugün
   düzeltilsin mi, Karar 7'yi mi beklesin?
7. Masraf için M2 (bağlı gider kaydı) kabul mü; kart kaynağında masraf kart
   harcaması olarak yazılmaya devam etsin mi (Karar 7)?
8. Ay değişince seçili hesap korunup boş durum mu gösterilsin (Karar 8)?
