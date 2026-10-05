# İzin ve Kullanıcı İzolasyonu

## Güvenilir kullanıcı kimliği

Finansal kayıtlarda kullanılacak kullanıcı kimliği request body, query string
veya route içinden alınmaz. Application use case'leri kimliği `ICurrentUser`
üzerinden ister. API'deki `HttpContextCurrentUser`, JWT bearer middleware'in
doğruladığı principal içindeki `sub` claim'ini GUID'e çevirir. Eksik veya
geçersiz claim kimlik üretmez.

İstek DTO'sunda gönderilen bir UserId hiçbir finansal sahiplik kararının girdisi
değildir. `GetAccountQuery` ve `DeactivateAccountCommand` yalnız `AccountId`
taşır.

## Mevcut izin matrisi

MVP'de iki etkili rol vardır: anonim ve authenticated owner. Admin veya başka
kullanıcının kaydına erişen ayrıcalıklı rol yoktur.

| Kaynak / operasyon | Anonim | Authenticated owner | Authenticated non-owner |
|---|---|---|---|
| Register/login/refresh | İlgili public endpoint | İzinli | Sahiplik uygulanmaz |
| Logout | Token yoksa 401 | Kendi session'ı revoke | Başka session'a erişemez |
| Hesap/kategori CRUD | 401 | İzinli | 404 veya listeden dışlanır |
| Kendi hesabını okuma (`GET /api/v1/account`) | 401 | Yalnız kendi e-postası ve açık oturum sayısı | Başkasının hesabına yol yok |
| Oturum listeleme / tek oturum kapatma | 401 | Yalnız kendi oturumları; satır token veya hash taşımaz | Başkasının oturumu 404 ve açık kalır |
| Parola değiştirme | 401 | Mevcut parola doğrulanır; bütün oturumlar kapanır, çağıran cihaza taze çift döner | Başka kullanıcının parolasına yol yok |
| Hesap silme (`DELETE /api/v1/account`) | 401 | Parola + açık onay; yalnız kendi verisi (ADR 0017) | Başka kullanıcının verisine dokunan yol yok |
| E-posta doğrulama kodu isteme/onaylama | 401 | Yalnız kendi adresine; kod hash'lenir ve cevapta dönmez | Başka adresi doğrulatan yol yok |
| Parola sıfırlama isteme (`POST /api/v1/auth/password-reset`) | **İzinli, kimlik istemez** | Aynı cevap | Bilinmeyen ve bilinen adres aynı `202`'ye döner |
| Parola sıfırlamayı tamamlama | **İzinli, kimlik istemez** | Kod + yeni parola; bütün oturumlar kapanır | Bilinmeyen adres, yanlış/süresi geçmiş kod aynı `authentication.invalid_reset_code` |
| Transaction create/read/cancel | 401 | Aktif owner kaynağıyla izinli | 404/400 veya dışlanır |
| Bütçe create/update/read | 401 | Owner gider kategorisiyle izinli | 404/400 veya dışlanır |
| Dashboard/report | 401 | Yalnız kendi aggregate sonucu | Verisi toplama girmez |
| Transfer create/read/cancel | 401 | İki owner hesapla izinli | 404/400 veya listeden dışlanır |
| Kart create/update/read | 401 | Yalnız kendi kartında izinli | 404 veya listeden dışlanır |
| Kart harcama/ödeme/activity | 401 | Owner kart ve owner kategori/hesapla izinli | 404/400 veya dışlanır |
| Ekstre okuma | 401 | Yalnız owner kart projection'ı | 404 |
| Taksit create/list/realize | 401 | Owner kart/kategori/plan ile izinli | 404/400 veya dışlanır |
| Recurring create/list/active | 401 | Owner hesap/kategori ve current user planı | 404/400 veya dışlanır |
| Occurrence generate/list/realize | 401 | Yalnız current user plan/occurrence'ı | 404/400 veya dışlanır |
| Recurring düzenleme, "tutar belli oldu", "Ödedim"i geri alma | 401 | Plan ve occurrence `(Id, current UserId)` ile bulunur; yeni kaynak ve kategori ayrı ayrı owner kapsamında doğrulanır | 404 |
| Vergi ekranı, tanım ayrıntısı, Ödenenler | 401 | Plan, planlanan projection ve ödeme sorguları current `UserId` ile başlar | 404 / listeye girmez |
| Toplu vergi ödemesi ve geri alması | 401 | Hesap/kart, vergi kategorisi ve kapatılan her kalemin planı owner kapsamında bulunur; ödeme kimliği istek kimliği **ve kullanıcı kimliğinden** türetilir | 404; başkasının kalemi kapatılamaz |
| Borç create/list/pay | 401 | Owner hesap ve owner gider kategorisiyle izinli | 404/400 veya listeden dışlanır |
| Karşı taraf create/list/get/update/delete | 401 | Yalnız current user'ın karşı tarafı; `asOfDate` yalnız gecikme projection'ını belirler | 404; hareketi varsa 409 |
| Cari borçlandırma / tahsilat | 401 | Owner karşı taraf + owner kategori/hesap; opsiyonel vade sahiplik girdisi değildir | 404/400; pasif tarafa borçlandırma 409 |
| Cari hareket iptali | 401 | Yalnız current user'ın hareketi | 404 |
| Birleşik feed `counterpartyId` filtresi | 401 | Her satır zaten owner kapsamlı; filtre yalnız daraltır | Başkasının kimliği boş liste döner |
| Borç açılışını tamamlama | 401 | Yalnız current user'ın açılışı kayıtsız borcu | 404; kaydedilmişse 409 |
| Upcoming payments | 401 | Üç kaynağın tamamı current `UserId` ile başlar | Feed'e girmez |
| Advanced report | 401 | Bütün aggregate/join sorguları current `UserId` kapsamlıdır | Toplamlara girmez |
| Fiş analizi | 401 | Yalnız kendi aktif gider kategorileri modele verilir | Kategorisi modele/taslağa girmez; hiçbir kayıt yazılmaz |
| Profil okuma/güncelleme | 401 | Yalnız kendi profili; `ICurrentUser`'dan türetilir | Başkasının profiline erişemez |
| Health endpointleri | İzinli | İzinli | Finans verisi içermez |

| İşlem | Kimlik gerekli | Sahiplik kuralı | Başkasının kaydı |
|---|---|---|---|
| Hesap oluşturma | Evet | Yeni `Account.UserId`, current user olur | UserId girdisi kabul edilmez |
| Hesap listeleme | Evet | Repository yalnız current user kayıtlarını listeler | Listeye dahil edilmez |
| Tek hesap okuma | Evet | `Account.Id` ve `Account.UserId` birlikte eşleşir | `accounts.not_found` |
| Hesap pasifleştirme | Evet | Bulma ve update current user ile kapsamlanır | Değişiklik yok, `accounts.not_found` |
| Hesap güncelleme/bakiye | Evet | `AccountId + current UserId`; bakiye yalnız sahip hareketlerinden hesaplanır | `accounts.not_found` |
| Kategori oluşturma/listeleme | Evet | Varsayılan ve özel satırların `UserId` değeri current user olur | Listeye dahil edilmez |
| Kategori güncelleme/pasifleştirme | Evet | `CategoryId + current UserId` | `categories.not_found` |
| Gelir/gider oluşturma | Evet | Account ve Category ayrı ayrı current user ile bulunur | `transactions.account_unavailable` veya `category_unavailable` |
| Hareket iptali | Evet | `TransactionId + current UserId` | `transactions.not_found` |
| Hareket liste/detay | Evet | İlk sorgu koşulu current `UserId`; diğer filtreler bundan sonra uygulanır | Listeye girmez / `transactions.not_found` |
| Aylık bütçe yazma/okuma | Evet | Kategori ve bütçe current `UserId` ile bulunur | `category_unavailable` / `budgets.not_found` |
| Aylık bütçe silme (`DELETE /api/v1/budgets/{id}`) | Evet | `FindOwned`+`Remove` tek sorguda `(Id, UserId)` ile; başkasının bütçesi ve var olmayan bütçe aynı sonuca gider | `budgets.not_found` |
| Aylık rapor/dashboard | Evet | Bütün aggregate ve join sorguları current `UserId` ile başlar | Toplamlara, gruplara ve bakiyeye girmez |
| Transfer oluşturma | Evet | Source ve destination ayrı ayrı `AccountId + current UserId` ile bulunur | `transfers.account_unavailable` |
| Transfer liste/detay/iptal | Evet | Transfer sorgusu `TransferId + current UserId` ile kapsamlanır | Listeye girmez / `transfers.not_found` |
| Kart oluşturma/liste/güncelleme | Evet | Kartın `UserId` değeri current user; sorgular owner predicate taşır | Listeye girmez / `credit_cards.not_found` |
| Kart harcaması | Evet | Kart ve gider kategorisi current user ile ayrı ayrı doğrulanır | Kaynak erişilemez hatası; kayıt yok |
| Kart ödemesi | Evet | Kart ve banka hesabı current user ile ayrı ayrı doğrulanır | Kaynak erişilemez hatası; kayıt yok |
| Kart activity/ekstre | Evet | İlk koşul `CreditCardId + current UserId`; bütün toplamlar aynı owner kapsamındadır | `credit_cards.not_found` |
| Taksit planı/gerçekleştirme | Evet | Kart, kategori ve plan current user ile bulunur; request ID owner kapsamında tekildir | Listeye girmez / not found |
| Recurring tanımı | Evet | Hesap/kategori current user ile bulunur; yeni plan `UserId`yi oturumdan alır | Kaynak erişilemez / listeye girmez |
| Occurrence üretme/onaylama | Evet | Due sorgusu ve occurrence lookup current `UserId` taşır; key owner kapsamında tekildir | Üretilmez / not found |
| Recurring düzenleme (`PUT /api/v1/recurring-transactions/{id}`) | Evet | Plan `(Id, current UserId)`; yeni hesap/kart ve kategori ayrı ayrı current user ile bulunur | `recurring.not_found` |
| Kaleme tutar yazma / ödemeyi geri alma | Evet | Plan ya da occurrence `(Id, current UserId)`; üretme yalnız o planı ilerletir | `recurring.not_found` / `recurring.occurrence_not_found` |
| Vergi ekranı (`GET /api/v1/taxes`, `/taxes/plans/{id}`) | Evet | Plan listesi, planlanan projection ve Ödenenler sorgusu current `UserId` ile başlar; vergi olmayan plan ayrıntıda bulunmaz | `taxes.plan_not_found` |
| POS tanımı listeleme/oluşturma (`GET`/`POST /api/v1/pos-definitions`) | Evet | Liste yalnız current user; hesap ve iki kategori `(Id, current UserId)` ile okunur, başka kullanıcının hesabı ya da kategorisiyle tanım kurulamaz | `pos_definitions.account_unavailable` / `*_category_unavailable` |
| POS tanımını güncelleme, pasife alma, ana POS yapma, silme, önizleme (`PUT`/`PATCH .../active`/`PUT .../default`/`DELETE`/`GET .../preview` `/api/v1/pos-definitions/{id}`) | Evet | Tanım `(Id, current UserId)`; var olmayan ve başka kullanıcıya ait kimlik aynı cevabı alır. Tahsilat isteğindeki `posDefinitionId` de aynı kapsamda okunur | `pos_definitions.not_found` / `pos_settlements.definition_unavailable` |
| Toplu vergi tanımlama (`POST /api/v1/taxes/plans`) | Evet | Her öğe tek plan oluşturmanın kuralıyla: hesap/kart ve kategori `(Id, current UserId)`; başka kullanıcının kategorisi `recurring.category_unavailable` ile reddedilir ve **hiçbir öğe yazılmaz** (tek `SaveChanges`) | `recurring.*_unavailable` |
| Toplu vergi ödemesi (`POST /api/v1/tax-payments`) | Evet | Hesap/kart ve kategori current user ile; kapatılan her kalemin planı `(Id, current UserId)`; ödeme kimliği `clientRequestId` + current `UserId`'den türetilir, iki kullanıcı aynı istek kimliğiyle çarpışamaz | `tax_payments.item_not_found` / kaynak erişilemez |
| Vergi ödemesini geri alma (`POST /api/v1/tax-payments/{id}/undo`) | Evet | Gider ya da kart harcaması `(Id, current UserId)`; kapattığı kalemler de owner kapsamında açılır | `tax_payments.not_found` |
| Upcoming ve advanced report | Evet | Her kaynak sorgusunun ilk filtresi current `UserId`dir | Feed ve aggregate'e girmez |
| Fiş fotoğrafı analizi | Evet | Kategori sorgusu `current UserId + Expense + Active` ile sınırlıdır; request `UserId` taşımaz | Kategori adı modele verilmez ve kimliğe çözülemez; endpoint salt okunurdur |

Başkasının kaydı için Forbidden yerine NotFound dönülür. Böylece saldırgan,
tahmin ettiği AccountId'nin sistemde bulunup bulunmadığını hata farkından
öğrenemez.

API integration testinde iki kullanıcı aynı izole test sunucusunun persistence
katmanını paylaşır. Kullanıcı A hesap ve hareket oluşturduktan sonra B'nin
listeleri boş, A'nın hareket detayına B'nin isteği `404` kalır. Gerçek SQL
persistence testi de aynı hesabın farklı kullanıcı kapsamından okunamadığını
kanıtlar. Böylece JWT doğrulama yanında repository owner predicate'i iki test
katmanında doğrulanmıştır.

Transaction liste route'u bearer token gerektirir ve request'ten `UserId` kabul
etmez. Owner-scoped sorgu uygulanmıştır; geçerli filtre sayfalı
`200` döner ve başka kullanıcı satırları sonuca girmez.

Transfer ve kart request modelleri de `UserId` kabul etmez. Transferin iki account FK'si,
kart hareketlerinin kart/hesap/kategori FK'leri ve taksit planı/item ilişkileri
`UserId` içeren composite key'lerle aynı sahibin kaynaklarına bağlanır. Repository
predicate'i birinci, SQL composite FK/owner unique index'leri ikinci savunma
katmanıdır. API negatif testleri başka kullanıcının transfer/kart kaydını 404 veya
boş liste olarak; gerçek SQL mapping testleri çapraz owner ilişkisinin model
tarafından sınırlandığını kanıtlar.

Recurring ve rapor request/query modelleri de kullanıcı kimliği kabul etmez. Recurring
tanımı, occurrence ve gerçekleşen transaction ilişkileri `UserId` taşıyan
composite key/FK'lerle aynı sahibin kaynaklarına bağlanır. Occurrence unique key'i
başka kullanıcıyı küresel olarak kilitlemez; tekillik owner kapsamındadır.
Upcoming ve advanced report sorgularında recurring, installment, card, account,
budget ve transaction kaynaklarının her biri current user filtresiyle başlar.
Gerçek SQL fixture'ları yabancı kullanıcının upcoming/rapor sonucunu
değiştirmediğini doğrular.

## Kapsam sahiplik sınırını değiştirmez

Kapsam (`scope`) bir **raporlama boyutudur**, izin değildir. Sahiplik kontrolünün
yerine geçmez, üstüne biner: her sorgu önce `ICurrentUser` üzerinden gelen
`UserId` ile kapsanır, kapsam ondan sonra eler. Başka kullanıcının kaydına
`scope=business` göndererek erişilemez — kapsam filtresi owner predicate'ini
gevşetmez.

Kapsam alanını taşıyan istek/cevap sözleşmeleri:

| Endpoint | Alan | Zorunlu mu |
|---|---|---|
| `POST /api/v1/transactions` | `scope` | Hayır — boşsa hesap → kategori zincirinden çözülür; çözülemezse `transactions.scope_unresolved`, tanınmayan değer `transactions.invalid_scope` |
| `POST /api/v1/budgets` | `scope` | Hayır — boşsa kategoriden çözülür; `budgets.scope_unresolved` / `budgets.invalid_scope` |
| `POST /api/v1/credit-cards/{id}/charges` | `scope` | Hayır — boşsa kart → kategori; `credit_cards.scope_unresolved` / `credit_cards.invalid_scope` |
| `POST /api/v1/installment-plans` | `scope` | Hayır — boşsa kart → kategori; `credit_cards.scope_unresolved` / `installments.invalid_scope` |
| `POST /api/v1/recurring-transactions` | `scope` | Hayır — boşsa kaynak → kategori; `recurring.scope_unresolved` / `recurring.invalid_scope` |
| `POST /api/v1/recurring-transactions` | `occurrenceLimit` | Hayır — verilirse pozitif toplam occurrence sınırıdır; `endDate` ile birlikte verilebilir ve önce dolan sınır planı pasifleştirir. Sahiplik girdisi değildir |
| `POST /api/v1/debts` | `scope` | Hayır — boşsa açılış hesabı → kategori; `debt.scope_unresolved` / `debt.invalid_contract` |
| `POST`/`PUT` hesap, kategori, kart | `defaultScope` | Hayır — boş bırakılabilir; **güncellemede yetkilidir**, boş göndermek etiketi kaldırır. Tanınmayan değer `*.invalid_default_scope` |
| `POST /api/v1/imports/{id}/confirm` | — | İçe aktarılan CSV kapsam kolonu taşımaz; zincirin ilk halkası hiç dolmaz, hesabın yoksa kategorinin varsayılanı kullanılır, ikisi de boşsa `imports.scope_unresolved` |
| `GET /api/v1/exports/transactions.csv` | `scope` | Dosya her satırın kapsamını `type`'ın yanında taşır. Dışa aktarma okumak ve arşivlemek içindir; aynı dosya içe aktarılamaz (istemci tanır ve reddeder), veri taşımanın yolu yedek/geri yüklemedir |
| `GET /api/v1/goals` | isteğe bağlı `scope` | Hedefler owner kapsamlıdır. Filtreli okuma kapsamsız hedefleri de eler; kırılım yalnız filtresiz okumada döner |
| `GET /api/v1/tax-calendar/suggestions` | — | Hazır vergi türleri; owner verisi okumaz, hiçbir şey yazmaz, tutar ve kategori taşımaz. Tür mevcut tekrarlayan plan ucundan `taxKind` ile kurulur |
| `POST`/`PUT /api/v1/recurring-transactions` (vergi planı), `POST /api/v1/taxes/plans` | `scope` | Hayır — boşsa profilin tarafı; **hesabın/kartın etiketine bakılmaz**; sonuç hiç boş değildir (ADR 0018 İ9, 30 Eylül 2026). Tanınmayan değer `recurring.invalid_scope` |
| `POST /api/v1/tax-payments` | `scope` | Hayır — boşsa profilin tarafı; hesabın/kartın etiketine bakılmaz (İ9). İstemci göndermez. Tanınmayan değer `tax_payments.invalid_scope` |
| `POST`/`PUT` kategori | `isTax` | Hayır — yalnız gider kategorisi işaretlenir; güncellemede boş göndermek işareti **değiştirmez**. Sahiplik girdisi değildir |
| `GET /api/v1/exports/counterparty-ledger.csv` | `scope` (yalnız borçlandırma satırlarında) | Cari defterin dökümü; owner kapsamlı, `ICurrentUser`'dan türetilir. Borçlandırma kategori ve kapsam taşır, hesap kolonu boştur; tahsilat hesap taşır, kategori ve kapsam kolonları boştur (ADR 0014) — `kind` kolonu hangisinin okunacağını söyler. İşlem CSV'sine kolon eklenmedi: o dosya `BudgetTransaction` dökümüdür ve cari hareket orada bulunmaz. Bu dosya da geri yüklenemez |

Kart ödemesi ve transfer endpoint'leri kapsam **almaz**: gelir/gider raporuna
sıfır etki ederler.

Kapsam **okuma** filtresi alan endpoint'ler — hepsinde isteğe bağlı `scope`
query parametresi, tanınmayan değer `*.invalid_scope`:

| Endpoint | Bölünen | Bölünmeyen |
|---|---|---|
| `GET /api/v1/reports/monthly` | Gelir, gider, kategori dağılımı | `accountBalances` |
| `GET /api/v1/dashboard` (aynı uç nokta) | Aynı | Aynı |
| `GET /api/v1/reports/advanced` | Dönem karşılaştırması, nakit akışı, bütçe sapması | `netWorth`, `accountDistribution`, `cardDistribution` |
| `GET /api/v1/financial-activities` | Feed'in tamamı | — |
| `GET /api/v1/financial-activities/planned` | Listenin tamamı | — |

`GET /api/v1/upcoming-payments` kapsam parametresi **almaz**: ödenecek para tek
havuzdan çıkar. Her iki uçta da tek seferlik yükümlülük sorgusu current
`UserId` ile başlar; başka owner'ın aynı tarih ve tutardaki kaydı görünmez.

Aylık rapor **filtresiz** okunduğunda ayın iki tarafını ayrı ayrı toplayan bir
`scopeBreakdown` alanı da döner (`business` / `personal`, her biri
`income`/`expense`/`net`). Filtreli okumada alan `null`'dır: rapor zaten tek
tarafı anlatıyordur. Kırılım sahiplik sınırını değiştirmez — aynı owner
predicate'inin içinde, yalnız kapsama göre gruplanmış toplamlardır.

Kapsam değeri kararlı makine metnidir (`business` / `personal`); kullanıcıya
gösterilecek cümleyi istemci üretir (`İşletme` / `Şahsi` / filtrede `Hepsi`).

Flutter istemcisi bu sözleşmeyi şöyle kullanır: kapsam boyutu görünmeyen
kullanıcıda **hiçbir istekte** `scope` gitmez (ne filtre ne yazma alanı);
görünen kullanıcıda okuma isteklerine aktif anahtar konumu, yazma isteklerine
formda gösterilen değer eklenir. İstemcinin kapsam göndermesi hiçbir yerde
zorunlu değildir — göndermediğinde sunucu zinciri kendi işletir.

## Defense in depth

SQL Server'da row-level security (RLS) politikası yoktur. Birincil authorization
katmanı zorunlu `UserId` predicate'idir. `(UserId, resourceId)` composite foreign
key'leri ve owner-scoped unique index'ler ikinci savunma katmanıdır; RLS yerine
geçtiği iddiası değildir. Yeni sorgularda owner predicate ile pozitif ve negatif
izolasyon testi zorunludur.

Sahiplik yalnız controller veya istemci kontrolüne bırakılmaz. Use case
`ICurrentUser.UserId` değerini alır; repository portu hem hesap ID'sini hem
kullanıcı ID'sini ister. Update portu da kullanıcı ID'sini yeniden ister. Aşama
06 Grup 3 EF modeli transaction ve bütçe ilişkilerini `(UserId, kayıt ID)`
composite foreign key'leriyle sınırlar. Gerçek repository sorgu/update
predicate'leri SQL Server integration testlerinde; composite FK ve owner-scoped
unique index'ler model/migration testlerinde doğrulanmıştır.

Borç sözleşmesi üç ilişki taşır: açılışın hesabı, kategorisi ve **karşı
tarafı**. Üçü de aynı desenle bağlıdır — `(UserId, OpeningAccountId)`,
`(UserId, CategoryId)` ve `(UserId, CounterpartyId)` composite foreign
key'leri. Başka kullanıcının hesabına, kategorisine ya da karşı tarafına
bağlanan bir borç, Application katmanı hiç devreye girmese bile veritabanı
seviyesinde yazılamaz. `POST /api/v1/debts` karşı tarafı **addan** çözer ve
yalnız current user'ın kayıtları arasında arar; bulamazsa o kullanıcıya ait
yeni bir karşı taraf kurar. Cevap `counterpartyId` taşır.

Cari liste ve tekil okuma `asOfDate` alabilir; bu değer kullanıcı kimliği veya
sahiplik seçimi değildir, yalnız vadesi geçmiş tutarın hangi güne göre
türetileceğini belirler. Her alt sorgu önce current `UserId` ile daralır.
`CounterpartyCharge.dueDate` isteğe bağlıdır; mevcut geçmişin bilinmeyen vadesi
`null` kalır. API toplam, gecikmiş ve vadesi geçmemiş/vadesiz alacak-borç
tutarlarını kararlı para dizeleri olarak döndürür.

`POST /api/v1/obligations` aynı savunmayı Application ve veritabanı katmanlarında taşır:
kategori ve isteğe bağlı karşı taraf, settlement'ın hesabı ve bağlı olduğu
yükümlülük `(UserId, kayıt ID)` composite foreign key'leriyle sınırlıdır.
`(UserId, ObligationId)` tekilliği farklı bir kullanıcıya bağlanmayı ve aynı
yükümlülüğe ikinci kapanış yazmayı veritabanı seviyesinde reddeder. Use case
kategoriyi ve verilmişse karşı tarafı yalnız current user'ın aktif kayıtları
arasından çözer; kimlik request body'den alınmaz. `GET /api/v1/obligations`
yalnız current user satırlarını döndürür. `POST
/api/v1/obligations/{id}/settlement` hem yükümlülüğü hem hesabı current user
kapsamında çözer; yabancı yükümlülük `404`, yabancı/pasif/para birimi uyumsuz
hesap doğrulama hatasıdır. Aynı yükümlülüğe ikinci çağrı mevcut settlement'ı
döndürür. Gerçekleşen ve planlanan feed okumaları da owner-scoped kalır; API
entegrasyon testi yabancı listenin boş ve yabancı settlement'ın reddedildiğini
kanıtlar.

`POST /api/v1/cash-counts` ve `POST /api/v1/pos-settlements` aynı deseni
sürdürür. Sayımın hesabı, düzeltmenin kategorisi, tahsilatın hesabı ile gelir ve
komisyon kategorileri yalnız current user'ın aktif kayıtları arasından çözülür;
kimlik hiçbir gövdeden okunmaz. Yabancı ya da uygun olmayan hesap `404`,
yabancı kategori doğrulama hatasıdır. `POST /api/v1/cash-counts/{id}/adjustment`
ve `DELETE /api/v1/pos-settlements/{id}` (tahsilatın iptali) kaydı
`(UserId, kayıt ID)` ile çözer; başkasının kaydı ile var olmayan kayıt aynı
`404` sonucuna gider.

POS yatışının dört ucu (Aşama 06.3 Grup 5) aynı sınırı taşır.
`POST /api/v1/pos-deposits` ve `GET /api/v1/pos-deposits/preview` tahsilat
kimliklerini gövdede ve sorgu dizesinde alır; tahsilatlar yalnız current user'ın
satırları arasından çözülür ve **biri bile** bulunamazsa cevap
`404 pos_deposits.settlement_not_found`'dur — başkasının tahsilatı ile var
olmayan tahsilat ayırt edilemez. Kesinti kategorisi de owner-scoped çözülür.
`GET` ve `DELETE /api/v1/pos-deposits/{id}` yatışı `(UserId, kayıt ID)` ile
çözer. Sahiplik SQL'de de durur: yatış hesaba ve kesinti giderine, tahsilat
yatışa `UserId` ile başlayan bileşik foreign key'lerle bağlanır; başka
kullanıcının yatışına tahsilat bağlanamaz. Kimlik taşıyan iki uç ve önizleme
`OwnershipIsolationTests` prob tablosundadır; gövdeyle çalışan `POST`
`PosDepositEndpointTests` içinde ölçülür.

Gün sonunun beş ucu (Aşama 06.3 Grup 5) aynı sınırı taşır.
`POST /api/v1/day-closes` ve `POST /api/v1/day-closes/preview` POS, kasa ve
kategori kimliklerini gövdede alır; hepsi yalnız current user'ın satırları
arasından çözülür. Başkasının POS'u ile var olmayan POS aynı
`404 day_closes.pos_unavailable`, başkasının kasası ile var olmayan kasa aynı
`404 day_closes.cash_account_unavailable` cevabını alır. "Zaten girilmiş
kayıtlar" listesi ve "bu gün kapatıldı" bilgisi yalnız o kullanıcının
kayıtlarından kurulur; başka kullanıcının aynı günü kapatmış olması görünmez.
`GET /api/v1/day-closes?from=&to=` ve `GET /api/v1/day-closes/day?date=`
yalnız current user'ın gün sonlarını ve kayıtlarını döner. Sayılan kaydın bağı
(`DayCloseCountedRecords`) `UserId` taşır ve "sayıldı mı" okuması owner-scoped
yapılır: başka kullanıcının bağı bir kaydı kilitleyemez.
`GET` ve `DELETE /api/v1/day-closes/{id}` gün sonunu `(UserId, kayıt ID)` ile
çözer. Sahiplik SQL'de de durur: gelir ve tahsilat gün sonuna `UserId` ile
başlayan bileşik foreign key'le bağlanır; tekil indeksler `UserId` ile başlar,
yani iki kullanıcı aynı günü ve aynı Z numarasını ayrı ayrı kapatabilir.
Kimlik taşıyan iki uç ve liste `OwnershipIsolationTests` içindedir; gövdeyle
çalışan iki `POST` `DayCloseEndpointTests` içinde ölçülür.

`GET /api/v1/financial-activities/{activityKind}/{activityId}/balances` (işlem
sonrası bakiye, 2 Ekim 2026) hareketi kendi tablosunda `(UserId, kayıt ID)` ile
arar; başkasının hareketi, olmayan hareket ve türü tutmayan kimlik aynı `404`
cevabını alır. Dönen hesap ve kart da owner-scoped okunur; bakiye toplamı yalnız
o kullanıcının hareketlerinden kurulur. Uç `OwnershipIsolationTests` prob
tablosundadır.

29 Eylül 2026'da eklenen `POST`/`DELETE /api/v1/pos-settlements/{id}/transfer`
uçları 2 Ekim 2026'da **kalktı**: para hesaba yalnız yatışla geçer.
`GET /api/v1/cash-counts`, `GET /api/v1/cash-counts/today` ve
`GET /api/v1/pos-settlements` yalnız current user satırlarını döndürür — yoldaki
toplam da öyle; tahsilatı sayan gün sonunun kimliği (`countedInDayCloseId`) de
`DayCloseCountedRecords`'tan aynı `UserId` süzgeciyle okunur. `today` cevabındaki son sayım ve bugünkü nakit giriş/çıkış da
aynı `(UserId, AccountId)` süzgeciyle okunur; hesap önce sahiplikle çözülür. API entegrasyon testleri iki ucun da yabancı okumasının boş ve
yabancı yazmasının reddedildiğini kanıtlar.

Fiş analizi sahiplik sınırını yazmadan uygular. Use case current
user kimliğini `ICurrentUser`dan alır ve modele yalnız o kullanıcının aktif
gider kategorilerinin adlarını kapalı küme olarak verir. Sağlayıcıdan dönen ad
aynı owner-scoped liste içinde kimliğe çözülür; benzer ada veya küresel kategori
aramasına düşülmez. API entegrasyon testi A'nın özel kategorisinin prompt'a
girdiğini, B'ninkinin girmediğini ve çağrıdan sonra finansal tablo satır
sayısının değişmediğini birlikte kanıtlar.

## Mobil istemci güven sınırı

Flutter request body veya query alanına `UserId` eklemez. Kullanıcı sahipliği,
secure storage'daki access tokenın API tarafından doğrulanan `sub` claim'inden
türetilir. Mobil route guard yalnız ekran erişimini düzenler; güvenlik kararı
değildir. Başka istemci route guard'ı atlayabileceği için owner predicate ve
Domain doğrulaması sunucuda çalışmaya devam eder.

Access ve refresh token ile iki expiry değeri secure storage'da tek session
kaydı olarak tutulur; parola saklanmaz. Refresh rotation cevabı eski kaydın
yerine atomik yazılır. Aynı anda oluşan yenileme talepleri tek uçuşta birleştirilir;
bu, eski refresh tokenın paralel tekrar kullanımının session reuse savunmasını
yanlışlıkla tetiklemesini önler.

CSV, JSON ve backup için `Cihaza kaydet` akışı Android Storage Access Framework
üzerindeki `ACTION_CREATE_DOCUMENT` intent'ini kullanır. Kullanıcı hedef URI'yi
sistem ekranında açıkça seçer; uygulama genel dosya sistemi veya Downloads
klasörü izni istemez. Flutter–Android platform kanalından yalnız o anda
indirilen dosya byte'ları, önerilen dosya adı ve MIME türü geçer; token, UserId
veya sunucu secret'ı native katmana aktarılmaz.

## Sahiplik denetimi tek yerde ölçülür (28 Ağustos 2026, Aşama 06.1 Grup 3)

İzolasyonun kanıtı uçlara dağıtılmış testlere bırakılmadı. Dağıtılmış testlerde
**olanı görmek kolay, eksik olanı görmek zordur**; bu grubun işi ise eksik test
aramaktı. `OwnershipIsolationTests` denetimi tek dosyada toplar ve üç şey ölçer.

**1. Kimlik taşıyan her uç (46 uç).** Sahip her kayıt türünden bir tane kurar;
saldırgan her ucu iki kez çağırır — bir kez sahibin gerçek kimliğiyle, bir kez
hiç var olmamış bir kimlikle. İki cevabın **statüsü aynı ve 404 olmak**
zorundadır. Farklı cevap vermek (biri 404, öteki 403 ya da 409) kaydın
varlığını sızdırır ve saldırgana kimin nesi olduğunu haritalama imkânı verir.

**2. Kimlik taşımayan okuma uçları (31 uç).** Burada denenecek kimlik yoktur;
sızıntı olursa sunucunun kendi listesinden gelir. Ölçü cevabın şekline bakmadan
kurulur: sahibin hiçbir kimliği saldırganın gövdesinde geçmemelidir. Aynı test
sahibin o kimlikleri **gördüğünü** de doğrular — görünmeyen bir kayıt için
"saldırgan görmüyor" demek bir şey söylemez.

**3. Listenin kendisi.** Prob tablosu elle tutulur ve elle tutulan liste eskir.
`EveryOwnerScopedRoute_IsProbed` onu uygulamanın gerçek route tablosuyla
karşılaştırır: kimlik taşıyan yeni bir uç eklenip tabloya yazılmazsa takım
kırmızıya döner. Denetim dışında bırakılan bir uç, gerekçesiyle birlikte
`NotOwnerScoped` içine yazılır — bugün o liste **boştur**.

### Denetimin bulduğu

Kırk altı ucun kırk beşi kuralı zaten uyguluyordu. Bir tanesi uygulamıyordu:
**taksit gerçekleştirme**, bulunamayan plan için `400` dönüyordu. Başka
kullanıcının planı da aynı `400`'ü aldığı için sızıntı değildi, ama ürünün
kendi kuralını bozuyordu ve istemcinin hata sözlüğünde karşılığı olmayan bir
koda düşüyordu. `installments.not_found` ile `404`'e çevrildi.

## Admin bypass durumu

Mevcut MVP'de rol, admin claim'i veya sahipliği atlayan repository metodu yoktur.
Request modelleri `IsAdmin`, `Role` veya alternatif UserId taşımaz. Gelecekte
yönetim ihtiyacı doğarsa açık politika, audit ve ayrı testlerle tasarlanmalıdır;
bugünkü sorgulara gizli koşul eklenmez.

## Henüz uygulanmayan sınırlar

- Hane üyeliği/rolleri — ilgili sonraki roadmap kapsamı
- Hesap silme ve dışa aktarma **uygulandı**: silme ADR 0017'ye göre gerçek silmedir
  (parola + açık onay), dışa aktarma yedek/geri yükleme yolundadır

## İşlem araması (27 Eylül 2026)

`GET /api/v1/financial-activities?search=` yeni bir uç değildir; mevcut feed
sorgusunun bir filtresidir ve sahiplik kapsamı (`UserId`) aynı sorguda
uygulanır. Başka kullanıcının kaydı eşleşmez; SQL integration testi
(`FinancialActivityFeed_SearchMatchesVisibleTextAndCountsOnlyMatches`) iki
kullanıcılı veriyle bunu doğrular.

## Aşama 06.3 Grup 5 — Kartla tahsil (5 Ekim 2026)

`POST /api/v1/counterparties/{id}/payments` ve
`POST /api/v1/obligations/{id}/settlement` isteğe bağlı bir `card` bloğu alır
(`posDefinitionId`, `accountId`, `commissionAmount`, `commissionRate`,
`commissionCategoryId`, `expectedTransferDate`). POS, hesap ve komisyon
kategorisi current user kapsamında okunur; başkasının POS'u var olmayanla aynı
`404 pos_settlements.definition_unavailable` cevabını alır ve hiçbir kayıt
yazılmaz (`CardCollectionEndpointTests`). Kart yalnız tahsilatta geçerlidir
(`counterparty_payments.card_requires_collection`,
`obligations.card_requires_receivable`). Tahsil kaydı tahsilatla tek
`SaveChanges` sınırında yazılır; bağ `(UserId, PosSettlementId)` composite
foreign key'idir. `DELETE /api/v1/pos-settlements/{id}` tahsil kaydında
`409 pos_settlements.collection_locked`; cari tahsilatın iptali para yatışla
geçtiyse `409 counterparty_payments.deposit_locked`, gün sonunda sayıldıysa
`409 counterparty_payments.day_close_counted` döner.
`GET /api/v1/pos-settlements` satırı tahsilde boş `categoryId`/`categoryName`,
`kind` ve kişinin adını (`counterpartyName`) taşır; ad owner-scoped okunur.
Yatış ayrıntısı brütü `saleAmount` ve `collectionAmount` olarak ayırır.
