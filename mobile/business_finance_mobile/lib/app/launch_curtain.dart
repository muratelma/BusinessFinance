import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../core/presentation/launch_signals.dart';
import '../features/auth/presentation/auth_controller.dart';
import '../features/auth/presentation/nomiva_brand.dart';

/// Uygulama açılırken telefonun kendi açılış ekranını devralan perde.
///
/// Telefon (Android 12+ sistem ekranı, eski Android'de pencere zemini,
/// iPhone'da açılış ekranı) koyu zeminin ortasında 108 dp genişliğinde
/// Nomiva işaretini çizer. Flutter'ın ilk karesi bu perdedir ve işareti
/// **aynı yerde, aynı boyda** çizer; telefonun ekranından uygulamaya geçişte
/// dikiş görünmez. Ardından (kullanıcı kararı, 10 Kasım 2026):
///
/// - **Oturum açıksa** perde Özet'in ilk verisi gelene kadar (en çok
///   [contentWait]) durur, sonra kısa bir geçişle kalkar: işaret küçülerek
///   silinir, koyu zemin Özet'in üstünden çekilir. Kişi yapay olarak
///   bekletilmez; veri hızlı gelirse işaret neredeyse hiç durmaz.
/// - **Oturum kapalıysa** giriş ekranı işareti aynı yerden devralır
///   ([LaunchHandoff]) ve uzun açılışı oynatır; perde altındaki ilk kare
///   çizilince silinir.
///
/// Perde yalnız uygulama açılırken vardır; çıkış yapmak ya da arka plandan
/// dönmek onu yeniden göstermez.
class LaunchCurtain extends StatefulWidget {
  const LaunchCurtain({
    required this.authController,
    required this.child,
    super.key,
    this.contentWait = const Duration(seconds: 6),
    this.minimumHold = const Duration(milliseconds: 1000),
  });

  final AuthController authController;
  final Widget child;

  /// Oturum açıldıktan sonra Özet'in verisi en çok bu kadar beklenir
  /// (kullanıcı kararı, 10 Kasım 2026: Özet hazır olana kadar sabit işarette
  /// kalınır; Özet "yükleniyor" ekranıyla açılmaz). 1,2 sn'yi aşan beklemede
  /// işaretin altında küçük bir gösterge çıkar; süre dolarsa (çok yavaş ağ)
  /// Özet kendi yükleme hâliyle açılır.
  final Duration contentWait;

  /// Sabit işaret en az bu kadar görünür (kullanıcı kararı, 10 Kasım 2026):
  /// hızlı telefonda işaret hemen kaybolup animasyon başlasaydı göz onu
  /// seçemezdi. Yavaş telefonda açılış zaten bundan uzun sürer; süre
  /// uzatılmaz, yalnız alt sınırdır. Sayaç perde kurulunca başlar; telefonun
  /// kendi açılış ekranı ondan önce de görünür.
  final Duration minimumHold;

  /// İşaretin telefonun açılış ekranındaki genişliği (dp). Android
  /// `splash_icon` / `splash_mark` ve iPhone `LaunchImage` bu boydadır.
  static const double markWidth = 108;

  /// Perde sırasında çizilen işaret: ilk karede hazır olsun diye PNG'dir
  /// (SVG ilk karede çizilmeyebiliyor).
  static const markAsset = 'assets/brand/nomiva_isaret_acilis.png';

  @override
  State<LaunchCurtain> createState() => _LaunchCurtainState();
}

enum _Phase { covering, lifting, handingOff, done, entering }

class _LaunchCurtainState extends State<LaunchCurtain>
    with TickerProviderStateMixin {
  late final AnimationController _lift = AnimationController(
    vsync: this,
    duration: const Duration(milliseconds: 700),
  )..addListener(() => setState(() {}));

  /// Girişten sonraki örtünün solması.
  late final AnimationController _enter = AnimationController(
    vsync: this,
    duration: const Duration(milliseconds: 480),
  )..addListener(() => setState(() {}));

  AuthStatus? _lastStatus;

  _Phase _phase = _Phase.covering;
  bool _firstFrameDeferred = false;
  bool _slow = false;
  Timer? _slowTimer;
  Timer? _contentTimer;
  final _held = Stopwatch()..start();

  /// [LaunchCurtain.minimumHold] dolana kadar bekler.
  Future<void> _holdMinimum() async {
    final left = widget.minimumHold - _held.elapsed;
    if (left > Duration.zero) await Future<void>.delayed(left);
  }

  @override
  void initState() {
    super.initState();
    LaunchHandoff.begin();
    // İşaret çözülene kadar ilk kare gönderilmez: telefonun açılış ekranı
    // ekranda kalır, arada boş bir kare görünmez.
    WidgetsBinding.instance.deferFirstFrame();
    _firstFrameDeferred = true;
    widget.authController.addListener(_onAuth);
    LaunchHandoff.contentReady.addListener(_onContentReady);
    // Oturum kontrolü uzarsa (yavaş ağ) işaretin altında küçük bir bekleme
    // göstergesi belirir.
    _slowTimer = Timer(const Duration(milliseconds: 1200), () {
      if (mounted && _phase == _Phase.covering) setState(() => _slow = true);
    });
  }

  bool _precacheStarted = false;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    if (_precacheStarted) return;
    _precacheStarted = true;
    unawaited(_prepare());
  }

  Future<void> _prepare() async {
    try {
      // İlk kare için yalnız perdenin PNG işareti gerekir. Giriş ekranının
      // SVG işaretini o ekran kendisi hazırlar; beklenseydi telefonun açılış
      // ekranı her açılışta ~0,7 sn uzardı (emülatörde ölçüldü).
      unawaited(precacheNomivaMark(context).catchError((Object _) {}));
      await precacheImage(
        const AssetImage(LaunchCurtain.markAsset),
        context,
      ).timeout(const Duration(seconds: 2));
    } on Object {
      // Çözülemezse açılış yine sürer; ilk kare sonsuza dek bekletilmez.
    }
    _allowFirstFrame();
    if (mounted) _onAuth();
  }

  void _allowFirstFrame() {
    if (!_firstFrameDeferred) return;
    _firstFrameDeferred = false;
    WidgetsBinding.instance.allowFirstFrame();
  }

  void _onAuth() {
    if (!mounted) return;
    final status = widget.authController.status;
    final previous = _lastStatus;
    _lastStatus = status;
    // Çıkıştan sonra eski Özet'in "hazırım"ı sonraki girişe taşınmasın.
    if (status == AuthStatus.unauthenticated) {
      LaunchHandoff.contentReady.value = false;
    }
    if (_phase == _Phase.done &&
        previous == AuthStatus.unauthenticated &&
        status == AuthStatus.authenticated) {
      unawaited(_coverEntry());
      return;
    }
    if (_phase != _Phase.covering || _firstFrameDeferred) return;
    switch (status) {
      case AuthStatus.restoring:
        return;
      case AuthStatus.unauthenticated:
        unawaited(_handOff());
      case AuthStatus.authenticated:
        if (LaunchHandoff.contentReady.value) {
          _startLift();
        } else {
          _contentTimer ??= Timer(widget.contentWait, _startLift);
        }
    }
  }

  void _onContentReady() {
    if (LaunchHandoff.contentReady.value &&
        widget.authController.status == AuthStatus.authenticated) {
      _startLift();
    }
  }

  /// Girişten sonra: giriş ekranının dairesi ekranı Özet'in zemin rengiyle
  /// kaplamıştı. Aynı renk Özet'in ilk verisi gelene kadar (en çok
  /// [LaunchCurtain.contentWait]) durur, sonra Özet tek bir solmayla belirir.
  /// Aradaki boş sayfa ve "yükleniyor" ekranı görünmez.
  Future<void> _coverEntry() async {
    if (MediaQuery.disableAnimationsOf(context)) return;
    _enter.value = 0;
    setState(() => _phase = _Phase.entering);
    if (!LaunchHandoff.contentReady.value) {
      final ready = Completer<void>();
      void listener() {
        if (LaunchHandoff.contentReady.value && !ready.isCompleted) {
          ready.complete();
        }
      }

      LaunchHandoff.contentReady.addListener(listener);
      await ready.future
          .timeout(widget.contentWait, onTimeout: () {})
          .whenComplete(
            () => LaunchHandoff.contentReady.removeListener(listener),
          );
    }
    // Özet'in dolu karesi örtünün altında çizilsin.
    await WidgetsBinding.instance.endOfFrame;
    if (!mounted || _phase != _Phase.entering) return;
    await _enter.forward().orCancel.catchError((_) {});
    if (mounted) setState(() => _phase = _Phase.done);
  }

  /// Giriş ekranı işareti aynı yerde çizer; birkaç kare sonra perde
  /// kısa bir solmayla çekilir (SVG'nin ilk karesi geç gelirse bu solma
  /// onu örter).
  Future<void> _handOff() async {
    setState(() => _phase = _Phase.handingOff);
    await _holdMinimum();
    await WidgetsBinding.instance.endOfFrame;
    await WidgetsBinding.instance.endOfFrame;
    await WidgetsBinding.instance.endOfFrame;
    if (!mounted) return;
    _lift.duration = const Duration(milliseconds: 180);
    await _lift.forward().orCancel.catchError((_) {});
    // Giriş ekranı işareti yerinden ancak perde tamamen çekilince
    // kaldırır; yoksa solan perdedeki işaretle kayan işaret bir an çift
    // görünürdü.
    LaunchHandoff.release();
    if (mounted) setState(() => _phase = _Phase.done);
  }

  Future<void> _startLift() async {
    if (!mounted || _phase != _Phase.covering) return;
    _contentTimer?.cancel();
    LaunchHandoff.cancelHandoff();
    setState(() => _phase = _Phase.lifting);
    if (MediaQuery.disableAnimationsOf(context)) {
      setState(() => _phase = _Phase.done);
      return;
    }
    // Özet'in ilk karesi perdenin altında çizilsin.
    await WidgetsBinding.instance.endOfFrame;
    await _holdMinimum();
    if (!mounted) return;
    await _lift.forward().orCancel.catchError((_) {});
    if (mounted) setState(() => _phase = _Phase.done);
  }

  @override
  void dispose() {
    LaunchHandoff.release();
    _allowFirstFrame();
    _slowTimer?.cancel();
    _contentTimer?.cancel();
    widget.authController.removeListener(_onAuth);
    LaunchHandoff.contentReady.removeListener(_onContentReady);
    _lift.dispose();
    _enter.dispose();
    super.dispose();
  }

  double _between(double start, double end, Curve curve) {
    final t = ((_lift.value - start) / (end - start)).clamp(0.0, 1.0);
    return curve.transform(t);
  }

  @override
  Widget build(BuildContext context) {
    // Ağaç her hâlde aynı biçimdedir (Stack, ilk çocuk sayfa): örtü yalnız
    // eklenir ve çıkarılır. Biçim değişseydi perde kalkarken bütün uygulama
    // baştan kurulurdu.
    return Stack(
      textDirection: TextDirection.ltr,
      fit: StackFit.expand,
      children: [
        // Özet perdenin altından hafifçe yaklaşarak yerine oturur: koyu
        // zeminden açık sayfaya "pat" diye geçilmez. Ölçek her hâlde vardır
        // (1), ağacın biçimi değişmesin diye.
        Transform.translate(
          // Girişten sonra Özet hafifçe yukarı kayarak yerine oturur;
          // düz renkten "tık" diye geçilmez.
          offset: Offset(
            0,
            _phase == _Phase.entering
                ? 16 * (1 - Curves.easeOutCubic.transform(_enter.value))
                : 0,
          ),
          child: Transform.scale(
            scale: _phase == _Phase.lifting
                ? .96 + .04 * _between(.15, 1, Curves.easeOutCubic)
                : 1,
            child: widget.child,
          ),
        ),
        if (_phase == _Phase.entering)
          AbsorbPointer(
            child: Opacity(
              opacity: (1 - Curves.easeInOutCubic.transform(_enter.value))
                  .clamp(0.0, 1.0),
              child: ColoredBox(
                color: Theme.of(context).scaffoldBackgroundColor,
              ),
            ),
          ),
        if (_phase != _Phase.done && _phase != _Phase.entering)
          _curtain(context),
      ],
    );
  }

  Widget _curtain(BuildContext context) {
    final double markOpacity;
    final double markScale;
    final double groundOpacity;
    var layerOpacity = 1.0;
    if (_phase == _Phase.handingOff) {
      // Giriş ekranı işareti zaten aynı yerde çiziyor. Perde tek katman
      // olarak solar: zemin ve işaret ayrı ayrı solsaydı iki yarı saydam
      // katman üst üste biner, işaret bir an kararırdı.
      markOpacity = 1;
      markScale = 1;
      groundOpacity = 1;
      layerOpacity = 1 - _lift.value;
    } else {
      // İşaret hafifçe büyüyerek silinir; zemin hemen ardından yumuşakça
      // çekilir ve altındaki Özet yaklaşarak oturur. Üçü üst üste biner:
      // arada boş koyu bir an kalmaz.
      final mark = _between(0, .45, Curves.easeInCubic);
      markOpacity = 1 - mark;
      markScale = 1 + .1 * mark;
      groundOpacity = 1 - _between(.15, 1, Curves.easeInOutCubic);
    }

    return AnnotatedRegion<SystemUiOverlayStyle>(
      value: SystemUiOverlayStyle.light.copyWith(
        statusBarColor: Colors.transparent,
      ),
      child: AbsorbPointer(
        absorbing: _phase == _Phase.covering,
        child: Semantics(
          label: 'Nomiva açılıyor',
          liveRegion: true,
          child: Opacity(
            opacity: layerOpacity.clamp(0.0, 1.0),
            child: Stack(
              fit: StackFit.expand,
              children: [
                Opacity(
                  opacity: groundOpacity.clamp(0.0, 1.0),
                  child: const ColoredBox(color: NomivaColors.top),
                ),
                Center(
                  child: Opacity(
                    opacity: markOpacity.clamp(0.0, 1.0),
                    child: Transform.scale(
                      scale: markScale,
                      child: Image.asset(
                        LaunchCurtain.markAsset,
                        width: LaunchCurtain.markWidth,
                        gaplessPlayback: true,
                        excludeFromSemantics: true,
                      ),
                    ),
                  ),
                ),
                // Bekleme göstergesi işaretin altında, onu itmeden durur.
                // Yalnız gerektiğinde ağaçtadır: görünmezken de dönen bir
                // gösterge her karede boşuna çizim isterdi.
                if (_slow && _phase == _Phase.covering)
                  const Align(
                    alignment: Alignment(0, .32),
                    child: SizedBox.square(
                      dimension: 22,
                      child: CircularProgressIndicator(
                        strokeWidth: 2.5,
                        color: NomivaColors.tagline,
                      ),
                    ),
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
