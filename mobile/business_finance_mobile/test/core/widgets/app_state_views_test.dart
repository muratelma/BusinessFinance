import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/widgets/app_state_views.dart';

void main() {
  testWidgets('loading view exposes one live semantic message', (tester) async {
    final semantics = tester.ensureSemantics();

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: AppLoadingView(message: 'Hesaplar yükleniyor'),
      ),
    );

    expect(find.bySemanticsLabel('Hesaplar yükleniyor'), findsOneWidget);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    semantics.dispose();
  });

  testWidgets('error view runs retry action', (tester) async {
    var retryCount = 0;

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: AppErrorView(
          message: 'Sunucuya ulaşılamadı.',
          onRetry: () => retryCount++,
        ),
      ),
    );

    await tester.tap(find.text('Tekrar dene'));

    expect(retryCount, 1);
  });

  testWidgets('empty view remains scrollable with large text', (tester) async {
    tester.platformDispatcher.textScaleFactorTestValue = 2;
    addTearDown(tester.platformDispatcher.clearTextScaleFactorTestValue);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: SizedBox(
          height: 240,
          child: AppEmptyView(
            title: 'Henüz işlem yok',
            message: 'Gelir ve gider hareketleriniz burada listelenecek.',
            icon: Icons.receipt_long_outlined,
          ),
        ),
      ),
    );

    expect(tester.takeException(), isNull);
    expect(find.byType(SingleChildScrollView), findsOneWidget);
  });
}
