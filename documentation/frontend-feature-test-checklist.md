# Frontend Özellik Test Kontrol Listesi

Bu belge, Stage 0–12 arasında uygulanmış mobil özellikleri kullanıcıyla birlikte
manuel test etmek için kısa devam noktasıdır. Test çalışması
`test/frontend-feature-testing` branch'inde yürütülür. Stage 13 kapsamına
geçilmez.

Durum işaretleri:

- `[ ]` Bekliyor
- `[~]` Devam ediyor
- `[x]` Başarılı
- `[!]` Sorun bulundu

## Manuel kabul sonrası kalan UX iyileştirmeleri

Bu liste, 13 Ağustos 2026 manuel kabulünde fonksiyonel doğruluğu bozmayan fakat
kullanıcı deneyimini eksik bırakan bulguları ayrı bir kapanış kapısı olarak
tutar. Grafik iyileştirmesi önceki çalışma grubunda tamamlanmıştır; diğer
maddeler otomasyon ve güncel APK kabulü olmadan tamamlandı sayılmaz.

- [~] Yanlış parola için API'nin İngilizce mesajı yerine
  `E-posta veya parola geçersiz.` gösterilir.
- [~] CSV onayı sonrasında içe aktarılan, atlanan ve hatalı satır sayıları açık
  bir başarı özetinde gösterilir; kullanıcı `İşlemlerde görüntüle` eylemiyle
  İşlemler ekranına geçebilir.
- [~] İşlem CSV'si ve finans JSON'u paylaşılmadan önce uygulama içinde sınırlı
  içerik önizlemesi, dosya adı, türü, boyutu ve oluşturulma zamanı gösterilir.
- [~] Backup için hassas payload'u açmadan sürüm, kayıt sayısı, dosya boyutu ve
  oluşturulma zamanı özeti gösterilir.
- [~] CSV, JSON ve backup için paylaşmadan ayrı, Android sistem dosya oluşturma
  ekranını açan görünür `Cihaza kaydet` eylemi bulunur.
- [x] Nakit akışı grafiğinde her ayın `AA.YYYY` etiketi, net değeri ve sıfır
  aylar için eksen noktası görünürdür.
- [~] Kullanıcıya gösterilen API hata kodları genel olarak taranır; bilinen
  İngilizce auth/authorization mesajları merkezi olarak Türkçeleştirilir.

Doğrulama kapısı: hedef Flutter testleri, bütün `flutter test`,
`flutter analyze`, Android debug APK build ve ardından Pixel 8 manuel kabulü.

Uygulama durumu — 14 Ağustos 2026:

- Altı açık madde Flutter ve Android tarafında uygulandı. Hedef testler 23/23,
  bütün Flutter paketi 102/102 geçti; analyze temiz ve debug APK başarılıdır.
- `[~]` maddeler yalnız güncel APK ile Pixel 8'de yanlış giriş, CSV onayı,
  CSV/JSON/backup önizleme, paylaşma ve sistem `Cihaza kaydet` akışları manuel
  kabul edilince `[x]` yapılacaktır.

### CSV export → import sözleşmesi bulgusu — 14 Ağustos 2026

- [~] Uygulamanın işlem CSV export'u banka ekstresi import sözleşmesiyle aynı
  değildir: export yönü `type` kolonunda, importer yönü tutarın işaretinde taşır.
  Yanlış sınıflandırmayı önleyen açıklayıcı engel; yaygın `Türkçe banka`,
  `İngilizce banka` ve tamamen düzenlenebilir `Özel` eşleme seçenekleri
  uygulandı. Güncel APK manuel kabulü bekliyor.

## Test ön koşulları

- Pixel 8 emulator çalışıyor.
- Flutter uygulaması güncel branch'ten Pixel 8'e kurulmuş.
- Yerel SQL Server container sağlıklı.
- ASP.NET Core API yerel geliştirme adresinde çalışıyor.
- Yalnız sentetik finansal veri kullanılıyor.

## Mevcut özellik envanteri

Bu tablo yalnız Stage 0–12 sonunda mobil uygulamada ve bağlı backend'de mevcut
olan davranışları içerir. Stage 13 ve sonrası özellikler aşağıdaki “kapsam dışı”
bölümünde ayrıca listelenmiştir.

| No | Özellik | Ne işe yarar? | Testte gözlenecek ana sonuç |
|---:|---|---|---|
| 1 | Kayıt olma | E-posta ve parolayla yeni kullanıcı oluşturur. | Geçerli kayıt kabul edilir; hatalı form güvenli biçimde reddedilir. |
| 2 | Giriş yapma | Mevcut kullanıcıyı doğrular ve oturum açar. | Başarılı giriş Özet ekranına götürür; yanlış bilgi açık hata verir. |
| 3 | Oturumu hatırlama | Tokenları güvenli mobil depoda saklar. | Uygulama kapatılıp açıldığında geçerli oturum geri gelir. |
| 4 | Token yenileme | Süresi dolan access tokenı refresh token ile yeniler. | Kullanıcı gereksiz yere giriş ekranına atılmaz ve yalnız bir retry yapılır. |
| 5 | Çıkış yapma | Sunucu oturumunu ve cihazdaki tokenları temizler. | Çıkıştan sonra korumalı ekranlara erişilemez. |
| 6 | Kullanıcı izolasyonu | Her kullanıcının finansal verisini ayrı tutar. | İkinci kullanıcı ilk kullanıcının kayıtlarını göremez veya değiştiremez. |
| 7 | Hesap yönetimi | Nakit veya banka hesabı oluşturur ve düzenler. | Ad, tür, açılış bakiyesi ve aktiflik doğru saklanır. |
| 8 | Hesap bakiyesi | Bakiyeyi açılış ve finansal hareketlerden hesaplar. | İşlem, transfer ve ödemeler sonrasında doğru bakiye görünür. |
| 9 | Kategori yönetimi | Gelir ve giderleri sınıflandırır. | Kategori oluşturma/düzenleme çalışır ve gelir–gider türü korunur. |
| 10 | Gelir/gider işlemleri | Hesaba tarihli finansal hareket kaydeder. | Tutar, hesap, kategori, tarih ve açıklama kayıpsız görünür. |
| 11 | İşlem filtreleme | İşlemleri tür, hesap, kategori ve tarihe göre süzer. | Liste yalnız seçilen koşullara uyan kayıtları gösterir. |
| 12 | İşlem iptali | Hatalı hareketi geçmişi silmeden etkisizleştirir. | Kayıt iptal olarak kalır; bakiye ve rapor etkisi kalkar. |
| 13 | Aylık özet | Ayın gelir, gider, net ve kategori dağılımını gösterir. | Ay değiştirildiğinde bütün özet değerleri birlikte yenilenir. |
| 14 | Aylık kategori bütçesi | Gider kategorisine aylık limit koyar. | Harcanan, kalan ve aşım gerçek işlemlerden hesaplanır. |
| 15 | Hesaplar arası transfer | Parayı iki hesap arasında taşır. | Kaynak azalır, hedef artar; gelir/gider toplamı değişmez. |
| 16 | Kredi kartı | Limit, kesim ve son ödeme bilgileriyle kart tanımlar. | Kart borcu ve kullanılabilir limit doğru görüntülenir. |
| 17 | Kart harcaması | Kredi kartına harcama kaydeder. | Kart borcu artar ve kullanılabilir limit azalır. |
| 18 | Kart borcu ödemesi | Bir hesaptan karta ödeme aktarır. | Hesap azalır, kart borcu düşer ve ödeme ikinci gider olmaz. |
| 19 | Kart ekstresi | Seçilen dönemin kart hareketlerini ve borcunu gösterir. | Harcama, ödeme ve dönem borcu aynı ekstrede tutarlı görünür. |
| 20 | Taksit planı | Kart harcamasını tarihli aylık parçalara böler. | Taksitlerin toplamı plan toplamına tam eşittir. |
| 21 | Taksit gerçekleştirme | Vadesi gelen taksidi kart hareketine dönüştürür. | Aynı taksit ikinci kez gerçekleştirilemez. |
| 22 | Tekrarlayan işlem planı | Maaş, kira veya abonelik tekrarını tanımlar. | Planın aktifliği ve sonraki çalışma tarihi görünür. |
| 23 | Planlanan kayıt üretme | Vadesi gelen tekrarları aday kayda dönüştürür. | Aynı dönem için mükerrer aday oluşmaz. |
| 24 | Planlanan kaydı onaylama | Adayı gerçek gelir veya gider yapar. | Finansal etki yalnız açık onaydan sonra oluşur. |
| 25 | Yaklaşan ödemeler | Kart, taksit, tekrar ve borç vadelerini birleştirir. | Seçilen zaman ufkunda yaklaşan/geciken kayıtlar görünür. |
| 26 | Gelişmiş raporlar | Net varlık, dönem neti ve ödeme yükünü hesaplar. | Hesap/kart dağılımı, bütçe sapması ve nakit akışı tutarlıdır. |
| 27 | CSV önizleme | Ekstreyi doğrudan işlem oluşturmadan staging alanına alır. | Geçerli ve bozuk satırlar ayrılır; bakiye henüz değişmez. |
| 28 | CSV kolon eşleme | Tarih, tutar, açıklama ve referans kolonlarını belirler. | Satırlar seçilen hesap ve kategoriyle hazır duruma gelir. |
| 29 | CSV onaylama | Hazır satırları gerçek finansal işleme dönüştürür. | Seçilen hazır satırlar atomik biçimde içe alınır. |
| 30 | Mükerrer import kontrolü | Aynı dosya veya satırın tekrarını yakalar. | Kullanıcı atlama veya yine de içe alma kararı verebilir. |
| 31 | Borç/alacak planı | Ödenecek borç veya alınacak alacak takvimi oluşturur. | Anapara, toplam ödeme, faiz ve taksitler doğru görünür. |
| 32 | Borç taksiti ödeme | Taksidi bir hesap hareketiyle kapatır. | Hesap etkilenir; normal gelir/gider raporu bozulmaz. |
| 33 | Tasarruf hedefi | Hedef tutar ve tarihle birikimi izler. | Ayrılan, kalan, yüzde ve durum doğru hesaplanır. |
| 34 | Manuel hedef katkısı | Hedefe bağımsız katkı kaydeder. | Tekrarlanan aynı istek yalnız bir katkı oluşturur. |
| 35 | Hesap bakiyeli hedef | Hesap bakiyesini hedef ilerlemesi olarak kullanır. | Hesap bakiyesi ve manuel katkı birlikte sayılmaz. |
| 36 | Fiş/belge eki | İşleme PDF, JPEG veya PNG bağlar. | Belge listelenir, indirilir ve paylaşım ekranına gönderilir. |
| 37 | Dosya güvenliği | Boyut, uzantı, medya türü ve byte imzasını denetler. | Sahte uzantı veya izin verilmeyen içerik reddedilir. |
| 38 | Transaction CSV export | İşlem listesini tablo dosyası olarak dışa aktarır. | Güvenli CSV Android paylaşım ekranına gönderilir. |
| 39 | Finans JSON export | Finansal veriyi makinece okunur biçimde dışa aktarır. | Finans verisi çıkar; parola ve token dosyaya girmez. |
| 40 | Backup oluşturma | Restore edilebilir versiyonlu yedek üretir. | Backup dosyası oluşturulup paylaşılabilir. |
| 41 | Backup doğrulama | Restore öncesinde format, hash ve referansları kontrol eder. | Bozuk veya uyumsuz dosya veri yazmadan reddedilir. |
| 42 | Backup geri yükleme | Doğrulanmış yedeği boş kullanıcı alanına yükler. | Açık onay gerekir ve başarısızlık yarım veri bırakmaz. |
| 43 | Loading/empty/error durumları | Ağ ve veri durumlarını görünür kılar. | Yükleniyor, boş, hata, retry ve yetkisiz durumları anlaşılırdır. |
| 44 | Responsive navigasyon | Telefon ve geniş ekrana uygun gezinme sunar. | Telefonda alt menü, geniş ekranda NavigationRail görünür. |
| 45 | Erişilebilirlik | Büyük metin ve ekran okuyucu kullanımını destekler. | Kritik bilgi yalnız renk/grafikle verilmez ve semantics bulunur. |

## Özellik grupları

### 1. Kimlik ve oturum

- [x] Yeni kullanıcı kaydı ve form doğrulaması
- [x] Giriş ve yanlış parola hatası
- [x] Uygulamayı kapatıp açınca güvenli oturumun geri gelmesi
- [x] Access token yenileme davranışı
- [x] Çıkış sonrası korumalı ekranların kapanması
- [x] İkinci kullanıcının ilk kullanıcının verilerini görememesi

Beklenen ana sonuç: Kullanıcı güvenli biçimde giriş yapar; oturum cihazda
korunur ve kullanıcı verileri birbirinden ayrılır.

Test notu — 13 Ağustos 2026:

- Mevcut sentetik emulator hesabının daha önce oluşturulduğu başarılı girişle
  doğrulandı; geçersiz e-posta form seviyesinde reddedildi.
- Yanlış parola korumalı ekrana geçmedi ve görünür hata mesajı gösterdi. Mesajın
  İngilizce olması (`Email or password is invalid.`) yerelleştirme gözlemi olarak
  kaydedildi.
- Uygulama `force-stop` sonrasında doğrudan Özet ekranına döndü; güvenli oturum
  geri yüklendi.
- Açık onaylı çıkıştan sonra giriş ekranı gösterildi ve korumalı Özet ekranı
  kapandı. Test sonunda aynı sentetik hesapla yeniden giriş yapıldı.
- İkinci sentetik kullanıcı oluşturulup giriş yapıldığında ilk Özet çiziminde
  önceki kullanıcının dashboard snapshot'ı görünür kaldı. Uygulama tamamen
  kapatılıp açıldıktan sonra Özet `Henüz hesap yok`, Hesaplar ekranı da
  `Henüz hesap yok` gösterdi. Backend sorgusu izole olsa da aynı process içindeki
  kullanıcı değişiminde eski Flutter state'inin temizlenmemesi veri gizliliği
  sorunu olarak kaydedildi. Sorun oturum nesli değiştiğinde korumalı Flutter
  state subtree'sini yeniden kuran sınırla giderildi. Widget testi A → B
  geçişinde eski snapshot'ın atıldığını kanıtladı; güncel APK ile emulator
  kabulünde A → B → A geçişleri aşağı çekerek yenileme yapılmadan doğru Özet
  verisini gösterdi.
- Restore hesabında uygulama 16 dakikadan uzun süre arka planda tutulduktan sonra
  Özet ve İşlemler yeniden giriş istemeden yenilendi. Access token refresh ve
  kaynak/restore kullanıcı geçişinde veri izolasyonu manuel kabul edildi.

### 2. Hesaplar ve kategoriler

- [x] Nakit ve banka hesabı oluşturma
- [x] Açılış bakiyesi
- [x] Hesap adı ve aktiflik durumunu düzenleme
- [x] Hiç kullanılmamış hesabı onayla kalıcı silme; kullanılmış hesabın reddedilmesi
- [x] Gelir ve gider kategorisi oluşturma/düzenleme
- [x] Pasif hesap veya kategoriyle yeni işlem oluşturulamaması

Beklenen ana sonuç: Hesap ve kategori yaşam döngüsü geçmiş veriyi silmeden
çalışır.

Envanter notu — 13 Ağustos 2026:

- Ana test hesabında `Test Nakit Güncel` adlı aktif Nakit hesabı bulunuyor.
  1.174,50 TRY güncel bakiyeden 200,00 TRY gelir çıkarılıp 25,50 TRY aktif gider
  geri eklendiğinde 1.000,00 TRY açılış bakiyesi doğrulanıyor.
- `Ulaşım Testi` özel gider kategorisi ile hazır gelir/gider kategorileri
  bulunuyor. Banka hesabı, özel gelir kategorisi ve pasif hesap/kategori negatif
  akışı henüz tamamlanmadığı için birleşik maddeler devam ediyor bırakıldı.
- Kullanılmamış hesap silme backend ve Flutter tarafında uygulandı. Manuel kabul
  sırasında yanlışlıkla açılan kullanılmamış hesap onay penceresinden silindi.
  Finansal hareket bulunan `Test Nakit Güncel` hesabının silinmesi ise hata
  mesajıyla reddedildi ve geçmiş korundu.
- `Test Banka` adlı banka hesabı 2.500 TRY açılış bakiyesiyle oluşturuldu ve adı
  `Test Banka Güncel` olarak düzenlendi. Hesap sonraki transfer ve kart ödeme
  testleri için korunuyor.
- `Test Banka Güncel` pasife alındığında işlem oluşturma formundaki hesap
  seçeneklerinden çıkarıldı; yeniden aktifleştirme ve listede aktif durumun geri
  gelmesi sorunsuz çalıştı. `Freelance Test` gelir kategorisi oluşturulup
  `Freelance Test Güncel` olarak yeniden adlandırıldı.
- `Freelance Test Güncel` pasife alındığında gelir işlemi formundan çıkarıldı;
  yeniden aktifleştirme sorunsuz çalıştı.

### 3. İşlemler ve özet

- [x] Gelir ve gider kaydetme
- [x] Hesap, kategori, tür ve tarih filtreleri
- [x] İşlem iptali ve iptal geçmişinin görünmesi
- [x] Hesap bakiyesinin hareketlerden güncellenmesi
- [x] Aylık gelir, gider, net ve kategori dağılımı
- [x] Aylar arasında ileri/geri gezinme

Beklenen ana sonuç: İşlem, bakiye ve raporlar aynı finansal hareketlerden tutarlı
biçimde hesaplanır.

Test notu — 13 Ağustos 2026:

- 1,00 TRY sentetik gider oluşturulduğunda İşlemler listesi kendi kendine
  yenilendi. Pull-to-refresh yapılmadan Özet gideri 25,50 → 26,50 TRY, neti
  174,50 → 173,50 TRY ve hesap bakiyesi 1.174,50 → 1.173,50 TRY oldu.
- Aynı değişiklik Bütçeler ekranındaki Groceries harcananını 25,50 → 26,50 TRY,
  kalanı 74,50 → 73,50 TRY yaptı. Test işlemi iptal edilince Özet ve Bütçeler
  yine pull-to-refresh olmadan başlangıç değerlerine döndü; iptal kaydı geçmişte
  korundu.
- `Test Banka Güncel` hesabına `Freelance Test Güncel` kategorisiyle 300 TRY
  gelir kaydedildi. Tür, hesap, kategori ve 13 Ağustos 2026 tek-gün tarih
  filtreleri doğru sonuç verdi; filtre temizleme tam listeyi geri getirdi.
  Önceki aya gidilip Ağustos 2026'ya dönüldüğünde ayın verileri doğru yenilendi.

### 4. Bütçeler ve transferler

- [x] Aylık kategori bütçesi oluşturma
- [x] Bütçe limitini güncelleme
- [x] Harcanan, kalan ve aşım durumları
- [x] İki farklı hesap arasında transfer
- [x] Transfer iptali
- [x] Transferin gelir/gider toplamını değiştirmemesi

Beklenen ana sonuç: Transfer yalnız hesap bakiyelerini etkiler; bütçe gerçek
giderlerden hesaplanır.

Envanter notu — 13 Ağustos 2026:

- Ağustos 2026 Groceries bütçesi 100,00 TRY limit, 25,50 TRY harcanan ve 74,50
  TRY kalan gösteriyor. Geçmişte `Bütçe aşım testi` adlı 80,00 TRY gider
  oluşturulup iptal edilmiş; aşım görünümü bu oturumda canlı gözlenmediği için
  birleşik madde devam ediyor bırakıldı.
- `Groceries` limiti 20 TRY'ye indirilince 25,50 TRY harcama için 5,50 TRY
  aşım gösterildi; limit yeniden 100 TRY yapıldığında kalan 74,50 TRY oldu.
- `Test Nakit Güncel` hesabından `Test Banka Güncel` hesabına 100 TRY transfer
  kaydedildi. Bakiyeler sırasıyla 1.074,50 ve 2.900 TRY oldu; net varlık
  3.974,50 TRY, dönem gelir/gider/net değerleri 500 / 25,50 / 474,50 TRY olarak
  değişmeden kaldı. Ekranlar pull-to-refresh gerektirmeden doğru veriyi gösterdi.
- Transfer satırına dokunmak detay veya işlem ekranı açmıyor; mobil arayüzde
  iptal eylemi bulunmadığından `Transfer iptali` maddesi açık eksik olarak kaldı.
- Flutter'a transfer ayrıntı penceresi, aktif durum için ikinci onaylı iptal eylemi
  ve iptal sonrası hesap/özet yenilemesi eklendi. Finans testleri 9/9 ve analyze
  temizdir. Güncel APK'da ayrıntı, kapatma ve vazgeçme bakiyeleri değiştirmedi;
  onaylı iptal satırı `İptal` durumuna getirdi, hesap bakiyelerini geri aldı ve
  gelir/gider/net toplamlarını değiştirmedi. Tekrar iptal eylemi sunulmadı.
- Kart harcaması sonrasında Özet, Groceries giderini 625,50 TRY olarak doğru
  toplarken Bütçeler ekranı yalnız normal işlemlerdeki 25,50 TRY'yi gösterdi.
  `EfBudgetRepository` kart harcamalarını da toplayacak biçimde düzeltildi.
  Güncel backend/APK ile aynı mevcut veri yeniden okunduğunda harcanan 625,50
  TRY ve aşım 525,50 TRY olarak manuel doğrulandı; yeni harcama gerekmedi.

### 5. Kredi kartı ve taksitler

- [x] Kart, limit, kesim günü ve son ödeme günü oluşturma
- [x] Kart harcaması ve kullanılabilir limit
- [x] Banka/nakit hesabından kart ödemesi
- [x] Kart ödemesinin ikinci kez gider sayılmaması
- [x] Aylık ekstre görüntüleme
- [x] Taksit planı oluşturma ve taksit gerçekleştirme
- [x] Aynı taksidin ikinci kez gerçekleştirilememesi

Beklenen ana sonuç: Kart borcu, limit, ekstre ve taksitler kayıpsız ve mükerrersiz
çalışır.

Test notu — 13 Ağustos 2026:

- `Test Kart` için 600 TRY Groceries harcaması kart borcunu 600 TRY yaptı;
  200 TRY ödeme `Test Banka Güncel` hesabından yapıldığında kart borcu 400 TRY,
  kullanılabilir limit 4.600 TRY ve banka bakiyesi 2.700 TRY oldu.
- Kart ödemesi Ağustos giderini ve Groceries bütçe kullanımını ikinci kez
  artırmadı. Likit varlık 3.774,50 TRY, kart borcu 400 TRY ve net varlık
  3.374,50 TRY olarak tutarlı kaldı.
- Ağustos 2026 ekstresinde 600 TRY dönem harcaması/ekstre borcu, ödeme sonrası
  400 TRY kalan ve `Açık` durumu doğrulandı.
- `Test Kart` üzerinde Groceries kategorisinde 120 TRY / 3 taksit planı kuruldu;
  40 TRY tutarlı Ağustos, Eylül ve Ekim parçalarının toplamı tam 120 TRY oldu.
  Plan oluşturma tek başına finansal etki üretmedi. İlk taksit gerçekleştirilince
  kart borcu 440 TRY, Ağustos gideri ve Groceries harcananı 665,50 TRY oldu;
  aynı item tekrar gerçekleştirilemedi ve net varlık 3.334,50 TRY kaldı.

### 6. Planlama ve gelişmiş raporlar

- [x] Tekrarlayan gelir/gider planı oluşturma ve pasifleştirme
- [x] Vadesi gelen adayları üretme
- [x] Adayı gerçek işleme dönüştürme
- [x] Aynı dönemin iki kez üretilmemesi
- [x] 7/30/90 günlük yaklaşan ödeme görünümü
- [x] Net varlık, dönem neti ve gelecek ödeme yükü
- [x] Nakit akışı, bütçe sapması, hesap ve kart dağılımları

Beklenen ana sonuç: Planlanan kayıtlar açık onaydan önce finansı etkilemez ve
raporlar gerçek finansal kaynakları doğru sınıflandırır.

Envanter notu — 13 Ağustos 2026:

- Raporlar ekranı 1.174,50 TRY net varlık, 174,50 TRY dönem neti, 0,00 TRY kart
  borcu ve 0,00 TRY gelecek ödeme yükü gösterdi. Tekrarlayan plan bulunmadığı
  için yaklaşan yükümlülük listesi boştu.
- Aylık 50 TRY `Manuel tekrarlayan gider` planı için 13 Ağustos adayı üretildi;
  ikinci üretme isteği aynı dönem için yeni aday oluşturmadı. Açık onaydan önce
  finansal etki oluşmadı, onaydan sonra kayıt gerçek gidere dönüştü.
- İlk manuel kabulde gerçekleşen kayıt backend'de doğru olmasına rağmen Özet ve
  İşlemler ekranları manuel yenileme bekledi. `PlanningController` finansal
  değişim sinyaline bağlandı ve `TransactionsController` dışarıdan oluşan normal
  işlemleri dinleyecek şekilde düzeltildi. Düzeltme analyze ve 16 hedef testle
  doğrulanıp güncel APK emülatöre kuruldu; aynı aday ikinci kez onaylanmadı.
- İkinci aylık `Otomatik yenileme testi` planından 10 TRY gider onaylandı.
  İşlemler, Özet ve Bütçeler manuel yenileme olmadan sırasıyla yeni kayıt,
  725,50 TRY gider / -225,50 TRY net ve 725,50 TRY Groceries harcanan değerini
  gösterdi. Test planları daha sonra pasife alındı.
- İki plan yeniden aktif edilip referans 13 Ağustos ve ufuk 90 gün yapıldığında
  Yaklaşanlar'da görünmedi. Sorgu aktif `RecurringTransaction.NextOccurrenceDate`
  yerine yalnız önceden üretilmiş planned occurrence kayıtlarını okuduğu için
  gelecekteki Eylül/Ekim tekrarlarını öngöremiyordu. Aktif planları domain takvim
  kuralıyla vade ufkuna yansıtan düzeltme güncel APK'da manuel olarak doğrulandı.
- Nakit akışı grafiğinde yalnız Ağustos'un negatif neti kırmızı bir sütun olarak
  görünürken sıfır değerli önceki ayların sütunları ve ay ekseni görünmedi.
  Ekran görüntüsü `Grafiğin metin alternatifi` altında 03.2026–08.2026 arasındaki
  altı ayın tamamını doğruladı; Mart–Temmuz sıfır, Ağustos gelir/gider/neti
  500 / 725,50 / -225,50 TRY. Veri doğru ve erişilebilir metin tam; görsel
  grafikte ay/değer etiketleri olmadığı için okunabilirlik zayıftı. Her aya net
  tutar+tarih etiketi ve sıfır aylar için eksen noktası eklendi; güncel APK'da
  görsel grafik sorunsuz doğrulandı.

### 7. CSV import

Başlangıç notu — 13 Ağustos 2026: CSV dosyası seçilmeden önce uygulama açılışında
Flutter `_dependents.isEmpty` assertion ekranı görüldü. Session restore sırasında
router/provider kökünü yeniden kurmayan düzeltme yazıldı; analyze ve 86/86 widget
testi geçti. Güncel APK manuel kabulünden sonra aşağıdaki CSV maddelerine devam
edilecek.

- [x] CSV seçme, encoding/ayraç ve kolon eşleme
- [x] Geçerli ve bozuk satırların ön izlemesi
- [x] Hesap ve kategori eşleme
- [x] Hazır satırları içe aktarma
- [x] Aynı dosyayı tekrar yükleme
- [x] Duplicate adayı için “Atla” ve “Yine de içe al” kararları
- [x] Onay öncesinde bakiyenin değişmemesi

Beklenen ana sonuç: CSV önce staging alanında incelenir; yalnız onaylanan satırlar
işleme dönüşür ve tekrar yükleme mükerrer hareket üretmez.

Manuel kabul sonucu — 13 Ağustos 2026:

- Bir gider (`-12,50 TRY`), bir gelir (`+75 TRY`) ve bozuk tarihli satır ön
  izlemede sırasıyla valid, valid ve invalid göründü. Yalnız eşlenip onaylanan iki
  geçerli satır işleme dönüştü; batch `partially-imported` kaldı.
- Aynı dosya yeniden seçildiğinde yeni finansal hareket oluşmadı. Duplicate
  fixture'da bir aday atlandı, biri açık kararla yine de içe alındı. Başlangıç
  bakiyesine göre toplam net değişim `+50 TRY` oldu; bu yalnız bir ek `-12,50 TRY`
  duplicate giderinin yazıldığını doğruladı.
- UX bulgusu: başarılı onay sonrasında ekranda yalnız teknik satır durumları ve
  batch status'u görünüyor. İçe aktarılan/atlanan/hatalı sayısını özetleyen açık
  başarı mesajı ve “İşlemlerde görüntüle” eylemi bulunmadığı için kullanıcı
  işlemin tamamlanıp tamamlanmadığından emin olamıyor. Bu, finansal doğruluk
  hatası değil; manuel kabul sonrası toplu UX iyileştirme listesine alındı.

### 8. Borçlar ve tasarruf hedefleri

Başlangıç bulgusu — 13 Ağustos 2026: Alınacak planı oluşturma denemesinde genel
`Debt fields are invalid` cevabı görüldü. API'nin `receivable`, 60/60 TRY, sıfır
faiz ve iki taksiti kabul ettiği pozitif sözleşme testiyle doğrulandı. Flutter
formunun boş/geçersiz para metnini sessizce `0.0000` yapması kaldırıldı; ad,
pozitif anapara/toplam, toplam-anapara ilişkisi, faiz ve taksit alanları pencere
kapanmadan doğrulanıyor. Güncel APK manuel tekrar kabulünde 60 TRY, sıfır faizli,
iki taksitli alınacak planı oluşturuldu ve ilk 30 TRY taksit Test Nakit hesabına
başarıyla tahsil edildi.

Yanlış ilerleme kaynağıyla oluşturulan hedef için güvenli silme eklendi. Katkısı
olmayan current-user hedefi açık onayla silinir; manuel katkı geçmişi bulunan
hedef `409 Conflict` ile korunur, yabancı hedef `404` görünür. Güncel APK manuel
kabulü bekliyor.

- [x] Ödenecek borç ve alınacak alacak planı oluşturma
- [x] Taksit tarihleri ve yuvarlama toplamı
- [x] Borç/alacak taksidini ödeme veya tahsil etme
- [x] Borç hareketlerinin gelir/gider raporunu bozmaması
- [x] Manuel katkılı tasarruf hedefi
- [x] Hesap bakiyesine bağlı tasarruf hedefi
- [x] Aynı katkının tekrar gönderilmesinde tek kayıt oluşması
- [x] Hedef ilerlemesinde çifte sayım olmaması

Beklenen ana sonuç: Borç ve hedef ilerlemesi hesap bakiyesiyle tutarlı, fakat
gelir/gider sınıflandırmasından ayrıdır.

Manuel kabul sonucu — 13 Ağustos 2026:

- `Türkçe Acil Durum` manuel hedefinde 10/100 TRY, yüzde 10 ilerleme ve 90 TRY
  kalan tutar doğru gösterildi. Yenileme ve sekme geçişi katkıyı çoğaltmadı.
- Katkılı hedef silinmedi ve `Katkı geçmişi bulunan tasarruf hedefi silinemez.`
  mesajı Türkçe karakterleri bozulmadan gösterildi.
- Test Nakit hesabına bağlı hedef ilk bakiyeyi doğru aldı. İkinci 30 TRY alacak
  taksiti tahsil edilince hesap ve bağlı hedef 30 TRY arttı; manuel hedef 10 TRY
  kaldı. Aylık gelir, gider ve net değerleri değişmedi.
- Bu akış okuma/yenileme sırasında çifte sayım olmadığını doğruladı. Aynı
  `clientRequestId` ile ağ retry davranışı mobil arayüzden ayrıca sınanmadığı için
  idempotency maddesi açık bırakıldı.
- `Manuel Katkı Testi` hedefinde 1 TRY katkı için kaydet eylemine hızlıca iki
  kez basıldı; submit kilidi yalnız tek isteğe izin verdi ve ilerleme 1 TRY arttı.
  Backend `clientRequestId` tekrar korumasıyla birlikte idempotency maddesi kabul edildi.

### 9. Belgeler, export ve backup

- [x] İşleme PDF/JPEG/PNG ekleme
- [x] Geçersiz uzantı, imza ve büyük dosyanın reddedilmesi
- [x] Belge indirme ve Android paylaşım ekranı
- [x] Transaction CSV export
- [x] Finans JSON export
- [x] Versiyonlu backup oluşturma
- [x] Backup doğrulama
- [x] Açık onayla boş kullanıcıya restore
- [x] Bozuk restore denemesinde yarım veri kalmaması

Beklenen ana sonuç: Dosyalar sahiplik ve içerik sınırlarıyla korunur; backup
restore öncesi doğrulanır ve atomik uygulanır.

Manuel kabul bulgusu — 13 Ağustos 2026:

- Seçilen işleme PNG belge yüklendi, listelendi ve `Paylaş` eylemi Android paylaşım
  panelini açtı. İşlem CSV, finans JSON ve versiyonlu backup da doğru dosya adlarıyla
  paylaşım paneline ulaştı.
- Paylaşım panelinde yalnız Quick Share, Gmail ve Drive gibi genel hedefler
  görünür durumda. Uygulama içinde önizleme, dosya boyutu/oluşturulma zamanı/kayıt
  sayısı özeti veya açık `Cihaza kaydet` eylemi yok. Kullanıcı export'un içeriğini
  paylaşmadan doğrulayamıyor ve yerel Downloads klasörüne kaydetme yolu görünür
  değil.
- Teknik üretme/paylaşma maddeleri başarılıdır; ürün UX kabulü için CSV/JSON ön
  izleme veya özet, backup metadata özeti ve ayrı `Cihaza kaydet` eylemi düzeltme
  listesine alındı.
- Sentetik minimal PDF kabul edilip listelendi ve paylaşım paneline indirildi.
  PDF uzantılı fakat imzası eşleşmeyen dosya ile `/JavaScript` aktif içerik belirteci
  taşıyan PDF Türkçe güvenlik mesajlarıyla reddedildi; ikisi de belge listesine
  eklenmedi.
- İlk 6 MiB denemesi HTTP katmanında bağlantı kesildiği için yanlış genel ağ hatası
  gösterdi. Flutter'a 5 MiB ön kontrolü eklendikten sonra aynı dosya sunucuya
  gönderilmeden `Belge 5 MiB boyut sınırını aşıyor.` mesajıyla reddedildi; mevcut
  belgeler korundu. Birleşik belge türü/imza/içerik/boyut maddesi manuel kabul edildi.
- Uygulamanın oluşturduğu yedek, negatif CSV gider tutarındaki eksi işareti
  doğrulayıcı tarafından kabul edilmediği için reddedildi. Ondalık okuyucu
  `decimal(19,4)` ve metin kodlama sınırlarını koruyarak eksi işaretini kabul edecek
  şekilde düzeltildi. Negatif tutar içeren üret→doğrula→geri yükle regresyon testi
  4/4 geçti; emulator tekrarı bekleniyor.
- Yeni hesapta otomatik oluşan sekiz başlangıç kategorisinin boş hedef kontrolünü
  engellediği saptandı. Yalnızca eksiksiz, aktif ve değiştirilmemiş başlangıç seti
  geri yükleme transaction'ı içinde yedek kategorileriyle değiştiriliyor. Özel kategori
  ise hedefi dolu tutuyor ve korunuyor; veri taşınabilirlik testleri 5/5 geçti.
  Gerçek SQL testi bağlantı değişkeni olmadığı için skip edildi. Emulator
  kabulünde yeni sentetik hesaba restore tamamlandı; 2 hesap, 1 bütçe, 1 kart ve
  1 transfer beklenen biçimde göründü.
- Aynı yedek dolu hedefe ikinci kez uygulandığında Türkçe boş-hedef mesajıyla
  reddedildi ve ikinci kopyalar oluşmadı. Kopya veri koruması manuel kabul edildi.
- Checksum'u bilerek geçersizleştirilen yedek `Yedek veri bütünlüğü
  doğrulanamadı.` mesajıyla reddedildi. Doğrulama yazma ve boş-hedef kontrolünden
  önce sonlandı; bozuk restore/yarım veri bırakmama maddesi kabul edildi.
- Restore edilen iki attachment yeniden indirildi ve Android paylaşım ekranına
  verildi; dosya byte'larının yeni owner anahtarlarıyla geri geldiği kabul edildi.

### 10. Mobil kalite durumları

- [x] Loading durumu
- [x] Empty durumu
- [x] Error, retry ve oturumsuz geri dönüş koruması
- [x] Önceki veri varken stale hata görünümü
- [x] Telefon alt navigasyonu ve hızlı sekme geçişi
- [x] Geniş ekranda NavigationRail
- [x] Büyük metin ölçeği
- [x] Kritik semantics etiketleri ve ekran okuyucu
- [x] Kritik bilginin yalnız renk veya grafikle anlatılmaması

Manuel kabul bulgusu — 13 Ağustos 2026:

- API durdurulup Veri Araçları yenilendiğinde mevcut snapshot ekranda korundu ve
  bağlantı hatası banner olarak gösterildi. Ekrana API kapalıyken yeniden girildiğinde
  tam hata/retry durumu açıldı; API başlatılıp tekrar denendiğinde geri yüklenmiş
  veriler yeniden geldi. Stale-cache ve retry toparlanması kabul edildi.
- Hızlı alt menü geçişlerinde yanlış ekran veya hata görülmedi. Pixel 8 yatay
  konumda alt menü yerine solda NavigationRail gösterdi; dikey konuma dönüş korundu.
  En büyük yazı boyutunda kategori ve hesap adları satıra sarıldı, tutarlar ve
  eylemler kesilmedi. Çıkış sonrası geri tuşu/uygulamaya dönüşte eski finans
  verisi görünmedi.
- Veri olmayan `03.2026` bütçe ayında eski Ağustos verisi tutulmadı ve açıklayıcı
  boş durum gösterildi; `08.2026`ya dönüşte veri geri geldi. En büyük görüntü
  boyutunda form kaydırma ve eylemlere erişim korundu. Bütçe aşımı, negatif
  nakit akışı ve taksit durumu renk yanında metin/değerle de aktarıldı.
- Otomatik semantics kabulünde Özet loading durumu canlı bölge olarak
  `Finansal özet yükleniyor` mesajını sundu. Gelir, gider ve net kartları ad ile
  para değerini tek semantics düğümünde okudu; nakit akışı grafiği ay ve net
  değerlerden oluşan açıklayıcı etiketi korudu. Hedef testler 9/9, tüm Flutter
  paketi 95/95 geçti ve analyze temizdir. Manuel TalkBack tekrarı gerekmedi.

Kapanış otomasyon kanıtı: Domain 129/129, Application 86/86, Infrastructure
72/72 ve API 77/77 olmak üzere .NET Release toplamı 364/364 geçti. Infrastructure
22/22 ve API 1/1 gerçek SQL testleri yerel container'da skip olmadan çalıştı.
Flutter 95/95, `flutter analyze` ve debug APK build başarılıdır. Bu kontrol
listesinde açık test maddesi kalmadı.

Beklenen ana sonuç: Uygulama ağ ve veri durumlarını görünür gösterir; temel
erişilebilirlik ve responsive düzen korunur.

### Ortak Türkçe karakter kabul bulgusu — 13 Ağustos 2026

- [x] `charset` belirtmeyen UTF-8 ProblemDetails yanıtının Türkçe gösterilmesi
- [x] Ad ve açıklama alanlarının Türkçe klavye dilini istemesi
- [x] Güncel APK'da Türkçe karakterli sentetik ad ve açıklama girişi
- [x] Katkılı hedefi silme hatasının `Katkı geçmişi bulunan tasarruf hedefi silinemez.`
  olarak bozulmadan görünmesi

Kod doğrulamasında Flutter analyze temiz ve bütün test paketi 89/89 başarılıdır.
APK kabulünde emülatör klavyesinde Türkçe dili etkin olmalıdır.

### Türkçe kullanıcı metinleri — 13 Ağustos 2026

- [x] Yeni kullanıcı varsayılan kategori adlarının Türkçe üretilmesi
- [x] Mevcut İngilizce varsayılan kategori adlarının kimlikleri korunarak çevrilmesi
- [x] Eski rapor cevabındaki varsayılan kategori adının Flutter'da Türkçe gösterilmesi
- [x] CSV, borç ve hedef teknik durumlarının Türkçe gösterilmesi
- [x] Güncel API ve APK ile mevcut test hesabında kategori adlarının manuel kabulü

Beklenen adlar: `Maaş`, `Diğer Gelir`, `Market Alışverişi`, `Konut`, `Ulaşım`,
`Faturalar`, `Sağlık` ve `Eğlence`. Kullanıcının oluşturduğu özel kategori adları
otomatik olarak değiştirilmez.

## Bu çalışmada kapsam dışı

- Azure ve internete açık API
- E-posta doğrulama ve parola sıfırlama
- Offline cache ve tam offline senkronizasyon
- Canlı banka/Open Banking bağlantısı
- Yatırım ve gerçek çoklu para birimi
- Push bildirimleri ve mağaza yayını
- Şifreli production backup ve yönetilen malware taraması

## Yarın başlanacak tek küçük görev

Grup 1 — Kimlik ve oturum:

1. Yerel SQL Server ve API'nin çalıştığını doğrula.
2. Sentetik bir kullanıcıyla kayıt ve giriş yap.
3. Uygulamayı kapatıp açarak oturum geri yüklemeyi kontrol et.
4. Çıkış yapıp korumalı ekranların kapandığını doğrula.
