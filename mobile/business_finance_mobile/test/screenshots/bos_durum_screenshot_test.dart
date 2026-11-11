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

  // Kalan sayfaların ilk kullanım metinleri (sayfalardaki metnin aynısı).
  const pages = <(String, String, IconData, String, String, String, IconData)>[
    (
      'kategoriler',
      'Kategori yönetimi',
      Icons.category_outlined,
      'Henüz kategori yok',
      'Gelir ve giderlerinizi kategorilere ayırın.',
      'Kategori ekle',
      Icons.add,
    ),
    (
      'cari',
      'Cari hesap',
      Icons.people_outline,
      'Henüz karşı taraf yok',
      'Cari hareketinizin olduğu kişileri ekleyin.',
      'Karşı taraf ekle',
      Icons.person_add_alt,
    ),
    (
      'borc',
      'Borç ve alacaklar',
      Icons.handshake_outlined,
      'Henüz borç ya da alacak yok',
      'Aldığınız ya da verdiğiniz borcu ekleyin.',
      'Borç / alacak ekle',
      Icons.add,
    ),
    (
      'hedef',
      'Tasarruf hedefleri',
      Icons.savings_outlined,
      'Henüz tasarruf hedefi yok',
      'Biriktirmek istediğiniz hedef tutarı ekleyin.',
      'Tasarruf hedefi ekle',
      Icons.add,
    ),
    (
      'tekrarlayan',
      'Planlama ve raporlar',
      Icons.event_repeat,
      'Henüz tekrarlayan plan yok',
      'Kira, maaş gibi düzenli ödemelerinizi ekleyin.',
      'Tekrarlayan plan ekle',
      Icons.add,
    ),
    (
      'pos',
      "POS'larım",
      Icons.point_of_sale_outlined,
      'Henüz POS yok',
      "POS'unuzu ya da yemek kartınızı bir kez ekleyin.",
      'POS ekle',
      Icons.add,
    ),
    (
      'islemler',
      'İşlemler',
      Icons.receipt_long_outlined,
      'Henüz işlem yok',
      'İlk gelir ya da giderinizi ekleyin.',
      'İşlem ekle',
      Icons.add,
    ),
  ];

  for (final (name, bar, icon, title, message, action, actionIcon) in pages) {
    testWidgets('ilk kullanım · $name', (tester) async {
      await captureScreen(
        tester,
        'ilk-kullanim-$name',
        page(bar, [
          AppFirstUseView(
            icon: icon,
            title: title,
            message: message,
            actionLabel: action,
            actionIcon: actionIcon,
            onAction: () {},
          ),
        ]),
        withNavBar: false,
        pushed: true,
      );
    }, skip: !screenshotsEnabled);
  }

  testWidgets('ilk kullanım · halka · kopyala', (tester) async {
    await captureScreen(
      tester,
      'ilk-kullanim-halka-kopyala',
      page('Bütçeler', [
        AppFirstUseView(
          preview: AppEmptyPreview.ring,
          icon: Icons.donut_small_outlined,
          title: 'Bu ay için bütçe yok',
          message: 'Geçen ayın bütçelerini bu aya kopyalayın.',
          actionLabel: 'Geçen ayı kopyala',
          actionIcon: Icons.content_copy_outlined,
          onAction: () {},
        ),
      ]),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);

  // Kısa telefon: üst boşluk küçülür, üçüncü parça çizilmez.
  testWidgets('ilk kullanım · kısa telefon', (tester) async {
    await captureScreen(
      tester,
      'ilk-kullanim-kisa-telefon',
      page('İşlemler', [
        AppFirstUseView(
          icon: Icons.receipt_long_outlined,
          title: 'Henüz işlem yok',
          message: 'İlk gelir ya da giderinizi ekleyin.',
          actionLabel: 'İşlem ekle',
          onAction: () {},
        ),
      ]),
      selectedTab: 1,
      frame: const Size(360, 640),
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
