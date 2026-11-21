# Planlananlar, tekrarlayanlar ve abonelikler — ortak çalışma

Son güncelleme: 21 Kasım 2026. **Özet’in backend ve Flutter değişiklikleri doğrulandı. Sırada kullanıcının emülatör incelemesi var.**

### Oturum günlüğü · 21 Kasım 2026

**İlk emülatör incelemesinden sonraki yerleşim düzeltmesi.** Kullanıcı Varlık
durumu’nun fazla yüksek olduğunu ve bütçe kutularındaki çizginin seçilmediğini
bildirdi. Aynı İşletme verisiyle, 412 dp genişlikte Varlık durumu kartı
yaklaşık 549 dp’den 399 dp’ye, dört kutulu Bütçeler 258 dp’den 211 dp’ye
indi. Varlık tutarları aynı sağ kenarda, POS oku adın yanında. Bütçe
kutularında halka ve tutarlar aynı tabana oturur. Kalan/Aşım çizgisi açık
temada koyu, koyu temada açıktır. Tek bütçenin yatay düzeni sürer.
[Varlık önce](../../tasarim-onizleme/ozet-yerlesim-2026-11-21/ONCE/varlik.png) ·
[Varlık sonra](../../tasarim-onizleme/ozet-yerlesim-2026-11-21/SONRA/varlik.png) ·
[Bütçeler önce](../../tasarim-onizleme/ozet-yerlesim-2026-11-21/ONCE/butceler.png) ·
[Bütçeler sonra](../../tasarim-onizleme/ozet-yerlesim-2026-11-21/SONRA/butceler.png).
Flutter analiz, 73 ilgili test, iki görsel kontrol ve Android derlemesi geçti.
Düzeltme APK’sı Pixel 8’e veri korunarak kuruldu, uygulama açıldı ve API
hazırlık kontrolü 200 döndü. Sıradaki görev kullanıcının yeni yerleşimi incelemesi.

**Son kodlama checkpoint’i.** Dal `ozet-tamamlama`. İlk commit `b2e29c7`,
Özet teslimi Git geçmişinde `feat: align dashboard payments and planned activity with the selected week`
adıyla bulunur. Sonraki oturumda önce kullanıcının emülatör bulgularına bakılır.
Özet kabulünden sonra Tekrarlayanlar ve Planlananlar’ın geri kalan akışları ele alınır.

- Flutter analyze temiz, Özet/Bütçeler/Planlananlar ve mimari kontrollerinde
  300 test geçti. Beş sentetik ekran görüntüsü üretildi.
  [Bütçeler ve hafta](../../tasarim-onizleme/ozet-uygulama-2026-11-21/04-ozet-alt.png) ·
  [Varlık durumu](../../tasarim-onizleme/ozet-uygulama-2026-11-21/04b-ozet-en-alt.png).
- Backend çözümü derlendi ve format kontrolü geçti. SQL dahil koşuda Domain
  416, Application 422 ve API 399 test geçti. Yeni karma ekstre SQL testi
  ayrıca geçti. Infrastructure turunda 281 test geçti, iki test atlandı.
  Eski yaklaşan ödemeler akışının kart taksitlerini göstermesi korununca
  ilk turdaki üç başarısız SQL testi de yeniden koşulup geçti.
- Son debug APK, `API_BASE_URL=http://10.0.2.2:5284` ile derlendi.
  Pixel 8’e `adb -s emulator-5554 install -r` ile kuruldu ve uygulama açıldı.
  Yerel API `http` profilinde hazır olma kontrolüne HTTP 200 döndürdü.

- Kullanıcı Özet’in backend ve Flutter geliştirmesini bu oturumda Codex ile sürdürme kararı aldı. Codex ve Claude sonraki oturuma bu günlükten devam eder.
- Kategori giderlerinde mevcut halka, dört kategori ve varsa Diğer onaylandı. Reddedilen yatay şeridin iki adet 18 numaralı önizlemesi kaldırıldı. Geçerli görsel aşağıdaki 02 numaralı ekran kaydıdır.
- Kullanıcının talimatıyla `ozet-tamamlama` dalı `borc-alacak-cari` dalının `d359ab7` commit’inden açıldı. Bu noktada kaynak dal yerel `main` dalından beş commit ilerideydi. Önceden bulunan çalışma dosyaları korundu.
- Flutter Özet ekranında Bütçeler başlığı seçili rapordaki tüm bütçeleri sayıyor. Üç bütçede en dolu iki halka gösteriliyor. Mevcut widget testinin başlık beklentisi üç olarak güncellendi.
- Bütçe sayısı düzeltmesinin kontrolleri geçti. `flutter analyze --no-pub` sorun bulmadı, `dashboard_sections_test.dart` içindeki 37 test geçti ve `flutter build web --no-pub` başarılı oldu. Bu ilk commit Git geçmişinde `fix: show the total budget count in the dashboard header` adıyla bulunur. 16/17 numaralı bütçe kutularının yerleşimi henüz uygulanmadı.
- Yedi günlük okuma backend ve Flutter’da birlikte değişti. Haftalık istemci `countTodayInHorizon=true` gönderir. Sunucu ilk günü sayar, eski istemci parametre göndermezse önceki bitiş tarihi geçerlidir. Backend build ve format, Flutter analyze, kapsam ve Özet testleri geçti. API yerel HTTP profiliyle açıldı ve hazır olma kontrolü 200 döndü.
- Emülatör `Pixel_8` / `emulator-5554`. Haftalık okuma düzeltmesini içeren debug APK veri korunarak kuruldu ve açıldı. Kullanıcının güncel kararı: geliştirme sırasında APK kurulmaz. Sonraki kurulum Özet’in tüm işleri ve kontrolleri bitince yapılır. Kullanıcı en son o sürümü inceleyecek.
- Özet’te 16/17 numaralı bütçe yerleşimleri, 08 numaralı haftalık vade önizlemesi ve 11 numaralı Varlık durumu kodlandı. Planlananlar’ın tarih girişi Özet’in okuduğu günü taşır, gecikme girişi vadesi geçmiş ödemeleri ilk bölümde açar. Kişinin hesap vadeleri haftalık okumaya dahil edildi.
- Karma ekstre ölçütü (kullanıcının seçimi geliştirene bıraktığı 21 Kasım kararı): kesilmiş ve açık kart borcunu oluşturan hareketler kullanılır. Kartın varsayılan kapsamı kullanılmaz. Borç gün sonunda kapanınca önceki kapsamlar temizlenir. Kısmi ödemede kapsam tahmini yapılmaz. İki kapsamın bulunduğu ekstre, iki filtrede de tam kalanıyla yer alır ve “Karma ekstre” olarak görünür. Kapanış sonrasındaki yeni harcama önceki ekstreyi değiştirmez.
- Abonelik ve ücretsiz deneme olaylarının bağımsız veri modeli sonraki aşamanın bağımlılığıdır. Mevcut hesap/kart kaynaklı tekrarlayan vadeler kullanılır. Özet’te sahte abonelik, hatırlatma veya deneme bitişi üretilmez.

## Oturumu devralan ajan için

### 21 Kasım — buradan devam et

**Bütçeler — onaylanan tasarım.** Çoklu kutularda [17’nin sağ seçeneği](../../tasarim-onizleme/ozet-inceleme-2026-11-21/17-butce-etiket-cizgisi.png) uygulanır. Kategori simgesi ve adı üstte, halka sol altta, sağda “Kalan” veya “Aşım” ile tutar arasında para sütunu genişliğinde ince çizgi. Tek bütçede [16’nın alttaki yatay kutusu](../../tasarim-onizleme/ozet-inceleme-2026-11-21/16-butce-kalan-tutar.png) kullanılır: etiket ve tutar yan yana, çizgisiz. Üç bütçede en dolu iki kutu, dört veya fazlasında dört kutu görünür. Başlıkta seçili ay ve kapsamdaki toplam adet tam listeyi açar. Aşım yuvarlanmış yüzdeden türetilmez. Aşağıdaki önceki denemeler tasarım geçmişidir.

**Ay özeti ve kategori giderleri — kapanış.** [Ay özeti](../../tasarim-onizleme/ozet-inceleme-2026-11-21/01-ilk-ekran.png) ve [kategori giderleri](../../tasarim-onizleme/ozet-inceleme-2026-11-21/02-kategori-butce.png) çalışan uygulamadaki görünümleriyle kalır. Halka yanında “Bu ay toplam” ve “En büyük pay”, listede en fazla dört adı verilen kategori ve varsa “Diğer” gösterilir. Ek yüzde cümlesi ve “Diğer” için kategori adedi satırı uygulanmayacak.

**Bölüm sırası.** Özet’te mevcut sıra korunarak ilerlenir: ayın özeti, kategori giderleri, bütçeler, yaklaşanlar, varlık durumu ve hesap bakiyeleri. Gecikme varsa üst şeritten erişilir. Yaklaşanlar’ı yukarı taşıma önerisi için kullanıcı kararı alınmadı.

**Kodlama devri — ilk iş Özet’in tamamını uçtan uca uygulamak.** Önce Özet için gereken ASP.NET Core ve Flutter değişiklikleri birlikte tamamlanır ve doğrulanır. Sonra Tekrarlayanlar, Planlananlar ve kalan ekranların tam akışlarına geçilir. Kullanıcının bu oturumdaki kararıyla ürün kodunu Codex de değiştirir. Tamamlanan iş, doğrulama sonucu, commit ve sıradaki görev bu günlükte tutulur.

1. **Sunucu ve veri sözleşmesi.** Aylık özet, kategori giderleri, bütçeler ve varlık için mevcut okumaları kullan. Eksikliği aşağıdaki kabul örneklerinden doğrulanan alanları tamamla. Yaklaşanlar’da aynı kapsam ve yedi günlük aralık için açık vadeler, gerçek ödeme adedi, bilinen ödeme toplamı, tutarı belirsiz ödemeler ve önceki gecikmeler tutarlı dönmeli. Kart taksidini ekstre ödemesiyle çift sayma. Veri modeli henüz bulunmayan abonelik/deneme olayı için bağımlılığı yaz; istemcide sahte kayıt üretme.
2. **Flutter ve gezinme.** [Özet sayfası](../../mobile/business_finance_mobile/lib/features/dashboard/presentation/dashboard_page.dart) ve [ortak kartlar](../../mobile/business_finance_mobile/lib/features/dashboard/presentation/dashboard_sections.dart) üzerinden altı bölümü birlikte kontrol et. Ay özeti, Kategori giderleri ve Hesap bakiyeleri mevcut davranışıyla sürer. Bütçeler 16/17, Varlık durumu 11, Yaklaşanlar 08 panosuna göre uygulanır. Tarihten açılan Planlananlar aynı haftayı ve kapsamı göstermeli; üst gecikme şeridinden girildiğinde eski vadeler gerçek tarihleriyle açık gelmeli. Hedef ekranın bu iki girişi karşılayan kısmı Özet teslimine dahildir.
3. **Özet’in kabulü.** API cevabı, ekrandaki adet/tutar ve açılan listenin aynı sentetik örnekte uyuştuğunu doğrula. Ay ve kapsam değişimi, tek/üç/dört bütçe, limit aşımı, gecikme, bilinmeyen tutar, boş ve hatalı okuma, büyük yazı ve ekran okuyucu kontrol edilir. Değişen backend için build/test/format, Flutter için analyze/ilgili test/build sonuçları kaydedilir. Sonraki ekranlara geçişte kalan gerçek veri bağımlılıkları açıkça devredilir.

**Onaylanan iki giriş.** Özet’te tarih bağlantısı aynı kapsam ve aynı yedi günlük aralıkla Planlananlar listesinin başını açar. Sayfanın en üstündeki gecikme şeridi aynı kapsamla, gecikenler genişletilmiş ve görünür konumda açılır. Örneğin 21 Kasım’da aralık 21–27 Kasım’dır. 25 Ekim vadeli açık ödeme 25 Ekim olarak gecikmelerde yer alır; bugün vadeli kayıt haftanın listesine girer. Eski gecikmeleri göstermek için haftanın başlangıcı geriye çekilmez. Kullanıcının Özet’te seçtiği rapor ayı bugüne bağlı haftayı değiştirmez.

**Aboneliğin görünmesi ile ödeme sayılması.** Tasarımda ücretli aboneliğin açık vadesi ve ücretsiz denemenin takip edilecek olayı kendi gününde Yaklaşanlar’a adaydır; aynı tarih ve önizleme sınırı geçerlidir. Örneğin hatırlatma 23, deneme sonu 25 Kasım ise kayıt yalnız ilgili olay ve durum için gösterilir. Hatırlatma tutar ve ödeme adedi üretmez. Denemeye ücret uydurulmaz; kullanıcı ücretli döneme geçişi teyit edince ilgili ücretli vade gösterilir. Hesaptan ödenen ücret haftalık ödeme toplamına girer; karta yansıyacak ücretin nakit ödemesi ekstrede izlenir.

**Sunucu ve istemci için uygulama devri.** Aşağıdakiler tasarımın gerektirdiği işlerdir. Sözleşme biçimi uygulama aşamasında seçilecek. [Bugünkü okuma ve toplama](../../src/BusinessFinance.Application/FinancialActivities/ListPlannedActivitiesUseCase.cs) · [Sözleşme ve yükümlülük kuralı](../../src/BusinessFinance.Application/FinancialActivities/PlannedActivityContracts.cs)

| Öncelik | Bugünkü durum ve gereken davranış | Kabul örneği |
|---|---|---|
| Kritik · çift sayım ve belirsiz tutar | Mevcut okuma, ödeme yükümlülüğü işaretli gecikmeyen kayıtları topluyor. Yeni Özet için ödeme, tahsilat, karta yansıma ve hatırlatma aynı kanonik okumada ayrılmalı. Sunucu satırın türü ve kaynağından kararlı makine değerleri üretir; istemci başlık, renk veya hizmet adından tahmin yürütmez. Adet ve tutar, önizleme kesilmeden önce aynı kullanıcı/kapsam/dönemin tüm açık ödemelerinden gelir. Bilinen toplam, bilinmeyen tutar adedi ve toplam ödeme adedi ayırt edilir. | 08’de dört ödeme ₺7.500. ₺6.800 tahsilat ve ₺1.400 karta yansıma bu toplama eklenmez. Tutarı bilinmeyen bir vergi daha varsa beş ödeme, ₺7.500 bilinen tutar ve bir tutarı belirsiz ödeme vardır. |
| Kritik · dönem ve gecikme | Bugünkü 7/30/90 gün okuması üst sınırı başlangıca seçilen gün sayısını ekleyerek kuruyor. Tasarımda iki uç da dahil: bugün + 6 gün. Özet ve Planlananlar aynı referans günü, kapsamı ve vade tarihini kullanmalı. Önceki açık ödemeler ayrı kümedir; gecikmelerin alt sınırı hafta başı değildir. Eski istemciler için mevcut sözleşme davranışı ayrıca incelenir. | 21–27 Kasım’da 20 Kasım açık ödeme gecikmede, 21 ve 27 Kasım vadeleri haftada, 28 Kasım dışarıda. Ödenmiş kalem görünmez. Şeritteki adet açılan bölümle tutar. Açık/kapalı ve kaydırma konumu istemcinin gezinme durumudur. |
| Yüksek · kaynak ve kimlik | Ücretli abonelik, ücretsiz deneme olayı, gerçek ödeme kaynağı ve ilgili vade ayırt edilebilmeli. Deneme bildirimi ödeme yükümlülüğü oluşturmaz. Verginin bilinmeyen tutarı korunur. İsteğe bağlı cari vade türleri istemcinin tanıdığı türlerle uyumlu devreye alınır; aynı açık bakiye iki satıra yansımaz. | Kart aboneliği karta yansır, hesaptan abonelik ödemedir. Deneme teyidi sonrası aynı dönem iki kez oluşmaz. Kısmi ödeme sonrası aynı vade kalan tutarla okunur. |
| Yüksek · kapsam ve güncel durum | Liste, gecikmeler ve toplamlar aynı kullanıcıya ve aynı seçime ait olmalı. Karma ekstre için kararlaştırılan tam kalan davranışını doğrula. Varlık, kart borcu ve hesap bakiyeleri tek havuzdur. Seçili rapor kapsamı bunların tutarını bölmez. Başarısız veya eksik okumayı sıfır olarak gönderme/gösterme. | Aynı kullanıcıda İşletme/Şahsi geçişi haftalık akışı değiştirebilir; net varlık sabit kalır. Başka kullanıcıya ait vade veya tutar hiçbir görünümde yer almaz. |

**Güncel Varlık durumu önerisi.** [11 — uygulama ve yeni yerleşim](../../tasarim-onizleme/ozet-inceleme-2026-11-21/11-varlik-durumu.png) · [Pano kaynağı](ozette-varlik-durumu.html). Kullanıcı 10’daki yalın listeyi ve açıklamanın başlık sağında duruşunu beğenmedi. 11’de net varlık, kapsam açıklaması, oran çubuğu ve iki toplam aynı üst yüzeyde okunur. “İşletme ve şahsi toplamı” seçili kapsam varken net tutarın altındadır. Alt satırlar cihazdaki kendi simgelerini kullanır. Varlık ve borç renkleri üst çubuk, açıklamalar ve simge zeminleri arasında eşleşir. POS satırında toplam beklenen yatış tutarı ve ondan ayrı “Geciken yatış var” uyarısı görünür; ok Kasa’nın POS bölümüne gider. Tarih ve tekil tutarlar orada incelenir. Gecikme yokken uyarı kalkar. Mevcut üründe olduğu gibi kart borcu varsa borçlarda, fazla ödeme varsa varlıklarda gösterilir. Bu yerleşim görüşme önerisidir. Büyük yazı, dar ekran ve koyu tema cihazda ayrıca doğrulanacak.

**Yaklaşanlar satır metinleri ve Planlananlar’a geçiş.** Flutter modelindeki yedi tür, sunucunun isteğe bağlı iki cari vade türü ve tasarımdaki abonelik durumları karşılaştırıldı. Bunlar tasarım karşılıklarıdır. Mevcut kart ödeme yükümlülüklerine daraltılır. Açıklama seçiminde ad, tutarın işareti veya renk tek başına kullanılmaz.

| Kayıt | Yaklaşanlar’daki kısa açıklama |
|---|---|
| Tekrarlayan gelir / gider | Gelirde “Beklenen tahsilat”. Giderde “Ödeme”, hesap kaynağı doğrulanmışsa “Hesaptan ödenecek”, kart harcamasında “Karta yansıyacak”. |
| Vergi ve SGK | Kalemin kendi adı ile “Ödeme”. Tutar bilinmiyorsa para alanında “Tutarı ödemede girilecek”. Haftalık özette yalnız bilinen tutar toplanır, bilinmeyen ödeme ayrıca belirtilir. |
| Kesilmiş ekstre / kart taksidi | “Ekstre ödemesi” / “Kart taksidi · Karta yansıyacak”. Ekstrenin kalan tutarı ödeme toplamına girer. Kart taksidi için ayrıca nakit çıkışı yazılmaz. |
| Borç taksidi / alacak taksidi | “Borç taksidi” / “Alacak taksidi · Beklenen tahsilat”. |
| Ödenecek / tahsil edilecek yükümlülük | Örneğin faturada “Ödeme”, alacakta “Beklenen tahsilat”. |
| Kişiye ödeme / kişiden tahsilat | “Borç ödemesi” / “Beklenen tahsilat”. Sunucuda isteğe bağlı; Flutter modelinde yeni iki tür henüz karşılanmıyor. Hedefler cari akışla tekrar doğrulanacak. |
| Ücretli abonelik / deneme | Kaynağa göre “Abonelik · Hesaptan ödenecek” veya “Abonelik · Karta yansıyacak”. Denemede ilgili güne göre “Deneme hatırlatması” veya “Deneme bitişi”; ödeme adedine ve toplamına eklenmez. |

08 panosundaki örnek açıklamaları korunur. Uygulama için açık nokta: sunucudaki mevcut ödeme yükümlülüğü işareti kart taksidini de kapsar. Yeni bandın ödeme adedi ve tutarı bu işaretten tek başına alınamaz. Tekrarlayan satırın kart/hesap niteliği de yalnız gider yönünden anlaşılamaz. Ödeme, tahsilat ve karta yansıma aynı sınıflamayla hem satıra hem haftalık özete uygulanmalı.

**Ana liste tasarımına başlangıç.** [Emülatörde çalışan liste](../../tasarim-onizleme/ozet-inceleme-2026-11-21/09-planlananlar-mevcut.png) · [04 — dönem ve kapsam](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/04-donem-ve-kapsam.png). Özet’in gecikme şeridinden girildiğinde mevcut ekran 30 gün ve Tümü ile açıldı. Geçmiş vadeler tek tek büyük kartlarda, dönem ve tür seçimleri iki ayrı şeritte. İlk tasarım işi açılış bağlamını karşılaştırmak: tarih bağlantısından aynı yedi gün ve kapsam; gecikme şeridinden aynı dönemin gecikmeler bölümü açık. Sonra gün başlıkları, satır eylemleri ve dönem özeti 04 üzerinden ele alınacak.

**Güncel öneri — haftanın ödeme toplamı.** [08 panosu](../../tasarim-onizleme/ozet-inceleme-2026-11-21/08-yaklasanlar-odeme-toplami.png) · [HTML](ozette-iki-hafta.html). Kullanıcı 07’de yoğun haftanın alt bandının kaldırılmasını beğenmedi. İki örnekte de solda “7 günde ödenecek · 3 ödeme” veya “7 günde ödenecek · 4 ödeme”, sağda o haftadaki ödemelerin toplamı yer alıyor. Ödeme adedi ve toplam seçili kapsamın tam yedi gününden geliyor. Pano genişliğinde her ikisi de tek satırda. Büyük yazı ayrıca doğrulanacak.

### Önceki incelemeler — karar geçmişi

**O turdaki tasarım yönü.** Her bölüm, kullanıcının orada anlayacağı bilgi ve yapacağı işe uygun alanı alır. Önceki iki satır ve 150–180 dp önerisi geri çekildi. Beş satırlı pano içerik ve dokunma hedeflerini örnekler.

Emülatörde Hepsi / Kasım 2026 incelendi (1080 × 2400, 420 dpi, yaklaşık 411 × 914 dp). [İlk ekran](../../tasarim-onizleme/ozet-inceleme-2026-11-21/01-ilk-ekran.png), [kategori ve bütçe](../../tasarim-onizleme/ozet-inceleme-2026-11-21/02-kategori-butce.png), [mevcut Yaklaşanlar](../../tasarim-onizleme/ozet-inceleme-2026-11-21/03-yaklasanlar.png), [varlık durumu](../../tasarim-onizleme/ozet-inceleme-2026-11-21/05-varlik.png), [hesap bakiyeleri](../../tasarim-onizleme/ozet-inceleme-2026-11-21/04-hesaplar.png). Özet başlığı, kapsam ve alt gezinme arasında yaklaşık 650 dp görünür alan var. Üç vadeli mevcut Yaklaşanlar başlığıyla yaklaşık 345 dp. Beş vadeli HTML önerisi başlığıyla yaklaşık 590 CSS pikseli. Sonuncusu tarayıcı ölçüsü, Flutter cihaz ölçümü değil.

**Sayfa düzeni önerisi (görüşmede).** Mevcut sırayı başlangıç al: ayın özeti → kategori giderleri → bütçeler → Yaklaşanlar → varlık durumu → hesap bakiyeleri. İlk üçü seçilen ayı anlatır. Yaklaşanlar’da gerçek tarih aralığı, geçmiş ay seçiliyken de bugünden başlayan haftayı açık eder. Son iki bölüm güncel ortak havuzun farklı görünümleridir.

- Ayın özetinde gelir ve giderden nete, oradan Hepsi kapsamındaki işletme/şahsi kırılımına geçiş korunur. Kategoride en büyük iki payın yakınlığını okumak için aynı hizadaki çubuklar öne çıkar. Bütçede halka ile birlikte aşılmış/kalan tutar okunur.
- Acil gecikmeler üstte tek girişten açılır. Yaklaşanlar’da tarih, ad, paraya etkisi ve tutar birlikte okunur. Üç vadeli mevcut örnek eksiksiz gösterilir. Yoğun haftanın önizleme sınırı, tutar özeti ve devam bağlantısı seyrek ve karma haftalar yan yana denenerek seçilecek.
- Varlıkta kullanılabilir para ile yoldaki POS ve alacak ayrımı korunur. Hesap bakiyelerinde grup toplamı altında eksiye düşen hesabın adı ve tutarı görünür kalır.
- Sonraki görsel karşılaştırmada geçmiş ay seçimi, yoğun hafta, aşılmış bütçe, eksi hesap ve büyük yazıyla uzun ad denenir. Bölüm başlıkları, tutar hizaları ve dokunma hedefleri ortak tasarım sistemini izler.
- Apple grafiklerde ana bulgunun sözcüklerle de anlaşılmasını örnekliyor. Monarch aylık değerlendirmeyi ayrı bir akışta, vade takvimini kendi ekranında sunuyor. Sayfa sırası ve yerleşim bizim önerimizdir. [Apple — Charts](https://developer.apple.com/design/human-interface-guidelines/charts) · [Monarch — Monthly Progress Report](https://help.monarch.com/hc/en-us/articles/4402543752468-Monthly-Progress-Report) · [Monarch — Recurring](https://help.monarch.com/hc/en-us/articles/4890751141908-Tracking-Recurring-Expenses-and-Bills)

[Yaklaşanlar kartı](../../tasarim-onizleme/planlananlar-2026-11-21/ozet-yaklasanlar.png) · [Dokunma hedeflerini incele](ozet-yaklasanlar.html) · [24 panoluk galeri](revizyon-galerisi.html)

**Devralınan iş.** 20 Kasım galeri düzeni diskte tamamlanmıştı. 21 Kasım kontrolünde 24 ayrı pano, galeri içindeki 80 bağlantı hedefi ve 39 görsel doğrulandı. 05, 12, 17–18 ve 19-03 için aşağıdaki devir geçerli. Panoları yeniden incelemeye açmaya gerek yok.

**Bugünkü uygulama.** Aşağıda bağlantıları verilen DashboardUpcomingCard, DashboardViewModel ve dashboard_page üzerinden kontrol edildi. Seçilen kapsam ve bugünün tarihiyle yapılan okumada istemci ödeme yükümlülüklerini ayırıyor, gecikmeyenleri vade sırasına koyup ilk beşini gösteriyor. Gizli adet hesaplanıyor ama karta verilmiyor. Tarih sütunu ve zaman çizgisi var. Kart ve bölüm bağlantısı dönem veya kayıt taşımadan Planlananlar’a gidiyor. Okuma hatasında listeler temizlendiğinden boş hafta mesajı çıkabiliyor.

**Görüşülecek taslak.** Örnek bugün 12 Aralık. 04 panosundaki dönem 12–18 Aralık’a daraltıldı. 9 Aralık faturası önceki iki gecikmeye katılıyor (₺2.800 + ₺480). 21 Aralık vergisi bu haftanın dışında.

| Konu | Önerilen davranış |
|---|---|
| Hafta ve kapsam | Özet’te seçili kapsam, bugün dahil yedi takvim günü (bugün + 6). Ay raporunda başka bir ay açık olsa da kartın başlangıcı bugün. İki sınır da dahil. Mevcut sunucu ufku özel tarih aralığına geçirilirken üst sınır doğrulanacak. |
| Satırlar | Planlananlar’daki aynı açık vadeler tarih sırasıyla. Tekrarlayan gelir/gider, vergiler, abonelik ödemeleri, kesilmiş ekstre, kart taksidi, borç ödemesi ve alacak tahsilatı. Deneme hatırlatması kendi gününde gösterilir, satırında tutar yer almaz. Aynı gün ve kapsamda sıra iki ekranda da aynı kalır. İlk beş satırdan sonrası varsa “N kayıt daha” aynı haftanın tam listesini açar. |
| Tutar | Aynı haftanın bütün kayıtları kullanılır. Ödenecek, beklenen giriş ve karta yansıyacak ayrı gösterilir. Yalnız dolu gruplar görünür. Tutarı bilinmeyen ödeme kendi grubunda adetle belirtilir. Hatırlatma toplam üretmez. Karma ekstre için 04 panosundaki tam kalan kuralı geçerlidir. |
| Gecikenler | Bugünden önceki açık ödeme yükümlülükleri üstte tek satırda toplanır. Adet, en eski vade ve bilinen tutar görünür. Bilinmeyen tutar varsa adetle belirtilir. Bu satır beş kayıt sınırının ve dönem toplamının dışındadır. |
| Dokunma ve geri dönüş | “Tümünü gör” aynı dönem ve kapsam, tüm türler ile Planlananlar listesinin başını açar. Gecikenler aynı listede gecikme bölümünü açıp görünür kılar. Tek satır aynı hafta içinde ilgili vadeyi seçerek o türün mevcut panelini veya ayrıntısını açar. Ödeme ayrı onay ister. Panelden dönüşte aynı vade/listedeki konum, listeden geri dönünce Özet konumu korunur. Başarılı işlemden sonra iki görünüm de yenilenir. HTML’deki bilgi kutuları önerilen hedefleri açıklar. |
| Boşluk ve hata | Başarılı okumada hafta boşsa “Önümüzdeki 7 günde bekleyen kayıt yok.” Gecikme varsa üst satır kalır. İlk yüklemede yükleniyor durumu; yeni seçim okunamadıysa kart içinde yeniden deneme. Aynı seçim yenilenemediyse son veriler ve “Liste güncellenemedi.” uyarısı birlikte kalır. |

**Sıradaki görüşme.** Yaklaşanlar’ı Özet içindeki komşu bölümleriyle ve farklı yoğunluktaki haftalarla değerlendir. Bunlar henüz onaylanmadı. Ardından Claude’u beklemeden Planlananlar’ın gerçek ana listesiyle 04 panosunu yan yana açarak tasarım aşamasına devam et. Cari akış tamamlanınca borç ve alacak hedeflerini yeniden doğrula. Taksit planında 19-03 için gerçek ekran incelemesi hâlâ açık. PNG tarayıcıda kontrol edildi. Cihaz, büyük yazı ve Flutter/API kontrolleri uygulama aşamasında.

### 20 Kasım — önceki devir ve çalışma sırası

24 panonun kullanıcıyla senaryo incelemesi bitti. Bu bölüm 20 Kasım sonundaki durumu kaydeder. Aşağıdaki tarihli incelemeler karar geçmişidir. Özellikle aramanın yanındaki eski tür menüsünü, siyah dönem özetini, tam sayfa tarih seçimini veya 19-03'teki temsili taksit ekranını uygulama referansı alma.

1. **Görsel düzenleme tamamlandı.** [Galeri](revizyon-galerisi.html) dört konuya ayrıldı. 24 pano numarası ve eski bağlantı hedefleri korundu. Güncel görseller önde, eski senaryolar arşivdedir. 12'de katalog ve arama, 17–18'de logo, ödeme takvimi ve geçmiş kararları yeni çizimlere işlendi. 05'in ana görseli kalan borcun onaylanan alt konumuyla yeniden üretildi. 19-03'ün temsili taksit ekranı arşivde açıklandı.
2. **Sıradaki oturumun ilk işi — Özet → Yaklaşanlar.** Başlangıç kaynakları [DashboardUpcomingCard](../../mobile/business_finance_mobile/lib/features/dashboard/presentation/dashboard_sections.dart), [DashboardViewModel](../../mobile/business_finance_mobile/lib/features/dashboard/presentation/dashboard_view_model.dart) ve [Özet sayfası](../../mobile/business_finance_mobile/lib/features/dashboard/presentation/dashboard_page.dart). Mevcut uygulamada seçili kapsamla 7 günlük veri alınır. Ödeme yükümlülüklerinden bugün ve ilerideki ilk beş satır gösterilir, gecikmeler ayrı özetlenir. Kart bir bütün olarak Planlananlar’a gider. Gerçek kart üzerinden tarih sütununu, türleri, dönem toplamının anlamını, “Tümünü gör”, gecikme ve tek satır hedeflerini, boş/hata davranışını değerlendir. Görünür satır sayısı için beş, mevcut uygulamanın değeridir. Özet widget sıralaması ve kişiselleştirme ayrı konulardır.
3. **Tasarım aşaması.** Bu iki işin ardından Claude’un cari çalışmasını beklemeden devam et. Ortak panel ve ekranlar için önce çalışan uygulamayı incele. Uygulamaya özgü bir uyarlama gerekiyorsa kararını gerçek ekran üzerinden ver. Ekran çizmek için Flutter veya backend değiştirme.
4. **Claude cari işini bitirdiğinde** yeni sözleşme ve gerçek ekranlarla borç ödemesi / alacak tahsilatı satırlarının türünü, yönünü, hedefini ve geri dönüşünü doğrula. Etkilenen tasarımları o zaman düzelt. Cari uygulaması Claude’a aittir.

**Kontrol ve sınır.** Galeride 24 pano hedefi, 80 bağlantı ve 39 görsel doğrulandı. Yeni abonelik tutarları örnek veridir. Eski ilk tur görevleri tarihsel kayıttır. Tasarım aşamasında abonelik kapsam seçicisi ve gerçek taksit ekranı uygulama üzerinden, dar ekran ve büyük yazı da cihazda doğrulanacak. Flutter veya API testi yapılmadı.

**19-02 — ödeme sonrası dönüş onaylandı.** 9 Aralık vadesi 12 Aralık’ta ödenince ödeme tarihi 12 Aralık olarak kaydedilir. Liste kullanıcının seçtiği 9 Aralık’ta kalır. O günün son açık kaydı kapandıysa dönem boş mesajı görünür. Daha eski gecikmeler kendi bölümünde kalır.

**19-03 — tasarım aşamasına bırakılan inceleme.** Sağdaki çizim temsili bir senaryodur. Kullanıcı, uygulamadaki taksit planı ekranının farklı olduğunu belirtti. Tasarım aşamasında önce Kartlar’daki gerçek ekran ve oraya ulaşan akış incelenecek. Planlananlar’dan gelindiğinde ilgili taksidin kolay bulunup bulunmadığı orada değerlendirilecek. Gri vurgulu satır ve “Sıradaki taksit” kartı için yerleşim kararı alınmadı.

**18-01 — [onaylanan yenileme durumları](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/18-01-yenileme.png).** Aynı dönem, kapsam ve tür için son okunan tutarlar ile kayıtlar ekranda kalır. Özetin altında bilgi simgeli, nötr gri “Liste güncellenemedi.” satırından tekrar denenir. Yeni seçime ait veri yoksa dönem ve kapsam başlığının altında 06. panodaki ortak yükleme hatası gösterilir. Başarılı yenilemeyle uyarı kalkar.

**18-03 — dönem ve gecikme davranışı onaylandı.** Açık vade seçilen aralıktaysa kendi gününde listelenir ve ilgili dönem toplamına girer. Ödeme vadesi hem bugünden hem de aralığın başlangıcından önceyse “Geciken ödemeler” bölümünde gösterilir. Bu bölümün tutarı dönem toplamından ayrıdır. Alt satır seçimin başlangıcını belirtir (“15 Aralık öncesi”). Geçmiş bir dönem seçildiğinde o dönemden bugün hâlâ açık olan vadeler görünür. Yerleşim için güncel 04 panosu esas alınır.

**18-02, 18-04 ve 18-05 — [filtre ve tarih seçimi](planlananlar-filtresi.html).** Tür menüsü filtre panelinin üzerine açılır. Cari vadeler yönüne göre “Borç ödemesi” ve “Alacak tahsilatı” altında ilgili borç ve alacak taksitleriyle birlikte yer alır. Kart taksidi ayrı seçilir. Kapsam ana ekrandadır. Dönem seçeneklerinin işareti soldadır. Tarih aralığına dokununca panelin üzerinde takvim açılır. Seç ile tarih panele döner, Sonuçları göster ile uygulanır. Sağ üstteki Sıfırla siyah ve altı çizilidir. Tüm zamanlar eklenmemesi kabul edildi. Tarih aralığı dönem seçeneklerinin altındaki takvim kutusundan açılır. Kartlar’daki seçicinin ortalı başlığı, ay okları, kesintisiz hafta şeritleri, dairesel uçları ve alt düğmeleri kullanılır. Planlananlar özel aralıkla açılır. Tür menüsünde ortak yükseklikte simgeli satırlar kullanılır. Menü sığmadığında alt panel gerektiği kadar yükselir.

**10. pano — [ilk giriş ve adla arama önerisi](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/10-ilk-giris-ve-arama.png).** İlk kullanım açıklaması “Kira, maaş ve aboneliklerini ekle.” İlk girişte ortak boş ekranın “Tekrarlayan plan ekle” düğmesinin altında “Abonelik ekle” bağlantısı doğrudan hizmet seçimine gider. Kayıtlı planlar varken aramanın sağında Abonelikler sayfası bağlantısı kalır. Boş olmayan bir adla arama sonuçsuzsa yazılan adla plan formunu açma önerilir; yalnız ad taslağa taşınır ve düzenlenebilir. Aramayı temizle de erişilebilir. Eşleşme varsa yalnız sonuçlar listelenir. Bu öneri ilk kullanım ve sonuçsuz aramaya özeldir.

**08. pano — son vade ekranı için metin önerisi.** Başlık ve düğme “Planı sona erdir”, tarih alanı “Son vade”. Takvim örneğinin yerine seçimin etkisini belirten kısa metin: “5 Şubat’taki ₺12.500 ödeme beklemeye devam edecek. 5 Mart’tan itibaren yeni vade oluşmayacak.” Tarihler, tutar ve bekleme durumu seçilen vadeye göre gösterilir.

**06. pano — görünüm.** Sağ üstte yalnız altı çizili “Filtre” görünür. Etkin tür kendi kaldırılabilir etiketinde kalır. Sonuç bulunamadığında ortak boş durum, hata halinde ortak hata görünümü kullanılır. Önceki gecikmeler görünürken dönem boşsa kartın altına ortak açık mavi bilgi satırı yerleşir: “Bu dönemde bekleyen kayıt yok.”

**20 Kasım — kapsam seçimi ve 06. pano.** Tarihin sağındaki Hepsi / İşletme / Şahsi seçicisi uygulamanın genel kapsamını günceller. 04 taslağında her örnek ekranın kapsamı değiştirilebilir. “Filtreyi temizle” türü kaldırır, dönem ile genel kapsamı korur. Dönem de boşsa “Dönem seç” gösterilir. Önceki gecikmeler varsa dönem boş mesajıyla birlikte korunur. [Güncel 06. pano](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/06-bos-durumlar.png).

**05 — kalan ekstre borcu, kabul edilen yerleşim.** Aynı kısmi ödeme iki konumda karşılaştırıldı. Kullanıcı Açıklama alanından sonra ve düğmelerden önce açık gri bir sonuç satırını seçti. Boş tutarda seçili ekstrenin güncel kalanı, geçerli ödeme girilince ödeme sonrası kalanı gösterilir. Bu bir önizlemedir; asıl ödeme Kaydet ile gerçekleşir. [Konum karşılaştırması](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/05-ekstre-konum.png) · [Üç tutar durumu](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/05-ekstre-kalani.png). Ortak Kartlar formuna uygulama inceleme sonrasında değerlendirilecek; klavye açık cihaz kontrolü yapılmadı.

**20 Kasım — 05. pano, ortak ödeme akışı.** Kart ekstresi satırının bütünü seçilen kart ve kesilmiş dönemle Kartlar’daki “Ödeme tutarı” panelini açar. Başlık altında kart adının karşısında “Ekstreyi görüntüle” seçilen dönemi açar; geri dönüş seçimi korur. Devam et ile ortak “Kart ödemesi” formuna gidilir; Kaydet sonrası Planlananlar aynı dönem ve kapsamda yenilenir. Aynı ekstre bilgisiyle Kartlar’dan açıldığında da bağlantının yeri aynıdır. Görünüm önerisi kullanıcı ekran görüntülerine dayanır. Fatura ve vergide kaynak ayrıntısı ilk ödeme panelinin başlığı altında aynı konumda yer alır. [Ekstre](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/05-ekstre-odemesi.png) · [Fatura ve vergi](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/05-fatura-vergi.png).

**04. pano — taksit ve eksik vergi tutarı.** Taksit satırında “Planı görüntüle ›” kullanıldı. Ödenecek toplamının altındaki gösterge daire içinde ünlemle “Tutarı eksik: 1 vergi” şeklindedir. Sayı, seçili dönem ve kapsamda tutarı girilmemiş vergi sayısıdır. Sıfırsa gösterge gizlenir.

**20 Kasım — 04. pano ikinci inceleme.** Filtre yazısının sağ kenarı dönem kartının dış kenarıyla hizalı ve altı çizili. Simgelerde uygulamanın ortak açık tema renkleri geri geldi. Örnekteki tek eksik vergi için “Vergi tutarı henüz girilmedi.” denendi. Önceki vadeler “Geciken ödemeler” ve “8 Aralık öncesi” olarak iki satırda; açıkken kayıtlar aynı kartta görünür. [Açılır taslak](donem-ve-kapsam.html).

**20 Kasım — [04. pano](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/04-donem-ve-kapsam.png).** Dönem ve seçili kapsam açık gri başlıkta; bilinen ödemeler, varsa belirsiz vergi notu, beklenen giriş ve karta yansıyacak tutar beyaz yüzeyde. İşletme veya Şahsi görünümünde kayıt kapsam etiketi tekrarlanmaz; Hepsi görünümünde korunur. Karma ekstre üçünde de “Ana kart · Karma ekstre”. Kaynak yazısı gri, gecikme kırmızı. Aynı günün vadeleri tek kartta ince ayraçlarla gösterildi. Statik tasarım incelemesi.

**20 Kasım — [09. pano](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/09-tekrarlayanlar.png).** Ana listede aramanın sağından Abonelikler sayfasına geçilir. Yeni çizimde karma listede “Aylık gider”, “Haftalık gelir”, “Yıllık abonelik” gibi kısa açıklamalar denendi. Abonelikler sayfasında ücretli planın altında yalnız sıklık yazıyor. Deneme satırı karma listede “Aylık abonelik”, Abonelikler sayfasında “Aylık” olarak görünür. Sağda “Ücretsiz deneme” ve bitiş tarihi korunur. Sıklık, denemeden sonra geçerli olacak ücretli döneme aittir. Kullanıcının renk düzeltmesiyle kira ve abonelik geçmişindeki “Tümü ›” bölüm başlığının rengini kullanır. 07. pano güncellendi; 17. panonun güncel çizimi galeriye eklendi.

**20 Kasım — 07. pano son düzeltme.** Başlık ve Düzenle aynı dikey merkezde. Açık vadede kaynak ve eylem sığdığında ortak alt satırda durur; dar alanda eylem alta geçer. Ödeme taslağında “Gecikmiş” kırmızı. Geçmiş ödemeler başlığında, gösterilen ikiden daha eski ödeme varsa “Tümü ›” görünür.

**20 Kasım — güncel 07 ve 16.** [07. panoda](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/07-kira-vadeleri.png) plan düzenlemenin sonundaki mavi özet kaldırıldı. Şubat vadesinde “Tekrarlayan ödeme” tarihin, “Gecikti” tutarın altında. Kaynak bilgisi tam genişlikte. [16. panoda](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/16-deneme-takibi.png) deneme durumu hizmet satırından biraz daha yüksek; Kapsam etiketi üstte ve tercih satırı gri. Durum ve dönem ücreti panolarda ayrı çizimler. Ortak yerleşim ve ayrı görsel vurgu tasarım önerisi.

**20 Kasım — 18. pano.** Abonelik ayrıntısındaki sıradaki ödeme kartında hizmet logosu korunur. Logo, tarih ve ödeme kaynağının yanında ortalanır. “Ücretli abonelik” satırı kaldırılır. Tutar sağ üstte, “Harcamayı kaydet ›” sağ altta yer alır.

**20 Kasım — 17. pano.** “Sıradaki ödeme” başlığının sağında mavi “Ödeme takvimi ›”, “Geçmiş ödemeler” başlığının sağında yalnız daha eski kayıt varsa “Tümü ›” yer alacak. Ortak bölüm başlığı büyük yazıda eylemi alt satıra alır. “Harcamayı kaydet” ilgili vadenin kartında kalır. Normal yazıda 320 ve 390 genişlik için yaklaşık metin ölçümü yapıldı; cihaz kontrolü uygulama sırasında yapılacak.

**20 Kasım — 16. pano için çizime işlenecek düzeltmeler.** Kapsam etiketi, ödeme kaynağında olduğu gibi seçim alanının üstünde yer alacak. “Denemeden sonra” bölümündeki tercih satırı eski gri zeminini koruyacak. Kullanıcı bu iki düzeltmenin şimdilik not alınmasını istedi.

**20 Kasım — [16. pano](../../tasarim-onizleme/planlananlar-revizyon-2026-11-20/16-deneme-takibi.png).** Deneme durumu, hizmet satırıyla aynı yükseklikte açık gri bir alanda gösterildi. Hatırlatmadan ve deneme sonundan iptal teyidi “Aboneliğimi iptal ettim” ile verilir. Ücretli takibe geçiş formuna güncel sıklık, ücret ve kapsam yerleşimi işlendi.

**20 Kasım — abonelik formu.** Ücretli abonelikte önce yenilenme sıklığı seçilir. Ardından “Aylık ücret” veya “Yıllık ücret” gibi seçilen dönemi belirten alan gelir. Deneme niyetinde “Devam edeceğim” ve “İptal edeceğim” alt alta gösterilir. Ücretli formda ödeme kaynağının altında Kapsam satırı seçili değeri gösterir. Dokununca ortak seçim görünümünde yalnız İşletme ve Şahsi sunulur. Yalnız şahsi kapsamı kullanan kişiye seçim sorulmaz.

**20 Kasım incelemesi — hizmet seçimi.** İlk açılışta kullanıcı hazır hizmetlerden birine doğrudan dokunabilir. Kataloğun altında “+ Başka hizmet ekle” bağlantısı görünür. Arama yapıldığında yalnız eşleşen hizmetler listelenir ve aynı bağlantı sonuçların altında kalır. Eşleşme yoksa eylem, örneğin ‘“Spor salonu” adıyla ekle’ biçiminde gösterilir ve yazılan ad forma taşınır. 15. panodaki “Kendi hizmetimi ekle” önceki metindir.

**20 Kasım incelemesi — 11. pano.** Tekrarlayanlar listesi normal planları ve abonelikleri birlikte gösterir. Arama ikisini de bulur. Aramanın sağında tür filtresinin yerine **Abonelikler ›** bağlantısıyla devam ediyoruz. Başlık altında ek satır yok. “Diğer planlar” filtresi kalkar. Ana listedeki + yeni plan açar. Bağlantıdan açılan **Abonelikler** sayfasında yalnız abonelikler görünür. Sağ üstteki + hizmet kataloğunu açar. İlk abonelik için ortak boş ekranın “Abonelik ekle” düğmesi de aynı kataloğa gider. 19 Kasım çizimlerindeki tür menüsü önceki öneridir. Hizmet seçiminin güncel örnekleri galeride 19-12 altındadır.

**19 Kasım kapanışı.** 18 ve 19 Kasım’a ait 24 pano sondan başa yeniden çizildi. Kullanıcı yeni seriyi sonraki oturumda birlikte incelemek istiyor. Çizimler statik taslak aşamasında.

**19 Kasım tarihli inceleme başlangıcı (tamamlandı).** [karşılaştırmalı galeriyi](revizyon-galerisi.html) kullanıcıyla açmak. İlk görüşme konusu, 12 planlık listede aramanın yanındaki tür menüsünün yerleşimi. [09’da listenin yoğunluğu](../../tasarim-onizleme/planlananlar-revizyon-2026-11-19/19-09.png), [11’de menünün kapalı ve açık hâli](../../tasarim-onizleme/planlananlar-revizyon-2026-11-19/19-11.png) görülüyor. Son kararla arama yanındaki menünün yerini Abonelikler bağlantısı aldı. Diğer panoların önceki çizimleri galeride, senaryo ve tutar kontrolleri [dış göz incelemesinde](DIS-GOZ-2026-11-19-tasarim-revizyonu.md).

### Önceki oturum notları

Bu belge kullanıcıya okunacak rapor değil, Codex/Claude Code arasında çalışma belleğidir (kullanıcının açık talebi). Kullanıcının karar vermesi gereken seçenek ve öneriler **sohbette** anlatılır; belgeye yazmak onay almak değildir. Kullanıcının belgeyi okuması beklenmez.

- **18 Kasım oturum kapanışı:** Kullanıcı işi çıktığı için burada durdu. Güncel [vade ajandasını](ilk-taslak.html) henüz incelemedi. İlk gösterilecek [yoğun liste görseli](../../tasarim-onizleme/planlananlar-2026-11-18/06-vade-ajandasi.png). 01–05 görselleri önceki taslaklardır.
- **Son kullanıcı düzeltmeleri:** adın üstte konumlanması talebi zorunluluk talebi değildir; uygulama genelindeki form değişikliği bu işin dışında. Varlık/hesap bölümleri kesin ertelenmedi, Özet turunda incelenecek. Öneri vermeden mevcut kod/ekran okunacak.
- **Son kararlar:** D1 ve D2 kullanıcı tarafından kabul edildi. D3 değişti: deneme sonunda kullanıcının devam niyeti sorulacak; iki niyette de bildirim olacak. Niyetin tam metni, bildirim zamanı ve deneme sonu teyit akışı henüz kesinleşmedi.
- **Kapanış kararları:** D4 kabul edildi. Henüz kesilmemiş ekstre bu çalışmada tahmin edilmez veya gösterilmez. D6 kapsamında kullanıcı deneme sonrası ücreti hizmetin teklifindeki tutara göre girer. Ücreti bilinmeyen ayrı deneme kaydı bu çalışmanın dışındadır.
- **Doğrulama sınırı:** kaynak okundu, gerçek API'ye yazılmadı; cihazda senaryo yürütülmedi. Aşağıdaki çıkarımlar test sonucu değildir.

## 1. Çalışma sınırı ve yönetimi

- Kullanıcı süreç sırasını, belge yazma zamanını ve branch yönetimini Codex'e bıraktı. Bu tur tasarım çalışması mevcut `borc-alacak-cari` dalında yürür; temel commit `d359ab7`.
- Codex'in yazdığı dosyalar `design/` altında kalır. Bu belge aşama açmaz, ürün/ADR kararı yerine geçmez. Kod, aşama ve kalıcı belgeler için mevcut çalışma sınırları korunur.
- Borç-alacak çalışması Claude Code'dadır; Planlananlar tasarımı bu işten bağımsız ilerler. Kişi, borç, yükümlülük ekranları; migration ve yedek kapsam dışıdır.
- `debt/receivable-installment`, `payable/receivable-obligation`, `counterparty-*-due` türlerinin ve eylemlerinin anlamı değiştirilmez. `ListDebtAsync`, `ListObligationsAsync`, `ListCounterpartyDuesAsync`, `includeCounterpartyDues`, `counterparty-loan`, `counterparty-write-off` borç-alacak işiyle koordinasyon sınırıdır.
- Şimdilik tek belge: kararlar, inceleme, tasarım önerileri, plan ve ilerleme burada tutulur. Ayrı ilerleme belgesi ancak uygulama checkpoint'leri başlayıp ihtiyaç doğarsa ayrılır; her görüşmede yeni belge açılmaz.
- Branch ayrımı uygulama teslimi somutlaşınca değerlendirilir. Mevcut kullanıcı değişiklikleri taşınmaz, silinmez veya commit'e katılmaz. Bu tasarım belgesi için tek başına commit atılmaz.

## 2. Kullanıcının belirlediği yön

| Konu | Karar / talep |
|---|---|
| Ortak tasarım | Ortak bileşen temel alınır, sayfanın ihtiyacına uyarlanır. Daha iyi ortak çözüm bulunursa bileşen de gelişir; ekranlara bağımsız ölçü kopyalanmaz. |
| Ortak planlama | Planlananlar, yaklaşanlar, tekrarlayanlar ve vergi bekleyenleri ortak kaynak ve davranış kurallarına bağlanır. |
| Vergiler | Vergi takibi ayrı erişilebilir ekran olarak kalır. Vergiler ortak listede de görünür; iki yerden aynı vergi ödeme akışına gidilir. |
| Filtreler | Yeni ortak filtre paneli ve tarih seçimi dili kullanılacak; alanlar ekrana göre değişecek. |
| Abonelikler | Netflix/Spotify gibi hizmetlerin ad ve logoyla hazır olması araştırılacak. Bunlar kategori değildir. Ücretsiz deneme bitişi ve hatırlatma önemli ihtiyaçtır; ayrıntılı kapsam henüz kesin değil. |
| Adlandırma | İlk çalışma yalnız tekrarlayan/abonelik tasarımında. Kullanıcı ad alanının üstte olmasını sordu; zorunlu doldurulmasını istemedi. Zorunluluk kararı verilmedi. |
| Özet | Yaklaşanlara ilgili bütün türler katılmalı. Diğer widget'lar bu ekran çalışılırken incelenebilir; bütün Özet'i yeniden kurmak şimdiki zorunlu kapsam değil. |
| Sonraki fikirler | Ertelenen işler, nedenleri ve yeniden ele alınacak bağlamları bu belgede ayrıca tutulacak. |
| İnceleme disiplini | Mevcut ekran/kod görülmeden genel çözüm önerilmez. Gözlem, öneri ve onaylı karar ayrılır. |

## 3. Doğrulanmış mevcut yapı

Uçlar `/api/v1/` altında. İnceleme kaynak kod ve kullanıcının ekran görüntüleri üzerinden; bu oturumda cihaz kabulü yapılmadı.

| Yüzey | Okuma ve davranış |
|---|---|
| İşlemler | `financial-activities`; üst şerit ayrıca `financial-activities/planned` ile 30 günü okur. |
| Planlananlar | `financial-activities/planned`; 7/30/90 gün, kapsam ve istemci tarafında tür filtresi. Gecikmişler alt tarih sınırı olmadan dahil. |
| Özet / Yaklaşanlar | Aynı planlanan uç, 7 gün; istemci `isPaymentObligation` ile daraltır. Bugün ve yaklaşanlardan ilk 5, gecikmişlerden ayrı özet. |
| Planlama ve raporlar | Tekrarlayanlar, Yaklaşanlar, Raporlar sekmeleri. İlk açılışta plan, dönem kayıtları, yaklaşanlar, gelişmiş rapor, hesap, kategori ve kart olmak üzere 7 okuma birlikte beklenir. |
| Eski Yaklaşanlar | `upcoming-payments`; sunucuda kanonik planlanan okumanın ödeme yükümlülüklerine daraltılmış hâli. Kapsam filtresi yok. |
| Vergi | `taxes`, `taxes/plans/{id}`, `tax-payments`; plan ve dönem yazmaları tekrarlayan uçlarını kullanır. |
| Hatırlatma | `ReminderController` planlananların 30 gününü okur. `ReminderPlanner` seçilen türleri vade günündeki tek günlük bildirime toplar. Deneme bitişinden önce hatırlatma bugünkü modelin sunduğu bir özellik değildir. |

### Tekrarlayan planın bugünkü akışı

1. Planlama ve raporlar → Tekrarlayanlar → Tekrarlayan plan ekle.
2. Form sırası: hesap/kart → kategori → tutar → tür → sıklık → ay sonu davranışı → başlangıç → bitiş → tekrar sınırı → açıklama.
3. Tür `income`, `expense` veya `bill-payment` olabilir. Gelir yalnız hesaba bağlanır. Türün sonradan seçilmesi, önceki kaynak/kategori alanlarını etkiler.
4. Görünen ad `description ?? kategori adı`dır. Ad alanı yok; son alan “Açıklama (isteğe bağlı)”dır.
5. Plan tanımı tek başına gelir/gider oluşturmaz. Planlanan okuma, veritabanında dönem kaydı oluşmamış tarihleri de gösterir; mevcut dönem kaydıyla aynı tarihi çoğaltmaz.
6. Gerçekleştirme üretilmiş kalemde dönem kimliğiyle, henüz üretilmemiş kalemde plan kimliği + vade ile yapılır.
7. Normal tekrarlayanın ödeme panelinde tutar/kaynak yalnız gösterilir; kullanıcı gerçekleşme gününü seçer. Varsayılan bugün; ileri tarih seçilemez. Erken ödeme mümkündür.
8. Erken ödenen kalemden önce bekleyen dönem varsa sunucu açık sıra atlama onayı ister. Onay önceki dönemleri kapatmaz.
9. Hesaptan gerçekleşme normal işlem, karttan gerçekleşme kart harcaması üretir. Kart harcaması banka hesabından ödeme değildir.
10. Plan satırına dokununca yalnız duraklat/sürdür ve sil paneli açılır. Düzenleme, dönem geçmişi ve geri alma normal plan yönetiminde sunulmaz.

### Sunucuda zaten bulunan fakat normal plan arayüzünde sunulmayanlar

| Olanak | Kod gerçeği / sınır |
|---|---|
| Plan düzenleme | `PUT recurring-transactions/{id}` ve `UpdateRecurringTransactionUseCase` var; yalnız vergiye kilitli değil. |
| Tutar/kaynak düzeltme | Gerçekleştirme sözleşmesi bu dönemin tutarını ve ödeme kaynağını alabilir. `SetOccurrenceAmountUseCase` ayrıca dönem tutarı yazar. |
| Geri alma | `occurrences/{id}/undo`; üretilen hareket iptal edilir, dönem yeniden bekler. Toplu ödemeyle kapatılmış dönemin yolu farklıdır. |
| Plan düzenlemesinin etkisi | Ödenmiş/kapatılmış dönemler korunur. Ritim değişirse bekleyen dönemler yeniden kurulur; başlangıç son kapanmış vadenin sonrasına konmalıdır. |
| Tutar takibi | Ritim değişmezse `FollowPlan`, bekleyen kalemin tutarı önceki plan tutarına eşitse yeni tutarı uygular; farklıysa korur. Açık bir “bu döneme özel” bayrağı yerine tutar eşitliği kullanılıyor. Tasarım bunun sınırını gözetmeli. |

Bu tablo koddan doğrulandı; bu oturumda uçlara yazma isteği gönderilmedi.

## 4. Bulgular ve tasarımın çözmesi gerekenler

| No | Bulgu | Kullanıcıya etkisi |
|---|---|---|
| B1 | Özet plan okuması hata alınca listeler boşaltılıyor. | “Bu hafta ödeme yok” yanlışlıkla gösterilebilir. |
| B2 | İşlemler'in planlanan şeridi yalnız ilk açılışta okunuyor. | Yenileme/kapsam/başka ekrandaki işlem sonrası sayı eski kalabilir. |
| B3 | Normal plan oluşturma, duraklatma, sürdürme ve silme yalnız kendi listesini yeniliyor; gerçekleştirme dışında ortak değişiklik sinyali yok. | Diğer açık görünümlerin ve hatırlatmaların güncellenmesi ayrıca ele alınmalı. |
| B4 | Planlananlar kapsamlı, eski Yaklaşanlar kapsamsız; kapsam seçimi kart ekstresini planlanan okumadan çıkarıyor. | Aynı ödeme ekranlar arasında kaybolmuş görünür. Çözüm kullanıcıya gösterilen davranış kararı gerektirir. |
| B5 | Tek bilinmeyen tür `FormatException` üretebilir. | Bütün liste bozulabilir; sessizce atlamak da listeyi eksiksiz gösterme riski taşır. |
| B6 | Vergi/kart eylemi genel sayfaya gidiyor. | Seçilen kaydın yeniden bulunması gerekir. |
| B7 | Özet'in `upcomingHiddenCount` değeri kartta sunulmuyor; sunucudaki bilinmeyen tutar sayaçları istemci plan modelinde okunmuyor. | Kırpılan kayıtlar ve eksik toplam yeterince görünür değil. |
| B8 | Ödeme yükümlülüğü sınıflaması kart taksitlerini ve kart kaynaklı tekrarlayan giderleri de içerebilir. | Mevcut “7 günde çıkacak” etiketinin banka/kasa nakit çıkışıyla eşdeğer olduğu varsayılamaz. Toplamlar kaynak bazında ayrıca incelenecek. |
| B9 | Duraklatılan planın bekleyenleri ortak listelerden düşer; bu yalnız gelecekte yeni kalem üretmeyi durdurmak değildir. | “Yeni kayıt üretmez” metni sonucu eksik anlatır. Aboneliği sona erdirmenin mevcut borcu gizlememesi ayrıca tasarlanmalı. |
| B10 | Planlama sayfasının 7 okuması birlikte başarısız olabilir. | Rapor hatası plan yönetimini de engelleyebilir. |
| B11 | “Planlananlar raporlara girmez” bütün türler için fazla genel. | Ekstre içindeki harcama zaten gider olabilir; ödeme ikinci gider değildir. |

## 5. İlk ekran ve akış önerileri — henüz nihai karar değil

### Plan oluşturma

- Normal plan: üstte **Plan adı**, ardından gelir/gider niyeti, tutar, tekrar ve ilk vade; kaynak ve kategori. Tam sıra görsel denemede sınanacak.
- Abonelik: hizmet seçimi (logo + ad + arama + kendi hizmeti) → abonelik bilgileri. Hizmet, plan adı, kategori ve not birbirinin yerine geçmez.
- Ad zorunluluğu henüz kararlaştırılmadı. Logo katalogdaki hizmet kimliğinden gelir; açıklama/kategori adına göre tahmin edilmez. Özel abonelikte ortak bir simge kullanılabilir.
- Bitiş/tekrar sınırı ve yalnız ilgili ritimlerde gereken ay sonu seçimi ikinci seviyede açılır. Mevcut davranış gizlice değiştirilmez.

### Plan ayrıntısı

Üstte plan adı ve durumu → sıradaki/bekleyen dönemler → geçmiş gerçekleşmeler → plan bilgileri → düzenle/duraklat/sil. Aynı döneme Planlananlar ve Vergiler'den erişilince kimlik ve eylem korunur.

Üç kullanıcı niyeti ayrı anlatılmalı:

- **Bu ödemeyi düzelt:** örneğin yalnız Kasım tutarı farklı; planın sonraki tutarı değişmez.
- **Planı düzenle:** sonraki dönemlerin kuralı değişir; bugünkü sunucu geçmişteki bekleyen kalemleri de etkileyebilir. “Aralık'tan itibaren zam” gibi etkili tarih desteği varmış gibi tasarlanmaz.
- **Gerçekleşmeyi geri al:** yanlış hareket iptal edilir; dönem yeniden bekler. Geçmiş fiziksel silinmez.

### Abonelik / deneme

Deneme bitişi, ilk ücretli ödeme ve yenilenme tarihi ayrı anlamlar taşır. Deneme hatırlatması para hareketi değildir; ödeme toplamına girmez. Tarih geldi diye gider otomatik gerçekleşmez.

Kararlaştırılacaklar: deneme bitişinden kaç gün önce bildirim; bildirim izni yokken görünüm; fiyat bilinmiyorsa kayıt imkânı (bugün normal plan tutarı zorunlu); ücretli döneme geçişin onayı; hizmette iptal ile uygulamada takibi durdurma ayrımı; aylık/yıllık ücret sunumu; logo kaynakları. Gerçek hizmet fiyatı veya ücretsiz deneme koşulu katalogdan doğru kabul edilmez.

**Sonraki kullanıcı onayı — sade etkileşim:** alanlar ve seçenekler ihtiyaca göre açılır; tasarım açıklama paragraflarıyla taşınmaz. Önce hizmet/ücret/yenilenme; “Deneme sürümündeyim” seçilince bitiş ve “Denemeden sonra → Devam edeceğim / İptal edeceğim”. Hatırlatma ayrıntıları ikinci seviyede. Teyit sorusu kurulumda değil zamanı geldiğinde ilgili kayıtta. Listede “Deneme · 3 gün kaldı” gibi kısa durum. Açıklama yalnız kritik eylemin sonucunu kısa ve yerinde anlatır; büyük yazı/dokunma ölçüleri sıkıştırılmaz. Bu onay kesin piksel düzeni veya bildirim zamanlaması kararı değildir.

### Ortak filtre ve tarih

`AppFilterSheet`, `AppFilterSegments`, `AppFilterSelectField`, `AppFilterMenu` ve ortak tarih seçiminin dili kullanılır. Seçimler panelde taslaktır; “Sonuçları göster” uygular. Başlıkta filtre düğmesi ve etkin filtre sayısı, listede kaldırılabilir etiketler.

Kartın ekstre tarihi mantığı bütün planlara kopyalanmaz. Genel planlamada takvim dönemi kullanılır. Bugünkü 7/30/90 gün sözleşmesi özel tarih aralığı desteği değildir. 18 Kasım kararı: özel aralığa vade tarihiyle girilir. Aralıktan önce vadesi geçmiş açık ödemeler ayrı açılır satırda gösterilir ve dönem toplamına girmez. Dönem içindeki gecikmiş ödeme kendi satırında işaretlenir ve dönemde yalnız bir kez sayılır.

### Özet / Yaklaşanlar ve diğer widget'lar

- Mevcut tarih sütunu ve zaman çizgisi değerlendirilecek; bütün kartı baştan değiştirmek zorunlu değil.
- Tür kapsamı genişler, görünür satır sayısı sınırlı kalır; kalan sayı görünür olur. Liste eksikliği ile veri hatası birbirinden ayrılır.
- Vergi/abonelik/ekstre/kart taksidi gibi anlamlı alt etiketler, her satırdaki “Tekrarlanan” yerine değerlendirilir.
- Gecikenler bağlantısı gecikenleri; “Tümünü gör” aynı dönemi açar. Tek satır kaydın ayrıntısına gider.
- Önerilen sıra: Ayın özeti → Yaklaşanlar → Kategori giderleri → Bütçeler → Varlık durumu → Hesap bakiyeleri. **Kullanıcı tarafından nihai sıralama olarak onaylanmadı**; kullanıcı kategori/bütçenin arada kalmasını önemsiyor.
- Varlık durumu için bugün bir genel ayrıntı ekranı yok. Olmayan ekrana “Ayrıntı” bağlantısı önerilmez. Bu bölüm ve hesap bakiyeleri Özet turunda mevcut erişim yollarıyla incelenir; yeni rapor ekranı gerektirmeyen iyileştirmeler aynı çalışmaya alınabilir.
- Boy azaltmak için yazı/dokunma hedefleri küçültülmez; tekrar eden bilgi ve gösterilen ayrıntı değerlendirilir.

## 5a. Senaryo incelemesi ve açık davranış kararları

Bu bölüm uygulama talimatı değildir. Kullanıcıya sohbette sunulan öneriler ile çalışan kodun farkını kaydeder. Tutarlar ve tarihler sentetiktir.

| ID | Kullanıcı senaryosu | Mevcut kod / eksik | Önerilen deneyim |
|---|---|---|---|
| S1 | 19 Kasım vadeli 12.500 TL kira 17 Kasım'da ödendi | Erken gerçekleştirme var; `paidOn` gerçekleşme gününü belirler. | Kayıt paneli vadeyi ayrı gösterir; ödeme günü bugünle açılır. Tek dönem kapanır. |
| S2 | Kasım faturası normalden farklı geldi | API dönem tutarı ve kaynak değişimini alır; normal plan paneli yalnız günü sorar. | Tutar/kaynak plandan dolu gelir, bu ödeme için değiştirilebilir. Planın sonraki tutarı değişmez. Önce bu ayrımın metni tasarlanmalı. |
| S3 | Aralık'tan itibaren kira 15.000 TL; Kasım hâlâ 12.500 TL olarak bekliyor | `FollowPlan` önceki tutara eşit bekleyenleri de değiştirir. Etkili başlangıç dönemi sözleşmesi yok. | D1: fiyat değişikliği bir dönemden itibaren uygulanır; eski bekleyen ve gerçekleşmiş dönemler korunur. Backend tasarımı gerektirir. |
| S4 | 12.500 TL olan bir döneme kullanıcı yine 12.500 TL yazdı; sonra plan değişti | `FollowPlan` değeri eşit olduğundan onu özel tutar saymaz. | “Özel tutar her koşulda korunur” vaadi mevcut kodla kurulamaz. Kullanıcı niyetini izleme gereksinimi D1'in teknik incelemesinde değerlendirilir. |
| S5 | Kira 17 Kasım'da yanlış kaydedildi | Sunucu undo ile hareketi iptal edip dönemi bekleyene döndürür; normal plan UI yolu yok. | Gerçekleşmiş dönem ayrıntısından geri alma; sonuç açıklanır, geçmiş silinmez. |
| S6 | Abonelik hizmette iptal edildi, geçmiş bir dönem hâlâ bekliyor | `Deactivate` yalnız `IsActive=false`; planlanan okuma duraklatılan planın mevcut bekleyenlerini de gizler. | D2: sona erdirme ileri tarihleri keser, geçmiş bekleyenleri kendiliğinden silmez/kapatmaz. Ayrı durum ve bitiş semantiği gerekir; mevcut active bayrağını yeniden adlandırmak yeterli değil. |
| S7 | Plan bir ay duraklatılıp sonra sürdürülüyor | `Activate` yalnız aktif bayrağını açar; sıradaki tarih ileri alınmaz. Projection eski tarihten tekrar yürür. | Duraklatılan aralığın atlanması mı, yeniden beklemesi mi istendiği ayrıca çözülmeli. Mevcut davranış “o aylar atlandı” değildir. D2'nin alt sorusu; ilk görüşmede kullanıcıya bütün kenar durumlar yığılmayacak. |
| S8 | Deneme 1 Aralık'ta ücretli olacak | Hizmet/deneme kimliği ve erken hatırlatma alanı yok. Normal planın tutarı zorunlu. | D3: bitiş öncesi hatırlatma + ücretli ilk tarih; o gün planlanan ödeme gösterilir ama gerçek gider açık onaysız oluşmaz. Denemeden ücretli plana ikinci onayın gerekli olup olmadığı sohbette sorulacak. |
| S9 | Deneme iptal edildi, ücret çekilmedi | Mevcut plan durumu hizmette iptali temsil etmez. | “Hizmette iptal ettim” kullanıcının beyanıdır; sağlayıcıya iptal isteği gönderilmez. Ücretli gelecek kalemlerin kaldırılması açıkça gösterilir. |
| S10 | Abonelik karttan alındı, ekstre daha sonra hesaptan ödenecek | Gerçekleşme `CreditCardCharge` üretir. Planlanan ödeme yükümlülüğü sınıflaması yalnız nakit çıkışı demek değildir. | Aynı para iki kez “hesaptan çıkacak” sayılmaz. Kart harcaması ve kart ödemesi toplamları ayrıca kaynak incelemesinde ele alınır. |
| S11 | Fiyatı bilinmeyen deneme kaydedilecek | Normal tekrarlayan planın `Amount` değeri boş olamaz; vergi istisnası var. | Kapanışta D6 ile şimdiki akıştan çıkarıldı: kullanıcı deneme sonrasında teklifin belirttiği ücreti esas almak istiyor. Verginin bilinmeyen tutar istisnası aboneliğe genellenmez. |

### Sohbette görüşülecek kararlar

| ID | Öneri | Durum | Neyi etkiler? |
|---|---|---|---|
| D1 | Tek dönem tutarı ile “seçilen dönemden itibaren yeni tutar” ayrı eylem olsun. Geçmiş bekleyenler yeni fiyatla değişmesin. | Kabul edildi: kullanıcı “bunun haricinde önerilerine katılıyorum” | Plan düzenleme sözleşmesi, dönem/tutar modeli, önizleme ve geçmişin korunması |
| D2 | Hizmette abonelik iptali/planı sona erdirme, duraklatmadan ayrı olsun; geçmiş bekleyenleri kendiliğinden gizlemesin. | Kabul edildi: aynı kullanıcı yanıtı | Yaşam döngüsü ve planlanan görünüm; mevcut duraklatma davranışı değiştirilmeden ayrıca tasarlanmalı |
| D3 | Deneme eklerken kullanıcının yalnız denemeyi mi kullanacağı, ücretli devam mı edeceği sorulur. İki durumda da bildirim yapılır. | Kullanıcı yönlendirmesiyle değiştirildi; ayrıntılar tasarım önerisi | Niyet, gerçek iptal ve gerçek ödeme ayrı bilgiler. Eski “niyet sormadan ücretli takibi sürdür” önerisi artık geçerli değil. |

**D3 uygulama öncesi ayrım:** “Yalnız denemeyi kullanacağım” gerçek iptal değildir. Tarih gelince aboneliği kendiliğinden iptal edilmiş veya ödeme riskini bitmiş sayma. Önerilen akış: bitiş öncesi iptal hatırlatması; bitişte “İptal ettim / Devam ediyorum” teyidi; yanıt yoksa kontrol bekleyen uyarı. “Ücretli devam edeceğim” için yenilenme tarihi/tutarı hatırlatılır; gerçekleşmiş gider yine açık onay gerektirir. Bu teyit akışı ve bildirim zamanları henüz kullanıcı onaylı değil. Ücretsiz dönem her hizmette bir ay varsayılmaz; kullanıcının girdiği tarihe göre çalışır. Bildirim cihaz iznine bağlıdır, izin yokken uygulama içi uyarı gerekir.

**Kapanışta D6 netleşti:** deneme formu hizmetin teklifinde belirtilen **deneme sonrası ücret** ile kurulur. Tutarı katalogdan güncel fiyat diye otomatik kabul etme; kullanıcı kendi teklifindeki tutarı girer. Önceki “ücret bilinmese de bağımsız hatırlatma ekleyelim” ajan önerisi seçilmedi. Bu karar vergi tutarı bilinmeme davranışını değiştirmez. Yukarıdaki daha eski açık soru listelerinde geçen “fiyat bilinmiyorsa” maddesi D6 ile bu teslim için kapanmıştır.

Yeni durum/model önerileri bağlayıcı ADR'lerle henüz tam karşılaştırılmadı. Uygulama öncesi mevcut kurallarla çelişki ve veri/API etkisi incelenir; bu belge bir migration veya bağlayıcı ilke değişikliğini yetkilendirmez.

## 5b. Planlanan toplamlarının kod incelemesi

### Gözlem — 17 Kasım 2026

`ListPlannedActivitiesUseCase` tüm satırları `PlannedActivityRules.IsPaymentObligation` ile süzer; gecikmiş ve bugün/gelecek ayrı `Amount` toplamlarına ayrılır. Kural gelir ve tahsilat eylemlerini dışarıda bırakır; kart taksidini ve kart kaynaklı tekrarlayan gideri dışarıda bırakmaz. Dolayısıyla `UpcomingOutgoingTotal` yalnız banka/kasa nakit çıkışı değildir. Özet'in “7 günde çıkacak” etiketi bu ayrımı taşımıyor.

**Çifte sayım hakkında sınır:** bu okuma her mevcut taksidi aynı anda ekstrede tekrar sayıyor sonucu çıkarılmadı. Taksit okuması `CreditCardChargeId == null` kalemleri alırken ekstre mevcut gerçekleşmiş kart hareketlerinden hesaplanıyor. Doğrulanan sorun farklı finansal etkilerin tek çıkış toplamına katılmasıdır. Aynı kart giderinin ileri vadeli nakit etkisi ancak ekstre/ödeme ilişkisiyle hesaplanabilir; basitçe bu toplamı nakit tahmini diye sunma.

| Kaynak | Bugün listedeki etki | Tasarımda ayrılacak anlam |
|---|---|---|
| Hesap kaynaklı tekrarlayan gider | Expense / Realize | Planlandığı biçimde gerçekleşirse hesap bakiyesi azalır. |
| Kart kaynaklı tekrarlayan gider | Expense / Realize | Kart borcu artar; o gün banka/kasa bakiyesi azalmaz. |
| Kart taksidi | Expense / Realize | Karta yansır; hesap ödemesi değildir. |
| Kart ekstresi | Neutral / PayCard | Hesaptan ödeme yükümlülüğü; ikinci gider değildir. |
| Tekrarlayan gelir | Income / Realize | Beklenen giriş; henüz bakiye değildir. |
| Borç/alacak/kişi vadeleri | Mevcut borç-alacak eylemleri | Yön gösterilir; ödeme yöntemi seçilmeden tümünü banka çıkışı sayma. Bu türlerin yazma anlamı bu çalışmada değiştirilmez. |
| Kaynağı seçilmemiş vergi | Expense / Realize, source boş olabilir | Meşru plan; hesap/kart ödeme anında seçilir. Nakit çıkışı veya kart harcaması diye uydurulamaz. |
| Deneme bitişi (önerilen yeni kayıt) | Henüz mevcut değil | Hatırlatma; para toplamına dahil değil. |

Planlanan DTO `sourceId/sourceName` taşıyor ama tekrarlayan satır için `sourceType` taşımıyor. `effect=expense` veya `isPaymentObligation=true` hesap/kart ayrımını çözmez. İstemci adları inceleyerek veya bütün hesap/kart listelerini çekerek finansal sınıflama üretmemeli. Olası yeni kararlı sınıflama alanı ve sunucu toplamları teknik teslimde ele alınacak; alan adı/sözleşme henüz kararlaştırılmadı.

### Tasarım önerisi — henüz kullanıcı onaylı değil

- Tek büyük “çıkacak” toplamı yerine **Ödemeler / Kart harcamaları / Beklenen girişler** anlamları ayrılabilir. Ödemeler, yöntemi henüz seçilmemiş borç/vergi gibi kalemleri de içerebileceği için “kesin banka çıkışı” diye etiketlenmez.
- Toplamlar birbirinden mahsup edilerek “kalan para” üretilmez. Bilinmeyen tutar sıfır değildir; ilgili toplamın yanında kısa `+1 tutar belirsiz` gibi bir gösterim denenir (metin henüz kesin değil).
- Özet'te bütün ayrımlar için büyük metrik kartları eklenmez. Satır alt etiketi (`Karttan çekilecek`, `Kart ödemesi`, `Beklenen gelir`) ve kısa alt toplam satırları prototipte değerlendirilir; yalnız ilgili veri olduğunda görünür.
- Gecikmiş toplam seçilen gelecek dönem toplamına karışmaz. Filtre sonrası tutar ile listenin aynı seçimi temsil etmesi gerekir; finansal toplam sunucudan gelir.
- Bugünkü kapsam filtresi kart ekstresini kaldırıyor. Onaylanan D5 davranışı için ortak planlanan okuma, kapsam seçiliyken de kesilmiş ekstrenin kalan borcunu sunmalı.
- Takvim ufku bugün `asOfDate.AddDays(daysAhead)` ve üst sınır dahil olarak çalışır. “7 gün” etiketinin bugünü dahil sayması ilk/son gün senaryosuyla netleştirilecek; mevcut sözleşme sessizce değiştirilmeyecek.
- Ekstre projection'ı her kart için referans tarihe göre bir kesilmiş dönem hesaplıyor; 90 günlük seçimin üç gelecek ekstre tutarını bildiği varsayılmamalı. Gelecek ekstre tahmini ayrı ürün davranışıdır, bu çalışmaya kendiliğinden eklenmez.

### Tasarım kabulünde kullanılacak sentetik örnek

19 Kasım kira 12.500 TL (hesap), 20 Kasım abonelik 200 TL (kart), 21 Kasım taksit 1.000 TL (kart), 22 Kasım mevcut ekstre 4.000 TL, 23 Kasım beklenen gelir 3.000 TL. Bugünkü çıkış toplamı 17.700 TL olur; bunun 1.200 TL'si hesaptan çıkacak ödeme değil karta eklenecek harcamadır. Önerilen gösterim: ödemeler 16.500 TL; kart harcamaları 1.200 TL; beklenen girişler 3.000 TL. Bu rakamlardan net bakiye türetilmez. Kaynağı bilinmeyen vergi eklendiğinde bunun kesin nakit çıkışını söyleyemediğimiz ayrıca sınanır. Örnek hesap elle kontrol edildi; API/SQL testi çalıştırılmadı.

## 5c. Ekstre ile bekleyen kart taksidinin ilişkisi

Kullanıcı önceki örnekte 1.000 TL taksidin 4.000 TL ekstre içinde olup olmadığını ve 90 günlük pencerenin anlamını sordu. Önceki örnekte kesim tarihi belirtilmemişti; aşağıdaki tarihli örnek onun açıklamasıdır.

### Kanıt zinciri

1. `EfPlannedActivityRepository.ListInstallmentsAsync`: yalnız `CreditCardChargeId == null` taksitleri alır.
2. `RealizeInstallmentUseCase`: daha önce gerçekleşmiş taksitte mevcut harcamayı döndürür; yeni harcamanın `ChargeDate` değeri `item.ScheduledDate` olur. Item bu harcamanın kimliğiyle işaretlenir.
3. `EfInstallmentPlanRepository.RealizeAsync`: izlenen taksit değişikliği ve yeni harcama aynı `SaveChangesAsync` ile kaydedilir.
4. `CardDebt.ChargeRows`: iptal edilmemiş gerçekleşmiş kart harcamalarını ve kartla borç/kişi ödemelerini okur; bekleyen taksit planlarını okumaz.
5. `ListStatementsAsync`: bu hareketleri ekstre kesim dönemine ve önceki dönem devrine ayırır. `CreditCardStatement.Create` ödemeleri ve alacaklı devri dikkate alır. Kesim günü henüz kesilmiş sayılmaz.

Sonuç: normal yazma/okuma yolunda **aynı gerçekleşmiş taksit hem bekleyen taksit satırı hem ekstre satırı değildir**. Bu kaynak incelemesi mevcut bir çifte sayım hatasını kanıtlamadı; bütün sınır durumlar test edilmiş sayılmaz.

### Tarihli örnek (tamamı sentetik)

- Bugün 17 Kasım 2026; kart kesimi ayın 15'i, son ödemesi 22'si.
- Listelenen son kesilmiş ekstre 16 Ekim–15 Kasım hareketlerini ve varsa önceki dönemden kalanı kapsar; 22 Kasım'da kalan 4.000 TL ödenecektir.
- 20 Kasım aboneliği 200 TL ve 21 Kasım taksidi 1.000 TL henüz gerçekleşmemiştir. **4.000 TL'ye dahil değildir.** Gerçekleşince 16 Kasım–15 Aralık dönemine girer; ödeme günü 22 Aralık olur.
- Aynı taksit planının 21 Ekim taksidi gerçekleşmişse 15 Kasım ekstresinde olabilir. Bu “her zaman bir önceki ay” kuralı değil, harcamanın tarihinin kesim dönemine düşmesidir.
- Kira 12.500 TL eklenince önceki örnekte 16.500 TL ödeme ve 1.200 TL karta yansıyacak harcama ayrımı bu varsayımlarla anlamlıdır. Ödeme yöntemi bilinmeyen diğer kayıtlar için banka çıkışı iddiasına genellenmez.

### Hata / eksiklik riskleri (henüz düzeltme yok)

- **Geç gerçekleşen geçmiş taksit:** 10 Kasım taksidi 17 Kasım'da hâlâ bekliyorsa ekstreye dahil değildir. 17 Kasım'da gerçekleştirilince 10 Kasım'a yazılır ve 15 Kasım için yeniden hesaplanan ekstre değişir. Ekstre kalıcı banka belgesi değil projection'dır. Kullanıcı unutmuşsa gösterilen ekstre banka ekstresinden eksik kalabilir; geç kaydı otomatik gerçekleşmiş saymak mevcut açık onay kuralını değiştirir ve yapılmayacak.
- **Yeni döneme devreden eski borç:** her kart için en son kesilmiş dönem sunulur; eski borç yeni döneme taşınabilir. Bu görünüm eski ekstrelerin bağımsız vade geçmişini saklamaz. Yeni kesimde gecikme görünümünün etkisi için ayrı senaryo testi gerekli; “gecikmeler hep doğru korunuyor” iddia edilmez.
- **Kart alacağı / kısmi ödeme / kesim günü:** toplamların kontrolünde bunlar ayrı sınanmalı. Alanları yeniden toplayıp Domain hesabını istemciye kopyalamak yasak.
- **Gelecek ödeme tahmini eklenirse:** karta yansıyacak 1.000 TL'yi hem kart harcaması toplamı hem gelecekteki ekstre ödemesi olarak aynı genel toplamda saymak çifte sayım yaratır. Gelecek ekstre tahmini bu oturumda kabul edilmedi.

### 90 günün bugün anlamı

`daysAhead=90` yalnız üst tarih sınırını büyütür. Tekrarlayan tarihler ve taksitler bu sınıra kadar döner. Ekstre okuması ise kart başına referans tarihe göre **bir son kesilmiş dönem** hesaplar; Aralık/Ocak/Şubat için döngüyle yeni ekstre üretmez. Örnekte Kasım'ın 4.000 TL kalan ekstresi dönebilir, Aralık/Ocak taksitleri de ayrı satırlarda görülebilir; bunların gelecekteki ekstre toplamları ayrıca yoktur. “90 gün” = kayıtların görüntülendiği pencere; eksiksiz gelecek banka çıkışı tahmini değildir.

Kaynaklar: `src/BusinessFinance.Domain/CreditCardStatement.cs` (period sınıfı da aynı dosyada), `src/BusinessFinance.Application/CreditCards/InstallmentUseCases.cs`, `src/BusinessFinance.Infrastructure/CreditCards/EfInstallmentPlanRepository.cs`, `CardDebt.cs`, `EfPlannedActivityRepository.cs`.

Okunan testler: `CreditCardStatementTests` dönem/devir/ödeme/kesim sınırları; `SqlServerPersistenceIntegrationTests.PlannedActivities_ProjectEveryKindWithReadinessAndNoDuplicates` kaynakların görünümü. Bu oturumda testler çalıştırılmadı. Bu test adlarının varlığı geç taksit → yeniden ekstre hesabı senaryosunun test edildiğini kanıtlamaz.

## 6. Çalışma sırası ve çıkışları

| Adım | Çıktı | Durum |
|---|---|---|
| 1. Kod ve akış haritası | Okumalar, mevcut form, gerçekleştirme ve düzenleme sınırları | İlk okuma tamamlandı; toplam sınıflaması ve kapsam incelemesi devam edecek |
| 2. Senaryolar ve davranış kararları | Kira, abonelik/deneme, vergi, kart ve tahsilat senaryoları; tek dönem / plan ayrımı | D1–D2 kabul, D3 niyet sorusuyla güncellendi; deneme sonu ayrıntıları açık |
| 3. Görsel taslak | Planlananlar + filtre/tarih + plan ayrıntısı + abonelik oluşturma | Vade ajandası kullanıcı incelemesini bekliyor. Sonra plan ayrıntısı, tek dönem ödemesi ve abonelik |
| 4. Özet ve İşlemler bağlantısı | Kısa görünüm, eylem hedefleri, yenileme ve hata durumları | Başlamadı |
| 5. Teslim | Onaylanan davranışlar, açık kararlar, mevcut API ile yapılabilenler ve gereken değişiklikler | Başlamadı |

Her adımda kullanıcıya somut farklar gösterilir. Davranış değişiklikleri öneriden karara geçirilirken kullanıcı kararı kaydedilir. Teslim ortak ölçü/bileşen adlarını kullanır; borç-alacak sınırı tekrar kontrol edilir.

### Senaryo kontrol listesi

- Kira erken/geç ödenir; ödeme tarihi ile vade karışmaz.
- Bu ay farklı tutar, gelecek ay kalıcı fiyat artışı ve geçmiş ödeme birbirinden ayrılır.
- Deneme biter; ilk ücret, hatırlatma ve gerçek gider birbirine karışmaz.
- Kart kaynaklı abonelik + ekstre aynı nakit çıkışı gibi toplanmaz.
- Verginin tutarı bilinmez; toplam sıfır veya eksiksizmiş gibi sunulmaz.
- Duraklat/sürdür/sil/geri al sonrası bütün ilgili ekranlar güncellenir.
- Çok gecikmiş kayıt, çok yaklaşan kayıt, hiçbir kayıt olmaması ve filtre sonucu boş olması ayrı denenir.
- Bilinmeyen tür ve ağ hatası eksik listeyi “ödeme yok”a dönüştürmez.
- Büyük yazı, dar ekran, koyu tema ve ekran okuyucuda ortak bileşen davranışı korunur.

## 7. Sonraya bırakılan işler ve fikirler

| Fikir / iş | Neden şimdi değil? | Ne zaman yeniden ele alınır? |
|---|---|---|
| Uygulama genelinde ad/açıklama/not ayrımı | Önce tekrarlayan ve abonelikte çözülmesi istendi. Diğer formlar yeterince incelenmedi. | Ana gelir/gider ve diğer kayıt formlarının tasarım turunda |
| Kullanıcının Özet'e widget ekleyip düzenlemesi | Ana sayfalar henüz tamamlanmadı; kullanıcı erken buluyor. | Raporlar ve Özet özelleştirmesi çalışmasında |
| Raporların grafik ve görsel özetlerle genişlemesi | Ayrı ürün kapsamı; mevcut Planlananlar işini büyütmemeli. | Raporlar aşaması kullanıcıyla açıldığında |
| Varlıklar için yeni ayrıntı/rapor ekranı | Bugün yok; sadeleştirmeyi olmayan hedefe bağlayamayız. | Raporlar çalışmasında ihtiyaç doğrulanırsa |
| Gelecek, henüz kesilmemiş ekstrelerin tahmini | Kullanıcı bu çalışmaya alınmamasını kabul etti; yapılmamış harcamalar üzerinden ekstre gösterilmesini uygun bulmuyor. | Kullanıcı ileride açıkça nakit tahmini talep ederse yeniden tartışılabilir; otomatik sonraki aşama işi değildir. |

Abonelikler ve deneme takibi ertelenmiş sayılmaz: bu çalışmanın araştırılacak tasarım alanıdır. Kapsamı henüz uygulama kararı değildir.

## 8. İlerleme günlüğü

### 17 Kasım 2026 — ilk kayıt

- Kullanıcı görüşmesindeki yön, düzeltmeler, açık öneriler ve ertelenen fikirler bu belgede birleştirildi.
- Tekrarlayan formu, liste paneli, ödeme günü paneli, istemci repository/controller, sunucudaki düzenleme/gerçekleştirme/geri alma yolları ve mevcut hatırlatma planlayıcısı okundu.
- Plan oluşturma/duraklatma/silmede ortak yenileme sinyali eksikliği ve sunucuda var olup normal planda sunulmayan olanaklar kaydedildi.
- Uygulama kodu, aşama belgesi ve borç-alacak çalışması değiştirilmedi. Branch değiştirilmedi, commit atılmadı.
- Build/test/analyze ve cihaz turu yapılmadı: yalnız kaynak incelemesi ve tasarım belgesi.
- Sıradaki tek görev: senaryolar üzerinden **bu dönemin ödemesi / planın geleceği / abonelik denemesi** davranışlarını ayrıştırmak.

### 17 Kasım 2026 — senaryo incelemesi ve belgeyi devir belleğine dönüştürme

- Kullanıcı belgenin kendisi için sunum değil, sonraki ajan/oturum için ayrıntılı çalışma belleği olmasını istedi. Sorular ve öneriler sohbette konuşulacak. En üste devam yönergesi eklendi.
- `RecurringTransaction.Reschedule`, `Deactivate`, `Activate`, `AdvanceAfter`; `RecurringTransactionOccurrence.FollowPlan`; `EfPlannedActivityRepository.ListRecurringAsync`; `RealizeRecurringOccurrenceUseCase` okundu.
- Yeni somut bulgu: duraklat/sürdür tarih aralığı kaydetmiyor veya sıradaki tarihi ileri almıyor. Sürdürmeyi “duraklatılan aylar atlanır” diye tarif etmek yanlış olur.
- 11 sentetik senaryo mevcut API/domain sınırlarıyla eşlendi. D1–D3 kullanıcıya öneri olarak sunulacak; yanıt gelmeden onaylıya çevrilmeyecek.
- Yalnız bu belge değiştirildi. Kod, branch ve commit değişmedi; build/test/analyze veya cihaz testi yapılmadı.
- Devam: sohbet yanıtları → D1–D3 durumları → ortak listedeki nakit çıkışı/kart harcaması/tahsilat toplamlarının kaynak incelemesi.

### 17 Kasım 2026 — kullanıcı kararları

- D1 (dönemden itibaren fiyat değişimi) ve D2 (sona erdirme, geçmiş bekleyenleri koruma) kabul edildi.
- Kullanıcı D3'e deneme sonu niyet sorusu ekledi: yalnız deneme / ücretli devam. İki durumda da bildirim istedi. Önceki öneri buna göre değiştirildi.
- Sohbette sunulan sonraki öneri: yalnız deneme niyeti gerçek iptal sayılmaz; bitişte teyit ve yanıt yoksa kontrol bekleyen görünüm. Bu ayrıntı kabul edilmiş sayılmamalı.
- Yalnız belge değişti; kod, branch, commit ve test durumu aynı. Sonraki iş ortak planlanan toplamlarının incelenmesi.

### 17 Kasım 2026 — sade tasarım onayı ve toplam sınıflaması

- Kullanıcı açıklama yükünü azaltan, alanları gerektiğinde açan abonelik yaklaşımını kabul etti. İlgili not §5'e eklendi.
- Okunanlar: `ListPlannedActivitiesUseCase`, uygulama/API planlanan DTO'ları, `BuildRecurring`, `ListInstallmentsAsync`, `ListStatementsAsync`, mevcut toplam testleri; ADR 0013 ve ADR 0020'nin kapsam ilkeleri.
- Kanıt: mevcut “çıkacak” toplamı kart kaynaklı harcamayı da içeriyor; bunun kesin kasa/banka çıkışı olmadığı doğrulandı. Aynı taksidin ekstreyle mevcut okumada mutlaka iki kez sayıldığı iddia edilmedi.
- DTO'da tekrarlayanın kaynak türü eksikliği, ödeme yöntemi belirsiz kayıtlar, bir kart için tek kesilmiş ekstre ve tarih ufku sınırı teslim riskleri olarak not edildi.
- Yalnız bu tasarım belgesi değişti; build/test/analyze ve cihaz/API testi yapılmadı. Branch/commit değişmedi.
- Sonraki iş: ortak ekranın bilgi hiyerarşisi ve filtre-kapsam davranışını somutlaştırmak. Toplam ayrımı önerisini sohbette kullanıcıya anlat; onaylı davranış gibi kodlama.

### 17 Kasım 2026 — taksit/ekstre ilişkisi ve oturum devri

- Kullanıcı bu oturumda ekran tasarımına geçilmemesini, açık kararların ve sonraki başlangıcın netleştirilmesini istedi. Bu sınır korundu.
- Taksit gerçekleştirme → kart harcaması → ekstre okuma zinciri ve tek kayıt sınırı okundu. Önceki 17.700 TL örneği kesim tarihleriyle açıklandı.
- Geçmiş taksidin geç onaylanmasının hesaplanan eski ekstreyi değiştirebildiği kaynak üzerinden saptandı; banka ekstresi eşitliği doğrulanmadı. Yeni dönem devrinde gecikmenin görünümü test gerektiren açık konu olarak tutuldu.
- D4–D6 kararları ile uygulama öncesi teknik kontrol listesi §10'a kondu. Bunlar öneridir; onay yokken karara çevrilmez.
- Yalnız `design/planlananlar/README.md` güncellendi. Uygulama kodu, branch, commit değişmedi. Build/test/analyze ve emülatör/API senaryo testi yapılmadı.

### 17 Kasım 2026 — kullanıcı isteğiyle oturum kapanışı

- Kullanıcı D4'ü açıkça kabul etti: henüz kesilmemiş ekstre tahmini veya gösterimi bu çalışmaya alınmayacak.
- D6 için teklifin yazdığı deneme sonrası ücretin esas alınmasını istedi. Ücreti bilinmeyen ayrı deneme kaydı önerisi şimdiki kapsamdan çıkarıldı; fiyat hizmet kataloğundan uydurulmayacak.
- Kullanıcı D5'i henüz anlamadığını belirtti ve sonraki oturumda açıklanmasını istedi. Sonraki ilk konu: aynı kartın işletme/şahsi harcamaları ile tek ekstre ödemesinin kapsam filtresinde davranışı.
- Kullanıcı “gerekli yerleri güncelle sonra sessionu burada bırakalım” dedi. Yeni araştırma, tasarım veya uygulama işi başlatılmadı.
- Güncel devir en üstte ve §10'da. Tek değiştirilen dosya bu README; `borc-alacak-cari` dalında, son bilinen HEAD `d359ab7`. Belge henüz untracked; commit atılmadı. Diğer kullanıcı değişikliklerine dokunulmadı.
- Bu oturum boyunca build/test/analyze çalıştırılmadı, görsel taslak üretilmedi. Kaynak incelemesi ile çalışma belleği oluşturuldu. Kullanıcıdan yeniden onay gerekmeyen kararlar ile açık D5 ayrıldı.

## 9. Başvuru kaynakları

### Yerel kaynaklar

- `mobile/business_finance_mobile/lib/features/planning/presentation/planning_page.dart`: form, plan satırı ve mevcut eylemler.
- `mobile/business_finance_mobile/lib/features/planning/presentation/planning_controller.dart`: yazma sonrası yenileme.
- `mobile/business_finance_mobile/lib/features/planning/presentation/realize_day_sheet.dart`: gerçekleşme tarihi ve sıra atlama.
- `mobile/business_finance_mobile/lib/features/activities/`: ortak planlanan model, liste, eylem ve İşlemler şeridi.
- `mobile/business_finance_mobile/lib/features/dashboard/presentation/`: Yaklaşanlar daraltması, hata/boş durum ve kart.
- `mobile/business_finance_mobile/lib/features/reminders/`: yerel hatırlatma altyapısı.
- `src/BusinessFinance.Api/Features/RecurringTransactions/RecurringContracts.cs` ve `RecurringEndpoints.cs`: yazma sözleşmeleri.
- `src/BusinessFinance.Application/RecurringTransactions/RecurringUseCases.cs`: düzenleme, dönem tutarı, gerçekleşme ve geri alma.
- `src/BusinessFinance.Domain/RecurringTransactionOccurrence.cs`: `FollowPlan` ve dönem kaynak/tutar değişimi.
- `src/BusinessFinance.Application/FinancialActivities/PlannedActivityContracts.cs`: ödeme yükümlülüğü sınıflaması ve bilinmeyen tutar sayaçları.
- `design/design_handoff_kart_hareketleri/README.md`, `design/vergiler-handoff/README.md`, `documentation/design-system.md`: görsel dil ve mevcut ortak bileşenler.
- `stages/06.3-borc-alacak/README.md`: paralel çalışmanın sınırları; `documentation/adr/0018-tax-is-a-cash-plan.md`: vergi ilkeleri.

### Önceki görüşmede yapılan kısa dış tarama

Bu kaynaklar ürün kararı değildir; kapsamlı abonelik araştırması henüz yapılmadı.

- [Rocket Money — düzenli ödemelerin görünümleri](https://help.rocketmoney.com/en/articles/3117398-where-can-i-view-my-subscriptions-and-bills): aynı alanda tümü/yaklaşan/takvim görünümleri.
- [Rocket Money — elle abonelik ve fatura ekleme](https://help.rocketmoney.com/en/articles/2185531-managing-your-bills-and-subscriptions): elle takip örneği.
- [Apple — abonelik iptali](https://support.apple.com/en-us/118428): deneme bitişinden en az 24 saat önce iptal örneği; bütün sağlayıcılara genellenmez.

## 10. Sonraki oturuma devir — güncel başlangıç noktası

### Henüz karar verilmemiş başlıklar

| ID | Soru | Ajan önerisi / durum |
|---|---|---|
| D4 | Gelecek ekstreler tahmin edilecek mi? | **Hayır, kabul edildi.** Kilit ayrım henüz kesilmemiş ekstre: bu çalışmada tahmin edilmez/gösterilmez. Kesilmiş ekstre ve bekleyen kart harcamaları ayrı kalır. |
| D5 | Kapsam seçimi kesilmiş ekstreyi nasıl etkiler? | **Kabul edildi (18 Kasım).** Hepsi, İşletme ve Şahsi seçimlerinin her birinde aynı ekstre, vade gününde tek bir ekstre satırı olarak görünür. Satır son ödeme gününü ve kalan borcun tamamını gösterir. Harcamaların kapsamı ayrı tutulur. Ekstre ödemesi yeniden gider yazılmaz. |
| D6 | Deneme sonrası ücret nasıl alınacak? | **Kullanıcı yönlendirmesiyle kapandı.** Hizmet teklifinde yazan deneme sonrası ücret kullanıcıdan alınır. Ücreti bilinmeyen bağımsız hatırlatma akışı şimdiki tasarımın parçası değildir. |

Diğer açık ayrıntılar görsel denemeyle netleşebilir: bildirim önceliği/zamanı, plan adı zorunluluğu, gösterilecek satır sayısı, özel aralık sınırı, sonlandırma panelinin tarihi, duraklat/sürdür aralığı, logo katalog kapsamı. Onaylı değilse makul varsayım olarak işaretlenir; davranış değişikliği kodlanmadan kullanıcıya sunulur.

### Önceki çalışma sırası (tarihsel)

1. AGENTS/CLAUDE, proje durumu, aşama zinciri ve aktif aşama başlangıç okumalarını yap; mevcut branch/log/status'u tekrar kontrol et. Önceki temiz olmayan çalışma ağacını düzeltmeye kalkma.
2. Bu dosyanın üst özeti, §2, §5a–5c ve §10'u oku. D4/D6 kapanışta işlendi; tekrar onay isteme. Önceki ajan önerisini kullanıcı kararı gibi alma; eski günlükler kronolojidir, güncel karar tablosu geçerlidir.
3. Claude Code bu arada çalışmışsa `d359ab7` sonrasındaki ilgili değişiklikleri ve `stages/06.3-borc-alacak/README.md`yi oku. Tür/eylem/bayrak sözleşmelerini yeniden doğrula; diğer çalışmayı yeniden tasarlama.
4. Kabul edilen özel tarih kuralını koru (§5). Dönem içindeki açık kayıt vade gününde görünür ve ilgili toplamda bir kez sayılır. Aralıktan önce gecikmiş açık ödeme ayrı satırdan açılır ve dönem toplamına girmez.
5. Mevcut vade ajandasını aç. Örnek gün 8 Aralık 2026, başlangıç görünümü Yoğun / Hepsi / Önümüzdeki 30 gün. Filtre paneli ve takvim çalışır. Kullanıcının görsel incelemesini buradan sürdür.
6. Sonra plan ayrıntısı, bir dönemin ödemesi, fiyat değişikliği ve abonelik/deneme akışları. Özet/İşlemler/Vergiler bağlantısı ardından gelir.
7. Uygulama öncesi API/model ihtiyaçları ve borç-alacak kesişimi ayrı listelenir. Codex'in `design/` sınırı hâlen geçerli; süreç yönetimi yetkisi kendiliğinden kod/ADR yazma izni değildir. Branch ayrımı gerekiyorsa kullanıcıdan alınmış süreç yetkisi kapsamında, mevcut değişiklikler korunarak karar verilir.

### Uygulama öncesi doğrulama kuyruğu

- Geç taksit, kesim günü, kısmi ödeme, kart alacağı ve yeni döneme devir testleri; taksit satırının gerçekleşmede çıkması ve ekstreye doğru tarihte katılması.
- Fiyatın seçilen dönemden itibaren değişmesi, özel dönem tutarı ve eski bekleyenlerin korunması.
- Aboneliği sona erdirme/duraklatma/sürdürme; geçmiş bekleyenler ve bildirimlerin doğru yaşam döngüsü.
- Kapsam ve tür filtreleri ile toplamların uyumu; kart/hesap/kaynağı belirsiz ayrımı; bilinmeyen tutar sayaçları.
- Bilinmeyen türde eksik veri uyarısı; Özet ve İşlemler'de boş/hata ayrımı ve değişiklik sonrası yenileme.

### Dosya ve teslim stratejisi

- **Tek giriş kapısı bu dosya.** Güncel konum en üstte, ayrıntılı kanıt/kararlar ilgili bölümde, yapılanlar günlükte; ertelenen fikirler §7'de.
- Taslaklar `design/planlananlar/` altında; proje içi görsel çıktılar kullanıcının genel kuralına göre `tasarim-onizleme/<konu>-<YYYY-AA-GG>/` altında. 18 Kasım ilk taslağı etkileşimli HTML olarak hazırlandı.
- Plan/ilerleme için şu anda ayrı dosya gerekmiyor. Uygulama tesliminde ayrılırsa bu README bağlantıları ve son konumu tutacak; `stages/06.3-planlananlar/` bu oturumda oluşturulmadı.
- Kullanıcı belgeleri okumak zorunda değil. Claude Code bu belgeyle ilerlemeyi anlayabilmeli; onay bekleyen konular yalnız burada bırakılmaz, sohbette açıkça söylenir.

### 18 Kasım — Kart Hareketleri ile ortak görünüm (önceki taslak)

Güncel taslak [Planlananlar](ilk-taslak.html). Liste vade tarihine göre artan sırada, gün başlıklarıyla akar. Tahsilat ve kesilmiş ekstre aynı güne denk gelirse aynı başlık altında kendi adı ve işlem türüyle görünür. Tür seçimi filtre panelindedir. Önceki dönemde gecikmiş açık kayıtların açılır satırı korunur. İşletme/Şahsi seçiminde karma ekstre kalan borcun tamamını gösterir. Kart taksidi ve tahsilat ödeme toplamına katılmaz. Tutarı bilinmeyen kayıt ilgili özette ayrıca belirtilir.

Kart Hareketleri sayfasındaki AppPageHeader, AppDayHeading, ActivityTile, özet kartı, filtre rozeti, etkin filtre etiketleri, ortak filtre paneli ve özel aralık takvimi Flutter tesliminin dayanağıdır. Taslak, Kart Hareketleri tesliminin renk, boşluk ve köşe değişkenlerini kullanır. Bir bileşen geliştirmesi gerektiğinde diğer kullanan sayfalarla birlikte değerlendirilir. HTML örneğindeki tür menüsü ve özet metinleri uygulama sözleşmesi değildir.

Tasarım başvuruları: [Apple liste rehberi](https://developer.apple.com/design/human-interface-guidelines/lists-and-tables), [Material 3 tarih seçimi](https://m3.material.io/components/date-pickers/), [Actual Budget planlanan işlem görünümleri](https://actualbudget.org/docs/schedules/). Figürler ve tutarlar sentetiktir. Proje içi görseller: [liste](../../tasarim-onizleme/planlananlar-2026-11-18/03-tarih-listesi.png), [filtre](../../tasarim-onizleme/planlananlar-2026-11-18/04-filtre-paneli.png), [takvim](../../tasarim-onizleme/planlananlar-2026-11-18/05-takvim.png).

### 18 Kasım — Planlananlar için vade ajandası (güncel taslak)

Kullanıcının düzeltmesiyle Planlananlar satırı kendi amacına göre tasarlandı. Başlık altındaki tek satırda dönem ve seçilen tür görünür. Dönem toplamları başlıktaki Toplamlar düğmesiyle açılır. Önceki dönemden gecikmiş açık ödemeler katlanabilir satırda kalır. Vade günü soldadır; aynı günün planları tek yüzeyde tür, kapsam veya kart, ad, beklenen eylem ve tutarla sıralanır. Uygulamada ActivityTile kullanmak bu ekran için gerekmiyor. Ortak tarih seçimi, filtre paneli ve ölçü kuralları geçerli. Toplamların açılır panelde olması ve bu satır düzeni tasarım önerisidir. [Güncel önizleme](../../tasarim-onizleme/planlananlar-2026-11-18/06-vade-ajandasi.png).

### 18 Kasım — kapanış ve karar sınırı

- Bu turda D1–D6 ve özel tarih aralığı kararı korundu. Kullanıcının son yönlendirmeleri doğrultusunda kayıtlar vade sırasıyla ortak akışta gösterilir, tür satırda anlaşılır ve tür filtresi panelde uygulanır. Kart ekstresinin ayrı bölümde gösterilmesine ilişkin eski yerleşim önerisi yerini vade günündeki tek satıra bıraktı.
- İncelenecek öneriler: soldaki gün sütunu, aynı güne ait satırların birlikte sunumu, tür ve beklenen eylem metinleri, etkin seçimin başlık altında okunması ve toplamların düğmeyle açılması. Sonuncusu toplamları görmek için bir dokunuş gerektirir. Bunların hiçbiri son kullanıcı onayı almış sayılmaz. Bir öneri hesaplamayı, kaydın görünürlüğünü veya kullanıcıdan istenen eylemi değiştirirse etkisini ayrıca anlatıp karar al.
- Devam sırası: ajanda geri bildirimi → plan ayrıntısı ve dönem/fiyat değişikliği → abonelik/deneme → Özet, İşlemler ve Vergiler bağlantıları → uygulama teslimi. Deneme bildirim zamanı ve teyit akışı açık kalır.
- Son ajandanın yoğun görünümü yerel Chrome’da görsel olarak incelendi. Flutter/cihaz veya gerçek API doğrulaması yapılmadı. Son konum borc-alacak-cari dalı, HEAD d359ab7. Tasarım dosyaları çalışma ağacında; commit yok.

### 19 Kasım — liste eylemi ve ödeme paneli, görüşme taslağı

[Görüşme panosu](../../tasarim-onizleme/planlananlar-2026-11-19/01-vade-ve-odeme.png). Kullanıcı, ödeme yapılabildiğinin listede anlaşılmasını, simge ve logoların yerinin ve İşlemler ile ortak ayrıntıların birlikte kararlaştırılmasını istedi. A seçeneğinde kısa yazılı eylem, B seçeneğinde yalnız ayrıntı oku var. Öneri A. Ortak öneriler gün başlığı, hizmet logosu veya tür simgesi, ad ve tutar, altında tür/kapsam veya kart adı, seçili döneme bağlı görünür toplamlar. Tekrarlayan faturanın paneli ödeme tutarı, hesap ve günü birlikte gösteriyor. İşlemler’deki simge kapsülü, tutar ve ayrıntı satırı dili paylaşılabilir. Türün izin verdiği eylem ve kayıt akışı korunur. Bunlar kullanıcı kararı bekleyen statik çizimlerdir. Önceki etkileşimli taslak ayrı duruyor. Sonraki görüşme bu panodaki satır ve panel üzerinden başlar.

### 19 Kasım — panodaki toplam ve ödeme yolu düzeltmesi (görüşme)

Panodaki ₺4.680 ancak Netflix hesaptan ödeniyorsa geçerlidir; çizimde kaynak gösterilmediği için sayı kullanıcı tarafından doğrulanamıyor. Netflix kart kaynaklıysa ödemeler ₺4.480, karta yansıyacak ₺200, beklenen giriş ₺7.500. 18 Kasım yoğun veri kümesi farklıdır: bilinen ödemeler ₺6.590 ve tutarı beklenen bir vergi, karta yansıyacak taksit ₺1.200, giriş ₺7.500. Önceki dönem ₺2.800 ayrıca görünür. Son panonun iki tutarlı başlığı kartlı senaryo için eksiktir.

Görüşmeye sunulan öneri: her satırda ad/tutar, tür/kapsam (ekstrede kart), beklenen etki ve biliniyorsa kaynak. Kategori türün yerini almaz. Açık eylem ilgili kaydın formunu tek dokunuşla açar; sıradan dönem için ortak dönem paneli, ekstre/vergi/kişi vadesi için kendi akışı. Desteklenmeyen eylem sunulmaz. Tam planı yönetme ikincil erişimdir. Bu öneriler ve panel metinleri henüz onaylanmadı; Flutter veya API testi yapılmadı.

### 19 Kasım — Planlananlar’dan ödeme ve dönüş (değerlendirilen tasarım)

[Üç kareli örnek](../../tasarim-onizleme/planlananlar-2026-11-19/02-donem-odemesi.png). Önerilen varsayılan: liste eylemi ilgili özelliğin ödeme formunu doğrudan Planlananlar üzerinde açar. Arkada liste, seçilen dönem ve filtre kalır. Tek kayıt için iki ayrı ödeme formu yapılmaz. Tam planı yönetme/kişi hesabını inceleme ayrı bir gezinme niyetidir; ait olduğu ayrıntı sayfasına gider. Bu davranış kaynak uygulamada tüm türler için hazır değildir. Yerleşim kullanıcı kabulünü bekler.

- Vade gün başlığıdır. Simge veya tanınan hizmet logosu adın solunda; ad ve tutar ilk satırda, tür ve kapsam (ekstrede kart) ikinci satırda, paraya etkisi ve uygun kısa eylem son satırdadır. Satır ve eylem aynı hedefi açar. Yazı büyürse eylem bir alt satıra iner. Açıklama/uyarı, olmayan bir ödeme eylemi gibi gösterilmez.
- Üstte dönemle birlikte ödenecek, beklenen giriş ve karta yansıyacak tutarlar yalnız ilgili kayıt varsa görünür. Önceki dönem gecikmesi dönem kartının hemen altında ayrı açılır satırdır. Tür ve kapsam değişince liste ve özet aynı seçimden yenilenir. Kaynağı belirtilmemiş tutar kesin banka çıkışı diye adlandırılmaz. Tutarı bilinmeyen ilgili grupta sayı olarak belirtilir. Dönem içindeki gecikme bir kez sayılır.
- Hesaptan tekrarlayan gider, gelir ve karttan abonelik aynı dönem formunu paylaşır; başlık/eylem ve kaynak türü etkiye uygun olur. Tutar/kaynak düzenlemesi yalnız bu dönemi etkiler. Gelecek ödeme günü seçilmez. Önce bekleyen dönem varsa mevcut sıra atlama teyidi korunur. Planın fiyatını ileriye dönük değiştirmek ayrı plan yönetimi akışıdır.
- Ekstrede kart ve kesilmiş dönem bellidir. Kart ekranının mevcut ödeme seçimleri ve tutarı düzenlenebilir ödeme formu kullanılır; ekstre kalanı seçili gelir, kullanıcı varsa asgari veya farklı tutara geçebilir. Kısmi ödemede yeni kalan gösterilir. Karma kart tüm kapsamlarda tam kalanla görünür; gelecekteki ekstre tahmin edilmez. Açılışta dönem/kalan değişmişse yeniden okunur, başka döneme sessizce ödeme yazılmaz.
- Vergide aynı vadenin mevcut Ödedim paneli kullanılır. Gecikmiş veya tutarı bilinmeyen verginin tutarı boş açılır, kullanıcı ödeneni ve kaynağı girer. Planlananlar üzerinden yeni, bağımsız vergi ödemesi başlatılmaz.
- Kart taksidinde vade gelmeden gerçekleştirme eylemi yoktur. İzin verildiği gün kart ve tutar gösterilip onay alınır; kaydın tarihi taksit vadesidir. Borç, alacak, kişi/yükümlülük türlerinde hedef ve desteklenen eylem doğrulanır. Aynı vadenin mevcut akışı tam sayfa gerektiriyorsa o vadeye doğrudan gidilir ve geri dönüş Planlananlar’adır. Yazma yolu yoksa bilgilendirici satır kalır. Deneme bitişi hatırlatmadır, toplama girmez; niyet ve sonuç teyidi önceki karar sınırındadır.
- Başarıda ortak veri yenilenir: tam ödeme kapanır, kısmi ödemede kalan görünür. Kullanıcı vazgeçerse aynı konuma döner. Yazma hatasında girişler korunur; yazma başarılı ama liste yenilemesi başarısızsa aynı ödeme tekrar gönderilmez, yalnız liste yenilenir. Boş, eksik, yükleme hatası ve oturum kaybı sıfır toplam olarak gösterilmez.
- Kaynak kontrolü: planned_activity_page.dart (_act), finance_page.dart (_choosePayment, _addPayment), tax_sheets.dart (showTaxPaySheet), realize_day_sheet.dart. Panodaki üç ekran sentetik ve statiktir, Flutter/API senaryosu çalıştırılmadı.

### 19 Kasım — yoğun ana ekran ve kapsam karşılaştırması

Kullanıcı kararı: Eylemi olan satırın tamamı dokunulabilir. Ad, tutar veya kısa eyleme dokununca aynı hedef açılır. Tek dokunuş mali kaydı onaylamaz. Eylemi henüz yapılamayan taksitte ödeme eylemi sunulmaz.

[Yoğun ekran — üç kapsam](../../tasarim-onizleme/planlananlar-2026-11-19/03-yogun-kapsam.png). Örnek bugün 12 Aralık, seçili dönem 8–21 Aralık. Ekranlar kaydırılabilir içeriğin tamamını gösteren statik örnektir. Dönem toplamları dikey hizalı; bilinmeyen vergi ödeme toplamının hemen altında; önceki dönem gecikmesi ayrı açılır satırda. İşletme örneğinde gecikmeler açık gösterildi. Tür ve tutara göre simge, kayıt adı, kapsam veya kart, etki ve kısa eylem birlikte okunur. Yerleşim onay bekliyor.

| Kapsam | Bilinen ödemeler | Beklenen giriş | Karta yansıyacak | Önceki dönem |
|---|---:|---:|---:|---:|
| Hepsi | ₺4.830 ve 1 belirsiz vergi | ₺7.500 | ₺1.400 | ₺2.800 |
| İşletme | ₺4.480 ve 1 belirsiz vergi | ₺7.500 | ₺1.200 | ₺2.000 |
| Şahsi | ₺4.350 | kayıt yok | ₺200 | ₺800 |

Sentetik satırlardan üç kapsamın sayıları hesaplandı ve beklenen değerlerle karşılaştırıldı. 9 Aralık vadesi gecikmiş ama dönem içindedir; ₺480 bir kez sayılır. Karma ekstre ₺4.000 üç görünümde de tamdır. Flutter/cihaz ve API testi yapılmadı. Sırada bu yerleşimin görüşülmesi, ardından plan bağlantılı dönem paneli, kısmi ekstre ve tutarı belirsiz vergi akışları var.

### 19 Kasım — dönem seçimi ve ödeme panelleri

Kullanıcı yönlendirmesi uygulandı: dönem özetinin ilk satırında tarih solda, genel kapsam sağda; tüm türler seçiliyken fazladan satır yok. Tür filtresi etkinse seçilen tür ortak kaldırılabilir filtre etiketiyle görünür. [Güncel üç kapsam](../../tasarim-onizleme/planlananlar-2026-11-19/04-donem-ve-kapsam.png). [Plan bağlantısı, kısmi ekstre ve vergi örnekleri](../../tasarim-onizleme/planlananlar-2026-11-19/05-vadeden-odeme.png).

Dönem seçimi için senaryo açıklaması: ana liste seçim başlangıç ve bitiş günleri dahil aralıkta vadesi olan, bugün açık kalemleri gösterir. Gecikme bugüne göre belirlenir. Vadesi hem başlangıçtan hem bugünden önce olan açık ödemeler katlanabilir satırdadır. İleri döneme bakarken bugünden önce vadesi geçmemiş bir ödeme yalnız seçimin önünde kaldığı için gecikmiş sayılamaz. Geçmiş döneme bakarken seçilen bitişten sonra geciken vadeler o ekrana taşınmaz. Ödenmiş dönemler plan geçmişinden incelenir. Dönem değişince liste, gecikenler ve toplamlar aynı kapsam/tür seçimiyle yenilenir. Seçili dönemde kayıt yokken önceki gecikme varsa ikisi ayrı gösterilir. Bugün 12 Aralık varsayımıyla 9 Aralık açık ödeme için 8–21 Aralık ve 1–31 Aralık: dönem içinde gecikmiş; 12–18 veya 15–21 Aralık: önceki gecikmeler; 1–7 Aralık: görünmez. 12 Aralık vadeli ödeme bugün gecikmiş sayılmaz. Bugünkü uygulamanın 7/30/90 gün ufku özel takvim aralığı değildir. Bugün dahil 7 günün bugün+6 ile kapanması tasarım önerisidir; backend üst sınırı buna göre ele alınmalı.

Panel çizimleri tek vadeyi örnekler: plan bağlantısı başlık altında, 4.000 TL ekstreye 1.500 TL kısmi ödeme ve tutarı bilinmeyen vergi. Kart örneği düzenlenmiş tutarla son ödeme adımıdır; mevcut ödeme seçimi/asgari tutar yolu korunur. Statik tasarım; cihaz ve API testi yapılmadı. Sonraki iş tam sayfa plan ayrıntısında tek dönem değişikliği, ileri tarihli fiyat değişikliği ve sona erdirmenin ayrılması.

### 19 Kasım — dönem adlandırması ve ödeme ekranlarının sahibi

Öneri: özel tarih aralığında “Önceki dönemden geciken” yerine seçimin başlangıcını kullan (ör. “15 Aralık’tan önce gecikenler”). Bu bölüm daha eski birden çok dönemden açık ödeme içerebilir. Vade başlangıç ve bitiş günleri dahil seçimdeyse açık kalem ana listededir. Önceki gecikmelere girmek için vade hem seçim başlangıcından hem bugünden önce olmalıdır. Bu bölüm tek başına varsa dönem içi boş durum korunur. Geçmiş seçim bugünkü açık vadeleri gösterir; geçmiş bir tarihin borç fotoğrafı veya ödeme geçmişi değildir. Backend bugün bugüne göre vade sınıflaması ve ileri ufuk kullanır; seçilen aralığın başını referans gününe çevirmek özellikle kesilmiş ekstreyi değiştirir ve önerilmez.

05 numaralı pano ödeme bağlamının senaryo çizimidir. Kart ve vergi ödeme formları kendi özellikleriyle ortak kullanılacak; baştan ikinci bir ödeme arayüzü çizmek veya panodaki alanları mevcut formların yerine geçirmek önerilmedi. Kaynak ayrıntısına erişim sunulursa tüm ortak form girişlerinde aynı ikincil bağlantı önerilir. Ekstre için aynı kartın aynı kesilmiş dönemi, vergi için ilgili vergi planı hedeflenir. Seçilen kaydı tekrar aratan genel liste bağlantısı önerilmez. Düzenlenmiş ödeme formundan incelemeye gidilip dönülürse alanlar korunmalı, güncel bakiye doğrulanmalıdır. Bağlantıların kabulü açık. Tekrarlayan planın ayrıntısı ve yönetimi kullanıcının isteğiyle Tekrarlayanlar çalışmasına bırakıldı; sonraki iş olarak bu ekran çizilmeyecek.

### 19 Kasım — kabul edilen dönem ve ayrıntıya erişim; türlere uygulama

**Karar:** Seçim başlangıcı ve sonu dahil, vadesi seçimdeki açık kayıtlar ana listededir. Gecikme bugüne göre belirlenir. Hem seçim başlangıcından hem bugünden önce vadesi geçmiş açık ödemeler “15 Aralık’tan önce gecikenler” gibi tarihli ayrı satırdan açılır; dönem toplamına girmez. Bugün vadeli kalem gün boyunca “Bugün”dür. Geçmiş seçimi bugünkü açık vadeleri gösterir; ödeme geçmişi kendi yerinde kalır. Kapsam ve tür seçimi korunur. Bu, tasarım kararıdır; uygulamanın bugünkü 7/30/90 gün sözleşmesine özel aralık eklenmedi.

**Kabul edilen gezinme:** Görünür eylem ve satırın tamamı aynı hedefi açar. Ödeme akışı ilgili özelliğin formunu kullanır. Seçilen kaydı inceleme, formda başlığın altında ikincil erişimdir. İncelemeden dönüldüğünde taslak tutar, gün ve kaynak korunur; kayıt başka yerde kapanmış veya kalan değişmişse güncel bilgi gösterilir, eski tutarla sessizce kayıt yapılmaz. Genel listeye bırakıp tekrar aratma. Aşağıdaki uygulama ayrıntıları tasarım önerisidir.

| Senaryo | Satırın ilk hedefi | İnceleme hedefi ve sınır |
|---|---|---|
| Hesaptan tekrarlayan fatura/gider; tekrarlayan gelir | Seçilen dönemin ödeme/gelir paneli. Erken ödeme ve önceki bekleyen dönem teyidi korunur. | **Planı görüntüle** → aynı tekrarlayan plan. Henüz dönem satırı üretilmemişse plan kimliği + vade ile açılır; yalnız inceleme için dönem üretilmez. |
| Kart kaynaklı düzenli harcama veya ücretli abonelik | Seçilen dönemin kart harcamasını kaydetme paneli | **Planı görüntüle** → aynı plan/abonelik. Gerçekleşmemiş harcama için ekstre ayrıntısı önerilmez. |
| Deneme bitişi | İlgili aboneliğin deneme durumu. Ödeme formu açılmaz. | Aboneliğin kendisi zaten açıksa ikinci bir görüntüleme bağlantısı gerekmez. Devam niyeti, iptal teyidi ve gerçekleşen ücret ayrıdır. Hatırlatma zamanı ve son teyit ekranı açık kalır. |
| Kart taksidi, ör. monitör 2/4 | Vade gelmeden salt okunur **Taksit planını görüntüle**; vade geldiğinde yapılabiliyorsa karta yansıtma onayı. | Kartlar’daki mevcut taksit planı ayrıntısı, ilgili sıra belirgin. Onaydan **Taksit planını görüntüle** ile erişilebilir. Bilgiye dokunmak harcama oluşturmaz. |
| Kesilmiş ekstre, asgari/kısmi/tam ödeme | Seçili kart ve dönemin kart ödeme formu | **Ekstreyi görüntüle** → aynı kartın ilgili kesilmiş dönemi. Yeni ekstre kesildiyse başka dönemle sessiz değişim yapılmaz. Kısmi ödemede kalan, tam ödemede satırın kapanması listeye yansır. |
| Vergi, bilinen/belirsiz/gecikmiş tutar | Seçilen vergi vadesinin mevcut Ödedim paneli | **Vergi planını görüntüle** → ilgili plan. Aynı türde iki plan varsa plan kimliği belirleyicidir. Gecikmiş vergide kullanıcının ödeyeceği tutar varsayılmaz. |
| Borç taksidi / alacak taksidi | İlgili borç/alacak ve taksit sırasıyla kendi ödeme/tahsilat akışı | **Borcu görüntüle / Alacağı görüntüle** → aynı borcun taksitleri, ilgili sıra belirgin. Ödeme hesabı ve tahsilat hesabı yönü korunur. |
| Tekil ödenecek/tahsil edilecek yükümlülük | Yazma desteklenene kadar açık tutar ve engel gösterilir. | Seçili yükümlülüğün doğrulanmış ayrıntı hedefi varsa ona git. Genel liste bağlantısı ve çalışmayan ödeme eylemi gösterilmez. Yeni borç-alacak çalışmasının sözleşmesiyle yeniden kontrol edilir. |
| Kişi vadesi: aynı kişi/gün/yönde birden fazla kaynak | Desteklendiğinde o kişi, vade ve yöndeki ortak ödeme/tahsilat akışı | **Kişi hesabını görüntüle** → aynı kişi, mümkünse ilgili gün ve yön belirgin. Satır birkaç kaydı toplar; tek bir faturayı seçip onun borcu gibi gösterme. Bayrak ve istemci desteği doğrulanmadan açılmaz. |
| Gecikmiş satır, pasif kaynak, yetersiz limit, kapanmış veya çözülemeyen hedef | Gecikme tek başına hedefi değiştirmez. Ödeme engeli varsa neden görünür; mevcut salt okunur inceleme korunur. | Eylem hedefi ile inceleme hedefi ayrı doğrulanır. Eksik hedef için başka kayda yönlendirme yapılmaz. Yenilemede kapanan satır güncellenir; yazma tekrarlanmaz. |

**Kaynak kontrolü:** Flutter PlannedActivity modelinde yedi tür, plan kimliği, eylem hedefi ve gerektiğinde sıra var. Kartlar’da showInstallmentPlanDetail ve ekstre/ödeme akışı, Vergiler’de TaxPlanDetailPage ve showTaxPaySheet, borçlarda debts_page.dart içinde taksite özel ödeme/tahsilat var. EfPlannedActivityRepository.ListCounterpartyDuesAsync kişi vadelerini kişi + yön + gün ile toplar; hedef kimliği kişidir. Bu isteğe bağlı sunucu türleri mevcut Flutter PlannedKind içinde yoktur. Borç-alacak koordinasyonu ve deneme/abonelik desteği açık kalır. Normal plan ayrıntısının iç düzeni Tekrarlayanlar turunda çalışılır.

**Sonraki Planlananlar işi:** tür/tarih filtresiyle sonuç yok, dönemde açık kayıt yok ama önceki gecikme var, eksik veri ve yenileme hatası görünümlerini aynı ekran üzerinde ayırt etmek.

### 19 Kasım — boş durumlar mevcut ortak bileşenlerle

[Görsel karşılaştırma](../../tasarim-onizleme/planlananlar-2026-11-19/06-bos-durumlar.png). Kullanıcının paylaştığı filtre gövdesi ilk karede kullanıldı. Diğer kareler yerleşim taslağıdır. Flutter teslimi ortak widget’ın ölçü, semantik, renk, ikon ve eylemlerini alır. Kaynak: documentation/design-system.md “Sonuç yok türü” ve “Bölüm boş türü”, app_no_results_view.dart, app_section_empty.dart, app_state_views.dart, planned_activity_page.dart ve planned_activity_controller.dart.

| Doğrulanmış durum | Planlananlar davranışı |
|---|---|
| Okuma tamamlandı; seçili türle eşleşen görünür kayıt yok | Mevcut AppNoResultsView.filter görünümü: “Bu filtreye uyan planlanan işlem yok”, filterSentence ile ör. “Filtre: Ekstre”, “Filtreyi temizle”. Tür seçimini kaldırır; genel kapsam ve seçili dönem korunur. Tür seçiliyse hem ana hem önceki gecikme bölümü bu türü dikkate alır. Örneğin eski kira, ekstre filtresinde gösterilmez. |
| Tür filtresi yok, dönemde ve gecikmelerde gösterilecek kayıt yok | AppNoResultsView, period sebebi. Mevcut 7→30→90 gün düğmesi sonraki ufku doğrudan açar; 90 günde cümle ve düğme yoktur. Yeni özel aralık için öneri: “Bu dönemde bekleyen kayıt yok” ve “Dönem seç” doğrudan ortak tarih seçiciyi mevcut seçimle açar. Tarih ve genel kapsam görünür kalır. |
| Dönemde eşleşen açık kayıt yok, önceki gecikmelerde eşleşen ödeme var | Tarihli gecikme satırı kullanılabilir kalır. Ana dönem alanında AppSectionEmpty: “Bu dönemde bekleyen kayıt yok.” Boş gruplar için özet metrik çizilmez. |
| Henüz ilk yanıt alınmadı veya başarısız | AppLoadingView; hata halinde AppErrorView + Tekrar dene; oturum geçersizse AppUnauthorizedView. Para toplamı ve ödeme yok iddiası gösterilmez. |
| Yenileme başarısız / yanıt eksik | Mevcut controller eski page değerini koruyabiliyor, sayfa hata görünümünü yalnız page boşken açıyor. Öneri: aynı seçimden kalan kayıt varsa ortak satır içi uyarıyla güncellenemediğini bildir ve yeniden okuma eylemi sun. Farklı seçimden eski listeyi yeni başlıkla gösterme. Eksik yanıtı veya bilinmeyen türü boş/eksiksiz sonuç olarak gösterme. |
| Tutarı bilinmeyen kayıt | Liste doludur. Kayıt ve ilgili toplamın belirsiz tutar bilgisi görünür. |
| İlk kullanım | Tek bir aralıkta sıfır kayıt olması ilk kullanım kanıtı değildir. Planlananlar’ın mevcut dönemli okumasından ilk kullanım çizimi veya “ilk planını ekle” eylemi türetilmez. |

06 numaralı pano bugünkü Flutter ekranından yalnız filtre gövdesini örnekler; özel aralık ve önceki gecikmeler tasarım kararıdır. Cihaz testi yapılmadı. Sonraki adım ortak filtre panelinde dönem/tür seçimini, seçimden vazgeçmeyi ve uygulama sonrası liste–özet eşleşmesini doğrulamak.

### 19 Kasım — ortak dönem görünümü ve filtre paneli

Döneme göre sonuç yok görünümü AppNoResultsView.period ile zaten ortak. “Bir şeyler ters gitti” için de çalışan AppErrorView var; görsel yenilemesi kullanıcının yönlendirmesiyle ileride ortak çalışma olarak ele alınacak ve ilgili ekranlara birlikte uygulanacak. 06 panosunun hata karesi yeni ortak tasarım onayı değildir.

Kart Hareketleri’ndeki AppFilterSheet kullanımı ile ilk-taslak.html içindeki etkileşim karşılaştırıldı. Açık panelde tür/dönem değişiklikleri taslaktır. Takvimde Vazgeç önceki panel seçimini korur, Seç yeni aralığı panele taşır. Sıfırla paneli açık tutarak 30 gün / tüm türler varsayılanına döner. Varsayılanda pasiftir. Paneli kapatmak değişiklikleri bırakır. Sonuçları göster listeyi, önceki gecikmeleri ve dönem toplamlarını aynı seçimle günceller; genel kapsam ayrı kalır. Sonuç yok görünümündeki Filtreyi temizle ise türü doğrudan kaldırır ve dönemi korur.

Tasarım prototipindeki hemen uygulanan Sıfırla ve gün ufkundaki fazladan gün düzeltildi. Bugün dahil 7 gün örneği 8–14 Aralık, 30 gün 8 Aralık–6 Ocak. JS sözdizimi ve temsili DOM üzerinde filtre/tutar, sıfırlama/iptal, takvim vazgeç/seç, tek günlük aralık ve yıl geçişi kontrol edildi. Flutter ve cihaz testi yapılmadı. HTML önceki görsel taslaktır; son yerleşim 04–06 panolarında. Sonraki tasarım işi Tekrarlayanlar listesi ve buradan ya da Planlananlar’dan açılacak ortak plan ayrıntısıdır.

### 19 Kasım — Tekrarlayanlar listesi ve ortak plan ayrıntısı

[Üç ekranlı kira örneği](../../tasarim-onizleme/planlananlar-2026-11-19/07-tekrarlayan-kira.png). Gün 12 Aralık 2026. Mevcut Tekrarlayanlar sekmesi, ekleme eylemi, boş liste için AppFirstUseView ve duraklatılan/biten planların katlanabilir bölümü dayanak alındı. Güncel Flutter satırı yönetim menüsü açıyor. Önerilen ilk hedef tam sayfa plan ayrıntısı. Üç kare birer tasarım önerisidir.

- Bir plan listede bir kez görünür. Tutar plan tutarıdır. Altında varsa en eski açık gecikmiş vade, yoksa sıradaki vade gösterilir. Gecikme yazısı oluşturma gününden veya son ödenen tarihten tahmin edilmez; açık vadeye dayanır.
- Tekrarlayanlar satırı ve Planlananlar ödeme panelindeki “Planı görüntüle” aynı plan kimliğine gider. İkinci yolda seçilen dönem bağlamı korunur. Geri dönüşte yarım girilen form değerleri korunur, güncel vade yeniden doğrulanır. Plan ayrıntısında zaten açık bir ödeme taslağına ait aynı vadeye yeniden dokunulursa o taslağa dönülür; iç içe ikinci ödeme başlatılmaz.
- Bekleyen vade satırı tek dönemin ortak ödeme formunu açar (erken ödeme ve sıra atlama kuralı geçerli). Aralık kirasını 12.750 TL ödemek yalnız Aralık hareketini oluşturur. Kasım gerçekleşmesinde varsa üretilmiş hareket kimliğiyle mevcut ayrıntı açılır; planın tüm geçmişine bir ödeme eylemi uygulanmaz.
- “Plan tutarını değiştir” plan seviyesindedir. Eşik serbest takvim günü yerine bu planın vadesinden seçilir. Önizlemede Aralık açık 12.500 TL, Ocak ve Şubat 15.000 TL. Ödenmiş Kasım geçmiş olarak kalır. Plan bitişi/tekrar sınırı sonrasına dönem uydurulmaz. Önceden tek döneme özel yazılmış tutar için önizleme uygulanacak son tutarı göstermeli; FollowPlan tutar eşitliği bugün kullanıcı niyetini kesin tanımlamaz. Tarih etkili fiyat şu an uygulanmış değildir.
- Üst menüde plan yönetimi yer alabilir. Sona erdirme ileri vadeleri keserken geçmiş bekleyeni korur (D2); duraklat/sürdür aralığının anlamı ve son vade seçiminin metni ayrıca incelenecek. Bugünkü pasif bayrağını sona erdirme gibi sunma. Fiyat değişikliğini altta ana eylem yapmanın yerleşimi henüz onaylanmadı.
- Kaynaklar: planning_page.dart içindeki _recurringTab ve _RecurringPlanCard, RecurringTransaction.Reschedule/Deactivate ve RecurringTransactionOccurrence.FollowPlan. Statik pano görsel olarak kontrol edildi. Uygulama kodu veya cihaz testi yok. Sonraki konu aynı örnekte son vade ve sona erdirme; tekrarlayan oluşturma ve abonelik/deneme sonraki akışlardır.

### 19 Kasım — bağımsız Raporlar ve yoğun Tekrarlayanlar listesi

Kullanıcı kararı: Raporlar bağımsız tek bir hedef olur. Öneri: Daha Fazla → Raporlar. Tam vade sayfası Planlananlar adını taşır. Özet kartı Yaklaşanlar adını koruyabilir; bugün de plannedLocation açıyor. Eski Yaklaşanlar sekmesinin işlevi aynı Planlananlar’a taşınır. Özet ve İşlemler kendi dönem/kapsamıyla aynı ajandaya bağlanır. Tekrarlayanlar plan yönetimidir; rapor okumalarıyla açılmamalı.

[Güncel liste: 12 aktif kayıt](../../tasarim-onizleme/planlananlar-2026-11-19/09-yogun-tekrarlayanlar.png). [Plan ayrıntısı, düzenleme ve sona erdirme](../../tasarim-onizleme/planlananlar-2026-11-19/08-plan-yonetimi.png). 08’in liste karesini 09 değiştirir. Yerleşimler inceleme bekliyor.

- Tekrarlayanlar → Tümü / Planlar / Abonelikler. Tümü bir planı bir kez gösterir. Planlar abonelik dışı planları içerir. Ekle, seçime göre normal formu veya hizmet seçimini açar; Tümü’nde kısa tür seçimi sunulur. Hizmet biliniyorsa logo, özel abonelikte genel simge. Denemede yenilenme yerine deneme bitişi gösterilir.
- Başlıkta Ekle ve arama; ortak yüzeyde ayraçlı satırlar. Ad ve tutar üstte, sıklık ve ilgili vade altta. Önce en eski açık gecikme sonra sıradaki vade; eşitlikte ad. Uzun ad sarar; yazı ve dokunma hedefi küçültülmez. Gerçek tutar biçimi ortak MoneyText. Arama ad/hizmette çalışır. Dönüşte seçim ve kaydırma korunur. Duraklatılan ve bitenler katlanır. Kaydırılabilir örnekten gerçek cihaz için sabit satır sayısı çıkarılmaz.
- Ortak ayrıntıda en eski gecikmiş + varsa diğer gecikenlere erişim; sıradaki tek vade; geçmişte son üç gerçekleşme. Ödeme takviminde ay seçilebilir, yalnız o aralık üretilir. Süresiz planın bitişi varsayılmaz. Şubatta başlayan ve ilk dönemi ödenmiş aylık planda Mart öne çıkar. Eski planda geçmiş tarih sırasıyla yeniden eskiye parça parça açılır. Dışarıdan seçili vadeyle gelindiyse o vade görünür. Biten plandaki açık vade Planlananlar’da kalır.
- Planı düzenle: ad, kategori, tutar, hesap/kart, sıklık, ödeme günü, bitiş. İlgili ritimlerde ay sonu, seçili aylar ve tekrar sınırı açılır. Gelirde yalnız hesap. Finansal değişikliğin ilk etkilenen vadesi ve yeni tutar/kaynak gösterilir. Kaynak hesaptan karta geçerse etkilenen vadeler ajandada karta yansıyacak tutara geçer. Tarih/sıklık değişiminde eski/yeni takvim önizlenir. Sadece adı değişiyorsa ilk vade sorulmaz. Tek dönem istisnası ödeme panelindedir.
- Üç nokta → Duraklat / Planı sona erdir; duraklatılmışsa Sürdür. Sona erdirmede son dahil vade ve oluşmayacak ilk vade gösterilir. Açık geçmiş korunur. Aboneliği uygulamada sona erdirmek sağlayıcıda iptal oluşturmaz. Duraklatılan aralık ve mevcut silme davranışı ayrıca netleşmeli.
- Kaynaklar: dashboard_page.dart, more_page.dart, planning_page.dart, UpdateRecurringTransactionUseCase. Bugün takvim değişiminde bekleyenler yeniden kuruluyor; diğer değişiklikler FollowPlan kullanıyor. Seçilen vadeden itibaren kaynak/tutar/takvim değişimi ve eski açıkların korunması, sınırlı takvim okuması ve çok yıllık geçmişi sayfalama uygulama tesliminde doğrulanacak. Panolar statik; cihaz veya Flutter testi yapılmadı.

### 19 Kasım — ilk kullanım, arama ve görünür yönetim eylemleri (öneri)

[Güncel üç ekran](../../tasarim-onizleme/planlananlar-2026-11-19/10-tekrarlayan-plan-yonetimi.png). Kullanıcının iki gerçek ekranı ve kaynak kodu incelendi: planning_page.dart ilk kullanımda AppFirstUseView, activity_feed_page.dart arama alanı, Hesaplar/Kartlar/POS başlıklarında açıklamalı IconButton +. Şu ayrıntılar önceki 08/09 yerleşimini günceller:

- İlk kullanımda mevcut ortak çizim ve ortadaki yazılı ekleme eylemi kalır; boş arama ve yinelenen ekleme eylemi gösterilmez. Ortadaki giriş plan veya abonelik oluşturmayı açarsa önerilen etiketi “Plan veya abonelik ekle”. Dolu listede başlığın sağında erişilebilir adı olan tek +, altında görünür “Plan veya abonelik ara” alanı. Aranabilir plan mevcut ama etkin seçim boşsa arama/filtreler korunur ve uygun sonuç-yok durumu kullanılır.
- Tümü / Planlar / Abonelikler tercihi: farklı türleri birlikte yenilenme gününe göre taramak ve adıyla bulmak için Tümü başlangıçtır; diğer iki seçim ilgisiz türleri gizler. Tek toplam listeye abonelik iki kez eklenmez. Planlar sıradan düzenli planları gösterir. Arama etkin seçim içinde çalışır. Ekle seçili türün oluşturma yolunu, Tümü'nde kısa tür seçimini açar.
- Liste plan başına tek satır: anlamlı simge veya bilinen hizmet logosu, ad, her dönemin tutarı, tekrar kuralı ve en önemli tarih. Öncelik gecikmiş açık vade, sonra sıradaki vade veya deneme bitişidir. Ad/gün büyük yazıda sarılır, tutar ve durum tek başına renkle anlatılmaz. Hesap/kart, ayrıntılı takvim, geçmiş ve yönetim tam sayfa ayrıntıdadır. Kaydırma ve arama durumu dönüşte korunur.
- Üst sağ Düzenle; alt bölümde metinli Planı duraklat / Planı sona erdir (duraklatılmışta Sürdür). Bunlar ayrı işlemlerdir; çöp simgesi veya üç noktayla gizleme önerisi güncellendi. Sonlandırma ayrı sonuç önizlemesi ve onay ister. Var olan gerçek silme davranışı ayrı ürün kararı gerektirir; bu pano onu sona erdirmeyle eşitlemez.
- Ad ve tutara doğrudan yazılır; kaynak ortak seçim menüsünü açar; kategori, sıklık, gün ve bitiş satırlarının bütününe dokununca ilgili seçim açılır. Eşik yalnız gelecekteki dönemleri etkileyen değişikliklerde görünür. Vade kuralı değişirse eski/yeni tarihler, kaynak değişirse yeni finansal etki gösterilir. Kaydet tek seferde uygular; kaynak veya ritim değişikliği için bugün olmayan dönem etkili sunucu desteği ayrıca gerekir.
- Ayrıntı için önerilen sınır: en eski gecikmiş vade + diğer gecikmelere sayı ile giriş; bugün vadesi varsa o görünür, yoksa ilk gelecek vade. Aynı vade iki bölümde tekrarlanmaz. Takvim mevcut ayla (belirli vadeden gelindiyse onun ayıyla) açılır ve seçilen aya doğrudan geçilebilir; günlük planda o ayın günleri, yıllık planda doğrudan sıradaki ödeme ayına erişim. Son vade/tekrar sınırından sonra vade üretme. Duraklatma aylarını otomatik atlanmış sayma. Geçmiş önizlemesi varsayılan son iki gerçekleşme; daha eskisi gerçekten varsa Tüm geçmişi görüntüle. Geçmiş gerçekleşme gününe göre yeniden eskiye, farklıysa ödeme vadesi ayrıca görünür; uzun geçmiş parça parça yüklenir. Bir plan seçili bir vadeden açıldıysa öncelikle o vadeye dönmek mümkün olmalı.
- Statik örnek, gerçek cihazda büyük yazı/klavye veya ekran okuyucu testi yapılmadı. Sonraki inceleme: abonelik seçiliyken + ile hizmet seçimi, özel hizmet ve deneme/ücretli başlangıç.

### 19 Kasım — plan satırında bilgi sırası ve tür menüsü, inceleme önerisi

[11 — satır ve seçim](../../tasarim-onizleme/planlananlar-2026-11-19/11-tekrarlayan-satir-ve-secim.png). 10’un yalnız liste karesini günceller. Ücretli ve sıradan aktif kayıtlarda sol üst ad, sağ üst planın belirtilen sıklıktaki tutarıdır (yıllık ücret aylığa bölünmez). Sol alt sıra her zaman yön (Gelir/Gider), sıklık, varsa Abonelik. Kategori veya ödeme aracı bu alanın yerine geçmez. Aylık tüm örneklerde Aylık yazılır. Kesin gün, ay sonu davranışı ve seçili ayların adları ayrıntıdadır. Çalışan sıklıklar Günlük, Haftalık, Aylık, Üç ayda bir, Yıllık, Seçilen aylarda. Sağ altta sıradaki gerçek açık vade; bugün ve gecikme sözcükle belirtilir. Daha eski bekleyen tutar plan tutarından farklıysa satırdaki para plan tarifesidir; ödeme anında gerçek dönem tutarı gösterilir, iki tutar bağlamsız yan yana kullanılmaz. Böyle bir örnekte “Plan tutarı” veya “Bekleyen” etiketiyle ayrıca sınanacak.

Denemede sağ üst Ücretsiz deneme, sağ alt bitiş günü; soldaki “Abonelik · Sonrasında aylık” gelecekteki ücret ritmidir. Deneme sonrası kullanıcının girdiği ücret ayrıntıda görülür. Deneme otomatik gider yaratmaz. Duraklatılmış/bitenlerin katlı bölümünde gelecek vade varmış gibi tarih basılmaz; açık borç varsa gecikme bilgisi korunur. Kaynak, kapsam, ayrıntılı takvim ve hareket geçmişi ayrıntıdadır.

Üç büyük çipin yerine arama alanının sağında mevcut seçimi gösteren tek menü önerildi: Tümü, Abonelikler ve abonelik dışı kayıtlar için Diğer planlar. İlk tasarımda Tümü açık. Seçim tek dokunuş daha ister, ana ekranda bir yatay şerit kazandırır. Seçili tür arama ve + için geçerlidir; geniş yazıda seçim aramanın altına sarabilir. İstemci ortak menü/arama/dokunma kurallarını kullanır. Pano statiktir; kullanıcı onayı ve gerçek cihazda taşma kontrolü beklenir.

### 19 Kasım — satır kuralları kabulü, tür seçimi ve hizmet seçimi

Kullanıcı tür seçiminin görünümü dışındaki son önerileri kabul etti. 11 numaralı panodaki ad, yön/sıklık, abonelik işareti ve ilgili açık vade kullanılır. Önceki açık dönemin tutarı güncel tarifeden farklıysa sağdaki tutar gösterilen vadenindir. Örnek: gecikmiş şubat ₺12.500, mart tarifesi ₺15.000 ise listede ₺12.500 ve şubat gecikmesi, ayrıntıda ₺15.000 plan tutarı. Önceki bölümün “ayrıca sınanacak” notu karar açısından güncellendi. Denemede ücret yerine deneme durumu ve bitiş günü görünür.

[12 — ikonlu tür seçimi, hizmet seçimi ve deneme](../../tasarim-onizleme/planlananlar-2026-11-19/12-tur-menusu-ve-abonelik.png). İki kullanıcı referansı değerlendirildi. Öneri, yoğun telefon listesinde aramanın yanında AppFilterMenu ve onun AppAnimatedMenu açılma/kapanma davranışı. Her türde simge, seçili satırda ortak zemin/sol işaret. Üç tür tek düz listedir, yapay grup başlığı gerekmez. Tam genişlikte sayaçlı üçlü şerit tek dokunuş ve tüm türlerin keşfi avantajına sahip; karar kullanıcı incelemesinde. Tür için ortak simge, katalogdaki hizmet için marka logosu kullanılır; panodaki logolar temsilidir.

Abonelikler seçiliyken + doğrudan aranabilir hizmet seçimine gider; katalogdan seçim ve Kendi hizmetimi ekle aynı abonelik formuna çıkar. Özel hizmette kullanıcı adı girer; eşleşmeyen aramada özel hizmet yolu korunur. Ücretli abonelikte teklif ücreti, sıklık ve ilk/sonraki ödeme tarihi; denemede bitiş, tekliften kullanıcı tarafından girilen ücret ve devam niyeti. İki niyette de hatırlatma. Deneme bitişi ve ilk ücretli vade ayrı anlamdadır; ilk ücretli tarih kendiliğinden aynı gün kabul edilmez. Panodaki Devam et sonraki plan bilgilerine (kaynak/kategori, gerekiyorsa kapsam, gözden geçirme) gider, ücret kaydı üretmez. Bildirimin zamanı ve deneme sonu teyidi açık kalır. Kaynak kodda ortak menü ve ikon desteği kontrol edildi. Pano statik, animasyon/cihaz testi yapılmadı.

### 19 Kasım — abonelik ekleme araştırması ve Türkiye başlangıç kataloğu

[13 — katalog ve ücret koşulları](../../tasarim-onizleme/planlananlar-2026-11-19/13-hizmet-secimi-ve-kosullar.png) · [14 — deneme, hatırlatma ve ilk ücretli vade](../../tasarim-onizleme/planlananlar-2026-11-19/14-deneme-ve-ilk-vade.png). 12 numaralı örneğin sonraki tasarım turudur. Tutar ve tarihler sentetik. Araştırma ürünlerin kendi yardım belgelerinden yapıldı; ücretli uygulamalarda cihaz üstünde rakip testi yapılmadı.

| Birincil kaynak ve gözlem | Bizim ürüne çıkarım |
|---|---|
| [Rocket Money — manuel abonelik](https://help.rocketmoney.com/en/articles/2185531-managing-your-bills-and-subscriptions): + ile isim arama/hizmet veya özel isim; ardından gelecek vade ve tutarı düzeltme | Katalog yalnız tanıma ve bulmayı hızlandırır. Eşleşme yokken yazılan adı “Bu adla ekle” ile taşı; bankadan işlem bulma ürün sınırı dışında. |
| [Google Play — abonelik sunumu](https://support.google.com/googleplay/android-developer/answer/9900533?hl=en): teklif ücretini, faturalama sıklığını ve yenileme koşullarını açık gösterir | Bizde satın alma akışı yok. Ücretli dönem tutarını gerçek ritmiyle göster; yıllık planı aylıklaştırıp ana tutar yapma. Deneme bitişiyle ücretli dönemi görsel olarak ayır. |
| [Apple — abonelik iptali](https://support.apple.com/tr-tr/118428): iptal yeri faturalandıran şirkete bağlı; Apple denemeleri için önceden iptal yönlendirmesi var | Sağlayıcı kurallarını kataloğa genelleme. Hatırlatma günü kullanıcı tarafından değiştirilebilir; “iptal edeceğim” gelecekteki niyet, “hizmette iptal ettim” gerçekleşmiş teyit. |
| [YNAB — planlanan işlemler](https://support.ynab.com/scheduled-transactions-a-guide-BygrAIFA9?mobile-help=true): sıradaki işlem tarihi ve tekrar ritmi ayrı girdiler; gerçekleşme onayı ayrı adım | Yeni abonelik kaydı gider üretmez; ilk ücretli vade ile fiili ödeme ayrı. |
| [Exxen](https://plan.exxen.com/tr) ücretli tanıtım denemesi örneği, [GAİN kampanya koşulları](https://destek.gain.tv/kampanya-yararlanma.html) dönemsel teklifler | “Deneme” sözcüğünden ücretsiz olduğu çıkarılmaz. Ücretli tanıtım döneminde ödenen ilk tutar ve sonraki tarife farklı olabilir; mevcut tek dönem / ileri fiyat değişikliğiyle göster. Bu oturumda yeni promosyon motoru önerilmedi. |

**Katalog ve varlıklar.** [Kaynak manifesti](hizmet-simgeleri/kaynaklar.json) dosya, yayıncı, TR mağaza bağlantısı ve özgün CDN adresini saklar; aynı klasörde 9 doğrulanmış PNG var. İş ve depolama: Canva, Dropbox, Google One, Paraşüt. Müzik ve video: Exxen, GAİN, Netflix, Spotify, YouTube Premium. Bunlar dengeli bir ilk tasarım önerisidir; popülerlik sıralaması değildir. Paraşüt burada kullanıcının ödediği hizmeti temsil eder. Gruplar yalnız aramaya yardımcıdır; finansal kategori ve işletme/şahsi kapsamı kullanıcı kaydında çözülür. Her grup ad sırasıyla; özel hizmet seçimi arama sırasında da erişilebilir. YouTube Premium için YouTube uygulama simgesi kullanıldı. Görsel referanslar tasarım klasöründedir. Dağıtımda marka varlıkları ve kullanım koşulları ayrıca doğrulanır. [Netflix marka dosyaları](https://brand.netflix.com/en/assets/logos/) · [Spotify marka dosyaları](https://newsroom.spotify.com/media-kit/logo-and-brand-assets/) · [YouTube basın ve marka kaynakları](https://blog.youtube/press/).

**Akış önerisi.** Katalog seçimi ücret/sıklık/deneme süresi doldurmaz. Hizmet kimliği sabit; kullanıcı gerekirse plan adını “Ofis hesabı” gibi farklılaştırabilir. Özel hizmette ad yukarıdadır. Aynı hizmetin iki üyeliği olabilir; olası eşleşmede mevcut planı görüntüleme veya ayrı kayıt ekleme sunulur, otomatik birleştirme yok. Geri dönüş taslağı korur. Hizmetin ücretsiz denemesi yoksa uygulama bunu vaat etmez; kişi kendi sözleşmesinin türünü seçer.

- Ücretli: döngü başına tutar, sıklık, sıradaki ödeme günü → kaynak, kategori ve kategori izin verirse kapsam → kaydet. Yıllık ödeme yıllık tutarla; kartta karta yansıyacak, hesapta ödenecek. Sonraki ödeme tarihi geçmişse gecikmiş vade olup olmadığını kullanıcı doğrular. Yeni üyelik satın alma ve para çekme yok.
- Ücretsiz deneme: bitiş günü, deneme sonrası teklif ücreti/sıklığı ve D3 niyeti → değiştirilebilir hatırlatma günü ve tarih özeti → denemeyi kaydet. Bu aşamada ödeme kaynağını zorunlu tutmama önerisi; D6 gereği teklif ücreti bilinmelidir. İlk ücretli tarih teklifte belliyse isteğe bağlı saklanabilir, bilinmiyorsa ücretli takibe geçerken alınır. Erken kurulumda ikinci bir zorunlu tarih sorulmaz.
- Örnek zaman çizgisi: 23 Şubat hatırlatma, 25 Şubat deneme bitişi, kullanıcı doğrularsa 1 Mart ilk ücretli vade. İptal niyetinde “iptali hatırlat”, devam niyetinde “ücretli dönemi hatırlat”. Bildirim kapalıysa aynı kayıt uygulamada görünür. Son dakika eklenen denemede geçmiş saate bildirim kurulmaz. 23 Şubat örnek kullanıcı seçimidir; herkese sabit bir ön süre kararı yoktur.
- Bitişte “Hizmette iptal ettim / Ücretli devam ediyorum / Henüz kontrol etmedim” tasarım önerisi. İptal teyidi gelecek ücretli vadeleri keser; teyitsiz kalırsa listede kontrol bekler ve hayali ücret/ödeme üretmez. Devam teyidinde ilk ücretli tarih, tutar, kaynak ve kategori alınır; gerçek gider için normal dönem onayı gerekir. Tekrarlanan dokunuş/yeniden açılış aynı denemeden ikinci bir ücretli plan doğurmamalı. Mevcut geçmiş ödemeler korunur. Ödeme zaten gerçekleşmişse gerçek ödeme gününü kullanıcı dönem akışında kaydeder.
- Erişilebilirlik: hizmet adı ekran okuyucuda tek kez; logo dekoratif, seçimin durumu metinle; büyük yazıda alanlar dikey büyür; klavye açıkken özel hizmet ve sonraki eylem kaydırmayla erişilebilir. Arama Türkçe harf/kasa ve yaygın alternatif yazımları desteklemeli. Doğrulama hatası alanın yanında, girilen bilgiler yerinde kalır.
- Durum: dokuz ikon indirildi, yayıncı/ad eşleşmeleri ve iki pano görsel olarak kontrol edildi. Flutter, bildirim zamanlaması, cihaz veya logo dağıtım izni doğrulanmadı. Açık tasarım kararı: ilk ücretli vade ve kaynak zorunluluğunun devam teyidinde tamamlanması, bildirim önerisi ve son teyit dili.

### 19 Kasım — 14 hazır hizmet, ortak ücretli form ve deneme sonrası dönüş

[15 — güncel katalog ve tek ekranda kurulum](../../tasarim-onizleme/planlananlar-2026-11-19/15-katalog-ve-tek-form.png) · [16 — aynı aboneliğin üç ayrı tarihte görünümü](../../tasarim-onizleme/planlananlar-2026-11-19/16-deneme-takibi-ve-gecis.png). Önceki 13/14 tarihsel taslaktır. Kullanıcının istediği ChatGPT, Claude, Gemini ve aynı grupta Canva; ek öneri Notion, Apple Music ve Disney+. Paraşüt güncel listeden ve simge klasöründen çıkarıldı. 14 PNG ve yayıncı doğrulamalı [manifest](hizmet-simgeleri/kaynaklar.json). Grammarly vb. başka hizmetler için özel adla ekleme açık. Katalog aramasında GPT → ChatGPT, Google Gemini → Gemini önerilir. Mağaza simgesi hizmeti tanıtır; ₺350 veya ücretsiz deneme, gerçek hizmet teklifi değildir.

Ücretli abonelikte ilk formda ve denemeden geçişte tek sözlük, aynı sıra: Dönem ücreti → Yenilenme sıklığı → Sıradaki ödeme tarihi → Ödeme kaynağı → Kategori → Aboneliği kaydet. Ücretli döneme geçerken deneme sonrası bilinen ücret ve sıklık korunur; kullanıcı tarih ve kaynak bilgilerini aynı formda tamamlar. “Ödediğim tutar” ve “ilk ücretli vade” alan etiketi olarak kaldırıldı. Yeni kayıt takvimde açık vade oluşturur; gerçekleşme ayrı eylemdir. Denemede farklı olan “Deneme sonrası dönem ücreti” gelecekteki tutarı açıklar. Ücretsiz denemede hatırlatma günü ve devam niyeti aynı kurulum formunda; ikinci onay sayfası yok.

Kategori incelemesi: DefaultCategorySets hem şahsi hem işletme varsayılan setinde Personal("Abonelikler", Expense) tanımlıyor. Dolayısıyla ortak abonelik türü sınıflaması ile gider kategorisinin kapsamını ayır. Şahsi bağlamda uygun aktif Abonelikler kullanıcının kategorilerinde varsa önerilebilir; kullanıcı değiştirebilir. İşletme için şahsi kategoriyi sessizce bağlama. Uygun aktif işletme kategorisini veya kullanıcının kendi kategorisini seçtir. Uygun kategori bulunmazsa açık seçim göster. Marka veya seçili kart kategori/kapsam tayin etmez; ADR 0020 geçerli. Ücretsiz denemede ücretli plan teyit edilene kadar kaynak/kategori zorunlu değil (öneri).

16 numaralı pano sihirbaz değildir: 23 Şubat planlanan hatırlatmadan mevcut kayıt açılır. 25 Şubat son deneme günü; örnekte 26 Şubat hâlâ sonuç teyidi bekleniyor. Sağlayıcıda iptalse takibi tamamla; kararsızsa açık bırak; ücretli devam ediyorsa aynı kaydı ücretli forma geçir, yeni mükerrer abonelik yaratma. Kullanıcı önce “devam edeceğim” dediyse 23 Şubat hatırlatması ücretli dönemi hatırlatır. Panodaki günler, tutar, seçilmiş kart/kategori örnek kullanıcınındır; ön seçim kuralı veya gerçek ChatGPT teklifi değildir. Cihaz testi yapılmadı.

### 19 Kasım — 16 hizmet, kategori önerisi ve ücretli aboneliğin ayrıntısı

[17 — katalog, kapsam ve ücretli ayrıntı](../../tasarim-onizleme/planlananlar-2026-11-19/17-hizmet-kapsam-ve-abonelik-ayrintisi.png). İş ve depolama grubuna Trello, müzik ve videoya Deezer önerildi. İki hizmetin yayıncı ve simgesi TR App Store üzerinden doğrulandı; [manifest](hizmet-simgeleri/kaynaklar.json) 16 kayıt taşır. Gruplarda 4/4/8 hizmet var. Katalog hücreleri eş yükseklikte, hizmet adları ve simgeleri aynı çizgide. Büyük yazıda tek sütun ve doğal yükseklik; alt eylem güvenli alan/klavyeyle çakışmamalı. Ortak token ve Flutter denetimleri uygulama tesliminde geçerli.

**Karşılaştırma.** [YNAB](https://support.ynab.com/en_us/categorizing-transactions-a-guide-HyRl60sks) ödemeyi kime yaptığın üzerinden geçmiş kategori tercihini hatırlıyor ve değiştirilebilir tutuyor. Bizde aynı hizmetin hem iş hem şahsi planı olabilir; marka bazlı son kapsamı kopyalamak bu senaryoyu bozar. [Apple](https://support.apple.com/tr-tr/118448) listeden seçilen hizmetin plan seçeneklerine gider. [Rocket Money](https://help.rocketmoney.com/en/articles/2185531-managing-your-bills-and-subscriptions) manuel aboneliğin sonraki vade/tutarını yönetir. Nomiva için çıkarım: ayrıntı hizmet kimliğiyle birlikte sonraki mali etkiyi göstermeli. Sağlayıcıdaki paketi değiştirmek Nomiva eylemi değildir.

**Öneri, henüz ürün kararı değil:** Üç katalog grubunu rapor kategorisi yapmak raporda abonelik toplamını parçalar; özel hizmetlerde hangi gruba girdiğini de ayrıca sormak gerekir. İki ayrı kategoriye işletme/şahsi anlamı yüklemek yerine, bu giriş için varsayılan rapor kategorisi **Abonelikler** olsun ve kategori iki kapsama açık olsun. Şirketi olan kullanıcı ortak Kapsam seçimiyle İşletme / Şahsi belirler. Şirketi olmayanda geçerli şahsi kapsam kullanılır; iki seçenek gösterilmez. Ödeme kaynağı için ortak kapsam ön değeri kuralları geçerlidir; kullanıcı seçimini değiştirebilir. Aynı normalleştirme ücretli başlangıç ve denemeden ücretliye geçişte kullanılır. Deneme hatırlatması tek başına gidere dönüşmez.

Bugün varsayılan Abonelikler kategorisi iki sette de şahsi ve özel bir rol taşımıyor. Tasarım bugünkü API ile bu kategoriyi işletme olarak kaydedebilir varsayımı yapmaz. Öneri kabul edilirse yeni kullanıcıların varsayılanı, güvenilir biçimde tanınabilen mevcut varsayılanların iki kapsama genişletilmesi ve aktarılmış/eski planlar ayrı incelenir (ADR 0020 İ1–İ6). Ad eşitliğiyle kullanıcının özel kategorisine dokunulmaz; yeniden adlandırılmış, pasif, silinmiş veya belirsiz kayıtta açık çözüm gerekir. Eski kayıt/plan kapsamı geriye dönük değiştirilmez. Var olan farklı kategorili bir plan sırf düzenleme açıldı diye yeniden sınıflanmaz. Gerekirse istisnai kullanımda ayrıntılı düzenlemede “Kategoriyi değiştir”; sıradan kurulumda uygun özel kategori yaratmak veya genel listede aramak kullanıcıdan istenmez. Kullanıcı kabul etmeden kategori seti ve çalışan kod değişmez.

**Ayrıntı akışı:** Aktif satır → hizmete ait tam sayfa (aynı hizmetin ayrı planları varsa plan kimliğiyle). Üst özet plan adı/logo, kapsam, kaynak ve olağan dönem tarifesi; ilk açık vade kendi tutarı ve kaynak etkisi. Karta bağlıysa “Harcamayı kaydet”, hesaba bağlıysa “Ödeme kaydet”. Eylem tek dönemin panelini açar. Örnek 5 Mart ₺350 kart harcaması gerçekleşince mart bekleyenden çıkar, geçmişe girer, aylık kuralda sıradaki 5 Nisan öne alınır. Hesaptan nakit çıkışı ekstre ödemesinde ele alınır. Geçmişten dokunulan dönem oluşturduğu harekete gider; son iki gösterilir. Örnek planın ikiden fazla eski ödemesi var. Yeni kaydedilen planda geçmiş yoktur (panonun üçüncü karesi ayrı, süregelen plan örneğidir). Aynı ücretli plan Düzenle ile önceki alanları dolu açar; tek vadeyi değiştirmek tüm planı değiştirmez. Gecikenler ve kullanıcı seçili vadeden gelmişse bağlam korunur. Duraklatılan/biten durumda durum görünür; biten plan için yeni vade üretilmez, geçmiş bekleyen korunur. Takibi sona erdirmek sağlayıcıdaki üyeliği iptal etmez. Ücret ve tarih sentetiktir. Statik panoda hizalama ve boşluklar gözle kontrol edildi; cihaz/büyük yazı testi yapılmadı.

### 19 Kasım — kapsam seçimi, düzenleme ve geçmişin görünürlüğü

[Güncel pano](../../tasarim-onizleme/planlananlar-2026-11-19/18-abonelik-katalog-ve-ayrinti.png). Katalogda Trello yerine Microsoft 365, Deezer yerine önerilen Prime Video var. Office, Excel, Word ve PowerPoint aramalarında Microsoft 365 gösterilmeli. Prime Video ve Amazon Prime araması aynı aday hizmeti bulabilir; paket için aynı ödeme ikinci kez planlanmaz. Güncel 16 hizmetin kaynakları hizmet-simgeleri/kaynaklar.json içinde. Microsoft için Microsoft’un kurumsal dört renkli simgesi kullanıldı. Önceki panolar tarihsel.

Kullanıcı ortak Abonelikler kategorisinin iki kapsamı desteklemesini kabul etti. Yeni ücretli kayıt ve denemeden geçişte kategori satırı kaldırılır. Genel kullanıcı formda yalnız geçerli kapsamı seçer (yalnız şahsi kullanıcıda gereksiz seçim yok); ödeme kaynağı ayrı kalır. Kategori sunucuda doğrulanır; bugünkü şahsi hazır kategori işletmeye sessiz atanamaz. Mevcut özel kategorili planlar düzenleme veya yükseltmede kendiliğinden sınıflandırılmaz.

Düzenleme için öneri: sağ üstte tek eylem olarak kalem ve “Düzenle”; ekran okuyucu adı “Aboneliği düzenle”. Artı yeni kayıt içindir. Geçmiş son iki gerçekleşmeyi gösterir. 0 kayıtta bölüm-boş metni, 1–2 kayıtta mevcut satırlar, 3 ve üzerinde son iki satır ve “Tüm geçmişi görüntüle”. Bağlantı ancak gerçekten başka kayıt olduğu biliniyorsa gösterilir (örneğin fazladan bir satır veya devam bilgisi). Ödeme takvimi bağlantısı gelecek/açık vadeler içindir; bu geçmiş eşiğinden bağımsızdır. Panoda iki geçmiş ödeme örneklenir. Statik çizim kontrol edildi; Flutter/cihaz testi yapılmadı.
