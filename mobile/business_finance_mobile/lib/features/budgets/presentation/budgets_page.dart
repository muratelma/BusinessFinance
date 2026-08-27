import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_month_picker.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/budget_models.dart';
import '../data/budget_repository.dart';
import 'budgets_controller.dart';

class BudgetsPage extends StatefulWidget {
  const BudgetsPage({
    super.key,
    this.controller,
    this.repository,
    this.changes,
    this.scopeController,
  });
  final BudgetsController? controller;
  final BudgetRepositoryContract? repository;
  final FinancialDataChanges? changes;

  /// Yalnız kapsam boyutunun görünürlüğü için; liste kapsamla filtrelenmez.
  final ScopeController? scopeController;

  @override
  State<BudgetsPage> createState() => _BudgetsPageState();
}

class _BudgetsPageState extends State<BudgetsPage> {
  BudgetsController? _controller;
  bool _ownsController = false;

  @override
  void initState() {
    super.initState();
    _controller = widget.controller;
    if (_controller == null && widget.repository != null) {
      _controller = BudgetsController(
        widget.repository!,
        changes: widget.changes,
        scopeController: widget.scopeController,
      );
      _ownsController = true;
    }
    _controller?.addListener(_changed);
    WidgetsBinding.instance.addPostFrameCallback((_) => _controller?.load());
  }

  @override
  void dispose() {
    _controller?.removeListener(_changed);
    if (_ownsController) _controller?.dispose();
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final controller = _controller;
    if (controller == null) {
      return const AppErrorView(message: 'Bütçe servisi yapılandırılmadı.');
    }
    return Scaffold(
      appBar: AppBar(
        // Başlıkta kapsam yazmıyor çünkü bu liste **bölünmüyor**: iki tarafın
        // sınırı da aynı listede duruyor ve her satır kendi kapsamını yazıyor.
        title: const Text('Aylık bütçeler'),
        // Sayfaya özel ekleme eylemi başlıkta durur; ekranın altındaki
        // ortalanmış buton uygulamanın genel "işlem ekle" eylemine ayrılmıştır
        // ve iki kayan buton aynı ekranda birbiriyle yarışıyordu.
        // Sade bir ikon başlık çubuğunda kayboluyordu: aynı renkte, aynı
        // ağırlıkta, çerçevesiz. Dolgulu bir buton onu bir eylem gibi
        // gösteriyor. Etiket de eklendi — ikon tek başına "neyi ekliyorum"
        // sorusunu cevaplamıyordu ve tooltip yalnız uzun basınca çıkıyor.
        actions: [
          FilledButton.tonalIcon(
            onPressed: controller.isSubmitting
                ? null
                : () => _showCreate(controller),
            icon: const Icon(Icons.add),
            label: const Text('Bütçe'),
          ),
          PopupMenuButton<_BudgetsMenuAction>(
            tooltip: 'Bütçe seçenekleri',
            enabled: !controller.isSubmitting,
            onSelected: (action) => switch (action) {
              _BudgetsMenuAction.copyPreviousMonth => _copyPreviousMonth(
                controller,
              ),
            },
            itemBuilder: (_) => const [
              PopupMenuItem(
                value: _BudgetsMenuAction.copyPreviousMonth,
                child: Text('Geçen ayın bütçelerini kopyala'),
              ),
            ],
          ),
        ],
      ),
      body: Column(
        children: [
          _MonthSelector(
            controller: controller,
            onPickMonth: () => _pickMonth(controller),
          ),
          if (controller.successMessage != null)
            MaterialBanner(
              content: Text(controller.successMessage!),
              actions: [
                TextButton(
                  onPressed: controller.clearMessage,
                  child: const Text('Kapat'),
                ),
              ],
            ),
          if (controller.errorMessage != null && controller.items != null)
            MaterialBanner(
              content: Text(controller.errorMessage!),
              actions: [
                TextButton(
                  onPressed: controller.clearMessage,
                  child: const Text('Kapat'),
                ),
              ],
            ),
          Expanded(child: _body(controller)),
        ],
      ),
    );
  }

  Widget _body(BudgetsController controller) {
    if (controller.unauthorized) {
      return const AppUnauthorizedView();
    }
    if (controller.isLoading && controller.items == null) {
      return const AppLoadingView(message: 'Bütçeler yükleniyor');
    }
    if (controller.errorMessage != null && controller.items == null) {
      return AppErrorView(
        message: controller.errorMessage!,
        onRetry: controller.load,
      );
    }
    final items = controller.items ?? const <BudgetItem>[];
    if (items.isEmpty) {
      return RefreshIndicator(
        onRefresh: controller.load,
        child: ListView(
          children: [
            SizedBox(
              height: 440,
              child: AppEmptyView(
                icon: Icons.donut_small_outlined,
                title: 'Bu ay için bütçe yok',
                message:
                    'Bir gider kategorisine aylık limit belirleyebilirsiniz.',
                // Boş ekran çıkmaz sokak olmasın: ayın ilkinde asıl istenen
                // şey sıfırdan kurmak değil, geçen ayki kararı sürdürmektir.
                action: TextButton.icon(
                  onPressed: controller.isSubmitting
                      ? null
                      : () => _copyPreviousMonth(controller),
                  icon: const Icon(Icons.content_copy_outlined),
                  label: const Text('Geçen ayın bütçelerini kopyala'),
                ),
              ),
            ),
          ],
        ),
      );
    }
    return RefreshIndicator(
      onRefresh: controller.load,
      child: ListView.separated(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.small,
          AppSpacing.medium,
          AppSpacing.fabClearance,
        ),
        itemCount: items.length,
        separatorBuilder: (_, _) => const SizedBox(height: AppSpacing.small),
        itemBuilder: (context, index) => _BudgetCard(
          item: items[index],
          busy: controller.isSubmitting,
          showScope: controller.isScopeVisible,
          onEdit: () => _showUpdate(controller, items[index]),
          onDelete: () => _confirmDelete(controller, items[index]),
          onShowSpending: () => _showSpending(controller, items[index]),
        ),
      ),
    );
  }

  Future<void> _pickMonth(BudgetsController controller) async {
    final picked = await AppMonthPicker.show(
      context: context,
      initialMonth: controller.selectedMonth,
    );
    if (picked == null) return;
    await controller.selectMonth(picked);
  }

  Future<void> _copyPreviousMonth(BudgetsController controller) =>
      controller.copyFromPreviousMonth();

  /// Harcanan tutarın arkasındaki satırlar.
  ///
  /// `1.500 / 2.000` bir sonuçtur; kullanıcının sorduğu soru o sonucun
  /// nereden geldiğidir ve ekranda bunu söyleyen hiçbir şey yoktu.
  Future<void> _showSpending(BudgetsController controller, BudgetItem item) =>
      AppAdaptiveSheet.show<void>(
        context: context,
        builder: (_) => _SpendingSheet(controller: controller, item: item),
      );

  Future<void> _confirmDelete(
    BudgetsController controller,
    BudgetItem item,
  ) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      title: 'Bütçeyi sil',
      highlight:
          '${item.categoryName} · ${_monthText(controller.selectedMonth)}',
      // Silmenin ne yaptığını ve **ne yapmadığını** birlikte söylüyor: bütçe
      // bir sınırdır, harcama değil. Kullanıcının burada kaybetmekten korktuğu
      // şey kayıtlarıdır ve onlara dokunulmuyor.
      message:
          'Bu aylık limit kaldırılır. Harcamalarınız olduğu gibi kalır; '
          'yalnız bu kategorinin sınırı silinir.',
      confirmLabel: 'Sil',
      icon: Icons.delete_outline,
      destructive: true,
    );
    if (!confirmed) return;
    await controller.delete(item.id);
  }

  Future<void> _showCreate(BudgetsController controller) async {
    final input = await AppFormSheet.show<CreateBudgetInput>(
      context: context,
      builder: (_) => _CreateBudgetDialog(controller: controller),
    );
    if (input != null) await controller.create(input);
  }

  Future<void> _showUpdate(
    BudgetsController controller,
    BudgetItem item,
  ) async {
    final limit = await AppFormSheet.show<String>(
      context: context,
      builder: (_) => _UpdateBudgetDialog(item: item),
    );
    if (limit != null) await controller.update(item.id, limit);
  }
}

enum _BudgetsMenuAction { copyPreviousMonth }

class _MonthSelector extends StatelessWidget {
  const _MonthSelector({required this.controller, required this.onPickMonth});
  final BudgetsController controller;
  final VoidCallback onPickMonth;

  @override
  Widget build(BuildContext context) => Semantics(
    container: true,
    label: 'Seçili bütçe ayı ${_monthText(controller.selectedMonth)}',
    child: Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        IconButton(
          tooltip: 'Önceki ay',
          onPressed: controller.isLoading
              ? null
              : () => controller.changeMonth(-1),
          icon: const Icon(Icons.chevron_left),
        ),
        // En büyük yazı ölçeğinde ay adı iki ok butonu arasına sığmıyor ve
        // satırı taşırıyordu; esnek olduğu için artık daralıyor.
        // Ayın kendisi de bir eylem: oklar komşu ay içindir, uzağa gitmek
        // için dönem seçici açılır.
        Flexible(
          child: TextButton(
            onPressed: controller.isLoading ? null : onPickMonth,
            child: Text(
              _monthText(controller.selectedMonth),
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.titleMedium,
            ),
          ),
        ),
        IconButton(
          tooltip: 'Sonraki ay',
          onPressed: controller.isLoading
              ? null
              : () => controller.changeMonth(1),
          icon: const Icon(Icons.chevron_right),
        ),
      ],
    ),
  );
}

class _BudgetCard extends StatelessWidget {
  const _BudgetCard({
    required this.item,
    required this.busy,
    required this.showScope,
    required this.onEdit,
    required this.onDelete,
    required this.onShowSpending,
  });
  final BudgetItem item;
  final bool busy;
  final bool showScope;
  final VoidCallback onEdit;
  final VoidCallback onDelete;
  final VoidCallback onShowSpending;

  @override
  Widget build(BuildContext context) {
    final limit = MoneyText.format(item.limit, item.currency);
    final spent = MoneyText.format(item.spent, item.currency);
    final remaining = MoneyText.format(item.remaining, item.currency);
    final exceeded = MoneyText.format(item.exceeded, item.currency);
    final financeColors = AppFinanceColors.of(context);
    final percent = item.spentPercent;
    // Kart tek cümlede duyurulur, fakat eylem menüsü ağaçta kalır: bütün
    // kartı `ExcludeSemantics` ile sarmak menünün erişilebilirliğini de
    // götürüyordu.
    return Semantics(
      container: true,
      label:
          '${item.categoryName} bütçesi.'
          '${showScope ? ' Kapsam ${item.scope.label}.' : ''}'
          ' Limit $limit. Harcanan $spent.'
          '${item.isExceeded ? ' Limit $exceeded aşıldı.' : ' Kalan $remaining.'}',
      child: AppCard(
        onTap: onShowSpending,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Expanded(
                  child: ExcludeSemantics(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          item.categoryName,
                          style: Theme.of(context).textTheme.titleMedium,
                        ),
                        // Bütçe **kategori + kapsam çiftini** sınırlar; hangi
                        // tarafı sınırladığı yazmazsa, öteki tarafın harcaması
                        // sessizce sayılmıyor gibi görünür.
                        if (showScope)
                          Text(
                            item.scope.label,
                            style: Theme.of(context).textTheme.bodySmall
                                ?.copyWith(
                                  color: Theme.of(
                                    context,
                                  ).colorScheme.onSurfaceVariant,
                                ),
                          ),
                      ],
                    ),
                  ),
                ),
                PopupMenuButton<_BudgetRowAction>(
                  tooltip: 'Bütçe eylemleri',
                  enabled: !busy,
                  onSelected: (action) => switch (action) {
                    _BudgetRowAction.edit => onEdit(),
                    _BudgetRowAction.delete => onDelete(),
                    _BudgetRowAction.showSpending => onShowSpending(),
                  },
                  itemBuilder: (_) => [
                    const PopupMenuItem(
                      value: _BudgetRowAction.edit,
                      child: Text('Limiti düzenle'),
                    ),
                    const PopupMenuItem(
                      value: _BudgetRowAction.showSpending,
                      child: Text('Harcamaları gör'),
                    ),
                    const PopupMenuItem(
                      value: _BudgetRowAction.delete,
                      child: Text('Bütçeyi sil'),
                    ),
                  ],
                ),
              ],
            ),
            ExcludeSemantics(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  const SizedBox(height: AppSpacing.small),
                  // Harcanan tutar bütçenin asıl sayısıdır; limit onun yanında
                  // bağlam olarak durur.
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.baseline,
                    textBaseline: TextBaseline.alphabetic,
                    children: [
                      Flexible(
                        child: AppMoneyText(
                          amount: item.spent,
                          currency: item.currency,
                          effect: item.isExceeded
                              ? AppMoneyEffect.expense
                              : null,
                          style: Theme.of(context).textTheme.titleLarge,
                        ),
                      ),
                      const SizedBox(width: AppSpacing.xSmall),
                      Flexible(
                        child: Text(
                          '/ $limit',
                          style: Theme.of(context).textTheme.bodySmall,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: AppSpacing.small),
                  ClipRRect(
                    borderRadius: BorderRadius.circular(AppRadius.chip),
                    child: LinearProgressIndicator(
                      value: item.progress,
                      minHeight: 8,
                      // Marka rengi akromatik olduğu için burada neredeyse
                      // siyah bir şerit çiziliyordu. İlerleme bir grafik
                      // dolgusudur; rengi de dolgu token'ından gelir.
                      //
                      // Eşiği geçmiş bütçe **gider rengine dönmez**: uyarı ile
                      // aşım aynı görünseydi ikisini ayırt eden tek şey metin
                      // kalır ve renk, aşılmamış bir sınır için alarm verirdi.
                      color: item.isExceeded
                          ? financeColors.expenseFill
                          : financeColors.neutralFill,
                    ),
                  ),
                  const SizedBox(height: AppSpacing.small),
                  // Aşım rengin yanında rozetle de bildirilir; renk tek başına
                  // taşımaz.
                  if (item.isExceeded)
                    Align(
                      alignment: Alignment.centerLeft,
                      child: AppStatusChip(
                        label: 'Limit $exceeded aşıldı',
                        icon: Icons.warning_amber_rounded,
                        tone: AppStatusTone.expense,
                      ),
                    )
                  // Aşılmadan önce de konuşur. Bütçenin işi ayın sonunda kötü
                  // haberi bildirmek değil, ay sürerken uyarmaktır.
                  else if (item.isNearLimit && percent != null)
                    Align(
                      alignment: Alignment.centerLeft,
                      child: AppStatusChip(
                        label:
                            'Limitin yüzde $percent kadarı harcandı · '
                            'kalan $remaining',
                        icon: Icons.info_outline,
                        tone: AppStatusTone.planned,
                      ),
                    )
                  else
                    Text(
                      'Kalan: $remaining',
                      style: Theme.of(context).textTheme.bodySmall,
                    ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

enum _BudgetRowAction { edit, showSpending, delete }

class _CreateBudgetDialog extends StatefulWidget {
  const _CreateBudgetDialog({required this.controller});
  final BudgetsController controller;

  @override
  State<_CreateBudgetDialog> createState() => _CreateBudgetDialogState();
}

class _CreateBudgetDialogState extends State<_CreateBudgetDialog> {
  final _formKey = GlobalKey<FormState>();
  final _limit = TextEditingController();
  late Future<List<BudgetCategory>> _categories;
  List<BudgetCategory> _available = const [];
  String? _categoryId;
  TransactionScope? _explicitScope;
  bool _scopeMissing = false;

  @override
  void initState() {
    super.initState();
    _categories = widget.controller.loadCategories();
  }

  @override
  void dispose() {
    _limit.dispose();
    super.dispose();
  }

  bool get _showScope => widget.controller.isScopeVisible;

  TransactionScope? get _categoryScope {
    for (final category in _available) {
      if (category.id == _categoryId) return category.defaultScope;
    }
    return null;
  }

  /// Zincirin istemcideki önizlemesi. Bütçenin hesabı yoktur; ara halka
  /// yalnız kategorinin varsayılanıdır.
  TransactionScope? get _resolvedScope =>
      previewResolvedScope(explicit: _explicitScope, category: _categoryScope);

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<CreateBudgetInput>(
      title: '${_monthText(widget.controller.selectedMonth)} bütçesi',
      description: 'Limit bu aya özeldir; sonraki ay kendi limitini alır.',
      submitLabel: 'Oluştur',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        if (_showScope && _resolvedScope == null) {
          setState(() => _scopeMissing = true);
          return null;
        }
        final month = widget.controller.selectedMonth;
        return CreateBudgetInput(
          categoryId: _categoryId!,
          limit: MoneyText.normalizeInput(_limit.text)!,
          year: month.year,
          month: month.month,
          scope: _showScope ? _resolvedScope : null,
        );
      },
      children: [
        AppFormField(
          child: FutureBuilder<List<BudgetCategory>>(
            future: _categories,
            builder: (context, snapshot) {
              if (snapshot.connectionState != ConnectionState.done) {
                return const LinearProgressIndicator();
              }
              if (snapshot.hasError) {
                return const Text('Kategoriler yüklenemedi.');
              }
              // O ay bütçesi olan kategori listeden düşer: tekil indeks
              // ikincisini reddediyor ve seçilebilir bırakmak kullanıcıyı
              // sunucunun çakışma cevabına götürürdü.
              final budgeted = widget.controller.budgetedCategoryIds;
              _available = [
                for (final category
                    in snapshot.data ?? const <BudgetCategory>[])
                  if (category.isActive && !budgeted.contains(category.id))
                    category,
              ];
              if (_available.isEmpty) {
                return const Text(
                  'Bu ayda bütçesi olmayan gider kategoriniz kalmadı.',
                );
              }
              return DropdownButtonFormField<String>(
                initialValue: _categoryId,
                isExpanded: true,
                decoration: const InputDecoration(
                  labelText: 'Gider kategorisi',
                ),
                items: _available
                    .map(
                      (category) => DropdownMenuItem(
                        value: category.id,
                        child: Text(category.name),
                      ),
                    )
                    .toList(growable: false),
                onChanged: (value) => setState(() {
                  _categoryId = value;
                  _scopeMissing = false;
                }),
                validator: (value) => value == null ? 'Kategori seçin.' : null,
              );
            },
          ),
        ),
        if (_showScope)
          AppFormField(
            child: AppScopeField(
              value: _resolvedScope,
              onChanged: (scope) => setState(() {
                _explicitScope = scope;
                _scopeMissing = false;
              }),
              helperText: _explicitScope == null
                  ? 'Kategorinin varsayılanından önerildi; değiştirebilirsiniz.'
                  : 'Sizin seçiminiz; kategori varsayılanını ezer.',
              errorText: _scopeMissing
                  ? 'Bütçenin hangi tarafı sınırladığını seçin.'
                  : null,
            ),
          ),
        AppFormField(
          child: TextFormField(
            controller: _limit,
            decoration: const InputDecoration(
              labelText: 'Aylık limit',
              suffixText: 'TRY',
            ),
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            validator: _budgetMoneyError,
          ),
        ),
      ],
    ),
  );
}

class _UpdateBudgetDialog extends StatefulWidget {
  const _UpdateBudgetDialog({required this.item});
  final BudgetItem item;

  @override
  State<_UpdateBudgetDialog> createState() => _UpdateBudgetDialogState();
}

class _UpdateBudgetDialogState extends State<_UpdateBudgetDialog> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _limit;

  @override
  void initState() {
    super.initState();
    // Sunucu dört ondalıklı gönderir; alan onu ham hâliyle gösterirse
    // kullanıcı `2000.0000` okur. `editable` yalnız anlamsız sıfırları atar,
    // yuvarlamaz — düzenlenen değer sunucudaki değerin aynısıdır.
    _limit = TextEditingController(text: MoneyText.editable(widget.item.limit));
  }

  @override
  void dispose() {
    _limit.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<String>(
      title: '${widget.item.categoryName} limitini güncelle',
      submitLabel: 'Güncelle',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        return MoneyText.normalizeInput(_limit.text)!;
      },
      children: [
        AppFormField(
          child: TextFormField(
            controller: _limit,
            autofocus: true,
            decoration: const InputDecoration(
              labelText: 'Yeni limit',
              suffixText: 'TRY',
            ),
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            validator: _budgetMoneyError,
          ),
        ),
      ],
    ),
  );
}

String? _budgetMoneyError(String? value) {
  final normalized = MoneyText.normalizeInput(value ?? '');
  if (normalized == null || RegExp(r'^0+(\.0{1,4})?$').hasMatch(normalized)) {
    return 'Sıfırdan büyük bir limit girin (en fazla 4 ondalık).';
  }
  return null;
}

String _monthText(DateTime value) =>
    DateText.monthYear(value.year, value.month);

/// Harcanan tutarın dökümü.
///
/// Bütçe ekranının burada sorduğu soru dar: "bu toplam nereden geldi". İptal
/// ve ek işlemler feed'in kendi ekranına ait; buraya taşınsaydı bütçe ekranı
/// ikinci bir işlem listesi olurdu.
class _SpendingSheet extends StatefulWidget {
  const _SpendingSheet({required this.controller, required this.item});

  final BudgetsController controller;
  final BudgetItem item;

  @override
  State<_SpendingSheet> createState() => _SpendingSheetState();
}

class _SpendingSheetState extends State<_SpendingSheet> {
  late Future<List<BudgetSpendingLine>> _lines = _load();

  Future<List<BudgetSpendingLine>> _load() =>
      widget.controller.loadSpending(widget.item);

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.all(AppSpacing.medium),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Semantics(
            header: true,
            child: Text(
              '${widget.item.categoryName} harcamaları',
              style: theme.textTheme.titleMedium,
            ),
          ),
          const SizedBox(height: AppSpacing.xSmall),
          Text(
            widget.controller.isScopeVisible
                ? '${_monthText(DateTime(widget.item.year, widget.item.month))}'
                      ' · ${widget.item.scope.label}'
                : _monthText(DateTime(widget.item.year, widget.item.month)),
            style: theme.textTheme.bodySmall?.copyWith(
              color: theme.colorScheme.onSurfaceVariant,
            ),
          ),
          const SizedBox(height: AppSpacing.medium),
          FutureBuilder<List<BudgetSpendingLine>>(
            future: _lines,
            builder: (context, snapshot) {
              if (snapshot.connectionState != ConnectionState.done) {
                return const Padding(
                  padding: EdgeInsets.symmetric(vertical: AppSpacing.large),
                  child: AppLoadingView(message: 'Harcamalar yükleniyor'),
                );
              }
              if (snapshot.hasError) {
                return AppErrorView(
                  message: 'Harcamalar okunamadı.',
                  onRetry: () => setState(() => _lines = _load()),
                );
              }
              final lines = snapshot.data ?? const <BudgetSpendingLine>[];
              if (lines.isEmpty) {
                return Text(
                  'Bu ay bu bütçeye düşen harcama yok.',
                  style: theme.textTheme.bodyMedium,
                );
              }
              return Flexible(
                child: ListView.separated(
                  shrinkWrap: true,
                  itemCount: lines.length,
                  separatorBuilder: (_, _) => const Divider(height: 1),
                  itemBuilder: (context, index) {
                    final line = lines[index];
                    return ListTile(
                      contentPadding: EdgeInsets.zero,
                      title: Text(line.title),
                      subtitle: Text(
                        line.sourceName == null
                            ? DateText.dayMonth(line.date)
                            : '${DateText.dayMonth(line.date)} · '
                                  '${line.sourceName}',
                      ),
                      trailing: AppMoneyText(
                        amount: line.amount,
                        currency: line.currency,
                        effect: AppMoneyEffect.expense,
                      ),
                    );
                  },
                ),
              );
            },
          ),
        ],
      ),
    );
  }
}
