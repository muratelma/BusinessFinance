import 'dart:async';

import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_math.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/network/client_request_id.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../../../core/network/api_error_messages.dart';
import '../data/day_close_repository.dart';
import 'day_close_answers.dart';
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
/// Hiçbir tutar burada hesaplanmaz: düşülen kayıtlar, komisyon ve yazılacak
/// tutar **sunucunun önizlemesinden** gelir. Form yalnız yazılanı ve verilen
/// cevapları gönderir, gelen cevabı gösterir.
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

  /// Kullanıcının işaretleri ve cevapları.
  final answers = DayCloseAnswers();
  final overlapChoices = <String, _OverlapChoice>{};
  final overlapControllers = <String, TextEditingController>{};
  bool previewPending = true;

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
    for (final item in overlapControllers.values) {
      item.dispose();
    }
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  /// Boş alan gönderilmez; o tarafa dokunulmaz.
  String? _wire(TextEditingController field) {
    return MoneyMath.fromInput(field.text);
  }

  DayCloseInput _input() {
    final cashAmount = _wire(cashController);
    return DayCloseInput(
      date: date,
      cashAmount: cashAmount,
      posAmounts: {
        for (final entry in posControllers.entries)
          entry.key: ?_wire(entry.value),
      },
      totalAmount: _wire(totalController),
      cashAccountId: cashAccountId,
      cashCategoryId: cashCategoryId,
      recordOverrides: Map.of(answers.recordOverrides),
      // Ortak tutar yalnız nakit tutarı yazılmışken sorulur; nakit boşken
      // gönderilen cevabı sunucu reddeder.
      overlaps: cashAmount == null ? const {} : Map.of(answers.overlaps),
      isAdditional: isAdditional,
    );
  }

  /// Tutar değişince önizleme kısa bir duraklamadan sonra istenir; her tuşta
  /// istek atılmaz.
  void _amountChanged(String _) {
    ++_previewRequest;
    setState(() => previewPending = true);
    _previewTimer?.cancel();
    _previewTimer = Timer(const Duration(milliseconds: 350), _loadPreview);
  }

  Future<void> _loadPreview() async {
    _previewTimer?.cancel();
    final request = ++_previewRequest;
    if (mounted) setState(() => previewPending = true);
    final result = await controller.preview(_input());
    // Sonradan değişen girdinin cevabı eskisinin üstüne yazılmasın.
    if (!mounted || request != _previewRequest) return;
    // Listede artık olmayan bir kaydın cevabı düştüyse sunucu bu girdiyi
    // reddetmiştir; güncel listeyle yeniden sorulur.
    if (result != null) {
      final previousOverlaps = answers.overlaps.keys.toSet();
      final changed = answers.reconcile(result);
      for (final id in previousOverlaps) {
        if (!answers.overlaps.containsKey(id)) {
          overlapChoices.remove(id);
          overlapControllers[id]?.clear();
        }
      }
      if (changed) return _loadPreview();
    }
    setState(() {
      previewPending = false;
      if (result == null) return;
      preview = result;
      if (result.cash.stated) {
        final asked = {for (final group in result.overlapGroups) group.groupId};
        overlapChoices.removeWhere((id, _) => !asked.contains(id));
      }
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

  void _toggleRecord(DayCloseExistingRecord record) {
    setState(() {
      answers.toggle(record);
      overlapChoices.remove(record.groupId);
      overlapControllers[record.groupId]?.clear();
    });
    _loadPreview();
  }

  void _setAll(List<DayCloseExistingRecord> records, bool value) {
    setState(() {
      answers.setAll(records, value);
      for (final record in records) {
        overlapChoices.remove(record.groupId);
        overlapControllers[record.groupId]?.clear();
      }
    });
    _loadPreview();
  }

  void _chooseOverlap(DayCloseOverlapGroup group, _OverlapChoice choice) {
    setState(() {
      overlapChoices[group.groupId] = choice;
      switch (choice) {
        case _OverlapChoice.separate:
          answers.setOverlap(group.groupId, '0.0000');
        case _OverlapChoice.inside:
          answers.setOverlap(group.groupId, group.maximumOverlap);
        case _OverlapChoice.partial:
          _setPartialOverlap(group.groupId);
      }
    });
    _loadPreview();
  }

  void _setPartialOverlap(String groupId) {
    final field = overlapControllers.putIfAbsent(
      groupId,
      TextEditingController.new,
    );
    final amount = _wire(field);
    if (amount == null) {
      answers.clearOverlap(groupId);
    } else {
      answers.setOverlap(groupId, amount);
    }
  }

  void _dateChanged(String value) {
    setState(() {
      date = value;
      // Başka günün kayıtları başka kayıtlardır.
      answers.clear();
      overlapChoices.clear();
      for (final field in overlapControllers.values) {
        field.clear();
      }
      isAdditional = false;
    });
    _loadPreview();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: formKey,
    child: AppFormSheet<bool>(
      title: 'Gün sonu',
      // Teslim 16 dp yan boşlukla çizildi; 24 dp'de kural cümlesi ve fatura
      // sorusu ikinci satıra taşıyor.
      horizontalPadding: AppSpacing.medium,
      submitLabel: 'Gün sonunu kaydet',
      onSubmit:
          previewPending ||
              _waitingForAnswers ||
              controller.previewError != null
          ? null
          : _submit,
      children: _fields(context),
    ),
  );

  List<Widget> _fields(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
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
        valueText: DateText.dayMonthWeekday(date),
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
        inputFormatters: const [TurkishAmountInputFormatter()],
        decoration: InputDecoration(
          labelText: 'Nakit tutarı',
          suffixIcon: _CurrencySuffix(shown?.currency),
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
          inputFormatters: const [TurkishAmountInputFormatter()],
          decoration: InputDecoration(
            labelText: line.name,
            suffixIcon: _CurrencySuffix(shown?.currency),
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
        inputFormatters: const [TurkishAmountInputFormatter()],
        decoration: InputDecoration(
          labelText: 'Toplam (isteğe bağlı)',
          suffixIcon: _CurrencySuffix(shown?.currency),
          errorMaxLines: 3,
          errorText: blocker == 'day_closes.total_below_parts'
              ? ApiErrorMessages.resolve(blocker!)
              : null,
        ),
        onChanged: _amountChanged,
        validator: _optionalMoneyError,
      ),
      if (shown?.totalDifference != null)
        AppInlineNotice(
          message: _differenceNotice(shown!),
          margin: const EdgeInsets.only(top: AppSpacing.small),
        ),
      // Gün kapalıyken yazılacak bir şey yok; liste "ek gün sonu" seçilince
      // gelir.
      if (shown != null && !closedAndNotAdditional)
        ..._existingFields(shown, busy),
      if (notice != null)
        AppInlineNotice(
          message: notice,
          icon: Icons.error_outline,
          margin: const EdgeInsets.only(top: AppSpacing.small),
        ),
      if (shown != null && _hasSummary(shown)) ...[
        const SizedBox(height: AppSpacing.medium),
        Text(
          'Yazılacak',
          style: theme.textTheme.labelMedium?.copyWith(
            color: surfaces.inkFaint,
          ),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        _WriteSummary(preview: shown, cashPending: _cashPendingMessage(shown)),
        if (!showTargets)
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

  bool get _waitingForAnswers {
    final shown = preview;
    if (shown == null || !shown.cash.stated) return false;
    return shown.existingRecords.any(
          (record) =>
              record.isCash &&
              record.requiresAnswer &&
              answers.included(record) == null,
        ) ||
        shown.overlapGroups.any(
          (group) => !answers.overlaps.containsKey(group.groupId),
        );
  }

  String? _cashPendingMessage(DayClosePreview shown) {
    final count = shown.existingRecords
        .where(
          (record) =>
              record.isCash &&
              record.requiresAnswer &&
              answers.included(record) == null,
        )
        .length;
    if (count > 0) return '$count kayıt için seçim yapılınca hesaplanır.';
    if (shown.blockerCode == 'day_closes.overlap_unanswered' ||
        shown.overlapGroups.any(
          (group) => !answers.overlaps.containsKey(group.groupId),
        )) {
      return 'Yukarıdaki soru cevaplanınca hesaplanır.';
    }
    return previewPending ? 'Hesaplanıyor…' : null;
  }

  String _differenceNotice(DayClosePreview shown) {
    final prefix = !shown.cash.stated
        ? 'Yalnız kart satışı kaydedilir.'
        : shown.posLines.every((line) => !line.stated)
        ? 'Yalnız nakit satış kaydedilir.'
        : 'Nakit ve kart satışı kaydedilir.';
    return '$prefix Toplamla arasındaki '
        '${MoneyText.format(MoneyText.unsigned(shown.totalDifference!), shown.currency)} kaydedilmez.';
  }

  List<Widget> _existingFields(DayClosePreview shown, bool busy) {
    final records = shown.existingRecords
        .where((record) => !record.isCash || _wire(cashController) != null)
        .toList();
    if (records.isEmpty) return [];
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final asked = records.where((record) => record.requiresAnswer).toList();
    final orderedAsked = <DayCloseExistingRecord>[];
    final placed = <String>{};
    // Her kişinin/faturanın satırları bitişiktir; soru kendi çiftinin altındadır.
    for (final record in asked) {
      if (!placed.add(record.key)) continue;
      orderedAsked.add(record);
      if (record.groupId != null) {
        for (final sibling in asked) {
          if (sibling.groupId == record.groupId && placed.add(sibling.key)) {
            orderedAsked.add(sibling);
          }
        }
      }
    }
    Widget row(DayCloseExistingRecord record) => _CheckRow(
      key: ValueKey('day-close-record-${record.key}'),
      title: _recordTitle(record),
      subtitle: _recordSubtitle(record, shown),
      amount: record.amount,
      currency: shown.currency,
      value: answers.included(record),
      enabled: !busy,
      onChanged: (_) => _toggleRecord(record),
    );
    return [
      const SizedBox(height: AppSpacing.medium),
      Text(
        'Gün içinde girilenler',
        style: theme.textTheme.labelMedium?.copyWith(color: surfaces.inkFaint),
      ),
      Text(
        'İşaretli kayıtlar yazılan tutarın içindedir; tekrar kaydedilmez.',
        // Teslimde harf aralığı yoktur; temanın aralığıyla cümle ikinci
        // satıra taşıyor.
        style: theme.textTheme.bodySmall?.copyWith(
          color: surfaces.inkMuted,
          letterSpacing: 0,
        ),
      ),
      for (final record in records.where((record) => !record.requiresAnswer))
        row(record),
      if (asked.isNotEmpty) ...[
        // Çizgi işaretli gelenleri sorulanlardan ayırır; üstte satır yoksa
        // ayıracak bir şey de yoktur.
        if (records.any((record) => !record.requiresAnswer))
          Divider(
            height: AppSpacing.medium,
            thickness: 1,
            color: surfaces.border,
          )
        else
          const SizedBox(height: AppSpacing.small),
        Text(
          'Bunlar yazdığınız nakit tutarın içinde mi?',
          style: theme.textTheme.bodySmall,
        ),
        const SizedBox(height: AppSpacing.small),
        AppSegmentRail<bool>(
          key: const ValueKey('day-close-all'),
          values: const [true, false],
          selected: answers.allIncluded(asked),
          onChanged: (value) {
            if (!busy) _setAll(asked, value);
          },
          semanticLabel: 'Nakit tutarına dahil olan kayıtlar',
          segmentLabel: (value) => value ? 'Hepsi içinde' : 'Hiçbiri',
          segmentBuilder: (context, value, selected) => Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.small),
            child: Text(
              value ? 'Hepsi içinde' : 'Hiçbiri',
              textAlign: TextAlign.center,
              style: theme.textTheme.labelLarge?.copyWith(
                color: selected ? surfaces.ink : surfaces.inkMuted,
              ),
            ),
          ),
        ),
        const SizedBox(height: AppSpacing.small),
        Text(
          'Tek tek değiştirmek için satıra dokunun.',
          style: theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted),
        ),
        for (var i = 0; i < orderedAsked.length; i++) ...[
          row(orderedAsked[i]),
          if (i == orderedAsked.length - 1 ||
              orderedAsked[i + 1].groupId != orderedAsked[i].groupId)
            for (final group in shown.overlapGroups)
              if (group.groupId == orderedAsked[i].groupId &&
                  orderedAsked
                      .where((record) => record.groupId == group.groupId)
                      .any(
                        (record) =>
                            record.isDeferredSale &&
                            answers.included(record) == true,
                      ) &&
                  orderedAsked
                      .where((record) => record.groupId == group.groupId)
                      .any(
                        (record) =>
                            record.isCollection &&
                            answers.included(record) == true,
                      ))
                _OverlapBlock(
                  key: ValueKey('day-close-overlap-${group.groupId}'),
                  group: group,
                  currency: shown.currency,
                  choice: overlapChoices[group.groupId],
                  controller: overlapControllers.putIfAbsent(
                    group.groupId,
                    TextEditingController.new,
                  ),
                  enabled: !busy,
                  pending: previewPending,
                  onChoose: (choice) => _chooseOverlap(group, choice),
                  onAmountChanged: (value) {
                    _setPartialOverlap(group.groupId);
                    _amountChanged(value);
                  },
                ),
        ],
      ],
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

  /// Ana POS (yoksa ilk POS) ve tutar yazılmış satırlar; `Diğer POS'lar`
  /// açıldıysa hepsi.
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

  bool _hasSummary(DayClosePreview shown) =>
      (shown.blockerCode == null ||
          shown.blockerCode == 'day_closes.records_unanswered' ||
          shown.blockerCode == 'day_closes.overlap_unanswered') &&
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
    'day_closes.invalid_overlap' => ApiErrorMessages.resolve(blocker!),
    _ => null,
  };

  static String? _optionalMoneyError(String? value) {
    final text = (value ?? '').trim();
    if (text.isEmpty) return null;
    return MoneyMath.fromInput(text) == null
        ? 'Geçerli bir tutar girin.'
        : null;
  }

  static String _recordTitle(DayCloseExistingRecord record) {
    if (record.title.isNotEmpty) return record.title;
    return switch (record.kind) {
      'pos-settlement' => 'POS satışı',
      'counterparty-payment' => 'Tahsilat',
      'obligation-settlement' => 'Tahsilat',
      'counterparty-charge' => 'Veresiye satış',
      'obligation' => 'Alacak faturası',
      _ => 'Gelir',
    };
  }

  static String _recordSubtitle(
    DayCloseExistingRecord record,
    DayClosePreview shown,
  ) {
    if (!record.isCash) {
      // Kartla tahsil satış değildir: kendini söyler (KP7).
      final label = record.isCardCollection ? 'Kartla tahsil' : 'Kartla';
      for (final line in shown.posLines) {
        if (line.posDefinitionId == record.posDefinitionId) {
          return '$label · ${line.name}';
        }
      }
      return '$label · ${record.accountName}';
    }
    final kind = switch (record.kind) {
      'counterparty-payment' => 'Tahsilat',
      'obligation-settlement' => 'Tahsilat',
      'counterparty-charge' => 'Veresiye satış',
      'obligation' => 'Alacak faturası',
      _ => 'Nakit',
    };
    // Veresiye satış ve alacak faturası bir hesaba girmez.
    return [
      kind,
      if (record.accountName.isNotEmpty) record.accountName,
    ].join(' · ');
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
    if (shown == null ||
        previewPending ||
        controller.previewError != null ||
        shown.blockerCode != null) {
      return null;
    }
    if (showTargets && !formKey.currentState!.validate()) return null;

    final saved = await controller.create(
      clientRequestId: requestId,
      input: _input(),
    );
    return saved == null ? null : true;
  }
}

enum _OverlapChoice { separate, inside, partial }

class _CurrencySuffix extends StatelessWidget {
  const _CurrencySuffix(this.currency);
  final String? currency;

  @override
  Widget build(BuildContext context) => Center(
    widthFactor: 1,
    heightFactor: 1,
    child: Padding(
      padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
      child: Text(currency ?? '', style: Theme.of(context).textTheme.bodySmall),
    ),
  );
}

/// Her seçenek tutarını sunucu verir; seçili seçenek panelin durumudur.
class _OverlapBlock extends StatelessWidget {
  const _OverlapBlock({
    required this.group,
    required this.currency,
    required this.choice,
    required this.controller,
    required this.enabled,
    required this.pending,
    required this.onChoose,
    required this.onAmountChanged,
    super.key,
  });

  final DayCloseOverlapGroup group;
  final String currency;
  final _OverlapChoice? choice;
  final TextEditingController controller;
  final bool enabled;
  final bool pending;
  final ValueChanged<_OverlapChoice> onChoose;
  final ValueChanged<String> onAmountChanged;

  String get insideLabel => group.isInvoice
      ? group.collectionsLarger
            ? 'Fatura tahsilatın içinde'
            : 'Tahsilat faturanın içinde'
      : group.collectionsLarger
      ? 'Satış tahsilatın içinde'
      : 'Tahsilat satışın içinde';

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final helper = theme.textTheme.bodySmall?.copyWith(
      color: surfaces.inkFaint,
    );
    return Container(
      margin: const EdgeInsets.only(
        top: AppSpacing.small,
        bottom: AppSpacing.small,
      ),
      padding: const EdgeInsets.all(AppSpacing.medium),
      decoration: BoxDecoration(
        color: surfaces.cardMuted,
        borderRadius: BorderRadius.circular(AppRadius.field),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // Ad yazılmaz: blok o kişinin (ya da faturanın) satırlarının hemen
          // altındadır.
          Text(
            group.isInvoice
                ? 'Fatura ve tahsilatı yazdığınız nakit tutarda nasıl sayıldı?'
                : 'Satış ve tahsilat yazdığınız nakit tutarda nasıl sayıldı?',
            // Teslimdeki gibi harf aralıksız: soru tek satıra sığar.
            style: theme.textTheme.bodySmall?.copyWith(
              color: surfaces.ink,
              letterSpacing: 0,
            ),
          ),
          // Sağdaki tutarların ne olduğunu sütunun başlığı söyler.
          Align(
            alignment: AlignmentDirectional.centerEnd,
            child: Padding(
              padding: const EdgeInsets.only(top: AppSpacing.xSmall),
              child: Text('Kayıtlı sayılan', style: helper),
            ),
          ),
          for (final option in _OverlapChoice.values)
            _OverlapOption(
              key: ValueKey(
                'day-close-overlap-${group.groupId}-${option.name}',
              ),
              label: switch (option) {
                _OverlapChoice.separate => 'İkisi ayrı ayrı',
                _OverlapChoice.inside => insideLabel,
                _OverlapChoice.partial => 'Bir kısmı ikisinde de var',
              },
              amount: switch (option) {
                _OverlapChoice.separate => group.separateAmount,
                _OverlapChoice.inside => group.insideAmount,
                _OverlapChoice.partial =>
                  choice == _OverlapChoice.partial &&
                          !pending &&
                          group.overlapAmount != null
                      ? group.deductedAmount
                      : null,
              },
              currency: currency,
              selected: option == choice,
              onTap: enabled ? () => onChoose(option) : null,
            ),
          if (choice == _OverlapChoice.partial) ...[
            const SizedBox(height: AppSpacing.small),
            TextFormField(
              key: ValueKey('day-close-shared-${group.groupId}'),
              controller: controller,
              enabled: enabled,
              keyboardType: const TextInputType.numberWithOptions(
                decimal: true,
              ),
              inputFormatters: const [TurkishAmountInputFormatter()],
              decoration: InputDecoration(
                labelText: 'İkisinde de sayılan',
                suffixIcon: _CurrencySuffix(currency),
                helperText:
                    'En çok ${MoneyText.format(group.maximumOverlap, currency)}.',
                helperMaxLines: 3,
              ),
              validator: _DayCloseFormState._optionalMoneyError,
              onChanged: onAmountChanged,
            ),
          ],
        ],
      ),
    );
  }
}

class _OverlapOption extends StatelessWidget {
  const _OverlapOption({
    required this.label,
    required this.amount,
    required this.currency,
    required this.selected,
    required this.onTap,
    super.key,
  });

  final String label;
  final String? amount;
  final String currency;
  final bool selected;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) => Semantics(
    checked: selected,
    inMutuallyExclusiveGroup: true,
    enabled: onTap != null,
    label: [
      label,
      if (amount != null) MoneyText.format(amount!, currency),
    ].join(', '),
    onTap: onTap,
    excludeSemantics: true,
    child: InkWell(
      onTap: onTap,
      child: ConstrainedBox(
        constraints: const BoxConstraints(minHeight: 48),
        child: Row(
          children: [
            SizedBox(
              width: 36,
              child: Icon(
                selected
                    ? Icons.radio_button_checked
                    : Icons.radio_button_unchecked,
                size: 20,
                color: selected
                    ? AppSurfaces.of(context).ink
                    : AppSurfaces.of(context).inkMuted,
              ),
            ),
            Expanded(
              child: _LabelAmount(
                centered: true,
                label: Text(
                  label,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: AppSurfaces.of(context).ink,
                  ),
                ),
                amount: amount == null
                    ? null
                    : Text(
                        MoneyText.format(amount!, currency),
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: AppSurfaces.of(context).inkMuted,
                        ),
                      ),
              ),
            ),
          ],
        ),
      ),
    ),
  );
}

/// Yalnız yazılmış alanların kartları. Sayılar ve döküm önizlemedendir.
class _WriteSummary extends StatelessWidget {
  const _WriteSummary({required this.preview, this.cashPending});

  final DayClosePreview preview;
  final String? cashPending;

  @override
  Widget build(BuildContext context) {
    final cash = preview.cash;
    final deductions = cash.deductions;
    String money(String value) => MoneyText.format(value, preview.currency);
    final parts = [
      if (!isZeroMoney(deductions.salesAmount))
        '${money(deductions.salesAmount)} satış',
      if (!isZeroMoney(deductions.creditSalesAmount))
        '${money(deductions.creditSalesAmount)} veresiye satış',
      if (!isZeroMoney(deductions.invoicesAmount))
        '${money(deductions.invoicesAmount)} alacak faturası',
      if (!isZeroMoney(deductions.collectionsAmount))
        '${money(deductions.collectionsAmount)} tahsilat',
    ];
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (cash.stated)
          _WriteCard(
            title: 'Yeni nakit satış',
            subtitle: [
              cash.accountName ?? 'Kasa seçilmedi',
              cash.categoryName ?? 'Kategori seçilmedi',
            ].join(' · '),
            amount: cash.amountToWrite,
            enteredLabel: 'Nakit tutarı',
            entered: cash.enteredAmount,
            deducted: cash.deductedAmount,
            currency: preview.currency,
            pending: cashPending,
            breakdown: parts.join(' + '),
            shared: isZeroMoney(deductions.sharedAmount)
                ? null
                : '− ${money(deductions.sharedAmount)} ikisinde de',
          ),
        for (final line in preview.posLines)
          if (line.stated) ...[
            if (cash.stated ||
                preview.posLines.where((item) => item.stated).first != line)
              const SizedBox(height: AppSpacing.small),
            _WriteCard(
              title: '${line.name} satışı',
              subtitle: [
                if (line.writes) 'Komisyon ${money(line.commissionAmount)}',
                if (line.writes)
                  '${DateText.dayMonthWeekday(line.expectedTransferDate)} hesaba geçer',
              ].join('\n'),
              amount: line.amountToWrite,
              enteredLabel: 'Kart tutarı',
              entered: line.enteredAmount,
              deducted: line.deductedAmount,
              currency: preview.currency,
              showCalculation: !isZeroMoney(line.deductedAmount),
            ),
          ],
        if (cashPending == null &&
            !cash.writes &&
            preview.posLines.every((line) => !line.writes))
          Padding(
            padding: const EdgeInsets.only(top: AppSpacing.small),
            child: Text(
              'Yazılacak kayıt yok; gün kapatılır.',
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ),
      ],
    );
  }
}

class _WriteCard extends StatelessWidget {
  const _WriteCard({
    required this.title,
    required this.subtitle,
    required this.amount,
    required this.enteredLabel,
    required this.entered,
    required this.deducted,
    required this.currency,
    this.pending,
    this.breakdown = '',
    this.shared,
    this.showCalculation = true,
  });

  final String title;
  final String subtitle;
  final String amount;
  final String enteredLabel;
  final String entered;
  final String deducted;
  final String currency;
  final String? pending;
  final String breakdown;
  final String? shared;
  final bool showCalculation;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final helper = theme.textTheme.bodySmall?.copyWith(
      color: surfaces.inkMuted,
    );
    return Container(
      padding: const EdgeInsets.all(AppSpacing.medium),
      decoration: BoxDecoration(
        color: surfaces.cardMuted,
        borderRadius: BorderRadius.circular(AppRadius.field),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          _LabelAmount(
            label: Text(title, style: theme.textTheme.bodyMedium),
            amount: pending == null
                ? AppMoneyText(
                    amount: amount,
                    currency: currency,
                    signed: true,
                    effect: isZeroMoney(amount) ? null : AppMoneyEffect.income,
                    size: AppMoneySize.body,
                    style: const TextStyle(fontWeight: FontWeight.w600),
                  )
                : null,
          ),
          if (pending != null)
            Text(pending!, style: helper)
          else ...[
            if (subtitle.isNotEmpty) Text(subtitle, style: helper),
            if (showCalculation) ...[
              Divider(
                height: AppSpacing.large,
                thickness: 1,
                color: surfaces.border,
              ),
              _LabelAmount(
                label: Text(enteredLabel, style: helper),
                amount: Text(
                  MoneyText.format(entered, currency),
                  style: theme.textTheme.bodySmall,
                ),
              ),
              const SizedBox(height: AppSpacing.xSmall),
              _LabelAmount(
                label: Text('Zaten kayıtlı', style: helper),
                amount: Text(
                  '${isZeroMoney(deducted) ? '' : '−'}${MoneyText.format(deducted, currency)}',
                  style: theme.textTheme.bodySmall,
                ),
              ),
              if (breakdown.isNotEmpty) ...[
                const SizedBox(height: AppSpacing.small),
                Text(
                  breakdown,
                  style: theme.textTheme.labelMedium?.copyWith(
                    color: surfaces.inkFaint,
                    fontWeight: FontWeight.w400,
                  ),
                ),
              ],
              if (shared != null)
                Text(
                  shared!,
                  style: theme.textTheme.labelMedium?.copyWith(
                    color: surfaces.inkFaint,
                    fontWeight: FontWeight.w400,
                  ),
                ),
            ],
          ],
        ],
      ),
    );
  }
}

/// Büyük yazıda tutar ayrı satıra iner; hiçbir tutar kırpılmaz veya küçülmez.
class _LabelAmount extends StatelessWidget {
  const _LabelAmount({required this.label, this.amount, this.centered = false});
  final Widget label;
  final Widget? amount;

  /// Tutar satırın ortasına hizalanır (kayıt ve seçenek satırları); kartın
  /// başlığında üstte durur.
  final bool centered;

  @override
  Widget build(BuildContext context) {
    if (amount == null) return label;
    if (MediaQuery.textScalerOf(context).scale(16) > 24) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          label,
          const SizedBox(height: AppSpacing.xSmall),
          amount!,
        ],
      );
    }
    return Row(
      crossAxisAlignment: centered
          ? CrossAxisAlignment.center
          : CrossAxisAlignment.start,
      children: [
        Expanded(child: label),
        const SizedBox(width: AppSpacing.small),
        amount!,
      ],
    );
  }
}

/// Satır bütünü onay kutusudur; cevapsız satır karışık durumuyla okunur.
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
  final bool? value;
  final bool enabled;
  final ValueChanged<bool> onChanged;
  final String? amount;
  final String? currency;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    void toggle() => onChanged(!(value ?? false));
    return Semantics(
      checked: value ?? false,
      mixed: value == null,
      enabled: enabled,
      label: [
        title,
        subtitle,
        if (amount != null && currency != null)
          MoneyText.format(amount!, currency!),
      ].join(', '),
      onTap: enabled ? toggle : null,
      excludeSemantics: true,
      child: InkWell(
        onTap: enabled ? toggle : null,
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 56),
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
            child: Row(
              children: [
                SizedBox(
                  width: 48,
                  height: 48,
                  child: value == null
                      ? Icon(
                          Icons.help_outline,
                          size: 20,
                          color: surfaces.inkMuted,
                        )
                      : Checkbox(
                          value: value,
                          onChanged: enabled ? (_) => toggle() : null,
                        ),
                ),
                Expanded(
                  child: _LabelAmount(
                    centered: true,
                    label: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          title,
                          style: theme.textTheme.bodyMedium?.copyWith(
                            fontWeight: FontWeight.w600,
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
                    amount: amount != null && currency != null
                        ? AppMoneyText(
                            amount: amount!,
                            currency: currency!,
                            size: AppMoneySize.body,
                            style: const TextStyle(fontWeight: FontWeight.w600),
                          )
                        : null,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
