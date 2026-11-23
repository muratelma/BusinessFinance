import 'package:flutter/material.dart';

import '../../../core/formatters/money_math.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/network/client_request_id.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_account_field.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../data/debt_book_models.dart';
import 'debt_book_parts.dart';

/// Kişinin hesabına tahsilat ya da ödeme (`05-EKRAN-TASARIMI.md` §3.3). Tek
/// panel, tek adım; hedef sormaz (K6). Kısayol tutarları sunucudandır ve
/// yalnız kişinin hesabınındır.
class DebtSettleSheet extends StatefulWidget {
  const DebtSettleSheet({
    required this.side,
    required this.personName,
    required this.shortcuts,
    required this.options,
    required this.today,
    required this.onSubmit,
    super.key,
    this.initialAccountId,
    this.initialAmount,
    this.initialDiscount = false,
    this.subtitle,
    this.chips,
    this.fixedAmount,
    this.overLimitNote,
    this.allowDiscount = true,
  });

  /// Başlığın altındaki satır; verilmezse kişinin adı ve kalan.
  final String? subtitle;

  /// Tutar kısayolları (ad, tutar); verilmezse `Tamamı` ve `Gecikmiş`.
  final List<(String, String)>? chips;

  /// Faizli kayıtta yalnız tam taksit ödenir: tutar sabittir, alan kapalı.
  final String? fixedAmount;

  /// Tutar kalanı aşınca alanın altındaki not; verilmezse kişinin hesabının
  /// notu (fazlası karşı yönde kalır).
  final String? overLimitNote;

  /// `Kalanı indirim say` yalnız kişinin hesabında.
  final bool allowDiscount;

  /// `receivable` tahsilat, `payable` ödeme.
  final DebtSide side;
  final String personName;
  final DebtShortcuts shortcuts;

  /// Tahsilatta hesaplar ve POS'lar, ödemede hesaplar ve kredi kartları.
  final List<AppAccountOption> options;
  final DateTime today;
  final String? initialAccountId;

  /// Sunucu biçiminde (`3500.0000`); verilirse alan dolu açılır.
  final String? initialAmount;
  final bool initialDiscount;

  /// Yazıldıysa `null`, reddedildiyse formun içinde gösterilecek cümle.
  final Future<String?> Function(DebtSettlement settlement) onSubmit;

  @override
  State<DebtSettleSheet> createState() => _DebtSettleSheetState();
}

/// Panelin gönderdiği kayıt; istek gövdesini depo kurar.
class DebtSettlement {
  const DebtSettlement({
    required this.side,
    required this.amount,
    required this.date,
    required this.sourceId,
    required this.clientRequestId,
    this.description,
    this.closeRemainderAsDiscount = false,
  });

  final DebtSide side;

  /// Dört ondalıklı sunucu biçimi.
  final String amount;

  /// `yyyy-MM-dd`.
  final String date;

  /// `account:…`, `pos:…` ya da `card:…`.
  final String sourceId;
  final String? description;
  final bool closeRemainderAsDiscount;

  /// Panel açılırken bir kez üretilir: yinelenen gönderim ikinci kayıt
  /// yazmaz.
  final String clientRequestId;
}

class _DebtSettleSheetState extends State<DebtSettleSheet> {
  late final TextEditingController _amount = TextEditingController(
    text: (widget.fixedAmount ?? widget.initialAmount) == null
        ? null
        : MoneyText.field((widget.fixedAmount ?? widget.initialAmount)!),
  );
  final TextEditingController _description = TextEditingController();
  final String _requestId = newClientRequestId();
  late String _date = _iso(widget.today);
  late String? _source = widget.initialAccountId;
  late bool _discount = widget.initialDiscount;
  String? _error;

  bool get _isCollect => widget.side == DebtSide.receivable;

  static String _iso(DateTime day) =>
      '${day.year.toString().padLeft(4, '0')}-'
      '${day.month.toString().padLeft(2, '0')}-'
      '${day.day.toString().padLeft(2, '0')}';

  @override
  void dispose() {
    _amount.dispose();
    _description.dispose();
    super.dispose();
  }

  /// Yazılan tutar sunucu biçiminde; okunamıyorsa `null`.
  String? get _entered => MoneyText.normalizeInput(_amount.text);

  /// Yazılan tutarın kalanla karşılaştırması (hesap değil, karşılaştırma).
  int? get _comparedToFull {
    final entered = double.tryParse(_entered ?? '');
    final full = double.tryParse(widget.shortcuts.full);
    if (entered == null || full == null || entered <= 0) return null;
    return entered.compareTo(full);
  }

  void _use(String amount) => setState(() {
    _amount.text = MoneyText.field(amount);
  });

  Future<bool?> _submit() async {
    final amount = _entered;
    final source = _source;
    if (amount == null || source == null) {
      setState(() => _error = 'Tutarı ve hesabı seçin.');
      return null;
    }
    final error = await widget.onSubmit(
      DebtSettlement(
        side: widget.side,
        amount: amount,
        date: _date,
        sourceId: source,
        clientRequestId: _requestId,
        description: _description.text.trim().isEmpty
            ? null
            : _description.text.trim(),
        closeRemainderAsDiscount:
            widget.allowDiscount &&
            !_isCollect &&
            _discount &&
            (_comparedToFull ?? 0) < 0,
      ),
    );
    if (error != null) {
      setState(() => _error = error);
      return null;
    }
    return true;
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final shortcuts = widget.shortcuts;
    final compared = _comparedToFull;
    final full = MoneyText.format(shortcuts.full, 'TRY');
    final chips =
        widget.chips ??
        [
          if (shortcuts.hasFull) ('Tamamı', shortcuts.full),
          if (shortcuts.hasOverdue && shortcuts.overdue != shortcuts.full)
            ('Gecikmiş', shortcuts.overdue),
        ];
    final fixed = widget.fixedAmount != null;
    return AppFormSheet<bool>(
      title: _isCollect ? 'Tahsilat' : 'Ödeme',
      description:
          widget.subtitle ??
          (shortcuts.hasFull
              ? '${widget.personName} · '
                    '${_isCollect ? 'alacağınız' : 'borcunuz'} $full'
              : widget.personName),
      submitLabel: 'Kaydet',
      onSubmit: _submit,
      errorMessage: _error,
      children: [
        AppFormField(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              TextFormField(
                controller: _amount,
                keyboardType: const TextInputType.numberWithOptions(
                  decimal: true,
                ),
                inputFormatters: const [TurkishAmountInputFormatter()],
                onChanged: (_) => setState(() {}),
                readOnly: fixed,
                decoration: InputDecoration(
                  labelText: 'Tutar',
                  prefixText: '₺',
                  helperText: fixed
                      ? 'Faizli kayıtta tam taksit ödenir.'
                      : null,
                ),
              ),
              if (!fixed && chips.isNotEmpty) ...[
                const SizedBox(height: AppSpacing.small),
                Wrap(
                  spacing: AppSpacing.small,
                  runSpacing: AppSpacing.small,
                  children: [
                    for (final (label, amount) in chips)
                      ActionChip(
                        label: Text(
                          '$label ${MoneyText.format(amount, 'TRY')}',
                        ),
                        onPressed: () => _use(amount),
                      ),
                  ],
                ),
              ],
              if (compared != null && compared > 0) ...[
                const SizedBox(height: AppGaps.notice),
                DebtFieldNote(
                  widget.overLimitNote ??
                      (_isCollect
                          ? 'Kalandan fazla; fazlası kişinin sizdeki alacağı '
                                'olur.'
                          : 'Kalandan fazla; fazlası sizin alacağınız olur.'),
                ),
              ],
            ],
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'Gün',
            value: _date,
            lastDate: widget.today,
            onChanged: (value) => setState(() => _date = value),
          ),
        ),
        AppFormField(
          child: AppAccountField(
            label: _isCollect ? 'Hesap ya da POS' : 'Hesap ya da kart',
            options: widget.options,
            value: _source,
            onChanged: (value) => setState(() => _source = value),
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
        if (widget.allowDiscount &&
            !_isCollect &&
            compared != null &&
            compared < 0)
          MergeSemantics(
            child: Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Kalanı indirim say',
                        style: theme.textTheme.bodyLarge,
                      ),
                      const SizedBox(height: AppGaps.rowText),
                      Text(
                        'Kalan tutar alım indirimi olarak kapanır.',
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: surfaces.inkMuted,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: AppSpacing.compact),
                Switch(
                  value: _discount,
                  onChanged: (value) => setState(() => _discount = value),
                ),
              ],
            ),
          ),
      ],
    );
  }
}
