# Manuel Rakip Uygulama Test Protokolü

## Amaç

Rakip uygulamaları aynı görevler ve aynı sentetik verilerle karşılaştırmak;
arayüz beğenisini ürün davranışından ayırmak ve Aşama 06.2 için kanıt üretmek.
Bu bir QA kabul testi değildir: rakibin “doğru” davranmasını beklemeyiz,
gözlenen davranışı kaydederiz.

## İncelenen uygulamalar

**Tur 1 çekirdek 7 uygulama** (`DURUM.md` Tur 1 tablosu):

| # | Uygulama | Paket | Erişim |
|---|---|---|---|
| 1 | Money Manager (Realbyte) | `com.realbyteapps.moneymanagerfree` | Sürülebilir |
| 2 | Paraşüt | `com.parasut` | Masa başı (kayıt web + ücretli) |
| 3 | Logo İşbaşı | `com.isbasi` | Masa başı (Müşavir Portal) |
| 4 | KolayBi | `com.kolaybi.mobil` | Masa başı |
| 5 | QuickBooks Solopreneur | `com.intuit.quickbooks` | Masa başı (ödeme + bölge) |
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

Ek koşum kayıtları test sonrası **silinmez, olduğu gibi cihazda bırakılır**
(kontrol değerinden sapmış hâlde kalır) ve **uygulama Tur 2'ye seçilse bile
sıfırlanmaz** — Tur 2 kayıtları bu verinin üzerine eklenir. (11 Eyl 2026'da
sabitlendi; ilk sürümde "Tur 2'ye seçilirse sıfırlanır" yazıyordu, ama Hesap
Defterim'in ikinci ek koşumunda kullanıcıya sorulmadan silme denendi ve
kullanıcı bunu reddetti — kural o olaydan sonra "hiçbir zaman sormadan silme"
olarak kesinleşti, bkz. `gozlemler/hesap-defterim.md` "Metodoloji notu"; 11 Eyl
2026'da Bluecoins Tur 2'ye seçilince aynı ilke Bluecoins'e de uygulandı).
Yalnız kullanıcı açıkça "sil" veya "kontrol değerine döndür" derse temizlenir.
İçine girilemeyen (resmî kaynak) uygulamalarda A/B/B1/B2 canlı koşulmaz; varsa
video/yardım merkezinden `Resmî kaynak` etiketiyle not edilir.

Bir özellik yoksa benzerini zorlayarak üretme. Örneğin işletme/şahsi ayrımı
yoksa kategoriyle taklit etme; doğrudan `Desteklenmiyor` yaz. Bu yokluğun
kendisi araştırma bulgusudur (B1/B2 için de geçerli).

**K00–K08'den sonra ~10 dk'lık arayüz taraması yapılır** (görevlerden bağımsız):
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

## Test sırasında not alma

Her görevde yalnız şu alanlar doldurulur:

- Sonuç: `Tamamlandı`, `Desteklenmiyor`, `Ücretli`, `Engelli`, `Belirsiz`
- Adım/dokunuş sayısı: Yaklaşık değer
- İyi çalışan nokta: Bir cümle
- Sürtünme veya belirsizlik: Bir cümle
- Kanıt: İlgili ekran görüntüsü adı
- BusinessFinance kararı: `Doğrudan al`, `Uyarlayarak al`, `Alma`, `Henüz karar verme`

## İlk oturum

İlk pilot **Money Manager (Realbyte)** ile yapılır:

1. `K00–K08` görevlerini uygula.
2. Yukarıdaki ekran görüntülerini al.
3. `UYGULAMA-GOZLEM-SABLONU.md` dosyasını Money Manager için kopyala ve kısa
   notları doldur.
4. Görüntüleri ve notları toplu olarak yapay zekâya ver.
5. Formda eksik bir alan yoksa Paraşüt'e ve ardından kalan uygulamalara geç.

