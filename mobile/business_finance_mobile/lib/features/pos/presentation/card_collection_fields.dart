import 'dart:async';

import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_input.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_choice_segments.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../categories/presentation/fixed_scope_category_field.dart';
import '../../../core/widgets/app_detail_block.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../data/pos_repository.dart';
import '../../../core/widgets/app_account_field.dart';
import '../../../features/accounts/presentation/account_directory.dart';

/// Bir tahsilatın nasıl alındığı: elden / hesaba ya da kartla (POS).
enum CollectionMethod {
  account('Nakit / hesaba'),
  card('Kartla (POS)');

  const CollectionMethod(this.label);

  final String label;
}

/// `Nakit / hesaba · Kartla (POS)` rayı (K1). Yalnız tahsilatta çıkar:
/// tedarikçiye ödeme POS'tan geçmez (ADR 0019 T7).
class CollectionMethodRail extends StatelessWidget {
  const CollectionMethodRail({
    required this.selected,
    required this.onChanged,
    super.key,
  });

  final CollectionMethod selected;
  final ValueChanged<CollectionMethod> onChanged;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return AppSegmentRail<CollectionMethod>(
      values: CollectionMethod.values,
      selected: selected,
      semanticLabel: 'Nasıl ödendi?',
      segmentLabel: (value) => value.label,
      onChanged: onChanged,
      segmentBuilder: (context, value, isSelected) => Text(
        value.label,
        style: Theme.of(context).textTheme.bodyMedium?.copyWith(
          fontWeight: FontWeight.w600,
          color: isSelected ? surfaces.ink : surfaces.inkMuted,
        ),
      ),
    );
  }
}

/// Kartla tahsil formunun ihtiyaç duyduğu okumalar (ADR 0019 T5).
///
/// Cari tahsilat ve tek seferlik alacağın kapanışı aynı alan grubunu kullanır;
/// iki ekran POS'ları ve önizlemeyi aynı yerden okur.
class CardCollectionSource {
  const CardCollectionSource({
    required this.definitions,
    required this.bankAccounts,
    required this.expenseCategories,
    required this.preview,
  });

  /// Aktif POS'lar; ana POS seçili gelir.
  final List<PosDefinitionItem> definitions;

  /// "Elle gir"de paranın geçeceği hesap: POS parası bankaya geçer.
  final List<DataChoice> bankAccounts;
  final List<DataChoice> expenseCategories;

  /// Komisyon, net ve beklenen gün sunucudan gelir; istemci hesaplamaz.
  final Future<PosPreview> Function({
    required String definitionId,
    required String grossAmount,
    required String settlementDate,
  })
  preview;

  PosDefinitionItem? get preferred {
    for (final item in definitions) {
      if (item.isDefault) return item;
    }
    return definitions.isEmpty ? null : definitions.first;
  }
}

enum _CommissionMode { none, amount, rate }

/// "Kartla (POS)" seçiliyken hesap alanının yerine gelen alanlar.
///
/// POS seçiliyse yalnız önizleme görünür (komisyon, hesaba geçecek, beklenen
/// gün, geçeceği hesap); "Elle gir"de hesap, beklenen gün ve isteğe bağlı
/// komisyon sorulur. Değer [CardCollectionFieldsState.request] ile okunur ve
/// sunucuya `card` bloğu olarak gider. Tutar ve gün formun kendi
/// alanlarıdır; burada yalnız okunur.
class CardCollectionFields extends StatefulWidget {
  const CardCollectionFields({
    required this.source,
    required this.amount,
    required this.collectedOn,
    super.key,
  });

  final CardCollectionSource source;

  /// Formun tutar alanı; önizleme her değişimde kısa bir duraklamadan sonra
  /// yeniden istenir.
  final TextEditingController amount;

  /// Tahsil günü (`yyyy-MM-dd`).
  final String collectedOn;

  @override
  State<CardCollectionFields> createState() => CardCollectionFieldsState();
}

class CardCollectionFieldsState extends State<CardCollectionFields> {
  /// "Elle gir" seçeneğinin açılır listedeki değeri.
  static const _manual = '';

  final _commission = TextEditingController();
  String? _definitionId;
  String? _accountId;
  late List<DataChoice> _bankAccounts = widget.source.bankAccounts;
  String? _expected;
  String? _commissionCategoryId;
  _CommissionMode _mode = _CommissionMode.none;
  PosPreview? _preview;
  Timer? _timer;
  int _request = 0;

  PosDefinitionItem? get _definition {
    for (final item in widget.source.definitions) {
      if (item.id == _definitionId) return item;
    }
    return null;
  }

  /// Sunucuya giden `card` bloğu. Form doğrulaması geçtikten sonra okunur.
  Map<String, Object?> get request {
    final current = _definition;
    if (current != null) return {'posDefinitionId': current.id};
    return {
      'accountId': _accountId,
      'expectedTransferDate': _expected ?? widget.collectedOn,
      if (_mode == _CommissionMode.amount)
        'commissionAmount': MoneyInput.wire(_commission.text),
      // Oran saklanmaz, sunucu tutara çevirir (ADR 0009).
      if (_mode == _CommissionMode.rate)
        'commissionRate': PosRate.fractionWire(_commission.text),
      if (_mode != _CommissionMode.none)
        'commissionCategoryId': _commissionCategoryId,
    };
  }

  @override
  void initState() {
    super.initState();
    _definitionId = widget.source.preferred?.id;
    _accountId = _bankAccounts.isEmpty ? null : _bankAccounts.first.id;
    widget.amount.addListener(_schedule);
    _schedule();
  }

  @override
  void didUpdateWidget(covariant CardCollectionFields oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.amount != widget.amount) {
      oldWidget.amount.removeListener(_schedule);
      widget.amount.addListener(_schedule);
    }
    if (oldWidget.collectedOn != widget.collectedOn) _schedule();
  }

  @override
  void dispose() {
    _timer?.cancel();
    widget.amount.removeListener(_schedule);
    _commission.dispose();
    super.dispose();
  }

  void _schedule() {
    _timer?.cancel();
    final amount = MoneyInput.parse(widget.amount.text);
    if (_definition == null || amount == null || amount <= 0) {
      if (_preview != null && mounted) setState(() => _preview = null);
      return;
    }
    _timer = Timer(const Duration(milliseconds: 350), _load);
  }

  Future<void> _load() async {
    final current = _definition;
    if (current == null) return;
    final request = ++_request;
    try {
      final result = await widget.source.preview(
        definitionId: current.id,
        grossAmount: MoneyInput.wire(widget.amount.text),
        settlementDate: widget.collectedOn,
      );
      if (!mounted || request != _request) return;
      setState(() => _preview = result);
    } on Exception {
      // Önizleme bir yardımdır; okunamazsa kayıt yine sunucunun kuralıyla
      // yazılır. Alanlar "—" gösterir.
      if (!mounted || request != _request) return;
      setState(() => _preview = null);
    }
  }

  @override
  Widget build(BuildContext context) {
    final definitions = widget.source.definitions;
    final current = _definition;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppAccountField(
          label: 'POS',
          options: [
            for (final item in definitions)
              AccountChoices.of(
                context,
              ).pos(item.id, item.name, accountName: item.accountName),
          ],
          value: _definitionId ?? _manual,
          manualLabel: 'Elle gir',
          manualNote: 'POS seçmeden; hesabı siz seçersiniz',
          manualValue: _manual,
          onChanged: (value) {
            setState(() {
              _definitionId = value == null || value == _manual ? null : value;
              _preview = null;
            });
            _schedule();
          },
        ),
        const SizedBox(height: AppSpacing.medium),
        if (current != null) _previewBlock(current) else ..._manualFields(),
      ],
    );
  }

  Widget _previewBlock(PosDefinitionItem current) {
    final shown = _preview;
    return AppDetailBlock(
      rows: [
        AppDetailRow(
          label: 'Komisyon (${current.rateLabel})',
          trailing: shown == null
              ? null
              : AppMoneyText(
                  amount: shown.commissionAmount,
                  currency: shown.currency,
                  effect: AppMoneyEffect.expense,
                  signed: !_isZero(shown.commissionAmount),
                  size: AppMoneySize.body,
                ),
          value: shown == null ? '—' : null,
        ),
        AppDetailRow(
          label: 'Hesaba geçecek',
          trailing: shown == null
              ? null
              : AppMoneyText(
                  amount: shown.netAmount,
                  currency: shown.currency,
                  size: AppMoneySize.body,
                  style: const TextStyle(fontWeight: FontWeight.w600),
                ),
          value: shown == null ? '—' : null,
        ),
        AppDetailRow(
          label: 'Beklenen gün',
          value: shown == null
              ? current.transferLabel
              : DateText.dayMonthWeekday(shown.expectedTransferDate),
        ),
        AppDetailRow(label: 'Geçeceği hesap', value: current.accountName),
      ],
    );
  }

  List<Widget> _manualFields() => [
    AppAccountField(
      label: 'Paranın geçeceği hesap',
      value: _accountId,
      options: [
        for (final account in _bankAccounts)
          AccountChoices.of(
            context,
          ).account(account.id, account.name, type: 'bank'),
      ],
      emptyTitle: 'Henüz banka hesabı yok',
      emptyMessage: 'POS parası bir banka hesabına geçer; önce onu ekleyin.',
      onAdd: () async {
        final created = await addAccountFromForm(context, type: 'bank');
        if (created == null || created.type != 'bank' || !mounted) return;
        setState(() {
          _bankAccounts = [..._bankAccounts, created.choice];
          _accountId = created.id;
        });
      },
      onChanged: (value) => setState(() => _accountId = value),
      validator: (value) => value == null ? 'Hesap seçin.' : null,
    ),
    const SizedBox(height: AppSpacing.medium),
    AppDateField(
      label: 'Paranın beklendiği gün',
      value: _expected ?? widget.collectedOn,
      firstDate: AppDateField.parse(widget.collectedOn),
      onChanged: (value) => setState(() => _expected = value),
    ),
    const SizedBox(height: AppSpacing.medium),
    // POS tahsilatıyla aynı parçalı seçici.
    AppChoiceSegments<_CommissionMode>(
      label: 'Komisyon',
      values: _CommissionMode.values,
      selected: _mode,
      labelOf: (mode) => switch (mode) {
        _CommissionMode.none => 'Yok',
        _CommissionMode.amount => 'Tutar',
        _CommissionMode.rate => 'Oran',
      },
      onChanged: (value) => setState(() {
        _mode = value;
        _commission.clear();
      }),
    ),
    if (_mode != _CommissionMode.none) ...[
      const SizedBox(height: AppSpacing.medium),
      TextFormField(
        controller: _commission,
        keyboardType: const TextInputType.numberWithOptions(decimal: true),
        decoration: InputDecoration(
          labelText: _mode == _CommissionMode.amount
              ? 'Komisyon tutarı'
              : 'Komisyon oranı (%)',
          helperText: _mode == _CommissionMode.amount
              ? 'Tahsil günü ayrı bir gider olarak yazılır.'
              : 'Örnek: 1,5 yazın. Oran saklanmaz, tutara çevrilir.',
        ),
        validator: _mode == _CommissionMode.amount
            ? MoneyInput.positiveError
            : (value) => PosRate.fractionWire(value ?? '') == null
                  ? 'Oranı 0 ile 100 arasında yazın.'
                  : null,
      ),
      const SizedBox(height: AppSpacing.medium),
      FixedScopeCategoryField(
        label: 'Komisyon gider kategorisi',
        type: 'expense',
        categories: widget.source.expenseCategories,
        value: _commissionCategoryId,
        onChanged: (value) => setState(() => _commissionCategoryId = value),
        validator: (value) =>
            value == null ? 'Komisyon için kategori seçin.' : null,
      ),
    ],
  ];
}

bool _isZero(String value) =>
    !value.replaceAll(RegExp('[^0-9]'), '').contains(RegExp('[1-9]'));
