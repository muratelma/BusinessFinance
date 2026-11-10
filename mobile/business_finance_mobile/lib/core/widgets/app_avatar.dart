import 'package:flutter/material.dart';

import '../theme/app_finance_colors.dart';
import '../theme/app_surfaces.dart';

/// Avatarın köşesindeki e-posta durumu.
enum AppAvatarStatus {
  /// Sağ üstte kırmızı nokta: e-posta doğrulanmadı.
  unverified,

  /// Sağ altta yeşil tik: e-posta doğrulandı.
  verified,
}

/// Hesabın avatarı: marka zemininde e-postanın baş harfleri.
///
/// Uygulamada profil fotoğrafı yok; baş harf okunamıyorsa kişi ikonu çizilir.
/// [status] e-postanın durumunu köşede gösterir (teslim
/// `design/design_handoff_hesabim`, 10 Kasım 2026): doğrulanmamış adres sağ
/// üstte kırmızı nokta, doğrulanmış adres sağ altta yeşil tik. İşaret avatarın
/// boyuna göre ölçeklenir. Avatar ekran okuyucuya kapalıdır; durumun adı
/// (`E-posta doğrulandı` / `E-posta doğrulanmadı`) çağıranın etiketinde yazar,
/// bilgi yalnız renkle taşınmaz.
class AppAvatar extends StatelessWidget {
  const AppAvatar({
    required this.initials,
    super.key,
    this.size = 32,
    this.status,
  });

  final String initials;
  final double size;
  final AppAvatarStatus? status;

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
                fontSize: size * 0.38,
                height: 1,
                fontWeight: FontWeight.w700,
                letterSpacing: 0,
                color: scheme.onPrimary,
              ),
            ),
    );
    final status = this.status;
    if (status == null) return ExcludeSemantics(child: circle);
    return ExcludeSemantics(
      child: SizedBox(
        width: size,
        height: size,
        child: Stack(
          clipBehavior: Clip.none,
          children: [
            circle,
            switch (status) {
              AppAvatarStatus.unverified => _dot(context),
              AppAvatarStatus.verified => _tick(context),
            },
          ],
        ),
      ),
    );
  }

  Widget _dot(BuildContext context) {
    final diameter = (size * 0.2).roundToDouble();
    const ring = 2.0;
    return Positioned(
      top: -1 - ring,
      right: -1 - ring,
      child: Container(
        width: diameter + ring * 2,
        height: diameter + ring * 2,
        decoration: BoxDecoration(
          color: AppFinanceColors.of(context).expenseFill,
          shape: BoxShape.circle,
          border: Border.all(
            color: AppSurfaces.of(context).canvas,
            width: ring,
          ),
        ),
      ),
    );
  }

  Widget _tick(BuildContext context) {
    final diameter = (size * 0.34).roundToDouble();
    return Positioned(
      right: -2,
      bottom: -2,
      child: Container(
        width: diameter,
        height: diameter,
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: AppSurfaces.of(context).card,
          shape: BoxShape.circle,
        ),
        child: Icon(
          Icons.verified,
          size: diameter - 2,
          color: AppFinanceColors.of(context).income,
        ),
      ),
    );
  }
}
