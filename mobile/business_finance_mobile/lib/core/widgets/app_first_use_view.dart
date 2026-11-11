import 'dart:math' as math;

import 'package:flutter/material.dart';

import '../theme/app_finance_colors.dart';
import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_typography.dart';
import 'app_icon_capsule.dart';
import 'app_status_chip.dart';

/// İlk kullanım boş durumunda sayfanın dolu hâlini anlatan soluk çizim.
enum AppEmptyPreview {
  /// Liste satırları: Hesaplar, Cari, Borç, Tekrarlayanlar, POS, İşlemler.
  rows,

  /// Kart: Kredi kartlarım.
  card,

  /// Halkalı satırlar: Bütçeler.
  ring,
}

/// Sayfada hiç kayıt yokken çizilen blok (boş durumun "ilk kullanım" türü;
/// 11 Kasım 2026). İkon yerine sayfanın dolu hâlinin soluk çizimi, altında
/// başlık, tek cümle ve tek düğme durur. Sayfa yalnız çizimin türünü, ikonu,
/// metni ve eylemi verir; ölçülerin hepsi buradadır.
///
/// Çizim "yükleniyor" iskeletine benzemesin diye hareketsizdir, aşağı doğru
/// daralır ve silikleşir, yalnız ilk parçası renklidir. Üç türün çizimi aynı
/// yüksekliktedir ([_previewHeight]); başlık her sayfada aynı yerde durur.
class AppFirstUseView extends StatelessWidget {
  const AppFirstUseView({
    required this.title,
    required this.message,
    required this.icon,
    super.key,
    this.preview = AppEmptyPreview.rows,
    this.actionLabel,
    this.actionIcon = Icons.add,
    this.onAction,
  });

  final String title;
  final String message;
  final IconData icon;
  final AppEmptyPreview preview;
  final String? actionLabel;
  final IconData actionIcon;
  final VoidCallback? onAction;

  /// Çizim sayfayı yatayda sarar; geniş ekranda bu genişlikte durur.
  static const double _maxWidth = 420;

  /// Her alt parça bir öncekinden bu kadar dardır.
  static const double _step = 24;
  static const double _capsule = 40;
  static const double _ring = 36;
  static const double _rowGap = AppSpacing.small;

  /// Satırın yüksekliği: kapsül + üst ve alt iç boşluk.
  static const double _rowHeight = _capsule + AppSpacing.medium * 2;

  /// Üç satır ve aralarındaki iki boşluk; kart çizimi de bu boydadır.
  static const double _previewHeight = _rowHeight * 3 + _rowGap * 2;

  /// Düğme kısa etikette de küçük kalmaz.
  static const double _minButtonWidth = 160;

  /// Blok sayfanın ortasının biraz üstünde durur.
  static const double _topRatio = 0.14;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: EdgeInsets.fromLTRB(
        AppSpacing.small,
        MediaQuery.sizeOf(context).height * _topRatio,
        AppSpacing.small,
        AppSpacing.xLarge,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          // Çizim süstür; ekran okuyucu başlığı, cümleyi ve düğmeyi okur.
          ExcludeSemantics(
            child: LayoutBuilder(
              builder: (context, constraints) {
                final width = math.min(constraints.maxWidth, _maxWidth);
                return switch (preview) {
                  AppEmptyPreview.card => _CardPreview(
                    icon: icon,
                    width: width,
                  ),
                  _ => Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      for (var i = 0; i < 3; i++) ...[
                        if (i > 0) const SizedBox(height: _rowGap),
                        _RowPreview(
                          icon: icon,
                          index: i,
                          width: width,
                          ring: preview == AppEmptyPreview.ring,
                        ),
                      ],
                    ],
                  ),
                };
              },
            ),
          ),
          const SizedBox(height: AppSpacing.large),
          Text(
            title,
            textAlign: TextAlign.center,
            style: theme.textTheme.titleMedium?.copyWith(
              fontSize: AppTextSizes.section,
              height: 1.3,
            ),
          ),
          const SizedBox(height: AppSpacing.small),
          Text(
            message,
            textAlign: TextAlign.center,
            style: theme.textTheme.bodyMedium?.copyWith(
              fontSize: AppTextSizes.body,
              height: 1.4,
              color: surfaces.inkMuted,
            ),
          ),
          if (actionLabel != null) ...[
            const SizedBox(height: AppSpacing.large),
            FilledButton.icon(
              onPressed: onAction,
              style: FilledButton.styleFrom(
                minimumSize: const Size(_minButtonWidth, AppSizes.button),
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.large,
                ),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(AppRadius.field),
                ),
              ),
              icon: Icon(actionIcon, size: 20),
              label: Text(actionLabel!),
            ),
          ],
        ],
      ),
    );
  }
}

const List<double> _fade = [1, 0.55, 0.22];

Color _ghost(BuildContext context) =>
    AppSurfaces.of(context).inkMuted.withValues(alpha: 0.22);

class _Bar extends StatelessWidget {
  const _Bar(this.width, this.height);

  final double width;
  final double height;

  @override
  Widget build(BuildContext context) => Container(
    width: width,
    height: height,
    decoration: BoxDecoration(
      color: _ghost(context),
      borderRadius: BorderRadius.circular(AppRadius.chip),
    ),
  );
}

class _RowPreview extends StatelessWidget {
  const _RowPreview({
    required this.icon,
    required this.index,
    required this.width,
    required this.ring,
  });

  final IconData icon;
  final int index;
  final double width;
  final bool ring;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final first = index == 0;
    final rowWidth = width - index * AppFirstUseView._step;
    final Widget leading;
    if (ring) {
      final tone = AppFinanceColors.of(context).neutral;
      leading = SizedBox.square(
        // Halka dolu kapsülden ağır durur; bir kademe küçük çizilir.
        dimension: AppFirstUseView._ring,
        child: CustomPaint(
          painter: _RingPainter(
            // Halkanın tamamı görünür; yalnız yay çizilirse dönen bekleme
            // halkasına benzer.
            track: first ? tone.withValues(alpha: 0.2) : _ghost(context),
            arc: first ? tone : null,
          ),
          child: first ? Icon(icon, size: 14, color: tone) : null,
        ),
      );
    } else if (first) {
      leading = AppIconCapsule(
        icon: icon,
        tone: AppStatusTone.neutral,
        size: AppFirstUseView._capsule,
      );
    } else {
      leading = Container(
        width: AppFirstUseView._capsule,
        height: AppFirstUseView._capsule,
        decoration: BoxDecoration(
          color: surfaces.cardMuted,
          shape: BoxShape.circle,
        ),
      );
    }
    return Opacity(
      opacity: _fade[index],
      child: Container(
        width: rowWidth,
        height: AppFirstUseView._rowHeight,
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
        decoration: BoxDecoration(
          color: surfaces.card,
          borderRadius: BorderRadius.circular(AppRadius.card),
          border: Border.all(color: surfaces.cardMuted),
        ),
        child: Row(
          children: [
            SizedBox(
              width: AppFirstUseView._capsule,
              child: Center(child: leading),
            ),
            const SizedBox(width: AppSpacing.medium),
            Column(
              mainAxisAlignment: MainAxisAlignment.center,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _Bar(rowWidth * (0.36 - index * 0.05), 10),
                const SizedBox(height: AppSpacing.small),
                _Bar(rowWidth * (0.22 + index * 0.03), 8),
              ],
            ),
            const Spacer(),
            _Bar(rowWidth * (0.22 - index * 0.03), 10),
          ],
        ),
      ),
    );
  }
}

class _RingPainter extends CustomPainter {
  const _RingPainter({required this.track, this.arc});

  final Color track;
  final Color? arc;

  @override
  void paint(Canvas canvas, Size size) {
    const stroke = 5.0;
    final rect = (Offset.zero & size).deflate(stroke / 2);
    final paint = Paint()
      ..style = PaintingStyle.stroke
      ..strokeWidth = stroke
      ..strokeCap = StrokeCap.round
      ..color = track;
    canvas.drawArc(rect, 0, math.pi * 2, false, paint);
    if (arc case final color?) {
      canvas.drawArc(
        rect,
        -math.pi / 2,
        math.pi * 2 * 0.68,
        false,
        paint..color = color,
      );
    }
  }

  @override
  bool shouldRepaint(_RingPainter old) => old.track != track || old.arc != arc;
}

/// Üç parçalı kart destesi: öndeki kart tam görünür, arkasındaki iki kartın
/// alt şeridi aşağı doğru daralır ve silikleşir. Satır çizimiyle aynı ritim.
class _CardPreview extends StatelessWidget {
  const _CardPreview({required this.icon, required this.width});

  final IconData icon;
  final double width;

  /// Arkadaki kartların görünen şeridi.
  static const double _strip = 44;
  static const double _height = AppFirstUseView._previewHeight - _strip * 2;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    BoxDecoration face() => BoxDecoration(
      color: surfaces.card,
      borderRadius: BorderRadius.circular(AppRadius.card),
      border: Border.all(color: surfaces.cardMuted),
    );
    return SizedBox(
      width: width,
      height: AppFirstUseView._previewHeight,
      child: Stack(
        alignment: Alignment.topCenter,
        children: [
          for (var i = 2; i > 0; i--)
            Positioned(
              top: _strip * i,
              child: Opacity(
                opacity: _fade[i],
                child: Container(
                  width: width - i * AppFirstUseView._step,
                  height: _height,
                  alignment: Alignment.bottomCenter,
                  padding: const EdgeInsets.fromLTRB(
                    AppSpacing.large,
                    0,
                    AppSpacing.large,
                    AppSpacing.medium,
                  ),
                  decoration: face(),
                  child: Row(
                    children: [
                      _Bar(width * (0.3 - i * 0.04), 10),
                      const Spacer(),
                      _Bar(width * (0.18 - i * 0.02), 10),
                    ],
                  ),
                ),
              ),
            ),
          Container(
            width: width,
            height: _height,
            padding: const EdgeInsets.all(AppSpacing.large),
            decoration: face(),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    AppIconCapsule(
                      icon: icon,
                      tone: AppStatusTone.neutral,
                      size: AppFirstUseView._capsule,
                    ),
                    const SizedBox(width: AppSpacing.medium),
                    _Bar(width * 0.3, 10),
                  ],
                ),
                const Spacer(),
                _Bar(width * 0.42, 16),
                const Spacer(),
                Row(
                  children: [
                    _Bar(width * 0.26, 10),
                    const Spacer(),
                    _Bar(width * 0.16, 10),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
