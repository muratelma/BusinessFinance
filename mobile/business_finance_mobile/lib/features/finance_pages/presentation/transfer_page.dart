import '../../../core/theme/app_spacing.dart';
import 'package:flutter/material.dart';
import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_math.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_error.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_untracked_text.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../cards/presentation/transfer_prefill.dart';
import '../data/finance_page_models.dart';
import '../data/finance_page_repository.dart';
import 'transfer_controller.dart';
import '../../../core/widgets/app_account_field.dart';
import '../../../features/accounts/presentation/account_directory.dart';

class TransferPage extends StatelessWidget {
  const TransferPage({
    required this.repository,
    super.key,
    this.changes,
    this.prefill,
  });
  final FinancePageRepositoryContract repository;
  final FinancialDataChanges? changes;
  final TransferPrefill? prefill;
  @override
  Widget build(BuildContext context) =>
      AppUntrackedText(child: _TransferPageBody(this));
}

class _TransferPageBody extends StatefulWidget {
  const _TransferPageBody(this.page);
  final TransferPage page;
  @override
  State<_TransferPageBody> createState() => _TransferPageState();
}

class _TransferPageState extends State<_TransferPageBody> {
  late final controller = TransferController(
    widget.page.repository,
    changes: widget.page.changes,
  );
  final form = GlobalKey<FormState>();
  late final amount = TextEditingController(
    text: widget.page.prefill?.amount == null
        ? ''
        : _editableAmount(widget.page.prefill!.amount!),
  );
  late final fee = TextEditingController(
    text: widget.page.prefill?.feeAmount == null
        ? ''
        : _editableAmount(widget.page.prefill!.feeAmount!),
  );
  late final description = TextEditingController(
    text: widget.page.prefill?.description ?? '',
  );
  late String date =
      widget.page.prefill?.date ??
      DateTime.now().toIso8601String().substring(0, 10);
  String? source;
  String? destination;
  TransactionScope? explicit;
  @override
  void initState() {
    super.initState();
    controller.load();
  }

  @override
  void dispose() {
    controller.dispose();
    amount.dispose();
    fee.dispose();
    description.dispose();
    super.dispose();
  }

  AccountOverviewItem? get from =>
      controller.accounts?.items.where((a) => a.id == source).firstOrNull;
  void _preview() {
    final value = MoneyMath.fromInput(amount.text);
    final feeValue = fee.text.trim().isEmpty
        ? '0.0000'
        : MoneyMath.fromInput(fee.text);
    controller.setInput(
      source == null ||
              destination == null ||
              source == destination ||
              value == null ||
              value == '0.0000' ||
              feeValue == null
          ? null
          : TransferInput(
              sourceAccountId: source!,
              destinationAccountId: destination!,
              amount: value,
              date: date,
              feeAmount: feeValue,
              feeScope: previewResolvedScope(
                explicit: explicit,
                source: from?.defaultScope,
              ),
              description: description.text.trim().isEmpty
                  ? null
                  : description.text.trim(),
            ),
    );
  }

  Future<void> _save() async {
    if (!form.currentState!.validate()) return;
    if (await controller.save() && mounted) {
      Navigator.pop(context, true);
    }
  }

  @override
  Widget build(BuildContext context) =>
      Scaffold(body: SafeArea(child: _page()));

  Widget _page() => AnimatedBuilder(
    animation: controller,
    builder: (context, _) => Column(
      children: [
        AppPageHeader(
          title: 'Yeni transfer',
          compact: true,
          onBack: controller.isSaving
              ? null
              : () => Navigator.maybePop(context),
        ),
        Expanded(
          child: controller.unauthorized
              ? const AppUnauthorizedView()
              : controller.accounts == null
              ? controller.errorMessage != null
                    ? AppErrorView(
                        message: controller.errorMessage!,
                        onRetry: controller.load,
                      )
                    : const AppLoadingView()
              : Form(
                  key: form,
                  child: ListView(
                    padding: const EdgeInsets.only(
                      left: AppSpacing.medium,
                      top: AppSpacing.small + AppSpacing.xSmall,
                      right: AppSpacing.medium,
                      bottom: AppSpacing.large,
                    ),
                    children: [
                      AppDateField(
                        label: 'Tarih',
                        iconLeading: true,
                        value: date,
                        valueText: DateText.dayMonthYear(date),
                        lastDate: DateTime.now(),
                        enabled: !controller.isSaving,
                        onChanged: (v) {
                          date = v;
                          _preview();
                        },
                      ),
                      const SizedBox(height: AppSpacing.medium),
                      _account('Çıkan hesap', source, (v) {
                        source = v;
                        if (destination == source) destination = null;
                        explicit = null;
                        _preview();
                      }),
                      const SizedBox(height: AppSpacing.medium),
                      _account('Giren hesap', destination, (v) {
                        destination = v;
                        _preview();
                      }, exclude: source),
                      const SizedBox(height: AppSpacing.medium),
                      _money('Tutar', amount, requiredValue: true),
                      const SizedBox(height: AppSpacing.medium),
                      _money('Masraf (isteğe bağlı)', fee),
                      if ((MoneyMath.fromInput(fee.text) ?? '0.0000') !=
                              '0.0000' &&
                          from?.defaultScope == null &&
                          (controller.preview?.needsScope ??
                              false || explicit != null)) ...[
                        const SizedBox(
                          height: AppSpacing.medium + AppSpacing.xSmall,
                        ),
                        AppScopeSection(
                          explicit: explicit,
                          source: from?.defaultScope,
                          category: null,
                          sourceName: from?.name,
                          onChanged: (v) {
                            if (controller.isSaving) return;
                            explicit = v;
                            _preview();
                          },
                        ),
                      ],
                      const SizedBox(height: AppSpacing.medium),
                      if (controller.isPreviewing)
                        const Padding(
                          padding: EdgeInsets.all(
                            AppSpacing.small + AppSpacing.xSmall,
                          ),
                          child: LinearProgressIndicator(),
                        ),
                      if (controller.preview != null)
                        AppInlineNotice(
                          margin: EdgeInsets.zero,
                          compact: true,
                          emphasis: MoneyText.format(
                            controller.preview!.totalOutflow,
                            'TRY',
                          ),
                          message: controller.preview!.feeAmount == '0.0000'
                              ? '${controller.preview!.sourceAccountName} hesabından ${MoneyText.format(controller.preview!.totalOutflow, 'TRY')} çıkar.'
                              : '${controller.preview!.sourceAccountName} hesabından toplam ${MoneyText.format(controller.preview!.totalOutflow, 'TRY')} çıkar. Masraf ${controller.preview!.feeCategoryName} olarak gider yazılır.',
                        ),
                      const SizedBox(height: AppSpacing.medium),
                      TextFormField(
                        controller: description,
                        enabled: !controller.isSaving,
                        maxLength: 500,
                        maxLines: 1,
                        decoration: const InputDecoration(
                          labelText: 'Açıklama (isteğe bağlı)',
                          counterText: '',
                        ),
                        onChanged: (_) => _preview(),
                      ),
                      if (controller.errorMessage != null)
                        AppFormError(message: controller.errorMessage!),
                      const SizedBox(height: AppSpacing.large),
                    ],
                  ),
                ),
        ),
        // Kaydet altta sabittir; içerikten ince bir çizgiyle ayrılır.
        if (controller.accounts != null && !controller.unauthorized)
          DecoratedBox(
            decoration: BoxDecoration(
              border: Border(
                top: BorderSide(color: AppSurfaces.of(context).border),
              ),
            ),
            child: Padding(
              padding: const EdgeInsets.fromLTRB(
                AppSpacing.medium,
                AppSpacing.small + AppSpacing.xSmall,
                AppSpacing.medium,
                AppSpacing.medium + AppSpacing.xSmall,
              ),
              child: SizedBox(width: double.infinity, child: _saveButton()),
            ),
          ),
      ],
    ),
  );
  Widget _saveButton() => FilledButton(
    onPressed:
        controller.isSaving ||
            controller.preview == null ||
            controller.preview!.needsScope ||
            controller.isPreviewing
        ? null
        : _save,
    style: FilledButton.styleFrom(
      minimumSize: const Size.fromHeight(52),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
    ),
    child: Text(controller.isSaving ? 'Kaydediliyor…' : 'Kaydet'),
  );
  Widget _account(
    String label,
    String? value,
    ValueChanged<String?> changed, {
    String? exclude,
  }) {
    final choices = AccountChoices.of(context);
    return AppAccountField(
      label: label,
      value: value,
      // Giren hesapta çıkan hesap listede kalır ama seçilemez: kullanıcı
      // nereden çıktığını menüde de görür.
      options: [
        for (final a in controller.accounts!.items)
          if (a.isActive)
            choices.account(
              a.id,
              a.name,
              type: a.type,
              scope: a.defaultScope,
              balance: a.balance,
              disabledNote: a.id == exclude ? 'Para buradan çıkıyor' : null,
            ),
      ],
      onChanged: controller.isSaving ? null : changed,
      onAdd: () => _addAccount(changed),
      validator: (v) => v == null ? 'Hesap seçin.' : null,
    );
  }

  /// `Hesap ekle`den dönülünce yeni hesap listede ve bu alanda seçili gelir.
  Future<void> _addAccount(ValueChanged<String?> changed) async {
    final id = (await addAccountFromForm(context))?.id;
    if (id == null || !mounted) return;
    await controller.load();
    if (!mounted) return;
    if (controller.accounts?.items.any((a) => a.id == id) ?? false) {
      setState(() => changed(id));
    }
  }

  Widget _money(
    String label,
    TextEditingController text, {
    bool requiredValue = false,
  }) => TextFormField(
    controller: text,
    enabled: !controller.isSaving,
    keyboardType: const TextInputType.numberWithOptions(decimal: true),
    inputFormatters: const [TurkishAmountInputFormatter()],
    decoration: InputDecoration(
      labelText: label,
      prefixText: '₺ ',
      helperText:
          !requiredValue &&
              (MoneyMath.fromInput(text.text) ?? '0.0000') == '0.0000'
          ? 'Bankanın aldığı ücret. Çıkan hesaptan düşer.'
          : null,
    ),
    onChanged: (_) => _preview(),
    validator: (v) {
      if (!requiredValue && (v ?? '').isEmpty) return null;
      final wire = MoneyMath.fromInput(v ?? '');
      return wire == null || (requiredValue && wire == '0.0000')
          ? 'Geçerli bir tutar yazın.'
          : null;
    },
  );
  static String _editableAmount(String wire) {
    var text = MoneyText.editable(wire);
    if (!text.contains(',')) {
      text += ',00';
    } else {
      final parts = text.split(',');
      text = '${parts.first},${parts.last.padRight(2, '0')}';
    }
    return const TurkishAmountInputFormatter()
        .formatEditUpdate(
          const TextEditingValue(),
          TextEditingValue(text: text),
        )
        .text;
  }
}
