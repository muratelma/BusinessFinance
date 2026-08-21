import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Bir ekranın erişilebilirlik kapısını geçmesi: dokunma hedefi boyutu,
/// adlandırılmış hedef ve metin kontrastı.
///
/// Üç ölçüt de Flutter'ın kendi kılavuz matcher'larıdır; ayrı bir kural
/// yazılmadı çünkü asıl kaynak platform kılavuzudur.
Future<void> expectMeetsAccessibility(WidgetTester tester) async {
  final handle = tester.ensureSemantics();
  await expectLater(tester, meetsGuideline(androidTapTargetGuideline));
  await expectLater(tester, meetsGuideline(labeledTapTargetGuideline));
  await expectLater(tester, meetsGuideline(textContrastGuideline));
  handle.dispose();
}

/// Verilen kurulumu en büyük sistem yazı ölçeğinde çizer.
///
/// Android'in en büyük ayarı yaklaşık 2.0x'tir. Bu ölçekte taşan bir ekran
/// gerçek cihazda kırpılmış metin, kaybolmuş buton veya kırmızı taşma şeridi
/// gösterir; kullanıcı formu dolduramaz.
Future<void> pumpAtLargestTextScale(
  WidgetTester tester,
  Widget app, {
  double scale = 2,
  Size surfaceSize = const Size(400, 900),
}) async {
  tester.view.physicalSize = surfaceSize;
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);

  await tester.pumpWidget(
    MediaQuery(
      data: MediaQueryData(textScaler: TextScaler.linear(scale)),
      child: app,
    ),
  );
  await tester.pumpAndSettle();
}

/// En büyük yazı ölçeğinde çizim sırasında taşma olmadığını doğrular.
///
/// `RenderFlex overflowed` bir exception olarak raporlanır; testin sessizce
/// geçmemesi için açıkça tüketilip iddia edilir.
void expectNoOverflow(WidgetTester tester) {
  expect(
    tester.takeException(),
    isNull,
    reason:
        'En büyük yazı ölçeğinde yerleşim taştı. Sabit yükseklik veya '
        'kaydırılamayan bir sütun büyüyen metni sığdıramıyor.',
  );
}
