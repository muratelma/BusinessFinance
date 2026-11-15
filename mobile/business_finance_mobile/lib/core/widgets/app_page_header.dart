import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_breakpoints.dart';
import '../theme/app_typography.dart';

/// Sayfa başlığı: başlık sayfanın kendisinden gelir.
///
/// Tek ölçü (10 Kasım 2026): satır saatin hemen altında başlar ve en az
/// [rowHeight] yüksekliğindedir; yazı ve eylemler satırın ortasında durur.
/// Ana sekmede başlık 26/700, [onBack] verilen (içine girilen) sayfada
/// 22/600'dür; ikisi de harf aralıksızdır. Başlığın altındaki boşluk kuralı
/// `documentation/design-system.md` "Başlık ve boşluk standardı" bölümündedir.
///
/// `AppBar` yerine kullanılır çünkü sağdaki ikon eylemleri glif kenarı
/// 16 dp'ye hizalanacak şekilde 48 dp dokunma alanında durur. `Scaffold.appBar`
/// yuvasında aynı satırı `AppPageBar` çizer.
class AppPageHeader extends StatelessWidget {
  const AppPageHeader({
    required String this.title,
    super.key,
    this.actions = const [],
    this.onBack,
    this.actionsInset = defaultActionsInset,
  }) : titleWidget = null,
       fixedHeight = false;

  /// Başlığı bir widget olan satır (duruma göre değişen başlık). Yazı stili
  /// satırdan gelir; [title] yalnız bir `Text` taşımalıdır.
  const AppPageHeader.custom({
    required Widget title,
    super.key,
    this.actions = const [],
    this.onBack,
    this.fixedHeight = false,
    this.actionsInset = defaultActionsInset,
  }) : titleWidget = title,
       title = null;

  final String? title;
  final Widget? titleWidget;
  final List<Widget> actions;
  final VoidCallback? onBack;

  /// Satır [rowHeight]'ta kalır: büyük yazıda başlık ikinci satıra çıkmaz ve
  /// eylemler alta inmez.
  final bool fixedHeight;

  /// Material 3 küçük üst çubuğunun yüksekliği.
  static const double rowHeight = 64;

  /// Sağdaki eylemlerin ekran kenarına uzaklığı: 48 dp dokunma alanının sağ
  /// kenarı. 8 dp'de 24 dp'lik ikonun kenarı 20 dp'de durur; 16 dp'deki ikon
  /// gözle fazla sağda okunuyordu (kullanıcı kararı, 15 Kasım 2026: önce kart
  /// sayfalarında denendi, sonra bütün sayfalara kural oldu). Yan yana iki
  /// eylem ve ay seçici gibi yazılı eylemler de aynı payla durur.
  static const double defaultActionsInset = AppSpacing.small;

  /// Sağdaki eylemlerin ekran kenarına uzaklığı.
  final double actionsInset;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final pushed = onBack != null;
    final wraps = context.usesLargeText && !fixedHeight;
    final style = theme.textTheme.headlineSmall!.copyWith(
      fontSize: pushed ? AppTextSizes.metric : AppTextSizes.pageTitle,
      fontWeight: pushed ? FontWeight.w600 : null,
      height: 1.2,
      letterSpacing: 0,
    );
    return Padding(
      padding: EdgeInsets.only(
        left: pushed ? AppSpacing.xSmall : AppSpacing.medium,
        right: actionsInset,
      ),
      // Başlık eylemleri mürekkep renginde: gri ikon devre dışı gibi okunur.
      child: IconButtonTheme(
        data: IconButtonThemeData(
          style: IconButton.styleFrom(
            foregroundColor: AppSurfaces.of(context).ink,
          ),
        ),
        child: ConstrainedBox(
          constraints: BoxConstraints(
            minHeight: rowHeight,
            maxHeight: fixedHeight ? rowHeight : double.infinity,
          ),
          // En az yükseklik başlığı ortalar; büyük yazıda satır uzar.
          child: Column(
            mainAxisSize: MainAxisSize.min,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Row(
                children: [
                  if (onBack != null) ...[
                    IconButton(
                      tooltip: 'Geri',
                      onPressed: onBack,
                      icon: const Icon(Icons.arrow_back),
                    ),
                    const SizedBox(width: AppSpacing.xSmall),
                  ],
                  Expanded(
                    child: Semantics(
                      header: true,
                      child: DefaultTextStyle(
                        style: style,
                        maxLines: wraps ? 2 : 1,
                        overflow: TextOverflow.ellipsis,
                        child: titleWidget ?? Text(title!),
                      ),
                    ),
                  ),
                  if (!wraps) ...actions,
                ],
              ),
              if (wraps && actions.isNotEmpty)
                Wrap(alignment: WrapAlignment.end, children: actions),
            ],
          ),
        ),
      ),
    );
  }
}
