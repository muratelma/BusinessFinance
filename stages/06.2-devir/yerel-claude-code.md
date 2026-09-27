# 06.2 devir notu — yerel Claude Code için

> Bu not `ui-trials` dalındaki Claude Design uygulamasının durumunu anlatır.
> Yerel oturumda önce `CLAUDE.md`, sonra `stages/06.2-arayuz-duzeni.md` ve bu
> not okunur. Tarih: 27 Eylül 2026.

## 1. Bulut oturumunda ne yapıldı (özet)

Claude Design teslim paketi (`design/claude-design-handoff/`) Flutter
istemcisine uygulandı. HTML/JSX kopyalanmadı; ekranlar mevcut Flutter
bileşenleriyle ve `lib/core/widgets/` altına eklenen yeni ortak bileşenlerle
yeniden kuruldu. Her ekran tek commit:

| Commit | Ekran | Özet |
|---|---|---|
| `eacd120` | Aşama belgesi | 06.2'ye "Madde 1 — Claude Design teslim paketi" eklendi; backend/migration yasağı kaldırıldı |
| `8c25df4` | Özet | Başlık + avatar, kapsam rayı, kategori/bütçe halkaları, yaklaşanlar zaman çizelgesi, net varlık kartı. Backend: net varlığa varlık/borç ara toplamları ve sıradaki geçiş günü; planlananlara 7 günlük çıkış toplamı |
| `5c7ca5b` | Bütçeler | Geri oklu başlık, ay seçici, doluluğa göre sıralı kartlar, durum kapsülü, düzenle/sil harcama panelinde |
| `14bb2aa` | İşlemler + detay | Arama (backend `search` filtresi, 100 karakter sınırı), `Borçlar` çipi, planlananlar şeridi, güne göre gruplu akış, yeni detay paneli |
| `550ed2b` | İşlem ekle | Üç renkli kutucuk, belgeden oku, diğer listesi |
| `4deef5c` | Kasa | Sekmeler kalktı, tek akış; sayım paneli (`Toplamı yaz / Banknotla say`, binlik ayırıcı, tam aritmetikli canlı fark). Backend: `today` cevabına son sayım + bugünkü nakit giriş/çıkış; sayım anındaki beklenen bakiye `ExpectedAtCount` olarak saklanıyor (**yeni migration**) |
| `fe20443` | Diğer | Hesap kartı + dört grup (Para ve hesaplar, Planlama, Vergi ve muhasebe, Ayarlar) |
| `41242f6` | Hesabım | Uyarı kimlik kartında, Tercihler, oturum sayacı + `Diğerlerini kapat`, ayrı çıkış düğmesi, silmeden önce yedek |

Yeni ortak bileşenler: `AppPageHeader`, `AppAvatar`, `AppIconCapsule`,
`AppRow`, `AppDividedColumn`, `AppStatusTag`, `AppCardHead`, `AppTextAction`,
`AppDateLeaf`, `AppDetailBlock`, `AppSegmentRail`; biçimlendirici
`core/formatters/money_math.dart` (`MoneyMath`, `TurkishAmountInputFormatter`).

Buluttaki son kontrol: Flutter analyze temiz, 891 test geçti, web build
başarılı; backend build/format temiz, Domain/Application/Api testleri geçti.
**SQL Server testleri bulutta hiç koşmadı** (ortamda SQL yok).

### Kurallardan / tasarımdan bilinçli sapmalar

1. `ExpectedAtCount` saklanıyor; mimari belgesi "beklenen tutar saklanmaz"
   diyordu. Yalnız geçmiş listesi için gözlemdir, hiçbir hesap okumaz; belge
   güncellendi. Kolon nullable, backfill yok. **Yedek v9 bu kolonu taşımaz**
   (geri yüklenen sayımlarda beklenen/fark boş görünür).
2. Farkı kaydedilmiş günün sayımı farkı bu gözlemden okur (yoksa sıfır görünürdü).
3. Tasarım "25 Eylül Perşembe" diyor; 25 Eylül 2026 Cuma. Uygulama doğru günü yazar.
4. Sayım panelinden not alanı kalktı (tasarımda yok); eski notlar geçmişte görünür.
5. `Diğerlerini kapat` için backend'de toplu uç yok; istemci tekil uçla sırayla kapatır.
6. Takvim yaprağının metni 1,2×'te durur (2×'te sığmıyordu); tarih ekran okuyucuya ayrıca okunur.
7. Kişisel profilde `Kasa` → Para ve hesaplar; `Hesabım` liste satırı değil, üst kart.
8. Avatar baş harfleri e-postadan (isim saklanmıyor).
9. Önceki turlardan: "Şahsi çekim" etiketi, "Limite yakın" rozeti, nötr
   "Gelir/gider raporunu etkilemez" notu, Belge satırı "Veri araçlarında",
   borç satırı kırmızı, çipler 48 dp için biraz daha yüksek.

## 2. Yerelde yapılacaklar

### 2.1 Dalı çek

```powershell
git fetch origin
git checkout ui-trials
git pull origin ui-trials
```

### 2.2 Veritabanı

Bu dalda **tek yeni migration** var: `20260927104304_AddCashCountExpectedSnapshot`
(`CashCounts` tablosuna nullable `ExpectedAtCount decimal(19,4)`; backfill yok).

```powershell
docker compose up -d sqlserver
docker compose ps          # business-finance-sqlserver "healthy" olmalı
dotnet tool restore
dotnet ef database update --project src/BusinessFinance.Infrastructure --startup-project src/BusinessFinance.Api
```

Bağlantı hatasında `documentation/local-setup-and-acceptance.md` bölüm 2'ye bak
(user-secrets `ConnectionStrings:BusinessFinance`).

### 2.3 Backend testleri — özellikle SQL olanlar

```powershell
dotnet build
dotnet test
```

SQL testleri yalnız `BUSINESS_FINANCE_SQL_TEST_CONNECTION` ortam değişkeni
tanımlıyken koşar. Bu dalda eklenen ve **ilk kez yerelde koşacak** testler:

- `FinancialActivityFeed_SearchMatchesVisibleTextAndCountsOnlyMatches` (arama)
- POS/kasa SQL testine eklenen `ExpectedAtCount` ve günlük giriş/çıkış doğrulamaları
- `MigrationHistoryTests.AddCashCountExpectedSnapshot_AddsOneNullableColumnWithoutBackfill`

```powershell
dotnet test src/BusinessFinance.Infrastructure.Tests --filter "FullyQualifiedName~SqlServerPersistence"
```

Bulutta düşen 10 `ReceiptImagePreprocessorTests` testi ortam kaynaklıydı
(SkiaSharp yerel kütüphanesi yüklenemedi); yerelde geçmeleri beklenir.

### 2.4 API ve uygulama

```powershell
dotnet run --project src/BusinessFinance.Api --launch-profile http
# ayrı terminal
cd mobile/business_finance_mobile
flutter pub get
flutter analyze
flutter test
flutter run -d chrome --web-port=65087
```

Android için: `flutter run --dart-define=API_BASE_URL=http://10.0.2.2:5284`.

### 2.5 Ekran kontrol listesi

- **Özet**: kapsam rayı (Hepsi/İşletme/Şahsi), avatar noktası (doğrulanmamış e-posta), bütçe halkaları, yaklaşanlar toplamı, net varlık kartı.
- **Bütçeler**: ay değiştirme, kart sırası, harcama panelinden düzenle/sil.
- **İşlemler**: arama (350 ms gecikmeli), çipler, planlananlar şeridi, gün başlıkları, detay paneli ve iptal onayı.
- **İşlem ekle**: ortadaki `+`; üç kutucuk ve `Diğer` listesi doğru formları açıyor mu.
- **Kasa**: kasa seçici bakiyeleri, `Sayımı gir` (iki mod), `Farkı kaydet`, geçmiş satırlarında "Beklenen" ve fark, POS `+ Ekle` ve detay paneli, `İşlem ekle > POS tahsilatı` formu doğrudan açıyor mu.
- **Diğer**: hesap kartı, gruplar, kişisel/işletme profilinde farklı satırlar.
- **Hesabım**: doğrulama akışı, `İşletmem var`, `Diğerlerini kapat`, parola, çıkış, silme iki kapısı.
- **Büyük yazı**: cihazda yazı boyutunu en büyüğe al, taşma var mı bak.
- **Koyu tema**: tasarım yalnız açık temayı çizdi; koyu temada okunabilirliğe bak.

### 2.6 Tasarımla karşılaştırma (isteğe bağlı)

```powershell
cd mobile/business_finance_mobile
$env:SCREENSHOT_DIR = "$PWD/build/shots"
flutter test test/screenshots
python tool/compare_screenshots.py ../../design/claude-design-handoff/screenshots/11-kasa.png build/shots/11-kasa.png build/cmp-11.png
```

Görüntü testleri `SCREENSHOT_DIR` yoksa atlanır; normal `flutter test`i
etkilemez. Tasarım görüntüleri 01–18 numaralıdır (`design/claude-design-handoff/README.md`).

## 3. Açık konular

- SQL testleri yerelde doğrulanmalı (2.3).
- Yedek biçimi `ExpectedAtCount`'u taşımıyor; sonraki yedek sürümünde eklenebilir.
- Bulut ortamında `flutter analyze` `analysis_options.yaml`'a `analyzer: exclude`
  bloğu ekliyordu; commit'lere girmedi. Yerelde görülürse geri alınmalı.
- `ui-trials` henüz `main`'e birleşmedi; kalıcı olacak parçalar 06.2 gruplarına
  bağlanarak kapatılacak (aşama belgesi).

## 4. Düzeltme isterken

En verimli biçim: **ekran → ne gördün → ne bekliyordun** (varsa ekran
görüntüsü). Örnek: "Kasa → Banknotla say → ₺5 satırındaki tutar sağa
yaslanmamış → diğer satırlar gibi sağda olmalı".
