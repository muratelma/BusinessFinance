import 'package:flutter/material.dart';

import '../theme/app_radius.dart';

/// Tarih seçen form alanı.
///
/// Tarih her formda çıplak bir `ListTile` olarak duruyordu: yanındaki dolgulu,
/// kenarlıklı metin alanlarıyla aynı türden görünmüyor, dokunulabilir olduğu
/// da anlaşılmıyordu. `InputDecorator` sayesinde alan gerçek girdi alanlarıyla
/// **aynı** dekorasyonu (dolgu, kenarlık, etiket, yardımcı metin, hata) tema
/// üzerinden alır; burada ayrı bir görünüm tanımlanmaz.
///
/// Değer `yyyy-MM-dd` metnidir: API sözleşmesinin biçimi budur ve formlar
/// arada `DateTime`'a çevirip geri dönmek zorunda kalmaz.
class AppDateField extends StatelessWidget {
  const AppDateField({
    required this.label,
    required this.value,
    required this.onChanged,
    super.key,
    this.helperText,
    this.placeholder = 'Seçilmedi',
    this.firstDate,
    this.lastDate,
    this.enabled = true,
  });

  final String label;

  /// `yyyy-MM-dd` veya null (henüz seçilmedi).
  final String? value;

  final String? helperText;

  /// Değer yokken gösterilen metin.
  final String placeholder;

  final DateTime? firstDate;
  final DateTime? lastDate;
  final bool enabled;

  final ValueChanged<String> onChanged;

  static DateTime? parse(String? value) {
    if (value == null) return null;
    final parts = value.split('-');
    if (parts.length != 3) return null;
    final year = int.tryParse(parts[0]);
    final month = int.tryParse(parts[1]);
    final day = int.tryParse(parts[2]);
    if (year == null || month == null || day == null) return null;
    return DateTime(year, month, day);
  }

  static String format(DateTime value) =>
      '${value.year.toString().padLeft(4, '0')}-'
      '${value.month.toString().padLeft(2, '0')}-'
      '${value.day.toString().padLeft(2, '0')}';

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final hasValue = value != null && value!.isNotEmpty;

    return Semantics(
      button: enabled,
      label: '$label: ${hasValue ? value! : placeholder}',
      excludeSemantics: true,
      child: InkWell(
        onTap: enabled ? () => _pick(context) : null,
        borderRadius: BorderRadius.circular(AppRadius.field),
        child: InputDecorator(
          isEmpty: !hasValue,
          decoration: InputDecoration(
            labelText: label,
            helperText: helperText,
            enabled: enabled,
            suffixIcon: const Icon(Icons.calendar_today_outlined),
          ),
          child: Text(
            hasValue ? value! : placeholder,
            style: hasValue
                ? null
                : theme.textTheme.bodyMedium?.copyWith(
                    color: theme.colorScheme.onSurfaceVariant,
                  ),
          ),
        ),
      ),
    );
  }

  Future<void> _pick(BuildContext context) async {
    final picked = await showDatePicker(
      context: context,
      initialDate: parse(value) ?? DateTime.now(),
      firstDate: firstDate ?? DateTime(2000),
      lastDate: lastDate ?? DateTime(2100),
    );
    if (picked != null) onChanged(format(picked));
  }
}
