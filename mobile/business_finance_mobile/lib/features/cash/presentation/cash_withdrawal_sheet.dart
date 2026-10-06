import 'package:flutter/material.dart';

import '../../../core/formatters/money_input.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_segment_rail.dart';
import 'cash_controller.dart';

/// Kasadan alınan paranın nasıl yazılacağı.
enum WithdrawalMethod {
  transfer('Şahsi hesaba aktar'),
  expense('Şahsi gider');

  const WithdrawalMethod(this.label);

  final String label;
}

/// `Kendime aldım` panelini açar (Aşama 06.3 K9).
///
/// [initialAmount] kasa farkından gelindiğinde tutarı dolu getirir.
/// [onOpenPersonalAccount] verilirse şahsi hesabı olmayan kullanıcıya
/// `Şahsi cüzdan aç` bağlantısı gösterilir.
Future<bool?> showCashWithdrawal(
  BuildContext context,
  CashCountController controller, {
  String? initialAmount,
  VoidCallback? onOpenPersonalAccount,
}) => AppFormSheet.show<bool>(
  context: context,
  builder: (_) => CashWithdrawalForm(
    controller: controller,
    initialAmount: initialAmount,
    onOpenPersonalAccount: onOpenPersonalAccount,
  ),
);

/// Esnafın kasadan kendine aldığı para: yeni bir kayıt türü değildir. Şahsi
/// hesaba aktarım var olan transferdir ve işletme netine dokunmaz; şahsi
/// gider `Şahsi` kapsamlı sıradan bir giderdir.
class CashWithdrawalForm extends StatefulWidget {
  const CashWithdrawalForm({
    required this.controller,
    super.key,
    this.initialAmount,
    this.onOpenPersonalAccount,
  });

  final CashCountController controller;
  final String? initialAmount;
  final VoidCallback? onOpenPersonalAccount;

  @override
  State<CashWithdrawalForm> createState() => _CashWithdrawalFormState();
}

class _CashWithdrawalFormState extends State<CashWithdrawalForm> {
  final formKey = GlobalKey<FormState>();
  late final TextEditingController amountController;
  late final Future<(List<DataChoice>, List<DataChoice>)> options;
  String date = AppDateField.format(DateTime.now());
  WithdrawalMethod? chosenMethod;
  String? accountId;
  String? categoryId;

  @override
  void initState() {
    super.initState();
    amountController = TextEditingController(text: widget.initialAmount ?? '');
    options = _load();
  }

  Future<(List<DataChoice>, List<DataChoice>)> _load() async {
    final accounts = await widget.controller.loadWithdrawalAccounts();
    final categories = await widget.controller.loadWithdrawalCategories();
    // Tek şahsi hesap varsa seçili gelir.
    if (accounts.length == 1) accountId = accounts.single.id;
    return (accounts, categories);
  }

  @override
  void dispose() {
    amountController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Form(
      key: formKey,
      child: AppFormSheet<bool>(
        title: 'Kendime aldım',
        description:
            '${widget.controller.selectedAccount?.name ?? 'Kasa'} '
            'bakiyesinden düşer.',
        submitLabel: 'Kaydet',
        onSubmit: _submit,
        children: [
          TextFormField(
            controller: amountController,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(labelText: 'Tutar'),
            validator: MoneyInput.positiveError,
          ),
          const SizedBox(height: AppSpacing.medium),
          AppDateField(
            label: 'Gün',
            value: date,
            lastDate: DateTime.now(),
            onChanged: (value) => setState(() => date = value),
          ),
          const SizedBox(height: AppSpacing.medium),
          FutureBuilder<(List<DataChoice>, List<DataChoice>)>(
            future: options,
            builder: (context, snapshot) {
              if (snapshot.hasError) {
                return const Text(
                  'Seçenekler yüklenemedi. Paneli kapatıp yeniden deneyin.',
                );
              }
              if (!snapshot.hasData) return const LinearProgressIndicator();
              final (accounts, categories) = snapshot.data!;
              // Şahsi hesap yoksa aktarılacak yer de yoktur.
              final method = accounts.isEmpty
                  ? WithdrawalMethod.expense
                  : chosenMethod ?? WithdrawalMethod.transfer;
              return Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  if (accounts.isNotEmpty) ...[
                    AppSegmentRail<WithdrawalMethod>(
                      values: WithdrawalMethod.values,
                      selected: method,
                      semanticLabel: 'Nasıl yazılsın?',
                      segmentLabel: (value) => value.label,
                      onChanged: (value) =>
                          setState(() => chosenMethod = value),
                      segmentBuilder: (context, value, isSelected) => Text(
                        value.label,
                        style: theme.textTheme.bodyMedium?.copyWith(
                          fontWeight: FontWeight.w600,
                          color: isSelected ? surfaces.ink : surfaces.inkMuted,
                        ),
                      ),
                    ),
                    const SizedBox(height: AppSpacing.medium),
                  ],
                  if (method == WithdrawalMethod.transfer)
                    DropdownButtonFormField<String>(
                      key: const ValueKey('withdrawal-account'),
                      initialValue: accountId,
                      isExpanded: true,
                      decoration: const InputDecoration(
                        labelText: 'Şahsi hesap',
                        helperText: 'Gider yazılmaz; para yer değiştirir.',
                      ),
                      items: [
                        for (final account in accounts)
                          DropdownMenuItem(
                            value: account.id,
                            child: Text(account.name),
                          ),
                      ],
                      onChanged: (value) => accountId = value,
                      validator: (value) =>
                          value == null ? 'Hesap seçin.' : null,
                    )
                  else
                    DropdownButtonFormField<String>(
                      key: const ValueKey('withdrawal-category'),
                      initialValue: categoryId,
                      isExpanded: true,
                      decoration: const InputDecoration(
                        labelText: 'Kategori',
                        helperText: 'Şahsi gider olarak yazılır.',
                      ),
                      items: [
                        for (final category in categories)
                          DropdownMenuItem(
                            value: category.id,
                            child: Text(category.name),
                          ),
                      ],
                      onChanged: (value) => categoryId = value,
                      validator: (value) =>
                          value == null ? 'Kategori seçin.' : null,
                    ),
                  if (accounts.isEmpty &&
                      widget.onOpenPersonalAccount != null) ...[
                    const SizedBox(height: AppSpacing.xSmall),
                    Align(
                      alignment: Alignment.centerLeft,
                      child: TextButton(
                        onPressed: () {
                          Navigator.of(context).pop();
                          widget.onOpenPersonalAccount!();
                        },
                        child: const Text('Şahsi cüzdan aç'),
                      ),
                    ),
                  ],
                ],
              );
            },
          ),
        ],
      ),
    );
  }

  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    final transfer =
        accountId != null &&
        (chosenMethod ?? WithdrawalMethod.transfer) ==
            WithdrawalMethod.transfer;
    if (!transfer && categoryId == null) return null;
    final saved = await widget.controller.recordWithdrawal(
      amount: MoneyInput.wire(amountController.text),
      date: date,
      personalAccountId: transfer ? accountId : null,
      categoryId: transfer ? null : categoryId,
    );
    return saved ? true : null;
  }
}
