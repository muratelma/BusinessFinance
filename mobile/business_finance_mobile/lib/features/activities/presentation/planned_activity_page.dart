import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_row_action.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_unknown_amount.dart';
import '../data/activity_models.dart';
import '../data/activity_repository.dart';
import '../data/planned_activity_models.dart';
import 'planned_activity_controller.dart';

/// Planned movements, kept on their own screen so they never blend into the
/// realized history. Nothing here has touched a balance yet.
class PlannedActivityPageView extends StatefulWidget {
  const PlannedActivityPageView({
    required this.repository,
    super.key,
    this.changes,
    this.scopeController,
  });

  final ActivityRepositoryContract repository;
  final FinancialDataChanges? changes;

  /// Uygulama genelindeki kapsam anahtarı; bu ekran onu uygular ve yazar,
  /// değiştirmez.
  final ScopeController? scopeController;

  @override
  State<PlannedActivityPageView> createState() =>
      _PlannedActivityPageViewState();
}

class _PlannedActivityPageViewState extends State<PlannedActivityPageView> {
  late final PlannedActivityController _controller;

  @override
  void initState() {
    super.initState();
    _controller = PlannedActivityController(
      widget.repository,
      financialDataChanges: widget.changes,
      scopeController: widget.scopeController,
    );
    _controller.load();
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: AnimatedBuilder(
          animation: _controller,
          builder: (context, _) => Text(
            _controller.scope == null
                ? 'Planlananlar'
                : 'Planlananlar · ${_controller.scope!.label}',
          ),
        ),
      ),
      body: AnimatedBuilder(
        animation: _controller,
        builder: (context, _) => _buildBody(context),
      ),
    );
  }

  Widget _buildBody(BuildContext context) {
    if (_controller.unauthorized) return const AppUnauthorizedView();

    return Column(
      children: [
        _HorizonBar(
          selected: _controller.horizon,
          onSelected: _controller.selectHorizon,
        ),
        _TypeFilterBar(
          selected: _controller.typeFilter,
          onSelected: _controller.selectTypeFilter,
        ),
        const _BalanceNotice(),
        Expanded(child: _buildList(context)),
      ],
    );
  }

  /// Satırın eylemini yürütür.
  ///
  /// İki yol var ve ayrımı kaydın kendisi belirliyor: `realize` gövde
  /// istemiyor, o yüzden burada onayla tamamlanıyor. Ödeme ve tahsilat hangi
  /// hesaptan yapılacağını soruyor; formu bu listeye kopyalamak yerine
  /// kullanıcı o kaydın kendi ekranına gidiyor.
  Future<void> _act(PlannedActivity activity) async {
    // Vergi kalemi tutar, ödeme günü ve hesap/kart ister; onları vergi
    // ekranının "Ödedim" paneli sorar (ADR 0018 T4).
    if (activity.isTax) {
      await context.push(taxesLocation);
      return;
    }
    // Hesap seçen yükümlülük ödeme/tahsilat akışı Grup 6'da açılacak. Satır ve
    // uyarı bu grupta görünür, fakat yanlış bir forma yönlendirilmez.
    if (activity.plannedKind == PlannedKind.payableObligation ||
        activity.plannedKind == PlannedKind.receivableObligation) {
      return;
    }
    if (!activity.isDirectlyRealizable) {
      final destination = switch (activity.plannedKind) {
        PlannedKind.cardStatement ||
        PlannedKind.cardInstallment => '/more/cards',
        PlannedKind.debtInstallment ||
        PlannedKind.receivableInstallment => '/more/debts',
        PlannedKind.recurringOccurrence => '/more/planning',
        PlannedKind.payableObligation ||
        PlannedKind.receivableObligation => '/more/activities/planned',
      };
      await context.push(destination);
      return;
    }

    // Kaynak da yazılı: para bir yerden çıkacak ve kullanıcı onaylarken onun
    // hangisi olduğunu görmeli. Kaynağı burada sormuyoruz — tekrarlayan planın
    // kaynağı kuruluşta seçilir ve kayıt onu zaten taşır.
    final source = activity.sourceName;
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.playlist_add_check,
      title: 'Gerçekleştirilsin mi?',
      highlight:
          '${activity.title}\n'
          '${MoneyText.format(activity.amount!, activity.currency)}',
      message:
          '${source == null ? 'Bu kayıt' : '$source hesabındaki bu kayıt'} '
          'gerçek harekete dönüşecek ve bakiyeye girecek.',
      confirmLabel: 'Gerçekleştir',
    );
    if (!confirmed || !mounted) return;

    final success = await _controller.realize(activity);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          success
              ? 'Kayıt gerçekleşti ve bakiyeye girdi.'
              : _controller.errorMessage ?? 'Gerçekleştirilemedi.',
        ),
      ),
    );
  }

  Widget _buildList(BuildContext context) {
    if (_controller.isLoading && _controller.page == null) {
      return const AppLoadingView();
    }
    if (_controller.errorMessage != null && _controller.page == null) {
      return AppErrorView(
        message: _controller.errorMessage!,
        onRetry: _controller.load,
      );
    }
    if (_controller.isEmpty) {
      return const AppEmptyView(
        title: 'Planlanan hareket yok',
        message: 'Seçtiğiniz dönemde bekleyen bir ödeme veya plan bulunmuyor.',
        icon: Icons.schedule_outlined,
      );
    }

    final items = _controller.visibleItems;
    return RefreshIndicator(
      onRefresh: _controller.load,
      child: ListView.separated(
        padding: const EdgeInsets.only(bottom: AppSpacing.xLarge),
        itemCount: items.length,
        separatorBuilder: (_, _) => const SizedBox(height: AppSpacing.small),
        itemBuilder: (context, index) => PlannedActivityTile(
          key: ValueKey(items[index].listKey),
          activity: items[index],
          onAction: () => _act(items[index]),
        ),
      ),
    );
  }
}

/// A planned row is visibly not a realized one: a dashed border, a clock icon
/// and an explicit line saying it is not in the balance. None of that relies on
/// colour alone.
class PlannedActivityTile extends StatelessWidget {
  const PlannedActivityTile({required this.activity, super.key, this.onAction});

  final PlannedActivity activity;
  final VoidCallback? onAction;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = theme.colorScheme;
    final overdue = activity.timing == PlannedTiming.overdue;

    return Semantics(
      label: _semanticsLabel(),
      excludeSemantics: true,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
        child: AppCard(
          padding: EdgeInsets.zero,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Satır uygulamanın ortak liste satırı: ikon kapsülü, başlık,
              // alt satır, rozet ve sağda tutar. Önceki hâlde bu kart kendi
              // ölçülerini kuruyordu — 18 dp çıplak ikon, başlıkla aynı
              // kademede bir tutar ve satır içinde tam boy dolgulu bir buton —
              // ve aynı bilgiyi taşıyan diğer listelerden iri görünüyordu.
              AppListRow(
                icon: _kindIcon,
                title: activity.title,
                titleMaxLines: 1,
                subtitle: [
                  activity.plannedKind.label,
                  DateText.dayMonth(activity.dueDate),
                  ?activity.sourceName,
                ].join(' • '),
                badge: Wrap(
                  alignment: WrapAlignment.spaceBetween,
                  spacing: AppSpacing.small,
                  runSpacing: AppSpacing.small,
                  crossAxisAlignment: WrapCrossAlignment.center,
                  children: [
                    AppStatusChip(
                      label: activity.timing.label,
                      icon: switch (activity.timing) {
                        PlannedTiming.overdue => Icons.warning_amber_outlined,
                        PlannedTiming.today => Icons.today_outlined,
                        PlannedTiming.upcoming => Icons.schedule,
                      },
                      // Gecikmiş olan ödenmesi gereken bir borçtur; gider tonu
                      // bunu söyler. Vakti gelmemiş olan hâlâ yalnız plan.
                      tone: overdue
                          ? AppStatusTone.expense
                          : AppStatusTone.planned,
                    ),
                    if (onAction != null && _showsAction) _action(context),
                  ],
                ),
                trailing: activity.amount == null
                    // Tutarı ödeme gününe kadar belli olmayan vergi: sıfır
                    // değildir, tahmin de gösterilmez (ADR 0018 İ5).
                    ? const AppUnknownAmount()
                    : AppMoneyText(
                        amount: activity.amount!,
                        currency: activity.currency,
                        // Planlanan tutar henüz hareket etmedi; rolü taşır
                        // ama gerçekleşmiş bir kayıt gibi okunmaz.
                        effect: activity.effect == ActivityEffect.income
                            ? AppMoneyEffect.income
                            : AppMoneyEffect.expense,
                      ),
              ),
              if (activity.attentionCode != null)
                Padding(
                  padding: const EdgeInsets.fromLTRB(
                    AppSpacing.medium,
                    0,
                    AppSpacing.medium,
                    AppSpacing.small + AppSpacing.xSmall,
                  ),
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Icon(
                        Icons.warning_amber_outlined,
                        size: 16,
                        color: colors.error,
                      ),
                      const SizedBox(width: AppSpacing.small),
                      Expanded(
                        child: Text(
                          activity.attentionCode!.message,
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: colors.error,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }

  /// Satırın soluna gelen ikon.
  ///
  /// Türü söyler; zamanlamayı rozet söylüyor. Önceki hâlde her satırda aynı
  /// saat ikonu vardı ve rozetteki ikonu tekrarlıyordu.
  IconData get _kindIcon => switch (activity.plannedKind) {
    PlannedKind.recurringOccurrence => Icons.event_repeat,
    PlannedKind.cardInstallment => Icons.calendar_view_month,
    PlannedKind.cardStatement => Icons.credit_card,
    PlannedKind.debtInstallment ||
    PlannedKind.receivableInstallment => Icons.handshake_outlined,
    PlannedKind.payableObligation ||
    PlannedKind.receivableObligation => Icons.receipt_long_outlined,
  };

  /// Satırın eylemi; rozetle aynı yükseklikte.
  ///
  /// Eskiden tam boy bir `FilledButton.tonal`'dı: kartın en ağır ögesi oydu ve
  /// yanındaki rozetin neredeyse iki katı yükseklikteydi.
  Widget _action(BuildContext context) {
    if (activity.plannedKind == PlannedKind.payableObligation ||
        activity.plannedKind == PlannedKind.receivableObligation) {
      return const SizedBox.shrink();
    }
    // Vergi kalemi vergi ekranında ödenir; vadesi gelmemişi de ödenebilir.
    if (activity.isTax) {
      return AppRowAction(label: 'Ödedim →', onPressed: onAction);
    }
    return AppRowAction(
      // Ok yalnız başka bir ekrana giden eylemde: `Öde →` kullanıcıyı ödeme
      // formuna götürür, `Gerçekleştir` burada tamamlanır. Engelli bir
      // gerçekleştirme de burada tamamlanacak iştir, yalnız şu an yapılamıyor.
      label:
          activity.isDirectlyRealizable ||
              activity.actionKind == PlannedAction.realize
          ? activity.actionKind.label
          : '${activity.actionKind.label} →',
      // Engelli satırın nedeni zaten kartta yazılı; buton sessizce çalışmıyor
      // görünmesin diye kapatılıyor.
      onPressed: activity.needsAttention || activity.actionTargetId == null
          ? null
          : onAction,
    );
  }

  /// Vakti gelmemiş satırda eylem **hiç çıkmaz**, gri de çıkmaz.
  ///
  /// Kapalı bir buton "bir şey eksik" der ve kullanıcıyı eksiği aramaya
  /// gönderir; oysa eksik bir şey yok, yalnız gün gelmemiş. Satırdaki tarih ve
  /// `Yaklaşan` rozeti bunu zaten söylüyor.
  bool get _showsAction =>
      activity.isTax ||
      activity.actionKind != PlannedAction.realize ||
      activity.isDue;

  String _semanticsLabel() {
    final buffer = StringBuffer()
      ..write(activity.plannedKind.label)
      ..write('. ')
      ..write(activity.title)
      ..write('. ')
      ..write(
        activity.amount == null
            ? AppUnknownAmount.text
            : MoneyText.format(activity.amount!, activity.currency),
      )
      ..write('. ')
      ..write(activity.timing.label)
      ..write(' ')
      ..write(activity.dueDate)
      ..write('. Bakiyeye dahil değil');
    if (activity.isProjected) buffer.write('. Henüz oluşturulmadı');
    if (activity.attentionCode != null) {
      buffer.write('. Dikkat: ${activity.attentionCode!.message}');
    }
    return buffer.toString();
  }
}

class _HorizonBar extends StatelessWidget {
  const _HorizonBar({required this.selected, required this.onSelected});

  final PlannedHorizon selected;
  final ValueChanged<PlannedHorizon> onSelected;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        AppSpacing.small,
        AppSpacing.medium,
        0,
      ),
      child: SegmentedButton<PlannedHorizon>(
        segments: [
          for (final horizon in PlannedHorizon.values)
            ButtonSegment(value: horizon, label: Text(horizon.label)),
        ],
        selected: {selected},
        onSelectionChanged: (value) => onSelected(value.first),
      ),
    );
  }
}

class _TypeFilterBar extends StatelessWidget {
  const _TypeFilterBar({required this.selected, required this.onSelected});

  final PlannedTypeFilter selected;
  final ValueChanged<PlannedTypeFilter> onSelected;

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium,
        vertical: AppSpacing.small,
      ),
      child: Row(
        children: [
          for (final filter in PlannedTypeFilter.values)
            Padding(
              padding: const EdgeInsets.only(right: AppSpacing.small),
              child: ChoiceChip(
                label: Text(filter.label),
                selected: filter == selected,
                onSelected: (_) => onSelected(filter),
              ),
            ),
        ],
      ),
    );
  }
}

class _BalanceNotice extends StatelessWidget {
  const _BalanceNotice();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
      child: Text(
        'Planlananlar gerçekleşene kadar bakiyeye ve raporlara girmez.',
        style: theme.textTheme.bodySmall?.copyWith(
          color: theme.colorScheme.outline,
        ),
      ),
    );
  }
}
