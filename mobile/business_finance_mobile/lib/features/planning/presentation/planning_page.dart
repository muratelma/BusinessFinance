import 'dart:math' as math;

import 'package:flutter/material.dart';

import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_menu_group_label.dart';
import '../../../core/widgets/app_responsive_grid.dart';
import '../../../core/widgets/app_row_action.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/planning_models.dart';
import '../data/planning_repository.dart';
import 'planning_controller.dart';

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
      appBar: AppBar(
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

  Widget _recurringTab(PlanningSnapshot snapshot) => RefreshIndicator(
    onRefresh: controller.load,
    child: ListView(
      padding: const EdgeInsets.all(AppSpacing.medium),
      children: [
        FilledButton.icon(
          onPressed: controller.isSubmitting
              ? null
              : () => _showRecurringForm(snapshot),
          icon: const Icon(Icons.add),
          label: const Text('Tekrarlayan plan ekle'),
        ),
        const SizedBox(height: AppSpacing.medium),
        if (snapshot.recurringTransactions.isEmpty)
          const SizedBox(
            height: 220,
            child: AppEmptyView(
              title: 'Henüz tekrarlayan plan yok',
              message: 'Düzenli gelir, gider veya fatura planınızı ekleyin.',
              icon: Icons.event_repeat,
            ),
          )
        else ...[
          for (final item in snapshot.recurringTransactions.where(
            (item) => item.isActive,
          ))
            _RecurringPlanCard(
              item: item,
              snapshot: snapshot,
              controller: controller,
            ),
          // Paused plans are kept but folded away: they are history the user
          // chose to stop, and after a dozen of them they would bury the plans
          // that still run.
          if (snapshot.recurringTransactions.any((item) => !item.isActive))
            ExpansionTile(
              title: Text(
                'Duraklatılmış planlar '
                '(${snapshot.recurringTransactions.where((i) => !i.isActive).length})',
              ),
              children: [
                for (final item in snapshot.recurringTransactions.where(
                  (item) => !item.isActive,
                ))
                  _RecurringPlanCard(
                    item: item,
                    snapshot: snapshot,
                    controller: controller,
                  ),
              ],
            ),
        ],
      ],
    ),
  );

  Widget _upcomingTab(PlanningSnapshot snapshot) => RefreshIndicator(
    onRefresh: controller.load,
    child: ListView(
      padding: const EdgeInsets.all(AppSpacing.medium),
      children: [
        Text('Vade ufku', style: Theme.of(context).textTheme.labelLarge),
        const SizedBox(height: AppSpacing.small),
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
        const SizedBox(height: AppSpacing.medium),
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
        const SizedBox(height: AppSpacing.medium),
        if (snapshot.upcomingPayments.isEmpty)
          const SizedBox(
            height: 260,
            child: AppEmptyView(
              title: 'Yaklaşan ödeme yok',
              message: 'Seçilen vade ufkunda bekleyen yükümlülük bulunmuyor.',
              icon: Icons.event_available,
            ),
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
                padding: const EdgeInsets.only(bottom: AppSpacing.small),
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
                    trailing: AppMoneyText(
                      amount: item.amount,
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
    final generated = snapshot.occurrences.any(
      (occurrence) => occurrence.id == item.sourceId && occurrence.canRealize,
    );
    // Vakti gelmemiş satırda onay yok: plan tarihi gelene kadar bir tahmindir
    // ve gelecek ayın kirasını bugün yazmak parayı çıkmadığı bir aya koyar.
    final approvable =
        item.sourceType == 'recurring-occurrence' && timing != 'upcoming';
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
        else
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

  /// Para hareket etmeden önce açık onay ister ve **hangi hesaptan** çıkacağını
  /// söyler.
  ///
  /// İkisi de eksikti: satır tek dokunuşla para hareket ettiriyordu ve kaynağı
  /// hiçbir yerde yazmıyordu. Kaynak planda duruyor — tekrarlayan planın
  /// kaynağı kuruluşta seçilir — ama kullanıcı onaylarken onu göremiyordu.
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

    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.playlist_add_check,
      title: 'Gerçekleştirilsin mi?',
      highlight:
          '${item.title}\n'
          '${MoneyText.format(item.amount, item.currency)}',
      message:
          '${source == null ? 'Bu kayıt' : '$source hesabındaki bu kayıt'} '
          'gerçek harekete dönüşecek ve bakiyeye girecek.',
      confirmLabel: 'Gerçekleştir',
    );
    if (!confirmed) return;

    if (generated) {
      await controller.realizeOccurrence(item.sourceId);
    } else {
      await controller.realizeDue(item.sourceId, item.dueDate);
    }
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
      padding: const EdgeInsets.all(AppSpacing.medium),
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
        const SizedBox(height: AppSpacing.large),
        const AppSectionHeader(title: 'Nakit akışı eğilimi'),
        AppCard(
          child: CashFlowChart(
            points: report.cashFlowTrend,
            currency: report.currency,
          ),
        ),
        const SizedBox(height: AppSpacing.large),
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
        const SizedBox(height: AppSpacing.large),
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
        const SizedBox(height: AppSpacing.large),
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

  Future<void> _showRecurringForm(PlanningSnapshot snapshot) async {
    await AppFormSheet.show<bool>(
      context: context,
      builder: (context) => _RecurringForm(
        snapshot: snapshot,
        onSubmit: controller.createRecurring,
      ),
    );
  }
}

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
            'Kart borcu ${MoneyText.format(report.creditCardDebt, report.currency)}',
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
      const SizedBox(height: AppSpacing.large),
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
  const _RecurringForm({required this.snapshot, required this.onSubmit});
  final PlanningSnapshot snapshot;
  final Future<bool> Function(Map<String, Object?>) onSubmit;

  @override
  State<_RecurringForm> createState() => _RecurringFormState();
}

class _RecurringFormState extends State<_RecurringForm> {
  final formKey = GlobalKey<FormState>();
  final amount = TextEditingController();
  final occurrenceLimit = TextEditingController();
  final description = TextEditingController();
  String? accountId;
  String? creditCardId;
  String? categoryId;
  String kind = 'expense';
  String frequency = 'monthly';
  String monthEndBehavior = 'clamp-to-last-day';
  DateTime startDate = DateTime.now();

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
          DropdownButtonFormField<String>(
            key: ValueKey('income-account-$accountId'),
            initialValue: accountId,
            isExpanded: true,
            decoration: const InputDecoration(labelText: 'Hesap'),
            items: [
              for (final account in widget.snapshot.accounts)
                DropdownMenuItem(value: account.id, child: Text(account.name)),
            ],
            onChanged: (value) => setState(() {
              accountId = value;
              creditCardId = null;
            }),
            validator: (value) => value == null ? 'Hesap seçin.' : null,
          )
        else
          DropdownButtonFormField<String>(
            key: ValueKey('source-$kind-$_sourceValue'),
            initialValue: _sourceValue,
            isExpanded: true,
            decoration: const InputDecoration(labelText: 'Kaynak'),
            items: [
              if (widget.snapshot.accounts.isNotEmpty)
                const DropdownMenuItem<String>(
                  enabled: false,
                  child: AppMenuGroupLabel('Hesaplar'),
                ),
              for (final account in widget.snapshot.accounts)
                DropdownMenuItem(
                  value: 'account:${account.id}',
                  child: Text(account.name),
                ),
              if (widget.snapshot.creditCards.isNotEmpty)
                const DropdownMenuItem<String>(
                  enabled: false,
                  child: AppMenuGroupLabel('Kredi kartları'),
                ),
              for (final card in widget.snapshot.creditCards)
                DropdownMenuItem(
                  value: 'card:${card.id}',
                  child: Text(card.name),
                ),
            ],
            onChanged: (value) => setState(() {
              if (value == null) return;
              final isCard = value.startsWith('card:');
              creditCardId = isCard ? value.substring(5) : null;
              accountId = isCard ? null : value.substring(8);
            }),
            validator: (value) => value == null ? 'Kaynak seçin.' : null,
          ),
        const SizedBox(height: AppSpacing.medium),
        DropdownButtonFormField<String>(
          key: ValueKey('category-$kind-$categoryId'),
          initialValue: categoryId,
          decoration: const InputDecoration(labelText: 'Kategori'),
          items: [
            for (final category in _availableCategories)
              DropdownMenuItem(value: category.id, child: Text(category.name)),
          ],
          onChanged: (value) => categoryId = value,
          validator: (value) => value == null ? 'Kategori seçin.' : null,
        ),
        const SizedBox(height: AppSpacing.medium),
        TextFormField(
          controller: amount,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          decoration: const InputDecoration(labelText: 'Tutar'),
          validator: _amountError,
        ),
        const SizedBox(height: AppSpacing.medium),
        DropdownButtonFormField<String>(
          initialValue: kind,
          decoration: const InputDecoration(labelText: 'Tür'),
          items: const [
            DropdownMenuItem(value: 'income', child: Text('Gelir')),
            DropdownMenuItem(value: 'expense', child: Text('Gider')),
            DropdownMenuItem(
              value: 'bill-payment',
              child: Text('Fatura / abonelik'),
            ),
          ],
          onChanged: (value) => setState(() {
            kind = value!;
            if (!_availableCategories.any(
              (category) => category.id == categoryId,
            )) {
              categoryId = null;
            }
          }),
        ),
        const SizedBox(height: AppSpacing.medium),
        DropdownButtonFormField<String>(
          initialValue: frequency,
          decoration: const InputDecoration(labelText: 'Sıklık'),
          items: const [
            DropdownMenuItem(value: 'daily', child: Text('Günlük')),
            DropdownMenuItem(value: 'weekly', child: Text('Haftalık')),
            DropdownMenuItem(value: 'monthly', child: Text('Aylık')),
            DropdownMenuItem(value: 'yearly', child: Text('Yıllık')),
          ],
          onChanged: (value) => setState(() => frequency = value!),
        ),
        const SizedBox(height: AppSpacing.medium),
        DropdownButtonFormField<String>(
          initialValue: monthEndBehavior,
          decoration: const InputDecoration(labelText: 'Ay sonu davranışı'),
          items: const [
            DropdownMenuItem(
              value: 'clamp-to-last-day',
              child: Text('Ayın son gününe taşı'),
            ),
            DropdownMenuItem(
              value: 'skip-invalid-period',
              child: Text('Geçersiz ayı atla'),
            ),
          ],
          onChanged: (value) => monthEndBehavior = value!,
        ),
        const SizedBox(height: AppSpacing.medium),
        AppDateField(
          label: 'Başlangıç',
          value: _formatDate(startDate),
          onChanged: (value) => setState(() {
            startDate = AppDateField.parse(value)!;
            final end = AppDateField.parse(endDate);
            if (end != null && end.isBefore(startDate)) endDate = null;
          }),
        ),
        const SizedBox(height: AppSpacing.medium),
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
        const SizedBox(height: AppSpacing.medium),
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
        const SizedBox(height: AppSpacing.medium),
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

  String? get _sourceValue => creditCardId != null
      ? 'card:$creditCardId'
      : (accountId == null ? null : 'account:$accountId');

  Iterable<PlanningChoice> get _availableCategories {
    final requiredType = kind == 'income' ? 'income' : 'expense';
    return widget.snapshot.categories.where(
      (category) => category.type == null || category.type == requiredType,
    );
  }

  /// Panel yalnız başarıda kapanır. Başarısızlıkta `null` döner ve alanlar
  /// ekranda kalır; hata mesajını controller banner'da gösterir.
  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    final normalizedAmount = double.parse(
      amount.text.trim().replaceAll(',', '.'),
    ).toStringAsFixed(4);
    final success = await widget.onSubmit({
      // Sent explicitly so the server never has to infer which source this is.
      'sourceType': creditCardId == null ? 'account' : 'credit-card',
      'accountId': accountId,
      'creditCardId': creditCardId,
      'categoryId': categoryId,
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
      'description': description.text.trim().isEmpty
          ? null
          : description.text.trim(),
    });
    return success ? true : null;
  }

  String? _amountError(String? value) {
    final normalized = value?.trim().replaceAll(',', '.');
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

String? _categoryName(PlanningSnapshot snapshot, String categoryId) {
  for (final category in snapshot.categories) {
    if (category.id == categoryId) return category.name;
  }
  return null;
}

String _kindLabel(String kind) => switch (kind) {
  'income' => 'Düzenli gelir',
  'expense' => 'Düzenli gider',
  'bill-payment' => 'Fatura / abonelik',
  _ => kind,
};

IconData _kindIcon(String kind) => switch (kind) {
  'income' => Icons.south_west,
  'expense' => Icons.north_east,
  _ => Icons.receipt_long,
};

String _frequencyLabel(String frequency) => switch (frequency) {
  'daily' => 'Günlük',
  'weekly' => 'Haftalık',
  'monthly' => 'Aylık',
  'yearly' => 'Yıllık',
  _ => frequency,
};

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

/// Tekrarlayan bir planın liste satırı.
///
/// Satırda **hiç denetim yok**: ikon, ad, özet ve sağda tutar. Önceki iki
/// deneme de cihazda yanlış çıktı — önce üç satırlık bir `SwitchListTile` ve
/// altında tam genişlikte bir `Sil` şeridi vardı (kart içindeki bilgiden çok
/// daha büyüktü), sonra silme satırın rozet alanına taşındı (nadir ve geri
/// alınamaz bir iş, her satırda bağırıyordu ve tutarın yerini almıştı).
///
/// Duraklatma ve silme, satıra dokununca açılan panelde. Liste böylece tek
/// işi yapıyor: hangi planlar var, ne kadar ve ne zaman.
class _RecurringPlanCard extends StatelessWidget {
  const _RecurringPlanCard({
    required this.item,
    required this.snapshot,
    required this.controller,
  });

  final RecurringTransactionItem item;
  final PlanningSnapshot snapshot;
  final PlanningController controller;

  String get _title =>
      item.description ??
      _categoryName(snapshot, item.categoryId) ??
      _kindLabel(item.kind);

  @override
  Widget build(BuildContext context) {
    final next = item.nextOccurrenceDate;

    return Padding(
      padding: const EdgeInsets.only(bottom: AppSpacing.small),
      child: AppCard(
        padding: EdgeInsets.zero,
        child: AppListRow(
          icon: _kindIcon(item.kind),
          title: _title,
          // Türü soldaki ikon zaten söylüyor; alt satır sıklığı ve sıradaki
          // tarihi taşıyor. Ham `2026-09-30` sunucunun iç gösterimi.
          subtitle: [
            _frequencyLabel(item.frequency),
            if (item.occurrenceLimit != null)
              '${item.generatedOccurrenceCount} / ${item.occurrenceLimit} tekrar',
            next == null
                ? 'plan tamamlandı'
                : 'Sonraki ${DateText.dayMonth(next)}',
          ].join(' · '),
          // Duraklatılmış plan yalnız bulunduğu bölümden anlaşılmıyor: bölüm
          // kapalıyken açıp tek satıra bakan kullanıcı durumu görmeli.
          badge: item.isActive
              ? null
              : const AppStatusChip(
                  label: 'Duraklatıldı',
                  icon: Icons.pause_circle_outline,
                  tone: AppStatusTone.cancelled,
                ),
          dimmed: !item.isActive,
          trailing: AppMoneyText(amount: item.amount, currency: item.currency),
          onTap: () => _openSheet(context),
        ),
      ),
    );
  }

  Future<void> _openSheet(BuildContext context) async {
    final action = await AppAdaptiveSheet.show<_PlanAction>(
      context: context,
      builder: (sheetContext) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(
                AppSpacing.medium,
                0,
                AppSpacing.medium,
                AppSpacing.small,
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    _title,
                    style: Theme.of(sheetContext).textTheme.titleLarge,
                  ),
                  const SizedBox(height: AppSpacing.xSmall),
                  Text(
                    '${MoneyText.format(item.amount, item.currency)} · '
                    '${_frequencyLabel(item.frequency)} · '
                    '${_kindLabel(item.kind)}',
                    style: Theme.of(sheetContext).textTheme.bodyMedium,
                  ),
                ],
              ),
            ),
            ListTile(
              leading: Icon(
                item.isActive
                    ? Icons.pause_circle_outline
                    : Icons.play_circle_outline,
              ),
              title: Text(item.isActive ? 'Duraklat' : 'Sürdür'),
              // Duraklatma silmenin yumuşak alternatifi: geçmiş kalır, plan
              // yalnız yeni kayıt üretmez.
              subtitle: Text(
                item.isActive
                    ? 'Geçmiş kalır; plan yeni kayıt üretmez.'
                    : 'Plan yeniden kayıt üretmeye başlar.',
              ),
              onTap: () =>
                  Navigator.of(sheetContext).pop(_PlanAction.toggleActive),
            ),
            ListTile(
              leading: const Icon(Icons.delete_outline),
              title: const Text('Sil'),
              subtitle: const Text(
                'Yalnız hiç gerçekleşmemiş plan silinebilir.',
              ),
              onTap: () => Navigator.of(sheetContext).pop(_PlanAction.delete),
            ),
            const SizedBox(height: AppSpacing.small),
          ],
        ),
      ),
    );

    if (action == null || !context.mounted) return;
    switch (action) {
      case _PlanAction.toggleActive:
        await controller.setRecurringActive(item.id, !item.isActive);
      case _PlanAction.delete:
        await _confirmDelete(context);
    }
  }

  Future<void> _confirmDelete(BuildContext context) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.delete_outline,
      destructive: true,
      highlight: _title,
      title: 'Plan silinsin mi?',
      message:
          'Plan ve henüz onaylanmamış kayıtları kaldırılır. Bu plandan daha '
          'önce gerçekleşmiş hareketler varsa silme reddedilir; onları korumak '
          'için planı duraklatabilirsiniz.',
      confirmLabel: 'Sil',
    );
    if (confirmed) await controller.deleteRecurring(item.id);
  }
}

/// Panelden dönen seçim.
enum _PlanAction { toggleActive, delete }
