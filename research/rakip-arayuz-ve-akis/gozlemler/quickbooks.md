# Uygulama Gözlem Formu — QuickBooks Solopreneur (Intuit)

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | QuickBooks Solopreneur / Intuit Inc. |
| Sürüm | Mobil 30.4.7 (React Native) — onboarding'e girildi |
| Test tarihi | 1–2 Eylül 2026 (emülatör), 9 Eylül 2026 (resmî kaynak) |
| Cihaz / işletim sistemi | Android emülatör + masa başı (yardım merkezi) |
| Dil / para birimi | İngilizce / USD |
| Erişim kısıtı | **Girilemedi — ödeme kapısı + bölge.** 2 Eyl'de kullanıcı Intuit hesabı açtı (e-posta + SMS), onboarding'e girdi; "Choose a plan" ekranında durdu (ücretli plan zorunlu, 1 aylık deneme bile kredi kartı istiyor). QuickBooks Solopreneur ABD dışına kapalı. K01–K08 ve işletme/şahsi sınıflandırma **tümüyle resmî kaynak** |
| İnceleme türü | Resmî kaynak (onboarding'in ilk 4 ekranı manuel gözlem) |

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | Tek kişilik işletmenin banka hareketlerini otomatik indirip her birini **İşletme / Şahsi** olarak etiketleyen, yıl sonunda **Schedule C** vergi beyanına hazır çıktı veren "hafif" araç |
| Asıl hedef kullanıcı | ABD'de **Schedule C dolduran** sole proprietor / tek üyeli LLC; muhasebe bilgisi olmayan, tek işi olan girişimci |
| Çözdüğü ana iş | İşletme ve şahsi harcamanın **vergi amaçlı ayrıştırılması** + tahmini üç aylık vergi + fatura kesme + km takibi |
| Açıkça kapsam dışı bıraktığı | **Çift taraflı muhasebe yok, bilanço yok, hesap planı (chart of accounts) özelleştirilemez, tek işletme sınırı, borç/alacak (A/P–A/R) modülü yok.** Bunları isteyen QuickBooks Online Simple Start'a yönlendiriliyor |
| İş modeli | **Ücretli abonelik** (Simple Start ~244,99 TRY/ay listelendi; ABD'de ~20 USD/ay). Ücretsiz katman yok |
| BusinessFinance ile aynı kulvarda mı | **Kısmen — kavramsal olarak en yakın rakip.** "İşletme ve şahsi para tek yerde, ayrım bir etikettir" fikri ortak. Ama QBS'nin ayrımı **vergi (Schedule C) eksenli ve ABD'ye özel**; BusinessFinance'in kapsamı bir **raporlama boyutu** (ADR 0013), vergi hesaplamıyor |

## Görev gözlemleri

| Görev | Sonuç | Not |
|---|---|---|
| K00 İlk açılış | Kısmi (manuel) | 3 slaytlık onboarding ("Get paid anywhere" vb.), yeşil "Create account" + çerçeveli "Sign in" (`kanitlar/quickbooks/01`) |
| K00 (hesap açma) | Tamamlandı (manuel, 2 Eyl) | Kullanıcı e-posta + telefon SMS ile Intuit hesabı açtı. Welcome ("we're glad you're here", 3 madde) → Get started (`kanitlar/quickbooks/02`) |
| K00 (onboarding) | Kısmi (manuel) | "Let's begin with some basic info" → tek alan **Business name** (+ "No business name? Use your name"). "Deniz Tasarim" → Next (`kanitlar/quickbooks/03`) |
| K00 (plan kapısı) | Engelli (manuel) | **"Choose a plan"** — Simple Start (TRY 244,99/ay). Özellikler listesi: Track income/expenses, Send custom invoices/estimates, Auto-track mileage, Create custom categories, Run reports. "try free for 1 month" bile kredi kartı istiyor → durduruldu (`kanitlar/quickbooks/04`) |
| K01–K08 | **Engelli → resmî kaynak** | Aşağıdaki "Sistem işleyişi" ve "Video/doküman akış yeniden kurulumu" bölümlerinden |

## Sistem işleyişi / pipeline

`Resmî kaynak` — Intuit yardım merkezi (9 Eyl 2026). Canlı ürün davranışı değildir.

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Bir gelir/gider kaydı arka planda ne üretir | Kategorize edilen her işlem **Schedule C'de bir satıra eşlenir** ("Each time you categorize a transaction, QuickBooks matches it to a line on your Schedule C"). Kâr-zarar (P&L), nakit akışı geçmişi ve genel bakış panosu bu kayıtlardan türetilir | Resmî kaynak |
| İşlemler sisteme nasıl giriyor | Kullanıcı banka + kredi kartı hesaplarını bağlar → QBS **son işlemleri otomatik indirir** ve "sizin ve diğer müşterilerin benzer işlemleri nasıl kategorize ettiğine göre" otomatik kategori önerir. Manuel ekleme ve fiş fotoğrafı da var | Resmî kaynak |
| İşletme/şahsi ayrım hangi katmanda | **İşlem başına bir alan: "Type" sütunu = Business / Personal.** Üçüncü değer yok. İşlem listesinde Type başlığından süzülür. Şahsi işaretlenen işlem vergi/işletme raporuna girmez ama silinmez | Resmî kaynak |
| Bölünmüş işlem (kısmen işletme kısmen şahsi) | İşlem → Edit → **Split transaction** → tutarı böl → her parçayı ayrı ayrı **personal / business** işaretle + kategori seç. Toplam orijinal tutara eşitlenir. (İstisna: araç/yakıt giderini bölme, tümünü işletme işaretle — oranı muhasebeci hesaplar) | Resmî kaynak |
| Tekrarlayan işlem | **Kural (Rules) motoru** — en çok 30 kural; tekrarlayan/benzer işlemi otomatik kategorize eder. Sistem zamanla kullanıcının kalıbını öğrenip otomatik eşler | Resmî kaynak |
| Hariç tutma | İşlem **silinmez**, "Exclude" ile hariç tutulur (yinelenen/ilgisiz kayıt) | Resmî kaynak |
| Transfer / hesaplar arası | Yardım makalelerinde işletme/şahsi bağlamında ayrı bir "transfer" modeli belgelenmemiş; QBS çift taraflı muhasebe yapmıyor. **Doğrulanamadı** | Doğrulanamadı |
| Kategori sistemi | Kategoriler **IRS Schedule C (Form 1040)** kalemleriyle hizalı (Advertising, Car and truck, Legal and professional, Utilities…). Kullanıcı en çok 180 kategori ekleyebilir; hesap planı özelleştirmesi yok | Resmî kaynak |
| Veri nereye yazılıyor | Bulut; bağlı banka hesaplarından gerçek zamanlı senkron | Resmî kaynak |
| Muhasebeci tarafı | Solopreneur'da muhasebeci erişimi/dışa aktarma yardım merkezinde net belgelenmemiş; ürün "muhasebecin gerekmeden" konumlanıyor, çıktı vergi yazılımına aktarılıyor. **Doğrulanamadı** | Doğrulanamadı |

**Pipeline şeması (resmî kaynak):**
`banka/kart bağla → işlemler otomatik iner → otomatik kategori + Type önerisi →
kullanıcı "Bank transactions" ekranında gözden geçirir → Type (Business/Personal)
+ kategori düzelt / gerekirse Split → Rules benzerlerini otomatikleştirir →
P&L + nakit akışı + üç aylık tahmini vergi + yıl sonu Schedule C`

## Video/doküman akış yeniden kurulumu

`Resmî kaynak` — yardım merkezi makaleleri. Kullanıcı ürün turu videosu izlerse
buraya zaman damgalı eklenir.

| Akış | Kaynak | Adımlar (yeniden kurulmuş) | BusinessFinance karşılığı |
|---|---|---|---|
| Gider girişi ve sınıflandırma | "Categorize bank transactions in QuickBooks Solopreneur" | All apps → Accounting → Bank transactions → Update → liste → Type sütunundan Business/Personal süz → Category dropdown ile düzelt | Bizde işlem formunda kapsam (Business/Personal) **çipi** + kategori; ama banka bağlama yok, kayıt manuel/fiş önerisi |
| Kısmen işletme gideri | "Split transactions in QuickBooks Solopreneur" | İşlem → Edit → Split transaction → tutar böl → parça başına personal/business + kategori | Bizde bir kaydın tek kapsamı var; bölme yok — kapsam kayıt düzeyinde, kalem düzeyinde değil |
| Tekrarlayan gideri otomatikleştirme | "Use rules to categorize bank transactions" | Accounting → Rules → New rule → koşul + kategori + Type | Bizde `RecurringTransaction`: tanım rapor üretmez, onayla `realize`; QBS kuralı banka feed'ini etiketliyor, bizimki kayıt üretiyor |
| Üç aylık tahmini vergi | Solopreneur ürün sayfası | Kategorize edilmiş işlemlerden tahmini vergi hesaplanır, ödeme hatırlatması | **Bizde yok ve olmayacak** — uygulama vergi hesaplamaz (ADR 0016); yalnız tutarsız takvim önerisi döndürür |
| Fatura kesme | Solopreneur ürün sayfası | Müşteri seç → kalem → ödeme yöntemleri (kart/ACH/PayPal/Venmo) → gönder + otomatik hatırlatma | **Bizde e-belge/fatura kapsam dışı** (PRD); `Obligation`/`CounterpartyCharge` tanıma modeli var |

## Arayüz incelemesi (onboarding, sınırlı)

| Başlık | Kısa gözlem |
|---|---|
| Onboarding tonu | 3 kısa slayt + tek alanlı "basic info" — düşük sürtünme; ama hemen ardından **sert ödeme duvarı** |
| Bilgi hiyerarşisi (resmî kaynak) | İşlem listesi merkezli; "Type" ve "Category" iki ayrı sütun — ayrım ile sınıflandırma görsel olarak ayrılmış |
| Form alanları | "No business name? Use your name" — şahıs şirketi gerçeğini kabul eden mikro kopya (bizim `HasBusiness` tek sorusuyla aynı ruh) |

## Akış özeti

- En kısa ve güçlü akış: Banka bağla → işlemler otomatik iner ve otomatik
  kategorize olur → kullanıcı sadece düzeltir. Sıfırdan giriş yok
- En fazla sürtünme yaratan akış: Onboarding'in hemen ardından ücretli plan
  duvarı; ürünü denemeden karar verilemiyor
- Uygulamanın hedef kullanıcı varsayımı: ABD'de Schedule C dolduran, banka
  hesabını bağlamaya istekli, tek işi olan girişimci
- İşletme ve şahsi para yaklaşımı: **Tek işlem akışı, işlem başına Business/Personal
  etiketi.** Kavramsal olarak ADR 0013'e en yakın rakip — ama ekseni vergi
- Transfer ve kart ödemesi yaklaşımı: Belgelenmemiş; çift taraflı muhasebe yok
- Planlama, borç ve tahsilat yaklaşımı: Borç/alacak modülü yok; "planlama" =
  tahmini vergi + hedefler (goals)

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşlem başına tek "Type: Business/Personal" alanı, üçüncü değer yok | Doğrudan al (zaten böyle) | ADR 0013 ile birebir: `TransactionScope` = Business/Personal, "bilinmiyor" yok. Güçlü doğrulayıcı referans | Kapsam boyutu |
| "Type" ve "Category" ayrı sütunlar / ayrı alanlar | Doğrudan al (zaten böyle) | Bizde de kapsam kategoriyle temsil edilmez, ayrı boyut. QBS bunu görsel olarak da ayırmış | İşlem formu, feed filtresi |
| Şahsi işaretlenen işlem işletme/vergi raporundan düşer ama **silinmez** | Uyarlayarak al | Bizde de kapsam filtresi raporu böler; ama bizde şahsi kayıt **net varlıktan düşmez** (ADR 0013: kapsam parayı bölmez) — QBS'de şahsi tamamen "kayıp" | Rapor, özet hero |
| Bölünmüş işlem (kalem başına işletme/şahsi) | Henüz karar verme | Bizde kapsam kayıt düzeyinde; kalem düzeyinde bölme karmaşıklık getirir. Talebin kanıtı olarak not | İşlem formu |
| Banka bağlantısıyla otomatik indirme + otomatik kategori + kural motoru | Alma | Banka bağlantısı **kesin kapsam dışı** (açık bankacılık yasak). Kayıt bizde manuel + fiş önerisi | — |
| Schedule C / tahmini vergi / beyan çıktısı | Alma | Uygulama vergi hesaplamaz, beyanname üretmez, "kâr" demez (kritik kısıt + ADR 0016). Nakit esaslı işletme neti kullanılır | — |
| "No business name? Use your name" mikro kopyası | Uyarlayarak al | Şahıs şirketi gerçeğini kabul eden dil; bizim onboarding tek sorusuyla uyumlu | Onboarding |
| Çift taraflı muhasebe / bilanço / hesap planı yokluğu (bilinçli sadelik) | Doğrudan al (aynı felsefe) | Biz de muhasebe uygulaması değiliz; "hafif, tek kişilik, vergi-hazır" konumu bizim "işletme finansı takip" konumumuza yakın | — |

## Kanıt ve güven düzeyi

- Manuel gözlem: Yalnız onboarding'in ilk 4 ekranı (`kanitlar/quickbooks/01`–`04`)
- Resmî kaynak: Intuit yardım merkezi — "Introduction to QuickBooks Solopreneur",
  "Categorize bank transactions", "Use rules to categorize", "Split transactions",
  "Schedule C expense categories", Solopreneur ürün + business/personal sayfaları
  (9 Eyl 2026)
- Yorum: "ADR 0013'e en yakın rakip" — QBS'nin işlem başına Business/Personal
  etiketi ile bizim kapsam boyutumuzun aynı fikir olmasından; eksen farkı (vergi
  vs. raporlama) vurgulandı
- Doğrulanamadı: Transfer/kart ödemesi modeli, muhasebeci erişimi, canlı ürün
  davranışı ve ekran tasarımının tamamı

## Tek cümlelik sonuç

QuickBooks Solopreneur, işletme ve şahsi parayı tek işlem akışında tutup her
işleme Business/Personal etiketi veren yönüyle ADR 0013'ün kavramsal en yakın
rakibi; ama ayrımı ABD Schedule C vergi beyanına hizalı olduğu için "kapsam bir
raporlama boyutudur, parayı bölmez" tezimizden ayrışıyor ve banka bağlantısı +
vergi hesaplama gibi bizde kesin kapsam dışı olan mekanizmalara dayanıyor.
