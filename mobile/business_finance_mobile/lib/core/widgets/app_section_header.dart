import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';

/// Bölüm başlığı ve isteğe bağlı yan eylemi.
///
/// Başlık ekran okuyucuya `header` olarak bildirilir; TalkBack kullanıcısı
/// başlıktan başlığa atlayarak uzun bir sayfada gezinebilir.
class AppSectionHeader extends StatelessWidget {
  const AppSectionHeader({required this.title, super.key, this.trailing});

  final String title;
  final Widget? trailing;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
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
