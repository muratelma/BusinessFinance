import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_typography.dart';

/// Listenin neden boş kaldığı. İkonu sayfa değil, sebep seçer.
enum AppNoResultsReason {
  /// Aranan kelime hiçbir kayıtla eşleşmedi.
  search(Icons.search),

  /// Seçili filtreye uyan kayıt yok.
  filter(Icons.filter_alt_off_outlined),

  /// Seçili dönemde (ay, son 3 ay, 30 gün) kayıt yok.
  period(Icons.event_busy_outlined);

  const AppNoResultsReason(this.icon);
  final IconData icon;
}

/// Kayıt var ama arama, filtre ya da dönem boş kaldığında çizilen blok (boş
/// durumun "sonuç yok" türü; 12 Kasım 2026). İlk kullanımdan küçük ve
/// renksizdir: ortada soluk ikon, başlık, tek cümle ve tek çerçeveli düğme.
///
/// Başlık neyin boş olduğunu söyler (`“kira” için sonuç yok`, `Son 3 ayda kart
/// hareketi yok`), cümle sebebini ya da çıkışı (`Filtre: Transferler`), düğme
/// çıkışı kendisi yapar (`Filtreyi temizle`, `Son 1 yıla bak`). Ekleme düğmesi
/// bu türde yoktur. Blok üstündeki içerikten her sayfada aynı uzaklıkta
/// başlar; hem liste gövdesinde hem kaydırılan sayfanın içinde kullanılır.
class AppNoResultsView extends StatelessWidget {
  const AppNoResultsView({
    required this.reason,
    required this.title,
    super.key,
    this.message,
    this.actionLabel,
    this.onAction,
  });

  final AppNoResultsReason reason;
  final String title;
  final String? message;
  final String? actionLabel;
  final VoidCallback? onAction;

  /// Blokun üstündeki içerikten uzaklığı; kısa telefonda küçülür.
  static const double _topGap = 120;
  static const double _shortTopGap = 56;
  static const double _shortHeight = 760;

  static const double _iconCircle = 56;

  /// İlk kullanımdaki düğmeyle aynı en az genişlik.
  static const double _minButtonWidth = 160;

  /// Cümlede adıyla yazılan filtre sayısı; fazlası `+2` diye sayılır.
  static const int _namedFilters = 2;

  /// Seçili filtreleri tek satırlık cümleye çevirir: `Filtre: Transferler`,
  /// `Filtre: Transferler · Bu ay`, `Filtre: Transferler · Bu ay +2`. Yeni bir
  /// filtre eklendiğinde tasarım değişmez; listeye bir ad daha girer.
  static String filterSentence(List<String> names) {
    final named = names.take(_namedFilters).join(' · ');
    final rest = names.length - _namedFilters;
    return rest > 0 ? 'Filtre: $named +$rest' : 'Filtre: $named';
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final short = MediaQuery.sizeOf(context).height < _shortHeight;
    return Align(
      alignment: Alignment.topCenter,
      child: Padding(
        padding: EdgeInsets.fromLTRB(
          AppSpacing.xLarge,
          short ? _shortTopGap : _topGap,
          AppSpacing.xLarge,
          AppSpacing.xLarge,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ExcludeSemantics(
              child: Container(
                width: _iconCircle,
                height: _iconCircle,
                decoration: BoxDecoration(
                  color: surfaces.cardMuted,
                  shape: BoxShape.circle,
                ),
                child: Icon(reason.icon, size: 28, color: surfaces.inkMuted),
              ),
            ),
            const SizedBox(height: AppSpacing.medium),
            // Arama yazan kişi listenin boşaldığını duyar.
            Semantics(
              liveRegion: true,
              child: Text(
                title,
                textAlign: TextAlign.center,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: AppTextSizes.row,
                  height: 1.3,
                ),
              ),
            ),
            if (message != null) ...[
              const SizedBox(height: AppSpacing.xSmall),
              Text(
                message!,
                textAlign: TextAlign.center,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.bodySmall?.copyWith(
                  fontSize: AppTextSizes.body,
                  height: 1.4,
                  color: surfaces.inkMuted,
                ),
              ),
            ],
            if (actionLabel != null) ...[
              const SizedBox(height: AppSpacing.large),
              OutlinedButton(
                onPressed: onAction,
                style: OutlinedButton.styleFrom(
                  minimumSize: const Size(_minButtonWidth, AppSizes.button),
                ),
                child: Text(actionLabel!),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
