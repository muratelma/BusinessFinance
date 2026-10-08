import 'package:flutter/material.dart';

import '../models/transaction_scope.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import 'app_segment_rail.dart';

/// `Hepsi · İşletme · Şahsi` anahtarı.
///
/// Uygulamanın **tek** kapsam anahtarıdır; sekme başına ayrı filtre yoktur
/// (ADR 0013). Boş değer `Hepsi` demektir: iki tarafı birden oku.
///
/// Tek parça raydır ([AppSegmentRail]): seçili dilim beyaz yüzey, diğerleri
/// gri zemin. Etiket büyük yazıda dilim içinde küçülür, ray taşmaz.
class AppScopeSwitch extends StatelessWidget {
  const AppScopeSwitch({
    required this.value,
    required this.onChanged,
    super.key,
  });

  final TransactionScope? value;
  final ValueChanged<TransactionScope?> onChanged;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return AppSegmentRail<TransactionScope?>(
      semanticLabel: 'Kapsam filtresi',
      values: const [
        null,
        TransactionScope.business,
        TransactionScope.personal,
      ],
      selected: value,
      onChanged: onChanged,
      segmentLabel: scopeFilterLabel,
      segmentBuilder: (context, option, selected) => Padding(
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.xSmall),
        child: FittedBox(
          fit: BoxFit.scaleDown,
          child: Text(
            scopeFilterLabel(option),
            maxLines: 1,
            style: theme.textTheme.bodyMedium?.copyWith(
              fontWeight: FontWeight.w600,
              height: 1.2,
              color: selected ? surfaces.ink : surfaces.inkMuted,
            ),
          ),
        ),
      ),
    );
  }
}

/// Taraf bölümlerinin yatay girintisi: üstlerindeki ve altlarındaki kutuların
/// sol kenarının bir tık içinden başlarlar. Kutuların köşesi yuvarlak olduğu
/// için tam kenarda duran yazı dışarı taşmış, alanın yazısıyla hizalanan yazı
/// ise fazla içeride görünüyordu.
const EdgeInsets _scopeInset = EdgeInsets.symmetric(
  horizontal: AppSpacing.small,
);

/// Formdaki kapsam alanı: ayrı bir zorunlu soru değil, **düzeltilebilir** iki
/// çip.
///
/// Değer türetme zincirinden dolu gelir (kaynağın etiketi → kategorinin
/// varsayılanı). Boş kalması yalnız zincir çözülemediğinde olur; o zaman alan
/// zorunludur, çünkü sunucu kapsam uydurmaz ve kayıt yazılmaz.
class AppScopeField extends StatelessWidget {
  const AppScopeField({
    required this.value,
    required this.onChanged,
    this.helperText,
    this.errorText,
    super.key,
  });

  final TransactionScope? value;
  final ValueChanged<TransactionScope> onChanged;
  final String? helperText;

  /// Zincir çözülemedi ve kullanıcı da seçmeden kaydetmeye çalıştı.
  final String? errorText;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: _scopeInset,
      child: Semantics(
        container: true,
        label: 'Kapsam',
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Kapsam', style: theme.textTheme.labelMedium),
            const SizedBox(height: AppSpacing.xSmall),
            Wrap(
              spacing: AppSpacing.small,
              runSpacing: AppSpacing.small,
              children: [
                for (final scope in TransactionScope.values)
                  AppScopeChoiceChip(
                    label: scope.label,
                    selected: value == scope,
                    onSelected: () => onChanged(scope),
                  ),
              ],
            ),
            if (helperText != null || errorText != null) ...[
              const SizedBox(height: AppSpacing.xSmall),
              Text(
                errorText ?? helperText!,
                style: theme.textTheme.bodySmall?.copyWith(
                  color: errorText == null
                      ? theme.colorScheme.onSurfaceVariant
                      : theme.colorScheme.error,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

/// Formdaki taraf bölümü (ADR 0020): tek taraflı kategoride **bilgi satırı**,
/// iki tarafa açık kategoride iki çip.
///
/// Kategori tarafı söylüyorsa sorulacak bir şey yoktur; çip çizmek kullanıcıya
/// değiştiremeyeceği bir seçim göstermek olurdu. İki tarafa açık kategoride
/// çipin ön değeri kaynağın (hesap/kart) etiketidir.
class AppScopeSection extends StatelessWidget {
  const AppScopeSection({
    required this.onChanged,
    this.explicit,
    this.source,
    this.category,
    this.sourceName,
    this.errorText,
    super.key,
  });

  /// Kullanıcının bu kayıt için açık seçimi.
  final TransactionScope? explicit;

  /// Hesabın ya da kartın etiketi; kaynağı olmayan formda boş.
  final TransactionScope? source;

  /// Seçili kategorinin tarafı; boşsa kategori iki tarafa açıktır.
  final TransactionScope? category;

  final String? sourceName;
  final ValueChanged<TransactionScope> onChanged;

  /// Taraf bulunamadı ve kullanıcı da seçmeden kaydetmeye çalıştı.
  final String? errorText;

  @override
  Widget build(BuildContext context) {
    if (category case final side?) {
      return AppScopeInfoRow(scope: side, reason: 'kategoriden');
    }
    return AppScopeField(
      value: previewResolvedScope(explicit: explicit, source: source),
      onChanged: onChanged,
      helperText: scopePreviewHelperText(
        explicit: explicit,
        source: source,
        sourceName: sourceName,
      ),
      errorText: errorText,
    );
  }
}

/// Sorulmayan tarafın bilgi satırı: `Şahsi · kategoriden`.
///
/// Taraf vurgulu, nedeni soluk yazılır; satır dokunulmaz ve ekran okuyucu
/// ikisini tek cümle okur.
class AppScopeInfoRow extends StatelessWidget {
  const AppScopeInfoRow({required this.scope, required this.reason, super.key});

  final TransactionScope scope;

  /// Tarafın nereden geldiği, küçük harfle: `kategoriden`.
  final String reason;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: _scopeInset,
      child: Semantics(
        container: true,
        label: 'Kapsam: ${scope.label}, $reason',
        child: ExcludeSemantics(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('Kapsam', style: theme.textTheme.labelMedium),
              const SizedBox(height: AppSpacing.xSmall),
              Text.rich(
                TextSpan(
                  children: [
                    TextSpan(
                      text: scope.label,
                      style: theme.textTheme.bodyLarge?.copyWith(
                        fontWeight: FontWeight.w600,
                        color: surfaces.ink,
                      ),
                    ),
                    TextSpan(
                      text: ' · $reason',
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: surfaces.inkMuted,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// İki denetimin ortak çipi.
///
/// Seçili olmak yalnız renkle değil, onay işaretiyle de bildiriliyor: renk
/// körlüğünde dolgu farkı hiçbir ölçütle garanti edilemez.
class AppScopeChoiceChip extends StatelessWidget {
  const AppScopeChoiceChip({
    required this.label,
    required this.selected,
    required this.onSelected,
    super.key,
  });

  final String label;
  final bool selected;
  final VoidCallback onSelected;

  @override
  Widget build(BuildContext context) {
    return ChoiceChip(
      label: Text(label),
      selected: selected,
      onSelected: (_) => onSelected(),
      // Çip varsayılan olarak 32 dp yüksekliğe oturur; dokunma hedefi
      // eşiği 48 dp.
      materialTapTargetSize: MaterialTapTargetSize.padded,
    );
  }
}

/// Hesabın, kartın ve kategorinin **varsayılan** kapsamı.
///
/// Üç konumu var çünkü alan nullable: `Belirtilmedi` eksik veri değil, meşru
/// bir cevaptır — "bu kaynak kapsam belirlemiyor, zincirin bir sonraki halkası
/// karar versin". Filtrenin `Hepsi`siyle karıştırılmasın diye etiketi ayrı:
/// orada boşluk "iki tarafı birden oku" demek, burada "ben söylemiyorum".
///
/// Kaydın kendi kapsamını soran [AppScopeField]'dan da ayrı: orası bir kaydın
/// hangi tarafa yazılacağını sorar ve boş bırakılamaz.
class AppScopeDefaultField extends StatelessWidget {
  const AppScopeDefaultField({
    required this.value,
    required this.onChanged,
    required this.helperText,
    this.label = 'Varsayılan kapsam',
    this.emptyLabel = 'Belirtilmedi',
    super.key,
  });

  final TransactionScope? value;
  final ValueChanged<TransactionScope?> onChanged;

  /// Alanın başlığı. Hesapta ve kartta bir varsayılandır; kategoride kaydın
  /// alabileceği taraftır ve başlığı ona göre verilir.
  final String label;

  /// Boş değerin adı. Hesapta ve kartta "söylemiyorum" (`Belirtilmedi`),
  /// kategoride "iki tarafa da açık" demektir (`İkisi de`).
  final String emptyLabel;

  /// Alanın ne işe yaradığını söyleyen cümle; çağıran yazar çünkü hesapta,
  /// kartta ve kategoride farklı okunur.
  final String helperText;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: _scopeInset,
      child: Semantics(
        container: true,
        label: label,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(label, style: theme.textTheme.labelMedium),
            const SizedBox(height: AppSpacing.xSmall),
            Wrap(
              spacing: AppSpacing.small,
              runSpacing: AppSpacing.small,
              children: [
                for (final option in <TransactionScope?>[
                  null,
                  TransactionScope.business,
                  TransactionScope.personal,
                ])
                  AppScopeChoiceChip(
                    label: option?.label ?? emptyLabel,
                    selected: value == option,
                    onSelected: () => onChanged(option),
                  ),
              ],
            ),
            const SizedBox(height: AppSpacing.xSmall),
            Text(
              helperText,
              style: theme.textTheme.bodySmall?.copyWith(
                color: theme.colorScheme.onSurfaceVariant,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
