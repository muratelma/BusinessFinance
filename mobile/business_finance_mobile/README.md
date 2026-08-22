# BusinessFinance Mobil — Flutter İstemci

Şahıs şirketi ve esnaf için işletme finansı uygulamasının Material 3 Android
istemcisi; auth, özet, hesap, kategori, işlem, bütçe, kart, planlama, borç,
hedef, veri araçları ve fiş okuma özelliklerini ASP.NET Core API'ye bağlar. View → Controller/ViewModel → Repository
→ Service ayrımı kullanılır. Tokenlar secure storage'da tutulur; sunucu secret'ı
ve connection string APK'ya girmez.

## Yerel çalıştırma

Önce repository kökündeki SQL Server ve API'yi başlatın, ardından Pixel 8
emulatorü için:

```powershell
flutter pub get
flutter run --dart-define=API_BASE_URL=http://10.0.2.2:5284
```

Kalite kontrolleri:

```powershell
dart format --output=none --set-exit-if-changed lib test
flutter analyze
flutter test
flutter build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5284
```

Temiz APK kurulumu, sentetik veri ve kabul adımları için
[`../../documentation/local-setup-and-acceptance.md`](../../documentation/local-setup-and-acceptance.md)
belgesini kullanın. Debug HTTP ve debug signing production yayın kanıtı değildir.
