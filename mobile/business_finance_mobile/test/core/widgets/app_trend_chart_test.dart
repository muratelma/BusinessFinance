import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_trend_chart.dart';

void main() {
  Future<void> pump(WidgetTester tester, List<AppTrendPoint> points) =>
      tester.pumpWidget(
        MaterialApp(
          theme: AppTheme.light(),
          home: Scaffold(
            body: Padding(
              padding: const EdgeInsets.all(16),
              child: AppTrendChart(points: points, currency: 'TRY'),
            ),
          ),
        ),
      );

  // Bu regresyon daha önce Özet ekranı üzerinden korunuyordu; Özet'te grafik
  // geçici olarak kapatılınca koruma da düşecekti. Hata bileşene ait olduğu
  // için testi de bileşene taşındı.
  testWidgets('çubuklar gerçekten çizilir, sıfır boyutta kalmaz', (
    tester,
  ) async {
    // `FractionallySizedBox` yalnız verilen eksende oran uygular; diğer
    // eksende çocuğunun boyutunu alır. Boyutsuz bir çocukla birlikte
    // kullanıldığında çubuk sıfır genişlikte çiziliyor ve grafik "var ama
    // boş" görünüyordu.
    await pump(tester, const [
      AppTrendPoint(label: '07', net: '1400.0000', hasData: true),
      AppTrendPoint(label: '08', net: '-600.0000', hasData: true),
    ]);

    final bars = find.descendant(
      of: find.byType(AppTrendChart),
      matching: find.byType(FractionallySizedBox),
    );
    expect(bars, findsWidgets);

    final size = tester.getSize(bars.first);
    expect(size.width, greaterThan(0));
    expect(size.height, greaterThan(0));
  });

  testWidgets('hareketi olmayan dönem çubuk çizmez', (tester) async {
    // Sıfır yüksekliğinde bir çubuk "sıfır net" ile "veri yok"u aynı
    // gösteriyordu.
    await pump(tester, const [
      AppTrendPoint(label: '07', net: '0.0000', hasData: false),
    ]);

    expect(
      find.descendant(
        of: find.byType(AppTrendChart),
        matching: find.byType(FractionallySizedBox),
      ),
      findsNothing,
    );
  });
}
