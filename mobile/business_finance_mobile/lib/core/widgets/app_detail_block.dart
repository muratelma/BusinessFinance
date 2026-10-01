import 'package:flutter/material.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import 'app_divided_column.dart';

/// Panel içindeki gri bilgi bloğu: etiket–değer satırları, ayırıcı ikonun
/// bittiği yerden.
///
/// İşlem detayı (Tarih / Kategori / Hesap / Not) ve POS tahsilatı detayı
/// (Brüt satış / Komisyon / Beklenen gün) bununla çizilir.
class AppDetailBlock extends StatelessWidget {
  const AppDetailBlock({required this.rows, super.key, this.background});

  final List<AppDetailRow> rows;

  /// Zemin; verilmezse gri kart yüzeyi. Gri sayfa zemininde duran blok
  /// (vergi tanımı ayrıntısı) kart yüzeyini alır.
  final Color? background;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
      decoration: BoxDecoration(
        color: background ?? AppSurfaces.of(context).cardMuted,
        borderRadius: BorderRadius.circular(AppRadius.field),
      ),
      child: AppDividedColumn(
        inset: rows.any((row) => row.icon != null) ? 30 : 0,
        children: rows,
      ),
    );
  }
}

/// [AppDetailBlock] satırı: solda ikon 18 + gri etiket 14, sağda değer
/// 14/500. Değer metin değilse (ör. tutar) [trailing] verilir.
class AppDetailRow extends StatelessWidget {
  const AppDetailRow({
    required this.label,
    super.key,
    this.icon,
    this.value,
    this.trailing,
  }) : assert(value != null || trailing != null);

  final IconData? icon;
  final String label;
  final String? value;
  final Widget? trailing;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return MergeSemantics(
      child: ConstrainedBox(
        constraints: const BoxConstraints(minHeight: 44),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
          child: Row(
            children: [
              if (icon != null) ...[
                Icon(icon, size: 18, color: surfaces.inkMuted),
                const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
              ],
              Text(
                label,
                style: theme.textTheme.bodyMedium?.copyWith(
                  height: 1.3,
                  color: surfaces.inkMuted,
                ),
              ),
              const SizedBox(width: AppSpacing.medium),
              Expanded(
                child: Align(
                  alignment: Alignment.centerRight,
                  child:
                      trailing ??
                      Text(
                        value!,
                        textAlign: TextAlign.right,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.bodyMedium?.copyWith(
                          height: 1.35,
                          fontWeight: FontWeight.w500,
                          color: surfaces.ink,
                        ),
                      ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
