# Kasa ve POS: esnafın gün sonu — karar belgesi

**Durum:** taslak, 28 Eylül 2026 — **kısmen aşıldı.** Kullanıcı aynı gün: banka sayımı alınmadı
(§4.4, K5 düştü); yatış kesintisi İşlemler detayında görünecek; CSV ve dekont kapıları bu tura girmiyor;
Kasa şahsi cüzdanı göstermeyecek; KDV gün sonunda sorulmayacak. Güncel çerçeve ve açık kararlar
`research/para-akisi/HARITA.md` belgesindedir; bu belge gerekçe ve bütünlük tablosu için okunur.
**Tema kapandı:** geçerli kararlar `KAPANIS.md` (KP1–KP21).

**Girdiler:** `ARASTIRMA-PLANI.md`, `BULGULAR.md` (U1–U13, H1–H16, T1–T9), PRD §6.3 ve §6.8,
ADR 0013, 0014, 0015, 0016.

---

## 0 · Çerçeve: kullanıcının iki ilkesi (28 Eylül 2026)

| # | İlke | Belgede nasıl uygulanıyor |
|---|---|---|
| İ1 | **Bütünlük:** yapılan değişiklik uygulamanın diğer özellikleriyle çatışmaz, uyum içinde çalışır | Her önerinin §5'te "hangi mevcut özelliğe dokunuyor, nasıl uyuşuyor" satırı var. Yeni kavram, var olanın ikinci kopyası olarak kurulmaz; mümkünse var olan kayıtları üretir |
| İ2 | **Kapsam:** biz bir işletme bütçe uygulamasıyız | Kasa "param nerede, bugün ne kazandım, ne bekliyorum, kasa ve banka tuttu mu" sorularını cevaplar. Muhasebe defteri, stok, adisyon, satış satış kayıt ve vergi hesabı kapsam dışıdır (PRD §2, §17). §7 bu sınırı madde madde yazar |

Hedef kullanıcı PRD §3'teki iki profil: **tezgâh üstü** (bakkal, kafe, berber; nakit + POS + gün sonu)
ve **faturalı iş** (tamirci, toptancı; cari ve vadeli tahsilat). Kasa sekmesi birincisinin günlük
ritmidir; ikincisini bozmamalıdır.

## 1 · Sorun: 13 bulgu, dört kök neden

Bulgular tek tek yamanırsa Kasa bir özellik yığını olarak kalır. Hepsi dört kök nedene iniyor:

| Kök neden | Bulgular | Belirti |
|---|---|---|
| **N1 · "Gün" kavramı yok** | U1, U9, U10, T9 | Nakit satış tek tek girilmezse Kasa "fazla" der; Kasa ile POS birbirinden habersiz; sekme "bugün ne oldu" sorusunu cevaplamaz |
| **N2 · Banka tarafında kontrol noktası yok** | U2, U3, U8, PRD §6.3 | Gerçek yatan tutar yazılamaz, toplu yatış eşleşmez, ekstre içe aktarımı POS'u ikinci kez gelir sayar; PRD'deki "hesap mutabakatı" hiç yapılmamış |
| **N3 · POS girişi tekrarlı ve düzeltilemez** | U12, U13, U6, sahte "Gecikti" | Her akşam aynı beş seçim; oran girilince net görünmez; yanlış kayıt kalıcı; iş günü yok |
| **N4 · Görünürlük** | U7, U11 | Beklenen tahsilat yaklaşanlarda yok; nakit kayıt Kasa'yı yenilemiyor |

## 2 · Hedef: Kasa sekmesi ne cevaplamalı

Tezgâh üstü esnafın akşam sorduğu dört soru:

1. **Bugün ne sattım?** Nakit ve kartla, toplam.
2. **Kasa tuttu mu?** Sayılan ile olması gereken.
3. **Bankaya ne gelecek, ne geldi?** Yoldaki POS, bugün yatan, eksik yatan.
4. **Banka tuttu mu?** Uygulamadaki bakiye ile bankanın söylediği.

Bugünkü sekme yalnız 2'yi ve 3'ün yarısını cevaplıyor.

## 3 · Model seçenekleri

| | A · Yamalar | B · Gün sonu | C · Gün sonu + banka mutabakatı |
|---|---|---|---|
| Ne | Bulguları tek tek düzelt: iptal, canlı net, iş günü, yaklaşanlar, yenilenme | A + **gün sonu girişi** (nakit ve kart satış toplamı tek adımda) + **POS tanımı** | B + **yatış kaydı** (gerçekleşen tutar, toplu eşleştirme, fark) + **banka sayımı** |
| Çözdüğü kök neden | N3, N4 | N1, N3, N4 | N1–N4 |
| Kazandırdığı | En düşük maliyet; hiçbir kavram değişmez | Kasa gerçek hayatta çalışır; akşam girişi tek adıma iner | Banka tarafı da güvenilir olur; PRD §6.3 kapanır; CSV/dekont ve ileride entegrasyon aynı kapıdan girer |
| Kaybettirdiği | U1 kalır: Kasa, satış girmeyen esnafta yanlış konuşmaya devam eder. Sekme hâlâ alt çubuğu hak etmez | Banka tarafı "yazdığın kadar" doğru kalır; BSMV ve toplu yatış farkı görünmez, U8 çifte sayımı yalnız bir uyarıyla kapanır | En büyük iş: yeni kayıt türü, migration, yedek sürümü, yeni ADR |
| Yeni kayıt türü | Yok | POS tanımı | POS tanımı + yatış |

**Önerim: C, ama sade sürümüyle ve parça parça (§8).** B tek başına Kasa'yı düzeltir ama banka
tarafını bugünkü gibi bırakır; oysa esnafın parasının büyük kısmı kartla geliyor ve bankaya eksik
yatan tutar bugün uygulamada **hiçbir yere yazılamıyor**. C'nin maliyetini düşüren şey, yeni kayıtların
çoğunun **var olan kayıtları üretmesi** (§4).

## 4 · Önerilen yapı (C, sade sürüm)

### 4.1 · Gün sonu girişi — yeni kayıt değil, var olanları üreten bir eylem

Esnaf akşam Z raporuna ya da kendi hesabına bakıp tek ekranda şunları girer:
- **Nakit satış toplamı** → var olan `BudgetTransaction` (gelir, kasa hesabına, satış kategorisine).
- **Kartlı satış toplamı** (POS tanımı başına bir satır) → var olan `PosSettlement` (POS tanımından
  hesap, oran, komisyon kategorisi ve beklenen gün dolar).
- İsteğe bağlı: **kasayı say** → var olan `CashCount`. Satış önce girildiği için fark artık gerçek
  açık/fazladır (U1 kapanır).

Hepsi tek `SaveChanges` sınırında ve **(kullanıcı, gün, kasa)** anahtarıyla idempotent yazılır; tekrar
basmak ikinci satış üretmez (PRD başarı ölçütü). Yeni bir "gün sonu" tablosu gerekip gerekmediği
açık karardır (§9, K3); önerim yalnız bağlayıcı bir kimlik.

**Neden var olan kayıtlar:** raporlar, işletme neti, birleşik akış, bütçe, muhasebeci paketi, kapsam
filtresi ve yedek, bu kayıtları bugün zaten doğru okuyor. Yeni bir satış türü açmak bunların hepsine
ikinci bir yol eklemek olurdu (İ1).

### 4.2 · POS tanımı — tek yeni ayar kaydı

Kullanıcının bir kez girdiği ayar: **ad** (ör. "Ziraat POS", "Multinet"), **tür** (banka POS · yemek
kartı · ödeme kuruluşu), **geçeceği hesap**, **varsayılan oran**, **komisyon kategorisi**, **geçiş süresi**
(gün sayısı + takvim/iş günü).

- Formu doldurur; kayıt yine tutarı saklar, oranı saklamaz (`PosSettlement`in bugünkü kuralı korunur).
  Tanımdaki oran bir varsayılandır, kaydın gerçeği değil; ADR 0016'nın "oran kullanıcınındır" ilkesiyle uyumlu.
- İş günü seçeneği beklenen günü hafta sonunu atlayarak hesaplar; **tatil takvimi yok**, gün her zaman
  elle düzeltilebilir. Sahte "Gecikti" kapanır.
- Yemek kartı ayrı özellik değil, tanımın bir türüdür (T5).
- Akşam girişi ~15 dokunuştan "tutar + kaydet"e iner (U13).

### 4.3 · Yatış kaydı — "hesaba geçti"nin yerine

Bugünkü `Hesaba geçti` (yalnız tarih) yerine: **yatan tutar + yatış günü + hangi yoldaki POS'ları
kapattığı** (bir ya da birden çok).
- Yatan tutar beklenenden farklıysa fark tek satırla yazılır: eksikse gider (varsayılan: POS tanımının
  komisyon kategorisi; BSMV, kesinti), fazlaysa gelir. Ayrı BSMV alanı yok (U2, değer sınavı).
- Birden çok günü tek yatışla kapatır (U3).
- Geri alınabilir: yatış iptal edilirse kapattığı POS'lar yeniden "yolda" olur (U12'nin ikinci yarısı).
- ADR 0014 ile uyumu: yatış **taşır**; bakiyeyi değiştirir, gelir/gider üretmez. Yalnız fark satırı
  tanır ve o da ayrı bir kayıttır, tıpkı dekonttaki işlem ücreti gibi (flows.md).

**Yatışın üç giriş kapısı, tek kayıt:**
1. Kasa sekmesinin banka tarafı (elle).
2. **Dekont okuma** karar sayfası ("Bu tutar ne?") → yeni seçenek "POS yatışı".
3. **CSV içe aktarım** → satırı "POS yatışı" diye işaretleme; o satır gelir yazılmaz, yatış olur.
   **U8 çifte sayımı böyle kapanır.**

İleride bir entegrasyon gelirse dördüncü kapı olur; model değişmez (BULGULAR §2-F).

### 4.4 · Banka sayımı — kasa sayımının genişlemesi

PRD §6.3 "hesap mutabakatı". Yeni kavram değil: **`CashCount` banka hesaplarına genişler.** Kullanıcı
banka uygulamasındaki bakiyeyi yazar, uygulama farkı ve olası sebebini (yoldaki POS'lardan hangisi
yatmış olabilir) gösterir.
- Banka farkı **otomatik kayıt önermez**: bankadaki fark çoğu zaman girilmemiş bir harcamadır, kasa
  açığı değildir. Önerilen eylem "eksik kaydı bul" ve yatış eşleştirmesidir.
- Açık karar: sayım kaydı saklanır mı, anlık karşılaştırma mı (K5).

### 4.5 · Hatalar ve görünürlük (hangi seçenek olursa olsun)

- **U12:** POS iptali (domain'de var, API ve ekran yok) ve yatışın geri alınması.
- **U11:** `transactionsChanged` / `transferChanged` Kasa'yı da yeniler.
- **U10:** kapanmış sayım, sonradan girilen hareketler bakiyesini değiştirdiyse "sayımdan sonra
  değişti" der; "oturdu" demeye devam etmez.
- **U7:** beklenen POS tahsilatı kanonik planlanan projection'a **giriş** olarak katılır; yaklaşanlar
  "7 günde girecek" satırını da gösterir. `IUpcomingPaymentRepository` ikinci sorgu tutmaz kuralı korunur.
- **U6:** formda canlı komisyon ve net.

## 5 · Bütünlük tablosu (İ1)

| Mevcut özellik | Nasıl dokunuyor | Uyum koşulu |
|---|---|---|
| Gelir/gider (`BudgetTransaction`) | Gün sonu nakit satışı bir gelir kaydıdır | Kayıt adı "Nakit satış · 28 Eylül"; kapsam zinciri (`TransactionScopeResolution`) aynen |
| POS tahsilatı (`PosSettlement`) | Gün sonu kart satışı bir POS kaydıdır; yatış `TransferredOn`un yerini alır | Mevcut kayıtların `TransferredOn` değeri, yatış kaydına tek yönlü taşınır ya da ikisi yan yana okunur — migration kararı (K6) |
| Kasa sayımı (`CashCount`) | Aynı kayıt, banka hesaplarına genişler | Nakit farkı gelir/gider önerir; banka farkı önermez |
| Transfer | Bankaya nakit yatırma ve şahsi çekim bugünkü gibi transferdir | Gün sonu ekranında kısayol olabilir; yeni kayıt türü değil |
| Birleşik akış ve kanonik planlanan projection | Yatış yeni bir satır türüdür; beklenen POS planlanana girer | `activityKind` / `effect` / `sourceGroup` boyutlarına eklenir; `canCancel` kuralı yatışı kapsar |
| Dekont okuma | "Bu tutar ne?" sayfasına "POS yatışı" seçeneği | Okuma bir öneri katmanıdır (ADR 0011); eşleştirmeyi kullanıcı onaylar |
| CSV içe aktarma | Satır "POS yatışı" olarak işaretlenebilir | İşaretli satır gelir yazmaz — U8 kapanır |
| Cari hesap | **Çatışma adayı:** müşteri veresiye borcunu kartla öderse para yoldadır ama gelir zaten borçlandırmada tanındı. Bugünkü `PosSettlement` her zaman gelir tanır, yani bu yolu kullanırsa gelir iki kez sayılır | Bu tur çözülmez; kartla cari tahsilat için "gelir tanımayan yoldaki para" gerekir. Açık karar K7 |
| Kredi kartı taksitleri (borç tarafı) | POS'ta taksit **ertelendi**. İleride açılırsa ADR 0015'teki ayrım gibi borç tarafının taksitiyle ad ve kavram çakışmayacak biçimde tasarlanır | Kasa tarafında "taksit" kelimesi bu turda kullanılmaz |
| KDV (ADR 0016) | Gün sonu satış kayıtları isteğe bağlı KDV taşır | Taşınır, hesaplanmaz. Z raporundaki oran kırılımı bu turda yok (K8) |
| Muhasebeci paketi | Değişmez; yeni satışlar ve fark satırları var olan kayıtlar olduğu için pakete kendiliğinden girer | Ayrı bir hesaplama yolu açılmaz |
| Kapsam (ADR 0013) | Satış, komisyon ve fark işletme kapsamındadır; yatış kapsam taşımaz (taşıyan kayıt) | Bakiye ve net varlık kapsamdan etkilenmez |
| `FinancialDataChanges` | Yatış ve gün sonu yeni olaylar | Her olay yalnız etkilediğini yükseltir |
| Yedek / geri yükleme | POS tanımı ve yatış yeni koleksiyonlar | Yedek şeması v10 → v11; `ExpectedAtCount` da bu sürüme girer |
| Tekrarlayan plan | POS'un aylık cihaz ücreti bugünkü gibi tekrarlayan giderdir | Değişmez |

## 6 · Kasa sekmesinin yapısı

| | A · Bugünkü gibi | B · "Gün" sekmesi | C · `Diğer` altına iner |
|---|---|---|---|
| İçerik | Sayım + POS listesi | Üstte **Bugün** kartı (nakit satış, kartlı satış, gün sonu durumu); altında **Nakit** (sayım) ve **Banka** (yolda, bugün geçecek, yatış, banka sayımı) | Aynı ekran, alt çubukta başka bir şey |
| Kazandırdığı | İş yok | Dört sorunun (§2) dördü tek ekranda; akşam ritmi tek yerde | Alt çubuk yeri başka bir özelliğe açılır |
| Kaybettirdiği | Sekme alt çubuğu hak etmez (kullanıcı gözlemi) | Tasarım ve uygulama işi | Tezgâh üstü esnafın her akşam kullandığı ekran bir dokunuş uzaklaşır |

**Önerim: B.** Sekmenin adı açık karar (K2): "Kasa" korunabilir ya da "Gün sonu" / "Bugün" olabilir.
ADR 0015'in kuralı korunur: üçüncü sekme onboarding ön ayarına göre değişir; kişisel profilde yine
`Bütçeler`dir.

## 7 · Kapsam sınırı (İ2): yapmayacaklarımız

| Konu | Neden değil |
|---|---|
| Satış satış kayıt, adisyon, ürün/stok | İşletme bütçesi, satış noktası yazılımı değil (PRD §17) |
| Z raporunun mali kaydı ve KDV beyanı | Muhasebecinin işi; biz taşırız, hesaplamayız (ADR 0016) |
| Kart türüne göre oran | Esnaf gün sonunda kart türü dağılımını ayrıca girmez; doldurulmayacak detay (D3) |
| Ayrı BSMV alanı | Yatıştaki fark satırı yakalıyor |
| Resmî tatil takvimi | Bakımı bize düşer; iş günü seçeneği ve elle düzeltme yeterli |
| Taksit taksit geçiş | Değeri sektöre göre değişiyor; açılırsa borç tarafının taksitiyle birlikte tasarlanır (§5) |
| Banka entegrasyonu | PRD §17'de kapsam dışı; model ona kapı bırakır, bağlantı kurmaz |
| Bloke çözüm (erken çekim) hesabı | Hesaplayan alan olurdu; gerekirse fark satırı yakalar |

## 8 · Değer sınavı ve önerilen sıra

| Sıra | Parça | D1 ihtiyaç | D2 en az detay | D3 doldurulur mu | Sonuç |
|---|---|---|---|---|---|
| 1 | Hatalar: POS iptali, Kasa yenilenmesi, CSV'de POS'u gelir yazmama uyarısı | Kesin (U8, U11, U12) | — | — | **Yap** |
| 2 | POS tanımı + form önden dolar + canlı net + iş günü | Güçlü (U13, U6, H7) | 6 alan, bir kez | Bir kez | **Yap** |
| 3 | Gün sonu girişi | Güçlü (U1, U9, muhasebe pratiği) | Nakit toplamı + POS başına toplam | Her akşam | **Yap** |
| 4 | Yatış kaydı (+ dekont ve CSV kapıları) | Güçlü (U2, U3, U8) | Tutar + gün + kapattığı POS'lar | Yatış günlerinde | **Sade sürümünü yap** |
| 5 | Kasa sekmesinin yeniden tasarımı (Claude Design) | Kullanıcı gözlemi, T9 | §6-B | Her gün | **Yap**, 2–4'ten sonra |
| 6 | Banka sayımı | Orta (PRD §6.3, H16 dolaylı) | Bakiye yaz, farkı gör | Haftada/ayda bir | **Sade sürümünü yap** |
| 7 | Beklenen POS yaklaşanlarda | Orta (U7) | — | — | **Yap** |

Her parça kendi başına çalışır ve kendi checkpoint'idir. 1 bugünkü modelde yapılır; 2–4 yeni ADR'yi
bekler.

## 9 · Açık kararlar

| # | Karar | Seçenekler | Önerim |
|---|---|---|---|
| **K1** | Model | A · yamalar · B · gün sonu · C · gün sonu + banka mutabakatı | **C, sade sürüm, §8 sırasıyla.** Banka tarafı esnafın parasının büyük kısmı; bugün eksik yatan tutar hiçbir yere yazılamıyor |
| **K2** | Sekme | A · bugünkü gibi · B · "Gün" sekmesi · C · `Diğer`e iner. Ad: "Kasa" / "Gün sonu" / "Bugün" | **B, adı "Kasa" kalsın.** Esnafın dilinde gün sonu "kasayı kapatmak"tır; ad değiştirmek alışkanlığı bozar |
| **K3** | Gün sonu kaydı ayrı tablo mu | A · yalnız ürettiği kayıtlar + bağlayıcı kimlik · B · ayrı `DailyClose` tablosu (Z toplamı, not, durum) | **A.** B, raporlara ikinci bir kaynak ekler; "bugün gün sonu yapıldı mı" sorusu bağlayıcı kimlikten cevaplanır |
| **K4** | Yatış farkı varsayılan kategorisi | A · POS tanımının komisyon kategorisi · B · her seferinde sor | **A**, değiştirilebilir |
| **K5** | Banka sayımı kalıcı mı | A · kasa sayımı gibi saklanır · B · anlık karşılaştırma, saklanmaz | **A.** Kasa sayımıyla aynı kayıt olursa geçmiş, yedek ve arayüz tek; ayrı bir yol açılmaz |
| **K6** | Mevcut `TransferredOn` verisi | A · yatış kaydına taşınır (her birine bir yatış) · B · eski alan okunmaya devam eder, yeni kayıtlar yatışla | **A.** İki yol yan yana yaşarsa her rapor iki yerden okur; yerel veri sentetik, taşıma yolu testle kanıtlanır |
| **K7** | Kartla cari tahsilat | A · bu tur kapsam dışı, kayıt · B · bu tur "gelir tanımayan yoldaki para" | **A.** Bütünlük riski gerçek ama ayrı bir tasarım ister; §5'te kayıtlı |
| **K8** | Gün sonunda KDV | A · yok · B · tek toplam KDV (isteğe bağlı) · C · oran kırılımı | **B.** Taşıma kuralına uyar, muhasebeci paketine girer; C, Z raporunu yeniden yazdırmak olur ve muhasebeci Z'yi zaten alıyor |
| **K9** | Belge ve sıra | Bu belge onaylanınca: ADR 0018 (ADR 0015'i genişletir: gün sonu, POS tanımı, yatış) + aşama belgesi (06.3 önerisi) + Claude Design brifi | Önce 1. parça (hatalar) mevcut modelde; ADR onayından sonra 2–4 |

## 10 · Dış göz için sorular

1. Gün sonu girişinin var olan kayıtları üretmesi (§4.1) doğru mu, yoksa ileride "bugün ne sattım"
   raporunu zorlaştırır mı?
2. Yatışın gelir/gider üretmeyip yalnız fark satırı yazması ADR 0014 ile tutarlı mı?
3. Banka sayımının kasa sayımının genişlemesi olması (§4.4) iki farklı fark anlamını karıştırır mı?
4. §7'deki kapsam sınırı fazla mı dar: hedef kullanıcı için vazgeçilmez bir şey dışarıda kalıyor mu?
