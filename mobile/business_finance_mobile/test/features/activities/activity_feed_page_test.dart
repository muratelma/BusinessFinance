import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import '../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/activities/presentation/activity_feed_page.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/pos/data/pos_repository.dart';

void main() {
  testWidgets('işlem akışı erişilebilirlik kapısını geçer', (tester) async {
    await pumpAtLargestTextScale(
      tester,
      MaterialApp(
        theme: AppTheme.light(),
        home: ActivityFeedPage(
          repository: _FakeRepository(
            pages: [
              _page([_activity(id: 'a')]),
            ],
          ),
        ),
      ),
    );

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  // Kullanıcı bir açıklama yazdığı anda kayıt adı o cümle oluyor ve kategori
  // satırdan tamamen kayboluyordu: "Ulaşım" gideri listede yalnız
  // "İstanbulkart'la Marmaray'a bindim" olarak görünüyordu.
  testWidgets('kayıt adı serbest metinken kategori alt satırda kalır', (
    tester,
  ) async {
    await _pump(
      tester,
      _FakeRepository(
        pages: [
          _page([
            _activity(
              id: 'ride',
              title: 'İstanbulkart\'la Marmaray\'a bindim',
              categoryName: 'Ulaşım',
              sourceName: 'Banka',
            ),
          ]),
        ],
      ),
    );

    expect(find.text('İstanbulkart\'la Marmaray\'a bindim'), findsOneWidget);
    expect(find.textContaining('Ulaşım'), findsOneWidget);
    // Serbest metin satırı aşağı itmemeli; tam metin ayrıntı panelindedir.
    final title = tester.widget<Text>(
      find.text('İstanbulkart\'la Marmaray\'a bindim'),
    );
    expect(title.maxLines, 1);
  });

  testWidgets('kayıt adı zaten kategoriyse kategori tekrar yazılmaz', (
    tester,
  ) async {
    await _pump(
      tester,
      _FakeRepository(
        pages: [
          _page([
            // Kullanıcı açıklama yazmadığında sunucu başlığı kategori adından
            // üretir; aynı kelimeyi iki kez göstermek bilgi eklemez.
            _activity(id: 'plain', title: 'Ulaşım', categoryName: 'Ulaşım'),
          ]),
        ],
      ),
    );

    expect(find.textContaining('Ulaşım • Banka'), findsNothing);
    expect(find.text('Ulaşım'), findsOneWidget);
  });

  testWidgets('lists every kind with a signed amount and its source', (
    tester,
  ) async {
    await _pump(
      tester,
      _FakeRepository(
        pages: [
          _page([
            _activity(
              id: 'income',
              effect: ActivityEffect.income,
              title: 'Maaş',
              sourceName: 'Banka',
              amount: '18000.0000',
            ),
            _activity(
              id: 'transfer',
              kind: ActivityKind.transfer,
              effect: ActivityEffect.neutral,
              sourceGroup: ActivitySourceGroup.transfer,
              title: 'Nakit',
              sourceName: 'Banka',
              destinationName: 'Nakit',
              amount: '250.0000',
            ),
          ]),
        ],
      ),
    );

    expect(find.text('Maaş'), findsOneWidget);
    expect(find.textContaining('+'), findsOneWidget);
    // A neutral movement carries no sign: it is neither income nor expense.
    expect(find.textContaining('Banka → Nakit'), findsOneWidget);
  });

  testWidgets('marks a cancelled movement in words, not only by fading it', (
    tester,
  ) async {
    await _pump(
      tester,
      _FakeRepository(
        pages: [
          _page([
            _activity(
              id: 'a',
              status: ActivityStatus.cancelled,
              canCancel: false,
            ),
          ]),
        ],
      ),
    );

    expect(find.text('İptal edildi'), findsOneWidget);
  });

  testWidgets('describes a row in one sentence for a screen reader', (
    tester,
  ) async {
    final handle = tester.ensureSemantics();
    await _pump(
      tester,
      _FakeRepository(
        pages: [
          _page([
            _activity(
              id: 'a',
              title: 'Market',
              sourceName: 'Test Kart',
              status: ActivityStatus.cancelled,
            ),
          ]),
        ],
      ),
    );

    expect(
      find.bySemanticsLabel(RegExp(r'Gider.*Market|Market.*Gider')),
      findsOneWidget,
    );
    expect(find.bySemanticsLabel(RegExp('İptal edildi')), findsOneWidget);
    handle.dispose();
  });

  testWidgets('a quick chip narrows the feed through the repository', (
    tester,
  ) async {
    final repository = _FakeRepository(
      pages: [
        _page([_activity(id: 'a')]),
        _page([_activity(id: 'b', kind: ActivityKind.cardCharge)]),
      ],
    );
    await _pump(tester, repository);

    await tester.tap(find.text('Kredi kartları'));
    await tester.pumpAndSettle();

    expect(
      repository.filters.last.quickFilter,
      ActivityQuickFilter.creditCards,
    );
  });

  testWidgets('searches on the server after the user pauses typing', (
    tester,
  ) async {
    // Sayfalı listenin yüklenen kısmını süzmek eksik sonuç verirdi; arama
    // sunucuya gider ve her tuşta değil, yazma duraklayınca.
    final repository = _FakeRepository(
      pages: [
        _page([_activity(id: 'a')]),
      ],
    );
    await _pump(tester, repository);
    final before = repository.filters.length;

    await tester.enterText(find.byType(TextField), '  kira ');
    await tester.pump(const Duration(milliseconds: 100));
    expect(repository.filters, hasLength(before));

    await tester.pump(const Duration(milliseconds: 400));
    await tester.pumpAndSettle();
    expect(repository.filters.last.search, 'kira');

    await tester.tap(find.byTooltip('Aramayı temizle'));
    await tester.pumpAndSettle();
    expect(repository.filters.last.search, isNull);
  });

  testWidgets('says what was searched when nothing matches', (tester) async {
    final repository = _FakeRepository(
      pages: [
        _page([_activity(id: 'a')]),
        _page([]),
      ],
    );
    await _pump(tester, repository);

    await tester.enterText(find.byType(TextField), 'zzz');
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pumpAndSettle();

    expect(find.text('Eşleşen hareket yok'), findsOneWidget);
    expect(find.textContaining('"zzz"'), findsOneWidget);
  });

  testWidgets('groups the feed by day with a relative day name', (
    tester,
  ) async {
    // Tarih satırda tekrar yazılmaz; gün başlığı söyler.
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ActivityFeedPage(
          repository: _FakeRepository(
            pages: [
              _page([_activity(id: 'a')]),
            ],
          ),
          now: () => DateTime(2026, 8, 15),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('14 Ağustos  Dün', findRichText: true), findsOneWidget);
    // Satırın alt metni yalnız hesabı taşır, tarihi değil.
    expect(find.text('Banka'), findsOneWidget);
  });

  testWidgets('shows an empty state that names the reason', (tester) async {
    await _pump(tester, _FakeRepository(pages: [_page([])]));

    expect(find.text('Henüz hareket yok'), findsOneWidget);
  });

  testWidgets('shows a retryable error instead of an empty list', (
    tester,
  ) async {
    final repository = _FakeRepository(
      error: const ApiException(code: 'server.error', message: 'Sunucu hatası'),
    );
    await _pump(tester, repository);

    expect(find.text('Sunucu hatası'), findsOneWidget);
    expect(find.text('Tekrar dene'), findsOneWidget);
  });

  testWidgets('an expired session is reported, not shown as no data', (
    tester,
  ) async {
    await _pump(
      tester,
      _FakeRepository(
        error: const ApiException(
          code: 'authentication.unauthorized',
          message: 'Oturum sona erdi',
          statusCode: 401,
        ),
      ),
    );

    expect(find.textContaining('Oturumunuz sona erdi'), findsOneWidget);
  });

  testWidgets('offers a way to load the next page when the server has one', (
    tester,
  ) async {
    final repository = _FakeRepository(
      pages: [
        _page([_activity(id: 'a')], hasNext: true, totalCount: 2),
        _page([_activity(id: 'b')], pageNumber: 2, totalCount: 2),
      ],
    );
    await _pump(tester, repository);

    await tester.tap(find.text('Daha fazla göster'));
    await tester.pumpAndSettle();

    expect(repository.requestedPages, [1, 2]);
    expect(find.text('Toplam 2 hareket'), findsOneWidget);
  });

  testWidgets('stays readable at the largest text scale', (tester) async {
    tester.view.physicalSize = const Size(400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(
      MediaQuery(
        data: const MediaQueryData(textScaler: TextScaler.linear(2)),
        child: MaterialApp(
          theme: AppTheme.light(),
          home: ActivityFeedPage(
            repository: _FakeRepository(
              pages: [
                _page([_activity(id: 'a', title: 'Çok uzun bir kategori adı')]),
              ],
            ),
          ),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('Çok uzun bir kategori adı'), findsOneWidget);
  });

  testWidgets('opens the detail sheet from a row and cancels through it', (
    tester,
  ) async {
    final repository = _FakeRepository(
      pages: [
        _page([_activity(id: 'a', title: 'Market')]),
        _page([]),
      ],
    );
    await _pump(tester, repository);

    await tester.tap(find.text('Market'));
    await tester.pumpAndSettle();
    expect(find.text('Tarih'), findsOneWidget);

    await tester.tap(find.text('Hareketi iptal et'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Hareketi iptal et').last);
    await tester.pumpAndSettle();

    expect(repository.cancelled, hasLength(1));
    // The sheet closes and the outcome is reported once.
    expect(find.text('Tarih'), findsNothing);
    expect(find.text('Hareket iptal edildi.'), findsOneWidget);
  });

  testWidgets('a locked row opens the sheet without offering a cancel', (
    tester,
  ) async {
    await _pump(
      tester,
      _FakeRepository(
        pages: [
          _page([_activity(id: 'a', title: 'Kira', canCancel: false)]),
        ],
      ),
    );

    await tester.tap(find.text('Kira'));
    await tester.pumpAndSettle();

    expect(find.text('Tarih'), findsOneWidget);
    expect(find.text('Hareketi iptal et'), findsNothing);
  });

  // Yatış satırı genel ayrıntıyı değil yatışın kendi ayrıntısını açar:
  // kapattığı tahsilatlar, kesinti ve geri alma oradadır (ADR 0019 T5).
  testWidgets('POS yatışı satırı yatış ayrıntısını açar', (tester) async {
    final posRepository = _FakePosRepository();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ActivityFeedPage(
          repository: _FakeRepository(
            pages: [
              _page([
                _activity(
                  id: 'deposit-1',
                  kind: ActivityKind.posDeposit,
                  effect: ActivityEffect.neutral,
                  sourceGroup: ActivitySourceGroup.pos,
                  title: 'Ziraat',
                  canCancel: false,
                ),
              ]),
            ],
          ),
          posRepository: posRepository,
        ),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Ziraat'));
    await tester.pumpAndSettle();

    expect(posRepository.requestedDepositId, 'deposit-1');
    expect(find.text('POS yatışı'), findsOneWidget);
    expect(find.text('Yatışı geri al'), findsOneWidget);
    // Genel ayrıntı değil: onun durum kapsülü burada yoktur.
    expect(find.text('Gerçekleşti'), findsNothing);
  });

  // 2 Ekim 2026 emülatör turu: yatışa dokununca önce ekranı kaplayan bir
  // "yükleniyor" penceresi açılıyor, sonra küçülüyordu. Panel satırın
  // taşıdıklarıyla hemen ve son boyutunda açılır.
  testWidgets('yatış ayrıntısı yüklenmeyi beklemeden açılır', (tester) async {
    final posRepository = _FakePosRepository()..pending = Completer<void>();
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: ActivityFeedPage(
          repository: _FakeRepository(
            pages: [
              _page([
                _activity(
                  id: 'deposit-1',
                  kind: ActivityKind.posDeposit,
                  effect: ActivityEffect.neutral,
                  sourceGroup: ActivitySourceGroup.pos,
                  title: '',
                  sourceName: null,
                  destinationName: 'Ziraat',
                  amount: '195.0000',
                  canCancel: false,
                ),
              ]),
            ],
          ),
          posRepository: posRepository,
        ),
      ),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('POS yatışı'));
    await tester.pumpAndSettle();

    expect(find.text('Yatış yükleniyor'), findsNothing);
    expect(find.text('Hesaba yatan tutar'), findsOneWidget);
    expect(find.text('Tarih'), findsOneWidget);
    // Yüklenmeden geri alınamaz.
    expect(
      tester
          .widget<OutlinedButton>(
            find.ancestor(
              of: find.text('Yatışı geri al'),
              matching: find.bySubtype<OutlinedButton>(),
            ),
          )
          .onPressed,
      isNull,
    );

    posRepository.pending!.complete();
    await tester.pumpAndSettle();

    expect(
      tester
          .widget<OutlinedButton>(
            find.ancestor(
              of: find.text('Yatışı geri al'),
              matching: find.bySubtype<OutlinedButton>(),
            ),
          )
          .onPressed,
      isNotNull,
    );
    expect(find.text('Bakiye'), findsOneWidget);
  });

  testWidgets('ayrıntı açılırken kalan bakiye bir kez istenir', (tester) async {
    final repository =
        _FakeRepository(
            pages: [
              _page([_activity(id: 'a', title: 'Kira', canCancel: false)]),
            ],
          )
          ..balances = const [
            ActivityBalance(
              holder: ActivityBalanceHolder.account,
              name: 'Banka',
              balance: '900.0000',
              currency: 'TRY',
              change: ActivityBalanceChange.decreased,
            ),
          ];
    await _pump(tester, repository);

    await tester.tap(find.text('Kira'));
    await tester.pumpAndSettle();

    expect(find.text('Bakiye'), findsOneWidget);
    expect(repository.balanceRequests, 1);
  });

  testWidgets('POS deposu yokken yatış satırı genel ayrıntıda açılır', (
    tester,
  ) async {
    await _pump(
      tester,
      _FakeRepository(
        pages: [
          _page([
            _activity(
              id: 'deposit-1',
              kind: ActivityKind.posDeposit,
              effect: ActivityEffect.neutral,
              sourceGroup: ActivitySourceGroup.pos,
              title: 'Ziraat',
              canCancel: false,
            ),
          ]),
        ],
      ),
    );

    await tester.tap(find.text('Ziraat'));
    await tester.pumpAndSettle();

    expect(find.text('Tarih'), findsOneWidget);
    expect(
      find.text("Yatış, Kasa'daki POS tahsilatlarından geri alınır."),
      findsOneWidget,
    );
  });

  testWidgets('borç taksidi satırı tek kayıttır, faizini alt satırda söyler', (
    tester,
  ) async {
    // Faiz ayrı bir satır olsaydı işlemler toplamı hesaptan çıkan parayla
    // tutmazdı (2.600 + 100 = 2.700). Tek satır kalıyor, ama 2.600'ün nesinin
    // gider olduğu ödemenin göründüğü yerde okunabiliyor.
    await _pump(
      tester,
      _FakeRepository(
        pages: [
          _page([
            _activity(
              id: 'debt',
              kind: ActivityKind.debtPayment,
              effect: ActivityEffect.neutral,
              sourceGroup: ActivitySourceGroup.debt,
              title: 'Konut Kredisi',
              amount: '2600.0000',
              principalPortion: '2500.0000',
              interestPortion: '100.0000',
              canCancel: false,
            ),
          ]),
        ],
      ),
    );

    // Tek satır: faiz için ikinci bir kayıt yok.
    expect(find.text('Konut Kredisi'), findsOneWidget);
    expect(find.textContaining('faizi ₺100,00'), findsOneWidget);
  });

  testWidgets('faizsiz taksitte alt satır kalabalık yapmaz', (tester) async {
    await _pump(
      tester,
      _FakeRepository(
        pages: [
          _page([
            _activity(
              id: 'debt',
              kind: ActivityKind.debtPayment,
              effect: ActivityEffect.neutral,
              sourceGroup: ActivitySourceGroup.debt,
              title: 'Arkadaş borcu',
              amount: '500.0000',
              principalPortion: '500.0000',
              interestPortion: '0.0000',
              canCancel: false,
            ),
          ]),
        ],
      ),
    );

    expect(find.textContaining('faizi'), findsNothing);
  });
}

/// Yalnız yatış okumasını karşılar; feed başka hiçbir POS ucuna dokunmaz.
class _FakePosRepository implements PosRepositoryContract {
  String? requestedDepositId;

  /// Verilirse okuma bu tamamlanana kadar bekler.
  Completer<void>? pending;

  @override
  Future<PosDeposit> getDeposit({required String depositId}) async {
    requestedDepositId = depositId;
    await pending?.future;
    return const PosDeposit(
      id: 'deposit-1',
      accountName: 'Ziraat',
      depositDate: '2026-08-27',
      expectedAmount: '195.0000',
      depositedAmount: '195.0000',
      deductionAmount: '0.0000',
      currency: 'TRY',
      isCancelled: false,
      settlements: [],
      balanceAfter: '1195.0000',
    );
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

Future<void> _pump(
  WidgetTester tester,
  ActivityRepositoryContract repository,
) async {
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: ActivityFeedPage(repository: repository),
    ),
  );
  await tester.pumpAndSettle();
}

ActivityPage _page(
  List<FinancialActivity> items, {
  int pageNumber = 1,
  int? totalCount,
  bool hasNext = false,
}) => ActivityPage(
  items: items,
  pagination: ActivityPagination(
    pageNumber: pageNumber,
    pageSize: 20,
    totalCount: totalCount ?? items.length,
    totalPages: hasNext ? pageNumber + 1 : pageNumber,
    hasPreviousPage: pageNumber > 1,
    hasNextPage: hasNext,
  ),
);

FinancialActivity _activity({
  required String id,
  ActivityKind kind = ActivityKind.accountTransaction,
  ActivityEffect effect = ActivityEffect.expense,
  ActivitySourceGroup sourceGroup = ActivitySourceGroup.account,
  ActivityStatus status = ActivityStatus.realized,
  String title = 'Market',
  String amount = '625.5000',
  String? sourceName = 'Banka',
  String? destinationName,
  String? categoryName,
  bool canCancel = true,
  String? principalPortion,
  String? interestPortion,
}) => FinancialActivity(
  categoryName: categoryName,
  activityId: id,
  kind: kind,
  effect: effect,
  sourceGroup: sourceGroup,
  origin: ActivityOrigin.manual,
  status: status,
  activityDate: '2026-08-14',
  amount: amount,
  currency: 'TRY',
  title: title,
  sourceName: sourceName,
  destinationName: destinationName,
  canCancel: canCancel,
  supportsAttachments: kind == ActivityKind.accountTransaction,
  principalPortion: principalPortion,
  interestPortion: interestPortion,
);

class _FakeRepository implements ActivityRepositoryContract {
  @override
  Future<List<ActivityBalance>> balancesAfter(
    FinancialActivity activity,
  ) async {
    balanceRequests++;
    return balances;
  }

  /// İşlem ayrıntısındaki "kalan bakiye" için dönecek cevap.
  List<ActivityBalance> balances = const [];
  int balanceRequests = 0;

  _FakeRepository({this.pages = const [], this.error});

  final List<ActivityPage> pages;
  final Object? error;
  final List<int> requestedPages = [];
  final List<ActivityFilter> filters = [];
  int _call = 0;

  @override
  Future<ActivityPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    ActivityFilter filter = const ActivityFilter(),
    DateTime? today,
    TransactionScope? scope,
  }) async {
    requestedPages.add(pageNumber);
    filters.add(filter);
    if (error != null) throw error!;
    final page = pages[_call.clamp(0, pages.length - 1)];
    _call++;
    return page;
  }

  final List<FinancialActivity> cancelled = [];
  Object? cancelError;

  @override
  Future<void> cancel(FinancialActivity activity) async {
    cancelled.add(activity);
    if (cancelError != null) throw cancelError!;
  }

  PlannedActivityPage? plannedPage;

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async =>
      plannedPage ??
      const PlannedActivityPage(
        asOfDate: '2026-08-14',
        daysAhead: 30,
        totalCount: 0,
        items: [],
      );

  @override
  Future<void> realizePlanned(PlannedActivity activity) =>
      throw UnimplementedError();
}
