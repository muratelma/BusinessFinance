import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_avatar.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../account/presentation/account_status_controller.dart';
import '../../activities/data/planned_activity_models.dart';
import '../../planning/data/planning_models.dart';
import '../data/dashboard_models.dart';
import 'dashboard_sections.dart';
import 'dashboard_view_model.dart';

/// Özet: ayın akışı (gelir, gider, net, kategori, bütçe, yaklaşanlar) ve şu
/// anki durum (varlık, hesap bakiyeleri).
///
/// Kapsam değişince akış bölümleri o kapsama göre sunucudan yeniden okunur;
/// varlık durumu ve hesap bakiyeleri tek havuzdur ve değişmez (ADR 0013).
class DashboardPage extends StatelessWidget {
  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<DashboardViewModel>();
    final status = context.watch<AccountStatusController?>();
    final state = _stateView(viewModel);
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: Column(
          children: [
            AppPageHeader(
              title: 'Özet',
              actions: [
                // Hesabın kapısı: doğrulanmamış e-postanın kalıcı uyarısı
                // avatarın üstünde kırmızı nokta.
                IconButton(
                  tooltip: status?.needsEmailVerification ?? false
                      ? 'Hesabım, e-posta doğrulanmadı'
                      : 'Hesabım',
                  onPressed: () => context.push(accountLocation),
                  icon: AppAvatar(
                    initials: status?.initials ?? '',
                    badge: status?.needsEmailVerification ?? false,
                  ),
                ),
              ],
            ),
            // Anahtar kaydırılan gövdenin **dışında**: uygulamanın tek kapsam
            // denetimi bu ve yükleme, hata ya da boş durumda da yerinde durur.
            if (viewModel.isScopeVisible)
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.medium,
                  AppSpacing.xSmall,
                  AppSpacing.medium,
                  AppSpacing.small + AppSpacing.xSmall,
                ),
                child: AppScopeSwitch(
                  value: viewModel.scope,
                  onChanged: (value) =>
                      context.read<DashboardViewModel>().selectScope(value),
                ),
              ),
            Expanded(child: state ?? _buildReport(context, viewModel)),
          ],
        ),
      ),
    );
  }

  Widget _buildReport(BuildContext context, DashboardViewModel viewModel) {
    final report = viewModel.report!;
    final advanced = viewModel.advanced;
    final budgets = advanced?.budgetVariances ?? const <BudgetVarianceItem>[];

    return RefreshIndicator(
      onRefresh: viewModel.load,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.xSmall,
          AppSpacing.medium,
          AppSpacing.fabClearance,
        ),
        children: [
          _MonthStepper(
            label: DateText.monthYear(report.year, report.month),
            onPrevious: viewModel.previousMonth,
            onNext: viewModel.nextMonth,
          ),
          // Gecikme varsa ekranın söylemesi gereken ilk şey odur; yokken
          // şerit hiç çizilmez.
          if (viewModel.overdue.isNotEmpty)
            Padding(
              padding: const EdgeInsets.only(
                top: AppSpacing.small,
                bottom: AppSpacing.medium,
              ),
              child: _OverdueBand(items: viewModel.overdue),
            )
          else
            const SizedBox(height: AppSpacing.small),
          _HeroCard(viewModel: viewModel, report: report, advanced: advanced),

          DashboardBlock(
            title: 'Kategori giderleri',
            count: report.categoryExpenses.isEmpty
                ? null
                : '${report.categoryExpenses.length} kategori',
            child: DashboardCategoryCard(report: report),
          ),
          if (advanced != null)
            DashboardBlock(
              title: 'Bütçeler',
              count: budgets.isEmpty ? null : _budgetCount(budgets),
              onOpen: () => context.push(budgetsLocation),
              child: DashboardBudgetRings(
                items: budgets,
                currency: advanced.currency,
                onOpen: () => context.push(budgetsLocation),
              ),
            ),
          DashboardBlock(
            title: 'Yaklaşanlar',
            count: DashboardViewModel.upcomingHorizon.label,
            onOpen: () => context.push(plannedLocation),
            child: DashboardUpcomingCard(
              items: viewModel.upcoming,
              overdue: viewModel.overdue,
              overdueTotal: viewModel.overdueOutgoingTotal,
              total: viewModel.upcomingOutgoingTotal,
              currency: report.currency,
              today: viewModel.today,
              onOpen: () => context.push(plannedLocation),
            ),
          ),
          if (advanced != null)
            DashboardBlock(
              title: 'Varlık durumu',
              note: viewModel.scope == null ? null : unsplitNote,
              child: DashboardNetWorthCard(report: advanced),
            ),
          DashboardBlock(
            title: 'Hesap bakiyeleri',
            note: viewModel.scope == null ? null : unsplitNote,
            onOpen: () => context.push(accountsLocation),
            child: DashboardAccountBalances(
              report: report,
              onOpen: () => context.push(accountsLocation),
            ),
          ),
        ],
      ),
    );
  }

  /// Halkada gösterilen bütçe sayısı; dörtten fazlası varsa en dolu dördü.
  static String _budgetCount(List<BudgetVarianceItem> items) =>
      '${DashboardBudgetRings.shownCount(items.length)} bütçe';

  /// Kapsam filtresinden etkilenmeyen bölümün notu.
  ///
  /// Bakiye, kart borcu ve net varlık tek havuzdur (ADR 0013); filtre açıkken
  /// sessizce aynı kalan bir sayı filtrelenmiş sanılır.
  static const unsplitNote =
      'Kapsam filtresinden etkilenmez: işletme ve şahsi toplamıdır.';
}

extension on DashboardPage {
  Widget? _stateView(DashboardViewModel viewModel) {
    if (viewModel.isLoading && !viewModel.hasLoaded) {
      return const AppLoadingView(message: 'Finansal özet yükleniyor');
    }
    if (viewModel.error case final error?) {
      if (error.isUnauthorized) return const AppUnauthorizedView();
      return AppErrorView(message: error.message, onRetry: viewModel.load);
    }
    if (viewModel.report == null) {
      return const AppEmptyView(
        icon: Icons.account_balance_wallet_outlined,
        title: 'Finansal özet bulunamadı',
        message: 'Özetinizi yenilemek için tekrar deneyin.',
      );
    }
    return null;
  }
}

/// Ay seçici: solda dönem (bölüm başlığı ölçeğinde), sağda iki ok.
class _MonthStepper extends StatelessWidget {
  const _MonthStepper({
    required this.label,
    required this.onPrevious,
    required this.onNext,
  });

  final String label;
  final VoidCallback onPrevious;
  final VoidCallback onNext;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: Semantics(
            header: true,
            child: Text(label, style: Theme.of(context).textTheme.titleMedium),
          ),
        ),
        // Okların glifi sayfa kenarına hizalansın diye dokunma alanı sağa
        // taşar; metin yerinde kalır.
        Transform.translate(
          offset: const Offset(AppSpacing.small + AppSpacing.xSmall, 0),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              IconButton(
                tooltip: 'Önceki ay',
                onPressed: onPrevious,
                icon: const Icon(Icons.chevron_left),
              ),
              IconButton(
                tooltip: 'Sonraki ay',
                onPressed: onNext,
                icon: const Icon(Icons.chevron_right),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

/// Gecikmiş ödemelerin uyarı şeridi: sayı ve en eski vade. Dokununca
/// Planlananlar açılır.
///
/// **Tutar toplamı yok.** Şeridin taşıdığı bilgi sayı ve en eski vade: ikisi de
/// sayma işidir, para aritmetiği değil. Yalnız ödeme **yükümlülükleri**
/// sayılır; bunu sunucu işaretler.
class _OverdueBand extends StatelessWidget {
  const _OverdueBand({required this.items});

  final List<PlannedActivity> items;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final oldest = items
        .map((item) => item.dueDate)
        .reduce((a, b) => a.compareTo(b) <= 0 ? a : b);
    final foreground = colors.onExpenseContainer;
    final radius = BorderRadius.circular(AppRadius.card);

    return Semantics(
      container: true,
      liveRegion: true,
      button: true,
      label:
          '${items.length} gecikmiş ödeme var. '
          'En eskisinin vadesi ${DateText.dayMonth(oldest)}. '
          'Onaylanana kadar kayda geçmez. Planlananları açar.',
      child: ExcludeSemantics(
        child: Material(
          color: colors.expenseContainer,
          borderRadius: radius,
          child: InkWell(
            borderRadius: radius,
            onTap: () => context.push(plannedLocation),
            child: ConstrainedBox(
              constraints: const BoxConstraints(minHeight: 52),
              child: Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.medium,
                  AppSpacing.small,
                  AppSpacing.small,
                  AppSpacing.small,
                ),
                child: Row(
                  children: [
                    // Renk tek başına anlam taşımaz: ikon ve metin de var.
                    Icon(
                      Icons.warning_amber_outlined,
                      size: 20,
                      color: foreground,
                    ),
                    const SizedBox(width: AppSpacing.small),
                    Flexible(
                      child: Text(
                        '${items.length} gecikmiş ödeme',
                        style: theme.textTheme.titleSmall?.copyWith(
                          fontSize: 15,
                          height: 1.3,
                          color: foreground,
                        ),
                      ),
                    ),
                    const SizedBox(width: AppSpacing.small),
                    Expanded(
                      child: Text(
                        'En eskisi ${DateText.dayMonth(oldest)}',
                        textAlign: TextAlign.right,
                        maxLines: 2,
                        style: theme.textTheme.bodyMedium?.copyWith(
                          height: 1.3,
                          color: foreground,
                        ),
                      ),
                    ),
                    // Ok tarihe yapışık duruyordu (kullanıcı, 30 Eylül).
                    const SizedBox(width: AppSpacing.small),
                    Icon(Icons.chevron_right, size: 22, color: foreground),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Ayın gelir, gider ve neti tek kartta.
///
/// Üstte gelir ve gider satırları, kalın bir çizgi, altında net (hero).
/// Kapsam boyutu görünür ve anahtar `Hepsi`'deyse net kartının altında
/// işletme neti ile şahsi taraf ayrı satırlarda durur; üçü de sunucudan gelir
/// (`scopeBreakdown`), istemci çıkarma yapmaz.
class _HeroCard extends StatelessWidget {
  const _HeroCard({
    required this.viewModel,
    required this.report,
    required this.advanced,
  });

  final DashboardViewModel viewModel;
  final DashboardReport report;
  final AdvancedReport? advanced;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final breakdown = viewModel.isScopeVisible && viewModel.scope == null
        ? report.scopeBreakdown
        : null;
    final caption = viewModel.scope == null ? _netCaption(advanced) : null;

    return AppCard(
      key: const ValueKey('dashboard-hero'),
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.large - AppSpacing.xSmall,
        AppSpacing.medium,
        AppSpacing.large - AppSpacing.xSmall,
        AppSpacing.large - AppSpacing.xSmall,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          _FlowLine(
            key: const ValueKey('dashboard-summary-Gelir'),
            label: 'Gelir',
            icon: Icons.south_west,
            tone: AppStatusTone.income,
            effect: AppMoneyEffect.income,
            amount: report.totalIncome,
            currency: report.currency,
          ),
          _FlowLine(
            key: const ValueKey('dashboard-summary-Gider'),
            label: 'Gider',
            icon: Icons.north_east,
            tone: AppStatusTone.expense,
            effect: AppMoneyEffect.expense,
            amount: report.totalExpense,
            currency: report.currency,
          ),
          Padding(
            padding: const EdgeInsets.only(
              top: AppSpacing.small,
              bottom: AppSpacing.medium,
            ),
            child: Container(height: 2, color: surfaces.ink),
          ),
          Semantics(
            key: const ValueKey('dashboard-summary-Net'),
            container: true,
            label:
                '${_netLabel(viewModel.scope)}: '
                '${MoneyText.format(report.net, report.currency)}'
                '${caption == null ? '' : '. ${caption.text}'}',
            child: ExcludeSemantics(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Wrap(
                    alignment: WrapAlignment.spaceBetween,
                    crossAxisAlignment: WrapCrossAlignment.end,
                    spacing: AppSpacing.small + AppSpacing.xSmall,
                    children: [
                      Padding(
                        padding: const EdgeInsets.only(
                          bottom: AppSpacing.small,
                        ),
                        child: Text(
                          _netLabel(viewModel.scope),
                          style: theme.textTheme.titleSmall?.copyWith(
                            height: 1.3,
                          ),
                        ),
                      ),
                      AppMoneyText(
                        amount: report.net,
                        currency: report.currency,
                        size: AppMoneySize.hero,
                      ),
                    ],
                  ),
                  if (caption != null) _caption(context, caption),
                ],
              ),
            ),
          ),
          if (breakdown != null) ...[
            const SizedBox(height: AppSpacing.medium),
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
                vertical: AppSpacing.xSmall,
              ),
              decoration: BoxDecoration(
                color: surfaces.cardMuted,
                borderRadius: BorderRadius.circular(AppRadius.field),
              ),
              child: Column(
                children: [
                  _SplitLine(
                    key: const ValueKey('dashboard-summary-Business'),
                    icon: Icons.storefront_outlined,
                    label: 'İşletme neti',
                    amount: breakdown.business.net,
                    currency: report.currency,
                  ),
                  Divider(height: 1, thickness: 1, color: surfaces.border),
                  _SplitLine(
                    key: const ValueKey('dashboard-summary-Personal'),
                    icon: Icons.person_outline,
                    label: _netLabel(TransactionScope.personal),
                    amount: breakdown.personal.net,
                    currency: report.currency,
                  ),
                ],
              ),
            ),
          ],
        ],
      ),
    );
  }

  /// Geçen ayla karşılaştırma satırı: sağa yaslı, eğilim ikonuyla.
  Widget _caption(
    BuildContext context,
    ({String text, IconData? trend}) caption,
  ) {
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: const EdgeInsets.only(top: AppSpacing.small),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.end,
        children: [
          if (caption.trend != null) ...[
            Icon(caption.trend, size: 18, color: surfaces.inkMuted),
            const SizedBox(width: AppSpacing.small),
          ],
          Flexible(
            child: Text(
              caption.text,
              textAlign: TextAlign.right,
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ),
        ],
      ),
    );
  }

  /// Anahtarın konumuna göre netin adı; "kâr" değil — muhasebe kârı satılan
  /// malın maliyetini ister ve ürün sınırının dışındadır.
  static String _netLabel(TransactionScope? scope) => switch (scope) {
    null => 'Bu ayın neti',
    TransactionScope.business => 'İşletme neti',
    TransactionScope.personal => 'Şahsi net',
  };

  /// Geçen ayla karşılaştırma. Fark sunucudan gelir (`netChange`); eski
  /// sunucu göndermiyorsa satır açıklamaya döner.
  static ({String text, IconData? trend})? _netCaption(
    AdvancedReport? advanced,
  ) {
    final change = advanced?.netChange;
    if (advanced == null || change == null) {
      return (text: 'Gelir eksi gider', trend: null);
    }
    if (double.tryParse(change) == 0) {
      return (text: 'Geçen ayla aynı', trend: Icons.trending_flat);
    }
    final worse = MoneyText.isNegative(change);
    final amount = MoneyText.format(
      MoneyText.unsigned(change),
      advanced.currency,
    );
    return (
      text: 'Geçen aya göre $amount ${worse ? 'daha düşük' : 'daha iyi'}',
      trend: worse ? Icons.trending_down : Icons.trending_up,
    );
  }
}

/// Hero kartın gelir veya gider satırı: 32'lik rol kapsülü, ad, işaretli
/// tutar.
class _FlowLine extends StatelessWidget {
  const _FlowLine({
    required this.label,
    required this.icon,
    required this.tone,
    required this.effect,
    required this.amount,
    required this.currency,
    super.key,
  });

  final String label;
  final IconData icon;
  final AppStatusTone tone;
  final AppMoneyEffect effect;
  final String amount;
  final String currency;

  @override
  Widget build(BuildContext context) {
    return Semantics(
      container: true,
      label: '$label: ${MoneyText.format(amount, currency)}',
      child: ExcludeSemantics(
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 48),
          child: Row(
            children: [
              AppIconCapsule(icon: icon, tone: tone, size: 32),
              const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
              Expanded(
                child: _LabelAndAmount(
                  label: Text(
                    label,
                    style: Theme.of(context).textTheme.bodyLarge,
                  ),
                  amount: AppMoneyText(
                    amount: amount,
                    currency: currency,
                    effect: effect,
                    signed: true,
                    size: AppMoneySize.row,
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// `Hepsi` konumunda netin iki tarafı: işletme neti ve şahsi taraf.
class _SplitLine extends StatelessWidget {
  const _SplitLine({
    required this.icon,
    required this.label,
    required this.amount,
    required this.currency,
    super.key,
  });

  final IconData icon;
  final String label;
  final String amount;
  final String currency;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      container: true,
      label: '$label: ${MoneyText.format(amount, currency)}',
      child: ExcludeSemantics(
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 48),
          child: Row(
            children: [
              Icon(icon, size: 20, color: surfaces.inkMuted),
              const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
              Expanded(
                child: _LabelAndAmount(
                  label: Text(
                    label,
                    style: Theme.of(context).textTheme.bodyMedium,
                  ),
                  amount: AppMoneyText(
                    amount: amount,
                    currency: currency,
                    size: AppMoneySize.row,
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Ad ve tutar yan yana; büyük yazıda tutar adın altına, sağa iner.
class _LabelAndAmount extends StatelessWidget {
  const _LabelAndAmount({required this.label, required this.amount});

  final Widget label;
  final Widget amount;

  @override
  Widget build(BuildContext context) {
    if (context.usesLargeText) {
      return Padding(
        padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            label,
            Align(alignment: Alignment.centerRight, child: amount),
          ],
        ),
      );
    }
    return Row(
      children: [
        Expanded(child: label),
        const SizedBox(width: AppSpacing.small),
        amount,
      ],
    );
  }
}
