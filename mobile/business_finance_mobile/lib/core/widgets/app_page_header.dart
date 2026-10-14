import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_breakpoints.dart';

/// Ana ekranların başlık satırı: sayfa başlığı sayfanın kendisinden gelir.
///
/// `AppBar` yerine kullanılır çünkü başlık tasarımda 26/700 ve sağdaki ikon
/// eylemleri glif kenarı 16 dp'ye hizalanacak şekilde 48 dp dokunma alanında
/// durur; `AppBar`'ın kendi yüksekliği ve başlık boşlukları bunu vermez.
/// [onBack] verilirse solda geri oku çıkar ve başlık ona yaslanır.
class AppPageHeader extends StatelessWidget {
  const AppPageHeader({
    required this.title,
    super.key,
    this.actions = const [],
    this.onBack,
    this.compact = false,
  });

  final String title;
  final List<Widget> actions;
  final VoidCallback? onBack;
  final bool compact;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    // Tasarımda 64 dp en az yükseklik iç boşluğun **üstüne** eklenir (CSS
    // content-box): toplam 76 dp.
    return Padding(
      padding: EdgeInsets.fromLTRB(
        onBack == null
            ? AppSpacing.medium
            : compact
            ? AppSpacing.small
            : AppSpacing.xSmall,
        compact ? AppSpacing.small + AppSpacing.xSmall : AppSpacing.small,
        AppSpacing.xSmall,
        AppSpacing.xSmall,
      ),
      // Başlık eylemleri mürekkep renginde: gri ikon devre dışı gibi okunur.
      child: IconButtonTheme(
        data: IconButtonThemeData(
          style: IconButton.styleFrom(
            foregroundColor: AppSurfaces.of(context).ink,
          ),
        ),
        child: ConstrainedBox(
          constraints: BoxConstraints(minHeight: compact ? 48 : 64),
          // En az yükseklik başlığı ortalar; üste yaslanınca ana sekmelerin
          // başlığı 16 dp yukarı kayıyordu.
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
                      child: Text(
                        title,
                        maxLines: context.usesLargeText ? 2 : 1,
                        overflow: TextOverflow.ellipsis,
                        // Okuma sayfalarının başlığı teslimde harf aralıksız
                        // ve 1,2 satır yüksekliğiyle çizildi.
                        style: theme.textTheme.headlineSmall?.copyWith(
                          fontSize: 26,
                          height: compact ? 1.2 : null,
                          letterSpacing: compact ? 0 : -0.6,
                        ),
                      ),
                    ),
                  ),
                  if (!context.usesLargeText) ...actions,
                ],
              ),
              if (context.usesLargeText && actions.isNotEmpty)
                Wrap(alignment: WrapAlignment.end, children: actions),
            ],
          ),
        ),
      ),
    );
  }
}
