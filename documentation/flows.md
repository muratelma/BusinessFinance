# Kimlik ve İzin Akışları

## Akış okuma sözleşmesi

Kritik akışlarda aktör mobil kullanıcıdır. Başlangıç koşulu yerel SQL ve API'nin
hazır, APK'nın doğru `API_BASE_URL` ile kurulmuş olmasıdır. Başarı sonucu yalnız
JWT'deki current user'a ait verinin kalıcılaşıp görünmesidir. `[TB]` güven sınırı
geçişini, `ALLOW/DENY` yetkilendirme kararını gösterir.

## API başlangıç ve hata pipeline'ı

```text
HTTP request
  -> Global exception boundary
  -> HTTPS redirection
  -> route/endpoint
  -> Application use case
  -> HTTP response

Beklenmeyen exception
  -> güvenli 500 ProblemDetails
  -> kararlı server.unexpected_error kodu + traceId
  -> exception mesajı response'a girmez
```

OpenAPI yalnız Development ortamında `/openapi/v1.json` adresinden sunulur.
Use case DI kayıtları API composition root'unda tutulur; Application katmanı DI
framework'ü veya ASP.NET Core bağımlılığı almaz.

## Kayıt

```text
POST /api/v1/auth/register (email + parola + hasBusiness JSON)
  -> RegisterRequest -> RegisterUserCommand
  -> RegisterUserUseCase
  -> IIdentityAccountService
  -> Identity UserManager
  -> parola politikası + hash + normalize e-posta
  -> UserProfile (hasBusiness) yazılır
  -> 201 RegisterResponse (UserId + e-posta)
```

Parola yalnız doğrulama/hash girdisidir; response veya Domain finans nesnesine
girmez. Identity kullanıcı ve parola hash'i EF Core store üzerinden SQL
Server'da kalıcıdır.

`hasBusiness` onboarding'in tek sorusudur ve **hiçbir özelliği kapatmaz**:
yalnız hangi varsayılan kategori setiyle başlanacağını ve kapsam boyutunun
arayüzde görünüp görünmeyeceğini belirler. Gönderilmezse "işletmesi yok"
sayılır. Profil kayıtla aynı akışta yazılır; sonraya bırakmak, kişisel setle
başlayan bir esnafa işletme kalemlerini bir daha hiç veremezdi çünkü set yalnız
hiç kategorisi olmayan kullanıcıya bir kez uygulanır.

İlk `GET /api/v1/categories` çağrısı seti kurar. `GET /api/v1/profile` istemciye
kapsam boyutunu gösterip göstermeyeceğini söyler; `PUT /api/v1/profile` cevabı
değiştirir ve yalnız arayüzü etkiler — kategoriler olduğu gibi kalır.

## Profil ön ayarlı ana gezinme

```text
hasBusiness = true   -> Özet · İşlemler · Kasa      · Diğer
                         Diğer -> Bütçeler

hasBusiness = false  -> Özet · İşlemler · Bütçeler · Diğer
                         Diğer -> Kasa
```

Üçüncü dal aynı shell geri yığınını korur; yalnız etiketi, ikonu ve gösterdiği
ekran profil cevabına göre değişir. Bu bir yetki veya özellik bayrağı değildir.
Profil cevabı sonradan değişirse ana hedef ile `Diğer` altındaki hedef yer
değiştirir; ikisi de erişilebilir kalır (ADR 0013 ve ADR 0015).

## Gün sonu kasa sayımı

```text
Kasa -> Gün sonu -> nakit hesap seç
     -> GET /api/v1/cash-counts/today?accountId=...
     -> beklenen bakiye sunucudan gelir
     -> sayılan tutarı gir -> POST /api/v1/cash-counts
     -> fark yalnız gösterilir; bakiye/rapor değişmez
     -> kullanıcı "Farkı kaydet" der ve kategori seçer
     -> POST /api/v1/cash-counts/{id}/adjustment
     -> tek gelir/gider kaydı doğar
```

İstemci beklenen tutarı veya farkı hesaplamaz. Aynı gün ikinci sayım öncekini
iptal eder; geçmiş gözlem silinmez. Sayım tuttuysa düzeltme eylemi yoktur.

## POS tahsilatı ve hesaba geçiş

```text
Kasa -> POS tahsilatları -> Tahsilat ekle
     -> banka hesabı + gelir kategorisi + brüt + tarih/vade
     -> komisyon: yok | tutar | oran (yalnız biri)
     -> POST /api/v1/pos-settlements
     -> brüt gelir ve komisyon gideri tanınır; hesap değişmez

Yolda satırına dokun -> görünür onay
     -> POST /api/v1/pos-settlements/{id}/transfer
     -> hedef hesap net kadar artar; gelir/gider yeniden yazılmaz
```

`Yolda` toplamı net tutardır ve liste tarih aralığından bağımsızdır. POS hedefi
yalnız banka hesabıdır; kasa hesabı seçilemez. Ekranda brüt, komisyon ve net
ayrı okunur.

## Giriş ve session oluşturma

```text
POST /api/v1/auth/login (email + parola)
  -> Identity doğrulaması
  -> aktif ApplicationUser
  -> kısa ömürlü imzalı access token
  -> rastgele refresh token + SHA-256 hash
  -> hash taşıyan RefreshSession
  -> 200 access token + ham refresh token response'u
```

Bilinmeyen e-posta, yanlış parola ve pasif kullanıcı aynı genel credential
hatasına gider. Ham refresh token yalnız istemciye döner; repository sessionı
hash ile bulur.

## Token yenileme

```text
POST /api/v1/auth/refresh (ham refresh token)
  -> SHA-256 hash
  -> session lookup
  -> aktif mi, süresi geçerli mi, kullanıcı aktif mi?
  -> yeni access + refresh token
  -> eski session revoke + yeni session ekle (rotation)
```

Revoke edilmiş token tekrar kullanılırsa reuse kaydedilir ve kullanıcının bütün
aktif refresh sessionlarını kapatma işlemi istenir. Gerçek persistence'ta
rotation tek transaction olmalıdır.

## Çıkış

```text
POST /api/v1/auth/logout
  -> refresh token -> hash -> session lookup -> revoke -> 204
```

Token bilinmiyorsa veya önceden revoke edilmişse de çıkış idempotent başarıdır.
Mevcut access token kendi kısa ömrü bitene kadar geçerli olabilir.

## Auth rate limit

```text
Auth request
  -> remote IP partition
  -> 1 dakikada ilk 10 istek geçer
  -> 11. istek: 429 ProblemDetails / rate_limit.exceeded
```

Kuyruk yoktur. Proxy/load balancer güven ayarları bulut aşamasında eklenmeden
forwarded IP header'larına güvenilmez.

## Sahiplik kontrollü hesap okuma/değiştirme

```text
AccountId request'i
  -> doğrulanmış JWT sub claim'i
  -> ICurrentUser.UserId
  -> Get/Deactivate use case
  -> repository: AccountId AND UserId
  -> eşleşme varsa DTO veya Domain Deactivate + scoped update
  -> eşleşme yoksa NotFound
```

Request'te UserId alanı bulunmadığı için istemci başka kullanıcı ID'si seçemez.
Repository eşleşmeyi hem kaynak ID'si hem current-user ID'siyle yapar. Admin
bypass yolu yoktur.

## Hesap oluşturma ve listeleme API akışı

```text
POST /api/v1/accounts
  -> JWT authorization
  -> CreateAccountRequest string allow-list dönüşümü
  -> CreateAccountUseCase + current-user sub
  -> owner ataması + EF Core repository + SQL Server
  -> 201 AccountResponse + Location

GET /api/v1/accounts?pageNumber=1&pageSize=20&isActive=true&type=bank
  -> JWT authorization
  -> current-user sub
  -> owner scope -> filter -> sort -> pagination
  -> 200 items + pagination metadata
```

Request ve query içinde `UserId` kabul edilmez. Owner-scoped davranış hem hızlı
API testlerinde hem gerçek SQL integration testinde doğrulanmıştır. Liste ve
detay DTO'larındaki güncel bakiye açılış bakiyesi ile iptal edilmemiş hareketlerden
hesaplanır.

## Transaction liste ve detay akışı

```text
GET /api/v1/transactions + bearer token
  -> page/date/account/category/type/sort validation
  -> hatalı filtre: 400 ProblemDetails
  -> WHERE UserId = current user
  -> optional tarih/account/category/type filtreleri
  -> date DESC, id DESC; sonra Skip/Take
  -> 200 TransactionListResponse + pagination

GET /api/v1/transactions/{id}
  -> TransactionId + current UserId
  -> 200 detail veya varlık sızdırmayan 404
```

## Finansal hareket yazma ve iptal akışı

```text
Bearer JWT sub
  -> CreateTransactionRequest (amount string, TRY, yyyy-MM-dd)
  -> AccountId + current UserId ile aktif hesap
  -> CategoryId + current UserId ile aktif kategori
  -> Domain: sahiplik + gelir/gider tür uyumu + pozitif Money
  -> EF Core SaveChanges
  -> 201 TransactionResponse

DELETE /transactions/{id}
  -> TransactionId + current UserId
  -> Cancel(UTC now), fiziksel silme yok
  -> aynı istek tekrarlanırsa ilk iptal zamanı korunur
  -> bakiye/rapor sorgularında iptal edilen hareket dışlanır
```

## Aylık bütçe ve rapor akışı

```text
POST /api/v1/budgets
  -> current user'a ait aktif Expense category
  -> user + category + year + month uniqueness
  -> pozitif TRY limit
  -> 201

GET /api/v1/budgets?year&month
  -> [ayın ilk günü, sonraki ayın ilk günü)
  -> iptal edilmemiş kategori giderleri SUM
  -> spent / remaining / exceeded

GET /api/v1/reports/monthly veya /api/v1/dashboard
  -> owner-scoped aylık income/expense SUM
  -> expense GROUP BY category
  -> tüm zaman hareketlerinden account balance
  -> tek MonthlyReportResponse
```

## Finansal akış ve yan etki haritası

```text
Authenticated request
        │
        ├─ POST /transfers
        │     ├─ current user ile source + destination bul
        │     ├─ Domain: farklı/aktif/aynı owner/TRY
        │     └─ tek Transfer kaydı
        │           ├─ source balance -amount
        │           ├─ destination balance +amount
        │           └─ monthly report ±0
        │
        ├─ POST /credit-cards/{id}/charges
        │     ├─ owner card + expense category
        │     ├─ active ve limit kontrolü
        │     └─ CardCharge → debt +amount, report expense +amount
        │
        ├─ POST /credit-cards/{id}/payments
        │     ├─ owner account + card
        │     ├─ active account ve debt kontrolü
        │     └─ CardPayment → account -amount, debt -amount, report ±0
        │
        └─ POST /installment-plans/{id}/items/{sequence}/realize
              ├─ owner plan + planlı item
              └─ tek SQL transaction
                    ├─ item realized/chargeId
                    └─ CardCharge → debt/report +item amount
```

### Transfer oluşturma ve iptal

API para/tarih sözleşmesini çözdükten sonra use case kaynak ve hedef hesabı ayrı
owner-scoped sorgularla bulur. Domain iki hesabın farklı, aktif, aynı kullanıcıya
ait ve TRY olmasını zorunlu tutar. Tek `Transfer` yazılır. Liste/detay yalnız
current user verisini döndürür. İptal aynı satıra UTC iptal zamanı yazar;
tekrarlanan iptal aynı sonucu verir. Bakiye projection'ı iptal edilen satırı dışlar.

### Tasarruf hedefinin iki modu

`account-balance`: ilerleme seçilen hesabın bakiyesidir; kullanıcı hiçbir şey
girmez. `manual-contributions`: ilerleme kullanıcının eklediği katkıların
toplamıdır ve **para hareket etmez** — hiçbir hesaptan çıkış olmaz, tutulan şey
"şu kadarını ayırdım" notudur.

İki mod ekranda birbirinin aynısı görünüyordu ve manuel katkı gerçek bir para
hareketi sanılıyordu. Artık üç yerde de açıkça yazılı: hedef kartında mod
rozeti ve tek cümlelik açıklama (bakiye modunda izlenen hesabın adıyla),
katkı formunda tutar alanının altında, hedef oluştururken mod seçicinin
altında. Uyarı eylemin yapıldığı yerde tekrarlanıyor: kartta yazması yetmiyor,
kullanıcı tutarı yazarken görmeli.

### Borç faizi

Borcun faizi gerçek bir maliyettir: ödenen faiz gider, tahsil edilen faiz
gelirdir. Ayrım sözleşme anında sabitlenir (`PrincipalPortion` /
`InterestPortion`) ve taksit ödendiğinde o dönemin toplamlarına girer.

**Faiz kalıcı bir hareket üretmez.** Taksit ödemesi hesabı zaten tutarın
tamamı kadar düşürüyor; faiz için ikinci bir `BudgetTransaction` yazmak aynı
parayı bakiyeden iki kez düşerdi. Rapor faizi doğrudan taksit satırından
okur.

Faiz kategori dağılımında da görünür: ödenen faiz kullanıcının
`Faiz ve finansman gideri` kategorisine yazılır. Bu satır olmadan gider
**toplamı** faizi içeriyor ama **kategori listesi** içermiyordu ve aradaki
fark açıklamasız kalıyordu. Kategori kanonik adla bulunur; kullanıcı onu
yeniden adlandırdıysa faiz toplamda doğru kalır ama kovasız görünür — uydurma
bir kimlikle satır üretmek daha kötü olurdu, istemci o kimlikle filtreleyip
boş sonuç alırdı.

**Feed'de tek satır kalır.** Taksit tek para hareketidir; anapara ve faiz onun
bölünmesidir, ayrı kayıt değil. Ayrı yazılsaydı işlemler toplamı hesaptan çıkan
parayla tutmazdı (2.600 + 100 = 2.700). Çift taraflı muhasebede de tek yevmiye
kaydının ayrı ayaklarıdır:

```text
Borç  Borç anaparası   2.500
Borç  Faiz gideri        100
      Alacak  Banka             2.600
```

Bunun görünür karşılığı: satırın tutarı 2.600, alt satırında `faizi ₺100,00`
yazar ve ayrıntı panelinde `Anapara` / `Faiz (gider)` kırılımı durur. Sözleşme
iki payı da taşır (`principalPortion`, `interestPortion`); anapara istemcide
çıkarılmaz çünkü istemci para aritmetiği yapmaz.

Anapara/faiz ayrımı olmayan taksitler tamamı anapara sayılır. Yerel
veritabanındaki böyle kayıtlar, oranları paradan yeniden çözülerek
doldurulmuştur; bu bir şema migration'ı değil, sentetik veri üzerinde tek
seferlik onarımdır (anüite hesabı SQL'de ifade edilemez).

### Geçmiş listelerinin sınırı

Kart hareketleri ve transferler tarih filtresi ve üst sınır olmadan tüm geçmişi
çekiyordu. İkisi de her gün büyüyen kayıtlar; bu ilk aylarda görünmez, sonra
sessizce yavaşlar.

Sınır **iki katmanlı**, çünkü tek başına ikisi de yetmez:

```text
GET /api/v1/credit-cards/{id}/activity?from=&to=&all=
GET /api/v1/transfers?from=&to=&all=

tarih penceresi   -> kullanıcının ne gördüğünü belirler (varsayılan son 3 ay)
satır tavanı      -> pencere ne kadar genişse genişlesin sorguyu sabitler
hasMore           -> kırpma olduysa bildirilir
```

Varsayılanı **sunucu** koyar: istemci alan göndermeyi unutursa sessizce
sınırsız sorguya dönmemeli. "Tümü" ayrı ve açık bir niyettir (`all=true`) ve
yalnız tarih sınırını kaldırır — satır tavanı orada da geçerlidir, aksi hâlde
bu seçenek sorunun kendisini geri getirirdi. Kırpma sessiz değildir: `hasMore`
ile bildirilir ve ekran "hepsi bu" iddiasında bulunmaz.

**Borç listesi bilerek sınırlanmadı.** Borç bir hareket değil sözleşmedir:
günlük değil, kasıtlı olarak oluşur ve sayısı yavaş büyür. Daha önemlisi
yükümlülük listesini kırpmak, kullanıcının ödemesi gereken bir borcu
gizlemek olurdu — yavaş bir sorgu, eksik bir borç listesinden iyidir.

### Kart harcaması, ödeme ve ekstre

Harcama akışı aktif owner kartı ile aktif owner gider kategorisini doğrular ve
mevcut borç + yeni tutarın limiti aşmasını reddeder. Ödeme akışı aktif owner banka
hesabını ve owner kartı doğrular; kart pasif olsa da borç kapatma devam eder, fakat
borcu aşan ödeme reddedilir. Her iki kaydın iptali geçmişi silmeden yapılır.

Ekstre GET çağrısı `year/month/asOf` alır. Repository dört owner-scoped toplam
üretir: önceki bakiye, dönem harcaması, kesime kadar ödeme ve kesim sonrası ödeme.
Domain dönem sınırı, son ödeme tarihi, ekstre/kalan borç ve ödeme durumunu hesaplar.
Bu çağrı veri yazmaz.

`GET /api/v1/credit-cards/{id}/statements/current` kesim tarihi geçmiş **en son**
dönemi bulup aynı projection'ı döndürür; kartın ilk kesimi henüz gelmemişse
`statement: null` gelir ve bu bir hata değildir. Ayrı uç nokta olmasının nedeni
maliyet: bir ekstre beş toplam sorgusudur, kart listesi DTO'suna gömülseydi her
kart için tekrarlanırdı. Kart **detayı** tek kart gösterdiği için orada tek çağrı
düşer.

Asgari ödeme ekstre projection'ının parçasıdır: `minimumPayment` ekstre borcunun
karta ait `minimumPaymentRate` yüzdesiyle çarpımıdır (kuruşa yukarı yuvarlanır,
ekstre borcunu aşamaz), `remainingMinimumPayment` ise kesim sonrası ödemelerle
erir. Oran koda gömülü değil kart başına saklanır: asgari oranı bankası ve limiti
belirliyor, yönetmelik değişince koda gömülü bir sabit sessizce yanlış tutar
gösterirdi. Varsayılan %20, kart formundan değiştirilebilir.

Ekstrenin **ödeme eylemi yoktur**: ekstre kalıcı bir kayıt değil, kesim ve ödeme
tarihlerinden hesaplanan bir görünümdür, ödemenin bağlanacağı bir satır yoktur.
Uygulamadaki `Ekstreyi öde` ve `Asgariyi öde` kısayolları yalnız mevcut kart
ödemesi panelini o tutarla önceden doldurur; tutar düzenlenebilir kalır çünkü
kısmi ödeme meşrudur. Ödemenin dönemle ilişkisi yine yalnız tarih üzerinden
kurulur.

### Gecikmiş yükümlülük uyarısı

Planlanan projection'da `ScheduledDate` için **alt sınır yoktur**: vadesi
geçmiş ama hâlâ gerçekleşmemiş kayıt hangi ufuk istenirse istensin döner ve
`timing = overdue` taşır. Bu bilinçli — gerçekleşmemiş bir taksit kart borcu,
ekstre ya da gider üretmez (onaysız para hareketi olmaz), ama gerçekten
yapılmış bir harcamanın `Gerçekleştir` unutulduğu için sessizce kayıt dışı
kalmaması gerekir.

Özet ekranı bu satırları en üstte bir uyarı bandında sayar. Bant **tutar
toplamı göstermez**: istemci finansal toplamı ikinci kez hesaplamaz ve API de
aynı gerekçeyle planlanan listesinde toplam vermez. Taşıdığı bilgi sayı ve en
eski vadedir; ikisi de sayma işidir.

Hangi satırın kullanıcının **borcu** olduğunu sunucu `isPaymentObligation` ile
bildirir (`PlannedActivityRules.IsPaymentObligation`). Tekrarlanan gelir ve
tahsil edilecek alacak planlanan hareketlerdir ama yükümlülük değildir; ayrımı
istemcinin `effect` ve `actionKind`'dan yeniden türetmesi aynı kuralın ikinci
bir kopyasını doğururdu.

Açık tek seferlik yükümlülük de aynı hatta yer alır. Vade geçtiyse ödenecek yön
Özet bandını besler; tahsil edilecek yön planlanan listede kalır fakat borç
sayılmaz. Settlement oluştuğunda kalıcı bir durum bayrağı güncellenmez, güncel
projection satırı artık üretmez.

### Planlanan satırın eylemi

Her planlanan satır `actionKind` taşır (`realize`, `pay-card`, `pay-debt`,
`collect-debt`, `pay-obligation`, `collect-obligation`) — hangi işin yapılacağını
sunucu söyler, istemci tahmin etmez.
Yanında `actionTargetId` ve `actionSequence` gider: `plannedActivityId`
planlanan **satırı** tanımlar, yazma uç noktaları ise sahibi olan aggregate'i
adresler ve ikisi çoğu türde farklıdır.

| Tür | `actionTargetId` | `actionSequence` |
|---|---|---|
| Tekrarlanan | occurrence kimliği; **üretilmemişse plan kimliği** | — |
| Kart taksidi | taksit planı kimliği | taksit sırası |
| Kart ekstresi | kart kimliği | — |
| Borç/alacak taksidi | borç kimliği | taksit sırası |
| Tek seferlik yükümlülük | yükümlülük kimliği | — |

Uygulamada eylem ikiye ayrılır ve ayrımı kaydın kendisi belirler.
`realize` gövde istemez — onay dışında sorulacak bir şey yoktur, tutar ve tarih
zaten kayıtta — bu yüzden listede tek dokunuşla, açık onayla tamamlanır.
Ödeme ve tahsilat hangi hesaptan yapılacağını sorar; formu bu listeye
kopyalamak yerine kullanıcı o kaydın kendi ekranına gider. Engelli
(`readiness = needs-attention`) satırda buton kapalıdır; nedeni
`attentionCode`'dan üretilen cümleyle satırda zaten yazılıdır.

Tek seferlik yükümlülük `Diğer > Yükümlülükler` altında yaklaşan, geciken ve
kapanan sekmelerinde okunur. Açık satıra dokununca hesap seçilen tek panel açılır;
`Öde/Tahsil et ve kapat` çağrısı settlement'ı yazar, ekranı yeniler ve kayıt
kapananlara geçer. Panel, işlemin gelir/gideri yeniden yazmadığını açıkça söyler.
Yükleme, boş, hata, unauthorized ve son bilinen veriyi gösteren stale durumları
ayrı görünür.

**Yükümlülük kamerayla sınırlı değildir.** Aynı kayıt `İşlem ekle >
Ödenmemiş fatura` satırından ve `Yükümlülükler` ekranındaki ekleme
eyleminden de açılır; form önerisiz de çalışır. Fotoğrafı olmayan kullanıcının
elindeki kâğıt faturayı yazamaması, aşamanın kapatmak için açıldığı boşluğun
kendisiydi. Menü satırı parayı bugün hareket ettirmediğini yazar; yazmasaydı
kullanıcı satırı gider sanıp bakiyesinin azalmasını beklerdi.

**Yönü elle girişte kullanıcı seçer** (`Ödenecek` / `Tahsil edilecek`), fiş
dalında **sorulmaz**: "henüz ödemedim" cevabı borçlu olunduğunu bir adım önce
söyledi ve aynı soruyu tekrar sormak olurdu. Yön değişince kategori listesi
yeniden okunur — alacak gelir, borç gider kategorisi ister ve sunucu yönle
çelişen kategoriyi zaten reddeder.

**Para hareket etmeden önce açık onay istenir ve kaynak adıyla yazılır.**
Gerçekleştirme kaynağı **sormaz** — tekrarlayan planın kaynağı kuruluşta
seçilir ve kayıt onu zaten taşır — ama onay penceresi paranın hangi hesaptan
çıkacağını söyler. Satırda kaynak yazmıyordu ve tek dokunuş para hareket
ettiriyordu; kullanıcı onayladığı şeyi görmeden onaylıyordu.

Ayrı bir "üret" adımı **yok**. `Bugüne kadar üret` düğmesi kaldırıldı: tek işi
gerçekleştirilebilir bir satır üretebilmekti ve başka hiçbir şey önceden
üretilmiş occurrence'a bağlı değil. `POST /occurrences/generate` sözleşmede
duruyor; çağıranı artık `RealizeDueRecurringUseCase`.

Eylemin adı iki ekranda da **`Gerçekleştir`**. `Onayla` ardından başka bir adım
gelecekmiş izlenimi veriyordu; oysa adım tek.

**Vakti gelmemiş satırda eylem hiç çizilmez** — gri de çizilmez. Kapalı bir
buton "bir şey eksik" der ve kullanıcıyı eksiği aramaya gönderir; oysa eksik bir
şey yoktur, yalnız gün gelmemiştir. Kural sunucuda da vardır
(`recurring.not_due_yet`): gelecek ayın kirasını bugün gerçekleştirmek parayı
çıkmadığı bir aya yazar ve tarihe göre okuyan her rapor o andan sonra yanlış
olur.

**Henüz üretilmemiş tekrarlanan satır plan kimliğini ve tarihini gönderir**
(`POST /api/v1/recurring-transactions/{planId}/occurrences/realize`); sunucu
eksik occurrence'ı kendi üretip gerçekleştirir. Önceki hâlde `actionTargetId`
`null` geliyor, istemci butonu kapatıyor ve üretme düğmesi başka bir ekranda
duruyordu: ufuktaki her ileri tarihli satır kalıcı olarak kapalıydı. Üretmek bir
kullanıcı kararı değil, sistemin idempotentlik için tuttuğu defter işidir; karar
gerçekleştirmektir.

### Taksit planı ve gerçekleştirme

Plan create çağrısı `ClientRequestId` taşır. Aynı kullanıcı ve request ID daha
önce varsa mevcut plan döner. Yeni planda toplam 2–60 item'a dört ondalıkla
bölünür ve fark son item'a taşınır. Planlama rapor yan etkisi üretmez.

Gerçekleştirme owner plan içindeki sequence'i bulur. Item zaten gerçekleşmişse
bağlı charge döner; değilse item state değişimi ile `CreditCardCharge` aynı
`SaveChangesAsync` içinde yazılır. Böylece item gerçekleşmiş görünüp charge'ın
eksik kalması veya tersinin oluşması önlenir.

### Flutter finans akışı

`/more/cards` authenticated shell içindedir. `FinanceController` ilk açılışta
hesap, gider kategorisi, kart/activity ve plan verilerini repository üzerinden
yükler. 401 ayrı unauthorized state'e; diğer ağ/API hataları retry içeren error
state'e gider. Form submit sırasında controller ikinci dokunuşu reddeder, başarıdan
sonra snapshot'ı API'den yeniden yükler. Flutter validation yalnız erken geri
bildirimdir; finans kararları backend'de tekrar uygulanır.

## Tekrarlayan plan, upcoming ve rapor akışı

```text
POST /recurring-transactions
  -> current user'a ait aktif account + türe uygun aktif category
  -> tarih/sıklık/ay-sonu + opsiyonel bitiş tarihi/occurrence sayısı invariant'ları
  -> RecurringTransaction (henüz finans hareketi değil)

POST /recurring-transactions/occurrences/generate
  -> current user due planları
  -> occurrence key üret
  -> owner + key unique constraint
  -> aynı throughDate retry: duplicate yok
  -> bitiş tarihi veya occurrence sayısından önce dolan sınırda plan pasif
  -> gerçekleşmiş/planned geçmiş occurrence satırları korunur

POST /recurring-transactions/occurrences/{id}/realize
  -> occurrenceId + current UserId
  -> planned mı?
  -> aynı SQL transaction: occurrence realized + BudgetTransaction
```

```text
GET /upcoming-payments?asOfDate&daysAhead
  -> recurring planned + closed card statement + unrealized installment
  -> overdue / today / upcoming
  -> dueDate sonra source type ile deterministik sıra

GET /reports/advanced?year&month&asOfDate&trendMonths&daysAhead
  -> owner-scoped projection ve aggregate'ler
  -> net varlık + dönem karşılaştırması + trend
  -> bütçe sapması + gelecek yük + hesap/kart dağılımı
```

Plan formu **iki sınırı da** taşır: isteğe bağlı bitiş tarihi ve isteğe bağlı
toplam tekrar sınırı. İkisi birden verilebilir; sunucu önce dolanı uygular ve
form bunu alanın yardımcı metninde yazar. Form yalnız sayacı gönderip tarihi
sabit boş bıraktığı sürece "31 Aralık'ta bitsin" diyen kullanıcı planı elle
kapatmak zorunda kalıyordu.

Flutter “Daha fazla > Planlama ve raporlar” rotasında bu endpointleri tek
repository snapshot'ında birleştirir. İlk çağrıda loading; boş koleksiyonlarda
feature-specific empty; 401'de unauthorized; ilk yükleme hatasında retry görünür.
Önceden başarılı snapshot varken yenileme hatası oluşursa veri silinmez, stale
banner'ı gösterilir. Grafik değerleri ayrıca görünür metin listesinde sunulur.

## Flutter oturum ve korumalı istek akışı

```text
Uygulama açılır -> restoring ekranı
  -> secure storage'dan tek session kaydı
  -> geçerli access token: authenticated shell
  -> süresi yaklaşmış token: single-flight refresh + atomik token rotation
  -> refresh başarısız/geçersiz: local session temizlenir -> login

Korumalı API isteği -> bearer access token
  -> 2xx: DTO -> Repository -> ViewModel -> View
  -> ilk 401: tek refresh -> isteği bir kez tekrar et
  -> ikinci 401: tekrar döngüsü yok -> unauthorized/login
```

Register cevabı token taşımaz; başarılı kayıttan sonra e-posta login formuna
aktarılır. Logout sunucu çağrısı başarısız olsa bile local token `finally`
sınırında temizlenir. Parola hiçbir zaman secure storage'a yazılmaz.

## Flutter ekran akışı

```text
Dashboard -> kapsam anahtarı + backend aylık toplamları ve hesap bakiyeleri
İşlemler -> filtre/sayfalama -> ekle veya onayla iptal et
Bütçeler -> ay seç -> gider kategorisi bütçesi ekle/limiti güncelle
Diğer -> Hesaplar / Kategoriler / İşletmem var / Çıkış
Ana işlem düğmesi -> /transactions/new -> aktif hesap + uygun aktif kategori
```

## Kapsam anahtarının akışı

```text
Kayıt -> hasBusiness -> UserProfile (sunucu)
Oturum açıldı -> ScopeController.ensureLoaded()
  -> cihazdaki kopya (anahtar konumu + son bilinen cevap) okunur, ekran çizilir
  -> GET /api/v1/profile -> cevap tazelenir ve cihaza yazılır
  -> profil düşerse: cihazdaki kopya geçerli kalır, boyut kaybolmaz

Anahtar dokunuşu -> ScopeController.select
  -> cihaza yazılır
  -> dinleyen ekranlar (özet, feed, planlanan) yeniden okur
  -> okuma isteklerine `scope` query parametresi eklenir

Cevap "hayır" -> anahtar hiç çizilmez, hiçbir istekte `scope` gitmez
Çıkış -> ScopeController.forget -> seçim ve cevap cihazdan silinir
```

Anahtar **Özet ekranının başlığının altındadır** ve uygulamanın tek kapsam
denetimidir; feed ve planlanan görünüm onu uygular ve başlıklarında yazar ama
değiştirmez. Bölünmeyen bölümler (net varlık, hesap bakiyeleri) filtre açıkken
toplam gösterdiklerini ekranda yazar.

`Diğer -> İşletmem var` anahtarı `PUT /api/v1/profile` çağırır ve dönen cevabı
uygular; kategorilere dokunmaz.

## Özet ekranının ayı okuma biçimi

```text
GET /api/v1/dashboard?year&month[&scope]
  -> filtresizse: toplamlar + scopeBreakdown (business, personal)
  -> filtreliyse: yalnız o tarafın toplamları, scopeBreakdown yok

Kapsam boyutu görünmüyor      -> tek sayı: `Bu ayın neti`
Anahtar `Hepsi`               -> `İşletme neti` (hero)
                                 `Şahsi çekim` + `Bu ayın neti`
Anahtar `İşletme` / `Şahsi`   -> o tarafın neti, adı yazılı
```

Üç sayının üçü de sunucudan gelir; istemci aralarında çıkarma yapmaz. Şahsi
tarafın adı sayının yönüne göre `Şahsi çekim` ya da `Şahsi net` olur. Hesaplanan
şey nakit esaslı **işletme netidir**; "kâr" kelimesi kullanılmaz.

## POS tahsilatının birleşik akıştaki üç satırı

```text
10 Ağustos  POS satışı            +1.000,00   gelir    (kapsam taşır)
10 Ağustos  POS komisyonu            17,50    gider    (kapsam taşır)
13 Ağustos  POS parası hesaba geçti +982,50   nötr     (kapsam taşımaz)
```

Üç satır tek kayıttır ve aynı kimliği taşır; istemci onları `tür + kimlik`
ikilisiyle ayırır. Tek satıra indirilseydi ya komisyon görünmez olurdu ya da
hesabın bakiyesindeki artışın günü yanlış yazılırdı.

Geçiş satırı kapsam taşımadığı için kapsam filtreli okumada düşer — transfer ve
kart ödemesiyle aynı kural. Üç satırın hiçbiri feed üzerinden iptal edilemez;
iptal `Kasa > POS tahsilatları` ekranından tek eylemle yapılır ve üçünü birlikte
kapatır.

**Gün sonu kasa sayımı feed'de görünmez**: para hareketi üretmeyen bir gözlemdir.
Farkı onaylandığında üretilen düzeltme kaydı normal bir işlem satırı olarak
zaten görünür.

## `İşlem ekle` menüsünün niyet ekseni

Menü tek giriş noktasıdır: kabuktaki çentikli buton ve İşlemler ekranındaki
eylem aynı launcher'ı açar. Dokuz satır düz bir listede telefonda okunmuyordu;
satırlar **niyet** başlıklarının altında toplandı.

```text
Para girdi   -> Gelir
                POS tahsilatı            (Kasa > POS tahsilatları sekmesi)
Para çıktı   -> Gider
                Ödenmemiş fatura         (para henüz hareket etmez)
Para taşı    -> Hesaplar arası transfer
                Kredi kartı borcu öde
Belge okut   -> Fiş veya fatura okut
                Dekont okut
Plan kur     -> Tekrarlayan işlem planla
```

Eksen kayıt türü değil niyettir: kullanıcı menüyü açarken "bu bir kart ödemesi
kaydı mı" diye düşünmez, "param nereye gitti" diye düşünür.

`Para taşı` ayrı bir başlıktır çünkü transfer ile kart borcu ödemesi **gider
değildir** (ADR 0014): ödemeyi taşır, gelir/gider yazmazlar. İkisini
`Para çıktı` altına koymak menünün kendisine raporu yanlış anlattırırdı.

**Gün sonu kasa sayımı bu menüde yoktur.** Hiçbir para hareketi üretmeyen bir
gözlemdir ve `Kasa > Gün sonu` ekranında durur; menüye alınsaydı `İşlem ekle`
işlem olmayan bir şeyi işlem gibi gösterirdi. Hesap/kart açma, CSV içe aktarma
ve borç/taksit planları da aynı gerekçeyle kendi ekranlarındadır: bunlar bir şey
**kurar**, bugün para hareket ettirmez.

Açıklama satırı yalnız yanlış anlaşılabilecek satırlarda durur. `Gelir`,
`Gider` ve `Hesaplar arası transfer` başlığın altında zaten anlaşılıyor;
`POS tahsilatı` ise paranın bugün hesaba geçmediğini yazmak zorundadır.

`POS tahsilatı` menüden seçildiğinde `Kasa` ekranı POS sekmesi seçili açılır
(`/more/cash?tab=pos`). Menü ikinci bir kopya form açmaz: tahsilat formu
listeyle aynı yerde durur ve kullanıcı yoldaki parayı girdiği anda görür.

## Kapsamlı işlem ekleme

```text
Gider/gelir formu açılır
  -> seçenekler yüklenir (hesap/kart/kategori + defaultScope alanları)
  -> kaynak seçilir -> çip kaynağın etiketiyle dolar
  -> kaynak kapsamsızsa kategori seçimi çipi doldurur
  -> kullanıcı çipe dokunursa onun seçimi ikisini de yener
  -> Kaydet -> istek gösterilen kapsamı taşır
  -> zincir çözülemediyse: istek gönderilmez, alanın yanında "kapsam seçin"
```

Çip alanın altında **nereden geldiğini** yazar (kaynağın adı, kategorinin
varsayılanı ya da kullanıcının kendi seçimi). Kapsam boyutu görünmeyen
kullanıcıda alan hiç çizilmez; istek kapsam göndermez ve sunucu kategoriden
türetir.

İptal, fiziksel silme değildir; kayıt tarihçede etiketli kalır. Hesap/kategori
pasifleştirme de silme değildir. Offline cache bulunmadığı için ağ hatasında
stale veri gösterilmez; açık hata ve tekrar deneme sunulur.

## Temiz kurulumdan finans ekranına

```text
Aktör: sentetik yerel kullanıcı
Ön koşul: SQL ready + API ready + temiz debug APK

APK -> secure storage boş -> login/register
  -> [TB cihaz/API] geçerli token: ALLOW -> authenticated shell
  -> eksik/geçersiz token: DENY 401 -> login
  -> [TB API/SQL] owner-scoped kayıt ve sorgu
  -> dashboard/işlem/bütçe sonucu

Başarı: session restore edilir ve yalnız current-user finans verisi görünür.
```

## Yetkilendirme ve hata akışı

Korumalı endpoint önce bearer tokenı doğrular. Token yok/geçersizse `DENY 401`;
geçerliyse `ICurrentUser` kullanılır. Kaynak current user'a aitse `ALLOW`, değilse
varlığı sızdırmamak için `DENY 404` veya kaynak seçiminde güvenli `400` döner.
Flutter route guard yalnız navigasyon kolaylığıdır, güvenlik kararı değildir.

```text
API kapalı -> connection/timeout -> güvenli Türkçe hata -> Retry
SQL kapalı -> live=200, ready=503 -> güvenli hata -> SQL start -> ready=200
401 -> tek refresh + tek retry -> başarı
refresh reddi -> local session temizliği -> login
validation -> istek gönderilmez veya 400 ProblemDetails
```

APK uninstall/install cihaz session'ını temizler, SQL verisini temizlemez; kabul
koşuları benzersiz sentetik kullanıcılarla izole edilir.

## Import, attachment ve restore akışları

```text
CSV seç -> 2 MiB/type/encoding/delimiter/header doğrula
  -> Türkçe banka | İngilizce banka | Özel eşleme profili seç
  -> exact header + encoding + tarih biçimi + ondalık ayırıcıyı gönder
  -> uygulama transaction export'uysa güvenli biçimde durdur
     (type kolonu ile signed amount sözleşmesi aynı değildir; backup'a yönlendir)
  -> staging rows (transaction yok)
  -> owner account/category eşle
  -> duplicate: skip | import-anyway
  -> ready row kimlikleriyle confirm
  -> tek SQL transaction; retry yeni transaction üretmez

Fiş seç -> 5 MiB + allowlist -> magic byte + threat signature
  -> random object key'e temp write/move
  -> owner+transaction metadata save
  -> save hata: object compensating delete
  -> foreign list/download: 404

Backup seç -> validate (base64/length/SHA-256/strict JSON/graph/re-scan)
  -> Flutter schema+entity count gösterir
  -> kullanıcı ikinci onayı verir
  -> empty-owner kontrolü + GUID remap + Serializable restore
  -> başarı: SQL graph + object content
  -> hata: SQL rollback + bu çağrının object key'lerini sil

CSV (işlem) / CSV (cari defter) / JSON / backup dışa aktar
  -> authenticated binary download
  -> dosya adı + MIME türü + boyut + oluşturulma zamanı
  -> CSV/JSON: sınırlı uygulama içi önizleme
  -> backup: schema + toplam kayıt; payload ekranda gösterilmez
  -> Paylaş: Android sharesheet
  -> Cihaza kaydet: ACTION_CREATE_DOCUMENT -> kullanıcı hedefi seçer
  -> seçilen URI'ye byte yaz; geniş depolama izni isteme
```

Borç taksiti `payable` ise yaklaşan ödemede görünür; ödeme hesabı bakiyesini
azaltır. `receivable` tahsilatı bakiyeyi artırır. İkisi de aylık income/expense
toplamına girmez. Tasarruf hedefi ya bağlı hesabın bakiyesini ya manuel katkıları
izler; iki kaynak hiçbir zaman toplanmaz.

### Fişten gelen karşı taraf önerisi

```text
Fiş okundu -> sunucu adı kullanıcının karşı taraflarıyla eşleştirir (tam ad)
  -> eşleşme yoksa: ad taslakta kalır, uyarı yok
  -> eşleşme varsa: borç/alacak formunda rozet
       "… defterinizde kayıtlı. Bu kayıt aynı karşı tarafa bağlanacak."
       -> kabul: ad olduğu gibi gönderilir, sunucu aynı kişiye bağlar
       -> "Bu kişi değil": ad silinir, imleç alana gider, öneri geri gelmez
       -> ad değiştirilir: rozet düşer
```

Model karşı tarafı **seçmez, önerir** (ADR 0011). Rozetin işi, dolu gelen bir
adı kullanıcının kendi yazdığı sanıp geçmesini engellemek: kayıt var olan bir
kişinin açık bakiyesine eklenir ve yanlış eşleşme iki müşterinin hesabını
birbirine karıştırır.

### Borç / alacak planı oluşturma

*(Karar gerekçeleri ADR 0009'da.)*

```text
Borç / alacak ekle
  -> kişi/kurum adı  -> sunucu karşı tarafı bulur, yoksa kurar
  -> yön (ödenecek | alınacak)
  -> borcu ne doğurdu:  nakit  -> paranın girdiği/çıktığı hesap
                        gider  -> kategori   (yalnız "ödenecek" yönünde)
  -> anapara
  -> elinizde hangisi var:  toplam geri ödeme  |  yıllık faiz %
  -> taksit sayısı, başlangıç tarihi, ilk taksit vadesi, not
  -> sunucu: eksik olanı anüiteyle hesaplar
             taksit = toplam geri ödeme / taksit sayısı (kuruş artığı sonuncuya)
             her taksite anapara/faiz payı yazılır
             vade = ilk vade + n ay
```

#### Para nereye gidiyor

| An | Bakiye | Gelir / gider |
|---|---|---|
| Açılış, kaynak `cash`, borç | hesap **+anapara** | yok |
| Açılış, kaynak `cash`, alacak | hesap **−anapara** | yok |
| Açılış, kaynak `expense` | değişmez | **gider +anapara**, kategorili |
| Taksit ödemesi (borç) | hesap **−taksit** | **gider +faiz payı** |
| Taksit tahsilatı (alacak) | hesap **+taksit** | **gelir +faiz payı** |

Anapara geri ödemesi gider değildir: borç azalır, para azalır, servet değişmez.
Kredi kartı ödemesinin gider üretmemesiyle aynı kural. Gerçek maliyet faizdir.

### Veresiye satış, tahsilat ve iptal

```text
Karşı taraf ekle (ad, isteğe bağlı not)
  -> Borçlandır:  yön (alacak | borç)
                  tutar, tarih, **isteğe bağlı vade**, kategori, açıklama
                  kapsam: açık seçim -> kategorinin varsayılanı
     sonuç: gelir/gider **bugün** yazılır, kasa kıpırdamaz
  -> Tahsilat / ödeme:  hesap, tutar, tarih, açıklama
     sonuç: kasa değişir, gelir/gider **üretilmez**
```

Yön kategorinin türünü belirler: alacak doğuran borçlandırma gelir
kategorisi, borç doğuran gider kategorisi ister; tutmayan istek reddedilir.
Tahsilat ne kategori ne kapsam sorar — gelir/gider raporuna hiç girmez.

Vade boş bırakılabilir; geçmiş cari satırlara sonradan tarih uydurulmaz. Vade
seçilirse işlem tarihinden önce olamaz. Cari liste ve ayrıntı `asOfDate`
gönderir; toplam bakiye yanında vadesi geçmiş tutar, ayrıntıda ise
vadesi geçmiş ve vadesi geçmemiş/vadesiz kalan ayrı okunur. Ödemeler belirli
bir satıra tahsis edilmediği için önce gecikmiş borçlandırmayı kapatır.

Pasif karşı tarafa **yeni borçlandırma yazılamaz** (`409`), **tahsilat
yazılabilir**: aksi hâlde artık iş yapılmayan bir müşterinin kalan borcu
kapatılamaz hâle gelirdi. Hiç hareketi olmayan karşı taraf silinebilir;
hareketi varsa `409` gelir ve kullanıcı pasifleştirmeye yönlendirilir (boş
hesap kuralının aynısı).

İptal iki yönlü çalışır: borçlandırmanın iptali tanınan gelir/gideri ve açık
bakiyeyi birlikte geri alır, tahsilatın iptali parayı kasaya geri koyar ve
açık bakiyeyi yeniden doğurur. İkisi de birleşik feed'de `canCancel: true`
olarak raporlanır.

Fazla tahsilat **kırpılmaz**: taraf eksiye düşer ve bu gerçektir. Sıfır
bakiyeli karşı taraf listeden düşmez, yalnız ayrı okunur
(`?balance=open|settled|all`).

#### Fiş okuma karşı tarafı önerir

Fişten okunan ad kullanıcının kendi karşı taraflarında **tam ad** ile
aranır; bulunan kayıt taslakta `counterpartyId` olarak döner. Bu bir öneri
katmanıdır (ADR 0011): hiçbir şey yazılmaz, karşı taraf kurulmaz ve
kullanıcı öneriyi reddedebilir. Ad her hâlükârda yerinde kalır — reddeden
kullanıcı adsız bir taslakla baş başa kalmamalı. Eşleşme yoksa taslak yalnız
adı taşır; kayıt onaylanırken karşı taraf bulunur ya da kurulur (borç
akışındaki bul-ya-da-oluştur ile aynı yol).

#### Karşı taraf tek kayıttır

Yazılan ad mevcut bir karşı tarafla eşleşirse sözleşme **ona** bağlanır;
eşleşmezse karşı taraf o anda kurulur. Kullanıcı önce "karşı taraf ekle"
adımına gitmez. Cevap hem `counterpartyId` hem `counterpartyName` taşır: ad
artık sözleşmede değil, karşı tarafta durur ve oradan okunur. Aynı ad başka
kullanıcıda ayrı bir karşı taraftır.

Bu ayrımdan önce açılış hiçbir kayıt üretmiyordu; taksitler hesabı
boşaltıyor ama karşılığında hiçbir şey girmemiş görünüyordu.

#### Faiz üçlüsü

Kullanıcı **toplam geri ödemeyi ya da yıllık faizi** girer, ikisini birden
değil. Diğerini sunucu hesaplar (anüite, aylık oran = yıllık / 12). İkisi
birden gelip çelişirse istek `debt.repayment_conflict` ile reddedilir.

Anapara ile toplam kanoniktir, oran türetilmiştir. Tersi olsaydı yuvarlama
kullanıcının yazdığı toplamı değiştirirdi (400,00 → 399,99). İstemci hesabı
tekrarlamaz; sunucunun döndürdüğü değeri gösterir.

#### Açılışı kayıtsız borçlar

```text
Borç listesi
  -> "Bu borcun açılışı kayıtlı değil" notu + Tamamla
  -> nakit mi gider mi + hesap/kategori
  -> POST /api/v1/debts/{id}/opening   (tek kez; ikincisi 409)
```

Bu ayrımdan önce açılmış kayıtların açılışında ne olduğu hiçbir sayıdan
türetilemiyor — yalnız kullanıcı biliyor. Tamamlanana kadar borç ne bakiyeye
ne rapora girer, ve uygulama bunu gizlemez.

Başlangıç ve ilk vade tarihleri seçilebilir. Önceki sürümde ikisi de bugüne
sabitlenmişti ve geçmişte başlamış bir kredi girilemiyordu. İlk vade
başlangıçtan öne çekilemez: kullanıcı başlangıcı ileri aldığında vade tarihi de
birlikte taşınır, böylece sunucunun reddedeceği bir kombinasyon hiç
oluşturulamaz.

## Diğer menüsünün yerleşimi

Menü yedi kutu ve sıra frekansa değil **ne yaptığınıza** göre:

| | Kutu | İçerik |
|---|---|---|
| 1 | Hesaplar ve transferler | `/more/accounts` — iki sekme |
| 2 | Kredi kartları | `/more/cards` — harcama, taksitli harcama, ödeme, ekstre |
| 3 | Kategoriler | `/more/categories` |
| 4 | Cari hesap | `/more/counterparties` |
| 5 | Borç ve alacaklar | `/more/debts` |
| 5 | Tasarruf hedefleri | `/more/goals` |
| 6 | Planlama ve raporlar | `/more/planning` — tekrarlayanlar, yaklaşanlar, raporlar |
| 7 | Veri ve yedek | `/more/data-tools` — CSV, belgeler, yedek |
| 8 | Hesabım | `/more/account` — e-posta, işletme cevabı, oturumlar, parola, çıkış, hesabı kapatma |

İlk ikisi yalnız **bakmak** için açtığınız yerler; 3–5 **kurduğunuz** şeyler;
6 bakış, 7 dosya işi.

Önceki hâlde beş kutu vardı ve biri "İçe aktarma, borç, hedef ve yedek"
diyordu — dört alakasız şeyi sayan bir başlık, gruplamanın yanlış olduğunun
kendi itirafıydı.

Neyin nereye gittiği ve neden:

- **Transfer hesaplara gitti.** İki hesap arasında para taşır; tanımı hesabı
  içerir, hesap olmadan anlamsızdır. Kredi kartıyla tek ortak noktası "hesap
  işlemi değil" olmasıydı — olumsuz bir tanım.
- **Kredi kartı kendi kutusunu aldı.** Hesap para tutar, kart borç tutar; kartın
  kendi yaşam döngüsü var (harcama → ekstre → son ödeme → ödeme) ve hesabın yok.
- **Taksitli harcama karta gitti** (ayrı bir sekmeydi): taksitli alışveriş bir
  kart harcamasıdır.
- **Borç ve hedef Veri Araçları'ndan çıktı.** İkisi de finansal kavram; CSV ve
  yedekle aynı çekmecede durmalarının sebebi yalnız tarihseldi. Ayrıldıklarında
  her ekran yalnız kendi verisini çeker oldu — borç listesi açılırken belge
  listesi yüklenmiyor.

Hızlı ekleme menüsündeki `Transfer`, `/more/accounts?tab=transfers` ile
doğrudan Transferler sekmesini açar; kullanıcıyı Hesaplar'a bırakıp sekmeyi
kendisinin bulmasını beklemek istediği işi bir adım uzatırdı.

## Hesabım akışı (Aşama 06 Grup 1)

Hesaba dair parçalar tek sayfada toplandı. Önceden `Diğer` menüsünün altında
iki ayrı kart olarak duran **işletme cevabı** ve **çıkış** oraya taşındı;
menüde yalnız sayfanın kapısı kaldı.

Sayfaya iki yerden girilir: **Özet ekranının sağ üstündeki hesap ikonu** ve
`Diğer > Hesabım`. İkisinin birden kalıp kalmayacağına Aşama 06.2 karar verir;
o karara kadar iki kapı bir kusur değil, bilerek bırakılmış geçici hâldir.
İkon `AppBar` action'ıdır ve kapsam anahtarına dokunmaz: anahtar `AppBar`'da
değil, altındaki ayrı barda ve kaydırılan gövdenin dışında durur.

Sayfanın sırası tehlikeye göredir:

| Sıra | Bölüm | Ne yapar |
|---|---|---|
| 1 | Kimlik | E-posta ve hesap açılış tarihi |
| 2 | İşletmem var | Onboarding cevabı; kategorilere dokunmaz |
| 3 | Açık oturumlar | Her satır açılış ve geçerlilik tarihi; bu cihaz işaretli |
| 4 | Güvenlik | Parola değiştirme, çıkış |
| 5 | Hesabı kapat | Geri dönüşü olmayan silme |

**Oturum satırı ne olduğunu değil ne zaman açıldığını söyler.** Cihaz adı, IP
ve konum yoktur çünkü sunucu bunları hiç saklamıyor; uydurulmuş bir
"Windows, İstanbul" satırı denetlenemeyen bir güven vaadi olurdu. Bu cihazın
satırında kapatma düğmesi yoktur — kendi oturumunu kapatmak "çıkış yap"tır ve
onun kendi satırı vardır.

**Parola değişimi kullanıcıyı uygulamadan atmaz.** Sunucu bütün oturumları
kapatır ve cevapla birlikte bu cihaza taze bir token çifti verir; istemci onu
hemen benimser. Kullanıcı ekranda "diğer cihazlardaki oturumlar kapatıldı"
cümlesini görür ve yerinde kalır.

**Hesabı kapatmak iki kapıdan geçer** (ADR 0017): önce ne olacağını söyleyen
açık onay — yedek alma önerisiyle birlikte — sonra parolanın yeniden yazıldığı
panel. İkisinden biri tamamlanmazsa hiçbir istek gitmez. Silme başarılı olunca
istemci çıkış yapar; sunucuda hesap kalmadığı için elindeki token da ölüdür.

## Doğrulama ve parola sıfırlama akışı (Aşama 06 Grup 2)

Kayıt olan kullanıcıya altı haneli bir kod gider ve **hesap kilitlenmez**:
kullanıcı hemen giriş yapar, uygulamanın tamamını kullanır. Doğrulanmamış
adresin karşılığı iki yerde görünen tek bir uyarıdır:

| Yer | Ne gösterir |
|---|---|
| Özet ekranının hesap ikonu | Küçük bir nokta (`Badge`) — uyarının doğal yeri, gidip düzelteceği sayfanın kapısıdır |
| `Hesabım` sayfası | Ne olduğunu ve neden işe yaradığını anlatan kart + `Adresimi doğrula` |

Panel açılır açılmaz kod ister; kullanıcı zaten bunun için gelmiştir. Kod
gönderilemiyorsa (sunucuda gönderici yapılandırılmamışsa) bu **söylenir** —
yoksa kullanıcı hiç gelmeyecek bir postayı bekler. `Kodu yeniden gönder` aynı
yolu tekrar çağırır; sunucu 60 saniyeden önceki ikinci isteği reddeder.

**Parolamı unuttum** giriş ekranının hemen altındadır: aranacağı yer,
kaybedildiği yerdir. Tek sayfa, iki adım — adres yazılır ve kod istenir, sonra
kod ile yeni parola yazılır. İki ayrı ekran, gelen kodu okuyup dönen kullanıcıya
adresini yeniden yazdırırdı.

Birinci adımın cevabı **her zaman aynıdır**: *"Adres kayıtlıysa kod gönderildi."*
Kayıtlı olmayan adres için de bu cümle çıkar; sunucu hangi adresin hesabı
olduğunu söylemiyor, ekran da söylemez. Sıfırlama tamamlanınca kullanıcı giriş
ekranına döner ve yeni parolasıyla girer — bütün oturumlar kapandığı için başka
cihazda açık kalmış bir oturum da düşmüştür.

## Fazla ödenmiş kartın anlatımı (Aşama 06 Grup 5)

Kart borcu artık negatif olabilir. Ekranda bu **eksi borç** diye değil,
**alacak** diye okunur: cümle yönü kendisi söyler, eksi işaretine gerek yoktur.

| Yer | Borçluyken | Alacaklıyken |
|---|---|---|
| Kart listesi satırı | `Borç ₺500,00` | `Kartınızda ₺500,00 alacağınız var` |
| Kart ayrıntısı kutusu | `Güncel borç`, gider tonu | `Kart alacağınız`, gelir tonu |
| Özet → Varlık durumu | `Kart borcu … net varlığı düşürür` | `Kart alacağı … net varlığa eklenir` |
| Planlama özeti | `Kart borcu ₺500,00` | `Kart alacağı ₺500,00` |

Satır hem **adını** hem **yönünü** değiştirir. Yalnız birini değiştirmek iki kez
yanlış olurdu: "Kart borcu −₺500,00, net varlığı düşürür" hem borç olmadığı hâlde
borç der, hem varlığı artıran bir tutarı düşüren diye anlatır.

Kullanılabilir limit o kartta limitin **üstüne** çıkar; para karttadır ve
harcanabilir. Ekstre ise değişmez: ödenecek tutar hiçbir zaman negatif olmaz.

## Varsayılan kapsamın ayarlanması (Aşama 06 Grup 4)

Kapsam türetme zinciri üç halkalıdır: **kullanıcının açık seçimi → kaynağın
(hesap/kart) etiketi → kategorinin varsayılanı**. Orta halka bugüne kadar yalnız
API'den kurulabiliyordu; artık üç formdan da kuruluyor.

| Form | Alan nerede | Ne anlatır |
|---|---|---|
| Hesap ekle/düzenle | Açılış bakiyesinin altında | "Bu hesaptan yazılan kayıtlar bu tarafa yazılır" |
| Kart ekle/düzenle | Asgari ödeme oranının altında | "Bu kartla yapılan harcamalar bu tarafa yazılır" |
| Kategori ekle/düzenle | Kategori türünün altında | Zincirin son halkası |

Alanın **üç** konumu var: `Belirtilmedi · İşletme · Şahsi`. `Belirtilmedi` eksik
veri değil, meşru bir cevaptır — "ben söylemiyorum, kararı bir sonraki halka
versin". Filtrenin `Hepsi`siyle karıştırılmasın diye adı ayrı: orada boşluk
"iki tarafı birden oku" demektir.

Alan yalnız **kapsam boyutunu gören** kullanıcıda çizilir; işletmesi olmayan
kullanıcı üç formda da onu hiç görmez.

Böylece tek hesabına "dükkân kasası" diyen esnaf her kaydı tek tek
işaretlemekten kurtulur: kayıt kategori ne derse desin hesabın etiketini alır.

## Hatırlatma akışı (Aşama 06 Grup 3)

Hatırlatma **cihazda** kurulur: telefonun kendi zamanlayıcısına yazılır,
internet gerektirmez ve geliştirme makinesi kapalıyken de çalışır. Sunucudan
push gelmez (FCM bu aşamada bilerek eklenmedi).

Kapısı `Diğer → Hatırlatmalar`. `Hesabım` içinde değil, çünkü bu bir cihaz
ayarıdır: aynı hesaba başka bir telefondan girildiğinde o telefon kendi kararını
taşır.

| Adım | Ne olur |
|---|---|
| Ekran ilk açıldığında | Yalnız `Hatırlatmaları aç` anahtarı görünür; hiçbir izin sorulmamıştır ve hiçbir bildirim kurulmamıştır |
| Anahtar açılır | Bildirim izni **o an** istenir. Verilirse yaklaşan kayıtlar okunur ve bildirimler kurulur |
| İzin reddedilir | Anahtar kapalı kalır, ekran bunu söyler ve uygulama sessizce çalışmaya devam eder |
| Tür ve saat seçilir | Beş kova (`Ödenecek yükümlülükler`, `Tahsil edilecekler`, `Tekrarlanan kayıtlar`, `Kart ekstreleri`, `Taksitler`) ve tek bir saat. Her değişiklik listeyi baştan kurar |
| Veri değişir (ödeme yapıldı, yükümlülük kapandı) | Planlanan görünüm değiştiği için liste baştan kurulur; kapanan kalemin bildirimi **düşer** |
| Oturum kapanır | Ayar ve kurulu bildirimler unutulur; aynı cihazdan giren ikinci kullanıcı birincisinin hatırlatmalarını devralmaz |

Bildirim **gün başına tektir**: aynı güne düşen kalemler tek satırda toplanır
(`2 ödenecek yükümlülük, 1 kart ekstresi`). Beş kalem beş bildirim olsaydı
kullanıcı ilk gün bildirimleri kapatırdı.

Gövdede **tutar ve karşı taraf adı geçmez** — kilit ekranında görünen bir metin
finansal bilgi taşımamalı. Ne olduğunu söyler, ne kadar olduğunu değil.

Vergi takvimi kaleminin kendi kovası yok: kalem tekrarlayan bir plandır ve
planlanan görünüme öyle düşer. Ona ayrı bir kova açmak aynı kaydı iki yerden
hatırlatırdı.

Yaklaşan liste okunamazsa (sunucuya ulaşılamıyor) **kurulu bildirimler olduğu
gibi kalır** ve ekran bunu söyler: dünkü hatırlatmayı silmek, kullanıcıyı
faturasından habersiz bırakırdı.

## Responsive gezinme ve panel akışı

Uygulama tek bir kırılım noktası tanımına dayanır (`AppBreakpoints`, Material 3
pencere sınıfları). Daha önce shell 720 dp, planlama ekranı 700 dp kullanıyordu;
aynı kavramın iki eşiği ekranların birbirinden farklı davranmasına yol açıyordu.

```text
Pencere genişliği
├── < 600 dp   compact    telefon dikey
├── 600–839 dp medium     telefon yatay, küçük tablet
└── >= 840 dp  expanded   tablet ve üzeri
```

Gezinme kabuğu:

```text
compact   -> AppBar + NavigationBar (alt çubuk)
medium    -> NavigationRail (ikon + etiket, daraltılmış)
expanded  -> NavigationRail(extended: true) (etiketler açık)
```

Ray açılan iki kademede sayfa içeriği `AppContentWidth` ile 720 dp'ye
sınırlanır ve ortalanır. Sınır olmasaydı bir işlem satırı 1400 dp ekranda ekran
boyunca uzar, göz satır sonundan satır başına dönerken yerini kaybederdi.

Panel akışı (ayrıntı sheet'i, filtre sheet'i, `İşlem ekle` launcher'ı ve
bütün form panelleri) `AppAdaptiveSheet` üzerinden açılır — veri giren
paneller bunu `AppFormSheet` kabuğuyla sarar:

```text
compact           -> showModalBottomSheet (drag handle, tam genişlik)
medium / expanded -> showDialog (ortalanmış, en fazla 720 dp,
                     yükseklik ekranın %85'i ile sınırlı)
```

İçerik widget'ı iki kademede de aynıdır; yalnız sunum kabuğu değişir ve her iki
yolda da seçilen değer çağırana aynı biçimde döner.

Kart ızgaraları (`AppResponsiveGrid`) sütun sayısını sabit bir eşikten değil
kartın okunabilir en küçük genişliğinden türetir; böylece üç kartın sığdığı bir
ekranda iki sütunda takılı kalınmaz.

## Fiş ile gider ekleme akışı

```text
İşlem ekle -> Fiş veya fatura okut -> ReceiptScanPage
                                 |  kamera | galeri (eşit iki giriş)
                                 |  -> cihazda hazırla (EXIF, 2400 px, q85)
                                 |  -> POST /api/v1/receipts/analyze
                                 v
                    pushReplacement /transactions/new/expense (extra: öneriler)
                                 v
                       mevcut gider formu, önü dolu ve her alan "öneri"
                                 v
                     kullanıcı kaynağı seçer, kontrol eder, Kaydet
```

**Ayrı bir onay formu yazılmadı.** Fişten gelen kayıt, elle girilen giderle
aynı formu ve aynı yazma yolunu kullanır; ikinci bir form zamanla ayrışır ve
aynı finansal kural iki yerde durur. Formun tek farkı, alanların önünün dolu
gelmesi ve her dolu alanın **öneri** olduğunu söylemesidir.

Akışın hiçbir adımı para yazmaz. Analiz uç noktası salt okunurdur; finansal
kayıt yalnız kullanıcı `Kaydet`e bastığında, mevcut transaction veya card-charge
uç noktasından yazılır.

**Ödeme kaynağı her zaman boş başlar ve zorunludur.** Fiş hangi hesaptan ya da
hangi karttan ödendiğini bilmez; yalnız üzerinde `NAKİT`/`KREDİ KARTI` yazdığını
bilir. Bu ipucu listenin **sırasını** değiştirir ve alanın altında bir not
olarak görünür; diğer grup gizlenmez, çünkü kullanıcı fişte yazanın aksine
ödemiş olabilir.

**Okuma başarısız olursa akış tıkanmaz.** Hata ekranında hem `Yeniden dene`
(elde duran fotoğrafı yeniden okutur, kullanıcı fişi tekrar çekmez) hem
`Bilgileri elle gir` (aynı formu boş açar) vardır. Sağlayıcının çalışmaması,
harcamanın kaydedilememesi demek değildir.

Okunamayan alan boş kalır — tahmin edilmez. Hiçbir alanı okunamayan yanıt
"okundu" diye gösterilmez; form bunu ayrı bir notla söyler.

### Rıza kapısı ve belgenin yazılması (Grup 7)

```text
Fiş veya fatura okut -> rıza soruldu mu?
                 hayır -> açıklama ekranı -> Kabul / Bilgileri elle gir
                 evet  -> kamera | galeri -> okuma -> form
form Kaydet -> hesap kaynağı:  POST /api/v1/transactions
            -> kart kaynağı:   POST /api/v1/credit-cards/{id}/charges
                 |
                 +-- Fişi sakla açık ve kaynak hesap ise:
                     POST /api/v1/transactions/{id}/attachments  (ORİJİNAL foto)
```

**Rıza ilk okumadan önce sorulur.** Fiş fotoğrafı işletme, tarih, tutar ve kimi
zaman kart son dört hanesi taşır ve cihazdan çıkıp Google'a gider. Kabul
edilmeden kamera düğmesi bile görünmez; kapı controller'da olduğu için ekranın
bir yerini atlayan ikinci bir yol açılsa da fotoğraf onaysız gitmez. Kabul
edilmezse özellik kapalı kalır, elle giriş bozulmaz. **Rıza kaydı v1'de
cihazdadır**; gerçek kullanıcıya açılırsa sunucuya taşınması gerekir.

**Belge ikinci bir istektir ve kaydın ardından gelir.** Sıra tersine
çevrilemez: belgenin bağlanacağı işlem henüz yoktur. Saklanan **orijinal**
fotoğraftır, analize giden küçültülmüş kopya değil — belge kanıttır.

**Belge eklenemezse gider kalır.** Kullanıcıya yalnız belgenin eklenemediği
söylenir (form kapandıktan sonra görünen uyarı). Fotoğraf uğruna finansal kayıt
geri alınmaz.

**Kart harcamasında fotoğraf saklanamıyor ve sebebi ekranda yazıyor.**
Sunucudaki belge uç noktası bir **işleme** bağlanır
(`/transactions/{id}/attachments`); kart harcamasının böyle bir kimliği yok ve
bu aşamada yeni yazma uç noktası açılmıyor. Anahtar bu durumda kapalı ve
gerekçesi görünür — sessizce yok sayılsaydı kullanıcı fişi sakladığını sanırdı.

**Fişi sakla anahtarı kapalıysa** byte'lar istekten hiç geçmez: dosya hiçbir
yere yazılmaz. Anahtarın son durumu cihazda hatırlanır, varsayılanı açıktır —
belge biriktirmek geri alınabilir (kullanıcı belgeyi siler), kaçırılan fiş geri
gelmez.

### Belge türü kapısı (19 Ağustos 2026)

```text
fotoğraf -> ön işleme (yön, ölçek, hafif kontrast)  [içerik denetimi YOK]
         -> model: önce documentType, sonra alanlar
              purchase_receipt -> doğrulayıcı -> taslak
              bank_document    -> 400 receipt.bank_document  -> Transfer ekranı
              diğer / belge değil -> 400 receipt.not_a_receipt -> elle giriş
         -> hiçbir alan okunamadıysa: 400 receipt.unreadable (tek cümle)
```

Ön işleme adımı **geometrik ve fotometriktir** — yön, ölçek ve yalnız karanlık
fotoğrafta hafif kontrast. Bir fotoğrafın fiş olup olmadığını sormaz ve sormak
için tasarlanmadı; belge türü kararı modelden gelir ve sunucuda kapıya bağlanır.

Dekont ekranında `Yeniden dene` **gösterilmez**: aynı fotoğraf aynı cevabı
getirir. Yerine `Dekont olarak okut` durur — aynı fotoğraf, yalnız yakalama
seçeneği değişir (20 Ağustos 2026; eskiden burada kullanıcıyı boş transfer
ekranına gönderen bir düğme vardı).

## Yakalama seçeneği ve belge başına dallanma

```text
İşlem ekle -> Fiş veya fatura okut -> seçici: Harcama | Gelir
           -> Dekont okut          -> seçici YOK (intent=bank_slip)
                                          |
                    fotoğraf -> POST /api/v1/receipts/analyze (intent ile)
                                          v
                            okunan belgeye göre dallanır:
   ┌──────────────────────────────────────────────────────────────────────┐
   │ alışveriş fişi          -> gider/gelir formu (öneriler)              │
   │ vadeli fatura           -> "Ödediniz mi?"                            │
   │        Ödedim           -> gider formu                               │
   │        Henüz ödemedim   -> yükümlülük formu (gider var, nakit yok)    │
   │ taksitli fiş            -> kart seçimi -> taksit planı formu          │
   │ iade fişi               -> eşleşen harcama -> onayla -> İPTAL         │
   │ banka belgesi (dekont)  -> karar sayfası: "Bu tutar ne?"              │
   │        Harcama          -> gider formu                                │
   │        Kendi hesabıma   -> transfer ekranı                            │
   │        Kart ödemesi     -> kart seçimi -> ödeme formu                 │
   │        Geri bekliyorum  -> alacak formu                               │
   └──────────────────────────────────────────────────────────────────────┘
```

**Menüdeki iki giriş iki ayrı sayfadır.** Alışveriş belgesinde sorulan soru
yöndür ve fotoğraftan **önce** sorulmak zorundadır (aynı kira makbuzu kiracı
için gider, ev sahibi için gelirdir). Dekontta sorulan soru belgedeki tutarın ne
olduğudur ve okuma bitmeden **sorulamaz** — aynı 5.000 TL ödeme de olabilir,
borç verme de, aktarma da. İkisi tek ekranda tek bir seçicide toplanmıştı;
kullanıcı menüde verdiği cevabı ekranda ikinci kez veriyor, dekontta ise karar
sayfası aynı soruyu üçüncü kez sorup cevabı eziyordu.

**Transfer seçeneği kalktı** (20 Ağustos 2026). Kabul ettiği tek belge türü
(`bank_document`, kendi hesaplar arası aktarma) dekont yolunda da okunuyor ve
yön zaten karar sayfasında soruluyordu; buna karşılık gerçek dekontların çoğunu
(`bank_payment`, üçüncü tarafa ödeme) reddediyordu. Sunucudaki `transfer`
niyeti duruyor, istemci artık göndermiyor.

**Yanlış kapı bir hata değil.** Dekont, fiş sayfasında okutulup reddedilirse
ekranda `Dekont olarak okut` çıkar: aynı fotoğraf, yalnız belge sınıfı değişir.

**Dekonttaki işlem ücreti ikinci bir form açmaz.** Ana kayıtla aynı kaynaktan
yazılır (banka ücreti, paranın çıktığı yerden alınır) ve **ayrı bir kayıt**
kalır; ana tutara eklemek belgede yazmayan bir toplam uydurmak olurdu. Ücret
satırı yalnız belgede ücret varsa görünür.

**Ödenmemiş fatura para hareketi üretmez ama gideri tanır.** Kullanıcı belge
tarihi, son ödeme tarihi, gider kategorisi, isteğe bağlı karşı taraf ve kapsamı
doğrulayarak tek seferlik yükümlülük yazar. Form hesap veya sıklık sormaz: kasa
ödeme yapılana kadar değişmez; gider belge tarihinde rapora girer. Ödeme daha
sonra yükümlülüğü kapatan ayrı ve nötr nakit olayıdır.

**İade yeni kayıt üretmez.** Sunucu geri verilen harcamayı arar, kullanıcı
görüp onaylar, harcama **iptal edilir** (silinmez). Kısmi iadede kalan tutar
için gider formu açılır ve kalan sunucuda hesaplanır.

**Taksitli fiş tek seferlik tam tutar gideri üretmez.** Kartı kullanıcı seçer,
plan yalnız niyettir; yalnız gerçekleşen taksit kart harcaması ve gider üretir.

**Dekonttan gelen borç/alacak planı kapsam sorar** (Aşama 06 Grup 6). Kayıt
gelir/gider raporunu etkiler ve kapsam taşımak zorundadır; zincir (kaynağın
etiketi → kategorininki) çözülemiyorsa alan zorunludur ve bu **istek gitmeden**
formda söylenir. Kapsamı görmeyen kullanıcıda alan hiç çizilmez.

## Vergi tarafının istemcisi (Aşama 05 Grup 7)

**Vergi bölümü yalnız kapsam boyutunu gören kullanıcıda çizilir.** "İşletmem
yok" diyen kişinin formunda hiç görünmez: KDV ve matrah onun sorusu değil ve
alanı göstermek, formu cevaplanmayacak bir soruyla uzatırdı.

Bölüm **kapalı** açılır ve boşken tek satırdır (`KDV girilmedi`). İçinde iki
alan var — oran (%) ve tutar — ve ikisi de isteğe bağlıdır; boş bırakılırsa
istekte `vatRate`/`vatAmount` hiç gitmez. Oranla tutarın birbirini tutup
tutmadığına dair **uyarı yok**: o uyarı istemcide bir vergi tutarı hesaplamak
olurdu ve bu üründe finansal değeri istemci hesaplamaz. Yazılan yüzde
sözleşmenin oranına çevrilir (20 → `0.2000`); bu bir hesap değil, birim
çevirisidir. Tek istemci doğrulaması bir **sınırdır**: KDV kaydın tutarını
aşamaz.

**İndirilebilirlik anahtarı yalnız işletme kapsamlı giderde görünür.** Kapsam
çipi şahsiye çevrilince anahtar kaybolur ve istekte alan hiç gitmez — sunucu
zaten reddederdi; kullanıcının görmediği bir alanı göndermek, reddedilecek bir
istek kurmak olurdu. Anahtarın altındaki cümle değerin nereden geldiğini söyler
(sizin seçiminiz / kategorinin varsayılanı / kategori belirlemiyor).

`Diğer` menüsünde iki yeni kapı, **yalnız işletmesi olan kullanıcıda**:

- **Vergi takvimi**: hazır kalemleri listeler ve mevzuat takibi yapılmadığını
  ekranda yazar. Kaleme dokunmak tekrarlayan plan formunu **önü dolu** açar;
  ekranın kendi yazma yolu yoktur — olsaydı aynı plan iki ayrı biçimde
  oluşabilirdi. Öneri tutar taşımaz; başlangıç günü önerilen günün bugünden
  sonraki ilk düşüşüdür, çünkü geçmişe kurmak ilk gerçekleşmeyi daha kurulurken
  gecikmiş yapardı.
- **Muhasebeci paketi**: varsayılan dönem **geçen aydır** (ay kapanmadan paket
  hazırlanmaz). Ekran toplamları, kayıt ve belge sayısını, KDV yazılmamış ve
  indirilebilirliği cevaplanmamış kalem sayısını gösterir; hepsi sunucudan
  gelir. `Paketi paylaş` dosyayı indirir ve cihazın paylaşım sayfasını açar.
  Boyut tavanını aşan belge varsa bu ekranda **yazılır**, sessizce düşmez.
