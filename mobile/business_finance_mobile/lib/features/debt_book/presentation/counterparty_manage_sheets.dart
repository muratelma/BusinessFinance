import 'package:flutter/material.dart';

import '../../../core/formatters/money_math.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_filter_sheet.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../data/debt_book_models.dart';
import 'debt_book_parts.dart';

/// Kişinin `⋮` menüsündeki `Düzenle`: ad, not ve (kapsam görünen
/// kullanıcıda) tür. Tür listeyi böler, tutarları değiştirmez.
class CounterpartyEditSheet extends StatefulWidget {
  const CounterpartyEditSheet({
    required this.page,
    required this.scopeVisible,
    required this.onSubmit,
    super.key,
  });

  final CounterpartyPageData page;
  final bool scopeVisible;

  /// Yazıldıysa `null`, reddedildiyse formda gösterilecek cümle.
  final Future<String?> Function(
    String name,
    String? note,
    TransactionScope? scope,
  )
  onSubmit;

  @override
  State<CounterpartyEditSheet> createState() => _CounterpartyEditSheetState();
}

class _CounterpartyEditSheetState extends State<CounterpartyEditSheet> {
  late final TextEditingController _name = TextEditingController(
    text: widget.page.name,
  );
  late final TextEditingController _note = TextEditingController(
    text: widget.page.note ?? '',
  );

  /// Türü boş eski kişide seçilene kadar boş kalır; uydurulmaz.
  late TransactionScope? _scope = widget.page.scope;
  String? _error;

  @override
  void dispose() {
    _name.dispose();
    _note.dispose();
    super.dispose();
  }

  Future<bool?> _submit() async {
    final name = _name.text.trim();
    if (name.isEmpty) {
      setState(() => _error = 'Kişinin adını yazın.');
      return null;
    }
    final note = _note.text.trim();
    final error = await widget.onSubmit(
      name,
      note.isEmpty ? null : note,
      widget.scopeVisible ? _scope : null,
    );
    if (error != null) {
      if (mounted) setState(() => _error = error);
      return null;
    }
    return true;
  }

  @override
  Widget build(BuildContext context) => AppFormSheet<bool>(
    title: 'Kişiyi düzenle',
    submitLabel: 'Kaydet',
    onSubmit: _submit,
    errorMessage: _error,
    children: [
      AppFormField(
        child: TextFormField(
          controller: _name,
          textCapitalization: TextCapitalization.words,
          decoration: const InputDecoration(labelText: 'Ad'),
        ),
      ),
      AppFormField(
        child: TextFormField(
          controller: _note,
          maxLines: 2,
          decoration: const InputDecoration(labelText: 'Not (isteğe bağlı)'),
        ),
      ),
      if (widget.scopeVisible) ...[
        AppFormField(
          // Türü boş eski kişide hiçbir dilim seçili gelmez.
          child: AppFilterSegments<TransactionScope?>(
            values: TransactionScope.values,
            selected: _scope,
            label: (value) => value!.label,
            onChanged: (value) => setState(() => _scope = value),
            semanticLabel: 'Kişinin türü',
          ),
        ),
        const DebtFieldNote(
          'Tür listeyi böler; tutarları ve raporu değiştirmez.',
        ),
      ],
    ],
  );
}

/// Kişinin `⋮` menüsündeki `Açılış bakiyesi`: uygulamadan önce oluşmuş
/// alacak ve borç. Gelir ya da gider yazmaz; kişinin hesabına en eski kalem
/// olarak girer. Sıfır devri kaldırır.
class CounterpartyOpeningSheet extends StatefulWidget {
  const CounterpartyOpeningSheet({
    required this.page,
    required this.onSubmit,
    super.key,
  });

  final CounterpartyPageData page;

  /// Yazıldıysa `null`, reddedildiyse formda gösterilecek cümle.
  final Future<String?> Function(String receivable, String payable) onSubmit;

  @override
  State<CounterpartyOpeningSheet> createState() =>
      _CounterpartyOpeningSheetState();
}

class _CounterpartyOpeningSheetState extends State<CounterpartyOpeningSheet> {
  late final TextEditingController _receivable = TextEditingController(
    text: _initial(widget.page.openingReceivable),
  );
  late final TextEditingController _payable = TextEditingController(
    text: _initial(widget.page.openingPayable),
  );
  String? _error;

  static String _initial(String amount) =>
      isPositiveAmount(amount) ? MoneyText.field(amount) : '';

  @override
  void dispose() {
    _receivable.dispose();
    _payable.dispose();
    super.dispose();
  }

  /// Boş alan sıfırdır; yazılmış ama okunamayan alan reddedilir.
  String? _read(TextEditingController controller) {
    if (controller.text.trim().isEmpty) return '0.0000';
    return MoneyText.normalizeInput(controller.text);
  }

  Future<bool?> _submit() async {
    final receivable = _read(_receivable);
    final payable = _read(_payable);
    if (receivable == null || payable == null) {
      setState(() => _error = 'Tutarı rakamla yazın.');
      return null;
    }
    final error = await widget.onSubmit(receivable, payable);
    if (error != null) {
      if (mounted) setState(() => _error = error);
      return null;
    }
    return true;
  }

  @override
  Widget build(BuildContext context) => AppFormSheet<bool>(
    title: 'Açılış bakiyesi',
    description: widget.page.name,
    submitLabel: 'Kaydet',
    onSubmit: _submit,
    errorMessage: _error,
    children: [
      AppFormField(
        child: TextFormField(
          controller: _receivable,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          inputFormatters: const [TurkishAmountInputFormatter()],
          decoration: const InputDecoration(
            labelText: 'Size borcu',
            prefixText: '₺',
          ),
        ),
      ),
      AppFormField(
        child: TextFormField(
          controller: _payable,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          inputFormatters: const [TurkishAmountInputFormatter()],
          decoration: const InputDecoration(
            labelText: 'Sizin borcunuz',
            prefixText: '₺',
          ),
        ),
      ),
      const SizedBox(height: AppSpacing.compact),
      const DebtFieldNote(
        'Uygulamayı kullanmaya başlamadan önceki bakiye. Gelir ya da gider '
        'yazılmaz; ödemeler önce bunu kapatır.',
      ),
    ],
  );
}
