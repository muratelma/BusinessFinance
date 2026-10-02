import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/features/activities/data/activity_models.dart';

void main() {
  group('FinancialActivity parsing', () {
    test('her hareket türü kendi kablo değerinden geri okunabilir', () {
      // Bu kapı bir üretim hatasından sonra yazıldı: `debt-opening` sunucuda
      // enum'a eklendi ama kablo değeri eşlemesine yazılmadı ve İşlemler
      // sayfası `KeyNotFoundException` ile açılmaz oldu. İstemcide karşılığı
      // `fromApi`: bilinmeyen değer `FormatException` atar, yani eksik bir
      // eşleme yine sayfayı boş bırakır.
      //
      // Etiket ve ikon tarafını Dart derleyicisi zaten koruyor — enum
      // switch'leri tam kapsamlı olmak zorunda. Korunmayan tek yer bu.
      for (final kind in ActivityKind.values) {
        expect(
          ActivityKind.fromApi(kind.apiValue),
          kind,
          reason: '${kind.name} kablo değerinden geri okunamıyor.',
        );
        expect(kind.label, isNotEmpty);
      }

      expect(
        ActivityKind.values.map((kind) => kind.apiValue).toSet().length,
        ActivityKind.values.length,
        reason: 'İki tür aynı kablo değerini paylaşıyor.',
      );
    });

    // Yukarıdaki döngü istemcinin **kendi** listesini gezer: sunucunun
    // gönderdiği ama istemcide hiç tanımlanmamış bir tür ona görünmez. Tam
    // olarak bu oldu — `obligation-settlement` sunucuda vardı, burada yoktu ve
    // yükümlülüğünü ödeyen kullanıcının İşlemler sayfası `FormatException` ile
    // açılmaz oluyordu. Bu liste sunucunun bugün gönderdiği değerleri sabit
    // tutar; sunucuya yeni bir tür eklenip buraya yazılmazsa test düşer.
    test('sunucunun gönderdiği her kablo değeri istemcide karşılanır', () {
      const serverKinds = {
        'account-transaction',
        'transfer',
        'card-charge',
        'card-payment',
        'debt-payment',
        'debt-collection',
        'debt-opening',
        'counterparty-charge',
        'counterparty-settlement',
        'obligation',
        'obligation-settlement',
        'pos-sale',
        'pos-deposit',
      };

      expect(
        ActivityKind.values.map((kind) => kind.apiValue).toSet(),
        serverKinds,
      );
      for (final value in serverKinds) {
        expect(() => ActivityKind.fromApi(value), returnsNormally);
      }

      const serverGroups = {
        'account',
        'credit-card',
        'transfer',
        'debt',
        'counterparty',
        'obligation',
        'pos',
      };
      expect(
        ActivitySourceGroup.values.map((group) => group.apiValue).toSet(),
        serverGroups,
      );
    });

    test('reads every classification dimension and capability', () {
      final activity = FinancialActivity.fromJson(_json());

      expect(activity.kind, ActivityKind.cardCharge);
      expect(activity.effect, ActivityEffect.expense);
      expect(activity.sourceGroup, ActivitySourceGroup.creditCard);
      expect(activity.origin, ActivityOrigin.installment);
      expect(activity.status, ActivityStatus.realized);
      // Money stays the exact string the server sent; the client never recomputes it.
      expect(activity.amount, '625.5000');
      expect(activity.canCancel, isFalse);
      expect(activity.supportsAttachments, isFalse);
    });

    test(
      'keys a row by kind and id, because ids repeat across write models',
      () {
        final charge = FinancialActivity.fromJson(_json());
        final transfer = FinancialActivity.fromJson(
          _json(
            activityKind: 'transfer',
            effect: 'neutral',
            sourceGroup: 'transfer',
          ),
        );

        expect(charge.listKey, isNot(transfer.listKey));
        expect(charge.listKey, 'card-charge:${charge.activityId}');
      },
    );

    test('her köken değerini okur; yatış kesintisi kendi kökenidir', () {
      const serverOrigins = {
        'manual',
        'csv-import',
        'recurring',
        'installment',
        'pos-deposit',
      };

      expect(
        ActivityOrigin.values.map((origin) => origin.apiValue).toSet(),
        serverOrigins,
      );
      expect(ActivityOrigin.fromApi('pos-deposit').label, 'Yatış kesintisi');
      expect(ActivityKind.fromApi('pos-deposit'), ActivityKind.posDeposit);
    });

    test('rejects an unknown enum value instead of guessing', () {
      expect(
        () => FinancialActivity.fromJson(_json(activityKind: 'crypto-swap')),
        throwsFormatException,
      );
      expect(
        () => FinancialActivity.fromJson(_json(effect: 'refund')),
        throwsFormatException,
      );
      expect(
        () => FinancialActivity.fromJson(_json(origin: 'open-banking')),
        throwsFormatException,
      );
    });

    test('rejects a malformed payload rather than showing a wrong amount', () {
      final broken = _json()..remove('amount');
      expect(
        () => FinancialActivity.fromJson(broken),
        throwsA(isA<TypeError>()),
      );
    });
  });

  group('quick filters', () {
    test('map to source group or origin, which are independent dimensions', () {
      expect(ActivityQuickFilter.all.sourceGroup, isNull);
      expect(ActivityQuickFilter.all.origin, isNull);
      expect(
        ActivityQuickFilter.accounts.sourceGroup,
        ActivitySourceGroup.account,
      );
      expect(ActivityQuickFilter.accounts.origin, isNull);
      // Recurring narrows the origin, not the source: a recurring movement can
      // come from an account or a card.
      expect(ActivityQuickFilter.recurring.sourceGroup, isNull);
      expect(ActivityQuickFilter.recurring.origin, ActivityOrigin.recurring);
    });
  });

  group('date ranges', () {
    final today = DateTime(2026, 8, 14);

    test('default sends no lower bound so history stays reachable', () {
      expect(ActivityDateRange.all.dateFrom(today), isNull);
    });

    test('named ranges start at the first day of their earliest month', () {
      expect(ActivityDateRange.thisMonth.dateFrom(today), '2026-08-01');
      expect(ActivityDateRange.lastThreeMonths.dateFrom(today), '2026-06-01');
      expect(ActivityDateRange.lastTwelveMonths.dateFrom(today), '2025-09-01');
    });

    test('crosses the year boundary correctly', () {
      expect(
        ActivityDateRange.lastThreeMonths.dateFrom(DateTime(2026, 1, 20)),
        '2025-11-01',
      );
    });
  });

  group('ActivityFilter', () {
    test('reports advanced filters only when one is actually set', () {
      const plain = ActivityFilter();
      expect(plain.hasAdvancedFilters, isFalse);
      // A quick chip is not an advanced filter: it has its own visible control.
      expect(
        plain
            .copyWith(quickFilter: ActivityQuickFilter.accounts)
            .hasAdvancedFilters,
        isFalse,
      );
      expect(
        plain
            .copyWith(dateRange: ActivityDateRange.thisMonth)
            .hasAdvancedFilters,
        isTrue,
      );
      expect(
        plain.copyWith(includeCancelled: false).hasAdvancedFilters,
        isTrue,
      );
    });

    test('clears a value explicitly, since null alone means "unchanged"', () {
      const filter = ActivityFilter(effect: ActivityEffect.income);
      expect(filter.copyWith().effect, ActivityEffect.income);
      expect(filter.copyWith(clearEffect: true).effect, isNull);
    });
  });

  group('borç taksidinin anapara/faiz kırılımı', () {
    test('sözleşme her iki payı da taşır, istemci çıkarma yapmaz', () {
      // Faiz ayrı bir hareket değil, bu hareketin bölünmesi. Ayrı satır
      // olsaydı işlemler toplamı hesaptan çıkan parayla tutmazdı.
      final activity = FinancialActivity.fromJson({
        ..._json(
          activityKind: 'debt-payment',
          effect: 'neutral',
          sourceGroup: 'debt',
          origin: 'manual',
        ),
        'amount': '2600.0000',
        'principalPortion': '2500.0000',
        'interestPortion': '100.0000',
      });

      expect(activity.amount, '2600.0000');
      expect(activity.principalPortion, '2500.0000');
      expect(activity.interestPortion, '100.0000');
    });

    test('borç dışı hareketlerde iki pay da boştur', () {
      final activity = FinancialActivity.fromJson(_json());

      expect(activity.principalPortion, isNull);
      expect(activity.interestPortion, isNull);
    });
  });
}

Map<String, dynamic> _json({
  String activityKind = 'card-charge',
  String effect = 'expense',
  String sourceGroup = 'credit-card',
  String origin = 'installment',
  String status = 'realized',
}) => <String, dynamic>{
  'activityId': '8f1c6d2a-4b7e-4a51-9c33-2d5e8a7b1f04',
  'activityKind': activityKind,
  'effect': effect,
  'sourceGroup': sourceGroup,
  'origin': origin,
  'status': status,
  'activityDate': '2026-08-14',
  'amount': '625.5000',
  'currency': 'TRY',
  'title': 'Market',
  'description': 'Haftalık alışveriş',
  'categoryId': '3a9b0c11-5d2f-4e88-b7a6-0c1d4e5f6a72',
  'categoryName': 'Groceries',
  'sourceId': 'c7e2f4a8-9b13-4d6e-8f52-1a3b5c7d9e01',
  'sourceName': 'Test Kart',
  'destinationId': null,
  'destinationName': null,
  'cancelledAtUtc': null,
  'principalPortion': null,
  'interestPortion': null,
  'canCancel': false,
  'supportsAttachments': false,
};
