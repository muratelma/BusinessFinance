import '../../../core/formatters/money_math.dart';
import 'dart:math' as math;

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/widgets/app_pick_field.dart';
import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_category_field.dart';
import '../../../core/widgets/app_no_results_view.dart';
import '../../../core/widgets/app_page_bar.dart';
import '../../categories/presentation/category_create_page.dart';
import '../../categories/presentation/recent_categories.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_month_chips.dart';
import '../../../core/widgets/app_responsive_grid.dart';
import '../../../core/widgets/app_row_action.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_unknown_amount.dart';
import '../data/planning_models.dart';
import '../data/planning_repository.dart';
import 'planning_controller.dart';
import 'recurring_plans_page.dart';
import 'realize_day_sheet.dart';
import '../../../core/widgets/app_account_field.dart';
import '../../../features/accounts/presentation/account_directory.dart';

class PlanningPage extends StatefulWidget {
  const PlanningPage({
    required this.repository,
    this.financialDataChanges,
    super.key,
  });

  final PlanningRepositoryContract repository;
  final FinancialDataChanges? financialDataChanges;

  @override
  State<PlanningPage> createState() => _PlanningPageState();
}

class _PlanningPageState extends State<PlanningPage> {
  late final PlanningController controller;

  @override
  void initState() {
    super.initState();
    controller = PlanningController(
      widget.repository,
      financialDataChanges: widget.financialDataChanges,
    )..addListener(_changed);
    controller.load();
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    controller.dispose();
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) => DefaultTabController(
    length: 3,
    child: Scaffold(
      appBar: AppPageBar(
        title: const Text('Planlama ve raporlar'),
        bottom: const TabBar(
          isScrollable: true,
          tabs: [
            Tab(text: 'Tekrarlayanlar'),
            Tab(text: 'Yaklaşanlar'),
            Tab(text: 'Raporlar'),
          ],
        ),
      ),
      body: _body(),
    ),
  );

  Widget _body() {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading && controller.snapshot == null) {
      return const AppLoadingView(message: 'Planlama verileri yükleniyor');
    }
    if (controller.errorMessage != null && controller.snapshot == null) {
      return AppErrorView(
        message: controller.errorMessage!,
        onRetry: controller.load,
      );
    }

    final snapshot = controller.snapshot!;
    return Column(
      children: [
        if (controller.isLoading) const LinearProgressIndicator(),
        if (controller.errorMessage != null ||
            controller.successMessage != null)
          MaterialBanner(
            leading: controller.isStale
                ? const Icon(Icons.cloud_off_outlined)
                : null,
            content: Text(
              controller.isStale
                  ? 'Son güncel veriler gösteriliyor. ${controller.errorMessage}'
                  : controller.errorMessage ?? controller.successMessage!,
            ),
            actions: [
              if (controller.isStale)
                TextButton(
                  onPressed: controller.load,
                  child: const Text('Yenile'),
                ),
              TextButton(
                onPressed: controller.clearMessage,
                child: const Text('Kapat'),
              ),
            ],
          ),
        Expanded(
          child: TabBarView(
            children: [
              _recurringTab(snapshot),
              _upcomingTab(snapshot),
              _reportsTab(snapshot.report),
            ],
          ),
        ),
      ],
    );
  }

  // Tekrarlayanlar kendi sayfasıyla aynı listeyi çizer (22 Kasım 2026);
  // iki ayrı görünüm ayrışmasın.
  Widget _recurringTab(PlanningSnapshot snapshot) => RefreshIndicator(
    onRefresh: controller.load,
    child: RecurringPlanList(snapshot: snapshot, controller: controller),
  );

  /// Seçili vade ufkundan bir sonraki; en genişte yoktur.
  int? get _widerDays => switch (controller.daysAhead) {
    7 => 30,
    30 => 90,
    _ => null,
  };

  Widget _upcomingTab(PlanningSnapshot snapshot) => RefreshIndicator(
    onRefresh: controller.load,
    child: ListView(
      padding: AppPageInsets.underTabs,
      children: [
        Text('Vade ufku', style: Theme.of(context).textTheme.labelLarge),
        const SizedBox(height: AppGaps.sectionTitle),
        Wrap(
          spacing: AppSpacing.small,
          children: [
            for (final days in [7, 30, 90])
              ChoiceChip(
                label: Text('$days gün'),
                selected: controller.daysAhead == days,
                onSelected: controller.isLoading
                    ? null
                    : (_) => controller.changeDaysAhead(days),
              ),
          ],
        ),
        const SizedBox(height: AppGaps.block),
        // `Bugüne kadar üret` düğmesi kaldırıldı.
        //
        // Tek işi, gerçekleştirilebilir bir satır üretebilmekti; artık
        // gerçekleştirmenin kendisi eksik kaydı sunucuda üretiyor. Geriye
        // kullanıcının hiçbir zaman vermek zorunda olmadığı bir karar kalıyordu:
        // üretmek sistemin defter işi ve düğmenin bastıktan sonra ekranda
        // değiştirdiği tek şey, projeksiyon satırlarının aynı bilgiyle bekleyen
        // kayıt olarak yeniden çizilmesiydi.
        OutlinedButton.icon(
          onPressed: controller.isLoading ? null : _pickAsOfDate,
          icon: const Icon(Icons.today),
          label: Text('Referans tarihi: ${_formatDate(controller.asOfDate)}'),
        ),
        const SizedBox(height: AppGaps.block),
        if (snapshot.upcomingPayments.isEmpty)
          AppNoResultsView(
            reason: AppNoResultsReason.period,
            title: '${controller.daysAhead} gün içinde yaklaşan ödeme yok',
            message: _widerDays == null
                ? null
                : 'Daha ilerisi için dönemi genişletin.',
            actionLabel: _widerDays == null ? null : '$_widerDays güne bak',
            onAction: _widerDays == null || controller.isLoading
                ? null
                : () => controller.changeDaysAhead(_widerDays!),
          )
        else
          for (final timing in ['overdue', 'today', 'upcoming']) ...[
            if (snapshot.upcomingPayments.any((item) => item.timing == timing))
              Padding(
                padding: const EdgeInsets.only(
                  top: AppSpacing.medium,
                  bottom: AppSpacing.small,
                ),
                child: Text(
                  _timingLabel(timing),
                  style: Theme.of(context).textTheme.titleMedium,
                ),
              ),
            for (final item in snapshot.upcomingPayments.where(
              (item) => item.timing == timing,
            ))
              Padding(
                padding: const EdgeInsets.only(bottom: AppGaps.card),
                child: AppCard(
                  padding: EdgeInsets.zero,
                  child: AppListRow(
                    icon: _sourceIcon(item.sourceType),
                    title: item.title,
                    subtitle:
                        '${DateText.dayMonth(item.dueDate)} • '
                        '${_sourceLabel(item.sourceType)}'
                        '${item.description == null ? '' : '\n${item.description}'}',
                    badge: _upcomingBadge(snapshot, item, timing),
                    trailing: item.amount == null
                        ? const AppUnknownAmount()
                        : AppMoneyText(
                            amount: item.amount!,
                            currency: item.currency,
                          ),
                  ),
                ),
              ),
          ],
      ],
    ),
  );

  /// Yaklaşan bir yükümlülüğün durumu ve — varsa — eylemi.
  ///
  /// Onay bekleyen tekrarlayan kayıtlar bu listede **zaten** vardı; eksik olan
  /// tek şey onları buradan onaylayabilmekti. Ayrı bir "Planlanan kayıtlar"
  /// bölümü aynı satırları ikinci kez, başka bir sekmede gösteriyordu.
  ///
  /// Onaylanabilirliği `sourceType`'tan tahmin etmiyoruz: sunucunun kendi
  /// `canRealize` cevabı kaynak kayıtta duruyor ve kimlikten eşleşiyor. Pasif
  /// planın kaydı gibi durumları ekranın yeniden karar vermesi gerekmiyor.
  Widget? _upcomingBadge(
    PlanningSnapshot snapshot,
    UpcomingPaymentItem item,
    String timing,
  ) {
    // Vergi kalemi burada gerçekleştirilmez; vadesi gelmemişi de ödenebilir.
    if (item.isTax) {
      return Wrap(
        spacing: AppSpacing.small,
        runSpacing: AppSpacing.small,
        crossAxisAlignment: WrapCrossAlignment.center,
        children: [
          if (timing == 'overdue')
            const AppStatusChip(
              label: 'Gecikmiş',
              icon: Icons.warning_amber_outlined,
              tone: AppStatusTone.expense,
            ),
          AppRowAction(
            label: 'Ödedim →',
            onPressed: () => context.push(taxesLocation),
          ),
        ],
      );
    }
    final generated = snapshot.occurrences.any(
      (occurrence) => occurrence.id == item.sourceId && occurrence.canRealize,
    );
    // Vadesi gelmemiş kalem de gerçekleştirilir: kayıt vadeye değil ödendiği
    // güne yazılır (kullanıcı kararı, 12 Ekim 2026), yani kirayı erken ödemek
    // parayı çıkmadığı bir aya koymaz. Tutarı olmayan kalem burada
    // gerçekleşmez.
    final approvable =
        item.sourceType == 'recurring-occurrence' && item.amount != null;
    if (timing != 'overdue' && !approvable) return null;

    return Wrap(
      alignment: WrapAlignment.spaceBetween,
      spacing: AppSpacing.small,
      runSpacing: AppSpacing.small,
      crossAxisAlignment: WrapCrossAlignment.center,
      children: [
        // Gecikme kartın tümünü kırmızıya boyamak yerine rozetle söyleniyor:
        // dolu bir hata zemini satırdaki tutarın kendi finansal rengini de
        // bastırıyordu.
        if (timing == 'overdue')
          const AppStatusChip(
            label: 'Gecikmiş',
            icon: Icons.warning_amber_outlined,
            tone: AppStatusTone.expense,
          )
        else if (timing != 'upcoming')
          const AppStatusChip(
            label: 'Onay bekliyor',
            icon: Icons.schedule,
            tone: AppStatusTone.planned,
          ),
        // Kimlik occurrence varsa onun, yoksa planındır; ekran ikisini ayırt
        // edip doğru ucu seçiyor. Kullanıcı için ikisi de tek bir eylem.
        if (approvable)
          AppRowAction(
            label: 'Gerçekleştir',
            onPressed: controller.isSubmitting
                ? null
                : () => _confirmRealize(snapshot, item, generated),
          ),
      ],
    );
  }

  /// Para hareket etmeden önce açık onay ister, **hangi hesaptan** çıkacağını
  /// söyler ve hangi gün ödendiğini sorar.
  ///
  /// Kaynak planda duruyor — tekrarlayan planın kaynağı kuruluşta seçilir —
  /// ve panelde yalnız gösterilir. Gün kullanıcınındır: kayıt vadeye değil
  /// ödendiği güne yazılır.
  Future<void> _confirmRealize(
    PlanningSnapshot snapshot,
    UpcomingPaymentItem item,
    bool generated,
  ) async {
    final planId = generated
        ? snapshot.occurrences
              .where((occurrence) => occurrence.id == item.sourceId)
              .map((occurrence) => occurrence.recurringTransactionId)
              .firstOrNull
        : item.sourceId;
    final source = planId == null ? null : _planSourceName(snapshot, planId);

    final plan = planId == null
        ? null
        : snapshot.recurringTransactions
              .where((candidate) => candidate.id == planId)
              .firstOrNull;
    final paidOn = await showRealizeDaySheet(
      context,
      title: item.title,
      amount: item.amount!,
      currency: item.currency,
      dueDate: item.dueDate,
      sourceName: source,
      isIncome: plan?.kind == 'income',
    );
    if (paidOn == null) return;

    Future<bool> realize({bool allowEarlierPending = false}) => generated
        ? controller.realizeOccurrence(
            item.sourceId,
            paidOn: paidOn,
            allowEarlierPending: allowEarlierPending,
          )
        : controller.realizeDue(
            item.sourceId,
            item.dueDate,
            paidOn: paidOn,
            allowEarlierPending: allowEarlierPending,
          );

    // Sıra atlamayı sunucu söyler: planın daha önceki bir kalemi bekliyorsa
    // istek onay isteğiyle döner; onaylanırsa aynı gün ile yinelenir.
    if (await realize() || !controller.earlierItemPending || !mounted) return;
    final confirmed = await confirmSkippingEarlierItem(
      context,
      title: item.title,
      dueDate: item.dueDate,
      paidOn: paidOn,
      pendingDate: controller.earlierPendingDate,
      pendingCount: controller.earlierPendingCount,
    );
    if (confirmed) await realize(allowEarlierPending: true);
  }

  /// Planın kaynağının adı: hesap ya da kart.
  String? _planSourceName(PlanningSnapshot snapshot, String planId) {
    final plan = snapshot.recurringTransactions
        .where((item) => item.id == planId)
        .firstOrNull;
    if (plan == null) return null;
    final accountId = plan.accountId;
    if (accountId != null) {
      return snapshot.accounts
          .where((choice) => choice.id == accountId)
          .map((choice) => choice.name)
          .firstOrNull;
    }
    final cardId = plan.creditCardId;
    if (cardId == null) return null;
    return snapshot.creditCards
        .where((choice) => choice.id == cardId)
        .map((choice) => choice.name)
        .firstOrNull;
  }

  Widget _reportsTab(AdvancedReport report) => RefreshIndicator(
    onRefresh: controller.load,
    child: ListView(
      padding: AppPageInsets.underTabs,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            IconButton(
              tooltip: 'Önceki ay',
              onPressed: controller.isLoading
                  ? null
                  : () => controller.moveReportMonth(-1),
              icon: const Icon(Icons.chevron_left),
            ),
            // Aynı ay seçici deseni: başlık iki ok butonu arasında esnek
            // olmazsa büyük yazı ölçeğinde satırı taşırır.
            Flexible(
              child: Text(
                '${controller.reportMonth.toString().padLeft(2, '0')}.${controller.reportYear}',
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.titleLarge,
              ),
            ),
            IconButton(
              tooltip: 'Sonraki ay',
              onPressed: controller.isLoading
                  ? null
                  : () => controller.moveReportMonth(1),
              icon: const Icon(Icons.chevron_right),
            ),
          ],
        ),
        const SizedBox(height: AppSpacing.medium),
        _SummaryGrid(report: report),
        const SizedBox(height: AppGaps.section),
        const AppSectionHeader(title: 'Nakit akışı eğilimi'),
        AppCard(
          child: CashFlowChart(
            points: report.cashFlowTrend,
            currency: report.currency,
          ),
        ),
        const SizedBox(height: AppGaps.section),
        // Grafiğin metin alternatifi grafiğin yerine geçmez, onu tamamlar:
        // aynı sayılar okunabilir biçimde de durur. Kendi kartında toplanır,
        // yoksa on iki satır ekranı düz bir duvara çevirir.
        const AppSectionHeader(
          key: Key('cash-flow-text-alternative'),
          title: 'Aylık döküm',
        ),
        if (report.cashFlowTrend.isEmpty)
          const _EmptyNote('Bu dönem için nakit akışı kaydı yok.')
        else
          _RowGroup(
            children: [
              for (final point in report.cashFlowTrend)
                AppListRow(
                  title:
                      '${point.month.toString().padLeft(2, '0')}.${point.year}',
                  subtitle:
                      'Gelir ${MoneyText.format(point.income, report.currency)}'
                      ' • gider '
                      '${MoneyText.format(point.expense, report.currency)}',
                  trailing: AppMoneyText(
                    amount: point.net,
                    currency: report.currency,
                    effect: _netEffect(point.net),
                    style: Theme.of(context).textTheme.titleSmall,
                  ),
                ),
            ],
          ),
        const SizedBox(height: AppGaps.section),
        const AppSectionHeader(title: 'Bütçe sapmaları'),
        if (report.budgetVariances.isEmpty)
          const _EmptyNote('Bu dönem için bütçe sapması yok.')
        else
          _RowGroup(
            children: [
              for (final variance in report.budgetVariances)
                AppListRow(
                  icon: variance.isExceeded
                      ? Icons.warning_amber_rounded
                      : Icons.check_circle_outline,
                  iconColor: variance.isExceeded
                      ? AppFinanceColors.of(context).onExpenseContainer
                      : AppFinanceColors.of(context).onIncomeContainer,
                  iconBackground: variance.isExceeded
                      ? AppFinanceColors.of(context).expenseContainer
                      : AppFinanceColors.of(context).incomeContainer,
                  title: variance.categoryName,
                  subtitle:
                      'Limit ${MoneyText.format(variance.limit, report.currency)}'
                      ' • harcanan '
                      '${MoneyText.format(variance.spent, report.currency)}',
                  trailing: AppMoneyText(
                    amount: variance.remaining,
                    currency: report.currency,
                    effect: variance.isExceeded
                        ? AppMoneyEffect.expense
                        : AppMoneyEffect.income,
                    style: Theme.of(context).textTheme.titleSmall,
                  ),
                ),
            ],
          ),
        const SizedBox(height: AppGaps.section),
        _DistributionSection(
          title: 'Hesap dağılımı',
          items: report.accountDistribution,
          currency: report.currency,
        ),
        _DistributionSection(
          title: 'Kart borçları',
          items: report.cardDistribution,
          currency: report.currency,
          effect: AppMoneyEffect.expense,
        ),
      ],
    ),
  );

  static AppMoneyEffect _netEffect(String net) {
    final value = double.tryParse(net) ?? 0;
    if (value > 0) return AppMoneyEffect.income;
    if (value < 0) return AppMoneyEffect.expense;
    return AppMoneyEffect.neutral;
  }

  Future<void> _pickAsOfDate() async {
    final selected = await showDatePicker(
      context: context,
      initialDate: controller.asOfDate,
      firstDate: DateTime(2000),
      lastDate: DateTime(2200),
    );
    if (selected != null) await controller.changeAsOfDate(selected);
  }
}

/// Yeni tekrarlayan planın formu; Tekrarlayanlar ve Planlama ortak.
/// [initialName] sonuçsuz aramadan gelir ve yalnız adı doldurur.
Future<void> showRecurringPlanForm(
  BuildContext context, {
  required PlanningSnapshot snapshot,
  required PlanningController controller,
  String? initialName,
}) => AppFormSheet.show<bool>(
  context: context,
  builder: (context) => _RecurringForm(
    snapshot: snapshot,
    onSubmit: controller.createRecurring,
    initialName: initialName,
  ),
);

class _SummaryGrid extends StatelessWidget {
  const _SummaryGrid({required this.report});
  final AdvancedReport report;

  @override
  Widget build(BuildContext context) => AppResponsiveGrid(
    children: [
      _SummaryCard(
        title: 'Net varlık',
        amount: report.netWorth,
        currency: report.currency,
        detail:
            'Likit ${MoneyText.format(report.liquidAssets, report.currency)} • '
            '${MoneyText.isNegative(report.creditCardDebt) ? 'Kart alacağı' : 'Kart borcu'} '
            '${MoneyText.format(MoneyText.unsigned(report.creditCardDebt), report.currency)}',
      ),
      _SummaryCard(
        title: 'Dönem neti',
        amount: report.currentPeriod.net,
        currency: report.currency,
        detail:
            'Önceki dönem ${MoneyText.format(report.previousPeriod.net, report.currency)}',
      ),
      _SummaryCard(
        title: 'Gelecek ödeme yükü',
        amount: report.futureLoad,
        currency: report.currency,
        detail: 'Seçilen vade ufku',
      ),
    ],
  );
}

class _SummaryCard extends StatelessWidget {
  const _SummaryCard({
    required this.title,
    required this.amount,
    required this.currency,
    required this.detail,
  });
  final String title;
  final String amount;
  final String currency;
  final String detail;

  @override
  Widget build(BuildContext context) => AppCard(
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(title, style: Theme.of(context).textTheme.labelLarge),
        const SizedBox(height: AppSpacing.small),
        Text(
          MoneyText.format(amount, currency),
          style: Theme.of(context).textTheme.headlineSmall,
        ),
        Text(detail),
      ],
    ),
  );
}

class CashFlowChart extends StatelessWidget {
  const CashFlowChart({
    required this.points,
    required this.currency,
    super.key,
  });
  final List<CashFlowPoint> points;
  final String currency;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    final description = points.isEmpty
        ? 'Nakit akışı grafiğinde veri yok.'
        : points
              .map(
                (point) =>
                    '${point.month}.${point.year} net ${MoneyText.format(point.net, currency)}',
              )
              .join(', ');
    return Semantics(
      key: const Key('cash-flow-chart'),
      container: true,
      image: true,
      label: 'Nakit akışı grafiği. $description',
      child: ExcludeSemantics(
        child: SizedBox(
          height: 260,
          width: double.infinity,
          child: Column(
            children: [
              Expanded(
                child: CustomPaint(
                  painter: _CashFlowPainter(
                    points: points,
                    // Artı ay gelir, eksi ay giderdir; renkleri de finans
                    // token'larından gelir. Önceden artı çubuklar akromatik
                    // marka renginden siyah, eksi çubuklar Material'in
                    // `error` kırmızısından geliyordu: ikisi de uygulamanın
                    // gelir/gider paletinin dışındaydı.
                    positiveColor: colors.incomeFill,
                    negativeColor: colors.expenseFill,
                    axisColor: Theme.of(context).colorScheme.outlineVariant,
                  ),
                  child: const SizedBox.expand(),
                ),
              ),
              const SizedBox(height: AppSpacing.small),
              Row(
                key: const Key('cash-flow-chart-labels'),
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  for (final point in points)
                    Expanded(
                      child: Column(
                        children: [
                          FittedBox(
                            fit: BoxFit.scaleDown,
                            child: Text(
                              MoneyText.format(point.net, currency),
                              style: Theme.of(context).textTheme.labelSmall,
                            ),
                          ),
                          Text(
                            '${point.month.toString().padLeft(2, '0')}.${point.year}',
                            style: Theme.of(context).textTheme.labelSmall,
                          ),
                        ],
                      ),
                    ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _CashFlowPainter extends CustomPainter {
  const _CashFlowPainter({
    required this.points,
    required this.positiveColor,
    required this.negativeColor,
    required this.axisColor,
  });
  final List<CashFlowPoint> points;
  final Color positiveColor;
  final Color negativeColor;
  final Color axisColor;

  @override
  void paint(Canvas canvas, Size size) {
    final axis = Paint()
      ..color = axisColor
      ..strokeWidth = 1;
    final baseline = size.height / 2;
    canvas.drawLine(Offset(0, baseline), Offset(size.width, baseline), axis);
    if (points.isEmpty) return;
    final maximum = points
        .map((point) => point.netValue.abs())
        .fold<double>(1, math.max);
    final slot = size.width / points.length;
    final barWidth = math.min(36.0, slot * .55);
    for (var index = 0; index < points.length; index++) {
      final value = points[index].netValue;
      final centerX = index * slot + slot / 2;
      if (value == 0) {
        canvas.drawCircle(
          Offset(centerX, baseline),
          3,
          Paint()..color = axisColor,
        );
        continue;
      }
      final height = value.abs() / maximum * (baseline - 16);
      final left = index * slot + (slot - barWidth) / 2;
      final rect = value >= 0
          ? Rect.fromLTWH(left, baseline - height, barWidth, height)
          : Rect.fromLTWH(left, baseline, barWidth, height);
      canvas.drawRRect(
        RRect.fromRectAndRadius(rect, const Radius.circular(6)),
        Paint()..color = value >= 0 ? positiveColor : negativeColor,
      );
    }
  }

  @override
  bool shouldRepaint(covariant _CashFlowPainter oldDelegate) =>
      oldDelegate.points != points ||
      oldDelegate.positiveColor != positiveColor ||
      oldDelegate.negativeColor != negativeColor ||
      oldDelegate.axisColor != axisColor;
}

class _DistributionSection extends StatelessWidget {
  const _DistributionSection({
    required this.title,
    required this.items,
    required this.currency,
    this.effect,
  });
  final String title;
  final List<DistributionItem> items;
  final String currency;

  /// Satırdaki tutarın finansal rolü.
  ///
  /// **Hesap bakiyesinde null**, yani metnin varsayılan mürekkep rengi: para
  /// orada duruyor, ne kazanıldı ne harcandı ve rol tonu taşımıyor. `neutral`
  /// tonu verilmişti, ama o ton **maviyi** getiriyor (ADR 0008) — bakiye
  /// listesi bir anda renkli bir role sahipmiş gibi okunuyordu.
  ///
  /// Kart borcu ise gerçekten bir rol taşır: **yükümlülüktür**. Bakiyeyle aynı
  /// renkte yazılması "elimde 12.000 var" ile "12.000 borçluyum" farkını tek
  /// bakışta göstermiyordu.
  final AppMoneyEffect? effect;

  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      AppSectionHeader(title: title),
      if (items.isEmpty)
        const _EmptyNote('Gösterilecek dağılım yok.')
      else
        _RowGroup(
          children: [
            for (final item in items)
              AppListRow(
                title: item.name,
                trailing: AppMoneyText(
                  amount: item.primaryAmount,
                  currency: currency,
                  effect: effect,
                  style: Theme.of(context).textTheme.titleSmall,
                ),
              ),
          ],
        ),
      const SizedBox(height: AppGaps.section),
    ],
  );
}

/// Satırları tek kartta toplayıp ince ayırıcılarla ayırır.
///
/// Her satırı ayrı karta koymak listeyi parçalıyor, hiç kart kullanmamak ise
/// bölümleri birbirine yapıştırıyordu: raporlar sekmesi baştan sona tek bir
/// düz metin duvarı gibi okunuyordu.
class _RowGroup extends StatelessWidget {
  const _RowGroup({required this.children});

  final List<Widget> children;

  @override
  Widget build(BuildContext context) => AppCard(
    padding: EdgeInsets.zero,
    child: Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        for (var i = 0; i < children.length; i++) ...[
          if (i > 0) const Divider(height: 1, indent: AppSpacing.medium),
          children[i],
        ],
      ],
    ),
  );
}

/// Bölümün boş olduğunu söyleyen tek satır; en soluk mürekkep kademesinde.
class _EmptyNote extends StatelessWidget {
  const _EmptyNote(this.message);

  final String message;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
    child: Text(message, style: Theme.of(context).textTheme.bodySmall),
  );
}

class _RecurringForm extends StatefulWidget {
  const _RecurringForm({
    required this.snapshot,
    required this.onSubmit,
    this.initialName,
  });
  final PlanningSnapshot snapshot;
  final String? initialName;
  final Future<bool> Function(Map<String, Object?>) onSubmit;

  @override
  State<_RecurringForm> createState() => _RecurringFormState();
}

class _RecurringFormState extends State<_RecurringForm> {
  final formKey = GlobalKey<FormState>();
  final amount = TextEditingController();
  final occurrenceLimit = TextEditingController();
  late final description = TextEditingController(text: widget.initialName);
  String? accountId;
  String? creditCardId;
  String? categoryId;
  late List<PlanningChoice> accounts = widget.snapshot.accounts;
  late List<PlanningChoice> categories = widget.snapshot.categories;

  /// İki kapsama açık kategorinin menüde seçilen kapsamı. Plan kapsamını
  /// kurulurken alır ve ürettiği her kayda taşır (ADR 0020 Y4).
  TransactionScope? categoryScope;
  String kind = 'expense';
  String frequency = 'monthly';
  String monthEndBehavior = 'clamp-to-last-day';
  DateTime startDate = DateTime.now();

  /// "Seçilen aylarda" ritminin ayları (1–12); başlangıç ayıyla açılır.
  late Set<int> months = {DateTime.now().month};
  bool monthsInvalid = false;

  /// `yyyy-MM-dd` veya null. Bitiş tarihi ile tekrar sınırı **birlikte**
  /// verilebilir; sunucu önce dolanı uygular.
  String? endDate;

  @override
  void dispose() {
    amount.dispose();
    occurrenceLimit.dispose();
    description.dispose();
    super.dispose();
  }

  @override
  // Teslimle kurulan panel: 16 dp yan boşluk, metin harf aralıksız.
  Widget build(BuildContext context) => Form(
    key: formKey,
    child: AppFormSheet<bool>(
      title: 'Tekrarlayan plan ekle',
      description:
          'Plan tek başına para üretmez; kayıt ancak gerçekleştirildiğinde '
          'bakiyeye ve raporlara girer.',
      submitLabel: 'Kaydet',
      onSubmit: _submit,
      children: [
        // Income can only land in an account: a card cannot receive money
        // and card refunds are not modelled. An expense or bill may be
        // charged to a card, which is how a subscription is really paid.
        if (kind == 'income')
          AppAccountField(
            label: 'Hesap',
            value: accountId,
            options: [
              for (final account in accounts)
                AccountChoices.of(context).account(
                  account.id,
                  account.name,
                  type: account.type,
                  scope: account.defaultScope,
                ),
            ],
            onChanged: (value) => setState(() {
              accountId = value;
              creditCardId = null;
            }),
            onAdd: _addAccount,
            validator: (value) => value == null ? 'Hesap seçin.' : null,
          )
        else
          AppAccountField(
            label: 'Kaynak',
            value: _sourceValue,
            options: [
              for (final account in accounts)
                AccountChoices.of(context)
                    .account(
                      account.id,
                      account.name,
                      type: account.type,
                      scope: account.defaultScope,
                    )
                    .withId('account:${account.id}'),
              for (final card in widget.snapshot.creditCards)
                AccountChoices.of(context)
                    .card(card.id, card.name, scope: card.defaultScope)
                    .withId('card:${card.id}'),
            ],
            onChanged: (value) => setState(() {
              if (value == null) return;
              final isCard = value.startsWith('card:');
              creditCardId = isCard ? value.substring(5) : null;
              accountId = isCard ? null : value.substring(8);
            }),
            onAdd: _addAccount,
            validator: (value) => value == null ? 'Kaynak seçin.' : null,
          ),
        const SizedBox(height: AppGaps.field),
        AppCategoryField(
          options: _categoryOptions,
          value: _categoryValue,
          recent: RecentCategories.of(context, _categoryType),
          showScope: context.watch<ScopeController?>()?.isVisible ?? false,
          onChanged: (value) => setState(() {
            categoryId = value?.categoryId;
            categoryScope = value?.scope;
          }),
          onAdd: _addCategory,
          validator: (value) => value == null ? 'Kategori seçin.' : null,
        ),
        const SizedBox(height: AppGaps.field),
        TextFormField(
          controller: amount,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          inputFormatters: const [TurkishAmountInputFormatter()],
          decoration: const InputDecoration(labelText: 'Tutar'),
          validator: _amountError,
        ),
        const SizedBox(height: AppGaps.field),
        AppPickField<String>(
          label: 'Tür',
          value: kind,
          options: [
            AppPickOption(
              value: 'income',
              label: 'Gelir',
              icon: Icons.south_west,
              iconColor: AppFinanceColors.of(context).income,
            ),
            AppPickOption(
              value: 'expense',
              label: 'Gider',
              icon: Icons.north_east,
              iconColor: AppFinanceColors.of(context).expense,
            ),
            const AppPickOption(
              value: 'bill-payment',
              label: 'Fatura / abonelik',
              icon: Icons.receipt_long_outlined,
            ),
          ],
          onChanged: (value) => setState(() {
            kind = value;
            if (!_availableCategories.any(
              (category) => category.id == categoryId,
            )) {
              categoryId = null;
              categoryScope = null;
            }
          }),
        ),
        const SizedBox(height: AppGaps.field),
        AppPickField<String>(
          label: 'Sıklık',
          value: frequency,
          options: const [
            // Emlak Mayıs ve Kasım, gelir vergisi Mart ve Temmuz gibi
            // ritimler "6 ayda bir"e sığmıyor (ADR 0018 T2).
            AppPickOption(
              value: 'selected-months',
              label: 'Seçilen aylarda',
              icon: Icons.calendar_month_outlined,
            ),
            AppPickOption(
              value: 'daily',
              label: 'Günlük',
              icon: Icons.today_outlined,
            ),
            AppPickOption(
              value: 'weekly',
              label: 'Haftalık',
              icon: Icons.date_range_outlined,
            ),
            AppPickOption(
              value: 'monthly',
              label: 'Aylık',
              icon: Icons.calendar_view_month_outlined,
            ),
            // Geçici verginin ritmi; aylığın üç adımlık hâli.
            AppPickOption(
              value: 'quarterly',
              label: 'Üç ayda bir',
              icon: Icons.event_repeat_outlined,
            ),
            AppPickOption(
              value: 'yearly',
              label: 'Yıllık',
              icon: Icons.event_outlined,
            ),
          ],
          onChanged: (value) => setState(() => frequency = value),
        ),
        if (frequency == 'selected-months') ...[
          const SizedBox(height: AppSpacing.small),
          AppMonthChips(
            selected: months,
            onChanged: (value) => setState(() {
              months = value;
              monthsInvalid = false;
            }),
          ),
          if (monthsInvalid)
            Padding(
              padding: const EdgeInsets.only(top: AppSpacing.small),
              child: Text(
                months.isEmpty
                    ? 'En az bir ay seçin.'
                    : 'Başlangıç seçilen aylardan birinde olmalı.',
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: Theme.of(context).colorScheme.error,
                ),
              ),
            ),
        ],
        const SizedBox(height: AppGaps.field),
        AppPickField<String>(
          label: 'Ay sonu davranışı',
          value: monthEndBehavior,
          options: const [
            AppPickOption(
              value: 'clamp-to-last-day',
              label: 'Ayın son gününe taşı',
              icon: Icons.last_page,
            ),
            AppPickOption(
              value: 'skip-invalid-period',
              label: 'Geçersiz ayı atla',
              icon: Icons.skip_next_outlined,
            ),
          ],
          onChanged: (value) => setState(() => monthEndBehavior = value),
        ),
        const SizedBox(height: AppGaps.field),
        AppDateField(
          label: 'Başlangıç',
          value: _formatDate(startDate),
          onChanged: (value) => setState(() {
            startDate = AppDateField.parse(value)!;
            final end = AppDateField.parse(endDate);
            if (end != null && end.isBefore(startDate)) endDate = null;
          }),
        ),
        const SizedBox(height: AppGaps.field),
        AppDateField(
          label: 'Bitiş tarihi (isteğe bağlı)',
          value: endDate,
          firstDate: startDate,
          helperText: 'Boş bırakırsanız plan süresiz tekrar eder.',
          onChanged: (value) => setState(() => endDate = value),
        ),
        if (endDate != null)
          Align(
            alignment: Alignment.centerRight,
            child: TextButton.icon(
              onPressed: () => setState(() => endDate = null),
              icon: const Icon(Icons.clear),
              label: const Text('Bitiş tarihini kaldır'),
            ),
          ),
        const SizedBox(height: AppGaps.field),
        TextFormField(
          controller: occurrenceLimit,
          keyboardType: TextInputType.number,
          decoration: const InputDecoration(
            labelText: 'Toplam tekrar sınırı (isteğe bağlı)',
            helperText:
                'Plan bu sayıda kayıt ürettikten sonra tamamlanır. '
                'Bitiş tarihiyle birlikte verilirse önce dolan geçerlidir.',
          ),
          validator: (value) {
            final text = value?.trim() ?? '';
            if (text.isEmpty) return null;
            final parsed = int.tryParse(text);
            return parsed == null || parsed <= 0
                ? 'Sıfırdan büyük tam sayı girin.'
                : null;
          },
        ),
        const SizedBox(height: AppGaps.field),
        TextFormField(
          controller: description,
          hintLocales: const [Locale('tr', 'TR')],
          maxLength: 200,
          decoration: const InputDecoration(
            labelText: 'Açıklama (isteğe bağlı)',
          ),
        ),
      ],
    ),
  );

  /// `Hesap ekle`den dönülünce yeni hesap listede ve seçili gelir.
  Future<void> _addAccount() async {
    final created = await addAccountFromForm(context);
    if (created == null || !mounted) return;
    setState(() {
      accounts = [
        ...accounts,
        PlanningChoice(
          id: created.id,
          name: created.name,
          type: created.type,
          defaultScope: created.defaultScope,
        ),
      ];
      accountId = created.id;
      creditCardId = null;
    });
  }

  String? get _sourceValue => creditCardId != null
      ? 'card:$creditCardId'
      : (accountId == null ? null : 'account:$accountId');

  String get _categoryType => kind == 'income' ? 'income' : 'expense';

  Iterable<PlanningChoice> get _availableCategories => categories.where(
    (category) => category.type == null || category.type == _categoryType,
  );

  List<AppCategoryOption> get _categoryOptions => [
    for (final category in _availableCategories)
      AppCategoryOption(
        id: category.id,
        name: category.name,
        scope: category.defaultScope,
      ),
  ];

  AppCategoryValue? get _categoryValue =>
      categoryId == null ? null : AppCategoryValue(categoryId!, categoryScope);

  /// `+ "…" ekle`den dönülünce yeni kategori listede ve seçili gelir.
  Future<void> _addCategory(String name) async {
    final created = await addCategoryFromForm(
      context,
      type: _categoryType,
      name: name,
    );
    if (created == null || !mounted) return;
    final showScope = context.read<ScopeController?>()?.isVisible ?? false;
    setState(() {
      categories = [
        ...categories,
        PlanningChoice(
          id: created.id,
          name: created.name,
          type: created.type,
          defaultScope: created.defaultScope,
        ),
      ];
      categoryId = created.id;
      categoryScope = showScope ? created.defaultScope : null;
    });
  }

  /// Panel yalnız başarıda kapanır. Başarısızlıkta `null` döner ve alanlar
  /// ekranda kalır; hata mesajını controller banner'da gösterir.
  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    // Taraf görünmeyen kullanıcıda hiçbir istekte `scope` gitmez.
    final showScope = context.read<ScopeController?>()?.isVisible ?? false;
    // İki kapsama açık kategoride kapsamı alan kendisi ister; hesabın ya da
    // kartın kapsamı ön değer olmaz (kullanıcı kararı D5).
    final scope = AppCategoryField.scopeOf(_categoryOptions, _categoryValue);
    final selectedMonths = frequency == 'selected-months';
    if (selectedMonths &&
        (months.isEmpty || !months.contains(startDate.month))) {
      setState(() => monthsInvalid = true);
      return null;
    }
    final normalizedAmount = double.parse(
      MoneyText.normalizeInput(amount.text) ??
          amount.text.trim().replaceAll(',', '.'),
    ).toStringAsFixed(4);
    final success = await widget.onSubmit({
      // Sent explicitly so the server never has to infer which source this is.
      'sourceType': creditCardId == null ? 'account' : 'credit-card',
      'accountId': accountId,
      'creditCardId': creditCardId,
      'categoryId': categoryId,
      if (showScope) 'scope': scope?.apiValue,
      'amount': normalizedAmount,
      'currency': 'TRY',
      'kind': kind,
      'frequency': frequency,
      'startDate': _formatDate(startDate),
      'endDate': endDate,
      'occurrenceLimit': occurrenceLimit.text.trim().isEmpty
          ? null
          : int.parse(occurrenceLimit.text.trim()),
      'monthEndBehavior': monthEndBehavior,
      if (selectedMonths) 'months': (months.toList()..sort()),
      'description': description.text.trim().isEmpty
          ? null
          : description.text.trim(),
    });
    return success ? true : null;
  }

  String? _amountError(String? value) {
    final normalized =
        MoneyText.normalizeInput(value ?? '') ??
        value?.trim().replaceAll(',', '.');
    final parsed = double.tryParse(normalized ?? '');
    if (parsed == null || parsed <= 0) return 'Sıfırdan büyük tutar girin.';
    if (!RegExp(r'^\d+(?:[.,]\d{1,4})?$').hasMatch(value!.trim())) {
      return 'En fazla dört ondalık basamak kullanın.';
    }
    return null;
  }
}

String _formatDate(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-'
    '${value.month.toString().padLeft(2, '0')}-'
    '${value.day.toString().padLeft(2, '0')}';

String _timingLabel(String timing) => switch (timing) {
  'overdue' => 'Gecikmiş',
  'today' => 'Bugün',
  _ => 'Yaklaşan',
};

String _sourceLabel(String source) => switch (source) {
  'recurring-occurrence' => 'Tekrarlayan plan',
  'credit-card-statement' => 'Kredi kartı ekstresi',
  'installment' => 'Taksit',
  'debt-installment' => 'Borç taksiti',
  _ => source,
};

IconData _sourceIcon(String source) => switch (source) {
  'credit-card-statement' => Icons.credit_card,
  'installment' => Icons.calendar_view_month,
  'debt-installment' => Icons.handshake_outlined,
  _ => Icons.event_repeat,
};
