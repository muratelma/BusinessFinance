import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/widgets/app_card.dart';
import 'package:business_finance_mobile/core/theme/app_finance_icons.dart';
import 'package:business_finance_mobile/core/theme/app_finance_colors.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_donut_chart.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';
import 'package:business_finance_mobile/features/activities/data/activity_repository.dart';
import 'package:business_finance_mobile/features/activities/data/planned_activity_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_models.dart';
import 'package:business_finance_mobile/features/dashboard/data/dashboard_repository.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_page.dart';
import 'package:business_finance_mobile/features/dashboard/presentation/dashboard_view_model.dart';
import 'package:business_finance_mobile/features/planning/data/planning_models.dart';
import 'package:provider/provider.dart';

import '../../helpers/accessibility.dart';
import 'package:business_finance_mobile/core/models/transaction_scope.dart';

void main() {
  group('hesap ve kategori ikonları', () {
    test('nakit ile banka hesabı farklı ikon alır', () {
      final cash = AppFinanceIcons.forAccountType('cash');
      final bank = AppFinanceIcons.forAccountType('bank');

      expect(cash, isNot(bank));
    });

    test('bilinmeyen hesap türü satırı ikonsuz bırakmaz', () {
      // Sunucu ileride yeni bir tür eklerse ekran çökmemeli.
      expect(AppFinanceIcons.forAccountType('crypto'), isNotNull);
      expect(AppFinanceIcons.forAccountType(null), isNotNull);
    });

    test('varsayılan kategoriler birbirinden ayırt edilir', () {
      final icons = {
        for (final name in [
          'salary',
          'groceries',
          'housing',
          'transport',
          'bills',
          'health',
          'entertainment',
        ])
          name: AppFinanceIcons.forCategory(name),
      };

      // Sekiz kategori tek bir pasta dilimi ikonunu paylaşırsa göz satır
      // ayırt edemez; ikon ancak ayırt ediyorsa bilgi taşır.
      expect(icons.values.toSet(), hasLength(icons.length));
    });

    test('eşleme kanonik ad üzerinden yapılır, çeviri üzerinden değil', () {
      // Etiket çevirisi değişince ikonun düşmemesi gerekir.
      expect(
        AppFinanceIcons.forCategory('groceries', displayName: 'Kahvaltılık'),
        AppFinanceIcons.forCategory('groceries'),
      );
    });

    test('kullanıcının kendi kategorisi anahtar kelimeden ikon alır', () {
      final bill = AppFinanceIcons.forCategory('Elektrik faturası');
      final fallback = AppFinanceIcons.forCategory('Zzz bilinmeyen');

      expect(bill, isNot(fallback));
      expect(fallback, isNotNull);
    });
  });

  group('özet bölümleri', () {
    testWidgets('gelişmiş rapor gelince varlık ve bütçe bölümleri açılır', (
      tester,
    ) async {
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      expect(find.text('Varlık durumu'), findsOneWidget);
      expect(find.text('Net varlık'), findsOneWidget);
      expect(find.text('Bütçe durumu'), findsOneWidget);
      // `Bu ay nasıl bölündü` ve `Son 6 ay` geçici olarak kapalı: iki grafik
      // de biçim kararını bekliyor. Kapalı olmaları da bir karardır ve
      // sessizce geri açılmamalı.
      expect(find.text('Bu ay nasıl bölündü'), findsNothing);
      expect(find.text('Son 6 ay'), findsNothing);
    });

    testWidgets('yaklaşanlar bölümü en yakın üç kalemi vadesiyle yazar', (
      tester,
    ) async {
      // Aynı istek gecikmişler için zaten yapılıyordu; vadesi gelmemiş
      // kayıtlar süzülüp atılıyordu. Ekranın "önümde ne var" sorusunu
      // yanıtlayan yarısı indirilip çöpe atılan bu listeydi.
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        activityRepository: _PlannedSource([
          _planned(id: 'a', timing: 'upcoming', dueDate: '2026-08-14'),
          _planned(id: 'b', timing: 'today', dueDate: '2026-08-09'),
          _planned(id: 'c', timing: 'upcoming', dueDate: '2026-08-12'),
          _planned(id: 'd', timing: 'upcoming', dueDate: '2026-08-15'),
        ]),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      expect(find.text('Yaklaşanlar'), findsOneWidget);
      expect(find.text('7 gün'), findsOneWidget);
      // Yakın uçtan uzağa sıralı ve üçle sınırlı; dördüncü satır sayıya iner.
      expect(viewModel.upcoming.map((item) => item.plannedActivityId), [
        'b',
        'c',
        'a',
      ]);
      expect(find.text('1 kalem daha'), findsOneWidget);
    });

    testWidgets('yaklaşan yokken bölüm gizlenmez, iyi haberi yazar', (
      tester,
    ) async {
      // Gecikmiş bandı bir istisnadır, yokken çizilmemesi doğru. Yaklaşanlar
      // ise sabit bir bölüm: gizlenirse sayfa düzeni her hafta değişir ve
      // kullanıcı böyle bir bölümün varlığını hiç öğrenemez. "Ödeme yok"
      // kendi başına iyi haberdir; boşluk aynı şeyi söylemez.
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        activityRepository: _PlannedSource([
          _planned(id: 'a', timing: 'overdue', dueDate: '2026-07-15'),
        ]),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      expect(find.text('Yaklaşanlar'), findsOneWidget);
      expect(find.text('Bu hafta ödeme yok.'), findsOneWidget);
    });

    testWidgets('yaklaşan tutarı gider kırmızısında yazılır', (tester) async {
      // Bölümün bütün söylediği şey bunların ödenecek olması; rengi kısmak
      // uyarıyı kısar. Bir ara nötr bırakılmıştı, geri alındı.
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        activityRepository: _PlannedSource([
          _planned(
            id: 'a',
            timing: 'upcoming',
            dueDate: '2026-08-12',
            amount: '777.0000',
          ),
        ]),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      final expense = AppTheme.light().extension<AppFinanceColors>()!.expense;
      final amount = tester.widget<Text>(find.text('₺777,00'));
      expect(amount.style?.color, expense);
    });

    testWidgets('varlık kartı net varlığın dört terimini de gösterir', (
      tester,
    ) async {
      // Sunucu net varlığı `likit − kart borcu + alacak − borç` ile
      // hesaplıyor. Kart uzun süre ilk ikisini gösteriyordu; açık borcu ya da
      // alacağı olan kullanıcıda alttaki döküm üstteki toplamı açıklamıyordu.
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      expect(find.text('₺2.370,00'), findsOneWidget); // net varlık
      expect(find.text('₺2.400,00'), findsOneWidget); // likit varlık
      expect(find.text('₺250,00'), findsOneWidget); // kart borcu
      expect(find.text('Alacak'), findsOneWidget);
      expect(find.text('₺340,00'), findsOneWidget);
      expect(find.text('Borç'), findsOneWidget);
      expect(find.text('₺120,00'), findsOneWidget);
    });

    testWidgets('alacak ve borç sıfırken satırları çizilmez', (tester) async {
      // Borcu olmayan kullanıcı için kart eskisi gibi kalmalı; sıfır yazan
      // iki satır boş yer kaplar ve olmayan bir yükümlülüğü varmış gibi
      // gösterir.
      final viewModel = DashboardViewModel(
        _Source(
          _report(),
          advanced: _advanced(receivableDebt: '0.0000', payableDebt: '0.0000'),
        ),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      expect(find.text('Likit varlık'), findsOneWidget);
      expect(find.text('Alacak'), findsNothing);
      expect(find.text('Borç'), findsNothing);
    });

    testWidgets('yoldaki para likit varlıktan ayrı bir satırda duruyor', (
      tester,
    ) async {
      // POS'tan geçen para kullanıcının parasıdır ama bugün harcanamaz
      // (ADR 0015). Likit varlığa katılsaydı kullanıcı bankaya ulaşmamış
      // parayı harcanabilir sanırdı; hiç gösterilmeseydi net varlık likit
      // varlıktan büyük çıkar ve aradaki fark açıklamasız kalırdı.
      final viewModel = DashboardViewModel(
        _Source(
          _report(),
          advanced: _advanced(
            receivableDebt: '0.0000',
            payableDebt: '0.0000',
            moneyInTransit: '490.0000',
            netWorth: '2640.0000',
          ),
        ),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      expect(find.text('Yolda'), findsOneWidget);
      expect(find.text('POS tahsilatı'), findsOneWidget);
      // Kartın kelimesi ADR 0015'in kelimesidir: `bloke` bankacılık jargonu.
      expect(find.textContaining('Bloke'), findsNothing);
    });

    testWidgets('yolda para yokken satır hiç çizilmiyor', (tester) async {
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      expect(find.text('Likit varlık'), findsOneWidget);
      expect(find.text('Yolda'), findsNothing);
    });

    test('net varlık yoldaki parayı içerir, likit varlık içermez', () {
      // İki sayı aynı soruya cevap vermiyor: likit varlık "bugün ne
      // harcayabilirim", net varlık "neyim var". Farkları tam olarak yoldaki
      // tutar kadar olmalı — bu kapı sözleşmenin o vaadini tutuyor.
      final report = _advanced(
        receivableDebt: '0.0000',
        payableDebt: '0.0000',
        moneyInTransit: '490.0000',
        netWorth: '2640.0000',
      );
      double value(String amount) => double.parse(amount);

      expect(
        value(report.netWorth) - value(report.liquidAssets),
        value(report.moneyInTransit) - value(report.creditCardDebt),
      );
      expect(
        value(report.liquidAssets) +
            value(report.moneyInTransit) -
            value(report.creditCardDebt) +
            value(report.receivableDebt) -
            value(report.payableDebt),
        value(report.netWorth),
      );
    });

    test('net varlık, kartta gösterilen dört kalemin toplamıdır', () {
      // Toplam sunucuda hesaplanıyor, istemcide değil. Ama ikisi birbirini
      // tutmak **zorunda**: kart dört kalemi gösterip beşinci bir şey
      // saklıyorsa kullanıcı toplamı doğrulayamaz ve "faiz burada mı" gibi
      // sorular doğar. Bu kapı sözleşmenin o vaadini tutuyor.
      final report = _advanced();
      double value(String amount) => double.parse(amount);

      expect(
        value(report.liquidAssets) -
            value(report.creditCardDebt) +
            value(report.receivableDebt) -
            value(report.payableDebt),
        value(report.netWorth),
      );
    });

    testWidgets('varlık kartında renk yönü söyler', (tester) async {
      // Kart borcu kırmızı: birikmiş `CreditCardCharge`'lardır ve her biri
      // gider olarak yazılmıştır, yani kırmızı burada gerçekten gideri
      // gösterir. Borç ve alacak ise **simetrik** ve ikisi de mavi: taksit
      // ödemesi gider yazmaz, tahsilat gelir yazmaz — ikisi de saf bakiye
      // hareketidir.
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      final colors = AppTheme.light().extension<AppFinanceColors>()!;
      expect(
        tester.widget<Text>(find.text('₺250,00')).style?.color, // kart borcu
        colors.expense,
      );
      expect(
        tester.widget<Text>(find.text('₺120,00')).style?.color, // borç
        colors.neutral,
      );
      expect(
        tester.widget<Text>(find.text('₺340,00')).style?.color, // alacak
        colors.neutral,
      );
      // Sayının faizi içerip içermediği ekrandan okunabilmeli.
      expect(find.text('faiz hariç'), findsNWidgets(2));
    });

    testWidgets('alacak gelir yeşiline boyanmaz', (tester) async {
      // Alacak gelir değil, bir varlık kalemidir. Yeşile boyanırsa hem
      // "yeşil yalnız gelirdir" kuralı kırılır hem de tahsil edilmemiş bir
      // para kazanılmış gibi okunur. Likit varlık gibi nötr kalmalı.
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      final income = AppTheme.light().extension<AppFinanceColors>()!.income;
      final receivable = tester.widget<Text>(find.text('₺340,00'));
      expect(receivable.style?.color, isNot(income));
    });

    testWidgets('kategori efsanesinde tutar tek satırda kalır', (tester) async {
      // Gerçek hata buydu: tutar `Flexible` içindeydi, yani genişliği satırın
      // boş alanının yarısıyla sınırlıydı, ve `titleSmall`'ın kalın (w600)
      // rakamları o kutuya sığmayınca **alt satıra sarıyordu**. Halkanın
      // yanındaki efsane sütunu zaten dar; orada birkaç piksel fark satırı
      // ikiye bölmeye yetiyor.
      //
      // Kapı tutarın yüksekliğini aynı satırdaki kategori adıyla
      // karşılaştırıyor: ikisi de tek satırsa yükseklikleri aynı kademede
      // kalır, tutar sarsa iki katına çıkar. Kutunun sağ kenarına bakmak bu
      // hatayı **yakalamıyor** — sarma sırasında kutu aynı yerde durur.
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      // `Faturalar` yalnız efsanede var; `Market Alışverişi` bütçe
      // bölümünde de geçtiği için ölçüm oraya kaymasın.
      final amountHeight = tester.getSize(find.text('₺100,00')).height;
      final nameHeight = tester.getSize(find.text('Faturalar')).height;

      expect(
        amountHeight,
        lessThan(nameHeight * 1.5),
        reason:
            'Tutar $amountHeight px, kategori adı $nameHeight px: '
            'tutar alt satıra sarmış.',
      );
    });

    testWidgets('bütçe durumu aşım tutarını ve aşmayanların sayısını yazar', (
      tester,
    ) async {
      // Önceden yalnız aşan kategorinin adı yazılıyordu: 5 TL ile 500 TL
      // aşım aynı görünüyor, aşmayan bütçeler ise ekrandan tamamen
      // kayboluyordu. İkisi de bu kapının konusu.
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      expect(find.text('1 bütçe limiti aştı'), findsOneWidget);
      expect(find.text('-₺50,00'), findsOneWidget);
      expect(find.text('1 bütçe limit içinde'), findsOneWidget);
    });

    testWidgets('ekran önce bu ayı, sonra şu anki durumu gösterir', (
      tester,
    ) async {
      // Sıra bir tercih değil, ekranın iddiası: akış bölümleri (kategori,
      // bütçe) durum bölümlerinden (varlık, hesap) önce gelir. Önceden
      // varlık durumu ikisinin arasındaydı ve okuma akıştan duruma, oradan
      // tekrar akışa atlıyordu.
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      double topOf(String title) => tester.getTopLeft(find.text(title)).dy;

      expect(topOf('Kategori giderleri'), lessThan(topOf('Bütçe durumu')));
      expect(topOf('Bütçe durumu'), lessThan(topOf('Varlık durumu')));
      expect(topOf('Varlık durumu'), lessThan(topOf('Hesap bakiyeleri')));
    });

    testWidgets('gelişmiş rapor düşerse ay özeti yine gösterilir', (
      tester,
    ) async {
      // İkincil okumanın hatası kullanıcıyı bütün ekrandan etmemeli.
      final viewModel = DashboardViewModel(
        _Source(_report()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel);

      expect(find.text('Bu ayın neti'), findsOneWidget);
      expect(find.text('Varlık durumu'), findsNothing);
      expect(viewModel.error, isNull);
    });

    testWidgets('kategori dağılımı halka ve ikonlu efsaneyle çizilir', (
      tester,
    ) async {
      // Önce her kategori bir pay çubuğuydu. Çubuk oranı doğru okutur ama
      // "bu ayın parası nasıl bölündü" sorusunu tek bakışta yanıtlamaz.
      final viewModel = DashboardViewModel(
        _Source(_report()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();
      await _pump(tester, viewModel);

      expect(find.byType(AppDonutChart), findsOneWidget);
      // Efsane satırları: ad ve tutar okunur, renk tek başına anlam taşımaz.
      expect(find.text('Market Alışverişi'), findsOneWidget);
      expect(find.text('Faturalar'), findsOneWidget);
      expect(find.text('Diğer'), findsOneWidget);
      expect(find.text('₺300,00'), findsOneWidget);
      expect(find.text('₺50,00'), findsOneWidget);
    });

    testWidgets('"Diğer" satırı kategori ikonu taşımaz', (tester) async {
      // O satır bir kategori değil; kategori ikonu vermek onu adı olan bir
      // kategori gibi gösterirdi.
      final viewModel = DashboardViewModel(
        _Source(_report()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();
      await _pump(tester, viewModel);

      expect(find.byIcon(Icons.more_horiz), findsOneWidget);
    });

    testWidgets('hesap satırı türünü yazıyla da söyler', (tester) async {
      final viewModel = DashboardViewModel(
        _Source(_report()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel);

      expect(find.text('Nakit'), findsOneWidget);
      expect(find.text('Banka hesabı'), findsOneWidget);
    });

    testWidgets('hero kart önceki dönemle karşılaştırır', (tester) async {
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel, height: 3000);

      expect(find.textContaining('Geçen aya göre'), findsOneWidget);
    });

    testWidgets('zenginleştirilmiş özet erişilebilirlik kapısını geçer', (
      tester,
    ) async {
      final viewModel = DashboardViewModel(
        _Source(_report(), advanced: _advanced()),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await pumpAtLargestTextScale(
        tester,
        MaterialApp(
          theme: AppTheme.light(),
          home: ChangeNotifierProvider.value(
            value: viewModel,
            child: const DashboardPage(),
          ),
        ),
        surfaceSize: const Size(400, 6000),
      );

      expectNoOverflow(tester);
      await expectMeetsAccessibility(tester);
    });
  });

  group('gecikmiş ödeme uyarısı', () {
    test('yalnız vadesi geçmiş yükümlülükler sayılır', () {
      // Tekrarlanan gelir ve tahsil edilecek alacak da planlanan hareketlerdir
      // ama borç değildir; uyarı bandına girmemeleri gerekir. Ayrımı sunucu
      // `isPaymentObligation` ile bildiriyor.
      final viewModel = DashboardViewModel(
        _Source(_report()),
        activityRepository: _PlannedSource([
          _planned(
            id: 'gecikmis-fatura',
            timing: 'overdue',
            kind: 'payable-obligation',
            action: 'pay-obligation',
          ),
          _planned(id: 'bugun', timing: 'today'),
          _planned(id: 'yaklasan', timing: 'upcoming'),
          _planned(
            id: 'gecikmis-alacak',
            timing: 'overdue',
            isPaymentObligation: false,
          ),
        ]),
        now: () => DateTime(2026, 8, 9),
      );

      return viewModel.load().then((_) {
        expect(viewModel.overdue.map((item) => item.plannedActivityId), [
          'gecikmis-fatura',
        ]);
      });
    });

    test('planlanan okuma düşerse ay özeti ayakta kalır', () async {
      // Uyarı ikincil bir okuma; hatası kullanıcıyı bütün ekrandan etmemeli.
      final viewModel = DashboardViewModel(
        _Source(_report()),
        activityRepository: _PlannedSource.failing(),
        now: () => DateTime(2026, 8, 9),
      );

      await viewModel.load();

      expect(viewModel.overdue, isEmpty);
      expect(viewModel.report, isNotNull);
      expect(viewModel.error, isNull);
    });

    testWidgets('bant sayıyı ve en eski vadeyi gösterir', (tester) async {
      final viewModel = DashboardViewModel(
        _Source(_report()),
        activityRepository: _PlannedSource([
          _planned(id: 'a', timing: 'overdue', dueDate: '2026-07-15'),
          _planned(id: 'b', timing: 'overdue', dueDate: '2026-08-01'),
        ]),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel);

      expect(find.text('2 gecikmiş ödeme'), findsOneWidget);
      // Tutar toplamı yok: istemci parayı ikinci kez hesaplamaz.
      // Vade ve gerekçe **ayrı** satırlar; tek cümle olsalardı sarma noktası
      // kabın genişliğine düşer ve son kelime yalnız başına alt satıra
      // kalırdı (telefonda `geçmez` böyle yetim kalıyordu). Tam eşleşme
      // aranıyor: biri ikisini tekrar tek `Text`te birleştirirse düşer.
      expect(find.text('En eskisi 15 Temmuz'), findsOneWidget);
      // Ayrı bir buton yok: bandın kendisi tek dokunma hedefi. Pembe zeminin
      // üstündeki gri `FilledButton.tonal` ikinci bir renkli yüzey açıyor,
      // bandı hem yükseltiyor hem alacalı gösteriyordu.
      expect(find.byType(FilledButton), findsNothing);
      // Chevron banda ait olanla sınırlanıyor: ay seçicinin ileri oku da aynı
      // ikonu kullanıyor.
      final band = find.ancestor(
        of: find.text('2 gecikmiş ödeme'),
        matching: find.byType(AppCard),
      );
      expect(
        find.descendant(of: band, matching: find.byIcon(Icons.chevron_right)),
        findsOneWidget,
      );
      // Rol ekran okuyucuya bildirilmeli: görünürde buton olmayan bir hedef,
      // rolü söylenmezse TalkBack kullanıcısı için hiç yoktur.
      expect(tester.getSemantics(band).flagsCollection.isButton, isTrue);
      // Semantics'te "buton" demek ama eylem bağlamamak sessiz bir yalan
      // olurdu: chevron çizilir, ekran okuyucu düğme der, dokunma hiçbir şey
      // yapmaz. Router bu koşumda kurulu olmadığı için dokunma denenemiyor;
      // eylemin bağlı olduğu burada doğrulanıyor.
      expect(tester.widget<AppCard>(band).onTap, isNotNull);
      // Bandın var olma sebebi: onaylanmayan hareket hiçbir yerde görünmüyor.
      // Bu cümle "Onay bekliyor"a kısaltılırsa sonuç kaybolur.
      expect(find.text('Onaylanana kadar kayda geçmez'), findsOneWidget);
    });

    testWidgets('gecikme yokken bant hiç çizilmez', (tester) async {
      final viewModel = DashboardViewModel(
        _Source(_report()),
        activityRepository: _PlannedSource([
          _planned(id: 'a', timing: 'upcoming'),
        ]),
        now: () => DateTime(2026, 8, 9),
      );
      await viewModel.load();

      await _pump(tester, viewModel);

      expect(find.textContaining('gecikmiş ödeme'), findsNothing);
    });
  });
}

Future<void> _pump(
  WidgetTester tester,
  DashboardViewModel viewModel, {
  double height = 2000,
}) async {
  tester.view.physicalSize = Size(400, height);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);

  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: ChangeNotifierProvider.value(
        value: viewModel,
        child: const DashboardPage(),
      ),
    ),
  );
  await tester.pumpAndSettle();
}

DashboardReport _report() => DashboardReport.fromJson({
  'year': 2026,
  'month': 8,
  'totalIncome': '2000.0000',
  'totalExpense': '400.0000',
  'net': '1600.0000',
  'currency': 'TRY',
  'categoryExpenseSlices': [
    {
      'categoryId': '11111111-1111-1111-1111-111111111111',
      'categoryName': 'groceries',
      'amount': '300.0000',
    },
    {
      'categoryId': '11111111-1111-1111-1111-111111111112',
      'categoryName': 'bills',
      'amount': '100.0000',
    },
    // "Diğer" satırının kategori kimliği yoktur: bir kategori değil, birden
    // çoğunun toplamıdır.
    {'categoryId': null, 'categoryName': 'Diğer', 'amount': '50.0000'},
  ],
  'categoryExpenses': [
    {
      'categoryId': '11111111-1111-1111-1111-111111111111',
      'categoryName': 'groceries',
      'amount': '300.0000',
    },
    {
      'categoryId': '11111111-1111-1111-1111-111111111112',
      'categoryName': 'bills',
      'amount': '100.0000',
    },
  ],
  'accountBalances': [
    {
      'accountId': '22222222-2222-2222-2222-222222222222',
      'accountName': 'Cüzdan',
      'balance': '500.0000',
      'type': 'cash',
    },
    {
      'accountId': '22222222-2222-2222-2222-222222222223',
      'accountName': 'Vadesiz',
      'balance': '1500.0000',
      'type': 'bank',
    },
  ],
});

AdvancedReport _advanced({
  String receivableDebt = '340.0000',
  String payableDebt = '120.0000',
  String netWorth = '2370.0000',
  String moneyInTransit = '0.0000',
}) => AdvancedReport.fromJson({
  'asOfDate': '2026-08-31',
  'currency': 'TRY',
  // Net varlık dört terimden geliyor: 2400 - 250 + 340 - 120 = 2370.
  // Tutarlar ekranın başka yerindekilerle çakışmayacak biçimde seçildi;
  // Ekranın başka yerlerindeki tutarlarla çakışmayan değerler seçildi:
  // `100.0000` ve `300.0000` kategori efsanesinde, `2000.0000` gelir
  // kutusunda zaten görünüyor ve `find.text` iki eşleşme buluyordu.
  // Fixture bilerek dördünü de doldurur; üçüyle yazılsaydı kartın dökümünün
  // toplamı açıklayıp açıklamadığı sınanamazdı.
  'netWorth': {
    'liquidAssets': '2400.0000',
    'creditCardDebt': '250.0000',
    'receivableDebt': receivableDebt,
    'payableDebt': payableDebt,
    'netWorth': netWorth,
    'moneyInTransit': moneyInTransit,
  },
  'periodComparison': {
    'current': {
      'year': 2026,
      'month': 8,
      'income': '2000.0000',
      'expense': '400.0000',
      'net': '1600.0000',
    },
    'previous': {
      'year': 2026,
      'month': 7,
      'income': '1800.0000',
      'expense': '600.0000',
      'net': '1200.0000',
    },
  },
  'cashFlowTrend': [
    for (var month = 3; month <= 8; month++)
      {
        'year': 2026,
        'month': month,
        'income': '${1000 + month * 100}.0000',
        'expense': '${200 + month * 20}.0000',
        'net': '${800 + month * 80}.0000',
      },
  ],
  'budgetVariances': [
    // Aşan bütçede `remaining` negatiftir ve aşım tutarı odur; sunucu
    // `limit - spent` hesabını kendisi yapar.
    {
      'categoryName': 'groceries',
      'limit': '250.0000',
      'spent': '300.0000',
      'remaining': '-50.0000',
      'isExceeded': true,
    },
    // Aşmayan bütçe: bölümün bütün bütçeleri hesaba kattığını sınamak için.
    {
      'categoryName': 'transport',
      'limit': '400.0000',
      'spent': '120.0000',
      'remaining': '280.0000',
      'isExceeded': false,
    },
  ],
  'futureLoad': {
    'recurringAmount': '0.0000',
    'creditCardStatementAmount': '0.0000',
    'installmentAmount': '0.0000',
    'debtInstallmentAmount': '0.0000',
    'totalAmount': '0.0000',
  },
  'accountDistribution': const <Map<String, dynamic>>[],
  'cardDistribution': const <Map<String, dynamic>>[],
});

class _Source implements DashboardDataSource {
  _Source(this.report, {this.advanced});

  final DashboardReport report;
  final AdvancedReport? advanced;

  @override
  Future<DashboardReport> getMonthly(
    int year,
    int month, {
    TransactionScope? scope,
  }) async => report;

  @override
  Future<AdvancedReport> getAdvanced(
    int year,
    int month, {
    TransactionScope? scope,
  }) async {
    final value = advanced;
    if (value == null) {
      throw Exception('gelişmiş rapor kullanılamıyor');
    }
    return value;
  }
}

/// Planlanan projeksiyonun sahtesi.
class _PlannedSource implements ActivityRepositoryContract {
  _PlannedSource(this.items);
  _PlannedSource.failing() : items = null;

  final List<PlannedActivity>? items;

  @override
  Future<PlannedActivityPage> listPlanned({
    required PlannedHorizon horizon,
    DateTime? today,
    TransactionScope? scope,
  }) async {
    final loaded = items;
    if (loaded == null) throw Exception('planlanan okunamadı');
    return PlannedActivityPage(
      asOfDate: '2026-08-09',
      daysAhead: horizon.days,
      totalCount: loaded.length,
      items: loaded,
    );
  }

  @override
  Future<ActivityPage> list({
    int pageNumber = 1,
    int pageSize = 20,
    ActivityFilter filter = const ActivityFilter(),
    DateTime? today,
    TransactionScope? scope,
  }) => throw UnimplementedError();

  @override
  Future<void> cancel(FinancialActivity activity) => throw UnimplementedError();

  @override
  Future<void> realizePlanned(PlannedActivity activity) =>
      throw UnimplementedError();
}

PlannedActivity _planned({
  required String id,
  required String timing,
  String dueDate = '2026-08-01',
  String amount = '500.0000',
  bool isPaymentObligation = true,
  String kind = 'card-installment',
  String action = 'realize',
}) => PlannedActivity.fromJson({
  'plannedActivityId': id,
  'plannedKind': kind,
  'effect': 'expense',
  'timing': timing,
  'readiness': 'ready',
  'attentionCode': null,
  'actionKind': action,
  'dueDate': dueDate,
  'amount': amount,
  'currency': 'TRY',
  'title': 'Buzdolabı 6 taksit',
  'description': null,
  'sourceId': null,
  'sourceName': 'Bankamatik Kart',
  'categoryId': null,
  'categoryName': null,
  'isProjected': false,
  'isPaymentObligation': isPaymentObligation,
});
