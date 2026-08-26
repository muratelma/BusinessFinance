import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_client.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/core/theme/app_theme.dart';
import 'package:business_finance_mobile/core/widgets/app_state_views.dart';
import 'package:business_finance_mobile/features/planning/presentation/recurring_prefill.dart';
import 'package:business_finance_mobile/features/taxes/data/tax_models.dart';
import 'package:business_finance_mobile/features/taxes/data/tax_repository.dart';
import 'package:business_finance_mobile/features/taxes/presentation/accountant_package_page.dart';
import 'package:business_finance_mobile/features/taxes/presentation/tax_calendar_page.dart';
import 'package:business_finance_mobile/features/taxes/presentation/tax_controller.dart';

/// Aşama 05 Grup 7: vergi takvimi ve muhasebeci paketi ekranları.
void main() {
  group('vergi takvimi', () {
    testWidgets('mevzuat takibi yapılmadığı ekranda yazılı', (tester) async {
      await _pumpCalendar(tester, _FakeTaxRepository());

      expect(
        find.textContaining('Uygulama mevzuat takibi yapmaz'),
        findsOneWidget,
      );
      expect(find.textContaining('Öneriler tutar taşımaz'), findsOneWidget);
    });

    testWidgets('hazır kalemler ritmiyle listelenir', (tester) async {
      await _pumpCalendar(tester, _FakeTaxRepository());

      expect(find.text('KDV beyanı'), findsOneWidget);
      expect(find.text('Her ay • ayın 28. günü'), findsOneWidget);
      expect(find.text('Geçici vergi'), findsOneWidget);
      expect(find.text('Üç ayda bir • ayın 17. günü'), findsOneWidget);
    });

    testWidgets('kaleme dokunmak kurulum yolunu açar', (tester) async {
      final installed = <TaxCalendarSuggestion>[];
      await _pumpCalendar(
        tester,
        _FakeTaxRepository(),
        onInstall: installed.add,
      );

      await tester.tap(find.text('KDV beyanı'));
      await tester.pumpAndSettle();

      expect(installed.single.key, 'vat-return');
    });

    testWidgets('oturum düştüğünde ekran bunu söyler', (tester) async {
      await _pumpCalendar(tester, _FakeTaxRepository(unauthorized: true));

      expect(find.byType(AppUnauthorizedView), findsOneWidget);
    });
  });

  group('takvim ön dolumu', () {
    test('önerilen gün bu ay geçtiyse gelecek aya kurulur', () {
      final prefill = RecurringPrefill.fromSuggestion(
        label: 'KDV beyanı',
        frequency: 'monthly',
        dayOfMonth: 28,
        categoryName: 'SGK ve vergi ödemesi',
        today: DateTime(2026, 8, 29),
      );

      // Geçmişe kurmak, ilk gerçekleşmeyi daha kurulurken gecikmiş yapardı.
      expect(prefill.startDate, DateTime(2026, 9, 28));
      expect(prefill.description, 'KDV beyanı');
      expect(prefill.kind, 'bill-payment');
    });

    test('gün henüz gelmediyse bu ay kurulur', () {
      final prefill = RecurringPrefill.fromSuggestion(
        label: 'Geçici vergi',
        frequency: 'quarterly',
        dayOfMonth: 17,
        categoryName: 'SGK ve vergi ödemesi',
        today: DateTime(2026, 8, 3),
      );

      expect(prefill.startDate, DateTime(2026, 8, 17));
      expect(prefill.frequency, 'quarterly');
    });
  });

  group('muhasebeci paketi', () {
    testWidgets('toplamlar ve içerik sunucudan geldiği gibi gösterilir', (
      tester,
    ) async {
      await _pumpPackage(tester, _FakeTaxRepository());

      expect(find.text('Temmuz 2026'), findsOneWidget);
      expect(find.text('12 kayıt'), findsOneWidget);
      expect(find.text('3 belge'), findsOneWidget);
      expect(find.text('2 kayıtta KDV yazılmamış'), findsOneWidget);
      expect(
        find.text('1 giderde indirilebilirlik cevaplanmamış'),
        findsOneWidget,
      );
    });

    testWidgets('şahsi kaydın pakete girmediği ekranda yazılı', (tester) async {
      await _pumpPackage(tester, _FakeTaxRepository());

      expect(find.textContaining('şahsi hiçbir kayıt girmez'), findsOneWidget);
    });

    testWidgets('paylaş dosyayı indirir ve adını dönemden kurar', (
      tester,
    ) async {
      final shared = <String>[];
      await _pumpPackage(
        tester,
        _FakeTaxRepository(),
        share: (bytes, name) async => shared.add(name),
      );

      await tester.tap(find.text('Paketi paylaş'));
      await tester.pumpAndSettle();

      expect(shared.single, 'muhasebeci-paketi-2026-07.zip');
    });

    testWidgets('boş ayda paket boş olduğunu söyler', (tester) async {
      await _pumpPackage(tester, _FakeTaxRepository(emptyMonth: true));

      expect(
        find.textContaining('Bu ayda işletme kapsamlı kayıt yok'),
        findsOneWidget,
      );
    });

    testWidgets('sığmayan belge sessizce düşmez', (tester) async {
      await _pumpPackage(tester, _FakeTaxRepository(omittedAttachment: true));

      expect(
        find.textContaining('boyut sınırını aştığı için dosyaya konmadı'),
        findsOneWidget,
      );
    });
  });
}

Future<void> _pumpCalendar(
  WidgetTester tester,
  TaxRepositoryContract repository, {
  void Function(TaxCalendarSuggestion)? onInstall,
}) async {
  tester.view.physicalSize = const Size(500, 1400);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: TaxCalendarPage(
        controller: TaxCalendarController(repository),
        onInstall: onInstall,
      ),
    ),
  );
  await tester.pumpAndSettle();
}

Future<void> _pumpPackage(
  WidgetTester tester,
  TaxRepositoryContract repository, {
  Future<void> Function(Uint8List, String)? share,
}) async {
  tester.view.physicalSize = const Size(500, 2000);
  tester.view.devicePixelRatio = 1;
  addTearDown(tester.view.reset);
  await tester.pumpWidget(
    MaterialApp(
      theme: AppTheme.light(),
      home: AccountantPackagePage(
        controller: AccountantPackageController(
          repository,
          today: DateTime(2026, 8, 26),
        ),
        share: share,
      ),
    ),
  );
  await tester.pumpAndSettle();
}

class _FakeTaxRepository implements TaxRepositoryContract {
  _FakeTaxRepository({
    this.unauthorized = false,
    this.emptyMonth = false,
    this.omittedAttachment = false,
  });

  final bool unauthorized;
  final bool emptyMonth;
  final bool omittedAttachment;

  @override
  Future<List<TaxCalendarSuggestion>> loadSuggestions() async {
    if (unauthorized) {
      throw const ApiException(
        code: 'authentication.required',
        message: 'Oturum sona erdi.',
        statusCode: 401,
      );
    }
    return const [
      TaxCalendarSuggestion(
        key: 'vat-return',
        frequency: 'monthly',
        suggestedDayOfMonth: 28,
        suggestedCategoryName: 'SGK ve vergi ödemesi',
        kind: 'expense',
        scope: 'business',
      ),
      TaxCalendarSuggestion(
        key: 'advance-tax',
        frequency: 'quarterly',
        suggestedDayOfMonth: 17,
        suggestedCategoryName: 'SGK ve vergi ödemesi',
        kind: 'expense',
        scope: 'business',
      ),
    ];
  }

  @override
  Future<AccountantPackage> loadPackage(int year, int month) async =>
      AccountantPackage.fromJson({
        'year': year,
        'month': month,
        'currency': 'TRY',
        'totalIncome': emptyMonth ? '0.0000' : '9400.0000',
        'totalExpense': emptyMonth ? '0.0000' : '3450.5000',
        'net': emptyMonth ? '0.0000' : '5949.5000',
        'vatOnIncome': '180.0000',
        'vatOnExpense': '20.0000',
        'linesWithoutVat': 2,
        'nonDeductibleExpense': '500.0000',
        'nonDeductibleCount': 1,
        'deductibilityUnansweredCount': 1,
        'lines': emptyMonth
            ? <Map<String, dynamic>>[]
            : List.generate(12, (index) => <String, dynamic>{'id': '$index'}),
        'attachments': emptyMonth
            ? <Map<String, dynamic>>[]
            : [
                {'isIncluded': true},
                {'isIncluded': true},
                {'isIncluded': !omittedAttachment},
              ],
      });

  @override
  Future<ApiBinaryResponse> downloadPackage(int year, int month) async =>
      const ApiBinaryResponse(bytes: [1, 2, 3], contentType: 'application/zip');
}
