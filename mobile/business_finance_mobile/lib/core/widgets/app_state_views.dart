import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

class AppLoadingView extends StatelessWidget {
  const AppLoadingView({super.key, this.message = 'Yükleniyor'});

  final String message;

  @override
  Widget build(BuildContext context) {
    return Semantics(
      container: true,
      liveRegion: true,
      label: message,
      child: Center(
        child: Padding(
          padding: const EdgeInsets.all(AppSpacing.large),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const ExcludeSemantics(
                child: SizedBox.square(
                  dimension: 28,
                  child: CircularProgressIndicator(strokeWidth: 3),
                ),
              ),
              const SizedBox(height: AppSpacing.medium),
              ExcludeSemantics(
                child: Text(
                  message,
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.bodySmall,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Durum ekranlarının ortak iskeleti: yuvarlak ikon kapsülü, başlık, açıklama
/// ve isteğe bağlı eylem. Üçü de aynı ritmi kullanır, böylece boş ekran ile
/// hata ekranı birbirinin farklı bir sürümü gibi değil, aynı ailenin üyesi
/// gibi görünür.
class _StateScaffold extends StatelessWidget {
  const _StateScaffold({
    required this.icon,
    required this.title,
    required this.message,
    required this.tone,
    this.action,
    this.semanticsLabel,
    this.isLive = false,
  });

  final IconData icon;
  final String title;
  final String message;
  final Color tone;
  final Widget? action;
  final String? semanticsLabel;
  final bool isLive;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);

    final content = Center(
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(AppSpacing.large),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              width: 72,
              height: 72,
              decoration: BoxDecoration(
                color: surfaces.cardMuted,
                borderRadius: BorderRadius.circular(AppRadius.card),
              ),
              child: Icon(icon, size: 32, color: tone),
            ),
            const SizedBox(height: AppSpacing.medium),
            Text(
              title,
              textAlign: TextAlign.center,
              style: theme.textTheme.titleLarge,
            ),
            const SizedBox(height: AppSpacing.small),
            // Ölçü sınırı: uzun açıklama satırı ekran boyunca uzamaz.
            ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 320),
              child: Text(
                message,
                textAlign: TextAlign.center,
                style: theme.textTheme.bodyMedium?.copyWith(
                  color: theme.colorScheme.onSurfaceVariant,
                ),
              ),
            ),
            if (action != null) ...[
              const SizedBox(height: AppSpacing.large),
              action!,
            ],
          ],
        ),
      ),
    );

    if (semanticsLabel == null) return content;
    return Semantics(
      container: true,
      liveRegion: isLive,
      label: semanticsLabel,
      child: ExcludeSemantics(excluding: action == null, child: content),
    );
  }
}

class AppErrorView extends StatelessWidget {
  const AppErrorView({required this.message, super.key, this.onRetry});

  final String message;
  final VoidCallback? onRetry;

  @override
  Widget build(BuildContext context) {
    return _StateScaffold(
      icon: Icons.error_outline,
      title: 'Bir şeyler ters gitti',
      message: message,
      tone: Theme.of(context).colorScheme.error,
      semanticsLabel: 'Hata. $message',
      isLive: true,
      action: onRetry == null
          ? null
          : FilledButton.icon(
              onPressed: onRetry,
              icon: const Icon(Icons.refresh),
              label: const Text('Tekrar dene'),
            ),
    );
  }
}

class AppEmptyView extends StatelessWidget {
  const AppEmptyView({
    required this.title,
    required this.message,
    required this.icon,
    super.key,
    this.action,
  });

  final String title;
  final String message;
  final IconData icon;

  /// Boş ekran çıkmaz sokak olmamalı: mümkünse buradan bir sonraki adım
  /// başlatılabilir.
  final Widget? action;

  @override
  Widget build(BuildContext context) {
    return _StateScaffold(
      icon: icon,
      title: title,
      message: message,
      // Boş durum bir uyarı değil; ikonu ekrandaki en koyu leke olmamalı.
      tone: Theme.of(context).colorScheme.onSurfaceVariant,
      action: action,
    );
  }
}

class AppUnauthorizedView extends StatelessWidget {
  const AppUnauthorizedView({super.key});

  @override
  Widget build(BuildContext context) {
    return _StateScaffold(
      icon: Icons.lock_outline,
      title: 'Oturumunuz sona erdi',
      message: 'Devam etmek için lütfen yeniden giriş yapın.',
      tone: Theme.of(context).colorScheme.error,
      semanticsLabel: 'Hata. Oturumunuz sona erdi. Lütfen yeniden giriş yapın.',
      isLive: true,
    );
  }
}
