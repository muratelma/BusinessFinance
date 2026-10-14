# Kategori ve hesap teslimi denetimi — 13 Ekim 2026

Önceki teslimin görsel doğrulaması yetersizdi. Kullanıcının bildirdiği tarih seçici, pasif hesaplar ve hesap ayarları sorunları doğrulandı. Testlerin geçmesi, referans tasarımın uygulandığını kanıtlamamıştı.

## Önceki karşılaştırmanın sınırı

Referans 412×892, Flutter çıktısı 824×1784 idi. Önceki altı karşılaştırma sayfasında ikisi de 206×446'ya küçültüldü; birkaç kare ayrıca açıldı. Ekran testleri bir referans görüntüye karşı piksel farkı ölçmüyor: yalnız bileşenleri çiziyor ve exception denetliyor. Ayarlar callback'inin ekran testinde boş olması gerçek gezinme hatasını gizledi. Bu yönteme dayanarak “tasarım doğrulandı” sonucu verilmesi değerlendirme hatasıdır.

Bu turda referans kendi 412×892 boyutunda tutuldu; yalnız Flutter'ın 2× çıktısı aynı mantıksal boyuta getirildi. Karşılaştırma sayfası 51 hâlin referansını, önceki teslimini ve düzeltmesini birlikte gösterir: [karşılaştırmalar](../scratch/category-account-audit/index.html). Bu da otomatik piksel eşitliği ya da kullanıcı kabulü değildir.

## Doğrulanan bulgular ve düzeltmeler

| Bulgu | Etkilenen ekran / kaynak | Düzeltme ve kanıt |
|---|---|---|
| Ay seçici dar, hücreler kısa; çubuk hücrenin kenarında; ileri boş aylar düz gri; mevcut ayda gereksiz “Bu ay” düğmesi | A9, H15, `AppMonthPicker` | 348 dp dialog, 60 dp hücre, içerden yuvarlak çubuk, seçilide beyaz çubuk, kapalı ayda kesikli kenar, seçilen ay/tutar ve filtre bağlamı. Büyük yazıda yükseklik ve eylemler uyarlanır. |
| Pasif hesaplar düz bir metin düğmesi; liste satırlarında ok yok; toplamın alt hücreleri hizasız | D5, `AccountsOverviewPage` | Pasif bölüm ikonlu kart ve açılma göstergesi; alt listesi çalışır. Bakiye açıklaması, satır okları, ayırıcı ve eşit özet sütunları. Pasif bakiye toplamda kalır. |
| Ayarlar düğmesi formu açamıyor; aynı hata Hesap ekle'de | Gerçek `createAppRouter` rotaları | `pageBuilder` context'i Navigator'ın üstündeydi. `Builder` ile sayfa context'i kullanılır. Hata gerçek rota testinde `Navigator operation requested with a context that does not include a Navigator` olarak yakalandı. |
| Ayarlar okunurken geri bildirim yok; hata yalnız kısa bildirimde | Hesap ayarları | Formun açıldığı rota hemen yükleme gösterir; hata, yeniden deneme ve oturum sonu görünür kalır. Başarılı kaydetme aynı hesap sayfasına döner. |
| Seçili kapsam/yön işareti yok | A/B/H filtreleri | Ortak filtre çipinde görünür onay işareti. |
| Menüler yanlış konumda, bağlam başlığı yok, açık/seçili durum zayıf; uzun seçenekler dar | A2/A6/A7, B2/B3, H4/H5/H6 | Referanstaki başlık, ayırıcı, seçili yüzey, ok ve kenar payı; kategori menüleri solda, özet/hesap menüleri sağda. Uzun menü ve büyük yazı için kaydırma korunur. |
| Kişi menüsünde toplam yok; hesap çıkış menüsünün “Hepsi” tutarı işaretsiz | B3, H6 | “Hepsi” toplamı sunucudan; çıkan toplam ve seçimin toplamı yön işaretiyle gösterilir. Yöne bağlı adlar ortak sözlükten gelir. |
| Kategori satırlarında kart/vadeli ikonları kategoriyi anlatmıyor; parça satırları ebeveyn bilgisini kaybediyor | B1/B5/B6/B7 | Normal kalem kategorinin ikonunu; komisyon/masraf/faiz parçası bağlı işlemin ikonunu kullanır. Kişi, hesap, POS ve transfer bağlantısı doğru DTO alanından okunur. |
| Ödeme durumu sağdaki tutar yüzünden erken kesiliyor | B5 | Durum satırı tutarın altına uzanabilecek genişlikte; başlık ve tutar üstten hizalı. |
| Büyük hesap tutarlarında Net ikinci satıra düşüyor | H13 | Normal yazıda üç eşit sütun, 15 dp metrik; büyük yazıda doğal sarma. Pozitif netin işareti görünür. |
| Hesap hareketlerinde yön oku eksik, tür/kişi tekrarı var; POS tarih bilgisi eksik | H1–H13 | Transfer oku ikonla; tekrarlar ayıklanır; POS satış tarih aralığı ve çoklu tahsilat sayısı korunur. |
| Boş ekran genel hata şablonuna benziyor; ekleme düğmesi küçük | A10/A11, B10, H12, D6 | Referansın yuvarlak ikonu, 17 dp başlığı ve üst boşluğu; seçimin korunduğu açıklaması, uygun filtre kaldırma; tam genişlikte Hesap ekle. |
| Transfer masraf bilgisi açıklamanın altında; alan ve düğme düzeni farklı | D3/D3b | Tarih ikonu solda, masraf bilgisi masraf alanından sonra, tek satır açıklama, “Kaydet”. Ön doldurulan tutarın binlik ayracı dört ondalığı kaybetmeden gösterilir. |
| Kayıt sürerken kapsam çipi input/preview'i değiştirebiliyor | `TransferController`, transfer formu | Kaydetme sırasında giriş değişimi engellenir; kaydedilen önizleme yakalanır. Tek istek ve doğru bütçe yenileme sinyali regression testiyle doğrulanır. |
| Kaynak menüsü nakit/banka ikonunu ayıramıyor | `SourcesAsync`, `PageOption.Kind` | Mevcut isteğe bağlı `kind` alanı hesap seçeneğinde `cash/bank` olarak doldurulur; boş sonuçta seçili kaynakta da korunur. Sözleşme kırılmaz, migration eklenmez. |

## Backend denetimi

Finansal doğruluk görsel kaliteden türetilmedi. Gerçek SQL üzerinde bilinen sentetik tutarlarla iki yeni senaryo eklendi:

- Mal alımı 1.270: hesaptan 200, karttan 300, vadeli 770. Sonradan bankadan ödenen 120, gelir/gider kaynağını bankaya taşımaz. Kişisiz 500 ve kişiye bağlı 770; kaynak/kişi kesişimi, boş seçili nakit hesabı, ödeme durumu ve kapsam toplamları sınandı.
- Banka giren 4.975, çıkan 2.620, net 2.355, bakiye 12.355. Yolda 294 bu toplamları artırmaz; en erken kartla tahsilin neti 98'dir. Tür/yön seçimleri bakiyeyi değiştirmez; boş ayda bakiye taşınır.

Önceki sahiplik, sayfalama, iptal/parça bağlantıları ve migration kontrolleri korunur. Ayrıca ayrı 5291 loopback Release API'de 78 HTTP kontrolü tekrar geçti. Bu kapsamda yeni bir sayısal hesaplama hatası bulunmadı; bütün olası backend hatalarının yokluğu sonucuna varılmaz.

Tam backend koşusu: **1.386 başarılı**, iki isteğe bağlı canlı sağlayıcı testi atlandı (Domain 402, Application 400, API 327, Infrastructure 257). Release build sıfır uyarı/hata; format kapısı temiz.

## Referansla ayrılması gereken veri ve karar farkları

- API denemesi ileri kayıt yasağını bozmamak için Ekim gün numaralarını Eylül'e taşır. Geçmiş ayı doldurmayan senaryoda “Geçen ay” sıfır olabilir; bu tutar arayüze örnek sayı olarak yazılmaz.
- H1'deki dört satış toplamı, tek satış örneğiyle aynı veri değildir. Gerçek liste ve kutu aynı sunucu kaynağından gelir.
- H11'de 2.000 tutarlı alacak taksidi hesaba gerçekten girer; referansın nakit giren sayısındaki eksik korunmaz.
- H13'te sunucunun taşımadığı yükümlülük masrafı uydurulmaz.
- A8c/A8d ve kategori atama kullanıcının kapsam dışı kararıdır; kategorisiz geçmiş salt okunur.
- POS ve yükümlülük adları aşamadaki tek sözlük kararından okunur. Çizimdeki farklı işlem adları sayfalara kopyalanmaz.
- Ortak işlem ayrıntısı mevcut bakiyeleri, iptal kilitlerini ve kalan tutarları korur. Çizimde görünmüyor diye finansal bilgi kaldırılmaz.

## Son doğrulama

Flutter'ın son koşusu, APK ve canlı emülatör kontrolü tamamlandığında burası güncellenecek. Tasarım kabulünün kullanıcıda olduğu korunur; Aşama 06.3 Aktif kalır.

Ek incelemede ortak ayrıntının “Gelir/gider raporunu etkilemez” açıklamasının masraflı transfer ve faizli ödemede yanlış olduğu doğrulandı. Ana tutar nötrdür; masraf/komisyon/faiz gelir veya giderdir. Açıklama buna göre düzeltildi ve regresyon eklendi. Yolda kutusu yalnız güncel ayın Tümü/Giren görünümünde yer alır; geçmiş ay ve Çıkan görünümü kendi hareketlerini gösterir. Seçim toplamının işaretli tutarı sunucunun seçeneğinden okunur.

D1/D2/D2b ortak panel testlerinde tutarlı sentetik ayrıntı örnekleri kullanılır; referansın bire bir veri seti değildir. Ortak panelin bakiye ve iptal kilidi alanları korunur. K0'da profil örneği; A8/A8b'de geçmiş örneği; B9/B9b/H13/H10b'de ek sentetik örnekler vardır. Bunlar aynı veriyle piksel karşılaştırması olarak yorumlanmaz.
