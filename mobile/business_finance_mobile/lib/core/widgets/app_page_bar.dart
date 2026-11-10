import 'package:flutter/material.dart';

import 'app_page_header.dart';

/// `Scaffold.appBar` yuvasına oturan sayfa başlığı: [AppPageHeader] ile aynı
/// satırı çizer, böylece `appBar` kullanan sayfalar da tek başlık ölçüsünü
/// taşır. `AppBar`'ın yerine geçer; geri oku rota geri dönebiliyorsa
/// kendiliğinden çıkar. [bottom] sekme çubuğu gibi başlığa yapışık şerittir.
class AppPageBar extends StatelessWidget implements PreferredSizeWidget {
  const AppPageBar({
    required this.title,
    super.key,
    this.actions = const [],
    this.bottom,
  });

  final Widget title;
  final List<Widget> actions;
  final PreferredSizeWidget? bottom;

  @override
  Size get preferredSize => Size.fromHeight(
    AppPageHeader.rowHeight + (bottom?.preferredSize.height ?? 0),
  );

  @override
  Widget build(BuildContext context) {
    final canPop = ModalRoute.of(context)?.canPop ?? false;
    return SafeArea(
      bottom: false,
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          AppPageHeader.custom(
            title: title,
            actions: actions,
            onBack: canPop ? () => Navigator.of(context).maybePop() : null,
            // `appBar` yuvasının yüksekliği sabittir; satır uzayamaz.
            fixedHeight: true,
          ),
          ?bottom,
        ],
      ),
    );
  }
}
