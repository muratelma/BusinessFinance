import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';

/// Açılır listedeki grup başlığı (`Hesaplar`, `Kredi kartları`).
///
/// Grup başlıkları seçilemeyen `DropdownMenuItem`'lardır, fakat `Dropdown`
/// bütün öğelere aynı metin stilini uygular: başlık da seçenek de aynı boyda,
/// aynı ağırlıkta, aynı renkte çiziliyordu. Listede `Hesaplar`, `Banka`,
/// `Nakit`, `Kredi kartları`, `Test Kart` alt alta tek bir düz blok gibi
/// görünüyor, hangisinin başlık hangisinin seçenek olduğu anlaşılmıyordu.
///
/// Başlık üç şeyle birden ayrılır; hiçbiri tek başına yeterli değil:
///
/// 1. **Boyut ve ağırlık** — seçeneklerden büyük ve daha kalın. Yalnız
///    soluklaştırmak yetmedi: soluk *ve* küçük bir metin başlık gibi değil,
///    devre dışı bırakılmış bir seçenek gibi okunuyordu.
/// 2. **Renk** — yardımcı mürekkep kademesi (`inkMuted`). En soluk kademe
///    (`inkFaint`) burada fazla geri çekiliyor.
/// 3. **Altındaki ayırıcı** — grubun nerede başladığını çizgi söyler.
///
/// Büyük harf **kullanılmaz**: Türkçede `i/İ` dönüşümü tuzaklıdır ve ekran
/// okuyucu büyük harfli metni harf harf okuyabilir.
class AppMenuGroupLabel extends StatelessWidget {
  const AppMenuGroupLabel(this.label, {super.key});

  final String label;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);

    return Column(
      mainAxisSize: MainAxisSize.min,
      mainAxisAlignment: MainAxisAlignment.end,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          label,
          style: theme.textTheme.titleSmall?.copyWith(
            color: surfaces.inkMuted,
            letterSpacing: 0.4,
          ),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        Divider(height: 1, thickness: 1, color: surfaces.border),
      ],
    );
  }
}
