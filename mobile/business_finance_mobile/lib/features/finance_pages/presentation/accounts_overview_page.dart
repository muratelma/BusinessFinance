import '../../../core/theme/app_spacing.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_summary_card.dart';
import '../../../core/widgets/app_untracked_text.dart';
import '../data/finance_page_models.dart';
import '../data/finance_page_repository.dart';
import 'finance_page_controller.dart';

/// Hesaplar: toplam, toplamın nakit / banka / pasif dağılımı ve gruplar.
///
/// Tasarım HS1e (14 Ekim 2026). Pay çubuğu ve yüzdeler sunucudan gelir;
/// istemci pay hesaplamaz. Pay gelmediyse (eksi bakiyeli hesap) çubuk ve
/// yüzde çizilmez, tutarlar kalır.
class AccountsOverviewPage extends StatelessWidget {
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
  Widget build(BuildContext context) =>
      AppUntrackedText(child: _AccountsOverviewPageBody(this));
}

class _AccountsOverviewPageBody extends StatefulWidget {
  const _AccountsOverviewPageBody(this.page);
  final AccountsOverviewPage page;
  @override
  State<_AccountsOverviewPageBody> createState() =>
      _AccountsOverviewPageState();
}

class _AccountsOverviewPageState extends State<_AccountsOverviewPageBody> {
  late final controller = FinancePageController(
    widget.page.repository,
    kind: FinancePageKind.accounts,
    changes: widget.page.changes,
  );

  /// Satırda parçaların arası ve ayırıcının başladığı yer (teslimde 14 / 70).
  static const double _gap =
      AppSpacing.small + AppSpacing.xSmall + AppSpacing.xxSmall;
  static const double _dividerInset = AppSpacing.medium + 40 + _gap;

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
        builder: (context, _) {
          final hasAccounts = controller.accounts?.items.isNotEmpty ?? false;
          return Column(
            children: [
              AppPageHeader(
                title: 'Hesaplar',
                compact: true,
                onBack: () => Navigator.maybePop(context),
                actions: [
                  if (hasAccounts)
                    IconButton(
                      tooltip: 'Transfer yap',
                      onPressed: () => context.push(transferLocation),
                      icon: const Icon(Icons.swap_horiz),
                    ),
                  if (hasAccounts && widget.page.onCreate != null)
                    IconButton(
                      tooltip: 'Hesap ekle',
                      onPressed: widget.page.onCreate,
                      icon: const Icon(Icons.add),
                    ),
                ],
              ),
              if (controller.isStale)
                TextButton(
                  onPressed: controller.load,
                  child: const Text('Son alınan veriler · Yenile'),
                ),
              Expanded(child: _body(context)),
            ],
          );
        },
      ),
    ),
  );

  Widget _body(BuildContext context) {
    if (controller.unauthorized) return const AppUnauthorizedView();
    final data = controller.accounts;
    if (data == null) {
      return controller.errorMessage != null
          ? AppErrorView(
              message: controller.errorMessage!,
              onRetry: controller.load,
            )
          : const AppLoadingView();
    }
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final cashColor = colors.categorySlice(2);
    final bankColor = colors.categorySlice(4);
    final passiveColor = colors.categoryOtherSlice;
    final cash = data.items.where((a) => a.isActive && a.type == 'cash');
    final bank = data.items.where((a) => a.isActive && a.type == 'bank');
    final passive = data.items.where((a) => !a.isActive);
    return RefreshIndicator(
      onRefresh: controller.load,
      child: ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: EdgeInsets.only(
          left: AppSpacing.medium,
          top: data.items.isEmpty ? 0 : AppSpacing.small,
          right: AppSpacing.medium,
          bottom: AppSpacing.xLarge,
        ),
        children: [
          if (data.items.isEmpty) ...[
            const AppEmptyView(
              compact: true,
              title: 'Henüz hesap yok',
              message:
                  'Kasanızı ya da banka hesabınızı ekleyerek başlayabilirsiniz.',
              icon: Icons.event_busy_outlined,
            ),
            if (widget.page.onCreate != null)
              FilledButton.icon(
                onPressed: widget.page.onCreate,
                style: FilledButton.styleFrom(
                  minimumSize: const Size.fromHeight(52),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(16),
                  ),
                ),
                icon: const Icon(Icons.add, size: 20),
                label: const Text('Hesap ekle'),
              ),
          ] else
            _totalCard(data, [
              (
                label: 'Nakit',
                amount: data.cashTotal,
                share: data.cashShare,
                color: cashColor,
              ),
              (
                label: 'Banka',
                amount: data.bankTotal,
                share: data.bankShare,
                color: bankColor,
              ),
              // Pasif bakiye sıfırsa dilimi ve satırı yoktur.
              if (data.inactiveTotal != '0.0000')
                (
                  label: 'Pasif',
                  amount: data.inactiveTotal,
                  share: data.inactiveShare,
                  color: passiveColor,
                ),
            ]),
          ..._group('Nakit', cash.toList(), cashColor),
          ..._group('Banka', bank.toList(), bankColor),
          ..._group('Pasif', passive.toList(), surfaces.inkMuted),
        ],
      ),
    );
  }

  /// Toplam kartı: tutar, pay çubuğu ve her dilimin adı, yüzdesi, tutarı.
  Widget _totalCard(
    AccountsOverviewData data,
    List<({String label, String amount, String? share, Color color})> parts,
  ) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    // Çubuk yalnız bütün paylar geldiyse çizilir; sıfır paylı dilim atlanır.
    final slices = [
      for (final part in parts)
        (
          color: part.color,
          flex: ((double.tryParse(part.share ?? '') ?? 0) * 100).round(),
        ),
    ].where((slice) => slice.flex > 0).toList();
    final hasShares = parts.every((p) => p.share != null) && slices.isNotEmpty;
    Widget partAmount(String amount) => Text(
      MoneyText.format(amount, 'TRY'),
      textAlign: TextAlign.end,
      style: theme.textTheme.bodyMedium?.copyWith(
        height: 1.35,
        fontWeight: FontWeight.w600,
        color: surfaces.ink,
        fontFeatures: AppTypography.tabularFigures,
      ),
    );
    return AppCard(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium + AppSpacing.xSmall,
        AppSpacing.medium,
        AppSpacing.medium + AppSpacing.xSmall,
        AppSpacing.medium + AppSpacing.xxSmall,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text('Toplam bakiye', style: AppSummaryCard.helperStyle(context)),
          const SizedBox(height: AppSpacing.xSmall),
          FittedBox(
            fit: BoxFit.scaleDown,
            alignment: Alignment.centerLeft,
            child: AppMoneyText(
              amount: data.total,
              currency: 'TRY',
              size: AppMoneySize.metric,
              style: const TextStyle(
                fontSize: 32,
                height: 1.25,
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
          if (hasShares) ...[
            const SizedBox(height: AppSpacing.medium),
            // Çubuğun yalnız iki ucu yuvarlaktır; dilimler arası düz kesilir.
            ExcludeSemantics(
              child: ClipRRect(
                borderRadius: BorderRadius.circular(999),
                child: Row(
                  children: [
                    for (var i = 0; i < slices.length; i++) ...[
                      if (i > 0)
                        const SizedBox(
                          width: AppSpacing.xxSmall + AppSpacing.xxSmall / 2,
                        ),
                      Expanded(
                        flex: slices[i].flex,
                        child: Container(height: 10, color: slices[i].color),
                      ),
                    ],
                  ],
                ),
              ),
            ),
          ],
          const SizedBox(
            height: AppSpacing.small + AppSpacing.xSmall + AppSpacing.xxSmall,
          ),
          for (var i = 0; i < parts.length; i++) ...[
            if (i > 0)
              const SizedBox(height: AppSpacing.small + AppSpacing.xxSmall),
            MergeSemantics(
              child: Row(
                children: [
                  Container(
                    width: 10,
                    height: 10,
                    decoration: BoxDecoration(
                      color: parts[i].color,
                      shape: BoxShape.circle,
                    ),
                  ),
                  const SizedBox(width: AppSpacing.small + AppSpacing.xxSmall),
                  Expanded(
                    child: Text(
                      parts[i].label,
                      style: theme.textTheme.bodyMedium?.copyWith(height: 1.35),
                    ),
                  ),
                  if (hasShares)
                    Text(
                      '%${MoneyText.percent(parts[i].share!)}',
                      style: AppSummaryCard.smallStyle(context),
                    ),
                  if (!context.usesLargeText)
                    ConstrainedBox(
                      constraints: const BoxConstraints(minWidth: 104),
                      child: Padding(
                        padding: const EdgeInsets.only(left: AppSpacing.small),
                        child: partAmount(parts[i].amount),
                      ),
                    ),
                ],
              ),
            ),
            // Büyük yazıda tutar adın altına iner; yan yana sığmıyor.
            if (context.usesLargeText)
              Align(
                alignment: Alignment.centerRight,
                child: partAmount(parts[i].amount),
              ),
          ],
        ],
      ),
    );
  }

  /// Grup: `Nakit · 3` başlığı ve hesapların kartı.
  List<Widget> _group(
    String label,
    List<AccountOverviewItem> items,
    Color iconColor,
  ) {
    if (items.isEmpty) return [];
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return [
      Padding(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.xSmall,
          AppSpacing.large,
          AppSpacing.xSmall,
          AppSpacing.small,
        ),
        child: Semantics(
          header: true,
          child: Text(
            '$label · ${items.length}',
            style: theme.textTheme.labelMedium?.copyWith(
              height: 1,
              letterSpacing: 0.52,
              color: surfaces.inkFaint,
            ),
          ),
        ),
      ),
      AppCard(
        padding: EdgeInsets.zero,
        child: Column(
          children: [
            for (var i = 0; i < items.length; i++) ...[
              if (i > 0) const Divider(height: 1, indent: _dividerInset),
              _row(items[i], iconColor),
            ],
          ],
        ),
      ),
    ];
  }

  /// Hesap satırı: gri kapsülde grubun rengiyle ikon, ad ve kapsam, bakiye.
  /// Pasif hesap soluk yazılır ve kapsamın yerinde `Pasif` etiketi taşır.
  Widget _row(AccountOverviewItem item, Color iconColor) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final ink = item.isActive ? surfaces.ink : surfaces.inkMuted;
    final money = Text(
      MoneyText.format(item.balance, 'TRY'),
      style: theme.textTheme.titleSmall?.copyWith(
        height: 1.25,
        color: ink,
        fontFeatures: AppTypography.tabularFigures,
      ),
    );
    return MergeSemantics(
      child: Semantics(
        button: true,
        child: InkWell(
          onTap: () => context.push('$accountsLocation/${item.id}'),
          child: Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.small + AppSpacing.xSmall,
              AppSpacing.small + AppSpacing.xSmall,
              AppSpacing.small + AppSpacing.xSmall,
            ),
            child: Row(
              children: [
                AppIconCapsule(
                  icon: item.type == 'cash'
                      ? Icons.payments_outlined
                      : Icons.account_balance_outlined,
                  iconColor: iconColor,
                ),
                const SizedBox(width: _gap),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        item.name,
                        maxLines: context.usesLargeText ? 2 : 1,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.titleSmall?.copyWith(
                          height: 1.3,
                          color: ink,
                        ),
                      ),
                      if (!item.isActive)
                        Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(
                              Icons.pause_circle_outline,
                              size: 16,
                              color: surfaces.inkMuted,
                            ),
                            const SizedBox(width: AppSpacing.xSmall),
                            Text(
                              'Pasif',
                              style: AppSummaryCard.smallStyle(
                                context,
                              )?.copyWith(fontWeight: FontWeight.w600),
                            ),
                          ],
                        )
                      else if (item.defaultScope != null)
                        Text(
                          item.defaultScope!.label,
                          style: AppSummaryCard.helperStyle(context),
                        ),
                      if (context.usesLargeText) ...[
                        const SizedBox(height: AppSpacing.xSmall),
                        money,
                      ],
                    ],
                  ),
                ),
                if (!context.usesLargeText) ...[
                  const SizedBox(width: _gap),
                  money,
                ],
                const SizedBox(width: _gap),
                Icon(Icons.chevron_right, size: 20, color: surfaces.inkMuted),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
