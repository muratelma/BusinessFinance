import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_scope_selector.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Formdaki taraf bölümü (ADR 0020): tek taraflı kategoride soru yoktur,
/// bilgi satırı vardır; iki tarafa açık kategoride iki çip çıkar.
void main() {
  Future<void> pump(WidgetTester tester, Widget child) => tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: Scaffold(body: child),
    ),
  );

  testWidgets('tek taraflı kategoride çip çizilmez, bilgi satırı çizilir', (
    tester,
  ) async {
    await pump(
      tester,
      AppScopeSection(
        category: TransactionScope.personal,
        source: TransactionScope.business,
        onChanged: (_) {},
      ),
    );

    expect(find.byType(ChoiceChip), findsNothing);
    final row = tester.widget<AppScopeInfoRow>(find.byType(AppScopeInfoRow));
    expect(row.scope, TransactionScope.personal);
    expect(row.reason, 'kategoriden');
    expect(find.bySemanticsLabel('Kapsam: Şahsi, kategoriden'), findsOneWidget);
  });

  testWidgets('iki tarafa açık kategoride çip kaynağın etiketiyle açılır', (
    tester,
  ) async {
    TransactionScope? chosen;
    await pump(
      tester,
      AppScopeSection(
        source: TransactionScope.business,
        sourceName: 'Dükkân kasası',
        onChanged: (value) => chosen = value,
      ),
    );

    expect(find.byType(AppScopeInfoRow), findsNothing);
    expect(
      tester.widget<AppScopeField>(find.byType(AppScopeField)).value,
      TransactionScope.business,
    );
    expect(
      find.text('Dükkân kasası etiketinden geldi — değiştirebilirsiniz.'),
      findsOneWidget,
    );

    await tester.tap(find.text('Şahsi'));
    expect(chosen, TransactionScope.personal);
  });

  testWidgets('hiçbir işaret yoksa seçim istenir ve hata alanın yanındadır', (
    tester,
  ) async {
    await pump(
      tester,
      AppScopeSection(
        errorText: 'Bu kayıt için kapsam seçin.',
        onChanged: (_) {},
      ),
    );

    expect(
      tester.widget<AppScopeField>(find.byType(AppScopeField)).value,
      isNull,
    );
    expect(find.text('Bu kayıt için kapsam seçin.'), findsOneWidget);
  });
}
