import 'package:business_finance_mobile/features/auth/presentation/auth_validation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
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

  testWidgets('başka cihazın oturumu onaydan sonra kapanır', (tester) async {
    final repository = FakeAccountRepository();
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithIcon(IconButton, Icons.close));
    await tester.pumpAndSettle();
    expect(find.text('Oturum kapatılsın mı?'), findsOneWidget);

    await tester.tap(find.widgetWithText(FilledButton, 'Oturumu kapat'));
    await tester.pumpAndSettle();

    expect(repository.revokedSessionId, 'session-other');
    expect(find.text('Başka bir cihaz'), findsNothing);
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

  testWidgets('oturum satırı göreli zamanı ve geçerliliği söyler', (
    tester,
  ) async {
    await tester.pumpWidget(
      _app(FakeAccountRepository(), now: () => DateTime(2026, 8, 17, 12)),
    );
    await tester.pumpAndSettle();
    expect(
      find.text("7 gün önce açıldı · 9 Eylül'e kadar geçerli"),
      findsOneWidget,
    );
    expect(find.text("Şu an açık · 19 Eylül'e kadar geçerli"), findsOneWidget);
  });

  testWidgets('diğer oturumlar tek onayla kapanır; bu cihaz kalır', (
    tester,
  ) async {
    final repository = FakeAccountRepository();
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    expect(find.text('Açık oturumlar · 2'), findsOneWidget);
    await tester.tap(find.text('Diğerlerini kapat'));
    await tester.pumpAndSettle();
    expect(find.text('Diğer oturumlar kapatılsın mı?'), findsOneWidget);

    await tester.tap(find.widgetWithText(FilledButton, 'Hepsini kapat'));
    await tester.pumpAndSettle();

    expect(repository.revokedSessionId, 'session-other');
    expect(find.text('Başka bir cihaz'), findsNothing);
    expect(find.text('Bu cihaz'), findsOneWidget);
    expect(find.text('Diğerlerini kapat'), findsNothing);
  });

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
    expect(find.text(AuthValidation.passwordRule), findsOneWidget);
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

  testWidgets('hesap silme iki kapıdan geçer', (tester) async {
    final repository = FakeAccountRepository();
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    await tester.scrollUntilVisible(find.text('Hesabımı sil'), 200);
    await tester.ensureVisible(find.text('Hesabımı sil'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Hesabımı sil'));
    await tester.pumpAndSettle();

    // Birinci kapı: ne olacağını söyleyen onay.
    expect(find.text('Hesabınız silinsin mi?'), findsOneWidget);
    expect(repository.deletedWithPassword, isNull);
    await tester.tap(find.widgetWithText(FilledButton, 'Devam et'));
    await tester.pumpAndSettle();

    // İkinci kapı: parolanın yeniden yazılması. Boş parola istek üretmez.
    await tester.tap(find.widgetWithText(FilledButton, 'Hesabımı sil'));
    await tester.pumpAndSettle();
    expect(repository.deletedWithPassword, isNull);
    expect(find.text('Parolanızı yazın.'), findsOneWidget);

    await tester.enterText(
      find.widgetWithText(TextField, 'Parolanız'),
      'Valid-Password-123!',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Hesabımı sil'));
    await tester.pumpAndSettle();

    expect(repository.deletedWithPassword, 'Valid-Password-123!');
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
    expect(repository.passwordCodeRequests, ['user@example.test']);

    await tester.tap(find.widgetWithText(FilledButton, 'Parolayı belirle'));
    await tester.pumpAndSettle();
    expect(repository.passwordChangeCount, 0, reason: 'Kod yazılmadı.');

    await tester.enterText(find.widgetWithText(TextField, 'Kod'), '123456');
    await tester.enterText(
      find.widgetWithText(TextField, 'Yeni parola'),
      'Valid-Password-123!',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Parolayı belirle'));
    await tester.pumpAndSettle();

    expect(repository.lastResetCode, '123456');
    expect(repository.lastCurrentPassword, isNull);
  });

  testWidgets('parolasız hesap Google onayıyla silinir', (tester) async {
    final repository = FakeAccountRepository()..hasPassword = false;
    final auth = FakeAuthSessionRepository()..session = testSession();
    final google = _FakeGoogle('google-token');
    final controller = AuthController(auth, google);
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
    expect(find.textContaining('Google hesabınızı seçerek'), findsOneWidget);
    await tester.tap(find.widgetWithText(FilledButton, 'Devam et'));
    await tester.pumpAndSettle();

    // Parola alanı açılmaz; onay Google'dan gelir.
    expect(find.widgetWithText(TextField, 'Parolanız'), findsNothing);
    expect(repository.deletedWithGoogle, 'google-token');
    expect(repository.deletedWithPassword, isNull);
  });

  testWidgets('Google penceresi kapatılırsa hesap silinmez', (tester) async {
    final repository = FakeAccountRepository()..hasPassword = false;
    final auth = FakeAuthSessionRepository()..session = testSession();
    final controller = AuthController(auth, _FakeGoogle(null));
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
    await tester.tap(find.widgetWithText(FilledButton, 'Devam et'));
    await tester.pumpAndSettle();

    expect(repository.deletedWithGoogle, isNull);
  });

  testWidgets('doğrulanmış adreste uyarı çıkmaz', (tester) async {
    await tester.pumpWidget(_app(FakeAccountRepository()));
    await tester.pumpAndSettle();

    expect(
      find.textContaining('E-posta adresiniz doğrulanmadı.'),
      findsNothing,
    );
  });

  testWidgets('doğrulanmamış adres uyarı taşır ama hesabı kilitlemez', (
    tester,
  ) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    expect(
      find.textContaining('E-posta adresiniz doğrulanmadı.'),
      findsOneWidget,
    );

    // Sayfanın geri kalanı yerinde: uyarı bir engel değil. Kart araya
    // girdiği için aşağısı katlanmanın altında kalıyor, kaydırılıyor.
    expect(find.text('Bu cihaz'), findsOneWidget);
    await tester.scrollUntilVisible(find.text('Parolamı değiştir'), 200);
    expect(find.text('Parolamı değiştir'), findsOneWidget);
  });

  testWidgets('doğrulama paneli açılınca kod isteği gider', (tester) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Adresimi doğrula'));
    await tester.pumpAndSettle();

    expect(repository.sendVerificationCount, 1);
    expect(find.widgetWithText(TextField, 'Doğrulama kodu'), findsOneWidget);
  });

  testWidgets('eksik kod istek üretmez', (tester) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Adresimi doğrula'));
    await tester.pumpAndSettle();

    await tester.enterText(
      find.widgetWithText(TextField, 'Doğrulama kodu'),
      '12',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Doğrula'));
    await tester.pumpAndSettle();

    expect(repository.confirmedWithCode, isNull);
    expect(find.text('Altı haneli kodu yazın.'), findsOneWidget);
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
    await tester.tap(find.text('Adresimi doğrula'));
    await tester.pumpAndSettle();

    await tester.enterText(
      find.widgetWithText(TextField, 'Doğrulama kodu'),
      '000000',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Doğrula'));
    await tester.pumpAndSettle();

    expect(find.text('Kod geçersiz veya süresi dolmuş.'), findsOneWidget);
    expect(find.widgetWithText(TextField, 'Doğrulama kodu'), findsOneWidget);
  });

  testWidgets('doğru kod uyarıyı kaldırır', (tester) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Adresimi doğrula'));
    await tester.pumpAndSettle();

    await tester.enterText(
      find.widgetWithText(TextField, 'Doğrulama kodu'),
      '123456',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Doğrula'));
    await tester.pumpAndSettle();

    expect(repository.confirmedWithCode, '123456');
    expect(find.text('E-posta adresiniz doğrulandı.'), findsOneWidget);
    expect(
      find.textContaining('E-posta adresiniz doğrulanmadı.'),
      findsNothing,
    );
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
  );

  bool hasPassword = true;
  String? lastCurrentPassword;
  String? lastResetCode;

  @override
  Future<List<UserSessionSummary>> listSessions() async => [
    UserSessionSummary(
      // `testSession()` bu kimlikle geliyor: satır "bu cihaz" olmalı.
      sessionId: '33333333-3333-3333-3333-333333333333',
      createdAtUtc: DateTime.utc(2026, 8, 20, 9),
      expiresAtUtc: DateTime.utc(2026, 9, 19, 9),
    ),
    UserSessionSummary(
      sessionId: 'session-other',
      createdAtUtc: DateTime.utc(2026, 8, 10, 9),
      expiresAtUtc: DateTime.utc(2026, 9, 9, 9),
    ),
  ];

  @override
  Future<void> revokeSession(String sessionId) async {
    revokedSessionId = sessionId;
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
    deletedWithPassword = password;
    deletedWithGoogle = googleIdToken;
  }

  String? deletedWithGoogle;
  List<String> passwordCodeRequests = [];

  @override
  Future<void> requestPasswordCode(String email) async {
    passwordCodeRequests.add(email);
  }

  bool emailConfirmed = true;
  int sendVerificationCount = 0;
  String? confirmedWithCode;
  ApiException? confirmError;

  @override
  Future<VerificationSendResult> sendEmailVerification() async {
    sendVerificationCount++;
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

class _FakeGoogle implements GoogleIdTokenSource {
  _FakeGoogle(this.token);

  final String? token;

  @override
  Future<String?> obtainIdToken() async => token;

  @override
  Future<void> signOut() async {}
}
