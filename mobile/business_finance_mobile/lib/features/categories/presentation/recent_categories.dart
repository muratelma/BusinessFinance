import 'package:flutter/widgets.dart';
import 'package:provider/provider.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_client.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/widgets/app_category_field.dart';

/// Kategori seçicinin `Son kullanılanlar` bölümü; sunucudan, her cihazda aynı
/// (kullanıcı kararı, 28 Ekim 2026).
///
/// Her form kendi kategorilerini bilir; son kullanılanlar ise her yerde
/// aynıdır. Tür başına bir kez okunur, akışa bir kayıt yazılınca yeniden.
/// Yalnız gösterimdir: okunamazsa bölüm çizilmez, seçim etkilenmez.
class RecentCategories extends ChangeNotifier {
  RecentCategories({
    required this.load,
    FinancialDataChanges? changes,
    Listenable? session,
  }) : _changes = changes,
       _session = session {
    _revision = changes?.activityFeedRevision ?? 0;
    changes?.addListener(_onChanges);
    session?.addListener(clear);
  }

  factory RecentCategories.fromApi(
    ApiClient client, {
    FinancialDataChanges? changes,
    Listenable? session,
  }) => RecentCategories(
    load: (type) async {
      final items = (await client.get(
        '/api/v1/categories/recent?type=$type',
      )).requireObject()['items'];
      return [
        for (final item in items is List ? items : const [])
          if (item is Map<String, dynamic>)
            AppCategoryValue(
              item['categoryId'] as String,
              TransactionScope.fromApi(item['scope'] as String),
            ),
      ];
    },
    changes: changes,
    session: session,
  );

  /// `expense` ya da `income` için en yeni dört kategori + kapsam çifti.
  final Future<List<AppCategoryValue>> Function(String type) load;
  final FinancialDataChanges? _changes;
  final Listenable? _session;
  final Map<String, List<AppCategoryValue>> _items = {};
  final Set<String> _loading = {};
  int _revision = 0;

  /// İlk seçici çizildiğinde okur; yarışan çağrılar tek okumadır.
  void ensureLoaded(String type) {
    if (_items.containsKey(type) || _loading.contains(type)) return;
    _reload(type);
  }

  Future<void> _reload(String type) async {
    _loading.add(type);
    try {
      _items[type] = await load(type);
      notifyListeners();
    } on Object {
      // Bölüm çizilmez; bir sonraki kayıtta yeniden denenir.
      _items[type] = const [];
    } finally {
      _loading.remove(type);
    }
  }

  void _onChanges() {
    final revision = _changes!.activityFeedRevision;
    if (revision == _revision) return;
    _revision = revision;
    for (final type in _items.keys.toList()) {
      _reload(type);
    }
  }

  /// Oturum değişince önceki kullanıcının kayıtları unutulur.
  void clear() {
    _items.clear();
    notifyListeners();
  }

  List<AppCategoryValue> itemsOf(String type) => _items[type] ?? const [];

  /// Ekranın türü için liste; kaynak yoksa (testler, ön izlemeler) boştur.
  static List<AppCategoryValue> of(BuildContext context, String type) {
    RecentCategories? recent;
    try {
      recent = context.watch<RecentCategories?>();
    } on ProviderNotFoundException {
      return const [];
    }
    recent?.ensureLoaded(type);
    return recent?.itemsOf(type) ?? const [];
  }

  @override
  void dispose() {
    _changes?.removeListener(_onChanges);
    _session?.removeListener(clear);
    super.dispose();
  }
}
