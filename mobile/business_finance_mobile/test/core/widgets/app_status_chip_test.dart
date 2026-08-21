import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_finance_colors.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_status_chip.dart';

void main() {
  Future<void> pump(WidgetTester tester, AppStatusChip chip) =>
      tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(body: chip),
        ),
      );

  testWidgets('her rozet ikon ve metni birlikte taşır', (tester) async {
    // Yalnız renkli bir nokta, renk körü ve ekran okuyucu kullanıcısına
    // hiçbir bilgi vermez.
    await pump(
      tester,
      const AppStatusChip(
        label: 'İptal edildi',
        icon: Icons.block,
        tone: AppStatusTone.cancelled,
      ),
    );

    expect(find.text('İptal edildi'), findsOneWidget);
    expect(find.byIcon(Icons.block), findsOneWidget);
  });

  testWidgets('ton kendi container çiftini kullanır', (tester) async {
    await pump(
      tester,
      const AppStatusChip(
        label: 'Planlandı',
        icon: Icons.schedule,
        tone: AppStatusTone.planned,
      ),
    );

    final decoration =
        tester.widget<Container>(find.byType(Container)).decoration
            as BoxDecoration;
    expect(decoration.color, AppFinanceColors.light.plannedContainer);

    final icon = tester.widget<Icon>(find.byIcon(Icons.schedule));
    expect(icon.color, AppFinanceColors.light.onPlannedContainer);
  });

  testWidgets('karanlık temada karanlık paletten okur', (tester) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.dark(),
        home: const Scaffold(
          body: AppStatusChip(
            label: 'Gecikmiş',
            icon: Icons.warning_amber,
            tone: AppStatusTone.expense,
          ),
        ),
      ),
    );

    final decoration =
        tester.widget<Container>(find.byType(Container)).decoration
            as BoxDecoration;
    expect(decoration.color, AppFinanceColors.dark.expenseContainer);
  });

  testWidgets('en büyük metin ölçeğinde taşmaz', (tester) async {
    tester.view.physicalSize = const Size(300, 600);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: const MediaQuery(
          data: MediaQueryData(textScaler: TextScaler.linear(2)),
          // Rozetin gerçek kabı Wrap veya Column'dur; ikisi de genişliği
          // sınırlar. Sınırsız genişlik veren bir Row'da her widget taşar,
          // bu rozete özgü bir kırılganlık değildir.
          child: Scaffold(
            body: Wrap(
              children: [
                AppStatusChip(
                  label: 'Tekrarlayan plandan üretildi',
                  icon: Icons.event_repeat,
                  tone: AppStatusTone.planned,
                ),
              ],
            ),
          ),
        ),
      ),
    );

    expect(tester.takeException(), isNull);
  });
}
