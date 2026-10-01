import 'package:flutter/material.dart';

import '../theme/app_breakpoints.dart';
import '../theme/app_radius.dart';

/// Pencere boyutuna göre bottom sheet veya dialog açar.
///
/// Telefonda alttan açılan panel doğru davranıştır: başparmağa yakındır ve
/// ekranın tamamını kullanır. Tablette aynı panel ekran boyunca gerilir, tek
/// bir onay satırı için 1000 dp genişliğinde bir yüzey açar ve içerik
/// kenarlarda kaybolur. Aynı içerik geniş ekranda ortalanmış bir dialog olarak
/// gösterilir.
abstract final class AppAdaptiveSheet {
  static Future<T?> show<T>({
    required BuildContext context,
    required WidgetBuilder builder,
    bool isScrollControlled = true,
    bool isDismissible = true,
  }) {
    if (context.windowSize.isCompact) {
      return showModalBottomSheet<T>(
        context: context,
        // Kök Navigator zorunlu. `StatefulShellRoute` her sekmeye kendi
        // Navigator'ını veriyor ve o Navigator kabuğun `Scaffold` gövdesinin
        // **içinde**; varsayılan (`false`) ile panel o iç katmanda çiziliyor,
        // yani kayan eylem butonunun ve alt gezinme çubuğunun **altında**
        // kalıyordu. Cihazda işlem ayrıntısının altı artı butonunun arkasına
        // giriyordu.
        //
        // `showDialog` bunu zaten kök Navigator'da açıyor (varsayılanı
        // `true`); iki dalın aynı katmanda açılması ayrıca tutarlılık.
        useRootNavigator: true,
        // Uzun panel ekranın tepesine kadar çıkınca tutamaç durum çubuğunun
        // altında kalıyor ve panel tutulup indirilemiyordu.
        useSafeArea: true,
        isScrollControlled: isScrollControlled,
        isDismissible: isDismissible,
        enableDrag: isDismissible,
        showDragHandle: true,
        builder: builder,
      );
    }

    return showDialog<T>(
      context: context,
      barrierDismissible: isDismissible,
      builder: (dialogContext) {
        // Dialog yüksekliği ekranla sınırlanır; içerik kendi kaydırmasını
        // yönetir. Sabit bir yükseklik verilseydi küçük tablette veya yatay
        // modda panel ekranın dışına taşardı.
        final maxHeight = MediaQuery.sizeOf(dialogContext).height * 0.85;
        return Dialog(
          clipBehavior: Clip.antiAlias,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppRadius.sheet),
          ),
          child: ConstrainedBox(
            constraints: BoxConstraints(
              maxWidth: AppBreakpoints.contentMaxWidth,
              maxHeight: maxHeight,
            ),
            child: builder(dialogContext),
          ),
        );
      },
    );
  }
}
