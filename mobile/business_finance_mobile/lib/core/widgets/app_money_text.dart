import 'package:flutter/material.dart';

import '../formatters/money_text.dart';
import '../theme/app_finance_colors.dart';
import '../theme/app_typography.dart';

/// Bir tutarın finansal anlamı: rengi, işareti ve ekran okuyucuya söylenen
/// cümle tek yerden gelir.
enum AppMoneyEffect { income, expense, neutral }

/// Tasarım sisteminin para ölçeği (`AppMoneyText size`): ekranın tek büyük
/// sayısı `hero`, kart içi toplam `metric`, liste satırı `row`, satır içi
/// ikincil tutar `body`.
enum AppMoneySize { hero, metric, row, body }

/// Tutar gösterimi.
///
/// Üç işi birden yapar ve üçü de daha önce her çağrı yerinde tekrar
/// karar veriliyordu:
/// - rengi `AppFinanceColors`'tan alır (ham `Colors.green` yerine),
/// - sabit genişlikli rakam kullanır, böylece tutarlar sütunda hizalanır,
/// - ekran okuyucuya `₺` glifini değil okunabilir bir cümle verir.
class AppMoneyText extends StatelessWidget {
  const AppMoneyText({
    required this.amount,
    required this.currency,
    super.key,
    this.effect,
    this.isCancelled = false,
    this.signed = false,
    this.style,
    this.textAlign,
    this.semanticsSuffix,
    this.onContainer = false,
    this.size,
  });

  /// Backend'in kayıpsız string sözleşmesindeki tutar (`1234.5600`).
  final String amount;
  final String currency;

  /// Null ise renk temanın varsayılan metin renginde kalır.
  final AppMoneyEffect? effect;

  /// İptal edilmiş hareket tutarı soluk ve nötr okunur; iptal bilgisi ayrıca
  /// görünür bir etiketle taşınır, renkle değil.
  final bool isCancelled;

  /// Gelirin başına `+`, giderin başına `-` koyar.
  final bool signed;

  final TextStyle? style;
  final TextAlign? textAlign;

  /// Ekran okuyucu cümlesinin sonuna eklenecek bağlam (ör. `İptal edildi`).
  final String? semanticsSuffix;

  /// Tutar kendi rolünün **renkli zemini** üzerinde gösteriliyor.
  ///
  /// Yüzey rengi değişince metin rengi de değişmek zorundadır: yüzeye göre
  /// seçilmiş `income` tonu, gelir zemini üzerinde 3,95:1'e düşüyor ve AA
  /// eşiğini geçemiyor. Bu bayrak verildiğinde rol, kontrast kapısında zaten
  /// ölçülen `on*Container` çiftine döner.
  final bool onContainer;

  /// Verilirse taban stil tasarım sisteminin para ölçeğinden gelir; [style]
  /// yine de üzerine yazılır (ör. ağırlık veya renk inceltmesi).
  final AppMoneySize? size;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final scaled = _scaled(theme.textTheme);
    final base = scaled == null
        ? (style ?? theme.textTheme.titleMedium ?? const TextStyle())
        : scaled.merge(style);
    final money = size == AppMoneySize.hero
        ? AppTypography.heroMoney(base)
        : AppTypography.money(base);

    return Text(
      _displayText,
      textAlign: textAlign,
      semanticsLabel: _semanticsLabel,
      style: money.copyWith(
        color: _color(context),
        decoration: isCancelled ? TextDecoration.lineThrough : null,
      ),
    );
  }

  TextStyle? _scaled(TextTheme text) => switch (size) {
    null => null,
    AppMoneySize.hero => text.displaySmall,
    AppMoneySize.metric => text.titleLarge?.copyWith(
      fontWeight: FontWeight.w600,
    ),
    AppMoneySize.row => text.titleSmall,
    AppMoneySize.body => text.bodyMedium,
  };

  Color _color(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    if (isCancelled) {
      return onContainer ? colors.onCancelledContainer : colors.cancelled;
    }
    if (onContainer) {
      return switch (effect) {
        AppMoneyEffect.income => colors.onIncomeContainer,
        AppMoneyEffect.expense => colors.onExpenseContainer,
        AppMoneyEffect.neutral => colors.onNeutralContainer,
        null => Theme.of(context).colorScheme.onSurface,
      };
    }
    return switch (effect) {
      AppMoneyEffect.income => colors.income,
      AppMoneyEffect.expense => colors.expense,
      AppMoneyEffect.neutral => colors.neutral,
      null => Theme.of(context).colorScheme.onSurface,
    };
  }

  String get _formatted => MoneyText.format(amount, currency);

  String get _displayText {
    // İptal edilmiş tutar işaret taşımaz: artık bir yöne para hareketi değil,
    // üstü çizili bir kayıttır (DS `AppMoneyText`).
    if (!signed || isCancelled) return _formatted;
    return switch (effect) {
      AppMoneyEffect.income => '+$_formatted',
      AppMoneyEffect.expense => '-$_formatted',
      _ => _formatted,
    };
  }

  /// `-₺1.234,56` ekran okuyucuda glif glif okunur. Anlam kelimeyle verilir.
  String get _semanticsLabel {
    final buffer = StringBuffer()
      ..write(_spokenAmount)
      ..write(' ')
      ..write(_spokenCurrency);
    final meaning = switch (effect) {
      AppMoneyEffect.income => 'gelir',
      AppMoneyEffect.expense => 'gider',
      AppMoneyEffect.neutral => 'bakiye etkisi nötr',
      null => null,
    };
    if (meaning != null) buffer.write(' $meaning');
    if (semanticsSuffix != null && semanticsSuffix!.isNotEmpty) {
      buffer.write('. $semanticsSuffix');
    }
    return buffer.toString();
  }

  /// Biçimlenmiş metinden para simgesini düşürür; sayı okunuşu kalır.
  String get _spokenAmount =>
      _formatted.replaceAll('₺', '').replaceAll(currency, '').trim();

  String get _spokenCurrency => switch (currency) {
    'TRY' => 'lira',
    _ => currency,
  };
}
