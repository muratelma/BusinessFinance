import 'package:flutter/material.dart';

import '../theme/app_typography.dart';

/// Alt ağacı teslimin ölçüsüyle, harf aralıksız çizer.
///
/// Gün sonu panelinde alınan karar (10 Ekim 2026) yeni sayfalarda sürer:
/// kenar boşluğu 16 dp, metin harf aralıksız. Eski ekranlar temanın aralığını
/// korur; bu sarmalayıcı yalnız teslimle birebir kurulan sayfalarda kullanılır.
class AppUntrackedText extends StatelessWidget {
  const AppUntrackedText({required this.child, super.key});

  final Widget child;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Theme(
      data: theme.copyWith(
        textTheme: AppTypography.untracked(theme.textTheme),
        extensions: [
          ...theme.extensions.values,
          const AppTextTracking(untracked: true),
        ],
      ),
      child: DefaultTextStyle.merge(
        style: const TextStyle(letterSpacing: 0),
        child: child,
      ),
    );
  }
}
