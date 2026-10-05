import 'dart:async';

import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_input.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/network/client_request_id.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_money_text.dart';
import '../data/day_close_repository.dart';
import 'day_close_controller.dart';

/// Gün sonu panelini açar (ADR 0019 T1–T2). Kaydedilince `true`, vazgeçilince
/// `null`.
///
/// [initialDate] verilmezse bugün seçili gelir.
Future<bool?> showDayCloseForm(
  BuildContext context,
  DayCloseController controller, {
  String? initialDate,
  bool additional = false,
}) => AppFormSheet.show<bool>(
  context: context,
  builder: (_) => _DayCloseForm(
    controller: controller,
    initialDate: initialDate ?? AppDateField.format(DateTime.now()),
    additional: additional,
  ),
);

class _DayCloseForm extends StatefulWidget {
  const _DayCloseForm({
    required this.controller,
    required this.initialDate,
    this.additional = false,
  });

  final DayCloseController controller;
  final String initialDate;

  /// Gün zaten kapalıyken Kasa'dan açıldı: "ek gün sonu" işaretli gelir.
  final bool additional;

  @override
  State<_DayCloseForm> createState() => _DayCloseFormState();
}

/// Gün sonu formu: nakit, her POS için kart ve toplam; o gün zaten girilmiş
/// kayıtlar ve yazılacakların özeti.
///
/// Hiçbir tutar burada hesaplanmaz: hesaplanan alan, düşülen kayıtlar,
/// komisyon ve yazılacak tutar **sunucunun önizlemesinden** gelir. Form
/// yalnız yazılanı gönderir ve cevabı gösterir.
class _DayCloseFormState extends State<_DayCloseForm> {
  /// Aynı panelden ikinci gönderim ikinci gün sonu yazmasın: kimlik panel
  /// açıldığında bir kez üretilir.
  final requestId = newClientRequestId();
  final formKey = GlobalKey<FormState>();
  final cashController = TextEditingController();
  final totalController = TextEditingController();
  final posControllers = <String, TextEditingController>{};

  late String date = widget.initialDate;
  late bool isAdditional = widget.additional;

  /// Kullanıcının varsayılandan farklı işaretledikleri.
  final overrides = <String, bool>{};

  String? cashAccountId;
  String? cashCategoryId;

  /// Kasa ve kategori seçimi yalnız istenince ya da gerekince açılır.
  bool showTargets = false;
  Future<DayCloseOptions>? options;

  DayClosePreview? preview;

  /// Kaydetmeye basıldı: eksik tutar artık söylenir.
  bool submitted = false;

  /// Ana POS'un alanı hep görünür; diğer POS'lar istenince açılır. Çoğu akşam
  /// tek POS'a yazılır ve üç dört boş alan paneli boşuna uzatır.
  bool showOtherPos = false;
  Timer? _previewTimer;
  int _previewRequest = 0;

  DayCloseController get controller => widget.controller;

  @override
  void initState() {
    super.initState();
    controller
      ..errorMessage = null
      ..errorCode = null
      ..addListener(_changed);
    _loadPreview();
  }

  @override
  void dispose() {
    _previewTimer?.cancel();
    controller.removeListener(_changed);
    cashController.dispose();
    totalController.dispose();
    for (final item in posControllers.values) {
      item.dispose();
    }
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  /// Boş alan gönderilmez; sunucu onu hesaplar.
  String? _wire(TextEditingController field) {
    final text = field.text.trim();
    if (text.isEmpty || MoneyInput.parse(text) == null) return null;
    return MoneyInput.wire(text);
  }

  DayCloseInput _input() => DayCloseInput(
    date: date,
    cashAmount: _wire(cashController),
    posAmounts: {
      for (final entry in posControllers.entries)
        entry.key: ?_wire(entry.value),
    },
    totalAmount: _wire(totalController),
    cashAccountId: cashAccountId,
    cashCategoryId: cashCategoryId,
    recordOverrides: Map.of(overrides),
    isAdditional: isAdditional,
  );

  /// Tutar değişince önizleme kısa bir duraklamadan sonra istenir; her tuşta
  /// istek atılmaz.
  void _amountChanged(String _) {
    _previewTimer?.cancel();
    _previewTimer = Timer(const Duration(milliseconds: 350), _loadPreview);
  }

  Future<void> _loadPreview() async {
    final request = ++_previewRequest;
    final result = await controller.preview(_input());
    // Sonradan değişen girdinin cevabı eskisinin üstüne yazılmasın.
    if (!mounted || request != _previewRequest) return;
    setState(() {
      if (result == null) return;
      preview = result;
      for (final line in result.posLines) {
        posControllers.putIfAbsent(
          line.posDefinitionId,
          TextEditingController.new,
        );
      }
      if (result.blockerCode == 'day_closes.cash_account_required' ||
          result.blockerCode == 'day_closes.cash_category_required') {
        showTargets = true;
      }
    });
  }

  void _toggleRecord(DayCloseExistingRecord record, bool value) {
    setState(() {
      if (value == record.includedByDefault) {
        overrides.remove(record.key);
      } else {
        overrides[record.key] = value;
      }
    });
    _loadPreview();
  }

  void _dateChanged(String value) {
    setState(() {
      date = value;
      // Başka günün kayıtları başka kayıtlardır.
      overrides.clear();
      isAdditional = false;
    });
    _loadPreview();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: formKey,
    child: AppFormSheet<bool>(
      title: 'Gün sonu',
      description:
          'Günün satış tutarlarını yazın. Tek tek girdikleriniz '
          'düşülür; aynı satış iki kez yazılmaz.',
      submitLabel: 'Gün sonunu kaydet',
      onSubmit: _submit,
      children: _fields(context),
    ),
  );

  List<Widget> _fields(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final muted = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final shown = preview;
    final blocker = shown?.blockerCode;
    final error = controller.errorMessage ?? controller.previewError;
    final notice = _notice(blocker);
    final busy = controller.isSubmitting;
    final closedAndNotAdditional = (shown?.isClosed ?? false) && !isAdditional;

    return [
      if (error != null)
        AppInlineNotice(
          message: error,
          icon: Icons.error_outline,
          margin: const EdgeInsets.only(bottom: AppSpacing.small),
        ),
      AppDateField(
        label: 'Gün',
        value: date,
        lastDate: DateTime.now(),
        onChanged: _dateChanged,
      ),
      if (shown != null && shown.isClosed) ...[
        AppInlineNotice(
          message:
              'Bu günün gün sonu girildi. İkinci bir cihazın gün sonuysa '
              'ek olarak kaydedin.',
          icon: Icons.check_circle_outline,
          margin: const EdgeInsets.only(top: AppSpacing.small),
        ),
        _CheckRow(
          key: const ValueKey('day-close-additional'),
          title: 'Ek gün sonu',
          subtitle: 'İkinci cihazın satışları',
          value: isAdditional,
          enabled: !busy,
          onChanged: (value) {
            setState(() => isAdditional = value);
            _loadPreview();
          },
        ),
      ],
      const SizedBox(height: AppSpacing.medium),
      TextFormField(
        key: const ValueKey('day-close-cash'),
        controller: cashController,
        enabled: !closedAndNotAdditional,
        keyboardType: const TextInputType.numberWithOptions(decimal: true),
        decoration: InputDecoration(
          labelText: 'Nakit',
          helperText: _computedHelper(
            shown?.cash.isComputed ?? false,
            shown,
            shown?.cash.enteredAmount,
          ),
          errorMaxLines: 3,
          errorText: blocker == 'day_closes.existing_exceeds_cash'
              ? 'İşaretli kayıtlar bu tutarı aşıyor. Tutarı düzeltin ya da '
                    'işareti kaldırın.'
              : null,
        ),
        onChanged: _amountChanged,
        validator: _optionalMoneyError,
      ),
      for (final line in _visiblePosLines(shown)) ...[
        const SizedBox(height: AppSpacing.medium),
        TextFormField(
          key: ValueKey('day-close-pos-${line.posDefinitionId}'),
          controller: posControllers[line.posDefinitionId],
          enabled: !closedAndNotAdditional,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          decoration: InputDecoration(
            labelText: line.name,
            helperText: _computedHelper(
              line.isComputed,
              shown,
              line.enteredAmount,
            ),
          ),
          onChanged: _amountChanged,
          validator: _optionalMoneyError,
        ),
      ],
      if (_hiddenPosCount(shown) > 0)
        Align(
          alignment: AlignmentDirectional.centerStart,
          child: TextButton(
            key: const ValueKey('day-close-other-pos'),
            onPressed: closedAndNotAdditional
                ? null
                : () => setState(() => showOtherPos = true),
            child: Text("Diğer POS'lar (${_hiddenPosCount(shown)})"),
          ),
        ),
      const SizedBox(height: AppSpacing.medium),
      TextFormField(
        key: const ValueKey('day-close-total'),
        controller: totalController,
        enabled: !closedAndNotAdditional,
        keyboardType: const TextInputType.numberWithOptions(decimal: true),
        decoration: InputDecoration(
          labelText: 'Toplam',
          helperText: 'İkisini yazmak yeter; üçüncüsü hesaplanır.',
          helperMaxLines: 2,
          errorMaxLines: 3,
          errorText: switch (blocker) {
            'day_closes.total_below_parts' =>
              'Toplam, yazdığınız tutardan küçük olamaz.',
            'day_closes.pos_required' =>
              'Kart tutarı için önce bir POS ekleyin.',
            _ => null,
          },
        ),
        onChanged: _amountChanged,
        validator: _optionalMoneyError,
      ),
      if (shown?.totalDifference != null)
        AppInlineNotice(
          message:
              'Nakit ve kart toplamı, yazdığınız toplamdan '
              '${MoneyText.format(MoneyText.unsigned(shown!.totalDifference!), shown.currency)} '
              'farklı. Genelde faturalı satış ya da veresiye tahsilatıdır; '
              'ayrıca kaydedilmez.',
          margin: const EdgeInsets.only(top: AppSpacing.small),
        ),
      // Gün kapalıyken yazılacak bir şey yok; liste "ek gün sonu" seçilince
      // gelir.
      if (shown != null &&
          shown.existingRecords.isNotEmpty &&
          !closedAndNotAdditional) ...[
        const SizedBox(height: AppSpacing.large),
        Text('Gün sonu tutarında var mı?', style: theme.textTheme.labelMedium),
        Text('İşaretli kayıtlar düşülür.', style: muted),
        for (final record in shown.existingRecords)
          _CheckRow(
            key: ValueKey('day-close-record-${record.key}'),
            title: _recordTitle(record),
            subtitle: _recordSubtitle(record, shown),
            amount: record.amount,
            currency: shown.currency,
            value: record.included,
            enabled: !busy,
            onChanged: (value) => _toggleRecord(record, value),
          ),
      ],
      if (notice != null)
        AppInlineNotice(
          message: notice,
          icon: Icons.error_outline,
          margin: const EdgeInsets.only(top: AppSpacing.small),
        ),
      if (shown != null && _hasSummary(shown)) ...[
        const SizedBox(height: AppSpacing.large),
        Text('Yazılacak', style: theme.textTheme.labelMedium),
        const SizedBox(height: AppSpacing.xSmall),
        _WriteSummary(preview: shown),
        if (shown.cash.writes && !showTargets)
          Align(
            alignment: AlignmentDirectional.centerStart,
            child: TextButton(
              onPressed: () => setState(() => showTargets = true),
              child: const Text('Kasayı ya da kategoriyi değiştir'),
            ),
          ),
      ],
      if (showTargets) ..._targetFields(shown),
    ];
  }

  /// Nakit satışın yazılacağı kasa ve kategori. Sunucu ikisini de seçili
  /// gönderir; burada yalnız değiştirilir ya da (seçemediyse) seçilir.
  List<Widget> _targetFields(DayClosePreview? shown) => [
    const SizedBox(height: AppSpacing.medium),
    FutureBuilder<DayCloseOptions>(
      future: options ??= controller.loadOptions(),
      builder: (context, snapshot) {
        if (snapshot.hasError) {
          return const Text(
            'Seçenekler yüklenemedi. Paneli kapatıp yeniden deneyin.',
          );
        }
        if (!snapshot.hasData) return const LinearProgressIndicator();
        final data = snapshot.data!;
        return Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            _choiceField(
              label: 'Kasa',
              fieldKey: 'day-close-account',
              choices: data.cashAccounts,
              selected: cashAccountId ?? shown?.cash.accountId,
              missing: 'Nakit satışın yazılacağı kasayı seçin.',
              onChanged: (value) => cashAccountId = value,
            ),
            const SizedBox(height: AppSpacing.medium),
            _choiceField(
              label: 'Satış kategorisi',
              fieldKey: 'day-close-category',
              choices: data.categories,
              selected: cashCategoryId ?? shown?.cash.categoryId,
              missing: 'Nakit satış için kategori seçin.',
              onChanged: (value) => cashCategoryId = value,
            ),
          ],
        );
      },
    ),
  ];

  Widget _choiceField({
    required String label,
    required String fieldKey,
    required List<DataChoice> choices,
    required String? selected,
    required String missing,
    required ValueChanged<String?> onChanged,
  }) {
    // Seçili gelen kayıt listede yoksa (pasife alınmış) alan boş kalır.
    final value = choices.any((item) => item.id == selected) ? selected : null;
    return DropdownButtonFormField<String>(
      key: ValueKey('$fieldKey-$value'),
      initialValue: value,
      isExpanded: true,
      decoration: InputDecoration(labelText: label),
      items: [
        for (final choice in choices)
          DropdownMenuItem(value: choice.id, child: Text(choice.name)),
      ],
      onChanged: (selection) {
        setState(() => onChanged(selection));
        _loadPreview();
      },
      validator: (selection) => selection == null ? missing : null,
    );
  }

  /// Ana POS (yoksa ilk POS) ve tutar yazılmış ya da hesaplanmış satırlar;
  /// `Diğer POS'lar` açıldıysa hepsi.
  List<DayClosePosLine> _visiblePosLines(DayClosePreview? shown) {
    final lines = shown?.posLines ?? const <DayClosePosLine>[];
    if (showOtherPos || lines.length < 2) return lines;
    final main = lines.firstWhere(
      (line) => line.isDefault,
      orElse: () => lines.first,
    );
    return [
      for (final line in lines)
        if (line == main ||
            line.stated ||
            (posControllers[line.posDefinitionId]?.text.isNotEmpty ?? false))
          line,
    ];
  }

  int _hiddenPosCount(DayClosePreview? shown) =>
      (shown?.posLines.length ?? 0) - _visiblePosLines(shown).length;

  /// Toplamdan hesaplanan alan boş durur; hesaplanan tutar altında yazar.
  String? _computedHelper(
    bool computed,
    DayClosePreview? shown,
    String? amount,
  ) => computed && shown != null && amount != null
      ? 'Toplamdan hesaplandı: ${MoneyText.format(amount, shown.currency)}'
      : null;

  bool _hasSummary(DayClosePreview shown) =>
      shown.blockerCode == null &&
      (shown.cash.stated || shown.posLines.any((line) => line.stated));

  /// Bir alanın yanında söylenmeyen engeller.
  String? _notice(String? blocker) => switch (blocker) {
    'day_closes.amounts_required' =>
      submitted ? 'Nakit ya da kart tutarını yazın.' : null,
    'day_closes.existing_exceeds_card' =>
      'İşaretli kartlı kayıtlar kart tutarını aşıyor. Tutarı düzeltin ya da '
          'işareti kaldırın.',
    'day_closes.scope_unresolved' =>
      'Kasanın ya da kategorinin İşletme / Şahsi etiketi yok. Birini '
          'etiketleyip yeniden deneyin.',
    'day_closes.pos_unusable' =>
      "POS'un hesabı ya da kategorisi kullanılamıyor. POS'u düzenleyin.",
    'day_closes.not_closed_yet' =>
      'Bu gün henüz kapatılmadı; ek gün sonu yazılamaz.',
    _ => null,
  };

  static String? _optionalMoneyError(String? value) {
    final text = (value ?? '').trim();
    if (text.isEmpty) return null;
    final amount = MoneyInput.parse(text);
    return amount == null || amount < 0 ? 'Geçerli bir tutar girin.' : null;
  }

  static String _recordTitle(DayCloseExistingRecord record) {
    if (record.title.isNotEmpty) return record.title;
    return switch (record.kind) {
      'pos-settlement' => 'POS satışı',
      'counterparty-payment' => 'Cari tahsilat',
      'obligation-settlement' => 'Alacak tahsilatı',
      _ => 'Gelir',
    };
  }

  static String _recordSubtitle(
    DayCloseExistingRecord record,
    DayClosePreview shown,
  ) {
    if (!record.isCash) {
      for (final line in shown.posLines) {
        if (line.posDefinitionId == record.posDefinitionId) {
          return 'Kart · ${line.name}';
        }
      }
      return 'Kart · ${record.accountName}';
    }
    final kind = switch (record.kind) {
      'counterparty-payment' => 'Cari tahsilat',
      'obligation-settlement' => 'Alacak tahsilatı',
      _ => 'Nakit',
    };
    return '$kind · ${record.accountName}';
  }

  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    // Bekleyen önizleme güncel girdiyle kapatılır: gönderilen tutarlar
    // sunucunun gördüğü tutarlar olmalı.
    _previewTimer?.cancel();
    await _loadPreview();
    if (!mounted) return null;
    final shown = preview;
    setState(() => submitted = true);
    if (shown == null || shown.blockerCode != null) return null;
    if (showTargets && !formKey.currentState!.validate()) return null;

    final saved = await controller.create(
      clientRequestId: requestId,
      input: _input(),
    );
    return saved == null ? null : true;
  }
}

/// Yazılacak kayıtların özeti: nakit satış kasaya bir gelir, her POS'un
/// kartlı satışı bir POS tahsilatı. Bütün tutarlar önizlemeden gelir.
class _WriteSummary extends StatelessWidget {
  const _WriteSummary({required this.preview});

  final DayClosePreview preview;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final cash = preview.cash;
    final lines = [
      for (final line in preview.posLines)
        if (line.stated) line,
    ];
    final writesNothing = !cash.writes && lines.every((line) => !line.writes);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
      decoration: BoxDecoration(
        color: surfaces.cardMuted,
        borderRadius: BorderRadius.circular(AppRadius.field),
      ),
      child: AppDividedColumn(
        children: [
          if (cash.stated)
            _WriteRow(
              title: 'Nakit satış',
              subtitle: [
                if (cash.writes) cash.accountName ?? 'Kasa seçilmedi',
                _deducted(cash.deductedAmount),
              ].nonNulls.join(' · '),
              amount: cash.amountToWrite,
              currency: preview.currency,
            ),
          for (final line in lines)
            _WriteRow(
              title: line.name,
              // İki kısa satır: önce düşülen, sonra komisyon ve beklenen gün.
              subtitle: [
                _deducted(line.deductedAmount),
                [
                  if (line.writes && !isZeroMoney(line.commissionAmount))
                    'komisyon '
                        '${MoneyText.format(line.commissionAmount, preview.currency)}',
                  if (line.writes)
                    '${DateText.dayMonth(line.expectedTransferDate)} beklenir',
                ].join(' · '),
              ].nonNulls.where((part) => part.isNotEmpty).join('\n'),
              amount: line.amountToWrite,
              currency: preview.currency,
            ),
          if (writesNothing)
            const _WriteNote('Yazılacak kayıt yok; gün kapatılır.'),
        ],
      ),
    );
  }

  String? _deducted(String amount) => isZeroMoney(amount)
      ? null
      : '${MoneyText.format(amount, preview.currency)} düşüldü';
}

class _WriteRow extends StatelessWidget {
  const _WriteRow({
    required this.title,
    required this.subtitle,
    required this.amount,
    required this.currency,
  });

  final String title;
  final String subtitle;
  final String amount;
  final String currency;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
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
                      style: theme.textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    if (subtitle.isNotEmpty)
                      Text(
                        subtitle,
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: surfaces.inkMuted,
                        ),
                      ),
                  ],
                ),
              ),
              const SizedBox(width: AppSpacing.medium),
              AppMoneyText(
                amount: amount,
                currency: currency,
                effect: isZeroMoney(amount) ? null : AppMoneyEffect.income,
                size: AppMoneySize.body,
                style: const TextStyle(fontWeight: FontWeight.w600),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _WriteNote extends StatelessWidget {
  const _WriteNote(this.text);

  final String text;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
    child: Text(
      text,
      style: Theme.of(
        context,
      ).textTheme.bodySmall?.copyWith(color: AppSurfaces.of(context).inkMuted),
    ),
  );
}

/// Onay kutulu satır: başlık, alt yazı ve isteğe bağlı tutar.
///
/// `CheckboxListTile` değil: o kendi yazı stilini taşır ve uygulamanın
/// tipografisinden ayrılır. Satırın tamamı dokunma hedefidir.
class _CheckRow extends StatelessWidget {
  const _CheckRow({
    required this.title,
    required this.subtitle,
    required this.value,
    required this.enabled,
    required this.onChanged,
    super.key,
    this.amount,
    this.currency,
  });

  final String title;
  final String subtitle;
  final bool value;
  final bool enabled;
  final ValueChanged<bool> onChanged;
  final String? amount;
  final String? currency;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return MergeSemantics(
      child: InkWell(
        onTap: enabled ? () => onChanged(!value) : null,
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 48),
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
            child: Row(
              children: [
                Checkbox(
                  value: value,
                  onChanged: enabled
                      ? (selection) => onChanged(selection ?? false)
                      : null,
                ),
                const SizedBox(width: AppSpacing.xSmall),
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
                          color: surfaces.inkMuted,
                        ),
                      ),
                    ],
                  ),
                ),
                if (amount != null && currency != null) ...[
                  const SizedBox(width: AppSpacing.small),
                  AppMoneyText(
                    amount: amount!,
                    currency: currency!,
                    size: AppMoneySize.body,
                  ),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}
