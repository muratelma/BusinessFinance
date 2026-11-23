import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_math.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_account_field.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_category_field.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../data/debt_book_models.dart';
import 'debt_book_parts.dart';
import 'debt_entry_sheet.dart';

/// Takvim önizlemesinin bir satırı; tutar ve gün sunucudandır.
class DebtPreviewInstallment {
  const DebtPreviewInstallment({
    required this.sequence,
    required this.dueDate,
    required this.amount,
  });

  final int sequence;
  final String dueDate;
  final String amount;
}

/// Yeni borç ya da alacak formu (`05-EKRAN-TASARIMI.md` §3.4). Tam sayfa,
/// alt çubuksuz. Ana alanlar her zaman görünür; isteğe bağlı üç blok
/// (`Peşin / avans`, `Taksitli`, `Uygulamadan önce oluştu`) kapalı satır olarak
/// durur ve açılınca genişler. Sunucuya bağlama Faz 7'dedir.
class DebtEntryFormView extends StatefulWidget {
  const DebtEntryFormView({
    required this.kind,
    required this.today,
    super.key,
    this.personName,
    this.personLocked = false,
    this.accountOptions = const [],
    this.categoryOptions = const [],
    this.initialCategoryId,
    this.initialAccountId,
    this.initialAmount,
    this.initialUpfront = false,
    this.initialUpfrontAmount,
    this.initialInstallments = false,
    this.initialInstallmentCount,
    this.initialCarriedOver = false,
    this.initialAdvanceSide = DebtSide.payable,
    this.preview = const [],
    this.previewTotalCount,
    this.previewLastDate,
    this.onBack,
    this.onPickPerson,
  });

  final DebtEntryKind kind;
  final DateTime today;
  final String? personName;

  /// Kişi sayfasından açıldı: kişi değiştirilemez.
  final bool personLocked;
  final List<AppAccountOption> accountOptions;
  final List<AppCategoryOption> categoryOptions;
  final String? initialCategoryId;
  final String? initialAccountId;
  final String? initialAmount;
  final bool initialUpfront;
  final String? initialUpfrontAmount;
  final bool initialInstallments;
  final int? initialInstallmentCount;
  final bool initialCarriedOver;

  /// Avansta tarafın ön değeri: `payable` aldım (kişiye borçlusunuz).
  final DebtSide initialAdvanceSide;

  /// Sunucunun takvim önizlemesi (ilk üç taksit) ve toplamı.
  final List<DebtPreviewInstallment> preview;
  final int? previewTotalCount;
  final String? previewLastDate;
  final VoidCallback? onBack;
  final VoidCallback? onPickPerson;

  @override
  State<DebtEntryFormView> createState() => _DebtEntryFormViewState();
}

class _DebtEntryFormViewState extends State<DebtEntryFormView> {
  late final TextEditingController _amount = TextEditingController(
    text: widget.initialAmount == null
        ? null
        : MoneyText.field(widget.initialAmount!),
  );
  late final TextEditingController _upfrontAmount = TextEditingController(
    text: widget.initialUpfrontAmount == null
        ? null
        : MoneyText.field(widget.initialUpfrontAmount!),
  );
  late final TextEditingController _count = TextEditingController(
    text: widget.initialInstallmentCount?.toString(),
  );
  final TextEditingController _description = TextEditingController();
  late final TextEditingController _installmentAmount = TextEditingController(
    text: widget.preview.isEmpty
        ? null
        : MoneyText.field(widget.preview.first.amount),
  );
  late String _date = _iso(widget.today);
  String? _due;
  String? _firstDue;
  late String? _account = widget.initialAccountId;
  late String? _upfrontAccount = widget.initialAccountId;
  late AppCategoryValue? _category = widget.initialCategoryId == null
      ? null
      : AppCategoryValue(widget.initialCategoryId!);
  late bool _upfront = widget.initialUpfront;
  late bool _installments = widget.initialInstallments;
  late bool _carriedOver = widget.initialCarriedOver;
  late DebtSide _advanceSide = widget.initialAdvanceSide;

  static String _iso(DateTime day) =>
      '${day.year.toString().padLeft(4, '0')}-'
      '${day.month.toString().padLeft(2, '0')}-'
      '${day.day.toString().padLeft(2, '0')}';

  @override
  void dispose() {
    _amount.dispose();
    _upfrontAmount.dispose();
    _count.dispose();
    _description.dispose();
    _installmentAmount.dispose();
    super.dispose();
  }

  DebtEntryKind get _kind => widget.kind;
  bool get _isTrade =>
      _kind == DebtEntryKind.sale || _kind == DebtEntryKind.purchase;
  bool get _isAdvance => _kind == DebtEntryKind.advance;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final fields = <Widget>[
      if (_isAdvance)
        AppFormField(
          child: AppSegmentRail<DebtSide>(
            values: const [DebtSide.payable, DebtSide.receivable],
            selected: _advanceSide,
            onChanged: (side) => setState(() => _advanceSide = side),
            semanticLabel: 'Avansın yönü',
            segmentBuilder: (context, side, selected) => Text(
              side == DebtSide.payable ? 'Aldım' : 'Verdim',
              style: theme.textTheme.labelLarge?.copyWith(
                fontWeight: selected ? FontWeight.w700 : FontWeight.w500,
                letterSpacing: 0,
              ),
            ),
          ),
        ),
      AppFormField(
        child: _PersonField(
          name: widget.personName,
          locked: widget.personLocked,
          onTap: widget.onPickPerson,
        ),
      ),
      AppFormField(
        child: TextFormField(
          controller: _amount,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          inputFormatters: const [TurkishAmountInputFormatter()],
          decoration: InputDecoration(
            labelText: _carriedOver ? 'Kalan tutar' : 'Tutar',
            prefixText: '₺',
          ),
        ),
      ),
      AppFormField(
        child: AppDateField(
          label: _carriedOver ? 'İtibarıyla' : 'Gün',
          value: _date,
          lastDate: widget.today,
          onChanged: (value) => setState(() => _date = value),
        ),
      ),
      if (_isTrade && !_carriedOver)
        AppFormField(
          child: AppCategoryField(
            options: widget.categoryOptions,
            value: _category,
            onChanged: (value) => setState(() => _category = value),
          ),
        ),
      if (!_isTrade && !_carriedOver)
        AppFormField(
          child: AppAccountField(
            label: _accountLabel,
            options: widget.accountOptions,
            value: _account,
            onChanged: (value) => setState(() => _account = value),
          ),
        ),
      if (!_installments && !_isAdvance)
        AppFormField(
          child: AppDateField(
            label: 'Vade (isteğe bağlı)',
            value: _due,
            firstDate: widget.today,
            onChanged: (value) => setState(() => _due = value),
          ),
        ),
      AppFormField(
        child: TextFormField(
          controller: _description,
          decoration: const InputDecoration(
            labelText: 'Açıklama (isteğe bağlı)',
          ),
        ),
      ),
      if (!_isAdvance) ...[
        const SizedBox(height: AppSpacing.xSmall),
        AppCard(
          padding: EdgeInsets.zero,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: _optionBlocks(context),
          ),
        ),
      ],
      const SizedBox(height: AppGaps.section),
      FilledButton(onPressed: () {}, child: const Text('Kaydet')),
    ];
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppPageHeader(title: _kind.formTitle, onBack: widget.onBack),
        Expanded(
          child: ListView(padding: AppPageInsets.form, children: fields),
        ),
      ],
    );
  }

  String get _accountLabel => switch (_kind) {
    DebtEntryKind.lend => 'Çıkan hesap',
    DebtEntryKind.borrow => 'Giren hesap',
    _ => _advanceSide == DebtSide.payable ? 'Giren hesap' : 'Çıkan hesap',
  };

  List<Widget> _optionBlocks(BuildContext context) {
    final blocks = <Widget>[
      if (_isTrade && !_carriedOver)
        _OptionBlock(
          title: _kind == DebtEntryKind.sale
              ? 'Peşin / avans alındı'
              : 'Peşin ödendi',
          hint: _kind == DebtEntryKind.sale
              ? 'Bugün alınan kısım'
              : 'Bugün ödenen kısım',
          value: _upfront,
          onChanged: (value) => setState(() => _upfront = value),
          children: [
            AppFormField(
              child: TextFormField(
                controller: _upfrontAmount,
                keyboardType: const TextInputType.numberWithOptions(
                  decimal: true,
                ),
                inputFormatters: const [TurkishAmountInputFormatter()],
                decoration: InputDecoration(
                  labelText: _kind == DebtEntryKind.sale
                      ? 'Alınan tutar'
                      : 'Ödenen tutar',
                  prefixText: '₺',
                ),
              ),
            ),
            AppAccountField(
              label: _kind == DebtEntryKind.sale
                  ? 'Giren hesap'
                  : 'Çıkan hesap',
              options: widget.accountOptions,
              value: _upfrontAccount,
              onChanged: (value) => setState(() => _upfrontAccount = value),
            ),
            if (_kind == DebtEntryKind.sale) ...[
              const SizedBox(height: AppGaps.notice),
              const DebtFieldNote(
                'Satış bugünün geliri olur. İş bitince yazmak isterseniz '
                'yalnız avansı kaydedin.',
              ),
            ],
          ],
        ),
      _OptionBlock(
        title: 'Taksitli',
        hint: 'Kendi takvimiyle takip edilir',
        value: _installments,
        onChanged: (value) => setState(() => _installments = value),
        children: [
          AppFormField(
            child: TextFormField(
              controller: _count,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(labelText: 'Taksit sayısı'),
            ),
          ),
          // K17: kredide uygulama faiz hesaplamaz; bankanın taksit tutarı
          // yazılır.
          if (!_isTrade)
            AppFormField(
              child: TextFormField(
                controller: _installmentAmount,
                keyboardType: const TextInputType.numberWithOptions(
                  decimal: true,
                ),
                inputFormatters: const [TurkishAmountInputFormatter()],
                decoration: const InputDecoration(
                  labelText: 'Taksit tutarı',
                  prefixText: '₺',
                ),
              ),
            ),
          AppDateField(
            label: 'İlk taksit günü',
            value: _firstDue ?? widget.preview.firstOrNull?.dueDate,
            onChanged: (value) => setState(() => _firstDue = value),
          ),
          if (widget.preview.isNotEmpty) ...[
            const SizedBox(height: AppGaps.field),
            _SchedulePreview(
              items: widget.preview,
              totalCount: widget.previewTotalCount,
              lastDate: widget.previewLastDate,
            ),
          ],
        ],
      ),
      _OptionBlock(
        title: 'Uygulamadan önce oluştu',
        hint: 'Yalnız kalan tutar yazılır',
        value: _carriedOver,
        onChanged: (value) => setState(() => _carriedOver = value),
        children: const [DebtFieldNote('Hesap bakiyesi ve raporlar değişmez.')],
      ),
    ];
    return [
      for (final (index, block) in blocks.indexed) ...[
        if (index > 0) const Divider(height: 1, indent: AppSpacing.medium),
        block,
      ],
    ];
  }
}

/// Kişi alanı: baş harf dairesi ve ad; kişi sayfasından açılınca kilitli.
/// Seçici (arama, `"…" kişisini ekle`, yeni kişinin türü) Faz 7'de bağlanır.
class _PersonField extends StatelessWidget {
  const _PersonField({required this.name, required this.locked, this.onTap});

  final String? name;
  final bool locked;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final value = name;
    return Semantics(
      button: !locked,
      label: value == null ? 'Kişi, seçilmedi' : 'Kişi, $value',
      excludeSemantics: true,
      child: InkWell(
        onTap: locked ? null : onTap,
        borderRadius: BorderRadius.circular(AppRadius.field),
        child: InputDecorator(
          isEmpty: value == null,
          decoration: InputDecoration(
            labelText: 'Kişi',
            enabled: !locked,
            floatingLabelBehavior: FloatingLabelBehavior.always,
            suffixIcon: locked
                ? null
                : Icon(Icons.chevron_right, color: surfaces.inkMuted),
          ),
          child: value == null
              ? Text(
                  'Kişi seçin ya da ekleyin',
                  style: theme.textTheme.bodyLarge?.copyWith(
                    color: surfaces.inkFaint,
                  ),
                )
              : Row(
                  children: [
                    PersonInitials(name: value),
                    const SizedBox(width: AppSpacing.compact),
                    Expanded(
                      child: Text(
                        value,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.bodyLarge?.copyWith(
                          color: surfaces.ink,
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

/// Kapalı satır: başlık, soluk ipucu, anahtar; açılınca altındaki alanlar
/// kartın içinde görünür.
class _OptionBlock extends StatelessWidget {
  const _OptionBlock({
    required this.title,
    required this.hint,
    required this.value,
    required this.onChanged,
    required this.children,
  });

  final String title;
  final String hint;
  final bool value;
  final ValueChanged<bool> onChanged;
  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        MergeSemantics(
          child: InkWell(
            onTap: () => onChanged(!value),
            child: ConstrainedBox(
              constraints: const BoxConstraints(minHeight: 56),
              child: Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.medium,
                  AppSpacing.small,
                  AppSpacing.compact,
                  AppSpacing.small,
                ),
                child: Row(
                  children: [
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text(title, style: theme.textTheme.titleSmall),
                          const SizedBox(height: AppGaps.rowText),
                          Text(
                            hint,
                            style: theme.textTheme.bodySmall?.copyWith(
                              color: surfaces.inkMuted,
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(width: AppSpacing.compact),
                    Switch(value: value, onChanged: onChanged),
                  ],
                ),
              ),
            ),
          ),
        ),
        if (value)
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.small,
              AppSpacing.medium,
              AppSpacing.medium,
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: children,
            ),
          ),
      ],
    );
  }
}

/// Sunucunun takvim önizlemesi: ilk üç taksit ve toplam.
class _SchedulePreview extends StatelessWidget {
  const _SchedulePreview({
    required this.items,
    required this.totalCount,
    required this.lastDate,
  });

  final List<DebtPreviewInstallment> items;
  final int? totalCount;
  final String? lastDate;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final small = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final total = totalCount;
    final last = lastDate;
    return Container(
      padding: const EdgeInsets.all(AppSpacing.compact),
      decoration: BoxDecoration(
        color: surfaces.cardMuted,
        borderRadius: BorderRadius.circular(AppRadius.inner),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text('Takvim', style: small?.copyWith(fontWeight: FontWeight.w600)),
          const SizedBox(height: AppSpacing.small),
          for (final item in items.take(3))
            Padding(
              padding: const EdgeInsets.only(bottom: AppSpacing.xSmall),
              child: Row(
                children: [
                  Expanded(
                    child: Text(
                      '${item.sequence}. taksit · '
                      '${DateText.dayMonthYear(item.dueDate)}',
                      style: theme.textTheme.bodyMedium,
                    ),
                  ),
                  Text(
                    MoneyText.format(item.amount, 'TRY'),
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                      fontFeatures: AppTypography.tabularFigures,
                    ),
                  ),
                ],
              ),
            ),
          if (total != null && last != null)
            Text(
              '$total taksit · son taksit ${DateText.dayMonthYear(last)}',
              style: small,
            ),
        ],
      ),
    );
  }
}
