import 'dart:async';
import 'dart:math' as math;

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../core/network/api_exception.dart';
import 'auth_panels.dart';
import 'nomiva_brand.dart';

/// Giriş akışının panelleri. Sıra, yatay kaymanın yönünü belirler.
enum AuthView { login, register, reset, business }

typedef AuthLogin =
    Future<void> Function(
      String email,
      String password, {
      Future<void> Function()? onVerified,
    });

typedef AuthRegister =
    Future<void> Function(
      String email,
      String password, {
      required bool hasBusiness,
      String? username,
      Future<void> Function()? onVerified,
    });

/// Açılış, giriş, hesap oluşturma, parola sıfırlama ve "Bir adım kaldı" tek
/// ekranda (Nomiva tasarımı, 7 Kasım 2026).
///
/// Üstteki siyah bölüm ve logo yerinde kalır, yalnız form kayar: ayrı
/// sayfalar arasında gidip gelmek yerine aynı ekranın içinde yön duygusu
/// verir. Ölçüler 320 × 700 pt'lik tasarım ekranındandır ve [NomivaScale] ile
/// ekrana oranla büyür; iki telefonda da neyin nerede durduğu aynıdır.
///
/// Açılış animasyonu yalnız bu ekran **açılırken** oynar: ilk açılışta ve
/// çıkış yapıldıktan sonra. Oturumu açık olan kişi bu ekranı hiç görmez,
/// beklemez.
class AuthFlow extends StatefulWidget {
  const AuthFlow({
    required this.onLogin,
    required this.onRegister,
    required this.onRequestPasswordReset,
    required this.onResetPassword,
    super.key,
    this.onCheckUsername,
    this.initialView = AuthView.login,
    this.initialEmail,
  });

  final AuthLogin onLogin;
  final AuthRegister onRegister;
  final Future<void> Function(String email) onRequestPasswordReset;
  final Future<void> Function({
    required String email,
    required String code,
    required String newPassword,
  })
  onResetPassword;
  final AuthView initialView;

  /// Kayıt formunun "bu kullanıcı adı boş mu" sorusu.
  final Future<bool> Function(String username)? onCheckUsername;
  final String? initialEmail;

  @override
  State<AuthFlow> createState() => _AuthFlowState();
}

/// Bir panelin üst bölüm yerleşimi (tasarım birimiyle).
@immutable
class _Frame {
  const _Frame(this.sheetTop, this.brandY, this.brandScale, this.dip);

  final double sheetTop;
  final double brandY;
  final double brandScale;

  /// Kağıdın kavisli kenarının ortada ne kadar aşağı indiği.
  final double dip;

  static _Frame lerp(_Frame a, _Frame b, double t) => _Frame(
    _l(a.sheetTop, b.sheetTop, t),
    _l(a.brandY, b.brandY, t),
    _l(a.brandScale, b.brandScale, t),
    _l(a.dip, b.dip, t),
  );

  static double _l(double a, double b, double t) => a + (b - a) * t;
}

// Tasarım birimiyle; bütün dikey ölçüler tasarımdaki durum çubuğunu içerir.
const _standard = _Frame(158, 62, .62, 52);
const _register = _Frame(148, 58, .58, 52);
const _ease = Cubic(.2, .8, .2, 1);

_Frame _frameOf(AuthView view) =>
    view == AuthView.register ? _register : _standard;

// Açılış zaman çizelgesi (ms), tasarım notlarındaki sırayla.
//
// Slate örneğine göre (kullanıcı, 8 Kasım 2026): logo beklemeden gelir ve
// ~0,9 sn'de yerine oturur; ad, slogan ve dalga ~1,7 sn'de tamamlanır (ilk
// denemedeki 1,3 sn kullanıcıya hızlı geldi); tamamlanmış hâl ~1,3 sn sakin
// durur, sonra yüzey 1 sn'de yavaş başlayıp yavaş biterek yükselir. Önceki
// çizelgede logo 250 ms gecikiyordu, parçalar 2 sn'ye yayılıyordu, bütün
// hâl yalnız ~1,2 sn duruyordu ve yükseliş çok hızlı başlıyordu: açılış
// "hemen geçiyor" gibi görünüyordu.
const int _introMs = 4200;
const int _composed = 1700;
const int _riseStart = 3000;
const int _riseEnd = 4000;

/// Yükseliş eğrisi: yavaş başlar, yavaş biter. Kağıt bir anda fırlamaz.
const _rise = Cubic(.45, 0, .2, 1);

class _AuthFlowState extends State<AuthFlow> with TickerProviderStateMixin {
  late final AnimationController _intro;
  late final AnimationController _move;
  late final AnimationController _panels;
  late final AnimationController _reveal;

  late final TextEditingController _loginEmail;
  final _resetEmail = TextEditingController();

  late AuthView _view;
  AuthView? _leaving;
  bool _forward = true;
  late _Frame _moveFrom;
  late _Frame _moveTo;
  Offset? _revealOrigin;

  String? _pendingUsername;
  String? _pendingEmail;
  String? _pendingPassword;
  String? _registerError;

  @override
  void initState() {
    super.initState();
    _view = widget.initialView;
    _moveFrom = _moveTo = _frameOf(_view);
    _loginEmail = TextEditingController(text: widget.initialEmail);
    _intro = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: _introMs),
    )..addListener(_rebuild);
    _move = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 420),
    )..addListener(_rebuild);
    _panels = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 360),
      value: 1,
    )..addListener(_rebuild);
    _reveal = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 560),
    )..addListener(_rebuild);
  }

  bool _started = false;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    if (_started) return;
    _started = true;
    // Hareketi azaltmak isteyen kullanıcı açılışı izlemez; form hemen gelir.
    if (MediaQuery.disableAnimationsOf(context)) {
      _intro.value = 1;
      for (final c in [_move, _panels, _reveal]) {
        c.duration = Duration.zero;
      }
    } else {
      unawaited(_startIntro());
    }
  }

  /// Açılış, logo çözülüp ilk kare çizildikten sonra başlar. Uygulama
  /// açılırken ilk kareler pahalıdır; animasyon zamana bağlı olduğu için o
  /// sırada başlasaydı kareler atlanır, logo soluk takılır, açılış "hemen
  /// geçmiş" gibi görünürdü (emülatörde ölçüldü, 8 Kasım 2026).
  Future<void> _startIntro() async {
    try {
      await precacheNomivaMark(context);
    } on Object {
      // Logo çözülemezse açılış yine oynar.
    }
    await WidgetsBinding.instance.endOfFrame;
    await WidgetsBinding.instance.endOfFrame;
    if (!mounted || _intro.value > 0) return;
    unawaited(_intro.forward());
  }

  void _rebuild() => setState(() {});

  @override
  void dispose() {
    _intro.dispose();
    _move.dispose();
    _panels.dispose();
    _reveal.dispose();
    _loginEmail.dispose();
    _resetEmail.dispose();
    super.dispose();
  }

  bool get _introRunning => _intro.value < 1;

  /// Açılış sürerken ekrana dokunmak onu bitirir.
  void _skipIntro() {
    if (_introRunning) _intro.value = 1;
  }

  double _interval(int startMs, int endMs, {Curve curve = _ease}) {
    final t = (_intro.value * _introMs - startMs) / (endMs - startMs);
    return curve.transform(t.clamp(0.0, 1.0));
  }

  /// [screenHeight] tasarım birimindedir.
  _Frame _currentFrame(double screenHeight) {
    if (_introRunning) {
      final splash = _Frame(
        screenHeight,
        NomivaSpacing.splashBrandY,
        1,
        NomivaSpacing.splashDip,
      );
      return _Frame.lerp(
        splash,
        _frameOf(_view),
        _interval(_riseStart, _riseEnd, curve: _rise),
      );
    }
    return _Frame.lerp(_moveFrom, _moveTo, _ease.transform(_move.value));
  }

  void _go(AuthView to) {
    if (to == _view) return;
    _skipIntro();
    FocusScope.of(context).unfocus();
    final from = _view;
    final target = _frameOf(to);
    setState(() {
      _forward = to.index > from.index;
      _leaving = from;
      _view = to;
      // Sunucunun kayıt reddi kayıt denemesine aittir; kayıttan girişe
      // dönen kişi onu bir daha görmez. "Bir adım kaldı"dan kayda dönüşte
      // ise tam o uyarı gösterilir (_registerSubmit kurar).
      if (from == AuthView.register && to != AuthView.business) {
        _registerError = null;
      }
      if (_frameOf(from) != target) {
        _moveFrom = _Frame.lerp(
          _moveFrom,
          _moveTo,
          _ease.transform(_move.value),
        );
        _moveTo = target;
        _move.forward(from: 0);
      }
    });
    _panels.forward(from: 0).whenCompleteOrCancel(() {
      if (mounted) setState(() => _leaving = null);
    });
  }

  void _back() => _go(switch (_view) {
    AuthView.business => AuthView.register,
    _ => AuthView.login,
  });

  /// Başarılı girişte Özet'in zemin rengi basılan düğmeden yayılır; daire
  /// ekranı kaplayınca oturum açılır ve Özet aynı renkte belirir.
  Future<void> _playReveal(Offset? origin) async {
    setState(() => _revealOrigin = origin);
    await _reveal.forward(from: 0).orCancel.catchError((_) {});
  }

  Future<void> _login(String email, String password, Offset? origin) =>
      widget.onLogin(email, password, onVerified: () => _playReveal(origin));

  void _registerNext(String username, String email, String password) {
    _pendingUsername = username;
    _pendingEmail = email;
    _pendingPassword = password;
    setState(() => _registerError = null);
    _go(AuthView.business);
  }

  Future<void> _registerSubmit({
    required bool hasBusiness,
    Offset? origin,
  }) async {
    final email = _pendingEmail;
    final password = _pendingPassword;
    if (email == null || password == null) {
      _go(AuthView.register);
      return;
    }
    try {
      await widget.onRegister(
        email,
        password,
        hasBusiness: hasBusiness,
        username: _pendingUsername,
        onVerified: () => _playReveal(origin),
      );
    } on ApiException catch (error) {
      // Reddin sebebi neredeyse her zaman e-posta ya da parola; kişi onları
      // düzeltebileceği yere döner.
      if (!mounted) return;
      setState(() => _registerError = error.message);
      _go(AuthView.register);
    }
  }

  void _resetCompleted(String email) {
    _loginEmail.text = email;
    _go(AuthView.login);
  }

  @override
  Widget build(BuildContext context) {
    final base = Theme.of(context);
    final colors = NomivaColors.of(context);
    // Klavye açılınca ekranın boyu değişmez: katsayı tam ekrandan okunur.
    final n = NomivaScale.forSize(MediaQuery.sizeOf(context));
    final safeTop = MediaQuery.paddingOf(context).top;
    // Durum çubuğu tasarımdakinden uzunsa her şey o kadar aşağı iner.
    final dy = math.max(0.0, safeTop - n(NomivaSpacing.statusBar));

    return Theme(
      data: nomivaAuthTheme(base),
      child: NomivaScaleScope(
        scale: n,
        child: AnnotatedRegion<SystemUiOverlayStyle>(
          value: SystemUiOverlayStyle.light.copyWith(
            statusBarColor: Colors.transparent,
          ),
          child: PopScope(
            canPop: _view == AuthView.login,
            onPopInvokedWithResult: (didPop, _) {
              if (!didPop) _back();
            },
            child: Scaffold(
              backgroundColor: NomivaColors.top,
              body: LayoutBuilder(
                builder: (context, box) {
                  final frame = _currentFrame(box.maxHeight / n.factor);
                  final sheetTop = n(frame.sheetTop) >= box.maxHeight
                      ? box.maxHeight
                      : n(frame.sheetTop) + dy;
                  return Stack(
                    children: [
                      ..._splashLayers(box, dy, n),
                      Positioned(
                        left: 0,
                        right: 0,
                        top: dy + n(frame.brandY),
                        child: _Brand(
                          scale: frame.brandScale * n.factor,
                          logo: _interval(0, 900),
                          letter: (i) => _interval(550 + 75 * i, 1000 + 75 * i),
                          tagline:
                              _interval(1250, _composed, curve: Curves.linear) -
                              _interval(
                                _riseStart,
                                _riseStart + 300,
                                curve: Curves.linear,
                              ),
                        ),
                      ),
                      Positioned(
                        left: 0,
                        right: 0,
                        top: sheetTop,
                        bottom: 0,
                        child: CustomPaint(
                          painter: _SheetPainter(
                            dip: n(frame.dip),
                            edgeHeight: n(NomivaSpacing.edgeHeight),
                            color: colors.paper,
                          ),
                          child: Padding(
                            padding: EdgeInsets.only(
                              top: n(NomivaSpacing.paperInset),
                            ),
                            child: _panelStack(n),
                          ),
                        ),
                      ),
                      if (_introRunning)
                        Positioned.fill(
                          child: GestureDetector(
                            behavior: HitTestBehavior.opaque,
                            onTap: _skipIntro,
                            child: Semantics(
                              button: true,
                              label: 'Açılışı geç',
                              child: const SizedBox.expand(),
                            ),
                          ),
                        ),
                      if (_reveal.value > 0)
                        Positioned.fill(
                          child: IgnorePointer(
                            child: CustomPaint(
                              painter: _RevealPainter(
                                progress: const Cubic(
                                  .5,
                                  0,
                                  .2,
                                  1,
                                ).transform(_reveal.value),
                                origin: _revealOrigin,
                                color: base.scaffoldBackgroundColor,
                              ),
                            ),
                          ),
                        ),
                    ],
                  );
                },
              ),
            ),
          ),
        ),
      ),
    );
  }

  List<Widget> _splashLayers(BoxConstraints box, double dy, NomivaScale n) {
    if (!_introRunning) return const [];
    final glow =
        _interval(350, _composed) -
        _interval(_riseStart, _riseStart + 450, curve: Curves.linear);
    final waves = _interval(450, _composed);
    return [
      Positioned(
        top: dy + n(NomivaSpacing.glowTop),
        left: (box.maxWidth - n(NomivaSpacing.glowSize)) / 2,
        width: n(NomivaSpacing.glowSize),
        height: n(NomivaSpacing.glowSize),
        child: IgnorePointer(
          child: Opacity(
            opacity: glow.clamp(0.0, 1.0),
            child: Transform.scale(
              scale: .7 + .3 * _interval(350, _composed),
              child: const DecoratedBox(
                decoration: BoxDecoration(
                  gradient: RadialGradient(
                    // CSS'teki "farthest-corner" dairesi: yarıçap köşeye
                    // kadardır, kenara kadar değil.
                    radius: .707,
                    colors: [
                      Color(0x616A6CF6),
                      Color(0x1F6A6CF6),
                      Color(0x006A6CF6),
                    ],
                    stops: [0, .42, .70],
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
      Positioned(
        left: 0,
        right: 0,
        bottom: 0,
        height: n(NomivaSpacing.wavesHeight),
        child: IgnorePointer(
          child: Opacity(
            opacity: waves,
            child: Transform.translate(
              offset: Offset(0, n(40) * (1 - waves)),
              child: const CustomPaint(painter: _WavesPainter()),
            ),
          ),
        ),
      ),
    ];
  }

  Widget _panelStack(NomivaScale n) {
    final entrance = _interval(_riseEnd - 350, _introMs);
    final slide = _ease.transform(_panels.value);
    Widget slot(AuthView view, Widget child) {
      final current = view == _view;
      final leaving = view == _leaving;
      final visible = current || leaving;
      final double dx;
      final double opacity;
      if (current) {
        dx = (_forward ? 1.0 : -0.3) * (1 - slide);
        opacity = slide;
      } else if (leaving) {
        dx = (_forward ? -0.3 : 1.0) * slide;
        opacity = 1 - slide;
      } else {
        dx = 0;
        opacity = 0;
      }
      // Ağaç her durumda aynı biçimdedir: panel görünmezken de yaşar ve
      // yazılanı kaybetmez (kayıttan "Bir adım kaldı"ya gidip dönmek gibi).
      return Offstage(
        offstage: !visible,
        child: TickerMode(
          enabled: visible,
          child: IgnorePointer(
            ignoring: !current,
            child: ExcludeSemantics(
              excluding: !current,
              child: FractionalTranslation(
                translation: Offset(dx, 0),
                child: Opacity(opacity: opacity, child: child),
              ),
            ),
          ),
        ),
      );
    }

    return ClipRect(
      child: Opacity(
        opacity: entrance,
        child: Transform.translate(
          offset: Offset(0, n(8) * (1 - entrance)),
          child: Stack(
            fit: StackFit.expand,
            children: [
              slot(
                AuthView.login,
                LoginPanel(
                  active: _view == AuthView.login,
                  emailController: _loginEmail,
                  onSubmit: _login,
                  onForgotPassword: () {
                    _resetEmail.text = _loginEmail.text.trim();
                    _go(AuthView.reset);
                  },
                  onRegister: () => _go(AuthView.register),
                ),
              ),
              slot(
                AuthView.register,
                RegisterPanel(
                  active: _view == AuthView.register,
                  onCheckUsername: widget.onCheckUsername,
                  error: _registerError,
                  onNext: _registerNext,
                  onLogin: () => _go(AuthView.login),
                ),
              ),
              slot(
                AuthView.reset,
                PasswordResetPanel(
                  active: _view == AuthView.reset,
                  emailController: _resetEmail,
                  onRequestCode: widget.onRequestPasswordReset,
                  onReset: widget.onResetPassword,
                  onCompleted: _resetCompleted,
                  onBackToLogin: () => _go(AuthView.login),
                ),
              ),
              slot(
                AuthView.business,
                BusinessStepPanel(onSubmit: _registerSubmit),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Logo, ad ve slogan. Yazı ölçeğinden etkilenmez: bu bir logodur, metin
/// değil; büyüse üst bölüme sığmazdı.
class _Brand extends StatelessWidget {
  const _Brand({
    required this.scale,
    required this.logo,
    required this.letter,
    required this.tagline,
  });

  final double scale;
  final double logo;
  final double Function(int index) letter;
  final double tagline;

  static const _name = 'Nomiva';

  @override
  Widget build(BuildContext context) => MediaQuery.withNoTextScaling(
    child: Transform.scale(
      scale: scale,
      alignment: Alignment.topCenter,
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Opacity(
            opacity: logo,
            child: Transform.translate(
              offset: Offset(0, 6 * (1 - logo)),
              child: Transform.scale(
                scale: .85 + .15 * logo,
                child: const NomivaMark(width: NomivaSpacing.markWidth),
              ),
            ),
          ),
          const SizedBox(height: NomivaSpacing.brandGap),
          ExcludeSemantics(
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                for (var i = 0; i < _name.length; i++)
                  Opacity(
                    opacity: letter(i),
                    child: Transform.translate(
                      offset: Offset(0, 10 * (1 - letter(i))),
                      child: Text(
                        _name[i],
                        style: const TextStyle(
                          fontFamily: nomivaFont,
                          fontSize: 30,
                          fontWeight: FontWeight.w700,
                          letterSpacing: -.6,
                          height: 1.2,
                          color: NomivaColors.brandName,
                        ),
                      ),
                    ),
                  ),
              ],
            ),
          ),
          const SizedBox(height: NomivaSpacing.brandGap),
          Opacity(
            opacity: tagline.clamp(0.0, 1.0),
            child: const Text(
              'İşin ve cebin tek yerde.',
              style: TextStyle(
                fontFamily: nomivaFont,
                height: nomivaLineHeight,
                letterSpacing: nomivaLetterSpacing,
                fontSize: 13,
                color: NomivaColors.tagline,
              ),
            ),
          ),
        ],
      ),
    ),
  );
}

/// Form kağıdı: üst kenarı kavislidir, yanlarda tepede, ortada aşağıda.
class _SheetPainter extends CustomPainter {
  const _SheetPainter({
    required this.dip,
    required this.edgeHeight,
    required this.color,
  });

  final double dip;
  final double edgeHeight;
  final Color color;

  Path _path(Size size) {
    final w = size.width;
    final path = Path()..moveTo(0, 0);
    // Kübik eğri, 320 genişlikte kontrol noktaları x = 90 ve x = 230.
    const steps = 48;
    for (var i = 1; i <= steps; i++) {
      final t = i / steps;
      final u = 1 - t;
      final x =
          3 * u * u * t * (w * 90 / 320) +
          3 * u * t * t * (w * 230 / 320) +
          t * t * t * w;
      final y = 3 * u * u * t * dip + 3 * u * t * t * dip;
      path.lineTo(x, math.min(y, edgeHeight));
    }
    return path
      ..lineTo(w, size.height)
      ..lineTo(0, size.height)
      ..close();
  }

  @override
  void paint(Canvas canvas, Size size) {
    final path = _path(size);
    canvas.drawPath(
      path.shift(const Offset(0, -6)),
      Paint()
        ..color = const Color(0x14141828)
        ..maskFilter = const MaskFilter.blur(BlurStyle.normal, 7),
    );
    canvas.drawPath(path, Paint()..color = color);
  }

  @override
  bool shouldRepaint(_SheetPainter old) =>
      old.dip != dip || old.edgeHeight != edgeHeight || old.color != color;
}

/// Açılıştaki iki katmanlı dalga (vurgu ve vurgu 2 degradesi).
class _WavesPainter extends CustomPainter {
  const _WavesPainter();

  @override
  void paint(Canvas canvas, Size size) {
    final sx = size.width / 320;
    final sy = size.height / 200;
    Path wave(List<double> p) {
      final path = Path()
        ..moveTo(0, p[0] * sy)
        ..cubicTo(
          p[1] * sx,
          p[2] * sy,
          p[3] * sx,
          p[4] * sy,
          p[5] * sx,
          p[6] * sy,
        )
        ..cubicTo(
          p[7] * sx,
          p[8] * sy,
          p[9] * sx,
          p[10] * sy,
          size.width,
          p[11] * sy,
        )
        ..lineTo(size.width, size.height)
        ..lineTo(0, size.height)
        ..close();
      return path;
    }

    final back = wave([120, 70, 70, 150, 150, 220, 110, 260, 88, 295, 70, 76]);
    final front = wave([
      150,
      60,
      120,
      120,
      175,
      200,
      150,
      250,
      134,
      290,
      112,
      118,
    ]);
    canvas.drawPath(
      back,
      Paint()
        ..shader = LinearGradient(
          begin: Alignment.topRight,
          end: Alignment.bottomLeft,
          colors: [
            NomivaColors.accent2.withValues(alpha: .55),
            NomivaColors.accent.withValues(alpha: .35),
          ],
        ).createShader(back.getBounds()),
    );
    canvas.drawPath(
      front,
      Paint()
        ..shader = LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            NomivaColors.accent.withValues(alpha: .9),
            NomivaColors.accent2.withValues(alpha: .95),
          ],
        ).createShader(front.getBounds()),
    );
  }

  @override
  bool shouldRepaint(_WavesPainter old) => false;
}

class _RevealPainter extends CustomPainter {
  const _RevealPainter({
    required this.progress,
    required this.origin,
    required this.color,
  });

  final double progress;
  final Offset? origin;
  final Color color;

  @override
  void paint(Canvas canvas, Size size) {
    final o = origin ?? size.center(Offset.zero);
    final far = [
      Offset.zero,
      Offset(size.width, 0),
      Offset(0, size.height),
      Offset(size.width, size.height),
    ].map((corner) => (corner - o).distance).reduce(math.max);
    final radius = 24 + (far - 24) * progress;
    canvas.drawCircle(o, radius, Paint()..color = color);
  }

  @override
  bool shouldRepaint(_RevealPainter old) =>
      old.progress != progress || old.origin != origin || old.color != color;
}
