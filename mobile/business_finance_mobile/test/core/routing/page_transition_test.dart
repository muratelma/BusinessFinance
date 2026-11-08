import 'dart:async';

import 'package:business_finance_mobile/core/routing/app_router.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/accounts/presentation/account_directory.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Üstüne açılan sayfa iPhone'da soldan kenarından çekilerek kapanır; bu
/// hareket yalnız kayarak açılan sayfada vardır. Bütün sayfalar geçişsizken
/// iPhone'da kenardan çekerek geri dönülemiyordu (7 Kasım 2026).
/// Test ortamında hesap servisi yok; sayfa kendi hata ekranını açar. Önemli
/// olan sayfanın kendisi değil, nasıl açılıp kapandığıdır.
final _pushed = find.text('Hesap servisi yapılandırılmadı.');

final _page = find.ancestor(of: _pushed, matching: find.byType(Scaffold)).first;

void main() {
  Future<void> openForm(WidgetTester tester, TargetPlatform platform) async {
    tester.view.physicalSize = const Size(400, 800);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final router = createAppRouter(initialLocation: '/more');
    addTearDown(router.dispose);
    await tester.pumpWidget(
      MaterialApp.router(
        theme: AppTheme.light().copyWith(platform: platform),
        routerConfig: router,
      ),
    );
    await tester.pumpAndSettle();
    unawaited(router.push(accountCreateLocation));
    await tester.pumpAndSettle();
    expect(_pushed, findsOneWidget);
  }

  testWidgets('iPhone: üstüne açılan sayfa soldan çekince kapanır', (
    tester,
  ) async {
    await openForm(tester, TargetPlatform.iOS);

    await tester.dragFrom(const Offset(2, 400), const Offset(300, 0));
    await tester.pumpAndSettle();

    expect(_pushed, findsNothing);
  });

  testWidgets('iPhone: üstüne açılan sayfa sağdan kayarak gelir', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 800);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final router = createAppRouter(initialLocation: '/more');
    addTearDown(router.dispose);
    await tester.pumpWidget(
      MaterialApp.router(
        theme: AppTheme.light().copyWith(platform: TargetPlatform.iOS),
        routerConfig: router,
      ),
    );
    await tester.pumpAndSettle();

    unawaited(router.push(accountCreateLocation));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 100));
    // Geçişin ortasında sayfa henüz ekranın solunda değildir.
    expect(tester.getTopLeft(_page).dx, greaterThan(0));
    await tester.pumpAndSettle();
    expect(tester.getTopLeft(_page).dx, 0);
  });
}
