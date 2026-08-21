# Kişisel Bütçe Mobil — Flutter İstemci

Material 3 Android istemcisi; auth, dashboard, hesap, kategori, işlem ve bütçe
özelliklerini ASP.NET Core API'ye bağlar. View → Controller/ViewModel → Repository
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
dart format --output=none --set-exit-if-changed .
flutter analyze
flutter test
flutter build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5284
```

Temiz APK kurulumu, sentetik veri ve kabul adımları için
[`../../documentation/mvp1-local-setup-and-acceptance.md`](../../documentation/mvp1-local-setup-and-acceptance.md)
belgesini kullanın. Debug HTTP ve debug signing production yayın kanıtı değildir.
