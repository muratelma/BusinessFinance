# Aşama 03 — Yükümlülük ve vade

## Belge durumu

- Durum: **Aktif** (24 Ağustos 2026'da kullanıcı onayıyla açıldı)
- Ön koşul: Aşama 02 — Cari hesap: karşı taraf ve açık bakiye
  (**tamamlandı**, `docs/archive/stages/02-cari-hesap-ve-karsi-taraf.md`)
- Sonraki aşama: Aşama 04 — Kasa, POS ve gezinme
- Dokunulacak kalıcı belgeler: `documentation/architecture.md`,
  `documentation/flows.md`, `documentation/permissions.md`,
  `documentation/tests.md`, `documentation/financial-activity-api-contract.md`,
  `documentation/receipt-analysis-api-contract.md`,
  `documentation/restore-runbook.md`
- Doğrulanmış ilerleme: `docs/project-status.md`

## Amaç

Ödenmemiş faturanın bugün bir kabı yok. Kullanıcı faturayı okutup "henüz
ödemedim" dediğinde uygulama onu **tekrarlayan plan formuna** gönderiyor
(`app_router.dart`, `_recordInvoice`). `RecurrenceFrequency` enum'unda tek
seferlik bir değer olmadığı için kullanıcı tek seferlik elektrik faturasına
Günlük/Haftalık/Aylık/Yıllık bir sıklık seçmek ve sonra planı elle
pasifleştirmek zorunda kalıyor.

Bu aşama iki eksiği kapatıyor: yükümlülüğün kendi kabı ve **vade** kavramı.

## Kullanıcıya katkı

- Gelen faturayı okutup "ödemedim" demek; hiçbir sıklık sorulmadan kaydedilmesi
- "Bu hafta hangi ödemelerim var, hangisi gecikti?" sorusunun tek listeden
  cevaplanması
- Vadesi geçmiş cari bakiyenin ayrı görünmesi
- 12 aylık kira sözleşmesinin 12'nci ayda kendiliğinden bitmesi; her yıl planı
  elle kapatmak zorunda kalmamak

## Değiştirilmeyecek mimari kararlar

- **Ekonomik olay tanır, ödeme taşır** (Aşama 02'de yazılan ADR).
- Kapsam boyutu ve türetme zinciri (ADR 0013).
- Bakiye kalıcı kolon değildir; gecikme durumu da kalıcı değildir — tarihten
  türetilir.
- Silme yerine iptal.
- Fiş okuma bir öneri katmanıdır; yönü ve ödeme kaynağını model seçmez
  (ADR 0011).
- Planlanan görünüm kanonik kaynaktır; ikinci bir sorgu tutulmaz.

## Aşama 02'den devralınan tutarsızlık

Aşama 02'nin ADR'si şunu yazılı olarak devretti: **fiş okumanın "ödemedim"
yolu ekonomik olayı tanımıyor.** Bugünkü davranış "hiçbir para hareketi yok,
gider de yok" diyor; oysa cari tarafta vadeli alım gideri **anında** tanıyor.
Aynı fatura, hangi ekrandan girildiğine göre farklı davranıyor.

Bu aşamanın ilk işi bu ikiliği kapatmak:

- Ödenmemiş fatura bir **yükümlülük** üretir ve gideri **tanır**; hesap
  bakiyesine dokunmaz. Kart harcamasının yaptığının aynısı.
- Ödeme yapıldığında yükümlülük kapanır, hesaptan para çıkar, **ikinci bir
  gider yazılmaz.**

Bugünkü ekrandaki "gider olarak yazılır / bakiyenizi etkilemez" ikilemi
kaybolur; ikisi de doğru olur ve sırayla gerçekleşir.

## Kapsam

### Dahil

- Tek seferlik yükümlülük kaydı (karşı taraflı veya karşı tarafsız)
- Vade tarihi, ödendi/ödenmedi durumu, gecikme
- Cari borçlandırmalara vade eklenmesi
- Tekrarlayan planda bitiş sınırı: bitiş tarihi veya tekrar sayısı
- Yaklaşanlar ve gecikenler görünümünün yükümlülükleri içermesi
- Fiş okumanın "henüz ödemedim" yolunun buraya bağlanması
- Yedek şeması v8

### Açıkça kapsam dışında

- Fatura **kesme**: numara, seri, e-Fatura/e-Arşiv gönderimi
- Otomatik ödeme, banka talimatı
- Push bildirimi ile hatırlatma (bulut aşamasına bağlı)
- KDV alanları — Aşama 05
- Gecikme faizi hesaplama

## Çalışma grupları

### Grup 1 — Domain: yükümlülük ve vade — **Tamamlandı**

- `Obligation`: tek seferlik yükümlülük. Taşıdıkları: yön (ödenecek/tahsil
  edilecek), tutar, düzenleme tarihi, **vade tarihi**, kategori, kapsam,
  açıklama, isteğe bağlı karşı taraf, durum.
- **Karşı taraf zorunlu değildir.** Elektrik faturası için kullanıcıyı cari
  kaydı açmaya zorlamak, kavramı hak etmediği bir yere sokar.
- Durum kalıcı bir "gecikti" bayrağı **taşımaz**; gecikme vade ile bugünün
  karşılaştırmasından türetilir. Kalıcı bayrak, saati geçtiğinde yanlış
  gösteren bir alan olurdu.
- Kapanış: yükümlülük ödendiğinde nakit hareketi üretir ve kapanır. Kapanış
  **idempotenttir** — ikinci onay ikinci ödeme üretmez.
- Vade doğrulaması gider/gelir tarihinden farklıdır: bir fiş yarından olamaz
  ama bir faturanın vadesi tam da gelecektedir ve geçmiş vade de meşrudur.
- Ölçüt: domain testleri tanıma/ödeme ayrımını ve idempotent kapanışı
  kanıtlıyor.
- Uygulanan ayrım: `Obligation` ekonomik olayı tanır; ona bire bir bağlı
  `ObligationSettlement` nakdi taşır. Settlement kategori ve kapsam taşımaz.
- `ObligationStatus` yalnız `Open`, `Settled`, `Cancelled` değerlerini taşır;
  gecikme `IsOverdueOn(asOfDate)` ile tarihten türetilir.
- Kapanış aynı aggregate üzerinde idempotenttir: ikinci çağrı ilk settlement'ı
  döndürür. Grup 2, `(UserId, ObligationId)` tekilliği ve iki stale SQL yazarı
  testiyle kalıcılık/eşzamanlılık kapısını da tamamladı.
- Grup 1 yalnız Domain'e dokundu; EF modeli ve migration Grup 2'de eklendi.

### Grup 2 — Cari borçlandırmaya vade — **Tamamlandı**

- Aşama 02'de tarihsiz açılan cari borçlandırma isteğe bağlı vade alır.
- Vadesi geçmiş cari bakiye ayrı okunur; karşı taraf listesinde görünür.
- Ölçüt: aynı karşı tarafın vadesi gelmiş ve gelmemiş bakiyesi ayrı raporlanıyor.
- `CounterpartyCharge.DueDate` nullable: eski satırların bilinmeyen vadesine
  tarih uydurulmadı ve migration kalıcı DEFAULT bırakmadı.
- Gecikme `asOfDate` ile owner-scoped tek SQL sorgusunda türetiliyor. Tahsilat
  belirli bir satıra bağlı olmadığı için önce gecikmiş borçlandırmayı kapatıyor;
  kalan `Overdue*` ve `NotOverdue*` olarak ayrı dönüyor.
- API liste ve ayrıntıda toplam + iki vade kırılımını kararlı para dizeleriyle
  döndürüyor; borçlandırma isteği opsiyonel `dueDate` alıyor.
- Flutter formu opsiyonel vadeyi taşıyor; liste gecikmeyi saat ikonu ve metinle,
  ayrıntı iki kırılımı ayrı satırlarla gösteriyor. 2.0× metin/a11y kapısı geçti.
- Grup 1'in `Obligation` ve `ObligationSettlement` tipleri owner-scoped bileşik
  foreign key'lerle kalıcılaştı; bir yükümlülüğe tek settlement veritabanında
  tekil. Endpoint/use case davranışı henüz açılmadı.

### Grup 3 — Tekrarlayan planda bitiş sınırı — **Tamamlandı**

- Plana bitiş tarihi **veya** tekrar sayısı eklenir; ikisi birden zorunlu değil,
  ikisi birden verilirse önce dolan geçerlidir.
- Sınıra ulaşan plan **pasifleşir, silinmez**: gerçekleşmiş occurrence geçmiştir.
- `generate(throughDate)` sınırı aşan occurrence üretmez; idempotency korunur.
- Bu, tek seferlik faturanın yerine geçen bir çözüm **değildir** — kendi başına
  gerçek bir ihtiyaçtır (12 aylık kira sözleşmesi) ve Grup 1 ile birlikte
  "ödenmemiş fatura plan olmaya zorlanıyor" hatasını tamamen kapatır.
- Ölçüt: 12 tekrarlı plan 13'üncüyü üretmiyor; retry ikinci kayıt açmıyor.
- Uygulanan alanlar: opsiyonel toplam `OccurrenceLimit` ve kalıcı
  `GeneratedOccurrenceCount`. Sayaç gerçekleşen para hareketini değil üretilmiş
  occurrence satırını sayar; bu yüzden onay bekleyen occurrence da sözleşmenin
  bir tekrarıdır.
- Bitiş tarihi ve tekrar sınırı birlikte verilebilir. `AdvanceAfter` önce sayaç
  sınırını, sonra bir sonraki tarihin `EndDate` sınırını denetler; önce dolan
  planı pasifleştirip `NextOccurrenceDate` alanını temizler. Geçmiş satırlar
  korunur ve tamamlanmış plan yeniden açılamaz.
- Planlanan projection henüz üretilmemiş tarihleri türetirken aynı sayacı
  ilerletir; sınırdan sonra hayali satır göstermez.
- `AddRecurringOccurrenceLimit` mevcut planların sayacını occurrence
  tablosundan backfill eder. Kolonlar backfill'den, backfill `NOT NULL` ve CHECK
  kısıtlarından önce gelir; kalıcı DEFAULT bırakılmaz. Mevcut planların bilinmeyen
  sınırı `null` kalır.
- API create/list sözleşmesi `occurrenceLimit` ve
  `generatedOccurrenceCount` alanlarını taşır. Flutter form alanı Grup 6'da
  açılacak; yedek v8 kapsamı Grup 7'de tamamlanacak.

### Grup 4 — Planlanan görünüm ve gecikenler — **Tamamlandı**

- Yükümlülükler planlanan projection'a katılır; `readiness` ve `attentionCode`
  kaynağın güncel durumundan türetilir, kalıcı değildir.
- Gecikenler Özet ekranındaki mevcut uyarı bandını besler.
- `IUpcomingPaymentRepository` ikinci bir sorgu tutmaz; aynı projection'ın
  daraltılmış görünümünü okur.
- Ölçüt: planlanan görünüm tek sorguda; yükümlülük ödendiği anda listeden
  düşüyor.
- Uygulanan projection iki yönü ayrı kararlı tür ve eylem kodlarıyla taşır.
  Beklenen settlement nötrdür; ekonomik olay düzenleme tarihinde zaten
  tanınmıştır. Ödenecek yön `isPaymentObligation=true`, tahsil edilecek yön
  `false` döner.
- Açık, owner-scoped yükümlülükler kategori ve isteğe bağlı karşı tarafla tek
  SQL komutunda okunur. İptal veya settlement varlığı kalıcı readiness/gecikme
  alanı yazmadan satırı düşürür.
- Flutter iki türü parse eder ve gecikmiş ödenecek satırı mevcut Özet bandında
  gösterir. Hesap seçen ödeme eylemi Grup 6 gelmeden yanlış forma bağlanmadı.

### Grup 5 — Fiş okumanın bağlanması — **Tamamlandı**

- `_recordInvoice`'ın "ödemedim" dalı planlama formuna değil yükümlülük
  kaydına gider.
- Okunan `dueDate` yükümlülüğün vadesi olur; belgenin kendi tarihi düzenleme
  tarihi kalır — ikisi karıştırılmaz.
- `receiptBillPrefillFrom`, `receiptObligationPrefillFrom` ile değiştirilir;
  `BillPrefill`'in planlama formuna bağlı hâli kaldırılır.
- Ölçüt: fatura okutulup "ödemedim" seçildiğinde hiçbir sıklık sorulmuyor.
- `POST /api/v1/obligations` current user'dan türeyen sahiplik kapısıyla açıldı;
  kategori zorunlu, karşı taraf isteğe bağlı ve ikisi de owner-scoped doğrulanıyor.
- Ödenmemiş fatura formu belge tarihi ile vadeyi ayrı gösteriyor; hesap ve sıklık
  istemiyor. Gider belge tarihinde tanınıyor, hesap bakiyesi değişmiyor.

### Grup 6 — Flutter

- Yükümlülük listesi: yaklaşanlar, gecikenler, kapananlar.
- Ödeme akışı: hesap seç, öde, kapat — tek ekran.
- Karşı taraf ayrıntısında vadeli bakiye satırı.
- Tekrarlayan plan formunda bitiş sınırı alanı.
- Ölçüt: zorunlu ekran durumları; gecikme rengi tasarım sistemi kontrast
  kapısını geçiyor.

### Grup 7 — Yedek v8 ve sözleşme belgeleri

- Şema **v8** yazar, yalnız v8 okur.
- `documentation/financial-activity-api-contract.md` yükümlülükle genişletilir.
- `documentation/receipt-analysis-api-contract.md`'nin "istemci ne yapar"
  tablosu yeni davranışa göre düzeltilir.
- Ölçüt: yükümlülükler ve plan bitiş sınırları kayıpsız geri yükleniyor.

## Zorunlu testler

### Domain / Application

- Ödenmemiş yükümlülük gideri tanır, hesap bakiyesini değiştirmez.
- Ödeme hesabı düşürür ve **ikinci gider yazmaz**.
- İkinci ödeme onayı ikinci kayıt üretmez (idempotent).
- Gecikme kalıcı alan değil; tarih değişince sonuç değişir.
- Bitiş sınırına ulaşan plan yeni occurrence üretmez.
- Vade geçmişte olabilir; belge tarihi gelecekte olamaz.

### API ve gerçek SQL

- Kullanıcı izolasyonu; pozitif **ve** negatif senaryo.
- Planlanan görünüm tek sorguda kalır; bounded query-count.
- Yükümlülük ödendiğinde feed, planlanan görünüm ve net varlık tutarlı.
- Yedek v8 yazma/okuma; v7 reddi.

### Flutter

- Fatura okuma → "ödemedim" → yükümlülük; sıklık sorulmuyor.
- Gecikmiş yükümlülüğün Özet uyarı bandında görünmesi.
- Zorunlu ekran durumları.

## Kalite komutları

```bash
dotnet build BusinessFinance.slnx --configuration Release --no-restore
dotnet test BusinessFinance.slnx --configuration Release --no-restore
dotnet format BusinessFinance.slnx --verify-no-changes --no-restore
```

```bash
flutter analyze
flutter test
dart format --set-exit-if-changed lib test
flutter build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5284
```

## Belge güncellemeleri

- `documentation/architecture.md`: yükümlülük ve vade projection'ı
- `documentation/flows.md`: fatura okuma, yükümlülük ödeme, plan bitişi
- `documentation/permissions.md`: yeni endpoint'ler
- `documentation/financial-activity-api-contract.md`: yükümlülük kaynağı
- `documentation/receipt-analysis-api-contract.md`: "ödemedim" davranışı
- `documentation/tests.md`, `documentation/restore-runbook.md`
- `docs/project-status.md`: yalnız doğrulanmış checkpoint'ler

## Güvenlik ve veri sınırları

- Bütün sorgular current-user kapsamlıdır.
- Yalnız sentetik veri ve sentetik fatura görseli kullanılır.
- Gerçek fatura, bulut güvenlik kapısı (Aşama 06) tamamlanmadan gönderilmez.

## Riskler ve azaltımlar

| Risk | Azaltım |
|---|---|
| Yükümlülük ödendiğinde giderin iki kez sayılması | Tanıma/ödeme ayrımı testi, çıkış koşulu |
| Gecikme durumunun kalıcı alana kaçması | Türetme kuralı belgede; tarih değiştiren test |
| Yükümlülük ile cari borçlandırmanın çakışması | Karşı taraflı yükümlülük cari bakiyeye katılır; tek sayım testi |
| Plan bitiş sınırının idempotency'yi bozması | Çift `generate` testi korunur |
| Fiş akışının eski plana bağlı kalması | `BillPrefill`'in planlama bağı kaldırılır, yerine test |

## Çıkış koşulları

- [ ] Bütün çalışma grupları tamamlandı.
- [ ] Backend build, test ve format kontrolleri geçti.
- [ ] Flutter analyze, test, format ve debug build kontrolleri geçti.
- [ ] Kullanıcı izolasyonu negatif senaryolarla kanıtlandı.
- [ ] Fatura okutulup "ödemedim" seçildiğinde sıklık sorulmuyor; ödendiğinde
      gider ikinci kez sayılmıyor.
- [ ] Bitiş sınırlı plan sınırı aşmıyor ve retry ikinci kayıt üretmiyor.
- [ ] `documentation/` ve `docs/project-status.md` güncel.
- [ ] Kullanıcı Aşama 04'ü açıkça onayladı.

## Tamamlanma kaydı

Aşama kapandığında burada: hangi commit'lerle bitti, hangi kontroller geçti,
belge `docs/archive/stages/` altına taşındı mı.
