import 'package:flutter/widgets.dart';

import 'app_breakpoints.dart';

abstract final class AppSpacing {
  /// Başlık ile alt satırı arasındaki nefes payı (tasarımda `gap: 2`).
  static const double xxSmall = 2;
  static const double xSmall = 4;
  static const double small = 8;

  /// Kart ve satır içinin sık adımı: 8 dar, 16 geniş kaldığında.
  static const double compact = 12;
  static const double medium = 16;
  static const double large = 24;
  static const double xLarge = 32;
  static const double xxLarge = 48;

  /// Ara adımlar. Bileşen içi ince ayarlarda toplama olarak yazılıyorlardı
  /// (`small + xxSmall`); değerleri korunarak adlandırıldı. Yeni kodda
  /// kullanılmaz: merdivenin kendi adımı seçilir. Komşu adıma birleşmeleri
  /// ayrı bir karardır.
  static const double step6 = 6;
  static const double step10 = 10;
  static const double step14 = 14;
  static const double step20 = 20;

  /// Kaydırılabilir listelerin altına bırakılan pay: son satır kayan eylem
  /// butonunun altında kalmamalı. Ölçekteki bir adım değil, tek amaçlı bir
  /// yerleşim sabitidir.
  static const double fabClearance = 96;
}

/// Sayfa gövdesinin iç boşlukları: başlığın altındaki içeriğin nereden
/// başladığı ve nerede bittiği tek yerden gelir. Sayfalar kaydırılan gövdeye
/// kendi `EdgeInsets` değerini yazmaz, içeriğin türüne uyan adı seçer; bir
/// değeri değiştirmek bütün uygulamayı değiştirir. Kural
/// `documentation/design-system.md` "Başlık ve boşluk standardı" bölümündedir.
abstract final class AppPageInsets {
  /// Ekran kenarı.
  static const double edge = AppSpacing.medium;

  /// Liste ya da kartla başlayan sayfa: başlığın altında ek boşluk yok.
  static const EdgeInsets list = EdgeInsets.fromLTRB(
    edge,
    0,
    edge,
    AppSpacing.xLarge,
  );

  /// [list], sonu kayan `+` düğmesinin altında kalmayacak kadar uzun.
  static const EdgeInsets listWithFab = EdgeInsets.fromLTRB(
    edge,
    0,
    edge,
    AppSpacing.fabClearance,
  );

  /// Kenardan kenara satırlarla çizilen liste (gün başlıklı akışlar).
  static const EdgeInsets fullBleedList = EdgeInsets.only(
    bottom: AppSpacing.xLarge,
  );

  /// Etiketli alanla başlayan form: etiket çerçevenin üstüne taştığı için
  /// 8 dp, görünen boşluk listeyle aynı kalır.
  static const EdgeInsets form = EdgeInsets.fromLTRB(
    edge,
    AppSpacing.small,
    edge,
    AppSpacing.large,
  );

  /// Sekme çubuğunun altındaki liste.
  static const EdgeInsets underTabs = EdgeInsets.fromLTRB(
    edge,
    AppSpacing.medium,
    edge,
    AppSpacing.xLarge,
  );

  /// [underTabs], sonu kayan `+` düğmesinin altında kalmayacak kadar uzun.
  static const EdgeInsets underTabsWithFab = EdgeInsets.fromLTRB(
    edge,
    AppSpacing.medium,
    edge,
    AppSpacing.fabClearance,
  );

  /// Alt gezinme çubuğuyla birlikte görünen sayfanın gövdesi: çubuk içeriğin
  /// üstünde yüzer, son satır çubuğun perdesinin başladığı yerde biter.
  /// Çubuğun kapladığı pay `MediaQuery`'den okunur (kabuk `extendBody` ile
  /// bildirir); çubuğun olmadığı yerde (geniş ekran, kabuk dışı rota) [base]
  /// olduğu gibi kalır.
  static EdgeInsets aboveBar(BuildContext context, EdgeInsets base) {
    final bar = MediaQuery.paddingOf(context).bottom;
    return bar > base.bottom ? base.copyWith(bottom: bar) : base;
  }

  /// Yatay kayan süzgeç şeridi: yalnız kenar payı. Dikey nefesi çipin
  /// kendi dokunma payı verir.
  static const EdgeInsets filterStrip = EdgeInsets.symmetric(horizontal: edge);

  /// Başlığa yapışık denetim (Özet'in kapsam anahtarı, Kasa'nın seçicisi):
  /// altındaki içerikle arası 16 dp, içerik ayrıca üst boşluk eklemez.
  static const EdgeInsets headerControl = EdgeInsets.fromLTRB(
    edge,
    0,
    edge,
    AppSpacing.medium,
  );
}

/// Alttan açılan panelin (form ve ayrıntı) iç boşlukları. Panelin kenarı
/// sayfanın kenarıyla aynıdır; başlık, gövde ve eylem satırı aynı hizadan
/// başlar. Onay kutusu (`AppConfirmDialog`) panel değildir, Material'ın
/// 24 dp'lik kutu payını korur.
abstract final class AppSheetInsets {
  /// Panelin iki yanı.
  static const double edge = AppPageInsets.edge;

  /// Panelin ilk satırının üstü. Telefonda panelin tutamacı kendi boşluğunu
  /// taşır ve başlık hemen altından başlar; geniş ekranda aynı içerik
  /// tutamaçsız bir kutuda açılır ve üstten pay ister.
  static double top(BuildContext context) =>
      context.windowSize.isCompact ? 0 : AppSpacing.large;

  /// Başlık ve açıklaması.
  static EdgeInsets header(BuildContext context) =>
      EdgeInsets.fromLTRB(edge, top(context), edge, AppSpacing.small);

  /// Eylem satırı sabit olmayan panelin (ayrıntı, menü, süzgeç) kaydırılan
  /// gövdesi. [keyboard] klavyenin kapladığı paydır.
  static EdgeInsets detail(BuildContext context, {double keyboard = 0}) =>
      EdgeInsets.fromLTRB(
        edge,
        top(context),
        edge,
        AppSpacing.large + keyboard,
      );

  /// Kaydırılan gövde; klavye payı ayrıca eklenir.
  static const EdgeInsets body = EdgeInsets.fromLTRB(
    edge,
    AppSpacing.small,
    edge,
    AppSpacing.medium,
  );

  /// Gövdenin dışında duran hata satırı.
  static const EdgeInsets error = EdgeInsets.fromLTRB(
    edge,
    0,
    edge,
    AppSpacing.small,
  );

  /// Alttaki eylem satırı.
  static const EdgeInsets actions = EdgeInsets.fromLTRB(
    edge,
    AppSpacing.small,
    edge,
    AppSpacing.medium,
  );
}

/// Sayfa içindeki dikey aralıklar. Sayfalar `SizedBox(height: …)` için sayı
/// yerine aralığın anlamını yazar.
abstract final class AppGaps {
  /// Aynı gruptaki kartlar arası.
  static const double card = AppSpacing.small;

  /// Bölüm başlığı ile altındaki kart arası.
  static const double sectionTitle = AppSpacing.small;

  /// Uyarı ya da not ile ilgili olduğu içerik arası.
  static const double notice = AppSpacing.small;

  /// Form alanları arası.
  static const double field = AppSpacing.medium;

  /// Aynı bölümdeki ayrı bloklar arası (düğme ile liste, kart ile not).
  static const double block = AppSpacing.medium;

  /// İki bölüm arası.
  static const double section = AppSpacing.large;

  /// Yan yana çipler arası.
  static const double chip = AppSpacing.small;

  /// Satırda başlık ile alt yazı arası.
  static const double rowText = AppSpacing.xxSmall;
}
