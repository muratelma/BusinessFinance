import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/localization/default_category_labels.dart';

void main() {
  test('localizes only known legacy default category names', () {
    expect(DefaultCategoryLabels.localized('Groceries'), 'Market Alışverişi');
    expect(DefaultCategoryLabels.localized('salary'), 'Maaş');
    expect(DefaultCategoryLabels.localized('Evcil Hayvan'), 'Evcil Hayvan');
  });
}
