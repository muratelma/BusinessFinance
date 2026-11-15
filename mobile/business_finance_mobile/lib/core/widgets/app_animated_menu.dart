import 'dart:math' as math;

import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import 'app_form_sheet.dart';

/// Alanın üstünde yüzen, açılırken ve kapanırken hareket eden menü
/// (kullanıcı, 13 Kasım 2026: filtrenin kategori menüsü bir anda ve büyük
/// beliriyordu; doğal bir geçiş istendi).
///
/// Hareket Material 3'ün menü hareketini izler:
/// - **Açılış (220 ms):** kutu alanın kenarından dikeyde büyür (%85 → %100)
///   ve belirir; satırlar biraz gecikmeyle gelir, önce kutu sonra içerik
///   okunur. Eğri yavaşlayarak durur (`easeOutCubic`).
/// - **Kapanış (150 ms):** aynı yolun tersi, daha kısa.
/// - Telefonda "animasyonları kapat" açıksa hareket yoktur.
///
/// Yön ve boy: altında yer varsa aşağı, yoksa yeri çok olan tarafa açılır.
/// Boy en çok [maxMenuHeight]'tır ve ekranın görünen alanını aşmaz; uzun
/// liste kutunun içinde kayar, yarım görünen son satır kaydığını söyler.
///
/// Dışarıya dokunmak ve geri tuşu menüyü kapatır.
class AppAnimatedMenu extends StatefulWidget {
  const AppAnimatedMenu({
    required this.builder,
    required this.menuBuilder,
    super.key,
    this.maxMenuHeight = AppSizes.menuMaxHeight,
    this.spanKey,
    this.expectedHeight,
    this.anchorHeight,
    this.menuWidth,
    this.menuScreenLeft,
  });

  /// Menünün genişliği; boşsa alanın (ya da [spanKey]'in) genişliği. Ekrana
  /// sığmıyorsa ekranın genişliğine iner.
  final double? menuWidth;

  /// Menünün sol kenarının ekrandaki yeri; boşsa alanın sol kenarı. Çipin
  /// menüsü çipe değil sayfanın kenarına hizalanır.
  final double? menuScreenLeft;

  /// Menünün içeriğinin yaklaşık boyu. Panelde menüye yer açarken kullanılır:
  /// üç satırlık menü için yedi satırlık yer ayrılmaz. Boşsa [maxMenuHeight].
  final double? expectedHeight;

  /// Alanın kutusunun boyu. Alanın altında yardım ya da hata yazısı varsa
  /// menü yazının değil kutunun altından açılır ve yazıyı örter: alanla
  /// menünün arasına bir şey girmez (kullanıcı, 15 Kasım 2026). Boşsa menü
  /// alanın tamamının altından açılır.
  final double? anchorHeight;

  /// Menü alanın değil bu kutunun genişliğinde ve onunla hizalı açılır
  /// (dar alanın menüsü bütün satıra yayılır: taksit sayısı). Boşsa menü
  /// alanın genişliğindedir.
  final GlobalKey? spanKey;

  /// Alan: açık mı ve aç/kapat.
  final Widget Function(BuildContext context, bool open, VoidCallback toggle)
  builder;

  /// Menünün satırları; satır seçilince `close` çağrılır.
  final Widget Function(BuildContext context, VoidCallback close) menuBuilder;

  /// Menünün en çok boyu; ortak kural [AppSizes.menuMaxHeight].
  final double maxMenuHeight;

  @override
  State<AppAnimatedMenu> createState() => _AppAnimatedMenuState();
}

class _AppAnimatedMenuState extends State<AppAnimatedMenu>
    with SingleTickerProviderStateMixin {
  static const _gap = AppSpacing.small;

  /// Ekranın kenarına bırakılan pay.
  static const _margin = AppSpacing.medium;

  final _portal = OverlayPortalController();
  final _link = LayerLink();
  final _fieldKey = GlobalKey();
  late final AnimationController _motion;

  bool _open = false;
  bool _below = true;
  double _width = 280;
  double _dx = 0;
  double _dy = 0;

  /// Panelde menüye ayrılan yer: alanın altına akışa eklenir, panel o kadar
  /// uzar. Yüzen menü bu yerin üstünde durur; eylem satırının üstüne taşmaz.
  double _reserve = 0;
  final _menuKey = GlobalKey();
  double _maxHeight = AppSizes.menuMaxHeight;
  final _scroll = ScrollController();

  @override
  void initState() {
    super.initState();
    // Burada kurulur: tembel kurulsaydı hiç açılmayan menüde ilk kez
    // `dispose` içinde kurulur ve devre dışı ağaçta hata verirdi.
    _motion = AnimationController(
      vsync: this,
      duration: AppMenuMotion.open,
      reverseDuration: AppMenuMotion.close,
    );
  }

  @override
  void dispose() {
    _motion.dispose();
    _scroll.dispose();
    super.dispose();
  }

  bool get _still => MediaQuery.of(context).disableAnimations;

  void _toggle() => _open ? _close() : _show();

  /// Alanın altındaki ve üstündeki yer. Alt sınır ekranın görünen sonu ve
  /// alanın içinde durduğu kaydırılan gövdenin sonudur: menü panelin sabit
  /// eylem satırının (`Vazgeç`, `Kaydet`) üstüne taşmaz.
  ({RenderBox box, double below, double above})? _space() {
    final box = _fieldKey.currentContext?.findRenderObject() as RenderBox?;
    if (box == null || !box.hasSize || !box.attached) return null;
    final media = MediaQuery.of(context);
    final top = box.localToGlobal(Offset.zero).dy;
    final bottom = top + (widget.anchorHeight ?? box.size.height);
    // Alttaki sistem çubuğu ve yüzen alt çubuk da yer sayılmaz
    // (`padding.bottom`); menü onların arkasında bitmesin.
    var limit =
        media.size.height -
        math.max(media.viewInsets.bottom, media.padding.bottom);
    final scrollable = Scrollable.maybeOf(context);
    final viewport = scrollable?.context.findRenderObject();
    if (scrollable != null &&
        axisDirectionToAxis(scrollable.axisDirection) == Axis.vertical &&
        viewport is RenderBox &&
        viewport.hasSize) {
      limit = math.min(
        limit,
        viewport.localToGlobal(Offset(0, viewport.size.height)).dy,
      );
    }
    return (
      box: box,
      below: limit - bottom - _gap - _bottomMargin,
      above: top - media.padding.top - _gap - _margin,
    );
  }

  /// Menünün alt payı: ekranda bir ölçek (16), panelde menüye yer açıldıysa
  /// kategori menüsünün eylem satırına bıraktığı kadar (8).
  double get _bottomMargin => _fitted ? AppSpacing.small : _margin;

  /// Panel menü için boylandı mı ([AppFormSheetFit]).
  bool _fitted = false;
  double? _savedOffset;

  double _boundedHeight(({RenderBox box, double below, double above}) space) =>
      math.max(
        AppSizes.button,
        math.min(widget.maxMenuHeight, _below ? space.below : space.above),
      );

  Future<void> _show() async {
    var space = _space();
    if (space == null) return;
    final desired = math.min(
      widget.maxMenuHeight,
      widget.expectedHeight ?? widget.maxMenuHeight,
    );
    // Panelde yer yetmiyorsa menü yukarı açılmaz ve eylem satırına taşmaz:
    // panel alan + menü kadar boylanır ve alan başa kayar (kategori alanının
    // paneldeki davranışı); panelin diğer alanları o sırada görünmez. Yer
    // yetiyorsa hiçbir şey itilmez, menü alanların üstünde yüzer.
    final fit = AppFormSheetFit.maybeOf(context);
    if (space.below < desired && fit != null) {
      final position = Scrollable.maybeOf(context)?.position;
      _savedOffset = position?.hasPixels == true ? position!.pixels : null;
      setState(() {
        _fitted = true;
        _reserve = _gap + desired;
      });
      _fitSheet(fit, desired);
      // Panelin uzaması, alanın başa kayması ve menünün açılması birlikte
      // olur; sırayla olsaydı menü panel boylandıktan sonra, gecikerek
      // belirirdi. Menünün boyu panelin verebileceği yerden bilinir, ölçüm
      // beklenmez; menü alanı izlediği için panel uzarken onunla taşınır.
      await WidgetsBinding.instance.endOfFrame;
      if (!mounted) return;
      _scrollFieldToTop();
      space = _space();
      if (space == null) return;
    }
    final box = space.box;
    final want = math.min(desired, 240.0);
    _below = _fitted || space.below >= want || space.below >= space.above;
    _maxHeight = _fitted && fit != null
        ? math.max(
            AppSizes.button,
            math.min(
              widget.maxMenuHeight,
              fit.maxHeight -
                  AppSpacing.medium -
                  (widget.anchorHeight ?? box.size.height) -
                  _gap -
                  AppSpacing.small,
            ),
          )
        : _boundedHeight(space);
    // Menü kutunun altından açılır; alttaki yardım yazısını örter.
    _dy = widget.anchorHeight == null
        ? 0
        : widget.anchorHeight! - box.size.height;
    final span = widget.spanKey?.currentContext?.findRenderObject();
    if (span is RenderBox && span.hasSize) {
      _width = span.size.width;
      _dx =
          span.localToGlobal(Offset.zero).dx -
          box.localToGlobal(Offset.zero).dx;
    } else {
      _width = box.size.width;
      _dx = 0;
    }
    if (widget.menuWidth != null) {
      final screen = MediaQuery.sizeOf(context).width;
      final fieldLeft = box.localToGlobal(Offset.zero).dx;
      _width = math.min(widget.menuWidth!, screen - _margin * 2);
      final left = (widget.menuScreenLeft ?? fieldLeft).clamp(
        _margin,
        math.max(_margin, screen - _margin - _width),
      );
      _dx = left - fieldLeft;
    }
    // Uzun menü daha uzun sürede açılır; aynı süre kısa menüde ağır, uzun
    // menüde "bir anda" okunur.
    _motion
      ..duration = AppMenuMotion.openFor(math.min(desired, _maxHeight))
      ..reverseDuration = AppMenuMotion.closeFor(math.min(desired, _maxHeight));
    setState(() => _open = true);
    _portal.show();
    if (_still) {
      _motion.value = 1;
    } else {
      _motion.forward(from: 0);
    }
  }

  /// Panelin gövdesi: 16 + alan + boşluk + menü + 8 (kategori alanıyla aynı
  /// hesap; menünün altında eylem satırına 8 dp kalır).
  void _fitSheet(AppFormSheetFit fit, double menuHeight) {
    final box = _fieldKey.currentContext?.findRenderObject() as RenderBox?;
    final field = widget.anchorHeight ?? box?.size.height ?? 56;
    fit.height.value =
        AppSpacing.medium + field + _gap + menuHeight + AppSpacing.small;
  }

  /// Alanı kaydırılan gövdenin başına getirir; üstünde bir ölçek kalır.
  Future<void> _scrollFieldToTop() async {
    final box = _fieldKey.currentContext?.findRenderObject();
    final position = Scrollable.maybeOf(context)?.position;
    if (box is! RenderBox || position == null) return;
    final viewport = RenderAbstractViewport.maybeOf(box);
    if (viewport == null) return;
    final target =
        (viewport.getOffsetToReveal(box, 0).offset - AppSpacing.medium).clamp(
          position.minScrollExtent,
          position.maxScrollExtent,
        );
    if ((target - position.pixels).abs() < 0.5) return;
    if (_still) {
      position.jumpTo(target);
    } else {
      await position.animateTo(
        target,
        duration: const Duration(milliseconds: 150),
        curve: Curves.easeOutCubic,
      );
    }
  }

  /// Menü çizildikten sonra: ölçü o anki yere göre yenilenir (klavye
  /// kapanırken ya da panel boylanırken alınan ölçü eskir ve menü gereksiz
  /// kısa kalırdı), panelde ayrılan yer menünün gerçek boyuna çekilir.
  void _settle() {
    if (!mounted || !_open) return;
    final space = _space();
    if (space == null) return;
    final menu = _menuKey.currentContext?.findRenderObject();
    if (_fitted) {
      // Panelde menünün boyu panelden bilinir; yalnız ayrılan yer menünün
      // gerçek boyuna çekilir (içerik tahminden kısaysa).
      final fit = AppFormSheetFit.maybeOf(context);
      if (fit != null && menu is RenderBox && menu.hasSize) {
        final actual = menu.size.height;
        if (actual < _maxHeight - 0.5 &&
            (_reserve - _gap - actual).abs() > 0.5) {
          setState(() => _reserve = _gap + actual);
          _fitSheet(fit, actual);
        }
      }
      return;
    }
    final bounded = _boundedHeight(space);
    if ((bounded - _maxHeight).abs() > 0.5) {
      setState(() => _maxHeight = bounded);
    }
  }

  Future<void> _close() async {
    if (!_open) return;
    setState(() => _open = false);
    if (!_still) await _motion.reverse();
    if (!mounted || _open) return;
    _portal.hide();
    if (!_fitted) return;
    final saved = _savedOffset;
    _savedOffset = null;
    AppFormSheetFit.maybeOf(context)?.height.value = null;
    setState(() {
      _fitted = false;
      _reserve = 0;
    });
    final position = Scrollable.maybeOf(context)?.position;
    if (saved == null || position == null) return;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted || _open || !position.hasPixels) return;
      position.jumpTo(
        saved.clamp(position.minScrollExtent, position.maxScrollExtent),
      );
    });
  }

  @override
  Widget build(BuildContext context) => PopScope(
    canPop: !_open,
    onPopInvokedWithResult: (didPop, _) {
      if (!didPop) _close();
    },
    // Çapraz eksen `start`: alan bir satırın içindeyse (filtre çipi)
    // genişliği sınırsızdır ve `stretch` kullanılamaz; form alanı zaten
    // bulunduğu genişliği doldurur.
    child: Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        CompositedTransformTarget(
          link: _link,
          child: OverlayPortal(
            controller: _portal,
            overlayChildBuilder: _overlay,
            child: KeyedSubtree(
              key: _fieldKey,
              child: widget.builder(context, _open, _toggle),
            ),
          ),
        ),
        if (_reserve > 0) SizedBox(height: _reserve),
      ],
    ),
  );

  Widget _overlay(BuildContext context) {
    // Klavye ya da ekran değişince ölçü yenilenir.
    MediaQuery.viewInsetsOf(context);
    WidgetsBinding.instance.addPostFrameCallback((_) => _settle());
    final surfaces = AppSurfaces.of(context);
    return Stack(
      children: [
        Positioned.fill(
          child: GestureDetector(
            behavior: HitTestBehavior.translucent,
            onTap: _close,
          ),
        ),
        CompositedTransformFollower(
          link: _link,
          showWhenUnlinked: false,
          targetAnchor: _below ? Alignment.bottomLeft : Alignment.topLeft,
          followerAnchor: _below ? Alignment.topLeft : Alignment.bottomLeft,
          offset: Offset(_dx, _below ? _gap + _dy : -_gap),
          child: SizedBox(
            width: _width,
            child: AppMenuFade(
              animation: _motion,
              child: ConstrainedBox(
                constraints: BoxConstraints(maxHeight: _maxHeight),
                child: Material(
                  color: surfaces.card,
                  elevation: AppElevation.floating,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(AppRadius.field),
                    side: BorderSide(color: surfaces.border),
                  ),
                  clipBehavior: Clip.antiAlias,
                  // Kutu alanın kenarından uzar; köşeleri ve gölgesi
                  // hareket boyunca yerindedir.
                  child: AppMenuReveal(
                    animation: _motion,
                    fromBottom: !_below,
                    // Liste kutudan uzunsa kaydırma çubuğu hep görünür:
                    // devamı olduğu ilk bakışta okunur.
                    child: Scrollbar(
                      key: _menuKey,
                      controller: _scroll,
                      thumbVisibility: true,
                      child: SingleChildScrollView(
                        controller: _scroll,
                        child: widget.menuBuilder(context, _close),
                      ),
                    ),
                  ),
                ),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

/// Seçim alanının oku: menüyle birlikte döner (220 ms); "animasyonları
/// kapat" açıksa bir anda döner.
class AppMenuArrow extends StatelessWidget {
  const AppMenuArrow({required this.open, super.key});
  final bool open;

  @override
  Widget build(BuildContext context) => AnimatedRotation(
    turns: open ? 0.5 : 0,
    duration: MediaQuery.of(context).disableAnimations
        ? Duration.zero
        : AppMenuMotion.open,
    curve: Curves.easeOutCubic,
    child: const Icon(Icons.arrow_drop_down, size: 24),
  );
}

/// Seçim menülerinin ortak hareketi (kullanıcı, 15 Kasım 2026: menüler
/// "bir anda" ve yapay açılıyordu; uzun kategori menüsünde daha da belli
/// oluyordu). Yüzen menü ([AppAnimatedMenu]), akış içindeki kategori menüsü
/// ve filtre menüsü aynı hareketi kullanır:
///
/// - Kutu alanın kenarından **uzar** ([AppMenuReveal]); ölçeklenmez, köşeleri
///   ve gölgesi hareket boyunca yerindedir. Hızlı başlar, yavaşlayarak durur.
/// - İçerik kutuyla birlikte açılır ve hareketin ilk yarısında belirir.
/// - Süre menünün boyuna göre uzar ([openFor]): kısa menü çabuk, uzun menü
///   daha yavaş açılır; göz her ikisinde de hareketi izleyebilir.
abstract final class AppMenuMotion {
  /// Boyu bilinmeyen menünün açılışı.
  static const Duration open = Duration(milliseconds: 320);

  /// Boyu bilinmeyen menünün kapanışı.
  static const Duration close = Duration(milliseconds: 230);

  /// [height] dp boyundaki menünün açılış süresi: 240–420 ms.
  static Duration openFor(double height) =>
      Duration(milliseconds: (200 + height * 0.36).clamp(240, 420).round());

  /// Kapanış açılıştan kısadır ama izlenebilir: 190–300 ms.
  static Duration closeFor(double height) =>
      Duration(milliseconds: (160 + height * 0.24).clamp(190, 300).round());
}

/// Menü kutusunun **içine** konur: içerik alanın kenarından aşağı (ya da
/// [fromBottom] ise yukarı) doğru açılır, kutu onunla birlikte uzar.
/// [animation] 0–1 arasında ham denetleyicidir; eğriler burada uygulanır.
/// İçerik baştan tam boyuyla yerleşir; ölçmek isteyen içeriği ölçer.
class AppMenuReveal extends StatelessWidget {
  const AppMenuReveal({
    required this.animation,
    required this.child,
    super.key,
    this.fromBottom = false,
  });

  final Animation<double> animation;
  final Widget child;

  /// Menü alanın üstüne açılıyorsa içerik alt kenarından açılır.
  final bool fromBottom;

  /// Kutunun o anki boy oranı (0–1). Açılış yavaşlayarak durur
  /// (`easeOutCubic`), kapanış yumuşak başlar ve biter. Menüyle birlikte
  /// hareket eden her şey (ayrılan yer, sayfanın kayması) aynı oranı okur.
  static double sizeFactor(Animation<double> animation) =>
      animation.status == AnimationStatus.reverse
      ? Curves.easeInOutCubic.transform(animation.value)
      : Curves.easeOutCubic.transform(animation.value);

  @override
  Widget build(BuildContext context) => AnimatedBuilder(
    animation: animation,
    builder: (context, child) {
      final t = animation.value;
      final size = sizeFactor(animation);
      final opacity = Curves.easeOut.transform((t / 0.55).clamp(0.0, 1.0));
      return ClipRect(
        child: Align(
          alignment: fromBottom ? Alignment.bottomCenter : Alignment.topCenter,
          heightFactor: size,
          child: Opacity(opacity: opacity, child: child),
        ),
      );
    },
    child: child,
  );
}

/// Menü kutusunun dışına konur: kutu (zemin, kenar, gölge) hareketin en
/// başında belirir, en sonunda kaybolur; boş bir çizgi olarak görünmez.
class AppMenuFade extends StatelessWidget {
  const AppMenuFade({required this.animation, required this.child, super.key});
  final Animation<double> animation;
  final Widget child;

  @override
  Widget build(BuildContext context) => AnimatedBuilder(
    animation: animation,
    builder: (context, child) => Opacity(
      opacity: (animation.value / 0.18).clamp(0.0, 1.0),
      child: child,
    ),
    child: child,
  );
}
