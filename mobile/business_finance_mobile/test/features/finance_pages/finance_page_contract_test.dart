import 'dart:convert';
import 'dart:io';
import 'package:business_finance_mobile/features/finance_pages/data/finance_page_models.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // 5291'de güncel Release API'nin yalnız sentetik kullanıcıyla ürettiği cevaplar.
  final data =
      jsonDecode(
            File(
              'test/fixtures/category-account-contract.json',
            ).readAsStringSync(),
          )
          as Map<String, dynamic>;
  test('gerçek API kategori ve parça cevapları açık modellere okunur', () {
    expect(data['synthetic'], true);
    expect(CategorySummaryData.fromJson(data['summary']).total, '26930.0000');
    expect(CategorySummaryData.fromJson(data['income']).total, '44550.0000');
    expect(
      CategoryPageData.fromJson(data['category']).wholeTotal,
      '12480.0000',
    );
    expect(
      CategoryPageData.fromJson(
        data['bill'],
      ).items.any((i) => i.state == 'settled'),
      true,
    );
    final parts = CategoryPageData.fromJson(data['commissions']).items;
    expect(parts.where((i) => i.part != null).map((i) => i.part).toSet(), {
      'fee',
      'commission',
      'deduction',
    });
  });
  test(
    'gerçek API hesap, yolda, ay, masraf önizlemesi ve ayrıntısı okunur',
    () {
      expect(AccountDetailData.fromJson(data['account']).net, '-3624.0000');
      expect(
        AccountDetailData.fromJson(data['cashAccount']).inflow,
        '16800.0000',
      );
      expect(TransitPageData.fromJson(data['transit']).total, '8074.0000');
      expect(
        AccountsOverviewData.fromJson(data['accounts']).total,
        '48330.0000',
      );
      expect(FinanceYearMonths.fromJson(data['months']).months.length, 12);
      expect(
        TransferPreviewData.fromJson(data['preview']).totalOutflow,
        '10050.0000',
      );
      expect(ActivityPartData.fromJson(data['parts']['items'][0]).kind, 'fee');
    },
  );
}
