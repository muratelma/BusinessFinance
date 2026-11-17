# Borç, alacak ve cari — sunucu tasarımı (Faz 1)

16 Kasım 2026. Karar `02-TASARIM.md` ve ADR 0022. Bu belge kodun bugünkü
hâline bakarak sunucuda neyin değişeceğini söyler. **Kod değişmedi.**

## 1. Okunanlar ve okunmayanlar

| Durum | Dosyalar |
|---|---|
| Okundu | `Domain/CounterpartyCharge.cs`, `CounterpartyPayment.cs`, `CounterpartyBalance.cs`, `Counterparty.cs` (alanlar); `Application/Counterparties/CounterpartyLedgerUseCases.cs`, `CounterpartyBalanceSummary.cs`, `CounterpartyContracts.cs`; `Infrastructure/Counterparties/CounterpartyAging.cs`; `Domain/Debt.cs` başı (alanlar, işlem imzaları, `DebtPayment` alanları); `Application/Debts/DebtBook.cs` sözleşmeleri |
| Tarandı (arama) | Cari tabloları okuyan 16 dosya; `CardDebt`in borç ödemelerini okuması; gün sonunun cari satış okuması; planlanan projection'ın kişi ve borç okumaları; Flutter'da `obligations` çağıranlar; hatırlatmaların kaynağı |
| **Okunmadı** | `Application/Debts/DebtUseCases.cs` ve `DebtClosingUseCases.cs`; `EfCounterpartyRepository`; `EfDebtBookReader`; `EfFinancialActivityRepository` (akış); `AccountMovements`; yedek dışa ve içe aktarma; API uç dosyaları; testler |

Okunmayanlar Faz 2'nin ilk işidir; bu belgedeki "doğrulanacak" işaretli
maddeler onlara bağlıdır.

## 2. Bugünkü model (doğrulanan)

- **`CounterpartyCharge`** gelir ya da gider tanır: `CategoryId` ve `Scope`
  zorunludur, kategori türü yönle sınanır (alacak → gelir, borç → gider),
  `DueDate` isteğe bağlıdır, indirimleri vardır (`NetAmount`).
- **Kapsam** kullanım durumunda bağlamla zorlanıyor:
  `TransactionScopeResolution.ResolveInContext(TransactionScope.Business, …)`.
- **`CounterpartyPayment`** para taşır: `AccountId` zorunlu, `Direction`
  taşır, kartla tahsilde `PosSettlementId`; istek kimliği var
  (`RequestScopedId`, amaç `counterparty-payment`).
- **Borçlandırmada istek kimliği yok** (`Guid.NewGuid()`).
- **Bakiye** iki ayrı toplamdır (`Receivable`, `Payable`); ekrana giden
  `OwedToYou` / `OwedByYou` eksiye düşen tarafı öbür tarafa taşır.
- **Gecikme** (`CounterpartyAging`) okunurken hesaplanır: ödemeler ve açılış
  bakiyesi en eski borçlandırmadan başlayarak düşülür (gün, sonra giriş anı);
  vadesi geçmiş açık kısım gecikmiş, vadesi gelmemiş en yakın açık kısım
  sıradaki vadedir.
- **`Debt`** kendi kapsamını taşır; anapara ve toplam geri ödeme kullanıcıdan
  gelir, yıllık oran bunlardan **türetilir** (kullanıcıdan alınmaz); kalem
  başına anapara ve faiz payı `AmortizationSchedule` ile dağıtılır. Kartla
  ödeme `DebtPayment.CreditCardId` ile yazılır ve `CardDebt` onu okur.
- **Gün sonu** o günün alacak yönlü cari borçlandırmalarını kapsam süzmeden
  okur (bugün hepsi `İşletme`).
- **Planlanan projection borcu ve yükümlülüğü okur, cari vadeleri okumaz.**
  Hatırlatmalar planlanan projection'dan beslenir; yani kişinin hesabındaki
  vadeler bugün ne planlananlarda ne hatırlatmalarda var.

## 3. Teknik soruların cevapları

**1 — Borç para kaydı: borçlandırmaya tür mü, ayrı tablo mu? → Ayrı tablo.**

`CounterpartyCharge` tanıyan bir kayıttır ve 16 dosya onu "gelir ya da gider"
diye okur (`RecognizedItems`, raporlar, gün sonu, fiş tekrar araması, kategori
kullanımı). Borç parayı aynı tabloya koymak kategori ve kapsamı nullable
yapmayı ve her okuyana "tür değilse" süzgeci eklemeyi gerektirir; unutulan bir
süzgeç borç parayı sessizce gelir yazar. Ters hata (ayrı tablonun bir bakiye
okumasında unutulması) bakiyeyi görünür biçimde bozar ve testte yakalanır.

Yeni kayıt (`CounterpartyLoan`; çalışma adı `CounterpartyAdvance` idi):

| Alan | Not |
|---|---|
| `Id`, `UserId`, `CounterpartyId` | Sahiplik bileşik anahtarla |
| ~~`Kind`~~ | **Kalktı** (17 Kasım): avans ayrı kayıt değil, tahsilat ya da ödemedir; senaryo 22 ancak böyle sağlanır. Sapma `06.3-butunsel-duzenleme.md` "Sapmalar" |
| `Direction` | `Receivable`: verdim, alacağınız artar, hesaptan çıkar. `Payable`: aldım, borcunuz artar, hesaba girer |
| `AccountId` | Zorunlu |
| `Amount`, `Date`, `DueDate?`, `Description?` | `Date` ileri olamaz; `DueDate` olabilir |
| İptal alanları, giriş anı | Diğer kayıtlarla aynı |

Kategori ve kapsam taşımaz (ADR 0014: taşır, tanımaz).

**2 — Kapsam.** Kişiye `Scope` (`TransactionScope?`, nullable) eklenir.
Borçlandırmada `ResolveInContext(Business, …)` yerine genel zincir kullanılır
(kategori → açık seçim → kişinin türü ön değer). Gün sonunun cari satış
okumasına `charge.Scope == Business` süzgeci eklenir; nakit tahsilat okuması
değişmez. Avans ve borç para gün sonunun nakit tarafında "nakit tahsilat"
gibi sorulmalı mı: **doğrulanacak** (kasaya giren ama satış olmayan para;
önerim sorulması).

**3 — Hesaba kartla ödeme.** `CounterpartyPayment.AccountId` nullable olur,
`CreditCardId?` eklenir; tam olarak biri doludur (Domain değişmezi ve SQL
CHECK). Yalnız `Payable` yönünde. `CardDebt` toplamına ve kart hareketlerine
üçüncü kaynak olarak girer; limit denetimi kart harcamasıyla aynı kuraldadır
(`limit − borç − bekleyen taksitler`). Gider yazmaz. Kart sayfasının hareket
okuması (`EfFinancePageReader`) bu kaynağı da listeler: **doğrulanacak**.

**4 — Vazgeçme ve kalanı indirim sayma.**

- *Kalanı indirim say:* yeni kayıt gerekmez. Var olan borçlandırma indirimi
  kullanılır; sunucu kalan tutarı en yeni açık borçlandırmalardan başlayarak
  indirim olarak yazar (indirim borçlandırmayı aşamaz). Ödeme ve indirimler
  tek yazma sınırında, tek istek kimliğiyle yazılır. Açılış bakiyesinden kalan
  tutar indirimle kapatılamaz (düşecek gider yok): o durumda vazgeçme yazılır.
- *Vazgeçme:* yeni kayıt (çalışma adı `CounterpartyWriteOff`): yön, tutar,
  gün, kategori, kapsam; kalan bakiyeyi aşamaz. Bağışlanan borç o gün gelir,
  vazgeçilen alacak o gün gider tanır; `RecognizedItems`e dokuzuncu kaynak
  olarak girer; bakiyede ödeme gibi düşer; para taşımaz.

**5 — Kişili taksitsiz `Debt`.** Sunucu açılışı **reddetmez**. Fişin
"henüz ödemedim" yolu ve yedek geri yükleme bu biçimi yazıyor ya da okuyor;
kural istemcide ve fiş yönlendirmesinde uygulanır (K9, K10). Liste okuması
(S7) bu kayıtları kayıt satırı olarak verir.

**6 — `/obligations` çağıranlar (Flutter).** `obligations` özelliğinin
kendisi; `app_router` (rota ve fiş yolu), `receipt_prefill`,
`quick_add_navigation`, `more_page`, `dashboard_view_model`,
`counterparty_models` ve `counterparty_repository` (kişideki `Bekleyen
faturalar`), `planned_activity_models`, `activity_repository`,
`api_error_messages`, `app_locations`, `app_dependencies`. Sunucuda uçlar
`Api/Features/Obligations`. Hepsi Faz 8'de kalkar; planlanan türlerin adı
(`payableObligation`, `receivableObligation`) sözleşmede kalıp kalmayacağı
Faz 3'te kararlaştırılır.

**7 — Hatırlatmalar.** Planlanan projection'dan beslenir ve cari vadeleri
göremez (§2). K1'den sonra borçların çoğu kişinin hesabında olacağı için bu
artık bir eksik: yeni iş **S11**.

**8 — Eski kişinin türü.** `Counterparty.Scope` nullable başlar ve
uydurulmaz. Okuma kuralı: türü boş kişi `Hepsi` görünümünde durur; `İşletme`
ya da `Şahsi` görünümünde, o kapsamda kaydı varsa (borçlandırma kapsamı, borç
kapsamı) görünür. Kişi sayfası türü boş kişide bir kez sorar. Yeni kişide tür
zorunludur (işletmesi olmayan kullanıcıda sunucu `Şahsi` yazar).

**9 — Kredi.** Bugünkü model K17 ile uyumlu: tutarları kullanıcı girer
(anapara, toplam), oran türetilir ve gösterim içindir. Türetilen tek şey
taksit başına anapara ve faiz payıdır (faizin hangi ay gider yazılacağı);
toplam faiz kullanıcının girdiğine eşittir. Değişiklik gerekmiyor. Formda
`Çekilen tutar`, `Taksit sayısı`, `Taksit tutarı`, `İlk taksit günü`
sorulur; toplam = taksit sayısı × taksit tutarı. KKDF ve BSMV ayrı
sorulmaz; taksitin içindedir. Devredilen kredide kalan taksit sayısı, taksit
tutarı, sıradaki taksit günü ve isteğe bağlı kalan anapara: `Debt.CarriedOver`
imzası **doğrulanacak**.

## 4. İş listesi

| # | İş | Dokunulan yerler |
|---|---|---|
| S1 | Borç para (`CounterpartyLoan`) | Domain; EF yapılandırması ve migration; `CounterpartyBalance`; bakiye SQL'i; `CounterpartyAging` (vadeli satır olarak); `CounterpartyNet`; `AccountMovements`; akış (yeni türler); hesap sayfası okuması; yedek; hesap silme; istek kimliği |
| S2 | Hesaba kartla ödeme | `CounterpartyPayment`; CHECK; `CardDebt`; kart sayfası okuması; akış |
| S3 | Borçlandırmada istek kimliği | `CreateCounterpartyChargeUseCase` (`RequestScopedId`, amaç `counterparty-charge`) |
| S4 | Kişide tür; borçlandırmada genel kapsam zinciri | `Counterparty`; migration (nullable); kişi uçları; gün sonu süzgeci; profil yoksa `Şahsi` |
| S5 | Vazgeçme; kalanı indirim sayma | `CounterpartyWriteOff`; `RecognizedItems`; `ICategoryUsageReader`; ödeme kullanım durumu |
| S6 | Kişi sayfası okuması | Yeni uç: özet (iki tutar, gecikmiş, sıradaki vade; taksitli kayıtlar dahil), kısayol tutarları, taksitli kayıt satırları, sayfalı ve süzülebilir hareketler |
| S7 | Liste okuması | `debt-book`un yeni biçimi: kapsam parametresi, iki toplam, yön başına kişi ve kayıt satırları (bölüm ve sıra sunucudan), kişiler, kapanan sayısı |
| S8 | Fiş "henüz ödemedim" yönlendirmesi | Kişi varsa vadeli alım, yoksa kişisiz `Debt` |
| S9 | Kalkanlar | `Api/Features/Obligations`, `Application/Obligations`, eski taksit ödeme ucu, profilde `hasCounterpartyLedger`, kişideki `OpenReceivableObligations` / `OpenPayableObligations` |
| S10 | Yedek | Yeni sürüm: kişinin türü, avanslar, vazgeçmeler, kartla ödeme alanı; eski sürümler okunur |
| S11 | **Hesaptaki vadeler planlananlara ve hatırlatmalara girer** | Planlanan projection'a kişi başına açık vadeli tutar (gecikme okumasıyla aynı sıra); yeni planlanan tür; hatırlatma türü istemcide |

## 5. Migration

Tek migration, sırası `AGENTS.md` kurallarıyla:

1. `Counterparties.Scope` nullable kolon (geçmiş bilinmiyor; backfill yok).
2. `CounterpartyLoans` ve `CounterpartyWriteOffs` tabloları (boş; sahiplik
   bileşik anahtarları, kişi ve hesap için bileşik yabancı anahtarlar).
3. `CounterpartyPayments.CreditCardId` nullable kolon; `AccountId` nullable'a
   çevrilir; sonra CHECK (tam olarak biri dolu). Var olan satırların hepsinde
   `AccountId` dolu olduğu için CHECK geçer.

Veri kaybettiren adım yok. Yükseltme dolu veritabanında test edilir.

## 6. Uçlar (taslak; kesin biçim Faz 2 ve 3'te, sözleşme belgesiyle)

| Uç | İş |
|---|---|
| `POST /api/v1/counterparties/{id}/loans` | Borç para |
| `DELETE /api/v1/counterparty-loans/{id}` | İptal |
| `POST /api/v1/counterparties/{id}/write-offs`, `DELETE …/{id}` | Vazgeçme ve iptali |
| `POST /api/v1/counterparties/{id}/payments` | + `creditCardId`, + `closeRemainderAsDiscount` |
| `POST /api/v1/counterparties/{id}/charges` | + `clientRequestId`, + `paidNow` (peşin ödenen: tutar ve hesap; tek yazma sınırı) |
| `POST`, `PUT /api/v1/counterparties` | + `scope` |
| `GET /api/v1/counterparties/{id}/page` | S6 |
| `GET /api/v1/debt-book?scope=` | S7 |

Hata kodları var olan adlandırmayı izler (`counterparties.*`); yeni kodlar:
`counterparties.card_requires_payment`, `counterparties.write_off_exceeds_balance`,
`counterparties.discount_exceeds_charges`, `counterparties.scope_required`.

## 7. Test listesi (senaryo numaraları `02-TASARIM.md` §9)

| Katman | Kanıt |
|---|---|
| Domain | Avansın değişmezleri; ödemede hesap ya da kart (tam olarak biri); vazgeçme kalanı aşamaz; bakiye avansı ve vazgeçmeyi sayar |
| Application | 2, 3, 4, 10, 17, 19, 20, 22; kapsam zinciri (13, 26); istek kimliği tekrarları |
| API | 1, 5, 6, 7, 11, 12, 16, 18, 21, 24, 25, 31; sahiplik izolasyonu (pozitif ve negatif) |
| Para tarafı | `MoneyFoundation_*`: avans hesap hareketinde var, gelir/gider toplamında yok; kartla ödeme kart borcunda var, giderde yok; vazgeçme giderde/gelirde bir kez |
| Gün sonu | Var olan testler değişmeden geçer; Şahsi kapsamlı satış sorulmaz |
| SQL | Migration dolu veritabanında; CHECK; bileşik anahtarlar; okumaların sorgu sayısı |
| Yedek | Yeni sürüm gidiş-dönüş; eski sürüm okunur; senaryo 15 |

## 8. Faz 2'nin başında yapılacak okuma

Okunmayan dosyalar (§1) okunur ve şu "doğrulanacak" maddeler kapatılır:

1. Avans ve borç para gün sonunun nakit tarafında sorulur mu?
2. Kart sayfasının hareket okuması yeni kaynağı nasıl alır; akışta kartla
   yapılan hesap ödemesi hangi türle yazılır?
3. `Debt.CarriedOver` devredilen kredinin alanlarını karşılıyor mu?
4. Akışta yeni türler (`counterparty-loan`, `counterparty-write-off`) ve
   Flutter'ın bilinmeyen türü reddetmesi: Faz 2 eski istemciyi kırmamalı;
   yeni türler istemci onları tanıyana kadar akışa nasıl girer?
5. Yedek şemasının güncel sürümü ve dönüştürme yeri.

Bulunan her çelişki `README.md` `Açık sorular`a yazılır.
