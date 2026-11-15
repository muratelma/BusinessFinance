import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/cards/data/card_page_models.dart';
import 'package:business_finance_mobile/features/cards/presentation/card_movements.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Ekstreye göre tarih aralığı: kullanıcı iki ekstreye yayılan bir aralık
/// seçebilmeli (13 Kasım 2026: 10 Ekim'i seçip sonraki ekstreye geçince
/// seçim siliniyor, Kasım ekstresi seçiliyordu).
void main() {
  const statements = [
    CardStatementRange(
      year: 2026,
      month: 10,
      start: '2026-09-16',
      end: '2026-10-15',
    ),
    CardStatementRange(
      year: 2026,
      month: 11,
      start: '2026-10-16',
      end: '2026-11-15',
    ),
  ];

  CardPeriodChoice? result;

  Future<void> open(WidgetTester tester, CardPeriodChoice initial) async {
    result = null;
    tester.view.physicalSize = const Size(412, 915);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Builder(
          builder: (context) => Scaffold(
            body: Center(
              child: TextButton(
                onPressed: () async {
                  result = await showDialog<CardPeriodChoice>(
                    context: context,
                    builder: (_) => CardStatementRangeDialog(
                      statements: statements,
                      initial: initial,
                    ),
                  );
                },
                child: const Text('aç'),
              ),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();
  }

  Finder day(String isoLabel) => find.bySemanticsLabel(isoLabel);

  testWidgets('özel aralıkta oklar seçimi silmez; aralık iki ekstreye '
      'yayılabilir', (tester) async {
    await open(tester, const CardPeriodChoice.statement(2026, 10));
    expect(find.text('Ekim ekstresi'), findsOneWidget);

    // Ekstre modunda bir güne dokunmak özel aralığa geçer.
    await tester.tap(day('10 Ekim 2026'));
    await tester.pumpAndSettle();
    expect(find.text('10 Eki – …'), findsOneWidget);

    await tester.tap(find.byTooltip('Sonraki ekstre'));
    await tester.pumpAndSettle();
    // Sayfa çevrildi (başlığın altı); başlangıç yerinde (başlık).
    expect(find.text('Kasım ekstresi'), findsOneWidget);
    expect(find.text('10 Eki – …'), findsOneWidget);

    await tester.tap(day('10 Kasım 2026'));
    await tester.pumpAndSettle();
    expect(find.text('10 Eki – 10 Kas'), findsOneWidget);

    await tester.tap(find.text('Seç'));
    await tester.pumpAndSettle();
    expect(result?.from, '2026-10-10');
    expect(result?.to, '2026-11-10');
  });

  testWidgets('Ekstre modu görünen ekstrenin tamamını seçer', (tester) async {
    await open(tester, const CardPeriodChoice.statement(2026, 10));
    await tester.tap(day('10 Ekim 2026'));
    await tester.pumpAndSettle();
    await tester.tap(find.byTooltip('Sonraki ekstre'));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Ekstre'));
    await tester.pumpAndSettle();
    // Başlık ekstreyi, altı aralığını yazar.
    expect(find.text('Kasım ekstresi'), findsOneWidget);
    expect(find.text('16 Eki – 15 Kas'), findsOneWidget);
    await tester.tap(find.text('Seç'));
    await tester.pumpAndSettle();
    expect(result?.isCurrent, isTrue);
  });

  testWidgets('Özel aralık boş başlar', (tester) async {
    await open(tester, const CardPeriodChoice.statement(2026, 10));
    await tester.tap(find.text('Özel aralık'));
    await tester.pumpAndSettle();
    // Başlık (ve seçici) `Özel aralık` yazar; ipucu cümlesi yok.
    expect(find.text('Özel aralık'), findsNWidgets(2));
    expect(find.text('Ekim ekstresi'), findsOneWidget);
    final select = tester.widget<FilledButton>(
      find.widgetWithText(FilledButton, 'Seç'),
    );
    expect(select.onPressed, isNull);
  });

  testWidgets('ekstre modunda oklar ekstreyi seçer', (tester) async {
    await open(tester, const CardPeriodChoice.statement(2026, 10));
    await tester.tap(find.byTooltip('Sonraki ekstre'));
    await tester.pumpAndSettle();
    expect(find.text('Kasım ekstresi'), findsOneWidget);
    expect(find.text('16 Eki – 15 Kas'), findsOneWidget);
  });
}
