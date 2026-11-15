import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_animated_menu.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_pick_field.dart';
import '../data/card_page_models.dart';

/// Taksit sayısı menüsünün sık sayıları (teslim §8).
const installmentCountChoices = [2, 3, 4, 6, 9, 12];

/// Taksitli harcama panelindeki taksit sayısı alanı (teslim §8, A): menü
/// sık sayıları ve her birinin aylık tutarını gösterir, en altta `Diğer
/// sayı`. Menü iki alanın genişliğindedir ([menuAnchorKey] o satırın
/// anahtarıdır). `Diğer sayı` seçilince alan yazılabilir olur (2–60) ve
/// altında yardım yazısı yoktur. Aylık tutarlar sunucudandır.
class InstallmentCountField extends StatefulWidget {
  const InstallmentCountField({
    required this.count,
    required this.other,
    required this.otherController,
    required this.previews,
    required this.onCount,
    required this.onOther,
    required this.menuAnchorKey,
    super.key,
  });

  final int count;
  final bool other;
  final TextEditingController otherController;
  final List<InstallmentPreviewOption> previews;
  final ValueChanged<int> onCount;
  final VoidCallback onOther;
  final GlobalKey menuAnchorKey;

  @override
  State<InstallmentCountField> createState() => _InstallmentCountFieldState();
}

class _InstallmentCountFieldState extends State<InstallmentCountField> {
  String? _amountOf(int count) => widget.previews
      .where((p) => p.count == count)
      .firstOrNull
      ?.installmentAmount;

  @override
  Widget build(BuildContext context) {
    if (widget.other) {
      return TextFormField(
        controller: widget.otherController,
        autofocus: true,
        keyboardType: TextInputType.number,
        decoration: const InputDecoration(labelText: 'Taksit sayısı (2–60)'),
        validator: (v) {
          final n = int.tryParse(v ?? '');
          return n == null || n < 2 || n > 60 ? '2–60 arası girin.' : null;
        },
      );
    }
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final rowStyle = theme.textTheme.bodyLarge?.copyWith(
      fontSize: AppTextSizes.compact,
      height: 1.2,
    );
    return AppAnimatedMenu(
      // Menü dar alanın değil bütün satırın genişliğindedir.
      spanKey: widget.menuAnchorKey,
      expectedHeight:
          (installmentCountChoices.length + 1) * 48.0 +
          AppSpacing.medium +
          AppSpacing.small +
          1,
      menuBuilder: (context, close) => Padding(
        padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            for (final n in installmentCountChoices)
              AppPickMenuRow(
                minHeight: 48,
                selected: n == widget.count,
                semanticsLabel: [
                  '$n taksit',
                  if (_amountOf(n) != null)
                    'ayda ${MoneyText.format(_amountOf(n)!, 'TRY')}',
                ].join(', '),
                onPressed: () {
                  close();
                  widget.onCount(n);
                },
                child: Row(
                  children: [
                    Expanded(
                      child: Text(
                        '$n taksit',
                        style: rowStyle?.copyWith(
                          fontWeight: n == widget.count
                              ? FontWeight.w700
                              : FontWeight.w400,
                        ),
                      ),
                    ),
                    if (_amountOf(n) != null)
                      Text(
                        '${MoneyText.format(_amountOf(n)!, 'TRY')} / ay',
                        style: theme.textTheme.bodyMedium?.copyWith(
                          fontWeight: n == widget.count
                              ? FontWeight.w700
                              : FontWeight.w500,
                          color: n == widget.count
                              ? surfaces.ink
                              : surfaces.inkMuted,
                          fontFeatures: AppTypography.tabularFigures,
                        ),
                      ),
                  ],
                ),
              ),
            Divider(height: AppSpacing.small + 1, color: surfaces.border),
            AppPickMenuRow(
              minHeight: 48,
              selected: false,
              semanticsLabel: 'Diğer sayı, 2 ile 60 arası',
              onPressed: () {
                close();
                widget.onOther();
              },
              child: Row(
                children: [
                  Icon(Icons.edit_outlined, size: 20, color: surfaces.ink),
                  const SizedBox(width: AppSpacing.compact),
                  Expanded(child: Text('Diğer sayı', style: rowStyle)),
                  Text(
                    '2–60',
                    style: theme.textTheme.bodySmall?.copyWith(
                      fontSize: AppTextSizes.caption,
                      color: surfaces.inkMuted,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
      builder: (context, open, toggle) => Semantics(
        button: true,
        expanded: open,
        label: 'Taksit sayısı',
        value: '${widget.count} taksit',
        child: ExcludeSemantics(
          child: InkWell(
            onTap: toggle,
            borderRadius: BorderRadius.circular(AppRadius.field),
            child: InputDecorator(
              isFocused: open,
              decoration: InputDecoration(
                labelText: 'Taksit sayısı',
                suffixIcon: AppMenuArrow(open: open),
              ),
              child: Text(
                '${widget.count}',
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: AppTextSizes.row,
                  height: 1.2,
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Mavi önizleme kutusu (teslim §8): `Aylık taksit ₺722,22 × 9`, altında
/// taksit çubuğu, uçlarda ilk ve son taksit günü. `Son taksit` satırı yok.
/// Tutar sunucudandır; önizleme yoksa kutu çizilmez.
class InstallmentPreviewBox extends StatelessWidget {
  const InstallmentPreviewBox({required this.preview, super.key});
  final InstallmentPreviewOption preview;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final ink = colors.onNeutralContainer;
    final small = theme.textTheme.bodySmall?.copyWith(
      fontSize: AppTextSizes.caption,
      height: 1.3,
      color: ink,
    );
    final amount = AppMoneyText(
      amount: preview.installmentAmount,
      currency: 'TRY',
      effect: AppMoneyEffect.neutral,
      onContainer: true,
      size: AppMoneySize.row,
      style: const TextStyle(
        fontSize: AppTextSizes.metric,
        fontWeight: FontWeight.w700,
        height: 1.2,
      ),
    );
    return Semantics(
      container: true,
      label:
          'Aylık taksit ${MoneyText.format(preview.installmentAmount, 'TRY')}, '
          '${preview.count} taksit, '
          '${DateText.dayMonthYear(preview.firstDate)} ile '
          '${DateText.dayMonthYear(preview.lastDate)} arası',
      excludeSemantics: true,
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.medium,
          vertical: AppSpacing.step14,
        ),
        decoration: BoxDecoration(
          color: colors.neutralContainer,
          borderRadius: BorderRadius.circular(AppRadius.field),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Wrap(
              alignment: WrapAlignment.spaceBetween,
              crossAxisAlignment: WrapCrossAlignment.center,
              spacing: AppSpacing.small,
              children: [
                Text(
                  'Aylık taksit',
                  style: theme.textTheme.bodyMedium?.copyWith(color: ink),
                ),
                Row(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.baseline,
                  textBaseline: TextBaseline.alphabetic,
                  children: [
                    amount,
                    const SizedBox(width: AppSpacing.small),
                    Text(
                      '× ${preview.count}',
                      style: theme.textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w600,
                        color: ink,
                      ),
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: AppSpacing.step10),
            ExcludeSemantics(
              child: Row(
                children: [
                  for (var i = 0; i < preview.count; i++) ...[
                    if (i > 0)
                      SizedBox(
                        width: preview.count > 24 ? 1 : AppSpacing.xSmall,
                      ),
                    Expanded(
                      child: Container(
                        height: 4,
                        decoration: BoxDecoration(
                          color: i == 0
                              ? colors.neutralFill
                              : ink.withValues(alpha: 0.25),
                          borderRadius: BorderRadius.circular(AppRadius.chip),
                        ),
                      ),
                    ),
                  ],
                ],
              ),
            ),
            const SizedBox(height: AppSpacing.step10),
            Row(
              children: [
                Expanded(
                  child: Text(
                    DateText.dayMonthYear(preview.firstDate),
                    style: small,
                  ),
                ),
                Text(DateText.dayMonthYear(preview.lastDate), style: small),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

/// Panel başlığının sağındaki kart etiketi: küçük koyu kart ve kartın adı.
class CardNameChip extends StatelessWidget {
  const CardNameChip({required this.name, super.key});
  final String name;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return ConstrainedBox(
      constraints: BoxConstraints(maxWidth: context.usesLargeText ? 120 : 160),
      child: Container(
        height: 28,
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.step10),
        decoration: BoxDecoration(
          color: surfaces.cardMuted,
          borderRadius: BorderRadius.circular(AppRadius.chip),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              width: 16,
              height: 11,
              decoration: BoxDecoration(
                color: AppSurfaces.light.ink,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            const SizedBox(width: AppSpacing.step6),
            Flexible(
              child: Text(
                name,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  fontSize: AppTextSizes.micro,
                  fontWeight: FontWeight.w600,
                  height: 1,
                  color: surfaces.ink,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
