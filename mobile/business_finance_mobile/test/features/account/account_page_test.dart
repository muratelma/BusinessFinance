import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_code_input.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/account/data/account_models.dart';
import 'package:business_finance_mobile/features/account/data/account_repository.dart';
import 'package:business_finance_mobile/features/account/presentation/account_page.dart';

import 'package:business_finance_mobile/features/auth/data/google_id_token_source.dart';
import 'package:business_finance_mobile/features/auth/presentation/auth_controller.dart';
import 'package:provider/provider.dart';

import '../../helpers/fake_auth.dart';

import '../../helpers/accessibility.dart';

void main() {
  testWidgets('hesap sayfası e-postayı ve açık oturumları gösterir', (
    tester,
  ) async {
    await tester.pumpWidget(_app(FakeAccountRepository()));
    await tester.pumpAndSettle();

    expect(find.text('user@example.test'), findsOneWidget);
    expect(find.text('Bu cihaz'), findsOneWidget);
    expect(find.text('Başka bir cihaz'), findsOneWidget);
  });

  testWidgets('bu cihazın oturumu listeden kapatılamaz', (tester) async {
    // Kendi oturumunu kapatmak "çıkış yap"tır ve onun kendi satırı var;
    // aynı şeyi iki isimle sunmak kullanıcıyı yanıltırdı.
    await tester.pumpWidget(_app(FakeAccountRepository()));
    await tester.pumpAndSettle();

    expect(find.widgetWithIcon(IconButton, Icons.close), findsOneWidget);
  });

  testWidgets('başka cihazın oturumu × ile doğrudan kapanır', (tester) async {
    // Onay sorulmaz: geri alınabilir bir iştir, o cihazda yeniden giriş
    // yapılır (teslim, 10 Kasım 2026).
    final repository = FakeAccountRepository();
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithIcon(IconButton, Icons.close));
    await tester.pumpAndSettle();

    expect(find.text('Oturum kapatılsın mı?'), findsNothing);
    expect(repository.revokedSessionId, 'session-other');
    expect(find.text('Başka bir cihaz'), findsNothing);
    expect(find.text('Oturum kapatıldı.'), findsOneWidget);
  });

  testWidgets('sayfa büyük metin erişilebilirlik kapısını geçer', (
    tester,
  ) async {
    await pumpAtLargestTextScale(tester, _app(FakeAccountRepository()));
    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
    await tester.scrollUntilVisible(find.text('Hesabımı sil'), 300);
    await tester.pumpAndSettle();
    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });

  testWidgets('oturum satırı göreli zamanı söyler', (tester) async {
    await tester.pumpWidget(
      _app(FakeAccountRepository(), now: () => DateTime(2026, 8, 17, 12)),
    );
    await tester.pumpAndSettle();
    expect(find.text('7 gün önce açıldı'), findsOneWidget);
    expect(find.text('Şu an açık'), findsOneWidget);
  });

  testWidgets('tek oturumda Açık oturumlar yalnız bu cihazı yazar', (
    tester,
  ) async {
    await tester.pumpWidget(_app(FakeAccountRepository()..otherSessions = 0));
    await tester.pumpAndSettle();

    expect(find.text('Açık oturumlar'), findsOneWidget);
    expect(find.text('Bu cihaz'), findsOneWidget);
    expect(find.text('Şu an açık'), findsOneWidget);
    expect(find.text('Başka bir cihaz'), findsNothing);
    expect(find.byIcon(Icons.close), findsNothing);
  });

  testWidgets(
    'üç ve fazla başka cihaz tek satırda toplanır ve sayfası açılır',
    (tester) async {
      final repository = FakeAccountRepository()..otherSessions = 3;
      await tester.pumpWidget(_app(repository));
      await tester.pumpAndSettle();

      // Hesabım'da tek tek yazılmaz ve toplu kapatma burada yoktur.
      expect(find.text('Diğer cihazlar · 3'), findsOneWidget);
      expect(find.text('Başka bir cihaz'), findsNothing);
      expect(find.text('Diğer oturumları kapat'), findsNothing);

      await tester.tap(find.text('Diğer cihazlar · 3'));
      await tester.pumpAndSettle();

      expect(find.text('Başka bir cihaz'), findsNWidgets(3));
      await tester.tap(find.text('Diğer oturumları kapat'));
      await tester.pumpAndSettle();
      expect(find.text('Diğer oturumlar kapatılsın mı?'), findsOneWidget);

      await tester.tap(find.widgetWithText(FilledButton, 'Hepsini kapat'));
      await tester.pumpAndSettle();

      expect(repository.revokedSessionIds, hasLength(3));
      expect(find.text('Başka bir cihaz'), findsNothing);
      expect(find.text('Bu cihaz'), findsOneWidget);
    },
  );

  testWidgets('zayıf yeni parola istek gitmeden reddedilir', (tester) async {
    final repository = FakeAccountRepository();
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Parolamı değiştir'));
    await tester.pumpAndSettle();

    await tester.enterText(
      find.widgetWithText(TextField, 'Mevcut parola'),
      'Valid-Password-123!',
    );
    await tester.enterText(
      find.widgetWithText(TextField, 'Yeni parola'),
      'kisa',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Değiştir'));
    await tester.pumpAndSettle();

    expect(repository.passwordChangeCount, 0);
    expect(
      find.text('Yeni parola kuralların hepsini sağlamalı.'),
      findsOneWidget,
    );
    // Kurallar yazdıkça işaretlenir: 'kisa' yalnız küçük harf taşır.
    expect(find.text('En az 12 karakter'), findsOneWidget);
    expect(find.byIcon(Icons.check_circle), findsNothing);

    await tester.enterText(
      find.widgetWithText(TextField, 'Yeni parola'),
      'Valid-Password-123!',
    );
    await tester.pump();
    expect(find.byIcon(Icons.check_circle), findsNWidgets(4));
  });

  testWidgets('kimlik kartında kullanıcı adı, e-posta ve doğrulama tiki', (
    tester,
  ) async {
    final repository = FakeAccountRepository()
      ..username = 'murat_elma'
      ..emailConfirmed = true;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    expect(find.text('murat_elma'), findsOneWidget);
    expect(find.text('user@example.test'), findsOneWidget);
    // Tik yalnız renkle anlatılmaz: ekran okuyucu adını okur.
    expect(
      find.byWidgetPredicate(
        (widget) =>
            widget is Semantics &&
            widget.properties.label == 'E-posta doğrulandı',
      ),
      findsOneWidget,
    );
    expect(find.byIcon(Icons.verified), findsOneWidget);
    expect(find.text('E-postanızı doğrulayın'), findsNothing);
  });

  testWidgets('Çıkış yap sayfanın en altında, Hesabımı sil üstünde', (
    tester,
  ) async {
    await tester.pumpWidget(_app(FakeAccountRepository()));
    await tester.pumpAndSettle();
    await tester.scrollUntilVisible(find.text('Çıkış yap'), 200);

    final delete = tester.getTopLeft(find.text('Hesabımı sil')).dy;
    final logout = tester.getTopLeft(find.text('Çıkış yap')).dy;
    expect(logout, greaterThan(delete));
  });

  testWidgets('parola değişince taze token benimsenir', (tester) async {
    final repository = FakeAccountRepository();
    final authRepository = FakeAuthSessionRepository()..session = testSession();
    await tester.pumpWidget(_app(repository, authRepository: authRepository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Parolamı değiştir'));
    await tester.pumpAndSettle();
    await tester.enterText(
      find.widgetWithText(TextField, 'Mevcut parola'),
      'Valid-Password-123!',
    );
    await tester.enterText(
      find.widgetWithText(TextField, 'Yeni parola'),
      'Another-Password-456!',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Değiştir'));
    await tester.pumpAndSettle();

    expect(repository.passwordChangeCount, 1);
    expect(authRepository.session?.refreshToken, 'rotated-refresh');
    expect(authRepository.session?.sessionId, 'session-rotated');
  });

  testWidgets('yanlış parola oturum listesini bozmaz', (tester) async {
    final repository = FakeAccountRepository()
      ..passwordError = ApiException(
        statusCode: 401,
        code: 'account.invalid_password',
        message: 'Parolanız hatalı.',
      );
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Parolamı değiştir'));
    await tester.pumpAndSettle();
    await tester.enterText(
      find.widgetWithText(TextField, 'Mevcut parola'),
      'Wrong-Password-123!',
    );
    await tester.enterText(
      find.widgetWithText(TextField, 'Yeni parola'),
      'Another-Password-456!',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Değiştir'));
    await tester.pumpAndSettle();

    expect(find.text('Parolanız hatalı.'), findsOneWidget);
    // Panel açık kalır: kullanıcı yazdığını düzeltebilsin.
    expect(find.widgetWithText(TextField, 'Mevcut parola'), findsOneWidget);
  });

  testWidgets('hesap silme sayfası parola ve son onayla siler', (tester) async {
    final repository = FakeAccountRepository();
    final auth = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(auth);
    await tester.pumpWidget(
      ChangeNotifierProvider<AuthController?>.value(
        value: controller,
        child: _app(repository, authRepository: auth),
      ),
    );
    await tester.pumpAndSettle();

    await tester.scrollUntilVisible(find.text('Hesabımı sil'), 200);
    await tester.ensureVisible(find.text('Hesabımı sil'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Hesabımı sil'));
    await tester.pumpAndSettle();

    // Silme bir sayfadır: ne olacağını söyler, yedek kapısı oradadır.
    expect(find.text('Önce yedeğinizi alın'), findsOneWidget);
    final submit = find.widgetWithText(FilledButton, 'Hesabımı sil');
    // Parola yazılana kadar düğme pasiftir.
    expect(tester.widget<FilledButton>(submit).onPressed, isNull);

    await tester.enterText(
      find.widgetWithText(TextField, 'Parolanız'),
      'Valid-Password-123!',
    );
    await tester.pump();
    await tester.tap(submit);
    await tester.pumpAndSettle();

    // Ayrı son onay penceresi yok: düğme isteği doğrudan gönderir.
    expect(find.text('Hesabınız silinecek'), findsNothing);
    expect(repository.deletedWithPassword, 'Valid-Password-123!');
    expect(find.text('Hesabınız silindi'), findsOneWidget);
    expect(find.text('Giriş ekranına dön'), findsOneWidget);
  });

  testWidgets('yanlış parola Hesabımı sil sayfasında hemen söylenir', (
    tester,
  ) async {
    final repository = FakeAccountRepository()
      ..deleteError = const ApiException(
        statusCode: 400,
        code: 'account.invalid_password',
        message: 'Parolanız doğrulanamadı.',
      );
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();
    await tester.scrollUntilVisible(find.text('Hesabımı sil'), 200);
    await tester.ensureVisible(find.text('Hesabımı sil'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Hesabımı sil'));
    await tester.pumpAndSettle();

    await tester.enterText(
      find.widgetWithText(TextField, 'Parolanız'),
      'yanlis',
    );
    await tester.pump();
    await tester.tap(find.widgetWithText(FilledButton, 'Hesabımı sil'));
    await tester.pumpAndSettle();

    expect(find.text('Parolanız doğrulanamadı.'), findsOneWidget);
    expect(find.text('Hesabınız silindi'), findsNothing);
  });

  testWidgets('parolasız hesapta Parola belirle kodla parola koyar', (
    tester,
  ) async {
    final repository = FakeAccountRepository()..hasPassword = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    expect(find.text('Parolamı değiştir'), findsNothing);
    await tester.scrollUntilVisible(find.text('Parola belirle'), 200);
    await tester.tap(find.text('Parola belirle'));
    await tester.pumpAndSettle();

    // Kod pencere açılınca gider.
    expect(repository.passwordCodeRequests, ['request']);
    expect(find.byType(AppCodeInput), findsOneWidget);

    await tester.tap(find.widgetWithText(FilledButton, 'Parolayı belirle'));
    await tester.pumpAndSettle();
    expect(repository.passwordChangeCount, 0, reason: 'Kod yazılmadı.');

    await tester.enterText(_codeField(), '123456');
    await tester.enterText(
      find.widgetWithText(TextField, 'Yeni parola'),
      'Valid-Password-123!',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Parolayı belirle'));
    await tester.pumpAndSettle();

    expect(repository.lastResetCode, '123456');
    expect(repository.lastCurrentPassword, isNull);
  });

  testWidgets('posta gitmezse Parola belirle bunu söyler', (tester) async {
    final repository = FakeAccountRepository()
      ..hasPassword = false
      ..passwordCodeSent = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    await tester.scrollUntilVisible(find.text('Parola belirle'), 200);
    await tester.tap(find.text('Parola belirle'));
    await tester.pumpAndSettle();

    expect(
      find.text('Kod gönderilemedi: e-posta servisi yapılandırılmamış.'),
      findsOneWidget,
    );
    expect(find.text('Tekrar dene'), findsOneWidget);
  });

  testWidgets('Parolamı unuttum mevcut parola yerine e-posta kodunu sorar', (
    tester,
  ) async {
    final repository = FakeAccountRepository();
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    await tester.scrollUntilVisible(find.text('Parolamı değiştir'), 200);
    await tester.tap(find.text('Parolamı değiştir'));
    await tester.pumpAndSettle();
    expect(repository.passwordCodeRequests, isEmpty);

    await tester.tap(find.text('Parolamı unuttum'));
    await tester.pumpAndSettle();

    expect(repository.passwordCodeRequests, ['request']);
    expect(find.widgetWithText(TextField, 'Mevcut parola'), findsNothing);
    expect(find.byType(AppCodeInput), findsOneWidget);

    await tester.enterText(_codeField(), '654321');
    await tester.enterText(
      find.widgetWithText(TextField, 'Yeni parola'),
      'Valid-Password-123!',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Değiştir'));
    await tester.pumpAndSettle();

    expect(repository.lastResetCode, '654321');
    expect(repository.lastCurrentPassword, isNull);
  });

  testWidgets('yalnız parolası olan hesapta Google ile doğrulama yok', (
    tester,
  ) async {
    await _openDeletePage(tester, FakeAccountRepository());

    expect(find.text('Kimliğinizi doğrulayın'), findsOneWidget);
    expect(find.widgetWithText(TextField, 'Parolanız'), findsOneWidget);
    expect(find.text('Google ile doğrula'), findsNothing);
    expect(find.text('ya da'), findsNothing);
  });

  testWidgets('parolasız hesap Google ile doğrulanır, sonra silinir', (
    tester,
  ) async {
    final repository = FakeAccountRepository()..hasPassword = false;
    await _openDeletePage(tester, repository, google: _token('ayse@gmail.com'));

    expect(find.widgetWithText(TextField, 'Parolanız'), findsNothing);
    final submit = find.widgetWithText(FilledButton, 'Hesabımı sil');
    // Doğrulanmadan kırmızı düğme kapalı.
    expect(tester.widget<FilledButton>(submit).onPressed, isNull);

    await tester.tap(find.text('Google ile doğrula'));
    await tester.pumpAndSettle();

    // Google penceresi silmez; yalnız kimliği doğrular.
    expect(repository.deletedWithGoogle, isNull);
    expect(find.text('Google ile doğrulandı'), findsOneWidget);
    expect(find.text('ayse@gmail.com'), findsOneWidget);
    expect(tester.widget<FilledButton>(submit).onPressed, isNotNull);

    await tester.tap(submit);
    await tester.pumpAndSettle();

    expect(repository.deletedWithGoogle, _token('ayse@gmail.com'));
    expect(repository.deletedWithPassword, isNull);
    expect(find.text('Hesabınız silindi'), findsOneWidget);
  });

  testWidgets('Google penceresi kapatılırsa doğrulama olmaz', (tester) async {
    final repository = FakeAccountRepository()..hasPassword = false;
    await _openDeletePage(tester, repository, google: null);

    await tester.tap(find.text('Google ile doğrula'));
    await tester.pumpAndSettle();

    expect(find.text('Google ile doğrulandı'), findsNothing);
    final submit = find.widgetWithText(FilledButton, 'Hesabımı sil');
    expect(tester.widget<FilledButton>(submit).onPressed, isNull);
    expect(repository.deletedWithGoogle, isNull);
  });

  testWidgets('Google ve parolası olan hesap ikisinden biriyle doğrular', (
    tester,
  ) async {
    final repository = FakeAccountRepository()..hasGoogle = true;
    await _openDeletePage(tester, repository, google: _token('ayse@gmail.com'));

    expect(find.widgetWithText(TextField, 'Parolanız'), findsOneWidget);
    expect(find.text('ya da'), findsOneWidget);
    expect(find.text('Google ile doğrula'), findsOneWidget);

    await tester.tap(find.text('Google ile doğrula'));
    await tester.pumpAndSettle();
    // Doğrulandıktan sonra parola alanı çekilir; `Vazgeç` geri getirir.
    expect(find.widgetWithText(TextField, 'Parolanız'), findsNothing);
    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();
    expect(find.widgetWithText(TextField, 'Parolanız'), findsOneWidget);

    await tester.tap(find.text('Google ile doğrula'));
    await tester.pumpAndSettle();
    await tester.tap(find.widgetWithText(FilledButton, 'Hesabımı sil'));
    await tester.pumpAndSettle();

    expect(repository.deletedWithGoogle, isNotNull);
    expect(repository.deletedWithPassword, isNull);
  });

  testWidgets('başka bir Google hesabı seçildiyse doğrulama düşer', (
    tester,
  ) async {
    final repository = FakeAccountRepository()
      ..hasGoogle = true
      ..deleteError = const ApiException(
        statusCode: 401,
        code: 'account.reauthentication_failed',
        message: 'Onayı bu hesaba bağlı Google hesabıyla vermelisiniz.',
      );
    await _openDeletePage(
      tester,
      repository,
      google: _token('baska@gmail.com'),
    );

    await tester.tap(find.text('Google ile doğrula'));
    await tester.pumpAndSettle();
    await tester.tap(find.widgetWithText(FilledButton, 'Hesabımı sil'));
    await tester.pumpAndSettle();

    expect(
      find.text('Onayı bu hesaba bağlı Google hesabıyla vermelisiniz.'),
      findsOneWidget,
    );
    expect(find.text('Google ile doğrulandı'), findsNothing);
    expect(find.text('Hesabınız silindi'), findsNothing);
  });

  testWidgets('doğrulanmış adreste doğrulama satırı çıkmaz', (tester) async {
    await tester.pumpWidget(_app(FakeAccountRepository()));
    await tester.pumpAndSettle();

    expect(find.text('E-postanızı doğrulayın'), findsNothing);
  });

  testWidgets('doğrulanmamış adres satır taşır ama hesabı kilitlemez', (
    tester,
  ) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    expect(find.text('E-postanızı doğrulayın'), findsOneWidget);
    expect(find.byIcon(Icons.verified), findsNothing);

    // Sayfanın geri kalanı yerinde: satır bir engel değil.
    expect(find.text('Bu cihaz'), findsOneWidget);
    await tester.scrollUntilVisible(find.text('Parolamı değiştir'), 200);
    expect(find.text('Parolamı değiştir'), findsOneWidget);
  });

  testWidgets('doğrulama paneli açılınca kod isteği gider', (tester) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('E-postanızı doğrulayın'));
    await tester.pumpAndSettle();

    expect(repository.sendVerificationCount, 1);
    expect(find.byType(AppCodeInput), findsOneWidget);
    expect(
      find.text('user@example.test adresine gelen 6 haneli kodu yazın.'),
      findsOneWidget,
    );
    // Panelde tek eylem var; kapatmak tutamaçla ya da dışına dokunarak.
    expect(find.text('Vazgeç'), findsNothing);
  });

  testWidgets('yeniden gönderme süre dolunca açılır', (tester) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();
    await tester.tap(find.text('E-postanızı doğrulayın'));
    await tester.pump();
    await tester.pump(const Duration(seconds: 1));

    expect(find.textContaining('sonra isteyebilirsiniz'), findsOneWidget);
    expect(find.text('Kodu yeniden gönder'), findsNothing);

    await tester.pump(const Duration(seconds: 60));
    await tester.tap(find.text('Kodu yeniden gönder'));
    await tester.pump();

    expect(repository.sendVerificationCount, 2);
  });

  testWidgets('az önce gönderilmiş kod hata sayılmaz', (tester) async {
    final repository = FakeAccountRepository()
      ..emailConfirmed = false
      ..firstSendError = const ApiException(
        statusCode: 409,
        code: 'account.verification_code_too_soon',
        message: 'Az önce bir kod gönderildi.',
      );
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();
    await tester.tap(find.text('E-postanızı doğrulayın'));
    await tester.pump();
    await tester.pump(const Duration(seconds: 1));

    expect(find.text('Kod az önce gönderildi.'), findsOneWidget);
    expect(find.text('Kod gönderiliyor…'), findsNothing);
    expect(find.textContaining('biraz bekleyin'), findsNothing);
  });

  testWidgets('eksik kod istek üretmez', (tester) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();
    await tester.tap(find.text('E-postanızı doğrulayın'));
    await tester.pumpAndSettle();

    await tester.enterText(find.byType(TextField).last, '12');
    await tester.tap(find.widgetWithText(FilledButton, 'Doğrula'));
    await tester.pumpAndSettle();

    expect(repository.confirmedWithCode, isNull);
  });

  testWidgets('yanlış kod panelde söylenir', (tester) async {
    final repository = FakeAccountRepository()
      ..emailConfirmed = false
      ..confirmError = const ApiException(
        statusCode: 400,
        code: 'account.invalid_verification_code',
        message: 'Kod geçersiz veya süresi dolmuş.',
      );
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();
    await tester.tap(find.text('E-postanızı doğrulayın'));
    await tester.pumpAndSettle();

    // Son rakamla kod kendiliğinden denenir; düğmeye basmak gerekmez.
    await tester.enterText(find.byType(TextField).last, '000000');
    await tester.pumpAndSettle();

    expect(find.text('Kod yanlış ya da süresi dolmuş.'), findsOneWidget);
    expect(find.byType(AppCodeInput), findsOneWidget);
  });

  testWidgets('doğru kod uyarıyı kaldırır', (tester) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();
    await tester.tap(find.text('E-postanızı doğrulayın'));
    await tester.pumpAndSettle();

    await tester.enterText(find.byType(TextField).last, '123456');
    await tester.pumpAndSettle();

    expect(repository.confirmedWithCode, '123456');
    expect(find.text('E-posta adresiniz doğrulandı.'), findsOneWidget);
    expect(find.text('E-postanızı doğrulayın'), findsNothing);
  });
}

Widget _app(
  AccountRepositoryContract repository, {
  FakeAuthSessionRepository? authRepository,
  DateTime Function()? now,
}) => MaterialApp(
  theme: AppTheme.light(),
  home: AccountPage(
    repository: repository,
    now: now,
    authRepository:
        authRepository ??
        (FakeAuthSessionRepository()..session = testSession()),
  ),
);

class FakeAccountRepository implements AccountRepositoryContract {
  String? revokedSessionId;
  String? deletedWithPassword;
  int passwordChangeCount = 0;
  ApiException? passwordError;

  @override
  Future<UserAccount> read() async => UserAccount(
    userId: '11111111-1111-1111-1111-111111111111',
    email: 'user@example.test',
    emailConfirmed: emailConfirmed,
    createdAtUtc: DateTime.utc(2026, 8, 1, 9),
    activeSessionCount: 2,
    hasPassword: hasPassword,
    username: username,
    hasGoogle: hasGoogle,
  );

  bool hasPassword = true;
  bool hasGoogle = false;
  String? username;
  String? lastCurrentPassword;
  String? lastResetCode;

  /// Bu cihazın dışındaki açık oturum sayısı.
  int otherSessions = 1;
  final List<String> revokedSessionIds = [];

  @override
  Future<List<UserSessionSummary>> listSessions() async => [
    UserSessionSummary(
      // `testSession()` bu kimlikle geliyor: satır "bu cihaz" olmalı.
      sessionId: '33333333-3333-3333-3333-333333333333',
      createdAtUtc: DateTime.utc(2026, 8, 20, 9),
      expiresAtUtc: DateTime.utc(2026, 9, 19, 9),
    ),
    for (var i = 0; i < otherSessions; i++)
      UserSessionSummary(
        sessionId: i == 0 ? 'session-other' : 'session-other-$i',
        createdAtUtc: DateTime.utc(2026, 8, 10 - i, 9),
        expiresAtUtc: DateTime.utc(2026, 9, 9, 9),
      ),
  ].where((s) => !revokedSessionIds.contains(s.sessionId)).toList();

  @override
  Future<void> revokeSession(String sessionId) async {
    revokedSessionId = sessionId;
    revokedSessionIds.add(sessionId);
  }

  @override
  Future<RotatedTokens> changePassword({
    String? currentPassword,
    required String newPassword,
    String? resetCode,
  }) async {
    if (passwordError case final error?) {
      throw error;
    }
    lastCurrentPassword = currentPassword;
    lastResetCode = resetCode;
    passwordChangeCount++;
    return RotatedTokens(
      sessionId: 'session-rotated',
      accessToken: 'rotated-access',
      accessTokenExpiresAtUtc: DateTime.utc(2026, 8, 27, 10),
      refreshToken: 'rotated-refresh',
      refreshTokenExpiresAtUtc: DateTime.utc(2026, 9, 26, 10),
    );
  }

  @override
  Future<void> deleteAccount({String? password, String? googleIdToken}) async {
    if (deleteError case final error?) throw error;
    deletedWithPassword = password;
    deletedWithGoogle = googleIdToken;
  }

  String? deletedWithGoogle;
  ApiException? deleteError;
  List<String> passwordCodeRequests = [];

  bool passwordCodeSent = true;

  @override
  Future<bool> requestPasswordCode() async {
    passwordCodeRequests.add('request');
    return passwordCodeSent;
  }

  bool emailConfirmed = true;
  int sendVerificationCount = 0;
  String? confirmedWithCode;
  ApiException? confirmError;

  /// Yalnız ilk istekte atılır; yeniden gönderim başarılı olur.
  ApiException? firstSendError;

  @override
  Future<VerificationSendResult> sendEmailVerification() async {
    sendVerificationCount++;
    if (sendVerificationCount == 1 && firstSendError != null) {
      throw firstSendError!;
    }
    return const VerificationSendResult(
      alreadyConfirmed: false,
      codeSent: true,
    );
  }

  @override
  Future<void> confirmEmail(String code) async {
    if (confirmError case final error?) {
      throw error;
    }
    confirmedWithCode = code;
    emailConfirmed = true;
  }
}

/// Kod kutularının arkasındaki tek alan.
Finder _codeField() => find.descendant(
  of: find.byType(AppCodeInput),
  matching: find.byType(TextField),
);

/// Sahte Google belirteci: imzasız, yalnız adres taşır.
String _token(String email) {
  String part(Object value) =>
      base64Url.encode(utf8.encode(jsonEncode(value))).replaceAll('=', '');
  return '${part({'alg': 'none'})}.${part({'email': email})}.imza';
}

Future<void> _openDeletePage(
  WidgetTester tester,
  FakeAccountRepository repository, {
  String? google,
}) async {
  final auth = FakeAuthSessionRepository()..session = testSession();
  final controller = AuthController(auth, _FakeGoogle(google));
  await tester.pumpWidget(
    ChangeNotifierProvider<AuthController?>.value(
      value: controller,
      child: _app(repository, authRepository: auth),
    ),
  );
  await tester.pumpAndSettle();
  await tester.scrollUntilVisible(find.text('Hesabımı sil'), 200);
  await tester.ensureVisible(find.text('Hesabımı sil'));
  await tester.pumpAndSettle();
  await tester.tap(find.text('Hesabımı sil'));
  await tester.pumpAndSettle();
}

class _FakeGoogle implements GoogleIdTokenSource {
  _FakeGoogle(this.token);

  final String? token;

  @override
  Future<String?> obtainIdToken() async => token;

  @override
  Future<void> signOut() async {}
}
