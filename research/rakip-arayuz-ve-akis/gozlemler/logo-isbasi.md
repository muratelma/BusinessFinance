# Uygulama Gözlem Formu — Logo İşbaşı

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Logo İşbaşı / Logo Yazılım |
| Sürüm | 3.20.0 (Android paketi) |
| Test tarihi | **1 Eyl 2026** (emülatörde giriş/kayıt yüzeyi) · **2 Eyl 2026** (kullanıcı gerçek bilgiyle kayıt oldu + e-postayla gelen tanıtım videosu) · **9 Eyl 2026** (isbasi.com / logo.com.tr) · **12 Eyl 2026** (Faz 7.5 doğrulama turu + kaynak kontrolü) |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | **Arayüz etiketleri İngilizce** (`e-mail`, `password`, `Login`, `Register`, `FORGOT PASSWORD?`, `Company name*`, `Sector*`, `Phone Number*`, `CANCEL`, `I AGREE`), **içerik Türkçe** (sektör listesi, sözleşme metni) — aynı ekranda iki dil |
| Hesap veya plan türü | — (hesap açılamadı) |
| Erişim kısıtı | **Kayıt engeli — iki katmanlı.** (1) Kayıt formu telefon + SMS doğrulaması istiyor; sahte numara **sessizce** reddediliyor (koşum notu; ret anı karede yok). (2) 2 Eyl 2026: kullanıcı gerçek bilgiyle kayıt oldu, ardından *"Hesabınız firmanıza özel hazırlanmaktadır, işlemler tamamlandığında e-posta ile bilgilendirileceksiniz"* pop-up'ı çıktı — hesap satış/hazırlık sürecine girdi, uygulamaya giriş açılmadı (kullanıcı aktarımı; pop-up karesi yok) |
| İnceleme türü | **Masa başı.** Manuel gözlem yalnız giriş/kayıt yüzeyine ait (`01-giris-ekrani.png`, `02-kayit-formu.png`, `03-sektor-listesi.png`, `04-sozlesme.png`); ürünün içi tümüyle `Resmî kaynak` |

### Kanıt tavanı (bu formun sınırı)

Uygulamanın **giriş öncesi yüzeyi sürüldü** (giriş ekranı, kayıt formu, sektör
listesi, sözleşme akışı) — bu dört kare gerçek manuel gözlemdir. **Giriş
sonrası hiçbir ekran görülmedi.** Ürünün içine dair her cümle isbasi.com /
logo.com.tr kaynaklarına veya ~1 dk'lık tanıtım videosunun iki karesine
dayanıyor ve **hiçbir davranış doğrulanmadı.**

Bu asimetri bilinçli: Paraşüt, KolayBi ve Logo İşbaşı aynı kulvarda (Türk ön
muhasebe) ve özellik yüzeyleri büyük ölçüde örtüşüyor; **derin inceleme için
KolayBi seçildi** (kullanıcı kararı). Logo bu derinliğe kasten çıkarılmadı,
yalnız metni kaynakla tutarlı hâle getirildi. Belge 1/2'de bu fark açıkça
yazılır.

**Kayıt kapısı kullanıcı tarafından aşılabilir** (`README.md` → "Kullanıcının
yapacağı: kayıt / SMS / VKN kapılarını geçmek"). Hesap açılırsa Logo
sürülebilir hâle gelir; şu an öyle bir ihtiyaç yok.

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | **Firma bilgisi tanımlamadan cep telefonundan fatura kesmeye** başlatan, e-Fatura + ön muhasebe + stok + cari birleşik bulut programı. "Firma bilgisi tanımlamadan" ürünün beyanıdır; kayıt formunda `Company name*` zorunlu (`02-kayit-formu.png`), vergi profilinin sonradan istenip istenmediği bilinmiyor |
| Asıl hedef kullanıcı | Mikro işletme, tek kişilik girişimci, esnaf, serbest meslek. Kayıt sektör listesinin görünen kısmında **`Kurye`** ve **`Öğrenci`** seçenekleri var (`03-sektor-listesi.png`; liste kaydırılınca devam ediyor). Seçeneklerin varlığı konumlandırma gözlemidir; bu kitlelerin ürünü gerçekten kullandığı veya talep ettiği kanıtlanmaz *(P1-B08)* |
| Çözdüğü ana iş | e-Fatura/e-Arşiv kesme + gelir-gider + cari + kasa-banka + stok + çek-senet; hepsi tek panelde |
| Açıkça kapsam dışı bıraktığı | Kişisel/gündelik bütçe, yatırım takibi; bordro sınırlı |
| İş modeli | Ücretli abonelik, 3 paket; kayıt sonrası **satış/hazırlık süreci** — uygulamaya anında giriş açılmıyor |
| BusinessFinance ile aynı kulvarda mı | **Kısmen.** Gelir-gider + cari + fiş okuma + tekrarlayan ortak; ama Logo İşbaşı **e-belge + firma defteri + hafif ERP** (stok, sipariş). İşletme/şahsi tek havuz kavramı yok |

## Görev gözlemleri

| Görev | Sonuç | Not | Kanıt |
|---|---|---|---|
| K00 Giriş ekranı | Tamamlandı (giriş öncesi) | `e-mail` + `password` + **`Login`** + `or` ayracı + **Google ve Apple SSO** + `Register` + `FORGOT PASSWORD?`. Bulut/gökyüzü arka planı, Logo kırmızısı, **çay bardağı ikonu** (yerel dokunuş) | `01-giris-ekrani.png` |
| K00 Kayıt formu | Tamamlandı (giriş öncesi) | **Yalnız 3 alan**: `Company name*`, `Sector*` (açılır liste, varsayılan `Select`), `Phone Number*`; altında tek onay kutusu *"I agree to terms of service and privacy policy."* (kutu karede işaretli) ve `Register`. **VKN/TCKN sorulmuyor** — ön muhasebe ürünleri içinde dikkat çekici bir düşük eşik | `02-kayit-formu.png` |
| K00 Sektör listesi | Tamamlandı (giriş öncesi) | `Select` · `Ticaret ve Perakende` · `İmalat ve Üretim` · **`Kurye`** · **`Öğrenci`** · `İnşaat ve Taahhüt` · `Hizmet Sektörü` · `Sağlık ve Medikal` · `Bilişim ve Teknoloji` · `Eğitim ve Danışmanlık` · `Otelcilik, Restoran ve Kafe` — kaydırma çubuğu listenin devam ettiğini gösteriyor, tamamı çekilmedi | `03-sektor-listesi.png` |
| K00 Sözleşme | Tamamlandı (giriş öncesi) | `Register` → **"LOGO BULUT HİZMETLERİ ÇERÇEVE SÖZLEŞMESİ"** tam metni ekranda (1. Giriş, 2. Onay ve Bağlayıcılık, 3. Konu, 4. Tanımlar…) + `CANCEL` / `I AGREE` | `04-sozlesme.png` |
| K00 (sahte numara denemesi) | Engelli | `Deniz Tasarim` / `Hizmet Sektörü` / sahte numara → sözleşme → `I AGREE` → ~10 sn spinner → **sessizce giriş ekranına dönüyor**. Hata mesajı yok, hesap oluşmuyor | `04-sozlesme.png` yalnız sözleşme adımını gösterir; spinner, geri dönüş ve hesap oluşmaması koşum notu (`03-sektor-listesi.png`'teki klavye önerisi `Tasarim` deneme adıyla tutarlı) |
| K00 (gerçek kayıt, 2 Eyl) | Engelli | Kullanıcı gerçek telefon + SMS ile kaydoldu; *"hesabınız hazırlanıyor, e-posta ile bilgilendirileceksiniz"* pop-up'ı → giriş açılmadı. Ayrıca e-postayla ~1 dk'lık tanıtım videosu geldi | — (`Doğrulanamadı`) |
| K01–K08 | **Engelli** | Doğrulanmış hesap olmadan devam edilemiyor | — (`Doğrulanamadı`) |

## Arayüz incelemesi (giriş öncesi, sınırlı)

| Başlık | Kısa gözlem |
|---|---|
| Giriş ekranı | Bulut/gökyüzü fotoğraf arka planı, Logo kırmızısı vurgu, çay bardağı ikonu. `Login` beyaz zeminli, `Register` dolu kırmızı — ikinci eylem daha baskın |
| Kayıt sürtünmesi | 3 alan az görünüyor ama arkasında SMS doğrulaması, tam sözleşme metni onayı ve kayıt sonrası **insan onaylı hazırlık süreci** var. Yani "3 alan" ekrandaki sürtünme, süreçteki değil |
| Hata geri bildirimi | **Zayıf** — başarısız kayıt hiçbir mesaj vermeden giriş ekranına atıyor (koşum notu; karesi yok) |
| SSO | Google + Apple ile giriş var (Paraşüt'te yoktu) |
| Dil tutarlılığı | Etiketler İngilizce, içerik Türkçe; aynı ekranda `Sector*` ile `Ticaret ve Perakende` yan yana duruyor |

## Resmî kaynak — özellikler

`Resmî kaynak (ürün sayfaları / blog)`, isbasi.com + logo.com.tr, 2 ve 9 Eyl
2026; **12 Eyl 2026'da sesli fatura ve Müşavir Portal kontrol edildi.**

**Not:** aşağıdaki "kapsam dışı" ibareleri bir **öneri kararıdır** (Belge 3),
gözlem değil. Her özellik Belge 1/2'de tarafsız anlatılır.

| Grup | Özellik | BusinessFinance açısından |
|---|---|---|
| Fatura/e-belge | Online fatura kesme, e-Fatura, e-Arşiv, e-SMM, e-İrsaliye; **sesli komutla fatura** (aşağıda ayrı başlık) | Bizde e-belge kesme yok; `Obligation` / `CounterpartyCharge` tanıma modeli var |
| Finans takibi | Gelir-gider takibi, cari hesap takibi, tahsilat ve ödeme takibi, nakit (kasa-banka) yönetimi, çek giriş/çıkış | Cari + tahsilat/ödeme bizde ADR 0014 ile var; çek takibi yok |
| Operasyon | Stok takibi (otomatik güncelleme), teklif ve sipariş oluşturma, özet raporlar | Biz finans uygulamasıyız, ERP değil |
| Entegrasyon | **8 entegrasyon** — aşağıdaki kare tablosunda tam listesi | Banka bağlantısı bizde kesin kapsam dışı; fiş okuma ve POS bizde var |
| Muhasebeci | **Mali Müşavir Paneli / Müşavir Portal** (aşağıda ayrı başlık) | Bizde tek yönlü dosya paketi (Aşama 05 Grup 5) |
| Hedef kitle | Mikro işletme: tek kişilik girişimci ve küçük ekipler | Bizimkiyle aynı segment (şahıs şirketi, esnaf) |
| Fiyat | 3 paket; kayıttan sonra satış/hazırlık süreci | Ücretli ürün. **Güncel rakamlar bu turda doğrulanmadı**, önceki formda yazılı tutarlar çıkarıldı |

### Sesli komutla fatura kesme (12 Eyl 2026'da kaynakla doğrulandı, çalışırken görülmedi — özgün bulgu)

İncelenen dokuz uygulamanın hiçbirinde benzeri görülmeyen tek özellik.

- Logo'nun kendi duyurusuna göre **"Türkiye'de bir ilk"**: fatura bilgileri
  butonlarla değil **sesle** sisteme kaydediliyor; birkaç kelimeyle fatura
  hazırlanıp düzenlenebiliyor.
- Önce **Android**'de yayınlandı, iOS "yakın dönem" olarak duyuruldu.
- Kaynak: logo.com.tr blog — *"Türkiye'de bir ilk: Logo İşbaşı ile sesli
  fatura kesilebilecek!"* `Resmî kaynak (blog/duyuru)`. **Çalışırken
  görülmedi.**

**Ne kazandırıyor:** elleri dolu, hareket hâlinde veya ekrana bakamayacak
durumdaki esnaf (ör. kurye, tezgâh başındaki satıcı — varsayımsal senaryo;
kayıt formundaki `Kurye` seçeneği bu kitlenin ürünü kullandığını göstermez)
kaydı anında, form doldurmadan yapabilir;
"sonra girerim" kaynaklı kayıp azalıyor.
**Ne kaybettiriyor:** finansal bir kaydın doğruluğu konuşma tanımaya bağlanıyor
— yanlış duyulan bir rakam sessizce yanlış tutar yazar ve kullanıcı ekrana
bakmadığı için fark etmesi zorlaşır. Doğrulama adımı olmadan bu, ADR 0011'in
fiş okuma için kurduğu "model önerir, kullanıcı onaylar" ilkesinin tersi bir
yönde ilerler.

### Mali Müşavir Paneli / Müşavir Portal (12 Eyl 2026'da keskinleştirildi)

Video karesi (`06-video-musavir-portal.png`) stilize bir pencere çerçevesi
içinde ayrı bir arayüz gösteriyor (web mi olduğu karede yazmıyor): üstte
**`MÜŞAVİR PORTAL`** başlığı ve fotoğraflı profil simgesi, altında dört satırlık
liste ve **her satırda kalem / liste / çöp kutusu ikonları** (düzenle / detay /
sil anlamı ikon biçiminden çıkarım). Kare üzerindeki
metin: *"Alış-Satış Faturalarınızı, Gider Fişlerinizi Muhasebecinize Kolayca
Aktarın"*.

**Karedeki satırlar boş şablon çubukları** (gerçek veri yok) ve her satırın
solunda **farklı birer logo simgesi** (çiçek, şef şapkası, daire, kamyon)
duruyor; bunların firma logosu olduğu da çıkarımdır. Bu yüzden iki okuma mümkün:
liste ya tek müşterinin fatura/fiş satırlarıdır, ya da **müşavirin müşteri
firmaları listesidir**. Kareden ayırt edilemiyor — `çıkarım`.

Resmî ürün sayfası ikinci okumayı destekliyor ve mekanizmayı açıklıyor:
müşavir, müşterilerinin verisini görüntüleyip erişebiliyor; **müşteri onu
mali müşavir olarak ekledikten sonra müşavir müşterinin adına işlem
yapabiliyor** ve başka müşterilerini de ürüne davet edebiliyor.
`Resmî kaynak (ürün sayfası)`

**Ne kazandırıyor:** ay sonu dosya alışverişi tamamen kalkıyor; müşavir veriyi
anlık görüyor, eksik belgeyi kendisi tamamlayabiliyor ve tek panelden birden
çok müşteriyi yürütüyor — muhasebecinin gerçek çalışma biçimine oturuyor.
**Ne kaybettiriyor:** "müşteri adına işlem yapma" yetkisi, verinin sahibi ile
onu değiştirebilen kişiyi ayırıyor; kaydı kimin yazdığı ve kimin sildiği
sorusu bir yetki/iz katmanı gerektiriyor. Bizim `ICurrentUser` + owner-scoped
izolasyon modelimizde böyle bir delegasyon yolu **yok** ve eklemek mimari bir
karar olur. Aşama 05 Grup 5 bunun yerine tek yönlü dosya paketini seçti: çok
daha az güç, çok daha küçük güven sınırı.

## Tanıtım videosu kareleri

**Kaynak:** kullanıcıya 2 Eyl 2026'da e-postayla gelen ~1 dk'lık tanıtım
videosu; 2 kare teslim edildi. `05-video-entegrasyonlar.png`'in üst kenarında kesik bir oynatıcı başlığı
ve altta ilerleme çubuğu görünüyor; kareler bir video oynatıcıdan alınmış, tam
URL ve yayın tarihi yok. Etiket: `Resmî kaynak (tanıtım videosu)`.
**Kareler pazarlama grafiğidir** — gerçek ürün ekranı değil, ikon ve şablon
çubuklarından oluşan stilize görseller.

| # | Kare | Ekranda görünen | Not |
|---|---|---|---|
| 05 | `05-video-entegrasyonlar.png` | Başlık *"Entegrasyonlarımız İle Verimliliğinizi Artırın"*; 8 kutucuk: **`Pazaryeri ve e-Ticaret`** · **`GİB e-Arşiv Portal`** · **`Banka Hesap Hareketleri`** · **`Akıllı Fiş Okuma`** · **`Online Tahsilat`** · **`Müşavir Portal`** · **`İşbaşı POS`** · **`Kargo Yönetimi`** | Entegrasyonların **adları** gözlem; her birinin ne yaptığı ve nasıl çalıştığı bu kareden okunamaz |
| 06 | `06-video-musavir-portal.png` | `MÜŞAVİR PORTAL` başlıklı stilize arayüz; dört satırlık liste, satır başına kalem/liste/çöp kutusu ikonları; satırlar boş şablon, her birinde farklı logo simgesi | Yukarıdaki "Mali Müşavir Paneli" başlığında işlendi |

| Entegrasyon | BusinessFinance açısından |
|---|---|
| `Pazaryeri ve e-Ticaret` | Kapsam dışı |
| `GİB e-Arşiv Portal` | Kapsam dışı (e-belge kesmiyoruz) |
| `Banka Hesap Hareketleri` | **Kesin kapsam dışı** — banka bağlantısı `PROJECT-ROADMAP`'te yasak |
| `Akıllı Fiş Okuma` | Bizde ADR 0011 öneri katmanı var; onlar "entegrasyon" diyor |
| `Online Tahsilat` | Ödeme başlatma bizde hiçbir koşulda yok |
| `Müşavir Portal` | Muhasebeci paketi (Aşama 05) ile aynı ihtiyaç, farklı çözüm |
| `İşbaşı POS` | POS tahsilat — bizde `PosSettlement` var (ADR 0015) |
| `Kargo Yönetimi` | Kapsam dışı |

## Sistem işleyişi / pipeline

`Resmî kaynak` — isbasi.com + logo.com.tr blog (9 Eyl 2026). **Canlı ürün
davranışı doğrulanmadı; bu bölümün tamamı ürünün kendi anlatımıdır.**

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Kayıt mekanizması | Gelir ve gider **fatura ve fişlerle** kaydediliyor. Ayrıca **sesli komutla** işlem girişi | Resmî kaynak |
| Kayıt → entegre güncelleme | Bir işlem kaydedilince **cari hesap** (borç/alacak), **kasa-banka bakiyeleri** ve **stok seviyeleri** birlikte güncelleniyor — üç defter tek kayıttan besleniyor | Resmî kaynak |
| Fatura kesme akışı | Satış faturası: müşteri listeden seç → **bakiyesi anında görünür** → ürün seç (veya yeni ürün bilgisi gir) → kaydet. Firma bilgisi tanımlamadan da kesilebiliyor | Resmî kaynak |
| Gider akışı | Gider fişi fotoğrafı → `Akıllı Fiş Okuma` (OCR) → otomatik gider kaydı | Resmî kaynak |
| Cari hesap | Tahsilat, borç-alacak ve ödeme işlemleri cari hesap içinde toplanıyor; ekstre + rapor | Resmî kaynak |
| Tahsilat/ödeme | Nakit tahsilat ve banka bilgisi telefondan eklenebiliyor; cari bakiyeyi kapatır, kasa/banka bakiyesini oynatır | Resmî kaynak |
| Raporlama | Tarih aralığına göre kategorize raporlar + görsel grafik | Resmî kaynak |
| İşletme/şahsi ayrım | **Yok.** Klasik firma defteri; patronun şahsi harcaması ancak dolaylı (ortak cari / çekilen para) girebilir | `Çıkarım` (kaynakta böyle bir kullanım anlatılmıyor; cari modelinden türetildi) |
| Muhasebeci tarafı | `Müşavir Portal` — müşavir müşterinin verisine erişiyor ve **müşteri adına işlem yapabiliyor** | Resmî kaynak (ürün sayfası) |
| Veri nereye yazılıyor | Bulut (web + mobil), "her alandan erişim" | Resmî kaynak |

**Pipeline şeması (satış → tahsilat):**
`müşteri seç (bakiye görünür) → ürün/kalem seç → fatura kaydet
(e-Fatura/e-Arşiv) → cari borç +tutar, stok −adet → Tahsilat Ekle
(nakit/banka) → cari kapanır, kasa/banka +tutar → rapor + grafik →
(Müşavir Portal muhasebeciye yansır)`

**Pipeline şeması (gider):**
`fiş fotoğrafı → Akıllı Fiş Okuma (OCR) → gider kaydı → gider raporu +tutar,
tedarikçi cari +tutar → Ödeme Ekle → kasa/banka −tutar`

## BusinessFinance için kararlar

> `alma` ve `kararı yeniden sor` satırlarında "ne kazandırıyor / ne
> kaybettiriyor" zorunludur (`MANUEL-TEST-PROTOKOLU.md` → yazım kuralı 6).
> Sonuç tanımları: `README.md` → "Karar sonuçları — tek kaynak".
>
> **Bu tablo büyük ölçüde masa başı kaynaklara dayanıyor**; yalnız kayıt
> yüzeyine ait satırlar canlı gözlemdir. Aynı kulvardaki derin inceleme
> KolayBi'de yapıldı, bu tablo onunla birlikte okunmalıdır.

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| Kayıtta yalnız 3 alan (`Company name`, `Sector`, `Phone Number`), **VKN/TCKN sorulmuyor** | Uyarlayarak al | Düşük eşikli kayıt, mikro işletmeyi ürüne sokmanın en kısa yolu; bizde de onboarding tek soruya indirgenmiş (`HasBusiness`). Sektör sormak bize gerekmiyor — kapsam boyutumuz sektöre değil, kaydın kendisine bağlı | Kayıt / onboarding |
| Ekrandaki 3 alanın arkasında SMS + tam sözleşme + **insan onaylı hazırlık süreci** olması | Alma | **Kazandırdığı:** sahte kayıt engelleniyor, ücretli ürün gerçek işletmeyle eşleşiyor ve satış tarafı devreye girebiliyor. **Kaybettirdiği:** kullanıcı "3 alan" görüp başlıyor, sonra beklemeye düşüyor; ürünü o gün deneyemiyor. Bizde kayıt anında kullanılabilir olmalı | Kayıt / onboarding |
| Başarısız kayıtta **sessiz geri dönüş**, hiç hata mesajı yok | Alma | **Kazandırdığı:** hiçbir şey — muhtemelen numara doğrulamasının başarısızlığını kullanıcıya sızdırmama tercihi. **Kaybettirdiği:** kullanıcı neyin yanlış olduğunu bilmiyor, aynı formu tekrar doldurup yine başarısız oluyor. Bizde her ekranda görünür error durumu zorunlu bir kabul kriteri | Tüm formlar |
| **Sesli komutla fatura kesme** ("Türkiye'de bir ilk") | Henüz karar verme | Hedef kitlemizle örtüşebilecek bir sürtünme noktası hipotezine dokunuyor (elleri dolu esnaf, kurye — ölçülmedi *(P1-B08)*) ve dokuz uygulama içinde tek örnek. Ama **çalışırken görülmedi**; doğruluk/onay akışı bilinmiyor. Bizde karşılığı "kaydı sesle başlat, ekranda onayla" biçiminde olabilir — ADR 0011'in öneri katmanı mantığıyla. Veri yetmiyor | İşlem ekle (gelecek) |
| `Müşavir Portal` — müşavirin müşteri verisine erişip **müşteri adına işlem yapması** | Alma | **Kazandırdığı:** ay sonu dosya alışverişi kalkıyor, müşavir eksik belgeyi kendi tamamlıyor, tek panelden çok müşteri yürütüyor. **Kaybettirdiği:** verinin sahibi ile onu değiştirebilen kişi ayrışıyor; kaydı kimin yazdığı/sildiği bir yetki ve iz katmanı gerektiriyor, `ICurrentUser` + owner-scoped izolasyon modelimizde böyle bir delegasyon yolu yok. İki rakipte (Paraşüt'te de) bulunması ürünlerin bu yolu seçtiğini gösterir; ihtiyacın varlığı veya büyüklüğü kullanıcıyla doğrulanmadı *(P1-B08)*; biz Aşama 05'te tek yönlü dosya paketini seçtik | Muhasebeci paketi |
| Tek kayıttan **cari + kasa/banka + stok** üç defterin birlikte güncellenmesi | Uyarlayarak al | Bizde de tek `SaveChanges` sınırında birden çok okuma modeli besleniyor (`FinancialDataChanges` hedefleri); stok hariç aynı fikir | Mutation → feed yükseltme |
| Banka hesap hareketleri entegrasyonu | Alma | **Kazandırdığı:** hareketler elle girilmiyor, mutabakat kendiliğinden oluşuyor. **Kaybettirdiği:** sağlayıcı bağımlılığı, banka kimlik bilgisi güven sınırı ve düzenleme yükü; `PROJECT-ROADMAP` bunu kapsam dışı bırakıyor | — |
| e-Fatura / e-Arşiv / e-SMM / e-İrsaliye kesme | Alma | **Kazandırdığı:** belgeyi kesen ile kaydı tutan aynı ürün olunca çifte giriş bitiyor. **Kaybettirdiği:** GİB uyum yükü ve sürekli mevzuat takibi — başlı başına ayrı bir ürün. Bizde `CounterpartyCharge` yalnız tanır, belge kesmez | — |
| `Akıllı Fiş Okuma` | Uyarlayarak al | Bizde ADR 0011 zaten bir öneri katmanı tanımlıyor; fark, Logo'nun bunu "otomatik gider kaydı" diye sunması — bizde model yönü ve ödeme kaynağını seçmez | Fiş okuma akışı |
| `İşbaşı POS` | Not | Bizde `PosSettlement` ADR 0015 ile zaten var; kıyas noktası | POS tahsilatları |
| Stok / sipariş / teklif / kargo | Henüz karar verme | Esnafın gerçek ihtiyacı olabilir ama ERP sınırına giriyor; kapsam kararı patronundur | — |
| Sektör listesinde **`Kurye` ve `Öğrenci`** gibi bireysel seçeneklerin bulunması | Not | Rakibin konumlandırmasına dair ekran gözlemi: seçenekler bireysel kullanıcıyı da hedeflediğini düşündürüyor (`çıkarım`). Gerçek kullanıcı dağılımı veya segmentimizle örtüşme kanıtlanmaz *(P1-B08)* | — |

## Kanıt ve güven düzeyi

- **Manuel gözlem (1 Eyl 2026):** giriş ekranı, kayıt formu, sektör listesi,
  sözleşme akışı; sahte numarayla kayıt denendi ve reddedildi (ret anı karede
  yok, koşum notu). 14 Eyl 2026'da altı kare yeniden açıldı (ayrıntı
  `KANIT-ENVANTERI.md` P1-logo-isbasi-G01). Kareler
  `01-giris-ekrani.png`, `02-kayit-formu.png`, `03-sektor-listesi.png`,
  `04-sozlesme.png`
- **Resmî kaynak (tanıtım videosu):** `05-video-entegrasyonlar.png`,
  `06-video-musavir-portal.png` — kullanıcıya e-postayla gelen ~1 dk'lık
  video (2 Eyl 2026). **Pazarlama grafiği**; kutucuk ve başlık adları
  gözlem, çalışma biçimi değil
- **Resmî kaynak (ürün sayfası / blog):** isbasi.com özellik sayfaları,
  `isbasi.com/mali-musavirlere-ozel`; logo.com.tr blog — "kontrollü
  gelir-gider takibi" ve **"Türkiye'de bir ilk: Logo İşbaşı ile sesli fatura
  kesilebilecek!"** (9 ve 12 Eyl 2026)
- **Çıkarım:** patronun şahsi harcamasının ortak cari / çekilen para üzerinden
  gireceği — kaynakta böyle bir kullanım anlatılmıyor, cari modelinden
  türetildi
- **Çıkarım:** `06-video-musavir-portal.png` karesindeki listenin müşavirin **müşteri firmaları** mı
  yoksa tek müşterinin fatura satırları mı olduğu; karede satırlar boş şablon
- **Doğrulanamadı:** giriş sonrası **hiçbir ekran ve hiçbir davranış**; sesli
  fatura özelliğinin gerçekte nasıl çalıştığı ve bir onay adımı olup olmadığı;
  sektör listesinin tamamı (kaydırıldıkça devam ediyor); kayıt formundaki
  telefon alanının otomatik `0090` ön eki eklediği iddiası (önceki formda
  yazılıydı, karede görünmüyor — çıkarıldı); güncel fiyatlandırma (önceki
  formdaki tutarlar bu turda doğrulanmadığı için çıkarıldı)
- **Kasten yapılmadı:** derin inceleme ve kayıt kapısının aşılması. Aynı
  kulvarda KolayBi seçildi

## Tek cümlelik sonuç

Logo İşbaşı, üç alanlık kayıt formu, `Kurye`/`Öğrenci` sektör seçenekleri ve
yerel marka diliyle kendini mikro işletmeye konumlandıran (gerçek kullanıcı
dağılımı ölçülmedi) bir e-Fatura + ön muhasebe ürünü; iki ayırt edici yanı **sesle fatura kesme** (dokuz uygulama
içinde tek örnek) ve müşavirin müşteri adına işlem yapabildiği **Müşavir
Portal** — ama kayıttan sonra hesap insan onaylı bir hazırlık sürecine
girdiği için uygulamanın içi bu çalışmada hiç görülmedi ve buradaki her
davranış cümlesi ürünün kendi anlatımıdır.

---

## Faz 8 — hedefli kaynak taraması (masa başı, 22 Eylül 2026)

Belge 2 Bölüm 9 için açık kalan sorular tarandı. Hepsi `Resmî kaynak`; ürünün
içi yine görülmedi.

### Kaydın resmî değeri yok — kaynağın kendi cümlesi

İşlem Rehberi, Fatura Yönetimi sekmesi: *"İşbaşı üzerinde oluşturduğunuz
**fatura kayıtlarının resmi değeri yoktur** ancak matbu faturanızı kullanarak
çıktı alırsanız faturanızı resmileştirmiş olursunuz."* (`isbasi.com/islem-rehberi`)

`çıkarım`: Üründeki "belge" kendiliğinden resmî belge değil; resmîlik ya
matbu çıktıyla ya da ayrıca satın alınan e-Dönüşüm modülüyle geliyor.
Dayanağı: yukarıdaki cümle + aşağıdaki paket ayrımı.

### Müşavir yetkisi: salt okunur değil, ama sınırı yazılı değil

- Ekleme yolu: *"Ayarlar > Firma Bilgilerim > Kayıtlı Kullanıcılar > Kullanıcı
  Ekle"* ile davet (`isbasi.com/musavir-portal-entegrasyonu`).
- Yetki cümlesi: *"Mükellefiniz kayıtlı kullanıcılar alanında sizi Mali
  Müşavir'i olarak eklediğinde **onun adına işlemleri gerçekleştirebilir**,
  diğer mükelleflerinizi Logo İşbaşı'na davet edebilirsiniz."*
  (`isbasi.com/mali-musavirlere-ozel`)
- Diğer bütün ifadeler okuma/aktarma yönünde: *"tüm verilerine erişir"*,
  *"görüntüleyin"*, *"excele aktar"*.
- Müşavir Portal'in kendi kapsamı ön muhasebe kaydı değil, mükellef yönetimi:
  dosya takibi, beyanname ve tahakkuk bildirimi, KDV/KDV2/Gelir Vergisi/İşkur/
  BA-BS takibi, işe giriş-çıkış ve izin formları. Ücretsiz.

**Arandı, bulunamadı:** yetki seviyesi / rol tanımı (hangi menü açık, silme var
mı) hiçbir resmî İşbaşı sayfasında tarif edilmiyor. Kaydı kimin oluşturduğunu
gösteren bir **iz/log katmanı** da geçmiyor. (Aramada çıkan "Kullanıcı İşlem
Yetkileri" ve "Yetki Kodları" dokümanları Logo'nun **masaüstü ERP** ürünlerine
ait — Tiger/GO/Sistem İşletmeni — İşbaşı'na dair değil, delil sayılmadı.)

### Sesli fatura — iki yeni ayrıntı

İşlem Rehberi, Fatura Yönetimi: *"…aynı zamanda Türkiye'de bir ilk olan sesli
fatura özelliğimiz sayesinde **telefonunuzu sallayarak** vereceğiniz sesli komut
ile de faturalarınızı rahatlıkla oluşturabilirsiniz."*

Duyuru: *"**sistemde kayıtlı hazır fatura şablonları yardımıyla** saniyeler
içinde satış faturası düzenleme imkânı"*; *"ilk etapta Android"*.

Özellik hâlâ yayında: `isbasi.com/mobil` (© 2026) *"Sesli Komutla Fatura Kesin"*
başlığıyla pazarlıyor.

`çıkarım`: Sesin çözmesi gereken alan sayısı hazır şablonla azaltılmış;
"saniyeler içinde" vaadi bununla tutarlı. Dayanağı: duyurunun şablon cümlesi.

**Arandı, bulunamadı:** sesle girilenin kullanıcıya **onaylatıldığı** bir adım
ne duyuruda ne SSS'te geçiyor. Güncel platform kapsamı da çelişkili: duyuru
"ilk etapta Android", SSS "IOS veya Android" diyor.

### Stok tek düzlemde

SSS (`Stok & Hizmet`): `Stok&Hizmet > Ürünler`'de Excel'den veri aktarımı,
`Stok Hareketleri > Yeni > Stok Girişi`, ürün listesinde **`Stok Miktarı`
kolonu**, ürün yanındaki `hareketler` düğmesiyle hareket geçmişi. Ürün
sayfasının başlıkları: Ürün&Stok Kartı Tanımlama, Excel ile Stok Miktarını
Aktarma, Stok Durumunu Her Yerden Görüntüleyin, Stok Hareketlerinin Dökümünü
Alın, Stokların Maliyetini Görüntüleyin.

**Arandı, bulunamadı:** çoklu depo / depolar arası transfer anlatan resmî bir
İşbaşı sayfası yok. Olmadığını söyleyen açık bir cümle de yok — **yokluk
kanıtlanmadı, yalnız bulunamadı.** (`isbasi.com/stok-takibi` sayfasındaki
"depo stok takibi" ifadesi ürünü değil, genel olarak stok programlarını
anlatan tanıtım metninde geçiyor; Logo'nun depo yönetimi ayrı bir ürün.)

### e-Belge ayrı modül

*"İşbaşı üzerinden e-Fatura&e-Arşiv kullanabilmeniz için **bu modülümüzü satın
almanız gerekir.**"* Fiyat listesinde `Ön Muhasebe Paketi` satırının altında
*"e-Fatura özelliği bulunmamaktadır."*; `e-Dönüşüm Paketi` yıllık 1000 kontör
hediye taşıyor, **kullanım süresi 12 ay**. *"Her kontör gelen ve giden
e-faturalar olmak üzere 1 kontöre tekabül etmektedir."* Yönlendirme otomatik:
*"kaydettiğiniz **VKN'ye göre** e-Fatura ya da e-Arşiv Fatura olarak gönderiminiz
otomatik gerçekleştirilir."*
