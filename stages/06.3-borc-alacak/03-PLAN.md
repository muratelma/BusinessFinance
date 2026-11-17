# Borç, alacak ve cari — plan (16 Kasım 2026)

Tasarım `02-TASARIM.md`. Bu belge işin sırasını, her fazın giriş ve çıkış
koşulunu söyler. Güncel durum ve oturum günlüğü `README.md` içindedir; bu
belge yalnız plan değişirse düzenlenir.

## Çalışma kuralları

1. **Bir faz, bir checkpoint.** Faz çıkış koşulları sağlanmadan bitmiş
   sayılmaz; bitince tek commit atılır (kod + o davranışın belgeleri).
2. **Karar yeniden açılmaz.** `02-TASARIM.md` §2'deki bir karar yalnız para ya
   da kayıt yanlışlığı gösterilirse yeniden açılır. "Daha iyi olabilir" fikri
   `README.md` `Sonra` listesine yazılır ve faz ilerler.
3. **Kod gerçeği önce gelir.** Bir faz, dokunacağı kod okunmadan başlamaz.
   Tasarımla kod çelişirse faz durur, çelişki `README.md` `Açık sorular`a
   yazılır.
4. **İstemci hesap yapmaz.** Ekranda görünen her toplam sunucudan gelir.
5. **Kullanıcıya soru az ve net.** Yalnız kullanıcının gördüğü davranışı
   değiştiren ve tasarımda yazmayan şey sorulur; tek cümle, öneriyle birlikte.
6. **Kapsam sızmaz.** Bu işin dışındaki bir kusur görülürse düzeltilmez,
   `docs/backlog.md`ye yazılır (tek satırlık kesin hata hariç).
7. **Her oturum `README.md`yi günceller**: durum tablosu, günlük, sıradaki tek
   görev. Ayrı devir notu açılmaz.

## Fazlar

| Faz | İş | Tahmini oturum | Çıktı |
|---|---|---|---|
| 0 | Karar kapısı | 1 (bu oturum + onay) | Onaylı tasarım, ADR 0022 |
| 1 | Kod gerçeği ve sunucu tasarımı | 1 | `04-SUNUCU-TASARIMI.md` |
| 2 | Sunucu: kişinin hesabı genişler | 2 | S1–S5, migration, yedek |
| 3 | Sunucu: okumalar | 1 | S6–S8 |
| 4 | Ekran tasarımı | 1–2 | `05-EKRAN-TASARIMI.md`, kareler |
| 5 | Flutter: `Borç ve alacak` listesi | 1 | §6.1 |
| 6 | Flutter: kişi sayfası, tahsilat ve ödeme | 1–2 | §6.2, §6.3 |
| 7 | Flutter: yeni kayıt ve kayıt sayfası | 1–2 | §6.4, §6.5 |
| 8 | Eski yolların kalkması ve bağlantılar | 1 | §6.6, S9 |
| 9 | Kabul turu ve kapanış | 1 | Senaryo kanıtı, belgeler |

Sunucu fazları ekran değiştirmez; Flutter fazları sunucuya dokunmaz (eksik bir
alan çıkarsa o fazın içinde, sözleşme belgesiyle birlikte eklenir).

### Faz 0 — Karar kapısı

- **Giriş:** araştırma, tasarım ve plan yazıldı (16 Kasım).
- **İş:** kullanıcı `02-TASARIM.md` §2 ve §3'ü okur; itiraz ettiği karar
  varsa yalnız o karar konuşulur. Sonra:
  - ADR 0022 yazılır (iki katmanlı; ilkeler K1–K8, başlangıç tasarımı §4–§7).
    ADR 0020 ve 0021'in başına hangi maddelerinin değiştiği notu düşülür.
  - `stages/06.3-butunsel-duzenleme.md` Grup 8 bu klasörü gösterir;
    `PROJECT-ROADMAP.md` ADR tablosu ve `CLAUDE.md` güncellenir.
  - PRD §6.6–6.7 kullanıcı onayıyla bu modele göre düzeltilir.
- **Çıkış:** ADR 0022 `Kabul edildi`. Kod değişmedi.

### Faz 1 — Kod gerçeği ve sunucu tasarımı

- **Okunacaklar:** `Domain/Debt.cs`, `Counterparty*.cs`; `Application/Debts`,
  `Application/Counterparties`, `Application/Obligations`; ilgili
  `Infrastructure` okumaları (`DebtBook`, `CounterpartyBalanceSummary`,
  `RecognizedItems`, `AccountMovements`, `CardDebt`, planlanan projection, net
  varlık); gün sonunun vadeli satış ve tahsilat okumaları (`DayClosePlan`);
  fişin "henüz ödemedim" yolu; yedek dışa ve içe aktarma; ilgili testler.
- **Cevaplanacak teknik sorular:**
  1. Borç para kaydı borçlandırmaya tür olarak mı, ayrı tablo olarak mı
     girer? Ölçüt: gecikme okuması, akış, yedek ve iptal yollarında en az
     değişiklik.
  2. Kapsam kuralı (K3) borçlandırmada hangi yolları etkiler; gün sonu
     süzgeci nerede uygulanır?
  3. Hesaba kartla ödeme `CardDebt`e hangi kaynak olarak girer?
  4. Vazgeçme kaydı hangi tabloda durur; iptali neyi geri alır?
  5. Kişili taksitsiz `Debt` açılışı sunucuda reddedilir mi, yalnız istemci mi
     açmaz (K9)? Yedek geri yükleme her iki durumda da okumalı.
  6. `/obligations` uçlarını hâlâ kim çağırıyor (fiş yolu, hatırlatma, gün
     sonu)?
  7. Hatırlatmalar kişinin hesabındaki vadeleri nasıl okuyor?
  8. Kişinin türü (K3) eski kişilerde nasıl okunur; listeler türü boş kişiyi
     nerede gösterir?
  9. Faizli borç bugün anapara ve faizi nasıl dağıtıyor; kullanıcının girdiği
     banka planıyla (K17) farkı ne?
- **Çıktı:** `04-SUNUCU-TASARIMI.md`: tablo ve kolon değişiklikleri, migration
  adımları (kuralları `AGENTS.md`), uçlar ve hata kodları, etkilenen okumalar,
  test listesi (senaryo numaralarıyla), yedek sürümü.
- **Çıkış:** belge yazıldı; Faz 2 ve 3 işleri oradan sırayla okunabiliyor.
  Tasarımla çelişki varsa kullanıcıya getirildi.

### Faz 2 — Sunucu: kişinin hesabı genişler

- **İş:** S1 (borç para, avans), S2 (kartla ödeme), S3 (istek kimliği), S4
  (kişinin türü, kapsam), S5 (vazgeçme, kalanı indirim sayma), S5b (kredi); migration; yedek sürümü; para tarafının tek listeleri.
- **Testler:** Domain, Application ve API seviyesinde; senaryo 1–7, 10–14, 16,
  17. `MoneyFoundation_*` genişler. Gün sonu testleri değişmeden geçer.
  Gerçek SQL koşusu (`BUSINESS_FINANCE_SQL_TEST_CONNECTION`).
- **Belgeler:** API sözleşmesi (`debt-api-contract.md` ya da yeni belge),
  `permissions.md`, `architecture.md`, `tests.md`, `restore-runbook.md`.
- **Çıkış:** build, format, tam test (SQL dahil) geçti; ekran değişmedi; eski
  istemci çalışmaya devam ediyor.
- İki oturuma bölünürse sınır: önce S1, S3, S4 (+ migration), sonra S2, S5 ve
  yedek.

### Faz 3 — Sunucu: okumalar

- **İş:** S6 (kişi sayfası), S7 (liste), S8 (fiş yönlendirmesi), S11
  (hesaptaki vadelerin planlananlara girmesi).
- **Kural:** bölüm (`Geciken / Yaklaşan / Diğer`), sıra, gecikmiş tutar,
  sıradaki vade ve kısayol tutarları (`Tamamı`, `Gecikmiş`) sunucudan gelir.
- **Testler:** senaryo 5, 8, 9, 15; sorgu sayısı ölçümü (liste ve kişi sayfası
  sabit sayıda sorguyla); sahiplik izolasyonu.
- **Çıkış:** iki okuma sözleşmesi belgede, testler geçti.

### Faz 4 — Ekran tasarımı

Kullanıcının ölçütü (16 Kasım): hazır bileşenlerin hızlıca dizildiği ekran
değil; üstüne düşünülmüş, büyük uygulama geliştiricilerinin düzeyinde tasarım.

- **Başvuru incelemesi:** aynı işi yapan ekranların nasıl kurulduğu — Apple
  Wallet (Apple Card taksitleri, ödeme akışı), Apple'ın tasarım yönergeleri
  (liste, ayrıntı, panel, veri girişi), büyük banka ve ödeme uygulamaları
  (borç ve kredi ayrıntısı, taksit takvimi), veresiye uygulamaları (kişi
  sayfası, iki eylem). Her ekran için alınan ve alınmayan kalıp gerekçesiyle
  yazılır.
- **Ekran tarifi (`05-EKRAN-TASARIMI.md`):** her ekran için bilgi hiyerarşisi
  (ilk bakışta ne okunur, ikinci bakışta ne), yerleşim, kullanılan ortak
  bileşen ve ölçü adları, her durum (yükleniyor, boş, sonuç yok, hata, eski
  veri, uzun ad, büyük tutar, büyük yazı), dokunma hedefleri, erişilebilirlik
  metinleri, hareket.
- **Ölçü kuralı:** ortak ölçüler temeldir. Bir yerde ortak ölçü daha kötü
  sonuç veriyorsa daha iyisi seçilir; ekrana özel sayı yazılmaz, kurala yeni
  ad eklenir ve tarifin "Ortak kuraldan ayrılanlar" tablosuna gerekçesiyle
  yazılır.
- **Kareler:** ekranlar örnek veriyle (tek tutarlı veri seti; sayılar
  birbirini tutar) kurulur ve ekran karesi testleriyle
  `tasarim-onizleme/borc-alacak-<tarih>/` altına çıkarılır: 412 ve 360 dp,
  açık ve karanlık tema.
- **Denetim listesi (her ekran):** tek ana eylem var mı; ilk bakışta en
  önemli sayı okunuyor mu; hiçbir bilgi iki yerde tekrar etmiyor mu; tek
  kelime taşması yok mu; renk tek başına anlam taşımıyor mu; boş ve hata
  durumu çizildi mi; metinler doğal Türkçe mi.
- **Dış göz denetimi (kullanıcı kararı, 16 Kasım: denetimi Claude Code yapar,
  kullanıcı tek tek bakmaz):** kareler çıktıktan sonra ekranlar tasarlayanın
  gözüyle değil, ilk kez gören birinin gözüyle yeniden incelenir. Yöntem:
  kareler dosyadan açılıp bakılır (koddan tahmin edilmez); her kare denetim
  listesinden ve başvuru ekranlarıyla yan yana karşılaştırmadan geçer;
  bulgular `05-EKRAN-TASARIMI.md` "Denetim bulguları" tablosuna önem
  derecesiyle yazılır (kusur / zayıf / tercih); kusur ve zayıf olanlar
  düzeltilir ve kare yeniden çıkarılır; en az iki tur yapılır, ikinci turda
  kusur kalmamalıdır. Düzeltilmeyen her bulgu gerekçesiyle tabloda kalır.
- **Kullanıcıya rapor:** son kareler, alınan başvuru kalıpları, ortak kuraldan
  ayrılanlar ve düzeltilmeyen bulgular tek raporda verilir; onay beklenmez,
  itiraz gelirse o ekran yeniden açılır.
- **Araçlar:** bu faz için gereken beceri (skill) ve araçlar eklenebilir
  (kullanıcı izni, 16 Kasım).
- **Çıkış:** tarif yazılı, kareler çıkmış, denetimin ikinci turunda kusur yok. Bu fazda yazılan ekran kodu Faz 5–7'nin
  başlangıcıdır; sunucuya bağlama o fazlarda yapılır.

### Faz 5 — Flutter: `Borç ve alacak` listesi

- **İş:** §6.1. Yeni özellik klasörü; eski `debts`, `counterparties`,
  `obligations` sayfaları bu fazda **silinmez**, yeni sayfa yanlarında kurulur
  ve `Diğer` menüsünden açılır.
- **Durumlar:** yükleniyor, boş (ilk kullanım), sonuç yok (arama, anahtar),
  hata, oturum düştü, eski veri.
- **Testler:** bileşen testleri; 320–800 dp ve 1.0–2.0 yazı ölçeğinde taşma;
  açık ve karanlık tema.
- **Denetim:** Faz 4'teki dış göz denetimi gerçek veriyle bağlanmış ekranda
  yinelenir (emülatörde, gerçek API ile); kullanıcıya rapor verilir.
- **Çıkış:** analyze, format, tam test; `design-system.md`, `flows.md`.

### Faz 6 — Flutter: kişi sayfası, tahsilat ve ödeme

- **İş:** §6.2, §6.3. Hareketlerin sayfalı okunması, ay seçici, ortak işlem
  ayrıntısı paneli, iptal ve indirim.
- **`FinancialDataChanges`:** tahsilat ve ödeme `counterparties`, `accounts`,
  `activityFeed`, `dashboard`, gerekiyorsa `cards` ve `cash` hedeflerini
  yükseltir; bütçeyi yükseltmez.
- **Testler ve kapı:** Faz 5 ile aynı kalıp; fazla ödeme uyarısı, POS ve kart
  yolları.

### Faz 7 — Flutter: yeni kayıt ve kayıt sayfası

- **İş:** §6.4 (dört form, `Taksitli` ve `Uygulamadan önce` anahtarları, kişi
  seçici) ve §6.5 (taksitli ve kişisiz kaydın sayfası; kısmi ödeme, geri alma,
  plan değişikliği, indirim, iptal).
- **Testler ve kapı:** aynı kalıp; kapsam bölümünün üç hâli; takvim önizlemesi.

### Faz 8 — Eski yolların kalkması ve bağlantılar

- **İş:** §6.6 ve S9. `İşlem ekle` girişleri, akıştaki tür adları, fiş
  "henüz ödemedim" yolu, hatırlatmalar; eski sayfa ve denetleyicilerin, eski
  uçların ve testlerinin silinmesi; `Cari hesap` menüsü ve ön ayarı.
- **Çıkış:** kodda `obligations` özelliği ve eski borç sayfası kalmadı; ölü
  kod taraması temiz; tam testler.

### Faz 9 — Kabul turu ve kapanış

- **İş:** §9'daki senaryolar gerçek API + SQL + emülatörde; yedek alıp geri
  yükleme; eski sürüm yedeğinin okunması.
- **Belgeler:** `flows.md`, `tests.md`, `design-system.md`, `permissions.md`,
  `architecture.md`, sözleşme belgeleri, `restore-runbook.md`, `CLAUDE.md`
  özeti, `docs/project-status.md`; `06.3-butunsel-duzenleme.md` Grup 8'in
  kapanışı; `design/brifler/07` ve `design/borc-ve-alacaklar/` için "yerini bu
  klasöre bıraktı" notu.
- **Çıkış:** senaryoların hepsi denendi, sonuçları sayılarıyla raporlandı;
  kullanıcı son hâli gördü.

## Doğrulama komutları (her faz sonunda)

```bash
dotnet build BusinessFinance.slnx --configuration Release --no-restore
dotnet format BusinessFinance.slnx --verify-no-changes --no-restore
dotnet test BusinessFinance.slnx --configuration Release --no-restore
# mobile/business_finance_mobile içinden
flutter analyze
flutter test
dart format --set-exit-if-changed lib test
```

Sunucu fazlarında SQL'li koşu ayrıca yapılır; bağlantı `.env`'den okunur,
ekrana yazılmaz.

## Oturum başlangıç ve bitiş listesi

**Başlarken:**

1. `AGENTS.md`, `docs/project-status.md`, `stages/README.md`.
2. Bu klasörün `README.md`si: durum tablosu, günlüğün son kaydı, sıradaki tek
   görev, açık sorular.
3. Sıradaki fazın bölümü (bu belge) ve `02-TASARIM.md`de ilgili bölüm; sunucu
   fazlarında `04-SUNUCU-TASARIMI.md`.
4. `git status --short --branch`, `git log -5 --oneline`.

**Bitirirken:**

1. Doğrulama komutları; sonuç sayıları `README.md` günlüğüne.
2. Faz bittiyse commit (İngilizce, tipi dürüst, imzasız).
3. `README.md`: durum tablosu, günlük kaydı (ne yapıldı, ne yapılmadı, neye
   bakılmadı), sıradaki tek görev.
4. Açtığım API ve emülatör kapatılır.
