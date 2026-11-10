import 'package:business_finance_mobile/app/launch_curtain.dart';
import 'package:business_finance_mobile/core/presentation/launch_signals.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_controller.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../helpers/fake_auth.dart';

void main() {
  setUp(LaunchHandoff.reset);

  Future<AuthController> controller({required bool signedIn}) async {
    final repository = FakeAuthSessionRepository()
      ..session = signedIn ? testSession() : null;
    return AuthController(repository);
  }

  Widget app(AuthController auth) => MaterialApp(
    home: LaunchCurtain(
      authController: auth,
      child: const Scaffold(body: Text('İçerik')),
    ),
  );

  Finder curtain() => find.bySemanticsLabel('Nomiva açılıyor');

  /// Perdenin beklemeleri kare değil zamanlayıcıdır; `pumpAndSettle` onları
  /// ilerletmez. Saniye saniye ilerletilir.
  Future<void> settle(WidgetTester tester) async {
    for (var i = 0; i < 6; i++) {
      await tester.pump(const Duration(milliseconds: 500));
    }
    await tester.pumpAndSettle();
  }

  testWidgets('oturum açıkken perde Özet hazır olunca kalkar', (tester) async {
    final auth = await controller(signedIn: true);
    await tester.pumpWidget(app(auth));
    await auth.initialize();
    await tester.pump(const Duration(seconds: 3));
    expect(auth.status, AuthStatus.authenticated);
    // Özet verisini bekliyor.
    expect(curtain(), findsOneWidget);

    LaunchHandoff.contentReady.value = true;
    await settle(tester);

    expect(curtain(), findsNothing);
    expect(find.text('İçerik'), findsOneWidget);
    // Oturum açıkken giriş ekranına devredilecek işaret yok.
    expect(LaunchHandoff.take(), isFalse);
  });

  testWidgets('Özet verisi hiç gelmezse perde 6 sn sonra yine kalkar', (
    tester,
  ) async {
    final auth = await controller(signedIn: true);
    await tester.pumpWidget(app(auth));
    await auth.initialize();
    await tester.pump(const Duration(seconds: 3));
    // Özet hazır olana kadar sabit işarette kalınır.
    expect(curtain(), findsOneWidget);
    await tester.pump(const Duration(seconds: 4));
    await settle(tester);

    expect(curtain(), findsNothing);
  });

  testWidgets('oturum kapalıyken işaret giriş ekranına devredilir', (
    tester,
  ) async {
    final auth = await controller(signedIn: false);
    await tester.pumpWidget(app(auth));
    await auth.initialize();
    await tester.pump(const Duration(seconds: 3));

    // Giriş ekranı işareti perdedeki yerinden devralır; bir kez.
    expect(LaunchHandoff.take(), isTrue);
    expect(LaunchHandoff.take(), isFalse);

    await settle(tester);
    expect(curtain(), findsNothing);
    var lifted = false;
    await LaunchHandoff.curtainLifted.then((_) => lifted = true);
    expect(lifted, isTrue);
  });

  testWidgets('işaret en az bir saniye görünür', (tester) async {
    final auth = await controller(signedIn: true);
    await tester.pumpWidget(app(auth));
    // Özet hemen hazır; işaret yine de en az 1 sn görünür.
    LaunchHandoff.contentReady.value = true;
    await auth.initialize();
    await tester.pump(const Duration(milliseconds: 300));

    expect(curtain(), findsOneWidget);
    await tester.pump(const Duration(seconds: 3));
    await settle(tester);
    expect(curtain(), findsNothing);
  });

  testWidgets('girişten sonra Özet hazır olana kadar zemin rengi örter', (
    tester,
  ) async {
    final auth = await controller(signedIn: false);
    await tester.pumpWidget(app(auth));
    await auth.initialize();
    await settle(tester);
    expect(curtain(), findsNothing);

    await auth.login('kisi@ornek.com', 'Parola-123456');
    await tester.pump();
    // Örtü kurulu: dokunuşu yutar, içerik altında hazırlanır.
    expect(find.byType(AbsorbPointer), findsWidgets);
    final cover = find.byWidgetPredicate(
      (widget) => widget is ColoredBox && widget.color != Colors.transparent,
    );
    expect(cover, findsWidgets);

    LaunchHandoff.contentReady.value = true;
    await settle(tester);
    expect(find.text('İçerik'), findsOneWidget);
    expect(
      find.descendant(
        of: find.byType(LaunchCurtain),
        matching: find.byType(AbsorbPointer),
      ),
      findsNothing,
    );
  });
}
