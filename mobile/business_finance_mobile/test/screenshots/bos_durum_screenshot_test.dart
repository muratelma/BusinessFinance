import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/widgets/app_first_use_view.dart';
import 'package:business_finance_mobile/core/widgets/app_page_bar.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';

/// İlk kullanım boş durumunun kart ve halka çizimleri. Blok gerçek
/// bileşendir; sayfa başlığı ve kenar boşluğuyla kurulur. Kredi kartları'nın
/// gerçek sayfası `kredi_kartlarim_screenshot_test.dart` içindedir.
void main() {
  Widget page(String title, List<Widget> body) => Scaffold(
    appBar: AppPageBar(title: Text(title)),
    body: ListView(padding: AppPageInsets.list, children: body),
  );

  testWidgets('ilk kullanım · kart', (tester) async {
    await captureScreen(
      tester,
      'ilk-kullanim-kart',
      page('Kredi kartları', [
        AppFirstUseView(
          preview: AppEmptyPreview.card,
          icon: Icons.credit_card,
          title: 'Henüz kredi kartı yok',
          message: 'Limit ve ekstre günleriyle ilk kartınızı ekleyin.',
          actionLabel: 'Kart ekle',
          onAction: () {},
        ),
      ]),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);

  testWidgets('ilk kullanım · halka', (tester) async {
    await captureScreen(
      tester,
      'ilk-kullanim-halka',
      page('Bütçeler', [
        AppFirstUseView(
          preview: AppEmptyPreview.ring,
          icon: Icons.donut_small_outlined,
          title: 'Bu ay için bütçe yok',
          message: 'Bir gider kategorisine aylık limit belirleyin.',
          actionLabel: 'Bütçe ekle',
          onAction: () {},
        ),
      ]),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);
}
