import 'package:flutter/material.dart';

import '../theme/app_finance_colors.dart';
import '../theme/app_spacing.dart';

/// Bir kaydın eksiğini, o kaydın yanında söyleyen not.
///
/// Hata değildir: veri geçerli, yalnız yarım. Hata görünümü kullanılırsa
/// kullanıcı bir şeyin bozulduğunu sanır ve düzeltmek yerine korkar. Bu
/// yüzden nötr ton kullanılır ve doğrudan yanında eksiği kapatan eylem durur —
/// kullanıcıyı "bir yerlerde bir ayar vardır" aramasına bırakmaz.
///
/// Bilgi yalnız renkle taşınmaz: ikon, cümle ve eylem etiketi aynı şeyi ayrı
/// ayrı söyler.
class AppInlineNotice extends StatelessWidget {
  const AppInlineNotice({
    required this.message,
    this.icon = Icons.info_outline,
    this.actionLabel,
    this.onAction,
    this.compact = false,
    this.emphasis,
    this.margin = const EdgeInsets.fromLTRB(
      AppSpacing.medium,
      AppSpacing.small,
      AppSpacing.medium,
      AppSpacing.small,
    ),
    super.key,
  });

  /// Dış boşluk. Kartın içinde duran uyarı (Hesabım kimlik kartı) kartın
  /// kendi iç boşluğunu kullanır ve sıfır verir.
  final EdgeInsetsGeometry margin;

  final String message;

  /// Teslimin bilgi notu: 16 dp yarıçap, 10/14 iç boşluk, 14 dp yazı.
  final bool compact;

  /// [message] içinde kalın yazılacak parça (ör. toplam tutar).
  final String? emphasis;
  final IconData icon;
  final String? actionLabel;
  final VoidCallback? onAction;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);

    final style = theme.textTheme.bodyMedium?.copyWith(
      height: compact ? 1.4 : null,
      color: colors.onNeutralContainer,
    );
    final cut = emphasis == null ? -1 : message.indexOf(emphasis!);
    return Padding(
      padding: margin,
      child: Container(
        padding: compact
            ? const EdgeInsets.symmetric(
                horizontal:
                    AppSpacing.small + AppSpacing.xSmall + AppSpacing.xxSmall,
                vertical: AppSpacing.small + AppSpacing.xxSmall,
              )
            : const EdgeInsets.all(AppSpacing.medium),
        decoration: BoxDecoration(
          color: colors.neutralContainer,
          borderRadius: BorderRadius.circular(
            compact ? AppSpacing.medium : AppSpacing.small,
          ),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Icon(
                  icon,
                  size: compact ? 18 : 20,
                  color: colors.onNeutralContainer,
                ),
                const SizedBox(width: AppSpacing.small),
                Expanded(
                  child: cut < 0
                      ? Text(message, style: style)
                      : Text.rich(
                          TextSpan(
                            children: [
                              TextSpan(text: message.substring(0, cut)),
                              TextSpan(
                                text: emphasis,
                                style: const TextStyle(
                                  fontWeight: FontWeight.w700,
                                ),
                              ),
                              TextSpan(
                                text: message.substring(cut + emphasis!.length),
                              ),
                            ],
                          ),
                          style: style,
                        ),
                ),
              ],
            ),
            if (actionLabel != null)
              Align(
                alignment: AlignmentDirectional.centerEnd,
                child: TextButton(
                  onPressed: onAction,
                  style: TextButton.styleFrom(
                    foregroundColor: colors.onNeutralContainer,
                  ),
                  child: Text(actionLabel!),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
