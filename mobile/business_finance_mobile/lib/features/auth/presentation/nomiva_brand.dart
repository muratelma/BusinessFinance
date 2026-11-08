import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

/// Nomiva giriş akışının renkleri ve yazısı.
///
/// Kaynak `design/nomiva-handoff/TASARIM_NOTLARI.md` renk tablosudur. Bu renkler
/// **yalnız giriş akışında** kullanılır (kullanıcı kararı, 7 Kasım 2026):
/// uygulamanın geri kalanı kendi temasında kalır. Üst bölüm iki temada da
/// siyahtır; değişen yalnız form kağıdıdır.
@immutable
class NomivaColors {
  const NomivaColors._({
    required this.paper,
    required this.text,
    required this.muted,
    required this.field,
    required this.fieldIcon,
    required this.line,
    required this.chip,
    required this.button,
    required this.onButton,
    required this.error,
    required this.errorFill,
  });

  /// Açılış ve üst bölüm zemini. Telefonun kendi açılış ekranı da bu renktir;
  /// farklı olsaydı Flutter başlarken bir yanıp sönme görünürdü.
  static const Color top = Color(0xFF050608);
  static const Color brandName = Color(0xFFF2F3F7);
  static const Color tagline = Color(0xFFA3A9BA);

  /// Vurgu (indigo). Küçük metinde kullanılmaz; yalnız açılıştaki hale ve
  /// dalgada ve "İşletmem var" anahtarında.
  static const Color accent = Color(0xFF6A6CF6);
  static const Color accent2 = Color(0xFF8B5CF6);

  /// Anahtarın topuzu, iki temada da.
  static const Color thumb = Color(0xFFFFFFFF);

  static const light = NomivaColors._(
    paper: Color(0xFFFFFFFF),
    text: Color(0xFF16191D),
    muted: Color(0xFF5E6670),
    field: Color(0xFFF2F4F6),
    fieldIcon: Color(0xFF8B939D),
    line: Color(0xFFE3E6EA),
    chip: Color(0xFFFFFFFF),
    button: Color(0xFF16191D),
    onButton: Color(0xFFFFFFFF),
    error: Color(0xFFCF2E1F),
    errorFill: Color(0xFFFBE7E5),
  );

  static const dark = NomivaColors._(
    paper: Color(0xFF161922),
    text: Color(0xFFF2F3F7),
    muted: Color(0xFFA3A9BA),
    field: Color(0xFF1F2330),
    fieldIcon: Color(0xFF7D8596),
    line: Color(0xFF2C3140),
    chip: Color(0xFF1F2330),
    button: Color(0xFFF2F3F7),
    onButton: Color(0xFF0E1015),
    error: Color(0xFFFF8A80),
    errorFill: Color(0xFF3A1E22),
  );

  static NomivaColors of(BuildContext context) =>
      Theme.of(context).brightness == Brightness.dark ? dark : light;

  final Color paper;
  final Color text;
  final Color muted;
  final Color field;
  final Color fieldIcon;
  final Color line;

  /// Google / Apple düğmesinin zemini.
  final Color chip;
  final Color button;
  final Color onButton;
  final Color error;
  final Color errorFill;
}

/// Giriş akışının ölçüleri, **tasarım biriminde** (320 × 700 pt'lik tasarım
/// ekranı; `design/nomiva-handoff/TASARIM_NOTLARI.md` ve prototipin CSS'i).
///
/// Bu sayılar ekrana doğrudan yazılmaz: hepsi [NomivaScale] ile çarpılır.
/// Tasarım 320 pt genişlikte çizildi; ölçüler olduğu gibi alınınca 412 dp'lik
/// bir telefonda her şey ~%22 küçük kalmıştı (7 Kasım 2026).
abstract final class NomivaSpacing {
  /// Tasarım ekranı.
  static const double designWidth = 320;
  static const double designHeight = 700;

  /// Tasarım ekranında durum çubuğu; dikey konumlar onu içerir.
  static const double statusBar = 44;

  /// Kağıdın üst kenarından formun başına: kavisli kenarın yüksekliği.
  static const double paperInset = 55;

  /// Kavisli kenarın çizildiği bant ve ortada ne kadar aşağı indiği.
  static const double edgeHeight = 56;
  static const double edgeDip = 52;

  /// Formun yan boşluğu ve panelin üst payı.
  static const double side = 24;
  static const double panelTop = 2;

  /// Öğeler arası: girişte 11, kayıtta ve parola sıfırlamada 8.
  static const double gap = 11;
  static const double gapTight = 8;

  /// Kayıtta ve parola sıfırlamada ana düğmenin üstüne eklenen pay;
  /// "Parolamı unuttum"un altındaki boşluk da bu kadardır.
  static const double beforeButton = 10;

  /// Etiket ile alan arası.
  static const double labelGap = 5;

  /// Logo, ad ve slogan arası.
  static const double brandGap = 14;

  /// Logonun tasarımdaki genişliği.
  static const double markWidth = 84;

  /// Alan: yükseklik, köşe, iç boşluk, simge, simge ile yazı arası.
  static const double fieldHeight = 46;
  static const double radius = 12;
  static const double fieldInset = 14;
  static const double fieldIcon = 18;
  static const double iconGap = 10;

  /// Ana düğme yüksekliği (tam yuvarlak).
  static const double buttonHeight = 50;
  static const double spinner = 20;

  /// Sosyal düğme: çap, simge, aralarındaki boşluk.
  static const double socialSize = 52;
  static const double socialIcon = 22;
  static const double socialGap = 16;

  /// "ya da" çizgisi ile yazısı arası.
  static const double orGap = 10;

  /// Alt bağlantı sosyal düğmelerden 30 pt aşağıdadır (öğe aralığına ek);
  /// altında 28 pt pay.
  static const double footerTop = 30;
  static const double footerBottom = 28;

  /// Klavye açıkken alanın altında bırakılan pay: hata yazısı ve bir boşluk
  /// klavyenin üstünde görünür kalır.
  static const double keyboardClearance = 48;

  /// Uyarı kutusunun iç boşluğu.
  static const double bannerX = 12;
  static const double bannerY = 10;

  /// "İşletmem var" kutusunun iç boşluğu.
  static const double switchX = 14;
  static const double switchY = 11;

  /// Alt yazı başlığa 6 pt yaklaşır, altında 2 pt pay bırakır.
  static const double subtitlePull = 6;
  static const double subtitleAfter = 2;

  /// Açılış: logonun dikey yeri, hale çapı ve üst kenarı, dalga yüksekliği.
  static const double splashBrandY = 250;
  static const double splashDip = 120;
  static const double glowSize = 300;
  static const double glowTop = 190;
  static const double wavesHeight = 190;
}

/// Tasarım biriminden ekran birimine çeviren tek katsayı.
///
/// Telefon dikeyde (`compact`) tasarım ekranı ekrana oranla büyür:
/// katsayı genişliğin ve yüksekliğin tasarıma oranlarının küçüğüdür, böylece
/// iki telefonda da neyin nerede durduğu aynı kalır, kısa ekranda form ekranın
/// dışına taşmaz. Telefon yatayda ve tablette büyüme [maxScale]'de durur;
/// form ortada bir telefon genişliğinde kalır, üst bölüm ve kağıt ekrana
/// yayılır. Erişilebilirlik yazı ölçeği bu katsayıyla çarpılmaz, üstüne biner.
@immutable
class NomivaScale {
  const NomivaScale(this.factor);

  /// Büyük telefonların (≈430 pt) oranı; tablette form bundan büyümez.
  static const double maxScale = 1.35;

  /// Bundan küçülürse yazı okunmaz olurdu; altında form kayar.
  static const double minScale = .85;

  final double factor;

  /// Tasarım birimindeki [value]'nun ekrandaki karşılığı.
  double call(double value) => value * factor;

  /// Formun en geniş hâli: en büyük telefondaki genişliği. Telefonda form
  /// her zaman ekranı (yan boşluklar dışında) doldurur; tablette ve telefon
  /// yatayda burada durur.
  static const double maxFormWidth = NomivaSpacing.designWidth * maxScale;

  static NomivaScale forSize(Size size) {
    final byWidth = size.width / NomivaSpacing.designWidth;
    final byHeight = size.height / NomivaSpacing.designHeight;
    final factor = byWidth < byHeight ? byWidth : byHeight;
    return NomivaScale(factor.clamp(minScale, maxScale));
  }

  static NomivaScale of(BuildContext context) =>
      context.dependOnInheritedWidgetOfExactType<NomivaScaleScope>()?.scale ??
      const NomivaScale(1);
}

/// Akışın altındaki her parçaya aynı katsayıyı verir.
class NomivaScaleScope extends InheritedWidget {
  const NomivaScaleScope({
    required this.scale,
    required super.child,
    super.key,
  });

  final NomivaScale scale;

  @override
  bool updateShouldNotify(NomivaScaleScope oldWidget) =>
      oldWidget.scale.factor != scale.factor;
}

/// Figtree yalnız giriş akışında; dosya uygulamanın içindedir.
const String nomivaFont = 'Figtree';

/// Figtree'nin "normal" satır yüksekliği (yazı boyunun katı); prototipteki CSS
/// de bunu kullanır. Açıkça verilir: verilmezse yazı Material 3'ün gövde
/// satırını (1,43) miras alır ve her satır tasarımdakinden uzun olur.
const double nomivaLineHeight = 1.2;

/// Harf aralığı sıfırdır (prototipin CSS'i). Açıkça verilir: verilmezse yazı
/// Material 3'ün stillerinden 0,25–0,4 aralık miras alır, satırlar
/// tasarımdakinden uzun olur ve hata cümleleri ikinci satıra taşar.
const double nomivaLetterSpacing = 0;

/// Giriş akışının temasını Figtree'ye çevirir; alan içi metin, hata satırı ve
/// düğme yazısı da aynı yazıyla çizilir.
ThemeData nomivaAuthTheme(ThemeData base) =>
    base.copyWith(textTheme: base.textTheme.apply(fontFamily: nomivaFont));

const _markAsset = 'assets/brand/nomiva_isaret_koyu_zemin.svg';

/// Logoyu önceden çözer. Açılış animasyonu logoyla başlar; SVG ilk karede
/// hazır olmazsa logo bir an geç belirirdi. Oturum kontrolü sürerken
/// çağrılır.
Future<void> precacheNomivaMark(BuildContext context) =>
    const SvgAssetLoader(_markAsset).loadBytes(context);

/// Logonun kendisi. Koyu zemin sürümü kullanılır: üst bölüm iki temada da
/// siyahtır.
class NomivaMark extends StatelessWidget {
  const NomivaMark({required this.width, super.key});

  final double width;

  @override
  Widget build(BuildContext context) => SvgPicture.asset(
    _markAsset,
    width: width,
    // Kaynak SVG'nin oranı 636 × 684.
    height: width * 684 / 636,
    semanticsLabel: 'Nomiva',
  );
}
