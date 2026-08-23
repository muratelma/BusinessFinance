import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/auth/presentation/register_page.dart';
import 'package:business_finance_mobile/features/more/presentation/more_page.dart';
import 'package:business_finance_mobile/features/profile/data/profile_repository.dart';

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

  group('ayarlar', () {
    testWidgets('cevap değiştirilebilir ve kapsam boyutu ona uyar', (
      tester,
    ) async {
      final repository = _FakeProfileRepository();
      final scope = await _scopeController(hasBusiness: false);
      await _pumpMore(tester, repository: repository, scope: scope);
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
      await _pumpMore(tester, repository: repository, scope: scope);
      await _revealBusinessAnswer(tester);

      await tester.tap(find.text('İşletmem var'));
      await tester.pumpAndSettle();

      expect(scope.isVisible, isFalse);
      expect(find.text('Profil güncellenemedi.'), findsOneWidget);
    });

    testWidgets('bağlanmamış kabukta çalışmayan anahtar gösterilmez', (
      tester,
    ) async {
      await tester.pumpWidget(_moreHost(const MorePage()));
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

/// Menü test ekranına sığmıyor: kapı sayısı arttıkça onboarding cevabı
/// katlanmanın altına iniyor. Kaydırmadan dokunmak, cevabın kaybolduğunu
/// değil ekranın küçük olduğunu ölçerdi.
Future<void> _revealBusinessAnswer(WidgetTester tester) async {
  await tester.dragUntilVisible(
    find.text('İşletmem var'),
    find.byType(ListView),
    const Offset(0, -120),
  );
  await tester.pumpAndSettle();
}

Future<void> _pumpMore(
  WidgetTester tester, {
  required _FakeProfileRepository repository,
  required ScopeController scope,
}) async {
  await tester.pumpWidget(
    _moreHost(
      MultiProvider(
        providers: [
          ChangeNotifierProvider.value(value: scope),
          Provider<ProfileRepositoryContract>.value(value: repository),
        ],
        child: const MorePage(),
      ),
    ),
  );
  await tester.pumpAndSettle();
}

Widget _moreHost(Widget child) => MaterialApp.router(
  theme: AppTheme.light(),
  routerConfig: GoRouter(
    routes: [
      GoRoute(path: '/', builder: (_, _) => child),
      for (final path in [
        '/more/accounts',
        '/more/cards',
        '/more/categories',
        '/more/debts',
        '/more/goals',
        '/more/planning',
        '/more/data-tools',
      ])
        GoRoute(path: path, builder: (_, _) => const SizedBox.shrink()),
    ],
  ),
);

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
