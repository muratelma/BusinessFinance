import 'dart:convert';

import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/auth/data/google_id_token_source.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_controller.dart';
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
  Future<Widget> page(
    WidgetTester tester, [
    AccountRepositoryContract? repository,
  ]) async {
    final scope = ScopeController(readHasBusiness: () async => true);
    await tester.runAsync(scope.ensureLoaded);
    return MultiProvider(
      providers: [
        ChangeNotifierProvider<ScopeController?>.value(value: scope),
        Provider<ProfileRepositoryContract?>.value(value: _Profile()),
      ],
      child: AccountPage(
        repository: repository ?? _DesignAccount(),
        authRepository: FakeAuthSessionRepository()
          ..session = testSession(sessionId: 'current'),
        now: () => designToday,
      ),
    );
  }

  // 10 Kasım 2026: doğrulama panelinin üç hâli.
  Future<void> verifySheet(
    WidgetTester tester,
    String name,
    AccountRepositoryContract repository, {
    String? typed,
  }) async {
    await captureScreen(
      tester,
      name,
      await page(tester, repository),
      withNavBar: false,
      pushed: true,
      before: (tester) async {
        await tester.tap(find.text('E-postanızı doğrulayın'));
        await tester.pump();
        await tester.pump(const Duration(milliseconds: 500));
        await tester.pump(const Duration(seconds: 15));
        if (typed != null) {
          await tester.enterText(find.byType(TextField).last, typed);
          await tester.pump();
          await tester.pump(const Duration(milliseconds: 300));
        }
      },
    );
  }

  testWidgets('22 doğrulama · kod gönderildi', (tester) async {
    await verifySheet(tester, '22-dogrulama-gonderildi', _DesignAccount());
  }, skip: !screenshotsEnabled);

  testWidgets('23 doğrulama · yanlış kod', (tester) async {
    await verifySheet(
      tester,
      '23-dogrulama-yanlis-kod',
      _DesignAccount(),
      typed: '482913',
    );
  }, skip: !screenshotsEnabled);

  testWidgets('24 doğrulama · az önce gönderildi', (tester) async {
    await verifySheet(
      tester,
      '24-dogrulama-az-once',
      _DesignAccount(tooSoon: true),
    );
  }, skip: !screenshotsEnabled);

  // 10 Kasım 2026: kimlik doğrulama (B). Silme sayfasının üç hesap türü,
  // Google ile doğrulanmış hâl ve parola panelleri.
  Future<void> deleteShot(
    WidgetTester tester,
    String name,
    _VerifiedAccount account, {
    bool verify = false,
  }) async {
    final auth = AuthController(
      FakeAuthSessionRepository()..session = testSession(sessionId: 'current'),
      _ShotGoogle(),
    );
    await captureScreen(
      tester,
      name,
      await page(tester, account),
      wrapApp: (app) => ChangeNotifierProvider<AuthController?>.value(
        value: auth,
        child: app,
      ),
      withNavBar: false,
      pushed: true,
      before: (tester) async {
        await tester.drag(find.byType(ListView), const Offset(0, -2000));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Hesabımı sil').last);
        await tester.pumpAndSettle();
        if (verify) {
          await tester.tap(find.text('Google ile doğrula'));
          await tester.pumpAndSettle();
        }
      },
    );
  }

  testWidgets('25 sil · yalnız parola', (tester) async {
    await deleteShot(tester, '25-sil-parola', _VerifiedAccount());
  }, skip: !screenshotsEnabled);

  testWidgets('26 sil · yalnız Google', (tester) async {
    await deleteShot(
      tester,
      '26-sil-google',
      _VerifiedAccount(hasPassword: false, hasGoogle: true),
    );
  }, skip: !screenshotsEnabled);

  testWidgets('27 sil · parola ya da Google', (tester) async {
    await deleteShot(tester, '27-sil-ikisi', _VerifiedAccount(hasGoogle: true));
  }, skip: !screenshotsEnabled);

  testWidgets('28 sil · Google ile doğrulandı', (tester) async {
    await deleteShot(
      tester,
      '28-sil-dogrulandi',
      _VerifiedAccount(hasGoogle: true),
      verify: true,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('29 parolamı unuttum', (tester) async {
    await captureScreen(
      tester,
      '29-parolami-unuttum',
      await page(tester, _VerifiedAccount()),
      withNavBar: false,
      pushed: true,
      before: (tester) async {
        await tester.tap(find.text('Parolamı değiştir'));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Parolamı unuttum'));
        await tester.pump();
        await tester.pump(const Duration(milliseconds: 500));
        await tester.pump(const Duration(seconds: 15));
      },
    );
  }, skip: !screenshotsEnabled);

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

  // 9 Kasım 2026 gözden geçirmesi: kullanıcı adı, doğrulanmış adres.
  testWidgets('19 hesabım · doğrulanmış', (tester) async {
    await captureScreen(
      tester,
      '19-hesabim-dogrulanmis',
      await page(tester, _VerifiedAccount()),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('20 parolamı değiştir', (tester) async {
    await captureScreen(
      tester,
      '20-parolami-degistir',
      await page(tester, _VerifiedAccount()),
      withNavBar: false,
      pushed: true,
      before: (tester) async {
        await tester.tap(find.text('Parolamı değiştir'));
        await tester.pumpAndSettle();
        await tester.enterText(
          find.widgetWithText(TextField, 'Yeni parola'),
          'Kasim2026',
        );
        await tester.pump();
      },
    );
  }, skip: !screenshotsEnabled);

  testWidgets('21 parola belirle', (tester) async {
    await captureScreen(
      tester,
      '21-parola-belirle',
      await page(tester, _VerifiedAccount(hasPassword: false)),
      withNavBar: false,
      pushed: true,
      before: (tester) async {
        await tester.tap(find.text('Parola belirle'));
        await tester.pumpAndSettle();
      },
    );
  }, skip: !screenshotsEnabled);
}

class _ShotGoogle implements GoogleIdTokenSource {
  @override
  Future<String?> obtainIdToken() async {
    String part(Object value) =>
        base64Url.encode(utf8.encode(jsonEncode(value))).replaceAll('=', '');
    return '${part({'alg': 'none'})}.'
        '${part({'email': 'murat@ornek.com'})}.imza';
  }

  @override
  Future<void> signOut() async {}
}

class _VerifiedAccount extends Fake implements AccountRepositoryContract {
  _VerifiedAccount({this.hasPassword = true, this.hasGoogle = false});

  final bool hasPassword;
  final bool hasGoogle;

  @override
  Future<UserAccount> read() async => UserAccount(
    userId: 'u1',
    email: 'murat@ornek.com',
    username: 'murat_elma',
    emailConfirmed: true,
    createdAtUtc: DateTime.utc(2026, 3, 14),
    activeSessionCount: 1,
    hasPassword: hasPassword,
    hasGoogle: hasGoogle,
  );

  @override
  Future<List<UserSessionSummary>> listSessions() async => [
    UserSessionSummary(
      sessionId: 'current',
      createdAtUtc: DateTime(2026, 9, 25, 9).toUtc(),
      expiresAtUtc: DateTime(2026, 10, 22, 9).toUtc(),
    ),
  ];

  @override
  Future<bool> requestPasswordCode() async => true;
}

class _DesignAccount extends DesignAccount
    implements AccountRepositoryContract {
  _DesignAccount({this.tooSoon = false});

  final bool tooSoon;

  @override
  Future<VerificationSendResult> sendEmailVerification() async {
    if (tooSoon) {
      throw const ApiException(
        code: 'account.verification_code_too_soon',
        message: 'Az önce bir kod gönderildi.',
      );
    }
    return const VerificationSendResult(
      alreadyConfirmed: false,
      codeSent: true,
    );
  }

  @override
  Future<void> confirmEmail(String code) async => throw const ApiException(
    code: 'account.invalid_verification_code',
    message: 'Doğrulama kodu geçersiz.',
  );

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
