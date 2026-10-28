import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/widgets/app_category_field.dart';
import 'package:flutter_test/flutter_test.dart';

/// Kategori alanı: [label] verilmezse sayfadaki ilk alan.
Finder categoryField([String? label]) => label == null
    ? find.byType(AppCategoryField).first
    : find
          .ancestor(
            of: find.text(label),
            matching: find.byType(AppCategoryField),
          )
          .first;

/// Açık menüdeki satır. Menü alanın içinde, kapalı değerin altında çizilir;
/// bu yüzden alanın içindeki son eşleşme menü satırıdır.
Finder categoryMenuRow(String name, {String? label}) =>
    find.descendant(of: categoryField(label), matching: find.text(name)).last;

/// Alanı açar ve [name] satırını seçer. İki kapsama açık kategoride satır
/// açılır ve [scope] satırı seçilir.
Future<void> chooseCategory(
  WidgetTester tester,
  String name, {
  TransactionScope? scope,
  String? label,
}) async {
  await tester.tap(categoryField(label));
  await tester.pumpAndSettle();
  await tester.tap(categoryMenuRow(name, label: label));
  await tester.pumpAndSettle();
  if (scope == null) return;
  await tester.tap(categoryMenuRow(scope.label, label: label));
  await tester.pumpAndSettle();
}
