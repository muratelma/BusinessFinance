import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_detail_block.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_status_tag.dart';
import '../data/day_close_repository.dart';
import 'day_close_controller.dart';

/// Bir günün gün sonu ayrıntısını açar: o günün gün sonları (ana ve ekler),
/// her birinin yazdığı ve saydığı kayıtlar, dışarıda kalan kayıtlar ve geri
/// alma. Kasa'daki karttan ve İşlemler'de bir gün sonuna bağlı kaydın
/// ayrıntısından açılır.
Future<void> showDayCloseDay(
  BuildContext context, {
  required DayCloseController controller,
  required String date,
}) => AppAdaptiveSheet.show<void>(
  context: context,
  builder: (_) => DayCloseDaySheet(controller: controller, date: date),
);

/// Günün bütünü.
///
/// Ekran tek bir gün sonunu değil **günü** gösterir: hangi kayıttan açılırsa
/// açılsın ana ve ek gün sonları yan yana durur. Gün sonu tutar taşımaz;
/// buradaki toplamlar sunucunun kayıtlardan topladığıdır. Geri alma bir
/// bütündür: gün sonunun yazdığı kayıtlar birlikte iptal olur, saydığı
/// kayıtlar serbest kalır.
class DayCloseDaySheet extends StatefulWidget {
  const DayCloseDaySheet({
    required this.controller,
    required this.date,
    super.key,
  });

  final DayCloseController controller;
  final String date;

  @override
  State<DayCloseDaySheet> createState() => _DayCloseDaySheetState();
}

class _DayCloseDaySheetState extends State<DayCloseDaySheet> {
  DayCloseDay? day;
  bool loading = true;

  DayCloseController get controller => widget.controller;

  @override
  void initState() {
    super.initState();
    controller
      ..errorMessage = null
      ..errorCode = null
      ..addListener(_changed);
    _load();
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  Future<void> _load() async {
    final result = await controller.readDay(widget.date);
    if (!mounted) return;
    setState(() {
      loading = false;
      if (result != null) day = result;
    });
  }

  @override
  Widget build(BuildContext context) {
    final shown = day;
    if (shown == null) {
      // Kısa ve sabit: ortalanan bekleme görünümü paneli tam yüksekliğe
      // çıkarır, içerik gelince panel bir anda küçülürdü.
      return SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(AppSpacing.medium),
          child: SizedBox(
            height: 300,
            child: controller.unauthorized
                ? const AppUnauthorizedView()
                : loading
                ? const AppLoadingView(message: 'Gün sonu yükleniyor')
                : AppErrorView(
                    message: controller.errorMessage ?? 'Gün sonu okunamadı.',
                    onRetry: _load,
                  ),
          ),
        ),
      );
    }
    return _content(context, shown);
  }

  Widget _content(BuildContext context, DayCloseDay shown) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final note = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final error = controller.errorMessage;
    final hasAdditional = shown.closes.any((close) => close.isAdditional);
    return SafeArea(
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          0,
          AppSpacing.medium,
          AppSpacing.large,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          mainAxisSize: MainAxisSize.min,
          children: [
            Row(
              children: [
                AppIconCapsule(
                  icon: Icons.fact_check_outlined,
                  tone: shown.isClosed
                      ? AppStatusTone.income
                      : AppStatusTone.neutral,
                ),
                const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Semantics(
                        header: true,
                        child: Text(
                          'Gün sonu',
                          style: theme.textTheme.titleSmall,
                        ),
                      ),
                      Text(
                        DateText.dayMonthYear(shown.date),
                        style: theme.textTheme.bodySmall,
                      ),
                    ],
                  ),
                ),
                IconButton(
                  tooltip: 'Kapat',
                  onPressed: () => Navigator.of(context).maybePop(),
                  icon: const Icon(Icons.close),
                ),
              ],
            ),
            const SizedBox(height: AppSpacing.medium),
            Align(
              alignment: AlignmentDirectional.centerStart,
              child: shown.isClosed
                  ? const AppStatusTag(
                      label: 'Gün kapatıldı',
                      icon: Icons.check_circle_outline,
                      tone: AppStatusTone.income,
                    )
                  : const AppStatusTag(
                      label: 'Gün sonu girilmedi',
                      icon: Icons.radio_button_unchecked,
                      tone: AppStatusTone.neutral,
                    ),
            ),
            if (error != null)
              AppInlineNotice(
                message: error,
                icon: Icons.error_outline,
                margin: const EdgeInsets.only(top: AppSpacing.small),
              ),
            if (shown.isClosed) ...[
              const SizedBox(height: AppSpacing.medium),
              // Günün toplamı: yazılan ile sayılanın toplamı, yani o gün için
              // yazılan tutar. Sunucudan gelir.
              AppDetailBlock(
                rows: [
                  AppDetailRow(
                    icon: Icons.payments_outlined,
                    label: 'Nakit',
                    trailing: AppMoneyText(
                      amount: shown.cashTotal,
                      currency: shown.currency,
                      size: AppMoneySize.body,
                      style: const TextStyle(fontWeight: FontWeight.w600),
                    ),
                  ),
                  AppDetailRow(
                    icon: Icons.point_of_sale_outlined,
                    label: 'Kart',
                    trailing: AppMoneyText(
                      amount: shown.cardTotal,
                      currency: shown.currency,
                      size: AppMoneySize.body,
                      style: const TextStyle(fontWeight: FontWeight.w600),
                    ),
                  ),
                ],
              ),
            ],
            for (final close in shown.closes) ...[
              const SizedBox(height: AppSpacing.large - AppSpacing.xSmall),
              _CloseSection(
                close: close,
                // Ana gün sonu, ekleri geri alınmadan geri alınamaz.
                blockedByAdditional: !close.isAdditional && hasAdditional,
                busy: controller.isSubmitting,
                onRevert: () => _confirmRevert(context, close),
              ),
            ],
            if (shown.outsideRecords.isNotEmpty) ...[
              const SizedBox(height: AppSpacing.large - AppSpacing.xSmall),
              Text(
                shown.isClosed ? 'Gün sonunun dışında' : 'Tek tek girilenler',
                style: theme.textTheme.labelMedium,
              ),
              const SizedBox(height: AppSpacing.xSmall),
              _RecordBlock(
                children: [
                  for (final record in shown.outsideRecords)
                    _RecordRow(
                      title: _recordTitle(record),
                      subtitle: _recordSide(record),
                      amount: record.amount,
                      currency: shown.currency,
                    ),
                ],
              ),
              if (shown.isClosed) ...[
                const SizedBox(height: AppSpacing.xSmall),
                Text('Gün sonu tutarına katılmadı.', style: note),
              ],
            ],
            if (!shown.isClosed && shown.outsideRecords.isEmpty) ...[
              const SizedBox(height: AppSpacing.medium),
              Text('Bu gün için gün sonu girilmedi.', style: note),
            ],
          ],
        ),
      ),
    );
  }

  Future<void> _confirmRevert(BuildContext context, DayClose close) async {
    final count = close.recordCount;
    final navigator = Navigator.of(context);
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.undo,
      title: close.isAdditional
          ? 'Ek gün sonu geri alınsın mı?'
          : 'Gün sonu geri alınsın mı?',
      message: count == 0
          ? 'Gün yeniden açılır.'
          : 'Yazdığı $count kayıt birlikte iptal edilir. Saydığı kayıtlar '
                'yerinde kalır.',
      highlight: DateText.dayMonthYear(close.closedOn),
      confirmLabel: 'Geri al',
    );
    if (!confirmed || !mounted) return;
    final reverted = await controller.revert(close.id);
    if (reverted == null || !mounted) return;
    await _load();
    // Günde gün sonu kalmadıysa gösterilecek bir şey de kalmadı.
    if (mounted && !(day?.isClosed ?? false)) navigator.maybePop();
  }
}

String _recordTitle(DayCloseExistingRecord record) {
  if (record.title.isNotEmpty) return record.title;
  return switch (record.kind) {
    'pos-settlement' => 'POS satışı',
    'counterparty-payment' => 'Cari tahsilat',
    'obligation-settlement' => 'Alacak tahsilatı',
    'counterparty-charge' => 'Veresiye satış',
    'obligation' => 'Alacak faturası',
    _ => 'Gelir',
  };
}

/// Kaydın ne olduğu: aynı kişinin veresiye satışı ile tahsilatı aynı adı
/// taşır, satırları bu ayırır.
String _recordSide(DayCloseExistingRecord record) {
  if (!record.isCash) return record.isCardCollection ? 'Kartla tahsil' : 'Kart';
  if (record.isCollection) return 'Tahsilat';
  return switch (record.kind) {
    'counterparty-charge' => 'Veresiye satış',
    'obligation' => 'Alacak faturası',
    _ => 'Nakit',
  };
}

/// Bir gün sonu: yazdığı ve saydığı kayıtlar, altında geri alma.
class _CloseSection extends StatelessWidget {
  const _CloseSection({
    required this.close,
    required this.blockedByAdditional,
    required this.busy,
    required this.onRevert,
  });

  final DayClose close;
  final bool blockedByAdditional;
  final bool busy;
  final VoidCallback onRevert;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final note = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final name = close.isAdditional ? 'Ek gün sonu' : 'Gün sonu';
    final empty = close.recordCount == 0 && close.countedRecords.isEmpty;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          [name, if (close.zNumber != null) 'Z ${close.zNumber}'].join(' · '),
          style: theme.textTheme.labelMedium,
        ),
        const SizedBox(height: AppSpacing.xSmall),
        if (empty)
          Text('Kayıt yazmadı.', style: note)
        else
          _RecordBlock(
            children: [
              for (final income in close.incomes)
                _RecordRow(
                  title: 'Nakit satış',
                  subtitle: 'Yazıldı · ${income.accountName}',
                  amount: income.amount,
                  currency: close.currency,
                ),
              for (final settlement in close.settlements)
                _RecordRow(
                  title: settlement.posDefinitionName ?? 'POS satışı',
                  subtitle: settlement.posDepositId != null
                      ? 'Yazıldı · hesaba geçti'
                      : 'Yazıldı · yolda',
                  amount: settlement.grossAmount,
                  currency: close.currency,
                ),
              for (final record in close.countedRecords)
                _RecordRow(
                  title: _recordTitle(record),
                  subtitle: 'Sayıldı · ${_recordSide(record).toLowerCase()}',
                  amount: record.amount,
                  currency: close.currency,
                ),
              // Satışta da tahsilatta da görünen para bir kez sayıldı; bu
              // satır olmadan sayılanlar günün toplamını tutmaz.
              for (final overlap in close.overlaps)
                if (overlap.overlapAmount case final amount?)
                  _RecordRow(
                    key: ValueKey('day-close-overlap-${overlap.groupId}'),
                    title: overlap.name.isEmpty
                        ? 'Alacak faturası'
                        : overlap.name,
                    subtitle: 'İkisinde de var · bir kez sayıldı',
                    amount: '-$amount',
                    currency: close.currency,
                  ),
            ],
          ),
        const SizedBox(height: AppSpacing.small),
        if (blockedByAdditional)
          Text('Geri almak için önce ek gün sonunu geri alın.', style: note)
        else
          OutlinedButton.icon(
            key: ValueKey('day-close-revert-${close.id}'),
            style: OutlinedButton.styleFrom(
              minimumSize: const Size.fromHeight(48),
            ),
            onPressed: busy ? null : onRevert,
            icon: const Icon(Icons.undo),
            label: Text(
              close.isAdditional ? 'Eki geri al' : 'Gün sonunu geri al',
            ),
          ),
      ],
    );
  }
}

class _RecordBlock extends StatelessWidget {
  const _RecordBlock({required this.children});

  final List<Widget> children;

  @override
  Widget build(BuildContext context) => Container(
    padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
    decoration: BoxDecoration(
      color: AppSurfaces.of(context).cardMuted,
      borderRadius: BorderRadius.circular(AppRadius.field),
    ),
    child: AppDividedColumn(children: children),
  );
}

class _RecordRow extends StatelessWidget {
  const _RecordRow({
    required this.title,
    required this.subtitle,
    required this.amount,
    required this.currency,
    super.key,
  });

  final String title;
  final String subtitle;
  final String amount;
  final String currency;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return MergeSemantics(
      child: ConstrainedBox(
        constraints: const BoxConstraints(minHeight: 48),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
          child: Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      title,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    Text(
                      subtitle,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppSurfaces.of(context).inkMuted,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: AppSpacing.medium),
              AppMoneyText(
                amount: amount,
                currency: currency,
                size: AppMoneySize.body,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Kasa'daki gün sonu kartı: bugün kapatıldı mı, kapatıldıysa ne yazıldı.
///
/// Kart kendi kaydını tutmaz: gördüğü, bugünün gün sonlarıdır. Gün kapalıyken
/// birincil eylem yoktur; ikinci cihazın gün sonu ikincil eylemle girilir.
class DayCloseTodayCard extends StatelessWidget {
  const DayCloseTodayCard({
    required this.controller,
    required this.onEnter,
    required this.onOpen,
    super.key,
  });

  final DayCloseController controller;

  /// Gün sonu panelini açar; `additional` gün zaten kapalıyken `true`.
  final void Function({required bool additional}) onEnter;

  /// Günün ayrıntısını açar; değer günün tarihidir.
  final ValueChanged<String> onOpen;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final today = controller.today;
    final closed = today?.isClosed ?? false;
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              AppIconCapsule(
                icon: Icons.fact_check_outlined,
                tone: closed ? AppStatusTone.income : AppStatusTone.neutral,
              ),
              const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Gün sonu', style: theme.textTheme.titleSmall),
                    Text(
                      closed ? 'Bugün girildi' : 'Bugün girilmedi',
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: surfaces.inkMuted,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          if (today != null && closed)
            // Günün toplamı tek satırda; dokununca günün ayrıntısı açılır.
            InkWell(
              key: const ValueKey('day-close-today-row'),
              onTap: () => onOpen(today.date),
              child: ConstrainedBox(
                constraints: const BoxConstraints(minHeight: 48),
                child: Padding(
                  padding: const EdgeInsets.only(top: AppSpacing.small),
                  child: Row(
                    children: [
                      Expanded(
                        child: Text(
                          'Nakit '
                          '${MoneyText.format(today.cashTotal, today.currency)}'
                          ' · Kart '
                          '${MoneyText.format(today.cardTotal, today.currency)}',
                          style: theme.textTheme.bodyMedium,
                        ),
                      ),
                      Icon(
                        Icons.chevron_right,
                        size: 22,
                        color: surfaces.inkMuted,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          const SizedBox(height: AppSpacing.small + AppSpacing.xSmall),
          if (closed)
            Align(
              alignment: AlignmentDirectional.centerStart,
              child: TextButton(
                onPressed: () => onEnter(additional: true),
                child: const Text('Ek gün sonu gir'),
              ),
            )
          else
            FilledButton(
              style: FilledButton.styleFrom(
                minimumSize: const Size.fromHeight(48),
              ),
              onPressed: () => onEnter(additional: false),
              child: const Text('Gün sonunu gir'),
            ),
        ],
      ),
    );
  }
}
