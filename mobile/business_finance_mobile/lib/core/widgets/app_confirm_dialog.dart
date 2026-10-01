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
///
/// Tasarım sistemindeki diyalogda ayrı bir başlık yoktur: büyük satır kararın
/// konusudur (`Motorlu taşıtlar · ₺2.180,00`) ve eylemin adı onay butonunda
/// yazar. [title] verilmezse diyalog tam olarak öyle çizilir. Başlık veren
/// çağrılarda başlık büyük satırdır, vurgu kendi zemininde altında kalır.
///
/// **Kayıt kartı** ([AppConfirmSubject]): karar tek bir kayıt hakkındaysa
/// kayıt kendi kartında, sola yaslı durur — ad, tarih ve kaynak, tutar ayrı
/// satırlarda. Ad ile tutar tek satıra sıkıştırılmadığı için uzun adlar
/// ortalanmış büyük satırı ikiye bölmez. Kartla açılan pencerede ikon ve
/// büyük satır çizilmez; neyin olacağını mesaj ve onay butonu söyler.
class AppConfirmSubject {
  const AppConfirmSubject({required this.title, this.detail, this.amount});

  /// Kaydın adı (`Motorlu taşıtlar`).
  final String title;

  /// Tarih ve kaynak gibi tek satırlık ayrıntı (`31 Temmuz · Bonus`).
  final String? detail;

  /// Gösterime hazır tutar (`₺2.180,00`).
  final String? amount;
}

abstract final class AppConfirmDialog {
  static Future<bool> show({
    required BuildContext context,
    required String message,
    required String confirmLabel,
    String? title,
    String cancelLabel = 'Vazgeç',
    IconData? icon,
    bool destructive = false,
    String? highlight,
    AppConfirmSubject? subject,
  }) async {
    assert(
      title != null || highlight != null || subject != null,
      'Diyalog neyin onaylandığını söylemeli: başlık, vurgu ya da kayıt.',
    );
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
        subject: subject,
      ),
    );
    // Barrier'a dokunup kapatmak da vazgeçmektir; null asla onay sayılmaz.
    return confirmed ?? false;
  }
}

class _ConfirmDialog extends StatelessWidget {
  const _ConfirmDialog({
    required this.message,
    required this.confirmLabel,
    required this.cancelLabel,
    required this.destructive,
    this.title,
    this.icon,
    this.highlight,
    this.subject,
  });

  final AppConfirmSubject? subject;
  final String? title;
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
    final headline = title ?? highlight;
    final boxed = title == null ? null : highlight;
    final record = subject;

    return Dialog(
      insetPadding: const EdgeInsets.all(AppSpacing.large),
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 360),
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(AppSpacing.large),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              if (record != null)
                _SubjectCard(record)
              else ...[
                // İkon en üstte ve ortada: kararın türü, okumaya başlamadan
                // önce görünür. Yıkıcı kararın rengi hata rolünden gelir;
                // ilerleten karar akromatik kalır.
                Container(
                  width: 48,
                  height: 48,
                  decoration: BoxDecoration(
                    color: destructive
                        ? colors.errorContainer
                        : surfaces.cardMuted,
                    borderRadius: BorderRadius.circular(AppRadius.field),
                  ),
                  child: Icon(
                    icon ??
                        (destructive
                            ? Icons.warning_amber_rounded
                            : Icons.check),
                    color: destructive
                        ? colors.onErrorContainer
                        : colors.onSurface,
                  ),
                ),
                const SizedBox(height: AppSpacing.medium),
                Semantics(
                  header: true,
                  namesRoute: true,
                  child: Text(
                    headline!,
                    textAlign: TextAlign.center,
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontSize: 22,
                      height: 1.3,
                      fontWeight: FontWeight.w700,
                      letterSpacing: -0.2,
                    ),
                  ),
                ),
                if (boxed != null) ...[
                  const SizedBox(height: AppSpacing.medium),
                  // Kararın konusu kendi zemininde: tutar ve ad, sonucu
                  // anlatan cümlenin içinde kaybolmaz.
                  Container(
                    width: double.infinity,
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppSpacing.medium,
                      vertical: AppSpacing.small + AppSpacing.xSmall,
                    ),
                    decoration: BoxDecoration(
                      color: surfaces.cardMuted,
                      borderRadius: BorderRadius.circular(AppRadius.field),
                    ),
                    child: Text(
                      boxed,
                      textAlign: TextAlign.center,
                      style: theme.textTheme.titleSmall,
                    ),
                  ),
                ],
              ],
              const SizedBox(height: AppSpacing.medium),
              SizedBox(
                width: double.infinity,
                child: Text(
                  message,
                  textAlign: record == null
                      ? TextAlign.center
                      : TextAlign.start,
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: surfaces.inkMuted,
                  ),
                ),
              ),
              const SizedBox(height: AppSpacing.medium),
              // Butonlar tam genişlikte ve alt alta: yıkıcı bir kararda yan
              // yana iki küçük hedef, yanlış olanına dokunmayı kolaylaştırıyor.
              // Onay üstte çünkü asıl sorulan o; vazgeçmek her zaman geri
              // dönüştür.
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
              const SizedBox(height: AppSpacing.small),
              SizedBox(
                width: double.infinity,
                child: FilledButton.tonal(
                  onPressed: () => Navigator.of(context).pop(false),
                  child: Text(cancelLabel),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Kararın konusu olan kayıt: ad, ayrıntı ve tutar; sola yaslı, kendi
/// zemininde.
class _SubjectCard extends StatelessWidget {
  const _SubjectCard(this.subject);

  final AppConfirmSubject subject;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(AppSpacing.medium),
      decoration: BoxDecoration(
        color: surfaces.cardMuted,
        borderRadius: BorderRadius.circular(AppRadius.field),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Semantics(
            header: true,
            namesRoute: true,
            child: Text(
              subject.title,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: theme.textTheme.titleSmall,
            ),
          ),
          if (subject.detail case final detail?) ...[
            const SizedBox(height: AppSpacing.xxSmall),
            Text(
              detail,
              style: theme.textTheme.bodyMedium?.copyWith(
                color: surfaces.inkMuted,
              ),
            ),
          ],
          if (subject.amount case final amount?) ...[
            const SizedBox(height: AppSpacing.small),
            Text(
              amount,
              style: theme.textTheme.titleLarge?.copyWith(
                fontSize: 22,
                height: 1.3,
                fontWeight: FontWeight.w700,
                letterSpacing: -0.2,
              ),
            ),
          ],
        ],
      ),
    );
  }
}
