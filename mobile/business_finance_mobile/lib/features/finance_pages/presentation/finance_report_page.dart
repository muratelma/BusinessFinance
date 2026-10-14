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
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_icons.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_day_heading.dart';
import '../../../core/widgets/app_filter_menu.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_month_picker.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../../../core/widgets/app_share_bar.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_summary_card.dart';
import '../../../core/widgets/app_untracked_text.dart';
import '../../../core/theme/app_surfaces.dart';
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
class FinanceReportPage extends StatelessWidget {
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
  Widget build(BuildContext context) =>
      AppUntrackedText(child: _FinanceReportPageBody(this));
}

class _FinanceReportPageBody extends StatefulWidget {
  const _FinanceReportPageBody(this.page);
  final FinanceReportPage page;
  @override
  State<_FinanceReportPageBody> createState() => _FinanceReportPageState();
}

class _FinanceReportPageState extends State<_FinanceReportPageBody> {
  late final FinancePageController controller = FinancePageController(
    widget.page.repository,
    kind: widget.page.kind,
    id: widget.page.id,
    month: widget.page.month,
    income: widget.page.income,
    scope: widget.page.scope,
    source: widget.page.source,
    person: widget.page.person,
    changes: widget.page.changes,
    scopeController: widget.page.scopeController,
  );
  ActivityController? details;
  final scroll = ScrollController();
  final names = <String, String>{};
  bool hasPeople = false;

  /// Teslimin 14 dp adımı: kategori satırının dikey boşluğu ve Yolda
  /// kutusundaki satırın yan boşluğu.
  static const double _rowGap =
      AppSpacing.small + AppSpacing.xSmall + AppSpacing.xxSmall;
  String? filterNotice;
  @override
  void initState() {
    super.initState();
    if (widget.page.activityRepository != null) {
      details = ActivityController(
        widget.page.activityRepository!,
        financialDataChanges: widget.page.changes,
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

  bool get isAccount => widget.page.kind == FinancePageKind.account;
  bool get isSummary => widget.page.kind == FinancePageKind.summary;
  bool get isTransit => widget.page.kind == FinancePageKind.transit;
  String get title => isAccount
      ? controller.account?.name ?? 'Hesap'
      : isTransit
      ? 'Yolda'
      : isSummary
      ? 'Kategori özeti'
      : controller.category?.name ??
            (widget.page.id == null ? 'Kategorisi yok' : 'Kategori');
  AppMoneyEffect get effect =>
      controller.income ? AppMoneyEffect.income : AppMoneyEffect.expense;
  Future<void> _month() async {
    final value = await AppMonthPicker.showWithAmounts(
      context: context,
      initialMonth: controller.month,
      load: controller.months,
      caption: isAccount
          ? '$title · ay sonu bakiye'
          : '$title · ${controller.income ? 'Gelir' : 'Gider'}${controller.scope == null ? '' : ' · ${controller.scope!.label}'}${controller.source == null ? '' : ' · ${names[controller.source] ?? ''}'}',
      effect: isAccount ? null : effect,
    );
    if (value != null && mounted) await controller.chooseMonth(value);
  }

  Future<void> _detail(FinancePageActivity row) async {
    if (details == null) return;
    // Ayrıntı İşlemler'deki ortak paneldir; her yerden aynı görünsün diye
    // sayfanın harf aralıksız temasını değil, gezginin temasını alır.
    await showActivityDetail(
      Navigator.of(context).context,
      activity: row.activity,
      controller: details!,
      pageRepository: widget.page.repository,
      posRepository: widget.page.posRepository,
      dayCloseRepository: widget.page.dayCloseRepository,
      changes: widget.page.changes,
    );
  }

  @override
  Widget build(BuildContext context) =>
      Scaffold(body: SafeArea(bottom: false, child: _page()));

  Widget _page() => AnimatedBuilder(
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
              if (isAccount && widget.page.onAccountSettings != null)
                IconButton(
                  tooltip: 'Hesap ayarları',
                  onPressed: widget.page.onAccountSettings,
                  icon: const Icon(Icons.tune, size: 22),
                ),
              if (!isTransit) _monthButton(context),
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
  );

  /// Başlıktaki ay düğmesi: 48 dp, 16/600, yanında 20 dp ok.
  Widget _monthButton(BuildContext context) => InkWell(
    onTap: _month,
    borderRadius: BorderRadius.circular(16),
    child: ConstrainedBox(
      constraints: const BoxConstraints(minHeight: 48),
      child: Padding(
        padding: const EdgeInsets.only(
          left: AppSpacing.small + AppSpacing.xxSmall,
          right: AppSpacing.xSmall + AppSpacing.xxSmall,
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              DateText.monthYear(controller.month.year, controller.month.month),
              style: Theme.of(
                context,
              ).textTheme.titleSmall?.copyWith(height: 1),
            ),
            const SizedBox(width: AppSpacing.xxSmall),
            const Icon(Icons.expand_more, size: 20),
          ],
        ),
      ),
    ),
  );

  Widget _filters() => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      if (isSummary)
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            AppSpacing.xSmall,
            AppSpacing.medium,
            0,
          ),
          child: AppSegmentRail<bool>(
            values: const [false, true],
            selected: controller.income,
            onChanged: controller.chooseIncome,
            semanticLabel: 'Gelir veya gider',
            minSegmentHeight: 36,
            segmentLabel: (v) => v ? 'Gelir' : 'Gider',
            segmentBuilder: (context, v, selected) => Padding(
              padding: const EdgeInsets.symmetric(horizontal: AppSpacing.small),
              child: Text(
                v ? 'Gelir' : 'Gider',
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.titleSmall?.copyWith(
                  fontSize: 15,
                  height: 1,
                  color: selected
                      ? Theme.of(context).colorScheme.onSurface
                      : Theme.of(context).colorScheme.onSurfaceVariant,
                ),
              ),
            ),
          ),
        ),
      // Çipler kenardan kenara kayar; hap kendi 8/4 dp boşluğunu taşır.
      SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
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
                width: controller.direction == 'out' ? 340 : 320,
                maxHeight: controller.direction == 'out' ? 600 : 520,
                caption:
                    '${DateText.months[controller.month.month - 1]}${controller.direction == null
                        ? ''
                        : controller.direction == 'in'
                        ? ' giren'
                        : ' çıkan'} · $title',
                selected: controller.selection,
                options: _accountOptions(),
                onSelected: controller.chooseSelection,
              ),
            ] else ...[
              if (isSummary &&
                  (widget.page.scopeController?.isVisible ?? false)) ...[
                AppScopeSwitch(
                  value: controller.scope,
                  onChanged: widget.page.scopeController!.select,
                  compact: true,
                ),
                const SizedBox(width: AppSpacing.small),
              ],
              AppFilterMenu(
                label: names[controller.source] ?? 'Hesap / kart',
                alignToPageStart: !isSummary,
                width: _sourceOptions().length > 7 ? 348 : 340,
                maxHeight: _sourceOptions().length > 7 ? 690 : 520,
                caption: _filterCaption,
                muted:
                    controller.source == null &&
                    (controller.categorySummary?.sources
                                .where(
                                  (p) => p.key != 'all' && p.amount != '0.0000',
                                )
                                .length ??
                            0) <=
                        (isSummary ? 0 : 1),
                onUnavailable: () => setState(
                  () => filterNotice = 'Bu seçimde başka hesap veya kart yok.',
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
                  alignToPageStart: true,
                  caption: _filterCaption,
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
                    AppFilterOption(
                      key: null,
                      label: 'Hepsi',
                      amount: controller.category?.people
                          .where((p) => p.key == 'all')
                          .firstOrNull
                          ?.amount,
                      icon: Icons.layers_outlined,
                    ),
                    for (final p
                        in controller.category?.people ?? <FinancePageOption>[])
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
  );
  String get _filterCaption => !isSummary
      ? '${DateText.months[controller.month.month - 1]} · $title${controller.source == null ? '' : ' · ${names[controller.source] ?? ''}'}'
      : '${DateText.months[controller.month.month - 1]} ${controller.income ? 'geliri' : 'gideri'}${controller.scope == null ? '' : ' · ${controller.scope!.label}'}${controller.source == null ? '' : ' · ${names[controller.source] ?? ''}'}';
  Widget _chip(String label, bool selected, VoidCallback onTap) =>
      AppFilterChip(label: label, selected: selected, onSelected: onTap);
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
    if (option.kind != null) {
      return ActivityKind.fromApi(option.kind!).labelFor(
        option.key.startsWith('in:')
            ? ActivityDirection.receivable
            : ActivityDirection.payable,
      );
    }
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
              ? Icons.event_available_outlined
              : p.kind == 'cash'
              ? Icons.payments_outlined
              : Icons.account_balance_outlined,
        ),
  ];
  List<AppFilterOption> _accountOptions() => [
    AppFilterOption(
      key: null,
      label: 'Hepsi',
      amount: controller.direction == null
          ? null
          : controller.account?.options
                .where((p) => p.key == 'all')
                .firstOrNull
                ?.amount,
      signedByAmount: controller.direction != null,
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
          icon: p.group == 'category' || p.kind == 'category'
              ? p.name == null
                    ? Icons.category_outlined
                    : AppFinanceIcons.forCategory(p.name!)
              : p.group == 'transfer' || p.kind == 'transfer'
              ? Icons.swap_horiz
              // Ödeme ve tahsilat satırı, listede o işlemin taşıdığı ikonu
              // taşır.
              : p.kind == 'card-payment'
              ? Icons.credit_card
              : p.kind == 'pos-deposit'
              ? Icons.move_to_inbox_outlined
              : p.kind == 'debt-payment' || p.kind == 'debt-collection'
              ? Icons.request_quote_outlined
              : p.kind == 'counterparty-settlement'
              ? Icons.groups_outlined
              : p.kind == 'obligation-settlement'
              ? Icons.receipt_long_outlined
              : p.key.startsWith('in:')
              ? Icons.south_west
              : Icons.north_east,
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
          if (!isSummary &&
              !isAccount &&
              !isTransit &&
              (controller.items.isNotEmpty ||
                  controller.source != null ||
                  controller.person != null))
            _filters(),
          if (filterNotice != null)
            AppInlineNotice(
              message: filterNotice!,
              compact: true,
              margin: const EdgeInsets.fromLTRB(
                AppSpacing.medium,
                AppSpacing.xSmall,
                AppSpacing.medium,
                0,
              ),
            ),
          if (isSummary)
            _categories()
          else if (controller.items.isEmpty)
            _empty()
          else
            ..._days(),
          if (isTransit && controller.items.isNotEmpty)
            const AppInlineNotice(
              compact: true,
              margin: EdgeInsets.fromLTRB(
                AppSpacing.medium,
                AppSpacing.small + AppSpacing.xSmall,
                AppSpacing.medium,
                0,
              ),
              message:
                  'Para hesaba geçince kayıt buradan çıkar; hesapta POS yatışı olarak görünür.',
            ),
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
          if (controller.hasMore && controller.items.isNotEmpty)
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
                _categoryRow(slices[i]),
              ],
            ],
          ),
        );
    return Padding(
      padding: const EdgeInsets.only(
        left: AppSpacing.medium,
        top: AppSpacing.small + AppSpacing.xSmall,
        right: AppSpacing.medium,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (regular.isNotEmpty) rowCard(regular),
          if (missing.isNotEmpty) ...[
            Padding(
              padding: EdgeInsets.fromLTRB(
                AppSpacing.xSmall,
                regular.isEmpty ? AppSpacing.xSmall : AppSpacing.medium,
                AppSpacing.xSmall,
                AppSpacing.small,
              ),
              child: Text(
                'Bu tutarın kategorisi bulunamadı',
                style: AppSummaryCard.smallStyle(context),
              ),
            ),
            rowCard(missing, dashed: true),
          ],
        ],
      ),
    );
  }

  /// Kategori satırı: kapsül, ad ve pay, sağda tutar ile 96 dp pay çubuğu.
  Widget _categoryRow(CategoryPageSlice slice) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    final missing = slice.id == null;
    final amount = AppMoneyText(
      amount: slice.amount,
      currency: 'TRY',
      size: AppMoneySize.row,
      style: const TextStyle(height: 1.25),
    );
    // Çok küçük pay da görünür kalsın: çubuk en az yüzde üç dolar.
    final bar = SizedBox(
      width: 96,
      child: AppShareBar(
        ratio: slice.barRatio <= 0
            ? 0
            : slice.barRatio < 0.03
            ? 0.03
            : slice.barRatio,
        color: missing
            ? surfaces.inkFaint
            : controller.income
            ? colors.incomeFill
            : colors.expenseFill,
      ),
    );
    final text = Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(
          slice.name ?? 'Kategorisi yok',
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
          style: theme.textTheme.titleSmall?.copyWith(height: 1.3),
        ),
        Text(
          'Pay %${MoneyText.percent(slice.share)}',
          style: AppSummaryCard.helperStyle(context),
        ),
        if (context.usesLargeText) ...[
          const SizedBox(height: AppSpacing.xSmall),
          amount,
          const SizedBox(height: AppSpacing.xSmall + AppSpacing.xxSmall),
          bar,
        ],
      ],
    );
    return MergeSemantics(
      child: Semantics(
        button: true,
        child: InkWell(
          onTap: () => context.push(
            categoryPageLocation(
              slice.id,
              controller.month,
              income: controller.income,
              scope: controller.scope,
              source: controller.source,
            ),
          ),
          child: Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              _rowGap,
              AppSpacing.small + AppSpacing.xSmall,
              _rowGap,
            ),
            child: Row(
              children: [
                AppIconCapsule(
                  icon: missing
                      ? Icons.category_outlined
                      : AppFinanceIcons.forCategory(slice.name!),
                  tone: missing
                      ? null
                      : controller.income
                      ? AppStatusTone.income
                      : AppStatusTone.expense,
                ),
                const SizedBox(width: AppSpacing.medium),
                Expanded(child: text),
                if (!context.usesLargeText) ...[
                  const SizedBox(width: AppSpacing.medium),
                  SizedBox(
                    width: 112,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        FittedBox(fit: BoxFit.scaleDown, child: amount),
                        const SizedBox(
                          height: AppSpacing.xSmall + AppSpacing.xxSmall,
                        ),
                        bar,
                      ],
                    ),
                  ),
                ],
                const SizedBox(width: AppSpacing.medium),
                Icon(Icons.chevron_right, size: 20, color: surfaces.inkMuted),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _accountSummary() {
    final data = controller.account!;
    final month = DateText.months[controller.month.month - 1];
    final metrics = [
      _metric('$month giren', data.inflow, AppMoneyEffect.income),
      _metric('$month çıkan', data.outflow, AppMoneyEffect.expense),
      _metric('Net', data.net, null, signed: true),
    ];
    return Padding(
      padding: const EdgeInsets.only(
        left: AppSpacing.medium,
        top: AppSpacing.small,
        right: AppSpacing.medium,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppSummaryCard(
            label: data.isCurrentMonth ? 'Bakiye' : '$month sonu bakiye',
            amount: data.balance,
            subtitle: !data.isCurrentMonth
                ? 'Bugün ${MoneyText.format(data.currentBalance, 'TRY')}'
                : null,
            footer: context.usesLargeText
                ? Wrap(
                    spacing: AppSpacing.large,
                    runSpacing: AppSpacing.small + AppSpacing.xSmall,
                    children: metrics,
                  )
                : Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      for (var i = 0; i < metrics.length; i++) ...[
                        if (i > 0) const SizedBox(width: AppSpacing.small),
                        Expanded(child: metrics[i]),
                      ],
                    ],
                  ),
          ),
          if (data.isCurrentMonth &&
              controller.direction != 'out' &&
              data.transit.count > 0) ...[
            const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
            _transitPreview(data.transit),
          ],
          if (controller.selection != null)
            Padding(
              padding: const EdgeInsets.only(
                left: AppSpacing.xSmall,
                top: AppSpacing.medium,
                right: AppSpacing.xSmall,
              ),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.baseline,
                textBaseline: TextBaseline.alphabetic,
                children: [
                  Expanded(
                    child: Text(
                      names[controller.selection] ?? 'Seçimin toplamı',
                      style: Theme.of(
                        context,
                      ).textTheme.titleSmall?.copyWith(height: 1.3),
                    ),
                  ),
                  const SizedBox(width: AppSpacing.small),
                  AppMoneyText(
                    amount:
                        data.options
                            .where((p) => p.key == controller.selection)
                            .firstOrNull
                            ?.amount ??
                        (controller.selection!.startsWith('out:') &&
                                data.selectionTotal != '0.0000'
                            ? '-${data.selectionTotal}'
                            : data.selectionTotal),
                    signedByAmount: true,
                    currency: 'TRY',
                    size: AppMoneySize.row,
                    style: const TextStyle(height: 1.25),
                  ),
                ],
              ),
            ),
        ],
      ),
    );
  }

  /// Yolda kutusu: mavi zemin, toplam, en yakın tek kayıt ve fazlası için
  /// `Tümünü gör`. Kutunun ve içindeki beyaz satırın kenarlığı yoktur.
  Widget _transitPreview(AccountTransitSummary transit) {
    final nearest = transit.nearest;
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final many = transit.count > 1;
    return Container(
      padding: EdgeInsets.fromLTRB(
        AppSpacing.small + AppSpacing.xSmall,
        AppSpacing.small + AppSpacing.xSmall,
        AppSpacing.small + AppSpacing.xSmall,
        many ? 0 : AppSpacing.small + AppSpacing.xSmall,
      ),
      decoration: BoxDecoration(
        color: colors.neutralContainer,
        borderRadius: BorderRadius.circular(20),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.xSmall,
              AppSpacing.xxSmall,
              AppSpacing.xSmall,
              AppSpacing.small + AppSpacing.xxSmall,
            ),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.baseline,
              textBaseline: TextBaseline.alphabetic,
              children: [
                Expanded(
                  child: Text(
                    'Yolda',
                    style: theme.textTheme.titleSmall?.copyWith(
                      height: 1.3,
                      color: colors.onNeutralContainer,
                    ),
                  ),
                ),
                const SizedBox(width: AppSpacing.small),
                AppMoneyText(
                  amount: transit.total,
                  currency: 'TRY',
                  effect: AppMoneyEffect.neutral,
                  onContainer: true,
                  size: AppMoneySize.row,
                  style: const TextStyle(
                    height: 1.25,
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
          if (nearest != null)
            Material(
              color: surfaces.card,
              borderRadius: BorderRadius.circular(16),
              clipBehavior: Clip.antiAlias,
              child: InkWell(
                onTap: () => _detail(nearest),
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: _rowGap,
                    vertical: AppSpacing.small + AppSpacing.xSmall,
                  ),
                  child: _transitRow(nearest),
                ),
              ),
            ),
          if (many)
            InkWell(
              onTap: () => context.push(
                '$accountsLocation/${widget.page.id}/in-transit',
              ),
              borderRadius: BorderRadius.circular(16),
              child: ConstrainedBox(
                constraints: const BoxConstraints(minHeight: 44),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Flexible(
                      child: Text(
                        'Tümünü gör · ${transit.count} satış',
                        style: theme.textTheme.bodyMedium?.copyWith(
                          height: 1,
                          fontWeight: FontWeight.w600,
                          color: colors.onNeutralContainer,
                        ),
                      ),
                    ),
                    const SizedBox(width: AppSpacing.xxSmall),
                    Icon(
                      Icons.chevron_right,
                      size: 20,
                      color: colors.onNeutralContainer,
                    ),
                  ],
                ),
              ),
            ),
        ],
      ),
    );
  }

  /// Yolda kutusundaki en yakın kayıt. Büyük yazıda tutar adın altına iner;
  /// yan yana sığmıyor.
  Widget _transitRow(FinancePageActivity nearest) {
    final theme = Theme.of(context);
    final stacked = context.usesLargeText;
    final texts = [
      Text(
        _transitTitle(nearest.activity),
        maxLines: stacked ? 2 : 1,
        overflow: TextOverflow.ellipsis,
        style: theme.textTheme.titleSmall?.copyWith(height: 1.3),
      ),
      Text(
        '${nearest.activity.isCardCollection ? 'Tahsilat · ' : ''}${DateText.dayShortMonth(nearest.activity.expectedTransferDate ?? nearest.activity.activityDate)} bekleniyor',
        maxLines: stacked ? 2 : 1,
        overflow: TextOverflow.ellipsis,
        style: AppSummaryCard.helperStyle(context),
      ),
    ];
    final amounts = [
      AppMoneyText(
        amount: nearest.amount,
        currency: 'TRY',
        effect: AppMoneyEffect.neutral,
        signedByAmount: true,
        size: AppMoneySize.row,
        style: const TextStyle(height: 1.25),
      ),
      if (nearest.activity.feeAmount != null)
        Text(
          '${MoneyText.format(nearest.activity.feeAmount!, 'TRY')} komisyon sonrası',
          textAlign: TextAlign.end,
          style: AppSummaryCard.smallStyle(context),
        ),
    ];
    if (stacked) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        mainAxisSize: MainAxisSize.min,
        children: [
          ...texts,
          const SizedBox(height: AppSpacing.xSmall),
          for (final amount in amounts)
            Align(alignment: Alignment.centerRight, child: amount),
        ],
      );
    }
    return Row(
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: texts,
          ),
        ),
        const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
        Column(
          crossAxisAlignment: CrossAxisAlignment.end,
          mainAxisSize: MainAxisSize.min,
          children: amounts,
        ),
      ],
    );
  }

  Widget _metric(
    String label,
    String amount,
    AppMoneyEffect? effect, {
    bool signed = false,
  }) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      Text(label, style: AppSummaryCard.smallStyle(context)),
      AppMoneyText(
        amount: amount,
        currency: 'TRY',
        effect: effect,
        signedByAmount: signed,
        size: AppMoneySize.row,
        style: const TextStyle(fontSize: 15, height: 1.25),
      ),
    ],
  );
  Widget _transitSummary() => Padding(
    padding: const EdgeInsets.only(
      left: AppSpacing.medium,
      top: AppSpacing.small,
      right: AppSpacing.medium,
    ),
    child: AppSummaryCard(
      label: "${controller.transit!.name}'e geçecek",
      amount: controller.transit!.total,
      effect: AppMoneyEffect.neutral,
      subtitle:
          '${controller.transit!.totalCount} kartlı satış · komisyon sonrası',
    ),
  );
  Widget _empty() {
    final month = DateText.monthLocative(controller.month.month);
    final filters =
        controller.source != null ||
        controller.person != null ||
        controller.selection != null;
    final selectedSource = isSummary && controller.source != null
        ? names[controller.source]
        : null;
    final direction = controller.income ? 'gelir' : 'gider';
    return AppEmptyView(
      compact: true,
      title: isTransit
          ? 'Yolda satış yok'
          : selectedSource != null
          ? '$month $selectedSource için $direction yok'
          : filters
          ? 'Bu seçime uyan işlem yok'
          : isAccount
          ? '$month bu hesapta hareket yok'
          : isSummary
          ? '$month $direction yok'
          : '$month bu kategoride $direction yok',
      message: isTransit
          ? 'Parası henüz hesaba geçmemiş satışlar burada görünür.'
          : selectedSource != null
          ? 'Seçim korunur; başka bir aya geçince yine $selectedSource gösterilir.'
          : filters
          ? 'Başka bir hesap, kart veya kişi seçebilirsiniz.'
          : isSummary
          ? '${controller.income ? 'Gelir' : 'Gider'} ekledikçe kategoriler burada toplanır.'
          : 'Başka bir ay seçebilirsiniz.',
      icon: Icons.event_busy_outlined,
      action: filters
          ? TextButton(
              onPressed: controller.clearFilters,
              child: Text(
                selectedSource != null
                    ? 'Filtreyi kaldır'
                    : 'Filtreleri temizle',
              ),
            )
          : null,
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
                  // Çizgi yazının başladığı yerden çizilir: kartta 70, tam
                  // genişlik blokta 72 dp.
                  if (i > 0)
                    Divider(
                      height: 1,
                      indent: isAccount || isTransit ? 72 : 70,
                    ),
                  _tile(entry.value[i]),
                ],
              ],
            ),
          ),
        ),
      ],
    ];
  }

  /// Yoldaki kaydı tanıtan ad: kartlı satışta POS, kartla tahsilatta kişi.
  ///
  /// `POS satışı` her satırda aynı olurdu; POS'un adı başlıkta durunca alt
  /// yazı yalnız beklenen günü taşır ve tek satıra sığar.
  String _transitTitle(FinancialActivity a) => a.isCardCollection
      ? (a.title.isEmpty ? a.kindLabel : a.title)
      : a.channelName ?? a.kindLabel;

  Widget _tile(FinancePageActivity row) {
    final a = row.activity;
    final categoryPage = !isAccount && !isTransit;
    final piece = row.part != null;
    final deferred =
        a.kind == ActivityKind.obligation ||
        a.kind == ActivityKind.counterpartyCharge ||
        a.kind == ActivityKind.debtOpening;
    final pieces = switch (row.part) {
      'interest' => 'Faiz',
      'commission' => 'Komisyon',
      'deduction' => 'Kesinti',
      'fee' || 'transfer-fee' => 'Masraf',
      _ => null,
    };
    var displayTitle = isTransit
        ? _transitTitle(a)
        : a.title.isEmpty
        ? a.kindLabel
        : a.title;
    if (categoryPage && a.kind == ActivityKind.posSale) {
      displayTitle =
          '${a.kindLabel}${a.channelName == null ? '' : ' · ${a.channelName}'}';
    }
    final subtitle = <String>[];
    if (categoryPage && piece) {
      displayTitle = a.kindLabel;
      final parent = MoneyText.format(row.parentAmount ?? a.amount, 'TRY');
      subtitle.add(switch (row.part) {
        'interest' => '$parent taksidin faizi',
        'fee' || 'transfer-fee' => '$parent transferin masrafı',
        'deduction' => '$parent yatışın kesintisi',
        'commission' =>
          '$parent ${a.isCardCollection ? 'tahsilatın' : 'satışın'} komisyonu',
        _ => '$parent · ${a.kindLabel}',
      });
    }
    // Kişinin adı başlıkta; hesap/kart alt satırdadır.
    if (categoryPage &&
        (piece &&
                (a.kind == ActivityKind.debtPayment ||
                    a.kind == ActivityKind.debtCollection) ||
            a.kind == ActivityKind.counterpartyCharge)) {
      final person = a.kind == ActivityKind.counterpartyCharge
          ? a.sourceName
          : a.title;
      if (person != null && !displayTitle.contains(person)) {
        displayTitle = '$displayTitle · $person';
      }
    }
    if (categoryPage &&
        piece &&
        a.kind == ActivityKind.transfer &&
        a.sourceName != null &&
        a.destinationName != null) {
      displayTitle = '${a.kindLabel} · ${a.sourceName} → ${a.destinationName}';
    } else if (categoryPage && piece && a.kind == ActivityKind.posSale) {
      // POS seçilmeden girilen satışta satırı hesabı tanıtır.
      final place = a.channelName ?? a.destinationName ?? a.sourceName;
      if (place != null) displayTitle = '${a.kindLabel} · $place';
    } else if (categoryPage && piece && a.kind == ActivityKind.posDeposit) {
      final account = a.destinationName ?? a.sourceName;
      if (account != null) displayTitle = '${a.kindLabel} · $account';
    }
    if (categoryPage && deferred && !piece) subtitle.add('Vadeli');
    if (isAccount) {
      if (a.kind == ActivityKind.transfer) {
        displayTitle = a.kindLabel;
        final other = row.amount.startsWith('-')
            ? a.destinationName
            : a.sourceName;
        if (other != null) {
          subtitle.add(other);
        }
      } else if (a.kind == ActivityKind.cardPayment) {
        displayTitle = a.kindLabel;
        if (a.destinationName != null) subtitle.add(a.destinationName!);
      } else if (a.kind == ActivityKind.debtPayment ||
          a.kind == ActivityKind.debtCollection ||
          a.kind == ActivityKind.counterpartySettlement ||
          a.kind == ActivityKind.obligationSettlement) {
        subtitle.add(a.kindLabel);
      } else if (a.categoryName != null && a.categoryName != displayTitle) {
        subtitle.add(a.categoryName!);
      }
    }
    if (isTransit) {
      if (a.isCardCollection) subtitle.add('Tahsilat');
      if (a.expectedTransferDate != null) {
        subtitle.add(
          '${DateText.dayShortMonth(a.expectedTransferDate!)} bekleniyor',
        );
      }
    } else if (categoryPage &&
        piece &&
        (a.kind == ActivityKind.posSale ||
            a.kind == ActivityKind.posDeposit ||
            a.kind == ActivityKind.transfer)) {
      // Komisyon, kesinti ve masraf satırında yer başlıktadır; alt yazı
      // yalnız bağlı kaydın tutarını söyler (kare B6).
    } else if (!isAccount || a.kind == ActivityKind.posDeposit) {
      // Başlıkta zaten yazan ad, harf büyüklüğü farklı da olsa tekrarlanmaz
      // (`ÖRNEK ELEKTRİK` başlığının altında `Örnek Elektrik`).
      String fold(String text) => text.toUpperCase().replaceAll('İ', 'I');
      for (final value in [a.sourceName, a.destinationName, a.channelName]) {
        if (value != null &&
            !fold(displayTitle).contains(fold(value)) &&
            value != controller.account?.name &&
            value != controller.category?.name &&
            !subtitle.contains(value)) {
          subtitle.add(value);
        }
      }
    }
    if (isAccount &&
        a.kind == ActivityKind.posDeposit &&
        row.firstSaleDate != null) {
      // Tek satışta günü, birkaç tahsilatta önce sayı sonra gün aralığı.
      // `tahsilat` denir: yatış kartla tahsilatı da kapatabilir.
      final last = row.lastSaleDate ?? row.firstSaleDate!;
      if ((a.settlementCount ?? 0) > 1) {
        subtitle.add('${a.settlementCount} tahsilat');
        subtitle.add(DateText.dayRangeShort(row.firstSaleDate!, last));
      } else {
        subtitle.add('${DateText.dayShortMonth(row.firstSaleDate!)} satışı');
      }
    }
    if (a.isWrittenByDayClose && !displayTitle.contains('Gün sonu')) {
      displayTitle = '$displayTitle · Gün sonu';
    }
    if (row.showScope && row.scope != null && categoryPage) {
      subtitle.add(row.scope!.label);
    }
    final state = isTransit
        ? null
        : switch (row.state) {
            'open' => 'Açık',
            'paid' || 'settled' =>
              '${controller.income ? 'Tahsil edildi' : 'Ödendi'}${row.settledOn == null ? '' : ' · ${DateText.dayShortMonth(row.settledOn!)}'}${row.settledSource == null ? '' : ' · ${row.settledSource}'}',
            // Kartlı satışın parası günüyle ayrı durum satırında okunur
            // (kullanıcı kararı, 14 Ekim 2026; teslim B8'deki alt yazı
            // `Hesaba geçti`yi dar ekranda iki satıra taşıyordu ve gün
            // yazmıyordu).
            'transit' =>
              'Yolda${a.expectedTransferDate == null ? '' : ' · ${DateText.dayShortMonth(a.expectedTransferDate!)} bekleniyor'}',
            'deposited' =>
              'Hesaba geçti${a.transferredOn == null ? '' : ' · ${DateText.dayShortMonth(a.transferredOn!)}'}',
            _ => null,
          };
    return ActivityTile(
      activity: a,
      showDate: false,
      onTap: () => _detail(row),
      compact: true,
      alignTop: categoryPage,
      mutedIcon: categoryPage && controller.category?.name == null,
      icon: categoryPage && controller.category?.name == null
          ? Icons.category_outlined
          // Gün sonunun yazdığı nakit satış kasanın ikonunu taşır.
          : categoryPage &&
                !piece &&
                a.isWrittenByDayClose &&
                a.kind == ActivityKind.accountTransaction
          ? Icons.payments_outlined
          : categoryPage && !piece && a.kind != ActivityKind.posSale
          ? AppFinanceIcons.forCategory(
              controller.category?.name ?? 'Kategorisi yok',
            )
          : null,
      title: displayTitle,
      subtitle: subtitle.join(' · '),
      subtitleIcon: isAccount && a.kind == ActivityKind.transfer
          ? row.amount.startsWith('-')
                ? Icons.arrow_forward
                : Icons.arrow_back
          : null,
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
      effect: isAccount
          ? a.effect
          : isTransit
          ? ActivityEffect.neutral
          : row.amount.startsWith('-')
          ? ActivityEffect.expense
          : ActivityEffect.income,
      signedByAmount: isAccount || isTransit,
      hideFee: isAccount || isTransit || piece,
      amountNote: isTransit && a.hasFee
          ? '${MoneyText.format(a.feeAmount!, 'TRY')} komisyon sonrası'
          : isAccount && row.partAmount != null && pieces != null
          ? '${MoneyText.format(row.partAmount!, 'TRY')} ${pieces.toLowerCase()} ${row.part == 'deduction' ? 'sonrası' : 'dahil'}'
          : null,
    );
  }
}
