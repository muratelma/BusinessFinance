import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/transaction_models.dart';
import '../data/transaction_repository.dart';
import 'transactions_controller.dart';

class TransactionsPage extends StatefulWidget {
  const TransactionsPage({
    super.key,
    this.controller,
    this.repository,
    this.changes,
    this.showCreateOnOpen = false,
  });

  final TransactionsController? controller;
  final TransactionRepositoryContract? repository;
  final FinancialDataChanges? changes;
  final bool showCreateOnOpen;

  @override
  State<TransactionsPage> createState() => _TransactionsPageState();
}

class _TransactionsPageState extends State<TransactionsPage> {
  TransactionsController? _controller;
  bool _ownsController = false;

  @override
  void initState() {
    super.initState();
    _controller = widget.controller;
    if (_controller == null && widget.repository != null) {
      _controller = TransactionsController(
        widget.repository!,
        financialDataChanges: widget.changes,
      );
      _ownsController = true;
    }
    _controller?.addListener(_changed);
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      final controller = _controller;
      if (controller == null) return;
      await controller.load();
      if (widget.showCreateOnOpen && mounted) {
        await _showCreateForm(controller);
      }
    });
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
      return const AppErrorView(message: 'İşlem servisi yapılandırılmadı.');
    }
    return Scaffold(
      appBar: AppBar(
        title: const Text('İşlemler'),
        // Ekleme eylemi başlıkta durur: bu sayfa alt gezinme çubuğunun
        // bulunduğu bir sekmede gösterilebiliyor ve orada çubuğun ortasındaki
        // buton zaten aynı işi yapıyor. İki kayan buton aynı ekranda
        // birbiriyle yarışıyordu.
        actions: [
          IconButton(
            tooltip: 'İşlemleri filtrele',
            onPressed: controller.isSubmitting
                ? null
                : () => _showFilter(controller),
            icon: const Icon(Icons.filter_list),
          ),
          IconButton(
            tooltip: 'İşlem ekle',
            onPressed: controller.isSubmitting
                ? null
                : () => _showCreateForm(controller),
            icon: const Icon(Icons.add),
          ),
        ],
      ),
      body: _body(controller),
    );
  }

  Widget _body(TransactionsController controller) {
    if (controller.unauthorized) {
      return const AppUnauthorizedView();
    }
    if (controller.isLoading && controller.page == null) {
      return const AppLoadingView(message: 'İşlemler yükleniyor');
    }
    if (controller.errorMessage != null && controller.page == null) {
      return AppErrorView(
        message: controller.errorMessage!,
        onRetry: controller.load,
      );
    }
    final items = controller.page?.items ?? const <TransactionItem>[];
    if (items.isEmpty) {
      return RefreshIndicator(
        onRefresh: controller.load,
        child: ListView(
          children: const [
            SizedBox(
              height: 480,
              child: AppEmptyView(
                icon: Icons.receipt_long_outlined,
                title: 'Henüz işlem yok',
                message: 'İlk gelir veya giderinizi ekleyebilirsiniz.',
              ),
            ),
          ],
        ),
      );
    }
    return Column(
      children: [
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
        if (controller.errorMessage != null)
          MaterialBanner(
            content: Text(controller.errorMessage!),
            actions: [
              TextButton(
                onPressed: controller.clearMessage,
                child: const Text('Kapat'),
              ),
            ],
          ),
        Expanded(
          child: RefreshIndicator(
            onRefresh: () =>
                controller.load(pageNumber: controller.page?.pageNumber ?? 1),
            child: ListView.separated(
              padding: const EdgeInsets.fromLTRB(
                AppSpacing.medium,
                AppSpacing.small,
                AppSpacing.medium,
                AppSpacing.fabClearance,
              ),
              itemCount: items.length,
              separatorBuilder: (_, _) =>
                  const SizedBox(height: AppSpacing.small),
              itemBuilder: (context, index) => _TransactionCard(
                item: items[index],
                busy: controller.isSubmitting,
                onCancel: () => _confirmCancel(controller, items[index]),
              ),
            ),
          ),
        ),
        _PaginationBar(controller: controller),
      ],
    );
  }

  Future<void> _showCreateForm(TransactionsController controller) async {
    final result = await AppFormSheet.show<CreateTransactionInput>(
      context: context,
      builder: (_) => _TransactionFormDialog(controller: controller),
    );
    if (result != null && mounted) await controller.create(result);
  }

  Future<void> _showFilter(TransactionsController controller) async {
    final result = await AppFormSheet.show<TransactionFilter>(
      context: context,
      builder: (_) => _TransactionFilterDialog(
        initial: controller.filter,
        loadAccounts: controller.loadAccounts,
        loadCategories: controller.loadFilterCategories,
      ),
    );
    if (result != null) await controller.applyFilter(result);
  }

  Future<void> _confirmCancel(
    TransactionsController controller,
    TransactionItem item,
  ) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.block,
      destructive: true,
      title: 'İşlemi iptal et?',
      message:
          'Kayıt silinmez; iptal edildi olarak işaretlenir ve toplamları '
          'artık etkilemez.',
      confirmLabel: 'İptal et',
    );
    if (confirmed) await controller.cancel(item.id);
  }
}

class _TransactionCard extends StatelessWidget {
  const _TransactionCard({
    required this.item,
    required this.busy,
    required this.onCancel,
  });
  final TransactionItem item;
  final bool busy;
  final VoidCallback onCancel;

  @override
  Widget build(BuildContext context) {
    final income = item.kind == TransactionKind.income;
    final financeColors = AppFinanceColors.of(context);
    final money = MoneyText.format(item.amount, item.currency);
    final effect = income ? AppMoneyEffect.income : AppMoneyEffect.expense;
    final date = DateText.dayMonth(item.transactionDate);
    final description = item.description?.trim();

    return Semantics(
      label:
          '${income ? 'Gelir' : 'Gider'}, $money, '
          '$date${item.isCancelled ? ', iptal edildi' : ''}',
      child: AppCard(
        padding: EdgeInsets.zero,
        child: AppListRow(
          icon: income ? Icons.arrow_downward : Icons.arrow_upward,
          iconColor: item.isCancelled
              ? financeColors.cancelled
              : income
              ? financeColors.income
              : financeColors.expense,
          // Kaydın adı kullanıcının yazdığıdır; yazmamışsa yönün adı.
          title: description == null || description.isEmpty
              ? (income ? 'Gelir' : 'Gider')
              : description,
          titleMaxLines: 1,
          subtitle: date,
          dimmed: item.isCancelled,
          // Durum ve eylem aynı yerde, aynı boyda: önceki hâlde iptal edilmiş
          // satırda kocaman bir `Chip`, edilmemişte bir ikon butonu vardı ve
          // iki satır farklı yükseklikte görünüyordu.
          badge: item.isCancelled
              ? const AppStatusChip(
                  label: 'İptal edildi',
                  icon: Icons.block,
                  tone: AppStatusTone.cancelled,
                )
              : Tooltip(
                  message: 'İşlemi iptal et',
                  child: TextButton(
                    style: TextButton.styleFrom(
                      visualDensity: VisualDensity.compact,
                      padding: const EdgeInsets.symmetric(
                        horizontal: AppSpacing.small,
                      ),
                      minimumSize: Size.zero,
                      tapTargetSize: MaterialTapTargetSize.padded,
                    ),
                    onPressed: busy ? null : onCancel,
                    child: const Text('İptal et'),
                  ),
                ),
          trailing: AppMoneyText(
            amount: item.amount,
            currency: item.currency,
            effect: effect,
            isCancelled: item.isCancelled,
            signed: true,
            style: item.isCancelled
                ? const TextStyle(decoration: TextDecoration.lineThrough)
                : null,
          ),
        ),
      ),
    );
  }
}

class _PaginationBar extends StatelessWidget {
  const _PaginationBar({required this.controller});
  final TransactionsController controller;

  @override
  Widget build(BuildContext context) {
    final page = controller.page!;
    return SafeArea(
      top: false,
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          IconButton(
            tooltip: 'Önceki sayfa',
            onPressed: page.hasPreviousPage && !controller.isLoading
                ? () => controller.load(pageNumber: page.pageNumber - 1)
                : null,
            icon: const Icon(Icons.chevron_left),
          ),
          Text(
            '${page.pageNumber} / ${page.totalPages == 0 ? 1 : page.totalPages}',
          ),
          IconButton(
            tooltip: 'Sonraki sayfa',
            onPressed: page.hasNextPage && !controller.isLoading
                ? () => controller.load(pageNumber: page.pageNumber + 1)
                : null,
            icon: const Icon(Icons.chevron_right),
          ),
        ],
      ),
    );
  }
}

class _TransactionFormDialog extends StatefulWidget {
  const _TransactionFormDialog({required this.controller});
  final TransactionsController controller;

  @override
  State<_TransactionFormDialog> createState() => _TransactionFormDialogState();
}

class _TransactionFormDialogState extends State<_TransactionFormDialog> {
  final _formKey = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _description = TextEditingController();
  TransactionKind _kind = TransactionKind.expense;
  String? _accountId;
  String? _categoryId;
  late String _date;
  late Future<List<TransactionChoice>> _accounts;
  late Future<List<TransactionChoice>> _categories;

  @override
  void initState() {
    super.initState();
    final now = DateTime.now();
    _date = _dateText(now);
    _accounts = widget.controller.loadAccounts();
    _categories = widget.controller.loadCategories(_kind);
  }

  @override
  void dispose() {
    _amount.dispose();
    _description.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<CreateTransactionInput>(
      title: 'Yeni işlem',
      submitLabel: 'Kaydet',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        return CreateTransactionInput(
          accountId: _accountId!,
          categoryId: _categoryId!,
          amount: _amount.text.trim().replaceAll(',', '.'),
          kind: _kind,
          transactionDate: _date,
          description: _description.text.trim().isEmpty
              ? null
              : _description.text.trim(),
        );
      },
      children: [
        AppFormField(
          child: SegmentedButton<TransactionKind>(
            segments: const [
              ButtonSegment(
                value: TransactionKind.expense,
                label: Text('Gider'),
              ),
              ButtonSegment(
                value: TransactionKind.income,
                label: Text('Gelir'),
              ),
            ],
            selected: {_kind},
            onSelectionChanged: (selection) {
              setState(() {
                _kind = selection.first;
                _categoryId = null;
                _categories = widget.controller.loadCategories(_kind);
              });
            },
          ),
        ),
        AppFormField(
          child: _choiceField(
            future: _accounts,
            label: 'Hesap',
            value: _accountId,
            onChanged: (value) => setState(() => _accountId = value),
          ),
        ),
        AppFormField(
          child: _choiceField(
            future: _categories,
            label: 'Kategori',
            value: _categoryId,
            onChanged: (value) => setState(() => _categoryId = value),
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: _amount,
            decoration: const InputDecoration(
              labelText: 'Tutar',
              suffixText: 'TRY',
            ),
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            validator: _moneyError,
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'İşlem tarihi',
            value: _date,
            onChanged: (value) => setState(() => _date = value),
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: _description,
            hintLocales: const [Locale('tr', 'TR')],
            // Bu alan kaydın listede görünen adı olur; etiket bunu söyler,
            // yoksa kullanıcı buraya bir cümle yazıp adı cümle yapıyor.
            decoration: const InputDecoration(
              labelText: 'Ad (isteğe bağlı)',
              hintText: 'İstanbulkart yükleme',
              helperText: 'Boş bırakılırsa kategori adı kullanılır.',
            ),
            maxLength: 250,
            buildCounter:
                (
                  _, {
                  required currentLength,
                  required isFocused,
                  required maxLength,
                }) => null,
          ),
        ),
      ],
    ),
  );

  Widget _choiceField({
    required Future<List<TransactionChoice>> future,
    required String label,
    required String? value,
    required ValueChanged<String?> onChanged,
  }) => FutureBuilder<List<TransactionChoice>>(
    future: future,
    builder: (context, snapshot) {
      if (snapshot.connectionState != ConnectionState.done) {
        return const LinearProgressIndicator();
      }
      if (snapshot.hasError) return Text('$label yüklenemedi.');
      final choices = snapshot.data ?? const [];
      return DropdownButtonFormField<String>(
        isExpanded: true,
        initialValue: value,
        decoration: InputDecoration(labelText: label),
        items: choices
            .where((choice) => choice.isActive)
            .map(
              (choice) =>
                  DropdownMenuItem(value: choice.id, child: Text(choice.name)),
            )
            .toList(growable: false),
        onChanged: onChanged,
        validator: (selected) => selected == null ? '$label seçin.' : null,
      );
    },
  );
}

class _TransactionFilterDialog extends StatefulWidget {
  const _TransactionFilterDialog({
    required this.initial,
    required this.loadAccounts,
    required this.loadCategories,
  });
  final TransactionFilter initial;
  final Future<List<TransactionChoice>> Function() loadAccounts;
  final Future<List<TransactionChoice>> Function(TransactionKind? kind)
  loadCategories;

  @override
  State<_TransactionFilterDialog> createState() =>
      _TransactionFilterDialogState();
}

class _TransactionFilterDialogState extends State<_TransactionFilterDialog> {
  TransactionKind? _kind;
  String? _accountId;
  String? _categoryId;
  String? _from;
  String? _to;
  late final Future<List<TransactionChoice>> _accounts;
  late Future<List<TransactionChoice>> _categories;

  @override
  void initState() {
    super.initState();
    _kind = widget.initial.kind;
    _accountId = widget.initial.accountId;
    _categoryId = widget.initial.categoryId;
    _from = widget.initial.dateFrom;
    _to = widget.initial.dateTo;
    _accounts = widget.loadAccounts();
    _categories = widget.loadCategories(_kind);
  }

  @override
  Widget build(BuildContext context) => AppFormSheet<TransactionFilter>(
    title: 'İşlemleri filtrele',
    submitLabel: 'Uygula',
    secondaryLabel: 'Temizle',
    // Temizlemek vazgeçmek değildir: vazgeçmek filtreyi olduğu gibi bırakır,
    // temizlemek boş filtreyi uygular.
    onSecondary: () async => const TransactionFilter(),
    onSubmit: _from != null && _to != null && _from!.compareTo(_to!) > 0
        ? null
        : () async => TransactionFilter(
            kind: _kind,
            accountId: _accountId,
            categoryId: _categoryId,
            dateFrom: _from,
            dateTo: _to,
          ),
    children: [
      AppFormField(
        child: DropdownButtonFormField<TransactionKind?>(
          initialValue: _kind,
          isExpanded: true,
          decoration: const InputDecoration(labelText: 'Tür'),
          items: const [
            DropdownMenuItem(value: null, child: Text('Tümü')),
            DropdownMenuItem(
              value: TransactionKind.income,
              child: Text('Gelir'),
            ),
            DropdownMenuItem(
              value: TransactionKind.expense,
              child: Text('Gider'),
            ),
          ],
          onChanged: (value) => setState(() {
            _kind = value;
            _categoryId = null;
            _categories = widget.loadCategories(value);
          }),
        ),
      ),
      AppFormField(
        child: _FilterChoiceField(
          label: 'Hesap',
          value: _accountId,
          choices: _accounts,
          onChanged: (value) => setState(() => _accountId = value),
        ),
      ),
      AppFormField(
        child: _FilterChoiceField(
          label: 'Kategori',
          value: _categoryId,
          choices: _categories,
          onChanged: (value) => setState(() => _categoryId = value),
        ),
      ),
      AppFormField(
        child: AppDateField(
          label: 'Başlangıç',
          value: _from,
          onChanged: (value) => setState(() => _from = value),
        ),
      ),
      AppFormField(
        child: AppDateField(
          label: 'Bitiş',
          value: _to,
          onChanged: (value) => setState(() => _to = value),
        ),
      ),
    ],
  );
}

class _FilterChoiceField extends StatelessWidget {
  const _FilterChoiceField({
    required this.label,
    required this.value,
    required this.choices,
    required this.onChanged,
  });

  final String label;
  final String? value;
  final Future<List<TransactionChoice>> choices;
  final ValueChanged<String?> onChanged;

  @override
  Widget build(BuildContext context) => FutureBuilder<List<TransactionChoice>>(
    future: choices,
    builder: (context, snapshot) {
      if (snapshot.connectionState != ConnectionState.done) {
        return const LinearProgressIndicator();
      }
      if (snapshot.hasError) {
        return Text('$label seçenekleri yüklenemedi.');
      }
      return DropdownButtonFormField<String?>(
        isExpanded: true,
        initialValue: value,
        decoration: InputDecoration(labelText: label),
        items: [
          const DropdownMenuItem(value: null, child: Text('Tümü')),
          for (final choice in snapshot.data ?? const <TransactionChoice>[])
            DropdownMenuItem(value: choice.id, child: Text(choice.name)),
        ],
        onChanged: onChanged,
      );
    },
  );
}

String? _moneyError(String? value) {
  final normalized = value?.trim().replaceAll(',', '.') ?? '';
  if (!RegExp(r'^\d+(\.\d{1,4})?$').hasMatch(normalized)) {
    return 'Pozitif bir tutar girin (en fazla 4 ondalık).';
  }
  if (RegExp(r'^0+(\.0{1,4})?$').hasMatch(normalized)) {
    return 'Tutar sıfırdan büyük olmalı.';
  }
  return null;
}

String _dateText(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-${value.month.toString().padLeft(2, '0')}-${value.day.toString().padLeft(2, '0')}';
