import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import 'nomiva_brand.dart';

/// Oturum kontrol edilirken görünen ekran.
///
/// Telefonun kendi açılış ekranıyla aynı koyu renktir; uygulama açılırken
/// arada beyaz bir an ya da yazı belirmez. Oturum varsa doğrudan Özet, yoksa
/// giriş animasyonu bu zeminden başlar. Kontrol uzarsa (yavaş ağ) kısa bir
/// süre sonra küçük bir bekleme göstergesi çıkar.
class AuthRestoringView extends StatefulWidget {
  const AuthRestoringView({super.key});

  @override
  State<AuthRestoringView> createState() => _AuthRestoringViewState();
}

class _AuthRestoringViewState extends State<AuthRestoringView> {
  Timer? _timer;
  bool _slow = false;

  @override
  void initState() {
    super.initState();
    _timer = Timer(const Duration(milliseconds: 900), () {
      if (mounted) setState(() => _slow = true);
    });
  }

  bool _precached = false;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    // Oturum yoksa sıradaki ekran logoyla açılır; logo şimdiden hazırlansın.
    if (!_precached) {
      _precached = true;
      unawaited(precacheNomivaMark(context));
    }
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => AnnotatedRegion<SystemUiOverlayStyle>(
    value: SystemUiOverlayStyle.light.copyWith(
      statusBarColor: Colors.transparent,
    ),
    child: Scaffold(
      backgroundColor: NomivaColors.top,
      body: Semantics(
        label: 'Oturum kontrol ediliyor',
        liveRegion: true,
        child: Center(
          child: AnimatedOpacity(
            opacity: _slow ? 1 : 0,
            duration: const Duration(milliseconds: 300),
            child: const SizedBox.square(
              dimension: 22,
              child: CircularProgressIndicator(
                strokeWidth: 2.5,
                color: NomivaColors.tagline,
              ),
            ),
          ),
        ),
      ),
    ),
  );
}
