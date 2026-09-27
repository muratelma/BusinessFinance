# Manuel Rakip Uygulama Test Protokolü

**15 Eylül son durum: Faz 8 kullanıcı onayıyla açıldı. P5-P pilot ve teknik dışa aktarım hazır; kullanıcı pilot değerlendirmesi bekleniyor.**
365/365 görsel incelendi ve envanterde (357 başlangıç + GB-U01 3, WL-U 3, BC-U01 2). B09 WL-U01 ile kapandı: Wallet'ın borç modeli ADR 0014'e eşdeğer değil.
B01–B19: 14 kanıtla kapalı, 5 kapsamı sınırlı (B06, B08, B10, B11, B12), 0 açık. P4-tema-01–10 kapandı. f7-54 hesap sahibinin adını gösterir: teslimde karartılmalı.
Ölçülmeyen davranışlar (Wallet Postpone/Dismiss sonucu, Bluecoins otomatik kol ve liste yenilenme kök nedeni, Hesap Defterim e-posta alıcısı/teslimi) doğrulanmış sayılmaz.
Mekanik kapı: dokuz form 0 hata/0 uyarı, 21/21 test. Geçiş kapısı 12/12; 15 Eylül kullanıcı onayı kaydedildi. Pilot değerlendirilmeden tam raporlar yazılmaz.
Sıradaki tek iş pilot değerlendirmesi. Teslim: raporlar/pilot/pilot-islem-ekleme.pdf ve .docx.
Aşağıdaki eski paket devirleri tarihseldir; güncel ayrıntı BULGU-DOGRULAMA-KAYDI.md sonundadır.


14 Eylül kullanıcı kararı: mevcut koşumlar korunur; gerekli ek emülatör
kontrollerini agentın uygulama başına hazırladığı listeyle kullanıcı yapar.
Agent otomatik canlı test başlatmaz. Güncel sınır ve kapanış kuralları
[mevcut plandadır](FAZ7-8-UYGULAMA-PLANI.md).
Goodbudget GB-U01 kullanıcı ekran kontrolüyle kapandı; P1-kolaybi-G01/G02, P1-B08, P1-B15, P1-hesap-defterim-G01/G02, P1-B11, P1-B12, P1-money-manager-G01, P1-parasut-G01, P1-logo-isbasi-G01, P1-quickbooks-G01, P1-B13, P1-B18 ve P1-K de kapandı, sonraki paket P2-G01.


## Amaç

Rakip uygulamaları aynı görevler ve aynı sentetik verilerle karşılaştırmak;
arayüz beğenisini ürün davranışından ayırmak ve **BusinessFinance'in tamamı
için** kanıt üretmek — arayüz, özellikler, akışlar ve pipeline'lar
(`README.md` → "Belge durumu"). Bu bir QA kabul testi **değildir**: rakibin
"doğru" davranmasını beklemeyiz, gözlenen davranışı kaydederiz. Rakibin bizden
farklı davranması bir hata değil, başka bir tasarım kararıdır.

## İncelenen uygulamalar

**Tur 1 çekirdek 7 uygulama** (`DURUM.md` Tur 1 tablosu):

| # | Uygulama | Paket | Erişim |
|---|---|---|---|
| 1 | Money Manager (Realbyte) | `com.realbyteapps.moneymanagerfree` | Sürülebilir |
| 2 | Paraşüt | `com.parasut` | Masa başı (kayıt web + ücretli) |
| 3 | Logo İşbaşı | `com.isbasi` | Masa başı (Müşavir Portal) |
| 4 | KolayBi | `com.kolaybi.mobil` | Masa başı |
| 5 | QuickBooks Solopreneur (kareler: QuickBooks mobil onboarding + QBO Simple Start plan ekranı) | `com.intuit.quickbooks` | Masa başı (ödeme + bölge) |
| 6 | Wallet by BudgetBakers | `com.droid4you.application.wallet` | Sürülebilir |
| 7 | Bluecoins | `com.rammigsoftware.bluecoins` | Sürülebilir |

**"Bize benzemeyen" 3. Tur 2 slotu için iki aday** (10 Eyl 2026 eklendi; Faz 4'te
ikisine de tam Tur 1 koşumu yapılır, Faz 5'te biri seçilir):

| # | Uygulama | Paket | Tür | Tur 1 durumu |
|---|---|---|---|---|
| 8 | Hesap Defterim (Cash Book / Ankit Saraf) | `cashbook.cashbook` | Esnaf kasa/veresiye defteri, TR yerelleşmiş, kayıt yok | **Tamamlandı (10 Eyl)** — `gozlemler/hesap-defterim.md` |
| 9 | Goodbudget | `com.dayspringtech.envelopes` | Dijital zarf bütçe, household hesabı gerekli | **Tamamlandı (11 Eyl)** — `gozlemler/goodbudget.md` |

Kendi uygulamalarımız (rakip değil, karşılaştırma zemini): `com.nef.business_finance_mobile`,
`com.nef.personal_budget_mobile`.

## Test öncesi sabitler

- Cihaz: Aynı telefon veya emülatör
- Görünüm: Açık tema, varsayılan yazı boyutu ve aynı ekran ölçeği
- Dil/para birimi: Mümkünse Türkçe ve TRY
- Veri: Yalnız `SENTETIK-TEST-VERISI.md` içindeki kurgu veriler
- Güvenlik: Gerçek ad, finansal bilgi, banka bağlantısı, VKN/TCKN veya belge yok
- Sürüm kaydı: Uygulama adı, geliştirici, sürüm ve test tarihi forma yazılır

Bir uygulama geçerli şirket/vergi bilgisi, banka bağlantısı veya ücretli paket
istiyorsa bu adım aşılmaz. Sonuç `Engelli`, `Ücretli` veya `Desteklenmiyor`
olarak kaydedilir; özellik varmış gibi varsayılmaz.

## İki turlu yöntem

### Tur 1 — bütün uygulamalarda hızlı karşılaştırma

Hedef süre uygulama başına 20–30 dakikadır.

| Kimlik | Görev | Kaydedilecek ana soru |
|---|---|---|
| K00 | İlk açılış ve kayıt | Ürün kimi hedefliyor, başlamak için ne istiyor? |
| K01 | Ana ekranı incele | İlk bakışta hangi bilgi ve birincil eylem öne çıkıyor? |
| K02 | Hesap/cüzdan oluştur | Nakit, banka ve kart kavramlarını nasıl ayırıyor? |
| K03 | İşletme geliri ekle | Kaç adım sürüyor, varsayılanlar doğru mu? |
| K04 | Şahsi gideri aynı havuza ekle | İşletme/şahsi ayrımı var mı; yoksa nasıl davranıyor? |
| K05 | İşletme kart gideri ekle | Hesap, kategori, kapsam ve belge ilişkisi nasıl kuruluyor? |
| K06 | Hesaplar arası transfer ekle | Transfer gelir/giderden ayrılıyor mu? |
| K07 | Liste, detay ve aylık raporu incele | Kayıtlar bulunabiliyor, filtrelenebiliyor ve anlaşılabiliyor mu? |
| K08 | Bir kaydı düzelt veya iptal et | Geri bildirim, hata önleme ve geri alma nasıl? |

**Sürülen uygulamalarda K08'den sonra ek koşum — A / B / B1 / B2** (10 Eyl 2026'dan
itibaren standart; tutarlar `SENTETIK-TEST-VERISI.md` "Ek koşum olayları"):

| Kimlik | Görev | Kaydedilecek ana soru |
|---|---|---|
| A | Kısmi kredi kartı ödemesi (₺400) | Kart ödemesinde tutar serbestçe düşürülebiliyor mu; kısmi ödeme borca/ekstreye nasıl yansıyor |
| B | Fiş / kamera | OCR mu (tutar/tarih/satıcı okur) yoksa sadece dosya eki mi |
| B1 | Tekrarlayan gider (₺600/ay, ilk çekim 10 Ağu) | Tanım nasıl kuruluyor; ileri aylara otomatik mi / onaylı mı düşüyor; pasifleştirme/silme |
| B2 | Taksitli kart harcaması (₺6.000 = 6×₺1.000, ilk 15 Ağu) | Taksit planı nasıl kuruluyor; Ağustos ekstresine kaç TL; kalan taksitler nasıl |

**Test verisi silme kuralı** `SENTETIK-TEST-VERISI.md` → "Ek koşum olayları"
bölümündedir (tek kaynak): kayıtlar silinmez, Tur 2'ye seçilse bile
sıfırlanmaz, yalnız kullanıcı açıkça isterse temizlenir.

İçine girilemeyen (resmî kaynak) uygulamalarda A/B/B1/B2 canlı koşulmaz; varsa
resmî destek merkezi, yardım makalesi veya videodan `Resmî kaynak` etiketiyle
not edilir.

Bir özellik yoksa benzerini zorlayarak üretme. Örneğin işletme/şahsi ayrımı
yoksa kategoriyle taklit etme; doğrudan `Desteklenmiyor` yaz. Bu yokluğun
kendisi araştırma bulgusudur (B1/B2 için de geçerli).

**K00–K08'den sonra ~10 dk'lık _hızlı arayüz gezintisi_ yapılır** (görevlerden
bağımsız; Tur 2'nin çok daha uzun **"tam arayüz taraması"**ndan farklıdır —
aşağıda konu 6):
görev dışı kalan tüm ekranlar bir kez açılır — diğer sekmeler, rapor drill-down,
bütçe/planlama, ayar derinliği, arama/filtre, boş ve hata durumları. Bu adım
Belge 1'in (arayüz) genişliğini besler; K-görevleri yalnız akışları
(Belge 2) besler. Kontrol listesi `UYGULAMA-GOZLEM-SABLONU.md` içindedir.

**Görevler sırasında sistem işleyişi de not edilir.** Her K-görevinde ekranın
ne gösterdiğinin yanında kaydın arka planda ne ürettiği (bakiye, rapor, başka
kayıt), akışların birbirine bağlanışı ve varsa entegrasyon temas noktaları
gözlem formundaki **"Sistem işleyişi / pipeline"** bölümüne yazılır. İçine
girilemeyen uygulamalarda bu bölüm ve **"Video/doküman akış yeniden kurulumu"**
yardım merkezi + ürün turu + kullanıcının video notlarından `Resmî kaynak`
etiketiyle doldurulur.

### Ara tur — boşluk koşumu (10 Eyl 2026 eklendi)

Tur 1 ile Tur 2 arasında bir ara adım: sürülebilen üç uygulamada (Money Manager,
Wallet, Bluecoins) formun **kendi "eksik kanıt" listesinden** gidilir. Amaç Tur 2
seçimini kör yapmamak. **Orta derinlik** — Tur 2 kadar ayrıntılı değil.
(Yeni sürülen uygulamalarda A/B/B1/B2 zaten Tur 1'in parçası — yukarı bakın;
boşluk koşumu bu üç uygulamaya özgü, çünkü Tur 1'leri bu adım eklenmeden yapıldı.)

- **Her uygulamada canlı kurulur ve davranışı gözlemlenir:** çekirdek 5 işlem
  (doğru Ağustos tarihleriyle), **kredi kartı harcaması + ödemesi**,
  **kısmi kart ödemesi** (A), **tekrarlayan işlem** (B1), **taksit planı** (B2),
  **fiş/kamera** (B) — hepsi `SENTETIK-TEST-VERISI.md` "Ek koşum olayları".
  Bunlar sadece "var" denmez, gerçek kayıt oluşturulup üretim/ekstre davranışı
  izlenir; özellik yoksa `Desteklenmiyor`.
- Planlama / borç / hatırlatıcı gibi formun kendi eksik kalemleri koşulur;
  K00–K08 baştan tekrarlanmaz.
- **Kapsam dışı:** export, yedek/geri yükleme, bütçe kurulum ekranı — seçim-kritik
  değil; gerekirse Tur 2'de veya belge yazımında bakılır.
- Money Manager istisnası: Tur 1'de işlemler yanlış tarihe girildiği için
  çekirdek işlemler Ağustos 2026 tarihleriyle yeniden girildi (10 Eyl, tamam).
- Eksik ekran görüntüleri tamamlanır; form + varsa kontrol değerleri güncellenir.

Ayrıntılı faz planı: `TUR2-YOL-HARITASI.md`.

### Tur 2 — seçilen üç uygulamada derin akış

Boşluk koşumu + yeni uygulama (Goodbudget) testinden sonra üç uygulama seçilir.
Yalnız bu üçünde şunlar denenir:

1. Kredi kartı borcu ve kart ödemesi
2. Planlanan veya tekrarlayan ödeme
3. Fatura/borç oluşturma ve ödeme/tahsilat bağlantısı
4. Kısmi tahsilat veya kısmi ödeme
5. Arama, filtre, dışa aktarma ve hata/boş durumları
6. **Tam arayüz taraması** (11 Eyl 2026 eklendi) — ana sekmelerden erişilebilen
   **her ekran ve alt menü** en az bir kez açılır, ne gösterdiği kısaca not
   edilir: tüm Ayarlar alt sayfaları, her rapor/grafik türü (tek örnek değil,
   hepsi), her formun "gelişmiş/more options" bölümü, her farklı **kayıt
   türünün** (aynı türden değil, her türden bir örnek) detay sayfası, widget/
   entegrasyon ekranları (denenmese de açılıp içeriği not edilir). Amaç, sabit
   K-görevleri dışında kalan köşelerde saklı özellikleri keşfetmek — Bluecoins'in
   ekstre kesim günü ve bağımsız hatırlatıcı bulguları tam olarak böyle çıktı.

   **Dahil/hariç ilkesi:** *"Bu ekran uygulamanın finansal davranışını/veri
   modelini mi gösteriyor, yoksa genel-app altyapısını mı?"* Finansal model →
   gir. Genel-app altyapısı → atla.

   **Atlanır** (hangi uygulama olursa olsun aynı, ürün farkı göstermez):
   reklamlar; paywall/abonelik satış sayfaları; senkronizasyon/bulut yedekleme
   altyapısı (QuickSync ve benzerleri — zaten export/yedek kapsam dışı kararıyla
   aynı mantık); "Arkadaşına öner" / Puanla / Geri bildirim gönder / sosyal
   medya linkleri; dil, tema (açık/koyu), bildirim izni/sıklığı, PIN/biyometrik
   kilit gibi mobil uygulama standardı ayarlar.

   **Girilir** (Ayarlar menüsünün içinde olsa bile — finansal parametre
   taşıdıkları için): varsayılan hesap/kategori/kapsam ayarı; ay/bütçe döngüsü
   başlangıç günü; kategori yönetimi (ekle/düzenle/sil, alt kategori); hesap
   yönetimi (arşivleme/pasifleştirme, sıralama, tür değiştirme); kredi kartı
   ekstre parametreleri (kesim/son ödeme günü varsayılanı); tekrarlayan/planlı
   işlem genel davranış ayarı (örn. "otomatik mi onaylı mı" varsayılanı).

   Kısa test: *"Bu ayar değişirse bir sonraki kayıt/rapor farklı görünür mü?"*
   Evet → gir. Hayır (yalnız görünüm/dil/bildirim tercihi) → atla.

Bu ayrım, her uygulamanın her ayrıntısını test ederek süreyi büyütmeyi önler.

## Ekran görüntüsü planı

Her dokunuşun görüntüsü alınmaz. Aşağıdaki kontrol noktaları yeterlidir:

| Dosya | Görüntü |
|---|---|
| `00-magaza.png` | Uygulama adı, geliştirici ve sürüm bilgisi |
| `01-ilk-acilis.png` | Onboarding veya kayıt yaklaşımı |
| `02-bos-ana-ekran.png` | Veri eklenmeden önce ana ekran |
| `03-dolu-ana-ekran.png` | Ortak veriler girildikten sonra ana ekran |
| `04-islem-formu.png` | Gelir/gider formunun en açıklayıcı hâli |
| `05-siniflandirma.png` | Kategori, kapsam veya en yakın sınıflandırma |
| `06-islem-listesi.png` | Liste ve satır bilgi hiyerarşisi |
| `07-rapor.png` | Aylık rapor/nakit akışı |
| `08-hata-veya-bos-durum.png` | Varsa açıklayıcı hata/boş durum |
| `09-ozgun-ozellik.png` | Uygulamayı ayıran tek güçlü örnek |

Ham görüntü kırpılmaz; cihaz ve saat bağlamı korunur. Gerçek bilgi yanlışlıkla
görünürse paylaşmadan önce bulanıklaştırılır. Tam oturum için ekran kaydı
isteğe bağlıdır; bu sabit ekran görüntüleri zorunlu kanıttır.

### Kanıt karesi **dolu** olmalı ve dönemi **Ağustos 2026** (12 Eyl 2026 eklendi)

Bu kareler belge yazımında kullanılacak; okuyanın uygulamayı anlaması için
ekranda **veri görünmesi** gerekir. İki kural:

**1. Dolu durum çekilir.** Boş veya sıfırlanmış ekran kanıt değildir. Kart
borcu, taksit serisi ve tekrarlayan kayıt **ekranda görünürken** çekilir.
*(Money Manager'da kontrol karesinde kart ₺0,00 görünüyordu — harcandı ve
ödendi — okuyan "bu uygulama kart harcamasını göstermiyor" sanabilirdi.)*

**2. Dönem seçilebiliyorsa Ağustos 2026'ya alınır.** Sentetik verinin
çoğunluğu Ağustos 2026'ya girildi (`SENTETIK-TEST-VERISI.md`); ay gezgini,
filtre paneli, rapor dönemi ve hesap defteri o aya alınarak çekilir, yoksa
ekranlar boş veya yarım görünür. Kare adında ay belirtmek serbesttir.

**Üç istisna — bu durumlarda Ağustos zorlanmaz:**

- **Dönemsiz ekranlar:** form, ayar, onboarding, diyalog, boş durum, mağaza
  sayfası, kategori/hesap yönetimi. Dönem kavramı yoksa kural uygulanmaz.
- **Verinin başka aya düştüğü durum:** bazı uygulamalar geçmiş tarihe kayıt
  kurdurmuyor (Wallet'ta B1 planı 10 Eylül'e düştü) ya da taksit/tekrar sonraki
  aya taşıyor. Veri neredeyse kare oradan çekilir.
- **Varsayılan dönemin kendisi bulguysa:** Goodbudget'ta raporların varsayılan
  olarak **içinde bulunulan ayı** göstermesi kritik bir bulgudur
  (`14-reports-default-current-month.png`); o kareyi Ağustos'a çevirmek kanıtı
  yok eder. Bu durumda **hem varsayılan hem Ağustos** çekilir.

Dönem seçici bir ürün özelliğiyse (Hesap Defterim'de `Herşey / Günlük /
Haftalık / Aylık / Yıllık`) farklı seçimlerin davranış farkı **kasıtlı olarak**
ayrı karelerle gösterilir.

**Ayrıca hesap kavramı olan uygulamalarda her hesap türünün kendi defteri ayrı
bir karedir** — hesap listesi çoğu üründe yalnız toplam gösterir, hareketler
hesabın içindedir. **Varsa** en az bir nakit hesabı ve bir kredi kartı hesabının
defteri çekilir; kart defterinde harcama + ödeme + varsa taksit satırı **aynı
karede** görünmelidir. Hesap ayrımı olmayan üründe (Hesap Defterim'in tek
sütunlu yürüyen bakiye defteri) bu madde uygulanmaz.


## Gözlem formu yazım kuralları (12 Eyl 2026 — Money Manager denetiminden)

> **Bu kuralların makineyle bakılabilen kısmı `./denetim.sh` ile denetlenir**
> (ölü atıf, yollu atıf dahil · yetim kare, yalnız kısa kodla anılan dahil ·
> belirsiz aralık atıfı · yasak kalıp · tablo sütun uyumsuzluğu). Formu yazmadan
> önce ve yazdıktan sonra çalıştırın. Çıkış kodu: 0 hata yok, 1 hata var, 2
> kullanım hatası veya olmayan form. Tam adı başka yerde geçen karenin kısa
> kodla da anılması ve çözülemeyen kısa kodlar varsayılan olarak hatadır;
> `--siki` uyumluluk için aynı davranışı korur. Çift ters tırnakla
> yazılmış kod parçası ve çitli kod bloğu kural örneği sayılır. Kapının kendi pozitif/
> negatif testleri: `node denetim-test.cjs` *(14 Eyl 2026, P1-K)*. İçerik
> doğruluğu — karede yazan ile metinde yazanın aynı olması — elle kontrol
> edilir, script bunu göremez.

Money Manager doğrulama turunda 24 hata çıktı ve hepsi dört kalıptan geliyordu.
Kalan uygulamalarda tekrarlanmasın diye kural hâline getirildi.

### 1. Ekran etiketi **ekranda göründüğü dilde** alıntılanır

Uygulama Türkçe çalışıyorsa sekme, buton, sütun, boş durum ve hata metni
**Türkçe** yazılır. Belge 1 bir arayüz belgesidir; İngilizce ada göre yazılmış
bir sekme adı orada doğrudan yanlış bilgi olur.

> Money Manager'da 11 yerde İngilizce yazılmıştı: `Daily/Calendar/Monthly` →
> **Gün/Takvim/Ay**, `Save/Continue` → **Kaydet/Devam et**,
> `"No data available."` → **"Veri yok."**, `Balance Payable/Outstanding` →
> **Bu Ay/Gelecek Ay**.

Uygulamanın kendi dili İngilizceyse (Wallet, Goodbudget) İngilizce yazılır —
kural "Türkçeleştir" değil, **"ekranda ne yazıyorsa o"**.

Uygulamalar arası karşılaştırma tablolarında özgün etiketin yanına parantez
içinde Türkçe karşılık yazılabilir (`Envelopes` (zarflar)); okunabilirlik için
serbesttir, ama **özgün etiket silinmez**.

### 2. Kanıt atıfı **tam dosya adıyla** yapılır

`` `04-islem-formu-ve-kategori.png` `` yazılır. **Yasak olan belirsiz
aralıktır** (`` `04` ``, `` `10`–`28` ``): hangi karenin hangi cümleyi
desteklediğini kaybettiriyor ve Belge 1/2'de her cümlenin altına kare konacak.

**Serbest olan iki biçim:**

- **Açık liste** — bir iddia birden çok kareye dayanıyorsa hepsi adıyla yazılır:
  `` `12-hesaplar-kart-borcu-bu-ay.png`, `23-taksit-kart-borcu-bu-gelecek-ay.png` ``
- **Sıralı akış** — bir akış anlatılıyorsa kareler ok diziliminde verilir:
  `` `f7-15-yeni-islem.png` → `f7-19-tutar-girildi.png` → `f7-24-d2-kaydedildi.png` ``

> Bluecoins (90), Wallet (106) ve KolayBi (39) hâlâ aralıkla anıyor —
> 235 kare. Bu üçü doğrulama turunda tam ada çevrilecek.
> *(14 Eyl 2026, P1-K: KolayBi tam ada çevrildi ve P1'in yedi formu yeni
> kapıdan hatasız geçiyor. Bluecoins'te 80, Wallet'ta 97 tam adsız kare ve
> üçer belirsiz aralık kaldı; P2/P3 G paketlerinde kapanır.)*

### 3. Kare değişirse **atıf da değişir**; ölü atıf bırakılmaz

Bir kare silinir, yeniden çekilir veya adı değişirse forma giden bütün atıflar
aynı commit'te güncellenir. Form denetiminin ilk adımı şudur: **formda anılan
her dosya diskte var mı, diskteki her dosya formda anılıyor mu.**

> Money Manager'da Faz 1'de İngilizce kareler Türkçeleriyle değiştirilmiş ama
> form güncellenmemişti: **9 ölü atıf**, üçünün karşılığı hiç yoktu.

### 4. Yeniden koşumda eski koşumun iddiaları **ya doğrulanır ya düşer**

Bir uygulama yeni cihazda/dilde/sürümde yeniden koşulduğunda, önceki koşumdan
gelen her cümle ya yeni kareyle doğrulanır ya `Doğrulanamadı` olarak işaretlenir
ya da silinir. Sessizce taşınmaz.

> Money Manager'da Tur 1'den taşınan iki cümle yeni koşumun kareleriyle
> **çelişiyordu**: "Salary %100" (gelirin kategorisi **Diğer** çıktı) ve
> "Türkçe yok" (arayüzün tamamı Türkçeydi).

### 5. Emülatörde ayar değiştirilirse **ölçülür ve geri alınır**

Davranış anlamak için bir ayar açılıp kapatılabilir; ama önce/sonra değerleri
kareyle kaydedilir, ayar **aynı oturumda eski hâline döndürülür** ve geri
alındığı forma yazılır. **Geri alınamayacak bir değişiklikse (tek yönlü geçiş,
veri üreten/silen ayar) önce kullanıcıya sorulur.** Kayıt silme bu kapsamda
değildir — o `SENTETIK-TEST-VERISI.md` kuralına tabidir (kullanıcı istemeden
silinmez).

### 6. "Ne kazandırıyor / ne kaybettiriyor" nerede zorunlu

Karar tablosunda **`alma` ve `kararı yeniden sor`** satırlarında **zorunludur** —
bunlar bir yaklaşımı reddeden veya kendi kararımızı sorgulayan satırlar, gerekçe
tek taraflı olamaz. `doğrudan al` ve apaçık `uyarlayarak al` satırlarında
isteğe bağlıdır; kısa gerekçe yeter. Ayrıca formun **"Ne kazandırıyor / ne
kaybettiriyor"** bölümü yalnız o uygulamanın **ayırt edici** yaklaşımları için
doldurulur, her gözlem için değil.

> Money Manager'da "Toplama Dahil Et" kapatılıp net varlığa etkisi ölçüldü
> (₺42.350 → ₺38.200), sonra açılıp toplamların döndüğü doğrulandı.

## Test sırasında not alma

Her görevde yalnız şu alanlar doldurulur:

- Sonuç: `Tamamlandı`, `Desteklenmiyor`, `Ücretli`, `Engelli`, `Belirsiz`
- Adım/dokunuş sayısı: Yaklaşık değer
- İyi çalışan nokta: Bir cümle
- Sürtünme veya belirsizlik: Bir cümle
- Kanıt: İlgili ekran görüntüsü adı **+ kanıt etiketi**
  (`Manuel gözlem` / `Resmî kaynak (alt tür)` / `Çıkarım` / `Doğrulanamadı` —
  tanımlar `README.md` → "Kanıt etiketleri — tek kaynak")
- BusinessFinance kararı: beş sonuçtan biri — tanımlar `README.md` →
  "Karar sonuçları — tek kaynak". Gerekçe yalnız "ADR'miz böyle" diyemez

## Uygulama başına döngü

Pilot **Money Manager (Realbyte)** ile yapıldı (1 Eyl 2026). Sonraki her
uygulamada izlenen sıra:

1. `K00–K08` görevlerini uygula.
2. Sürülen uygulamada ek koşumu (A / B / B1 / B2) ekle.
3. ~10 dk hızlı arayüz gezintisi; kanıt karelerini
   `kanitlar/<uygulama>/` altına adlandırma kuralıyla kaydet
   (`kanitlar/README.md`).
4. `UYGULAMA-GOZLEM-SABLONU.md` kopyalanarak `gozlemler/<uygulama>.md`
   doldurulur; her satır kanıt etiketi taşır.
5. `DURUM.md` tablosu güncellenir, kullanıcı formu onaylar, sıradaki uygulamaya
   geçilir.

**Dokuz uygulamanın dokuzu da bu döngüden geçti** (5 canlı, 4 masa başı);
güncel dağılım `README.md` → "İnceleme türü" bölümündedir.

