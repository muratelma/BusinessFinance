import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_surfaces.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';

import '../../helpers/contrast.dart';

void main() {
  final themes = {'light': AppTheme.light(), 'dark': AppTheme.dark()};

  group('mürekkep merdiveni', () {
    for (final entry in themes.entries) {
      final themeName = entry.key;
      final surfaces = entry.value.extension<AppSurfaces>()!;
      final backgrounds = {
        'canvas': surfaces.canvas,
        'card': surfaces.card,
        'cardMuted': surfaces.cardMuted,
      };

      // Soluk olmak silik olmak değildir: en soluk kademe bile normal metin
      // eşiğini geçer. Aksi hâlde "soluklaştırma" okunamayan metin üretirdi.
      for (final ink in {
        'ink': surfaces.ink,
        'inkMuted': surfaces.inkMuted,
        'inkFaint': surfaces.inkFaint,
      }.entries) {
        for (final background in backgrounds.entries) {
          test(
            '$themeName: ${ink.key} ${background.key} üzerinde AA geçer',
            () {
              final ratio = contrastRatio(ink.value, background.value);
              expect(
                ratio,
                greaterThanOrEqualTo(wcagAaNormalText),
                reason:
                    '${ink.key} ${background.key} üzerinde '
                    '${ratio.toStringAsFixed(2)}:1 — en az '
                    '$wcagAaNormalText:1 olmalı.',
              );
            },
          );
        }
      }

      test('$themeName: üç kademe gerçekten ayrışır', () {
        // Üçü birbirine yakın olsaydı hiyerarşi yalnız isimde kalırdı; ekran
        // yine baştan sona aynı koyulukta okunurdu. Sıralama da korunmalı:
        // ink en güçlü, inkFaint en soluk.
        final strong = contrastRatio(surfaces.ink, surfaces.card);
        final muted = contrastRatio(surfaces.inkMuted, surfaces.card);
        final faint = contrastRatio(surfaces.inkFaint, surfaces.card);

        expect(strong, greaterThan(muted));
        expect(muted, greaterThan(faint));
        expect(
          strong / faint,
          greaterThan(2),
          reason:
              'En güçlü ve en soluk kademe arasında en az iki kat fark '
              'olmalı; yoksa üç kademe tek kademe gibi okunur.',
        );
      });

      test('$themeName: tema mürekkep rollerini paletten alır', () {
        // `ColorScheme.fromSeed`'in ürettiği tonlara geri düşülürse merdiven
        // sessizce kaybolur ve ekran yine "her yeri siyah" olur.
        final scheme = entry.value.colorScheme;
        expect(scheme.onSurface, surfaces.ink);
        expect(scheme.onSurfaceVariant, surfaces.inkMuted);
      });

      test('$themeName: bölüm etiketi en soluk kademeyi kullanır', () {
        expect(entry.value.textTheme.labelMedium?.color, surfaces.inkFaint);
        expect(entry.value.textTheme.labelSmall?.color, surfaces.inkFaint);
      });
    }
  });
}
