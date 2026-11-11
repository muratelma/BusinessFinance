import 'package:flutter/material.dart';

import '../theme/app_surfaces.dart';
import 'app_card.dart';

/// Dolu bir sayfanın içindeki boş bölüm (boş durumun "bölüm boş" türü;
/// 12 Kasım 2026): kartın içinde tek soluk cümle. İkon, başlık ve düğme
/// yoktur; bölümün eylemi zaten bölümün kendi düğmesindedir.
///
/// Sayfanın tamamı boşsa [AppFirstUseView], arama ya da filtre boşsa
/// [AppNoResultsView] kullanılır.
class AppSectionEmpty extends StatelessWidget {
  const AppSectionEmpty(this.message, {super.key});

  final String message;

  @override
  Widget build(BuildContext context) => AppCard(
    child: Text(
      message,
      style: Theme.of(
        context,
      ).textTheme.bodyMedium?.copyWith(color: AppSurfaces.of(context).inkMuted),
    ),
  );
}
