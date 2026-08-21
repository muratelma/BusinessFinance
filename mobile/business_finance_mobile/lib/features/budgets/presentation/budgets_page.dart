import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_money_text.dart';
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
  });
  final BudgetsController? controller;
  final BudgetRepositoryContract? repository;
  final FinancialDataChanges? changes;

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
        title: const Text('Aylık bütçeler'),
        // Sayfaya özel ekleme eylemi başlıkta durur; ekranın altındaki
        // ortalanmış buton uygulamanın genel "işlem ekle" eylemine ayrılmıştır
        // ve iki kayan buton aynı ekranda birbiriyle yarışıyordu.
        // Sade bir ikon başlık çubuğunda kayboluyordu: aynı renkte, aynı
        // ağırlıkta, çerçevesiz. Dolgulu bir buton onu bir eylem gibi
        // gösteriyor. Etiket de eklendi — ikon tek başına "neyi ekliyorum"
        // sorusunu cevaplamıyordu ve tooltip yalnız uzun basınca çıkıyor.
        actions: [
          Padding(
            padding: const EdgeInsets.only(right: AppSpacing.small),
            child: FilledButton.tonalIcon(
              onPressed: controller.isSubmitting
                  ? null
                  : () => _showCreate(controller),
              icon: const Icon(Icons.add),
              label: const Text('Bütçe'),
            ),
          ),
        ],
      ),
      body: Column(
        children: [
          _MonthSelector(controller: controller),
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
          children: const [
            SizedBox(
              height: 440,
              child: AppEmptyView(
                icon: Icons.donut_small_outlined,
                title: 'Bu ay için bütçe yok',
                message:
                    'Bir gider kategorisine aylık limit belirleyebilirsiniz.',
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
          onEdit: () => _showUpdate(controller, items[index]),
        ),
      ),
    );
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
  const _MonthSelector({required this.controller});
  final BudgetsController controller;

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
        Flexible(
          child: Text(
            _monthText(controller.selectedMonth),
            textAlign: TextAlign.center,
            style: Theme.of(context).textTheme.titleMedium,
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
    required this.onEdit,
  });
  final BudgetItem item;
  final bool busy;
  final VoidCallback onEdit;

  @override
  Widget build(BuildContext context) {
    final limit = MoneyText.format(item.limit, item.currency);
    final spent = MoneyText.format(item.spent, item.currency);
    final remaining = MoneyText.format(item.remaining, item.currency);
    final exceeded = MoneyText.format(item.exceeded, item.currency);
    final financeColors = AppFinanceColors.of(context);
    // Kart tek cümlede duyurulur, fakat düzenle butonu ağaçta kalır: bütün
    // kartı `ExcludeSemantics` ile sarmak butonun erişilebilirliğini de
    // götürüyordu.
    return Semantics(
      container: true,
      label:
          '${item.categoryName} bütçesi. Limit $limit. Harcanan $spent.'
          '${item.isExceeded ? ' Limit $exceeded aşıldı.' : ' Kalan $remaining.'}',
      child: AppCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Expanded(
                  child: ExcludeSemantics(
                    child: Text(
                      item.categoryName,
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                  ),
                ),
                IconButton(
                  tooltip: 'Bütçe limitini düzenle',
                  onPressed: busy ? null : onEdit,
                  icon: const Icon(Icons.edit_outlined),
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
  String? _categoryId;

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

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<CreateBudgetInput>(
      title: '${_monthText(widget.controller.selectedMonth)} bütçesi',
      description: 'Limit bu aya özeldir; sonraki ay kendi limitini alır.',
      submitLabel: 'Oluştur',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        final month = widget.controller.selectedMonth;
        return CreateBudgetInput(
          categoryId: _categoryId!,
          limit: _limit.text.trim().replaceAll(',', '.'),
          year: month.year,
          month: month.month,
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
              return DropdownButtonFormField<String>(
                initialValue: _categoryId,
                isExpanded: true,
                decoration: const InputDecoration(
                  labelText: 'Gider kategorisi',
                ),
                items: (snapshot.data ?? const [])
                    .where((category) => category.isActive)
                    .map(
                      (category) => DropdownMenuItem(
                        value: category.id,
                        child: Text(category.name),
                      ),
                    )
                    .toList(growable: false),
                onChanged: (value) => setState(() => _categoryId = value),
                validator: (value) => value == null ? 'Kategori seçin.' : null,
              );
            },
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
    _limit = TextEditingController(text: widget.item.limit);
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
        return _limit.text.trim().replaceAll(',', '.');
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
  final normalized = value?.trim().replaceAll(',', '.') ?? '';
  if (!RegExp(r'^\d+(\.\d{1,4})?$').hasMatch(normalized) ||
      RegExp(r'^0+(\.0{1,4})?$').hasMatch(normalized)) {
    return 'Sıfırdan büyük bir limit girin (en fazla 4 ondalık).';
  }
  return null;
}

String _monthText(DateTime value) =>
    DateText.monthYear(value.year, value.month);
