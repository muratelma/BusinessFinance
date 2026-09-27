import 'package:flutter/material.dart';

import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../pos/presentation/pos_controller.dart';
import '../../pos/presentation/pos_settlements_view.dart';
import '../data/cash_repository.dart';
import 'cash_controller.dart';
import 'cash_count_view.dart';

/// `Kasa`: tezgâh üstü esnafın günlük ekranı.
///
/// Tasarım teslimi (27 Eylül 2026, KasaV4): sekme yok, tek akış — bugünün
/// sayımı → yoldaki POS tahsilatları → son sayımlar. İkisi tek ekranda çünkü
/// ikisi de **günün parası**: kasadaki nakit ve müşterinin kartla ödediği,
/// henüz yolda olan para. Kredi kartı borcu burada değildir — o başka bir
/// şeydir ve adı `Kredi kartlarım`dır (ADR 0015).
class CashPage extends StatefulWidget {
  const CashPage({
    required this.cashController,
    required this.posController,
    super.key,
    this.scopeController,
    this.ownsControllers = true,
    this.initialTab = 0,
  });

  final CashCountController cashController;
  final PosController posController;
  final ScopeController? scopeController;
  final bool ownsControllers;

  /// `1`: açılışta yeni POS tahsilatı formu açılır.
  ///
  /// `İşlem ekle > POS tahsilatı` buraya gelir; eskiden ikinci sekmeyi
  /// seçiyordu, sekmeler kalkınca doğrudan formu açıyor.
  final int initialTab;

  @override
  State<CashPage> createState() => _CashPageState();
}

class _CashPageState extends State<CashPage> {
  CashCountController get _cash => widget.cashController;

  @override
  void initState() {
    super.initState();
    _cash.addListener(_changed);
    _cash.load();
    if (widget.initialTab == 1) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) return;
        showPosSettlementForm(
          context,
          widget.posController,
          widget.scopeController,
        );
      });
    }
  }

  @override
  void dispose() {
    _cash.removeListener(_changed);
    if (widget.ownsControllers) {
      widget.cashController.dispose();
      widget.posController.dispose();
    }
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  Future<void> _refresh() =>
      Future.wait([_cash.load(), widget.posController.load()]);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Kasa',
              actions: [
                IconButton(
                  tooltip: 'Bütün sayımlar',
                  onPressed: _cash.hasCashAccount
                      ? () => showAllCashCounts(context, _cash)
                      : null,
                  icon: const Icon(Icons.history),
                ),
              ],
            ),
            if (_cash.accounts.length > 1)
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.medium,
                  0,
                  AppSpacing.medium,
                  AppSpacing.small + AppSpacing.xSmall,
                ),
                child: _CashPicker(controller: _cash),
              ),
            Expanded(child: _body()),
          ],
        ),
      ),
    );
  }

  Widget _body() {
    final controller = _cash;
    if (controller.isLoading && controller.today == null) {
      return const AppLoadingView(message: 'Kasa yükleniyor');
    }
    if (controller.unauthorized && controller.today == null) {
      return const AppUnauthorizedView();
    }
    final hasAccount = controller.hasCashAccount;
    final today = controller.today;
    if (hasAccount && today == null) {
      return AppErrorView(
        message: controller.errorMessage ?? 'Kasa okunamadı.',
        onRetry: controller.load,
      );
    }

    return RefreshIndicator(
      onRefresh: _refresh,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.xSmall,
          AppSpacing.medium,
          AppSpacing.fabClearance,
        ),
        children: [
          if (controller.isStale)
            Padding(
              padding: const EdgeInsets.only(bottom: AppSpacing.small),
              child: AppInlineNotice(
                message:
                    '${controller.errorMessage} Son bilinen sayım '
                    'gösteriliyor.',
                actionLabel: 'Yenile',
                onAction: controller.load,
              ),
            ),
          if (today != null)
            CashTodayCard(
              controller: controller,
              today: today,
              onCount: () => showCashCountSheet(
                context,
                controller,
                widget.scopeController,
              ),
              onSaveDifference: () =>
                  showCashDifferenceForm(context, controller),
            )
          else
            const AppCard(
              child: AppEmptyView(
                title: 'Sayılacak bir kasa yok.',
                message:
                    'Gün sonu sayımı yalnız nakit hesaplar içindir; banka '
                    'bakiyesi elle sayılmaz. Önce bir nakit hesap açın.',
                icon: Icons.point_of_sale_outlined,
              ),
            ),
          const SizedBox(height: AppSpacing.large - AppSpacing.xSmall),
          PosSection(
            controller: widget.posController,
            scopeController: widget.scopeController,
          ),
          if (hasAccount) ...[
            const SizedBox(height: AppSpacing.large - AppSpacing.xSmall),
            CashPastCountsSection(
              controller: controller,
              onShowAll: () => showAllCashCounts(context, controller),
            ),
          ],
        ],
      ),
    );
  }
}

/// Kasa seçici: her dilim kasanın adını ve uygulamaya göre bakiyesini
/// birlikte söyler. İşletme kasası dükkan ikonuyla, diğerleri cüzdanla.
class _CashPicker extends StatelessWidget {
  const _CashPicker({required this.controller});

  final CashCountController controller;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final accounts = controller.accounts;
    final selected = accounts.firstWhere(
      (account) => account.id == controller.selectedAccountId,
      orElse: () => accounts.first,
    );
    String? balanceOf(CashAccount account) {
      final balance = controller.expectedByAccount[account.id];
      return balance == null ? null : MoneyText.format(balance, 'TRY');
    }

    return AppSegmentRail<CashAccount>(
      values: accounts,
      selected: selected,
      semanticLabel: 'Kasa',
      radius: AppRadius.field,
      minSegmentHeight: 56,
      onChanged: (account) => controller.selectAccount(account.id),
      segmentLabel: (account) => [account.name, ?balanceOf(account)].join(', '),
      segmentBuilder: (context, account, isSelected) {
        final ink = isSelected ? surfaces.ink : surfaces.inkMuted;
        final balance = balanceOf(account);
        return Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: AppSpacing.small + AppSpacing.xSmall,
            vertical: AppSpacing.xSmall,
          ),
          child: Row(
            children: [
              Icon(
                account.defaultScope == TransactionScope.business
                    ? Icons.storefront_outlined
                    : Icons.wallet,
                size: 20,
                color: ink,
              ),
              const SizedBox(width: AppSpacing.small + AppSpacing.xxSmall),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      account.name,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w600,
                        height: 1.25,
                        color: ink,
                      ),
                    ),
                    if (balance != null) ...[
                      const SizedBox(height: AppSpacing.xxSmall),
                      Text(
                        balance,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: AppTypography.money(theme.textTheme.labelSmall!)
                            .copyWith(
                              fontWeight: FontWeight.w500,
                              color: surfaces.inkMuted,
                            ),
                      ),
                    ],
                  ],
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}
