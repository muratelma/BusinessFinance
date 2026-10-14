# Kategori ve hesap sayfaları — tasarım düzeltmesi (13 Ekim 2026, gece)

Codex'in yarıda kalan düzeltme turundan sonra yapılan inceleme ve düzeltme.
Önceki denetim: [DIS-GOZ-2026-10-13-kategori-hesap-teslim-denetimi.md](DIS-GOZ-2026-10-13-kategori-hesap-teslim-denetimi.md).
Ölçüt: `design_handoff_secici/` teslimi — kareler ve kaynak dosyaları
(`screens-v9/*.jsx`). Dal `kategori-hesap-sayfalari`; **commit atılmadı**.

## Yöntem

1. 51 kare çalışma ağacından yeniden üretildi ve her biri referansıyla aynı
   ölçekte yan yana kondu (`scratch/kh-tasarim-duzeltmesi/karsilastirma/`).
2. Fark eyeballing ile değil, teslimin kaynak ölçüleriyle bulundu: JSX'teki
   her boşluk, yazı boyu ve satır yüksekliği Flutter kodundaki karşılığıyla
   karşılaştırıldı.
3. Düzeltmeden sonra kareler yeniden üretildi ve yeniden bakıldı.
4. Yeni APK emülatöre kuruldu; gerçek API (5284) ve `test4@test.test`
   verisiyle Hesaplar, hesap sayfası, `Tür` menüsü, ay seçici, hesap ayarları
   ve geri dönüşü, Kategori özeti, `Hesap / kart` menüsü, kategori sayfası ve
   Yeni transfer açıldı (`scratch/kh-tasarim-duzeltmesi/emulator/`).

## Kök neden

Görev belgesi gün sonunda alınan iki ölçü kararını taşımıyordu: **yan boşluk
16 dp** ve **harf aralığı yok**. Codex sayfaları Material'ın hazır çip,
düğme ve menü bileşenleriyle kurdu; bunlar kendi iç boşluklarını ve dokunma
payını getirir. Tema da boş bırakılan yazı kademelerinde Material'ın harf
aralığını devralır. Sonuç her sayfada birkaç dp'lik kaymaydı: çipler alçak ve
dar, alt yazılar geniş, kartlar uzun, satırlar farklı hizada.

## Düzeltilenler

| Yer | Ne yanlıştı | Ne yapıldı |
|---|---|---|
| Bütün yeni sayfalar | Alt yazı ve etiketler harf aralıklı | `AppUntrackedText`: sayfa, menü ve ay seçici harf aralıksız; para metni de |
| Başlık | Yazı −0,6 harf aralıklı ve dar; ay düğmesi 5 dp solda, oku 18 dp; `tune` 24 dp | Harf aralıksız 26/700; ay düğmesi 48 dp, 10/6 boşluk, 20 dp ok; `tune` 22 dp |
| **Ana sekme başlıkları** (Özet, İşlemler, Kasa, Diğer) | Codex başlığı sütuna çevirince yazı üste yaslanmış, 16 dp yukarı kaymıştı (commit `97bce47`'da) | Başlık yeniden ortalandı |
| Çip ve menü düğmesi | Material çipi: 34 dp, dar, kendi dokunma payıyla; seçeneği kalmayan düğmenin kesik kenarı 48 dp çiziliyordu | Tek ortak hap: 36 dp, 14 dp yan boşluk, satırı 48 dp ve dokunma alanı o satırın tamamı |
| Menü | Başlığın üstünde fazladan 4 dp; ara başlıktan önce boşluk yok; ödeme satırlarında genel ok ikonu | Teslimin boşlukları; ödeme ve tahsilat satırı listede taşıdığı ikonu taşır |
| Menü + geri tuşu | Menü açıkken geri tuşu **sayfayı** kapatıyordu (emülatörde görüldü) | Geri tuşu menüyü kapatır |
| Özet kartı | 1,45 satır yüksekliği, alt hücreler 16 dp aşağıda; kart 5–6 dp uzun | 14/1,35 etiket, 28/1,25 tutar, hücreler 12 dp aşağıda |
| Kategori satırı | Parçalar arası 14/12/8 dp; çubuk koyu kırmızı (metin tonu) | 16 dp aralar, sağ sütun 112 dp; çubuk `expenseFill` / `incomeFill`, en az yüzde üç |
| `Kategorisi yok` | Kesik kenar soluk; üstündeki not gövde yazısıyla | Kenar `borderStrong`; not 13 dp soluk, çubuk gri |
| Gün başlığı | Üst 18 dp; tarih ile gün arası iki boşluk karakteri | Üst 20 dp, arası 8 dp |
| Hesap satırları | Beyaz blokların üst ve alt çizgisi yoktu | Çizgiler geldi; iç çizgi 72 dp'den (kartta 70) |
| Kartlı satış (B8) | `Yolda · 26 Ekim bekleniyor` ayrı mavi satır | Alt yazıda: `Ziraat Vadesiz · Yolda` / `· Hesaba geçti` |
| Parça satırları (B6) | POS'suz satışta hesap alt yazıya iniyor, satır iki satıra taşıyordu; yatışta hesap adı başlığa gelmiyordu | `POS satışı · Ziraat Vadesiz`, `POS yatışı · Ziraat Vadesiz`; alt yazı yalnız `₺… satışın komisyonu` |
| Durum satırı | `Ödendi · 20 Ekim · …` uzun ay | Kısa ay: `20 Eki`; Yolda'da `26 Eki bekleniyor` |
| Yolda kutusu | Kutunun ve içindeki satırın kenarlığı vardı; `Tümünü gör … ›` düz karakterle | Kenarlıksız mavi kutu, beyaz satır 16 dp yarıçap, ok ikonu |
| Seçim başlığı (H7) | 56 dp satır | 16 dp üst boşluk, 16/600 |
| Boş hâl | Eylem düz düğme; `Eylül ayında … yok` | Altı çizili 15/600; `Eylül'de bu hesapta hareket yok` |
| Bilgi notu (B4b, D3b) | 8 dp yarıçap, 16 dp iç boşluk | 16 dp yarıçap, 10/14; toplam tutar kalın |
| Hesaplar | Düğme 47 dp; grup toplamı koyu; `Pasif hesaplar` altında `Toplam bakiyeye dahildir` | 52 dp düğme; toplam soluk; pasif satır alt yazısız, bakiye sıfır değilse tutarı yazar (görev §5.2) |
| Yeni transfer | Alanlar arası 20 dp; alt çubuk çizgisiz; bilgi cümlesi çeviri kokuyordu | 16 dp; çizgi ve 12/16/20 boşluk; "… hesabından toplam **₺…** çıkar. Masraf … olarak gider yazılır." |
| Kategori sayfası (cihazda) | Başlık `ÖRNEK ELEKTRİK DAĞITIM A.Ş.`, alt yazı `Vadeli · Örnek Elektrik Dağıtım A.Ş.`: aynı ad iki kez | Ad harf büyüklüğüne bakılmadan karşılaştırılır; tekrar yazılmaz |

## Kareyle bilerek farklı kalanlar

| Fark | Neden |
|---|---|
| `POS satışı` (karede `Kartlı satış`) | İşlem adları tek sözlükten gelir; ad değişikliği `docs/backlog.md` 13'ün konusu |
| Eksi işareti `-` (karede `−`) | `AppMoneyText` bütün uygulamada `-` yazar; değiştirmek uygulama geneli bir karar ve çok sayıda testi etkiler. **Karar bekliyor** |
| `Kategorisi yok` alt yazısı, A8c/A8d | Görev §5.1: kategori atama kurulmaz |
| `Pasif hesaplar` satırı | Görev §5.2 |
| Boş hâlde hesap adına ek (`Bonus Kart'tan`) | Ad kullanıcının yazdığı serbest metindir; ek güvenle türetilemez. `… için gider yok` yazılır |
| İşlem ayrıntısı (D1, D2, D2b) | Görev §5.4: İşlemler'deki ortak panelin kendisidir; ikonlu satırları, `Gerçekleşti` rozeti ve nötr iptal düğmesi o panelin bugünkü dilidir. **Kare ile aynı değil** |
| İşlemler satırında `masraf ₺50,00` (karede `+ ₺50,00 masraf`) | Tutarın altı yazısı `komisyon ₺…` kalıbını izler (5 Ekim kararı K2) |
| `Vadeli` menü ikonu | Karedeki `event_upcoming` Flutter'ın ikon setinde yok; en yakını kullanıldı |
| Diğer (K0) profil kartı ve satır yazısı | Sayfanın bugünkü hâli; bu işte yalnız grupları değişti |
| Yeni transfer alanları | Uygulamanın ortak form alanları (para alanında ayrı `₺` öneki, küçük etiket) |
| Örnek sayılar, sıra, hafta günü | Kare uydurma veriyle çizildi; sunucu cevabı gösterilir |

Görüntü testinin yazı tipi 600 ağırlığı taşımıyor (Roboto Medium ve Bold
yüklü), bu yüzden test karelerinde 16/600 yazılar cihazdakinden biraz geniş
çıkar ve `Faiz ve finansman gideri` karede farklı yerden kırılır. Cihazda
yazı gerçek ağırlığıyla çizilir. İşlemler karesindeki `□` da aynı nedenle:
test yazı tipinde ok işareti yok, cihazda var.

## Kontroller

| Kontrol | Sonuç |
|---|---|
| `dart format --set-exit-if-changed lib test` | 358 dosya, 0 değişiklik |
| `flutter analyze` | Temiz |
| `flutter test` | **1.169 geçti**, 180 görüntü testi normal koşuda atlanır |
| Görüntü koşusu (`SCREENSHOT_DIR`) | 51/51 çizildi ve referansla yan yana bakıldı |
| Debug APK | Derlendi, emülatöre kuruldu |
| Backend Release build / format | 0 uyarı, 0 hata / temiz |
| Backend testleri (gerçek SQL) | **1.386 geçti**, 2 canlı sağlayıcı testi atlandı (Domain 402 · Application 400 · API 327 · Infrastructure 257) |

Bozulan ve düzeltilen kapılar: boşluk token kapısı (ham 14/10/6/1 dp →
`AppSpacing` adlarının toplamı), hesap sayfasının 2× yazı kapısı (Yolda
satırı büyük yazıda alt alta), pasif hesap testi (yeni satır biçimi), hesap
ekleme rotası testi (`Hesap ekle` adı).

## Bakmadıklarım

- Koyu temada ve 2× yazıda kareleri gözle karşılaştırmadım; yalnız taşma
  kapıları koştu.
- Emülatörde transfer **kaydetmedim**, hesap ayarlarında bir şey
  değiştirmedim, `Yolda` sayfasını ve geçmiş ayı açmadım.
- TalkBack ile okuma yapmadım.
- Codex'in işinin para tarafını (migration, yedek, iki kanonik liste,
  transfer masrafının iptali) bu oturumda **okumadım**; yalnız testlerini
  koştum. `DEVIR-2026-10-13.md` §2'deki kod incelemesi duruyor.

## Ek — deneme verisi (13 Ekim, gece)

Kullanıcının isteğiyle `test4@test.test` hesabına 5284'teki API üzerinden 59
sentetik kayıt yazıldı (Ağustos, Eylül ve 1–13 Ekim; ileri tarihli kayıt yok).
Yeni hesap `Garanti Vadesiz`, yeni kişiler `Test Toptancı`, `Örnek Banka`,
`Ahmet Usta`, `Kemal Bey`. Betikler `scratch/kh-tasarim-duzeltmesi/seed.py`
(yazar; ikinci kez çalışmaz) ve `verify.py` (okur ve toplamları denetler);
parola ortam değişkeninden okunur. Yazmadan sonra 19 toplam denetimi tuttu:
kategori satırları = kart, menü satırları = `Hepsi`, hesap satırları = giren −
çıkan, Hesaplar hücreleri = toplam.

Gerçek veriyle bir fark daha bulundu ve düzeltildi: POS'un adı varken komisyon
satırının alt yazısına hesap adı da ekleniyor, satır iki satıra taşıyordu.
Komisyon, kesinti ve masraf satırında alt yazı artık yalnız bağlı kaydın
tutarını söyler (kare B6). Bu düzeltmeden sonra Flutter 1.169 test yeniden
geçti ve APK yeniden kuruldu.

## Ek — ölçüt düzeltmesi (kullanıcı, 13 Ekim gece)

Kullanıcı: kareyle birebir aynılık amaç değildir; uygulamaya uyan, öbür
kurallarla çakışmayan ve daha iyi duran seçilir. Bu ölçütle beş küçük fark
yeniden değerlendirildi:

| Fark | Seçilen | Gerekçe |
|---|---|---|
| Birkaç satışı kapatan yatışın alt yazısı | `Ziraat POS · 3 tahsilat · 15–17 Eyl` | Sıra ve kısa aralık karedeki gibi; kelime `tahsilat` kalır, çünkü yatış kartla tahsilatı da kapatır (karedeki `satış` o durumda yanlış olurdu) |
| Yolda sayfasında alt yazıdaki POS adı | Kalır (`Ziraat POS · 26 Eki bekleniyor`) | Aynı hesaba birkaç POS'tan para geliyorsa satırı ayırt eden tek bilgi; İşlemler'de de satırı POS tanıtır |
| Transfer masrafı başlığındaki ok | İkon kalır, iki yanına boşluk geldi | `→` karakteri yazı tipinde yok, yedek yazı tipinden gelir; ikon her cihazda aynı çizilir |
| Transfer bilgi notunun sonundaki `(İşletme)` | Kalktı | Kapsamı arayüzde görmeyen (işletmesi olmayan) kullanıcıya da `(Şahsi)` yazıyordu; kapsam görünürlüğü kuralıyla çakışıyordu |
| `Tür` menüsünde ödeme sırası | Yapılmadı | Büyükten küçüğe dizmek daha iyi (Kategori özeti de öyle dizer) ama sıra sunucuda verilmeli; API'nin yeniden başlatılmasını ister, sıradaki backend işiyle yapılır |

## Ek — 14 Ekim: kullanıcının dört maddesi

1. **Yolda:** başlık POS'un adı, alt yazı yalnız gün; dip not bilgi kutusu.
2. **Ay seçici:** `Bu ay` sola alındı.
3. **Ay seçicinin üst yazısı:** sayfanın adıyla başlar.
4. **Hesaplar:** kullanıcının yeni tasarımı (HS1e) uygulandı. Pay çubuğu ve
   yüzdeler için sunucuya `cashShare` / `bankShare` / `inactiveShare` eklendi
   (var olan cevabı kırmaz); geçici bir API örneğinde (5291, aynı geliştirme
   verisi) cihazda doğrulandı: %58 / %29 / %13, tasarımdaki sayılarla aynı.

Yolda bulunanlar:

- **Harf aralığı sarmalayıcısı sayfanın yarısına işlemiyordu.** Kategori
  satırı, hesap satırı ve Yolda kutusu temayı sarmalayıcının üstündeki
  bağlamdan okuyordu. Üç sayfa sarmalayıcının tamamen altına alındı.
- **Kasa sayımı testleri gece 00:00–03:00 arasında kırılıyor.** On test
  "bugün"ü UTC'den okuyordu; sunucunun günü Türkiye günü olduğu için o saatlerde
  sayım "dün"e yazılıyor ve test boş cevap alıyordu. Testler artık
  `LocalDay.Today` kullanır. Ürün kodunda hata yok; yalnız testin saati.

Kontroller: Flutter 1.169 test, analyze ve format temiz. Backend build 0
uyarı, format temiz; Domain 402, Application 400, Infrastructure 257 (pay
alanıyla) ve API 327 test geçti (API, test düzeltmesinden sonra yeniden koştu).

Kullanıcının yapması gereken: **5284'teki API'yi yeniden başlatmak.** Çalışan
süreç eski kodu taşıyor ve pay alanlarını göndermiyor; yeniden başlayana kadar
Hesaplar'da pay çubuğu ve yüzdeler görünmez (sayfa bozulmaz, tutarlar görünür).
