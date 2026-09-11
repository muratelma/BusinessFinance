# Uygulama Gözlem Formu — Wallet by BudgetBakers

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Wallet: Budget Expense Tracker / BudgetBakers |
| Sürüm | 9.3.6 (`versionCode=90164`) · Faz 2'de aynı sürüm, kurulum 2026-09-07 |
| Test tarihi | 1 Eylül 2026 (Tur 1, eski PC) · **10 Eylül 2026 (Faz 2 boşluk koşumu, yeni emülatör)** |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | **İngilizce** / TRY — Wallet sistem dilini (Türkçe) almıyor, uygulama arayüzü İngilizce açılıyor; yalnız Android sistem diyalogları (bildirim izni vb.) Türkçe. Money Manager'ın aksine. |
| Hesap veya plan türü | Ücretsiz, daha önce deneme için açılmış BudgetBakers hesabı (`My Wallet`) — Faz 2'de yeni emülatörde e-posta/Google ile **kullanıcı giriş yaptı**, bulut verisi (Tur 1 çekirdek 5 işlemi + 3 hesap) geri yüklendi |
| Erişim kısıtı | Asıl manuel kayıt ve rapor akışlarında yok; banka senkronizasyonu, "Create Automatic Rule", 6M/1Y rapor aralığı ve bazı özellikler Premium (🔒). Faz 2'de **cihaz rehberi izni** reddedilince "I Lent/I Borrowed" borç formu kapanıyor (emülatör tuzağı — özellik yok değil). |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Kısmi | — | Önceden deneme için açılmış hesap doğrudan kullanılabildi; sentetik kayıtlar buluta senkronize hesaba eklendi | Kayıt/onboarding bu turda görülmedi; uygulama kullanıcı adı ve `My Wallet` çalışma alanını gösteriyor. Play Store test sırasında yüklü 9.3.6 için güncelleme bulunduğunu gösterdi | `00-magaza.png`, `02b-menu.png` |
| K01 Ana ekran | Tamamlandı | 0 | Hesaplar yatay kartlarda ve bakiyeleriyle ilk blokta; `Accounts` ile `Budgets & Goals` aynı ana yüzeyde | Hesapların altında büyük Premium, banka bağlantısı ve çapraz ürün tanıtımları asıl içeriği aşağı itiyor | `03-dolu-ana-ekran.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı | ~6/hesap | Manual Input altında Cash, Checking account, Credit card, Savings, Loan, Mortgage ve overdraft dahil açık hesap türleri var; kartta limit, son ödeme tarihi, gösterilecek bakiye ve başlangıç değeri alanları bulunuyor | Cash/checking oluştururken açılış bakiyesi alanı yok; bu iki hesap protokol gereği sıfır açıldı. Checking hesabı kaydedilince veri içe aktarmayı etkinleştirmek için hesaba e-posta gönderildi | `03-dolu-ana-ekran.png` |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~12 | Hızlı form tutar + hesap + kategoriyle çok kısa; ayrıntıda tarih, not, payer, label ve ödeme türü eklenebiliyor | Form varsayılan olarak **Expense** açılıyor; Income seçimi yalnız sekme rengiyle anlaşılıyor. Hizmet geliri için işletme sınıfı yok, en yakın kategori `Sale`; ayrıntılar kayıt sonrası ikinci ekranda tamamlandı | `04-islem-formu.png` |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~10 | `Groceries` gibi iki katmanlı kategori ve notta serbest metin var; arama not içindeki `Market` sözcüğünü buldu | **İşletme/şahsi kapsam alanı yok.** Label veya kategoriyle taklit edilmedi; protokole göre bu boyut `Desteklenmiyor` | `05-siniflandirma.png` |
| K05 İşletme kart gideri | Tamamlandı | ~10 | Gider kart hesabına yazıldığı anda kart bakiyesi `-TRY 1,200`; Spending raporunda aynı gün gider oldu. Kart hesabı ayrıca limit ve vade alanı taşıyor | İşletme kapsamı yok; tedarikçi yalnız `Payee`, açıklama ise `Note` alanında tutuluyor | `05-siniflandirma.png`, `06-islem-listesi.png` |
| K06 Transfer | Tamamlandı | ~7 | Ayrı Transfer sekmesi var; From/To açıkça seçiliyor. Ana Hesap→Ortak Cüzdan ve Ana Hesap→İş Kartı iki bağlı satır (+/−) olarak görünse de toplam ve cash-flow etkisi sıfır | Liste aynı transferi iki satırda gösterdiği için ilk bakışta çift kayıt izlenimi verebiliyor; form yeni açıldığında son kullanılan yönü korumadı ve hesaplar yeniden seçildi | `06-islem-listesi.png` |
| K07 Liste ve rapor | Tamamlandı | ~4 | Liste haftalara ayrılıyor; arama not metnini buluyor. Cash-flow tam `Income TRY 25,000`, `Expenses -TRY 2,050`, net `TRY 22,950` gösterdi; transferler ve kart ödemesi yeniden gelir/gider sayılmadı. Spending kategori dağılımı ve en büyük giderleri gösteriyor | Business/personal kırılımı yok; Records menüsündeki seçenekler yalnız “her kayıtta bakiye” ve “planlananları göster”, gelişmiş filtre görülmedi | `06-islem-listesi.png`, `07-rapor.png` |
| K08 Düzeltme/iptal | Tamamlandı | ~3 | Satır → Record detail ile alanlar doğrudan değiştiriliyor; `Split` eylemi de aynı üst çubukta | Düzenleme geçmişi korumadan mevcut kaydı değiştiriyor. Delete tek Yes/No onayından sonra kalıcı; void/iptal veya geri alma yok | Record detail manuel gözlemi |

## Kontrol değeri doğrulaması

Wallet cash/checking hesaplarında açılış bakiyesi kabul etmediği için kontrol,
yalnız çekirdek işlemlerin ürettiği değerlere uyarlandı. Bu fark ürün davranışı
olarak kaydedildi; gelir işlemiyle yapay açılış bakiyesi oluşturulmadı.

| Değer | İşlem-only beklenen | Wallet | Durum |
|---|---:|---:|---|
| Ana Hesap bakiyesi | 20.800,00 | TRY 20,800.00 | ✓ |
| Ortak Cüzdan bakiyesi | 2.150,00 | TRY 2,150.00 | ✓ |
| İş Kartı bakiyesi/borcu | 0,00 | TRY 0 | ✓ |
| Toplam net bakiye | 22.950,00 | TRY 22,950.00 | ✓ |
| Gelir toplamı | 25.000,00 | TRY 25,000.00 | ✓ |
| Gider toplamı | 2.050,00 | TRY 2,050.00 | ✓ |

**Faz 2 (10 Eyl 2026)** — aynı 5 çekirdek işlem bulutta korunmuştu ve tekrar
doğrulandı (Records `12`, cash-flow `11`): Ana Hesap ₺20.800 · Ortak Cuzdan ₺2.150 ·
İş Kartı ₺0 · net ₺22.950 · Ağustos gelir ₺25.000 / gider ₺2.050 — birebir. B1/B2
bu doğrulamadan **sonra** eklendi; sonraki durum "Faz 2 boşluk koşumu" bölümünün
sonundadır (net ₺16.350).

## Arayüz taraması (görev dışı, ~10 dk)

| Alan | Gezildi mi | Kısa gözlem |
|---|---|---|
| Tüm ana sekmeler / alt görünümler | ✓ | Yan menü çok geniş: Home, Records, Investments, Statistics, Planned payments, Budgets, Debts, Goals, Shopping lists, Warranties, Loyalty cards, Currency rates, Group sharing ve Others. Statistics içinde Balance, Outlook, Cash-flow, Spending, Credit, Reports ve Assets sekmeleri var | `02b-menu.png` |
| Bir raporun içine tıklama | Kısmen | Cash-flow ve Spending açıldı; Spending kategori/label geçişi, trend ve Top 5 expenses taşıyor. `Go deeper` ayrıntısı açılmadı | `05-siniflandirma.png`, `07-rapor.png` |
| Bütçe / hedef / planlama ekranı | ✓ (boş durum) | Planned payments gelecekteki tahsilat/gideri anlatıp `+` yönlendirmesi yapıyor; Budgets doğrudan `Create Budget`; Debts Active/Closed, Goals Active/Paused/Reached sekmeli | `09-ozgun-ozellik.png` |
| Ayarların derinliği | Kısmen | Menü seviyesinde Dark mode, Hide Amounts, Currency rates, Group sharing, Help ve Settings görüldü; ayarların içine girilmedi | — |
| Arama ve filtre davranışı | ✓ | Serbest metin araması not içindeki `Market` ifadesiyle tek kaydı buldu. Records seçeneklerinde gelişmiş filtre görülmedi | — |
| Boş durum ekranları | ✓ | Planned payments, Budgets, Debts ve Goals boş durumları ne işe yaradığını açıklıyor ve ilk eylemi söylüyor; Money Manager'daki yalnız “No data” yaklaşımından daha yönlendirici | `09-ozgun-ozellik.png` |
| Hata / uç durum | ✓ | Sıfır tutarda kayıt engellendi ve alt toast/snackbar ile “Please fill in the amount.” dendi; alan içi hata yok. İlk yanlış tipte girilen sentetik kayıt Delete→Yes ile tamamen silindi | `08-hata-veya-bos-durum.png` |
| Widget / hızlı giriş / kısayol | Kısmen | Her ana ekranda bağlama göre mavi `+` var; cihaz widget'ı denenmedi | — |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Home hesap bakiyelerini önce gösteriyor; raporlar soru başlığı (“Am I spending less than I make?”, “Where does my money go?”) + toplam + kırılım düzeninde |
| Alt/üst gezinme | Kalıcı alt bar yok; çok geniş hamburger menüsü ve ekran içi yatay sekmeler kullanılıyor. İşlev sayısı arttıkça keşfedilebilirlik menüye yüklenmiş |
| Renklerin anlamı ve tutarlılığı | Hesap rengi hızlı işlem formunun tüm zeminini değiştiriyor. Seçili Income/Expense/Transfer sekmesi büyük ölçüde yalnız arka plan tonuyla belirtiliyor; yanlış seçim riski var |
| Tipografi ve para değerlerinin okunması | Para değerleri büyük ve sağa hizalı; negatifler `-TRY` ile açık. Uzun açıklamalar ve hesap adları bazı alanlarda kırpılıyor |
| Kart, liste ve grafik kullanımı | Home yatay hesap kartları; Records hafta gruplu liste; Cash-flow/Spending büyük özet + trend/grafik + en büyük kayıtlar düzeni |
| Form alanları ve varsayılanlar | Hızlı form yalnız tutar/hesap/kategori; ayrıntılar ikinci ekranda. Varsayılan Expense, son kullanılan hesap/kategori hatırlanıyor; transfer yönü güvenilir biçimde hatırlanmadı |
| Loading, boş, hata ve başarı geri bildirimi | Boş durumlar yönlendirici; sıfır tutar toast/snackbar ile engelleniyor. Başarı sessizce listeye dönüyor. Silme onaylı ama geri alınamaz |
| Erişilebilirlik / dokunma alanları / metin yoğunluğu | Dokunma alanları büyük. Sekme seçiminin renk ağırlıklı olması ve tek hamburger menüsündeki çok sayıda hedef bilişsel yük oluşturuyor; İngilizce arayüz kullanıldı |

## Akış özeti

- En kısa ve güçlü akış: Hızlı işlem girişi — tür → tutar → hesap → kategori → Save.
- En fazla sürtünme yaratan akış: İşletme geliri — varsayılan Expense'ten Income'a geçmek, uygun olmayan kişisel kategori ağacından `Sale` seçmek ve tarih/not/payer için kaydı tekrar açmak.
- Uygulamanın hedef kullanıcı varsayımı: Birden çok hesabı, hedefi ve planı olan kişisel/ortak hane finansı kullanıcısı; ayrıca Premium banka senkronizasyonu müşterisi.
- İşletme ve şahsi para yaklaşımı: **Yok.** Label var ama özel kapsam boyutu değil; taklit edilmedi.
- Transfer ve kart ödemesi yaklaşımı: Transfer ayrı tür ve raporda nötr; kart ödemesi Ana Hesap→İş Kartı transferiyle kart bakiyesini kapattı, gideri ikinci kez saymadı.
- Planlama, borç ve tahsilat yaklaşımı: Planned payments, Debts, Goals ve Budgets ayrı yüzeyler; Tur 1'de yalnız boş durumları incelendi (Faz 2'de bütçe + hedef canlı kuruldu, borç formu görüldü).

## Faz 2 boşluk koşumu (10 Eyl 2026) — kart, tekrarlayan, taksit, bütçe, hedef

Yeni emülatörde kullanıcı giriş yaptı; bulut verisi (Tur 1 çekirdek 5 işlem + 3 hesap)
geri geldi. Kareler `kanitlar/wallet-budgetbakers/10`–`40`.

### Çekirdek doğrulama (olaylar eklenmeden önce)

Tur 1'in 5 çekirdek işlemi bulutta **doğru Ağustos 2026 tarihleriyle** duruyordu
(Records: Sale 3 Ağu ₺25.000 · Groceries 5 Ağu ₺850 · Software 8 Ağu ₺1.200 kart ·
Transfer 12 Ağu ₺3.000 · Transfer "Kart borcu odemesi" 18 Ağu ₺1.200). Money
Manager'daki "tek güne girildi" tarih sapması Wallet'ta **yok**. Kontrol değerleri
birebir: Ana Hesap ₺20.800 · Ortak Cuzdan ₺2.150 · İş Kartı ₺0 · net ₺22.950 ·
Ağustos geliri ₺25.000 · gider ₺2.050 (`10`, `11`, `12`). Nakit/çek hesabında
açılış bakiyesi hâlâ kabul edilmiyor — kontrol "işlem-only" değerlerine göre.

### Kredi kartı hesap modeli — basit negatif bakiye

İş Kartı hesabı, borcu **negatif bakiye** olarak taşır (İş Kartı `−₺6.000,00`).
Money Manager'daki "Bu Ay / Gelecek Ay" iki sütunlu ekstre modeli, hesap kesim
tarihi ve son ödeme tarihi **yok**. Hesap ayarları (`25`): `Credit card / Overdraft
limit` (limit alanı), `Balance Display Options` = *Available Credit* / *Balance*
(kartın kalan limiti mi yoksa borcu mu gösterileceği), `Payment Due Date` = tek bir
son ödeme tarihi (yalnız hatırlatıcı, ekstre dönemi kurmaz). Hesap detayında
bakiyeyi doğrudan düzenleyen bir kalem var (bakiye ayarlama). Kart borcu düştüğünde
"minimum eşiğin altına indi" uyarısı çıkıyor (hesap başına minimum bakiye alarmı).
Kart ekstresi / dönem kavramı olmadığı için Wallet, "kesilen ekstre vs bekleyen
harcama" ayrımını yapmaz. Kareler `24`, `25`.

### Kart ödemesi — ayrı buton yok

Wallet'ta "kartı öde" diye özel bir akış yok; kart ödemesi Tur 1'deki gibi Ana
Hesap → İş Kartı **transferi** olarak girilir (raporda nötr). Bu yüzden **kısmi
kart ödemesi** ayrı bir özellik değildir: transfere istenen tutar yazılır, kartın
negatif bakiyesi o kadar azalır. Money Manager'ın ön doldurulmuş "Ödeme" butonu +
"kısmi yalnız kesilmiş ekstreye uygulanır" davranışının karşılığı yok.

### Tekrarlayan işlem (B1 — aylık ₺600 abonelik, canlı test 10 Eyl)

Wallet'ta tekrarlayan = **Planned payments** → `Frequency: Recurrent payment`.
Form (`15`): Income/Expense/Transfer + Name + Category + Account + Amount +
Payment Type + `Frequency` + `Start date` + `Notifications` + `Recurrence`
("Every 1 month" / haftalık / yıllık; "aynı gün / her 2. Cumartesi"; "Forever" /
bitiş). Kuruldu: Bulut yazilim aboneligi, Software apps games, Ana Hesap, ₺600, aylık.

Gözlemler:
- **Geçmiş tarihe kurulamıyor.** Başlangıç tarih seçicisi bu sürümde/kademede
  içinde bulunulan aydan **önceki aya gidemiyor** (ok yok, kaydırma çalışmıyor,
  yıl seçici var ama ay seçici yok). SENTETIK B1 "ilk çekim 10 Ağustos" hedefi
  reddedildi; başlangıç bugün (10 Eyl) yapıldı. Money Manager geçmişe kuruluyor ve
  geçmiş/bugünkü tekrarları otomatik gerçekleştiriyordu — Wallet bunu **hiç yapmaz**.
  Kare `14`, `22`.
- **Tanım tek başına para üretmez** (BusinessFinance `RecurringTransaction` ile
  aynı). Kaydedilince liste kartında turuncu saat ikonuyla "−₺600,00 / Today"
  görünür; bakiyeye/rapora etki **yok**.
- Vadesi gelen her örnek **bekleyen** kalır: plan detayında "Due today" satırı +
  **Confirm** butonu + ⋮ → **Postpone** / **Dismiss** (BusinessFinance'in
  occurrence → realize / skip'ine çok yakın). Kare `16`.
- **Confirm** önce bir "Payment summary" gösterir (tarih / hesap / **tutar
  düzenlenebilir**) → onaylanınca gerçek gider kaydı oluşur, bakiye düşer
  (Ana Hesap ₺20.800 → ₺20.200). Kare `17`, `20`.
- İlk onayda **plan bazında** sorulur: "gelecek örnekleri otomatik mi
  oluşturayım?" → *Yes (Recommended)* (Money Manager gibi otomatik) / *No*
  (BusinessFinance gibi onay bekler). **"No" seçildi.** Kare `18`.
- Sonraki örnek (10.10.2026) tek başına "Due in 30 days" + kendi Confirm'iyle
  bekler; **tüm seri önceden listelenmez**, yalnız bir sonraki görünür. Kare `19`,
  `33`.
- Onaylanan tekrar kaydı Records listesinde sıradan bir gider gibi görünür; Money
  Manager'daki "(Aylık)" etiketi gibi görünür bir tekrar rozeti **yok**.

### Taksit (B2 — tasarım ekipmanı ₺6.000, canlı test 10 Eyl)

**Wallet'ta taksit / taksit planı özelliği yok.** İşlem formunun ne hızlı ne de
ayrıntı katmanında taksit alanı var (`21`); Payment Type açılırında Cash / Debit
card / Credit card / Bank transfer / Voucher / Mobile payment / Web payment var,
alt-taksit seçeneği yok. En yakın davranış: kart harcaması olarak tek ₺6.000
kaydı. Sonuç: **tüm ₺6.000 aynı gün kart borcuna yazıldı** (İş Kartı `−₺6.000`)
ve **tüm ₺6.000 o ayın gideri** sayıldı (Ağustos'a değil, kaydın tarihine — Eylül).
Money Manager ay ay ₺1.000 ekstreye böler, BusinessFinance `InstallmentPlan`
per-item `realize()` eder; Wallet ikisini de yapmaz, harcamayı tek parça tutar.
Kareler `21`, `23`, `24`.

### Bütçe (canlı kuruldu — Tur 1'de yalnız boş durum görülmüştü)

Form (`26`, `27`): Name + `Period` (Monthly...) + Amount/Currency + **Categories**
(All / seçili) + **Accounts** (All / seçili) + Labels + Notifications. Kuruldu:
"Aylik gider butcesi", aylık, ₺5.000, tüm kategori + tüm hesap.
- Kategori **ve** hesap filtresi birlikte → BusinessFinance'in "harcama =
  kategori + kapsam çiftiyle toplanır" bütçe ilerlemesine mimari kuzen.
- Bütçe kart harcamasının **tam tutarını** harcandığı ayda sayar: Eylül harcaması
  ₺6.600 (B2 ₺6.000 + B1 ₺600) → ₺5.000 bütçe **aşıldı**, kırmızı çubuk +
  "You have exceeded your budget" toast + özet kartında "Remains" görünümü
  `−1.600,00 / 5.000,00`. Kareler `28`, `29`.
- Bütçe detayı (`29`): Spent / Remains toggle, "10 days" ilerleme, **Forecasted
  Spend** (30 güne yansıtma), ortalama günlük harcama, "vs past period" yüzdesi,
  kategori/label kırılımı donut'u, "Go deeper" (Premium).

### Hedef (Goals — canlı kuruldu)

"What are you saving for?" — ad + hazır ikonlar (New Vehicle / Emergency Fund...).
Detay formu (`31`): Name + **Target amount** + **Saved already** + **Desired date** +
renk/ikon + Note. Kuruldu: "Yeni ekipman fonu", hedef ₺20.000, biriken ₺0.
Özet kartında `₺0 / 0 %` ilerleme çubuğu (`32`). Basit birikim izleyici; hesap
bakiyesine bağlanması ayrı bir adım (denenmedi).

### Borç (Debts — canlı oluşturuldu, 10 Eyl ek koşum)

+ → **I Lent** / **I Borrowed**. Sonra "Bu borç kaydı Wallet'ta zaten var mı?" →
*Yes, select record* (mevcut bir işleme **bağla**) / *No, skip* (bağımsız). Kare `35`.
Form (`36`, `41`): Name (kime/kimden) + Description + **Account (zorunlu)** + Amount +
Date + Due date (varsayılan +1 yıl). Active / Closed sekmeleri. Kuruldu: I Lent
"Ada Reklam" ₺5.000, Ana Hesap.

- **Kaydederken sorulur:** *"Do you want to create a Record for this Debt? If you
  create a Record your balance will change." → **No** / **Yes, create record***
  (`42`). Yani **bakiyeye dokunmak borç bazında bir seçim** — zorunlu değil.
  (Tur 1'deki "hesap zorunlu → bakiye hareket eder" çıkarımı bu yüzden düzeltildi.)
- **"Yes, create record"** → Ana Hesap'a **"Loan, interests"** kategorili gerçek
  **gider** kaydı −₺5.000 (Ana Hesap ₺20.200 → **₺15.200**); raporlarda gider sayılır.
  Kareler `43`, `44`.
- Borç detayı = açık **Total** + bağlı **Records** listesi; geri ödemeler için
  **"Add Record"**; Total 0'a inince borç kapanır.
- **BusinessFinance farkı:** ADR 0014'ün "tanır vs taşır" ayrımı yok — her borç
  kaydı ya bakiyeyi hareket ettirir ya ettirmez, tek biçim. Ürettiği kayıt genel
  bir "Loan, interests" gideri, doğru gelir/gider tanıması değil.
- Emülatör tuzağı: cihaz rehberi izni ("name suggestions") — uygulama içi
  "Cancel" formu kapatıyor; **Android sistem diyaloğunda "İzin verme"** seçince
  form açık kalıyor ve kayıt tamamlanabiliyor.

### Tekrarlayan plan yönetimi (10 Eyl ek koşum)

- **Otomatik/onaylı kolu her zaman değiştirilebilir:** plan detayında **dişli
  ikonu** → aynı "otomatik mi / onaylı mı?" diyaloğu (yalnız ilk onayda değil).
  Kare `45`.
- **Plan silme:** düzenleme formunda **çöp ikonu** → yalnız *"Do you really want
  to delete this item?"* düz onayı. **Gerçekleşmiş occurrence hakkında uyarı
  yok** (BusinessFinance'in `409 recurring.has_realized_history` korumasının
  aksine); materyalize olmuş "Paid Today" kaydı bağımsız kalır. Kareler `46`, `47`.

### Split transaction (canlı görüldü, kaydedilmedi)

Kayıt detayı üst çubuğunda dallanan-ok ikonu → "Split record" → orijinal kaydın
altında **Split** → "New record" diyaloğu (Category / Note / **Amount**) →
Create. Yani bir kaydın bir parçası kendi kategorisi/notu/tutarıyla ayrı alt-kayda
oyulur; orijinalin tutarı o kadar azalır. Kareler `38`, `39`.

### Fiş / kamera — OCR YOK

Kayıt detayı → Attachments → **Add receipt** → yalnız "Pick a file" / "Take a
picture" (`40`). Fiş sadece **dosya/fotoğraf eki** olarak saklanır; hiçbir tutar,
tarih veya satıcı okunmaz. OCR / tarama-çıkarma yok (bu ücretsiz akışta). ADR 0011
öneri katmanının karşılığı değil.

### Faz 2 sonrası veri durumu

Ana Hesap ₺15.200 · Ortak Cuzdan ₺2.150 · İş Kartı `−₺6.000` · net ₺11.350
(10 Eyl ek koşumda "Ada Reklam" borcu için −₺5.000 "Loan, interests" gideri eklendi).
Ek: "Aylik gider butcesi" ₺5.000 bütçe, "Yeni ekipman fonu" ₺20.000 hedef,
B1 planı (sonraki vade 10.10.2026, onay-bekler modda). **B/B1/B2 canlı koşuldu**
(B: fiş/split akışı görüldü, OCR yok; B1: tekrarlayan canlı; B2: taksit
özelliği yok, tek kayıt) — Faz 7'de tekrar koşulmaz. **A (₺400 kısmi kredi
kartı ödemesi) standart ek koşum olarak hiç koşulmadı** — yalnız K06 çekirdek
görevinde kavramsal olarak "kart ödemesi = küçük transfer" gözlemlendi, ₺400'lük
ayrı bir kısmi ödeme testi yapılmadı; **Wallet Faz 7'de bu eksik tamamlanmalı**.
Wallet Tur 2'ye seçildi (11 Eyl); bulut verisi **sıfırlanmadan** üzerine
Faz 7 kayıtları eklenecek.

### Faz 7 — Tur 2 derin koşum (11 Eyl 2026, yapay zekâ)

**A tamamlama (₺400 kısmi kart ödemesi):** Bluecoins/Money Manager'la aynı
model — jenerik bir Transfer (Ana Hesap → İş Kartı, ₺400). Ana Hesap
₺15.200 → ₺14.800, İş Kartı −₺6.000 → −₺5.600. Ayrı bir "ekstre öde/asgari
tutar" kavramı yok, tamamen serbest tutarlı transfer.

**D1 (ofis kirası ₺10.000):** "Planned payments" ekranında **One-Time** bir
planlı ödeme olarak kuruldu. **Geçmiş tarih (5 Eylül) tarih seçiciden tamamen
engellendi** (1–10 Eylül günleri gri/disabled) — B1'in "geçmişe kurulamıyor"
bulgusunu doğruluyor, Bluecoins'in aksine. Kategori seçimi **zorunlu**
(boş bırakılınca kırmızı hata). Bugüne (11 Eylül, "Due today") kurulup
**Confirm** ile gerçek işleme dönüştürüldü — "Payment summary" ile tarih/
hesap/tutar son kez düzenlenebiliyor, tıpkı B1'in occurrence realize akışı
gibi. Ana Hesap ₺14.800 → ₺4.800.

**D2 (Ada Reklam'a ₺12.000 hizmet faturası) — en değerli bulgu:** "Debts"
sekmesinde yeni bir **"I Lent"** kaydı olarak oluşturuldu (Name/Description/
Account/Amount/Date/Due date alanlarıyla). Kaydederken **"Do you want to
create a Record for this Debt? If you create a Record your balance will
change."** sorusu çıktı — **"No"** seçildi çünkü bu yalnız bir fatura, henüz
tahsil edilmedi. Sonuç: "Ada Reklam OWES ME ₺12.000,00" borç kaydı oluştu,
**hiçbir hesap bakiyesi değişmedi** ve işlem geçmişinde hiç görünmedi (yalnız
Debts ekranında yaşıyor). **Bu, ADR 0014'ün "tanır vs taşır" ayrımını native
olarak destekleyen tek rakip mekanizması** — Bluecoins ve diğerlerinde böyle
bir soru/ayrım yok.

**D3 (₺5.000 kısmi tahsilat) — D2'ye sistemsel bağlı:** Aynı Debt kartındaki
**"Add Record" → "Create new Record"** ile eklendi (Debt action: "Repay
debt", Account: Ana Hesap, Amount: ₺5.000,00 — placeholder tam borç
tutarını "₺12.000,00 to Repay debt" gösterip kısmi tutar girilebiliyor).
Sonuç: borç ₺12.000 → **₺7.000** (aynı Debt/Records nesnesi altında, running
balance otomatik güncellendi), Ana Hesap ₺4.800 → **₺9.800**. **Bluecoins'in
D2/D3'ü bağımsız iki hareket olarak tutmasının aksine, Wallet'ta fatura ve
kısmi tahsilat gerçekten aynı nesnenin parçası** — bizim
`CounterpartyCharge`/`CounterpartyPayment` çiftine kavramsal olarak en yakın
rakip model.

**Arama (konu 5):** Canlı filtre + doğru sonuç. "Ada" araması 3 kayıt
buldu: D3'ün Record'u ("Lending, renting" +₺5.000, notta "Ada Reklam → Me :
Hizmet faturasi"), ilk borcun Record'u ("Loan, interests" −₺5.000) ve
çekirdek gelir kaydı ("Sale" ₺25.000). **D2 (Record'suz borç) arama
sonuçlarında hiç çıkmadı** — işlem tablosuna hiç yazılmadığını doğruluyor.
Filtre/export'a özel üç nokta menüsü bu oturumda bulunamadı (zaman kısıtı,
doğrulanamadı olarak işaretlendi).

**Tam arayüz taraması (konu 6, finansal parametre taşıyan ekranlar):**
- Hamburger menüsü çok geniş: Investments, Statistics, Budgets, Debts,
  Goals, **Shopping lists**, **Warranties**, **Loyalty cards**, **Currency
  rates**, **Group sharing** (finansal/yönetim özellikleri) + Get Premium/
  Bank Sync/Dark mode/Hide Amounts/Invite friends/Follow us/Help (genel-app,
  atlandı).
- **Settings → Filters:** "Set custom filters that you can use in
  Statistics or Records" — Bluecoins'in kayıtlı filtre profillerine benzer
  bir mekanizma.
- **Settings → Automatic rules:** "Set up rules to automatically assign
  categories and labels to your records **and recognize transfers**" —
  otomatik transfer eşleştirme kuralı, hiçbir rakipte görülmemiş bir özellik.
- **Settings → Currencies:** "Add other currencies, adjust exchange rates" —
  Bluecoins'teki çoklu para birimi bulgusuyla paralel, BusinessFinance
  kapsamı dışı.
- **Settings → Advanced settings → "Initial day of the month":**
  **"Beginning of the accounting period: 1"** — tam bizim ay/bütçe döngüsü
  başlangıç günü kavramımızın karşılığı, değiştirilebilir bir muhasebe
  dönemi parametresi.
- Templates: "Create templates to speed up the addition of new records" —
  Money Manager'ın "Ödeme" ön doldurma butonuna benzer bir kolaylık.
- Atlanan (genel-app altyapısı): User profile, Premium plans, Notifications,
  Security (PIN/Fingerprint), Personal data & Privacy, About Wallet.

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| Rapor başlığını kullanıcı sorusu olarak kurma | Uyarlayarak al | “Nakit akışım ne durumda?” gibi başlıklar grafiğin neden var olduğunu açıklar; muhasebe terimini tek başına bırakmaz | Özet ve raporlar |
| Cash-flow kartında gelir, gider ve neti aynı blokta gösterme | Doğrudan al / zaten kısmen var | Üç rakamın ilişkisi tek bakışta anlaşılıyor; transferler nötr kalıyor | Aylık özet |
| Yönlendirici boş durum + tek CTA | Doğrudan al | “Veri yok” yerine özelliği ve ilk adımı anlatıyor; proje empty-state kuralıyla uyumlu | Tüm boş listeler |
| Hızlı form ile ayrıntı formunu iki katmana ayırma | Uyarlayarak al | Sık giriş hızlanır; fakat kapsam/KDV/indirilebilirlik gibi görünür onay gerektiren alanlar gizlenmemeli | İşlem ekleme |
| Seçili işlem türünü yalnız renkle belirtme ve Expense'i varsayma | Alma | Gelirin gider olarak kaydedilmesi kolay; ikon/metin/işaretle çoklu seçililik göstergesi gerekir | İşlem türü seçimi |
| Geniş hamburger menüsünde bütün ürün yüzeylerini düz listeleme | Alma | İşlev sayısını gösteriyor ama çekirdek akışları gömüyor; BusinessFinance'te ana işler alt gezinmede kalmalı | Ana gezinme |
| Transferi iki bağlı satır gösterip toplamı nötr tutma | Uyarlayarak al | Kaynak ve hedef etkisi izlenebilir; satırların bağlı olduğu daha açık anlatılmalı | Aktivite feed'i ve transfer detayı |
| İşletme/şahsi boyutunun bulunmaması | Alma | ADR 0013'ün temel farklılaşmasını karşılamıyor | İşlem formu ve rapor filtresi |
| Doğrudan düzenleme + kalıcı silme | Alma | Finansal geçmişi değiştiriyor; BusinessFinance sil yerine iptal ve düzeltme kaydı kullanır | Kayıt detayı |
| Nakit/banka hesabında açılış bakiyesi alanı olmaması | Alma | Kullanıcıyı sahte gelir girmeye itebilir; bizde açılış bakiyesi raporu kirletmeden ayrı tutuluyor | Hesap oluşturma |
| Tekrarlayan örneğin bekleyen kalıp Confirm / Postpone / Dismiss ile ele alınması | Doğrudan al | BusinessFinance occurrence → realize / skip modeliyle birebir; onay öncesi tutar düzenlenebiliyor | Planlanan görünüm |
| "Gelecek örnekleri otomatik mi oluşturayım?" seçimini plan bazında sorma | Uyarlayarak al | Otomatik vs onaylı ikilemini kullanıcıya bırakıyor; bizde varsayılan onaylı ama plan bazlı tercih iyi bir fikir | Tekrarlayan plan kurulumu |
| Tekrarlayan planın geçmiş tarihe kurulamaması / geçmiş ayı seçememe | Alma | "Abonelik geçen ay başladı" modellenemiyor; bizim anchor tarihi serbest | Plan tarih seçici |
| Taksit / taksit planı kavramının hiç olmaması | Alma | ₺6.000'lık alım tek parça borç + tek parça gider; TR pazarında taksit yaygın, InstallmentPlan bunu ayrı modelliyor | Kart harcaması formu |
| Kredi kartını dönemsiz negatif bakiye + tek son ödeme tarihi olarak tutma | Alma | Ekstre kesim/ödeme dönemi yok; "kesilen ekstre vs bekleyen harcama" ayrımı kaybolur | Kart hesabı |
| Borç kaydederken "Record oluştur → bakiye değişir" seçeneği | Uyarlayarak al | Bakiyeye dokunmayı borç bazında opsiyona bağlaması iyi; ama bizde bu "tanır vs taşır" ayrımı olarak yapısal (ADR 0014), tek soru değil | Cari / borç kaydı |
| Borcun ürettiği kaydın genel "Loan, interests" gideri olması | Alma | Doğru gelir/gider tanıması değil; bizde `CounterpartyCharge` yönü ve kategoriyi doğru belirler | Cari / borç kaydı |
| Tekrarlayan planı gerçekleşmiş occurrence uyarısı olmadan silme | Alma | BusinessFinance `409 recurring.has_realized_history` + duraklatma yönlendirmesi kullanır; geçmişi olan plan sessizce silinmemeli | Tekrarlayan plan yönetimi |
| Otomatik/onaylı kolunun plan detayında her zaman değiştirilebilmesi | Doğrudan al | Kullanıcı planı kurduktan sonra da modunu değiştirebilmeli | Tekrarlayan plan yönetimi |
| Bütçeye kategori **ve** hesap filtresi verme | Uyarlayarak al | Bizim kategori+kapsam çiftiyle toplama mantığına yakın; hesap yerine kapsam bizde doğru eksen | Bütçe kurulumu |
| Bütçe detayında Forecasted Spend + ortalama günlük + geçmiş döneme kıyas | Henüz karar verme | Yararlı ama bizim aşama kapsamımızda değil; patronlara sunulacak | Bütçe raporu |
| Fiş = yalnız dosya/foto eki, OCR yok | Not | Wallet ücretsiz akışında fiş okuma yok; ADR 0011 öneri katmanı bizde ayrı | İşlem eki |
| Split transaction (kaydı alt-kayıtlara oyma) | Henüz karar verme | Tek fişteki farklı kategorileri ayırmak için işe yarar; bizde henüz yok | Kayıt detayı |
| Hesap başına minimum bakiye alarmı | Henüz karar verme | Kart borcu / hesap dibe vurunca uyarı; küçük ama kullanışlı | Hesap ayarı / bildirim |
| Borç kaydederken "Record oluşturursan bakiyen değişir" sorusu ve Record'suz borcun işlem tablosuna hiç yazılmaması | Doğrudan al (olumlu referans) | ADR 0014'ün "tanır vs taşır" ayrımını native destekleyen tek rakip; bizim `CounterpartyCharge` (bakiyeye dokunmaz) / `CounterpartyPayment` (bakiyeyi değiştirir) ayrımının doğruluğunu kanıtlıyor | Cari hesap / fatura akışı |
| Kısmi tahsilatın ("Add Record") aynı Debt nesnesine bağlı kalması, running balance'ın otomatik düşmesi | Doğrudan al | Bizim `CounterpartyBalance` projeksiyonuyla birebir aynı davranış; Bluecoins'in bağımsız iki hareketinden daha doğru | Cari hesap / kısmi tahsilat |
| Geçmiş tarihli planlı ödeme/tekrarlayan kurulumunun tarih seçiciden tamamen engellenmesi | Henüz karar verme | Bizde geçmiş tarihli plan kurulabiliyor (ADR gereği); bu bir kısıt, kopyalanacak bir davranış değil ama not edilir | Planlı ödeme tarih seçici |
| Otomatik transfer tanıma kuralı (Automatic rules) | Henüz karar verme | İki bağımsız kaydın transfer olduğunu otomatik algılama; ilginç ama bizde işlem başlangıcında zaten Transfer tipi seçiliyor | Otomatik kategori/kural motoru |
| Muhasebe dönemi başlangıç günü ayarı (Advanced settings) | Henüz karar verme | Bütçe/ay döngüsünün 1'den farklı bir günde başlaması; bizim `MonthlyBudget` modelimizde şu an sabit ay başı | Bütçe/ay döngüsü ayarı |

## Kanıt ve güven düzeyi

- Manuel gözlem (Tur 1): K01–K08 emülatörde sentetik veriyle tamamlandı; işlem-only kontrol değerleri birebir tuttu.
- Manuel gözlem (Faz 2, 10 Eyl): çekirdek doğrulama + B1 tekrarlayan (canlı, tam akış: kurulum → bekleyen → Confirm → materyalize) + B2 taksit (özellik yok, tek kayıt) + bütçe canlı kurulum + hedef canlı kurulum + split ve fiş akışları görüldü. Kareler `10`–`40`.
- Resmî kaynak: —
- Yorum: Hızlı formun varsayılan Expense seçimi ve renk ağırlıklı seçili durumu yanlış kayıt riskini artırır. Tekrarlayan modeli (bekleyen + Confirm/Postpone/Dismiss) BusinessFinance'e en yakın rakip davranışlarından biri; buna karşılık taksit yokluğu ve dönemsiz kart modeli TR pazarı için belirgin eksik.
- Manuel gözlem (10 Eyl ek koşum): Debt canlı oluşturuldu (I Lent ₺5.000, "Yes create record" → −₺5.000 "Loan, interests" gideri, Ana Hesap ₺15.200); tekrarlayan planın otomatik/onaylı kolu her zaman değiştirilebilir (dişli); plan silme düz onay, gerçekleşmiş occurrence uyarısı yok. Kareler `41`–`47`.
- Manuel gözlem (Faz 7, 11 Eyl): **A** (₺400 kısmi kart ödemesi, jenerik
  transfer) + **D1** (planlı ödeme kurulumu, geçmiş tarih engeli, Confirm ile
  realize) + **D2** (I Lent borç, "Record oluşturursan bakiyen değişir"
  sorusu, No → bakiye değişmedi) + **D3** (aynı Debt'e Record ile kısmi
  tahsilat, running balance otomatik güncellendi) canlı test edildi. Arama
  (konu 5) ve Settings/hamburger menü tam taraması (konu 6: Filters,
  Automatic rules, Currencies, Advanced settings/Initial day of the month)
  yapıldı. Kareler `f7-00`–`f7-58` (`kanitlar/wallet/`).
- Doğrulanamadı: Onboarding (yeni hesap ilk kurulum), Premium özelliklerin asıl davranışı (Automatic Rule, AI receipt), banka bağlantısı, dışa aktarma/yedek, widget, hedefin hesaba bağlanması, tekrarlayan planın "Yes/otomatik" kolunun üretim davranışı, Records ekranının filtre/export üç nokta menüsü.

## Tek cümlelik sonuç

Wallet güçlü rapor soruları, yönlendirici boş durumları, nötr transfer modeli,
BusinessFinance'e çok yakın "bekleyen → onayla" tekrarlayan akışıyla iyi bir
finansal UX referansı ve **Debt/Records mekanizmasıyla ADR 0014'ün "tanır vs
taşır" ayrımını native destekleyen tek rakip** (Faz 7, D2/D3); buna karşılık
varsayılan Expense seçimi, kapsam boyutunun yokluğu, taksit kavramının hiç
olmaması, dönemsiz kart modeli ve aşırı geniş hamburger menüsü BusinessFinance
için açık negatif örnekler.
