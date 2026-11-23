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
| 4 | Ekran tasarımı | **Bitti** (23 Kasım), commit `652f728` |
| 5 | Flutter: liste | **Bitti** (23 Kasım): `Diğer` → `Borç ve alacak` gerçek listeyi açar |
| 6 | Flutter: kişi sayfası, tahsilat ve ödeme | **Bitti** (23 Kasım): kişi sayfası, `Tahsilat` / `Ödeme`, hareket süzgeci, iptal, indirim; borç para ve vazgeçme akış türleri. Faz 5 ile tek commit |
| 7 | Flutter: yeni kayıt ve kayıt sayfası | **Bitti** (23 Kasım), commit `15b1d09`: `Ne oldu?` → form (kişi seçici, peşin, taksitli + sunucudan önizleme, uygulamadan önce), kayıt sayfası (ödeme, geri alma, plan, indirim, iptal); sunucuda takvim önizlemesi ve taksit tutarı |
| 8 | Eski yolların kalkması | **Bitti** (23 Kasım): istemci bağlantıları ve eski sayfaların silinmesi, sunucu S9, satış iptalinde peşin sorusu, `MoneyFoundation_*` genişletmesi (net varlık hatası bulundu ve düzeltildi). Tek commit |
| 9 | Kabul turu ve kapanış | Başlamadı |

## Sıradaki tek görev

> **Sonraki oturumun sırası (kullanıcı kararı, 23 Kasım akşamı):**
>    ~~1. Faz 8~~ bitti (23 Kasım gece).
> 2. Ertelenen **Özet / Planlananlar / Tekrarlayanlar testi**
>    (`research/DEVIR-2026-11-22.md`, `stages/06.3-planlananlar/KABUL.md`).
> 3. Borç-alacak aynı şekilde test edilir: **Faz 9** kabul turu (§9
>    senaryoları emülatörde, Faz 4 kareleriyle yan yana; K15 sapması burada
>    değerlendirilir) ve kapanış. Faz 8'in iki sapması (kişi menüsünde
>    `Kişiyi sil` / `Yeniden etkinleştir`; hatırlatmada kişinin vadeleri)
>    kullanıcıya **ayrı maddeler olarak** denetilir.
>
> Devir: `research/DEVIR-2026-11-23.md`. Dal `borc-alacak-cari`
> (`ozet-tamamlama` buna ileri alındı). **Faz 7 commit edilmedi**
> (kullanıcıya soruldu).

**Faz 9'a giren karar (kullanıcıya emülatörde sorulacak):** akışta peşin
tahsilatın etiketi. Bugün satışla birlikte alınan peşin para `Tahsilat`
yazar (§7a: avans ayrı kayıt değildir). Seçenekler: (A) böyle kalsın; (B)
satışa bağlı satır `Peşin tahsilat` / `Peşin ödeme` yazsın (sunucu bağı
biliyor, istemci alanı eklenir); (C) `Avans`. Öneri: **A** — kişinin
hareketlerinde satışın hemen altında aynı gün durur ve iptal sorusu bağı
zaten söylüyor; ayrı ad bir kavram daha öğretir.
**Faz 7'den devreden:** satış iptalinde "avans da silinsin mi" sorusu
(`?withPaidNow=`; akış satırı satışa bağlı avansı bilmiyor, sunucu alanı
gerekir); akışta `avans` etiketi kararı; `MoneyFoundation_*` genişletmesi
(Faz 2'den kalan).

**Kullanıcı incelemesi (23 Kasım kararı):** ekranlara emülatörde, sunucuya
bağlandıktan sonra bakılır; K15 sapması (hareketler varsayılan olarak tüm
zamanlar) da o zaman değerlendirilir. Özet/Planlananlar testi de bekliyor
(yukarıdaki not).


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

### 23 Kasım 2026 — Faz 4 başladı: tarif, liste, kişi sayfası, panel

- **Kullanıcı kararı:** Özet/Planlananlar/Tekrarlayanlar testi ertelendi
  (kalkmadı); borç-alacağa geçildi. Belgeler buna göre düzeltildi
  (`stages/06.3-planlananlar/README.md`, `docs/project-status.md`,
  `research/DEVIR-2026-11-22.md`, bu dosya).
- **Yapılan:** `05-EKRAN-TASARIMI.md` yazıldı: başvuru incelemesi (ekran ekran
  alınan ve alınmayan kalıp), ortak dil, beş ekranın tarifi ve durumları,
  denetim tablosu. Görünümler kuruldu (sunucuya bağlı değil, Faz 5–6'nın
  başlangıcı): `lib/features/debt_book/` — `debt_book_models.dart` (liste ve
  kişi sayfası cevaplarının birebir karşılığı, `fromJson`),
  `debt_book_parts.dart` (satır, baş harf dairesi, bölüm kartı, toplam sütunu,
  koyu kart), `debt_book_view.dart` (liste: özet, sekmeler, `Yalnız
  gecikenler`, bölümler, `Kapananlar`, arama, ilk kullanım),
  `counterparty_page_view.dart` (kişi sayfası), `debt_settle_sheet.dart`
  (tahsilat/ödeme paneli). Örnek veri tek tutarlı settir (toplamlar satırları
  tutar): `test/screenshots/borc_alacak_tasarim_test.dart`, 12 kare × 3
  (412, 360, karanlık).
- **Denetim:** iki tur; birinci turda 4 kusur, 3 zayıf bulundu ve düzeltildi
  (Kişiler satırının boyu, çipin kayması, hareket başlığının kişi adını
  tekrar etmesi, panel notları); ikinci turda kusur yok, iki tercih kaldı
  (§5).
- **Tarifte koddan sonra değişen:** kapsam alt yazının sonunda; hareketler
  gün başlıksız tek kartta; panel notları tutar yazmaz (istemci çıkarma
  yapmaz). K15'ten sapma: kişinin hareketleri varsayılan olarak tüm
  zamanlar, ay filtrenin seçeneği (§3.2).
- **Yapılmayan:** yeni kayıt ve kayıt sayfası; büyük yazı ölçümü; sunucuya
  bağlama (Faz 5). Commit yok.
- **Kullanıcı kararı (aynı gün):** K15 sapmasına emülatörde son ekranlar
  bağlandıktan sonra bakılacak; beğenilmezse değişir.
- **İkinci bölüm:** `Ne oldu?` paneli (`debt_entry_sheet.dart`), yeni kayıt
  formu (`debt_entry_form_view.dart`: kişi alanı, kapalı satırlar, takvim
  önizlemesi) ve kayıt sayfası (`debt_record_view.dart`; model
  `DebtRecordData`, `DebtResponse`un karşılığı). Sekiz kare daha (toplam 20 ×
  3). Denetim: birinci turda 4 kusur, 2 zayıf; ikinci turda 2 zayıf, kusur
  yok (§5). Bulunan bir eksik: faizli kaydın `Kalan anapara`sı sunucu
  cevabında yok; istemci toplamaz, Faz 7'de sunucuya eklenecek. **Faz 4
  bitti.** Doğrulama: `dart format`, `flutter analyze` temiz; `flutter test`
  1343 geçti (295 atlandı: ekran karesi testleri yalnız `SCREENSHOT_DIR` ile koşar). Commit yok (kullanıcıya soruldu).

### 23 Kasım 2026 (devam) — Faz 5: liste gerçek veride

- **Faz 5 (kullanıcı: "commit et, birleştir, Faz 5'e geç, faizli
  kısımda önerini uygula"):** Faz 4 `652f728` olarak commit edildi;
  `borc-alacak-cari` `ozet-tamamlama`ya ileri alındı (çakışma yok) ve çalışma
  o dalda sürüyor. Sunucu: borç cevabına `remainingPrincipal` (açık
  taksitlerin planlı anapara payı; Domain `Debt.RemainingPrincipal`, Domain
  ve API testi). Flutter: `DebtBookRepository`, `DebtBookController` (kapsam
  anahtarını izler, cari/akış değişince yenilenir, düşen yenilemede eski
  liste + `Liste güncellenemedi.`), `DebtBookPage`, rota `/more/debt-book`,
  `Diğer` menüsünde `Borç ve alacak`. Geçiş: kişi → eski kişi sayfası, kayıt
  ve `Kapananlar` → eski borç sayfası, `+` → `Ne oldu?` → eski sayfalar.
- **Testler:** `test/features/debt_book/debt_book_test.dart` (22): taşma
  testi büyük yazıda üç gerçek taşma buldu (uzun tutar başlığı itiyordu) →
  satırda tutar büyük yazıda başlığın altına iner, normal yazıda en çok %60.
- **Emülatör (gerçek API, geliştirme veritabanı):** liste toplamı satırları
  tutuyor (alacak 2.200 + 2.950 + 1.400 + 300 = 6.850); kapsam menüsü genel
  kapsamı değiştirip listeyi süzüyor (sonra `Hepsi`ye geri alındı); kişi ve
  `+` yolları açılıyor. **Bulunan kusur:** kişisiz tek ödemeli faturalar
  `0/1 taksit` yazıyordu (sunucu tek ödemede de taksit sayısı gönderiyor) →
  taksitlilik plan türünden okunuyor, test eklendi, cihazda doğrulandı.
  Eski kişilerin türü boş olduğu için `Hepsi`de kapsam etiketi yazmıyor
  (doğru: uydurulmuyor). Karelerde sayfalar alt çubuksuz çizilir (`Diğer`den
  açılan sayfa kuralı).
- **Doğrulama:** `dart format`, `flutter analyze` temiz; `flutter test` 1365
  geçti. Backend build, `dotnet format` temiz; Domain 416, Application 431,
  Infrastructure 203, API 392 geçti (SQL bağlantısı tanımlı değildi: SQL'e
  bağlı testler atlandı; değişiklik sorgu içermiyor).
- **Yapılmayan:** `MoneyFoundation_*`. Faz 5'in commit'i Faz 6 ile birlikte
  (kullanıcı kararı).


### 23 Kasım 2026 (devam) — Faz 6: kişi sayfası canlı

- **Kullanıcı:** "Faz 6'ya geç, commit'i Faz 6 ile beraber at."
- **Akış türleri:** `counterparty-loan` (`Borç verdim` / `Borç aldım`) ve
  `counterparty-write-off` (`Alacaktan vazgeçme` / `Bağışlanan borç`)
  istemcide tanınıyor: ikon, ayrıntı satırları, iptal yolu, yenilenen ekranlar.
  Önceden bu türlerden biri yazıldığında bütün akış açılmıyordu.
- **Kişi sayfası:** `CounterpartyPage` + `CounterpartyPageController`: özet,
  sayfalı hareketler (`Daha eski hareketler`), süzgeç (dönem, tür, iki
  yönlüde taraf; sunucuda), `Tahsilat` / `Ödeme` paneli (hesap, POS, kredi
  kartı; istek kimliği panel açılırken bir kez üretilir), ayrıntıdan iptal,
  satış/alımda `İndirim yap` (`DebtDiscountSheet`), akış satırında `indirim
  ₺…` (`discountAmount`). Hareket alt yazısı sayfanın kişisini tekrar etmez
  (`ActivityTile.hideParty`).
- **Emülatörde bulunan iki kusur (gerçek API):** (1) kişi sayfası "beklenmeyen
  yanıt" diyordu — kısayol nesneleri yanlış okunuyordu (`JsonReaders.object`a
  alanın değeri yerine bütün cevap); sentetik bir deneme hesabıyla
  (`faz6-deneme@example.test`) ham cevap alındı, gerçek cevapla test eklendi.
  (2) `Tahsilat` paneli açılmıyordu — hesap seçenekleri `build` dışında
  `context.watch` ile okunuyordu; build'de alınıyor.
- **Emülatör sonucu:** 100 TL tahsilat (alacak 2.950 → 2.850), ayrıntıdan
  iptal (2.950'ye döndü), tür süzgeci. Kullanıcının verisi eski hâline döndü;
  geliştirme veritabanında sentetik deneme hesabı kaldı.
- **Kapsam dışı görülen:** form panelinde klavye `Kaydet`i örtüyor (bütün
  paneller) → `docs/backlog.md` madde 18.

### 23 Kasım 2026 (devam) — Faz 7: yeni kayıt ve kayıt sayfası

- **Kullanıcı:** "Devam edebilirsin."
- **Sunucu:** planın tek kuralı `Debt.PlanAmounts` (borç açılışı ve
  önizleme aynı yerden); `POST /api/v1/debts/schedule-preview` (kaydetmeden
  plan); borç açılışına `installmentAmount` (K17: toplamı sunucu kurar, istemci
  çarpmaz). API testi `InstallmentAmount_BuildsTheTotal_AndThePreviewUsesTheSamePlan`.
- **Kayıt sayfası:** `DebtRecordPage` + `DebtRecordController`: ödeme/tahsilat
  (ortak panel; kısayollar `Sıradaki taksit`, `Tamamı`; faizlide tam taksit),
  ödemeye dokununca onaylı geri alma, `⋮` planı değiştir / indirim / iptal,
  bütün takvim sayfası, kişi bağlantısı. Kapanışın yerinin adı hesap
  listesinden.
- **Yeni kayıt:** `DebtEntryPage` (seçenekler, kişi seçici
  `DebtPersonPickerSheet`, önizleme), `DebtEntryFormView` canlı (doğrulama,
  form içi ret), `DebtEntrySubmitter` (kaydın nereye gideceği tek yerde).
  `+` ve kayıt satırları artık yeni sayfaları açıyor.
- **Emülatör (gerçek API):** `Su faturası` ödendi (kapandı) ve geri alındı;
  `+` → `Veresiye sattım` → Ayşe Terzi → 100 TL (alacak 6.850 → 6.950) ve
  kişi sayfasından iptal (2.950'ye döndü). Veri eski hâlinde. Düzeltilen:
  tek ödemeli kayıtta geri alma onayı "taksit" diyordu.
- **Bilinçli bırakılan:** satış iptalinde avans sorusu ve akışta avans
  etiketi (Faz 8'e devir, yukarıda).

### 23 Kasım 2026 (akşam) — Faz 7 commit, Faz 8'in istemci tarafı

- **Kullanıcı:** Faz 7 ayrı commit (kullanıcı onayı; `design/`, kök
  görseller ve `research/` dışarıda).
- **Bağlantılar:** kişinin `⋮` menüsü (`Düzenle`, `Açılış bakiyesi`, `Pasife
  al` / `Yeniden etkinleştir`, `Kişiyi sil`; `counterparty_manage_sheets.dart`,
  depoya `PUT`/`DELETE /counterparties/{id}`). `Kapananlar` sayfası
  (`DebtClosedPage`, `GET /debt-book/closed`). Yeni kayıt rotası
  `/transactions/new/debt` (`DebtEntryRequest`, `DebtEntryPrefill`): `İşlem
  ekle > Borç veya alacak` (`Ödenmemiş fatura`nın yerine), fiş "henüz
  ödemedim" → `Vadeli aldım` (K10: eşleşen kişi kişiye, yoksa kişisiz;
  belgenin uyarıları formun başında), dekont "borç verdim" → `Borç verdim`
  (ücret kayıttan sonra paranın çıktığı hesaptan). Kişi ve kayıt sayfası
  kimlikle açılır (`/more/debt-book/people/{id}`, `/records/{id}`);
  planlananlarda borç satırları oraya götürür. Hatırlatmalar kişinin
  vadelerini kapsar; akıştaki tür adları §6.6'ya göre; tek ödemeli kaydın
  iptali `DELETE /debts/{id}`.
- **Silinenler (istemci):** `lib/features/debts`, `counterparties`,
  `obligations` ve testleri; `Cari hesap` ve `Yükümlülükler` menüleri;
  `hasCounterpartyLedger` ve `ScopeController.showsCounterpartyLedger`;
  eski ekranları çizen kare testleri. Aşama 02 kabul testi yeni depoya
  taşındı.
- **Sapmalar:** `Kişiyi sil` ve `Yeniden etkinleştir`; hatırlatma
  kovasının adı ve kişinin vadelerinin her ufukta okunması
  (`06.3-butunsel-duzenleme.md` Sapmalar). **Kullanıcı kararı:** şimdilik
  kalır; Faz 9'da emülatörde bu iki konu kullanıcıya **ayrıca** denetilir.
  Commit Faz 8 sonunda tek seferde.
- **Sunucu S9 (aynı oturum):** `/api/v1/obligations` uçları ve
  `Application/Obligations`, `Infrastructure/Obligations` katmanları; eski
  taksit ödeme ucu ve kullanım durumu (alan modelinin `PayInstallment`ı geri
  yükleme için yerinde); profilde `hasCounterpartyLedger`; kişi cevabında
  açık yükümlülük alanları kalktı. `GET /debts` ve `/opening` S9'da yok,
  yerinde. Dokuz API test dosyası `SingleDebts` düzeneğine taşındı;
  `DebtMergeBaselineTests` altın dosyasından yalnız kalkan üç şey çıktı (v12
  yedeği değişmedi).
- **Bulunan Faz 7 hatası:** yeni kayıt formu ayrı kayıt açarken (kişisiz
  fatura, taksitli satış/alım, faizsiz taksitli borç para) toplam geri
  ödemeyi göndermiyordu; sunucu `debt.repayment_required` ile reddederdi.
  `DebtEntrySubmitter` taksit tutarı yoksa toplamı tutar olarak gönderir;
  test eklendi. Emülatör denemesi bu yolu kapsamamıştı.
- **Doğrulama:** build ve `dotnet format` temiz; SQL Server'a karşı tam koşu:
  Domain 416, Application 431, API 400, Infrastructure 284 geçti (2 atlandı).
  Flutter: analyze temiz, 1317 test (tam koşu, sunucu öncesi), değişen
  dosyalar yeniden koştu.
- **Faz 7'den devredenler (aynı oturum):** akış satırına `paidNowAmount`
  (satışa bağlı iptal edilmemiş peşin tutar; 16 kola alan eklendi); iptal
  onayından sonra `Peşin alınan da iptal edilsin mi?` (`Yalnız satışı` /
  `İkisini de iptal et`; `?withPaidNow=true`). `MoneyFoundation_*` tohumuna
  borç para, kişiye kartla ödeme ve vazgeçme eklendi; **net varlığın borç
  parayı ve vazgeçmeyi saymadığı bulundu ve düzeltildi**
  (`EfFinancialReportRepository`). Avans etiketi Faz 9 kararı (yukarıda).
- **Son doğrulama:** build ve `dotnet format` temiz; SQL Server'a karşı tam
  koşu: Domain 416, Application 431, API 400, Infrastructure 284 (2 atlandı)
  geçti. Flutter: format ve analyze temiz, 1318 test geçti.
  satırında.
- **Yapılmayan:** emülatörde deneme (Faz 9); Aşama 02 kabul testi
  (`integration_test`) gerçek API'de koşmadı.
