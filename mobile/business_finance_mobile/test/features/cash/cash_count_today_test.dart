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

  Map<String, dynamic> count({
    String? status,
    String? transaction,
    String? transfer,
  }) => {
    'id': 'count',
    'accountId': 'cash-account',
    'accountName': 'Merkez kasa',
    'countDate': '2026-10-08',
    'countedAmount': '900.0000',
    'currency': 'TRY',
    'isCancelled': false,
    'adjustmentTransactionId': transaction,
    'adjustmentTransferId': transfer,
    'adjustmentStatus': ?status,
  };

  // "Fark kaydedildi" demek için kaydın **durması** gerekir: iptal edilmiş
  // kayıt sayımı kaydedilmiş saymaz. Aktarımla açıklanan fark da kaydedilmiştir.
  test(
    'a count is adjusted only while the record of its difference stands',
    () {
      final byTransfer = CashCountItem.fromJson(
        count(status: 'recorded', transfer: 'transfer'),
      );
      expect(byTransfer.isAdjusted, isTrue);
      expect(byTransfer.adjustmentTransferId, 'transfer');

      final cancelled = CashCountItem.fromJson(
        count(status: 'cancelled', transaction: 'transaction'),
      );
      expect(cancelled.isAdjusted, isFalse);
      expect(cancelled.adjustmentCancelled, isTrue);

      final open = CashCountItem.fromJson(count(status: 'none'));
      expect(open.isAdjusted, isFalse);
      expect(open.adjustmentCancelled, isFalse);

      // Alanı göndermeyen eski sunucu: bağ varsa kaydedilmiş sayılır.
      expect(
        CashCountItem.fromJson(count(transaction: 'transaction')).isAdjusted,
        isTrue,
      );
      expect(CashCountItem.fromJson(count()).isAdjusted, isFalse);
    },
  );
}
