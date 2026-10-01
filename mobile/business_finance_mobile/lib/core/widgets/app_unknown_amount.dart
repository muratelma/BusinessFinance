import 'package:flutter/material.dart';

import '../theme/app_surfaces.dart';

/// Tutarı ödeme gününe kadar belli olmayan kalemin tutar yeri.
///
/// Sıfır ya da tahmin yazmaz (ADR 0018 İ5): boş tutar sıfır değildir ve
/// toplamlar onu saymaz. Planlananlar, Özet ve Vergi takibi aynı cümleyi
/// kullanır.
class AppUnknownAmount extends StatelessWidget {
  const AppUnknownAmount({
    super.key,
    this.textAlign = TextAlign.right,
    this.maxWidth = 120,
  });

  static const text = 'Tutar ödemede girilecek';

  final TextAlign textAlign;
  final double maxWidth;

  @override
  Widget build(BuildContext context) => ConstrainedBox(
    constraints: BoxConstraints(maxWidth: maxWidth),
    child: Text(
      text,
      textAlign: textAlign,
      maxLines: 2,
      style: Theme.of(
        context,
      ).textTheme.bodySmall?.copyWith(color: AppSurfaces.of(context).inkMuted),
    ),
  );
}
