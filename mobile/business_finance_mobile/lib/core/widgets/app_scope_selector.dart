import 'package:flutter/material.dart';

import '../models/transaction_scope.dart';
import '../theme/app_spacing.dart';

/// `Hepsi · İşletme · Şahsi` anahtarı.
///
/// Uygulamanın **tek** kapsam anahtarıdır; sekme başına ayrı filtre yoktur
/// (ADR 0013). Boş değer `Hepsi` demektir: iki tarafı birden oku.
///
/// Segmentli buton yerine sarmalanan çipler kullanılıyor: üç etiket 2.0×
/// yazı ölçeğinde tek satıra sığmıyor ve segmentli buton kaydırılamıyor —
/// büyük yazıda anahtarın bir ucu ekran dışında kalırdı.
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
    return Semantics(
      container: true,
      label: 'Kapsam filtresi',
      child: Wrap(
        spacing: AppSpacing.small,
        runSpacing: AppSpacing.small,
        children: [
          for (final option in <TransactionScope?>[
            null,
            TransactionScope.business,
            TransactionScope.personal,
          ])
            AppScopeChoiceChip(
              label: scopeFilterLabel(option),
              selected: value == option,
              onSelected: () => onChanged(option),
            ),
        ],
      ),
    );
  }
}

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
    return Semantics(
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
    super.key,
  });

  final TransactionScope? value;
  final ValueChanged<TransactionScope?> onChanged;

  /// Alanın ne işe yaradığını söyleyen cümle; çağıran yazar çünkü hesapta,
  /// kartta ve kategoride farklı okunur.
  final String helperText;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Semantics(
      container: true,
      label: 'Varsayılan kapsam',
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Varsayılan kapsam', style: theme.textTheme.labelMedium),
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
                  label: option?.label ?? 'Belirtilmedi',
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
    );
  }
}
