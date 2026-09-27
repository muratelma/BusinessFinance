# 06.2 devir notu — bulut (claude.ai) session'ı için

> Yeni bulut session'ında okuma sırası: `CLAUDE.md` → `stages/06.2-arayuz-duzeni.md`
> → `stages/06.2-devir/yerel-claude-code.md` (yapılanların özeti, sapmalar,
> açık konular) → bu not (bulut ortamına özgü çalışma biçimi).
> Tarih: 27 Eylül 2026.

## 1. Durum

- Dal: `ui-trials`. Claude Design teslimindeki yedi ekranın hepsi uygulandı
  (Özet, Bütçeler, İşlemler + detay, İşlem ekle, Kasa, Diğer, Hesabım).
  Ayrıntı ve commit tablosu yerel devir notunda.
- Bu session'ın işi: kullanıcının yerelde veya cihazda gördüğü **düzeltmeleri**
  uygulamak. Düzeltmeler "ekran → ne gördüm → ne bekliyordum" biçiminde gelir.
- Tasarım kaynağı repoda: `design/claude-design-handoff/`
  (`README.md`, `Son Tasarim.html`, `screens-v2/v3/v4/*.jsx`, `_ds/` token'ları,
  `screenshots/01–18`). HTML/JSX kopyalanmaz; ekranlar Flutter bileşenleriyle kurulur.

## 2. Kullanıcının bu dal için verdiği kurallar

- Bütün işler `ui-trials` dalında yapılır ve oraya push edilir.
- 06.2'nin eski "backend/migration değişmez" kısıtı **kaldırıldı**: backend ve
  migration değişikliği serbest, gerektiğinde yapılır.
- **Commit'lerde yapay zekâ imzası olmaz**: `Co-Authored-By`, "Generated with",
  session bağlantısı yok. Yazar `muratelma <elma6004@gmail.com>`. Mesaj
  İngilizce, dürüst tür (`feat`, `fix`, `docs`, `test`, `chore`) — repo kuralı.
- Her adımda test yapılır ve ekran **tasarımla karşılaştırılır** (bölüm 4).
- Sorunlarda en mantıklı çözüm uygulanır; mevcut bir kuralla çatışıyorsa yine
  uygulanır ama **raporda nerede ayrıldığı yazılır**.
- Raporlar Türkçe verilir.

## 3. Ortam

Setup script (`scripts/cloud-setup.sh`, ortam ayarlarına da girildi) session
açılırken şunları kurar:

- .NET 10 SDK (Ubuntu paketi `dotnet-sdk-10.0`),
- Flutter **3.47.5** → `/opt/flutter`, PATH `~/.bashrc` üzerinden,
- Pillow (karşılaştırma betiği için).

İlk komutta doğrula:

```bash
flutter --version && dotnet --version
```

`flutter` bulunamazsa: `export PATH=/opt/flutter/bin:$PATH`. Hiç kurulmamışsa
`bash scripts/cloud-setup.sh` (≈40 sn).

Bilinen ortam durumları — kodla ilgisi yok:

- **SQL Server yok.** `[SqlServerFact]` testleri atlanır; SQL'e dokunan
  değişiklik yerelde doğrulanmalı (kullanıcıya not düşülür).
- Infrastructure'daki **10 `ReceiptImagePreprocessorTests` testi** düşer:
  SkiaSharp yerel kütüphanesi yüklenemiyor. Değişiklik öncesi kodda da aynı.
- `flutter analyze` / `flutter pub get` bazen `analysis_options.yaml`'a
  `analyzer: exclude` bloğu ekliyor ve `pubspec.lock`'u değiştiriyor. **Commit
  öncesi** `git checkout -- mobile/business_finance_mobile/analysis_options.yaml
  mobile/business_finance_mobile/pubspec.lock`.
- `dotnet-install.sh` proxy'de 403 verir; .NET yalnız apt ile kurulur.

## 4. Çalışma döngüsü (her düzeltme için)

```bash
cd mobile/business_finance_mobile
dart format lib test
flutter analyze
flutter test                                  # hedefli: flutter test test/features/cash
```

Backend dokunulduysa, repo kökünden:

```bash
dotnet build
dotnet format --verify-no-changes
dotnet test --no-build
```

Tasarım karşılaştırması (ekran görüntüsü testleri `test/screenshots/`,
`SCREENSHOT_DIR` yoksa atlanır):

```bash
cd mobile/business_finance_mobile
SCREENSHOT_DIR=/tmp/shots flutter test test/screenshots/kasa_screenshot_test.dart
python3 tool/compare_screenshots.py \
  ../../design/claude-design-handoff/screenshots/11-kasa.png \
  /tmp/shots/11-kasa.png /tmp/cmp-11.png
```

Sonra `/tmp/cmp-11.png` görüntü olarak okunur (solda tasarım, sağda Flutter).
Ekran ↔ test dosyası: `dashboard_` (01–03), `budgets_` (05),
`transactions_` (06–09), `quick_add_` (10), `kasa_` (11–15), `diger_` (16),
`hesabim_` (17–18). Örnek veri: `test/screenshots/design_sample_data.dart`.

## 5. Tuzaklar (önceki session'da öğrenilenler)

- `design_tokens_test` `EdgeInsets`/`SizedBox` içinde sayı sabiti yasaklar;
  `AppSpacing` ifadeleri kullanılır (`AppSpacing.small + AppSpacing.xSmall`).
- Erişilebilirlik kapısı: 48 dp dokunma hedefi, her dokunulabilir düğümde
  etiket (`MergeSemantics` + `Semantics(button: true)`), 2.0× yazıda taşma yok
  (`context.usesLargeText` ile alt alta dizme). Her ekran testi bunu sınar.
- `AppMoneyText` rengi rolden seçer, `style.color`'ı ezer; soluk tutar için düz
  `Text` + `AppTypography.money`.
- Para istemcide `double` ile hesaplanmaz; önizleme gerekiyorsa `MoneyMath`.
- Ekran görüntüsü testlerinde `pumpAndSettle` yerine harness'in sabit pump'ı;
  fontlar `runAsync` içinde yüklenir.
- 25 Eylül 2026 **Cuma**dır (tasarım "Perşembe" yazıyor).

## 6. Bitirirken

- İlgili belgeler güncellenir (`documentation/design-system.md`,
  `documentation/flows.md`, gerekirse `architecture.md`, `docs/project-status.md`).
- Commit + `git push origin ui-trials`.
- Türkçe rapor: ne değişti, test sonuçları, tasarımla karşılaştırma, kurallardan
  sapılan yerler.
