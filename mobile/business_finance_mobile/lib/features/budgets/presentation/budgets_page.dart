import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_finance_icons.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_month_picker.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_share_bar.dart';
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
    // Kişisel profilde ana sekmedir ve geri oku yoktur; işletme profilinde
    // Özet'ten veya `Diğer`'den açılır ve geri döner (ADR 0015).
    final canPop = Navigator.of(context).canPop();
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: Column(
          children: [
            // Başlıkta kapsam yazmıyor çünkü bu liste **bölünmüyor**: iki
            // tarafın sınırı da aynı listede duruyor ve her kart kendi
            // kapsamını yazıyor.
            AppPageHeader(
              title: 'Bütçeler',
              onBack: canPop ? () => Navigator.of(context).maybePop() : null,
              actions: [
                IconButton(
                  tooltip: 'Bütçe ekle',
                  onPressed: controller.isSubmitting
                      ? null
                      : () => _showCreate(controller),
                  icon: const Icon(Icons.add),
                ),
              ],
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
    final month = _MonthSelector(
      controller: controller,
      onPickMonth: () => _pickMonth(controller),
    );
    final items = controller.items ?? const <BudgetItem>[];
    if (items.isEmpty) {
      return RefreshIndicator(
        onRefresh: controller.load,
        child: ListView(
          padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
          children: [
            month,
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
    // En dolu bütçe en üstte: ekranın ilk söylemesi gereken aşılanlardır.
    final sorted = [...items]
      ..sort((a, b) => b.progressRatio.compareTo(a.progressRatio));
    return RefreshIndicator(
      onRefresh: controller.load,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.xSmall,
          AppSpacing.medium,
          AppSpacing.fabClearance,
        ),
        children: [
          month,
          const SizedBox(height: AppSpacing.small),
          for (var i = 0; i < sorted.length; i++) ...[
            if (i > 0) const SizedBox(height: AppSpacing.small),
            _BudgetCard(
              item: sorted[i],
              showScope: controller.isScopeVisible,
              onOpen: () => _showSpending(controller, sorted[i]),
            ),
          ],
          const SizedBox(height: AppSpacing.medium),
          OutlinedButton.icon(
            onPressed: controller.isSubmitting
                ? null
                : () => _copyPreviousMonth(controller),
            icon: const Icon(Icons.content_copy_outlined),
            label: const Text('Geçen ayın bütçelerini kopyala'),
          ),
        ],
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
  ///
  /// Panel bütçenin ayrıntısıdır: limiti düzenleme ve silme de oradadır, kart
  /// yüzeyi tasarımdaki gibi sade kalır.
  Future<void> _showSpending(
    BudgetsController controller,
    BudgetItem item,
  ) async {
    final action = await AppAdaptiveSheet.show<_BudgetRowAction>(
      context: context,
      builder: (_) => _SpendingSheet(controller: controller, item: item),
    );
    if (!mounted || action == null) return;
    switch (action) {
      case _BudgetRowAction.edit:
        await _showUpdate(controller, item);
      case _BudgetRowAction.delete:
        await _confirmDelete(controller, item);
    }
  }

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

class _MonthSelector extends StatelessWidget {
  const _MonthSelector({required this.controller, required this.onPickMonth});
  final BudgetsController controller;
  final VoidCallback onPickMonth;

  @override
  Widget build(BuildContext context) => Semantics(
    container: true,
    label: 'Seçili bütçe ayı ${_monthText(controller.selectedMonth)}',
    child: Row(
      children: [
        // Ayın kendisi de bir eylem: oklar komşu ay içindir, uzağa gitmek
        // için dönem seçici açılır.
        Expanded(
          child: Align(
            alignment: AlignmentDirectional.centerStart,
            child: InkWell(
              onTap: controller.isLoading ? null : onPickMonth,
              borderRadius: BorderRadius.circular(AppRadius.field),
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  vertical: AppSpacing.small + AppSpacing.xSmall,
                ),
                child: Text(
                  _monthText(controller.selectedMonth),
                  style: Theme.of(context).textTheme.titleMedium,
                ),
              ),
            ),
          ),
        ),
        // Okların glifi sayfa kenarına hizalansın diye dokunma alanı sağa
        // taşar.
        Transform.translate(
          offset: const Offset(AppSpacing.small + AppSpacing.xSmall, 0),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              IconButton(
                tooltip: 'Önceki ay',
                onPressed: controller.isLoading
                    ? null
                    : () => controller.changeMonth(-1),
                icon: const Icon(Icons.chevron_left),
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
        ),
      ],
    ),
  );
}

/// Bir bütçe: kategori kapsülü, ad ve kapsam, durum kapsülü; harcanan / limit,
/// dolum çubuğu, yüzde ve kalan (ya da aşım).
///
/// Kalan ve aşım sunucudan gelir (`remaining`, `exceeded`); yüzde ve çubuk
/// yalnız çizim oranıdır.
class _BudgetCard extends StatelessWidget {
  const _BudgetCard({
    required this.item,
    required this.showScope,
    required this.onOpen,
  });
  final BudgetItem item;
  final bool showScope;
  final VoidCallback onOpen;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final limit = MoneyText.format(item.limit, item.currency);
    final spent = MoneyText.format(item.spent, item.currency);
    final remaining = MoneyText.format(item.remaining, item.currency);
    final exceeded = MoneyText.format(item.exceeded, item.currency);
    final over = item.isExceeded;
    final percent = item.spentPercent;
    final tone = over ? colors.expense : surfaces.inkMuted;
    final footStyle = theme.textTheme.labelMedium?.copyWith(
      letterSpacing: 0,
      color: tone,
    );

    return Semantics(
      container: true,
      button: true,
      onTap: onOpen,
      label:
          '${item.categoryName} bütçesi.'
          '${showScope ? ' Kapsam ${item.scope.label}.' : ''}'
          ' Limit $limit. Harcanan $spent.'
          '${over ? ' Limit $exceeded aşıldı.' : ' Kalan $remaining.'}'
          '${!over && item.isNearLimit && percent != null ? ' Limitin yüzde $percent kadarı harcandı.' : ''}'
          ' Harcamaları ve eylemleri açar.',
      child: ExcludeSemantics(
        child: AppCard(
          onTap: onOpen,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  AppIconCapsule(
                    icon: AppFinanceIcons.forCategory(item.categoryName),
                    tone: over ? AppStatusTone.expense : null,
                  ),
                  const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          item.categoryName,
                          style: theme.textTheme.titleSmall,
                        ),
                        // Bütçe **kategori + kapsam çiftini** sınırlar; hangi
                        // tarafı sınırladığı yazmazsa öteki tarafın harcaması
                        // sessizce sayılmıyor gibi görünür.
                        if (showScope)
                          Text(
                            item.scope.label,
                            style: theme.textTheme.bodySmall,
                          ),
                      ],
                    ),
                  ),
                  const SizedBox(width: AppSpacing.small),
                  // Aşım rengin yanında rozetle de bildirilir; renk tek
                  // başına taşımaz. Eşiği geçen bütçe aşılmadan önce de
                  // konuşur (Aşama 06 kararı): bütçenin işi ay sürerken
                  // uyarmaktır.
                  switch ((over, item.isNearLimit)) {
                    (true, _) => const AppStatusChip(
                      label: 'Aşıldı',
                      icon: Icons.warning_amber_rounded,
                      tone: AppStatusTone.expense,
                    ),
                    (false, true) => const AppStatusChip(
                      label: 'Limite yakın',
                      icon: Icons.info_outline,
                      tone: AppStatusTone.planned,
                    ),
                    _ => const AppStatusChip(
                      label: 'Limit içinde',
                      icon: Icons.check_circle_outline,
                      tone: AppStatusTone.planned,
                    ),
                  },
                ],
              ),
              const SizedBox(height: AppSpacing.medium),
              // Harcanan tutar bütçenin asıl sayısıdır; limit yanında bağlam.
              Wrap(
                crossAxisAlignment: WrapCrossAlignment.end,
                spacing: AppSpacing.small - AppSpacing.xxSmall,
                children: [
                  AppMoneyText(
                    amount: item.spent,
                    currency: item.currency,
                    size: AppMoneySize.metric,
                    style: const TextStyle(fontWeight: FontWeight.w700),
                  ),
                  Padding(
                    padding: const EdgeInsets.only(bottom: AppSpacing.xSmall),
                    child: Text('/ $limit', style: theme.textTheme.bodySmall),
                  ),
                ],
              ),
              const SizedBox(height: AppSpacing.small),
              // Eşiği geçmiş ama aşmamış bütçe gider rengine dönmez: uyarı ile
              // aşım aynı görünseydi renk, aşılmamış bir sınır için alarm
              // verirdi.
              AppShareBar(
                ratio: item.progress,
                color: over ? colors.expenseFill : colors.neutralFill,
                height: 8,
              ),
              const SizedBox(height: AppSpacing.small),
              Row(
                children: [
                  Expanded(
                    child: Text(
                      percent == null ? '' : '%$percent',
                      style: footStyle,
                    ),
                  ),
                  Text(
                    over ? '$exceeded fazla' : '$remaining kaldı',
                    style: footStyle,
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

enum _BudgetRowAction { edit, delete }

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
          const SizedBox(height: AppSpacing.medium),
          // Limiti düzenlemek ve bütçeyi silmek bütçenin ayrıntısına aittir;
          // kart yüzeyi sade kalır.
          Row(
            children: [
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: widget.controller.isSubmitting
                      ? null
                      : () => Navigator.of(context).pop(_BudgetRowAction.edit),
                  icon: const Icon(Icons.edit_outlined),
                  label: const Text('Limiti düzenle'),
                ),
              ),
              const SizedBox(width: AppSpacing.small),
              Expanded(
                child: TextButton.icon(
                  onPressed: widget.controller.isSubmitting
                      ? null
                      : () =>
                            Navigator.of(context).pop(_BudgetRowAction.delete),
                  style: TextButton.styleFrom(
                    foregroundColor: theme.colorScheme.error,
                  ),
                  icon: const Icon(Icons.delete_outline),
                  label: const Text('Bütçeyi sil'),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
