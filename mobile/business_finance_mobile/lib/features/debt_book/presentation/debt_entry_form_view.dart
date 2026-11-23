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
import '../../../core/models/transaction_scope.dart';
import '../../../core/network/client_request_id.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_error.dart';
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

/// Formun gönderdiği kayıt; hangi uca gideceğini sayfa bilir (K1: kaydın
/// nereye yazılacağını uygulama bilir).
class DebtEntryDraft {
  const DebtEntryDraft({
    required this.kind,
    required this.side,
    required this.amount,
    required this.date,
    required this.clientRequestId,
    this.person,
    this.categoryId,
    this.scope,
    this.accountId,
    this.dueDate,
    this.description,
    this.upfrontAmount,
    this.upfrontAccountId,
    this.installmentCount,
    this.firstDueDate,
    this.installmentAmount,
    this.carriedOver = false,
  });

  final DebtEntryKind kind;

  /// Kaydın açtığı taraf (avansta seçilen yön).
  final DebtSide side;
  final DebtPersonChoice? person;
  final String amount;
  final String date;
  final String? categoryId;
  final TransactionScope? scope;
  final String? accountId;
  final String? dueDate;
  final String? description;
  final String? upfrontAmount;
  final String? upfrontAccountId;

  /// Doluysa taksitli kayıt.
  final int? installmentCount;
  final String? firstDueDate;
  final String? installmentAmount;
  final bool carriedOver;
  final String clientRequestId;

  bool get isInstallments => installmentCount != null;
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
    this.initialPerson,
    this.onPreview,
    this.onSubmit,
  });

  /// Kişi sayfasından açıldığında kişi (kimliğiyle).
  final DebtPersonChoice? initialPerson;

  /// Sunucunun takvim önizlemesi (taksit sayısı, ilk vade, tutar ya da taksit
  /// tutarı); okunamazsa `null`.
  final Future<DebtSchedulePreview?> Function(Map<String, Object?> query)?
  onPreview;

  /// Yazıldıysa `null`, reddedildiyse formun içinde gösterilecek cümle.
  final Future<String?> Function(DebtEntryDraft draft)? onSubmit;

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
  final Future<DebtPersonChoice?> Function()? onPickPerson;

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
  late String? _firstDue = widget.preview.firstOrNull?.dueDate;
  late String? _account = widget.initialAccountId;
  late String? _upfrontAccount = widget.initialAccountId;
  late AppCategoryValue? _category = widget.initialCategoryId == null
      ? null
      : AppCategoryValue(widget.initialCategoryId!);
  late bool _upfront = widget.initialUpfront;
  late bool _installments = widget.initialInstallments;
  late bool _carriedOver = widget.initialCarriedOver;
  late DebtSide _advanceSide = widget.initialAdvanceSide;
  late DebtPersonChoice? _person =
      widget.initialPerson ??
      (widget.personName == null
          ? null
          : DebtPersonChoice(name: widget.personName!));
  late List<DebtPreviewInstallment> _preview = widget.preview;
  late int? _previewCount = widget.previewTotalCount;
  late String? _previewLast = widget.previewLastDate;
  int _previewGeneration = 0;
  final String _requestId = newClientRequestId();
  bool _saving = false;
  String? _error;

  Future<void> _pickPerson() async {
    final picked = await widget.onPickPerson?.call();
    if (picked != null && mounted) setState(() => _person = picked);
  }

  /// Takvim önizlemesi sunucudan; alanlar değiştikçe yeniden istenir ve
  /// yalnız son isteğin cevabı yazılır.
  Future<void> _refreshPreview() async {
    final load = widget.onPreview;
    final count = int.tryParse(_count.text.trim());
    final first = _firstDue;
    final amount = MoneyText.normalizeInput(_amount.text);
    final perInstallment = _isTrade
        ? null
        : MoneyText.normalizeInput(_installmentAmount.text);
    if (load == null ||
        !_installments ||
        count == null ||
        count < 1 ||
        first == null ||
        (amount == null && perInstallment == null)) {
      if (_preview.isNotEmpty) setState(() => _preview = const []);
      return;
    }
    final generation = ++_previewGeneration;
    final result = await load({
      'firstDueDate': first,
      'installmentCount': count,
      if (perInstallment != null)
        'installmentAmount': perInstallment
      else
        'amount': amount,
    });
    if (!mounted || generation != _previewGeneration) return;
    setState(() {
      _preview = [
        for (final item
            in result?.items ??
                const <({int sequence, String dueDate, String amount})>[])
          DebtPreviewInstallment(
            sequence: item.sequence,
            dueDate: item.dueDate,
            amount: item.amount,
          ),
      ];
      _previewCount = result?.items.length;
      _previewLast = result?.items.lastOrNull?.dueDate;
    });
  }

  /// Kişi zorunlu mu: kişisiz kayıt yalnız taksitli ya da vadeli alımda
  /// (fatura) olur (`02-TASARIM.md` §5).
  bool get _personRequired => !_installments && _kind != DebtEntryKind.purchase;

  String? _validate() {
    final amount = MoneyText.normalizeInput(_amount.text);
    if (amount == null || !isPositiveAmount(amount)) return 'Tutarı yazın.';
    if (_person == null && _personRequired) return 'Kişiyi seçin.';
    if (_isTrade && !_carriedOver && _category == null) {
      return 'Kategoriyi seçin.';
    }
    if (!_isTrade && !_carriedOver && _account == null) {
      return 'Hesabı seçin.';
    }
    if (_installments) {
      final count = int.tryParse(_count.text.trim());
      if (count == null || count < 1) return 'Taksit sayısını yazın.';
      if (_firstDue == null) return 'İlk taksit gününü seçin.';
    }
    if (_upfront && _isTrade && !_carriedOver) {
      final upfront = MoneyText.normalizeInput(_upfrontAmount.text);
      if (upfront == null || !isPositiveAmount(upfront)) {
        return 'Peşin tutarı yazın.';
      }
      if (_upfrontAccount == null) return 'Peşin tutarın hesabını seçin.';
    }
    if (_person == null && _description.text.trim().isEmpty) {
      return 'Kişisiz kayıtta açıklama yazın.';
    }
    return null;
  }

  Future<void> _submit() async {
    final submit = widget.onSubmit;
    if (submit == null || _saving) return;
    final invalid = _validate();
    if (invalid != null) {
      setState(() => _error = invalid);
      return;
    }
    final side = _kind.side ?? _advanceSide;
    final trade = _isTrade && !_carriedOver;
    final draft = DebtEntryDraft(
      kind: _kind,
      side: side,
      person: _person,
      amount: MoneyText.normalizeInput(_amount.text)!,
      date: _date,
      categoryId: trade ? _category?.categoryId : null,
      scope: trade ? _category?.scope : null,
      accountId: !_isTrade && !_carriedOver ? _account : null,
      dueDate: _installments || _isAdvance ? null : _due,
      description: _description.text.trim().isEmpty
          ? null
          : _description.text.trim(),
      upfrontAmount: _upfront && trade
          ? MoneyText.normalizeInput(_upfrontAmount.text)
          : null,
      upfrontAccountId: _upfront && trade ? _upfrontAccount : null,
      installmentCount: _installments ? int.parse(_count.text.trim()) : null,
      firstDueDate: _installments ? _firstDue : null,
      installmentAmount: _installments && !_isTrade
          ? MoneyText.normalizeInput(_installmentAmount.text)
          : null,
      carriedOver: _carriedOver,
      clientRequestId: _requestId,
    );
    setState(() {
      _saving = true;
      _error = null;
    });
    final error = await submit(draft);
    if (!mounted) return;
    setState(() {
      _saving = false;
      _error = error;
    });
  }

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
          name: _person?.name,
          locked: widget.personLocked,
          onTap: widget.onPickPerson == null ? null : _pickPerson,
          optional: !_personRequired,
        ),
      ),
      AppFormField(
        child: TextFormField(
          controller: _amount,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          inputFormatters: const [TurkishAmountInputFormatter()],
          onChanged: (_) => _refreshPreview(),
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
      if (_error != null) ...[
        AppFormError(message: _error!),
        const SizedBox(height: AppGaps.notice),
      ],
      FilledButton(
        onPressed: _saving ? null : _submit,
        child: Text(_saving ? 'Kaydediliyor' : 'Kaydet'),
      ),
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
        onChanged: (value) {
          setState(() => _installments = value);
          _refreshPreview();
        },
        children: [
          AppFormField(
            child: TextFormField(
              controller: _count,
              keyboardType: TextInputType.number,
              onChanged: (_) => _refreshPreview(),
              decoration: const InputDecoration(labelText: 'Taksit sayısı'),
            ),
          ),
          // K17: kredide uygulama faiz hesaplamaz; bankanın taksit tutarı
          // yazılır.
          if (!_isTrade)
            AppFormField(
              child: TextFormField(
                controller: _installmentAmount,
                onChanged: (_) => _refreshPreview(),
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
            onChanged: (value) {
              setState(() => _firstDue = value);
              _refreshPreview();
            },
          ),
          if (_preview.isNotEmpty) ...[
            const SizedBox(height: AppGaps.field),
            _SchedulePreview(
              items: _preview,
              totalCount: _previewCount,
              lastDate: _previewLast,
            ),
          ],
        ],
      ),
      _OptionBlock(
        title: 'Uygulamadan önce oluştu',
        hint: 'Yalnız kalan tutar yazılır',
        value: _carriedOver,
        onChanged: (value) => setState(() => _carriedOver = value),
        children: [
          DebtFieldNote(
            _person != null && !_installments
                ? 'Kişinin açılış bakiyesi bu tutar olur; hesap bakiyesi ve '
                      'raporlar değişmez.'
                : 'Hesap bakiyesi ve raporlar değişmez.',
          ),
        ],
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
  const _PersonField({
    required this.name,
    required this.locked,
    this.onTap,
    this.optional = false,
  });

  final String? name;
  final bool locked;
  final VoidCallback? onTap;

  /// Kişisiz kayıt olabilir (taksitli ya da fatura).
  final bool optional;

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
            labelText: optional ? 'Kişi (isteğe bağlı)' : 'Kişi',
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
