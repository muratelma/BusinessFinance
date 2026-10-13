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
import '../../cards/presentation/transfer_prefill.dart';
import '../data/finance_page_models.dart';
import '../data/finance_page_repository.dart';
import 'transfer_controller.dart';

class TransferPage extends StatefulWidget {
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
  State<TransferPage> createState() => _TransferPageState();
}

class _TransferPageState extends State<TransferPage> {
  late final controller = TransferController(
    widget.repository,
    changes: widget.changes,
  );
  final form = GlobalKey<FormState>();
  late final amount = TextEditingController(
    text: widget.prefill?.amount == null
        ? ''
        : MoneyText.editable(widget.prefill!.amount!),
  );
  late final fee = TextEditingController(
    text: widget.prefill?.feeAmount == null
        ? ''
        : MoneyText.editable(widget.prefill!.feeAmount!),
  );
  late final description = TextEditingController(
    text: widget.prefill?.description ?? '',
  );
  late String date =
      widget.prefill?.date ?? DateTime.now().toIso8601String().substring(0, 10);
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
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      child: AnimatedBuilder(
        animation: controller,
        builder: (context, _) => Column(
          children: [
            AppPageHeader(
              title: 'Yeni transfer',
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
                          top: AppSpacing.xSmall,
                          right: AppSpacing.medium,
                          bottom: AppSpacing.xLarge,
                        ),
                        children: [
                          AppDateField(
                            label: 'Tarih',
                            value: date,
                            valueText: DateText.dayMonthYear(date),
                            lastDate: DateTime.now(),
                            enabled: !controller.isSaving,
                            onChanged: (v) {
                              date = v;
                              _preview();
                            },
                          ),
                          const SizedBox(
                            height: AppSpacing.medium + AppSpacing.xSmall,
                          ),
                          _account('Çıkan hesap', source, (v) {
                            source = v;
                            if (destination == source) destination = null;
                            explicit = null;
                            _preview();
                          }),
                          const SizedBox(
                            height: AppSpacing.medium + AppSpacing.xSmall,
                          ),
                          _account('Giren hesap', destination, (v) {
                            destination = v;
                            _preview();
                          }, exclude: source),
                          const SizedBox(
                            height: AppSpacing.medium + AppSpacing.xSmall,
                          ),
                          _money('Tutar', amount, requiredValue: true),
                          const SizedBox(
                            height: AppSpacing.medium + AppSpacing.xSmall,
                          ),
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
                                explicit = v;
                                _preview();
                              },
                            ),
                          ],
                          const SizedBox(
                            height: AppSpacing.medium + AppSpacing.xSmall,
                          ),
                          TextFormField(
                            controller: description,
                            enabled: !controller.isSaving,
                            maxLength: 500,
                            maxLines: 2,
                            decoration: const InputDecoration(
                              labelText: 'Açıklama (isteğe bağlı)',
                            ),
                            onChanged: (_) => _preview(),
                          ),
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
                              message: controller.preview!.feeAmount == '0.0000'
                                  ? '${controller.preview!.sourceAccountName} hesabından ${MoneyText.format(controller.preview!.totalOutflow, 'TRY')} çıkacak.'
                                  : '${controller.preview!.sourceAccountName} hesabından masrafla birlikte ${MoneyText.format(controller.preview!.totalOutflow, 'TRY')} çıkacak. Masraf, ${controller.preview!.feeCategoryName} kategorisine${controller.preview!.resolvedScope == null ? '' : ' · ${controller.preview!.resolvedScope!.label}'} yazılacak.',
                            ),
                          if (controller.errorMessage != null)
                            AppFormError(message: controller.errorMessage!),
                          const SizedBox(height: AppSpacing.large),
                        ],
                      ),
                    ),
            ),
            if (controller.accounts != null && !controller.unauthorized)
              Padding(
                padding: const EdgeInsets.all(AppSpacing.medium),
                child: SizedBox(width: double.infinity, child: _saveButton()),
              ),
          ],
        ),
      ),
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
    style: FilledButton.styleFrom(minimumSize: const Size.fromHeight(52)),
    child: Text(controller.isSaving ? 'Kaydediliyor…' : 'Transferi kaydet'),
  );
  Widget _account(
    String label,
    String? value,
    ValueChanged<String?> changed, {
    String? exclude,
  }) => DropdownButtonFormField<String>(
    key: ValueKey((label, value)),
    initialValue: value,
    isExpanded: true,
    decoration: InputDecoration(labelText: label),
    items: [
      for (final a in controller.accounts!.items)
        if (a.isActive && a.id != exclude)
          DropdownMenuItem(value: a.id, child: Text(a.name)),
    ],
    onChanged: controller.isSaving ? null : changed,
    validator: (v) => v == null ? 'Hesap seçin.' : null,
  );
  Widget _money(
    String label,
    TextEditingController text, {
    bool requiredValue = false,
  }) => TextFormField(
    controller: text,
    enabled: !controller.isSaving,
    keyboardType: const TextInputType.numberWithOptions(decimal: true),
    inputFormatters: const [TurkishAmountInputFormatter()],
    decoration: InputDecoration(labelText: label, prefixText: '₺ '),
    onChanged: (_) => _preview(),
    validator: (v) {
      if (!requiredValue && (v ?? '').isEmpty) return null;
      final wire = MoneyMath.fromInput(v ?? '');
      return wire == null || (requiredValue && wire == '0.0000')
          ? 'Geçerli bir tutar yazın.'
          : null;
    },
  );
}
