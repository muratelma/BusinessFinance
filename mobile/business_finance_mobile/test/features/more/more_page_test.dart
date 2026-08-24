import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/widgets/app_list_row.dart';
import 'package:business_finance_mobile/features/more/presentation/more_page.dart';

import '../../helpers/accessibility.dart';

void main() {
  /// Menü satırlarının sırası ve adları kasıtlı.
  ///
  /// Önceki hâlde tek bir satır "İçe aktarma, borç, hedef ve yedek" diyordu:
  /// dört alakasız şeyi sayan bir başlık, gruplamanın yanlış olduğunun kendi
  /// itirafıydı. Sıra da frekansa değil ne yaptığınıza göre: ilk ikisi bakmak
  /// için, sonraki üçü kurmak için.
  const expectedOrder = [
    'Hesaplar ve transferler',
    'Kredi kartları',
    'Kategoriler',
    // Cari hesap ile taksitli sözleşme kardeş kapılar ve yan yana duruyorlar:
    // biri yürüyen bir hesap, diğeri vadesi belli bir plan.
    'Cari hesap',
    'Yükümlülükler',
    'Borç ve alacaklar',
    'Tasarruf hedefleri',
    'Planlama ve raporlar',
    'Veri ve yedek',
  ];

  testWidgets('menü dokuz kutuyu kararlaştırılan sırayla gösterir', (
    tester,
  ) async {
    await tester.pumpWidget(_host());
    await tester.pumpAndSettle();

    final titles = tester
        .widgetList<AppListRow>(find.byType(AppListRow))
        .map((row) => row.title)
        .where((title) => title != 'Çıkış yap')
        .toList(growable: false);

    expect(titles, expectedOrder);

    // Eski birleşik satır geri gelmemeli.
    expect(find.textContaining('İçe aktarma, borç'), findsNothing);
  });

  /// Kapıların hepsi tek kart, aralarında ayrıcı.
  ///
  /// Satır başına ayrı kart iki yönden de yanlıştı: yapışık hâlde yan yana iki
  /// kenarlık kalın bir çizgi gibi görünüyor, boşluklu hâlde ise birbiriyle
  /// ilgili yedi kapı yedi ayrı kutu gibi okunuyordu.
  testWidgets('menü tek kart içinde, ayrıcılarla bölünmüş', (tester) async {
    // Uzun bir ekran: sekizinci kapı geldiğinde menü varsayılan test
    // penceresine sığmaz oldu ve çıkış kartı hiç kurulmuyordu. Ölçülen şey
    // yapı, pencerenin boyu değil.
    tester.view.physicalSize = const Size(1080, 3200);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(_host());
    await tester.pumpAndSettle();

    // Menü kapıları için bir kart, çıkış için bir kart.
    expect(find.byType(AppCard), findsNWidgets(2));
    // Satırlar arasında birer ayrıcı, artı çıkış kartını ayıran bir tane.
    expect(find.byType(Divider), findsNWidgets(expectedOrder.length - 1 + 1));
  });

  testWidgets('menü erişilebilirlik kapısını geçer', (tester) async {
    await pumpAtLargestTextScale(tester, _host());

    expectNoOverflow(tester);
    await expectMeetsAccessibility(tester);
  });
}

Widget _host() => MaterialApp.router(
  theme: AppTheme.light(),
  routerConfig: GoRouter(
    routes: [
      GoRoute(path: '/', builder: (_, _) => const MorePage()),
      // Menüdeki her satırın gerçekten bir hedefi olduğunu doğrulamak için
      // hepsi tanımlı; eksik bir rota testte değil kullanıcıda patlardı.
      for (final path in [
        '/more/accounts',
        '/more/cards',
        '/more/categories',
        '/more/counterparties',
        '/more/obligations',
        '/more/debts',
        '/more/goals',
        '/more/planning',
        '/more/data-tools',
      ])
        GoRoute(path: path, builder: (_, _) => const SizedBox.shrink()),
    ],
  ),
);
