import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_surfaces.dart';

/// Bir kalemin toplam içindeki payını gösteren ince çubuk.
///
/// Liste satırının altına oturur: tutar zaten yazıyor, çubuk yalnız
/// "bu kalem toplamın ne kadarı" sorusunu bir bakışta yanıtlar. Bilgi
/// çubuğa bağlı değildir — tutar da yüzde de metin olarak okunur.
class AppShareBar extends StatelessWidget {
  const AppShareBar({
    required this.ratio,
    required this.color,
    super.key,
    this.height = 6,
  });

  /// 0 ile 1 arasında. Aralık dışı değerler kırpılır; sıfır toplamda
  /// çubuk boş kalır.
  final double ratio;
  final Color color;
  final double height;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final safeRatio = ratio.isFinite ? ratio.clamp(0.0, 1.0) : 0.0;

    return ClipRRect(
      borderRadius: BorderRadius.circular(AppRadius.chip),
      child: SizedBox(
        height: height,
        child: ColoredBox(
          color: surfaces.cardMuted,
          child: Align(
            alignment: Alignment.centerLeft,
            child: FractionallySizedBox(
              widthFactor: safeRatio,
              heightFactor: 1,
              child: ColoredBox(color: color),
            ),
          ),
        ),
      ),
    );
  }
}
