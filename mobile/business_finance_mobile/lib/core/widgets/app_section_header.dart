import 'package:flutter/material.dart';

import '../theme/app_breakpoints.dart';
import '../theme/app_spacing.dart';

/// Bölüm başlığı ve isteğe bağlı yan eylemi.
///
/// Başlık altındaki içerikle arasındaki boşluğu kendisi taşır
/// ([AppGaps.sectionTitle]); sayfa ardına ayrıca boşluk koymaz. Yan eylem
/// 48 dp'lik bir düğmeyse satır o kadar uzar ve o pay boşluğun yerini tutar;
/// yalnız o durumda `padding: EdgeInsets.zero` verilir. Başlığın **üstündeki** bölüm aralığını ([AppGaps.section])
/// sayfa verir: ilk bölümün üstünde boşluk olmaz.
///
/// Başlık ekran okuyucuya `header` olarak bildirilir; TalkBack kullanıcısı
/// başlıktan başlığa atlayarak uzun bir sayfada gezinebilir.
class AppSectionHeader extends StatelessWidget {
  const AppSectionHeader({
    required this.title,
    super.key,
    this.trailing,
    this.padding,
  });

  final String title;
  final Widget? trailing;

  /// Kuralın dışına çıkması gereken tek tük yer için; verilmezse kural
  /// uygulanır.
  final EdgeInsetsGeometry? padding;

  @override
  Widget build(BuildContext context) {
    final resolved =
        padding ?? const EdgeInsets.only(bottom: AppGaps.sectionTitle);
    final heading = Semantics(
      header: true,
      child: Text(title, style: Theme.of(context).textTheme.titleMedium),
    );
    // Büyük yazıda yan eylem başlığın yanına sığmaz; altına iner.
    if (trailing != null && context.usesLargeText) {
      return Padding(
        padding: resolved,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [heading, trailing!],
        ),
      );
    }
    return Padding(
      padding: resolved,
      child: Row(
        children: [
          Expanded(child: heading),
          ?trailing,
        ],
      ),
    );
  }
}
