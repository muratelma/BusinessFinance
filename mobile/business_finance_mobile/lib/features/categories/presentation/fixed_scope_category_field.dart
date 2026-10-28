import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/widgets/app_category_field.dart';
import 'category_create_page.dart';
import 'recent_categories.dart';

/// Kapsamı ekranın anlamından gelen kategori alanı (POS, gün sonu, yatış
/// kesintisi, cari: `İşletme`).
///
/// Yalnız o kapsamın ve iki kapsama açık kategoriler listelenir, kapsam
/// sorulmaz; değer yalnız kategori kimliğidir. `+ "…" ekle`den dönülen
/// kategoriyi alan kendisi tutar ve seçer, çağıranın listesi değişmez.
class FixedScopeCategoryField extends StatefulWidget {
  const FixedScopeCategoryField({
    required this.label,
    required this.type,
    required this.categories,
    required this.value,
    required this.onChanged,
    super.key,
    this.scope = TransactionScope.business,
    this.validator,
    this.helperText,
  });

  final String label;

  /// `income` ya da `expense`.
  final String type;
  final List<DataChoice> categories;
  final String? value;
  final ValueChanged<String?> onChanged;
  final TransactionScope scope;
  final FormFieldValidator<String>? validator;
  final String? helperText;

  @override
  State<FixedScopeCategoryField> createState() =>
      _FixedScopeCategoryFieldState();
}

class _FixedScopeCategoryFieldState extends State<FixedScopeCategoryField> {
  final _added = <DataChoice>[];

  @override
  Widget build(BuildContext context) {
    final known = {for (final c in widget.categories) c.id};
    final value = widget.value;
    return AppCategoryField(
      label: widget.label,
      helperText: widget.helperText,
      options: [
        for (final category in [
          ...widget.categories,
          ..._added.where((c) => !known.contains(c.id)),
        ])
          AppCategoryOption(
            id: category.id,
            name: category.name,
            scope: category.defaultScope,
          ),
      ],
      value: value == null ? null : AppCategoryValue(value, widget.scope),
      recent: RecentCategories.of(context, widget.type),
      showScope: _showScope(context),
      fixedScope: widget.scope,
      validator: widget.validator == null
          ? null
          : (selected) => widget.validator!(selected?.categoryId),
      onChanged: (selected) => widget.onChanged(selected?.categoryId),
      onAdd: (name) async {
        final created = await addCategoryFromForm(
          context,
          type: widget.type,
          name: name,
        );
        if (created == null || !mounted) return;
        setState(() => _added.add(created.choice));
        widget.onChanged(created.id);
      },
    );
  }

  static bool _showScope(BuildContext context) {
    try {
      return context.watch<ScopeController?>()?.isVisible ?? false;
    } on ProviderNotFoundException {
      return false;
    }
  }
}
