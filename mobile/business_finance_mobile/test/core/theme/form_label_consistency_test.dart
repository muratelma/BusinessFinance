import 'dart:io';

import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_account_field.dart';
import 'package:business_finance_mobile/core/widgets/app_category_field.dart';
import 'package:business_finance_mobile/core/widgets/app_date_field.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter_test/flutter_test.dart';

/// Form alanlarının adı her alan türünde aynı boyda çizilir (teslim A,
/// 28 Ekim 2026): boş alanda 16, kenara kaymış hâlde 12. Metin alanı, açılır
/// liste, tarih, hesap ve kategori alanı ayrı bileşenlerdir; biri kendi
/// boyunu getirirse aynı formda iki farklı ad boyu görünür.
void main() {
  testWidgets('boş alanın adı her alan türünde 16', (tester) async {
    await _pump(tester, filled: false);

    for (final label in _labels) {
      expect(_shownSize(tester, label), closeTo(16, 0.1), reason: label);
    }
  });

  testWidgets('kenara kaymış ad her alan türünde 12', (tester) async {
    await _pump(tester, filled: true);

    for (final label in _labels) {
      expect(_shownSize(tester, label), closeTo(12, 0.1), reason: label);
    }
  });

  test('form alanlarının adı yalnız temadan gelir', () {
    // Bir alan kendi `labelStyle`'ını taşırsa formda ayrı boyda görünür.
    final offenders = <String>[];
    for (final entity in Directory('lib').listSync(recursive: true)) {
      if (entity is! File || !entity.path.endsWith('.dart')) continue;
      final path = entity.path.replaceAll(r'\', '/');
      if (path.endsWith('core/theme/app_theme.dart')) continue;
      final lines = entity.readAsLinesSync();
      for (var i = 0; i < lines.length; i++) {
        if (RegExp(r'\b(floatingLabelStyle|labelStyle):').hasMatch(lines[i]) &&
            !path.endsWith('core/widgets/app_trend_chart.dart')) {
          offenders.add('$path:${i + 1}');
        }
      }
    }
    expect(offenders, isEmpty);
  });
}

const _labels = ['Metin', 'Liste', 'Tarih', 'Hesap', 'Kategori'];

/// Ekranda görünen yazı boyu: yazının stili ve kenara kayarken uygulanan
/// küçültme birlikte.
double _shownSize(WidgetTester tester, String text) {
  final finder = find.text(text);
  final paragraph = tester.renderObject<RenderParagraph>(
    find
            .descendant(of: finder, matching: find.byType(RichText))
            .evaluate()
            .isEmpty
        ? finder
        : find.descendant(of: finder, matching: find.byType(RichText)),
  );
  double? size;
  paragraph.text.visitChildren((span) {
    size ??= span.style?.fontSize;
    return size == null;
  });
  final scale = tester.getRect(finder).width / paragraph.size.width;
  return (size ?? paragraph.text.style!.fontSize!) * scale;
}

Future<void> _pump(WidgetTester tester, {required bool filled}) async {
  tester.view.physicalSize = const Size(412, 1000);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: Scaffold(
        body: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            TextFormField(
              key: ValueKey(filled),
              initialValue: filled ? '1.250' : null,
              decoration: const InputDecoration(labelText: 'Metin'),
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<String>(
              key: ValueKey('liste$filled'),
              initialValue: filled ? 'a' : null,
              decoration: const InputDecoration(labelText: 'Liste'),
              items: const [DropdownMenuItem(value: 'a', child: Text('A'))],
              onChanged: (_) {},
            ),
            const SizedBox(height: 16),
            if (filled)
              AppDateField(
                label: 'Tarih',
                value: '2026-10-28',
                onChanged: (_) {},
              )
            else
              const TextField(decoration: InputDecoration(labelText: 'Tarih')),
            const SizedBox(height: 16),
            AppAccountField(
              label: 'Hesap',
              value: filled ? 'k' : null,
              options: const [
                AppAccountOption(
                  id: 'k',
                  name: 'Kasa',
                  kind: AppAccountKind.cash,
                ),
              ],
              onChanged: (_) {},
            ),
            const SizedBox(height: 16),
            AppCategoryField(
              value: filled
                  ? const AppCategoryValue('m', TransactionScope.personal)
                  : null,
              options: const [
                AppCategoryOption(
                  id: 'm',
                  name: 'Market',
                  scope: TransactionScope.personal,
                ),
              ],
              onChanged: (_) {},
            ),
          ],
        ),
      ),
    ),
  );
  await tester.pumpAndSettle();
}
