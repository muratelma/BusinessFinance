import '../../../core/theme/app_spacing.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_finance_icons.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_day_heading.dart';
import '../../../core/widgets/app_filter_menu.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_month_picker.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../../../core/widgets/app_share_bar.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_summary_card.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../activities/data/activity_models.dart';
import '../../activities/data/activity_repository.dart';
import '../../activities/presentation/activity_controller.dart';
import '../../activities/presentation/activity_tile.dart';
import '../../activities/presentation/show_activity_detail.dart';
import '../../day_close/data/day_close_repository.dart';
import '../../pos/data/pos_repository.dart';
import '../data/finance_page_models.dart';
import '../data/finance_page_repository.dart';
import 'finance_page_controller.dart';

/// Aynı okuma sözleşmesinin dört sayfası; işlem satırı ve ayrıntı ortaktır.
class FinanceReportPage extends StatefulWidget {
  const FinanceReportPage({
    required this.repository,
    required this.kind,
    super.key,
    this.id,
    this.month,
    this.income = false,
    this.scope,
    this.source,
    this.person,
    this.changes,
    this.scopeController,
    this.activityRepository,
    this.posRepository,
    this.dayCloseRepository,
    this.onAccountSettings,
  });
  final FinancePageRepositoryContract repository;
  final FinancePageKind kind;
  final String? id;
  final DateTime? month;
  final bool income;
  final TransactionScope? scope;
  final String? source;
  final String? person;
  final FinancialDataChanges? changes;
  final ScopeController? scopeController;
  final ActivityRepositoryContract? activityRepository;
  final PosRepositoryContract? posRepository;
  final DayCloseRepositoryContract? dayCloseRepository;
  final Future<void> Function()? onAccountSettings;
  @override
  State<FinanceReportPage> createState() => _FinanceReportPageState();
}

class _FinanceReportPageState extends State<FinanceReportPage> {
  late final FinancePageController controller = FinancePageController(
    widget.repository,
    kind: widget.kind,
    id: widget.id,
    month: widget.month,
    income: widget.income,
    scope: widget.scope,
    source: widget.source,
    person: widget.person,
    changes: widget.changes,
    scopeController: widget.scopeController,
  );
  ActivityController? details;
  final scroll = ScrollController();
  final names = <String, String>{};
  bool hasPeople = false;
  String? filterNotice;
  @override
  void initState() {
    super.initState();
    if (widget.activityRepository != null) {
      details = ActivityController(
        widget.activityRepository!,
        financialDataChanges: widget.changes,
        observeChanges: false,
      );
    }
    scroll.addListener(() {
      if (scroll.hasClients && scroll.position.extentAfter < 250) {
        controller.loadMore();
      }
    });
    controller.load();
  }

  @override
  void dispose() {
    controller.dispose();
    details?.dispose();
    scroll.dispose();
    super.dispose();
  }

  bool get isAccount => widget.kind == FinancePageKind.account;
  bool get isSummary => widget.kind == FinancePageKind.summary;
  bool get isTransit => widget.kind == FinancePageKind.transit;
  String get title => isAccount
      ? controller.account?.name ?? 'Hesap'
      : isTransit
      ? 'Yolda'
      : isSummary
      ? 'Kategori özeti'
      : controller.category?.name ??
            (widget.id == null ? 'Kategorisi yok' : 'Kategori');
  AppMoneyEffect get effect =>
      controller.income ? AppMoneyEffect.income : AppMoneyEffect.expense;
  Future<void> _month() async {
    final value = await AppMonthPicker.showWithAmounts(
      context: context,
      initialMonth: controller.month,
      load: controller.months,
      caption: isAccount
          ? 'Ay sonu bakiyesi'
          : controller.income
          ? 'Aylık gelir'
          : 'Aylık gider',
      effect: isAccount ? null : effect,
    );
    if (value != null && mounted) await controller.chooseMonth(value);
  }

  Future<void> _detail(FinancePageActivity row) async {
    if (details == null) return;
    await showActivityDetail(
      context,
      activity: row.activity,
      controller: details!,
      pageRepository: widget.repository,
      posRepository: widget.posRepository,
      dayCloseRepository: widget.dayCloseRepository,
      changes: widget.changes,
    );
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: AnimatedBuilder(
        animation: controller,
        builder: (context, _) {
          for (final option in [
            ...?controller.categorySummary?.sources,
            ...?controller.category?.people,
            ...?controller.account?.options,
          ]) {
            names[option.key] = _optionLabel(option);
          }
          hasPeople =
              (controller.category?.hasPeople ?? hasPeople) ||
              controller.person != null;
          return Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AppPageHeader(
                title: title,
                compact: true,
                onBack: () => Navigator.maybePop(context),
                actions: [
                  if (isAccount && widget.onAccountSettings != null)
                    IconButton(
                      tooltip: 'Hesap ayarları',
                      onPressed: widget.onAccountSettings,
                      icon: const Icon(Icons.tune),
                    ),
                  if (!isTransit)
                    TextButton(
                      onPressed: _month,
                      child: Row(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text(
                            DateText.monthYear(
                              controller.month.year,
                              controller.month.month,
                            ),
                          ),
                          const Icon(Icons.expand_more, size: 18),
                        ],
                      ),
                    ),
                ],
              ),
              if (!isTransit && (isAccount || isSummary)) _filters(),
              if (controller.isStale)
                Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.medium,
                  ),
                  child: AppInlineNotice(
                    message:
                        'Bilgiler güncellenemedi. Son alınan veriler gösteriliyor.',
                    actionLabel: 'Yenile',
                    onAction: controller.load,
                  ),
                ),
              if (controller.isLoading && controller.hasData)
                const LinearProgressIndicator(),
              Expanded(child: _body()),
            ],
          );
        },
      ),
    ),
  );

  Widget _filters() => Padding(
    padding: const EdgeInsets.only(
      left: AppSpacing.medium,
      right: AppSpacing.medium,
      bottom: AppSpacing.small,
    ),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (isSummary) ...[
          AppSegmentRail<bool>(
            values: const [false, true],
            selected: controller.income,
            onChanged: controller.chooseIncome,
            semanticLabel: 'Gelir veya gider',
            segmentLabel: (v) => v ? 'Gelir' : 'Gider',
            segmentBuilder: (context, v, selected) => Padding(
              padding: const EdgeInsets.all(AppSpacing.small),
              child: Text(
                v ? 'Gelir' : 'Gider',
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.titleSmall,
              ),
            ),
          ),
          const SizedBox(height: AppSpacing.small),
        ],
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: Row(
            children: [
              if (isAccount) ...[
                _chip(
                  'Tümü',
                  controller.direction == null,
                  () => controller.chooseDirection(null),
                ),
                const SizedBox(width: AppSpacing.small),
                _chip(
                  'Giren',
                  controller.direction == 'in',
                  () => controller.chooseDirection('in'),
                ),
                const SizedBox(width: AppSpacing.small),
                _chip(
                  'Çıkan',
                  controller.direction == 'out',
                  () => controller.chooseDirection('out'),
                ),
                const SizedBox(width: AppSpacing.small),
                AppFilterMenu(
                  label: names[controller.selection] ?? 'Tür',
                  selected: controller.selection,
                  options: _accountOptions(),
                  onSelected: controller.chooseSelection,
                ),
              ] else ...[
                if (isSummary &&
                    (widget.scopeController?.isVisible ?? false)) ...[
                  AppScopeSwitch(
                    value: controller.scope,
                    onChanged: widget.scopeController!.select,
                    compact: true,
                  ),
                  const SizedBox(width: AppSpacing.small),
                ],
                AppFilterMenu(
                  label: names[controller.source] ?? 'Hesap / kart',
                  muted:
                      !isSummary &&
                      controller.source == null &&
                      (controller.categorySummary?.sources
                                  .where(
                                    (p) =>
                                        p.key != 'all' && p.amount != '0.0000',
                                  )
                                  .length ??
                              0) <=
                          1,
                  onUnavailable: () => setState(
                    () =>
                        filterNotice = 'Bu seçimde başka hesap veya kart yok.',
                  ),
                  selected: controller.source,
                  options: _sourceOptions(),
                  onSelected: (v) {
                    setState(() => filterNotice = null);
                    controller.chooseSource(v == 'all' ? null : v);
                  },
                ),
                if (!isSummary && hasPeople) ...[
                  const SizedBox(width: AppSpacing.small),
                  AppFilterMenu(
                    label: names[controller.person] ?? 'Kişi',
                    muted:
                        controller.person == null &&
                        !(controller.category?.people.any(
                              (p) =>
                                  p.key.startsWith('person:') &&
                                  p.amount != '0.0000',
                            ) ??
                            false),
                    onUnavailable: () => setState(
                      () => filterNotice = names[controller.source] == null
                          ? 'Bu seçimde kişi yok.'
                          : '${names[controller.source]} kayıtlarında kişi yok.',
                    ),
                    selected: controller.person,
                    options: [
                      const AppFilterOption(
                        key: null,
                        label: 'Tüm kişiler',
                        icon: Icons.layers_outlined,
                      ),
                      for (final p
                          in controller.category?.people ??
                              <FinancePageOption>[])
                        if (p.key != 'all')
                          AppFilterOption(
                            key: p.key,
                            label: _optionLabel(p),
                            amount: p.amount,
                            icon: Icons.person_outline,
                          ),
                    ],
                    onSelected: (v) =>
                        controller.choosePerson(v == 'all' ? null : v),
                  ),
                ],
              ],
            ],
          ),
        ),
      ],
    ),
  );
  Widget _chip(String label, bool selected, VoidCallback onTap) => ChoiceChip(
    label: Text(label),
    selected: selected,
    showCheckmark: false,
    onSelected: (_) => onTap(),
    materialTapTargetSize: MaterialTapTargetSize.padded,
  );
  String _optionLabel(FinancePageOption option) {
    if (option.name != null) return option.name!;
    if (option.key == 'none') return 'Kişi yok';
    if (option.key == 'all') return 'Tümü';
    if (option.group == 'deferred') return 'Vadeli';
    if (option.group == 'in' || option.group == 'out') {
      return option.kind == 'category'
          ? 'Kategori'
          : _groupLabel(option.kind ?? 'all');
    }
    if (option.kind != null) return ActivityKind.fromApi(option.kind!).label;
    return _groupLabel(option.group);
  }

  String _groupLabel(String group) => switch (group) {
    'accounts' || 'account' => 'Nakit ve banka',
    'cards' || 'card' => 'Kredi kartı',
    'deferred' => 'Hesaptan geçmeyen',
    'category' => 'Kategori',
    'payment' => 'Ödeme',
    'collection' => 'Tahsilat',
    'transfer' => 'Transfer',
    _ => 'Tümü',
  };
  List<AppFilterOption> _sourceOptions() => [
    AppFilterOption(
      key: null,
      label: 'Hepsi',
      amount:
          controller.categorySummary?.sources
              .where((p) => p.key == 'all')
              .firstOrNull
              ?.amount ??
          controller.categorySummary?.total,
      icon: Icons.layers_outlined,
    ),
    for (final p
        in controller.categorySummary?.sources ?? <FinancePageOption>[])
      if (p.key != 'all')
        AppFilterOption(
          key: p.key,
          label: _optionLabel(p),
          amount: p.amount,
          subtitle: p.group == 'deferred'
              ? controller.income
                    ? 'Veresiye satış ve alacaklar'
                    : 'Vadeli alım, fatura ve borçla alınanlar'
              : null,
          group: _groupLabel(p.group),
          icon: p.group == 'cards'
              ? Icons.credit_card
              : p.group == 'deferred'
              ? Icons.schedule
              : Icons.account_balance_outlined,
        ),
  ];
  List<AppFilterOption> _accountOptions() => [
    AppFilterOption(
      key: null,
      label: 'Hepsi',
      amount: controller.direction == 'in'
          ? controller.account?.inflow
          : controller.direction == 'out'
          ? controller.account?.outflow
          : null,
      icon: Icons.layers_outlined,
    ),
    for (final p in [
      ...?controller.account?.options.where((p) => p.key.startsWith('in:')),
      ...?controller.account?.options.where((p) => !p.key.startsWith('in:')),
    ])
      if (p.key != 'all')
        AppFilterOption(
          key: p.key,
          label: _optionLabel(p),
          amount: p.amount,
          signedByAmount: true,
          effect: p.group == 'category' || p.kind == 'category'
              ? p.key.startsWith('in:')
                    ? AppMoneyEffect.income
                    : AppMoneyEffect.expense
              : AppMoneyEffect.neutral,
          group: controller.direction == null
              ? (p.key.startsWith('in:') ? 'Giren' : 'Çıkan')
              : _groupLabel(p.group),
          icon: p.group == 'category'
              ? Icons.local_offer_outlined
              : p.group == 'transfer'
              ? Icons.swap_horiz
              : Icons.payments_outlined,
        ),
  ];
  Widget _body() {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (!controller.hasData) {
      return controller.errorMessage != null
          ? AppErrorView(
              message: controller.errorMessage!,
              onRetry: controller.load,
            )
          : const AppLoadingView();
    }
    return RefreshIndicator(
      onRefresh: controller.load,
      child: ListView(
        controller: scroll,
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.only(bottom: AppSpacing.xLarge),
        children: [
          if (isAccount)
            _accountSummary()
          else if (isTransit)
            _transitSummary()
          else
            _categorySummary(),
          if (!isSummary && !isAccount && !isTransit) _filters(),
          if (filterNotice != null) AppInlineNotice(message: filterNotice!),
          if (isSummary)
            _categories()
          else if (controller.items.isEmpty)
            _empty()
          else
            ..._days(),
          if (controller.hasMore)
            Padding(
              padding: const EdgeInsets.all(AppSpacing.medium),
              child: OutlinedButton(
                onPressed: controller.isLoadingMore
                    ? null
                    : controller.loadMore,
                child: Text(
                  controller.isLoadingMore
                      ? 'Yükleniyor…'
                      : 'Daha fazla göster',
                ),
              ),
            ),
          if (!isSummary && controller.items.isNotEmpty)
            Padding(
              padding: const EdgeInsets.all(AppSpacing.medium),
              child: Text(
                '${controller.totalCount} işlem',
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.bodySmall,
              ),
            ),
          if (controller.errorMessage != null && !controller.isStale)
            Padding(
              padding: const EdgeInsets.all(AppSpacing.medium),
              child: Text(controller.errorMessage!),
            ),
        ],
      ),
    );
  }

  Widget _categorySummary() {
    final data = controller.categorySummary!;
    final selecting =
        controller.source != null ||
        controller.person != null ||
        (controller.scope != null && controller.category?.showScope == true);
    return Padding(
      padding: const EdgeInsets.only(
        left: AppSpacing.medium,
        top: AppSpacing.small,
        right: AppSpacing.medium,
        bottom: AppSpacing.small + AppSpacing.xSmall,
      ),
      child: AppSummaryCard(
        label:
            '${DateText.months[controller.month.month - 1]} ${controller.income ? 'geliri' : 'gideri'}${controller.scope == null || (!isSummary && controller.category?.showScope != true) ? '' : ' · ${controller.scope!.label}'}${controller.source == null || names[controller.source] == null ? '' : ' · ${names[controller.source]}'}',
        amount: data.total,
        effect: isSummary ? null : effect,
        subtitle: isSummary
            ? 'Geçen ay ${MoneyText.format(data.previousTotal, 'TRY')}'
            : selecting
            ? 'Kategorinin tamamı ${MoneyText.format(controller.category!.wholeTotal, 'TRY')}'
            : 'Geçen ay ${MoneyText.format(data.previousTotal, 'TRY')}',
      ),
    );
  }

  Widget _categories() {
    final rows = controller.categorySummary!.categories;
    if (rows.isEmpty) return _empty();
    final regular = rows.where((row) => row.id != null).toList();
    final missing = rows.where((row) => row.id == null).toList();
    Widget rowCard(List<CategoryPageSlice> slices, {bool dashed = false}) =>
        AppCard(
          padding: EdgeInsets.zero,
          dashed: dashed,
          child: Column(
            children: [
              for (var i = 0; i < slices.length; i++) ...[
                if (i > 0) const Divider(height: 1),
                AppRow(
                  onTap: () => context.push(
                    categoryPageLocation(
                      slices[i].id,
                      controller.month,
                      income: controller.income,
                      scope: controller.scope,
                      source: controller.source,
                    ),
                  ),
                  padding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 14,
                  ),
                  minHeight: 40,
                  title: slices[i].name ?? 'Kategorisi yok',
                  titleMaxLines: 2,
                  subtitle: 'Pay %${MoneyText.percent(slices[i].share)}',
                  leading: AppIconCapsule(
                    icon: slices[i].id == null
                        ? Icons.category_outlined
                        : AppFinanceIcons.forCategory(slices[i].name!),
                    tone: slices[i].id == null
                        ? null
                        : controller.income
                        ? AppStatusTone.income
                        : AppStatusTone.expense,
                  ),
                  trailing: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.end,
                        children: [
                          AppMoneyText(
                            amount: slices[i].amount,
                            currency: 'TRY',
                            size: AppMoneySize.row,
                          ),
                          const SizedBox(
                            height: AppSpacing.xSmall + AppSpacing.xxSmall,
                          ),
                          SizedBox(
                            width: 96,
                            child: AppShareBar(
                              ratio: slices[i].barRatio,
                              color: controller.income
                                  ? AppFinanceColors.of(context).income
                                  : AppFinanceColors.of(context).expense,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(width: AppSpacing.small),
                      const Icon(Icons.chevron_right, size: 20),
                    ],
                  ),
                ),
              ],
            ],
          ),
        );
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (regular.isNotEmpty) rowCard(regular),
          if (missing.isNotEmpty) ...[
            const Padding(
              padding: EdgeInsets.fromLTRB(
                AppSpacing.xSmall,
                AppSpacing.medium,
                AppSpacing.xSmall,
                AppSpacing.small,
              ),
              child: Text('Bu tutarın kategorisi bulunamadı'),
            ),
            rowCard(missing, dashed: true),
          ],
        ],
      ),
    );
  }

  Widget _accountSummary() {
    final data = controller.account!;
    final month = DateText.months[controller.month.month - 1];
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppSummaryCard(
            label: data.isCurrentMonth ? 'Bakiye' : '$month sonu bakiye',
            amount: data.balance,
            subtitle: !data.isCurrentMonth
                ? 'Bugün ${MoneyText.format(data.currentBalance, 'TRY')}'
                : null,
            footer: Column(
              children: [
                const Divider(height: 1),
                const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
                Wrap(
                  spacing: 24,
                  runSpacing: 12,
                  children: [
                    _metric('$month giren', data.inflow, AppMoneyEffect.income),
                    _metric(
                      '$month çıkan',
                      data.outflow,
                      AppMoneyEffect.expense,
                    ),
                    _metric('Net', data.net, null),
                  ],
                ),
              ],
            ),
          ),
          if (data.transit.count > 0) ...[
            const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
            _transitPreview(data.transit),
          ],
          if (controller.selection != null)
            Padding(
              padding: const EdgeInsets.only(
                top: AppSpacing.small + AppSpacing.xSmall,
              ),
              child: AppRow(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.xSmall,
                ),
                title: names[controller.selection] ?? 'Seçimin toplamı',
                trailing: AppMoneyText(
                  amount: data.selectionTotal,
                  currency: 'TRY',
                  size: AppMoneySize.row,
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _transitPreview(AccountTransitSummary transit) {
    final nearest = transit.nearest;
    final colors = AppFinanceColors.of(context);
    return AppCard(
      background: colors.neutralContainer,
      padding: const EdgeInsets.all(AppSpacing.small + AppSpacing.xSmall),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppRow(
            padding: const EdgeInsets.only(
              left: AppSpacing.xSmall,
              right: AppSpacing.xSmall,
              bottom: AppSpacing.small + AppSpacing.xxSmall,
            ),
            minHeight: 0,
            title: 'Yolda',
            titleStyle: Theme.of(
              context,
            ).textTheme.titleSmall?.copyWith(color: colors.onNeutralContainer),
            trailing: AppMoneyText(
              amount: transit.total,
              currency: 'TRY',
              effect: AppMoneyEffect.neutral,
              onContainer: true,
              size: AppMoneySize.row,
            ),
          ),
          if (nearest != null)
            AppCard(
              onTap: () => _detail(nearest),
              padding: const EdgeInsets.symmetric(
                horizontal:
                    AppSpacing.small + AppSpacing.xSmall + AppSpacing.xxSmall,
                vertical: AppSpacing.small + AppSpacing.xSmall,
              ),
              child: AppRow(
                padding: EdgeInsets.zero,
                minHeight: 0,
                title: nearest.activity.kindLabel,
                subtitle:
                    '${DateText.dayMonth(nearest.activity.expectedTransferDate ?? nearest.activity.activityDate)} bekleniyor',
                trailing: Column(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    AppMoneyText(
                      amount: nearest.amount,
                      currency: 'TRY',
                      effect: AppMoneyEffect.neutral,
                      signedByAmount: true,
                      size: AppMoneySize.row,
                    ),
                    if (nearest.activity.feeAmount != null)
                      Text(
                        '${MoneyText.format(nearest.activity.feeAmount!, 'TRY')} komisyon sonrası',
                        style: Theme.of(context).textTheme.bodySmall,
                      ),
                  ],
                ),
              ),
            ),
          if (transit.count > 1)
            TextButton(
              onPressed: () =>
                  context.push('$accountsLocation/${widget.id}/in-transit'),
              child: Text('Tümünü gör · ${transit.count} satış ›'),
            ),
        ],
      ),
    );
  }

  Widget _metric(String label, String amount, AppMoneyEffect? effect) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      Text(label, style: Theme.of(context).textTheme.bodySmall),
      const SizedBox(height: AppSpacing.xSmall),
      AppMoneyText(
        amount: amount,
        currency: 'TRY',
        effect: effect,
        size: AppMoneySize.row,
      ),
    ],
  );
  Widget _transitSummary() => Padding(
    padding: const EdgeInsets.all(AppSpacing.medium),
    child: AppSummaryCard(
      label: "${controller.transit!.name}'e geçecek",
      amount: controller.transit!.total,
      effect: AppMoneyEffect.neutral,
      subtitle:
          '${controller.transit!.totalCount} kartlı satış · komisyon sonrası',
    ),
  );
  Widget _empty() {
    if (isTransit) {
      return const SizedBox(
        height: 360,
        child: AppEmptyView(
          title: 'Yolda satış yok',
          message: 'Parası henüz hesaba geçmemiş satışlar burada görünür.',
          icon: Icons.point_of_sale_outlined,
        ),
      );
    }
    final filters =
        controller.source != null ||
        controller.person != null ||
        controller.selection != null;
    return SizedBox(
      height: 360,
      child: AppEmptyView(
        title: filters
            ? 'Bu seçime uyan işlem yok'
            : isSummary
            ? 'Bu ay kayıt yok'
            : 'Bu ay işlem yok',
        message: filters
            ? 'Başka bir hesap, kart veya kişi seçebilirsiniz.'
            : 'Başka bir aya bakabilirsiniz.',
        icon: Icons.receipt_long_outlined,
        action: TextButton(
          onPressed: filters ? controller.clearFilters : _month,
          child: Text(filters ? 'Filtreleri temizle' : 'Ay seç'),
        ),
      ),
    );
  }

  List<Widget> _days() {
    final groups = <String, List<FinancePageActivity>>{};
    for (final row in controller.items) {
      groups.putIfAbsent(row.activity.activityDate, () => []).add(row);
    }
    return [
      for (final entry in groups.entries) ...[
        AppDayHeading(entry.key),
        Padding(
          padding: EdgeInsets.symmetric(
            horizontal: isAccount || isTransit ? 0 : AppSpacing.medium,
          ),
          child: AppCard(
            flat: isAccount || isTransit,
            padding: EdgeInsets.zero,
            child: Column(
              children: [
                for (var i = 0; i < entry.value.length; i++) ...[
                  if (i > 0) const Divider(height: 1, indent: 72),
                  _tile(entry.value[i]),
                ],
              ],
            ),
          ),
        ),
      ],
    ];
  }

  Widget _tile(FinancePageActivity row) {
    final a = row.activity;
    final piece = row.part != null;
    final subtitle = <String>[];
    if (!isAccount &&
        !isTransit &&
        !piece &&
        (a.kind == ActivityKind.obligation ||
            a.kind == ActivityKind.counterpartyCharge ||
            a.kind == ActivityKind.debtOpening)) {
      subtitle.add('Vadeli');
    }
    if (piece && !isAccount && !isTransit) {
      final parent = MoneyText.format(row.parentAmount ?? a.amount, 'TRY');
      subtitle.add(switch (row.part) {
        'interest' => '$parent taksidin faizi',
        'fee' => '$parent transferin masrafı',
        'deduction' => '$parent yatışın kesintisi',
        'commission' =>
          a.isCardCollection
              ? '$parent tahsilatın komisyonu'
              : '$parent satışın komisyonu',
        _ => '$parent · ${a.kindLabel}',
      });
    }
    if (isAccount && a.categoryName != null) subtitle.add(a.categoryName!);
    if (isAccount &&
        a.effect == ActivityEffect.neutral &&
        a.kind != ActivityKind.transfer &&
        a.kind != ActivityKind.posDeposit) {
      subtitle.add(a.kindLabel);
    }
    for (final value in [a.sourceName, a.destinationName, a.channelName]) {
      if (value != null &&
          value != a.title &&
          value != controller.account?.name &&
          value != controller.category?.name &&
          !subtitle.contains(value)) {
        subtitle.add(value);
      }
    }
    if (isAccount &&
        a.kind == ActivityKind.posDeposit &&
        row.firstSaleDate != null) {
      subtitle.add(
        row.firstSaleDate == row.lastSaleDate
            ? DateText.dayMonth(row.firstSaleDate!)
            : '${DateText.dayMonth(row.firstSaleDate!)}–${DateText.dayMonth(row.lastSaleDate!)}',
      );
    }
    if (row.showScope && row.scope != null && !isAccount) {
      subtitle.add(row.scope!.label);
    }
    final state = switch (row.state) {
      'open' => 'Açık',
      'paid' || 'settled' =>
        '${controller.income ? 'Tahsil edildi' : 'Ödendi'}${row.settledOn == null ? '' : ' · ${DateText.dayMonth(row.settledOn!)}'}${row.settledSource == null ? '' : ' · ${row.settledSource}'}',
      'transit' =>
        'Yolda${a.expectedTransferDate == null ? '' : ' · ${DateText.dayMonth(a.expectedTransferDate!)} bekleniyor'}',
      'deposited' =>
        'Hesaba geçti${a.transferredOn == null ? '' : ' · ${DateText.dayMonth(a.transferredOn!)}'}',
      _ => null,
    };
    final pieceName = switch (row.part) {
      'interest' => 'Faiz',
      'commission' => 'Komisyon',
      'deduction' => 'Kesinti',
      'fee' || 'transfer-fee' => 'Masraf',
      _ => null,
    };
    final selectedEffect = isAccount
        ? a.effect
        : isTransit
        ? ActivityEffect.neutral
        : row.amount.startsWith('-')
        ? ActivityEffect.expense
        : ActivityEffect.income;
    return ActivityTile(
      activity: a,
      showDate: false,
      onTap: () => _detail(row),
      title: isTransit
          ? a.kindLabel
          : piece && !isAccount && pieceName != null
          ? a.kindLabel
          : (a.title.isEmpty ? a.kindLabel : a.title),
      subtitle: subtitle.join(' · '),
      stateLabel: state,
      stateTone:
          row.state == 'settled' ||
              row.state == 'paid' ||
              row.state == 'deposited'
          ? AppStatusTone.income
          : row.state == 'transit'
          ? AppStatusTone.neutral
          : null,
      amount: row.amount,
      effect: selectedEffect,
      signedByAmount: isAccount,
      hideFee: isAccount || isTransit || piece,
      amountNote: isTransit && a.feeAmount != null
          ? '${MoneyText.format(a.feeAmount!, 'TRY')} komisyon sonrası'
          : isAccount && row.partAmount != null && pieceName != null
          ? '${MoneyText.format(row.partAmount!, 'TRY')} ${pieceName.toLowerCase()} ${row.part == 'deduction' ? 'sonrası' : 'dahil'}'
          : null,
    );
  }
}
