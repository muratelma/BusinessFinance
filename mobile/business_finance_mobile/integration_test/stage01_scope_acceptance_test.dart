import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:integration_test/integration_test.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/storage/secure_session_store.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_models.dart';
import 'package:business_finance_mobile/features/auth/data/auth_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_service.dart';
import 'package:business_finance_mobile/features/categories/data/category_models.dart';
import 'package:business_finance_mobile/features/categories/data/category_repository.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';
import 'package:business_finance_mobile/features/profile/data/profile_repository.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_models.dart';
import 'package:business_finance_mobile/features/transactions/data/transaction_repository.dart';

/// Aşama 01'in kabul turu: iki esnaf, gerçek API ve gerçek SQL üzerinde.
///
/// Ölçüt aşama belgesinden: **işletme neti şahsi harcamadan etkilenmez.**
/// Buradaki her kayıt kapsam **göndermeden** oluşturuluyor; kapsamı sunucu
/// türetiyor. Kullanıcının uygulamada yapabildiği şey budur: kaydolurken
/// "işletmem var" der, kategori setini alır ve dokunmadan doğru tarafa yazar.
void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('esnaf, dükkânın netini şahsi harcamasından ayrı okur', (
    tester,
  ) async {
    final suffix = DateTime.now().microsecondsSinceEpoch;
    final email = 'stage01.kasap.$suffix@example.test';
    const password = 'Stage01-Scope-2026';
    final today = DateTime.now();
    final date = _dateText(today);

    final httpClient = http.Client();
    addTearDown(httpClient.close);
    final session = _MemorySessionStore();
    final auth = AuthRepository(
      remoteService: ApiAuthService(
        ApiClient(config: ApiConfig.fromEnvironment(), httpClient: httpClient),
      ),
      sessionStore: session,
    );

    // Kaydolurken sorulan tek soru: işletmesi var.
    await auth.register(email, password, hasBusiness: true);
    await auth.login(email, password);
    final trader = _Repositories(auth, httpClient);

    final profile = await trader.profile.read();
    expect(profile.hasBusiness, isTrue, reason: 'Cevap sunucuda saklanmalı.');

    final account = await trader.accounts.create(
      name: 'Dükkân kasası $suffix',
      type: 'cash',
      openingBalance: '1000',
    );

    // İşletme kategori seti kurulu geldi; kalemler kapsam taşıyor.
    final sales = await trader.categoryNamed('Satış geliri', 'income');
    final stock = await trader.categoryNamed('Ticari mal alımı', 'expense');
    final groceries = await trader.categoryNamed(
      'Market Alışverişi',
      'expense',
    );

    // Dükkân: 600 satış, 200 mal alımı. Kapsam **gönderilmiyor**.
    await trader.record(
      account.id,
      sales.id,
      '600.0000',
      TransactionKind.income,
      date,
    );
    await trader.record(
      account.id,
      stock.id,
      '200.0000',
      TransactionKind.expense,
      date,
    );

    final beforeGroceries = await trader.dashboard.getMonthly(
      today.year,
      today.month,
    );
    final businessNetBefore = beforeGroceries.scopeBreakdown!.business.net;
    expect(businessNetBefore, '400.0000');
    expect(beforeGroceries.scopeBreakdown!.personal.net, '0.0000');

    // Patron markete gidiyor: aynı kasadan, şahsi kategoriden 300.
    await trader.record(
      account.id,
      groceries.id,
      '300.0000',
      TransactionKind.expense,
      date,
    );

    final afterGroceries = await trader.dashboard.getMonthly(
      today.year,
      today.month,
    );

    // Aşamanın çıkış ölçütü: işletme neti kıpırdamadı.
    expect(afterGroceries.scopeBreakdown!.business.net, businessNetBefore);
    expect(afterGroceries.scopeBreakdown!.personal.net, '-300.0000');
    expect(afterGroceries.net, '100.0000');

    // Kapsam raporu böler, parayı bölmez: bakiye üç okumada da aynı.
    final balances = <String>[];
    for (final filter in <TransactionScope?>[
      null,
      TransactionScope.business,
      TransactionScope.personal,
    ]) {
      final report = await trader.dashboard.getMonthly(
        today.year,
        today.month,
        scope: filter,
      );
      balances.add(
        report.accountBalances
            .firstWhere((item) => item.accountId == account.id)
            .balance,
      );
    }
    expect(balances.toSet(), {'1100.0000'});

    // Filtreli okuma tek tarafı anlatır ve kırılım taşımaz.
    final businessOnly = await trader.dashboard.getMonthly(
      today.year,
      today.month,
      scope: TransactionScope.business,
    );
    expect(businessOnly.totalIncome, '600.0000');
    expect(businessOnly.totalExpense, '200.0000');
    expect(businessOnly.scopeBreakdown, isNull);

    // Feed de bölünüyor: dükkân tarafında iki satır var, market yok.
    final businessFeed = await trader.activities.list(
      scope: TransactionScope.business,
    );
    final personalFeed = await trader.activities.list(
      scope: TransactionScope.personal,
    );
    expect(businessFeed.items.length, 2);
    expect(personalFeed.items.length, 1);
    expect(personalFeed.items.single.amount, '300.0000');
  });

  testWidgets('istisna tek dokunuşla düzeltilir ve ikinci esnaf onu görmez', (
    tester,
  ) async {
    final suffix = DateTime.now().microsecondsSinceEpoch;
    final firstEmail = 'stage01.manav.$suffix@example.test';
    final secondEmail = 'stage01.terzi.$suffix@example.test';
    const password = 'Stage01-Scope-2026';
    final today = DateTime.now();
    final date = _dateText(today);

    final httpClient = http.Client();
    addTearDown(httpClient.close);

    final firstAuth = AuthRepository(
      remoteService: ApiAuthService(
        ApiClient(config: ApiConfig.fromEnvironment(), httpClient: httpClient),
      ),
      sessionStore: _MemorySessionStore(),
    );
    await firstAuth.register(firstEmail, password, hasBusiness: true);
    await firstAuth.login(firstEmail, password);
    final greengrocer = _Repositories(firstAuth, httpClient);

    final account = await greengrocer.accounts.create(
      name: 'Manav kasası $suffix',
      type: 'cash',
      openingBalance: '0',
    );
    final sales = await greengrocer.categoryNamed('Satış geliri', 'income');
    final vehicle = await greengrocer.categoryNamed('Araç ve yakıt', 'expense');

    await greengrocer.record(
      account.id,
      sales.id,
      '1000.0000',
      TransactionKind.income,
      date,
    );
    // İstisna: kamyonete konan yakıtın bir kısmı hafta sonu şahsi kullanım.
    // Kullanıcı formda çipe dokunup kapsamı değiştiriyor; kategori işletme
    // kalemi olduğu hâlde kayıt şahsi tarafa yazılıyor.
    await greengrocer.record(
      account.id,
      vehicle.id,
      '150.0000',
      TransactionKind.expense,
      date,
      scope: TransactionScope.personal,
    );

    final report = await greengrocer.dashboard.getMonthly(
      today.year,
      today.month,
    );
    expect(report.scopeBreakdown!.business.net, '1000.0000');
    expect(report.scopeBreakdown!.personal.net, '-150.0000');
    expect(report.net, '850.0000');

    // İkinci esnaf: kendi verisi boş, birincininkini hiçbir kapsamda görmez.
    final secondAuth = AuthRepository(
      remoteService: ApiAuthService(
        ApiClient(config: ApiConfig.fromEnvironment(), httpClient: httpClient),
      ),
      sessionStore: _MemorySessionStore(),
    );
    await secondAuth.register(secondEmail, password, hasBusiness: true);
    await secondAuth.login(secondEmail, password);
    final tailor = _Repositories(secondAuth, httpClient);

    final tailorReport = await tailor.dashboard.getMonthly(
      today.year,
      today.month,
    );
    expect(tailorReport.scopeBreakdown!.business.net, '0.0000');
    expect(tailorReport.scopeBreakdown!.personal.net, '0.0000');
    expect(tailorReport.accountBalances, isEmpty);

    for (final filter in <TransactionScope?>[
      null,
      TransactionScope.business,
      TransactionScope.personal,
    ]) {
      final feed = await tailor.activities.list(scope: filter);
      expect(
        feed.items,
        isEmpty,
        reason: 'Yabancı kayıt hiçbir kapsamda sızmaz.',
      );
    }
  });

  testWidgets('işletmesi olmayan kullanıcıda her kayıt sessizce şahsi olur', (
    tester,
  ) async {
    final suffix = DateTime.now().microsecondsSinceEpoch;
    final email = 'stage01.evhali.$suffix@example.test';
    const password = 'Stage01-Scope-2026';
    final today = DateTime.now();
    final date = _dateText(today);

    final httpClient = http.Client();
    addTearDown(httpClient.close);
    final auth = AuthRepository(
      remoteService: ApiAuthService(
        ApiClient(config: ApiConfig.fromEnvironment(), httpClient: httpClient),
      ),
      sessionStore: _MemorySessionStore(),
    );
    await auth.register(email, password, hasBusiness: false);
    await auth.login(email, password);
    final household = _Repositories(auth, httpClient);

    expect((await household.profile.read()).hasBusiness, isFalse);

    final account = await household.accounts.create(
      name: 'Cüzdan $suffix',
      type: 'cash',
      openingBalance: '0',
    );
    final categories = await household.categories.list(
      type: 'expense',
      isActive: true,
    );
    await household.record(
      account.id,
      categories.first.id,
      '75.0000',
      TransactionKind.expense,
      date,
    );

    final report = await household.dashboard.getMonthly(
      today.year,
      today.month,
    );

    // Kapsam arayüzde hiç görünmese de kayıt bir tarafa yazılıyor; kişisel
    // setin tamamı şahsi olduğu için işletme tarafı boş kalıyor.
    expect(report.scopeBreakdown!.business.expense, '0.0000');
    expect(report.scopeBreakdown!.personal.expense, '75.0000');
  });
}

String _dateText(DateTime value) {
  final month = value.month.toString().padLeft(2, '0');
  final day = value.day.toString().padLeft(2, '0');
  return '${value.year}-$month-$day';
}

class _Repositories {
  _Repositories(AuthRepository auth, http.Client httpClient)
    : _client = ApiClient(
        config: ApiConfig.fromEnvironment(),
        httpClient: httpClient,
        accessTokenProvider: auth.getValidAccessToken,
      ) {
    accounts = ApiAccountRepository(_client);
    categories = ApiCategoryRepository(_client);
    transactions = TransactionRepository(_client);
    dashboard = DashboardRepository(_client);
    activities = ActivityRepository(_client);
    profile = ProfileRepository(_client);
  }

  final ApiClient _client;
  late final AccountRepository accounts;
  late final CategoryRepository categories;
  late final TransactionRepositoryContract transactions;
  late final DashboardDataSource dashboard;
  late final ActivityRepositoryContract activities;
  late final ProfileRepositoryContract profile;

  Future<BudgetCategory> categoryNamed(String name, String type) async {
    final all = await categories.list(type: type, isActive: true);
    return all.firstWhere(
      (item) => item.name == name,
      orElse: () => throw StateError(
        'Varsayılan sette "$name" yok: ${all.map((item) => item.name).join(', ')}',
      ),
    );
  }

  Future<void> record(
    String accountId,
    String categoryId,
    String amount,
    TransactionKind kind,
    String date, {
    TransactionScope? scope,
  }) => transactions.create(
    CreateTransactionInput(
      accountId: accountId,
      categoryId: categoryId,
      amount: amount,
      kind: kind,
      transactionDate: date,
      scope: scope,
    ),
  );
}

class _MemorySessionStore implements SessionStore {
  AuthSession? _session;

  @override
  Future<void> clear() async => _session = null;

  @override
  Future<AuthSession?> read() async => _session;

  @override
  Future<void> write(AuthSession session) async => _session = session;
}
