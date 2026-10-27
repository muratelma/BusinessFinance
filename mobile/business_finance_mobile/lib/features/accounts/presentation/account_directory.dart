import 'package:flutter/widgets.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/widgets/app_account_field.dart';
import '../../cards/data/finance_models.dart';
import '../data/account_models.dart';
import '../../finance_pages/data/finance_page_models.dart';

/// Hesap seçicinin satır bilgisi: tür, kapsam, bakiye ve kart borcu.
///
/// Her form kendi seçeneklerini (hangi hesaplar, hangi sırayla) bilir; satırın
/// görünüşü için gereken bilgi ise her yerde aynıdır. Hesaplar sayfasının
/// okuması (`/accounts/overview`) ve kart listesi bir kez okunur, hesap ya da
/// kart değişince yeniden. Bu yalnız gösterimdir: okuma başarısız olursa
/// seçenek adıyla ve formun bildiği türle çizilir, seçim etkilenmez.
class AccountDirectory extends ChangeNotifier {
  AccountDirectory({
    required this.loadAccounts,
    required this.loadCards,
    FinancialDataChanges? changes,
    Listenable? session,
  }) : _changes = changes,
       _session = session {
    _accountsRevision = changes?.accountsRevision ?? 0;
    _cardsRevision = changes?.cardsRevision ?? 0;
    changes?.addListener(_onChanges);
    session?.addListener(clear);
  }

  factory AccountDirectory.fromApi(
    ApiClient client, {
    FinancialDataChanges? changes,
    Listenable? session,
  }) => AccountDirectory(
    loadAccounts: () async => AccountsOverviewData.fromJson(
      (await client.get('/api/v1/accounts/overview')).requireObject(),
    ).items,
    loadCards: () async {
      final items = (await client.get(
        '/api/v1/credit-cards',
      )).requireObject()['items'];
      return [
        for (final item in items is List ? items : const [])
          if (item is Map<String, dynamic>) CreditCardItem.fromJson(item),
      ];
    },
    changes: changes,
    session: session,
  );

  final Future<List<AccountOverviewItem>> Function() loadAccounts;
  final Future<List<CreditCardItem>> Function() loadCards;
  final FinancialDataChanges? _changes;
  final Listenable? _session;
  Map<String, AccountOverviewItem> _accounts = const {};
  Map<String, CreditCardItem> _cards = const {};
  bool _loaded = false;
  bool _loading = false;
  int _accountsRevision = 0;
  int _cardsRevision = 0;

  /// İlk seçici çizildiğinde bir kez okur; yarışan çağrılar tek okumadır.
  void ensureLoaded() {
    if (_loaded || _loading) return;
    _reload();
  }

  Future<void> _reload() async {
    _loading = true;
    try {
      final results = await Future.wait([loadAccounts(), loadCards()]);
      _accounts = {
        for (final a in results[0] as List<AccountOverviewItem>) a.id: a,
      };
      _cards = {for (final c in results[1] as List<CreditCardItem>) c.id: c};
      _loaded = true;
      notifyListeners();
    } on Object {
      // Satır bilgisi olmadan da seçim yapılabilir; bir sonraki değişiklikte
      // yeniden denenir.
      _loaded = true;
    } finally {
      _loading = false;
    }
  }

  void _onChanges() {
    final changes = _changes!;
    if (changes.accountsRevision == _accountsRevision &&
        changes.cardsRevision == _cardsRevision) {
      return;
    }
    _accountsRevision = changes.accountsRevision;
    _cardsRevision = changes.cardsRevision;
    if (_loaded) _reload();
  }

  /// Oturum değişince önceki kullanıcının hesapları unutulur.
  void clear() {
    _accounts = const {};
    _cards = const {};
    _loaded = false;
    notifyListeners();
  }

  AccountOverviewItem? accountOf(String id) => _accounts[id];
  CreditCardItem? cardOf(String id) => _cards[id];

  @override
  void dispose() {
    _changes?.removeListener(_onChanges);
    _session?.removeListener(clear);
    super.dispose();
  }
}

/// Bir ekranın seçeneklerini rehberle tamamlar.
///
/// ```dart
/// final choices = AccountChoices.of(context);
/// AppAccountField(options: [for (final a in accounts) choices.account(a.id, a.name)], …)
/// ```
class AccountChoices {
  const AccountChoices._(this._directory, this._showScope);

  /// Rehber yoksa (testler, ön izlemeler) seçenekler yalnız adla çizilir.
  factory AccountChoices.of(BuildContext context) {
    final directory = _watch(context);
    directory?.ensureLoaded();
    bool showScope;
    try {
      showScope = context.watch<ScopeController?>()?.isVisible ?? false;
    } on ProviderNotFoundException {
      showScope = false;
    }
    return AccountChoices._(directory, showScope);
  }

  static AccountDirectory? _watch(BuildContext context) {
    try {
      return context.watch<AccountDirectory?>();
    } on ProviderNotFoundException {
      return null;
    }
  }

  final AccountDirectory? _directory;
  final bool _showScope;

  /// [type], [scope] ve [balance] rehberde yoksa formun bildiğidir.
  AppAccountOption account(
    String id,
    String name, {
    String? type,
    TransactionScope? scope,
    String? balance,
    String? disabledNote,
  }) {
    final known = _directory?.accountOf(id);
    final kind = (known?.type ?? type) == 'cash'
        ? AppAccountKind.cash
        : AppAccountKind.bank;
    return AppAccountOption(
      id: id,
      name: name,
      kind: kind,
      scope: _scope(known?.defaultScope ?? scope),
      amount: known?.balance ?? balance,
      disabledNote: disabledNote,
    );
  }

  /// Kartın tutarı borcudur ve eksi yazılır.
  AppAccountOption card(String id, String name, {TransactionScope? scope}) {
    final known = _directory?.cardOf(id);
    final debt = known?.currentDebt;
    return AppAccountOption(
      id: id,
      name: name,
      kind: AppAccountKind.card,
      scope: _scope(known?.defaultScope ?? scope),
      amount: debt == null
          ? null
          : RegExp(r'^-?0+\.0+$').hasMatch(debt)
          ? '0.0000'
          : debt.startsWith('-')
          ? debt.substring(1)
          : '-$debt',
    );
  }

  /// POS: alt yazı paranın geçeceği hesaptır.
  AppAccountOption pos(String id, String name, {String? accountName}) =>
      AppAccountOption(
        id: id,
        name: name,
        kind: AppAccountKind.pos,
        note: accountName,
      );

  String? _scope(TransactionScope? scope) => _showScope ? scope?.label : null;
}

/// `Hesap ekle`: ekleme formu doğrudan açılır (Hesaplar sayfası araya girmez).
const accountCreateLocation = '$accountsLocation/new';

/// Formdaki `Hesap ekle`. Kaydedilirse yeni hesap döner ve form onu seçer:
/// listesi denetleyiciden gelen form listeyi yeniden okur, listesi dışarıdan
/// gelen form hesabı türü uyuyorsa listesine ekler. Vazgeçilirse `null`.
/// [type] ekleme formunun ön değeridir (`bank`: paranın bankaya geçtiği alan).
Future<Account?> addAccountFromForm(
  BuildContext context, {
  String? type,
}) async {
  final result = await context.push<Object?>(
    type == null ? accountCreateLocation : '$accountCreateLocation?type=$type',
  );
  return result is Account ? result : null;
}

/// Listesi dışarıdan gelen formun eklediği hesap.
extension AddedAccountChoice on Account {
  DataChoice get choice =>
      DataChoice(id, name, type: type, defaultScope: defaultScope);
}
