import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';

/// Bölüm başlığı ve isteğe bağlı yan eylemi.
///
/// Başlık ekran okuyucuya `header` olarak bildirilir; TalkBack kullanıcısı
/// başlıktan başlığa atlayarak uzun bir sayfada gezinebilir.
class AppSectionHeader extends StatelessWidget {
  const AppSectionHeader({
    required this.title,
    super.key,
    this.trailing,
    this.padding = const EdgeInsets.symmetric(vertical: AppSpacing.small),
  });

  final String title;
  final Widget? trailing;

  /// Kart üstü bölümlerde (Kasa) başlık satırı yan eylemin 48 dp'lik
  /// dokunma alanı kadardır; ek dikey boşluk tasarımdaki aralığı açar.
  final EdgeInsetsGeometry padding;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: padding,
      child: Row(
        children: [
          Expanded(
            child: Semantics(
              header: true,
              child: Text(
                title,
                style: Theme.of(context).textTheme.titleMedium,
              ),
            ),
          ),
          ?trailing,
        ],
      ),
    );
  }
}
