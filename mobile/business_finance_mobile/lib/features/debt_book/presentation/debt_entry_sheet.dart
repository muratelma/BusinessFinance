import 'package:flutter/material.dart';

import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_row.dart';
import '../data/debt_book_models.dart';

/// `Ne oldu?` panelinin seçenekleri (`02-TASARIM.md` §6.4). Seçenek
/// kullanıcının ağzından yazılır; kaydın nereye gideceğini uygulama bilir.
enum DebtEntryKind {
  sale('Veresiye sattım', 'Sonra tahsil edeceğim', 'Veresiye satış'),
  purchase('Vadeli aldım', 'Sonra ödeyeceğim', 'Vadeli alım'),
  lend('Borç verdim', 'Geri alacağım para', 'Borç verdim'),
  borrow('Borç aldım', 'Geri ödeyeceğim para, kredi', 'Borç aldım'),
  advance(
    'Avans aldım / verdim',
    'İş ya da mal öncesi alınan, verilen para',
    'Avans',
  );

  const DebtEntryKind(this.label, this.hint, this.formTitle);

  final String label;
  final String hint;

  /// Formun başlığı.
  final String formTitle;

  /// Kişinin hesabında açtığı taraf; avansta formda seçilir.
  DebtSide? get side => switch (this) {
    sale || lend => DebtSide.receivable,
    purchase || borrow => DebtSide.payable,
    advance => null,
  };

  /// Satış ve alım mal, borç para para; aynı taraftaki iki seçenek aynı
  /// ikonu taşımaz.
  IconData get icon => switch (this) {
    sale => Icons.sell_outlined,
    purchase => Icons.shopping_bag_outlined,
    lend => Icons.volunteer_activism_outlined,
    borrow => Icons.account_balance_outlined,
    advance => Icons.swap_horiz,
  };

  /// Pasif kişiye yazılamayan kayıt: yeni satış, alım ve borç para. Avans
  /// tahsilat ya da ödemedir, yazılır.
  bool get needsActivePerson => this != advance;
}

/// `Ne oldu?` paneli. Kişi sayfasından açılınca başlığın altında kişinin
/// adı yazar; pasif kişide yazılamayan satırlar soluk ve dokunulmazdır.
class DebtEntrySheet extends StatelessWidget {
  const DebtEntrySheet({super.key, this.personName, this.personActive = true});

  final String? personName;
  final bool personActive;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return SafeArea(
      child: SingleChildScrollView(
        padding: AppSheetInsets.detail(context),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Semantics(
              header: true,
              child: Text('Ne oldu?', style: theme.textTheme.titleLarge),
            ),
            if (personName != null) ...[
              const SizedBox(height: AppSpacing.xSmall),
              Text(
                personName!,
                style: theme.textTheme.bodySmall?.copyWith(
                  color: surfaces.inkMuted,
                ),
              ),
            ],
            const SizedBox(height: AppSpacing.compact),
            AppDividedColumn(
              inset: AppIconCapsule.rowInset - AppSpacing.medium,
              children: [
                for (final kind in DebtEntryKind.values) _option(context, kind),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _option(BuildContext context, DebtEntryKind kind) {
    final surfaces = AppSurfaces.of(context);
    final blocked = !personActive && kind.needsActivePerson;
    final row = AppRow(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.compact),
      leading: AppIconCapsule(icon: kind.icon),
      title: kind.label,
      subtitle: blocked ? 'Pasif kişiye yeni kayıt yazılmaz' : kind.hint,
      semanticLabel: blocked
          ? '${kind.label}. Pasif kişiye yeni kayıt yazılmaz'
          : '${kind.label}. ${kind.hint}',
      trailing: blocked
          ? null
          : Icon(Icons.chevron_right, color: surfaces.inkMuted),
      onTap: blocked ? null : () => Navigator.of(context).pop(kind),
    );
    return blocked ? Opacity(opacity: 0.45, child: row) : row;
  }
}
