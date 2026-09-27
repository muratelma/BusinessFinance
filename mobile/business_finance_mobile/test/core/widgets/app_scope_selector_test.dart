import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_scope_selector.dart';

import '../../helpers/accessibility.dart';

void main() {
  group('AppScopeSwitch', () {
    testWidgets('üç konumu da gösterir', (tester) async {
      await _pumpSwitch(tester, value: null);

      expect(find.text('Hepsi'), findsOneWidget);
      expect(find.text('İşletme'), findsOneWidget);
      expect(find.text('Şahsi'), findsOneWidget);
    });

    testWidgets('dokunulan konumu bildirir', (tester) async {
      final selected = <TransactionScope?>[];
      await _pumpSwitch(tester, value: null, onChanged: selected.add);

      await tester.tap(find.text('İşletme'));
      await tester.pumpAndSettle();

      expect(selected, [TransactionScope.business]);
    });

    testWidgets('seçili konum yalnız renkle değil işaretle bildirilir', (
      tester,
    ) async {
      await _pumpSwitch(tester, value: TransactionScope.personal);

      // Seçili dilim beyaz yüzey ve güçlü kenarla ayrılır; ekran okuyucuya
      // da `seçili` olarak söylenir — renk körlüğünde dolgu farkı tek
      // başına yetmez.
      expect(
        tester.getSemantics(find.bySemanticsLabel('Şahsi')),
        matchesSemantics(
          label: 'Şahsi',
          isButton: true,
          hasSelectedState: true,
          isSelected: true,
          hasTapAction: true,
        ),
      );
    });

    testWidgets('erişilebilirlik kapısını geçer', (tester) async {
      await _pumpSwitch(tester, value: TransactionScope.business);

      await expectMeetsAccessibility(tester);
    });

    testWidgets('en büyük yazı ölçeğinde taşmaz', (tester) async {
      await pumpAtLargestTextScale(
        tester,
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(body: AppScopeSwitch(value: null, onChanged: (_) {})),
        ),
      );

      expectNoOverflow(tester);
    });
  });

  group('AppScopeField', () {
    testWidgets('iki kapsamı sunar ve değeri işaretler', (tester) async {
      await _pumpField(tester, value: TransactionScope.business);

      final chip = tester.widget<ChoiceChip>(
        find.ancestor(
          of: find.text('İşletme'),
          matching: find.byType(ChoiceChip),
        ),
      );
      expect(chip.selected, isTrue);
      expect(find.text('Hepsi'), findsNothing);
    });

    testWidgets('değerin nereden geldiğini yazar', (tester) async {
      await _pumpField(
        tester,
        value: TransactionScope.personal,
        helperText: 'Kategorinin varsayılanından geldi',
      );

      expect(find.text('Kategorinin varsayılanından geldi'), findsOneWidget);
    });

    testWidgets('hata metni yardımcı metnin yerine geçer', (tester) async {
      await _pumpField(
        tester,
        value: null,
        helperText: 'Yardımcı',
        errorText: 'Bu kayıt için kapsam seçin.',
      );

      expect(find.text('Bu kayıt için kapsam seçin.'), findsOneWidget);
      expect(find.text('Yardımcı'), findsNothing);
    });
  });
}

Future<void> _pumpSwitch(
  WidgetTester tester, {
  required TransactionScope? value,
  ValueChanged<TransactionScope?>? onChanged,
}) => tester.pumpWidget(
  MaterialApp(
    theme: AppTheme.light(),
    home: Scaffold(
      body: AppScopeSwitch(value: value, onChanged: onChanged ?? (_) {}),
    ),
  ),
);

Future<void> _pumpField(
  WidgetTester tester, {
  required TransactionScope? value,
  String? helperText,
  String? errorText,
  ValueChanged<TransactionScope>? onChanged,
}) => tester.pumpWidget(
  MaterialApp(
    theme: AppTheme.light(),
    home: Scaffold(
      body: AppScopeField(
        value: value,
        helperText: helperText,
        errorText: errorText,
        onChanged: onChanged ?? (_) {},
      ),
    ),
  ),
);
