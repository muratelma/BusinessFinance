# Değişkenler ve Secret Sınırı

Bu belge yalnız uygulanmış yapılandırmayı kaydeder; gerçek secret değer içermez.

## Yapılandırma envanteri

| Ad | Kullanan | Kapsam | Kaynak | Rotation/değişim | Yanlış kullanım riski |
|---|---|---|---|---|---|
| `Jwt:Issuer` | API | Sunucu | `appsettings.json` | Sözleşme değişiminde | Mevcut tokenlar reddedilir |
| `Jwt:Audience` | API | Sunucu | `appsettings.json` | Sözleşme değişiminde | Yanlış hedef token kabulü/reddi |
| `Jwt:AccessTokenLifetime` | API | Sunucu | `appsettings.json` | Güvenlik politikasında | Fazla uzun erişim süresi |
| `Jwt:RefreshTokenLifetime` | API | Sunucu | `appsettings.json` | Güvenlik politikasında | Fazla uzun session süresi |
| `Jwt:SigningKey` | API | Secret | Local user-secrets | Sızıntıda ve planlı rotation | Token sahteciliği; eski tokenlar geçersizleşir |
| `MSSQL_SA_PASSWORD` | Compose SQL | Secret | Git dışı `.env` | Sızıntıda/ortam yenilemede | Veritabanı yönetici erişimi |
| `MSSQL_HOST_PORT` | Compose | Host | Git dışı `.env`; varsayılan 14334 | Port çakışmasında | Yanlış/broad port yayını |
| `ConnectionStrings:BusinessFinance` | API/EF | Secret | Local user-secrets | SQL parolası değişince | Veri erişimi ve parola sızıntısı |
| `ConnectionStrings__BusinessFinance` | EF CLI | Geçici process | Environment | Her komut/oturum | Profile/log içine kalıcı sızıntı |
| `BUSINESS_FINANCE_SQL_TEST_CONNECTION` | SQL testleri | Geçici test | Environment | Test ortamıyla | Testin yanlış veritabanına yazması |
| `BUSINESS_FINANCE_REPOSITORY_ROOT` | Mimari test | Test | Environment | Repo yolu değişince | Yanlış kaynak ağacı taraması |
| `BUSINESS_FINANCE_GEMINI_TEST_KEY` | Canlı sözleşme testi ve manuel fiş ölçümü | **Secret**, geçici test | Environment | Test/ölçüm oturumuyla | Kotanın habersiz harcanması; değer test veya ölçüm çıktısına yazılmaz |
| `BUSINESS_FINANCE_GEMINI_TEST_MODEL` | Canlı sözleşme testi | Test | Environment; varsayılan `gemini-3.5-flash-lite` | Ölçüm turunda | RPD 20'lik modelde testin kotayı bitirmesi |
| `API_BASE_URL` | Flutter | Build-time, secret değil | `--dart-define` | Hedef ortam değişince | Yanlış API'ye istek; emulator için localhost hatası |
| `ASPNETCORE_ENVIRONMENT` | API | Process/profil | launch profile/environment | Ortama göre | Development OpenAPI'nin yanlış ortamda açılması |
| `ASPNETCORE_URLS` / `--urls` | API | Process | launch profile/CLI | Yerel porta göre | `0.0.0.0` ile istenmeyen ağ yayını |
| `AttachmentStorage:RootPath` | API | Sunucu, secret değil | `appsettings.json`/environment | Storage taşımasında | Root kaçışı, kayıp veya yanlış volume |
| `Gemini:ApiKey` | API | **Secret** | Local user-secrets | Sızıntıda ve planlı rotation | Üçüncü taraf kotasının ve faturasının başkasınca kullanımı |
| `Gemini:Model` | API | Sunucu, secret değil | `appsettings.json`; varsayılan `gemini-3.5-flash-lite` | Ölçüm turu sonucuna göre | RPD 20'lik bir modele geçmek günlük kotayı bitirir |
| `Gemini:BaseUrl` | API | Sunucu, secret değil | `appsettings.json` | Sağlayıcı/bölge değişiminde | İsteğin yanlış hedefe gitmesi |
| `Gemini:TimeoutSeconds` | API | Sunucu, secret değil | `appsettings.json`; varsayılan **60** (ölçülen gecikmeden türetildi) | Nihai model seçilince | Kısa değer, model okumayı bitirmişken çağrıyı keser; uzun değer kullanıcıyı bekletir |
| `ReceiptImage:MaxLongEdgePixels` | API | Sunucu, secret değil | `appsettings.json`; varsayılan 1536 | Ölçüm turu sonucuna göre | Fazla küçültme okunabilirliği, fazla büyütme token maliyetini bozar |
| `ReceiptImage:JpegQuality` | API | Sunucu, secret değil | `appsettings.json`; varsayılan 85 | Ölçüm turu sonucuna göre | Aşırı sıkıştırma soluk baskıyı siler |
| `ReceiptImage:DarkImageLuminanceThreshold` | API | Sunucu, secret değil | `appsettings.json`; varsayılan 0.38 | Ölçüm turu sonucuna göre | Yüksek eşik iyi aydınlatılmış fişe gereksiz müdahale eder |
| `ReceiptImage:ContrastAmount` / `:BrightnessAmount` | API | Sunucu, secret değil | `appsettings.json`; varsayılan 1.15 / 0.06 | Ölçüm turu sonucuna göre | Sert değer eşiklemeye dönüşüp bilgi siler |

## Yerel secret sınırı

Transfer/kart/taksit ve recurring/upcoming/report kapsamları yeni
environment variable veya secret eklemedi. Yeni API endpoint'leri mevcut JWT,
SQL connection string ve Flutter `API_BASE_URL` sınırlarını yeniden kullanır.
Taksit `ClientRequestId` ile recurring `OccurrenceKey` değerleri secret değildir;
retry/idempotency kimlikleridir ve authentication yerine geçmez.

`Jwt:SigningKey` en az 32 byte olmalıdır; eksik/kısa değer API başlangıcında
reddedilir. Connection string ve signing key `appsettings`, `.http`, log,
response veya Git'e yazılmaz. Local user-secrets üretim vault'u değildir.

Compose parola değerini taşımaz ve eksik `MSSQL_SA_PASSWORD` ile kurulumu
reddeder. `.env.example` yalnız anahtar ve örnek biçim içerir. Gerçek `.env`
Git dışında kalır ve `scripts/New-LocalSqlEnvironment.ps1` ile değer ekrana
basılmadan oluşturulabilir.

Yerel SQL adresi `127.0.0.1,14334` biçimindedir. `TrustServerCertificate=True`
yalnız yerel container geliştirmesi içindir. SQL test connection değeri yoksa
`[SqlServerFact]` testleri açıkça skip olur; değer test çıktısına yazılmaz.

`BUSINESS_FINANCE_SQL_TEST_CONNECTION`'ın **iki tüketicisi vardır ve
beklentileri aynı değildir**; tek bir değer ikisini birden karşılamak zorunda
olduğu için hedef veritabanının doğru seçilmesi gerekir:

| Tüketici | Bağlantıyı nasıl kullanır | Hedef veritabanından beklentisi |
|---|---|---|
| `SqlServerPersistenceIntegrationTests` (Infrastructure) | Yalnız sunucu/kimlik bilgisini alır; `InitialCatalog`'u `BusinessFinanceIntegration_<guid>` ile değiştirip kendi geçici veritabanını kurar, migrate eder ve test sonunda düşürür | Yok — adı görmezden gelir |
| `SqlServerApiIntegrationTests` ve `ReceiptEndpointTests` (API) | Bağlantıyı **olduğu gibi** uygulamanın connection string'i yapar; API başlangıçta migration uygulamaz | Şeması **önceden uygulanmış** bir veritabanı |

Bu yüzden değer `master` gibi şemasız bir veritabanına yöneltilirse
Infrastructure testleri geçer ama API SQL testleri `500` ile düşer. Yerelde
kullanılan hedef `BusinessFinanceApiSqlTests`'tir ve **her yeni migration'dan
sonra ayrıca `dotnet ef database update` ile yükseltilir** — üretim verisinin
değil, test hedefinin bakımıdır.

## Fiş okuma ve dış sağlayıcı sınırı

`Gemini:ApiKey` bu projedeki **ilk dış servis secret'ıdır** ve yalnız local
user-secrets'ta durur. `appsettings.json`, `.env`, `.http` dosyası, log,
response gövdesi ve Git'e yazılmaz. İstemciye hiç gitmez: anahtar yalnız
sunucu tarafında kullanılır, çünkü APK'ya konan bir anahtar herkesin
anahtarıdır. Fiş okuma uç noktasının var olma sebebi budur.

Anahtar **yokken uygulama başlar**; yalnız fiş okuma uç noktası
`receipt.disabled` döner. Eksik anahtar bir başlangıç hatası değildir, kapalı
bir özelliktir.

Fotoğraf, sağlayıcıya giden istek ve sağlayıcıdan dönen yanıt **loglanmaz**;
hata yolunda yalnız durum kodu ve süre tutulur. Sağlayıcının hata gövdesi
kullanıcının kendi fişini geri taşıyabildiği için istisna mesajına da
konmaz.

Ücretsiz katmanda gönderilen içerik Google'ın ürün geliştirmesinde
kullanılır; bu yüzden bu aşamada yalnız **sentetik fiş** gönderilir. Gerçek
fiş gönderimi hem ücretli katman hem bulut güvenlik kapısı (Aşama 07) ister.

## Mobil istemci sınırı

Flutter APK içinde connection string, JWT signing key, SQL parolası veya başka
sunucu secret'ı bulunmaz. `API_BASE_URL` bir secret değildir; debug APK için
`http://10.0.2.2:5284` emulator-host köprüsünü seçer. Dart yapılandırması HTTP ve
HTTPS şemalarını kabul eder; debug cleartext izni yalnız debug Android manifestte
bulunur. Bu durum release/production HTTPS politikasının henüz tamamlandığı
anlamına gelmez.

Access ve refresh tokenlar çalışma zamanında Android secure storage'da tek bir
session kaydı olarak tutulur. Parola saklanmaz ve tokenlar loglanmaz.

Attachment root varsayılan olarak API executable base dizinine göre
`storage/attachments` değeridir. Object key kullanıcı girdisinden üretilmez ve
root dışında çözülen path reddedilir. Bu klasör Git'e eklenmez; container/host
kalıcılığı ayrıca volume ile sağlanmalıdır. Yerel disk şifreleme veya yönetilen
malware scanning sağlamaz. Backup v2 de şifreli değildir; gerçek finans verisiyle
kullanım bulut güvenlik kapısına (Aşama 07) kadar yasaktır.

## Yayın öncesi kontrol listesi

- Production signing key yönetilen secret deposundan alınmalı ve rotation planı
  belgelenmelidir.
- Production SQL hesabı en düşük yetkili olmalı; SA kullanılmamalıdır.
- API yalnız HTTPS üzerinden sunulmalı; release manifest/network policy ayrıca
  doğrulanmalıdır.
- Kalıcı application ID ve Android signing yapılandırması belirlenmelidir.
- Secret, APK ve Git history taramaları temiz olmalıdır.
- Development OpenAPI ve ayrıntılı hata davranışı production'da kapalı olmalıdır.

## Vergi tarafının yapılandırma kaynağı: yok

Aşama 05 **yeni bir yapılandırma anahtarı, secret ya da ortam değişkeni
getirmedi** ve bu bilinçli bir sonuçtur (ADR 0016).

- **Oranlar ve beyan tarihleri yapılandırma değildir.** Mevzuata bağlı hiçbir
  değer koda, `appsettings`'e ya da ortam değişkenine yazılmaz; uygulama mevzuat
  takibi yapmaz. Kullanıcının kurduğu takvim kalemi kendi verisidir ve
  veritabanında tekrarlayan bir plan olarak durur.
- Takvimin hazır kalemleri (`TaxCalendarSuggestions`) kodda duran bir
  **şablondur**: tutar taşımaz, kurulduğu an kullanıcının verisi olur ve
  uygulama onu sonradan kendiliğinden güncellemez. Yapılandırma kaynağı değil,
  formun ön dolumudur.
- Muhasebeci paketi dosyası kullanıcının **kendi cihazından** paylaşılır;
  sunucu üçüncü kişiye hiçbir şey göndermez ve paket için bir adres, anahtar ya
  da sağlayıcı ayarı yoktur.

