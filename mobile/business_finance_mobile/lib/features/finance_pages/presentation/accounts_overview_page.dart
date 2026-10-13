import '../../../core/theme/app_spacing.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_summary_card.dart';
import '../data/finance_page_models.dart';
import '../data/finance_page_repository.dart';
import 'finance_page_controller.dart';

class AccountsOverviewPage extends StatefulWidget {
  const AccountsOverviewPage({
    required this.repository,
    super.key,
    this.changes,
    this.onCreate,
  });
  final FinancePageRepositoryContract repository;
  final FinancialDataChanges? changes;
  final Future<void> Function()? onCreate;
  @override
  State<AccountsOverviewPage> createState() => _AccountsOverviewPageState();
}

class _AccountsOverviewPageState extends State<AccountsOverviewPage> {
  late final controller = FinancePageController(
    widget.repository,
    kind: FinancePageKind.accounts,
    changes: widget.changes,
  );
  bool passive = false;
  @override
  void initState() {
    super.initState();
    controller.load();
  }

  @override
  void dispose() {
    controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      child: AnimatedBuilder(
        animation: controller,
        builder: (context, _) => Column(
          children: [
            AppPageHeader(
              title: 'Hesaplar',
              compact: true,
              onBack: () => Navigator.maybePop(context),
              actions: [
                if (widget.onCreate != null)
                  IconButton(
                    tooltip: 'Yeni hesap',
                    onPressed: widget.onCreate,
                    icon: const Icon(Icons.add),
                  ),
              ],
            ),
            if (controller.isStale)
              TextButton(
                onPressed: controller.load,
                child: const Text('Son alınan veriler · Yenile'),
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
                  : RefreshIndicator(
                      onRefresh: controller.load,
                      child: ListView(
                        physics: const AlwaysScrollableScrollPhysics(),
                        padding: const EdgeInsets.only(
                          left: AppSpacing.medium,
                          top: AppSpacing.small,
                          right: AppSpacing.medium,
                          bottom: AppSpacing.xLarge,
                        ),
                        children: [
                          if (controller.accounts!.items.isNotEmpty) ...[
                            AppSummaryCard(
                              label: 'Toplam bakiye',
                              amount: controller.accounts!.total,
                              footer: Wrap(
                                spacing: 24,
                                runSpacing: 12,
                                children: [
                                  _metric(
                                    'Nakit',
                                    controller.accounts!.cashTotal,
                                  ),
                                  _metric(
                                    'Banka',
                                    controller.accounts!.bankTotal,
                                  ),
                                  if (controller.accounts!.inactiveTotal !=
                                      '0.0000')
                                    _metric(
                                      'Pasif',
                                      controller.accounts!.inactiveTotal,
                                    ),
                                ],
                              ),
                            ),
                            const SizedBox(height: AppSpacing.medium),
                            OutlinedButton.icon(
                              onPressed: () => context.push(transferLocation),
                              icon: const Icon(Icons.swap_horiz),
                              label: const Text('Transfer yap'),
                            ),
                          ],
                          if (controller.accounts!.items.isEmpty)
                            SizedBox(
                              height: 280,
                              child: AppEmptyView(
                                title: 'Henüz hesap yok',
                                message: 'Paranızın durduğu hesapları ekleyin.',
                                icon: Icons.account_balance_wallet_outlined,
                                action: widget.onCreate == null
                                    ? null
                                    : FilledButton.icon(
                                        onPressed: widget.onCreate,
                                        icon: const Icon(Icons.add),
                                        label: const Text('Hesap ekle'),
                                      ),
                              ),
                            ),
                          ..._group(
                            'Nakit',
                            'cash',
                            controller.accounts!.cashTotal,
                          ),
                          ..._group(
                            'Banka',
                            'bank',
                            controller.accounts!.bankTotal,
                          ),
                          if (controller.accounts!.items.any(
                            (a) => !a.isActive,
                          )) ...[
                            const SizedBox(
                              height: AppSpacing.medium + AppSpacing.xSmall,
                            ),
                            TextButton(
                              onPressed: () =>
                                  setState(() => passive = !passive),
                              child: Row(
                                children: [
                                  Expanded(
                                    child: Text(
                                      'Pasif hesaplar · ${controller.accounts!.items.where((a) => !a.isActive).length}',
                                    ),
                                  ),
                                  if (controller.accounts!.inactiveTotal !=
                                      '0.0000')
                                    Flexible(
                                      child: Text(
                                        MoneyText.format(
                                          controller.accounts!.inactiveTotal,
                                          'TRY',
                                        ),
                                      ),
                                    ),
                                  Icon(
                                    passive
                                        ? Icons.expand_less
                                        : Icons.expand_more,
                                  ),
                                ],
                              ),
                            ),
                            if (passive)
                              _list(
                                controller.accounts!.items
                                    .where((a) => !a.isActive)
                                    .toList(),
                              ),
                          ],
                        ],
                      ),
                    ),
            ),
          ],
        ),
      ),
    ),
  );
  Widget _metric(String label, String amount) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      Text(label),
      AppMoneyText(amount: amount, currency: 'TRY', size: AppMoneySize.row),
    ],
  );
  List<Widget> _group(String label, String type, String total) {
    final items = controller.accounts!.items
        .where((a) => a.isActive && a.type == type)
        .toList();
    return items.isEmpty
        ? []
        : [
            const SizedBox(height: AppSpacing.medium + AppSpacing.xSmall),
            Padding(
              padding: const EdgeInsets.only(bottom: AppSpacing.small),
              child: Row(
                children: [
                  Expanded(
                    child: Text(
                      label,
                      style: Theme.of(context).textTheme.titleSmall,
                    ),
                  ),
                  AppMoneyText(
                    amount: total,
                    currency: 'TRY',
                    size: AppMoneySize.body,
                  ),
                ],
              ),
            ),
            _list(items),
          ];
  }

  Widget _list(List<AccountOverviewItem> items) => AppCard(
    padding: EdgeInsets.zero,
    child: Column(
      children: [
        for (var i = 0; i < items.length; i++) ...[
          if (i > 0) const Divider(height: 1, indent: 72),
          AppRow(
            title: items[i].name,
            leading: AppIconCapsule(
              icon: items[i].type == 'cash'
                  ? Icons.payments_outlined
                  : Icons.account_balance_outlined,
            ),
            subtitle: !items[i].isActive
                ? 'Pasif'
                : items[i].defaultScope?.label,
            trailing: AppMoneyText(
              amount: items[i].balance,
              currency: 'TRY',
              size: AppMoneySize.row,
            ),
            onTap: () => context.push('$accountsLocation/${items[i].id}'),
          ),
        ],
      ],
    ),
  );
}
