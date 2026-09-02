# Uygulama Gözlem Formu — Bluecoins

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Bluecoins Finance & Budget / Mabuhay Software |
| Sürüm | 13.1.45 (`versionCode=33111`) |
| Test tarihi | 1 Eylül 2026 |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | Türkçe / TRY |
| Hesap veya plan türü | Yerel ücretsiz sürüm; reklam ve uygulama içi satın alma içeriyor |
| Erişim kısıtı | Kayıt/giriş yok; PDF/yazıcı, CSV ve HTML dışa aktarma menüsü Premium yükseltme yüzeyinde gösterildi |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Tamamlandı | 2 | Kayıt, e-posta veya telefon istemiyor; Türkçe dil seçili geliyor. İlk ekranda “İlk İşlemi Ekle” ve örnek veriyle “Demo Dosyasını Dene” yolları ayrılmış | Demo önerisi ilk `+` dokunuşunda yeniden soruluyor; gerçek test verisiyle karışmaması için reddedildi | `00-magaza.png`, `01-ilk-acilis.png` |
| K01 Ana ekran | Tamamlandı | 0 | Hesaplar / İşlemler / Hatırlatıcılar / Tümünü göster yatay geçişleri; günlük özet, bütçe ve takvim gibi kartlar; kart listesi kullanıcı tarafından düzenlenebiliyor | İlk bakış yoğun; ekrandaki kartların bir kısmı aynı dönemde “işlem yok” mesajını tekrar ediyor | `02-bos-ana-ekran.png`, `09-ozgun-ozellik.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı | ~8/hesap | Açılış bakiyesi **ve açılış tarihi** aynı formda. Banka, nakit, kredi kartı, kredi, ipotek, alacak, yatırım, sanal hesap ve dış varlık dahil geniş tür ağacı var | Uygulama Birikimler, Cüzdan, Kredi Kartı ve Ev İpoteği gibi birçok sıfır bakiyeli örnek hesabı hazır getiriyor; hesap seçici kalabalıklaşıyor | `03-dolu-ana-ekran.png` |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~9 | Tek formda ad, tarih/saat, planlama, tutar, para birimi, kategori, hesap, bölme, durum, etiket ve not var; ayrı `GELİR` türü | Form yoğun ve varsayılan `GİDER`; işletme hizmet geliri kategorisi/kapsamı yok, `Diğer` kullanıldı | `04-islem-formu.png` |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~8 | İşlem adı listede birincil başlık; kategori ve hesap ikinci satırlarda. Tutar ve tarih aynı formda değişiyor | **İşletme/şahsi kapsam alanı yok.** Etiketle taklit edilmedi; bu boyut `Desteklenmiyor` | `05-siniflandirma.png` |
| K05 İşletme kart gideri | Tamamlandı | ~9 | Kredi kartı hesabı seçilince `Taksit şartlarını seçin` alanı beliriyor; ₺1.200 gider harcama tarihinde rapora girdi ve kart bakiyesi −₺1.200 oldu | İşletme kapsamı ve tedarikçi alanı yok; tedarikçi ancak ad/not/etiketle taşınabilir | `06-islem-listesi.png` |
| K06 Transfer | Tamamlandı | ~10 | Ayrı `TRANSFER`; kaynak/hedef, `Takas` ve transfer ücreti alanları var. Listede iki bağlı satır ve her iki hesabın işlem sonrası bakiyesi gösteriliyor. Ana Hesap→İş Kartı transferi kartı sıfırladı ve gün toplamı ₺0 kaldı | Kaynak ve hedef aynı varsayılan hesapla açılıyor; iki hesap da ayrı ayrı yeniden seçilmeli. Aynı olayın iki satırı ilk bakışta çift kayıt sanılabilir | `06-islem-listesi.png` |
| K07 Liste ve rapor | Tamamlandı | ~4 | Liste tarih/gün netiyle gruplanıyor; her satırda işlem sonrası hesap bakiyesi var. Raporlar dönem karşılaştırması yapıyor: Ağustos gideri −₺2.050; hesap raporunda toplam varlık ₺44.950. Filtre ve yazdır/dışa aktar yüzeyi mevcut | Rapor sekmeleri yatay ve çok sayıda; bazıları ekran dışında kalıyor. CSV/PDF/HTML yüzeyi Premium yükseltme başlığı altında açıldı | `06-islem-listesi.png`, `07-rapor.png` |
| K08 Düzeltme/iptal | Tamamlandı | ~4 | Satır → detay → Düzenle ile aynı form açılıyor; kart giderinin tarihi bu yolla düzeltildi. Silmede Türkçe onay var | Düzenleme kaydı yerinde değiştiriyor; silme kalıcı ve void/iptal/geri alma yok. **Sıfır tutarlı gider uyarısız kaydedildi**, sonra Sil→Tamam ile temizlendi | `08-hata-veya-bos-durum.png` |

## Kontrol değeri doğrulaması

| Değer | Beklenen | Bluecoins | Durum |
|---|---:|---:|---|
| Ana Hesap bakiyesi | 40.800,00 | ₺40.800,00 | ✓ |
| Ortak Cüzdan bakiyesi | 4.150,00 | ₺4.150,00 | ✓ |
| İş Kartı bakiyesi/borcu | 0,00 | ₺0,00 | ✓ |
| Toplam net varlık | 44.950,00 | ₺44.950,00 | ✓ |
| Gelir işlemi | 25.000,00 | ₺25.000,00 | ✓ |
| Gider toplamı | 2.050,00 | −₺2.050,00 | ✓ |

Transfer ve kart ödemesi listede kaynak/hedef için ayrı satırlar oluşturdu,
fakat tarih başlığındaki toplamı ve gider raporunu ikinci kez etkilemedi.

## Arayüz taraması (görev dışı, ~10 dk)

| Alan | Gezildi mi | Kısa gözlem |
|---|---|---|
| Tüm ana sekmeler / alt görünümler | ✓ | Hesaplar, İşlemler, Hatırlatıcılar, Tümünü göster, Bütçe, Net Kazançlar, Öğeler Özeti ve Etiketler görüldü; yatay sekme listesi ekranı aşacak kadar geniş | — |
| Bir raporun içine tıklama | ✓ | Net Kazançlar dönem karşılaştırması ve Tümünü göster hesap/varlık raporu açıldı; satırlar hesap türü→hesap hiyerarşisinde açılıyor | `07-rapor.png` |
| Bütçe / hedef / planlama ekranı | Kısmen | Bütçe Özeti, Hatırlatıcılar ve işlem formunda `Planlı İşlemler` alanı görüldü; derin oluşturma Tur 2'ye bırakıldı | — |
| Ayarların derinliği | Kısmen | Ana ekran kartları: Günlük Özet, Takvim, Bütçe Özeti, Net Kazançlar, Kredi Kartı Özeti, Net Kazanç, Nakit Akışı ve Favori Hesaplar. Dört favori hesap yuvası var | `09-ozgun-ozellik.png` |
| Arama ve filtre davranışı | Kısmen | Hesap seçicide arama; raporlarda Filter; işlem listesinin serbest metin araması bu turda denenmedi | — |
| Boş durum ekranları | ✓ | İlk kurulum “İlk İşlemi Ekle” / “Demo Dosyasını Dene” diye iki yön veriyor; dashboard kartları dönem boşsa açıkça söylüyor | `01-ilk-acilis.png`, `02-bos-ana-ekran.png` |
| Hata / uç durum | ✓ | Sıfır tutar **hata vermeden kaydedildi**. Silme bottom sheet'i “Silmek istediğinize emin misiniz?” + İptal/Tamam sunuyor | `08-hata-veya-bos-durum.png` |
| Widget / hızlı giriş / kısayol | Kısmen | Ana ekranda sabit `+`; düzenlenebilir dashboard kartları var. Android ana ekran widget'ı denenmedi | `09-ozgun-ozellik.png` |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Dashboard kart tabanlı; işlem listesi gün toplamı → işlem adı/tutar → kategori/hesap → işlem sonrası bakiye hiyerarşisi kuruyor |
| Alt/üst gezinme | Alt bar yerine ekran üstünde yatay ve kaydırılabilir çok sayıda sekme; sağ altta bağlama göre `+`. Sekmeler büyüdükçe hedefler ekran dışına taşıyor |
| Renklerin anlamı ve tutarlılığı | Gelir yeşil, gider pembe/kırmızı, transfer mavi; seçili tür dolu arka planla ve tutar alanındaki +/− işaretiyle destekleniyor |
| Tipografi ve para değerlerinin okunması | Para sağda, iki ondalık ve ₺ ile; günlük toplam ile satır tutarı ayrışıyor. Çok sayıda küçük ikincil metin yoğunluk yaratıyor |
| Kart, liste ve grafik kullanımı | Dashboard kartları; tarih gruplu işlem listesi; iki dönem sütunlu rapor. Varlık raporu tür ve hesap düzeyinde açılıyor |
| Form alanları ve varsayılanlar | Tek ekran çok güçlü ama yoğun. Gider varsayılan; kart seçimi taksit alanını dinamik ekliyor; transferde kaynak/hedef + kur/takas + ücret var |
| Loading, boş, hata ve başarı geri bildirimi | Boş durumlar açıklayıcı; kayıt başarısı sessizce önceki ekrana dönüyor. Sıfır tutar doğrulaması yok; silme onaylı ama geri alınamaz |
| Erişilebilirlik / dokunma alanları / metin yoğunluğu | Dokunma hedefleri genelde büyük; ancak yatay sekme keşfi, yoğun tek form ve bazı çeviriler (`Yinelenmek`, `Takas`) bilişsel yük yaratıyor |

## Akış özeti

- En kısa ve güçlü akış: Tek ekranda ayrıntılı işlem oluşturma; tarih, planlama, bölme ve etiket başka bir ayrıntı sayfasına dağılmıyor.
- En fazla sürtünme yaratan akış: Hesap seçimi — hazır gelen çok sayıda sıfır hesap arasında kaynak/hedefi iki ayrı bottom sheet'ten bulmak.
- Uygulamanın hedef kullanıcı varsayımı: Finansal model ayrıntısı isteyen ileri seviye kişisel bütçe kullanıcısı; çok hesap, kredi, ipotek ve rapor kullanıyor.
- İşletme ve şahsi para yaklaşımı: **Yok.** Kategori/etiket mevcut ama özel kapsam boyutu değil.
- Transfer ve kart ödemesi yaklaşımı: Ayrı transfer türü, çift bağlı satır, gün toplamında sıfır; kart ödemesi ikinci kez gider değil.
- Planlama, borç ve tahsilat yaklaşımı: Planlı işlem, hatırlatıcı, kredi/ipotek ve taksit alanları var; cari hesap tür adı görülse de işletme carisi/tahsilat bağı Tur 1'de doğrulanmadı.

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşlem satırında işlem sonrası hesap bakiyesi | Uyarlayarak al | Kullanıcı hareketin etkisini anında görüyor; birleşik feed'de isteğe bağlı ikincil bilgi olabilir | Aktivite feed'i |
| Gün başlığında o günün neti | Uyarlayarak al | Gelir/gider/transfer yoğunluğunu gün düzeyinde özetliyor; transferlerin sıfır etkisi görünür oluyor | Aktivite feed'i |
| Açılış bakiyesi + açılış tarihi aynı hesap formunda | Doğrudan al / mevcut yaklaşımı doğrular | Açılış bakiyesi raporu kirletmeden zaman bağlamı kazanıyor | Hesap oluşturma |
| Dinamik kart alanı (`Taksit şartlarını seçin`) | Uyarlayarak al | Yalnız seçilen hesap türüne ilişkin alanları açmak form yoğunluğunu azaltır | İşlem/kart formu |
| Kullanıcı tarafından düzenlenen dashboard kartları | Uyarlayarak al | Farklı kullanıcı önceliklerine uyar; ancak ilk sürümde az sayıda anlamlı ön ayar yeterli | Ana ekran |
| Tek ekrana bütün ayrıntıları yığmak | Alma / dikkat | Güçlü ama esnaf için fazla yoğun; temel alanlar üstte, vergi/etiket/not gibi ayrıntılar kontrollü açılmalı | İşlem formu |
| Çok geniş hesap türü ve hazır örnek hesap listesi | Alma | Kredi/ipotek/yatırım kapsamı çekirdek işi gömer; BusinessFinance yalnız kendi domain türlerini göstermeli | Hesap listesi ve oluşturma |
| Sıfır tutarı kabul etmek | Alma | Anlamsız finansal hareket oluşturuyor; istemci ve sunucu birlikte reddetmeli | Tüm para formları |
| Doğrudan düzenleme + kalıcı silme | Alma | Finansal geçmiş korunmuyor; BusinessFinance düzeltme ve iptal kaydı kullanır | Kayıt detayı |
| Transferi iki bağlı satır ve iki running balance ile gösterme | Uyarlayarak al | Kaynak/hedef etkisi güçlü; tek olay oldukları görsel bağla daha açık tutulmalı | Transfer detayı ve feed |
| İşletme/şahsi boyutunun olmaması | Alma | ADR 0013'ün temel ihtiyacını karşılamıyor | İşlem formu ve rapor filtresi |

## Kanıt ve güven düzeyi

- Manuel gözlem: K00–K08 emülatörde sentetik veriyle tamamlandı; bütün bakiye kontrol değerleri birebir tuttu.
- Resmî kaynak: —
- Yorum: Bluecoins para modeli BusinessFinance'e güçlü bir teknik referans, fakat bilgi mimarisi hedef esnaf için gereğinden geniş.
- Doğrulanamadı: Tur 2 planlı/taksitli/cari derin akışlar, yedek/geri yükleme, Android widget'ı ve Premium dışa aktarmanın son adımı.

## Tek cümlelik sonuç

Bluecoins açılış bakiyesi, running balance, nötr transfer ve dinamik kart/taksit alanlarıyla en güçlü para modeli referansı; ancak yoğun formu, geniş hesap evreni, sıfır tutarı kabul etmesi ve silme yaklaşımı BusinessFinance için sadeleştirilmesi gereken negatif örnekler.
