# Yerel Kurulum ve Kabul Rehberi

Bu rehber Windows üzerinde SQL Server, ASP.NET Core API ve Pixel 8 Android
emulatorünü birlikte çalıştırır. Yalnız sentetik veri kullanılır. Fiziksel cihaz
kanıtı bu yerel kabulün parçası değildir.

## 1. Ön koşullar

- Docker Desktop, .NET 10 SDK, Visual Studio ve Android Studio kurulu olmalıdır.
- Android Studio içindeki `Pixel_8` AVD'si çalışabilmelidir.
- Repository kökünde Git dışı `.env`, API projesinde Git dışı user-secrets
  bulunmalıdır. Secret değerlerini terminale, ekran görüntüsüne veya belgeye
  yazmayın.
- `flutter doctor` Android toolchain'i doğrulamalıdır. Windows masaüstü workload
  uyarısı Android hedefi için engel değildir.

## 2. SQL Server'ı başlatma

Bu komut Compose tanımındaki yerel SQL Server'ı başlatır; named volume korunur.

```powershell
docker compose up -d sqlserver
docker compose ps
```

Beklenen sonuç `business-finance-sqlserver` container'ının `healthy` olması ve
port eşlemesinin yalnız `127.0.0.1:14334` görünmesidir. `starting` görülürse birkaç
saniye sonra `docker compose ps` komutunu yeniden çalıştırın.

Migration gerekiyorsa connection string değerini göstermeden Visual Studio
Package Manager Console veya repository'de daha önce doğrulanan EF komutuyla
uygulayın. Bağlantı hatasında önce container health durumunu, sonra API
user-secrets içindeki `ConnectionStrings:BusinessFinance` anahtarının varlığını
kontrol edin.

## 3. API'yi çalıştırma

Bu komut API'yi host loopback üzerinde başlatır; internete yayınlamaz.

```powershell
dotnet run --project src/BusinessFinance.Api --launch-profile http
```

Beklenen adres `http://localhost:5284` olur. Ayrı bir terminalde:

```powershell
Invoke-WebRequest http://localhost:5284/health/live
Invoke-WebRequest http://localhost:5284/health/ready
```

İki endpoint de sağlıklı ortamda `200` ve `Healthy` döndürmelidir. `live` 200,
`ready` 503 ise API çalışıyor fakat SQL'e erişemiyor demektir. API terminalini
açık bırakın.

## 4. Pixel 8 emulatorünü hazırlama

Android Studio > Device Manager > `Pixel_8` > Start yolunu izleyin. Emulator
host makinenin loopback adresine `10.0.2.2` takma adıyla ulaşır. Bu nedenle mobil
API adresi `http://10.0.2.2:5284` olmalıdır; emulator içinde `localhost` API'yi
değil emulatorün kendisini gösterir.

Bağlı cihazı doğrulamak için Android SDK yolunuz farklıysa yolu uyarlayın:

```powershell
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" devices
```

## 5. Debug APK üretme ve temiz kurma

Bu build yerel kabul içindir; production/release imzası kanıtı değildir.

```powershell
Set-Location mobile/business_finance_mobile
flutter build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5284
```

Ardından uygulama paketini kaldırıp üretilen APK'yı yeniden kurun:

```powershell
$adb = "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe"
& $adb uninstall com.nef.business_finance_mobile
& $adb install build\app\outputs\flutter-apk\app-debug.apk
& $adb shell am start -n com.nef.business_finance_mobile/.MainActivity
```

İlk uninstall daha önce kurulum yoksa hata verebilir; install sonucunun
`Success` olması gerekir. Temiz kurulum mobil paketi ve secure-storage oturumunu
siler, SQL named volume içindeki sunucu verisini silmez. Test izolasyonu için her
koşuda benzersiz sentetik e-posta kullanın.

## 6. Sentetik kabul veri seti

Gerçek ad, e-posta, parola, hesap veya finansal açıklama kullanmayın.

| Aktör/veri | Sentetik değer | Beklenen sonuç |
|---|---|---|
| Kullanıcı A | `stage9-a-<benzersiz>@example.test` | Yalnız A verilerini görür |
| Kullanıcı B | `stage9-b-<benzersiz>@example.test` | A verilerini göremez |
| A hesabı | Açılış 1.000,0000 TRY | İlk bakiye 1.000,0000 |
| A geliri | 250,0000 TRY | Bakiye 1.250,0000 |
| A gideri | 125,5000 TRY | Bakiye 1.124,5000 |
| A aylık bütçesi | Limit 100,0000 TRY | Harcanan 125,5000; aşım 25,5000 |
| Gider iptali | 125,5000 TRY gider | Bakiye 1.250,0000; harcanan 0; kalan 100 |

## 7. Kabul kontrol listesi

`Kanıt` alanına test adı, tarihli ekran görüntüsü veya kısa manuel gözlem yazılır.

| Kontrol | Tür | Beklenen sonuç | Kanıt |
|---|---|---|---|
| Temiz kurulum/ilk açılış | Manuel | Login ekranı, eski oturum yok | Pixel 8 clean-install gözlemi |
| Register/login | Otomatik+manuel | Geçerli kullanıcı korumalı shell'e girer | `stage8_smoke_test.dart` |
| Session restore/refresh | Otomatik | Yeniden oluşturulan uygulama oturumu açar; token döner | `stage9_auth_acceptance_test.dart` |
| Logout/local temizlik | Otomatik | Secure storage boş; eski refresh reddedilir | `stage9_auth_acceptance_test.dart` |
| İki kullanıcı izolasyonu | Otomatik | B, A'nın verisini görmez; A kendi verisini görür | auth+finance integration testleri |
| Hesap/gelir/gider matematiği | Otomatik | 1.000 + 250 - 125,50 = 1.124,50 | `stage9_finance_acceptance_test.dart` |
| Tarih/tür/hesap/kategori filtreleri | Otomatik | Yalnız seçilen ve current-user kapsamlı satırlar | finance + widget testleri |
| Bütçe spent/remaining/exceeded | Otomatik | 125,50 / 0 / 25,50; iptal sonrası 0 / 100 / 0 | finance integration testi |
| Dashboard/rapor | Otomatik | Gelir 250; gider 125,50; net 124,50 | finance integration testi |
| Hesap pasifleştirme | Otomatik | Seçenek listesinden düşer, yeni hareket 400 | finance integration testi |
| API kapalı ve tekrar deneme | Manuel+unit | Güvenli ağ hatası; API dönünce toparlanır | Kullanıcı gözlemi + `api_client_test.dart` |
| SQL kapalı | Manuel | Live 200, ready 503; SQL dönünce ready 200 | kontrollü stop/start |
| Validation/boş veri | Widget+unit | Alan hatası veya açıklayıcı boş durum | transaction/budget widget testleri |
| Timeout/çift gönderim | Unit | Açık timeout; istemci aynı submit'i kilitler | ApiClient/controller testleri |
| Kapsam: iki esnaf senaryosu | Otomatik | Kapsam gönderilmeden kayıt doğru tarafa yazılır; **işletme neti şahsi harcamadan etkilenmez**; bakiye üç kapsamda da aynı; ikinci esnaf birincinin kaydını hiçbir kapsamda görmez | `stage01_scope_acceptance_test.dart` (Pixel 8 + gerçek API/SQL, 23 Ağustos 2026) |
| Kapsam: özet ekranı ve form | Manuel | Anahtar üç konumda; hero `İşletme neti` / `Şahsi çekim` / `Bu ayın neti`; `İşletme` seçilince gider yalnız işletme tarafını gösterir; `Varlık durumu` ve `Hesap bakiyeleri` toplam gösterdiğini yazar; formdaki çip kategoriden dolar ve tek dokunuşla değişir | Pixel 8 gözlemi, 23 Ağustos 2026 |

## 8. Durdurma ve sorun giderme

API terminalinde `Ctrl+C` kullanın. SQL'i veriyi koruyarak durdurmak için:

```powershell
docker compose stop sqlserver
```

`docker compose down -v` named volume'ü siler; kabul sırasında kullanmayın.

- `İstek tamamlanamadı`: API terminalini, `/health/ready` sonucunu ve APK'nın
  doğru `API_BASE_URL` ile üretildiğini kontrol edin.
- Emulator görünmüyor: Device Manager'dan AVD'yi yeniden başlatıp `adb devices`
  çalıştırın.
- `ready` 503: SQL health ve user-secret connection string anahtarını kontrol edin.
- **Uygulama açılıyor ama ekran boş kalıyor (Flutter logosunda takılıyor):**
  Pixel 8 AVD'de Impeller/OpenGLES ilk kareyi çizemeyebiliyor; süreç yaşıyor,
  Dart VM açılıyor, ama yüzey boyanmıyor. Uygulamayı Impeller kapalı başlatın:
  `adb shell am start -n com.nef.business_finance_mobile/.MainActivity --ez enable-impeller false`.
  Bu bir uygulama hatası değil, emulator grafik yığınının durumudur; 23 Ağustos
  2026 kabul turunda görüldü ve bu yolla aşıldı.
- **Impeller kapalı başlatmak da yetmiyorsa** (26 Ağustos 2026 kabul turu):
  ekran siyah kalır, `adb shell uiautomator dump` eski bir kareyi gösterir ve
  dokunuşlar hiçbir şeyi değiştirmez — uygulama kare üretmiyordur. AVD'yi
  yazılım render'ıyla yeniden başlatmak sorunu kesin olarak çözdü:
  `emulator -avd Pixel_8 -gpu swiftshader_indirect -no-snapshot-load`.
  Yavaştır ama her kareyi çizer; kabul turu bu ayarla tamamlandı.
- Login ekranında eski oturum: APK'yı uninstall/install edin; SQL verisinin
  kalacağını unutmayın ve benzersiz sentetik kullanıcı kullanın.

## Ertelenmiş fiziksel cihaz riski

Gerçek Wi-Fi yönlendirmesi, Windows firewall izni, USB/ADB, üretici Android
katmanı ve donanım destekli secure storage fiziksel cihaz olmadan kanıtlanamaz.
Bunlar haricî test veya mağaza yayını öncesinde ayrı kapıdır; yerel kabulün
Pixel 8 emulator kabulünü geçersiz kılmaz.
