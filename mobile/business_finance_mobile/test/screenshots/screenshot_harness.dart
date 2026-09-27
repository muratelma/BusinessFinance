import 'dart:io';
import 'dart:ui' as ui;

import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/activities/presentation/quick_add_navigation.dart';
import 'package:business_finance_mobile/features/shell/presentation/main_shell.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';

/// Ekranları tasarım teslim paketindeki çerçeveyle (412×892 dp, 2×) aynı
/// biçimde PNG'ye çizer; görüntüler tasarımla yan yana karşılaştırılır.
///
/// Yalnız `SCREENSHOT_DIR` verildiğinde çalışır: normal `flutter test`
/// koşusunda atlanır. Test ortamının varsayılan kutu fontu yerine gerçek
/// Roboto ve Material ikon fontu yüklenir (`FLUTTER_ROOT` altındaki
/// `material_fonts`).
///
/// ```bash
/// SCREENSHOT_DIR=/tmp/shots flutter test test/screenshots
/// ```
final String? screenshotDir = Platform.environment['SCREENSHOT_DIR'];

bool get screenshotsEnabled => screenshotDir != null;

const Size designFrame = Size(412, 892);

bool _fontsLoaded = false;

Future<void> loadDesignFonts() async {
  if (_fontsLoaded) return;
  final root =
      Platform.environment['FLUTTER_ROOT'] ??
      File(Platform.resolvedExecutable).parent.parent.parent.parent.path;
  final dir = Directory('$root/bin/cache/artifacts/material_fonts');
  final roboto = FontLoader('Roboto');
  for (final weight in ['Light', 'Regular', 'Medium', 'Bold', 'Black']) {
    roboto.addFont(_fontData('${dir.path}/Roboto-$weight.ttf'));
  }
  await roboto.load();

  final icons = FontLoader('MaterialIcons')
    ..addFont(_fontData('${dir.path}/MaterialIcons-Regular.otf'));
  await icons.load();
  _fontsLoaded = true;
}

Future<ByteData> _fontData(String path) async =>
    ByteData.sublistView(await File(path).readAsBytes());

/// [page]'i tasarım çerçevesinde çizer ve `<SCREENSHOT_DIR>/<name>.png`
/// olarak yazar. [withNavBar] ana sekmelerin çentikli alt çubuğunu ekler
/// ([selectedTab] seçili sekme).
Future<void> captureScreen(
  WidgetTester tester,
  String name,
  Widget page, {
  bool withNavBar = true,
  int selectedTab = 0,
  bool hasBusiness = true,
  bool pushed = false,
  Future<void> Function(WidgetTester tester)? before,
}) async {
  // Font yükleme gerçek G/Ç'dir; sahte zamanın içinde hiç tamamlanmaz.
  await tester.runAsync(loadDesignFonts);
  tester.view.physicalSize = designFrame * 2;
  tester.view.devicePixelRatio = 2;
  addTearDown(tester.view.reset);

  final boundary = GlobalKey();
  final body = withNavBar
      ? Scaffold(
          body: page,
          floatingActionButtonLocation:
              FloatingActionButtonLocation.centerDocked,
          floatingActionButton: Builder(
            builder: (context) => FloatingActionButton(
              heroTag: 'screenshot-fab',
              tooltip: 'İşlem ekle',
              onPressed: () => openQuickAdd(context),
              child: const Icon(Icons.add),
            ),
          ),
          bottomNavigationBar: ShellNavigationBar(
            destinations: MainShell.destinationsFor(hasBusiness),
            currentIndex: selectedTab,
            onSelected: (_) {},
          ),
        )
      : page;
  await tester.pumpWidget(
    RepaintBoundary(
      key: boundary,
      child: MaterialApp(
        debugShowCheckedModeBanner: false,
        theme: _screenshotTheme(),
        home: pushed ? _PushedHost(child: body) : body,
      ),
    ),
  );
  await _settle(tester);
  if (before != null) {
    await before(tester);
    await _settle(tester);
  }

  await tester.runAsync(() async {
    final render =
        boundary.currentContext!.findRenderObject()! as RenderRepaintBoundary;
    final image = await render.toImage(pixelRatio: 2);
    final bytes = await image.toByteData(format: ui.ImageByteFormat.png);
    final file = File('$screenshotDir/$name.png')..createSync(recursive: true);
    file.writeAsBytesSync(bytes!.buffer.asUint8List());
  });
}

/// Sonsuz animasyonlu ekranda (ilerleme göstergesi) `pumpAndSettle` hiç
/// bitmez; bir saniyelik sabit adım yeter.
Future<void> _settle(WidgetTester tester) async {
  for (var i = 0; i < 10; i++) {
    await tester.pump(const Duration(milliseconds: 100));
  }
}

/// Sayfayı bir önceki rotanın üstüne iter: geri oku olan tam ekran sayfalar
/// (Bütçeler, Hesabım) böyle çizilir.
class _PushedHost extends StatefulWidget {
  const _PushedHost({required this.child});

  final Widget child;

  @override
  State<_PushedHost> createState() => _PushedHostState();
}

class _PushedHostState extends State<_PushedHost> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      Navigator.of(context).push(
        PageRouteBuilder<void>(
          transitionDuration: Duration.zero,
          pageBuilder: (_, _, _) => widget.child,
        ),
      );
    });
  }

  @override
  Widget build(BuildContext context) => const SizedBox.shrink();
}

/// Düğme temaları ham `labelLarge`'ı alır ve font ailesi taşımaz; cihazda o
/// metin Roboto'dur, test motorunda kutu fontudur. Görüntü cihazdaki gibi
/// olsun diye düğme metni temanın birleşik (Roboto'lu) stiliyle çizilir.
ThemeData _screenshotTheme() {
  final theme = AppTheme.light();
  final label = WidgetStatePropertyAll(theme.textTheme.labelLarge);
  final merged = theme.textTheme.bodyMedium;
  return theme.copyWith(
    chipTheme: theme.chipTheme.copyWith(
      labelStyle: merged?.merge(theme.chipTheme.labelStyle),
      secondaryLabelStyle: merged?.merge(theme.chipTheme.secondaryLabelStyle),
    ),
    filledButtonTheme: FilledButtonThemeData(
      style: theme.filledButtonTheme.style?.copyWith(textStyle: label),
    ),
    outlinedButtonTheme: OutlinedButtonThemeData(
      style: theme.outlinedButtonTheme.style?.copyWith(textStyle: label),
    ),
    textButtonTheme: TextButtonThemeData(
      style: theme.textButtonTheme.style?.copyWith(textStyle: label),
    ),
  );
}
