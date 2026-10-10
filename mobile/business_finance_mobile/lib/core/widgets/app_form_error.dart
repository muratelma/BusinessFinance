import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';

/// Kaydın neden yazılmadığını formun içinde, `Kaydet`in yanında söyler.
///
/// Reddedilen kayıt formu kapatmaz: form kapanıp cümle arkadaki sayfada
/// çıkarsa kullanıcı kaydın yazıldığını sanabilir ve yazdıkları da kaybolur.
///
/// `AppInlineNotice` ile aynı biçimdedir (ikon + cümle, kapsül zemin) ama
/// hata tonundadır: o bir kaydın eksiğini söyler, bu bir kaydın reddini.
/// Bilgi yalnız renkle taşınmaz; ikon ve cümle aynı şeyi ayrı ayrı söyler.
class AppFormError extends StatelessWidget {
  const AppFormError({required this.message, super.key});

  final String message;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final scheme = theme.colorScheme;

    return Semantics(
      liveRegion: true,
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.medium,
          vertical: AppSpacing.small + AppSpacing.xSmall,
        ),
        decoration: BoxDecoration(
          color: scheme.errorContainer,
          borderRadius: BorderRadius.circular(AppSpacing.small),
        ),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(Icons.error_outline, size: 20, color: scheme.onErrorContainer),
            const SizedBox(width: AppSpacing.small),
            Expanded(
              child: Text(
                message,
                style: theme.textTheme.bodyMedium?.copyWith(
                  color: scheme.onErrorContainer,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
