import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';

/// Yatay kayan süzgeç şeridi: çipler kenardan kenara kayar.
///
/// Şerit dikey boşluk taşımaz; çipin kendi dokunma payı üstündeki ve
/// altındaki nefesi verir. Böylece şerit başlığın, arama alanının ya da başka
/// bir şeridin altında aynı uzaklıkta durur. Kenar payı ve çipler arası
/// `AppPageInsets.filterStrip` ile `AppGaps.chip`'ten gelir.
class AppFilterStrip extends StatelessWidget {
  /// Çipleri [AppGaps.chip] aralıkla dizer.
  const AppFilterStrip({required this.label, required this.children, super.key})
    : child = null;

  /// Aralarını kendisi kuran satır (çip, menü ve anahtar karışık dizilirse).
  const AppFilterStrip.row({
    required this.label,
    required Widget this.child,
    super.key,
  }) : children = const [];

  /// Ekran okuyucunun şerit için okuduğu ad.
  final String label;
  final List<Widget> children;
  final Widget? child;

  @override
  Widget build(BuildContext context) => Semantics(
    container: true,
    label: label,
    child: SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      padding: AppPageInsets.filterStrip,
      child:
          child ??
          Row(
            children: [
              for (var i = 0; i < children.length; i++) ...[
                if (i > 0) const SizedBox(width: AppGaps.chip),
                children[i],
              ],
            ],
          ),
    ),
  );
}
