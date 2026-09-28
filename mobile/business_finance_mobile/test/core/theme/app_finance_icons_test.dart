import 'package:business_finance_mobile/core/theme/app_finance_icons.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('varsayılan setteki Yeme-içme kategorisi yemek ikonunu alır', () {
    expect(
      AppFinanceIcons.forCategory('Yeme-içme', displayName: 'Yeme-içme'),
      Icons.restaurant_outlined,
    );
    expect(
      AppFinanceIcons.forCategory('Yemek', displayName: 'Yemek'),
      Icons.restaurant_outlined,
    );
  });

  test('ödeme içeren ad yemek ikonuna düşmez', () {
    expect(
      AppFinanceIcons.forCategory(
        'SGK ve vergi ödemesi',
        displayName: 'SGK ve vergi ödemesi',
      ),
      isNot(Icons.restaurant_outlined),
    );
  });
}
