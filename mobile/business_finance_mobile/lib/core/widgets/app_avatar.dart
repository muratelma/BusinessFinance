import 'package:flutter/material.dart';

/// Hesabın avatarı: marka zemininde e-postanın baş harfleri.
///
/// Uygulamada profil fotoğrafı yok; baş harf okunamıyorsa kişi ikonu çizilir.
/// [badge] doğrulanmamış e-posta gibi kalıcı bir uyarıyı sağ üstte kırmızı
/// noktayla gösterir; noktanın anlamı çağıranın semantik etiketinde yazar.
class AppAvatar extends StatelessWidget {
  const AppAvatar({
    required this.initials,
    super.key,
    this.size = 32,
    this.badge = false,
  });

  final String initials;
  final double size;
  final bool badge;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final circle = Container(
      width: size,
      height: size,
      alignment: Alignment.center,
      decoration: BoxDecoration(color: scheme.primary, shape: BoxShape.circle),
      child: initials.isEmpty
          ? Icon(
              Icons.person_outline,
              size: size * 0.56,
              color: scheme.onPrimary,
            )
          : Text(
              initials,
              maxLines: 1,
              textScaler: TextScaler.noScaling,
              style: TextStyle(
                fontSize: size * 0.4,
                height: 1,
                fontWeight: FontWeight.w600,
                letterSpacing: 0.3,
                color: scheme.onPrimary,
              ),
            ),
    );
    if (!badge) return ExcludeSemantics(child: circle);
    final dot = size * 0.44;
    return ExcludeSemantics(
      child: SizedBox(
        width: size,
        height: size,
        child: Stack(
          clipBehavior: Clip.none,
          children: [
            circle,
            Positioned(
              top: -dot * 0.2,
              right: -dot * 0.2,
              child: Container(
                width: dot,
                height: dot,
                decoration: BoxDecoration(
                  color: scheme.error,
                  shape: BoxShape.circle,
                  border: Border.all(
                    color: Theme.of(context).scaffoldBackgroundColor,
                    width: 2,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
