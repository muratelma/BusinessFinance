import 'package:flutter/material.dart';

import '../formatters/date_text.dart';
import '../theme/app_breakpoints.dart';
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
    this.valueText,
    this.iconLeading = false,
  });

  /// Takvim simgesi değerin solunda durur (tasarımdaki panel alanları).
  /// Varsayılan sağdadır; diğer formlar öyle çizilmiştir.
  final bool iconLeading;

  /// Değerin ekrandaki hâli (`29 Eylül 2026 · bugün`); verilmezse gün, ayın
  /// adı ve yıl yazılır (`28 Ekim 2026`).
  final String? valueText;

  final String label;

  /// `yyyy-MM-dd` veya null (henüz seçilmedi).
  final String? value;

  final String? helperText;

  /// Değer yokken gösterilen metin.
  final String placeholder;

  final DateTime? firstDate;

  /// Seçilebilecek en ileri gün. Olmuş bir kaydın günü ileride olamaz
  /// (gelir, gider, harcama, ödeme, tahsilat, transfer): o formlar bugünü
  /// verir. Vade, plan başlangıcı ve hedef tarihi boş bırakır.
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
    final shown = hasValue
        ? (valueText ?? DateText.dayMonthYear(value!))
        : placeholder;

    return Semantics(
      button: enabled,
      label: '$label: $shown',
      excludeSemantics: true,
      child: InkWell(
        onTap: enabled ? () => _pick(context) : null,
        borderRadius: BorderRadius.circular(AppRadius.field),
        child: InputDecorator(
          isEmpty: !hasValue,
          decoration: InputDecoration(
            labelText: label,
            // Değer yokken de yer tutucu ("Seçilmedi") içerik yerinde
            // çizildiği için etiket her zaman üstte durur; aksi hâlde ikisi
            // üst üste biniyordu (vergi araştırması V-U6).
            floatingLabelBehavior: FloatingLabelBehavior.always,
            helperText: helperText,
            enabled: enabled,
            prefixIcon: iconLeading ? const Icon(Icons.event_outlined) : null,
            suffixIcon: iconLeading
                ? null
                : const Icon(Icons.calendar_today_outlined),
          ),
          // Metin alanının satır yüksekliği (24): alan diğerleri gibi 56 dp.
          child: SizedBox(
            height: context.usesLargeText ? null : 24,
            child: Align(
              alignment: Alignment.centerLeft,
              child: Text(
                shown,
                style: hasValue
                    ? null
                    : theme.textTheme.bodyMedium?.copyWith(
                        color: theme.colorScheme.onSurfaceVariant,
                      ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _pick(BuildContext context) async {
    final first = DateUtils.dateOnly(firstDate ?? DateTime(2000));
    final last = DateUtils.dateOnly(lastDate ?? DateTime(2100));
    // Değer aralığın dışında gelebilir (belgeden okunan ileri bir tarih);
    // takvim aralığın dışındaki bir günle açılamaz.
    var initial = parse(value) ?? DateTime.now();
    if (initial.isAfter(last)) initial = last;
    if (initial.isBefore(first)) initial = first;
    final picked = await showDatePicker(
      context: context,
      initialDate: initial,
      firstDate: first,
      lastDate: last,
    );
    if (picked != null) onChanged(format(picked));
  }
}
