import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_input.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_card_head.dart';
import '../../../core/widgets/app_detail_block.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_status_tag.dart';
import '../../../core/widgets/app_text_action.dart';
import '../data/pos_repository.dart';
import 'pos_controller.dart';

/// POS tahsilatları bölümü: Kasa ekranının tek akışındaki kart.
///
/// Bu ekranda `kart` kelimesi tek başına geçmez (ADR 0015): borçlandığın kart
/// başka bir şeydir. Henüz hesaba geçmemiş para **yolda**dır; `bloke`
/// bankacılık jargonudur ve kullanıcının kelimesi değildir.
///
/// Tasarım teslimi (27 Eylül 2026): başlık şeridi `Yolda · hesaba geçmedi` ve
/// tahsilat sayısı, yoldaki toplam, altında yoldakiler (mavi saat kapsülü) ve
/// son geçenler (yeşil tik). Ekleme bölüm başlığındaki `+ Ekle` ile; sayfanın
/// kendi yüzen düğmesi yok (06.2 Grup 6 madde 1).
class PosSection extends StatefulWidget {
  const PosSection({required this.controller, super.key, this.scopeController});

  final PosController controller;
  final ScopeController? scopeController;

  /// Geçmiş tahsilatlardan kaç tanesi gösterilir; yoldakilerin hepsi durur.
  static const recentTransferredCount = 3;

  @override
  State<PosSection> createState() => _PosSectionState();
}

class _PosSectionState extends State<PosSection> {
  @override
  void initState() {
    super.initState();
    widget.controller.addListener(_changed);
    widget.controller.load();
  }

  @override
  void dispose() {
    widget.controller.removeListener(_changed);
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final controller = widget.controller;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppSectionHeader(
          title: 'POS tahsilatları',
          padding: EdgeInsets.zero,
          trailing: AppTextAction(
            label: 'Ekle',
            icon: Icons.add,
            onPressed: () => showPosSettlementForm(
              context,
              controller,
              widget.scopeController,
            ),
          ),
        ),
        _body(context, controller),
      ],
    );
  }

  Widget _body(BuildContext context, PosController controller) {
    if (controller.isLoading && controller.settlements == null) {
      return const AppCard(
        child: AppLoadingView(message: 'Tahsilatlar yükleniyor'),
      );
    }
    if (controller.unauthorized && controller.settlements == null) {
      return const AppUnauthorizedView();
    }
    if (controller.errorMessage != null && controller.settlements == null) {
      return AppCard(
        child: AppErrorView(
          message: controller.errorMessage!,
          onRetry: controller.load,
        ),
      );
    }
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final inTransit = controller.items.where((item) => item.isInTransit);
    final transferred = controller.items
        .where((item) => !item.isInTransit)
        .take(PosSection.recentTransferredCount);
    final rows = [...inTransit, ...transferred];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (controller.isStale)
          Padding(
            padding: const EdgeInsets.only(bottom: AppSpacing.small),
            child: AppInlineNotice(
              message:
                  '${controller.errorMessage} Son bilinen liste gösteriliyor.',
              actionLabel: 'Yenile',
              onAction: controller.load,
            ),
          ),
        AppCard(
          padding: EdgeInsets.zero,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AppCardHead(
                title: 'Yolda',
                meta: 'hesaba geçmedi',
                status: AppStatusTag(
                  label: '${controller.inTransitCount} tahsilat',
                  icon: Icons.schedule,
                  tone: AppStatusTone.neutral,
                ),
              ),
              MergeSemantics(
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.medium,
                    vertical: AppSpacing.small + AppSpacing.xSmall,
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      AppMoneyText(
                        amount: controller.moneyInTransit,
                        currency: 'TRY',
                        size: AppMoneySize.metric,
                        style: const TextStyle(fontWeight: FontWeight.w700),
                      ),
                      const SizedBox(height: AppSpacing.xxSmall),
                      // Yoldaki para kullanıcınındır ve net varlığa girer ama
                      // bugün harcanamaz (ADR 0015).
                      Text(
                        'Net varlığa dahildir',
                        style: theme.textTheme.bodySmall,
                      ),
                    ],
                  ),
                ),
              ),
              Divider(height: 1, thickness: 1, color: surfaces.border),
              if (rows.isEmpty)
                Padding(
                  padding: const EdgeInsets.all(AppSpacing.medium),
                  child: Text(
                    'Tahsilat yok. Müşterinizin kartla ödediği tutarı '
                    'yazdığınızda satış o gün gelir olarak tanınır; para '
                    'hesabınıza geçtiğinde işaretlersiniz.',
                    style: theme.textTheme.bodySmall,
                  ),
                )
              else
                AppDividedColumn(
                  inset: AppIconCapsule.rowInset,
                  children: [
                    for (final item in rows)
                      _SettlementRow(
                        item: item,
                        onTap: () => _openDetail(context, controller, item),
                      ),
                  ],
                ),
            ],
          ),
        ),
      ],
    );
  }

  Future<void> _openDetail(
    BuildContext context,
    PosController controller,
    PosSettlementItem item,
  ) => AppAdaptiveSheet.show<void>(
    context: context,
    builder: (_) => PosSettlementSheet(item: item, controller: controller),
  );
}

/// Tahsilatın başlığı: kullanıcının yazdığı açıklama, yoksa günü.
String posSettlementTitle(PosSettlementItem item) =>
    item.description ?? '${DateText.dayMonth(item.settlementDate)} gün sonu';

class _SettlementRow extends StatelessWidget {
  const _SettlementRow({required this.item, required this.onTap});

  final PosSettlementItem item;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final late = item.isInTransit && item.isLate;
    final subtitle = item.isInTransit
        ? '${late ? 'Gecikti · beklenen' : 'Hesaba geçecek ·'} '
              '${DateText.dayMonth(item.expectedTransferDate)}'
        : 'Hesaba geçti · ${DateText.dayMonth(item.transferredOn!)}';
    return AppRow(
      onTap: onTap,
      leading: AppIconCapsule(
        icon: item.isInTransit
            ? (late ? Icons.warning_amber_rounded : Icons.schedule)
            : Icons.check,
        tone: item.isInTransit
            ? (late ? AppStatusTone.expense : AppStatusTone.neutral)
            : AppStatusTone.income,
      ),
      title: posSettlementTitle(item),
      subtitle: subtitle,
      trailing: AppMoneyText(
        amount: item.netAmount,
        currency: item.currency,
        size: AppMoneySize.row,
        style: item.isInTransit ? null : TextStyle(color: surfaces.inkMuted),
      ),
    );
  }
}

/// POS tahsilatının ayrıntısı: net tutar ve durum, brüt/komisyon/gün/hesap,
/// kural metni ve yoldaysa `Hesaba geçti`.
class PosSettlementSheet extends StatelessWidget {
  const PosSettlementSheet({
    required this.item,
    required this.controller,
    super.key,
  });

  final PosSettlementItem item;
  final PosController controller;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final done = !item.isInTransit;
    final note = theme.textTheme.bodySmall;
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
                const AppIconCapsule(icon: Icons.point_of_sale_outlined),
                const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Semantics(
                        header: true,
                        child: Text(
                          posSettlementTitle(item),
                          style: theme.textTheme.titleSmall,
                        ),
                      ),
                      Text(
                        ['POS tahsilatı', ?item.scope?.label].join(' · '),
                        style: note,
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
            Wrap(
              alignment: WrapAlignment.spaceBetween,
              crossAxisAlignment: WrapCrossAlignment.center,
              spacing: AppSpacing.small,
              runSpacing: AppSpacing.small,
              children: [
                AppMoneyText(
                  amount: item.netAmount,
                  currency: item.currency,
                  size: AppMoneySize.metric,
                  style: const TextStyle(
                    fontSize: 28,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                done
                    ? const AppStatusTag(
                        label: 'Hesaba geçti',
                        icon: Icons.check_circle_outline,
                        tone: AppStatusTone.income,
                      )
                    : AppStatusTag(
                        label:
                            '${item.isLate ? 'Gecikti' : 'Yolda'} · '
                            '${DateText.dayMonth(item.expectedTransferDate)}',
                        icon: item.isLate
                            ? Icons.warning_amber_rounded
                            : Icons.schedule,
                        tone: item.isLate
                            ? AppStatusTone.expense
                            : AppStatusTone.neutral,
                      ),
              ],
            ),
            Text('Hesaba geçecek net tutar', style: note),
            const SizedBox(height: AppSpacing.medium),
            AppDetailBlock(
              rows: [
                AppDetailRow(
                  label: 'Brüt satış',
                  trailing: AppMoneyText(
                    amount: item.grossAmount,
                    currency: item.currency,
                    size: AppMoneySize.body,
                  ),
                ),
                AppDetailRow(
                  label: 'Komisyon',
                  trailing: AppMoneyText(
                    amount: item.commissionAmount,
                    currency: item.currency,
                    effect: AppMoneyEffect.expense,
                    signed: !_isZeroMoney(item.commissionAmount),
                    size: AppMoneySize.body,
                  ),
                ),
                AppDetailRow(
                  label: done ? 'Geçtiği gün' : 'Beklenen gün',
                  value: DateText.dayMonth(
                    done ? item.transferredOn! : item.expectedTransferDate,
                  ),
                ),
                AppDetailRow(
                  label: done ? 'Geçtiği hesap' : 'Geçeceği hesap',
                  value: item.accountName,
                ),
              ],
            ),
            const SizedBox(height: AppSpacing.medium),
            Text(
              done
                  ? 'Tutar hesaba geçti; komisyon ayrı bir gider olarak '
                        'kaydedildi.'
                  : 'Yoldaki tutar net varlığa dahildir. Hesaba geçtiğinde '
                        'işaretleyin; komisyon gider olarak kalır.',
              style: note?.copyWith(color: surfaces.inkMuted),
            ),
            if (!done) ...[
              const SizedBox(height: AppSpacing.medium),
              FilledButton.icon(
                style: FilledButton.styleFrom(
                  minimumSize: const Size.fromHeight(48),
                ),
                onPressed: controller.isSubmitting
                    ? null
                    : () => _confirmTransfer(context),
                icon: const Icon(Icons.check),
                label: const Text('Hesaba geçti'),
              ),
            ],
            // Yanlış girişin düzeltme yolu (28 Eylül denetimi U12): geçiş
            // geri alınır, kayıt silinmez iptal edilir.
            const SizedBox(height: AppSpacing.small),
            if (done)
              OutlinedButton.icon(
                style: OutlinedButton.styleFrom(
                  minimumSize: const Size.fromHeight(48),
                ),
                onPressed: controller.isSubmitting
                    ? null
                    : () => _confirmRevert(context),
                icon: const Icon(Icons.undo),
                label: const Text('Hesaba geçmedi, geri al'),
              ),
            TextButton.icon(
              style: TextButton.styleFrom(
                minimumSize: const Size.fromHeight(48),
                foregroundColor: theme.colorScheme.error,
              ),
              onPressed: controller.isSubmitting
                  ? null
                  : () => _confirmCancel(context),
              icon: const Icon(Icons.block),
              label: const Text('Kaydı iptal et'),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _confirmRevert(BuildContext context) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.undo,
      title: 'Geçiş geri alınsın mı?',
      message:
          'Net tutar hesaptan geri çekilir ve yeniden yolda görünür. Satış ve '
          'komisyon olduğu gibi kalır.',
      highlight: MoneyText.format(item.netAmount, item.currency),
      confirmLabel: 'Geri al',
    );
    if (!confirmed || !context.mounted) return;
    final saved = await controller.revertTransfer(item);
    if (saved && context.mounted) Navigator.of(context).maybePop();
  }

  Future<void> _confirmCancel(BuildContext context) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.block,
      title: 'POS tahsilatı iptal edilsin mi?',
      message: item.isInTransit
          ? 'Satış ve komisyon kayıtlardan düşer, yoldaki tutar kalkar.'
          : 'Satış ve komisyon kayıtlardan düşer, hesaba geçen tutar geri '
                'çekilir.',
      highlight: MoneyText.format(item.grossAmount, item.currency),
      confirmLabel: 'İptal et',
      destructive: true,
    );
    if (!confirmed || !context.mounted) return;
    final saved = await controller.cancel(item);
    if (saved && context.mounted) Navigator.of(context).maybePop();
  }

  Future<void> _confirmTransfer(BuildContext context) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.account_balance_outlined,
      title: 'Para hesaba geçti mi?',
      message:
          'Hesabınıza net tutar eklenir. Satış tahsil edildiği gün zaten gelir '
          'olarak yazıldı; bu adım gelir veya gider yazmaz.',
      highlight: MoneyText.format(item.netAmount, item.currency),
      confirmLabel: 'Geçti olarak işaretle',
    );
    if (!confirmed || !context.mounted) return;
    final saved = await controller.markTransferred(item, _today());
    if (saved && context.mounted) Navigator.of(context).maybePop();
  }
}

/// Yeni POS tahsilatı formunu açar (bölüm başlığındaki `+ Ekle` ve
/// `İşlem ekle > POS tahsilatı`). Kaydedilince `true`, vazgeçilince `null`.
Future<bool?> showPosSettlementForm(
  BuildContext context,
  PosController controller,
  ScopeController? scopeController,
) => AppFormSheet.show<bool>(
  context: context,
  builder: (_) =>
      _SettlementForm(controller: controller, scopeController: scopeController),
);

String _today() {
  final value = DateTime.now();
  return '${value.year.toString().padLeft(4, '0')}-'
      '${value.month.toString().padLeft(2, '0')}-'
      '${value.day.toString().padLeft(2, '0')}';
}

bool _isZeroMoney(String value) =>
    !value.replaceAll(RegExp('[^0-9]'), '').contains(RegExp('[1-9]'));

enum _CommissionMode { none, amount, rate }

class _SettlementForm extends StatefulWidget {
  const _SettlementForm({required this.controller, this.scopeController});

  final PosController controller;
  final ScopeController? scopeController;

  @override
  State<_SettlementForm> createState() => _SettlementFormState();
}

class _SettlementFormState extends State<_SettlementForm> {
  final formKey = GlobalKey<FormState>();
  final grossController = TextEditingController();
  final commissionController = TextEditingController();
  final descriptionController = TextEditingController();

  late final Future<PosOptions> options;
  String? accountId;
  String? categoryId;
  String? commissionCategoryId;
  _CommissionMode commissionMode = _CommissionMode.none;
  late String settlementDate = _today();
  late String expectedTransferDate = _today();
  TransactionScope? explicitScope;
  bool scopeMissing = false;
  PosOptions? loaded;

  @override
  void initState() {
    super.initState();
    options = widget.controller.loadOptions();
  }

  @override
  void dispose() {
    grossController.dispose();
    commissionController.dispose();
    descriptionController.dispose();
    super.dispose();
  }

  TransactionScope? get resolvedScope =>
      explicitScope ??
      _choiceScope(accountId, loaded?.accounts) ??
      _choiceScope(categoryId, loaded?.incomeCategories);

  TransactionScope? _choiceScope(String? id, List<DataChoice>? choices) {
    if (id == null || choices == null) return null;
    for (final choice in choices) {
      if (choice.id == id) return choice.defaultScope;
    }
    return null;
  }

  bool get showScope =>
      (widget.scopeController?.isVisible ?? false) || resolvedScope == null;

  @override
  Widget build(BuildContext context) => Form(
    key: formKey,
    child: AppFormSheet<bool>(
      title: 'POS tahsilatı',
      description:
          'Satış bugün gelir olarak tanınır; para hesabınıza geçtiğinde '
          'ayrıca işaretlersiniz.',
      submitLabel: 'Tahsilatı kaydet',
      onSubmit: _submit,
      children: [
        FutureBuilder<PosOptions>(
          future: options,
          builder: (context, snapshot) {
            if (snapshot.hasError) {
              return const Text(
                'Hesap ve kategoriler yüklenemedi. Paneli kapatıp yeniden '
                'deneyin.',
              );
            }
            if (!snapshot.hasData) return const LinearProgressIndicator();
            loaded = snapshot.data;
            return Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: _fields(snapshot.data!),
            );
          },
        ),
      ],
    ),
  );

  List<Widget> _fields(PosOptions options) {
    return [
      TextFormField(
        controller: grossController,
        keyboardType: const TextInputType.numberWithOptions(decimal: true),
        decoration: const InputDecoration(
          labelText: 'Müşterinin ödediği',
          helperText: 'Brüt tutar: kestiğiniz fatura budur.',
        ),
        validator: MoneyInput.positiveError,
      ),
      const SizedBox(height: AppSpacing.medium),
      DropdownButtonFormField<String>(
        initialValue: accountId,
        isExpanded: true,
        decoration: const InputDecoration(
          labelText: 'Paranın geçeceği hesap',
          helperText: 'POS parası bankaya geçer.',
        ),
        items: [
          for (final account in options.accounts)
            DropdownMenuItem(value: account.id, child: Text(account.name)),
        ],
        onChanged: (value) => setState(() => accountId = value),
        validator: (value) => value == null ? 'Hesap seçin.' : null,
      ),
      const SizedBox(height: AppSpacing.medium),
      DropdownButtonFormField<String>(
        initialValue: categoryId,
        isExpanded: true,
        decoration: const InputDecoration(labelText: 'Satış kategorisi'),
        items: [
          for (final category in options.incomeCategories)
            DropdownMenuItem(value: category.id, child: Text(category.name)),
        ],
        onChanged: (value) => setState(() => categoryId = value),
        validator: (value) => value == null ? 'Kategori seçin.' : null,
      ),
      const SizedBox(height: AppSpacing.medium),
      AppDateField(
        label: 'Tahsilat günü',
        value: settlementDate,
        lastDate: DateTime.now(),
        onChanged: (value) => setState(() => settlementDate = value),
      ),
      const SizedBox(height: AppSpacing.medium),
      AppDateField(
        label: 'Paranın beklendiği gün',
        value: expectedTransferDate,
        firstDate: AppDateField.parse(settlementDate),
        onChanged: (value) => setState(() => expectedTransferDate = value),
      ),
      const SizedBox(height: AppSpacing.medium),
      // Başlık segmentin dışında: üç segment genişliği paylaşınca
      // `Komisyon yok` kelimenin ortasından bölünüyordu. Kapsam seçicisinde
      // olduğu gibi grubun adı üstte durur, segmentler kısa kalır.
      Semantics(
        container: true,
        label: 'Komisyon',
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Komisyon', style: Theme.of(context).textTheme.labelMedium),
            const SizedBox(height: AppSpacing.xSmall),
            SegmentedButton<_CommissionMode>(
              segments: const [
                ButtonSegment(value: _CommissionMode.none, label: Text('Yok')),
                ButtonSegment(
                  value: _CommissionMode.amount,
                  label: Text('Tutar'),
                ),
                ButtonSegment(value: _CommissionMode.rate, label: Text('Oran')),
              ],
              selected: {commissionMode},
              onSelectionChanged: (value) => setState(() {
                commissionMode = value.first;
                commissionController.clear();
              }),
            ),
          ],
        ),
      ),
      if (commissionMode != _CommissionMode.none) ...[
        const SizedBox(height: AppSpacing.medium),
        TextFormField(
          controller: commissionController,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          decoration: InputDecoration(
            labelText: commissionMode == _CommissionMode.amount
                ? 'Kesilen komisyon'
                : 'Komisyon oranı (%)',
            helperText: commissionMode == _CommissionMode.amount
                ? 'Brüt tutardan düşülmez, yanında ayrı gider olarak durur.'
                : 'Örnek: 1,5 yazın. Oran saklanmaz, tutara çevrilir.',
          ),
          validator: MoneyInput.positiveError,
        ),
        const SizedBox(height: AppSpacing.medium),
        DropdownButtonFormField<String>(
          initialValue: commissionCategoryId,
          isExpanded: true,
          decoration: const InputDecoration(
            labelText: 'Komisyon gider kategorisi',
          ),
          items: [
            for (final category in options.expenseCategories)
              DropdownMenuItem(value: category.id, child: Text(category.name)),
          ],
          onChanged: (value) => setState(() => commissionCategoryId = value),
          validator: (value) =>
              value == null ? 'Komisyon için kategori seçin.' : null,
        ),
      ],
      const SizedBox(height: AppSpacing.medium),
      TextFormField(
        controller: descriptionController,
        decoration: const InputDecoration(labelText: 'Açıklama (isteğe bağlı)'),
      ),
      if (showScope) ...[
        const SizedBox(height: AppSpacing.medium),
        AppScopeField(
          value: resolvedScope,
          onChanged: (value) => setState(() {
            explicitScope = value;
            scopeMissing = false;
          }),
          errorText: scopeMissing ? 'Bu tahsilat için kapsam seçin.' : null,
        ),
      ],
    ];
  }

  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    if (resolvedScope == null) {
      setState(() => scopeMissing = true);
      return null;
    }

    // Komisyon **ya tutar ya oran** gider; ikisini birden göndermek sunucunun
    // reddettiği bir istektir ve haklıdır.
    String? commissionAmount;
    String? commissionRate;
    if (commissionMode == _CommissionMode.amount) {
      commissionAmount = MoneyInput.wire(commissionController.text);
    } else if (commissionMode == _CommissionMode.rate) {
      final percent = MoneyInput.parse(commissionController.text) ?? 0;
      commissionRate = (percent / 100).toStringAsFixed(4);
    }

    final description = descriptionController.text.trim();
    final saved = await widget.controller.create(
      accountId: accountId!,
      categoryId: categoryId!,
      grossAmount: MoneyInput.wire(grossController.text),
      settlementDate: settlementDate,
      expectedTransferDate: expectedTransferDate,
      scope: resolvedScope,
      commissionAmount: commissionAmount,
      commissionRate: commissionRate,
      commissionCategoryId: commissionMode == _CommissionMode.none
          ? null
          : commissionCategoryId,
      description: description.isEmpty ? null : description,
    );
    return saved ? true : null;
  }
}
