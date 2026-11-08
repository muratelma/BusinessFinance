import 'dart:io';
import 'dart:ui' as ui;

import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_flow.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';

import '../helpers/auth_flow.dart';
import 'screenshot_harness.dart';

/// Bir çizim çerçevesi: ekran boyu, durum çubuğu ve alt pay, platform.
typedef _Device = ({
  String name,
  Size size,
  double top,
  double bottom,
  TargetPlatform platform,
});

/// Nomiva giriş akışını iki telefonda (iPhone 15, Pixel 8) ve telefon yatay ile
/// tablet çerçevesinde çizer. Telefon kareleri `design/nomiva-handoff/ekranlar/` ile aynı
/// oranda yan yana konup karşılaştırılır. Yalnız `SCREENSHOT_DIR` verilince
/// çalışır.
void main() {
  const iphone = (
    name: 'iphone',
    size: Size(393, 852),
    top: 59.0,
    bottom: 34.0,
    platform: TargetPlatform.iOS,
  );
  const pixel = (
    name: 'pixel',
    size: Size(412, 915),
    top: 24.0,
    bottom: 24.0,
    platform: TargetPlatform.android,
  );
  const landscape = (
    name: 'yatay',
    size: Size(915, 412),
    top: 24.0,
    bottom: 0.0,
    platform: TargetPlatform.android,
  );
  const tablet = (
    name: 'tablet',
    size: Size(800, 1280),
    top: 24.0,
    bottom: 24.0,
    platform: TargetPlatform.android,
  );

  Future<void> figtree() async {
    final loader = FontLoader('Figtree');
    for (final w in ['Regular', 'Medium', 'SemiBold', 'Bold']) {
      loader.addFont(rootBundle.load('assets/fonts/figtree/Figtree-$w.ttf'));
    }
    await loader.load();
  }

  Future<void> shoot(
    WidgetTester tester,
    _Device device,
    String name, {
    bool dark = false,
    AuthView view = AuthView.login,
    Duration? stopIntroAt,
    Future<void> Function(WidgetTester tester)? before,
  }) async {
    await tester.runAsync(() async {
      await loadDesignFonts();
      await figtree();
    });
    const ratio = 3.0;
    tester.view.physicalSize = device.size * ratio;
    tester.view.devicePixelRatio = ratio;
    tester.view.padding = FakeViewPadding(
      top: device.top * ratio,
      bottom: device.bottom * ratio,
    );
    addTearDown(tester.view.reset);
    final base = dark ? AppTheme.dark() : AppTheme.light();
    final boundary = GlobalKey();
    await tester.pumpWidget(
      RepaintBoundary(
        key: boundary,
        child: authFlowApp(
          theme: base.copyWith(platform: device.platform),
          initialView: view,
        ),
      ),
    );
    // SVG çözümlemesi gerçek zamanda biter.
    await tester.runAsync(
      () => Future<void>.delayed(const Duration(milliseconds: 300)),
    );
    if (stopIntroAt != null) {
      await tester.pump(stopIntroAt);
    } else {
      await tester.pumpAndSettle();
      if (before != null) {
        await before(tester);
        await tester.pumpAndSettle();
      }
      // İmleç ve klavye karede görünmesin.
      FocusManager.instance.primaryFocus?.unfocus();
      await tester.pumpAndSettle();
    }
    await tester.runAsync(() async {
      final render =
          boundary.currentContext!.findRenderObject()! as RenderRepaintBoundary;
      final image = await render.toImage(pixelRatio: ratio);
      final bytes = await image.toByteData(format: ui.ImageByteFormat.png);
      File('$screenshotDir/giris-${device.name}-$name.png')
        ..createSync(recursive: true)
        ..writeAsBytesSync(bytes!.buffer.asUint8List());
    });
  }

  Future<void> fillLogin(WidgetTester tester) async {
    await tester.enterText(
      authField('E-posta ya da kullanıcı adı'),
      'ahmet.yilmaz',
    );
    await tester.enterText(authField('Parola'), '123456789');
  }

  Future<void> fillRegister(
    WidgetTester tester, {
    String password = 'Valid-Password-123!',
  }) async {
    await tester.enterText(authField('Kullanıcı adı'), 'ahmet.yilmaz');
    await tester.enterText(authField('E-posta'), 'ahmet@ornek.com');
    await tester.enterText(authField('Parola'), password);
  }

  for (final device in [iphone, pixel]) {
    testWidgets('${device.name}: açılış', (tester) async {
      await shoot(
        tester,
        device,
        '01-acilis',
        stopIntroAt: const Duration(milliseconds: 2000),
      );
    }, skip: !screenshotsEnabled);

    for (final dark in [false, true]) {
      final tone = dark ? 'koyu' : 'acik';

      testWidgets('${device.name}: giriş, $tone', (tester) async {
        await shoot(
          tester,
          device,
          '02-giris-$tone',
          dark: dark,
          before: fillLogin,
        );
      }, skip: !screenshotsEnabled);

      testWidgets('${device.name}: hesap oluştur, $tone', (tester) async {
        await shoot(
          tester,
          device,
          '03-hesap-$tone',
          dark: dark,
          view: AuthView.register,
          before: fillRegister,
        );
      }, skip: !screenshotsEnabled);

      testWidgets('${device.name}: parola sıfırla, $tone', (tester) async {
        await shoot(
          tester,
          device,
          '04-sifirla-$tone',
          dark: dark,
          view: AuthView.reset,
        );
      }, skip: !screenshotsEnabled);
    }

    testWidgets('${device.name}: hesap oluştur, hatalar', (tester) async {
      await shoot(
        tester,
        device,
        '06-hesap-hatali',
        view: AuthView.register,
        before: (tester) async {
          await fillRegister(tester, password: '123456789');
          await tester.tap(authButton('Hesap oluştur'));
        },
      );
    }, skip: !screenshotsEnabled);

    testWidgets('${device.name}: giriş, hatalar', (tester) async {
      await shoot(
        tester,
        device,
        '05-giris-hatali',
        before: (tester) async {
          await tester.enterText(
            authField('E-posta ya da kullanıcı adı'),
            'ahmet@ornek',
          );
          await tester.tap(authButton('Giriş yap'));
        },
      );
    }, skip: !screenshotsEnabled);

    testWidgets('${device.name}: hesap oluştur, kural hataları', (
      tester,
    ) async {
      await shoot(
        tester,
        device,
        '07-hesap-kurallar',
        view: AuthView.register,
        before: (tester) async {
          await tester.enterText(authField('Kullanıcı adı'), 'ahmet yılmaz');
          await tester.enterText(authField('E-posta'), 'ahmet@ornek.com');
          await tester.enterText(authField('Parola'), 'ahmetyilmaz1234');
          await tester.tap(authButton('Hesap oluştur'));
        },
      );
    }, skip: !screenshotsEnabled);

    testWidgets('${device.name}: bir adım kaldı', (tester) async {
      await shoot(
        tester,
        device,
        '08-bir-adim',
        view: AuthView.register,
        before: (tester) async {
          await fillRegister(tester);
          await tester.tap(authButton('Hesap oluştur'));
          await tester.pumpAndSettle();
          await tester.tap(find.text('İşletmem var'));
        },
      );
    }, skip: !screenshotsEnabled);
  }

  for (final device in [landscape, tablet]) {
    testWidgets('${device.name}: giriş', (tester) async {
      await shoot(tester, device, '02-giris-acik', before: fillLogin);
    }, skip: !screenshotsEnabled);
  }
}
