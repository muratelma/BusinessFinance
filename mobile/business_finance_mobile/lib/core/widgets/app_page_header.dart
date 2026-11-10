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
        right: AppSpacing.xSmall,
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
