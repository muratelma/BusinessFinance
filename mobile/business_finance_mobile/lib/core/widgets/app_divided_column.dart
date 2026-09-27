import 'package:flutter/material.dart';

import '../theme/app_surfaces.dart';

/// Kart içi satır listesi: satırlar arasına ayırıcı, yazının başladığı
/// yerden.
///
/// Kapsüllü satırlarda [inset] 72'dir (`AppIconCapsule.rowInset`); kapsülsüz
/// satırlarda kart kenarıyla aynı hizada 16.
class AppDividedColumn extends StatelessWidget {
  const AppDividedColumn({required this.children, super.key, this.inset = 16});

  final List<Widget> children;
  final double inset;

  @override
  Widget build(BuildContext context) {
    final border = AppSurfaces.of(context).border;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      mainAxisSize: MainAxisSize.min,
      children: [
        for (var i = 0; i < children.length; i++) ...[
          if (i > 0)
            Padding(
              padding: EdgeInsets.only(left: inset),
              child: Divider(height: 1, thickness: 1, color: border),
            ),
          children[i],
        ],
      ],
    );
  }
}
