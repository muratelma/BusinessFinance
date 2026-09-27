# Uygulama Gözlem Formu — QuickBooks Solopreneur (Intuit)

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | QuickBooks Solopreneur / Intuit Inc. |
| Sürüm | Mobil 30.4.7 (React Native) — onboarding'e girildi (sürüm dört karenin hiçbirinde görünmüyor, koşum notu) |
| Test tarihi | **1–2 Eyl 2026** (emülatör, onboarding) · **9 Eyl 2026** (Intuit yardım merkezi) · **12 Eyl 2026** (Faz 7.5 doğrulama turu) |
| Cihaz / işletim sistemi | Android emülatör + masa başı (yardım merkezi) |
| Dil / para birimi | İngilizce / ekranda **TRY** (mağaza bölgesi Türkiye), ürünün kendi ekseni USD |
| Erişim kısıtı | **Girilemedi — ödeme kapısı + bölge.** 2 Eyl'de kullanıcı Intuit hesabı açtı (e-posta + SMS) ve QuickBooks mobil onboarding'ine girdi; QBO `Simple Start` plan ekranında durdu. Kullanıcı notu: *"1 aylık deneme bile kredi kartı istiyor"* — bu **karede görünmüyor**, manuel koşum notudur ve Simple Start ekranındaki denemeye aittir; Solopreneur'ün deneme koşulu ayrıca doğrulanmadı. QuickBooks Solopreneur ABD dışına kapalı |
| İnceleme türü | **Masa başı.** Manuel gözlem yalnız onboarding'in ilk dört ekranı; K01–K08 ve işletme/şahsi sınıflandırmanın tamamı `Resmî kaynak` |

### Kanıt tavanı (bu formun sınırı)

**Uygulamanın içine hiç girilemedi.** Dahası, elde edilen dört kare
**Solopreneur'ün kendisini değil**, QuickBooks mobil uygulamasının genel
onboarding'ini ve sonunda teklif edilen **`Simple Start`** planını gösteriyor
(`04-choose-plan-paywall.png`). `Simple Start` QuickBooks **Online**'ın giriş
planıdır; Solopreneur ayrı bir üründür. Yani:

- **Manuel gözlem = QuickBooks mobil onboarding'i** (dört ekran).
- **Solopreneur'e dair her cümle = Intuit yardım merkezi** (resmî kaynak).
- Solopreneur'ün kendi arayüzünün **tek bir ekranı bile görülmedi.**

Bu ayrım formun her yerinde korunuyor; "Type: Business/Personal" gibi bizim
için en kritik bulgu dahi **yalnız yardım merkezine** dayanıyor.

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | Tek kişilik işletmenin banka hareketlerini otomatik indirip her birini **İşletme / Şahsi** olarak etiketleyen, yıl sonunda **Schedule C** vergi beyanına hazır çıktı veren "hafif" araç |
| Asıl hedef kullanıcı | ABD'de **Schedule C dolduran** sole proprietor / tek üyeli LLC; muhasebe bilgisi olmayan, tek işi olan girişimci |
| Çözdüğü ana iş | İşletme ve şahsi harcamanın **vergi amaçlı ayrıştırılması** + tahmini üç aylık vergi + fatura kesme + km takibi |
| Açıkça kapsam dışı bıraktığı | **Çift taraflı muhasebe yok, bilanço yok, hesap planı özelleştirilemez, tek işletme sınırı, borç/alacak (A/P–A/R) modülü yok.** Bunları isteyen QuickBooks Online Simple Start'a yönlendiriliyor |
| İş modeli | **Solopreneur:** ücretli abonelik, ücretsiz katman yok (yardım merkezi/ürün sayfası beyanı); Solopreneur'ün güncel fiyatı ve deneme koşulu bu çalışmada doğrulanmadı. **Karelerdeki fiyat Solopreneur'ün değildir:** QuickBooks mobil onboarding'inin sonunda teklif edilen QBO `Simple Start` kartı — liste fiyatı ~~TRY 819,99~~, **TRY 244,99/ay "for 6 months"** (kampanya, 2 Eyl yakalama anı, `04-choose-plan-paywall.png`) |
| BusinessFinance ile aynı kulvarda mı | **Kısmen — kavramsal olarak en yakın rakip.** "İşletme ve şahsi para tek yerde, ayrım bir etikettir" fikri ortak. Ama QBS'nin ayrımı **vergi (Schedule C) eksenli ve ABD'ye özel**; bizim kapsamımız bir **raporlama boyutu** (ADR 0013) ve vergi hesaplanmıyor |

## Görev gözlemleri

| Görev | Sonuç | Not | Kanıt |
|---|---|---|---|
| K00 İlk açılış | Kısmi (manuel) | QuickBooks mobil uygulamasının **3 noktalı** onboarding karuseli; görülen slayt: *"Get paid anywhere — Send and track custom invoices."* Altta yeşil dolu `Create account` + çerçeveli `Sign in`; karede ürün adı yazmıyor | `01-onboarding.png` |
| K00 (hesap açma) | Tamamlandı (manuel, 2 Eyl) | Kullanıcı e-posta + SMS ile Intuit hesabı açtı. *"Welcome to QuickBooks, we're glad you're here"* + üç madde: *"First, let's talk about your business."* / **"Then, choose a plan or try it out for free."** / *"After that, we'll get some things set up for you."* → `Get started` | `02-welcome-get-started.png` (yalnız hoş geldin ekranı; e-posta/SMS adımları karede yok, koşum notu) |
| K00 (onboarding) | Kısmi (manuel) | *"Let's begin with some basic info"* → **tek alan: `Business name`** + altında **"No business name? Use your name"** bağlantısı. Üstte `Sign out`, alt metin *"We'll use this to get you started in QuickBooks."* "Deniz Tasarim" → `Next` (karede alan boş; giriş koşum notu) | `03-onboarding-basic-info.png` |
| K00 (plan kapısı, QBO Simple Start) | Engelli (manuel) | Ekran başlığı **`Simple Start`** (QuickBooks Online planı; Solopreneur plan ekranı değil); üstte *"Subscribe and save or try free for 1 month*"*. Kart: ~~TRY819.99~~ **TRY244.99/mo** *"for 6 months"* + `Get started`. Özellikler: `Track income/expenses` · `Send custom invoices/estimates` · `Auto-track mileage` · `Create custom categories` · `Run reports`. Altta `Simple Start details*` / `Privacy` / `Terms of service`. **Plan seçim listesinin kendisi çekilmedi**; karede yalnız bu tek plan var | `04-choose-plan-paywall.png` |
| K01–K08 | **Engelli → resmî kaynak** | Aşağıdaki "Sistem işleyişi" bölümünden | — (`Doğrulanamadı`) |

**Vaat ile kapı arasındaki gerilim (QuickBooks mobil onboarding → QBO Simple
Start; Solopreneur'ün deneme koşulu değildir):** `02-welcome-get-started.png` karesi kullanıcıya *"choose a plan
**or try it out for free**"* diyor; `04-choose-plan-paywall.png` karesinin üst bandı da
`Subscribe and save or try free for 1 month*` yazıyor, ama ekranda ayrı bir
deneme düğmesi yok — tek eylem kart içindeki `Get started` ve denemeyi mi
aboneliği mi başlattığı karede belli değil. Ücretsiz denemenin kart istediği
bilgisi kullanıcının manuel koşumundan geliyor, karede yazmıyor.

## Sistem işleyişi / pipeline

`Resmî kaynak` — Intuit yardım merkezi (9 Eyl 2026). **Canlı ürün davranışı
değildir; Solopreneur arayüzü hiç görülmedi.**

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Bir gelir/gider kaydı arka planda ne üretir | Kategorize edilen her işlem **Schedule C'de bir satıra eşlenir** (*"Each time you categorize a transaction, QuickBooks matches it to a line on your Schedule C"*). Kâr-zarar, nakit akışı geçmişi ve genel bakış panosu bu kayıtlardan türetilir | Resmî kaynak |
| İşlemler sisteme nasıl giriyor | Kullanıcı banka + kredi kartı hesaplarını bağlar → QBS **son işlemleri otomatik indirir** ve "sizin ve diğer müşterilerin benzer işlemleri nasıl kategorize ettiğine göre" otomatik kategori önerir. Manuel ekleme ve fiş fotoğrafı da var | Resmî kaynak |
| İşletme/şahsi ayrım hangi katmanda | **İşlem başına bir alan: `Type` sütunu = Business / Personal.** Üçüncü değer yok. İşlem listesinde `Type` başlığından süzülüyor. Şahsi işaretlenen işlem vergi/işletme raporuna girmez ama **silinmez** | Resmî kaynak |
| Bölünmüş işlem (kısmen işletme, kısmen şahsi) | İşlem → `Edit` → **`Split transaction`** → tutarı böl → her parçayı ayrı ayrı personal / business işaretle + kategori seç; toplam orijinal tutara eşitlenir. **İstisna:** araç/yakıt giderini bölme, tümünü işletme işaretle — oranı muhasebeci hesaplar | Resmî kaynak |
| Tekrarlayan işlem | **Kural (`Rules`) motoru** — en çok 30 kural; tekrarlayan/benzer işlemi otomatik kategorize eder ve zamanla kullanıcının kalıbını öğrenir | Resmî kaynak |
| Hariç tutma | İşlem **silinmez**, `Exclude` ile hariç tutulur (yinelenen/ilgisiz kayıt) | Resmî kaynak |
| Transfer / hesaplar arası | İşletme/şahsi bağlamında ayrı bir "transfer" modeli belgelenmemiş; QBS çift taraflı muhasebe yapmıyor | Doğrulanamadı |
| Kategori sistemi | Kategoriler **IRS Schedule C (Form 1040)** kalemleriyle hizalı (Advertising, Car and truck, Legal and professional, Utilities…). Kullanıcı en çok 180 kategori ekleyebilir; hesap planı özelleştirmesi yok | Resmî kaynak |
| Veri nereye yazılıyor | Bulut; bağlı banka hesaplarından senkron | Resmî kaynak |
| Muhasebeci tarafı | Solopreneur'da muhasebeci erişimi/dışa aktarma yardım merkezinde net belgelenmemiş; ürün "muhasebecin gerekmeden" konumlanıyor | Doğrulanamadı |

**Pipeline şeması (resmî kaynak):**
`banka/kart bağla → işlemler otomatik iner → otomatik kategori + Type önerisi →
kullanıcı "Bank transactions" ekranında gözden geçirir → Type
(Business/Personal) + kategori düzelt / gerekirse Split → Rules benzerlerini
otomatikleştirir → P&L + nakit akışı + üç aylık tahmini vergi + yıl sonu
Schedule C`

## Doküman akış yeniden kurulumu

`Resmî kaynak` — Intuit yardım merkezi makaleleri. **Ekran görüntüsü yok**;
adımlar makale metninden yeniden kuruldu.

| Akış | Kaynak makale | Adımlar | BusinessFinance karşılığı |
|---|---|---|---|
| Gider girişi ve sınıflandırma | "Categorize bank transactions in QuickBooks Solopreneur" | All apps → Accounting → Bank transactions → Update → liste → `Type` sütunundan Business/Personal süz → `Category` açılırından düzelt | Bizde işlem formunda kapsam çipi + kategori; banka bağlama yok, kayıt elle/fiş önerisiyle |
| Kısmen işletme gideri | "Split transactions in QuickBooks Solopreneur" | İşlem → `Edit` → `Split transaction` → tutar böl → parça başına personal/business + kategori | **Bizde bir kaydın tek kapsamı var; bölme yok** — kapsam kayıt düzeyinde, kalem düzeyinde değil |
| Tekrarlayan gideri otomatikleştirme | "Use rules to categorize bank transactions" | Accounting → `Rules` → New rule → koşul + kategori + Type | Bizde `RecurringTransaction`: tanım rapor üretmez, onayla `realize` olur. QBS kuralı **banka feed'ini etiketliyor**, bizimki **kayıt üretiyor** — farklı işler |
| Üç aylık tahmini vergi | Solopreneur ürün sayfası | Kategorize edilmiş işlemlerden tahmini vergi hesaplanır, ödeme hatırlatması verilir | Bizde yok: uygulama vergi hesaplamaz (ADR 0016), yalnız tutarsız takvim önerisi döndürür |
| Fatura kesme | Solopreneur ürün sayfası | Müşteri seç → kalem → ödeme yöntemi (kart/ACH/PayPal/Venmo) → gönder + otomatik hatırlatma | Bizde e-belge/fatura yok; `Obligation` / `CounterpartyCharge` tanıma modeli var |

## Arayüz incelemesi (onboarding, sınırlı)

| Başlık | Kısa gözlem |
|---|---|
| Onboarding tonu (QuickBooks mobil → QBO Simple Start) | 3 noktalı karusel (bir slayt görüldü) + üç maddelik hoş geldin + tek alanlı "basic info" — düşük sürtünme görünümü; ardından plan ekranı. "Sert ödeme duvarı" yorumdur: karede deneme ifadesi var, deneme yolunun koşulu görülmedi |
| Form alanları | Tek alan `Business name` (zorunluluk işareti karede yok); altında **"No business name? Use your name"** — şahıs şirketi gerçeğini kabul eden mikro kopya |
| Fiyat sunumu (QBO Simple Start kartı; Solopreneur fiyatı değil) | Liste fiyatı üstü çizili, kampanya fiyatı büyük punto, süre küçük punto (`for 6 months`) — iki fiyatın oranı ≈ 3,35; kampanya bitiminde bu fiyata dönüleceği `çıkarım` — karede yalnız `for 6 months` ve yıldızlı `Simple Start details*` bağlantısı var |
| Bilgi hiyerarşisi (resmî kaynak) | İşlem listesi merkezli; `Type` ve `Category` **iki ayrı sütun** — ayrım ile sınıflandırma görsel olarak da ayrılmış |

## Akış özeti

- **En kısa ve güçlü akış:** banka bağla → işlemler otomatik iner ve otomatik
  kategorize olur → kullanıcı yalnız düzeltir. Sıfırdan veri girişi yok
- **En fazla sürtünme yaratan akış:** QuickBooks mobil onboarding'inin hemen
  ardından QBO `Simple Start` plan ekranı (Solopreneur'ün kendi akışı görülmedi); deneme ifadesi ekranda var ama deneme yolu ve kart istemi karede
  görülmedi (kullanıcı notu), ürün denenmeden kalındı
- **Hedef kullanıcı varsayımı:** ABD'de Schedule C dolduran, banka hesabını
  bağlamaya istekli, tek işi olan girişimci
- **İşletme ve şahsi para yaklaşımı:** tek işlem akışı + işlem başına
  `Business`/`Personal` etiketi — kavramsal olarak ADR 0013'e en yakın rakip,
  ama ekseni vergi
- **Transfer ve kart ödemesi yaklaşımı:** belgelenmemiş; çift taraflı muhasebe yok
- **Planlama, borç ve tahsilat yaklaşımı:** borç/alacak modülü yok; "planlama"
  = tahmini vergi + hedefler

## BusinessFinance için kararlar

> `alma` ve `kararı yeniden sor` satırlarında "ne kazandırıyor / ne
> kaybettiriyor" zorunludur (`MANUEL-TEST-PROTOKOLU.md` → yazım kuralı 6).
> Sonuç tanımları: `README.md` → "Karar sonuçları — tek kaynak".
>
> **Bu tablonun tamamı masa başı kaynaklara dayanıyor**; Solopreneur arayüzünün
> hiçbir ekranı görülmedi.

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşlem başına tek `Type: Business/Personal` alanı, üçüncü değer yok | Doğrudan al (zaten böyle) | `TransactionScope` ile birebir aynı: `Business = 1`, `Personal = 2`, "bilinmiyor" yok. Pazardaki en güçlü doğrulayıcı referans | Kapsam boyutu |
| `Type` ve `Category` ayrı sütunlar / ayrı alanlar | Doğrudan al (zaten böyle) | Bizde de kapsam kategoriyle temsil edilmez, ayrı boyuttur; QBS bunu görsel olarak da ayırmış | İşlem formu, feed filtresi |
| **`Split transaction` — tek harcamayı kalem kalem bölüp her parçaya ayrı Business/Personal vermek** | **Kararı yeniden sor** | **Boyut:** aynı harcamanın bir kısmı işletme bir kısmı şahsi olan kalemlerde (yakıt, telefon, ev ofisi elektriği) kaydın doğru bölünmesi — şahıs şirketi ve esnafta sık, hedef kitlemizin gündelik işi. **Kazandırdığı:** kullanıcı tek kaydı ikiye bölüp her parçayı doğru tarafa yazıyor; iki ayrı kayıt uydurmak veya tamamını bir tarafa atmak zorunda kalmıyor. **Kaybettirdiği:** kapsam artık kayıt düzeyinde tek değer olmaktan çıkıyor; `BudgetTransaction` başına tek `TransactionScope` varsayımı, bütçe ilerlemesi (kategori + kapsam çifti) ve `scopeBreakdown` bu değişiklikten etkilenir. **Karşı ağırlık:** QBS'nin kendisi de en sık bölünen kalemde (araç/yakıt) bölmeyi **önermiyor** — "tümünü işletme işaretle, oranı muhasebecin hesaplasın" diyor; yani bölme her yerde doğru cevap değil. ADR 0013 yeniden değerlendirilmeli mi, Belge 3'te ödünleşimiyle sorulur | Kapsam boyutu, işlem formu, aylık rapor kırılımı |
| Şahsi işaretlenen işlem işletme/vergi raporundan düşer ama **silinmez** | Uyarlayarak al | Bizde de kapsam filtresi raporu böler. Fark: bizde şahsi kayıt **net varlıktan düşmez** (ADR 0013: kapsam parayı bölmez, raporu böler); QBS'de şahsi taraf işletme bakışından tamamen çıkar | Rapor, özet hero |
| Banka bağlantısıyla otomatik indirme + otomatik kategori + `Rules` motoru | Alma | **Kazandırdığı:** veri girişi neredeyse sıfıra iniyor; kullanıcı yazmıyor, yalnız düzeltiyor ve kural motoru zamanla düzeltmeyi de azaltıyor — bu ürünün en güçlü yanı. **Kaybettirdiği:** sağlayıcı bağımlılığı, banka kimlik bilgisi güven sınırı ve düzenleme yükü; ayrıca kategori önerisi "diğer müşterilerin nasıl kategorize ettiğine" dayandığı için kullanıcının verisi başkalarının kalıbıyla etiketleniyor. `PROJECT-ROADMAP` açık bankacılığı kesin kapsam dışı bırakıyor | — |
| Schedule C eşlemesi / tahmini üç aylık vergi / beyana hazır çıktı | Alma | **Kazandırdığı:** yıl sonu beyan işi ürünün içinde bitiyor; kullanıcı ayrı bir hazırlık yapmıyor ve vergiyi yıl boyunca tahminle görebiliyor. **Kaybettirdiği:** ürün tek bir ülkenin vergi formuna çivileniyor — Solopreneur'ün ABD dışına kapalı olmasının sebebi de bu; mevzuat değişince ürün değişmek zorunda ve yanlış hesap kullanıcıyı doğrudan riske sokuyor. Bizim kritik kısıtımız net: uygulama vergi hesaplamaz, beyanname üretmez, "kâr" demez | — |
| `Exclude` — kaydı silmek yerine hariç tutmak | Doğrudan al (ruhen) | "Sil yerine koru" ilkemizle aynı yön; bizde idempotent iptal aynı işi görüyor | İşlem iptal |
| "No business name? Use your name" mikro kopyası | Uyarlayarak al | Şahıs şirketi gerçeğini kabul eden dil; onboarding tek sorumuzla (`HasBusiness`) aynı ruh | Onboarding |
| Çift taraflı muhasebe / bilanço / hesap planı yokluğu (bilinçli sadelik) | Doğrudan al (aynı felsefe) | Biz de muhasebe uygulaması değiliz; "hafif, tek kişilik, vergi-hazır" konumu bizim "işletme finansı takibi" konumumuza yakın | — |
| QBO `Simple Start` plan kartında kampanya fiyatının büyük, liste fiyatının üstü çizili sunulması; ücretsiz deneme vaadinin hemen kapıya çıkması (Solopreneur'ün fiyat sunumu değil) | Alma | **Kazandırdığı:** dönüşüm oranını artıran standart bir sunum; kullanıcı ilk ayda düşük rakam görüyor. **Kaybettirdiği:** `02-welcome-get-started.png` ekranı "try it out for free" diyor, `04-choose-plan-paywall.png` üst bandında da deneme yazıyor ama ayrı bir deneme düğmesi yok ve kart istemi yalnız kullanıcı notunda — vaat ile yol arasındaki belirsizliğin güveni zedelediği yorumdur. Bizde fiyat/plan kararı henüz yok, ama bu desen alınmamalı | — |

## Kanıt ve güven düzeyi

- **Manuel gözlem (1–2 Eyl 2026):** yalnız onboarding'in dört ekranı —
  `01-onboarding.png`, `02-welcome-get-started.png`,
  `03-onboarding-basic-info.png`, `04-choose-plan-paywall.png`.
  **Bu kareler Solopreneur arayüzü değildir**; QuickBooks mobil
  onboarding'i ve `Simple Start` plan sayfasıdır. Durum çubuğu `01-onboarding.png` 1:34,
  E0269–E0271 7:32–7:33 (iki ayrı oturum); 14 Eyl 2026'da dört kare yeniden
  açıldı (`KANIT-ENVANTERI.md` P1-quickbooks-G01)
- **Resmî kaynak (yardım merkezi):** Intuit — "Introduction to QuickBooks
  Solopreneur", "Categorize bank transactions", "Use rules to categorize",
  "Split transactions", "Schedule C expense categories", Solopreneur ürün ve
  business/personal sayfaları (9 Eyl 2026)
- **Çıkarım:** "ADR 0013'e en yakın rakip" değerlendirmesi — QBS'nin işlem
  başına Business/Personal etiketiyle bizim kapsam boyutumuzun aynı fikir
  olmasından türetildi; eksen farkı (vergi vs. raporlama) ayrıca yazıldı
- **Doğrulanamadı:** Solopreneur'ün **hiçbir ekranı ve hiçbir davranışı**;
  transfer/kart ödemesi modeli; muhasebeci erişimi; plan seçim listesinin
  tamamı (`04-choose-plan-paywall.png` yalnız `Simple Start`'ı gösteriyor); QBO Simple Start
  ekranındaki ücretsiz denemenin kredi kartı istediği (kullanıcı notu, karede
  yok); Solopreneur'ün kendi fiyatı, deneme koşulu ve onboarding'i
- **Kasten yapılmadı:** ödeme kapısının aşılması. Ürün ABD dışına kapalı ve
  ücretli; kart bilgisi girilmedi

## Tek cümlelik sonuç

QuickBooks Solopreneur, işletme ve şahsi parayı tek işlem akışında tutup her
işleme `Business`/`Personal` etiketi vermesiyle ADR 0013'ün pazardaki en yakın
kavramsal karşılığı; ayrımı ABD Schedule C beyanına hizalı olduğu için ekseni
bizimkinden farklı ve banka bağlama + vergi hesaplama gibi bizde kapsam dışı
mekanizmalara dayanıyor — ayrıca `Split transaction` ile tek harcamayı iki
kapsama bölebilmesi, bizde karşılığı olmayan ve Belge 3'te sorulması gereken
tek noktası.
