import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';

/// Sıra atlamanın onayı. Bekleyen kalemi sunucu söyler
/// (`ApiException.isEarlierItemPending`); ekrandaki liste yalnız 30 ya da 90
/// günü gösterir ve ona bakılmaz.
///
/// Onaylanırsa yalnız seçilen kalem kapanır; önceki bekliyor olarak kalır.
Future<bool> confirmSkippingEarlierItem(
  BuildContext context, {
  required String title,
  required String dueDate,
  required String paidOn,
  String? pendingDate,
  int? pendingCount,
}) => AppConfirmDialog.show(
  context: context,
  icon: Icons.low_priority,
  title: 'Önceki kalem bekliyor',
  highlight: '$title\nvade ${DateText.dayMonth(dueDate)}',
  message:
      '${_pendingSentence(pendingDate, pendingCount)}\n'
      'Kayıt günü: ${DateText.dayMonthYear(paidOn)}.',
  confirmLabel: 'Yine de gerçekleştir',
);

/// Hangi kalemin beklediğini söyler. Günü sunucu gönderir; göndermediyse
/// (eski sunucu) kalem adı verilmeden sorulur.
String _pendingSentence(String? pendingDate, int? pendingCount) {
  if (pendingDate == null) {
    return 'Bu plandan daha önceki bir kalem hâlâ bekliyor. Bu kayıt yalnız '
        'seçtiğiniz kalemi kapatır; önceki kalem beklemeye devam eder.';
  }
  final day = DateText.dayMonth(pendingDate);
  if ((pendingCount ?? 1) > 1) {
    return 'Bu planın daha önceki $pendingCount kalemi hâlâ bekliyor; ilki '
        '$day vadeli. Bu kayıt yalnız seçtiğiniz kalemi kapatır; öncekiler '
        'beklemeye devam eder.';
  }
  return 'Bu planın $day vadeli kalemi hâlâ bekliyor. Bu kayıt yalnız '
      'seçtiğiniz kalemi kapatır; o beklemeye devam eder.';
}

/// `Gerçekleştir` paneli: tekrarlayan kalemin hangi gün gerçekleştiğini sorar.
///
/// Kayıt vadeye değil ödendiği güne yazılır; bu yüzden vadesi gelmemiş kalem
/// de buradan gerçekleştirilir (kirayı erken ödeyen) ve geçmiş bir gün
/// seçilebilir (dün ödeyip bugün giren). Gün ileride olamaz. Tutar ve kaynak
/// planındır; burada yalnız gösterilir.
///
/// Seçilen günü (`yyyy-MM-dd`) döner; vazgeçilirse `null`.
Future<String?> showRealizeDaySheet(
  BuildContext context, {
  required String title,
  required String amount,
  required String currency,
  required String dueDate,
  String? sourceName,
  bool isIncome = false,
  DateTime? today,
}) => AppFormSheet.show<String>(
  context: context,
  builder: (_) => _RealizeDaySheet(
    title: title,
    amount: amount,
    currency: currency,
    dueDate: dueDate,
    sourceName: sourceName,
    isIncome: isIncome,
    today: DateUtils.dateOnly(today ?? DateTime.now()),
  ),
);

class _RealizeDaySheet extends StatefulWidget {
  const _RealizeDaySheet({
    required this.title,
    required this.amount,
    required this.currency,
    required this.dueDate,
    required this.sourceName,
    required this.isIncome,
    required this.today,
  });

  final String title;
  final String amount;
  final String currency;
  final String dueDate;
  final String? sourceName;
  final bool isIncome;
  final DateTime today;

  @override
  State<_RealizeDaySheet> createState() => _RealizeDaySheetState();
}

class _RealizeDaySheetState extends State<_RealizeDaySheet> {
  late final String _todayIso = AppDateField.format(widget.today);

  // Ön değer her kalemde bugündür: gecikmiş kira bugün ödeniyorsa vade
  // gününe, yani geçmiş aya yazılmamalı.
  late String _day = _todayIso;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return AppFormSheet<String>(
      title: widget.title,
      description: [
        MoneyText.format(widget.amount, widget.currency),
        ?widget.sourceName,
        'vade ${DateText.dayMonth(widget.dueDate)}',
      ].join(' · '),
      submitLabel: 'Gerçekleştir',
      onSubmit: () async => _day,
      children: [
        AppDateField(
          label: widget.isIncome ? 'Tahsilat günü' : 'Ödeme günü',
          value: _day,
          valueText: _day == _todayIso
              ? '${DateText.dayMonthYear(_day)} · bugün'
              : DateText.dayMonthYear(_day),
          lastDate: widget.today,
          onChanged: (value) => setState(() => _day = value),
        ),
        const SizedBox(height: AppSpacing.small),
        Text(
          'Kayıt seçtiğiniz güne yazılır ve bakiyeye girer.',
          style: theme.textTheme.bodySmall?.copyWith(
            color: theme.colorScheme.onSurfaceVariant,
          ),
        ),
      ],
    );
  }
}
