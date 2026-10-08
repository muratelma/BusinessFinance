import 'package:business_finance_mobile/features/cash/data/cash_repository.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  Map<String, dynamic> today({Object? requiresRecount}) => {
    'accountId': 'cash-account',
    'accountName': 'Merkez kasa',
    'expectedBalance': '1200.0000',
    'currency': 'TRY',
    'count': null,
    'previousCount': null,
    'todayInflow': '200.0000',
    'todayOutflow': '0.0000',
    'requiresRecount': ?requiresRecount,
  };

  // Sunucu "yeniden sayın" dediğinde ekran farkı kaydettirmez; alanı
  // göndermeyen eski sunucuda davranış değişmez.
  test('the today response carries whether the cash must be counted again', () {
    expect(
      CashCountToday.fromJson(today(requiresRecount: true)).requiresRecount,
      isTrue,
    );
    expect(
      CashCountToday.fromJson(today(requiresRecount: false)).requiresRecount,
      isFalse,
    );
    expect(CashCountToday.fromJson(today()).requiresRecount, isFalse);
  });
}
