# Manuel Rakip Uygulama Test Protokolü

## Amaç

Yedi uygulamayı aynı görevler ve aynı sentetik verilerle karşılaştırmak;
arayüz beğenisini ürün davranışından ayırmak ve Aşama 06.2 için kanıt üretmek.
Bu bir QA kabul testi değildir: rakibin “doğru” davranmasını beklemeyiz,
gözlenen davranışı kaydederiz.

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

Bir özellik yoksa benzerini zorlayarak üretme. Örneğin işletme/şahsi ayrımı
yoksa kategoriyle taklit etme; doğrudan `Desteklenmiyor` yaz. Bu yokluğun
kendisi araştırma bulgusudur.

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
seçimini kör yapmamak. **Orta derinlik** — Tur 2 kadar ayrıntılı değil:

- Her formun kendi eksik listesi koşulur (kart Pay akışı, planlı işlem oluşturma,
  taksit, yedek, export vb.); K00–K08 baştan tekrarlanmaz.
- Money Manager istisnası: Tur 1'de işlemler yanlış tarihe girildiği için
  çekirdek işlemler **Ağustos 2026** tarihleriyle yeniden girilir.
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

