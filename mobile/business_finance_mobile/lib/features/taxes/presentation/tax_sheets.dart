import 'dart:async';

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_math.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_detail_block.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_status_tag.dart';
import '../../../core/widgets/app_submit_button.dart';
import '../../activities/data/planned_activity_models.dart';
import '../data/tax_models.dart';
import 'tax_controller.dart';
import 'tax_parts.dart';
import 'tax_schedule.dart';

/// Sunucu tutarını Türkçe alan değerine çevirir (`8950.0000` → `8.950`).
String taxEditable(String amount) => const TurkishAmountInputFormatter()
    .formatEditUpdate(
      TextEditingValue.empty,
      TextEditingValue(text: MoneyText.editable(amount)),
    )
    .text;

/// Kalemin ya da ödemenin alt satırı: `Vergi · İşletme`. Kapsamı görmeyen
/// kullanıcıda yalnız `Vergi`. Kalemin kapsamı planındadır.
String taxKindLine(
  BuildContext context,
  TransactionScope? scope, {
  String prefix = 'Vergi',
}) {
  final visible = context.read<ScopeController?>()?.isVisible ?? false;
  return visible && scope != null ? '$prefix · ${scope.label}' : prefix;
}

// ---------------------------------------------------------------------------
// V3 · Ödedim
// ---------------------------------------------------------------------------

/// "Ödedim" panelini açar. Kaydedilince `true`.
Future<bool?> showTaxPaySheet(
  BuildContext context,
  TaxController controller,
  PlannedActivity item,
) {
  controller.clearWriteError();
  return AppAdaptiveSheet.show<bool>(
    context: context,
    builder: (_) => _PaySheet(controller: controller, item: item),
  );
}

class _PaySheet extends StatefulWidget {
  const _PaySheet({required this.controller, required this.item});

  final TaxController controller;
  final PlannedActivity item;

  @override
  State<_PaySheet> createState() => _PaySheetState();
}

class _PaySheetState extends State<_PaySheet> {
  late final bool late = widget.item.timing == PlannedTiming.overdue;

  // Gecikmiş kalemde tutar boş gelir: ödenen tutar gecikme zammını da içerir
  // ve uygulama zammı hesaplamaz.
  late final amount = TextEditingController(
    text: late || widget.item.amount == null
        ? ''
        : taxEditable(widget.item.amount!),
  );
  late String paidOn = widget.controller.todayIso;
  late String? sourceId = widget.item.sourceId;
  bool showErrors = false;

  @override
  void dispose() {
    amount.dispose();
    super.dispose();
  }

  String? get wireAmount => MoneyMath.fromInput(amount.text);

  @override
  Widget build(BuildContext context) {
    final controller = widget.controller;
    final item = widget.item;
    final options = controller.options;
    final card = options?.isCard(sourceId) ?? false;
    final fromPlan = sourceId != null && sourceId == item.sourceId;
    final rule = late
        ? 'Gecikme zammı dahil, ödediğiniz tutarı yazın; uygulama zammı '
              'hesaplamaz.'
        : (item.amount == null
              ? null
              : 'Beklenen tutar; ödediğiniz farklıysa değiştirin.');
    return ListenableBuilder(
      listenable: controller,
      builder: (context, _) => TaxSheetBody(
        footer: _SheetSubmit(
          error: controller.writeError,
          child: AppSubmitButton(
            label: 'Ödemeyi kaydet',
            icon: Icons.check,
            isBusy: controller.isSubmitting,
            onSubmit: _submit,
          ),
        ),
        children: [
          TaxSheetTitle(
            title: item.title,
            subtitle:
                'Vade ${DateText.dayMonth(item.dueDate)} · '
                '${TaxSchedule.relative(item.dueDate, controller.today)}',
          ),
          const SizedBox(height: AppSpacing.large),
          TaxAmountInput(
            controller: amount,
            label: 'Ödenen tutar',
            autofocus: late || item.amount == null,
          ),
          if (showErrors && wireAmount == null)
            _FieldError('Ödediğiniz tutarı yazın.'),
          if (rule != null) TaxRule(rule, textAlign: TextAlign.center),
          const SizedBox(height: AppSpacing.large),
          AppDateField(
            label: 'Ödeme günü',
            iconLeading: true,
            value: paidOn,
            valueText: taxDateText(paidOn, controller.today),
            lastDate: controller.today,
            onChanged: (value) => setState(() => paidOn = value),
          ),
          const SizedBox(height: AppSpacing.medium),
          if (options != null)
            TaxSourceField(
              options: options,
              value: sourceId,
              label: 'Nereden ödendi',
              errorText: showErrors && sourceId == null
                  ? 'Hesap ya da kart seçin.'
                  : null,
              helperText: card
                  ? 'Kart harcaması olarak yazılır; kart borcunuza eklenir.'
                  : (fromPlan ? 'Tanımdan geldi.' : null),
              onChanged: (value) => setState(() => sourceId = value),
            ),
          const TaxRule(
            'Gider ödeme gününe yazılır; vadeye değil.',
            top: AppSpacing.medium,
          ),
        ],
      ),
    );
  }

  Future<void> _submit() async {
    final value = wireAmount;
    if (value == null || sourceId == null) {
      setState(() => showErrors = true);
      return;
    }
    final saved = await widget.controller.pay(
      item: widget.item,
      amount: value,
      paidOn: paidOn,
      sourceId: sourceId!,
    );
    if (saved && mounted) Navigator.of(context).pop(true);
  }
}

// ---------------------------------------------------------------------------
// Tutarı gir
// ---------------------------------------------------------------------------

/// "Tutar belli oldu": bekleyen kaleme bu dönemin tutarını yazar; tanım
/// değişmez. "Ödedim" panelinin tutar kısmıyla aynı dil. Kaydedilince yazılan
/// tutarı (dört basamaklı) döner.
Future<String?> showTaxAmountSheet(
  BuildContext context,
  TaxController controller,
  PlannedActivity item,
) {
  controller.clearWriteError();
  return AppAdaptiveSheet.show<String>(
    context: context,
    builder: (_) => _AmountSheet(controller: controller, item: item),
  );
}

class _AmountSheet extends StatefulWidget {
  const _AmountSheet({required this.controller, required this.item});

  final TaxController controller;
  final PlannedActivity item;

  @override
  State<_AmountSheet> createState() => _AmountSheetState();
}

class _AmountSheetState extends State<_AmountSheet> {
  late final amount = TextEditingController(
    text: widget.item.amount == null ? '' : taxEditable(widget.item.amount!),
  );
  bool showErrors = false;

  @override
  void dispose() {
    amount.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final controller = widget.controller;
    final item = widget.item;
    return ListenableBuilder(
      listenable: controller,
      builder: (context, _) => TaxSheetBody(
        footer: _SheetSubmit(
          error: controller.writeError,
          child: AppSubmitButton(
            label: 'Kaydet',
            icon: Icons.check,
            isBusy: controller.isSubmitting,
            onSubmit: _submit,
          ),
        ),
        children: [
          TaxSheetTitle(
            title: item.title,
            subtitle:
                'Vade ${DateText.dayMonth(item.dueDate)} · '
                '${TaxSchedule.relative(item.dueDate, controller.today)}',
          ),
          const SizedBox(height: AppSpacing.large),
          TaxAmountInput(controller: amount, label: 'Tutar', autofocus: true),
          if (showErrors && MoneyMath.fromInput(amount.text) == null)
            _FieldError('Tutarı yazın.'),
          const TaxRule(
            'Yalnız bu kalemin tutarı; tanım değişmez.',
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: AppSpacing.medium),
        ],
      ),
    );
  }

  Future<void> _submit() async {
    final value = MoneyMath.fromInput(amount.text);
    if (value == null || MoneyMath.parse(value) == BigInt.zero) {
      setState(() => showErrors = true);
      return;
    }
    final saved = await widget.controller.setAmount(
      item: widget.item,
      amount: value,
    );
    if (saved && mounted) Navigator.of(context).pop(value);
  }
}

// ---------------------------------------------------------------------------
// V4 / V5 · Vergi ödemesi ekle
// ---------------------------------------------------------------------------

/// "Vergi ödemesi ekle": tek tutar, tanım gerekmez (ADR 0018 İ4). Bekleyen
/// kalem varsa hangilerini kapattığı sorulur; vadesi gelmiş ve geçmiş
/// olanlar seçili gelir.
Future<bool?> showTaxPaymentSheet(
  BuildContext context,
  TaxController controller,
) {
  controller.clearWriteError();
  return AppAdaptiveSheet.show<bool>(
    context: context,
    builder: (_) => _PaymentSheet(controller: controller),
  );
}

class _PaymentSheet extends StatefulWidget {
  const _PaymentSheet({required this.controller});

  final TaxController controller;

  @override
  State<_PaymentSheet> createState() => _PaymentSheetState();
}

class _PaymentSheetState extends State<_PaymentSheet> {
  /// Aynı panelden ikinci gönderim ikinci gider yazmasın: kimlik panel
  /// açıldığında bir kez üretilir.
  final requestId = newTaxRequestId();
  final amount = TextEditingController();
  final note = TextEditingController();
  late String paidOn = widget.controller.todayIso;
  String? sourceId;
  late String? categoryId = widget.controller.defaultTaxCategory?.id;
  late final Set<String> closes = {
    for (final item in pending)
      if (item.isDue) item.listKey,
  };
  bool showErrors = false;

  List<PlannedActivity> get pending =>
      widget.controller.overview?.pending ?? const [];

  @override
  void dispose() {
    amount.dispose();
    note.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final controller = widget.controller;
    final options = controller.options;
    final categories = options?.taxCategories ?? const <TaxChoice>[];
    final card = options?.isCard(sourceId) ?? false;
    return ListenableBuilder(
      listenable: controller,
      builder: (context, _) => TaxSheetBody(
        footer: _SheetSubmit(
          error: controller.writeError,
          child: AppSubmitButton(
            label: 'Ödemeyi kaydet',
            icon: Icons.check,
            isBusy: controller.isSubmitting,
            onSubmit: categories.isEmpty ? null : _submit,
          ),
        ),
        children: [
          const TaxSheetTitle(
            title: 'Vergi ödemesi ekle',
            subtitle: 'Tek tutar; tanım gerekmez',
          ),
          const SizedBox(height: AppSpacing.large),
          TaxAmountInput(controller: amount, label: 'Tutar', autofocus: true),
          if (showErrors && MoneyMath.fromInput(amount.text) == null)
            _FieldError('Ödediğiniz tutarı yazın.'),
          const SizedBox(height: AppSpacing.large),
          AppDateField(
            label: 'Ödeme günü',
            iconLeading: true,
            value: paidOn,
            valueText: taxDateText(paidOn, controller.today),
            lastDate: controller.today,
            onChanged: (value) => setState(() => paidOn = value),
          ),
          const SizedBox(height: AppSpacing.medium),
          if (options != null)
            TaxSourceField(
              options: options,
              value: sourceId,
              label: 'Nereden ödendi',
              errorText: showErrors && sourceId == null
                  ? 'Hesap ya da kart seçin.'
                  : null,
              helperText: card
                  ? 'Kart harcaması olarak yazılır; kart borcunuza eklenir.'
                  : null,
              onChanged: (value) => setState(() => sourceId = value),
            ),
          // Not yalnız bekleyen kalem yokken sorulur (tasarım V4). Bekleyen
          // varken ödemenin ne olduğunu kapattığı kalemler söyler (V5).
          if (pending.isEmpty) ...[
            const SizedBox(height: AppSpacing.medium),
            TextField(
              controller: note,
              maxLength: 200,
              decoration: const InputDecoration(
                labelText: 'Not (isteğe bağlı)',
                hintText: 'ör. Temmuz–Ağustos Bağkur',
                // Sınır yine geçerli; sayaç alanın altında yer tutmasın.
                counterText: '',
              ),
            ),
          ],
          if (categories.isEmpty)
            AppInlineNotice(
              margin: const EdgeInsets.only(top: AppSpacing.small),
              message: 'Vergi işaretli bir gider kategoriniz yok.',
              actionLabel: 'Kategorilere git',
              onAction: () {
                Navigator.of(context).pop();
                GoRouter.of(context).push('/more/categories');
              },
            )
          else if (categories.length > 1)
            Padding(
              padding: const EdgeInsets.only(top: AppSpacing.small),
              child: DropdownButtonFormField<String>(
                initialValue: categoryId,
                isExpanded: true,
                decoration: const InputDecoration(labelText: 'Kategori'),
                items: [
                  for (final category in categories)
                    DropdownMenuItem(
                      value: category.id,
                      child: Text(category.name),
                    ),
                ],
                onChanged: (value) => setState(() => categoryId = value),
              ),
            )
          else if (pending.isEmpty)
            Padding(
              padding: const EdgeInsets.only(top: AppSpacing.small),
              child: AppDetailBlock(
                rows: [
                  AppDetailRow(
                    label: 'Kategori',
                    value: categories.single.name,
                  ),
                ],
              ),
            ),
          if (pending.isNotEmpty) ...[
            const TaxFieldLabel(
              'Bu ödeme hangilerini kapatıyor?',
              hint:
                  'Tutar kalemlere dağıtılmaz; seçtikleriniz ödendi sayılır '
                  've bekleyenlerden düşer.',
            ),
            TaxBorderedList(
              children: [
                for (final item in pending)
                  TaxCheckRow(
                    selected: closes.contains(item.listKey),
                    title: item.title,
                    subtitle:
                        '${TaxSchedule.shortDate(item.dueDate)} · '
                        '${TaxSchedule.relative(item.dueDate, controller.today)}',
                    trailing: item.timing == PlannedTiming.overdue
                        ? const AppStatusTag(
                            label: 'Gecikti',
                            icon: Icons.error_outline,
                            tone: AppStatusTone.expense,
                          )
                        : null,
                    onChanged: (value) => setState(() {
                      if (value) {
                        closes.add(item.listKey);
                      } else {
                        closes.remove(item.listKey);
                      }
                    }),
                  ),
              ],
            ),
            const TaxRule('Hiçbirini seçmeden de kaydedebilirsiniz.'),
          ],
          const TaxRule(
            "Gider ödeme gününe yazılır ve Vergi takibi › Ödenenler'de "
            'görünür.',
            top: AppSpacing.medium,
          ),
        ],
      ),
    );
  }

  Future<void> _submit() async {
    final value = MoneyMath.fromInput(amount.text);
    if (value == null ||
        MoneyMath.parse(value) == BigInt.zero ||
        sourceId == null ||
        categoryId == null) {
      setState(() => showErrors = true);
      return;
    }
    final text = note.text.trim();
    final saved = await widget.controller.createPayment(
      clientRequestId: requestId,
      amount: value,
      paidOn: paidOn,
      sourceId: sourceId!,
      categoryId: categoryId!,
      note: text.isEmpty ? null : text,
      closes: [
        for (final item in pending)
          if (closes.contains(item.listKey)) item,
      ],
    );
    if (saved && mounted) Navigator.of(context).pop(true);
  }
}

// ---------------------------------------------------------------------------
// V8 · Bekleyen kalem ayrıntısı
// ---------------------------------------------------------------------------

/// Bekleyen kalemin ayrıntısı: tutar ya da "belli değil", durum, vade,
/// nereden ödenecek, tanıma giden kart; altta [Tutarı gir] [Ödedim].
///
/// "Tutarı gir" bu panelin **yerine** açılır, üstüne değil: iki panel aynı
/// yüzeydir ve üst üste durduklarında tutar panelinin inişi görünmez, panel
/// bir anda yok olmuş gibi kalır. Tutar paneli kapanınca (kaydedilsin ya da
/// vazgeçilsin) kullanıcı aynı kaleme, varsa yeni tutarla geri döner.
Future<void> showTaxPendingSheet(
  BuildContext context,
  TaxController controller,
  PlannedActivity item, {
  VoidCallback? onOpenPlan,
}) async {
  var current = item;
  while (true) {
    final shown = current;
    // İki panel sırayla oynar, üst üste değil: biri inerken öbürü (klavyesiyle
    // birlikte) çıkarsa iki animasyon aynı karelere biner ve açılış takılır.
    final pendingGone = Completer<void>();
    final wantsAmount = await AppAdaptiveSheet.show<bool>(
      context: context,
      builder: (_) => _GoneSignal(
        onGone: pendingGone.complete,
        child: _PendingSheet(
          controller: controller,
          item: shown,
          onOpenPlan: onOpenPlan,
        ),
      ),
    );
    if (wantsAmount != true || !context.mounted) return;
    await pendingGone.future;
    if (!context.mounted) return;

    controller.clearWriteError();
    final amountGone = Completer<void>();
    final saved = await AppAdaptiveSheet.show<String>(
      context: context,
      builder: (_) => _GoneSignal(
        onGone: amountGone.complete,
        child: _AmountSheet(controller: controller, item: shown),
      ),
    );
    if (!context.mounted) return;
    if (saved != null) current = current.withAmount(saved);
    await amountGone.future;
    if (!context.mounted) return;
    // Klavye inmeden açılan panel, alt boşluğu kare kare küçülürken çizilir.
    await _keyboardClosed(context);
    if (!context.mounted) return;
  }
}

/// Panelin kapanış animasyonu bitip ağaçtan çıktığını haber verir.
/// `showModalBottomSheet`'in sonucu `pop` anında döner; animasyon o sırada
/// daha başlamıştır.
class _GoneSignal extends StatefulWidget {
  const _GoneSignal({required this.onGone, required this.child});

  final VoidCallback onGone;
  final Widget child;

  @override
  State<_GoneSignal> createState() => _GoneSignalState();
}

class _GoneSignalState extends State<_GoneSignal> {
  @override
  void dispose() {
    widget.onGone();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => widget.child;
}

Future<void> _keyboardClosed(BuildContext context) async {
  for (var i = 0; i < 12; i++) {
    if (!context.mounted || View.of(context).viewInsets.bottom == 0) return;
    await Future<void>.delayed(const Duration(milliseconds: 40));
  }
}

class _PendingSheet extends StatelessWidget {
  const _PendingSheet({
    required this.controller,
    required this.item,
    this.onOpenPlan,
  });

  final TaxController controller;
  final PlannedActivity item;
  final VoidCallback? onOpenPlan;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final plan = controller.overview?.planOf(item.recurringTransactionId);
    final days = TaxSchedule.daysUntil(item.dueDate, controller.today);
    final tag = switch (item.timing) {
      PlannedTiming.overdue => AppStatusTag(
        label: '${-days} gün gecikti',
        icon: Icons.error_outline,
        tone: AppStatusTone.expense,
      ),
      PlannedTiming.today => const AppStatusTag(
        label: 'Bugün',
        icon: Icons.schedule,
        tone: AppStatusTone.planned,
      ),
      PlannedTiming.upcoming => AppStatusTag(
        label:
            'Yaklaşıyor · ${TaxSchedule.relative(item.dueDate, controller.today)}',
        icon: Icons.schedule,
        tone: AppStatusTone.planned,
      ),
    };
    return TaxSheetBody(
      children: [
        TaxSheetHead(
          icon: taxKindIconOf(item.taxKind),
          tone: AppStatusTone.expense,
          title: item.title,
          subtitle: taxKindLine(context, plan?.scope),
        ),
        const SizedBox(height: AppSpacing.large),
        Row(
          children: [
            Expanded(
              child: item.amount == null
                  ? Text(
                      'Tutar belli değil',
                      style: theme.textTheme.titleLarge?.copyWith(
                        fontSize: 22,
                        fontWeight: FontWeight.w700,
                        color: surfaces.inkMuted,
                      ),
                    )
                  : AppMoneyText(
                      amount: item.amount!,
                      currency: item.currency,
                      size: AppMoneySize.metric,
                      style: const TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
            ),
            const SizedBox(width: AppSpacing.small),
            tag,
          ],
        ),
        const SizedBox(height: AppSpacing.medium),
        AppDetailBlock(
          rows: [
            AppDetailRow(
              label: 'Vade',
              value: DateText.dayMonthWeekday(item.dueDate),
            ),
            AppDetailRow(
              label: 'Nereden ödenecek',
              value: item.sourceName ?? 'Seçilmedi',
            ),
          ],
        ),
        if (plan != null && onOpenPlan != null)
          TaxActionCard(
            children: [
              TaxActionRow(
                icon: Icons.event_repeat_outlined,
                title: '${plan.name} ayrıntıları',
                subtitle: TaxSchedule.planRhythm(plan),
                chevron: true,
                onTap: () {
                  Navigator.of(context).pop();
                  onOpenPlan!();
                },
              ),
            ],
          ),
        TaxSheetFooter(
          children: [
            OutlinedButton.icon(
              style: OutlinedButton.styleFrom(
                minimumSize: const Size.fromHeight(48),
              ),
              onPressed: () => Navigator.of(context).pop(true),
              icon: const Icon(Icons.edit_outlined),
              label: const Text('Tutarı gir'),
            ),
            FilledButton.icon(
              style: FilledButton.styleFrom(
                minimumSize: const Size.fromHeight(48),
              ),
              onPressed: () async {
                final saved = await showTaxPaySheet(context, controller, item);
                if (saved == true && context.mounted) {
                  Navigator.of(context).pop();
                }
              },
              icon: const Icon(Icons.check),
              label: const Text('Ödedim'),
            ),
          ],
        ),
      ],
    );
  }
}

// ---------------------------------------------------------------------------
// V9 · Ödenmiş kalem / toplu ödeme
// ---------------------------------------------------------------------------

/// Ödemenin ayrıntısı: tek kalem ya da toplu ödeme; altta "Ödemeyi geri al".
Future<void> showTaxPaidSheet(
  BuildContext context,
  TaxController controller,
  TaxPayment payment,
) => AppAdaptiveSheet.show<void>(
  context: context,
  builder: (_) => _PaidSheet(controller: controller, payment: payment),
);

class _PaidSheet extends StatelessWidget {
  const _PaidSheet({required this.controller, required this.payment});

  final TaxController controller;
  final TaxPayment payment;

  @override
  Widget build(BuildContext context) {
    final bulk = payment.isBulk;
    final realized = payment.realizedItem;
    final source = payment.isCard
        ? '${payment.sourceName} · kart harcaması'
        : payment.sourceName;
    return TaxSheetBody(
      children: [
        TaxSheetHead(
          icon: realized == null
              ? Icons.receipt_long_outlined
              : taxKindIcon(realized.taxKind ?? TaxKind.custom),
          tone: AppStatusTone.expense,
          title: payment.title,
          subtitle: taxKindLine(
            context,
            payment.scope,
            prefix: bulk ? 'Toplu ödeme' : 'Vergi',
          ),
        ),
        const SizedBox(height: AppSpacing.large),
        Row(
          children: [
            Expanded(
              child: AppMoneyText(
                amount: payment.amount,
                currency: payment.currency,
                effect: AppMoneyEffect.expense,
                signed: true,
                size: AppMoneySize.metric,
                style: const TextStyle(
                  fontSize: 28,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            const AppStatusTag(
              label: 'Ödendi',
              icon: Icons.check_circle_outline,
              tone: AppStatusTone.income,
            ),
          ],
        ),
        const SizedBox(height: AppSpacing.medium),
        AppDetailBlock(
          rows: [
            AppDetailRow(
              label: 'Ödeme günü',
              value: DateText.dayMonthWeekday(payment.paidOn),
            ),
            AppDetailRow(label: 'Nereden', value: source),
            if (realized != null)
              AppDetailRow(
                label: 'Vade',
                value: DateText.dayMonth(realized.scheduledDate),
              ),
            // Not, başlıkta zaten yazıyorsa ikinci kez gösterilmez.
            if (payment.note != null && payment.note != payment.title)
              AppDetailRow(label: 'Not', value: payment.note!),
          ],
        ),
        if (bulk) ...[
          const TaxFieldLabel('Kapattığı kalemler'),
          TaxBorderedList(
            inset: 16,
            children: [
              for (final item in payment.closedItems)
                AppRow(
                  title: item.label,
                  subtitle:
                      'Vade ${DateText.dayMonth(item.scheduledDate)} · '
                      'Kapatıldı',
                  trailing: Icon(
                    Icons.task_alt,
                    size: 18,
                    color: AppFinanceColors.of(context).neutral,
                  ),
                ),
            ],
          ),
        ],
        TaxActionCard(
          children: [
            TaxActionRow(
              icon: Icons.swap_vert,
              title: 'İşlemlerde görüntüle',
              subtitle:
                  '${payment.sourceName} '
                  '${payment.isCard ? 'kartla' : 'hesabından'} ödendi · '
                  '${DateText.dayMonth(payment.paidOn)}',
              chevron: true,
              onTap: () {
                Navigator.of(context).pop();
                GoRouter.of(context).go('/transactions');
              },
            ),
          ],
        ),
        TaxSheetFooter(
          children: [
            OutlinedButton.icon(
              style: taxDangerOutlineStyle(context),
              onPressed: () => _confirmUndo(context),
              icon: const Icon(Icons.undo),
              label: const Text('Ödemeyi geri al'),
            ),
          ],
        ),
      ],
    );
  }

  Future<void> _confirmUndo(BuildContext context) async {
    final message = payment.isBulk
        ? 'Gider iptal edilir; kapattığı ${payment.closedItems.length} '
              'kalem yeniden bekler.'
        : payment.realizedItem != null
        ? 'Gider iptal edilir; kalem yeniden bekleyene döner.'
        : 'Gider iptal edilir ve toplamları artık etkilemez.';
    final confirmed = await AppConfirmDialog.show(
      context: context,
      destructive: true,
      subject: AppConfirmSubject(
        title: payment.title,
        detail: '${DateText.dayMonth(payment.paidOn)} · ${payment.sourceName}',
        amount: MoneyText.format(payment.amount, payment.currency),
      ),
      message: message,
      confirmLabel: 'Ödemeyi geri al',
    );
    if (!confirmed || !context.mounted) return;
    final undone = await controller.undoPayment(payment);
    if (!context.mounted) return;
    if (undone) {
      Navigator.of(context).pop();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(controller.writeError ?? 'Geri alınamadı.')),
      );
    }
  }
}

/// Toplu ödemeyle kapatılmış kalem: kendi sonucu yoktur; geri alma kapatan
/// ödemeden yapılır.
Future<void> showTaxClosedItemSheet(
  BuildContext context,
  TaxController controller, {
  required TaxPlan plan,
  required TaxHistoryItem item,
}) => AppAdaptiveSheet.show<void>(
  context: context,
  builder: (_) =>
      _ClosedItemSheet(controller: controller, plan: plan, item: item),
);

class _ClosedItemSheet extends StatelessWidget {
  const _ClosedItemSheet({
    required this.controller,
    required this.plan,
    required this.item,
  });

  final TaxController controller;
  final TaxPlan plan;
  final TaxHistoryItem item;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final amount = item.amount ?? plan.amount;
    return TaxSheetBody(
      children: [
        TaxSheetHead(
          icon: taxKindIcon(plan.taxKind),
          tone: AppStatusTone.neutral,
          title: plan.name,
          subtitle: taxKindLine(context, plan.scope),
        ),
        const SizedBox(height: AppSpacing.large),
        Row(
          children: [
            Expanded(
              child: amount == null
                  ? Text(
                      'Tutar belli değil',
                      style: theme.textTheme.titleLarge?.copyWith(
                        fontSize: 22,
                        fontWeight: FontWeight.w700,
                        color: surfaces.inkMuted,
                      ),
                    )
                  : AppMoneyText(
                      amount: amount,
                      currency: plan.currency,
                      size: AppMoneySize.metric,
                      style: const TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
            ),
            const SizedBox(width: AppSpacing.small),
            const AppStatusTag(
              label: 'Kapatıldı',
              icon: Icons.task_alt,
              tone: AppStatusTone.neutral,
            ),
          ],
        ),
        const SizedBox(height: AppSpacing.medium),
        AppDetailBlock(
          rows: [
            AppDetailRow(
              label: 'Vade',
              value: DateText.dayMonth(item.scheduledDate),
            ),
          ],
        ),
        TaxActionCard(
          children: [
            TaxActionRow(
              icon: Icons.receipt_long_outlined,
              title: 'Kapatan ödemeyi aç',
              subtitle:
                  '${TaxSchedule.shortDate(item.payment.paidOn)} · '
                  '${item.payment.title}',
              chevron: true,
              onTap: () {
                Navigator.of(context).pop();
                showTaxPaidSheet(context, controller, item.payment);
              },
            ),
          ],
        ),
        const TaxRule(
          'Geri alma kapatan ödemeden yapılır.',
          top: AppSpacing.medium,
        ),
      ],
    );
  }
}

/// Form panelinin sabit alt eylemi: varsa sunucu hatası, altında buton.
class _SheetSubmit extends StatelessWidget {
  const _SheetSubmit({required this.child, this.error});

  final Widget child;
  final String? error;

  @override
  Widget build(BuildContext context) => Column(
    mainAxisSize: MainAxisSize.min,
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      if (error != null) ...[
        _FieldError(error!),
        const SizedBox(height: AppSpacing.small),
      ],
      child,
    ],
  );
}

class _FieldError extends StatelessWidget {
  const _FieldError(this.message);

  final String message;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.only(top: AppSpacing.small),
      child: Semantics(
        liveRegion: true,
        child: Text(
          message,
          textAlign: TextAlign.center,
          style: theme.textTheme.bodySmall?.copyWith(
            color: theme.colorScheme.error,
          ),
        ),
      ),
    );
  }
}
