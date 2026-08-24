import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:integration_test/integration_test.dart';
import 'package:business_finance_mobile/core/config/api_config.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/storage/secure_session_store.dart';
import 'package:business_finance_mobile/features/accounts/data/account_repository.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_models.dart';
import 'package:business_finance_mobile/features/auth/data/auth_repository.dart';
import 'package:business_finance_mobile/features/auth/data/auth_service.dart';
import 'package:business_finance_mobile/features/categories/data/category_models.dart';
import 'package:business_finance_mobile/features/categories/data/category_repository.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_models.dart';
import 'package:business_finance_mobile/features/counterparties/data/counterparty_repository.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';

/// Aşama 02'nin kabul turu: **Aşama 01'in kabul hesabıyla**, gerçek API ve
/// gerçek SQL üzerinde.
///
/// Hesap yeniden kurulmuyor, giriş yapılıyor: dükkânın zaten bir geçmişi var
/// ve aşamanın vaadi o geçmişin üstüne gelmeli. Her ölçü **fark** olarak
/// alınıyor; sabit bir toplam beklemek, turu ancak boş bir hesapta doğru
/// yapardı.
///
/// Ölçüt aşama belgesinden: **veresiye satış geliri o gün tanınır ama kasa
/// kıpırdamaz; tahsilat kasayı değiştirir ve geliri ikinci kez saymaz**
/// (ADR 0014).
void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  const email = 'stage01.kabul@example.test';
  const password = 'Stage01-Scope-2026';

  testWidgets('esnaf, veresiye defterini kasadan ayrı okur', (tester) async {
    final suffix = DateTime.now().microsecondsSinceEpoch;
    final today = DateTime.now();
    final date = _dateText(today);

    final httpClient = http.Client();
    addTearDown(httpClient.close);
    final trader = await _signIn(email, password, httpClient);

    // Turun kasası ayrı açılıyor: aynı hesabı tekrar tekrar kullanmak, ikinci
    // turda ilk turun bakiyesini ölçmek olurdu. Defterin sahibi yine aynı
    // esnaf.
    final till = await trader.accounts.create(
      name: 'Veresiye kasası $suffix',
      type: 'cash',
      openingBalance: '0',
    );
    final sales = await trader.categoryNamed('Satış geliri', 'income');

    final incomeBefore = (await trader.dashboard.getMonthly(
      today.year,
      today.month,
    )).totalIncome;
    final worthBefore = await trader.dashboard.getAdvanced(
      today.year,
      today.month,
    );

    await trader.counterparties.create('Kabul Manavı $suffix', 'Kabul turu');
    final manav = await trader.counterpartyNamed('Kabul Manavı $suffix');

    // Üç veresiye satış: 400 + 350 + 250 = 1.000. Kapsam **gönderilmiyor**;
    // kategoriden çözülüyor.
    for (final amount in ['400.0000', '350.0000', '250.0000']) {
      await trader.counterparties.addCharge(manav.id, {
        'direction': 'receivable',
        'amount': amount,
        'currency': 'TRY',
        'categoryId': sales.id,
        'chargeDate': date,
        'description': 'Veresiye satış',
      });
    }

    final afterCharges = await trader.dashboard.getMonthly(
      today.year,
      today.month,
    );
    final worthAfterCharges = await trader.dashboard.getAdvanced(
      today.year,
      today.month,
    );

    // Tanır: gelir 1.000 arttı.
    expect(_delta(afterCharges.totalIncome, incomeBefore), '1000.0000');
    // Taşımaz: kasa kıpırdamadı.
    expect(_balanceOf(afterCharges, till.id), '0.0000');
    // Alacak net varlığa girdi ve **bir kez** sayıldı.
    expect(
      _delta(worthAfterCharges.receivableDebt, worthBefore.receivableDebt),
      '1000.0000',
    );
    expect(
      _delta(worthAfterCharges.netWorth, worthBefore.netWorth),
      '1000.0000',
    );

    final openLedger = await trader.counterpartyNamed('Kabul Manavı $suffix');
    expect(openLedger.receivable, '1000.0000');
    expect(openLedger.net, '1000.0000');
    expect(openLedger.isSettled, isFalse);

    // İki kısmi tahsilat: 400 + 200 = 600. Kategori ve kapsam **sorulmuyor**.
    for (final amount in ['400.0000', '200.0000']) {
      await trader.counterparties.addPayment(manav.id, {
        'direction': 'receivable',
        'amount': amount,
        'currency': 'TRY',
        'accountId': till.id,
        'paymentDate': date,
        'description': 'Kısmi tahsilat',
      });
    }

    final afterCollections = await trader.dashboard.getMonthly(
      today.year,
      today.month,
    );
    final worthAfterCollections = await trader.dashboard.getAdvanced(
      today.year,
      today.month,
    );

    // Gelir ikinci kez sayılmadı; kasa tahsil edileni gördü.
    expect(_delta(afterCollections.totalIncome, incomeBefore), '1000.0000');
    expect(_balanceOf(afterCollections, till.id), '600.0000');
    // Net varlık kıpırdamadı: alacak kasaya taşındı, yeni varlık doğmadı.
    expect(
      _delta(worthAfterCollections.netWorth, worthBefore.netWorth),
      '1000.0000',
    );

    final partlyPaid = await trader.counterpartyNamed('Kabul Manavı $suffix');
    expect(partlyPaid.net, '400.0000');

    // Kişinin geçmişi ekranın okuduğu yerden okunuyor: birleşik feed'in
    // `counterpartyId` ile daraltılmış hâli, ikinci bir geçmiş modeli değil.
    final detail = await trader.counterparties.loadDetail(manav.id, date);
    final charges = detail.activities
        .where(
          (item) =>
              item.sourceGroup == ActivitySourceGroup.counterparty &&
              item.effect == ActivityEffect.income,
        )
        .toList(growable: false);
    final collections = detail.activities
        .where(
          (item) =>
              item.sourceGroup == ActivitySourceGroup.counterparty &&
              item.effect == ActivityEffect.neutral,
        )
        .toList(growable: false);
    expect(charges.length, 3);
    expect(collections.length, 2);
    expect(charges.every((item) => item.canCancel), isTrue);
    expect(collections.every((item) => item.canCancel), isTrue);

    // Tahsilat iptali parayı geri alır ve açık bakiyeyi yeniden doğurur.
    final cancelled = collections.firstWhere(
      (item) => item.amount == '200.0000',
    );
    await trader.activities.cancel(cancelled);

    final afterCancel = await trader.dashboard.getMonthly(
      today.year,
      today.month,
    );
    expect(_balanceOf(afterCancel, till.id), '400.0000');
    expect(_delta(afterCancel.totalIncome, incomeBefore), '1000.0000');
    expect(
      (await trader.counterpartyNamed('Kabul Manavı $suffix')).net,
      '600.0000',
    );
  });

  /// Yedek v7 defterin kendisini taşır; cari CSV'si onu okunur hâlde yazar.
  testWidgets('yedek v7 ve cari defterin dosyası aynı defteri anlatır', (
    tester,
  ) async {
    final httpClient = http.Client();
    addTearDown(httpClient.close);
    final trader = await _signIn(email, password, httpClient);

    final backup = await trader.client.getBytes('/api/v1/backups/download');
    final envelope =
        jsonDecode(utf8.decode(backup.bytes)) as Map<String, dynamic>;
    expect(envelope['format'], 'business-finance-backup');
    expect(envelope['schemaVersion'], 7);

    final snapshot =
        jsonDecode(utf8.decode(base64Decode(envelope['payload'] as String)))
            as Map<String, dynamic>;
    // v7'nin taşıdığı üç yeni koleksiyon; defter boş olsaydı tur zaten
    // hiçbir şey kanıtlamazdı.
    expect((snapshot['counterparties'] as List).isNotEmpty, isTrue);
    expect((snapshot['counterpartyCharges'] as List).isNotEmpty, isTrue);
    expect((snapshot['counterpartyPayments'] as List).isNotEmpty, isTrue);
    // Sözleşme karşı tarafını adla değil kimlikle gösteriyor.
    for (final debt in snapshot['debts'] as List) {
      final item = debt as Map<String, dynamic>;
      expect(item.containsKey('counterpartyId'), isTrue);
      expect(item.containsKey('counterpartyName'), isFalse);
    }

    final ledger = await trader.client.getBytes(
      '/api/v1/exports/counterparty-ledger.csv',
    );
    final lines = const LineSplitter()
        .convert(utf8.decode(ledger.bytes).replaceFirst('﻿', ''))
        .where((line) => line.trim().isNotEmpty)
        .toList(growable: false);
    final header = lines.first.split(',');
    expect(header, contains('counterpartyName'));
    expect(lines.length, greaterThan(1));

    final kind = header.indexOf('kind');
    final accountName = header.indexOf('accountName');
    final categoryName = header.indexOf('categoryName');
    final scope = header.indexOf('scope');
    final rows = lines.skip(1).map((line) => line.split(',')).toList();

    // Borçlandırma tanır: kategori ve kapsam dolu, hesap boş.
    final charge = rows.firstWhere((row) => row[kind] == 'charge');
    expect(charge[accountName], isEmpty);
    expect(charge[categoryName], isNotEmpty);
    expect(charge[scope], anyOf('business', 'personal'));

    // Tahsilat taşır: hesap dolu, kategori ve kapsam boş.
    final payment = rows.firstWhere((row) => row[kind] == 'payment');
    expect(payment[accountName], isNotEmpty);
    expect(payment[categoryName], isEmpty);
    expect(payment[scope], isEmpty);
  });
}

Future<_Repositories> _signIn(
  String email,
  String password,
  http.Client httpClient,
) async {
  final auth = AuthRepository(
    remoteService: ApiAuthService(
      ApiClient(config: ApiConfig.fromEnvironment(), httpClient: httpClient),
    ),
    sessionStore: _MemorySessionStore(),
  );
  await auth.login(email, password);
  return _Repositories(auth, httpClient);
}

String _dateText(DateTime value) {
  final month = value.month.toString().padLeft(2, '0');
  final day = value.day.toString().padLeft(2, '0');
  return '${value.year}-$month-$day';
}

/// İki dört ondalıklı tutarın farkı, yine dört ondalıklı.
///
/// Sunucudan gelen metin üzerinde çalışıyor: çift duyarlıklı sayıya çevirip
/// çıkarmak, kuruşu olan tutarlarda kabul turunu gerçek olmayan bir farkla
/// düşürürdü.
String _delta(String after, String before) {
  final value = _cents(after) - _cents(before);
  final sign = value < 0 ? '-' : '';
  final absolute = value.abs();
  final whole = absolute ~/ 10000;
  final fraction = (absolute % 10000).toString().padLeft(4, '0');
  return '$sign$whole.$fraction';
}

int _cents(String amount) {
  final parts = amount.split('.');
  final whole = int.parse(parts[0]);
  final fraction = int.parse(
    parts.length > 1 ? parts[1].padRight(4, '0') : '0',
  );
  return whole.isNegative ? whole * 10000 - fraction : whole * 10000 + fraction;
}

String _balanceOf(dynamic report, String accountId) =>
    report.accountBalances
            .firstWhere((dynamic item) => item.accountId == accountId)
            .balance
        as String;

class _Repositories {
  _Repositories(AuthRepository auth, http.Client httpClient)
    : client = ApiClient(
        config: ApiConfig.fromEnvironment(),
        httpClient: httpClient,
        accessTokenProvider: auth.getValidAccessToken,
      ) {
    accounts = ApiAccountRepository(client);
    categories = ApiCategoryRepository(client);
    counterparties = CounterpartyRepository(client);
    dashboard = DashboardRepository(client);
    activities = ActivityRepository(client);
  }

  final ApiClient client;
  late final AccountRepository accounts;
  late final CategoryRepository categories;
  late final CounterpartyRepositoryContract counterparties;
  late final DashboardDataSource dashboard;
  late final ActivityRepositoryContract activities;

  Future<BudgetCategory> categoryNamed(String name, String type) async {
    final all = await categories.list(type: type, isActive: true);
    return all.firstWhere(
      (item) => item.name == name,
      orElse: () => throw StateError(
        'Varsayılan sette "$name" yok: ${all.map((item) => item.name).join(', ')}',
      ),
    );
  }

  /// Oluşturma ucu kimlik döndürmüyor; kişi listeden adıyla bulunuyor.
  Future<CounterpartySummary> counterpartyNamed(String name) async {
    final snapshot = await counterparties.load(
      CounterpartyBalanceFilter.all,
      _dateText(DateTime.now()),
    );
    return snapshot.counterparties.firstWhere(
      (item) => item.name == name,
      orElse: () => throw StateError('Karşı taraf bulunamadı: $name'),
    );
  }
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
