# Proje Durumu

Bu belge yalnız doğrulanmış proje gerçeğini kaydeder. Planlanan işler,
uygulanmış veya tamamlanmış gibi gösterilmez.

## Aktif konum

- **Repo 21 Ağustos 2026'da kuruldu.** Kod tabanı, kişisel bütçe projesinden
  (`Kisisel-Butce-Mobil`) tek temiz commit olarak taşındı; ürün yönü şahıs
  şirketi ve esnaf finansına çevrildi. Eski repo dokunulmadan arşiv olarak
  duruyor ve geçmiş kaydı orada
- **22 Ağustos 2026: ürün yönü ve aşama zinciri kararlaştırıldı.** Belgeler
  yeniden yazıldı; kod değişmedi
- **Aşama 01 — Kapsam boyutu ve işletme kimliği: tamamlandı** (22 Ağustos'ta
  açıldı, 23 Ağustos 2026'da kullanıcı onayıyla kapandı). Dokuz çalışma
  grubunun hepsi bitti, cihaz kabul turu yürütüldü. Belge
  `docs/archive/stages/01-kapsam-boyutu-ve-isletme-kimligi.md` altına taşındı
  ve tamamlanma kaydı orada
- **Aşama 02 — Cari hesap: karşı taraf ve açık bakiye: tamamlandı**
  (23 Ağustos'ta açıldı, 24 Ağustos 2026'da kullanıcı onayıyla kapandı).
  Belgesi `docs/archive/stages/02-cari-hesap-ve-karsi-taraf.md` altına taşındı
  ve tamamlanma kaydı orada. Sekiz çalışma grubu: **Grup 1–5
  tamamlandı**: karar kapısı (ADR 0014) kabul edildi, cari hesabın domain
  katmanı yazıldı, üç tablo kalıcılığa girdi, cari bakiye tek sorgulu bir
  okuma modeli olarak gerçek SQL üzerinde ölçüldü, taksitli borç modeli
  karşı tarafa bağlandı (yükseltme yolu dolu bir veritabanında test edildi)
  ve cari hesabın yazma yolu, birleşik feed'e ve raporlara katılması
  tamamlandı. **Grup 6** fiş okumanın karşı taraf önerisini bağladı ve
  **Grup 7** cari hesabın Flutter ekranlarını yazdı (liste, ayrıntı, dört
  form, `Diğer` menüsünde kendi kapısı). **Grup 8** yedek şemasını v7'ye
  taşıdı ve cari deftere kendi CSV dışa aktarımını verdi. **Sekiz çalışma
  grubunun hepsi bitti.** Fiş önerisinin görünür kabul/red rozeti eklendi ve
  cihaz kabul turu tamamlandı
- **Aşama 03 — Yükümlülük ve vade: tamamlandı** (24 Ağustos 2026'da açıldı ve
  aynı gün kullanıcı onayıyla kapandı). Belgesi
  `docs/archive/stages/03-yukumluluk-ve-vade.md` altına taşındı ve tamamlanma
  kaydı orada. Yedi çalışma grubunun hepsi bitti: yükümlülüğün domain'i ve
  kalıcılığı, cari borçlandırmaya vade, tekrarlayan planda bitiş sınırı,
  kanonik planlanan projection'a katılım, fiş okumanın "ödemedim" yolunun
  bağlanması, Flutter liste + idempotent kapanış akışı ve yedek şemasının
  v8'e taşınması. Kapanış öncesi kod denetimi iki arayüz boşluğu buldu ve
  ikisi de kapatıldı: yükümlülüğün fotoğrafsız (elle) girişi — yön seçimiyle —
  ve tekrarlayan plan formunun bitiş tarihi alanı
- **Aşama 04 — Kasa, POS ve gezinme: tamamlandı** (24 Ağustos 2026'da açıldı,
  26 Ağustos 2026'da cihaz kabul turuyla kapandı). Belgesi
  `docs/archive/stages/04-kasa-pos-ve-gezinme.md` altına taşındı ve tamamlanma
  kaydı orada. Sekiz çalışma grubunun hepsi bitti: ADR 0015, gün sonu kasa
  sayımı, POS tahsilatı ve yoldaki para, üçüncü ana sekmenin ön ayara göre
  değişmesi, `İşlem ekle` menüsünün niyet eksenine taşınması ve yedek v9
- **Aşama 05 — Vergi ve muhasebeci: tamamlandı** (26 Ağustos 2026'da açıldı ve
  aynı gün cihaz kabul turuyla kapandı). Belgesi
  `docs/archive/stages/05-vergi-ve-muhasebeci.md` altına taşındı ve tamamlanma
  kaydı orada. Sekiz çalışma grubunun hepsi bitti: ADR 0016, KDV taşıyan
  alanlar, indirilebilirlik, vergi/SGK takvimi, ay sonu muhasebeci paketi,
  karşılık olarak hedefler, Flutter ve yedek v10
- **26 Ağustos 2026: zincire bir aşama kümesi eklendi.** Kullanıcı bulut
  geçişini ertelemek ve öncesinde ürünü tamamlamak istedi. Bulut aşaması
  **Aşama 07** oldu (`stages/07-bulut-guvenli-beta.md`) ve önüne üç aşama
  girdi: **06 — Hesap ve kalan işler**, **06.1 — Güvenlik taraması**,
  **06.2 — Arayüz düzeni**. Küme, iş biriktiği için bölündü: uygulamaya bir şey
  eklendikçe arkasında yeni iş kalıyor ve tek bir dev aşama hiç kapanmıyor;
  ihtiyaç oldukça 06.3, 06.4 açılacak, hepsi kapandıktan sonra 07'ye geçilecek.
  `stages/README.md` içindeki "alt seviye numara alınmaz" kuralı bu yüzden
  değiştirildi. E-posta doğrulama ile parola sıfırlama buluttan 06'ya alındı
  (kod tabanlı, giden e-posta servisiyle çalıştığı için buluta ihtiyaç
  duymuyor); push bildirim kararı 07'de kaldı; fiş veri sınırı kararı
  (`docs/backlog.md` madde 4) 07'nin **açılış kararı** oldu.
  **Yalnız karar belgeleri değişti, kod değişmedi**
- **Aşama 06 — Hesap ve kalan işler: tamamlandı** (27 Ağustos 2026'da açıldı,
  28 Ağustos 2026'da kullanıcı onayıyla kapandı). Belgesi
  `docs/archive/stages/06-hesap-ve-kalan-isler.md` altına taşındı ve tamamlanma
  kaydı orada. Açık kapsamlı bir aşamaydı ve "liste bitti" diye değil, kullanıcı
  kapatmak istediği için kapandı. Sekiz çalışma grubu: hesap ve güvenlik ekranı
  (ADR 0017), e-posta doğrulama ve parola sıfırlama, cihaz üstü hatırlatma,
  varsayılan kapsam, kart alacağının kırpılmaması, fiş/dekont kabul turu, bütçe
  ekranının on maddesi ve kabul turunda çıkan üç kusurun kapanması.
  `docs/backlog.md` maddeleri 1, 2, 3, 5, 6, 7 ve 8 kapandı; yalnız madde 4
  kaldı ve o Aşama 07'nin açılış kararı. **İki iş devredildi:** gerçek posta
  ile doğrulama turu (Brevo anahtarı kurulunca) ve Pixel 8 toplu kabul turu
- **Aşama 06.1 — Güvenlik taraması: tamamlandı** (28 Ağustos 2026'da açıldı,
  1 Eylül 2026'da kullanıcı onayıyla kapandı). Belge
  `docs/archive/stages/06.1-guvenlik-taramasi.md` altına taşındı; tamamlanma
  kaydı orada. Beş çalışma grubu: secret taraması, bağımlılık zafiyet
  taraması, yetkilendirme kapsamı denetimi, log ve hata cevabı sızıntısı,
  bulguların çözülmesi ve CI kapısının kurulması. **Aşama 06'nın Pixel 8 toplu
  kabul turunu da taşıyor** — kullanıcı kararı: bu aşama koda dokunacağı için
  (bağımlılık yükseltmesi, eksik negatif testler, log/hata sınırı) turu ondan
  önce koşmak aynı ekranları iki kez gezmek olurdu
- **31 Ağustos 2026: Aşama 06.1 Grup 5 tamamlandı.** Grup 1–4'ten çıkan her
  bulgu bir sonuca bağlıydı (dört düzeltme, üç yazılı gerekçeyle kabul) ve
  kapının kurulmasında bir eksik çıktı: bağımlılık taramasının pub ayağı CI'da
  yoktu, yalnız Grup 2'de elle koşulmuştu. `scripts/Invoke-PubAdvisoryScan.ps1`
  ile kalıcı oldu (pub.dev güvenlik duyurusu ve geri çekme bayrakları).
  Dört taramanın da CI'da koştuğu ve **dördünün de bir bulgu enjekte
  edildiğinde gerçekten kırıldığı** ayrı ayrı doğrulandı; enjeksiyonlar
  yalnız yerelde yapıldı ve commit edilmedi. Kilit dosyasındaki kısıt içi dört
  yükseltme alındı. Backend 1010 test geçti (55 skip: SQL bağlantısı ve Gemini
  canlı sözleşmesi yok), Flutter 875 test yeşil; format ve analyze temiz. **Aşamanın kalan tek işi Pixel 8 toplu kabul turudur**
- **31 Ağustos 2026: Aşama 06'dan devredilen toplu kabul turu Pixel 8'de
  koşuldu.** Tur iki kalıcı araç bıraktı — `scripts/New-AcceptanceFixture.ps1`
  (kabul hesabını API üzerinden dolduruyor) ve `scripts/new-sample-documents.py`
  (yedi sentetik fiş/fatura/dekont ve bir banka ekstresi CSV'si). **İki kusur
  bulundu ve ikisi de düzeltildi:** kart detay ekranı yazma sonucunu (hatayı da
  başarıyı da) hiç söylemiyordu — reddedilen bir ödeme sessizce kayboluyordu; ve
  fişten okunan KDV "henüz ödemedim" yolunda düşüyordu, oysa `Obligation`
  ADR 0016'nın KDV taşıyan beş kaydından biri. İkisi de teste bağlandı ve
  düzeltmeden sonra cihazda yeniden denendi. Flutter tarafında 878 test geçiyor.
  Kabul turunda çıkan bir yerleşim işi (`Kasa` sekmesinde iki yüzen düğmenin
  çakışması) `docs/backlog.md` madde 12 olarak Aşama 06.2'ye bırakıldı
- **1 Eylül 2026: Aşama 06.1 kapandı, sonraki aşama açılmadı.** 06.2 (Arayüz
  düzeni) `Planlandı` kalıyor; kapsamını `research/rakip-arayuz-ve-akis/`
  altındaki rakip arayüz/akış araştırması besliyor (ayrı oturumda yürüyor,
  uygulama koduna dokunmuyor). Araştırma bitince kullanıcı 06.2'yi açar.
  **Şu an hiçbir aşama Aktif değil; bu süre boyunca kod değişmez**
- **2 Eylül 2026: Aşama 06.2 dar bir yerel web deneme checkpoint'iyle açıldı.**
  Flutter'ın resmî `web/` platform kabuğu eklendi; web yerelde
  `http://localhost:5284`, Android emulator `http://10.0.2.2:5284` varsayılanını
  kullanıyor. API yalnız Development'ta yapılandırılmış
  `http://localhost:65087` origin'ine CORS izni veriyor; izinli ve izinsiz
  preflight yolları integration testiyle korunuyor. Responsive web düzeni,
  bildirim, kamera, yayın ve ayrı API bu checkpoint'in kapsamında değil
- **3–27 Eylül 2026: rakip arayüz/akış araştırması yürüdü, uygulama kodu
  değişmedi.** `research/rakip-arayuz-ve-akis/` altında Belge 1 (arayüz) ve
  Belge 2 (finansal akışlar) tamamlandı; Belge 3'ün (BusinessFinance için
  kararlar) yöntem planı taslak, yazımına başlanmadı. Araştırmanın canlı durumu
  `research/rakip-arayuz-ve-akis/DURUM.md` içindedir
- **27 Eylül 2026: 06.2'ye cloud üzerinde Flutter arayüz denemeleri
  checkpoint'i açıldı.** Belge 3'ten önce `ui-trials` dalında yapılır; tasarımın
  gerektirdiği veri için backend'e ekleme yapılabilir. Kapsamı denemeler
  ilerledikçe aşama belgesine yazılır. İlk madde Claude Design teslim
  paketinin uygulanması; ekran ekran ilerliyor:
  - **Özet**: tipografi DS token'larına eşitlendi, yeni ortak parçalar
    (`AppPageHeader`, `AppIconCapsule`, `AppRow`, `AppStatusTag`,
    `AppCardHead`, `AppTextAction`, `AppDateLeaf`, `AppDetailBlock`,
    `AppSegmentRail`, `AppAvatar`) eklendi, ekran yeniden kuruldu. Backend
    net varlığın iki tarafının toplamını, yoldaki paranın geçiş gününü ve
    planlanan görünümde vadesi gelmemiş ödemelerin toplamını
    (`upcomingOutgoingTotal`) döndürüyor. Ekran görüntüsü düzeneği
    (`test/screenshots/`) ekranı teslim paketindeki çerçeveyle aynı biçimde
    çizip karşılaştırmaya açıyor. Kontroller: Flutter analyze temiz, 879 test
    geçti; backend Release build 0 uyarı, format temiz, Domain/Application/Api
    testleri geçti. Infrastructure'daki 10 fiş görüntü testi cloud ortamında
    `libSkiaSharp` yerel kütüphanesi yüklenemediği için düşüyor; değişiklik
    öncesi kodda da aynı sonuç alındı (ortam kaynaklı)
  - **Bütçeler**: geri oklu başlık ve `+`, doluluğa göre dizilmiş kartlar,
    durum kapsülü (`Aşıldı` / `Limite yakın` / `Limit içinde`), limit
    düzenleme ve silme harcama panelinin altında. 880 Flutter testi geçti
  - **İşlemler + detay**: arama alanı (backend'de feed sorgusunun `search`
    filtresi, 100 karakter sınırı), `Borçlar` çipi, planlananlar şeridi,
    güne göre gruplu ve başlığı sabit akış, yeni ayrıntı paneli. İptal
    edilmiş tutar artık işaretsiz yazılıyor (DS). Kontroller: 881 Flutter
    testi, backend Application 325 test geçti; SQL arama testi yerelde
    koşacak (cloud'da SQL yok)
  - **İşlem ekle paneli**: üç renkli kutucuk, belgeden oku, diğer listesi;
    seçenek adları tasarıma göre kısaldı
  - **Kasa**: sekmeler kalktı, tek akış (kasa seçici rayı → bugünün sayım
    kartı → POS tahsilatları → son sayımlar). Sayım paneli `Toplamı yaz |
    Banknotla say`, Türkçe binlik ayırıcı ve tam aritmetikli canlı fark
    önizlemesi taşıyor; POS detayı yeni panelde. Backend: `today` cevabına son
    sayım ve bugünkü nakit giriş/çıkış eklendi; sayım anındaki beklenen tutar
    `ExpectedAtCount` olarak saklanıyor (`AddCashCountExpectedSnapshot`,
    nullable, backfill yok, yedek v9 taşımıyor). Kontroller: 888 Flutter testi,
    backend Domain/Application/Api geçti; Infrastructure'da yalnız ortam
    kaynaklı 10 SkiaSharp testi düşüyor
  - **Diğer**: hesap kartı (baş harf, e-posta, doğrulanmamış adres rozeti) ve
    dört grup — Para ve hesaplar, Planlama, Vergi ve muhasebe (yalnız
    işletme), Ayarlar. `Hesabım` liste satırından karta taşındı; kişisel
    profilde `Kasa` Para ve hesaplar altında. 888 Flutter testi geçti
  - **Hesabım**: geri oklu tam ekran; doğrulama uyarısı kimlik kartının
    içinde, `Tercihler` altında `İşletmem var`, oturumlarda sayaç, göreli
    zaman ve `Diğerlerini kapat` (var olan tekil uçla sırayla), ayrı
    `Çıkış yap` düğmesi, silmeden önce yedek kapısı. Büyük yazıda bölüm
    başlığının yan eylemi alta iniyor. 891 Flutter testi geçti
  - **Yerel doğrulama ve ilk cihaz düzeltmeleri (28 Eylül 2026)**: migration
    geliştirme ve `BusinessFinanceApiSqlTests` veritabanlarına uygulandı; SQL
    dahil bütün backend testleri yerelde ilk kez koştu ve SkiaSharp testleri
    geçti. Buluttan kalan tek kırık: gelişmiş raporun sorgu bütçesi 69'du,
    net varlığın sıradaki geçiş günü sabit bir `MIN` sorgusu ekliyor → 70.
    Pixel 8 karşılaştırmasından üç düzeltme: `İşlem ekle > POS tahsilatı`
    formundan vazgeçen kullanıcı geldiği ekrana dönüyor ve ayrı sayfa olarak
    açılan `Kasa` geri ok taşıyor (alt çubuksuz ekranda kalıyordu); şahsi
    tarafın adı yönünden bağımsız olarak `Şahsi net` (tasarım kararı, bulut
    oturumu eski `Şahsi çekim` kuralını korumuştu); `Yeme-içme` kategorisi
    nötr ikona düşüyordu, yemek ikonunu alıyor. Yerel sentetik veride eski
    sayımların `ExpectedAtCount` değeri kayıtlı düzeltme kaydından geri
    çıkarıldı (kod değişmedi; alan hâlâ backfill'siz). Kontroller: backend
    SQL dahil geçti (Domain 311, Application 325, Api 235, Infrastructure 203
    + 2 canlı test atlandı), format temiz; Flutter analyze temiz, 895 test
  - **Bütünsel düzenleme kararları (28 Eylül 2026, kod değişmedi)**: Kasa/POS ve
    vergi temaları araştırılıp kullanıcı kararıyla kapandı (`research/YOL-HARITASI.md`,
    `research/kasa-pos-gun-sonu/KAPANIS.md`, `research/vergi/YENI-YAKLASIM.md`).
    **Veri kaybettiren karar (kullanıcı onayı):** muhasebeci paketi, KDV alanları ve
    indirilebilirlik kaldırılacak; girilmiş KDV/indirilebilirlik verisi silinebilir —
    yerel veri sentetik. Uygulamadan önce kararlar temiz bir oturumda denetlenecek;
    ADR, PRD ve aşama belgeleri denetimden sonra yazılacak
- **29 Eylül 2026: karar denetimi yapıldı, karar belgeleri yazıldı, Aşama 06.3
  açıldı (kod değişmedi, commit atılmadı).** Denetim `research/DENETIM-2026-09-29.md`
  ve dış kaynak ekinde; sonuçları `research/kasa-pos-gun-sonu/KAPANIS.md`
  (KP1–KP22) ve `research/vergi/YENI-YAKLASIM.md` §6.6'ya işlendi. Kullanıcı
  muhasebeci paketinin **tamamen** kaldırılmasına karar verdi (ürün bütçe
  uygulamasıdır, ön muhasebe değildir). Yazılanlar: **ADR 0018** (vergi bir nakit
  planıdır) ve **ADR 0019** (gün sonu, POS tanımı, yatış, kartla tahsil) —
  ikisi de kullanıcı onayıyla **kabul edildi** (29 Eylül 2026); ADR 0005, 0015 ve 0016'ya
  yönlendirme notları; PRD, `AGENTS.md`, `CLAUDE.md`, `PROJECT-ROADMAP.md`.
  Kullanıcı kararıyla ADR'ler artık **iki katmanlı** (bağlayıcı "İlkeler" +
  değişebilir "Başlangıç tasarımı"); `AGENTS.md`'ye "Kararlardan sapma" kuralı,
  aşama belgelerine "Sapmalar" tablosu eklendi.
  **Aşama 06.3 — Bütünsel düzenleme Aktif, 06.2 Beklemede** (kullanıcı kararı).
  Dal `ui-trials`'ta kalıyor; bir bulut oturumu açıldığında `main`'e alınıp yeni
  dal açılacak
  - **06.3 Grup 1 — kesin hatalar (29 Eylül 2026, tamamlandı; emülatör
    kabulü kullanıcıyla yapıldı)**: Kasa her bakiye değişiminde yenileniyor
    (Kasa/POS controller'ları `cash` hedefini hiç dinlemiyordu; hesapları
    yükselten her olay artık `cash`'i de yükseltiyor); POS kaydı iptal
    edilebiliyor ve yanlış "hesaba geçti" geri alınabiliyor (iki yeni `DELETE`
    ucu, sahiplik denetiminde); sayımdan sonra değişen kasa "oturdu" demiyor
    (`today.changeSinceCount`); içe aktarımda çifte sayım notu, ipucu ve satır
    başına "Atla"; boş tarih alanında üst üste binen etiket, "quarterly" ve
    yanıltıcı "Sonraki" düzeldi. Kontroller: backend SQL dahil geçti (Domain
    313, Application 325, Api 237, Infrastructure 203 + 2 canlı test atlandı),
    build 0 uyarı, format temiz; Flutter analyze temiz, 907 test geçti (19 ekran
    görüntüsü testi atlandı), format temiz, debug APK derlendi. Pixel 8
    denemesinde (T1b, T2, T6, CSV, plan satırı) sayım kartının anlatımı
    kullanıcıyla yeniden kuruldu: bugünkü sayım `Son sayımlar`da da görünüyor,
    durum `Sonradan kayıt girildi`, değişim `Uygulamaya göre`nin altında kısa
    açıklama ve tek cümlelik yönlendirme. Son koşum: Flutter 909 test geçti
  - **06.3 Grup 2 — KDV, indirilebilirlik ve muhasebeci paketi kalktı
    (29 Eylül 2026, tamamlandı; emülatör kabulü kullanıcıyla yapıldı)**: ADR 0018
    İ2'nin uygulaması. Beş kayıttan KDV, gider kayıtlarından ve kategoriden
    indirilebilirlik, muhasebeci paketinin iki ucu ve ekranı kaldırıldı.
    **Veri kaybettiren migration** `RemoveVatAndTaxDeductibility` (yukarıdaki 28
    Eylül kullanıcı onayı): on beş kolon ve on beş `CK_*` kısıtı düşüyor,
    kısıtlar önce; dolu veritabanında yükseltme testi geçti. Yedek şeması v11
    (KDV'siz), v10 reddediliyor. Fiş okuma KDV'yi forma yazmıyor; tutar yalnız
    toplam denetiminde. Formlarda KDV bölümü ve indirilebilirlik anahtarı yok;
    `Diğer`deki grup `Vergi` adını aldı. Kontroller: backend SQL dahil geçti
    (Domain 292, Application 308, Api 220, Infrastructure 202 + 2 canlı test
    atlandı), build 0 uyarı, format temiz; Flutter analyze temiz, 895 test
    geçti (19 ekran görüntüsü testi atlandı), format temiz, debug APK derlendi.
    Migration yerel geliştirme veritabanına uygulandı
  - **06.3 Grup 3 — Vergi takibi (1 Ekim 2026, tamamlandı; emülatör denemesini
    kullanıcı yaptı)**: ADR 0018'in uygulaması. Tekrarlayan plan vergi türü,
    ayın günü ve "seçilen aylarda" ritmi taşıyor; vergi planında tutar ve
    kaynak boş olabiliyor; kalem "kapatıldı" durumunu ve kapatan ödemeyi
    taşıyor; kategoride vergi işareti var. `AddTaxPlans` migration'ı veri
    kaybettirmiyor, dolu veritabanında yükseltme testi geçti. Yeni uçlar:
    vergi okuması, tanım ayrıntısı, toplu tanımlama, "Ödedim" (ödeme günü +
    hesap/kart), "tutar belli oldu", geri alma, toplu vergi ödemesi (istek
    kimliğiyle idempotent) ve Ödenenler. Vergide kapsam kaynağın etiketine
    bakmıyor (ADR 0018 İ9, kullanıcı kararıyla güncellendi). Flutter'da
    `Vergi takvimi` kalktı, yerine **Vergi takibi** geldi: ilk kullanım,
    Bekleyenler / Vergilerim / Ödenenler (son beş, hepsi ay başlıklarıyla
    `Tümü`nde), "Ödedim", "Tutarı gir", "Vergi ödemesi ekle", tanım formu ve
    ayrıntısı. Emülatör turunda kullanıcının bildirdikleri düzeltildi: tutar
    alanının ortalanması, "Tutarı gir"in kaleme geri dönmesi, panellerin ekranı
    kaplaması ve kapatılamaması, geri alma onayının tasarımdaki kayıt kartına
    geçmesi, `Sil`in kırmızı kalması. Tasarımla karşılaştırma için
    `test/screenshots/taxes_screenshot_test` eklendi. Vergi menüsü kişisel
    profilde gizli kalıyor (kullanıcı kararı, 1 Ekim). Kontroller: backend SQL
    dahil geçti (Domain 307, Application 327, Api 240, Infrastructure 206 + 2
    canlı test atlandı), build 0 uyarı, format temiz; Flutter analyze temiz,
    919 test geçti (33 ekran görüntüsü testi atlandı), format temiz, debug APK
    derlendi
- Zincir: 01 kapsam boyutu → 02 cari → 03 yükümlülük/vade → 04 kasa/POS →
  05 vergi/muhasebeci → 06 hesap/kalan işler → 06.1 güvenlik taraması (kapandı) →
  06.2 arayüz düzeni (beklemede) → **06.3 bütünsel düzenleme (aktif)** →
  (gerekirse 06.x) → 07 bulut (`PROJECT-ROADMAP.md`)

## Taşımada yapılan ve doğrulanan işler

21 Ağustos 2026, tek oturum:

- **Yeniden adlandırma.** `PersonalBudget.*` → `BusinessFinance.*` (404 C#
  dosyası), Flutter paketi `business_finance_mobile`, Android
  `com.nef.business_finance_mobile`, veritabanı `BusinessFinance`,
  `UserSecretsId` `business-finance-api`. MethodChannel adı Dart ve Kotlin
  yakalarında birlikte değişti
- **Belge budaması.** 80 Markdown → 35. Arşiv aşama belgeleri, öğrenme
  notları, oturum notları ve taslak 13–18 aşamaları taşınmadı; eski repoda
  duruyorlar
- **Migration çökertmesi.** 43 dosya / 36.766 satır → 3 dosya / 5.924 satır,
  tek `InitialCreate`. Şema denkliği, gerçek veriyi taşıyan veritabanı ile
  sıfırdan kurulan veritabanının **543 satırlık tam dökümü** karşılaştırılarak
  kanıtlandı (kolon, indeks, CHECK, FK, PK/UQ — hepsi birebir). Backfill
  migration'larından kalan 5 artık DEFAULT kısıtı düşürüldü; şema artık modelle
  tam örtüşüyor
- **Veri korundu.** Eski veritabanı `COPY_ONLY` yedekle kopyalandı; 28 tablonun
  satır sayıları birebir aynı. `__EFMigrationsHistory` 21 satırdan 1 satıra
  indi, `database update` no-op
- **Ayrı Docker container.** `business-finance` compose projesi, kendi volume'u
  ve `127.0.0.1:14334` portu. Eski container ve volume ile hiçbir ortak nokta
  yok; yeni SA parolası ve yeni JWT imzalama anahtarı üretildi
- **CI kuruldu.** Önceki repoda `.github/workflows` boştu

### Taşımada kaybedilen test kapsamı — bilinçli

Migration zinciri taşınmadığı için ona bağlı iki test geçersiz kaldı:

- `Stage125Migration_BackfillsExistingRecurringRowsAsAccountSource` **silindi**.
  Yükseltme yolunu (v2 satırların `SourceType=Account` olarak dönüşmesi) gerçek
  SQL üzerinde doğruluyordu. O dönüşüm kopyalanan veritabanında zaten uygulanmış
  durumda ve yükseltilecek başka veritabanı yok
- `MigrationHistoryTests` **yeniden yazıldı**. Eskisi 21 migration'lık zincirin
  adlarını, sırasını ve yükseltme güvenliği kurallarını doğruluyordu; yenisi
  tekliği, modelle örtüşmeyi (`HasPendingModelChanges`) ve yapısal sayımları
  doğruluyor
- Zincirin taşıdığı **yükseltme güvenliği bilgisi kaybolmadı**: kurallar
  `AGENTS.md` içindeki "Migration kuralları" bölümüne yazıldı ve bundan sonraki
  her migration için bağlayıcı

`SourceType` invariant'ının kendisi Domain, Application ve API seviyesinde
testli kalmaya devam ediyor; kaybolan yalnız bir kereye mahsus geçmiş
dönüşümün testiydi.

## Son doğrulamalar

21 Ağustos 2026 itibarıyla, taşınmış kod tabanı üzerinde:

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **733 geçti**, 1 atlandı |
| Flutter analyze | No issues found |
| Flutter format | Temiz |
| Flutter test | **637 geçti** |
| Şema denkliği | 543/543 satır birebir |
| Veri bütünlüğü | 28 tablo, satır sayıları birebir |

Atlanan tek test `GeminiLiveContractTests` — canlı API anahtarı ortam değişkeni
istiyor, yokken kendiliğinden skip oluyor.

## Doğrulanan ortam

- .NET SDK: net10.0, çalışıyor
- Docker/Compose: `business-finance-sqlserver-1` healthy, `127.0.0.1:14334`
- SQL Server: `BusinessFinance` (veri) ve `BusinessFinanceApiSqlTests`
  (API SQL testlerinin hedefi; şema uygulandı). Infrastructure SQL testleri
  bağlantıdan yalnız sunucuyu alıp kendi geçici veritabanını kurar
- Flutter/Dart: çalışıyor, 637 test geçiyor
- Android emulator: bu oturumda çalıştırılmadı

## Güvenlik ve veri durumu

- Gerçek finansal veri kullanılmıyor; veritabanındaki her kayıt sentetiktir
- `.env` ve user-secrets Git dışında; `.gitignore` `.env`'i kapsıyor
- Eski reponun Git geçmişi denetlendi: parola, connection string veya API
  anahtarı sızıntısı **yok**
- İnternete açık servis yok; SQL yalnız loopback'e bind'lı

## 22 Ağustos 2026 — planlama turu

Kod değişmedi; ürün yönü ve zincir kararlaştırıldı ve belgelere yazıldı:

- **ADR 0013 yazıldı** — işletme ve şahsi tek havuzda bir boyuttur. Giriş modu
  seçimi ve iki ayrı veri alanı gerekçeleriyle reddedildi. Karar, uygulamanın
  bugünkü yüzeyi (endpoint'ler, Flutter ekranları, domain modeli) okunarak ve
  şahıs şirketinin tüzel kişiliği olmaması gerçeğinden türetildi
- **Altı aşamalık zincir kuruldu**, sırası bağımlılığa göre belirlendi (kapsam
  boyutu en altta, cari onun üstünde, fatura cari'nin üstünde) ve **altısının
  da ayrıntılı belgesi yazıldı**. Üçü bir ADR ile açılıyor: 02, 04 ve 05
- **Yeni bir çakışma bulundu ve 02–03'e bağlandı:** kod tabanı ekonomik olayı
  iki farklı zamanda tanıyor. Kart harcaması ve borç açılışı gideri **anında**
  tanırken, fiş okumanın "faturayı ödemedim" yolu hiçbir şey tanımıyor. Aynı
  fatura, hangi ekrandan girildiğine göre farklı davranıyor. Kural (ekonomik
  olay tanır, ödeme taşır) 02'de ADR olarak yazılacak, tutarsızlık 03'te
  kapatılacak
- **`PRD-BusinessFinance.md` yeniden yazıldı.** Açık bankacılık kapsam dışına
  alındı; MVP-1/1.5/2 bölümleri kaldırıldı (devralınan tabanda zaten
  uygulanmışlardı ve PRD onları "yapılacak" diye gösteriyordu)
- **Kod okumasından çıkan dört çakışma kayda geçti:** ödenmemiş faturanın
  tekrarlayan plan olmaya zorlanması, "kredi kartı"nın POS ile ters anlam
  taşıması, cari ile taksitli borç modelinin çakışma riski, varsayılan
  kategori setinin tamamen ev bütçesi olması
- **İki kural çelişkisi düzeltildi:** `main` üzerinde çalışma yasağı ile branch
  açma yasağı aynı anda uygulanamıyordu; planlama belgeleri için tek başına
  commit istisnası tanımlandı

## 22 Ağustos 2026 — Aşama 01, Grup 1: veri sıfırlama ve temiz zemin

Kullanıcı onayıyla uygulandı; geri alınamaz adımdı.

- **Yerel `BusinessFinance` veritabanı düşürüldü ve `InitialCreate` ile sıfırdan
  kuruldu.** Sonuç doğrulandı: 28 tablo ayakta, iş verisi taşıyan satır yok
  (tek satır `__EFMigrationsHistory` kaydı). Silinen her kayıt kişisel bütçe
  uygulamasından kopyalanmış sentetik veriydi
- Bu, Grup 4'ün ön koşuludur: tablolar boş olduğu için `AddTransactionScope`
  migration'ı kapsam kolonlarını backfill'siz `NOT NULL` ekleyebilir. Aynı sıra
  dolu bir tabloda kullanılamaz (`AGENTS.md`, "Migration kuralları")
- **`manual-test-data/` gözden geçirildi; değişiklik gerekmedi.** İçerik ürün
  yönünden bağımsız: iki CSV yalnız `date,amount` kolonlarıyla içe aktarma
  ayrıştırıcısını ve yinelenen satır tespitini zorluyor, üç PDF ek dosya imza
  doğrulamasının fixture'ı, `New-CorruptedBackup.ps1` geri yükleme reddini
  test ediyor. Hiçbiri ev bütçesi kategorisi veya kişisel bütçe kimliği
  taşımıyor
- **`BUSINESS_FINANCE_SQL_TEST_CONNECTION`'ın iki tüketicisinin farklı beklentisi
  olduğu bu turda ortaya çıktı** ve `documentation/variables.md` içine yazıldı:
  Infrastructure testleri bağlantıdan yalnız sunucuyu alıp kendi geçici
  veritabanını kurup migrate ediyor, API SQL testi ise bağlantıyı olduğu gibi
  kullanıyor ve şeması önceden uygulanmış bir veritabanı istiyor
  (`BusinessFinanceApiSqlTests`)

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **733 geçti**, 1 atlandı (`GeminiLiveContractTests`, canlı anahtar yok) |
| Temiz veritabanı | 28 tablo, 0 iş verisi satırı |

## 22 Ağustos 2026 — Aşama 01, Grup 2 ve 4: kapsam boyutu

`TransactionScope` (`Business = 1`, `Personal = 2`) domainden veritabanına
kadar eklendi. Üçüncü bir "bilinmiyor" değeri yok: boyut boş bir veritabanına
girdiği için yorumlanacak geçmiş de yok.

- **Kapsamı zorunlu taşıyanlar:** `BudgetTransaction`, `CreditCardCharge`,
  `MonthlyBudget`, `InstallmentPlan`, `RecurringTransaction` ve occurrence'ı,
  `DebtAgreement`. **Taşımayanlar:** `Transfer` ve `CreditCardPayment` — bir
  test alanın sonradan eklenmediğini koruyor
- **`Account`, `Category`, `CreditCard` nullable `DefaultScope` taşır** ve bu
  alan API yüzeyine de çıktı. Boş olması meşrudur; "kapsamı bilmiyorum" değil,
  "bu kaynak kapsamı belirlemiyor" demektir
- **Bütçe ilerlemesi kapsama duyarlı hâle geldi** ve kural iki yerde birden
  yazılı: `MonthlyBudget.CalculateProgress` ve `EfBudgetRepository`. İkisi de
  harcamayı kategori + kapsam çiftiyle topluyor
- **Occurrence kapsamı plandan kopyalanır**, gerçekleşmede yeniden türetilmez.
  Kopya olması planlanan projection'ın kapsamı join'siz filtrelemesini de
  sağlayacak
- **Grup 4 bu checkpoint'e alındı.** `MigrationHistoryTests` modelle şemanın
  örtüşmesini doğruluyor; kapsam modele girip migration üretilmezse grup
  kırmızı kalırdı. Aynı zorunlulukla Grup 9'un **yalnız sürüm kapısı** da
  buraya girdi — geri yükleme kodu kapsam alanı olmadan derlenmiyordu
- **`20260822120440_AddTransactionScope`**, `InitialCreate`'ten sonra zincirin
  ilk gerçek yükseltme adımı. EF'in ürettiği `defaultValue: 0` kaldırıldı:
  kalıcı bir veritabanı varsayılanı bırakıyordu ve bıraktığı değer tablonun
  kendi `[Scope] IN (1, 2)` kısıtını ihlal ediyordu. Zorunlu kolonlar ham SQL
  ile varsayılansız ekleniyor ve bu, boş tablo ön koşulunu **denetliyor**
- **Yedek şeması v6**; v2–v5 `restore.unsupported_version` ile reddediliyor ve
  yükseltilmiyor. Eksik kapsam alanını doldurmak, kullanıcının işletme ile cebi
  arasındaki ayrımını uydurmak olurdu (ADR 0013)
- **Bilinen ve kabul edilen boşluk:** kapsam sunucuda **türetilmiyor**; istek
  onu açıkça göndermek zorunda. Bu yüzden Flutter istemcisi bu commit'te API'ye
  karşı çalışmıyor — create istekleri `*.invalid_scope` ile 400 alır. Boşluğu
  Grup 3 (türetme zinciri) kapatıyor; Flutter'ın kendi kontrolleri
  (`analyze`, 637 test, `format`) bu commit'te de temiz
- **Yerel SQL test hedefleri yeniden kuruldu.** `BusinessFinanceApiSqlTests`
  önceki koşumlardan kalan satırlar taşıyordu ve boş tablo ön koşulunu
  karşılamıyordu; düşürülüp migration zinciriyle yeniden kuruldu. Kullanılmayan
  `BusinessFinanceSqlTests` silindi

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **753 geçti**, 1 atlandı |
| Flutter analyze / format / test | Temiz, temiz, **637 geçti** |
| Migration | `AddTransactionScope` iki veritabanına uygulandı; `HasPendingModelChanges` yok |
| Şema | 7 tabloda `Scope NOT NULL`, 3 tabloda `DefaultScope` nullable, hiçbirinde DEFAULT kısıtı yok |

## 22 Ağustos 2026 — Aşama 01, Grup 3: kapsam türetme zinciri

Kapsam artık sunucuda çözülüyor; istemcinin göndermesi zorunlu değil.

- **Sıra tek bir yerde yazılı** (`TransactionScopeResolution`): kullanıcının
  açık seçimi → hesabın/kartın etiketi → kategorinin varsayılanı. Altı oluşturma
  yolu (hareket, kart harcaması, bütçe, taksit planı, tekrarlayan plan, borç) ve
  CSV içe aktarma aynı fonksiyonu çağırıyor; farklı cevap vermeleri, aynı
  harcamanın hangi ekrandan girildiğine göre farklı etiketlenmesi demekti
- **Üçü de boşsa istek reddediliyor** ve hiçbir kayıt yazılmıyor:
  `transactions.scope_unresolved` ve özellik başına karşılıkları. Sunucu kapsam
  uydurmuyor — yanlış etiketlenmiş bir kayıt işletme netini sessizce bozar
- **API sözleşmesinde `scope` isteğe bağlı oldu.** Tanınmayan bir metin hâlâ
  `*.invalid_scope` ile reddediliyor; boş olmakla yanlış olmak ayrı şeyler
- **`defaultScope` güncellemede yetkilidir**: boş göndermek etiketi kaldırır,
  "dokunma" demek değildir. Kaynağın tam güncel hâlini gönderen mevcut
  güncelleme sözleşmesiyle tutarlı
- **Bilinen ve kabul edilen boşluk:** hiçbir varsayılan kategori kapsam
  taşımadığı için zincir pratikte yalnız hesabına ya da kartına elle etiket
  koyan kullanıcı için çözülüyor. Bunu Grup 5'in kategori setleri kapatıyor;
  Flutter istemcisi de o noktada API'ye karşı yeniden çalışır hâle gelecek

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **767 geçti**, 1 atlandı |

## 22 Ağustos 2026 — Aşama 01, Grup 5: kategori setleri ve işletme kimliği

Onboarding'in tek sorusu eklendi ve türetme zincirinin son halkası doldu.

- **`UserProfile` tablosu**, kullanıcı başına tek satır, tek alan:
  `HasBusiness`. Cihazda değil sunucuda duruyor — uygulamayı silip yeniden kuran
  ya da ikinci cihazdan giren kullanıcı işletme sahibi olmayı kaybetmemeli.
  Profili olmayan kullanıcı "işletmesi yok" sayılıyor; bu bir varsayım değil,
  sorunun sorulmadığı hâlin doğru cevabı
- **Cevap kayıt akışında yazılıyor** (`POST /api/v1/auth/register` gövdesinde
  `hasBusiness`). Sonraya bırakılamazdı: varsayılan set ilk kategori okumasında
  kuruluyor ve yalnız hiç kategorisi olmayan kullanıcıya bir kez uygulanıyor
- **İki set:** kişisel setin tamamı `Şahsi` (devralınan liste olduğu gibi kaldı);
  işletme seti 22 işletme kalemi (`İşletme`) **ve** patronun gündelik hayatı için
  12 şahsi kalem taşıyor. İkisi birden gerekiyor, çünkü esnafın market alışverişi
  de aynı uygulamaya giriyor
- **Türetme zinciri artık pratikte çözülüyor:** her kategori kapsam taşıdığı için
  istemci kapsam göndermeden kayıt oluşturabiliyor. Bir API testi bunu iki set
  için de kanıtlıyor. Reddi görebilmek için artık kullanıcının kendi açtığı,
  kapsamsız bir kategori kurmak gerekiyor
- **`GET`/`PUT /api/v1/profile`** cevabı okuyup değiştiriyor. Değiştirmek yalnız
  arayüzü etkiliyor; kategoriler olduğu gibi kalıyor — o noktada liste artık
  kullanıcınındır ve sildiği bir kategoriyi geri getirmek silme eylemini
  anlamsız kılardı
- **Üçüncü migration: `AddUserProfile`.** Yeni tablo olduğu için backfill sorusu
  yok

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **800 geçti**, 1 atlandı |
| Flutter analyze / test | No issues found, **637 geçti** |

## 22 Ağustos 2026 — Aşama 01, Grup 6: kapsama duyarlı okuma modelleri

Kapsam artık raporları ve listeleri bölüyor; parayı bölmüyor.

- **Filtre alan okumalar:** aylık rapor (gelir, gider, kategori dağılımı),
  gelişmiş rapor (dönem karşılaştırması, nakit akışı eğilimi, bütçe sapması),
  birleşik feed ve planlanan görünüm. Hepsinde isteğe bağlı `scope` query
  parametresi; tanınmayan değer `*.invalid_scope` ile reddediliyor
- **Filtre almayan okumalar ve gerekçesi:** hesap bakiyeleri, net varlık, kart
  dağılımı ve yaklaşan ödemeler. Kasadaki para ve karta olan borç tek havuzdur;
  anahtarın konumuna göre değişseydi "ne kadar param var" sorusunun aynı anda
  iki farklı doğru cevabı olurdu
- **Filtre her yerde SQL'e iniyor.** Feed'de tek `UNION ALL` sorgusunun içinde,
  planlanan görünümde her kaynağın kendi sorgusunda; bellekte eleme yok
- **Kapsam filtresi kapsamsız satırları da eliyor.** Transfer, kart ödemesi ve
  kart ekstresi kapsam taşımaz (ADR 0002, ADR 0003); ikisini birden iki tarafta
  göstermek, kullanıcı tarafları karşılaştırdığında aynı para hareketini iki kez
  saydırırdı. Bu yüzden iki tarafın toplamı filtresiz toplamdan küçük ve olması
  gereken bu
- **Bütçe sapması da kategori + kapsam çiftiyle toplanıyor.** Kural artık üç
  yerde birden yazılı: domain hesabı, bütçe listesi ve gelişmiş rapor
- **Bölünmezlik testle korunuyor.** Gerçek SQL üzerinde çalışan yeni test aynı
  ayı üç kapsamda okuyor: gelir/gider bölünüyor ve iki taraf toplamı veriyor,
  hesap bakiyesi ile net varlık üç okumada da aynı kalıyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **801 geçti**, 1 atlandı |

## 23 Ağustos 2026 — Aşama 01, Grup 7: Flutter kapsam anahtarı ve formlar

Kapsam boyutu artık kullanıcının gördüğü yerde. Backend Grup 6'da hazırdı;
istemci bu turda hem gönderiyor hem gösteriyor.

- **Tek denetim, tek anahtar** (`ScopeController`): anahtarın konumu ve
  onboarding cevabı. Sekme başına ayrı filtre yok (ADR 0013). Anahtar Özet
  ekranında, kaydırılan gövdenin **dışında** duruyor — yükleme, hata ve boş
  durumda da yerinde; bölünen diğer ekranlar (feed, planlanan) onu uygulayıp
  başlıklarında yazıyor, denetimi kopyalamıyor
- **Seçim oturumlar arası hatırlanıyor**, cihaz deposunda (`ScopePreferences`,
  `flutter_secure_storage` — `ReceiptPreferences` ile aynı gerekçe: iki değer
  için ikinci bir depolama paketi eklemek bakımı olan yeni bir bağımlılıktı).
  Onboarding cevabının **kaynağı sunucu**; cihazdaki kopya yalnız profil
  okunamadığında boyutun sessizce kaybolmasını engelliyor. "Hayır" varsaymak,
  işletme sahibinin boyutunu bir ağ hatasına kurban ederdi
- **Çıkışta seçim ve cevap unutuluyor**; aynı cihazdan giren ikinci kullanıcı
  birincisinin anahtar konumunu devralmıyor. Bağlama işi kompozisyon kökünde,
  çünkü kimliği ve kapsamı birlikte tanıması gereken tek yer orası
- **Bölünmeyen bölümler bölünmediklerini yazıyor.** Net varlık ve hesap
  bakiyelerinin altında toplam gösterdiklerini söyleyen bir satır duruyor;
  sessizce aynı kalan bir sayı filtrelenmiş sanılır ve kullanıcı iki tarafı
  toplamaya çalışırdı
- **Formdaki çip zincirin önizlemesi.** Kararın sahibi sunucu
  (`TransactionScopeResolution`); form aynı sırayı yalnız **gösterebilmek** için
  uyguluyor ve gösterdiği değeri açıkça gönderiyor — ekranda okunan ile yazılan
  aynı olmalı. Alanın altında değerin nereden geldiği yazılı. Zincir
  çözülemezse istek sunucuya gitmeden duruyor; sunucu da reddederdi
  (`*.scope_unresolved`), ama hata kullanıcının düzeltebileceği yerde görünmeli
- **`FinancialDataChanges`'e bağlanmadı** ve gerekçesi belgeye yazıldı: o sinyal
  "veri değişti" der, anahtar veriyi değiştirmez. Oraya bağlansaydı her kapsam
  dokunuşu kapsamdan etkilenmeyen ekranları (hesaplar, kartlar) da boşuna
  yükletirdi
- **Grup 5'in Flutter yüzü de bu checkpoint'e girdi:** kayıt formundaki tek soru
  (`hasBusiness` artık istekle gidiyor, varsayılan kapalı) ve `Diğer`
  menüsündeki `İşletmem var` anahtarı (`PUT /api/v1/profile`). İkisi olmadan
  hiçbir Flutter kullanıcısı işletme sahibi olamıyor ve kapsam boyutunu hiç
  göremiyordu
- **Bilinen boşluk:** `ScopePreferences`'ın kendisinin doğrudan testi yok
  (`ReceiptPreferences` ile aynı gerekçe — platform kanalı ister); sözleşmesi
  (`ScopeStore`) sahte uygulamayla testli

| Kontrol | Sonuç |
|---|---|
| Flutter analyze | No issues found |
| Flutter format (`--set-exit-if-changed lib test`) | Temiz |
| Flutter test | **692 geçti** (637 → +55) |
| Flutter debug APK | Derlendi (`app-debug.apk`) |
| Backend | Bu turda değişmedi |

## 23 Ağustos 2026 — Aşama 01, Grup 8: özet ekranının hero metriği

Kapsam varken tek bir "net" hangi neti sorduğunu söylemiyordu; artık ay iki
tarafıyla birlikte okunuyor.

- **Aylık rapor filtresiz okunduğunda kırılım da döndürüyor**
  (`scopeBreakdown`): işletme ve şahsi tarafın gelir/gider/net tabloları ayrı
  ayrı. İki tarafın toplamı raporun kendi toplamına eşit — gelir/gider üreten
  her kayıt tam olarak bir kapsam taşıyor ve üçüncü bir kova yok
- **Kırılım sunucudan hazır geliyor** çünkü istemci finansal toplamı ikinci kez
  hesaplamaz. "İşletme neti" ile "şahsi çekim" bir çıkarma değil, ayrı ayrı
  toplanmış iki tablo; istemci çıkarsaydı ekrandaki sayı sunucununkiyle
  tutmayabilirdi
- **Sorgu sayısı değişmedi.** Toplamlar `SUM` yerine kapsama göre `GROUP BY`
  ile okunuyor; en fazla iki satır dönüyor ve toplam onların toplamı. Kırılımı
  ikinci bir tur sorguyla almak özet ekranının ilk isteğini iki katına
  çıkarırdı. Bounded query-count ölçüsü olduğu gibi geçiyor
- **Filtreli okuma kırılım taşımıyor:** rapor zaten tek tarafı anlatıyor,
  kırılım göndermek dışlanan tarafı sıfır gösterip "o tarafta hiç hareket yok"
  dedirtirdi
- **Ekranda üç sayı:** `İşletme neti` (hero), `Şahsi çekim` ve `Bu ayın neti`.
  Kapsam boyutu görünmeyen kullanıcıda ekran bugünkü hâlini koruyor; bir taraf
  seçiliyken hero o tarafın netini adıyla gösteriyor
- **Aşama belgesinden bilerek sapıldı:** üçüncü sayı `kasa değişimi` diye
  planlanmıştı, `Bu ayın neti` oldu. Rapor gideri harcandığı gün tanıyor — kart
  harcaması aynı ay gider yazılır, borcu bir sonraki ay ödenir — dolayısıyla ilk
  iki sayının toplamı kasadaki değişim değil. "Kasa değişimi" demek, ekrandaki
  üç sayıyı toplayan kullanıcıya kasada olmayan bir para söylemek olurdu.
  Kasanın gerçek hâli aynı ekranda `Hesap bakiyeleri` ve `Varlık durumu`
  bölümlerinde zaten duruyor
- **Şahsi tarafın adı sayının yönüne göre değişiyor** (`Şahsi çekim` /
  `Şahsi net`): çoğu ayda şahsi taraf yalnız harcamadır, ama şahsi bir gelir
  girilen ayda "çekim" demek artı bir sayıyı eksi gibi okuturdu
- **"Kâr" kelimesi ekranda hiç geçmiyor** ve bunu bir test koruyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **803 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze / format | No issues found, temiz |
| Flutter test | **700 geçti** (692 → +8) |
| Flutter debug APK | Derlendi |

## 23 Ağustos 2026 — Aşama 01, Grup 9: CSV kapsam kolonu ve yedek tatbikatı

Aşamanın son grubu. Kapsam artık kullanıcının dosyalarında da yazılı.

- **İşlem CSV'si `scope` kolonu taşıyor**, `type`'ın hemen yanında: ikisi de
  kaydın ne olduğunu söyleyen boyutlar ve satırı okuyan kişi "expense,
  personal" diye yan yana okuyor. Kapsamsız bir dosya, kullanıcının kendi
  arşivinde aynı hesaptan aynı kategoriye yazılmış iki kaydı bir daha ayıramaz
  hâle getirirdi
- **İçe aktarma kolonu okumuyor** ve okumayacak: o ayrıştırıcı banka ekstresi
  içindir, kapsamı zincirden çözer. Dışa aktarma okumak ve arşivlemek için;
  veri taşımanın tek yolu yedek/geri yükleme
- **Yolda sessiz bir kırılma bulundu ve kapatıldı.** İstemci, kendi dışa
  aktarımını içe aktarma ekranında **tam başlık dizesiyle** tanıyordu. Kapsam
  kolonu o dizeyi değiştirdiği anda koruma sessizce kalkacak, kullanıcı kendi
  dosyasını banka importer'ına verip her hareketi ikinci kez yazdırabilecekti.
  Tanıma artık yalnız bu dosyada bulunan kolonlara bakıyor
  (`transactionDate` + `isCancelled`; hiçbir banka ekstresinde `isCancelled`
  yoktur) ve bir test hem eski hem de gelecekte bir kolon daha eklenmiş
  başlığın tanındığını koruyor
- **Runbook** v6'ya göre zaten yazılmıştı (Grup 2); bu turda tatbikata kapsam
  doğrulama adımı eklendi, hata tablosundaki eskimiş `v1` satırı v2–v5
  reddine güncellendi, CSV ile yedeğin aynı şey olmadığı açıkça yazıldı.
  Geri yüklemede kapsamın korunduğunu gerçek SQL üzerinde çalışan
  `Backup_ValidatesAndRestoresCompleteSyntheticGraphToEmptyOwner` kanıtlıyor
- Eskimiş bir kod yorumu da düzeltildi: doğrulama özeti "v2 hâlâ kabul
  ediliyor" diyordu; yalnız v6 okunuyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dahil) | **804 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze / format | No issues found, temiz |
| Flutter test | **701 geçti** |
| Flutter debug APK | Derlendi |

## 23 Ağustos 2026 — Aşama 01 kabul turu (Pixel 8 + gerçek API + gerçek SQL)

Aşamanın son çıkış koşulu karşılandı. Kurulum: SQL container healthy, API
güncel derlemeyle `http://localhost:5284` (live/ready 200), Pixel 8 emulator,
debug APK `API_BASE_URL=http://10.0.2.2:5284`.

**Otomatik senaryolar** — `integration_test/stage01_scope_acceptance_test.dart`,
cihazda çalıştı, üçü de geçti. Her kayıt kapsam **göndermeden** oluşturuldu;
kapsamı sunucu türetti:

- **Kasap:** dükkân kasası, 600 satış + 200 mal alımı → işletme neti 400.
  Ardından aynı kasadan 300 market alışverişi → **işletme neti kıpırdamadı**,
  şahsi taraf −300, ayın neti 100. Bakiye üç kapsam okumasında da 1.100.
  Filtreli okuma kırılım taşımadı; feed işletme tarafında iki, şahsi tarafta
  bir satır gösterdi
- **Manav + terzi:** işletme kategorisine yazılan bir gider çipten şahsi
  seçilerek istisna edildi (işletme neti 1.000, şahsi −150). İkinci esnaf
  kaydolduğunda kendi tablosu boş geldi ve birincinin kaydını **hiçbir
  kapsamda** görmedi
- **Ev hâli:** "işletmem yok" diyen kullanıcıda kayıt sessizce şahsi tarafa
  yazıldı; işletme tarafı 0 kaldı

**Elle gezilen ekran** — aynı verinin uygulamadaki hâli:

- Özet başlığının altında `Hepsi · İşletme · Şahsi` anahtarı; hero
  `İşletme neti ₺4.250,00`, altında `Şahsi çekim −₺1.275,50` ve
  `Bu ayın neti ₺2.974,50`. Üç sayı birbirini tutuyor
- Anahtar `İşletme`ye alınınca gider 3.425,50 → 2.150,00 düştü, kategori
  listesi 2 kategoriden 1'e indi, hero `Yalnız işletme tarafı` yazdı
- `Varlık durumu` ve `Hesap bakiyeleri` filtre açıkken **toplam gösterdiklerini
  yazdı** ve değişmedi (₺3.974,50)
- `İşlemler` başlığı `İşlemler · İşletme` oldu ve market satırı listeden düştü
- Gider formunda kategori seçilince kapsam çipi `İşletme` olarak doldu ve
  altında "Kategorinin varsayılanından geldi" yazdı; `Şahsi`ye dokununca
  "Bu kayıt için siz seçtiniz." oldu

**Kabul turunun iki bulgusu:**

- **Hesap, kart ve kategorinin varsayılan kapsamı uygulamadan ayarlanamıyor.**
  Alan API'de var, Flutter istemcisi ne gönderiyor ne gösteriyor. Zincirin orta
  halkası bu yüzden yalnız API'den kurulabiliyor; uygulamada kapsam kategoriden
  çözülüyor ve istisna çiple düzeltiliyor. Aşamanın vaadi bu hâliyle
  karşılanıyor — ama tek hesabına "dükkân kasası" deyip her kaydı oradan
  işletme saymak isteyen esnaf bunu yapamıyor. `docs/backlog.md` 5. madde
- **Pixel 8 AVD'de uygulama Impeller ile ilk kareyi çizmiyor**; süreç yaşıyor,
  Dart VM açılıyor, ekran Flutter logosunda kalıyor.
  `--ez enable-impeller false` ile açılıyor. Uygulama hatası değil, emulator
  grafik yığını; `documentation/local-setup-and-acceptance.md` sorun giderme
  bölümüne yazıldı

Turda 08:35'ten beri çalışan **eski derlemeli** bir API örneği bulundu ve
durduruldu; kabul güncel derlemeye karşı yürütüldü ve API o hâliyle çalışır
bırakıldı.

## 23 Ağustos 2026 — Aşama 01 kapandı, Aşama 02 açıldı

- **Aşama 01 kullanıcı onayıyla kapandı.** Sekiz çıkış koşulunun sekizi
  karşılandı; belge `docs/archive/stages/` altına taşındı ve tamamlanma kaydı
  (commit zinciri, son kontroller, sapmalar, çıkan açık işler) oraya yazıldı.
  `stages/README.md` ve `PROJECT-ROADMAP.md` durumu `Tamamlandı`
- **Aşama 02 açıldı** ve zincir belgelerinde **Aktif** olarak işaretlendi
- **ADR 0014 yazıldı: "ekonomik olay tanır, ödeme taşır."** Aşamanın karar
  kapısı ve Grup 1'i. Kural yeni değil — kart harcaması, kart ödemesi, transfer
  ve borç açılışı bugün zaten böyle davranıyor; ADR bunu ilk kez yazıya geçirdi
  ve cari hesaba nasıl uygulanacağını sabitledi (borçlandırma tanır, tahsilat
  taşır; cari bakiye projection'dır; tahsilat kategori ve kapsam taşımaz).
  Dört alternatif gerekçesiyle reddedildi. Fiş okumanın "faturayı ödemedim"
  yolundaki tutarsızlık kayda geçti ve Aşama 03'e devredildi
- **ADR'nin durumu `Öneri`**: kabul edilmeden Aşama 02'nin koduna
  başlanmıyor (`AGENTS.md`, "Kalite ve aşama geçişi"). Kod tarafında bu turda
  hiçbir değişiklik yapılmadı

## 23 Ağustos 2026 — Aşama 02, Grup 1 ve 2: karar kapısı ve cari domaini

- **ADR 0014 kullanıcı onayıyla kabul edildi.** Bir kayıt ya ekonomik olayı
  tanır ya ödemeyi taşır; ikisini birden yapması ancak olay ile ödemenin aynı
  ana düşmesidir. Kural yeni değil — kart, transfer ve borç modelleri bugün
  zaten böyle davranıyor — ama ilk kez yazıya geçti ve cari hesaba nasıl
  uygulanacağını sabitledi
- **Domain katmanı yazıldı:** `Counterparty`, `CounterpartyCharge`,
  `CounterpartyPayment`, `CounterpartyBalance`. 17 yeni domain testi
- **Grup yalnız domaine dokundu.** Tipler EF modeline girmediği için migration
  üretilmedi ve `HasPendingModelChanges` temiz kaldı; kalıcılık Grup 3'ün işi.
  Aşama 01'de migration'ın domain checkpoint'ine girme sebebi (model/şema
  örtüşme testi) burada oluşmadı
- **Yön için ikinci bir enum açılmadı:** `DebtDirection` yeniden kullanılıyor,
  çünkü sorduğu soru aynı — yükümlülük kimin üzerinde
- **Yön kategorinin türünü belirliyor**: alacak doğuran borçlandırma gelir
  kategorisi, borç doğuran gider kategorisi ister; tutmayan istek reddediliyor
- **Tahsilat ne kategori ne kapsam taşıyor** ve bunu bir test koruyor: kategori
  "ne satıldı" sorusunu cevaplar ve o soru borçlandırmada sorulmuştur; kapsam
  gelir/gider raporunu böler, tahsilat o rapora hiç girmez
- **Aşama belgesinin yazmadığı iki karar verildi ve gerekçesiyle yazıldı:**
  pasif karşı tarafa yeni borçlandırma yazılamaz ama **tahsilat yazılabilir**
  (aksi hâlde açık bakiye kapatılamaz hâle gelirdi); **fazla tahsilat
  kırpılmaz**, taraf eksiye düşer (kırpmak kullanıcının parasını ekranda yok
  ederdi — aynı hatanın kart tarafındaki hâli backlog 1. maddede)

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dâhil) | **821 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Migration | Üretilmedi ve gerekmedi; `HasPendingModelChanges` temiz |
| Flutter | Bu turda değişmedi |

## 24 Ağustos 2026 — Aşama 02, Grup 8: yedek v7 ve cari defterin dışa aktarımı

- **Yedek şeması v7.** Karşı tarafın kendisi (ad, not, aktiflik) ve cari
  defterin iki hareket türü yedeğe girdi. Geri yükleme gerçek SQL üzerinde
  kayıpsız doğrulandı: üç karşı taraf, iki borçlandırma, iki tahsilat
- **Sözleşme karşı tarafı artık adla değil kimlikle gösteriyor.** v6 adı
  taşıyor ve geri yüklerken addan yeniden kuruyordu; karşı taraf kendi
  kaydıyla dosyaya girdiğine göre ad yedeğin içinde tek yerde durmalı
- **v6 yükseltilmiyor, `restore.unsupported_version` ile reddediliyor.** O
  dosyada cari defter hiç yok; karşı tarafı bakiyesiz kurmak kullanıcının
  alacağını sessizce sıfırlamak olurdu. v2–v5 için kapsam boyutunda verilen
  kararın aynısı (ADR 0013)
- **Pasifleştirme geri yüklemede hareketlerden sonra uygulanıyor** (hesap ve
  kategorilerdeki sıranın aynısı). Ters sıra, pasif bir müşterinin geçmişini
  geri yüklenemez yapardı — borçlandırma yalnız aktif karşı tarafa yazılabilir
- **Kullanıcı kararı: işlem CSV'sine karşı taraf kolonu eklenmedi; cari defter
  kendi dosyasını aldı** (`GET /api/v1/exports/counterparty-ledger.csv`).
  İşlem CSV'si `BudgetTransaction` tablosunun dökümüdür ve cari hareket orada
  hiç bulunmaz — kolon her satırda boş kalırdı. İki kayıt türünün alan listesi
  dosyada da bilerek farklı: borçlandırma kategori ve kapsam taşır, hesap
  kolonu boştur; tahsilat hesap taşır, kategori ve kapsam kolonları boştur
  (ADR 0014). Cari CSV'si de işlem CSV'si gibi **geri yüklenemez**
- Cari dosyası Veri araçları > Yedek sekmesine kendi kartıyla girdi ve kendi
  özet cümlesini kuruyor (`n cari hareket satırı`)

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (SQL dâhil) | **838 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze | No issues found |
| Flutter format | Temiz |
| Flutter test | **715 geçti** |
| Flutter debug APK | Derlendi |
| Migration | Üretilmedi ve gerekmedi; şema değişmedi (yedek dosya biçimi değişti) |

## 24 Ağustos 2026 — Aşama 02 kabul turu (Pixel 8 + gerçek API + gerçek SQL)

Tur **Aşama 01'in kabul hesabıyla** yürütüldü (`stage01.kabul@example.test`):
dükkânın zaten bir geçmişi var ve aşamanın vaadi o geçmişin üstüne geldi. Her
ölçü fark olarak alındı.

**Otomatik senaryolar** — `integration_test/stage02_counterparty_acceptance_test.dart`,
cihazda çalıştı, ikisi de geçti:

- **Veresiye defteri:** üç veresiye satış (400+350+250) → gelir **1.000 arttı**,
  kasa kıpırdamadı, net varlık 1.000 arttı ve alacak bir kez sayıldı. İki kısmi
  tahsilat (400+200) → kasa 600 oldu, **gelir ikinci kez sayılmadı**, net varlık
  değişmedi (alacak kasaya taşındı). Cari bakiye 400. Feed beş boyutu doldurdu:
  üç `income`/`counterparty`, iki `neutral`; hepsi `canCancel`. 200'lük tahsilat
  iptal edilince kasa 400'e döndü ve açık bakiye 600 olarak yeniden doğdu
- **Yedek v7 ve cari dosyası:** yedek `schemaVersion 7` ve üç yeni koleksiyonu
  dolu taşıdı; her sözleşme karşı tarafını **kimlikle** gösterdi (`counterpartyName`
  alanı dosyada yok). Cari CSV'sinde borçlandırma satırı kategori ve kapsam
  taşıdı, hesap kolonu boştu; tahsilat satırı hesap taşıdı, kategori ve kapsam
  kolonları boştu

**Elle gezilen ekran:**

- `Diğer > Cari hesap` kapısı `Borç ve alacaklar`ın hemen üstünde; liste iki
  karşı tarafı net işaretiyle gösterdi (`₺600,00`, `₺400,00`, ikisi de
  "Sizden alacağı yok, size borçlu")
- Ayrıntı: `Size borcu ₺600,00 / Sizin borcunuz ₺0,00 / Net ₺600,00`, dört
  eylem (veresiye satış, vadeli alım, tahsilat, ödeme), hareket geçmişinde üç
  satış, iptal edilmiş tahsilat `İptal edildi` rozetiyle soluk, geçerli
  tahsilat nötr mavi tonda (ADR 0008)
- `Veri araçları > Yedek`: yeni `Cari hareket CSV dosyası` kartı; önizleme
  `10 cari hareket satırı` dedi ve başlık satırı `counterpartyName` taşıdı.
  Uygulama yedeğinin özeti `Yedek sürümü: 7 — 53 kayıt`

**Kabul turunun bulgusu — düzeltildi:**

- **Karşı taraf ayrıntısı gerçek API'de hiç açılmıyordu.** Ekran sözleşmeleri
  `GET /api/v1/debts` ile okuyor ama **zorunlu** `asOfDate` parametresini
  göndermiyordu; sunucu `request.invalid_format` döndürüyor, ayrıntı ekranı
  hataya düşüyordu. Widget testleri sahte repository kullandığı için bunu
  göremezdi. Repository artık borç ekranıyla aynı parametreyi gönderiyor ve
  bir test tarihin gittiğini sabitliyor. Kalan tutar kalıcı bir kolon değil,
  bir tarihe göre hesaplanan projection'dır; tarihsiz sorulamaz

**Kabul turunun verisi sentetiktir** ve kabul hesabının defterinde kalmıştır:
iki `Kabul Manavı …` karşı tarafı, altı borçlandırma ve dört tahsilat. Aynı
tur yeniden çalıştırılabilir; her tur kendi kasasını ve kendi karşı tarafını
açar.

## Açık kararlar ve riskler

- ~~Eski repoda 16 commit push edilmemiş~~ — kapandı: commit'ler
  `origin/feat/mobile-data-tools-ux`'e gönderildi ve `main`'e merge edildi
  (`1af11f9`). Eski repo artık eksiksiz ve arşiv olarak tam
- Devralınan açık işler `docs/backlog.md` içinde (fazla ödenmiş kart bakiyesi,
  bütçe ekranı, fiş akışının cihaz kabul turu, fiş veri sınırı kararı)
- Repo sahipliği (kişisel hesap mı organizasyon mu) ve lisans kararı
  verilmedi; ürün ticari olarak sunulacaksa ikisi de netleşmeli

## Sıradaki tek küçük görev

- **Aşama 03, Grup 3: tekrarlayan planda bitiş sınırı.** Plan isteğe bağlı
  bitiş tarihi ve/veya tekrar sayısı alacak; önce dolan sınırdan sonra yeni
  occurrence üretmeyecek ve geçmişi silmeden pasifleşecek.

## 24 Ağustos 2026 — Aşama 02 kapandı, Aşama 03 açıldı

- **Aşama 02 kullanıcı onayıyla kapandı.** Dokuz çıkış koşulunun dokuzu
  karşılandı; belge `docs/archive/stages/` altına taşındı ve tamamlanma kaydı
  (commit zinciri, son kontroller, belgede yazmayan kararlar, kabul turunun
  bulgusu) oraya yazıldı. `stages/README.md` ve `PROJECT-ROADMAP.md` durumu
  `Tamamlandı`
- **Aşama 03 — Yükümlülük ve vade açıldı** ve zincir belgelerinde **Aktif**
  olarak işaretlendi. ADR kapısı yok; kod Grup 1'den başlayabilir
- Bu turda kod değişmedi; yalnız karar belgeleri güncellendi

## 24 Ağustos 2026 — Aşama 03, Grup 1: yükümlülük domaini

- **`Obligation` ekonomik olayı tanıyor:** ödenecek yön gider, tahsil edilecek
  yön gelir; kategori ve kapsam zorunlu, karşı taraf isteğe bağlı. Hesap alanı
  taşımadığı için kayıt anında kasa değişmiyor
- **`ObligationSettlement` nakdi taşıyor:** kategori ve kapsam taşımıyor;
  tahsilat hesabı artırıyor, ödeme azaltıyor ve aynı gelir/gideri ikinci kez
  yazmıyor
- **Kapanış idempotent:** ikinci `Settle` çağrısı ilk settlement'ı döndürüyor.
  Veritabanı tekilliği ve eşzamanlı istek kanıtı kalıcılığın ekleneceği Grup
  2'de tamamlanacak
- **Gecikme saklanmıyor:** durum `Open`, `Settled`, `Cancelled`; gecikme
  `IsOverdueOn(asOfDate)` ile vade ve sorgu tarihinden türetiliyor
- Belge tarihi sistemin UTC kayıt tarihinden ileri olamıyor. Vade geçmişte veya
  gelecekte olabiliyor fakat belge tarihinden önce olamıyor
- İptal, yükümlülükle varsa settlement'ı birlikte UTC damgasıyla iptal ediyor;
  fiziksel silme yok
- Grup yalnız Domain'e dokundu; EF modeline tip eklenmedi, migration/API/Flutter
  değişikliği oluşmadı

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend test (gerçek SQL dâhil) | **851 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **235 geçti**; `ObligationTests` 13 yeni vaka |
| Migration | Üretilmedi ve gerekmedi; EF modeli değişmedi |
| Flutter | Bu turda değişmedi |

## 24 Ağustos 2026 — Aşama 04, Grup 2: gün sonu kasa sayımı (Domain)

- `CashCount` bir para hareketi değil **gözlemdir**: hesap bakiyesine dokunmaz,
  gelir/gider yazmaz, tek başına hiçbir rapora girmez
- **Beklenen tutar saklanmıyor.** Fark `DifferenceFrom(expectedBalance)` ile
  okunduğu anda türetiliyor; aynı sayım, bir hareket sonradan iptal edilince
  farklı bir fark veriyor ve testi bunu kanıtlıyor. Saklansaydı kayıt doğduğu
  andan itibaren eskiyen ikinci bir gerçek olurdu
- **Onaysız hiçbir finansal kayıt üretilmiyor** (grubun çıkış ölçütü). Düzeltme
  ikinci ve ayrı bir eylem: `RecordAdjustment` tek `BudgetTransaction` kimliği
  bağlıyor, idempotent ve bir sayımın ikinci düzeltmesi reddediliyor
- Aynı gün + aynı hesabın ikinci sayımı öncekini **iptal ediyor, üzerine
  yazmıyor**; eski gözlem tutarıyla birlikte kalıyor
- Sayılan tutar `Money` değil: sıfır meşru (boş kasa da sayılır), negatif değil.
  Yalnız kullanıcının kendi, aktif ve **nakit** hesabı sayılabiliyor
- Bu grup yalnız Domain'e dokundu; `CashCount` ve Grup 3'ün `PosSettlement`
  tipi için EF modeli, migration ve yazma yolu Grup 4'e alındı — ikisini SQL'den
  okuması gereken ilk grup odur ve kalıcılığı bölmek migration zincirini
  gereksiz uzatırdı. Bu kapsam netleştirmesi aşama belgesine yazıldı

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **887 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **255 geçti** (17'si yeni `CashCountTests`) |

Flutter tarafına dokunulmadı; bu grup yalnız Domain katmanıdır.

## 24 Ağustos 2026 — Aşama 04, Grup 3: POS tahsilatı (Domain)

- `PosSettlement` ADR 0014'ün ayrımını tek kaydın **iki anına** koyuyor:
  tahsilat günü gelir **brüt** tutar kadar tanınıyor ve komisyon ayrı gider
  yazılıyor, hesap bakiyesi kıpırdamıyor; geçiş günü hesap **net** tutar kadar
  artıyor ve hiçbir gelir/gider yeniden yazılmıyor. Grubun çıkış ölçütü tek
  testte duruyor: gelir bir kez, komisyon bir kez
- **Komisyon brüte eklenmiyor ve ondan düşülerek gizlenmiyor.** Net tutarı gelir
  yazmak, kesilen faturayı küçültür ve komisyonu görünmez bir gidere çevirirdi.
  Komisyon `Money` değil: sıfır meşru, her kartta komisyon kesilmez
- **Oran saklanmıyor, paradan çözülüyor** (ADR 0009'un aynı kararı). Oran ve
  tutar ayrı ayrı saklansaydı ikisi ayrı ayrı düzenlenip sessizce çelişirdi
- `PosTransitBalance` yoldaki parayı veriyor: bekleyenlerin **net** toplamı,
  kalıcı kolon değil ve hesap türü değil (ADR 0015). İptal edilmiş ve geçmiş
  tahsilatlar sayılmıyor — aynı parayı iki yerde göstermek olurdu
- Para banka hesabına geçiyor; kasa kart ödemesi alamıyor. `MarkTransferred`
  idempotent ve ikinci bir günle işaretleme reddediliyor

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **904 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Domain testleri | **272 geçti** (17'si yeni `PosSettlementTests`) |

Flutter tarafına dokunulmadı; bu grup da yalnız Domain katmanıdır.

## 24 Ağustos 2026 — Aşama 04, Grup 4 (1/2): kalıcılık, bakiye ve net varlık

- `AddCashCountsAndPosSettlements` iki tabloyu kurdu. İkisi de **boş doğuyor**;
  zorunlu kolonlar backfill istemiyor, kalıcı DEFAULT bırakılmıyor ve mevcut
  hiçbir tabloya kolon eklenmiyor. Yükseltme `BusinessFinance` ve
  `BusinessFinanceApiSqlTests` veritabanlarına uygulandı
- **Türetilen hiçbir şey kolon değil**: net tutar, komisyon oranı, "yolda mı",
  beklenen bakiye ve fark şemada yok. Migration testi bu yokluğu açıkça sınıyor
- `CashCounts` üzerindeki filtreli tekil indeks (`IsCancelled = 0`) bir gün ve
  bir kasa için tek açık sayım bırakıyor; gerçek SQL testinde ikinci açık sayım
  reddediliyor, iptal edip yenisini yazma yolu çalışıyor
- Geçmiş POS tahsilatı hesap bakiyesine **net** giriyor, yoldaki hiç girmiyor.
  Net varlık `moneyInTransit` taşıyor ve **iki sayının farkı tam olarak yoldaki
  tutar** — grubun ölçütü gerçek SQL testinde kapıya bağlandı
- Özet ekranına `Yolda` satırı eklendi (alt başlık `POS tahsilatı`), yolda para
  yokken çizilmiyor. Arayüz ADR 0015'in kelimesini kullanıyor, `bloke` demiyor
- Gelişmiş raporun sabit SQL komut kapısı 61'den **63**'e çıktı: biri geçmiş
  tahsilatı hesap bakiyesine koyan, biri yoldakini toplayan iki gruplanmış
  sorgu. İkisi de tahsilat adediyle büyümüyor
- **Kalan:** POS'un gelir/gider raporunda tanınması (brüt gelir + komisyon
  gideri) ve yazma uçları. Bugün tablo boş olduğu için gözlenebilir bir
  tutarsızlık yok, ama yazma açılmadan önce kapatılacak

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **907 geçti**, 1 atlandı (`GeminiLiveContractTests`) |
| Flutter analyze | No issues found |
| Flutter format | 227 dosya, değişiklik gerektirmedi |
| Flutter test | **735 geçti** |
| Android debug build | `app-debug.apk` üretildi |

Not: migration'ı üretebilmek için yerelde çalışan `BusinessFinance.Api` süreci
kullanıcı onayıyla durduruldu; yeniden başlatılmadı.

## 24 Ağustos 2026 — Aşama 04, Grup 4 (2/2): POS satışının raporda tanınması

- POS satışı **tahsil edildiği gün** rapora giriyor, geçtiği gün değil: brüt
  tutar gelire, komisyon ayrı gider olarak dönem toplamlarına, kategori
  dağılımına ve nakit akışı eğilimine katılıyor. Geçiş günü rapora hiçbir şey
  eklemiyor — eklerse aynı satış iki kez sayılırdı
- **Komisyon bütçeyi tüketiyor**: kendi kategorisi olan, o gün tanınmış gerçek
  bir gider. Brüt tutar tüketmiyor çünkü o bir gelir ve bütçe gider bütçesi
- Gerçek SQL testi tanımayı kapıya bağladı: iki açık tahsilatın brütü (1500)
  gelire, komisyonları (30) gidere giriyor; geçmiş olanın geçiş günü hiçbir şey
  eklemiyor ve iptal edilen hiç görünmüyor
- Sabit SQL komut kapısı 63'ten **69**'a çıktı: iki dönemin gelir ve komisyon
  toplamları, trend ve bütçe sapması. Altı gruplanmış okuma, hiçbiri tahsilat
  adediyle büyümüyor
- Yazma uçları bu grupta açılmadı; tükettikleri ekranlarla birlikte **Grup 7**'ye
  alındı (Aşama 03'te yükümlülük uçlarında verilen kararın aynısı)

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **907 geçti**, 1 atlandı (`GeminiLiveContractTests`) |

Flutter tarafına dokunulmadı; bu adım yalnız sunucu raporlarıdır.

## 24 Ağustos 2026 — Aşama 04, Grup 7 (1/2): kasa ve POS yazma uçları

- **Sıra kararı**: Grup 7, Grup 5'ten önce yapılıyor. Grup 5'in üçüncü sekmesi
  `Kasa` ekranına işaret ediyor; o ekran ve onu besleyen uç burada doğmadan
  sekme boş bir yere açılır ve grubun kendi ölçütü ilk günden ihlal olurdu
- Yedi uç açıldı: sayımın günü/geçmişi/oluşturulması/farkın onaylanması ve
  tahsilatın listesi/oluşturulması/geçişin işaretlenmesi
- **Sayım bir gözlemdir**: yazıldığında ne bakiye ne rapor kıpırdıyor. Fark
  ancak açık onayla **tek** gelir/gider kaydına dönüşüyor ve o kayıt kapsamını
  sayımdan alıyor — kategoriden yeniden türetilseydi aynı sayım farklı günlerde
  farklı kapsam üretebilirdi
- Beklenen bakiye ve fark **yalnız günün açık sayımında** dönüyor: geçmiş bir
  günün farkını bugünkü bakiyeye karşı hesaplamak, aradaki bütün hareketleri o
  günün farkına yazmak olurdu
- Aynı gün ikinci sayım öncekini iptal ediyor ve ikisi tek `SaveChanges`
  sınırında yazılıyor; SQL'deki filtreli tekil indeks aynı kuralı zaten tutuyor
- POS tarafında tahsilat günü tanıyor, geçiş günü taşıyor: geçişten sonra hesap
  net kadar artıyor, yoldaki toplam sıfırlanıyor ve rapora hiçbir şey eklenmiyor
- Komisyon **ya tutar ya oran** olarak alınıyor; ikisi birden gönderilirse
  istek reddediliyor. Oran tutara çevriliyor, saklanan tek şey tutar
- `moneyInTransit` liste penceresinden bağımsız okunuyor: yolda olan para,
  kullanıcının hangi aya baktığından etkilenmemeli

| Kontrol | Sonuç |
|---|---|
| Backend build (Release) | 0 uyarı, 0 hata |
| Backend format (`--verify-no-changes`) | Temiz |
| Backend test (gerçek SQL dâhil) | **914 geçti**, 1 atlandı (`GeminiLiveContractTests`) |

Grubun ikinci yarısı Flutter ekranlarıdır (`features/cash/`, POS tahsilatı) ve
sıradaki iştir.

## 24 Ağustos 2026 — Aşama 04, Grup 7 (2/2) ve Grup 5

- Kasa/POS uçları açık Dart modelleri, repository ve controller katmanlarıyla
  Flutter'a bağlandı. `Kasa` sayfasında `Gün sonu` ve `POS tahsilatları` alt
  sekmeleri var; sayım farkı istemcide hesaplanmıyor, POS brüt/komisyon/neti
  ayrı okunuyor
- `FinancialDataChanges` iş anlamına göre ayrıldı: sayım yalnız Kasa'yı,
  farkın açık onayı finansal yüzeyleri, POS tanıma ile geçiş farklı hedefleri
  yeniliyor. Feed, yeni kaynakları Grup 8'de öğrenene kadar yükseltilmiyor
- Controller'lar shell profil/kapsam değişimlerinde yeniden çizilen sayfadan
  daha uzun yaşıyor ve tek yerde dispose ediliyor; yeniden çizim ağ isteği ve
  listener sızıntısı üretmiyor
- Ana shell dört hedefi koruyor. Üçüncü hedef işletme profilinde `Kasa`, kişisel
  profilde `Bütçeler`; yerinden inen hedef `Diğer` altında. Profil cevabı
  sonradan değişince ikisi birlikte yer değiştiriyor, özellik kapanmıyor
- `Kredi kartları` arayüz adı ADR 0015 uyarınca `Kredi kartlarım` oldu;
  tahsilat tarafı yalnız `POS tahsilatları` adını kullanıyor
- Yeni Flutter testleri para string hassasiyetini, kasa/POS mutation hedeflerini,
  iki profilde gezinmeyi ve Kasa'nın iki alt ekranını 2.0× metin ölçeğinde
  erişilebilirlik kapısıyla doğruluyor

| Kontrol | Sonuç |
|---|---|
| Flutter analyze | No issues found |
| Flutter format | Temiz |
| Flutter test | **742 geçti** |
| Flutter debug APK | Oluşturuldu (`API_BASE_URL=http://10.0.2.2:5284`) |

Sıradaki görev Aşama 04 Grup 6'dır: `İşlem ekle` menüsünü dört niyet başlığına
taşımak.

## 27 Ağustos 2026 — Aşama 06 Grup 3: cihaz üstü hatırlatma

- Vadesi gelen kayıtlar için hatırlatma **telefonun kendi zamanlayıcısına**
  kuruluyor: internet gerektirmiyor, geliştirme makinesi kapalıyken de çalışıyor.
  Sunucuda tek satır kod ve tek kolon şema değişmedi
- Kapı `Diğer → Hatırlatmalar`. `Hesabım` içinde değil: bu bir cihaz ayarıdır ve
  aynı hesaba başka bir telefondan girildiğinde o telefon kendi kararını taşır
- Ekran **kademeli**: kapalıyken yalnız tek anahtar duruyor. İzin ancak kullanıcı
  anahtarı açtığında isteniyor; reddedilirse anahtar kapalı kalıyor, ekran bunu
  söylüyor ve uygulama sessizce çalışmaya devam ediyor
- Kaynak kanonik planlanan projection (30 günlük ufuk); istemci **ikinci bir vade
  mantığı kurmuyor**. Yeniden kurma tek tek iptalin yerine geçiyor: her
  senkronizasyon `cancelAll` ile başlıyor, ödenen kalemin bildirimi listede
  olmadığı için kendiliğinden düşüyor
- Tetikleyici iki tane: oturum açılışı ve `FinancialDataChanges.planningRevision`.
  Yalnız planlanan hedefe bağlı — gün sonu sayımı gibi hatırlatmayı
  ilgilendirmeyen bir mutation telefonun bildirimlerini yeniden yazdırmıyor
- **Bildirim gün başına tek**: aynı güne düşen kalemler tek satırda toplanıyor
  (`2 ödenecek yükümlülük, 1 kart ekstresi`). Gövdede **tutar ve karşı taraf adı
  yok** — kilit ekranında görünen metin finansal bilgi taşımamalı
- Beş kova var; vergi takvimi kaleminin ayrısı **yok**, çünkü kalem tekrarlayan
  bir plandır ve öyle hatırlatılıyor
- Liste okunamazsa kurulu bildirimler **düşürülmüyor** ve bu ekranda yazıyor:
  dünkü hatırlatmayı silmek kullanıcıyı faturasından habersiz bırakırdı
- Oturum kapanınca ayar siliniyor ve bütün bildirimler iptal ediliyor
- Kesin alarm istenmiyor (`inexactAllowWhileIdle`); zaman dilimi cihazın o anki
  UTC farkından kuruluyor, üçüncü bir native bağımlılık eklenmedi
- Eklenen paketler: `flutter_local_notifications`, `timezone`. Android tarafında
  `RECEIVE_BOOT_COMPLETED` izni, eklentinin iki alıcısı ve core library
  desugaring
- Geçen kontroller: flutter analyze temiz, `dart format` temiz, **814 test
  geçti**, Android debug build üretildi

## 27 Ağustos 2026 — Aşama 06 Grup 4: varsayılan kapsam

- Hesap, kredi kartı ve kategori formuna **varsayılan kapsam** alanı eklendi.
  Kapsam türetme zincirinin orta halkası (kaynağın etiketi) artık uygulamadan
  kuruluyor; tek hesabına "dükkân kasası" diyen esnaf her kaydı tek tek
  işaretlemek zorunda değil
- Alanın **üç** konumu var: `Belirtilmedi · İşletme · Şahsi`. `Belirtilmedi`
  eksik veri değil, meşru bir cevap. Filtrenin `Hepsi`siyle aynı bileşende
  toplanmadı — orada boşluk "iki tarafı birden oku", burada "ben söylemiyorum"
- Alan yalnız kapsam boyutunu **gören** kullanıcıda çiziliyor; işletmesi
  olmayan kullanıcı üç formda da onu hiç görmüyor
- **Sessiz bir veri kaybı düzeltildi**: sunucudaki `PUT` yetkili ve
  gönderilmeyen `defaultScope` "kaldır" demek. Flutter üç güncellemede de alanı
  hiç göndermiyordu; API'den kurulmuş bir etiket, kullanıcı adı değiştirdiği an
  siliniyordu. Kategoride aynı kapı `defaultIsTaxDeductible`'ı da siliyordu — o
  alan bu ekranda düzenlenmiyor ama artık olduğu gibi geri gönderiliyor
- Backend'de tek satır değişmedi; sözleşme zaten alanı taşıyordu
- `docs/backlog.md` madde 5 kapandı
- Geçen kontroller: flutter analyze temiz, `dart format` temiz, **823 test
  geçti**, Android debug build üretildi

## 27 Ağustos 2026 — Aşama 06 Grup 5: fazla ödenmiş kart bakiyesi

- Kart borcunun kart başına `Math.Max(0, harcama − ödeme)` ile kırpılması
  **kaldırıldı**. Alacaklı bakiye artık net varlığa giriyor, kart dağılımında
  görünüyor ve kullanılabilir limiti limitin üstüne çıkarıyor. Ürün "fazla
  ödenen para kimin?" sorusuna artık tek cevap veriyor; cari hesap tarafı bu
  cevabı Aşama 02'den beri veriyordu
- Senaryonun gerçek yolu ödeme değil **iptal**: uygulama borcu aşan ödemeyi
  zaten reddediyor, ama ödenmiş bir harcamanın iptali kartı alacaklı bırakıyor
- **Taban iki yerde bilinçli olarak duruyor**, çünkü ayrı bir soruyu
  cevaplıyorlar: limiti aşmış kartta kullanılabilir tutar sıfırdır ve ekstre
  borcu negatif olmaz — ekstre "bu ay ne kadar ödemeliyim"in cevabıdır.
  Ekstrenin **devri** ise kırpılmıyor: geçen dönemden kalan alacak bu dönemin
  ekstresinden düşüyor, yoksa kartında parası olan kullanıcıdan borçlu olmadığı
  tutar isteniyordu
- Arayüz ad ve yön birlikte değişiyor: liste satırı `Kartınızda ₺500,00
  alacağınız var`, kart ayrıntısı `Kart alacağınız` (gelir tonu), Özet'in varlık
  kartı `Kart alacağı … net varlığa eklenir`. `Borç −₺500,00` hiçbir yerde
  yazmıyor
- Kırılan tek mevcut test `CalculateAvailableLimit`'in negatif borçta exception
  atmasını bekleyen testti; bilinçli olarak güncellendi
- `docs/backlog.md` madde 1 kapandı
- Yanı sıra: `SqlTestDatabase.ExecuteAsync` parametre dizisi nullable yapıldı;
  önceki gruplardan kalan tek derleyici uyarısı (CS8604) böylece düştü
- Geçen kontroller: backend build (**0 uyarı**) + format temiz + **1041 test**
  (gerçek SQL dâhil, 2 skip: Gemini ve Brevo canlı sözleşme testleri); Flutter
  analyze + format + **829 test** + Android debug build

## 27 Ağustos 2026 — Aşama 06 Grup 6: fiş/dekont cihaz kabul turu

Pixel 8 emulator'ünde, gerçek API + gerçek SQL + gerçek Gemini ile, Aşama 05'in
kabul hesabı üzerinde koşuldu. Belgelerin hepsi sentetiktir (HTML'den üretilmiş
PNG) ve tur bitince cihazdan silindi.

**Altı yolun hepsi tamamlandı:**

| Yol | Sonuç |
|---|---|
| Harcama | Market fişi okundu: tutar 280, tarih, ad ve kategori önerildi; **ödeme kaynağı boş bırakıldı** (ADR 0011) |
| Gelir | Tahsilat makbuzu 2.400 okundu, kategori `Satış geliri`, kapsam kategoriden `İşletme` |
| Vadeli fatura | `Bu faturayı ödediniz mi?` çıktı; `Henüz ödemedim` yükümlülük formunu açtı (hesap ve sıklık sormadan), son ödeme 2026-09-05 okundu |
| Taksitli fiş | Kart seçimine dallandı, plan 6 taksit / 3.600 önü dolu geldi; kaydedildikten sonra **güncel borç ₺0,00** kaldı — plan yalnız niyettir |
| İade | Eşleşen 280'lik harcama bulundu, onayla **iptal** edildi (silinmedi) |
| Dekont | Dört seçenekli karar sayfası; `Geri bekliyorum` borç/alacak planını açtı, ₺7,50 işlem ücreti **ayrı kayıt** olarak önerildi |

Turun sonunda aritmetik birebir tuttu: işletme neti −₺1.700 → **+₺60,00**
(+2.400 gelir − 640 yükümlülük), şahsi çekim −₺300 → **−₺307,50** (280'lik
harcama iade ile iptal edildi, kalan yalnız 7,50'lik dekont ücreti). Taksit
planı ve borç/alacak gelir/gidere hiç girmedi.

**Turda iki kusur bulundu ve düzeltildi:**

- **Eşleşme bulunan iade fişi hiç açılmıyordu.** `JsonReaders.object` **değer**
  alır, anahtar değil; çağrı iç nesne yerine taslağın kendisini geçiyordu.
  Taslak da geçerli bir `Map` olduğu için hata ancak `transactionId` aranırken
  çıkıyor ve ekranda "Sunucudan beklenmeyen bir fiş yanıtı alındı" görünüyordu.
  Mevcut testler eşleşmeyi hep elle inşa ettiği için kusur görünmüyordu; iki
  yeni test artık gövdeden okuyor
- **Borç/alacak formunda kapsam alanı yoktu** (Aşama 01'de atlanmış).
  `DebtAgreement` kapsam taşımak zorundadır; kaynağı ve kategorisi kapsam
  taşımayan kullanıcıda sunucu isteği `scope_unresolved` ile reddediyor ve
  dekonttan gelen borç planı hiç kurulamıyordu. Alan eklendi: zincirin
  önizlemesini gösteriyor, çözülemezse **istek gitmeden** formda söylüyor

**Üç kusur backlog'a yazıldı** (6, 7, 8): sunucunun İngilizce hata metninin
kullanıcıya düşmesi, taksit akışındaki boş kart listesinin sessiz kalması, fiş
okumanın belgede yazan KDV'yi önermemesi. Üçü de tek satırlık düzeltme değil.

- `docs/backlog.md` madde 3 kapandı
- Geçen kontroller: flutter analyze temiz, `dart format` temiz, **835 test
  geçti**, Android debug build üretildi. Backend'e dokunulmadı

## 27 Ağustos 2026 — Aşama 06 Grup 7: bütçe ekranı iyileştirmeleri

Kayıttaki "kapsamı belirsiz" madde önce **on maddelik bir öneri listesine**
döndü, liste kullanıcıyla onaylandı ve maddelerin hepsi uygulandı — 06.2'ye
devredilmesi önerilen ikisi dâhil.

**Verilen üç karar:**

- **Bütçe gerçekten silinir**, iptal edilmez. "Silme yerine iptal" kuralı para
  hareketlerini korur; bütçe bir olay değil, kullanıcının kendine koyduğu bir
  sınırdır ve hiçbir tutarı beslemez. Pasifleştirme yolu bırakılmadı çünkü tekil
  indeks (`UserId, CategoryId, Year, Month`) yanlış kurulmuş bir sınırın
  **doğrusunun da** kurulmasını engelliyordu
- **Aynı kategorinin iki tarafı ayrı bütçelenmiyor**: indekse kapsam eklenmedi,
  şema açılmadı. Bunun yerine satır hangi tarafı sınırladığını yazıyor — sessiz
  bir yarım toplam, olmayan bir ikinci limitten daha yanıltıcıydı
- **Eşik uyarısı için altıncı bir renk rolü açılmadı** (ADR 0008): uyarı ile
  aşım aynı renkte olsaydı renk, aşılmamış bir sınır için alarm verirdi

**Uygulanan maddeler:**

| # | İş | Nerede |
|---|---|---|
| 1 | Geçen ayın bütçelerini kopyalama; bu ayda karşılığı olan kategori atlanıyor | `budgets_controller` |
| 2 | Kapsam alanı forma, kapsam etiketi satıra; zincir önizleniyor, çözülemezse istek gitmiyor | `budgets_page`, `budget_models` |
| 3 | (b) yolu: şema açılmadı, satır hangi tarafı sınırladığını yazıyor | karar |
| 4 | `DELETE /api/v1/budgets/{id}` + açık onaylı silme | `DeleteBudgetUseCase`, `BudgetEndpoints` |
| 5 | Eşik uyarısı (`budgetWarningThreshold`), aşımdan önce konuşuyor | `core/models/budget_threshold.dart` |
| 6 | Harcama dökümü: kanonik feed projection'ının daraltılmış okuması | `BudgetRepository.listSpending` |
| 7 | Düzenleme alanı `100.0000` yerine `100` gösteriyor | `MoneyText.editable` |
| 8 | O ay bütçesi olan kategori seçim listesinden düşüyor | `budgets_page` |
| 9 | Dönem seçici: sekiz ay geriye tek dokunuş, tek istek | `AppMonthPicker` |
| 10 | Özet'in `Bütçe durumu` kartı eşiği de bildiriyor ve tıklanabilir | `dashboard_page` |

**Bir tespit yanlış çıktı ve düzeltildi:** "bütçe Özet ekranında hiç görünmüyor"
denmişti; `Bütçe durumu` kartı zaten oradaydı. Gerçek boşluk daha dardı — kart
yalnız aşımı konuşuyor, eşiğe dayanmış bütçeyi "limit içinde" sayıyordu ve
karttan bütçe ekranına gidilmiyordu.

Yanı sıra iki gizli kusur kapandı: kapsamı çözülemeyen bütçe isteği sunucunun
İngilizce cümlesini ekrana düşürüyordu (backlog madde 6'nın tam örneği) ve
kapsamı `Şahsi` doğan bir bütçe, aynı kategorideki işletme harcamasını hiç
saymadan "kalan 500" diyebiliyordu.

- `docs/backlog.md` madde 2 kapandı
- Geçen kontroller: backend build (**0 uyarı**) + format temiz + **1042 test**
  (gerçek SQL dâhil, 2 skip: Gemini ve Brevo canlı sözleşme testleri); Flutter
  analyze + `dart format` temiz + **857 test** + Android debug build

## 28 Ağustos 2026 — Aşama 06 Grup 8: kabul turunda çıkan üç kusur

27 Ağustos kabul turunda bulunup backlog'a yazılan üç madde (6, 7, 8) kapandı.
Üçü de "tek satırlık düzeltme değil" diye ertelenmişti ve gerçekten öyle çıktı.

**Madde 6 — sunucunun İngilizce hata metni kullanıcıya düşüyordu.**
Kabul turunda `The scope could not be resolved from the request, the account or
the category.` cümlesi Türkçe arayüzde göründü. Sebep yapıda değil, kapsamdaydı:
çeviri zaten tek yerdeydi (`ApiException._localizedMessage`) ama sözlükte
karşılığı olmayan **her** kod sunucunun `detail` alanına düşüyordu.

Backend'in ürettiği kod envanteri çıkarıldı: ~250 kod, hepsi `<alan>.<sebep>`
kalıbında. Her birine elle cümle yazan bir sözlük ilk yeni koddan sonra yine
sunucunun cümlesine düşerdi, bu yüzden çözüm **üç katmanlı** oldu — tam kod →
kalıp (alanın Türkçe adı + sebebin şablonu) → nötr yedek. `detail` artık
hiçbir koşulda ekrana yazılmıyor; kapı testi bunu sızıntı işaretleri arayarak
tutuyor.

**Madde 7 — taksit akışında boş kart listesi sessizdi.** Boşluk `children.isEmpty`
ile ölçülüyordu; fişten gelen mavi yönlendirme kutusu da bir liste elemanı
olduğu için kart yokken bile liste dolu sayılıyor ve `Henüz kredi kartı yok`
hiç çizilmiyordu. Ölçü artık listenin **verisine** bağlı.

**Madde 8 — fiş okuma KDV önermiyordu.** Model KDV tutarını zaten okuyordu
(toplam denetimi için); eksik olan onu taşımaktı. Prompt'a fişte **basılı**
oranı okuma görevi eklendi ve `taxAmount` ile `totalAmount`'tan oran
hesaplaması açıkça yasaklandı. Analiz cevabı artık kaydın KDV alanıyla aynı
biçimde `vat` (`rate` + `amount`) ve `vatState` taşıyor; form vergi bölümünü
**açık** açıyor, çünkü kapalı kalsaydı kullanıcı onaylaması gereken bir öneriyi
görmeden kaydederdi.

ADR 0016 burada da korundu: oran ile tutar bağımsız kaldı ve eksik olan
diğerinden **türetilmedi**. 280 brütün içindeki 46,67 tam olarak %20'dir ve
hesaplaması kolaydır — yine de yapılmıyor, çünkü yapılsaydı taşınan bilgi
uydurulmuş bilgiye dönerdi. Market fişinin tek bir oranı olmadığı için (%1, %10
ve %20 bir arada) oranın boş gelmesi meşru bir durumdur. Brütten büyük okunan
KDV düzeltilmiyor, reddediliyor (`receipt.vat_out_of_range`).

Sözleşme değişikliği geriye dönük kırıcıdır: `vat` ve `vatState` zorunlu
alanlardır ve istemci eksik gövdeyi `FormatException` ile reddeder. Ürün henüz
dağıtılmadığı için istemci ile sunucu birlikte gidiyor; katılığın sebebi diğer
`*State` alanlarıyla tutarlı olmak.

- `docs/backlog.md` maddeleri 6, 7 ve 8 kapandı; tabloda yalnız madde 4
  (Aşama 07'nin açılış kararı) kaldı
- Geçen kontroller: backend build (**0 uyarı**) + format temiz + **1052 test**
  (gerçek SQL dâhil, 2 skip: Gemini ve Brevo canlı sözleşme testleri); Flutter
  analyze + `dart format` temiz + **873 test**

## 28 Ağustos 2026 — Aşama 06.1 Grup 1: secret taraması

Üç ayak da tarandı ve **döndürülmesi gereken bir anahtar çıkmadı.**

- **Çalışma ağacı**: sağlayıcı anahtarı, özel anahtar bloğu, kodlanmış JWT ve
  parolalı connection string biçimleri arandı; bulunan tek şey sentetik test
  parolaları oldu
- **Repo geçmişi**: 62 commit, 3661 nesne. `.env`, `secrets.json`, keystore
  veya sertifika türünden bir dosya **hiç** commit edilmemiş
- **Debug APK**: gömülü anahtar yok; paketteki tek proje adresi `10.0.2.2`,
  yani `API_BASE_URL`'in emulator varsayılanı
- Yerel user-secrets üç değer taşıyor ve üçü de `documentation/variables.md`
  envanterinde yazılı; envanterde olmayan bir değer koda sızmamış

İki eksik kapandı. `.gitignore` imza anahtarı, sertifika ve `key.properties`
desenlerini taşımıyordu — bugün böyle bir dosya yok, ama Aşama 07 Android imza
anahtarını üretecek ve o gün geç kalmış bir `.gitignore` anahtarın commit
edilmesi demektir. Yapılandırma kapısı da dardı: yalnız `appsettings.json`
içindeki iki anahtara bakıyordu, `Gemini:ApiKey` / `Brevo:ApiKey` ve
`appsettings.Development.json` kapsam dışındaydı.

Tarama artık kalıcı ve iki yerde yaşıyor: çalışma ağacı `SecretScanTests` ile
her `dotnet test` koşusunda, repo geçmişi `scripts/Invoke-SecretScan.ps1` ile
CI'ın kendi işinde (`fetch-depth: 0` zorunlu — sığ klonda tarama sessizce
hiçbir şey görmez). **Kapının kırıldığı denendi**: sentetik bir anahtar taşıyan
paket bulguyu türüyle raporlattı ve betik 1 ile çıktı. Bulgunun değeri hiçbir
yere yazılmaz; rapor türü ve yeri taşır.

Geçen kontroller: backend build (0 uyarı) + `dotnet format` temiz +
**1057 test** (gerçek SQL dâhil, 2 skip: Gemini ve Brevo canlı sözleşme
testleri).

## 28 Ağustos 2026 — Aşama 06.1 Grup 2: bağımlılık zafiyet taraması

Backend'de transitive dâhil **zafiyetli paket yok**. Flutter tarafında kısıt
içindeki **14 paket yükseltildi** — aralarında `archive` 4.0.9 → 4.2.0, yani zip
ayrıştırıcısı; güvenlik düzeltmesinin en çok anlam taşıdığı yer orası. 873 testin
hepsi yükseltmeden sonra geçti.

Android release sınıfyolu ayrıca denetlendi: `junit`, `espresso` ve eski `guava`
yalnız `integration_test` projesinde duruyor, **ürüne girmiyorlar.** Bu bir
varsayım değil, `releaseRuntimeClasspath` çözülerek ölçüldü.

**Planlanmamış ama gerçek bir bulgu:** Android derlemesi temiz bir ağaçta hiç
kurulmuyordu. Kırılma bugünün değişikliklerinden gelmiyor — commit'li durum da
düşüyor; yalnız Gradle önbelleği eski çıktıyı tuttuğu için görünmüyordu. Sebep
AGP 9'un kendi Kotlin desteğini getirmesi ve kendi Kotlin Gradle eklentisini
uygulayan `share_plus`'ın derlenmemesi (12.0.2 ve 13.3.0'da aynı).
`settings.gradle.kts` AGP **8.13.0**'a sabitlendi, temiz derleme geri geldi ve
yeni APK yeniden tarandı — temiz.

Üç bulgu **gerekçesiyle kabul edildi** ve `docs/backlog.md` içine yazıldı:
AGP 9 sabiti (9), dört Flutter major yükseltmesi (10) ve `xunit` 2.9.3'ün
`Legacy` işareti (11). Hiçbirini bilinen bir zafiyet sürüklemiyor.

Zafiyet taraması CI kalite kapısına eklendi. `dotnet list --vulnerable` bulgu
bulduğunda sıfırdan farklı dönmüyor; çıktı okunup iş elle kırılıyor ve kapının
kırıldığı denendi.

Geçen kontroller: backend build (0 uyarı) + format temiz + **1057 test**;
Flutter analyze + `dart format` temiz + **873 test** + önbelleksiz debug APK.

## 28 Ağustos 2026 — Aşama 06.1 Grup 3: yetkilendirme kapsamı denetimi

Uç listesi elle sayılmadı, koddan üretildi: **108 uç, 46'sı kimlik taşıyor.**
Denetim tek dosyada toplandı (`OwnershipIsolationTests`) — bu grubun işi yeni
test yazmaktan çok eksik test aramaktı ve dağıtılmış testlerde eksik olanı
görmek, olanı görmekten zor.

- Kimlik taşıyan **46 uç**: sahip her kayıt türünden bir tane kuruyor,
  saldırgan her ucu iki kez çağırıyor — sahibin gerçek kimliğiyle ve hiç var
  olmamış bir kimlikle. İki statü aynı ve **404** olmak zorunda
- Kimlik taşımayan **31 okuma ucu**: sahibin hiçbir kimliği saldırganın
  gövdesinde geçmiyor. Ölçü cevabın şekline bakmadan kuruluyor
- Ölçümün boş olmadığı ayrıca kapıda: sahibin **24 kimliğinin hepsi** aynı
  uçlarda kendisine görünüyor
- Prob tablosu uygulamanın gerçek route tablosuyla karşılaştırılıyor; kimlik
  taşıyan yeni bir uç yazılmazsa takım kırmızıya döner. Denetim dışı bırakılan
  uç listesi bugün boş

**Denetim bir kusur buldu.** Taksit gerçekleştirme ucu bulunamayan plan için
`400` dönüyordu. Başka kullanıcının planı da aynı `400`'ü aldığı için sızıntı
değildi, ama ürünün kendi kuralını bozuyordu — "yok olan kayıt ve başkasının
kaydı aynı 404'e gider" — ve istemcinin hata sözlüğünde karşılığı olmayan bir
koda düşüyordu. `installments.not_found` ile 404'e çevrildi ve istemcideki
Türkçe karşılığı da kapıya bağlandı.

Geçen kontroller: backend build (0 uyarı) + format temiz + **1065 test**
(gerçek SQL dâhil, 2 skip); Flutter analyze + format temiz + **873 test**.

## 28 Ağustos 2026 — Aşama 06.1 Grup 4: log ve hata cevabı sızıntısı

Elle bakmak bir kereliktir; denetim teste bağlandı (`LeakageTests`). Gerçek bir
oturum koşuluyor — kayıt, giriş, hesap/kategori/işlem/cari yazma, doğrulama
kodu, bozuk gövde, yanlış parola, sorgu dizeli okuma — ve log gövdesi sekiz
değer için taranıyor: e-posta, parola, tutar, açıklama, karşı taraf adı, access
token, refresh token, doğrulama kodu. **Hiçbiri geçmiyor.**

Ölçü iki seviyede: ürünün gönderdiği yapılandırmayla ve bütün süzgeçler
kaldırılıp `Trace`'e inildiğinde. Uygulamanın kendi log satırları bilerek
yoksul — ele geçmemiş istisna yalnız türünü ve `traceId`'sini yazıyor, posta
sağlayıcısı yalnız sonucu yazıyor.

**Bulgu: ASP.NET Core istek satırını sorgu dizesiyle birlikte loglar.**
İstemcinin kurduğu bütün sorgular tarandı; bugün orada yalnız kimlik, tarih,
enum ve bayrak var — hiçbir tutar, açıklama veya ad yok. Ama bunu tutan bir şey
yoktu. `Microsoft.AspNetCore` kategorisinin `Warning`'de kalması artık bir
gürültü tercihi değil, sınırın kendisi ve testle tutuluyor.

Hata cevabı tarafında ele geçmemiş bir istisna üretildi: gövdede yığın izi,
istisna türü/mesajı, SQL metni, iç dosya yolu veya framework sürümü yok.
**Development ve Production ayrı ayrı** ölçüldü. İstemci tarafı da kapıya
bağlandı — `lib/` kaynağında yakalanan hatayı metne çevirmek yasak;
`error.message` serbest, çünkü o değer sunucudan değil sözlükten gelir.

Geçen kontroller: backend build (0 uyarı) + format temiz + **1065 test**
(gerçek SQL dâhil, 2 skip); Flutter analyze + format temiz + **875 test**.

## 1 Eylül 2026 — Aşama 06.1 kapandı

- **Aşama 06.1 kullanıcı onayıyla kapandı.** Beş çalışma grubunun hepsi, dört
  CI tarama kapısı ve Pixel 8 toplu kabul turu bitti. Belge
  `docs/archive/stages/06.1-guvenlik-taramasi.md` altına taşındı; tamamlanma
  kaydı (commit aralığı `9b42693`..`c56fb56`, grup grup çıktı, geçen kontroller)
  orada. `PROJECT-ROADMAP.md` ve `stages/README.md` durumu `Tamamlandı`
- **Sonraki aşama şimdilik açılmadı.** 06.2 (Arayüz düzeni) `Planlandı` kalıyor;
  kapsamı `research/rakip-arayuz-ve-akis/` altındaki rakip arayüz/akış
  araştırmasıyla besleniyor ve araştırma bitince kullanıcı açacak. **1 Eylül
  2026 itibarıyla hiçbir aşama Aktif değil; bu süre boyunca kod değişmez**
  (`stages/README.md`). Araştırma bu turda başka bir oturumda yürüyor ve
  uygulama koduna dokunmuyor
- Bu turda yalnız karar/durum belgeleri değişti; uygulama kodu değişmedi
- Geçen kontroller (1 Eylül 2026): backend build 0 uyarı + `dotnet format`
  temiz + `dotnet test` **1010 geçti**, 55 skip (bu koşuda SQL bağlı değildi;
  gerçek SQL dâhil son tam koşu 31 Ağustos 2026 Grup 4'te 1065 test). Flutter
  analyze + `dart format` temiz + **878 test** yeşil

## 2 Eylül 2026 — Aşama 06.2 yerel web denemesi

- Aşama 06.2 kullanıcı isteğiyle açıldı; yalnız ilk bağımsız checkpoint
  tamamlandı. Geniş arayüz grupları rakip araştırmasını beklemeye devam ediyor
- Flutter'ın resmî `web/` platform kabuğu eklendi. `ApiConfig` web'de
  `http://localhost:5284`, Android emulatorde `http://10.0.2.2:5284`
  varsayılanını seçiyor; açık `API_BASE_URL` her ikisini de ezebiliyor
- API'nin Development pipeline'ı yalnız `WebClient:AllowedOrigins` içindeki
  kesin origin'lere CORS izni veriyor. Yerel deneme origin'i
  `http://localhost:65087`; izinli ve izinsiz preflight ayrı testlerle tutuluyor
- Kapsam özellikle dar kaldı: responsive web düzeni, bildirim, kamera, web
  yayını, Windows/iOS ve ayrı API eklenmedi
- Geçen kontroller: backend Release build 0 uyarı + format temiz + **1012 test**
  (55 skip: SQL/canlı sağlayıcı ortamı yok); Flutter analyze + format temiz +
  **879 test**; web build ve Android debug APK başarılı

## 27 Eylül 2026 — Araştırma kaydı ve cloud denemesine hazırlık

- Araştırmanın Belge 1 ve Belge 2 kaynakları, bölüm PDF'leri ve birleşik
  PDF'leri Git'e alındı; tur öncesi yedekler (`raporlar/_yedek/`), eski sürüm
  (`belge2-v1/`) ve pilot/deneme belgeleri yerelde kalıyor (`.gitignore`)
- Aşama 06.2'ye cloud üzerinde Flutter arayüz denemeleri checkpoint'i yazıldı;
  `docs/backlog.md` madde 12 (Kasa sekmesinde çakışan yüzen düğmeler) 06.2
  Grup 6 listesine ilk madde olarak girdi
- `CLAUDE.md` belge haritasındaki "hiçbir aşama Aktif değil" ifadesi 06.2'nin
  Aktif olduğu gerçeğine çekildi
- Uygulama kodu değişmedi; build/test koşulmadı. Son doğrulanmış kontroller
  2 Eylül 2026 kaydındadır

## Son oturum kapanışı

- Yapılan değişiklik: araştırmanın Belge 1 ve 2'si commit'e alındı, karar ve
  durum belgeleri güncellendi, 06.2'ye cloud Flutter deneme checkpoint'i açıldı
- Geçen kontroller: kod değişmediği için koşulmadı; son tam koşu 2 Eylül 2026
  (backend **1012 test**, 55 skip; Flutter **879 test**; web ve Android debug
  build başarılı)
- Sıradaki görev: commit'leri push edip cloud üzerinde ilk Flutter arayüz
  denemesini başlatmak; ardından araştırmanın Belge 3'ü
