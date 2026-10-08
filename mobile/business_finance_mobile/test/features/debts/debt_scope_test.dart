import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/features/debts/data/debt_models.dart';
import 'package:business_finance_mobile/features/debts/data/debt_repository.dart';
import 'package:business_finance_mobile/features/debts/presentation/debts_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

/// Aşama 06 Grup 6 cihaz kabul turu: borç/alacak formunda kapsam alanı yoktu.
///
/// `DebtAgreement` gelir/gider raporunu etkiler ve kapsam taşımak zorundadır.
/// Kaynağı ve kategorisi kapsam taşımayan kullanıcıda sunucu isteği
/// `*.scope_unresolved` ile reddediyor, dekonttan gelen borç planı hiç
/// kurulamıyordu.
void main() {
  testWidgets('kapsamı görmeyen kullanıcıda alan çıkmaz ve scope gitmez', (
    tester,
  ) async {
    _tallWindow(tester);
    final repository = _FakeDebtRepository();
    await tester.pumpWidget(_host(repository, hasBusiness: false));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();

    expect(find.text('Kapsam'), findsNothing);

    await _fillValidDebt(tester);
    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    expect(repository.lastCreateInput!['scope'], isNull);
  });

  testWidgets('kapsamı gören kullanıcıda seçim istekle birlikte gider', (
    tester,
  ) async {
    _tallWindow(tester);
    final repository = _FakeDebtRepository();
    await tester.pumpWidget(_host(repository, hasBusiness: true));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();

    expect(find.text('Kapsam'), findsOneWidget);
    await _fillValidDebt(tester);
    await tester.tap(find.widgetWithText(ChoiceChip, 'İşletme'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    expect(repository.lastCreateInput!['scope'], 'business');
  });

  testWidgets('zincir çözülemezse istek gitmeden alanın yanında söylenir', (
    tester,
  ) async {
    // Sunucu kapsam uydurmaz; istemci de uydurmaz. Reddi ekranda ham sunucu
    // metniyle göstermek yerine soruyu formda soruyoruz.
    _tallWindow(tester);
    final repository = _FakeDebtRepository();
    await tester.pumpWidget(_host(repository, hasBusiness: true));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();
    await _fillValidDebt(tester);

    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    expect(repository.lastCreateInput, isNull);
    expect(find.textContaining('bu kayıt için seçin'), findsOneWidget);
  });

  testWidgets('hesabın varsayılan kapsamı alana dolu gelir', (tester) async {
    _tallWindow(tester);
    final repository = _FakeDebtRepository(
      accountScope: TransactionScope.business,
    );
    await tester.pumpWidget(_host(repository, hasBusiness: true));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Borç / alacak ekle'));
    await tester.pumpAndSettle();
    await _fillValidDebt(tester);

    expect(find.textContaining('etiketinden geldi'), findsOneWidget);

    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    expect(repository.lastCreateInput!['scope'], 'business');
  });
}

Future<void> _fillValidDebt(WidgetTester tester) async {
  await tester.enterText(
    find.widgetWithText(TextFormField, 'Kişi / kurum'),
    'Kemal Demir',
  );
  await tester.enterText(find.widgetWithText(TextFormField, 'Anapara'), '1500');
  await tester.enterText(
    find.widgetWithText(TextFormField, 'Toplam geri ödeme'),
    '1500',
  );
  await tester.pumpAndSettle();
}

/// Panel uzun: kısa pencerede kapsam çipleri ile `Oluştur` aynı anda
/// kurulmuyor ve dokunuş boşa gidiyor. Ölçülen şey davranış, pencerenin boyu
/// değil.
void _tallWindow(WidgetTester tester) {
  tester.view.physicalSize = const Size(1200, 2600);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
}

Widget _host(DebtRepositoryContract repository, {required bool hasBusiness}) {
  final controller = ScopeController(
    store: _FakeScopeStore(hasBusiness: hasBusiness),
    readHasBusiness: () async => hasBusiness,
  )..ensureLoaded();

  return ChangeNotifierProvider<ScopeController>.value(
    value: controller,
    child: MaterialApp(
      theme: AppTheme.light(),
      home: DebtsPage(repository: repository),
    ),
  );
}

class _FakeScopeStore implements ScopeStore {
  _FakeScopeStore({required this.hasBusiness});

  final bool hasBusiness;

  @override
  Future<TransactionScope?> readScope() async => null;

  @override
  Future<void> writeScope(TransactionScope? scope) async {}

  @override
  Future<bool?> readHasBusiness() async => hasBusiness;

  @override
  Future<void> writeHasBusiness(bool value) async {}

  @override
  Future<void> clear() async {}
}

class _FakeDebtRepository implements DebtRepositoryContract {
  _FakeDebtRepository({this.accountScope});

  /// Hesabın varsayılan kapsamı: zincirin orta halkası (Aşama 06 Grup 4).
  final TransactionScope? accountScope;
  Map<String, Object?>? lastCreateInput;

  @override
  Future<DebtsSnapshot> load(String asOfDate) async => DebtsSnapshot(
    accounts: [
      DataChoice('account-1', 'Kasa', type: 'bank', defaultScope: accountScope),
    ],
    categories: const [DataChoice('category-1', 'Market', type: 'expense')],
    debts: const [],
  );

  @override
  Future<void> create(Map<String, Object?> input) async =>
      lastCreateInput = input;

  @override
  Future<void> recordOpening(String debtId, Map<String, Object?> input) async {}

  @override
  Future<void> pay(
    String debtId,
    int sequence,
    Map<String, Object?> input,
  ) async {}
}
