import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/models/data_choice.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/goals/data/goal_models.dart';
import 'package:business_finance_mobile/features/goals/data/goal_repository.dart';
import 'package:business_finance_mobile/features/goals/presentation/goals_page.dart';

import '../../helpers/accessibility.dart';

void main() {
  testWidgets('goal deletion requires confirmation and reloads data', (
    tester,
  ) async {
    final repository = FakeGoalRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: GoalsPage(repository: repository),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Hedefi sil'));
    await tester.pumpAndSettle();
    expect(find.text('Tasarruf hedefi silinsin mi?'), findsOneWidget);
    await tester.tap(find.widgetWithText(FilledButton, 'Hedefi sil'));
    await tester.pumpAndSettle();

    expect(repository.deletedGoalId, 'goal-1');
    expect(repository.loadCount, 2);
    expect(find.text('Tasarruf hedefi silindi.'), findsOneWidget);
  });

  testWidgets('hedef eylemleri ayrı biçimlerde ve taşmadan durur', (
    tester,
  ) async {
    // İki eylem önceden aynı ağırlıkta iki `TextButton` olarak alt alta
    // duruyordu; hangisinin yıkıcı olduğu yalnız metinden anlaşılıyordu.
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: GoalsPage(repository: FakeGoalRepository()),
      ),
    );

    expect(find.widgetWithText(FilledButton, 'Katkı ekle'), findsOneWidget);
    expect(find.widgetWithText(OutlinedButton, 'Hedefi sil'), findsOneWidget);
    expectNoOverflow(tester);
  });

  testWidgets('manuel hedef, katkının para taşımadığını söyler', (
    tester,
  ) async {
    // Kullanıcı "sadece buraya para yükleyebiliyoruz demi" diye sormuştu:
    // iki mod ekranda birbirinin aynısıydı ve manuel katkı gerçek bir para
    // hareketi sanılıyordu.
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: GoalsPage(repository: FakeGoalRepository()),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Elle takip'), findsOneWidget);
    expect(
      find.text('Katkılar yalnız kayıttır; hesaplarınızdan para çıkmaz.'),
      findsOneWidget,
    );
  });

  testWidgets('bakiye hedefi hangi hesabı izlediğini adıyla söyler', (
    tester,
  ) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: GoalsPage(
          repository: FakeGoalRepository(
            mode: 'account-balance',
            accountId: 'account-1',
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Hesap bakiyesi'), findsOneWidget);
    expect(find.text('Cash hesabının bakiyesini izler.'), findsOneWidget);
    // Bakiye hedefinde elle katkı yok: sayı hesabın kendi bakiyesi.
    expect(find.widgetWithText(FilledButton, 'Katkı ekle'), findsNothing);
  });

  testWidgets('katkı formu tutar alanının altında da uyarır', (tester) async {
    // Kartta yazması yetmiyor: uyarı eylemin yapıldığı yerde de olmalı.
    tester.view.physicalSize = const Size(600, 1200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: GoalsPage(repository: FakeGoalRepository()),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithText(FilledButton, 'Katkı ekle'));
    await tester.pumpAndSettle();

    expect(
      find.text('Bu kayıt para taşımaz; hesap bakiyeniz değişmez.'),
      findsOneWidget,
    );
  });
}

class FakeGoalRepository implements GoalRepositoryContract {
  FakeGoalRepository({this.mode = 'manual-contributions', this.accountId});

  final String mode;
  final String? accountId;
  int loadCount = 0;
  String? deletedGoalId;
  String? contributedGoalId;

  @override
  Future<GoalsSnapshot> load(String asOfDate) async {
    loadCount++;
    return GoalsSnapshot(
      accounts: const [DataChoice('account-1', 'Cash', type: 'bank')],
      goals: [
        GoalItem(
          'goal-1',
          'Emergency fund',
          '1000.0000',
          '250.0000',
          '750.0000',
          '25.0000',
          mode,
          'active',
          accountId: accountId,
        ),
      ],
    );
  }

  @override
  Future<void> create(Map<String, Object?> input) async {}

  @override
  Future<void> delete(String goalId) async {
    deletedGoalId = goalId;
  }

  @override
  Future<void> contribute(String goalId, Map<String, Object?> input) async {
    contributedGoalId = goalId;
  }
}
