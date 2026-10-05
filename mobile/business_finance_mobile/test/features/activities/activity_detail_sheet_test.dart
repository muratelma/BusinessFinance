import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_detail_sheet.dart';

void main() {
  testWidgets('labels a transfer by direction rather than generic source', (
    tester,
  ) async {
    await _pump(
      tester,
      _activity(
        kind: ActivityKind.transfer,
        effect: ActivityEffect.neutral,
        sourceName: 'Banka',
        destinationName: 'Nakit',
      ),
    );

    // Tasarım teslimi: iki hesap tek satırda, paranın yönüyle.
    expect(find.text('Hesaplar'), findsOneWidget);
    expect(find.text('Banka → Nakit'), findsOneWidget);
  });

  testWidgets('labels a card payment by the account and the card it settles', (
    tester,
  ) async {
    await _pump(
      tester,
      _activity(
        kind: ActivityKind.cardPayment,
        effect: ActivityEffect.neutral,
        sourceName: 'Banka',
        destinationName: 'Test Kart',
      ),
    );

    expect(find.text('Hesaplar'), findsOneWidget);
    expect(find.text('Banka → Test Kart'), findsOneWidget);
  });

  testWidgets('spells out that a neutral movement does not change the report', (
    tester,
  ) async {
    await _pump(
      tester,
      _activity(kind: ActivityKind.transfer, effect: ActivityEffect.neutral),
    );

    expect(find.text('Gelir/gider raporunu etkilemez'), findsOneWidget);
  });

  testWidgets('offers cancelling only when the server allows it', (
    tester,
  ) async {
    await _pump(tester, _activity(), onCancel: () async {});
    expect(find.text('Hareketi iptal et'), findsOneWidget);
  });

  testWidgets('explains why a recurring result cannot be cancelled', (
    tester,
  ) async {
    await _pump(
      tester,
      _activity(origin: ActivityOrigin.recurring, canCancel: false),
    );

    expect(find.text('Hareketi iptal et'), findsNothing);
    expect(
      find.text('Tekrarlayan plandan üretilen hareket iptal edilemez.'),
      findsOneWidget,
    );
  });

  testWidgets('explains why an installment result cannot be cancelled', (
    tester,
  ) async {
    await _pump(
      tester,
      _activity(
        kind: ActivityKind.cardCharge,
        origin: ActivityOrigin.installment,
        canCancel: false,
      ),
    );

    expect(
      find.text('Taksit planından üretilen hareket iptal edilemez.'),
      findsOneWidget,
    );
  });

  testWidgets('hides the attachment action on kinds that cannot carry one', (
    tester,
  ) async {
    await _pump(
      tester,
      _activity(kind: ActivityKind.transfer, supportsAttachments: false),
    );
    expect(find.byIcon(Icons.attach_file), findsNothing);

    await _pump(tester, _activity());
    expect(find.byIcon(Icons.attach_file), findsOneWidget);
  });

  testWidgets('asks for confirmation and does nothing when dismissed', (
    tester,
  ) async {
    var calls = 0;
    await _pump(tester, _activity(), onCancel: () async => calls++);

    await tester.tap(find.text('Hareketi iptal et'));
    await tester.pumpAndSettle();
    expect(find.text('Hareket iptal edilsin mi?'), findsOneWidget);

    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();

    expect(calls, 0);
  });

  testWidgets('cancels once the confirmation is accepted', (tester) async {
    var calls = 0;
    await _pump(tester, _activity(), onCancel: () async => calls++);

    await tester.tap(find.text('Hareketi iptal et'));
    await tester.pumpAndSettle();
    // Onay düğmesi panelin kendi düğmesiyle aynı adı taşır; son eşleşme
    // açılan onay penceresindedir.
    await tester.tap(find.text('Hareketi iptal et').last);
    await tester.pumpAndSettle();

    expect(calls, 1);
  });

  /// The server has no idempotency key yet, so a second tap would be a second
  /// write. The button must be inert while the first request is in flight.
  testWidgets('refuses a second tap while a cancel is already running', (
    tester,
  ) async {
    var calls = 0;
    await _pump(
      tester,
      _activity(),
      onCancel: () async => calls++,
      isCancelling: true,
    );

    await tester.tap(find.text('Hareketi iptal et'));
    await tester.pump();

    expect(find.text('Hareket iptal edilsin mi?'), findsNothing);
    expect(calls, 0);
  });

  // Gün sonundan gelen kayıt tek başına iptal edilemez (ADR 0019 T1): ayrıntı
  // nedenini söyler ve gün sonuna götürür; geri alma oradadır.
  testWidgets('gün sonundan gelen kayıt iptal sunmaz, gün sonuna götürür', (
    tester,
  ) async {
    var opened = 0;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: ActivityDetailSheet(
            activity: _activity(
              effect: ActivityEffect.income,
              origin: ActivityOrigin.dayClose,
              canCancel: false,
            ),
            onShowDayClose: () => opened++,
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Hareketi iptal et'), findsNothing);
    expect(
      find.textContaining('Gün sonundan gelen kayıt tek başına iptal edilemez'),
      findsOneWidget,
    );
    await tester.tap(find.text('Gün sonunu gör'));
    expect(opened, 1);
  });

  // Tek tek girilmiş ama bir gün sonunda sayılmış kayıt da iptal sunmaz:
  // iptal edilseydi günün geliri sessizce eksilirdi. Ayrıntı nedenini söyler
  // ve aynı güne götürür.
  testWidgets('gün sonunda sayılan kayıt iptal sunmaz, güne götürür', (
    tester,
  ) async {
    var opened = 0;
    final activity = _activity(
      effect: ActivityEffect.income,
      canCancel: false,
      dayCloseId: 'close-1',
    );
    expect(activity.isCountedInDayClose, isTrue);
    expect(activity.isWrittenByDayClose, isFalse);

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: ActivityDetailSheet(
            activity: activity,
            onShowDayClose: () => opened++,
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Hareketi iptal et'), findsNothing);
    expect(find.textContaining('Gün sonunda sayıldı'), findsOneWidget);
    await tester.tap(find.text('Gün sonunu gör'));
    expect(opened, 1);
  });

  testWidgets('wraps a long description instead of overflowing', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await _pump(
      tester,
      _activity(
        description: 'Çok uzun bir açıklama ' * 12,
        title: 'Çok uzun bir kategori adı ' * 4,
      ),
    );

    expect(tester.takeException(), isNull);
  });

  testWidgets('stays usable at the largest text scale', (tester) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MediaQuery(
        data: const MediaQueryData(textScaler: TextScaler.linear(2)),
        child: MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: ActivityDetailSheet(
              activity: _activity(),
              onCancel: () async {},
            ),
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('Hareketi iptal et'), findsOneWidget);
  });
}

Future<void> _pump(
  WidgetTester tester,
  FinancialActivity activity, {
  Future<void> Function()? onCancel,
  bool isCancelling = false,
}) async {
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: Scaffold(
        body: ActivityDetailSheet(
          activity: activity,
          onCancel: onCancel,
          isCancelling: isCancelling,
        ),
      ),
    ),
  );
  // A cancel in flight shows a spinner, which never settles, so those cases
  // advance a single frame instead.
  if (isCancelling) {
    await tester.pump();
  } else {
    await tester.pumpAndSettle();
  }
}

FinancialActivity _activity({
  ActivityKind kind = ActivityKind.accountTransaction,
  ActivityEffect effect = ActivityEffect.expense,
  ActivityOrigin origin = ActivityOrigin.manual,
  ActivityStatus status = ActivityStatus.realized,
  String title = 'Market',
  String? description,
  String? sourceName = 'Banka',
  String? destinationName,
  bool canCancel = true,
  bool supportsAttachments = true,
  String? dayCloseId,
}) => FinancialActivity(
  activityId: 'a',
  kind: kind,
  effect: effect,
  sourceGroup: ActivitySourceGroup.account,
  origin: origin,
  status: status,
  activityDate: '2026-08-14',
  amount: '625.5000',
  currency: 'TRY',
  title: title,
  description: description,
  categoryName: 'Groceries',
  sourceName: sourceName,
  destinationName: destinationName,
  canCancel: canCancel,
  supportsAttachments: supportsAttachments,
  dayCloseId: dayCloseId,
);
