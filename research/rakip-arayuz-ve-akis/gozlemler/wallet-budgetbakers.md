# Uygulama Gözlem Formu — Wallet by BudgetBakers

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Wallet: Budget Expense Tracker / BudgetBakers |
| Sürüm | 9.3.6 (`versionCode=90164`) · Faz 2'de aynı sürüm, kurulum 2026-09-07 |
| Test tarihi | 1 Eylül 2026 (Tur 1, eski PC) · **10 Eylül 2026 (Faz 2 boşluk koşumu 12:47–13:16 ve ek koşum 16:19–16:25, yeni emülatör)** · **11 Eylül 2026 (Faz 7 derin koşum 14:42–15:32)** — saatler kare durum çubuğundan; Faz 7'de sürüm yeniden doğrulanmadı *(P3-K, WL-Q01)* |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | **İngilizce** / TRY — Wallet sistem dilini (Türkçe) almıyor, uygulama arayüzü İngilizce açılıyor; yalnız Android sistem diyalogları (bildirim izni vb.) Türkçe. Money Manager'ın aksine. |
| Hesap veya plan türü | Ücretsiz, daha önce deneme için açılmış BudgetBakers hesabı (`My Wallet`) — Faz 2'de yeni emülatörde e-posta/Google ile **kullanıcı giriş yaptı**, bulut verisi (Tur 1 çekirdek 5 işlemi + 3 hesap) geri yüklendi |
| Erişim kısıtı | Asıl manuel kayıt ve rapor akışlarında yok; banka senkronizasyonu, "Create Automatic Rule", 6M/1Y rapor aralığı ve bazı özellikler Premium (🔒). Faz 2'de **cihaz rehberi izni** reddedilince "I Lent/I Borrowed" borç formu kapanıyor (emülatör tuzağı — özellik yok değil). |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Kısmi | — | Önceden deneme için açılmış hesap doğrudan kullanılabildi; sentetik kayıtlar buluta senkronize hesaba eklendi | Kayıt/onboarding bu turda görülmedi; uygulama kullanıcı adı ve `My Wallet` çalışma alanını gösteriyor. Play Store'un yüklü 9.3.6 için güncelleme gösterdiği koşum notudur; kare kanıtı yok *(P3-G01 düzeltmesi: `00-magaza.png` mağaza değil Home alt kartlarını gösteriyor)* | `00-magaza.png`, `02b-menu.png` |
| K01 Ana ekran | Tamamlandı | 0 | Hesaplar yatay kartlarda ve bakiyeleriyle ilk blokta; `Accounts` ile `Budgets & Goals` aynı ana yüzeyde | Hesapların altında büyük Premium, banka bağlantısı ve çapraz ürün tanıtımları asıl içeriği aşağı itiyor | `03-dolu-ana-ekran.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı | ~6/hesap | Manual Input altında Cash, Checking account, Credit card, Savings, Loan, Mortgage ve overdraft dahil açık hesap türleri var; kartta limit, son ödeme tarihi ve gösterilecek bakiye alanları bulunuyor (`25-kart-hesap-ayarlari.png`); başlangıç değeri alanı o karenin görünen bölümünde yok, koşum notudur *(P3-G01)* | Cash/checking oluştururken açılış bakiyesi alanı yok; bu iki hesap protokol gereği sıfır açıldı. Checking hesabı kaydedilince veri içe aktarmayı etkinleştirmek için hesaba e-posta gönderildi | `03-dolu-ana-ekran.png` |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~12 | Hızlı form tutar + hesap + kategoriyle çok kısa; ayrıntıda tarih, not, payer, label ve ödeme türü eklenebiliyor | Form varsayılan olarak **Expense** açılıyor; Income seçimi yalnız sekme rengiyle anlaşılıyor. Hizmet geliri için işletme sınıfı yok, en yakın kategori `Sale`; ayrıntılar kayıt sonrası ikinci ekranda tamamlandı. *(P3-G01 sınırı: `04-islem-formu.png` doğru gelir kaydını değil, Expense seçiliyken −25,000 TRY ve `Sale` kategorisinin birlikte seçilebildiği ara durumu gösteriyor; gelir sonucu `11-kontrol-cash-flow-agustos.png` ve `12-kontrol-records-listesi.png` ile doğrulanır)* | `04-islem-formu.png` |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~10 | `Groceries` gibi iki katmanlı kategori ve notta serbest metin var; arama not içindeki `Market` sözcüğünü buldu | **İşletme/şahsi kapsam alanı yok.** Label veya kategoriyle taklit edilmedi; protokole göre bu boyut `Desteklenmiyor`. *(P3-G01 sınırı: `05-siniflandirma.png` işlem formu değil Spending raporudur; kapsam alanı yokluğu ve not araması koşum notudur)* | `05-siniflandirma.png` |
| K05 İşletme kart gideri | Tamamlandı | ~10 | Gider kart hesabına yazıldığı anda kart bakiyesi `-TRY 1,200`; Spending raporunda aynı gün gider oldu. Kart hesabı ayrıca limit ve vade alanı taşıyor | İşletme kapsamı yok; tedarikçi yalnız `Payee`, açıklama ise `Note` alanında tutuluyor | `05-siniflandirma.png`, `06-islem-listesi.png` |
| K06 Transfer | Tamamlandı | ~7 | Ayrı Transfer sekmesi var; From/To açıkça seçiliyor. Ana Hesap→Ortak Cüzdan ve Ana Hesap→İş Kartı iki bağlı satır (+/−) olarak görünse de toplam ve cash-flow etkisi sıfır | Liste aynı transferi iki satırda gösterdiği için ilk bakışta çift kayıt izlenimi verebiliyor; form yeni açıldığında son kullanılan yönü korumadı ve hesaplar yeniden seçildi | `06-islem-listesi.png` |
| K07 Liste ve rapor | Tamamlandı | ~4 | Liste haftalara ayrılıyor; arama not metnini buluyor. Cash-flow tam `Income TRY 25,000`, `Expenses -TRY 2,050`, net `TRY 22,950` gösterdi; transferler ve kart ödemesi yeniden gelir/gider sayılmadı. Spending kategori dağılımı ve en büyük giderleri gösteriyor | Business/personal kırılımı yok; Records menüsündeki seçenekler yalnız “her kayıtta bakiye” ve “planlananları göster”, gelişmiş filtre görülmedi *(WL-U03, 15 Eyl: menü karede — `49-u03-records-menu-bakiye-planli-secenekleri.png`; filtre/dışa aktarma bu menüde yok)* | `06-islem-listesi.png`, `07-rapor.png`, `49-u03-records-menu-bakiye-planli-secenekleri.png` |
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
doğrulandı (`10-kontrol-hesaplar.png`, `12-kontrol-records-listesi.png`, `11-kontrol-cash-flow-agustos.png`): Ana Hesap ₺20.800 · Ortak Cuzdan ₺2.150 ·
İş Kartı ₺0 · net ₺22.950 · son 12 haftada gelir ₺25.000 / gider ₺2.050 — birebir
*(P3-G01 düzeltmesi: önceki metin “Ağustos gelir” diyordu; cash-flow karesi ay değil
12 haftalık aralık gösteriyor. Net ₺22.950 ekranda Cash-flow neti olarak yazar, hesap
karesinde üç bakiyenin toplamıdır)*. B1/B2
bu doğrulamadan **sonra** eklendi; sonraki durum "Faz 2 boşluk koşumu" bölümünün
sonundadır (net ₺16.350).

## Arayüz taraması (görev dışı, ~10 dk)

| Alan | Gezildi mi | Kısa gözlem | Kanıt |
|---|---|---|---|
| Tüm ana sekmeler / alt görünümler | ✓ | Yan menü çok geniş: Home, Records, Investments, Statistics, Planned payments, Budgets, Debts, Goals, Shopping lists, Warranties, Loyalty cards, Currency rates, Group sharing ve Others. Statistics içinde Balance, Outlook, Cash-flow, Spending, Credit, Reports ve Assets sekmeleri var. *(P3-G01 sınırı: `02b-menu.png` çekmecenin yalnız üstünü Goals'a kadar gösteriyor; alttaki öğeler ve Statistics sekmeleri koşum notudur. P4-tema-02: çekmecenin alt öğeleri Faz 7'de kareli — `f7-54-hamburger.png`, `f7-55-menu-scroll.png`; Statistics'te Balance / Outlook / Cash-flow / Spending / Credit sekmeleri `48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png`'de görünür, Reports ve Assets o karede kesik)* | `02b-menu.png`, `f7-54-hamburger.png`, `f7-55-menu-scroll.png` |
| Bir raporun içine tıklama | Kısmen | Cash-flow ve Spending açıldı; Spending kategori/label geçişi, trend ve Top 5 expenses taşıyor. `Go deeper` ayrıntısı açılmadı | `05-siniflandirma.png`, `07-rapor.png` |
| Bütçe / hedef / planlama ekranı | ✓ (boş durum) | Planned payments gelecekteki tahsilat/gideri anlatıp `+` yönlendirmesi yapıyor; Budgets doğrudan `Create Budget`; Debts Active/Closed, Goals Active/Paused/Reached sekmeli. *(P3-G01 sınırı: kare yalnız Planned payments boş durumunu gösteriyor; Budgets/Goals koşum notudur. P3-G02: Debts boş durumu ve Active/Closed Faz 2'de `34-debts-bos-durum.png` ile kareli; Goals Active/Paused/Reached sekmeleri karede yok)* | `09-ozgun-ozellik.png` |
| Ayarların derinliği | Kısmen | Menü seviyesinde Dark mode, Hide Amounts, Currency rates, Group sharing, Help ve Settings görüldü; Tur 1'de ayarların içine girilmedi. *(P4-tema-02: Faz 7'de Settings ve Advanced settings açıldı — General: Accounts, Categories, Labels, Templates, Filters, Automatic rules, Currencies; Other: Notifications, Security, Advanced settings, Personal data & Privacy; Advanced: Number format, Active module after launch, Initial day of the month. Anahtarların sonuç davranışı ölçülmedi)* | `f7-56-settings.png`, `f7-57-settings-scroll.png`, `f7-58-advanced.png` |
| Arama ve filtre davranışı | ✓ | Serbest metin araması not içindeki `Market` ifadesiyle tek kaydı buldu. Records seçeneklerinde gelişmiş filtre görülmedi. *(P4-tema-02: Faz 7 “Ada” araması üç kaydı ve Σ ₺25.000'yi gösteriyor — `f7-52-search.png`; Records ⋮ menüsünde yalnız “Show balance in every record” ve “Show planned payments” var — `49-u03-records-menu-bakiye-planli-secenekleri.png`; kayıtlı filtre girişi Settings → Filters'ta — `f7-56-settings.png`)* | `f7-52-search.png`, `49-u03-records-menu-bakiye-planli-secenekleri.png` |
| Boş durum ekranları | ✓ | Planned payments, Budgets, Debts ve Goals boş durumları ne işe yaradığını açıklıyor ve ilk eylemi söylüyor; Money Manager'daki yalnız “No data” yaklaşımından daha yönlendirici. *(P3-G01 sınırı: Tur 1 kare kanıtı yalnız Planned payments için; P3-G02: Debts boş durumu Faz 2 karesiyle `34-debts-bos-durum.png`)* | `09-ozgun-ozellik.png` |
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
geri geldi. Bu bölümün kareleri envanterde E0283–E0313'tür; her kare aşağıda tam adıyla anılır.

### Çekirdek doğrulama (olaylar eklenmeden önce)

Tur 1'in 5 çekirdek işlemi bulutta **doğru Ağustos 2026 tarihleriyle** duruyordu
(Records: Sale 3 Ağu ₺25.000 · Groceries 5 Ağu ₺850 · Software 8 Ağu ₺1.200 kart ·
Transfer 12 Ağu ₺3.000 · Transfer "Kart borcu odemesi" 18 Ağu ₺1.200). Money
Manager'daki "tek güne girildi" tarih sapması Wallet'ta **yok**. Kontrol değerleri
birebir: Ana Hesap ₺20.800 · Ortak Cuzdan ₺2.150 · İş Kartı ₺0 · net ₺22.950 ·
12 haftalık gelir ₺25.000 · gider ₺2.050 (`10-kontrol-hesaplar.png`,
`11-kontrol-cash-flow-agustos.png`, `12-kontrol-records-listesi.png`; P3-G01: “Ağustos” değil 12 hafta). Nakit/çek hesabında
açılış bakiyesi hâlâ kabul edilmiyor — kontrol "işlem-only" değerlerine göre.

### Kredi kartı hesap modeli — basit negatif bakiye

İş Kartı hesabı, borcu **negatif bakiye** olarak taşır (İş Kartı `−₺6.000,00`).
Money Manager'daki "Bu Ay / Gelecek Ay" iki sütunlu ekstre modeli, hesap kesim
tarihi ve son ödeme tarihi **yok**. Hesap ayarları (`25-kart-hesap-ayarlari.png`): `Credit card / Overdraft
limit` (limit alanı, bu hesapta 0), `Balance Display Options` (bu hesapta *Available Credit*;
diğer seçenek *Balance* koşum notudur), `Payment Due Date` = tek bir
son ödeme tarihi alanı (bu hesapta *Not set*), `Exclude from stats` anahtarı (kapalı) ve
hesap silme ikonu. Hesap detayında (`24-kart-hesap-detay-negatif-bakiye.png`)
bakiyenin yanında bir düzenleme kalemi var (bakiye ayarlama girişi; etkisi denenmedi). Kart
bakiyesi −₺6.000'a indiğinde "Your balance on Is Karti dropped below the minimum threshold."
uyarısı çıktı (`23-b2-6000-tek-kart-borcu.png`).
Kart ekstresi / dönem kavramı olmadığı için Wallet, "kesilen ekstre vs bekleyen
harcama" ayrımını yapmaz. *(P3-G01 düzeltmesi: önceki metin uyarıyı "kart borcu
düştüğünde" diye ters yönde yazıyordu ve "hesap başına minimum bakiye alarmı" diyordu;
eşiğin nerede tanımlandığı görülmedi. Son ödeme tarihinin "yalnız hatırlatıcı, ekstre
kurmaz" olduğu alan açılmadığı için kanıtlanmadı. Limit 0 iken Available Credit ile
Balance gösteriminin farkı bu veriyle ölçülemez.)*

### Kart ödemesi — ayrı buton yok

Wallet'ta "kartı öde" diye özel bir akış yok; kart ödemesi Tur 1'deki gibi Ana
Hesap → İş Kartı **transferi** olarak girilir (raporda nötr). Bu yüzden **kısmi
kart ödemesi** ayrı bir özellik değildir: transfere istenen tutar yazılır, kartın
negatif bakiyesi o kadar azalır. Money Manager'ın ön doldurulmuş "Ödeme" butonu +
"kısmi yalnız kesilmiş ekstreye uygulanır" davranışının karşılığı yok.

### Tekrarlayan işlem (B1 — aylık ₺600 abonelik, canlı test 10 Eyl)

Wallet'ta tekrarlayan = **Planned payments** → `Frequency: Recurrent payment`.
Kurulumdan önce ekran boştu (`13-planned-payments-bos-durum.png`; Tur 1'deki
`09-ozgun-ozellik.png` ile aynı metin ve CTA, yeni davranış göstermez).
Form (`15-b1-tekrarlayan-form.png`): Income/Expense/Transfer + Name + Category + Account + Amount +
Currency + Payment Type + Payee + `Frequency` + `Start date` + `Notifications` + `Recurrence`
("Every 1 month" / haftalık / yıllık; "aynı gün / her 2. Cumartesi"; "Forever" /
bitiş). Kuruldu: Bulut yazilim aboneligi, Software apps games, Ana Hesap, ₺600, aylık.
*(P3-G01 sınırı: karede Frequency'ye kadar olan alanlar görünür, Start date ve
Notifications yalnız etiket; Recurrence seçenekleri koşum notudur. Payee alanı
önceki listede yoktu.)*

Gözlemler:
- **Geçmiş tarihe kurulamıyor.** Başlangıç tarih seçicisi bu sürümde/kademede
  içinde bulunulan aydan **önceki aya gidemiyor** (ok yok, kaydırma çalışmıyor,
  yıl seçici var ama ay seçici yok). SENTETIK B1 "ilk çekim 10 Ağustos" hedefi
  reddedildi; başlangıç bugün (10 Eyl) yapıldı. Money Manager geçmişe kuruluyor ve
  geçmiş/bugünkü tekrarları otomatik gerçekleştiriyordu — Wallet bunu **hiç yapmaz**.
  Kare `14-planned-gecmis-tarih-kapali.png`: ay oku görünmüyor, 1–9 Eylül soluk.
  *(P3-G01 düzeltmesi: önceki metin `22-tarih-secici-onceki-ay-yok.png` karesini de buraya
  bağlıyordu; o kare planlı ödemenin değil B2 **normal kaydının** tarih seçicisidir ve
  içinde bulunulan ayın geçmiş günü (6 Eylül) orada seçilebilir durumdadır. Kaydırmanın
  çalışmadığı ve ay seçicinin olmadığı statik karede görünmez, koşum notudur.)*
- **Tanım tek başına para üretmez** (BusinessFinance `RecurringTransaction` ile
  aynı). Kaydedilince liste kartında turuncu saat ikonuyla "−₺600,00 / Today"
  görünür; bakiyeye/rapora etki **yok**.
- Vadesi gelen her örnek **bekleyen** kalır: plan detayında "Due today" satırı +
  **Confirm** butonu + ⋮ → **Postpone** / **Dismiss** (BusinessFinance'in bekleyen
  occurrence → realize akışına yakın; *P3-T düzeltmesi: önceki metin "realize / skip"
  diyordu — BF'de örnek yalnız `Planned`/`Realized` durumundadır, atlama veya erteleme
  yolu yoktur*). Kare `16-b1-plan-detay-due-today.png`
  *(P3-G01 sınırı: karede ⋮ kapalı. WL-U02, 15 Eyl: menü açık hâli
  `50-u02-plan-menu-postpone-dismiss.png` — Postpone ve Dismiss seçenekleri kareli; seçimlerin
  sonucu denenmedi)*.
- **Confirm** önce bir "Payment summary" gösterir (tarih / hesap / **tutar**
  açılır alan olarak sunulur) → onaylanınca gerçek gider kaydı oluşur, bakiye düşer
  (Ana Hesap ₺20.800 → ₺20.200). Kare `17-b1-confirm-payment-summary.png`,
  `20-b1-sonrasi-ana-hesap-20200.png`. Değiştirilmiş değerle onay denenmedi.
- İlk örnek onaylandıktan **sonra** sorulur: "gelecek örnekleri otomatik mi
  oluşturayım?" → *Yes (Recommended)* (Money Manager gibi otomatik) / *No*
  (BusinessFinance gibi onay bekler). **"No" seçildi.** Kare `18-b1-otomatik-mi-onayli-mi-secimi.png`.
  *(P3-G01 düzeltmesi: karede **Yes önceden seçili** ve arkada onaylanmış Today satırı
  görünüyor. "Plan bazında" ifadesi soru metninde yok, çıkarımdır. "No" seçimi karede
  görünmez; sıradaki örneğin bekliyor görünmesi otomatik kolda vade gelmeden de aynı
  olabileceği için seçimi tek başına kanıtlamaz.)*
- Sonraki örnek (10.10.2026) tek başına "Due in 30 days" + kendi Confirm'iyle
  bekler; **tüm seri önceden listelenmez**, yalnız bir sonraki görünür. Kare
  `19-b1-onay-sonrasi-paid-today-siradaki-pending.png`, `33-planned-payments-b1-siradaki.png`
  (liste düzeyinde yalnız sıradaki vade 10.10.2026 görünür). Vadesinden önce Confirm
  düğmesi görünür; erken onay denenmedi.
- Onaylanan tekrar kaydı Records listesinde sıradan bir gider gibi görünür; Money
  Manager'daki "(Aylık)" etiketi gibi görünür bir tekrar rozeti **yok**
  (`37-records-listesi-b1-b2.png`: Software, apps, games · Ana Hesap · −₺600 · Today, notsuz;
  aynı hafta Σ −₺6.600 ve liste toplamı ₺16.350).

### Taksit (B2 — tasarım ekipmanı ₺6.000, canlı test 10 Eyl)

**Wallet'ta taksit / taksit planı özelliği yok.** İşlem formunun ne hızlı ne de
ayrıntı katmanında taksit alanı var (`21-b2-islem-detay-taksit-alani-yok.png`); Payment Type açılırında Cash / Debit
card / Credit card / Bank transfer / Voucher / Mobile payment / Web payment var,
alt-taksit seçeneği yok. *(P3-G01 sınırı: kare yalnız ayrıntı katmanının görünen
bölümünü gösteriyor — Note, Labels, Payee, Date/Time, Payment Type, Warranty, Status,
Place, Attachments; hızlı katman, ekranın altı ve açılır liste koşum notudur. Karede
Payment Type henüz Cash; `22-tarih-secici-onceki-ay-yok.png` arka planında Credit
card'a çevrilmiş görünüyor.)* En yakın davranış: kart harcaması olarak tek ₺6.000
kaydı. Sonuç: **tüm ₺6.000 aynı gün kart borcuna yazıldı** (İş Kartı `−₺6.000`)
ve **tüm ₺6.000 o ayın gideri** sayıldı (Ağustos'a değil, kaydın tarihine — Eylül).
Money Manager ay ay ₺1.000 ekstreye böler, BusinessFinance `InstallmentPlan`
per-item `realize()` eder; Wallet ikisini de yapmaz, harcamayı tek parça tutar.
Kareler `21-b2-islem-detay-taksit-alani-yok.png`, `23-b2-6000-tek-kart-borcu.png`,
`24-kart-hesap-detay-negatif-bakiye.png` (tek −₺6.000 kaydı, tarih Today). Giderin
Eylül'e yazıldığı bu karelerde değil, bütçe karelerinde okunur (P3-G02).

### Bütçe (canlı kuruldu — Tur 1'de yalnız boş durum görülmüştü)

Form (`26-butce-olusturma-formu.png`, `27-butce-formu-dolu.png`): Name + `Period` (Monthly...) + Amount/Currency + **Categories**
(All / seçili) + **Accounts** (All / seçili) + Labels + Notifications. Kuruldu:
"Aylik gider butcesi", aylık, ₺5.000, tüm kategori + tüm hesap.
*(P3-G02 sınırı: varsayılanlar Monthly / All / All görünür; seçili liste seçeneği ve
Period'un diğer değerleri karede açık değil.)*
- Kategori **ve** hesap filtresi birlikte → BusinessFinance'in "harcama =
  kategori + kapsam çiftiyle toplanır" bütçe ilerlemesine benzer iki eksenli bir
  filtre (`çıkarım`; ikinci eksen bizde kapsam, Wallet'ta hesap).
- Bütçe kart harcamasının **tam tutarını** harcandığı ayda sayar: Eylül harcaması
  ₺6.600 (B2 ₺6.000 + B1 ₺600) → ₺5.000 bütçe **aşıldı**, kırmızı çubuk +
  "You have exceeded your budget" toast + özet kartında "Remains" görünümü
  `−1.600,00 / 5.000,00`. Kareler `28-butce-olusturuldu-over-budget.png`,
  `29-butce-detay-6600-harcama.png`. Aşım bütçe **oluşturulduğu dakikada** gösterildi:
  bütçe, kurulmadan önceki ay içi harcamayı da sayıyor. 6.600'ün B1+B2 dökümü
  ekranda yazmaz, Eylül'deki iki kaydın (`37-records-listesi-b1-b2.png`) toplamıdır.
- Bütçe detayı (`29-butce-detay-6600-harcama.png`): Spent / Remains toggle, "10 days" ilerleme, **Forecasted
  Spend**, ortalama günlük harcama (₺660), "vs past period to date" yüzdesi (+222%),
  kategori/label kırılımı donut'u, "Go deeper".
  *(P3-G02 düzeltmesi: önceki metin tahmini "30 güne yansıtma" ve Go deeper'ı Premium
  diye yazıyordu. Karede tahmin ₺6.600, yani harcanana eşit — 660×30 doğrusal
  yansıtma değil, yöntem bilinmiyor. Go deeper'da kilit görünmez; Premium olduğu koşum
  notudur. +222%, 1–10 Ağustos gideri 2.050 ile aritmetik olarak tutarlı:
  6.600 / 2.050 = 3,22.)*

### Hedef (Goals — canlı kuruldu)

"What are you saving for?" — ad + hazır ikonlar (New Vehicle / Emergency Fund...)
(`30-goal-olusturma.png`). Şablon etiketlerinde İngilizce büyük harfe Türkçe noktalı
**İ** karışıyor (`NEW VEHİCLE`, `HOLİDAY TRİP`; plan detayında `PAYMENT OVERVİEW`,
bütçede `THİS MONTH`): sistem dili İngilizce arayüz metnini bozuyor.
Detay formu (`31-goal-detay-formu.png`): Name + **Target amount** + **Saved already** + **Desired date** (varsayılan bugün) +
renk/ikon + Note. Kuruldu: "Yeni ekipman fonu", hedef ₺20.000, biriken ₺0.
Özet kartında `₺0 / 0 %` ilerleme çubuğu (`32-butce-ve-goal-birlikte.png`). Basit birikim izleyici; hesap
bakiyesine bağlanması ayrı bir adım (denenmedi).
*(P3-G02 sınırı: detay formu karesinde Target amount henüz 0; ₺20.000 hedef hiçbir
karede görünmez, koşum notudur.)*

### Borç (Debts — canlı oluşturuldu, 10 Eyl ek koşum)

Borç eklenmeden önce Debts yönlendirici bir boş durum gösteriyor: "Track what you lent
and borrowed" + Active / Closed sekmeleri (`34-debts-bos-durum.png`).
+ → **I Lent** / **I Borrowed**. Sonra "Bu borç kaydı Wallet'ta zaten var mı?" →
*Yes, select record* (mevcut bir işleme **bağla**) / *No, skip* (bağımsız; "you can create
it later"). Kare `35-debt-kayit-baglama-sorusu.png`; bağlama dalı denenmedi.
Form (`36-debt-i-lent-formu.png`, `41-debt-i-lent-formu-dolu.png`): Name (kime/kimden) + Description + **Account** + Amount +
Date + Due date (varsayılan +1 yıl). Kuruldu: I Lent
"Ada Reklam" ₺5.000, Ana Hesap. *(P3-G02 sınırı: hesabın zorunlu olduğu yalnız
"Select an account, please." yer tutucusundan çıkarılır; boş kaydetme karede yok. Boş
form 13:10, dolu form 16:19 — iki ayrı deneme.)*

- **Kaydederken sorulur:** *"Do you want to create a Record for this Debt? If you
  create a Record your balance will change." → **No** / **Yes, create record***
  (`42-debt-kayit-olustur-mu-bakiye-degisir.png`). Yani **bakiyeye dokunmak borç bazında bir seçim** — zorunlu değil.
  (Tur 1'deki "hesap zorunlu → bakiye hareket eder" çıkarımı bu yüzden düzeltildi.)
- **"Yes, create record"** → Ana Hesap'a **"Loan, interests"** kategorili −₺5.000
  kaydı; notu "Me → Ada Reklam". Kareler `43-debt-olusturuldu-i-lent.png`,
  `44-debt-records-loan-interests-kaydi.png`.
  *(P3-G02 düzeltmesi: önceki metin kaydı "gerçek gider", Ana Hesap ₺20.200 → ₺15.200 ve
  "raporlarda gider sayılır" diye kesin yazıyordu. Karelerde kategori ve eksi tutar
  görünür; bakiye düşüşü aritmetik, rapor etkisi ölçülmedi — WL-Q02.)*
- Borç kartı "ADA REKLAM OWES ME ₺5.000,00" gösterirken Debt Records listesi aynı borç için
  **Total −₺5.000,00** gösteriyor: kart borcu, liste kayıt toplamını işaretiyle yazıyor.
  Geri ödemeler için **"Add Record"**. Total 0'a inince borcun kapandığı bu karelerde
  yok, koşum notudur.
- **BusinessFinance farkı:** ADR 0014'ün "tanır vs taşır" ayrımı yok — her borç
  kaydı ya bakiyeyi hareket ettirir ya ettirmez, tek biçim. Ürettiği kayıt genel
  bir "Loan, interests" gideri, doğru gelir/gider tanıması değil.
- Emülatör tuzağı: cihaz rehberi izni ("name suggestions") — uygulama içi
  "Cancel" formu kapatıyor; **Android sistem diyaloğunda "İzin verme"** seçince
  form açık kalıyor ve kayıt tamamlanabiliyor.

### Tekrarlayan plan yönetimi (10 Eyl ek koşum)

- **Otomatik/onaylı kolu her zaman değiştirilebilir:** plan detayında **dişli
  ikonu** → aynı "otomatik mi / onaylı mı?" diyaloğu (yalnız ilk onayda değil).
  Kare `45-planned-otomatik-onayli-toggle-her-zaman.png`: bu kez **No önceden seçili** ve
  Cancel eklenmiş — kayıtlı modun No olduğunu, dolayısıyla ilk onayda No seçildiğini
  dolaylı olarak destekler. Diyaloğu dişlinin açtığı karede görünmez, koşum notudur.
- **Plan silme:** düzenleme formunda **çöp ikonu** (`46-planned-duzenleme-formu-cop-ikonu.png`) → yalnız *"Do you really want
  to delete this item?"* düz onayı (`47-planned-silme-basit-onay-gecmis-uyarisi-yok.png`). **Gerçekleşmiş occurrence hakkında uyarı
  yok** (BusinessFinance'in `409 recurring.has_realized_history` korumasının
  aksine). *(P3-G02 düzeltmesi: önceki metin "materyalize olmuş Paid Today kaydı
  bağımsız kalır" diyordu; silme onaylanmadı — aşağıdaki veri durumunda plan ve
  10.10.2026 vadesi sürüyor — bu yüzden silme sonrası davranış gözlenmedi.)*

### Split transaction (canlı görüldü, kaydedilmedi)

Kayıt detayı üst çubuğunda dallanan-ok ikonu → "Split record" → orijinal kaydın
altında **Split** → "New record" diyaloğu (Category / Note / **Amount**) →
Create. Yani bir kaydın bir parçası kendi kategorisi/notu/tutarıyla ayrı alt-kayda
oyulur. Kareler `38-split-record-ekrani.png`, `39-split-yeni-kayit-dialog.png`.
*(P3-G02 düzeltmesi: önceki metin "orijinalin tutarı o kadar azalır" diyordu; bölme
tamamlanmadığı için bu çıkarımdır.)*

### Fiş / kamera — OCR YOK

Kayıt detayı → Attachments → **Add receipt** → yalnız "Pick a file" / "Take a
picture" (`40-add-receipt-dosya-veya-foto.png`). Fiş girişi yalnız **dosya/fotoğraf eki** olarak sunuluyor;
OCR / tarama-çıkarma seçeneği yok (bu ücretsiz akışta). ADR 0011
öneri katmanının karşılığı değil. Aynı ekranın ACTİONS bölümünde
**Create Automatic Rule** kilitli görünüyor. *(P3-G02 sınırı: dosya veya foto seçildikten
sonra hiçbir alanın doldurulmadığı denenmedi; "hiçbir tutar okunmaz" diyalog
seçeneklerinden çıkarımdır.)*

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

**Başlangıç (11 Eyl):** Ana Hesap ₺15.200, İş Kartı −₺6.000, Ortak Cuzdan ₺2.150 ve
tek açık borç Ada Reklam ₺5.000 (`f7-00-baslangic.png`, `f7-02-debts.png`;
`f7-01-debts.png` ve `f7-05-back-check.png` Home tekrarı).

**A tamamlama (₺400 kısmi kart ödemesi):** Bluecoins'teki gibi jenerik bir
transfer *(P3-T düzeltmesi: önceki metin "Bluecoins/Money Manager'la aynı model"
diyordu; Money Manager'da ön doldurulmuş "Ödeme" ve kısmi ödemenin kesilmiş ekstreye
uygulanması var, Wallet'ta ikisi de yok)* — Transfer (Ana Hesap → İş Kartı, ₺400). Ana Hesap
₺15.200 → ₺14.800, İş Kartı −₺6.000 → −₺5.600. Ayrı bir "ekstre öde/asgari
tutar" kavramı yok, tamamen serbest tutarlı transfer. Kareler: FAB menüsü
`f7-03-fab-menu.png`, transfer formu `f7-06-transfer-form2.png` ve hazır form
`f7-12-tutar-final.png` ("Target amount ~ ₺400,00"), sonuç `f7-13-a-kaydedildi.png`.
Transfer hesap seçicisinde "…outside of Wallet" hedefi de var (`f7-07-tutar-400.png`;
denenmedi). *(P3-G03 sınırı: `f7-04-transfer-form.png` adına karşın şablon formudur;
`f7-08-back-transfer.png`, `f7-09-tutar-400-v2.png`, `f7-10-cleared.png` ve
`f7-11-cleared2.png` otomatik tutar girişinin hatalı ara kareleridir — 86.400,
864.006.666.666 — ürün iddiası taşımaz. MM ile "aynı model" hükmü WL-Q06'da açık.)*

**D1 (ofis kirası ₺10.000):** "Planned payments" ekranında **One-Time** bir
planlı ödeme olarak kuruldu (`f7-15-add-planned.png`). **Geçmiş tarih (5 Eylül) tarih seçiciden
engellendi** (1–10 Eylül günleri soluk, seçim 11'de kaldı: `f7-24-date-picker.png`,
`f7-25-day5-attempt.png`) — B1'in "geçmişe kurulamıyor" bulgusunu doğruluyor, Bluecoins'in
aksine. *(P3-G03 sınırı: aynı soluklaşma normal kayıt tarih seçicisinde yok —
`22-tarih-secici-onceki-ay-yok.png`'de 6 Eylül seçilebilir; kısıt planlı ödemeye özgü.)*
Kategori seçimi **zorunlu**
(boş bırakılınca alan yanında kırmızı "Select category" hatası: `f7-26-d1-saved.png`,
`f7-27-category.png`). Tutar planlı ödemede hesap makinesi diyaloğuyla giriliyor
(`f7-18-form-check.png`, `f7-22-amount-verify.png`; hatalı ara girişler
`f7-19-amount-10000.png`, `f7-20-amount-check2.png`, `f7-21-cleared.png`). Plan hesap
seçicisi `f7-16-name-amount.png`; tür bir ara karede Income'a kaymış
(`f7-17-back-form.png`), tutarlı ama kategorisiz form `f7-23-form-with-amount.png`.
Kategori seçici ve alt kategoriler: `f7-28-category-list.png`, `f7-29-category-selected.png`
(Shopping), `f7-30-housing.png`.
*(P3-G03 düzeltmesi: D1 "Ofis kirasi" **Rent ile değil Property insurance** kategorisiyle
kaydedildi — `f7-31-rent-selected.png` adına karşın Property insurance gösteriyor; D1
sonrası tüm karelerde kategori Property insurance. Otomasyon hatasıdır, ürün kusuru değil.)*
Bugüne (11 Eylül, "Due today") kurulup (`f7-32-d1-final.png`, `f7-33-tap-planned.png`)
**Confirm** ile gerçek işleme dönüştürüldü — "Payment summary" ile tarih/
hesap/tutar son kez değiştirilebilir alan olarak sunuluyor (`f7-34-confirm-dialog.png`), tıpkı B1'in occurrence realize akışı
gibi; sonuç Paid Today (`f7-35-d1-confirmed.png`). One-Time planda dişli (otomatik/onaylı)
görünmüyor. Ana Hesap ₺14.800 → ₺4.800 (`f7-36-nav-check.png`). B1 planı Faz 7'de
hâlâ listede (`f7-14-planned.png`).

**D2 (Ada Reklam'a ₺12.000 hizmet faturası) — en değerli bulgu:** "Debts"
sekmesinde yeni bir **"I Lent"** kaydı olarak oluşturuldu (Name/Description/
Account/Amount/Date/Due date alanlarıyla). Kaydederken **"Do you want to
create a Record for this Debt? If you create a Record your balance will
change."** sorusu çıktı — **"No"** seçildi çünkü bu yalnız bir fatura, henüz
tahsil edilmedi. Sonuç: "Ada Reklam OWES ME ₺12.000,00" borç kaydı oluştu,
**hiçbir hesap bakiyesi değişmedi** ve işlem geçmişinde hiç görünmedi (yalnız
Debts ekranında yaşıyor). Kareler: I Lent / I Borrowed girişi `f7-37-debts-fab.png`,
bağlama sorusu `f7-39-i-lent-form2.png` (tekrar `f7-40-i-lent-form3.png`), boş form
`f7-41-i-lent-form4.png`, doldurulmuş form ve 12.000 `f7-43-amount-12000.png`, kayıt
oluşturma sorusu `f7-44-d2-saved.png`, sonuç `f7-45-d2-final.png`; ara kareler
`f7-38-i-lent-form.png`, `f7-42-form-filled.png`.
*(P3-G04 sınırı: "No" seçimi karede görünmez; D2'nin bakiye etkisinin 0 olduğu D1 sonrası
4.800 ile D3 sonrası 9.800 arasındaki farkın yalnız D3'ün +5.000'i olmasından
(`f7-50-balance-check.png`) ve kayıt listesinde/aramada 12.000 satırı olmamasından
(`f7-51-records.png`, `f7-52-search.png`) okunur. D2 hizmet faturası "I Lent" — "To whom
you have lent?" — formuyla girildi; üründe fatura alacağı için ayrı tür yok. Mevcut
₺5.000 borçla **aynı adla** ikinci, ayrı bir kart oluştu; formda uyarı görülmedi ve
Debts listesi karşı taraf başına birleştirme/toplam göstermiyor — WL-Q03 bu karelerle
cevaplandı.)* **Borcun bakiyeye dokunup dokunmayacağını kayıt anında kullanıcıya soran,
test edilen rakipler arasındaki tek mekanizma** — Bluecoins ve diğerlerinde böyle bir
soru yok. *(P3-T düzeltmesi: önceki metin bunu ADR 0014'ün "tanır vs taşır" ayrımını
native destekleyen tek rakip sayıyordu. Kareli olan: Record'suz borç bakiyeye, listeye
ve aramaya dokunmuyor; Record'lu dal kategori taşıyan bir kayıt üretiyor.)*
**WL-U01 sonucu (15 Eyl, kullanıcı ekran kontrolü,
`48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png`):** Cash-flow son 30 gün Income
₺5.000, Expenses −₺21.600, net −₺16.600. Gider = 600 + 6.000 + 10.000 + **5.000 (Loan,
interests — borç verme)**; gelir = yalnız **5.000 (Lending, renting — D3 tahsilatı)**.
Yani Wallet'ta borç vermek **gider**, tahsilat **gelir** sayılıyor; Record'suz ₺12.000
fatura hiçbir zaman gelir yazılmıyor. **Model ADR 0014'e eşdeğer değil**: soru bakiye
hareketini sorar, ekonomik olayın tanınmasını değil; gelir/gider yalnız para hareketiyle
oluşuyor. B09 kanıtla kapandı.

**D3 (₺5.000 kısmi tahsilat) — D2'ye sistemsel bağlı:** Aynı Debt kartındaki
**"Add Record" → "Create new Record"** ile eklendi (Debt action: "Repay
debt", Account: Ana Hesap, Amount: ₺5.000,00 — placeholder tam borç
tutarını "₺12.000,00 to Repay debt" gösterip kısmi tutar girilebiliyor).
Sonuç: borç ₺12.000 → **₺7.000** (aynı borç kartında kalan tutar otomatik
güncellendi), Ana Hesap ₺4.800 → **₺9.800**. Kareler: seçim diyaloğu
`f7-46-add-record-form.png` ("Select Record" — mevcut kaydı bağla / "Create new Record"
— "to repay or **increase** the Debt manually"), form `f7-47-new-record-form.png`,
tutar `f7-48-amount-5000.png`, borç sonucu `f7-49-d3-saved.png`, bakiye
`f7-50-balance-check.png`. Kayıt listesinde D3 "Lending, renting" kategorisiyle yeşil
+₺5.000 görünüyor (`f7-51-records.png`).
*(P3-G04 sınırı: önceki metin "aynı Debt/Records nesnesi altında running balance" diyordu;
borcun kendi kayıt listesi bu turda açılmadı, kanıt kart tutarıdır. D3 kaydının
Cash-flow/gelir etkisi o görsel paketinde ölçülmemişti; **15 Eylül WL-U01 ile D3 gelir sayıldığı doğrulandı, B09 kapandı**. Borcu
artırma ve mevcut kaydı bağlama dalları denenmedi.)* **Bluecoins'in
D2/D3'ü bağımsız iki hareket olarak tutmasının aksine, Wallet'ta fatura ve
kısmi tahsilat gerçekten aynı nesnenin parçası**. *(P1-B01 düzeltmesi, 14 Eyl
2026: önceki metin bunu bizim `CounterpartyCharge`/`CounterpartyPayment` çiftine
kavramsal olarak en yakın rakip model sayıyordu. Bu bağ BusinessFinance cari
modelinde yok: `CounterpartyPayment` karşı tarafa yazılır, belirli bir
`CounterpartyCharge`'a bağlanmaz. Belirli kayda bağlı kapanış yalnız tek seferlik
yükümlülükte (`ObligationSettlement`) vardır ve tam tutarlıdır; kısmi tahsilatı
aynı kayda bağlayan bir karşılık yok. Tanıma/taşıma eşdeğerliği WL-U01 ile reddedildi; B09 kanıtla kapandı.)*

**Arama (konu 5):** Canlı filtre + doğru sonuç. "Ada" araması 3 kayıt
buldu: D3'ün Record'u ("Lending, renting" +₺5.000, notta "Ada Reklam → Me :
Hizmet faturasi"), ilk borcun Record'u ("Loan, interests" −₺5.000) ve
çekirdek gelir kaydı ("Sale" ₺25.000). **D2 (Record'suz borç) arama
sonuçlarında hiç çıkmadı** ve yalnız Debts ekranında görünüyor. Bu arama/liste
davranışıdır; kaydın arka planda hangi yapıda saklandığı veya hiç yazılmadığı
ekrandan çıkarılamaz. Rapor etkisi bu eski koşumda ölçülmemişti; 15 Eylül WL-U01 Cash-flow kontrolünde Record'suz 12.000 gelirde yoktu, D3 5.000 gelir sayıldı *(P1-B12 düzeltmesi: önceki metin işlem tablosuna hiç yazılmadığını
doğruladığını söylüyordu)*.
Filtre/export'a özel üç nokta menüsü bu oturumda bulunamadı (zaman kısıtı,
doğrulanamadı olarak işaretlendi). Arama karesi `f7-52-search.png`; boş arama ve liste
`f7-53-more-options.png`.
*(P3-G04 düzeltmesi: `f7-53-more-options.png` Records araç çubuğunda ⋮ girişinin
bulunduğunu gösteriyor; menü açılmadığı için içeriği bilinmiyor — "bulunamadı" girişin
yokluğu değil, açılmamış olmasıdır. "Canlı filtre" tek son kareden kanıtlanmaz. Filtreli
görünümde hafta başlıklarındaki Balance güncel bakiyeyi (₺6.350) gösteriyor.)*

**Tam arayüz taraması (konu 6, finansal parametre taşıyan ekranlar):**
- Hamburger menüsü çok geniş: Investments, Statistics, Budgets, Debts,
  Goals, **Shopping lists**, **Warranties**, **Loyalty cards**, **Currency
  rates**, **Group sharing** (finansal/yönetim özellikleri) + Get Premium/
  Bank Sync/Dark mode/Hide Amounts/Invite friends/Follow us/Help (genel-app,
  atlandı). Kareler `f7-54-hamburger.png`, `f7-55-menu-scroll.png`; Settings
  `f7-56-settings.png`, `f7-57-settings-scroll.png`, Advanced settings `f7-58-advanced.png`.
  *(P3-G04 sınırı: Automatic rules Settings listesinde kilitsiz görünüyor, kayıt
  detayındaki "Create Automatic Rule" ise kilitli (`40-add-receipt-dosya-veya-foto.png`);
  kuralın içi açılmadığı için "hiçbir rakipte görülmemiş" hükmü doğrulanmadı — WL-Q10.
  WL-U04, 15 Eyl, kullanıcı beyanı (görsel yok): Automatic rules ücretsiz sürümde
  açılmıyor; özellik Premium kapsamında sayılır, kilidin biçimi bilinmiyor. Filters, Templates ve Currencies yalnız giriş açıklamasıyla
  kareli. `f7-54-hamburger.png` hesap sahibinin adını gösteriyor; teslimde karartma adayı.)*
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
  **"Beginning of the accounting period: 1"** — değiştirilebilir bir muhasebe
  dönemi parametresi. *(P3-T düzeltmesi: önceki metin bunu "tam bizim ay/bütçe döngüsü
  başlangıç günü kavramımızın karşılığı" sayıyordu; BusinessFinance'te böyle bir ayar
  yok, `MonthlyBudget` dönemi her zaman ayın 1'inde başlar.)*
- Templates: "Create templates to speed up the addition of new records" —
  Money Manager'ın "Ödeme" ön doldurma butonuna benzer bir kolaylık. Home FAB'ında
  "Create first template" girişi ve şablon formu (Name, Amount, Account, Category,
  Payment Type, Labels, Note, Payee, Type) karelidir: `f7-03-fab-menu.png`,
  `f7-04-transfer-form.png`; şablon kaydedilmedi.
- Atlanan (genel-app altyapısı): User profile, Premium plans, Notifications,
  Security (PIN/Fingerprint), Personal data & Privacy, About Wallet.

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| Rapor başlığını kullanıcı sorusu olarak kurma | Uyarlayarak al | “Nakit akışım ne durumda?” gibi başlıklar grafiğin neden var olduğunu açıklar; muhasebe terimini tek başına bırakmaz | Özet ve raporlar |
| Cash-flow kartında gelir, gider ve neti aynı blokta gösterme | Doğrudan al | Üç rakamın ilişkisi tek bakışta anlaşılıyor; transferler nötr kalıyor. Bizde aylık özette kısmen var *(P3-T: sözlük dışı "Doğrudan al / zaten kısmen var" etiketi sadeleştirildi)* | Aylık özet |
| Yönlendirici boş durum + tek CTA | Doğrudan al | “Veri yok” yerine özelliği ve ilk adımı anlatıyor; proje empty-state kuralıyla uyumlu | Tüm boş listeler |
| Hızlı form ile ayrıntı formunu iki katmana ayırma | Uyarlayarak al | Sık giriş hızlanır; fakat kapsam/KDV/indirilebilirlik gibi görünür onay gerektiren alanlar gizlenmemeli | İşlem ekleme |
| Seçili işlem türünü yalnız renkle belirtme ve Expense'i varsayma | Alma | Gelirin gider olarak kaydedilmesi kolay; ikon/metin/işaretle çoklu seçililik göstergesi gerekir | İşlem türü seçimi |
| Geniş hamburger menüsünde bütün ürün yüzeylerini düz listeleme | Alma | İşlev sayısını gösteriyor ama çekirdek akışları gömüyor; BusinessFinance'te ana işler alt gezinmede kalmalı | Ana gezinme |
| Transferi iki bağlı satır gösterip toplamı nötr tutma | Uyarlayarak al | Kaynak ve hedef etkisi izlenebilir; satırların bağlı olduğu daha açık anlatılmalı | Aktivite feed'i ve transfer detayı |
| İşletme/şahsi boyutunun bulunmaması | Alma | ADR 0013'ün temel farklılaşmasını karşılamıyor | İşlem formu ve rapor filtresi |
| Doğrudan düzenleme + kalıcı silme | Alma | Finansal geçmişi değiştiriyor; BusinessFinance sil yerine iptal ve düzeltme kaydı kullanır | Kayıt detayı |
| Nakit/banka hesabında açılış bakiyesi alanı olmaması | Alma | Kullanıcıyı sahte gelir girmeye itebilir; bizde açılış bakiyesi raporu kirletmeden ayrı tutuluyor. *(P3-T sınırı: alan yokluğu kare kanıtı taşımıyor ve hesap detayındaki bakiye düzenleme girişi denenmedi; sahte gelir riski `çıkarım`dır)* | Hesap oluşturma |
| Tekrarlayan örneğin bekleyen kalıp Confirm / Postpone / Dismiss ile ele alınması | Uyarlayarak al | Bekleyen örneği onayla gerçekleştirme ve onay öncesi tutarı düzeltme bizim realize akışımıza yakın. Bizde atlama/erteleme yok; Postpone/Dismiss bu yüzden doğrudan eşleşmez; seçeneklerin varlığı WL-U02 ile kareli, sonuçları denenmedi. *(P3-T düzeltmesi: önceki metin "realize / skip modeliyle birebir — Doğrudan al" diyordu; BF'de örnek yalnız `Planned`/`Realized`)* | Planlanan görünüm |
| "Gelecek örnekleri otomatik mi oluşturayım?" seçimini plan bazında sorma | Uyarlayarak al | Otomatik vs onaylı ikilemini kullanıcıya bırakıyor. Bizde yalnız açık onayla gerçekleştirme var, otomatik mod hiç yok; plan bazlı tercih ancak yeni bir kararla eklenebilir. *(P3-T düzeltmesi: önceki metin "bizde varsayılan onaylı" diyordu. P3-G01: Wallet'ta soru ilk onaydan sonra gelir ve Yes/otomatik önceden seçilidir)* | Tekrarlayan plan kurulumu |
| Tekrarlayan planın geçmiş tarihe kurulamaması / geçmiş ayı seçememe | Alma | "Abonelik geçen ay başladı" modellenemiyor; bizim anchor tarihi serbest | Plan tarih seçici |
| Taksit / taksit planı kavramının hiç olmaması | Alma | ₺6.000'lık alım tek parça borç + tek parça gider; TR pazarında taksit yaygın, InstallmentPlan bunu ayrı modelliyor | Kart harcaması formu |
| Kredi kartını dönemsiz negatif bakiye + tek son ödeme tarihi olarak tutma | Alma | Ekstre kesim/ödeme dönemi yok; "kesilen ekstre vs bekleyen harcama" ayrımı kaybolur | Kart hesabı |
| Borç kaydederken "Record oluştur → bakiye değişir" seçeneği | Uyarlayarak al | Bakiyeye dokunmayı borç bazında opsiyona bağlaması iyi; ama bizde bu "tanır vs taşır" ayrımı olarak yapısal (ADR 0014), tek soru değil | Cari / borç kaydı |
| Borcun ürettiği kaydın genel "Loan, interests" gideri olması | Alma | Doğru gelir/gider tanıması değil; bizde `CounterpartyCharge` yönü ve kategoriyi doğru belirler | Cari / borç kaydı |
| Tekrarlayan planı gerçekleşmiş occurrence uyarısı olmadan silme | Alma | BusinessFinance `409 recurring.has_realized_history` + duraklatma yönlendirmesi kullanır; geçmişi olan plan sessizce silinmemeli | Tekrarlayan plan yönetimi |
| Otomatik/onaylı kolunun plan detayında her zaman değiştirilebilmesi | Doğrudan al | Kullanıcı planı kurduktan sonra da modunu değiştirebilmeli | Tekrarlayan plan yönetimi |
| Bütçeye kategori **ve** hesap filtresi verme | Uyarlayarak al | Bizim kategori+kapsam çiftiyle toplama mantığına yakın; hesap yerine kapsam bizde doğru eksen | Bütçe kurulumu |
| Bütçe detayında Forecasted Spend + ortalama günlük + geçmiş döneme kıyas | Henüz karar verme | Yararlı ama bizim aşama kapsamımızda değil; patronlara sunulacak | Bütçe raporu |
| Fiş = yalnız dosya/foto eki, OCR yok | Alma | **Kazandırdığı:** ek almak basit ve hatasız; model yanlış tutar önermez. **Kaybettirdiği:** fişten tutar/tarih önerisi yok, kullanıcı her alanı elle yazar. Bizde ADR 0011 öneri katmanı ayrı. *(P3-T: sözlük dışı "Not" etiketi `alma` yapıldı; OCR yokluğu ücretsiz akış ve seçim öncesi diyalogla sınırlı)* | İşlem eki |
| Split transaction (kaydı alt-kayıtlara oyma) | Henüz karar verme | Tek fişteki farklı kategorileri ayırmak için işe yarar; bizde henüz yok | Kayıt detayı |
| Bakiye minimum eşiğin altına inince uyarı | Henüz karar verme | Kart bakiyesi eksiye inince uyarı göründü; küçük ama kullanışlı. *(P3-G01: eşiğin hesap başına mı, nerede tanımlandığı görülmedi)* | Hesap ayarı / bildirim |
| Borç kaydederken "Record oluşturursan bakiyen değişir" sorusu ve Record'suz borcun işlem listesinde ve aramada görünmemesi (saklama yapısı bilinmiyor — P1-B12) | Uyarlayarak al | **Kazandırdığı:** "para el değiştirdi mi?" sorusu kullanıcıya **soru olarak** soruluyor; bakiye buna göre doğru kalıyor. **Kaybettirdiği:** soru yalnız bakiyeyi belirliyor, tanımayı değil — Record'suz fatura hiç gelir olmuyor, Record'lu borç verme gider, tahsilat gelir sayılıyor (WL-U01); her borçta bir soru daha ve yanlış cevap sessizce yanlış bakiye üretiyor. Bizde tanıma ve taşıma kayıt türüyle ayrı (ADR 0014); alınabilecek olan yalnız bakiye etkisini kullanıcıya açıkça söyleyen dil. *(WL-U01 düzeltmesi, 15 Eyl: önceki gerekçe "model buradan doğru kuruluyor — Doğrudan al" diyordu; rapor etkisi bu öncülü çürüttü)* | Cari hesap / fatura akışı |
| Kısmi tahsilatın ("Add Record") aynı borca bağlı kalması, borç kartındaki kalan tutarın otomatik düşmesi | Doğrudan al | **Kazandırdığı:** "bu borçtan ne kaldı" sorusu ürün tarafından cevaplanıyor, kullanıcı hesap yapmıyor. **Kaybettirdiği:** borç bir nesne olduğu için her tahsilatın hangi borca ait olduğunu seçmek gerekiyor — Bluecoins'in bağımsız iki hareket modeli bu adımı hiç sormuyor. Bizim `CounterpartyBalance` projeksiyonu bu soruyu karşı taraf toplamında cevaplar, tek bir borç veya fatura düzeyinde değil; Wallet'ın borç başına kalan tutarının bizdeki karşılığı yalnız tam tutarla kapanan tek seferlik yükümlülüktür. *(P1-B01 düzeltmesi, 14 Eyl 2026: önceki gerekçe projeksiyonumuzu Wallet'la aynı tarafta sayıyordu.)* | Cari hesap / kısmi tahsilat |
| Geçmiş tarihli planlı ödeme/tekrarlayan kurulumunun tarih seçiciden engellenmesi | Alma | Yukarıdaki "Tekrarlayan planın geçmiş tarihe kurulamaması" satırıyla aynı gözlem. **Kaybettirdiği:** geçen ay başlamış bir yükümlülük planlanamıyor. Bizde geçmiş başlangıç kodda serbest. *(P3-T düzeltmesi: önceki etiket "Henüz karar verme" ve dayanak "ADR gereği" idi; iki satır hizalandı, ADR dayanağı bulunmadı. Kısıt planlı ödemeye özgü; normal kayıtta ay içi geçmiş gün seçilebilir)* | Planlı ödeme tarih seçici |
| Otomatik transfer tanıma kuralı (Automatic rules) | Henüz karar verme | İki bağımsız kaydın transfer olduğunu otomatik algılama; ilginç ama bizde işlem başlangıcında zaten Transfer tipi seçiliyor | Otomatik kategori/kural motoru |
| Muhasebe dönemi başlangıç günü ayarı (Advanced settings) | Henüz karar verme | Bütçe/ay döngüsünün 1'den farklı bir günde başlaması; bizim `MonthlyBudget` modelimizde şu an sabit ay başı | Bütçe/ay döngüsü ayarı |

## Kanıt ve güven düzeyi

- Manuel gözlem (Tur 1): K01–K08 emülatörde sentetik veriyle tamamlandı; işlem-only kontrol değerleri birebir tuttu.
- Manuel gözlem (Faz 2, 10 Eyl): çekirdek doğrulama + B1 tekrarlayan (canlı, tam akış: kurulum → bekleyen → Confirm → materyalize) + B2 taksit (özellik yok, tek kayıt) + bütçe canlı kurulum + hedef canlı kurulum + split ve fiş akışları görüldü. Kareler envanterde E0283–E0313; tam adlar ilgili bölümlerde.
- Kullanıcı kontrolü (15 Eyl 2026, mevcut veri, yeni kayıt yok): WL-U01 Cash-flow son 30 gün
  (`48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png`) — Record'lu borç kayıtları gelir/gidere
  dahil, B09 kapandı; WL-U03 Records ⋮ menüsü
  (`49-u03-records-menu-bakiye-planli-secenekleri.png`); WL-U04 Automatic rules ücretsiz
  sürümde açılmıyor (beyan, görsel yok); WL-U02 plan ⋮ menüsü Postpone/Dismiss
  (`50-u02-plan-menu-postpone-dismiss.png`). Görseller önceki koşumlarla aynı cihazdan çekildi.
- Resmî kaynak: —
- Çıkarım: Hızlı formun varsayılan Expense seçimi ve renk ağırlıklı seçili durumu yanlış kayıt riskini artırabilir (ölçülmedi). Tekrarlayan modelde bekleyen örneği onayla gerçekleştirme BusinessFinance'e yakın; buna karşılık taksit yokluğu ve dönemsiz kart modeli taksit ve ekstre kullanan kullanıcı için eksik kalabilir (pazar ölçümü değil). *(P3-T: eski "Yorum" etiketi `Çıkarım` yapıldı; "belirgin eksik" ve Postpone/Dismiss eşleşmesi daraltıldı)*
- Manuel gözlem (10 Eyl ek koşum): Debt canlı oluşturuldu (I Lent ₺5.000, "Yes create record" → −₺5.000 "Loan, interests" gideri, Ana Hesap ₺15.200); tekrarlayan planın otomatik/onaylı kolu her zaman değiştirilebilir (dişli); plan silme düz onay, gerçekleşmiş occurrence uyarısı yok (silme onaylanmadı). Kareler envanterde E0314–E0320; tam adlar "Borç" ve "Tekrarlayan plan yönetimi" bölümlerinde.
- Manuel gözlem (Faz 7, 11 Eyl): **A** (₺400 kısmi kart ödemesi, jenerik
  transfer) + **D1** (planlı ödeme kurulumu, geçmiş tarih engeli, Confirm ile
  realize) + **D2** (I Lent borç, "Record oluşturursan bakiyen değişir"
  sorusu, No → bakiye değişmedi; "No" karede değil etkiden okunur) + **D3** (aynı borca
  Record ile kısmi tahsilat, borç kartındaki kalan tutar 12.000 → 7.000) canlı test edildi. Arama
  (konu 5) ve Settings/hamburger menü tam taraması (konu 6: Filters,
  Automatic rules, Currencies, Advanced settings/Initial day of the month)
  yapıldı. Kareler envanterde E0321–E0379 (`kanitlar/wallet-budgetbakers/`); tam adlar Faz 7 paragraflarında.
- Doğrulanamadı: Onboarding (yeni hesap ilk kurulum), Premium özelliklerin asıl davranışı (Automatic Rule, AI receipt), banka bağlantısı, dışa aktarma/yedek, widget, hedefin hesaba bağlanması, tekrarlayan planın "Yes/otomatik" kolunun üretim davranışı, Records ekranının filtre/export üç nokta menüsü.

## Tek cümlelik sonuç

Wallet güçlü rapor soruları, yönlendirici boş durumları, nötr transfer modeli,
BusinessFinance'e çok yakın "bekleyen → onayla" tekrarlayan akışıyla iyi bir
finansal UX referansı ve **borcun bakiyeye dokunup dokunmayacağını kayıt anında soran,
test edilenler arasındaki tek rakip** (Faz 7, D2/D3); ancak gelir/gideri yalnız para
hareketiyle yazdığı için ADR 0014'ün tanıma/taşıma ayrımına eşdeğer değil (WL-U01: borç
verme gider, tahsilat gelir, fatura hiç gelir değil); buna karşılık
varsayılan Expense seçimi, kapsam boyutunun yokluğu, taksit kavramının hiç
olmaması, dönemsiz kart modeli ve aşırı geniş hamburger menüsü BusinessFinance
için açık negatif örnekler.

## 23 Eylül 2026 — ek eksik koşumu (22 Eylül) ve kare doğrulaması

Koşum listesi `raporlar/eksik-kosum-ortak-listesi.md` (WL-01…WL-21). Aşağıdakiler
kareler tek tek açılarak doğrulanan ve Belge 2'ye taşınan kısımdır.

**Test verisi (silinmedi):** ₺100 `Isletme` etiketli Groceries ve ₺50 `Sahsi` etiketli
Lending, renting gideri (Ana Hesap); ₺300 Ana Hesap → Is Karti aktarımı; iki yeni etiket.
Ay gideri 21.600 → 21.750, Ana Hesap 9.800 → 9.350, Is Karti −5.600 → −5.300.

| Soru | Gözlem | Kanıt |
|---|---|---|
| Aktarım formu (WL-03) | INCOME / EXPENSE / **TRANSFER** aynı formun üç sekmesi; From account → To account, "Target amount ~", hesap makineli tuş takımı | `f7-66-aktarim-formu.png` (E0436) |
| Aktarım ve kısmi kart ödemesi (WL-04, WL-16) | Kart ödemesi ayrı bir kavram değil, hesaptan karta aktarım. ₺300 aktarım: Ana Hesap 9.800 → 9.500, Is Karti −5.600 → −5.300; son 30 gün gideri **21.600'de kaldı** | `f7-68-…` (E0438) |
| Kart harcaması dönem giderine giriyor mu (WL-05) | **Evet — aritmetikle.** Son 30 gün / bu ay gideri 21.600 = 10.000 Property insurance + **6.000 Electronics, accessories (Is Karti, "Tasarim ekipmani")** + 5.000 borç verme + 600 Software. Hesap filtreli bir kare yok; koşumun "Ana Hesap 15.600" notu kareyle desteklenmiyor. `Credit` sekmesindeki %2147483647 iddiası da karede görünmüyor (görünen %0, limit 0) | `f7-76-…` (E0440), `f7-86-…` (E0441), E0398; E0461 |
| Açılış bakiyesi alanı (WL-02) | Dördüncü hesap ücretsiz sürümde açılamıyor; mevcut hesabın **düzenleme** formu tarandı: Color · Import email · Enable automatic imports · Exclude from stats · Archive · Minimum balance (bildirim, 0.00) · Maximum balance. **Açılış bakiyesi alanı yok.** Minimum balance açık ve 0.00 — kart eksiye inince çıkan "dropped below the minimum threshold" uyarısının (E0296) eşiği bu | `f7-65-…` (E0437 — import e-postası, basılmaz) |
| Labels görünümü (WL-07) | Labels sekmesi yalnız etiketli kayıtları topluyor: This month ₺150 (Isletme ₺100 · Sahsi ₺50); aynı ayın Categories toplamı ₺21.750. Etiketsiz kayıtlar bu görünümde hiç yok | `f7-80-…` (E0425), `f7-81-…` (E0439) |
| Borca kayıt bağlama (WL-12, WL-13) | Select Record bütün mevcut kayıtları, etiket çipleriyle listeliyor. Debt action tam iki değer: Repay debt · Increase debt | `f7-86-…` (E0441), `f7-88-…` (E0451) |
| Debts › Closed (WL-06) | Boş: "No closed debts" (koşum kaydı; `f7-83` envantere girmedi) | koşum kaydı |
| Dışa aktarma (WL-09) | Ayarlar'da ve Records menüsünde yok; çekmecenin **katlanmış Others** bölümünde Imports · Exports · Locations. Exports formu: Account · Type · Payment Type · From/To · Include account transfers · PDF / XLS / CSV. Dosya üretilmedi | `f7-104-…` (E0450), `f7-105-…` (E0428) |
| Silme (WL-17) | Kayıt ayrıntısında sil · böl · kaydet; silme onayı "Do you really want to delete this item?"; çekmecede ve Ayarlar'da çöp kutusu yok | `f7-103-…` (E0452), koşum kaydı |
| Sıfır tutar (WL-11) | Koşum "mesajsız red" dedi; kare (E0460) yalnız 0 TRY'lik formu gösteriyor. E0281 aynı durumda "Please fill in the amount." baloncuğunu gösteriyor. **Bu kare yeni bir sonuç kanıtlamaz**; E0281 geçerli | E0460, E0281 |

## 24 Eylül 2026 — Belge 1 düzeltme turu: kare doğrulaması

Belge 1'e dayanak yapılmadan önce kareler tek tek açıldı. Aşağıdakiler ya ortak listedeki koşum
özetini kareye göre düzeltir ya da Belge 1'in kullanmadığı bir karede görülen arayüz ayrıntısıdır.
Yeni koşum yapılmadı.

| Soru | Gözlem | Kanıt |
|---|---|---|
| İki tutar yazımının nedeni (WL-19) | Advanced settings'teki **Number format** tek anahtar: "Use decimals within amounts". Kapatılınca tutarlar ondalıksız (₺9.350); simge ve Türkçe ayraç değişmiyor. Belge 1'in sorduğu **para kodu ve ayraç** farkını (TRY 20,800.00 ↔ ₺20.800,00) bu ayar açıklamıyor. Ortak listedeki "kapandı" Belge 1 için **kısmen** | E0482, E0485 |
| Aynı ekranda iki yan ayar | Active module after launch (Dashboard module) · Initial day of the month (Beginning of the accounting period: 1) | E0482 |
| Hesap türü seçimi (WL-02) | Dört seçenek: Bank Sync · Investments · **File Import** ("Import CSV, Excel, OFX, … Update your account by importing your transactions as data files to Wallet **via email**…") · Manual Input. Ücretsiz sürümde yeni hesap premium duvarına çarpıyor | E0471, E0472 |
| Hesap düzenleme formunun altı | Import email + **Enable automatic imports** (kapalı) · **Minimum balance** "Get notified when balance drops below this amount" (açık, 0.00) · Maximum balance (kapalı). Kart eksiye inince görülen uyarının bu ayardan geldiği karede bağlanmıyor | E0437 |
| Dönem seçici (WL-08) | Üç sayfa: göreli çipler (7D · 30D · 12W · 6M 🔒 · 1Y 🔒) · adlandırılmış dönem (Today · This week · This month · **This year 🔒**) · özel tarih aralığı. Ortak liste "This year"ın kilidini yazmıyor | E0473, E0474, E0475 |
| Borcun kayıt listesi (WL-14) | Debt Records: Total ₺5.000,00, tek satır (Ada Reklam → Me, +5.000, Ana Hesap), altta Add Record. **Manage debt karede yok** (ortak liste yazıyor) | E0478 |
| Transfer hedefi | Hesap seçicide dördüncü seçenek "…outside of Wallet" (Cash); kullanılmadı | E0328 |
| Borç ekleme yönü ve vade | Debts'te ekleme düğmesi iki yön açıyor: I Lent · I Borrowed. I Lent formunda Date ve **Due date**; vadenin varsayılanı bir yıl sonrası | E0358, E0362 |
| Automatic rules | Settings: "Set up rules to automatically assign categories and labels to your records and recognize transfers." Kurulmadı | E0378 |
| Etiket oluşturma (WL-07) | Add label formu: Name · Color · **Auto assign to new records** (kapalı). Etiketi kullanıcı adıyla oluşturuyor; anahtar açıkken ne olduğu denenmedi | E0497 |
| Planlı ödeme sıralaması (WL-21) | Araç çubuğunda arama ve sıralama. Sorting: By due date - newest (seçili) · oldest · By name A→Z · Z→A; Default düğmesi | E0498 |
