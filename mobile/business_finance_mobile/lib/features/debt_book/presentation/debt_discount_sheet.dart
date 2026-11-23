import 'package:flutter/material.dart';

import '../../../core/formatters/money_math.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/network/client_request_id.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../data/debt_book_models.dart';
import 'debt_book_parts.dart';

/// Kişinin hesabındaki bir satış ya da alıma indirim (`POST
/// /api/v1/counterparty-charges/{id}/discounts`). Bedel düşer; gelir ya da
/// gider kaydın gününde azalır, para hareket etmez.
class DebtDiscountSheet extends StatefulWidget {
  const DebtDiscountSheet({
    required this.title,
    required this.amount,
    required this.isSale,
    required this.today,
    required this.onSubmit,
    super.key,
  });

  /// Kaydın adı ve bedeli (başlığın altında).
  final String title;
  final String amount;
  final bool isSale;
  final DateTime today;

  /// Yazıldıysa `null`, reddedildiyse formda gösterilecek cümle.
  final Future<String?> Function(
    String amount,
    String date,
    String clientRequestId,
  )
  onSubmit;

  @override
  State<DebtDiscountSheet> createState() => _DebtDiscountSheetState();
}

class _DebtDiscountSheetState extends State<DebtDiscountSheet> {
  final TextEditingController _amount = TextEditingController();
  final String _requestId = newClientRequestId();
  late String _date =
      '${widget.today.year.toString().padLeft(4, '0')}-'
      '${widget.today.month.toString().padLeft(2, '0')}-'
      '${widget.today.day.toString().padLeft(2, '0')}';
  String? _error;

  @override
  void dispose() {
    _amount.dispose();
    super.dispose();
  }

  Future<bool?> _submit() async {
    final amount = MoneyText.normalizeInput(_amount.text);
    if (amount == null || !isPositiveAmount(amount)) {
      setState(() => _error = 'İndirim tutarını yazın.');
      return null;
    }
    final error = await widget.onSubmit(amount, _date, _requestId);
    if (error != null) {
      setState(() => _error = error);
      return null;
    }
    return true;
  }

  @override
  Widget build(BuildContext context) => AppFormSheet<bool>(
    title: 'İndirim',
    description: '${widget.title} · ${MoneyText.format(widget.amount, 'TRY')}',
    submitLabel: 'Kaydet',
    onSubmit: _submit,
    errorMessage: _error,
    children: [
      AppFormField(
        child: TextFormField(
          controller: _amount,
          autofocus: true,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          inputFormatters: const [TurkishAmountInputFormatter()],
          decoration: const InputDecoration(
            labelText: 'İndirim tutarı',
            prefixText: '₺',
          ),
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
      DebtFieldNote(
        widget.isSale
            ? 'Satışın geliri kendi gününde azalır; para hareket etmez.'
            : 'Alımın gideri kendi gününde azalır; para hareket etmez.',
      ),
    ],
  );
}
