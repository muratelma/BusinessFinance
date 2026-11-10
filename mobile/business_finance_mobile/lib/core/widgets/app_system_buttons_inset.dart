import 'package:flutter/material.dart';

/// Üç düğmeli Android gezinmesinde uygulamayı düğmelerin üstünde bitirir.
///
/// Uygulama ekranın altına kadar çizilir. Hareket çizgisi olan telefonda bu
/// istenen şeydir: çizgi ince ve saydamdır, içerik arkasından akar. Üç
/// düğmeli gezinmede ise sistem çubuğu opaktır ve sayfanın son 48 dp'sini
/// örter; listenin sonundaki boşluk ve son satırın alt kenarı görünmez
/// kalıyordu (10 Kasım 2026, emülatörde görüldü).
///
/// Ayrım sistem alt boşluğunun boyundan yapılır: hareket çizgisi 16–34 dp,
/// düğmeler 48 dp bırakır. Boşluk [_buttonsFrom] ya da fazlaysa içerik o kadar
/// yukarıda biter ve altındaki sayfalar boşluğu sıfır görür; böylece her
/// sayfa ve alt çubuk kendi kuralını düğmelerin üstünden uygular.
class AppSystemButtonsInset extends StatelessWidget {
  const AppSystemButtonsInset({required this.child, super.key});

  final Widget child;

  static const double _buttonsFrom = 40;

  @override
  Widget build(BuildContext context) {
    final media = MediaQuery.of(context);
    final system = media.viewPadding.bottom;
    if (system < _buttonsFrom) return child;
    // Klavye açıkken sistem çubuğunun payı klavyenin payının içindedir;
    // ikisi birden eklenmez.
    final keyboard = media.viewInsets.bottom;
    final inset = keyboard > 0 ? 0.0 : system;
    return ColoredBox(
      color: Theme.of(context).scaffoldBackgroundColor,
      child: Padding(
        padding: EdgeInsets.only(bottom: inset),
        child: MediaQuery(
          data: media.copyWith(
            padding: media.padding.copyWith(bottom: 0),
            viewPadding: media.viewPadding.copyWith(bottom: 0),
          ),
          child: child,
        ),
      ),
    );
  }
}
