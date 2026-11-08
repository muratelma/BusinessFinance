import 'dart:async';

import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_flow.dart';
import 'package:business_finance_mobile/features/auth/presentation/nomiva_brand.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_validation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../../../helpers/accessibility.dart';
import '../../../helpers/auth_flow.dart';

const loginId = 'E-posta ya da kullanıcı adı';

void main() {
  group('giriş', () {
    testWidgets('erişilebilirlik kapısını geçer', (tester) async {
      await pumpAtLargestTextScale(tester, authFlowApp());

      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);
    });

    testWidgets('koyu temada da erişilebilirlik kapısını geçer', (
      tester,
    ) async {
      await pumpAtLargestTextScale(tester, authFlowApp(theme: AppTheme.dark()));

      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);
    });

    testWidgets('alanlar doğrulanmadan istek gitmez', (tester) async {
      var submitCount = 0;
      await pumpAuthFlow(
        tester,
        authFlowApp(onLogin: (_, _, {onVerified}) async => submitCount++),
      );

      await tester.tap(authButton('Giriş yap'));
      await tester.pump();

      expect(find.text(AuthValidation.identifierEmpty), findsOneWidget);
      expect(find.text(AuthValidation.passwordEmpty), findsOneWidget);
      expect(submitCount, 0);
    });

    testWidgets('istek sürerken ikinci kez gönderilmez', (tester) async {
      final completer = Completer<void>();
      var submitCount = 0;
      await pumpAuthFlow(
        tester,
        authFlowApp(
          onLogin: (_, _, {onVerified}) {
            submitCount++;
            return completer.future;
          },
        ),
      );
      await tester.enterText(authField(loginId), 'user@example.test');
      await tester.enterText(authField('Parola'), 'password');

      await tester.tap(authButton('Giriş yap'));
      await tester.pump();
      await tester.tap(find.byType(FilledButton));
      await tester.pump();

      expect(submitCount, 1);
      expect(find.byType(CircularProgressIndicator), findsOneWidget);
      completer.complete();
      await tester.pumpAndSettle();
    });

    testWidgets('sunucunun cevabı formun içinde görünür', (tester) async {
      await pumpAuthFlow(
        tester,
        authFlowApp(
          onLogin: (_, _, {onVerified}) => throw const ApiException(
            statusCode: 401,
            code: 'authentication.invalid_credentials',
            message: 'E-posta veya parola geçersiz.',
          ),
        ),
      );
      await tester.enterText(authField(loginId), 'user@example.test');
      await tester.enterText(authField('Parola'), 'wrong');

      expect(find.byTooltip('Parolayı göster'), findsOneWidget);
      await tester.tap(authButton('Giriş yap'));
      await tester.pumpAndSettle();

      expect(find.text('E-posta veya parola geçersiz.'), findsOneWidget);
      // Düğme geri açıldı: yeniden denenebilir.
      expect(find.byType(CircularProgressIndicator), findsNothing);
    });

    testWidgets('başarılı girişte oturum geçiş bitince açılır', (tester) async {
      var verified = false;
      await pumpAuthFlow(
        tester,
        authFlowApp(
          onLogin: (_, _, {onVerified}) async {
            await onVerified?.call();
            verified = true;
          },
        ),
      );
      await tester.enterText(authField(loginId), 'user@example.test');
      await tester.enterText(authField('Parola'), 'password');

      await tester.tap(authButton('Giriş yap'));
      await tester.pump(const Duration(milliseconds: 200));
      expect(verified, isFalse);

      await tester.pumpAndSettle();
      expect(verified, isTrue);
    });

    testWidgets('bağlantıların yanındaki boşluk bağlantı değildir', (
      tester,
    ) async {
      tester.view.physicalSize = const Size(390, 844);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      await pumpAuthFlow(tester, authFlowApp());

      final formLeft = tester.getTopLeft(find.byType(FilledButton)).dx;
      for (final label in ['Parolamı unuttum', 'Hesap oluştur']) {
        final text = tester.getCenter(find.text(label));
        await tester.tapAt(Offset(formLeft + 8, text.dy));
        await tester.pumpAndSettle();
      }
      expect(find.text('Parolanı sıfırla'), findsNothing);
      expect(find.widgetWithText(FilledButton, 'Hesap oluştur'), findsNothing);

      await tester.tap(find.text('Hesap oluştur'));
      await tester.pumpAndSettle();
      expect(authButton('Hesap oluştur'), findsOneWidget);
    });
  });

  group('açılış', () {
    testWidgets('dokunmak açılışı bitirir', (tester) async {
      await tester.pumpWidget(authFlowApp());
      await tester.pump(const Duration(milliseconds: 300));
      // Form henüz ekranın altında, açılışın arkasında: dokunulamaz.
      expect(authButton('Giriş yap').hitTestable(), findsNothing);

      await tester.tap(find.bySemanticsLabel('Açılışı geç'));
      await tester.pump();

      expect(find.bySemanticsLabel('Açılışı geç'), findsNothing);
      expect(authButton('Giriş yap').hitTestable(), findsOneWidget);
    });

    testWidgets('hareketi azaltan kullanıcı açılışı izlemez', (tester) async {
      await tester.pumpWidget(
        MediaQuery(
          data: const MediaQueryData(disableAnimations: true),
          child: authFlowApp(),
        ),
      );
      await tester.pump();

      expect(authButton('Giriş yap'), findsOneWidget);
      expect(find.bySemanticsLabel('Açılışı geç'), findsNothing);
    });
  });

  group('hesap oluşturma', () {
    testWidgets('parola kuralı tutmadan ikinci adıma geçilmez', (tester) async {
      await pumpAuthFlow(tester, authFlowApp(initialView: AuthView.register));

      await tester.enterText(authField('Kullanıcı adı'), 'ahmet.yilmaz');
      await tester.enterText(authField('E-posta'), 'new@example.test');
      await tester.enterText(authField('Parola'), 'short');
      await tester.tap(authButton('Hesap oluştur'));
      await tester.pumpAndSettle();

      expect(find.text(AuthValidation.passwordRule), findsOneWidget);
      expect(find.text('Bir adım kaldı'), findsNothing);
    });

    testWidgets('hesap ikinci adımda açılır ve oturum hemen başlar', (
      tester,
    ) async {
      ({String email, String password, bool hasBusiness})? sent;
      var verified = false;
      await pumpAuthFlow(
        tester,
        authFlowApp(
          initialView: AuthView.register,
          onRegister:
              (email, password, {required hasBusiness, onVerified}) async {
                sent = (
                  email: email,
                  password: password,
                  hasBusiness: hasBusiness,
                );
                await onVerified?.call();
                verified = true;
              },
        ),
      );

      await tester.enterText(authField('Kullanıcı adı'), 'ahmet.yilmaz');
      await tester.enterText(authField('E-posta'), 'new@example.test');
      await tester.enterText(authField('Parola'), 'Valid-Password-123!');
      await tester.tap(authButton('Hesap oluştur'));
      await tester.pumpAndSettle();

      expect(sent, isNull, reason: 'Hesap ilk adımda açılmaz.');
      expect(find.text('Bir adım kaldı'), findsOneWidget);

      await tester.tap(authButton('Devam et'));
      await tester.pumpAndSettle();

      expect(sent?.email, 'new@example.test');
      expect(sent?.password, 'Valid-Password-123!');
      expect(sent?.hasBusiness, isFalse);
      expect(verified, isTrue);
    });

    testWidgets('sunucunun reddi kişiyi forma geri getirir', (tester) async {
      await pumpAuthFlow(
        tester,
        authFlowApp(
          initialView: AuthView.register,
          onRegister: (_, _, {required hasBusiness, onVerified}) =>
              throw const ApiException(
                statusCode: 409,
                code: 'registration.duplicate_email',
                message: 'Bu e-posta adresi zaten kayıtlı.',
              ),
        ),
      );
      await tester.enterText(authField('Kullanıcı adı'), 'ahmet.yilmaz');
      await tester.enterText(authField('E-posta'), 'old@example.test');
      await tester.enterText(authField('Parola'), 'Valid-Password-123!');
      await tester.tap(authButton('Hesap oluştur'));
      await tester.pumpAndSettle();
      await tester.tap(authButton('Devam et'));
      await tester.pumpAndSettle();

      expect(find.text('Bu e-posta adresi zaten kayıtlı.'), findsOneWidget);
      expect(authButton('Hesap oluştur'), findsOneWidget);
      // Yazılanlar duruyor.
      expect(find.text('old@example.test'), findsOneWidget);
    });

    testWidgets('geri hareketi bir önceki adıma döner, uygulamadan çıkmaz', (
      tester,
    ) async {
      await pumpAuthFlow(tester, authFlowApp(initialView: AuthView.register));

      await tester.binding.handlePopRoute();
      await tester.pumpAndSettle();

      expect(authButton('Giriş yap'), findsOneWidget);
    });
  });

  group('tasarımla aynı yerleşim', () {
    // Tasarım 320 pt genişlikte çizildi; ölçüler ekrana oranla büyür. İki
    // telefonda alanın ekran genişliğine oranı ve başlığın yazı boyunun
    // genişliğe oranı tasarımdakiyle aynı olmalı.
    for (final phone in const [Size(412, 915), Size(393, 852)]) {
      testWidgets('${phone.width.toInt()} pt genişlikte oranlar tasarımdaki', (
        tester,
      ) async {
        tester.view.physicalSize = phone;
        tester.view.devicePixelRatio = 1;
        addTearDown(tester.view.reset);
        await pumpAuthFlow(tester, authFlowApp());

        final scale = NomivaScale.forSize(phone).factor;
        final field = tester.getSize(authField(loginId));
        // Alan: ekran genişliği − 2 × 24 pt yan boşluk (tasarımda 272 / 320).
        expect(field.width, closeTo(phone.width - 2 * 24 * scale, 0.5));
        expect(field.height, closeTo(46 * scale, 0.5));
        expect(
          tester.getSize(authButton('Giriş yap')).height,
          closeTo(50 * scale, 0.5),
        );
        final title = tester.widget<Text>(find.text('Giriş yap').first);
        expect(title.style?.fontSize, closeTo(23 * scale, 0.01));
        // Katsayı genişliğe göredir ve iki telefonda %2'den az ayrışır.
        expect(scale, closeTo(phone.width / 320, phone.width / 320 * 0.02));
      });
    }

    testWidgets('tablette form bir telefon genişliğinde kalır', (tester) async {
      tester.view.physicalSize = const Size(800, 1280);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      await pumpAuthFlow(tester, authFlowApp());

      final field = tester.getSize(authField(loginId));
      expect(field.width, lessThanOrEqualTo(320 * NomivaScale.maxScale));
      expectNoOverflow(tester);
    });

    testWidgets('telefon yatayda form kayar, taşmaz', (tester) async {
      tester.view.physicalSize = const Size(915, 412);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      await pumpAuthFlow(tester, authFlowApp());

      expectNoOverflow(tester);
      await tester.scrollUntilVisible(
        find.text('Hesap oluştur'),
        100,
        scrollable: find.byType(Scrollable).first,
      );
      expect(find.text('Hesap oluştur'), findsOneWidget);
    });
  });

  group('Google ve Apple', () {
    testWidgets('Google düğmesi görünür; henüz hazır olmadığını söyler', (
      tester,
    ) async {
      await pumpAuthFlow(tester, authFlowApp());

      expect(find.text('ya da'), findsOneWidget);
      await tester.tap(find.bySemanticsLabel('Google ile devam et'));
      await tester.pumpAndSettle();

      expect(
        find.textContaining('Google ile giriş henüz hazır değil.'),
        findsOneWidget,
      );
    });

    testWidgets('Apple düğmesi yalnız iPhone’da', (tester) async {
      await pumpAuthFlow(tester, authFlowApp());
      expect(find.bySemanticsLabel('Apple ile devam et'), findsNothing);

      await pumpAuthFlow(
        tester,
        authFlowApp(
          theme: AppTheme.light().copyWith(platform: TargetPlatform.iOS),
        ),
      );
      expect(find.bySemanticsLabel('Apple ile devam et'), findsOneWidget);
    });

    testWidgets('kullanıcı adıyla giriş istek göndermeden açıklanır', (
      tester,
    ) async {
      var submitCount = 0;
      await pumpAuthFlow(
        tester,
        authFlowApp(onLogin: (_, _, {onVerified}) async => submitCount++),
      );
      await tester.enterText(authField(loginId), 'ahmet.yilmaz');
      await tester.enterText(authField('Parola'), 'password');
      await tester.tap(authButton('Giriş yap'));
      await tester.pump();

      expect(find.text(AuthValidation.usernameLoginNotReady), findsOneWidget);
      expect(submitCount, 0);
    });

    testWidgets('kayıtta kullanıcı adı sorulur ve doğrulanır', (tester) async {
      await pumpAuthFlow(tester, authFlowApp(initialView: AuthView.register));

      await tester.enterText(authField('Kullanıcı adı'), 'a b');
      await tester.enterText(authField('E-posta'), 'new@example.test');
      await tester.enterText(authField('Parola'), 'Valid-Password-123!');
      await tester.tap(authButton('Hesap oluştur'));
      await tester.pumpAndSettle();

      expect(find.text('Bir adım kaldı'), findsNothing);
      expect(find.text(AuthValidation.usernameCharacters), findsOneWidget);
    });
  });

  group('hata zamanlaması', () {
    // Yaygın kural ("reward early, punish late"): yazarken denetlenmez; hata
    // alandan çıkınca ya da gönderince görünür, düzelince hemen kalkar.
    testWidgets('hata yazarken başka hataya dönmez, düzelince kalkar', (
      tester,
    ) async {
      await pumpAuthFlow(tester, authFlowApp(initialView: AuthView.register));
      await tester.tap(authButton('Hesap oluştur'));
      await tester.pumpAndSettle();
      expect(find.text(AuthValidation.usernameEmpty), findsOneWidget);

      await tester.enterText(authField('Kullanıcı adı'), 'ab');
      await tester.pumpAndSettle();
      // Hâlâ kurala uymuyor: aynı yazı durur, "en az 3" diye değişmez.
      expect(find.text(AuthValidation.usernameEmpty), findsOneWidget);
      expect(find.text(AuthValidation.usernameTooShort), findsNothing);

      await tester.enterText(authField('Kullanıcı adı'), 'ahmet');
      await tester.pumpAndSettle();
      expect(find.text(AuthValidation.usernameEmpty), findsNothing);
    });

    testWidgets('alandan çıkınca denetlenir; boş alan denetlenmez', (
      tester,
    ) async {
      await pumpAuthFlow(tester, authFlowApp(initialView: AuthView.register));

      // Boş kullanıcı adından geçip gitmek hata yazdırmaz.
      await tester.tap(authField('Kullanıcı adı'));
      await tester.tap(authField('E-posta'));
      await tester.pumpAndSettle();
      expect(find.text(AuthValidation.usernameEmpty), findsNothing);

      await tester.enterText(authField('E-posta'), 'ahmet@ornek');
      await tester.pumpAndSettle();
      // Yazarken sessiz.
      expect(find.text(AuthValidation.emailInvalid), findsNothing);

      await tester.tap(authField('Parola'));
      await tester.pumpAndSettle();
      expect(find.text(AuthValidation.emailInvalid), findsOneWidget);
    });

    testWidgets('panelden ayrılınca hatalar temizlenir, yazılanlar kalır', (
      tester,
    ) async {
      await pumpAuthFlow(
        tester,
        authFlowApp(
          onLogin: (_, _, {onVerified}) => throw const ApiException(
            statusCode: 401,
            code: 'authentication.invalid_credentials',
            message: 'E-posta veya parola geçersiz.',
          ),
        ),
      );
      await tester.enterText(authField(loginId), 'esnaf@example.test');
      await tester.tap(authButton('Giriş yap'));
      await tester.pumpAndSettle();
      expect(find.text(AuthValidation.passwordEmpty), findsOneWidget);

      await tester.tap(find.text('Hesap oluştur'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Giriş yap').last);
      await tester.pumpAndSettle();

      expect(find.text(AuthValidation.passwordEmpty), findsNothing);
      expect(find.text('esnaf@example.test'), findsOneWidget);

      // Sunucunun uyarısı da kalkar.
      await tester.enterText(authField('Parola'), 'yanlis');
      await tester.tap(authButton('Giriş yap'));
      await tester.pumpAndSettle();
      expect(find.text('E-posta veya parola geçersiz.'), findsOneWidget);
      await tester.tap(find.text('Parolamı unuttum'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Girişe dön'));
      await tester.pumpAndSettle();
      expect(find.text('E-posta veya parola geçersiz.'), findsNothing);
    });

    testWidgets('gönderim reddedilince imleç ilk hatalı alana gider', (
      tester,
    ) async {
      await pumpAuthFlow(tester, authFlowApp(initialView: AuthView.register));
      await tester.enterText(authField('Kullanıcı adı'), 'ahmet');
      await tester.enterText(authField('E-posta'), 'ahmet@ornek.com');
      await tester.enterText(authField('Parola'), 'kisa');
      FocusManager.instance.primaryFocus?.unfocus();
      await tester.pumpAndSettle();

      await tester.tap(authButton('Hesap oluştur'));
      await tester.pumpAndSettle();

      final password = tester.widget<EditableText>(
        find.descendant(
          of: authField('Parola'),
          matching: find.byType(EditableText),
        ),
      );
      expect(password.focusNode.hasFocus, isTrue);
    });
  });

  testWidgets('parolamı unuttum yazılan adresi sıfırlamaya taşır', (
    tester,
  ) async {
    final requests = <String>[];
    await pumpAuthFlow(
      tester,
      authFlowApp(onRequestPasswordReset: (email) async => requests.add(email)),
    );
    await tester.enterText(authField(loginId), 'esnaf@example.test');
    await tester.tap(find.text('Parolamı unuttum'));
    await tester.pumpAndSettle();
    await tester.tap(authButton('Kod gönder'));
    await tester.pumpAndSettle();

    expect(requests, ['esnaf@example.test']);
  });
}
