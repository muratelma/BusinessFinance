import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/features/account/data/account_models.dart';
import 'package:business_finance_mobile/features/account/data/account_repository.dart';
import 'package:business_finance_mobile/features/account/presentation/account_page.dart';
import 'package:business_finance_mobile/features/profile/data/profile_repository.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import '../helpers/fake_auth.dart';
import 'design_sample_data.dart';
import 'screenshot_harness.dart';

/// Tasarımın `HesabimV5` verisi: doğrulanmamış adres, işletme açık, üç oturum.
void main() {
  Future<Widget> page(WidgetTester tester) async {
    final scope = ScopeController(readHasBusiness: () async => true);
    await tester.runAsync(scope.ensureLoaded);
    return MultiProvider(
      providers: [
        ChangeNotifierProvider<ScopeController?>.value(value: scope),
        Provider<ProfileRepositoryContract?>.value(value: _Profile()),
      ],
      child: AccountPage(
        repository: _DesignAccount(),
        authRepository: FakeAuthSessionRepository()
          ..session = testSession(sessionId: 'current'),
        now: () => designToday,
      ),
    );
  }

  testWidgets('17 hesabım', (tester) async {
    await captureScreen(
      tester,
      '17-hesabim',
      await page(tester),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('18 hesabım · alt', (tester) async {
    await captureScreen(
      tester,
      '18-hesabim-alt',
      await page(tester),
      withNavBar: false,
      pushed: true,
      before: (tester) async {
        await tester.drag(find.byType(ListView), const Offset(0, -2000));
      },
    );
  }, skip: !screenshotsEnabled);
}

class _DesignAccount extends DesignAccount
    implements AccountRepositoryContract {
  @override
  Future<List<UserSessionSummary>> listSessions() async => [
    UserSessionSummary(
      sessionId: 'current',
      createdAtUtc: DateTime(2026, 9, 25, 9).toUtc(),
      expiresAtUtc: DateTime(2026, 10, 22, 9).toUtc(),
    ),
    UserSessionSummary(
      sessionId: 'b',
      createdAtUtc: DateTime(2026, 9, 18, 9).toUtc(),
      expiresAtUtc: DateTime(2026, 10, 18, 9).toUtc(),
    ),
    UserSessionSummary(
      sessionId: 'c',
      createdAtUtc: DateTime(2026, 9, 2, 9).toUtc(),
      expiresAtUtc: DateTime(2026, 10, 2, 9).toUtc(),
    ),
  ];
}

class _Profile extends Fake implements ProfileRepositoryContract {}
