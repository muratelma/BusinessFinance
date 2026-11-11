import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_no_results_view.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('filtre cümlesi en çok iki adı yazar, fazlasını sayar', () {
    expect(
      AppNoResultsView.filterSentence(['Transferler']),
      'Filtre: Transferler',
    );
    expect(
      AppNoResultsView.filterSentence(['Transferler', 'Bu ay']),
      'Filtre: Transferler · Bu ay',
    );
    expect(
      AppNoResultsView.filterSentence([
        'Transferler',
        'Bu ay',
        'Gider',
        'Hesap',
      ]),
      'Filtre: Transferler · Bu ay +2',
    );
  });

  testWidgets('düğmesiz hâlde yalnız başlık çizilir', (tester) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: const Scaffold(
          body: AppNoResultsView(
            reason: AppNoResultsReason.period,
            title: 'Bu kartta kart hareketi yok',
          ),
        ),
      ),
    );

    expect(find.text('Bu kartta kart hareketi yok'), findsOneWidget);
    expect(find.byType(OutlinedButton), findsNothing);
  });

  testWidgets('en büyük yazıda ve dar ekranda taşmaz', (tester) async {
    tester.view.physicalSize = const Size(360, 640);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.light(),
        home: MediaQuery(
          data: const MediaQueryData(
            size: Size(360, 640),
            textScaler: TextScaler.linear(2),
          ),
          child: Scaffold(
            body: ListView(
              children: [
                AppNoResultsView(
                  reason: AppNoResultsReason.search,
                  title: '“çok uzun bir arama kelimesi yazıldı” için sonuç yok',
                  message: 'Yazımı kontrol edin ya da yeniden arayın.',
                  actionLabel: 'Aramayı temizle',
                  onAction: () {},
                ),
              ],
            ),
          ),
        ),
      ),
    );

    expect(tester.takeException(), isNull);
  });
}
