import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/auth/presentation/register_page.dart';
import 'package:business_finance_mobile/features/account/data/account_models.dart';
import 'package:business_finance_mobile/features/account/data/account_repository.dart';
import 'package:business_finance_mobile/features/account/presentation/account_page.dart';
import 'package:business_finance_mobile/features/profile/data/profile_repository.dart';

import '../../helpers/fake_auth.dart';

void main() {
  group('kayıt', () {
    testWidgets('onboarding sorusu varsayılan olarak kapalıdır', (
      tester,
    ) async {
      bool? sent;
      await _pumpRegister(tester, onSubmit: (value) => sent = value);

      await _fillRegistrationForm(tester);
      await tester.tap(find.text('Kayıt ol'));
      await tester.pumpAndSettle();

      expect(sent, isFalse);
    });

    testWidgets('cevap kayıt isteğiyle birlikte gider', (tester) async {
      bool? sent;
      await _pumpRegister(tester, onSubmit: (value) => sent = value);

      await _fillRegistrationForm(tester);
      await tester.tap(find.text('İşletmem var'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Kayıt ol'));
      await tester.pumpAndSettle();

      expect(sent, isTrue);
    });
  });

  // Cevap `Diğer` menüsünden `Hesabım` sayfasına taşındı: hesaba dair
  // parçalar tek yerde toplanıyor (Aşama 06 Grup 1).
  group('hesabım', () {
    testWidgets('cevap değiştirilebilir ve kapsam boyutu ona uyar', (
      tester,
    ) async {
      final repository = _FakeProfileRepository();
      final scope = await _scopeController(hasBusiness: false);
      await _pumpAccount(tester, repository: repository, scope: scope);
      await _revealBusinessAnswer(tester);

      await tester.tap(find.text('İşletmem var'));
      await tester.pumpAndSettle();

      expect(repository.written, [true]);
      expect(scope.isVisible, isTrue);
    });

    testWidgets('sunucu reddederse boyut değişmez ve sebebi söylenir', (
      tester,
    ) async {
      final repository = _FakeProfileRepository(
        error: const ApiException(
          code: 'profile.unavailable',
          message: 'Profil güncellenemedi.',
          statusCode: 500,
        ),
      );
      final scope = await _scopeController(hasBusiness: false);
      await _pumpAccount(tester, repository: repository, scope: scope);
      await _revealBusinessAnswer(tester);

      await tester.tap(find.text('İşletmem var'));
      await tester.pumpAndSettle();

      expect(scope.isVisible, isFalse);
      expect(find.text('Profil güncellenemedi.'), findsOneWidget);
    });

    testWidgets('bağlanmamış kabukta çalışmayan anahtar gösterilmez', (
      tester,
    ) async {
      await tester.pumpWidget(_host(_accountPage()));
      await tester.pumpAndSettle();

      expect(find.text('İşletmem var'), findsNothing);
    });
  });
}

Future<void> _fillRegistrationForm(WidgetTester tester) async {
  await tester.enterText(
    find.byType(TextFormField).first,
    'esnaf@example.test',
  );
  await tester.enterText(
    find.byType(TextFormField).last,
    'Valid-Password-123!',
  );
}

Future<void> _pumpRegister(
  WidgetTester tester, {
  required void Function(bool hasBusiness) onSubmit,
}) async {
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: RegisterPage(
        onSubmit: (_, _, {required hasBusiness}) async => onSubmit(hasBusiness),
        onCompleted: (_) {},
        onBackToLogin: () {},
      ),
    ),
  );
  await tester.pumpAndSettle();
}

/// Sayfa test ekranına sığmıyor: onboarding cevabı katlanmanın altına
/// inebiliyor. Kaydırmadan dokunmak, cevabın kaybolduğunu değil ekranın küçük
/// olduğunu ölçerdi.
Future<void> _revealBusinessAnswer(WidgetTester tester) async {
  await tester.dragUntilVisible(
    find.text('İşletmem var'),
    find.byType(ListView),
    const Offset(0, -120),
  );
  await tester.pumpAndSettle();
}

Future<void> _pumpAccount(
  WidgetTester tester, {
  required _FakeProfileRepository repository,
  required ScopeController scope,
}) async {
  await tester.pumpWidget(
    _host(
      MultiProvider(
        providers: [
          ChangeNotifierProvider.value(value: scope),
          Provider<ProfileRepositoryContract>.value(value: repository),
        ],
        child: _accountPage(),
      ),
    ),
  );
  await tester.pumpAndSettle();
}

Widget _accountPage() => AccountPage(
  repository: _StubAccountRepository(),
  authRepository: FakeAuthSessionRepository()..session = testSession(),
);

Widget _host(Widget child) => MaterialApp.router(
  theme: AppTheme.light(),
  routerConfig: GoRouter(
    routes: [GoRoute(path: '/', builder: (_, _) => child)],
  ),
);

/// Cevabın kendisi test konusu; hesap bilgisi yalnız sayfanın çizilebilmesi
/// için gerekiyor.
class _StubAccountRepository implements AccountRepositoryContract {
  @override
  Future<UserAccount> read() async => UserAccount(
    userId: '11111111-1111-1111-1111-111111111111',
    email: 'user@example.test',
    emailConfirmed: false,
    createdAtUtc: DateTime.utc(2026, 8, 1),
    activeSessionCount: 0,
  );

  @override
  Future<List<UserSessionSummary>> listSessions() async => const [];

  @override
  Future<void> revokeSession(String sessionId) async {}

  @override
  Future<RotatedTokens> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async => throw UnimplementedError();

  @override
  Future<void> deleteAccount({required String password}) async {}
}

Future<ScopeController> _scopeController({required bool hasBusiness}) async {
  final controller = ScopeController(
    store: _MemoryStore(hasBusiness: hasBusiness),
  );
  await controller.ensureLoaded();
  return controller;
}

class _MemoryStore implements ScopeStore {
  _MemoryStore({this.hasBusiness});

  TransactionScope? scope;
  bool? hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => scope;

  @override
  Future<void> writeScope(TransactionScope? value) async => scope = value;

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<void> writeHasBusiness(bool value) async => hasBusiness = value;

  @override
  Future<void> clear() async {
    scope = null;
    hasBusiness = null;
  }
}

class _FakeProfileRepository implements ProfileRepositoryContract {
  _FakeProfileRepository({this.error});

  final ApiException? error;
  final written = <bool>[];

  @override
  Future<UserProfile> read() async => const UserProfile(hasBusiness: false);

  @override
  Future<UserProfile> update({required bool hasBusiness}) async {
    if (error case final failure?) throw failure;
    written.add(hasBusiness);
    return UserProfile(hasBusiness: hasBusiness);
  }
}
