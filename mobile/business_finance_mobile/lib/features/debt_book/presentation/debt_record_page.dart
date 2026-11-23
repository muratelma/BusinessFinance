import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_filter_sheet.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../accounts/presentation/account_directory.dart';
import '../data/debt_book_models.dart';
import '../data/debt_book_repository.dart';
import 'debt_book_parts.dart';
import 'debt_discount_sheet.dart';
import 'debt_record_controller.dart';
import 'debt_record_view.dart';
import 'debt_settle_sheet.dart';

/// Taksitli ya da kişisiz kaydın sayfası (Faz 7, `05-EKRAN-TASARIMI.md`
/// §3.5): ödeme ya da tahsilat, ödemeyi geri alma, indirim, plan değişikliği
/// ve iptal.
class DebtRecordPage extends StatefulWidget {
  const DebtRecordPage({
    required this.repository,
    required this.debtId,
    required this.title,
    super.key,
    this.changes,
    this.personPage,
  });

  final DebtBookRepositoryContract repository;
  final String debtId;

  /// Kayıt okunana kadar başlıkta yazan ad.
  final String title;
  final FinancialDataChanges? changes;

  /// Kişinin sayfası (kimlik, ad).
  final Widget Function(String counterpartyId, String name)? personPage;

  @override
  State<DebtRecordPage> createState() => _DebtRecordPageState();
}

enum _RecordAction { plan, discount, cancel }

class _DebtRecordPageState extends State<DebtRecordPage> {
  late final DebtRecordController _controller = DebtRecordController(
    widget.repository,
    debtId: widget.debtId,
    changes: widget.changes,
  );
  late AccountChoices _choices;

  @override
  void initState() {
    super.initState();
    _controller.load();
    _controller.addListener(_closeIfCancelled);
  }

  @override
  void dispose() {
    _controller
      ..removeListener(_closeIfCancelled)
      ..dispose();
    super.dispose();
  }

  void _closeIfCancelled() {
    if (_controller.cancelled && mounted) Navigator.maybePop(context);
  }

  bool get _scopeVisible {
    try {
      return context.read<ScopeController?>()?.isVisible ?? false;
    } on ProviderNotFoundException {
      return false;
    }
  }

  void _say(String message) => ScaffoldMessenger.of(
    context,
  ).showSnackBar(SnackBar(content: Text(message)));

  Future<void> _pay(DebtRecordData record) async {
    final DebtSettleSources sources;
    try {
      sources = await _controller.loadSources();
    } on Exception {
      if (mounted) _say('Hesaplar okunamadı. Tekrar deneyin.');
      return;
    }
    if (!mounted) return;
    final collect = record.side == DebtSide.receivable;
    final options = [
      for (final account in sources.accounts)
        _choices
            .account(account.id, account.name, type: account.type)
            .withId('account:${account.id}'),
      if (collect)
        for (final pos in sources.posDefinitions)
          _choices
              .pos(pos.id, pos.name, accountName: pos.note)
              .withId('pos:${pos.id}')
      else
        for (final card in sources.cards)
          _choices.card(card.id, card.name).withId('card:${card.id}'),
    ];
    final next = record.openInstallments.firstOrNull;
    await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => DebtSettleSheet(
        side: record.side,
        personName: record.counterpartyName ?? record.title,
        subtitle:
            '${record.title} · kalan '
            '${MoneyText.format(record.remaining, 'TRY')}',
        shortcuts: DebtShortcuts(full: record.remaining, overdue: '0.0000'),
        chips: [
          if (record.isInstallment &&
              next != null &&
              next.openAmount != record.remaining)
            ('Sıradaki taksit', next.openAmount),
          ('Tamamı', record.remaining),
        ],
        fixedAmount: record.fixedPaymentAmount,
        overLimitNote: 'Kalandan fazla yazılamaz.',
        allowDiscount: false,
        options: options,
        initialAccountId: options.firstOrNull?.id,
        today: _controller.today,
        onSubmit: _controller.pay,
      ),
    );
  }

  Future<void> _reverse(DebtRecordClosing closing) async {
    final installments = _controller.record?.isInstallment ?? false;
    final payment = closing.isDiscount ? 'İndirimi' : 'Ödemeyi';
    final confirmed = await AppConfirmDialog.show(
      context: context,
      subject: AppConfirmSubject(
        title: closing.isDiscount ? 'İndirim' : (closing.sourceName ?? 'Ödeme'),
        detail: DateText.dayMonthYear(closing.date),
        amount: MoneyText.format(closing.amount, 'TRY'),
      ),
      message: installments
          ? 'Kapattığı taksitler yeniden açılır.'
          : 'Kalan tutar yeniden açılır.',
      confirmLabel: '$payment geri al',
      destructive: true,
    );
    if (!confirmed) return;
    final error = await _controller.reverse(closing);
    if (error != null && mounted) _say(error);
  }

  Future<void> _menu(DebtRecordData record, _RecordAction action) async {
    switch (action) {
      case _RecordAction.plan:
        await AppFormSheet.show<bool>(
          context: context,
          builder: (_) => _PlanSheet(
            record: record,
            today: _controller.today,
            onSubmit: _controller.changePlan,
          ),
        );
      case _RecordAction.discount:
        await AppFormSheet.show<bool>(
          context: context,
          builder: (_) => DebtDiscountSheet(
            title: record.title,
            amount: record.remaining,
            isSale: record.side == DebtSide.receivable,
            today: _controller.today,
            onSubmit: _controller.discount,
          ),
        );
      case _RecordAction.cancel:
        final confirmed = await AppConfirmDialog.show(
          context: context,
          subject: AppConfirmSubject(
            title: record.title,
            detail: record.counterpartyName,
            amount: MoneyText.format(record.remaining, 'TRY'),
          ),
          message:
              'Kayıt ödemeleri ve indirimleriyle birlikte iptal edilir; '
              'yazdığı para hareketleri geri alınır.',
          confirmLabel: 'Kaydı iptal et',
          destructive: true,
        );
        if (!confirmed) return;
        final error = await _controller.cancel();
        if (error != null && mounted) _say(error);
    }
  }

  Future<void> _openPerson(DebtRecordData record) async {
    final builder = widget.personPage;
    final id = record.counterpartyId;
    if (builder == null || id == null) return;
    await Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) => builder(id, record.counterpartyName ?? ''),
      ),
    );
    if (mounted) await _controller.load();
  }

  @override
  Widget build(BuildContext context) {
    _choices = AccountChoices.of(context);
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: AnimatedBuilder(
          animation: _controller,
          builder: (context, _) {
            final record = _controller.record;
            void back() => Navigator.maybePop(context);
            if (_controller.unauthorized) {
              return _framed(back, const AppUnauthorizedView());
            }
            if (record == null) {
              return _framed(
                back,
                _controller.errorMessage != null
                    ? AppErrorView(
                        message: _controller.errorMessage!,
                        onRetry: _controller.load,
                      )
                    : const AppLoadingView(message: 'Kayıt yükleniyor'),
              );
            }
            final actions = [
              if (record.canChangePlan) (_RecordAction.plan, 'Planı değiştir'),
              if (record.canDiscount) (_RecordAction.discount, 'İndirim'),
              (_RecordAction.cancel, 'Kaydı iptal et'),
            ];
            return DebtRecordView(
              record: record,
              scopeVisible: _scopeVisible,
              onBack: back,
              moreMenu: PopupMenuButton<_RecordAction>(
                tooltip: 'Diğer seçenekler',
                icon: const Icon(Icons.more_vert),
                onSelected: (action) => _menu(record, action),
                itemBuilder: (_) => [
                  for (final (action, label) in actions)
                    PopupMenuItem(
                      value: action,
                      child: Text(
                        label,
                        style: action == _RecordAction.cancel
                            ? TextStyle(
                                color: Theme.of(context).colorScheme.error,
                              )
                            : null,
                      ),
                    ),
                ],
              ),
              onPay: () => _pay(record),
              onOpenPerson: record.counterpartyId == null
                  ? null
                  : () => _openPerson(record),
              onShowSchedule: () => Navigator.of(context).push(
                MaterialPageRoute<void>(
                  builder: (_) => _SchedulePage(record: record),
                ),
              ),
              onOpenClosing: _reverse,
              onRefresh: _controller.load,
              notice: _controller.isStale
                  ? AppInlineNotice(
                      margin: EdgeInsets.zero,
                      message: 'Sayfa güncellenemedi.',
                      actionLabel: 'Tekrar dene',
                      onAction: _controller.load,
                    )
                  : null,
            );
          },
        ),
      ),
    );
  }

  Widget _framed(VoidCallback back, Widget body) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      AppPageHeader(title: widget.title, onBack: back),
      Expanded(child: body),
    ],
  );
}

/// Planı değiştirme: kalan tutar yeni takvime bölünür (sunucuda); eski
/// kalemler kalır ama kalana girmez. Faizli kaydın planı değişmez.
class _PlanSheet extends StatefulWidget {
  const _PlanSheet({
    required this.record,
    required this.today,
    required this.onSubmit,
  });

  final DebtRecordData record;
  final DateTime today;
  final Future<String?> Function({
    required String planKind,
    required String? firstDueDate,
    required int installmentCount,
  })
  onSubmit;

  @override
  State<_PlanSheet> createState() => _PlanSheetState();
}

class _PlanSheetState extends State<_PlanSheet> {
  late bool _installments = widget.record.isInstallment;
  late final TextEditingController _count = TextEditingController(
    text: widget.record.isInstallment
        ? '${widget.record.openInstallments.length}'
        : null,
  );
  late String? _firstDue = widget.record.openInstallments.firstOrNull?.dueDate;
  String? _error;

  @override
  void dispose() {
    _count.dispose();
    super.dispose();
  }

  Future<bool?> _submit() async {
    final count = _installments ? int.tryParse(_count.text.trim()) : 1;
    if (count == null || count < 1) {
      setState(() => _error = 'Taksit sayısını yazın.');
      return null;
    }
    if (_firstDue == null) {
      setState(() => _error = 'İlk vadeyi seçin.');
      return null;
    }
    final error = await widget.onSubmit(
      planKind: _installments ? 'installments' : 'single',
      firstDueDate: _firstDue,
      installmentCount: count,
    );
    if (error != null) {
      setState(() => _error = error);
      return null;
    }
    return true;
  }

  @override
  Widget build(BuildContext context) => AppFormSheet<bool>(
    title: 'Planı değiştir',
    description:
        'Kalan ${MoneyText.format(widget.record.remaining, 'TRY')} yeni '
        'takvime bölünür.',
    submitLabel: 'Kaydet',
    onSubmit: _submit,
    errorMessage: _error,
    children: [
      AppFormField(
        child: AppFilterSegments<bool>(
          values: const [true, false],
          selected: _installments,
          label: (value) => value ? 'Taksitli' : 'Tek ödeme',
          onChanged: (value) => setState(() => _installments = value),
          semanticLabel: 'Plan',
        ),
      ),
      if (_installments)
        AppFormField(
          child: TextFormField(
            controller: _count,
            keyboardType: TextInputType.number,
            decoration: const InputDecoration(labelText: 'Taksit sayısı'),
          ),
        ),
      AppFormField(
        child: AppDateField(
          label: _installments ? 'İlk taksit günü' : 'Vade',
          value: _firstDue,
          onChanged: (value) => setState(() => _firstDue = value),
        ),
      ),
      const DebtFieldNote('Ödenmiş taksitler ve ödemeler değişmez.'),
    ],
  );
}

/// Bütün takvim: ödenenler dahil, sırayla.
class _SchedulePage extends StatelessWidget {
  const _SchedulePage({required this.record});

  final DebtRecordData record;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppPageHeader(
            title: 'Takvim',
            onBack: () => Navigator.maybePop(context),
          ),
          Expanded(
            child: ListView(
              padding: AppPageInsets.list,
              children: [
                DebtSectionCard(
                  children: [
                    for (final item in record.installments)
                      DebtRowFrame(
                        leading: Text(
                          '${item.sequence}.',
                          style: Theme.of(context).textTheme.titleSmall,
                        ),
                        title: item.dueDate == null
                            ? 'Vadesiz'
                            : DateText.dayMonthYear(item.dueDate!),
                        trailing: DebtRowAmount(item.amount),
                        subtitle: DebtRowSubtitle(
                          alert: item.isOverdue ? 'Gecikti' : null,
                          parts: [
                            if (item.isPaid) 'Ödendi',
                            if (item.isPartlyPaid)
                              '${MoneyText.format(item.paidAmount, 'TRY')} '
                                  'ödendi',
                          ],
                        ),
                        semanticLabel:
                            '${item.sequence}. taksit, '
                            '${item.dueDate == null ? 'vadesiz' : DateText.dayMonthYear(item.dueDate!)}, '
                            '${MoneyText.format(item.amount, 'TRY')}'
                            '${item.isPaid ? ', ödendi' : ''}',
                      ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    ),
  );
}
