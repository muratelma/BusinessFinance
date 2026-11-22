# Borç, alacak ve cari — giriş kapısı

Aşama 06.3 Grup 8'in ("Cari, borç ve alacak") çalışma klasörü. **Bu konuda
yeni oturum buradan başlar.** Dal: `borc-alacak-cari`.

| Belge | İçerik | Ne zaman değişir |
|---|---|---|
| `README.md` (bu) | Durum, günlük, sıradaki görev, açık sorular, `Sonra` | Her oturum |
| `01-ARASTIRMA.md` | Bugünkü durum, dış araştırma, bulgular, kaynaklar | Değişmez (gerekçe) |
| `02-TASARIM.md` | Kararlar K1–K13, kavramlar, ekranlar, sunucu farkı, senaryolar | Yalnız karar değişirse |
| `03-PLAN.md` | Çalışma kuralları, fazlar, oturum listesi | Yalnız plan değişirse |
| `04-SUNUCU-TASARIMI.md` | Şema, uçlar, migration, test listesi | Faz 1'de yazılır |
| `05-EKRAN-TASARIMI.md` | Başvuru incelemesi, ekran tarifleri, ortak kuraldan ayrılanlar | Faz 4'te yazılır |

Bu klasör şunların **yerini alır** (onlar geçmiştir, karar kaynağı değildir):
`design/borc-ve-alacaklar/` panoları ve README'si, `design/brifler/07-*`,
`research/cari-borc-alacak/`, ADR 0021'in T7 ve T8 bölümleri. ADR 0021'in tek
borç modeli (`Debt`) ve sunucudaki uygulaması geçerlidir.

## Tek cümle

Kişiyle olan her şey kişinin hesabında yürür; yalnız taksitli olan ya da
kişisi olmayan borç ayrı bir kayıttır. Nereye gideceğini uygulama bilir,
kullanıcıya sorulmaz.

## Durum

| Faz | İş | Durum |
|---|---|---|
| 0 | Karar kapısı | **Bitti** (16 Kasım 2026): kullanıcı onayladı, ADR 0022 kabul edildi |
| 1 | Kod gerçeği ve sunucu tasarımı | **Belge yazıldı** (`04-SUNUCU-TASARIMI.md`); kalan okuma Faz 2'nin ilk işi |
| 2 | Sunucu: kişinin hesabı genişler | **Sürüyor** (commit edilmedi): **Bitti** (17 Kasım 2026): S1 borç para, S2 kişiye kartla ödeme, S3 istek kimliği, S4 kişinin türü, S5 vazgeçme ve kalanı indirim sayma, peşin/avans tutarı (`paidNow`), yedek v14, dört migration |
| 3 | Sunucu: okumalar | **Bitti** (17 Kasım 2026): S6 kişi sayfası, S7 liste, S8 (sunucu değişikliği gerekmedi), S11 vadeler planlananlarda (isteğe bağlı) |
| 4 | Ekran tasarımı | Başlamadı |
| 5 | Flutter: liste | Başlamadı |
| 6 | Flutter: kişi sayfası, tahsilat ve ödeme | Başlamadı |
| 7 | Flutter: yeni kayıt ve kayıt sayfası | Başlamadı |
| 8 | Eski yolların kalkması | Başlamadı |
| 9 | Kabul turu ve kapanış | Başlamadı |

## Sıradaki tek görev

**Faz 4 — Ekran tasarımı** (`03-PLAN.md`): başvuru incelemesi ve
`05-EKRAN-TASARIMI.md` (liste, kişi sayfası, tahsilat/ödeme paneli, yeni kayıt,
kayıt sayfası). Kullanıcı kararı (16 Kasım): denetimi Claude Code dış göz olarak
yapar. Ayrıntılı devir: `research/DEVIR-2026-11-17.md` (§8 adımlar).

**Planlananlar ve tekrarlayanlar üzerinde Codex çalışması** (kullanıcı kararı,
17 Kasım 2026): Claude Code'un limiti yenilenene kadar kullanıcı planlananlar,
tekrarlayanlar, Özet'teki yaklaşanlar ve işlemlerin üst kısmını Codex'e
inceletecek (önce yalnız okuma; sonra plan ve uygulama, ayrı dalda, iki belge:
plan ve ilerleme). Sırayla çalışılır, aynı anda değil.
- **Başlangıç:** `d359ab7` (borç-alacak sunucusu bu hâliyle). Codex'in dalı
  `borc-alacak-cari`nin üstünden açılır; Faz 5'ten (Flutter) önce onaylanan
  iş bu dala birleştirilir.
- **Borç-alacağa ait, anlamı değişmeyecek olanlar:** planlanan türleri
  `debt-installment` / `receivable-installment`, `payable-obligation` /
  `receivable-obligation` (Faz 8'de değişecek), `counterparty-receivable-due` /
  `counterparty-payable-due` ve eylemleri, `actionTargetId` /
  `actionSequence` anlamı; `ListDebtAsync`, `ListObligationsAsync`,
  `ListCounterpartyDuesAsync`; `includeCounterpartyDues` bayrağı (Faz 5–7'de
  borç-alacak ekranlarıyla açılır); yükümlülük ekranı ve uçları (Faz 8'de
  kalkar). Yeniden düzenlenirse çıktılar birebir aynı kalır.
- **Codex yaparsa iyi olur:** istemci bilinmeyen planlanan ve akış türünü
  atlasın (bugün `FormatException` ile ekranı bozuyor); satırın etiketi,
  ikonu ve eylemi tek bir eşleme tablosundan gelsin (yeni tür tek satırla
  eklenir).
- Döndüğümde ilk iş: Codex'in ilerleme belgesini ve `d359ab7`den sonraki
  commit'lerini okumak; sonra Faz 4.

Faz 4'ten bağımsız açık işler (devir §7):
- Geliştirme veritabanı `BusinessFinance` dört migration'la yükseltildi
  (17 Kasım, oturum sonu); API 5284'te ve emülatör açık bırakıldı.
- `MoneyFoundation_*` SQL testi borç para, kartla ödeme ve vazgeçmeyle
  genişletilmedi (planın Faz 2 testi); Faz 5'ten önce eklenmeli.
- `docs/project-status.md` checkpoint satırı yazıldı, commit edilmedi (bir
  sonraki kod commit'iyle girer).
Açık iş: Flutter bilinmeyen akış türünde (`counterparty-loan`,
`counterparty-write-off`) akışı reddediyor; istemci türleri tanıyana kadar bu
kayıtlar yalnız API'den yazılabiliyor.

Açık iş (Faz 2 çıkışı "eski istemci çalışır" koşulu): Flutter
`ActivityKind.fromApi` bilinmeyen türde `FormatException` atıyor; ilk
`counterparty-loan` kaydı yazıldığında bugünkü istemcide akış açılmaz. Bugün
istemciden borç para yazılamadığı için tetiklenmiyor; istemci türü tanıyana kadar
ya istemciye tür eklenir ya da bu not Faz 5'e taşınır.

## Açık sorular

Kullanıcıya: yok. Avans kararı (`02-TASARIM.md` §7a, seçenek A + satış
formunda `Peşin / avans alınan`) 17 Kasım'da onaylandı. Akışta `avans` etiketi
Faz 7'de kararlaştırılır.

Kod okunarak cevaplanacaklar: `04-SUNUCU-TASARIMI.md` §8 (beş madde).

## Sonra

Plan dışı fikirler buraya yazılır; fazı bekletmez. Başlangıç listesi
`02-TASARIM.md` §8.

- (boş)

## Günlük

Her oturum sona bir kayıt ekler: tarih, yapılan, yapılmayan, bakılmayan,
doğrulama sonuçları.

### 16 Kasım 2026 — araştırma, tasarım, plan

- **Yapılan:** dal `borc-alacak-cari` açıldı (`main`den). Önceki kararlar
  (ADR 0020, 0021, 29 Ekim panoları, ortak karar listesi, kod gerçeği notu),
  sunucunun borç ve cari modelinin iskeleti ve PRD okundu. Dış araştırma
  yapıldı (veresiye uygulamaları, ön muhasebe ürünleri, kişisel finans
  uygulamaları, Apple Card ve tasarım yönergeleri). Dört belge yazıldı.
- **Yapılmayan:** kod değişmedi; ADR yazılmadı; `06.3-butunsel-duzenleme.md`,
  `PROJECT-ROADMAP.md`, `CLAUDE.md` ve PRD'ye dokunulmadı (Faz 0 onayını
  bekliyor); belgeler commit edilmedi.
- **Bakılmayan:** sunucu kodu satır satır okunmadı (Faz 1'in işi; tasarımın §7
  bölümü iskelet düzeyinde doğrulandı). Rakip uygulamalar cihazda denenmedi.
  29 Ekim'deki "her alım ayrı borç" modelinden dönüşün gerekçesi belgelerde
  yazılı değil; oturum notuna göre kullanıcı reddetti (Borçlarım karma bir
  listeye dönmüştü).
- **Ortam:** 5284'teki API ve Pixel 8 emülatörü bu oturumda açıldı ve açık
  bırakıldı (kullanıcı ekranlara bakıyor).

### 16 Kasım 2026 (devam) — tasarımın kullanıcıyla görüşülmesi

- **Yapılan:** tasarım sohbette diyagramlar ve senaryolarla anlatıldı.
  Kullanıcının soruları ve önerileriyle değişenler `02-TASARIM.md`ye işlendi:
  - K3 yeniden yazıldı: kapsamı kişi taşır (`İşletme` / `Şahsi`); cari,
    İşletme kişilerinin hesabıdır. Kullanıcının önerisi; kayıt kayıt kapsam
    hesabından daha sağlam bulundu.
  - K4 yeniden yazıldı: sayfa kapsam anahtarını izler (ilk hâlinde kapsam
    filtresi yoktu; kullanıcı cari tanımını hatırlattı).
  - K14–K18 eklendi: taksitli kayıt kendi satırı, kişide hareket filtreleri,
    çek ve senet yalnız vade, kredide faiz hesaplanmaz, uzun takvim kısalır.
  - Yeni kayıtlar: `Peşin ödenen`, `Kalanı indirim say`, `Avans aldım /
    verdim`. Senaryolar 18–29 eklendi.
  - `03-PLAN.md` Faz 1'e iki teknik soru (kişinin türü, kredi dağılımı) ve
    Faz 2'ye S5b eklendi.
- **Yapılmayan:** bütünün onayı alınmadı; ADR yazılmadı; kod değişmedi;
  belgeler commit edilmedi.
- **Kısa kredi taraması:** eşit taksitli tüketici kredisi, erken kapama, spot,
  rotatif ve kredili mevduat, ödemesiz dönem. Kaynaklar sohbette verildi;
  ayrıntılı karşılaştırma Faz 1'in işi.
- **Sonraki kararlar (aynı gün):** seçerek ödeme ilk sürümde yok (K7 onay).
  K13 yeniden yazıldı ve plana **Faz 4 — Ekran tasarımı** eklendi (sonraki
  fazlar bir kaydı): kullanıcı hazır bileşenlerden hızlı kurulan ekran
  istemiyor. Karma kişi değerlendirildi: "kişi başına iki hesap" reddedildi,
  gerekçesi `02-TASARIM.md` §3; taksitli ve kişisiz kayıt kendi kapsamını
  taşır (senaryo 30–31).
- **Onay (aynı gün):** kullanıcı tasarımı genel olarak onayladı. Ek karar:
  ekran tasarımının denetimini Claude Code dış göz olarak kendisi yapar,
  kullanıcı tek tek bakmaz ("ben bakarsam süreç uzar; son sürüme uygun
  kalitede tasarım yap; gereken skilleri ekleyebilirsin"). `03-PLAN.md` Faz 4
  buna göre yazıldı. **Faz 0 kapandı:** ADR 0022 yazıldı; ADR 0020 ve 0021'e
  not, `PROJECT-ROADMAP.md`, `stages/README.md`, `06.3-butunsel-duzenleme.md`,
  `CLAUDE.md` ve PRD §6.6 güncellendi. Commit edilmedi.

### 16 Kasım 2026 (devam) — Faz 1: kod gerçeği ve sunucu tasarımı

- **Yapılan:** cari defterin Domain ve kullanım durumları, gecikme okuması ve
  `Debt`in başı okundu; cari tablolarını okuyan 16 dosya, kart borcu, gün
  sonu, planlanan projection, Flutter'daki `obligations` çağıranları ve
  hatırlatmaların kaynağı tarandı. Dokuz teknik soru cevaplandı,
  `04-SUNUCU-TASARIMI.md` yazıldı.
- **Bulgular (tasarımı etkileyen):**
  - Borç para borçlandırmaya tür olarak değil **ayrı tablo** olarak girer
    (tasarımın ilk önerisi türdü): borçlandırmayı 16 dosya gelir/gider diye
    okuyor; unutulan bir süzgeç borç parayı sessizce gelir yazardı.
  - **Planlanan projection cari vadeleri okumuyor**; hatırlatmalar da oradan
    besleniyor. Borçların çoğu kişinin hesabına taşınacağı için yeni iş S11
    eklendi.
  - Kredi modeli K17 ile zaten uyumlu: tutarlar kullanıcıdan, oran türetiliyor.
- **Yapılmayan / okunmayan:** `DebtUseCases`, `DebtClosingUseCases`,
  `EfCounterpartyRepository`, `EfDebtBookReader`, akış okuması,
  `AccountMovements`, yedek, API uç dosyaları ve testler okunmadı; beş madde
  "doğrulanacak" olarak işaretli. Kod değişmedi; hiçbir belge commit edilmedi.
- **Ortam:** 5284'teki API ve emülatör bu oturumda açıldı; oturum sonunda
  kapatıldı.

### 16 Kasım 2026 (gece) — Faz 2 başladı, oturum yarıda kaldı

Bu kayıt 17 Kasım'da çalışma ağacından yazıldı; oturum süre sınırında kesildi
ve kendi kaydını yazamadı.

- **Yapılan (commit edilmedi):** kişide tür (`Counterparty.Scope`, nullable;
  işletmesi olmayan kullanıcıda `Şahsi`, işletmesi olanda gönderilmezse boş);
  borçlandırmada genel kapsam zinciri (kategori → açık seçim → kişinin türü;
  türü boş eski kişide profil) ve istek kimliği (`counterparty-charge`); borç
  para kaydı (Domain, kullanım durumları, EF deposu, uçlar) ve onu okuyan
  bakiye, gecikme, işlem sonrası net, hesap hareketleri, akış, hesap sayfası,
  kişi silme ve hesap silme yolları; gün sonu yalnız `İşletme` kapsamlı
  veresiye satışı sayar; migration; yeni API testleri.
- **Yarıda kalan:** kod `CounterpartyAdvance` (tür: `loan` / `advance`) olarak
  yazıldı, son yazılan test ise avansı tahsilat sayan `CounterpartyLoan`
  modelini kullandı; test projesi derlenmiyordu. Son komut (SQL'li tam koşu)
  bu yüzden düştü.
- **Bakılmayan:** §8'deki beş "doğrulanacak" maddenin kapandığına dair kayıt
  yok.

### 17 Kasım 2026 — yarıda kalan Faz 2'nin toparlanması

- **İnceleme:** önceki oturumun farkı okundu (38 değişen, 7 yeni dosya).
  Uygulama projeleri derleniyordu; tek kırık test projesiydi. Kodda başka hata
  görülmedi.
- **Karar (sapma, onay bekliyor):** testin modeli seçildi. Avans ayrı kayıt
  değildir, tahsilat ya da ödemedir; kayıt `CounterpartyLoan` oldu, `kind`
  alanı, `counterparty-advance` akış türü ve `Kind` CHECK'i kalktı; uçlar
  `POST /counterparties/{id}/loans`, `DELETE /counterparty-loans/{id}`.
- **Migration:** uygulanmamış `AddCounterpartyScopeAndAdvances` kaldırıldı,
  `AddCounterpartyScopeAndLoans` üretildi (nullable kolon, boş tablo, CHECK
  kolondan sonra). Zincir testi ve migration sıra testi eklendi.
- **Testler:** eski "cari borçlandırma her zaman işletme" testi ADR 0022'ye
  göre yeniden yazıldı (şahsi kategori şahsi yazar, çelişen seçim reddedilir);
  sahiplik denetimine iki yeni uç ve borç para kimliği eklendi.
- **Bulunan gerileme:** gecikme okuması borç parayı ayrı sorguyla okuyordu;
  kişi listesi 2 yerine 3 sorgu atıyordu (`CounterpartyBalances_ComeFromOneQuery…`
  yakaladı). Alımlar ve borç para tek `UNION ALL` sorgusunda okunuyor.
- **Doğrulama:** build ve `dotnet format` temiz. SQL Server'a karşı tam koşu
  (`BusinessFinanceApiSqlTests` yeni migration'la yükseltildi): Domain 416,
  Application 418, API 386, Infrastructure 278 geçti, 2 atlandı, 0 düştü.
- **Yapılmayan:** belgeler (`documentation/`) ve commit; Faz 2'nin ikinci
  yarısı.
- **Avans görüşmesi (aynı gün):** kullanıcı avansın tanımını verdi ("hak
  edilecek tutara mahsuben ön ödeme"); araştırma (Tek Düzen 340/159, Paraşüt,
  Zoho, ERPNext) ve seçenekler `02-TASARIM.md` §7a'ya yazıldı. Kullanıcının
  önerisi (anlaşma + avans tek formda) artı/eksiyle değerlendirildi; ikisi de
  kalır, eksilerin karşılığı aynı bölümde. Kullanıcı onayladı.
- **Yedek v14:** kişinin türü, borç para ve avansın satışa bağı. İlk yarının
  commit'i yedeksiz kalmasın diye öne alındı (ikinci yarıya planlanmıştı);
  v14 yayımlanmadan S2 ve S5 ile genişler.
- **`paidNow` (kullanıcı onayı):** satış ya da alımla birlikte peşin/avans
  tutarı tek yazmada; tahsilat satışa bağlı (`PaidWithChargeId`, ikinci
  migration `AddCounterpartyPaidNow`); satış iptali avansı varsayılan olarak
  bırakır, `?withPaidNow=true` ikisini birlikte iptal eder.
- **İki migration'ın `Down`'ı** veri varsa durur (`THROW 50023`, `50024`).
- **Belgeler:** `debt-api-contract.md`, `permissions.md`, `architecture.md`,
  `tests.md`, `restore-runbook.md`, `CLAUDE.md` özeti.
- **Son doğrulama:** build ve `dotnet format` temiz; SQL Server'a karşı tam
  koşu (`BusinessFinanceApiSqlTests` iki migration'la yükseltildi): Domain 416,
  Application 418, API 389, Infrastructure 281 geçti, 2 atlandı, 0 düştü.
- **Commit'ler (kullanıcı onayıyla):** `7ffc915` karar belgeleri, `021845d`
  borç para, kişinin türü, `paidNow`, yedek v14.
- **S2 — kişiye kredi kartıyla ödeme:** `CounterpartyPayment.CreditCardId`
  (hesap o zaman boş; CHECK tam olarak birini ister, kart yalnız ödemede);
  limit kart harcamasıyla aynı kural; `CardDebt` üçüncü kaynak olarak okur
  (borç, ekstre, kalan limit, net varlık); kart sayfası hareketleri, akış
  (kaynak kartın adı) ve işlem sonrası kart borcu. Migration
  `AddCounterpartyCardPayments` dolu SQL test veritabanında ileri, geri, ileri
  denendi. Tam koşu: Domain 416, Application 418, API 391, Infrastructure 282
  geçti, 2 atlandı.
- **Commit:** `670d1ed` kişiye kartla ödeme.
- **S5 — vazgeçme ve kalanı indirim sayma:** `CounterpartyWriteOff` (tanır;
  alacaktan vazgeçmek gider, bağışlanan borç gelir; `RecognizedItems`, bakiye,
  gecikme, `CounterpartyNet`, akış `counterparty-write-off`, kategori
  kullanımı, yedek). `Kalanı indirim say`: ödemeyle aynı yazmada en yeni açık
  alımlara indirim, ödemeye bağlı (`PaymentId`); ödeme iptali onu da geri
  alır, tek başına geri alınamaz; kalan borç para ya da devreden bakiyeden
  geliyorsa reddedilir. Hesap silme indirimleri ödemelerden önce
  siler. Migration `AddCounterpartyWriteOffsAndRemainderDiscounts` dolu SQL
  test veritabanında ileri, geri, ileri denendi.
- **Doğrulama:** build ve format temiz; SQL'e karşı tam koşu: Domain 416,
  Application 418, API 394, Infrastructure 283 geçti, 2 atlandı.
- **Faz 2 kapandı.** Çıkış koşulları: build, format, SQL dahil tam koşu geçti;
  ekran değişmedi. "Eski istemci çalışır" koşulu yeni kayıtlar yazılmadıkça
  sağlanıyor (bilinmeyen akış türü notu yukarıda).

### 17 Kasım 2026 (devam) — kalanın sırası, Faz 3 başlıyor

- **Kalanı indirim say, sıra düzeltmesi** (kullanıcı onayı): kalan artık
  ödemelerin sırasıyla bulunur (en eski önce kapanır). Kalanın içinde borç
  para, devreden bakiye, gün sonunda sayılmış ya da ödeme gününden sonraki
  alım varsa reddedilir. Test: `RemainderDiscount_FollowsThePaymentOrder`
  (borç para alımdan sonra → red; önce → indirim).
- **Faz 3 (sunucu okumaları):**
  - **S7 liste** (`GET /api/v1/debt-book`): `scope` süzgeci (türü bilinmeyen
    eski kişi o kapsamda kaydı varsa), `Yaklaşan` 30 gün ve `other` bölümü,
    bölüm içi sabit sıra (gecikenlerde en eski vade), iki gecikmiş toplam,
    satırda kapsam, avans payı ve taksit sayıları, Kişiler'de bütün kişiler
    ada göre (pasif ve bakiyesiz dahil). Sabit sayıda sorgu (SQL testi).
  - **S6 kişi sayfası** (`GET /api/v1/counterparties/{id}/page`): özet hesap +
    taksitli kayıtlar, kısayollar yalnız hesabın, taksitli kayıt satırları;
    hareketler akıştan, yeni `direction` süzgeciyle.
  - **S8:** sunucu değişikliği yok (eşleşme ve uçlar hazır); yönlendirme Faz 7.
  - **S11:** `includeCounterpartyDues=true` ile kişinin vadeli açık tutarları
    planlananlarda (yeni türler ve eylemler); varsayılan kapalı.
  - Sapmalar `06.3-butunsel-duzenleme.md`de (S6 hareketleri akıştan, S11
    isteğe bağlı, S8 istemcide).
  - **Doğrulama:** build ve format temiz; SQL'e karşı tam koşu: Domain 416,
    Application 418, API 398, Infrastructure 284 geçti, 2 atlandı.
