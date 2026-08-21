import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';

/// Bir liste satırının kendi eylemi: `Öde`, `Tahsil et`, `Gerçekleştir`.
///
/// **Yanındaki durum rozetiyle aynı aileden görünür**: aynı yükseklik, aynı
/// hap biçimi, aynı yazı kademesi. Satır bir "durum + eylem" çifti taşır ve
/// ikisi birbirine benzemediğinde satır dağılmış görünüyordu.
///
/// **Dolgusu vardır, çerçevesi değil.** İki uç da denendi ve ikisi de cihazda
/// yanlış çıktı: çerçevesiz metin butonu rozetin yanında ikinci bir etiket gibi
/// okunuyor ve tıklanabilir olduğu fark edilmiyordu; ince çizgili hap ise
/// içi boş, soluk bir hayalet gibi duruyordu. Yumuşak dolgu (`tonal`) hem
/// dokunulacak yeri söylüyor hem de ekranın birincil eyleminin ağırlığına
/// çıkmıyor — o ağırlık `FilledButton`'a ayrılmıştır.
///
/// **İkon isteğe bağlıdır ve varsayılan olarak yoktur.** Rozet zaten bir ikon
/// taşıyor; satırda yan yana iki küçük ikon, ikisini de okunmaz yapıyordu.
class AppRowAction extends StatelessWidget {
  const AppRowAction({
    required this.label,
    required this.onPressed,
    super.key,
    this.icon,
  });

  final String label;

  /// `null` ise buton görünür ama kapalıdır: kaybolan bir eylem, kullanıcıya
  /// neden yapamadığını söylemez.
  final VoidCallback? onPressed;

  final IconData? icon;

  /// Rozetin en küçük yüksekliğiyle aynı; ikisi yan yana aynı hizada durur.
  static const double height = 32;

  static ButtonStyle styleOf(BuildContext context) => FilledButton.styleFrom(
    backgroundColor: Theme.of(context).colorScheme.secondaryContainer,
    foregroundColor: Theme.of(context).colorScheme.onSecondaryContainer,
    visualDensity: VisualDensity.standard,
    padding: const EdgeInsets.symmetric(
      horizontal: AppSpacing.medium - AppSpacing.xSmall,
    ),
    minimumSize: const Size(0, height),
    // Dokunma hedefi Material'in kendi payıyla korunur: görünen hap küçük,
    // dokunulan alan değil.
    tapTargetSize: MaterialTapTargetSize.padded,
    shape: const StadiumBorder(),
    textStyle: Theme.of(context).textTheme.labelLarge,
  );

  @override
  Widget build(BuildContext context) {
    final style = styleOf(context);
    if (icon == null) {
      return FilledButton(
        onPressed: onPressed,
        style: style,
        child: Text(label),
      );
    }
    return FilledButton.icon(
      onPressed: onPressed,
      style: style,
      icon: Icon(icon, size: 16),
      label: Text(label),
    );
  }
}
