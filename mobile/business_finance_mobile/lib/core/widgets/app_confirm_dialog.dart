import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Geri alınması zor bir eylemden önceki açık onay.
///
/// Onay reddedilirse hiçbir istek gitmez. Metinler çağıranın işine göre
/// verilir; "Emin misiniz?" gibi neyin olacağını söylemeyen bir kalıp
/// kullanılmaz.
///
/// **Neden düz metinden fazlası.** Önceki hâli başlık + paragraf + iki
/// butondan ibaretti ve ekranda bir metin belgesi gibi duruyordu: en önemli
/// bilgi — ne kadar para, hangi yönde — paragrafın ortasında bir cümlenin
/// içinde kayboluyordu. Şimdi üç kademe var: **ikon** ne tür bir karar
/// olduğunu (yıkıcı mı, ilerleten mi) tek bakışta söyler, **vurgu satırı**
/// kararın konusunu taşır, paragraf ise sonucu anlatır.
abstract final class AppConfirmDialog {
  static Future<bool> show({
    required BuildContext context,
    required String title,
    required String message,
    required String confirmLabel,
    String cancelLabel = 'Vazgeç',
    IconData? icon,
    bool destructive = false,
    String? highlight,
  }) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => _ConfirmDialog(
        title: title,
        message: message,
        confirmLabel: confirmLabel,
        cancelLabel: cancelLabel,
        icon: icon,
        destructive: destructive,
        highlight: highlight,
      ),
    );
    // Barrier'a dokunup kapatmak da vazgeçmektir; null asla onay sayılmaz.
    return confirmed ?? false;
  }
}

class _ConfirmDialog extends StatelessWidget {
  const _ConfirmDialog({
    required this.title,
    required this.message,
    required this.confirmLabel,
    required this.cancelLabel,
    required this.destructive,
    this.icon,
    this.highlight,
  });

  final String title;
  final String message;
  final String confirmLabel;
  final String cancelLabel;
  final bool destructive;
  final IconData? icon;
  final String? highlight;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = theme.colorScheme;
    final surfaces = AppSurfaces.of(context);

    // Yıkıcı kararın rengi hata rolünden gelir; ilerleten karar akromatik
    // kalır. Marka rengi akromatik olduğu için ekranda anlam taşıyan tek renk
    // budur ve o yüzden ancak gerçekten uyarı olan yerde kullanılır.
    final accent = destructive ? colors.error : colors.onSurface;
    final accentSurface = destructive
        ? colors.errorContainer
        : surfaces.cardMuted;

    return AlertDialog(
      // İkon başlığın üstünde ve ortada: kararın türü, okumaya başlamadan
      // önce görünür.
      icon: Container(
        width: 48,
        height: 48,
        decoration: BoxDecoration(
          color: accentSurface,
          borderRadius: BorderRadius.circular(AppRadius.field),
        ),
        child: Icon(
          icon ?? (destructive ? Icons.warning_amber_rounded : Icons.check),
          color: destructive ? colors.onErrorContainer : accent,
        ),
      ),
      title: Text(title, textAlign: TextAlign.center),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (highlight case final line?) ...[
            // Kararın konusu kendi zemininde: tutar ve ad, sonucu anlatan
            // cümlenin içinde kaybolmaz.
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
                vertical: AppSpacing.small + AppSpacing.xSmall,
              ),
              decoration: BoxDecoration(
                color: surfaces.cardMuted,
                borderRadius: BorderRadius.circular(AppRadius.field),
              ),
              child: Text(
                line,
                textAlign: TextAlign.center,
                style: theme.textTheme.titleSmall,
              ),
            ),
            const SizedBox(height: AppSpacing.medium),
          ],
          Text(
            message,
            textAlign: TextAlign.center,
            style: theme.textTheme.bodyMedium?.copyWith(
              color: surfaces.inkMuted,
            ),
          ),
        ],
      ),
      // Butonlar tam genişlikte ve alt alta: yıkıcı bir kararda yan yana iki
      // küçük hedef, yanlış olanına dokunmayı kolaylaştırıyor. Onay üstte
      // çünkü asıl sorulan o; vazgeçmek her zaman geri dönüştür.
      actionsAlignment: MainAxisAlignment.center,
      actionsOverflowDirection: VerticalDirection.down,
      actions: [
        SizedBox(
          width: double.infinity,
          child: FilledButton(
            style: destructive
                ? FilledButton.styleFrom(
                    backgroundColor: colors.error,
                    foregroundColor: colors.onError,
                  )
                : null,
            onPressed: () => Navigator.of(context).pop(true),
            child: Text(confirmLabel),
          ),
        ),
        SizedBox(
          width: double.infinity,
          child: TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: Text(cancelLabel),
          ),
        ),
      ],
    );
  }
}
