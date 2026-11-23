import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_money_text.dart';
import '../data/debt_book_models.dart';

/// Borç ve alacak ekranlarının ortak parçaları: satır, baş harf dairesi,
/// bölüm kartı. Görsel dil Planlananlar ve Tekrarlayanlar satırıdır
/// (`stages/06.3-borc-alacak/05-EKRAN-TASARIMI.md` §2).

/// Satırın başı ile yazının başı arası; ayırıcı yazının başladığı yerden
/// çizilir (16 kenar + 36 baş + 12 boşluk).
const double debtRowTextInset =
    AppSpacing.medium + AppSizes.rowButton + AppSpacing.compact;

/// Tarafın ikonu; işlem ayrıntısındaki `Alacağın` / `Borcun` satırlarıyla
/// aynı ikonlar.
IconData debtSideIcon(DebtSide side) =>
    side == DebtSide.receivable ? Icons.call_received : Icons.call_made;

/// Kişinin baş harfleri: ilk iki kelimenin ilk harfi. Türkçe `i` büyük `İ`
/// olur (Dart'ın `toUpperCase`'i yerel değildir).
String personInitials(String name) {
  final words = name
      .trim()
      .split(RegExp(r'\s+'))
      .where((word) => word.isNotEmpty)
      .take(2);
  String upper(String letter) => switch (letter) {
    'i' => 'İ',
    'ı' => 'I',
    _ => letter.toUpperCase(),
  };
  final letters = words.map((word) => upper(word.characters.first)).join();
  return letters.isEmpty ? '?' : letters;
}

/// Kişinin 36 dp baş harf dairesi. Kayıt satırlarındaki ikon kapsülüyle aynı
/// boy; ekran okuyucuya okunmaz (ad satırda yazılıdır).
class PersonInitials extends StatelessWidget {
  const PersonInitials({required this.name, super.key, this.muted = false});

  final String name;
  final bool muted;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return ExcludeSemantics(
      child: Container(
        width: AppSizes.rowButton,
        height: AppSizes.rowButton,
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: surfaces.cardMuted,
          shape: BoxShape.circle,
        ),
        child: Text(
          personInitials(name),
          maxLines: 1,
          textScaler: TextScaler.noScaling,
          style: theme.textTheme.labelMedium?.copyWith(
            fontSize: AppTextSizes.caption,
            fontWeight: FontWeight.w600,
            letterSpacing: 0,
            color: muted ? surfaces.inkFaint : surfaces.ink,
          ),
        ),
      ),
    );
  }
}

/// Bir bölümün satırları tek kartta; ayırıcı yazının başladığı yerden.
class DebtSectionCard extends StatelessWidget {
  const DebtSectionCard({
    required this.children,
    super.key,
    this.indent = debtRowTextInset,
  });

  final List<Widget> children;

  /// Ayırıcının başladığı yer: satırın yazısının başı.
  final double indent;

  @override
  Widget build(BuildContext context) => AppCard(
    padding: EdgeInsets.zero,
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        for (final (index, child) in children.indexed) ...[
          if (index > 0)
            Divider(height: 1, indent: indent, endIndent: AppSpacing.medium),

          child,
        ],
      ],
    ),
  );
}

/// Satırın ortak iskeleti: 36 dp baş, başlık ve tutar üstte, alt yazı ve
/// sağda isteğe bağlı ikinci değer altta.
class DebtRowFrame extends StatelessWidget {
  const DebtRowFrame({
    required this.leading,
    required this.title,
    required this.semanticLabel,
    super.key,
    this.trailing,
    this.subtitle,
    this.subtitleTrailing,
    this.footer,
    this.onTap,
    this.titleMuted = false,
  });

  final Widget leading;
  final String title;
  final Widget? trailing;
  final Widget? subtitle;

  /// Alt yazı satırının sağı (tutarın altında: `size borçlu`).
  final Widget? subtitleTrailing;

  /// Satırın en altı, sağa yaslı (iki yönlü kişinin ikinci tarafı).
  final Widget? footer;
  final String semanticLabel;
  final VoidCallback? onTap;
  final bool titleMuted;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      button: onTap != null,
      label: semanticLabel,
      excludeSemantics: true,
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            AppSpacing.compact,
            AppSpacing.medium,
            AppSpacing.compact,
          ),
          child: Row(
            crossAxisAlignment: subtitle == null && footer == null
                ? CrossAxisAlignment.center
                : CrossAxisAlignment.start,
            children: [
              leading,
              const SizedBox(width: AppSpacing.compact),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: Text(
                            title,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: theme.textTheme.titleSmall?.copyWith(
                              color: titleMuted ? surfaces.inkMuted : null,
                            ),
                          ),
                        ),
                        if (trailing != null) ...[
                          const SizedBox(width: AppSpacing.small),
                          trailing!,
                        ],
                      ],
                    ),
                    if (subtitle != null || subtitleTrailing != null) ...[
                      const SizedBox(height: AppGaps.rowText),
                      Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Expanded(child: subtitle ?? const SizedBox.shrink()),
                          if (subtitleTrailing != null) ...[
                            const SizedBox(width: AppSpacing.small),
                            subtitleTrailing!,
                          ],
                        ],
                      ),
                    ],
                    if (footer != null) ...[
                      const SizedBox(height: AppSpacing.xSmall),
                      Align(alignment: Alignment.centerRight, child: footer),
                    ],
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

/// Satır tutarı: 16/700, renksiz. Taraf sekmeden ve alt yazıdan okunur.
class DebtRowAmount extends StatelessWidget {
  const DebtRowAmount(this.amount, {super.key});

  final String amount;

  @override
  Widget build(BuildContext context) => AppMoneyText(
    amount: amount,
    currency: 'TRY',
    size: AppMoneySize.row,
    style: const TextStyle(fontWeight: FontWeight.w700),
  );
}

/// Alt yazının parçaları `·` ile; gecikme parçası gider renginde.
class DebtRowSubtitle extends StatelessWidget {
  const DebtRowSubtitle({required this.parts, super.key, this.alert});

  final List<String> parts;

  /// Başta duran ve gider renginde yazılan parça (`₺2.000,00 gecikmiş`).
  final String? alert;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    final small = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final rest = parts.join(' · ');
    return Text.rich(
      TextSpan(
        children: [
          if (alert != null)
            TextSpan(
              text: alert,
              style: small?.copyWith(
                color: colors.expense,
                fontWeight: FontWeight.w600,
              ),
            ),
          if (alert != null && rest.isNotEmpty) const TextSpan(text: ' · '),
          TextSpan(text: rest),
        ],
      ),
      maxLines: 2,
      overflow: TextOverflow.ellipsis,
      style: small,
    );
  }
}

/// Liste ve kişi sayfasındaki satır: kişinin hesabı ya da taksitli/kişisiz
/// kayıt (`05-EKRAN-TASARIMI.md` §3.1).
class DebtBookItemRow extends StatelessWidget {
  const DebtBookItemRow({
    required this.item,
    super.key,
    this.showScope = false,
    this.onTap,
  });

  final DebtBookItem item;

  /// `Hepsi` görünümünde alt yazının başında kapsam etiketi.
  final bool showScope;
  final VoidCallback? onTap;

  String? get _alert => item.hasOverdue
      ? '${MoneyText.format(item.overdue, 'TRY')} gecikmiş'
      : null;

  List<String> get _parts {
    final scope = showScope ? item.scope?.label : null;
    final due = item.nextDueDate;
    final String? status;
    if (item.isAllAdvance) {
      status = item.side == DebtSide.payable ? 'Alınan avans' : 'Verilen avans';
    } else if (item.isInstallment) {
      status = [
        '${item.paidInstallmentCount}/${item.installmentCount} taksit',
        if (due != null) 'sıradaki ${DateText.dayMonth(due)}',
      ].join(' · ');
    } else if (item.hasOverdue) {
      status = null;
    } else if (due != null) {
      status = item.nextDueIsPartial
          ? 'Vade ${DateText.dayMonth(due)} · '
                '${MoneyText.format(item.nextDueAmount, 'TRY')}'
          : 'Vade ${DateText.dayMonth(due)}';
    } else {
      status = 'Vadesiz';
    }
    // Kapsam en sonda: Planlananlar satırı gibi (`Tekrarlayan ödeme ·
    // İşletme`); satırın asıl bilgisi başta okunur.
    return [?status, ?scope];
  }

  @override
  Widget build(BuildContext context) {
    final parts = _parts;
    return DebtRowFrame(
      leading: item.isLedger
          ? PersonInitials(name: item.title)
          : AppIconCapsule(
              icon: item.isInstallment
                  ? Icons.event_repeat
                  : Icons.receipt_long_outlined,
              size: AppSizes.rowButton,
            ),
      title: item.title,
      trailing: DebtRowAmount(item.remaining),
      subtitle: DebtRowSubtitle(alert: _alert, parts: parts),
      onTap: onTap,
      semanticLabel: [
        item.title,
        MoneyText.format(item.remaining, 'TRY'),
        ?_alert,
        ...parts,
      ].join('. '),
    );
  }
}

/// `Kişiler` sekmesinin satırı: sağda her açık taraf için tutar ve altında
/// tarafın adı; bakiyesiz kişide `Hesap kapalı`.
class DebtPersonRow extends StatelessWidget {
  const DebtPersonRow({
    required this.person,
    super.key,
    this.showScope = false,
    this.onTap,
  });

  final DebtBookPerson person;
  final bool showScope;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final small = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final sides = [
      if (isPositiveAmount(person.owedToYou)) (person.owedToYou, 'size borçlu'),
      if (isPositiveAmount(person.owedByYou)) (person.owedByYou, 'borçlusunuz'),
    ];
    final details = [
      if (showScope) ?person.scope?.label,
      if (!person.isActive) 'Pasif',
    ];
    final second = sides.length > 1 ? sides[1] : null;
    return DebtRowFrame(
      leading: PersonInitials(name: person.name, muted: !person.isActive),
      title: person.name,
      titleMuted: !person.isActive,
      trailing: sides.isEmpty
          ? Text('Hesap kapalı', style: small)
          : DebtRowAmount(sides.first.$1),
      subtitle: details.isEmpty ? null : DebtRowSubtitle(parts: details),
      subtitleTrailing: sides.isEmpty
          ? null
          : Text(sides.first.$2, style: small),
      footer: second == null
          ? null
          : Text.rich(
              TextSpan(
                children: [
                  TextSpan(
                    text: MoneyText.format(second.$1, 'TRY'),
                    style: TextStyle(
                      color: surfaces.ink,
                      fontWeight: FontWeight.w700,
                      fontFeatures: AppTypography.tabularFigures,
                    ),
                  ),
                  TextSpan(text: ' ${second.$2}'),
                ],
              ),
              style: small,
            ),
      onTap: onTap,
      semanticLabel: [
        person.name,
        for (final (amount, label) in sides)
          '${MoneyText.format(amount, 'TRY')} $label',
        if (sides.isEmpty) 'Hesap kapalı',
        ...details,
      ].join('. '),
    );
  }
}

/// Liste sonundaki kapı satırı (`Kapananlar`), Tekrarlayanlar'daki
/// `Duraklatılan ve bitenler` satırının aynısı.
class DebtGateRow extends StatelessWidget {
  const DebtGateRow({
    required this.label,
    required this.count,
    super.key,
    this.onTap,
  });

  final String label;
  final int count;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      button: true,
      label: '$label, $count',
      excludeSemantics: true,
      child: AppCard(
        padding: EdgeInsets.zero,
        onTap: onTap,
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 56),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
            child: Row(
              children: [
                Expanded(child: Text(label, style: theme.textTheme.titleSmall)),
                Text(
                  '$count',
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: surfaces.inkMuted,
                  ),
                ),
                const SizedBox(width: AppSpacing.xSmall),
                Icon(Icons.chevron_right, color: surfaces.inkMuted),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Bölüm başlığının yanındaki soluk not (`Yaklaşan  30 gün içinde`),
/// Planlananlar'daki gün başlığının `Bugün` notu gibi.
class DebtSectionTitle extends StatelessWidget {
  const DebtSectionTitle({required this.title, super.key, this.note});

  final String title;
  final String? note;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: const EdgeInsets.only(bottom: AppGaps.sectionTitle),
      child: Semantics(
        header: true,
        child: Text.rich(
          TextSpan(
            children: [
              TextSpan(text: title),
              if (note != null)
                TextSpan(
                  text: '  $note',
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: surfaces.inkMuted,
                  ),
                ),
            ],
          ),
          style: theme.textTheme.titleMedium?.copyWith(
            fontSize: AppTextSizes.compact,
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
    );
  }
}

/// Kapsam görünen kullanıcıda kartın üstündeki gri şerit: ad ve kapsam
/// menüsü (Planlananlar'ın dönem kartı). Menü uygulamanın genel kapsamını
/// değiştirir.
class DebtScopeStrip extends StatelessWidget {
  const DebtScopeStrip({
    required this.label,
    required this.scope,
    required this.onScope,
    super.key,
  });

  final String label;
  final TransactionScope? scope;
  final ValueChanged<TransactionScope?> onScope;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Container(
      constraints: const BoxConstraints(minHeight: AppSizes.button),
      padding: const EdgeInsets.only(left: AppSpacing.medium),
      color: surfaces.cardMuted,
      child: Row(
        children: [
          Expanded(
            child: Text(
              label,
              style: theme.textTheme.bodyMedium?.copyWith(
                color: surfaces.inkMuted,
              ),
            ),
          ),
          PopupMenuButton<String>(
            tooltip: 'Kapsam: ${scope?.label ?? 'Hepsi'}',
            initialValue: scope?.apiValue ?? 'all',
            onSelected: (key) => onScope(
              key == 'all'
                  ? null
                  : TransactionScope.values.firstWhere(
                      (value) => value.apiValue == key,
                    ),
            ),
            itemBuilder: (_) => [
              const PopupMenuItem(value: 'all', child: Text('Hepsi')),
              for (final value in TransactionScope.values)
                PopupMenuItem(value: value.apiValue, child: Text(value.label)),
            ],
            child: ConstrainedBox(
              constraints: const BoxConstraints(minHeight: AppSizes.button),
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.compact,
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      scope?.label ?? 'Hepsi',
                      style: theme.textTheme.bodyMedium,
                    ),
                    const SizedBox(width: AppSpacing.xSmall),
                    Icon(
                      Icons.expand_more,
                      size: AppTextSizes.section,
                      color: surfaces.inkMuted,
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// Liste özetinin bir sütunu: yön ikonu ve etiket, tutar, varsa gecikmiş.
class DebtTotalColumn extends StatelessWidget {
  const DebtTotalColumn({
    required this.side,
    required this.amount,
    required this.overdue,
    super.key,
  });

  final DebtSide side;
  final String amount;
  final String overdue;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final colors = AppFinanceColors.of(context);
    final label = side == DebtSide.receivable ? 'Alacağınız' : 'Borcunuz';
    final hasOverdue = isPositiveAmount(overdue);
    final small = theme.textTheme.bodySmall?.copyWith(
      fontSize: AppTextSizes.caption,
      color: surfaces.inkMuted,
    );
    return MergeSemantics(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(
                debtSideIcon(side),
                size: AppTextSizes.row,
                color: surfaces.inkMuted,
              ),
              const SizedBox(width: AppSpacing.xSmall),
              Flexible(child: Text(label, style: small)),
            ],
          ),
          const SizedBox(height: AppSpacing.xSmall),
          FittedBox(
            fit: BoxFit.scaleDown,
            alignment: Alignment.centerLeft,
            child: Text(
              MoneyText.format(amount, 'TRY'),
              maxLines: 1,
              style: theme.textTheme.titleLarge?.copyWith(
                fontSize: AppTextSizes.metric,
                fontWeight: FontWeight.w700,
                fontFeatures: AppTypography.tabularFigures,
                letterSpacing: 0,
              ),
            ),
          ),
          if (hasOverdue) ...[
            const SizedBox(height: AppSpacing.xSmall),
            Row(
              children: [
                Icon(
                  Icons.schedule,
                  size: AppTextSizes.body,
                  color: colors.expense,
                ),
                const SizedBox(width: AppSpacing.xSmall),
                Flexible(
                  child: Text(
                    '${MoneyText.format(overdue, 'TRY')} gecikmiş',
                    style: small?.copyWith(
                      color: colors.expense,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ],
      ),
    );
  }
}

/// Koyu özet kartının zemini ve yazı renkleri (plan ayrıntısının kartı).
/// Gecikme kırmızısı koyu zeminde zıt temanın gider rengidir.
class DebtInkCard extends StatelessWidget {
  const DebtInkCard({required this.child, super.key});

  final Widget child;

  static Color onInk(BuildContext context) =>
      Theme.of(context).colorScheme.onInverseSurface;

  static Color onInkMuted(BuildContext context) =>
      onInk(context).withValues(alpha: 0.8);

  static Color alertOnInk(BuildContext context) =>
      Theme.of(context).brightness == Brightness.light
      ? AppFinanceColors.dark.expense
      : AppFinanceColors.light.expense;

  @override
  Widget build(BuildContext context) => Container(
    padding: const EdgeInsets.all(AppSpacing.medium),
    decoration: BoxDecoration(
      color: AppSurfaces.of(context).ink,
      borderRadius: BorderRadius.circular(AppRadius.card),
    ),
    child: child,
  );
}

/// Alanın altındaki tek satırlık bilgi: soluk ikon ve cümle. Kutu değildir;
/// kaydı engellemeyen bir bilgi alanın yardım yazısı kadar yer kaplar.
class DebtFieldNote extends StatelessWidget {
  const DebtFieldNote(this.message, {super.key});

  final String message;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final style = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    return Semantics(
      liveRegion: true,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.only(top: AppSpacing.xxSmall),
            child: Icon(
              Icons.info_outline,
              size: AppTextSizes.body,
              color: surfaces.inkMuted,
            ),
          ),
          const SizedBox(width: AppSpacing.xSmall),
          Expanded(child: Text(message, style: style)),
        ],
      ),
    );
  }
}
