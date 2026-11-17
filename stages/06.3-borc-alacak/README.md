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
| 2 | Sunucu: kişinin hesabı genişler | **Sürüyor** (commit edilmedi): S1 borç para, S3 istek kimliği, S4 kişinin türü, peşin/avans tutarı (`paidNow`), yedek v14 ve iki migration bitti. Kalan: S2 (hesaba kartla ödeme), S5 (vazgeçme, kalanı indirim sayma) |
| 3 | Sunucu: okumalar | Başlamadı |
| 4 | Ekran tasarımı | Başlamadı |
| 5 | Flutter: liste | Başlamadı |
| 6 | Flutter: kişi sayfası, tahsilat ve ödeme | Başlamadı |
| 7 | Flutter: yeni kayıt ve kayıt sayfası | Başlamadı |
| 8 | Eski yolların kalkması | Başlamadı |
| 9 | Kabul turu ve kapanış | Başlamadı |

## Sıradaki tek görev

**Faz 2'nin yapılan kısmını commit etmek** (kullanıcı onayıyla; belgeler
yazıldı, SQL dahil tam koşu yeşil). Sonra Faz 2'nin kalanı: S2 (hesaba kartla
ödeme), S5 (vazgeçme, kalanı indirim sayma); yedek v14 bunlarla genişler
(v14 henüz yayımlanmadı).

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
