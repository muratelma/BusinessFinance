import 'dart:math' as math;

import 'package:flutter/material.dart';

import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../data/finance_models.dart';

/// Kredi kartları sayfasının üstünde yan yana kayan kartın yüzü (Claude
/// Design, kart taksit ve limit teslimi §1, `KartSerit`).
///
/// Üstte kartın adı, solda güncel borç, sağda son ödeme ve kesim günü; altta
/// kullanılabilir limit ve çubuğu. Limit ayrı bir kutu değildir, kartın
/// içindedir. Çubuk dönem borcunu kırmızı, borcun geri kalanını gri çizer;
/// tutarlar sunucudandır, istemci yalnız çubuğun oranını çizer.
///
/// **Kartın ölçüleri bilerek ortak kuralın biraz üstündedir** (kullanıcı
/// kararı, 12 Kasım 2026): kart bir nesne gibi okunsun diye yazılar teslimin
/// boyundadır (borç 28/700, değerler 15/600, etiketler 13). Kural yalnız bu
/// kartın yüzü için gevşer.
class CreditCardTile extends StatelessWidget {
  const CreditCardTile({
    required this.card,
    super.key,
    this.onTap,
    this.statementBalance,
    this.fill = false,
  });

  /// Kart verilen yüksekliği doldurur (kaydırma şeridi); limit en alta iner.
  final bool fill;

  final CreditCardItem card;
  final VoidCallback? onTap;

  /// Kesilmiş son ekstrenin kalan borcu; çubuğun kırmızı payı. Bilinmiyorsa
  /// kullanılan limitin tamamı gri çizilir.
  final String? statementBalance;

  /// Kartın normal yazı boyundaki en küçük yüksekliği.
  static const double height = 196;

  /// Kartın iç boşluğu.
  static const double _inset = AppSpacing.large;

  /// Limit bölümü borçtan bu kadar aşağıda başlar (teslimde 16 + 10).
  static const double _limitGap = AppSpacing.step10;

  /// Kullanılabilir limit bu oranın altına inince uyarı tonuna geçer.
  static const double _lowShare = 0.10;

  /// Kartın bu ekrandaki yüksekliği; kaydırma şeridi bu boyda kurulur.
  /// Büyük yazı ayarında yazılar sığsın diye kart da uzar.
  static double heightOf(
    BuildContext context, {
    CreditCardItem? card,
    double? width,
  }) {
    final scale = MediaQuery.textScalerOf(context);
    final inner = (width ?? MediaQuery.sizeOf(context).width * .9) - _inset * 2;
    Size measure(InlineSpan text, double maxWidth, {int? lines}) {
      final painter = TextPainter(
        text: text,
        textDirection: Directionality.of(context),
        textScaler: scale,
        maxLines: lines,
      )..layout(maxWidth: math.max(1, maxWidth));
      return painter.size;
    }

    final styles = _Styles(Theme.of(context));
    final currency = card?.currency ?? 'TRY';
    final name = measure(
      TextSpan(text: card?.name ?? '', style: styles.name),
      inner - 36,
      lines: 2,
    ).height;
    final label = measure(
      TextSpan(text: 'Güncel borç', style: styles.label),
      inner,
    );
    final amount = measure(
      TextSpan(
        text: MoneyText.format(
          MoneyText.unsigned(card?.currentDebt ?? '0'),
          currency,
        ),
        style: styles.amount,
      ),
      inner,
    );
    final debt = Size(
      math.max(label.width, amount.width),
      label.height + AppSpacing.xxSmall + amount.height,
    );
    Size labelOf(String name) =>
        measure(TextSpan(text: name, style: styles.label), inner);
    Size valueOf(int day) =>
        measure(TextSpan(text: dayOfMonth(day), style: styles.value), inner);
    final dueLabel = labelOf('Son ödeme');
    final closingLabel = labelOf('Kesim');
    final dueValue = valueOf(card?.paymentDueDay ?? 28);
    final closingValue = valueOf(card?.statementClosingDay ?? 28);
    final dates = Size(
      math.max(dueLabel.width, closingLabel.width) +
          AppSpacing.step10 +
          math.max(dueValue.width, closingValue.width),
      math.max(dueLabel.height, dueValue.height) +
          AppSpacing.step6 +
          math.max(closingLabel.height, closingValue.height) +
          AppSpacing.xSmall,
    );
    final middle = debt.width + AppSpacing.compact + dates.width <= inner
        ? math.max(debt.height, dates.height)
        : debt.height + AppSpacing.small + dates.height;
    final available = measure(
      TextSpan(
        children: [
          TextSpan(text: 'Kullanılabilir  ', style: styles.label),
          TextSpan(
            text: MoneyText.format(card?.availableLimit ?? '0', currency),
            style: styles.available,
          ),
        ],
      ),
      inner,
    );
    final total = measure(
      TextSpan(
        text: '/ ${MoneyText.format(card?.limit ?? '0', currency)}',
        style: styles.label,
      ),
      inner,
    );
    // Limit dolmak üzereyken satırın başındaki 16'lık uyarı simgesi ortaya
    // hizalanır ve satırı birkaç dp uzatır; ölçü onu da sayar.
    final cardLimit = double.tryParse(card?.limit ?? '') ?? 0;
    final cardAvailable = double.tryParse(card?.availableLimit ?? '') ?? 0;
    final warns = cardLimit > 0 && cardAvailable / cardLimit <= _lowShare;
    final availableLine = warns
        ? available.height + AppSpacing.xSmall
        : available.height;
    final limit =
        6 +
        AppSpacing.small +
        (available.width + 20 + AppSpacing.small + total.width <= inner
            ? math.max(availableLine, total.height)
            : availableLine + total.height);
    return math.max(
      height,
      _inset * 2 +
          name +
          AppSpacing.medium +
          middle +
          AppSpacing.medium +
          _limitGap +
          limit +
          // Yuvarlama payı: ölçü ile çizim arasındaki kesir farkı satırı
          // taşırmasın.
          AppSpacing.xSmall,
    );
  }

  /// `Ayın 15'i`: günün iyelik eki son rakama göre değişir.
  static String dayOfMonth(int day) {
    final suffix = switch (day) {
      10 => "'u",
      20 => "'si",
      _ => switch (day % 10) {
        1 || 5 || 8 => "'i",
        2 || 7 => "'si",
        3 || 4 => "'ü",
        6 => "'sı",
        _ => "'u",
      },
    };
    return 'Ayın $day$suffix';
  }

  @override
  Widget build(BuildContext context) {
    final styles = _Styles(Theme.of(context));
    const surfaces = AppSurfaces.dark;
    const colors = AppFinanceColors.dark;
    final inCredit = MoneyText.isNegative(card.currentDebt);
    final debtLabel = inCredit ? 'Karttaki fazla ödeme' : 'Güncel borç';
    final debt = MoneyText.format(
      inCredit ? MoneyText.unsigned(card.currentDebt) : card.currentDebt,
      card.currency,
    );
    final closing = dayOfMonth(card.statementClosingDay);
    final due = dayOfMonth(card.paymentDueDay);
    final limit = double.tryParse(card.limit) ?? 0;
    final available = double.tryParse(card.availableLimit) ?? 0;
    final low = limit > 0 && available / limit <= _lowShare;
    final double used = limit <= 0
        ? 0.0
        : ((limit - available) / limit).clamp(0.0, 1.0);
    final double period = statementBalance == null || limit <= 0
        ? 0.0
        : ((double.tryParse(statementBalance!) ?? 0) / limit).clamp(0.0, used);
    final availableText = MoneyText.format(card.availableLimit, card.currency);
    final limitText = MoneyText.format(card.limit, card.currency);
    final warn = low ? colors.expense : null;

    // İkinci satır 6 dp aşağıda başlar (teslimdeki ızgaranın satır arası).
    TableRow fact(String name, String value, {bool top = false}) {
      final gap = top
          ? const EdgeInsets.only(top: AppSpacing.step6)
          : EdgeInsets.zero;
      return TableRow(
        children: [
          Padding(
            padding: gap.add(const EdgeInsets.only(right: AppSpacing.step10)),
            child: Text(name, style: styles.label),
          ),
          Padding(
            padding: gap,
            child: Text(value, style: styles.value, textAlign: TextAlign.end),
          ),
        ],
      );
    }

    return Semantics(
      button: onTap != null,
      label:
          '${card.name}${card.isActive ? '' : ', pasif'}. $debtLabel $debt. '
          'Son ödeme $due, kesim $closing. '
          'Kullanılabilir limit $availableText, toplam $limitText.',
      excludeSemantics: true,
      child: Opacity(
        opacity: card.isActive ? 1 : 0.55,
        child: Material(
          color: AppSurfaces.light.ink,
          borderRadius: BorderRadius.circular(AppRadius.card),
          clipBehavior: Clip.antiAlias,
          child: InkWell(
            onTap: onTap,
            child: ConstrainedBox(
              constraints: BoxConstraints(
                minHeight: heightOf(context, card: card),
              ),
              child: Stack(
                children: [
                  // Köşedeki iki soluk daire: yüzeyi düz bir kutudan ayıran
                  // tek süs.
                  const Positioned(
                    right: -64,
                    top: -72,
                    child: _Glow(size: 208),
                  ),
                  const Positioned(
                    right: 40,
                    bottom: -120,
                    child: _Glow(size: 168),
                  ),
                  Padding(
                    padding: const EdgeInsets.all(_inset),
                    // Şeritteki kartlar en uzununun boyundadır; limit
                    // bölümü kısa kartta da en altta durur.
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      mainAxisSize: fill ? MainAxisSize.max : MainAxisSize.min,
                      children: [
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.stretch,
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Row(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Expanded(
                                  child: Text(
                                    card.isActive
                                        ? card.name
                                        : '${card.name} · Pasif',
                                    maxLines: 2,
                                    overflow: TextOverflow.ellipsis,
                                    style: styles.name,
                                  ),
                                ),
                                const SizedBox(width: AppSpacing.compact),
                                Icon(
                                  Icons.contactless_outlined,
                                  size: 24,
                                  color: surfaces.ink,
                                ),
                              ],
                            ),
                            const SizedBox(height: AppSpacing.medium),
                            Wrap(
                              alignment: WrapAlignment.spaceBetween,
                              crossAxisAlignment: WrapCrossAlignment.end,
                              runSpacing: AppSpacing.small,
                              spacing: AppSpacing.compact,
                              children: [
                                Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    Text(debtLabel, style: styles.label),
                                    const SizedBox(height: AppSpacing.xxSmall),
                                    Text(debt, style: styles.amount),
                                  ],
                                ),
                                Padding(
                                  padding: const EdgeInsets.only(
                                    bottom: AppSpacing.xSmall,
                                  ),
                                  // Teslimdeki iki sütunlu ızgara: etiketler
                                  // sola, günler sağa yaslı; aralarında 10.
                                  child: Table(
                                    defaultColumnWidth:
                                        const IntrinsicColumnWidth(),
                                    defaultVerticalAlignment:
                                        TableCellVerticalAlignment.baseline,
                                    textBaseline: TextBaseline.alphabetic,
                                    children: [
                                      fact('Son ödeme', due),
                                      fact('Kesim', closing, top: true),
                                    ],
                                  ),
                                ),
                              ],
                            ),
                          ],
                        ),
                        const SizedBox(height: AppSpacing.medium + _limitGap),
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.stretch,
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            _LimitBar(period: period, used: used),
                            const SizedBox(height: AppSpacing.small),
                            Wrap(
                              alignment: WrapAlignment.spaceBetween,
                              crossAxisAlignment: WrapCrossAlignment.center,
                              spacing: AppSpacing.small,
                              children: [
                                Text.rich(
                                  TextSpan(
                                    children: [
                                      if (low) ...[
                                        WidgetSpan(
                                          alignment:
                                              PlaceholderAlignment.middle,
                                          child: Icon(
                                            Icons.warning_amber_rounded,
                                            size: 16,
                                            color: warn,
                                          ),
                                        ),
                                        const TextSpan(text: ' '),
                                      ],
                                      TextSpan(
                                        text: 'Kullanılabilir  ',
                                        style: styles.label.copyWith(
                                          color: warn,
                                        ),
                                      ),
                                      TextSpan(
                                        text: availableText,
                                        style: styles.available.copyWith(
                                          color: warn,
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                                Text('/ $limitText', style: styles.label),
                              ],
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Kartın yazı ölçüleri (teslimin `up` boyu); yalnız bu kartta kullanılır.
class _Styles {
  _Styles(ThemeData theme)
    : name = theme.textTheme.titleSmall!.copyWith(
        fontSize: AppTextSizes.row,
        fontWeight: FontWeight.w600,
        height: 1.25,
        color: AppSurfaces.dark.ink,
      ),
      label = theme.textTheme.bodySmall!.copyWith(
        fontSize: AppTextSizes.caption,
        height: 1.25,
        color: AppSurfaces.dark.inkMuted,
      ),
      // Günler etiketlerden bir boy büyük, ama tutardan küçük (kullanıcı,
      // 13 Kasım 2026: `Ayın 25'i` iri duruyordu).
      value = theme.textTheme.titleSmall!.copyWith(
        fontSize: AppTextSizes.body,
        fontWeight: FontWeight.w600,
        height: 1.25,
        color: AppSurfaces.dark.ink,
      ),
      amount = theme.textTheme.headlineSmall!.copyWith(
        fontSize: AppTextSizes.amount,
        fontWeight: FontWeight.w700,
        height: 1.2,
        color: AppSurfaces.dark.ink,
        fontFeatures: AppTypography.tabularFigures,
      ),
      available = theme.textTheme.titleSmall!.copyWith(
        fontSize: AppTextSizes.compact,
        fontWeight: FontWeight.w700,
        height: 1.25,
        color: AppSurfaces.dark.ink,
        fontFeatures: AppTypography.tabularFigures,
      );

  final TextStyle name;
  final TextStyle label;
  final TextStyle value;
  final TextStyle amount;
  final TextStyle available;
}

/// Limit çubuğu: dönem borcu kırmızı, borcun geri kalanı gri, boşluk
/// kullanılabilir limit. Parçaların arasında 2 dp ara vardır.
class _LimitBar extends StatelessWidget {
  const _LimitBar({required this.period, required this.used});

  /// Dönem borcunun limite oranı.
  final double period;

  /// Kullanılan limitin (güncel borç) limite oranı.
  final double used;

  @override
  Widget build(BuildContext context) {
    const surfaces = AppSurfaces.dark;
    const colors = AppFinanceColors.dark;
    final rest = (used - period).clamp(0.0, 1.0);
    return ExcludeSemantics(
      child: ClipRRect(
        borderRadius: BorderRadius.circular(AppRadius.chip),
        child: Container(
          height: 6,
          decoration: BoxDecoration(
            color: surfaces.cardMuted,
            border: Border.all(color: surfaces.border),
            borderRadius: BorderRadius.circular(AppRadius.chip),
          ),
          child: LayoutBuilder(
            builder: (context, constraints) {
              final width = constraints.maxWidth;
              return Row(
                children: [
                  if (period > 0)
                    Container(width: width * period, color: colors.expenseFill),
                  if (period > 0 && rest > 0)
                    const SizedBox(width: AppSpacing.xxSmall),
                  if (rest > 0)
                    Container(
                      width: math.max(0, width * rest - AppSpacing.xxSmall),
                      color: colors.planned,
                    ),
                ],
              );
            },
          ),
        ),
      ),
    );
  }
}

class _Glow extends StatelessWidget {
  const _Glow({required this.size});

  final double size;

  @override
  Widget build(BuildContext context) => Container(
    width: size,
    height: size,
    decoration: BoxDecoration(
      shape: BoxShape.circle,
      color: AppSurfaces.dark.ink.withValues(alpha: 0.06),
    ),
  );
}
