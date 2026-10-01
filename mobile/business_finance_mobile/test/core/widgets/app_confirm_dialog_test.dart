import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_confirm_dialog.dart';

void main() {
  Future<bool?> open(WidgetTester tester) async {
    bool? result;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () async {
                result = await AppConfirmDialog.show(
                  context: context,
                  title: 'Hareket iptal edilsin mi?',
                  message: 'Hareket geçmişte kalır ancak bakiyeye katılmaz.',
                  confirmLabel: 'İptal et',
                );
              },
              child: const Text('aç'),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();
    return result;
  }

  testWidgets('onay verilince true döner', (tester) async {
    await open(tester);
    await tester.tap(find.text('İptal et'));
    await tester.pumpAndSettle();

    expect(find.text('Hareket iptal edilsin mi?'), findsNothing);
  });

  testWidgets('vazgeçilince false döner ve eylem çalışmaz', (tester) async {
    var confirmed = true;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () async {
                confirmed = await AppConfirmDialog.show(
                  context: context,
                  title: 'Silinsin mi?',
                  message: 'Bu işlem geri alınamaz.',
                  confirmLabel: 'Sil',
                );
              },
              child: const Text('aç'),
            ),
          ),
        ),
      ),
    );

    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();

    expect(confirmed, isFalse);
  });

  testWidgets('barrier ile kapatmak onay sayılmaz', (tester) async {
    // Dışarı dokunup kapatmak bir karar değildir; null asla onaya dönüşmemeli.
    var confirmed = true;
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () async {
                confirmed = await AppConfirmDialog.show(
                  context: context,
                  title: 'Silinsin mi?',
                  message: 'Bu işlem geri alınamaz.',
                  confirmLabel: 'Sil',
                );
              },
              child: const Text('aç'),
            ),
          ),
        ),
      ),
    );

    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();
    await tester.tapAt(const Offset(10, 10));
    await tester.pumpAndSettle();

    expect(confirmed, isFalse);
  });

  testWidgets('mesaj ne olacağını söyler', (tester) async {
    await open(tester);

    expect(
      find.text('Hareket geçmişte kalır ancak bakiyeye katılmaz.'),
      findsOneWidget,
    );
  });

  /// Kararın konusu kendi zemininde durur.
  ///
  /// Önceki hâlde tutar ve ad, sonucu anlatan cümlenin içine gömülüyordu ve
  /// pencere ekranda bir metin belgesi gibi duruyordu.
  testWidgets('vurgu satırı kararın konusunu ayrı taşır', (tester) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () => AppConfirmDialog.show(
                context: context,
                title: 'Gerçekleştirilsin mi?',
                highlight: 'Ev kirası\n₺12.500,00',
                message: 'Bakiyeye girecek.',
                confirmLabel: 'Gerçekleştir',
              ),
              child: const Text('aç'),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();

    expect(find.text('Ev kirası\n₺12.500,00'), findsOneWidget);
    expect(find.text('Bakiyeye girecek.'), findsOneWidget);
  });

  /// Tasarım sistemindeki diyalogda ayrı başlık yoktur: büyük satır kararın
  /// konusudur, eylemin adı onay butonunda yazar.
  testWidgets('başlık verilmezse vurgu satırı büyük satırdır', (tester) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () => AppConfirmDialog.show(
                context: context,
                destructive: true,
                highlight: 'Motorlu taşıtlar · ₺2.180,00',
                message: 'Gider iptal edilir; kalem yeniden bekleyene döner.',
                confirmLabel: 'Ödemeyi geri al',
              ),
              child: const Text('aç'),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();

    final headline = tester.widget<Text>(
      find.text('Motorlu taşıtlar · ₺2.180,00'),
    );
    expect(headline.style?.fontSize, 22);
    // Eylemin adı yalnız butonda; ayrı bir başlık satırı yok.
    expect(find.text('Ödemeyi geri al'), findsOneWidget);
    expect(find.widgetWithText(FilledButton, 'Vazgeç'), findsOneWidget);
  });

  testWidgets('2.0× yazıda taşma olmaz', (tester) async {
    tester.view.physicalSize = const Size(360 * 3, 640 * 3);
    tester.view.devicePixelRatio = 3;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        builder: (context, child) => MediaQuery(
          data: MediaQuery.of(
            context,
          ).copyWith(textScaler: const TextScaler.linear(2)),
          child: child!,
        ),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () => AppConfirmDialog.show(
                context: context,
                destructive: true,
                highlight: 'Motorlu taşıtlar · ₺2.180,00',
                message:
                    'Gider iptal edilir; kapattığı 2 kalem yeniden bekler.',
                confirmLabel: 'Ödemeyi geri al',
              ),
              child: const Text('aç'),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
  });

  /// Kayıt kartı: ad ve tutar ayrı satırlardadır; uzun ad büyük satırı
  /// ikiye bölmez.
  testWidgets('kayıt kartı adı, ayrıntıyı ve tutarı ayrı taşır', (
    tester,
  ) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: Scaffold(
          body: Builder(
            builder: (context) => TextButton(
              onPressed: () => AppConfirmDialog.show(
                context: context,
                destructive: true,
                subject: const AppConfirmSubject(
                  title: 'SGK ve vergi ödemesi · Eylül dönemi',
                  detail: '26 Eylül · Ziraat vadesiz',
                  amount: '₺1.300,00',
                ),
                message: 'Gider iptal edilir ve toplamları artık etkilemez.',
                confirmLabel: 'Ödemeyi geri al',
              ),
              child: const Text('aç'),
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.text('aç'));
    await tester.pumpAndSettle();

    expect(find.text('SGK ve vergi ödemesi · Eylül dönemi'), findsOneWidget);
    expect(find.text('26 Eylül · Ziraat vadesiz'), findsOneWidget);
    expect(find.text('₺1.300,00'), findsOneWidget);
    // Kartla açılan pencerede ikon kutusu çizilmez.
    expect(find.byIcon(Icons.warning_amber_rounded), findsNothing);
    expect(tester.takeException(), isNull);
  });

  /// Yıkıcı kararın rengi hata rolünden gelir.
  ///
  /// Marka rengi akromatik olduğu için ekranda anlam taşıyan renk azdır; onay
  /// butonunu her pencerede kırmızıya boyamak o anlamı tüketirdi.
  testWidgets('yıkıcı karar hata renginde, ilerleten karar değil', (
    tester,
  ) async {
    Future<void> pumpDialog({required bool destructive}) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: Builder(
              builder: (context) => TextButton(
                onPressed: () => AppConfirmDialog.show(
                  context: context,
                  title: 'Başlık',
                  message: 'Mesaj.',
                  confirmLabel: 'Onay',
                  destructive: destructive,
                ),
                child: const Text('aç'),
              ),
            ),
          ),
        ),
      );
      await tester.tap(find.text('aç'));
      await tester.pumpAndSettle();
    }

    await pumpDialog(destructive: true);
    final scheme = Theme.of(tester.element(find.byType(Dialog))).colorScheme;
    var button = tester.widget<FilledButton>(
      find.widgetWithText(FilledButton, 'Onay'),
    );
    expect(
      button.style?.backgroundColor?.resolve(const <WidgetState>{}),
      scheme.error,
    );

    // İlk pencere kapatılmadan ikincisi açılamıyor: barrier dokunuşu yutuyor.
    await tester.tap(find.text('Vazgeç'));
    await tester.pumpAndSettle();

    await pumpDialog(destructive: false);
    button = tester.widget<FilledButton>(
      find.widgetWithText(FilledButton, 'Onay'),
    );
    expect(button.style?.backgroundColor, isNull);
  });
}
