import 'dart:math' as math;
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_finance_colors.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';

import '../../helpers/contrast.dart';

/// Bir finans rolünün üç rengi: yüzey üzerindeki metin rengi ve chip çifti.
typedef _Role = ({
  String name,
  Color onSurface,
  Color container,
  Color onContainer,
});

List<_Role> _rolesOf(AppFinanceColors colors) => [
  (
    name: 'income',
    onSurface: colors.income,
    container: colors.incomeContainer,
    onContainer: colors.onIncomeContainer,
  ),
  (
    name: 'expense',
    onSurface: colors.expense,
    container: colors.expenseContainer,
    onContainer: colors.onExpenseContainer,
  ),
  (
    name: 'neutral',
    onSurface: colors.neutral,
    container: colors.neutralContainer,
    onContainer: colors.onNeutralContainer,
  ),
  (
    name: 'planned',
    onSurface: colors.planned,
    container: colors.plannedContainer,
    onContainer: colors.onPlannedContainer,
  ),
  (
    name: 'cancelled',
    onSurface: colors.cancelled,
    container: colors.cancelledContainer,
    onContainer: colors.onCancelledContainer,
  ),
];

void main() {
  final themes = {'light': AppTheme.light(), 'dark': AppTheme.dark()};

  group('AppFinanceColors kontrast kapısı', () {
    for (final entry in themes.entries) {
      final themeName = entry.key;
      final theme = entry.value;
      final colors = theme.extension<AppFinanceColors>()!;
      final scheme = theme.colorScheme;

      // Tutarlar hem düz zeminde hem kart içinde gösteriliyor; kartın rengi
      // yüzeyden farklı olduğu için iki zemine karşı da ölçülür.
      final surfaces = {
        'surface': scheme.surface,
        'surfaceContainerLow': scheme.surfaceContainerLow,
        'surfaceContainerHighest': scheme.surfaceContainerHighest,
      };

      for (final role in _rolesOf(colors)) {
        for (final surface in surfaces.entries) {
          test(
            '$themeName: ${role.name} metni ${surface.key} üzerinde AA geçer',
            () {
              final ratio = contrastRatio(role.onSurface, surface.value);
              expect(
                ratio,
                greaterThanOrEqualTo(wcagAaNormalText),
                reason:
                    '${role.name} rengi ${surface.key} üzerinde '
                    '${ratio.toStringAsFixed(2)}:1 — en az '
                    '$wcagAaNormalText:1 olmalı.',
              );
            },
          );
        }

        test('$themeName: ${role.name} chip çifti AA geçer', () {
          final ratio = contrastRatio(role.onContainer, role.container);
          expect(
            ratio,
            greaterThanOrEqualTo(wcagAaNormalText),
            reason:
                'on${role.name}Container / ${role.name}Container '
                '${ratio.toStringAsFixed(2)}:1 — en az '
                '$wcagAaNormalText:1 olmalı.',
          );
        });

        test('$themeName: ${role.name} chip zemini yüzeyden ayırt edilir', () {
          final ratio = contrastRatio(role.container, scheme.surface);
          expect(
            ratio,
            greaterThanOrEqualTo(1.2),
            reason:
                '${role.name}Container yüzeyden ayırt edilemiyor '
                '(${ratio.toStringAsFixed(2)}:1); chip görünmez olur.',
          );
        });
      }

      // Grafik dolguları metin değildir; eşikleri 3:1'dir. Ayrı ölçülmelerinin
      // sebebi, metin tonuyla aynı olmamaları: metin 4.5:1'e mecbur olduğu
      // için canlı olamıyor, dolgu olabiliyor.
      final fills = {
        'incomeFill': colors.incomeFill,
        'expenseFill': colors.expenseFill,
        'neutralFill': colors.neutralFill,
      };
      for (final fill in fills.entries) {
        for (final surface in surfaces.entries) {
          test(
            '$themeName: ${fill.key} ${surface.key} üzerinde metin dışı AA geçer',
            () {
              final ratio = contrastRatio(fill.value, surface.value);
              expect(
                ratio,
                greaterThanOrEqualTo(wcagAaLargeText),
                reason:
                    '${fill.key} ${surface.key} üzerinde '
                    '${ratio.toStringAsFixed(2)}:1 — en az '
                    '$wcagAaLargeText:1 olmalı.',
              );
            },
          );
        }
      }

      for (var i = 0; i < colors.categorySlices.length; i++) {
        for (final surface in surfaces.entries) {
          test(
            '$themeName: kategori dilimi $i ${surface.key} üzerinde görünür',
            () {
              final ratio = contrastRatio(
                colors.categorySlices[i],
                surface.value,
              );
              expect(
                ratio,
                greaterThanOrEqualTo(wcagAaLargeText),
                reason:
                    'kategori dilimi $i ${surface.key} üzerinde '
                    '${ratio.toStringAsFixed(2)}:1 — en az '
                    '$wcagAaLargeText:1 olmalı.',
              );
            },
          );
        }
      }
    }
  });

  group('AppFinanceColors sözleşmesi', () {
    test('her rol kendine ait bir renk taşır', () {
      // Rollerin birbirine eşit olmaması, iki farklı anlamın aynı token'a
      // düşmesini yakalar (ör. nötr hareketin gelir yeşilini devralması).
      //
      // Renklerin birbirinden *ayırt edilebilirliği* burada test edilmez ve
      // edilemez: kontrast oranı parlaklık farkını ölçer, ton farkını değil;
      // renk körlüğünde yeşil/kırmızı ayrımı hiçbir ölçütle garanti edilemez.
      // Bu yüzden proje kuralı "hiçbir bilgi yalnız renkle taşınmaz"dır ve
      // gerçek koruma ikon + metin ile widget katmanında test edilir.
      for (final colors in [AppFinanceColors.light, AppFinanceColors.dark]) {
        final roleColors = _rolesOf(colors).map((role) => role.onSurface);
        expect(roleColors.toSet(), hasLength(roleColors.length));
      }
    });

    test('her rolün metin ve dolgu tonu ayrıdır', () {
      // Bu ayrım kasıtlıdır ve kaybolursa "üç farklı yeşil" sorunu geri
      // gelir: dolgu metin tonuna eşitlenirse grafikler koyu ve ölü, metin
      // dolguya eşitlenirse yazı okunmaz olur.
      for (final colors in [AppFinanceColors.light, AppFinanceColors.dark]) {
        expect(colors.incomeFill, isNot(colors.income));
        expect(colors.expenseFill, isNot(colors.expense));
        expect(colors.neutralFill, isNot(colors.neutral));
      }
    });

    test('kategori paleti rol renklerine girmez ve dilimler ayrışır', () {
      // Palet önce tek hue'un tonlarıydı; halka grafiğinde yan yana duran altı
      // kırmızı tonu efsaneyle eşleştirmek mümkün olmuyordu. Ayrı hue'lara
      // geçildi; hiçbir hue artık yasak değil (yeşil ve mavi de paletin
      // içinde), koruma **uzaklığa** bakıyor: bir dilim rol renklerinden
      // ölçülebilir biçimde ayrılmak zorunda.
      for (final colors in [AppFinanceColors.light, AppFinanceColors.dark]) {
        expect(colors.categorySlices, isNotEmpty);
        expect(
          colors.categorySlices.toSet(),
          hasLength(colors.categorySlices.length),
          reason: 'Aynı renkli iki dilim ayırt edilemez.',
        );
        // Dilimler birbirinden de ayrışmalı: efsane ile grafiği eşleştirmek
        // ancak o zaman mümkün. Eşik düşük tutuldu çünkü RGB uzaklığı
        // algısal farkın kaba bir vekili — koyu kırmızı ile koyu turuncu
        // sayıca yakın çıkar ama gözle ayrılır. Bu kapı yakın kopyaları
        // yakalar, ince ayrımı okunurluk kontrast kapısına bırakır.
        for (var i = 0; i < colors.categorySlices.length; i++) {
          for (var j = i + 1; j < colors.categorySlices.length; j++) {
            expect(
              _distance(colors.categorySlices[i], colors.categorySlices[j]),
              greaterThan(0.15),
              reason:
                  'Dilim $i ile $j birbirinden ayrışmıyor: '
                  '${colors.categorySlices[i]} / ${colors.categorySlices[j]}',
            );
          }
        }
        for (final slice in colors.categorySlices) {
          // Burada eskiden "hiçbir dilim yeşile kayamaz" kapısı vardı ve
          // paletten yeşili tümüyle dışarıda tutuyordu. Kaldırıldı: turuncu
          // ve altın açıldıkça kahverengiye kaçtığı için palet yeşil ve
          // maviye geçti. Yani "yeşil yalnız gelirdir" kategori paletinde
          // bilerek gevşetildi.
          //
          // Gevşeyen şey hue yasağı; koruma değil. Korumayı aşağıdaki uzaklık
          // kapısı sürdürüyor: kategori yeşili gelir yeşilinden, kategori
          // mavisi nötr maviden ölçülebilir biçimde uzak kalmak zorunda. Ton
          // ailesi ayrı olduğu sürece (misket yeşili / orman yeşili, teal /
          // mavi) ikisi karışmıyor; asıl güvence ise renk değil, efsanedeki
          // ikon ve metin.
          //
          // Rol renklerinden yeterince uzak olmalı. Kanal baskınlığına
          // bakmak fazla kaba bir ölçüydü: mor kanal olarak mavi baskındır
          // ama algıda nötr maviyle karışmaz.
          for (final role in [
            colors.income,
            colors.incomeFill,
            colors.neutral,
            colors.neutralFill,
          ]) {
            expect(
              _distance(slice, role),
              greaterThan(0.25),
              reason: 'Kategori dilimi $slice rol rengi $role ile karışıyor.',
            );
          }
        }
      }
    });

    test('"Diğer" dilimi paletin dışında ve nötr kalır', () {
      // O dilim bir kategori değil kalanın toplamı; adı olan kategorilerle
      // renk yarışına girmemeli.
      for (final colors in [AppFinanceColors.light, AppFinanceColors.dark]) {
        expect(
          colors.categorySlices,
          isNot(contains(colors.categoryOtherSlice)),
        );
        final other = colors.categoryOtherSlice;
        final channels = [other.r, other.g, other.b];
        expect(
          channels.reduce((a, b) => a > b ? a : b) -
              channels.reduce((a, b) => a < b ? a : b),
          lessThan(0.1),
          reason: '"Diğer" dilimi gri kalmalı, bir hue benimsememeli.',
        );
      }
    });

    test('dilim indeksi palet uzunluğunu aştığında başa döner', () {
      final colors = AppFinanceColors.light;
      expect(
        colors.categorySlice(colors.categorySlices.length),
        colors.categorySlices.first,
      );
    });

    test('aydınlık ve karanlık palet aynı değildir', () {
      // Aynı paleti iki temada kullanmak, karanlık temada okunmayan koyu
      // tonlar bırakır. Bu testin amacı o regresyonu yakalamaktır.
      expect(
        AppFinanceColors.light.income,
        isNot(AppFinanceColors.dark.income),
      );
      expect(
        AppFinanceColors.light.expense,
        isNot(AppFinanceColors.dark.expense),
      );
    });

    test('lerp uç noktaları korur', () {
      expect(
        AppFinanceColors.light.lerp(AppFinanceColors.dark, 0).income,
        AppFinanceColors.light.income,
      );
      expect(
        AppFinanceColors.light.lerp(AppFinanceColors.dark, 1).income,
        AppFinanceColors.dark.income,
      );
      expect(
        AppFinanceColors.light.lerp(null, 0.5),
        same(AppFinanceColors.light),
      );
    });

    test('copyWith yalnız verilen alanı değiştirir', () {
      final updated = AppFinanceColors.light.copyWith(
        income: const Color(0xFF123456),
      );
      expect(updated.income, const Color(0xFF123456));
      expect(updated.expense, AppFinanceColors.light.expense);
    });

    test('tema uzantısı her iki temada da kayıtlıdır', () {
      expect(AppTheme.light().extension<AppFinanceColors>(), isNotNull);
      expect(AppTheme.dark().extension<AppFinanceColors>(), isNotNull);
    });

    testWidgets('of() uzantı yoksa sessizce varsayılana düşmez', (
      tester,
    ) async {
      late BuildContext capturedContext;
      await tester.pumpWidget(
        MaterialApp(
          theme: ThemeData(useMaterial3: true),
          home: Builder(
            builder: (context) {
              capturedContext = context;
              return const SizedBox.shrink();
            },
          ),
        ),
      );

      expect(
        () => AppFinanceColors.of(capturedContext),
        throwsA(isA<FlutterError>()),
      );
    });
  });
}

/// İki rengin RGB uzaklığı; renklerin ayrışıp ayrışmadığının kaba ama yeterli
/// ölçüsü.
double _distance(Color a, Color b) {
  final dr = a.r - b.r;
  final dg = a.g - b.g;
  final db = a.b - b.b;
  return math.sqrt(dr * dr + dg * dg + db * db);
}
