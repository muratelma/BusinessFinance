import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/account/data/account_models.dart';
import 'package:business_finance_mobile/features/account/data/account_repository.dart';
import 'package:business_finance_mobile/features/account/presentation/account_page.dart';

import '../../helpers/fake_auth.dart';

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

    await tester.tap(find.widgetWithText(FilledButton, 'Kapat'));
    await tester.pumpAndSettle();

    expect(repository.revokedSessionId, 'session-other');
    expect(find.text('Başka bir cihaz'), findsNothing);
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
    expect(find.text('Parola en az 12 karakter olmalıdır.'), findsOneWidget);
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

  testWidgets('doğrulanmış adreste uyarı çıkmaz', (tester) async {
    await tester.pumpWidget(_app(FakeAccountRepository()));
    await tester.pumpAndSettle();

    expect(find.text('E-posta adresiniz doğrulanmadı'), findsNothing);
  });

  testWidgets('doğrulanmamış adres uyarı taşır ama hesabı kilitlemez', (
    tester,
  ) async {
    final repository = FakeAccountRepository()..emailConfirmed = false;
    await tester.pumpWidget(_app(repository));
    await tester.pumpAndSettle();

    expect(find.text('E-posta adresiniz doğrulanmadı'), findsOneWidget);

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
    expect(find.text('E-posta adresiniz doğrulanmadı'), findsNothing);
  });
}

Widget _app(
  AccountRepositoryContract repository, {
  FakeAuthSessionRepository? authRepository,
}) => MaterialApp(
  theme: AppTheme.light(),
  home: AccountPage(
    repository: repository,
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
  );

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
    required String currentPassword,
    required String newPassword,
  }) async {
    if (passwordError case final error?) {
      throw error;
    }
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
  Future<void> deleteAccount({required String password}) async {
    deletedWithPassword = password;
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
